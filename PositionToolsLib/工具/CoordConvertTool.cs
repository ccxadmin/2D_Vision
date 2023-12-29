using HalconDotNet;
using OSLog;
using PositionToolsLib.参数;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.工具
{
    public class CoordConvertTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号
        public string CalibFilePath = "";//标定文件路径
        public CoordConvertTool()
        {
            toolParam = new CoordConvertParam();
            toolName = "坐标换算" + inum;
            inum++;
        }

        public void Dispose()
        {

        }

        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("坐标换算");

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("坐标换算", ""));
            if (number > inum)
                inum = number;
            //toolName = "图像颜色转换" + number;
            inum++;
        }

        public override RunResult Run()
        {
            RunResult result = new RunResult();
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (sw == null) sw = new System.Diagnostics.Stopwatch();
            sw.Restart();
            DataManage dm = GetManage();
            try
            {
                bool transFlag = false;

                double  x1 = 0, y1 = 0;
                if (dm.resultFlagDic[(toolParam as CoordConvertParam).CoordXName])
                {
                    StuCoordinateData xDat = dm.PositionDataDic[(toolParam as CoordConvertParam).CoordXName];
                    x1 = xDat.x;
                }
                if (dm.resultFlagDic[(toolParam as CoordConvertParam).CoordYName])
                {
                    StuCoordinateData yDat = dm.PositionDataDic[(toolParam as CoordConvertParam).CoordYName];
                    y1 = yDat.y;
                }
               
                HTuple Cx = 0, Cy = 0;
                if ((toolParam as CoordConvertParam).ConvertWay == EumConvertWay.ToPhysical)
                    transFlag = Transformation_POINT(x1, y1, out Cx, out Cy); //转物理
                else
                    transFlag = Transformation_POINT_INV(x1, y1, out Cx, out Cy);//转像素

                if (!transFlag)
                {
                    (toolParam as CoordConvertParam).CoordConvertRunStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName + "坐标换算异常";
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

                    (toolParam as CoordConvertParam).ConvertedX = 0;
                    (toolParam as CoordConvertParam).ConvertedY = 0;

                    sw.Stop();
                    result.runTime = sw.ElapsedMilliseconds;
                    return result;
                 
                }
                if (!dm.PositionDataDic.ContainsKey(toolName))
                    dm.PositionDataDic.Add(toolName, new StuCoordinateData(Cx.D, Cy.D, 0));
                else
                    dm.PositionDataDic[toolName] = new StuCoordinateData(Cx.D, Cy.D, 0);

                (toolParam as CoordConvertParam).ConvertedX = Cx.D;
                (toolParam as CoordConvertParam).ConvertedY = Cy.D;

                string info = toolName + "检测完成";
                if (!dm.resultInfoDic.ContainsKey(toolName))
                    dm.resultInfoDic.Add(toolName, info);
                else
                    dm.resultInfoDic[toolName] = info;

                result.runFlag = true;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, true);
                else
                    dm.resultFlagDic[toolName] = true;
                (toolParam as CoordConvertParam).CoordConvertRunStatus = true;

            }
            catch (Exception er)
            {
                log.Error(funName, er.Message);
                result.runFlag = false;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, false);
                else
                    dm.resultFlagDic[toolName] = false;
                (toolParam as CoordConvertParam).CoordConvertRunStatus = false;
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
