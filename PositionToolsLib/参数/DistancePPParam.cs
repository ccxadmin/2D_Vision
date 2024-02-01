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
    /// 点点距离检测参数
    /// </summary>
    [Serializable]
    public class DistancePPParam : BaseParam
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

        string startXName = "";
        /// <summary>
        /// 起点行坐标名称
        /// </summary>
        [Description("起点X坐标名称"), DefaultValue("")]
        public string StartXName
        {
            get => this.startXName;
            set
            {
                this.startXName = value;
            }
        }

        double x1 = 0;
        /// <summary>
        /// 起点X坐标
        /// </summary>
        [Description("起点行坐标"), DefaultValue("0")]
        public double X1
        {
            get => this.x1;
            set
            {
                this.x1 = value;
            }
        }


        string startYName = "";
        /// <summary>
        /// 起点Y坐标名称
        /// </summary>
        [Description("起点Y坐标名称"), DefaultValue("")]
        public string StartYName
        {
            get => this.startYName;
            set
            {
                this.startYName = value;
            }
        }

        double y1 = 0;
        /// <summary>
        /// 起点Y坐标
        /// </summary>
        [Description("起点Y坐标"), DefaultValue("0")]
        public double Y1
        {
            get => this.y1;
            set
            {
                this.y1 = value;
            }
        }

        string endXName = "";
        /// <summary>
        /// 终点X坐标名称
        /// </summary>
        [Description("终点X坐标名称"), DefaultValue("")]
        public string EndXName
        {
            get => this.endXName;
            set
            {
                this.endXName = value;
            }
        }

        double x2 = 0;
        /// <summary>
        /// 终点X坐标
        /// </summary>
        [Description("终点X坐标"), DefaultValue("0")]
        public double X2
        {
            get => this.x2;
            set
            {
                this.x2 = value;
            }
        }


        string endYName = "";
        /// <summary>
        /// 终点Y标名称
        /// </summary>
        [Description("终点Y坐标名称"), DefaultValue("")]
        public string EndYName
        {
            get => this.endYName;
            set
            {
                this.endYName = value;
            }
        }

        double y2 = 0;
        /// <summary>
        /// 终点Y坐标
        /// </summary>
        [Description("终点Y坐标"), DefaultValue("0")]
        public double Y2
        {
            get => this.y2;
            set
            {
                this.y2 = value;
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
        bool distancePPRunStatus = false;
        /// <summary>
        /// 工具运行结果状态
        /// </summary>
        [Description("工具运行结果状态"), DefaultValue(false)]
        public bool DistancePPRunStatus
        {
            get => distancePPRunStatus;
            set
            {
                distancePPRunStatus = value;
            }
        }

    }
}
