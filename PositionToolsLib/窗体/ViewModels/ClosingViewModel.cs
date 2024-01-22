using ControlShareResources.Common;
using PositionToolsLib.参数;
using PositionToolsLib.工具;
using PositionToolsLib.窗体.Models;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Media.Media3D;

namespace PositionToolsLib.窗体.ViewModels
{
    public class ClosingViewModel : BaseViewModel
    {
        public static ClosingViewModel This { get; set; }
        public ClosingModel Model { get; set; }

        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }

        public ClosingViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new ClosingModel();
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
            /*--如果直接xaml中设置Combox.SelectionChanged="SelectionChanged"，
             * 那么在Loaded时，SelectionChanged事件会正常触发。
             * loaded时，Triggers还没生效，也就是说要等页面加载完毕Triggers才会被实例化。
             */
            cobxImageList_SelectedIndexChanged(null);
        }

        /// <summary>
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        private  void ShowData()
        {
            BaseParam par = baseTool.GetParam();
            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);          
            string imageName = (par as ClosingParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;
      
            int maskWidthValue = (par as ClosingParam).MaskWidth;
            int index2 = Model.MaskWidthList.IndexOf(maskWidthValue);
            Model.SelectMaskWidthIndex = index2;

            int maskHeightValue = (par as ClosingParam).MaskHeight;
            int index3 = Model.MaskHeightList.IndexOf(maskHeightValue);
            Model.SelectMaskHeightIndex = index3;

            Model.SelectImageName = (par as ClosingParam).InputImageName;
            Model.SelectMaskWidth = (par as ClosingParam).MaskWidth;
            Model.SelectMaskHeight = (par as ClosingParam).MaskHeight;
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
            if (!DilationTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as ClosingParam).InputImageName = Model.SelectImageName;
        }

        /// <summary>
        /// 图像闭运算测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as ClosingParam).MaskWidth = Model.SelectMaskWidth;
            (par as ClosingParam).MaskHeight = Model.SelectMaskHeight;
            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispImage((par as ClosingParam).OutputImg);
                ShowTool.D_HImage = (par as ClosingParam).OutputImg;
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
        /// <summary>
        /// 图像闭运算参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as ClosingParam).MaskWidth = Model.SelectMaskWidth;
            (par as ClosingParam).MaskHeight = Model.SelectMaskHeight;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
        }
    }
}
