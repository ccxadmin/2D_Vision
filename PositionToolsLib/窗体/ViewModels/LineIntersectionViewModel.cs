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
    public class LineIntersectionViewModel : BaseViewModel
    {
        public static LineIntersectionViewModel This { get; set; }
        public LineIntersectionModel Model { get; set; }
        StuLineData lineData1;
        StuLineData lineData2;
        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase Line1SelectionChangedCommand { get; set; }
        public CommandBase Line2SelectionChangedCommand { get; set; }
        public CommandBase SaveButClickCommand { get; set; }
        public CommandBase TestButClickCommand { get; set; }

        public LineIntersectionViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new LineIntersectionModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
          
            #region  Command
            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            Line1SelectionChangedCommand = new CommandBase();
            Line1SelectionChangedCommand.DoExecute = new Action<object>((o) => cobxLineList_SelectedIndexChanged(o));
            Line1SelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            Line2SelectionChangedCommand = new CommandBase();
            Line2SelectionChangedCommand.DoExecute = new Action<object>((o) => cobxLine2List_SelectedIndexChanged(o));
            Line2SelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

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
        /// <summary>
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        void ShowData()
        {
            BaseParam par = baseTool.GetParam();
            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);
            string imageName = (par as LineIntersectionParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;
            Model.SelectImageName = (par as LineIntersectionParam).InputImageName;

            foreach (var s in dataManage.LineDataDic)
                Model.LineList.Add(s.Key);

            string lineName = (par as LineIntersectionParam).InputLineName;
            int index2 = Model.LineList.IndexOf(lineName);
            Model.SelectLine1Index = index2;


            string line2Name = (par as LineIntersectionParam).InputLine2Name;
            int index3 = Model.LineList.IndexOf(line2Name);
            Model.SelectLine2Index = index3;

            lineData1 = (par as LineIntersectionParam).LineData;
            lineData2 = (par as LineIntersectionParam).LineData2;
        }
        /// <summary>
        ///输入图像选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cobxImageList_SelectedIndexChanged(object value)
        {
            if (!LineIntersectionTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as LineIntersectionParam).InputImageName = Model.SelectImageName;
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
            (par as LineIntersectionParam).InputLineName = Model.SelectLine1Name;

            ExtendLine(lineData1.spRow, lineData1.spColumn, lineData1.epRow, lineData1.epColumn, 100,
                   out HTuple row1, out HTuple colomn1, out HTuple row2, out HTuple column2);

            HOperatorSet.GenContourPolygonXld(out HObject lineContour1, row1.TupleConcat(row2),
                  colomn1.TupleConcat(column2));

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
            (par as LineIntersectionParam).InputLine2Name = Model.SelectLine2Name;

            ExtendLine(lineData2.spRow, lineData2.spColumn, lineData2.epRow, lineData2.epColumn, 100,
                out HTuple row1, out HTuple colomn1, out HTuple row2, out HTuple column2);

            HOperatorSet.GenContourPolygonXld(out HObject lineContour2, row1.TupleConcat(row2),
                  colomn1.TupleConcat(column2));

            ShowTool.DispRegion(lineContour2, "green");
            ShowTool.AddregionBuffer(lineContour2, "green");
        }

        /// <summary>
        /// 参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {
            BaseParam par = baseTool.GetParam();
            (par as LineIntersectionParam).LineData = lineData1;
            (par as LineIntersectionParam).LineData2 = lineData2;
            (par as LineIntersectionParam).InputLineName = Model.SelectLine1Name;
            (par as LineIntersectionParam).InputLine2Name = Model.SelectLine2Name;
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
            OnSaveManageHandle?.Invoke(dataManage);
        }
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click()
        {

            BaseParam par = baseTool.GetParam();
            (par as LineIntersectionParam).LineData = lineData1;
            (par as LineIntersectionParam).LineData2 = lineData2;
            (par as LineIntersectionParam).InputLineName = Model.SelectLine1Name;
            (par as LineIntersectionParam).InputLine2Name = Model.SelectLine2Name;

            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {

                ShowTool.DispConcatedObj((par as LineIntersectionParam).OutputImg, EumCommonColors.green);
                ShowTool.AddConcatedObjBuffer((par as LineIntersectionParam).OutputImg, EumCommonColors.green);

                ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                //更新结果表格数据
                UpdateResultView(new LineIntersectionData(1,
                    (par as LineIntersectionParam).IntersectionColumn,
                    (par as LineIntersectionParam).IntersectionRow
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
        /// <summary>
        /// 更新直线交点结果表格数据
        /// </summary>
        /// <param name="LineIntersectionData"></param>
        void UpdateResultView(LineIntersectionData Data)
        {
            Model.LineIntersectionDataList.Clear();
            Model.LineIntersectionDataList.Add(Data);

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
