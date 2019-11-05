/****************************************************
    文件：ClientSession.cs
	作者：WangZhen
    日期：2019/5/29 22:52:26
	功能：客户端网络会话
*****************************************************/

using UnityEngine;
using Protocols;

public class ClientSession : WZNet.WZSession<NetMsg> 
{
    protected override void OnConnected()
    {
        CommonTools.Log("Client Connect");
    }

    protected override void OnReciveMsg(NetMsg msg)
    {
        CommonTools.Log("ReciveMsg: " + ((MsgCommand)msg.cmd).ToString());
        //网络消息包加到队列中
        NetSvc.Instance.AddNetMsg(msg);
    }

    protected override void OnDisConnected()
    {
        CommonTools.Log("Server DisConnected");
    }
}