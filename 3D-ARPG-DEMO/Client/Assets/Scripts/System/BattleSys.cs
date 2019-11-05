/****************************************************
    文件：BattleSys.cs
	作者：WangZhen
    日期：2019/6/22 23:23:56
	功能：战斗系统
*****************************************************/

using UnityEngine;
using Protocols;

public class BattleSys : SystemRoot 
{
    public static BattleSys Instance = null;
    public PlayerControlWnd _playerControlWnd;
    public FinishBattleWnd _finishBattleWnd;

    BattleManager battleMgr = null;
    int currentChapterId = -1;
    double battleStartTimes = 0;

    public BattleManager BattleMgr
    {
        get
        {
            return battleMgr;
        }
    }

    public override void InitSys()
    {
        CommonTools.Log("Init BattleSys.....");
        base.InitSys();
        Instance = this;
    }

    public void StartBattle(int chapterId)
    {
        currentChapterId = chapterId;
        GameObject objBattle = new GameObject
        {
            name = "BattleRoot"
        };

        objBattle.transform.SetParent(GameRoot.Instance.transform);
        battleMgr = objBattle.AddComponent<BattleManager>();
        battleMgr.Init(chapterId, () =>
        {
            battleStartTimes = _timeSvc.GetNowTime();
        });
    }

    public void FinishBattle(bool isWin, int remainHp)
    {
        _playerControlWnd.SetWndState(false);
        GameRoot.Instance._dynamicWnd.RemoveAllHpBarItems();
        if (isWin)
        {
            double battleEndTimes = _timeSvc.GetNowTime();
            //发送战斗结束消息
            NetMsg netMsg = new NetMsg
            {
                cmd = (int)MsgCommand.Cmd_ReqEndBattle,
                reqEndBattle = new RequestEndBattle
                {
                    bWin = isWin,
                    chapterID = currentChapterId,
                    leftHp = remainHp,
                    costTime = (int)(battleEndTimes - battleStartTimes)
                }
            };
            _netSvc.SendMsg(netMsg);
        }
        else
        {
            SetFinishBattleWnd(FinishBattleType.Lose);
        }
    }

    public void SetPlayerControlWndState(bool bActive = true)
    {
        _playerControlWnd.SetWndState(bActive);
    }

    public void SetPlayerDirection(Vector2 dirVec)
    {
        battleMgr.SetPlayerDirection(dirVec);
    }

    public void ReleaseSkill(Constants.SkillType skillType)
    {
        battleMgr.ReleaseSkill(skillType);
    }

    public Vector2 GetCurrentDirection()
    {
        return _playerControlWnd.CurrentDirection;
    }

    public void SetFinishBattleWnd(FinishBattleType battleType, bool bActive = true)
    {
        _finishBattleWnd.SetEndBattleType(battleType);
        _finishBattleWnd.SetWndState(bActive);
    }

    public void DestroyBattle()
    {
        SetPlayerControlWndState(false);
        SetFinishBattleWnd(FinishBattleType.None, false);
        GameRoot.Instance._dynamicWnd.RemoveAllHpBarItems();
        Destroy(battleMgr.gameObject);
    }

    public void RspEndBattleResult(NetMsg msg)
    {
        ResponseEndBattle data = msg.rspEndBattle;
        GameRoot.Instance.UpdatePlayerDataOfBattleResult(data);
        _finishBattleWnd.SetBattleResultData(data.chapterID, data.costTime, data.leftHp);
        SetFinishBattleWnd(FinishBattleType.Win);
    }
}