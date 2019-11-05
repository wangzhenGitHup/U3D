using System;
using WZNet;
using Protocols;
using System.Threading;

public class ServerStart
{
    static void Main(string[] args)
    {
        ServerRoot.Instance.Init();

        while (true)
        {
            ServerRoot.Instance.Update();
            //休眠20毫秒，没必要全力跑占用cpu资源
            Thread.Sleep(20);
        }
    }
}

