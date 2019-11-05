/*任务引导系统*/

using WZNet;
using Protocols;

public class GuideTaskSys
{
    private static GuideTaskSys s_instance = null;

    public static GuideTaskSys Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new GuideTaskSys();
            }

            return s_instance;
        }
    }

    private ConfigSvc cfgSvc = null;

    private CacheSvc cacheSvc = null;
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        CommonTools.Log("GuideTaskSys Init Done");
        cacheSvc = CacheSvc.Instance;
        cfgSvc = ConfigSvc.Instance;
    }

    /// <summary>
    /// 发送任务引导消息
    /// </summary>
    /// <param name="packMsg"></param>
    public void RequestGuideTaskMessage(MsgPack packMsg)
    {
        RequestGuideTask taskData = packMsg._msg.reqGuideTask;
        NetMsg netMsg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_RspGuideTask,
        };

        PlayerData playerData = cacheSvc.GetPlayerDataBySession(packMsg._session);
        GuideTaskCfg cfgData = cfgSvc.GetGuideTaskData(playerData.guideid);

        //更新任务ID
        if (playerData.guideid == taskData.taskId)
        {
            //智者点拨任务
            if (playerData.guideid == 1001)
            {
                TaskRewardSys.Instance.CalcTaskProgress(playerData, Constants.cTask_WisemanTellme);
            }

            //玩家数据更新
            playerData.guideid += 1;
            playerData.coin += cfgData.gainCoin;
            CommonTools.CalcExp(playerData, cfgData.gainExp);

            //任务奖励更新到数据库
            if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
            {
                netMsg.err = (int)ErroCode.Error_UpdateDB;
            }
            else
            {
                netMsg.rspGuideTask = new ResponseGuideTask
                {
                    taskId = playerData.guideid,
                    gainCoin = playerData.coin,
                    roleLv = playerData.lv,
                    gainExp = playerData.exp
                };
            }
        }
        else
        {
            netMsg.err = (int)ErroCode.Error_ServerData;
        }

       //发给客户端
       packMsg._session.SendMsg(netMsg);
    }

}
