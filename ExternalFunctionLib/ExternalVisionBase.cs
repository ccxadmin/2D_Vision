using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalFunctionLib
{
    /// <summary>
    /// 外部视觉基类
    /// </summary>
    abstract   public  class ExternalVisionBase
    {
        /// <summary>
        /// 外部视觉基类
        /// </summary>
        public ExternalVisionBase()
        {

        }
        /// <summary>
        /// 外部视觉是否正常连接
        /// </summary>
        abstract public bool IsConnected { get; protected set; }

        /// <summary>
        ///开始外部TCP连接
        /// </summary>
        abstract public void StartConnect();

        /// <summary>
        ///断开外部TCP连接
        /// </summary>
        abstract public void Disconnect();

        /// <summary>
        /// 调试界面显示
        /// </summary>
        abstract public void DebugFormShow();
        /// <summary>
        /// 调试界面关闭释放
        /// </summary>
        abstract public void DebugFormRelease();
        /// <summary>
        /// 调试界面隐藏
        /// </summary>
        abstract public void DebugFormHide();

        /// <summary>
        /// 异常信息通知事件
        /// </summary>
        abstract public event EventHandler<ExceptionArgs> ExceptionInfoHandle;


    }

    /// <summary>
    /// 异常信息
    /// </summary>
    public class ExceptionArgs : EventArgs
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errInfo { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public int errCode { get; set; }
    }

}
