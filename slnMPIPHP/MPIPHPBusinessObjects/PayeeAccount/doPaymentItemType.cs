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
	/// Class MPIPHP.DataObjects.doPaymentItemType:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentItemType : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPaymentItemType() : base()
         {
         }
         public int payment_item_type_id { get; set; }
         public string item_type_code { get; set; }
         public string item_type_description { get; set; }
         public int item_priority { get; set; }
         public int item_usage_id { get; set; }
         public string item_usage_description { get; set; }
         public string item_usage_value { get; set; }
         public int check_type_id { get; set; }
         public string check_type_description { get; set; }
         public string check_type_value { get; set; }
         public int item_type_direction { get; set; }
         public string check_component_description { get; set; }
         public string taxable_item_flag { get; set; }
         public string vendor_flag { get; set; }
         public string payment_1099r_flag { get; set; }
         public int check_group_code_id { get; set; }
         public string check_group_code_description { get; set; }
         public string check_group_code_value { get; set; }
         public int adjustment_code_id { get; set; }
         public string adjustment_code_description { get; set; }
         public string adjustment_code_value { get; set; }
         public int deduction_id { get; set; }
         public string deduction_description { get; set; }
         public string deduction_value { get; set; }
         public int special_tax_treatment_code_id { get; set; }
         public string special_tax_treatment_code_description { get; set; }
         public string special_tax_treatment_code_value { get; set; }
         public int allow_rollover_code_id { get; set; }
         public string allow_rollover_code_description { get; set; }
         public string allow_rollover_code_value { get; set; }
         public string rollover_item_code { get; set; }
         public string comments { get; set; }
         public int monthly_summary_group_id { get; set; }
         public string monthly_summary_group_description { get; set; }
         public string monthly_summary_group_value { get; set; }
         public int master_payment_group_id { get; set; }
         public string master_payment_group_description { get; set; }
         public string master_payment_group_value { get; set; }
         public int ach_check_group_id { get; set; }
         public string ach_check_group_description { get; set; }
         public string ach_check_group_value { get; set; }
         public string base_amount_flag { get; set; }
         public string item_type_code_value { get; set; }
         public int payee_detail_group_id { get; set; }
         public string payee_detail_group_description { get; set; }
         public string payee_detail_group_value { get; set; }
         public int cancel_payment_1099r_id { get; set; }
         public string cancel_payment_1099r_description { get; set; }
         public string cancel_payment_1099r_value { get; set; }
         public int non_monthly_payment_group_id { get; set; }
         public string non_monthly_payment_group_description { get; set; }
         public string non_monthly_payment_group_value { get; set; }
         public string receiavable_item_for_retro_flag { get; set; }
         public int adhoc_cola_group_id { get; set; }
         public string adhoc_cola_group_description { get; set; }
         public string adhoc_cola_group_value { get; set; }
         public string reissue_items_flag { get; set; }
         public int receivable_creation_1099r_id { get; set; }
         public string receivable_creation_1099r_description { get; set; }
         public string receivable_creation_1099r_value { get; set; }
    }
    [Serializable]
    public enum enmPaymentItemType
    {
         payment_item_type_id ,
         item_type_code ,
         item_type_description ,
         item_priority ,
         item_usage_id ,
         item_usage_description ,
         item_usage_value ,
         check_type_id ,
         check_type_description ,
         check_type_value ,
         item_type_direction ,
         check_component_description ,
         taxable_item_flag ,
         vendor_flag ,
         payment_1099r_flag ,
         check_group_code_id ,
         check_group_code_description ,
         check_group_code_value ,
         adjustment_code_id ,
         adjustment_code_description ,
         adjustment_code_value ,
         deduction_id ,
         deduction_description ,
         deduction_value ,
         special_tax_treatment_code_id ,
         special_tax_treatment_code_description ,
         special_tax_treatment_code_value ,
         allow_rollover_code_id ,
         allow_rollover_code_description ,
         allow_rollover_code_value ,
         rollover_item_code ,
         comments ,
         monthly_summary_group_id ,
         monthly_summary_group_description ,
         monthly_summary_group_value ,
         master_payment_group_id ,
         master_payment_group_description ,
         master_payment_group_value ,
         ach_check_group_id ,
         ach_check_group_description ,
         ach_check_group_value ,
         base_amount_flag ,
         item_type_code_value ,
         payee_detail_group_id ,
         payee_detail_group_description ,
         payee_detail_group_value ,
         cancel_payment_1099r_id ,
         cancel_payment_1099r_description ,
         cancel_payment_1099r_value ,
         non_monthly_payment_group_id ,
         non_monthly_payment_group_description ,
         non_monthly_payment_group_value ,
         receiavable_item_for_retro_flag ,
         adhoc_cola_group_id ,
         adhoc_cola_group_description ,
         adhoc_cola_group_value ,
         reissue_items_flag ,
         receivable_creation_1099r_id ,
         receivable_creation_1099r_description ,
         receivable_creation_1099r_value ,
    }
}

