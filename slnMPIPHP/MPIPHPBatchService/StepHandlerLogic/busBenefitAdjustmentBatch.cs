using System;
using System.Data;
using System.Collections.ObjectModel;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using System.Linq;
using Sagitec.Common;
using Microsoft.Reporting.WinForms;
using System.IO;


namespace MPIPHPJobService
{
    public class busBenefitAdjustmentBatch : busBatchHandler
    {
        #region Properties

        private object iobjLock = null;
        public busIapAllocationSummary ibusPrevYearAllocationSummary { get; set; }
        public DataTable idtLateHoursAndContributions { get; set; }
        public DataTable idtIAPPersonAccounts { get; set; }

        #endregion



        #region Benefit Adjustment Batch

        public void BenefitAdjustmentBatch()
        {
            busBase lobjBase = new busBase();

            //For Retirement
            DataTable ldtbResult = busBase.Select("cdoBenefitApplication.GetPayeeAccountsWithLateHoursAfterRetrment", new object[0] { });
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
            if (ldtbResult != null && ldtbResult.Rows.Count > 0)
                lclbPayeeAccount = lobjBase.GetCollection<busPayeeAccount>(ldtbResult, "icdoPayeeAccount");

            DataTable ldtbReportTable01 = new DataTable();
            ldtbReportTable01.TableName = "ReportTable01";

            //Required Columns in report
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
            ldtbReportTable01.Columns.Add("AMOUNT_DIFFERENCE", typeof(decimal));
            ldtbReportTable01.Columns.Add("PAYEE_ACCOUNT_ID", typeof(int));
            ldtbReportTable01.Columns.Add("COMMENT", typeof(string));
            ldtbReportTable01.Columns.Add("CALCULATION_ID", typeof(Int32));
            ldtbReportTable01.Columns.Add("INCREASE_ELIGIBLE", typeof(string));
            ldtbReportTable01.Columns.Add("REEMPLOYED", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_OPTION", typeof(string));
            ldtbReportTable01.Columns.Add("STATUS_VALUE", typeof(string));
            ldtbReportTable01.Columns.Add("SUSPENSION_STATUS_REASON_VALUE", typeof(string));
            ldtbReportTable01.Columns.Add("AGE", typeof(decimal));


            foreach (busPayeeAccount lbusPayeeAccount in lclbPayeeAccount)
            {
                ReCalculateRetirementBenefit(lbusPayeeAccount, ldtbReportTable01);
            }

            if (ldtbReportTable01 != null && ldtbReportTable01.Rows.Count > 0)
                ldtbReportTable01.Rows.Cast<DataRow>().Where(r => r.Field<string>("PLAN_NAME") == "MPIPP" && Convert.ToDecimal(r.Field<decimal?>("RETIREMENT_BENEFIT_AMOUNT")) == Convert.ToDecimal(r.Field<decimal?>("RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"))).ToList().ForEach(r1 => r1.Delete());
            ldtbReportTable01.AcceptChanges();

            CreateExcelReport(ldtbReportTable01, "rptPensionBenefitAdjustmentBatch");

        }


        public void ReCalculateRetirementBenefit(busPayeeAccount abusPayeeAccount, DataTable ldtbReportTable01)
        {
            DataRow dr = ldtbReportTable01.NewRow();


            LoadPayeeAccountDetails(ref abusPayeeAccount);
                     

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
            dr["REEMPLOYED"] = string.Empty;
            dr["STATUS_VALUE"] = abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_description;
            if (abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value.IsNotNullOrEmpty())
            {
                dr["SUSPENSION_STATUS_REASON_VALUE"] = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Payee_Account_Suspension_Reason_id,
                        abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value).description;
            }
            else
                dr["SUSPENSION_STATUS_REASON_VALUE"] = string.Empty;

            dr["AGE"] = Math.Round(busGlobalFunctions.CalculatePersonAge(abusPayeeAccount.ibusParticipant.icdoPerson.date_of_birth, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate), 2);


            try
            {
                int lintParticipantHasPendingDRO = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.CheckIfParticipantHasPendingDRO", new object[1] { abusPayeeAccount.icdoPayeeAccount.person_id },

                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                int lintParticipantDateOfDeath = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.CheckIfParticipantIsDead", new object[1] { abusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                int lintPendingAdjCalcCount = (int)DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.GetCountofPendingAdjCalc",
                                                                                new object[3] { abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value, abusPayeeAccount.icdoPayeeAccount.iintPlanId },
                                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if (lintParticipantHasPendingDRO > 0 && abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.BENEFIT_TYPE_QDRO)
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



                if (abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
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
                    lbusBenefitCalculationHeader = abusPayeeAccount.ReCalculateBenefitForRetirement(lstrBenefitOption, ablnPostRetDeath: true);//ablnPostRetDeath it needs to be true, else it will attach the calculation with the payee account if there are hours after retirement.
                    DateTime ldtVestedDt = new DateTime();
                    DataTable ldtGetVestedDate = busBase.Select("cdoPersonAccountEligibility.GetVestedDateForMD", new object[1] { lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.person_id });
                    if (ldtGetVestedDate != null && ldtGetVestedDate.Rows.Count > 0 && (Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]) != DateTime.MinValue))
                    {
                        ldtVestedDt = Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]);
                    }
                        if (abusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.MPIPP_PLAN_ID && abusPayeeAccount.icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_LATE && lbusBenefitCalculationHeader != null &&
                            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail != null && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Count > 0

                            && lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.age > busGlobalFunctions.GetMinDistributionAge(lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.person_id, ldtVestedDt) // busConstant.BenefitCalculation.AGE_70_HALF
                            && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null
                            && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Where(t => t.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).Count() <= 0)
                    {
                        lbusBenefitCalculationHeader.RecalculateMDBenefits(lbusBenefitCalculationHeader, abusPayeeAccount, ablnRetiree: true);
                    }
                    else
                    {
                        // DateTime ldtMDdt = new DateTime(lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.icdoPerson.date_of_birth.AddYears(70).AddMonths(6).Year + 1, 04, 01);
                        DateTime ldtMDdt = new DateTime();
                        ldtMDdt =  busGlobalFunctions.GetMinDistributionDate(lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.person_id, ldtVestedDt);

                        if (ldtMDdt.Year <= DateTime.Now.Year &&
                           lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail
                           .Where(t => t.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES && t.icdoBenefitCalculationYearlyDetail.plan_year >= ldtMDdt.Year).Count() > 0
                          )
                        {
                            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.istrBenefitOptionValue = lstrBenefitOption;
                            lbusBenefitCalculationHeader.RecalculateMDBenefits(lbusBenefitCalculationHeader, abusPayeeAccount, true);
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
                            dr["AGE"] = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.age > 0 ? Math.Round(lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.age, 2) : dr["AGE"];
                        }

                       
                        if (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null
                            && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Count > 0)
                        {
                           
                           if (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null &&
                                lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Count() > 0 &&
                                lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Where(t => t.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).Count() > 0)
                            {
                                dr["REEMPLOYED"] = busConstant.YES_CAPS;
                            }
                        }
                    }

                    if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id == 0)
                    {
                        dr["COMMENT"] += "Participant is not eligible for Retirement Benefits";
                        ldtbReportTable01.Rows.Add(dr);
                        return;
                    }

                    dr["AMOUNT_DIFFERENCE"] = Convert.ToDecimal(dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"]) - Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]);

                    ldtbReportTable01.Rows.Add(dr);
                }
                else if (abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                {
                    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                    lbusPlanBenefitXr.FindPlanBenefitXr(abusPayeeAccount.icdoPayeeAccount.plan_benefit_id);
                    string lstrBenefitOption = lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;

                    DataTable ldtbList = busBase.Select("cdoDroApplication.LoadAllExistingQDRO", new object[1] { abusPayeeAccount.icdoPayeeAccount.person_id });

                    if (ldtbList != null && ldtbList.Rows.Count > 0)
                    {
                        dr["COMMENT"] += "QDRO On File.";
                    }

                    busBenefitCalculationHeader lbusBenefitCalculationDisability = new busBenefitCalculationHeader();
                    lbusBenefitCalculationDisability = abusPayeeAccount.RecalculateParticipantDisabilityBenefits(lstrBenefitOption);

                    if (lbusBenefitCalculationDisability != null && lbusBenefitCalculationDisability.iclbBenefitCalculationDetail != null
                        && lbusBenefitCalculationDisability.iclbBenefitCalculationDetail.Count() > 0)
                    {
                        if (lbusBenefitCalculationDisability.iclbBenefitCalculationDetail.FirstOrDefault() != null &&
                            lbusBenefitCalculationDisability.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions != null
                            && lbusBenefitCalculationDisability.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Count() > 0)
                        {
                            dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"] = lbusBenefitCalculationDisability.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.disability_amount;
                            dr["CALCULATION_ID"] = lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                            dr["AGE"] = lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.age > 0 ? Math.Round(lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.age, 2) : dr["AGE"];
                        }
                    }

                   
                    if (lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null
                        && lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Count > 0)
                    {
                        if (lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null &&
                                lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Count() > 0 &&
                                lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Where(t => t.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).Count() > 0)
                        {
                            dr["REEMPLOYED"] = busConstant.YES_CAPS;
                        }
                    }

                    if (lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.benefit_calculation_header_id == 0)
                    {
                        dr["COMMENT"] += "Participant is not eligible for Retirement Benefits";
                        ldtbReportTable01.Rows.Add(dr);
                        return;

                    }

                    dr["AMOUNT_DIFFERENCE"] = Convert.ToDecimal(dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"]) - Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]);
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
             

        #endregion Benefit Adjustment Batch


        #region Benefit Adjustment Batch - Old Code
        //public void BenefitAdjustmentBatch()
        //{
        //    int lintCount = 0;
        //    int lintTotalCount = 0;

        //    utlConnection utlLegacyDBConnection = HelperFunction.GetDBConnectionProperties("Legacy");
        //    string astrLegacyDBConnection = utlLegacyDBConnection.istrConnectionString;

        //    utlConnection utlCoreDBConnection = HelperFunction.GetDBConnectionProperties("core");
        //    string astrCoreDBConnection = utlCoreDBConnection.istrConnectionString;

        //    Dictionary<string, object> ldictParams = new Dictionary<string, object>();
        //    foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
        //    {
        //        ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
        //    }
        //    //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
        //    iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
        //    utlPassInfo lobjMainPassInfo = iobjPassInfo;
        //    iobjLock = new object();


        //    SqlParameter[] LateHourparameters = new SqlParameter[1];
        //    SqlParameter LateHourparam1 = new SqlParameter("@BatchRunDate", DbType.DateTime);

        //    LateHourparam1.Value = iobjSystemManagement.icdoSystemManagement.batch_date;// DateTime.Now;
        //    LateHourparameters[0] = LateHourparam1;

        //    DataTable ldtLateHoursInformation = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_PensionLateHours", astrLegacyDBConnection, null, LateHourparameters);

        //    if ((ldtLateHoursInformation != null && ldtLateHoursInformation.Rows.Count <= 0) || ldtLateHoursInformation == null)
        //        return;

        //    DataTable ldtblIAPPayeeAccount = busBase.Select("cdoBenefitApplication.GetIAPPayeeAccountsForBenefitAdjustmentBatch", new object[0] { });


        //    SqlParameter[] parameters = new SqlParameter[1];
        //    SqlParameter param1 = new SqlParameter("@BATCH_RUN_DATE", DbType.DateTime);

        //    param1.Value = iobjSystemManagement.icdoSystemManagement.batch_date;
        //    parameters[0] = param1;

        //    DataTable ldtPersonInformation = busGlobalFunctions.ExecuteSPtoGetDataTable("Get_PreviousMonths_LateHours_And_Contributions", astrCoreDBConnection, null, parameters);
        //    if ((ldtPersonInformation != null && ldtPersonInformation.Rows.Count <= 0) || ldtPersonInformation == null)
        //        return;




        //    var collection = (from t1 in ldtPersonInformation.AsEnumerable()
        //                      join t2 in ldtLateHoursInformation.AsEnumerable()
        //                         on Convert.ToString(t1["MPI_PERSON_ID"]).Trim() equals Convert.ToString(t2["MPID"]).Trim()
        //                      where Convert.ToString(t2["REPORTSTATUS"]) == "L" && Convert.ToString(t1["RETIREMENT_DATE"]).IsNotNullOrEmpty()
        //                      && Convert.ToDateTime(t2["PROCESSEDDATE"]) > Convert.ToDateTime(t1["RETIREMENT_DATE"])
        //                         && Convert.ToDateTime(t2["PAYPERIODSTARTDATE"]) <= Convert.ToDateTime(t1["RETIREMENT_DATE"])
        //                      select
        //                      new
        //                      {
        //                          PERSON_ID = t1["PERSON_ID"],
        //                          MPI_PERSON_ID = t1["MPI_PERSON_ID"],
        //                          UVHP_AMOUNT = 0,
        //                          COMPUTATIONAL_YEAR = 0,
        //                          PAYEE_ACCOUNT_ID = t1["PAYEE_ACCOUNT_ID"],
        //                          RETIREMENT_DATE = t1["RETIREMENT_DATE"],
        //                          MODIFIED_DATE = t1["MODIFIED_DATE"],
        //                          REPORTSTATUS = t2["REPORTSTATUS"]
        //                      }).Distinct();

        //    DataTable ldtLateHourOpusInformation = new DataTable();
        //    ldtLateHourOpusInformation.Columns.Add("PERSON_ID", typeof(int));
        //    ldtLateHourOpusInformation.Columns.Add("MPI_PERSON_ID", typeof(string));
        //    ldtLateHourOpusInformation.Columns.Add("UVHP_AMOUNT", typeof(decimal));
        //    ldtLateHourOpusInformation.Columns.Add("COMPUTATIONAL_YEAR", typeof(int));
        //    ldtLateHourOpusInformation.Columns.Add("PAYEE_ACCOUNT_ID", typeof(int));
        //    ldtLateHourOpusInformation.Columns.Add("RETIREMENT_DATE", typeof(DateTime));
        //    ldtLateHourOpusInformation.Columns.Add("MODIFIED_DATE", typeof(DateTime));
        //    ldtLateHourOpusInformation.Columns.Add("REPORTSTATUS", typeof(string));

        //    if (collection.ToList().Count() > 0)
        //    {
        //        foreach (var item in collection)
        //        {
        //            ldtLateHourOpusInformation.Rows.Add(item.PERSON_ID, item.MPI_PERSON_ID, item.UVHP_AMOUNT, item.COMPUTATIONAL_YEAR, item.PAYEE_ACCOUNT_ID, item.RETIREMENT_DATE
        //                , item.MODIFIED_DATE, item.REPORTSTATUS);
        //        }
        //    }

        //    ldtLateHourOpusInformation.AcceptChanges();


        //    ldtPersonInformation.Rows.Cast<DataRow>().Where(r => r.Field<string>("REPORTSTATUS") == "L").ToList().ForEach(r1 => r1.Delete());
        //    ldtPersonInformation.AcceptChanges();

        //    ldtLateHourOpusInformation.Merge(ldtPersonInformation);

        //    if (ldtLateHourOpusInformation != null && ldtLateHourOpusInformation.Rows.Count > 0)
        //        ldtLateHourOpusInformation = RemoveDuplicateRows(ldtLateHourOpusInformation, "PAYEE_ACCOUNT_ID", "MPI_PERSON_ID", "REPORTSTATUS");


        //    //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
        //    ParallelOptions po = new ParallelOptions();
        //    po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;


        //    Parallel.ForEach(ldtLateHourOpusInformation.AsEnumerable(), po, (acdoPerson, loopState) =>
        //    {
        //        utlPassInfo lobjPassInfo = new utlPassInfo();
        //        lobjPassInfo.idictParams = ldictParams;
        //        lobjPassInfo.idictParams["ID"] = "BenefitAdjustmentBatch";
        //        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
        //        utlPassInfo.iobjPassInfo = lobjPassInfo;

        //        //if (Convert.ToInt32(acdoPerson[enmPerson.person_id.ToString()]) == 240)
        //        //{
        //        InitiateWorkflow(acdoPerson, lobjPassInfo, lintCount, lintTotalCount, ldtLateHoursInformation, astrLegacyDBConnection);

        //        if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
        //        {
        //            lobjPassInfo.iconFramework.Close();
        //        }

        //        lobjPassInfo.iconFramework.Dispose();
        //        lobjPassInfo.iconFramework = null;
        //        //}

        //    });



        //    #region Reemployment Benefit Adjustment (Post IAP Allocations for Reemployed Period)

        //    lintCount = 0;
        //    lintTotalCount = 0;

        //    if (ldtblIAPPayeeAccount == null || (ldtblIAPPayeeAccount != null && ldtblIAPPayeeAccount.Rows.Count <= 0))
        //        return;

        //    //ldtPersonInformation = new DataTable();
        //    // ldtPersonInformation = busBase.Select("cdoBenefitApplication.GetIAPPayeeAccountsForBenefitAdjustmentBatch", new object[0] { });

        //    var ReemployedCollection = (from t1 in ldtblIAPPayeeAccount.AsEnumerable()
        //                                join t2 in ldtLateHoursInformation.AsEnumerable()
        //                                   on Convert.ToString(t1["MPI_PERSON_ID"]).Trim() equals Convert.ToString(t2["MPID"]).Trim()
        //                                where Convert.ToString(t2["REPORTSTATUS"]) == "L" && Convert.ToString(t1["RETIREMENT_DATE"]).IsNotNullOrEmpty()
        //                                 && t2.Field<DateTime>("PROCESSEDDATE") > t1.Field<DateTime>("RETIREMENT_DATE")
        //                                   && t2.Field<DateTime>("PAYPERIODSTARTDATE") > t1.Field<DateTime>("RETIREMENT_DATE")
        //                                select
        //                                new
        //                                {
        //                                    PERSON_ID = t1["PERSON_ID"],
        //                                    MPI_PERSON_ID = t1["MPI_PERSON_ID"],
        //                                    UVHP_AMOUNT = 0,
        //                                    COMPUTATIONAL_YEAR = 0,
        //                                    PAYEE_ACCOUNT_ID = t1["PAYEE_ACCOUNT_ID"],
        //                                    RETIREMENT_DATE = t1["RETIREMENT_DATE"],
        //                                    MODIFIED_DATE = t1["MODIFIED_DATE"],
        //                                    REPORTSTATUS = t2["REPORTSTATUS"]
        //                                }).Distinct();

        //    DataTable ldtLateHourOpusReempInformation = new DataTable();
        //    ldtLateHourOpusReempInformation.Columns.Add("PERSON_ID", typeof(int));
        //    ldtLateHourOpusReempInformation.Columns.Add("MPI_PERSON_ID", typeof(string));
        //    ldtLateHourOpusReempInformation.Columns.Add("UVHP_AMOUNT", typeof(decimal));
        //    ldtLateHourOpusReempInformation.Columns.Add("COMPUTATIONAL_YEAR", typeof(int));
        //    ldtLateHourOpusReempInformation.Columns.Add("PAYEE_ACCOUNT_ID", typeof(int));
        //    ldtLateHourOpusReempInformation.Columns.Add("RETIREMENT_DATE", typeof(DateTime));
        //    ldtLateHourOpusReempInformation.Columns.Add("MODIFIED_DATE", typeof(DateTime));
        //    ldtLateHourOpusReempInformation.Columns.Add("REPORTSTATUS", typeof(string));

        //    if (ReemployedCollection.ToList().Count() > 0)
        //    {
        //        foreach (var item in ReemployedCollection)
        //        {
        //            ldtLateHourOpusReempInformation.Rows.Add(item.PERSON_ID, item.MPI_PERSON_ID, item.UVHP_AMOUNT, item.COMPUTATIONAL_YEAR, item.PAYEE_ACCOUNT_ID, item.RETIREMENT_DATE
        //                , item.MODIFIED_DATE, item.REPORTSTATUS);
        //        }
        //    }

        //    ldtLateHourOpusReempInformation.AcceptChanges();

        //    if (ldtLateHourOpusReempInformation != null && ldtLateHourOpusReempInformation.Rows.Count > 0)
        //        ldtLateHourOpusReempInformation = RemoveDuplicateRows(ldtLateHourOpusReempInformation, "PAYEE_ACCOUNT_ID", "MPI_PERSON_ID", "REPORTSTATUS");



        //    //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
        //    po = new ParallelOptions();
        //    po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;


        //    Parallel.ForEach(ldtLateHourOpusReempInformation.AsEnumerable(), po, (acdoPerson, loopState) =>
        //    {
        //        utlPassInfo lobjPassInfo = new utlPassInfo();
        //        lobjPassInfo.idictParams = ldictParams;
        //        lobjPassInfo.idictParams["ID"] = "BenefitAdjustmentBatch";
        //        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
        //        utlPassInfo.iobjPassInfo = lobjPassInfo;


        //        CalculateIAPAllocationsForReemployment(acdoPerson, lobjPassInfo, lintCount, lintTotalCount, ldtLateHoursInformation, astrLegacyDBConnection);

        //        if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
        //        {
        //            lobjPassInfo.iconFramework.Close();
        //        }

        //        lobjPassInfo.iconFramework.Dispose();
        //        lobjPassInfo.iconFramework = null;
        //        //}

        //    });

        //    lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
        //    utlPassInfo.iobjPassInfo = lobjMainPassInfo;

        //    #endregion Reemployment Benefit Adjustment (Post IAP Allocations for Reemployed Period)
        //}

        //public DataTable RemoveDuplicateRows(DataTable adtblMainTable, string DistinctColumnPayeeAccountID, string DistinctColumnMPID, string DistinctColumnReportStatus)
        //{
        //    try
        //    {
        //        // ArrayList larrUniqueRecords = new ArrayList();
        //        ArrayList larrDuplicateRecords = new ArrayList();

        //        List<Tuple<int, string, string>> lstTplUniqueRecords = new List<Tuple<int, string, string>>();

        //        // Check if records is already added to UniqueRecords otherwise,
        //        // Add the records to DuplicateRecords
        //        foreach (DataRow dRow in adtblMainTable.Rows)
        //        {
        //            Tuple<int, string, string> ltpldRow = new Tuple<int, string, string>(Convert.ToString(dRow[DistinctColumnPayeeAccountID]).IsNotNullOrEmpty() ? Convert.ToInt32(dRow[DistinctColumnPayeeAccountID]) : 0,
        //                Convert.ToString(dRow[DistinctColumnMPID]).IsNotNullOrEmpty() ? Convert.ToString(dRow[DistinctColumnMPID]) : string.Empty,
        //                Convert.ToString(dRow[DistinctColumnReportStatus]).IsNotNullOrEmpty() ? Convert.ToString(dRow[DistinctColumnReportStatus]) : string.Empty);

        //            if (lstTplUniqueRecords.Contains(ltpldRow))
        //                larrDuplicateRecords.Add(dRow);
        //            else
        //                lstTplUniqueRecords.Add(ltpldRow);
        //        }

        //        // Remove dupliate rows from DataTable added to DuplicateRecords
        //        foreach (DataRow dRow in larrDuplicateRecords)
        //        {
        //            adtblMainTable.Rows.Remove(dRow);
        //        }

        //        // Return the clean DataTable which contains unique records.
        //        return adtblMainTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        return adtblMainTable;
        //    }
        //}

        //private void InitiateWorkflow(DataRow acdoPerson, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount,
        //                                DataTable adtLateHoursInformation, string astrLegacyDBConnection)
        //{

        //    lock (iobjLock)
        //    {
        //        aintCount++;
        //        aintTotalCount++;
        //        if (aintCount == 100)
        //        {
        //            String lstrMsg = aintTotalCount + " : " + " Records Has Been Processed";
        //            PostInfoMessage(lstrMsg);
        //            aintCount = 0;
        //        }
        //    }

        //    autlPassInfo.BeginTransaction();
        //    int lintPayeeAccountId = 0;
        //    int lintPersonId = 0;
        //    string lstrParticipantMPID = string.Empty;

        //    try
        //    {

        //        if (Convert.ToString(acdoPerson[enmPerson.person_id.ToString()]).IsNotNullOrEmpty())
        //        {
        //            lintPersonId = Convert.ToInt32(acdoPerson[enmPerson.person_id.ToString()]);
        //        }

        //        if (Convert.ToString(acdoPerson["MPI_PERSON_ID"]).IsNotNullOrEmpty())
        //        {
        //            lstrParticipantMPID = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
        //        }

        //        if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty())
        //        {
        //            lintPayeeAccountId = Convert.ToInt32(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]);
        //        }

        //        if (Convert.ToString(acdoPerson["REPORTSTATUS"]).IsNotNullOrEmpty() && acdoPerson["REPORTSTATUS"].ToString() != "L" &&
        //            lintPayeeAccountId == 0)
        //        {
        //            busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.WITHDRAWAL_WORKFLOW_NAME, lintPersonId, 0, 0, null);
        //        }
        //        else if (Convert.ToString(acdoPerson["REPORTSTATUS"]).IsNotNullOrEmpty() && acdoPerson["REPORTSTATUS"].ToString() != "L" &&
        //                    lintPayeeAccountId != 0)
        //        {
        //            busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.RE_CALCULATE_BENEFIT_WORKFLOW_NAME, lintPersonId, 0, lintPayeeAccountId, null);
        //        }
        //        else
        //        {
        //            DataTable ldtLateHoursInformationForParticipant = new DataTable();

        //            if (acdoPerson["MPI_PERSON_ID"] != DBNull.Value && Convert.ToString(acdoPerson["MPI_PERSON_ID"]).IsNotNullOrEmpty()
        //                && adtLateHoursInformation.AsEnumerable().Where(o => o.Field<string>("MPID") == Convert.ToString(acdoPerson["MPI_PERSON_ID"])).Count() > 0)
        //            {
        //                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
        //                lbusPayeeAccount.FindPayeeAccount(lintPayeeAccountId);
        //                lbusPayeeAccount.LoadBenefitDetails();
        //                lbusPayeeAccount.LoadDRODetails();


        //                if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
        //                {

        //                    lbusPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
        //                    lbusPayeeAccount.ibusPayee.FindPerson(lintPersonId);
        //                    lbusPayeeAccount.ibusPayee.LoadPersonAccounts();

        //                    ldtLateHoursInformationForParticipant = adtLateHoursInformation.AsEnumerable().Where(o => o.Field<string>("MPID")
        //                        == Convert.ToString(acdoPerson["MPI_PERSON_ID"])).CopyToDataTable();

        //                    if (ibusPrevYearAllocationSummary == null)
        //                        LoadPreviousYearAllocationSummary();

        //                    DataTable ldtFullWorkHistoryForIAP = new DataTable();
        //                    SqlParameter[] parameter = new SqlParameter[3];
        //                    SqlParameter par1 = new SqlParameter("@SSN", DbType.String);
        //                    SqlParameter par2 = new SqlParameter("@FROMDATE", DbType.DateTime);
        //                    SqlParameter par3 = new SqlParameter("@COMPUTATIONYEAR", DbType.Int32);

        //                    par1.Value = Convert.ToString(ldtLateHoursInformationForParticipant.Rows[0]["ssn"]);
        //                    parameter[0] = par1;
        //                    par2.Value = iobjSystemManagement.icdoSystemManagement.batch_date.AddMonths(-1).GetFirstDayofMonth();
        //                    parameter[1] = par2;
        //                    par3.Value = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.computation_year + 1;
        //                    parameter[2] = par3;

        //                    ldtFullWorkHistoryForIAP = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkhistoryForIAPAllocation", astrLegacyDBConnection, null, parameter);
        //                    ProcessLateAllocation(ldtLateHoursInformationForParticipant, ldtFullWorkHistoryForIAP, lbusPayeeAccount);
        //                }
        //            }

        //            busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.RE_CALCULATE_BENEFIT_WORKFLOW_NAME, lintPersonId, 0, lintPayeeAccountId, null);
        //        }

        //        lock (iobjLock)
        //        {
        //            String lstrMsg = "Record Processed successfully For MPID :" + lstrParticipantMPID + " and Payee Account Id : " + lintPayeeAccountId;
        //            PostInfoMessage(lstrMsg);
        //        }
        //        autlPassInfo.Commit();

        //    }
        //    catch (Exception e)
        //    {
        //        lock (iobjLock)
        //        {
        //            ExceptionManager.Publish(e);
        //            String lstrMsg = "Error while Executing Batch,Error Message For MPID :" + lstrParticipantMPID + ":" + e.ToString();
        //            PostErrorMessage(lstrMsg);
        //        }
        //        autlPassInfo.Rollback();
        //    }

        //}

        //#region Process LATE IAP ALLOCATIONS

        //private void LoadFullWorkHistoryForIAPUptoRetirementDate(busPayeeAccount abusPayeeAccount, ref DataTable adtFullWorkHistoryForIAP)
        //{
        //    if (adtFullWorkHistoryForIAP != null && adtFullWorkHistoryForIAP.Rows.Count > 0)
        //    {
        //        if (adtFullWorkHistoryForIAP.AsEnumerable().Where(item => item.Field<Int16>("COMPUTATIONYEAR") <= abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate.Year - 1).Count() > 0)
        //        {
        //            adtFullWorkHistoryForIAP = adtFullWorkHistoryForIAP.AsEnumerable().Where(item => item.Field<Int16>("COMPUTATIONYEAR") <= abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate.Year - 1)
        //                .CopyToDataTable();

        //            #region To Set Values for IAP QTR Allocations

        //            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
        //            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

        //            SqlParameter[] parameters = new SqlParameter[3];
        //            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
        //            SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
        //            SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);
        //            DateTime ldtRetirementDate = new DateTime();

        //            ldtRetirementDate = abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate;

        //            param1.Value = abusPayeeAccount.ibusPayee.icdoPerson.istrSSNNonEncrypted;
        //            parameters[0] = param1;
        //            busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
        //            lbusIapAllocationSummary.LoadLatestAllocationSummary();
        //            param2.Value = busGlobalFunctions.GetFirstDateOfComputationYear(ldtRetirementDate.Year);
        //            parameters[1] = param2;
        //            param3.Value = busGlobalFunctions.GetLastDayOfWeek(ldtRetirementDate);                  //PROD PIR 113
        //            parameters[2] = param3;

        //            DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPHourInfoForQuarterlyAllocation", astrLegacyDBConnetion, null, parameters);
        //            if (ldtbIAPInfo.Rows.Count > 0)
        //            {
        //                DataRow ldrWorkHistoryForIAPUptoRetirementDate = adtFullWorkHistoryForIAP.NewRow();

        //                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")) > 0)
        //                    ldrWorkHistoryForIAPUptoRetirementDate["IAPHOURS"] = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")));

        //                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")) > 0)
        //                    ldrWorkHistoryForIAPUptoRetirementDate["IAPHOURSA2"] = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")));

        //                DataTable ldtIAPFiltered;
        //                decimal ldecIAPPercent = 0M;
        //                busIAPAllocationHelper aobjIAPAllocationHelper = new busIAPAllocationHelper();
        //                foreach (DataRow ldrIAPPercent in ldtbIAPInfo.Rows)
        //                {
        //                    if (ldrIAPPercent["IAPPercent"] != DBNull.Value && Convert.ToString(ldrIAPPercent["IAPPercent"]).IsNotNullOrEmpty() &&
        //                        Convert.ToDecimal(ldrIAPPercent["IAPPercent"]) > 0)
        //                    {
        //                        ldtIAPFiltered = new DataTable();
        //                        ldtIAPFiltered = ldtbIAPInfo.AsEnumerable().Where(o => o.Field<Int16?>("ComputationYear") == Convert.ToInt16(ldrIAPPercent["ComputationYear"])
        //                            && o.Field<int?>("EmpAccountNo") == Convert.ToInt32(ldrIAPPercent["EmpAccountNo"])).CopyToDataTable();

        //                        ldecIAPPercent += aobjIAPAllocationHelper.CalculateAllocation4Amount(Convert.ToInt32(ldrIAPPercent["ComputationYear"]), ldtIAPFiltered);
        //                    }

        //                }

        //                if (ldtbIAPInfo.Rows[0]["EmpAccountNo"].IsNotNull() && Convert.ToString(ldtbIAPInfo.Rows[0]["EmpAccountNo"]).IsNotNullOrEmpty())
        //                    ldrWorkHistoryForIAPUptoRetirementDate["EmpAccountNo"] = ldtbIAPInfo.Rows[0]["EmpAccountNo"];

        //                ldrWorkHistoryForIAPUptoRetirementDate["IAPPERCENT"] = ldecIAPPercent;
        //                ldrWorkHistoryForIAPUptoRetirementDate["COMPUTATIONYEAR"] = ldtRetirementDate.Year;
        //                ldrWorkHistoryForIAPUptoRetirementDate["SSN"] = abusPayeeAccount.ibusPayee.icdoPerson.istrSSNNonEncrypted;

        //                adtFullWorkHistoryForIAP.Rows.Add(ldrWorkHistoryForIAPUptoRetirementDate);
        //                adtFullWorkHistoryForIAP.AcceptChanges();

        //            }
        //            #endregion
        //        }
        //    }
        //}

        //private void LoadLateHoursAndContributions(DataTable adtLateHoursInformationForParticipant, DataTable adtFullWorkHistoryForIAP, busPayeeAccount abusPayeeAccount, ref DataTable adtLateHoursAndContributions, bool ablnIsReemployed = false)
        //{

        //    adtLateHoursAndContributions.Columns.Add("computationyear", Type.GetType("System.Int32"));
        //    adtLateHoursAndContributions.Columns.Add("ssn", Type.GetType("System.String"));
        //    adtLateHoursAndContributions.Columns.Add("iaphours", Type.GetType("System.Decimal"));
        //    adtLateHoursAndContributions.Columns.Add("iaphoursa2", Type.GetType("System.Decimal"));
        //    adtLateHoursAndContributions.Columns.Add("iappercent", Type.GetType("System.Decimal"));
        //    adtLateHoursAndContributions.Columns.Add("lateiaphours", Type.GetType("System.Decimal"));
        //    adtLateHoursAndContributions.Columns.Add("lateiaphoursa2", Type.GetType("System.Decimal"));
        //    adtLateHoursAndContributions.Columns.Add("lateiappercent", Type.GetType("System.Decimal"));


        //    if (adtLateHoursInformationForParticipant.Rows.Count > 0)
        //    {
        //        if (!ablnIsReemployed)
        //            LoadFullWorkHistoryForIAPUptoRetirementDate(abusPayeeAccount, ref adtFullWorkHistoryForIAP);


        //        DataTable ldtTempFullWorkHistoryForIAP = new DataTable();
        //        foreach (DataRow ldrLHP in adtLateHoursInformationForParticipant.Rows)
        //        {

        //            DataRow[] ldrRemainingLateHoursAndContributions = adtLateHoursAndContributions.FilterTable(utlDataType.Numeric, "computationyear", Convert.ToInt32(ldrLHP["computationyear"]));
        //            if (ldrRemainingLateHoursAndContributions.Count() == 0)
        //            {
        //                DataRow ldrLateHoursAndContribution = adtLateHoursAndContributions.NewRow();
        //                ldrLateHoursAndContribution["computationyear"] = ldrLHP["computationyear"];
        //                ldrLateHoursAndContribution["ssn"] = ldrLHP["SSN"];

        //                ldrLateHoursAndContribution["lateiaphours"] = ldrLHP["iaphours"];
        //                ldrLateHoursAndContribution["lateiaphoursa2"] = ldrLHP["iaphoursa2"];
        //                ldrLateHoursAndContribution["lateiappercent"] = ldrLHP["iappercent"];

        //                DataRow[] ldrRemaining = null;
        //                if (adtFullWorkHistoryForIAP != null && adtFullWorkHistoryForIAP.Rows.Count > 0)
        //                {
        //                    ldrRemaining = adtFullWorkHistoryForIAP.FilterTable(utlDataType.Numeric, "computationyear", Convert.ToInt32(ldrLHP["computationyear"]));
        //                }

        //                ldtTempFullWorkHistoryForIAP.Clear();
        //                if (ldrRemaining != null && ldrRemaining.Count() > 0)
        //                {
        //                    ldtTempFullWorkHistoryForIAP = ldrRemaining.CopyToDataTable();
        //                    ldrLateHoursAndContribution["iaphours"] = ldtTempFullWorkHistoryForIAP.Compute("Sum(iaphours)", "");
        //                    ldrLateHoursAndContribution["iaphoursa2"] = ldtTempFullWorkHistoryForIAP.Compute("Sum(iaphoursa2)", "");
        //                    ldrLateHoursAndContribution["iappercent"] = ldtTempFullWorkHistoryForIAP.Compute("Sum(iappercent)", "");
        //                }
        //                adtLateHoursAndContributions.Rows.Add(ldrLateHoursAndContribution);
        //            }
        //            else
        //            {
        //                foreach (DataRow ldrLateHoursAndContributions in adtLateHoursAndContributions.Rows)
        //                {
        //                    if (Convert.ToInt32(ldrLateHoursAndContributions["computationyear"]) == Convert.ToInt32(ldrLHP["computationyear"]))
        //                    {
        //                        ldrLateHoursAndContributions["lateiaphours"] = Convert.ToDecimal(ldrLateHoursAndContributions["lateiaphours"]) + Convert.ToDecimal(ldrLHP["iaphours"]);
        //                        ldrLateHoursAndContributions["lateiaphoursa2"] = Convert.ToDecimal(ldrLateHoursAndContributions["lateiaphoursa2"]) + Convert.ToDecimal(ldrLHP["iaphoursa2"]);
        //                        ldrLateHoursAndContributions["lateiappercent"] = Convert.ToDecimal(ldrLateHoursAndContributions["lateiappercent"]) + Convert.ToDecimal(ldrLHP["iappercent"]);
        //                        ldrLateHoursAndContributions.AcceptChanges();
        //                    }
        //                }
        //                adtLateHoursAndContributions.AcceptChanges();
        //            }
        //        }



        //    }
        //}

        //private void LoadPreviousYearAllocationSummary()
        //{
        //    ibusPrevYearAllocationSummary = new busIapAllocationSummary();
        //    ibusPrevYearAllocationSummary.LoadLatestAllocationSummary();
        //}



        //private void ProcessLateAllocation(DataTable adtLateHoursInformationForParticipant, DataTable adtFullWorkHistoryForIAP, busPayeeAccount abusPayeeAccount)
        //{
        //    DataTable ldtLateHoursAndContributions = new DataTable();

        //    LoadLateHoursAndContributions(adtLateHoursInformationForParticipant, adtFullWorkHistoryForIAP, abusPayeeAccount, ref ldtLateHoursAndContributions);


        //    Collection<cdoIapAllocation5Recalculation> lclbIapAllocation5Recalculation = null;
        //    busIAPAllocationHelper lobjIAPAllocationHelper = new busIAPAllocationHelper();
        //    lobjIAPAllocationHelper.LoadIAPAllocationFactor();
        //    int lintPersonAccountID = 0;


        //    if (abusPayeeAccount.ibusPayee.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
        //    {
        //        lintPersonAccountID = abusPayeeAccount.ibusPayee.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
        //    }


        //    DataTable ldtIAPContributions = new DataTable();
        //    DataTable ldtIAPFiltered = new DataTable();
        //    int lintComputationYear, lintPrevComputationYear;
        //    lintComputationYear = lintPrevComputationYear = 0;
        //    decimal ldecTotalYTDHours, ldecThru79Hours;
        //    decimal ldecTotalIAPHours = 0M;
        //    ldecThru79Hours = ldecTotalYTDHours = 0.0M;
        //    decimal ldecAllocation4Amount, ldecIAPAccountBalance = 0.00M, ldecAlloc1Amount, ldecAlloc2Amount, ldecAlloc2InvstAmount, ldecAlloc2FrftAmount, ldecAlloc3Amount, ldecAlloc4InvstAmount, ldecAlloc4FrftAmount, ldecAlloc5AfflAmount,
        //        ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount;
        //    bool lblnAgeFlag = false;
        //    decimal ldecFactor = 0;
        //    busBenefitApplication lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
        //    lclbIapAllocation5Recalculation = LoadIAPAllocation5Information(lintPersonAccountID);

        //    foreach (DataRow ldrLateHours in ldtLateHoursAndContributions.Rows)
        //    {
        //        if (lintPrevComputationYear == Convert.ToInt32(ldrLateHours["computationyear"]))
        //            continue;

        //        ldecTotalYTDHours = 0.0M;
        //        lintComputationYear = 0;
        //        //lblnAgeFlag = false;
        //        ldecAlloc1Amount = ldecAlloc2Amount = ldecAlloc2InvstAmount = ldecAlloc2FrftAmount = ldecAlloc3Amount = ldecAllocation4Amount = ldecAlloc4InvstAmount = ldecAlloc4FrftAmount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = 0.00M;
        //        ldtIAPFiltered = new DataTable();

        //        lintComputationYear = Convert.ToInt32(ldrLateHours["computationyear"]);
        //        if (lintPrevComputationYear == 0)
        //        {
        //            ldtIAPContributions = new DataTable();
        //            //Method to load IAP allocations from sgt_person_account_contribution table
        //            ldtIAPContributions = LoadIAPContributions(lintPersonAccountID, lintComputationYear, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate);
        //            if (ldtIAPContributions != null && ldtIAPContributions.Rows.Count > 0 &&
        //               ldtIAPContributions.AsEnumerable().Where(item => Convert.ToInt32(item["COMPUTATIONAL_YEAR"]) <= abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate.Year).Count() > 0)
        //            {
        //                ldtIAPContributions = ldtIAPContributions.AsEnumerable().Where(item => Convert.ToInt32(item["COMPUTATIONAL_YEAR"]) <= abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate.Year).CopyToDataTable();
        //            }
        //            //Method to load the IAP account balace as of the first year for which late hours came in
        //            lock (iobjLock)
        //            {
        //                DataTable ldtblIAPAccountBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsOfYearTillRetirementDate",
        //                   new object[3] { lintPersonAccountID, lintComputationYear, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate });
        //                if (ldtblIAPAccountBalance != null && ldtblIAPAccountBalance.Rows.Count > 0 && Convert.ToString(ldtblIAPAccountBalance.Rows[0][0]).IsNotNullOrEmpty())
        //                    ldecIAPAccountBalance = Convert.ToDecimal(ldtblIAPAccountBalance.Rows[0][0]);
        //                //ldecIAPAccountBalance = Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.GetIAPBalanceAsOfYearTillRetirementDate",
        //                //    new object[3] { lintPersonAccountID, lintComputationYear, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate },
        //                //    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache));
        //            }
        //        }
        //        //Block to load person account and work history. Used for allocation 2 and 5 calculation
        //        if (lintPrevComputationYear == 0)
        //        {
        //            lblnAgeFlag = false;
        //            lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
        //            lobjBenefitApplication.icdoBenefitApplication.person_id = abusPayeeAccount.ibusPayee.icdoPerson.person_id;
        //            lobjBenefitApplication.LoadPerson();
        //            lobjBenefitApplication.ibusPerson.LoadPersonAccountsByPlanId(busConstant.IAP_PLAN_ID);
        //            lobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
        //            lblnAgeFlag = busGlobalFunctions.CalculatePersonAge(lobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, Convert.ToDateTime(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPInceptionDate))) < 55 ? true : false;


        //            //cdoDummyWorkData lcdoWorkData1979 = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == 1979).FirstOrDefault();
        //            ////IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
        //            //if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
        //            //{
        //            //    ldecThru79Hours = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= 1979).Sum(o => o.qualified_hours);
        //            //}


        //            #region IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979

        //            //Remove history for any forfieture year 1979
        //            if (lobjBenefitApplication.aclbPersonWorkHistory_IAP != null && lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
        //            {
        //                if (lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.istrForfietureFlag == busConstant.FLAG_YES).Count() > 0)
        //                {
        //                    int lintMaxForfietureYearBefore1979 = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(item => item.year <= busConstant.BenefitCalculation.YEAR_1979 && item.istrForfietureFlag == busConstant.FLAG_YES).Max(t => t.year);
        //                    lobjBenefitApplication.aclbPersonWorkHistory_IAP = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(item => item.year > lintMaxForfietureYearBefore1979).ToList().ToCollection();
        //                }
        //            }

        //            if (lobjBenefitApplication.aclbPersonWorkHistory_IAP != null && lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
        //            {
        //                decimal ldecPreviousYearPaidIAPAccountBalance = 0M;
        //                cdoDummyWorkData lcdoWorkData1979 = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).FirstOrDefault();
        //                //IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
        //                if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
        //                {
        //                    int lintPaymentYear = 0;
        //                    DataTable ldtblPaymentYear = busBase.Select("cdoPersonAccountRetirementContribution.GetMaxPaymentYearOnOrBefore1979", new object[1] { lobjBenefitApplication.icdoBenefitApplication.person_id });
        //                    if (ldtblPaymentYear != null && ldtblPaymentYear.Rows.Count > 0 && Convert.ToString(ldtblPaymentYear.Rows[0][0]).IsNotNullOrEmpty())
        //                    {
        //                        lintPaymentYear = Convert.ToInt32(ldtblPaymentYear.Rows[0][0]);
        //                    }
        //                    if (lintPaymentYear == 0)
        //                    {

        //                        ldecThru79Hours = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Sum(o => o.qualified_hours);

        //                    }
        //                    else
        //                    {
        //                        if (lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Count() > 0)
        //                        {
        //                            ldecThru79Hours = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Sum(o => o.qualified_hours);
        //                        }
        //                    }

        //                    ldecThru79Hours += ldecPreviousYearPaidIAPAccountBalance;
        //                    if (ldecThru79Hours < 0)
        //                        ldecThru79Hours = 0;
        //                }
        //            }

        //            if (lobjBenefitApplication.aclbPersonWorkHistory_IAP != null && lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
        //            {
        //                lobjBenefitApplication.aclbPersonWorkHistory_IAP = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).ToList().ToCollection();
        //            }

        //            #endregion


        //        }
        //        //Block to update the IAP account balance if the late years are not consecutive
        //        while (lintPrevComputationYear != 0 && (lintPrevComputationYear + 1) < lintComputationYear)
        //        {
        //            lintPrevComputationYear++;
        //            //to recalculate the alloc1 for intermediate years

        //            if (lintPrevComputationYear == abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate.Year)
        //            {
        //                int lintQuarter = 0;
        //                lintQuarter = busGlobalFunctions.GetPreviousQuarter(abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate);
        //                ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintPrevComputationYear, ldecIAPAccountBalance, lintQuarter, ref ldecFactor);
        //            }
        //            else
        //            {
        //                ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintPrevComputationYear, ldecIAPAccountBalance, 4, ref ldecFactor);
        //            }

        //            DataRow[] ldrIAPCont = ldtIAPContributions.FilterTable(utlDataType.Numeric, "computational_year", lintPrevComputationYear);
        //            if (ldrIAPCont.Length > 0)
        //            {
        //                PostDifferenceAmountIntoContributionForAllocation1(lintPersonAccountID, lintPrevComputationYear, Convert.ToDecimal(ldrIAPCont[0]["alloc1"]), ldecAlloc1Amount, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate);
        //                //Block to calculate allocation 5 amount
        //                if (lintPrevComputationYear >= 1996 && lintPrevComputationYear <= 2001)
        //                {
        //                    string lstrCalculateAlloc5 = busConstant.FLAG_NO;
        //                    if (lclbIapAllocation5Recalculation != null && lclbIapAllocation5Recalculation.Where(item => item.computational_year == lintPrevComputationYear).Count() > 0)
        //                    {
        //                        lstrCalculateAlloc5 = lclbIapAllocation5Recalculation.Where(item => item.computational_year == lintPrevComputationYear).FirstOrDefault().iap_allocation5_recalculate_flag;
        //                    }
        //                    else if (Convert.ToDecimal(ldrIAPCont[0]["alloc4"]) != 0.00M)
        //                    {
        //                        lstrCalculateAlloc5 = busConstant.FLAG_YES;
        //                    }

        //                    if (lstrCalculateAlloc5 == busConstant.FLAG_YES)
        //                    {
        //                        decimal ldecYTDHours = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == lintPrevComputationYear).Sum(o => o.qualified_hours);


        //                        if (lintPrevComputationYear == 1996)
        //                        {
        //                            ldecAlloc5AfflAmount = lobjIAPAllocationHelper.CalcuateAllocation5AffliatesAmount(lintPrevComputationYear, lobjBenefitApplication.aclbPersonWorkHistory_IAP, lblnAgeFlag);
        //                        }
        //                        else
        //                        {
        //                            if (lobjIAPAllocationHelper.CheckParticipantIsAffiliate(lintPrevComputationYear, lobjBenefitApplication.ibusPerson.icdoPerson.istrSSNNonEncrypted))
        //                                ldecAlloc5AfflAmount = lobjIAPAllocationHelper.CalcuateAllocation5AffliatesAmount(lintPrevComputationYear, lobjBenefitApplication.aclbPersonWorkHistory_IAP, lblnAgeFlag);
        //                            else
        //                                ldecAlloc5NonAfflAmount = lobjIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(lintPrevComputationYear, ldecYTDHours, busConstant.IAPAllocationNonAffiliatesFlag);
        //                            ldecAlloc5BothAmount = lobjIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(lintPrevComputationYear, ldecYTDHours, busConstant.IAPAllocationBothAffAndNonAffFlag);
        //                        }
        //                        PostDifferenceAmountIntoContributionForAllocation5Affl(lintPersonAccountID, lintPrevComputationYear, Convert.ToDecimal(ldrIAPCont[0]["alloc5"]),
        //                            (ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount));
        //                    }
        //                }
        //                ldecIAPAccountBalance += ldecAlloc1Amount + Convert.ToDecimal(ldrIAPCont[0]["alloc2"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc2_invt"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc2_frft"]) +
        //                    Convert.ToDecimal(ldrIAPCont[0]["alloc3"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc4"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc4_invt"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc4_frft"]) +
        //                    ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount;
        //            }
        //            else
        //            {    

        //                PostDifferenceAmountIntoContributionForAllocation1(lintPersonAccountID, lintPrevComputationYear, 0.00M, ldecAlloc1Amount, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate);
        //                ldecIAPAccountBalance += ldecAlloc1Amount + ldecAlloc5AfflAmount;
        //            }
        //            ldecAlloc1Amount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = 0.00M;
        //        }

        //        //ldecTotalYTDHours = Convert.ToDecimal(ldrLateHours["iaphoursa2"]) + Convert.ToDecimal(ldrLateHours["lateiaphoursa2"]);
        //        ldtIAPFiltered = ldtLateHoursAndContributions.AsEnumerable().Where(o => o.Field<int>("computationyear") == lintComputationYear).CopyToDataTable();
        //        foreach (DataRow ldr in ldtIAPFiltered.Rows)
        //        {
        //            ldecTotalIAPHours += (ldr["iaphours"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphours"])) + (ldr["lateiaphours"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["lateiaphours"]));
        //            ldecTotalYTDHours += (ldr["iaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphoursa2"])) + (ldr["lateiaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["lateiaphoursa2"]));
        //        }

        //        //FIX for PIR 1023
        //        //ldecAllocation4Amount = Convert.ToDecimal(ldrLateHours["iappercent"]) + Convert.ToDecimal(ldrLateHours["lateiappercent"]);
        //        // ldecAllocation4Amount = lobjIAPAllocationHelper.CalculateAllocation4Amount(lintComputationYear, ldtIAPFiltered);

        //        DataRow[] ldrIAPContribution = ldtIAPContributions.FilterTable(utlDataType.Numeric, "computational_year", lintComputationYear);
        //        //Method to calculate Allocation 1 amount
        //        if (ldrIAPContribution.Length > 0 && lintPrevComputationYear == 0)
        //            ldecAlloc1Amount = Convert.ToDecimal(ldrIAPContribution[0]["alloc1"]);
        //        else
        //        {

        //            if (lintComputationYear == abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate.Year)
        //            {
        //                int lintQuarter = 0;
        //                lintQuarter = busGlobalFunctions.GetPreviousQuarter(abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate);
        //                ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecIAPAccountBalance, lintQuarter, ref ldecFactor);
        //            }
        //            else
        //            {
        //                ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecIAPAccountBalance, 4, ref ldecFactor);
        //            }
        //        }

        //        if (ldecTotalIAPHours >= Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.QualifiedYearHours)))
        //        {
        //            //method to calculate allocation 2 amount
        //            ldecAlloc2Amount = lobjIAPAllocationHelper.CalculateAllocation2Amount(lintComputationYear, ldecThru79Hours, ldecTotalYTDHours, DateTime.MinValue,
        //                                                                    DateTime.MinValue, DateTime.MinValue);
        //            //method to calculate allocation 2 investment amount
        //            ldecAlloc2InvstAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
        //                                                                    DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationInvestmentFlag);
        //            //method to calculate allocation 2 forfeiture amount
        //            ldecAlloc2FrftAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
        //                                                                    DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationForfeitureFlag);
        //            //method to calculate allocation 3 amount
        //            ldecAlloc3Amount = lobjIAPAllocationHelper.CalculateAllocation3Amount(lintComputationYear, ldecThru79Hours, ldecTotalYTDHours);

        //            //FIX for PIR 1023
        //            ldecAllocation4Amount = lobjIAPAllocationHelper.CalculateAllocation4Amount(lintComputationYear, ldtIAPFiltered);

        //            //method to calculate allocation 4 investment amount
        //            ldecAlloc4InvstAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationInvestmentFlag);
        //            //method to calculate allocation 4 forfeiture amount
        //            ldecAlloc4FrftAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationForfeitureFlag);
        //            //Block to calculate allocation 5 amount
        //            if (lintComputationYear >= 1996 && lintComputationYear <= 2001)
        //            {
        //                string lstrCalculateAlloc5 = busConstant.FLAG_NO;
        //                if (lclbIapAllocation5Recalculation != null && lclbIapAllocation5Recalculation.Where(item => item.computational_year == lintComputationYear).Count() > 0)
        //                {
        //                    lstrCalculateAlloc5 = lclbIapAllocation5Recalculation.Where(item => item.computational_year == lintComputationYear).FirstOrDefault().iap_allocation5_recalculate_flag;
        //                }
        //                else if (ldecAllocation4Amount != 0.00M)
        //                {
        //                    lstrCalculateAlloc5 = busConstant.FLAG_YES;
        //                }

        //                if (lstrCalculateAlloc5 == busConstant.FLAG_YES)
        //                {

        //                    if (lintComputationYear == 1996)
        //                    {
        //                        ldecAlloc5AfflAmount = lobjIAPAllocationHelper.CalcuateAllocation5AffliatesAmount(lintComputationYear, lobjBenefitApplication.aclbPersonWorkHistory_IAP, lblnAgeFlag);
        //                    }
        //                    else
        //                    {
        //                        if (lobjIAPAllocationHelper.CheckParticipantIsAffiliate(lintComputationYear, lobjBenefitApplication.ibusPerson.icdoPerson.istrSSNNonEncrypted))
        //                            ldecAlloc5AfflAmount = lobjIAPAllocationHelper.CalcuateAllocation5AffliatesAmount(lintComputationYear, lobjBenefitApplication.aclbPersonWorkHistory_IAP, lblnAgeFlag);
        //                        else
        //                            ldecAlloc5NonAfflAmount = lobjIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(lintComputationYear, ldecTotalYTDHours, busConstant.IAPAllocationNonAffiliatesFlag);
        //                        ldecAlloc5BothAmount = lobjIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(lintComputationYear, ldecTotalYTDHours, busConstant.IAPAllocationBothAffAndNonAffFlag);
        //                    }
        //                }
        //            }
        //        }
        //        //Method to post the difference amount into contribution amount
        //        PostDifferenceAmountIntoContribution(lintComputationYear, lintPersonAccountID, ldrIAPContribution, ldecAlloc1Amount, ldecAlloc2Amount, ldecAlloc2InvstAmount, ldecAlloc2FrftAmount, ldecAlloc3Amount, ldecAllocation4Amount,
        //                                        ldecAlloc4InvstAmount, ldecAlloc4FrftAmount, ldecAlloc5AfflAmount, ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount);
        //        //updating IAP account balance with the latest allocation amounts
        //        ldecIAPAccountBalance = ldecIAPAccountBalance + ldecAlloc1Amount + ldecAlloc2Amount + ldecAlloc2InvstAmount + ldecAlloc2FrftAmount + ldecAlloc3Amount + ldecAllocation4Amount +
        //                                        ldecAlloc4InvstAmount + ldecAlloc4FrftAmount + ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount;
        //        lintPrevComputationYear = lintComputationYear;
        //    }
        //    //To recalculate the allocation 1 for the last person
        //    PostAllocation1And5ForRemainingYears(lintPrevComputationYear, lintPersonAccountID, ldecIAPAccountBalance, ldtIAPContributions, lobjIAPAllocationHelper, lobjBenefitApplication, lblnAgeFlag, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate, lclbIapAllocation5Recalculation);

        //}


        //private void LoadIAPPersonAccount(int aintPersonId)
        //{
        //    idtIAPPersonAccounts = new DataTable();
        //    idtIAPPersonAccounts = busBase.Select("cdoPersonAccount.GetIAPPersonAccount", new object[1] { aintPersonId });
        //}

        ///// <summary>
        ///// Method to post the difference amount into contribution amount
        ///// </summary>
        ///// <param name="aintComputationYear">Computation Year</param>
        ///// <param name="aintPersonAccountID">Person Account ID</param>
        ///// <param name="adrIAPContribution">Filtered collection of contribution table</param>
        ///// <param name="adecAlloc1Amount">New Allocation 1 amount</param>
        ///// <param name="adecAlloc2Amount">New allocation 2 amount</param>
        ///// <param name="adecAlloc2InvstAmount">New allocation 2 invst amount</param>
        ///// <param name="adecAlloc2FrftAmount">New allocation 2 forfi</param>
        ///// <param name="adecAlloc3Amount">New Allocation 3 amount</param>
        ///// <param name="adecAllocation4Amount">New allocation 4 amount</param>
        ///// <param name="adecAlloc4InvstAmount">New allocation 4 Invst</param>
        ///// <param name="adecAlloc4FrftAmount">New allocation 4 frft</param>
        ///// <param name="adecAlloc5AfflAmount">New allocation 5 affiliate amount</param>
        ///// <param name="adecAlloc5NonAfflAmount">New allocation 5 non affiliate amount</param>
        ///// <param name="adecAlloc5BothAmount">New allocation 5 both amount</param>
        //private void PostDifferenceAmountIntoContribution(int aintComputationYear, int aintPersonAccountID, DataRow[] adrIAPContribution, decimal adecAlloc1Amount, decimal adecAlloc2Amount, decimal adecAlloc2InvstAmount, decimal adecAlloc2FrftAmount,
        //    decimal adecAlloc3Amount, decimal adecAllocation4Amount, decimal adecAlloc4InvstAmount, decimal adecAlloc4FrftAmount, decimal adecAlloc5AfflAmount, decimal adecAlloc5NonAfflAmount, decimal adecAlloc5BothAmount)
        //{
        //    busPersonAccountRetirementContribution lobjRetrContribution;
        //    //block to insert the allocation 1 difference amount            
        //    lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

        //    if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc1"].IsDBNull()))
        //        adecAlloc1Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc1"]);
        //    if (adecAlloc1Amount != 0)
        //    {
        //        lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc1Amount,
        //        astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation1);
        //    }
        //    //block to insert the allocation 2 difference amount            
        //    lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

        //    if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc2"].IsDBNull()))
        //        adecAlloc2Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc2"]);

        //    if (adecAlloc2Amount != 0)
        //    {
        //        lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc2Amount,
        //            astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation2);
        //    }
        //    //block to insert the allocation 2 invst difference amount            
        //    lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

        //    if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc2_invt"].IsDBNull()))
        //        adecAlloc2InvstAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc2_invt"]);
        //    if (adecAlloc2InvstAmount != 0)
        //    {
        //        lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc2InvstAmount,
        //        astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation2, astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
        //    }
        //    //block to insert the allocation 2 frft difference amount            
        //    lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

        //    if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc2_frft"].IsDBNull()))
        //        adecAlloc2FrftAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc2_frft"]);
        //    if (adecAlloc2FrftAmount != 0)
        //    {
        //        lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc2FrftAmount,
        //        astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation2, astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
        //    }
        //    //block to insert the allocation 3 difference amount            
        //    lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

        //    if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc3"].IsDBNull()))
        //        adecAlloc3Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc3"]);
        //    if (adecAlloc3Amount != 0)
        //    {
        //        lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc3Amount,
        //        astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation3);
        //    }
        //    //block to insert the allocation 4 difference amount            
        //    lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

        //    if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc4"].IsDBNull()))
        //        adecAllocation4Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc4"]);
        //    if (adecAllocation4Amount != 0)
        //    {
        //        lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAllocation4Amount,
        //        astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation4);
        //    }
        //    //block to insert the allocation 4 invt difference amount            
        //    lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

        //    if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc4_invt"].IsDBNull()))
        //        adecAlloc4InvstAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc4_invt"]);
        //    if (adecAlloc4InvstAmount != 0)
        //    {
        //        lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc4InvstAmount,
        //        astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation4, astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
        //    }
        //    //block to insert the allocation 4 forft difference amount            
        //    lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

        //    if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc4_frft"].IsDBNull()))
        //        adecAlloc4FrftAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc4_frft"]);
        //    if (adecAlloc4FrftAmount != 0)
        //    {
        //        lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc4FrftAmount,
        //        astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation4, astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
        //    }
        //    ////block to insert the allocation 5 affl difference amount            
        //    //lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

        //    //if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc5_affl"].IsDBNull()))
        //    //    adecAlloc5AfflAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc5_affl"]);
        //    //if (adecAlloc5AfflAmount != 0)
        //    //{
        //    //    lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc5AfflAmount,
        //    //    astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation5, astrContributionSubtype: busConstant.RCContributionSubTypeAffiliates);
        //    //}
        //    ////block to insert the allocation 5 non affl difference amount            
        //    //lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

        //    //if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc5alloc4_both_nonaffl"].IsDBNull()))
        //    //    adecAlloc5NonAfflAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc5_nonaffl"]);
        //    //if (adecAlloc5NonAfflAmount != 0)
        //    //{
        //    //    lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc5NonAfflAmount,
        //    //    astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation5, astrContributionSubtype: busConstant.RCContributionSubTypeNonAffiliates);
        //    //}
        //    ////block to insert the allocation 5 affl & non affl difference amount            
        //    //lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

        //    //if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc5_both"].IsDBNull()))
        //    //    adecAlloc5BothAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc5_both"]);
        //    //if (adecAlloc5BothAmount != 0)
        //    //{
        //    //    lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc5BothAmount,
        //    //    astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation5, astrContributionSubtype: busConstant.RCContributionSubTypeBoth);
        //    //}

        //    //block to insert the allocation 5 affl & non affl difference amount            
        //    lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
        //    decimal ldecTotalAlloc5 = adecAlloc5AfflAmount + adecAlloc5NonAfflAmount + adecAlloc5BothAmount;
        //    if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc5"].IsDBNull()))
        //        ldecTotalAlloc5 -= Convert.ToDecimal(adrIAPContribution[0]["alloc5"]);
        //    if (ldecTotalAlloc5 != 0)
        //    {
        //        lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: ldecTotalAlloc5,
        //        astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation5);
        //    }
        //}

        ///// <summary>
        ///// Method to post the allocation 1 amount for remaining years
        ///// </summary>
        ///// <param name="aintPrevComputationYear">previous computation year</param>
        ///// <param name="aintPersonAccountID">person account id</param>
        ///// <param name="adecPrevYearAccountBalance">Prev year iap account balance</param>
        ///// <param name="adtIAPContributions">IAP allocations from contribution table</param>
        ///// <param name="aobjIAPHelper">bus. object containing allcation formula</param>
        //private void PostAllocation1And5ForRemainingYears(int aintPrevComputationYear, int aintPersonAccountID, decimal adecPrevYearAccountBalance, DataTable adtIAPContributions, busIAPAllocationHelper aobjIAPHelper,
        //    busBenefitApplication aobjBenefitApplication, bool ablnAgeFlag, DateTime aintRetirementDate, Collection<cdoIapAllocation5Recalculation> aclbIapAllocation5Recalculation)
        //{
        //    IEnumerable<DataRow> lenmRemainingContributions = adtIAPContributions.AsEnumerable().Where(o => o.Field<decimal>("computational_year") > Convert.ToDecimal(aintPrevComputationYear));
        //    decimal ldecAlloc1Amount, ldecAlloc5AfflAmount, ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount, ldecFactor;
        //    ldecAlloc1Amount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = ldecFactor = 0.00M;
        //    foreach (DataRow ldr in lenmRemainingContributions)
        //    {
        //        //method to calculate allocation 1 amount
        //        if (Convert.ToInt32(ldr["computational_year"]) == aintRetirementDate.Year)
        //        {
        //            int lintQuarter = 0;
        //            lintQuarter = busGlobalFunctions.GetPreviousQuarter(aintRetirementDate);
        //            ldecAlloc1Amount = aobjIAPHelper.CalculateAllocation1Amount(Convert.ToInt32(ldr["computational_year"]), adecPrevYearAccountBalance, lintQuarter, ref ldecFactor);
        //        }
        //        else
        //        {
        //            ldecAlloc1Amount = aobjIAPHelper.CalculateAllocation1Amount(Convert.ToInt32(ldr["computational_year"]), adecPrevYearAccountBalance, 4, ref ldecFactor);
        //        }

        //        //Block to calculate allocation 5 amount
        //        if (Convert.ToInt32(ldr["computational_year"]) >= 1996 && Convert.ToInt32(ldr["computational_year"]) <= 2001)
        //        {
        //            string lstrCalculateAlloc5 = busConstant.FLAG_NO;
        //            if (aclbIapAllocation5Recalculation != null && aclbIapAllocation5Recalculation.Where(item => item.computational_year == Convert.ToInt32(ldr["computational_year"])).Count() > 0)
        //            {
        //                lstrCalculateAlloc5 = aclbIapAllocation5Recalculation.Where(item => item.computational_year == Convert.ToInt32(ldr["computational_year"])).FirstOrDefault().iap_allocation5_recalculate_flag;
        //            }
        //            else if (Convert.ToDecimal(ldr["alloc4"]) != 0.00M)
        //            {
        //                lstrCalculateAlloc5 = busConstant.FLAG_YES;
        //            }

        //            if (lstrCalculateAlloc5 == busConstant.FLAG_YES)
        //            {
        //                decimal ldecYTDHours = aobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == Convert.ToInt32(ldr["computational_year"])).Sum(o => o.qualified_hours);

        //                if (Convert.ToInt32(ldr["computational_year"]) == 1996)
        //                {
        //                    ldecAlloc5AfflAmount = aobjIAPHelper.CalcuateAllocation5AffliatesAmount(Convert.ToInt32(ldr["computational_year"]), aobjBenefitApplication.aclbPersonWorkHistory_IAP, ablnAgeFlag);
        //                }
        //                else
        //                {
        //                    if (aobjIAPHelper.CheckParticipantIsAffiliate(Convert.ToInt32(ldr["computational_year"]), aobjBenefitApplication.ibusPerson.icdoPerson.istrSSNNonEncrypted))
        //                        ldecAlloc5AfflAmount = aobjIAPHelper.CalcuateAllocation5AffliatesAmount(Convert.ToInt32(ldr["computational_year"]), aobjBenefitApplication.aclbPersonWorkHistory_IAP, ablnAgeFlag);
        //                    else
        //                        ldecAlloc5NonAfflAmount = aobjIAPHelper.CalcuateAllocation5NonAffOrBothAmount(Convert.ToInt32(ldr["computational_year"]), ldecYTDHours, busConstant.IAPAllocationNonAffiliatesFlag);
        //                    ldecAlloc5BothAmount = aobjIAPHelper.CalcuateAllocation5NonAffOrBothAmount(Convert.ToInt32(ldr["computational_year"]), ldecYTDHours, busConstant.IAPAllocationBothAffAndNonAffFlag);
        //                }
        //                PostDifferenceAmountIntoContributionForAllocation5Affl(aintPersonAccountID, Convert.ToInt32(ldr["computational_year"]), Convert.ToDecimal(ldr["alloc5"]),
        //                    (ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount));
        //            }
        //        }
        //        //method to post the difference amount into contribution table
        //        PostDifferenceAmountIntoContributionForAllocation1(aintPersonAccountID, Convert.ToInt32(ldr["computational_year"]),
        //            ldr["alloc1"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc1"]) : 0.0M, ldecAlloc1Amount, aintRetirementDate);
        //        //updating iap account balance
        //        adecPrevYearAccountBalance += (ldecAlloc1Amount + (ldr["alloc2"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2"]) : 0.0M) +
        //            (ldr["alloc2_invt"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2_invt"]) : 0.0M) + (ldr["alloc2_frft"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2_frft"]) : 0.0M) +
        //                (ldr["alloc3"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc3"]) : 0.0M) + (ldr["alloc4"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4"]) : 0.0M) +
        //            (ldr["alloc4_invt"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4_invt"]) : 0.0M) +
        //                (ldr["alloc4_frft"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4_frft"]) : 0.0M) + ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount);
        //        ldecAlloc1Amount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = 0.00M;
        //    }
        //}

        ///// <summary>
        ///// Method to post the difference amount into contribution table for allocation 1
        ///// </summary>
        ///// <param name="aintPersonAccountID">person account id</param>
        ///// <param name="aintComputationYear">computation year</param>
        ///// <param name="adecOldAlloc1Amount">old allcation 1 amount</param>
        ///// <param name="adecNewAlloc1Amount">new allocation 1 amount</param>
        //private void PostDifferenceAmountIntoContributionForAllocation1(int aintPersonAccountID, int aintComputationYear, decimal adecOldAlloc1Amount, decimal adecNewAlloc1Amount, DateTime adtRetirementDate)
        //{
        //    busPersonAccountRetirementContribution lobjRetrContribution = new busPersonAccountRetirementContribution();
        //    if ((adecNewAlloc1Amount - adecOldAlloc1Amount) != 0)
        //    {
        //        //method to post the entires into contribution table
        //        if (adtRetirementDate.Year == aintComputationYear)
        //        {
        //            lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfPreviousQuarter(adtRetirementDate), DateTime.Now, aintComputationYear,
        //          adecIAPBalanceAmount: (adecNewAlloc1Amount - adecOldAlloc1Amount), astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation1);
        //        }
        //        else
        //        {
        //            lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear,
        //            adecIAPBalanceAmount: (adecNewAlloc1Amount - adecOldAlloc1Amount), astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation1);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Method to post the difference amount into contribution table for allocation 5 affliates
        ///// </summary>
        ///// <param name="aintPersonAccountID">Person Account ID</param>
        ///// <param name="aintComputationYear">Computation year</param>
        ///// <param name="adecOldAlloc5AfflAmount">Old Alloc 5 Affl amount</param>
        ///// <param name="adecNewAlloc5AfflAmount">New Alloc 5 Affl amount</param>
        //private void PostDifferenceAmountIntoContributionForAllocation5Affl(int aintPersonAccountID, int aintComputationYear, decimal adecOldAlloc5Amount, decimal adecNewAlloc5Amount)
        //{
        //    busPersonAccountRetirementContribution lobjRetrContribution = new busPersonAccountRetirementContribution();
        //    if ((adecNewAlloc5Amount - adecOldAlloc5Amount) != 0)
        //    {
        //        //method to post the entires into contribution table
        //        lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear,
        //        adecIAPBalanceAmount: (adecNewAlloc5Amount - adecOldAlloc5Amount), astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation5);
        //    }
        //}


        //private DataTable LoadIAPContributions(int aintPersonAccountID, int aintComputationYear, DateTime adtRetirementDate)
        //{
        //    return busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationsForPersonAccountTillRetirementDate", new object[3] { aintPersonAccountID, aintComputationYear, adtRetirementDate });
        //}

        //private DataTable LoadIAPContributionsAfterRetirement(int aintPersonAccountID, int aintComputationYear, DateTime adtRetirementDate)
        //{
        //    return busBase.Select("cdoPersonAccountRetirementContribution.GetPaidIAPAllocationsDetailsAfterRetirement", new object[3] { aintPersonAccountID, aintComputationYear, adtRetirementDate });
        //}

        //public Collection<cdoIapAllocation5Recalculation> LoadIAPAllocation5Information(int aintPersonAccountId)
        //{
        //    Collection<cdoIapAllocation5Recalculation> lclbIapAllocation5Recalculation = new Collection<cdoIapAllocation5Recalculation>();

        //    DataTable ldtblIapAllocation5Recalculation = busBase.Select("cdoIapAllocation5Recalculation.GetAllocation5Information", new object[1] { aintPersonAccountId });
        //    if (ldtblIapAllocation5Recalculation.Rows.Count > 0)
        //    {
        //        lclbIapAllocation5Recalculation = cdoIapAllocation5Recalculation.GetCollection<cdoIapAllocation5Recalculation>(ldtblIapAllocation5Recalculation);
        //    }

        //    return lclbIapAllocation5Recalculation;

        //}

        //#endregion Process LATE IAP ALLOCATIONS
        #endregion

        #region Re-employment - Old Code

        //private void CalculateIAPAllocationsForReemployment(DataRow acdoPerson, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount,
        //                            DataTable adtLateHoursInformation, string astrLegacyDBConnection)
        //{

        //    lock (iobjLock)
        //    {
        //        aintCount++;
        //        aintTotalCount++;
        //        if (aintCount == 100)
        //        {
        //            String lstrMsg = aintTotalCount + " : " + " Records Has Been Processed";
        //            PostInfoMessage(lstrMsg);
        //            aintCount = 0;
        //        }
        //    }

        //    autlPassInfo.BeginTransaction();
        //    int lintPayeeAccountId = 0;
        //    int lintPersonId = 0;
        //    string lstrParticipantMPID = string.Empty;

        //    try
        //    {

        //        if (Convert.ToString(acdoPerson[enmPerson.person_id.ToString()]).IsNotNullOrEmpty())
        //        {
        //            lintPersonId = Convert.ToInt32(acdoPerson[enmPerson.person_id.ToString()]);
        //        }

        //        if (Convert.ToString(acdoPerson["MPI_PERSON_ID"]).IsNotNullOrEmpty())
        //        {
        //            lstrParticipantMPID = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
        //        }

        //        if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty())
        //        {
        //            lintPayeeAccountId = Convert.ToInt32(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]);
        //        }
        //        DataTable ldtLateHoursInformationForParticipant = new DataTable();

        //        if (acdoPerson["MPI_PERSON_ID"] != DBNull.Value && Convert.ToString(acdoPerson["MPI_PERSON_ID"]).IsNotNullOrEmpty()
        //            && adtLateHoursInformation.AsEnumerable().Where(o => o.Field<string>("MPID") == Convert.ToString(acdoPerson["MPI_PERSON_ID"])).Count() > 0)
        //        {
        //            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
        //            lbusPayeeAccount.FindPayeeAccount(lintPayeeAccountId);
        //            lbusPayeeAccount.LoadBenefitDetails();
        //            lbusPayeeAccount.LoadDRODetails();


        //            if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
        //            {

        //                lbusPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
        //                lbusPayeeAccount.ibusPayee.FindPerson(lintPersonId);
        //                lbusPayeeAccount.ibusPayee.LoadPersonAccounts();

        //                ldtLateHoursInformationForParticipant = adtLateHoursInformation.AsEnumerable().Where(o => o.Field<string>("MPID")
        //                    == Convert.ToString(acdoPerson["MPI_PERSON_ID"])).CopyToDataTable();

        //                DataTable ldtFullWorkHistoryForIAPReEmp = new DataTable();
        //                LoadFullWorkHistoryForIAPAfterRetirementDate(lbusPayeeAccount, ref ldtFullWorkHistoryForIAPReEmp);

        //                if (ibusPrevYearAllocationSummary == null)
        //                    LoadPreviousYearAllocationSummary();

        //                DataTable ldtFullWorkHistoryForIAP = new DataTable();
        //                SqlParameter[] parameter = new SqlParameter[4];
        //                SqlParameter par1 = new SqlParameter("@SSN", DbType.String);
        //                SqlParameter par2 = new SqlParameter("@FROMDATE", DbType.DateTime);
        //                SqlParameter par3 = new SqlParameter("@COMPUTATIONYEAR", DbType.Int32);
        //                SqlParameter par4 = new SqlParameter("@RETIREMENTDATE", DbType.DateTime);

        //                par1.Value = Convert.ToString(ldtLateHoursInformationForParticipant.Rows[0]["ssn"]);
        //                parameter[0] = par1;
        //                par2.Value = iobjSystemManagement.icdoSystemManagement.batch_date.AddMonths(-1).GetFirstDayofMonth();
        //                parameter[1] = par2;
        //                par3.Value = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.computation_year + 1;
        //                parameter[2] = par3;
        //                par4.Value = busGlobalFunctions.GetLastDayOfWeek(lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate);
        //                parameter[3] = par4;

        //                ldtFullWorkHistoryForIAP = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkhistoryForIAPAllocation", astrLegacyDBConnection, null, parameter);

        //                ProcessLateAllocationForReemployment(ldtLateHoursInformationForParticipant, ldtFullWorkHistoryForIAP, lbusPayeeAccount, ldtFullWorkHistoryForIAPReEmp);

        //                lock (iobjLock)
        //                {
        //                    String lstrMsg = "Reemployed participant gets processed successfully For MPID :" + lstrParticipantMPID + " and Payee Account Id : " + lintPayeeAccountId;
        //                    PostInfoMessage(lstrMsg);
        //                }

        //                //reemployment section
        //                busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.RE_CALCULATE_BENEFIT_WORKFLOW_NAME, lintPersonId, 0, lintPayeeAccountId, null);


        //            }
        //        }


        //        autlPassInfo.Commit();

        //    }
        //    catch (Exception e)
        //    {
        //        lock (iobjLock)
        //        {
        //            ExceptionManager.Publish(e);
        //            String lstrMsg = "Error while Executing Batch,Error Message For MPID :" + lstrParticipantMPID + ":" + e.ToString();
        //            PostErrorMessage(lstrMsg);
        //        }
        //        autlPassInfo.Rollback();
        //    }

        //}

        //private void ProcessLateAllocationForReemployment(DataTable adtLateHoursInformationForParticipant, DataTable adtFullWorkHistoryForIAP, busPayeeAccount abusPayeeAccount, DataTable ldtFullWorkHistoryForIAPReEmp)
        //{
        //    DataTable ldtLateHoursAndContributions = new DataTable();

        //    LoadLateHoursAndContributions(adtLateHoursInformationForParticipant, adtFullWorkHistoryForIAP, abusPayeeAccount, ref ldtLateHoursAndContributions, ablnIsReemployed: true);


        //    Collection<cdoIapAllocation5Recalculation> lclbIapAllocation5Recalculation = null;
        //    busIAPAllocationHelper lobjIAPAllocationHelper = new busIAPAllocationHelper();
        //    lobjIAPAllocationHelper.LoadIAPAllocationFactor();
        //    int lintPersonAccountID = 0;


        //    if (abusPayeeAccount.ibusPayee.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
        //    {
        //        lintPersonAccountID = abusPayeeAccount.ibusPayee.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
        //    }


        //    DataTable ldtIAPContributions = new DataTable();
        //    DataTable ldtIAPFiltered = new DataTable();
        //    int lintComputationYear, lintPrevComputationYear;
        //    lintComputationYear = lintPrevComputationYear = 0;
        //    decimal ldecTotalYTDHours, ldecThru79Hours;
        //    decimal ldecTotalIAPHours = 0M;
        //    ldecThru79Hours = ldecTotalYTDHours = 0.0M;
        //    decimal ldecAllocation4Amount, ldecIAPAccountBalance = 0.00M, ldecAlloc1Amount, ldecAlloc2Amount, ldecAlloc2InvstAmount, ldecAlloc2FrftAmount, ldecAlloc3Amount, ldecAlloc4InvstAmount, ldecAlloc4FrftAmount, ldecAlloc5AfflAmount,
        //        ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount;
        //    bool lblnAgeFlag = false;
        //    decimal ldecFactor = 0;
        //    busBenefitApplication lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
        //    lclbIapAllocation5Recalculation = LoadIAPAllocation5Information(lintPersonAccountID);

        //    foreach (DataRow ldrLateHours in ldtLateHoursAndContributions.Rows)
        //    {
        //        if (lintPrevComputationYear == Convert.ToInt32(ldrLateHours["computationyear"]))
        //            continue;

        //        ldecTotalYTDHours = 0.0M;
        //        lintComputationYear = 0;
        //        //lblnAgeFlag = false;
        //        ldecAlloc1Amount = ldecAlloc2Amount = ldecAlloc2InvstAmount = ldecAlloc2FrftAmount = ldecAlloc3Amount = ldecAllocation4Amount = ldecAlloc4InvstAmount = ldecAlloc4FrftAmount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = 0.00M;
        //        ldtIAPFiltered = new DataTable();

        //        lintComputationYear = Convert.ToInt32(ldrLateHours["computationyear"]);

        //        if (lintPrevComputationYear == 0)
        //        {
        //            ldtIAPContributions = new DataTable();
        //            //Method to load IAP allocations from sgt_person_account_contribution table
        //            ldtIAPContributions = LoadIAPContributionsAfterRetirement(lintPersonAccountID, lintComputationYear, busGlobalFunctions.GetLastDayOfWeek(abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate));
        //            //Method to load the IAP account balace as of the first year for which late hours came in

        //            DataTable ldtblIAPAccountBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsOfYearAfterRetirementDate",
        //                 new object[3] { lintPersonAccountID, lintComputationYear, busGlobalFunctions.GetLastDayOfWeek(abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate) });
        //            if (ldtblIAPAccountBalance != null && ldtblIAPAccountBalance.Rows.Count > 0 && Convert.ToString(ldtblIAPAccountBalance.Rows[0][0]).IsNotNullOrEmpty())
        //                ldecIAPAccountBalance = Convert.ToDecimal(ldtblIAPAccountBalance.Rows[0][0]);

        //        }
        //        //Block to load person account and work history. Used for allocation 2 and 5 calculation
        //        if (lintPrevComputationYear == 0)
        //        {
        //            lblnAgeFlag = false;
        //            lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
        //            lobjBenefitApplication.icdoBenefitApplication.person_id = abusPayeeAccount.ibusPayee.icdoPerson.person_id;
        //            lobjBenefitApplication.LoadPerson();
        //            lobjBenefitApplication.ibusPerson.LoadPersonAccountsByPlanId(busConstant.IAP_PLAN_ID);
        //            lobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
        //            lblnAgeFlag = busGlobalFunctions.CalculatePersonAge(lobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, Convert.ToDateTime(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPInceptionDate))) < 55 ? true : false;

        //            //cdoDummyWorkData lcdoWorkData1979 = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == 1979).FirstOrDefault();
        //            ////IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
        //            //if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
        //            //{
        //            //    ldecThru79Hours = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= 1979).Sum(o => o.qualified_hours);
        //            //}


        //            #region IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979

        //            //Remove history for any forfieture year 1979
        //            if (lobjBenefitApplication.aclbPersonWorkHistory_IAP != null && lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
        //            {
        //                if (lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.istrForfietureFlag == busConstant.FLAG_YES).Count() > 0)
        //                {
        //                    int lintMaxForfietureYearBefore1979 = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(item => item.year <= busConstant.BenefitCalculation.YEAR_1979 && item.istrForfietureFlag == busConstant.FLAG_YES).Max(t => t.year);
        //                    lobjBenefitApplication.aclbPersonWorkHistory_IAP = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(item => item.year > lintMaxForfietureYearBefore1979).ToList().ToCollection();
        //                }
        //            }

        //            if (lobjBenefitApplication.aclbPersonWorkHistory_IAP != null && lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
        //            {
        //                decimal ldecPreviousYearPaidIAPAccountBalance = 0M;
        //                cdoDummyWorkData lcdoWorkData1979 = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).FirstOrDefault();
        //                //IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
        //                if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
        //                {
        //                    int lintPaymentYear = 0;
        //                    DataTable ldtblPaymentYear = busBase.Select("cdoPersonAccountRetirementContribution.GetMaxPaymentYearOnOrBefore1979", new object[1] { lobjBenefitApplication.icdoBenefitApplication.person_id });
        //                    if (ldtblPaymentYear != null && ldtblPaymentYear.Rows.Count > 0 && Convert.ToString(ldtblPaymentYear.Rows[0][0]).IsNotNullOrEmpty())
        //                    {
        //                        lintPaymentYear = Convert.ToInt32(ldtblPaymentYear.Rows[0][0]);
        //                    }
        //                    if (lintPaymentYear == 0)
        //                    {

        //                        ldecThru79Hours = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Sum(o => o.qualified_hours);

        //                    }
        //                    else
        //                    {
        //                        if (lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Count() > 0)
        //                        {
        //                            ldecThru79Hours = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Sum(o => o.qualified_hours);
        //                        }
        //                    }

        //                    ldecThru79Hours += ldecPreviousYearPaidIAPAccountBalance;
        //                    if (ldecThru79Hours < 0)
        //                        ldecThru79Hours = 0;
        //                }
        //            }

        //            if (lobjBenefitApplication.aclbPersonWorkHistory_IAP != null && lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
        //            {
        //                lobjBenefitApplication.aclbPersonWorkHistory_IAP = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).ToList().ToCollection();
        //            }

        //            #endregion
        //        }
        //        //Block to update the IAP account balance if the late years are not consecutive
        //        while (lintPrevComputationYear != 0 && (lintPrevComputationYear + 1) < lintComputationYear)
        //        {
        //            lintPrevComputationYear++;
        //            //to recalculate the alloc1 for intermediate years

        //            if (!CheckIfAgeAtReemploymentIsGreaterThan65(ldtFullWorkHistoryForIAPReEmp, lobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, lintPrevComputationYear))
        //            {
        //                ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintPrevComputationYear, ldecIAPAccountBalance, 4, ref ldecFactor);
        //            }

        //            DataRow[] ldrIAPCont = ldtIAPContributions.FilterTable(utlDataType.Numeric, "computational_year", lintPrevComputationYear);
        //            if (ldrIAPCont.Length > 0)
        //            {
        //                PostDifferenceAmountIntoContributionForAllocation1(lintPersonAccountID, lintPrevComputationYear, Convert.ToDecimal(ldrIAPCont[0]["alloc1"]), ldecAlloc1Amount, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate);
        //                //Block to calculate allocation 5 amount

        //                ldecIAPAccountBalance += ldecAlloc1Amount + Convert.ToDecimal(ldrIAPCont[0]["alloc2"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc2_invt"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc2_frft"]) +
        //                    Convert.ToDecimal(ldrIAPCont[0]["alloc4"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc4_invt"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc4_frft"]);
        //            }
        //            else
        //            {
        //                PostDifferenceAmountIntoContributionForAllocation1(lintPersonAccountID, lintPrevComputationYear, 0.00M, ldecAlloc1Amount, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate);
        //                ldecIAPAccountBalance += ldecAlloc1Amount + ldecAlloc5AfflAmount;
        //            }
        //            ldecAlloc1Amount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = 0.00M;
        //        }

        //        //ldecTotalYTDHours = Convert.ToDecimal(ldrLateHours["iaphoursa2"]) + Convert.ToDecimal(ldrLateHours["lateiaphoursa2"]);
        //        ldtIAPFiltered = ldtLateHoursAndContributions.AsEnumerable().Where(o => o.Field<int>("computationyear") == lintComputationYear).CopyToDataTable();
        //        foreach (DataRow ldr in ldtIAPFiltered.Rows)
        //        {
        //            ldecTotalIAPHours += (ldr["iaphours"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphours"])) + (ldr["lateiaphours"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["lateiaphours"]));
        //            ldecTotalYTDHours += (ldr["iaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphoursa2"])) + (ldr["lateiaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["lateiaphoursa2"]));
        //        }

        //        //FIX for PIR 1023
        //        //ldecAllocation4Amount = Convert.ToDecimal(ldrLateHours["iappercent"]) + Convert.ToDecimal(ldrLateHours["lateiappercent"]);
        //        // ldecAllocation4Amount = lobjIAPAllocationHelper.CalculateAllocation4Amount(lintComputationYear, ldtIAPFiltered);

        //        DataRow[] ldrIAPContribution = ldtIAPContributions.FilterTable(utlDataType.Numeric, "computational_year", lintComputationYear);
        //        //Method to calculate Allocation 1 amount

        //        if (!CheckIfAgeAtReemploymentIsGreaterThan65(ldtFullWorkHistoryForIAPReEmp, lobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, lintComputationYear))
        //        {
        //            if (ldrIAPContribution.Length > 0 && lintPrevComputationYear == 0)
        //                ldecAlloc1Amount = Convert.ToDecimal(ldrIAPContribution[0]["alloc1"]);
        //            else
        //            {
        //                ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecIAPAccountBalance, 4, ref ldecFactor);
        //            }
        //        }

        //        if (ldecTotalIAPHours >= 870M)
        //        {
        //            //method to calculate allocation 2 amount
        //            ldecAlloc2Amount = lobjIAPAllocationHelper.CalculateAllocation2Amount(lintComputationYear, ldecThru79Hours, ldecTotalYTDHours, DateTime.MinValue,
        //                                                                    DateTime.MinValue, DateTime.MinValue);
        //            //method to calculate allocation 2 investment amount
        //            ldecAlloc2InvstAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
        //                                                                    DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationInvestmentFlag);
        //            //method to calculate allocation 2 forfeiture amount
        //            ldecAlloc2FrftAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
        //                                                                    DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationForfeitureFlag);

        //            //FIX for PIR 1023
        //            ldecAllocation4Amount = lobjIAPAllocationHelper.CalculateAllocation4Amount(lintComputationYear, ldtIAPFiltered);

        //            //method to calculate allocation 4 investment amount
        //            ldecAlloc4InvstAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationInvestmentFlag);
        //            //method to calculate allocation 4 forfeiture amount
        //            ldecAlloc4FrftAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationForfeitureFlag);
        //        }
        //        //Method to post the difference amount into contribution amount
        //        PostDifferenceAmountIntoContribution(lintComputationYear, lintPersonAccountID, ldrIAPContribution, ldecAlloc1Amount, ldecAlloc2Amount, ldecAlloc2InvstAmount, ldecAlloc2FrftAmount, ldecAlloc3Amount, ldecAllocation4Amount,
        //                                        ldecAlloc4InvstAmount, ldecAlloc4FrftAmount, ldecAlloc5AfflAmount, ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount);
        //        //updating IAP account balance with the latest allocation amounts
        //        ldecIAPAccountBalance = ldecIAPAccountBalance + ldecAlloc1Amount + ldecAlloc2Amount + ldecAlloc2InvstAmount + ldecAlloc2FrftAmount + ldecAlloc3Amount + ldecAllocation4Amount +
        //                                        ldecAlloc4InvstAmount + ldecAlloc4FrftAmount + ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount;
        //        lintPrevComputationYear = lintComputationYear;
        //    }
        //    //To recalculate the allocation 1 for the last person
        //    PostAllocation1ForRemainingYearsForReemployment(lintPrevComputationYear, lintPersonAccountID, ldecIAPAccountBalance, ldtIAPContributions, lobjIAPAllocationHelper, lobjBenefitApplication, lblnAgeFlag, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate, ldtFullWorkHistoryForIAPReEmp);

        //}

        //private void PostAllocation1ForRemainingYearsForReemployment(int aintPrevComputationYear, int aintPersonAccountID, decimal adecPrevYearAccountBalance, DataTable adtIAPContributions, busIAPAllocationHelper aobjIAPHelper,
        //busBenefitApplication aobjBenefitApplication, bool ablnAgeFlag, DateTime aintRetirementDate, DataTable ldtFullWorkHistoryForIAPReEmp)
        //{
        //    IEnumerable<DataRow> lenmRemainingContributions = adtIAPContributions.AsEnumerable().Where(o => o.Field<decimal>("computational_year") > Convert.ToDecimal(aintPrevComputationYear));
        //    decimal ldecAlloc1Amount, ldecAlloc5AfflAmount, ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount, ldecFactor;
        //    ldecAlloc1Amount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = ldecFactor = 0.00M;
        //    foreach (DataRow ldr in lenmRemainingContributions)
        //    {
        //        //method to calculate allocation 1 amount

        //        if (!CheckIfAgeAtReemploymentIsGreaterThan65(ldtFullWorkHistoryForIAPReEmp, aobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, Convert.ToInt32(ldr["computational_year"])))
        //        {
        //            ldecAlloc1Amount = aobjIAPHelper.CalculateAllocation1Amount(Convert.ToInt32(ldr["computational_year"]), adecPrevYearAccountBalance, 4, ref ldecFactor);
        //        }

        //        //method to post the difference amount into contribution table
        //        PostDifferenceAmountIntoContributionForAllocation1(aintPersonAccountID, Convert.ToInt32(ldr["computational_year"]),
        //            ldr["alloc1"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc1"]) : 0.0M, ldecAlloc1Amount, aintRetirementDate);
        //        //updating iap account balance
        //        adecPrevYearAccountBalance += (ldecAlloc1Amount + (ldr["alloc2"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2"]) : 0.0M) +
        //            (ldr["alloc2_invt"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2_invt"]) : 0.0M) + (ldr["alloc2_frft"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2_frft"]) : 0.0M) +
        //                (ldr["alloc3"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc3"]) : 0.0M) + (ldr["alloc4"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4"]) : 0.0M) +
        //            (ldr["alloc4_invt"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4_invt"]) : 0.0M) +
        //                (ldr["alloc4_frft"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4_frft"]) : 0.0M) + ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount);
        //        ldecAlloc1Amount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = 0.00M;
        //    }
        //}

        //private void LoadFullWorkHistoryForIAPAfterRetirementDate(busPayeeAccount abusPayeeAccount, ref DataTable adtFullWorkHistoryForIAP)
        //{

        //    adtFullWorkHistoryForIAP = new DataTable();
        //    //utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
        //    //string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

        //    //SqlParameter[] lsqlParameters = new SqlParameter[7];
        //    //SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
        //    //SqlParameter param2 = new SqlParameter("@FROM_DATE", DbType.DateTime);
        //    //SqlParameter param3 = new SqlParameter("@TO_DATE", DbType.DateTime);
        //    //SqlParameter param4 = new SqlParameter("@PLANCODE", DbType.String);
        //    //SqlParameter param5 = new SqlParameter("@PROCESSED_FROM_DATE", DbType.DateTime);
        //    //SqlParameter param6 = new SqlParameter("@PROCESSED_TO_DATE", DbType.DateTime);

        //    //SqlParameter returnvalue = new SqlParameter("@RETURN_VALUE", DbType.Int32);
        //    //returnvalue.Direction = ParameterDirection.ReturnValue;

        //    //param1.Value = abusPayeeAccount.ibusPayee.icdoPerson.istrSSNNonEncrypted;
        //    //lsqlParameters[0] = param1;

        //    //param2.Value = busGlobalFunctions.GetLastDayOfWeek(abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate).AddDays(1);
        //    //lsqlParameters[1] = param2;

        //    //param3.Value = DateTime.Now;
        //    //lsqlParameters[2] = param3;

        //    //param4.Value = "IAP";
        //    //lsqlParameters[3] = param4;

        //    //param5.Value = DBNull.Value;
        //    //lsqlParameters[4] = param5;

        //    //param6.Value = DBNull.Value;
        //    //lsqlParameters[5] = param6;

        //    //lsqlParameters[6] = returnvalue;
        //    ////lsqlParameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

        //    //DataTable ldtReemployedWorkHistory = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataBetweenTwoDates", lstrLegacyDBConnetion, lsqlParameters);
        //    utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
        //    string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

        //    SqlParameter[] lsqlParameters = new SqlParameter[2];
        //    SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
        //    SqlParameter param2 = new SqlParameter("@RETIREMENT_DATE", DbType.DateTime);

        //    param1.Value = abusPayeeAccount.ibusPayee.icdoPerson.istrSSNNonEncrypted; ;
        //    lsqlParameters[0] = param1;

        //    param2.Value = busGlobalFunctions.GetLastDayOfWeek(abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate);
        //    lsqlParameters[1] = param2;


        //    adtFullWorkHistoryForIAP = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataAfterRetirement", lstrLegacyDBConnetion, null, lsqlParameters);

        //    //if (ldtReemployedWorkHistory != null && ldtReemployedWorkHistory.Rows.Count > 0)
        //    //{
        //    //    DateTime ldtDate = busGlobalFunctions.GetLastDayOfWeek(abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate).AddDays(1);
        //    //    if (adtFullWorkHistoryForIAP != null && adtFullWorkHistoryForIAP.Rows.Count > 0 &&
        //    //      adtFullWorkHistoryForIAP.AsEnumerable().Where(item => item.Field<DateTime>("FromDate") >= ldtDate).Count() > 0)
        //    //    {
        //    //        adtFullWorkHistoryForIAP = adtFullWorkHistoryForIAP.AsEnumerable().Where(item => item.Field<DateTime>("FromDate") >= ldtDate).CopyToDataTable();
        //    //    }
        //    //}
        //}

        //public bool CheckIfAgeAtReemploymentIsGreaterThan65(DataTable adtFullWorkHistoryForIAP, DateTime adtDateOfBirth, int aintComputationYear)
        //{
        //    bool lblnAgeAtReemploymentGreaterThan65 = false;
        //    DataTable ldtIAPFiltered = new DataTable();
        //    if (adtFullWorkHistoryForIAP != null && adtFullWorkHistoryForIAP.Rows.Count > 0 &&
        //        adtFullWorkHistoryForIAP.AsEnumerable().Where(o => o.Field<Int16>("computationyear") == aintComputationYear).Count() > 0)
        //    {
        //        ldtIAPFiltered = adtFullWorkHistoryForIAP.AsEnumerable().Where(o => o.Field<Int16>("computationyear") == aintComputationYear).CopyToDataTable();
        //        if (ldtIAPFiltered != null && ldtIAPFiltered.Rows.Count > 0)
        //        {
        //            DateTime ldtFirstReemployedHour = Convert.ToDateTime(ldtIAPFiltered.Rows[0]["FromDate"]);
        //            lblnAgeAtReemploymentGreaterThan65 = busGlobalFunctions.CalculatePersonAge(adtDateOfBirth,
        //                ldtFirstReemployedHour) > 65 ? true : false;
        //        }
        //    }

        //    return lblnAgeAtReemploymentGreaterThan65;
        //}
        #endregion
    }
}
