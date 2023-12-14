using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionShowLib;
using HalconDotNet;
using VisionShowLib.UserControls;
using ROIGenerateLib;
using System.Collections.ObjectModel;
using LightSourceController.Views;
using LightSourceController.Models;
using FilesRAW.Common;
using GlueDetectionLib.窗体.Views;
using GlueDetectionLib.窗体;
using GlueDetectionLib.窗体.ViewModels;
using GlueDetectionLib.工具;
using PositionToolsLib.窗体.Views;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow :Window
    {
        public MainWindow()
        {
            dataList.Add(new data(1,"xiaoming",12));
            dataList.Add(new data(1, "xiaoming", 12));
            dataList.Add(new data(1, "xiaoming", 12));
            dataList.Add(new data(1, "xiaoming", 12));
            dataList.Add(new data(1, "xiaoming", 12));
            dataList.Add(new data(1, "xiaoming", 12));
            dataList.Add(new data(1, "xiaoming", 12));
            dataList.Add(new data(1, "xiaoming", 12));

            ObservableCollection<person> list = new ObservableCollection<person>()
            {
                new person{ FirstName="Mark",LastName="Otto",Username="@mdo"},
                new person{ FirstName="cob",LastName="Thornton",Username="@fat"},
                new person{ FirstName="Larry",LastName="the Bird",Username="@twitter"},
            };

            InitializeComponent();
            this.DataContext = dataList;
            dg.ItemsSource = list;
            ShowTreeView();         
            ExPandAllNodes(treeview.Items);
        }
        /// <summary>
        /// 装载winform控件，主动释放方式内存泄漏
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            winHost.Child.Dispose();
            winHost.Child = null;
            base.OnClosed(e);
        }

        VisionShowTool tool; HObject grabImg = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetSystem("temporary_mem_cache", "false");
            HOperatorSet.SetSystem("clip_region", "false");

            tool = new VisionShowTool();
            tool.SetColorOfTopBottomTitle(System.Drawing.Color.FromArgb(255, 109, 60));
            tool.Dock = System.Windows.Forms.DockStyle.Fill;
            tool.Padding = new System.Windows.Forms.Padding(2);
            tool.LoadedImageNoticeHandle += new EventHandler(LoadedImageNoticeEvent);
            tool.SetBackgroundColor(EumControlBackColor.white);
            tool.setDraw(EumDrawModel.margin);
            winHost.Child = tool;
            RoiEditer.SetRoiController(tool.RoiController);
            tool.ViewController.setDispLevel(HWndCtrl.MODE_INCLUDE_ROI);         
            tool.SetSystemPatten(EumSystemPattern.DesignModel);


          
        }

        LightSource lightSource = null;//光源控制器
        void LoadedImageNoticeEvent(object sender, EventArgs e)
        {
            HOperatorSet.GenEmptyObj(out grabImg);
            grabImg.Dispose();
            grabImg = tool.D_HImage;
            //object obj = RoiEditer.DataContext;
            //传递图像尺寸方便组合区域移动超边界
            RoiEditer.ImgWidth = tool.ImageWidth;
            RoiEditer.ImgHeight = tool.ImageHeight;
        }
        public ObservableCollection<data> dataList = new ObservableCollection<data>();

        public ObservableCollection<OrgModel> OrgList { get; set; }
        /// <summary>
        /// 加载tree数据
        /// </summary>
        private void ShowTreeView()
        {

            OrgList = new ObservableCollection<OrgModel>()
            {
                new OrgModel()
                {
                    IsGrouping=true,
                    DisplayName="单位名称(3/7)",
                    Children=new ObservableCollection<OrgModel>()
                    {
                        new OrgModel(){
                            IsGrouping=true,
                            DisplayName="未分组联系人(2/4)",
                            Children=new ObservableCollection<OrgModel>()
                            {
                                new OrgModel(){
                                    IsGrouping=false,
                                    SurName="刘",
                                    Name="刘棒",
                                    Info="我要走向天空！",
                                    Count=3
                                }
                            }
                        }
                    },
                }

            };
            treeview.ItemsSource = OrgList;                 
        }

        public void ExPandAllNodes(ItemCollection items)
        {
            foreach(var s in items)
            {
                var treeviewItem = treeview.ItemContainerGenerator
                    .ContainerFromItem(s) as TreeViewItem;
                if(treeviewItem!=null)
                {
                    treeviewItem.IsExpanded = true;
                    ExPandAllNodes(treeviewItem.Items);
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //string port = GeneralUse.ReadValue("光源控制器", "端口", "config", "COM1");
            //lightSource = new LightSource(port);
            //lightSource.Open(port);
            //LightSourceWindow f = new LightSourceWindow(lightSource);
            //f.ShowDialog();
            //BaseTool tool = new ColorConvertTool();
            FormTrajectoryExtraction f = new FormTrajectoryExtraction();
            //BinaryzationViewModel.This.OnSaveParamHandle += new BaseViewModel.SaveParamHandle(OnSaveParamEvent);
            //BinaryzationViewModel.This.OnSaveManageHandle = new BaseViewModel.SaveManageHandle(SaveManageOfGlue);
            f.ShowDialog();

        }



    }

    public class data
    {
        public data(int id,string name,int age)
        {
            ID = id;
            Name = name;
            Age = age;
        }
       public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

    }
    public class person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }

    public class OrgModel
    {
        public bool IsGrouping { get; set; }
        public ObservableCollection<OrgModel> Children { get; set; }
        public string DisplayName { get; set; }
        public string SurName { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public int Count { get; set; }
    }
}
