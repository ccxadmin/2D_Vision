using ROIGenerateLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;
using VisionShowLib.Models;

namespace VisionShowLib.ViewModels
{
    public class CommandModel : INotifyPropertyChanged
    {
        bool isCanExec = true;
        public event PropertyChangedEventHandler PropertyChanged;
        public MyICommand ButtonCommand { get; private set; }
        public int ImgWidth { get; set; }
        public int ImgHeight { get; set; }
        public ROIController roiController { get; set; } 
        public delegate void deleROIUpdateNotify(ROI currRegionList, EumROIupdate EumROIupdateArgs);
        public deleROIUpdateNotify ROIUpdateNotifyHandle;

        public CommandModel()
        {
            ButtonCommand = new MyICommand(MyExecute, MyCanExec);

        }
        public void setRoiController(ROIController rc)
        {
            roiController = rc;
        }

        private bool MyCanExec(object parameter)
        {
            return isCanExec;
        }

        private void MyExecute(object parameter)
        {
            switch ((parameter as Button).Name)
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

            }

        }

    }
}
