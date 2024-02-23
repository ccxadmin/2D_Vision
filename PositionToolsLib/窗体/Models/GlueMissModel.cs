using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionToolsLib.窗体.Models
{

    public class GlueMissModel : NotifyBase
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
        private string selectImageName = "";
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
        /// <summary>
        /// 区域生成方式
        /// </summary>
        private EumGenRegionWay genRegionWay = EumGenRegionWay.auto;
        public EumGenRegionWay GenRegionWay
        {
            get { return this.genRegionWay; }
            set
            {
                genRegionWay = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 区域类型索引
        /// </summary>
        private int autoRegionTypeSelectIndex = 0;
        public int AutoRegionTypeSelectIndex
        {
            get { return this.autoRegionTypeSelectIndex; }
            set
            {
                autoRegionTypeSelectIndex = value;
                DoNotify();
            }
        }
      
        /// <summary>
        ///选择黑白极性索引编号
        /// </summary>
        private int selectPolarityIndex;
        public int SelectPolarityIndex
        {
            get { return this.selectPolarityIndex; }
            set
            {
                selectPolarityIndex = value;
                DoNotify();
            }
        }

        private bool useAutoGenOuterChecked = false;
        public bool UseAutoGenOuterChecked
        {
            get { return this.useAutoGenOuterChecked; }
            set
            {
                useAutoGenOuterChecked = value;
                DoNotify();
            }
        }
        private bool useAutoGenInnerChecked = false;
        public bool UseAutoGenInnerChecked
        {
            get { return this.useAutoGenInnerChecked; }
            set
            {
                useAutoGenInnerChecked = value;
                DoNotify();
            }
        }


        private bool btnAutoGenOuterRegionEnable = false;
        public bool BtnAutoGenOuterRegionEnable
        {
            get { return this.btnAutoGenOuterRegionEnable; }
            set
            {
                btnAutoGenOuterRegionEnable = value;
                DoNotify();
            }
        }
        private bool btnAutoGenInnerRegionEnable = false;
        public bool BtnAutoGenInnerRegionEnable
        {
            get { return this.btnAutoGenInnerRegionEnable; }
            set
            {
                btnAutoGenInnerRegionEnable = value;
                DoNotify();
            }
        }

        private bool cobxUnionWayEnable = false;
        public bool CobxUnionWayEnable
        {
            get { return this.cobxUnionWayEnable; }
            set
            {
                cobxUnionWayEnable = value;
                DoNotify();
            }
        }
        private bool cobxUnionWay2Enable = false;
        public bool CobxUnionWay2Enable
        {
            get { return this.cobxUnionWay2Enable; }
            set
            {
                cobxUnionWay2Enable = value;
                DoNotify();
            }
        }

        private int morphProcessSelectIndex;
        public int MorphProcessSelectIndex
        {
            get { return this.morphProcessSelectIndex; }
            set
            {
                morphProcessSelectIndex = value;
                DoNotify();
            }
        }

        private int numRadius = 0;
        public int NumRadius
        {
            get { return this.numRadius; }
            set
            {
                numRadius = value;
                DoNotify();
            }
        }

        private int convertUnitsSelectIndex ;
        public int ConvertUnitsSelectIndex
        {
            get { return this.convertUnitsSelectIndex; }
            set
            {
                convertUnitsSelectIndex = value;
                DoNotify();
            }
        }
        private int unionWaySelectIndex;
        public int UnionWaySelectIndex
        {
            get { return this.unionWaySelectIndex; }
            set
            {
                unionWaySelectIndex = value;
                DoNotify();
            }
        }
        private int unionWay2SelectIndex ;
        public int UnionWay2SelectIndex
        {
            get { return this.unionWay2SelectIndex; }
            set
            {
                unionWay2SelectIndex = value;
                DoNotify();
            }
        }

        private string pixelRatio = "1";
        public string PixelRatio
        {
            get { return this.pixelRatio; }
            set
            {
                pixelRatio = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 区域类型索引
        /// </summary>
        private int manulRegionTypeSelectIndex = 0;
        public int ManulRegionTypeSelectIndex
        {
            get { return this.manulRegionTypeSelectIndex; }
            set
            {
                manulRegionTypeSelectIndex = value;
                DoNotify();
            }
        }

        private bool useManualDrawRegionChecked = false;
        public bool UseManualDrawRegionChecked
        {
            get { return this.useManualDrawRegionChecked; }
            set
            {
                useManualDrawRegionChecked = value;
                DoNotify();
            }
        }

        private bool btnManualDrawRegionEnable = false;
        public bool BtnManualDrawRegionEnable
        {
            get { return this.btnManualDrawRegionEnable; }
            set
            {
                btnManualDrawRegionEnable = value;
                DoNotify();
            }
        }

        private bool cobxUnionWay3Enable = false;
        public bool CobxUnionWay3Enable
        {
            get { return this.cobxUnionWay3Enable; }
            set
            {
                cobxUnionWay3Enable = value;
                DoNotify();
            }
        }

        private int unionWay3SelectIndex ;
        public int UnionWay3SelectIndex
        {
            get { return this.unionWay3SelectIndex; }
            set
            {
                unionWay3SelectIndex = value;
                DoNotify();
            }
        }


        private bool useManualDrawRegion2Checked = false;
        public bool UseManualDrawRegion2Checked
        {
            get { return this.useManualDrawRegion2Checked; }
            set
            {
                useManualDrawRegion2Checked = value;
                DoNotify();
            }
        }
        private bool btnManualDrawRegion2Enable = false;
        public bool BtnManualDrawRegion2Enable
        {
            get { return this.btnManualDrawRegion2Enable; }
            set
            {
                btnManualDrawRegion2Enable = value;
                DoNotify();
            }
        }

        private bool cobxUnionWay4Enable = false;
        public bool CobxUnionWay4Enable
        {
            get { return this.cobxUnionWay4Enable; }
            set
            {
                cobxUnionWay4Enable = value;
                DoNotify();
            }
        }

        private int unionWay4SelectIndex ;
        public int UnionWay4SelectIndex
        {
            get { return this.unionWay4SelectIndex; }
            set
            {
                unionWay4SelectIndex = value;
                DoNotify();
            }
        }

        private bool usePosiCorrectChecked = false;
        public bool UsePosiCorrectChecked
        {
            get { return this.usePosiCorrectChecked; }
            set
            {
                usePosiCorrectChecked = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 矩阵源集合
        /// </summary>
        private ObservableCollection<string> matrixList = new ObservableCollection<string>();
        public ObservableCollection<string> MatrixList
        {
            get { return this.matrixList; }
            set
            {
                matrixList = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择矩阵名称
        /// </summary>
        private string selectMatrixName = "";
        public string SelectMatrixName
        {
            get { return this.selectMatrixName; }
            set
            {
                selectMatrixName = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择矩阵索引编号
        /// </summary>
        private int selectMatrixIndex;
        public int SelectMatrixIndex
        {
            get { return this.selectMatrixIndex; }
            set
            {
                selectMatrixIndex = value;
                DoNotify();
            }
        }

        private bool matrixEnable = false;
        public bool MatrixEnable
        {
            get { return this.matrixEnable; }
            set
            {
                matrixEnable = value;
                DoNotify();
            }
        }

        private byte grayDown = 0;
        public byte GrayDown
        {
            get { return this.grayDown; }
            set
            {
                grayDown = value;
                DoNotify();
            }
        }

        private byte grayUp = 0;
        public byte GrayUp
        {
            get { return this.grayUp; }
            set
            {
                grayUp = value;
                DoNotify();
            }
        }
        private double areaDown = 0;
        public double AreaDown
        {
            get { return this.areaDown; }
            set
            {
                areaDown = value;
                DoNotify();
            }
        }

        private double areaUp = 0;
        public double AreaUp
        {
            get { return this.areaUp; }
            set
            {
                areaUp = value;
                DoNotify();
            }
        }

        private double offsetXDown = 0;
        public double OffsetXDown
        {
            get { return this.offsetXDown; }
            set
            {
                offsetXDown = value;
                DoNotify();
            }
        }

        private double offsetXUp = 0;
        public double OffsetXUp
        {
            get { return this.offsetXUp; }
            set
            {
                offsetXUp = value;
                DoNotify();
            }
        }

        private double offsetYDown = 0;
        public double OffsetYDown
        {
            get { return this.offsetYDown; }
            set
            {
                offsetYDown = value;
                DoNotify();
            }
        }

        private double offsetYUp = 0;
        public double OffsetYUp
        {
            get { return this.offsetYUp; }
            set
            {
                offsetYUp = value;
                DoNotify();
            }
        }

   

    }
}
