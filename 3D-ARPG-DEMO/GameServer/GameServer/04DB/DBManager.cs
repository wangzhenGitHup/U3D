/*
    数据库管理
 */
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MySql.Data;
using Protocols;

public class DBManager
{
    private static DBManager s_instance = null;
    public static DBManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new DBManager();
            }

            return s_instance;
        }
    }

    private MySqlConnection conn;

    public void Init()
    {
        CommonTools.Log("DB Init Done");
        conn = new MySqlConnection("server=localhost;User Id=root;password=123456;Database=darkgod;Charset=utf8");
        conn.Open();
        QueryPlayerData("wz00", "123456");
    }

    #region 玩家数据
    /// <summary>
    /// 查询玩家数据
    /// </summary>
    /// <param name="account">玩家账号</param>
    /// <param name="pwd">玩家密码</param>
    /// <returns></returns>
    public PlayerData QueryPlayerData(string account, string pwd)
    {
        PlayerData playerData = null;

        MySqlDataReader reader = null;

        bool isNewAccount = true;
        try
        {
            MySqlCommand cmd = new MySqlCommand("select* from account where account = @account", conn);
            cmd.Parameters.AddWithValue("account", account);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                isNewAccount = false;
                string pwdStr = reader.GetString("pwd");
                if (pwdStr.Equals(pwd))
                {
                    //密码正确，返回玩家数据
                    playerData = new PlayerData
                    {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name"),
                        lv = reader.GetInt32("level"),
                        exp = reader.GetInt32("exp"),
                        power = reader.GetInt32("power"),
                        coin = reader.GetInt32("coin"),
                        diamond = reader.GetInt32("diamond"),
                        crystal = reader.GetInt32("crystal"),
                        hp = reader.GetInt32("hp"),
                        ad = reader.GetInt32("ad"),
                        ap = reader.GetInt32("ap"),
                        addef = reader.GetInt32("addef"),
                        apdef = reader.GetInt32("apdef"),
                        dodge = reader.GetInt32("dodge"),
                        pierce = reader.GetInt32("pierce"),
                        critical = reader.GetInt32("critical"),
                        guideid = reader.GetInt32("guideid"),
                        time = reader.GetInt64("time"),
                        curChapter = reader.GetInt32("curChapter")
                        //TODO:
                    };

                    #region 强化数据
                    int[] parseStrongerData = new int[6];
                    string[] strongerStr = reader.GetString("stronger").Split('#');
                    for (int idx = 0; idx < strongerStr.Length; idx++)
                    {
                        if (strongerStr[idx] == "")
                        {
                            continue;
                        }

                        int startLv = 0;
                        if (int.TryParse(strongerStr[idx], out startLv))
                        {
                            parseStrongerData[idx] = startLv;
                        }
                         else
                        {
                            CommonTools.Log("Parse Stronger Data Error:", LogType.LogType_Error);
                        }
                    }

                    playerData.strongerArray = parseStrongerData;
                    #endregion

                    #region 任务奖励

                    string[] taskDataAtrrStr = reader.GetString("task").Split('#');
                    playerData.taskReward = new string[6];

                    for (int idx = 0; idx < taskDataAtrrStr.Length; idx++)
                    {
                        if (taskDataAtrrStr[idx] == "")
                        {
                            continue;
                        }
                        else if (taskDataAtrrStr[idx].Length >= 5)
                        {
                            playerData.taskReward[idx] = taskDataAtrrStr[idx];
                        }
                        else
                        {
                            CommonTools.Log("Parse Task Data Error:", LogType.LogType_Error);
                        }
                    }
                    #endregion
                }
            }
        }
        catch (Exception e)
        {
            CommonTools.Log("Query PlayerData by Account&Pwd Error: " + e, LogType.LogType_Error);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }

            if (isNewAccount)
            {
                //不存在账号数据，创建新的默认账号数据，并返回
                playerData = new PlayerData
                {
                    id = -1,
                    name = "",
                    lv = 1,
                    exp = 0,
                    power = 150,
                    coin = 5000,
                    diamond = 500,
                    crystal = 500,
                    hp = 2000,
                    ad = 275,
                    ap = 265,
                    addef = 67,
                    apdef = 43,
                    dodge = 7,
                    pierce = 5,
                    critical = 2,
                    guideid = 1001,
                    strongerArray = new int[6],
                    time = TimerSvc.Instance.GetNowTime(),
                    taskReward = new string[6],
                    curChapter = 10001,
                };
                //初始化任务奖励数据
                for(int i = 0; i < playerData.taskReward.Length; i++)
                {
                    playerData.taskReward[i] = (i +1) + "|0|0";
                }
                playerData.id = InsertNewAccountData(account, pwd, playerData);
            }
        }

        return playerData;
    }

    private int InsertNewAccountData(string account, string pwd, PlayerData insertData)
    {
        int id = -1;
        try
        {
            MySqlCommand cmd = new MySqlCommand(
               "insert into account set account=@account,pwd =@pwd,name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond, crystal=@crystal," +
               "hp = @hp, ad = @ad, ap = @ap, addef = @addef, apdef = @apdef, dodge = @dodge, pierce = @pierce, critical = @critical, guideid = @guideid, stronger=@stronger, time=@time, task = @task, curChapter = @curChapter", conn);

            cmd.Parameters.AddWithValue("account", account);
            cmd.Parameters.AddWithValue("pwd", pwd);
            cmd.Parameters.AddWithValue("name", insertData.name);
            cmd.Parameters.AddWithValue("level", insertData.lv);
            cmd.Parameters.AddWithValue("exp", insertData.exp);
            cmd.Parameters.AddWithValue("power", insertData.power);
            cmd.Parameters.AddWithValue("coin", insertData.coin);
            cmd.Parameters.AddWithValue("diamond", insertData.diamond);
            cmd.Parameters.AddWithValue("crystal", insertData.crystal);

            cmd.Parameters.AddWithValue("hp", insertData.hp);
            cmd.Parameters.AddWithValue("ad", insertData.ad);
            cmd.Parameters.AddWithValue("ap", insertData.ap);
            cmd.Parameters.AddWithValue("addef", insertData.addef);
            cmd.Parameters.AddWithValue("apdef", insertData.apdef);
            cmd.Parameters.AddWithValue("dodge", insertData.dodge);
            cmd.Parameters.AddWithValue("pierce", insertData.pierce);
            cmd.Parameters.AddWithValue("critical", insertData.critical);
            cmd.Parameters.AddWithValue("guideid", insertData.guideid);

            //强化数据数组转成字符串
            string strongerInfo = "";
            for(int idx = 0; idx < insertData.strongerArray.Length; idx++)
            {
                strongerInfo += insertData.strongerArray[idx];
                strongerInfo += '#';
            }
            cmd.Parameters.AddWithValue("stronger", strongerInfo);
            cmd.Parameters.AddWithValue("time", insertData.time);

            string taskInfo = "";
            for (int i = 0; i < insertData.taskReward.Length; i++)
            {
                taskInfo += insertData.taskReward[i];
                taskInfo += "#";
            }
            cmd.Parameters.AddWithValue("task", taskInfo);
            cmd.Parameters.AddWithValue("curChapter", insertData.curChapter);
            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;
        }
        catch (Exception e)
        {
            CommonTools.Log("Insert PlayerData  Error: " + e, LogType.LogType_Error);
        }
        return id;
    }

    public bool QueryNameData(string name)
    {

        bool isExist = false;
        MySqlDataReader reader = null;

        try
        {
            MySqlCommand cmd = new MySqlCommand("select* from account where name=@name", conn);
            cmd.Parameters.AddWithValue("name", name);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isExist = true;
            }
        }
        catch (Exception e)
        {
            CommonTools.Log("Query Name Error: " + e, LogType.LogType_Error);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }
        return isExist;
    }

    public bool UpdatePlayerData(int id, PlayerData playerData)
    {
        try
        {
            MySqlCommand cmd = new MySqlCommand(
           "update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond, crystal=@crystal," +
           "hp = @hp, ad = @ad, ap = @ap, addef = @addef, apdef = @apdef, dodge = @dodge, pierce = @pierce, critical = @critical, guideid = @guideid, stronger = @stronger, time=@time,  task = @task, curChapter = @curChapter where id =@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("level", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("power", playerData.power);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.crystal);
            cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);
            cmd.Parameters.AddWithValue("guideid", playerData.guideid);

            //强化数据数组转成字符串
            string strongerInfo = "";
            for(int idx = 0; idx < playerData.strongerArray.Length; idx++)
            {
                strongerInfo += playerData.strongerArray[idx];
                strongerInfo += '#';
            }
            cmd.Parameters.AddWithValue("stronger", strongerInfo);
            cmd.Parameters.AddWithValue("time", playerData.time);

            string taskInfo = "";
            for (int i = 0; i < playerData.taskReward.Length; i++)
            {
                taskInfo += playerData.taskReward[i];
                taskInfo += "#";
            }
            cmd.Parameters.AddWithValue("task", taskInfo);
            cmd.Parameters.AddWithValue("curChapter", playerData.curChapter);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            CommonTools.Log("Update PlayerData Error: " + e, LogType.LogType_Error);
            return false;
        }

        return true;
    }

    #endregion
}