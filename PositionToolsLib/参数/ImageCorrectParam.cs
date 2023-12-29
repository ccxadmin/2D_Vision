using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.参数
{
    public class ImageCorrectParam : BaseParam
    {
        HTuple hv_CamParam ;
        /// <summary>
        /// 相机内参
        /// </summary>
        [Description("相机内参"), DefaultValue(null)]
        public HTuple Hv_CamParam
        {
            get => this.hv_CamParam;
            set
            {
                this.hv_CamParam = value;
            }
        }


        //Run status value define----1
        bool imageCorrectRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool ImageCorrectRunStatus
        {
            get => imageCorrectRunStatus;
            set
            {
                imageCorrectRunStatus = value;
            }
        }

    }
}
