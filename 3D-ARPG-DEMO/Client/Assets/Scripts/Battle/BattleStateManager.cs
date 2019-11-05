/****************************************************
    文件：BattleStateManager.cs
	作者：WangZhen
    日期：2019/6/22 23:33:28
	功能：战斗状态管理
*****************************************************/

using UnityEngine;
using System.Collections.Generic;

public class BattleStateManager : MonoBehaviour 
{
    private Dictionary<PlayerAniState, IState> dictFSM = new Dictionary<PlayerAniState, IState>();
   
    public void Init()
    {
        CommonTools.Log("BattleStateManager Init.....");
        dictFSM.Add(PlayerAniState.Idle, new StateIdle());
        dictFSM.Add(PlayerAniState.Move, new StateMove());
        dictFSM.Add(PlayerAniState.Attack, new StateAttack());
        dictFSM.Add(PlayerAniState.Born, new StateBorn());
        dictFSM.Add(PlayerAniState.Die, new StateDie());
        dictFSM.Add(PlayerAniState.Hit, new StateHit());
    }

    public void SwitchState(EntityBase entity, PlayerAniState targetState, params object[] args)
    {
        if (entity._currentPlayerAniState == targetState)
        {
            return;
        }

        if (dictFSM.ContainsKey(targetState))
        {
            if (entity._currentPlayerAniState != PlayerAniState.None)
            {
                dictFSM[entity._currentPlayerAniState].ExitState(entity, args);
            }
            dictFSM[targetState].EnterState(entity, args);
            dictFSM[targetState].DoState(entity, args);

        }
    }
}