using ControlShareResources.Common;
using HalconDotNet;
using PositionToolsLib.参数;
using PositionToolsLib.工具;
using PositionToolsLib.窗体.Models;
using PositionToolsLib.窗体.Pages;
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
        List<DataOfResultShow> resultShowDataList = new List<DataOfResultShow>();//表格数据集合
        double xCoor;
        double yCoor;
        double angleCoor;
        EumOutputType currOutputType = EumOutputType.Location;
        public static ResultShowViewModel This { get; set; }
        public ResultShowModel Model { get; set; }
      
        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase NewMenuItemClickCommand { get; set; }
        public CommandBase DelMenuItemClickCommand { get; set; }
        public CommandBase OutputTypeSelectionChangedCommand { get; set; }
        public CommandBase SaveButClickCommand { get; set; }
        public CommandBase TestButClickCommand { get; set; }
        public CommandBase XCoorSelectionChangedCommand { get; set; }
        public CommandBase YCoorSelectionChangedCommand { get; set; }
        public CommandBase AngCoorSelectionChangedCommand { get; set; }

        public ResultShowViewModel(BaseTool tool) : base(tool)
        {
            dataManage = tool.GetManage();
            This = this;
            Model = new ResultShowModel();
            //图像控件      
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            Model.TitleName = baseTool.GetToolName();//工具名称
        
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

            XCoorSelectionChangedCommand = new CommandBase();
            XCoorSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxXCoorList_SelectedIndexChanged(o));
            XCoorSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            YCoorSelectionChangedCommand = new CommandBase();
            YCoorSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxYCoorList_SelectedIndexChanged(o));
            YCoorSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            AngCoorSelectionChangedCommand = new CommandBase();
            AngCoorSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxAngleCoorList_SelectedIndexChanged(o));
            AngCoorSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            OutputTypeSelectionChangedCommand = new CommandBase();
            OutputTypeSelectionChangedCommand.DoExecute = new Action<object>((o) => cobxOutputTypeList_SelectedIndexChanged(o));
            OutputTypeSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

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
        ///输入图像选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cobxImageList_SelectedIndexChanged(object value)
        {
            if (Model.SelectImageIndex == -1) return;
            if (!dataManage.imageBufDic.ContainsKey(Model.SelectImageName)) return;
            if (!OpeningTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
            imgBuf = dataManage.imageBufDic[Model.SelectImageName].Clone();
            ShowTool.ClearAllOverLays();
            ShowTool.DispImage(imgBuf);
            ShowTool.D_HImage = imgBuf;
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).InputImageName = Model.SelectImageName;
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
            string imageName = (par as ResultShowParam).InputImageName;
            int index = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index;
            currOutputType = (par as ResultShowParam).OutputType;
            Model.OutputTypeSelectIndex = (int)currOutputType;
            SelectOutputType(par);
            Model.SelectImageName = (par as ResultShowParam).InputImageName;
            Model.NmCoorX = (baseTool as ResultShowTool).inforCoorX;
            Model.NmCoorY = (baseTool as ResultShowTool).inforCoorY;
            Model.ShowInspectRegChecked = (baseTool as ResultShowTool).isShowInspectRegion;

            resultShowDataList = (par as ResultShowParam).ResultShowDataList;
            if (resultShowDataList == null) return;

            Model.DgDataOfResultShowList.Clear();
            GetToolNameList();
            foreach (var s in resultShowDataList)
            {
                if (dataManage.enumerableTooDic.Count <= 0) break;
                if (!dataManage.enumerableTooDic.Contains(s.ToolName)) continue;             
                Model.DgDataOfResultShowList.Add(new DgDataOfResultShow(s.Use,s.ToolName,s.ToolStatus));

            }
            if (Model.DgDataOfResultShowList.Count > 0)
                Model.DgDataSelectIndex = Model.DgDataOfResultShowList.Count - 1;
            
              
        }
        //输出结果类型选择
        void SelectOutputType(BaseParam par)
        {
            if (currOutputType == EumOutputType.Location)//输出类型为定位
            {
                Model.OutputLocationEnable = System.Windows.Visibility.Visible;
                Model.OutputSizeEnable = System.Windows.Visibility.Hidden;
                Model.OutputTrajectoryEnable = System.Windows.Visibility.Hidden;
                Model.OutputAOIEnable = System.Windows.Visibility.Hidden;
                //X
                foreach (var s in dataManage.PositionDataDic)
                    Model.XCoorList.Add(s.Key);
                string rowName = (par as ResultShowParam).InputXCoorName;
                int index2 = Model.XCoorList.IndexOf(rowName);
                Model.SelectXCoorIndex = index2;
                //Y
                foreach (var s in dataManage.PositionDataDic)
                    Model.YCoorList.Add(s.Key);
                string columnName = (par as ResultShowParam).InputYCoorName;
                int index3 = Model.YCoorList.IndexOf(columnName);
                Model.SelectYCoorIndex = index3;
                //angle
                foreach (var s in dataManage.PositionDataDic)
                    Model.AngCoorList.Add(s.Key);
                string angleName = (par as ResultShowParam).InputAngleCoorName;
                int index4 = Model.AngCoorList.IndexOf(angleName);
                Model.SelectAngCoorIndex = index4;
            }
            else if (currOutputType == EumOutputType.Trajectory)//输出类型为轨迹
            {
                GetTrajectoryToolNameList();
                Model.OutputLocationEnable = System.Windows.Visibility.Hidden;
                Model.OutputSizeEnable = System.Windows.Visibility.Hidden;
                Model.OutputTrajectoryEnable = System.Windows.Visibility.Visible;
                Model.OutputAOIEnable = System.Windows.Visibility.Hidden;
                List<OutputTypeOfTrajectory> Names = (par as ResultShowParam).TrajectoryNameList;
                Model.DgDataOfOutputTrajectoryList.Clear();
                foreach (var s in Names)
                    Model.DgDataOfOutputTrajectoryList.Add(new DgOutputTypeOfTrajectory
                        (s.ID, s.IsUse, s.ToolName));

            }
            else if (currOutputType == EumOutputType.Size)//输出类型为尺寸
            {
                GetSizeToolNameList();
                Model.OutputLocationEnable = System.Windows.Visibility.Hidden;
                Model.OutputSizeEnable = System.Windows.Visibility.Visible;
                Model.OutputTrajectoryEnable = System.Windows.Visibility.Hidden;
                Model.OutputAOIEnable = System.Windows.Visibility.Hidden;
                List<OutputTypeOfSize> Names = (par as ResultShowParam).SizeNameList;
                Model.DgDataOfOutputSizeList.Clear();
                foreach (var s in Names)
                    Model.DgDataOfOutputSizeList.Add(new DgOutputTypeOfSize
                        (s.ID, s.IsUse, s.ToolName));
            }
            else//输出类型为AOI
            {
                GetAoiToolNameList();
                Model.OutputLocationEnable = System.Windows.Visibility.Hidden;
                Model.OutputSizeEnable = System.Windows.Visibility.Hidden;
                Model.OutputTrajectoryEnable = System.Windows.Visibility.Hidden;
                Model.OutputAOIEnable = System.Windows.Visibility.Visible;
                List<OutputTypeOfAoi> Names = (par as ResultShowParam).AoiNameList;
                Model.DgDataOfOutputSizeList.Clear();
                foreach (var s in Names)
                    Model.DgDataOfOutputAoiList.Add(new DgOutputTypeOfAoi
                        (s.ID, s.IsUse, s.ToolName));
            }
        }
        private void 新增toolStripMenuItem_Click()
        {
            //新增时就更新表格
            //GetToolNameList();
            DgDataOfResultShow dat = new DgDataOfResultShow(false, "", "");
            resultShowDataList.Add(new DataOfResultShow(false, "", ""));
            Model.DgDataOfResultShowList.Add(dat);
            Model.DgDataSelectIndex = Model.DgDataOfResultShowList.Count - 1;
           
        }
        void GetToolNameList()
        {
            Model.ToolNameList.Clear();
            foreach (var s in dataManage.enumerableTooDic)
                Model.ToolNameList.Add(s);
        }
        /// <summary>
        /// 获取当前轨迹工具名称
        /// </summary>
        void GetTrajectoryToolNameList()
        {
            BaseParam par = baseTool.GetParam();
            List<OutputTypeOfTrajectory> temList = new List<OutputTypeOfTrajectory>();
            //DataManage.DeepCopy2<List<OutputTypeOfTrajectory>>((par as ResultShowParam).TrajectoryNameList);
            //(par as ResultShowParam).TrajectoryNameList.Clear();
            int index = 0;
            foreach (var s in dataManage.trajectoryTooDic)
            {
                int n_index = (par as ResultShowParam).TrajectoryNameList.
                       FindIndex(t => t.ToolName == s);
                if (n_index == -1) //不存在
                    temList.Add(new OutputTypeOfTrajectory(index, false, s));
                else
                    temList.Add(new OutputTypeOfTrajectory(index,
                          (par as ResultShowParam).TrajectoryNameList[n_index].IsUse,
                          s));
                index++;
            }
            (par as ResultShowParam).TrajectoryNameList = temList;
        }

        /// <summary>
        /// 获取当前尺寸工具名称
        /// </summary>
        void GetSizeToolNameList()
        {
            BaseParam par = baseTool.GetParam();
            List<OutputTypeOfSize> temList = new List<OutputTypeOfSize>();
            //DataManage.DeepCopy2<List<OutputTypeOfTrajectory>>((par as ResultShowParam).TrajectoryNameList);
            //(par as ResultShowParam).TrajectoryNameList.Clear();
            int index = 0;
            foreach (var s in dataManage.sizeTooDic)
            {
                int n_index = (par as ResultShowParam).SizeNameList.
                       FindIndex(t => t.ToolName == s);
                if (n_index == -1) //不存在
                    temList.Add(new OutputTypeOfSize(index, false, s));
                else
                    temList.Add(new OutputTypeOfSize(index,
                          (par as ResultShowParam).SizeNameList[n_index].IsUse,
                          s));
                index++;
            }
            (par as ResultShowParam).SizeNameList = temList;
        }

        /// <summary>
        /// 获取当前Aoi工具名称
        /// </summary>
        void GetAoiToolNameList()
        {
            BaseParam par = baseTool.GetParam();
            List<OutputTypeOfAoi> temList = new List<OutputTypeOfAoi>();
            //DataManage.DeepCopy2<List<OutputTypeOfTrajectory>>((par as ResultShowParam).TrajectoryNameList);
            //(par as ResultShowParam).TrajectoryNameList.Clear();
            int index = 0;
            foreach (var s in dataManage.aoiTooDic)
            {
                int n_index = (par as ResultShowParam).AoiNameList.
                       FindIndex(t => t.ToolName == s);
                if (n_index == -1) //不存在
                    temList.Add(new OutputTypeOfAoi(index, false, s));
                else
                    temList.Add(new OutputTypeOfAoi(index,
                          (par as ResultShowParam).AoiNameList[n_index].IsUse,
                          s));
                index++;
            }
            (par as ResultShowParam).AoiNameList = temList;
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
                DataOfResultShow dat = new DataOfResultShow(isUse, toolName, status);
                resultShowDataList.Add(dat);
            }
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).ResultShowDataList = resultShowDataList;
            //定位
            (par as ResultShowParam).InputXCoorName = Model.SelectXCoorName;
            (par as ResultShowParam).InputYCoorName = Model.SelectYCoorName;
            (par as ResultShowParam).InputAngleCoorName = Model.SelectAngCoorName;
            StuCoordinateData data = new StuCoordinateData(xCoor, yCoor, angleCoor);
            (par as ResultShowParam).CoordinateData = data;
            //轨迹
            List<OutputTypeOfTrajectory> Names = (par as ResultShowParam).TrajectoryNameList;
            Names.Clear();
            foreach (var s in Model.DgDataOfOutputTrajectoryList)
                Names.Add(new OutputTypeOfTrajectory
                    (s.ID, s.IsUse, s.ToolName));
            (par as ResultShowParam).TrajectoryNameList = Names;

            //尺寸
            List<OutputTypeOfSize> SizeNames = (par as ResultShowParam).SizeNameList;
            SizeNames.Clear();
            foreach (var s in Model.DgDataOfOutputSizeList)
                SizeNames.Add(new OutputTypeOfSize
                    (s.ID, s.IsUse, s.ToolName));
            (par as ResultShowParam).SizeNameList = SizeNames;

            //AOI
            List<OutputTypeOfAoi> AoiNames = (par as ResultShowParam).AoiNameList;
            AoiNames.Clear();
            foreach (var s in Model.DgDataOfOutputAoiList)
                AoiNames.Add(new OutputTypeOfAoi
                    (s.ID, s.IsUse, s.ToolName));
            (par as ResultShowParam).AoiNameList = AoiNames;

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
                DataOfResultShow dat = new DataOfResultShow(isUse, toolName, status);
                resultShowDataList.Add(dat);
            }
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).ResultShowDataList = resultShowDataList;
            //输出定位结果
            (par as ResultShowParam).InputXCoorName = Model.SelectXCoorName;
            (par as ResultShowParam).InputYCoorName = Model.SelectYCoorName;
            (par as ResultShowParam).InputAngleCoorName = Model.SelectAngCoorName;
            //输出轨迹结果
            List<OutputTypeOfTrajectory> Names = (par as ResultShowParam).TrajectoryNameList;
            Names.Clear();
            foreach (var s in Model.DgDataOfOutputTrajectoryList)
                Names.Add(new OutputTypeOfTrajectory
                    (s.ID, s.IsUse, s.ToolName));
            (par as ResultShowParam).TrajectoryNameList = Names;
            //尺寸
            List<OutputTypeOfSize> SizeNames = (par as ResultShowParam).SizeNameList;
            SizeNames.Clear();
            foreach (var s in Model.DgDataOfOutputSizeList)
                SizeNames.Add(new OutputTypeOfSize
                    (s.ID, s.IsUse, s.ToolName));
            (par as ResultShowParam).SizeNameList = SizeNames;
            //AOI
            List<OutputTypeOfAoi> AoiNames = (par as ResultShowParam).AoiNameList;
            AoiNames.Clear();
            foreach (var s in Model.DgDataOfOutputAoiList)
                AoiNames.Add(new OutputTypeOfAoi
                    (s.ID, s.IsUse, s.ToolName));
            (par as ResultShowParam).AoiNameList = AoiNames;

            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispImage((par as ResultShowParam).OutputImg);
                ShowTool.D_HImage = (par as ResultShowParam).OutputImg;
                ShowTool.DispMessage("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - (width / 1000 + 1) * 200, "green", 100);

                ShowTool.DispRegion((par as ResultShowParam).ResultRegion, "green");
                ShowTool.AddregionBuffer((par as ResultShowParam).ResultRegion, "green");
                StuCoordinateData data = (par as ResultShowParam).CoordinateData;
                List<DgTrajectoryData> datas = (par as ResultShowParam).TrajectoryDataList;
                List<double> diatances = (par as ResultShowParam).Distances;
                List<StuFlagInfo> info = (par as ResultShowParam).ResultInfo;
                bool aoiCheckFlag = (par as ResultShowParam).AoiResultFlag;

                int num = info.Count;
                for (int i = 0; i < num; i++)
                {
                    ShowTool.DispMessage(info[i].info, 10 + i * 150, 10, info[i].flag ? "green" : "red", 16);
                    ShowTool.AddTextBuffer(info[i].info, 10 + i * 150, 10, info[i].flag ? "green" : "red", 16);
                }
                if ((par as ResultShowParam).OutputType == EumOutputType.Location)
                {
                    ShowTool.DispMessage(string.Format("特征点坐标,x:{0:f3},y:{1:f3},a:{2:f3}", data.x, data.y, data.angle),
                  10 + num * 150, 10, "green", 16);
                    ShowTool.AddTextBuffer(string.Format("特征点坐标,x:{0:f3},y:{1:f3},a:{2:f3}", data.x, data.y, data.angle),
                        10 + num * 150, 10, "green", 16);
                    Model.DgResultOfResultShowList.Clear();
                    Model.DgResultOfResultShowList.Add(new DgResultOfResultShow(1,
                        data.x, data.y, data.angle));
                }
                else if ((par as ResultShowParam).OutputType == EumOutputType.Trajectory)
                {
                    int indx = 0;
                    Model.DgResultOfResultShowList.Clear();
                    foreach (var s in datas)
                    {
                        ShowTool.DispMessage(string.Format("轨迹点坐标,id:{0},x:{1:f3},y:{2:f3},r:{3:f3}", s.ID, s.X, s.Y, s.Radius),
                             10 + (num + indx) * 150, 10, "green", 16);
                        ShowTool.AddTextBuffer(string.Format("轨迹点坐标,id:{0},x:{1:f3},y:{2:f3},r:{3:f3}", s.ID, s.X, s.Y, s.Radius),
                            10 + (num + indx) * 150, 10, "green", 16);
                        Model.DgResultOfResultShowList.Add(new DgResultOfResultShow(s.ID,
                          s.X, s.Y, (float)s.Radius));
                        indx++;
                    }


                }
                else if ((par as ResultShowParam).OutputType == EumOutputType.Size)
                {
                    
                    int indx = 0;
                    Model.DgResultOfResultShowList.Clear();
                    foreach (var s in diatances)
                    {
                        ShowTool.DispMessage(string.Format("尺寸,id:{0},distance:{1:f3}", indx, s),
                             10 + (num + indx) * 150, 10, "green", 16);
                        ShowTool.AddTextBuffer(string.Format("尺寸,id:{0},distance:{1:f3}", indx, s),
                            10 + (num + indx) * 150, 10, "green", 16);
                        Model.DgResultOfResultShowList.Add(new DgResultOfResultShow(indx, s));
                        indx++;
                    }

                }
                else if ((par as ResultShowParam).OutputType == EumOutputType.AOI)
                {

                    int indx = 0;
                    Model.DgResultOfResultShowList.Clear();
                    ShowTool.DispMessage(string.Format("AOI,id:{0},result:{1}", indx, aoiCheckFlag ? "OK" : "NG"),
                               10 + (num + indx) * 150, 10, "green", 16);
                    ShowTool.AddTextBuffer(string.Format("AOI,id:{0},result:{1}", indx, aoiCheckFlag?"OK":"NG"),
                               10 + (num + indx) * 150, 10, "green", 16);
                    Model.DgResultOfResultShowList.Add(new DgResultOfResultShow(indx, aoiCheckFlag));

                }
                //HOperatorSet.GenCrossContourXld(out HObject cross, data.y, data.x, 20, data.angle);
                //if (BaseTool.ObjectValided(cross))
                //{
                //    ShowTool.DispRegion(cross, "red");
                //    ShowTool.AddregionBuffer(cross, "red");
                //}

            }
            else
            {
                ShowTool.DispImage(imgBuf);
                ShowTool.DispMessage("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.AddTextBuffer("NG", 10, width - (width / 1000 + 1) * 200, "red", 100);
                ShowTool.DispAlarmMessage(rlt.errInfo, 100, 10, 12);


            //    ShowTool.DispMessage("特征点坐标：0,0,0", 10, 10, "red", 16);
            //    ShowTool.AddTextBuffer("特征点坐标：0,0,0", 10, 10, "red", 16);
            }
        }

        private void cobxXCoorList_SelectedIndexChanged(object o)
        {
            if (Model.SelectXCoorName == "") return;
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectXCoorName];
             xCoor = data.x;
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).InputXCoorName = Model.SelectXCoorName;
        }

        private void cobxYCoorList_SelectedIndexChanged(object o)
        {
            if (Model.SelectYCoorName == "") return;
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectYCoorName];
            yCoor = data.y;
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).InputYCoorName = Model.SelectYCoorName;
        }

        private void cobxAngleCoorList_SelectedIndexChanged(object o)
        {
            if (Model.SelectAngCoorName == "") return;
            StuCoordinateData data = dataManage.PositionDataDic[Model.SelectAngCoorName];
            angleCoor = data.angle;
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).InputAngleCoorName = Model.SelectAngCoorName;
        }
        /// <summary>
        /// 输出结果类型切换
        /// </summary>
        /// <param name="o"></param>
        private void cobxOutputTypeList_SelectedIndexChanged(object o)
        {
            if (Model.OutputTypeSelectIndex == -1) return;
            BaseParam par = baseTool.GetParam();
            (par as ResultShowParam).OutputType= currOutputType = (EumOutputType)Model.OutputTypeSelectIndex;
            SelectOutputType(par);


        }
    }
}
