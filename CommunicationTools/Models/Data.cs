using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationTools.Models
{
    [Serializable]
    public class Data
    {
        /// <summary>
        /// 连接端口
        /// </summary>
        static public string port = "COM1";
        /// <summary>
        /// 通道1亮度值
        /// </summary>
        static public byte ch1Value = 0;
        /// <summary>
        /// 通道2亮度值
        /// </summary>
        static public byte ch2Value = 0;
        /// <summary>
        /// 通道3亮度值
        /// </summary>
        static public byte ch3Value = 0;
        /// <summary>
        /// 通道4亮度值
        /// </summary>
        static public byte ch14Value = 0;
    }
    /// <summary>
    /// 光源模式
    /// </summary>
    public enum EumLightMode
    {
        /// <summary>
        /// 常亮
        /// </summary>
        normal_on,
        /// <summary>
        /// 常灭
        /// </summary>
        normal_off,
        /// <summary>
        /// 未知
        /// </summary>
        unknown
    }

    /// <summary>
    /// 通道值转换符
    /// </summary>
    public enum EnumChn
    {
        A = 1,
        B,
        C,
        D
    }

    /// <summary>
    /// 工具运行结果基类
    /// </summary>
    [Serializable]
    public class RunResult
    {
        [NonSerialized]
        public int m_index = 0;

        private bool m_runFlag = false;
        /// <summary>
        /// 运行状态
        /// </summary>
        public bool runFlag
        {
            get => this.m_runFlag;
            set
            {
                m_runFlag = value;
                m_index = 1;
            }
        }

        /// <summary>
        /// 运行时间
        /// </summary>
        public long runTime = 0;
        /// <summary>
        /// 工具运行错误信息
        /// </summary>
        public string errInfo = "";
        /// <summary>
        /// 附件信息（分支工具的选择结果，循环的次数等）
        /// </summary>
        public object options = null;
    }
}
