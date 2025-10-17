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

namespace MPIPHPJobService
{
    public class busYearEndIAPAllocationBatch : busBatchHandler
    {
        //Property to contain all IAP factor calculation methods and properties
        public busIAPYearEndFactorCalculation ibusIAPYearEndFactorCalculation { get; set; }
        //property to contain all the IAP allocation details from snap shot batch
        public DataTable idtIAPAllocationDetail { get; set; }
        public DataTable dtIAPPaybackAmount { get; set; }
        //IAP Allocation factor bus object
        public busIapAllocationFactor ibusIAPAllocationFactor { get; set; }
        //ChangeID: 59078
        public busIapOverlimitContributionsInterestDetails ibusPrevYrIapOverlimitcontributionsInterestDetails { get; set; }
        public busIapOverlimitContributionsInterestDetails ibusCurrYrIAPOverlimitContributionsInterestDetails { get; set; }

        //Collection of iap allocation details
        public Collection<busIapAllocationDetail> iclbIAPAllocationDetail { get; set; }
        //property to contain previous year iap allocation summary
        public busIapAllocationSummary ibusPrevYearAllocationSummary { get; set; }
        private object iobjLock = null;
        int iintRecordCount = 0;
        int iintTotalCount = 0;
        
    
        public override void Process()
        {
            try
            {
                iobjPassInfo.BeginTransaction();
                ibusIAPYearEndFactorCalculation = new busIAPYearEndFactorCalculation();
                LoadIAPAllocationYear();
                LoadSummaryFromAllocationDetail();
                //ChangeID: 59078
                LoadPrevYearIAPOverLimitContributionsAndInterest();

                if (ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary != null && ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows.Count > 0)
                {
                    if (ibusPrevYearAllocationSummary == null)
                        LoadPreviousYearAllocationSummary();
                    RetrieveBatchParameters();
                    CalculatePreviousYearEndBalance();
                    //ChangeID: 59078

                    LoadIAPBalanceNotElegibleForInvestmentAllocation();
                    ibusIAPYearEndFactorCalculation.CalculateIAPFactors();
                    SaveIAPAllocationSummary();
                    SaveNewAllocationFactors();
                    SaveOverLimitContributionsAndInterest();

                    iobjPassInfo.Commit();
                    CalculateAllocationsBasedOnCategory();
                    //CreateReconciliationReports();

                    //iobjPassInfo.BeginTransaction();
                    //String lstrMsg = "Get Last Year's Ending Balance";
                    //PostInfoMessage(lstrMsg);
                    //GetBeginningBalances(ibusIAPYearEndFactorCalculation.iintAllocationYear);

                    iobjPassInfo.BeginTransaction();
                    String lstrMsg = "Get Last Year's Ending Balance";
                    lstrMsg = "Get Late Hourly Contribution And Late Salary Compensation";
                    PostInfoMessage(lstrMsg);
                    UpdateLateContributionAndLateSalaryCompensation(ibusIAPYearEndFactorCalculation.iintAllocationYear);

                    lstrMsg = "Get Last Year's Ending Balance";
                    lstrMsg = "Update Ending Balances";
                    PostInfoMessage(lstrMsg);
                    UpdateEndingBalances(ibusIAPYearEndFactorCalculation.iintAllocationYear);

                    iobjPassInfo.Commit();


                    LoadIAPAllocationDetail();
                    busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary { icdoIapAllocationSummary = new cdoIapAllocationSummary() };
                    lbusIapAllocationSummary.LoadLatestAllocationSummary();
                    if (lbusIapAllocationSummary != null && lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year == ibusIAPYearEndFactorCalculation.iintAllocationYear)
                    {

                        lbusIapAllocationSummary.icdoIapAllocationSummary.allocable_beg_bal = ibusIAPYearEndFactorCalculation.idecBalanceEligibleForInvstIncomeAllocation; 

                        lbusIapAllocationSummary.icdoIapAllocationSummary.iap_grand_total = ibusIAPYearEndFactorCalculation.idecBalanceNotEligibleForInvstIncomeAllocation +
                        ibusIAPYearEndFactorCalculation.idecBalanceEligibleForInvstIncomeAllocation + ibusIAPYearEndFactorCalculation.idecInvstIncomeAmountAlloc1 + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["hourly_contribution_amount"]) +
                        ibusIAPYearEndFactorCalculation.idecInvstIncomeForHourly + ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc2 + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"]) +
                        ibusIAPYearEndFactorCalculation.idecInvstIncomeForCompensation + ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc4
                        + (-1 * Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["unallocable_amount"]));
                        
                        lbusIapAllocationSummary.icdoIapAllocationSummary.ending_balance = ibusIAPYearEndFactorCalculation.idecBalanceNotEligibleForInvstIncomeAllocation +
                        ibusIAPYearEndFactorCalculation.idecBalanceEligibleForInvstIncomeAllocation + ibusIAPYearEndFactorCalculation.idecInvstIncomeAmountAlloc1 + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["hourly_contribution_amount"]) +
                        ibusIAPYearEndFactorCalculation.idecInvstIncomeForHourly + ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc2 + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"]) +
                        ibusIAPYearEndFactorCalculation.idecInvstIncomeForCompensation + ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc4
                        + (-1 * Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["unallocable_amount"]));
                        
                    }
                    lbusIapAllocationSummary.icdoIapAllocationSummary.net_invst_income_for_all = ibusIAPYearEndFactorCalculation.idecNetInvstIncomeForAllocations;
                    lbusIapAllocationSummary.icdoIapAllocationSummary.invst_income_proration_alloc1 = ibusIAPYearEndFactorCalculation.idecInvstIncomeProRationAlloc1;
                    lbusIapAllocationSummary.icdoIapAllocationSummary.invst_income_factor_alloc1 = ibusIAPYearEndFactorCalculation.idecAlloc1Factor;
                    lbusIapAllocationSummary.icdoIapAllocationSummary.invst_income_proration_alloc2 = ibusIAPYearEndFactorCalculation.idecInvstIncomeProRationAlloc2;
                    lbusIapAllocationSummary.icdoIapAllocationSummary.invst_income_factor_alloc2 = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idecAlloc2InvstFactor);
                    lbusIapAllocationSummary.icdoIapAllocationSummary.frft_related_factor_alloc2 = ibusIAPYearEndFactorCalculation.idecAlloc2FrftFactor;
                    lbusIapAllocationSummary.icdoIapAllocationSummary.invst_income_proration_alloc4 = ibusIAPYearEndFactorCalculation.idecInvstIncomeProRationAlloc4;
                    lbusIapAllocationSummary.icdoIapAllocationSummary.invst_income_factor_alloc4 = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idecAlloc4InvstFactor);
                    lbusIapAllocationSummary.icdoIapAllocationSummary.frft_related_factor_alloc4 = ibusIAPYearEndFactorCalculation.idecAlloc4FrftFactor;
                    lbusIapAllocationSummary.icdoIapAllocationSummary.invst_income_amount_alloc2_and_alloc4 = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idecInvstIncomeAmountAlloc2And4);
                    //PIR 630
                    lbusIapAllocationSummary.icdoIapAllocationSummary.net_ending_asset_frm_accounting =
                        lbusIapAllocationSummary.icdoIapAllocationSummary.total_assets_frm_accounting + lbusIapAllocationSummary.icdoIapAllocationSummary.iap_hourly_contrb_frm_accounting
                        + lbusIapAllocationSummary.icdoIapAllocationSummary.iap_percent_compensation_frm_accounting + lbusIapAllocationSummary.icdoIapAllocationSummary.total_investment_income_frm_accounting
                        + lbusIapAllocationSummary.icdoIapAllocationSummary.administrative_expenses_frm_accounting + lbusIapAllocationSummary.icdoIapAllocationSummary.total_payouts_frm_accounting;

                    lbusIapAllocationSummary.icdoIapAllocationSummary.Update();
                    ibusIAPYearEndFactorCalculation.idecIAPGrandTotal = lbusIapAllocationSummary.icdoIapAllocationSummary.iap_grand_total;
                    CreateReconciliationReports();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                String lstrMsg = "Error while Executing Batch,Error Message: " + ex.ToString();
                PostErrorMessage(lstrMsg);
                iobjPassInfo.Rollback();
            }
        }

        //Rohan
        private void UpdateLateContributionAndLateSalaryCompensation(int aintAllocationYear)
        {
            DBFunction.DBNonQuery("cdoIapAllocationDetail.UpdateIAPHoursA2AndIAPPercent",
                new object[1] { aintAllocationYear },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            //DBFunction.DBNonQuery("cdoIapAllocationDetail.UpdateIAPHoursA2AndIAPPercentInSummary",
            //   new object[1] { aintAllocationYear },
            //                         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
        }

        //Rohan
        private void UpdateEndingBalances(int aintAllocationYear)
        {
            DBFunction.DBNonQuery("cdoIapAllocationDetail.UpdateEndingBalances",
                new object[1] { aintAllocationYear },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }


        //Rohan
        private void GetBeginningBalances(int aintAllocationYear)
        {

            DBFunction.DBNonQuery("cdoIapAllocationDetail.UpdateBeginningBalances",
               new object[0] { },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            DBFunction.DBNonQuery("cdoIapAllocationDetail.InsertMissingBeginningBalances",
               new object[1] { aintAllocationYear },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            #region Commented Code
            /*
            DataTable ldtGetBeginningBalances = busBase.Select("cdoIapAllocationDetail.GetBeginningBalances", new object[0] { });
            if (ldtGetBeginningBalances.Rows.Count > 0 && iclbIAPAllocationDetail != null && iclbIAPAllocationDetail.Count > 0)
            {

                foreach(DataRow ldr in ldtGetBeginningBalances.Rows)
                {
                    int lintPersonAccountId = 0;
                    string lstrFundType = string.Empty;
                    decimal ldecPreviousYearEndingBalance = 0M;
                    decimal ldecBeginningBalAdjs = 0M;

                    if(Convert.ToString(ldr[enmIapAllocationDetail.person_account_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                    {
                        lintPersonAccountId = Convert.ToInt32(ldr[enmIapAllocationDetail.person_account_id.ToString().ToUpper()]);
                    }

                    if(Convert.ToString(ldr[enmIapAllocationDetail.fund_type.ToString().ToUpper()]).IsNotNullOrEmpty())
                    {
                        lstrFundType = Convert.ToString(ldr[enmIapAllocationDetail.fund_type.ToString().ToUpper()]);
                    }


                    if (Convert.ToString(ldr["ENDBAL"]).IsNotNullOrEmpty())
                    {
                        ldecPreviousYearEndingBalance = Convert.ToDecimal(ldr["ENDBAL"]);
                    }


                    if (iclbIAPAllocationDetail != null && iclbIAPAllocationDetail.Where(item => item.icdoIapAllocationDetail.person_account_id == lintPersonAccountId &&
                        item.icdoIapAllocationDetail.fund_type == lstrFundType && item.icdoIapAllocationDetail.computation_year == aintAllocationYear).Count() > 0)
                    {
                        busIapAllocationDetail lbusIapAllocationDetail = new busIapAllocationDetail { icdoIapAllocationDetail = new cdoIapAllocationDetail() };
                        lbusIapAllocationDetail = iclbIAPAllocationDetail.Where(item => item.icdoIapAllocationDetail.person_account_id == lintPersonAccountId &&
                        item.icdoIapAllocationDetail.fund_type == lstrFundType).FirstOrDefault();

                        if (lbusIapAllocationDetail.icdoIapAllocationDetail.fund_type == "IAP")
                        {
                            ldecBeginningBalAdjs = ldecPreviousYearEndingBalance - lbusIapAllocationDetail.icdoIapAllocationDetail.system_beginning_balance;
                        }
                        else if (lbusIapAllocationDetail.icdoIapAllocationDetail.fund_type == "L052")
                        {
                            ldecBeginningBalAdjs = ldecPreviousYearEndingBalance - lbusIapAllocationDetail.icdoIapAllocationDetail.l52_special_account_amount;
                        }
                        else
                        {
                            ldecBeginningBalAdjs = ldecPreviousYearEndingBalance - lbusIapAllocationDetail.icdoIapAllocationDetail.l161_special_account_amount;
                        }

                        lbusIapAllocationDetail.icdoIapAllocationDetail.current_yr_beginning_balance = ldecPreviousYearEndingBalance;
                        lbusIapAllocationDetail.icdoIapAllocationDetail.beginning_bal_adjustment = ldecBeginningBalAdjs;

                        lbusIapAllocationDetail.icdoIapAllocationDetail.Update();

                    }
                    else
                    {
                        busIapAllocationDetail lbusIapAllocationDetail = new busIapAllocationDetail { icdoIapAllocationDetail = new cdoIapAllocationDetail() };
                        lbusIapAllocationDetail.icdoIapAllocationDetail.person_account_id = lintPersonAccountId;
                        lbusIapAllocationDetail.icdoIapAllocationDetail.computation_year = aintAllocationYear;
                        lbusIapAllocationDetail.icdoIapAllocationDetail.fund_type = lstrFundType;
                        lbusIapAllocationDetail.icdoIapAllocationDetail.current_yr_beginning_balance = ldecPreviousYearEndingBalance;
                        lbusIapAllocationDetail.icdoIapAllocationDetail.beginning_bal_adjustment = ldecPreviousYearEndingBalance;
                        lbusIapAllocationDetail.icdoIapAllocationDetail.Insert();

                    }
                    
                }
            }
              */
            #endregion Commented Code
        }


        /// <summary>
        /// Method to retrieve Job Parameters and assign to appropriate properties
        /// </summary>
        /// 
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
                            case busConstant.JobParamTotalInvstAmtFrmAccounting:
                                ibusIAPYearEndFactorCalculation.idecTotalInvstIncome = Convert.ToDecimal(lobjParam.icdoJobParameters.param_value);
                                break;
                            case busConstant.JobParamAdmExpFrmAccounting:
                                ibusIAPYearEndFactorCalculation.idecAdministrativeExpenses = Convert.ToDecimal(lobjParam.icdoJobParameters.param_value); //* -1;
                                break;
                            case busConstant.JobParamWeightedAvgFrmAccounting:
                                ibusIAPYearEndFactorCalculation.idecWeightedAverage = Convert.ToDecimal(lobjParam.icdoJobParameters.param_value);
                                break;
                            case busConstant.JobParamTotalAssetsFrmAccounting:
                                ibusIAPYearEndFactorCalculation.idecTotalAssetsFromAccounting = Convert.ToDecimal(lobjParam.icdoJobParameters.param_value);
                                break;
                            case busConstant.JobParamIAPHourlyFrmAccounting:
                                ibusIAPYearEndFactorCalculation.idecIAPHourlyFromAccounting = Convert.ToDecimal(lobjParam.icdoJobParameters.param_value);
                                break;
                            case busConstant.JobParamIAPPercentFrmAccounting:
                                ibusIAPYearEndFactorCalculation.idecIAPPercentFromAccounting = Convert.ToDecimal(lobjParam.icdoJobParameters.param_value);
                                break;
                            case busConstant.JobParamUnallocableOverlimitAmtFrmAccounting://PIR 630
                                ibusIAPYearEndFactorCalculation.idecUnallocableOverlimitAmtFrmAccounting = Convert.ToDecimal(lobjParam.icdoJobParameters.param_value);// * -1;
                                break;
                            case busConstant.JobParamMiscAdjustemntsFrmAccounting://PIR 630
                                ibusIAPYearEndFactorCalculation.idecMiscAdjustmentsFrmAccounting = Convert.ToDecimal(lobjParam.icdoJobParameters.param_value);// * -1;
                                break;
                            case busConstant.JobParamPayoutsFrmAccounting://PIR 630
                                ibusIAPYearEndFactorCalculation.idecPayoutsFrmAccounting = Convert.ToDecimal(lobjParam.icdoJobParameters.param_value);// * -1;
                                break;

                            case busConstant.JobParamOverlimitInvIncomeOrLossFactor://ChangeID: 59078
                                ibusIAPYearEndFactorCalculation.idecOverlimitInvIncomeOrLossFactor = Convert.ToDecimal(lobjParam.icdoJobParameters.param_value);
                                break;

                            case busConstant.JobParamOverlimitInterest://ChangeID: 59078
                                ibusIAPYearEndFactorCalculation.idecOverlimitInterest = Convert.ToDecimal(lobjParam.icdoJobParameters.param_value);
                                break;


                        }
                    }
                }
            }
        }

        private void LoadIAPBalanceNotElegibleForInvestmentAllocation()
        {
            DataTable ldtblRetirementyear = busBase.Select("cdoIapAllocationDetail.GetRetiredInStatementYearRU65Balance", new object[1] { ibusIAPYearEndFactorCalculation.iintAllocationYear });
            if (ldtblRetirementyear != null && ldtblRetirementyear.Rows.Count > 0)
            {
                ibusIAPYearEndFactorCalculation.idecRetiredInStatementYearRU65Balance = Convert.ToDecimal(ldtblRetirementyear.Rows[0][0]);
            }
            DataTable ldtIAPPaybackAmount = busBase.Select("cdoIapAllocationDetail.GetIAPPaybackSummary", new object[1] { ibusIAPYearEndFactorCalculation.iintAllocationYear });
            if (ldtIAPPaybackAmount != null && ldtIAPPaybackAmount.Rows.Count > 0)
            {
                ibusIAPYearEndFactorCalculation.idecIAPPaybackSummary = Convert.ToDecimal(ldtIAPPaybackAmount.Rows[0][0]);
            }
            ibusIAPYearEndFactorCalculation.idecBalanceNotEligibleForInvstIncomeAllocation = ibusIAPYearEndFactorCalculation.idecRetiredInStatementYearRU65Balance + ibusIAPYearEndFactorCalculation.idecIAPPaybackSummary;
        }

        /// <summary>
        /// Method to calculate Previous Year ending balance
        /// </summary>
        private void CalculatePreviousYearEndBalance()
        {
            //PIR 630 Rohan
            ibusIAPYearEndFactorCalculation.idecPrevYearEndBalance = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.previous_year_ending_balance + ibusPrevYearAllocationSummary.icdoIapAllocationSummary.opening_balance_adjustments +
                    ibusPrevYearAllocationSummary.icdoIapAllocationSummary.forfeited_balance + ibusPrevYearAllocationSummary.icdoIapAllocationSummary.late_eligible_hourly_contribution_amount +
                    ibusPrevYearAllocationSummary.icdoIapAllocationSummary.late_eligible_compensation_amount +
                    ibusPrevYearAllocationSummary.icdoIapAllocationSummary.late_allocations_amount +
                    ibusPrevYearAllocationSummary.icdoIapAllocationSummary.quaterly_allocations_amount + ibusPrevYearAllocationSummary.icdoIapAllocationSummary.retirement_year_allocation2_amount +
                    ibusPrevYearAllocationSummary.icdoIapAllocationSummary.retirement_year_allocation4_amount + ibusPrevYearAllocationSummary.icdoIapAllocationSummary.payouts +
                    ibusPrevYearAllocationSummary.icdoIapAllocationSummary.invst_income_allocation1 +
                    ibusPrevYearAllocationSummary.icdoIapAllocationSummary.hourly_contribution_amount + ibusPrevYearAllocationSummary.icdoIapAllocationSummary.invst_income_allocation2 +
                    ibusPrevYearAllocationSummary.icdoIapAllocationSummary.late_ineligible_hourly_contribution_amount + ibusPrevYearAllocationSummary.icdoIapAllocationSummary.current_year_ineligible_contribution_amount +
                    ibusPrevYearAllocationSummary.icdoIapAllocationSummary.percentage_of_compensation_amount +
                    ibusPrevYearAllocationSummary.icdoIapAllocationSummary.invst_income_allocation4 + ibusPrevYearAllocationSummary.icdoIapAllocationSummary.late_inelgibile_compensation_amount +
                    ibusPrevYearAllocationSummary.icdoIapAllocationSummary.current_year_inelgibile_compensation_amount; //+ ibusPrevYearAllocationSummary.icdoIapAllocationSummary.overlimit_contributions_amount;//PIR 630 Rohan
        }

        /// <summary>
        /// Method to retrieve Current allocation Year
        /// </summary>
        private void LoadIAPAllocationYear()
        {
            if (ibusPrevYearAllocationSummary == null)
                LoadPreviousYearAllocationSummary();
            ibusIAPYearEndFactorCalculation.iintAllocationYear = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.computation_year + 1;
        }

        /// <summary>
        /// Method to get the summay from allocation details
        /// </summary>
        private void LoadSummaryFromAllocationDetail()
        {
            ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary = busBase.Select("cdoIapAllocationDetail.LoadSummaryForYearEndAllocation", new object[1] { ibusIAPYearEndFactorCalculation.iintAllocationYear });
        }

        /// <summary>
        /// Method to load the IAP allocation factor
        /// </summary>
        private void LoadIAPAllocationFactor()
        {
            ibusIAPAllocationFactor = new busIapAllocationFactor();
            ibusIAPAllocationFactor.LoadIAPAllocationFactorByPlanYear(ibusIAPYearEndFactorCalculation.iintAllocationYear);
        }
        //ChangeID: 59078
        private void LoadPrevYearIAPOverLimitContributionsAndInterest()
        {
            ibusPrevYrIapOverlimitcontributionsInterestDetails = new busIapOverlimitContributionsInterestDetails();
            ibusPrevYrIapOverlimitcontributionsInterestDetails.LoadIAPOverLimitContributionsInterestDetails(ibusIAPYearEndFactorCalculation.iintAllocationYear - 1); // 1 Constant is used to verify the ending balance of Overlimit + limit for the previous year. It will
                                                                                                                                                               //help to compute the final total Overlimit and interest value.         
        }

        /// <summary>
        /// Method to save the current year allocation summary
        /// </summary>
        private void SaveIAPAllocationSummary()
        {
            if (ibusPrevYearAllocationSummary == null)
                LoadPreviousYearAllocationSummary();

            cdoIapAllocationSummary lcdoIAPAllocationSummary = new cdoIapAllocationSummary();
            lcdoIAPAllocationSummary.computation_year = ibusIAPYearEndFactorCalculation.iintAllocationYear;
            lcdoIAPAllocationSummary.total_investment_income_frm_accounting = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idecTotalInvstIncome);
            lcdoIAPAllocationSummary.administrative_expenses_frm_accounting = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idecAdministrativeExpenses);
            lcdoIAPAllocationSummary.weighted_average_frm_accounting = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idecWeightedAverage);
            lcdoIAPAllocationSummary.iap_hourly_contrb_frm_accounting = ibusIAPYearEndFactorCalculation.idecIAPHourlyFromAccounting;
            lcdoIAPAllocationSummary.iap_percent_compensation_frm_accounting = ibusIAPYearEndFactorCalculation.idecIAPPercentFromAccounting;
            lcdoIAPAllocationSummary.previous_year_ending_balance = ibusIAPYearEndFactorCalculation.idecPrevYearEndBalance;
            lcdoIAPAllocationSummary.system_beginning_balance = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["system_beginning_balance"]);
            lcdoIAPAllocationSummary.opening_balance_adjustments = ibusIAPYearEndFactorCalculation.idecOpeningBalanceAdjustment;
            lcdoIAPAllocationSummary.total_assets_frm_accounting = ibusIAPYearEndFactorCalculation.idecTotalAssetsFromAccounting;

            lcdoIAPAllocationSummary.total_unallocable_overlimit_amt = ibusIAPYearEndFactorCalculation.idecUnallocableOverlimitAmtFrmAccounting;//PIR 630
            lcdoIAPAllocationSummary.misc_adjustments_frm_accounting = ibusIAPYearEndFactorCalculation.idecMiscAdjustmentsFrmAccounting;//PIR 630
            lcdoIAPAllocationSummary.total_payouts_frm_accounting = ibusIAPYearEndFactorCalculation.idecPayoutsFrmAccounting;//PIR 630


            //rohan
            lcdoIAPAllocationSummary.late_allocations_amount = (Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc1_amount"]) + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc2_amount"]) +
                Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc3_amount"]) + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc4_amount"]) +
                Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc5_amount"])
                - (Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_eligible_hourly_contribution_amount"]) +
                Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_eligible_compensation_amount"])));

            lcdoIAPAllocationSummary.late_eligible_hourly_contribution_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_eligible_hourly_contribution_amount"]);
            lcdoIAPAllocationSummary.late_eligible_compensation_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_eligible_compensation_amount"]);

            lcdoIAPAllocationSummary.forfeited_balance = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["forfeited_balance"]);
            lcdoIAPAllocationSummary.quaterly_allocations_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["quaterly_allocations_amount"]);
            lcdoIAPAllocationSummary.retirement_year_allocation2_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["retirement_year_allocation2_amount"]);
            lcdoIAPAllocationSummary.retirement_year_allocation4_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["retirement_year_allocation4_amount"]);
            lcdoIAPAllocationSummary.payouts = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["payouts"]);
            lcdoIAPAllocationSummary.unallocable_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["unallocable_amount"]);
            lcdoIAPAllocationSummary.late_ineligible_hours = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_ineligible_hours"]);
            lcdoIAPAllocationSummary.late_ineligible_hourly_contribution_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_ineligible_hourly_contribution_amount"]);
            lcdoIAPAllocationSummary.current_year_ineligible_hours = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["current_year_ineligible_hours"]);
            lcdoIAPAllocationSummary.current_year_ineligible_contribution_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["current_year_ineligible_contribution_amount"]);
            lcdoIAPAllocationSummary.eligible_hours = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["eligible_hours"]);
            lcdoIAPAllocationSummary.hourly_contribution_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["hourly_contribution_amount"]);
            lcdoIAPAllocationSummary.late_inelgibile_compensation_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_inelgibile_compensation_amount"]);
            lcdoIAPAllocationSummary.current_year_inelgibile_compensation_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["current_year_inelgibile_compensation_amount"]);
            lcdoIAPAllocationSummary.overlimit_contributions_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["overlimit_contributions_amount"]);
            lcdoIAPAllocationSummary.percentage_of_compensation_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"]);
            lcdoIAPAllocationSummary.invst_income_allocation1 = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idecInvstIncomeAmountAlloc1);
            lcdoIAPAllocationSummary.invst_income_allocation2 = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idecInvstIncomeForHourly);
            lcdoIAPAllocationSummary.invst_income_allocation4 = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idecInvstIncomeForCompensation);

            lcdoIAPAllocationSummary.Insert();
        }

        private void LoadPreviousYearAllocationSummary()
        {
            ibusPrevYearAllocationSummary = new busIapAllocationSummary();
            ibusPrevYearAllocationSummary.LoadLatestAllocationSummary();
        }

        /// <summary>
        /// Method to save the current year calculated iap allocation factors
        /// </summary>
        private void SaveNewAllocationFactors()
        {
            if (ibusIAPAllocationFactor == null)
                LoadIAPAllocationFactor();

            ibusIAPAllocationFactor.icdoIapAllocationFactor.alloc1_qf4_factor = ibusIAPYearEndFactorCalculation.idecAlloc1Factor;
            ibusIAPAllocationFactor.icdoIapAllocationFactor.alloc2_factor = Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPAllocation2Factor));
            ibusIAPAllocationFactor.icdoIapAllocationFactor.alloc2_invst_factor = ibusIAPYearEndFactorCalculation.idecAlloc2InvstFactor;
            ibusIAPAllocationFactor.icdoIapAllocationFactor.alloc2_frft_factor = ibusIAPYearEndFactorCalculation.idecAlloc2FrftFactor;
            ibusIAPAllocationFactor.icdoIapAllocationFactor.alloc4_invst_factor = ibusIAPYearEndFactorCalculation.idecAlloc4InvstFactor;
            ibusIAPAllocationFactor.icdoIapAllocationFactor.alloc4_frft_factor = ibusIAPYearEndFactorCalculation.idecAlloc4FrftFactor;

            if (ibusIAPAllocationFactor.icdoIapAllocationFactor.iap_allocation_factor_id > 0)
                ibusIAPAllocationFactor.icdoIapAllocationFactor.Update();
            else
            {
                ibusIAPAllocationFactor.icdoIapAllocationFactor.plan_year = ibusIAPYearEndFactorCalculation.iintAllocationYear;
                ibusIAPAllocationFactor.icdoIapAllocationFactor.Insert();
            }
        }
        //ChangeID: 59078
        private void SaveOverLimitContributionsAndInterest()
        {
            ibusCurrYrIAPOverlimitContributionsInterestDetails = 
                new busIapOverlimitContributionsInterestDetails {icdoIapOverlimitContributionsInterestDetails = new cdoIapOverlimitContributionsInterestDetails()};
            if(ibusPrevYrIapOverlimitcontributionsInterestDetails != null)
            {
                ibusCurrYrIAPOverlimitContributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.computation_year = ibusIAPYearEndFactorCalculation.iintAllocationYear;
                ibusCurrYrIAPOverlimitContributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.overlimit_amount = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["OVERLIMIT_CONTRIBUTIONS_AMOUNT"]);
                ibusCurrYrIAPOverlimitContributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.alloc1_factor = ibusIAPYearEndFactorCalculation.idecOverlimitInvIncomeOrLossFactor;
                ibusCurrYrIAPOverlimitContributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.interest = ibusIAPYearEndFactorCalculation.idecOverlimitInterest * -1;
                ibusCurrYrIAPOverlimitContributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.total_overlimit_contributions_interest_amount = ibusPrevYrIapOverlimitcontributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.total_overlimit_contributions_interest_amount + ibusCurrYrIAPOverlimitContributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.interest + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["overlimit_contributions_amount"]);
                ibusCurrYrIAPOverlimitContributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.Insert();
            }
        }
        /// <summary>
        /// Method to iterate through iap allocation details and calculate the eligible allocations
        /// </summary>
        private void CalculateAllocationsBasedOnCategory()
        {
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            iobjLock = new object();

            if (iclbIAPAllocationDetail == null)
                LoadIAPAllocationDetail();

            decimal ldecIAPAccountBalance = 0.00M;
            int lintPersonAccountID = 0;
            busIAPAllocationHelper lobjIAPHelper = new busIAPAllocationHelper();
            lobjIAPHelper.LoadIAPAllocationFactor();

            dtIAPPaybackAmount = busBase.Select("cdoPaymentHistoryHeader.GetIAPPayback", new object[1] { ibusIAPYearEndFactorCalculation.iintAllocationYear });

            ParallelOptions p = new ParallelOptions();
            p.MaxDegreeOfParallelism = 1;//System.Environment.ProcessorCount * 4;
            Parallel.ForEach(iclbIAPAllocationDetail, p, (lobjAllocationDetail, loopstate) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "IAPYearEndAllocation-Batch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;

                lock (iobjLock)
                {
                    iintRecordCount++;
                    iintTotalCount++;
                    if (iintRecordCount == 100)
                    {
                        String lstrMsg = iintTotalCount + " : " + " Records Has Been Processed";
                        PostInfoMessage(lstrMsg);
                        iintRecordCount = 0;
                    }
                }

                lobjPassInfo.BeginTransaction();
                try
                {
                    decimal payback = 0;
                    DataRow[] drIAPPaybackAmount = dtIAPPaybackAmount.Select("PERSON_ACCOUNT_ID = " + lobjAllocationDetail.icdoIapAllocationDetail.person_account_id.ToString());
                    if (drIAPPaybackAmount != null && drIAPPaybackAmount.Length > 0)
                        payback = drIAPPaybackAmount[0]["PAYBACK"] == DBNull.Value ? 0.0M : Convert.ToDecimal(drIAPPaybackAmount[0]["PAYBACK"]);

                    lintPersonAccountID = lobjAllocationDetail.icdoIapAllocationDetail.person_account_id;
                    ldecIAPAccountBalance = lobjAllocationDetail.icdoIapAllocationDetail.system_beginning_balance +
                                lobjAllocationDetail.icdoIapAllocationDetail.l52_special_account_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.l161_special_account_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.forfeited_balance +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc1_amount + lobjAllocationDetail.icdoIapAllocationDetail.late_alloc2_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc3_amount + lobjAllocationDetail.icdoIapAllocationDetail.late_alloc4_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc5_amount + lobjAllocationDetail.icdoIapAllocationDetail.quaterly_allocations_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.retirement_year_allocation2_amount + lobjAllocationDetail.icdoIapAllocationDetail.retirement_year_allocation4_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.payouts + lobjAllocationDetail.icdoIapAllocationDetail.unallocable_amount - payback;

                    //Rohan
                    if (ldecIAPAccountBalance < 0)
                        ldecIAPAccountBalance = 0;

                    switch (lobjAllocationDetail.icdoIapAllocationDetail.iap_allocation_category_value)
                    {
                        case busConstant.IAPAllocationCategoryActive:
                            //calculate allocation 1 for iap and special accounts if there is system beg. balance
                            CalculateAllocation1(ldecIAPAccountBalance, lobjIAPHelper, lobjAllocationDetail);
                            CalculateAllocation2And4(lobjIAPHelper, lobjAllocationDetail);
                            lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(124)";
                            break;
                        case busConstant.IAPAllocationCategoryNewParticipants:
                            //calculate allocation 1 for iap and special accounts if there is system beg. balance
                            CalculateAllocation1(ldecIAPAccountBalance, lobjIAPHelper, lobjAllocationDetail);
                            CalculateAllocation2And4(lobjIAPHelper, lobjAllocationDetail);
                            lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(124)";
                            break;
                        //Rohan
                        case busConstant.IAPAllocationCategoryInActiveZeroBalance:
                            //calculate allocation 1 for iap and special accounts
                            CalculateAllocation1(ldecIAPAccountBalance, lobjIAPHelper, lobjAllocationDetail);
                            lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(1)";

                            break;
                        case busConstant.IAPAllocationCategoryNonVstdBIS:
                            //calculate allocation 1 for iap and special accounts
                            CalculateAllocation1(ldecIAPAccountBalance, lobjIAPHelper, lobjAllocationDetail);
                            lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(1)";
                            break;
                        case busConstant.IAPAllocationCategoryVstBIS:
                            //calculate allocation 1 for iap and special accounts
                            CalculateAllocation1(ldecIAPAccountBalance, lobjIAPHelper, lobjAllocationDetail);
                            lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(1)";
                            break;
                        case busConstant.IAPAllocationCategoryActiveDeath:
                            //calculate allocation 1 for iap and special accounts if there is system beg. balance
                            CalculateAllocation1(ldecIAPAccountBalance, lobjIAPHelper, lobjAllocationDetail);
                            CalculateAllocation2And4(lobjIAPHelper, lobjAllocationDetail);
                            lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(124)";
                            break;
                        case busConstant.IAPAllocationCategoryEarlyWithdrawal:
                            //To calculate allocation 2 & 4 based on the hours after withdrawal date
                            CalculateAllocation2And4(lobjIAPHelper, lobjAllocationDetail);
                            lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(24)";
                            break;
                        case busConstant.IAPAllocationCategoryMDActiveReeval:
                            //calculate allocation 1 for iap and special accounts if there is system beg. balance
                            CalculateAllocation1(ldecIAPAccountBalance, lobjIAPHelper, lobjAllocationDetail);
                            CalculateAllocation2And4(lobjIAPHelper, lobjAllocationDetail);
                            lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(124)";
                            break;
                        case busConstant.IAPAllocationCategoryMDNew:
                            //calculate allocation 1 for iap and special accounts if there is system beg. balance
                            CalculateAllocation1(ldecIAPAccountBalance, lobjIAPHelper, lobjAllocationDetail);
                            CalculateAllocation2And4(lobjIAPHelper, lobjAllocationDetail);
                            lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(124)";
                            break;
                        case busConstant.IAPAllocationCategoryQDROActive:
                            CalculateAllocation1(ldecIAPAccountBalance, lobjIAPHelper, lobjAllocationDetail);
                            CalculateAllocation2And4(lobjIAPHelper, lobjAllocationDetail);
                            lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(124)";
                            break;
                        case busConstant.IAPAllocationCategoryReempOver65:
                            if (lobjAllocationDetail.icdoIapAllocationDetail.total_iap_hours >= 870)
                            {
                                CalculateAllocation2And4(lobjIAPHelper, lobjAllocationDetail);
                                lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(24)";
                            }
                            break;

                        case busConstant.IAPAllocationCategoryReempUnder65:

                            //rohan -- Change code next year after adding new fields
                            bool lblnflag = true;
                            if (lobjAllocationDetail.icdoIapAllocationDetail.fund_type == "IAP")
                            {
                                DataTable ldtblRetirementyear = busBase.Select("cdoIapAllocationDetail.CheckRetiremntYearRU65", new object[1] { lobjAllocationDetail.icdoIapAllocationDetail.person_account_id });

                                if (ldtblRetirementyear != null && ldtblRetirementyear.Rows.Count > 0 &&
                                    Convert.ToString(ldtblRetirementyear.Rows[0][0]).IsNotNullOrEmpty())
                                {
                                    if (Convert.ToInt32(ldtblRetirementyear.Rows[0][0]) == lobjAllocationDetail.icdoIapAllocationDetail.computation_year)
                                        lblnflag = false;
                                }
                            }

                            if (lblnflag)
                                CalculateAllocation1(ldecIAPAccountBalance, lobjIAPHelper, lobjAllocationDetail);

                            if (lobjAllocationDetail.icdoIapAllocationDetail.total_iap_hours >= 870)
                            {
                                CalculateAllocation2And4(lobjIAPHelper, lobjAllocationDetail);
                            }
                            lobjAllocationDetail.icdoIapAllocationDetail.alloctype_code = "(124)";
                            break;
                    }


                    //PIR 630
                    if (lobjAllocationDetail.icdoIapAllocationDetail.fund_type == "IAP")
                    {
                        lobjAllocationDetail.icdoIapAllocationDetail.allocable_beg_bal =
                            lobjAllocationDetail.icdoIapAllocationDetail.system_beginning_balance + lobjAllocationDetail.icdoIapAllocationDetail.forfeited_balance +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc1_amount + lobjAllocationDetail.icdoIapAllocationDetail.late_alloc2_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc3_amount + lobjAllocationDetail.icdoIapAllocationDetail.late_alloc4_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc5_amount + lobjAllocationDetail.icdoIapAllocationDetail.quaterly_allocations_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.retirement_year_allocation2_amount + lobjAllocationDetail.icdoIapAllocationDetail.retirement_year_allocation4_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.payouts + lobjAllocationDetail.icdoIapAllocationDetail.unallocable_amount - payback;

                        lobjAllocationDetail.icdoIapAllocationDetail.allocable_ending_balance = lobjAllocationDetail.icdoIapAllocationDetail.allocable_beg_bal + lobjAllocationDetail.icdoIapAllocationDetail.allocation1_amount
                            + lobjAllocationDetail.icdoIapAllocationDetail.allocation2_amount + lobjAllocationDetail.icdoIapAllocationDetail.allocation2_frft_amount
                            + lobjAllocationDetail.icdoIapAllocationDetail.allocation2_invst_amount + lobjAllocationDetail.icdoIapAllocationDetail.allocation4_amount
                            + lobjAllocationDetail.icdoIapAllocationDetail.allocation4_frft_amount + lobjAllocationDetail.icdoIapAllocationDetail.allocation4_invst_amount + payback;

                        if (lobjAllocationDetail.icdoIapAllocationDetail.iap_allocation_category_value == busConstant.IAPAllocationCategoryReempUnder65)
                        {
                            DataTable ldtblRetirementyear = busBase.Select("cdoIapAllocationDetail.CheckRetiremntYearRU65", new object[1] { lobjAllocationDetail.icdoIapAllocationDetail.person_account_id });

                            if (ldtblRetirementyear != null && ldtblRetirementyear.Rows.Count > 0 &&
                                Convert.ToString(ldtblRetirementyear.Rows[0][0]).IsNotNullOrEmpty())
                            {
                                if (Convert.ToInt32(ldtblRetirementyear.Rows[0][0]) == lobjAllocationDetail.icdoIapAllocationDetail.computation_year)
                                {
                                    lobjAllocationDetail.icdoIapAllocationDetail.allocable_beg_bal = 0;
                                    lobjAllocationDetail.icdoIapAllocationDetail.allocable_ending_balance =
                                        lobjAllocationDetail.icdoIapAllocationDetail.system_beginning_balance + lobjAllocationDetail.icdoIapAllocationDetail.forfeited_balance +
                                            lobjAllocationDetail.icdoIapAllocationDetail.late_alloc1_amount + lobjAllocationDetail.icdoIapAllocationDetail.late_alloc2_amount +
                                            lobjAllocationDetail.icdoIapAllocationDetail.late_alloc3_amount + lobjAllocationDetail.icdoIapAllocationDetail.late_alloc4_amount +
                                            lobjAllocationDetail.icdoIapAllocationDetail.late_alloc5_amount + lobjAllocationDetail.icdoIapAllocationDetail.quaterly_allocations_amount +
                                            lobjAllocationDetail.icdoIapAllocationDetail.retirement_year_allocation2_amount + lobjAllocationDetail.icdoIapAllocationDetail.retirement_year_allocation4_amount +
                                            lobjAllocationDetail.icdoIapAllocationDetail.payouts + lobjAllocationDetail.icdoIapAllocationDetail.unallocable_amount 
                                        + lobjAllocationDetail.icdoIapAllocationDetail.allocation1_amount
                                        + lobjAllocationDetail.icdoIapAllocationDetail.allocation2_amount + lobjAllocationDetail.icdoIapAllocationDetail.allocation2_frft_amount
                                        + lobjAllocationDetail.icdoIapAllocationDetail.allocation2_invst_amount + lobjAllocationDetail.icdoIapAllocationDetail.allocation4_amount
                                        + lobjAllocationDetail.icdoIapAllocationDetail.allocation4_frft_amount + lobjAllocationDetail.icdoIapAllocationDetail.allocation4_invst_amount;
                                }
                            }
                        }
                    }
                    else if (lobjAllocationDetail.icdoIapAllocationDetail.fund_type == "L052")
                    {
                        lobjAllocationDetail.icdoIapAllocationDetail.allocable_beg_bal =
                            lobjAllocationDetail.icdoIapAllocationDetail.l52_special_account_amount + lobjAllocationDetail.icdoIapAllocationDetail.forfeited_balance +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc1_amount + lobjAllocationDetail.icdoIapAllocationDetail.late_alloc2_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc3_amount + lobjAllocationDetail.icdoIapAllocationDetail.late_alloc4_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc5_amount + lobjAllocationDetail.icdoIapAllocationDetail.quaterly_allocations_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.retirement_year_allocation2_amount + lobjAllocationDetail.icdoIapAllocationDetail.retirement_year_allocation4_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.payouts + lobjAllocationDetail.icdoIapAllocationDetail.unallocable_amount;


                        lobjAllocationDetail.icdoIapAllocationDetail.allocable_ending_balance = lobjAllocationDetail.icdoIapAllocationDetail.allocable_beg_bal
                            + lobjAllocationDetail.icdoIapAllocationDetail.l52_allocation1_amount;
                    }
                    else if (lobjAllocationDetail.icdoIapAllocationDetail.fund_type == "L161")
                    {
                        lobjAllocationDetail.icdoIapAllocationDetail.allocable_beg_bal =
                            lobjAllocationDetail.icdoIapAllocationDetail.l161_special_account_amount + lobjAllocationDetail.icdoIapAllocationDetail.forfeited_balance +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc1_amount + lobjAllocationDetail.icdoIapAllocationDetail.late_alloc2_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc3_amount + lobjAllocationDetail.icdoIapAllocationDetail.late_alloc4_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.late_alloc5_amount + lobjAllocationDetail.icdoIapAllocationDetail.quaterly_allocations_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.retirement_year_allocation2_amount + lobjAllocationDetail.icdoIapAllocationDetail.retirement_year_allocation4_amount +
                                lobjAllocationDetail.icdoIapAllocationDetail.payouts + lobjAllocationDetail.icdoIapAllocationDetail.unallocable_amount;


                        lobjAllocationDetail.icdoIapAllocationDetail.allocable_ending_balance = lobjAllocationDetail.icdoIapAllocationDetail.allocable_beg_bal
                            + lobjAllocationDetail.icdoIapAllocationDetail.l161_allocation1_amount;
                    }



                    lobjAllocationDetail.icdoIapAllocationDetail.Update();
                    lobjPassInfo.Commit();
                }
                catch (Exception ex)
                {
                    lock (iobjLock)
                    {
                        ExceptionManager.Publish(ex);
                        String lstrMsg = "Error while calcualting Allocation for PersonAccount ID " + lintPersonAccountID.ToString() + " : " + ex.ToString();
                        PostErrorMessage(lstrMsg);
                    }
                    lobjPassInfo.Rollback();
                    throw ex;
                }
                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;
            });
            lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        }

        /// <summary>
        /// Method to load iap allocation details
        /// </summary>
        private void LoadIAPAllocationDetail()
        {
            DataTable ldtIAPAllocationDetail = busBase.Select<cdoIapAllocationDetail>(new string[1] { enmIapAllocationDetail.computation_year.ToString() }, new object[1] { ibusIAPYearEndFactorCalculation.iintAllocationYear }, null, null);
            busBase lobjBase = new busBase();
            iclbIAPAllocationDetail = lobjBase.GetCollection<busIapAllocationDetail>(ldtIAPAllocationDetail, "icdoIapAllocationDetail");
        }

        /// <summary>
        /// Method to calculate allocation 1 for iap and special account
        /// </summary>
        /// <param name="adecIAPAccountBalance">IAP Account Balance</param>
        /// <param name="abjIAPHelper">IAP Helper class</param>
        /// <param name="aobjAllocationDetail">IAP Allocation detail bus. object</param>
        private void CalculateAllocation1(decimal adecIAPAccountBalance, busIAPAllocationHelper abjIAPHelper, busIapAllocationDetail aobjAllocationDetail)
        {
            decimal ldecFactor = 0;
            if (aobjAllocationDetail.icdoIapAllocationDetail.fund_type == "IAP")
                aobjAllocationDetail.icdoIapAllocationDetail.allocation1_amount = abjIAPHelper.CalculateAllocation1Amount(ibusIAPYearEndFactorCalculation.iintAllocationYear, adecIAPAccountBalance, 4, ref ldecFactor);
            else if (aobjAllocationDetail.icdoIapAllocationDetail.fund_type == "L052")
                aobjAllocationDetail.icdoIapAllocationDetail.l52_allocation1_amount = abjIAPHelper.CalculateAllocation1Amount(ibusIAPYearEndFactorCalculation.iintAllocationYear, adecIAPAccountBalance, 4, ref ldecFactor);
            else if (aobjAllocationDetail.icdoIapAllocationDetail.fund_type == "L161")
                aobjAllocationDetail.icdoIapAllocationDetail.l161_allocation1_amount = abjIAPHelper.CalculateAllocation1Amount(ibusIAPYearEndFactorCalculation.iintAllocationYear, adecIAPAccountBalance, 4, ref ldecFactor);
        }

        private void CalculateAllocation2And4(busIAPAllocationHelper aobjIAPHelper, busIapAllocationDetail aobjAllocationDetail)
        {
            if (aobjAllocationDetail.icdoIapAllocationDetail.total_iap_hours >= 400)
            {
                aobjAllocationDetail.icdoIapAllocationDetail.allocation2_amount = aobjAllocationDetail.icdoIapAllocationDetail.hourly_contribution_amount;
                aobjAllocationDetail.icdoIapAllocationDetail.allocation2_invst_amount = aobjIAPHelper.CalculateAllocation2InvstOrFrftAmount(ibusIAPYearEndFactorCalculation.iintAllocationYear, aobjAllocationDetail.icdoIapAllocationDetail.eligible_hours,
                    DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationInvestmentFlag);
                aobjAllocationDetail.icdoIapAllocationDetail.allocation2_frft_amount = aobjIAPHelper.CalculateAllocation2InvstOrFrftAmount(ibusIAPYearEndFactorCalculation.iintAllocationYear, aobjAllocationDetail.icdoIapAllocationDetail.eligible_hours,
                    DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationForfeitureFlag);
                aobjAllocationDetail.icdoIapAllocationDetail.allocation4_amount = aobjAllocationDetail.icdoIapAllocationDetail.percentage_of_compensation_amount;
                aobjAllocationDetail.icdoIapAllocationDetail.allocation4_invst_amount = aobjIAPHelper.CalculateAllocation4InvstOrFrftAmount(ibusIAPYearEndFactorCalculation.iintAllocationYear, aobjAllocationDetail.icdoIapAllocationDetail.allocation4_amount,
                    busConstant.IAPAllocationInvestmentFlag);
                aobjAllocationDetail.icdoIapAllocationDetail.allocation4_frft_amount = aobjIAPHelper.CalculateAllocation4InvstOrFrftAmount(ibusIAPYearEndFactorCalculation.iintAllocationYear, aobjAllocationDetail.icdoIapAllocationDetail.allocation4_amount,
                    busConstant.IAPAllocationForfeitureFlag);
            }
        }

        /// <summary>
        /// Method to create the reconciliation reports
        /// </summary>
        private void CreateReconciliationReports()
        {
            try
            {
                iobjPassInfo.BeginTransaction();
                CreateAnnualAllocationSummaryAndPresentationReport();
                CreateDetailSummaryComparisonReport();
                CreateFinancialReport();
                CreateIRSLimitReport();
                iobjPassInfo.Commit();
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                String lstrMsg = "Error while generating reports : " + ex.ToString();
                PostErrorMessage(lstrMsg);
                iobjPassInfo.Rollback();
                throw ex;
            }
        }

        private void CreateAnnualAllocationSummaryAndPresentationReport()
        {
            DataTable ldtAllocationSummary = new DataTable();
            ldtAllocationSummary.Columns.Add("allocation_year", Type.GetType("System.Int32"));
            ldtAllocationSummary.Columns.Add("prev_year_ending_balance", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("system_beg_balance", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("open_bal_adjustments", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("forfeiture_amount", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_alloc1_amount", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_alloc2_amount", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_alloc3_amount", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_alloc4_amount", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_alloc5_amount", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_alloc_amount", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("quarterly_allocations", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("ry_allocation2", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("ry_allocation4", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("payouts", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("un_allocable_balance", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("net_bal_for_invst_income_alloc", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("alloc1_factor", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("total_invst_income_accounting", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("admin_expenses_accounting", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("net_invst_income_for_allocations", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("weighted_average_accounting", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("invs_income_pro_ration_alloc1", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("invs_income_alloc1", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("invs_income_alloc2and4", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("invs_income_pro_ration_alloc2", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("invs_income_pro_ration_alloc4", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_ineligible_hours", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_ineligible_contrb", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("current_ineligible_hours", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("current_ineligible_contrb", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("total_frft_amt_alloc2", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("alloc2_frft_factor", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("eligible_hours", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("hourly_contributions", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("invst_income_alloc2", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("alloc2_invst_factor", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("alloc2_flat_factor", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("net_alloc2_factor", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_ineligible_comp", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("current_inelibile_comp", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("overlimit", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("total_frft_amount_alloc4", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("alloc4_frft_factor", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("percent_comp", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("invst_income_alloc4", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("alloc4_invst_factor", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("net_alloc4_factor", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("ending_bal_alloc_active", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("actual_contrb_recvd", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("current_year_ending_balance", Type.GetType("System.Decimal"));
            // Request ID : 59078 (2017 - IAP Report Changes)
            ldtAllocationSummary.Columns.Add("current_total_iap_hours", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("current_total_thirty_and_half_hours", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("current_total_hourly_contribution", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("current_total_percent_of_compensation", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("total_unallocable_overlimit", Type.GetType("System.Decimal"));

            ldtAllocationSummary.Columns.Add("late_total_iap_hours", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_total_thirty_and_half_hours", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_total_hourly_contribution", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_total_percent_of_compensation", Type.GetType("System.Decimal"));
            //Rohan
            ldtAllocationSummary.Columns.Add("late_eligible_hourly_contribution_amount", Type.GetType("System.Decimal"));
            ldtAllocationSummary.Columns.Add("late_eligible_compensation_amount", Type.GetType("System.Decimal"));
            //PIR 885 New
            ldtAllocationSummary.Columns.Add("misc_adjustments_frm_accounting", Type.GetType("System.Decimal"));

            DataRow ldr = ldtAllocationSummary.NewRow();
            ldr["allocation_year"] = ibusIAPYearEndFactorCalculation.iintAllocationYear;
            ldr["prev_year_ending_balance"] = ibusIAPYearEndFactorCalculation.idecPrevYearEndBalance;
            ldr["system_beg_balance"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["system_beginning_balance"]);
            ldr["open_bal_adjustments"] = ibusIAPYearEndFactorCalculation.idecOpeningBalanceAdjustment;
            ldr["forfeiture_amount"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["forfeited_balance"]);
            ldr["late_alloc1_amount"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc1_amount"]);
            ldr["late_alloc2_amount"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc2_amount"]);
            ldr["late_alloc3_amount"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc3_amount"]);
            ldr["late_alloc4_amount"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc4_amount"]);
            ldr["late_alloc5_amount"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc5_amount"]);

            //rohan
            ldr["late_alloc_amount"] = (Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc1_amount"]) + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc2_amount"]) +
                Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc3_amount"]) + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc4_amount"]) +
                Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_alloc5_amount"])
                - (Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_eligible_hourly_contribution_amount"]) +
                Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_eligible_compensation_amount"])));

            ldr["quarterly_allocations"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["quaterly_allocations_amount"]);
            ldr["ry_allocation2"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["retirement_year_allocation2_amount"]);
            ldr["ry_allocation4"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["retirement_year_allocation4_amount"]);
            ldr["payouts"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["payouts"]);
            ldr["un_allocable_balance"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["unallocable_amount"]);
            ldr["net_bal_for_invst_income_alloc"] = ibusIAPYearEndFactorCalculation.idecBalanceEligibleForInvstIncomeAllocation + ibusIAPYearEndFactorCalculation.idecBalanceNotEligibleForInvstIncomeAllocation;
            ldr["alloc1_factor"] = ibusIAPYearEndFactorCalculation.idecAlloc1Factor;
            ldr["total_invst_income_accounting"] = ibusIAPYearEndFactorCalculation.idecTotalInvstIncome;
            ldr["admin_expenses_accounting"] = ibusIAPYearEndFactorCalculation.idecAdministrativeExpenses;
            ldr["net_invst_income_for_allocations"] = ibusIAPYearEndFactorCalculation.idecNetInvstIncomeForAllocations;
            ldr["weighted_average_accounting"] = ibusIAPYearEndFactorCalculation.idecWeightedAverage;
            ldr["invs_income_pro_ration_alloc1"] = ibusIAPYearEndFactorCalculation.idecInvstIncomeProRationAlloc1;
            ldr["invs_income_alloc1"] = ibusIAPYearEndFactorCalculation.idecInvstIncomeAmountAlloc1;
            ldr["invs_income_alloc2and4"] = ibusIAPYearEndFactorCalculation.idecInvstIncomeAmountAlloc2And4;
            ldr["invs_income_pro_ration_alloc2"] = ibusIAPYearEndFactorCalculation.idecInvstIncomeProRationAlloc2;
            ldr["invs_income_pro_ration_alloc4"] = ibusIAPYearEndFactorCalculation.idecInvstIncomeProRationAlloc4;
            ldr["late_ineligible_hours"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_ineligible_hours"]);
            ldr["late_ineligible_contrb"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_ineligible_hourly_contribution_amount"]);
            ldr["current_ineligible_hours"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["current_year_ineligible_hours"]);
            ldr["current_ineligible_contrb"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["current_year_ineligible_contribution_amount"]);
            ldr["total_frft_amt_alloc2"] = ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc2;
            ldr["alloc2_frft_factor"] = ibusIAPYearEndFactorCalculation.idecAlloc2FrftFactor;
            ldr["eligible_hours"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["eligible_hours"]);
            ldr["hourly_contributions"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["hourly_contribution_amount"]);
            ldr["invst_income_alloc2"] = ibusIAPYearEndFactorCalculation.idecInvstIncomeForHourly;
            ldr["alloc2_invst_factor"] = ibusIAPYearEndFactorCalculation.idecAlloc2InvstFactor;
            ldr["alloc2_flat_factor"] = Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPAllocation2Factor));
            ldr["net_alloc2_factor"] = Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPAllocation2Factor)) + ibusIAPYearEndFactorCalculation.idecAlloc2InvstFactor + ibusIAPYearEndFactorCalculation.idecAlloc2FrftFactor;
            ldr["late_ineligible_comp"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_inelgibile_compensation_amount"]);
            ldr["current_inelibile_comp"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["current_year_inelgibile_compensation_amount"]); //Rohan  - Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["overlimit_contributions_amount"]); ChangeID: 59078
            ldr["overlimit"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["overlimit_contributions_amount"]);
            ldr["total_frft_amount_alloc4"] = ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc4; 
            ldr["alloc4_frft_factor"] = ibusIAPYearEndFactorCalculation.idecAlloc4FrftFactor;
            ldr["percent_comp"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"]);
            ldr["invst_income_alloc4"] = ibusIAPYearEndFactorCalculation.idecInvstIncomeForCompensation;
            ldr["alloc4_invst_factor"] = ibusIAPYearEndFactorCalculation.idecAlloc4InvstFactor;
            ldr["net_alloc4_factor"] = ibusIAPYearEndFactorCalculation.idecAlloc4InvstFactor + ibusIAPYearEndFactorCalculation.idecAlloc4FrftFactor;
            ldr["ending_bal_alloc_active"] = ibusIAPYearEndFactorCalculation.idecBalanceEligibleForInvstIncomeAllocation + ibusIAPYearEndFactorCalculation.idecBalanceNotEligibleForInvstIncomeAllocation
                + ibusIAPYearEndFactorCalculation.idecInvstIncomeAmountAlloc1 + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["hourly_contribution_amount"]) +
                ibusIAPYearEndFactorCalculation.idecInvstIncomeForHourly + ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc2 + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"]) +
                ibusIAPYearEndFactorCalculation.idecInvstIncomeForCompensation + ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc4;
            ldr["actual_contrb_recvd"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"]) + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["current_year_inelgibile_compensation_amount"]);
            ldr["current_year_ending_balance"] = Convert.ToDecimal(ldr["ending_bal_alloc_active"]) + (-1 * Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["unallocable_amount"]));//ROHAN
            //Rohan
            ldr["late_eligible_hourly_contribution_amount"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_eligible_hourly_contribution_amount"]);
            ldr["late_eligible_compensation_amount"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_eligible_compensation_amount"]);
            //PIR 885 New
            ldr["misc_adjustments_frm_accounting"] = ibusIAPYearEndFactorCalculation.idecMiscAdjustmentsFrmAccounting;            
            //Request ID : 59078
            DataTable ldtResult = ibusPrevYearAllocationSummary.GetIAPAllocationWorkHistory();
            if(ldtResult.Rows.Count > 0)
            {
                DataRow[] ldtCurrentRow = (from currrow in ldtResult.AsEnumerable()
                                         where currrow.Field<string>("LATE_FLAG") == "N"
                                         select currrow).ToArray();                
                if(!ldtCurrentRow.IsEmpty())
                {
                    ldr["current_total_iap_hours"] = ldtCurrentRow[0]["TOTAL_IAP_HOURS"];
                    ldr["current_total_thirty_and_half_hours"] = ldtCurrentRow[0]["TOTAL_THIRTY_AND_HALF_HOURS"];
                    ldr["current_total_hourly_contribution"] = ldtCurrentRow[0]["TOTAL_HOURLY_CONTRIBUTION"];
                    ldr["current_total_percent_of_compensation"] = ldtCurrentRow[0]["TOTAL_PERCENT_OF_COMPENSATION"];
                }

                DataRow[] ldtlateRow = (from currrow in ldtResult.AsEnumerable()
                                           where currrow.Field<string>("LATE_FLAG") == "Y"
                                           select currrow).ToArray();
                if (!ldtlateRow.IsEmpty())
                {
                    ldr["late_total_iap_hours"] = ldtlateRow[0]["TOTAL_IAP_HOURS"];
                    ldr["late_total_thirty_and_half_hours"] = ldtlateRow[0]["TOTAL_THIRTY_AND_HALF_HOURS"];
                    ldr["late_total_hourly_contribution"] = ldtlateRow[0]["TOTAL_HOURLY_CONTRIBUTION"];
                    ldr["late_total_percent_of_compensation"] = ldtlateRow[0]["TOTAL_PERCENT_OF_COMPENSATION"];
                }
            }


            ldtAllocationSummary.Rows.Add(ldr);
            ldtAllocationSummary.AcceptChanges();
            ldtAllocationSummary.TableName = busConstant.ReportTable01ForIAP;
            CreatePDFReport(ldtAllocationSummary, busConstant.MPIPHPBatch.IAPAnnualAllocationSummaryReport);
            CreatePDFReport(ldtAllocationSummary, busConstant.MPIPHPBatch.IAPAnnualPresentationReport);
        }

        private void CreateDetailSummaryComparisonReport()
        {
            if (iclbIAPAllocationDetail == null)
                LoadIAPAllocationDetail();

            var lvarIAPAllocationDtlGrouped = from lobjIAPDetail in iclbIAPAllocationDetail
                                              group lobjIAPDetail by new { lobjIAPDetail.icdoIapAllocationDetail.computation_year }
                                                  into SummIAP
                                                  select new
                                                  {
                                                      ldecBegBalance = SummIAP.Sum(o => o.icdoIapAllocationDetail.system_beginning_balance + o.icdoIapAllocationDetail.forfeited_balance + o.icdoIapAllocationDetail.late_alloc1_amount +
                                                                                        o.icdoIapAllocationDetail.late_alloc2_amount + o.icdoIapAllocationDetail.late_alloc3_amount + o.icdoIapAllocationDetail.late_alloc4_amount +
                                                                                        o.icdoIapAllocationDetail.late_alloc5_amount + o.icdoIapAllocationDetail.quaterly_allocations_amount + o.icdoIapAllocationDetail.retirement_year_allocation2_amount +
                                                                                        o.icdoIapAllocationDetail.retirement_year_allocation4_amount + o.icdoIapAllocationDetail.payouts + o.icdoIapAllocationDetail.unallocable_amount +
                                                                                        o.icdoIapAllocationDetail.l161_special_account_amount + o.icdoIapAllocationDetail.l52_special_account_amount),
                                                      ldecAlloc1 = SummIAP.Sum(o => o.icdoIapAllocationDetail.allocation1_amount + o.icdoIapAllocationDetail.l161_allocation1_amount + o.icdoIapAllocationDetail.l52_allocation1_amount),
                                                      ldecAlloc2 = SummIAP.Sum(o => o.icdoIapAllocationDetail.allocation2_amount),
                                                      ldecAlloc2Invst = SummIAP.Sum(o => o.icdoIapAllocationDetail.allocation2_invst_amount),
                                                      ldecAlloc2Frft = SummIAP.Sum(o => o.icdoIapAllocationDetail.allocation2_frft_amount),
                                                      ldecAlloc4 = SummIAP.Sum(o => o.icdoIapAllocationDetail.allocation4_amount),
                                                      ldecAlloc4Invst = SummIAP.Sum(o => o.icdoIapAllocationDetail.allocation4_invst_amount),
                                                      ldecAlloc4Frft = SummIAP.Sum(o => o.icdoIapAllocationDetail.allocation4_frft_amount),
                                                      ldecUnallocabelAmount = SummIAP.Sum(o => o.icdoIapAllocationDetail.unallocable_amount)
                                                  };

            DataTable ldtDtlSummComparison = new DataTable();
            ldtDtlSummComparison.Columns.Add("description", Type.GetType("System.String"));
            ldtDtlSummComparison.Columns.Add("details_amount", Type.GetType("System.Decimal"));
            ldtDtlSummComparison.Columns.Add("summary_amount", Type.GetType("System.Decimal"));
            ldtDtlSummComparison.Columns.Add("diff_amount", Type.GetType("System.Decimal"));

            DataRow ldr1 = ldtDtlSummComparison.NewRow();
            ldr1["description"] = "Beginning Balance";
            ldr1["summary_amount"] = ibusIAPYearEndFactorCalculation.idecBalanceEligibleForInvstIncomeAllocation + ibusIAPYearEndFactorCalculation.idecBalanceNotEligibleForInvstIncomeAllocation;

            DataRow ldr2 = ldtDtlSummComparison.NewRow();
            ldr2["description"] = "Allocation 1";
            ldr2["summary_amount"] = ibusIAPYearEndFactorCalculation.idecInvstIncomeAmountAlloc1;

            DataRow ldr3 = ldtDtlSummComparison.NewRow();
            ldr3["description"] = "Allocation 2";
            ldr3["summary_amount"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["hourly_contribution_amount"]);

            DataRow ldr4 = ldtDtlSummComparison.NewRow();
            ldr4["description"] = "Allocation 2 - Investment Related";
            ldr4["summary_amount"] = ibusIAPYearEndFactorCalculation.idecInvstIncomeForHourly;

            DataRow ldr5 = ldtDtlSummComparison.NewRow();
            ldr5["description"] = "Allocation 2 - Forfeiture Related";
            ldr5["summary_amount"] = ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc2;

            DataRow ldr6 = ldtDtlSummComparison.NewRow();
            ldr6["description"] = "Allocation 4";
            ldr6["summary_amount"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"]);

            DataRow ldr7 = ldtDtlSummComparison.NewRow();
            ldr7["description"] = "Allocation 4 - Investment Related";
            ldr7["summary_amount"] = ibusIAPYearEndFactorCalculation.idecInvstIncomeForCompensation;

            DataRow ldr8 = ldtDtlSummComparison.NewRow();
            ldr8["description"] = "Allocation 4 - Forfeiture Related";
            ldr8["summary_amount"] = ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc4;

            DataRow ldr9 = ldtDtlSummComparison.NewRow();
            ldr9["description"] = "Ending Balance";
            ldr9["summary_amount"] = ibusIAPYearEndFactorCalculation.idecBalanceNotEligibleForInvstIncomeAllocation +
                ibusIAPYearEndFactorCalculation.idecBalanceEligibleForInvstIncomeAllocation + ibusIAPYearEndFactorCalculation.idecInvstIncomeAmountAlloc1 + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["hourly_contribution_amount"]) +
                ibusIAPYearEndFactorCalculation.idecInvstIncomeForHourly + ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc2 + Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"]) +
                ibusIAPYearEndFactorCalculation.idecInvstIncomeForCompensation + ibusIAPYearEndFactorCalculation.idecTotalForfeitureAmountAlloc4 - Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["unallocable_amount"]);

            Array.ForEach(lvarIAPAllocationDtlGrouped.ToArray(), o =>
            {
                ldr1["details_amount"] = o.ldecBegBalance;
                ldr2["details_amount"] = o.ldecAlloc1;
                ldr3["details_amount"] = o.ldecAlloc2;
                ldr4["details_amount"] = o.ldecAlloc2Invst;
                ldr5["details_amount"] = o.ldecAlloc2Frft;
                ldr6["details_amount"] = o.ldecAlloc4;
                ldr7["details_amount"] = o.ldecAlloc4Invst;
                ldr8["details_amount"] = o.ldecAlloc4Frft;
                ldr9["details_amount"] = o.ldecBegBalance + o.ldecAlloc1 + o.ldecAlloc2 + o.ldecAlloc2Invst + o.ldecAlloc2Frft + o.ldecAlloc4 + o.ldecAlloc4Invst + o.ldecAlloc4Frft - o.ldecUnallocabelAmount;
            });
            ldr1["diff_amount"] = Convert.ToDecimal(ldr1["details_amount"]) - Convert.ToDecimal(ldr1["summary_amount"]);
            ldr2["diff_amount"] = Convert.ToDecimal(ldr2["details_amount"]) - Convert.ToDecimal(ldr2["summary_amount"]);
            ldr3["diff_amount"] = Convert.ToDecimal(ldr3["details_amount"]) - Convert.ToDecimal(ldr3["summary_amount"]);
            ldr4["diff_amount"] = Convert.ToDecimal(ldr4["details_amount"]) - Convert.ToDecimal(ldr4["summary_amount"]);
            ldr5["diff_amount"] = Convert.ToDecimal(ldr5["details_amount"]) - Convert.ToDecimal(ldr5["summary_amount"]);
            ldr6["diff_amount"] = Convert.ToDecimal(ldr6["details_amount"]) - Convert.ToDecimal(ldr6["summary_amount"]);
            ldr7["diff_amount"] = Convert.ToDecimal(ldr7["details_amount"]) - Convert.ToDecimal(ldr7["summary_amount"]);
            ldr8["diff_amount"] = Convert.ToDecimal(ldr8["details_amount"]) - Convert.ToDecimal(ldr8["summary_amount"]);
            ldr9["diff_amount"] = Convert.ToDecimal(ldr9["details_amount"]) - Convert.ToDecimal(ldr9["summary_amount"]);

            ldtDtlSummComparison.Rows.Add(ldr1);
            ldtDtlSummComparison.Rows.Add(ldr2);
            ldtDtlSummComparison.Rows.Add(ldr3);
            ldtDtlSummComparison.Rows.Add(ldr4);
            ldtDtlSummComparison.Rows.Add(ldr5);
            ldtDtlSummComparison.Rows.Add(ldr6);
            ldtDtlSummComparison.Rows.Add(ldr7);
            ldtDtlSummComparison.Rows.Add(ldr8);
            ldtDtlSummComparison.Rows.Add(ldr9);
            ldtDtlSummComparison.AcceptChanges();

            ldtDtlSummComparison.TableName = busConstant.ReportTable01ForIAP;
            CreatePDFReport(ldtDtlSummComparison, busConstant.MPIPHPBatch.IAPAnnualDetailVsSummaryReport);
        }

        private void CreateFinancialReport()
        {
            DataTable ldtFinancialReport = new DataTable();
            ldtFinancialReport.Columns.Add("allocation_year", Type.GetType("System.Int32"));
            ldtFinancialReport.Columns.Add("accounting_start_date", Type.GetType("System.DateTime"));
            ldtFinancialReport.Columns.Add("accounting_end_date", Type.GetType("System.DateTime"));
            ldtFinancialReport.Columns.Add("plan_year_start_date", Type.GetType("System.DateTime"));
            ldtFinancialReport.Columns.Add("plan_year_end_date", Type.GetType("System.DateTime"));
            ldtFinancialReport.Columns.Add("prev_year_total_assets_frm_accounting", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("prev_year_total_assets_frm_system", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("prev_year_unallocable_amount", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("iap_hourly_frm_accounting", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("iap_hourly_frm_system", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("iap_percent_frm_accounting", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("iap_percent_frm_system", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("invst_income", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("admin_exp_frm_accounting", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("payouts", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("current_yr_overlimit", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("total_unallocable_overlimit_amount", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("overlimit_interest_amount", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("payouts_frm_accounting", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("weighted_average_frm_accounting", Type.GetType("System.Decimal"));
            ldtFinancialReport.Columns.Add("iap_grand_total", Type.GetType("System.Decimal"));

            DataRow ldr = ldtFinancialReport.NewRow();
            ldr["allocation_year"] = ibusIAPYearEndFactorCalculation.iintAllocationYear;
            ldr["accounting_start_date"] = new DateTime(ibusIAPYearEndFactorCalculation.iintAllocationYear, 1, 1);
            ldr["accounting_end_date"] = new DateTime(ibusIAPYearEndFactorCalculation.iintAllocationYear, 12, 31);
            ldr["plan_year_start_date"] = busGlobalFunctions.GetLastDateOfComputationYear(ibusIAPYearEndFactorCalculation.iintAllocationYear - 1).AddDays(1);
            ldr["plan_year_end_date"] = busGlobalFunctions.GetLastDateOfComputationYear(ibusIAPYearEndFactorCalculation.iintAllocationYear);
            ldr["prev_year_total_assets_frm_accounting"] = ibusIAPYearEndFactorCalculation.idecTotalAssetsFromAccounting;
            ldr["prev_year_total_assets_frm_system"] = ibusIAPYearEndFactorCalculation.idecPrevYearEndBalance + ibusPrevYrIapOverlimitcontributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.total_overlimit_contributions_interest_amount;
            ldr["prev_year_unallocable_amount"] = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.unallocable_amount;
            ldr["iap_hourly_frm_accounting"] = ibusIAPYearEndFactorCalculation.idecIAPHourlyFromAccounting;
            ldr["iap_hourly_frm_system"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_eligible_hourly_contribution_amount"]) +
                                            Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_ineligible_hourly_contribution_amount"]) +
                                            Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["retirement_year_allocation2_amount"]) +
                                            Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["current_year_ineligible_contribution_amount"]) +
                                            Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["hourly_contribution_amount"]);
            ldr["iap_percent_frm_accounting"] = ibusIAPYearEndFactorCalculation.idecIAPPercentFromAccounting;
            ldr["iap_percent_frm_system"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_eligible_compensation_amount"]) +
                                            Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["late_inelgibile_compensation_amount"]) +
                                            Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["retirement_year_allocation4_amount"]) +
                                            Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["current_year_inelgibile_compensation_amount"]) +
                                            Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"]) +
                                            Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["overlimit_contributions_amount"]);
            ldr["invst_income"] = ibusIAPYearEndFactorCalculation.idecTotalInvstIncome;
            ldr["admin_exp_frm_accounting"] = ibusIAPYearEndFactorCalculation.idecAdministrativeExpenses;
            ldr["payouts"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["payouts"]);
            ldr["current_yr_overlimit"] = Convert.ToDecimal(ibusIAPYearEndFactorCalculation.idtIAPAllocationSummary.Rows[0]["overlimit_contributions_amount"]);
            ldr["total_unallocable_overlimit_amount"] = ibusPrevYrIapOverlimitcontributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.total_overlimit_contributions_interest_amount;
            ldr["overlimit_interest_amount"] = ibusCurrYrIAPOverlimitContributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.interest;
            ldr["payouts_frm_accounting"] = ibusIAPYearEndFactorCalculation.idecPayoutsFrmAccounting;
            ldr["weighted_average_frm_accounting"] = ibusIAPYearEndFactorCalculation.idecWeightedAverage;
            ldr["iap_grand_total"] = ibusIAPYearEndFactorCalculation.idecIAPGrandTotal;

            ldtFinancialReport.Rows.Add(ldr);
            ldtFinancialReport.AcceptChanges();

            ldtFinancialReport.TableName = busConstant.ReportTable01ForIAP;
            CreatePDFReport(ldtFinancialReport, busConstant.MPIPHPBatch.IAPAnnualFinancialReport);
        }

        private void CreateIRSLimitReport()
        {
            DataTable ldtIRSLimit = busBase.Select("cdoPersonAccountOverlimitContribution.rptOverLimit", new object[0] { });
            ldtIRSLimit.TableName = busConstant.ReportTable01ForIAP;
            if (ldtIRSLimit != null && ldtIRSLimit.Rows.Count > 0)
            {
                CreatePDFReport(ldtIRSLimit, busConstant.MPIPHPBatch.IAPAnnualOverlimitReport);
                //To update the processed date after generating the report
                DBFunction.DBNonQuery("cdoPersonAccountOverlimitContribution.UpdateProcessDate", new object[0] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
        }

    }
}
