/*购买系统*/

using WZNet;
using Protocols;
using System.Collections.Generic;

public class BuySys
{
    private static BuySys s_instance = null;

    public static BuySys Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new BuySys();
            }

            return s_instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        CommonTools.Log("BuySys Init Done.....");
        cacheSvc = CacheSvc.Instance;
    }

    public void RequestBuyInfo(MsgPack packMsg)
    {
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(packMsg._session);
        RequestBuyInfo data = packMsg._msg.reqBuyInfo;
        NetMsg netMsg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_RspBuyInfo,

        };

        if (playerData.diamond < data.costDiamond)
        {
            netMsg.err = (int)ErroCode.Error_DiamondLack;
            return;
        }

        playerData.diamond = data.costDiamond;
        if (data.shopType == (int)Constants.ShopType.ShopType_BuyPower)
        {
            playerData.power += 100;
            TaskRewardSys.Instance.CalcTaskProgress(playerData, Constants.cTask_BuyPower);
        }
        else if (data.shopType == (int)Constants.ShopType.ShopType_MakeCoin)
        {
            playerData.coin += 1000;
            TaskRewardSys.Instance.CalcTaskProgress(playerData, Constants.cTask_MakeCoin);
        }

        if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
        {
            netMsg.err = (int)ErroCode.Error_UpdateDB;
            return;
        }

        netMsg.rspBuyInfo = new ResponseBuyInfo
        {
            shopType = data.shopType,
            leftDiamond = playerData.diamond,
            leftCoin = playerData.coin,
            leftPower = playerData.power
        };

        packMsg._session.SendMsg(netMsg);
    }

}