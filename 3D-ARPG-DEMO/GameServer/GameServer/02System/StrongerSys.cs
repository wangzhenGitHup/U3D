/*强化升级系统*/

using System;
using System.Collections.Generic;
using WZNet;
using Protocols;

public class StrongerSys
{
    private static StrongerSys s_instance = null;

    public static StrongerSys Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new StrongerSys();
            }

            return s_instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        CommonTools.Log("Stronger Init Done....");
        cacheSvc = CacheSvc.Instance;
    }

    public void RequestStronger(MsgPack packMsg)
    {
        RequestStronger data = packMsg._msg.reqStronger;
        NetMsg netMsg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_RspStronger,
        };

        PlayerData playerData = cacheSvc.GetPlayerDataBySession(packMsg._session);
        int curStarLv = playerData.strongerArray[data.stongerType];
        StrongerCfd nextCfg = ConfigSvc.Instance.GetStrongerData(data.stongerType, curStarLv + 1);
        //相关条件判断
        if (playerData.lv < nextCfg.minLv)
        {
            netMsg.err = (int)ErroCode.Error_LevelLack;
        }
        else if (playerData.coin < nextCfg.coin)
        {
            netMsg.err = (int)ErroCode.Error_CoinLack;
        }
        else if (playerData.crystal < nextCfg.crystal)
        {
            netMsg.err = (int)ErroCode.Error_CrystalLack;
        }
        else
        {
            //强化任务
            TaskRewardSys.Instance.CalcTaskProgress(playerData, 3);
            //资源扣除
            playerData.coin -= nextCfg.coin;
            playerData.crystal -= nextCfg.crystal;
            playerData.strongerArray[data.stongerType] += 1;
            //增加属性值
            playerData.hp += nextCfg.addHp;
            playerData.ad += nextCfg.addHurt;
            playerData.ap += nextCfg.addHurt;
            playerData.addef += nextCfg.addDef;
            playerData.apdef += nextCfg.addDef;
        }

        //更新数据库
        if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
        {
            netMsg.err = (int)ErroCode.Error_UpdateDB;
        }
        else
        {
            //发到客户端
            netMsg.rspStronger = new ResponseStronger
            {
                coin = playerData.coin,
                crystal = playerData.crystal,
                hp = playerData.hp,
                ad = playerData.ad,
                ap = playerData.ap,
                addDef = playerData.addef,
                apDef = playerData.apdef,
                strongerArr = playerData.strongerArray
            };

            packMsg._session.SendMsg(netMsg);
        }
        
    }
}