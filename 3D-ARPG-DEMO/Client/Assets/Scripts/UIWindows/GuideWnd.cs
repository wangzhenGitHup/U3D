/****************************************************
    文件：GuideWnd.cs
	作者：WangZhen
    日期：2019/6/17 23:10:54
	功能：自动任务引导界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using Protocols;

public class GuideWnd : WindowRoot 
{
    public Text txtName;
    public Text txtDialogContext;
    public Image imgGuider;


    private AutoGuideCfg curGuideTaskData; /*当前任务数据*/
    private string[] dialogStringArray; /*对话内容*/
    private int dialogIdx; /*对话内容数组索引*/
    private PlayerData playerData; /*玩家数据*/

    protected override void InitWnd()
    {
        base.InitWnd();

        //获取玩家数据
        playerData = GameRoot.Instance.PlayerData;
        //获取当前任务数据
        curGuideTaskData = MainCitySys.Instance.GetCurrentGuideTaskData();
        //解析任务数据
        dialogStringArray = curGuideTaskData.dialogArr.Split('#');
        dialogIdx = 1;
        SetTalk();
    }

    /// <summary>
    /// 对话
    /// </summary>
    private void SetTalk()
    {
        string[] talkContextArray = dialogStringArray[dialogIdx].Split('|');

        if (talkContextArray[0] == "0") //自己
        {
            SetSprite(imgGuider, PathDefine.cTaskDialogIcon_Self);
            SetText(txtName, playerData.name);
        }
        else  //NPC
        {
            switch (curGuideTaskData.npcID)
            {
                case 0:
                    SetSprite(imgGuider, PathDefine.cTaskDialogIcon_Wiseman);
                    SetText(txtName, "智者");
                    break;

                case 1:
                    SetSprite(imgGuider, PathDefine.cTaskDialogIcon_General);
                    SetText(txtName, "将军");
                    break;

                case 2:
                    SetSprite(imgGuider, PathDefine.cTaskDialogIcon_Artisan);
                    SetText(txtName, "工匠");
                    break;

                case 3:
                    SetSprite(imgGuider, PathDefine.cTaskDialogIcon_Trader);
                    SetText(txtName, "商人");
                    break;

                default:
                    SetSprite(imgGuider, PathDefine.cTaskDialogIcon_Default);
                    SetText(txtName, "阿丽莎");
                    break;
            }
        }

        imgGuider.SetNativeSize();
        SetText(txtDialogContext, talkContextArray[1].Replace("$name", playerData.name));
    }

    /// <summary>
    /// 下一个点击事件
    /// </summary>
    public void OnClickNext()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        //对话索引变更
        dialogIdx += 1;
        if (dialogIdx >= dialogStringArray.Length)
        {
            //任务完成
            //发送消息
            SendFinishTaskMessage();
            //关闭窗口
            SetWndState(false);
            return;
        }
        SetTalk();
    }

    private void SendFinishTaskMessage()
    {
        NetMsg msg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_ReqGuideTask,
            reqGuideTask = new RequestGuideTask
            {
                taskId = curGuideTaskData.ID,
            }
        };
        _netSvc.SendMsg(msg);
    }
}