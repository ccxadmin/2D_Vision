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
    public class DistancePLViewModel : BaseViewModel
    {
        public static DistancePLViewModel This { get; set; }
        public DistancePLModel Model { get; set; }
        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase OpenFileCommand { get; set; }
        public CommandBase StartXSelectionChangedCommand { get; set; }
        public CommandBase StartYSelectionChangedCommand { get; set; }
        public CommandBase Line1SelectionChangedCommand { get; set; }
        public CommandBase cobxUsePixelRatioCheckChangeCommand { get; set; }
        public CommandBase btnGetPixelRatioClickCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }

        StuLineData lineData1;
        public DistancePLViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new DistancePLModel();
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

            Line1SelectionChangedCommand = new CommandBase();
            Line1SelectionChangedCommand.DoExecute = new Action<object>((o) => cobxLineList_SelectedIndexChanged(o));
            Line1SelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            cobxUsePixelRatioCheckChangeCommand = new CommandBase();
            cobxUsePixelRatioCheckChangeCommand.DoExecute = new Action<object>((o) => cobxUsePixelRatio_CheckChange());
            cobxUsePixelRatioCheckChangeCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            btnGetPixelRatioClickCommand = new CommandBase();
            btnGetPixelRatioClickCommand.DoExecute = new Action<object>((o) => btnGetPixelRatio_Click());
            btnGetPixelRatioClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

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
            string imageName = (par as DistancePLParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;
            Model.SelectImageName = (par as DistancePLParam).InputImageName;

            Model.CamParamFilePath = (par as DistancePLParam).CamParamFilePath;
            Model.CamPoseFilePath = (par as DistancePLParam).CamPoseFilePath;

            foreach (var s in dataManage.PositionDataDic)
                Model.PositionDataList.Add(s.Key);
            //x
            string rowName = (par as DistancePLParam).StartXName;
            int index2 = Model.PositionDataList.IndexOf(rowName);
            Model.SelectStartXIndex = index2;

            //y
            string columnName = (par as DistancePLParam).StartYName;
            int index3 = Model.PositionDataList.IndexOf(columnName);
            Model.SelectStartYIndex = index3;

            foreach (var s in dataManage.LineDataDic)
                Model.LineList.Add(s.Key);
            string lineName = (par as DistancePLParam).InputLineName;
            int index4 = Model.LineList.IndexOf(lineName);
            Model.SelectLine1Index = index4;

            lineData1 = (par as DistancePLParam).LineData;

            Model.UsePixelRatio = (par as DistancePLParam).UsePixelRatio;
            Model.TxbPixelRatio = (par as DistancePLParam).PixelRatio;
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
            if (!DistancePLTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as DistancePLParam).InputImageName = Model.SelectImageName;
        }
        /// <summary>
        ///输入直线1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cobxLineList_SelectedIndexChanged(object value)
        {
            lineData1 = dataManage.LineDataDic[Model.SelectLine1Name];
            BaseParam par = baseTool.GetParam();
            (par as DistancePLParam).InputLineName = Model.SelectLine1Name;

            HOperatorSet.GenContourPolygonXld(out HObject lineContour1,
               new HTuple( lineData1.spRow).TupleConcat(lineData1.epRow),
                new HTuple(lineData1.spColumn).TupleConcat(lineData1.epColumn));

            ShowTool.DispRegion(lineContour1, "green");
            ShowTool.AddregionBuffer(lineContour1, "green");

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
                if (content == "相机内参文件选择")
                {
                    Model.CamParamFilePath = filename;
                    HOperatorSet.ReadCamPar(filename, out HTuple hv_CamParam);

                    //传递当前读取内参
                    BaseParam par = baseTool.GetParam();
                    (par as DistancePLParam).Hv_CamParam = hv_CamParam;
                }
                else
                {
                    Model.CamPoseFilePath = filename;
                    HOperatorSet.ReadPose(filename, out HTuple hv_CamPose);

                    //传递当前读位姿
                    BaseParam par = baseTool.GetParam();
                    (par as DistancePLParam).Hv_CamPose = hv_CamPose;
                }

            }
        }
        void cobxUsePixelRatio_CheckChange()
        {
            BaseParam par = baseTool.GetParam();
            (par as DistancePLParam).UsePixelRatio = Model.UsePixelRatio;
        }
        /// <summary>
        /// 获取像素转换比
        /// </summary>
        void btnGetPixelRatio_Click()
        {
            if (getPixelRatioHandle != null)
            {
                double tem = getPixelRatioHandle.Invoke();
                if (tem <= 0)
                    tem = 1;
                Model.TxbPixelRatio = tem;
            }
        }
        /// <summary>
        /// 参数保存
        /// </summary>
        void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as DistancePLParam).StartXName = Model.SelectStartXName;
            (par as DistancePLParam).StartYName = Model.SelectStartYName;
            (par as DistancePLParam).LineData = lineData1;
            (par as DistancePLParam).InputLineName = Model.SelectLine1Name;
            (par as DistancePLParam).CamParamFilePath = Model.CamParamFilePath;
            (par as DistancePLParam).CamPoseFilePath = Model.CamPoseFilePath;
            (par as DistancePLParam).UsePixelRatio = Model.UsePixelRatio;
            (par as DistancePLParam).PixelRatio = Model.TxbPixelRatio;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
            OnSaveManageHandle?.Invoke(dataManage);
        }
        private void cobxStartX_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectStartXName];

            BaseParam par = baseTool.GetParam();
            (par as DistancePLParam).StartXName = Model.SelectStartXName;
        }

        private void cobxStartY_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectStartYName];

            BaseParam par = baseTool.GetParam();
            (par as DistancePLParam).StartYName = Model.SelectStartYName;
        }
        /// <summary>
        /// 更新结果表格数据
        /// </summary>
        /// <param name="DistancePLResultData"></param>
        void UpdateResultView(DistancePLResultData Data)
        {
            Model.DgResultOfDistancePLList.Clear();
            Model.DgResultOfDistancePLList.Add(Data);
        }

        /// <summary>
        /// 手动测试
        /// </summary>
        void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as DistancePLParam).StartXName = Model.SelectStartXName;
            (par as DistancePLParam).StartYName = Model.SelectStartYName;
            (par as DistancePLParam).LineData = lineData1;
            (par as DistancePLParam).InputLineName = Model.SelectLine1Name;
            (par as DistancePLParam).UsePixelRatio = Model.UsePixelRatio;
            (par as DistancePLParam).PixelRatio = Model.TxbPixelRatio;
            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispConcatedObj((par as DistancePLParam).OutputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer((par as DistancePLParam).OutputImg, EumCommonColors.green);

                ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                //更新结果表格数据
                UpdateResultView(new DistancePLResultData(1,
                    (par as DistancePLParam).Distance
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
