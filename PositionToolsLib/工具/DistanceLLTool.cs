using HalconDotNet;
using OSLog;
using PositionToolsLib.参数;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.工具
{ 
    /// <summary>
  /// 线线距离
  /// </summary>
    [Serializable]
    public class DistanceLLTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号
        //public string CalibFilePath = "";//标定文件路径
        public DistanceLLTool()
        {
            toolParam = new DistanceLLParam();
            toolName = "线线距离" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if ((toolParam as DistanceLLParam).Hv_CamParam == null)
            {
                (toolParam as DistanceLLParam).Hv_CamParam = new HTuple();
            }

        }
        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("线线距离");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("线线距离", ""));
            if (number > inum)
                inum = number;

            inum++;
        }

        /// <summary>
        /// 获取相机内参
        /// </summary>
        /// <param name="homMat2D"></param>
        override public void SetMatrix(HTuple homMat2D)
        {
            (toolParam as DistanceLLParam).Hv_CamParam = homMat2D;
        }

  
        /// <summary>
        /// 线线距离工具运行
        /// </summary>
        /// <returns></returns>
        override public RunResult Run()
        {
            DataManage dm = GetManage();
            if (!dm.enumerableTooDic.Contains(toolName))
                dm.enumerableTooDic.Add(toolName);
            if (!dm.sizeTooDic.Contains(toolName))
                dm.sizeTooDic.Add(toolName);
            RunResult result = new RunResult();
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (sw == null) sw = new System.Diagnostics.Stopwatch();
            sw.Restart();

            try
            {
                if (!dm.SizeDataDic.ContainsKey(toolName))
                    dm.SizeDataDic.Add(toolName, 0);
                else
                    dm.SizeDataDic[toolName] = 0;
                (toolParam as DistanceLLParam).Distance = 0;
                (toolParam as DistanceLLParam).InputImg = dm.imageBufDic[(toolParam as DistanceLLParam).InputImageName];
                if (!ObjectValided((toolParam as DistanceLLParam).InputImg))
                {
                    (toolParam as DistanceLLParam).DistanceLLRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                HTuple CamParam = (toolParam as DistanceLLParam).Hv_CamParam;
                if ((CamParam == null || CamParam.Length <= 0)&&
                    !(toolParam as DistancePLParam).UsePixelRatio)
                {
                    (toolParam as DistanceLLParam).DistanceLLRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入相机内参无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }

                HTuple CamPose = (toolParam as DistanceLLParam).Hv_CamPose;
                if ((CamPose == null || CamPose.Length <= 0)&&
                    !(toolParam as DistancePLParam).UsePixelRatio)
                {
                    (toolParam as DistanceLLParam).DistanceLLRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入相机位姿无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
              //直线1
                if (!dm.resultFlagDic[(toolParam as DistanceLLParam).InputLineName])
                {
                    (toolParam as DistanceLLParam).DistanceLLRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入直线无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                StuLineData line1Data = dm.LineDataDic[(toolParam as DistanceLLParam).InputLineName];
                //直线
                HOperatorSet.GenContourPolygonXld(out HObject line,
                     new HTuple(line1Data.spRow).TupleConcat(line1Data.epRow),
                      new HTuple(line1Data.spColumn).TupleConcat(line1Data.epColumn));
                (toolParam as DistanceLLParam).Resultline1 = line.Clone();


                StuLineData line2Data = dm.LineDataDic[(toolParam as DistanceLLParam).InputLine2Name];
                //直线
                HOperatorSet.GenContourPolygonXld(out HObject line2,
                     new HTuple(line2Data.spRow).TupleConcat(line2Data.epRow),
                      new HTuple(line2Data.spColumn).TupleConcat(line2Data.epColumn));
                (toolParam as DistanceLLParam).Resultline2 = line2.Clone();

                //检测
                HOperatorSet.DistanceCcMinPoints(line, line2, "point_to_segment",
                  out HTuple distanceMin,out HTuple row1,out HTuple column1,
                  out HTuple row2,out HTuple column2);
                HOperatorSet.GenCrossContourXld(out HObject cross1, row1, column1, 20, 0);
                HOperatorSet.GenCrossContourXld(out HObject cross2, row2, column2, 20, 0);
                HOperatorSet.GenContourPolygonXld(out HObject contour,
                   row1.TupleConcat(row2), column1.TupleConcat(column2));

                //检测
                if ((toolParam as DistanceLLParam).UsePixelRatio)//使用像素比转换计算
                {
                    HOperatorSet.DistancePp(row1, column1, row2, column2, out HTuple hv_Distance1);
                    double distance = hv_Distance1.D * (toolParam as DistanceLLParam).PixelRatio;
                    (toolParam as DistanceLLParam).Distance = Math.Round(distance, 3);
                    if (!dm.SizeDataDic.ContainsKey(toolName))
                        dm.SizeDataDic.Add(toolName, Math.Round(distance, 3));
                    else
                        dm.SizeDataDic[toolName] = Math.Round(distance, 3);
                }
                else
                {


                    HOperatorSet.ImagePointsToWorldPlane(CamParam, CamPose, row1, column1, "mm",
                        out HTuple hv_X2, out HTuple hv_Y2);
                    HOperatorSet.ImagePointsToWorldPlane(CamParam, CamPose, row2, column2, "mm",
                        out HTuple hv_X3, out HTuple hv_Y3);
                    HOperatorSet.DistancePp(hv_Y2, hv_X2, hv_Y3, hv_X3, out HTuple hv_Distance1);
                    (toolParam as DistanceLLParam).Distance = Math.Round(hv_Distance1.D, 3);
                    if (!dm.SizeDataDic.ContainsKey(toolName))
                        dm.SizeDataDic.Add(toolName, Math.Round(hv_Distance1.D, 3));
                    else
                        dm.SizeDataDic[toolName] = Math.Round(hv_Distance1.D, 3);
                }
                string info = toolName + "检测完成";
                //测试信息
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;
                //测试结果
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, true);
                else
                    dm.resultFlagDic[toolName] = true;

               
                HOperatorSet.ConcatObj(cross1, cross2, out HObject objectConcat1);
                HOperatorSet.ConcatObj(line, line2, out HObject objectsConcat);
                HOperatorSet.ConcatObj(objectsConcat, objectConcat1, out HObject objectsConcat3);
                HOperatorSet.ConcatObj(objectsConcat3, contour, out HObject objectsConcat4);
                
                if (!dm.resultBufDic.ContainsKey(toolName))
                    dm.resultBufDic.Add(toolName, objectsConcat4.Clone());
                else
                    dm.resultBufDic[toolName] = objectsConcat4.Clone();

                HOperatorSet.ConcatObj((toolParam as DistanceLLParam).InputImg, objectsConcat4, out HObject objectsConcat2);
                (toolParam as DistanceLLParam).OutputImg = objectsConcat2;
                result.runFlag = true;
                (toolParam as DistanceLLParam).DistanceLLRunStatus = true;

                cross1.Dispose();
                cross2.Dispose();
                contour.Dispose();
                objectsConcat.Dispose();
                objectConcat1.Dispose();
                objectsConcat3.Dispose();
                objectsConcat4.Dispose();
                line.Dispose();
                line2.Dispose();
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
                (toolParam as DistanceLLParam).DistanceLLRunStatus = false;
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
}
