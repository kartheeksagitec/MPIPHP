using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoSpinMVVM.BPMExecution.Controller
{
    public class clsExecutedStep
    {

        public string ParentEleId;

        public bool IsCurrentNode;

        public bool IsExecuted;

        public bool IsBreakPointAddedForChild; //only for call activity

        public bool IsBreakPointAdded;

        public string ContainerElementId;

        public string ElementId;

        public byte[] ParametersSnapShot;

        public object Parameters;

        public List<clsExecutedStep> lstExecutedSteps;

        public string XMLFile;

        public int ActivityInstanceId;

        public string StartDate;

        public string EndDate;

        public string DueDate;

        //public List<clsEscalations> lstEscalations;
        public string EleType;



        public clsExecutedStep()
        {
            lstExecutedSteps = new List<clsExecutedStep>();
        }
    }
}