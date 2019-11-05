/****************************************************
    文件：StateAttack.cs
	作者：WangZhen
    日期：2019/6/24 21:19:5
	功能：攻击状态
*****************************************************/

using UnityEngine;

public class StateAttack : IState 
{
    public void EnterState(EntityBase entity, params object[] args)
    {
        entity._currentPlayerAniState = PlayerAniState.Attack;
        entity._skillCfg = ResSvc.Instance.GetSkillCfg((int)args[0]);
    }

    public void DoState(EntityBase entity, params object[] args)
    {
        if (entity._entityType == Constants.EntityType.Player)
        {
            entity._bCanReleaseSkill = false;
        }
        entity.AttackEffect((int)args[0]);
        entity.AttackDamage((int)args[0]);
    }

    public void ExitState(EntityBase entity, params object[] args)
    {
        entity.CheckCombo();
    }
}