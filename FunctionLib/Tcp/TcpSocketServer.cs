using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FunctionLib
{
    /// <summary>
    /// Tcp Server
    /// </summary>
    public class TcpSocketServer : IDisposable
    {
        /// <summary>
        /// 接收数据委托
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        public delegate void OnReceiveDataHanlder(IPEndPoint remote, byte[] buffer, int count);

        /// <summary>
        /// 远程连接
        /// </summary>
        public delegate void OnRemoteConnectHanlder();

        /// <summary>
        /// 远程关闭
        /// </summary>
        /// <param name="key"></param>
        public delegate void OnRemoteCloseHanlder(string key);

        /// <summary>
        /// 
        /// </summary>
        public Socket Socket
        {
            get;
            private set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Backlog
        {
            get; set;
        }

        /// <summary>
        /// 接收数据状态
        /// </summary>
        public bool State
        {
            get;
            private set;
        }
        /// <summary>
        /// 本地服务器
        /// </summary>
        public EndPoint Local
        {
            get;
            private set;
        }
        /// <summary>
        /// 接收数据编码格式
        /// </summary>
        public Encoding ReceiveEncoding
        {
            get; set;
        }
        /// <summary>
        /// 响应数据
        /// </summary>
        public event OnRemoteConnectHanlder RemoteConnect = null;
        /// <summary>
        /// 响应数据
        /// </summary>
        public event OnReceiveDataHanlder ReceiveData = null;

        /// <summary>
        /// 响应数据
        /// </summary>
        public event OnRemoteCloseHanlder RemoteClose = null;

        private Dictionary<string, SocketSession> _dicSessions = new Dictionary<string, SocketSession>();
        /// <summary>
        /// 获取连接对象
        /// </summary>
        public Dictionary<string, SocketSession> Sessions
        {
            get { return this._dicSessions; }
        }
        private System.Timers.Timer _SysTimer = null;
        private BackgroundWorker _Worker = null;
        private DateTime _RunTime = DateTime.Now;

        public TcpSocketServer() { }

        public TcpSocketServer(string ipString, int port)
        {
            this.Local = new IPEndPoint(IPAddress.Parse(ipString), port);
            this.Bind(this.Local);
        }
        public TcpSocketServer(IPEndPoint local)
        {
            this.Local = local;
            this.Bind(this.Local);
        }
        /// <summary>
        /// 绑定地址-
        /// </summary>
        /// <param name="local"></param>
        private void Bind(EndPoint local)
        {
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Socket.Bind(local);
        }

        #region 开始绑定接收连接
        /// <summary>
        /// 开始绑定接收连接
        /// </summary>
        /// <param name="ipString"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Start(string ipString, int port)
        {
            IPAddress address = null;
            IPAddress.TryParse(ipString, out address);
            if (address == null)
            {
                //Logger.Info("IP地址格式不正确！" + ipString);
                return false;
            }
            this.Bind(new IPEndPoint(address, port));
            return this.Start();
        }
        /// <summary>
        /// 开始绑定接收连接
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public bool Start(EndPoint local)
        {
            this.Bind(local);

            return this.Start();
        }
        /// <summary>
        /// 开始绑定接收连接
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            if (this.Socket == null)
            {
                //Logger.Error("Socket对象不能为空！");
                return false;
            }
            try
            {
                this.Socket.Listen(this.Backlog);
                //Logger.Info("启动服务成功！" + this.Socket.LocalEndPoint.ToString());
                this.State = true;

                ThreadPool.QueueUserWorkItem(new WaitCallback(Accept));
                //
                this._SysTimer = new System.Timers.Timer();
                this._SysTimer.Interval = 30 * 60 * 1000;//30分钟检查一次
                this._SysTimer.Elapsed += SysTimer_Elapsed;
                this._SysTimer.Start();

                return true;
            }
            catch (Exception ex)
            {
                //Logger.Error("启动服务异常：" + ex.ToString());
                return false;
            }
        }

        private void SysTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this._Worker != null && this._Worker.IsBusy)
            {
                //Logger.Info("检查线程正在运行!");
                TimeSpan timeSpan = DateTime.Now - this._RunTime;
                if (timeSpan.TotalMinutes < 60)
                    return;

                //Logger.Info("检查线程超：" + timeSpan.TotalMinutes + "运行中，需要停止重新启动...");
                this.CheckClient();
            }
        }

        private void CheckClient()
        {
            this._RunTime = DateTime.Now;
            string[] keys = this._dicSessions.Keys.ToArray();
            foreach (var item in keys)
            {
                try
                {
                    int result = this._dicSessions[item].GetState();
                    //Logger.Info("返回结果：" + result);
                }
                catch (Exception ex)
                {
                    //Logger.Error("获取状态异常：" + ex.ToString());
                    this._dicSessions.Remove(item);
                }
            }
        }

        private void Accept(object args)
        {
            while (this.State && this.Socket != null)
            {
                try
                {
                    Socket client = this.Socket.Accept();
                    lock (this._dicSessions)
                    {
                        string key = client.RemoteEndPoint.ToString();//Guid.NewGuid().ToString();
                        if (this._dicSessions.ContainsKey(key) == false)
                        {
                            SocketSession session = new SocketSession(key, client);
                            session.ReceiveEncoding = this.ReceiveEncoding;
                            session.RemoteClose += Session_RemoteClose;
                            session.ReceiveData += Session_ReceiveData;
                            this._dicSessions.Add(key, session);
                            if (this.RemoteConnect != null)
                                this.RemoteConnect();

                            //开始接收数据
                            session.StartReceive();
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Logger.Error("接收客户端异常：" + ex.ToString());
                }
            }
        }

        private void Session_ReceiveData(IPEndPoint remote, byte[] buffer, int count)
        {
            if (this.ReceiveData != null)
                this.ReceiveData(remote, buffer, count);
        }

        private void Session_RemoteClose(string key)
        {
            if (this._dicSessions.ContainsKey(key))
                this._dicSessions.Remove(key);

            if (this.RemoteClose != null)
                this.RemoteClose(key);
        }
        #endregion

        #region 向远程客户端发送信息
        /// <summary>
        /// 向远程客户端发送信息
        /// </summary>
        /// <param name="ipString"></param>
        /// <param name="port"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int SendTo(string key, byte[] data)
        {
            if (this._dicSessions.ContainsKey(key) == false)
            {
                //Logger.Info("未能找到远程连接：" + key);
                return -1;
            }
            var session = this._dicSessions[key];
            return session.Send(data);
        }
        /// <summary>
        /// 向远程客户端发送信息
        /// </summary>
        /// <param name="ipString"></param>
        /// <param name="port"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int SendTo(SocketSession session, byte[] data)
        {
            return session.Send(data);
        }
        /// <summary>
        /// 向远程客户端发送信息
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Dictionary<string, string> SendTo(byte[] data)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            if (this._dicSessions.Count <= 0)
            {
                //Logger.Info("未能找到远程连接!");
                return list;
            }
            bool success = true;
            string[] keys = this._dicSessions.Keys.ToArray();
            foreach (var item in keys)
            {
                try
                {
                    this._dicSessions[item].Send(data);
                }
                catch (Exception ex)
                {
                    success = false;
                    //Logger.Error("发送数据异常：" + ex.ToString());
                    this._dicSessions.Remove(item);//移除发送失败的远程连接
                }
            }
            //存在发送失败的远程
            if (success == false)
            {
                if (this.RemoteClose != null)
                    this.RemoteClose(null);
            }
            return list;
        }

        /// <summary>
        /// 向远程客户端发送信息
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<int> SendToAsync(string key, byte[] data)
        {
            return new Task<int>(() => this.SendTo(key, data));
        }
        /// <summary>
        /// 向远程客户端发送信息
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<Dictionary<string, string>> SendToAsync(byte[] data)
        {
            return Task.Factory.StartNew<Dictionary<string, string>>(() => {
                return this.SendTo(data);
            });
        }


        #endregion
        /// <summary>
        /// 停止监听
        /// </summary>
        public void Close()
        {
            this.ClientClose();

            if (this.Socket != null)
            {
                //由于套接字没有连接并且(当使用一个 sendto 调用发送数据报套接字时)没有提供地址，发送或接收数据的请求没有被接受。
                //this.Socket.Disconnect(true);

                this.Socket.Close(3);
                this.Socket.Dispose();
                this.Socket = null;
            }
        }

        private void ClientClose()
        {
            string[] keys = this._dicSessions.Keys.ToArray();
            foreach (var item in keys)
            {
                try
                {
                    //Logger.Info("开始关闭：" + this._dicSessions[item].Client.RemoteEndPoint.ToString() + "...");
                    this._dicSessions[item].Close();
                    this._dicSessions.Remove(item);
                    //Logger.Info("关闭成功");
                }
                catch (Exception ex)
                {
                    //Logger.Error("关闭异常：" + ex.ToString());
                    this._dicSessions.Remove(item);
                }
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }
    }
}
