using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.参数
{
    /// <summary>
    /// 图像二值化参数
    /// </summary>
    [Serializable]
    public class BinaryzationParam : BaseParam
    {
        public BinaryzationParam()
        {

        }
        /*****************************************Property*****************************************/
        #region---Property---
        //Input parmas value define --2


        byte grayMin = 0;
        /// <summary>
        ///最小灰度值
        /// </summary>
        [Description("最小灰度值"), DefaultValue(0)]
        public byte GrayMin
        {
            get => this.grayMin;
            set
            {
                this.grayMin = value;
            }
        }

        //Output parmas value define ---2

        byte grayMax = 128;
        /// <summary>
        /// 最大灰度值
        /// </summary>
        [Description("最大灰度值"), DefaultValue(128)]
        public byte GrayMax
        {
            get => this.grayMax;
            set
            {
                this.grayMax = value;
            }
        }


        bool isInvert = false;
        /// <summary>
        /// 是否反转图像
        /// </summary>
        [Description("是否反转图像"), DefaultValue(false)]
        public bool IsInvert
        {
            get => this.isInvert;
            set
            {
                this.isInvert = value;
            }
        }

        //Run status value define----1
        bool binaryzationRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool BinaryzationRunStatus
        {
            get => binaryzationRunStatus;
            set
            {
                binaryzationRunStatus = value;
            }
        }
        #endregion

    }
}
