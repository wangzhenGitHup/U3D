/****************************************************
    文件：ChatWnd.cs
	作者：WangZhen
    日期：2019/6/19 14:44:40
	功能：聊天界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Protocols;

public class ChatWnd : WindowRoot 
{
    public Image imgWorldBtn;
    public Image imgGuildBtn;
    public Image imgFriendBtn;
    public Text txtChatContext;
    public InputField inputContext;

    private int curChatType;
    private List<string> listChat = new List<string>();
    private bool bCanSend = true;

    protected override void InitWnd()
    {
        base.InitWnd();
        curChatType = (int)Constants.ChatType.ChatType_World;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (curChatType == (int)Constants.ChatType.ChatType_World)
        {
            string chatMsgStr = "";
            for (int i = 0; i < listChat.Count; i++)
            {
                chatMsgStr += listChat[i] + "\n";
            }
            SetText(txtChatContext, chatMsgStr);
            SetChatBtnImage();
        }
        else if (curChatType == (int)Constants.ChatType.ChatType_Guild)
        {
            SetText(txtChatContext, "功能等待开发！");
            SetChatBtnImage();
        }
        else if (curChatType == (int)Constants.ChatType.ChatType_Friend)
        {
            SetText(txtChatContext, "功能等待开发！");
            SetChatBtnImage();
        }
    }

    private void SetChatBtnImage()
    {
        SetSprite(imgWorldBtn, 
            (curChatType == (int)Constants.ChatType.ChatType_World) ? 
            PathDefine.cChatBtnPressed : PathDefine.cChatBtnNormal);

        SetSprite(imgGuildBtn, 
            (curChatType == (int)Constants.ChatType.ChatType_Guild) ? 
            PathDefine.cChatBtnPressed : PathDefine.cChatBtnNormal);

        SetSprite(imgFriendBtn, 
            (curChatType == (int)Constants.ChatType.ChatType_Friend) ? 
            PathDefine.cChatBtnPressed : PathDefine.cChatBtnNormal);
    }

    public void recieveChatMsg(string roleName, string chatContext)
    {
        listChat.Add(Constants.Color(roleName + ":", Constants.TextColor.Blue) + chatContext);
        if (listChat.Count > Constants.cMaxChatInfo)
        {
            listChat.RemoveAt(0);
        }

        if (GetWndState())
        {
            RefreshUI();
        }
    }

    public void OnClickClose()
    {
        curChatType = (int)Constants.ChatType.ChatType_World;
        _audioSvc.PlayUIAudio(Constants.cUIClosedBtnSound);
        SetWndState(false);
    }

    /// <summary>
    /// 发送聊天内容
    /// </summary>
    public void OnClickSendChat()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        if (!bCanSend)
        {
            GameRoot.AddTips("发送频率过快！");
            return;
        }

        if (inputContext.text != null && inputContext.text != "" && inputContext.text != " ")
        {
            if (inputContext.text.Length > 12)
            {
                GameRoot.AddTips("聊天信息不能超过12个字符！");
            }
            else
            {
                NetMsg netMsg = new NetMsg
                {
                    cmd = (int)MsgCommand.Cmd_SndChat,
                    sndChat = new SendChatInfo
                    {
                        chatInfo = inputContext.text
                    }
                };
                inputContext.text = "";
                _netSvc.SendMsg(netMsg);
                bCanSend = false;
                _timerSvc.AddTimeTask((int tid) => { bCanSend = true; }, 5.0f, WZTimeUnit.Second);
            }
        }
        else
        {
            GameRoot.AddTips("尚未输入聊天的信息");
        }
    }

    public void OnClickWorldBtn()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        curChatType = (int)Constants.ChatType.ChatType_World;
        RefreshUI();
    }

    public void OnClickGuildBtn()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        curChatType = (int)Constants.ChatType.ChatType_Guild;
        RefreshUI();
    }

    public void OnClickFriendBtn()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        curChatType = (int)Constants.ChatType.ChatType_Friend;
        RefreshUI();
    }
}