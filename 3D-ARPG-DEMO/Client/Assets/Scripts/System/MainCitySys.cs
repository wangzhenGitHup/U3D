/****************************************************
    文件：MainCitySys.cs
	作者：WangZhen
    日期：2019/6/15 0:5:35
	功能：主城业务系统
*****************************************************/

using UnityEngine;
using UnityEngine.AI;
using Protocols;

public class MainCitySys : SystemRoot 
{
    public static MainCitySys Instance = null;

    public MainCityWnd _mainCityWnd;      /*主城界面*/
    public PlayerInfoWnd _playerInfoWnd; /*角色信息界面*/
    public GuideWnd _guideTaskWnd;      /*自动任务引导界面*/
    public StrongerWnd _strongerWnd;   /*强化窗口*/
    public ChatWnd _chatWnd;              /*聊天窗口*/
    public ShopWnd _shopWnd;            /*商店窗口*/
    public TaskWnd _taskWnd;             /*任务窗口*/
    /******************************************************************************************/
    private PlayerController playerController; /*角色控制器*/
    private Transform playerCamera; /*专门抓拍角色的相机*/
    private float fRecordStartRotate = 0.0f; /*人物开始的角度*/
    private AutoGuideCfg curTaskData;
    private Transform[] npcPositionTrans; /*主城场景中npc位置*/
    private NavMeshAgent navMeshAgent; /*导航*/
    private bool bIsNavMeshAgent = false; /*是否在导航*/
    /**********************************************************************************/

    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        CommonTools.Log("Init MainCitySys...");
    }

    /// <summary>
    /// 打开主城界面
    /// </summary>
    public void OpenMainCity()
    {
        MapCfg mapData = _resSvc.GetMapCfgData(Constants.cMainCityMapID);
        if (mapData == null)
        {
            CommonTools.Log("mapData error", LogType.LogType_Error);
            return;
        }
        _resSvc.AsyncLoadScene(mapData.sceneName, () =>
        {
            CommonTools.Log("Open MainCity...");
            //加载主角
            LoadPlayer(mapData);
            //打开主城场景UI
            _mainCityWnd.SetWndState(true);

            GameRoot.Instance.GetComponent<AudioListener>().enabled = false;


            //播放主城背景音乐
            _audioSvc.PlayBGM(Constants.cBGMMainCity);

            //设置人物展示相机
            if(playerCamera != null)
            {
                playerCamera.gameObject.SetActive(false);
            }

            //获取主城场景中任务相关的几个npc的位置信息
            GameObject objMapRoot = GameObject.FindGameObjectWithTag("MapRoot");
            MapRootInfo mapRoot = objMapRoot.GetComponent<MapRootInfo>();
            npcPositionTrans = mapRoot._npcPositionTrans;
        });
    }

    /// <summary>
    /// 加载主角
    /// </summary>
    /// <param name="mapData"></param>
    private void LoadPlayer(MapCfg mapData)
    {
        GameObject objPlayer = _resSvc.LoadPrefab(PathDefine.cMainCityPlayerPrefab, true);
        objPlayer.transform.position = mapData.playerBornPos;
        objPlayer.transform.localEulerAngles = mapData.playerBornRote;
        objPlayer.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        //相机初始化
        Camera.main.transform.position = mapData.mainCameraPos;
        Camera.main.transform.localEulerAngles = mapData.mainCameraRote;

        playerController = objPlayer.GetComponent<PlayerController>();
        playerController.Init();

        navMeshAgent = playerController.GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// 设置角色移动方向
    /// </summary>
    /// <param name="dirVec"></param>
    public void SetMoveDirection(Vector2 dirVec)
    {
        StopNavigationTask();

        if (dirVec == Vector2.zero)
        {
            playerController.SetBlend(Constants.cBlendIdle);
        }
        else
        {
            playerController.SetBlend(Constants.cBlendRun);
        }

        playerController.CurDirection = dirVec;
    }

    #region 角色信息面板
    /// <summary>
    /// 打开角色信息界面
    /// </summary>
    public void OpenPlayerInfoWnd()
    {
        StopNavigationTask();

        //获取人物展示相机
        if (playerCamera == null)
        {
            playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera").transform;
        }
        //设置人物展示相机相对位置
        playerCamera.localPosition = playerController.transform.position + playerController.transform.forward * 3.8f + new Vector3(0, 1.2f, 0);
       //角度，人物正面加上人物自身的角度
        playerCamera.localEulerAngles = new Vector3(0, 180 + playerController.transform.localEulerAngles.y, 0);
        //缩放
        playerCamera.localScale = Vector3.one;
        //激活
        playerCamera.gameObject.SetActive(true);

        _playerInfoWnd.SetWndState();
    }

    /// <summary>
    /// 关闭角色信息界面处理
    /// </summary>
    public void ClosedPlayerInfoWnd()
    {
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
            _playerInfoWnd.SetWndState(false);
        }
    }

    /// <summary>
    /// 设置人物开始时的旋转角度
    /// </summary>
    public void RecordPlayerStartRotate()
    {
        fRecordStartRotate = playerController.transform.localEulerAngles.y;
    }

    /// <summary>
    /// 设置角色信息角色角度
    /// </summary>
    /// <param name="rotate"></param>
    public void SetPlayerInfoPlayerRotate(float rotate)
    {
        playerController.transform.localEulerAngles = new Vector3(0, fRecordStartRotate + rotate, 0);
    }

    #endregion

    #region 自动引导部分
    /// <summary>
    /// 跑任务重点逻辑处
    /// </summary>
    /// <param name="taskData"></param>
    public void RunTask(AutoGuideCfg taskData)
    {
        if (taskData != null)
        {
            curTaskData = taskData;
        }

        navMeshAgent.enabled = true;

        //解析任务数据
        if (curTaskData.npcID != -1)
        {
            //计算当前角色与任务点npc的距离
            float distance = Vector3.Distance(playerController.transform.position, npcPositionTrans[taskData.npcID].position);
            if (distance < 0.5f)
            {
                ClosedNavigationTask();
            }
            else
            {
                bIsNavMeshAgent = true;
                //距离大于0.5 启动自动导航
                navMeshAgent.enabled = true;
                //设置导航的速度
                navMeshAgent.speed = Constants.cNPCMoveSpeed;
                //导航的目标位置设置为相关npc的位置
                navMeshAgent.SetDestination(npcPositionTrans[taskData.npcID].position);
                playerController.SetBlend(Constants.cBlendRun);
            }
        }
        else
        {
            OpenAutoGuideWnd();
        }
    }

    /// <summary>
    /// 停止导航
    /// </summary>
    private void StopNavigationTask()
    {
        if (bIsNavMeshAgent)
        {
            bIsNavMeshAgent = false;
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
            playerController.SetBlend(Constants.cBlendIdle);
        }
    }

    /// <summary>
    /// 检测有木有到达任务点
    /// </summary>
    private void CheckArriveNavigationTarget()
    {
        float distance = Vector3.Distance(playerController.transform.position, npcPositionTrans[curTaskData.npcID].position);
        if (distance < 1.5f)
        {
            ClosedNavigationTask();
        }
    }

    /// <summary>
    /// 关闭任务导航
    /// </summary>
    private void ClosedNavigationTask()
    {
        bIsNavMeshAgent = false;
        //停止导航
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
        //玩家待机
        playerController.SetBlend(Constants.cBlendIdle);
        //打开任务窗口
        OpenAutoGuideWnd();
    }


    /// <summary>
    /// 打开任务窗口
    /// </summary>
    private void OpenAutoGuideWnd()
    {
        _guideTaskWnd.SetWndState();
    }

    /// <summary>
    /// 取得当前的任务数据
    /// </summary>
    /// <returns></returns>
    public AutoGuideCfg GetCurrentGuideTaskData()
    {
        return curTaskData;
    }

    /// <summary>
    /// 自动引导任务的响应
    /// </summary>
    /// <param name="msg"></param>
    public void RspGuideTask(NetMsg msg)
    {
        ResponseGuideTask taskData = msg.rspGuideTask;
        GameRoot.AddTips("任务奖励 金币 + " + curTaskData.gainCoin + "经验 + " + curTaskData.gainExp);

        switch (curTaskData.targetTaskID)
        {
            case 0: /*与智者对话*/
                break;

            case 1: /*进入副本*/
                OpenCopyerWnd();
                break;

            case 2: /*强化*/
                OpenStrongerWnd();
                break;

            case 3: /*体力购买*/
                OpenShopWnd(Constants.ShopType.ShopType_BuyPower);
                break;

            case 4: /*金币铸造*/
                OpenShopWnd(Constants.ShopType.ShopType_MakeCoin);
                break;

            case 5: /*世界聊天*/
                OpenChatWnd();
                break;
        }

        GameRoot.Instance.SetPlayerDataByGuideTask(taskData);
        _mainCityWnd.RefreshUI();
    }

    #endregion

    #region 强化窗口部分

    public void OpenStrongerWnd()
    {
        StopNavigationTask();
        _strongerWnd.SetWndState();
    }

    public void RspStrongerData(NetMsg msg)
    {
        int preFightVal = CommonTools.CalcFightByPlayerData(GameRoot.Instance.PlayerData);
        GameRoot.Instance.UpdatePlayerDataByStronger(msg.rspStronger);

        int curFightVal = CommonTools.CalcFightByPlayerData(GameRoot.Instance.PlayerData);

        GameRoot.AddTips("战力提升 + "+ (curFightVal - preFightVal));

        _strongerWnd.UpdateInfo();
        _mainCityWnd.RefreshUI();
    }

    #endregion

    #region 聊天部分

    public void OpenChatWnd()
    {
        StopNavigationTask();
        _chatWnd.SetWndState();
    }

    public void BroadcastChatInfo(NetMsg msg)
    {
        _chatWnd.recieveChatMsg(msg.pshChat.roleName, msg.pshChat.chatInfo);
    }

    #endregion

    #region 商店部分
    
    public void OpenShopWnd(Constants.ShopType shopType)
    {
        StopNavigationTask();
        _shopWnd.SetShopType(shopType);
        _shopWnd.SetWndState();
    }

    public void RspBuyInfo(NetMsg msg)
    {
        ResponseBuyInfo data = msg.rspBuyInfo;
        GameRoot.Instance.UpdatePlayerDataOfBuy(data);
        GameRoot.AddTips("购买成功！");

        _mainCityWnd.RefreshUI();
        _shopWnd.SetWndState(false);
    }

    #endregion

    #region 体力增加部分
    public void PushPower(NetMsg msg)
    {
        PushPower data = msg.pshPower;
        GameRoot.Instance.SetPlayerPower(data.power);
        if (_mainCityWnd.gameObject.activeSelf)
        { 
            _mainCityWnd.RefreshUI(); 
        }
    }
    #endregion


    #region 任务奖励部分

    public void OpenTaskWnd()
    {
        StopNavigationTask();
        _taskWnd.SetWndState();
    }

    public void RspTakeTaskReward(NetMsg msg)
    {
        ResponseTaskReward data = msg.rspTaskReward;
        GameRoot.Instance.SetPlayerDataByTask(data);
        UpdateTaskWndUI();
    }

    public void PushTaskProgress(NetMsg msg)
    {
        PushTaskProgress data = msg.pshTaskProgress;
        GameRoot.Instance.SetPlayerDataByTask(data);
        UpdateTaskWndUI();
    }

    private void UpdateTaskWndUI()
    {
        _taskWnd.RefreshUI();
        _mainCityWnd.RefreshUI();
    }
    #endregion

    #region 副本部分

    public void OpenCopyerWnd()
    {
        StopNavigationTask();
        CopyerSys.Instance.OpenCopyerWnd();
    }

    #endregion

    private void Update()
    {
        //正在导航，相机跟随
        if (bIsNavMeshAgent)
        {
            playerController.SetCameraFollow();
            CheckArriveNavigationTarget();
        }
    }



}