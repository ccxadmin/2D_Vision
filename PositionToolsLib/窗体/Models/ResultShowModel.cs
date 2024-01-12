using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Models
{
    public class ResultShowModel : NotifyBase
    {
        /// <summary>
        /// 窗体标题
        /// </summary>
        private string titleName = "参数设置窗体";
        public string TitleName
        {
            get { return this.titleName; }
            set
            {
                titleName = value;
                DoNotify();
            }
        }
        private ObservableCollection<DgDataOfResultShow> dgDataOfResultShowList =
           new ObservableCollection<DgDataOfResultShow>();
        public ObservableCollection<DgDataOfResultShow> DgDataOfResultShowList
        {
            get { return this.dgDataOfResultShowList; }
            set
            {
                dgDataOfResultShowList = value;
                DoNotify();

            }
        }
        /// <summary>
        ///选择表格索引编号
        /// </summary>
        private int dgDataSelectIndex;
        public int DgDataSelectIndex
        {
            get { return this.dgDataSelectIndex; }
            set
            {
                dgDataSelectIndex = value;
                DoNotify();
            }
        }


        /// <summary>
        /// 图像源集合
        /// </summary>
        private ObservableCollection<string> imageList = new ObservableCollection<string>();
        public ObservableCollection<string> ImageList
        {
            get { return this.imageList; }
            set
            {
                imageList = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择图像名称
        /// </summary>
        private string selectImageName;
        public string SelectImageName
        {
            get { return this.selectImageName; }
            set
            {
                selectImageName = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择图像索引编号
        /// </summary>
        private int selectImageIndex;
        public int SelectImageIndex
        {
            get { return this.selectImageIndex; }
            set
            {
                selectImageIndex = value;
                DoNotify();
            }
        }

        private bool showInspectRegChecked = false;
        public bool ShowInspectRegChecked
        {
            get { return this.showInspectRegChecked; }
            set
            {
                showInspectRegChecked = value;
                DoNotify();
            }
        }

        private int numCoorX = 0;
        public int NmCoorX
        {
            get { return this.numCoorX; }
            set
            {
                numCoorX = value;
                DoNotify();
            }
        }

        private int numCoorY = 0;
        public int NmCoorY
        {
            get { return this.numCoorY; }
            set
            {
                numCoorY = value;
                DoNotify();
            }
        }

        private ObservableCollection<string> toolNameList = new ObservableCollection<string>();
        public ObservableCollection<string> ToolNameList
        {
            get { return this.toolNameList; }
            set
            {
                toolNameList = value;
                DoNotify();
            }
        }


     
        private ObservableCollection<string> xCoorList = new ObservableCollection<string>();
        public ObservableCollection<string> XCoorList
        {
            get { return this.xCoorList; }
            set
            {
                xCoorList = value;
                DoNotify();
            }
        }
     
        private string selectXCoorName;
        public string SelectXCoorName
        {
            get { return this.selectXCoorName; }
            set
            {
                selectXCoorName = value;
                DoNotify();
            }
        }
      
        private int selectXCoorIndex;
        public int SelectXCoorIndex
        {
            get { return this.selectXCoorIndex; }
            set
            {
                selectXCoorIndex = value;
                DoNotify();
            }
        }


        private ObservableCollection<string> yCoorList = new ObservableCollection<string>();
        public ObservableCollection<string> YCoorList
        {
            get { return this.yCoorList; }
            set
            {
                yCoorList = value;
                DoNotify();
            }
        }

        private string selectYCoorName;
        public string SelectYCoorName
        {
            get { return this.selectYCoorName; }
            set
            {
                selectYCoorName = value;
                DoNotify();
            }
        }

        private int selectYCoorIndex;
        public int SelectYCoorIndex
        {
            get { return this.selectYCoorIndex; }
            set
            {
                selectYCoorIndex = value;
                DoNotify();
            }
        }

        private ObservableCollection<string> angCoorList = new ObservableCollection<string>();
        public ObservableCollection<string> AngCoorList
        {
            get { return this.angCoorList; }
            set
            {
                angCoorList = value;
                DoNotify();
            }
        }

        private string selectAngCoorName;
        public string SelectAngCoorName
        {
            get { return this.selectAngCoorName; }
            set
            {
                selectAngCoorName = value;
                DoNotify();
            }
        }

        private int selectAngCoorIndex;
        public int SelectAngCoorIndex
        {
            get { return this.selectAngCoorIndex; }
            set
            {
                selectAngCoorIndex = value;
                DoNotify();
            }
        }

        private ObservableCollection<DgResultOfResultShow> dgResultOfResultShowList =
         new ObservableCollection<DgResultOfResultShow>();
        public ObservableCollection<DgResultOfResultShow> DgResultOfResultShowList
        {
            get { return this.dgResultOfResultShowList; }
            set
            {
                dgResultOfResultShowList = value;
                DoNotify();

            }
        }
    }

    [Serializable]
    public class DgDataOfResultShow : NotifyBase
    {
        public DgDataOfResultShow(bool use, string toolName, string toolStatus)
        {
            Use = use;
            ToolName = toolName;
            ToolStatus = toolStatus;
            //toolNameList.Add("1");
            //toolNameList.Add("2");
            //toolNameList.Add("3");
        }
        private bool use;
        public bool Use {
            get=>this.use;

            set { this.use = value; DoNotify(); }
        }
        private string toolName;
        public string ToolName { 
            get=>this.toolName;
            set
            {
                this.toolName = value; DoNotify();
            }
        }

        private string toolStatus;
        public string ToolStatus {
            get=>this.toolStatus;
            set { this.toolStatus = value; DoNotify();
            } }

        //private ObservableCollection<string> toolNameList = new ObservableCollection<string>();
        //public ObservableCollection<string> ToolNameList
        //{
        //    get { return this.toolNameList; }
        //    set
        //    {
        //        toolNameList = value;
        //        //DoNotify();
        //    }
        //}

    }

    [Serializable]
    public class DgResultOfResultShow
    {
        public DgResultOfResultShow(int id,  double x,
                         double y, double angle)
        {
            ID = id;         
            X =double.Round( x,3);
            Y =double.Round( y,3);
            Angle =double.Round( angle,3);
        }
        public int ID { get; set; }
     
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }

    }
}
