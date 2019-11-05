/****************************************************
    文件：StateIdle.cs
	作者：WangZhen
    日期：2019/6/23 22:39:25
	功能：待机状态
*****************************************************/

using UnityEngine;

public class StateIdle : IState 
{
    public void EnterState(EntityBase entity, params object[] args)
    {
        entity._currentPlayerAniState = PlayerAniState.Idle;
        entity.SetDirection(Vector2.zero);
        entity._skillEndCallbackID = -1;
    }

    public void DoState(EntityBase entity, params object[] args)
    {
        if (entity._nextSkillID != 0)
        {
            entity.Attack(entity._nextSkillID);
        }
        else
        {
            if (entity._entityType == Constants.EntityType.Player)
            {
                entity._bCanReleaseSkill = true;
            }

            if (entity.GetCurrentDirection() != Vector2.zero)
            {
                entity.Move();
                entity.SetDirection(entity.GetCurrentDirection());
            }
            else
            {
                entity.SetBlend(Constants.cBlendIdle);
            }
        }
    }

    public void ExitState(EntityBase entity, params object[] args)
    {

    }
}