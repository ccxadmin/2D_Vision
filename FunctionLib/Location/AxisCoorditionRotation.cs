using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;

namespace FunctionLib.Location
{
   public  class AxisCoorditionRotation
    {
      /// <summary>
      /// 旋转中心计算
      /// </summary>
        /// <param name="point1">旋转前Mark坐标</param>
        /// <param name="point2">旋转后Mark坐标</param>
        /// <param name="RarotionAngle">旋转角度</param>
      /// <returns></returns>
       public static POINTD getRotateCenter(POINTD point1, POINTD point2, double RarotionAngle)
        {
            double sita = RarotionAngle * Math.PI / 180;
            double temx = ((point1.X + point2.X) - (Math.Sin(sita) / (1 - Math.Cos(sita)) * (point2.Y - point1.Y))) / 2;
            double temy = ((point1.Y + point2.Y) + (Math.Sin(sita) / (1 - Math.Cos(sita)) * (point2.X - point1.X))) / 2;
            return new POINTD(temx,temy);
        }

      /// <summary>
       /// 旋转前坐标点位计算
      /// </summary>
       /// <param name="point1">旋转后Mark坐标</param>
       /// <param name="pointC">旋转中心</param>
       /// <param name="RarotionAngle">旋转角度</param>
      /// <returns></returns>
       public static POINTD get_befor_RotatedPoint(POINTD point2, POINTD pointC, double RarotionAngle)
       {
           double sita = RarotionAngle * Math.PI / 180;
           double temx = (point2.X - pointC.X) * Math.Cos(sita) + (point2.Y - pointC.Y) * Math.Sin(sita) + pointC.X;
           double temy = (point2.Y - pointC.Y) * Math.Cos(sita) - (point2.X - pointC.X) * Math.Sin(sita) + pointC.Y;
           return new POINTD(temx, temy);
       }

        /// <summary>
        /// 旋转后坐标点位计算
        /// </summary>
        /// <param name="point1">旋转前Mark坐标</param>
        /// <param name="pointC">旋转中心</param>
        /// <param name="RarotionRad">旋转角度</param>
        /// <returns></returns>
        public static POINTD get_after_RotatePoint(POINTD point1, POINTD pointC, double RarotionAngle)
       {
           double sita = RarotionAngle * Math.PI / 180;
           double temx = (point1.X - pointC.X) * Math.Cos(sita) - (point1.Y - pointC.Y) * Math.Sin(sita) + pointC.X;
           double temy = (point1.Y - pointC.Y) * Math.Cos(sita) + (point1.X - pointC.X) * Math.Sin(sita) + pointC.Y;
           return new POINTD(temx, temy);
       }
      
        /// <summary>
        /// 多点拟合圆
        /// </summary>
        static public fitcircleData MulPoints_GetRotateCenter(HTuple hv_Rows, HTuple hv_Columns)
        {

            fitcircleData  d_fitcircleData = new fitcircleData();
            // Local iconic variables 

            HObject ho_Contour, ho_ContCircle;

            HTuple hv_Row = new HTuple(); HTuple hv_Column = new HTuple();
            HTuple hv_Radius = new HTuple(); HTuple hv_StartPhi = new HTuple();
            HTuple hv_EndPhi = new HTuple(); HTuple hv_PointOrder = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
         
            ho_Contour.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Columns);



            HOperatorSet.FitCircleContourXld(ho_Contour, "geohuber", -1, 0, 0, 3, 2, out hv_Row,
                out hv_Column, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
            ho_ContCircle.Dispose();
            HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_Row, hv_Column, hv_Radius,
                0, (new HTuple(360)).TupleRad(), "positive", 1);
            d_fitcircleData.center_Row = hv_Row;
            d_fitcircleData.center_Column = hv_Column;
            d_fitcircleData.center_Radium = hv_Radius;
            d_fitcircleData.circleContour.Dispose();
            HOperatorSet.CopyObj(ho_ContCircle, out d_fitcircleData.circleContour,1,1);

            ho_Contour.Dispose();
            ho_ContCircle.Dispose();
            return d_fitcircleData;
        }
    }
    public class fitcircleData
    {
        public fitcircleData()
        {
            center_Row = new HTuple();
            center_Column = new HTuple();
            center_Radium = new HTuple();
            circleContour = new HObject();
        }
        public HTuple center_Row;
        public HTuple center_Column;
        public HTuple center_Radium;
        public HObject circleContour;
    }
   [Serializable]
   public class POINTD
   {
        public POINTD()
        { }
       public POINTD(double x, double y)
       {
           X = x;
           Y = y;
       }
       public double X;
       public double Y;
       public double Angle;

   }
}
