using ROIGenerateLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserControl = System.Windows.Controls.UserControl;
using Button = System.Windows.Controls.Button;

namespace VisionShowLib.UserControls
{
    /// <summary>
    /// UIROIoperation.xaml 的交互逻辑
    /// </summary>
    public partial class UIROIoperation : UserControl
    {
        public UIROIoperation()
        {
            InitializeComponent();
            ButtonEvent.Button_Click += btn_Click;
        }
        public int ImgWidth { get; set; }
        public int ImgHeight { get; set; }
        public ROIController roiController { get; set; }
        public delegate void deleROIUpdateNotify(ROI currRegionList, EumROIupdate EumROIupdateArgs);
        public deleROIUpdateNotify ROIUpdateNotifyHandle;
        public RoutedEventHandler RoiConfirmHandle = null;
     
        public void SetRoiController(ROIController rc)
        {
            roiController = rc;
        }
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "btnCreateRect":
                    List<ROI> Rect1rOIs = null;
                    roiController.genRect1(100, 100, 200, 200, ref Rect1rOIs);
                    break;

                case "btnCreateRatRect":
                    List<ROI> Rect2rOIs = null;
                    roiController.genRect2(100, 100, 0, 100, 50, ref Rect2rOIs);
                    break;

                case "btnCreateCircle":
                    List<ROI> CirclerOIs = null;
                    roiController.genCircle(100, 100, 100, ref CirclerOIs);
                    break;

                case "btnCreateCircleArc":
                    List<ROI> CircleArcrOIs = null;
                    roiController.genCircularArc(100, 100, 100, 0, Math.PI / 2, "positive", ref CircleArcrOIs);
                    break;

                case "btnCreateLine":
                    List<ROI> LinerOIs = null;
                    roiController.genLine(100, 100, 100, 200, ref LinerOIs);
                    break;

                case "btnCreatePoint":
                    List<ROI> PointrOIs = null;
                    roiController.genPoint(100, 100, ref PointrOIs);
                    break;

                case "btnCreatePolygn":
                    List<ROI> PolygonrOIs = null;
                    roiController.genPolygon(new double[] { 100, 200, 200 },
                        new double[] { 300, 200, 400 }, ref PolygonrOIs);
                    break;

                case "btnCreateEllipse":
                    List<ROI> EllipserOIs = null;
                    roiController.genEllipse(100, 100, 0, 100, 50, ref EllipserOIs);
                    break;

                case "btnROIDelete":
                    roiController.removeActive();
                    foreach (var s in roiController.HwinOpreateROI)
                        ROIUpdateNotifyHandle?.Invoke(s,
                           roiController.HwinMouseOpreate);
                    break;

                case "btnROIAddup":
                    int count = roiController.activeROIidx.Count;
                    if (count < 2) return;
                    roiController.setROISign(ROIController.MODE_ROI_POS,
                       roiController.activeROIidx[0]);
                    roiController.setROISign(ROIController.MODE_ROI_POS,
                        roiController.activeROIidx[1]);
                    roiController.defineModelROI();
                    roiController.setROIShape(new ROICombine(roiController.ModelROI,
                        ImgWidth, ImgHeight));
                    double row = 0, column = 0;
                    if (roiController.ModelROI != null) roiController.ModelROI.AreaCenter(out row,
                        out column);
                    roiController.mouseDownAction(column, row);
                    foreach (var s in roiController.HwinOpreateROI)
                        ROIUpdateNotifyHandle?.Invoke(s,
                           roiController.HwinMouseOpreate);
                    break;

                case "btnROISubtract":
                    int count2 = roiController.activeROIidx.Count;
                    if (count2 < 2) return;
                    roiController.setROISign(ROIController.MODE_ROI_POS,
                         roiController.activeROIidx[0]);
                    roiController.setROISign(ROIController.MODE_ROI_NEG,
                       roiController.activeROIidx[1]);
                    roiController.defineModelROI();
                    roiController.setROIShape(new ROICombine(roiController.ModelROI,
                        ImgWidth, ImgHeight));
                    double row2 = 0, column2 = 0;
                    if (roiController.ModelROI != null) roiController.ModelROI.AreaCenter(out row2,
                        out column2);
                    roiController.mouseDownAction(column2, row2);
                    foreach (var s in roiController.HwinOpreateROI)
                        ROIUpdateNotifyHandle?.Invoke(s,
                           roiController.HwinMouseOpreate);
                    break;

                case "btnRoiConfirm":
                    RoiConfirmHandle?.Invoke(sender, e);
                    break;

            }
        }
        public  void EnableControl(bool enableFlag)
        {
            this.Dispatcher.Invoke(new Action(() => {
                this.panel.IsEnabled = enableFlag;
            }));
        }
        /// <summary>
        /// 点位新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (roiController.getActiveROI() == null) return;
            int count = roiController.getActiveROI().Count;
            if (count <= 0) return;
            ROIPolygon ROIPolygn = roiController.getActiveROI()[count - 1] as ROIPolygon;

            if (ROIPolygn == null) return;

            switch ((sender as MenuItem).Header)
            {
                case "点位新增":
                    roiController.AddPolygnPoint();
                    break;
                case "点位删除":
                    roiController.DelSelectPoint();
                    break;

            }
        }
    }
}
