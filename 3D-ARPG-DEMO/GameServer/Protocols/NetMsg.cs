/*
    网络通信协议
 */

using WZNet;
using System;

namespace Protocols
{
    /// <summary>
    /// 服务器配置
    /// </summary>
    public class ServerCfg
    {
        public const string srvIP = "154.221.20.68";
        public const int srvPort = 9999;
    }

    /// <summary>
    /// 协议
    /// </summary>
    [Serializable]
    public class NetMsg : WZMsg
    {
       // public string text;
        public RequestLoginMsg reqLoginMsg;              /*登录请求消息*/
        public ResponseLoginMsg rspLoginMsg;           /*登录响应消息*/

        public RequestRename reqRename;                /*重命名请求消息*/
        public ResponseRename rspRename;             /*重命名响应消息*/

        public RequestGuideTask reqGuideTask;        /*自动任务引导请求消息*/
        public ResponseGuideTask rspGuideTask;      /*自动任务引导响应消息*/

        public RequestStronger reqStronger;             /*强化请求消息*/
        public ResponseStronger rspStronger;          /*强化响应消息*/

        public SendChatInfo sndChat;                     /*聊天发送消息*/
        public PushChatInfo pshChat;                     /*广播聊天消息*/

        public RequestBuyInfo reqBuyInfo;             /*购买请求消息*/
        public ResponseBuyInfo rspBuyInfo;          /*购买请求消息*/

        public PushPower pshPower;                   /*体力恢复*/

        public RequestTaskReward reqTaskReward; /*任务奖励l领取*/
        public ResponseTaskReward rspTaskReward; /*任务奖励领取响应*/
        public PushTaskProgress pshTaskProgress;   /*推送任务进度*/

        public RequestCopyerFight reqCopyerFight;   /*副本战斗*/
        public ResponseCopyerFight rspCopyerFight;

        public RequestEndBattle reqEndBattle;  /*副本结算*/
        public ResponseEndBattle rspEndBattle;

    }

    #region 登录相关
    /// <summary>
    /// 登录请求
    /// </summary>
    [Serializable]
    public class RequestLoginMsg
    {
        public string _account;
        public string _pwd;
    }

    /// <summary>
    /// 登录响应
    /// </summary>
    [Serializable]
    public class ResponseLoginMsg
    {
        public PlayerData _playerData; //玩家数据
    }

    //玩家数据
    [Serializable]
    public class PlayerData
    {
        public int id;
        public string name;     //名字
        public int lv;              //等级
        public int exp;          //经验
        public int power;      //体力
        public int coin;         //金币
        public int diamond;  //钻石
        public int crystal;    //水晶
        public int hp;          //血量
        public int ad;         //物攻
        public int ap;        //法攻
        public int addef;   //物防
        public int apdef;  //法防
        public int dodge;//闪避概率
        public int pierce;//穿透比率
        public int critical;//暴击概率
        public int guideid; //任务引导id
        public int[] strongerArray; //强化数据
        public long time; //登录时间
        public string[] taskReward; //任务数据
        public int curChapter; //当前副本关卡
    }

    /// <summary>
    /// 重命名
    /// </summary>
    [Serializable]
    public class RequestRename
    {
        public string name;
    }

    [Serializable]
    public class ResponseRename
    {
        public string name;
    }
    #endregion

    #region 自动引导任务相关
    [Serializable]
    public class RequestGuideTask
    {
        public int taskId;

    }

    [Serializable]
    public class ResponseGuideTask
    {
        public int taskId;
        public int gainCoin;
        public int roleLv;
        public int gainExp;
    }

    #endregion

    #region 强化协议
    [Serializable]
    public class RequestStronger
    {
        public int stongerType; /*强化类型*/
    }

    [Serializable]
    public class ResponseStronger
    {
        public int coin;
        public int crystal;
        public int hp;
        public int ad;
        public int ap;
        public int addDef;
        public int apDef;
        public int[] strongerArr;
    }

    #endregion

    #region 聊天部分

    [Serializable]
    public class SendChatInfo
    {
        public string chatInfo;
    }

    [Serializable]
    public class PushChatInfo
    {
        public string roleName;
        public string chatInfo;
    }

    #endregion

    #region 购买部分

    [Serializable]
    public class RequestBuyInfo
    {
        public int shopType;
        public int costDiamond;
    }

    [Serializable]
    public class ResponseBuyInfo
    {
        public int shopType;
        public int leftDiamond;
        public int leftCoin;
        public int leftPower;
    }

    #endregion


    #region 体力部分

    [Serializable]
    public class PushPower
    {
        public int power;
    }
    #endregion

    #region 任务奖励

    [Serializable]
    public class RequestTaskReward
    {
        public int taskId;
    }

    [Serializable]
    public class ResponseTaskReward
    {
        public int coin;
        public int lv;
        public int exp;
        public string[] taskArr;
    }

    [Serializable]
    public class PushTaskProgress
    {
        public string[] taskArr;
    }

    #endregion

    #region 副本相关
    [Serializable]
    public class RequestCopyerFight
    {
        public int chapterID;
    }

    [Serializable]
    public class ResponseCopyerFight
    {
        public int chapterID;
        public int power;
    }

    [Serializable]
    public class RequestEndBattle
    {
        public int chapterID;
        public bool bWin;
        public int leftHp;
        public int costTime;
    }

    [Serializable]
    public class ResponseEndBattle
    {
        public int chapterID;
        public bool bWin;
        public int leftHp;
        public int costTime;

        //玩家数据
        public int coin;
        public int lv;
        public int exp;
        public int crystal;
        public int progress;
    }

    #endregion


    #region 枚举相关

    //消息命令
    public enum MsgCommand
    {
        Cmd_None = 0,
        //登录相关的命令号 100
        Cmd_HeartJump = 1,              //心跳包
        Cmd_ReqLogin = 101,            //请求登录
        Cmd_RspLogin = 102,           //响应登录
        Cmd_ReqRename = 103,      //请求重命名
        Cmd_RspRename = 104,      //响应重命名

        Cmd_ReqGuideTask = 200, //任务请求
        Cmd_RspGuideTask = 201, //任务响应

        Cmd_ReqStronger = 300,  //强化请求
        Cmd_RspStronger = 301,  //强化响应

        Cmd_SndChat = 400,       //发送聊天信息
        Cmd_PshChat = 401,       //推送聊天信息

        Cmd_ReqBuyInfo = 500,  //请求购买
        Cmd_RspBuyInfo = 501,  //响应购买

        Cmd_PshPower = 600,   //体力增长

        Cmd_ReqTaskReward = 700, //任务奖励领取
        Cmd_RspTaskReward = 701, //任务奖励领取响应
        Cmd_PshTaskProgress = 702, //推送任务进度

        Cmd_ReqCopyerFight = 800,  //副本战斗请求
        Cmd_RspCopyerFight = 801,  //副本战斗响应
        Cmd_ReqEndBattle = 802,     //战斗结算
        Cmd_RspEndBattle = 803,
    }

    //错误码
    public enum ErroCode
    {
        Error_None = 0,               //没有错误
        Error_AccountIsOnline,    //账号已经上线
        Error_WrongPwd,          //密码错误
        Error_NameIsExist,       //名字已经存在
        Error_UpdateDB,         //更新数据库出错
        Error_ServerData,       //服务器数据异常
        Error_LevelLack,        //等级不够
        Error_CoinLack,        //金币不够
        Error_CrystalLack,    //水晶不够
        Error_DiamondLack, //钻石不够
        Error_ClientData,     //客户端数据异常
        Error_PowerLack,   //体力不足
    }

    #endregion
}
