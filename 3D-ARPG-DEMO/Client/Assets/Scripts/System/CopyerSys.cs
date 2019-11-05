/****************************************************
    文件：CopyerSys.cs
	作者：WangZhen
    日期：2019/6/22 21:10:20
	功能：副本系统
*****************************************************/

using UnityEngine;
using Protocols;


public class CopyerSys : SystemRoot 
{
    public static CopyerSys Instance = null;
    public CopyerWnd _copyerWnd;

    public override void InitSys()
    {
        CommonTools.Log("Init CopyerSys....");
        base.InitSys();
        Instance = this;
    }

    public void OpenCopyerWnd()
    {
        _copyerWnd.SetWndState();
    }

    public void RspCopyerFight(NetMsg msg)
    {
        GameRoot.Instance.SetPlayerCopyerData(msg.rspCopyerFight);
        MainCitySys.Instance._mainCityWnd.SetWndState(false);
        _copyerWnd.SetWndState(false);
        BattleSys.Instance.StartBattle(msg.rspCopyerFight.chapterID);
    }
}