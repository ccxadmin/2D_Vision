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
    /// 点线距离
    /// </summary>
    [Serializable]
    public class DistancePLTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号
        //public string CalibFilePath = "";//标定文件路径
        public DistancePLTool()
        {
            toolParam = new DistancePLParam();
            toolName = "点线距离" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if ((toolParam as DistancePLParam).Hv_CamParam == null)
            {
                (toolParam as DistancePLParam).Hv_CamParam = new HTuple();
            }

        }
        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("点线距离");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("点线距离", ""));
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
            (toolParam as DistancePLParam).Hv_CamParam = homMat2D;
        }

        /// <summary>
        /// 点线距离工具运行
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
                (toolParam as DistancePLParam).Distance = 0;
                (toolParam as DistancePLParam).InputImg = dm.imageBufDic[(toolParam as DistancePLParam).InputImageName];
                if (!ObjectValided((toolParam as DistancePLParam).InputImg))
                {
                    (toolParam as DistancePLParam).DistancePLRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                HTuple CamParam = (toolParam as DistancePLParam).Hv_CamParam;
                if (CamParam == null || CamParam.Length <= 0)
                {
                    (toolParam as DistancePLParam).DistancePLRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入相机内参无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }

                HTuple CamPose = (toolParam as DistancePLParam).Hv_CamPose;
                if (CamPose == null || CamPose.Length <= 0)
                {
                    (toolParam as DistancePLParam).DistancePLRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入相机位姿无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                double x1 = 0, y1 = 0;
                if (dm.resultFlagDic[(toolParam as DistancePLParam).StartXName.Replace("起点", "").Replace("终点", "")])
                {
                    StuCoordinateData xDat = dm.PositionDataDic[(toolParam as DistancePLParam).StartXName];
                    x1 = xDat.x;
                }
                if (dm.resultFlagDic[(toolParam as DistancePLParam).StartYName.Replace("起点", "").Replace("终点", "")])
                {
                    StuCoordinateData yDat = dm.PositionDataDic[(toolParam as DistancePLParam).StartYName];
                    y1 = yDat.y;
                }

                if (!dm.resultFlagDic[(toolParam as DistancePLParam).InputLineName])
                {
                    (toolParam as DistancePLParam).DistancePLRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "输入直线无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                StuLineData line1 = dm.LineDataDic[(toolParam as DistancePLParam).InputLineName];
               //直线
                HOperatorSet.GenContourPolygonXld(out HObject line,
                     new HTuple ( line1.spRow).TupleConcat(line1.epRow),
                      new HTuple(line1.spColumn).TupleConcat(line1.epColumn));
                (toolParam as DistancePLParam).Resultline = line.Clone();

                double x2 = 0, y2 = 0;
                //垂点
                HOperatorSet.ProjectionPl(y1,x1, line1.spRow,
                    line1.spColumn, line1.epRow, line1.epColumn,
                    out HTuple rowProj,out HTuple colProj);
                //垂线
                HOperatorSet.GenContourPolygonXld(out HObject lineContour1,
                    new HTuple( y1).TupleConcat(rowProj),
                  new HTuple( x1).TupleConcat(colProj));
                
              
                x2 = colProj.D;
                y2 = rowProj.D;
                //检测
                if ((toolParam as DistancePLParam).UsePixelRatio)//使用像素比转换计算
                {
                    HOperatorSet.DistancePp(y1, x1, y2, x2, out HTuple hv_Distance1);
                    double distance = hv_Distance1.D * (toolParam as DistancePLParam).PixelRatio;
                    (toolParam as DistancePLParam).Distance = Math.Round(distance, 3);
                    if (!dm.SizeDataDic.ContainsKey(toolName))
                        dm.SizeDataDic.Add(toolName, Math.Round(distance, 3));
                    else
                        dm.SizeDataDic[toolName] = Math.Round(distance, 3);
                }
                else
                {

                    HOperatorSet.ImagePointsToWorldPlane(CamParam, CamPose, y1, x1, "mm",
                        out HTuple hv_X2, out HTuple hv_Y2);
                    HOperatorSet.ImagePointsToWorldPlane(CamParam, CamPose, y2, x2, "mm",
                        out HTuple hv_X3, out HTuple hv_Y3);
                    HOperatorSet.DistancePp(hv_Y2, hv_X2, hv_Y3, hv_X3, out HTuple hv_Distance1);
                    (toolParam as DistancePLParam).Distance = Math.Round(hv_Distance1.D, 3);
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

                HOperatorSet.GenCrossContourXld(out HObject cross1, y1, x1, 20, 0);
                HOperatorSet.GenCrossContourXld(out HObject cross2, y2, x2, 20, 0);
                HOperatorSet.ConcatObj(cross1, cross2, out HObject objectConcat1);
               
              

                HOperatorSet.ConcatObj(line, lineContour1,out HObject objectsConcat);
                HOperatorSet.ConcatObj(objectsConcat, objectConcat1, out HObject objectsConcat3);

                if (!dm.resultBufDic.ContainsKey(toolName))
                    dm.resultBufDic.Add(toolName, objectsConcat3.Clone());
                else
                    dm.resultBufDic[toolName] = objectsConcat3.Clone();

                HOperatorSet.ConcatObj((toolParam as DistancePLParam).InputImg, objectsConcat3, out HObject objectsConcat2);
                (toolParam as DistancePLParam).OutputImg = objectsConcat2;
                result.runFlag = true;
                (toolParam as DistancePLParam).DistancePLRunStatus = true;

                cross1.Dispose();
                cross2.Dispose();
                objectsConcat.Dispose();
                objectConcat1.Dispose();
                objectsConcat3.Dispose();
                line.Dispose();
                lineContour1.Dispose();
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
                (toolParam as DistancePLParam).DistancePLRunStatus = false;
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
