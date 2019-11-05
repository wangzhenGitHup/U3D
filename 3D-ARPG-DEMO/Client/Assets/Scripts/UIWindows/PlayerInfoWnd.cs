/****************************************************
    文件：PlayerInfoWnd.cs
	作者：WangZhen
    日期：2019/6/16 22:21:39
	功能：角色信息界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using Protocols;
using UnityEngine.EventSystems;

public class PlayerInfoWnd : WindowRoot
{
    #region 组件

    public Text txtInfo;
    public Text txtExp;
    public Image imgExpProgress;
    public Text txtPower;
    public Image imgPowerProgress;

    public Text txtJob;
    public Text txtFight;
    public Text txtHP;
    public Text txtHurt;
    public Text txtDef;

    /***************详情相关*****************/
    public Transform transDetailPage;
    public Text txtDetailHp; /*血量*/
    public Text txtDetailAD; /*物攻*/
    public Text txtDetailAP; /*法功*/
    public Text txtDetailADDef; /*物防*/
    public Text txtDetailAPDef; /*法防*/
    public Text txtDetailDodge; /*闪避*/
    public Text txtDetailPierce; /*穿透*/
    public Text txtDetailCritical; /*暴击*/
    /*****************************************/

    #endregion

    public RawImage playerImgRaw;

    private Vector2 curTouchPosVec; /*当前触摸的位置*/

    protected override void InitWnd()
    {
        base.InitWnd();
        SetActive(transDetailPage, false);
        RefreshUI();
        RegisterTouchEvt();
    }

    /// <summary>
    /// 
    /// </summary>
    private void RefreshUI()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;

        SetText(txtInfo, playerData.name + "LV." + playerData.lv);
        SetText(txtExp, playerData.exp + "/" + CommonTools.CalcExpValueByLv(playerData.lv));
        SetText(txtPower, playerData.power + "/" + CommonTools.CalcPowerLimit(playerData.lv));
        SetText(txtJob, "职业  暗夜刺客" );
        SetText(txtFight, "战力  " + CommonTools.CalcFightByPlayerData(playerData));
        SetText(txtHP, "血量  " + playerData.hp);
        SetText(txtHurt, "伤害  " + (playerData.ad + playerData.ap));
        SetText(txtDef, "防御  " + (playerData.addef + playerData.apdef));

        imgExpProgress.fillAmount = playerData.exp * 1.0f / CommonTools.CalcExpValueByLv(playerData.lv);
        imgPowerProgress.fillAmount = playerData.power * 1.0f / CommonTools.CalcPowerLimit(playerData.lv);

        //详情属性设置
        SetText(txtDetailHp,  playerData.hp);
        SetText(txtDetailAD, playerData.ad);
        SetText(txtDetailAP, playerData.ap);
        SetText(txtDetailADDef, playerData.addef);
        SetText(txtDetailAPDef, playerData.apdef);
        SetText(txtDetailDodge, playerData.dodge +"%");
        SetText(txtDetailPierce, playerData.pierce + "%");
        SetText(txtDetailCritical, playerData.critical + "%");
    }

    /// <summary>
    /// 注册触摸事件
    /// </summary>
    private void RegisterTouchEvt()
    {
        OnClickDown(playerImgRaw.gameObject, (PointerEventData evt) =>
        {
            curTouchPosVec = evt.position;
            MainCitySys.Instance.RecordPlayerStartRotate();
        });

        OnDragEvt(playerImgRaw.gameObject, (PointerEventData evt) =>
        {
            float rotate = (evt.position.x - curTouchPosVec.x) * 0.4f;
            MainCitySys.Instance.SetPlayerInfoPlayerRotate(-rotate);
        });
    }

    /// <summary>
    /// 关闭事件
    /// </summary>
    public void OnClickClosed()
    {
        _audioSvc.PlayUIAudio(Constants.cUIClosedBtnSound);
        MainCitySys.Instance.ClosedPlayerInfoWnd();
    }

    /// <summary>
    /// 打开详情事件
    /// </summary>
    public void OnClickDetail()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        SetActive(transDetailPage);
    }

    /// <summary>
    /// 关闭详情界面
    /// </summary>
    public void OnClickClosedDetail()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        SetActive(transDetailPage, false);
    }
}