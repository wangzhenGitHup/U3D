/****************************************************
    文件：MainCityWnd.cs
	作者：WangZhen
    日期：2019/6/15 0:4:15
	功能：主城界面
*****************************************************/

using UnityEngine;
using Protocols;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainCityWnd : WindowRoot
{
    #region UI变量
    
    public Text txtFight;
    public Text txtPower;
    public Text txtLevel;
    public Text txtName;
    public Text txtExpProgress;
    public Image imagePowerProgress;
    public Transform expProgressTrans;
    public Animation animMenu;
    public Animation animBtn;
    public Button btnMenu;
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    public Button btnGuide;

    #endregion

    private GridLayoutGroup gridExpBg;
    private bool bMenuState = true;
    private Vector2 vecStartPos = Vector2.zero;
    private Vector2 vecDefaultPos = Vector2.zero;
    private float fOperatorDistance;
    private AutoGuideCfg curGuideCfgData; /*当前任务数据*/


    protected override void InitWnd()
    {
        base.InitWnd();

        vecDefaultPos = imgDirBg.transform.position;
        //主要为了根据UI适配算出偏移量
        fOperatorDistance = Screen.height * 1.0f / Constants.cScreenStandardHeight * Constants.cOperatorOffset;
        //隐藏操作杆
        SetActive(imgDirPoint, false);
        RefreshUI();
        RegisterTouchEvents();
    }

    public void RefreshUI()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        SetText(txtFight, CommonTools.CalcFightByPlayerData(playerData));
        SetText(txtPower, "体力:" + playerData.power + "/" + CommonTools.CalcPowerLimit(playerData.lv));
        imagePowerProgress.fillAmount = playerData.power * 1.0f / CommonTools.CalcPowerLimit(playerData.lv);

        SetText(txtLevel, playerData.lv);
        SetText(txtName, playerData.name);

        //exp
        int expVal = (int) (playerData.exp / CommonTools.CalcExpValueByLv(playerData.lv) * 100);
        SetText(txtExpProgress, expVal + "%");
        
        //exp自适应处理
        gridExpBg = expProgressTrans.GetComponent<GridLayoutGroup>();

        //缩放比例
        float globalScaleRate = 1.0f * Constants.cScreenStandardHeight / Screen.height;
        float realWidth = Screen.width * globalScaleRate;
        float cellWidth = (realWidth - 180) / 10;
        gridExpBg.cellSize = new Vector2(cellWidth, 7);

        //进度条
        int expIdx = expVal / 10;
        for (int i = 0; i < expProgressTrans.childCount; i++)
        {
            Image img = expProgressTrans.GetChild(i).GetComponent<Image>();
            if (i < expIdx)
            {
                img.fillAmount = 1;
            }
            else if (i == expIdx)
            {
                img.fillAmount = expVal % 10 * 1.0f / 10;
            }
            else
            {
                img.fillAmount = 0;
            }
        }

        SetTaskData();
    }

    /// <summary>
    /// 注册摇杆事件
    /// </summary>
    public void RegisterTouchEvents()
    {
        //按下
        OnClickDown(imgTouch.gameObject, (PointerEventData evt) =>
        {
            imgDirBg.transform.position = evt.position;
            vecStartPos = evt.position;
            SetActive(imgDirPoint);
        });

        //抬起
        OnClickUp(imgTouch.gameObject, (PointerEventData evt) =>
        {
            imgDirBg.transform.position = vecDefaultPos;
            imgDirPoint.transform.localPosition = Vector2.zero;
            SetActive(imgDirPoint, false);

            //方向信息传递出去
            MainCitySys.Instance.SetMoveDirection(Vector2.zero);
        });

        //拖动
        OnDragEvt(imgTouch.gameObject, (PointerEventData evt) =>
        {
            Vector2 curDir = evt.position - vecStartPos;
            float len = curDir.magnitude;

            //有没有超出
            if (len > fOperatorDistance)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(curDir, fOperatorDistance);
                imgDirPoint.transform.position = vecStartPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = evt.position;
            }

            //方向信息传递
            MainCitySys.Instance.SetMoveDirection(curDir.normalized);
        });
    }

    private void SetTaskData()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        curGuideCfgData = _resSvc.GetAutoGuideCfgData(playerData.guideid);
        if (curGuideCfgData != null)
        {
            SetTaskIcon(curGuideCfgData.npcID);
        }
        else
        {
            SetTaskIcon(-1); //默认图标
        }
    }

    private void SetTaskIcon(int npcID)
    {
        string iconPath = "";
        Image imgBtn = btnGuide.GetComponent<Image>();
        switch (npcID)
        {
            case Constants.cNPC_Wiseman:
                iconPath = PathDefine.cTaskWiseManIcon;
                break;

            case Constants.cNPC_General:
                iconPath = PathDefine.cTasGeneralIcon;
                break;

            case Constants.cNPC_Artisan:
                iconPath = PathDefine.cTaskArtisanIcon;
                break;

            case Constants.cNPC_Trader:
                iconPath = PathDefine.cTaskTraderIcon;
                break;

            default:
                iconPath = PathDefine.cTaskDefaultIcon;
                break;
        }
        //设置当前任务按钮的图标
        SetSprite(imgBtn, iconPath);
    }


    #region 事件部分
    public void OnClickMenuBtn()
    {
        _audioSvc.PlayUIAudio(Constants.cUIExternBtnSound);
        bMenuState = !bMenuState;

        AnimationClip menuClip = null;
        //AnimationClip btnClip = null;
        if (bMenuState)
        {
            menuClip = animMenu.GetClip("OpenMenu");
            //btnClip = animBtn.GetClip("btnRotate");
        }
        else
        {
            menuClip = animMenu.GetClip("CloseMenu");
           // btnClip = animBtn.GetClip("btnReset");
        }

        animMenu.Play(menuClip.name);
        //animBtn.Play(btnClip.name);
    }

    public void OnClickHead()
    {
        _audioSvc.PlayUIAudio(Constants.cOpenBigWndSound);
        MainCitySys.Instance.OpenPlayerInfoWnd();
    }

    public void OnClickTask()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        if (curGuideCfgData != null)
        {
            MainCitySys.Instance.RunTask(curGuideCfgData);
        }
        else
        {
            GameRoot.AddTips("更多引导任务，正在开发中....");
        }
    }

    public void OnClickStrongerWnd()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);

        MainCitySys.Instance.OpenStrongerWnd();
    }

    public void OnClickOpenChat()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        MainCitySys.Instance.OpenChatWnd();
    }

    public void OnClickOpenShopWnd()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        MainCitySys.Instance.OpenShopWnd(Constants.ShopType.ShopType_BuyPower);
    }

    public void OnClickMintGoldCoin()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        MainCitySys.Instance.OpenShopWnd(Constants.ShopType.ShopType_MakeCoin);
    }

    public void OnClickOpenTaskWnd()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        MainCitySys.Instance.OpenTaskWnd();
    }

    public void OnClickOpenCopyerWnd()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        MainCitySys.Instance.OpenCopyerWnd();
    }

    #endregion
}