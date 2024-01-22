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
    /// 直线偏移工具
    /// </summary>
    [Serializable]
   public class LineOffsetTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public LineOffsetTool()
        {
            toolParam = new LineOffsetParam();
            toolName = "直线偏移" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("直线偏移");
        public EumSizeUnits sizeUnits = EumSizeUnits.pixel;//尺寸单位

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("直线偏移", ""));
            if (number > inum)
                inum = number;
         
            inum++;
        }
        /// <summary>
        /// 直线偏移工具运行
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
                //坐标点位1
                if (!dm.PositionDataDic.ContainsKey(toolName + "起点"))
                    dm.PositionDataDic.Add(toolName + "起点", new StuCoordinateData(0,
                       0, 0));
                else
                    dm.PositionDataDic[toolName + "起点"] = new StuCoordinateData(0,
                        0, 0);
                //坐标点位2
                if (!dm.PositionDataDic.ContainsKey(toolName + "终点"))
                    dm.PositionDataDic.Add(toolName + "终点", new StuCoordinateData(0, 0, 0));
                else
                    dm.PositionDataDic[toolName + "终点"] = new StuCoordinateData(0, 0, 0);

                (toolParam as LineOffsetParam).Row1 = 0;
                (toolParam as LineOffsetParam).Col1 = 0;
                (toolParam as LineOffsetParam).Row2 = 0;
                (toolParam as LineOffsetParam).Col2 = 0;


                (toolParam as LineOffsetParam).InputImg = dm.imageBufDic[(toolParam as LineOffsetParam).InputImageName];
                if (!ObjectValided((toolParam as LineOffsetParam).InputImg))
                {
                    (toolParam as LineOffsetParam).LineOffsetRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }

            
                //检测
                if (!dm.resultFlagDic[(toolParam as LineOffsetParam).InputLineName])
                {
                    (toolParam as LineOffsetParam).LineOffsetRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入直线无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
           
                StuLineData line1 = dm.LineDataDic[(toolParam as LineOffsetParam).InputLineName];
                HOperatorSet.GenContourPolygonXld(out HObject line,
                    new HTuple(line1.spRow).TupleConcat(line1.epRow),
                     new HTuple(line1.spColumn).TupleConcat(line1.epColumn));
                //像素距离
                double distance = (toolParam as LineOffsetParam).OffsetDistance;
                //物理距离转像素距离
                if (this.sizeUnits == EumSizeUnits.Physical)
                    distance = distance / (toolParam as LineOffsetParam).PixleRatio;
                //直线偏移
                HOperatorSet.GenParallelContourXld(line,out HObject parallelLine, "regression_normal",
                    distance);

                HOperatorSet.CopyObj(parallelLine , out HObject emptyRegionBuf,1,1);
                HOperatorSet.FitLineContourXld(parallelLine, "regression", -1, 0, 5, 2,
                   out HTuple  RowBegin, out HTuple ColBegin, out HTuple RowEnd, out HTuple ColEnd,
                      out HTuple Nr, out HTuple Nc, out HTuple Dist);

                if (RowBegin.TupleLength() > 0)
                {
                    HOperatorSet.GenCrossContourXld(out HObject cross, RowBegin.D, ColBegin.D, 20, 0);
                    if (ObjectValided(cross))
                        HOperatorSet.ConcatObj(emptyRegionBuf, cross, out emptyRegionBuf);
                }

                if (!dm.resultBufDic.ContainsKey(toolName))
                    dm.resultBufDic.Add(toolName, emptyRegionBuf.Clone());
                else
                    dm.resultBufDic[toolName] = emptyRegionBuf.Clone();

                //测试信息
                string info = toolName + "检测完成";
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;

                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, RowBegin.TupleLength() > 0);
                else
                    dm.resultFlagDic[toolName] = RowBegin.TupleLength() > 0;

                ////计算物理坐标系下的角度
                //HTuple Rx, Ry, Rx2, Ry2, Angle = 0;
                //bool transFlag = Transformation_POINT(ColBegin, RowBegin, out Rx, out Ry);
                //bool transFlag2 = Transformation_POINT(ColEnd, RowEnd, out Rx2, out Ry2);
                ////角度
                //if (transFlag && transFlag2)
                //    Angle = calAngleOfLx(Rx, Ry, Rx2, Ry2);
                //else
                //{
                //    HOperatorSet.AngleLx(RowBegin, ColBegin, RowEnd, ColEnd, out HTuple angle);
                //    Angle = angle.TupleDeg().D;
                //}

                HTuple Angle = 0;
                HOperatorSet.AngleLx(RowBegin, ColBegin, RowEnd, ColEnd, out HTuple angle);
                Angle = angle.TupleDeg().D;
                (toolParam as LineOffsetParam).LineAngle = Angle.D;
               
                if (RowBegin.TupleLength() > 0)
                { 
                    //坐标点位1
                    if (!dm.PositionDataDic.ContainsKey(toolName+ "起点"))
                        dm.PositionDataDic.Add(toolName + "起点", new StuCoordinateData(ColBegin.D, RowBegin.D,
                            Angle.D));
                    else
                        dm.PositionDataDic[toolName + "起点"] = new StuCoordinateData(ColBegin.D, RowBegin.D,
                           Angle.D);
                    //坐标点位2
                    if (!dm.PositionDataDic.ContainsKey(toolName + "终点"))
                        dm.PositionDataDic.Add(toolName + "终点", new StuCoordinateData(ColEnd.D, RowEnd.D,
                            Angle.D));
                    else
                        dm.PositionDataDic[toolName + "终点"] = new StuCoordinateData(ColEnd.D, RowEnd.D,
                           Angle.D);

                    (toolParam as LineOffsetParam).Row1 = RowBegin.D;
                    (toolParam as LineOffsetParam).Col1 = ColBegin.D;
                    (toolParam as LineOffsetParam).Row2 = RowEnd.D;
                    (toolParam as LineOffsetParam).Col2 = ColEnd.D;
                  
                }
               
                //+输入图像

                HOperatorSet.ConcatObj((toolParam as LineOffsetParam).InputImg, emptyRegionBuf, out HObject objectsConcat2);
                (toolParam as LineOffsetParam).OutputImg = objectsConcat2;
                (toolParam as LineOffsetParam).Resultline = parallelLine.Clone();

                //在添加
                if (!dm.LineDataDic.ContainsKey(toolName))
                    dm.LineDataDic.Add(toolName, new StuLineData(RowBegin.D, ColBegin.D,
                            RowEnd.D, ColEnd.D));
                else
                    dm.LineDataDic[toolName] = new StuLineData(RowBegin.D, ColBegin.D,
                            RowEnd.D, ColEnd.D);



                result.runFlag = true;
                (toolParam as LineOffsetParam).LineOffsetRunStatus = true;
                emptyRegionBuf.Dispose();
            }
            catch (Exception er)
            {
                string info = string.Format(toolName + "检测异常:{0}", er.Message);
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
                (toolParam as LineOffsetParam).LineOffsetRunStatus = false;
                result.errInfo = er.Message;
                sw.Stop();
                result.runTime = sw.ElapsedMilliseconds;
                return result;
            }
            sw.Stop();
            result.runTime = sw.ElapsedMilliseconds;
            return result;
        }
    }
    /// <summary>
    /// 尺寸单位
    /// </summary>
    public enum EumSizeUnits
    {
        pixel,
        Physical
    }
}
