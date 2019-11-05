/****************************************************
    文件：EntityPlayer.cs
	作者：WangZhen
    日期：2019/6/23 22:52:56
	功能：玩家逻辑实体
*****************************************************/

using UnityEngine;
using System.Collections.Generic;


public class EntityPlayer : EntityBase 
{
    public EntityPlayer()
    {
        _entityType = Constants.EntityType.Player;
    }

    public override Vector2 GetCurrentDirection()
    {
        return _battleMgr.GetCurrentDirection();
    }

    public override Vector2 CalcTargetDirection()
    {
        EntityMonster monster = FindNearestTarget();
        if (monster != null)
        {
            Vector3 targetMonster = monster.GetEntityPosition();
            Vector3 vecSelf = GetEntityPosition();
            Vector2 dir = new Vector2(targetMonster.x - vecSelf.x, targetMonster.z - vecSelf.z);
            return dir.normalized;
        }
        return Vector2.zero;
    }

    private EntityMonster FindNearestTarget()
    {
        List<EntityMonster> listMonster = _battleMgr.GetAllEntityMonsters();
        if (listMonster == null || listMonster.Count == 0)
        {
            return null;
        }

        Vector3 vecSelf = GetEntityPosition();
        EntityMonster targetMonster = null;
        float distance = 0.0f;

        for(int idx = 0; idx < listMonster.Count; idx++)
        {
            Vector3 vecMonster = listMonster[idx].GetEntityPosition();
            if (0 == idx)
            {
                distance = Vector3.Distance(vecSelf, vecMonster);
                targetMonster = listMonster[0];
            }
            else
            {
                float val = Vector3.Distance(vecSelf, vecMonster);
                if (distance > val)
                {
                    
                    distance = val;
                    targetMonster = listMonster[idx];
                }
            }
        }

        return targetMonster;
    }

    public override void SetHpVal( int oldVal, int newVal)
    {
        BattleSys.Instance._playerControlWnd.SetHpBarVal(newVal);
    }

    public override void SetDodge()
    {
        GameRoot.Instance._dynamicWnd.SetPlayerDodge();
    }
}