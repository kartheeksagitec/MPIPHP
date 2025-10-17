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
	/// Class MPIPHP.DataObjects.doReemploymentHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doReemploymentHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doReemploymentHistory() : base()
         {
         }
         public int reemployment_history_id { get; set; }
         public int payee_account_id { get; set; }
         public DateTime reemployed_flag_from_date { get; set; }
         public DateTime reemployed_flag_to_date { get; set; }
         public DateTime resume_benefit_date { get; set; }
         public Decimal support_deduction_amount { get; set; }
         public string reemployment_reason_value { get; set; }
         public int reemployment_reason_id { get; set; }
         public string reemployment_reason_description { get; set; }
         public string support_deduction_item_code { get; set; }
    }
    [Serializable]
    public enum enmReemploymentHistory
    {
         reemployment_history_id ,
         payee_account_id ,
         reemployed_flag_from_date ,
         reemployed_flag_to_date ,
         resume_benefit_date ,
         support_deduction_amount ,
         reemployment_reason_value ,
         reemployment_reason_id ,
         reemployment_reason_description ,
         support_deduction_item_code ,
    }
}

