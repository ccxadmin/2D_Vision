using FilesRAW.Common;
using FunctionLib.Location;
using HalconDotNet;
using MainFormLib.Models;
using MainFormLib.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionShowLib.UserControls;

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
        static public bool CalRotateCenter(ref NinePointsCalibModel model, out string info)
        {
            info = "";
            if (model.DgRotatePointDataList.Count < 5)
            {
                info = "点位坐标数据不足5条，请确认!";
                return false;
            }
            
            HTuple hv_MulRotate_PixelRow = new HTuple();
            HTuple hv_MulRotate_PixelColumn = new HTuple();
      
            foreach (var s in model.DgRotatePointDataList)
            {
                hv_MulRotate_PixelRow.Append(s.Y);
                hv_MulRotate_PixelColumn.Append(s.X);
             
            }
            fitcircleData d_fitcircleData =
                AxisCoorditionRotation.MulPoints_GetRotateCenter(hv_MulRotate_PixelRow,
                hv_MulRotate_PixelColumn);//圆心为像素坐标

            HTuple temmachineX = new HTuple(); HTuple temmachineY = new HTuple();
           Transformation_POINT(d_fitcircleData.center_Column,
                d_fitcircleData.center_Row, model.Hv_HomMat2D, out temmachineX, out temmachineY);//然后转换成机器人坐标
            model.TxbRotateCenterX = temmachineX.D;
            model.TxbRotateCenterY = temmachineY.D;
            return true;
                
        }

        /// <summary>
        /// 九点标定矩阵生成
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        static public bool GenNineCaliMatrix(ref NinePointsCalibModel model, out string info)
        {
            info = "";
            if (model.DgPixelPointDataList.Count != 9 ||
                            model.DgRobotPointDataList.Count != 9)
            {
                info = "点位坐标数据不足9条，请确认!";
                return false;
            }
            HTuple hv_PixelPointx = new HTuple();
            HTuple hv_PixelPointy = new HTuple();
            foreach (var s in model.DgPixelPointDataList)
            {
                hv_PixelPointx.Append(s.X);
                hv_PixelPointy.Append(s.Y);

            }
            HTuple hv_MechinePointX = new HTuple();
            HTuple hv_MechinePointY = new HTuple();
            foreach (var s in model.DgRobotPointDataList)
            {
                hv_MechinePointX.Append(s.X);
                hv_MechinePointY.Append(s.Y);
            }

            GuidePositioning_HDevelopExport.Transformation_matrix(hv_PixelPointx, hv_PixelPointy,
                hv_MechinePointX, hv_MechinePointY, out  HTuple hv_HomMat2D, out HTuple hv_ParArray);
            model.Hv_HomMat2D = hv_HomMat2D;
            if (hv_ParArray != null)
            {
                model.TxbSx = hv_ParArray[0].D;
                model.TxbSy = hv_ParArray[1].D;
                model.TxbPhi = hv_ParArray[2].D;
                model.TxbTheta = hv_ParArray[3].D;
                model.TxbTx = hv_ParArray[4].D;
                model.TxbTy = hv_ParArray[5].D;

            }
            return true;
        }
        /// <summary>
        /// 保存九点标定的数据
        /// </summary>
        static public bool SaveNineCaliData(NinePointsCalibModel model,
             string rootFolder, string currCalibName = "default")
        {
            string filePath = rootFolder + "\\标定矩阵\\九点标定\\" + currCalibName;
            if (!Directory.Exists(rootFolder + "\\标定矩阵"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵");
            if (!Directory.Exists(rootFolder + "\\标定矩阵\\九点标定"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵\\九点标定");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            try
            {  //九点标定关系参数保存
                GeneralUse.WriteSerializationFile<ObservableCollection<DgPixelPointData>>(filePath + "\\PixelPoint", model.DgPixelPointDataList);
                GeneralUse.WriteSerializationFile<ObservableCollection<DgRobotPointData>>(filePath + "\\RobotPoint", model.DgRobotPointDataList);
                GeneralUse.WriteValue("九点标定", "X缩放", model.TxbSx.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "Y缩放", model.TxbSy.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "旋转弧", model.TxbPhi.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "倾斜弧", model.TxbTheta.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "X偏移量", model.TxbTx.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "Y偏移量", model.TxbTy.ToString(), "config", filePath);
                if (model.Hv_HomMat2D != null &&
                        model.Hv_HomMat2D.Length > 0)
                    HOperatorSet.WriteTuple(model.Hv_HomMat2D, filePath + "\\hv_HomMat2D.tup");

                return true;

            }
            catch (Exception er)
            {
                return false;
            }
        }
        /// <summary>
        /// 保存旋转相关数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="rootFolder"></param>
        /// <param name="currCalibName"></param>
        /// <returns></returns>
        static public bool SaveRatateData(NinePointsCalibModel model,
            string rootFolder,string currCalibName="default")
        {
            string filePath = rootFolder + "\\标定矩阵\\九点标定\\" + currCalibName;
            if (!Directory.Exists(rootFolder + "\\标定矩阵"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵");
            if (!Directory.Exists(rootFolder + "\\标定矩阵\\九点标定"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵\\九点标定");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            try
            {

                GeneralUse.WriteValue("九点标定", "旋转中心X", model.TxbRotateCenterX.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "旋转中心Y", model.TxbRotateCenterY.ToString(), "config", filePath);
                GeneralUse.WriteSerializationFile<ObservableCollection<DgRotatePointData>>(filePath + "\\RotatePoint", model.DgRotatePointDataList);
                return true;
            }
            catch (Exception er)
            {
                return false;
            }
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
