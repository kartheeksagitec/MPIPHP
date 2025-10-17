using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;
using MPIPHP.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using System.Linq;
using MPIPHPJobService;
using Sagitec.CustomDataObjects;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.Common;
using System.Threading.Tasks;
using Sagitec.ExceptionPub;
using Microsoft.Reporting.WinForms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MPIPHPJobService
{
    public class busReEvaluationOfMinimumDistributionBatch : busBatchHandler
    {

        #region  Re-evaluation MD Batch
        DateTime idtFromDate { get; set; }
        DateTime idtToDate { get; set; }

        string astrOPUSDBConnetion;

        private string istrBatchName { get; set; }
        public void ReEvaluationOfMinimumDistributionBatch()
        {
            busBase lobjBase = new busBase();

            string lstrPlanIdentifier = busConstant.MPIPP;
            if (ibusJobHeader != null)
            {
                if (ibusJobHeader.iclbJobDetail == null)
                    ibusJobHeader.LoadJobDetail(true);

                foreach (busJobDetail lobjDetail in ibusJobHeader.iclbJobDetail)
                {
                    if (lobjDetail.iclbJobParameters != null && lobjDetail.iclbJobParameters.Count > 0)
                    {
                        foreach (busJobParameters lobjParam in lobjDetail.iclbJobParameters)
                        {
                            lstrPlanIdentifier = Convert.ToString(lobjParam.icdoJobParameters.param_value).TrimStart().TrimEnd();
                        }
                    }
                }
            }
            //For Retirement
            DataTable ldtbResult = busBase.Select("cdoBenefitCalculationHeader.LoadDataForMDReEvaluationBatch", new object[1] { lstrPlanIdentifier });

            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
            lclbPayeeAccount = lobjBase.GetCollection<busPayeeAccount>(ldtbResult, "icdoPayeeAccount");

            DataTable ldtbReportTable01 = new DataTable();
            ldtbReportTable01.TableName = "ReportTable01";

            //Required Columns in report
            ldtbReportTable01.Columns.Add("PARTICIPANT_LAST_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("PARTICIPANT_FIRST_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("PARTICIPANT_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("PARTICIPANT_MPI_PERSON_ID", typeof(string));
            ldtbReportTable01.Columns.Add("PAYEE_MPI_PERSON_ID", typeof(string));
            ldtbReportTable01.Columns.Add("PAYEE_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_TYPE", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_SUB_TYPE", typeof(string));
            ldtbReportTable01.Columns.Add("RETIREMENT_DATE", typeof(DateTime));
            ldtbReportTable01.Columns.Add("PLAN_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("RETIREMENT_BENEFIT_AMOUNT", typeof(decimal));
            ldtbReportTable01.Columns.Add("RECALCULATED_RETIREMENT_BENEFIT_AMOUNT", typeof(decimal));
            ldtbReportTable01.Columns.Add("PAYEE_ACCOUNT_ID", typeof(int));
            ldtbReportTable01.Columns.Add("COMMENT", typeof(string));
            ldtbReportTable01.Columns.Add("FROM_DATE", typeof(DateTime));
            ldtbReportTable01.Columns.Add("TO_DATE", typeof(DateTime));
            ldtbReportTable01.Columns.Add("CALCULATION_ID", typeof(Int32));
            ldtbReportTable01.Columns.Add("INCREASE_ELIGIBLE", typeof(string));
            ldtbReportTable01.Columns.Add("REEMPLOYED", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_OPTION", typeof(string));
            ldtbReportTable01.Columns.Add("STATUS_VALUE", typeof(string));
            ldtbReportTable01.Columns.Add("SUSPENSION_STATUS_REASON_VALUE", typeof(string));


            foreach (busPayeeAccount lbusPayeeAccount in lclbPayeeAccount)
            {
                ReEvaluationOfMinimumDistributionBenefit(lbusPayeeAccount, ldtbReportTable01);
            }

            if (ldtbReportTable01 != null && ldtbReportTable01.Rows.Count > 0)
                CreateExcelReport(ldtbReportTable01, "rptReEvaluationofMDBatchReport");

        }


        public void ReEvaluationOfMinimumDistributionBenefit(busPayeeAccount abusPayeeAccount, DataTable ldtbReportTable01)
        {
            DataRow dr = ldtbReportTable01.NewRow();


            LoadPayeeAccountDetails(ref abusPayeeAccount);

            dr["PARTICIPANT_LAST_NAME"] = abusPayeeAccount.ibusParticipant.icdoPerson.last_name;
            dr["PARTICIPANT_FIRST_NAME"] = abusPayeeAccount.ibusParticipant.icdoPerson.first_name;
            dr["PARTICIPANT_NAME"] = abusPayeeAccount.ibusParticipant.icdoPerson.first_name + " " + abusPayeeAccount.ibusParticipant.icdoPerson.last_name;
            dr["PAYEE_MPI_PERSON_ID"] = abusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id;
            dr["PAYEE_NAME"] = abusPayeeAccount.ibusPayee.icdoPerson.first_name + " " + abusPayeeAccount.ibusPayee.icdoPerson.last_name;
            dr["PARTICIPANT_MPI_PERSON_ID"] = abusPayeeAccount.ibusParticipant.icdoPerson.mpi_person_id;
            dr["PLAN_NAME"] = abusPayeeAccount.icdoPayeeAccount.istrPlanCode;
            dr["RETIREMENT_BENEFIT_AMOUNT"] = abusPayeeAccount.idecNextGrossPaymentACH == decimal.Zero ? abusPayeeAccount.idecPaidGrossAmount : abusPayeeAccount.idecNextGrossPaymentACH;
            dr["BENEFIT_TYPE"] = abusPayeeAccount.icdoPayeeAccount.benefit_account_type_description;
            dr["PAYEE_ACCOUNT_ID"] = abusPayeeAccount.icdoPayeeAccount.payee_account_id;
            dr["BENEFIT_SUB_TYPE"] = abusPayeeAccount.icdoPayeeAccount.retirement_type_description;
            dr["RETIREMENT_DATE"] = abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate;
            dr["BENEFIT_OPTION"] = abusPayeeAccount.icdoPayeeAccount.istrBenefitOption;
            dr["FROM_DATE"] = idtFromDate;
            dr["TO_DATE"] = idtToDate;
            dr["REEMPLOYED"] = string.Empty;
            dr["STATUS_VALUE"] = abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_description;
            if (abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value.IsNotNullOrEmpty())
            {
                dr["SUSPENSION_STATUS_REASON_VALUE"] = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Payee_Account_Suspension_Reason_id,
                        abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value).description;
            }
            else
                dr["SUSPENSION_STATUS_REASON_VALUE"] = string.Empty;



            try
            {
                int lintParticipantHasPendingDRO = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.CheckIfParticipantHasPendingDRO", new object[1] { abusPayeeAccount.icdoPayeeAccount.person_id },

                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                int lintParticipantDateOfDeath = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.CheckIfParticipantIsDead", new object[1] { abusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                int lintPendingAdjCalcCount = (int)DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.GetCountofPendingAdjCalc",
                                                                                new object[3] { abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value, abusPayeeAccount.icdoPayeeAccount.iintPlanId },
                                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if (lintParticipantHasPendingDRO > 0)
                {

                    dr["COMMENT"] = "QDRO Payee Account re-calculation must me be processed first.";
                    ldtbReportTable01.Rows.Add(dr);
                    return;
                }

                else if (lintPendingAdjCalcCount > 0)
                {
                    DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.CancelPendingAdjustmentCalculations",
                                                                                new object[3] { abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value, abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.iintPlanId },
                                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }



                if (abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && abusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                {
                    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                    lbusPlanBenefitXr.FindPlanBenefitXr(abusPayeeAccount.icdoPayeeAccount.plan_benefit_id);
                    string lstrBenefitOption = lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;

                    DataTable ldtbList = busBase.Select("cdoDroApplication.LoadAllExistingQDRO", new object[1] { abusPayeeAccount.icdoPayeeAccount.person_id });

                    if (ldtbList != null && ldtbList.Rows.Count > 0)
                    {
                        dr["COMMENT"] += "QDRO On File.";
                    }

                    busBenefitCalculationRetirement lbusBenefitCalculationHeader = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    if (abusPayeeAccount.icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                    {
                        lbusBenefitCalculationHeader = abusPayeeAccount.ReCalculateBenefitForRetirement(lstrBenefitOption, ablnReEvaluationMDBatch: true, ablnPostRetDeath: true);//ablnPostRetDeath it needs to be true, else it will attach the calculation with the payee account if there are hours after retirement.

                        if (lbusBenefitCalculationHeader != null && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail != null && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Count > 0)
                        {
                            lbusBenefitCalculationHeader.RecalculateMDBenefits(lbusBenefitCalculationHeader, abusPayeeAccount);
                        }
                    }


                    if (lbusBenefitCalculationHeader != null && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail != null
                        && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Count() > 0)
                    {
                        if (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault() != null &&
                            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions != null
                            && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Count() > 0)
                        {
                            dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"] = lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                            dr["CALCULATION_ID"] = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                        }

                        //Temporary Field
                        dr["INCREASE_ELIGIBLE"] = busConstant.NO_CAPS;

                        if (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null
                            && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Count > 0)
                        {

                            foreach (busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail in lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.OrderByDescending(t => t.icdoBenefitCalculationYearlyDetail.plan_year))
                            {
                                if ((lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year == Convert.ToDecimal(2017)
                                        || lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year == Convert.ToDecimal(2015))
                                    &&
                                            lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.active_retiree_inc > decimal.Zero
                                   )
                                {
                                    dr["INCREASE_ELIGIBLE"] = busConstant.YES_CAPS;
                                    break;
                                }
                            }
                        }
                    }

                    if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id == 0)
                    {
                        dr["COMMENT"] += "Participant is not eligible for Retirement Benefits";
                        ldtbReportTable01.Rows.Add(dr);
                        return;
                    }
                    ldtbReportTable01.Rows.Add(dr);
                }

            }
            catch (Exception e)
            {
                PostErrorMessage("Error Occured for MPID : " + abusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id);
                dr["COMMENT"] = "Error Occured for MPID : " + abusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id;
            }
        }


        public void LoadPayeeAccountDetails(ref busPayeeAccount abusPayeeAccount)
        {
            abusPayeeAccount.LoadPayeeAccountAchDetails();
            abusPayeeAccount.LoadPayeeAccountPaymentItemType();
            abusPayeeAccount.LoadPayeeAccountRetroPayments();
            abusPayeeAccount.LoadPayeeAccountRetroPaymentDetails();

            //Payment Adjustment
            abusPayeeAccount.LoadPayeeAccountBenefitOverPayment();
            // lobjPayeeAccount.LoadPayeeAccountOverPaymentPaymentDetails();
            abusPayeeAccount.LoadAllRepaymentSchedules();

            abusPayeeAccount.LoadPayeeAccountRolloverDetails();
            abusPayeeAccount.LoadPayeeAccountStatuss();
            abusPayeeAccount.LoadPayeeAccountTaxWithholdings();
            abusPayeeAccount.LoadBenefitDetails();
            abusPayeeAccount.LoadDRODetails();
            abusPayeeAccount.LoadNextBenefitPaymentDate();
            abusPayeeAccount.LoadTotalRolloverAmount();
            abusPayeeAccount.LoadGrossAmount();
            abusPayeeAccount.LoadPayeeAccountDeduction();
            abusPayeeAccount.LoadNonTaxableAmount();
            abusPayeeAccount.GetCalculatedTaxAmount();
            abusPayeeAccount.LoadDeathNotificationStatus();
            abusPayeeAccount.LoadWithholdingInformation();
            abusPayeeAccount.GetCuurentPayeeAccountStatus();
            abusPayeeAccount.CheckAnnuity();
            abusPayeeAccount.LoadLastPaymentDate();
            abusPayeeAccount.LoadPaymentHistoryHeaderDetails();

            //Payee Account Details
            if (abusPayeeAccount.icdoPayeeAccount.person_id != 0)
            {
                abusPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
                abusPayeeAccount.ibusPayee.FindPerson(abusPayeeAccount.icdoPayeeAccount.person_id);
            }
            //Organization Details
            if (abusPayeeAccount.icdoPayeeAccount.org_id != 0)
            {
                abusPayeeAccount.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                abusPayeeAccount.ibusOrganization.FindOrganization(abusPayeeAccount.icdoPayeeAccount.org_id);
            }

            //TransferOrg Details
            if (abusPayeeAccount.icdoPayeeAccount.transfer_org_id != 0)
            {
                busOrganization lbusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                if (lbusOrganization.FindOrganization(abusPayeeAccount.icdoPayeeAccount.transfer_org_id))
                {
                    abusPayeeAccount.icdoPayeeAccount.istrOrgMPID = lbusOrganization.icdoOrganization.mpi_org_id;
                    abusPayeeAccount.icdoPayeeAccount.istrOrgName = lbusOrganization.icdoOrganization.org_name;
                }
            }

            //Participant Account Details
            if (abusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
            {
                abusPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                abusPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(abusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                abusPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
                abusPayeeAccount.ibusParticipant.FindPerson(abusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);
            }
            //lobjPayeeAccount.CheckPaymentHistoryHeader();

            if (abusPayeeAccount.icdoPayeeAccount.adjustment_payment_eligible_flag == busConstant.Flag_Yes)
            {
                abusPayeeAccount.iblnAdjustmentPaymentEliglbleFlag = busConstant.YES;
            }

            abusPayeeAccount.LoadBreakDownDetails();
        }

        #region Create Excel Report
        private string CreateExcelReport(DataTable ldtbResultTable, string astrReportName, string astrPrefix = "")
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
            ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);

            rvViewer.LocalReport.DataSources.Add(lrdsReport);

            byte[] bytes = rvViewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string labsRptGenPath = string.Empty;
            labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);
            labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_REPORT_REEVALUATION_OF_MD_PATH);
            if (!Directory.Exists(labsRptGenPath))
                Directory.CreateDirectory(labsRptGenPath);

            string lstrReportFullName = string.Empty;

            if (astrPrefix.IsNotNullOrEmpty())
                lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            else
            {
                lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            }

            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            return lstrReportFullName;
        }
        #endregion


        #endregion Re-evaluation MD Batch

        #region Re-evaluation MD Batch Previous Code
        /*
        #region Properties

        private object iobjLock = null;
        public DataTable dtReport;
        #endregion

        #region ReEvaluation of Minimum Distribution Batch

        public void ReEvaluationOfMinimumDistributionBatch()
        {
            int lintCount = 0;
            int lintTotalCount = 0;

            utlConnection utlLegacyDBConnection = HelperFunction.GetDBConnectionProperties("core");
            string astrLegacyDBConnection = utlLegacyDBConnection.istrConnectionString;

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();

            //ROhan MD RE PIR 815
            string lstrPlanIdentifier = string.Empty;
            if (ibusJobHeader != null)
            {
                if (ibusJobHeader.iclbJobDetail == null)
                    ibusJobHeader.LoadJobDetail(true);

                foreach (busJobDetail lobjDetail in ibusJobHeader.iclbJobDetail)
                {
                    foreach (busJobParameters lobjParam in lobjDetail.iclbJobParameters)
                    {
                        lstrPlanIdentifier = Convert.ToString(lobjParam.icdoJobParameters.param_value).TrimStart().TrimEnd();
                    }
                }
            }

            DataTable ldtPersonInformation = busBase.Select("cdoBenefitCalculationHeader.LoadDataForMDReEvaluationBatch", new object[1] { lstrPlanIdentifier });

            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions po = new ParallelOptions();

            //Rashmi Fixed 04/24/2014(Removing Parallel loop)
            po.MaxDegreeOfParallelism = 1;// System.Environment.ProcessorCount * 4;
            CreateReportTable();
            //Parallel.ForEach(ldtPersonInformation.AsEnumerable(), po, (acdoPerson, loopState) =>
            //{
            foreach (DataRow acdoPerson in ldtPersonInformation.AsEnumerable())
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "ReEvaluationOfMinimumDistributionBatch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;

                CalculateAnnualMDAdjustment(acdoPerson, lobjPassInfo, lintCount, lintTotalCount);

                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;
            }
            //});
            //Rohan RE-MD PIR 815
            CreateBatchReport(lstrPlanIdentifier);
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        }

        #endregion

        #region Calculate ReEvaluation of Minimum Distribution

        private void CalculateAnnualMDAdjustment(DataRow acdoPerson, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        {

            busBase lbusBase = new busBase();
            busCalculation lbusCalculation = new busCalculation();

            lock (iobjLock)
            {
                aintCount++;
                aintTotalCount++;
                if (aintCount == 100)
                {
                    string lstrMsg = aintTotalCount + " : " + " Records Has Been Processed";
                    PostInfoMessage(lstrMsg);
                    aintCount = 0;
                }
            }

            autlPassInfo.BeginTransaction();
            try
            {
                DataRow dtReportRow = dtReport.NewRow();
                busBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busBenefitCalculationRetirement();
                lbusBenefitCalculationRetirement.CalculateMDBenefitForReEvaluationBatch(acdoPerson, dtReportRow, dtReport);
                //dtReport.Rows.Add(dtReportRow);
                autlPassInfo.Commit();
            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    string lstrMsg = "Error while Executing Batch, Error Message: " + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();
            }
        }

        #endregion
        //Rohan RE-MD PIR 815
        public void CreateBatchReport(string lstrPlanIdentifier)
        {
            if (dtReport != null && dtReport.Rows.Count > 0)
            {
                busCreateReports lobjCreateReports = new busCreateReports();
                dtReport.TableName = "ReportTable01";
                lobjCreateReports.CreatePDFReport(dtReport, "rptReEvaluationofMDBatchReport", lstrPlanIdentifier);
            }
        }
        public void CreateReportTable()
        {
            dtReport = new DataTable();
            if (dtReport.Rows.Count == 0)
            {
                dtReport.Columns.Add("First_Name");
                dtReport.Columns.Add("Last_Name");
                DataColumn ldtMNDate = new DataColumn("Minimum_Distribution_Date", Type.GetType("System.DateTime"));
                dtReport.Columns.Add(ldtMNDate);
                DataColumn lstrMPID = new DataColumn("MPI_PERSON_ID", Type.GetType("System.String"));
                dtReport.Columns.Add(lstrMPID);
                DataColumn lintCalId = new DataColumn("Calculation_Id", Type.GetType("System.Int32"));
                dtReport.Columns.Add(lintCalId);
                DataColumn ldecMnthBen = new DataColumn("Monthly_Benefit", Type.GetType("System.Decimal"));
                dtReport.Columns.Add(ldecMnthBen);
                DataColumn lstrPlanName = new DataColumn("PLAN_NAME", Type.GetType("System.String"));
                dtReport.Columns.Add(lstrPlanName);
                DataColumn lintSuspMnths = new DataColumn("Suspendible_Months", Type.GetType("System.Int32"));
                dtReport.Columns.Add(lintSuspMnths);
                DataColumn ldecERCrntYr = new DataColumn("ER_CurrentYear", Type.GetType("System.Decimal"));
                dtReport.Columns.Add(ldecERCrntYr);
                DataColumn ldecBenAsOfDeterminationDt = new DataColumn("BenefitAsOfDeterminationDt", Type.GetType("System.Decimal"));
                dtReport.Columns.Add(ldecBenAsOfDeterminationDt);
                DataColumn ldecDifferenceInAmount = new DataColumn("DifferenceInAmount", Type.GetType("System.Decimal"));
                dtReport.Columns.Add(ldecDifferenceInAmount);
            }
        }
        */
        #endregion Re-evaluation MD Batch Previous Code

        #region IAP Requaired Minimum Distribution Batch
        public void IAPRMDReport()
        {

            DataSet idtstIAPRMD = new DataSet();

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            DataTable ldtbIAPRMD = busBase.Select("cdoBenefitCalculationHeader.GetIAPRequiredMinimumDistribution", new object[0] { });
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 1;
            if (ldtbIAPRMD.Rows.Count > 0)
            {
                //Create Correspondence

                Parallel.ForEach(ldtbIAPRMD.AsEnumerable(), po, (ldtIAPRMD, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "IAPRequiredMinimumDistributionBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    if (istrBatchName.IsNullOrEmpty())
                        istrBatchName = lobjPassInfo.istrUserID;

                    CreateIAPRequiredMinimumDistributionCorrespondence(ldtIAPRMD, lobjPassInfo);

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;
                });


                idtstIAPRMD.Tables.Add(ldtbIAPRMD.Copy());
                idtstIAPRMD.Tables[0].TableName = "ReportTable01";
                idtstIAPRMD.DataSetName = "ReportTable01";
                //Excel Report
                string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_IAP_REQUIRED_MINIMUM_DISTRIBUTION + ".xlsx";
                string lstrIAPRequiredMinimumDistributionReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_IAPREQUIREDMINIMUMDISTRIBUTION_REPORT_PATH) + busConstant.REPORT_IAP_REQUIRED_MINIMUM_DISTRIBUTION + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
                busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrIAPRequiredMinimumDistributionReportPath, "ReportTable01", idtstIAPRMD);

                //Merge PDF
                this.MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Correspondence_Path, iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_IAPREQUIREDMINIMUMDISTRIBUTION_REPORT_PATH));

                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;

            }

        }

        private void CreateIAPRequiredMinimumDistributionCorrespondence(DataRow drIAPRMD, utlPassInfo autlPassInfo)
        {
            ArrayList aarrResult = new ArrayList();
            Hashtable ahtbQueryBkmarks = new Hashtable();
            this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;
            busPerson ibusPerson = new busPerson();
            string lstrIAPRequiredMinimumDistributionReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_IAPREQUIREDMINIMUMDISTRIBUTION_REPORT_PATH);


            ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            ibusPerson.icdoPerson.LoadData(drIAPRMD);
            busBenefitCalculationRetirement lbusBenefitCalculationHeader = new busBenefitCalculationRetirement();
            ibusPerson.icdoPerson.iintPlanId = 1;
             
            lbusBenefitCalculationHeader = lbusBenefitCalculationHeader.GenerateRequiredMinDistributionEstiFromBatch(ibusPerson,
                                            iobjSystemManagement.icdoSystemManagement.batch_date, "IAP", Convert.ToDateTime(drIAPRMD["MD_DATE"]));
            
            lbusBenefitCalculationHeader.icdoPerson = ibusPerson.icdoPerson;
            lbusBenefitCalculationHeader.iintPlanYear = DateTime.Now.Year;
            lbusBenefitCalculationHeader.istrAsofNowIapBalance = AppendDoller(Convert.ToDecimal(drIAPRMD["IAP_BALANCE"]));
            drIAPRMD["MD_DATE"]  =    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;
            


            aarrResult.Add(lbusBenefitCalculationHeader);
            this.CreateCorrespondence(busConstant.IAP_RMD_PACKET, autlPassInfo.istrUserID, autlPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true);


        }

        public string AppendDoller(decimal adecAmount)
        {
            //iblnNoBeneficiaryExists = !ibusCalculation.CheckIfSurvivorIsQualifiedSpouse(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);
            //  if (iblnNoBeneficiaryExists != true)
            //{
            if (adecAmount.IsNotNull() || adecAmount != 0)
            {
                string lstrAmount = String.Format("{0:c}", adecAmount);
                return lstrAmount;
            }
            else
            {
                return "N/A";
            }
            //}
            //return "n/a";
        }


        #endregion

    }
}
