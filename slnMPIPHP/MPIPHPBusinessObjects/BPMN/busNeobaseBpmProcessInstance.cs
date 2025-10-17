
using MPIPHP.BusinessObjects;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace NeoBase.BPM
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busBpmProcessInstance:
    /// Inherited from busBpmProcessInstanceGen, the class is used to customize the business object busBpmProcessInstanceGen.
    /// </summary>
    [Serializable]
    public class busNeobaseBpmProcessInstance : busBpmProcessInstance
    {
        /// <summary>
        /// Property to get the Process Name.
        /// </summary>
        public string istrEvent_Associated_Process_Name { get; set; }
        /// <summary>
        /// Property to get the Process Name.
        /// </summary>
        public string istrEvent_Associated_Activity { get; set; }
        /// <summary>
        /// Property to get the Process Name.
        /// </summary>
        public string istrEvent_Associated_Action { get; set; }
        /// <summary>
        /// <summary>
        /// Property to get the Process Name.
        /// </summary>
        public string istrProcess_Name { get; set; }
        /// <summary>
        /// Property to get Action Value
        /// </summary>
        public string istrAction { get; set; }
        /// <summary>
        /// Property to get Action Value
        /// </summary>
        public string istrActivity { get; set; }

        /// <summary>
        /// Gets or sets property Process Instance Status
        /// </summary>
        public string istrProcessInstanceStatus { get; set; }

        /// <summary>
        /// Collection to hold activity filtered instances
        /// </summary>
        public Collection<busBpmActivityInstance> iclbFilteredBpmActivityInstance { get; set; }


        public busNeobaseBpmProcessInstance()
        {
            ibusPerson = busBase.CreateNewObject(BpmClientBusinessObjects.istrPerson);
            ibusOrganization = busBase.CreateNewObject(BpmClientBusinessObjects.istrOrganization);
        }

        /// <summary>
        /// Load method
        /// </summary>
        /// <param name="aintProcessInstanceId"></param>
        public void FindBPMProcessInstance(int aintProcessInstanceId)
        {
            ibusBpmCaseInstance.ibusBpmCase = busBpmCase.GetBpmCase(ibusBpmCaseInstance.icdoBpmCaseInstance.case_id);
            if (ibusBpmCaseInstance.ibusBpmCase != null)
            {
                foreach (busBpmProcess lbusBpmProcess in ibusBpmCaseInstance.ibusBpmCase.iclbBpmProcess)
                {
                    lbusBpmProcess.iclbBpmActivity.ForEach(bpmActivity => bpmActivity.LoadRoles());
                }
            }
            ibusBpmCaseInstance.LoadOrganization();
            ibusBpmCaseInstance.LoadPerson();
            ibusBpmCaseInstance.LoadBpmRequest();
            ibusBpmProcess = ibusBpmCaseInstance.ibusBpmCase.iclbBpmProcess.Where(process => process.icdoBpmProcess.process_id == icdoBpmProcessInstance.process_id).FirstOrDefault();
            ibusBpmProcess.iclbBpmActivity.ForEach(lbusBpmActivity => lbusBpmActivity.LoadRoles());
        }


        public override busBpmActivityInstance CreateActivityInstance(string astrBpmActivityID, busBpmActivityInstance abusInitiator, string astrStatusValue)
        {
            busBpmActivityInstance lobjNextElementInstance = base.CreateActivityInstance(astrBpmActivityID, abusInitiator, astrStatusValue);
            //SR
            if (lobjNextElementInstance.ibusBpmActivity.IsNotNull() && lobjNextElementInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.GENERATE_RETIREMENT_APPLICATION_CANCELLATION_NOTICE_ACTIVITY && astrStatusValue == busConstant.ReturnToWorkRequest.STATUS_UNPROCESSED)
            {
                busUser lbusUser = new busUser { icdoUser = new MPIPHP.CustomDataObjects.cdoUser() };
                if (iobjPassInfo.istrUserID != null && lbusUser.FindUserByUserName(iobjPassInfo.istrUserID))
                {
                    lobjNextElementInstance.icdoBpmActivityInstance.checked_out_user = iobjPassInfo.istrUserID;
                    lobjNextElementInstance.icdoBpmActivityInstance.checked_out_date = DateTime.Now;
                }
                lobjNextElementInstance.icdoBpmActivityInstance.status_value = busConstant.ReturnToWorkRequest.STATUS_INPROGRESS;
                lobjNextElementInstance.icdoBpmActivityInstance.Update();
            }
            return lobjNextElementInstance;
        }



        //public override void ProcessActivityInstance(busBpmActivityInstance aobjNextElementInstance, busBpmActivity aobjNextActivity, busBpmActivityInstance aobjInitiator, string astrUserIdToAssignUserTask = null, enmActivityInitiateType aenmActivityInitiateType = enmActivityInitiateType.InQueue)
        //{
        //    if (aobjNextElementInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.LEGAL_DOCUMENT_REVIEW_ACTIVITY ||
        //        aobjNextElementInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.GENERATE_RETIREMENT_APPLICATION_CANCELLATION_NOTICE_ACTIVITY)
        //    {
        //        string LastActivityName = Convert.ToString(aobjNextElementInstance.GetBpmParameterValue("LastActivityName"));
        //        string astrLastAssignedUserId = Convert.ToString(aobjNextElementInstance.GetBpmParameterValue("LastCheckOutUser"));
        //        if (LastActivityName == busConstant.PersonAccountMaintenance.QDRO_LEAGL_REVIEW_BENEFIT_ELECTION_ACTIVITY ||
        //            LastActivityName == busConstant.PersonAccountMaintenance.PERSON_ACCOUNT_RETIREMENT_INTAKE_ACTIVITY ||
        //            LastActivityName == busConstant.PersonAccountMaintenance.APPLICATION_SERVICE_RETIREMENT_PROCESS)
        //        {
        //            utlBPMElement lutlBPMElement = aobjNextElementInstance.ibusBpmActivity.GetBpmActivityDetails();

        //            DataTable ldtblist = busPerson.Select("entSolBpmActivityInstance.GetUserWithRoleId", new object[2] { DateTime.Now, aobjNextElementInstance.ibusBpmActivity.role_id });
        //            if (ldtblist.Rows.Count > 0)
        //            {
        //                string lstrSelectedUserId = string.Empty;
        //                long llngUserSerialId = 0;
        //                int lintSelectedIndex = 0;
        //                List<string> llstListOfUsers = new List<string>();
        //                bool lblnConsiderUser = true;

        //                if (ldtblist.Rows.Count > 0)
        //                {
        //                    foreach (DataRow ldrRow in ldtblist.Rows)
        //                    {
        //                        lblnConsiderUser = true;
        //                        lstrSelectedUserId = (string)ldrRow["USER_ID"];
        //                        llngUserSerialId = Convert.ToInt64(ldrRow["USER_SERIAL_ID"].ToString());
        //                        if (aobjNextElementInstance.IsUserAvailable(llngUserSerialId, lstrSelectedUserId))
        //                        {
        //                            if (aobjNextElementInstance.ibusBpmActivity.icdoBpmActivity.assignable_acty_count > 0)
        //                                lblnConsiderUser = (Convert.ToInt32(ldrRow["ASSIGNED_ACTIVITY_COUNT"]) < aobjNextElementInstance.ibusBpmActivity.icdoBpmActivity.assignable_acty_count);
        //                            if (lblnConsiderUser)
        //                            {
        //                                llstListOfUsers.Add(lstrSelectedUserId);
        //                            }
        //                        }
        //                    }
        //                    if (llstListOfUsers.Count > 0)
        //                    {
        //                        if (!llstListOfUsers.Contains(astrLastAssignedUserId))
        //                        {
        //                            llstListOfUsers.Add(astrLastAssignedUserId);
        //                        }
        //                        llstListOfUsers.Sort(StringComparer.OrdinalIgnoreCase);
        //                        lintSelectedIndex = llstListOfUsers.IndexOf(astrLastAssignedUserId);
        //                        if (lintSelectedIndex == llstListOfUsers.Count() - 1)//assigned user is last in sorted list then select first user fron list
        //                        {
        //                            lintSelectedIndex = 0;
        //                        }
        //                        else
        //                        {
        //                            lintSelectedIndex++;//select next user for round robin
        //                        }
        //                        if (lintSelectedIndex > -1 && lintSelectedIndex != llstListOfUsers.Count()) // check for valid item
        //                        {
        //                            astrUserIdToAssignUserTask = llstListOfUsers[lintSelectedIndex];
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    base.ProcessActivityInstance(aobjNextElementInstance, aobjNextActivity, aobjInitiator, astrUserIdToAssignUserTask, aenmActivityInitiateType);   
        //}

        /// <summary>
        /// Filter the Completed and Initiated Status ActivityInstance Record 
        /// </summary>
        public override void LoadBpmActivityInstances()
        {
            base.LoadBpmActivityInstances();

            //DataTable adtbList = busBase.Select<doBpmActivityInstance>(new string[1]
            //{
            //    enmBpmActivityInstance.process_instance_id.ToString()
            //}, new object[1]
            //{
            //    (object) this.icdoBpmProcessInstance.process_instance_id
            //}, (object[])null, (string)null, false);

            //this.iclbBpmActivityInstance = this.GetCollection<busBpmActivityInstance, busBpmActivityInstance>((busBpmActivityInstance)null, ClassMapper.GetObject<busBpmActivityInstance>(), adtbList, "icdoBpmActivityInstance");
            //foreach (busBpmActivityInstance activityInstance in this.iclbBpmActivityInstance)
            //{
            //    if(this.ibusBpmProcess == null)
            //    {
            //        this.LoadBpmProcess();
            //    }
            //    //activityInstance.ibusBpmActivity = this.ibusBpmProcess.GetActivity(activityInstance.icdoBpmActivityInstance.activity_id);
            //    if(this.iclbBpmActivity == null)
            //    {
            //        this.LoadBpmActivitys();
            //    }
            //    activityInstance.ibusBpmActivity =  this.iclbBpmActivity.Where<busBpmActivity>((Func<busBpmActivity, bool>)(lbusBpmAct => lbusBpmAct.icdoBpmActivity.activity_id == activityInstance.icdoBpmActivityInstance.activity_id)).FirstOrDefault<busBpmActivity>();
            //    activityInstance.ibusBpmProcessInstance = this;
            //}

            iclbFilteredBpmActivityInstance = iclbBpmActivityInstance;
        }

        //public string istrPersonOrOrganizationName
        //{
        //    get
        //    {
        //        if (ibusBpmCaseInstance.icdoBpmCaseInstance.person_id > 0)
        //        {
        //            if (ibusBpmCaseInstance.ibusPerson == null)
        //            {
        //                ibusPerson = new busPerson { icdoPerson = new doPerson() };
        //                ((busPerson)ibusPerson).FindByPrimaryKey(ibusBpmCaseInstance.icdoBpmCaseInstance.person_id);
        //            }
        //            return ((busPerson)ibusPerson).istrFullName;
        //        }
        //        else if (ibusBpmCaseInstance.icdoBpmCaseInstance.org_id > 0)
        //        {
        //            if (ibusBpmCaseInstance.ibusOrganization == null)
        //            {
        //                ibusOrganization = new busOrganization() { icdoOrganization = new doOrganization() };
        //                ((busOrganization)ibusOrganization).FindByPrimaryKey(ibusBpmCaseInstance.icdoBpmCaseInstance.org_id);
        //            }
        //            return ((busOrganization)ibusOrganization).icdoOrganization.org_name;
        //        }
        //        return "";
        //    }
        //}
    }
}
