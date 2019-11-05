/****************************************************
    文件：FinishBattleWnd.cs
	作者：WangZhen
    日期：2019/7/2 22:50:3
	功能：战斗结算界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;


public class FinishBattleWnd : WindowRoot {
    public Transform transParent;
    public Button btnClose;
    public Button btnExit;
    public Button btnConfirm;
    public Text txtTime;
    public Text txtLeftHpVal;
    public Text txtRewardInfo;
    public Animation animReward;

    FinishBattleType finishBattleType = FinishBattleType.None;
    int curChapterId = -1;
    int battleCostTime = 0;
    int playerLeftHp = 0;

    protected override void InitWnd()
    {
        base.InitWnd();
        RefreshUI();
    }

    public void SetEndBattleType(FinishBattleType wndType)
    {
        finishBattleType = wndType;
    }

    private void RefreshUI()
    {
        switch (finishBattleType)
        {
            case FinishBattleType.Pause:
                SetActive(transParent, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject);
                break;

            case FinishBattleType.Win:
                SetActive(transParent, false);
                SetActive(btnExit.gameObject, false);
                SetActive(btnClose.gameObject, false);
                SetBattleResultShow();
                break;

            case FinishBattleType.Lose:
                SetActive(transParent, false);
                SetActive(btnClose.gameObject, false);
                SetActive(btnExit.gameObject);
                _audioSvc.PlayUIAudio(Constants.cBattleLose);
                break;
        }
    }

    public void SetBattleResultData(int chapterId, int costTime, int leftHp)
    {
        curChapterId = chapterId;
        battleCostTime = costTime;
        playerLeftHp = leftHp;
    }

    private void SetBattleResultShow()
    {
        MapCfg mapCfg = _resSvc.GetMapCfgData(curChapterId);
        int min = battleCostTime / 60;
        int sec = battleCostTime % 60;
        int coin = mapCfg.rewardCoin;
        int exp = mapCfg.rewardExp;
        int crystal = mapCfg.rewardCrystal;
        SetText(txtTime, "costtimes: " + min + "minutes" + sec + "seconds");
        SetText(txtLeftHpVal, "leftHp：" + playerLeftHp);
        SetText(txtRewardInfo, "Reward：" + 
            Constants.Color(coin + "coin", Constants.TextColor. Green) +
            Constants.Color(exp + "exp", Constants.TextColor.Yellow) +
            Constants.Color(crystal + "crystal", Constants.TextColor.Blue));

        _timerSvc.AddTimeTask((int tid) =>
        {
            SetActive(transParent);
            animReward.Play();
            _timerSvc.AddTimeTask((int subId1) =>
            {
                _audioSvc.PlayUIAudio(Constants.cCopyerUpdateSound);
                _timerSvc.AddTimeTask((int subId2) =>
                {
                    _audioSvc.PlayUIAudio(Constants.cCopyerUpdateSound);
                    _timerSvc.AddTimeTask((int subId3) =>
                    {
                        _audioSvc.PlayUIAudio(Constants.cCopyerUpdateSound);
                        _timerSvc.AddTimeTask((int subId4) =>
                        {
                            _audioSvc.PlayUIAudio(Constants.cBattleWin);
                        }, 300);
                    }, 270);
                }, 270);
            }, 325);
        }, 1000);

    }

    public void OnClickClosed()
    {
        _audioSvc.PlayUIAudio(Constants.cUIClosedBtnSound);
        BattleSys.Instance.BattleMgr._bIsPauseGame = false;
        SetWndState(false);
    }

    public void OnClickExit()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        //进入主城
        MainCitySys.Instance.OpenMainCity();
        //销毁当前战斗
        BattleSys.Instance.DestroyBattle();
    }

    public void OnClickConfirm()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        //进入主城
        MainCitySys.Instance.OpenMainCity();
        //销毁当前战斗
        BattleSys.Instance.DestroyBattle();
        //打开副本界面，打下一关
        CopyerSys.Instance.OpenCopyerWnd();
    }
}

public enum FinishBattleType
{
    None,
    Pause,
    Win,
    Lose,
}
