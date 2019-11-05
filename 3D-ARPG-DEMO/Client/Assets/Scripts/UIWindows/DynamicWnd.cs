/****************************************************
    文件：DynamicWnd.cs
	作者：WangZhen
    日期：2019/6/10 0:32:12
	功能：动态UI元素界面
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWnd : WindowRoot 
{
    public Animation tipsAnimation;
    public Text txtTips;
    public Transform transHpRoot;
    public Animation dodgeAnimation;

    private Queue<string> tipsQueue = new Queue<string>();
    private Dictionary<string, ItemHpBarWnd> dictItemHP = new Dictionary<string, ItemHpBarWnd>();

    private bool isTipShow = false;

    private void Update()
    {
        if (tipsQueue.Count > 0 && isTipShow == false)
        {
            lock (tipsQueue)
            {
                string tipStr = tipsQueue.Dequeue();
                isTipShow = true;
                SetTips(tipStr);
            }
        }
    }

    protected override void InitWnd()
    {
        base.InitWnd();

        SetActive(txtTips, false);
    }

    public void AddTips(string tipStr)
    {
        lock (tipsQueue)
        {
            tipsQueue.Enqueue(tipStr);
        }
    }

    public void SetTips(string tipsStr)
    {
        SetActive(txtTips);
        SetText(txtTips, tipsStr);

        AnimationClip aniClip = tipsAnimation.GetClip("TipsShowAni");
        tipsAnimation.Play();

        //延时关闭
        StartCoroutine(CloseAnimation(aniClip.length, () =>
        {
            //关闭tips
            SetActive(txtTips, false);
            isTipShow = false;
        }));
    }

    private IEnumerator CloseAnimation(float fTimes, Action doneCallback)
    {
        yield return new WaitForSeconds(fTimes);
        if (doneCallback != null)
        {
            doneCallback();
        }
    }

    public void AddHpBarItem(string name, int hp, Transform trans)
    {
        ItemHpBarWnd item = null;
        if (dictItemHP.TryGetValue(name, out item))
        {
            //return;
        }

        GameObject objHpItem = _resSvc.LoadPrefab(PathDefine.cItemHpBarPrefab, true);
        if (objHpItem != null)
        {

            objHpItem.transform.SetParent(transHpRoot);
            objHpItem.transform.localPosition = new Vector3(-1000, 0, 0);
            ItemHpBarWnd itemHp = objHpItem.GetComponent<ItemHpBarWnd>();
            itemHp.SetInfo(hp, trans);
            dictItemHP.Add(name, itemHp);
        }
    }

    public void RemoveHpBarItem(string name)
    {
        ItemHpBarWnd hpBar = null;
        if (dictItemHP.TryGetValue(name, out hpBar))
        {
            dictItemHP.Remove(name);
            Destroy(hpBar.gameObject);
        }
    }

    public void RemoveAllHpBarItems()
    {
        foreach (var item in dictItemHP)
        {
            Destroy(item.Value.gameObject);
        }

        dictItemHP.Clear();
    }

    public void SetDodge(string key)
    {
        ItemHpBarWnd hpBar = null;
        if (dictItemHP.TryGetValue(key, out hpBar))
        {
            hpBar.SetDodgeVal();
        }
    }

    public void SetCritical(string key, int critical)
    {
        ItemHpBarWnd hpBar = null;
        if (dictItemHP.TryGetValue(key, out hpBar))
        {
            hpBar.SetCriticalVal(critical);
        }
    }

    public void SetHurt(string key, int hurt)
    {
        ItemHpBarWnd hpBar = null;
        if (dictItemHP.TryGetValue(key, out hpBar))
        {
            hpBar.SetHurtVal(hurt);
        }
    }

    public void SetHpVal(string key, int oldVal, int newVal)
    {
        ItemHpBarWnd hpBar = null;
        if (dictItemHP.TryGetValue(key, out hpBar))
        {
            hpBar.SetHpVal(oldVal, newVal);
        }
    }

    public void SetPlayerDodge()
    {
        dodgeAnimation.Stop();
        dodgeAnimation.Play();
    }
}