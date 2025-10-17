#region [Using Directives]
using MPIPHP.BusinessTier;
using NeoBase.Common;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DataObjects;
using Sagitec.DBCache;
using Sagitec.Interface;
using Sagitec.MetaDataCache;
using Sagitec.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
#endregion [Using Directives]

namespace NeoBPMN.Service
{
    public partial class frmBPMNService : Form
    {
        #region [Constants]

        //private const string WORKFLOW_INSTANCE_POLLING_INTERVAL = "WORKFLOW_INSTANCE_POLLING_INTERVAL";

        //private const string RESUME_INSTANCE_POLLING_INTERVAL = "RESUME_INSTANCE_POLLING_INTERVAL";

        //private const string PENDING_REQUEST_POLLING_INTERVAL = "PENDING_REQUEST_POLLING_INTERVAL";

        //private const string PENDING_REQUEST_COMPLETION_INTERVAL = "PENDING_REQUEST_COMPLETION_INTERVAL";

        //private const string TIMER_ACTIVITY_TRIGGER_INTERVAL = "TIMER_ACTIVITY_TRIGGER_INTERVAL";

        //private const string ESCALATIONS_CHECK_INTERVAL = "ESCALATIONS_CHECK_INTERVAL";

        //private const string BASE_PATH = @"D:\BPMN\";
        #endregion

        #region [Members]

        string[] arguments;
        //string _istrUserId = "BPM Service";

        ////timer to check unprocessed requests
        //private System.Timers.Timer itmrRequestInstanceTimer = null;

        ////timer to resume suspended workflow if their resume date time is met
        //private System.Timers.Timer itmrResumeWorkflowInstanceTimer = null;

        ////timer to trigger Intermediate Timer Catch Events
        //private System.Timers.Timer itmrTimerEventsTimer = null;

        ////timer to trigger escalations if any
        //private System.Timers.Timer itmrEscalationsCheckTimer = null;

        ////timer to trigger process escalations if any
        //private System.Timers.Timer itmrProcessEscalationsCheckTimer = null;
        ////timer for uploading correspondence to ecm
        //private System.Timers.Timer itmrTiffConversionTimer = null;

        ////Handler to process queue objects
        //private busBpmQueueRequestHandler _ibusBpmQueueRequestHandler = null;
        ////  private busBpmTaskRequestHandler _ibusBpmTaskRequestHandler = null;

        ///// <summary>
        ///// Holds the unprocessed requests
        ///// </summary>
        //Collection<busSolBpmRequest> iclbUnprocessedRequests { get; set; }

        ///// <summary>
        ///// Holds the elapsed timer activity instances
        ///// </summary>
        //Collection<busBpmTimerActivityInstanceDetails> iclbElapsedTimerActivityInstances { get; set; }

        //// <summary>
        ///// Holds the escalation instances
        ///// </summary>
        //Collection<busSolBpmEscalationInstance> iclbEscalationInstances { get; set; }

        //// <summary>
        ///// Holds the process escalation instances
        ///// </summary>
        //Collection<busSolBpmProcessEscalationInstance> iclbProcessEscalationInstances { get; set; }

        //private string istrGeneratedPath;
        //private string istrImagedPath;
        //private string istrImagingPrinterName;
        //private string istrPurgedPath;
        ////private CorBuilder iobjCorBuilder;

        /// <summary>
        /// Service host collection
        /// </summary>
        private ICollection<ServiceHost> _icolServiceHosts;

        #endregion

        #region [Constructor/Destructor]

        public frmBPMNService(string[] args)
        {
            this.arguments = args;
            this._icolServiceHosts = new Collection<ServiceHost>();
            InitializeComponent();
        }

        #endregion

        #region [Event Handlers]

        ///// <summary>
        ///// Request Instance Time Elapsed Event Handler
        ///// </summary>
        ///// <param name="sender">Sender</param>
        ///// <param name="e">ElapsedEventArgs</param>
        //void itmrRequestInstanceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    //We are stopping the Timer here because if the task execution time takes more than interval time
        //    this.itmrRequestInstanceTimer.Stop();

        //    InitializeUtlPassInfo();
        //    try
        //    {
        //        LoadUnprocessedRequests();

        //        foreach (busBpmRequest lbusBpmRequest in iclbUnprocessedRequests)
        //        {
        //            try
        //            {
        //                utlPassInfo.iobjPassInfo.BeginTransaction();
        //                lbusBpmRequest.icdoBpmRequest.status_value = BpmRequestStatus.Picked;
        //                lbusBpmRequest.icdoBpmRequest.Update();
        //                lbusBpmRequest.icdoBpmRequest.Select();
        //                utlPassInfo.iobjPassInfo.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                ExceptionManager.Publish(ex);
        //                utlPassInfo.iobjPassInfo.Rollback();
        //                continue;
        //            }

        //            try
        //            {
        //                if (lbusBpmRequest.icdoBpmRequest.source_value.Equals(BpmRequestSource.MessageFlow))
        //                {
        //                    //Request initiated from message flow
        //                    SendMessageToTargetActivityInstance(lbusBpmRequest);
        //                }
        //                else
        //                {
        //                    //Request initiated online or from ECM
        //                    ProcessRequest(lbusBpmRequest);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ExceptionManager.Publish(ex);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionManager.Publish(ex);
        //    }
        //    finally
        //    {
        //        FreePassInfo(utlPassInfo.iobjPassInfo);
        //    }

        //    //Starting back the timer
        //    this.itmrRequestInstanceTimer.Start();
        //}

        ///// <summary>
        ///// Find the list of SUSPENDED activity instances,
        ///// If the suspension date/time of those instances is current data/time
        ///// then change the status of those activity instances to RESUMED.
        ///// </summary>        
        ///// <param name="sender">Sender</param>
        ///// <param name="e">ElapsedEventArgs</param>
        //void itmrResumeWorkflowInstanceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    //Find the list of SUSPENDED activity instances,
        //    //If the suspension date/time of those instances is current data/time
        //    //then change the status of those activity instances to RESUMED.

        //    //We are stopping the Timer here because if the task execution time takes more than interval time.
        //    this.itmrResumeWorkflowInstanceTimer.Stop();

        //    InitializeUtlPassInfo();

        //    try
        //    {
        //        ResumeSuspendedWorkflowInstances();
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionManager.Publish(ex);
        //    }
        //    finally
        //    {
        //        FreePassInfo(utlPassInfo.iobjPassInfo);
        //    }

        //    //Starting back the timer
        //    this.itmrResumeWorkflowInstanceTimer.Start();
        //}

        ///// <summary>
        ///// Timer Event 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void itmrTimerEventsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    //We are stopping the Timer here because if the task execution time takes more than interval time
        //    this.itmrTimerEventsTimer.Stop();

        //    utlPassInfo.iobjPassInfo = new utlPassInfo();
        //    utlPassInfo.iobjPassInfo.idictParams = new Dictionary<string, object>();
        //    utlPassInfo.iobjPassInfo.idictParams[utlConstants.istrConstUserID] = _istrUserId;
        //    utlPassInfo.iobjPassInfo.idictParams.Add(utlConstants.istrRequestIPAddress, utlFunctions.GetSystemIPAddress());
        //    utlPassInfo.iobjPassInfo.iconFramework = DBFunction.GetDBConnection();

        //    try
        //    {
        //        LoadElapsedTimerActivities();

        //        foreach (busBpmTimerActivityInstanceDetails lbusBpmTimerActivityInstanceDetails in iclbElapsedTimerActivityInstances)
        //        {
        //            try
        //            {
        //                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //                SqlParameter parameter = new SqlParameter("@activity_instance_id", lbusBpmTimerActivityInstanceDetails.icdoBpmTimerActivityInstanceDetails.activity_instance_id);
        //                parameter.DbType = DbType.Int32;
        //                lcolParameters.Add(parameter);

        //                int lintCaseInstId = DBFunction.DBGetInt("entBpmActivityInstance.GetCaseInstanceIdFromActivityInstance", lcolParameters, utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
        //                busBpmCaseInstance lbusBpmCaseInstance = new busBpmCaseInstance();
        //                busBpmActivityInstance lbusTimerActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst(lbusBpmTimerActivityInstanceDetails.icdoBpmTimerActivityInstanceDetails.activity_instance_id);

        //                utlPassInfo.iobjPassInfo.BeginTransaction();
        //                ((busBpmIntermediateTimerCatchEvent)lbusTimerActivityInstance.ibusBpmActivity).TriggerTimer(lbusTimerActivityInstance, lbusBpmTimerActivityInstanceDetails);
        //                utlPassInfo.iobjPassInfo.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                ExceptionManager.Publish(ex);
        //                utlPassInfo.iobjPassInfo.Rollback();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionManager.Publish(ex);
        //    }
        //    finally
        //    {
        //        FreePassInfo(utlPassInfo.iobjPassInfo);
        //    }

        //    //Starting back the timer
        //    this.itmrTimerEventsTimer.Start();
        //}

        ///// <summary>
        ///// Process Escalation Check Timer Event
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void itmrProcessEscalationsCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    //We are stopping the Timer here because if the task execution time takes more than interval time
        //    this.itmrProcessEscalationsCheckTimer.Stop();
        //    InitializeUtlPassInfo();
        //    try
        //    {
        //        LoadProcessEscalations();

        //        foreach (busSolBpmProcessEscalationInstance lbusBpmProcessEscalationInstance in iclbProcessEscalationInstances)
        //        {
        //            try
        //            {
        //                utlPassInfo.iobjPassInfo.BeginTransaction();
        //                lbusBpmProcessEscalationInstance.LoadRelatedObjects();
        //                lbusBpmProcessEscalationInstance.Escalate();
        //                utlPassInfo.iobjPassInfo.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                ExceptionManager.Publish(ex);
        //                utlPassInfo.iobjPassInfo.Rollback();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionManager.Publish(ex);
        //    }
        //    finally
        //    {
        //        FreePassInfo(utlPassInfo.iobjPassInfo);
        //    }

        //    //Starting back the timer
        //    this.itmrProcessEscalationsCheckTimer.Start();
        //}

        ///// <summary>
        ///// Escalation Check Timer Event
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void itmrEscalationsCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    //We are stopping the Timer here because if the task execution time takes more than interval time
        //    this.itmrEscalationsCheckTimer.Stop();
        //    InitializeUtlPassInfo();
        //    try
        //    {
        //        LoadEscalations();

        //        foreach (busBpmEscalationInstance lbusBpmEscalationInstance in iclbEscalationInstances)
        //        {
        //            try
        //            {
        //                utlPassInfo.iobjPassInfo.BeginTransaction();
        //                lbusBpmEscalationInstance.LoadRelatedObjects();
        //                lbusBpmEscalationInstance.Escalate();
        //                utlPassInfo.iobjPassInfo.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                ExceptionManager.Publish(ex);
        //                utlPassInfo.iobjPassInfo.Rollback();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionManager.Publish(ex);
        //    }
        //    finally
        //    {
        //        FreePassInfo(utlPassInfo.iobjPassInfo);
        //    }

        //    //Starting back the timer
        //    this.itmrEscalationsCheckTimer.Start();
        //}

        /// <summary>
        /// NeoFlow Service Form Closing Event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">FormClosingEventArgs</param>
        private void frmNeoFlowService_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.StopTimers();
            ExtendedBPMService.Instance.Stop();
            StopServiceHosts();
        }

        /// <summary>
        /// BPMNService Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="aEventArgs"></param>
        private void frmBPMNService_Shown(object sender, EventArgs aEventArgs)
        {
            // this.Text += " " + 
            lblStatus.Text = "Please wait while starting service...";
            Application.DoEvents();
            if (!InitializeCache())
            {
                this.Close();
                return;
            }
            //if (!InitializeTimers())
            //{
            //    this.Close();
            //    return;
            //}
            ExtendedBPMService.Initialize(arguments);
            ExtendedBPMService.Instance.Start();
            InitServiceHosts();
        }

        /// <summary>
        /// Form Close Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="aEventArgs"></param>
        private void btnClose_Click(object sender, EventArgs aEventArgs)
        {
            this.Close();
        }

        /// <summary>
        /// Refresh MetadataCache Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="aEventArgs"></param>
        private void btnRefreshMetaDataCache_Click(object sender, EventArgs aEventArgs)
        {
            //this.StopTimers();
            ExtendedBPMService.Instance.Suspend();
            this.LoadMetaDataCache();
            //this.InitializeTimers();
            ExtendedBPMService.Instance.Resume();
        }

        /// <summary>
        /// Refresh DB Cache Clack Event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="aEventArgs"></param>
        private void btnRefreshDBCache_Click(object sender, EventArgs aEventArgs)
        {
            //this.StopTimers();
            ExtendedBPMService.Instance.Suspend();
            this.LoadDBCache();
            //this.InitializeTimers();
            ExtendedBPMService.Instance.Resume();
        }

        /// <summary>
        /// Rfresh Rules Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="aEventArgs"></param>
        private void btnRefreshRules_Click(object sender, EventArgs e)
        {
            //this.StopTimers();
            ExtendedBPMService.Instance.Suspend();
            this.ExecuteRules();           
            //this.InitializeTimers();
            ExtendedBPMService.Instance.Resume();
            (new busNeoBase()).InitializeExtendedObjects();
        }
        #endregion

        #region [Private Methods]

        /// <summary>
        /// Initializes Utlpassinfo object
        /// </summary>
        //private void InitializeUtlPassInfo()
        //{
        //    utlPassInfo.iobjPassInfo = new utlPassInfo();
        //    utlPassInfo.iobjPassInfo.idictParams = new Dictionary<string, object>();
        //    utlPassInfo.iobjPassInfo.idictParams[utlConstants.istrConstUserID] = _istrUserId;
        //    utlPassInfo.iobjPassInfo.idictParams.Add(utlConstants.istrRequestIPAddress, utlFunctions.GetSystemIPAddress());
        //    utlPassInfo.iobjPassInfo.iconFramework = DBFunction.GetDBConnection();
        //}

        ///// <summary>
        ///// Closes the disposes the utlPassInfo object and its children
        ///// </summary>
        ///// <param name="aobjPassInfo"></param>
        //private void FreePassInfo(utlPassInfo aobjPassInfo)
        //{
        //    if ((aobjPassInfo.iconFramework != null) &&
        //        (aobjPassInfo.iconFramework.State == ConnectionState.Open))
        //    {
        //        aobjPassInfo.iconFramework.Close();
        //        aobjPassInfo.iconFramework.Dispose();
        //    }
        //    aobjPassInfo.isrvDBCache = null;
        //    aobjPassInfo.isrvMetaDataCache = null;
        //    aobjPassInfo = null;
        //}

        /// <summary>
        /// Caches BPM Case object in memory
        /// </summary>
        private void CacheBpmCase()
        {
            //InitializeUtlPassInfo();
            // If we want to load any huge BPM map into cache,while starting Business Tier,so it gets loaded quickly for the first time it is being referred,
            //then uncomment these lines and add the reference of that map as given below.
            //Sagitec.Bpm.BpmCaseCache.Instance.AddToCache("sbpCreatePersonAddress", utlPassInfo.iobjPassInfo, true, new TimeSpan(1, 0, 0, 0), MyCachePriority.SlidingExpiration);
            //FreePassInfo(utlPassInfo.iobjPassInfo);
        }

        ///// <summary>
        ///// This function is used to resume the suspended workflows. All the workitems whose suspension end date is less than or equal to the current date will be 
        ///// resumed as a result of this function invokation.
        ///// </summary>
        //private void ResumeSuspendedWorkflowInstances()
        //{
        //    //
        //    DataTable ldtbActivityInstancesToResume = busBase.Select("entBpmActivityInstance.GetUserActivitiesToResume", new object[1] { busGlobalFunctions.GetSystemDate() });
        //    foreach (DataRow ldtrRow in ldtbActivityInstancesToResume.Rows)
        //    {
        //        int lintBpmActivityInstanceId = (int)ldtrRow["ACTIVITY_INSTANCE_ID"];
        //        busSolBpmCaseInstance lbusSolBpmCaseInstance = new busSolBpmCaseInstance();
        //        busBpmActivityInstance lbusActivityInstanceToResume = lbusSolBpmCaseInstance.LoadWithActivityInst(lintBpmActivityInstanceId);
        //        lbusActivityInstanceToResume.UpdateBpmActivityInstanceByStatus(BpmActivityInstanceStatus.Resumed);
        //    }
        //}

        ///// <summary>
        ///// This function is used to initialize the workflow for the request instance passed as an argument.
        ///// </summary>
        ///// <param name="abusBpmRequest">Request instance.</param>
        //private void SendMessageToTargetActivityInstanceWhenIncomingDocumentReceived(busBpmRequest abusBpmRequest)
        //{
        //    bool lblnRequestProcessed = false;
        //    try
        //    {
        //        Collection<busBpmCaseInstance> lclbCaseInstances = busBpmCaseInstance.GetRunningCaseInstancesForPersonId(abusBpmRequest.icdoBpmRequest.person_id);
        //        foreach (busBpmCaseInstance lbusBpmCaseInstance in lclbCaseInstances)
        //        {
        //            lbusBpmCaseInstance.ibusBpmCase = busBpmCase.GetBpmCase(lbusBpmCaseInstance.icdoBpmCaseInstance.case_id);
        //            if (lbusBpmCaseInstance.ibusBpmCase != null)
        //            {
        //                foreach (busBpmProcess lbusBpmProcess in lbusBpmCaseInstance.ibusBpmCase.iclbBpmProcess)
        //                {
        //                    lbusBpmProcess.iclbBpmActivity.ForEach(bpmActivity => bpmActivity.LoadRoles());
        //                }
        //            }
        //            lbusBpmCaseInstance.LoadBpmCaseInstanceParameters();
        //            lbusBpmCaseInstance.LoadBpmProcessInstances();
        //            lbusBpmCaseInstance.LoadBpmRequest();
        //            abusBpmRequest.icdoBpmRequest.case_instance_id = lbusBpmCaseInstance.icdoBpmCaseInstance.case_instance_id;
        //            abusBpmRequest.icdoBpmRequest.Update();
        //            busBpmCaseMessageFlow lbusBpmCaseMessageFlow = lbusBpmCaseInstance.ibusBpmCase.iclbBpmCaseMessageFlow.Where(messageFlow => messageFlow.icdoBpmCaseMessageFlow.bpm_message_flow_id == abusBpmRequest.icdoBpmRequest.bpm_message_flow_id).FirstOrDefault();
        //            if (lbusBpmCaseMessageFlow != null)
        //            {
        //                busBpmProcessInstance lbusTargetProcessInstance = lbusBpmCaseInstance.GetProcInstWithBpmProcessId(lbusBpmCaseMessageFlow.icdoBpmCaseMessageFlow.target_bpm_process_id);
        //                if (lbusTargetProcessInstance != null)
        //                {
        //                    lbusTargetProcessInstance.ReceiveMessage(lbusBpmCaseMessageFlow);
        //                }
        //            }
        //        }
        //        abusBpmRequest.icdoBpmRequest.status_value = BpmRequestStatus.Processed;
        //        abusBpmRequest.icdoBpmRequest.Update();
        //        lblnRequestProcessed = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionManager.Publish(new Exception("An error occured sending message to activity request id: " + abusBpmRequest.icdoBpmRequest.request_id + " Error Message : " + ex));
        //    }

        //    if (!lblnRequestProcessed)
        //    {
        //        try
        //        {
        //            abusBpmRequest.icdoBpmRequest.case_instance_id = 0;
        //            abusBpmRequest.icdoBpmRequest.status_value = BpmRequestStatus.NotProcessed;
        //            abusBpmRequest.icdoBpmRequest.Update();
        //        }
        //        catch (Exception ex)
        //        {
        //            ExceptionManager.Publish(ex);
        //        }
        //    }
        //}

        ///// <summary>
        ///// This function is used to initialize the workflow for the request instance passed as an argument.
        ///// </summary>
        ///// <param name="abusBpmRequest">Request instance.</param>
        //private void SendMessageToTargetActivityInstance(busBpmRequest abusBpmRequest)
        //{
        //    bool lblnRequestProcessed = false;
        //    try
        //    {
        //        busSolBpmCaseInstance lbusBpmCaseInstance = new busSolBpmCaseInstance();
        //        if (lbusBpmCaseInstance.FindByPrimaryKey(abusBpmRequest.icdoBpmRequest.case_instance_id))
        //        {
        //            lbusBpmCaseInstance.ibusBpmCase = busBpmCase.GetBpmCase(lbusBpmCaseInstance.icdoBpmCaseInstance.case_id);
        //            if (lbusBpmCaseInstance.ibusBpmCase != null)
        //            {
        //                foreach (busBpmProcess lbusBpmProcess in lbusBpmCaseInstance.ibusBpmCase.iclbBpmProcess)
        //                {
        //                    lbusBpmProcess.iclbBpmActivity.ForEach(bpmActivity => bpmActivity.LoadRoles());
        //                }
        //            }
        //            lbusBpmCaseInstance.LoadBpmCaseInstanceParameters();
        //            lbusBpmCaseInstance.LoadBpmProcessInstances();
        //            lbusBpmCaseInstance.LoadBpmRequest();

        //            busBpmCaseMessageFlow lbusBpmCaseMessageFlow = lbusBpmCaseInstance.ibusBpmCase.iclbBpmCaseMessageFlow.Where(messageFlow => messageFlow.icdoBpmCaseMessageFlow.bpm_message_flow_id == abusBpmRequest.icdoBpmRequest.bpm_message_flow_id).FirstOrDefault();
        //            if (lbusBpmCaseMessageFlow != null)
        //            {
        //                busBpmProcessInstance lbusTargetProcessInstance = lbusBpmCaseInstance.GetProcInstWithBpmProcessId(lbusBpmCaseMessageFlow.icdoBpmCaseMessageFlow.target_bpm_process_id);
        //                if (lbusTargetProcessInstance != null)
        //                {
        //                    lbusTargetProcessInstance.ReceiveMessage(lbusBpmCaseMessageFlow);
        //                }
        //            }
        //        }
        //        abusBpmRequest.icdoBpmRequest.status_value = BpmRequestStatus.Processed;
        //        abusBpmRequest.icdoBpmRequest.Update();
        //        lblnRequestProcessed = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionManager.Publish(new Exception("An error occured sending message to activity request id: " + abusBpmRequest.icdoBpmRequest.request_id + " Error Message : " + ex));
        //    }

        //    if (!lblnRequestProcessed)
        //    {
        //        try
        //        {
        //            abusBpmRequest.icdoBpmRequest.case_instance_id = 0;
        //            abusBpmRequest.icdoBpmRequest.status_value = BpmRequestStatus.NotProcessed;
        //            abusBpmRequest.icdoBpmRequest.Update();
        //        }
        //        catch (Exception ex)
        //        {
        //            ExceptionManager.Publish(ex);
        //        }
        //    }
        //}

        ///// <summary>
        ///// This function is used to initialize the workflow for the request instance passed as an argument.
        ///// </summary>
        ///// <param name="abusBpmRequest">Request instance.</param>
        //private void ProcessRequest(busBpmRequest abusBpmRequest)
        //{
        //    bool lblnRequestProcessed = false;
        //    try
        //    {
        //        utlPassInfo.iobjPassInfo.BeginTransaction();
        //        //Check if the request is generated thru scanning/indexing or through the online initialization screen.
        //        if (abusBpmRequest.icdoBpmRequest.source_value.Equals(BpmRequestSource.ScanAndIndex))
        //        {
        //            if (abusBpmRequest.icdoBpmRequest.tracking_id > 0 &&
        //                abusBpmRequest.icdoBpmRequest.reason_value == busConstant.Communication.INCOMING_COMMUNICATION_STATUS_UNDELIVERED)
        //            {
        //                //Initiate the undelivered document map instance for person/organization
        //                InitializeUndeliveredDocumentProcessForPersonOrOrganization(abusBpmRequest);
        //                lblnRequestProcessed = true;
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrEmpty(abusBpmRequest.icdoBpmRequest.bpm_message_flow_id))
        //                {
        //                    SendMessageToTargetActivityInstanceWhenIncomingDocumentReceived(abusBpmRequest);
        //                    utlPassInfo.iobjPassInfo.Commit();
        //                    return;
        //                }
        //                if (abusBpmRequest.icdoBpmRequest.doc_type.IsNotNullOrEmpty())
        //                {
        //                    //Request generated thru scanning/indexing.
        //                    //Get the details of the event using the doc type value.
        //                    busBpmEvent lbusEvent = new busBpmEvent();

        //                    if (!lbusEvent.FindBpmEventByDocType(abusBpmRequest.icdoBpmRequest.doc_type))
        //                    {
        //                        //Document not recognized by the application.
        //                        InitializeDocumentExceptionProcess(abusBpmRequest);
        //                    }
        //                    else
        //                    {
        //                        //Get list of all the processes to which this event is attached to.                
        //                        lbusEvent.LoadBpmProcessEventXrsByProcessId(abusBpmRequest.icdoBpmRequest.process_id);
        //                        {
        //                            foreach (busBpmProcessEventXr lbusProcessEventXr in lbusEvent.iclbBpmProcessEventXr)
        //                            {
        //                                lbusProcessEventXr.LoadBpmProcess();
        //                                lbusProcessEventXr.ibusBpmProcess.LoadBpmCase();
        //                                Collection<busBpmActivityInstance> lclbBpmActivityInstance = GetActivityInstanceToResume(lbusProcessEventXr, abusBpmRequest);
        //                                if (lclbBpmActivityInstance != null && lclbBpmActivityInstance.Count > 0)
        //                                {
        //                                    foreach (busBpmActivityInstance lbusBpmActivityInstance in lclbBpmActivityInstance)
        //                                    {
        //                                        try
        //                                        {
        //                                            InitializeOrResumeWorkflow(abusBpmRequest, lbusBpmActivityInstance, lbusProcessEventXr);
        //                                        }
        //                                        catch (Exception ex)
        //                                        {
        //                                            //Not throwing the except so that remaining requests can be processed
        //                                            ExceptionManager.Publish(ex);
        //                                        }
        //                                    }
        //                                }
        //                                else if (lbusProcessEventXr.icdoBpmProcessEventXr.action_value == BpmResumeAction.InitiateNew || lbusProcessEventXr.icdoBpmProcessEventXr.action_value == BpmResumeAction.InitiateOrResume)
        //                                {
        //                                    if (lbusProcessEventXr.ibusBpmProcess.ibusBpmCase.icdoBpmCase.status_value == BpmCaseStatus.Active)
        //                                    {
        //                                        object lobjLatestVersion = DBFunction.DBExecuteScalar("entBpmCase.GetLatestVersionOfCaseByCaseId", new object[2] { lbusProcessEventXr.ibusBpmProcess.icdoBpmProcess.case_id, DateTimeExtensions.ApplicationDateTime.Date }, utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
        //                                        int lintLatestCaseId = 0;
        //                                        if (lobjLatestVersion != null)
        //                                        {
        //                                            lintLatestCaseId = (int)lobjLatestVersion;
        //                                        }
        //                                        if (lintLatestCaseId > 0 && lintLatestCaseId == lbusProcessEventXr.ibusBpmProcess.icdoBpmProcess.case_id)
        //                                        {
        //                                            abusBpmRequest.icdoBpmRequest.process_id = lbusProcessEventXr.icdoBpmProcessEventXr.process_id;
        //                                            busBpmCaseInstance lbusBpmCaseInstance = CreateWorkflow(abusBpmRequest);
        //                                            if (lbusBpmCaseInstance != null)
        //                                            {
        //                                                AddOrUpdateToProcessInstanceAttachments(lbusBpmCaseInstance, abusBpmRequest, lbusProcessEventXr);
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }

        //                            ///TODO : Pushawart: as part of framework 6.0.0.31, UpdateBPMActivityChecklist is moved to framework code in busBpmActivityInstance with changed signature
        //                            ///So whenever we test BPM checklist completion for incoming request, we need to try to call the method from framework code and remove this solution 
        //                            ///code of the method UpdateBPMActivityChecklist
        //                            UpdateBPMActivityChecklist(lbusEvent);
        //                        }
        //                    }
        //                    //Need to call this method here to tell communication module about receipt of
        //                    //turn around or undelivered document
        //                    IncomingDocumentReceived(abusBpmRequest);
        //                    lblnRequestProcessed = true;
        //                }
        //                else
        //                {
        //                    //Kick off the document exception workflow
        //                    InitializeDocumentExceptionProcess(abusBpmRequest);
        //                    lblnRequestProcessed = true;
        //                }
        //            }

        //            //Temporary code added to complete the request if the document is scanned and indexed. 
        //            //As we don't have the required configuration for processing the document requests from Filenet.
        //            //This should be removed once we are done with the configuration.
        //            if (lblnRequestProcessed)
        //            {
        //                CompleteRequest(abusBpmRequest);
        //            }
        //        }
        //        else if (abusBpmRequest.icdoBpmRequest.source_value.Equals(BpmRequestSource.Online) ||
        //            abusBpmRequest.icdoBpmRequest.source_value.Equals(BpmRequestSource.Batch))
        //        {
        //            //Online initialization.
        //            CreateWorkflow(abusBpmRequest);
        //            lblnRequestProcessed = true;
        //        }
        //        utlPassInfo.iobjPassInfo.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        utlPassInfo.iobjPassInfo.Rollback();
        //        ExceptionManager.Publish(ex);
        //    }
        //    if (!lblnRequestProcessed)
        //    {
        //        utlPassInfo.iobjPassInfo.BeginTransaction();
        //        try
        //        {
        //            abusBpmRequest.icdoBpmRequest.case_instance_id = 0;
        //            abusBpmRequest.icdoBpmRequest.status_value = BpmRequestStatus.NotProcessed;
        //            abusBpmRequest.icdoBpmRequest.Update();
        //            utlPassInfo.iobjPassInfo.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            utlPassInfo.iobjPassInfo.Rollback();
        //            ExceptionManager.Publish(ex);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Initializes or resumes the workflow based on arguments passed in
        ///// </summary>
        ///// <param name="abusBpmRequest"></param>
        ///// <param name="abusBpmActivityInstance"></param>
        ///// <param name="abusBpmProcessEventXr"></param>
        ///// <param name="ablnSkipAutoComplete"></param>
        //private void InitializeOrResumeWorkflow(busBpmRequest abusBpmRequest, busBpmActivityInstance abusBpmActivityInstance, busBpmProcessEventXr abusBpmProcessEventXr, bool ablnSkipAutoComplete = false)
        //{
        //    switch (abusBpmProcessEventXr.icdoBpmProcessEventXr.action_value)
        //    {
        //        case BpmResumeAction.InitiateNew:
        //            {
        //                //Will always create workflow process instance irrespective of whether there exists any process instance or not.
        //                if (abusBpmProcessEventXr.ibusBpmProcess.ibusBpmCase.icdoBpmCase.status_value == BpmCaseStatus.Active)
        //                {
        //                    object lobjLatestVersion = DBFunction.DBExecuteScalar("entBpmCase.GetLatestVersionOfCaseByCaseId", new object[2] { abusBpmProcessEventXr.ibusBpmProcess.icdoBpmProcess.case_id, DateTimeExtensions.ApplicationDateTime.Date }, utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
        //                    int lintLatestCaseId = 0;
        //                    if (lobjLatestVersion != null)
        //                    {
        //                        lintLatestCaseId = (int)lobjLatestVersion;
        //                    }
        //                    if (lintLatestCaseId > 0 && lintLatestCaseId == abusBpmProcessEventXr.ibusBpmProcess.icdoBpmProcess.case_id)
        //                    {
        //                        abusBpmRequest.icdoBpmRequest.process_id = abusBpmProcessEventXr.icdoBpmProcessEventXr.process_id;
        //                        busBpmCaseInstance lbusBpmCaseInstance = CreateWorkflow(abusBpmRequest);
        //                        if (lbusBpmCaseInstance != null)
        //                        {
        //                            AddOrUpdateToProcessInstanceAttachments(lbusBpmCaseInstance, abusBpmRequest, abusBpmProcessEventXr);
        //                        }
        //                    }
        //                }

        //            }
        //            break;
        //        case BpmResumeAction.InitiateOrResume:
        //            {
        //                //Will initiate a new workflow process if there is no workflow process, o.w. associates it with existing process and resumes it.
        //                if (abusBpmActivityInstance.IsNull())
        //                {
        //                    //Initiate the new process.
        //                    if (abusBpmProcessEventXr.ibusBpmProcess.ibusBpmCase.icdoBpmCase.status_value == BpmCaseStatus.Active)
        //                    {
        //                        object lobjLatestVersion = DBFunction.DBExecuteScalar("entBpmCase.GetLatestVersionOfCaseByCaseId", new object[2] { abusBpmProcessEventXr.ibusBpmProcess.icdoBpmProcess.case_id, DateTimeExtensions.ApplicationDateTime.Date }, utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
        //                        int lintLatestCaseId = 0;
        //                        if (lobjLatestVersion != null)
        //                        {
        //                            lintLatestCaseId = (int)lobjLatestVersion;
        //                        }
        //                        if (lintLatestCaseId > 0 && lintLatestCaseId == abusBpmProcessEventXr.ibusBpmProcess.icdoBpmProcess.case_id)
        //                        {
        //                            abusBpmRequest.icdoBpmRequest.process_id = abusBpmProcessEventXr.icdoBpmProcessEventXr.process_id;
        //                            busBpmCaseInstance lbusBpmCaseInstance = CreateWorkflow(abusBpmRequest);
        //                            if (lbusBpmCaseInstance != null)
        //                            {
        //                                AddOrUpdateToProcessInstanceAttachments(lbusBpmCaseInstance, abusBpmRequest, abusBpmProcessEventXr);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    //The incoming document may not contain the process id so setting the process id explicitely.
        //                    if (abusBpmRequest.icdoBpmRequest.process_id <= 0)
        //                    {
        //                        abusBpmRequest.icdoBpmRequest.process_id = abusBpmProcessEventXr.icdoBpmProcessEventXr.process_id;
        //                    }
        //                    //Resumes the existing process.
        //                    abusBpmRequest.icdoBpmRequest.case_instance_id = abusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.case_instance_id;
        //                    abusBpmRequest.icdoBpmRequest.Update();
        //                    utlPassInfo.iobjPassInfo.Commit();
        //                    utlPassInfo.iobjPassInfo.BeginTransaction();
        //                    if (ResumeActivityInstance(abusBpmActivityInstance, abusBpmProcessEventXr, abusBpmRequest, ablnSkipAutoComplete))
        //                    {
        //                        //Add or Update an entry for this event in the sgw_process_instance_attachments table.
        //                        AddOrUpdateToProcessInstanceAttachments(abusBpmActivityInstance.ibusBpmProcessInstance, abusBpmRequest.icdoBpmRequest.ecm_guid.ToString(), abusBpmRequest.icdoBpmRequest.doc_type, abusBpmProcessEventXr.ibusBpmEvent.icdoBpmEvent.document_category);
        //                    }
        //                    else
        //                    {
        //                        abusBpmActivityInstance.Resume();
        //                    }
        //                    ////Reason: Regardless of whether the Activity instance is there or not complete the request.
        //                    CompleteRequest(abusBpmRequest);
        //                }
        //            }
        //            break;
        //        case BpmResumeAction.NeverInitiateOnlyResume:
        //            {
        //                if (abusBpmActivityInstance.IsNotNull())
        //                {
        //                    //The incoming document may not contain the process id so setting the process id explicitely.
        //                    if (abusBpmRequest.icdoBpmRequest.process_id <= 0)
        //                    {
        //                        abusBpmRequest.icdoBpmRequest.process_id = abusBpmProcessEventXr.icdoBpmProcessEventXr.process_id;
        //                    }
        //                    abusBpmRequest.icdoBpmRequest.case_instance_id = abusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.case_instance_id;
        //                    abusBpmRequest.icdoBpmRequest.Update();
        //                    utlPassInfo.iobjPassInfo.Commit();

        //                    utlPassInfo.iobjPassInfo.BeginTransaction();
        //                    //Will associates it with existing process and resumes it, if no active process does nothing.                                               
        //                    if (ResumeActivityInstance(abusBpmActivityInstance, abusBpmProcessEventXr, abusBpmRequest, ablnSkipAutoComplete))
        //                    {
        //                        //Add or Update an entry for this event in the sgw_process_instance_attachments table.
        //                        AddOrUpdateToProcessInstanceAttachments(abusBpmActivityInstance.ibusBpmProcessInstance, abusBpmRequest.icdoBpmRequest.ecm_guid.ToString(), abusBpmRequest.icdoBpmRequest.doc_type, abusBpmProcessEventXr.ibusBpmEvent.icdoBpmEvent.document_category);
        //                    }
        //                    else
        //                    {
        //                        abusBpmActivityInstance.Resume();
        //                    }
        //                }
        //                //Reason: Regardless of whether the Activity instance is there or not complete the request.
        //                CompleteRequest(abusBpmRequest);
        //            }
        //            break;
        //        case BpmResumeAction.ResumeOrException:
        //            {
        //                if (abusBpmActivityInstance.IsNotNull())
        //                {
        //                    //The incoming document may not contain the process id so setting the process id explicitely.
        //                    if (abusBpmRequest.icdoBpmRequest.process_id <= 0)
        //                    {
        //                        abusBpmRequest.icdoBpmRequest.process_id = abusBpmProcessEventXr.icdoBpmProcessEventXr.process_id;
        //                    }
        //                    abusBpmRequest.icdoBpmRequest.case_instance_id = abusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.case_instance_id;
        //                    abusBpmRequest.icdoBpmRequest.Update();
        //                    utlPassInfo.iobjPassInfo.Commit();
        //                    utlPassInfo.iobjPassInfo.BeginTransaction();


        //                    //Will associates it with existing process and resumes it, if no active process does nothing.                                               
        //                    if (ResumeActivityInstance(abusBpmActivityInstance, abusBpmProcessEventXr, abusBpmRequest, ablnSkipAutoComplete))
        //                    {
        //                        //Add or Update an entry for this event in the sgw_process_instance_attachments table.
        //                        AddOrUpdateToProcessInstanceAttachments(abusBpmActivityInstance.ibusBpmProcessInstance, abusBpmRequest.icdoBpmRequest.ecm_guid.ToString(), abusBpmRequest.icdoBpmRequest.doc_type, abusBpmProcessEventXr.ibusBpmEvent.icdoBpmEvent.document_category);
        //                    }
        //                    else
        //                    {
        //                        abusBpmActivityInstance.Resume();
        //                    }
        //                    //Reason: Regardless of whether the Activity instance is there or not complete the request.
        //                    CompleteRequest(abusBpmRequest);
        //                }
        //                else
        //                {
        //                    PendRequest(abusBpmRequest);
        //                }
        //            }
        //            break;
        //    }

        //}


        ///// <summary>
        ///// Start the document exception workflow
        ///// </summary>
        ///// <param name="abusBpmRequest"></param>
        //private void InitializeUndeliveredDocumentProcessForPersonOrOrganization(busBpmRequest abusBpmRequest)
        //{
        //    //Need to call this method here to tell communication module about receipt of
        //    //turn around or undelivered document
        //    IncomingDocumentReceived(abusBpmRequest);

        //    if (abusBpmRequest.icdoBpmRequest.person_id > 0)
        //    {
        //        busBpmProcess lbusBpmProcess = busSolBpmActivityInstance.GetBpmProcess("sbpPersonAddressUndeliveredProcess ", "Person Address Undelivered Process", utlPassInfo.iobjPassInfo);

        //        if (lbusBpmProcess.icdoBpmProcess.process_id > 0)
        //        {
        //            abusBpmRequest.icdoBpmRequest.process_id = lbusBpmProcess.icdoBpmProcess.process_id;

        //            //Create the new request parameter for the TrackingId since map needs value of tracking id from request table.
        //            busBpmRequestParameter lbusBpmRequestParameter = new busBpmRequestParameter() { icdoBpmRequestParameter = new doBpmRequestParameter() };
        //            lbusBpmRequestParameter.icdoBpmRequestParameter.request_id = abusBpmRequest.icdoBpmRequest.request_id;
        //            lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_name = "TrackingId";
        //            lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_value = abusBpmRequest.icdoBpmRequest.tracking_id.ToString();
        //            lbusBpmRequestParameter.icdoBpmRequestParameter.Insert();
        //            abusBpmRequest.iclbBpmRequestParameter.Add(lbusBpmRequestParameter);

        //            busBpmCaseInstance lbusBpmCaseInstance = CreateWorkflow(abusBpmRequest);

        //            if (lbusBpmCaseInstance != null)
        //            {
        //                AddOrUpdateToProcessInstanceAttachments(lbusBpmCaseInstance.iclbBpmProcessInstance[0],
        //                                abusBpmRequest.icdoBpmRequest.ecm_guid,
        //                                abusBpmRequest.icdoBpmRequest.doc_type,
        //                                abusBpmRequest.icdoBpmRequest.doc_class);
        //            }
        //        }
        //    }
        //    else if (abusBpmRequest.icdoBpmRequest.org_id > 0)
        //    {
        //        busBpmProcess lbusBpmProcess = busSolBpmActivityInstance.GetBpmProcess("sbpOrganizationAddressUndeliveredProcess", "Organization Address Undelivered Process", utlPassInfo.iobjPassInfo);

        //        if (lbusBpmProcess.icdoBpmProcess.process_id > 0)
        //        {
        //            abusBpmRequest.icdoBpmRequest.process_id = lbusBpmProcess.icdoBpmProcess.process_id;

        //            //Create the new request parameter for the TrackingId since map needs value of tracking id from request table.
        //            busBpmRequestParameter lbusBpmRequestParameter = new busBpmRequestParameter() { icdoBpmRequestParameter = new doBpmRequestParameter() };
        //            lbusBpmRequestParameter.icdoBpmRequestParameter.request_id = abusBpmRequest.icdoBpmRequest.request_id;
        //            lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_name = "TrackingId";
        //            lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_value = abusBpmRequest.icdoBpmRequest.tracking_id.ToString();
        //            lbusBpmRequestParameter.icdoBpmRequestParameter.Insert();
        //            abusBpmRequest.iclbBpmRequestParameter.Add(lbusBpmRequestParameter);
        //            busBpmCaseInstance lbusBpmCaseInstance = CreateWorkflow(abusBpmRequest);
        //            if (lbusBpmCaseInstance != null)
        //            {
        //                AddOrUpdateToProcessInstanceAttachments(lbusBpmCaseInstance.iclbBpmProcessInstance[0],
        //                                abusBpmRequest.icdoBpmRequest.ecm_guid,
        //                                abusBpmRequest.icdoBpmRequest.doc_type,
        //                                abusBpmRequest.icdoBpmRequest.doc_class);
        //            }
        //        }
        //    }

        //}



        ///// <summary>
        ///// Start the document exception workflow
        ///// </summary>
        ///// <param name="abusBpmRequest"></param>
        //private void InitializeDocumentExceptionProcess(busBpmRequest abusBpmRequest)
        //{
        //    //Commenting out as per discussion with Pushawart.
        //    //Kick off the document exception workflow
        //    /* busRequest lbusRequest = new busRequest();
        //     acdoRequest.process_id = lbusRequest.GetProcessIDForWorkflow(BpmWorkflowConstants.DocumentException);
        //     WorkflowResult lresult = CreateWorkflow(acdoRequest);

        //     if (lresult.IsNotNull())
        //     {
        //         TurnAroundDocumentReceived(acdoRequest);
        //         //Add or Update an entry for this event in the sgw_process_instance_attachments table.
        //         AddOrUpdateToProcessInstanceAttachments(lresult.iintProcess_instance_id, acdoRequest);
        //     }*/
        //}

        ///// <summary>
        ///// Completes the Bpm Request
        ///// </summary>
        ///// <param name="abusBpmRequest"></param>
        //private void CompleteRequest(busBpmRequest abusBpmRequest)
        //{
        //    abusBpmRequest.icdoBpmRequest.status_value = BpmRequestStatus.Processed;
        //    abusBpmRequest.icdoBpmRequest.Update();
        //}

        ///// <summary>
        ///// Updates the Bpm Request to Pending 
        ///// </summary>
        ///// <param name="abusBpmRequest"></param>
        //private void PendRequest(busBpmRequest abusBpmRequest)
        //{
        //    abusBpmRequest.icdoBpmRequest.status_value = BpmRequestStatus.Pended;
        //    abusBpmRequest.icdoBpmRequest.Update();
        //}

        ///// <summary>
        ///// Returns the activity instance object to resume
        ///// </summary>
        ///// <param name="abusBpmProcessEventXr"></param>
        ///// <param name="abusBpmRequest"></param>
        ///// <returns></returns>
        //private Collection<busBpmActivityInstance> GetActivityInstanceToResume(busBpmProcessEventXr abusBpmProcessEventXr, busBpmRequest abusBpmRequest)
        //{
        //    Collection<busBpmActivityInstance> lclbBusBpmActivityInstance = null;
        //    //Assuming that incoming document has to be associated with the person or organization 
        //    //and corresponding person_id or org_id has to be added with incoming request recrod
        //    //search for activity instance is only selected if activity is selected in process event xr configuration else return null
        //    if (abusBpmProcessEventXr.icdoBpmProcessEventXr.activity_id > 0)
        //    {
        //        lclbBusBpmActivityInstance = LoadActivityInstanceWaitingForProcessEvent(abusBpmProcessEventXr, abusBpmRequest);
        //    }
        //    return lclbBusBpmActivityInstance;
        //}

        ///// <summary>
        ///// Calls  the IncomingDocumentRecevied method to notify communication module for the incoming document.
        ///// </summary>
        ///// <param name="abusBpmRequest"></param>
        //private void IncomingDocumentReceived(busBpmRequest abusBpmRequest)
        //{
        //    if (abusBpmRequest.icdoBpmRequest.tracking_id > 0 && abusBpmRequest.icdoBpmRequest.reason_value.IsNotNullOrEmpty())
        //    {
        //        new busCommunication().IncomingDocumentReceived(abusBpmRequest.icdoBpmRequest.tracking_id, abusBpmRequest.icdoBpmRequest.reason_value);
        //    }
        //}

        ///// <summary>
        ///// This methods retrieves all the documents associated to the process, pulls them from FileNet and adds them into process
        ///// instance attachments table
        ///// </summary>
        ///// <param name="abusBpmProcessInstance"></param>
        ///// <param name="acdoBpmRequest"></param>
        //private void AddDocumentsToProcessInstanceAttachments(busBpmProcessInstance abusBpmProcessInstance, doBpmRequest acdoBpmRequest)
        //{
        //    busBpmProcess lbusBpmProcess = abusBpmProcessInstance.ibusBpmProcess;
        //    //if (lbusBpmProcess.FindBpmProcess(acdoBpmRequest.process_id))
        //    {
        //        if (lbusBpmProcess.iclbBpmProcessEventXr == null || lbusBpmProcess.iclbBpmProcessEventXr.Count == 0)
        //            lbusBpmProcess.LoadBpmProcessEventXrs();

        //        if (lbusBpmProcess.iclbBpmProcessEventXr.Count > 0)
        //        {
        //            Collection<string> lcolDocTypes = new Collection<string>();
        //            //Collection<busECMDocument> lclbEcmDocuments = null;

        //            foreach (busBpmProcessEventXr lbusBpmProcessEventXr in lbusBpmProcess.iclbBpmProcessEventXr)
        //            {
        //                lbusBpmProcessEventXr.LoadBpmEvent();
        //                lcolDocTypes.Add(lbusBpmProcessEventXr.ibusBpmEvent.icdoBpmEvent.doc_type);
        //            }

        //            //if (lbusBpmProcess.icdoBpmProcess.type_value.Equals(BpmWorkflowConstants.ProcessTypePerson))
        //            //{
        //            //    lclbEcmDocuments = clsECMInterface.Instance.Search(string.Empty, acdoRequest.person_id.ToString(), lclbDocTypes);
        //            //}
        //            //else if (lbusBpmProcess.icdoProcess.type_value.Equals(BpmWorkflowConstants.ProcessTypeOrg))
        //            //{
        //            //    lclbEcmDocuments = clsECMInterface.Instance.Search(acdoRequest.org_id.ToString(), string.Empty, lclbDocTypes);
        //            //}
        //            //else
        //            //{
        //            //    lclbEcmDocuments = clsECMInterface.Instance.Search(string.Empty, string.Empty, lclbDocTypes);
        //            //}

        //            //if (lclbEcmDocuments.IsNotNull() && lclbEcmDocuments.Count > 0)
        //            //{
        //            //    foreach (busECMDocument lDocument in lclbEcmDocuments)
        //            //    {
        //            //        AddOrUpdateToProcessInstanceAttachments(aintProcessInstanceId, lDocument.icdoEcmDocument.document_id, lDocument.icdoEcmDocument.document_type);
        //            //    }
        //            //}
        //        }
        //    }
        //}

        ///// <summary>
        ///// Add Or Update the Process Instance Attachment
        ///// </summary>
        ///// <param name="abusBpmCaseInstance">BpmCaseInstance Object</param>
        ///// <param name="abusBpmRequest">BpmRequest Object</param>
        ///// <param name="abusBpmProcessEventXr">busBpmProcessEventXr Object</param>
        //private void AddOrUpdateToProcessInstanceAttachments(busBpmCaseInstance abusBpmCaseInstance, busBpmRequest abusBpmRequest, busBpmProcessEventXr abusBpmProcessEventXr)
        //{
        //    busBpmProcessInstance lbusBpmProcessInstance = abusBpmCaseInstance.iclbBpmProcessInstance.Where(processInstance => processInstance.icdoBpmProcessInstance.process_id == abusBpmProcessEventXr.icdoBpmProcessEventXr.process_id).FirstOrDefault();
        //    if (lbusBpmProcessInstance != null)
        //    {
        //        if (abusBpmProcessEventXr.ibusBpmEvent == null)
        //        {
        //            abusBpmProcessEventXr.LoadBpmEvent();
        //        }
        //        AddOrUpdateToProcessInstanceAttachments(lbusBpmProcessInstance, Convert.ToString(abusBpmRequest.icdoBpmRequest.ecm_guid), abusBpmRequest.icdoBpmRequest.doc_type, abusBpmProcessEventXr.ibusBpmEvent.icdoBpmEvent.document_category);
        //    }
        //}

        ///// <summary>
        ///// Adds the new ECM document entry into the process instance attachments table
        ///// </summary>
        //private void AddOrUpdateToProcessInstanceAttachments(busBpmProcessInstance abusBpmProcessInstance, string astrGuid, string astrDocType, string astrDocClass)
        //{
        //    busBpmProcessInstanceAttachments lbusBpmProcessInstanceAttachment = new busBpmProcessInstanceAttachments();
        //    lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments = new doBpmPrcsInstAttachments();
        //    lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.bpm_process_instance_id = abusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id;
        //    lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.doc_type = astrDocType;
        //    lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.ecm_guid = new Guid(astrGuid).ToString();
        //    lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.doc_class = astrDocClass;

        //    if (!lbusBpmProcessInstanceAttachment.FindBpmProcessInstanceAttachments(astrDocType, abusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id))
        //    {
        //        lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.Insert();
        //    }
        //    else
        //    {
        //        lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.Update();
        //    }
        //}

        ///// <summary>
        ///// Resumes the activity instance for the process event information.
        /////</remarks>        
        //private bool ResumeActivityInstance(busBpmActivityInstance abusBpmActivityInstance, busBpmProcessEventXr abusBpmProcessEventXr, busBpmRequest abusBpmRequest, bool ablnSkipAutoComplete = false)
        //{
        //    ArrayList larrList = new ArrayList();

        //    if (abusBpmActivityInstance.IsNotNull())
        //    {
        //        if (abusBpmActivityInstance.ibusBpmActivity is busBpmUserTask)
        //        {
        //            //Check if the activity instance is in suspended mode if so, then get the resume action value and accordingly resume the process.
        //            if (abusBpmActivityInstance.icdoBpmActivityInstance.status_value.Equals(BpmActivityInstanceStatus.Suspended))
        //            {
        //                if (abusBpmActivityInstance.icdoBpmActivityInstance.resume_action_value.Equals(BpmCommon.ResumeActionAnyDocument))
        //                {
        //                    abusBpmActivityInstance.UpdateBpmActivityInstanceByStatus(BpmActivityInstanceStatus.Resumed);
        //                    return true;
        //                }
        //                else if (abusBpmActivityInstance.icdoBpmActivityInstance.resume_action_value.Equals(BpmCommon.ResumeActionAllDocument))
        //                {
        //                    ////Resume the process only if all the documents associated with the process are received.
        //                    //TODO: 
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// This function is used to create the workflow for the request instance passed as an argument.
        ///// </summary>
        ///// <param name="abusBpmRequest">Request instance.</param>
        //private busBpmCaseInstance CreateWorkflow(busBpmRequest abusBpmRequest)
        //{
        //    bool lblnCreateInstance = true;
        //    if (abusBpmRequest.iblnCheckForExistingInstance)
        //    {
        //        ArrayList larrErrorMessages = abusBpmRequest.CheckForActiveInstances();
        //        if (larrErrorMessages.Count > 0)
        //        {
        //            lblnCreateInstance = false;
        //            SetInProgressCaseInstanceId(abusBpmRequest);
        //        }
        //    }
        //    if (lblnCreateInstance)
        //    {
        //        abusBpmRequest.LoadBpmRequestParameters();
        //        return abusBpmRequest.CreateBpmCaseInstance();
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Set the Process Request Case Instance Id
        ///// </summary>
        ///// <param name="abusBpmRequest"></param>
        //private void SetInProgressCaseInstanceId(busBpmRequest abusBpmRequest)
        //{
        //    string lstrQuery = null;
        //    object[] larrParams = null;
        //    if (abusBpmRequest.icdoBpmRequest.reference_id > 0)
        //    {
        //        lstrQuery = "entBpmProcessInstance.GetInProcessCaseInstanceIdByProcessIdPersonIdOrgIdReferenceId";
        //        larrParams = new object[]{
        //                                    abusBpmRequest.icdoBpmRequest.process_id,
        //                                    abusBpmRequest.icdoBpmRequest.person_id,
        //                                    abusBpmRequest.icdoBpmRequest.org_id,
        //                                    abusBpmRequest.icdoBpmRequest.reference_id
        //                                 };
        //    }
        //    else
        //    {
        //        lstrQuery = "entBpmProcessInstance.GetInProcessCaseInstanceIdByProcessIdPersonIdOrgId";
        //        larrParams = new object[]{
        //                                    abusBpmRequest.icdoBpmRequest.process_id,
        //                                    abusBpmRequest.icdoBpmRequest.person_id,
        //                                    abusBpmRequest.icdoBpmRequest.org_id,
        //                                 };
        //    }
        //    DataTable ldtCaseInstanceId = DBFunction.DBSelect(lstrQuery, larrParams, utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
        //    if (ldtCaseInstanceId != null && ldtCaseInstanceId.Rows.Count > 0)
        //    {
        //        abusBpmRequest.icdoBpmRequest.case_instance_id = (int)ldtCaseInstanceId.Rows[0][0];
        //    }
        //}

        /// <summary>
        /// This function returns the count of the document from process instance attachment table based on passed in guid
        /// </summary>
        /// <param name="astrECMGuid"></param>
        /// <returns></returns>
        private int GetDocumentAttachmentCount(string astrECMGuid)
        {
            //TODO:
            return 0;
        }

        ///// <summary>
        ///// This function is used for initializing timers for create workflow and resume workflow process.
        ///// </summary>
        //private bool InitializeTimers()
        //{
        //    string lstrServerName = null;
        //    try
        //    {
        //        double ldecWorkflowInstancePollingInterval = 600; //Default 
        //        double ldecResumeInstancePollingInterval = 6000; //Default           
        //        double ldecTimerActivitiesTriggerPoolingInterval = 600; // default
        //        double ldecEscalationsCheckPoolingInterval = 600; // default

        //        ldecWorkflowInstancePollingInterval = Convert.ToDouble(ConfigurationManager.AppSettings[WORKFLOW_INSTANCE_POLLING_INTERVAL]);
        //        ldecResumeInstancePollingInterval = Convert.ToDouble(ConfigurationManager.AppSettings[RESUME_INSTANCE_POLLING_INTERVAL]);
        //        ldecTimerActivitiesTriggerPoolingInterval = Convert.ToDouble(ConfigurationManager.AppSettings[TIMER_ACTIVITY_TRIGGER_INTERVAL]);
        //        ldecEscalationsCheckPoolingInterval = Convert.ToDouble(ConfigurationManager.AppSettings[ESCALATIONS_CHECK_INTERVAL]);

        //        //Initializing the Request Instance Timer
        //        itmrRequestInstanceTimer = new System.Timers.Timer(ldecWorkflowInstancePollingInterval);
        //        itmrRequestInstanceTimer.Elapsed += new System.Timers.ElapsedEventHandler(itmrRequestInstanceTimer_Elapsed);

        //        itmrRequestInstanceTimer.AutoReset = true;
        //        itmrRequestInstanceTimer.Enabled = true;
        //        itmrRequestInstanceTimer.Start();

        //        //Initializing the Resume Instance Timer
        //        itmrResumeWorkflowInstanceTimer = new System.Timers.Timer(ldecResumeInstancePollingInterval);
        //        itmrResumeWorkflowInstanceTimer.Elapsed += new System.Timers.ElapsedEventHandler(itmrResumeWorkflowInstanceTimer_Elapsed);

        //        itmrResumeWorkflowInstanceTimer.AutoReset = true;
        //        itmrResumeWorkflowInstanceTimer.Enabled = true;
        //        itmrResumeWorkflowInstanceTimer.Start();

        //        //Initializing the timer to handled timer activity trigger
        //        itmrTimerEventsTimer = new System.Timers.Timer(ldecTimerActivitiesTriggerPoolingInterval);
        //        itmrTimerEventsTimer.Elapsed += new System.Timers.ElapsedEventHandler(itmrTimerEventsTimer_Elapsed);

        //        itmrTimerEventsTimer.AutoReset = false;
        //        itmrTimerEventsTimer.Enabled = true;
        //        itmrTimerEventsTimer.Start();

        //        //Initializing the timer to handled escalations
        //        itmrEscalationsCheckTimer = new System.Timers.Timer(ldecEscalationsCheckPoolingInterval);
        //        itmrEscalationsCheckTimer.Elapsed += new System.Timers.ElapsedEventHandler(itmrEscalationsCheckTimer_Elapsed);

        //        itmrEscalationsCheckTimer.AutoReset = false;
        //        itmrEscalationsCheckTimer.Enabled = true;
        //        itmrEscalationsCheckTimer.Start();

        //        //Initializing the timer to handled Process escalations
        //        itmrProcessEscalationsCheckTimer = new System.Timers.Timer(ldecEscalationsCheckPoolingInterval);
        //        itmrProcessEscalationsCheckTimer.Elapsed += new System.Timers.ElapsedEventHandler(itmrProcessEscalationsCheckTimer_Elapsed);

        //        itmrProcessEscalationsCheckTimer.AutoReset = false;
        //        itmrProcessEscalationsCheckTimer.Enabled = true;
        //        itmrProcessEscalationsCheckTimer.Start();

        //        //Initializing the handler to Process Items From BPM Queue Table
        //        _ibusBpmQueueRequestHandler = new busBpmQueueRequestHandler(600);


        //        //If needs support of Multi Server BPM then un-comment following code and Pass the respective Server name (Framework Version 6.0.0.26.0)
        //        _ibusBpmQueueRequestHandler = new busBpmQueueRequestHandler(600, 10, lstrServerName, "rahul.mane");
        //        _ibusBpmQueueRequestHandler.StartProcessing();

        //        lblStatus.Text = "Service started";
        //        lblStatus.Refresh();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionManager.Publish(ex);
        //        MessageBox.Show("Error while initiliazinig timers. Eroor details:" + ex.Message);
        //        return false;
        //    }

        //    ///Commented below code because it is unreachable code. 
        //    ///Commented by vishal kedar
        //    //try
        //    //{
        //    //    _ibusBpmTaskRequestHandler = new busBpmTaskRequestHandler(BpmCommon.BPMNQueue);
        //    //    _ibusBpmTaskRequestHandler.iintMaximumTaskCount = 10;
        //    //    _ibusBpmTaskRequestHandler.StartProcessing();
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    ExceptionManager.Publish(ex);
        //    //    MessageBox.Show("Following Error occured while instantiating BPMN handler : " + ex.Message);
        //    //}
        //}

        ///// <summary>
        ///// Stops the Timers
        ///// </summary>
        //private void StopTimers()
        //{
        //    if (itmrRequestInstanceTimer != null)
        //    {
        //        itmrRequestInstanceTimer.AutoReset = false;
        //        itmrRequestInstanceTimer.Enabled = false;
        //        itmrRequestInstanceTimer.Stop();
        //        itmrRequestInstanceTimer.Dispose();
        //    }

        //    if (itmrResumeWorkflowInstanceTimer != null)
        //    {
        //        itmrResumeWorkflowInstanceTimer.AutoReset = false;
        //        itmrResumeWorkflowInstanceTimer.Enabled = false;
        //        itmrResumeWorkflowInstanceTimer.Stop();
        //        itmrResumeWorkflowInstanceTimer.Dispose();
        //    }

        //    if (itmrTimerEventsTimer != null)
        //    {
        //        itmrTimerEventsTimer.AutoReset = false;
        //        itmrTimerEventsTimer.Enabled = false;
        //        itmrTimerEventsTimer.Stop();
        //        itmrTimerEventsTimer.Dispose();
        //    }

        //    if (itmrEscalationsCheckTimer != null)
        //    {
        //        itmrEscalationsCheckTimer.AutoReset = false;
        //        itmrEscalationsCheckTimer.Enabled = false;
        //        itmrEscalationsCheckTimer.Stop();
        //        itmrEscalationsCheckTimer.Dispose();
        //    }

        //    if (itmrProcessEscalationsCheckTimer != null)
        //    {
        //        itmrProcessEscalationsCheckTimer.AutoReset = false;
        //        itmrProcessEscalationsCheckTimer.Enabled = false;
        //        itmrProcessEscalationsCheckTimer.Stop();
        //        itmrProcessEscalationsCheckTimer.Dispose();
        //    }

        //    if (_ibusBpmQueueRequestHandler != null)
        //    {
        //        _ibusBpmQueueRequestHandler.StopProcessing();
        //        _ibusBpmQueueRequestHandler.Dispose();
        //    }

        //    //if (_ibusBpmTaskRequestHandler != null)
        //    //{
        //    //    _ibusBpmTaskRequestHandler.StopProcessing();
        //    //}
        //}

        ///// <summary>
        ///// Used to running activity instance for given process event and person combination.
        ///// </summary>
        //private Collection<busBpmActivityInstance> LoadActivityInstanceWaitingForProcessEvent(busBpmProcessEventXr abusBpmProcessEventXr, busBpmRequest abusBpmRequest)
        //{
        //    Collection<busBpmActivityInstance> lclbBpmActivityInstance = new Collection<busBpmActivityInstance>();
        //    if (abusBpmRequest.icdoBpmRequest.person_id > 0 || abusBpmRequest.icdoBpmRequest.org_id > 0 || abusBpmRequest.icdoBpmRequest.reference_id > 0)
        //    {
        //        string lstrQuery = "";
        //        object[] larrObjValues = null;
        //        if (abusBpmRequest.icdoBpmRequest.person_id > 0)
        //        {
        //            lstrQuery = "entBpmActivityInstance.LoadActivityInstanceByProcessEventAndPerson";
        //            larrObjValues = new object[4] { abusBpmRequest.icdoBpmRequest.doc_type, abusBpmRequest.icdoBpmRequest.person_id, abusBpmProcessEventXr.icdoBpmProcessEventXr.process_id, abusBpmProcessEventXr.icdoBpmProcessEventXr.activity_id };
        //        }
        //        else if (abusBpmRequest.icdoBpmRequest.org_id > 0)
        //        {
        //            lstrQuery = "entBpmActivityInstance.LoadActivityInstanceByProcessEventAndOrg";
        //            larrObjValues = new object[4] { abusBpmRequest.icdoBpmRequest.doc_type, abusBpmRequest.icdoBpmRequest.org_id, abusBpmProcessEventXr.icdoBpmProcessEventXr.process_id, abusBpmProcessEventXr.icdoBpmProcessEventXr.activity_id };
        //        }
        //        else if (abusBpmRequest.icdoBpmRequest.reference_id > 0)
        //        {
        //            lstrQuery = "entBpmActivityInstance.LoadActivityInstanceByProcessEventAndReferenceId";
        //            larrObjValues = new object[4] { abusBpmRequest.icdoBpmRequest.doc_type, abusBpmRequest.icdoBpmRequest.reference_id, abusBpmProcessEventXr.icdoBpmProcessEventXr.process_id, abusBpmProcessEventXr.icdoBpmProcessEventXr.activity_id };
        //        }

        //        DataTable ldtbActivityInstance = busBase.Select(lstrQuery, larrObjValues);
        //        foreach (DataRow ldtRow in ldtbActivityInstance.Rows)
        //        {
        //            busBpmCaseInstance lbusBpmCaseInstance = new busBpmCaseInstance();
        //            lclbBpmActivityInstance.Add(lbusBpmCaseInstance.LoadWithActivityInst((int)ldtRow["ACTIVITY_INSTANCE_ID"]));
        //        }
        //        return lclbBpmActivityInstance;
        //    }
        //    return null;
        //}

        /// <summary>
        /// Initializes the Cache
        /// </summary>
        /// <returns></returns>
        private bool InitializeCache()
        {
            try
            {
                LoadMetaDataCache();
                LoadDBCache();
                ExecuteRules();
                
                CacheBpmCase();
                CorBookmarkHelper.CacheTemplateStdBookmarks();
                (new busNeoBase()).InitializeExtendedObjects();
                NeoSpinServer.InitializeClassMapper();
                NeoSpinServer.StartThreadToRefreshDBSettings();
                utlCacheRefreshHelper.StartThreadToRefreshDBSettings();
                return true;
            }
            catch (Exception er)
            {
                MessageBox.Show("Error occurred in initializing BPM Engine " + er.Message + "\n" + er.StackTrace.ToString());
                return false;
            }
        }

        /// <summary>
        /// Load MetaData Cache
        /// </summary>
        private void LoadMetaDataCache()
        {
            lblStatus.Text = "Loading XML Cache...";
            Application.DoEvents();
            srvMetaDataCache.LoadXMLCache();
            lblStatus.Text = "XML Cache loaded successfully.";
            Application.DoEvents();
        }

        /// <summary>
        /// Load DB Cache
        /// </summary>
        private void LoadDBCache()
        {
            lblStatus.Text = "Loading DB Cache...";
            Application.DoEvents();
            bool lblnSuccess = srvDBCache.LoadCacheInfo();
            if (!lblnSuccess)
            {
                MessageBox.Show(srvDBCache.istrResult);
                Close();
            }
            else
            {
                lblStatus.Text = "DB Cache loaded successfully.";
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Execute the Business Rules 
        /// </summary>
        private void ExecuteRules()
        {
            lblStatus.Text = "Executing rules...";
            Application.DoEvents();
            utlPassInfo.iobjPassInfo = new utlPassInfo();
            ParsingResult lobjResult = RulesEngine.LoadRulesAndExpressions(utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
            if (lobjResult.ilstErrors.Count > 0)
            {
                StringBuilder lstrRuleErrors = new StringBuilder();
                lstrRuleErrors.AppendLine("Rules Errors: ");
                foreach (utlRuleMessage lobjMessage in lobjResult.ilstErrors)
                {
                    lstrRuleErrors.AppendLine(string.Format("Object ID: {0}; Rule: {1}; Message: {2}",
                        lobjMessage.istrObjectID, lobjMessage.istrRuleID, lobjMessage.istrMessage));
                }
                MessageBox.Show(lstrRuleErrors.ToString());
            }
            else
            {
                lblStatus.Text = "Ready";
                Application.DoEvents();
            }
        }


        private void InitServiceHosts()
        {
            System.ServiceModel.Channels.Binding lntbBinding = ServiceHelper.GetNetTcpBinding(true);
            string lstrBaseUrl = ConfigurationManager.AppSettings["BPMServerUrl"];
            if (!string.IsNullOrWhiteSpace(lstrBaseUrl))
            {
                this._icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvCommon),
                    string.Format(lstrBaseUrl, "srvCommon"), lntbBinding));
            }
        }

        private void StopServiceHosts()
        {
            foreach (ServiceHost lobjServiceHost in _icolServiceHosts)
            {
                if (lobjServiceHost.State == CommunicationState.Opened)
                    lobjServiceHost.Close();
            }
        }
        ///// <summary>
        ///// This function is used for loading un-processed requests from request table.
        ///// </summary>
        //private void LoadUnprocessedRequests()
        //{
        //    if (iclbUnprocessedRequests != null)
        //    {
        //        iclbUnprocessedRequests.Clear();
        //    }
        //    try
        //    {
        //        if (utlPassInfo.iobjPassInfo.iconFramework.State != ConnectionState.Open)
        //        {
        //            utlPassInfo.iobjPassInfo.iconFramework.Open();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Logging exception if connection is closed.
        //        Exception lEx = new Exception("Error occurred while opening the connection ", ex);
        //        ExceptionManager.Publish(lEx);
        //        return;
        //    }
        //    string lstrQuery = "entBpmRequest.GetUnprocessedRequestsToExecuteOnCurrentDate";
        //    ArrayList larrParameters = new ArrayList();
        //    if (NeoBaseApplicationSettings.Instance.BpmEngineMode != null && NeoBaseApplicationSettings.Instance.BpmEngineMode.ToLower() == "debug")
        //    {
        //        lstrQuery = "entBpmRequest.GetUnprocessedRequestsToExecuteOnCurrentDateInDebugMode";
        //        larrParameters.Add(Environment.MachineName);
        //    }
        //    //Search the SGW_REQUEST table for records with the status 'UNPC' and whose start date is 'NULL' or 'Today's Date',
        //    //and fill up the collection ldtbWfUnprocessedRequests with unprocessed request objects.            
        //    DataTable ldtbWfUnprocessedRequests = busBase.Select(lstrQuery,
        //       larrParameters.ToArray());

        //    //Loading the Collection
        //    if (ldtbWfUnprocessedRequests != null)
        //    {
        //        busBase lbusbpmReq = new busBase();
        //        iclbUnprocessedRequests = lbusbpmReq.GetCollection<busSolBpmRequest>(ldtbWfUnprocessedRequests, "icdoBpmRequest");
        //    }
        //}

        ///// <summary>
        ///// This function is used for loading elapsed timer activities from Timer Activity Instance Details table.
        ///// </summary>
        //private void LoadElapsedTimerActivities()
        //{
        //    if (iclbElapsedTimerActivityInstances != null)
        //    {
        //        iclbElapsedTimerActivityInstances.Clear();
        //    }
        //    try
        //    {
        //        if (utlPassInfo.iobjPassInfo.iconFramework.State != ConnectionState.Open)
        //        {
        //            utlPassInfo.iobjPassInfo.iconFramework.Open();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Logging exception if connection is closed.
        //        Exception lEx = new Exception("Error occurred while opening the connection ", ex);
        //        ExceptionManager.Publish(lEx);
        //        return;
        //    }

        //    DataTable ldtbElapsedTimerActivities = busBase.Select("entBpmTimerActivityInstanceDetails.GetElapsedTimers", new object[1] { busGlobalFunctions.GetSystemDate() });

        //    //Loading the Collection
        //    if (ldtbElapsedTimerActivities != null)
        //    {
        //        busBase lbusbpmReq = new busBase();
        //        iclbElapsedTimerActivityInstances = lbusbpmReq.GetCollection<busBpmTimerActivityInstanceDetails>(ldtbElapsedTimerActivities, "icdoBpmTimerActivityInstanceDetails");
        //    }
        //}

        ///// <summary>
        ///// Load the Available Escalation Instance
        ///// </summary>
        //private void LoadEscalations()
        //{
        //    if (iclbEscalationInstances != null)
        //    {
        //        iclbEscalationInstances.Clear();
        //    }
        //    try
        //    {
        //        if (utlPassInfo.iobjPassInfo.iconFramework.State != ConnectionState.Open)
        //        {
        //            utlPassInfo.iobjPassInfo.iconFramework.Open();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Logging exception if connection is closed.
        //        Exception lEx = new Exception("Error occurred while opening the connection ", ex);
        //        ExceptionManager.Publish(lEx);
        //    }

        //    DataTable ldtbEscalationsToSend = busBase.Select("entBpmEscalationInstance.GetEscalationsToSend", new object[] { });

        //    //Loading the Collection
        //    if (ldtbEscalationsToSend != null)
        //    {
        //        busBase lbusbpmReq = new busBase();
        //        iclbEscalationInstances = lbusbpmReq.GetCollection<busSolBpmEscalationInstance>(ldtbEscalationsToSend, "icdoBpmEscalationInstance");
        //    }
        //}

        ///// <summary>
        ///// Loads the Process Escalations
        ///// </summary>
        //private void LoadProcessEscalations()
        //{
        //    if (iclbProcessEscalationInstances != null)
        //    {
        //        iclbProcessEscalationInstances.Clear();
        //    }
        //    try
        //    {
        //        if (utlPassInfo.iobjPassInfo.iconFramework.State != ConnectionState.Open)
        //        {
        //            utlPassInfo.iobjPassInfo.iconFramework.Open();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Logging exception if connection is closed.
        //        Exception lEx = new Exception("Error occurred while opening the connection ", ex);
        //        ExceptionManager.Publish(lEx);
        //    }

        //    DataTable ldtbEscalationsToSend = busBase.Select("entBpmProcessEscalationInstance.GetEscalationsToSent", new object[1] { busGlobalFunctions.GetSystemDate() });

        //    //Loading the Collection
        //    if (ldtbEscalationsToSend != null)
        //    {
        //        busBase lbusbpmReq = new busBase();
        //        iclbProcessEscalationInstances = lbusbpmReq.GetCollection<busSolBpmProcessEscalationInstance>(ldtbEscalationsToSend, "icdoBpmProcessEscalationInstance");
        //    }
        //}
        //#endregion

        //#region Update BPMActivityChecklist

        ///// <summary>
        ///// loading all BpmActivityInstanceChecklists Associated To Event
        ///// Iterating through each activity Insance if there is any InProcess/Suspended/Resumed 
        ///// Iterating through records and checking if required_flag is "Y"
        ///// Setting those records completed_ind flag="Y", Setting those records completed_date with Today's Date
        ///// </summary>
        ///// <param name="abusBpmEvent">object of busBpmEvent</param>
        //private void UpdateBPMActivityChecklist(busBpmEvent abusBpmEvent)
        //{
        //    DataTable ldtbUpdateCheckList = busBase.Select("entFramework.GetRecordsReadyForUpdateCheckList", new object[1] { abusBpmEvent.icdoBpmEvent.bpm_event_id });
        //    if (ldtbUpdateCheckList != null && ldtbUpdateCheckList.Rows.Count > 0)
        //    {
        //        foreach (DataRow ldtrRow in ldtbUpdateCheckList.Rows)
        //        {
        //            if (!string.IsNullOrEmpty(Convert.ToString(ldtrRow["ACTIVITY_INST_CHECKLIST_ID"])))
        //            {
        //                int lintActivtyInstCheckListId = Convert.ToInt32(ldtrRow["ACTIVITY_INST_CHECKLIST_ID"]);
        //                busBpmActivityInstanceChecklist lbusBpmActivityInstanceChecklist = new busBpmActivityInstanceChecklist { icdoBpmActivityInstanceChecklist = new doBpmActyInstChecklist() };
        //                if (lintActivtyInstCheckListId > 0)
        //                {
        //                    if (lbusBpmActivityInstanceChecklist.FindByPrimaryKey(lintActivtyInstCheckListId))
        //                    {
        //                        lbusBpmActivityInstanceChecklist.icdoBpmActivityInstanceChecklist.completed_ind = "Y";
        //                        lbusBpmActivityInstanceChecklist.icdoBpmActivityInstanceChecklist.completed_date = busGlobalFunctions.GetSystemDate();
        //                        lbusBpmActivityInstanceChecklist.icdoBpmActivityInstanceChecklist.Update();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion UpdateBPMActivityChecklist
    }
}
