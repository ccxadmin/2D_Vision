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
using System.Windows.Input;

namespace PositionToolsLib.窗体.ViewModels
{
    public class ResultShowViewModel : BaseViewModel
    {
        List<DgDataOfResultShow> resultShowDataList = new List<DgDataOfResultShow>();//表格数据集合
        double rowCoor;
        double colCoor;
        double angleCoor;
        public static ResultShowViewModel This { get; set; }
        public ResultShowModel Model { get; set; }
      
        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase NewMenuItemClickCommand { get; set; }
        public CommandBase DelMenuItemClickCommand { get; set; }
     
        public CommandBase SaveButClickCommand { get; set; }
        public CommandBase TestButClickCommand { get; set; }
        public CommandBase RowCoorSelectionChangedCommand { get; set; }
        public CommandBase ColCoorSelectionChangedCommand { get; set; }
        public CommandBase AngCoorSelectionChangedCommand { get; set; }

        public ResultShowViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new ResultShowModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
            BaseParam par = baseTool.GetParam();
            ShowData(par);

            ImageSelectionChangedCommand = new CommandBase();
            ImageSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxImageList_SelectedIndexChanged(o));
            ImageSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            NewMenuItemClickCommand = new CommandBase();
            NewMenuItemClickCommand.DoExecute = new Action<object>((o) => 新增toolStripMenuItem_Click());
            NewMenuItemClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DelMenuItemClickCommand = new CommandBase();
            DelMenuItemClickCommand.DoExecute = new Action<object>((o) => 删除toolStripMenuItem_Click());
            DelMenuItemClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

           
            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TestButClickCommand = new CommandBase();
            TestButClickCommand.DoExecute = new Action<object>((o) => btnTest_Click());
            TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            RowCoorSelectionChangedCommand = new CommandBase();
            RowCoorSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxRowCoorList_SelectedIndexChanged(o));
            RowCoorSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ColCoorSelectionChangedCommand = new CommandBase();
            ColCoorSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxColCoorList_SelectedIndexChanged(o));
            ColCoorSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            AngCoorSelectionChangedCommand = new CommandBase();
            AngCoorSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxAngleCoorList_SelectedIndexChanged(o));
            AngCoorSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });



            foreach (var s in dataManage.imageBufDic)
                Model.ImageList.Add(s.Key);
            string imageName = (par as ResultShowParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;
            //row
            foreach (var s in dataManage.PositionDataDic)
                Model.RowCoorList.Add(s.Key);
            string rowName = (par as ResultShowParam).InputRowCoorName;
            int index2 = Model.RowCoorList.IndexOf(rowName);
            Model.SelectRowCoorIndex = index2;
            //column
            foreach (var s in dataManage.PositionDataDic)
                Model.ColCoorList.Add(s.Key);
            string columnName = (par as ResultShowParam).InputColCoorName;
            int index3 = Model.ColCoorList.IndexOf(columnName);
            Model.SelectColCoorIndex = index3;
            //angle
            foreach (var s in dataManage.PositionDataDic)
                Model.AngCoorList.Add(s.Key);
            string angleName = (par as ResultShowParam).InputAngleCoorName;
            int index4 = Model.AngCoorList.IndexOf(angleName);
            Model.SelectAngCoorIndex = index4;
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
        /// 数据显示
        /// </summary>
        /// <param name="parDat"></param>
        void ShowData(BaseParam parDat)
        {
        
            Model.NmCoorX = (baseTool as ResultShowTool).inforCoorX;
            Model.NmCoorY = (baseTool as ResultShowTool).inforCoorY;
            Model.ShowInspectRegChecked = (baseTool as ResultShowTool).isShowInspectRegion;

            resultShowDataList = (parDat as ResultShowParam).ResultShowDataList;
            if (resultShowDataList == null) return;

            Model.DgDataOfResultShowList.Clear();
            GetToolNameList();
            foreach (var s in resultShowDataList)
            {
                if (dataManage.enumerableTooDic.Count <= 0) break;
                if (!dataManage.enumerableTooDic.Contains(s.ToolName)) continue;
                DgDataOfResultShow dat = new DgDataOfResultShow(s.Use, s.ToolName, s.ToolStatus);
                Model.DgDataOfResultShowList.Add(dat);

            }
            if (Model.DgDataOfResultShowList.Count > 0)
                Model.DgDataSelectIndex = Model.DgDataOfResultShowList.Count - 1;
        }


        private void 新增toolStripMenuItem_Click()
        {
            //新增时就更新表格
            GetToolNameList();
            DgDataOfResultShow dat = new DgDataOfResultShow(false, "", "");
            resultShowDataList.Add(dat);
            Model.DgDataOfResultShowList.Add(dat);
            Model.DgDataSelectIndex = Model.DgDataOfResultShowList.Count - 1;
           
        }
        void GetToolNameList()
        {
            Model.ToolNameList.Clear();
            foreach (var s in dataManage.enumerableTooDic)
                Model.ToolNameList.Add(s);
        }
        private void 删除toolStripMenuItem_Click()
        {
            if (Model.DgDataSelectIndex >= 0)
            {
            
                resultShowDataList.RemoveAt(Model.DgDataSelectIndex);
                Model.DgDataOfResultShowList.RemoveAt(Model.DgDataSelectIndex);
            }

        }
 
        /// <summary>
        /// 参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {
            (baseTool as ResultShowTool).inforCoorX = Model.NmCoorX;
            (baseTool as ResultShowTool).inforCoorY = Model.NmCoorY;
            (baseTool as ResultShowTool).isShowInspectRegion =
               Model.ShowInspectRegChecked;
            resultShowDataList.Clear();
            int count = Model.DgDataOfResultShowList.Count;

            for (int i = 0; i < count; i++)
            {
                bool isUse = Model.DgDataOfResultShowList[i].Use;
                string toolName = Model.DgDataOfResultShowList[i].ToolName;
                string status = Model.DgDataOfResultShowList[i].ToolStatus;
                DgDataOfResultShow dat = new DgDataOfResultShow(isUse, toolName, status);
                resultShowDataList.Add(dat);
            }
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).ResultShowDataList = resultShowDataList;
            (par as ResultShowParam).InputRowCoorName = Model.SelectRowCoorName;
            (par as ResultShowParam).InputColCoorName = Model.SelectColCoorName;
            (par as ResultShowParam).InputAngleCoorName = Model.SelectAngCoorName;
            StuCoordinateData data = new StuCoordinateData(rowCoor, colCoor, angleCoor);
            (par as ResultShowParam).CoordinateData = data;
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
            resultShowDataList.Clear();
            int count = Model.DgDataOfResultShowList.Count;

            for (int i = 0; i < count; i++)
            {
                bool isUse = Model.DgDataOfResultShowList[i].Use;
                string toolName = Model.DgDataOfResultShowList[i].ToolName;
                string status = Model.DgDataOfResultShowList[i].ToolStatus;
                DgDataOfResultShow dat = new DgDataOfResultShow(isUse, toolName, status);
                resultShowDataList.Add(dat);
            }
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).ResultShowDataList = resultShowDataList;
            (par as ResultShowParam).InputRowCoorName = Model.SelectRowCoorName;
            (par as ResultShowParam).InputColCoorName = Model.SelectColCoorName;
            (par as ResultShowParam).InputAngleCoorName = Model.SelectAngCoorName;

            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispImage((par as ResultShowParam).OutputImg);
                ShowTool.D_HImage = (par as ResultShowParam).OutputImg;
                ShowTool.DispMessage("OK", 10, width - 600, "green", 50);
                ShowTool.AddTextBuffer("OK", 10, width - 600, "green", 50);

                ShowTool.DispRegion((par as ResultShowParam).ResultRegion, "green");
                ShowTool.AddregionBuffer((par as ResultShowParam).ResultRegion, "green");
                StuCoordinateData data = (par as ResultShowParam).CoordinateData;
                List<StuFlagInfo> info = (par as ResultShowParam).ResultInfo;
                int num = info.Count;
                for (int i = 0; i < num; i++)
                {
                    ShowTool.DispMessage(info[i].info, 10 + i * 150, 10, info[i].flag ? "green" : "red", 16);
                    ShowTool.AddTextBuffer(info[i].info, 10 + i * 150, 10, info[i].flag ? "green" : "red", 16);
                }

                ShowTool.DispMessage(string.Format("特征点坐标,x:{0:f3},y:{1:f3},a:{2:f3}", data.column, data.row, data.angle),
                    10 + num * 150, 10, "green", 16);
                ShowTool.AddTextBuffer(string.Format("特征点坐标,x:{0:f3},y:{1:f3},a:{2:f3}", data.column, data.row, data.angle),
                    10 + num * 150, 10, "green", 16);

                HOperatorSet.GenCrossContourXld(out HObject cross, data.row, data.column, 20, data.angle);
                if (BaseTool.ObjectValided(cross))
                {
                    ShowTool.DispRegion(cross, "red");
                    ShowTool.AddregionBuffer(cross, "red");
                }
            }
            else
            {
                ShowTool.DispImage(imgBuf);
                ShowTool.DispMessage("NG", 10, width - 600, "red", 50);
                ShowTool.AddTextBuffer("NG", 10, width - 600, "red", 50);
                ShowTool.DispAlarmMessage(rlt.errInfo, 100, 10, 12);
                ShowTool.DispMessage("特征点坐标：0,0,0", 10, 10, "red", 16);
                ShowTool.AddTextBuffer("特征点坐标：0,0,0", 10, 10, "red", 16);
            }
        }

        private void cobxRowCoorList_SelectedIndexChanged(object o)
        {
            if (Model.SelectRowCoorName == "") return;
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectRowCoorName];
            rowCoor = data.row;
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).InputRowCoorName = Model.SelectRowCoorName;
        }

        private void cobxColCoorList_SelectedIndexChanged(object o)
        {
            if (Model.SelectColCoorName == "") return;
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectColCoorName];
            colCoor = data.column;
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).InputColCoorName = Model.SelectColCoorName;
        }

        private void cobxAngleCoorList_SelectedIndexChanged(object o)
        {
            if (Model.SelectAngCoorName == "") return;
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectAngCoorName];
            angleCoor = data.angle;
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).InputAngleCoorName = Model.SelectAngCoorName;
        }


    }
}
