using System;
using System.Collections;
using System.Data;
using System.Text;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Linq;
using MPIPHP.Common;
using Sagitec.CorBuilder;
using MPIPHPJobService;
using System.Collections.ObjectModel;
using MPIPHP.MPIPHPJobService.Core;
using Sagitec.ExceptionPub;
using Microsoft.Reporting.WinForms;
using System.IO;
using MPIPHPJobService.StepHandlerLogic;

namespace MPIPHP.MPIPHPJobService
{
    /// <summary>
    /// Summary description for BatchHandler.
    /// </summary>
    public class BatchHandler : JobDetailHandler
    {
        protected string StepName { get; set; }
        protected int Count { get; set; }
        protected int InternalCount { get; set; }
        protected int Limit { get; set; }
        protected int MaxCount { get; set; }
        //protected ReportDocument RptBatch { get; set;}

        protected int TotalRecords { get; set; }
        protected int TotalSuccess { get; set; }
        protected int TotalError { get; set; }
        protected string istrOutBoundError = string.Empty;
     

        protected int CurrentCycleNo { get; set; }
        protected busSystemManagement iobjSystemManagement { get; set; }
        //public static CorBuilder iobjCorBuilder;
        public cdoBatchSchedule iobjBatchSchedule;

        internal BatchHandler(busJobHeader aobjJobHeader, string astrWorkerName)
            : base(aobjJobHeader, astrWorkerName)
        {
            // Initialize the system management object as well since this will be needed by all handlers.
            iobjSystemManagement = new busSystemManagement();
            iobjSystemManagement.FindSystemManagement();
            CurrentCycleNo = iobjSystemManagement.icdoSystemManagement.current_cycle_no;
        }

        protected override void Run()
        {
            TraceIn();
            TraceLine("Enter Run()", TraceLevel.Debug);

            // Call the base class method to setup the UtlpassInfo which will be used later for executing any CRUD operations by the framework.
            SetUtlPassInfo("Job Service");

            StartTime = DateTime.Now;
            TraceLine("Started at: " + StartTime + ".", TraceLevel.Info);

            // notify work has started.
            FireWorkStarted(this, EventArgs.Empty);

            int lintReturnCode = -1;

            StartJob();

            try
            {
                // Sets the current job schedule step name to the property
                if (lobjJobHeader.ibusCurrentJobDetail != null)
                {
                    StepName = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule.step_name;
                    PostInfoMessage("Executing Job...");

                    BeginTransactionByCheckingFlag();

                    // actually execute the job
                    lintReturnCode = ExecuteJob();

                    if (lintReturnCode == 0) // Success
                    {
                        CommitTransactionByCheckingFlag();
                    }
                    else // Failure
                    {
                        RollbackTransactionByCheckingFlag();
                    }

                }
                else
                {
                    //should not come in this condition since scheduler shouldn't pickup job schedules with no steps
                    lintReturnCode = 0;
                }
            }
            catch (Exception ex)
            {
                StringBuilder lstrbException = new StringBuilder();
                lstrbException.Append("Exception Occurred: Starting Rollback Operation if any : " + ex.ToString() + "\n");
                PostErrorMessage(lstrbException.ToString());
                RollbackTransactionByCheckingFlag();
            }
            finally
            {
                if (lintReturnCode == 0)
                {
                    // Set the status of the current job to Processed Successfully
                    CompleteJobSuccessfully(lintReturnCode);
                    SendBatchStatusNotification(true, 0);
                }
                else if (lintReturnCode == -1)
                {
                    // Set the status of the current job to Processed with Errors.
                    CompleteJobWithErrors(lintReturnCode);
                    SendBatchStatusNotification(true, -1);
                }
                else if (lintReturnCode == -2)
                {
                    CompleteJobForUserCancellation(lintReturnCode);
                }

                CompleteTime = DateTime.Now;
                PostInfoMessage("Job Completed at : " + CompleteTime);
                // notify work has completed.
                FireWorkCompleted(this, EventArgs.Empty);
            }
        } // Run

        private void SendBatchStatusNotification(bool ablnHighPriority, int aintReturnCode)
        {
            //Send Email only if any error occurred for Real Time Batch Service.
            //Send Email all time from Nightly Batch
            try
            {
                //Load the Process Log Data
                string lstrMailFrom = iobjSystemManagement.icdoSystemManagement.email_notification;
                lstrMailFrom = HelperUtil.GetData1ByCodeValue(52, busConstant.EMAIL_NOTIFICATION);
                lobjJobHeader.LoadJobSchedule();

                if (lobjJobHeader.ibusJobSchedule.icdoJobSchedule.batch_error_email_recipient_groups.IsNullOrEmpty() && lobjJobHeader.ibusJobSchedule.icdoJobSchedule.batch_status_email_recipient_groups.IsNullOrEmpty())
                    return;
                
                string lstrSubject = String.Empty;
                string lstrBody = String.Empty;
                

                if (aintReturnCode == -1)
                {
                    lstrSubject = string.Format("Error Occurred ({0}): {1}", iobjSystemManagement.icdoSystemManagement.region_description, lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule.step_name);
                    lstrBody = lstrSubject;

                    busGlobalFunctions.SendMail(lstrMailFrom, lobjJobHeader.ibusJobSchedule.icdoJobSchedule.batch_error_email_recipient_groups, lstrSubject,
                          lstrBody, ablnHighPriority, true);
                }
                else if (aintReturnCode == 0)
                {
                    lstrSubject = string.Format("Successfully Executed ({0}): {1}", iobjSystemManagement.icdoSystemManagement.region_description, lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule.step_name);
                    lstrBody = lstrSubject;
                    busGlobalFunctions.SendMail(lstrMailFrom, lobjJobHeader.ibusJobSchedule.icdoJobSchedule.batch_status_email_recipient_groups, lstrSubject,
                          lstrBody, ablnHighPriority, true);
                }

            }

            catch (Exception _exc)
            {
                ExceptionManager.Publish(_exc);
            }
        }



        protected void UpdateProcessLog(string astrMessage, string astrMessageType, string astrStepName)
        {
            try
            {
                iobjPassInfoLog.BeginTransaction();
                try
                {
                    // Assigns the current job schedule step name when the passed parameter step name is empty
                    if (HelperUtil.IsNull(astrStepName) == string.Empty) astrStepName = StepName;

                    DBFunction.StoreProcessLog(CurrentCycleNo, astrStepName, astrMessageType, astrMessage, lobjJobHeader.BatchUserID,
                        iobjPassInfoLog.iconFramework, iobjPassInfoLog.itrnFramework);
                    iobjPassInfoLog.Commit();
                }
                catch (Exception ex)
                {
                    iobjPassInfoLog.Rollback();
                }

            }
            catch (Exception ex)
            {
                try
                {
                    ExceptionManager.Publish(ex);
                }
                catch
                {
                }
            }
        }

        // The executeJob method should be implmented by the handlers that are inherting from this
        // class, since every step would have it's own logic, I am forcing them to implement the same.
        protected int ExecuteJob()
        
         {
            int lintRetVal = -9;
            string strJobParameterValue = string.Empty;
            busBatchHandler lbusBatchHandler = null;
            busNotificationBatch lbusNotificationBatch;
            busBenefitApplicationBatch lbusBenefitApplicationBatch;
            busPersonNotificationBatch lbusPersonNotificationBatch;
            busIAPHardshipPaybackBatch lbusIAPHardshipPaybackBatch;
            busDeathNotification lbusDeathNotification;
            busMainBase lbusMainBase = new busMainBase();
            //FM upgrade: 6.0.6.2 changes - busProcessFiles should be replaced with busProcessOutboundFile for outbound related method and processing
            //busProcessFiles lbusProcessFiles;
            busProcessOutboundFile lbusProcessFiles;
            busBenefitAdjustmentBatch lbusBenefitAdjustmentBatch;
            busReEvaluationOfMinimumDistributionBatch lbusReEvaluationOfMinimumDistributionBatch;
            busActiveRetireeIncreaseBatch lbusActiveRetireeIncrease;
            busIAPPaymentNotificationProcess lbusIAPPaymentNotificationProcess;
            busVerificationOfHoursBatch lbusVerificationOfHoursBatch;
            busIAPPaymentAdjustmentBatch lbusIAPPaymentAdjustmentBatch;
            busReemployed lbusReemployed;
            busACHStatusUpdateBatch lbusACHStatusUpdateBatch;
            //PIR 1040 ACH_STATUS_UPDATE_DAILY_BATCH
            busUpdateCheckStatusBatch lbustatusUpdateDailyBatch;
            busAnnualStatementDataExtractionBatch lbusAnnualStatementDataExtractionBatch;
            busHealthEligibilityActuaryBatch lbusHealthEligibilityActuaryBatch;
            busGenerateAnnualStatementBatch lbusGenerateAnnualStatementBatch;
            busRetireeIncreaseCorrespondenceBatch lbusRetireeIncreaseCorrespondenceBatch;
            busEGWBHealthEligiblityBatch lbusEGWBHealthEligiblityBatch;
            bus5500ReportBatch lbus5500ReportBatch;
            busVIPStatusHistoryBatch VIPStatusHistoryBatch;
            busPensionVerificationHistoryBatch PensionVerificationHistoryBatch;
            busParticipantBenefitSummaryBatch lbusParticipantBenefitSummaryBatch;
            busIAPRecalcSnapshotCleanupBatch lbusIAPRecalcSnapshotCleanupBatch;
            busEEUVHPStatementBatch lbusEEUVHPStatementBatch;
            busRetiremenWorkshop lbusRetiremenWorkshop;
            busOneTimePensionPaymentCorrespondenceBatch lbusOneTimePensionPaymentCorrespondenceBatch;


            int iintCurrentCycleNo = CurrentCycleNo;

            try
            {

                if (lobjJobHeader.ibusCurrentJobDetail != null)
                {
                    switch (lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule.step_no)
                    {
                       
                        case busConstant.MPIPHPBatch.NOTIFICATION_BATCH:
                            lbusNotificationBatch = new busNotificationBatch();
                            lbusNotificationBatch.iobjPassInfo = iobjPassInfo;
                            lbusNotificationBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusNotificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusNotificationBatch.ibusJobHeader = lobjJobHeader;
                            lbusNotificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusNotificationBatch.ProcessNotifications();
                            break;

                        case busConstant.MPIPHPBatch.BENEFIT_APPLICATION_BATCH:
                            lbusBenefitApplicationBatch = new busBenefitApplicationBatch();
                            lbusBenefitApplicationBatch.iobjPassInfo = iobjPassInfo;
                            lbusBenefitApplicationBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusBenefitApplicationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusBenefitApplicationBatch.ibusJobHeader = lobjJobHeader;
                            lbusBenefitApplicationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusBenefitApplicationBatch.ChangeStatusToCancel();
                            break;


                        case busConstant.MPIPHPBatch.RECALCULATE_RETIREMENT_BENEFIT_BATCH:
                            lbusBenefitApplicationBatch = new busBenefitApplicationBatch();
                            lbusBenefitApplicationBatch.iobjPassInfo = iobjPassInfo;
                            lbusBenefitApplicationBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusBenefitApplicationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusBenefitApplicationBatch.ibusJobHeader = lobjJobHeader;
                            lbusBenefitApplicationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusBenefitApplicationBatch.ReCalculateRetirementBenefitBatch();
                            break;

                        case busConstant.MPIPHPBatch.APPROVE_10_PERCENT_INCREASE_PAYEE_ACCOUNT:
                            lbusBenefitApplicationBatch = new busBenefitApplicationBatch();
                            lbusBenefitApplicationBatch.iobjPassInfo = iobjPassInfo;
                            lbusBenefitApplicationBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusBenefitApplicationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusBenefitApplicationBatch.ibusJobHeader = lobjJobHeader;
                            lbusBenefitApplicationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusBenefitApplicationBatch.Approve10PercentIncreasePayeeAccount();
                            break;


                        case busConstant.MPIPHPBatch.RECEIVE_SMALL_WORLD_FILE:
                            lbusBatchHandler = new busReceiveFileHandler(
                                                                            busConstant.File.SMALL_WORLD_FILE_ID,
                                                                            iobjSystemManagement,
                                                                            new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                                                            iintCurrentCycleNo,
                                                                            iobjPassInfo);
                            lbusBatchHandler.Process();
                            break;

                        case busConstant.MPIPHPBatch.UPLOAD_SMALL_WORLD_FILE:
                            lbusBatchHandler = new busUploadFileHandler(
                                                                            busConstant.File.SMALL_WORLD_FILE_ID,
                                                                            iobjSystemManagement,
                                                                            new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                                                            iintCurrentCycleNo,
                                                                            iobjPassInfo);
                            lbusBatchHandler.Process();
                            break;

                        case busConstant.MPIPHPBatch.POST_SMALL_WORLD_INBOUND_FILE:
                            lbusBatchHandler = new busInboundFileHandler(busConstant.File.SMALL_WORLD_FILE_ID,iobjSystemManagement,
                                                                         new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                                                         iintCurrentCycleNo, iobjPassInfo);
                            lbusBatchHandler.Process();
                            break;    
                        
                        case busConstant.MPIPHPBatch.CANCEL_WITHDRAWAL_APPL:
                            lbusBenefitApplicationBatch = new busBenefitApplicationBatch();
                            lbusBenefitApplicationBatch.iobjPassInfo = iobjPassInfo;
                            lbusBenefitApplicationBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusBenefitApplicationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusBenefitApplicationBatch.ibusJobHeader = lobjJobHeader;
                            lbusBenefitApplicationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusBenefitApplicationBatch.ChangeWithdrawlStatusToCancel();
                            break;

                        case busConstant.MPIPHPBatch.PRENOTIFICAITON_BREAK_IN_SERVICE_BATCH:
                            lbusPersonNotificationBatch = new busPersonNotificationBatch();
                            lbusPersonNotificationBatch.iobjPassInfo = iobjPassInfo;
                            lbusPersonNotificationBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusPersonNotificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusPersonNotificationBatch.ibusJobHeader = lobjJobHeader;
                            lbusPersonNotificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusPersonNotificationBatch.PreNotificationBreakInServiceBatch();
                            break;

                        case busConstant.MPIPHPBatch.BREAK_IN_SERVICE_NOTIFICATION_BATCH:
                            lbusPersonNotificationBatch = new busPersonNotificationBatch();
                            lbusPersonNotificationBatch.iobjPassInfo = iobjPassInfo;
                            lbusPersonNotificationBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusPersonNotificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusPersonNotificationBatch.ibusJobHeader = lobjJobHeader;
                            lbusPersonNotificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusPersonNotificationBatch.NotificationBreakInServiceBatch();
                            break;


                        case busConstant.MPIPHPBatch.DEATH_REPORT_BATCH :
                             lbusNotificationBatch = new busNotificationBatch();
                            lbusNotificationBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusNotificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusNotificationBatch.ibusJobHeader = lobjJobHeader;
                            lbusNotificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            DataTable ldtDeathReportData = new DataTable();
                            ldtDeathReportData = lbusNotificationBatch.LoadDeathNotificationBatch();
                            if (ldtDeathReportData.Rows.Count > 0 && ldtDeathReportData.IsNotNull())
                            {
                                busCreateReports lobjCreateReports = new busCreateReports();
                                lobjCreateReports.CreatePDFReport(ldtDeathReportData, busConstant.MPIPHPBatch.REPORT_ACTIVE_DEATH_OUTBOUND);
                                //lobjCreateReports.CreatePDFReport(ldtDeathReportData, busConstant.MPIPHPBatch.REPORT_ACTIVE_DEATH_OUTBOUNDWithSSN);
                            }
                            break;

                        case busConstant.MPIPHPBatch.RETIREE_HEALTH_ELIGIBILITY_BATCH:
                             lbusPersonNotificationBatch = new busPersonNotificationBatch();
                             lbusPersonNotificationBatch.iobjPassInfo = iobjPassInfo;
                             lbusPersonNotificationBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusPersonNotificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusPersonNotificationBatch.ibusJobHeader = lobjJobHeader;
                             lbusPersonNotificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusPersonNotificationBatch.ProcessRetireeHealthElgibility();
                             break;
                        case busConstant.MPIPHPBatch.RETIRE_HEALTH_ELIGIBILITY_REPORT_BATCH://LA Sunset -- Health Eligibility batch for report.
                            lbusHealthEligibilityActuaryBatch = new busHealthEligibilityActuaryBatch();
                            lbusHealthEligibilityActuaryBatch.iobjPassInfo = iobjPassInfo;
                            lbusHealthEligibilityActuaryBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusHealthEligibilityActuaryBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusHealthEligibilityActuaryBatch.ibusJobHeader = lobjJobHeader;
                            lbusHealthEligibilityActuaryBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusHealthEligibilityActuaryBatch.RetireeHealthEligibilityReport(false);
                            break;

                        case busConstant.MPIPHPBatch.THIRTY_DAY_RETIRE_HEALTH_ELIGIBILITY_REPORT_BATCH:
                            lbusHealthEligibilityActuaryBatch = new busHealthEligibilityActuaryBatch();
                            lbusHealthEligibilityActuaryBatch.iobjPassInfo = iobjPassInfo;
                            lbusHealthEligibilityActuaryBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusHealthEligibilityActuaryBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusHealthEligibilityActuaryBatch.ibusJobHeader = lobjJobHeader;
                            lbusHealthEligibilityActuaryBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusHealthEligibilityActuaryBatch.RetireeHealthEligibilityReport(true);
                            break;

                        case busConstant.MPIPHPBatch.PENSION_ELIGIBILITY_BATCH:
                             lbusPersonNotificationBatch = new busPersonNotificationBatch();
                             lbusPersonNotificationBatch.iobjPassInfo = iobjPassInfo;
                             lbusPersonNotificationBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusPersonNotificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusPersonNotificationBatch.ibusJobHeader = lobjJobHeader;
                             lbusPersonNotificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusPersonNotificationBatch.PensionEligibilityBatch();
                             break;

                        case busConstant.MPIPHPBatch.ENCRYPT_SSN_BATCH:
                             lbusNotificationBatch = new busNotificationBatch();
                             lbusNotificationBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusNotificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusNotificationBatch.ibusJobHeader = lobjJobHeader;
                             lbusNotificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusNotificationBatch.EncryptSSN();
                             break;
                        
                        //Minimum Distribution Batch
                        case busConstant.MPIPHPBatch.MINIMUM_DISTRIBUTION_BATCH:
                             lbusPersonNotificationBatch = new busPersonNotificationBatch();
                             lbusPersonNotificationBatch.iobjPassInfo = iobjPassInfo;
                             lbusPersonNotificationBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusPersonNotificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusPersonNotificationBatch.ibusJobHeader = lobjJobHeader;
                             lbusPersonNotificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusPersonNotificationBatch.MinimumDistributionBatch();
                            
                             break;

                        //IAP Hardship Payback Batch
                        case busConstant.MPIPHPBatch.IAP_HARDSHIP_PAYBACK_BATCH:
                            lbusIAPHardshipPaybackBatch = new busIAPHardshipPaybackBatch();
                            lbusIAPHardshipPaybackBatch.iobjPassInfo = iobjPassInfo;
                            lbusIAPHardshipPaybackBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusIAPHardshipPaybackBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusIAPHardshipPaybackBatch.ibusJobHeader = lobjJobHeader;
                            lbusIAPHardshipPaybackBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusIAPHardshipPaybackBatch.IAPHardshipPaybackBatch();

                            break;

                        //Annual Interest Posting Batch
                        case busConstant.MPIPHPBatch.ANNUAL_INTEREST_POSTING_BATCH:
                             lbusPersonNotificationBatch = new busPersonNotificationBatch();
                             lbusPersonNotificationBatch.iobjPassInfo = iobjPassInfo;
                             lbusPersonNotificationBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusPersonNotificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusPersonNotificationBatch.ibusJobHeader = lobjJobHeader;
                             lbusPersonNotificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusPersonNotificationBatch.AnnualInterestPostingBatch();
                             break;

                        case busConstant.MPIPHPBatch.YEAR_END_SNAPSHOT:
                             busYearEndSnapshotBatch lbusYearEndSnapShotBatch = new busYearEndSnapshotBatch(iobjSystemManagement, new busBatchHandler.UpdateProcessLog(UpdateProcessLog), iobjPassInfoLog, lobjJobHeader);
                             lbusYearEndSnapShotBatch.Process();
                             break;

                        case busConstant.MPIPHPBatch.ACTIVE_PART_OUTBOUND_FILE:
                            busActivePartOutboundFile lbusActivePartOutboundFile = new busActivePartOutboundFile();
                            lbusActivePartOutboundFile.iobjPassInfo = iobjPassInfo;
                            lbusActivePartOutboundFile.iobjSystemManagement = iobjSystemManagement;
                            lbusActivePartOutboundFile.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusActivePartOutboundFile.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusActivePartOutboundFile.ibusJobHeader = lobjJobHeader;
                            lbusActivePartOutboundFile.istrProcessName = "Active Participant Outbound File";

                            DataTable ldtbResultData = lbusActivePartOutboundFile.ProcessActiveParticipant();
                            if (ldtbResultData.Rows.Count > 0)
                            {
                                lbusProcessFiles = new busProcessOutboundFile();
                                lbusProcessFiles.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                                lbusProcessFiles.iarrParameters = new object[1];
                                lbusProcessFiles.iarrParameters[0] = ldtbResultData;
                                if (lbusBatchHandler.IsNull())
                                    lbusBatchHandler = new busBatchHandler();
                                lbusBatchHandler.DeleteFile(1);
                                lbusProcessFiles.CreateOutboundFile(1);
                            }
                            break;

                        //Late IAP Allocation Batch
                        case busConstant.MPIPHPBatch.LATE_IAP_ALLOCATION_BATCH:
                             busLateIAPAllocationBatch lbusLateIAPAllocation = new busLateIAPAllocationBatch();
                             lbusLateIAPAllocation.iobjPassInfo = iobjPassInfo;
                             lbusLateIAPAllocation.iobjSystemManagement = iobjSystemManagement;
                             lbusLateIAPAllocation.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusLateIAPAllocation.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusLateIAPAllocation.ibusJobHeader = lobjJobHeader;
                             lbusLateIAPAllocation.istrProcessName = "Late IAP Allocation Batch";
                             lbusLateIAPAllocation.Process();
                             break;

                        //Year End IAP Allocation Batch
                        case busConstant.MPIPHPBatch.YEAREND_IAP_ALLOCATION_BATCH:
                             busYearEndIAPAllocationBatch lbusYearEndIAPAllocation = new busYearEndIAPAllocationBatch();
                             lbusYearEndIAPAllocation.iobjPassInfo = iobjPassInfo;
                             lbusYearEndIAPAllocation.iobjSystemManagement = iobjSystemManagement;
                             lbusYearEndIAPAllocation.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusYearEndIAPAllocation.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusYearEndIAPAllocation.ibusJobHeader = lobjJobHeader;
                             lbusYearEndIAPAllocation.istrProcessName = "Year End IAP Allocation Batch";
                             lbusYearEndIAPAllocation.Process();
                             break;

                        //Year end IAP Allocation Posting Batch
                        case busConstant.MPIPHPBatch.YEAREND_IAP_ALLOCATION_POSTING_BATCH:
                             busIAPAllocationPostingBatch lbusIAPAllocationPosting = new busIAPAllocationPostingBatch();
                             lbusIAPAllocationPosting.iobjPassInfo = iobjPassInfo;
                             lbusIAPAllocationPosting.iobjSystemManagement = iobjSystemManagement;
                             lbusIAPAllocationPosting.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusIAPAllocationPosting.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusIAPAllocationPosting.ibusJobHeader = lobjJobHeader;
                             lbusIAPAllocationPosting.istrProcessName = "Year End IAP Allocation Posting Batch";
                             lbusIAPAllocationPosting.Process();
                             break;
                        case busConstant.MPIPHPBatch.IAP_RECALCULATE_FILE_CLEANUP_BATCH:
                             lbusIAPRecalcSnapshotCleanupBatch = new busIAPRecalcSnapshotCleanupBatch();
                            lbusIAPRecalcSnapshotCleanupBatch.iobjPassInfo = iobjPassInfo;
                            lbusIAPRecalcSnapshotCleanupBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusIAPRecalcSnapshotCleanupBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusIAPRecalcSnapshotCleanupBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusIAPRecalcSnapshotCleanupBatch.ibusJobHeader = lobjJobHeader;
                            lbusIAPRecalcSnapshotCleanupBatch.istrProcessName = "IAP RECALCULATE FILE CLEANUP BATCH";
                            lbusIAPRecalcSnapshotCleanupBatch.ProcessIAPFileCleanUp();
                            break;

                        //Conversion Annual Interest Posting Batch
                        case busConstant.MPIPHPBatch.CONVERSION_INTEREST_POSTING_BATCH:
                             busConversionInterestPostingBatch lbusConversionInterestPostingBatch = new busConversionInterestPostingBatch();
                             lbusConversionInterestPostingBatch.iobjPassInfo = iobjPassInfo;
                             lbusConversionInterestPostingBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusConversionInterestPostingBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusConversionInterestPostingBatch.ibusJobHeader = lobjJobHeader;
                             lbusConversionInterestPostingBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusConversionInterestPostingBatch.Process();
                             break;
                        case busConstant.MPIPHPBatch.BatchPreNoteACH_Bound:
                             lbusProcessFiles = new busProcessOutboundFile();
                             if (this.iobjPassInfo.iconFramework.State == ConnectionState.Closed)
                             {
                                 this.iobjPassInfo.iconFramework.Open();
                             }
                             lbusProcessFiles.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                             string Plan_Identifier_Value = string.Empty;
                             // busProcessFiles lobjProcessPensionFile = new busProcessFiles();
                             lbusProcessFiles.iarrParameters = new object[1];
                             Plan_Identifier_Value = "PENSION";
                             lbusProcessFiles.iarrParameters[0] = Plan_Identifier_Value;
                              if (lbusBatchHandler.IsNull())
                                    lbusBatchHandler = new busBatchHandler();
                                lbusBatchHandler.DeleteFile(2);
                             lbusProcessFiles.CreateOutboundFile(2);
                             //SecondFile IAP
                             Plan_Identifier_Value = "IAP";
                             lbusProcessFiles.iarrParameters[0] = Plan_Identifier_Value;
                             lbusProcessFiles.CreateOutboundFile(2);
                             break;
                        case busConstant.MPIPHPBatch.SSA_DISABILITY_RE_CERTIFICATION_BATCH :
                             busSSADisabilityReCertificationBatch lbusSSADisabilityReCertificationBatch = new busSSADisabilityReCertificationBatch();
                             lbusSSADisabilityReCertificationBatch.iobjPassInfo = iobjPassInfo;
                             lbusSSADisabilityReCertificationBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusSSADisabilityReCertificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusSSADisabilityReCertificationBatch.ibusJobHeader = lobjJobHeader;
                             lbusSSADisabilityReCertificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusSSADisabilityReCertificationBatch.Process();
                             break;
                        case busConstant.MPIPHPBatch.BatchUpdateTaxAmount:
                             lbusProcessFiles = new busProcessOutboundFile();
                             if (this.iobjPassInfo.iconFramework.State == ConnectionState.Closed)
                             {
                                 this.iobjPassInfo.iconFramework.Open();
                             }
                             lbusProcessFiles.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                             // lbusProcessFiles.CreateOutboundFile(1);
                             busUpdateTaxAmount lbusUpdateTaxAmount = new busUpdateTaxAmount();
                             lbusUpdateTaxAmount.iobjPassInfo = iobjPassInfo;
                             lbusUpdateTaxAmount.iobjSystemManagement = iobjSystemManagement;
                             lbusUpdateTaxAmount.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusUpdateTaxAmount.ibusJobHeader = lobjJobHeader;
                             lbusUpdateTaxAmount.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusUpdateTaxAmount.UpdateFederalAndStateTaxAmount();
                             break;
                        case busConstant.MPIPHPBatch.STATE_TAX_UPDATE_BATCH:
                           
                            busUpdateTaxAmount lbusStateTaxUpdate = new busUpdateTaxAmount();
                            lbusStateTaxUpdate.iobjPassInfo = iobjPassInfo;
                            lbusStateTaxUpdate.iobjSystemManagement = iobjSystemManagement;
                            lbusStateTaxUpdate.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusStateTaxUpdate.ibusJobHeader = lobjJobHeader;
                            lbusStateTaxUpdate.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusStateTaxUpdate.UpdateStateTaxBatch();
                            break;
                        case busConstant.MPIPHPBatch.BatchMonthlyPayment:
                             busMonthlyPaymentProcess lobjMonthlyPaymentProcessBatch = new busMonthlyPaymentProcess
                             {
                                 idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                 iobjPassInfo = iobjPassInfo,
                                 //iobjSystemManagement = iobjSystemManagement;
                                 iobjSystemManagement = iobjSystemManagement,
                                 ibusJobHeader=lobjJobHeader
                             };
                             lobjMonthlyPaymentProcessBatch.ProcessPayments();
                             break;

                        case busConstant.MPIPHPBatch.BatchAdhocPayment:
                             busAdhocPaymentProcess lbusAdhocPaymentProcess = new busAdhocPaymentProcess
                             {
                                 idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                 iobjPassInfo = iobjPassInfo,
                                 iobjSystemManagement = iobjSystemManagement
                             };
                             lbusAdhocPaymentProcess.ProcessPayments();
                             break;
                        case busConstant.MPIPHPBatch.BatchWeaklyIAPPayment:
                             busWeeklyIAPPaymentProcess lbusWeeklyIAPPaymentProcess = new busWeeklyIAPPaymentProcess
                             {
                                 idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                 iobjPassInfo = iobjPassInfo,
                                 iobjSystemManagement = iobjSystemManagement,
                                 ibusJobHeader = lobjJobHeader
                             };
                             lbusWeeklyIAPPaymentProcess.ProcessPayments();
                             break;

                        case busConstant.MPIPHPBatch.NEW_PARTICIPANT_BATCH:
                             busNewParticipantBatch lbusNewParticipantBatch = new busNewParticipantBatch
                             {
                                 idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                 iobjPassInfo = iobjPassInfo,
                                 iobjSystemManagement = iobjSystemManagement,
                                 ibusJobHeader = lobjJobHeader
                             };
                             lbusNewParticipantBatch.NewParticipantBatch();
                             break;


                        //Re Evaluation of Minimum Distribution Batch
                        case busConstant.MPIPHPBatch.RE_EVALUATION_OF_MINIMUM_DISTRIBUTION_BATCH:
                             lbusReEvaluationOfMinimumDistributionBatch = new busReEvaluationOfMinimumDistributionBatch();
                             lbusReEvaluationOfMinimumDistributionBatch.iobjPassInfo = iobjPassInfo;
                             lbusReEvaluationOfMinimumDistributionBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusReEvaluationOfMinimumDistributionBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusReEvaluationOfMinimumDistributionBatch.ibusJobHeader = lobjJobHeader;
                             lbusReEvaluationOfMinimumDistributionBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusReEvaluationOfMinimumDistributionBatch.ReEvaluationOfMinimumDistributionBatch();
                             break;
                            
                         case busConstant.MPIPHPBatch.IAP_REQUIRED_MINIMUM_DISTRIBUTION_BATCH:
                            lbusReEvaluationOfMinimumDistributionBatch = new busReEvaluationOfMinimumDistributionBatch();
                            lbusReEvaluationOfMinimumDistributionBatch.iobjPassInfo = iobjPassInfo;
                            lbusReEvaluationOfMinimumDistributionBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusReEvaluationOfMinimumDistributionBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusReEvaluationOfMinimumDistributionBatch.ibusJobHeader = lobjJobHeader;
                            lbusReEvaluationOfMinimumDistributionBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusReEvaluationOfMinimumDistributionBatch.IAPRMDReport();
                            break;

                        //Benefit Adjustment Batch
                        case busConstant.MPIPHPBatch.BENEFIT_ADJUSTMENT_BATCH:
                             lbusBenefitAdjustmentBatch = new busBenefitAdjustmentBatch();
                             lbusBenefitAdjustmentBatch.iobjPassInfo = iobjPassInfo;
                             lbusBenefitAdjustmentBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusBenefitAdjustmentBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusBenefitAdjustmentBatch.ibusJobHeader = lobjJobHeader;
                             lbusBenefitAdjustmentBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusBenefitAdjustmentBatch.BenefitAdjustmentBatch();
                             break;

                        //Onetime Payment batch
                        case busConstant.MPIPHPBatch.ONETIME_PAYMENT_BATCH_STEP_NO:
                            busOnetimePaymentBatch lbusOnetimePayment = new busOnetimePaymentBatch();
                            lbusOnetimePayment.iobjPassInfo = iobjPassInfo;
                            lbusOnetimePayment.iobjSystemManagement = iobjSystemManagement;
                            lbusOnetimePayment.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusOnetimePayment.ibusJobHeader = lobjJobHeader;
                            lbusOnetimePayment.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusOnetimePayment.OnetimePaymentBatch();
                            break;

                        //Active Retiree Increase batch
                        case busConstant.MPIPHPBatch.ACTIVE_RETIREE_INCREASE_BATCH:
                             lbusActiveRetireeIncrease = new busActiveRetireeIncreaseBatch();
                             lbusActiveRetireeIncrease.iobjPassInfo = iobjPassInfo;
                             lbusActiveRetireeIncrease.iobjSystemManagement = iobjSystemManagement;
                             lbusActiveRetireeIncrease.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusActiveRetireeIncrease.ibusJobHeader = lobjJobHeader;
                             lbusActiveRetireeIncrease.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusActiveRetireeIncrease.ActiveRetireeIncreaseBatch();
                             break;

                        //Retiree Increase Rollover
                        case busConstant.MPIPHPBatch.RETIREE_INCREASE_ROLLOVER_BATCH:
                             lbusActiveRetireeIncrease = new busActiveRetireeIncreaseBatch();
                             lbusActiveRetireeIncrease.iobjPassInfo = iobjPassInfo;
                             lbusActiveRetireeIncrease.iobjSystemManagement = iobjSystemManagement;
                             lbusActiveRetireeIncrease.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusActiveRetireeIncrease.ibusJobHeader = lobjJobHeader;
                             lbusActiveRetireeIncrease.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusActiveRetireeIncrease.RetireeIncreaseRolloverBatch();
                             break;

                        //RETIREMENT_AFFIDAVIT_BATCH
                        case busConstant.MPIPHPBatch.RETIREMENT_AFFIDAVIT_BATCH:
                             lbusIAPPaymentNotificationProcess = new busIAPPaymentNotificationProcess();
                             lbusIAPPaymentNotificationProcess.iobjPassInfo = iobjPassInfo;
                             lbusIAPPaymentNotificationProcess.iobjSystemManagement = iobjSystemManagement;
                             lbusIAPPaymentNotificationProcess.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusIAPPaymentNotificationProcess.ibusJobHeader = lobjJobHeader;
                             lbusIAPPaymentNotificationProcess.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusIAPPaymentNotificationProcess.RetirementAffidavitBatch();
                             break;

                        //VERIFICATION OF HOURS BATCH
                        case busConstant.MPIPHPBatch.VERIFICATION_OF_HOURS_BATCH:
                             lbusVerificationOfHoursBatch = new busVerificationOfHoursBatch();
                             lbusVerificationOfHoursBatch.iobjPassInfo = iobjPassInfo;
                             lbusVerificationOfHoursBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusVerificationOfHoursBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusVerificationOfHoursBatch.ibusJobHeader = lobjJobHeader;
                             lbusVerificationOfHoursBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusVerificationOfHoursBatch.VerificationOfHoursBatch();
                             break;

                        //IAP_PAYMENT_ADJUSTMENT_BATCH
                        case busConstant.MPIPHPBatch.IAP_PAYMENT_ADJUSTMENT_BATCH:
                             lbusIAPPaymentAdjustmentBatch = new busIAPPaymentAdjustmentBatch();
                             lbusIAPPaymentAdjustmentBatch.iobjPassInfo = iobjPassInfo;
                             lbusIAPPaymentAdjustmentBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusIAPPaymentAdjustmentBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusIAPPaymentAdjustmentBatch.ibusJobHeader = lobjJobHeader;
                             lbusIAPPaymentAdjustmentBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusIAPPaymentAdjustmentBatch.IAPPaymentAdjustmentBatch();
                             break;

                        case busConstant.MPIPHPBatch.REEMPLOYED_BATCH:
                             lbusReemployed = new busReemployed();
                             lbusReemployed.iobjPassInfo = iobjPassInfo;
                             lbusReemployed.iobjSystemManagement = iobjSystemManagement;
                             lbusReemployed.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusReemployed.ibusJobHeader = lobjJobHeader;
                             lbusReemployed.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusReemployed.ReemployedBatch();
                             break;

                        case busConstant.MPIPHPBatch.REEVALUATION_OF_REEMPLOYED_BATCH:
                             lbusReemployed = new busReemployed();
                             lbusReemployed.iobjPassInfo = iobjPassInfo;
                             lbusReemployed.iobjSystemManagement = iobjSystemManagement;
                             lbusReemployed.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusReemployed.ibusJobHeader = lobjJobHeader;
                             lbusReemployed.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusReemployed.AnnualReEvaluationforReEmployedBatch();
                             break;

                        case busConstant.MPIPHPBatch.PAYEE_ERROR_BATCH:
                             busMonthlyPaymentProcess lobjMonthlyPaymentProcessBatchForPayeeError = new busMonthlyPaymentProcess
                             {
                                 idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                 iobjPassInfo = iobjPassInfo,
                                 iobjSystemManagement = iobjSystemManagement,
                                 ibusJobHeader=lobjJobHeader
                             };
                             lobjMonthlyPaymentProcessBatchForPayeeError.ProcessPayeeErrorBatch();
                             break;
                        case busConstant.MPIPHPBatch.VENDOR_PAYMENT_BATCH:
                             busMonthlyPaymentProcess lobjVendorPaymentProcessBatch = new busMonthlyPaymentProcess
                             {
                                 idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                 iobjPassInfo = iobjPassInfo,
                                 iobjSystemManagement = iobjSystemManagement,
                                 ibusJobHeader = lobjJobHeader
                             };
                             lobjVendorPaymentProcessBatch.ProcessVendorPayments();
                             break;

                        case busConstant.MPIPHPBatch.RECEIVE_CHECK_RECONCILIATION_SERVICE:
                             lbusBatchHandler = new busReceiveFileHandler(
                                                                             busConstant.File.CHECK_RECONCILIATION_SERVICE_FILE_ID,
                                                                             iobjSystemManagement,
                                                                             new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                                                             iintCurrentCycleNo,
                                                                             iobjPassInfo);
                             lbusBatchHandler.Process();
                             break;

                        case busConstant.MPIPHPBatch.UPLOAD_CHECK_RECONCILIATION_SERVICE:
                             lbusBatchHandler = new busUploadFileHandler(
                                                                             busConstant.File.CHECK_RECONCILIATION_SERVICE_FILE_ID,
                                                                             iobjSystemManagement,
                                                                             new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                                                             iintCurrentCycleNo,
                                                                             iobjPassInfo);
                             lbusBatchHandler.Process();
                             break;

                        case busConstant.MPIPHPBatch.POST_CHECK_RECONCILIATION_SERVICE:
                             lbusBatchHandler = new busInboundFileHandler(busConstant.File.CHECK_RECONCILIATION_SERVICE_FILE_ID, iobjSystemManagement,
                                                                          new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                                                          iintCurrentCycleNo, iobjPassInfo);
                             lbusBatchHandler.Process();
                             break;

                        case busConstant.MPIPHPBatch.INTEGERATION_REEMPLOYMENT_BATCH:
                             lbusReemployed = new busReemployed();
                             lbusReemployed.iobjPassInfo = iobjPassInfo;
                             lbusReemployed.iobjSystemManagement = iobjSystemManagement;
                             lbusReemployed.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusReemployed.ibusJobHeader = lobjJobHeader;
                             lbusReemployed.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusReemployed.UpadateParticipantAsReEmployedFromEADB();
                             break;

                        case busConstant.MPIPHPBatch.RESUME_BENEFITS_BATCH:
                             lbusReemployed = new busReemployed();
                             lbusReemployed.iobjPassInfo = iobjPassInfo;
                             lbusReemployed.iobjSystemManagement = iobjSystemManagement;
                             lbusReemployed.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusReemployed.ibusJobHeader = lobjJobHeader;
                             lbusReemployed.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusReemployed.ResumeBenefitsBatch();
                             break;

                        case busConstant.MPIPHPBatch.ACH_STATUS_UPDATE_BATCH:
                             lbusACHStatusUpdateBatch = new busACHStatusUpdateBatch();
                             lbusACHStatusUpdateBatch.iobjPassInfo = iobjPassInfo;
                             lbusACHStatusUpdateBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusACHStatusUpdateBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusACHStatusUpdateBatch.ibusJobHeader = lobjJobHeader;
                             lbusACHStatusUpdateBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusACHStatusUpdateBatch.ACHStatusUpdateBatch();
                             break;
                        //PIR 1040 UPDATE_CHECK_STATUS_BATCH
                        case busConstant.MPIPHPBatch.UPDATE_CHECK_STATUS_BATCH:
                            lbustatusUpdateDailyBatch = new busUpdateCheckStatusBatch();
                            lbustatusUpdateDailyBatch.iobjPassInfo = iobjPassInfo;
                            lbustatusUpdateDailyBatch.iobjSystemManagement = iobjSystemManagement;
                            lbustatusUpdateDailyBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbustatusUpdateDailyBatch.ibusJobHeader = lobjJobHeader;
                            lbustatusUpdateDailyBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbustatusUpdateDailyBatch.UpdateCheckStatusBatch();
                            break;

                        case busConstant.MPIPHPBatch.ANNUAL_STATEMENT_DATA_EXTRACTION_BATCH:
                             lbusAnnualStatementDataExtractionBatch = new busAnnualStatementDataExtractionBatch();
                             lbusAnnualStatementDataExtractionBatch.iobjPassInfo = iobjPassInfo;
                             lbusAnnualStatementDataExtractionBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusAnnualStatementDataExtractionBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusAnnualStatementDataExtractionBatch.ibusJobHeader = lobjJobHeader;
                             lbusAnnualStatementDataExtractionBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusAnnualStatementDataExtractionBatch.ProcessAnnualStatementDataExtraction();
                             break;

                        case busConstant.MPIPHPBatch.HEALTH_ELIGIBILITY_ACTUARY_BATCH:
                             lbusHealthEligibilityActuaryBatch = new busHealthEligibilityActuaryBatch();
                             lbusHealthEligibilityActuaryBatch.iobjPassInfo = iobjPassInfo;
                             lbusHealthEligibilityActuaryBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusHealthEligibilityActuaryBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusHealthEligibilityActuaryBatch.ibusJobHeader = lobjJobHeader;
                             lbusHealthEligibilityActuaryBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusHealthEligibilityActuaryBatch.ProcessHealthEligibilityActuaryBatch();
                             break;

                        case busConstant.MPIPHPBatch.EGWP_PARTICIPANTS_BATCH:
                            lbusEGWBHealthEligiblityBatch = new busEGWBHealthEligiblityBatch();
                            lbusEGWBHealthEligiblityBatch.iobjPassInfo = iobjPassInfo;
                            lbusEGWBHealthEligiblityBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusEGWBHealthEligiblityBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusEGWBHealthEligiblityBatch.ibusJobHeader = lobjJobHeader;
                            lbusEGWBHealthEligiblityBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusEGWBHealthEligiblityBatch.ProcessEGWPHealthEligibilityBatch();
                            break;

                        case busConstant.MPIPHPBatch.HEALTH_ELIGIBILITY_ACTUARY_OUTBOUND_FILE:
                             lbusProcessFiles = new busProcessOutboundFile();
                             if (this.iobjPassInfo.iconFramework.State == ConnectionState.Closed)
                             {
                                 this.iobjPassInfo.iconFramework.Open();
                             }
                             lbusProcessFiles.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                              if (lbusBatchHandler.IsNull())
                                    lbusBatchHandler = new busBatchHandler();
                                lbusBatchHandler.DeleteFile(1004);
                             lbusProcessFiles.CreateOutboundFile(1004);
                             break;

                        case busConstant.MPIPHPBatch.GENERATE_PENSION_ACTUARY_FILE:
                             lbusProcessFiles = new busProcessOutboundFile();
                             if (this.iobjPassInfo.iconFramework.State == ConnectionState.Closed)
                             {
                                 this.iobjPassInfo.iconFramework.Open();
                             }
                             lbusProcessFiles.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                              if (lbusBatchHandler.IsNull())
                                    lbusBatchHandler = new busBatchHandler();
                                lbusBatchHandler.DeleteFile(1005);
                             lbusProcessFiles.CreateOutboundFile(1005);
                             break;

                        case busConstant.MPIPHPBatch.RECEIVE_ACH_NOTIFICATION_OF_CHANGE_AND_RETURN_INBOUND_FILE:
                             lbusBatchHandler = new busReceiveFileHandler(
                                                                             busConstant.File.ACH_NOTIFICATION_OF_CHANGE_AND_RETURN_INBOUND_FILE,
                                                                             iobjSystemManagement,
                                                                             new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                                                             iintCurrentCycleNo,
                                                                             iobjPassInfo);
                             lbusBatchHandler.Process();
                             break;

                        case busConstant.MPIPHPBatch.UPLOAD_ACH_NOTIFICATION_OF_CHANGE_AND_RETURN_INBOUND_FILE:
                             lbusBatchHandler = new busUploadFileHandler(
                                                                             busConstant.File.ACH_NOTIFICATION_OF_CHANGE_AND_RETURN_INBOUND_FILE,
                                                                             iobjSystemManagement,
                                                                             new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                                                             iintCurrentCycleNo,
                                                                             iobjPassInfo);
                             lbusBatchHandler.Process();
                             break;

                        case busConstant.MPIPHPBatch.POST_ACH_NOTIFICATION_OF_CHANGE_AND_RETURN_INBOUND_FILE:
                             lbusBatchHandler = new busInboundFileHandler(busConstant.File.ACH_NOTIFICATION_OF_CHANGE_AND_RETURN_INBOUND_FILE, 
                                                                          iobjSystemManagement,
                                                                          new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                                                          iintCurrentCycleNo, iobjPassInfo);
                             lbusBatchHandler.Process();
                             break;

                        case busConstant.MPIPHPBatch.GENERATE_ANNUAL_STATEMENT_BATCH:
                             lbusGenerateAnnualStatementBatch = new busGenerateAnnualStatementBatch();
                             lbusGenerateAnnualStatementBatch.iobjPassInfo = iobjPassInfo;
                             lbusGenerateAnnualStatementBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusGenerateAnnualStatementBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusGenerateAnnualStatementBatch.ibusJobHeader = lobjJobHeader;
                             lbusGenerateAnnualStatementBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusGenerateAnnualStatementBatch.GenerateAnnualStatements();
                             break;

                        case busConstant.MPIPHPBatch.EDD_OUTBOUND_FILE:
                            busAnnual1099rBatch lobjAnnual1099rBatch = new busAnnual1099rBatch
                             {
                                 idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                 iobjPassInfo = iobjPassInfo,
                                 icdoBatchSchedule=lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule,
                                 iobjSystemManagement = iobjSystemManagement,
                                 ibusJobHeader = lobjJobHeader
                             };

                             lobjAnnual1099rBatch.ProcessEDDFiles();
                             break;
                            //lbusProcessFiles = new busProcessFiles();
                            //if (this.iobjPassInfo.iconFramework.State == ConnectionState.Closed)
                            //{
                            //    this.iobjPassInfo.iconFramework.Open();
                            //}
                            //lbusProcessFiles.idlgUpdateProcessLog = new busProcessFiles.UpdateProcessLog(UpdateProcessLog);
                            //lbusProcessFiles.CreateOutboundFile(1007);
                            //break;
                          //  Ticket#68932
                        case busConstant.MPIPHPBatch.PENSION_VERIFICATION_HISTORY_BATCH:
                            busPensionVerificationHistoryBatch lbusPensionVerificationHistoryBatch = new busPensionVerificationHistoryBatch();
                            lbusPensionVerificationHistoryBatch.iobjPassInfo = iobjPassInfo;
                            lbusPensionVerificationHistoryBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusPensionVerificationHistoryBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusPensionVerificationHistoryBatch.ibusJobHeader = lobjJobHeader;
                            lbusPensionVerificationHistoryBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusPensionVerificationHistoryBatch.Process();
                            break;
                        //WI 19555 - PBV Phase-1 Suspension Batch
                        case busConstant.MPIPHPBatch.PENSION_VERIFICATION_SUSPEND_BATCH:
                            busPensionVerificationHistoryBatch lbusPensionVerificationsSuspendBatch = new busPensionVerificationHistoryBatch();
                            lbusPensionVerificationsSuspendBatch.iobjPassInfo = iobjPassInfo;
                            lbusPensionVerificationsSuspendBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusPensionVerificationsSuspendBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusPensionVerificationsSuspendBatch.ibusJobHeader = lobjJobHeader;
                            lbusPensionVerificationsSuspendBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusPensionVerificationsSuspendBatch.SuspendProcess();
                            break;
                        case busConstant.MPIPHPBatch.RECLAMATION_OUTBOUND_FILE:
                            DataTable ldtACHPaymentDistribution = busBase.Select("cdoPaymentHistoryDistribution.LoadACHForReclamationFile", new object[0] { });
                            if (ldtACHPaymentDistribution.Rows.Count > 0)
                            {
                                //FM upgrade: 6.0.6.2 changes - busProcessFiles should be replaced with busProcessOutboundFile for outbound related method and processing
                                //busProcessFiles lobjProcessFiles = new busProcessFiles();
                                busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                                lobjProcessFiles.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                                lobjProcessFiles.iarrParameters = new object[3];
                                lobjProcessFiles.iarrParameters[0] = busConstant.BOOL_FALSE;
                                lobjProcessFiles.iarrParameters[1] = ldtACHPaymentDistribution;
                                lobjProcessFiles.iarrParameters[2] = busConstant.PAYMENT_DISTRIBUTION_STATUS_RECLAIMED;
                                if (lbusBatchHandler.IsNull())
                                    lbusBatchHandler = new busBatchHandler();
                                lbusBatchHandler.DeleteFile(4,"ACHReclamationFile");
                                lobjProcessFiles.CreateOutboundFile(4);
                            }
                        break;
                        case busConstant.MPIPHPBatch.BatchAnnual1099r:
                             busAnnual1099rBatch lobjEDDBatch = new busAnnual1099rBatch
                             {
                                 idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                 iobjPassInfo = iobjPassInfo,
                                 icdoBatchSchedule=lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule,
                                 iobjSystemManagement = iobjSystemManagement,
                                 ibusJobHeader = lobjJobHeader
                             };
                             lobjEDDBatch.ProcessAnnual1099rBatch();
                             break;
                        case busConstant.MPIPHPBatch.BatchGenrate1099r:
                             busGenerate1099rFile lobjGenerate1099rFile = new busGenerate1099rFile
                             {
                                 idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                 iobjPassInfo = iobjPassInfo,
                                 icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule,
                                 iobjSystemManagement = iobjSystemManagement,
                                 ibusJobHeader = lobjJobHeader
                             };
                             lobjGenerate1099rFile.ProcessBatch();
                             break;
                        case busConstant.MPIPHPBatch.BatchCorrected1099r:
                             busCorrected1099rBatch lobjCorrected1099rBatch = new busCorrected1099rBatch
                             {
                                 idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                 iobjPassInfo = iobjPassInfo,
                                 icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule,
                                 iobjSystemManagement = iobjSystemManagement,
                                 ibusJobHeader=lobjJobHeader
                             };
                             lobjCorrected1099rBatch.ProcessCorrected1099rBatch();
                             break;

                        //Retiree Increase Correspondence batch
                        case busConstant.MPIPHPBatch.RETIREE_INCREASE_CORRESPONDENCE_BATCH:
                             lbusRetireeIncreaseCorrespondenceBatch = new busRetireeIncreaseCorrespondenceBatch();
                             lbusRetireeIncreaseCorrespondenceBatch.iobjPassInfo = iobjPassInfo;
                             lbusRetireeIncreaseCorrespondenceBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusRetireeIncreaseCorrespondenceBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusRetireeIncreaseCorrespondenceBatch.ibusJobHeader = lobjJobHeader;
                             lbusRetireeIncreaseCorrespondenceBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusRetireeIncreaseCorrespondenceBatch.RetireeIncreaseCorrespondenceBatch();
                             break;
                        //Health Address Update Batch
                        case busConstant.MPIPHPBatch.BatchHealthAddrUpdateBatch:
                             busHealthAddrUpdateBatch lobjHealthAddrUpdateBatch = new busHealthAddrUpdateBatch
                             {
                                 idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog),
                                 iobjPassInfo = iobjPassInfo,
                                 icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule,
                                 iobjSystemManagement = iobjSystemManagement,
                                 ibusJobHeader=lobjJobHeader
                             };
                             lobjHealthAddrUpdateBatch.HealthAddrUpdateBatch();
                             break;

                        case busConstant.MPIPHPBatch.SMALL_WORLD_OUTBOUND_PAYEE_FILE:
                             lbusProcessFiles = new busProcessOutboundFile();
                             if (this.iobjPassInfo.iconFramework.State == ConnectionState.Closed)
                             {
                                 this.iobjPassInfo.iconFramework.Open();
                             }
                             lbusProcessFiles.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                             if (lbusBatchHandler.IsNull())
                                 lbusBatchHandler = new busBatchHandler();
                             lbusBatchHandler.DeleteFile(1009);
                             lbusProcessFiles.CreateOutboundFile(1009);
                             break;
                        //PIR 1003
                        case busConstant.MPIPHPBatch.ANNUAL_BENEFIT_SUMMARY_CORRESPONDENCE_BATCH:
                             lbusPersonNotificationBatch = new busPersonNotificationBatch();
                             lbusPersonNotificationBatch.iobjPassInfo = iobjPassInfo;
                             lbusPersonNotificationBatch.iobjSystemManagement = iobjSystemManagement;
                             lbusPersonNotificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                             lbusPersonNotificationBatch.ibusJobHeader = lobjJobHeader;
                             lbusPersonNotificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                             lbusPersonNotificationBatch.AnnualBenefitSummaryCorrespondenceBatch();
                             break;
                        //ID_59363
                        case busConstant.MPIPHPBatch.REPORT_5500_BATCH:
                            lbus5500ReportBatch = new bus5500ReportBatch();
                            lbus5500ReportBatch.iobjPassInfo = iobjPassInfo;
                            lbus5500ReportBatch.iobjSystemManagement = iobjSystemManagement;
                            lbus5500ReportBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbus5500ReportBatch.ibusJobHeader = lobjJobHeader;
                            lbus5500ReportBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbus5500ReportBatch.Report5500Batch();
                            break;

                        case busConstant.MPIPHPBatch.PARTICIPANT_SUMMARY_BATCH:
                            lbusParticipantBenefitSummaryBatch = new busParticipantBenefitSummaryBatch();
                            lbusParticipantBenefitSummaryBatch.iobjPassInfo = iobjPassInfo;
                            lbusParticipantBenefitSummaryBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusParticipantBenefitSummaryBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusParticipantBenefitSummaryBatch.ibusJobHeader = lobjJobHeader;
                            lbusParticipantBenefitSummaryBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusParticipantBenefitSummaryBatch.ParticipantBenefitSummaryBatch();
                            break;
                        //VIP Status Check History Batch
                        case busConstant.MPIPHPBatch.VIP_STATUS_HISTORY_BATCH:
                            VIPStatusHistoryBatch = new busVIPStatusHistoryBatch();
                            VIPStatusHistoryBatch.iobjPassInfo = iobjPassInfo;
                            VIPStatusHistoryBatch.iobjSystemManagement = iobjSystemManagement;
                            VIPStatusHistoryBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            VIPStatusHistoryBatch.ibusJobHeader = lobjJobHeader;
                            VIPStatusHistoryBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            VIPStatusHistoryBatch.LoadVIPStatusHistoryData();
                            break;

                        case busConstant.MPIPHPBatch.EE_UVHP_STATEMENT_BATCH:
                            lbusEEUVHPStatementBatch = new busEEUVHPStatementBatch();
                            lbusEEUVHPStatementBatch.iobjPassInfo = iobjPassInfo;
                            lbusEEUVHPStatementBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusEEUVHPStatementBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusEEUVHPStatementBatch.ibusJobHeader = lobjJobHeader;
                            lbusEEUVHPStatementBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusEEUVHPStatementBatch.LoadEEUVHPBenefitandInterestAmounts();
                            break;
                        case busConstant.MPIPHPBatch.RETIREMENT_WORKSHOP:
                            lbusRetiremenWorkshop = new busRetiremenWorkshop();
                            lbusRetiremenWorkshop.iobjPassInfo = iobjPassInfo;
                            lbusRetiremenWorkshop.iobjSystemManagement = iobjSystemManagement;
                            lbusRetiremenWorkshop.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusRetiremenWorkshop.ibusJobHeader = lobjJobHeader;
                            lbusRetiremenWorkshop.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusRetiremenWorkshop.CreateRetirmentWorkshopCorrespondence();
                            break;

                        case busConstant.MPIPHPBatch.LAST_ONE_YEAR_DEATH_NOTIFICATION_REPORT:
                            lbusNotificationBatch = new busNotificationBatch();
                            lbusNotificationBatch.iobjPassInfo = iobjPassInfo;
                            lbusNotificationBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusNotificationBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusNotificationBatch.ibusJobHeader = lobjJobHeader;
                            lbusNotificationBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusNotificationBatch.CreateLastOneYearDeathNotificationReport();
                            break;

                        case busConstant.MPIPHPBatch.ONE_TIME_PENSION_PAYMENT_CORRESPONDENCE_BATCH:
                            lbusOneTimePensionPaymentCorrespondenceBatch = new busOneTimePensionPaymentCorrespondenceBatch();
                            lbusOneTimePensionPaymentCorrespondenceBatch.iobjPassInfo = iobjPassInfo;
                            lbusOneTimePensionPaymentCorrespondenceBatch.iobjSystemManagement = iobjSystemManagement;
                            lbusOneTimePensionPaymentCorrespondenceBatch.idlgUpdateProcessLog = new busBatchHandler.UpdateProcessLog(UpdateProcessLog);
                            lbusOneTimePensionPaymentCorrespondenceBatch.ibusJobHeader = lobjJobHeader;
                            lbusOneTimePensionPaymentCorrespondenceBatch.icdoBatchSchedule = lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule;
                            lbusOneTimePensionPaymentCorrespondenceBatch.OneTimePensionPaymentCorrespondenceBatch();
                            break;
                    }
                }
                if (lintRetVal == -9)
                {
                    lintRetVal = 0;
                }
            }
            catch (Exception ex)
            {
                lintRetVal = -1;
                SendBatchStatusNotification(true, lintRetVal);
                try
                {
                    ExceptionManager.Publish(ex);
                    UpdateProcessLog(string.Format(busConstant.MPIPHPBatch.MESSAGE_FORMAT_UNCAUGHT_EXCEPTION, lobjJobHeader.icdoJobHeader.job_header_id.ToString(), ex.ToString()), busConstant.MPIPHPBatch.ERROR_TYPE_ERROR, lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule.step_name);
                }
                catch (Exception ex1)
                {
                    //need to eat up exception since this is the lowest level of error logging and cannot let batch process die
                }
                throw;
            }
            //SendBatchStatusNotification(false, lintRetVal);
            return lintRetVal;
        }


        private bool ShouldContinueDataProcessing(int aintJobHeaderId)
        {
            bool lblnResult = false;
            busJobHeader lbusJobHeader = new busJobHeader() { icdoJobHeader = new cdoJobHeader() };
            if (lbusJobHeader.FindJobHeader(aintJobHeaderId))
            {
                lblnResult = (BatchHelper.JOB_HEADER_STATUS_CANCEL_REQUESTED.ToLower() != lbusJobHeader.icdoJobHeader.status_value.ToLower());
            }
            else
            {
                //return false if job header is not found!!
                lblnResult = false;
            }
            return lblnResult;
        }
        /// <summary>
        /// Generic method to initalize all batch related informations. Set's Process Log, System Management and Batch Schedule
        /// </summary>
        /// <param name="aobjNeoSpinBatch">Type of NeoSpinBatch</param>
        private void InitializeBatchInfo(busMPIPHPBatch aobjNeoSpinBatch)
        {
            aobjNeoSpinBatch.idlgUpdateProcessLog = new busMPIPHPBatch.UpdateProcessLog(UpdateProcessLog);
            aobjNeoSpinBatch.iobjSystemManagement = iobjSystemManagement;
            aobjNeoSpinBatch.iobjBatchSchedule = iobjBatchSchedule;
        }

        /// <summary>
        /// Gets the job parameter value for the given job parameter name 
        /// </summary>
        /// <param name="strJobParamName"></param>
        /// <returns></returns>
        private string GetJobParameterValue(string strJobParamName)
        {
            string strJobParamValue = string.Empty;

            if (lobjJobHeader.ibusCurrentJobDetail.iclbEditableJobParameters != null
                && lobjJobHeader.ibusCurrentJobDetail.iclbEditableJobParameters.Count > 0)
            {
                // Get the current job parameter item for the given param name from the collection
                busJobParameters lobjJobParameters =
                    lobjJobHeader.ibusCurrentJobDetail.iclbEditableJobParameters
                        .Where(lobjJobParameter => lobjJobParameter.icdoJobParameters.param_name == strJobParamName)
                        .FirstOrDefault();

                // Gets the param value for the job parameter name if exists
                if (lobjJobParameters != null) strJobParamValue = lobjJobParameters.icdoJobParameters.param_value;
            }
            return strJobParamValue;
        }

        /// <summary>
        /// Generate correspondence, create the tracking record for the generated letter
        /// </summary>
        /// <param name="aintTemplateID"></param>
        /// <param name="aintPersonID"></param>
        /// <param name="astrUserId"></param>
        /// <param name="aarrResult"></param>
        /// <returns></returns>
        public string CreateCorrespondence(string astrTemplateName, string astrUserID, int aintUserSerialID, ArrayList aarrResult, Hashtable ahtbQueryBkmarks)
        {
            //utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence(
            //    astrTemplateName, astrUserID, aintUserSerialID, aarrResult, ahtbQueryBkmarks);

            //if (lobjCorresPondenceInfo == null)
            //{
            //    throw new Exception("Unable to create correspondence, SetCorrespondence method not found in " +
            //        " business solutions base object");
            //}

            string lstrFileName = "";
            //lstrFileName = iobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName,
            //    lobjCorresPondenceInfo, astrUserID);
            return lstrFileName;
        }

     
        public void StartCounter()
        {
            UpdateProcessLog("Started processing.. ", "INFO", StepName);
            Count = 0;
            InternalCount = 0;
        }

        public void StartCounter(int aintLimit)
        {
            StartCounter();
            Limit = aintLimit;
        }

        public void CallCounter()
        {
            if (InternalCount == Limit)
            {
                if (MaxCount != 0)
                {
                    //display this message if max count is available
                    UpdateProcessLog("Currently processing record " + Count + " of " + MaxCount, "INFO", StepName);
                }
                else
                {
                    UpdateProcessLog("Currently processing record " + Count, "INFO", StepName);
                }
                InternalCount = 0;
            }
            Count++;
            InternalCount++;
        }

        public void EndCounter()
        {
            UpdateProcessLog("Ended processing.. Total records read " + Count, "INFO", StepName);
        }

        public utlPassInfo iobjPassInfo
        {
            get
            {
                return utlPassInfo.iobjPassInfo;
            }
        }

        protected void PostErrorMessage(string astrMessageToBePosted)
        {
            TraceLine(astrMessageToBePosted, TraceLevel.Error);
            PostMessage(astrMessageToBePosted, BatchHelper.BATCH_MESSAGE_ERROR);
        }

        protected void PostSummaryMessage(string astrMessageToBePosted)
        {
            TraceLine(astrMessageToBePosted, TraceLevel.Info);
            PostMessage(astrMessageToBePosted, BatchHelper.BATCH_MESSAGE_SUMMARY);
        }

        protected void PostInfoMessage(string astrMessageToBePosted)
        {
            TraceLine(astrMessageToBePosted, TraceLevel.Info);
            PostMessage(astrMessageToBePosted, BatchHelper.BATCH_MESSAGE_INFO);
        }

        protected void PostMessage(string astrMessageToBePosted, string astrMessageType)
        {
            UpdateProcessLog(astrMessageToBePosted, astrMessageType, StepName);
        }

        protected virtual void SummarizeProcessCompletion()
        {
            PostSummaryMessage("Total number of records to be processed = " + TotalRecords);
            PostSummaryMessage("Total number of records successfully processed = " + TotalSuccess);
            PostSummaryMessage("Total number of records errored out = " + TotalError);
        }

        protected virtual void InitSummaryVariables()
        {
            TotalRecords = 0;
            TotalSuccess = 0;
            TotalError = 0;
        }

        private void SetErrorMessage(busBatchHandler abusBatchHandler, ref int aintValue)
        {
            if (abusBatchHandler.iintOutboundReturnCode == -1)
            {
                aintValue = -1;
                istrOutBoundError = abusBatchHandler.istrOutboundErrorMessage;
            }
        }
        /*Being Used For Crystal Reports
        public string CreateReport(string astrReportName, DataTable adstResult)
        {
            RptBatch = new ReportDocument();
            RptBatch.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init
            string labsRptDefPath = string.Empty;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptDF");
            RptBatch.Load(labsRptDefPath + astrReportName);
            RptBatch.SetDataSource(adstResult);             // gets the data and bind to the report doc control
            string labsRptGenPath = string.Empty;
            labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptGN");
            string lstrReportFullName = string.Empty;
            lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            RptBatch.ExportToDisk(ExportFormatType.PortableDocFormat, lstrReportFullName);
            return lstrReportFullName;
        }*/

        public void CreatePDFReport(DataTable ldtbResultTable, string astrReportName)
        {
            
                ReportViewer rvViewer = new ReportViewer();
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                string labsRptDefPath = string.Empty;

                DataTable ldtbReportTable = ldtbResultTable;
                rvViewer.ProcessingMode = ProcessingMode.Local;
                labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);
                
                rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";
                ReportDataSource lrdsReport = new ReportDataSource("ReportTable01", ldtbReportTable);

                rvViewer.LocalReport.DataSources.Add(lrdsReport);
                byte[] bytes = rvViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string labsRptGenPath = string.Empty;
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);
                string lstrReportFullName = string.Empty;
                lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";

                FileStream fs = new FileStream(@lstrReportFullName,
                   FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }

        /*
        private void OnReportDocInit(object sender, System.EventArgs e)
        {
            RptBatch.SetDatabaseLogon("", "");
        }*/

    }


}
