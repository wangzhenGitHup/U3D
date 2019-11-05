/****************************************************
    文件：CreateRoleWnd.cs
	作者：WangZhen
    日期：2019/6/10 20:55:21
	功能：角色创建界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using Protocols;

public class CreateRoleWnd : WindowRoot 
{
    public InputField inputRoleName;

    protected override void InitWnd()
    {
        base.InitWnd();

        //生成一个随机的角色名
        inputRoleName.text = _resSvc.GetRandomRoleName(false);
    }

    public void OnClickRandomBtn()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        //生成一个随机的角色名
        inputRoleName.text = ResSvc.Instance.GetRandomRoleName(false);
    }

    public void OnClickEnterGameBtn()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        if (inputRoleName.text != "")
        {
            //发送消息，进入游戏
            NetMsg msg = new NetMsg
            {
                cmd = (int)MsgCommand.Cmd_ReqRename,
                reqRename = new RequestRename
                {
                    name = inputRoleName.text
                },
            };

            _netSvc.SendMsg(msg);
        }
        else
        {
            GameRoot.AddTips("当前名字不合法！");
        }
    }
}