using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Pages
{
    /// <summary>
    /// 检测基础参数
    /// </summary>
    [Serializable]
    public class TrajectoryTypeBaseParam
    {
        [NonSerialized]
        public HObject InputImage;
        //public HObject DetectROI { get; set; }
        [NonSerialized]
        public HObject ResultInspectROI;
}
    /// <summary>
    /// 运行结果
    /// </summary>
    public struct TemRunResult
    {
        public bool runFlag;
        public string info;
        public HObject resultContour;
        public List<DgTrajectoryData> trajectoryDataPoints;

    }
    /// <summary>
    /// 轨迹点
    /// </summary>
    [Serializable]
    public class DgTrajectoryData
    {
        public DgTrajectoryData()
        {

        }
        /// <summary>
        /// 直线，矩形，旋转矩形，任意曲线轨迹描述
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public DgTrajectoryData(int id, double x, double y)
        {
            ID = id;
            X = Math.Round(x, 3);
            Y = Math.Round(y, 3);
            Radius = double.NaN;
        }

        /// <summary>
        /// 圆轨迹描述
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        public DgTrajectoryData(int id, double x, double y, double radius)
        {
            ID = id;
            X = Math.Round(x, 3);
            Y = Math.Round(y, 3);
            Radius = Math.Round(radius, 3);
        }

        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }


    }
}
