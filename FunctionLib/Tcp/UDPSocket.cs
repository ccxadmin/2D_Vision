
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionLib.TCP
{
    /// <summary>
    /// UDP 协议传输
    /// </summary>
    public class UDPSocket : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public Socket Socket
        {
            get;
            private set;
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
            get; set;
        }
        /// <summary>
        /// 接收数据编码格式
        /// </summary>
        public Encoding ReceiveEncoding
        {
            get; set;
        }

        /// <summary>
        /// 接收数据编码格式
        /// </summary>
        public string EncodingName
        {
            get; set;
        }
        /// <summary>
        /// 响应数据
        /// </summary>
        public event OnReceiveDataHanlder ReceiveData = null;

        public UDPSocket() { }

        public UDPSocket(string ipString, int port)
        {
            this.Local = new IPEndPoint(IPAddress.Parse(ipString), port);
            this.Bind(this.Local);
        }
        public UDPSocket(EndPoint local)
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
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.Socket.Bind(local);
        }

        #region 接收数据
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="remote"></param>
        /// <returns></returns>
        public int Receive(byte[] buffer)
        {
            return this.Socket.Receive(buffer);
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public bool ReceiveAsync(SocketAsyncEventArgs args)
        {
            return this.Socket.ReceiveAsync(args);
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="remote"></param>
        /// <returns></returns>
        public int ReceiveFrom(byte[] buffer, ref EndPoint remote)
        {
            return this.Socket.ReceiveFrom(buffer, ref remote);
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="remote"></param>
        /// <returns></returns>
        public bool ReceiveFromAsync(SocketAsyncEventArgs args)
        {
            return this.Socket.ReceiveFromAsync(args);
        }
        
        /// <summary>
        /// 开始接收远程对象的数据
        /// </summary>
        /// <param name="remote"></param>
        public bool StartReceive()
        {
            if (this.Socket == null)
            {
                //Logger.Error("Socket对象不能为空！");
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
        /// 开始接收-循环接收
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveThread(object state)
        {
            while (this.State && this.Socket != null)
            {
                try
                {
                    byte[] mBuffer = new byte[10 * 1024]; //10kb
                    EndPoint any = new IPEndPoint(IPAddress.Any, 0);
                    //int count = this.Socket.Receive(this.Buffer);//可以接收数据，但不清楚是哪个远程端发过来的
                    int count = this.Socket.ReceiveFrom(mBuffer, ref any);
                    if (count <= 0)
                        return;

                    //编码格式
                    if (this.ReceiveEncoding == null)
                        this.ReceiveEncoding = System.Text.Encoding.UTF8;

                    string data = this.ReceiveEncoding.GetString(mBuffer, 0, count);
                    //Logger.Info(any.ToString().Replace(":","_"), data);

                    if (this.ReceiveData != null)
                        this.ReceiveData((IPEndPoint)any, mBuffer, count);
                }
                catch (Exception ex)
                {
                    this.State = false;
                    //Logger.Error("接收数据异常：" + ex.ToString());
                }
            }
        }
        #endregion

        #region 向远程UDP目标发送信息
        /// <summary>
        /// 向远程UDP目标发送信息
        /// </summary>
        /// <param name="ipString"></param>
        /// <param name="port"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int SendTo(string ipString, int port, byte[] data)
        {
            IPEndPoint remote = new IPEndPoint(IPAddress.Parse(ipString), port);
            return this.Socket.SendTo(data, remote);
        }
        /// <summary>
        /// 向远程UDP目标发送信息
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int SendTo(EndPoint remote, byte[] data)
        {
            return this.Socket.SendTo(data, remote);
        }
        /// <summary>
        /// 向远程UDP目标发送信息
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool SendToAsync(SocketAsyncEventArgs args)
        {
            return this.Socket.SendToAsync(args);
        }
        /// <summary>
        /// 向远程UDP目标发送信息
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<int> SendToAsync(string ipString, int port, byte[] data)
        {
            return Task.Factory.StartNew<int>(() => {
                return this.SendTo(ipString, port, data);
            });
        }
        /// <summary>
        /// 向远程UDP目标发送信息
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<int> SendToAsync(EndPoint remote, byte[] data)
        {
            return new Task<int>(() => this.SendTo(remote, data));
        }
        
        #endregion

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (this.Socket != null)
            {
                //this.Socket.Disconnect(true);程序卡死

                this.Socket.Close(3);
                this.Socket.Dispose();
                this.Socket = null;
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
