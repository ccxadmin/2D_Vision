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
    public class ImageCorrectViewModel : BaseViewModel
    {
        public static ImageCorrectViewModel This { get; set; }
        public ImageCorrectModel Model { get; set; }
        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase OpenFileCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }

        public ImageCorrectViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new ImageCorrectModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
          

            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            OpenFileCommand = new CommandBase();
            OpenFileCommand.DoExecute = new Action<object>((o) => btnOpenFile_Click());
            OpenFileCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

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
        void ShowData()
        {
            BaseParam par = baseTool.GetParam();
            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);
            string imageName = (par as ImageCorrectParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;

            Model.CalibFilePath=(par as ImageCorrectParam).CalibFilePath ;
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
            if (!ImageCorrectTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as ImageCorrectParam).InputImageName = Model.SelectImageName;
        }

        /// <summary>
        /// 打开标定文件
        /// </summary>
        void btnOpenFile_Click()
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Calib Matrix"; // Default file name
            dialog.DefaultExt = ".dat"; // Default file extension
            dialog.Filter = "Calib Matrix (.dat)|*.dat"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                Model.CalibFilePath = filename;
                HOperatorSet.ReadCamPar(filename, out HTuple hv_CamParam);

                //传递当前读取矩阵
                BaseParam par = baseTool.GetParam();
                (par as ImageCorrectParam).Hv_CamParam = hv_CamParam;
              
            }
        }

        /// <summary>
        /// 参数保存
        /// </summary>
        void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as ImageCorrectParam).CalibFilePath = Model.CalibFilePath;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);

        }
        /// <summary>
        /// 手动测试
        /// </summary>
        void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();      
            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispImage((par as ImageCorrectParam).OutputImg);
                ShowTool.D_HImage = (par as ImageCorrectParam).OutputImg;
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
