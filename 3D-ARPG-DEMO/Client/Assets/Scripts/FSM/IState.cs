/****************************************************
    文件：IState.cs
	作者：WangZhen
    日期：2019/6/23 22:36:20
	功能：状态机接口
*****************************************************/

using UnityEngine;

public interface IState 
{
    void EnterState(EntityBase entity, params object[] args);
    void DoState(EntityBase entity, params object[] args);
    void ExitState(EntityBase entity, params object[] args);
}


public enum PlayerAniState
{
    None,
    Idle,
    Move,
    Attack,
    Born,
    Die,
    Hit
}