using ControlShareResources.Common;
using GlueDetectionLib.参数;
using GlueDetectionLib.窗体.Models;
using GlueDetectionLib.工具;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlueDetectionLib.窗体.ViewModels
{
    public class ErosionViewModel : BaseViewModel
    {
        public static ErosionViewModel This { get; set; }
        public ErosionModel Model { get; set; }

        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }

        public ErosionViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new ErosionModel();
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
            string imageName = (par as ErosionParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;

            int maskWidthValue = (par as ErosionParam).MaskWidth;
            int index2 = Model.MaskWidthList.IndexOf(maskWidthValue);
            Model.SelectMaskWidthIndex = index2;

            int maskHeightValue = (par as ErosionParam).MaskHeight;
            int index3 = Model.MaskHeightList.IndexOf(maskHeightValue);
            Model.SelectMaskHeightIndex = index3;

            Model.SelectImageName = (par as ErosionParam).InputImageName;
            Model.SelectMaskWidth = (par as ErosionParam).MaskWidth;
            Model.SelectMaskHeight = (par as ErosionParam).MaskHeight;
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
            if (!DilationTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as ErosionParam).InputImageName = Model.SelectImageName;
        }

        /// <summary>
        /// 图像闭运算测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as ErosionParam).MaskWidth = Model.SelectMaskWidth;
            (par as ErosionParam).MaskHeight = Model.SelectMaskHeight;
            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispImage((par as ErosionParam).OutputImg);
                ShowTool.D_HImage = (par as ErosionParam).OutputImg;
                ShowTool.DispMessage("OK", 10, width - 500, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - 500, "green", 100);

            }
            else
            {
                ShowTool.DispImage(imgBuf);
                ShowTool.DispMessage("NG", 10, width - 500, "red", 100);
                ShowTool.AddTextBuffer("NG", 10, width - 500, "red", 100);
                ShowTool.DispAlarmMessage(rlt.errInfo, 100, 10, 12);
            }
        }
        /// <summary>
        /// 图像闭运算参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as ErosionParam).MaskWidth = Model.SelectMaskWidth;
            (par as ErosionParam).MaskHeight = Model.SelectMaskHeight;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
        }
    }
}
