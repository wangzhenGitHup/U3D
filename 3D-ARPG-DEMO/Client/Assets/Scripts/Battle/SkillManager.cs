/****************************************************
    文件：SkillManager.cs
	作者：WangZhen
    日期：2019/6/22 23:34:9
	功能：技能管理
*****************************************************/

using UnityEngine;
using WZNet;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour 
{
    ResSvc resSvc;
    TimerSvc timeSvc;

    public void Init()
    {
        CommonTools.Log("SkillManager Init.....");
        resSvc = ResSvc.Instance;
        timeSvc = TimerSvc.Instance;
    }

    public void AttackEffect(EntityBase entity, int skillId)
    {
        SkillDataCfg skillData = resSvc.GetSkillCfg(skillId);

        //判断技能能不能碰撞
        if (!skillData.bCollide)
        {
            //忽略掉刚体碰撞
            Physics.IgnoreLayerCollision(Constants.cLayerPlayer,  Constants.cLayerMonster);
            timeSvc.AddTimeTask((int tid) =>
            {
                //技能播放完给恢复
                Physics.IgnoreLayerCollision(Constants.cLayerPlayer, Constants.cLayerMonster, false);
            }, skillData.skillTime);
        }

        if (entity._entityType == Constants.EntityType.Player)
        {
            if (entity.GetCurrentDirection() == Vector2.zero)
            {
                //搜索最近的怪物
                Vector2 dir = entity.CalcTargetDirection();
                if (dir != Vector2.zero)
                {
                    entity.SetAttackDirection(dir);
                }
            }
            else
            {
                entity.SetAttackDirection(entity.GetCurrentDirection(), true);
            }
        }

        entity.SetAction(skillData.aniAction);
        entity.SetFx(skillData.fx, skillData.skillTime);

        skillMoveLogical(entity, skillData);

        entity._bCanControl = false;
        entity.SetDirection(Vector2.zero);

        if (!skillData.bBreak)
        {
            entity._entityState = Constants.EntitiyState.NoEffectHit;
        }

        entity._skillEndCallbackID = timeSvc.AddTimeTask((int tid) =>
            {
                entity.Idle();
            }, skillData.skillTime);
    }

    public void AttackDamage(EntityBase entity, int skillId)
    {
        entity._listSkillDamageCallback.Clear();
        entity._listSkillMoveCallback.Clear();

        SkillDataCfg skillData = resSvc.GetSkillCfg(skillId);
        List<int> listSkillAction = skillData.skillActionList;
        int accumulate = 0;

        for (int idx = 0; idx < listSkillAction.Count; idx++)
        {
            SkillActionCfg actionCfg = resSvc.GetSkillActionCfg(listSkillAction[idx]);
            accumulate += actionCfg.delayTime;
            int skillIdx = idx;
            if (accumulate > 0)
            {
                int actionId = -1;
                actionId = timeSvc.AddTimeTask((int tid) =>
                {
                    if (entity != null)
                    {
                        SelectDamageObj(entity, skillData, skillIdx);
                        entity.RemoveDamageCallbackID(actionId);
                    }
                }, accumulate);
                entity._listSkillDamageCallback.Add(actionId);
            }
            else
            {
                SelectDamageObj(entity, skillData, skillIdx);
            }
        }
    }

    private void SelectDamageObj(EntityBase attacker, SkillDataCfg skillCfg, int skillIdx)
    {
        SkillActionCfg actionCfg = resSvc.GetSkillActionCfg(skillCfg.skillActionList[skillIdx]);
        int damageVal = skillCfg.skillDamageList[skillIdx];

        if (attacker._entityType == Constants.EntityType.Monster)
        {
            EntityPlayer entityPlayer = attacker._battleMgr.GetEntityPlayer;
            if (entityPlayer == null)
            {
                return;
            }

            if (InRange(attacker.GetEntityPosition(), entityPlayer.GetEntityPosition(), actionCfg.radius) &&
                    InAngle(attacker.GetEntityTransform(), entityPlayer.GetEntityPosition(), actionCfg.angle))
            {
                CalcDamage(attacker, entityPlayer, skillCfg, damageVal);
            }
        }
        else if (attacker._entityType == Constants.EntityType.Player)
        {
            //获取场景中所有的怪物
            List<EntityMonster> listMonster = attacker._battleMgr.GetAllEntityMonsters();
            for (int i = 0; i < listMonster.Count; i++)
            {
                EntityMonster monster = listMonster[i];
                //在攻击范围内
                if (InRange(attacker.GetEntityPosition(), monster.GetEntityPosition(), actionCfg.radius) &&
                    InAngle(attacker.GetEntityTransform(), monster.GetEntityPosition(), actionCfg.angle))
                {
                    CalcDamage(attacker, monster, skillCfg, damageVal);
                }
            }
        }
    }

    private void CalcDamage(EntityBase attacker, EntityBase patient, SkillDataCfg skillCfg, int damageVal)
    {
        int finalDamageVal = damageVal;
        if (skillCfg.damageType == Constants.DamageType.DamageType_AD)
        {
            System.Random rd = new System.Random();
            //闪避
            int dodgeVal = CommonTools.RandomInt(1, 100, rd);
            if (dodgeVal <= patient.BattleProps.dodge)
            {
                patient.SetDodge();
                return;
            }

            //基础属性
            finalDamageVal += attacker.BattleProps.ad;

            //暴击
            int criticalVal = CommonTools.RandomInt(1, 100, rd);
            if (criticalVal <= patient.BattleProps.critical)
            {
                float criticalRate = 1.0f + (CommonTools.RandomInt(1, 100) / 100.0f);
                finalDamageVal = (int)criticalRate * finalDamageVal;
                patient.SetCritical(finalDamageVal);
            }

            //穿甲
            int addefVal = (int)((1.0f - attacker.BattleProps.pierce / 100.0f) * patient.BattleProps.addef);
            finalDamageVal -= addefVal;
        }
        else if (skillCfg.damageType == Constants.DamageType.DamageType_AP)
        {
            //基础属性
            finalDamageVal += patient.BattleProps.ap;
            //魔法抗性
            finalDamageVal -= patient.BattleProps.apdef;
        }

        finalDamageVal = finalDamageVal < 0 ? 0 : finalDamageVal;
        if (finalDamageVal == 0)
        {
            return;
        }

        patient.SetHurt(finalDamageVal);

        if (patient.HP < finalDamageVal)
        {
            patient.HP = 0;
            patient.Die();
            if (patient._entityType == Constants.EntityType.Monster)
            {
                patient._battleMgr.RemoveMonsterData(patient.EntityName);
            }
            else if (patient._entityType == Constants.EntityType.Player)
            {
                patient._battleMgr.FinishBattle(false, 0);
                patient._battleMgr.GetEntityPlayer = null;
            }
        }
        else
        {
            patient.HP -= finalDamageVal;
            if (patient._entityState == Constants.EntitiyState.None && patient.CanBeBreak())
            {
                patient.Hit();
            }
        }
    }

    private bool InRange(Vector3 vecFrom, Vector3 vecTo, float range)
    {
        float dis = Vector3.Distance(vecFrom, vecTo);
        return dis <= range;
    }

    private bool InAngle(Transform trans, Vector3 vecTo, float angle)
    {
        if (angle == 360)
        {
            return true;
        }

        Vector3 forwardVec = trans.forward;
        Vector3 targetDir = (vecTo - trans.position).normalized;
        float fAngle = Vector3.Angle(forwardVec, targetDir);
        if (fAngle <= angle / 2)
        {
            return true;
        }

        return false;
    }

    private void skillMoveLogical(EntityBase entity, SkillDataCfg skillData)
    {
        int totalDelayTime = 0;
        List<int> skillMoveList = skillData.skillMoveList;
        for (int i = 0; i < skillMoveList.Count; i++)
        {
            SkillMoveCfg skillMoveCfg = resSvc.GetSkillMoveCfg(skillMoveList[i]);
            float spd = skillMoveCfg.moveDistance / (skillMoveCfg.moveTime / 1000.0f);
            totalDelayTime += skillMoveCfg.delayTime;

            if (totalDelayTime > 0)
            {
                int moveCallbackId = -1;
                moveCallbackId = timeSvc.AddTimeTask((int tid) => { 
                    entity.SetSkillMoveState(true, spd);
                    entity.RemoveMoveCallbackID(moveCallbackId);
                }, totalDelayTime);
                entity._listSkillMoveCallback.Add(moveCallbackId);
            }
            else
            {
                entity.SetSkillMoveState(true, spd);
            }

            totalDelayTime += skillMoveCfg.moveTime;

            int stopSkillMoveCallbackID = -1;
            stopSkillMoveCallbackID = timeSvc.AddTimeTask((int tid) =>
            {
                entity.SetSkillMoveState(false);
                entity.RemoveMoveCallbackID(stopSkillMoveCallbackID);
            }, totalDelayTime);
            entity._listSkillMoveCallback.Add(stopSkillMoveCallbackID);
        }
    }
}