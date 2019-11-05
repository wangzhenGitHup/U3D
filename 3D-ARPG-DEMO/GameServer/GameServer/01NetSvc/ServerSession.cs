/**/

using WZNet;
using Protocols;

public class ServerSession :WZSession<NetMsg>
{
    public int sessionID = 0;

    protected override void OnConnected()
    {
        sessionID = ServerRoot.Instance.GetSessionID();
        CommonTools.Log("SessionID: " + sessionID + " Client Connect");
    }

    protected override void OnDisConnected()
    {
        CommonTools.Log("SessionID: " + sessionID +  " Client OnDisConnected");
        LoginSys.Instance.ClearOfflineData(this);
    }

    protected override void OnReciveMsg(NetMsg msg)
    {
        CommonTools.Log("SessionID: " + sessionID +  " Client OnReciveMsg: " + ((MsgCommand)msg.cmd).ToString());
        NetSvc.Instance.AddMsgQueue(this, msg);
    }
}
