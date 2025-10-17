using Microsoft.Reporting.WinForms;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.ExceptionPub;
using System.Data;
using Sagitec.BusinessObjects;
using MPIPHP.BusinessObjects;
using System.Collections.ObjectModel;
using MPIPHP.CustomDataObjects;
using Sagitec.DBUtility;
using System.Data.SqlClient;
using Sagitec.Common;
using System.Collections;

namespace MPIPHPJobService
{
    public class busMonthlyPaymentProcess : busBatchHandler
    {


        public DataTable idtPayeeDetails { get; set; }
        //Property to generate correspondence for payees coming in New Payee detail report
        public DataTable idtNewPayees { get; set; }
        //Property to contain minimum guarantee amount for New Payees
        public decimal idecMinimumGuaranteeNewPayees { get; set; }
        //Property to contain minimum guarantee amount for Cancelled or Payments Complete Payees
        public decimal idecMinimumGuaranteeCancelledorPaymentsCompletePayees { get; set; }
        List<string> llstGeneratedCorrespondence = new List<string>();
        List<string> llstGeneratedReports = new List<string>();
        List<string> llstGeneratedFiles = new List<string>();
        busPaymentProcess lobjPaymentProcess = new busPaymentProcess();
        string lstrReportPrefixPaymentScheduleID = string.Empty;
        busBase lobjBase = new busBase();
        int aintNoOfChecksNeeded = 0;
        //Load Next Benefit Payment Date        
        public DateTime idtNextBenefitPaymentDate { get; set; }

        public DataTable idtNewPayee { get; set; }

        //Datatable to contain all Payment details processed for the schedule, used for correspondence
        public DataTable idtPaymentDetails { get; set; }

        //Datatable to contain all non taxable ending Payment details processed for the schedule, used for correspondence
        public DataTable idtNontaxable { get; set; }
        //Load All the payee accounts to be considered for the next payment date
        //Datatable to contain All the payee accounts to be considered for the next payment date, used for correspondence
        public DataTable idtPayeeAccount { get; set; }

        public Collection<busPayeeAccount> iclbBenefitPaymentChangePayeeAccounts { get; set; }


        public Collection<busPayeeAccount> iclbPayeeAccounts { get; set; }
        public void LoadPayeesForPaymentProcess(DateTime adtPaymentScheduleDate,int payment_schedule_id)
        {
            busBase lobjBase = new busBase();
            idtPayeeAccount = busBase.Select("cdoPaymentSchedule.LoadPayeeAccountsForPaymentProcess", new object[1] { adtPaymentScheduleDate.Date});
            iclbPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(idtPayeeAccount, "icdoPayeeAccount");
        }
        public void LoadNextBenefitPaymentDate(string ScheduleType)
        {
            idtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate(ScheduleType).AddMonths(1);
        }
        //Load the Next valid monthly payment schedule into the property ibusPaymentSchedule
        public busPaymentSchedule ibusPaymentSchedule { get; set; }
        public Collection<busPaymentScheduleStep> iclbProcessedSteps { get; set; }
        public int aintLastPaymentScheduleId { get; set; }
        public void LoadPaymentSchedule()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNextBenefitPaymentDate(busConstant.PaymentScheduleTypeMonthly);
            if (ibusPaymentSchedule == null) ibusPaymentSchedule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };
            //ask Abhishek
            DataTable ldtbPaymentSchedule = busBase.Select<cdoPaymentSchedule>(new string[2] { "schedule_type_value", "status_value" },
                                                   new object[2] {busConstant.PaymentScheduleTypeMonthly,busConstant.PaymentScheduleActionStatusReadyforFinal
                                                   }, null, null);
            if (ldtbPaymentSchedule.Rows.Count == 1)
            {
                ibusPaymentSchedule.icdoPaymentSchedule.LoadData(ldtbPaymentSchedule.Rows[0]);
                //for delete backupdata table
                aintLastPaymentScheduleId = ibusPaymentSchedule.GetLastPaymentScheduleDetails(busConstant.PaymentScheduleTypeMonthly);
            }
        }
        public void LoadVendorPaymentSchedule()
        {
            //if (idtNextBenefitPaymentDate == DateTime.MinValue)
            //    LoadNextBenefitPaymentDate(busConstant.PaymentScheduleVendor);
            if (ibusPaymentSchedule == null) ibusPaymentSchedule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };
            //ask Abhishek
            DataTable ldtbPaymentSchedule = busBase.Select<cdoPaymentSchedule>(new string[2] { "schedule_type_value", "status_value" },
                                                   new object[2] {busConstant.PaymentScheduleVendor,busConstant.PaymentScheduleActionStatusReadyforFinal
                                                   }, null, null);
            if (ldtbPaymentSchedule.Rows.Count > 0)
            {
                ibusPaymentSchedule.icdoPaymentSchedule.LoadData(ldtbPaymentSchedule.Rows[0]);
                aintLastPaymentScheduleId = ibusPaymentSchedule.GetLastPaymentScheduleDetails(busConstant.PaymentScheduleVendor);
            }
        }

        public busPaymentSchedule ibusLastPaymentScheule { get; set; }
        public bool LoadPaymentScheduleByPaymentDate(DateTime adtPaymentDate)
        {
            bool lblnResult = false;
            ibusLastPaymentScheule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };
            DataTable ldtPaymentSchedule = busBase.Select<cdoPaymentSchedule>
                (new string[2] { "payment_date", "schedule_type_value" }, new object[2] { adtPaymentDate, busConstant.PaymentScheduleTypeMonthly },
                null, null);
            if (ldtPaymentSchedule.Rows.Count > 0)
            {
                lblnResult = true;
                ibusLastPaymentScheule.icdoPaymentSchedule.LoadData(ldtPaymentSchedule.Rows[0]);
            }
            return lblnResult;
        }
        private void DeleteBackUpTables()
        {
            try
            {
                int lintrtn = 0;
                idlgUpdateProcessLog("Delete back-up data for previous payroll", "INFO", istrProcessName);
                lintrtn = DBFunction.DBNonQuery("cdoPayeeAccount.DeleteBackUpMonthly",
                          new object[1] { aintLastPaymentScheduleId },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //idlgUpdateProcessLog((lintrtn < 0) ? "Delete back-up data failed." + lintrtn.ToString() : "Delete back-up data - Successful." + lintrtn.ToString(), "INFO", istrProcessName);
                idlgUpdateProcessLog("Delete back-up data for previous payroll - Successful.", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Delete back-up data Failed.", "INFO", istrProcessName);
            }
        }
        public void ProcessPayments() 
        {
            istrProcessName = "Monthly Payment Batch";
            bool lblnStepFailedIndicator = false;
            //Load the payment schedule
            if (ibusPaymentSchedule == null)
                LoadPaymentSchedule();
            if (ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id > 0)
            {
                lstrReportPrefixPaymentScheduleID = ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id + "_";
                //update the process start time
                ibusPaymentSchedule.icdoPaymentSchedule.process_date = DateTime.Today;
                ibusPaymentSchedule.icdoPaymentSchedule.process_start_time = DateTime.Now;
                //intialize proccessed steps collection
                iclbProcessedSteps = new Collection<busPaymentScheduleStep>();
                //Load Payment Batch schedule steps
                if (ibusPaymentSchedule != null)
                    if (lobjPaymentProcess.iclbBatchScheduleSteps == null)
                        lobjPaymentProcess.LoadBatchScheduleSteps(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                try
                {
                    iobjPassInfo.BeginTransaction();
                    //Might not Need in MPI (Used in Correspondence)
                    //idtPayeeDetails = busBase.Select("cdoPayeeAccount.LoadPayeeAcountAndStatus",
                    //    new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_date });

                    idtNewPayees = new DataTable();
                    //Loop through the step and process payments
                    if (lobjPaymentProcess.iclbBatchScheduleSteps != null)
                    {
                        foreach (busPaymentScheduleStep lobjPaymentScheduleStep in lobjPaymentProcess.iclbBatchScheduleSteps)
                        {
                            if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 100 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 200)
                            {
                                switch (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence)
                                {
                                    // Load All the payee accounts to be considered for this payment process
                                    case 100:
                                        idlgUpdateProcessLog("Start Payment Process ", "INFO", istrProcessName);
                                        if (iclbPayeeAccounts == null)
                                            LoadPayeesForPaymentProcess(ibusPaymentSchedule.icdoPaymentSchedule.payment_date, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                                        lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                        iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                                        break;
                                }
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 200 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 300)
                            {
                                //Execute all the trial reports
                                if (ExecuteTrialReports() < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 300 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 400)
                            {
                                //Back up data prior to batch
                                if (BackUpData(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 400 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 600)
                            {
                                // Call the methods which are related to creating payment history
                                if (CreatePaymentHistory(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            
                            
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 600 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 700)
                            {
                                // Create Check File
                                if (CreateCheckFile(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 700 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 800)
                            {
                                // Create ACH File
                                if (CreateACHFile(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 900 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1000)
                            {
                                // Call the methods which are related to updating person account information
                                if (UpdatePersonAccountInfo(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1300 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1400)
                            {
                                // Call the methods which are related to creating vendor payment  summary  
                                if (CreateVendorPayment(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1600 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1700)
                            {
                                //Call the methods which are related to Prepare Payee Account for Next Pay Period(updating payee account related tables)
                                if (PreparePayeeAccountForNextPayPeriod(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            //Single step to call all final reports
                             if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence == 1700)
                            {
                                // Call the methods to create 'Final Total Per Items Report' from Payment History
                                if (ExecuteFinalReports() < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            //else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1200 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1300)
                            //{
                            //    // Call the methods which are related to creating Payment register report
                            //    if (ExecuteFinalReports(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                            //    {
                            //        lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                            //        lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                            //        lblnStepFailedIndicator = true;
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                            //    }
                            //    iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            //}
                            
                           
                        }

                        
                    }
                    if (lblnStepFailedIndicator)
                    {
                        idlgUpdateProcessLog("Batch Process Failed", "INFO", istrProcessName);
                        //LoadGeneratedFileInfo();
                        iobjPassInfo.Rollback();
                        //DeleteGeneratedReports();
                       // DeleteGeneratedFiles();
                        llstGeneratedFiles.Clear();
                        llstGeneratedReports.Clear();
                        llstGeneratedCorrespondence.Clear();
                    }
                    else
                    {
                        //To execute reports which doesnt have separate step number
                        //Not Needed ExecuteFinalReports(-1);
                        iobjPassInfo.Commit();
                        llstGeneratedFiles.Clear();
                        llstGeneratedReports.Clear();
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    iobjPassInfo.Rollback();
                    //LoadGeneratedFileInfo();
                    //iobjPassInfo.Rollback();
                    //DeleteGeneratedReports();
                    //DeleteGeneratedFiles();
                    llstGeneratedFiles.Clear();
                    llstGeneratedReports.Clear();
                    idlgUpdateProcessLog("Error Occured with Message = " + e.Message, "INFO", istrProcessName);
                }
                try
                {
                    iobjPassInfo.BeginTransaction();
                    //Update Payment shedule status                    
                    foreach (busPaymentScheduleStep lobjScheduleStep in lobjPaymentProcess.iclbBatchScheduleSteps)
                    {
                        lobjScheduleStep.icdoPaymentScheduleStep.batch_schedule_id = busConstant.MonthlyPaymentBatchScheduleID;
                        lobjScheduleStep.icdoPaymentScheduleStep.Update();
                    }
                    if (lblnStepFailedIndicator)
                    {
                        //temp
                        ibusPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleActionStatusFailed;
                    }
                    else
                    {
                        ibusPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleStatusProcessed;
                        //temp
                        ibusPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleActionStatusProcessed;
                        //to delete back up tables
                        DeleteBackUpTables();
                    }
                    //Wait for one min
                    //System.Threading.Thread.Sleep(1000 * 60);
                    ibusPaymentSchedule.icdoPaymentSchedule.process_end_time = DateTime.Now;
                    ibusPaymentSchedule.icdoPaymentSchedule.Update();
                    iobjPassInfo.Commit();
                    if (lblnStepFailedIndicator)
                    {
                        idlgUpdateProcessLog("Monthly Payment Process Ended with some Failed Step(s)", "INFO", istrProcessName);
                    }
                    else
                    {
                        //Generate Correspondence
                        idlgUpdateProcessLog("Generate Correspondence ", "INFO", istrProcessName);
                        try
                        {
                            //NOTE : Since outside commit, if any correspondence is using idtNextBenefitPaymentDate, need to test whether appropriate date is coming up
                            GenerateCorrespondence();
                            idlgUpdateProcessLog("Correspondence Generated Successfully", "INFO", istrProcessName);
                        }
                        catch (Exception e)
                        {
                            ExceptionManager.Publish(e);
                            idlgUpdateProcessLog("Correspondence Generation Failed.", "INFO", istrProcessName);
                           // DeleteGeneratedCorrespondence();
                            throw e;
                        }
                        llstGeneratedCorrespondence.Clear();
                        idlgUpdateProcessLog("Monthly Payment Process Successful", "INFO", istrProcessName);
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    iobjPassInfo.Rollback();
                    idlgUpdateProcessLog("Error Occured in Updating Schedule Status" + e.Message, "INFO", istrProcessName);
                }
            }
        }
        public void ProcessVendorPayments()
        {
            istrProcessName = "Monthly Payment Batch";
            bool lblnStepFailedIndicator = false;
            //Load the payment schedule
            if (ibusPaymentSchedule == null)
                LoadVendorPaymentSchedule();
            if (ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id > 0)
            {
                lstrReportPrefixPaymentScheduleID = ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id + "_";
                //update the process start time
                ibusPaymentSchedule.icdoPaymentSchedule.process_date = DateTime.Today;
                ibusPaymentSchedule.icdoPaymentSchedule.process_start_time = DateTime.Now;
                //intialize proccessed steps collection
                iclbProcessedSteps = new Collection<busPaymentScheduleStep>();
                //Load Payment Batch schedule steps
                if (ibusPaymentSchedule != null)
                    if (lobjPaymentProcess.iclbBatchScheduleSteps == null)
                        lobjPaymentProcess.LoadBatchScheduleSteps(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                try
                {
                    iobjPassInfo.BeginTransaction();
                    //Might not Need in MPI (Used in Correspondence)
                    //idtPayeeDetails = busBase.Select("cdoPayeeAccount.LoadPayeeAcountAndStatus",
                    //    new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_date });

                    idtNewPayees = new DataTable();
                    //Loop through the step and process payments
                    if (lobjPaymentProcess.iclbBatchScheduleSteps != null)
                    {
                        foreach (busPaymentScheduleStep lobjPaymentScheduleStep in lobjPaymentProcess.iclbBatchScheduleSteps)
                        {
                           
                            if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1300 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1400)
                            {
                                // Call the methods which are related to creating vendor payment  summary  
                                if (UpdateVendorPayment(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                           
                            if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence == 1700)
                            {
                                // Call the methods to create 'Final Total Per Items Report' from Payment History
                                if (ExecuteVendorSummary() < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                           


                        }
                    }
                    if (lblnStepFailedIndicator)
                    {
                        idlgUpdateProcessLog("Batch Process Failed", "INFO", istrProcessName);
                        //LoadGeneratedFileInfo();
                        iobjPassInfo.Rollback();
                        //DeleteGeneratedReports();
                        // DeleteGeneratedFiles();
                        llstGeneratedFiles.Clear();
                        llstGeneratedReports.Clear();
                        llstGeneratedCorrespondence.Clear();
                    }
                    else
                    {
                        //To execute reports which doesnt have separate step number
                        //Not Needed ExecuteFinalReports(-1);
                        iobjPassInfo.Commit();
                        llstGeneratedFiles.Clear();
                        llstGeneratedReports.Clear();
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    iobjPassInfo.Rollback();
                    //LoadGeneratedFileInfo();
                    iobjPassInfo.Rollback();
                    //DeleteGeneratedReports();
                    //DeleteGeneratedFiles();
                    llstGeneratedFiles.Clear();
                    llstGeneratedReports.Clear();
                    idlgUpdateProcessLog("Error Occured with Message = " + e.Message, "INFO", istrProcessName);
                }
                try
                {
                    iobjPassInfo.BeginTransaction();
                    //Update Payment shedule status                    
                    foreach (busPaymentScheduleStep lobjScheduleStep in lobjPaymentProcess.iclbBatchScheduleSteps)
                    {
                        lobjScheduleStep.icdoPaymentScheduleStep.batch_schedule_id = busConstant.MonthlyPaymentBatchScheduleID;
                        lobjScheduleStep.icdoPaymentScheduleStep.Update();
                    }
                    if (lblnStepFailedIndicator)
                    {
                        //temp
                        ibusPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleActionStatusFailed;
                    }
                    else
                    {
                        ibusPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleStatusProcessed;
                        //temp
                        ibusPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleActionStatusProcessed;
                        //to delete back up tables
                        DeleteBackUpTables();
                    }
                    //Wait for one min
                    //System.Threading.Thread.Sleep(1000 * 60);
                    ibusPaymentSchedule.icdoPaymentSchedule.process_end_time = DateTime.Now;
                    ibusPaymentSchedule.icdoPaymentSchedule.Update();
                    iobjPassInfo.Commit();
                    if (lblnStepFailedIndicator)
                    {
                        idlgUpdateProcessLog("Monthly Payment Process Ended with some Failed Step(s)", "INFO", istrProcessName);
                    }
                    else
                    {
                        //Generate Correspondence
                        idlgUpdateProcessLog("Generate Correspondence ", "INFO", istrProcessName);
                        try
                        {
                            //NOTE : Since outside commit, if any correspondence is using idtNextBenefitPaymentDate, need to test whether appropriate date is coming up
                            //GenerateCorrespondence();
                            idlgUpdateProcessLog("Correspondence Generated Successfully", "INFO", istrProcessName);
                        }
                        catch (Exception e)
                        {
                            ExceptionManager.Publish(e);
                            idlgUpdateProcessLog("Correspondence Generation Failed.", "INFO", istrProcessName);
                            // DeleteGeneratedCorrespondence();
                            throw e;
                        }
                        llstGeneratedCorrespondence.Clear();
                        idlgUpdateProcessLog("Monthly Payment Process Successful", "INFO", istrProcessName);
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    iobjPassInfo.Rollback();
                    idlgUpdateProcessLog("Error Occured in Updating Schedule Status" + e.Message, "INFO", istrProcessName);
                }
            }
        }
        //Execute Trial Reports
        public int ExecuteTrialReports()
        {
            int lintrtn = 0;
            busCreateReports lobjCreateReports = new busCreateReports();
            lobjCreateReports.CreateSceduleInfoTble(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.schedule_type_description,ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
            DataTable ldtReportResult;
            DataSet ldsReportResult;
            string lstrReportPath = string.Empty;
            //switch (aintRunSequence)
            //{
            //    case 210:
                    try
                    {
                        //Displays the payment processed by OPUS grouped by each item type.This report is a break-down of amounts that to be paid in the current month. 
                        //This report will be created as part of 'Trial Reports'. 
                        idlgUpdateProcessLog("Trial Monthly Benefit Payment by Item Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentbyItemReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date, true);
                        ldtReportResult.TableName = "rptTrialMonthlyBenefitPaymentbyItemReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {

                            #region Recurring Table
                            DataTable tempMonthlyData = new DataTable();
                            tempMonthlyData = ldtReportResult.Clone();
                            if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "MTLY").Count() > 0)
                            {
                                var MonthlyData = (from obj in ldtReportResult.AsEnumerable()
                                                   where obj.Field<string>("ITEMTYPE") == "MTLY"
                                                   select obj).ToList();

                                foreach (var dtMonthlyData in MonthlyData)
                                {
                                    DataRow drMonthlyData = tempMonthlyData.NewRow();
                                    drMonthlyData["ITEM_DESCRIPTION"] = dtMonthlyData["ITEM_DESCRIPTION"];

                                    if (ldtReportResult.AsEnumerable().Where(o => o.Field<int>("ITEM_PRIORITY") == 19).Count() > 0
                                        && Convert.ToString(dtMonthlyData["ITEM_DESCRIPTION"]) == "Monthly Taxable Amount")
                                    {
                                        drMonthlyData["MPIPP_Amount"] = (dtMonthlyData["MPIPP_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("MPIPP_Amount")) + (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                                 where obj.Field<int>("ITEM_PRIORITY") == 19
                                                                                                                                                                                 select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();

                                        drMonthlyData["L52_Amount"] = (dtMonthlyData["L52_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L52_Amount")) + (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                           where obj.Field<int>("ITEM_PRIORITY") == 19
                                                                                                                                                                           select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                                        drMonthlyData["L161_Amount"] = (dtMonthlyData["L161_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L161_Amount")) + (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                              where obj.Field<int>("ITEM_PRIORITY") == 19
                                                                                                                                                                              select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();

                                        drMonthlyData["L600_Amount"] = (dtMonthlyData["L600_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L600_Amount")) + (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                              where obj.Field<int>("ITEM_PRIORITY") == 19
                                                                                                                                                                              select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                                        drMonthlyData["L666_Amount"] = (dtMonthlyData["L666_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L666_Amount")) + (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                              where obj.Field<int>("ITEM_PRIORITY") == 19
                                                                                                                                                                              select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                                        drMonthlyData["L700_Amount"] = (dtMonthlyData["L700_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L700_Amount")) + (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                              where obj.Field<int>("ITEM_PRIORITY") == 19
                                                                                                                                                                              select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();
                                    }

                                    else if (dtMonthlyData.Field<int>("ITEM_PRIORITY") == 19)
                                    {
                                        drMonthlyData["MPIPP_Amount"] = Decimal.Zero - (dtMonthlyData["MPIPP_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("MPIPP_Amount"));
                                        drMonthlyData["L52_Amount"] = Decimal.Zero - (dtMonthlyData["L52_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L52_Amount"));
                                        drMonthlyData["L161_Amount"] = Decimal.Zero - (dtMonthlyData["L161_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L161_Amount"));
                                        drMonthlyData["L600_Amount"] = Decimal.Zero - (dtMonthlyData["L600_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L600_Amount"));
                                        drMonthlyData["L666_Amount"] = Decimal.Zero - (dtMonthlyData["L666_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L666_Amount"));
                                        drMonthlyData["L700_Amount"] = Decimal.Zero - (dtMonthlyData["L700_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L700_Amount"));
                                    }

                                    else
                                    {
                                        drMonthlyData["MPIPP_Amount"] = dtMonthlyData["MPIPP_AMOUNT"];
                                        drMonthlyData["L52_Amount"] = dtMonthlyData["L52_Amount"];
                                        drMonthlyData["L161_Amount"] = dtMonthlyData["L161_Amount"];
                                        drMonthlyData["L600_Amount"] = dtMonthlyData["L600_Amount"];
                                        drMonthlyData["L666_Amount"] = dtMonthlyData["L666_Amount"];
                                        drMonthlyData["L700_Amount"] = dtMonthlyData["L700_Amount"];
                                    }
                                    drMonthlyData["ITEM_TYPE_DIRECTION"] = 0;
                                    drMonthlyData["ALLOW_ROLLOVER_CODE_VALUE"] = string.Empty;
                                    drMonthlyData["ITEM_PRIORITY"] = dtMonthlyData["ITEM_PRIORITY"];
                                    drMonthlyData["PAYMENT_DATE"] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                                    drMonthlyData["ITEMTYPE"] = string.Empty;
                                    tempMonthlyData.Rows.Add(drMonthlyData);
                                }
                            }

                            //    tempMonthlyData = ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "MTLY").CopyToDataTable();
                            //else
                            //    tempMonthlyData = ldtReportResult.Clone();
                            #endregion

                            #region One Time Table
                            DataTable tempOneTimeData = new DataTable();
                            tempOneTimeData = ldtReportResult.Clone();
                            if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "ONTP").Count() > 0)
                            {
                                var OneTimeData = (from obj in ldtReportResult.AsEnumerable()
                                                   where obj.Field<string>("ITEMTYPE") == "ONTP"
                                                   select obj).ToList();

                                foreach (var dtOneTimeData in OneTimeData)
                                {
                                    //20130721 Added check for ALLOW_ROLLOVER_CODE_VALUE not equal to ROllover item reduction check
                                    if (dtOneTimeData["ALLOW_ROLLOVER_CODE_VALUE"] != DBNull.Value && Convert.ToString(dtOneTimeData["ALLOW_ROLLOVER_CODE_VALUE"]) != busConstant.RolloverItemReductionCheck)
                                    {
                                        DataRow drOneTimeData = tempOneTimeData.NewRow();
                                        drOneTimeData["ITEM_DESCRIPTION"] = dtOneTimeData["ITEM_DESCRIPTION"];

                                        if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover").Count() > 0
                                            && Convert.ToString(dtOneTimeData["ITEM_DESCRIPTION"]) == "One Time Taxable Amount")
                                        {
                                            drOneTimeData["MPIPP_Amount"] = (dtOneTimeData["MPIPP_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("MPIPP_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                     where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                                     select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();

                                            drOneTimeData["L52_Amount"] = (dtOneTimeData["L52_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L52_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                               where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                               select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                                            drOneTimeData["L161_Amount"] = (dtOneTimeData["L161_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L161_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                                  select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();

                                            drOneTimeData["L600_Amount"] = (dtOneTimeData["L600_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L600_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                                  select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                                            drOneTimeData["L666_Amount"] = (dtOneTimeData["L666_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L666_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                                  select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                                            drOneTimeData["L700_Amount"] = (dtOneTimeData["L700_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L700_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                                  select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();
                                        }
                                        //20130721
                                        else if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover").Count() > 0
                                       && Convert.ToString(dtOneTimeData["ITEM_DESCRIPTION"]) == "One Time Non Taxable Amount")
                                        {
                                            drOneTimeData["MPIPP_Amount"] = (dtOneTimeData["MPIPP_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("MPIPP_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                     where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                                     select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();

                                            drOneTimeData["L52_Amount"] = (dtOneTimeData["L52_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L52_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                               where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                               select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                                            drOneTimeData["L161_Amount"] = (dtOneTimeData["L161_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L161_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                                  select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();

                                            drOneTimeData["L600_Amount"] = (dtOneTimeData["L600_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L600_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                                  select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                                            drOneTimeData["L666_Amount"] = (dtOneTimeData["L666_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L666_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                                  select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                                            drOneTimeData["L700_Amount"] = (dtOneTimeData["L700_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L700_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                                  select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();
                                        }

                                        else if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover").Count() > 0
                                            && Convert.ToString(dtOneTimeData["ITEM_DESCRIPTION"]) == "Retiree Increase Taxable Amount")
                                        {
                                            drOneTimeData["MPIPP_Amount"] = (dtOneTimeData["MPIPP_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("MPIPP_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                     where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                                     select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();

                                            drOneTimeData["L52_Amount"] = (dtOneTimeData["L52_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L52_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                               where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                               select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                                            drOneTimeData["L161_Amount"] = (dtOneTimeData["L161_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L161_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                                  select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();

                                            drOneTimeData["L600_Amount"] = (dtOneTimeData["L600_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L600_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                                  select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                                            drOneTimeData["L666_Amount"] = (dtOneTimeData["L666_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L666_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                                  select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                                            drOneTimeData["L700_Amount"] = (dtOneTimeData["L700_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L700_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                                  select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();
                                        }

                                        //else if (dtOneTimeData.Field<int>("ITEM_PRIORITY") == 6)
                                        //{
                                        //    drOneTimeData["MPIPP_Amount"] = Decimal.Zero - (dtOneTimeData["MPIPP_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("MPIPP_Amount"));
                                        //    drOneTimeData["L52_Amount"] = Decimal.Zero - (dtOneTimeData["L52_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L52_Amount"));
                                        //    drOneTimeData["L161_Amount"] = Decimal.Zero - (dtOneTimeData["L161_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L161_Amount"));
                                        //    drOneTimeData["L600_Amount"] = Decimal.Zero - (dtOneTimeData["L600_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L600_Amount"));
                                        //    drOneTimeData["L666_Amount"] = Decimal.Zero - (dtOneTimeData["L666_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L666_Amount"));
                                        //    drOneTimeData["L700_Amount"] = Decimal.Zero - (dtOneTimeData["L700_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L700_Amount"));
                                        //}

                                        else
                                        {
                                            drOneTimeData["MPIPP_Amount"] = dtOneTimeData["MPIPP_AMOUNT"];
                                            drOneTimeData["L52_Amount"] = dtOneTimeData["L52_Amount"];
                                            drOneTimeData["L161_Amount"] = dtOneTimeData["L161_Amount"];
                                            drOneTimeData["L600_Amount"] = dtOneTimeData["L600_Amount"];
                                            drOneTimeData["L666_Amount"] = dtOneTimeData["L666_Amount"];
                                            drOneTimeData["L700_Amount"] = dtOneTimeData["L700_Amount"];
                                        }
                                        drOneTimeData["ITEM_TYPE_DIRECTION"] = 0;
                                        drOneTimeData["ALLOW_ROLLOVER_CODE_VALUE"] = string.Empty;
                                        drOneTimeData["ITEM_PRIORITY"] = dtOneTimeData["ITEM_PRIORITY"];
                                        drOneTimeData["PAYMENT_DATE"] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                                        drOneTimeData["ITEMTYPE"] = string.Empty;
                                        tempOneTimeData.Rows.Add(drOneTimeData);
                                    }
                                }

                            }



                            //if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "ONTP").Count() > 0)
                            //    tempOneTimeData = ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "ONTP").CopyToDataTable();
                            //else
                            //    tempOneTimeData = ldtReportResult.Clone();
                            #endregion

                            #region Gross Amount / Overpayment Reimbursments/ Current Month Pay Amount
                            DataTable tempPensionRecieveData = new DataTable();
                            tempPensionRecieveData = ldtReportResult.Clone();
                            // Gorss Amount
                            DataRow row = tempPensionRecieveData.NewRow();
                            row["ITEM_DESCRIPTION"] = "Gross Amount";
                            row["MPIPP_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                   where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                   select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum()
                                                    +
                                                    (from obj in ldtReportResult.AsEnumerable()
                                                     where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                     select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();


                            row["L52_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                 select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum()
                                                                         +
                                                (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                 select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                            row["L161_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                  where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                  select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum()
                                                   +
                                                (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                 select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();


                            row["L600_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                  where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                  select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum()
                                                  +
                                                (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                 select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                            row["L666_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                  where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                  select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum()
                                                  +
                                                (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                 select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                            row["L700_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                  where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                  select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum()
                                                 +
                                                (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                 select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();

                            row["ITEM_TYPE_DIRECTION"] = 0;
                            row["ALLOW_ROLLOVER_CODE_VALUE"] = string.Empty;
                            row["ITEM_PRIORITY"] = 0;
                            row["PAYMENT_DATE"] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                            row["ITEMTYPE"] = string.Empty;
                            tempPensionRecieveData.Rows.Add(row);

                            //Overpayment Reimbursments
                            if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "PENS").Count() > 0)
                            {
                                var OverpaymentReimbursments = (from obj in ldtReportResult.AsEnumerable()
                                                                where obj.Field<string>("ITEMTYPE") == "PENS"
                                                                select obj).ToList();

                                foreach (var dtrow in OverpaymentReimbursments)
                                {
                                    DataRow dr = tempPensionRecieveData.NewRow();
                                    dr["ITEM_DESCRIPTION"] = "Overpayment Reimbursments";
                                    dr["MPIPP_Amount"] = dtrow["MPIPP_AMOUNT"];
                                    dr["L52_Amount"] = dtrow["L52_Amount"];
                                    dr["L161_Amount"] = dtrow["L161_Amount"];
                                    dr["L600_Amount"] = dtrow["L600_Amount"];
                                    dr["L666_Amount"] = dtrow["L666_Amount"];
                                    dr["L700_Amount"] = dtrow["L700_Amount"];
                                    dr["ITEM_TYPE_DIRECTION"] = 0;
                                    dr["ALLOW_ROLLOVER_CODE_VALUE"] = string.Empty;
                                    dr["ITEM_PRIORITY"] = 0;
                                    dr["PAYMENT_DATE"] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                                    dr["ITEMTYPE"] = string.Empty;
                                    tempPensionRecieveData.Rows.Add(dr);
                                }
                            }
                            #endregion

                            #region State and Fed Tax
                            DataTable tempOtherData = new DataTable();
                            if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "OTHR").Count() > 0)
                                tempOtherData = ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "OTHR").CopyToDataTable();
                            else
                                tempOtherData = ldtReportResult.Clone();
                            #endregion

                            #region Net Amount Data Table
                            DataTable tempNetAmountData = new DataTable();
                            tempNetAmountData = ldtReportResult.Clone();

                            DataRow rowNetAmount = tempNetAmountData.NewRow();
                            rowNetAmount["ITEM_DESCRIPTION"] = "Net Amount";
                            rowNetAmount["MPIPP_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                            select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum()
                                                            +
                                                            (from obj in tempOtherData.AsEnumerable()
                                                             select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();

                            rowNetAmount["L52_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                          select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum()
                                                          +
                                                          (from obj in tempOtherData.AsEnumerable()
                                                           select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                            rowNetAmount["L161_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                           select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum()
                                                           +
                                                           (from obj in tempOtherData.AsEnumerable()
                                                            select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();

                            rowNetAmount["L600_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                           select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum()
                                                           +
                                                           (from obj in tempOtherData.AsEnumerable()
                                                            select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                            rowNetAmount["L666_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                           select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum()
                                                            +
                                                            (from obj in tempOtherData.AsEnumerable()
                                                             select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                            rowNetAmount["L700_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                           select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum()
                                                           +
                                                           (from obj in tempOtherData.AsEnumerable()
                                                            select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();


                            rowNetAmount["ITEM_TYPE_DIRECTION"] = 0;
                            rowNetAmount["ALLOW_ROLLOVER_CODE_VALUE"] = string.Empty;
                            rowNetAmount["ITEM_PRIORITY"] = 0;
                            rowNetAmount["PAYMENT_DATE"] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                            rowNetAmount["ITEMTYPE"] = string.Empty;
                            tempNetAmountData.Rows.Add(rowNetAmount);
                            #endregion

                            #region Grand Total Data Table
                            DataTable tempGrandTotalData = lobjCreateReports.TrialMonthlyBenefitPaymentGrandTotalReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date, true);
                            tempGrandTotalData.DataSet.Tables.Remove(tempGrandTotalData);
                            #endregion

                            #region Dataset and Report Tables
                            DataSet ldtReportData = new DataSet();
                            tempMonthlyData.TableName = "ReportTable01";
                            tempOneTimeData.TableName = "ReportTable02";
                            tempPensionRecieveData.TableName = "ReportTable03";
                            tempOtherData.TableName = "ReportTable04";
                            tempNetAmountData.TableName = "ReportTable05";
                            tempGrandTotalData.TableName = "ReportTable06";

                            ldtReportData.Tables.Add(tempMonthlyData);
                            ldtReportData.Tables.Add(tempOneTimeData);
                            ldtReportData.Tables.Add(tempPensionRecieveData);
                            ldtReportData.Tables.Add(tempOtherData);
                            ldtReportData.Tables.Add(tempNetAmountData);
                            ldtReportData.Tables.Add(tempGrandTotalData);
                            #endregion

                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportData, "rpt7_TrialMonthlyBenefitPaymentSummaryReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Trial Monthly Benefit Payment by Item Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Trial Monthly Benefit Payment by Item Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                //    break;
                //case 220:
                    try
                    {
                        //Displays all the payees who are getting the monthly benefits for the first time.This report will be a break-down of the 'New Payees' from 'Monthly Benefit Payment Summary Report'.
                        //This report will be created as part of 'Trial Reports'. 
                        idlgUpdateProcessLog("New Payee Detail Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialNewRetireeDetailReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        idtNewPayee = ldtReportResult;
                        ldtReportResult.TableName = "rptNewPayeeDetailReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt8_NewPayeeDetailReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("New Payee Detail Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                            //For generating correspondence related to BenefitCalculation
                            //IEnumerable<DataRow> lenuPayee =
                            //    from r1 in idtPayeeDetails.AsEnumerable()
                            //    join r2 in ldtReportResult.AsEnumerable()
                            //    on r1.Field<int>("payee_account_id") equals r2.Field<int>("payee_account_id")
                            //    select r1;
                            //idtNewPayees = lenuPayee.CopyToDataTable();
                            //idecMinimumGuaranteeNewPayees = ldtReportResult.AsEnumerable().Sum(o => o.Field<decimal>("minimum_guarantee_amount"));
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("New Payee Detail Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                //    break;
                //case 230:
                    try
                    {
                        //Displays all the payees whose 'Payee Status' was 'Suspended' received monthly payment prior to last month and did not receive any payments last month.This report will be a break-down for the 'Reinstated Payees' from the 'Monthly Benefit Payment Summary Report'.
                        //This report will be created as part of 'Trial Reports'. 

                        idlgUpdateProcessLog("Reinstated Payee Detail Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialReinstatedRetireeDetailReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptReinstatedPayeeDetailReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt9_ReinstatedPayeeDetailReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Reinstated Payee Detail Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Reinstated Payee Detail Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                //    break;
                //case 240:
                    try
                    {
                        //Report will display all payees whose 'Payee Status' was in 'Receiving' or 'Approved' and received monthly payment effective last 'Payment Cycle Date' and 'Payee Status' not in 'Approved' or 'Receiving' as of 'Current Payment Cycle Date' defined in Payment Schedule. 
                        //This report will be a break-down for the 'Closed/Suspended Payee' from 'Monthly Benefit Payment Summary Report'.
                        //This report will be created as part of 'Trial Reports'. 

                        idlgUpdateProcessLog("Closed/Suspended Payee Account Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialClosedorSuspendedPayeeAccountReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptClosedOrSuspendedPayeeAccountReport";
                        
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt10_ClosedOrSuspendedPayeeAccountReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Closed or Suspended Payee Account Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                            //idecMinimumGuaranteeCancelledorPaymentsCompletePayees =
                            //    ldtReportResult.AsEnumerable().Where(o => o.Field<string>("data2") == busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED
                            //        || o.Field<string>("data2") == busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED)
                            //        .Sum(o => o.Field<decimal>("minimum_guarantee_amount"));
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Closed Or Suspended Payee Account Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                
                    try
                    {
                        idlgUpdateProcessLog("Retirement Option Summary Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialRetirementOptionSummaryReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptRetirementOptionSummaryReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt11_RetirementOptionSummaryReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Retirement Option Summary Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Retirement Option Summary Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        //Report will display all payees with 'Benefit Option' as 'Lumpsum'. This report will be created as part of 'Trial Reports' for Schedule Type as 'Monthly' and 'Adhoc'. 
                        //This report will be a the beak down of 'Current Month One Time' from 'Monthly Benefit Payment Summary Report'.
                        //This report will be created as part of 'Trial Reports'. 
                        idlgUpdateProcessLog("Non-Monthly Payment Detail Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        
                        //Commented by Abhishek on 11/15/2012 -- Need to debug the issue, THIS report is causing the SERVER TO CRASH
                        ldtReportResult = lobjCreateReports.TrialNonMonthlyPaymentDetailReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        //ldtReportResult.TableName = "rptNonMonthlyPaymentDetailReport";
                        ldsReportResult = new DataSet();

                        if (ldtReportResult.Rows.Count > 0)
                        {
                            DataTable ldtTempDataTable01 = new DataTable();
                            DataTable ldtTempDataTable02 = new DataTable();

                            ldtTempDataTable01 = ldtReportResult.Clone();
                            ldtTempDataTable02 = ldtReportResult.Clone();

                            if (((from obj in ldtReportResult.AsEnumerable()
                                  where obj.Field<string>("RETIREE_INCR_FLAG") == busConstant.FLAG_YES
                                  select obj).Count()) > 0)

                                ldtTempDataTable01 = (from obj in ldtReportResult.AsEnumerable()
                                                      where obj.Field<string>("RETIREE_INCR_FLAG") == busConstant.FLAG_YES
                                                      select obj).CopyToDataTable();

                            if (((from obj in ldtReportResult.AsEnumerable()
                                  where obj.Field<string>("RETIREE_INCR_FLAG") != busConstant.FLAG_YES
                                  select obj).Count()) > 0)

                                ldtTempDataTable02 = (from obj in ldtReportResult.AsEnumerable()
                                                      where obj.Field<string>("RETIREE_INCR_FLAG") != busConstant.FLAG_YES
                                                      select obj).CopyToDataTable();

                            ldtTempDataTable01.TableName = "ReportTable01";
                            ldtTempDataTable02.TableName = "ReportTable02";

                            ldsReportResult.Tables.Add(ldtTempDataTable01);
                            ldsReportResult.Tables.Add(ldtTempDataTable02);

                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldsReportResult, "rpt12_NonMonthlyPaymentDetailReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Non-Monthly Payment Detail Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                       
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Non-Monthly Payment Detail Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                ////    break;
                //case 260:
                    try
                    {
                        //Report will display all payees that have any item in their payee account with a different amount when compared the amount in 'Payment History - Details' as of last 'Payment Cycle Date'. 
                        //This report will be a break-down for the 'Changes' from 'Monthly Benefit Payment Summary Report'.
                        //This report will be created as part of 'Trial Reports'. 


                        iclbBenefitPaymentChangePayeeAccounts = new Collection<busPayeeAccount>();
                        idlgUpdateProcessLog("Benefit Payment Change Report", "INFO", istrProcessName);
                        ldsReportResult = new DataSet();
                        ldsReportResult = lobjCreateReports.TrialBenefitPaymentChangeReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        if (ldsReportResult.Tables.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldsReportResult, "rpt13_BenefitPaymentChangeReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Benefit Payment Change Report generated succesfully", "INFO", istrProcessName);

                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Benefit Payment Change Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                //    break;
                //case 270:
                    try
                    {
                        //Report will display the last month's total balance as 'Beginning Balance'. It displays this month's activity and the ending balance.
                        //This report will be created as part of 'Trial Reports'. 
                        idlgUpdateProcessLog("Monthly Benefit Payment Summary Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentSummaryReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptMonthlyBenefitPaymentSummaryReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt14_MonthlyBenefitPaymentReconciliationReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            ibusPaymentSchedule.PostEndingBalance(ldtReportResult, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                            idlgUpdateProcessLog("Monthly Benefit Payment Summary Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Monthly Benefit Payment Summary Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }

                    try
                    {
                        //This report gives the count of participants and survivors grouped by the payee category
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialMonthlyReciepantCountReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptMonthlyRecipientCountReport";

                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt15_MonthlyRecipientCountReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Monthly Recipient Count Report generated succesfully", "INFO", istrProcessName);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Monthly Recipient Count Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                //    break;
                //case 280:
                    
                //    break;
                ////case 290:
                //    try
                //    {
                //        idlgUpdateProcessLog("Vendor Payment Summary Report", "INFO", istrProcessName);
                //        ldtReportResult = new DataTable();
                //        ldtReportResult = lobjCreateReports.TrialVendorPaymentSummary(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                //        if (ldtReportResult.Rows.Count > 0)
                //        {
                //            lstrReportPath = CreatePDFReport(ldtReportResult, "rptVendorPaymentSummary.rpt", lstrReportPrefixPaymentScheduleID);
                //            llstGeneratedReports.Add(lstrReportPath);
                //            idlgUpdateProcessLog("Vendor Payment Summary Report generated succesfully", "INFO", istrProcessName);
                //            lintrtn = 1;
                //        }
                //        else
                //        {
                //            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        ExceptionManager.Publish(e);
                //        idlgUpdateProcessLog("Vendor Payment Summary Report Failed.", "INFO", istrProcessName);
                //        return -1;
                //    }
            //        break;
            //}
            return lintrtn;
        }
        private int ExecuteVendorSummary()
        {
            int lintrtn = 0;
            string lstrReportPath = string.Empty;
            busCreateReports lobjCreateReports = new busCreateReports();
            lobjCreateReports.CreateSceduleInfoTble(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.schedule_sub_type_description);
            DataTable ldtReportResult;
           
            
            
            //try
            //{
            //    //The Report will displays the totals of all deductions paid to the Vendors. This report will be created as part of 'Trial Reports' and 'Final Process'. The source for 'Trial  Reports' and 'Final Process' is different as one takes data from payee account while other takes from 'Payment History'
            //    idlgUpdateProcessLog("Vendor Payment Summary Report", "INFO", istrProcessName);
            //    ldtReportResult = new DataTable();
            //    ldtReportResult = lobjCreateReports.FinalVendorPaymentSummary(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
            //       ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
            //    ldtReportResult.TableName = "rptVendorPaymentSummary";
            //    if (ldtReportResult.Rows.Count > 0)
            //    {
            //        lstrReportPath = CreatePDFReport(ldtReportResult, "rpt4_VendorPaymentSummary", lstrReportPrefixPaymentScheduleID + "FINAL_");
            //        llstGeneratedReports.Add(lstrReportPath);
            //        idlgUpdateProcessLog("Vendor Payment Summary Report generated succesfully", "INFO", istrProcessName);
            //    }
            //    else
            //    {
            //        idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            //    }
            //}
            //catch (Exception e)
            //{
            //    ExceptionManager.Publish(e);
            //    idlgUpdateProcessLog("Vendor Payment Summary Report Failed.", "INFO", istrProcessName);
            //}

            try
            {
                //The Report will displays the totals of all deductions paid to the Vendors. This report will be created as part of 'Trial Reports' and 'Final Process'. The source for 'Trial  Reports' and 'Final Process' is different as one takes data from payee account while other takes from 'Payment History'
                idlgUpdateProcessLog("Vendor Payment Summary Report", "INFO", istrProcessName);
                ldtReportResult = new DataTable();
                if (ibusPaymentSchedule.icdoPaymentSchedule.schedule_sub_type_value == "IAP")
                {
                    ldtReportResult = lobjCreateReports.FindVendorPaymentSummaryStatus(1, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                }
                else
                {
                    ldtReportResult = lobjCreateReports.FindVendorPaymentSummaryStatus(2, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                }
                ldtReportResult.TableName = "rptVendorPaymentSummary";
                if (ldtReportResult.Rows.Count > 0)
                {
                    lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt20_VendorPaymentSummary", lstrReportPrefixPaymentScheduleID + "FINAL_");
                    llstGeneratedReports.Add(lstrReportPath);
                    idlgUpdateProcessLog("Vendor Payment Summary Report generated succesfully", "INFO", istrProcessName);
                }
                else
                {
                    idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Vendor Payment Summary Report Failed.", "INFO", istrProcessName);
                return -1;
            }
            
            return lintrtn;
        }
        
        
        //Method to create Final Reports
        private int ExecuteFinalReports()
        {
            int lintrtn = 0;
            string lstrReportPath = string.Empty;
            busCreateReports lobjCreateReports = new busCreateReports();
            lobjCreateReports.CreateSceduleInfoTble(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.schedule_type_description);
            DataTable ldtReportResult;
            //switch (aintRunSequence)
            //{
            //    case 1000:
                    try
                    {
                        //Report will display the payments processed by the system grouped by each item type under it. This report is part of 'Final Report'.

                        idlgUpdateProcessLog("Final Monthly Benefit Payment Summary Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalMonthlyBenefitPaymentbyItemReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.TableName = "rptFinalMonthlyBenefitPaymentbyItemReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            #region Recurring Table
                            DataTable tempMonthlyData = new DataTable();
                            tempMonthlyData = ldtReportResult.Clone();
                            if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "MTLY").Count() > 0)
                            {
                                var MonthlyData = (from obj in ldtReportResult.AsEnumerable()
                                                   where obj.Field<string>("ITEMTYPE") == "MTLY"
                                                   select obj).ToList();

                                foreach (var dtMonthlyData in MonthlyData)
                                {
                                    DataRow drMonthlyData = tempMonthlyData.NewRow();
                                    drMonthlyData["ITEM_DESCRIPTION"] = dtMonthlyData["ITEM_DESCRIPTION"];

                                    if (ldtReportResult.AsEnumerable().Where(o => o.Field<int>("ITEM_PRIORITY") == 13).Count() > 0
                                        && Convert.ToString(dtMonthlyData["ITEM_DESCRIPTION"]) == "Monthly Taxable Amount")
                                    {
                                        drMonthlyData["MPIPP_Amount"] = (dtMonthlyData["MPIPP_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("MPIPP_Amount")) - (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                                 where obj.Field<int>("ITEM_PRIORITY") == 13
                                                                                                                                                                                 select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();

                                        drMonthlyData["L52_Amount"] = (dtMonthlyData["L52_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L52_Amount")) - (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                           where obj.Field<int>("ITEM_PRIORITY") == 13
                                                                                                                                                                           select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                                        drMonthlyData["L161_Amount"] = (dtMonthlyData["L161_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L161_Amount")) - (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                              where obj.Field<int>("ITEM_PRIORITY") == 13
                                                                                                                                                                              select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();

                                        drMonthlyData["L600_Amount"] = (dtMonthlyData["L600_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L600_Amount")) - (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                              where obj.Field<int>("ITEM_PRIORITY") == 13
                                                                                                                                                                              select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                                        drMonthlyData["L666_Amount"] = (dtMonthlyData["L666_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L666_Amount")) - (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                              where obj.Field<int>("ITEM_PRIORITY") == 13
                                                                                                                                                                              select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                                        drMonthlyData["L700_Amount"] = (dtMonthlyData["L700_Amount"] == DBNull.Value ? 0.0M : dtMonthlyData.Field<decimal>("L700_Amount")) - (from obj in MonthlyData.AsEnumerable()
                                                                                                                                                                              where obj.Field<int>("ITEM_PRIORITY") == 13
                                                                                                                                                                              select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();
                                    }

                                    else
                                    {
                                        drMonthlyData["MPIPP_Amount"] = dtMonthlyData["MPIPP_AMOUNT"];
                                        drMonthlyData["L52_Amount"] = dtMonthlyData["L52_Amount"];
                                        drMonthlyData["L161_Amount"] = dtMonthlyData["L161_Amount"];
                                        drMonthlyData["L600_Amount"] = dtMonthlyData["L600_Amount"];
                                        drMonthlyData["L666_Amount"] = dtMonthlyData["L666_Amount"];
                                        drMonthlyData["L700_Amount"] = dtMonthlyData["L700_Amount"];
                                    }
                                    drMonthlyData["ITEM_TYPE_DIRECTION"] = 0;
                                    drMonthlyData["ALLOW_ROLLOVER_CODE_VALUE"] = string.Empty;
                                    drMonthlyData["ITEM_PRIORITY"] = dtMonthlyData["ITEM_PRIORITY"];
                                    drMonthlyData["PAYMENT_DATE"] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                                    drMonthlyData["ITEMTYPE"] = string.Empty;
                                    tempMonthlyData.Rows.Add(drMonthlyData);
                                }
                            }

                            //    tempMonthlyData = ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "MTLY").CopyToDataTable();
                            //else
                            //    tempMonthlyData = ldtReportResult.Clone();
                            #endregion

                            #region One Time Table
                            DataTable tempOneTimeData = new DataTable();
                            tempOneTimeData = ldtReportResult.Clone();
                            if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "ONTP").Count() > 0)
                            {
                                var OneTimeData = (from obj in ldtReportResult.AsEnumerable()
                                                   where obj.Field<string>("ITEMTYPE") == "ONTP"
                                                   select obj).ToList();

                                foreach (var dtOneTimeData in OneTimeData)
                                {
                                    //20130721 Added check for ALLOW_ROLLOVER_CODE_VALUE not equal to ROllover item reduction check
                                    if (dtOneTimeData["ALLOW_ROLLOVER_CODE_VALUE"] != DBNull.Value && Convert.ToString(dtOneTimeData["ALLOW_ROLLOVER_CODE_VALUE"]) != busConstant.RolloverItemReductionCheck)
                                    {
                                        DataRow drOneTimeData = tempOneTimeData.NewRow();
                                        drOneTimeData["ITEM_DESCRIPTION"] = dtOneTimeData["ITEM_DESCRIPTION"];

                                        if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover").Count() > 0
                                            && Convert.ToString(dtOneTimeData["ITEM_DESCRIPTION"]) == "One Time Taxable Amount")
                                        {
                                            drOneTimeData["MPIPP_Amount"] = (dtOneTimeData["MPIPP_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("MPIPP_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                     where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                                     select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();

                                            drOneTimeData["L52_Amount"] = (dtOneTimeData["L52_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L52_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                               where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                               select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                                            drOneTimeData["L161_Amount"] = (dtOneTimeData["L161_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L161_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                                  select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();

                                            drOneTimeData["L600_Amount"] = (dtOneTimeData["L600_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L600_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                                  select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                                            drOneTimeData["L666_Amount"] = (dtOneTimeData["L666_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L666_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                                  select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                                            drOneTimeData["L700_Amount"] = (dtOneTimeData["L700_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L700_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Taxable Rollover"
                                                                                                                                                                                  select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();
                                        }

                                        //20130721
                                        else if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover").Count() > 0
                                             && Convert.ToString(dtOneTimeData["ITEM_DESCRIPTION"]) == "One Time Non Taxable Amount")
                                        {
                                            drOneTimeData["MPIPP_Amount"] = (dtOneTimeData["MPIPP_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("MPIPP_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                     where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                                     select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();

                                            drOneTimeData["L52_Amount"] = (dtOneTimeData["L52_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L52_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                               where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                               select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                                            drOneTimeData["L161_Amount"] = (dtOneTimeData["L161_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L161_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                                  select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();

                                            drOneTimeData["L600_Amount"] = (dtOneTimeData["L600_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L600_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                                  select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                                            drOneTimeData["L666_Amount"] = (dtOneTimeData["L666_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L666_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                                  select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                                            drOneTimeData["L700_Amount"] = (dtOneTimeData["L700_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L700_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "One Time Non Taxable Rollover"
                                                                                                                                                                                  select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();
                                        }

                                        else if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover").Count() > 0
                                            && Convert.ToString(dtOneTimeData["ITEM_DESCRIPTION"]) == "Retiree Increase Taxable Amount")
                                        {
                                            drOneTimeData["MPIPP_Amount"] = (dtOneTimeData["MPIPP_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("MPIPP_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                     where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                                     select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();

                                            drOneTimeData["L52_Amount"] = (dtOneTimeData["L52_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L52_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                               where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                               select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                                            drOneTimeData["L161_Amount"] = (dtOneTimeData["L161_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L161_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                                  select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();

                                            drOneTimeData["L600_Amount"] = (dtOneTimeData["L600_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L600_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                                  select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                                            drOneTimeData["L666_Amount"] = (dtOneTimeData["L666_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L666_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                                  select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                                            drOneTimeData["L700_Amount"] = (dtOneTimeData["L700_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L700_Amount")) + (from obj in OneTimeData.AsEnumerable()
                                                                                                                                                                                  where obj.Field<string>("ITEM_DESCRIPTION") == "Retiree Increase Taxable Rollover"
                                                                                                                                                                                  select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();
                                        }

                                        //else if (dtOneTimeData.Field<int>("ITEM_PRIORITY") == 6)
                                        //{
                                        //    drOneTimeData["MPIPP_Amount"] = Decimal.Zero - (dtOneTimeData["MPIPP_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("MPIPP_Amount"));
                                        //    drOneTimeData["L52_Amount"] = Decimal.Zero - (dtOneTimeData["L52_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L52_Amount"));
                                        //    drOneTimeData["L161_Amount"] = Decimal.Zero - (dtOneTimeData["L161_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L161_Amount"));
                                        //    drOneTimeData["L600_Amount"] = Decimal.Zero - (dtOneTimeData["L600_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L600_Amount"));
                                        //    drOneTimeData["L666_Amount"] = Decimal.Zero - (dtOneTimeData["L666_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L666_Amount"));
                                        //    drOneTimeData["L700_Amount"] = Decimal.Zero - (dtOneTimeData["L700_Amount"] == DBNull.Value ? 0.0M : dtOneTimeData.Field<decimal>("L700_Amount"));
                                        //}

                                        else
                                        {
                                            drOneTimeData["MPIPP_Amount"] = dtOneTimeData["MPIPP_AMOUNT"];
                                            drOneTimeData["L52_Amount"] = dtOneTimeData["L52_Amount"];
                                            drOneTimeData["L161_Amount"] = dtOneTimeData["L161_Amount"];
                                            drOneTimeData["L600_Amount"] = dtOneTimeData["L600_Amount"];
                                            drOneTimeData["L666_Amount"] = dtOneTimeData["L666_Amount"];
                                            drOneTimeData["L700_Amount"] = dtOneTimeData["L700_Amount"];
                                        }
                                        drOneTimeData["ITEM_TYPE_DIRECTION"] = 0;
                                        drOneTimeData["ALLOW_ROLLOVER_CODE_VALUE"] = string.Empty;
                                        drOneTimeData["ITEM_PRIORITY"] = dtOneTimeData["ITEM_PRIORITY"];
                                        drOneTimeData["PAYMENT_DATE"] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                                        drOneTimeData["ITEMTYPE"] = string.Empty;
                                        tempOneTimeData.Rows.Add(drOneTimeData);
                                    }
                                }

                            }



                            //if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "ONTP").Count() > 0)
                            //    tempOneTimeData = ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "ONTP").CopyToDataTable();
                            //else
                            //    tempOneTimeData = ldtReportResult.Clone();
                            #endregion

                            #region Gross Amount / Overpayment Reimbursments/ Current Month Pay Amount
                            DataTable tempPensionRecieveData = new DataTable();
                            tempPensionRecieveData = ldtReportResult.Clone();
                            // Gorss Amount
                            DataRow row = tempPensionRecieveData.NewRow();
                            row["ITEM_DESCRIPTION"] = "Gross Amount";
                            row["MPIPP_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                   where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                   select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum()
                                                    +
                                                    (from obj in ldtReportResult.AsEnumerable()
                                                     where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                     select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum()
                                                          -                                                                                               //20130721 Subtracting amount with Item Priority 13 for Every Plan
                                                    (from obj in ldtReportResult.AsEnumerable()
                                                     where obj.Field<int>("ITEM_PRIORITY") == 13
                                                     select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();


                            row["L52_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                 select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum()
                                                                         +
                                                (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                 select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum()
                                                      -                                                                                               //20130721 Subtracting amount with Item Priority 13 for Every Plan
                                                    (from obj in ldtReportResult.AsEnumerable()
                                                     where obj.Field<int>("ITEM_PRIORITY") == 13
                                                     select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                            row["L161_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                  where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                  select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum()
                                                   +
                                                (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                 select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum()
                                                  -                                                                                               //20130721 Subtracting amount with Item Priority 13 for Every Plan
                                                    (from obj in ldtReportResult.AsEnumerable()
                                                     where obj.Field<int>("ITEM_PRIORITY") == 13
                                                     select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();


                            row["L600_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                  where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                  select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum()
                                                  +
                                                (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                 select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum()
                                                   -                                                                                               //20130721 Subtracting amount with Item Priority 13 for Every Plan
                                                    (from obj in ldtReportResult.AsEnumerable()
                                                     where obj.Field<int>("ITEM_PRIORITY") == 13
                                                     select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                            row["L666_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                  where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                  select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum()
                                                  +
                                                (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                 select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum()
                                                   -                                                                                               //20130721 Subtracting amount with Item Priority 13 for Every Plan
                                                    (from obj in ldtReportResult.AsEnumerable()
                                                     where obj.Field<int>("ITEM_PRIORITY") == 13
                                                     select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                            row["L700_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                  where obj.Field<int>("ITEM_TYPE_DIRECTION") == 1
                                                  select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum()
                                                 +
                                                (from obj in ldtReportResult.AsEnumerable()
                                                 where obj.Field<string>("allow_rollover_code_value") == busConstant.RolloverItemReductionCheck
                                                 select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum()
                                                   -                                                                                               //20130721 Subtracting amount with Item Priority 13 for Every Plan
                                                    (from obj in ldtReportResult.AsEnumerable()
                                                     where obj.Field<int>("ITEM_PRIORITY") == 13
                                                     select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();

                            row["ITEM_TYPE_DIRECTION"] = 0;
                            row["ALLOW_ROLLOVER_CODE_VALUE"] = string.Empty;
                            row["ITEM_PRIORITY"] = 0;
                            row["PAYMENT_DATE"] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                            row["ITEMTYPE"] = string.Empty;
                            tempPensionRecieveData.Rows.Add(row);

                            //Overpayment Reimbursments
                            if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "PENS").Count() > 0)
                            {
                                var OverpaymentReimbursments = (from obj in ldtReportResult.AsEnumerable()
                                                                where obj.Field<string>("ITEMTYPE") == "PENS"
                                                                select obj).ToList();

                                foreach (var dtrow in OverpaymentReimbursments)
                                {
                                    DataRow dr = tempPensionRecieveData.NewRow();
                                    dr["ITEM_DESCRIPTION"] = "Overpayment Reimbursments";
                                    dr["MPIPP_Amount"] = dtrow["MPIPP_AMOUNT"];
                                    dr["L52_Amount"] = dtrow["L52_Amount"];
                                    dr["L161_Amount"] = dtrow["L161_Amount"];
                                    dr["L600_Amount"] = dtrow["L600_Amount"];
                                    dr["L666_Amount"] = dtrow["L666_Amount"];
                                    dr["L700_Amount"] = dtrow["L700_Amount"];
                                    dr["ITEM_TYPE_DIRECTION"] = 0;
                                    dr["ALLOW_ROLLOVER_CODE_VALUE"] = string.Empty;
                                    dr["ITEM_PRIORITY"] = 0;
                                    dr["PAYMENT_DATE"] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                                    dr["ITEMTYPE"] = string.Empty;
                                    tempPensionRecieveData.Rows.Add(dr);
                                }
                            }
                            #endregion

                            #region State and Fed Tax
                            DataTable tempOtherData = new DataTable();
                            if (ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "OTHR").Count() > 0)
                                tempOtherData = ldtReportResult.AsEnumerable().Where(o => o.Field<string>("ITEMTYPE") == "OTHR").CopyToDataTable();
                            else
                                tempOtherData = ldtReportResult.Clone();
                            #endregion

                            #region Net Amount Data Table
                            DataTable tempNetAmountData = new DataTable();
                            tempNetAmountData = ldtReportResult.Clone();

                            DataRow rowNetAmount = tempNetAmountData.NewRow();
                            rowNetAmount["ITEM_DESCRIPTION"] = "Net Amount";
                            rowNetAmount["MPIPP_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                            select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum()
                                                            +
                                                            (from obj in tempOtherData.AsEnumerable()
                                                             select obj["MPIPP_AMOUNT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("MPIPP_AMOUNT")).Sum();

                            rowNetAmount["L52_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                          select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum()
                                                          +
                                                          (from obj in tempOtherData.AsEnumerable()
                                                           select obj["L52_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L52_Amount")).Sum();

                            rowNetAmount["L161_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                           select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum()
                                                           +
                                                           (from obj in tempOtherData.AsEnumerable()
                                                            select obj["L161_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L161_Amount")).Sum();

                            rowNetAmount["L600_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                           select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum()
                                                           +
                                                           (from obj in tempOtherData.AsEnumerable()
                                                            select obj["L600_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L600_Amount")).Sum();

                            rowNetAmount["L666_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                           select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum()
                                                            +
                                                            (from obj in tempOtherData.AsEnumerable()
                                                             select obj["L666_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L666_Amount")).Sum();

                            rowNetAmount["L700_Amount"] = (from obj in tempPensionRecieveData.AsEnumerable()
                                                           select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum()
                                                           +
                                                           (from obj in tempOtherData.AsEnumerable()
                                                            select obj["L700_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("L700_Amount")).Sum();


                            rowNetAmount["ITEM_TYPE_DIRECTION"] = 0;
                            rowNetAmount["ALLOW_ROLLOVER_CODE_VALUE"] = string.Empty;
                            rowNetAmount["ITEM_PRIORITY"] = 0;
                            rowNetAmount["PAYMENT_DATE"] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                            rowNetAmount["ITEMTYPE"] = string.Empty;
                            tempNetAmountData.Rows.Add(rowNetAmount);
                            #endregion

                            #region Grand Total Data Table
                            DataTable tempGrandTotalData = lobjCreateReports.FinalMonthlyBenefitPaymentbyGrandTotalReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date, 
                                ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                            tempGrandTotalData.DataSet.Tables.Remove(tempGrandTotalData);
                            #endregion

                            #region Dataset and Report Tables
                            DataSet ldtReportData = new DataSet();
                            tempMonthlyData.TableName = "ReportTable01";
                            tempOneTimeData.TableName = "ReportTable02";
                            tempPensionRecieveData.TableName = "ReportTable03";
                            tempOtherData.TableName = "ReportTable04";
                            tempNetAmountData.TableName = "ReportTable05";
                            tempGrandTotalData.TableName = "ReportTable06";

                            ldtReportData.Tables.Add(tempMonthlyData);
                            ldtReportData.Tables.Add(tempOneTimeData);
                            ldtReportData.Tables.Add(tempPensionRecieveData);
                            ldtReportData.Tables.Add(tempOtherData);
                            ldtReportData.Tables.Add(tempNetAmountData);
                            ldtReportData.Tables.Add(tempGrandTotalData);
                            #endregion

                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportData, "rpt1_FinalMonthlyBenefitPaymentSummaryReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Final Monthly Benefit Payment Summary Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Final Monthly Benefit PaymentSummary Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }

                    try
                    {
                        //Report will display the payments processed by the system grouped by each item type under it. This report is part of 'Retiree List Report (Mngt. Report)'.
                        utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                        string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                        idlgUpdateProcessLog("Retiree List", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalRetireeListMangtReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.TableName = "ReportTable01";
                        //RID 71572 No need to add column here. In the main query Added UnionCode and order by union and then name
                        //ldtReportResult.Columns.Add("Occupation", typeof(string));
                        if (ldtReportResult != null && ldtReportResult.Rows.Count > 0)
                        {
                            //foreach (DataRow dr in ldtReportResult.Rows)
                            //{
                            //    SqlParameter[] parameters = new SqlParameter[1];
                            //    SqlParameter param1 = new SqlParameter("@SSN", DbType.String);

                            //    if (dr["DECRYPTED_SSN"] != DBNull.Value)
                            //    {
                            //        param1.Value = Convert.ToString(dr["DECRYPTED_SSN"]);
                            //        parameters[0] = param1;
                            //        DataTable ldtPersonOccupation = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetOccupationBasebOnUnion", lstrLegacyDBConnetion, null, parameters);
                            //        if (ldtPersonOccupation.Rows.Count > 0)
                            //        {
                            //            dr["Occupation"] = Convert.ToString(ldtPersonOccupation.Rows[0]["Name"]);
                            //        }
                            //        else
                            //        {
                            //            dr["Occupation"] = "NO UNION";
                            //        }

                            //    }
                            //}
                            //ldtReportResult.AcceptChanges();

                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt2_RetireeListReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Retiree List Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Retiree List Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                //    break;
                //case 1200:
                    try
                    {
                        //Report will display the payment totals to the payees as part of final payments.
                        idlgUpdateProcessLog("Master Payment Report", "INFO", istrProcessName);
                        DataSet ldtReportData = new DataSet();
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalMasterPaymentReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,true);
                        ldtReportResult.TableName = "rptMasterPaymentReport";

                        ldtReportResult = lobjCreateReports.FinalMasterPaymentReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, true);
                        ldtReportResult.DataSet.Tables.Remove(ldtReportResult);
                        ldtReportResult.TableName = "rptMasterPaymentReport";
                        ldtReportData.Tables.Add(ldtReportResult);

                        ldtReportResult = lobjCreateReports.FinalMasterBenefitPaymentReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.DataSet.Tables.Remove(ldtReportResult);
                        ldtReportResult.TableName = "rptMasterBenefitPaymentReport";
                        ldtReportData.Tables.Add(ldtReportResult);

                        if (ldtReportData.Tables[0].Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportData, "rpt3_MasterPaymentReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Master Payment Report generated succesfully", "INFO", istrProcessName);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Master Payment Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                //    break;
                //case 1300:
                    try
                    {
                        //The Report will displays the totals of all deductions paid to the Vendors. This report will be created as part of 'Trial Reports' and 'Final Process'. The source for 'Trial  Reports' and 'Final Process' is different as one takes data from payee account while other takes from 'Payment History'
                        idlgUpdateProcessLog("Vendor Payment Summary Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalVendorPayment(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                           ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.TableName = "rptVendorPaymentSummary";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt4_VendorPaymentSummary", lstrReportPrefixPaymentScheduleID + "FINAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Vendor Payment Summary Report generated succesfully", "INFO", istrProcessName);
                            //--------------------------------------------------------------
                            string templatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.VENDOR_PAYMENT_SUMMARY_REPORT + ".xlsx";
                            string reportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.VENDOR_PAYMENT_SUMMARY_REPORT + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

                            DataSet dsReportData = new DataSet();
                            dsReportData.Tables.Add(ldtReportResult.Copy());
                            dsReportData.Tables[0].TableName = "ReportTable01";

                            busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                            lbusExcelReportGenerator.CreateExcelReport(templatePath, reportPath, "VENDOR_PAYMENT_SUMMARY", dsReportData);

                            /*
                            ReportViewer rvViewer = new ReportViewer();
                            Warning[] warnings;
                            string[] streamIds;
                            string mimeType = string.Empty;
                            string encoding = string.Empty;
                            string extension = string.Empty;
                            string labsRptDefPath = string.Empty;

                            DataTable ldtbReportTable = ldtReportResult;
                            string lstrReportName = "rpt4_VendorPaymentSummary" + ".rdlc";
                            string lstrPrefix = lstrReportPrefixPaymentScheduleID + "FINAL_";

                            rvViewer.ProcessingMode = ProcessingMode.Local;
                            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

                            rvViewer.LocalReport.ReportPath = labsRptDefPath + lstrReportName;
                            ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);
                            rvViewer.LocalReport.DataSources.Add(lrdsReport);
                            byte[] bytes = rvViewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                            string labsRptGenPath = string.Empty;
                            labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);

                            string lstrReportFullName = string.Empty;

                            if (lstrPrefix.IsNotNullOrEmpty())
                                lstrReportFullName = labsRptGenPath + lstrPrefix + "_" + lstrReportName + "_" +
                                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
                            else
                            {
                                lstrReportFullName = labsRptGenPath + lstrReportName + "_" +
                                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
                            }

                            FileStream fs = new FileStream(@lstrReportFullName, FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                            */
                        //--------------------------------------------------------------
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Vendor Payment Summary Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                //    break;
                //case 1400:
                    try
                    {
                        //Report will display a list of all payees with payment type as 'ACH'. It displays their ACH information, 'Gross Amount', and 'Net Amount' for where the payment has been processed by OPUS. 
                        //This report is part of 'Final Process'.
                        idlgUpdateProcessLog("ACH Register Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalACHRegisterReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.TableName = "rptACHRegisterReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt5_ACHRegisterReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("ACH Register Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("ACH Register Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        //Report will display a list of all payees with payment type as 'Wire'. It displays their Wire information, 'Gross Amount', and 'Net Amount' for where the payment has been processed by OPUS. 
                        //This report is part of 'Final Process'.
                        idlgUpdateProcessLog("Wire Register Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalWireRegisterReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.TableName = "rptWireRegisterReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt5_WIRERegisterReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Wire Register Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Wire Register Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
            //break;
            //case 1405:
            try
                    {
                        //Report will display a list of all payees with 'Payment Type' of 'Check'. It will display the 'Check Number', 'Gross Amount', and 'Net Amount' where the payments has been processed by OPUS. This report is part of 'Final Process'.
                        idlgUpdateProcessLog("Check Register Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalCheckRegisterReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.TableName = "rptCheckRegisterReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt6_CheckRegisterReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Check Register Report generated succesfully", "INFO", istrProcessName);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Cheque Register Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }

            try
                    {
                        //Report will display a list of all payees with payment type as 'Wire'  containing wire tranfer fields for Accounting Team
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalWIRETransferReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.TableName = "rptWIRETransferReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt27_WIRETransferReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("WIRE Transfer Report generated succesfully", "INFO", istrProcessName);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("WIRE Transfer Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
            //    break;
            //case 1410:
            //try
            //{
            //    //The Report will display the list of payees with errors and or that are in 'Review' status

            //    ldtReportResult = new DataTable();
            //    ldtReportResult = lobjCreateReports.FinalErrorReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
            //        ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
            //    ldtReportResult.TableName = "rptPayeeErrorReport";

            //    if (ldtReportResult.Rows.Count > 0)
            //    {
            //        lstrReportPath = CreatePDFReport(ldtReportResult, "rptPayeeErrorReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
            //        llstGeneratedReports.Add(lstrReportPath);
            //        idlgUpdateProcessLog("Payee Error Report generated succesfully", "INFO", istrProcessName);
            //    }
            //    else
            //    {
            //        idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            //    }
            //}
            //catch (Exception e)
            //{
            //    ExceptionManager.Publish(e);
            //    idlgUpdateProcessLog("Payee Error Report Failed.", "INFO", istrProcessName);
            //}
            //    break;
            //case 1415:
            //try
            //{
            // // The Report will display the list of payees with 'Outstanding' cheque
            //    ldtReportResult = new DataTable();
            //    ldtReportResult = lobjCreateReports.FinalOutstanding(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
            //        ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
            //    ldtReportResult.TableName = "rptOutstandingChequeReport";
            //    if (ldtReportResult.Rows.Count > 0)
            //    {
            //        lstrReportPath = CreatePDFReport(ldtReportResult, "rptOutstandingChequeReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
            //        llstGeneratedReports.Add(lstrReportPath);
            //        idlgUpdateProcessLog("Outstanding Cheque Report generated succesfully", "INFO", istrProcessName);
            //    }
            //    else
            //    {
            //        idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            //    }
            //}
            //catch (Exception e)
            //{
            //    ExceptionManager.Publish(e);
            //    idlgUpdateProcessLog("Outstanding Cheque Report Failed.", "INFO", istrProcessName);
            // }
            //    break;
            //case 1420:
            //try
            //{

            //    ldtReportResult = new DataTable();
            //    //ldtReportResult = lobjCreateReports.FinalVendorPaymentSummary(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
            //    //    ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
            //    ldtReportResult.TableName = "rptPayeeErrorReport";
            //    if (ldtReportResult.Rows.Count > 0)
            //    {
            //        lstrReportPath = CreatePDFReport(ldtReportResult, "rptPayeeErrorReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
            //        llstGeneratedReports.Add(lstrReportPath);
            //        idlgUpdateProcessLog("Pay Cheque Report generated succesfully", "INFO", istrProcessName);
            //    }
            //    else
            //    {
            //        idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            //    }
            //}
            //catch (Exception e)
            //{
            //    ExceptionManager.Publish(e);
            //    idlgUpdateProcessLog("Pay Cheque Report Failed.", "INFO", istrProcessName);
            //}
            //    break;
            //case 1425:
            //try
            //{
            //    //This report gives the count of participants and survivors grouped by the payee category
            //    ldtReportResult = new DataTable();
            //    ldtReportResult = lobjCreateReports.FinalMonthlyReciepantCountReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
            //        ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
            //    ldtReportResult.TableName = "rptMonthlyRecipientCountReport";

            //    if (ldtReportResult.Rows.Count > 0)
            //    {
            //        lstrReportPath = CreatePDFReport(ldtReportResult, "rpt15_MonthlyRecipientCountReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
            //        llstGeneratedReports.Add(lstrReportPath);
            //        idlgUpdateProcessLog("Monthly Recipient Count Report generated succesfully", "INFO", istrProcessName);
            //    }
            //    else
            //    {
            //        idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            //    }
            //}
            //catch (Exception e)
            //{
            //    ExceptionManager.Publish(e);
            //    idlgUpdateProcessLog("Monthly Recipient Count Report Failed.", "INFO", istrProcessName);
            //}


            //}
            return lintrtn;
        }
        public int CreatePaymentHistory(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 400:
                    try
                    {
                        //Create Payment History Header for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Creating Payment History Headers", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreatePaymentHistoryHeader(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        aintNoOfChecksNeeded = lintrtn;
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Payment History Header Failed." : "Payment History Headers Created. ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment History Header Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 450:

                    //Create Payment History details for all the payee accounts considered for this payment process
                    try
                    {
                        idlgUpdateProcessLog("Creating Payment History details", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreatePaymentHistoryDetail(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Payment History Details Failed." : "Payment History Details created. ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment History Details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 500:


                    try
                    {
                        //Create ACH Check History for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Creating ACH History for Payees", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateACHHistoryforPayees(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);

                        //Update FBO CO
                        //lobjPaymentProcess.UpdateFBOCO(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        aintNoOfChecksNeeded = aintNoOfChecksNeeded - lintrtn;

                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of ACH Check History details Failed." : "ACH Payment Check History details Created. ", "INFO", istrProcessName);

                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of ACH Check History details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        //Create ACH Check History for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Creating WIRE History for Payees", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateWIREHistoryforPayees(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);

                        //Update FBO CO
                        //lobjPaymentProcess.UpdateFBOCO(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        aintNoOfChecksNeeded = aintNoOfChecksNeeded - lintrtn;

                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Wire Check History details Failed." : "Wire Payment Check History details Created. ", "INFO", istrProcessName);

                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Wire Check History details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        //Create Rollover Check History for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Creating Check History Rollover", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateRollOverACHHistoryforPayees(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        aintNoOfChecksNeeded = aintNoOfChecksNeeded - lintrtn;
                        //Update FBO CO
                        //lobjPaymentProcess.UpdateFBOCO(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);

                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Rollover Check History details Failed." : "Rollover Check History details Created. ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Rollover Check History details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        //Create Payment History Header for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Validate available Checks", "INFO", istrProcessName);
                        int lintAvailableCheck = lobjPaymentProcess.GetAvailableChecks(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        if (lintAvailableCheck < aintNoOfChecksNeeded)
                        {
                            idlgUpdateProcessLog("The Check Book has reached the Maximum Limit/Check book is not available for the payment date.", "INFO", istrProcessName);
                            return -1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment History Header Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        //Create Payment Check History details for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Creating Check History for Payees", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateCheckHistoryforPayees(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);

                        //Update FBO CO
                        lobjPaymentProcess.UpdateCO(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment Check History details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        //Create Payment Check History details for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Creating Payment History for Payees", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateOutstandingHistoryRecords(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment  History status Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                     try
                    {
                        //Create Payment Check History details for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Creating Reimbursement Detail for Payees", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateReimbursementDetail(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Reimbursement Detail status Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    
                    //    break;
                    //case 430:

                    break;
            }
            return lintrtn;
        }
        public int BackUpData(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 300:
                    try
                    {
                        if (!string.IsNullOrEmpty(ibusPaymentSchedule.icdoPaymentSchedule.backup_table_prefix))
                        {
                            idlgUpdateProcessLog("Deleting back up table for the current schedule id", "INFO", istrProcessName);
                            lobjPaymentProcess.DeleteBackUpDataForCurrentScheduleId(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                            idlgUpdateProcessLog("Deleting back up table for the current schedule id Successful.", "INFO", istrProcessName);
                        }
                        idlgUpdateProcessLog("Back-up data Prior to Batch", "INFO", istrProcessName);
                        ibusPaymentSchedule.icdoPaymentSchedule.backup_table_prefix = DBFunction.DBExecuteScalar("cdoPayeeAccount.BackupBeforePayroll",
                                  new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id },
                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework).ToString();
                        idlgUpdateProcessLog((lintrtn < 0) ? "Back-up data Prior to Batch Failed." : "Back-up data - Successful.", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Back-up data Prior to Batch Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        public int CreateCheckFile(int aintRunSequence)
        {
            int lintrtn = 1;
            switch (aintRunSequence)
            {
                case 600:
                    try
                    {
                        //Create Check File
                        idlgUpdateProcessLog("Create Check File", "INFO", istrProcessName);
                        DataTable ldtCheckFile = busBase.Select("cdoPaymentHistoryDistribution.LoadCheckPaymentDistribution", new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                        if (ldtCheckFile.Rows.Count > 0)
                        {
                            //FM upgrade: 6.0.6.2 changes - busProcessFiles should be replaced with busProcessOutboundFile for outbound related method and processing
                            //busProcessFiles lobjProcessFiles = new busProcessFiles();
                            busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                            //Creating File 
                            lobjProcessFiles.iarrParameters = new object[6];
                            
                            lobjProcessFiles.iarrParameters[1] = ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id;
                            lobjProcessFiles.iarrParameters[2] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                            lobjProcessFiles.iarrParameters[3] = true;
                            lobjProcessFiles.iarrParameters[4] = "MPIPPCHECK";
                            DataTable tempCheckData = new DataTable();
                            if (ldtCheckFile.AsEnumerable().Where(o => o.Field<decimal>("NET_AMOUNT") >= 100000).Count() > 0)
                            {
                                tempCheckData = ldtCheckFile.AsEnumerable().Where(o => o.Field<decimal>("NET_AMOUNT") >= 100000).CopyToDataTable();
                                lobjProcessFiles.iarrParameters[0] = tempCheckData;
                                lobjProcessFiles.iarrParameters[5] = "A02B406F11";
                                this.DeleteFile(5, "A02B406F11");
                                lobjProcessFiles.CreateOutboundFile(5);
                            }
                            if (ldtCheckFile.AsEnumerable().Where(o => o.Field<decimal>("NET_AMOUNT") < 100000 &&
                                o.Field<string>("ADDR_COUNTRY_VALUE").IsNotNullOrEmpty()
                               ).Count() > 0)
                            {
                                tempCheckData = ldtCheckFile.AsEnumerable().Where(o => o.Field<decimal>("NET_AMOUNT") < 100000 &&
                                o.Field<string>("ADDR_COUNTRY_VALUE").IsNotNullOrEmpty()
                               ).CopyToDataTable();
                                lobjProcessFiles.iarrParameters[0] = tempCheckData;
                                lobjProcessFiles.iarrParameters[5] = "A01B406F11";
                                this.DeleteFile(5, "A01B406F11");
                                lobjProcessFiles.CreateOutboundFile(5);
                                
                            }
                            if (ldtCheckFile.AsEnumerable().Where(o => o.Field<decimal>("NET_AMOUNT") < 100000 &&
                               o.Field<string>("ADDR_COUNTRY_VALUE").IsNullOrEmpty()
                              ).Count() > 0)
                            {
                                tempCheckData = ldtCheckFile.AsEnumerable().Where(o => o.Field<decimal>("NET_AMOUNT") < 100000 &&
                                o.Field<string>("ADDR_COUNTRY_VALUE").IsNullOrEmpty()
                                ).CopyToDataTable();
                                lobjProcessFiles.iarrParameters[0] = tempCheckData;
                                lobjProcessFiles.iarrParameters[5] = "A03B406F11";
                                this.DeleteFile(5, "A03B406F11");
                                lobjProcessFiles.CreateOutboundFile(5);
                            }
                            //else
                            //    tempCheckData = ldtCheckFile.Clone();


                           
                            idlgUpdateProcessLog("Check File created successfully", "INFO", istrProcessName);
                            //FM upgrade: 6.0.6.2 changes - busProcessFiles should be replaced with busProcessOutboundFile for outbound related method and processing
                            //busProcessFiles lbusProcessFiles = new busProcessFiles();
                            busProcessOutboundFile lbusProcessFiles = new busProcessOutboundFile();
                            if (this.iobjPassInfo.iconFramework.State == ConnectionState.Closed)
                            {
                                this.iobjPassInfo.iconFramework.Open();
                            }
                            //PROD PIR 53
                            lbusProcessFiles.iarrParameters = new object[2];
                            lbusProcessFiles.iarrParameters[0] = ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id;
                            lbusProcessFiles.iarrParameters[1] = "Pension";

                            this.DeleteFile(1008);
                            lbusProcessFiles.CreateOutboundFile(1008);
                            idlgUpdateProcessLog("Check Reconciliation Outbound File Created successfully", "INFO", istrProcessName);
                        }
                        else
                            idlgUpdateProcessLog("No records exist", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Check File Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        public int CreateACHFile(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 700:
                    try
                    {
                        //Create ACH File
                        idlgUpdateProcessLog("Create ACH File", "INFO", istrProcessName);
                        DataTable ldtACHPaymentDistribution = busBase.Select("cdoPaymentHistoryDistribution.LoadACHPaymentDistribution", new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                        if (ldtACHPaymentDistribution.Rows.Count > 0)
                        {
                            //FM upgrade: 6.0.6.2 changes - busProcessFiles should be replaced with busProcessOutboundFile for outbound related method and processing
                            //busProcessFiles lobjProcessFiles = new busProcessFiles();
                            busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                            lobjProcessFiles.iarrParameters = new object[4];
                            lobjProcessFiles.iarrParameters[0] = true;
                            lobjProcessFiles.iarrParameters[1] = ldtACHPaymentDistribution;
                            lobjProcessFiles.iarrParameters[2] = ibusPaymentSchedule.icdoPaymentSchedule.ach_effective_date; //ACH file Fixes if Payment date is holiday
                            lobjProcessFiles.iarrParameters[3] = "MPIPPCHECK";
                            //this.DeleteFile(4, "ACHFile_MPIPPCHECK");
                            this.DeleteFile(4, "xf00.acfhc964.w700.ACH_MPIPPCHECK");
                            lobjProcessFiles.CreateOutboundFile(4);
                            idlgUpdateProcessLog("ACH File created successfully", "INFO", istrProcessName);
                        }
                        else
                            idlgUpdateProcessLog("No records exist", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of ACH File Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        public int UpdatePersonAccountInfo(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 900:
                    //update the plan participation status to withdrawn if the benfit type is 'REFUND' and or Retired if the benefit type is 'Retirement' or
                    //Pre -retirement death
                    try
                    {
                        idlgUpdateProcessLog("Updating Person account Information", "INFO", istrProcessName);
                       lintrtn = lobjPaymentProcess.UpdatePersonAccountStatus(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Person account Information Failed." : "Updating Person account Information Successful .  ", "INFO", istrProcessName);

                        //Reduce the paid amount from the member contributions for the member payee account
                        //not for benefit type disability
                        lintrtn = lobjPaymentProcess.UpdateRetirementContributionForMember(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,true);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Person account Information Failed." : "Updating Person account retirement contribution for member  Successful .  ", "INFO", istrProcessName);

                        //Reduce the paid amount from the member contributions for the alternate payee account
                        //not for benefit type disability
                        //lintrtn = lobjPaymentProcess.UpdateRetirementContributionForAltPayee(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        //idlgUpdateProcessLog((lintrtn < 0) ? "Updating Person account Information Failed." : "Updating Person account retirement contribution for alternate payee Successful .  ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Updating Person account Information Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        public int CreateVendorPayment(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 1300:
                    try
                    {
                        //Create Vendor Payment Summary
                        idlgUpdateProcessLog("Create Vendor Payment Summary", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateVendorPayments(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Vendor Payment Summary Failed." : "Vendor Payment Summary Created. ", "INFO", istrProcessName);
                       
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Create Vendor Payment Summary Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        public int UpdateVendorPayment(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 1300:
                    try
                    {
                        //Create Vendor Payment Summary
                        idlgUpdateProcessLog("Create Vendor Payment Summary", "INFO", istrProcessName);
                        if (ibusPaymentSchedule.icdoPaymentSchedule.schedule_sub_type_value == "IAP")
                        {
                            lintrtn = lobjPaymentProcess.UpdateVendorPayments(1,ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                            
                        }
                        else
                        {
                            lintrtn = lobjPaymentProcess.UpdateVendorPayments(2,ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                            
                        }
                        
                        //lintrtn = lobjPaymentProcess.UpdateVendorPayments(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Vendor Payment Summary Failed." : "Vendor Payment Summary Created. ", "INFO", istrProcessName);

                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Create Vendor Payment Summary Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        public int PreparePayeeAccountForNextPayPeriod(int aintRunSequence)
        {
           
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 1695:
           
           
            
            try
            {
                //Updating Non-Taxable Amount
                idlgUpdateProcessLog("Updating Non-Taxable Amount ", "INFO", istrProcessName);
                lintrtn = lobjPaymentProcess.UpdateNonTaxableAmount(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                idlgUpdateProcessLog((lintrtn < 0) ? "Updating Non-Taxable Amount Failed." : "Updating Non-Taxable Amount Successful . ", "INFO", istrProcessName);

            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Updating payee status to Receiving / Processed Failed.", "INFO", istrProcessName);
                return -1;
            }
            idlgUpdateProcessLog("Update Retro ", "INFO", istrProcessName);
            try
            {
                lintrtn = lobjPaymentProcess.UpdatRetroPayment(ibusPaymentSchedule.icdoPaymentSchedule.payment_date, busConstant.PAYMENT_OPTION_REGULAR, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                
               
                idlgUpdateProcessLog("Update Retro Passed.", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Update Retro Failed.", "INFO", istrProcessName);
                return -1;
            }
            try
            {
                lintrtn = lobjPaymentProcess.UpdatRolloverPayment(ibusPaymentSchedule.icdoPaymentSchedule.payment_date, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);


                idlgUpdateProcessLog("Update Retro Passed.", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Update Retro Failed.", "INFO", istrProcessName);
                return -1;
            }       
            try
            {
                idtNontaxable = busBase.Select("cdoPaymentHistoryDistribution.LoadPayeeAccountsForRecalculatingTax",
                                             new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_date });
                busUpdateTaxAmount lobjUpdateTax = new busUpdateTaxAmount();
                lobjUpdateTax.ReCalculateTax(idtNontaxable, true);
                idlgUpdateProcessLog("Re-calculating Taxes Passed.", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Re-calculating Taxes Failed.", "INFO", istrProcessName);
                return -1;
            }
            break;
            case 1600:
            try
            {
                //we need this
                idlgUpdateProcessLog("Updating Benefit End date for Term Certain Payee Accounts ", "INFO", istrProcessName);
                lintrtn = lobjPaymentProcess.UpdateBenefitEndDateFromMonthlyForTermCertain(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                idlgUpdateProcessLog((lintrtn < 0) ? "Updating Benefit End date for Term Certain Accounts failed." :
                    "Updating Benefit End date for Term Certain Accounts Successful . ", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Updating Benefit End date for Term Certain Accounts Failed.", "INFO", istrProcessName);
                return -1;
            }
           
            try
            {
                idlgUpdateProcessLog("Updating repayment scedule ", "INFO", istrProcessName);
                lintrtn = lobjPaymentProcess.UpdateRepaymentScedule(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                idlgUpdateProcessLog((lintrtn < 0) ? "Updating Payee status to Receiving / Processed Failed." : "Updating Payee status to Receiving / Processed - Successful ", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Updating repayment scedule Failed.", "INFO", istrProcessName);
                return -1;
            }
            try
            {
                idlgUpdateProcessLog("Updating payee status to Receiving / Processed", "INFO", istrProcessName);
                lintrtn = lobjPaymentProcess.UpdatePayeeAccountStatustoPaymentComplete(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                idlgUpdateProcessLog((lintrtn < 0) ? "Updating Payee status to Receiving / Processed Failed." : "Updating Payee status to Receiving / Processed - Successful ", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Updating payee status to Complete Failed.", "INFO", istrProcessName);
                return -1;
            }
            try
            {
                idlgUpdateProcessLog("Updating payee status to Receiving / Processed", "INFO", istrProcessName);
                lintrtn = lobjPaymentProcess.UpdatePayeeAccountStatus(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                idlgUpdateProcessLog((lintrtn < 0) ? "Updating Payee status to Receiving / Processed Failed." : "Updating Payee status to Receiving / Processed - Successful ", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Updating payee status to Receiving / Processed Failed.", "INFO", istrProcessName);
                return -1;
            }
            //Commented code keep it      
            //try
            //{
            //    idlgUpdateProcessLog("Updating Begining Balance", "INFO", istrProcessName);
            //    lintrtn = lobjPaymentProcess.UpdateBeginingBalance(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
            //    idlgUpdateProcessLog((lintrtn < 0) ? "Updating Begining Balance Failed." : "Updating Begining Balance - Successful ", "INFO", istrProcessName);
            //}
            //catch (Exception e)
            //{
            //    ExceptionManager.Publish(e);
            //    idlgUpdateProcessLog("Updating Begining Balance Failed.", "INFO", istrProcessName);
            //    return -1;
            //}
            
            
            break;

                /*//Generate Correspondence
                       idlgUpdateProcessLog("Generate Correspondence ", "INFO", istrProcessName);
                       try
                       {
                           GenerateCorrespondence();
                           idlgUpdateProcessLog("Correspondence Generated Successfully", "INFO", istrProcessName);
                       }
                       catch (Exception e)
                       {
                           ExceptionManager.Publish(e);
                           idlgUpdateProcessLog("Correspondence Generation Failed.", "INFO", istrProcessName);
                           return -1;}*/

            }       
                   
                   

                   
                  
                   
            
            return lintrtn;
        }

        public void ProcessPayeeErrorBatch()
        {
            string lstrReportPath = string.Empty;
            busCreateReports lobjCreateReports = new busCreateReports();
            DataSet ldtReportResult;

            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNextBenefitPaymentDate(busConstant.PaymentScheduleTypeMonthly);

            try
            {
                //The Report will display the list of payees with errors and or that are in 'Review' status
                ldtReportResult = new DataSet();
                //ldtReportResult = lobjCreateReports.FinalPayeeErrorReport(idtNextBenefitPaymentDate);
                ldtReportResult.Tables[0].TableName = "rptPayeeErrorReport";
                ldtReportResult.Tables[1].TableName = "rptPayeeErrorReportSup";

                if (ldtReportResult.Tables[0].Rows.Count > 0 || ldtReportResult.Tables[1].Rows.Count > 0)
                {
                    lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt17_PayeeErrorReport", lstrReportPrefixPaymentScheduleID);
                    llstGeneratedReports.Add(lstrReportPath);
                    idlgUpdateProcessLog("Payee Error Report generated succesfully", "INFO", istrProcessName);
                }
                else
                {
                    idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Payee Error Batch Failed.", "INFO", istrProcessName);
            }
        }
        public void GenerateCorrespondence()
        {
            //idtNewPayee
            //idtPayeeAccount
            if (idtNewPayee != null)
            {
                int[] aintNewPayeeAccountId = (from obj in idtNewPayee.AsEnumerable()
                                               select obj.Field<int>("PAYEE_ACCOUNT_ID")).ToArray();
                Collection<busPayeeAccount> lclbNewPayeeAccount = (from obj in iclbPayeeAccounts.AsEnumerable()
                                                                   where aintNewPayeeAccountId.Contains(obj.icdoPayeeAccount.payee_account_id)
                                                                   select obj).ToList().ToCollection<busPayeeAccount>();
                foreach (busPayeeAccount lbusPayeeAccount in lclbNewPayeeAccount)
                {
                    ArrayList aarrResult = new ArrayList();
                    Hashtable ahtbQueryBkmarks = new Hashtable();
                    lbusPayeeAccount.istrCurrentDate = DateTime.Now.ToString(busConstant.DateFormat);
                    decimal AMOUNT = (from obj in idtNewPayee.AsEnumerable()
                                      where obj.Field<int>("PAYEE_ACCOUNT_ID") == lbusPayeeAccount.icdoPayeeAccount.payee_account_id
                                      select obj.Field<decimal>("MONTHLY_GROSS_AMOUNT")).FirstOrDefault();

                    if (AMOUNT != null)
                    {
                        lbusPayeeAccount.idecNextNetPaymentACH = AMOUNT;
                    }
                    var idtNewPayeeDetail = (from obj in idtPayeeAccount.AsEnumerable()
                                             where obj.Field<int>("PAYEE_ACCOUNT_ID") == lbusPayeeAccount.icdoPayeeAccount.payee_account_id
                                             select new
                                             {
                                                 PAYEE_ACCOUNT_ID = obj.Field<int>("PAYEE_ACCOUNT_ID"),
                                                 RETIREMENT_DATE = obj.Field<DateTime?>("RETIREMENT_DATE"),
                                                 PAYEE_NAME = obj.Field<string>("PAYEE_NAME"),
                                                 PAYEE_FIRST_NAME = obj.Field<string>("PAYEE_FIRST_NAME"),
                                                 PAYEE_LAST_NAME = obj.Field<string>("PAYEE_LAST_NAME"),

                                             }).FirstOrDefault();
                    if (idtNewPayeeDetail != null)
                    {
                        lbusPayeeAccount.istrRetirementDate = idtNewPayeeDetail.RETIREMENT_DATE != null ? Convert.ToDateTime(idtNewPayeeDetail.RETIREMENT_DATE).ToString(busConstant.DateFormat) : "";
                        lbusPayeeAccount.istrRetirementMonthYear = idtNewPayeeDetail.RETIREMENT_DATE != null ? Convert.ToDateTime(idtNewPayeeDetail.RETIREMENT_DATE).Year.ToString() : "";
                        lbusPayeeAccount.istrNextMonthAfterRetirementDate = idtNewPayeeDetail.RETIREMENT_DATE != null ? Convert.ToDateTime(idtNewPayeeDetail.RETIREMENT_DATE).Month.ToString() : "";
                        lbusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                        var idtNewPayeeDetailrow = (from obj in idtPayeeAccount.AsEnumerable()
                                                    where obj.Field<int>("PAYEE_ACCOUNT_ID") == lbusPayeeAccount.icdoPayeeAccount.payee_account_id
                                                    select obj).FirstOrDefault();
                        lbusPayeeAccount.ibusParticipant.icdoPerson.LoadData(idtNewPayeeDetailrow);
                        lbusPayeeAccount.ibusParticipant.icdoPerson.first_name = idtNewPayeeDetail.PAYEE_FIRST_NAME;
                        lbusPayeeAccount.ibusParticipant.icdoPerson.last_name = idtNewPayeeDetail.PAYEE_LAST_NAME;
                        //lbusPayeeAccount.ibusParticipant.icdoPerson.istrFullName = idtNewPayeeDetail.PAYEE_NAME;

                        //Create Correspondence.
                        aarrResult.Add(lbusPayeeAccount);
                        this.CreateCorrespondence(busConstant.CONFIRMATION_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                    }




                }



            }
        }

    }

}

