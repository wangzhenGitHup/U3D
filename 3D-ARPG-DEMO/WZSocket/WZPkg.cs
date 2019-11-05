/****************************************************
	文件：WZPkg.cs
	作者：WangZhen
	日期：2018/10/30 11:20   	
	功能：网络消息包
*****************************************************/

using System;

namespace WZNet {
    class WZPkg {
        public int headLen = 4;
        public byte[] headBuff = null;
        public int headIndex = 0;

        public int bodyLen = 0;
        public byte[] bodyBuff = null;
        public int bodyIndex = 0;

        public WZPkg() {
            headBuff = new byte[4];
        }

        public void InitBodyBuff() {
            bodyLen = BitConverter.ToInt32(headBuff, 0);
            bodyBuff = new byte[bodyLen];
        }

        public void ResetData() {
            headIndex = 0;
            bodyLen = 0;
            bodyBuff = null;
            bodyIndex = 0;
        }
    }
}