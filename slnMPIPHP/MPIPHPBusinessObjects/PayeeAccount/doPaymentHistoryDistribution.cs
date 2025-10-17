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
	/// Class MPIPHP.DataObjects.doPaymentHistoryDistribution:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentHistoryDistribution : doBase
    {
		 [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPaymentHistoryDistribution() : base()
         {
         }
         public int payment_history_distribution_id { get; set; }
         public int payment_history_header_id { get; set; }
         public int payment_method_id { get; set; }
         public string payment_method_description { get; set; }
         public string payment_method_value { get; set; }
         public DateTime check_ach_effective_date { get; set; }
         public string check_message { get; set; }
         public int distribution_status_id { get; set; }
         public string distribution_status_description { get; set; }
         public string distribution_status_value { get; set; }
         public Decimal net_amount { get; set; }
         public string check_number { get; set; }
         public int old_distribution_id { get; set; }
         public string recipient_name { get; set; }
         public string fbo_co { get; set; }
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
         public string addr_zip_4_code { get; set; }
         public string foreign_province { get; set; }
         public string foreign_postal_code { get; set; }
         public string bank_name { get; set; }
         public string routing_number { get; set; }
         public int account_type_id { get; set; }
         public string account_type_description { get; set; }
         public string account_type_value { get; set; }
         public string account_number { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int status_change_reason_id { get; set; }
         public string status_change_reason_description { get; set; }
         public string status_change_reason_value { get; set; }
         public string reissue_to_rollover_org_flag { get; set; }
         public string reissue_to_rollover_org_by { get; set; }
         public int payee_account_ach_detail_id { get; set; }
         public string rollover_account_no { get; set; }
         public string addr_line_3 { get; set; }
         public string ach_reversal_flag { get; set; }
         public string corres_generated_flag { get; set; }
         public string aba_swift_bank_code { get; set; }
         public int payee_account_wire_detail_id { get; set; }
    }
    [Serializable]
    public enum enmPaymentHistoryDistribution
    {
         payment_history_distribution_id ,
         payment_history_header_id ,
         payment_method_id ,
         payment_method_description ,
         payment_method_value ,
         check_ach_effective_date ,
         check_message ,
         distribution_status_id ,
         distribution_status_description ,
         distribution_status_value ,
         net_amount ,
         check_number ,
         old_distribution_id ,
         recipient_name ,
         fbo_co ,
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
         addr_zip_4_code ,
         foreign_province ,
         foreign_postal_code ,
         bank_name ,
         routing_number ,
         account_type_id ,
         account_type_description ,
         account_type_value ,
         account_number ,
         person_id ,
         org_id ,
         status_change_reason_id ,
         status_change_reason_description ,
         status_change_reason_value ,
         reissue_to_rollover_org_flag ,
         reissue_to_rollover_org_by ,
         payee_account_ach_detail_id ,
         rollover_account_no ,
         addr_line_3 ,
         ach_reversal_flag ,
         corres_generated_flag ,
         aba_swift_bank_code ,
         payee_account_wire_detail_id ,
    }
}

