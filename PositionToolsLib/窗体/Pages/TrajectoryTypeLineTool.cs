﻿using HalconDotNet;
using PositionToolsLib.参数;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Pages
{
    [Serializable]
    public class TrajectoryTypeLineTool : TrajectoryTypeBaseTool
    {
        public TrajectoryTypeLineTool()
        {
            base.param = new TrajectoryTypeLineParam();
        }

        public override TemRunResult Run()
        {
            TemRunResult result = new TemRunResult();
            result.trajectoryDataPoints=new List<DgTrajectoryData>();
            result.resultContour = null;
            param.ResultInspectROI = null;
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            try
            {
                if(!ObjectValided(param.InputImage))
                {
                    result.runFlag = false;
                    result.info = "输入图像无效";
                    return result;
                }
                if (!ObjectValided(((TrajectoryTypeLineParam)param ).InspectXLD))
                {
                    result.runFlag = false;
                    result.info = "输入检测区域无效";
                    return result;
                }
                HObject xld = (param as TrajectoryTypeLineParam).InspectXLD;
                HOperatorSet.GetContourXld(xld, out HTuple hv_Row, out HTuple hv_Column);
                HTuple hv_Row1 = hv_Row.TupleSelect(0);
                HTuple hv_Row2 = hv_Row.TupleSelect(1);
                HTuple hv_Column1 = hv_Column.TupleSelect(0);
                HTuple hv_Column2 = hv_Column.TupleSelect(1);

                get_rake_region(param.InputImage, out HObject ho_Regions,
                   (param as TrajectoryTypeLineParam).CaliperNum, (param as TrajectoryTypeLineParam).CaliperHeight,
                    (param as TrajectoryTypeLineParam).CaliperWidth, hv_Row1, hv_Column1, hv_Row2, hv_Column2);
                param.ResultInspectROI = ho_Regions.Clone();
                //卡尺
                rake(param.InputImage, out HObject ho_Regions2,
                    (param as TrajectoryTypeLineParam).CaliperNum, (param as TrajectoryTypeLineParam).CaliperHeight,
                    (param as TrajectoryTypeLineParam).CaliperWidth, 1.0,
                    (param as TrajectoryTypeLineParam).EdgeContrast,
                    Enum.GetName(typeof(EumTransition), (param as TrajectoryTypeLineParam).Transition),
                    Enum.GetName(typeof(EumSelect), (param as TrajectoryTypeLineParam).Select),
                    hv_Row1, hv_Column1,
                    hv_Row2, hv_Column2,
                    out HTuple hv_ResultRow, out HTuple hv_ResultColumn);
                //至少两点或以上才可拟合直线
                if (hv_ResultRow.TupleLength() <= 1)
                {
                 
                    result.runFlag = false;
                    result.info = "直线查找失败";
                    return result;
                }

                //结果直线
                pts_to_best_line(out HObject ho_Line, hv_ResultRow, hv_ResultColumn, 2, out HTuple hv_Row11,
                    out HTuple hv_Column11, out HTuple hv_Row21, out HTuple hv_Column21);

                if (hv_Row11.TupleLength() <= 0)
                {              
                    result.runFlag = false;
                    result.info = "直线查找失败";
                    return result;
                }

                HOperatorSet.GenCrossContourXld(out HObject cross, hv_Row11.D, hv_Column11.D, 50, 0);
                HOperatorSet.ConcatObj(ho_Line, cross, out HObject emptyRegionBuf);
                HOperatorSet.GenCrossContourXld(out HObject cross2, hv_Row21.D, hv_Column21.D, 50, 0);
                HOperatorSet.ConcatObj(emptyRegionBuf, cross2, out emptyRegionBuf);

                result.resultContour = emptyRegionBuf.Clone(); 
                result.trajectoryDataPoints.Add(new DgTrajectoryData(1, hv_Column11.D, hv_Row11.D));
                result.trajectoryDataPoints.Add(new DgTrajectoryData(2, hv_Column21.D, hv_Row21.D));             
                result.info = "直线轨迹检测完成";
                result.runFlag = true;             
                ho_Regions.Dispose();
                ho_Regions2.Dispose();
                ho_Line.Dispose();
                emptyRegionBuf.Dispose();
            }
            catch (Exception er)
            {               
                result.info = "检测异常:"+er.Message;
                result.runFlag = false;            
                return result;
            }         
            return result;
        }
        /*---------------------------------外部方法导入---------------------------*/
        #region ----External procedures----
        // Procedures 
        // External procedures 
        // Chapter: XLD / Creation
        // Short Description: Creates an arrow shaped XLD contour. 
        public void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
            HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_TempArrow = null;

            // Local control variables 

            HTuple hv_Length = null, hv_ZeroLengthIndices = null;
            HTuple hv_DR = null, hv_DC = null, hv_HalfHeadWidth = null;
            HTuple hv_RowP1 = null, hv_ColP1 = null, hv_RowP2 = null;
            HTuple hv_ColP2 = null, hv_Index = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_TempArrow);
            //This procedure generates arrow shaped XLD contours,
            //pointing from (Row1, Column1) to (Row2, Column2).
            //If starting and end point are identical, a contour consisting
            //of a single point is returned.
            //
            //input parameteres:
            //Row1, Column1: Coordinates of the arrows' starting points
            //Row2, Column2: Coordinates of the arrows' end points
            //HeadLength, HeadWidth: Size of the arrow heads in pixels
            //
            //output parameter:
            //Arrow: The resulting XLD contour
            //
            //The input tuples Row1, Column1, Row2, and Column2 have to be of
            //the same length.
            //HeadLength and HeadWidth either have to be of the same length as
            //Row1, Column1, Row2, and Column2 or have to be a single element.
            //If one of the above restrictions is violated, an error will occur.
            //
            //
            //Init
            ho_Arrow.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            //
            //Calculate the arrow length
            HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
            //
            //Mark arrows with identical start and end point
            //(set Length to -1 to avoid division-by-zero exception)
            hv_ZeroLengthIndices = hv_Length.TupleFind(0);
            if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
            {
                if (hv_Length == null)
                    hv_Length = new HTuple();
                hv_Length[hv_ZeroLengthIndices] = -1;
            }
            //
            //Calculate auxiliary variables.
            hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
            hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
            hv_HalfHeadWidth = hv_HeadWidth / 2.0;
            //
            //Calculate end points of the arrow head.
            hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
            hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
            hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
            hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
            //
            //Finally create output XLD contour for each input point pair
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                {
                    //Create_ single points for arrows with identical start and end point
                    ho_TempArrow.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(hv_Index),
                        hv_Column1.TupleSelect(hv_Index));
                }
                else
                {
                    //Create arrow contour
                    ho_TempArrow.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                        hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                        hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                        hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                        ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                        hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                        hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                        hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out ExpTmpOutVar_0);
                    ho_Arrow.Dispose();
                    ho_Arrow = ExpTmpOutVar_0;
                }
            }
            ho_TempArrow.Dispose();

            return;
        }

        public void pts_to_best_line(out HObject ho_Line, HTuple hv_Rows, HTuple hv_Cols,
            HTuple hv_ActiveNum, out HTuple hv_Row1, out HTuple hv_Column1, out HTuple hv_Row2,
            out HTuple hv_Column2)
        {



            // Local iconic variables 

            HObject ho_Contour = null;

            // Local control variables 

            HTuple hv_Length = null, hv_Nr = new HTuple();
            HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple(), hv_Length1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Line);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            //初始化
            hv_Row1 = 0;
            hv_Column1 = 0;
            hv_Row2 = 0;
            hv_Column2 = 0;
            //产生一个空的直线对象，用于保存拟合后的直线
            ho_Line.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Line);
            //计算边缘数量
            HOperatorSet.TupleLength(hv_Cols, out hv_Length);
            //当边缘数量不小于有效点数时进行拟合
            if ((int)((new HTuple(hv_Length.TupleGreaterEqual(hv_ActiveNum))).TupleAnd(new HTuple(hv_ActiveNum.TupleGreater(
                1)))) != 0)
            {
                //halcon的拟合是基于xld的，需要把边缘连接成xld
                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Cols);
                //拟合直线。使用的算法是'tukey'，其他算法请参考fit_line_contour_xld的描述部分。
                HOperatorSet.FitLineContourXld(ho_Contour, "tukey", -1, 0, 5, 2, out hv_Row1,
                    out hv_Column1, out hv_Row2, out hv_Column2, out hv_Nr, out hv_Nc, out hv_Dist);
                //判断拟合结果是否有效：如果拟合成功，数组中元素的数量大于0
                HOperatorSet.TupleLength(hv_Dist, out hv_Length1);
                if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
                {
                    ho_Contour.Dispose();

                    return;
                }
                //根据拟合结果，产生直线xld
                ho_Line.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Line, hv_Row1.TupleConcat(hv_Row2),
                    hv_Column1.TupleConcat(hv_Column2));
            }

            ho_Contour.Dispose();

            return;
        }

        public void rake(HObject ho_Image, out HObject ho_Regions, HTuple hv_Elements,
            HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_Sigma, HTuple hv_Threshold,
            HTuple hv_Transition, HTuple hv_Select, HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2,
            HTuple hv_Column2, out HTuple hv_ResultRow, out HTuple hv_ResultColumn)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionLines, ho_Cross = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_ATan = null;
            HTuple hv_i = null, hv_RowC = new HTuple(), hv_ColC = new HTuple();
            HTuple hv_Distance = new HTuple(), hv_MsrHandle_Measure = new HTuple();
            HTuple hv_RowEdge = new HTuple(), hv_ColEdge = new HTuple();
            HTuple hv_Amplitude = new HTuple(), hv_tRow = new HTuple();
            HTuple hv_tCol = new HTuple(), hv_t = new HTuple(), hv_Number = new HTuple();
            HTuple hv_j = new HTuple();
            HTuple hv_DetectWidth_COPY_INP_TMP = hv_DetectWidth.Clone();
            HTuple hv_Select_COPY_INP_TMP = hv_Select.Clone();
            HTuple hv_Transition_COPY_INP_TMP = hv_Transition.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            //获取图像尺寸
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            //产生一个空显示对象，用于显示
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);
            //初始化边缘坐标数组
            hv_ResultRow = new HTuple();
            hv_ResultColumn = new HTuple();
            //产生直线xld
            ho_RegionLines.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_RegionLines, hv_Row1.TupleConcat(hv_Row2),
                hv_Column1.TupleConcat(hv_Column2));
            //存储到显示对象
            //concat_obj (Regions, RegionLines, Regions)
            //计算直线与x轴的夹角，逆时针方向为正向。
            HOperatorSet.AngleLx(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_ATan);

            //边缘检测方向垂直于检测直线：直线方向正向旋转90°为边缘检测方向
            hv_ATan = hv_ATan + ((new HTuple(90)).TupleRad());

            //根据检测直线按顺序产生测量区域矩形，并存储到显示对象
            HTuple end_val18 = hv_Elements;
            HTuple step_val18 = 1;
            for (hv_i = 1; hv_i.Continue(end_val18, step_val18); hv_i = hv_i.TupleAdd(step_val18))
            {
                //RowC := Row1+(((Row2-Row1)*i)/(Elements+1))
                //ColC := Column1+(Column2-Column1)*i/(Elements+1)
                //if (RowC>Height-1 or RowC<0 or ColC>Width-1 or ColC<0)
                //continue
                //endif
                //如果只有一个测量矩形，作为卡尺工具，宽度为检测直线的长度
                if ((int)(new HTuple(hv_Elements.TupleEqual(1))) != 0)
                {
                    hv_RowC = (hv_Row1 + hv_Row2) * 0.5;
                    hv_ColC = (hv_Column1 + hv_Column2) * 0.5;
                    //判断是否超出图像,超出不检测边缘
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                        new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                        hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
                    {
                        continue;
                    }
                    HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Distance);
                    hv_DetectWidth_COPY_INP_TMP = hv_Distance.Clone();
                    //gen_rectangle2_contour_xld (Rectangle, RowC, ColC, ATan, DetectHeight/2, Distance/2)
                }
                else
                {
                    //如果有多个测量矩形，产生该测量矩形xld
                    hv_RowC = hv_Row1 + (((hv_Row2 - hv_Row1) * (hv_i - 1)) / (hv_Elements - 1));
                    hv_ColC = hv_Column1 + (((hv_Column2 - hv_Column1) * (hv_i - 1)) / (hv_Elements - 1));
                    //判断是否超出图像,超出不检测边缘
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                        new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                        hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
                    {
                        continue;
                    }
                    //gen_rectangle2_contour_xld (Rectangle, RowC, ColC, ATan, DetectHeight/2, DetectWidth/2)
                }

                //把测量矩形xld存储到显示对象
                //concat_obj (Regions, Rectangle, Regions)
                //if (i=1)
                //在第一个测量矩形绘制一个箭头xld，用于只是边缘检测方向
                //RowL2 := RowC+DetectHeight/2*sin(-ATan)
                //RowL1 := RowC-DetectHeight/2*sin(-ATan)
                //ColL2 := ColC+DetectHeight/2*cos(-ATan)
                //ColL1 := ColC-DetectHeight/2*cos(-ATan)
                //gen_arrow_contour_xld (Arrow1, RowL1, ColL1, RowL2, ColL2, 15, 15)
                //把xld存储到显示对象
                //concat_obj (Regions, Arrow1, Regions)
                //endif


                //产生测量对象句柄
                HOperatorSet.GenMeasureRectangle2(hv_RowC, hv_ColC, hv_ATan, hv_DetectHeight / 2,
                    hv_DetectWidth_COPY_INP_TMP / 2, hv_Width, hv_Height, "nearest_neighbor",
                    out hv_MsrHandle_Measure);

                //设置极性
                if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("negative"))) != 0)
                {
                    hv_Transition_COPY_INP_TMP = "negative";
                }
                else
                {
                    if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("positive"))) != 0)
                    {

                        hv_Transition_COPY_INP_TMP = "positive";
                    }
                    else
                    {
                        hv_Transition_COPY_INP_TMP = "all";
                    }
                }
                //设置边缘位置。最强点是从所有边缘中选择幅度绝对值最大点，需要设置为'all'
                if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("first"))) != 0)
                {
                    hv_Select_COPY_INP_TMP = "first";
                }
                else
                {
                    if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("last"))) != 0)
                    {

                        hv_Select_COPY_INP_TMP = "last";
                    }
                    else
                    {
                        hv_Select_COPY_INP_TMP = "all";
                    }
                }
                //检测边缘
                HOperatorSet.MeasurePos(ho_Image, hv_MsrHandle_Measure, hv_Sigma, hv_Threshold,
                    hv_Transition_COPY_INP_TMP, hv_Select_COPY_INP_TMP, out hv_RowEdge, out hv_ColEdge,
                    out hv_Amplitude, out hv_Distance);
                //清除测量对象句柄
                HOperatorSet.CloseMeasure(hv_MsrHandle_Measure);

                //临时变量初始化
                //tRow，tCol保存找到指定边缘的坐标
                hv_tRow = 0;
                hv_tCol = 0;
                //t保存边缘的幅度绝对值
                hv_t = 0;
                //找到的边缘必须至少为1个
                HOperatorSet.TupleLength(hv_RowEdge, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
                {
                    continue;
                }
                //有多个边缘时，选择幅度绝对值最大的边缘
                HTuple end_val102 = hv_Number - 1;
                HTuple step_val102 = 1;
                for (hv_j = 0; hv_j.Continue(end_val102, step_val102); hv_j = hv_j.TupleAdd(step_val102))
                {
                    if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_j))).TupleAbs())).TupleGreater(
                        hv_t))) != 0)
                    {

                        hv_tRow = hv_RowEdge.TupleSelect(hv_j);
                        hv_tCol = hv_ColEdge.TupleSelect(hv_j);
                        hv_t = ((hv_Amplitude.TupleSelect(hv_j))).TupleAbs();
                    }
                }
                //把找到的边缘保存在输出数组
                if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
                {
                    hv_ResultRow = hv_ResultRow.TupleConcat(hv_tRow);
                    hv_ResultColumn = hv_ResultColumn.TupleConcat(hv_tCol);
                }
            }
            if ((int)(new HTuple((new HTuple(hv_ResultRow.TupleLength())).TupleGreater(0))) != 0)
            {
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_ResultRow, hv_ResultColumn,
                    4, 0.785398);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Regions, ho_Cross, out ExpTmpOutVar_0);
                    ho_Regions.Dispose();
                    ho_Regions = ExpTmpOutVar_0;
                }
            }
            ho_RegionLines.Dispose();
            ho_Cross.Dispose();

            return;
        }

        static private void ExtendLine(HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2,
              HTuple hv_ExtendLength, out HTuple hv_RowStart, out HTuple hv_ColStart, out HTuple hv_RowEnd,
                 out HTuple hv_ColEnd)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Phi = null;
            // Initialize local and output iconic variables 

            //获取该直线的位置信息
            HOperatorSet.AngleLx(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Phi);
            //line_position (Row1, Column1, Row2, Column2, RowCenter, ColCenter, Length, Phi)
            //********************生成延长线***********************
            //延长线长度（不精确）
            //ExtendLength := 200

            //起点
            hv_RowStart = hv_Row1 - ((((hv_Phi + 1.5707963)).TupleCos()) * hv_ExtendLength);
            hv_ColStart = hv_Column1 - ((((hv_Phi + 1.5707963)).TupleSin()) * hv_ExtendLength);
            //终点
            hv_RowEnd = hv_Row2 - ((((hv_Phi - 1.5707963)).TupleCos()) * hv_ExtendLength);
            hv_ColEnd = hv_Column2 - ((((hv_Phi - 1.5707963)).TupleSin()) * hv_ExtendLength);


            return;
        }

        public void get_rake_region(HObject ho_Image, out HObject ho_Regions, HTuple hv_Elements,
           HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_Row1, HTuple hv_Column1,
            HTuple hv_Row2, HTuple hv_Column2)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionLines, ho_Rectangle = null;
            HObject ho_Arrow1 = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_ResultRow = null;
            HTuple hv_ResultColumn = null, hv_ATan = null, hv_i = null;
            HTuple hv_RowC = new HTuple(), hv_ColC = new HTuple();
            HTuple hv_Distance = new HTuple(), hv_RowL2 = new HTuple();
            HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
            HTuple hv_ColL1 = new HTuple();
            HTuple hv_DetectWidth_COPY_INP_TMP = hv_DetectWidth.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            //获取图像尺寸
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            //产生一个空显示对象，用于显示
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);
            //初始化边缘坐标数组
            hv_ResultRow = new HTuple();
            hv_ResultColumn = new HTuple();
            //产生直线xld
            ho_RegionLines.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_RegionLines, hv_Row1.TupleConcat(hv_Row2),
                hv_Column1.TupleConcat(hv_Column2));
            //存储到显示对象
            //concat_obj (Regions, RegionLines, Regions)
            //计算直线与x轴的夹角，逆时针方向为正向。
            HOperatorSet.AngleLx(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_ATan);

            //边缘检测方向垂直于检测直线：直线方向正向旋转90°为边缘检测方向
            hv_ATan = hv_ATan + ((new HTuple(90)).TupleRad());

            //根据检测直线按顺序产生测量区域矩形，并存储到显示对象
            HTuple end_val18 = hv_Elements;
            HTuple step_val18 = 1;
            for (hv_i = 1; hv_i.Continue(end_val18, step_val18); hv_i = hv_i.TupleAdd(step_val18))
            {
                //RowC := Row1+(((Row2-Row1)*i)/(Elements+1))
                //ColC := Column1+(Column2-Column1)*i/(Elements+1)
                //if (RowC>Height-1 or RowC<0 or ColC>Width-1 or ColC<0)
                //continue
                //endif
                //如果只有一个测量矩形，作为卡尺工具，宽度为检测直线的长度
                if ((int)(new HTuple(hv_Elements.TupleEqual(1))) != 0)
                {
                    hv_RowC = (hv_Row1 + hv_Row2) * 0.5;
                    hv_ColC = (hv_Column1 + hv_Column2) * 0.5;
                    //判断是否超出图像,超出不检测边缘
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                        new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                        hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
                    {
                        continue;
                    }
                    HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Distance);
                    hv_DetectWidth_COPY_INP_TMP = hv_Distance.Clone();
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                        hv_ATan, hv_DetectHeight / 2, hv_Distance / 2);
                }
                else
                {
                    //如果有多个测量矩形，产生该测量矩形xld
                    hv_RowC = hv_Row1 + (((hv_Row2 - hv_Row1) * (hv_i - 1)) / (hv_Elements - 1));
                    hv_ColC = hv_Column1 + (((hv_Column2 - hv_Column1) * (hv_i - 1)) / (hv_Elements - 1));
                    //判断是否超出图像,超出不检测边缘
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                        new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                        hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
                    {
                        continue;
                    }
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                        hv_ATan, hv_DetectHeight / 2, hv_DetectWidth_COPY_INP_TMP / 2);
                }

                //把测量矩形xld存储到显示对象
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle, out ExpTmpOutVar_0);
                    ho_Regions.Dispose();
                    ho_Regions = ExpTmpOutVar_0;
                }
                if ((int)(new HTuple(hv_i.TupleEqual(1))) != 0)
                {
                    //在第一个测量矩形绘制一个箭头xld，用于只是边缘检测方向
                    hv_RowL2 = hv_RowC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                    hv_RowL1 = hv_RowC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                    hv_ColL2 = hv_ColC + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                    hv_ColL1 = hv_ColC - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                    ho_Arrow1.Dispose();
                    gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                        15, 15);
                    //把xld存储到显示对象
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                        ho_Regions.Dispose();
                        ho_Regions = ExpTmpOutVar_0;
                    }
                }

            }

            ho_RegionLines.Dispose();
            ho_Rectangle.Dispose();
            ho_Arrow1.Dispose();

            return;
        }

        #endregion

    }
}
