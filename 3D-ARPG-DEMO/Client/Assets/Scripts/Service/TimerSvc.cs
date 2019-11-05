/****************************************************
    文件：TimerSvc.cs
	作者：WangZhen
    日期：2019/6/20 19:10:32
	功能：定时器服务
*****************************************************/

using UnityEngine;
using System;

public class TimerSvc : SystemRoot 
{
    public static TimerSvc Instance = null;

    private WZTimer timer;

    public void InitSvc()
    {
        Instance = this;

        timer = new WZTimer();
        timer.SetLog((string info) =>
        {
            CommonTools.Log(info);
        });
    }

    public int AddTimeTask(Action<int> callback, double delay, 
        WZTimeUnit timeUnit = WZTimeUnit.Millisecond, int count = 1)
    {
        return timer.AddTimeTask(callback, delay, timeUnit, count);
    }

    public void Update()
    {
        timer.Update();
    }

    public double GetNowTime()
    {
        return timer.GetMillisecondsTime();
    }

    public void RemoveTask(int tid)
    {
        timer.DeleteTimeTask(tid);
    }
}