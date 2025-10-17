using System;
using System.Collections;
using MPIPHP.Common;
using MPIPHP.Interface;
using MPIPHP.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using System.Data;
using Sagitec.DBUtility;
using System.Collections.Generic;
using System.Linq;


namespace MPIPHP.BusinessObjects
{
    public static class busWorkflowHelper
    {
        public static ArrayList UpdateWorkflowActivityByEvent(busBase abusObject, enmNextAction anmAction, string astrStatus, utlPassInfo aobjPassInfo)
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = null;
            utlPassInfo.iobjPassInfo = aobjPassInfo;
            busActivityInstance lobjActivityInstance = null;
            if (abusObject is busActivityInstance)
            {
                lobjActivityInstance = (busActivityInstance)abusObject;
            }
            else
            {
                lobjActivityInstance = (busActivityInstance)abusObject.ibusBaseActivityInstance;
            }
            if (lobjActivityInstance != null && lobjActivityInstance.icdoActivityInstance != null && lobjActivityInstance.icdoActivityInstance.activity_instance_id > 0)
            {
                //Update SEQ Fix : To Avoid Record Changed Since last modified.
                if (lobjActivityInstance.iclbProcessInstanceChecklist.IsNull())
                    lobjActivityInstance.LoadProcessInstanceChecklist();

                if (!lobjActivityInstance.iclbProcessInstanceChecklist.IsNullOrEmpty())
                {
                    int Count = lobjActivityInstance.iclbProcessInstanceChecklist.Where(i => i.icdoProcessInstanceChecklist.required_flag == busConstant.Flag_Yes
                                                                                        && (i.icdoProcessInstanceChecklist.received_date == DateTime.MinValue || i.icdoProcessInstanceChecklist.received_date.IsNull())).Count();
                    if (Count > 0)
                    {
                        lobjError = new utlError();
                        lobjError.istrErrorMessage = "Cannot Complete/Proceed the WorkFlow unless all the Required Documents are received.";
                        larrResult.Add(lobjError);

                        return larrResult;

                    }
                }

                object lobjUpdateSeq = DBFunction.DBExecuteScalar("select update_seq from sgw_activity_instance where activity_instance_id = " +
                    lobjActivityInstance.icdoActivityInstance.activity_instance_id.ToString(),  aobjPassInfo.iconFramework,  aobjPassInfo.itrnFramework);
                if (lobjUpdateSeq is Int32 && lobjActivityInstance.icdoActivityInstance.update_seq < (Int32)lobjUpdateSeq)
                {
                    lobjActivityInstance.icdoActivityInstance.Select();
                }

                lobjActivityInstance.icdoActivityInstance.busObject = abusObject;
                lobjActivityInstance.icdoActivityInstance.checked_out_user = aobjPassInfo.istrUserID;
                lobjActivityInstance.icdoActivityInstance.UserId = aobjPassInfo.istrUserID;
                lobjActivityInstance.icdoActivityInstance.UserSerialId = aobjPassInfo.iintUserSerialID;
                lobjActivityInstance.icdoActivityInstance.status_value = astrStatus;
                if (astrStatus == busConstant.ActivityStatusReturned)
                {
                    lobjActivityInstance.icdoActivityInstance.return_from_audit_flag = busConstant.Return_From_Audit_Flag_Yes;
                }
                else if (astrStatus == busConstant.ActivityStatusReturnedToAudit)
                {
                    lobjActivityInstance.icdoActivityInstance.return_from_audit_flag = busConstant.Return_From_Audit_Flag_No;
                }

                ActivityInstanceEventArgs aiEventArgs = new ActivityInstanceEventArgs();
                aiEventArgs.icdoActivityInstance = lobjActivityInstance.icdoActivityInstance;
                aiEventArgs.ienmNextAction = anmAction;//TODO: Refactor it later to decide when and how we should use it
                if (lobjActivityInstance.ibusProcessInstance.IsNull())
                    lobjActivityInstance.LoadProcessInstance();
                larrResult = RaiseWorkflowEvent(lobjActivityInstance.ibusProcessInstance.icdoProcessInstance.workflow_instance_guid, aiEventArgs);

            }
            else
            {
                lobjError = new utlError();
                lobjError.istrErrorMessage = "Workflow instance not found.";

                larrResult.Add(lobjError);
            }

            return larrResult;
        }

        public static ArrayList RaiseWorkflowEvent(Guid aGuidInstanceId, ActivityInstanceEventArgs aiEventArgs)
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = null;
            try
            {
                //FM upgrade: 6.0.7.0 changes - Removed support for reading system settings through HelperFunctions
                //string url = HelperFunction.GetAppSettings(WorkflowConstants.WorkflowServiceURL);
                string url = MPIPHP.Common.ApplicationSettings.Instance.NEOFLOW_SERVICE_WORKFLOW_URL;
                IWorkflowEngine engine = (IWorkflowEngine)Activator.GetObject(typeof(IWorkflowEngine), url);
                bool result = engine.ResumeBookmark(aGuidInstanceId, aiEventArgs);
                if (!result)
                {
                    lobjError = new utlError();
                    lobjError.istrErrorMessage = "Could not run workflow.";
                    larrResult.Add(lobjError);
                }
            }
            catch (Exception Ex)
            {
                string lstrMessage = string.Format("Exception Occured while resumeing the bookmak {0} of the activity {1}. Detailed error is : {2}"
                                    , aiEventArgs.icdoActivityInstance.activity_id, aiEventArgs.icdoActivityInstance.activity_id, Ex.ToString());

                ExceptionManager.Publish(new Exception(lstrMessage));
                throw new Exception(lstrMessage);
            }
            return larrResult;
        }

        public static ArrayList UpdateWorkflowActivityByStatus(string astrStatus, busMainBase abusActivityInstance, utlPassInfo aobjPassInfo)
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = null;
            utlPassInfo.iobjPassInfo = aobjPassInfo;

            busActivityInstance lobjActivityInstance = (busActivityInstance)abusActivityInstance;

            if (lobjActivityInstance != null && lobjActivityInstance.icdoActivityInstance != null && lobjActivityInstance.icdoActivityInstance.activity_instance_id > 0)
            {
                try
                {
                    //Update the Status, UserId of Activity Instance                    
                    lobjActivityInstance.icdoActivityInstance.status_value = astrStatus;
                    lobjActivityInstance.icdoActivityInstance.checked_out_user = String.Empty;
                    if (astrStatus == busConstant.ActivityStatusSuspended)
                    {
                        lobjActivityInstance.icdoActivityInstance.suspension_end_date = lobjActivityInstance.icdoActivityInstance.suspension_end_date;
                        lobjActivityInstance.icdoActivityInstance.suspension_start_date = DateTime.Now;
                        lobjActivityInstance.icdoActivityInstance.checked_out_user = aobjPassInfo.istrUserID;
                        lobjActivityInstance.icdoActivityInstance.resume_action_value = lobjActivityInstance.icdoActivityInstance.resume_action_value;
                    }
                    else if ((astrStatus == busConstant.ActivityStatusInProcess) || (astrStatus == busConstant.ActivityStatusResumed))
                    {
                        lobjActivityInstance.icdoActivityInstance.checked_out_user = aobjPassInfo.istrUserID;
                    }
                    lobjActivityInstance.icdoActivityInstance.Update();

                    //PROD PIR : 4060 Release the Workflow to Workflow If the resumed User Role is expired.
                    if (astrStatus == busConstant.ActivityStatusResumed)
                    {
                        busUser lbusCheckedoutUser = new busUser();
                        lbusCheckedoutUser.FindUserByUserName(lobjActivityInstance.icdoActivityInstance.checked_out_user);
                        if (lobjActivityInstance.ibusActivity == null)
                            lobjActivityInstance.LoadActivity();
                        if (!lbusCheckedoutUser.IsMemberActiveInRole(lobjActivityInstance.ibusActivity.icdoActivity.role_id, DateTime.Now))
                        {
                            larrResult = busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusReleased, lobjActivityInstance, aobjPassInfo);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    lobjError = new utlError();
                    lobjError.istrErrorMessage = "Could not checkout the activity.: " + Ex.Message;

                    larrResult.Add(lobjError);
                }
            }
            else
            {
                lobjError = new utlError();
                lobjError.istrErrorMessage = "Workflow instance not found.";

                larrResult.Add(lobjError);
            }

            return larrResult;
        }

        /// <summary>
        /// Method to check whether any running instance is available for person and process
        /// </summary>
        /// <param name="aintPersonID">Person ID</param>
        /// <param name="aintProcessID">Process ID</param>
        /// <returns>bool value</returns>
        public static bool IsActiveInstanceAvailable(int aintPersonID, int aintProcessID)
        {
            bool lblnResult = false;

            DataTable ldtRunningInstance = new DataTable();
            ldtRunningInstance = LoadRunningInstancesByPersonAndProcess(aintPersonID, aintProcessID);
            if (ldtRunningInstance.Rows.Count > 0)
                lblnResult = true;
            return lblnResult;
        }

        /// <summary>
        /// Method to load all running instance for person and process
        /// </summary>
        /// <param name="aintPersonID">Person ID</param>
        /// <param name="aintProcessID">Process ID</param>
        /// <returns>Data table</returns>
        public static DataTable LoadRunningInstancesByPersonAndProcess(int aintPersonID, int aintProcessID)
        {
            return busBase.Select("cdoActivityInstance.LoadRunningInstancesByPersonAndProcess",
                                    new object[2] { aintPersonID, aintProcessID });
        }

        /// <summary>
        /// Method to check whether any running instance is available for org and process
        /// </summary>
        /// <param name="aintOrgID">Org ID</param>
        /// <param name="aintProcessID">Process ID</param>
        /// <returns>bool value</returns>
        public static bool IsActiveInstanceAvailableForOrg(int aintOrgID, int aintProcessID)
        {
            bool lblnResult = false;

            DataTable ldtRunningInstance = new DataTable();
            ldtRunningInstance = LoadRunningInstancesByOrgAndProcess(aintOrgID, aintProcessID);
            if (ldtRunningInstance.Rows.Count > 0)
                lblnResult = true;
            return lblnResult;
        }

        /// <summary>
        /// Method to load all running instance for org and process
        /// </summary>
        /// <param name="aintOrgID">Org ID</param>
        /// <param name="aintProcessID">Process ID</param>
        /// <returns>Data table</returns>
        public static DataTable LoadRunningInstancesByOrgAndProcess(int aintOrgID, int aintProcessID)
        {
            return busBase.Select("cdoActivityInstance.LoadRunningInstancesByOrgAndProcess",
                                    new object[2] { aintOrgID, aintProcessID });
        }

        /// <summary>
        /// Initialize the workflow.
        /// </summary>
        /// <param name="astrProcessName"></param>
        public static ArrayList InitializeWorkflow(string astrProcessName, int aintPersonID, int aintOrgID, long aintReferenceID, Hashtable ahstRequestParameters)
        {
            ArrayList larrResult = null;
            busWorkflowRequest lbusWorkFlowRequest = new busWorkflowRequest { icdoWorkflowRequest = new cdoWorkflowRequest() };

            lbusWorkFlowRequest.icdoWorkflowRequest.person_id = aintPersonID;            
            lbusWorkFlowRequest.icdoWorkflowRequest.reference_id = aintReferenceID;

            if (ahstRequestParameters.IsNull())
            {
                larrResult = lbusWorkFlowRequest.AddNewWorkFlowRequest(astrProcessName);
            }
            else
            {
                larrResult = lbusWorkFlowRequest.AddNewWorkFlowRequest(astrProcessName, ahstRequestParameters);
            }

            if (larrResult.Count > 0 && larrResult[0] is utlError)
            {
                ExceptionManager.Publish(new Exception("Error occured while initilizing the workflow for process" + astrProcessName + ". The detailed error is: " + (larrResult[0] as utlError).istrErrorMessage));
            }
            return larrResult;
        }

        /// <summary>
        /// Initialize Workflow only if one does not exist.
        /// </summary>
        /// <param name="astrProcessName"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="aintOrgId"></param>
        /// <param name="aintReferenceID"></param>
        /// <returns></returns>
        public static ArrayList InitializeWorkflowIfNotExists(string astrProcessName, int aintPersonId, int aintOrgId, int aintReferenceID, Hashtable ahstRequestParameters,
            bool ablnCheckInstanceWithRefId = false)
        {
            ArrayList larrResult = null;
            busWorkflowRequest lbusWorkFlowRequest = new busWorkflowRequest { icdoWorkflowRequest = new cdoWorkflowRequest() };
            //Get the process id for the workflow process name passed as an argument.
            int lintProcessId = lbusWorkFlowRequest.GetProcessIDForWorkflow(astrProcessName);

            //Process is required to initialize workflow request.
            if (lintProcessId == 0)
            {
                utlError lutlError = new utlError();
                lutlError.istrErrorMessage = "Could not find the process " + astrProcessName;
                larrResult.Add(lutlError);
            }

            if (aintPersonId > 0)
            {
                if (!lbusWorkFlowRequest.IsActiveWorkflowExistsForPerson(aintPersonId, lintProcessId, ablnCheckInstanceWithRefId, aintReferenceID))
                    larrResult = InitializeWorkflow(astrProcessName, aintPersonId, aintOrgId, aintReferenceID, ahstRequestParameters);
            }

            else if(aintOrgId > 0)
            {

            }
             
            return larrResult;
        }

        /// <summary>
        /// Initialize Workflow only if one does not exist.
        /// </summary>
        /// <param name="astrProcessName"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="aintOrgId"></param>
        /// <param name="aintReferenceID"></param>
        /// <returns></returns>
        //public static ArrayList InitializeWorkflowIfNotExistsViaReferenceId(string astrProcessName, int aintPersonId, int aintOrgId, int aintReferenceID)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    busRequest lbusRequest = new busRequest();
        //    //Get the process id for the workflow process name passed as an argument.
        //    int lintProcessId = lbusRequest.GetProcessIDForWorkflow(astrProcessName);

        //    //Process is required to initialize workflow request.
        //    if (lintProcessId == 0)
        //    {
        //        utlError lutlError = new utlError();
        //        lutlError.istrErrorMessage = "Could not find the process " + astrProcessName;
        //        larrResult.Add(lutlError);
        //    }

        //    if (!lbusRequest.IsProcessRequestUnprocessedForReference(lintProcessId, aintReferenceID) &&
        //        !lbusRequest.IsProcessInstanceActiveForReference(lintProcessId, aintReferenceID))
        //    {
        //        larrResult = InitializeWorkflow(astrProcessName, aintPersonId, aintOrgId, aintReferenceID,null);
        //    }
        //    return larrResult;
        //}

    }
}
