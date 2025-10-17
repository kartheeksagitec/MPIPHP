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
using MPIPHP.Common;
using System.Linq;
using MPIPHP.Interface;
using MPIPHP.DataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busActivityInstance:
    /// Inherited from busActivityInstanceGen, the class is used to customize the business object busActivityInstanceGen.
    /// </summary>
    [Serializable]
    public class busActivityInstance : busActivityInstanceGen
    {
        public string istrViewDetails
        {
            get
            {
                return "View Details";
            }
        }
        public string istrView
        {
            get
            {
                return "View";
            }
        }

        /// <summary>
        /// Used for displaying the Checkout link on my basket screen.
        /// </summary>
        public string istrCheckoutAndLaunch
        {
            get
            {
                return "Checkout";
            }
        }

        /// <summary>
        /// Used for displaying the Launch link on my basket screen.
        /// </summary>
        public string istrLaunch
        {
            get
            {
                return "Launch";
            }
        }

        /// <summary>
        /// Used for displaying the Launch link on my basket screen for commn related activities.
        /// </summary>
        public string istrCommLaunch
        {
            get
            {
                return "Launch";
            }
        }
        

        public Collection<busActivityInstance> iclbUserAssignedActivities { get; set; }
        public Collection<busActivityInstance> iclbRoleAssignedActivities { get; set; }       

        public bool iblnWorkflowEventActionClicked { get; set; }

        public Collection<busProcessInstanceImageData> iclbProcessInstanceImageData { get; set; }
        public Collection<busActivityInstanceHistory> iclbProcessInstanceHistory { get; set; }
        public Collection<busNotes> iclbProcessInstanceNotes { get; set; }
        public Collection<busProcessInstanceChecklist> iclbProcessInstanceChecklist { get; set; }
        public string istrNewNotes { get; set; }


        /// <summary>
        /// Used to get value whether Logged In User has any activities assigned or not
        /// </summary>
        public bool iblnHasAssignedActivities
        {
            get
            {
                if (iclbUserAssignedActivities.IsNotNull() && iclbUserAssignedActivities.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Used to identify the selected record from the activities collection to set different style for it on UI
        /// </summary>
        public string istrIsActivitySelected { get; set; }

      
        public void LoadCenterleftObjects(int aintActivityInstanceID)
        {
            //TO-DO: REFACTOR THE CODE INSIDE INTO TWO METHODS
            LoadActivities();    

            if ((aintActivityInstanceID == 0 && iclbUserAssignedActivities.IsNotNull() && iclbUserAssignedActivities.Count > 0) ||
                 (!(FindActivityInstance(aintActivityInstanceID)) && (iclbUserAssignedActivities.Count > 0)) ||
                 ((iclbUserAssignedActivities.Count > 0) && (!IsActivityInstanceAssignable())))
            {
                aintActivityInstanceID = iclbUserAssignedActivities[0].icdoActivityInstance.activity_instance_id;
            }

            if (FindActivityInstance(aintActivityInstanceID))
            {
                LoadActivity();
                LoadProcessInstance();
                //PIR: 2134 Developer: Vijay Kaza Description: Loading the Process Instance Attachments to show the Images.
                //ibusProcessInstance.LoadProcessInstanceAttachments();
                ibusProcessInstance.LoadProcess();
                ibusProcessInstance.ibusProcess = ibusActivity.ibusProcess;
                //ibusProcessInstance.LoadOrganization();
                ibusProcessInstance.LoadPerson();
                ibusProcessInstance.LoadWorkflowRequest(); 
               
                LoadProcessInstanceNotes();

                ////Assign default values for suspend reason value.
                //if (this.icdoActivityInstance.suspension_reason_value.IsNullOrEmpty())
                //{
                //    this.icdoActivityInstance.suspension_reason_value = WorkflowConstants.SuspensionReasonIncompleteData;
                //}

                if (iclbUserAssignedActivities != null && iclbUserAssignedActivities.Count > 0)
                {
                    foreach (busActivityInstance lobjTemp in iclbUserAssignedActivities)
                    {
                        if (lobjTemp.icdoActivityInstance.activity_instance_id == aintActivityInstanceID)
                        {
                            lobjTemp.istrIsActivitySelected = WorkflowConstants.FlagYes;
                        }
                        else
                        {
                            lobjTemp.istrIsActivitySelected = WorkflowConstants.FlagNo;
                        }
                    }
                }
            }
                     
        }

        /// <summary>
        /// This function is used to get the assigned activities of the logged in user.
        /// </summary>
        public void LoadActivities()
        {
            string lstrQuery;
            Collection<utlWhereClause> lcolWhereClause = null;
            utlMethodInfo lutlMethodInfo;

            //Initialize the Collection to Avoid Null Exception
            iclbUserAssignedActivities = new Collection<busActivityInstance>();

            //Assign the Query Name By the Selected Filter
            lstrQuery = "MyBasketBaseQueryCentreLeft";

            //Load the only activities which are checkedout to the current user
            lcolWhereClause = BuildWhereClause(lstrQuery, WorkflowConstants.ActivityInstanceStatus_INPC_Or_RESU);

            lutlMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoActivityInstance." + lstrQuery);
            lstrQuery = lutlMethodInfo.istrCommand;

            string lstrFinalQuery = lstrQuery;// sqlFunction.AppendWhereClause(lstrQuery, lcolWhereClause, new Collection<IDbDataParameter>(), iobjPassInfo.iconFramework);
            lstrFinalQuery += " order by sai.modified_date desc ";

            DataTable ldtbList = DBFunction.DBSelect(lstrFinalQuery, iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);

            iclbUserAssignedActivities = GetCollection<busActivityInstance>(ldtbList, "icdoActivityInstance");

            //Load the only activities which are in the work pool for the current user
            lstrQuery = "MyBasketBaseQueryCentreLeft";
            lcolWhereClause = BuildWhereClause(lstrQuery, WorkflowConstants.ActivityInstanceStatus_UNPC_Or_RELE);

            lutlMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoActivityInstance." + lstrQuery);
            lstrQuery = lutlMethodInfo.istrCommand;

            lstrFinalQuery = lstrQuery;// sqlFunction.AppendWhereClause(lstrQuery, lcolWhereClause, new Collection<IDbDataParameter>(), iobjPassInfo.iconFramework);
            lstrFinalQuery += " order by activity_instance_id desc ";

            ldtbList = DBFunction.DBSelect(lstrFinalQuery, 
                                                               iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);

            iclbRoleAssignedActivities = GetCollection<busActivityInstance>(ldtbList, "icdoActivityInstance");
            
        }


        private Collection<utlWhereClause> BuildWhereClause(string astrQueryId, string astrStatusValue)
        {
            Collection<utlWhereClause> lcolWhereClause = new Collection<utlWhereClause>();
            busMPIPHPBase lbusMPIPHPBase = new busMPIPHPBase();

            if (astrStatusValue == WorkflowConstants.ActivityInstanceStatus_INPC_Or_RESU)
            {
                lcolWhereClause.Add(lbusMPIPHPBase.GetWhereClause(astrStatusValue, "", "sai.status_value", WorkflowConstants.DATATYPE_STRING, WorkflowConstants.BuildWhereClause_In, " ", astrQueryId));

                //This condition is added to prevent the retrieving of activity instances whose process instances are completed bcz of any error.
                //we are excluding such activity instances bcz for such activity instances there may not be any valid information in persistence store,
                //and hence activity instance completion is not possible, it will fail always.
                lcolWhereClause.Add(lbusMPIPHPBase.GetWhereClause("'INPC'", "", "spi.status_value", WorkflowConstants.DATATYPE_STRING, WorkflowConstants.BuildWhereClause_In, WorkflowConstants.BuildWhereClause_And, astrQueryId));

                lcolWhereClause.Add(lbusMPIPHPBase.GetWhereClause(iobjPassInfo.istrUserID, "", "checked_out_user", WorkflowConstants.DATATYPE_STRING, WorkflowConstants.Operator_EqualTo, WorkflowConstants.BuildWhereClause_And, astrQueryId));
            }

            if (astrStatusValue == WorkflowConstants.ActivityInstanceStatus_UNPC_Or_RELE)
            {
                lcolWhereClause.Add(lbusMPIPHPBase.GetWhereClause(astrStatusValue, "", "sai.status_value", WorkflowConstants.DATATYPE_STRING, WorkflowConstants.BuildWhereClause_In, " ", astrQueryId));

                utlWhereClause lobjWhereClause = lbusMPIPHPBase.GetWhereClause(iobjPassInfo.istrUserID, "", "user_id", "string", "exists", " and ", "UserRole");
                utlMethodInfo lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoActivityInstance.UserRole");
                lobjWhereClause.istrSubSelect = lobjMethodInfo.istrCommand;
                lcolWhereClause.Add(lobjWhereClause);
            }
            

            return lcolWhereClause;
        
        }

        /// <summary>
        /// Checks if whether the activity instance can be assignable to the user. The activities having status as PROC, CANC and SUSP are not assignable 
        /// activities. So left nav should be refreshed to reflect this change.
        /// </summary>
        /// <returns>True if the activity is assignable, false o.w.</returns>
        private bool IsActivityInstanceAssignable()
        {
            bool lblnIsAssignable = true;

            if (this.icdoActivityInstance.status_value.IsNotNull())
            {
                lblnIsAssignable = (!((this.icdoActivityInstance.status_value.Equals(WorkflowConstants.ActivityInstanceStatusProcessed)) ||
                       (this.icdoActivityInstance.status_value.Equals(WorkflowConstants.ActivityInstanceStatusSuspended)) ||
                       (this.icdoActivityInstance.status_value.Equals(WorkflowConstants.ActivityInstanceStatusCancelled))));
            }

            return lblnIsAssignable;
        }

        


        public void LoadProcessInstanceHistory()
        {
            DataTable ldtpProcessInstanceHistory = busMPIPHPBase.Select("cdoActivityInstance.LoadProcessInstanceHistory", new object[1] { icdoActivityInstance.process_instance_id });
            iclbProcessInstanceHistory = GetCollection<busActivityInstanceHistory>(ldtpProcessInstanceHistory, "icdoActivityInstanceHistory");
        }

        public void LoadProcessInstanceChecklist()
        {
            DataTable ldtbChecklist = busMPIPHPBase.Select("cdoActivityInstance.LoadProcessInstanceCheckList", new object[1] { icdoActivityInstance.process_instance_id });
            iclbProcessInstanceChecklist = GetCollection<busProcessInstanceChecklist>(ldtbChecklist, "icdoProcessInstanceChecklist");
        }

        public void LoadProcessInstanceNotes()
        {
            //if (ibusProcessInstance.icdoProcessInstance.person_id != 0)
            //{
            //    DataTable ldtbNotesPerson = busMPIPHPBase.Select("cdoNotes.PersonLookup",
            //                   new object[3] { "WRFL", icdoActivityInstance.process_instance_id, ibusProcessInstance.icdoProcessInstance.person_id });
            //    iclbProcessInstanceNotes = GetCollection<busNotes>(ldtbNotesPerson, "icdoNotes");
            //}
            //else if (ibusProcessInstance.icdoProcessInstance.org_id != 0)
            //{
            //    DataTable ldtbNotesOrg = busMPIPHPBase.Select("cdoNotes.OrgLookup",
            //                                  new object[3] { "WRFL", icdoActivityInstance.process_instance_id, ibusProcessInstance.icdoProcessInstance.org_id });
            //    iclbProcessInstanceNotes = GetCollection<busNotes>(ldtbNotesOrg, "icdoNotes");
            //}

            DataTable ldtbNotes = busMPIPHPBase.Select("cdoNotes.FindProcessInstanceNotes", new object[1] { icdoActivityInstance.process_instance_id });
            iclbProcessInstanceNotes = GetCollection<busNotes>(ldtbNotes, "icdoNotes");
        }

        /// <summary>
        /// Add Notes for the current activity.
        /// </summary>
        /// <returns>Current object if note is added succefully, error object o.w.</returns>
        public ArrayList AddWFNotes()
        {
            ArrayList larrResult = new ArrayList();

            if (istrNewNotes.IsNullOrEmpty())
            {
                utlError lutlError = AddError(WorkflowConstants.Message_Id_4076, string.Empty);
                larrResult.Add(lutlError);
                return larrResult;
            }

            cdoNotes lcdoNotes = new cdoNotes();                        
            lcdoNotes.notes = istrNewNotes;
            lcdoNotes.process_instance_id = this.ibusProcessInstance.icdoProcessInstance.process_instance_id;
            lcdoNotes.form_value = busConstant.WF_MANTAINENCE_FORM;
            lcdoNotes.created_by = iobjPassInfo.istrUserID;
            lcdoNotes.created_date = DateTime.Now;
            lcdoNotes.Insert();
            LoadProcessInstanceNotes();
            istrNewNotes = String.Empty;

            larrResult.Add(this);
            return larrResult;

        }

        public ArrayList InvokeWorkflowAction()
        {
            this.ibusBaseActivityInstance = this;
            ArrayList larrResult = new ArrayList();
            if (string.IsNullOrEmpty(istrInitiator))
            {
                istrInitiator = this.iobjPassInfo.istrPostBackControlID;
            }
            switch (istrInitiator)
            {
                case "btnSuspend":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusSuspended, ibusBaseActivityInstance, iobjPassInfo);
                    break;
                case "btnResume":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusResumed, ibusBaseActivityInstance, iobjPassInfo);
                    break;
                case "btnCancel":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Cancel, busConstant.ActivityStatusCancelled, iobjPassInfo);
                    //This Property setting it 
                    icdoActivityInstance.status_value = busConstant.ActivityStatusCancelled;
                    iblnWorkflowEventActionClicked = true;
                    break;
                case "btnRelease":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusReleased, ibusBaseActivityInstance, iobjPassInfo);
                    break;
                case "btnComplete":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Next, busConstant.ActivityStatusProcessed, iobjPassInfo);
                    if (!larrResult.IsNullOrEmpty() && !(larrResult[0] is utlError))
                    {
                        icdoActivityInstance.status_value = busConstant.ActivityStatusProcessed;
                        iblnWorkflowEventActionClicked = true;
                    }
                    break;
                case "btnReturntoAudit":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.ReturnBack, busConstant.ActivityStatusReturnedToAudit, iobjPassInfo);
                    icdoActivityInstance.status_value = busConstant.ActivityStatusReturnedToAudit;
                    iblnWorkflowEventActionClicked = true;
                    break;
                case "btnReturn":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Return, busConstant.ActivityStatusReturned, iobjPassInfo);
                    icdoActivityInstance.status_value = busConstant.ActivityStatusReturned;

                    iblnWorkflowEventActionClicked = true;
                    break;
            }
            

            if (larrResult.Count == 0) //If No Error
            {
                ReloadObjects();
                larrResult.Add(this);
            }
            return larrResult;
        }

        public void ReloadObjects()
        {
            //Reload the Status 
            icdoActivityInstance.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(20, icdoActivityInstance.status_value);
            //LoadProcessInstanceImageData();
            LoadProcessInstanceHistory();
            LoadProcessInstanceChecklist();
            LoadProcessInstanceNotes();            
            EvaluateInitialLoadRules();
            //Code Added to Refresh the OBJects of Centre Panel as soon as a Activity is operated upon
            LoadCenterleftObjects(this.icdoActivityInstance.activity_instance_id);
        }

        /// <summary>
        /// Load the required parameters for the activity instance object passed as an argument.
        /// </summary>
        /// <param name="adtrRow">Data row.</param>
        /// <param name="aobjActivityInstance">Activity instance.</param>
        private void LoadActivityInstance(DataRow adtrRow, busActivityInstance aobjActivityInstance)
        {
            aobjActivityInstance.ibusActivity = new busActivity { icdoActivity = new cdoActivity() };
            //aobjActivityInstance.ibusQueue = new busQueue { icdoQueue = new cdoQueue() };

            if (!Convert.IsDBNull(adtrRow[enmActivity.name.ToString()]))
            {
                aobjActivityInstance.ibusActivity.icdoActivity.name = adtrRow[enmActivity.name.ToString()].ToString();
            }
            if (!Convert.IsDBNull(adtrRow[enmActivity.display_name.ToString()]))
            {
                aobjActivityInstance.ibusActivity.icdoActivity.display_name = adtrRow[enmActivity.display_name.ToString()].ToString();
            }

            if (!Convert.IsDBNull(adtrRow[enmActivity.process_id.ToString()]))
            {
                aobjActivityInstance.ibusActivity.icdoActivity.process_id = Convert.ToInt32(adtrRow[enmActivity.process_id.ToString()]);
            }

            //if (!Convert.IsDBNull(adtrRow[enmActivity.queue_id.ToString()]))
            //{
            //    aobjActivityInstance.ibusQueue.icdoQueue.queue_id = Convert.ToInt32(adtrRow[enmActivity.queue_id.ToString()]);
            //}

            //if (!Convert.IsDBNull(adtrRow[WorkflowConstants.QueueName]))
            //{
            //    aobjActivityInstance.ibusQueue.icdoQueue.name = adtrRow[WorkflowConstants.QueueName].ToString();
            //}

            aobjActivityInstance.ibusProcessInstance = new busProcessInstance { icdoProcessInstance = new cdoProcessInstance() };
            aobjActivityInstance.ibusProcessInstance.icdoProcessInstance.process_instance_id = aobjActivityInstance.icdoActivityInstance.process_instance_id;
            aobjActivityInstance.ibusProcessInstance.ibusProcess = new busProcess { icdoProcess = new cdoProcess() };
            
            //aobjActivityInstance.ibusProcessInstance.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            
            aobjActivityInstance.ibusProcessInstance.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            aobjActivityInstance.ibusProcessInstance.ibusWorkflowRequest = new busWorkflowRequest { icdoWorkflowRequest = new cdoWorkflowRequest() };

            if (!Convert.IsDBNull(adtrRow[enmProcessInstance.process_instance_id.ToString()]))
            {
                aobjActivityInstance.ibusProcessInstance.icdoProcessInstance.process_instance_id = Convert.ToInt32(adtrRow[enmProcessInstance.process_instance_id.ToString()]);
            }

            if (!Convert.IsDBNull(adtrRow[enmProcessInstance.workflow_instance_guid.ToString()]))
            {
                aobjActivityInstance.ibusProcessInstance.icdoProcessInstance.workflow_instance_guid = new Guid(adtrRow[enmProcessInstance.workflow_instance_guid.ToString()].ToString());
            }

            if (!Convert.IsDBNull(adtrRow[enmPerson.person_id.ToString()]))
            {
                aobjActivityInstance.ibusProcessInstance.icdoProcessInstance.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                aobjActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                aobjActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.first_name = adtrRow[enmPerson.first_name.ToString()].ToString();
                aobjActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.last_name = adtrRow[enmPerson.last_name.ToString()].ToString();
                aobjActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.middle_name = adtrRow[enmPerson.middle_name.ToString()].ToString();
                aobjActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.ssn = Convert.ToString(adtrRow[enmPerson.ssn.ToString()]);

            }

            //if (!Convert.IsDBNull(adtrRow[enmOrganization.org_id.ToString()]))
            //{
            //    aobjActivityInstance.ibusProcessInstance.icdoProcessInstance.org_id = Convert.ToInt32(adtrRow[enmOrganization.org_id.ToString()]);
            //    aobjActivityInstance.ibusProcessInstance.ibusOrganization.icdoOrganization.org_id = Convert.ToInt32(adtrRow[enmOrganization.org_id.ToString()]);
            //    aobjActivityInstance.ibusProcessInstance.ibusOrganization.icdoOrganization.org_name = adtrRow[enmOrganization.org_name.ToString()].ToString();
            //}

            if (!Convert.IsDBNull(adtrRow[WorkflowConstants.ProcessName]))
            {
                aobjActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.name = adtrRow[WorkflowConstants.ProcessName].ToString();
            }

            if (!Convert.IsDBNull(adtrRow[WorkflowConstants.ProcessDescription]))
            {
                aobjActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.description = adtrRow[WorkflowConstants.ProcessDescription].ToString();
            }

            if (!Convert.IsDBNull(adtrRow[WorkflowConstants.SourceDescription]))
            {
                aobjActivityInstance.ibusProcessInstance.ibusWorkflowRequest.icdoWorkflowRequest.source_description = adtrRow[WorkflowConstants.SourceDescription].ToString();
            }
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busActivityInstance)
            {
                busActivityInstance lbusActivityInstance = (busActivityInstance)aobjBus;
                LoadActivityInstance(adtrRow, lbusActivityInstance);

                lbusActivityInstance.ibusProcessInstance = new busProcessInstance() { icdoProcessInstance = new cdoProcessInstance() };
                lbusActivityInstance.ibusProcessInstance.icdoProcessInstance.LoadData(adtrRow);

                if (lbusActivityInstance.ibusProcessInstance != null)
                {
                    lbusActivityInstance.ibusProcessInstance.ibusProcess = new busProcess() { icdoProcess = new cdoProcess() };
                    lbusActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.LoadData(adtrRow);
                    if (!Convert.IsDBNull(adtrRow[WorkflowConstants.ProcessName]))
                    {
                        lbusActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.name = adtrRow[WorkflowConstants.ProcessName] as string;
                    }

                    lbusActivityInstance.ibusProcessInstance.ibusWorkflowRequest = new busWorkflowRequest { icdoWorkflowRequest = new cdoWorkflowRequest() };
                    lbusActivityInstance.ibusProcessInstance.ibusWorkflowRequest.icdoWorkflowRequest.LoadData(adtrRow);

                    lbusActivityInstance.ibusProcessInstance.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };
                    lbusActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.LoadData(adtrRow);

                    //lbusActivityInstance.ibusProcessInstance.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                    //lbusActivityInstance.ibusProcessInstance.ibusOrganization.icdoOrganization.LoadData(adtrRow);

                    if (!Convert.IsDBNull(adtrRow[WorkflowConstants.ProcessDescription]))
                    {
                        lbusActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.description = adtrRow[WorkflowConstants.ProcessDescription].ToString();
                    }
                }
            }
            else if (aobjBus is busActivityInstanceHistory)
            {
                busActivityInstanceHistory lbusActivityInstanceHistory = (busActivityInstanceHistory)aobjBus;
                lbusActivityInstanceHistory.ibusSolutionActivityInstance = new busActivityInstance() { icdoActivityInstance = new cdoActivityInstance() };
                lbusActivityInstanceHistory.ibusSolutionActivityInstance.ibusActivity = new busActivity { icdoActivity = new cdoActivity() };
                if (!Convert.IsDBNull(adtrRow["STATUS_DESCRIPTION"]))
                {
                    lbusActivityInstanceHistory.icdoActivityInstanceHistory.status_description = adtrRow["STATUS_DESCRIPTION"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["ACTIVITY_NAME"]))
                {
                    lbusActivityInstanceHistory.ibusSolutionActivityInstance.ibusActivity.icdoActivity.display_name = adtrRow["ACTIVITY_NAME"].ToString();
                }
            }

            if (aobjBus is busProcessInstanceChecklist)
            {
                busProcessInstanceChecklist lbusProcessInstanceCheckList = (busProcessInstanceChecklist)aobjBus;
                lbusProcessInstanceCheckList.ibusDocument = new busDocument { icdoDocument = new cdoDocument() };
                lbusProcessInstanceCheckList.ibusDocument.icdoDocument.LoadData(adtrRow);
            }
        }

        public ArrayList btnApplyCheckList_Click()
        {
            ArrayList larrList = new ArrayList();
            if (iclbProcessInstanceChecklist != null)
            {
                foreach (busProcessInstanceChecklist lbusProcessInstance in iclbProcessInstanceChecklist)
                {
                    lbusProcessInstance.icdoProcessInstanceChecklist.Update();
                }
            }
            LoadProcessInstanceChecklist();
            larrList.Add(this);
            return larrList;
        }
    }
}
