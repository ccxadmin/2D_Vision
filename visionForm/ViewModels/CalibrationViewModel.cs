﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using visionForm.Models;

namespace visionForm.ViewModels
{
    public class CalibrationViewModel
    {
        public static CalibrationViewModel This { get; set; }
        public CalibrationModel Model { get; set; }

        public CalibrationViewModel()
        {         
            This = this;
            Model = new CalibrationModel();
        }
    }
}
