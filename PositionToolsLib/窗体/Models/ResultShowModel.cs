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


     
        private ObservableCollection<string> rowCoorList = new ObservableCollection<string>();
        public ObservableCollection<string> RowCoorList
        {
            get { return this.rowCoorList; }
            set
            {
                rowCoorList = value;
                DoNotify();
            }
        }
     
        private string selectRowCoorName;
        public string SelectRowCoorName
        {
            get { return this.selectRowCoorName; }
            set
            {
                selectRowCoorName = value;
                DoNotify();
            }
        }
      
        private int selectRowCoorIndex;
        public int SelectRowCoorIndex
        {
            get { return this.selectRowCoorIndex; }
            set
            {
                selectRowCoorIndex = value;
                DoNotify();
            }
        }


        private ObservableCollection<string> colCoorList = new ObservableCollection<string>();
        public ObservableCollection<string> ColCoorList
        {
            get { return this.colCoorList; }
            set
            {
                colCoorList = value;
                DoNotify();
            }
        }

        private string selectColCoorName;
        public string SelectColCoorName
        {
            get { return this.selectColCoorName; }
            set
            {
                selectColCoorName = value;
                DoNotify();
            }
        }

        private int selectColCoorIndex;
        public int SelectColCoorIndex
        {
            get { return this.selectColCoorIndex; }
            set
            {
                selectColCoorIndex = value;
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
    public class DgDataOfResultShow
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
        public bool Use { get; set; }
        public string ToolName { get; set; }
        public string ToolStatus { get; set; }

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
        public DgResultOfResultShow(int id,  double row,
                         double column, double angle)
        {
            ID = id;         
            Row = row;
            Column = column;
            Angle = angle;
        }
        public int ID { get; set; }
     
        public double Row { get; set; }
        public double Column { get; set; }
        public double Angle { get; set; }

    }
}
