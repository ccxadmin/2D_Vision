using ControlShareResources.Common;
using HalconDotNet;
using PositionToolsLib.参数;
using PositionToolsLib.工具;
using PositionToolsLib.窗体.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.ViewModels
{
    public class OpeningViewModel : BaseViewModel
    {
        public static OpeningViewModel This { get; set; }
        public OpeningModel Model { get; set; }

        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }

        public OpeningViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new OpeningModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
            BaseParam par = baseTool.GetParam();
            ShowData(par);

            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TestButClickCommand = new CommandBase();
            TestButClickCommand.DoExecute = new Action<object>((o) => btnTest_Click());
            TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);

            string imageName = (par as OpeningParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;

            int maskWidthValue = (par as OpeningParam).MaskWidth;
            int index2 = Model.MaskWidthList.IndexOf(maskWidthValue);
            Model.SelectMaskWidthIndex = index2;

            int maskHeightValue = (par as OpeningParam).MaskHeight;
            int index3 = Model.MaskHeightList.IndexOf(maskHeightValue);
            Model.SelectMaskHeightIndex = index3;
        }

        /// <summary>
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        void ShowData(BaseParam parDat)
        {

            Model.SelectImageName = (parDat as OpeningParam).InputImageName;
            Model.SelectMaskWidth = (parDat as OpeningParam).MaskWidth;
            Model.SelectMaskHeight = (parDat as OpeningParam).MaskHeight;
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
            if (!OpeningTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as OpeningParam).InputImageName = Model.SelectImageName;
        }

        /// <summary>
        /// 图像闭运算测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as OpeningParam).MaskWidth = Model.SelectMaskWidth;
            (par as OpeningParam).MaskHeight = Model.SelectMaskHeight;
            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispImage((par as OpeningParam).OutputImg);
                ShowTool.D_HImage = (par as OpeningParam).OutputImg;
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
            (par as OpeningParam).MaskWidth = Model.SelectMaskWidth;
            (par as OpeningParam).MaskHeight = Model.SelectMaskHeight;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
        }
    }
}
