using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace visionForm.Models
{
    public class CalibrationModel : NotifyBase
    {
        public CalibrationModel()
        {
            recipeDgList = new ObservableCollection<RecipeDg>();
        }
        
        public ObservableCollection<RecipeDg> recipeDgList = null;
        public ObservableCollection<RecipeDg> RecipeDgList
        {
            get { return this.recipeDgList; }
            set
            {
                recipeDgList = value;
                DoNotify();
            }
        }


    }


    public class RecipeDg
    {
        public RecipeDg()
        {
            Name = "default";
            IsUse = false;
        }
        public RecipeDg(string name, bool isUse)
        {
            Name = name;
            IsUse = isUse;
        }
       public string Name { get; set; }
       public bool IsUse { get; set; }
      
    }

}
