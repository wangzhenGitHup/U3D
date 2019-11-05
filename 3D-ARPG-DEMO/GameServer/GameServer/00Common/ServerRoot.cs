/*
    服务器初始化
 */

using System;
using System.Collections.Generic;


public class ServerRoot
{
    private static ServerRoot s_instance = null;

    public static ServerRoot Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new ServerRoot();
            }

            return s_instance;
        }
    }

    private int sessionID = 0;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        //服务层
        NetSvc.Instance.Init();
        CacheSvc.Instance.Init();
        ConfigSvc.Instance.Init();
        TimerSvc.Instance.Init();

        //数据层
        DBManager.Instance.Init();

        //业务系统
        LoginSys.Instance.Init();
        GuideTaskSys.Instance.Init();
        StrongerSys.Instance.Init();
        ChatInfoSys.Instance.Init();
        BuySys.Instance.Init();
        PowerSys.Instance.Init();
        TaskRewardSys.Instance.Init();
        CopyerFightSys.Instance.Init();
    }

    public void Update()
    {
        NetSvc.Instance.Update();
        TimerSvc.Instance.Update();
    }

    /// <summary>
    /// 生成唯一的sessionID
    /// </summary>
    /// <returns></returns>
    public int GetSessionID()
    {
        if (sessionID == int.MaxValue)
        {
            sessionID = 0;
        }
        return sessionID += 1;
    }
}
