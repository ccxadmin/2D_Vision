using FunctionLib.Location;
using HalconDotNet;
using MainFormLib.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainFormLib
{
    /// <summary>
    /// 九点标定工具
    /// </summary>
    static public class NinePointsCalibTool
    {
        /// <summary>
        /// 生成标定矩阵(平移，缩放，旋转)
        /// </summary>
        /// <param name="hv_PixelPointsX"></param>
        /// <param name="hv_PixelPointsY"></param>
        /// <param name="hv_MechinePointsX"></param>
        /// <param name="hv_MechinePointsY"></param>
        /// <param name="hv_ParArray"></param>
        /// <returns></returns>
        static public HTuple Transformation_matrix(HTuple hv_PixelPointsX, HTuple hv_PixelPointsY,
            HTuple hv_MechinePointsX, HTuple hv_MechinePointsY, out HTuple hv_ParArray)
        {
            GuidePositioning_HDevelopExport.Transformation_matrix(
                hv_PixelPointsX, hv_PixelPointsY,
                  hv_MechinePointsX, hv_MechinePointsY,
                  out HTuple hv_HomMat2D, out hv_ParArray);

            return hv_HomMat2D;
        }

        /// <summary>
        /// 像素坐标转机械坐标
        /// </summary>
        /// <param name="Px"></param>
        /// <param name="Py"></param>
        /// <param name="hv_HomMat2D"></param>
        /// <param name="Rx"></param>
        /// <param name="Ry"></param>
        /// <returns></returns>
        public static bool Transformation_POINT( HTuple Px, HTuple Py,HTuple hv_HomMat2D,
                       out HTuple Rx, out HTuple Ry)
        {
            Rx = Ry = 0;
            if (hv_HomMat2D != null && hv_HomMat2D.Length > 0)
                HOperatorSet.AffineTransPoint2d(hv_HomMat2D, Px, Py,
                   out Rx, out Ry);
            else
            {               
                return false;
            }
            return true;
        }

        /// <summary>
        /// 机械坐标转像素坐标
        /// </summary>
        /// <param name="Rx"></param>
        /// <param name="Ry"></param>
        /// <param name="hv_HomMat2D"></param>
        /// <param name="Px"></param>
        /// <param name="Py"></param>
        public static bool Transformation_POINT_INV( HTuple Rx, HTuple Ry, HTuple hv_HomMat2D,
                      out HTuple Px, out HTuple Py)
        {
            HTuple homMat2DInvert = null;
            Px = Py = 0;
            if (hv_HomMat2D != null && hv_HomMat2D.Length > 0)
            {
                HOperatorSet.HomMat2dInvert(hv_HomMat2D, out homMat2DInvert);
                HOperatorSet.AffineTransPoint2d(homMat2DInvert, Rx, Ry,
                 out Px, out Py);
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 计算旋转中心(像素中心转物理中心)
        /// </summary>
        static public void CalRotateCenter(HTuple hv_MulPixelsX,
            HTuple hv_MulPixelsY, HTuple hv_HomMat2D,out double Cx,out double Cy)
        {
            Cx = 0;Cy = 0;
            fitcircleData d_fitcircleData = AxisCoorditionRotation.MulPoints_GetRorateCenter(
                 hv_MulPixelsY, hv_MulPixelsX);//圆心为像素坐标
                                                           //
            Transformation_POINT(d_fitcircleData.center_Column,
                d_fitcircleData.center_Row, hv_HomMat2D,
                out HTuple temmachineX, out HTuple temmachineY);//然后转换成机器人坐标
            Cx = temmachineX.D;
            Cy = temmachineY.D;
        }
    }

    /// <summary>
    /// 九点标定像素坐标数据集合
    /// </summary>
    [Serializable]
    public class PixelPointData
    {
        public PixelPointData()
        {
            ListPoint = new List<PointF>(capacity);
        }
        ~PixelPointData()
        {
            ListPoint.Clear();
        }
        const int capacity = 9;
        public List<PointF> ListPoint { get; set; }

    }
    /// <summary>
    /// 九点标定机械坐标数据集合
    /// </summary>
    [Serializable]
    public class RobotPointData
    {
        public RobotPointData()
        {
            ListPoint = new List<PointF>(capacity);
        }
        ~RobotPointData()
        {
            ListPoint.Clear();
        }
        const int capacity = 9;
        public List<PointF> ListPoint { get; set; }
    }
    /// <summary>
    /// 旋转中心计算数据集合
    /// </summary>
    [Serializable]
    public class RotatePointData
    {
        public RotatePointData()
        {
            ListPoint = new List<PointF>(capacity);
        }
        ~RotatePointData()
        {
            ListPoint.Clear();
        }
        const int capacity = 6;
        public List<PointF> ListPoint { get; set; }

    }
    /// <summary>
    /// 坐标系转换数据
    /// </summary>
    [Serializable]
    public class ConverCoorditionData
    {
        public ConverCoorditionData()
        {
            Sx = new HTuple(0);
            Sy = new HTuple(0);
            Phi = new HTuple(0);
            Theta = new HTuple(0);
            Tx = new HTuple(0);
            Ty = new HTuple(0);
        }
        //X缩放
        public HTuple Sx { get; set; }
        //Y缩放
        public HTuple Sy { get; set; }
        //旋转角(弧度)
        public HTuple Phi { get; set; }
        //倾斜角(弧度)
        public HTuple Theta { get; set; }
        //X偏移量
        public HTuple Tx { get; set; }
        //Y偏移量
        public HTuple Ty { get; set; }

    }
}
