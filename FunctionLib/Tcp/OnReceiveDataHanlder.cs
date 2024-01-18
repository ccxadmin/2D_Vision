using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace FunctionLib.TCP
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
    public delegate void OnRemoteConnectHanlder(string key);

    /// <summary>
    /// 远程关闭
    /// </summary>
    /// <param name="key"></param>
    public delegate void OnRemoteCloseHanlder(string key);
}
