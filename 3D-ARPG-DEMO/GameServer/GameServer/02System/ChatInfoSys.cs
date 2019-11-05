/*聊天系统*/

using WZNet;
using Protocols;
using System.Collections.Generic;

public class ChatInfoSys
{
    private static ChatInfoSys s_instance = null;

    public static ChatInfoSys Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new ChatInfoSys();
            }

            return s_instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        CommonTools.Log("ChatInfoSys Init Done.....");
        cacheSvc = CacheSvc.Instance;
    }

    public void SendChatInfo(MsgPack packMsg)
    {
        SendChatInfo data = packMsg._msg.sndChat;
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(packMsg._session);

        NetMsg netMsg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_PshChat,
            pshChat = new PushChatInfo
            {
                roleName = playerData.name,
                chatInfo = data.chatInfo
            }
        };

        byte[] msgBytes = WZNet.WZTool.PackNetMsg(netMsg);
        List<ServerSession> onlineSessionList = cacheSvc.GetOnlineServerSession();
        for (int i = 0; i < onlineSessionList.Count; i++)
        {
            onlineSessionList[i].SendMsg(msgBytes);
        }

        TaskRewardSys.Instance.CalcTaskProgress(playerData, Constants.cTask_Chat);
    }
}

