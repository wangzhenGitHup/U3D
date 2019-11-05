/****************************************************
    文件：CopyerWnd.cs
	作者：WangZhen
    日期：2019/6/22 21:8:39
	功能：副本界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using Protocols;

public class CopyerWnd : WindowRoot 
{
    public Transform curPointer;
    public Button[] btnCopyerArr;

    private PlayerData playerData;

    protected override void InitWnd()
    {
        base.InitWnd();
        playerData = GameRoot.Instance.PlayerData;
        RefreshUI();
    }

    private void RefreshUI()
    {
        int chapterID = playerData.curChapter;
        for (int i = 0; i < btnCopyerArr.Length; i++)
        {
            if (i < (chapterID % 10000))
            {
                SetActive(btnCopyerArr[i].gameObject);
                if (i == (chapterID % 10000 - 1))
                {
                    curPointer.SetParent(btnCopyerArr[i].transform);
                    curPointer.localPosition = new Vector3(25, 100, 0);
                }
            }
            else
            {
                SetActive(btnCopyerArr[i].gameObject, false);
            }
        }
    }

    public void OnClickClose()
    {
        _audioSvc.PlayUIAudio(Constants.cUIClosedBtnSound);
        SetWndState(false);
    }

    public void OnClickFight(int chapterId)
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);

        //检测体力是否足够
        int needPower = _resSvc.GetMapCfgData(chapterId).costPower;
        if (needPower > playerData.power)
        {
            GameRoot.AddTips("体力值不足！");
            return;
        }

        _netSvc.SendMsg(new NetMsg
        {
            cmd =(int)MsgCommand.Cmd_ReqCopyerFight,
            reqCopyerFight = new RequestCopyerFight
            {
                chapterID = chapterId
            }
        });
    }

  
}