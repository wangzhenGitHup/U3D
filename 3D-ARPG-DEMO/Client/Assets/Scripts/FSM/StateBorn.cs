/****************************************************
    文件：StateBorn.cs
	作者：WangZhen
    日期：2019/6/27 21:58:43
	功能：出生状态
*****************************************************/

using UnityEngine;

public class StateBorn : IState 
{
    public void EnterState(EntityBase entity, params object[] args)
    {
        entity._currentPlayerAniState = PlayerAniState.Born;
    }

    public void DoState(EntityBase entity, params object[] args)
    {
        entity.SetAction(Constants.cActionBorn);
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            entity.SetAction(Constants.cActionDefault);
        }, 500);
    }

    public void ExitState(EntityBase entity, params object[] args)
    {

    }
}