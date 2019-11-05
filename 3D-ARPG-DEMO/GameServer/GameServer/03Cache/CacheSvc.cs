/*
    缓存层
 */
using System.Collections.Generic;
using Protocols;


public class CacheSvc
{
    private static CacheSvc s_instance = null;

    public static CacheSvc Instance
    {
        get 
        {
            if (s_instance == null)
            {
                s_instance = new CacheSvc();
            }

            return s_instance;
        }
    }

    private Dictionary<string, ServerSession> dictOnlineAccout = new Dictionary<string, ServerSession>(); /*账号缓存*/
    private Dictionary<ServerSession, PlayerData> dictOnlineSession = new Dictionary<ServerSession, PlayerData>(); /*玩家数据缓存*/
    private DBManager dbManager; /*数据库*/

    public void Init()
    {
        dbManager = DBManager.Instance;
        CommonTools.Log("CacheSvc Init  Done...");
    }

    public bool IsAccountOnLine(string accountKey)
    {
        return dictOnlineAccout.ContainsKey(accountKey);
    }

    public PlayerData GetPlayerData(string account, string pwd)
    {
        //从数据库查找账号密码
        return dbManager.QueryPlayerData(account, pwd);
    }

    /// <summary>
    /// 保存玩家数据账号到缓存
    /// </summary>
    /// <param name="accountKey"></param>
    /// <param name="session"></param>
    /// <param name="playerData"></param>
    public void SaveAccount(string accountKey, ServerSession session, PlayerData playerData)
    {
        dictOnlineAccout.Add(accountKey, session);
        dictOnlineSession.Add(session, playerData);
    }

    /// <summary>
    /// 检查名字是否已经存在在数据库中
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsNameExist(string name)
    {
        return dbManager.QueryNameData(name);
    }

    /// <summary>
    /// 通过serversession获取玩家的名字 从缓冲中
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public PlayerData GetPlayerDataBySession(ServerSession session)
    {
        PlayerData playerData;
        if(dictOnlineSession.TryGetValue(session, out playerData))
        {
            return playerData;
        }
        return null;
    }

    public bool UpdatePlayerData(int id, PlayerData data)
    {
        return dbManager.UpdatePlayerData(id, data);
    }

    /// <summary>
    /// 清除下线账号缓存数据
    /// </summary>
    /// <param name="svcSession"></param>
    public void ClearOfflineData(ServerSession svcSession)
    {
        foreach (var item in dictOnlineAccout)
        {
            if (item.Value == svcSession)
            {
                dictOnlineAccout.Remove(item.Key);
                break;
            }
        }

        bool bSucc = dictOnlineSession.Remove(svcSession);
        CommonTools.Log("Offline Result: SessionID: " + svcSession.sessionID + "  " + bSucc);
    }

    public List<ServerSession> GetOnlineServerSession()
    {
        List<ServerSession> listSrvSession = new List<ServerSession>();
        foreach (var session in dictOnlineSession)
        {
            listSrvSession.Add(session.Key);
        }

        return listSrvSession;
    }

    public Dictionary<ServerSession, PlayerData> GetOnlineCache()
    {
        return dictOnlineSession;
    }

    /// <summary>
    /// 通过玩家id获取session
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ServerSession GetSessionByPlayerID(int id)
    {
        ServerSession session = null;
        foreach (var item in dictOnlineSession)
        {
            if (item.Value.id == id)
            {
                session = item.Key;
                break;
            }
        }

        return session;
    }
}