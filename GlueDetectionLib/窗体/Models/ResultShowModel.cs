using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlueDetectionLib.窗体.Models
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
        private int dgDataSelectIndex=-1;
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

        //private string selectToolName;
        //public string SelectToolName
        //{
        //    get { return this.selectToolName; }
        //    set
        //    {
        //        selectToolName = value;
        //        DoNotify();
        //    }
        //}
        private Action numStartPxCommand;
        public Action NumStartPxCommand
        {
            get { return numStartPxCommand; }
            set
            {
                numStartPxCommand = value;
                DoNotify();
            }
        }
        private Action numStartPyCommand;
        public Action NumStartPyCommand
        {
            get { return numStartPyCommand; }
            set
            {
                numStartPyCommand = value;
                DoNotify();
            }
        }

        private ObservableCollection<string> glueNameList = new ObservableCollection<string>();
        public ObservableCollection<string> GlueNameList
        {
            get { return this.glueNameList; }
            set
            {
                glueNameList = value;
                DoNotify();
            }
        }

      
        private string selectGlueName;
        public string SelectGlueName
        {
            get { return this.selectGlueName; }
            set
            {
                selectGlueName = value;
                DoNotify();
            }
        }
       
        private int selectGlueIndex;
        public int SelectGlueIndex
        {
            get { return this.selectGlueIndex; }
            set
            {
                selectGlueIndex = value;
                DoNotify();
            }
        }


        private bool cobxGlueNameEnable = false;
        public bool CobxGlueNameEnable
        {
            get { return this.cobxGlueNameEnable; }
            set
            {
                cobxGlueNameEnable = value;
                DoNotify();
            }
        }

        private int numStartPx;
        public int NumStartPx
        {
            get { return this.numStartPx; }
            set
            {
                numStartPx = value;
                DoNotify();
            }
        }
        private int numStartPy;
        public int NumStartPy
        {
            get { return this.numStartPy; }
            set
            {
                numStartPy = value;
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
        public bool Use
        {
            get => this.use;

            set { this.use = value; DoNotify(); }
        }
        private string toolName;
        public string ToolName
        {
            get => this.toolName;
            set
            {
                this.toolName = value; DoNotify();
            }
        }

        private string toolStatus;
        public string ToolStatus
        {
            get => this.toolStatus;
            set
            {
                this.toolStatus = value; DoNotify();
            }
        }

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
}
