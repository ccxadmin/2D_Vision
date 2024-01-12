using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Models
{
    public class CoordConvertModel : NotifyBase
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

        private string testInfo;
        public string TestInfo
        {
            get { return this.testInfo; }
            set
            {
                testInfo = value;
                DoNotify();
            }
        }

        private int selectConvertWayIndex;
        public int SelectConvertWayIndex
        {
            get { return this.selectConvertWayIndex; }
            set
            {
                selectConvertWayIndex = value;
                DoNotify();
            }
        }


      

        private string  calibFilePath;
        public string CalibFilePath
        {
            get { return this.calibFilePath; }
            set
            {
                calibFilePath = value;
                DoNotify();
            }
        }

        private ObservableCollection<string> positionDataList = new ObservableCollection<string>();
        public ObservableCollection<string> PositionDataList
        {
            get { return this.positionDataList; }
            set
            {
                positionDataList = value;
                DoNotify();
            }
        }

        private string selectCoordXName;
        public string SelectCoordXName
        {
            get { return this.selectCoordXName; }
            set
            {
                selectCoordXName = value;
                DoNotify();
            }
        }
        private int selectCoordXIndex;
        public int SelectCoordXIndex
        {
            get { return this.selectCoordXIndex; }
            set
            {
                selectCoordXIndex = value;
                DoNotify();
            }
        }

        private string selectCoordYName;
        public string SelectCoordYName
        {
            get { return this.selectCoordYName; }
            set
            {
                selectCoordYName = value;
                DoNotify();
            }
        }
        private int selectCoordYIndex;
        public int SelectCoordYIndex
        {
            get { return this.selectCoordYIndex; }
            set
            {
                selectCoordYIndex = value;
                DoNotify();
            }
        }

      
        private ObservableCollection<CoordConvertData> dgResultOfCoordConvertList =
new ObservableCollection<CoordConvertData>();
        public ObservableCollection<CoordConvertData> DgResultOfCoordConvertList
        {
            get { return this.dgResultOfCoordConvertList; }
            set
            {
                dgResultOfCoordConvertList = value;
                DoNotify();

            }
        }
    }

    [Serializable]
    public class CoordConvertData
    {
        public CoordConvertData(int id, double x,double y)

        {
            ID = id;
            X =Math.Round( x,3);
            Y =Math.Round( y,3);
        }
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }


    }
   
}
