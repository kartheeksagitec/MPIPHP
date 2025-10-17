using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoSpinMVVM.BPMExecution.Controller
{   
    public class MapExecutionDetails
    {
        public List<ShapeDetails> lstShapes { get; set; }

        public List<clsExecutedStep> lstExecutedSteps { get; set; }

        public double Height;

        public double Width;

        public string WindowID;

        public string Title;

        public int ExecutedStepIndex;

        public MapExecutionDetails()
        {
            this.lstShapes = new List<ShapeDetails>();
            this.lstExecutedSteps = new List<clsExecutedStep>();
        }

    }
}