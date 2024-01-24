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
using VisionShowLib.UserControls;

namespace PositionToolsLib.窗体.ViewModels
{
    public class DistancePPViewModel : BaseViewModel
    {
        public static DistancePPViewModel This { get; set; }
        public DistancePPModel Model { get; set; }
        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase OpenFileCommand { get; set; }
        public CommandBase StartXSelectionChangedCommand { get; set; }
        public CommandBase StartYSelectionChangedCommand { get; set; }
        public CommandBase EndXSelectionChangedCommand { get; set; }
        public CommandBase EndYSelectionChangedCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }

        public DistancePPViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new DistancePPModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
          

            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            OpenFileCommand = new CommandBase();
            OpenFileCommand.DoExecute = new Action<object>((o) => btnOpenFile_Click(o));
            OpenFileCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            StartXSelectionChangedCommand = new CommandBase();
            StartXSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxStartX_SelectedIndexChanged(o));
            StartXSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            StartYSelectionChangedCommand = new CommandBase();
            StartYSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxStartY_SelectedIndexChanged(o));
            StartYSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            EndXSelectionChangedCommand = new CommandBase();
            EndXSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxEndX_SelectedIndexChanged(o));
            EndXSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            EndYSelectionChangedCommand = new CommandBase();
            EndYSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxEndY_SelectedIndexChanged(o));
            EndYSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


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
            string imageName = (par as DistancePPParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;
            Model.SelectImageName = (par as DistancePPParam).InputImageName;
            Model.CamParamFilePath=(par as DistancePPParam).CamParamFilePath ;
            Model.CamPoseFilePath = (par as DistancePPParam).CamPoseFilePath;

            foreach (var s in dataManage.PositionDataDic)
                Model.PositionDataList.Add(s.Key);
            //x
            string rowName = (par as DistancePPParam).StartXName;
            int index2 = Model.PositionDataList.IndexOf(rowName);
            Model.SelectStartXIndex = index2;

            //y

            string columnName = (par as DistancePPParam).StartYName;
            int index3 = Model.PositionDataList.IndexOf(columnName);
            Model.SelectStartYIndex = index3;

            //x2

            string rowName2 = (par as DistancePPParam).EndXName;
            int index4 = Model.PositionDataList.IndexOf(rowName2);
            Model.SelectEndXIndex = index4;

            //y2

            string columnName2 = (par as DistancePPParam).EndYName;
            int index5 = Model.PositionDataList.IndexOf(columnName2);
            Model.SelectEndYIndex = index5;

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
            if (!DistancePPTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as DistancePPParam).InputImageName = Model.SelectImageName;
        }

        /// <summary>
        /// 打开标定文件
        /// </summary>
        void btnOpenFile_Click(object o)
        {
            string content = o.ToString();
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "标定文件"; // Default file name
            dialog.DefaultExt = ".dat"; // Default file extension
            dialog.Filter = "标定文件 (.dat)|*.dat"; // Filter files by extension
            dialog.InitialDirectory = Environment.CurrentDirectory;
            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                if(content=="相机内参文件选择")
                {
                    Model.CamParamFilePath = filename;
                    HOperatorSet.ReadCamPar(filename, out HTuple hv_CamParam);

                    //传递当前读取内参
                    BaseParam par = baseTool.GetParam();
                    (par as DistancePPParam).Hv_CamParam = hv_CamParam;
                }
                else
                {
                    Model.CamPoseFilePath = filename;
                    HOperatorSet.ReadPose(filename, out HTuple hv_CamPose);

                    //传递当前读位姿
                    BaseParam par = baseTool.GetParam();
                    (par as DistancePPParam).Hv_CamPose = hv_CamPose;
                }
              
            }
        }

        /// <summary>
        /// 参数保存
        /// </summary>
        void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as DistancePPParam).StartXName = Model.SelectStartXName;
            (par as DistancePPParam).StartYName = Model.SelectStartYName;
            (par as DistancePPParam).EndXName = Model.SelectEndXName;
            (par as DistancePPParam).EndYName = Model.SelectEndYName;
            (par as DistancePPParam).CamParamFilePath = Model.CamParamFilePath;
            (par as DistancePPParam).CamPoseFilePath = Model.CamPoseFilePath;

            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);

        }
        private void cobxStartX_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectStartXName];

            BaseParam par = baseTool.GetParam();
            (par as DistancePPParam).StartXName = Model.SelectStartXName;
        }

        private void cobxStartY_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectStartYName];

            BaseParam par = baseTool.GetParam();
            (par as DistancePPParam).StartYName = Model.SelectStartYName;
        }

        private void cobxEndX_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectEndXName];

            BaseParam par = baseTool.GetParam();
            (par as DistancePPParam).EndXName = Model.SelectEndXName;
        }

        private void cobxEndY_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectEndYName];

            BaseParam par = baseTool.GetParam();
            (par as DistancePPParam).EndYName = Model.SelectEndYName;
        }
        /// <summary>
        /// 更新结果表格数据
        /// </summary>
        /// <param name="DistancePPResultData"></param>
        void UpdateResultView(DistancePPResultData Data)
        {
            Model.DgResultOfDistancePPList.Clear();
            Model.DgResultOfDistancePPList.Add(Data);
        }
        /// <summary>
        /// 手动测试
        /// </summary>
        void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as DistancePPParam).StartXName = Model.SelectStartXName;
            (par as DistancePPParam).StartYName = Model.SelectStartYName;
            (par as DistancePPParam).EndXName = Model.SelectEndXName;
            (par as DistancePPParam).EndYName = Model.SelectEndYName;
            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispConcatedObj((par as DistancePPParam).OutputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer((par as DistancePPParam).OutputImg, EumCommonColors.green);
            
                ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                //更新结果表格数据
                UpdateResultView(new DistancePPResultData(1,
                    (par as DistancePPParam).Distance
                 ));
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
