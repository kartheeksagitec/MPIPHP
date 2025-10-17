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
using System.Linq;
using Sagitec.ExceptionPub;
using System.IO;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPaymentSchedule:
    /// Inherited from busPaymentScheduleGen, the class is used to customize the business object busPaymentScheduleGen.
    /// </summary>
    [Serializable]
    public class busPaymentSchedule : busPaymentScheduleGen
    {
        #region Properties
        public Collection<busPaymentScheduleStep> iclbPaymentScheduleStep { get; set; }
        public Collection<busPaymentStepRef> iclbPaymentStepsRef { get; set; }
        public Collection<busPaymentStepRef> iclbPaymentStepRefReloaded { get; set; }

        public decimal idecMinimumGuaranteeNewPayees { get; set; }
        public decimal idecMinimumGuaranteeCancelledorPaymentsCompletePayees { get; set; }

        public DataTable idtPayeeAccount { get; set; }
        public Collection<busPayeeAccount> iclbPayeeAccounts { get; set; }

        private bool iblnIsNewMode = false;



        #endregion

        #region Approve For Final
        public ArrayList btn_ApproveForFinal()
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = new utlError();

            int lintCount = (int)DBFunction.DBExecuteScalar("cdoPaymentSchedule.CheckExistingPaymentScheduleForReadyForFinal", new object[1] {
                                                                this.icdoPaymentSchedule.schedule_type_value },
                                                         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (lintCount > 0)
            {
                lobjError = AddError(6086, "");
                larrErrors.Add(lobjError);
                return larrErrors;

            }

            //WI 19555 - PBV Phase-1 Suspension Batch
            //lintCount = (int)DBFunction.DBExecuteScalar("cdoPaymentSchedule.CheckAnyPayeeAccountShouldBeSuspendedForPensionVerification", new object[0] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //if (icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleTypeMonthly && lintCount > 0)
            //{
            //    lobjError = AddError(6313, "");
            //    larrErrors.Add(lobjError);
            //    //return larrErrors;
            //}

            if (Convert.ToString(MPIPHP.Common.ApplicationSettings.Instance.StateTaxBatchFutureFlag) != "Y")
            {
                lintCount = (int)DBFunction.DBExecuteScalar("cdoPaymentSchedule.CheckForPayeeAccountTaxWithholdingSetup", new object[0] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleTypeMonthly && lintCount > 0)
                {
                    lobjError = AddError(6316, "");
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }

            }

            if (icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleTypeMonthly
                //&& DateTime.Now < icdoPaymentSchedule.payment_date.AddMonths(-1).AddDays(19)
                && DateTime.Now <= icdoPaymentSchedule.payment_setup_cutoff_date
                )
            {
                lobjError = AddError(6038, "Cannot approve for final on or before payment setup cutoff date.");
                larrErrors.Add(lobjError);
                return larrErrors;
            }

            //Refresh payment steps 1.delete existing steps 2. create new steps
            if (iclbPaymentScheduleStep == null)
                LoadPaymentScheduleSteps();
            //foreach (busPaymentScheduleStep lobjStep in iclbPaymentScheduleStep)
            //    lobjStep.icdoPaymentScheduleStep.Delete();

            ////Create New Payment Schedule Steps
            //CreatePaymentSteps(true);

            //Reload Payment schedule steps - No null check bcoz of Reload
            LoadPaymentScheduleSteps();

			//LA Sunset - Payment Directives
            #region Payment Directives

            GeneratePaymentDirectives();
            DBFunction.DBExecuteScalar("cdoPaymentDirectives.RemoveDeletedPaymentAfter36Mnths", new object[] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            
            #endregion Payment Directives

            icdoPaymentSchedule.status_value = busConstant.PaymentScheduleActionStatusReadyforFinal;
            icdoPaymentSchedule.Update();
            larrErrors.Add(this);
            return larrErrors;

        }
		
		//LA Sunset - Payment Directives
        public void GeneratePaymentDirectives()
        {
            busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
            busCreateReports lobjCreateReports = new busCreateReports();
            DataTable ldtApprovedDirectives = busBase.Select("cdoPaymentDirectives.GetApprovedDirectivesForPaymentCycle", new object[1] { icdoPaymentSchedule.payment_date });
            Collection<busPaymentDirectives> lclbPaymentDirectives = new Collection<busPaymentDirectives>();
            lclbPaymentDirectives = GetCollection<busPaymentDirectives>(ldtApprovedDirectives, "icdoPaymentDirectives");

            if (lclbPaymentDirectives.Count > 0)
            {

                foreach (busPaymentDirectives lbusPaymentDirectives in lclbPaymentDirectives)
                {
                    DataSet ldsDataForPaymentDirective = new DataSet();
                    DataTable ldtDataForPaymentDirectives = lbusPayeeAccount.GetDataForPaymentDirectives(lbusPaymentDirectives);
                    ldtDataForPaymentDirectives.TableName = "ReportTable01";

                    ldsDataForPaymentDirective.Tables.Add(ldtDataForPaymentDirectives.Copy());
                    
                    if (lbusPaymentDirectives.icdoPaymentDirectives.payment_directive_type == busConstant.RECURRING_PAYMENT_DIRECTIVE)
                        lobjCreateReports.CreatePDFReport(ldsDataForPaymentDirective, "rptRecurringPaymentDirectives", busConstant.PAYMENT_DIRECTIVES,astrMPIID : lbusPaymentDirectives.icdoPaymentDirectives.payment_cycle_date.ToString("yyyy-MM-dd") + "_" + lbusPaymentDirectives.icdoPaymentDirectives.payee_mpid);
                    else if (lbusPaymentDirectives.icdoPaymentDirectives.payment_directive_type == busConstant.ONETIME_PAYMENT_DIRECTIVE)
                        lobjCreateReports.CreatePDFReport(ldsDataForPaymentDirective, "rptOneTimePaymentDirectives", busConstant.PAYMENT_DIRECTIVES, astrMPIID: lbusPaymentDirectives.icdoPaymentDirectives.payment_cycle_date.ToString("yyyy-MM-dd") + "_" + lbusPaymentDirectives.icdoPaymentDirectives.payee_mpid);
                    else if (lbusPaymentDirectives.icdoPaymentDirectives.payment_directive_type == busConstant.IAP_PAYMENT_DIRECTIVE)
                        lobjCreateReports.CreatePDFReport(ldsDataForPaymentDirective, "rptIAPPaymentDirectives", busConstant.PAYMENT_DIRECTIVES, astrMPIID: lbusPaymentDirectives.icdoPaymentDirectives.payment_cycle_date.ToString("yyyy-MM-dd") + "_" + lbusPaymentDirectives.icdoPaymentDirectives.payee_mpid);

                }
            }
        }

        #endregion

        #region Cancel Button
        public ArrayList btn_Cancel()
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = new utlError();

            if (this.icdoPaymentSchedule.payment_schedule_id != 0)
            {
                this.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleActionStatusCancelled;
                this.icdoPaymentSchedule.Update();
            }

            larrErrors.Add(this);
            return larrErrors;
        }
        #endregion

        #region Run Trial Reports
        public ArrayList btn_RunTrialReports()
        {
            ArrayList larrErrors = new ArrayList();
            int lintrtn = 0;
            lintrtn = RunTrialReports();
            //Update the status only if all reports executed successfully
            if (lintrtn != -1)
            {
                icdoPaymentSchedule.status_value = busConstant.PaymentScheduleActionStatusTrialExecuted;
                icdoPaymentSchedule.Update();
            }
            larrErrors.Add(this);
            return larrErrors;
        }

        public void GenerateTrialPaymentDirectives(string rptGenPath)
        {
            busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
            busCreateReports lobjCreateReports = new busCreateReports();
            DataTable ldtApprovedDirectives = busBase.Select("cdoPaymentDirectives.GetApprovedDirectivesForPaymentCycle", new object[1] { icdoPaymentSchedule.payment_date });
            Collection<busPaymentDirectives> lclbPaymentDirectives = new Collection<busPaymentDirectives>();
            lclbPaymentDirectives = GetCollection<busPaymentDirectives>(ldtApprovedDirectives, "icdoPaymentDirectives");

            if (lclbPaymentDirectives.Count > 0)
            {
                foreach (busPaymentDirectives lbusPaymentDirectives in lclbPaymentDirectives)
                {
                    DataSet ldsDataForPaymentDirective = new DataSet();
                    DataTable ldtDataForPaymentDirectives = lbusPayeeAccount.GetDataForPaymentDirectives(lbusPaymentDirectives);
                    ldtDataForPaymentDirectives.TableName = "ReportTable01";

                    ldsDataForPaymentDirective.Tables.Add(ldtDataForPaymentDirectives.Copy());

                    if (lbusPaymentDirectives.icdoPaymentDirectives.payment_directive_type == busConstant.IAP_PAYMENT_DIRECTIVE)
                        lobjCreateReports.CreatePDFReport(ldsDataForPaymentDirective, "rptIAPPaymentDirectives", astrMPIID: lbusPaymentDirectives.icdoPaymentDirectives.payment_cycle_date.ToString("yyyy-MM-dd") + "_" + lbusPaymentDirectives.icdoPaymentDirectives.payee_mpid, outputFolderPath : rptGenPath);
                }
            }
        }
        #endregion

        //LA Sunset - Payment Directives
        public ArrayList btn_PaymentDirectiveExceptionReport()
        {
            ArrayList larrErrors = new ArrayList();            
            busCreateReports lobjCreateReports = new busCreateReports();

            try
            {
                DataTable ldtReportResult = new DataTable();
                ldtReportResult = lobjCreateReports.TrialNewRetireeDetailReport(icdoPaymentSchedule.payment_date);
                ldtReportResult.TableName = "rptNewPayeeDetailReport";
                if (ldtReportResult.Rows.Count > 0)
                {
                    //lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt8_NewPayeeDetailReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                   // lintrtn = 1;
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
            }

            larrErrors.Add(this);
            return larrErrors;
        }

        #region Load All the Payment Steps
        public void LoadPaymentSteps()
        {
            DataTable ldtbPaymentStepsRef = iobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_step_ref", null);
            iclbPaymentStepsRef = GetCollection<busPaymentStepRef>(ldtbPaymentStepsRef, "icdoPaymentStepRef");
        }
        #endregion

        #region Load all payment steps which are active for the payment schedule
        public void LoadPaymentScheduleSteps()
        {
            iclbPaymentScheduleStep = new Collection<busPaymentScheduleStep>();
            DataTable ldtbScheduleSteps = Select("cdoPaymentSchedule.LoadPaymentScheduleSteps", new object[1] { icdoPaymentSchedule.payment_schedule_id });

            foreach (DataRow drStep in ldtbScheduleSteps.Rows)
            {
                busPaymentScheduleStep lobjPaymentScheduleStep = new busPaymentScheduleStep { icdoPaymentScheduleStep = new cdoPaymentScheduleStep() };
                lobjPaymentScheduleStep.ibusPaymentStepRef = new busPaymentStepRef { icdoPaymentStepRef = new cdoPaymentStepRef() };
                lobjPaymentScheduleStep.icdoPaymentScheduleStep.LoadData(drStep);
                lobjPaymentScheduleStep.ibusPaymentStepRef.icdoPaymentStepRef.LoadData(drStep);
                //step name and Flag
                lobjPaymentScheduleStep.icdoPaymentScheduleStep.istrStepName = lobjPaymentScheduleStep.ibusPaymentStepRef.icdoPaymentStepRef.step_name;
                lobjPaymentScheduleStep.icdoPaymentScheduleStep.istrActiveFlag = lobjPaymentScheduleStep.ibusPaymentStepRef.icdoPaymentStepRef.active_flag;

                iclbPaymentScheduleStep.Add(lobjPaymentScheduleStep);
            }
        }
        #endregion

        #region Reload or Refresh All the Payment Steps
        public void ReLoadPaymentSteps()
        {
            DataTable ldtbPaymentSteps = Select("cdoPaymentSchedule.ReloadPaymentStepRef",
                new object[2] { icdoPaymentSchedule.payment_schedule_id, icdoPaymentSchedule.schedule_type_value });
            iclbPaymentStepRefReloaded = GetCollection<busPaymentStepRef>(ldtbPaymentSteps, "icdoPaymentStepRef");
        }
        #endregion

        #region Create payment steps which are active for the payment schedule
        public void CreatePaymentSteps(bool ablnApproved)
        {
            //if approve button is clicked from the screen refresh the payment step ref collection, so that currently active steps will inserted into payment schedule steps
            if (ablnApproved && iclbPaymentStepRefReloaded == null)
                ReLoadPaymentSteps();
            // On creation of payment schedule steps ,take all the active steps from cache and insert into  payment schedule steps
            if (!ablnApproved && iclbPaymentStepsRef == null)
                LoadPaymentSteps();

            Collection<busPaymentStepRef> lclbPaymentStepRef = ablnApproved ? iclbPaymentStepRefReloaded : iclbPaymentStepsRef;
            if (iclbPaymentScheduleStep == null)
            {

                if (lclbPaymentStepRef.Count > 0)
                {
                    lclbPaymentStepRef = lclbPaymentStepRef.Where(o => o.icdoPaymentStepRef.active_flag == busConstant.Flag_Yes && o.icdoPaymentStepRef.schedule_type_value == icdoPaymentSchedule.schedule_type_value).ToList().ToCollection();

                    foreach (busPaymentStepRef lobjPaymentStep in lclbPaymentStepRef)//.Where(o => o.icdoPaymentStepRef.active_flag == busConstant.Flag_Yes))
                    {
                        busPaymentScheduleStep lobjPaymentScheduleStep = new busPaymentScheduleStep { icdoPaymentScheduleStep = new cdoPaymentScheduleStep() };
                        lobjPaymentScheduleStep.icdoPaymentScheduleStep.payment_schedule_id = icdoPaymentSchedule.payment_schedule_id;
                        lobjPaymentScheduleStep.icdoPaymentScheduleStep.payment_step_id = lobjPaymentStep.icdoPaymentStepRef.payment_step_id;
                        lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusPending;
                        lobjPaymentScheduleStep.icdoPaymentScheduleStep.Insert();
                    }

                }
            }
        }
        #endregion

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (aenmPageMode == utlPageMode.New)
            {
                iblnIsNewMode = true;
            }
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            //this.icdoPaymentSchedule.schedule_sub_type_id = 7034;

            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (iblnIsNewMode)
                CreatePaymentSteps(false);
            LoadPaymentScheduleSteps();
            EvaluateInitialLoadRules();
        }

        #region Method to run Create Trial reports
        /// <param name="aclbPaymentSteps">Payment step ref </param>
        /// <returns>int value</returns>
        private int RunTrialReports()
        {

            int lintrtn = 0;
            busCreateReports lobjCreateReports;

            if (icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleAdhocMonthly || icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleAdhocWeekly)
                lobjCreateReports = new busCreateReports(true, false);
            else
                lobjCreateReports = new busCreateReports();
            lobjCreateReports.CreateSceduleInfoTble(icdoPaymentSchedule.payment_schedule_id, icdoPaymentSchedule.schedule_type_description, icdoPaymentSchedule.payment_date);
            DataTable ldtReportResult;
            DataSet ldsReportResult;
            string lstrReportPath = string.Empty;
            string lstrReportPrefixPaymentScheduleID = Convert.ToString(icdoPaymentSchedule.payment_schedule_id) + "_";

            switch (icdoPaymentSchedule.schedule_type_value)
            {
                case busConstant.PaymentScheduleTypeMonthly:
                    LoadPayeesForMonthlyPaymentProcess(icdoPaymentSchedule.payment_date, icdoPaymentSchedule.payment_schedule_id);
                    try
                    {
                        //Displays the payment processed by OPUS grouped by each item type.This report is a break-down of amounts that to be paid in the current month. 
                        //This report will be created as part of ‘Trial Reports’. 
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentbyItemReport(icdoPaymentSchedule.payment_date, true);
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
                                    drMonthlyData["PAYMENT_DATE"] = icdoPaymentSchedule.payment_date;
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
                                        drOneTimeData["PAYMENT_DATE"] = icdoPaymentSchedule.payment_date;
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
                            row["PAYMENT_DATE"] = icdoPaymentSchedule.payment_date;
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
                                    dr["PAYMENT_DATE"] = icdoPaymentSchedule.payment_date;
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
                            rowNetAmount["PAYMENT_DATE"] = icdoPaymentSchedule.payment_date;
                            rowNetAmount["ITEMTYPE"] = string.Empty;
                            tempNetAmountData.Rows.Add(rowNetAmount);
                            #endregion

                            #region Grand Total Data Table
                            DataTable tempGrandTotalData = lobjCreateReports.TrialMonthlyBenefitPaymentGrandTotalReport(icdoPaymentSchedule.payment_date, true);
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
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }

                    try
                    {
                        //Displays all the payees who are getting the monthly benefits for the first time.This report will be a break-down of the ‘New Payees’ from ‘Monthly Benefit Payment Summary Report’.
                        //This report will be created as part of ‘Trial Reports’. 
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialNewRetireeDetailReport(icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptNewPayeeDetailReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt8_NewPayeeDetailReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }

                    try
                    {
                        //Displays all the payees whose ‘Payee Status’ was ‘Suspended’ received monthly payment prior to last month and did not receive any payments last month.This report will be a break-down for the ‘Reinstated Payees’ from the ‘Monthly Benefit Payment Summary Report’.
                        //This report will be created as part of ‘Trial Reports’. 
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialReinstatedRetireeDetailReport(icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptReinstatedPayeeDetailReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt9_ReinstatedPayeeDetailReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }

                    try
                    {
                        //Report will display all payees whose ‘Payee Status’ was in ‘Receiving’ or ‘Approved’ and received monthly payment effective last ‘Payment Cycle Date’ and ‘Payee Status’ not in ‘Approved’ or ‘Receiving’ as of ‘Current Payment Cycle Date’ defined in Payment Schedule. 
                        //This report will be a break-down for the ‘Closed/Suspended Payee’ from ‘Monthly Benefit Payment Summary Report’.
                        //This report will be created as part of ‘Trial Reports’. 

                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialClosedorSuspendedPayeeAccountReport(icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptClosedOrSuspendedPayeeAccountReport";

                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt10_ClosedOrSuspendedPayeeAccountReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }

                    try
                    {
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialRetirementOptionSummaryReport(icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptRetirementOptionSummaryReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt11_RetirementOptionSummaryReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }
                    try
                    {
                        //Report will display all payees with ‘Benefit Option’ as ‘Lumpsum’. This report will be created as part of ‘Trial Reports’ for Schedule Type as ‘Monthly’ and ‘Adhoc’. 
                        //This report will be a the beak down of ‘Current Month One Time’ from ‘Monthly Benefit Payment Summary Report’.
                        //This report will be created as part of ‘Trial Reports’. 
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialNonMonthlyPaymentDetailReport(icdoPaymentSchedule.payment_date);
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
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }

                    try
                    {
                        //Report will display all payees that have any item in their payee account with a different amount when compared the amount in ‘Payment History – Details’ as of last ‘Payment Cycle Date’. 
                        //This report will be a break-down for the ‘Changes’ from ‘Monthly Benefit Payment Summary Report’.
                        //This report will be created as part of ‘Trial Reports’. 
                        ldsReportResult = new DataSet();

                        //Commented by Abhishek on 11/15/2012 -- Need to debug the issue, THIS report is causing the SERVER TO CRASH
                        ldsReportResult = lobjCreateReports.TrialBenefitPaymentChangeReport(icdoPaymentSchedule.payment_date,
                            icdoPaymentSchedule.payment_schedule_id);
                        if (ldsReportResult.Tables.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldsReportResult, "rpt13_BenefitPaymentChangeReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }

                    try
                    {
                        //Report will display the last month’s total balance as ‘Beginning Balance’. It displays this month’s activity and the ending balance.
                        //This report will be created as part of ‘Trial Reports’. 
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentSummaryReport(icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptMonthlyBenefitPaymentSummaryReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt14_MonthlyBenefitPaymentReconciliationReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            //ibusPaymentSchedule.PostEndingBalance(ldtReportResult);
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }

                    try
                    {
                        //This report gives the count of participants and survivors grouped by the payee category
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialMonthlyReciepantCountReport(icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptMonthlyRecipientCountReport";

                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt15_MonthlyRecipientCountReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }
                    break;

                case busConstant.PaymentScheduleAdhocWeekly:
                    LoadPayeesForAdhocWeeklyPaymentProcess(icdoPaymentSchedule.payment_date, icdoPaymentSchedule.payment_schedule_id);
                    try
                    {
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialPayeeListReport(icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptTrialPayeeListReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt16_TrialPayeeListReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }

                    //Removing this redundent task to eliminate issue of OPUS no response or slowness during IAPHardship adhoc payment trial run. 
                    //try
                    //{
                    //    string rptGenPath = string.Format("{0}{1}\\", iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_TRIAL_REPORT_PATH), "TmpMergeFolder");
                    //    if (System.IO.Directory.Exists(rptGenPath))
                    //        System.IO.Directory.Delete(rptGenPath, true);
                    //    if (!System.IO.Directory.Exists(rptGenPath))
                    //        System.IO.Directory.CreateDirectory(rptGenPath);
                    //    GenerateTrialPaymentDirectives(rptGenPath);
                    //    MergePdfsFromPath(rptGenPath);
                    //}
                    //catch (Exception e)
                    //{
                    //    ExceptionManager.Publish(e);
                    //}

                    break;

                case busConstant.PaymentScheduleTypeWeekly:
                    LoadPayeesForIAPPaymentProcess(icdoPaymentSchedule.payment_date, icdoPaymentSchedule.payment_schedule_id);
                    try
                    {
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialPayeeListReport(icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptTrialPayeeListReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt16_TrialPayeeListReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            lintrtn = 1;
                        }

                        string rptGenPath = string.Format("{0}{1}\\", iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_TRIAL_REPORT_PATH), "TmpMergeFolder");
                        if (System.IO.Directory.Exists(rptGenPath))
                            System.IO.Directory.Delete(rptGenPath, true);
                        GenerateTrialPaymentDirectives(rptGenPath);
                        MergePdfsFromPath(rptGenPath);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }
                    break;
                case busConstant.PaymentScheduleAdhocMonthly:
                    LoadPayeesForAdhocMonthlyPaymentProcess(icdoPaymentSchedule.payment_date, icdoPaymentSchedule.payment_schedule_id);
                    try
                    {
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentbyItemReport(icdoPaymentSchedule.payment_date, true);
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
                                    drMonthlyData["PAYMENT_DATE"] = icdoPaymentSchedule.payment_date;
                                    drMonthlyData["ITEMTYPE"] = string.Empty;
                                    tempMonthlyData.Rows.Add(drMonthlyData);
                                }
                            }
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
                                        drOneTimeData["PAYMENT_DATE"] = icdoPaymentSchedule.payment_date;
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
                            row["PAYMENT_DATE"] = icdoPaymentSchedule.payment_date;
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
                                    dr["PAYMENT_DATE"] = icdoPaymentSchedule.payment_date;
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
                            rowNetAmount["PAYMENT_DATE"] = icdoPaymentSchedule.payment_date;
                            rowNetAmount["ITEMTYPE"] = string.Empty;
                            tempNetAmountData.Rows.Add(rowNetAmount);
                            #endregion

                            #region Grand Total Data Table
                            DataTable tempGrandTotalData = lobjCreateReports.TrialMonthlyBenefitPaymentGrandTotalReport(icdoPaymentSchedule.payment_date, true);
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
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }
                    try
                    {
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialPayeeListReport(icdoPaymentSchedule.payment_date);
                        ldtReportResult.TableName = "rptTrialPayeeListReport";
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt16_TrialPayeeListReport", lstrReportPrefixPaymentScheduleID + "TRIAL_");
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        return -1;
                    }
                    break;

            }
            return lintrtn;
        }
        #endregion
        #region
        public ArrayList btn_RunACHReversalRpt()
        {
            ArrayList larrErrors = new ArrayList();
            bool aBoolrtn = false;
            aBoolrtn = generateAchReversalRpt();
            //Update the status only if all reports executed successfully
            if (aBoolrtn == true)
            {

            }
            larrErrors.Add(this);
            return larrErrors;
        }
        private bool generateAchReversalRpt()
        {
            try
            {
                int aintPlanId = 0;
                if (icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleTypeWeekly || icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleAdhocWeekly)
                {
                    aintPlanId = 1;
                }
                DataTable idtStopPayments = busBase.Select("cdoPaymentSchedule.LoadAchStopPayments", new object[1] { aintPlanId });
                if (idtStopPayments.Rows.Count > 0)
                {
                    int lintrtn = 0;
                    busCreateReports lobjCreateReports;

                    if (icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleAdhocMonthly || icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleAdhocWeekly)
                        lobjCreateReports = new busCreateReports(true, false);
                    else
                        lobjCreateReports = new busCreateReports();
                    lobjCreateReports.CreateSceduleInfoTbleForAchreport(icdoPaymentSchedule.payment_schedule_id, icdoPaymentSchedule.schedule_type_value);
                    DataTable idtStopPaymentsEven = new DataTable();
                    DataTable idtStopPaymentsOdd = new DataTable();
                    var varStopPaymentsEOdd = (from obj in idtStopPayments.AsEnumerable()
                                               where obj.Field<Int64>("ROW_NO") % 2 != 1
                                               select obj);
                    if (varStopPaymentsEOdd.Count() > 0)
                    {
                        idtStopPaymentsOdd = varStopPaymentsEOdd.CopyToDataTable();
                    }
                    else
                    {
                        idtStopPaymentsOdd = idtStopPayments.Clone();
                    }
                    idtStopPaymentsOdd.TableName = "ReportTable01";
                    var varStopPaymentsEven = (from obj in idtStopPayments.AsEnumerable()
                                               where obj.Field<Int64>("ROW_NO") % 2 == 1
                                               select obj);
                    if (varStopPaymentsEven.Count() > 0)
                    {
                        idtStopPaymentsEven = varStopPaymentsEven.CopyToDataTable();
                    }
                    else
                    {
                        idtStopPaymentsEven = idtStopPayments.Clone();
                    }
                    idtStopPaymentsEven.TableName = "ReportTable02";
                    DataSet dsStopPayments = new DataSet();
                    dsStopPayments.Tables.Add(idtStopPaymentsEven);
                    dsStopPayments.Tables.Add(idtStopPaymentsOdd);
                    string lstrReportPath = lobjCreateReports.CreatePDFReport(dsStopPayments, "AchReversalRequestReport");
                    busUser lbusSenderInfo = new busUser();
                    if (lbusSenderInfo.FindUser(this.iobjPassInfo.iintUserSerialID))
                    {
                        lintrtn = DBFunction.DBNonQuery("cdoPaymentSchedule.UpdateAchReversalFlag",
                              new object[2] { aintPlanId, lbusSenderInfo.icdoUser.User_Name },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);

            }
            return true;
        }

        #endregion
        public bool IsPaymentDateSameAsToday()
        {
            bool lblnResult = false;
            if (icdoPaymentSchedule.payment_date == DateTime.Today)
                lblnResult = true;
            return lblnResult;
        }

        #region Validate hard Errors
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            this.EvaluateInitialLoadRules();
            base.ValidateHardErrors(aenmPageMode);

            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            if (icdoPaymentSchedule.status_value == busConstant.PaymentScheduleActionStatusCancelled &&
                (Convert.ToString(this.icdoPaymentSchedule.ihstOldValues[enmPaymentSchedule.status_value.ToString()]) != busConstant.PaymentScheduleActionStatusFailed
                || Convert.ToString(this.icdoPaymentSchedule.ihstOldValues[enmPaymentSchedule.status_value.ToString()]) != busConstant.PaymentScheduleActionStatusProcessed))
            {
                lobjError = AddError(6055, "");
                this.iarrErrors.Add(lobjError);
            }

            if (this.icdoPaymentSchedule.payment_date == DateTime.MinValue) //&& this.icdoPaymentSchedule.schedule_type_value != busConstant.PaymentScheduleVendor)
            {
                lobjError = AddError(6082, "");
                this.iarrErrors.Add(lobjError);
            }
            else if (this.icdoPaymentSchedule.payment_date < DateTime.Today) //&& this.icdoPaymentSchedule.schedule_type_value != busConstant.PaymentScheduleVendor)
            {
                lobjError = AddError(6146, "");
                this.iarrErrors.Add(lobjError);
            }


            if (this.icdoPaymentSchedule.ach_effective_date == DateTime.MinValue && this.icdoPaymentSchedule.schedule_type_value != busConstant.PaymentScheduleVendor)
            {
                lobjError = AddError(6083, "");
                this.iarrErrors.Add(lobjError);
            }

            if (icdoPaymentSchedule.schedule_type_value != busConstant.PaymentScheduleAdhocMonthly && icdoPaymentSchedule.schedule_type_value != busConstant.PaymentScheduleAdhocWeekly)
            {
                if (this.icdoPaymentSchedule.payment_date != DateTime.MinValue)
                {
                    int lintCount = (int)DBFunction.DBExecuteScalar("cdoPaymentSchedule.CheckExistingPaymentScheduleForSameDate", new object[3] { this.icdoPaymentSchedule.payment_date,
                                                                this.icdoPaymentSchedule.schedule_type_value, icdoPaymentSchedule.payment_schedule_id },
                                                             iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (lintCount > 0)
                    {
                        lobjError = AddError(6086, "");
                        this.iarrErrors.Add(lobjError);
                    }
                }
            }
            else
            {
                if (this.icdoPaymentSchedule.payment_date != DateTime.MinValue)
                {
                    int lintCount = (int)DBFunction.DBExecuteScalar("cdoPaymentSchedule.CheckExistingPaymentScheduleTypeForSameDate", new object[3]
                                      { icdoPaymentSchedule.payment_date,icdoPaymentSchedule.schedule_type_value,icdoPaymentSchedule.payment_schedule_id
                                      },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (lintCount != 0)
                    {
                        lobjError = AddError(6086, "");
                        this.iarrErrors.Add(lobjError);
                    }
                }

            }
        }
        #endregion

        #region Load Data For Trial Reports
        public void LoadPayeesForIAPPaymentProcess(DateTime adtPaymentScheduleDate, int payment_schedule_id)
        {
            busBase lobjBase = new busBase();
            idtPayeeAccount = busBase.Select("cdoPaymentSchedule.LoadPayeeAccountsForWeeklyIAPPaymentProcess", new object[1] { adtPaymentScheduleDate.Date });
            iclbPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(idtPayeeAccount, "icdoPayeeAccount");
        }

        public void LoadPayeesForAdhocWeeklyPaymentProcess(DateTime adtPaymentScheduleDate, int payment_schedule_id)
        {
            busBase lobjBase = new busBase();
            idtPayeeAccount = busBase.Select("cdoPaymentSchedule.LoadPayeeAccountsForWeeklyAdhocPaymentProcess", new object[1] { adtPaymentScheduleDate.Date });
            iclbPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(idtPayeeAccount, "icdoPayeeAccount");
        }

        public void LoadPayeesForMonthlyPaymentProcess(DateTime adtPaymentScheduleDate, int payment_schedule_id)
        {
            busBase lobjBase = new busBase();
            idtPayeeAccount = busBase.Select("cdoPaymentSchedule.LoadPayeeAccountsForPaymentProcess", new object[1] { adtPaymentScheduleDate.Date });
            iclbPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(idtPayeeAccount, "icdoPayeeAccount");
        }

        public void LoadPayeesForAdhocMonthlyPaymentProcess(DateTime adtPaymentScheduleDate, int payment_schedule_id)
        {
            busBase lobjBase = new busBase();
            idtPayeeAccount = busBase.Select("cdoPaymentSchedule.LoadPayeeAccountsForAdhocPaymentProcess", new object[1] { adtPaymentScheduleDate.Date });
            iclbPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(idtPayeeAccount, "icdoPayeeAccount");
        }
        #endregion

        #region Load Initial Data
        public void LoadInitialData()
        {
            if (this.icdoPaymentSchedule.status_value.IsNotNullOrEmpty())
                this.icdoPaymentSchedule.status_description = busGlobalFunctions.GetCodeValueDescriptionByValue(this.icdoPaymentSchedule.status_id,
                    this.icdoPaymentSchedule.status_value).description;

            if (this.icdoPaymentSchedule.schedule_type_value.IsNotNullOrEmpty())
                this.icdoPaymentSchedule.schedule_type_description = busGlobalFunctions.GetCodeValueDescriptionByValue(this.icdoPaymentSchedule.schedule_type_id,
                    this.icdoPaymentSchedule.schedule_type_value).description;
        }
        #endregion
        public void PostEndingBalance(DataTable ldtReportResult, int payment_schedule_id)
        {
            if (ldtReportResult.Rows.Count > 0)
            {
                cdoMonthlyBenifitSummary lcdoMonthlyBenifitSummary = new cdoMonthlyBenifitSummary();
                //var cdoMonthlyBenifitSummary = (from o in ldtReportResult.AsEnumerable()
                //                                select new
                //                      {
                //                          MPIPP_Amount = o.Field<Int32?>("MPIPP_Amount") == null ? 0 : Convert.ToInt32(o.Field<Int32?>("MPIPP_Amount")),
                //                          L52_Amount = o.Field<Int32?>("L52_Amount") == null ? 0 : Convert.ToInt32(o.Field<Int32?>("L52_Amount")),
                //                          L161_Amount = o.Field<Int32?>("L161_Amount") == null ? 0 : Convert.ToInt32(o.Field<Int32?>("L161_Amount")),
                //                          L600_Amount = o.Field<Int32?>("L600_Amount") == null ? 0 : Convert.ToInt32(o.Field<Int32?>("L600_Amount")),
                //                          L666_Amount = o.Field<Int32?>("L666_Amount") == null ? 0 : Convert.ToInt32(o.Field<Int32?>("L666_Amount")),
                //                          L700_Amount = o.Field<Int32?>("L700_Amount") == null ? 0 : Convert.ToInt32(o.Field<Int32?>("L700_Amount")),

                //                      }
                //                        );
                //lcdoMonthlyBenifitSummary.mpipp_amount = cdoMonthlyBenifitSummary.Sum(obj => obj.MPIPP_Amount);
                //lcdoMonthlyBenifitSummary.l52_amount = cdoMonthlyBenifitSummary.Sum(obj => obj.L52_Amount);
                //lcdoMonthlyBenifitSummary.l161_amount = cdoMonthlyBenifitSummary.Sum(obj => obj.L161_Amount);
                //lcdoMonthlyBenifitSummary.l600_amount = cdoMonthlyBenifitSummary.Sum(obj => obj.L600_Amount);
                //lcdoMonthlyBenifitSummary.l666_amount = cdoMonthlyBenifitSummary.Sum(obj => obj.L666_Amount);
                //lcdoMonthlyBenifitSummary.l700_amount = cdoMonthlyBenifitSummary.Sum(obj => obj.L700_Amount);
                //lcdoMonthlyBenifitSummary.payment_schedule_id = payment_schedule_id;
                //lcdoMonthlyBenifitSummary.Insert();
                foreach (DataRow dr in ldtReportResult.Rows)
                {
                    if (dr["MPIPP_Amount"] != DBNull.Value)
                    {
                        lcdoMonthlyBenifitSummary.mpipp_amount += Convert.ToDecimal(dr["MPIPP_Amount"]);
                    }
                    if (dr["L52_Amount"] != DBNull.Value)
                    {
                        lcdoMonthlyBenifitSummary.l52_amount += Convert.ToDecimal(dr["L52_Amount"]);
                    }
                    if (dr["L161_Amount"] != DBNull.Value)
                    {
                        lcdoMonthlyBenifitSummary.l161_amount += Convert.ToDecimal(dr["L161_Amount"]);
                    }
                    if (dr["L600_Amount"] != DBNull.Value)
                    {
                        lcdoMonthlyBenifitSummary.l600_amount += Convert.ToDecimal(dr["L600_Amount"]);
                    }
                    if (dr["L666_Amount"] != DBNull.Value)
                    {
                        lcdoMonthlyBenifitSummary.l666_amount += Convert.ToDecimal(dr["L666_Amount"]);
                    }
                    if (dr["L700_Amount"] != DBNull.Value)
                    {
                        lcdoMonthlyBenifitSummary.l700_amount += Convert.ToDecimal(dr["L700_Amount"]);
                    }

                    //lcdoMonthlyBenifitSummary.l52_amount = cdoMonthlyBenifitSummary.Sum(obj => obj.L52_Amount);
                    //lcdoMonthlyBenifitSummary.l161_amount = cdoMonthlyBenifitSummary.Sum(obj => obj.L161_Amount);
                    //lcdoMonthlyBenifitSummary.l600_amount = cdoMonthlyBenifitSummary.Sum(obj => obj.L600_Amount);
                    //lcdoMonthlyBenifitSummary.l666_amount = cdoMonthlyBenifitSummary.Sum(obj => obj.L666_Amount);
                    //lcdoMonthlyBenifitSummary.l700_amount = cdoMonthlyBenifitSummary.Sum(obj => obj.L700_Amount);

                }
                lcdoMonthlyBenifitSummary.payment_schedule_id = payment_schedule_id;
                lcdoMonthlyBenifitSummary.Insert();
            }

        }

        #region Last Processed Schedule Details
        public int GetLastPaymentScheduleDetails(string astrScheduleTypeValue)
        {
            int aintLastProcessedScheduleid = 0;

            DataTable ldtbLastProcessedScheduledetails = busBase.Select("cdoPaymentSchedule.GetLastProcssedScheduleDetailsbyShceduleType", new object[1] { astrScheduleTypeValue });
            if (ldtbLastProcessedScheduledetails != null && ldtbLastProcessedScheduledetails.Rows.Count > 0)
            {
                aintLastProcessedScheduleid = Convert.ToInt32(ldtbLastProcessedScheduledetails.Rows[0][0]);
            }

            return aintLastProcessedScheduleid;
        }
        #endregion

        #region Merged PDF
        private void MergePdfsFromPath(string rptGenPath)
        {
            string outputFolderPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_TRIAL_REPORT_PATH);
            string fileName = string.Format("{0}_TRIAL__rptIAPPaymentDirectives", icdoPaymentSchedule.payment_schedule_id);
            MergePDFs(rptGenPath, outputFolderPath, fileName);
        }

        void MergePDFs(string generatedpath, string outputpath, string fileName)
        {
            DirectoryInfo dir = new DirectoryInfo(generatedpath);
            List<string> filesPath = new List<string>();
            foreach (FileInfo fi in dir.GetFiles("*.pdf").OrderBy(item => item.CreationTime))
            {
                filesPath.Add(fi.FullName);
            }

            List<PdfReader> readerList = new List<PdfReader>();
            string outPutFilePath = outputpath + fileName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";

            foreach (string filePath in filesPath)
            {
                PdfReader pdfReader = new PdfReader(filePath);
                readerList.Add(pdfReader);
            }
            //Define a new output document and its size, type
            Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
            //Create blank output pdf file and get the stream to write on it.
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create));
            document.Open();
            foreach (PdfReader reader in readerList)
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    PdfImportedPage page = writer.GetImportedPage(reader, i);
                    document.Add(iTextSharp.text.Image.GetInstance(page));
                }
            }
            document.Close();
        }
        #endregion
    }
}
