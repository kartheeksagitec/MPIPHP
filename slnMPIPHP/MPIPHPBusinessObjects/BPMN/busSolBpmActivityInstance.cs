#region [Using Directives]
using MPIPHP.BusinessObjects;
using MPIPHP.Common;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using NeoBase.BPM;
using NeoBase.Common;
using NeoSpinConstants;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using busNotes = MPIPHP.BusinessObjects.busNotes;
//using busNotes = NeoBase.Common.busNotes;

#endregion [Using Directives]

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busBpmActivityInstance:
    /// Inherited from busBpmActivityInstanceGen, the class is used to customize the business object busBpmActivityInstanceGen.
    /// </summary>
    [Serializable]
    public class busSolBpmActivityInstance : busNeobaseBpmActivityInstance //busBpmActivityInstance
    {

        #region [Properties] Commented Define in busNeobaseBpmActivityInstance

        // /// <summary>
        // /// Property to get the Person details.
        // /// </summary>
        // public int iintPerson_Id { get; set; }

        // /// <summary>
        // /// Property to get the Organization details.
        // /// </summary>
        // public int iintOrg_Id { get; set; }

        // /// <summary>
        // /// Property to get the Source.
        // /// </summary>
        // public string istrSource { get; set; }

        // /// <summary>
        // /// Property to get the Organization details.
        // /// </summary>
        // public DateTime idtCheckedOutDate { get; set; }

        // /// <summary>
        // /// Property to get the PRIORITY details.
        // /// </summary>
        // public string istrPriority { get; set; }

        // /// <summary>
        // /// Property to get the Process Instance Created Date.
        // /// </summary>
        // public DateTime idtInitiatedDate { get; set; }

        // /// <summary>
        // /// Property to get the Activity Instance Start Date details.
        // /// </summary>
        // public DateTime idtStartDate { get; set; }

        // /// <summary>
        // /// Property to get the Activity Instance END Date details.
        // /// </summary>
        // public DateTime idtEndDate { get; set; }

        // /// <summary>
        // /// Property to get the Person Name.
        // /// </summary>
        // public string istrPersonName { get; set; }

        // /// <summary>
        // /// Property to get the Org Name.
        // /// </summary>
        // public string istrOrgName { get; set; }

        // /// <summary>
        // /// Property to get the Launch details.
        // /// </summary>
        // public string istrLaunch { get { return "Launch"; } }

        // /// <summary>
        // /// Property to get the Checkout details.
        // /// </summary>
        // public string istrCheckout { get { return "Checkout"; } }

        // /// <summary>
        // /// Property to get the Resume details.
        // /// </summary>
        // public string istrResume { get { return "Resume"; } }

        // /// <summary>
        // /// Property to get the Unassign details.
        // /// </summary>
        // public string istrUnassign { get { return "Unassign"; } }

        // public string istrIsTerminated { get; set; }

        // /// <summary>
        // /// Property used to set grid record color based on error.
        // /// 
        // /// </summary>
        // public string istrReassignErrorCSS { get; set; }

        // /// <summary>
        // /// Property to hold all the Escalation messages collection.
        // /// </summary>
        // public Collection<busSolBpmUsersEscalationMessage> iclbEscalationMessages { get; set; }

        // /// <summary>
        // /// Property to hold the holiday List.
        // /// </summary>
        // public Collection<DateTime> iclbHolidayList { get; set; }

        // /// <summary>
        // /// Property to hold the Document List.
        // /// </summary>
        // public Collection<busBpmProcessInstanceAttachments> iclbDocuments { get; set; }

        // /// <summary>
        // /// Collection for process instance notes.
        // /// </summary>
        // public Collection<busNotes> iclbProcessInstanceNotes { get; set; }

        // /// <summary>
        // /// Collection of activities assigned to the user.
        // /// </summary>
        // //public Collection<busSolBpmActivityInstance> iclbUserAssignedActivities { get; set; }

        // /// <summary>
        // /// Collection of BPM Escalation.
        // /// </summary>
        // public Collection<busBpmEscalation> iclbBpmEscalation { get; set; }

        // /// <summary>
        // /// Property to hold the Document Upload
        // /// </summary>
        //// public busBpmDocumentUpload ibusDocumentUpload { get; set; }

        // ///// <summary>
        // ///// property to hold the Process Instance 
        // ///// </summary>
        // public busBpmProcessInstance ibusProcessInstance { get; set; }

        // /// <summary>
        // /// Collection for Notification Messages.
        // /// </summary>
        // public Collection<busBpmProcessInstance> iclbBpmProcessInstance { get; set; }

        // ///// <summary>
        // ///// Collection for uploaded Document
        // ///// </summary>
        // public Collection<busBpmDocumentUpload> iclbBpmDocumentUplaod { get; set; }

        // ///// <summary>
        // ///// Collection to upload file 
        // ///// </summary>
        // public Dictionary<string, Collection<utlPostedFile>> idictHttpPostedFiles { get; set; }

        // /// <summary>
        // /// Used to get value whether 
        // /// </summary>
        // public bool iblnHasAssignedActivities
        // {
        //     get
        //     {
        //         //return true;
        //         if (iclbUserAssignedActivities.IsNotNull() && iclbUserAssignedActivities.Count > 0)
        //             return true;
        //         else
        //             return false;
        //     }
        // }
        public string ReWorkFlag { get; set; }

        public DateTime idtRetirementDate { get; set; }

        public string istrPlanDescription { get; set; }

        /// <summary>
        /// Collection for Restrict Notification Message.
        /// </summary>
        public Collection<busSolBpmProcessInstanceRestrictNotifyXr> iclbSolBpmProcessInstanceRestrictNotifyXr { get; set; }
        #endregion

        #region [Properties]

        ///// <summary>
        ///// Property to hold the Activity Instance of Person.
        ///// </summary>
        public busPerson ibusSolActivityInstancePerson { get; set; }
        ///// <summary>
        ///// Property to hold the Activity Instance of Organization.
        ///// </summary>
        public busOrganization ibusSolActivityInstanceOrganization { get; set; }


        // /// <summary>
        // /// Property to hold the assigned user tasks escalation messages collection.
        // /// </summary>
        //public Collection<busSolBpmUsersEscalationMessage> iclbAssignedActivitiesEscalationMessages { get; set; }

        // /// <summary>
        // /// Property to hold the collection for user tasks escalation messages assigned to others.
        // /// </summary>
        //public Collection<busSolBpmUsersEscalationMessage> iclbActivitiesEscalationMessagesAssignedToOthers { get; set; }

        // /// <summary>
        // /// Property to hold the process escalation messages collection.
        // /// </summary>
        // public Collection<busSolBpmUsersEscalationMessage> iclbProcessEscalationMessages { get; set; }
        
        #endregion [Properties]

        #region [Constructor]

        public busSolBpmActivityInstance()
            : base()
        {
            iclbBpmActivityInstanceChecklist = new Collection<busBpmActivityInstanceChecklist>();
            idictHttpPostedFiles = new Dictionary<string, Collection<utlPostedFile>>();
        }

        #endregion [Constructor]

        #region [Overridden Methods]
        /// <summary>
        /// This function is used to Load Other Objects of the current Activity Instance.
        /// </summary>
        /// <param name="adtrRow"></param>
        /// <param name="abusBusBase"></param>
        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBusBase)
        {
            if (abusBusBase is busNeobaseBpmActivityInstance)
            {
                //busSolBpmActivityInstance
                busNeobaseBpmActivityInstance lbusActivityInstance = (busNeobaseBpmActivityInstance)abusBusBase;
                lbusActivityInstance.ibusBpmActivity = busBpmActivity.GetBpmActivityByActivityType(adtrRow[enmBpmActivity.activity_type_value.ToString()].ToString());
                if (!Convert.IsDBNull(adtrRow[enmBpmActivity.name.ToString()]))//enmBpmActivity.name
                {
                    lbusActivityInstance.ibusBpmActivity.icdoBpmActivity.name = adtrRow[enmBpmActivity.name.ToString()].ToString();
                }

                if (!Convert.IsDBNull(adtrRow[enmBpmActivity.process_id.ToString()]))
                {
                    lbusActivityInstance.ibusBpmActivity.icdoBpmActivity.process_id = Convert.ToInt32(adtrRow[enmBpmActivity.process_id.ToString()]);
                }

                lbusActivityInstance.ibusBpmProcessInstance = new busSolBpmProcessInstance();
                lbusActivityInstance.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id = lbusActivityInstance.icdoBpmActivityInstance.process_instance_id;
                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess = new busBpmProcess();
                //busSolBpmCaseInstance
                busNeobaseBpmCaseInstance lbusSolBpmCaseInstance = new busNeobaseBpmCaseInstance();
                lbusSolBpmCaseInstance.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lbusSolBpmCaseInstance.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance = lbusSolBpmCaseInstance;


                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest = new busBpmRequest();
                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.LoadData(adtrRow);

                if (!Convert.IsDBNull(adtrRow[enmBpmProcessInstance.process_instance_id.ToString()]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id = Convert.ToInt32(adtrRow[enmBpmProcessInstance.process_instance_id.ToString()]);
                    lbusActivityInstance.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id = Convert.ToInt32(adtrRow[enmBpmProcessInstance.process_instance_id.ToString()]);
                }

                if (!Convert.IsDBNull(adtrRow[enmPerson.person_id.ToString()]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                    if (lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance is busSolBpmCaseInstance)
                    {
                        ((busPerson)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusPerson).icdoPerson.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                        ((busPerson)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusPerson).icdoPerson.first_name = adtrRow[enmPerson.first_name.ToString()].ToString();
                        ((busPerson)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusPerson).icdoPerson.last_name = adtrRow[enmPerson.last_name.ToString()].ToString();
                        ((busPerson)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusPerson).icdoPerson.middle_name = adtrRow[enmPerson.middle_name.ToString()].ToString();

                    }
                }

                if (!Convert.IsDBNull(adtrRow[enmOrganization.org_id.ToString()]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.org_id = Convert.ToInt32(adtrRow[enmOrganization.org_id.ToString()]);
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.org_id = Convert.ToInt32(adtrRow[enmOrganization.org_id.ToString()]);
                    if (lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance is busSolBpmCaseInstance)
                    {
                        ((busOrganization)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusOrganization).icdoOrganization.org_id = Convert.ToInt32(adtrRow[enmOrganization.org_id.ToString()]);
                        ((busOrganization)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusOrganization).icdoOrganization.org_name = adtrRow[enmOrganization.org_name.ToString()].ToString();

                    }
                }

                if (!Convert.IsDBNull(adtrRow[BpmCommon.ProcessName]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name = adtrRow[BpmCommon.ProcessName].ToString();
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name = adtrRow[BpmCommon.ProcessName].ToString();
                }

                if (!Convert.IsDBNull(adtrRow[BpmCommon.ProcessDescription]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.description = adtrRow[BpmCommon.ProcessDescription].ToString();
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.description = adtrRow[BpmCommon.ProcessDescription].ToString();
                }

                if (!Convert.IsDBNull(adtrRow[BpmCommon.SourceDescription]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_description = adtrRow[BpmCommon.SourceDescription].ToString();
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_description = adtrRow[BpmCommon.SourceDescription].ToString();
                }
            }
            else if (abusBusBase is busNeobaseBpmUsersEscalationMessage)
            {
                ((busNeobaseBpmUsersEscalationMessage)abusBusBase).CallLoadOtherObjects(adtrRow, abusBusBase);
            }
            else if (abusBusBase is busBpmProcessInstanceAttachments)
            {
                busBpmProcessInstanceAttachments lbusBpmProcessInstanceAttachments = abusBusBase as busBpmProcessInstanceAttachments;
                if (lbusBpmProcessInstanceAttachments.ibusBpmEvent == null)
                {
                    lbusBpmProcessInstanceAttachments.ibusBpmEvent = new busBpmEvent { icdoBpmEvent = new doBpmEvent() };
                }
                if (adtrRow.Table.Columns.Contains("EVENT_DESC"))
                {
                    lbusBpmProcessInstanceAttachments.ibusBpmEvent.icdoBpmEvent.event_desc = adtrRow["EVENT_DESC"].ToString();
                }
            }
            else if (abusBusBase is busSolBpmProcessInstanceRestrictNotifyXr)
            {
                if (!Convert.IsDBNull(adtrRow["NOTIFICATION_MESSAGE"]))
                {
                    ((busSolBpmProcessInstanceRestrictNotifyXr)abusBusBase).istrNotificationMessages = adtrRow["NOTIFICATION_MESSAGE"].ToString();
                }
            }
        }

        /// <summary>
        /// This function is used to Load the related objects of the Process Instance.
        /// </summary>
        public override void LoadRelatedObjects()
        {
            this.LoadProcessInstanceNotes();
        }

        public override void LoadProcessInstanceNotes()
        {
            int iintRequestId = 0;
            if (ibusBpmProcessInstance==null)
            {
                LoadBpmProcessInstance();
            }
           if (ibusBpmProcessInstance.ibusBpmCaseInstance != null)
            {
                iintRequestId = ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.request_id;
            }
            if (icdoBpmActivityInstance.process_instance_id > 0)
            {
                DataTable ldtbNotes = busMPIPHPBase.Select("cdoNotes.FindProcessInstanceNotes", new object[1] { icdoBpmActivityInstance.process_instance_id });
                iclbProcessInstanceNotes = GetCollection<busNotes>(ldtbNotes, "icdoNotes");
            }
        }
        /// <summary>
        /// Check user is available for the curent date or not.. 
        /// </summary>
        /// <param name="alngUserSerialId">User Serial Id</param>
        /// <param name="astrUserId">User Id</param>
        /// <returns></returns>
        public override bool IsUserAvailable(long alngUserSerialId, string astrUserId)
        {
            int lintCount = 0;
            if (alngUserSerialId > 0)
            {
                lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("entUserTimeoff.GetUserAvailableCountByUserSerialId",
                new object[1] { Convert.ToInt32(alngUserSerialId) }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            }
            else if (!string.IsNullOrEmpty(astrUserId))
            {
                lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("entUserTimeoff.GetUserAvailableCountByUserId",
                new object[1] { astrUserId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            }
            return !(lintCount > 0);
        }

        public override ArrayList UpdateBpmActivityInstanceByStatus(string astrStatus, bool ablnThrowException = true)
        {
            if (astrStatus == BpmActivityInstanceStatus.Resumed && icdoBpmActivityInstance.due_date == DateTime.MinValue)
                icdoBpmActivityInstance.due_date = busNeoBaseGlobalFunctions.GetSystemDate();
            return base.UpdateBpmActivityInstanceByStatus(astrStatus, ablnThrowException);
        }

        public override Collection<busBpmResumeAction> GetResumeActions()
        {
            List<utlCodeValue> lcolResumeActions = iobjPassInfo.isrvDBCache.GetCodeValuesFromDict(busConstant.BPM.RESUME_ACTION_CODE_ID);

            Collection<busBpmResumeAction> lclbResumeAction = new Collection<busBpmResumeAction>();

            if (lcolResumeActions != null && lcolResumeActions.Count > 0)
            {
                IOrderedEnumerable<utlCodeValue> lcolOrderedList = lcolResumeActions.OrderBy(resumeAction => resumeAction.code_value_order);
                foreach (utlCodeValue lobjResumeAction in lcolOrderedList)
                {
                    busBpmResumeAction lbusActionAnyDocument = new busBpmResumeAction();
                    lbusActionAnyDocument.istrDescription = lobjResumeAction.description;
                    lbusActionAnyDocument.istrValue = lobjResumeAction.code_value;
                    lclbResumeAction.Add(lbusActionAnyDocument);
                }
            }
            return lclbResumeAction;
        }

        public override DataTable GetFilteredEligibleUserList(DataTable adtUserList)
        {
            if (adtUserList != null)
            {
                if (ibusBpmActivity?.icdoBpmActivity?.name == busConstant.ReturnToWorkRequest.ACTIVITY_ERTW_CONDUCT_SECOND_AUDIT)
                {
                    int lintFirstActivityUser = (int)GetBpmParameterValue("FirstActivityUser"); ;
                    int lintFirstAuditUser = (int)GetBpmParameterValue("FirstAuditUser");

                    DataRow[] ldtrUserExists = adtUserList.Select("USER_SERIAL_ID = '" + lintFirstActivityUser + "'");
                    if (ldtrUserExists.Length != 0)
                        ldtrUserExists.ForEach(e => e.Delete());

                    ldtrUserExists = adtUserList.Select("USER_SERIAL_ID = '" + lintFirstAuditUser + "'");
                    if (ldtrUserExists.Length != 0)
                        ldtrUserExists.ForEach(e => e.Delete());

                    adtUserList.AcceptChanges();
                }
            }
            return adtUserList;
        }
        #endregion [Overridden Methods]

        #region [Public Methods]

        /// <summary>
        /// Load Method
        /// Dont move this to new project beacuse partial class(busPerson,busOrganization)
        /// </summary>
        public void LoadBpmProcessInstanceDetails()
        {
            // Developer : Rahul Mane
            // Date : 07-30-2021
            // PIR - 3688 - System does not load My Tasks panel and shows an error
            // Iteration - Main-Iteration10
            if (ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id > 0)
            {
                ibusBpmProcessInstance.LoadBpmProcess();
                ibusBpmProcessInstance.LoadBpmCaseInstance();
                ibusBpmProcessInstance.ibusBpmCaseInstance.LoadBpmCase();
                ibusBpmProcessInstance.LoadPerson();
                if (ibusBpmProcessInstance.ibusBpmCaseInstance != null)
                {
                    if (ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id > 0)
                    {
                        ibusSolActivityInstancePerson = new busPerson { icdoPerson = new cdoPerson() };
                        ibusSolActivityInstancePerson.FindByPrimaryKey(ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id);
                    }
                    else
                    {
                        ibusSolActivityInstanceOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                        ibusSolActivityInstanceOrganization.FindByPrimaryKey(ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.org_id);
                    }
                }
            }
        }

        /// <summary>
        /// This function is used to reload the center left navigation details.
        /// </summary>
        /// <param name="aintActivityInstanceID">Activity instance id</param>
        /// <returns></returns>
        public ArrayList ReloadCenterleft(int aintActivityInstanceID)
        {
            ArrayList larrResult = new ArrayList();
            LoadCenterleftObjects(aintActivityInstanceID);
            EvaluateInitialLoadRules();
            larrResult.Add(this);
            return larrResult;
        }

        /// <summary>
        /// This function is used to load the details of the current activity instance into the center left navigation panel.
        /// </summary>
        /// <param name="aintActivityInstanceID">Activity instance id.</param>
        public void LoadCenterleftObjects(int aintActivityInstanceID)
        {
            LoadAndCheckoutActivity(); //base
            if (aintActivityInstanceID > 0)
            {
                FindByPrimaryKey(aintActivityInstanceID);
            }
            if ((aintActivityInstanceID == 0 && iclbUserAssignedActivities.IsNotNullOrEmpty() && iclbUserAssignedActivities.Count > 0) ||
                 //(!(FindByPrimaryKey(aintActivityInstanceID)) && (iclbUserAssignedActivities.Count > 0)) ||
                 ((iclbUserAssignedActivities.Count > 0) && (!IsActivityInstanceAssignable()) && (iobjPassInfo.istrSenderForm != "wfmBPMProcessInstanceMaintenance" && iobjPassInfo.istrSenderForm != "wfmBPMMyBasketMaintenance" && iobjPassInfo.istrSenderForm != "wfmBpmReassignWorkMaintenance" && iobjPassInfo.istrSenderForm != "wfmBpmActivityInstanceMaintenance")))
            {
                aintActivityInstanceID = iclbUserAssignedActivities[0].icdoBpmActivityInstance.activity_instance_id;
            }

            //if no activities in pool
            if (iclbUserAssignedActivities.Count == 0)
                return;

            if (aintActivityInstanceID > 0)
            {
                //FindByPrimaryKey(aintActivityInstanceID);
                /*busSolBpmActivityInstance*/
                busNeobaseBpmActivityInstance lbusSolBpmActivityInstance = iclbUserAssignedActivities.Where(objActivityInstance => objActivityInstance.icdoBpmActivityInstance.activity_instance_id == aintActivityInstanceID).FirstOrDefault();
                if (lbusSolBpmActivityInstance == null)
                {
                    if (this.FindByPrimaryKey(aintActivityInstanceID))
                    {
                        this.LoadBpmActivity();
                        this.LoadBpmProcessInstance();
                        this.ibusBpmProcessInstance.LoadBpmCaseInstance();
                        // Developer: Ratnesh
                        // Date: 29 March 2023
                        // Iteration: 13.1
                        // Comment: Fixed-PIR-4375 The 'Termination Reason' which is required while terminating the activity is not showing after refreshing the screen.
                        this.istrTerminationReason = this.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.termination_reason;
                        this.ibusBpmProcessInstance.LoadPerson();
                        this.ibusSolActivityInstancePerson = (busPerson)this.ibusBpmProcessInstance.ibusPerson;
                        this.LoadProcessInstanceNotes();
                        if (this.icdoBpmActivityInstance.initiator_instance_id > 0)
                        {
                            this.ibpmInitiator = new busBpmActivityInstance();
                            this.ibpmInitiator.FindByPrimaryKey(this.icdoBpmActivityInstance.initiator_instance_id);
                        }
                    }
                }
                else
                {
                    LoadProcessInstanceNotes();
                    this.icdoBpmActivityInstance = lbusSolBpmActivityInstance.icdoBpmActivityInstance;
                    this.ibpmInitiator = lbusSolBpmActivityInstance.ibpmInitiator;
                    this.ibusBpmActivity = lbusSolBpmActivityInstance.ibusBpmActivity;
                    this.ibusBpmProcessInstance = lbusSolBpmActivityInstance.ibusBpmProcessInstance;
                    this.iclbProcessInstanceNotes = lbusSolBpmActivityInstance.iclbProcessInstanceNotes;
                }

                if (iclbUserAssignedActivities != null && iclbUserAssignedActivities.Count > 0)
                {
                    foreach (busNeobaseBpmActivityInstance lbusTemp in iclbUserAssignedActivities)
                    {
                        if (lbusTemp.icdoBpmActivityInstance.activity_instance_id == aintActivityInstanceID)
                        {
                            lbusTemp.istrIsActivitySelected = "Y";
                        }
                        else
                        {
                            lbusTemp.istrIsActivitySelected = "N";
                        }
                    }
                }
            }
            LoadProcessInstanseRestrictNotifyXR();
            LoadEscalationMessages();
            LoadDocuments();

        }

        /// <summary>
        /// Load Collection For Restrict Notification 
        /// </summary>
        public void LoadProcessInstanseRestrictNotifyXR()
        {
            object[] larrParameters = new object[1] { iobjPassInfo.istrUserID };
            DataTable ldtSolBpmProcessInstanceRestrictNotifyXr = DBFunction.DBSelect("entBpmProcessInstanceRestrictNotifyXr.GetAllBpmProcessInstanceRestrictNotifyMessagesForUser", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbSolBpmProcessInstanceRestrictNotifyXr = GetCollection<busSolBpmProcessInstanceRestrictNotifyXr>(ldtSolBpmProcessInstanceRestrictNotifyXr, "icdoBpmProcessInstanceRestrictNotifyXr");
        }

        /// <summary>
        /// This method is used to load all the Escalation Messages.
        /// </summary>
        public void LoadEscalationMessages()
        {//busSolBpmUsersEscalationMessage
            object[] larrParameters = new object[] { iobjPassInfo.iintUserSerialID, iobjPassInfo.istrUserID };
            DataTable ldtbAssignedActivitiesEscalationMessages = DBFunction.DBSelect("entBpmUsersEscalationMessage.UserTaskEscalationMessagesAssignedToUser", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbAssignedActivitiesEscalationMessages = GetCollection<busNeobaseBpmUsersEscalationMessage>(ldtbAssignedActivitiesEscalationMessages);

            larrParameters = new object[] { iobjPassInfo.iintUserSerialID, iobjPassInfo.istrUserID };
            DataTable ldtbEscalationMessagesAssignedToOtherUser = DBFunction.DBSelect("entBpmUsersEscalationMessage.UserTaskEscalationMessagesAssignedToOtherUser", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbActivitiesEscalationMessagesAssignedToOthers = GetCollection<busNeobaseBpmUsersEscalationMessage>(ldtbEscalationMessagesAssignedToOtherUser);

            larrParameters = new object[] { iobjPassInfo.iintUserSerialID };
            DataTable ldtbProcessEscalationMessages = DBFunction.DBSelect("entBpmUsersEscalationMessage.ProcessEscalationMessages", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbProcessEscalationMessages = GetCollection<busNeobaseBpmUsersEscalationMessage>(ldtbProcessEscalationMessages);
        }
       
        /// <summary>
        /// This function is used to update the Escalation Messages of the current Activity Instance.
        /// </summary>
        /// <param name="aintActivityInstanceID">Activity instance ID</param>
        /// <returns></returns>
        public ArrayList UpdateEscalationMessages(int aintActivityInstanceID)
        {
            ArrayList larrResult = new ArrayList();
            this.PersistChanges();
            this.LoadEscalationMessages();
            larrResult.Add(this);
            return larrResult;
        }
     
        /// <summary>
        /// This function is used to load and checkout the assigned activities of the logged in user. 
        /// </summary>
        public void LoadAndCheckoutActivity()
        {
            LoadAssignedActivities();

            //Check if the logged in user is on vacation then don't assign any workitem to him.
            busUser lbusUser = new busUser() { icdoUser = new cdoUser() };
            lbusUser.FindByPrimaryKey(utlPassInfo.iobjPassInfo.iintUserSerialID);

            //If the user marks himself as unavailable then don't assign any activity for that user.
            if (IsUserAvailable(utlPassInfo.iobjPassInfo.iintUserSerialID, utlPassInfo.iobjPassInfo.istrUserID))
                return;

            //Check if the logged in user is an external user if so, don't checkout the activity.
            if (iblnIsExternalSearch)
            {
                return;
            }
        }

        /// <summary>
        /// This function is used to get the assigned activities of the logged in user.
        /// </summary>
        public void LoadAssignedActivities()
        {
            string lstrQuery;
            Collection<utlWhereClause> lcolWhereClause = null;
            utlMethodInfo lutlMethodInfo;

            //Initialize the Collection to Avoid Null Exception
            iclbUserAssignedActivities = new Collection<busNeobaseBpmActivityInstance>();

            //Assign the Query Name By the Selected Filter
            lstrQuery = "MyBasketBaseQuery";

            //Load the only activities which are assigned to the current user
            lcolWhereClause = BuildWhereClause(lstrQuery, BpmCommon.INPC_Or_RESU);


            lutlMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("entBpmActivityInstance." + lstrQuery);
            lstrQuery = lutlMethodInfo.istrCommand;
            Collection<IDbDataParameter> lcolParams = new Collection<IDbDataParameter>();
            string lstrFinalQuery = lstrQuery + " , activity_instance_id desc ";
            lstrFinalQuery = sqlFunction.AppendWhereClause(lstrFinalQuery, lcolWhereClause, lcolParams, iobjPassInfo.iconFramework);

            DataTable ldtbList = DBFunction.DBSelect(lstrFinalQuery, lcolParams,
                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbUserAssignedActivities = GetCollection<busNeobaseBpmActivityInstance>(ldtbList, "icdoBpmActivityInstance");

        }

        /// <summary>
        /// This method is used to invoke the Work flow action of the current Activity Instance.
        /// </summary>
        /// <returns></returns>
        public new ArrayList InvokeWorkflowAction()
        {
            ArrayList larrResult;
            utlError lutlError = new utlError();
            busBpmCaseInstance lbusBpmCaseInstance = ClassMapper.GetObject<busBpmCaseInstance>(); ;
            busBpmActivityInstance lbusBpmActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst(icdoBpmActivityInstance.activity_instance_id);
            istrResumeActionValue = icdoBpmActivityInstance.resume_action_value;
            //FM 6.0.0.35 change
            if (utlPassInfo.iobjPassInfo.istrPostBackControlID == busNeoBaseConstants.BPM.BTN_TERMINATE_ACTIVITY)
            {
                lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.termination_reason = this.istrTerminationReason;
            }
            if (utlPassInfo.iobjPassInfo.istrPostBackControlID == "btnCompleteActivity")
            {
                if (!HasAllRequiredChecklistsCompleted())
                {
                    larrResult = new ArrayList();
                    lutlError = AddError(1565, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }
                if (IsCompletedDateFutureDate())
                {
                    larrResult = new ArrayList();
                    lutlError = AddError(1568, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }
                if (this.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.GENERATE_RETIREMENT_APPLICATION_CANCELLATION_NOTICE_ACTIVITY)
                {
                    int iintReferenceId = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue("ReferenceId"));
                    busPersonAccount lbusPersonAccount = new busPersonAccount();
                    if (lbusPersonAccount.FindPersonAccount(iintReferenceId))
                    {
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.GENERATE_CANCELLATION_NOTICE, lbusPersonAccount.IsCancellationNoticeGenerated());
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PAYEE_ACCOUNT_EXISTS, lbusPersonAccount.LoadPayeeAccount());
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PAYMENT_BEGIN_DATE_PROCESSED, lbusPersonAccount.IsRequirePayeeAudit());
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SERVICE_RETIREMENT_APPLICATION_EXISTS, lbusPersonAccount.IsApplicationExist());
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_ID, lbusPersonAccount.GetApplicationID());
                    }
                }
                this.BeforeValidate(utlPageMode.All);
            }
            if (utlPassInfo.iobjPassInfo.istrPostBackControlID == "btnSuspendActivity")
            {
                lbusBpmActivityInstance.icdoBpmActivityInstance.suspension_reason_value = icdoBpmActivityInstance.suspension_reason_value;
                lbusBpmActivityInstance.icdoBpmActivityInstance.comments = icdoBpmActivityInstance.comments;
                lbusBpmActivityInstance.istrResumeActionValue = icdoBpmActivityInstance.resume_action_value;
                lbusBpmActivityInstance.icdoBpmActivityInstance.resume_action_value = icdoBpmActivityInstance.resume_action_value;
                lbusBpmActivityInstance.icdoBpmActivityInstance.suspension_end_date = icdoBpmActivityInstance.suspension_end_date;
            }
            
            larrResult = lbusBpmActivityInstance.InvokeWorkflowAction();

            if (utlPassInfo.iobjPassInfo.istrPostBackControlID == "btnReleaseActivity")
            {
                larrResult = lbusBpmActivityInstance.UnassignActivityInstance();
            }
            bool lblnHasErrors = false;
            foreach (object lobjResult in larrResult)
            {
                if (lobjResult is utlError || lobjResult is utlErrorList)
                {
                    lblnHasErrors = true;
                }
            }
            if (!lblnHasErrors)
            {
                larrResult.Clear();
                if (!String.IsNullOrEmpty(icdoBpmActivityInstance.checked_out_user))
                    NotifyUser.RefreshLeftPanel(icdoBpmActivityInstance.checked_out_user);
                if (iobjPassInfo.istrFormName == "wfmBPMWorkflowCenterLeftMaintenance")
                {
                    LoadCenterleftObjects(icdoBpmActivityInstance.activity_instance_id);
                    if (icdoBpmActivityInstance != null && icdoBpmActivityInstance.activity_instance_id > 0)
                    {
                        LoadBpmActivity();
                        LoadBpmProcessInstance();
                        ibusBpmProcessInstance.LoadBpmProcess();
                        ibusBpmProcessInstance.LoadBpmCaseInstance();
                        ibusBpmProcessInstance.ibusBpmCaseInstance.LoadBpmCase();
                        ibusBpmProcessInstance.LoadPerson();
                    }
                }
                else
                {
                    icdoBpmActivityInstance.Select();
                }
                EvaluateInitialLoadRules();
                larrResult.Add(this);
            }
            return larrResult;
        }
        public ArrayList AddNotes()
        {
            ArrayList larrResult = new ArrayList();

            if (iblnNoteModeUpdate)
            {
                istrNewNotes = string.Empty;
                EvaluateInitialLoadRules();
                larrResult.Add(this);
                iblnNoteModeUpdate = false;
                return larrResult;
            }

            if (String.IsNullOrEmpty(istrNewNotes))
            {
                utlError lutlError = AddError(BpmMessages.Message_Id_1533, string.Empty);
                larrResult.Add(lutlError);
                return larrResult;
            }


            doNotes lcdoNotes = new doNotes();
            //lcdoNotes.table_name = BpmCommon.BpmProcessInstanceTable;
            //Set person_id
            lcdoNotes.person_id = ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id;
            //Set org_id
            lcdoNotes.org_id = ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.org_id;
            lcdoNotes.process_instance_id = icdoBpmActivityInstance.process_instance_id;
            lcdoNotes.notes = istrNewNotes;
            lcdoNotes.form_id = busConstant.BPM.BPM_NOTES_CODE_ID;
            lcdoNotes.form_value = NeoConstant.BPM.NotesCategory.BPMProcessNotes;
            lcdoNotes.Insert();
            LoadProcessInstanceNotes();
            istrNewNotes = String.Empty;
            EvaluateInitialLoadRules();
            iintMessageID = BpmMessages.Message_Id_1532;
            larrResult.Add(this);
            return larrResult;
        }
        public bool ValidateCheckoutUserIsSame()
        {
            if (icdoBpmActivityInstance.checked_out_user.EqualsWithNullCheck(iobjPassInfo.istrUserID))
                return true;
            return false;
        }
        #endregion [Public Methods]

        #region [Private Methods]

        /// <summary>
        /// Create the where clause that will be applied to the my basket base query depending upon the parameters selected in the 
        /// search screen.
        /// </summary>
        /// <param name="astrQueryId">Query id.</param>
        /// <param name="astrStatusValue">Status of the workitems.</param>
        /// <returns>Collection of where clause conditions.</returns>
        private Collection<utlWhereClause> BuildWhereClause(string astrQueryId, string astrStatusValue)
        {
            Collection<utlWhereClause> lcolWhereClause = new Collection<utlWhereClause>();
            lcolWhereClause.Add(busNeoBase.GetWhereClause(astrStatusValue, "", "sai.status_value", "string", "in", " ", astrQueryId));

            //This condition is added to prevent the retrieving of activity instances whose process instances are completed bcz of any error.
            //we are excluding such activity instances bcz for such activity instances there may not be any valid information in persistence store,
            //and hence activity instance completion is not possible, it will fail always.
            lcolWhereClause.Add(busNeoBase.GetWhereClause("'INPC'", "", "spi.status_value", "string", "in", "and", astrQueryId));

            lcolWhereClause.Add(busNeoBase.GetWhereClause(utlPassInfo.iobjPassInfo.istrUserID, "", "SAI.CHECKED_OUT_USER", "string", "in", "and", astrQueryId));

            return lcolWhereClause;
        }

        #endregion [Private Methods]
    }
}
