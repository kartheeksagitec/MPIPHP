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
    /// Class MPIPHP.DataObjects.doIapAllocationDetail:
    /// Inherited from doBase, the class is used to create a wrapper of database table object.
    /// Each property of an instance of this class represents a column of database table object.  
    /// </summary>
    [Serializable]
    public class doIapAllocationDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doIapAllocationDetail() : base()
         {
         }
         public int iap_allocation_detail_id { get; set; }
         public int person_account_id { get; set; }
         public int computation_year { get; set; }
         public Decimal system_beginning_balance { get; set; }
         public Decimal forfeited_balance { get; set; }
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
         public int iap_allocation_category_id { get; set; }
         public string iap_allocation_category_description { get; set; }
         public string iap_allocation_category_value { get; set; }
         public Decimal total_ineligible_iap_late_hours { get; set; }
         public Decimal total_iap_hours { get; set; }
         public Decimal l52_special_account_amount { get; set; }
         public Decimal l161_special_account_amount { get; set; }
         public Decimal allocation1_amount { get; set; }
         public Decimal l52_allocation1_amount { get; set; }
         public Decimal l161_allocation1_amount { get; set; }
         public Decimal allocation2_amount { get; set; }
         public Decimal allocation2_invst_amount { get; set; }
         public Decimal allocation2_frft_amount { get; set; }
         public Decimal allocation4_amount { get; set; }
         public Decimal allocation4_invst_amount { get; set; }
         public Decimal allocation4_frft_amount { get; set; }
         public Decimal late_alloc2_amount { get; set; }
         public Decimal late_alloc4_amount { get; set; }
         public Decimal late_alloc1_amount { get; set; }
         public Decimal late_alloc3_amount { get; set; }
         public Decimal late_alloc5_amount { get; set; }
         public string fund_type { get; set; }
         public Decimal late_hourly_contribution { get; set; }
         public Decimal late_salary_compensation { get; set; }
         public Decimal current_yr_beginning_balance { get; set; }
         public Decimal ending_balance { get; set; }
         public Decimal beginning_bal_adjustment { get; set; }
         public Decimal late_eligible_hourly_contribution_amount { get; set; }
         public Decimal late_eligible_compensation_amount { get; set; }
         public DateTime birth_date { get; set; }
         public DateTime md_date { get; set; }
         public DateTime retire_date { get; set; }
         public DateTime ss_award_date { get; set; }
         public DateTime deceased_date { get; set; }
         public DateTime withdrawal_date { get; set; }
         public DateTime first_pmt_date { get; set; }
         public DateTime qdro_commence_date { get; set; }
         public DateTime determination_date { get; set; }
         public Decimal prev_yr_ending_bal { get; set; }
         public Decimal eligible_quarter { get; set; }
         public Decimal quarterly_factor { get; set; }
         public string alloctype_code { get; set; }
         public Decimal allocable_beg_bal { get; set; }
         public Decimal allocable_ending_balance { get; set; }
         public Decimal hrs_before_det_date { get; set; }
         public Decimal hrs_after_det_date { get; set; }
         public Decimal hrs_a2_before_det_date { get; set; }
         public Decimal hrs_a2_after_det_date { get; set; }
    }
    [Serializable]
    public enum enmIapAllocationDetail
    {
         iap_allocation_detail_id ,
         person_account_id ,
         computation_year ,
         system_beginning_balance ,
         forfeited_balance ,
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
         iap_allocation_category_id ,
         iap_allocation_category_description ,
         iap_allocation_category_value ,
         total_ineligible_iap_late_hours ,
         total_iap_hours ,
         l52_special_account_amount ,
         l161_special_account_amount ,
         allocation1_amount ,
         l52_allocation1_amount ,
         l161_allocation1_amount ,
         allocation2_amount ,
         allocation2_invst_amount ,
         allocation2_frft_amount ,
         allocation4_amount ,
         allocation4_invst_amount ,
         allocation4_frft_amount ,
         late_alloc2_amount ,
         late_alloc4_amount ,
         late_alloc1_amount ,
         late_alloc3_amount ,
         late_alloc5_amount ,
         fund_type ,
         late_hourly_contribution ,
         late_salary_compensation ,
         current_yr_beginning_balance ,
         ending_balance ,
         beginning_bal_adjustment ,
         late_eligible_hourly_contribution_amount ,
         late_eligible_compensation_amount ,
         birth_date ,
         md_date ,
         retire_date ,
         ss_award_date ,
         deceased_date ,
         withdrawal_date ,
         first_pmt_date ,
         qdro_commence_date ,
         determination_date ,
         prev_yr_ending_bal ,
         eligible_quarter ,
         quarterly_factor ,
         alloctype_code ,
         allocable_beg_bal ,
         allocable_ending_balance ,
         hrs_before_det_date ,
         hrs_after_det_date ,
         hrs_a2_before_det_date ,
         hrs_a2_after_det_date ,
    }
}

