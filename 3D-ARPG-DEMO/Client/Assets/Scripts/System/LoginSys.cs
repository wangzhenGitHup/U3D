/****************************************************
    文件：LoginSys.cs
	作者：WangZhen
    日期：2019/6/9 22:17:29
	功能：登录系统
*****************************************************/

using UnityEngine;
using WZNet;
using Protocols;

public class LoginSys : SystemRoot
{
    public LoginWnd _loginWnd; /*登陆界面*/
    public CreateRoleWnd _createRoleWnd; /*创建角色界面*/

    public static LoginSys Instance = null;


    #region 属性

    public LoginWnd GetLoginWnd
    {
        get { return _loginWnd; }
    }
    #endregion

    /// <summary>
    /// 初始化
    /// </summary>
    public override void InitSys()
    {
        base.InitSys();
        CommonTools.Log("Init LoginSys....");
        Instance = this;
    }

    /// <summary>
    /// 进入登陆场景
    /// </summary>
    public void EnterLogin()
    {
        //异步加载登陆场景
        //显示加载的进度条
        //加载完成后再打开注册登陆界面
        _resSvc.AsyncLoadScene(Constants.cSceneLogin, OpenLoginWnd);
    }

    //打开登陆场景
    public void OpenLoginWnd()
    {
        _loginWnd.SetWndState(true);
        _audioSvc.PlayBGM(Constants.cBGM);
    }

    public void RspLogin(NetMsg msg)
    {
        GameRoot.AddTips("登录成功!");
        GameRoot.Instance.SetPlayerData(msg.rspLoginMsg);

        //没有玩家信息
        if (msg.rspLoginMsg._playerData.name == "")
        {
            //打开角色创建界面
            _createRoleWnd.SetWndState(true);
        }
        else
        {
            //进入主城界面
            MainCitySys.Instance.OpenMainCity();
        }
        
        //关闭登录界面
        _loginWnd.SetWndState(false);
    }

    /// <summary>
    /// 重命名响应
    /// </summary>
    /// <param name="msg"></param>
    public void RspRename(NetMsg msg)
    {
        //设置一下玩家的名字
        GameRoot.Instance.SetPlayerDataName(msg.rspRename.name);

        //跳转主场景 进入主城
        //打开主城的界面
        MainCitySys.Instance.OpenMainCity();

        //关闭创建界面
        _createRoleWnd.SetWndState(false);
    }
}