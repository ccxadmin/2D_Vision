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
    /// 图像膨胀参数
    /// </summary>
    [Serializable]
   public class DilationParam : BaseParam
    {
        public DilationParam()
        { }

        /*****************************************Property*****************************************/
        #region---Property---
        //Input parmas value define --2

       
        int maskWidth = 3;
        /// <summary>
        /// 掩膜宽度
        /// </summary>
        [Description("掩膜宽度"), DefaultValue(3)]
        public int MaskWidth
        {
            get => this.maskWidth;
            set
            {
                this.maskWidth = value;
            }
        }

        //Output parmas value define ---2

        int maskHeight = 3;
        /// <summary>
        /// 掩膜高度
        /// </summary>
        [Description("掩膜高度"), DefaultValue(3)]
        public int MaskHeight
        {
            get => this.maskHeight;
            set
            {
                this.maskHeight = value;
            }
        }

        //Run status value define----1
        bool dilationRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool DilationRunStatus
        {
            get => dilationRunStatus;
            set
            {
                dilationRunStatus = value;
            }
        }
        #endregion

    }
}
