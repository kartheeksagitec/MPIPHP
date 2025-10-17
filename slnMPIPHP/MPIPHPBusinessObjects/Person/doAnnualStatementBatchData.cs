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
	/// Class MPIPHP.DataObjects.doAnnualStatementBatchData:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doAnnualStatementBatchData : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doAnnualStatementBatchData() : base()
         {
         }
         public int annual_statement__batch_data_id { get; set; }
         public int person_id { get; set; }
         public int computational_year { get; set; }
         public int mpi_vested_years_as_of_prior_year { get; set; }
         public int mpi_qualified_years_as_of_prior_year { get; set; }
         public Decimal mpi_credited_hours_as_of_prior_year { get; set; }
         public Decimal mpi_ee_contributions_as_of_prior_year { get; set; }
         public Decimal mpi_accrued_benefit_as_of_prior_year { get; set; }
         public int mpi_vested_years_in_cureent_year { get; set; }
         public int mpi_qualified_years_in_cureent_year { get; set; }
         public Decimal mpi_credited_hours_in_cureent_year { get; set; }
         public Decimal mpi_accrued_benefits_in_cureent_year { get; set; }
         public Decimal mpi_late_credited_hours_in_cureent_year { get; set; }
         public int mpi_total_vested_years { get; set; }
         public int l600_vested_years { get; set; }
         public int l600_qualified_years { get; set; }
         public Decimal l600_credited_hours { get; set; }
         public Decimal l600_frozen_benefits { get; set; }
         public int l666_vested_years { get; set; }
         public int l666_qualified_years { get; set; }
         public Decimal l666_credited_hours { get; set; }
         public Decimal l666_frozen_benefits { get; set; }
         public int l700_vested_years { get; set; }
         public int l700_qualified_years { get; set; }
         public Decimal l700_credited_hours { get; set; }
         public Decimal l700_frozen_benefits { get; set; }
         public int l52_vested_years { get; set; }
         public int l52_qualified_years { get; set; }
         public Decimal l52_credited_hours { get; set; }
         public Decimal l52_frozen_benefits { get; set; }
         public int l161_vested_years { get; set; }
         public int l161_qualified_years { get; set; }
         public Decimal l161_credited_hours { get; set; }
         public Decimal l161_frozen_benefits { get; set; }
         public Decimal iap_ending_balance_for_prior_year { get; set; }
         public Decimal l52_ending_balance_for_prior_year { get; set; }
         public Decimal l161_ending_balance_for_prior_year { get; set; }
         public Decimal iap_prior_adjustment { get; set; }
         public Decimal l52_prior_adjustment { get; set; }
         public Decimal l161_prior_adjustment { get; set; }
         public Decimal iap_net_investment_income { get; set; }
         public Decimal l52_net_investment_income { get; set; }
         public Decimal l161_net_investment_income { get; set; }
         public Decimal iap_hourly_contributions_iaphoura2 { get; set; }
         public Decimal iap_hourly_contributions_iaphoura2_ialc { get; set; }
         public Decimal iap_hourly_contributions_iaphoura2_falc { get; set; }
         public Decimal iap_percentage_of_compensation { get; set; }
         public Decimal iap_percentage_of_compensation_ialc { get; set; }
         public Decimal iap_percentage_of_compensation_falc { get; set; }
         public Decimal iap_payouts { get; set; }
         public Decimal l52_payouts { get; set; }
         public Decimal l161_payouts { get; set; }
         public Decimal iap_balance_for_current_year { get; set; }
         public Decimal l52_balance_for_current_year { get; set; }
         public Decimal l161_balance_for_current_year { get; set; }
         public int addr_category_id { get; set; }
         public string addr_category_description { get; set; }
         public string addr_category_value { get; set; }
         public int batch_id { get; set; }
         public string person_name { get; set; }
         public string addr_line_1 { get; set; }
         public string addr_line_2 { get; set; }
         public string addr_city { get; set; }
         public int addr_state_id { get; set; }
         public string addr_state_description { get; set; }
         public string addr_state_value { get; set; }
         public int addr_country_id { get; set; }
         public string addr_country_description { get; set; }
         public string addr_country_value { get; set; }
         public string addr_zip_code { get; set; }
         public DateTime end_date { get; set; }
         public DateTime mpi_vested_date { get; set; }
         public string country_description { get; set; }
         public Decimal iap_hours_a2 { get; set; }
         public string corrected_flag { get; set; }
         public string eligible_active_incr_flag { get; set; }
         public string md_flag { get; set; }
         public string retr_special_account_flag { get; set; }
         public string reemployed_under_65_flag { get; set; }
         public string mpi_retiree_flag { get; set; }
         public string local600_retiree_flag { get; set; }
         public string local666_retiree_flag { get; set; }
         public string local700_retiree_flag { get; set; }
         public string local52_retiree_flag { get; set; }
         public string local161_retiree_flag { get; set; }
         public Decimal iap_lumsum_balance { get; set; }
         public Decimal est_iap_life_annuity { get; set; }
         public Decimal est_iap_js100_annuity { get; set; }
         public string pension_only_flag { get; set; }
    }
    [Serializable]
    public enum enmAnnualStatementBatchData
    {
         annual_statement__batch_data_id ,
         person_id ,
         computational_year ,
         mpi_vested_years_as_of_prior_year ,
         mpi_qualified_years_as_of_prior_year ,
         mpi_credited_hours_as_of_prior_year ,
         mpi_ee_contributions_as_of_prior_year ,
         mpi_accrued_benefit_as_of_prior_year ,
         mpi_vested_years_in_cureent_year ,
         mpi_qualified_years_in_cureent_year ,
         mpi_credited_hours_in_cureent_year ,
         mpi_accrued_benefits_in_cureent_year ,
         mpi_late_credited_hours_in_cureent_year ,
         mpi_total_vested_years ,
         l600_vested_years ,
         l600_qualified_years ,
         l600_credited_hours ,
         l600_frozen_benefits ,
         l666_vested_years ,
         l666_qualified_years ,
         l666_credited_hours ,
         l666_frozen_benefits ,
         l700_vested_years ,
         l700_qualified_years ,
         l700_credited_hours ,
         l700_frozen_benefits ,
         l52_vested_years ,
         l52_qualified_years ,
         l52_credited_hours ,
         l52_frozen_benefits ,
         l161_vested_years ,
         l161_qualified_years ,
         l161_credited_hours ,
         l161_frozen_benefits ,
         iap_ending_balance_for_prior_year ,
         l52_ending_balance_for_prior_year ,
         l161_ending_balance_for_prior_year ,
         iap_prior_adjustment ,
         l52_prior_adjustment ,
         l161_prior_adjustment ,
         iap_net_investment_income ,
         l52_net_investment_income ,
         l161_net_investment_income ,
         iap_hourly_contributions_iaphoura2 ,
         iap_hourly_contributions_iaphoura2_ialc ,
         iap_hourly_contributions_iaphoura2_falc ,
         iap_percentage_of_compensation ,
         iap_percentage_of_compensation_ialc ,
         iap_percentage_of_compensation_falc ,
         iap_payouts ,
         l52_payouts ,
         l161_payouts ,
         iap_balance_for_current_year ,
         l52_balance_for_current_year ,
         l161_balance_for_current_year ,
         addr_category_id ,
         addr_category_description ,
         addr_category_value ,
         batch_id ,
         person_name ,
         addr_line_1 ,
         addr_line_2 ,
         addr_city ,
         addr_state_id ,
         addr_state_description ,
         addr_state_value ,
         addr_country_id ,
         addr_country_description ,
         addr_country_value ,
         addr_zip_code ,
         end_date ,
         mpi_vested_date ,
         country_description ,
         iap_hours_a2 ,
         corrected_flag ,
         eligible_active_incr_flag ,
         md_flag ,
         retr_special_account_flag ,
         reemployed_under_65_flag ,
         mpi_retiree_flag ,
         local600_retiree_flag ,
         local666_retiree_flag ,
         local700_retiree_flag ,
         local52_retiree_flag ,
         local161_retiree_flag ,
         iap_lumsum_balance ,
         est_iap_life_annuity ,
         est_iap_js100_annuity ,
         pension_only_flag ,
    }
}
