#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.Bpm;

#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busPersonOverviewWorkFlows : busPerson
    {
        public Collection<busActivityInstance> iclbPersonWorkflows { get; set; }

        public Collection<busBpmActivityInstanceHistory> iclbPersonBPMflows { get; set; }




        public void LoadParticipantWorkFlows()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.GetBPMDetailsforPerson", new object[1] { this.icdoPerson.person_id });
            // iclbPersonWorkflows = GetCollection<busActivityInstance>(ldtblist, "icdoActivityInstance");
            iclbPersonBPMflows = GetCollection<busBpmActivityInstanceHistory>(ldtblist, "icdoBpmActivityInstanceHistory");
            foreach (busBpmActivityInstanceHistory lbusActivityInstancehistory in iclbPersonBPMflows)
            {
                lbusActivityInstancehistory.LoadBpmActivityInstance();    
                lbusActivityInstancehistory.ibusBpmActivityInstance.LoadBpmActivity();
                lbusActivityInstancehistory.ibusBpmActivityInstance.LoadBpmProcessInstance();
                lbusActivityInstancehistory.ibusBpmActivityInstance.ibusBpmProcessInstance.LoadBpmProcess();
            }
            

        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busActivityInstance)
            {
                busActivityInstance lbusActivityInstance = (busActivityInstance)aobjBus;

                lbusActivityInstance.ibusActivity = new busActivity { icdoActivity = new cdoActivity() };
                lbusActivityInstance.ibusActivity.ibusRoles = new busRoles { icdoRoles = new cdoRoles() };

                if (!Convert.IsDBNull(adtrRow["Display_Name"]))
                {
                    lbusActivityInstance.ibusActivity.icdoActivity.display_name = adtrRow["Display_Name"].ToString();
                }

                lbusActivityInstance.ibusProcessInstance = new busProcessInstance { icdoProcessInstance = new cdoProcessInstance() };
                lbusActivityInstance.ibusProcessInstance.icdoProcessInstance.process_instance_id = lbusActivityInstance.icdoActivityInstance.process_instance_id;
                lbusActivityInstance.ibusProcessInstance.ibusProcess = new busProcess { icdoProcess = new cdoProcess() };
                //lobjActivityInstance.ibusProcessInstance.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                //lobjActivityInstance.ibusProcessInstance.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };

                lbusActivityInstance.ibusProcessInstance.ibusWorkflowRequest = new busWorkflowRequest { icdoWorkflowRequest = new cdoWorkflowRequest() };


                if (!Convert.IsDBNull(adtrRow["Process_Name"]))
                {
                    lbusActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.name = adtrRow["Process_Name"].ToString();
                    lbusActivityInstance.icdoActivityInstance.istrProcessName = adtrRow["Process_Name"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["Process_Description"]))
                {
                    lbusActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.description = adtrRow["Process_Description"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["Source_Description"]))
                {
                    lbusActivityInstance.ibusProcessInstance.ibusWorkflowRequest.icdoWorkflowRequest.source_description = adtrRow["Source_Description"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["STATUS_DESCRIPTION"]))
                {
                    lbusActivityInstance.ibusProcessInstance.icdoProcessInstance.status_description = adtrRow["STATUS_DESCRIPTION"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["START_DATE"]))
                {
                    lbusActivityInstance.icdoActivityInstance.START_DATE = Convert.ToDateTime(adtrRow["START_DATE"]);
                }


                if (!Convert.IsDBNull(adtrRow["END_DATE"]))
                {
                    lbusActivityInstance.icdoActivityInstance.END_DATE = Convert.ToDateTime(adtrRow["END_DATE"]);
                }


                if (!Convert.IsDBNull(adtrRow["UserId"]))
                    lbusActivityInstance.icdoActivityInstance.UserId = adtrRow["UserId"].ToString();
                {
                }
            }
        }
    }
}
