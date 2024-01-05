using ControlShareResources.Common;
using FilesRAW.Common;
using FunctionLib.Location;
using HalconDotNet;
using MainFormLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using VisionShowLib.UserControls;

namespace MainFormLib.ViewModels
{
    public class NinePointsCalibViewModel
    {
        public static NinePointsCalibViewModel This { get; set; }
        public NinePointsCalibModel Model { get; set; }
        public VisionShowTool ShowTool { get; set; }
        //public EventHandler SaveCaliParmaHandleOfNightPoint = null;//参数保存返回事件,供外部订阅可获取坐标系变换矩阵
        public EventHandler SetCalCentreHandle;//参数保存返回事件,供外部订阅可获取旋转中心
        #region  Command
        public CommandBase GetPixelPointClickCommand { get; set; }
        public CommandBase NewPixelPointClickCommand { get; set; }
        public CommandBase DeletePixelPointClickCommand { get; set; }
        public CommandBase ModifyPixelPointClickCommand { get; set; }
        public CommandBase GetRobotPointClickCommand { get; set; }
        public CommandBase NewRobotPointClickCommand { get; set; }
        public CommandBase DeleteRobotPointClickCommand { get; set; }
        public CommandBase ModifyRobotPointClickCommand { get; set; }
        public CommandBase GetRotatePixelClickCommand { get; set; }
        public CommandBase NewRotatePixelClickCommand { get; set; }
        public CommandBase DeleteRotatePixelClickCommand { get; set; }
        public CommandBase ModifyRotatePixelClickCommand { get; set; }
        public CommandBase CalRotateCenterClickCommand { get; set; }
        public CommandBase SaveRatateDataClickCommand { get; set; }
        public CommandBase ConvertPixelToRobotClickCommand { get; set; }
        public CommandBase ConvertRobotToPixelClickCommand { get; set; }
        public CommandBase TestButClickCommand { get; set; }
        public CommandBase SaveButClickCommand { get; set; }
        public CommandBase PixelPointClearClickCommand { get; set; }
        public CommandBase RobotPointClearClickCommand { get; set; }
        public CommandBase RotatePointClearClickCommand { get; set; }


        #endregion


        HObject imgBuf = null;//图像缓存     
        private string rootFolder = Environment.CurrentDirectory; //根目录
        private string currCalibName = "default";//标定文件夹名称
        int PixelPoint_Indexer = 0;
        int RobotPoint_Indexer = 0;
        int RotatePoint_Indexer = 0;
        //HTuple hv_HomMat2D;//标定矩阵

        private NinePointsCalibViewModel()
        {
            HOperatorSet.GenEmptyObj(out imgBuf);
            //图像控件      
            ShowTool = new VisionShowTool();
            ShowTool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);

            #region Command

            GetPixelPointClickCommand = new CommandBase();
            GetPixelPointClickCommand.DoExecute = new Action<object>((o) => btnGetPixelPoint_Click());
            GetPixelPointClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            NewPixelPointClickCommand = new CommandBase();
            NewPixelPointClickCommand.DoExecute = new Action<object>((o) => btnNewPixelPoint_Click());
            NewPixelPointClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DeletePixelPointClickCommand = new CommandBase();
            DeletePixelPointClickCommand.DoExecute = new Action<object>((o) => btnDeletePixelPoint_Click());
            DeletePixelPointClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ModifyPixelPointClickCommand = new CommandBase();
            ModifyPixelPointClickCommand.DoExecute = new Action<object>((o) => btnModifyPixelPoint_Click());
            ModifyPixelPointClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            GetRobotPointClickCommand = new CommandBase();
            GetRobotPointClickCommand.DoExecute = new Action<object>((o) => btnGetRobotPoint_Click());
            GetRobotPointClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            NewRobotPointClickCommand = new CommandBase();
            NewRobotPointClickCommand.DoExecute = new Action<object>((o) => btnNewRobotPoint_Click());
            NewRobotPointClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DeleteRobotPointClickCommand = new CommandBase();
            DeleteRobotPointClickCommand.DoExecute = new Action<object>((o) => btnDeleteRobotPoint_Click());
            DeleteRobotPointClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ModifyRobotPointClickCommand = new CommandBase();
            ModifyRobotPointClickCommand.DoExecute = new Action<object>((o) => btnModifyRobotPoint_Click());
            ModifyRobotPointClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            GetRotatePixelClickCommand = new CommandBase();
            GetRotatePixelClickCommand.DoExecute = new Action<object>((o) => btnGetRotatePixel_Click());
            GetRotatePixelClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            NewRotatePixelClickCommand = new CommandBase();
            NewRotatePixelClickCommand.DoExecute = new Action<object>((o) => btnNewRotatePixel_Click());
            NewRotatePixelClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            DeleteRotatePixelClickCommand = new CommandBase();
            DeleteRotatePixelClickCommand.DoExecute = new Action<object>((o) => btnDeleteRotatePixel_Click());
            DeleteRotatePixelClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ModifyRotatePixelClickCommand = new CommandBase();
            ModifyRotatePixelClickCommand.DoExecute = new Action<object>((o) => btnModifyRotatePixel_Click());
            ModifyRotatePixelClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            CalRotateCenterClickCommand = new CommandBase();
            CalRotateCenterClickCommand.DoExecute = new Action<object>((o) => btnCalRotateCenter_Click());
            CalRotateCenterClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveRatateDataClickCommand = new CommandBase();
            SaveRatateDataClickCommand.DoExecute = new Action<object>((o) => btnSaveRatateData_Click());
            SaveRatateDataClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ConvertPixelToRobotClickCommand = new CommandBase();
            ConvertPixelToRobotClickCommand.DoExecute = new Action<object>((o) => btnConvertPixelToRobot_Click());
            ConvertPixelToRobotClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            ConvertRobotToPixelClickCommand = new CommandBase();
            ConvertRobotToPixelClickCommand.DoExecute = new Action<object>((o) => btnConvertRobotToPixel_Click());
            ConvertRobotToPixelClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            TestButClickCommand = new CommandBase();
            TestButClickCommand.DoExecute = new Action<object>((o) => btnTestBut_Click());
            TestButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SaveButClickCommand = new CommandBase();
            SaveButClickCommand.DoExecute = new Action<object>((o) => btnSaveBut_Click());
            SaveButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            PixelPointClearClickCommand = new CommandBase();
            PixelPointClearClickCommand.DoExecute = new Action<object>((o) => btnPixelPointClear_Click());
            PixelPointClearClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            RobotPointClearClickCommand = new CommandBase();
            RobotPointClearClickCommand.DoExecute = new Action<object>((o) => btnRobotPointClear_Click());
            RobotPointClearClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            RotatePointClearClickCommand = new CommandBase();
            RotatePointClearClickCommand.DoExecute = new Action<object>((o) => btnRotatePointClear_Click());
            RotatePointClearClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });


            #endregion
        }
        public NinePointsCalibViewModel(string _rootFolder, string _calibName = "default")
            :this()
        {
            if (Directory.Exists(_rootFolder))
                rootFolder = _rootFolder;
            currCalibName = _calibName;
         
            This = this;
            Model = new NinePointsCalibModel();
           
            LoadData();
        }

        public NinePointsCalibViewModel(NinePointsCalibModel model):this()
        {                 
            This = this;
            Model = model;
         
        }

        /// <summary>
        /// 图像加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadedImageNoticeEvent(object sender, EventArgs e)
        {

            imgBuf.Dispose();
            imgBuf = ShowTool.D_HImage;
        }
        /// <summary>
        /// 设置文件存放根目录
        /// </summary>
        /// <param name="_rootFolder"></param>
        public void SetRootFolder(string _rootFolder)
        {
            rootFolder = _rootFolder;
        }
        /// <summary>
        /// 设置当前九点标定文件夹名称
        /// </summary>
        /// <param name="name"></param>
        public void SetCalibName(string name)
        {
            currCalibName = name;
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        /// <param name="_rootFolder"></param>
        /// <param name="_calibName"></param>
        public void ReLoadData(string _rootFolder, string _calibName = "default")
        {
            if (Directory.Exists(_rootFolder))
                rootFolder = _rootFolder;
            currCalibName = _calibName;
            LoadData();
        }
        /// <summary>
        /// 加载参数
        /// </summary>
        void LoadData()
        {
            string filePath = rootFolder + "\\标定矩阵\\九点标定\\" + currCalibName;
            if (!Directory.Exists(rootFolder + "\\标定矩阵"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵");
            if (!Directory.Exists(rootFolder + "\\标定矩阵\\九点标定"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵\\九点标定");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            try
            { 
                //九点标定关系参数保存
                Model.DgPixelPointDataList= GeneralUse.ReadSerializationFile<ObservableCollection<DgPixelPointData>>(filePath+ "\\PixelPoint");
                Model.DgRobotPointDataList=GeneralUse.ReadSerializationFile<ObservableCollection<DgRobotPointData>>(filePath+ "\\RobotPoint");
                Model.TxbSx=double.Parse( GeneralUse.ReadValue("九点标定", "X缩放",  "config","1", filePath));
                Model.TxbSy=double.Parse( GeneralUse.ReadValue("九点标定", "Y缩放", "config","1", filePath));
                Model.TxbPhi=double.Parse( GeneralUse.ReadValue("九点标定", "旋转弧", "config","1", filePath));
                Model.TxbTheta=double.Parse(  GeneralUse.ReadValue("九点标定", "倾斜弧",  "config","1", filePath));
                Model.TxbTx= double.Parse( GeneralUse.ReadValue("九点标定", "X偏移量",  "config","1", filePath));
                Model.TxbTy=double.Parse( GeneralUse.ReadValue("九点标定", "Y偏移量",  "config","1", filePath));
                if (File.Exists(filePath + "\\hv_HomMat2D.tup"))
                {
                    HOperatorSet.ReadTuple(filePath + "\\hv_HomMat2D.tup", out HTuple hv_HomMat2D);
                    Model.Hv_HomMat2D = hv_HomMat2D;
                }
                   
                //旋转中心数据
                Model.TxbRotateCenterX=double.Parse( GeneralUse.ReadValue("九点标定", "旋转中心X", "config","0", filePath));
                Model.TxbRotateCenterY=double.Parse( GeneralUse.ReadValue("九点标定", "旋转中心Y", "config","0", filePath));
                Model.DgRotatePointDataList=GeneralUse.ReadSerializationFile<ObservableCollection<DgRotatePointData>>(filePath+ "\\RotatePoint");
            }
            catch (Exception er)
            {
                ShowTool.DispAlarmMessage("参数加载失败！" + er.Message, 500, 20, 30);
                //Appentxt("参数保存失败！" + er.Message);
                // MessageBox.Show("参数保存失败！" + er.Message);
            }
        }
        /// <summary>
        /// 9点标定，Mark点像素坐标获取
        /// </summary>
        private void btnGetPixelPoint_Click()
        {
          
        }
        /// <summary>
        /// 9点标定，新增像素坐标点
        /// </summary>
        private void btnNewPixelPoint_Click()
        {
            PixelPoint_Indexer++;
            Model.DgPixelPointDataList.Add(new DgPixelPointData(PixelPoint_Indexer,
                Model.TxbPixelX,
                Model.TxbPixelY));
        }
        /// <summary>
        /// 9点标定，删除像素坐标点
        /// </summary>
        private void btnDeletePixelPoint_Click()
        {
            if (Model.DgPixelPointSelectIndex < 0 ||
                Model.DgPixelPointSelectIndex>= Model.DgPixelPointDataList.Count)
                return;
            if (MessageBox.Show("确认删除？", "Information", MessageBoxButton.YesNo,
                MessageBoxImage.Question) ==MessageBoxResult.Yes)

            {
                Model.DgPixelPointDataList.RemoveAt(Model.DgPixelPointSelectIndex);

            }
        }

        /// <summary>
        /// 9点标定，修改像素坐标点
        /// </summary>
        private void btnModifyPixelPoint_Click()
        {
            int index = Model.DgPixelPointSelectIndex;
            if (index < 0 ||
               index >= Model.DgPixelPointDataList.Count)
                return;
            if (MessageBox.Show("确认修改？", "Information", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)

            {
                Model.DgPixelPointDataList.RemoveAt(index);
                Model.DgPixelPointDataList.Insert(index,new DgPixelPointData(index+1,
                     Model.TxbPixelX, Model.TxbPixelY));
                //Model.DgPixelPointDataList[index].X = Model.TxbPixelX;
                //Model.DgPixelPointDataList[index].Y = Model.TxbPixelY;
                
            }

        }
        /// <summary>
        /// 获取物理坐标点位
        /// </summary>
        private void btnGetRobotPoint_Click()
        {

        }
        /// <summary>
        /// 新增物理坐标点位
        /// </summary>
        private void btnNewRobotPoint_Click()
        {
            RobotPoint_Indexer++;
            Model.DgRobotPointDataList.Add(new DgRobotPointData(RobotPoint_Indexer,
                Model.TxbRobotX,
                Model.TxbRobotY));
        }
        /// <summary>
        /// 删除物理坐标点位
        /// </summary>
        private void btnDeleteRobotPoint_Click()
        {
            if (Model.DgRobotPointSelectIndex < 0 ||
                Model.DgRobotPointSelectIndex >= Model.DgRobotPointDataList.Count)
                return;
            if (MessageBox.Show("确认删除？", "Information", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)

            {
                Model.DgRobotPointDataList.RemoveAt(Model.DgRobotPointSelectIndex);

            }
        }

        /// <summary>
        /// 修改物理坐标点位
        /// </summary>
        private void btnModifyRobotPoint_Click()
        {
            int index = Model.DgRobotPointSelectIndex;
            if (index < 0 ||
               index >= Model.DgRobotPointDataList.Count)
                return;
            if (MessageBox.Show("确认修改？", "Information", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)

            {
                Model.DgRobotPointDataList.RemoveAt(index);
                Model.DgRobotPointDataList.Insert(index, new DgRobotPointData(index + 1,
                     Model.TxbRobotX, Model.TxbRobotY));
     
            }
        }
        /// <summary>
        /// 获取旋转像素点位
        /// </summary>
        private void  btnGetRotatePixel_Click()
        {

        }
        /// <summary>
        /// 新增旋转像素坐标点位
        /// </summary>
        private void btnNewRotatePixel_Click()
        {
            RotatePoint_Indexer++;
            Model.DgRotatePointDataList.Add(new DgRotatePointData(RotatePoint_Indexer,
                Model.TxbRotatePixelX,
                Model.TxbRotatePixelY));
        }
        /// <summary>
        /// 删除旋转像素坐标点位
        /// </summary>
        private void btnDeleteRotatePixel_Click()
        {
            if (Model.DgRotatePointSelectIndex < 0 ||
                Model.DgRotatePointSelectIndex >= Model.DgRotatePointDataList.Count)
                return;
            if (MessageBox.Show("确认删除？", "Information", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)

            {
                Model.DgRotatePointDataList.RemoveAt(Model.DgRotatePointSelectIndex);

            }

        }
        /// <summary>
        /// 修改旋转像素坐标点位
        /// </summary>
        private void btnModifyRotatePixel_Click()
        {
            int index = Model.DgRotatePointSelectIndex;
            if (index < 0 ||
               index >= Model.DgRotatePointDataList.Count)
                return;
            if (MessageBox.Show("确认修改？", "Information", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)

            {
                Model.DgRotatePointDataList.RemoveAt(index);
                Model.DgRotatePointDataList.Insert(index, new DgRotatePointData(index + 1,
                     Model.TxbRotatePixelX, Model.TxbRotatePixelY));

            }

        }
        /// <summary>
        /// 计算旋转中心
        /// </summary>
        private void btnCalRotateCenter_Click()
        {
            if (Model.DgRotatePointDataList.Count < 5)
            {
                ShowTool.DispAlarmMessage("点位坐标数据不足5条，请确认!", 500, 20, 30);
                MessageBox.Show("点位坐标数据不足5条，请确认!");
                return;
            }
            HTuple hv_MulRotate_PixelRow = new HTuple();
            HTuple hv_MulRotate_PixelColumn = new HTuple();
            ShowTool.RegionBufferClear();
            ShowTool.TextBufferClear();
            foreach (var s in Model.DgRotatePointDataList)
            {
                hv_MulRotate_PixelRow.Append(s.Y);
                hv_MulRotate_PixelColumn.Append(s.X);
                if ( VisionShowTool.ObjectValided(imgBuf))
                {

                    HObject cross = new HObject();
                    cross.Dispose();
                    HOperatorSet.GenCrossContourXld(out cross, s.Y, s.X, 50, 0);
                    ShowTool.DispRegion(cross, "green");
                    ShowTool.AddregionBuffer(cross, "green");
                }
            }
            fitcircleData d_fitcircleData = 
                AxisCoorditionRotation.MulPoints_GetRotateCenter(hv_MulRotate_PixelRow,
                hv_MulRotate_PixelColumn);//圆心为像素坐标

            HTuple temmachineX = new HTuple(); HTuple temmachineY = new HTuple();
            NinePointsCalibTool.Transformation_POINT(d_fitcircleData.center_Column,
                d_fitcircleData.center_Row, Model.Hv_HomMat2D , out temmachineX, out temmachineY);//然后转换成机器人坐标

            SetCalCentreHandle?.Invoke(new string[] { temmachineX.D.ToString("f3"),
                  temmachineY.D.ToString("f3")}, null);

            Model.TxbRotateCenterX= temmachineX.D;
            Model.TxbRotateCenterY = temmachineY.D;        
            ShowTool.DispRegion(d_fitcircleData.circleContour, "green");
            ShowTool.AddregionBuffer(d_fitcircleData.circleContour, "green");

        }

        /// <summary>
        /// 保存旋转数据
        /// </summary>
        private void btnSaveRatateData_Click()
        {
            string filePath = rootFolder + "\\标定矩阵\\九点标定\\"+ currCalibName;
            if (!Directory.Exists(rootFolder + "\\标定矩阵"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵");
            if (!Directory.Exists(rootFolder + "\\标定矩阵\\九点标定"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵\\九点标定");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            try
            {

                GeneralUse.WriteValue("九点标定", "旋转中心X", Model.TxbRotateCenterX.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "旋转中心Y", Model.TxbRotateCenterY.ToString(), "config", filePath);
                GeneralUse.WriteSerializationFile<ObservableCollection<DgRotatePointData>>(filePath+ "\\RotatePoint", Model.DgRotatePointDataList);
               
                SetCalCentreHandle?.Invoke(new string[] { Model.TxbRotateCenterX.ToString("f3"),
                  Model.TxbRotateCenterY.ToString("f3")}, null);
               
                // MessageBox.Show("参数保存成功！");
            }
            catch (Exception er)
            {
                ShowTool.DispAlarmMessage("参数保存失败！" + er.Message, 500, 20, 30);
                //Appentxt("参数保存失败！" + er.Message);
                //MessageBox.Show("参数保存失败！" + er.Message);
            }

        }
        /// <summary>
        /// 像素坐标转物理坐标
        /// </summary>
        private void btnConvertPixelToRobot_Click()
        {
            HTuple Rx = null, Ry = null;
            NinePointsCalibTool.Transformation_POINT(Model.TxbMarkPixelX,
               Model.TxbMarkPixelY, Model.Hv_HomMat2D , out Rx, out Ry);
            Model.TxbMarkRobotX = Rx.D;
            Model.TxbMarkRobotY = Ry.D;
        }
        /// <summary>
        /// 物理坐标转像素坐标
        /// </summary>
        private void btnConvertRobotToPixel_Click()
        {
            HTuple Px = null, Py = null;
            NinePointsCalibTool.Transformation_POINT_INV(Model.TxbMarkRobotX,
                Model.TxbMarkRobotY, Model.Hv_HomMat2D, out Px, out Py);
            Model.TxbMarkPixelX = Px.D;
            Model.TxbMarkPixelY = Py.D;

        }
        /// <summary>
        /// 生成
        /// </summary>
        private void btnTestBut_Click()
        {
            if (Model.DgPixelPointDataList.Count != 9 ||
                            Model.DgRobotPointDataList.Count != 9)
            {
                ShowTool.DispAlarmMessage("点位坐标数据不足9条，请确认!", 500, 20, 30);

                return;
            }
            HTuple hv_PixelPointx = new HTuple();
            HTuple hv_PixelPointy = new HTuple();
            foreach (var s in Model.DgPixelPointDataList)
            {
                hv_PixelPointx.Append(s.X);
                hv_PixelPointy.Append(s.Y);

            }
            HTuple hv_MechinePointX = new HTuple();
            HTuple hv_MechinePointY = new HTuple();
            foreach (var s in Model.DgRobotPointDataList)
            {
                hv_MechinePointX.Append(s.X);
                hv_MechinePointY.Append(s.Y);
            }

            GuidePositioning_HDevelopExport.Transformation_matrix(hv_PixelPointx, hv_PixelPointy,
                hv_MechinePointX, hv_MechinePointY, out HTuple hv_HomMat2D, out HTuple hv_ParArray);
            Model.Hv_HomMat2D = hv_HomMat2D;
            if (hv_ParArray != null)
            {
                Model.TxbSx = hv_ParArray[0].D;
                Model.TxbSy = hv_ParArray[1].D;
                Model.TxbPhi = hv_ParArray[2].D;
                Model.TxbTheta = hv_ParArray[3].D;
                Model.TxbTx = hv_ParArray[4].D;
                Model.TxbTy = hv_ParArray[5].D;

            }
        }
        /// <summary>
        /// 保存九点标定的数据
        /// </summary>
        private void btnSaveBut_Click()
        {
            string filePath = rootFolder + "\\标定矩阵\\九点标定\\" + currCalibName;
            if (!Directory.Exists(rootFolder + "\\标定矩阵"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵");
            if (!Directory.Exists(rootFolder + "\\标定矩阵\\九点标定"))
                Directory.CreateDirectory(rootFolder + "\\标定矩阵\\九点标定");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            try
            {  //九点标定关系参数保存
                GeneralUse.WriteSerializationFile<ObservableCollection<DgPixelPointData>>(filePath+ "\\PixelPoint", Model.DgPixelPointDataList);
                GeneralUse.WriteSerializationFile<ObservableCollection<DgRobotPointData>>(filePath+ "\\RobotPoint", Model.DgRobotPointDataList);
                GeneralUse.WriteValue("九点标定", "X缩放", Model.TxbSx.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "Y缩放", Model.TxbSy.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "旋转弧", Model.TxbPhi.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "倾斜弧", Model.TxbTheta.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "X偏移量", Model.TxbTx.ToString(), "config", filePath);
                GeneralUse.WriteValue("九点标定", "Y偏移量", Model.TxbTy.ToString(), "config", filePath);
                if (Model.Hv_HomMat2D != null &&
                        Model.Hv_HomMat2D.Length > 0)
                    HOperatorSet.WriteTuple(Model.Hv_HomMat2D, filePath + "\\hv_HomMat2D.tup");

                //SaveCaliParmaHandleOfNightPoint?.Invoke(filePath + "\\hv_HomMat2D.tup", null);
                //  MessageBox.Show("保存成功！");
              
            }
            catch (Exception er)
            {
                ShowTool.DispAlarmMessage("参数保存失败！" + er.Message, 500, 20, 30);
                //Appentxt("参数保存失败！" + er.Message);
                // MessageBox.Show("参数保存失败！" + er.Message);
            }
        }
        /// <summary>
        /// 像素坐标点清除
        /// </summary>
        private void btnPixelPointClear_Click()
        {
            PixelPoint_Indexer = 0;
            Model.DgPixelPointDataList.Clear();
        }
        /// <summary>
        /// 物理坐标点清除
        /// </summary>
        private void btnRobotPointClear_Click()
        {
            RobotPoint_Indexer = 0;
            Model.DgRobotPointDataList.Clear();
        }
        /// <summary>
        /// 旋转坐标点清除
        /// </summary>
        private void btnRotatePointClear_Click()
        {
            RotatePoint_Indexer = 0;
            Model.DgRotatePointDataList.Clear();

        }
    }
}
