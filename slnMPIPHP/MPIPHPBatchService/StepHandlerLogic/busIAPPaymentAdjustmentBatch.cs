using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;
using MPIPHP.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHPJobService;
using Sagitec.CustomDataObjects;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.Common;
using System.Threading.Tasks;
using Sagitec.ExceptionPub;

namespace MPIPHPJobService
{
    public class busIAPPaymentAdjustmentBatch : busBatchHandler
    {
        #region Properties

        private object iobjLock = null;
        int aintMaxYear;
        DateTime ldteNextBenefitPaymentDate;
        List<Int32> iclbSelectedPayee;
        public decimal idecRYAlloc2AsOnAwardedOnDate { get; set; }
        public decimal idecRYAlloc4AsOnAwardedOnDate { get; set; }
        public decimal idecRYIAPBalanceAsOnAwardedOnDate { get; set; }
        public decimal idecIAPBalanceAsOnAwardedOnDate { get; set; }
        public decimal idecL161BalanceAsOnAwardedOnDate { get; set; }
        public decimal idecL152BalanceAsOnAwardedOnDate { get; set; }
        public decimal idecQuarterlyAllocationIAPAsOnAwardedOnDate { get; set; }

        public DateTime idtBalanceAsOnAwardedOnDate { get; set; }
        public DateTime idtAwardedOnDateEffectiveDate { get; set; }

        public DataTable idtDroOfRetiredParticipants { get; set; }
        public DataTable idtDroBeforeRetirement { get; set; }

        public busCalculation ibusCalculation { get; set; }
        public static DataTable ldtbPersonData { get; set; }

        #endregion

        public busIAPPaymentAdjustmentBatch()
        {
            ibusCalculation = new busCalculation();
        }

        #region IAP_PAYMENT_ADJUSTMENT_BATCH

        public decimal LoadIapAdjustmentForQDRO(int aintPayeeBenefitAccountID, decimal adecParticipantIAPAdjustedBalance, DataTable adtQDROPayees)
        {
            decimal ldecQDROIAPBalance = decimal.Zero;
            if (adtQDROPayees.Rows.Count > 0)
            {
                DataRow ldtRow = null;
                int aintQDROCalculationDetailID;
                decimal ldecAlternatePayeeFraction = decimal.Zero;
                decimal ldecFlatPercenatage = decimal.Zero;
                decimal ldecFlatAmount = decimal.Zero;
                if (adtQDROPayees.AsEnumerable().Where(item => Convert.ToInt32(item[enmPayeeAccount.payee_benefit_account_id.ToString()]) == aintPayeeBenefitAccountID).Count() > 0)
                {
                    ldtRow = adtQDROPayees.AsEnumerable().Where(item => Convert.ToInt32(item[enmPayeeAccount.payee_benefit_account_id.ToString()]) == aintPayeeBenefitAccountID).FirstOrDefault();
                    if (!string.IsNullOrEmpty(Convert.ToString(ldtRow[enmPayeeAccount.dro_calculation_detail_id.ToString()])))
                    {
                        aintQDROCalculationDetailID = Convert.ToInt32(ldtRow[enmPayeeAccount.dro_calculation_detail_id.ToString()]);
                        if (!string.IsNullOrEmpty(Convert.ToString(ldtRow[enmQdroCalculationDetail.alt_payee_fraction.ToString()])))
                        {
                            ldecAlternatePayeeFraction = Convert.ToDecimal(ldtRow[enmQdroCalculationDetail.alt_payee_fraction.ToString()]);
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(ldtRow[enmQdroCalculationDetail.flat_percent.ToString()])))
                        {
                            ldecFlatPercenatage = Convert.ToDecimal(ldtRow[enmQdroCalculationDetail.flat_percent.ToString()]);
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(ldtRow[enmQdroCalculationDetail.flat_amount.ToString()])))
                        {
                            ldecFlatAmount = Convert.ToDecimal(ldtRow[enmQdroCalculationDetail.flat_amount.ToString()]);
                        }
                    }
                    ldecQDROIAPBalance = ibusCalculation.CalculateBenefitAmtBeforeConversion(adecParticipantIAPAdjustedBalance, ldecAlternatePayeeFraction, ldecFlatAmount, ldecFlatPercenatage);
                }
            }

            return ldecQDROIAPBalance;
        }

        public void IAPPaymentAdjustmentBatch()
        {
            busBase lobjBase = new busBase();
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            iobjLock = new object();
            iclbSelectedPayee = new List<int>();

            //PIR 985
            CreateReport();
        }

        private void IapCalculationForBenefitCalculation(DataTable ldtPayeeAccountList)
        {
            DateTime ldtDisbRetireementDate = new DateTime();
            DateTime ldtDisabilityAwardedOnDate = new DateTime();
            bool ablnCalculateDisabilityAdjustments = false;
            if (ldtPayeeAccountList.Rows.Count > 0)
            {
                foreach (DataRow ldrPerson in ldtPayeeAccountList.AsEnumerable())
                {
                    ablnCalculateDisabilityAdjustments = false;
                    if (ldrPerson["BENEFIT_CALCULATION_DETAIL_ID"].IsNotNull())
                    {
                        busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                        lbusPayeeAccount.icdoPayeeAccount.LoadData(ldrPerson);
                        busBenefitCalculationHeader lbusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                        lbusPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                        //PIR 869 Rohan
                        //lbusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.LoadData(ldrPerson);                        
                        lbusPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lbusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                        string lstrFundsType = string.Empty;
                        lstrFundsType = lbusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.funds_type_value;
                        // busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                        int lintBenefitCalculationHearderID = Convert.ToInt32(ldrPerson["BENEFIT_CALCULATION_HEADER_ID"]);
                        int lintBenefitCalculationDetailID = Convert.ToInt32(ldrPerson["BENEFIT_CALCULATION_DETAIL_ID"]);
                        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.LoadData(ldrPerson);
                        if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id != 0)// FindBenefitCalculationHeader(lintBenefitCalculationHearderID))
                        {

                            lbusBenefitCalculationHeader.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                            lbusBenefitCalculationHeader.ibusPerson.icdoPerson.LoadData(ldrPerson);
                            busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                            lbusPersonAccount.icdoPersonAccount.LoadData(ldrPerson);
                            lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                            lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount.Add(lbusPersonAccount);
                            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
                            busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                            lbusBenefitCalculationDetail.FindBenefitCalculationDetail(lintBenefitCalculationDetailID);
                            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetail);
                            //lbusBenefitCalculationHeader.LoadBenefitCalculationDetails();

                            lbusBenefitCalculationHeader.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                            lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                            lbusBenefitCalculationHeader.LoadAllRetirementContributions(null);

                            //PROD PIR 764
                            if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date == DateTime.MinValue)
                            {
                                if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_commencement_date != DateTime.MinValue)
                                {
                                    //Rohan 10062014 PIR 764
                                    DataTable ldtblFirstPaymentDate = busBase.Select("cdoIAPAllocationDetailPersonOverview.GetFirstPaymentDate", new object[1] { lbusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id });//PIR 869 Rohan
                                    if (ldtblFirstPaymentDate != null && ldtblFirstPaymentDate.Rows.Count > 0)
                                    {
                                        if (Convert.ToString(ldtblFirstPaymentDate.Rows[0][enmPaymentHistoryHeader.payment_date.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = Convert.ToDateTime(ldtblFirstPaymentDate.Rows[0][enmPaymentHistoryHeader.payment_date.ToString().ToUpper()]);
                                    }
                                }
                            }

                            if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id != 0)
                            {
                                lbusBenefitCalculationDetail = lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(obj => obj.icdoBenefitCalculationDetail.benefit_calculation_detail_id == lintBenefitCalculationDetailID).FirstOrDefault();

                                if (lbusBenefitCalculationDetail.IsNotNull())
                                {

                                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date != DateTime.MinValue)
                                    {
                                        if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                                        {
                                            busCalculation lbusCalculation = new busCalculation();

                                            ldtDisbRetireementDate = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;
                                            lbusBenefitCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = ldtDisbRetireementDate;
                                            ldtDisabilityAwardedOnDate = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date;
                                            if (ldtDisbRetireementDate >= lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date ||
                                                ldtDisabilityAwardedOnDate >= lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date)
                                            {
                                                if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date != DateTime.MinValue)
                                                {
                                                    if (aintMaxYear >= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.AddYears(-1).Year &&
                                                    aintMaxYear >= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date.AddYears(-1).Year)
                                                    {
                                                        if (lbusCalculation.CheckIfFactorAvailableForIapAllocation(lbusBenefitCalculationHeader))
                                                        {
                                                            ablnCalculateDisabilityAdjustments = true;
                                                        }
                                                    }
                                                }
                                                else if (aintMaxYear >= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.AddYears(-1).Year)
                                                {
                                                    if (lbusCalculation.CheckIfFactorAvailableForIapAllocation(lbusBenefitCalculationHeader))
                                                    {
                                                        ablnCalculateDisabilityAdjustments = true;
                                                    }
                                                }
                                            }
                                        }

                                        if (ablnCalculateDisabilityAdjustments)
                                        {
                                            if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date >= lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date
                                                && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date >= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date)
                                            {
                                                lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;
                                            }
                                            else if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date >= lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date
                                                && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date >= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date)
                                            {
                                                lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date;
                                            }
                                            else if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date >= lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date
                                                && lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date >= lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date)
                                            {
                                                if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date >= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date)
                                                {
                                                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date;
                                                }
                                                else
                                                {
                                                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;
                                                }
                                            }
                                            else
                                            {
                                                if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date >= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date)
                                                {
                                                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date;
                                                }
                                                else
                                                {
                                                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;
                                                }
                                            }
                                        }

                                        if ((lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date >= lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date &&
                                            lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_type_value != busConstant.BENEFIT_TYPE_DISABILITY)
                                            || ablnCalculateDisabilityAdjustments)
                                        {
                                            //bool IsAdjustmentDone;

                                            //if (aintMaxYear >= lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date.Year)
                                            if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.Year >= lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date.Year
                                                || ablnCalculateDisabilityAdjustments)//PROD PIR 764
                                            {

                                                #region Commented Code
                                                ////lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjusment_payment = CalculateIAPBenefitAmount(false, busConstant.CodeValueAll, lbusBenefitCalculationHeader, lstrFundsType, out IsAdjustmentDone);
                                                //lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjusment_payment = CalculateIAPBenefitAmount(false, busConstant.CodeValueAll, lbusBenefitCalculationHeader, lstrFundsType, out IsAdjustmentDone, lbusPersonAccount.icdoPersonAccount.person_account_id);//PROD PIR 764
                                                ////lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM21", lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjusment_payment, "0", 0,
                                                ////                                 ldteNextBenefitPaymentDate, DateTime.MinValue, "N", true);

                                                ////Payment Adjustment BR-023-84
                                                //decimal ldecFinalAdjusmentPayment = 0M;
                                                //decimal ldecAlternatePayeeAdjustment = decimal.Zero;

                                                //DataTable ldtblGetWithheldAmount = busBase.Select("cdoWithholdingInformation.GetTotalWithheldAmount", new object[3] {
                                                //lbusPayeeAccount.icdoPayeeAccount.payee_account_id,  lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date,
                                                //iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1)});

                                                //if (ldtblGetWithheldAmount.Rows.Count > 0)
                                                //{
                                                //    ldecFinalAdjusmentPayment = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjusment_payment +
                                                //        Convert.ToDecimal(ldtblGetWithheldAmount.Rows[0]["TAXABLE_WITHHOLD_FLAT_AMOUNT"]) +
                                                //        Convert.ToDecimal(ldtblGetWithheldAmount.Rows[0]["NON_TAXABLE_WITHHOLD_FLAT_AMOUNT"]);

                                                //    lbusPayeeAccount.iclbWithholdingInformation = new Collection<busWithholdingInformation>();
                                                //    lbusPayeeAccount.LoadWithholdingInformation();

                                                //    lbusPayeeAccount.LoadNextBenefitPaymentDate();

                                                //    foreach (busWithholdingInformation lbusWithholdingInfo in lbusPayeeAccount.iclbWithholdingInformation)
                                                //    {
                                                //        if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue)
                                                //        {
                                                //            if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from > lbusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1))
                                                //            {
                                                //                lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from;
                                                //            }
                                                //            else
                                                //            {
                                                //                lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lbusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1);
                                                //            }

                                                //            lbusWithholdingInfo.icdoWithholdingInformation.Update();
                                                //        }
                                                //    }
                                                //}
                                                //else
                                                //{
                                                //    ldecFinalAdjusmentPayment = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjusment_payment;
                                                //}
                                                //if (idtDroOfRetiredParticipants.AsEnumerable().Where(item => Convert.ToInt32(item[enmPayeeAccount.payee_benefit_account_id.ToString()]) == Convert.ToInt32(ldrPerson[enmPayeeAccount.payee_benefit_account_id.ToString()])).Count() > 0)
                                                //{
                                                //    DataRow ldtRow = idtDroOfRetiredParticipants.AsEnumerable().Where(item => Convert.ToInt32(item[enmPayeeAccount.payee_benefit_account_id.ToString()]) == Convert.ToInt32(ldrPerson[enmPayeeAccount.payee_benefit_account_id.ToString()])).FirstOrDefault();
                                                //    CalculateIapAdjustmentsForAlternatePayee(ldtRow, ref ldecAlternatePayeeAdjustment);
                                                //}
                                                //ldecFinalAdjusmentPayment = ldecFinalAdjusmentPayment - ldecAlternatePayeeAdjustment;

                                                //if (ldecFinalAdjusmentPayment > 0)
                                                //{
                                                //    //PROD PIR-764
                                                //    if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.survivor_percentage != 0 && lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.survivor_percentage.IsNotNull())
                                                //    ldecFinalAdjusmentPayment = ldecFinalAdjusmentPayment * (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.survivor_percentage / 100); 
                                                //    lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM21", ldecFinalAdjusmentPayment, "0", 0,
                                                //                                   ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                                //    iclbSelectedPayee.Add(lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
                                                //    lbusPayeeAccount.ProcessTaxWithHoldingDetails();
                                                //    lbusPayeeAccount.CreateReviewPayeeAccountStatusAftComplte();
                                                //}
                                                //else if (ldecFinalAdjusmentPayment != 0)
                                                //{
                                                //    decimal OverPaidTaxableAmount = -(ldecFinalAdjusmentPayment);
                                                //    lbusPayeeAccount.CreateOverPayments(lbusPayeeAccount, ldteNextBenefitPaymentDate, DateTime.MinValue, DateTime.MinValue, OverPaidTaxableAmount, 0,
                                                //        busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH);
                                                //    busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PROCESS_OVERPAYMENT_WORKFLOW, lbusPayeeAccount.icdoPayeeAccount.person_id,
                                                //        0, lbusPayeeAccount.icdoPayeeAccount.payee_account_id, null);
                                                //    iclbSelectedPayee.Add(lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
                                                //    lbusPayeeAccount.CreateReviewPayeeAccountStatusAftComplte();
                                                //}
                                                //if (IsAdjustmentDone)
                                                //{
                                                //    lbusPayeeAccount.icdoPayeeAccount.adjustment_payment_eligible_flag = busConstant.FLAG_NO;
                                                //    lbusPayeeAccount.icdoPayeeAccount.Update();
                                                //}

                                                // pir - 764 above code written in below seperate method 
                                                #endregion Commented Code
                                                IapFinalAdjustmentBenefitCalculation(lbusBenefitCalculationDetail, lbusBenefitCalculationHeader, lstrFundsType, lbusPersonAccount, lbusPayeeAccount, ldrPerson);

                                            }
                                            //pir - 764 - ROHAN
                                            else
                                            {
                                                if (!ablnCalculateDisabilityAdjustments)
                                                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date = DateTime.Now;

                                                IapFinalAdjustmentBenefitCalculation(lbusBenefitCalculationDetail, lbusBenefitCalculationHeader, lstrFundsType, lbusPersonAccount, lbusPayeeAccount, ldrPerson);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        // pir - 764 for post late retirement and iap_as_of_date is null
                                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date == DateTime.MinValue)
                                        {
                                            IapFinalAdjustmentBenefitCalculation(lbusBenefitCalculationDetail, lbusBenefitCalculationHeader, lstrFundsType, lbusPersonAccount, lbusPayeeAccount, ldrPerson);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void IapFinalAdjustmentBenefitCalculation(busBenefitCalculationDetail lbusBenefitCalculationDetail, busBenefitCalculationHeader lbusBenefitCalculationHeader, string lstrFundsType, busPersonAccount lbusPersonAccount, busPayeeAccount lbusPayeeAccount, DataRow ldrPerson)
        {
            try
            {
                bool IsAdjustmentDone;
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjusment_payment = CalculateIAPBenefitAmount(false, busConstant.CodeValueAll, lbusBenefitCalculationHeader, lstrFundsType, out IsAdjustmentDone, lbusPersonAccount.icdoPersonAccount.person_account_id);//PROD PIR 764

                busIapAllocationSummary ibusLatestIAPAllocationSummaryAsofYear = new busIapAllocationSummary();
                ibusLatestIAPAllocationSummaryAsofYear.LoadLatestAllocationSummaryAsofYear(lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.Year);

                decimal lintCompYear = 0M;
                if (ibusLatestIAPAllocationSummaryAsofYear.IsNotNull())
                {
                    lintCompYear = ibusLatestIAPAllocationSummaryAsofYear.GetMaxAllocationYear();
                }
                //for persons whoes retirements prior to the last computation year
                if (lintCompYear >= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.Year)
                {
                    DataTable ldtbIAPBalance = new DataTable();
                    if (lbusPersonAccount.icdoPersonAccount.person_account_id != 0)
                    {
                        ldtbIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceForYear",
                                   new object[1] { lbusPersonAccount.icdoPersonAccount.person_account_id });

                        if (ldtbIAPBalance.IsNotNull() && ldtbIAPBalance.Rows.Count > 0)
                        {
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjusment_payment = Convert.ToDecimal(Convert.ToBoolean(ldtbIAPBalance.Rows[0][0].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbIAPBalance.Rows[0][0]);
                        }
                        IsAdjustmentDone = true;
                    }
                }

                decimal ldecFinalAdjusmentPayment = 0M;
                decimal ldecAlternatePayeeAdjustment = decimal.Zero;

                DataTable ldtblGetWithheldAmount = busBase.Select("cdoWithholdingInformation.GetTotalWithheldAmount", new object[3] {
                                                lbusPayeeAccount.icdoPayeeAccount.payee_account_id,  lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date,
                                                iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1)});

                if (ldtblGetWithheldAmount.Rows.Count > 0)
                {
                    ldecFinalAdjusmentPayment = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjusment_payment +
                        Convert.ToDecimal(ldtblGetWithheldAmount.Rows[0]["TAXABLE_WITHHOLD_FLAT_AMOUNT"]) +
                        Convert.ToDecimal(ldtblGetWithheldAmount.Rows[0]["NON_TAXABLE_WITHHOLD_FLAT_AMOUNT"]);

                    lbusPayeeAccount.iclbWithholdingInformation = new Collection<busWithholdingInformation>();
                    lbusPayeeAccount.LoadWithholdingInformation();

                    lbusPayeeAccount.LoadNextBenefitPaymentDate();

                    foreach (busWithholdingInformation lbusWithholdingInfo in lbusPayeeAccount.iclbWithholdingInformation)
                    {
                        if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue)
                        {
                            if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from > lbusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1))
                            {
                                lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from;
                            }
                            else
                            {
                                lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lbusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1);
                            }

                            lbusWithholdingInfo.icdoWithholdingInformation.Update();
                        }
                    }
                }
                else
                {
                    ldecFinalAdjusmentPayment = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjusment_payment;
                }
                if (idtDroOfRetiredParticipants.AsEnumerable().Where(item => Convert.ToInt32(item[enmPayeeAccount.payee_benefit_account_id.ToString()]) == Convert.ToInt32(ldrPerson[enmPayeeAccount.payee_benefit_account_id.ToString()])).Count() > 0)
                {
                    DataRow ldtRow = idtDroOfRetiredParticipants.AsEnumerable().Where(item => Convert.ToInt32(item[enmPayeeAccount.payee_benefit_account_id.ToString()]) == Convert.ToInt32(ldrPerson[enmPayeeAccount.payee_benefit_account_id.ToString()])).FirstOrDefault();
                    CalculateIapAdjustmentsForAlternatePayee(ldtRow, ref ldecAlternatePayeeAdjustment);
                }
                ldecFinalAdjusmentPayment = ldecFinalAdjusmentPayment - ldecAlternatePayeeAdjustment;

                //Tushart
                DataRow ldrNewRow = FillData(lbusPayeeAccount, ldecFinalAdjusmentPayment);
                if (ldrNewRow.IsNotNull())
                {
                    ldtbPersonData.Rows.Add(ldrNewRow);
                }

                if (ldecFinalAdjusmentPayment > 0)
                {
                    //PROD PIR-764
                    if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.survivor_percentage != 0 && lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.survivor_percentage.IsNotNull())
                        ldecFinalAdjusmentPayment = ldecFinalAdjusmentPayment * (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.survivor_percentage / 100);
                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM21", ldecFinalAdjusmentPayment, "0", 0,
                                                   ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                    iclbSelectedPayee.Add(lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
                    lbusPayeeAccount.ProcessTaxWithHoldingDetails();
                    lbusPayeeAccount.CreateReviewPayeeAccountStatusAftComplte();
                }
                else if (ldecFinalAdjusmentPayment != 0)
                {
                    decimal OverPaidTaxableAmount = -(ldecFinalAdjusmentPayment);
                    lbusPayeeAccount.CreateOverPayments(lbusPayeeAccount, ldteNextBenefitPaymentDate, DateTime.MinValue, DateTime.MinValue, OverPaidTaxableAmount, 0,
                        busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH);
                    busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PROCESS_OVERPAYMENT_WORKFLOW, lbusPayeeAccount.icdoPayeeAccount.person_id,
                        0, lbusPayeeAccount.icdoPayeeAccount.payee_account_id, null);
                    iclbSelectedPayee.Add(lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
                    lbusPayeeAccount.CreateReviewPayeeAccountStatusAftComplte();
                }
                //if (IsAdjustmentDone)
                //{
                lbusPayeeAccount.icdoPayeeAccount.adjustment_payment_eligible_flag = busConstant.FLAG_NO;
                lbusPayeeAccount.icdoPayeeAccount.Update();
                // }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void IapCalculationForConversion(DataTable ldtPayeeAccountList)
        {
            bool ablnCalculateDisabilityAdjustments = false;
            DateTime ldtDisbRetireementDate = new DateTime();
            DateTime ldtDisabilityAwardedOnDate = new DateTime();
            if (ldtPayeeAccountList.Rows.Count > 0)
            {
                foreach (DataRow ldrPerson in ldtPayeeAccountList.AsEnumerable())
                {
                    ablnCalculateDisabilityAdjustments = false;
                    if (ldrPerson["BENEFIT_APPLICATION_ID"].IsNotNull())
                    {
                        busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                        lbusPayeeAccount.icdoPayeeAccount.LoadData(ldrPerson);
                        busBenefitCalculationHeader lbusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                        busBenefitApplication lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lbusPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                        lbusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.LoadData(ldrPerson);
                        string lstrFundsType = string.Empty;
                        lstrFundsType = lbusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.funds_type_value;
                        int lintPersonAccountID = Convert.ToInt32(ldrPerson["PERSON_ACCOUNT_ID"]);
                        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.LoadData(ldrPerson);

                        lbusBenefitApplication.icdoBenefitApplication.LoadData(ldrPerson);
                        if (lbusBenefitApplication.icdoBenefitApplication.benefit_application_id != 0)// FindBenefitCalculationHeader(lintBenefitCalculationHearderID))
                        {
                            lbusBenefitCalculationHeader.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                            lbusBenefitCalculationHeader.ibusPerson.icdoPerson.LoadData(ldrPerson);
                            busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                            lbusPersonAccount.icdoPersonAccount.LoadData(ldrPerson);
                            lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                            lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount.Add(lbusPersonAccount);
                            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
                            //busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                            //lbusBenefitCalculationDetail.FindBenefitCalculationDetail(lintBenefitCalculationDetailID);
                            //lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Add(lbusBenefitCalculationDetail);
                            //lbusBenefitCalculationHeader.LoadBenefitCalculationDetails();
                            lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date = lbusBenefitApplication.icdoBenefitApplication.awarded_on_date;
                            lbusBenefitCalculationHeader.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                            lbusBenefitCalculationHeader.ibusBenefitApplication = lbusBenefitApplication;
                            lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                            lbusBenefitCalculationHeader.LoadRetirementContributionsbyAccountId(lintPersonAccountID);
                            //DateTime iap_as_of_date = new DateTime();
                            decimal adjusment_payment = 0M;
                            if (lbusBenefitApplication.icdoBenefitApplication.benefit_application_id != 0)
                            {
                                var Lst_iap_as_of_date = lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(o => o.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT").OrderByDescending(obj => obj.icdoPersonAccountRetirementContribution.effective_date).FirstOrDefault();
                                if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                                {
                                    ldtDisbRetireementDate = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;
                                    ldtDisabilityAwardedOnDate = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date;
                                    busCalculation lbusCalculation = new busCalculation();
                                    //if (ldtDisbRetireementDate > Lst_iap_as_of_date.icdoPersonAccountRetirementContribution.effective_date ||
                                    //    ldtDisabilityAwardedOnDate > Lst_iap_as_of_date.icdoPersonAccountRetirementContribution.effective_date)
                                    //{
                                    if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date != DateTime.MinValue)
                                    {
                                        if (aintMaxYear >= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.AddYears(-1).Year &&
                                        aintMaxYear >= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date.AddYears(-1).Year)
                                        {
                                            if (lbusCalculation.CheckIfFactorAvailableForIapAllocation(lbusBenefitCalculationHeader))
                                            {
                                                ablnCalculateDisabilityAdjustments = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (aintMaxYear >= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.AddYears(-1).Year)
                                        {
                                            if (lbusCalculation.CheckIfFactorAvailableForIapAllocation(lbusBenefitCalculationHeader))
                                            {
                                                ablnCalculateDisabilityAdjustments = true;
                                            }
                                        }
                                    }
                                    //}
                                }
                                if (Lst_iap_as_of_date.IsNotNull())
                                {
                                    if ((//lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date > Lst_iap_as_of_date.icdoPersonAccountRetirementContribution.effective_date && 
                                        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue &&
                                        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_type_value != busConstant.BENEFIT_TYPE_DISABILITY)
                                        || ablnCalculateDisabilityAdjustments)
                                    {
                                        bool IsAdjustmentDone;
                                        //if (aintMaxYear >= Lst_iap_as_of_date.icdoPersonAccountRetirementContribution.effective_date.Year)
                                        //{

                                        adjusment_payment = CalculateIAPBenefitAmount(false, busConstant.CodeValueAll, lbusBenefitCalculationHeader, lstrFundsType, out IsAdjustmentDone, Lst_iap_as_of_date.icdoPersonAccountRetirementContribution.person_account_id);
                                        //lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM21", lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjusment_payment, "0", 0,
                                        //                                 ldteNextBenefitPaymentDate, DateTime.MinValue, "N", true);

                                        //Payment Adjustment BR-023-84
                                        decimal ldecFinalAdjusmentPayment = 0M;

                                        DataTable ldtblGetWithheldAmount = busBase.Select("cdoWithholdingInformation.GetTotalWithheldAmount", new object[3] {
                                           lbusPayeeAccount.icdoPayeeAccount.payee_account_id,  lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date,
                                           iobjSystemManagement.icdoSystemManagement.batch_date});

                                        if (ldtblGetWithheldAmount.Rows.Count > 0)
                                        {
                                            ldecFinalAdjusmentPayment = adjusment_payment +
                                                Convert.ToDecimal(ldtblGetWithheldAmount.Rows[0]["TAXABLE_WITHHOLD_FLAT_AMOUNT"]) +
                                                Convert.ToDecimal(ldtblGetWithheldAmount.Rows[0]["NON_TAXABLE_WITHHOLD_FLAT_AMOUNT"]);

                                            lbusPayeeAccount.iclbWithholdingInformation = new Collection<busWithholdingInformation>();
                                            lbusPayeeAccount.LoadWithholdingInformation();

                                            lbusPayeeAccount.LoadNextBenefitPaymentDate();

                                            foreach (busWithholdingInformation lbusWithholdingInfo in lbusPayeeAccount.iclbWithholdingInformation)
                                            {
                                                if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue)
                                                {
                                                    if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from > lbusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1))
                                                    {
                                                        lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from;
                                                    }
                                                    else
                                                    {
                                                        lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lbusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1);
                                                    }

                                                    lbusWithholdingInfo.icdoWithholdingInformation.Update();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ldecFinalAdjusmentPayment = adjusment_payment;
                                        }

                                        //INSERT DATA TO TEMP TABLE FOR ALL TYPE OF ENTRIES-TusharT
                                        DataRow ldrNewRow = FillData(lbusPayeeAccount, ldecFinalAdjusmentPayment);
                                        if (ldrNewRow.IsNotNull())
                                        {
                                            ldtbPersonData.Rows.Add(ldrNewRow);
                                        }

                                        if (ldecFinalAdjusmentPayment > 0)
                                        {
                                            lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM21", ldecFinalAdjusmentPayment, "0", 0,
                                                                           ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                            // Recalculate Tax
                                            //if (lbusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                                            //    lbusPayeeAccount.idtNextBenefitPaymentDate = ldteNextBenefitPaymentDate;
                                            lbusPayeeAccount.ProcessTaxWithHoldingDetails();
                                            iclbSelectedPayee.Add(lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
                                            lbusPayeeAccount.CreateReviewPayeeAccountStatusAftComplte();
                                        }
                                        else if (ldecFinalAdjusmentPayment != 0)
                                        {
                                            decimal OverPaidTaxableAmount = -(ldecFinalAdjusmentPayment);
                                            lbusPayeeAccount.CreateOverPayments(lbusPayeeAccount, ldteNextBenefitPaymentDate, DateTime.MinValue, DateTime.MinValue, OverPaidTaxableAmount, 0,
                                                busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH);
                                            busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PROCESS_OVERPAYMENT_WORKFLOW, lbusPayeeAccount.icdoPayeeAccount.person_id,
                                               0, lbusPayeeAccount.icdoPayeeAccount.payee_account_id, null);
                                            iclbSelectedPayee.Add(lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
                                            lbusPayeeAccount.CreateReviewPayeeAccountStatusAftComplte();

                                        }

                                        //if (IsAdjustmentDone)
                                        //{
                                        lbusPayeeAccount.icdoPayeeAccount.adjustment_payment_eligible_flag = busConstant.FLAG_NO;
                                        lbusPayeeAccount.icdoPayeeAccount.Update();
                                        //}

                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void IapCalculationForQdroCalculation(DataTable ldtPayeeAccountListQdro)
        {
            if (ldtPayeeAccountListQdro.Rows.Count > 0)
            {
                decimal ldecIAPAdjustment = decimal.Zero;
                foreach (DataRow ldrPerson in ldtPayeeAccountListQdro.AsEnumerable())
                {
                    CalculateIapAdjustmentsForAlternatePayee(ldrPerson, ref ldecIAPAdjustment, decimal.Zero);
                }
            }
        }

        private void CalculateIapAdjustmentsForAlternatePayee(DataRow adtAlternatePayee, ref decimal adecFinalAdjustmentPayment, decimal adecParticipantAdjustedAmount = decimal.Zero)
        {
            decimal ldecFinalAdjusmentPayment = 0M;
            if (adtAlternatePayee[enmDroBenefitDetails.dro_benefit_id.ToString()].IsNotNull())
            {
                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lbusPayeeAccount.icdoPayeeAccount.LoadData(adtAlternatePayee);
                busQdroCalculationHeader lbusQdroCalculationHeader = new busQdroCalculationHeader { icdoQdroCalculationHeader = new cdoQdroCalculationHeader() };
                lbusPayeeAccount.LoadNextBenefitPaymentDate();
                lbusPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                lbusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.LoadData(adtAlternatePayee);
                string lstrFundsType = string.Empty;
                lstrFundsType = lbusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.funds_type_value;
                //int lintQdroCalculationHearderID = Convert.ToInt32(adtAlternatePayee[enmQdroCalculationHeader.qdro_calculation_header_id.ToString()]);
                decimal ldecAdjustmentIapBalance = decimal.Zero;
                bool IsAdjustmentDone = false;

                #region Calculation
                if (Convert.ToString(adtAlternatePayee[enmPayeeAccount.dro_calculation_detail_id.ToString()]).IsNotNullOrEmpty())
                {
                    int lintQdroCalculationDetailID = Convert.ToInt32(adtAlternatePayee[enmPayeeAccount.dro_calculation_detail_id.ToString()]);
                    busQdroCalculationDetail lbusQdroCalculationDetail = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };

                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.LoadData(adtAlternatePayee);
                    lbusQdroCalculationHeader.FindQdroCalculationHeader(lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id);
                    lbusQdroCalculationHeader.iclbQdroCalculationDetail = new Collection<busQdroCalculationDetail>();
                    lbusQdroCalculationHeader.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetail);

                    lbusQdroCalculationHeader.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };
                    lbusQdroCalculationHeader.ibusAlternatePayee.FindPerson(Convert.ToInt32(adtAlternatePayee[enmDroApplication.alternate_payee_id.ToString()]));
                    busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                    lbusPersonAccount.icdoPersonAccount.LoadData(adtAlternatePayee);
                    lbusQdroCalculationHeader.ibusAlternatePayee.iclbPersonAccount = new Collection<busPersonAccount>();
                    lbusQdroCalculationHeader.ibusAlternatePayee.iclbPersonAccount.Add(lbusPersonAccount);
                    lbusQdroCalculationHeader.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lbusQdroCalculationHeader.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                    if (lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id != 0)
                    {
                        //lbusQdroCalculationDetail = lbusQdroCalculationHeader.iclbQdroCalculationDetail.Where(obj => obj.icdoQdroCalculationDetail.qdro_calculation_header_id == lintQdroCalculationDetailID).FirstOrDefault();

                        if (lbusQdroCalculationDetail.IsNotNull())
                        {
                            lbusQdroCalculationDetail.LoadQdroIapAllocationDetails();

                            lbusQdroCalculationHeader.icdoQdroCalculationHeader.retirement_date = lbusQdroCalculationHeader.GetRetirementDateforCalculation();
                            if (lbusQdroCalculationHeader.icdoQdroCalculationHeader.retirement_date > lbusQdroCalculationDetail.icdoQdroCalculationDetail.iap_as_of_date)
                            {
                                bool lblnCheckIfAdjustmentNeedsToBeDone = true;
                                DateTime ldtFromDate = lbusQdroCalculationDetail.icdoQdroCalculationDetail.net_investment_from_date;
                                DateTime ldtToDate = lbusQdroCalculationDetail.icdoQdroCalculationDetail.net_investment_to_date;
                                if (ldtFromDate != DateTime.MinValue && ldtToDate != DateTime.MinValue)
                                {
                                    if (!lbusQdroCalculationDetail.iclbQdroIapAllocationDetail.IsNullOrEmpty())
                                    {
                                        decimal ldecAmount = lbusQdroCalculationDetail.iclbQdroIapAllocationDetail.OrderByDescending(item => item.icdoQdroIapAllocationDetail.computation_year).FirstOrDefault().icdoQdroIapAllocationDetail.gain_loss_amount;
                                        if (ldecAmount > decimal.Zero)
                                        {
                                            lblnCheckIfAdjustmentNeedsToBeDone = false;
                                        }
                                        else
                                        {
                                            int lintCompYear = lbusQdroCalculationDetail.iclbQdroIapAllocationDetail.OrderBy(item => item.icdoQdroIapAllocationDetail.computation_year).Where(item => item.icdoQdroIapAllocationDetail.gain_loss_amount == decimal.Zero).FirstOrDefault().icdoQdroIapAllocationDetail.computation_year;
                                            if (lintCompYear > ldtFromDate.Year)
                                            {
                                                ldtFromDate = new DateTime(lintCompYear, 1, 1);
                                            }
                                        }
                                    }
                                }
                                if (ldtFromDate == DateTime.MinValue || ldtToDate == DateTime.MinValue)
                                {
                                    return;
                                }
                                if (aintMaxYear > lbusQdroCalculationDetail.icdoQdroCalculationDetail.balance_as_of_plan_year && lblnCheckIfAdjustmentNeedsToBeDone)
                                {
                                    ldecAdjustmentIapBalance = CalculateQDROIAPBenefitAmount(lbusQdroCalculationHeader, lbusPayeeAccount.icdoPayeeAccount.payee_account_id, ldtFromDate, lbusQdroCalculationDetail.icdoQdroCalculationDetail.net_investment_to_date, lstrFundsType, out IsAdjustmentDone);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Conversion
                else
                {
                    busDroBenefitDetails lbusDroBenefitDetails = new busDroBenefitDetails { icdoDroBenefitDetails = new cdoDroBenefitDetails() };
                    lbusDroBenefitDetails.icdoDroBenefitDetails.LoadData(adtAlternatePayee);
                    busQdroApplication lbusQdroApplication = new busQdroApplication { icdoDroApplication = new cdoDroApplication() };
                    lbusQdroApplication.FindQdroApplication(lbusDroBenefitDetails.icdoDroBenefitDetails.dro_application_id);

                    lbusQdroApplication.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };
                    lbusQdroApplication.ibusAlternatePayee.FindPerson(Convert.ToInt32(adtAlternatePayee[enmDroApplication.alternate_payee_id.ToString()]));


                    DateTime ldtFromDate = lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_from_date;
                    DateTime ldtToDate = lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date;
                    if (ldtFromDate == DateTime.MinValue || ldtToDate == DateTime.MinValue)
                    {
                        return;
                    }
                    if (aintMaxYear > lbusDroBenefitDetails.icdoDroBenefitDetails.balance_as_of_plan_year)
                    {
                        ldecAdjustmentIapBalance = CalculateQDROIAPBenefitAmount(null, lbusPayeeAccount.icdoPayeeAccount.payee_account_id, ldtFromDate, ldtToDate, lstrFundsType, out IsAdjustmentDone, lbusQdroApplication);
                    }
                }
                #endregion

                //lbusQdroCalculationDetail.icdoQdroCalculationDetail.adjusment_payment = CalculateIAPBenefitAmount(true, busConstant.CodeValueAll, null, lbusQdroCalculationHeader, lstrFundsType, out IsAdjustmentDone);
                DataTable ldtblGetWithheldAmount = busBase.Select("cdoWithholdingInformation.GetTotalWithheldAmount", new object[3] {
                                           lbusPayeeAccount.icdoPayeeAccount.payee_account_id,  lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date,
                                           iobjSystemManagement.icdoSystemManagement.batch_date});

                if (ldtblGetWithheldAmount.Rows.Count > 0)
                {
                    ldecFinalAdjusmentPayment = ldecAdjustmentIapBalance +
                        Convert.ToDecimal(ldtblGetWithheldAmount.Rows[0]["TAXABLE_WITHHOLD_FLAT_AMOUNT"]) +
                        Convert.ToDecimal(ldtblGetWithheldAmount.Rows[0]["NON_TAXABLE_WITHHOLD_FLAT_AMOUNT"]);

                    lbusPayeeAccount.iclbWithholdingInformation = new Collection<busWithholdingInformation>();
                    lbusPayeeAccount.LoadWithholdingInformation();

                    lbusPayeeAccount.LoadNextBenefitPaymentDate();

                    foreach (busWithholdingInformation lbusWithholdingInfo in lbusPayeeAccount.iclbWithholdingInformation)
                    {
                        if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue)
                        {
                            if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from > lbusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1))
                            {
                                lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from;
                            }
                            else
                            {
                                lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lbusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1);
                            }

                            lbusWithholdingInfo.icdoWithholdingInformation.Update();
                        }
                    }
                }
                else
                {
                    ldecFinalAdjusmentPayment = ldecAdjustmentIapBalance;
                }

                //INSERT DATA TO TEMP TABLE FOR ALL TYPE OF ENTRIES-TusharT
                DataRow ldrNewRow = FillData(lbusPayeeAccount, ldecFinalAdjusmentPayment);
                if (ldrNewRow.IsNotNull())
                {
                    ldtbPersonData.Rows.Add(ldrNewRow);
                }

                if (ldecFinalAdjusmentPayment > 0)
                {
                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM21, ldecFinalAdjusmentPayment, "0", 0,
                                                     ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                    lbusPayeeAccount.CreateReviewPayeeAccountStatusAftComplte();
                    iclbSelectedPayee.Add(lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
                    // Recalculate Tax
                    //if (lbusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    //    lbusPayeeAccount.idtNextBenefitPaymentDate = ldteNextBenefitPaymentDate;
                    //lbusPayeeAccount.ProcessTaxWithHoldingDetails();
                }
                else if (ldecFinalAdjusmentPayment != 0)
                {
                    decimal OverPaidTaxableAmount = -(ldecFinalAdjusmentPayment);
                    lbusPayeeAccount.CreateOverPayments(lbusPayeeAccount, ldteNextBenefitPaymentDate, DateTime.MinValue, DateTime.MinValue, OverPaidTaxableAmount, 0,
                        busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH);
                    lbusPayeeAccount.CreateReviewPayeeAccountStatusAftComplte();
                    iclbSelectedPayee.Add(lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
                }

                //if (IsAdjustmentDone)
                //{
                lbusPayeeAccount.icdoPayeeAccount.adjustment_payment_eligible_flag = busConstant.FLAG_NO;
                lbusPayeeAccount.icdoPayeeAccount.Update();
                //}
                adecFinalAdjustmentPayment = ldecFinalAdjusmentPayment;
            }
        }

        private decimal CalculateIAPBenefitAmount(bool isQdro, string astrBenefitOptionValue, busBenefitCalculationHeader objbusBenefitCalculationHeader, string lstrFundsType, out bool IsAdjustmentDone, int person_account_id = 0)
        {
            Decimal ldecIAPBalance = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHours4QtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHoursA2forQtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPPercent4forQtrAlloc = busConstant.ZERO_DECIMAL;
            busCalculation lbusCalculation = new busCalculation();

            //PIR 764 rohan
            //if (objbusBenefitCalculationHeader.IsNotNull() && objbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
            //{
            //    if (objbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date != DateTime.MinValue)
            //    {
            //        CalculateDisabilityBenefitAmountAsOnAwardedOnDate(astrBenefitOptionValue, objbusBenefitCalculationHeader, lstrFundsType, person_account_id);
            //        if (this.idecIAPBalanceAsOnAwardedOnDate != 0 || this.idecL161BalanceAsOnAwardedOnDate != 0 || this.idecL152BalanceAsOnAwardedOnDate != 0)
            //        {
            //            lbusCalculation.idecRYIAPBalanceAsOnAwardedOnDate = this.idecRYIAPBalanceAsOnAwardedOnDate;
            //            lbusCalculation.idecRYAlloc4AsOnAwardedOnDate = this.idecRYAlloc4AsOnAwardedOnDate;
            //            lbusCalculation.idecRYAlloc2AsOnAwardedOnDate = this.idecRYAlloc2AsOnAwardedOnDate;
            //            lbusCalculation.idtBalanceAsOnAwardedOnDate = this.idtBalanceAsOnAwardedOnDate;
            //            lbusCalculation.idtAwardedOnDateEffectiveDate = this.idtAwardedOnDateEffectiveDate;
            //            lbusCalculation.idecIAPBalanceAsOnAwardedOnDate = this.idecIAPBalanceAsOnAwardedOnDate;
            //            lbusCalculation.idecL161BalanceAsOnAwardedOnDate = this.idecL161BalanceAsOnAwardedOnDate;
            //            lbusCalculation.idecL152BalanceAsOnAwardedOnDate = this.idecL152BalanceAsOnAwardedOnDate;
            //            lbusCalculation.idecQuarterlyAllocationIAPAsOnAwardedOnDate = this.idecQuarterlyAllocationIAPAsOnAwardedOnDate;
            //            //lbusCalculation.idecL161BalanceAsOnAwardedOnDate = this.idecL161BalanceAsOnAwardedOnDate;
            //            //lbusCalculation.idecL152BalanceAsOnAwardedOnDate = this.idecL152BalanceAsOnAwardedOnDate;
            //        }
            //    }
            //}

            #region To Set Values for IAP QTR Allocations
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            SqlParameter[] parameters = new SqlParameter[3];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
            SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);
            DateTime retirement_date = new DateTime();
            DateTime ldtRetirementDateForPostingContributions = new DateTime();


            retirement_date = objbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;
            ldtRetirementDateForPostingContributions = objbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;

            param1.Value = objbusBenefitCalculationHeader.ibusPerson.icdoPerson.istrSSNNonEncrypted;
            parameters[0] = param1;
            busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
            lbusIapAllocationSummary.LoadLatestAllocationSummary();
            param2.Value = busGlobalFunctions.GetFirstDateOfComputationYear(retirement_date.Year);
            parameters[1] = param2;
            param3.Value = busGlobalFunctions.GetLastDayOfWeek(retirement_date); //PROD PIR 113
            parameters[2] = param3;

            DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPHourInfoForQuarterlyAllocation", astrLegacyDBConnetion, null, parameters);
            if (ldtbIAPInfo.Rows.Count > 0)
            {
                //if (ldtbIAPInfo.Rows[0]["IAPHours"] != DBNull.Value)
                //    ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHours"]);

                //if (ldtbIAPInfo.Rows[0]["IAPHoursA2"] != DBNull.Value)
                //    ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHoursA2"]);

                //if (ldtbIAPInfo.Rows[0]["IAPPercent"] != DBNull.Value)
                //    ldecIAPPercent4forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPPercent"]);

                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")) > 0)
                    ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")));

                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")) > 0)
                    ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")));

                DataTable ldtIAPFiltered;
                busIAPAllocationHelper aobjIAPAllocationHelper = new busIAPAllocationHelper();
                foreach (DataRow ldrIAPPercent in ldtbIAPInfo.Rows)
                {
                    if (ldrIAPPercent["IAPPercent"] != DBNull.Value && Convert.ToString(ldrIAPPercent["IAPPercent"]).IsNotNullOrEmpty() &&
                        Convert.ToDecimal(ldrIAPPercent["IAPPercent"]) > 0)
                    {
                        ldtIAPFiltered = new DataTable();
                        ldtIAPFiltered = ldtbIAPInfo.AsEnumerable().Where(o => o.Field<Int16?>("ComputationYear") == Convert.ToInt16(ldrIAPPercent["ComputationYear"])
                            && o.Field<int?>("EmpAccountNo") == Convert.ToInt32(ldrIAPPercent["EmpAccountNo"])).CopyToDataTable();

                        ldecIAPPercent4forQtrAlloc += aobjIAPAllocationHelper.CalculateAllocation4Amount(Convert.ToInt32(ldrIAPPercent["ComputationYear"]), ldtIAPFiltered);
                    }

                }
            }
            #endregion
            if (person_account_id == 0)
            {

                ////PROD PIR 764
                //if (objbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date != DateTime.MinValue)
                //    retirement_date = objbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date;
                ldecIAPBalance = lbusCalculation.GetIAPAndSpecialAdjustmentBalance(isQdro, objbusBenefitCalculationHeader.iclbBenefitCalculationDetail, objbusBenefitCalculationHeader,
                      retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, retirement_date.Year >= aintMaxYear ? aintMaxYear : retirement_date.Year, lstrFundsType, ldtRetirementDateForPostingContributions, out IsAdjustmentDone);

            }
            else
            {

                ////PROD PIR 764
                //if (objbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date != DateTime.MinValue)
                //    retirement_date = objbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date;
                ldecIAPBalance = lbusCalculation.GetIAPAndSpecialAdjustmentBalance(isQdro, objbusBenefitCalculationHeader.iclbBenefitCalculationDetail, objbusBenefitCalculationHeader,
                      retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, retirement_date.Year >= aintMaxYear ? aintMaxYear : retirement_date.Year, lstrFundsType, ldtRetirementDateForPostingContributions, out IsAdjustmentDone, true, false, false, person_account_id);
            }

            return ldecIAPBalance;

        }

        private decimal CalculateQDROIAPBenefitAmount(busQdroCalculationHeader objbusQdroCalculationHeader, int aintPayeeAccountID, DateTime adtFromDate, DateTime adtTodate, string astrFundsType, out bool IsAdjustmentDone, busQdroApplication abusQdroApplication = null, int person_account_id = 0)
        {
            Decimal ldecIAPBalance = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHours4QtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHoursA2forQtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPPercent4forQtrAlloc = busConstant.ZERO_DECIMAL;
            busCalculation lbusCalculation = new busCalculation();
            IsAdjustmentDone = false;
            #region To Set Values for IAP QTR Allocations
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            SqlParameter[] parameters = new SqlParameter[3];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
            SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);
            DateTime retirement_date = new DateTime();
            if (objbusQdroCalculationHeader.IsNotNull())
            {
                retirement_date = objbusQdroCalculationHeader.icdoQdroCalculationHeader.retirement_date;
                param1.Value = objbusQdroCalculationHeader.ibusAlternatePayee.icdoPerson.istrSSNNonEncrypted;
            }
            else
            {
                retirement_date = abusQdroApplication.icdoDroApplication.dro_commencement_date;
                param1.Value = abusQdroApplication.ibusAlternatePayee.icdoPerson.istrSSNNonEncrypted;
            }
            parameters[0] = param1;
            busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
            lbusIapAllocationSummary.LoadLatestAllocationSummary();
            param2.Value = busGlobalFunctions.GetFirstDateOfComputationYear(retirement_date.Year);
            parameters[1] = param2;
            param3.Value = busGlobalFunctions.GetLastDayOfWeek(retirement_date); //PROD PIR 113
            parameters[2] = param3;

            DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPHourInfoForQuarterlyAllocation", astrLegacyDBConnetion, null, parameters);
            if (ldtbIAPInfo.Rows.Count > 0)
            {
                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")) > 0)
                    ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")));

                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")) > 0)
                    ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")));

                DataTable ldtIAPFiltered;
                busIAPAllocationHelper aobjIAPAllocationHelper = new busIAPAllocationHelper();
                foreach (DataRow ldrIAPPercent in ldtbIAPInfo.Rows)
                {
                    if (ldrIAPPercent["IAPPercent"] != DBNull.Value && Convert.ToString(ldrIAPPercent["IAPPercent"]).IsNotNullOrEmpty() &&
                        Convert.ToDecimal(ldrIAPPercent["IAPPercent"]) > 0)
                    {
                        ldtIAPFiltered = new DataTable();
                        ldtIAPFiltered = ldtbIAPInfo.AsEnumerable().Where(o => o.Field<Int16?>("ComputationYear") == Convert.ToInt16(ldrIAPPercent["ComputationYear"])
                            && o.Field<int?>("EmpAccountNo") == Convert.ToInt32(ldrIAPPercent["EmpAccountNo"])).CopyToDataTable();

                        ldecIAPPercent4forQtrAlloc += aobjIAPAllocationHelper.CalculateAllocation4Amount(Convert.ToInt32(ldrIAPPercent["ComputationYear"]), ldtIAPFiltered);
                    }

                }
            }
            #endregion

            ldecIAPBalance = lbusCalculation.GetIAPAndSpecialAdjustmentBalanceForAlternatePayees(objbusQdroCalculationHeader, aintPayeeAccountID, adtFromDate, adtTodate, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc,
                ldecIAPPercent4forQtrAlloc, aintMaxYear, astrFundsType, out IsAdjustmentDone);


            //ldecIAPBalance = lbusCalculation.GetIAPAndSpecialAdjustmentBalance(isQdro, objbusQdroCalculationHeader.iclbQdroCalculationDetail, objbusBenefitCalculationHeader.iclbBenefitCalculationDetail, objbusBenefitCalculationHeader, objbusQdroCalculationHeader,
            //       retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, retirement_date.Year >= aintMaxYear ? aintMaxYear : retirement_date.Year, lstrFundsType, out IsAdjustmentDone);

            return ldecIAPBalance;

        }

        private void CalculateDisabilityBenefitAmountAsOnAwardedOnDate(string astrBenefitOptionValue, busBenefitCalculationHeader objbusBenefitCalculationHeader, string lstrFundsType, int person_account_id = 0)
        {
            Decimal ldecIAPBalance = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHours4QtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHoursA2forQtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPPercent4forQtrAlloc = busConstant.ZERO_DECIMAL;

            bool lblnIsAdjustmentDone = false;
            #region To Set Values for IAP QTR Allocations
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            SqlParameter[] parameters = new SqlParameter[3];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
            SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);
            DateTime retirement_date = new DateTime();
            DateTime ldtRetirementDateForPostingContributions = new DateTime();

            ldtRetirementDateForPostingContributions = objbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;

            retirement_date = objbusBenefitCalculationHeader.icdoBenefitCalculationHeader.awarded_on_date;

            param1.Value = objbusBenefitCalculationHeader.ibusPerson.icdoPerson.istrSSNNonEncrypted;
            parameters[0] = param1;
            busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
            lbusIapAllocationSummary.LoadLatestAllocationSummary();
            param2.Value = busGlobalFunctions.GetFirstDateOfComputationYear(retirement_date.Year);
            parameters[1] = param2;
            param3.Value = busGlobalFunctions.GetLastDayOfWeek(retirement_date); //PROD PIR 113
            parameters[2] = param3;

            DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPHourInfoForQuarterlyAllocation", astrLegacyDBConnetion, null, parameters);
            if (ldtbIAPInfo.Rows.Count > 0)
            {

                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")) > 0)
                    ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")));

                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")) > 0)
                    ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")));

                DataTable ldtIAPFiltered;
                busIAPAllocationHelper aobjIAPAllocationHelper = new busIAPAllocationHelper();
                foreach (DataRow ldrIAPPercent in ldtbIAPInfo.Rows)
                {
                    if (ldrIAPPercent["IAPPercent"] != DBNull.Value && Convert.ToString(ldrIAPPercent["IAPPercent"]).IsNotNullOrEmpty() &&
                        Convert.ToDecimal(ldrIAPPercent["IAPPercent"]) > 0)
                    {
                        ldtIAPFiltered = new DataTable();
                        ldtIAPFiltered = ldtbIAPInfo.AsEnumerable().Where(o => o.Field<Int16?>("ComputationYear") == Convert.ToInt16(ldrIAPPercent["ComputationYear"])
                            && o.Field<int?>("EmpAccountNo") == Convert.ToInt32(ldrIAPPercent["EmpAccountNo"])).CopyToDataTable();

                        ldecIAPPercent4forQtrAlloc += aobjIAPAllocationHelper.CalculateAllocation4Amount(Convert.ToInt32(ldrIAPPercent["ComputationYear"]), ldtIAPFiltered);
                    }

                }
            }
            #endregion
            busCalculation lbusCalculation = new busCalculation();
            if (person_account_id == 0)
            {

                ldecIAPBalance = lbusCalculation.GetIAPAndSpecialAdjustmentBalance(false, objbusBenefitCalculationHeader.iclbBenefitCalculationDetail, objbusBenefitCalculationHeader,
                      retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, retirement_date.Year >= aintMaxYear ? aintMaxYear : retirement_date.Year, lstrFundsType, ldtRetirementDateForPostingContributions, out lblnIsAdjustmentDone);
            }
            else
            {
                ldecIAPBalance = lbusCalculation.GetIAPAndSpecialAdjustmentBalance(false, objbusBenefitCalculationHeader.iclbBenefitCalculationDetail, objbusBenefitCalculationHeader,
                      retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, retirement_date.Year >= aintMaxYear ? aintMaxYear : retirement_date.Year, lstrFundsType, ldtRetirementDateForPostingContributions, out lblnIsAdjustmentDone, true, false, false, person_account_id);
            }
            if (lbusCalculation.idecIAPBalanceAsOnAwardedOnDate != 0 || lbusCalculation.idecL161BalanceAsOnAwardedOnDate != 0 || lbusCalculation.idecL152BalanceAsOnAwardedOnDate != 0)
            {
                this.idecRYIAPBalanceAsOnAwardedOnDate = lbusCalculation.idecRYIAPBalanceAsOnAwardedOnDate;
                this.idecRYAlloc4AsOnAwardedOnDate = lbusCalculation.idecRYAlloc4AsOnAwardedOnDate;
                this.idecRYAlloc2AsOnAwardedOnDate = lbusCalculation.idecRYAlloc2AsOnAwardedOnDate;
                this.idtBalanceAsOnAwardedOnDate = lbusCalculation.idtBalanceAsOnAwardedOnDate;
                this.idtAwardedOnDateEffectiveDate = lbusCalculation.idtAwardedOnDateEffectiveDate;
                this.idecIAPBalanceAsOnAwardedOnDate = lbusCalculation.idecIAPBalanceAsOnAwardedOnDate;
                this.idecL161BalanceAsOnAwardedOnDate = lbusCalculation.idecL161BalanceAsOnAwardedOnDate;
                this.idecL152BalanceAsOnAwardedOnDate = lbusCalculation.idecL152BalanceAsOnAwardedOnDate;
                this.idecQuarterlyAllocationIAPAsOnAwardedOnDate = lbusCalculation.idecQuarterlyAllocationIAPAsOnAwardedOnDate;

            }

        }

        //TusharT
        public DataTable CreateTempTableForReport()
        {
            ldtbPersonData = new DataTable();
            ldtbPersonData.Columns.Add(new DataColumn("PAYEE_ACCOUNT_ID", typeof(int)));
            ldtbPersonData.Columns.Add(new DataColumn("IAP_BALANCE", typeof(decimal)));
            return ldtbPersonData;
        }
        public DataRow FillData(busPayeeAccount lbusPayeeAccount, decimal Bal)
        {
            DataRow ldrNewRow = ldtbPersonData.NewRow();
            ldrNewRow["PAYEE_ACCOUNT_ID"] = (lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
            ldrNewRow["IAP_BALANCE"] = (Bal);

            return ldrNewRow;
        }

        private void CreateReport()
        {
            DataTable ldtNewPayee = busBase.Select("cdoPayeeAccountStatus.GetreportDataIapAdjustment", new object[0] { });
            string lstrReportPath = string.Empty;
            if (ldtNewPayee != null)
            {
                if (ldtNewPayee.Rows.Count > 0)
                {
                    //PDF Report
                    busCreateReports lobjCreateReports = new busCreateReports();
                    ldtNewPayee.TableName = "rptAdjustmentPaymentReport";
                    lstrReportPath = lobjCreateReports.CreatePDFReport(ldtNewPayee, "rptIAPAdjustmentPaymentReport");

                    //Excel Report
                    string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_IAP_ADJUSTMENT_PAYMENT + ".xlsx";
                    string lstrIapAdjustmentPaymentReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.REPORT_IAP_ADJUSTMENT_PAYMENT + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

                    DataTable adtbIapAdjustmentPayment = ldtNewPayee.Copy();
                    adtbIapAdjustmentPayment.TableName = "IapAdjustmentPayment";
                    DataSet ldsIapAdjustmentPaymentReportDataForExcel = new DataSet();
                    ldsIapAdjustmentPaymentReportDataForExcel.Tables.Add(adtbIapAdjustmentPayment);

                    busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                    lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrIapAdjustmentPaymentReportPath, "IAP Adjustment Payments", ldsIapAdjustmentPaymentReportDataForExcel);
                }
            }
        }

        #endregion
    }
}
