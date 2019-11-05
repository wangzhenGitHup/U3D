/****************************************************
    文件：StateMove.cs
	作者：WangZhen
    日期：2019/6/23 22:39:57
	功能：移动状态
*****************************************************/

using UnityEngine;

public class StateMove : IState 
{
    public void EnterState(EntityBase entity, params object[] args)
    {
        entity._currentPlayerAniState = PlayerAniState.Move;
    }

    public void DoState(EntityBase entity, params object[] args)
    {
        entity.SetBlend(Constants.cBlendRun);
    }

    public void ExitState(EntityBase entity, params object[] args)
    {

    }
}