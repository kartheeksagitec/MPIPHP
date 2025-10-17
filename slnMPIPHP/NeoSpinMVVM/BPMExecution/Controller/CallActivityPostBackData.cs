using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoSpinMVVM.BPMExecution.Controller
{
   
    public class CallActivityPostBackData
    {
        public string astrXMLFile { get; set; }

        public string Title { get; set; }

        public List<clsExecutedStep> lstExecutedSteps { get; set; }

        public bool IsExecuted { get; set; }
    }
}