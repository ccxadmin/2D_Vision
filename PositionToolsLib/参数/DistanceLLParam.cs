using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.参数
{
    /// <summary>
    /// 线线距离检测参数
    /// </summary>
    [Serializable]
    public class DistanceLLParam : BaseParam
    {
        HTuple hv_CamParam;
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

        string camParamFilePath;
        /// <summary>
        ///  相机内参存放路径
        /// </summary>    
        [Description("相机内参存放路径"), DefaultValue("")]
        public string CamParamFilePath
        {
            get => this.camParamFilePath;
            set
            {
                this.camParamFilePath = value;
            }
        }

        HTuple hv_CamPose;
        /// <summary>
        /// 相机位姿
        /// </summary>
        [Description("相机位姿"), DefaultValue(null)]
        public HTuple Hv_CamPose
        {
            get => this.hv_CamPose;
            set
            {
                this.hv_CamPose = value;
            }
        }
        string camPoseFilePath;
        /// <summary>
        ///  相机位姿存放路径
        /// </summary>    
        [Description("相机位姿存放路径"), DefaultValue("")]
        public string CamPoseFilePath
        {
            get => this.camPoseFilePath;
            set
            {
                this.camPoseFilePath = value;
            }
        }

        string inputLineName = "";
        /// <summary>
        /// 直线1名称
        /// </summary>
        [Description("直线1名称"), DefaultValue("")]
        public string InputLineName
        {
            get => this.inputLineName;
            set
            {
                this.inputLineName = value;
            }
        }

        StuLineData lineData = new StuLineData();
        /// <summary>
        /// 直线1
        /// </summary>
        [Description("直线1"), DefaultValue("")]
        public StuLineData LineData
        {
            get => this.lineData;
            set
            {
                this.lineData = value;
            }
        }


        string inputLine2Name = "";
        /// <summary>
        /// 直线2名称
        /// </summary>
        [Description("直线2名称"), DefaultValue("")]
        public string InputLine2Name
        {
            get => this.inputLine2Name;
            set
            {
                this.inputLine2Name = value;
            }
        }


        StuLineData lineData2 = new StuLineData();
        /// <summary>
        /// 直线2
        /// </summary>
        [Description("直线2"), DefaultValue("")]
        public StuLineData LineData2
        {
            get => this.lineData2;
            set
            {
                this.lineData2 = value;
            }
        }


        HObject resultline1 = null;
        /// <summary>
        ///直线1
        /// </summary>
        [Description("直线1"), DefaultValue(null)]
        public HObject Resultline1
        {
            get => this.resultline1;
            set
            {
                this.resultline1 = value;
            }
        }
        HObject resultline2 = null;
        /// <summary>
        ///直线2
        /// </summary>
        [Description("直线2"), DefaultValue(null)]
        public HObject Resultline2
        {
            get => this.resultline2;
            set
            {
                this.resultline2 = value;
            }
        }


        bool usePixelRatio = false;
        /// <summary>
        /// 使用像素比
        /// </summary>
        [Description("使用像素比"), DefaultValue(false)]
        public bool UsePixelRatio
        {
            get => this.usePixelRatio;
            set
            {
                this.usePixelRatio = value;
            }
        }

        double pixelRatio = 1;
        /// <summary>
        /// 像素比
        /// </summary>
        [Description("像素比"), DefaultValue(1)]
        public double PixelRatio
        {
            get => this.pixelRatio;
            set
            {
                this.pixelRatio = value;
            }
        }

        double distance = 0;
        /// <summary>
        ///距离
        /// </summary>
        [Description("距离"), DefaultValue(0)]
        public double Distance
        {
            get => this.distance;
            set
            {
                this.distance = value;
            }
        }
        //Run status value define----1
        bool distanceLLRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool DistanceLLRunStatus
        {
            get => distanceLLRunStatus;
            set
            {
                distanceLLRunStatus = value;
            }
        }


    }
}
