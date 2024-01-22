using System;
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
    /// 平行直线工具
    /// </summary>
    [Serializable]
    public  class CalParallelLineTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public CalParallelLineTool()
        {
            toolParam = new CalParallelLineParam();
            toolName = "平行直线" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("平行直线");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("平行直线", ""));
            if (number > inum)
                inum = number;
            //toolName = "平行直线" + number;
            inum++;
        }
        /// <summary>
        /// 平行直线工具运行
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
                if (!dm.PositionDataDic.ContainsKey(toolName))
                    dm.PositionDataDic.Add(toolName, new StuCoordinateData(0,
                       0, 0));
                else
                    dm.PositionDataDic[toolName] = new StuCoordinateData(0,
                        0, 0);


                (toolParam as CalParallelLineParam).InputImg = dm.imageBufDic[(toolParam as CalParallelLineParam).InputImageName];
                if (!ObjectValided((toolParam as CalParallelLineParam).InputImg))
                {
                    (toolParam as CalParallelLineParam).CalParallelLineRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                //检测
              
                if (!dm.resultFlagDic[(toolParam as CalParallelLineParam).InputLineName])
                {
                    (toolParam as CalParallelLineParam).CalParallelLineRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入直线1无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                if (!dm.resultFlagDic[(toolParam as CalParallelLineParam).InputLine2Name])
                {
                    (toolParam as CalParallelLineParam).CalParallelLineRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入直线2无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                StuLineData line1 = dm.LineDataDic[(toolParam as CalParallelLineParam).InputLineName];
                StuLineData line2 = dm.LineDataDic[(toolParam as CalParallelLineParam).InputLine2Name];
                //StuLineData line1 = (toolParam as LineIntersectionParam).LineData;
                //StuLineData line2 = (toolParam as LineIntersectionParam).LineData2;

                ExpandFunction.CalParallelLine(line1.spRow, line1.spColumn, line1.epRow, line1.epColumn,
                    line2.spRow, line2.spColumn, line2.epRow, line2.epColumn,
                    out HTuple parallenlRow1, out HTuple parallenlCol1,
                    out HTuple parallenlRow2, out HTuple parallenlCol2);


                //HOperatorSet.IntersectionLl(line1.spRow, line1.spColumn, line1.epRow, line1.epColumn,
                //    line2.spRow, line2.spColumn, line2.epRow, line2.epColumn,out HTuple row,out HTuple column,
                //    out HTuple  isParallel);

                ExtendLine(line1.spRow, line1.spColumn, line1.epRow, line1.epColumn, 100,
                    out HTuple row1, out HTuple colomn1, out HTuple row2, out HTuple column2);

                HOperatorSet.GenContourPolygonXld(out HObject lineContour1, row1.TupleConcat(row2),
                    colomn1.TupleConcat(column2));

                ExtendLine(line2.spRow, line2.spColumn, line2.epRow, line2.epColumn, 100,
                  out HTuple row21, out HTuple colomn21, out HTuple row22, out HTuple column22);

                HOperatorSet.GenContourPolygonXld(out HObject lineContour2, row21.TupleConcat(row22),
                      colomn21.TupleConcat(column22));

             
                HOperatorSet.ConcatObj(lineContour1, lineContour2, out HObject emptyRegionBuf);

                HObject lineContour3 = null;
                HOperatorSet.GenEmptyObj(out lineContour3);

             
                if (parallenlRow1.TupleLength() > 0)
                {
          
                    HOperatorSet.GenContourPolygonXld(out lineContour3, parallenlRow1.TupleConcat(parallenlRow2),
                     parallenlCol1.TupleConcat(parallenlCol2));

                    if (ObjectValided(lineContour3))
                        HOperatorSet.ConcatObj(emptyRegionBuf, lineContour3, out emptyRegionBuf);
                   
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
                    dm.resultFlagDic.Add(toolName, parallenlRow1.TupleLength() > 0);
                else
                    dm.resultFlagDic[toolName] = parallenlRow1.TupleLength() > 0;

                //计算物理坐标系下的角度
                HTuple Rx, Ry, Rx2, Ry2, Angle = 0;
                if (parallenlRow1.TupleLength() > 0)
                {
                    bool transFlag = Transformation_POINT(parallenlCol1, parallenlRow1, out Rx, out Ry);
                    bool transFlag2 = Transformation_POINT(parallenlCol2, parallenlRow2, out Rx2, out Ry2);
                    //角度
                    if (transFlag && transFlag2)
                        Angle = calAngleOfLx(Rx, Ry, Rx2, Ry2);
                    else
                    {
                        HOperatorSet.AngleLx(parallenlRow1, parallenlCol1, parallenlRow2, parallenlCol2, out HTuple angle);
                        Angle = angle.TupleDeg().D;
                    }
                                      
                    (toolParam as CalParallelLineParam).ParallelLineAngle = Angle.D;

                    if (!dm.PositionDataDic.ContainsKey(toolName))
                        dm.PositionDataDic.Add(toolName, new StuCoordinateData(
                            parallenlCol1.D, parallenlRow1.D, Angle.D));
                    else
                        dm.PositionDataDic[toolName] = new StuCoordinateData(
                            parallenlCol1.D, parallenlRow1.D, Angle.D);
                    //在添加
                    if (!dm.LineDataDic.ContainsKey(toolName))
                        dm.LineDataDic.Add(toolName, new StuLineData(parallenlRow1.D, parallenlCol1.D,
                            parallenlRow2.D, parallenlCol2.D));
                    else
                        dm.LineDataDic[toolName] = new StuLineData(parallenlRow1.D, parallenlCol1.D,
                            parallenlRow2.D, parallenlCol2.D);

                    (toolParam as CalParallelLineParam).ParallelLineRow1 = parallenlRow1.D;
                    (toolParam as CalParallelLineParam).ParallelLineColumn1 = parallenlCol1.D;
                    (toolParam as CalParallelLineParam).ParallelLineRow2 = parallenlRow2.D;
                    (toolParam as CalParallelLineParam).ParallelLineColumn2 = parallenlCol2.D;
                }
                else
                {
                  
                    //在添加
                    if (!dm.LineDataDic.ContainsKey(toolName))
                        dm.LineDataDic.Add(toolName, new StuLineData(0, 0,
                            0, 0));
                    else
                        dm.LineDataDic[toolName] = new StuLineData(0, 0,
                           0, 0);

                    (toolParam as CalParallelLineParam).ParallelLineRow1 = 0;
                    (toolParam as CalParallelLineParam).ParallelLineColumn1 = 0;
                    (toolParam as CalParallelLineParam).ParallelLineRow2 = 0;
                    (toolParam as CalParallelLineParam).ParallelLineColumn2 = 0;
                    (toolParam as CalParallelLineParam).ParallelLineAngle = 0;
                }

                //+输入图像

                HOperatorSet.ConcatObj((toolParam as CalParallelLineParam).InputImg, emptyRegionBuf, out HObject objectsConcat2);
                (toolParam as CalParallelLineParam).OutputImg = objectsConcat2;
                (toolParam as CalParallelLineParam).Resultline1 = lineContour1.Clone();
                (toolParam as CalParallelLineParam).Resultline2 = lineContour2.Clone();
                (toolParam as CalParallelLineParam).ResultParallelLine = lineContour3.Clone();

                //HOperatorSet.AngleLx(parallenlRow1, parallenlCol1, parallenlRow2, parallenlCol2, out HTuple angle);
                //(toolParam as CalParallelLineParam).ParallelLineAngle = angle.TupleDeg().D;

                result.runFlag = true;
                (toolParam as CalParallelLineParam).CalParallelLineRunStatus = true;
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
                (toolParam as CalParallelLineParam).CalParallelLineRunStatus = false;
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
        #endregion

    }
}
