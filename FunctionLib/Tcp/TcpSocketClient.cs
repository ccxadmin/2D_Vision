using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionLib.TCP
{
    /// <summary>
    /// Tcp Client
    /// </summary>
      [Serializable]
    public class TcpSocketClient : IDisposable
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
        /// 连接信息
        /// </summary>
        /// <param name="msg"></param>
        public delegate void OnStateInfohandle(string msg);



        public string IP { get; set; } = "127.0.0.1";


        public int Port { get; set; } = 60000;

        
       [NonSerialized]
        public Socket Socket;
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// 接收数据状态
        /// </summary>
        [NonSerialized]
        public bool State;
        //{
        //    get;
        //    private set;
        //}
        /// <summary>
        /// 本地服务器
        /// </summary>
        [NonSerialized]
        public EndPoint Local;
        //{
        //    get;
        //    private set;
        //}
        /// <summary>
        /// 接收数据编码格式
        /// </summary>
        [NonSerialized]
        public Encoding ReceiveEncoding;
        //{
        //    get; set;
        //}

        /// <summary>
        /// 数据编码
        /// </summary>
        public EumEncoding encoding { get; set; } = EumEncoding.UTF_8;
        /// <summary>
        /// 响应数据
        /// </summary>
        [NonSerialized]
        public  OnReceiveDataHanlder ReceiveData = null;

        /// <summary>
        /// 响应数据
        /// </summary>
        [NonSerialized]
        public  OnRemoteCloseHanlder RemoteClose = null;
        /// <summary>
        /// 连接信息
        /// </summary>
        [NonSerialized]
        public  OnStateInfohandle StateInfo = null;
        /// <summary>
        /// 远程服务器
        /// </summary>
        [NonSerialized]
        public EndPoint Remote;
        //{
        //    get;
        //    private set;
        //}

        #region 连接服务端
        /// <summary>
        /// 连接服务端
        /// </summary>
        /// <param name="ipString"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Connect(string ipString, int port)
        {
            IPAddress address = null;
            IPAddress.TryParse(ipString, out address);
            if (address == null)
            {
                //Logger.Info("IP地址格式不正确！" + ipString);
                return false;
            }
            IP = ipString;
            Port = port;
            return Connect(new IPEndPoint(address, port));
        }
        /// <summary>
        /// 连接服务端
        /// </summary>
        /// <param name="remote"></param>
        /// <returns></returns>
        public bool Connect(EndPoint remote)
        {
            try
            {
                this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.Socket.Connect(remote);
                IP = ((IPEndPoint)remote).Address.ToString();
                Port = ((IPEndPoint)remote).Port;
                this.Remote = this.Socket.RemoteEndPoint;
                this.Local = this.Socket.LocalEndPoint;

                this.Socket.SendTimeout = 10 * 1000;//10秒
                this.Socket.ReceiveTimeout = 10 * 1000;//10秒
                StateInfo?.Invoke("连接服务成功！" + this.Socket.RemoteEndPoint.ToString());
                //Logger.Info("连接服务成功！" + this.Socket.RemoteEndPoint.ToString());
              
                return true;
            }
            catch (Exception ex)
            {
                this.Socket = null;
                StateInfo?.Invoke("启动服务异常：" + ex.Message);
                //Logger.Error("启动服务异常：" + ex.ToString());
               
                return false;
            }
        }

        #endregion

        #region 接收数据
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int Receive(byte[] buffer)
        {
            return this.Socket.Receive(buffer);
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool ReceiveAsync(SocketAsyncEventArgs args)
        {
            return this.Socket.ReceiveAsync(args);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Task<int> ReceiveAsync(byte[] buffer)
        {
            return Task.Factory.StartNew<int>(() => {
                return this.Socket.Receive(buffer);
            });
        }
        /// <summary>
        /// 开始异步接收数据
        /// </summary>
        /// <returns></returns>
        public bool StartReceive()
        {
            if (this.Socket == null)
            {
                //Logger.Error("远程对象不能为空！");
                return false;
            }
            try
            {
                this.State = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ReceiveThread));

                //Logger.Info(this.Socket.LocalEndPoint.ToString() + "启动接收数据成功！");
                return true;
            }
            catch (Exception ex)
            {
                //Logger.Error("服务端启动异常：" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 循环接收数据
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveThread(object state)
        {
            while (this.State && this.Socket != null
                && this.Socket.Poll(-1, SelectMode.SelectRead))
            {
                try
                {
                    byte[] mBuffer = new byte[10 * 1024]; //10kb
                    /*2021-06-13 侯连文 返回的远程地址不对-0.0.0.0*/
                    //EndPoint any = new IPEndPoint(IPAddress.Any, 0);
                    //int count = this.Socket.ReceiveFrom(mBuffer, ref any);
                    int count = this.Socket.Receive(mBuffer, mBuffer.Length, 0);
                    if (count <= 0)
                    {
                        //服务端断开
                        this.State = false;
                        this.Close();
                        return;
                    }

                    //编码格式
                    if (this.ReceiveEncoding == null)
                        this.ReceiveEncoding = System.Text.Encoding.UTF8;

                    string data = this.ReceiveEncoding.GetString(mBuffer, 0, count);
                    //Logger.Info(this.Socket.RemoteEndPoint.ToString().Replace(":", "_"), data);

                    if (this.ReceiveData != null)
                        this.ReceiveData((IPEndPoint)this.Socket.RemoteEndPoint, mBuffer, count);
                }
                catch (Exception ex)
                {
                    this.State = false;
                    //Logger.Error("接收数据异常：" + ex.ToString());
                }
            }
        }
        #endregion

        #region 向远程服务器发送数据
        /// <summary>
        /// 向远程服务器发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Send(byte[] data)
        {
            return this.Socket.Send(data);
        }
        /// <summary>
        /// 向远程服务器发送数据
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool SendAsync(SocketAsyncEventArgs args)
        {
            return this.Socket.SendAsync(args);
        }
        /// <summary>
        /// 向远程服务器发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<int> SendAsync(byte[] data)
        {
            return Task.Factory.StartNew<int>(() => {
                return this.Socket.Send(data);
            });
        }
        #endregion

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (this.Socket != null)
            {
                string key=   Socket.RemoteEndPoint.ToString();
                if (this.Socket.Connected)
                    this.Socket.Shutdown(SocketShutdown.Both);

                this.State = false;
                this.Socket.Disconnect(true);

                this.Socket.Close(3);

                this.Socket.Dispose();
                this.Socket = null;

                if (this.RemoteClose != null)
                    this.RemoteClose(key);
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
