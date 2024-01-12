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
    public class AngleConvertViewModel : BaseViewModel
    {
        public static AngleConvertViewModel This { get; set; }
        public AngleConvertModel Model { get; set; }
        EumConvertWay convertWay = EumConvertWay.ToPhysical;


        public CommandBase StartXSelectionChangedCommand { get; set; }
        public CommandBase StartYSelectionChangedCommand { get; set; }
        public CommandBase EndXSelectionChangedCommand { get; set; }
        public CommandBase EndYSelectionChangedCommand { get; set; }
        public CommandBase OpenFileCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }

        public AngleConvertViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new AngleConvertModel();
            //图像控件      
            //ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
            BaseParam par = baseTool.GetParam();
            ShowData(par);

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
           

            OpenFileCommand = new CommandBase();
            OpenFileCommand.DoExecute = new Action<object>((o) => btnOpenFile_Click());
            OpenFileCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TestButClickCommand = new CommandBase();
            TestButClickCommand.DoExecute = new Action<object>((o) => btnTest_Click());
            TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            foreach (var s in dataManage.PositionDataDic)
                Model.PositionDataList.Add(s.Key);
            //x
            string rowName = (par as AngleConvertParam).StartXName;
            int index2 = Model.PositionDataList.IndexOf(rowName);
            Model.SelectStartXIndex = index2;

            //y

            string columnName = (par as AngleConvertParam).StartYName;
            int index3 = Model.PositionDataList.IndexOf(columnName);
            Model.SelectStartYIndex = index3;

            //x2

            string rowName2 = (par as AngleConvertParam).EndXName;
            int index4 = Model.PositionDataList.IndexOf(rowName2);
            Model.SelectEndXIndex = index4;

            //y2

            string columnName2 = (par as AngleConvertParam).EndYName;
            int index5 = Model.PositionDataList.IndexOf(columnName2);
            Model.SelectEndYIndex = index5;
        }

        private void cobxStartX_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectStartXName];

            BaseParam par = baseTool.GetParam();
            (par as AngleConvertParam).StartXName = Model.SelectStartXName;
        }

        private void cobxStartY_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectStartYName];

            BaseParam par = baseTool.GetParam();
            (par as AngleConvertParam).StartYName = Model.SelectStartYName;
        }

        private void cobxEndX_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectEndXName];

            BaseParam par = baseTool.GetParam();
            (par as AngleConvertParam).EndXName = Model.SelectEndXName;
        }

        private void cobxEndY_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectEndYName];

            BaseParam par = baseTool.GetParam();
            (par as AngleConvertParam).EndYName = Model.SelectEndYName;
        }

        /// <summary>
        /// 打开标定文件
        /// </summary>
        void btnOpenFile_Click()
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Calib Matrix"; // Default file name
            dialog.DefaultExt = ".tup"; // Default file extension
            dialog.Filter = "Calib Matrix (.tup)|*.tup"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                Model.CalibFilePath = filename;
                HOperatorSet.ReadTuple(filename, out HTuple hv_HomMat2D);
                //传递当前读取矩阵
                baseTool.GetParam().hv_HomMat2D = hv_HomMat2D;
            }
        }
        /// <summary>
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        void ShowData(BaseParam parDat)
        {
            convertWay = (parDat as AngleConvertParam).ConvertWay;
            Model.SelectConvertWayIndex = (int)convertWay;          
            Model.SelectStartXName = (parDat as AngleConvertParam).StartXName;
            Model.SelectStartYName = (parDat as AngleConvertParam).StartYName;
            Model.SelectEndXName = (parDat as AngleConvertParam).EndXName;
            Model.SelectEndYName = (parDat as AngleConvertParam).EndYName;
            Model.CalibFilePath = parDat.calibFilePath;

        }

        /// <summary>
        /// 参数保存
        /// </summary>
        void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            convertWay = (EumConvertWay)Model.SelectConvertWayIndex;
            (par as AngleConvertParam).ConvertWay = convertWay;
            (par as AngleConvertParam).StartXName = Model.SelectStartXName;
            (par as AngleConvertParam).StartYName = Model.SelectStartYName;
            (par as AngleConvertParam).EndXName = Model.SelectEndXName;
            (par as AngleConvertParam).EndYName = Model.SelectEndYName;
            par.calibFilePath = Model.CalibFilePath;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);

        }
        /// <summary>
        /// 手动测试
        /// </summary>
        void btnTest_Click()
        {
            BaseParam par = baseTool.GetParam();
            convertWay = (EumConvertWay)Model.SelectConvertWayIndex;
            (par as AngleConvertParam).ConvertWay = convertWay;        
            (par as AngleConvertParam).StartXName = Model.SelectStartXName;
            (par as AngleConvertParam).StartYName = Model.SelectStartYName;
            (par as AngleConvertParam).EndXName = Model.SelectEndXName;
            (par as AngleConvertParam).EndYName = Model.SelectEndYName;

            RunResult rlt = baseTool.Run();

            if (rlt.runFlag)
            {
                //更新结果表格数据
                UpdateResultView(new AngleConvertData(1,
                   (par as AngleConvertParam).Angle));
                Model.TestInfo = "坐标换算成功";
            }
            else
            {
                //更新结果表格数据
                UpdateResultView(new AngleConvertData(1, 0));
                Model.TestInfo = "坐标换算异常";
               
            }
        }
        /// <summary>
        /// 更新结果表格数据
        /// </summary>
        /// <param name="LineIntersectionData"></param>
        void UpdateResultView(AngleConvertData Data)
        {
            Model.DgResultOfAngleConvertList.Clear();
            Model.DgResultOfAngleConvertList.Add(Data);
        }
    }
}
