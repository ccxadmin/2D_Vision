using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunctionLib.Location;
using HalconDotNet;

namespace FunctionLib
{
    /// <summary>
    /// 变形检测
    /// </summary>
   public class VariantCheckTask
    {
        public static ResultOfVariantCheck Action(HObject srcImg, lineOrcirclefitParma variantCheckParam,
           string direction = "无", HObject SearchRegion=null, EumRegionType useRegionType= EumRegionType.直线)
        {
            ResultOfVariantCheck result = new ResultOfVariantCheck();
            if(useRegionType== EumRegionType.直线)
            {
                result.LineOrCircleFitResult = new OutputLineFit();
                //直线1
                HObject ho_Region = null;
                HOperatorSet.GenEmptyObj(out ho_Region);
                ho_Region.Dispose();
                //获取卡尺区域即检测区域
                HDevelopExport_calibr.get_rake_region(srcImg, out ho_Region,
                   variantCheckParam.CaliperNum, variantCheckParam.CaliperHeight, variantCheckParam.CaliperWidth,
                   EumTestModule.fit, SearchRegion);

                (result.LineOrCircleFitResult as OutputLineFit).ResultRegion = ho_Region;

                OutputLineFit linetemdata = HDevelopExport_calibr.createOrfitLine1(srcImg,
                      variantCheckParam, -1, EumTestModule.fit, SearchRegion);


                if (linetemdata == null || !GuidePositioning_HDevelopExport.ObjectValided(linetemdata.resultobj))

                {
                    result.errInfo = "直线拟合失败！";
                    result.runFlag = false;
                }
                else
                {
                    result.runFlag = true;
                    result.LineOrCircleFitResult = linetemdata;

                }
            }
            //圆弧
            else
            {
                result.LineOrCircleFitResult = new OutputCircleFit();
                //直线1
                HObject ho_Region = null;
                HOperatorSet.GenEmptyObj(out ho_Region);
                ho_Region.Dispose();
                //获取卡尺区域即检测区域
               
                HDevelopExport_calibr.get_spoke_region(srcImg, out ho_Region,
                   variantCheckParam.CaliperNum, variantCheckParam.CaliperHeight, variantCheckParam.CaliperWidth,
                  direction, EumTestModule.fit, SearchRegion);

                (result.LineOrCircleFitResult as OutputCircleFit).ResultRegion = ho_Region;
                OutputCircleFit circletemdata = new OutputCircleFit();
                HDevelopExport_calibr.DrawCirclefitROI(srcImg,
                      variantCheckParam, -1,ref  circletemdata, EumTestModule.fit, SearchRegion,
                      (EumCircleDirection)Enum.Parse(typeof(EumCircleDirection), direction));


                HDevelopExport_calibr.fitcircle(ref circletemdata);
                if (circletemdata == null || !GuidePositioning_HDevelopExport.ObjectValided(circletemdata.resultobj))
                {
                    result.errInfo = "圆弧线拟合失败！";
                    result.runFlag = false;
                }
                else
                {
                    result.runFlag = true;
                    result.LineOrCircleFitResult = circletemdata;

                }
              
            }
        
            return result;
        }

    }
    /// <summary>
    /// 区域类型
    /// </summary>
    public enum EumRegionType
    {
        直线,
        圆弧
    }
    public struct ResultOfVariantCheck
    {
        public bool runFlag;     
     //   public OutputLineFit LineFitResult;
        public object LineOrCircleFitResult;
        public string errInfo;
       
    }
}
