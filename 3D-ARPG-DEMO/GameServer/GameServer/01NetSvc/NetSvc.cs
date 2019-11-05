/*
    网络服务
 */

using WZNet;
using Protocols;
using System.Collections.Generic;

public class NetSvc
{
    private static NetSvc s_instance = null;

    public static NetSvc Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new NetSvc();
            }

            return s_instance;
        }
    }
    private Queue<MsgPack> _netMsgQueue = new Queue<MsgPack>(); /*网络消息队列*/

    public static readonly string lockObj = "lockObj";

    //初始化
    public void Init()
    {
        WZSocket<ServerSession, NetMsg> server = new WZSocket<ServerSession, NetMsg>();
        server.StartAsServer(ServerCfg.srvIP, ServerCfg.srvPort);

        WZTool.LogMsg("NetSvc Init Done.");
    }

    //消息队列
    public void AddMsgQueue(ServerSession svrSession, NetMsg msg)
    {
        //加入队列
        lock (lockObj)
        {
            _netMsgQueue.Enqueue(new MsgPack(svrSession, msg));
        }
    }

    //从消息队列中取网络消息
    public void Update()
    {
        if (_netMsgQueue.Count > 0)
        {
            CommonTools.Log("PackCount: " + _netMsgQueue.Count);

            //检查有没有消息，有就取出并分发
            lock (lockObj)
            {
                MsgPack msg = _netMsgQueue.Dequeue();
                DispatchMsg(msg);
            }
        }
    }

    //分发消息
    private void DispatchMsg(MsgPack packMsg)
    {

        switch ((MsgCommand)packMsg._msg.cmd)
        {
            case MsgCommand.Cmd_ReqLogin: /*登录请求*/
                LoginSys.Instance.RequsetLogin(packMsg);
                break;

            case MsgCommand.Cmd_ReqRename: /*重命名*/
                LoginSys.Instance.DoneRename(packMsg);
                break;

            case MsgCommand.Cmd_ReqGuideTask: /*任务引导*/
                GuideTaskSys.Instance.RequestGuideTaskMessage(packMsg);
                break;

            case MsgCommand.Cmd_ReqStronger: /*强化请求*/
                StrongerSys.Instance.RequestStronger(packMsg);
                break;

            case MsgCommand.Cmd_SndChat: /*发送聊天*/
                ChatInfoSys.Instance.SendChatInfo(packMsg);
                break;

            case MsgCommand.Cmd_ReqBuyInfo: /*购买*/
                BuySys.Instance.RequestBuyInfo(packMsg);
                break;

            case MsgCommand.Cmd_ReqTaskReward: /*任务领取*/
                TaskRewardSys.Instance.ReqTaskDataTake(packMsg);
                break;

            case MsgCommand.Cmd_ReqCopyerFight:  /*副本战斗*/
                CopyerFightSys.Instance.ReqFight(packMsg);
                break;

            case MsgCommand.Cmd_ReqEndBattle: /*战斗结束*/
                CopyerFightSys.Instance.ReqEndBattle(packMsg);
                break;
        }
    }
}

public class MsgPack
{
    public ServerSession _session;
    public NetMsg _msg;
    public MsgPack(ServerSession session, NetMsg msg)
    {
        _session = session;
        _msg = msg;
    }
}