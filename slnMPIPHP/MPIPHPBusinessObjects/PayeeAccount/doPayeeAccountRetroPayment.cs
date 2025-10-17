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
	/// Class MPIPHP.DataObjects.doPayeeAccountRetroPayment:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountRetroPayment : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccountRetroPayment() : base()
         {
         }
         public int payee_account_retro_payment_id { get; set; }
         public int payee_account_id { get; set; }
         public int retro_payment_type_id { get; set; }
         public string retro_payment_type_description { get; set; }
         public string retro_payment_type_value { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int payment_option_id { get; set; }
         public string payment_option_description { get; set; }
         public string payment_option_value { get; set; }
         public Decimal gross_payment_amount { get; set; }
         public Decimal net_payment_amount { get; set; }
         public string approved_flag { get; set; }
         public string is_overpayment_flag { get; set; }
         public int payment_history_header_id { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountRetroPayment
    {
         payee_account_retro_payment_id ,
         payee_account_id ,
         retro_payment_type_id ,
         retro_payment_type_description ,
         retro_payment_type_value ,
         effective_start_date ,
         effective_end_date ,
         start_date ,
         end_date ,
         payment_option_id ,
         payment_option_description ,
         payment_option_value ,
         gross_payment_amount ,
         net_payment_amount ,
         approved_flag ,
         is_overpayment_flag ,
         payment_history_header_id ,
    }
}

