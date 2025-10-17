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
using System.Collections.Concurrent;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace MPIPHPJobService
{
    class busRetireeIncreaseCorrespondenceBatch : busBatchHandler
    {
        #region Properties
        private object iobjLock = null;
        ConcurrentBag<busPayeeAccount> iclbPayeeAccount = new ConcurrentBag<busPayeeAccount>();

        bool iblnIsRollover { get; set; }
        bool iblnIsNonRollover { get; set; }
        bool iblnIsForeignAddress { get; set; }
        bool iblnLocalAddress { get; set; }
        //WI 14763 RID 118342
        bool iblnApprovedGroupOnly { get; set; }
        #endregion

        #region Public Methods

        private void RetrieveBatchParameters()
        {
            if (ibusJobHeader != null)
            {
                if (ibusJobHeader.iclbJobDetail == null)
                    ibusJobHeader.LoadJobDetail(true);

                foreach (busJobDetail lobjDetail in ibusJobHeader.iclbJobDetail)
                {
                    foreach (busJobParameters lobjParam in lobjDetail.iclbJobParameters)
                    {
                        switch (lobjParam.icdoJobParameters.param_name)
                        {
                            case busConstant.JobParamRollover:
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value) == "Y")
                                    iblnIsRollover = true;
                                else
                                    iblnIsRollover = false;
                                break;
                            case busConstant.JobParamNonRollover:
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value) == "Y")
                                    iblnIsNonRollover = true;
                                else
                                    iblnIsNonRollover = false;
                                break;
                            case busConstant.JobParamForeignAddress:
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value) == "Y")
                                    iblnIsForeignAddress = true;
                                else
                                    iblnIsForeignAddress = false;
                                break;
                            case busConstant.JobParamLocalAddress:
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value) == "Y")
                                    iblnLocalAddress = true;
                                else
                                    iblnLocalAddress = false;
                                break;
                            //WI 14763 RID 118342
                            case busConstant.JobParamApprovedGroupOnly:
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value) == "Y")
                                    iblnApprovedGroupOnly = true;
                                else
                                    iblnApprovedGroupOnly = false;
                                break;

                        }
                    }
                }
            }
        }

        public void RetireeIncreaseCorrespondenceBatch()
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

            #region Get Plan Year
            DataTable ldtbJobScheduleInfo = busBase.Select("cdoPayeeAccount.GetPlanYrForRetireeInc",
                                            new object[1] { busConstant.MPIPHPBatch.RETIREE_INCREASE_CORRESPONDENCE_BATCH_SCHEDULE_ID });
            int lintPlanYear = 0;

            lintPlanYear = iobjSystemManagement.icdoSystemManagement.batch_date.Year;
            //WI 15572 - RID 121787 Allow RI Correspondence Batch to run for previous year open contract.
            DataTable ldtbActiveRetireeIncreaseContract = busBase.Select("cdoActiveRetireeIncreaseContract.LoadActiveRetireeIncreaseContract", new object[0]);
            if (ldtbActiveRetireeIncreaseContract != null && ldtbActiveRetireeIncreaseContract.Rows.Count > 0)
            {
                lintPlanYear = Convert.ToInt32(ldtbActiveRetireeIncreaseContract.Rows[0]["PLAN_YEAR"]);
                string lstrMsg = "Active Retiree Increase Contract found for the year : " + lintPlanYear.ToString();
                PostInfoMessage(lstrMsg);
                DataTable ldtbApprovedPendingCorrespondence = busBase.Select("cdoActiveRetireeIncreaseContract.RIApprovedGroupsCorrespondenceNotGenerated",
                                                new object[1] { lintPlanYear });
                if (ldtbApprovedPendingCorrespondence != null && ldtbApprovedPendingCorrespondence.Rows.Count > 0)
                {
                    lstrMsg = "Correspondence will be generated for group(s): " + ldtbApprovedPendingCorrespondence.Rows[0]["GROUPS"].ToString();
                    PostInfoMessage(lstrMsg);
                }
                else
                {
                    lstrMsg = "There is no approved group pending to generate correspondence.";
                    PostInfoMessage(lstrMsg);
                }
            }
            else
            {
                string lstrMsg = "Active Retiree Increase Contract not found." ;
                PostInfoMessage(lstrMsg);
            }


            #endregion

            RetrieveBatchParameters();

            DataTable ldtbPersonAccountInfo = new DataTable();
            //WI 14763 RID 118342  adding iblnApprovedGroupOnly parameter
            string strApprovedGroupFlag = busConstant.FLAG_NO;
            if (this.iblnApprovedGroupOnly)
                strApprovedGroupFlag = busConstant.FLAG_YES;

            if (iblnIsRollover && iblnIsForeignAddress && !iblnIsNonRollover && !iblnLocalAddress)
                ldtbPersonAccountInfo = busBase.Select("cdoTempdata.GetRetireeIncreaseForReportRolloverForeignAddr", new object[2] { lintPlanYear, strApprovedGroupFlag });
            else if (iblnIsRollover && !iblnIsForeignAddress && !iblnIsNonRollover && iblnLocalAddress)
                ldtbPersonAccountInfo = busBase.Select("cdoTempdata.GetRetireeIncreaseForReportRolloverLocalAddr", new object[2] { lintPlanYear, strApprovedGroupFlag });
            else if (!iblnIsRollover && iblnIsForeignAddress && iblnIsNonRollover && !iblnLocalAddress)
                ldtbPersonAccountInfo = busBase.Select("cdoTempdata.GetRetireeIncreaseForReportNonRolloverForeignAddr", new object[2] { lintPlanYear, strApprovedGroupFlag });
            else if (!iblnIsRollover && !iblnIsForeignAddress && iblnIsNonRollover && iblnLocalAddress)
                ldtbPersonAccountInfo = busBase.Select("cdoTempdata.GetRetireeIncreaseForReportNonRolloverLocalAddr", new object[2] { lintPlanYear, strApprovedGroupFlag });
            else
                ldtbPersonAccountInfo = busBase.Select("cdoTempdata.GetRetireeIncreaseForReport", new object[2] { lintPlanYear, strApprovedGroupFlag });


            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            if (ldtbPersonAccountInfo != null && ldtbPersonAccountInfo.Rows.Count > 0)
            {
                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = 1; // System.Environment.ProcessorCount * 4;

                Parallel.ForEach(ldtbPersonAccountInfo.AsEnumerable(), po, (acdoPerson, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "RetireeIncreaseCorrespondenceBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    GenerateCorrespondence(ldtbPersonAccountInfo, acdoPerson, lintPlanYear, lobjPassInfo, lintCount, lintTotalCount);

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;
                });

                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;

                #region Create Retiree Increase Report

                if (iclbPayeeAccount != null)
                {
                    DataTable ldtbReportTable01 = new DataTable();
                    ldtbReportTable01.TableName = "ReportTable01";

                    //Required Columns in report
                    ldtbReportTable01.Columns.Add("PLAN_YEAR", typeof(int));
                    ldtbReportTable01.Columns.Add("MPI_PERSON_ID", typeof(string));
                    ldtbReportTable01.Columns.Add("PARTICIPANT_NAME", typeof(string));
                    ldtbReportTable01.Columns.Add("PLAN_NAME", typeof(string));
                    ldtbReportTable01.Columns.Add("GROSS_AMOUNT", typeof(decimal));
                    ldtbReportTable01.Columns.Add("RETIREMENT_BENEFIT_AMOUNT", typeof(decimal));
                    ldtbReportTable01.Columns.Add("STATE_TAX_AMOUNT", typeof(decimal));
                    ldtbReportTable01.Columns.Add("FEDERAL_TAX_AMOUNT", typeof(decimal));
                    ldtbReportTable01.Columns.Add("NET_AMOUNT", typeof(decimal));
                    ldtbReportTable01.Columns.Add("PAYMENT_METHOD", typeof(string));
                    ldtbReportTable01.Columns.Add("PAY_TO", typeof(string));
                    ldtbReportTable01.Columns.Add("RETIREMENT_DATE", typeof(DateTime));
                    ldtbReportTable01.Columns.Add("MD_AGE", typeof(string));
                    ldtbReportTable01.Columns.Add("RETIREE_INCREASE_ELIGIBLE", typeof(string));
                    ldtbReportTable01.Columns.Add("ROLLOVER_ELIGIBLE", typeof(string));
                    ldtbReportTable01.Columns.Add("ROLLOVER_Group", typeof(string));
                    ldtbReportTable01.Columns.Add("PERSON_TYPE", typeof(string));
                    ldtbReportTable01.Columns.Add("PAYEE_ACCOUNT_ID", typeof(int));
                    ldtbReportTable01.Columns.Add("Status", typeof(string)); // Wasim 09/10/2014
                    ldtbReportTable01.Columns.Add("BENEFIT_TYPE", typeof(string)); // PIR RID 71870
                    ldtbReportTable01.Columns.Add("RETIREMENT_TYPE", typeof(string)); // PIR RID 71870
                    ldtbReportTable01.Columns.Add("MD_DATE", typeof(DateTime)); // PIR RID 71870

                    var lclbReportData = iclbPayeeAccount;

                    foreach (var row in lclbReportData)
                    {
                        decimal idecState = 0.0M;
                        decimal idecFed = 0.0M;
                        DataRow dr = ldtbReportTable01.NewRow();
                        dr["PLAN_YEAR"] = row.icdoPayeeAccount.intPlanYear;
                        dr["MPI_PERSON_ID"] = row.icdoPayeeAccount.istrMPID;
                        dr["PARTICIPANT_NAME"] = row.icdoPayeeAccount.istrParticipantName;
                        dr["PLAN_NAME"] = row.icdoPayeeAccount.istrPlanDescription;
                        dr["GROSS_AMOUNT"] = row.icdoPayeeAccount.idecRetireeIncAmt;
                        dr["RETIREMENT_BENEFIT_AMOUNT"] = row.icdoPayeeAccount.idecGrossAmt;
                        dr["STATE_TAX_AMOUNT"] = Decimal.Zero;
                        dr["FEDERAL_TAX_AMOUNT"] = Decimal.Zero;//row.icdoPayeeAccount.idecStateTax;
                        dr["Status"] = row.icdoPayeeAccount.istrPayeeAccountCurrentStatus; // Wasim 09/10/2014
                        dr["BENEFIT_TYPE"] = row.icdoPayeeAccount.benefit_account_type_description; // PIR RID 71870
                        dr["RETIREMENT_TYPE"] = row.icdoPayeeAccount.retirement_type_description; // PIR RID 71870
                        dr["MD_DATE"] = row.icdoPayeeAccount.idtMDDate; // PIR RID 71870

                        if (row.icdoPayeeAccount.istrContactName.IsNullOrEmpty() && (row.icdoPayeeAccount.idecRetireeIncAmt < 750 || row.icdoPayeeAccount.istrMDAge == busConstant.FLAG_YES))
                        {
                            DataTable ldtblTaxOption = busBase.Select("cdoTempdata.GetTaxOptionRetireeIncreaseBatch", new object[1] { row.icdoPayeeAccount.payee_account_id });
                            if (ldtblTaxOption != null && ldtblTaxOption.Rows.Count > 0)
                            {
                                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
                                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.LoadData(ldtblTaxOption.Rows[0]);
                                
                                if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() && Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FLAT_DOLLAR)
                                {
                                    idecFed = (row.icdoPayeeAccount.idecFederalTax * Convert.ToInt32(row.icdoPayeeAccount.istrPercentIncrease)) / 100;//Decimal.Zero - row.icdoPayeeAccount.idecRetireeIncAmt * 0.1M;
                                    dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                                }
                                else if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() &&
                                    (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX
                                        || Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FEDRAL_TAX_IRS_TABLE))
                                {
                                    //2022 FDRL tax withholding. changes for new federal tax withholding method. Added additional parameters to pass withholdingoption and retiree increase batch flag.
                                    idecFed = Decimal.Zero - busPayeeAccountHelper.CalculateFedOrStateTax(row.icdoPayeeAccount.idecRetireeIncAmt, lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance,
                                                              new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.Year, 11, 01), lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value,
                                                              lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value,
                                                              lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount, lbusPayeeAccountTaxWithholding, false, null, true);  
                                    dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                                }
                                else if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() &&
                                    Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FLAT_PERCENT)
                                {
                                    idecFed = Decimal.Zero - (row.icdoPayeeAccount.idecRetireeIncAmt * lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_percentage);
                                    dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                                }
                                
                            }

                            //State Tax
                            ldtblTaxOption = busBase.Select("cdoTempdata.GetTaxOptionRetireeIncreaseBatchStateTax", new object[1] { row.icdoPayeeAccount.payee_account_id });
                            if (ldtblTaxOption != null && ldtblTaxOption.Rows.Count > 0)
                            {
                                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
                                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.LoadData(ldtblTaxOption.Rows[0]);

                                if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() && Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FLAT_DOLLAR)
                                {
                                    idecState = (row.icdoPayeeAccount.idecStateTax * Convert.ToInt32(row.icdoPayeeAccount.istrPercentIncrease)) / 100;//Decimal.Zero - row.icdoPayeeAccount.idecRetireeIncAmt * 0.1M;
                                    dr["STATE_TAX_AMOUNT"] = idecState;
                                }
                                else if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() &&
                                    (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.StateTaxOptionFedTaxBasedOnIRS
                                        || Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.StateTaxOptionFedTaxBasedOnIRSAndAdditional))
                                {
                                    idecState = Decimal.Zero - busPayeeAccountHelper.CalculateFedOrStateTax(row.icdoPayeeAccount.idecRetireeIncAmt, lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance,
                                                              new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.Year, 11, 01), lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value,
                                                              lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value,
                                                              lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount);
                                    dr["STATE_TAX_AMOUNT"] = idecState;
                                }
                                else if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() &&
                                    Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FLAT_PERCENT)
                                {
                                    idecState = Decimal.Zero - (row.icdoPayeeAccount.idecRetireeIncAmt * lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_percentage);
                                    dr["STATE_TAX_AMOUNT"] = idecState;
                                }
                            }

                        }
                        else if (row.icdoPayeeAccount.istrContactName.IsNullOrEmpty()
                                   && row.icdoPayeeAccount.idecRetireeIncAmt >= 750)
                        {
                            //if (row.icdoPayeeAccount.istrPersonType == "Participant Account")
                            //{
                                idecFed = Decimal.Zero - row.icdoPayeeAccount.idecRetireeIncAmt * 0.2M;
                                dr["FEDERAL_TAX_AMOUNT"] = idecFed;

                            //}
                            //else
                            //{
                            //    if (row.icdoPayeeAccount.istrPersonType == "Joint Annuitant")
                            //    {
                            //        idecFed = Decimal.Zero - row.icdoPayeeAccount.idecRetireeIncAmt * 0.2M;
                            //        dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                            //    }
                            //    else
                            //    {
                            //        idecFed = Decimal.Zero - row.icdoPayeeAccount.idecRetireeIncAmt * 0.1M;
                            //        dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                            //    }
                            //}
                        }
                        else
                        {
                            dr["FEDERAL_TAX_AMOUNT"] = Decimal.Zero;
                        }
                        dr["NET_AMOUNT"] = row.icdoPayeeAccount.idecRetireeIncAmt + Convert.ToDecimal(dr["FEDERAL_TAX_AMOUNT"]);//+ Convert.ToDecimal(dr["STATE_TAX_AMOUNT"]);
                        dr["PAYMENT_METHOD"] = row.icdoPayeeAccount.istrPaymentMethod;
                        dr["PAY_TO"] = row.icdoPayeeAccount.istrContactName;
                        dr["RETIREMENT_DATE"] = row.icdoPayeeAccount.idtRetireMentDate;
                        dr["MD_AGE"] = row.icdoPayeeAccount.istrMDAge;
                        dr["RETIREE_INCREASE_ELIGIBLE"] = row.icdoPayeeAccount.istrRetireeIncreaseEligible;
                        dr["ROLLOVER_ELIGIBLE"] = row.icdoPayeeAccount.istrRolloverEligible;
                        dr["ROLLOVER_Group"] = row.icdoPayeeAccount.istrRolloverGroup;
                        dr["PERSON_TYPE"] = row.icdoPayeeAccount.istrPersonType;
                        if (row.icdoPayeeAccount != null && Convert.ToString(row.icdoPayeeAccount.payee_account_id).IsNotNullOrEmpty())
                            dr["PAYEE_ACCOUNT_ID"] = row.icdoPayeeAccount.payee_account_id;
                        ldtbReportTable01.Rows.Add(dr);
                    }

                    if (ldtbReportTable01 != null && ldtbReportTable01.Rows.Count > 0)
                        CreateExcelReport(ldtbReportTable01, "rptRetireeIncreaseReport");
                }
                #endregion

                #region commented code for Create Retiree Increase Report
                //if (iclbPayeeAccount != null)
                //{
                //    DataTable ldtbReportTable01 = new DataTable();
                //    ldtbReportTable01.TableName = "ReportTable01";

                //    //Required Columns in report
                //    ldtbReportTable01.Columns.Add("PLAN_YEAR", typeof(int));
                //    ldtbReportTable01.Columns.Add("MPI_PERSON_ID", typeof(string));
                //    ldtbReportTable01.Columns.Add("PARTICIPANT_NAME", typeof(string));
                //    ldtbReportTable01.Columns.Add("PLAN_NAME", typeof(string));
                //    ldtbReportTable01.Columns.Add("GROSS_AMOUNT", typeof(decimal));
                //    ldtbReportTable01.Columns.Add("RETIREMENT_BENEFIT_AMOUNT", typeof(decimal));
                //    ldtbReportTable01.Columns.Add("STATE_TAX_AMOUNT", typeof(decimal));
                //    ldtbReportTable01.Columns.Add("FEDERAL_TAX_AMOUNT", typeof(decimal));
                //    ldtbReportTable01.Columns.Add("NET_AMOUNT", typeof(decimal));
                //    ldtbReportTable01.Columns.Add("PAYMENT_METHOD", typeof(string));
                //    ldtbReportTable01.Columns.Add("PAY_TO", typeof(string));
                //    ldtbReportTable01.Columns.Add("RETIREMENT_DATE", typeof(DateTime));
                //    ldtbReportTable01.Columns.Add("MD_AGE", typeof(string));
                //    ldtbReportTable01.Columns.Add("RETIREE_INCREASE_ELIGIBLE", typeof(string));
                //    ldtbReportTable01.Columns.Add("ROLLOVER_ELIGIBLE", typeof(string));
                //    ldtbReportTable01.Columns.Add("ROLLOVER_Group", typeof(string));
                //    ldtbReportTable01.Columns.Add("PERSON_TYPE", typeof(string));

                //    var lclbReportData = iclbPayeeAccount;

                //    foreach (var row in lclbReportData)
                //    {
                //        decimal idecFed = 0.0M;
                //        DataRow dr = ldtbReportTable01.NewRow();
                //        dr["PLAN_YEAR"] = row.icdoPayeeAccount.intPlanYear;
                //        dr["MPI_PERSON_ID"] = row.icdoPayeeAccount.istrMPID;
                //        dr["PARTICIPANT_NAME"] = row.icdoPayeeAccount.istrParticipantName;
                //        dr["PLAN_NAME"] = row.icdoPayeeAccount.istrPlanDescription;
                //        dr["GROSS_AMOUNT"] = row.icdoPayeeAccount.idecRetireeIncAmt;
                //        dr["RETIREMENT_BENEFIT_AMOUNT"] = row.icdoPayeeAccount.idecGrossAmt;
                //        dr["STATE_TAX_AMOUNT"] = Decimal.Zero;
                //        dr["FEDERAL_TAX_AMOUNT"] = Decimal.Zero;//row.icdoPayeeAccount.idecStateTax;
                //        if (row.icdoPayeeAccount.istrContactName.IsNullOrEmpty()
                //                   && row.icdoPayeeAccount.idecRetireeIncAmt >= 750)
                //        {
                //            if (row.icdoPayeeAccount.istrPersonType == "Participant Account")
                //            {
                //                if (row.icdoPayeeAccount.istrMDAge == busConstant.FLAG_YES)
                //                {
                //                    DataTable ldtblTaxOption = busBase.Select("cdoTempdata.GetTaxOptionRetireeIncreaseBatch", new object[1] { row.icdoPayeeAccount.payee_account_id });
                //                    if (ldtblTaxOption != null && ldtblTaxOption.Rows.Count > 0)
                //                    {
                //                        busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
                //                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.LoadData(ldtblTaxOption.Rows[0]);

                //                        if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() && Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FLAT_DOLLAR)
                //                        {
                //                            idecFed = (row.icdoPayeeAccount.idecFederalTax * Convert.ToInt32(row.icdoPayeeAccount.istrPercentIncrease)) / 100;//Decimal.Zero - row.icdoPayeeAccount.idecRetireeIncAmt * 0.1M;
                //                            dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                //                        }
                //                        else if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() &&
                //                            (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX
                //                                || Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FEDRAL_TAX_IRS_TABLE))
                //                        {
                //                            idecFed = Decimal.Zero - busPayeeAccountHelper.CalculateFedOrStateTax(row.icdoPayeeAccount.idecRetireeIncAmt, lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance,
                //                                                      new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.Year, 11, 01), lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value,
                //                                                      lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value,
                //                                                      lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount);
                //                            dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                //                        }
                //                        else if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() &&
                //                            Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FLAT_PERCENT)
                //                        {
                //                            idecFed = Decimal.Zero - (row.icdoPayeeAccount.idecRetireeIncAmt * lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_percentage);
                //                            dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                //                        }
                //                    }
                //                }
                //                else
                //                {
                //                    idecFed = Decimal.Zero - row.icdoPayeeAccount.idecRetireeIncAmt * 0.2M;
                //                    dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                //                }
                //            }
                //            else
                //            {
                //                if (row.icdoPayeeAccount.istrPersonType == "Joint Annuitant")
                //                {
                //                    idecFed = Decimal.Zero - row.icdoPayeeAccount.idecRetireeIncAmt * 0.2M;
                //                    dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                //                }
                //                else
                //                {
                //                    idecFed = Decimal.Zero - row.icdoPayeeAccount.idecRetireeIncAmt * 0.1M;
                //                    dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                //                }
                //            }
                //        }
                //        else if (row.icdoPayeeAccount.istrContactName.IsNullOrEmpty() && row.icdoPayeeAccount.idecRetireeIncAmt < 750)
                //        {
                //            DataTable ldtblTaxOption = busBase.Select("cdoTempdata.GetTaxOptionRetireeIncreaseBatch", new object[1] { row.icdoPayeeAccount.payee_account_id });
                //            if (ldtblTaxOption != null && ldtblTaxOption.Rows.Count > 0)
                //            {
                //                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
                //                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.LoadData(ldtblTaxOption.Rows[0]);

                //                if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() && Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FLAT_DOLLAR)
                //                {
                //                    idecFed = (row.icdoPayeeAccount.idecFederalTax * Convert.ToInt32(row.icdoPayeeAccount.istrPercentIncrease)) / 100;//Decimal.Zero - row.icdoPayeeAccount.idecRetireeIncAmt * 0.1M;
                //                    dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                //                }
                //                else if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() &&
                //                    (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX
                //                        || Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FEDRAL_TAX_IRS_TABLE))
                //                {
                //                    idecFed = Decimal.Zero - busPayeeAccountHelper.CalculateFedOrStateTax(row.icdoPayeeAccount.idecRetireeIncAmt, lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance,
                //                                              new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.Year, 11, 01), lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value,
                //                                              lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value,
                //                                              lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount);
                //                    dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                //                }
                //                else if (Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]).IsNotNullOrEmpty() &&
                //                    Convert.ToString(ldtblTaxOption.Rows[0][enmPayeeAccountTaxWithholding.tax_option_value.ToString()]) == busConstant.FLAT_PERCENT)
                //                {
                //                    idecFed = Decimal.Zero - (row.icdoPayeeAccount.idecRetireeIncAmt * lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_percentage);
                //                    dr["FEDERAL_TAX_AMOUNT"] = idecFed;
                //                }
                //            }

                //        }
                //        else
                //        {
                //            dr["FEDERAL_TAX_AMOUNT"] = Decimal.Zero;
                //        }
                //        dr["NET_AMOUNT"] = row.icdoPayeeAccount.idecRetireeIncAmt + Convert.ToDecimal(dr["FEDERAL_TAX_AMOUNT"]);//+ Convert.ToDecimal(dr["STATE_TAX_AMOUNT"]);
                //        dr["PAYMENT_METHOD"] = row.icdoPayeeAccount.istrPaymentMethod;
                //        dr["PAY_TO"] = row.icdoPayeeAccount.istrContactName;
                //        dr["RETIREMENT_DATE"] = row.icdoPayeeAccount.idtRetireMentDate;
                //        dr["MD_AGE"] = row.icdoPayeeAccount.istrMDAge;
                //        dr["RETIREE_INCREASE_ELIGIBLE"] = row.icdoPayeeAccount.istrRetireeIncreaseEligible;
                //        dr["ROLLOVER_ELIGIBLE"] = row.icdoPayeeAccount.istrRolloverEligible;
                //        dr["ROLLOVER_Group"] = row.icdoPayeeAccount.istrRolloverGroup;
                //        dr["PERSON_TYPE"] = row.icdoPayeeAccount.istrPersonType;
                //        ldtbReportTable01.Rows.Add(dr);
                //    }

                //    if (ldtbReportTable01 != null && ldtbReportTable01.Rows.Count > 0)
                //        CreateExcelReport(ldtbReportTable01, "rptRetireeIncreaseReport");
                //}
                #endregion
            }
            MergePdfsFromPath();
        }

        private void GenerateCorrespondence(DataTable adtPersonInfo, DataRow acdoPayeeAccount, int aintComputationYear, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        {
            ArrayList aarrResult = new ArrayList();
            Hashtable ahtbQueryBkmarks = new Hashtable();
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
                // pir - 783 for checking bad address flag
                string lstrActiveAddr = busConstant.Flag_Yes;
                if (!string.IsNullOrEmpty(Convert.ToString(acdoPayeeAccount["ACTIVE_ADDRESS"])))
                {
                    lstrActiveAddr = Convert.ToString(acdoPayeeAccount["ACTIVE_ADDRESS"]);
                }
                int lintPlanId = Convert.ToInt32(acdoPayeeAccount[enmPlanBenefitXr.plan_id.ToString()]);
                int lintPayementCount = 0, lintNonSuspendibleMonth = 0;
                decimal ldecGrossAmount = 0;

                busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                busPayeeAccount lbusParticipantPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                DateTime ldteNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate(lintPlanId).AddMonths(1);
                lbusPayeeAccount.istrDistributionType = "Lump Sum";
                LoadDataForRetireeIncreasePayeeAccount(lbusPerson, lbusPayeeAccount, acdoPayeeAccount, aintComputationYear,
                                                                      ref lintPayementCount, ref lintNonSuspendibleMonth, ref ldecGrossAmount);
                //RID 75975
                if ((lbusPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form == busConstant.FLAG_NO ||
                     lbusPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form.IsNullOrEmpty()) &&
                    (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING ||
                    (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED //&& lintPayementCount >= 1
                    ) ||
                    (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED //&& lintPayementCount >= 1
                    ) ||
                    (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_REVIEW //&& lintPayementCount >= 1
                    )) &&
                    ((lintNonSuspendibleMonth >= 1 || lbusPayeeAccount.icdoPayeeAccount.istrMDAge == busConstant.FLAG_YES) && lbusPerson.icdoPerson.date_of_death == DateTime.MinValue) && ldecGrossAmount != 0)
                {
                    decimal ldecGuaranteedAmt = 0;
                    DataTable ldtbParticipantPayeeAccountDetails = new DataTable();

                    if (Convert.ToString(acdoPayeeAccount[enmPayeeAccount.benefit_account_type_value.ToString()]) != busConstant.BENEFIT_TYPE_QDRO && adtPersonInfo.AsEnumerable().Where(
                                     item => item.Field<int>(enmPayeeAccount.payee_benefit_account_id.ToString()) == lbusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id &&
                                     item.Field<string>(enmPayeeAccount.benefit_account_type_value.ToString()) == busConstant.BENEFIT_TYPE_QDRO &&
                                     item.Field<string>(enmDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase.ToString()) == busConstant.FLAG_YES).Count() == 0)
                    {
                        decimal ldecRetireeIncAmt = 0M;
                        if (acdoPayeeAccount["RetireeIncAmt"] != DBNull.Value)
                            ldecRetireeIncAmt = Convert.ToDecimal(acdoPayeeAccount["RetireeIncAmt"]);

                        lbusPayeeAccount.idecRetireeIncAmount = ldecRetireeIncAmt;
                        SetPayeeAccountInfo(lbusPayeeAccount, aintComputationYear, acdoPayeeAccount);

                        if (lbusPayeeAccount.idecRetireeIncAmount >= 750M && lbusPayeeAccount.icdoPayeeAccount.istrMDAge != busConstant.FLAG_YES)
                            lbusPayeeAccount.istrShowRollOver = busConstant.FLAG_YES;

                        aarrResult.Add(lbusPayeeAccount);
                        iclbPayeeAccount.Add(lbusPayeeAccount);

                        if (ldecGrossAmount != 0)
                        {
                            this.CreateCorrespondence(busConstant.RETIREE_INCREASE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true, lstrActiveAddr);
                            //this.CreateCorrespondence(busConstant.LUMP_SUM_DISTRIBUTION_ELECTION_FROM_PAYEE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks,true);
                        }

                        if (acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()].ToString().IsNotNullOrEmpty())
                        {
                            if (Convert.ToString(acdoPayeeAccount["GUARANTEED_AMOUNT"]).IsNotNullOrEmpty())
                            {
                                #region Generate correspondence for John Hancock

                                busPayeeAccount lbusJohnHancockPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                SetPayeeAccountInfo(lbusJohnHancockPayeeAccount, aintComputationYear, acdoPayeeAccount);
                                decimal ldecRetireeIncAmtForJohnHancock = 0M;
                                ldecRetireeIncAmtForJohnHancock = Convert.ToDecimal(acdoPayeeAccount["GUARANTEED_AMOUNT"]);
                                lbusJohnHancockPayeeAccount.idecRetireeIncAmount = ldecRetireeIncAmtForJohnHancock;
                                lbusJohnHancockPayeeAccount.icdoPayeeAccount.idecRetireeIncAmt = ldecRetireeIncAmtForJohnHancock;

                                if (lbusJohnHancockPayeeAccount.idecRetireeIncAmount >= 750M && lbusJohnHancockPayeeAccount.icdoPayeeAccount.istrMDAge != busConstant.FLAG_YES)
                                    lbusJohnHancockPayeeAccount.istrShowRollOver = busConstant.FLAG_YES;

                                lbusJohnHancockPayeeAccount.icdoPayeeAccount.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                                //SetPayeeAccountInfo(lbusPayeeAccount, aintComputationYear, acdoPayeeAccount);
                                aarrResult.Add(lbusJohnHancockPayeeAccount);
                                iclbPayeeAccount.Add(lbusJohnHancockPayeeAccount);

                                if (ldecGrossAmount != 0)
                                {
                                    this.CreateCorrespondence(busConstant.RETIREE_INCREASE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true, lstrActiveAddr);
                                }
                                #endregion
                            }
                        }
                    }
                    else if (Convert.ToString(acdoPayeeAccount[enmPayeeAccount.benefit_account_type_value.ToString()]) == busConstant.BENEFIT_TYPE_QDRO)
                    {
                        decimal ldecRetireeIncAmt = 0M;
                        if (acdoPayeeAccount["RetireeIncAmt"] != DBNull.Value)
                            ldecRetireeIncAmt = Convert.ToDecimal(acdoPayeeAccount["RetireeIncAmt"]);

                        lbusPayeeAccount.idecRetireeIncAmount = ldecRetireeIncAmt;
                        SetPayeeAccountInfo(lbusPayeeAccount, aintComputationYear, acdoPayeeAccount);

                        if (lbusPayeeAccount.idecRetireeIncAmount >= 750M && lbusPayeeAccount.icdoPayeeAccount.istrMDAge != busConstant.FLAG_YES)
                            lbusPayeeAccount.istrShowRollOver = busConstant.FLAG_YES;

                        aarrResult.Add(lbusPayeeAccount);
                        iclbPayeeAccount.Add(lbusPayeeAccount);


                        if (ldecGrossAmount != 0)
                        {
                            this.CreateCorrespondence(busConstant.RETIREE_INCREASE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true, lstrActiveAddr);
                            //this.CreateCorrespondence(busConstant.LUMP_SUM_DISTRIBUTION_ELECTION_FROM_PAYEE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks,true);
                        }

                        if (acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()].ToString().IsNotNullOrEmpty())
                        {
                            ldecGuaranteedAmt = Convert.ToDecimal(acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()]);

                            if (ldecGuaranteedAmt > 0)
                            {
                                #region Generate correspondence for John Hancock
                                busPayeeAccount lbusJohnHancockPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                SetPayeeAccountInfo(lbusJohnHancockPayeeAccount, aintComputationYear, acdoPayeeAccount);

                                decimal ldecRetireeIncAmtForJohnHancock = 0M;
                                if (acdoPayeeAccount["GUARANTEED_AMOUNT"] != DBNull.Value)
                                    ldecRetireeIncAmtForJohnHancock = Convert.ToDecimal(acdoPayeeAccount["GUARANTEED_AMOUNT"]);
                                lbusJohnHancockPayeeAccount.idecRetireeIncAmount = ldecRetireeIncAmtForJohnHancock;
                                lbusJohnHancockPayeeAccount.icdoPayeeAccount.idecRetireeIncAmt = ldecRetireeIncAmtForJohnHancock;

                                if (lbusJohnHancockPayeeAccount.idecRetireeIncAmount >= 750M && lbusJohnHancockPayeeAccount.icdoPayeeAccount.istrMDAge != busConstant.FLAG_YES)
                                    lbusJohnHancockPayeeAccount.istrShowRollOver = busConstant.FLAG_YES;
                                lbusJohnHancockPayeeAccount.icdoPayeeAccount.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                                //SetPayeeAccountInfo(lbusPayeeAccount, aintComputationYear, acdoPayeeAccount);
                                aarrResult.Add(lbusJohnHancockPayeeAccount);
                                iclbPayeeAccount.Add(lbusJohnHancockPayeeAccount);

                                if (ldecGrossAmount != 0)
                                {
                                    this.CreateCorrespondence(busConstant.RETIREE_INCREASE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true, lstrActiveAddr);
                                }

                                #endregion
                            }
                        }

                        if (Convert.ToString(acdoPayeeAccount[enmDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase.ToString()]) == busConstant.FLAG_YES)
                        {
                            if (adtPersonInfo.AsEnumerable().Where(
                                         item => item.Field<int>(enmPayeeAccount.payee_benefit_account_id.ToString()) == lbusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id &&
                                         item.Field<string>(enmPayeeAccount.benefit_account_type_value.ToString()) != busConstant.BENEFIT_TYPE_QDRO).Count() > 0)
                            {
                                ldtbParticipantPayeeAccountDetails = adtPersonInfo.AsEnumerable().Where(
                                             item => item.Field<int>(enmPayeeAccount.payee_benefit_account_id.ToString()) == lbusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id &&
                                             item.Field<string>(enmPayeeAccount.benefit_account_type_value.ToString()) != busConstant.BENEFIT_TYPE_QDRO).CopyToDataTable();
                            }

                            if (ldtbParticipantPayeeAccountDetails.Rows.Count > 0)
                            {
                                int lintParticipantPaymentCount = 0, lintParticipantNonSuspendibleMonth = 0;
                                decimal ldecParticipantGrossAmount = 0;

                                busPerson lbusParticipant = new busPerson { icdoPerson = new cdoPerson() };

                                LoadDataForRetireeIncreasePayeeAccount(lbusParticipant, lbusParticipantPayeeAccount, ldtbParticipantPayeeAccountDetails.Rows[0], aintComputationYear,
                                                                        ref lintParticipantPaymentCount, ref lintParticipantNonSuspendibleMonth, ref ldecParticipantGrossAmount);
                                //RID 75975
                                if ((lbusParticipantPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form == busConstant.FLAG_NO ||
                                        lbusParticipantPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form.IsNullOrEmpty()) &&
                                        (ldtbParticipantPayeeAccountDetails.Rows[0]["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING ||
                                         (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED /*&& lintParticipantPaymentCount >= 1*/) ||
                                        (ldtbParticipantPayeeAccountDetails.Rows[0]["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED /*&& lintParticipantPaymentCount >= 1*/) ||
                                        (ldtbParticipantPayeeAccountDetails.Rows[0]["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_REVIEW /*&& lintParticipantPaymentCount >= 1*/)) &&
                                        ((lintParticipantNonSuspendibleMonth >= 1 || lbusParticipantPayeeAccount.icdoPayeeAccount.istrMDAge == busConstant.FLAG_YES) && lbusParticipant.icdoPerson.date_of_death == DateTime.MinValue) && ldecParticipantGrossAmount != 0)
                                {
                                    decimal ldecParticipantRetireeIncAmt = 0M;
                                    if (acdoPayeeAccount[enmActiveRetireeIncreaseContract.percent_increase_value.ToString()] != DBNull.Value)
                                        ldecParticipantRetireeIncAmt = Convert.ToDecimal(acdoPayeeAccount["RetireeIncAmt"]);

                                    lbusParticipantPayeeAccount.idecRetireeIncAmount = ldecParticipantRetireeIncAmt;
                                    lbusParticipantPayeeAccount.icdoPayeeAccount.person_id = lbusPayeeAccount.icdoPayeeAccount.person_id;
                                    SetPayeeAccountInfo(lbusParticipantPayeeAccount, aintComputationYear, acdoPayeeAccount);

                                    if (lbusParticipantPayeeAccount.idecRetireeIncAmount >= 750M && lbusParticipantPayeeAccount.icdoPayeeAccount.istrMDAge != busConstant.FLAG_YES)
                                        lbusParticipantPayeeAccount.istrShowRollOver = busConstant.FLAG_YES;

                                    aarrResult.Add(lbusParticipantPayeeAccount);
                                    iclbPayeeAccount.Add(lbusParticipantPayeeAccount);

                                    if (ldecGrossAmount != 0)
                                    {
                                        this.CreateCorrespondence(busConstant.RETIREE_INCREASE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true, lstrActiveAddr);
                                        //this.CreateCorrespondence(busConstant.LUMP_SUM_DISTRIBUTION_ELECTION_FROM_PAYEE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks,true);
                                    }
                                    if (ldtbParticipantPayeeAccountDetails.Rows[0][enmLocal700GuaranteedAmount.guaranteed_amount.ToString()].ToString().IsNotNullOrEmpty())
                                    {
                                        decimal ldecParticipantGuaranteedAmt = Convert.ToDecimal(ldtbParticipantPayeeAccountDetails.Rows[0][enmLocal700GuaranteedAmount.guaranteed_amount.ToString()]);
                                        decimal ldecParticipantGuaranteedRetireeIncAmt = 0M;
                                        if (acdoPayeeAccount[enmActiveRetireeIncreaseContract.percent_increase_value.ToString()] != DBNull.Value)
                                            ldecParticipantGuaranteedRetireeIncAmt = Convert.ToDecimal(acdoPayeeAccount["GUARANTEED_AMOUNT"]);
                                        SetPayeeAccountInfo(lbusParticipantPayeeAccount, aintComputationYear, acdoPayeeAccount);
                                        lbusParticipantPayeeAccount.idecRetireeIncAmount = ldecParticipantGuaranteedRetireeIncAmt;
                                        lbusParticipantPayeeAccount.icdoPayeeAccount.person_id = lbusPayeeAccount.icdoPayeeAccount.person_id;

                                        if (lbusParticipantPayeeAccount.idecRetireeIncAmount >= 750M && lbusParticipantPayeeAccount.icdoPayeeAccount.istrMDAge != busConstant.FLAG_YES)
                                            lbusParticipantPayeeAccount.istrShowRollOver = busConstant.FLAG_YES;

                                        aarrResult.Add(lbusParticipantPayeeAccount);
                                        iclbPayeeAccount.Add(lbusParticipantPayeeAccount);

                                        if (ldecGrossAmount != 0)
                                        {
                                            this.CreateCorrespondence(busConstant.RETIREE_INCREASE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true, lstrActiveAddr);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                autlPassInfo.Commit();
            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    string lstrMsg = "Error while Executing Batch (Payee Account ID : " + Convert.ToInt32(acdoPayeeAccount[enmPayeeAccount.payee_account_id.ToString()]) + " , Error Message: " + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();
            }
        }

        private void SetPayeeAccountInfo(busPayeeAccount abusPayeeAccount, int aintComputationYear, DataRow acdoPayeeAccount)
        {
            if (abusPayeeAccount.ibusParticipant != null)
            {
                //abusPayeeAccount.ibusParticipant.LoadCorrAddress();
                //abusPayeeAccount.icdoPayeeAccount.istrPrefix = abusPayeeAccount.ibusParticipant.icdoPerson.istrPreFix;
                //abusPayeeAccount.icdoPayeeAccount.istrLastName = abusPayeeAccount.ibusParticipant.icdoPerson.last_name;
                //abusPayeeAccount.icdoPayeeAccount.istrAddrLine1 = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_1;
                //abusPayeeAccount.icdoPayeeAccount.istrAddrLine2 = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_2;
                //abusPayeeAccount.icdoPayeeAccount.istrCity = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_city;
                //abusPayeeAccount.icdoPayeeAccount.istrState = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_description;
                //abusPayeeAccount.icdoPayeeAccount.istrZipCode = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.istrCompleteZipCode;
                //abusPayeeAccount.icdoPayeeAccount.istrForeignPostalCode = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.foreign_postal_code;
                //abusPayeeAccount.icdoPayeeAccount.istrCountryDescription = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_description;
                //abusPayeeAccount.icdoPayeeAccount.istrCountryValue= abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value;
            }

            #region retiree Increase Report
            abusPayeeAccount.icdoPayeeAccount.istrParticipantName = acdoPayeeAccount["PARTICIPANT_NAME"] == DBNull.Value ? "" : acdoPayeeAccount["PARTICIPANT_NAME"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrMPID = acdoPayeeAccount["MPI_PERSON_ID"] == DBNull.Value ? "" : acdoPayeeAccount["MPI_PERSON_ID"].ToString();
            abusPayeeAccount.icdoPayeeAccount.intPlanYear = acdoPayeeAccount["PLAN_YEAR"] == DBNull.Value ? 0 : Convert.ToInt32(acdoPayeeAccount["PLAN_YEAR"]);
            abusPayeeAccount.icdoPayeeAccount.istrPlanDescription = acdoPayeeAccount["PLAN_NAME"] == DBNull.Value ? "" : acdoPayeeAccount["PLAN_NAME"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrMDAge = acdoPayeeAccount["MD_AGE"] == DBNull.Value ? "" : acdoPayeeAccount["MD_AGE"].ToString();
            abusPayeeAccount.icdoPayeeAccount.idecFederalTax = acdoPayeeAccount["FEDERAL_TAX_AMOUNT"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["FEDERAL_TAX_AMOUNT"]);
            abusPayeeAccount.icdoPayeeAccount.idecStateTax = acdoPayeeAccount["STATE_TAX_AMOUNT"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["STATE_TAX_AMOUNT"]);
            abusPayeeAccount.icdoPayeeAccount.idecNetAmount = acdoPayeeAccount["NET_AMOUNT"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["NET_AMOUNT"]);
            abusPayeeAccount.icdoPayeeAccount.istrRetireeIncreaseEligible = acdoPayeeAccount["RETIREE_INCREASE_ELIGIBLE"] == DBNull.Value ? "" : acdoPayeeAccount["RETIREE_INCREASE_ELIGIBLE"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrRolloverEligible = acdoPayeeAccount["ROLLOVER_ELIGIBLE"] == DBNull.Value ? "" : acdoPayeeAccount["ROLLOVER_ELIGIBLE"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrRolloverGroup = acdoPayeeAccount["ROLLOVER_Group"] == DBNull.Value ? "" : acdoPayeeAccount["ROLLOVER_Group"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrPaymentMethod = acdoPayeeAccount["PAYMENT_METHOD"] == DBNull.Value ? "" : acdoPayeeAccount["PAYMENT_METHOD"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrContactName = acdoPayeeAccount["CONTACT_NAME"] == DBNull.Value ? "" : acdoPayeeAccount["CONTACT_NAME"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrPersonType = acdoPayeeAccount["PERSON_TYPE"] == DBNull.Value ? "" : acdoPayeeAccount["PERSON_TYPE"].ToString();
            abusPayeeAccount.icdoPayeeAccount.idecRetireeIncAmt = acdoPayeeAccount["RetireeIncAmt"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["RetireeIncAmt"]);
            abusPayeeAccount.icdoPayeeAccount.idecGrossAmt = acdoPayeeAccount["idecGrossAmount"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["idecGrossAmount"]);
            abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate = acdoPayeeAccount["BENEFIT_BEGIN_DATE"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(acdoPayeeAccount["BENEFIT_BEGIN_DATE"]);
            abusPayeeAccount.icdoPayeeAccount.IS_ROLLOVER = acdoPayeeAccount["ROLLOVER_ELIGIBLE"] == DBNull.Value ? string.Empty : Convert.ToString(acdoPayeeAccount["ROLLOVER_ELIGIBLE"]);
            abusPayeeAccount.icdoPayeeAccount.istrPercentIncrease = acdoPayeeAccount["PERCENT_INCREASE"] == DBNull.Value ? string.Empty : Convert.ToString(acdoPayeeAccount["PERCENT_INCREASE"]);
            abusPayeeAccount.icdoPayeeAccount.istrPayeeAccountCurrentStatus = acdoPayeeAccount["Status"] == DBNull.Value ? string.Empty : Convert.ToString(acdoPayeeAccount["Status"]); // Wasim 09/10/2014
            abusPayeeAccount.icdoPayeeAccount.retirement_type_description = acdoPayeeAccount["RETIREMENT_TYPE"] == DBNull.Value ? string.Empty : Convert.ToString(acdoPayeeAccount["RETIREMENT_TYPE"]); // PIR RID 71870
            abusPayeeAccount.icdoPayeeAccount.idtMDDate = acdoPayeeAccount["MD_DATE"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(acdoPayeeAccount["MD_DATE"]);  //PIR RID 71870
            //busCode lbusDistributionCode = new busCode();  //PIR RID 71870
            //abusPayeeAccount.icdoPayeeAccount.retirement_type_description = Convert.ToString(abusPayeeAccount.icdoPayeeAccount.retirement_type_value).IsNotNullOrEmpty() ?
            //                          lbusDistributionCode.GetCodeValue(abusPayeeAccount.icdoPayeeAccount.retirement_type_id, abusPayeeAccount.icdoPayeeAccount.retirement_type_value).description : string.Empty;  //PIR RID 71870

            #endregion

            DateTime ldtDateTime = new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.Year, 11, 1);
            abusPayeeAccount.istrBenefitBeginDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtDateTime);
            abusPayeeAccount.istrMonthYear = iobjSystemManagement.icdoSystemManagement.batch_date.Year.ToString();
            abusPayeeAccount.iintPlanYear = aintComputationYear;
        }

        #endregion

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

            string lstrReportFullName = string.Empty;

            if (iblnIsRollover && iblnIsForeignAddress && !iblnIsNonRollover && !iblnLocalAddress)
            {
                astrPrefix = "Rollover_Foreign_And_Bad_Address";
            }
            else if (iblnIsRollover && !iblnIsForeignAddress && !iblnIsNonRollover && iblnLocalAddress)
            {
                astrPrefix = "Rollover_Local_Address";
            }
            else if (!iblnIsRollover && iblnIsForeignAddress && iblnIsNonRollover && !iblnLocalAddress)
            {
                astrPrefix = "NonRollover_Foreign_And_Bad_Address";
            }
            else if (!iblnIsRollover && !iblnIsForeignAddress && iblnIsNonRollover && iblnLocalAddress)
            {
                astrPrefix = "NonRollover_Local_Address";
            }

            if (astrPrefix.IsNotNullOrEmpty())
                lstrReportFullName = labsRptGenPath + astrReportName + "_" + astrPrefix + "_" +
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

        #region Load Data For Retiree Increase Payee Account
        private void LoadDataForRetireeIncreasePayeeAccount(busPerson abusPerson, busPayeeAccount abusPayeeAccount, DataRow acdoPayeeAccount,
                                                           int aintComputationYear, ref int aintPayementCount, ref int aintNonSuspendibleMonth, ref decimal adecGrossAmount)
        {
            int lintPlanId = Convert.ToInt32(acdoPayeeAccount[enmPlanBenefitXr.plan_id.ToString()]);
            busCalculation lbusCalculation = new busCalculation();

            //abusPerson = new busPerson { icdoPerson = new cdoPerson() };
            abusPerson.icdoPerson.LoadData(acdoPayeeAccount);

            #region Calculate non suspendible month count

            //DateTime ldtStartDate = new DateTime(aintComputationYear, 01, 01);
            //RID 75975
            //DateTime ldtendDate = new DateTime(aintComputationYear, 09, 30);
            //DateTime ldtendDate = new DateTime(aintComputationYear, 07, 31);
            DateTime ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(aintComputationYear, 01);
            DateTime ldtendDate = busGlobalFunctions.GetLastPayrollDayOfMonth(aintComputationYear, 07);

            #endregion

            abusPayeeAccount.icdoPayeeAccount.LoadData(acdoPayeeAccount);
            //abusPayeeAccount.LoadPayeeAccountPaymentItemType();
            // abusPayeeAccount.LoadGrossAmount();
            if (Convert.ToString(acdoPayeeAccount["idecGrossAmount"]).IsNotNullOrEmpty())
                adecGrossAmount = Convert.ToDecimal(acdoPayeeAccount["idecGrossAmount"]);

            if (abusPayeeAccount.ibusParticipant == null)
            {
                abusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                abusPayeeAccount.ibusParticipant.FindPerson(abusPayeeAccount.icdoPayeeAccount.person_id);
            }

            if (acdoPayeeAccount["Status"].ToString() != busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING)
            {
                //aintPayementCount = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.GetCountOfPaymentMade",
                //                                   new object[2] { abusPayeeAccount.icdoPayeeAccount.payee_account_id, abusPayeeAccount.icdoPayeeAccount.person_id }, iobjPassInfo.iconFramework,
                //                                   iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);

                DataTable ldtbPaymentInfo = busBase.Select("cdoPayeeAccount.GetPaymentCount", new object[2] {
                                    abusPayeeAccount.icdoPayeeAccount.payee_account_id, abusPayeeAccount.icdoPayeeAccount.person_id });

                aintPayementCount = ldtbPaymentInfo.Rows.Count;

            }

            if (abusPayeeAccount.icdoPayeeAccount.reemployed_flag == busConstant.FLAG_YES)
            {
                Dictionary<int, Dictionary<int, decimal>> ldictHoursAfterRetirement = new Dictionary<int, Dictionary<int, decimal>>();
                DateTime ldtLastWorkingDate = new DateTime();
                string lstrEmpName = string.Empty;

                int lintReemployedYear = 0;
                ldictHoursAfterRetirement = lbusCalculation.LoadMPIHoursAfterRetirementDate(abusPerson.icdoPerson.istrSSNNonEncrypted,
                    ldtStartDate.AddMonths(-1), busConstant.MPIPP_PLAN_ID, ref ldtLastWorkingDate, ref lstrEmpName, lintReemployedYear);
                abusPayeeAccount.ibusParticipant.LoadPersonSuspendibleMonth();
                //RID 75975
                //aintNonSuspendibleMonth = 9 - (lbusCalculation.GetSuspendibleMonthsBetweenTwoDates(ldictHoursAfterRetirement, abusPayeeAccount.ibusParticipant.iclbPersonSuspendibleMonth, ldtStartDate, ldtendDate));
                aintNonSuspendibleMonth = 7 - (lbusCalculation.GetSuspendibleMonthsBetweenTwoDates(ldictHoursAfterRetirement, abusPayeeAccount.ibusParticipant.iclbPersonSuspendibleMonth, ldtStartDate, ldtendDate));
                if (aintNonSuspendibleMonth > 0)
                {
                    abusPayeeAccount.LoadPayeeAccountStatuss();
                    if (!abusPayeeAccount.iclbPayeeAccountStatus.IsNullOrEmpty())
                    {
                        if (abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(item => item.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_value != busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED)
                        {
                            abusPayeeAccount.LoadReemploymentHistorys();
                            if (!abusPayeeAccount.iclcReemploymentHistory.IsNullOrEmpty())
                            {
                                if ((abusPayeeAccount.iclcReemploymentHistory.Where(item => item.reemployed_flag_to_date == DateTime.MinValue)).Count() > 0)
                                {

                                    if (abusPayeeAccount.icdoPayeeAccount.benefit_calculation_detail_id == 0)
                                    {
                                        DataTable ldtGross = new DataTable();
                                        ArrayList larrPaymentDate = lbusCalculation.GetNonSuspendibleMonthBetweenTwoDates(ldictHoursAfterRetirement, abusPayeeAccount.ibusParticipant.iclbPersonSuspendibleMonth, ldtStartDate, ldtendDate);

                                        if (larrPaymentDate != null && larrPaymentDate.Count > 0)
                                        {
                                            foreach (DateTime ldtPaymentDate in larrPaymentDate)
                                            {
                                                DateTime ldt = new DateTime(ldtPaymentDate.Year, ldtPaymentDate.Month, 01);
                                                ldtGross = busBase.Select("cdoPaymentHistoryDistribution.GetGrossAmountInAMonth", new object[2] { abusPayeeAccount.icdoPayeeAccount.payee_account_id, ldt });
                                                if (ldtGross.Rows.Count > 0)
                                                {
                                                    if (Convert.ToString(ldtGross.Rows[0]["Gross_Amount"]).IsNotNullOrEmpty())
                                                    {
                                                        adecGrossAmount = Convert.ToDecimal(ldtGross.Rows[0]["Gross_Amount"]);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    else
                                    {
                                        DataTable ldtGross = busBase.Select("cdoPaymentHistoryDistribution.GetBenefitAmtFromCalc", new object[1] { abusPayeeAccount.icdoPayeeAccount.payee_account_id });
                                        if (ldtGross.Rows.Count > 0)
                                        {
                                            if (Convert.ToString(ldtGross.Rows[0][0]).IsNotNullOrEmpty())
                                            {
                                                adecGrossAmount = Convert.ToDecimal(ldtGross.Rows[0][0]);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                aintNonSuspendibleMonth = lbusCalculation.GetNonSuspendibleMonths(abusPerson.icdoPerson.ssn, abusPerson,
                            aintComputationYear, lintPlanId, null, ldtStartDate, ldtendDate, false);

            }
        }
        #endregion

        #region Merged PDF
        // pir - 783 Merge pdf
        private void MergePdfsFromPath()
        {
            string lstrFilepath = iobjSystemManagement.icdoSystemManagement.base_directory + "RetireeIncreaseCorr\\" + DateTime.Now.Year;
            //Added : Shankar Dated : 01-07-2016 (Create New Dir. If Does not Exists ) Retiree Increase Batch DIR Not Exists
            if (!System.IO.Directory.Exists(lstrFilepath))
                System.IO.Directory.CreateDirectory(lstrFilepath);
            //End 
            MergePDFs(lstrFilepath);
        }

        private void MergePDFs(string generatedpath)
        {
            DirectoryInfo dir = new DirectoryInfo(generatedpath);
            List<string> filesPath = new List<string>();
            foreach (FileInfo fi in dir.GetFiles("*.*").OrderBy(item => item.CreationTime))
            {
                if (fi.CreationTime > ibusJobHeader.icdoJobHeader.start_time)
                    filesPath.Add(fi.FullName);
            }

            // Get selected file path contains pdf files
            List<string> SelectedfilesPath = new List<string>();
            SelectedfilesPath = filesPath.Where(obj => obj.Contains(".pdf")).ToList();
            // validate SelectedfilesPath is null or empty
            if (SelectedfilesPath != null && SelectedfilesPath.Count > 0)
            {
                List<PdfReader> readerList = new List<PdfReader>();
                foreach (string filePath in SelectedfilesPath)
                {
                    PdfReader pdfReader = new PdfReader(filePath);
                    readerList.Add(pdfReader);
                }

                // Declare/Define output file path 
                //PIR 977
                string lstrfilepath = generatedpath + "\\MergedFiles\\";
                if (!Directory.Exists(lstrfilepath))
                {
                    Directory.CreateDirectory(lstrfilepath);
                }
                //PIR 977
                string istrOutPutFilePath = GetMergePdfOutputFilePath(lstrfilepath);

                Console.WriteLine("Merging Files");
                Document document = null;
                PdfWriter writer = null;
                int iintPrintingCount = 0;
                foreach (PdfReader reader in readerList)
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        // validating to generate 5000 pages in one pdf file
                        if (iintPrintingCount == 0)
                        {
                            //Add datetime to output file path
                            string outPutFilePath = "";
                            outPutFilePath = istrOutPutFilePath + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                            //Define a new output document and its size, type
                            document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                            //Create blank output pdf file and get the stream to write on it.                            
                            writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create));
                            document.Open();
                        }

                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        document.Add(iTextSharp.text.Image.GetInstance(page));

                        iintPrintingCount++;
                        if (iintPrintingCount == 5000)
                        {
                            document.Close();
                            iintPrintingCount = 0;
                        }
                    }
                }
                if (document.IsOpen())
                {
                    document.Close();
                }
                Console.WriteLine("Closing Files");
            }
        }

        private string GetMergePdfOutputFilePath(string generatedpath)
        {
            string outPutFilePath = "";
            if (iblnIsRollover && iblnIsForeignAddress && !iblnIsNonRollover && !iblnLocalAddress)
            {
                outPutFilePath = generatedpath + "RI_Corr_Merged_Rollover_Foreign" + "_";
            }
            else if (iblnIsRollover && !iblnIsForeignAddress && !iblnIsNonRollover && iblnLocalAddress)
            {
                outPutFilePath = generatedpath + "RI_Corr_Merged_Rollover_Local" + "_";
            }
            else if (!iblnIsRollover && iblnIsForeignAddress && iblnIsNonRollover && !iblnLocalAddress)
            {
                outPutFilePath = generatedpath + "RI_Corr_Merged_NonRollover_Foreign" + "_";
            }
            else if (!iblnIsRollover && !iblnIsForeignAddress && iblnIsNonRollover && iblnLocalAddress)
            {
                outPutFilePath = generatedpath + "RI_Corr_Merged_NonRollover_Local" + "_";
            }
            else
            {
                outPutFilePath = generatedpath + "RI_Corr_Merged" + "_";
            }
            return outPutFilePath;
        }
        #endregion
    }
}
