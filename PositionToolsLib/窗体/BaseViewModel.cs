using PositionToolsLib.参数;
using PositionToolsLib.工具;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionShowLib.UserControls;

namespace PositionToolsLib.窗体
{
    public class BaseViewModel
    {
        public HObject imgBuf = null;//图像缓存
        public DataManage dataManage = null;
        public BaseViewModel(BaseTool tool) 
        {
           
            baseTool = tool;
            ShowTool = new VisionShowTool();        

        }
        //图像显示工具
        public VisionShowTool ShowTool { get; set; }
        /// <summary>
        /// 工具
        /// </summary>
        protected BaseTool baseTool = null;

        public delegate void SaveParamHandle(string toolName, BaseParam par);
        public SaveParamHandle OnSaveParamHandle = null;
        public delegate double GetPixelRatioHandle();
        public GetPixelRatioHandle getPixelRatioHandle = null;
        public delegate void SaveManageHandle(DataManage data);
        public SaveManageHandle OnSaveManageHandle = null;

    }
}
