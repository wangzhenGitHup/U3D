/****************************************************
    文件：NetSvc.cs
	作者：WangZhen
    日期：2019/6/11 22:47:47
	功能：网络服务
*****************************************************/

using UnityEngine;
using WZNet;
using Protocols;
using System.Collections.Generic;

public class NetSvc : MonoBehaviour 
{
    public static NetSvc Instance = null;


    private WZNet.WZSocket<ClientSession, NetMsg> clientSock = null; /*客户端网络sock*/
    private Queue<NetMsg> msgQueue = new Queue<NetMsg>(); /*消息队列*/
    private static readonly string lockKey = "lockKey";

    /// <summary>
    /// 初始化
    /// </summary>
    public void InitSvc()
    {
        Instance = this;
        CommonTools.Log("Init NetSvc...");

        clientSock = new WZNet.WZSocket<ClientSession, NetMsg>();
        //设置打印日志接口
        clientSock.SetLog(true, (string msg, int lv) =>
        {
            switch (lv)
            {
                case 0:
                    msg = "Log: " + msg;
                    break;

                case 1:
                    msg = "Warnning: " + msg;
                    break;

                case 2:
                    msg = "Error: " + msg;
                    break;

                case 3:
                    msg = "Info: " + msg;
                    break;
            }
            Debug.Log(msg);
        });

        //启动客户端网络
        clientSock.StartAsClient(ServerCfg.srvIP, ServerCfg.srvPort);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msg"></param>
    public void SendMsg(NetMsg msg)
    {
        if (clientSock.session != null)
        {
            clientSock.session.SendMsg(msg);

        }
        else
        {
            GameRoot.AddTips("服务器未连接");
            InitSvc();
        }
    }

    /// <summary>
    /// 添加消息到消息队列
    /// </summary>
    public void AddNetMsg(NetMsg msg)
    {
        lock (lockKey)
        {
            msgQueue.Enqueue(msg);
        }
    }

    /// <summary>
    /// 更新取消息
    /// </summary>
    private void Update()
    {
        if (msgQueue.Count > 0)
        {
            lock (lockKey)
            {
                NetMsg tmpMsg = msgQueue.Dequeue();
                DispatchMsg(tmpMsg);
            }
        }
    }

    /// <summary>
    /// 分发消息
    /// </summary>
    /// <param name="msg"></param>
    private void DispatchMsg(NetMsg msg)
    {
        //违法的消息
        if (msg.err != (int)ErroCode.Error_None)
        {
            switch ((ErroCode)msg.err)
            {
                case ErroCode.Error_AccountIsOnline:
                    GameRoot.AddTips("当前的账号已经上线了！");
                    return;

                case ErroCode.Error_WrongPwd:
                  GameRoot.AddTips("密码错误！");
                   return;

                case ErroCode.Error_UpdateDB:
                   GameRoot.AddTips("网络不稳定！");//数据库更新异常
                   CommonTools.Log("数据库更新异常", LogType.LogType_Error);
                   return;

                case ErroCode.Error_ServerData:
                    GameRoot.AddTips("客户端数据异常！");
                    CommonTools.Log("数据库更新异常", LogType.LogType_Error);
                   break;

                case ErroCode.Error_LevelLack:
                   GameRoot.AddTips("等级不足！");
                   break;

                case ErroCode.Error_CoinLack:
                   GameRoot.AddTips("金币不足！");
                   break;

                case ErroCode.Error_CrystalLack:
                   GameRoot.AddTips("水晶不足！");
                   break;

                case ErroCode.Error_DiamondLack:
                   GameRoot.AddTips("钻石不足！");
                   break;

                case ErroCode.Error_ClientData:
                   GameRoot.AddTips("任务数据异常！");
                   break;

                case ErroCode.Error_PowerLack:
                   GameRoot.AddTips("体力不足！");
                   break;
                default: return;
            }
        }

        //合法的消息处理
        switch ((MsgCommand)msg.cmd)
        {
            case MsgCommand.Cmd_RspLogin: /*登录响应*/
                LoginSys.Instance.RspLogin(msg);
                break;

            case MsgCommand.Cmd_RspRename: /*重命名响应*/
                LoginSys.Instance.RspRename(msg);
                break;

            case MsgCommand.Cmd_RspGuideTask: /*自动引导任务响应*/
                MainCitySys.Instance.RspGuideTask(msg);
                break;

            case MsgCommand.Cmd_RspStronger: /*强化响应*/
                MainCitySys.Instance.RspStrongerData(msg);
                break;

            case MsgCommand.Cmd_PshChat: /*聊天推送*/
                MainCitySys.Instance.BroadcastChatInfo(msg);
                break;

            case MsgCommand.Cmd_RspBuyInfo: /*购买响应*/
                MainCitySys.Instance.RspBuyInfo(msg);
                break;

            case MsgCommand.Cmd_PshPower:/*体力推送*/
                MainCitySys.Instance.PushPower(msg);
                break;

            case MsgCommand.Cmd_RspTaskReward: /*任务奖励*/
                MainCitySys.Instance.RspTakeTaskReward(msg);
                break;

            case MsgCommand.Cmd_RspCopyerFight:  /*副本战斗*/
                CopyerSys.Instance.RspCopyerFight(msg);
                break;

            case MsgCommand.Cmd_RspEndBattle: /*战斗结算响应*/
                BattleSys.Instance.RspEndBattleResult(msg);
                break;
        }
    }
}