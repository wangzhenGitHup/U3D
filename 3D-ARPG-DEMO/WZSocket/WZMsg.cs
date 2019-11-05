/****************************************************
	文件：WZMsg.cs
	作者：WangZhen
	日期：2018/10/30 11:20   	
	功能：消息定义类
*****************************************************/

namespace WZNet {

    using System;

    [Serializable]
    public abstract class WZMsg {
        public int seq;
        public int cmd;
        public int err;
    }
}