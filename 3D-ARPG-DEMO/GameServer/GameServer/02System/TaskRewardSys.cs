/*任务奖励系统*/

using System;
using System.Collections.Generic;
using Protocols;

public class TaskRewardSys
{
    private static TaskRewardSys s_instance = null;

    public static TaskRewardSys Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new TaskRewardSys();
            }

            return s_instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private ConfigSvc configSvc = null;

    public void Init()
    {
        CommonTools.Log("TaskRewardSys Init Done.....");
        cacheSvc = CacheSvc.Instance;
        configSvc = ConfigSvc.Instance;
    }

    public void ReqTaskDataTake(MsgPack packMsg)
    {
        RequestTaskReward data = packMsg._msg.reqTaskReward;

        NetMsg netMsg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_RspTaskReward,
        };

        PlayerData playerData = cacheSvc.GetPlayerDataBySession(packMsg._session);
        TaskRewardCfg taskDataCfg = configSvc.GetTaskRewardData(data.taskId);
        TaskData taskData = CalcTaskRewardData(playerData, data.taskId);

        if (taskData.progress == taskDataCfg.count && !taskData.bTaked)
        {
            playerData.coin += taskDataCfg.coin;
            CommonTools.CalcExp(playerData, taskDataCfg.exp);
            taskData.bTaked = true;

            //更新任务进度数据
            UpdateTaskProgress(playerData, taskData);
            //更新数据库
            if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
            {
                netMsg.err = (int)ErroCode.Error_UpdateDB;
            }
            else
            {
                ResponseTaskReward rspData = new ResponseTaskReward
                {
                    coin = playerData.coin,
                    lv = playerData.lv,
                    exp = playerData.exp,
                    taskArr = playerData.taskReward
                };
                netMsg.rspTaskReward = rspData;
            }
        }
        else
        {
            netMsg.err = (int)ErroCode.Error_ClientData;
        }

        packMsg._session.SendMsg(netMsg);
    }

    public TaskData CalcTaskRewardData(PlayerData playerData, int taskId)
    {
        TaskData taskData = null;
        for (int i = 0; i < playerData.taskReward.Length; i++)
        {
            string[] taskInfo = playerData.taskReward[i].Split('|');
            if (int.Parse(taskInfo[0]) == taskId)
            {
                taskData = new TaskData
                {
                    ID = taskId,
                    progress = int.Parse(taskInfo[1]),
                    bTaked = taskInfo[2].Equals("1"),
                };
                break;
            }
        }

        return taskData;
    }

    private void UpdateTaskProgress(PlayerData playerData, TaskData taskData)
    {
        string resultStr = taskData.ID + "|" + taskData.progress + "|" + (taskData.bTaked ? 1 : 0);
        int idx = -1;
        for (int i = 0; i < playerData.taskReward.Length; i++)
        {
            string[] taskInfo = playerData.taskReward[i].Split('|');
            if (int.Parse(taskInfo[0]) == taskData.ID)
            {
                idx = i;
                break;
            }
        }

        playerData.taskReward[idx] = resultStr;
    }

    public void CalcTaskProgress(PlayerData playerData, int taskId)
    {
        TaskData taskData = CalcTaskRewardData(playerData, taskId);
        TaskRewardCfg taskDataCfg = configSvc.GetTaskRewardData(taskId);

        if (taskData.progress < taskDataCfg.count)
        {
            taskData.progress += 1;
            UpdateTaskProgress(playerData, taskData);

            //告诉客户端
            ServerSession session = cacheSvc.GetSessionByPlayerID(playerData.id);
            if (session != null)
            {
                session.SendMsg(new NetMsg
                {
                    cmd = (int)MsgCommand.Cmd_PshTaskProgress,
                    pshTaskProgress = new PushTaskProgress
                    {
                        taskArr = playerData.taskReward
                    }
                });
            }
        }
    }
}
