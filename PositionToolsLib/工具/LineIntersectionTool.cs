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
    /// 直线交点工具
    /// </summary>
    [Serializable]
    public  class LineIntersectionTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public LineIntersectionTool()
        {
            toolParam = new LineIntersectionParam();
            toolName = "直线交点" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("直线交点");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("直线交点", ""));
            if (number > inum)
                inum = number;
            //toolName = "直线交点" + number;
            inum++;
        }
        /// <summary>
        /// 直线交点工具运行
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

                (toolParam as LineIntersectionParam).IntersectionRow = 0;
                (toolParam as LineIntersectionParam).IntersectionColumn = 0;


                (toolParam as LineIntersectionParam).InputImg = dm.imageBufDic[(toolParam as LineIntersectionParam).InputImageName];
                if (!ObjectValided((toolParam as LineIntersectionParam).InputImg))
                {
                    (toolParam as LineIntersectionParam).LineIntersectionRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                //检测
              
                if (!dm.resultFlagDic[(toolParam as LineIntersectionParam).InputLineName])
                {
                    (toolParam as LineIntersectionParam).LineIntersectionRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入直线1无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                if (!dm.resultFlagDic[(toolParam as LineIntersectionParam).InputLine2Name])
                {
                    (toolParam as LineIntersectionParam).LineIntersectionRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入直线2无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                StuLineData line1 = dm.LineDataDic[(toolParam as LineIntersectionParam).InputLineName];
                StuLineData line2 = dm.LineDataDic[(toolParam as LineIntersectionParam).InputLine2Name];
                //StuLineData line1 = (toolParam as LineIntersectionParam).LineData;
                //StuLineData line2 = (toolParam as LineIntersectionParam).LineData2;
                HOperatorSet.IntersectionLl(line1.spRow, line1.spColumn, line1.epRow, line1.epColumn,
                    line2.spRow, line2.spColumn, line2.epRow, line2.epColumn,out HTuple row,out HTuple column,
                    out HTuple  isParallel);

                ExtendLine(line1.spRow, line1.spColumn, line1.epRow, line1.epColumn,100,
                    out HTuple row1,out HTuple colomn1,out HTuple row2,out HTuple column2);

                HOperatorSet.GenContourPolygonXld(out HObject lineContour1, row1.TupleConcat(row2),
                      colomn1.TupleConcat(column2));

                ExtendLine(line2.spRow, line2.spColumn, line2.epRow, line2.epColumn, 100,
                  out HTuple row21, out HTuple colomn21, out HTuple row22, out HTuple column22);

                HOperatorSet.GenContourPolygonXld(out HObject lineContour2, row21.TupleConcat(row22),
                      colomn21.TupleConcat(column22));

             
                HOperatorSet.ConcatObj(lineContour1, lineContour2, out HObject emptyRegionBuf);


                if (row.TupleLength() > 0)
                {
                    HOperatorSet.GenCrossContourXld(out HObject cross, row.D, column.D,20,0);
                    if (ObjectValided(cross))
                        HOperatorSet.ConcatObj(emptyRegionBuf, cross, out  emptyRegionBuf);
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
                    dm.resultFlagDic.Add(toolName, row.TupleLength() > 0);
                else
                    dm.resultFlagDic[toolName] = row.TupleLength() > 0;


                if (row.TupleLength() > 0)
                {
                    if (!dm.PositionDataDic.ContainsKey(toolName))
                        dm.PositionDataDic.Add(toolName, new StuCoordinateData(column.D, row.D,
                            0));
                    else
                        dm.PositionDataDic[toolName] = new StuCoordinateData(column.D, row.D,
                            0);
                    (toolParam as LineIntersectionParam).IntersectionRow = row.D;
                    (toolParam as LineIntersectionParam).IntersectionColumn = column.D;
                }
              

                //+输入图像

                HOperatorSet.ConcatObj((toolParam as LineIntersectionParam).InputImg, emptyRegionBuf, out HObject objectsConcat2);
                (toolParam as LineIntersectionParam).OutputImg = objectsConcat2;
                (toolParam as LineIntersectionParam).Resultline1 = lineContour1.Clone();
                (toolParam as LineIntersectionParam).Resultline2 = lineContour2.Clone();

            
                result.runFlag = true;
                (toolParam as LineIntersectionParam).LineIntersectionRunStatus = true;
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
                (toolParam as LineIntersectionParam).LineIntersectionRunStatus = false;
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
