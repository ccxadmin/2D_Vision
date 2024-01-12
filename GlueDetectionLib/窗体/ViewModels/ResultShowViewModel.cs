using ControlShareResources.Common;
using GlueDetectionLib.参数;
using GlueDetectionLib.工具;
using GlueDetectionLib.窗体.Models;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GlueDetectionLib.窗体.ViewModels
{
    public class ResultShowViewModel : BaseViewModel
    {
        public static ResultShowViewModel This { get; set; }
        public ResultShowModel Model { get; set; }
        List<DgDataOfResultShow> resultShowDataList = new List<DgDataOfResultShow>();//表格数据集合
        //图像源选择
        public CommandBase ImageSelectionChangedCommand { get; set; }
        public CommandBase NewMenuItemClickCommand { get; set; }
        public CommandBase DelMenuItemClickCommand { get; set; }
        public CommandBase GlueNameSelectionChangedCommand { get; set; }
        public CommandBase NumStartPxKeyDownCommand { get; set; }
        public CommandBase NumStartPyKeyDownCommand { get; set; }
        public CommandBase SaveButClickCommand { get; set; }
        public CommandBase TestButClickCommand { get; set; }


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

            GlueNameSelectionChangedCommand = new CommandBase();
            GlueNameSelectionChangedCommand.DoExecute = new Action<object>((o) => comboBox1_SelectedIndexChanged(o));
            GlueNameSelectionChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            NumStartPxKeyDownCommand = new CommandBase();
            NumStartPxKeyDownCommand.DoExecute = new Action<object>((o) => NumStartPxKeyDown(o));
            NumStartPxKeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            NumStartPyKeyDownCommand = new CommandBase();
            NumStartPyKeyDownCommand.DoExecute = new Action<object>((o) => NumStartPyKeyDown(o));
            NumStartPyKeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveParam_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TestButClickCommand = new CommandBase();
            TestButClickCommand.DoExecute = new Action<object>((o) => btnTest_Click());
            TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            
            Model.NumStartPxCommand = new Action(() => NumStartPxEvent());
            Model.NumStartPxCommand = new Action(() => NumStartPyEvent());


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
            if (!ResultShowTool.ObjectValided(dataManage.imageBufDic[Model.SelectImageName])) return;
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
            int index1 = Model.ImageList.IndexOf(imageName);
            Model.SelectImageIndex = index1;
            Model.SelectImageName= (par as ResultShowParam).InputImageName;
            Model.NmCoorX = (baseTool as ResultShowTool).inforCoorX;
            Model.NmCoorY = (baseTool as ResultShowTool).inforCoorY;
            Model.ShowInspectRegChecked = (baseTool as ResultShowTool).isShowInspectRegion;

            //numGlueInfoX.Value= (baseTool as ResultShowTool).glueInfoCoorX;
            //numGlueInfoY.Value = (baseTool as ResultShowTool).glueInfoCoorY;
            //numGlueInfoOffsetX.Value = (baseTool as ResultShowTool).glueInfoOffetX;
            //numGlueInfoOffsetY.Value = (baseTool as ResultShowTool).glueInfoOffetY;


            resultShowDataList = (par as ResultShowParam).ResultShowDataList;
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
            //if(dataGridViewEx1.RowCount>0)
            //    dataGridViewEx1.Rows[resultShowDataList.Count - 1].Selected = true;

            //胶水坐标信息
            Model.CobxGlueNameEnable = false;
            if (dataManage.enumerableTooDic.Count > 0)
            {
                int index = 1;
                //List<string> GlueNames = new List<string>();
                Model.GlueNameList.Clear();
                if ((baseTool as ResultShowTool).GlueInfoDic == null)
                    (baseTool as ResultShowTool).GlueInfoDic = new Dictionary<string, CoorditionDat>();

                foreach (var s in dataManage.enumerableTooDic)
                {
                    if (s.Contains("漏胶"))
                    {
                        if (!(baseTool as ResultShowTool).GlueInfoDic
                                   .ContainsKey(string.Format("R{0}", index)))
                            (baseTool as ResultShowTool).GlueInfoDic.Add(
                                  string.Format("R{0}", index), new CoorditionDat());
                        if (!Model.GlueNameList.Contains(string.Format("R{0}", index)))
                            Model.GlueNameList.Add(string.Format("R{0}", index));
                        index++;
                       
                    }


                }
                if(Model.GlueNameList.Count>0)
                {
                    Model.SelectGlueIndex = 0;
                    Model.SelectGlueName = "R1";
                    comboBox1_SelectedIndexChanged(null);
                }
                     
                Model.CobxGlueNameEnable = true;
             
            }

        }

        private void 新增toolStripMenuItem_Click()
        {
            ////新增时就更新表格
            //GetToolNameList();
            DgDataOfResultShow dat = new DgDataOfResultShow(false, "", "");
            resultShowDataList.Add(dat);
            Model.DgDataOfResultShowList.Add(dat);
            Model.DgDataSelectIndex = Model.DgDataOfResultShowList.Count - 1;
          
        }
       void  NumStartPxEvent()
        {
            if ((baseTool as ResultShowTool).GlueInfoDic.ContainsKey(Model.SelectGlueName))
            {
                CoorditionDat pos =
                    (baseTool as ResultShowTool).GlueInfoDic[Model.SelectGlueName];
                pos.Column = Model.NumStartPx;

                (baseTool as ResultShowTool).GlueInfoDic[Model.SelectGlueName] = pos;
            }
           
        }
        void NumStartPyEvent()
        {
            if ((baseTool as ResultShowTool).GlueInfoDic.ContainsKey(Model.SelectGlueName))
            {
                CoorditionDat pos =
                    (baseTool as ResultShowTool).GlueInfoDic[Model.SelectGlueName];
                pos.Column = Model.NumStartPx;

                (baseTool as ResultShowTool).GlueInfoDic[Model.SelectGlueName] = pos;
            }

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

        private void comboBox1_SelectedIndexChanged(object o)
        {
            if ((baseTool as ResultShowTool).GlueInfoDic.ContainsKey(Model.SelectGlueName))
            {
                CoorditionDat pos =
                    (baseTool as ResultShowTool).GlueInfoDic[Model.SelectGlueName];
                Model.NumStartPx = (int)pos.Column;
                Model.NumStartPy= (int)pos.Row;

            }
        }

        void NumStartPxKeyDown(object obj)
        {
            KeyEventArgs args = (KeyEventArgs)obj;
            if (args.Key == Key.Enter)
            {
                if ((baseTool as ResultShowTool).GlueInfoDic.ContainsKey(Model.SelectGlueName))
                {
                    CoorditionDat pos =
                        (baseTool as ResultShowTool).GlueInfoDic[Model.SelectGlueName];
                    pos.Column = Model.NumStartPx;

                    (baseTool as ResultShowTool).GlueInfoDic[Model.SelectGlueName] = pos;
                }
            }
        }
        void NumStartPyKeyDown(object obj)
        {
            KeyEventArgs args = (KeyEventArgs)obj;
            if (args.Key == Key.Enter)
            {
                if ((baseTool as ResultShowTool).GlueInfoDic.ContainsKey(Model.SelectGlueName))
                {
                    CoorditionDat pos =
                        (baseTool as ResultShowTool).GlueInfoDic[Model.SelectGlueName];
                    pos.Row = Model.NumStartPy;

                    (baseTool as ResultShowTool).GlueInfoDic[Model.SelectGlueName] = pos;
                }
            }
        }

        /// <summary>
        /// 图像结果显示参数保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveParam_Click()
        {

            (baseTool as ResultShowTool).inforCoorX = Model.NmCoorX;
            (baseTool as ResultShowTool).inforCoorY = Model.NmCoorY;
            (baseTool as ResultShowTool).isShowInspectRegion = Model.ShowInspectRegChecked;

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
            OnSaveParamHandle?.Invoke(baseTool.GetToolName(), par);
        }
        /// <summary>
        /// 图像结果显示测试
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

            RunResult rlt = baseTool.Run();
            ShowTool.ClearAllOverLays();
            HOperatorSet.GetImageSize(imgBuf, out HTuple width, out HTuple height);
            if (rlt.runFlag)
            {
                ShowTool.DispImage((par as ResultShowParam).OutputImg);
                ShowTool.D_HImage = (par as ResultShowParam).OutputImg;
                ShowTool.DispMessage("OK", 10, width - 500, "green", 100);
                ShowTool.AddTextBuffer("OK", 10, width - 500, "green", 100);

                ShowTool.DispRegion((par as ResultShowParam).ResultRegion, "green");
                ShowTool.AddregionBuffer((par as ResultShowParam).ResultRegion, "green");

                List<StuFlagInfo> info = (par as ResultShowParam).ResultInfo;
                int num = info.Count;
                for (int i = 0; i < num; i++)
                {
                    ShowTool.DispMessage(info[i].info, 10 + i * 150, 10, info[i].flag ? "green" : "red", 16);
                    ShowTool.AddTextBuffer(info[i].info, 10 + i * 150, 10, info[i].flag ? "green" : "red", 16);
                }
            }
            else
            {
                ShowTool.DispImage(imgBuf);
                ShowTool.DispMessage("NG", 10, width - 500, "red", 100);
                ShowTool.AddTextBuffer("NG", 10, width - 500, "red", 100);
                ShowTool.DispAlarmMessage(rlt.errInfo, 100, 10, 12);
            }
        }
    }
}
