using GlueDetectionLib.参数;
using HalconDotNet;
using OSLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;



namespace GlueDetectionLib.工具
{
    /// <summary>
    /// 图像结果显示工具
    /// </summary>
    [Serializable]
   public class ResultShowTool : BaseTool, IDisposable
    {
        public static int inum = 0;//工具编号

        public ResultShowTool()
        {
            toolParam = new ResultShowParam();
            toolName = "图像结果显示" + inum;
            inum++;
        }

        public void Dispose()
        {

        }
        //文本信息起始坐标
        public int inforCoorX = 10;
        public int inforCoorY = 300;
        //胶水信息

        public Dictionary<string, CoorditionDat> GlueInfoDic 
                            = new Dictionary<string, CoorditionDat>();
        //public int glueInfoCoorX = 10;
        //public int glueInfoCoorY = 2000;
        //public int glueInfoOffetX = 500;
        //public int glueInfoOffetY = 0;
        //工具日志:同类型工具日志信息放一起      
        static private Log log = new Log("图像结果显示");
        //是否显示检测区域
        public bool isShowInspectRegion = true;

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {

            int number = int.Parse(toolName.Replace("图像结果显示", ""));
            if (number > inum)
                inum = number;
            //toolName = "图像结果显示" + number;
            inum++;
        }
        /// <summary>
        ///图像结果显示工具运行
        /// </summary>
        /// <returns></returns>
        override public RunResult Run()
        {
            RunResult result = new RunResult();
            string funName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (sw == null) sw = new System.Diagnostics.Stopwatch();
            sw.Restart();
            DataManage dm = GetManage();
            try
            {
                if(BaseTool.ObjectValided((toolParam as ResultShowParam).ResultRegion))
                      (toolParam as ResultShowParam).ResultRegion.Dispose();
                (toolParam as ResultShowParam).ResultInfo.Clear();

                (toolParam as ResultShowParam).InputImg = dm.imageBufDic[(toolParam as ResultShowParam).InputImageName];
                if (!ObjectValided((toolParam as ResultShowParam).InputImg))
                {
                    (toolParam as ResultShowParam).ResultShowStatus = false;
                    result.runFlag = false;
                    result.errInfo = toolName+"输入图像无效";
                    if (!dm.resultFlagDic.ContainsKey(toolName))
                        dm.resultFlagDic.Add(toolName, false);
                    else
                        dm.resultFlagDic[toolName] = false;
                    return result;
                }
                (toolParam as ResultShowParam).OutputImg = (toolParam as ResultShowParam).InputImg;//输出图像   
                HObject emptyObj = null;
                HOperatorSet.GenEmptyObj(out emptyObj);
                List<StuFlagInfo> flagInfoList = new List<StuFlagInfo>();
             
                foreach (var s in (toolParam as ResultShowParam).ResultShowDataList)
                {
                    if (s.Use)
                    {
                        if (dm.resultBufDic.ContainsKey(s.ToolName))
                            if (BaseTool.ObjectValided(dm.resultBufDic[s.ToolName]))
                                HOperatorSet.ConcatObj(emptyObj, dm.resultBufDic[s.ToolName], out emptyObj);
                        if (dm.resultInfoDic.ContainsKey(s.ToolName))
                            flagInfoList.Add(new StuFlagInfo(dm.resultInfoDic[s.ToolName],
                                      dm.resultFlagDic[s.ToolName]));
                    }
                       
                }
                 (toolParam as ResultShowParam).ResultRegion = emptyObj.Clone();
                (toolParam as ResultShowParam).ResultInfo = flagInfoList;           
                result.runFlag = true;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, true);
                else
                    dm.resultFlagDic[toolName] = true;
                (toolParam as ResultShowParam).ResultShowStatus = true;
                emptyObj.Dispose();
            }
            catch (Exception er)
            {
                log.Error(funName, er.Message);
                result.runFlag = false;
                if (!dm.resultFlagDic.ContainsKey(toolName))
                    dm.resultFlagDic.Add(toolName, false);
                else
                    dm.resultFlagDic[toolName] = false;
                (toolParam as ResultShowParam).ResultShowStatus = false;
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
