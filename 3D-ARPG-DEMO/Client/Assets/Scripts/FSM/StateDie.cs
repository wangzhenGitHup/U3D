/****************************************************
    文件：StateDie.cs
	作者：WangZhen
    日期：2019/6/27 22:13:7
	功能：死亡状态
*****************************************************/

using UnityEngine;

public class StateDie : IState 
{
    public void EnterState(EntityBase entity, params object[] args)
    {
        entity._currentPlayerAniState = PlayerAniState.Die;
        entity.RemoveSkillCallback();
    }

    public void DoState(EntityBase entity, params object[] args)
    {
        entity.SetAction(Constants.cActionDie);
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            if (entity._entityType == Constants.EntityType.Monster)
            {
                entity.SetActive(false);
                entity.GetCharacterControl().enabled = false;
            }
        }, Constants.cDieAnimationLen);
    }

    public void ExitState(EntityBase entity, params object[] args)
    {

    }
}