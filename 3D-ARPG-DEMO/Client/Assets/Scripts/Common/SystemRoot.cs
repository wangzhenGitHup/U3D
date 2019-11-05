/****************************************************
    文件：SystemRoot.cs
	作者：WangZhen
    日期：2019/6/10 0:21:22
	功能：Nothing
*****************************************************/

using UnityEngine;

public class SystemRoot : MonoBehaviour 
{
    protected ResSvc _resSvc; /*资源服务*/
    protected AudioSvc _audioSvc; /*音效服务*/
    protected NetSvc _netSvc; /*网络服务*/
    protected TimerSvc _timeSvc; /*定时服务*/

    public virtual void InitSys()
    {
        _resSvc = ResSvc.Instance;
        _audioSvc = AudioSvc.Instance;
        _netSvc = NetSvc.Instance;
        _timeSvc = TimerSvc.Instance;
    }
}