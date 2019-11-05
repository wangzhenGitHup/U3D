/*
    登录业务系统
 */
using WZNet;
using Protocols;

public class LoginSys
{
    private static LoginSys s_instance = null;
    
    public static LoginSys Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new LoginSys();
            }

            return s_instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
       CommonTools.Log("LoginSys Init Done");
       cacheSvc = CacheSvc.Instance;
    }

    public void RequsetLogin(MsgPack packMsg)
    {
        RequestLoginMsg data = packMsg._msg.reqLoginMsg;
        //当前账号是否已经上线
        NetMsg svrMsg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_RspLogin,
        };

        //1. 已上线，返回一个信息
        if (cacheSvc.IsAccountOnLine(data._account))
        {
            svrMsg.err = (int)ErroCode.Error_AccountIsOnline;
        }
        else
        {
            //2. 未上线：
            //账号存在：检测密码
            PlayerData playerData = cacheSvc.GetPlayerData(data._account, data._pwd);
            if (playerData == null)
            {
                svrMsg.err = (int)ErroCode.Error_WrongPwd;
            }
            else
            {
                //计算离线体力增值
                int curPower = playerData.power;
                long nowTime = TimerSvc.Instance.GetNowTime();
                long timeInterval = nowTime - playerData.time;
                int increasePowerVal = (int)(timeInterval / (1000 * 60 * Constants.cPowerAddInterval)) * Constants.cPowerAddValPerChange;
                if (increasePowerVal > 0)
                {
                    int maxPowerVal = CommonTools.CalcPowerLimit(playerData.lv);

                    if (playerData.power < maxPowerVal)
                    {
                        playerData.power += increasePowerVal;
                        playerData.power = (playerData.power > maxPowerVal) ? maxPowerVal : playerData.power;
                    }
                }

                if (playerData.power != curPower)
                {
                    cacheSvc.UpdatePlayerData(playerData.id, playerData);
                }

                //不存在：创建默认的账号密码
                svrMsg.rspLoginMsg = new ResponseLoginMsg { _playerData = playerData };
                //存储到缓存中
                cacheSvc.SaveAccount(data._account, packMsg._session, playerData);
            }
        }

        //3.响应客户端的请求
        packMsg._session.SendMsg(svrMsg);
    }

    /// <summary>
    /// 处理重命名
    /// </summary>
    /// <param name="pack"></param>
    public void DoneRename(MsgPack packMsg)
    {
        RequestRename data = packMsg._msg.reqRename;
        NetMsg msg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_RspRename,
        };

        //检查名字是否在数据库中已经存在了
        if (cacheSvc.IsNameExist(data.name))
        {
            //存在：返回错误码
            msg.err = (int)ErroCode.Error_NameIsExist;
        }
        else
        {
            //不存在：更新缓存和数据库，再返回给客户端
            PlayerData playerData = cacheSvc.GetPlayerDataBySession(packMsg._session);
            playerData.name = data.name;

            //更新缓存失败
            if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
            {
                msg.err = (int)ErroCode.Error_UpdateDB;
            }
            else
            {
                //成功就重命名
                msg.rspRename = new ResponseRename{
                    name = data.name,
                };
           }

            //消息发送出去
            packMsg._session.SendMsg(msg);
        }
    }

    /// <summary>
    /// 清除下线账号缓存数据
    /// </summary>
    /// <param name="svcSession"></param>
    public void ClearOfflineData(ServerSession svcSession)
    {
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(svcSession);
        if (playerData != null)
        {
            playerData.time = TimerSvc.Instance.GetNowTime();
            if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
            {
                CommonTools.Log("Update offline time error", LogType.LogType_Error);
            }
        }
        cacheSvc.ClearOfflineData(svcSession);
    }
}