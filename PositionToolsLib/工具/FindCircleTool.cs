﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PositionToolsLib.参数;
using OSLog;
using HalconDotNet;
using System.Runtime.Serialization;

namespace PositionToolsLib.工具
{
    /// <summary>
    /// 查找圆检测参数
    /// </summary>
    [Serializable]
    public class FindCircleTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public FindCircleTool()
        {
            toolParam = new FindCircleParam();
            toolName = "查找圆" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("查找圆");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
          
            int number = int.Parse(toolName.Replace("查找圆", ""));
            if (number > inum)
                inum = number;
            //toolName = "查找圆" + number;
            inum++;
        }

       
        /// <summary>
        ///查找圆工具运行
        /// </summary>
        /// <returns></returns>
        override public RunResult Run()
        {
            DataManage dm = GetManage();
            if (!dm.enumerableTooDic.Contains(toolName))
                dm.enumerableTooDic.Add(toolName);
            RunResult result = new RunResult();
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (sw == null) sw = new System.Diagnostics.Stopwatch();
            sw.Restart();

            try
            {

                (toolParam as FindCircleParam).InputImg = dm.imageBufDic[(toolParam as FindCircleParam).InputImageName];
                if (!ObjectValided((toolParam as FindCircleParam).InputImg))
                {
                    (toolParam as FindCircleParam).FindCircleRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                /*----加入图像格式判断防止多通道----*/
                #region---图像格式判断---
                HOperatorSet.CountChannels((toolParam as FindCircleParam).InputImg, out HTuple channels);
                HObject grayImage = null;
                if (channels[0].I > 1)
                    HOperatorSet.Rgb1ToGray((toolParam as FindCircleParam).InputImg, out grayImage);
                else
                    grayImage = (toolParam as FindCircleParam).InputImg.Clone();
                #endregion

                if (!ObjectValided((toolParam as FindCircleParam).InspectXLD))
                {
                    (toolParam as FindCircleParam).FindCircleRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = "检测区域无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                //仿射变换矩阵
                HObject xld = (toolParam as FindCircleParam).InspectXLD;
                if ((toolParam as FindCircleParam).UsePosiCorrect)
                {
                    HTuple matrix2D = dm.matrixBufDic[(toolParam as FindCircleParam).MatrixName];
                    if (matrix2D != null)
                        HOperatorSet.AffineTransContourXld((toolParam as FindCircleParam).InspectXLD,
                          out xld, matrix2D);
                    else
                    {
                        if (!dm.resultFlagDic.ContainsKey(toolName))
                            dm.resultFlagDic.Add(toolName, false);
                        else
                            dm.resultFlagDic[toolName] = false;
                        (toolParam as FindCircleParam).FindCircleRunStatus = false;
                        result.runFlag = false;
                        result.errInfo = toolName + "检测区域位置补正异常";
                        return result;
                    }
                }
                //(toolParam as FindLineParam).ResultInspectROI = inspectROI.Clone();

                HOperatorSet.GetContourXld(xld, out HTuple hv_Rows, out HTuple hv_Columns);

                get_spoke_region(grayImage, out HObject ho_Regions,
                   (toolParam as FindCircleParam).CaliperNum, (toolParam as FindCircleParam).CaliperHeight,
                    (toolParam as FindCircleParam).CaliperWidth, hv_Rows, hv_Columns,
                      Enum.GetName(typeof(EumDirection), (toolParam as FindCircleParam).Direction));
                (toolParam as FindCircleParam).ResultInspectROI = ho_Regions.Clone();
                //卡尺
                spoke(grayImage, out HObject ho_Regions2,
                    (toolParam as FindCircleParam).CaliperNum, (toolParam as FindCircleParam).CaliperHeight,
                    (toolParam as FindCircleParam).CaliperWidth, 1.0,
                    (toolParam as FindCircleParam).EdgeThd,
                    Enum.GetName(typeof(EumTransition), (toolParam as FindCircleParam).Transition),
                    Enum.GetName(typeof(EumSelect), (toolParam as FindCircleParam).Select),
                   hv_Rows, hv_Columns,
                      Enum.GetName(typeof(EumDirection), (toolParam as FindCircleParam).Direction),
                   out HTuple hv_ResultRow, out HTuple hv_ResultColumn, out HTuple hv_ArcType);
                //至少三点或以上才可拟合圆
                if (hv_ResultRow.TupleLength() <= 0)
                {
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    (toolParam as FindCircleParam).FindCircleRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "圆查找失败";
                    return result;
                }

                //结果圆
                pts_to_best_circle(out HObject ho_Circle, hv_ResultRow, hv_ResultColumn, "circle",
                   3, out HTuple hv_RowCenter, out HTuple hv_ColCenter, out HTuple hv_Radius, 
                   out HTuple hv_StartPhi, out HTuple hv_EndPhi, out HTuple hv_PointOrder, out HTuple hv_ArcAngle);


                if (hv_RowCenter.TupleLength() <= 0)
                {
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    (toolParam as FindCircleParam).FindCircleRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "圆查找失败";
                    return result;
                }

                HOperatorSet.ConcatObj(ho_Regions2, ho_Circle, out HObject emptyRegionBuf);

                if (hv_RowCenter.TupleLength() > 0)
                {
                    HOperatorSet.GenCrossContourXld(out HObject cross, hv_RowCenter.D, hv_ColCenter.D, 20, 0);
                    if (ObjectValided(cross))
                        HOperatorSet.ConcatObj(emptyRegionBuf, cross, out emptyRegionBuf);
                }


                if (!dm.resultBufDic.ContainsKey(toolName))
                    dm.resultBufDic.Add(toolName, emptyRegionBuf.Clone());
                else
                    dm.resultBufDic[toolName] = emptyRegionBuf.Clone();

                string info = toolName+"检测完成";
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;

                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, hv_RowCenter.TupleLength() > 0);
                else
                    dm.resultFlagDic[toolName] = hv_RowCenter.TupleLength() > 0;

                if(hv_RowCenter.TupleLength() > 0)
                {
                    if (!dm.PositionDataDic.ContainsKey(toolName))
                        dm.PositionDataDic.Add(toolName, new StuCoordinateData(hv_RowCenter.D,
                            hv_ColCenter.D, 0));
                    else
                        dm.PositionDataDic[toolName] = new StuCoordinateData(hv_RowCenter.D,
                            hv_ColCenter.D, 0);

                }
                else
                {
                    if (!dm.PositionDataDic.ContainsKey(toolName))
                        dm.PositionDataDic.Add(toolName, new StuCoordinateData(0,
                            0, 0));
                    else
                        dm.PositionDataDic[toolName] = new StuCoordinateData(0,
                            0, 0);
                }

                grayImage.Dispose();
                //+输入图像

                HOperatorSet.ConcatObj((toolParam as FindCircleParam).InputImg, emptyRegionBuf, out HObject objectsConcat2);
                (toolParam as FindCircleParam).OutputImg = objectsConcat2;
                (toolParam as FindCircleParam).Row= hv_RowCenter.D;
                (toolParam as FindCircleParam).Column = hv_ColCenter.D;
                (toolParam as FindCircleParam).Radius = hv_Radius.D;
                result.runFlag = true;
                (toolParam as FindCircleParam).FindCircleRunStatus = true;
                emptyRegionBuf.Dispose();
            }
            catch (Exception er)
            {
                string info = string.Format(toolName+"检测异常:{0}", er.Message);
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;

                log.Error(funName, er.Message);
                result.runFlag = false;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, false);
                else
                    dm.resultFlagDic[toolName] = false;
                (toolParam as FindCircleParam).FindCircleRunStatus = false;
                result.errInfo = er.Message;
                sw.Stop();
                result.runTime = sw.ElapsedMilliseconds;
                return result;
            }
            sw.Stop();
            result.runTime = sw.ElapsedMilliseconds;
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

       
        public void get_spoke_region(HObject ho_Image, out HObject ho_Regions, HTuple hv_Elements,
            HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_ROIRows, HTuple hv_ROICols,
            HTuple hv_Direct)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Contour, ho_ContCircle, ho_Rectangle1 = null;
            HObject ho_Arrow1 = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_RowC = null;
            HTuple hv_ColumnC = null, hv_Radius = null, hv_StartPhi = null;
            HTuple hv_EndPhi = null, hv_PointOrder = null, hv_RowXLD = null;
            HTuple hv_ColXLD = null, hv_Length2 = null, hv_i = null;
            HTuple hv_j = new HTuple(), hv_ArcType = new HTuple();
            HTuple hv_RowE = new HTuple(), hv_ColE = new HTuple();
            HTuple hv_ATan = new HTuple(), hv_RowL2 = new HTuple();
            HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
            HTuple hv_ColL1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            //获取图像尺寸
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            //产生一个空显示对象，用于显示
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);

            //产生xld
            ho_Contour.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ROIRows, hv_ROICols);
            //用回归线法（不抛出异常点，所有点权重一样）拟合圆
            HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 1, 2, out hv_RowC,
                out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
            //根据拟合结果产生xld，并保持到显示对象
            ho_ContCircle.Dispose();
            HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_RowC, hv_ColumnC, hv_Radius,
                hv_StartPhi, hv_EndPhi, hv_PointOrder, 3);
            //concat_obj (Regions, ContCircle, Regions)

            //获取圆或圆弧xld上的点坐标
            HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);

            //求圆或圆弧xld上的点的数量
            HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
            if ((int)(new HTuple(hv_Elements.TupleLess(3))) != 0)
            { 
                //提示
                //if (hv_WindowHandle.D > 100000)
                //    HOperatorSet.SetFont(hv_WindowHandle, "Courier New-" + "12");
                //else
                //    HOperatorSet.SetFont(hv_WindowHandle, "-Courier New-" + "12" + "-*-*-*-*-1-");
                //disp_message(hv_WindowHandle, "检测的边缘数量太少，请重新设置!", "window", 52, 12, "red",
                //    "false");
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            //如果xld是圆弧，有Length2个点，从起点开始，等间距（间距为Length2/(Elements-1)）取Elements个点，作为卡尺工具的中点
            //如果xld是圆，有Length2个点，以0°为起点，从起点开始，等间距（间距为Length2/(Elements)）取Elements个点，作为卡尺工具的中点
            HTuple end_val24 = hv_Elements - 1;
            HTuple step_val24 = 1;
            for (hv_i = 0; hv_i.Continue(end_val24, step_val24); hv_i = hv_i.TupleAdd(step_val24))
            {

                if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                    hv_Length2 - 1)))) != 0)
                {
                    //xld的起点和终点坐标相对，为圆
                    HOperatorSet.TupleInt(((1.0 * hv_Length2) / hv_Elements) * hv_i, out hv_j);
                    hv_ArcType = "circle";
                }
                else
                {
                    //否则为圆弧
                    HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                    hv_ArcType = "arc";
                }
                //索引越界，强制赋值为最后一个索引
                if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
                {
                    hv_j = hv_Length2 - 1;
                    //continue
                }
                //获取卡尺工具中心
                hv_RowE = hv_RowXLD.TupleSelect(hv_j);
                hv_ColE = hv_ColXLD.TupleSelect(hv_j);

                //超出图像区域，不检测，否则容易报异常
                if ((int)((new HTuple((new HTuple((new HTuple(hv_RowE.TupleGreater(hv_Height - 1))).TupleOr(
                    new HTuple(hv_RowE.TupleLess(0))))).TupleOr(new HTuple(hv_ColE.TupleGreater(
                    hv_Width - 1))))).TupleOr(new HTuple(hv_ColE.TupleLess(0)))) != 0)
                {
                    continue;
                }
                //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
                if ((int)(new HTuple(hv_Direct.TupleEqual("inner"))) != 0)
                {
                    //求卡尺工具的边缘搜索方向
                    //求圆心指向边缘的矢量的角度
                    HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                    //角度反向
                    hv_ATan = ((new HTuple(180)).TupleRad()) + hv_ATan;
                }
                else
                {
                    //求卡尺工具的边缘搜索方向
                    //求圆心指向边缘的矢量的角度
                    HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                }


                //产生卡尺xld，并保持到显示对象
                ho_Rectangle1.Dispose();
                HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle1, hv_RowE, hv_ColE, hv_ATan,
                    hv_DetectHeight / 2, hv_DetectWidth / 2);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle1, out ExpTmpOutVar_0);
                    ho_Regions.Dispose();
                    ho_Regions = ExpTmpOutVar_0;
                }
                //用箭头xld指示边缘搜索方向，并保持到显示对象
                if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
                {
                    hv_RowL2 = hv_RowE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                    hv_RowL1 = hv_RowE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                    hv_ColL2 = hv_ColE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                    hv_ColL1 = hv_ColE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                    ho_Arrow1.Dispose();
                    gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                        5, 5);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                        ho_Regions.Dispose();
                        ho_Regions = ExpTmpOutVar_0;
                    }
                }

            }

            ho_Contour.Dispose();
            ho_ContCircle.Dispose();
            ho_Rectangle1.Dispose();
            ho_Arrow1.Dispose();

            return;
        }

        public void spoke(HObject ho_Image,  out HObject ho_Regions, HTuple hv_Elements,
            HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_Sigma, HTuple hv_Threshold,
            HTuple hv_Transition, HTuple hv_Select, HTuple hv_ROIRows, HTuple hv_ROICols,
            HTuple hv_Direct, out HTuple hv_ResultRow, out HTuple hv_ResultColumn, out HTuple hv_ArcType)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Contour, ho_ContCircle, ho_Cross = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_RowC = null;
            HTuple hv_ColumnC = null, hv_Radius = null, hv_StartPhi = null;
            HTuple hv_EndPhi = null, hv_PointOrder = null, hv_RowXLD = null;
            HTuple hv_ColXLD = null, hv_Length2 = null, hv_i = null;
            HTuple hv_j = new HTuple(), hv_RowE = new HTuple(), hv_ColE = new HTuple();
            HTuple hv_ATan = new HTuple(), hv_MsrHandle_Measure = new HTuple();
            HTuple hv_RowEdge = new HTuple(), hv_ColEdge = new HTuple();
            HTuple hv_Amplitude = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_tRow = new HTuple(), hv_tCol = new HTuple();
            HTuple hv_t = new HTuple(), hv_Number = new HTuple(), hv_k = new HTuple();
            HTuple hv_Select_COPY_INP_TMP = hv_Select.Clone();
            HTuple hv_Transition_COPY_INP_TMP = hv_Transition.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            hv_ArcType = new HTuple();
            //获取图像尺寸
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            //产生一个空显示对象，用于显示
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);
            //初始化边缘坐标数组
            hv_ResultRow = new HTuple();
            hv_ResultColumn = new HTuple();

            //产生xld
            ho_Contour.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ROIRows, hv_ROICols);
            //用回归线法（不抛出异常点，所有点权重一样）拟合圆
            HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 1, 2, out hv_RowC,
                out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
            //根据拟合结果产生xld，并保持到显示对象
            ho_ContCircle.Dispose();
            HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_RowC, hv_ColumnC, hv_Radius,
                hv_StartPhi, hv_EndPhi, hv_PointOrder, 3);
            //concat_obj (Regions, ContCircle, Regions)

            //获取圆或圆弧xld上的点坐标
            HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);

            //求圆或圆弧xld上的点的数量
            HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
            if ((int)(new HTuple(hv_Elements.TupleLess(3))) != 0)
            {
                //提示
                //if (hv_WindowHandle.D > 100000)
                //    HOperatorSet.SetFont(hv_WindowHandle, "Courier New-" + "12");
                //else
                //    HOperatorSet.SetFont(hv_WindowHandle, "-Courier New-" + "12" + "-*-*-*-*-1-");

                //disp_message(hv_WindowHandle, "检测的边缘数量太少，请重新设置!", "window", 52, 12, "red",
                //    "false");
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Cross.Dispose();

                return;
            }
            //如果xld是圆弧，有Length2个点，从起点开始，等间距（间距为Length2/(Elements-1)）取Elements个点，作为卡尺工具的中点
            //如果xld是圆，有Length2个点，以0°为起点，从起点开始，等间距（间距为Length2/(Elements)）取Elements个点，作为卡尺工具的中点
            HTuple end_val27 = hv_Elements - 1;
            HTuple step_val27 = 1;
            for (hv_i = 0; hv_i.Continue(end_val27, step_val27); hv_i = hv_i.TupleAdd(step_val27))
            {

                if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                    hv_Length2 - 1)))) != 0)
                {
                    //xld的起点和终点坐标相对，为圆
                    HOperatorSet.TupleInt(((1.0 * hv_Length2) / hv_Elements) * hv_i, out hv_j);
                    hv_ArcType = "circle";
                }
                else
                {
                    //否则为圆弧
                    HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                    hv_ArcType = "arc";
                }
                //索引越界，强制赋值为最后一个索引
                if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
                {
                    hv_j = hv_Length2 - 1;
                    //continue
                }
                //获取卡尺工具中心
                hv_RowE = hv_RowXLD.TupleSelect(hv_j);
                hv_ColE = hv_ColXLD.TupleSelect(hv_j);

                //超出图像区域，不检测，否则容易报异常
                if ((int)((new HTuple((new HTuple((new HTuple(hv_RowE.TupleGreater(hv_Height - 1))).TupleOr(
                    new HTuple(hv_RowE.TupleLess(0))))).TupleOr(new HTuple(hv_ColE.TupleGreater(
                    hv_Width - 1))))).TupleOr(new HTuple(hv_ColE.TupleLess(0)))) != 0)
                {
                    continue;
                }
                //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
                if ((int)(new HTuple(hv_Direct.TupleEqual("inner"))) != 0)
                {
                    //求卡尺工具的边缘搜索方向
                    //求圆心指向边缘的矢量的角度
                    HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                    //角度反向
                    hv_ATan = ((new HTuple(180)).TupleRad()) + hv_ATan;
                }
                else
                {
                    //求卡尺工具的边缘搜索方向
                    //求圆心指向边缘的矢量的角度
                    HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                }


                //产生卡尺xld，并保持到显示对象
                //gen_rectangle2_contour_xld (Rectangle1, RowE, ColE, ATan, DetectHeight/2, DetectWidth/2)
                //concat_obj (Regions, Rectangle1, Regions)
                //用箭头xld指示边缘搜索方向，并保持到显示对象
                //if (i=0)
                //RowL2 := RowE+DetectHeight/2*sin(-ATan)
                //RowL1 := RowE-DetectHeight/2*sin(-ATan)
                //ColL2 := ColE+DetectHeight/2*cos(-ATan)
                //ColL1 := ColE-DetectHeight/2*cos(-ATan)
                //gen_arrow_contour_xld (Arrow1, RowL1, ColL1, RowL2, ColL2, 5, 5)
                //concat_obj (Regions, Arrow1, Regions)
                //endif


                //产生测量对象句柄
                HOperatorSet.GenMeasureRectangle2(hv_RowE, hv_ColE, hv_ATan, hv_DetectHeight / 2,
                    hv_DetectWidth / 2, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);

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
                HOperatorSet.TupleLength(hv_RowEdge, out hv_Number);
                //找到的边缘必须至少为1个
                if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
                {
                    continue;
                }
                //有多个边缘时，选择幅度绝对值最大的边缘
                HTuple end_val122 = hv_Number - 1;
                HTuple step_val122 = 1;
                for (hv_k = 0; hv_k.Continue(end_val122, step_val122); hv_k = hv_k.TupleAdd(step_val122))
                {
                    if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_k))).TupleAbs())).TupleGreater(
                        hv_t))) != 0)
                    {

                        hv_tRow = hv_RowEdge.TupleSelect(hv_k);
                        hv_tCol = hv_ColEdge.TupleSelect(hv_k);
                        hv_t = ((hv_Amplitude.TupleSelect(hv_k))).TupleAbs();
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
            ho_Contour.Dispose();
            ho_ContCircle.Dispose();
            ho_Cross.Dispose();

            return;
        }

        public void pts_to_best_circle(out HObject ho_Circle, HTuple hv_Rows, HTuple hv_Cols,
            HTuple hv_ArcType, HTuple hv_ActiveNum, out HTuple hv_RowCenter, out HTuple hv_ColCenter,
            out HTuple hv_Radius, out HTuple hv_StartPhi, out HTuple hv_EndPhi, out HTuple hv_PointOrder,
            out HTuple hv_ArcAngle)
        {



            // Local iconic variables 

            HObject ho_Contour = null, ho_Circle1 = null, ho_Circle2 = null;

            // Local control variables 

            HTuple hv_Length = null, hv_Length1 = new HTuple();
            HTuple hv_DistanceMin1 = new HTuple(), hv_DistanceMax1 = new HTuple();
            HTuple hv_DistanceMin2 = new HTuple(), hv_DistanceMax2 = new HTuple();
            HTuple hv_Sum1 = new HTuple(), hv_Sum2 = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Col = new HTuple(), hv_CircleLength = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_Circle1);
            HOperatorSet.GenEmptyObj(out ho_Circle2);
            hv_StartPhi = new HTuple();
            hv_EndPhi = new HTuple();
            hv_PointOrder = new HTuple();
            hv_ArcAngle = new HTuple();
            //初始化
            hv_RowCenter = 0;
            hv_ColCenter = 0;
            hv_Radius = 0;
            //_Rows := []
            //_Cols := []
            //产生一个空的直线对象，用于保存拟合后的圆
            ho_Circle.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Circle);
            //计算边缘数量
            HOperatorSet.TupleLength(hv_Cols, out hv_Length);
            //当边缘数量不小于有效点数时进行拟合
            if ((int)((new HTuple(hv_Length.TupleGreaterEqual(hv_ActiveNum))).TupleAnd(new HTuple(hv_ActiveNum.TupleGreater(
                2)))) != 0)
            {
                //halcon的拟合是基于xld的，需要把边缘连接成xld
                if ((int)(new HTuple(hv_ArcType.TupleEqual("circle"))) != 0)
                {
                    //如果是闭合的圆，轮廓需要首尾相连
                    //_Rows := [_Rows,Rows,Rows[0]]
                    //_Cols := [_Cols,Cols,Cols[0]]
                    ho_Contour.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows.TupleConcat(hv_Rows.TupleSelect(
                        0)), hv_Cols.TupleConcat(hv_Cols.TupleSelect(0)));
                    //gen_contour_polygon_xld (Contour, Rows, Cols)
                }
                else
                {
                    ho_Contour.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Cols);
                }
                //拟合圆。使用的算法是''geotukey''，其他算法请参考fit_circle_contour_xld的描述部分。
                HOperatorSet.FitCircleContourXld(ho_Contour, "geotukey", -1, 0, 0, 3, 2, out hv_RowCenter,
                    out hv_ColCenter, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                //判断拟合结果是否有效：如果拟合成功，数组中元素的数量大于0
                HOperatorSet.TupleLength(hv_StartPhi, out hv_Length1);
                if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
                {
                    ho_Contour.Dispose();
                    ho_Circle1.Dispose();
                    ho_Circle2.Dispose();

                    return;
                }
                //根据拟合结果，产生直线xld
                if ((int)(new HTuple(hv_ArcType.TupleEqual("arc"))) != 0)
                {
                    //判断圆弧的方向：顺时针还是逆时针
                    //halcon求圆弧会出现方向混乱的问题
                    ho_Circle1.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_Circle1, hv_RowCenter, hv_ColCenter,
                        hv_Radius, hv_StartPhi, hv_EndPhi, "positive", 1);
                    ho_Circle2.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_Circle2, hv_RowCenter, hv_ColCenter,
                        hv_Radius, hv_StartPhi, hv_EndPhi, "negative", 1);

                    HOperatorSet.DistancePc(ho_Circle1, hv_Rows, hv_Cols, out hv_DistanceMin1,
                        out hv_DistanceMax1);
                    HOperatorSet.DistancePc(ho_Circle2, hv_Rows, hv_Cols, out hv_DistanceMin2,
                        out hv_DistanceMax2);
                    HOperatorSet.TupleSum(hv_DistanceMin1, out hv_Sum1);
                    HOperatorSet.TupleSum(hv_DistanceMin2, out hv_Sum2);
                    if ((int)(new HTuple(hv_Sum1.TupleLess(hv_Sum2))) != 0)
                    {
                        hv_PointOrder = "positive";
                    }
                    else
                    {
                        hv_PointOrder = "negative";
                    }
                    ho_Circle.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                        hv_Radius, hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);
                    HOperatorSet.GetContourXld(ho_Circle, out hv_Row, out hv_Col);
                    HOperatorSet.AngleLl(hv_RowCenter, hv_ColCenter, hv_Row.TupleSelect(0), hv_Col.TupleSelect(
                        0), hv_RowCenter, hv_ColCenter, hv_Row.TupleSelect((new HTuple(hv_Row.TupleLength()
                        )) - 1), hv_Col.TupleSelect((new HTuple(hv_Row.TupleLength())) - 1), out hv_ArcAngle);
                    if ((int)(0) != 0)
                    {
                        HOperatorSet.LengthXld(ho_Circle, out hv_CircleLength);
                        hv_ArcAngle = hv_EndPhi - hv_StartPhi;
                        if ((int)(new HTuple(hv_CircleLength.TupleGreater(((new HTuple(180)).TupleRad()
                            ) * hv_Radius))) != 0)
                        {
                            if ((int)(new HTuple(((hv_ArcAngle.TupleAbs())).TupleLess((new HTuple(180)).TupleRad()
                                ))) != 0)
                            {
                                if ((int)(new HTuple(hv_ArcAngle.TupleGreater(0))) != 0)
                                {
                                    hv_ArcAngle = ((new HTuple(360)).TupleRad()) - hv_ArcAngle;
                                }
                                else
                                {
                                    hv_ArcAngle = ((new HTuple(360)).TupleRad()) + hv_ArcAngle;
                                }
                            }
                        }
                        else
                        {
                            if ((int)(new HTuple(hv_CircleLength.TupleLess(((new HTuple(180)).TupleRad()
                                ) * hv_Radius))) != 0)
                            {
                                if ((int)(new HTuple(((hv_ArcAngle.TupleAbs())).TupleGreater((new HTuple(180)).TupleRad()
                                    ))) != 0)
                                {
                                    if ((int)(new HTuple(hv_ArcAngle.TupleGreater(0))) != 0)
                                    {
                                        hv_ArcAngle = hv_ArcAngle - ((new HTuple(360)).TupleRad());
                                    }
                                    else
                                    {
                                        hv_ArcAngle = ((new HTuple(360)).TupleRad()) + hv_ArcAngle;
                                    }
                                }
                            }

                        }
                    }
                }
                else
                {
                    hv_StartPhi = 0;
                    hv_EndPhi = (new HTuple(360)).TupleRad();
                    hv_ArcAngle = (new HTuple(360)).TupleRad();
                    ho_Circle.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                        hv_Radius, hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);
                }
            }

            ho_Contour.Dispose();
            ho_Circle1.Dispose();
            ho_Circle2.Dispose();

            return;
        }
        static public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            // Local iconic variables 

            // Local control variables 

            HTuple hv_Red = null, hv_Green = null, hv_Blue = null;
            HTuple hv_Row1Part = null, hv_Column1Part = null, hv_Row2Part = null;
            HTuple hv_Column2Part = null, hv_RowWin = null, hv_ColumnWin = null;
            HTuple hv_WidthWin = null, hv_HeightWin = null, hv_MaxAscent = null;
            HTuple hv_MaxDescent = null, hv_MaxWidth = null, hv_MaxHeight = null;
            HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRow = new HTuple();
            HTuple hv_FactorColumn = new HTuple(), hv_UseShadow = null;
            HTuple hv_ShadowColor = null, hv_Exception = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv_W = new HTuple(), hv_H = new HTuple(), hv_FrameHeight = new HTuple();
            HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
            HTuple hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
            HTuple hv_CurrentColor = new HTuple();
            HTuple hv_Box_COPY_INP_TMP = hv_Box.Clone();
            HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
            HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
            HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
            HTuple hv_String_COPY_INP_TMP = hv_String.Clone();

            // Initialize local and output iconic variables 
            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed
            //String: A tuple of strings containing the text message to be displayed
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Column: The column coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Color: defines the color of the text as string.
            //   If set to [], '' or 'auto' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically
            //   for each new textline.
            //Box: If Box[0] is set to 'true', the text is written within an orange box.
            //     If set to' false', no box is displayed.
            //     If set to a color string (e.g. 'white', '#FF00CC', etc.),
            //       the text is written in a box of that color.
            //     An optional second value for Box (Box[1]) controls if a shadow is displayed:
            //       'true' -> display a shadow in a default color
            //       'false' -> display no shadow (same as if no second value is given)
            //       otherwise -> use given string as color string for the shadow color
            //
            //Prepare window
            HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
            HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part, out hv_Row2Part,
                out hv_Column2Part);
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
                out hv_WidthWin, out hv_HeightWin);
            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
            //
            //default settings
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
            {
                hv_Color_COPY_INP_TMP = "";
            }
            //
            hv_String_COPY_INP_TMP = ((("" + hv_String_COPY_INP_TMP) + "")).TupleSplit("\n");
            //
            //Estimate extentions of text depending on font size.
            HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
                out hv_MaxWidth, out hv_MaxHeight);
            if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
            {
                hv_R1 = hv_Row_COPY_INP_TMP.Clone();
                hv_C1 = hv_Column_COPY_INP_TMP.Clone();
            }
            else
            {
                //Transform image to window coordinates
                hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
                hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
                hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
                hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
            }
            //
            //Display text box depending on text size
            hv_UseShadow = 1;
            hv_ShadowColor = "gray";
            if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleEqual("true"))) != 0)
            {
                if (hv_Box_COPY_INP_TMP == null)
                    hv_Box_COPY_INP_TMP = new HTuple();
                hv_Box_COPY_INP_TMP[0] = "#fce9d4";
                hv_ShadowColor = "#f28d26";
            }
            if ((int)(new HTuple((new HTuple(hv_Box_COPY_INP_TMP.TupleLength())).TupleGreater(
                1))) != 0)
            {
                if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual("true"))) != 0)
                {
                    //Use default ShadowColor set above
                }
                else if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual(
                    "false"))) != 0)
                {
                    hv_UseShadow = 0;
                }
                else
                {
                    hv_ShadowColor = hv_Box_COPY_INP_TMP[1];
                    //Valid color?
                    try
                    {
                        HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                            1));
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        hv_Exception = "Wrong value of control parameter Box[1] (must be a 'true', 'false', or a valid color string)";
                        throw new HalconException(hv_Exception);
                    }
                }
            }
            if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleNotEqual("false"))) != 0)
            {
                //Valid color?
                try
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_Exception = "Wrong value of control parameter Box[0] (must be a 'true', 'false', or a valid color string)";
                    throw new HalconException(hv_Exception);
                }
                //Calculate box extents
                hv_String_COPY_INP_TMP = (" " + hv_String_COPY_INP_TMP) + " ";
                hv_Width = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                        hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                    hv_Width = hv_Width.TupleConcat(hv_W);
                }
                hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    ));
                hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
                hv_R2 = hv_R1 + hv_FrameHeight;
                hv_C2 = hv_C1 + hv_FrameWidth;
                //Display rectangles
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                //Set shadow color
                HOperatorSet.SetColor(hv_WindowHandle, hv_ShadowColor);
                if ((int)(hv_UseShadow) != 0)
                {
                    HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 1, hv_C1 + 1, hv_R2 + 1, hv_C2 + 1);
                }
                //Set box color
                HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
            }
            //Write text.
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                    )));
                if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                    "auto")))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
                }
                else
                {
                    HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                }
                hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
                HOperatorSet.SetTposition(hv_WindowHandle, hv_Row_COPY_INP_TMP, hv_C1);
                HOperatorSet.WriteString(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                    hv_Index));
            }
            //Reset changed window settings
            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
            HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
                hv_Column2Part);

            return;
        }

        #endregion

    }
}
