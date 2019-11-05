/****************************************************
    文件：ShopWnd.cs
	作者：WangZhen
    日期：2019/6/20 17:47:52
	功能：商店交易系统
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using Protocols;

public class ShopWnd : WindowRoot 
{
    public Text txtInfo;
    public Button btnBuy;

    private Constants.ShopType curShopType;

    protected override void InitWnd()
    {
        base.InitWnd();
        btnBuy.interactable = true;
        RefreshUI();
    }

    public void SetShopType(Constants.ShopType shopType)
    {
        curShopType = shopType;
    }

    private void RefreshUI()
    {
        if (curShopType == Constants.ShopType.ShopType_BuyPower)
        {
            txtInfo.text = "是否花费" + Constants.Color("10钻石", Constants.TextColor.Red) + "购买" + Constants.Color("100体力", Constants.TextColor.Green) +  "?";
        }
        else if (curShopType == Constants.ShopType.ShopType_MakeCoin)
        {
            txtInfo.text = "是否花费" + Constants.Color("10钻石", Constants.TextColor.Red) + "铸造" + Constants.Color("1000金币", Constants.TextColor.Green) + "?";
        }
    }
    
    public void OnClickBuy()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);

        NetMsg netMsg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_ReqBuyInfo,
            reqBuyInfo = new RequestBuyInfo
            {
                shopType = (int)curShopType,
                costDiamond = 10
            }
        };
        _netSvc.SendMsg(netMsg);
        btnBuy.interactable = false;
    }

    public void OnClickClose()
    {
        _audioSvc.PlayUIAudio(Constants.cUIClosedBtnSound);
        SetWndState(false);
    }
}