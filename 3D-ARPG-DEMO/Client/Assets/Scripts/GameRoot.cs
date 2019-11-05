/****************************************************
    文件：GameRoot.cs
	作者：WangZhen
    日期：2019/6/9 22:15:29
	功能：游戏启动入口，初始化各个系统 ，保存核心数据
*****************************************************/

using UnityEngine;
using Protocols;

public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance = null;
    public LoadingWnd _loadingWnd; /*加载窗口*/
    public DynamicWnd _dynamicWnd; /*动态更新窗口*/


    private PlayerData playerData = null; /*玩家数据*/


    #region 属性
    
    public LoadingWnd GetLoadingWnd
    {
        get { return _loadingWnd; }
    }

    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
    }
    #endregion

    /// <summary>
    /// 启动游戏
    /// </summary>
    private void Start()
    {
        CommonTools.Log("Game Start.....");
        Instance = this;
        DontDestroyOnLoad(this);
        ClearUIRoot();
        Init();
    }

    /// <summary>
    /// 启动时隐藏不必要的窗口，只保留动态窗口的可见性
    /// </summary>
    private void ClearUIRoot()
    {
        Transform canvas = transform.Find("Canvas");
        for (int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        //服务模块初始化
        NetSvc netSvc = GetComponent<NetSvc>();
        netSvc.InitSvc();

        TimerSvc timerSvc = GetComponent<TimerSvc>();
        timerSvc.InitSvc();

        ResSvc resSvc = GetComponent<ResSvc>();
        resSvc.InitResSvc();

        AudioSvc audioSvc = GetComponent<AudioSvc>();
        audioSvc.InitAudioSvc();

        //业务系统
        LoginSys loginSys = GetComponent<LoginSys>();
        loginSys.InitSys();

        MainCitySys mainCitySys = GetComponent<MainCitySys>();
        mainCitySys.InitSys();

        CopyerSys copyerSys = GetComponent<CopyerSys>();
        copyerSys.InitSys();

        BattleSys battleSys = GetComponent<BattleSys>();
        battleSys.InitSys();

        //只打开dynamicWnd
        _dynamicWnd.SetWndState(true);

        //进入登陆场景并加载相应UI
        loginSys.EnterLogin();
    }

    /// <summary>
    /// 增加提示语句
    /// </summary>
    /// <param name="tipStr"></param>
    public static void AddTips(string tipStr)
    {
        Instance._dynamicWnd.AddTips(tipStr);
    }

    /// <summary>
    /// 设置玩家数据
    /// </summary>
    /// <param name="data"></param>
    public void SetPlayerData(ResponseLoginMsg data)
    {
        playerData = data._playerData;
    }

    /// <summary>
    /// 设置玩家名字
    /// </summary>
    /// <param name="name"></param>
    public void SetPlayerDataName(string name)
    {
        playerData.name = name;
    }

    /// <summary>
    /// 设置玩家引导任务数据
    /// </summary>
    /// <param name="taskData"></param>
    public void SetPlayerDataByGuideTask(ResponseGuideTask taskData)
    {
        PlayerData.coin = taskData.gainCoin;
        PlayerData.lv = taskData.roleLv;
        PlayerData.exp = taskData.gainExp;
        PlayerData.guideid = taskData.taskId;
    }

    public void UpdatePlayerDataByStronger(ResponseStronger data)
    {
        PlayerData.coin = data.coin;
        PlayerData.crystal = data.crystal;
        PlayerData.hp = data.hp;
        PlayerData.ad = data.ad;
        PlayerData.ap = data.ap;
        PlayerData.addef = data.addDef;
        PlayerData.apdef = data.apDef;
        PlayerData.strongerArray = data.strongerArr;
    }

    public void UpdatePlayerDataOfBuy(ResponseBuyInfo data)
    {
        PlayerData.coin = data.leftCoin;
        PlayerData.diamond = data.leftDiamond;
        PlayerData.power = data.leftPower;
    }

    public void SetPlayerPower(int power)
    {
        PlayerData.power = power;
    }

    public void SetPlayerDataByTask(ResponseTaskReward taskData)
    {
        PlayerData.coin = taskData.coin;
        PlayerData.lv = taskData.lv;
        PlayerData.exp = taskData.exp;
        PlayerData.taskReward = taskData.taskArr;
    }

    public void SetPlayerDataByTask(PushTaskProgress data)
    {
        PlayerData.taskReward = data.taskArr;
    }

    public void SetPlayerCopyerData(ResponseCopyerFight data)
    {
        PlayerData.power = data.power;
    }

    public void UpdatePlayerDataOfBattleResult(ResponseEndBattle data)
    {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
        PlayerData.crystal = data.crystal;
        PlayerData.curChapter = data.progress;
    }
}