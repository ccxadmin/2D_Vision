using HalconDotNet;
using ROIGenerateLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VisionShowLib.UserControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace PositionToolsLib.窗体.Pages
{
    [Serializable]
    public class TrajectoryTypeAnyCurveTool : TrajectoryTypeBaseTool
    {
        public TrajectoryTypeAnyCurveTool()
        {
            base.param = new TrajectoryTypeAnyCurveParam();
        }

        public override TemRunResult Run()
        {
            TemRunResult result = new TemRunResult();
            result.trajectoryDataPoints = new List<DgTrajectoryData>();
            result.resultContour = null;
            param.ResultInspectROI = null;
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            try
            {
                //输入图像
                if (!ObjectValided(param.InputImage))
                {
                    result.runFlag = false;
                    result.info = "输入图像无效";
                    return result;
                }
                //检测区域
                if (!ObjectValided(((TrajectoryTypeAnyCurveParam)param).InspectRegion))
                {
                    result.runFlag = false;
                    result.info = "输入检测区域无效";
                    return result;
                }
                HObject differenceRegion= (param as TrajectoryTypeAnyCurveParam).InspectRegion;

               //减去掩膜区域
                if (ObjectValided(((TrajectoryTypeAnyCurveParam)param).MaskRegion))
                {
                    HObject maskRegion = (param as TrajectoryTypeAnyCurveParam).MaskRegion;
                    HOperatorSet.Difference(differenceRegion, maskRegion,out differenceRegion);
                }

                HOperatorSet.ReduceDomain(param.InputImage, differenceRegion, out HObject ho_ImageReduced);
                param.ResultInspectROI = differenceRegion.Clone();
                ////轨迹提取

                HOperatorSet.EdgesSubPix(ho_ImageReduced, out HObject ho_Edges, "canny", 1,
                 (param as TrajectoryTypeAnyCurveParam).EdgeThdMin,
                 (param as TrajectoryTypeAnyCurveParam).EdgeThdMax);
                HOperatorSet.UnionAdjacentContoursXld(ho_Edges, out HObject ho_UnionContours,
                    100, 5, "attr_keep");

                //根据轮廓长度筛选轨迹
                HOperatorSet.SelectContoursXld(ho_UnionContours, out HObject ho_SelectedContours, "contour_length",
                    (param as TrajectoryTypeAnyCurveParam).XldLengthMin,
                   (param as TrajectoryTypeAnyCurveParam).XldLengthMax, -0.5, 0.5);

                if (!VisionShowTool.ObjectValided(ho_SelectedContours))
                {
                    result.runFlag = false;
                    result.info = "任意轨迹提取失败";
                    return result;

                }
                //是否闭合轮廓
                HObject ho_ClosedContours = null;
                if ((param as TrajectoryTypeAnyCurveParam).IsXldClosed)
                    HOperatorSet.CloseContoursXld(ho_SelectedContours, out ho_ClosedContours);
                else
                    HOperatorSet.CopyObj(ho_SelectedContours, out ho_ClosedContours, 1, -1);

          
                //轮廓平滑
                HOperatorSet.SmoothContoursXld(ho_ClosedContours, out HObject ho_SmoothedContours, 5);

                //点位离散

                HOperatorSet.CountObj(ho_SmoothedContours, out HTuple number);
                if (number.I<=0)
                {
                    result.runFlag = false;
                    result.info = "任意轨迹提取失败";
                    return result;

                }
                HTuple rows = new HTuple();
                HTuple columns = new HTuple();
                HOperatorSet.GenEmptyObj(out HObject emptyRegionBuf);
                HOperatorSet.ConcatObj(emptyRegionBuf,ho_SmoothedContours, out  emptyRegionBuf);

                for (int i=0;i< number.I;i++)
                {
                    HOperatorSet.SelectObj(ho_SmoothedContours, out HObject objectSelected, i+1);


                    EquidistancePoints(objectSelected, out HObject ho_newContour,
                                  (param as TrajectoryTypeAnyCurveParam).SamplingPointNums);

                    HOperatorSet.GetContourXld(ho_newContour, out HTuple hv_Row2, out HTuple hv_Col2);

                    hv_Row2 = hv_Row2.TupleSelectRange(0, (new HTuple(hv_Row2.TupleLength())) - 2);

                    hv_Col2 = hv_Col2.TupleSelectRange(0, (new HTuple(hv_Col2.TupleLength())) - 2);

                    HOperatorSet.GenCrossContourXld(out HObject ho_Cross, hv_Row2, hv_Col2, 10, 0);
                    HOperatorSet.ConcatObj(emptyRegionBuf, ho_Cross, out  emptyRegionBuf);
  
                    //点位排序
                    SortPoins(hv_Row2, hv_Col2, out HTuple sortRows, out HTuple sortCols, EumStartP.上);
                    rows= rows.TupleConcat(sortRows);
                    columns= columns.TupleConcat(sortCols);
                }
                
                result.resultContour = emptyRegionBuf.Clone();
                for(int j=0;j< rows.TupleLength();j++)
                   result.trajectoryDataPoints.
                        Add(new DgTrajectoryData(j+1,
                       columns.TupleSelect(j).D,
                       rows.TupleSelect(j).D));


                ho_ImageReduced.Dispose();
                differenceRegion.Dispose();
                ho_SmoothedContours.Dispose();        
                result.info = "任意轨迹检测完成";
                result.runFlag = true;
            }
            catch (Exception er)
            {
                result.info = "检测异常:" + er.Message;
                result.runFlag = false;
                return result;
            }
            return result;
        }


       

        /// <summary>
        /// 轮廓点位等距
        /// </summary>
        /// <param name="ho_originContour"></param>
        /// <param name="ho_newContour"></param>
        /// <param name="hv_cutNum"></param>
        private void EquidistancePoints(HObject ho_originContour, out HObject ho_newContour,
     HTuple hv_cutNum)
        {

            // Local iconic variables 

            // Local control variables 

            HTuple hv_Length = null, hv_Row = null, hv_Col = null;
            HTuple hv_newrow = null, hv_newcol = null, hv_rows = null;
            HTuple hv_cols = null, hv_length = null, hv_gap = null;
            HTuple hv_Index1 = null, hv_intdex = new HTuple(), hv_Int = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_newContour);
            HOperatorSet.LengthXld(ho_originContour, out hv_Length);
            HOperatorSet.GetContourXld(ho_originContour, out hv_Row, out hv_Col);

            hv_newrow = new HTuple();
            hv_newcol = new HTuple();
            hv_rows = hv_Row.TupleSelect(0);
            hv_cols = hv_Col.TupleSelect(0);

            hv_length = new HTuple(hv_Row.TupleLength());
            hv_gap = (hv_length * 1.0) / hv_cutNum;


            HTuple end_val12 = hv_cutNum - 1;
            HTuple step_val12 = 1;
            for (hv_Index1 = 1; hv_Index1.Continue(end_val12, step_val12); hv_Index1 = hv_Index1.TupleAdd(step_val12))
            {

                hv_intdex = 0 + (hv_Index1 * hv_gap);
                HOperatorSet.TupleRound(hv_intdex, out hv_Int);
                if (hv_Int.I >= hv_Row.TupleLength())
                {
                    hv_newrow = hv_Row.TupleSelect(0 + hv_Int - 1);
                    hv_newcol = hv_Col.TupleSelect(0 + hv_Int - 1);
                }
                else
                {
                    hv_newrow = hv_Row.TupleSelect(0 + hv_Int);
                    hv_newcol = hv_Col.TupleSelect(0 + hv_Int);

                }

                hv_rows = hv_rows.TupleConcat(hv_newrow);

                hv_cols = hv_cols.TupleConcat(hv_newcol);

            }

            ho_newContour.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_newContour, hv_rows.TupleConcat(hv_rows.TupleSelect(
                0)), hv_cols.TupleConcat(hv_cols.TupleSelect(0)));
            return;
        }


        /// <summary>
        /// 点位排序
        /// </summary>
        /// <param name="hv_T1"></param>
        /// <param name="hv_T2"></param>
        /// <param name="hv_SortMode"></param>
        /// <param name="hv_Sorted1"></param>
        /// <param name="hv_Sorted2"></param>
        void SortPairs(HTuple hv_T1, HTuple hv_T2, HTuple hv_SortMode, out HTuple hv_Sorted1,
    out HTuple hv_Sorted2)
        {
            // Local iconic variables 

            // Local control variables 

            HTuple hv_Indices1 = new HTuple(), hv_Indices2 = new HTuple();
            // Initialize local and output iconic variables 
            hv_Sorted1 = new HTuple();
            hv_Sorted2 = new HTuple();
            //Sort tuple pairs.
            //
            //input parameters:
            //T1: first tuple
            //T2: second tuple
            //SortMode: if set to '1', sort by the first tuple,
            //   if set to '2', sort by the second tuple
            //
            if ((int)((new HTuple(hv_SortMode.TupleEqual("1"))).TupleOr(new HTuple(hv_SortMode.TupleEqual(
                1)))) != 0)
            {
                HOperatorSet.TupleSortIndex(hv_T1, out hv_Indices1);
                hv_Sorted1 = hv_T1.TupleSelect(hv_Indices1);
                hv_Sorted2 = hv_T2.TupleSelect(hv_Indices1);
            }
            else if ((int)((new HTuple((new HTuple(hv_SortMode.TupleEqual("column"))).TupleOr(
                new HTuple(hv_SortMode.TupleEqual("2"))))).TupleOr(new HTuple(hv_SortMode.TupleEqual(
                2)))) != 0)
            {
                HOperatorSet.TupleSortIndex(hv_T2, out hv_Indices2);
                hv_Sorted1 = hv_T1.TupleSelect(hv_Indices2);
                hv_Sorted2 = hv_T2.TupleSelect(hv_Indices2);
            }

            return;
        }
        static public void sort_pairs(HTuple hv_T1, HTuple hv_T2, HTuple hv_SortMode, out HTuple hv_Sorted1,
     out HTuple hv_Sorted2)
        {
            // Local iconic variables 

            // Local control variables 

            HTuple hv_Indices1 = new HTuple(), hv_Indices2 = new HTuple();
            // Initialize local and output iconic variables 
            hv_Sorted1 = new HTuple();
            hv_Sorted2 = new HTuple();
            //Sort tuple pairs.
            //
            //input parameters:
            //T1: first tuple
            //T2: second tuple
            //SortMode: if set to '1', sort by the first tuple,
            //   if set to '2', sort by the second tuple
            //
            if ((int)((new HTuple(hv_SortMode.TupleEqual("1"))).TupleOr(new HTuple(hv_SortMode.TupleEqual(
                1)))) != 0)
            {
                HOperatorSet.TupleSortIndex(hv_T1, out hv_Indices1);
                hv_Sorted1 = hv_T1.TupleSelect(hv_Indices1);
                hv_Sorted2 = hv_T2.TupleSelect(hv_Indices1);
            }
            else if ((int)((new HTuple((new HTuple(hv_SortMode.TupleEqual("column"))).TupleOr(
                new HTuple(hv_SortMode.TupleEqual("2"))))).TupleOr(new HTuple(hv_SortMode.TupleEqual(
                2)))) != 0)
            {
                HOperatorSet.TupleSortIndex(hv_T2, out hv_Indices2);
                hv_Sorted1 = hv_T1.TupleSelect(hv_Indices2);
                hv_Sorted2 = hv_T2.TupleSelect(hv_Indices2);
            }

            return;
        }
        /// <summary>
        /// 点位排序
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="sortRows"></param>
        /// <param name="sortCols"></param>
        void SortPoins(HTuple rows, HTuple cols, out HTuple sortRows, out HTuple sortCols)
        {
            try
            {


                //////新增排序
                sort_pairs(rows, cols, "2", out HTuple hv_Sorted1, out HTuple hv_Sorted2);
                HOperatorSet.TupleFind(rows, hv_Sorted1.TupleSelect(0), out HTuple hv_Indices);
                HOperatorSet.TupleFind(cols, hv_Sorted2.TupleSelect(0), out HTuple hv_Indices1);
                HOperatorSet.TupleIntersection(hv_Indices, hv_Indices1, out HTuple hv_Intersection);
                HOperatorSet.TupleSelect(rows, HTuple.TupleGenSequence(0, hv_Intersection.TupleSelect(0) - 1, 1),
                    out HTuple hv_Selected);
                HOperatorSet.TupleSelect(rows, HTuple.TupleGenSequence(hv_Intersection.TupleSelect(0), (new HTuple(rows.TupleLength()
                    )) - 1, 1), out HTuple hv_Selected2);
                sortRows = new HTuple();
                sortRows = sortRows.TupleConcat(hv_Selected2);
                sortRows = sortRows.TupleConcat(hv_Selected);
                HOperatorSet.TupleSelect(cols, HTuple.TupleGenSequence(0, hv_Intersection.TupleSelect(0) - 1, 1),
                    out HTuple hv_Selected3);
                HOperatorSet.TupleSelect(cols, HTuple.TupleGenSequence(hv_Intersection.TupleSelect(0), (new HTuple(rows.TupleLength()
                    )) - 1, 1), out HTuple hv_Selected4);
                sortCols = new HTuple();
                sortCols = sortCols.TupleConcat(hv_Selected4);
                sortCols = sortCols.TupleConcat(hv_Selected3);
            }
            catch
            {
                sortRows = rows;
                sortCols = cols;
                //Appentxt("点位排序异常！");
            }
        }

        void SortPoins(HTuple rows, HTuple cols, out HTuple sortRows, out HTuple sortCols, EumStartP startP)
        {
            try
            {
                HTuple index = 0;
                switch (startP)
                {
                    case EumStartP.左:
                        HOperatorSet.TupleMin(cols, out HTuple min);
                        HOperatorSet.TupleFindFirst(cols, min, out index);
                        break;

                    case EumStartP.右:
                        HOperatorSet.TupleMax(cols, out HTuple max);
                        HOperatorSet.TupleFindFirst(cols, max, out index);
                        break;
                    case EumStartP.上:
                        HOperatorSet.TupleMin(rows, out HTuple min2);
                        HOperatorSet.TupleFindFirst(rows, min2, out index);
                        break;
                    case EumStartP.下:
                        HOperatorSet.TupleMax(rows, out HTuple max2);
                        HOperatorSet.TupleFindFirst(rows, max2, out index);
                        break;
                    default:
                        HOperatorSet.TupleMin(cols, out HTuple min3);
                        HOperatorSet.TupleFindFirst(cols, min3, out index);
                        break;
                }
                HOperatorSet.TupleSelect(rows, HTuple.TupleGenSequence(0, index - 1, 1),
                   out HTuple hv_Selected);
                HOperatorSet.TupleSelect(rows, HTuple.TupleGenSequence(index, (new HTuple(rows.TupleLength()
                    )) - 1, 1), out HTuple hv_Selected2);
                sortRows = new HTuple();
                sortRows = sortRows.TupleConcat(hv_Selected2);
                sortRows = sortRows.TupleConcat(hv_Selected);
                HOperatorSet.TupleSelect(cols, HTuple.TupleGenSequence(0, index - 1, 1),
                    out HTuple hv_Selected3);
                HOperatorSet.TupleSelect(cols, HTuple.TupleGenSequence(index, (new HTuple(rows.TupleLength()
                    )) - 1, 1), out HTuple hv_Selected4);
                sortCols = new HTuple();
                sortCols = sortCols.TupleConcat(hv_Selected4);
                sortCols = sortCols.TupleConcat(hv_Selected3);
            }

            catch
            {
                sortRows = rows;
                sortCols = cols;
                //Appentxt("点位排序异常！");
            }
        }
    }
}
