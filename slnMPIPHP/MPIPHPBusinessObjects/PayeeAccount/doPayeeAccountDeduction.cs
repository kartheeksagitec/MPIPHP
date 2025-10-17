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
	/// Class MPIPHP.DataObjects.doPayeeAccountDeduction:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountDeduction : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccountDeduction() : base()
         {
         }
         public int payee_account_deduction_id { get; set; }
         public int payee_account_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int payment_option_id { get; set; }
         public string payment_option_description { get; set; }
         public string payment_option_value { get; set; }
         public int payment_item_type_id { get; set; }
         public int payee_account_payment_item_type_id { get; set; }
         public Decimal amount { get; set; }
         public int vendor_org_id { get; set; }
         public string account_number { get; set; }
         public int payment_history_header_id { get; set; }
         public int deduction_type_id { get; set; }
         public string deduction_type_description { get; set; }
         public string deduction_type_value { get; set; }
         public int pay_to_id { get; set; }
         public string pay_to_description { get; set; }
         public string pay_to_value { get; set; }
         public string other_deduction_type_description { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountDeduction
    {
         payee_account_deduction_id ,
         payee_account_id ,
         start_date ,
         end_date ,
         payment_option_id ,
         payment_option_description ,
         payment_option_value ,
         payment_item_type_id ,
         payee_account_payment_item_type_id ,
         amount ,
         vendor_org_id ,
         account_number ,
         payment_history_header_id ,
         deduction_type_id ,
         deduction_type_description ,
         deduction_type_value ,
         pay_to_id ,
         pay_to_description ,
         pay_to_value ,
         other_deduction_type_description ,
         person_id ,
         org_id ,
    }
}

