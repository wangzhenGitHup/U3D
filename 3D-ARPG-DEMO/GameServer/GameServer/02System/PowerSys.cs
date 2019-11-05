/*体力恢复系统*/

using System;
using System.Collections.Generic;
using Protocols;

public class PowerSys
{
    private static PowerSys s_instance = null;

    public static PowerSys Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new PowerSys();
            }

            return s_instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        CommonTools.Log("PowerSys Init Done.....");
        cacheSvc = CacheSvc.Instance;
        TimerSvc.Instance.AddTimeTask(AutoCalcPowerIncrease, 
            Constants.cPowerAddInterval, WZTimeUnit.Second, 0);
    }

    private void AutoCalcPowerIncrease(int tid)
    {
        //在线玩家计算体力
        NetMsg netMsg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_PshPower
        };
        netMsg.pshPower = new PushPower();

        Dictionary<ServerSession, PlayerData> dictOnlinePlayer = cacheSvc.GetOnlineCache();
        foreach(var session in dictOnlinePlayer)
        {
            PlayerData playerData = session.Value;
            ServerSession svcSession = session.Key;

            int maxPowerVal = CommonTools.CalcPowerLimit(playerData.lv);
            if (maxPowerVal <= playerData.power)
            {
                continue;
            }

            playerData.time = TimerSvc.Instance.GetNowTime();
            playerData.power += Constants.cPowerAddValPerChange;
            if (playerData.power >= maxPowerVal)
            {
                playerData.power = maxPowerVal;
            }

            if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
            {
                netMsg.err = (int)ErroCode.Error_UpdateDB;
            }
            else
            {
                netMsg.pshPower.power = playerData.power;
                svcSession.SendMsg(netMsg);
            }
        }
    }
}