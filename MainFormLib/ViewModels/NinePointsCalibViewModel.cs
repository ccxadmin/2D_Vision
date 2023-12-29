using ControlShareResources.Common;
using HalconDotNet;
using MainFormLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisionShowLib.UserControls;

namespace MainFormLib.ViewModels
{
    public class NinePointsCalibViewModel
    {
        public static NinePointsCalibViewModel This { get; set; }
        public NinePointsCalibModel Model { get; set; }
        public VisionShowTool ShowTool { get; set; }

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
        int PixelPoint_Indexer = 0;
        int RobotPoint_Indexer = 0;
        int RotatePoint_Indexer = 0;


        public NinePointsCalibViewModel(string _rootFolder)
        {
            if (Directory.Exists(_rootFolder))
                rootFolder = _rootFolder;
            HOperatorSet.GenEmptyObj(out imgBuf);
            This = this;
            Model = new NinePointsCalibModel();
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

        }
        /// <summary>
        /// 删除物理坐标点位
        /// </summary>
        private void btnDeleteRobotPoint_Click()
        {

        }

        /// <summary>
        /// 修改物理坐标点位
        /// </summary>
        private void btnModifyRobotPoint_Click()
        {

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

        }
        /// <summary>
        /// 删除旋转像素坐标点位
        /// </summary>
        private void btnDeleteRotatePixel_Click()
        {

        }
        /// <summary>
        /// 修改旋转像素坐标点位
        /// </summary>
        private void btnModifyRotatePixel_Click()
        {


        }
        /// <summary>
        /// 计算旋转中心
        /// </summary>
        private void btnCalRotateCenter_Click()
        {

        }

        /// <summary>
        /// 保存旋转数据
        /// </summary>
        private void btnSaveRatateData_Click()
        {

        }
        /// <summary>
        /// 像素坐标转物理坐标
        /// </summary>
        private void btnConvertPixelToRobot_Click()
        {

        }
        /// <summary>
        /// 物理坐标转像素坐标
        /// </summary>
        private void btnConvertRobotToPixel_Click()
        {

        }
        /// <summary>
        /// 生成
        /// </summary>
        private void btnTestBut_Click()
        {

        }
        /// <summary>
        /// 保存
        /// </summary>
        private void btnSaveBut_Click()
        {
           
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
