using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlueDetectionLib.窗体.Models
{
    internal class GlueGapModel : NotifyBase
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
        ///选择黑白极性名称
        /// </summary>
        private string selectPolarityName = "";
        public string SelectPolarityName
        {
            get { return this.selectPolarityName; }
            set
            {
                selectPolarityName = value;
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

        private bool useAutoGenInner1Checked = false;
        public bool UseAutoGenInner1Checked
        {
            get { return this.useAutoGenInner1Checked; }
            set
            {
                useAutoGenInner1Checked = value;
                DoNotify();
            }
        }
        private bool useAutoGenInner2Checked = false;
        public bool UseAutoGenInner2Checked
        {
            get { return this.useAutoGenInner2Checked; }
            set
            {
                useAutoGenInner2Checked = value;
                DoNotify();
            }
        }


        private bool btnAutoGenInnerRegion1Enable = false;
        public bool BtnAutoGenInnerRegion1Enable
        {
            get { return this.btnAutoGenInnerRegion1Enable; }
            set
            {
                btnAutoGenInnerRegion1Enable = value;
                DoNotify();
            }
        }
        private bool btnAutoGenInnerRegion2Enable = false;
        public bool BtnAutoGenInnerRegion2Enable
        {
            get { return this.btnAutoGenInnerRegion2Enable; }
            set
            {
                btnAutoGenInnerRegion2Enable = value;
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

        private string morphProcessSelectName= "膨胀";
        public string MorphProcessSelectName
        {
            get { return this.morphProcessSelectName; }
            set
            {
                morphProcessSelectName = value;
                DoNotify();
            }
        }

        private int numRadius=0;
        public int NumRadius
        {
            get { return this.numRadius; }
            set
            {
                numRadius = value;
                DoNotify();
            }
        }

        private string convertUnitsSelectName= "像素";
        public string ConvertUnitsSelectName
        {
            get { return this.convertUnitsSelectName; }
            set
            {
                convertUnitsSelectName = value;
                DoNotify();
            }
        }
        private string unionWaySelectName = "+";
        public string UnionWaySelectName
        {
            get { return this.unionWaySelectName; }
            set
            {
                unionWaySelectName = value;
                DoNotify();
            }
        }
        private string unionWay2SelectName = "+";
        public string UnionWay2SelectName
        {
            get { return this.unionWay2SelectName; }
            set
            {
                unionWay2SelectName = value;
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

        private string unionWay3SelectName = "+";
        public string UnionWay3SelectName
        {
            get { return this.unionWay3SelectName; }
            set
            {
                unionWay3SelectName = value;
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

        private string unionWay4SelectName = "+";
        public string UnionWay4SelectName
        {
            get { return this.unionWay4SelectName; }
            set
            {
                unionWay4SelectName = value;
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

        private bool showBaseRegionChecked = false;
        public bool ShowBaseRegionChecked
        {
            get { return this.showBaseRegionChecked; }
            set
            {
                showBaseRegionChecked = value;
                DoNotify();
            }
        }

        private int cobxUnionWaySelect = 0;
        public int CobxUnionWaySelect
        {
            get { return this.cobxUnionWaySelect; }
            set
            {
                cobxUnionWaySelect = value;
                DoNotify();
            }
        }
        private int cobxUnionWay2Select = 0;
        public int CobxUnionWay2Select
        {
            get { return this.cobxUnionWay2Select; }
            set
            {
                cobxUnionWay2Select = value;
                DoNotify();
            }
        }

        private int cobxUnionWay3Select = 0;
        public int CobxUnionWay3Select
        {
            get { return this.cobxUnionWay3Select; }
            set
            {
                cobxUnionWay3Select = value;
                DoNotify();
            }
        }
        private int cobxUnionWay4Select = 0;
        public int CobxUnionWay4Select
        {
            get { return this.cobxUnionWay4Select; }
            set
            {
                cobxUnionWay4Select = value;
                DoNotify();
            }
        }


    }

}
