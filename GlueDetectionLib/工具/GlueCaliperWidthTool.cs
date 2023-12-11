using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GlueDetectionLib.参数;
using HalconDotNet;
using OSLog;

namespace GlueDetectionLib.工具
{
    /// <summary>
    /// 胶宽检测工具
    /// </summary>
    [Serializable]
    public class GlueCaliperWidthTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public GlueCaliperWidthTool()
        {
            toolParam = new GlueCaliperWidthParam();
            toolName = "胶宽检测" + inum;
            inum++;
        
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("胶宽检测");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("胶宽检测", ""));
            if (number > inum)
                inum = number;
            //toolName = "胶宽检测" + number;
            inum++;
        }
        /// <summary>
        ///胶宽检测工具运行
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
                (toolParam as GlueCaliperWidthParam).InputImg = dm.imageBufDic[(toolParam as GlueCaliperWidthParam).InputImageName];
                if (!ObjectValided((toolParam as GlueCaliperWidthParam).InputImg))
                {
                    (toolParam as GlueCaliperWidthParam).GlueWidthRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+ "输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                }

                if ((toolParam as GlueCaliperWidthParam).RegionBufList.Count<=0)
                {
                    (toolParam as GlueCaliperWidthParam).GlueWidthRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+ "检测区域无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }

                //仿射变换矩阵
                HObject emptyRegions = null;
                HOperatorSet.GenEmptyObj(out emptyRegions);
                foreach (var s in (toolParam as GlueCaliperWidthParam).RegionBufList)
                {
                    HObject xld = s.region;
                    if ((toolParam as GlueCaliperWidthParam).UsePosiCorrect)
                    {
                        HTuple matrix2D = dm.matrixBufDic[(toolParam as GlueCaliperWidthParam).MatrixName];
                        if (matrix2D != null)
                            HOperatorSet.AffineTransContourXld(s.region, out xld, matrix2D);
                        else
                        {
                            if (!dm.resultFlagDic.ContainsKey(toolName))
                                dm.resultFlagDic.Add(toolName, false);
                            else
                                dm.resultFlagDic[toolName] = false;

                            (toolParam as GlueCaliperWidthParam).GlueWidthRunStatus = false;
                            result.runFlag = false;
                            result.errInfo = toolName+"检测区域位置补正异常";
                            return result;
                        }
                    }
                    HOperatorSet.ConcatObj(emptyRegions, xld,out emptyRegions);    
                }

                //(toolParam as GlueCaliperWidthParam).ResultInspectRegions = emptyRegions.Clone();
                HOperatorSet.CountObj(emptyRegions, out HTuple nummber);
                //*********开始检测
                HObject searchRegion = null;
                HOperatorSet.GenEmptyObj(out searchRegion);
                HObject ho_EmptyObject = null;
                HOperatorSet.GenEmptyObj(out ho_EmptyObject);
                (toolParam as GlueCaliperWidthParam).DistanceList.Clear();
                bool pdFlag = true;
                for (int i = 1; i <= nummber; i++)
                {
                    HOperatorSet.SelectObj(emptyRegions, out HObject objectSelected, i);
                    HOperatorSet.GetContourXld(objectSelected, out HTuple hv_Row, out HTuple hv_Column);
                    HTuple hv_Row1 = hv_Row.TupleSelect(0);
                    HTuple hv_Row2 = hv_Row.TupleSelect(1);
                    HTuple hv_Column1 = hv_Column.TupleSelect(0);
                    HTuple hv_Column2 = hv_Column.TupleSelect(1);
                    get_rake2_region((toolParam as GlueCaliperWidthParam).InputImg,
                        out HObject ho_Regions11, (toolParam as GlueCaliperWidthParam).CaliperHeight,
                        hv_Row1, hv_Column1, hv_Row2, hv_Column2);
                    HOperatorSet.ConcatObj(searchRegion, ho_Regions11, out searchRegion);

                    //结果区域
                    rake2((toolParam as GlueCaliperWidthParam).InputImg, out HObject ho_Regions,
                        (toolParam as GlueCaliperWidthParam).CaliperHeight, 1.0,
                        (toolParam as GlueCaliperWidthParam).CaliperEdgeThd,
                         "all", "all",
                        hv_Row1, hv_Column1,
                        hv_Row2, hv_Column2, out HTuple hv_ResultRow1, out HTuple hv_ResultColumn1, out HTuple hv_Distance);

                    HOperatorSet.ConcatObj(ho_EmptyObject, ho_Regions, out ho_EmptyObject);

                    if (hv_Distance == null || hv_Distance.TupleLength() <= 0)
                    {
                       pdFlag = pdFlag && false;
                        result.errInfo = string.Format(toolName+"距离{0}识别失败", i);//运行状态
                        (toolParam as GlueCaliperWidthParam).DistanceList.Add(0);
                        (toolParam as GlueCaliperWidthParam).DistanceList.Add(0);
                        (toolParam as GlueCaliperWidthParam).DistanceList.Add(0);
                        continue;
                    }

                    (toolParam as GlueCaliperWidthParam).DistanceList.Add(
                        hv_ResultRow1.TupleSelect(1).D);//行坐标
                    (toolParam as GlueCaliperWidthParam).DistanceList.Add(
                        hv_ResultColumn1.TupleSelect(1).D);//列坐标
                    (toolParam as GlueCaliperWidthParam).DistanceList.Add(
                        hv_Distance.D);//距离

                    if (hv_Distance.D * (toolParam as GlueCaliperWidthParam).PixleRatio >=
                        (toolParam as GlueCaliperWidthParam).DistanceMin &&
                        hv_Distance.D * (toolParam as GlueCaliperWidthParam).PixleRatio <=
                        (toolParam as GlueCaliperWidthParam).DistanceMax)
                       pdFlag = pdFlag && true;
                    else
                        pdFlag =pdFlag && false;
                }
              
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, pdFlag);
                else
                    dm.resultFlagDic[toolName] = pdFlag;

                (toolParam as GlueCaliperWidthParam).ResultInspectRegions = searchRegion.Clone();
                searchRegion.Dispose();

                //*结果显示
                if (!dm.resultBufDic.ContainsKey(toolName))
                    dm.resultBufDic.Add(toolName, ho_EmptyObject);
                else
                    dm.resultBufDic[toolName] = ho_EmptyObject.Clone();

                string info = string.Format(toolName+"胶宽检测完成");
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;

                //+输入图像
                HOperatorSet.ConcatObj((toolParam as GlueCaliperWidthParam).InputImg, ho_EmptyObject, out HObject objectsConcat2);
                (toolParam as GlueCaliperWidthParam).OutputImg = objectsConcat2;

                result.runFlag = true;
                (toolParam as GlueCaliperWidthParam).GlueWidthRunStatus = true;
            }
            catch (Exception er)
            {
                string info = string.Format(toolName+"胶宽检测异常:{0}", er.Message);
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
                (toolParam as GlueCaliperWidthParam).GlueWidthRunStatus = false;
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
        // External procedures 
        static private void get_rake2_region(HObject ho_Image, out HObject ho_Regions, HTuple hv_DetectHeight,
            HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionLines, ho_Rectangle;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_ATan = null;
            HTuple hv_RowC = null, hv_ColC = null, hv_Distance = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            //获取图像尺寸
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            //产生一个空显示对象，用于显示
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);

            //产生直线xld
            ho_RegionLines.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_RegionLines, hv_Row1.TupleConcat(hv_Row2),
                hv_Column1.TupleConcat(hv_Column2));
            //存储到显示对象
            //concat_obj (Regions, RegionLines, Regions)
            //计算直线与x轴的夹角，逆时针方向为正向。
            HOperatorSet.AngleLx(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_ATan);

            //边缘检测方向：直线方向为边缘检测方向
            hv_ATan = hv_ATan + ((new HTuple(0)).TupleRad());

            //根据检测直线按顺序产生测量区域矩形，并存储到显示对象
            hv_RowC = (hv_Row1 + hv_Row2) * 0.5;
            hv_ColC = (hv_Column1 + hv_Column2) * 0.5;
            //判断是否超出图像,超出不检测边缘
            if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
            {
                ho_RegionLines.Dispose();
                ho_Rectangle.Dispose();

                return;
            }
            HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Distance);
            ho_Rectangle.Dispose();
            HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC, hv_ATan,
                hv_Distance / 2, hv_DetectHeight / 2);


            //把测量矩形xld存储到显示对象
            {
                HObject ExpTmpOutVar_0;
                HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle, out ExpTmpOutVar_0);
                ho_Regions.Dispose();
                ho_Regions = ExpTmpOutVar_0;
            }

            //在第一个测量矩形绘制一个箭头xld，用于只是边缘检测方向
            //RowL2 := RowC+Distance/2*sin(-ATan)
            //RowL1 := RowC-Distance/2*sin(-ATan)
            //ColL2 := ColC+Distance/2*cos(-ATan)
            //ColL1 := ColC-Distance/2*cos(-ATan)
            //gen_arrow_contour_xld (Arrow1, RowL1, ColL1, RowL2, ColL2, DetectHeight/2, DetectHeight/2)
            //把xld存储到显示对象
            //concat_obj (Regions, Arrow1, Regions)

            ho_RegionLines.Dispose();
            ho_Rectangle.Dispose();

            return;
        }

        // Chapter: XLD / Creation
        // Short Description: Creates an arrow shaped XLD contour. 
        static private void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
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

        // Chapter: Graphics / Text
        // Short Description: This procedure writes a text message. 
       
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

        static private void pts_to_best_line(out HObject ho_Line, HTuple hv_Rows, HTuple hv_Cols,
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

        static private void rakePoint(HObject ho_Image, out HObject ho_Regions, HTuple hv_Sigma,
            HTuple hv_Threshold, HTuple hv_Transition, HTuple hv_Select, HTuple hv_row1,
            HTuple hv_col1, HTuple hv_row2, HTuple hv_col2, HTuple hv_DetectHeight, out HTuple hv_ResultRow,
            out HTuple hv_ResultColumn)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionLines, ho_Cross = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_ATan = null;
            HTuple hv_Distance = null, hv_DetectWidth = null, hv_RowC = null;
            HTuple hv_ColC = null, hv_MsrHandle_Measure = null, hv_RowEdge = null;
            HTuple hv_ColEdge = null, hv_Amplitude = null, hv_tRow = null;
            HTuple hv_tCol = null, hv_t = null, hv_Number = null, hv_j = null;
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
            HOperatorSet.GenContourPolygonXld(out ho_RegionLines, hv_row1.TupleConcat(hv_row2),
                hv_col1.TupleConcat(hv_col2));
            //存储到显示对象
            //concat_obj (Regions, RegionLines, Regions)
            //计算直线与x轴的夹角，逆时针方向为正向。
            HOperatorSet.AngleLx(hv_row1, hv_col1, hv_row2, hv_col2, out hv_ATan);

            HOperatorSet.DistancePp(hv_row1, hv_col1, hv_row2, hv_col2, out hv_Distance);
            hv_DetectWidth = hv_Distance.Clone();
            //边缘检测方向平行于于检测直线：直线方向°为边缘检测方向
            hv_ATan = hv_ATan + ((new HTuple(0)).TupleRad());

            ////中心点
            hv_RowC = (hv_row1 + hv_row2) / 2;
            hv_ColC = (hv_col1 + hv_col2) / 2;


            //产生测量对象句柄
            HOperatorSet.GenMeasureRectangle2(hv_RowC, hv_ColC, hv_ATan, hv_DetectWidth / 2,
                hv_DetectHeight / 2, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);

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
                ho_RegionLines.Dispose();
                ho_Cross.Dispose();

                return;
            }
            //有多个边缘时，选择幅度绝对值最大的边缘
            HTuple end_val66 = hv_Number - 1;
            HTuple step_val66 = 1;
            for (hv_j = 0; hv_j.Continue(end_val66, step_val66); hv_j = hv_j.TupleAdd(step_val66))
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
            ////结果
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

        // Local procedures 
        static private void rakeex(HObject ho_Image, out HObject ho_Regions, HTuple hv_Sigma,
           HTuple hv_Threshold, HTuple hv_row1,
            HTuple hv_col1, HTuple hv_row2, HTuple hv_col2, HTuple hv_DetectHeight, out HTuple hv_ResultRow,
            out HTuple hv_ResultColumn)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionLines, ho_Rectangle, ho_Contour;
            HObject ho_Contour1, ho_Cross = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_ATan = null;
            HTuple hv_Distance = null, hv_DetectWidth = null, hv_RowC = null;
            HTuple hv_ColC = null, hv_Row = null, hv_Col = null, hv_DistanceMin = null;
            HTuple hv_DistanceMin1 = null, hv_rc1 = null, hv_cc1 = null;
            HTuple hv_rc2 = null, hv_cc2 = null, hv_Distance1 = null;
            HTuple hv_numP = null, hv_i = null, hv_RC = new HTuple();
            HTuple hv_CC = new HTuple(), hv_MsrHandle_Measure = new HTuple();
            HTuple hv_RowEdge = new HTuple(), hv_ColEdge = new HTuple();
            HTuple hv_Amplitude = new HTuple(), hv_tRow = new HTuple();
            HTuple hv_tCol = new HTuple(), hv_t = new HTuple(), hv_Number = new HTuple();
            HTuple hv_j = new HTuple();
       
         

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_Contour1);
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
            HOperatorSet.GenContourPolygonXld(out ho_RegionLines, hv_row1.TupleConcat(hv_row2),
                hv_col1.TupleConcat(hv_col2));
            //存储到显示对象
            //concat_obj (Regions, RegionLines, Regions)
            //计算直线与x轴的夹角，逆时针方向为正向。
            HOperatorSet.AngleLx(hv_row1, hv_col1, hv_row2, hv_col2, out hv_ATan);

            HOperatorSet.DistancePp(hv_row1, hv_col1, hv_row2, hv_col2, out hv_Distance);
            hv_DetectWidth = hv_Distance.Clone();
            //边缘检测方向平行于于检测直线：直线方向°为边缘检测方向
            hv_ATan = hv_ATan + ((new HTuple(0)).TupleRad());

            ////中心点
            hv_RowC = (hv_row1 + hv_row2) / 2;
            hv_ColC = (hv_col1 + hv_col2) / 2;
            ho_Rectangle.Dispose();
            HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC, hv_ATan,
                hv_DetectWidth / 2, hv_DetectHeight / 2);
            HOperatorSet.GetContourXld(ho_Rectangle, out hv_Row, out hv_Col);
            ////排序
            ho_Contour.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour, ((hv_Row.TupleSelect(0))).TupleConcat(
                hv_Row.TupleSelect(1)), ((hv_Col.TupleSelect(0))).TupleConcat(hv_Col.TupleSelect(
                1)));
            HOperatorSet.DistanceCcMin(ho_Contour, ho_RegionLines, "point_to_segment", out hv_DistanceMin);
            ho_Contour1.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour1, ((hv_Row.TupleSelect(1))).TupleConcat(
                hv_Row.TupleSelect(2)), ((hv_Col.TupleSelect(1))).TupleConcat(hv_Col.TupleSelect(
                2)));
            HOperatorSet.DistanceCcMin(ho_Contour1, ho_RegionLines, "point_to_segment", out hv_DistanceMin1);
            hv_rc1 = 0;
            hv_cc1 = 0;
            hv_rc2 = 0;
            hv_cc2 = 0;
            if ((int)(new HTuple(hv_DistanceMin.TupleGreater(hv_DistanceMin1))) != 0)
            {
                hv_rc1 = ((hv_Row.TupleSelect(0)) + (hv_Row.TupleSelect(1))) / 2.0;
                hv_cc1 = ((hv_Col.TupleSelect(0)) + (hv_Col.TupleSelect(1))) / 2.0;
                hv_rc2 = ((hv_Row.TupleSelect(2)) + (hv_Row.TupleSelect(3))) / 2.0;
                hv_cc2 = ((hv_Col.TupleSelect(2)) + (hv_Col.TupleSelect(3))) / 2.0;
            }
            else
            {
                hv_rc1 = ((hv_Row.TupleSelect(1)) + (hv_Row.TupleSelect(2))) / 2.0;
                hv_cc1 = ((hv_Col.TupleSelect(1)) + (hv_Col.TupleSelect(2))) / 2.0;
                hv_rc2 = ((hv_Row.TupleSelect(3)) + (hv_Row.TupleSelect(4))) / 2.0;
                hv_cc2 = ((hv_Col.TupleSelect(3)) + (hv_Col.TupleSelect(4))) / 2.0;
            }

            HOperatorSet.DistancePp(hv_rc1, hv_cc1, hv_rc2, hv_cc2, out hv_Distance1);


            //根据检测直线按顺序产生测量区域矩形，并存储到显示对象

            hv_numP = hv_DetectHeight / 10;
            if ((int)(new HTuple(hv_numP.TupleLess(2))) != 0)
            {
                hv_numP = 2;
            }


            HTuple end_val56 = hv_numP;
            HTuple step_val56 = 1;
            for (hv_i = 1; hv_i.Continue(end_val56, step_val56); hv_i = hv_i.TupleAdd(step_val56))
            {

                //如果有多个测量矩形，产生该测量矩形xld
                hv_RC = hv_rc1 + (((hv_rc2 - hv_rc1) * (hv_i - 1)) / (hv_numP - 1));
                hv_CC = hv_cc1 + (((hv_cc2 - hv_cc1) * (hv_i - 1)) / (hv_numP - 1));
                //判断是否超出图像,超出不检测边缘
                if ((int)((new HTuple((new HTuple((new HTuple(hv_RC.TupleGreater(hv_Height - 1))).TupleOr(
                    new HTuple(hv_RC.TupleLess(0))))).TupleOr(new HTuple(hv_CC.TupleGreater(
                    hv_Width - 1))))).TupleOr(new HTuple(hv_CC.TupleLess(0)))) != 0)
                {
                    continue;
                }
                //gen_rectangle2 (Rectangle1, RC, CC, ATan, DetectWidth/2, 5)
                //产生测量对象句柄
                HOperatorSet.GenMeasureRectangle2(hv_RC, hv_CC, hv_ATan, hv_DetectWidth / 2,
                    5, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);

                
                      
                //检测边缘
                HOperatorSet.MeasurePos(ho_Image, hv_MsrHandle_Measure, hv_Sigma, hv_Threshold,
                    "all", "all", out hv_RowEdge, out hv_ColEdge,
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
                HTuple end_val108 = hv_Number - 1;
                HTuple step_val108 = 1;
                for (hv_j = 0; hv_j.Continue(end_val108, step_val108); hv_j = hv_j.TupleAdd(step_val108))
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
            ho_Rectangle.Dispose();
            ho_Contour.Dispose();
            ho_Contour1.Dispose();
            ho_Cross.Dispose();

            return;
        }

        static void rake2(HObject ho_Image, out HObject ho_Regions, HTuple hv_DetectHeight,
     HTuple hv_Sigma, HTuple hv_Threshold, HTuple hv_Transition, HTuple hv_Select,
     HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2, out HTuple hv_ResultRow,
     out HTuple hv_ResultColumn, out HTuple hv_Distance)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionLines, ho_Contour = null, ho_Arrow1 = null;

            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_ATan = null;
            HTuple hv_RowC = null, hv_ColC = null, hv_Distance2 = null;
            HTuple hv_MsrHandle_Measure = null, hv_RowEdgeFirst = null;
            HTuple hv_ColumnEdgeFirst = null, hv_AmplitudeFirst = null;
            HTuple hv_RowEdgeSecond = null, hv_ColumnEdgeSecond = null;
            HTuple hv_AmplitudeSecond = null, hv_IntraDistance = null;
            HTuple hv_InterDistance = null, hv_tRow = null, hv_tCol = null;
            HTuple hv_t = null, hv_Number = null, hv_j = null, hv_TemATan = null;
            HTuple hv_Sin = null, hv_Cos = null, hv_Index = null, hv_R1 = new HTuple();
            HTuple hv_C1 = new HTuple(), hv_R2 = new HTuple(), hv_C2 = new HTuple();
            HTuple hv_Select_COPY_INP_TMP = hv_Select.Clone();
            HTuple hv_Transition_COPY_INP_TMP = hv_Transition.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            //获取图像尺寸
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            //产生一个空显示对象，用于显示
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);
            //初始化边缘坐标数组
            hv_ResultRow = new HTuple();
            hv_ResultColumn = new HTuple();
            hv_Distance = new HTuple();
            //产生直线xld
            ho_RegionLines.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_RegionLines, hv_Row1.TupleConcat(hv_Row2),
                hv_Column1.TupleConcat(hv_Column2));
            //存储到显示对象
            //concat_obj (Regions, RegionLines, Regions)
            //计算直线与x轴的夹角，逆时针方向为正向。
            HOperatorSet.AngleLx(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_ATan);

            //边缘检测方向：直线方向为边缘检测方向
            hv_ATan = hv_ATan + ((new HTuple(0)).TupleRad());

            //根据检测直线按顺序产生测量区域矩形，并存储到显示对象


            hv_RowC = (hv_Row1 + hv_Row2) * 0.5;
            hv_ColC = (hv_Column1 + hv_Column2) * 0.5;
            //判断是否超出图像,超出不检测边缘
            if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
            {
                ho_RegionLines.Dispose();
                ho_Contour.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Distance2);

            //产生测量对象句柄
            HOperatorSet.GenMeasureRectangle2(hv_RowC, hv_ColC, hv_ATan, hv_Distance2 / 2,
                hv_DetectHeight / 2, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);

            //设置极性
            if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("negative"))) != 0)
            {
                hv_Transition_COPY_INP_TMP = "negative";
            }
            else if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("negative_strongest"))) != 0)
            {
                hv_Transition_COPY_INP_TMP = "negative_strongest";
            }
            else if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("positive"))) != 0)
            {
                hv_Transition_COPY_INP_TMP = "positive";
            }
            else if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("positive_strongest"))) != 0)
            {
                hv_Transition_COPY_INP_TMP = "positive_strongest";
            }
            else if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("all_strongest"))) != 0)
            {
                hv_Transition_COPY_INP_TMP = "all_strongest";
            }
            else
            {
                hv_Transition_COPY_INP_TMP = "all";
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
            //检测边缘对
            HOperatorSet.MeasurePairs(ho_Image, hv_MsrHandle_Measure, hv_Sigma, hv_Threshold,
                hv_Transition_COPY_INP_TMP, hv_Select_COPY_INP_TMP, out hv_RowEdgeFirst,
                out hv_ColumnEdgeFirst, out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);

            //清除测量对象句柄
            HOperatorSet.CloseMeasure(hv_MsrHandle_Measure);
            /////---------第一边缘
            //临时变量初始化
            //tRow，tCol保存找到指定边缘的坐标
            hv_tRow = 0;
            hv_tCol = 0;
            //t保存边缘的幅度绝对值
            hv_t = 0;
            //找到的边缘必须至少为1个
            HOperatorSet.TupleLength(hv_RowEdgeFirst, out hv_Number);
            if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
            {
                ho_RegionLines.Dispose();
                ho_Contour.Dispose();
                ho_Arrow1.Dispose();

                return;
            }
            //有多个边缘对时，选择幅度绝对值最大的边缘对
            HTuple end_val75 = hv_Number - 1;
            HTuple step_val75 = 1;
            for (hv_j = 0; hv_j.Continue(end_val75, step_val75); hv_j = hv_j.TupleAdd(step_val75))
            {
                if ((int)(new HTuple(((((hv_AmplitudeFirst.TupleSelect(hv_j))).TupleAbs())).TupleGreater(
                    hv_t))) != 0)
                {

                    hv_tRow = hv_RowEdgeFirst.TupleSelect(hv_j);
                    hv_tCol = hv_ColumnEdgeFirst.TupleSelect(hv_j);
                    hv_t = ((hv_AmplitudeFirst.TupleSelect(hv_j))).TupleAbs();
                }
            }
            //把找到的边缘保存在输出数组
            if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
            {
                hv_ResultRow = hv_ResultRow.TupleConcat(hv_tRow);
                hv_ResultColumn = hv_ResultColumn.TupleConcat(hv_tCol);
            }
            /////---------第二边缘
            hv_tRow = 0;
            hv_tCol = 0;
            hv_t = 0;
            //有多个边缘对时，选择幅度绝对值最大的边缘对
            HTuple end_val93 = hv_Number - 1;
            HTuple step_val93 = 1;
            for (hv_j = 0; hv_j.Continue(end_val93, step_val93); hv_j = hv_j.TupleAdd(step_val93))
            {
                if ((int)(new HTuple(((((hv_AmplitudeSecond.TupleSelect(hv_j))).TupleAbs())).TupleGreater(
                    hv_t))) != 0)
                {

                    hv_tRow = hv_RowEdgeSecond.TupleSelect(hv_j);
                    hv_tCol = hv_ColumnEdgeSecond.TupleSelect(hv_j);
                    hv_t = ((hv_AmplitudeSecond.TupleSelect(hv_j))).TupleAbs();
                }
            }
            //把找到的边缘保存在输出数组
            if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
            {
                hv_ResultRow = hv_ResultRow.TupleConcat(hv_tRow);
                hv_ResultColumn = hv_ResultColumn.TupleConcat(hv_tCol);
            }

            ////-------结果汇总
            hv_TemATan = hv_ATan + ((new HTuple(90)).TupleRad());
            HOperatorSet.TupleSin(hv_TemATan, out hv_Sin);
            HOperatorSet.TupleCos(hv_TemATan, out hv_Cos);
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ResultRow.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {

                //gen_rectangle2_contour_xld (Rectangle, RowC, ColC, ATan, Distance/2, DetectHeight/2)
                hv_R1 = ((2 * (hv_ResultRow.TupleSelect(hv_Index))) - (hv_DetectHeight * hv_Sin)) / 2;
                hv_C1 = ((2 * (hv_ResultColumn.TupleSelect(hv_Index))) + (hv_DetectHeight * hv_Cos)) / 2;
                hv_R2 = hv_R1 + (hv_DetectHeight * hv_Sin);
                hv_C2 = hv_C1 - (hv_DetectHeight * hv_Cos);
                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_R1.TupleConcat(hv_R2),
                    hv_C1.TupleConcat(hv_C2));
                //gen_cross_contour_xld (Cross, ResultRow, ResultColumn, 4, 0.785398)
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Regions, ho_Contour, out ExpTmpOutVar_0);
                    ho_Regions.Dispose();
                    ho_Regions = ExpTmpOutVar_0;
                }

            }

            if ((int)(new HTuple((new HTuple(hv_ResultRow.TupleLength())).TupleGreaterEqual(
                2))) != 0)
            {
                HOperatorSet.DistancePp(hv_ResultRow.TupleSelect(0), hv_ResultColumn.TupleSelect(
                    0), hv_ResultRow.TupleSelect(1), hv_ResultColumn.TupleSelect(1), out hv_Distance);

                //在第一个测量矩形绘制一个箭头xld，用于只是边缘检测方向
                //RowL2 := RowC+Distance/2*sin(-ATan)
                //RowL1 := RowC-Distance/2*sin(-ATan)
                //ColL2 := ColC+Distance/2*cos(-ATan)
                //ColL1 := ColC-Distance/2*cos(-ATan)
                HTuple temValue = hv_Distance.D > hv_DetectHeight.D ? hv_DetectHeight.D : hv_Distance.D;
                ho_Arrow1.Dispose();
                gen_arrow_contour_xld(out ho_Arrow1, hv_ResultRow.TupleSelect(0), hv_ResultColumn.TupleSelect(
                    0), hv_ResultRow.TupleSelect(1), hv_ResultColumn.TupleSelect(1), temValue / 4,
                    temValue / 4);
                //把xld存储到显示对象
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                    ho_Regions.Dispose();
                    ho_Regions = ExpTmpOutVar_0;
                }

            }

            ho_RegionLines.Dispose();
            ho_Contour.Dispose();
            ho_Arrow1.Dispose();

            return;
        }


        #endregion
    }
}
