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
using System.Windows.Forms;
using VisionShowLib.UserControls;

namespace PositionToolsLib.窗体.ViewModels
{
    public class FitLineViewModel : BaseViewModel
    {
        public static FitLineViewModel This { get; set; }
        public FitLineModel Model { get; set; }

        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase StartXSelectionChangedCommand { get; set; }
        public CommandBase StartYSelectionChangedCommand { get; set; }
        public CommandBase EndXSelectionChangedCommand { get; set; }
        public CommandBase EndYSelectionChangedCommand { get; set; }
        public CommandBase SaveButClickCommand { get; set; }
        public CommandBase TestButClickCommand { get; set; }

        public FitLineViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new FitLineModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
                           
            #region Command
            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

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
            string imageName = (par as FitLineParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;
            Model.SelectImageName = (par as FitLineParam).InputImageName;

            foreach (var s in dataManage.PositionDataDic)
                Model.PositionDataList.Add(s.Key);
            //x
            string rowName = (par as FitLineParam).StartXName;
            int index2 = Model.PositionDataList.IndexOf(rowName);
            Model.SelectStartXIndex = index2;

            //y

            string columnName = (par as FitLineParam).StartYName;
            int index3 = Model.PositionDataList.IndexOf(columnName);
            Model.SelectStartYIndex = index3;

            //x2

            string rowName2 = (par as FitLineParam).EndXName;
            int index4 = Model.PositionDataList.IndexOf(rowName2);
            Model.SelectEndXIndex = index4;

            //y2

            string columnName2 = (par as FitLineParam).EndYName;
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
            if (!FitLineTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as FitLineParam).InputImageName = Model.SelectImageName;
        }


        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click()
        {

            BaseParam par = baseTool.GetParam();
            (par as FitLineParam).StartXName = Model.SelectStartXName;
            (par as FitLineParam).StartYName = Model.SelectStartYName;
            (par as FitLineParam).EndXName = Model.SelectEndXName;
            (par as FitLineParam).EndYName = Model.SelectEndYName;

            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {

                ShowTool.DispConcatedObj((par as FitLineParam).OutputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer((par as FitLineParam).OutputImg, EumCommonColors.green);

                ShowTool.DispMessage("OK", 10, width - 500, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - 500, "green", 100);
                //更新结果表格数据
                UpdateResultView(new FitLineResultData(1,
                    (par as FitLineParam).LineAngle
                 ));
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
        /// 更新结果表格数据
        /// </summary>
        /// <param name="LineIntersectionData"></param>
        void UpdateResultView(FitLineResultData Data)
        {
            Model.DgResultOfFitLineList.Clear();
            Model.DgResultOfFitLineList.Add(Data);
        }


        /// <summary>
        /// 参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as FitLineParam).StartXName = Model.SelectStartXName;
            (par as FitLineParam).StartYName = Model.SelectStartYName;
            (par as FitLineParam).EndXName = Model.SelectEndXName;
            (par as FitLineParam).EndYName = Model.SelectEndYName;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
            OnSaveManageHandle?.Invoke(dataManage);
        }

     

        private void cobxStartX_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectStartXName];
        
            BaseParam par = baseTool.GetParam();
            (par as FitLineParam).StartXName = Model.SelectStartXName;
        }

        private void cobxStartY_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectStartYName];
         
            BaseParam par = baseTool.GetParam();
            (par as FitLineParam).StartYName = Model.SelectStartYName;
        }

        private void cobxEndX_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectEndXName];
       
            BaseParam par = baseTool.GetParam();
            (par as FitLineParam).EndXName = Model.SelectEndXName;
        }

        private void cobxEndY_SelectedIndexChanged(object o)
        {
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectEndYName];
           
            BaseParam par = baseTool.GetParam();
            (par as FitLineParam).EndYName = Model.SelectEndYName;
        }


      
        /*---------------------------------外部方法导入---------------------------*/
        #region ----External procedures----
        static private void ExtendLine(HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2,
           HTuple hv_ExtendLength, out HTuple hv_RowStart, out HTuple hv_ColStart, out HTuple hv_RowEnd,
              out HTuple hv_ColEnd)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Phi = null;
            // Initialize local and output iconic variables 

            //获取该直线的位置信息
            HOperatorSet.AngleLx(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Phi);
            //line_position (Row1, Column1, Row2, Column2, RowCenter, ColCenter, Length, Phi)
            //********************生成延长线***********************
            //延长线长度（不精确）
            //ExtendLength := 200

            //起点
            hv_RowStart = hv_Row1 - ((((hv_Phi + 1.5707963)).TupleCos()) * hv_ExtendLength);
            hv_ColStart = hv_Column1 - ((((hv_Phi + 1.5707963)).TupleSin()) * hv_ExtendLength);
            //终点
            hv_RowEnd = hv_Row2 - ((((hv_Phi - 1.5707963)).TupleCos()) * hv_ExtendLength);
            hv_ColEnd = hv_Column2 - ((((hv_Phi - 1.5707963)).TupleSin()) * hv_ExtendLength);


            return;
        }
        #endregion

    }
}
