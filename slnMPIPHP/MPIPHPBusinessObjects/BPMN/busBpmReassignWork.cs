#region [Using Directives]
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using NeoBase.Common;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DataObjects;
using Sagitec.DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
#endregion [Using Directives]

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBpmReassignWork : busNeoSpinBase
    {
        #region [Properties]

        /// <summary>
        /// Get Activity Instance Priority.
        /// </summary>
        public string istrPriority { get; set; }
        /// <summary>
        /// Get Activity Instance all Status.
        /// </summary>
        public string istrActivityInstanceStatus { get; set; }

        /// <summary>
        /// activity instance Due date entered in search criteria.
        /// </summary>
        public DateTime idtActivityDueDateFrom { get; set; }
        /// <summary>
        /// activity instance Due date entered in search criteria.
        /// </summary>
        public DateTime idtActivityDueDateTo { get; set; }
        /// <summary>
        /// Stores the value of Process Id entered on search screen.
        /// </summary>
        public int iintProcessID { get; set; }

        /// <summary>
        /// Stores the value of the Activity Id entered on search screen depending on the process id.
        /// </summary>
        public int iintActivityID { get; set; }
        /// <summary>
        /// Stores the list of values of Activity Id entered on search screen depending on the process id.
        /// </summary>
        public string istrActivityID { get; set; }

        /// <summary>
        /// Stores the value of the Person Id entered on search screen
        /// </summary>
        // public int iintPersonID { get; set; }   ///Already decleared this property in base class (busNeoSpinBase) Hence commented.  vishal kedar

        /// <summary> 
        /// Stores the value of the Org ID entered on search screen.
        /// </summary>
        public int iintOrgnID { get; set; }

        ///// <summary>
        ///// Stores the value of queue id entered on search screen. 
        ///// </summary>
        //public int iintQueueID { get; set; }

        /// <summary>
        /// Stores the value of role id entered on search screen.
        /// </summary>
        public int iintRoleID { get; set; }

        /// <summary>
        /// Stores the users info. who has worked on the activity selected from the activity id on search screen.
        /// </summary>
        public string istrCheckedOutUser { get; set; }

        /// <summary>
        /// Used for storing the workflow activity instance created date from info entered in search criteria.
        /// </summary>
        public DateTime idtCreatedDateFrom { get; set; }

        /// <summary>
        /// Used for storing the workflow activity instance created date to info entered in search criteria.
        /// </summary>
        public DateTime idtCreatedDateTo { get; set; }

        /// <summary>
        /// Collection of workitems to be re-assigned to.
        /// </summary>
        public Collection<busSolBpmActivityInstance> iclbReassignmentActivities { get; set; }

        /// <summary>
        /// Stores the value of selected re-assign to user id.
        /// </summary>
        public string istrReassignUser { get; set; }

        /// <summary>
        /// Stores the value of selected to be re-assign to queue id.
        /// </summary>
        public int iintReassignQueueId { get; set; }

        /// <summary>
        /// Stores the MaxSearchCount value in web.config.
        /// </summary>
        public int iintMaxSearchCount { get; set; }

        /// <summary>
        /// Store the count of records fetched for each search criteria.
        /// </summary>
        public int iintActualRecordCount { get; set; }

        /// <summary>
        /// To store if there exists any search criteria.
        /// </summary>
        public bool iblnSearchCriteriaEntered { get; set; }

        /// <summary>
        /// Property used to store if the selected workitems are successfully re-assigned.
        /// </summary>
        public bool iblnAllWorkItemsReassignedSuccessfully { get; set; }

        /// <summary>
        /// Used to store the logged in user serial id.
        /// </summary>
        public int iintUserSerialId = 0;

        private ArrayList iarrStoredErrors;

        #endregion [Properties]

        #region [Public Methods]

        /// <summary>
        /// Search button handler        
        /// </summary>
        /// <returns>Current object.</returns>
        public ArrayList SearchReassignWorkItems()
        {
            ArrayList larrResult = new ArrayList();


            this.iintActualRecordCount = 0;
            iblnSearchCriteriaEntered = false;

            //Check if there exists some search criteria.
            //Developer - Vishakha Sancheti
            //Iteration - Iteration13
            //Date - 23 JAN 2023
            //Comment - Fixed PIR-4263-Made Activity Name a multiselect dropdown on screen, so istrActivityID will store the comma separated string with selected values.
            if ((iintProcessID == 0) && (string.IsNullOrEmpty(istrActivityID)) && (string.IsNullOrEmpty(istrMPIPersonID)) && (iintOrgnID == 0) && (string.IsNullOrEmpty(istrActivityInstanceStatus))
                && (string.IsNullOrEmpty(istrCheckedOutUser)) && (idtCreatedDateFrom.Equals(DateTime.MinValue)) && (idtCreatedDateTo.Equals(DateTime.MinValue))
                && (iintRoleID == 0) && (string.IsNullOrEmpty(istrPriority)) && (idtActivityDueDateFrom.Equals(DateTime.MinValue))
                && (idtActivityDueDateTo.Equals(DateTime.MinValue)))
            {
                iintMessageID = WorkflowConstants.MESSAGE_ID_5;
                this.istrMessage = busMainBase.GetMessage(utlMessageType.Framework, WorkflowConstants.MESSAGE_ID_5);
                larrResult.Add(this);
            }

            iblnSearchCriteriaEntered = true;
            string MaxSearchCount = busNeoBaseGlobalFunctions.GetData1ByCodeValue(20007002, "MAX");
            iintMaxSearchCount = Convert.ToInt32(MaxSearchCount);
            larrResult = SearchAndLoadReassignmentBasket();

            if (this.iblnSearchCriteriaEntered)
            {
                if (this.iintActualRecordCount == 0)
                {
                    this.iintMessageID = WorkflowConstants.MESSAGE_ID_2;
                    this.istrMessage = busMainBase.GetMessage(utlMessageType.Framework, WorkflowConstants.MESSAGE_ID_2);

                }
                else if (this.iintActualRecordCount > this.iintMaxSearchCount)
                {
                    this.iintMessageID = WorkflowConstants.MESSAGE_ID_3;
                    this.istrMessage = busMainBase.GetMessage(utlMessageType.Framework, WorkflowConstants.MESSAGE_ID_3, new object[2] { this.iintActualRecordCount, this.iintMaxSearchCount });

                }
                else
                {
                    this.iintMessageID = WorkflowConstants.MESSAGE_ID_1;
                    this.istrMessage = busMainBase.GetMessage(utlMessageType.Framework, WorkflowConstants.MESSAGE_ID_1, new object[1] { this.iintActualRecordCount });

                }
            }
            else
            {
                this.iintMessageID = WorkflowConstants.MESSAGE_ID_5;
                this.istrMessage = busMainBase.GetMessage(utlMessageType.Framework, WorkflowConstants.MESSAGE_ID_5);

            }

            larrResult.Add(this);
            return larrResult;
        }

        /// <summary>
        /// Used for reassigning the activity instance to different user.
        /// </summary>
        /// <param name="aarrSelectedObjects">Collection of the work items to reassign to user or queue.</param>
        /// <returns>Arraylist which includes current object.</returns>
        public virtual ArrayList WorkReassignment(ArrayList aarrSelectedObjects)
        {
            bool lblnAllWorkItemsAssigned = false;
            ArrayList larrList = new ArrayList();
            utlError lutlError = new utlError();
            iarrStoredErrors = new ArrayList();
            ArrayList larrRestricted = new ArrayList();
            // Vijay Kaza: Commenting the below 2 if conditions as we do not have Queue Id on the screen.
            //Either user id or queue id is required.
            //if ((istrReassignUser.IsNullOrEmpty()) && (iintReassignQueueId == 0))
            //{
            //    lutlError = AddError(WorkflowConstants.MESSAGE_ID_1537, String.Empty);

            //    iarrStoredErrors.Add(lutlError);
            //    larrList.Clear();
            //    //--
            //    larrList.Add(this);
            //    return larrList;
            //}

            //if ((istrReassignUser.IsNotNullOrEmpty()) && (iintReassignQueueId != 0))
            //{
            //    lutlError = AddError(WorkflowConstants.MESSAGE_ID_1520, String.Empty);
            //    iarrStoredErrors.Add(lutlError);
            //    larrList.Clear();
            //    //--
            //    larrList.Add(this);
            //    return larrList;
            //}

            //Check if the user id is valid.
            busUser lbusBpmUser = new busUser() { icdoUser = new cdoUser() };
            if (String.IsNullOrEmpty(istrReassignUser) || (!String.IsNullOrEmpty(istrReassignUser) && (!lbusBpmUser.FindUser(istrReassignUser))))
            {
                lutlError = AddError(WorkflowConstants.MESSAGE_ID_1538, String.Empty);
                iarrStoredErrors.Add(lutlError);
                larrList.Clear();
                //--
                larrList.Add(this);
                return larrList;
            }

            //Check if the user is available/Unavailable.
            if (CheckReassignUserAvailable())
            {
                lutlError = AddError(WorkflowConstants.MESSAGE_ID_1582, String.Empty);
                iarrStoredErrors.Add(lutlError);
                larrList.Clear();
                //--
                larrList.Add(this);
                return larrList;
            }

            if (aarrSelectedObjects != null && aarrSelectedObjects.Count > 0 && !String.IsNullOrEmpty(istrReassignUser))
            {
                //Check if selected activity for re-assignment is ESS if so then display message to not allow re-assignment.                
                bool lblnExternalActivitySelected = IsExternalActivitySelectedForReAssignment(aarrSelectedObjects);

                if (lblnExternalActivitySelected)
                {
                    lutlError = AddError(WorkflowConstants.MESSAGE_ID_1521, String.Empty);
                    iarrStoredErrors.Add(lutlError);
                    larrList.Clear();
                    //--
                    larrList.Add(this);
                    return larrList;
                }
                bool lblerrorParse = false;
                foreach (busSolBpmActivityInstance lbusSolBpmActivityInstance in aarrSelectedObjects)
                {
                    lblnAllWorkItemsAssigned = ReassignToUser(lbusSolBpmActivityInstance);
                    if (!lblnAllWorkItemsAssigned)
                    {
                        if (!lblerrorParse)
                        {
                            lblerrorParse = true;
                            //Some re-assignments not completed - incomplete rights.                    
                            lutlError = AddError(WorkflowConstants.MESSAGE_ID_1523, String.Empty);
                            iarrStoredErrors.Add(lutlError);
                            larrList.Clear();
                        }
                        larrRestricted.Add(lbusSolBpmActivityInstance);
                        larrList.Add(this);
                    }
                    else
                    {
                        iblnAllWorkItemsReassignedSuccessfully = true;
                    }
                }
            }
            //All workitems successfully reassigned.            
            SearchAndLoadReassignmentBasket();
            foreach (busSolBpmActivityInstance lbus in larrRestricted)
            {
                iclbReassignmentActivities.Where(x => x.icdoBpmActivityInstance.activity_instance_id == lbus.icdoBpmActivityInstance.activity_instance_id).ForEach(p => p.istrReassignErrorCSS = "ROLE_ERROR");
            }
            if (this.iblnAllWorkItemsReassignedSuccessfully)
            {
                if (!String.IsNullOrEmpty(this.istrReassignUser))
                {
                    this.iintMessageID = WorkflowConstants.MESSAGE_ID_1522;
                    this.istrMessage = GetMessage(WorkflowConstants.MESSAGE_ID_1522, new object[1] { this.istrReassignUser });
                }

                this.istrReassignUser = string.Empty;
                this.iintReassignQueueId = 0;
            }

            larrList.Add(this);
            return larrList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ArrayList CheckError()
        {
            if (iarrStoredErrors?.Count > 0)
            {
                return iarrStoredErrors;
            }
            return new ArrayList { this };
        }

        /// <summary>
        /// This function is used for loading activities on which the logged in user is working on and which
        /// can be reassigned to other user.
        /// </summary>
        /// <returns>Current object.</returns>
        public ArrayList SearchAndLoadReassignmentBasket(string istrsupervisour="")
        {
            ArrayList larrResult = new ArrayList();
            Collection<IDbDataParameter> lcolParams = new Collection<IDbDataParameter>();
            string lstrFinalQuery;
            string lstrQuery;
            Collection<utlWhereClause> lcolWhereClause;
            utlMethodInfo lutlMethodInfo;
            string MaxSearchCount = busNeoBaseGlobalFunctions.GetData1ByCodeValue(20007002, "MAX");
            iintMaxSearchCount = Convert.ToInt32(MaxSearchCount);
            iintUserSerialId = iobjPassInfo.iintUserSerialID;
            if (!String.IsNullOrEmpty(istrsupervisour))
            {
                istrCheckedOutUser = istrsupervisour;
            }
            
            lstrQuery = "SearchAndLoadReassignWork";
            lcolWhereClause = BuildWhereClause(lstrQuery, WorkflowConstants.ACTIVITYINSTANCE_STATUS_TO_REASSIGN);
            lutlMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("entBpmActivityInstance." + lstrQuery);
            lstrQuery = lutlMethodInfo.istrCommand;
            lstrFinalQuery = sqlFunction.AppendWhereClause(lstrQuery, lcolWhereClause, lcolParams, iobjPassInfo.iconFramework);
            lstrFinalQuery += " order by SAI.MODIFIED_DATE desc";
            if (DBFunction.IsPostgreSQLConnection())
            {
                lstrFinalQuery += " LIMIT " + iintMaxSearchCount;
            }
            if (DBFunction.IsOracleConnection())
            {
                lstrFinalQuery += " FETCH FIRST " + iintMaxSearchCount + " ROWS ONLY";
            }
            if (DBFunction.IsSQLConnection())
            {
                IDbDataParameter iparameter = DBFunction.GetDBParameter();
                iparameter.ParameterName = "@iintMaxSearchCount";
                iparameter.Value = iintMaxSearchCount;
                iparameter.DbType = DbType.Int32;
                lcolParams.Add(iparameter);
            }
            DataTable ldtbList = DBFunction.DBSelect(lstrFinalQuery, lcolParams,
                                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbReassignmentActivities = new Collection<busSolBpmActivityInstance>();
            iclbReassignmentActivities = GetCollection<busSolBpmActivityInstance>(ldtbList, "icdoBpmActivityInstance");
            foreach (busSolBpmActivityInstance lbus in iclbReassignmentActivities)
            {
                lbus.LoadBpmProcessInstance();
                lbus.ibusBpmProcessInstance.LoadBpmCaseInstance();
            }
            if (ldtbList.IsNotNullOrEmpty())
            {
                iintActualRecordCount = ldtbList.Rows[0].CheckAndGetValue<int>("TotalRecords");
            }
            larrResult.Add(this);
            return larrResult;
        }

        /// <summary>
        /// This function returns the collection of the processes accessible to the logged in user.
        /// </summary>
        public Collection<doBpmProcess> GetAccessibleUserProcesses()
        {
            utlPassInfo.iobjPassInfo.iintUserSerialID = iintUserSerialId;
            DataTable ldtbProcessesForSelectedUser = busBase.Select("entBpmProcess.GetProcessesForOnlineInitialization",
                                                          //new object[1] { 88 });
                                                          new object[] { });

            Collection<doBpmProcess> lclcProcessesForSelectedUser = doBase.GetCollection<doBpmProcess>(ldtbProcessesForSelectedUser);
            return lclcProcessesForSelectedUser;
        }
        /// <summary>
        /// Framework version 6.0.0.25 changes
        /// </summary>
        public ArrayList ReassignToSupervisour(ArrayList aarrSelectedObjects)
        {
            ArrayList larrList = new ArrayList();
            utlError lutlError = new utlError();
            iarrStoredErrors = new ArrayList();

            //Check if the user id is valid.
            busUser lbusBpmUser = new busUser() { icdoUser = new cdoUser() };
            if (String.IsNullOrEmpty(istrReassignUser) || (!String.IsNullOrEmpty(istrReassignUser) && (!lbusBpmUser.FindUser(istrReassignUser))))
            {
                lutlError = AddError(WorkflowConstants.MESSAGE_ID_1538, String.Empty);
                iarrStoredErrors.Add(lutlError);
                larrList.Clear();
                //--
                larrList.Add(this);
                return larrList;
            }

            //Developer - Ratnesh Kumar Singh
            //Iteration - Iteration12
            // Date   -  19/05/2022
            //Comment -Fixed PIR-4126-System is Not Displaying Correct User name in information Message when Reassign To Supervisor 
            else
            {
                this.iintMessageID = WorkflowConstants.MESSAGE_ID_1522;
                this.istrMessage = GetMessage(WorkflowConstants.MESSAGE_ID_1522, new object[1] { this.istrReassignUser });
            }

            foreach (busBpmActivityInstance lbusBpmActivityInstance in aarrSelectedObjects)
            {
                busBpmActivityInstance lbusSolBpmActivityInstance =
                lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.LoadWithActivityInst(lbusBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id);
                larrList = (lbusSolBpmActivityInstance.ibusBpmActivity as busBpmUserTask).ReassignToSupervisour(lbusSolBpmActivityInstance, istrReassignUser);
                if (larrList.Count > 0)
                {
                    break;
                }
            }
            if (larrList.Count == 0)
            {
                this.SearchAndLoadReassignmentBasket();
                larrList.Add(this);
            }
            return larrList;
        }

        #endregion [Public Methods]

        #region [Overriden Methods]

        /// <summary>
        ///  Overriden to load required parameters associated with the activity instance.
        /// </summary>
        /// <param name="adtrRow">Data Row.</param>
        /// <param name="abusBpmbusBpmBase">busBpminess Object.</param>
        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBpmbusBpmBase)
        {
            if (abusBpmbusBpmBase is busBpmActivityInstance)
            {
                busSolBpmActivityInstance lbusSolBpmActivityInstance = (busSolBpmActivityInstance)abusBpmbusBpmBase;
                //busSolBpmActivityInstance lbusSolBpmActivityInstance = new busSolBpmActivityInstance();
                busBpmActivityInstance lbusBpmActivityInstance = (busBpmActivityInstance)abusBpmbusBpmBase;

                lbusBpmActivityInstance.ibusBpmActivity = busBpmActivity.GetBpmActivityByActivityType(BpmActivityTypes.BaseActivity);

                if (!Convert.IsDBNull(adtrRow[enmBpmActivity.name.ToString()]))
                {
                    lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name = adtrRow[enmBpmActivity.name.ToString()].ToString();
                }
                if (!Convert.IsDBNull(adtrRow[enmBpmActivity.name.ToString()]))
                {
                    lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name = adtrRow[enmBpmActivity.name.ToString()].ToString();
                }

                if (!Convert.IsDBNull(adtrRow[enmBpmActivity.process_id.ToString()]))
                {
                    lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.process_id = Convert.ToInt32(adtrRow[enmBpmActivity.process_id.ToString()]);
                }


                lbusBpmActivityInstance.ibusBpmProcessInstance = new busBpmProcessInstance { icdoBpmProcessInstance = new doBpmProcessInstance() };
                lbusBpmActivityInstance.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id = lbusBpmActivityInstance.icdoBpmActivityInstance.process_instance_id;
                lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess = new busBpmProcess { icdoBpmProcess = new doBpmProcess() };

                lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance = new busSolBpmCaseInstance();

                if (!Convert.IsDBNull(adtrRow[enmBpmProcessInstance.process_instance_id.ToString()]))
                {
                    lbusBpmActivityInstance.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id = Convert.ToInt32(adtrRow[enmBpmProcessInstance.process_instance_id.ToString()]);
                }


                if (!Convert.IsDBNull(adtrRow["PROCESS_NAME"]))
                {
                    lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name = adtrRow[WorkflowConstants.PROCESS_NAME].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["PROCESS_DESCRIPTION"]))
                {
                    lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.description = adtrRow[WorkflowConstants.PROCESS_DESCRIPTION].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["PERSON_ID"]))
                {
                    lbusSolBpmActivityInstance.iintPerson_Id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                }
                if (!Convert.IsDBNull(adtrRow["MPI_PERSON_ID"]))
                {
                    lbusSolBpmActivityInstance.istrMPI_Person_ID = adtrRow[enmPerson.mpi_person_id.ToString()].ToString();
                }
                if (!Convert.IsDBNull(adtrRow["ORG_ID"]))
                {
                    lbusSolBpmActivityInstance.iintOrg_Id = Convert.ToInt32(adtrRow[enmOrganization.org_id.ToString()]);
                }

                if (!Convert.IsDBNull(adtrRow["SOURCE_VALUE"]))
                {
                    if (adtrRow["SOURCE_VALUE"].ToString() == busNeoBaseConstants.BPM.SOURCE_ONLINE_VALUE)
                        lbusSolBpmActivityInstance.istrSource = busNeoBaseConstants.BPM.SOURCE_ONLINE_DESCREPTION;
                    if (adtrRow["SOURCE_VALUE"].ToString() == busNeoBaseConstants.BPM.SOURCE_BATCH_VALUE)
                        lbusSolBpmActivityInstance.istrSource = busNeoBaseConstants.BPM.SOURCE_BATCH_DESCREPTION;
                    if (adtrRow["SOURCE_VALUE"].ToString() == busNeoBaseConstants.BPM.SOURCE_MESSAGE_FLOW_VALUE)
                        lbusSolBpmActivityInstance.istrSource = busNeoBaseConstants.BPM.SOURCE_MESSAGE_FLOW_DESCREPTION;
                    if (adtrRow["SOURCE_VALUE"].ToString() == busNeoBaseConstants.BPM.SOURCE_INDEXING_VALUE)
                        lbusSolBpmActivityInstance.istrSource = busNeoBaseConstants.BPM.SOURCE_INDEXING_DESCREPTION;
                }

                if (!Convert.IsDBNull(adtrRow["CHECKED_OUT_DATE"]))
                {
                    lbusSolBpmActivityInstance.idtCheckedOutDate = Convert.ToDateTime(adtrRow[enmBpmActivityInstance.checked_out_date.ToString()]);
                }

                if (!Convert.IsDBNull(adtrRow["PRIORITY_CODE_VALUE"]))
                {
                    if (adtrRow["PRIORITY_CODE_VALUE"].ToString() == busNeoBaseConstants.BPM.PRIORITY_NORMAL_VALUE)
                        lbusSolBpmActivityInstance.istrPriority = busNeoBaseConstants.BPM.PRIORITY_NORMAL_DESCREPTION;

                    if (adtrRow["PRIORITY_CODE_VALUE"].ToString() == busNeoBaseConstants.BPM.PRIORITY_HIGH_VALUE)
                        lbusSolBpmActivityInstance.istrPriority = busNeoBaseConstants.BPM.PRIORITY_HIGH_DESCREPTION;
                }

                if (!Convert.IsDBNull(adtrRow[busNeoBaseConstants.BPM.PROCESS_INSTANCE_CREATED_DATE]))
                {
                    lbusSolBpmActivityInstance.idtInitiatedDate = Convert.ToDateTime(adtrRow[busNeoBaseConstants.BPM.PROCESS_INSTANCE_CREATED_DATE]);
                }

                if (!Convert.IsDBNull(adtrRow[busNeoBaseConstants.BPM.ACTIVITY_INSTANCE_START_DATE]))
                {
                    lbusSolBpmActivityInstance.idtStartDate = Convert.ToDateTime(adtrRow[busNeoBaseConstants.BPM.ACTIVITY_INSTANCE_START_DATE]);
                }

                if (!Convert.IsDBNull(adtrRow[busNeoBaseConstants.BPM.ACTIVITY_INSTANCE_END_DATE]))
                {
                    lbusSolBpmActivityInstance.idtEndDate = Convert.ToDateTime(adtrRow[busNeoBaseConstants.BPM.ACTIVITY_INSTANCE_END_DATE]);
                }
                if (!Convert.IsDBNull(adtrRow[busNeoBaseConstants.BPM.BPM_ACTIVITY_INSTANCE_PERSON_NAME]))
                {
                    lbusSolBpmActivityInstance.istrPersonName = adtrRow[busNeoBaseConstants.BPM.BPM_ACTIVITY_INSTANCE_PERSON_NAME].ToString();
                }
                if (!Convert.IsDBNull(adtrRow[busNeoBaseConstants.BPM.BPM_ACTIVITY_INSTANCE_ORG_NAME]))
                {
                    lbusSolBpmActivityInstance.istrOrgName = adtrRow[busNeoBaseConstants.BPM.BPM_ACTIVITY_INSTANCE_ORG_NAME].ToString();
                }
            }

            base.LoadOtherObjects(adtrRow, abusBpmbusBpmBase);
        }

        #endregion [Overriden Methods]

        #region [Private Methods]

        /// <summary>
        /// This function is used to check if the external activity is selected for re-assignment.
        /// </summary>
        /// <param name="aarrSelectedObjects">Collection of selected workitems.</param>
        /// <returns>True if external activity is selcted, false o.w.</returns>
        private bool IsExternalActivitySelectedForReAssignment(ArrayList aarrSelectedObjects)
        {
            //foreach (Object lobjSelectedWork in aarrSelectedObjects)
            //{
            //    // Type cast selected object.
            //    busBpmActivityInstance lbusBpmReassignWork = lobjSelectedWork as busBpmActivityInstance;

            //    if (lbusBpmReassignWork.IsNotNull())
            //    {
            //        lbusBpmReassignWork.LoadBpmActivity();
            //        return (lbusBpmReassignWork.ibusBpmActivity.icdoBpmActivity.external_activity_ind.IsNotNull() &&
            //                lbusBpmReassignWork.ibusBpmActivity.icdoBpmActivity.external_activity_ind.Equals(WorkflowConstants.FlagYes));
            //    }
            //}

            return false;
        }

        /// <summary>
        ///  This function is used for re-assigning workitem to different user.
        /// </summary>
        /// <param name="aarrSelectedObjects">Collection of selected workitems.</param>
        /// <returns>True if all workitems are reassigned successfully, false o.w.</returns>
        private bool ReassignToUser(object aobjSelectedWork)
        {
            busUser lbusUser = new busUser() { icdoUser = new cdoUser() };
            bool lblnReassignToAllSuccess = true;
            string lstrCheckedOutUser = string.Empty;
            if (!lbusUser.FindUser(istrReassignUser))
                return false;
            // Type cast selected object.
            busBpmActivityInstance lbusBpmReassignWork = aobjSelectedWork as busBpmActivityInstance;
            lbusBpmReassignWork = lbusBpmReassignWork.ibusBpmProcessInstance.ibusBpmCaseInstance.LoadWithActivityInst(lbusBpmReassignWork.icdoBpmActivityInstance.activity_instance_id);
            if (lbusBpmReassignWork != null)
            {
                //Check if user is assigned to role same as activity role. 
                //if so then only do the re-assignment.
                List<string> llstEligibleUsersToReassign = ((busBpmUserTask)lbusBpmReassignWork.ibusBpmActivity).GetEligibleUsersForReAssignment(lbusBpmReassignWork);
                bool lblnEligibleUser = llstEligibleUsersToReassign.Where(userId => userId == istrReassignUser).FirstOrDefault() != null;
                if (lblnEligibleUser)
                {
                    lstrCheckedOutUser = lbusBpmReassignWork.icdoBpmActivityInstance.checked_out_user;
                    if (lbusBpmReassignWork.icdoBpmActivityInstance.status_value == BpmActivityInstanceStatus.NotStarted)
                    {
                        lbusBpmReassignWork.CheckoutActivity(istrReassignUser, false);
                    }
                    else
                    {
                        lbusBpmReassignWork.icdoBpmActivityInstance.checked_out_user = istrReassignUser;
                    }
                    lbusBpmReassignWork.icdoBpmActivityInstance.iblnNeedHistory = true;
                    lbusBpmReassignWork.icdoBpmActivityInstance.Update();
                    if (!String.IsNullOrEmpty(istrReassignUser))
                        NotifyUser.RefreshLeftPanel(istrReassignUser);
                    if (!String.IsNullOrEmpty(lstrCheckedOutUser))
                        NotifyUser.RefreshLeftPanel(lstrCheckedOutUser);
                }
                else
                {
                    lblnReassignToAllSuccess = false;
                }
            }

            return lblnReassignToAllSuccess;
        }

        /// <summary>
        /// Create the where clause that will be applied to the reassignment base query depending upon the parameters selected in the search screen.
        /// </summary>
        /// <param name="astrQueryId">Query Id for selected filters.</param>
        /// <param name="astrStatusValue">Status of the work items.</param>
        /// <returns>Collection of where clause conditions depending upon the filters selected.</returns>
        private Collection<utlWhereClause> BuildWhereClause(string astrQueryId, string astrStatusValue)
        {
            Collection<utlWhereClause> lcolWhereClause = new Collection<utlWhereClause>();

            //lcolWhereClause.Add(GetWhereClause(astrStatusValue, "", "sai.status_value", WorkflowConstants.DATATYPE_STRING, WorkflowConstants.BUILD_WHERE_CLAUSE_IN, " ", astrQueryId));
            if (!String.IsNullOrEmpty(istrActivityInstanceStatus))
                lcolWhereClause.Add(busNeoBase.GetWhereClause(istrActivityInstanceStatus, "", "sai.status_value", WorkflowConstants.DATATYPE_STRING, WorkflowConstants.BUILD_WHERE_CLAUSE_IN, " ", astrQueryId));
            else
                lcolWhereClause.Add(busNeoBase.GetWhereClause(astrStatusValue, "", "sai.status_value", WorkflowConstants.DATATYPE_STRING, WorkflowConstants.BUILD_WHERE_CLAUSE_IN, " ", astrQueryId));
            if (iintProcessID > 0)
                lcolWhereClause.Add(busNeoBase.GetWhereClause(iintProcessID, "", "spi.process_id", WorkflowConstants.DATATYPE_INT, WorkflowConstants.OPERATOR_EQUALTO, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));
            //We made Activity Name a multiselect dropdown,this property "istrActivityID" will have comma saperated values selected from search criteria, hence added here in buildwhereclause.
            if (!String.IsNullOrEmpty(istrActivityID))
                lcolWhereClause.Add(busNeoBase.GetWhereClause(istrActivityID, "", "sai.activity_id", WorkflowConstants.DATATYPE_INT, WorkflowConstants.BUILD_WHERE_CLAUSE_IN, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));
            //We are using "istrActivityID" hence commenting this condition.
            //if (iintActivityID > 0)
            //lcolWhereClause.Add(busNeoBase.GetWhereClause(iintActivityID, "", "sai.activity_id", WorkflowConstants.DATATYPE_INT, WorkflowConstants.OPERATOR_EQUALTO, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));

            if (iintOrgnID > 0)
                lcolWhereClause.Add(busNeoBase.GetWhereClause(iintOrgnID, "", "swr.org_id", WorkflowConstants.DATATYPE_INT, WorkflowConstants.OPERATOR_EQUALTO, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));

            if (!string.IsNullOrEmpty(istrMPIPersonID))
                lcolWhereClause.Add(busNeoBase.GetWhereClause(istrMPIPersonID, "", "P.MPI_PERSON_ID", WorkflowConstants.DATATYPE_STRING, "like", WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));

            //This condition is added to prevent the retrieving of activity instances whose process instances are completed bcz of any error.
            //we are excluding such activity instances bcz for such activity instances there may not be any valid information in persistence store,
            //and hence activity instance completion is not possible, it will fail always also completed activity instances are not supposed to be re-assigned.
            lcolWhereClause.Add(busNeoBase.GetWhereClause("'INPC'", "", "spi.status_value", WorkflowConstants.DATATYPE_STRING, WorkflowConstants.BUILD_WHERE_CLAUSE_IN, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));

            if (!String.IsNullOrEmpty(istrPriority))
            {
                lcolWhereClause.Add(busNeoBase.GetWhereClause(istrPriority, "", "PMI.priority_code_value", WorkflowConstants.DATATYPE_STRING, WorkflowConstants.OPERATOR_EQUALTO, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));
            }

            if (iintRoleID > 0)
                lcolWhereClause.Add(busNeoBase.GetWhereClause(iintRoleID, "", "sa.role_id", WorkflowConstants.DATATYPE_INT, WorkflowConstants.OPERATOR_EQUALTO, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));


            if (!String.IsNullOrEmpty(istrCheckedOutUser))
                lcolWhereClause.Add(busNeoBase.GetWhereClause(istrCheckedOutUser, "", "sai.checked_out_user", WorkflowConstants.DATATYPE_STRING, WorkflowConstants.OPERATOR_EQUALTO, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));

            if ((idtCreatedDateFrom != DateTime.MinValue) || (idtCreatedDateTo != DateTime.MinValue))
            {
                if (idtCreatedDateFrom == DateTime.MinValue)
                {
                    idtCreatedDateFrom = (DateTime)SqlDateTime.MinValue;
                }
                if (idtCreatedDateTo == DateTime.MinValue)
                {
                    idtCreatedDateTo = (DateTime)SqlDateTime.MaxValue;
                }
                // Developer - Rahul Mane
                // Date - 07/01/2021
                // Comment - Fixed PIR - 3631 (Server error is displayed after selecting Activity Initiated Date From on wfmBpmReassignWorkMaintenance screen)
                // Iteration - Main-Iteration9

                if (DBFunction.IsOracleConnection())
                    lcolWhereClause.Add(busNeoBase.GetWhereClause(idtCreatedDateFrom, idtCreatedDateTo == DateTime.MinValue ? DateTime.MaxValue : idtCreatedDateTo.AddDays(1).AddSeconds(-1), "cast(sai.start_date as date)", WorkflowConstants.DATATYPE_DATETIME, WorkflowConstants.OPERATOR_BETWEEN, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));
                else
                    lcolWhereClause.Add(busNeoBase.GetWhereClause(idtCreatedDateFrom, idtCreatedDateTo, "cast(sai.start_date as date)", WorkflowConstants.DATATYPE_DATETIME, WorkflowConstants.OPERATOR_BETWEEN, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));

                if (idtCreatedDateFrom == (DateTime)SqlDateTime.MinValue)
                {
                    idtCreatedDateFrom = DateTime.MinValue;
                }
                if (idtCreatedDateTo == (DateTime)SqlDateTime.MaxValue)
                {
                    idtCreatedDateTo = DateTime.MinValue;
                }
            }
            if ((idtActivityDueDateFrom != DateTime.MinValue) || (idtActivityDueDateTo != DateTime.MinValue))
            {
                if (idtActivityDueDateFrom == DateTime.MinValue)
                {
                    idtActivityDueDateFrom = (DateTime)SqlDateTime.MinValue;
                }
                if (idtActivityDueDateTo == DateTime.MinValue)
                {
                    idtActivityDueDateTo = (DateTime)SqlDateTime.MaxValue;
                }
                if (DBFunction.IsOracleConnection())
                    lcolWhereClause.Add(busNeoBase.GetWhereClause(idtActivityDueDateFrom, idtActivityDueDateTo == DateTime.MinValue ? DateTime.MaxValue : idtActivityDueDateTo.AddDays(1).AddSeconds(-1), "cast(sai.due_date as date)", WorkflowConstants.DATATYPE_DATETIME, WorkflowConstants.OPERATOR_BETWEEN, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));
                else
                    lcolWhereClause.Add(busNeoBase.GetWhereClause(idtActivityDueDateFrom, idtActivityDueDateTo, "cast(sai.due_date as date)", WorkflowConstants.DATATYPE_DATETIME, WorkflowConstants.OPERATOR_BETWEEN, WorkflowConstants.BUILD_WHERE_CLAUSE_AND, astrQueryId));
                if (idtActivityDueDateFrom == (DateTime)SqlDateTime.MinValue)
                {
                    idtActivityDueDateFrom = DateTime.MinValue;
                }
                if (idtActivityDueDateTo == (DateTime)SqlDateTime.MaxValue)
                {
                    idtActivityDueDateTo = DateTime.MinValue;
                }
            }
            return lcolWhereClause;
        }

        /// <summary>
        /// Method to check reassign user is available or not.
        /// </summary>
        /// <returns>Return true if user available or false if not.</returns>
        private bool CheckReassignUserAvailable()
        {
            try
            {
                int lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("entUserTimeoff.CheckReassignUserAvailable",
                                new object[1] { istrReassignUser }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));

                return lintCount > busNeoBaseConstants.ZERO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion [Private Methods]
    }
}

