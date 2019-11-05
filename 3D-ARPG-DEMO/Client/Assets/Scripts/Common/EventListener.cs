/****************************************************
    文件：EventListener.cs
	作者：WangZhen
    日期：2019/6/15 21:29:46
	功能：UI事件监听工具
*****************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EventListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler
{
    public Action<object> onClickImage; /*图片点击事件*/
    public Action<PointerEventData> OnClickDown; /*事件委托 按下*/
    public Action<PointerEventData> OnClickUp; /*事件委托 抬起*/
    public Action<PointerEventData> OnDragEvt; /*事件委托 拖动*/
    public object args;

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragEvt != null)
        {
            OnDragEvt(eventData);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnClickDown != null)
        {
            OnClickDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (OnClickUp != null)
        {
            OnClickUp(eventData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClickImage != null)
        {
            onClickImage(args);
        }
    }
}