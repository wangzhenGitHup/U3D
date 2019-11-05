/****************************************************
    文件：LoginWnd.cs
	作者：WangZhen
    日期：2019/6/9 23:4:4
	功能：登陆注册界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using Protocols;

public class LoginWnd : WindowRoot
{
    public InputField inputAccount;
    public InputField inputPwd;
    public Button btnEnterGame;
    public Button btnNotice;

    protected override void InitWnd()
    {
        base.InitWnd();

        //获取本地存储的账号密码
        if (PlayerPrefs.HasKey(Constants.cAccountKey) && PlayerPrefs.HasKey(Constants.cPwdKey))
        {
            inputAccount.text = PlayerPrefs.GetString(Constants.cAccountKey);
            inputPwd.text = PlayerPrefs.GetString(Constants.cPwdKey);
        }
        else
        {
            inputAccount.text = "";
            inputPwd.text = "";
        }


        //TODO:
    }

    public void OnClickEnterBtn()
    {
        _audioSvc.PlayUIAudio(Constants.cLoginBtnSound);

        //获取账号密码并保存
        string accountStr = inputAccount.text;
        string pwdStr = inputPwd.text;
        if (accountStr != "" && pwdStr != "")
        {
            PlayerPrefs.SetString(Constants.cAccountKey, accountStr);
            PlayerPrefs.SetString(Constants.cPwdKey, pwdStr);
            
            //发送网络消息，请求登录
            NetMsg msg = new NetMsg
            {
                cmd = (int)MsgCommand.Cmd_ReqLogin,
                reqLoginMsg = new RequestLoginMsg { _account = accountStr, _pwd = pwdStr}
            };
            _netSvc.SendMsg(msg);
            
            //TO Remove
            //LoginSys.Instance.RspLogin();
        }
        else
        {
            GameRoot.AddTips("账号或密码为空！");
        }
    }

    public void OnClickNoticeBtn()
    {
        _audioSvc.PlayUIAudio(Constants.cNormalUIBtnSound);
        GameRoot.AddTips("敬请期待");
    }
}