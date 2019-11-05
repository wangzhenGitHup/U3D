/****************************************************
    文件：WindowRoot.cs
	作者：WangZhen
    日期：2019/6/9 23:31:49
	功能：UI界面基类
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WindowRoot : MonoBehaviour 
{
    protected ResSvc _resSvc = null; /*资源服务*/
    protected AudioSvc _audioSvc = null; /*音效服务*/
    protected NetSvc _netSvc = null; /*网络服务*/
    protected TimerSvc _timerSvc = null; /*定时器服务*/

    /// <summary>
    /// 设置窗口可见性
    /// </summary>
    /// <param name="bActive"></param>
    public void SetWndState(bool bActive = true)
    {
        if (gameObject.activeSelf != bActive)
        {
            SetActive(gameObject, bActive);
        }

        if(bActive)
        {
            InitWnd();
        }
        else
        {
            ClearWnd();
        }
    }

    /// <summary>
    /// 初始化窗口
    /// </summary>
    protected virtual void InitWnd()
    {
        _resSvc = ResSvc.Instance;
        _audioSvc = AudioSvc.Instance;
        _netSvc = NetSvc.Instance;
        _timerSvc = TimerSvc.Instance;
    }

    /// <summary>
    /// 清理窗口
    /// </summary>
    protected virtual void ClearWnd()
    {
        _resSvc = null;
        _audioSvc = null;
        _netSvc = null;
        _timerSvc = null;
    }


    #region 设置对象可见性
    
    protected void SetActive(GameObject obj, bool bActive = true)
    {
        obj.SetActive(bActive);
    }

    protected void SetActive(Transform trans, bool bActive = true)
    {
        trans.gameObject.SetActive(bActive);
    }

    protected void SetActive(RectTransform rectTrans, bool bActive = true)
    {
        rectTrans.gameObject.SetActive(bActive);
    }

    protected void SetActive(Image img, bool bActive = true)
    {
        img.transform.gameObject.SetActive(bActive);
    }

    protected void SetActive(Text txtHandler, bool bActive = true)
    {
        txtHandler.transform.gameObject.SetActive(bActive);
    }
    #endregion

    #region 设置文本
    protected void SetText(Text txtHandler, string context = "")
    {
        txtHandler.text = context;
    }

    protected void SetText(Transform trans, int num = 0)
    {
        SetText(trans.GetComponent<Text>(), num);
    }

    protected void SetText(Transform trans, string context = "")
    {
        SetText(trans.GetComponent<Text>(), context);
    }

    protected void SetText(Text txtHandler, int num = 0)
    {
        SetText(txtHandler, num.ToString());
    }
    #endregion

    /// <summary>
    /// 获取物体上有无组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objItem"></param>
    /// <returns></returns>
    protected T GetOrAddComponect<T>(GameObject objItem) where T  : Component
    {
        T compent = objItem.GetComponent<T>();
        if (compent == null)
        {
            compent = objItem.AddComponent<T>();
        }

        return compent;
    }

    protected void OnClickDown(GameObject objItem, Action<PointerEventData> evtCallback)
    {
        EventListener evtListenter = GetOrAddComponect<EventListener>(objItem);
        evtListenter.OnClickDown = evtCallback;
    }

    protected void OnClickUp(GameObject objItem, Action<PointerEventData> evtCallback)
    {
        EventListener evtListenter = GetOrAddComponect<EventListener>(objItem);
        evtListenter.OnClickUp = evtCallback;
    }

    protected void OnDragEvt(GameObject objItem, Action<PointerEventData> evtCallback)
    {
        EventListener evtListenter = GetOrAddComponect<EventListener>(objItem);
        evtListenter.OnDragEvt = evtCallback;
    }

    protected void OnClickImageEvt(GameObject objItem, Action<object> evtCallback, object args)
    {
        EventListener evtListener = GetOrAddComponect<EventListener>(objItem);
        evtListener.onClickImage = evtCallback;
        evtListener.args = args;
    }

    /// <summary>
    /// 设置图标
    /// </summary>
    /// <param name="img">要设置的对象</param>
    /// <param name="path">图片路径</param>
    protected void SetSprite(Image img, string path)
    {
        Sprite sp = _resSvc.LoadSprite(path, true);
        img.sprite = sp;
    }

    public bool GetWndState()
    {
        return gameObject.activeSelf;
    }

    protected Transform GetTransformItem(Transform parent, string name)
    {
        if (parent != null)
        {
            return parent.Find(name);
        }

        return transform.Find(name);
    }
}