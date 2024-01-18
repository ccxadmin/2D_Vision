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
    /// Socket 会话
    /// </summary>
    [Serializable]
    public class SocketSession : IDisposable
    {

    
        /// <summary>
        /// 标识
        /// </summary>
        public string Key
        {
            get;
            private set;
        }
        /// <summary>
        /// 客户端
        /// </summary>
        [NonSerialized]
        public Socket Client;
        //{
        //    get;
        //    private set;
        //}
        /// <summary>
        /// 接收数据状态
        /// </summary>
        public bool State
        {
            get;
            private set;
        }

        /// <summary>
        /// 接收数据编码格式
        /// </summary>
        [NonSerialized]
        public Encoding ReceiveEncoding;
        //{
        //    get; set;
        //}
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

        public SocketSession(string key, Socket client)
        {
            this.Key = key;
            this.Client = client;
        }

        #region 接收数据
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int Receive(byte[] buffer)
        {
            return this.Client.Receive(buffer);
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool ReceiveAsync(SocketAsyncEventArgs args)
        {
            return this.Client.ReceiveAsync(args);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Task<int> ReceiveAsync(byte[] buffer)
        {
            return Task.Factory.StartNew<int>(() => {
                return this.Client.Receive(buffer);
            });
        }
        /// <summary>
        /// 开始异步接收数据
        /// </summary>
        /// <returns></returns>
        public bool StartReceive()
        {
            if (this.Client == null)
            {
                //Logger.Error("远程对象不能为空！");
                return false;
            }
            try
            {
                this.State = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ReceiveThread));

                //Logger.Info(this.Client.LocalEndPoint.ToString() + "启动接收数据成功！");
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
            while (this.State && this.Client != null && this.Client.Connected)
            {
                try
                {
                    byte[] mBuffer = new byte[10 * 1024]; //10kb
                    /*2021-06-13 返回的远程地址不对*/
                    //EndPoint any = new IPEndPoint(IPAddress.Any, 0);
                    //int count = this.Socket.ReceiveFrom(mBuffer, ref any);
                    int count = this.Client.Receive(mBuffer);
                    if (count <= 0)
                    {
                        //客户端断开
                        this.State = false;
                        this.Close();
                        return;
                    }
                    //编码格式
                    if (this.ReceiveEncoding == null)
                        this.ReceiveEncoding = System.Text.Encoding.UTF8;

                    string data = this.ReceiveEncoding.GetString(mBuffer, 0, count);
                    //Logger.Info(this.Client.RemoteEndPoint.ToString().Replace(":", "_"), data);

                    if (this.ReceiveData != null)
                        this.ReceiveData((IPEndPoint)this.Client.RemoteEndPoint, mBuffer, count);
                }
                catch (Exception ex)
                {
                    this.State = false;
                    //Logger.Error("接收数据异常：" + ex.ToString());
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <returns></returns>
        public int GetState()
        {
            return this.Send(System.Text.Encoding.UTF8.GetBytes("GetState"));
        }

        #region 向远程服务器发送数据
        /// <summary>
        /// 向远程服务器发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Send(byte[] data)
        {
            return this.Client.Send(data);
        }
        /// <summary>
        /// 向远程服务器发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendAsync(SocketAsyncEventArgs args)
        {
            return this.Client.SendAsync(args);
        }
        /// <summary>
        /// 向远程服务器发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<int> SendAsync(byte[] data)
        {
            return Task.Factory.StartNew<int>(() => {
                return this.Client.Send(data);
            });
        }
        #endregion
        /// <summary>
        /// 关闭-客户端断开
        /// </summary>
        public void Close()
        {
            if (this.Client != null)
            {
                if (this.Client.Connected)
                    this.Client.Shutdown(SocketShutdown.Both);

                //this.Client.Disconnect(true);//

                this.Client.Close(3);
                this.Client = null;

                if (this.RemoteClose != null)
                    this.RemoteClose(this.Key);
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
