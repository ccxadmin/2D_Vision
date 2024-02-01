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
    public class DistanceLLViewModel : BaseViewModel
    {
        public static DistanceLLViewModel This { get; set; }
        public DistanceLLModel Model { get; set; }
        StuLineData lineData1;
        StuLineData lineData2;

        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase OpenFileCommand { get; set; }
        public CommandBase Line1SelectionChangedCommand { get; set; }
        public CommandBase Line2SelectionChangedCommand { get; set; }
        public CommandBase cobxUsePixelRatioCheckChangeCommand { get; set; }
        public CommandBase btnGetPixelRatioClickCommand { get; set; }
        public CommandBase SaveButClickCommand { get; set; }
        public CommandBase TestButClickCommand { get; set; }

        public DistanceLLViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new DistanceLLModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
            #region  Command
            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            OpenFileCommand = new CommandBase();
            OpenFileCommand.DoExecute = new Action<object>((o) => btnOpenFile_Click(o));
            OpenFileCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            Line1SelectionChangedCommand = new CommandBase();
            Line1SelectionChangedCommand.DoExecute = new Action<object>((o) => cobxLineList_SelectedIndexChanged(o));
            Line1SelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            Line2SelectionChangedCommand = new CommandBase();
            Line2SelectionChangedCommand.DoExecute = new Action<object>((o) => cobxLine2List_SelectedIndexChanged(o));
            Line2SelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

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


            #endregion
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
            string imageName = (par as DistanceLLParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;
            Model.SelectImageName = (par as DistanceLLParam).InputImageName;

            Model.CamParamFilePath = (par as DistanceLLParam).CamParamFilePath;
            Model.CamPoseFilePath = (par as DistanceLLParam).CamPoseFilePath;

            foreach (var s in dataManage.LineDataDic)
                Model.LineList.Add(s.Key);

            string lineName = (par as DistanceLLParam).InputLineName;
            int index2 = Model.LineList.IndexOf(lineName);
            Model.SelectLine1Index = index2;


            string line2Name = (par as DistanceLLParam).InputLine2Name;
            int index3 = Model.LineList.IndexOf(line2Name);
            Model.SelectLine2Index = index3;

            lineData1 = (par as DistanceLLParam).LineData;
            lineData2 = (par as DistanceLLParam).LineData2;

            Model.UsePixelRatio = (par as DistanceLLParam).UsePixelRatio;
            Model.TxbPixelRatio = (par as DistanceLLParam).PixelRatio;
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
            if (!DistanceLLTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as DistanceLLParam).InputImageName = Model.SelectImageName;
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
            (par as DistanceLLParam).InputLineName = Model.SelectLine1Name;


            HOperatorSet.GenContourPolygonXld(out HObject lineContour1,
               new HTuple(lineData1.spRow).TupleConcat(lineData1.epRow),
                new HTuple(lineData1.spColumn).TupleConcat(lineData1.epColumn));

            ShowTool.DispRegion(lineContour1, "green");
            ShowTool.AddregionBuffer(lineContour1, "green");

        }

        /// <summary>
        ///输入直线2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cobxLine2List_SelectedIndexChanged(object value)
        {
            lineData2 = dataManage.LineDataDic[Model.SelectLine2Name];
            BaseParam par = baseTool.GetParam();
            (par as DistanceLLParam).InputLine2Name = Model.SelectLine2Name;


            HOperatorSet.GenContourPolygonXld(out HObject lineContour2,
               new HTuple(lineData2.spRow).TupleConcat(lineData2.epRow),
                new HTuple(lineData2.spColumn).TupleConcat(lineData2.epColumn));
 
            ShowTool.DispRegion(lineContour2, "green");
            ShowTool.AddregionBuffer(lineContour2, "green");
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
                    (par as DistanceLLParam).Hv_CamParam = hv_CamParam;
                }
                else
                {
                    Model.CamPoseFilePath = filename;
                    HOperatorSet.ReadPose(filename, out HTuple hv_CamPose);

                    //传递当前读位姿
                    BaseParam par = baseTool.GetParam();
                    (par as DistanceLLParam).Hv_CamPose = hv_CamPose;
                }

            }
        }
        void cobxUsePixelRatio_CheckChange()
        {
            BaseParam par = baseTool.GetParam();
            (par as DistanceLLParam).UsePixelRatio = Model.UsePixelRatio;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as DistanceLLParam).LineData = lineData1;
            (par as DistanceLLParam).LineData2 = lineData2;
            (par as DistanceLLParam).InputLineName = Model.SelectLine1Name;
            (par as DistanceLLParam).InputLine2Name = Model.SelectLine2Name;
            (par as DistanceLLParam).CamParamFilePath = Model.CamParamFilePath;
            (par as DistanceLLParam).CamPoseFilePath = Model.CamPoseFilePath;
            (par as DistanceLLParam).UsePixelRatio = Model.UsePixelRatio;
            (par as DistanceLLParam).PixelRatio = Model.TxbPixelRatio;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
            OnSaveManageHandle?.Invoke(dataManage);
        }

        /// <summary>
        /// 更新结果表格数据
        /// </summary>
        /// <param name="DistanceLLResultData"></param>
        void UpdateResultView(DistanceLLResultData Data)
        {
            Model.DgResultOfDistanceLLList.Clear();
            Model.DgResultOfDistanceLLList.Add(Data);
        }

        /// <summary>
        /// 手动测试
        /// </summary>
        void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as DistanceLLParam).LineData2 = lineData2;
            (par as DistanceLLParam).InputLine2Name = Model.SelectLine2Name;
            (par as DistanceLLParam).LineData = lineData1;
            (par as DistanceLLParam).InputLineName = Model.SelectLine1Name;
            (par as DistanceLLParam).UsePixelRatio = Model.UsePixelRatio;
            (par as DistanceLLParam).PixelRatio = Model.TxbPixelRatio;
            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispConcatedObj((par as DistanceLLParam).OutputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer((par as DistanceLLParam).OutputImg, EumCommonColors.green);

                ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                //更新结果表格数据
                UpdateResultView(new DistanceLLResultData(1,
                    (par as DistanceLLParam).Distance
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
