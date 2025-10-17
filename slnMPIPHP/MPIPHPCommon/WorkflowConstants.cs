using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.Common
{
    public class WorkflowConstants
    {       
        //Constants for Process Instance Status
        public const string WorkflowProcessInstanceStatusNotProcessed = "UNPC";
        public const string WorkflowProcessInstanceStatusProcessed = "PROC";
        public const string WorkflowProcessInstanceStatusInProgress = "INPC";
        public const string WorkflowProcessInstanceStatusAborted = "ABRT";

        ////Constants for ActivityInstance Status
        public const string ActivityInstanceStatusInitiated = "UNPC";        
        public const string ActivityInstanceStatusProcessed = "PROC";        
        public const string ActivityInstanceStatusSuspended = "SUSP";        
        public const string ActivityInstanceStatusCancelled = "CANC";        
        public const string WorkflowServiceURL = "NEOFLOW_SERVICE_WORKFLOW_URL";

        //Incoming Secure Message
        public const string IncomingSecureMessageXAML = "nfmIncomingSecureMessage";
        public const string IncomingSecureMessageXAML_Incoming_Message = "Incoming Message";

        // Member Crosswalk
        public const string ProcessMemberCrosswalkXAML = "nfmPersonPlanAdjustment";   

        ////Constants for identifying the process type
        public const string FlagYes = "Y";
        public const string FlagNo = "N";


        public const string SourceDescription = "Source_Description";
        public const string ProcessDescription = "Process_Description";
        public const string ProcessName = "Process_Name";
        

        ////Constants for datatype of the queue rule attributes
        public const string DATATYPE_STRING = "string";
        
        ////Constants for filter options.
        public const string ActivityInstanceStatus_INPC_Or_RESU = "'INPC','RESU'";
        public const string ActivityInstanceStatus_UNPC_Or_RELE = "'UNPC','RELE'";
        

        public const string BuildWhereClause_In = "in";
        public const string BuildWhereClause_And = " and ";

        public const string Operator_EqualTo = "=";        

        #region [Constants]

        public const int Message_Id_4076 = 4076;
        

        #endregion [Constants]
    }
}
