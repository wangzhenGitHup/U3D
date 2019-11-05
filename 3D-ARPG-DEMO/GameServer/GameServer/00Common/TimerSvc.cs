/*定时器*/

using System;
using System.Collections.Generic;


public class TimerSvc
{
    private static TimerSvc s_instance = null;

    public static TimerSvc Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new TimerSvc();
            }

            return s_instance;
        }
    }

    private WZTimer timer = null;
    private Queue<TaskPack> taskPackQueue = new Queue<TaskPack>(); /*任务队列*/
    private static readonly string taskLock = "taskLock"; /*任务锁*/

    public void Init()
    {
        CommonTools.Log("TimerSvc Init ....");

        taskPackQueue.Clear();
        timer = new WZTimer(100);
        timer.SetHandle((Action<int> callback, int tid) =>
        {
            if (callback != null)
            {
                lock (taskLock)
                {
                    taskPackQueue.Enqueue(new TaskPack(tid, callback));
                }
            }
        });
        timer.SetLog((string info) =>{
            CommonTools.Log(info);
        });
    }

    public void Update()
    {
        while (taskPackQueue.Count > 0)
        {
            TaskPack taskPack = null;
            lock (taskLock)
            {
                taskPack = taskPackQueue.Dequeue();
            }

            if (taskPack != null)
            {
                taskPack._callback(taskPack._tid);
            }
        }
    }

    public int AddTimeTask(Action<int> callback, double delay, WZTimeUnit timeUnit = WZTimeUnit.Millisecond, int count = 1)
    {
        return timer.AddTimeTask(callback, delay, timeUnit, count);
    }

    public long GetNowTime()
    {
        return (long)timer.GetMillisecondsTime();
    }

    /// <summary>
    /// 任务包 类
    /// </summary>
    class TaskPack
    {
        public int _tid;
        public Action<int> _callback;
        public TaskPack(int tid, Action<int> cb)
        {
            _tid = tid;
            _callback = cb;
        }
    }

}