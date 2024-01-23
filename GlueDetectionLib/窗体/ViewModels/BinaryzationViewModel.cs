using ControlShareResources.Common;
using GlueDetectionLib.参数;
using GlueDetectionLib.工具;
using GlueDetectionLib.窗体.Models;
using HalconDotNet;
using ROIGenerateLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisionShowLib.UserControls;

namespace GlueDetectionLib.窗体.ViewModels
{
    public class BinaryzationViewModel: BaseViewModel
    {

        //HObject imgBuf = null;//图像缓存
        //DataManage dataManage = null;
        public static BinaryzationViewModel This { get; set; }
        public BinaryzationModel Model { get; set; }
      
        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }
       //最小灰度值调整
        //public CommandBase NumGrayMinKeyDownCommand { get; set; }
        //最大灰度值调整
        //public CommandBase NumGrayMaxKeyDownCommand { get; set; }
        public BinaryzationViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new BinaryzationModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
                   
            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
         
            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TestButClickCommand = new CommandBase();
            TestButClickCommand.DoExecute = new Action<object>((o) => btnTest_Click());
            TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            //NumGrayMinKeyDownCommand = new CommandBase();
            //NumGrayMinKeyDownCommand.DoExecute = new Action<object>((o) => NumGrayMinKeyDown(o));
            //NumGrayMinKeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //Model.NumGrayMinValueChangeAction = new Action(() => NumGrayMinValueEvent());

            //NumGrayMaxKeyDownCommand = new CommandBase();
            //NumGrayMaxKeyDownCommand.DoExecute = new Action<object>((o) => NumGrayMaxKeyDown(o));
            //NumGrayMaxKeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //Model.NumGrayMaxValueChangeAction = new Action(() => NumGrayMaxValueEvent());

            ShowData();
            cobxImageList_SelectedIndexChanged(null);
        }
        /// <summary>
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        void ShowData()
        {
            BaseParam par = baseTool.GetParam();
            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);
            string imageName = (par as BinaryzationParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);

            Model.SelectImageIndex = index;
            Model.SelectImageName = (par as BinaryzationParam).InputImageName;
            Model.GrayMin = (par as BinaryzationParam).GrayMin;
            Model.GrayMax = (par as BinaryzationParam).GrayMax;
            Model.IsInvertImage = (par as BinaryzationParam).IsInvert;
        }
        /// <summary>
        /// 图像加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadedImageNoticeEvent(object sender, EventArgs e)
        {
            HOperatorSet.GenEmptyObj(out imgBuf);
            imgBuf.Dispose();
            imgBuf = ShowTool.D_HImage;
        }
        /// <summary>
        ///输入图像选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cobxImageList_SelectedIndexChanged(object value)
        {
            if (Model.SelectImageIndex == -1) return;
            if (!dataManage.imageBufDic.ContainsKey(Model.SelectImageName)) return;
            if (!DilationTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as BinaryzationParam).InputImageName = Model.SelectImageName;
        }
        /// <summary>
        /// 图像二值化参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as BinaryzationParam).GrayMin = Model.GrayMin;
            (par as BinaryzationParam).GrayMax = Model.GrayMax;
            (par as BinaryzationParam).IsInvert = Model.IsInvertImage;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
            OnSaveManageHandle?.Invoke(dataManage);
        }
        /// <summary>
        /// 图像二值化测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as BinaryzationParam).GrayMin = Model.GrayMin;
            (par as BinaryzationParam).GrayMax = Model.GrayMax;
            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispImage((par as BinaryzationParam).OutputImg);
                ShowTool.D_HImage = (par as BinaryzationParam).OutputImg;
                ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);

            }
            else
            {
                ShowTool.DispImage(imgBuf);
                ShowTool.DispMessage("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.AddTextBuffer("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.DispAlarmMessage(rlt.errInfo, 100, 10, 12);
            }
        }
      
    }
}
