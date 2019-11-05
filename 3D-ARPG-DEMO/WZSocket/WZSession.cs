/****************************************************
	文件：WZSession.cs
	作者：WangZhen
	日期：2018/10/30 11:20   	
	功能：网络会话管理
*****************************************************/

using System;
using System.Net.Sockets;

namespace WZNet {
    public abstract class WZSession<T> where T : WZMsg {
        private Socket skt;
        private Action closeCB;

        #region Recevie
        public void StartRcvData(Socket skt, Action closeCB) {
            try {
                this.skt = skt;
                this.closeCB = closeCB;

                OnConnected();

                WZPkg pack = new WZPkg();
                skt.BeginReceive(
                    pack.headBuff,
                    0,
                    pack.headLen,
                    SocketFlags.None,
                    new AsyncCallback(RcvHeadData),
                    pack);
            }
            catch (Exception e) {
                WZTool.LogMsg("StartRcvData:" + e.Message, LogLevel.Error);
            }
        }

        private void RcvHeadData(IAsyncResult ar) {
            try {
                WZPkg pack = (WZPkg)ar.AsyncState;
                int len = skt.EndReceive(ar);
                if (len > 0) {
                    pack.headIndex += len;
                    if (pack.headIndex < pack.headLen) {
                        skt.BeginReceive(
                            pack.headBuff,
                            pack.headIndex,
                            pack.headLen - pack.headIndex,
                            SocketFlags.None,
                            new AsyncCallback(RcvHeadData),
                            pack);
                    }
                    else {
                        pack.InitBodyBuff();
                        skt.BeginReceive(pack.bodyBuff,
                            0,
                            pack.bodyLen,
                            SocketFlags.None,
                            new AsyncCallback(RcvBodyData),
                            pack);
                    }
                }
                else {
                    OnDisConnected();
                    Clear();
                }
            }
            catch (Exception e) {
                WZTool.LogMsg("RcvHeadError:" + e.Message, LogLevel.Error);
            }
        }

        private void RcvBodyData(IAsyncResult ar) {
            try {
                WZPkg pack = (WZPkg)ar.AsyncState;
                int len = skt.EndReceive(ar);
                if (len > 0) {
                    pack.bodyIndex += len;
                    if (pack.bodyIndex < pack.bodyLen) {
                        skt.BeginReceive(pack.bodyBuff,
                            pack.bodyIndex,
                            pack.bodyLen - pack.bodyIndex,
                            SocketFlags.None,
                            new AsyncCallback(RcvBodyData),
                            pack);
                    }
                    else {
                        T msg = WZTool.DeSerialize<T>(pack.bodyBuff);
                        OnReciveMsg(msg);

                        //loop recive
                        pack.ResetData();
                        skt.BeginReceive(
                            pack.headBuff,
                            0,
                            pack.headLen,
                            SocketFlags.None,
                            new AsyncCallback(RcvHeadData),
                            pack);
                    }
                }
                else {
                    OnDisConnected();
                    Clear();
                }
            }
            catch (Exception e) {
                WZTool.LogMsg("RcvBodyError:" + e.Message, LogLevel.Error);

            }
        }
        #endregion

        #region Send
        /// <summary>
        /// Send message data
        /// </summary>
        public void SendMsg(T msg) {
            byte[] data = WZTool.PackLenInfo(WZTool.Serialize<T>(msg));
            SendMsg(data);
        }

        /// <summary>
        /// Send binary data
        /// </summary>
        public void SendMsg(byte[] data) {
            NetworkStream ns = null;
            try {
                ns = new NetworkStream(skt);
                if (ns.CanWrite) {
                    ns.BeginWrite(
                        data,
                        0,
                        data.Length,
                        new AsyncCallback(SendCB),
                        ns);
                }
            }
            catch (Exception e) {
                WZTool.LogMsg("SndMsgError:" + e.Message, LogLevel.Error);
            }
        }

        private void SendCB(IAsyncResult ar) {
            NetworkStream ns = (NetworkStream)ar.AsyncState;
            try {
                ns.EndWrite(ar);
                ns.Flush();
                ns.Close();
            }
            catch (Exception e) {
                WZTool.LogMsg("SndMsgError:" + e.Message, LogLevel.Error);
            }
        }
        #endregion

        /// <summary>
        /// Release Resource
        /// </summary>
        private void Clear() {
            if (closeCB != null) {
                closeCB();
            }
            skt.Close();
        }

        /// <summary>
        /// Connect network
        /// </summary>
        protected virtual void OnConnected() {
            WZTool.LogMsg("New Seesion Connected.", LogLevel.Info);
        }

        /// <summary>
        /// Receive network message
        /// </summary>
        protected virtual void OnReciveMsg(T msg) {
            WZTool.LogMsg("Receive Network Message.", LogLevel.Info);
        }

        /// <summary>
        /// Disconnect network
        /// </summary>
        protected virtual void OnDisConnected() {
            WZTool.LogMsg("Session Disconnected.", LogLevel.Info);
        }
    }
}