﻿/****************************************************
	文件：WZSocket.cs
	作者：WangZhen
	日期：2018/10/30 11:20   	
	功能：WZSocekt核心类
*****************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace WZNet {
    public class WZSocket<T, K>
        where T : WZSession<K>, new()
        where K : WZMsg {
        private Socket skt = null;
        public T session = null;
        public int backlog = 10;
        List<T> sessionLst = new List<T>();

        public WZSocket() {
            skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        #region Server
        /// <summary>
        /// Launch Server
        /// </summary>
        public void StartAsServer(string ip, int port) {
            try {
                skt.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                skt.Listen(backlog);
                skt.BeginAccept(new AsyncCallback(ClientConnectCB), skt);
                WZTool.LogMsg("\nServer Start Success!\nWaiting for Connecting......", LogLevel.Info);
            }
            catch (Exception e) {
                WZTool.LogMsg(e.Message, LogLevel.Error);
            }
        }

        void ClientConnectCB(IAsyncResult ar) {
            try {
                Socket clientSkt = skt.EndAccept(ar);
                T session = new T();
                session.StartRcvData(clientSkt, () => {
                    if (sessionLst.Contains(session)) {
                        sessionLst.Remove(session);
                    }
                });
                sessionLst.Add(session);
            }
            catch (Exception e) {
                WZTool.LogMsg(e.Message, LogLevel.Error);
            }
            skt.BeginAccept(new AsyncCallback(ClientConnectCB), skt);
        }
        #endregion

        #region Client
        /// <summary>
        /// Launch Client
        /// </summary>
        public void StartAsClient(string ip, int port) {
            try {
                skt.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), new AsyncCallback(ServerConnectCB), skt);
                WZTool.LogMsg("\nClient Start Success!\nConnecting To Server......", LogLevel.Info);
            }
            catch (Exception e) {
                WZTool.LogMsg(e.Message, LogLevel.Error);
            }
        }

        void ServerConnectCB(IAsyncResult ar) {
            try {
                skt.EndConnect(ar);
                session = new T();
                session.StartRcvData(skt, null);
            }
            catch (Exception e) {
                WZTool.LogMsg(e.Message, LogLevel.Error);
            }
        }
        #endregion

        public void Close() {
            if (skt != null) {
                skt.Close();
            }
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="log">log switch</param>
        /// <param name="logCB">log function</param>
        public void SetLog(bool log = true, Action<string, int> logCB = null) {
            if (log == false) {
                WZTool.log = false;
            }
            if (logCB != null) {
                WZTool.logCB = logCB;
            }
        }
    }
}