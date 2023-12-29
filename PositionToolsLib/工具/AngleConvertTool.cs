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
    public class AngleConvertTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号
        public string CalibFilePath = "";//标定文件路径
        public AngleConvertTool()
        {
            toolParam = new AngleConvertParam();
            toolName = "角度换算" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("角度换算");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("角度换算", ""));
            if (number > inum)
                inum = number;
         
            inum++;
        }
        /// <summary>
        /// 角度换算工具运行
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

            
                double x1 = 0, y1 = 0;
                if (dm.resultFlagDic[(toolParam as AngleConvertParam).StartXName])
                {
                    StuCoordinateData xDat = dm.PositionDataDic[(toolParam as AngleConvertParam).StartXName];
                    x1 = xDat.x;
                }
                if (dm.resultFlagDic[(toolParam as AngleConvertParam).StartYName])
                {
                    StuCoordinateData yDat = dm.PositionDataDic[(toolParam as AngleConvertParam).StartYName];
                    y1 = yDat.y;
                }
                double x2 = 0, y2 = 0;
                if (dm.resultFlagDic[(toolParam as AngleConvertParam).EndXName])
                {
                    StuCoordinateData xDat = dm.PositionDataDic[(toolParam as AngleConvertParam).EndXName];
                    x2 = xDat.x;
                }
                if (dm.resultFlagDic[(toolParam as AngleConvertParam).EndYName])
                {
                    StuCoordinateData yDat = dm.PositionDataDic[(toolParam as AngleConvertParam).EndYName];
                    y2 = yDat.y;
                }

                //计算物理坐标系下的角度
                HTuple Rx, Ry, Rx2, Ry2, Angle = 0;
                bool transFlag = Transformation_POINT(x1, y1, out Rx, out Ry);
                bool transFlag2 = Transformation_POINT(x2, y2, out Rx2, out Ry2);
                //角度
                if (transFlag && transFlag2)
                    Angle = calAngleOfLx(Rx, Ry, Rx2, Ry2);
                else
                {
                    result.errInfo = toolName + "角度换算检测异常";
                    (toolParam as AngleConvertParam).AngleConvertRunStatus = false;
                    result.runFlag = false;
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;

                    if (!dm.PositionDataDic.ContainsKey(toolName))
                        dm.PositionDataDic.Add(toolName, new StuCoordinateData(0,
                           0, 0));
                    else
                        dm.PositionDataDic[toolName] = new StuCoordinateData(0,
                            0, 0);
                    (toolParam as AngleConvertParam).Angle = 0;
                    sw.Stop();
                    result.runTime = sw.ElapsedMilliseconds;
                    return result;
                }                  
                (toolParam as AngleConvertParam).Angle = Angle.D;             

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

                          
                if (!dm.PositionDataDic.ContainsKey(toolName ))
                    dm.PositionDataDic.Add(toolName , new StuCoordinateData(0,
                          0, Angle.D));
                else
                    dm.PositionDataDic[toolName ] = new StuCoordinateData(0,
                            0, Angle.D);
          

                result.runFlag = true;
                (toolParam as AngleConvertParam).AngleConvertRunStatus = true;

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
                (toolParam as AngleConvertParam).AngleConvertRunStatus = false;
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
