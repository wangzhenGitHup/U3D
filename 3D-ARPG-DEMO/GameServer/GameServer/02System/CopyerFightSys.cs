/*副本战斗系统*/

using System;
using System.Collections.Generic;
using WZNet;
using Protocols;

public class CopyerFightSys
{
    private static CopyerFightSys s_instance = null;
    public static CopyerFightSys Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new CopyerFightSys();
            }

            return s_instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private ConfigSvc cfgSvc = null;

    public void Init()
    {
        CommonTools.Log("CopyerFightSys Init Done");
        cacheSvc = CacheSvc.Instance;
        cfgSvc = ConfigSvc.Instance;
    }

    public void ReqFight(MsgPack packMsg)
    {
        RequestCopyerFight data = packMsg._msg.reqCopyerFight;

        NetMsg netMsg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_RspCopyerFight,
        };

        PlayerData playerData = cacheSvc.GetPlayerDataBySession(packMsg._session);
        int needPower = cfgSvc.GetMapCfgData(data.chapterID).costPower;
        if (playerData.curChapter < data.chapterID)
        {
            netMsg.err = (int)ErroCode.Error_ClientData;
        }
        else
        {
            if (needPower <= playerData.power)
            {
                playerData.power -= needPower;
                if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
                {
                    netMsg.err = (int)ErroCode.Error_UpdateDB;
                }
                else
                {
                    netMsg.rspCopyerFight = new ResponseCopyerFight
                    {
                        chapterID = data.chapterID,
                        power = playerData.power
                    };
                }
            }
            else
            {
                netMsg.err = (int)ErroCode.Error_PowerLack;
            }

            packMsg._session.SendMsg(netMsg);
        }
    }

    public void ReqEndBattle(MsgPack packMsg)
    {
        RequestEndBattle data = packMsg._msg.reqEndBattle;

        NetMsg netMsg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_RspEndBattle,
        };

        //校验是否合法
        if (data.bWin)
        {
            if (data.costTime > 20 && data.leftHp > 0)
            {
                MapCfg mapCfg = cfgSvc.GetMapCfgData(data.chapterID);
                PlayerData playerData = cacheSvc.GetPlayerDataBySession(packMsg._session);

                if (playerData != null && mapCfg != null)
                {
                    TaskRewardSys.Instance.CalcTaskProgress(playerData, Constants.cTask_Copyer);
                    playerData.coin += mapCfg.rewardCoin;
                    playerData.critical += mapCfg.rewardCrystal;
                    CommonTools.CalcExp(playerData, mapCfg.rewardExp);

                    if (playerData.curChapter == data.chapterID)
                    {
                        playerData.curChapter += 1;
                    }

                    if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
                    {
                        netMsg.err = (int)ErroCode.Error_UpdateDB;
                    }
                    else
                    {
                        ResponseEndBattle rspData = new ResponseEndBattle
                        {
                            bWin = data.bWin,
                            chapterID = data.chapterID,
                            costTime = data.costTime,
                            leftHp = data.leftHp,
                            coin = playerData.coin,
                            lv = playerData.lv,
                            exp = playerData.exp,
                            crystal = playerData.crystal,
                            progress = playerData.curChapter
                        };

                        netMsg.rspEndBattle = rspData;
                    }
                }
            }
        }
        else
        {
            netMsg.err = (int)ErroCode.Error_ClientData;
        }

        packMsg._session.SendMsg(netMsg);
    }
}
