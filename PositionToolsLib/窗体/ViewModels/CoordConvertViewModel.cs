using ControlShareResources.Common;
using FilesRAW.DataBase;
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
    public class CoordConvertViewModel : BaseViewModel
    {
        public static CoordConvertViewModel This { get; set; }
        public CoordConvertModel Model { get; set; }
        EumConvertWay convertWay = EumConvertWay.ToPhysical;


        public CommandBase CoordXSelectionChangedCommand { get; set; }
        public CommandBase CoordYSelectionChangedCommand { get; set; }
        public CommandBase OpenFileCommand { get; set; }
        //保存
        public CommandBase SaveButClickCommand { get; set; }
        //测试
        public CommandBase TestButClickCommand { get; set; }


        public CoordConvertViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new CoordConvertModel();
            //图像控件      
            //ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
            BaseParam par = baseTool.GetParam();
            ShowData(par);


            CoordXSelectionChangedCommand = new CommandBase();
            CoordXSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxCoordXList_SelectedIndexChanged(o));
            CoordXSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
           
            CoordYSelectionChangedCommand = new CommandBase();
            CoordYSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxCoordYList_SelectedIndexChanged(o));
            CoordYSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


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
            string xName = (par as CoordConvertParam).CoordXName;
            int index2 = Model.PositionDataList.IndexOf(xName);
            Model.SelectCoordXIndex = index2;

            //y
            string yName = (par as CoordConvertParam).CoordYName;
            int index3 = Model.PositionDataList.IndexOf(yName);
            Model.SelectCoordYIndex = index3;
        }

        private void cobxCoordXList_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectCoordXName];

            BaseParam par = baseTool.GetParam();
            (par as CoordConvertParam).CoordXName = Model.SelectCoordXName;
        }
        private void cobxCoordYList_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectCoordYName];

            BaseParam par = baseTool.GetParam();
            (par as CoordConvertParam).CoordYName = Model.SelectCoordYName;
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
                HOperatorSet.ReadTuple(filename, out  HTuple hv_HomMat2D);
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
            convertWay = (parDat as CoordConvertParam).ConvertWay;
            Model.SelectConvertWayIndex = (int)convertWay;
            Model.SelectCoordXName = (parDat as CoordConvertParam).CoordXName;
            Model.SelectCoordYName = (parDat as CoordConvertParam).CoordYName;  
            Model.CalibFilePath = parDat.calibFilePath;
        }
     
        /// <summary>
        /// 参数保存
        /// </summary>
        void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            convertWay=(EumConvertWay)Model.SelectConvertWayIndex;
            (par as CoordConvertParam).ConvertWay = convertWay;
            (par as CoordConvertParam).CoordXName = Model.SelectCoordXName;
            (par as CoordConvertParam).CoordYName = Model.SelectCoordYName;
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
            (par as CoordConvertParam).ConvertWay = convertWay;
            (par as CoordConvertParam).CoordXName = Model.SelectCoordXName;
            (par as CoordConvertParam).CoordYName = Model.SelectCoordYName;

            RunResult rlt = baseTool.Run();
           
            if (rlt.runFlag)
            {
                //更新结果表格数据
                UpdateResultView(new CoordConvertData(1,
                   (par as CoordConvertParam).ConvertedX,
                   (par as CoordConvertParam).ConvertedY));
                Model.TestInfo = "坐标换算成功";
            }
            else
            {
                //更新结果表格数据
                UpdateResultView(new CoordConvertData(1, 0, 0)) ;
                Model.TestInfo = "坐标换算异常";
             
            }
        }
        /// <summary>
        /// 更新结果表格数据
        /// </summary>
        /// <param name="LineIntersectionData"></param>
        void UpdateResultView(CoordConvertData Data)
        {
            Model.DgResultOfCoordConvertList.Clear();
            Model.DgResultOfCoordConvertList.Add(Data);
        }
    }
}
