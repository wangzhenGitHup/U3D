/*客户端服务端通用工具*/
using WZNet;
using Protocols;

public enum LogType
{
    LogType_Log = 0,
    LogType_Warn,
    LogType_Error,
    LogType_Info
}

public class CommonTools
{

    public static void Log(string msg = "", LogType logType = LogType.LogType_Log)
    {
        LogLevel logLv = (LogLevel)logType;
        WZTool.LogMsg(msg, logLv);
    }

    public static int CalcFightByPlayerData(PlayerData playerData)
    {
        return playerData.lv * 100 + playerData.ad + playerData.ap + playerData.addef + playerData.apdef;
    }

    public static int CalcPowerLimit(int lv)
    {
        return (lv - 1) / 10 * 150 + 150;
    }

    public static int CalcExpValueByLv(int lv)
    {
        return 100 * lv * lv;
    }

    public static void CalcExp(PlayerData playerData, int addExp)
    {
        int curRoleLv = playerData.lv;
        int curExp = playerData.exp;
        int accumulateExp = addExp;

        while (true)
        {
            int upLvNeedExp = CommonTools.CalcExpValueByLv(curRoleLv) - curExp;
            if (accumulateExp >= upLvNeedExp)
            {
                curRoleLv += 1;
                curExp = 0;
                accumulateExp -= upLvNeedExp;
            }
            else
            {
                playerData.lv = curRoleLv;
                playerData.exp = accumulateExp + curExp;
                break;
            }
        }
    }

    public static int RandomInt(int min, int max, System.Random rd = null)
    {
        if (rd == null)
        {
            rd = new System.Random();
        }

        int val = rd.Next(min, max + 1);
        return val;
    }
}