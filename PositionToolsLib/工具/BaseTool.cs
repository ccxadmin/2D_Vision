using PositionToolsLib.参数;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSLog;
using HalconDotNet;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace PositionToolsLib.工具
{
    /// <summary>
    /// 定位检测工具基类
    /// </summary>
    [Serializable]
    abstract  public class BaseTool
    {
        protected BaseParam toolParam = null;//工具运行检测参数

        protected string toolName = string.Empty;//工具名称
        public string remark = "";//备注
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            //if (toolParam.Hv_HomMat2D == null)
            //{
            //    toolParam.Hv_HomMat2D = new HTuple();
            //}

        }
        public delegate DataManage GetManageHandle();
        [NonSerialized]
        public GetManageHandle OnGetManageHandle;
        
        public DataManage GetManage()
        {
            if (OnGetManageHandle == null)
                return new DataManage();
            else
                return OnGetManageHandle.Invoke();
        }
        /// <summary>
        /// 设置标定关系矩阵,外部切换标定关系
        /// </summary>
        /// <param name="homMat2D"></param>
        virtual  public void SetMatrix(HTuple homMat2D)
        {
            toolParam.hv_HomMat2D = homMat2D;
        }
        /// <summary>
        ///设置标定关系矩阵存放路径
        /// </summary>
        /// <param name="calibFilePath"></param>
        virtual public void SetCalibFilePath(string calibFilePath)
        {
            toolParam.calibFilePath = calibFilePath;
        }
        /// <summary>
        /// 计算直线角度 -180~180
        /// </summary>
        /// <param name="Rx">直线起点坐标x</param>
        /// <param name="Ry">直线起点坐标y</param>
        /// <param name="Rx2">直线终点坐标x</param>
        /// <param name="Ry2">直线终点坐标y</param>
        public  double calAngleOfLx(double Rx, double Ry, double Rx2, double Ry2)
        {
            if ((Rx2 == Rx) && (Ry2 > Ry))
                return 90;
            else if ((Rx2 == Rx) && (Ry2 < Ry))
                return -90;
            else if ((Rx2 == Rx) && (Ry2 == Ry))
                return -999;
            else
            {
                double temangle = Math.Atan((Ry2 - Ry) / (Rx2 - Rx)) * 180 / Math.PI;
                if (Ry2 - Ry > 0 && Rx2 - Rx > 0)  //第一象限
                    temangle = Math.Abs(temangle);
                else if (Ry2 - Ry > 0 && Rx2 - Rx < 0)//第二象限
                    temangle += 180;
                else if (Ry2 - Ry < 0 && Rx2 - Rx < 0)//第三象限
                    temangle -= 180;
                else if (Ry2 - Ry < 0 && Rx2 - Rx > 0)//第四象限
                    temangle = -Math.Abs(temangle);
                return temangle;
            }
        }
        /// <summary>
        /// 获取工具运行检测参数
        /// </summary>
        /// <returns></returns>
        public BaseParam GetParam()
        {          
            return toolParam;
        }
        /// <summary>
        ///  设置工具运行检测参数
        /// </summary>
        /// <param name="param"></param>
        public void  SetParam(BaseParam param)
        {
            toolParam = param;
        }
        /// <summary>
        /// 获取工具名称
        /// </summary>
        /// <returns></returns>
        public string GetToolName()
        {
            return toolName;
        }
        /// <summary>
        /// 检测工具运行
        /// </summary>
        /// <returns></returns>
        abstract public RunResult Run();

        /// <summary>
        /// 像素坐标转机械坐标
        /// </summary>
        /// <param name="fc"></param>
        /// <param name="Px">像素坐标X</param>
        /// <param name="Py">像素坐标Y</param>
        /// <param name="Rx">机械坐标X</param>
        /// <param name="Ry">机械坐标Y</param>
        public  bool Transformation_POINT( HTuple Px, HTuple Py,
                       out HTuple Rx, out HTuple Ry)
        {
           
            Rx = Ry = 0;
            if (toolParam == null) return false;
            if (toolParam.hv_HomMat2D != null && toolParam.hv_HomMat2D.Length > 0)
                HOperatorSet.AffineTransPoint2d(toolParam.hv_HomMat2D, Px, Py,
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
        /// <param name="Px"></param>
        /// <param name="Py"></param>
        /// <returns></returns>
        public bool Transformation_POINT_INV(HTuple Rx, HTuple Ry,
                      out HTuple Px, out HTuple Py)
        {

            Px = Py = 0;
            if (toolParam == null) return false;
            if (toolParam.hv_HomMat2D != null && toolParam.hv_HomMat2D.Length > 0)
            {
                HOperatorSet.HomMat2dInvert(toolParam.hv_HomMat2D, out HTuple homMat2DInvert);
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
        /// 获取像素比
        /// </summary>
        /// <returns></returns>
        public double GetPixelRatio()
        {
            if (!this.Transformation_POINT(1000, 1000, out HTuple rx, out HTuple ry)) return 1.0;
            if (!this.Transformation_POINT(1000, 1001, out HTuple rx2, out HTuple ry2)) return 1.0;
            return Math.Sqrt(Math.Pow(rx2 - rx, 2) + Math.Pow(ry2 - ry, 2));
        }
        /// 判断图像或区域是否存在
        /// </summary>
        /// <param name = "obj" > 区域 </ param >
        /// < returns ></ returns >
        static   public  bool ObjectValided(HObject obj)
        {
            try
            {
                if (obj == null)
                    return false;
                if (!obj.IsInitialized())
                {
                    return false;
                }
                if (obj.CountObj() < 1)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        //计时器
        [NonSerialized]
        public Stopwatch sw = new Stopwatch();
    }

    /// <summary>
    /// 运行结果基类
    /// </summary>
    public class RunResult
    {
        /// <summary>
        /// 运行时间
        /// </summary>
        public long runTime ;
        /// <summary>
        /// 运行状态
        /// </summary>
        public bool runFlag;
  
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errInfo;
    
    }


}
