/****************************************************
    文件：StrongerWnd.cs
	作者：WangZhen
    日期：2019/6/19 0:0:41
	功能：强化窗口
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Protocols;

public class StrongerWnd : WindowRoot
{
    #region 控件字段

    public Transform leftBtnTrans; /*左边点击面板*/
    public Image imgCurSelectPos;
    public Text txtStarLv;
    public Transform starTransGroup;
    public Text txtPropHP1;
    public Text txtPropHurt1;
    public Text txtPropDef1;
    public Text txtPropHp2;
    public Text txtPropHurt2;
    public Text txtPropDef2;
    public Image imgPropArrowHp;
    public Image imgPropArrowHurt;
    public Image imgPropArrowDef;
    public Text txtNeedLv;
    public Text txtCostCoin;
    public Text txtCostCrystal;
    public Text txtLeftCoin;
    public Transform costPanel;

    #endregion

    private Image[] imgBgArray = new Image[6];
    private int curSelectIdx; /*当前选择的哪项*/
    private PlayerData playerData; /*玩家数据*/
    private StrongerCfg nextCfg;
    
    /// <summary>
    /// 初始化
    /// </summary>
    protected override void InitWnd()
    {
        base.InitWnd();

        playerData = GameRoot.Instance.PlayerData;
        RegisterClickEvt();
        ClickImage(0);
    }

    /// <summary>
    /// 注册图片点击事件
    /// </summary>
    private void RegisterClickEvt()
    {
        for (int i = 0; i < leftBtnTrans.childCount; i++)
        {
            Image img = leftBtnTrans.GetChild(i).GetComponent<Image>();

            OnClickImageEvt(img.gameObject, (object args) =>
            {
                _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
                ClickImage((int) args);
            }, i);

            imgBgArray[i] = img;
        }
    }

    /// <summary>
    /// 刷新项目数据
    /// </summary>
    private void RefreshItemInfo()
    {
        //剩余金币
        SetText(txtLeftCoin, playerData.coin);

        //星级
        SetText(txtStarLv, playerData.strongerArray[curSelectIdx] + "星级");
        int curStarLv = playerData.strongerArray[curSelectIdx];
        for (int i = 0; i < starTransGroup.childCount; i++)
        {
            Image imgStar = starTransGroup.GetChild(i).GetComponent<Image>();
            if (i < curStarLv)
            {
                SetSprite(imgStar, PathDefine.cItemIconLightingStar);
            }
            else
            {
                SetSprite(imgStar, PathDefine.cItemIconDarkStar);
            }
        }
        
        //升级前属性数据
        int addHp = _resSvc.GetPropsIncreaseValue(curSelectIdx, curStarLv +1, Constants.PlayerPropType.PropType_Hp);
        int addHurt = _resSvc.GetPropsIncreaseValue(curSelectIdx, curStarLv +1, Constants.PlayerPropType.PropType_Hurt);
        int addDef = _resSvc.GetPropsIncreaseValue(curSelectIdx, curStarLv +1, Constants.PlayerPropType.PropType_Def);
        SetText(txtPropHP1, "生命 +" + addHp);
        SetText(txtPropHurt1, "伤害 +" + addHurt);
        SetText(txtPropDef1, "防御 +" + addDef);

        //下一星级属性数据
        int nextStarLv = curStarLv + 1;
        nextCfg = _resSvc.GetStrongerCfgData(curSelectIdx, nextStarLv);
        if (nextCfg != null)
        {
            SetNextPropShow(true);

            SetText(txtPropHp2, "强化后 +" + nextCfg.addHp);
            SetText(txtPropHurt2, "+" + nextCfg.addHurt);
            SetText(txtPropDef2, "+" + nextCfg.addDef);

            SetText(txtNeedLv, "需要等级：" + nextCfg.minLv);
            SetText(txtCostCoin, "需要消耗：      " + nextCfg.coin);
            SetText(txtCostCrystal, nextCfg.crystal + "/" + playerData.crystal);
        }
        else
        {
            SetNextPropShow(false);
        }

        //道具图标设置
        switch (curSelectIdx)
        {
            case 0:
                SetSprite(imgCurSelectPos, PathDefine.cItemIconHelmet);
                break;

            case 1:
                SetSprite(imgCurSelectPos, PathDefine.cItemIconBody);
                break;

            case 2:
                SetSprite(imgCurSelectPos, PathDefine.cItemIconWaist);
                break;

            case 3:
                SetSprite(imgCurSelectPos, PathDefine.cItemIconArm);
                break;

            case 4:
                SetSprite(imgCurSelectPos, PathDefine.cItemIconLeg);
                break;

            case 5:
                SetSprite(imgCurSelectPos, PathDefine.cItemIconShoes);
                break;

        }
    }

    /// <summary>
    /// 设置下一星级属性的显示与否
    /// </summary>
    /// <param name="bShow"></param>
    private void SetNextPropShow(bool bShow)
    {
        SetActive(txtPropHp2, bShow);
        SetActive(txtPropHurt2, bShow);
        SetActive(txtPropDef2, bShow);
        SetActive(costPanel, bShow);
        SetActive(imgPropArrowHp, bShow);
        SetActive(imgPropArrowHurt, bShow);
        SetActive(imgPropArrowDef, bShow);
    }

    public void UpdateInfo()
    {
        _audioSvc.PlayUIAudio(Constants.cCopyerUpdateSound);
        ClickImage(curSelectIdx);
    }

    #region 点击事件
    /// <summary>
    /// 点击项目事件
    /// </summary>
    /// <param name="idx"></param>
    private void ClickImage(int idx)
    {
        CommonTools.Log("Click Image: " + idx);

        curSelectIdx = idx;
        for (int i = 0; i < imgBgArray.Length; i++)
        {
            Transform trans = imgBgArray[i].transform;
            if(i == curSelectIdx)
            {
                SetSprite(imgBgArray[i],  PathDefine.cItemIconArrorBg);
                trans.localPosition = new Vector3(10, trans.localPosition.y, 0);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 95);
            }
            else
            {
                SetSprite(imgBgArray[i],  PathDefine.cItemIconPlaneBg);
                trans.localPosition = new Vector3(0, trans.localPosition.y, 0);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 85);
            }
        }

        RefreshItemInfo();
    }

    /// <summary>
    /// 关闭界面事件
    /// </summary>
    public void OnClickClose()
    {
        _audioSvc.PlayUIAudio(Constants.cUIClosedBtnSound);
        SetWndState(false);
    }

    /// <summary>
    /// 强化点击事件
    /// </summary>
    public void OnClickStronger()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);

        if (playerData.strongerArray[curSelectIdx] < Constants.cMaxStarLv)
        {
            if (playerData.lv < nextCfg.minLv)
            {
                GameRoot.AddTips("角色等级不够！");
                return;
            }

            if (playerData.coin < nextCfg.coin)
            {
                GameRoot.AddTips("金币不够！");
                return;
            }

            if (playerData.crystal < nextCfg.crystal)
            {
                GameRoot.AddTips("水晶不够！");
                return;
            }

            _netSvc.SendMsg(new NetMsg
            {
                cmd = (int)MsgCommand.Cmd_ReqStronger,
                reqStronger = new RequestStronger
                {
                    stongerType = curSelectIdx,
                },
            });
        }
        else
        {
            GameRoot.AddTips("星级已经升满了！");
        }
    }

    #endregion
}