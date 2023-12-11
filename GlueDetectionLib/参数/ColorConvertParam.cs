using GlueDetectionLib.窗体.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlueDetectionLib.参数
{
    /// <summary>
    /// 图像颜色转换参数
    /// </summary>
    [Serializable]
    public class ColorConvertParam : BaseParam
    {
        public ColorConvertParam()
        {

        }
        /*****************************************Property*****************************************/
        #region---Property---
        //Input parmas value define --2

        EumImgFormat imgFormat = EumImgFormat.gray;
        /// <summary>
        /// 图像格式
        /// </summary>
        [Description("图像格式"), DefaultValue(typeof(EumImgFormat), "gray")]
        public EumImgFormat ImgFormat
        {
            get => this.imgFormat;
            set
            {
                this.imgFormat = value;
            }
        }

     
        //Run status value define----1
        bool colorConvertRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool ColorConvertRunStatus
        {
            get => colorConvertRunStatus;
            set
            {
                colorConvertRunStatus = value;
            }
        }
        #endregion
    }
}
