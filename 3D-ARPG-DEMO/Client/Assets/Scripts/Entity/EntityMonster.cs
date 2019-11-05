/****************************************************
    文件：EntityMonster.cs
	作者：WangZhen
    日期：2019/6/25 22:50:3
	功能：怪物逻辑实体
*****************************************************/

using UnityEngine;

public class EntityMonster : EntityBase 
{
    public MonsterData monsterData;

    float fTickTime = 2.0f; /*tick间隔时间*/
    float fTickCount = 0.0f; /*tick计数*/
    bool bRunAI = true;
    float fAtkInterval = 0.0f; /*攻击间隔计数*/
    float fAtkTime = 2.0f; /*攻击间隔*/

    public EntityMonster()
    {
        _entityType = Constants.EntityType.Monster;
    }

    public override void SetBattleProps(BattleProps val)
    {
        int lv = monsterData.lv;
        BattleProps prop = new BattleProps
        {
            hp = val.hp * lv,
            ad = val.ad * lv,
            ap = val.ap * lv,
            addef = val.addef * lv,
            apdef = val.apdef * lv,
            dodge = val.dodge * lv,
            pierce = val.pierce * lv,
            critical = val.critical * lv
        };

        BattleProps = prop;
        HP = prop.hp;
    }

    public override Vector2 CalcTargetDirection()
    {
        EntityPlayer player = _battleMgr.GetEntityPlayer;
        if (player == null || player._currentPlayerAniState == PlayerAniState.Die)
        {
            bRunAI = false;
            return Vector2.zero;
        }

        Vector3 targetDir = player.GetEntityPosition();
        Vector3 vecSelf = GetEntityPosition();

        return  new Vector2(targetDir.x - vecSelf.x, targetDir.z - vecSelf.z).normalized;
    }

    public override void  UpdateAILogic()
    {
        if (!bRunAI)
        {
            return;
        }

        if (_currentPlayerAniState == PlayerAniState.Idle || 
            _currentPlayerAniState == PlayerAniState.Move)
        {
            if (_battleMgr._bIsPauseGame)
            {
                Idle();
                return;
            }

            float delta = Time.deltaTime;
            fTickCount += delta;
            if (fTickCount < fTickTime)
            {
                return;
            }

            //计算目标方向
            Vector2 targetDir = CalcTargetDirection();
            //是不是在攻击范围
            if (!InAttackRange())
            {
                //朝玩家移动
                SetDirection(targetDir);
                Move();
                return;
            }

            //停止移动
            SetDirection(Vector2.zero);
            fAtkInterval += fTickCount;
            if (fAtkInterval > fAtkTime)
            {
                SetAttackDirection(targetDir);
                Attack(monsterData.cfg.skillID);
                fAtkInterval = 0;
            }
            else
            {
                Idle();
            }

            fTickCount = 0;
            fTickTime = CommonTools.RandomInt(1, 5) * 1.0f / 10;
        }        
    }

    private bool InAttackRange()
    {
        EntityPlayer player = _battleMgr.GetEntityPlayer;
        if (player == null || player._currentPlayerAniState == PlayerAniState.Die)
        {
            bRunAI = false;
            return false;
        }

        Vector3 targetDir = player.GetEntityPosition();
        Vector3 vecSelf = GetEntityPosition();
        //排除y方向
        targetDir.y = 0;
        vecSelf.y = 0;

        float distance = Vector3.Distance(targetDir, vecSelf);
      
        return (distance <= monsterData.cfg.atkDistance);
    }

    public override bool CanBeBreak()
    {
        if (monsterData.cfg.bStop)
        {
            if (_skillCfg != null)
            {
                return _skillCfg.bBreak;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public override void SetHpVal(int oldVal, int newVal)
    {
        if (monsterData.cfg.monsterType == Constants.MonsterType.Boss)
        {
            BattleSys.Instance._playerControlWnd.SetBossHpVal(oldVal, newVal,  BattleProps.hp);
        }
        if (_controller != null)
        {
            base.SetHpVal(oldVal, newVal);
        }
    }
}