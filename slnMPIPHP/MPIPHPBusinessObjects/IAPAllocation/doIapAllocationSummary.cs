#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.DataObjects
{
    /// <summary>
    /// Class MPIPHP.DataObjects.doIapAllocationSummary:
    /// Inherited from doBase, the class is used to create a wrapper of database table object.
    /// Each property of an instance of this class represents a column of database table object.  
    /// </summary>
    [Serializable]
    public class doIapAllocationSummary : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doIapAllocationSummary() : base()
         {
         }
         public int iap_allocation_summary_id { get; set; }
         public int computation_year { get; set; }
         public Decimal previous_year_ending_balance { get; set; }
         public Decimal system_beginning_balance { get; set; }
         public Decimal opening_balance_adjustments { get; set; }
         public Decimal total_investment_income_frm_accounting { get; set; }
         public Decimal administrative_expenses_frm_accounting { get; set; }
         public Decimal weighted_average_frm_accounting { get; set; }
         public Decimal total_assets_frm_accounting { get; set; }
         public Decimal iap_hourly_contrb_frm_accounting { get; set; }
         public Decimal iap_percent_compensation_frm_accounting { get; set; }
         public Decimal forfeited_balance { get; set; }
         public Decimal late_allocations_amount { get; set; }
         public Decimal quaterly_allocations_amount { get; set; }
         public Decimal retirement_year_allocation2_amount { get; set; }
         public Decimal retirement_year_allocation4_amount { get; set; }
         public Decimal payouts { get; set; }
         public Decimal unallocable_amount { get; set; }
         public Decimal late_ineligible_hours { get; set; }
         public Decimal late_ineligible_hourly_contribution_amount { get; set; }
         public Decimal current_year_ineligible_hours { get; set; }
         public Decimal current_year_ineligible_contribution_amount { get; set; }
         public Decimal eligible_hours { get; set; }
         public Decimal hourly_contribution_amount { get; set; }
         public Decimal late_inelgibile_compensation_amount { get; set; }
         public Decimal current_year_inelgibile_compensation_amount { get; set; }
         public Decimal overlimit_contributions_amount { get; set; }
         public Decimal percentage_of_compensation_amount { get; set; }
         public Decimal invst_income_allocation1 { get; set; }
         public Decimal invst_income_allocation2 { get; set; }
         public Decimal invst_income_allocation4 { get; set; }
         public Decimal late_eligible_hourly_contribution_amount { get; set; }
         public Decimal late_eligible_compensation_amount { get; set; }
         public Decimal total_payouts_frm_accounting { get; set; }
         public Decimal net_ending_asset_frm_accounting { get; set; }
         public Decimal total_unallocable_overlimit_amt { get; set; }
         public Decimal allocable_beg_bal { get; set; }
         public Decimal net_invst_income_for_all { get; set; }
         public Decimal invst_income_proration_alloc1 { get; set; }
         public Decimal invst_income_factor_alloc1 { get; set; }
         public Decimal invst_income_proration_alloc2 { get; set; }
         public Decimal invst_income_factor_alloc2 { get; set; }
         public Decimal frft_related_factor_alloc2 { get; set; }
         public Decimal invst_income_proration_alloc4 { get; set; }
         public Decimal invst_income_factor_alloc4 { get; set; }
         public Decimal frft_related_factor_alloc4 { get; set; }
         public Decimal ending_balance { get; set; }
         public Decimal iap_grand_total { get; set; }
         public Decimal invst_income_amount_alloc2_and_alloc4 { get; set; }
         public Decimal misc_adjustments_frm_accounting { get; set; }
    }
    [Serializable]
    public enum enmIapAllocationSummary
    {
         iap_allocation_summary_id ,
         computation_year ,
         previous_year_ending_balance ,
         system_beginning_balance ,
         opening_balance_adjustments ,
         total_investment_income_frm_accounting ,
         administrative_expenses_frm_accounting ,
         weighted_average_frm_accounting ,
         total_assets_frm_accounting ,
         iap_hourly_contrb_frm_accounting ,
         iap_percent_compensation_frm_accounting ,
         forfeited_balance ,
         late_allocations_amount ,
         quaterly_allocations_amount ,
         retirement_year_allocation2_amount ,
         retirement_year_allocation4_amount ,
         payouts ,
         unallocable_amount ,
         late_ineligible_hours ,
         late_ineligible_hourly_contribution_amount ,
         current_year_ineligible_hours ,
         current_year_ineligible_contribution_amount ,
         eligible_hours ,
         hourly_contribution_amount ,
         late_inelgibile_compensation_amount ,
         current_year_inelgibile_compensation_amount ,
         overlimit_contributions_amount ,
         percentage_of_compensation_amount ,
         invst_income_allocation1 ,
         invst_income_allocation2 ,
         invst_income_allocation4 ,
         late_eligible_hourly_contribution_amount ,
         late_eligible_compensation_amount ,
         total_payouts_frm_accounting ,
         net_ending_asset_frm_accounting ,
         total_unallocable_overlimit_amt ,
         allocable_beg_bal ,
         net_invst_income_for_all ,
         invst_income_proration_alloc1 ,
         invst_income_factor_alloc1 ,
         invst_income_proration_alloc2 ,
         invst_income_factor_alloc2 ,
         frft_related_factor_alloc2 ,
         invst_income_proration_alloc4 ,
         invst_income_factor_alloc4 ,
         frft_related_factor_alloc4 ,
         ending_balance ,
         iap_grand_total ,
         invst_income_amount_alloc2_and_alloc4 ,
         misc_adjustments_frm_accounting ,
    }
}

