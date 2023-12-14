using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;


namespace PositionToolsLib.参数
{
    /// <summary>
    /// 胶水检测工具参数
    /// </summary>
    [Serializable]
    public class BaseParam
    {

        HTuple hv_HomMat2D = new HTuple ();
        /// <summary>
        /// 坐标转换矩阵
        /// </summary>
        [Description("坐标转换矩阵"), DefaultValue(null)]
        public HTuple Hv_HomMat2D
        {
            get => this.hv_HomMat2D;
            set
            {
                this.hv_HomMat2D = value;
            }
        }

        string inputImageName = "原始图像";
        /// <summary>
        /// 输入图像名称
        /// </summary>
        [Description("输入图像名称"), DefaultValue("原始图像")]
        public string InputImageName
        {
            get => this.inputImageName;
            set
            {
                this.inputImageName = value;
            }
        }

        [NonSerialized]
        HObject inputImg = null;
        /// <summary>
        /// 输入图像（图像类不做序列化处理）
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Description("输入图像"), DefaultValue(null)]
        public HObject InputImg
        {
            get => this.inputImg;
            set
            {
                this.inputImg = value;
            }
        }



        [NonSerialized]
        HObject outputImg = null;
        /// <summary>
        /// 输出图像(包含图像结果显示Overlay等)（图像类不做序列化处理）
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Description("输出图像"), DefaultValue(null)]
        public HObject OutputImg
        {
            get => this.outputImg;
            set
            {
                this.outputImg = value;
            }
        }

    }
}
