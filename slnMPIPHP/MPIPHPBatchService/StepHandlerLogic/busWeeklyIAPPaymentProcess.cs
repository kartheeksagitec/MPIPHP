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
using Sagitec.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.ExceptionPub;
using MPIPHP.BusinessObjects;

namespace MPIPHPJobService
{
    public class busWeeklyIAPPaymentProcess : busBatchHandler
    {
        public DataTable idtPayeeDetails { get; set; }
        //Property to generate correspondence for payees coming in New Payee detail report
        public DataTable idtNewPayees { get; set; }
        //Property to contain minimum guarantee amount for New Payees
        public decimal idecMinimumGuaranteeNewPayees { get; set; }
        public bool ablAdhoc { get; set; }
        //Property to contain minimum guarantee amount for Cancelled or Payments Complete Payees
        public decimal idecMinimumGuaranteeCancelledorPaymentsCompletePayees { get; set; }
        List<string> llstGeneratedCorrespondence = new List<string>();
        List<string> llstGeneratedReports = new List<string>();
        List<string> llstGeneratedFiles = new List<string>();
        busPaymentProcess lobjPaymentProcess = new busPaymentProcess(false,false);
        string lstrReportPrefixPaymentScheduleID = string.Empty;
        busBase lobjBase = new busBase();
        int aintNoOfChecksNeeded = 0;
        //Load Next Benefit Payment Date        
        public DateTime idtNextBenefitPaymentDate { get; set; }

        //Datatable to contain all Payment details processed for the schedule, used for correspondence
        public DataTable idtPaymentDetails { get; set; }

        //Datatable to contain all non taxable ending Payment details processed for the schedule, used for correspondence
        public DataTable idtNontaxable { get; set; }
        //Load All the payee accounts to be considered for the next payment date
        //Datatable to contain All the payee accounts to be considered for the next payment date, used for correspondence
        public DataTable idtPayeeAccount { get; set; }
        public DataTable idtNewPayee { get; set; }
        public Collection<busPayeeAccount> iclbBenefitPaymentChangePayeeAccounts { get; set; }

        public busPaymentSchedule ibusLastPaymentScheule { get; set; }
        public Collection<busPayeeAccount> iclbPayeeAccounts { get; set; }


        public void LoadPayeesForPaymentProcess(DateTime adtPaymentScheduleDate, int payment_schedule_id)
        {
            busBase lobjBase = new busBase();
            if(ablAdhoc)
                idtPayeeAccount = busBase.Select("cdoPaymentSchedule.LoadPayeeAccountsForWeeklyAdhocPaymentProcess", new object[1] { adtPaymentScheduleDate.Date });
            else
            idtPayeeAccount = busBase.Select("cdoPaymentSchedule.LoadPayeeAccountsForWeeklyIAPPaymentProcess", new object[1] { adtPaymentScheduleDate.Date});
            iclbPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(idtPayeeAccount, "icdoPayeeAccount");
        }


        public void LoadNextBenefitPaymentDate(string ScheduleType)
        {
            
            idtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate(ScheduleType).AddDays(7);
        }
        //Load the Next valid monthly payment schedule into the property ibusPaymentSchedule
        public busPaymentSchedule ibusPaymentSchedule { get; set; }
        public Collection<busPaymentScheduleStep> iclbProcessedSteps { get; set; }
        public int aintLastPaymentScheduleId { get; set; }

        public void LoadPaymentSchedule()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNextBenefitPaymentDate(busConstant.PaymentScheduleTypeWeekly);
            if (ibusPaymentSchedule == null) ibusPaymentSchedule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };

            DataTable ldtbPaymentSchedule = busBase.Select<cdoPaymentSchedule>(new string[2] { "schedule_type_value", "status_value" },
                                                   new object[2] {busConstant.PaymentScheduleTypeWeekly,busConstant.PaymentScheduleActionStatusReadyforFinal,
                                                   }, null, null);
            DataRow dr = (from obj in ldtbPaymentSchedule.AsEnumerable()
                            where obj.Field<string>("schedule_type_value") == busConstant.PaymentScheduleTypeWeekly
                            select obj).FirstOrDefault();
            if (dr != null)
            {
                ablAdhoc = false;
                ibusPaymentSchedule.icdoPaymentSchedule.LoadData(dr);
                aintLastPaymentScheduleId = ibusPaymentSchedule.GetLastPaymentScheduleDetails(busConstant.PaymentScheduleTypeWeekly);
            }
            else
            {
                 LoadNextBenefitPaymentDate(busConstant.PaymentScheduleAdhocWeekly);
                 if (idtNextBenefitPaymentDate.Date != DateTime.MinValue)
                 {
                     ldtbPaymentSchedule = busBase.Select<cdoPaymentSchedule>(new string[2] { "schedule_type_value", "status_value" },
                                                      new object[2] {busConstant.PaymentScheduleAdhocWeekly,busConstant.PaymentScheduleActionStatusReadyforFinal,
                                                   }, null, null);
                     dr = (from obj in ldtbPaymentSchedule.AsEnumerable()
                           where obj.Field<string>("schedule_type_value") == busConstant.PaymentScheduleAdhocWeekly
                           select obj).FirstOrDefault();
                     if (dr != null)
                     {
                         ablAdhoc = true;
                         ibusPaymentSchedule.icdoPaymentSchedule.LoadData(dr);
                         aintLastPaymentScheduleId = ibusPaymentSchedule.GetLastPaymentScheduleDetails(busConstant.PaymentScheduleAdhocWeekly);
                     }
                 }
             }
            
           
        }


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
            istrProcessName = "Weekly IAP Payment Batch";
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

                #region Generating Payment confirmation letters for IAPHW payees
                if (!lblnStepFailedIndicator)
                {
                    try
                    {
                        DataTable ldtbPayeeAccounts = new DataTable();
                        ldtbPayeeAccounts = busBase.Select("cdoPayeeAccount.GetIAPHW_PayeeAccountListFromTempPaymentTable", new object[0] { });

                        if (ldtbPayeeAccounts != null && ldtbPayeeAccounts.Rows.Count > 0)
                        {
                            iobjPassInfo.BeginTransaction();
                            idlgUpdateProcessLog("Started generating payment confirmation letteres.", "INFO", istrProcessName);
                            foreach (DataRow dr in ldtbPayeeAccounts.Rows)
                            {
                                ArrayList aarrResult = new ArrayList();
                                Hashtable ahtbQueryBkmarks = new Hashtable();
                                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                lbusPayeeAccount.FindPayeeAccount(Convert.ToInt32(dr[enmPayeeAccount.payee_account_id.ToString().ToUpper()]));
                                if (lbusPayeeAccount.ibusParticipant == null)
                                {
                                    lbusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                                    lbusPayeeAccount.ibusParticipant.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id);
                                }
                                lbusPayeeAccount.istrCurrentDate = DateTime.Now.ToString(busConstant.DateFormat);
                                lbusPayeeAccount.LoadGrossAmount();
                                aarrResult.Add(lbusPayeeAccount);

                                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                                foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
                                {
                                    ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
                                }
                                iobjPassInfo.idictParams["ID"] = istrProcessName;
                                this.CreateCorrespondence(busConstant.IAP_HARDSHIP_PAYMENT_CONFIRMATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true);
                            }
                            idlgUpdateProcessLog("Correspondence Generated Successfully", "INFO", istrProcessName);
                            iobjPassInfo.Commit();

                        }
                        istrMergeFilePrefix = ibusPaymentSchedule.icdoPaymentSchedule.payment_date.ToString("MM-dd-yyyy") + "_Confirmation";
                        this.MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + "Correspondence\\Generated\\", iobjSystemManagement.icdoSystemManagement.base_directory + "Correspondence\\IAPHWConfirms\\", false, false, true);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        iobjPassInfo.Rollback();
                        idlgUpdateProcessLog("Error Occured in generating payment confirmation letteres." + e.Message, "INFO", istrProcessName);
                    }
                }
                #endregion

                try
                {
                    iobjPassInfo.BeginTransaction();

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
                        }
                    }

                    //busProcessFiles lbusProcessFiles = new busProcessFiles();
                    //if (this.iobjPassInfo.iconFramework.State == ConnectionState.Closed)
                    //{
                    //    this.iobjPassInfo.iconFramework.Open();
                    //}
                    //lbusProcessFiles.iarrParameters = new object[1];
                    //lbusProcessFiles.iarrParameters[0] = ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id;
                    //lbusProcessFiles.CreateOutboundFile(1008);
                    //idlgUpdateProcessLog("Check Reconciliation Outbound File Created successfully", "INFO", istrProcessName);

                    if (lblnStepFailedIndicator)
                    {
                        idlgUpdateProcessLog("Batch Process Failed", "INFO", istrProcessName);
                        //LoadGeneratedFileInfo();
                        iobjPassInfo.Rollback();
                        //DeleteGeneratedReports();
                        //DeleteGeneratedFiles();
                        llstGeneratedFiles.Clear();
                        llstGeneratedReports.Clear();
                        llstGeneratedCorrespondence.Clear();
                    }
                    else
                    {                        
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
                        //TICKET - 53004
                        //Generate Correspondence
                        //idlgUpdateProcessLog("Generate Correspondence ", "INFO", istrProcessName);
                        //try
                        //{
                        //    //NOTE : Since outside commit, if any correspondence is using idtNextBenefitPaymentDate, need to test whether appropriate date is coming up
                        //    //GenerateCorrespondence();
                        //    //idlgUpdateProcessLog("Correspondence Generated Successfully", "INFO", istrProcessName);
                        //}
                        //catch (Exception e)
                        //{
                        //    ExceptionManager.Publish(e);
                        //    idlgUpdateProcessLog("Correspondence Generation Failed.", "INFO", istrProcessName);
                        //    // DeleteGeneratedCorrespondence();
                        //    throw e;
                        //}
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
            lobjCreateReports.CreateSceduleInfoTble(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.schedule_type_description);
            DataTable ldtReportResult;
            DataSet ldsReportResult;
            string lstrReportPath = string.Empty;

            try
            {
                idlgUpdateProcessLog("Trial Payee List Report", "INFO", istrProcessName);
                ldtReportResult = new DataTable();
                ldtReportResult = lobjCreateReports.TrialPayeeListReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                ldtReportResult.TableName = "rptTrialPayeeListReport";
                if (ldtReportResult.Rows.Count > 0)
                {
                    lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt16_TrialPayeeListReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                    llstGeneratedReports.Add(lstrReportPath);
                    idlgUpdateProcessLog("Trial Payee List Report generated succesfully", "INFO", istrProcessName);
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
                idlgUpdateProcessLog("Trial Payee List Report Failed.", "INFO", istrProcessName);
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

            DataTable ldtTempDataTable = new DataTable();
            ldtTempDataTable.Columns.Add("PAYMENT_METHOD", typeof(string));
            ldtTempDataTable.Columns.Add("Counts", typeof(Int32));
            ldtTempDataTable.Columns.Add("PAYMENT_DATE", typeof(DateTime));
            ldtTempDataTable.Columns.Add("Gross_Amount", Type.GetType("System.Decimal"));
            ldtTempDataTable.Columns.Add("Federal_Tax_Amount", Type.GetType("System.Decimal"));
            ldtTempDataTable.Columns.Add("State_Tax_Amount", Type.GetType("System.Decimal"));
            ldtTempDataTable.Columns.Add("Net_Amount", Type.GetType("System.Decimal"));

            //switch (aintRunSequence)
            //{
            //    case 1000:


            //    break;
            //case 1200:
            //try
            //{
            //    //Report will display the payment totals to the payees as part of final payments.
            //    idlgUpdateProcessLog("Master Payment Report", "INFO", istrProcessName);
            //    ldtReportResult = new DataTable();
            //    ldtReportResult = lobjCreateReports.FinalMasterPaymentReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
            //        ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,false);
            //    ldtReportResult.TableName = "rptMasterPaymentReport";
            //    if (ldtReportResult.Rows.Count > 0)
            //    {
            //        lstrReportPath = CreatePDFReport(ldtReportResult, "rpt3_MasterPaymentReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
            //        llstGeneratedReports.Add(lstrReportPath);
            //        idlgUpdateProcessLog("Master Payment Report generated succesfully", "INFO", istrProcessName);
            //    }
            //    else
            //    {
            //        idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            //    }
            //}
            //catch (Exception e)
            //{
            //    ExceptionManager.Publish(e);
            //    idlgUpdateProcessLog("Master Payment Report Failed.", "INFO", istrProcessName);
            //}
            //busNotes lbusNotes = new busNotes() { icdoNotes = new cdoNotes() };
            //lbusNotes.InsertNotes(lintPersonId, lstrformvalue, lstrNotes, lintorgid);

            try
            {
                int lintorgid = 0;
                int lintPersonId = 0;
                string lstrformvalue = busConstant.PERSON_OVERVIEW_MAINTAINANCE_FORM;
                string lstrNotes = string.Format("The IAP payment requested by the participant has been approved with a payment date of {0:MM/dd/yyyy}. See payee record under Other Details for further information.", ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                DataTable ldtPayeeList = new DataTable();
                ldtPayeeList = busBase.Select("cdoPayeeAccount.GetPersonIdsFromTempPaymentTable", new object[0] { });
                if (ldtPayeeList.Rows.Count > 0)
                {
                    cdoNotes lcdoNotes = new cdoNotes();
                    foreach (DataRow ldr in ldtPayeeList.Rows)
                    {
                        lintPersonId = Convert.ToInt32(ldr["PERSON_ID"]);

                        if (lintPersonId > 0)
                        {
                            lcdoNotes.notes = lstrNotes;
                            lcdoNotes.person_id = lintPersonId;
                            lcdoNotes.org_id = lintorgid;
                            lcdoNotes.form_id = busConstant.Form_ID;
                            lcdoNotes.form_value = lstrformvalue;
                            lcdoNotes.created_by = this.ibusJobHeader.BatchUserID; // iobjPassInfo.istrUserID;
                            lcdoNotes.created_date = DateTime.Now;
                            lcdoNotes.Insert();
                        }
                    }

                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Note insert failed.", "INFO", istrProcessName);
            }

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

                    DataRow drTotalSummary = ldtTempDataTable.NewRow();
                    drTotalSummary["PAYMENT_METHOD"] = "ACH";
                    drTotalSummary["Counts"] = (from obj in ldtReportResult.AsEnumerable()
                                                      select obj["PAYMENT_DATE"]).Count();
                    drTotalSummary["PAYMENT_DATE"] = (from obj in ldtReportResult.AsEnumerable()
                                               select obj["PAYMENT_DATE"]).FirstOrDefault();
                    drTotalSummary["Gross_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                               select obj["Gross_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("Gross_Amount")).Sum()
                    +(from obj in ldtReportResult.AsEnumerable()
                      select obj["PENSION_RECEIVABLE"] == DBNull.Value ? 0.0M : obj.Field<decimal>("PENSION_RECEIVABLE")).Sum()
                    +(from obj in ldtReportResult.AsEnumerable()
                      select obj["CHILDORSPOUSAL_SUPPORT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("CHILDORSPOUSAL_SUPPORT")).Sum()
                    +(from obj in ldtReportResult.AsEnumerable()
                      select obj["TAX_LEVY"] == DBNull.Value ? 0.0M : obj.Field<decimal>("TAX_LEVY")).Sum();

                    drTotalSummary["Federal_Tax_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                     select obj["Federal_Tax_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("Federal_Tax_Amount")).Sum();
                    drTotalSummary["State_Tax_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                   select obj["State_Tax_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("State_Tax_Amount")).Sum();
                    drTotalSummary["Net_Amount"] =  (from obj in ldtReportResult.AsEnumerable()
                                              select obj["Net_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("Net_Amount")).Sum();

                    ldtTempDataTable.Rows.Add(drTotalSummary);
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
            //break;
            //case 1405:
            try
            {
                //Report will display a list of all payees with 'Payment Type' of 'Check'. It will display the 'Check Number', 'Gross Amount', and 'Net Amount' where the payments has been processed by OPUS. This report is part of 'Final Process'.
                idlgUpdateProcessLog("Cheque Register Report", "INFO", istrProcessName);
                ldtReportResult = new DataTable();
                ldtReportResult = lobjCreateReports.FinalCheckRegisterReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                    ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                ldtReportResult.TableName = "rptCheckRegisterReport";
                if (ldtReportResult.Rows.Count > 0)
                {
                    lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt6_CheckRegisterReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
                    llstGeneratedReports.Add(lstrReportPath);
                    idlgUpdateProcessLog("Cheque Register Report generated succesfully", "INFO", istrProcessName);

                    DataRow drTotalSummary = ldtTempDataTable.NewRow();
                    drTotalSummary["PAYMENT_METHOD"] = "CHK";
                    drTotalSummary["Counts"] = (from obj in ldtReportResult.AsEnumerable()
                                                select obj["PAYMENT_DATE"]).Count();
                    drTotalSummary["PAYMENT_DATE"] = (from obj in ldtReportResult.AsEnumerable()
                                                      select obj["PAYMENT_DATE"]).FirstOrDefault();
                    drTotalSummary["Gross_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                      select obj["Gross_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("Gross_Amount")).Sum()
                    + (from obj in ldtReportResult.AsEnumerable()
                       select obj["PENSION_RECEIVABLE"] == DBNull.Value ? 0.0M : obj.Field<decimal>("PENSION_RECEIVABLE")).Sum()
                    + (from obj in ldtReportResult.AsEnumerable()
                       select obj["CHILDORSPOUSAL_SUPPORT"] == DBNull.Value ? 0.0M : obj.Field<decimal>("CHILDORSPOUSAL_SUPPORT")).Sum()
                    + (from obj in ldtReportResult.AsEnumerable()
                       select obj["TAX_LEVY"] == DBNull.Value ? 0.0M : obj.Field<decimal>("TAX_LEVY")).Sum();

                    drTotalSummary["Federal_Tax_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                            select obj["Federal_Tax_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("Federal_Tax_Amount")).Sum();
                    drTotalSummary["State_Tax_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                          select obj["State_Tax_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("State_Tax_Amount")).Sum();
                    drTotalSummary["Net_Amount"] = (from obj in ldtReportResult.AsEnumerable()
                                                    select obj["Net_Amount"] == DBNull.Value ? 0.0M : obj.Field<decimal>("Net_Amount")).Sum();

                    ldtTempDataTable.Rows.Add(drTotalSummary);
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
                //Report will display a list of all payees with 'Payment Type' of 'Check'. It will display the 'Check Number', 'Gross Amount', and 'Net Amount' where the payments has been processed by OPUS. This report is part of 'Final Process'.
                idlgUpdateProcessLog("IAP Summary Report", "INFO", istrProcessName);
                //ldtReportResult = new DataTable();
                //ldtReportResult = lobjCreateReports.FinalIAPSummaryReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                //    ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                
                if (ldtTempDataTable.Rows.Count > 0)
                {
                    ldtTempDataTable.TableName = "ReportTable01";
                    lstrReportPath = lobjCreateReports.CreatePDFReport(ldtTempDataTable, "rpt21_IAPSummaryReport", lstrReportPrefixPaymentScheduleID + "FINAL_");
                    llstGeneratedReports.Add(lstrReportPath);
                    idlgUpdateProcessLog("IAP Summary Report generated succesfully", "INFO", istrProcessName);
                }
                else
                {
                    idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("IAP Summary Report Failed.", "INFO", istrProcessName);
                return -1;
            }
            
            
            
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
                    if (ablAdhoc)
                    {
                        //Payment Adjustment - Reissue
                        try
                        {
                            //Update History Distribution Status To Reissued
                            idlgUpdateProcessLog("Updating old History Distribution Id", "INFO", istrProcessName);
                            lintrtn = lobjPaymentProcess.UpdateOldHistoryDistributionId(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        }
                        catch (Exception e)
                        {
                            ExceptionManager.Publish(e);
                            idlgUpdateProcessLog("Updating old History Distribution Id Failed", "INFO", istrProcessName);
                            return -1;
                        }
                        try
                        {
                            //Update History Distribution Status To Reissued
                            idlgUpdateProcessLog("Updating History Distribution Status To Reissued", "INFO", istrProcessName);
                            lintrtn = lobjPaymentProcess.UpdateHistoryDistributionStatusToReissued(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        }
                        catch (Exception e)
                        {
                            ExceptionManager.Publish(e);
                            idlgUpdateProcessLog("Updating History Distribution Status To Reissued Failed", "INFO", istrProcessName);
                            return -1;
                        }
                        
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
                                lobjProcessFiles.iarrParameters[5] = "A05B406F11";
                                this.DeleteFile(5, "A05B406F11");
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
                                lobjProcessFiles.iarrParameters[5] = "A04B406F11";
                                this.DeleteFile(5, "A04B406F11");
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
                                lobjProcessFiles.iarrParameters[5] = "A06B406F11";
                                this.DeleteFile(5, "A06B406F11");
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
                            lbusProcessFiles.iarrParameters[1] = "IAP";
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
        //public int CreateCheckFile(int aintRunSequence)
        //{
        //    int lintrtn = 1;
        //    switch (aintRunSequence)
        //    {
        //        case 600:
        //            try
        //            {
        //                //Create Check File
        //                idlgUpdateProcessLog("Create Check File", "INFO", istrProcessName);
        //                DataTable ldtCheckFile = busBase.Select("cdoPaymentHistoryDistribution.LoadCheckPaymentDistribution", new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
        //                if (ldtCheckFile.Rows.Count > 0)
        //                {
        //                    busProcessFiles lobjProcessFiles = new busProcessFiles();
        //                    lobjProcessFiles.iarrParameters = new object[5];
        //                    lobjProcessFiles.iarrParameters[0] = ldtCheckFile;
        //                    lobjProcessFiles.iarrParameters[1] = ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id;
        //                    lobjProcessFiles.iarrParameters[2] = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
        //                    lobjProcessFiles.iarrParameters[3] = true;
        //                    lobjProcessFiles.iarrParameters[4] = "IAPCHECK";
        //                    lobjProcessFiles.CreateOutboundFile(5);
        //                    idlgUpdateProcessLog("Check File created successfully", "INFO", istrProcessName);
        //                }
        //                else
        //                    idlgUpdateProcessLog("No records exist", "INFO", istrProcessName);
        //            }
        //            catch (Exception e)
        //            {
        //                ExceptionManager.Publish(e);
        //                idlgUpdateProcessLog("Creation of Check File Failed.", "INFO", istrProcessName);
        //                return -1;
        //            }
        //            break;
        //    }
        //    return lintrtn;
        //}
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
                            lobjProcessFiles.iarrParameters[3] = "IAPCHECK";
                            //this.DeleteFile(4, "ACHFile_IAPCHECK");
                            this.DeleteFile(4, "xf00.acfhc964.w700.ACH_IAPCHECK");
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
                        lintrtn = lobjPaymentProcess.UpdateRetirementContributionForMember(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,false);
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
        public int PreparePayeeAccountForNextPayPeriod(int aintRunSequence)
        {

            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 1695:



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
                    //try
                    //{
                    //    idlgUpdateProcessLog("Updating payee status to Receiving / Processed", "INFO", istrProcessName);
                    //    lintrtn = lobjPaymentProcess.UpdatePayeeAccountStatus(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                    //    idlgUpdateProcessLog((lintrtn < 0) ? "Updating Payee status to Receiving / Processed Failed." : "Updating Payee status to Receiving / Processed - Successful ", "INFO", istrProcessName);
                    //}
                    //catch (Exception e)
                    //{
                    //    ExceptionManager.Publish(e);
                    //    idlgUpdateProcessLog("Updating payee status to Receiving / Processed Failed.", "INFO", istrProcessName);
                    //    return -1;
                    //}
                    try
                    {
                        idlgUpdateProcessLog("Updating payee status to Payments Complete", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdatePayeeAccountStatustoPaymentCompleteIAP(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Payee status to Payments Complete Failed." : "Updating Payee status to Payments Complete - Successful ", "INFO", istrProcessName);

                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Updating payee status to Complete Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    if (ablAdhoc)
                    {
                        try
                        {
                            idlgUpdateProcessLog("Updating payee account Adhoc Flag", "INFO", istrProcessName);
                            lintrtn = lobjPaymentProcess.UpdateAdhocFlag(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                            idlgUpdateProcessLog((lintrtn < 0) ? "Updating Payee status to Payments Complete Failed." : "Updating Payee status to Payments Complete - Successful ", "INFO", istrProcessName);

                        }
                        catch (Exception e)
                        {
                            ExceptionManager.Publish(e);
                            idlgUpdateProcessLog("Updating payee account Adhoc Flag Failed.", "INFO", istrProcessName);
                            return -1;
                        }
                    }



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
        public void GenerateCorrespondence()
        {
            //idtNewPayee
            //idtPayeeAccount
            idtNewPayee = busBase.Select("cdoPaymentHistoryHeader.GetBenefitAmtForCorr", new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
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
                    lbusPayeeAccount.idtNextBenefitPaymentDate = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                    var AMOUNT = (from obj in idtNewPayee.AsEnumerable()
                                  where obj.Field<int>("PAYEE_ACCOUNT_ID") == lbusPayeeAccount.icdoPayeeAccount.payee_account_id
                                  select new
                                      {
                                          BENEFIT_AMOUNT = obj.Field<decimal>("BENEFIT_AMOUNT"),
                                          GROSS_AMOUNT = obj.Field<decimal>("GROSS_AMOUNT"),



                                      }).FirstOrDefault();

                    if (AMOUNT != null)
                    {
                        lbusPayeeAccount.idecNextNetPaymentACH = AMOUNT.BENEFIT_AMOUNT;
                        lbusPayeeAccount.idecIAPAmount = AMOUNT.GROSS_AMOUNT;


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
                                                 BENEFIT_OPTION_VALUE = obj.Field<string>("BENEFIT_OPTION_VALUE"),
                                                 BENEFIT_OPTION_ID = obj.Field<Int32>("BENEFIT_OPTION_ID"),

                                             }).FirstOrDefault();
                    if (idtNewPayeeDetail != null)
                    {

                        lbusPayeeAccount.istrRetroStartDate = idtNewPayeeDetail.RETIREMENT_DATE != null ? Convert.ToDateTime(idtNewPayeeDetail.RETIREMENT_DATE).ToString(busConstant.DateFormat) : "";
                        lbusPayeeAccount.istrRetirementDate = idtNewPayeeDetail.RETIREMENT_DATE != null ? Convert.ToDateTime(idtNewPayeeDetail.RETIREMENT_DATE).ToString(busConstant.DateFormat) : "";
                        lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate = idtNewPayeeDetail.RETIREMENT_DATE != null ? Convert.ToDateTime(idtNewPayeeDetail.RETIREMENT_DATE) : DateTime.MinValue;
                        lbusPayeeAccount.istrRetirementMonthYear = idtNewPayeeDetail.RETIREMENT_DATE != null ? Convert.ToDateTime(idtNewPayeeDetail.RETIREMENT_DATE).Year.ToString() : "";
                        lbusPayeeAccount.istrNextMonthAfterRetirementDate = idtNewPayeeDetail.RETIREMENT_DATE != null ? Convert.ToDateTime(idtNewPayeeDetail.RETIREMENT_DATE).Month.ToString() : "";
                        lbusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                        var idtNewPayeeDetailrow = (from obj in idtPayeeAccount.AsEnumerable()
                                                    where obj.Field<int>("PAYEE_ACCOUNT_ID") == lbusPayeeAccount.icdoPayeeAccount.payee_account_id
                                                    select obj).FirstOrDefault();
                        lbusPayeeAccount.ibusParticipant.icdoPerson.LoadData(idtNewPayeeDetailrow);
                        lbusPayeeAccount.ibusParticipant.icdoPerson.first_name = idtNewPayeeDetail.PAYEE_FIRST_NAME;
                        lbusPayeeAccount.ibusParticipant.icdoPerson.last_name = idtNewPayeeDetail.PAYEE_LAST_NAME;
                        //if (ldtrRow["BENEFIT_OPTION_VALUE"] != DBNull.Value && ldtrRow["BENEFIT_OPTION_ID"] != DBNull.Value)
                        //{
                        lbusPayeeAccount.istrBenefitOptionValue = idtNewPayeeDetail.BENEFIT_OPTION_VALUE;
                        lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue = idtNewPayeeDetail.BENEFIT_OPTION_VALUE;
                        
                        lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption = busGlobalFunctions.GetCodeValueDescriptionByValue(idtNewPayeeDetail.BENEFIT_OPTION_ID, idtNewPayeeDetail.BENEFIT_OPTION_VALUE).description;
                        //}
                        //lbusPayeeAccount.ibusParticipant.icdoPerson.istrFullName = idtNewPayeeDetail.PAYEE_NAME;

                        //Create Correspondence.

                        if (lbusPayeeAccount.ibusParticipant != null)
                        {
                            lbusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                            lbusPayeeAccount.ibusParticipant.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id);
                        }
                        aarrResult.Add(lbusPayeeAccount);
                        this.CreateCorrespondence(busConstant.CONFIRMATION_OF_ANNUITY_PAYMENT_TO_PAYEE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                        this.CreateCorrespondence(busConstant.NOTIFICATION_OF_ANNUITY_PURCHASE_TO_INSURANCE_AGENCY, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                    }




                }



            }
        }

    }
}
