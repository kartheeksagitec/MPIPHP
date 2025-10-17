using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using MPIPHP.CustomDataObjects;
using System.Collections.ObjectModel;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busIAPYearEndFactorCalculation : busMPIPHPBase
    {
        #region Properties Used

        public DataTable idtIAPAllocationSummary { get; set; }

        public int iintAllocationYear { get; set; }

        public decimal idecPrevYearEndBalance { get; set; }

        public decimal idecOpeningBalanceAdjustment { get; set; }

        public decimal idecBalanceEligibleForInvstIncomeAllocation { get; set; }

        public decimal idecTotalInvstIncome { get; set; }

        public decimal idecAdministrativeExpenses { get; set; }

        public decimal idecNetInvstIncomeForAllocations { get; set; }

        public decimal idecWeightedAverage { get; set; }

        public decimal idecTotalAssetsFromAccounting { get; set; }

        public decimal idecIAPHourlyFromAccounting { get; set; }

        public decimal idecIAPPercentFromAccounting { get; set; }

        public decimal idecInvstIncomeProRationAlloc1 { get; set; }

        public decimal idecInvstIncomeAmountAlloc1 { get; set; }

        public decimal idecInvstIncomeAmountAlloc2And4 { get; set; }

        public decimal idecInvstIncomeProRationAlloc2 { get; set; }

        public decimal idecInvstIncomeProRationAlloc4 { get; set; }

        public decimal idecTotalForfeitureAmountAlloc2 { get; set; }

        public decimal idecAlloc1Factor { get; set; }

        public decimal idecAlloc2InvstFactor { get; set; }

        public decimal idecAlloc2FrftFactor { get; set; }

        public decimal idecAlloc4InvstFactor { get; set; }

        public decimal idecAlloc4FrftFactor { get; set; }

        public decimal idecInvstIncomeForHourly { get; set; }

        public decimal idecTotalForfeitureAmountAlloc4 { get; set; }

        public decimal idecInvstIncomeForCompensation { get; set; }

        public decimal idecUnallocableOverlimitAmtFrmAccounting { get; set; } //PIR 630

        public decimal idecMiscAdjustmentsFrmAccounting { get; set; } //PIR 630

        public decimal idecPayoutsFrmAccounting { get; set; } //PIR 630

        public decimal idecIAPGrandTotal { get; set; } //ChangeID: 59078

        //ChangeID: 59078
        public decimal idecOverlimitInvIncomeOrLossFactor { get; set; }
        public decimal idecOverlimitInterest { get; set; }

        public decimal idecIAPPaybackSummary { get; set; }
        public decimal idecRetiredInStatementYearRU65Balance { get; set; }
        public decimal idecBalanceNotEligibleForInvstIncomeAllocation { get; set; }

        #endregion

        public void CalculateIAPFactors()
        {
            CalculateAllocation1Factor();
            CalculateInvestmentIncomeAmountForAllc2And4();
            CalculateAllocation2InvstAndFrftFactor();
            CalculateAllocation4InvstAndFrftFactor();
        }

        /// <summary>
        /// Method to calculate IAP Allocation 1 factor
        /// </summary>
        private void CalculateAllocation1Factor()
        {
            CalculateOpeningBalanceAdjustment();
            CalculateBalanceEligibleForInvstIncomeAllocation();
            CalculateNetInvstIncomeForAllocation();
            idecInvstIncomeProRationAlloc1 = CalculateInvestmentIncomeProRation(idecBalanceEligibleForInvstIncomeAllocation, idecWeightedAverage);
            idecInvstIncomeAmountAlloc1 = Math.Round((idecNetInvstIncomeForAllocations * idecInvstIncomeProRationAlloc1), 2, MidpointRounding.AwayFromZero);

            idecAlloc1Factor = Math.Round((idecInvstIncomeAmountAlloc1 / idecBalanceEligibleForInvstIncomeAllocation), 10, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Method to calculate Allocation 2 Invst and Forfeiture factors
        /// </summary>
        private void CalculateAllocation2InvstAndFrftFactor()
        {
            CalculateAllocation2InvtFactor();
            CalculateAllocation2FrftFactor();
        }

        /// <summary>
        /// Method to calculate Allocation 4 Invst and Forfeiture factors
        /// </summary>
        private void CalculateAllocation4InvstAndFrftFactor()
        {
            CalculateAllocation4InvtFactor();
            CalculateAllocation4FrftFactor();
        }

        /// <summary>
        /// method to calculate opening balace adjustments
        /// </summary>
        private void CalculateOpeningBalanceAdjustment()
        {
            idecOpeningBalanceAdjustment = Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["system_beginning_balance"]) - idecPrevYearEndBalance;
        }

        /// <summary>
        /// Method to calculate the balance eligible for investment income allocation
        /// </summary>
        private void CalculateBalanceEligibleForInvstIncomeAllocation()
        {
            idecBalanceEligibleForInvstIncomeAllocation = Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["system_beginning_balance"]) + Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["forfeited_balance"]) +
                Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_alloc1_amount"]) + Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_alloc2_amount"]) +
                Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_alloc3_amount"]) + Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_alloc4_amount"]) +
                Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_alloc5_amount"]) + Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["quaterly_allocations_amount"]) +
                Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["retirement_year_allocation2_amount"]) + Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["retirement_year_allocation4_amount"]) +
                Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["payouts"]) + Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["unallocable_amount"]) 
                - idecBalanceNotEligibleForInvstIncomeAllocation;
        }

        /// <summary>
        /// Method to calculate the net investment income for allocation
        /// </summary>
        private void CalculateNetInvstIncomeForAllocation()
        {
            idecNetInvstIncomeForAllocations = idecTotalInvstIncome + idecAdministrativeExpenses - idecOpeningBalanceAdjustment -
                Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["forfeited_balance"]) - Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["quaterly_allocations_amount"])
                + idecMiscAdjustmentsFrmAccounting//PIR 630
                - (Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_alloc1_amount"]) +
                   Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_alloc2_amount"]) +
                   Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_alloc3_amount"]) +
                   Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_alloc4_amount"]) +
                   Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_alloc5_amount"]) -
                   (Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_eligible_hourly_contribution_amount"]) + Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_eligible_compensation_amount"])));//Rohan
        }

        /// <summary>
        /// Method to calculate the investment income for pro ration
        /// </summary>
        /// <param name="adecAmount1"></param>
        /// <param name="adecAmount2"></param>
        /// <returns></returns>
        private decimal CalculateInvestmentIncomeProRation(decimal adecAmount1, decimal adecAmount2)
        {
            return Math.Round((adecAmount1 / (adecAmount1 + adecAmount2)), 10, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Method to calculate the investment income amount for allocatin 2 and 4
        /// </summary>
        private void CalculateInvestmentIncomeAmountForAllc2And4()
        {
            idecInvstIncomeAmountAlloc2And4 = idecNetInvstIncomeForAllocations - idecInvstIncomeAmountAlloc1;
        }

        /// <summary>
        /// method to calculate allocation 2 investment factor
        /// </summary>
        private void CalculateAllocation2InvtFactor()
        {
            idecInvstIncomeProRationAlloc2 = CalculateInvestmentIncomeProRation(Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["hourly_contribution_amount"]), Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"]));
            idecInvstIncomeForHourly = Math.Round((idecInvstIncomeAmountAlloc2And4 * idecInvstIncomeProRationAlloc2), 2, MidpointRounding.AwayFromZero);

            idecAlloc2InvstFactor = Math.Round((idecInvstIncomeForHourly / Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["eligible_hours"])), 10, MidpointRounding.AwayFromZero); //TODO confirm whether its divided by hours or contrb.
        }

        /// <summary>
        ///  method to calculate allocation 2 forfeiture factor
        /// </summary>
        private void CalculateAllocation2FrftFactor()
        {
            idecTotalForfeitureAmountAlloc2 = Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_ineligible_hourly_contribution_amount"]) + Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["current_year_ineligible_contribution_amount"]);

            idecAlloc2FrftFactor = Math.Round((idecTotalForfeitureAmountAlloc2 / Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["eligible_hours"])), 10, MidpointRounding.AwayFromZero);  //TODO confirm whether its divided by hours or contrb.
        }

        /// <summary>
        ///  method to calculate allocation 4 investment factor
        /// </summary>
        private void CalculateAllocation4InvtFactor()
        {
            idecInvstIncomeProRationAlloc4 = CalculateInvestmentIncomeProRation(Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"]), Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["hourly_contribution_amount"]));
            idecInvstIncomeForCompensation = Math.Round((idecInvstIncomeAmountAlloc2And4 * idecInvstIncomeProRationAlloc4), 2, MidpointRounding.AwayFromZero);

            idecAlloc4InvstFactor = Math.Round((idecInvstIncomeForCompensation / Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"])), 10, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        ///  method to calculate allocation 4 forfeiture factor
        /// </summary>
        private void CalculateAllocation4FrftFactor()
        {
            idecTotalForfeitureAmountAlloc4 = Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["late_inelgibile_compensation_amount"]) + Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["current_year_inelgibile_compensation_amount"]);

            idecAlloc4FrftFactor = Math.Round((idecTotalForfeitureAmountAlloc4 / Convert.ToDecimal(idtIAPAllocationSummary.Rows[0]["percentage_of_compensation_amount"])), 10, MidpointRounding.AwayFromZero);
        }
    }
}
