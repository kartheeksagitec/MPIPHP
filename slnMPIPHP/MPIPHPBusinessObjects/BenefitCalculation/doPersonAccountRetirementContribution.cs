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
	/// Class MPIPHP.DataObjects.doPersonAccountRetirementContribution:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountRetirementContribution : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonAccountRetirementContribution() : base()
         {
         }
         public int person_account_retirement_contribution_id { get; set; }
         public int person_account_id { get; set; }
         public DateTime transaction_date { get; set; }
         public DateTime effective_date { get; set; }
         public Decimal computational_year { get; set; }
         public Decimal iap_balance_amount { get; set; }
         public int transaction_type_id { get; set; }
         public string transaction_type_description { get; set; }
         public string transaction_type_value { get; set; }
         public Decimal ee_contribution_amount { get; set; }
         public Decimal uvhp_amount { get; set; }
         public Decimal ee_int_amount { get; set; }
         public Decimal uvhp_int_amount { get; set; }
         public Decimal local_frozen_benefit_amount { get; set; }
         public string contribution_type_value { get; set; }
         public int contribution_type_id { get; set; }
         public string contribution_type_description { get; set; }
         public int contribution_subtype_id { get; set; }
         public string contribution_subtype_description { get; set; }
         public string contribution_subtype_value { get; set; }
         public Decimal local_pre_bis_amount { get; set; }
         public Decimal local_post_bis_amount { get; set; }
         public Decimal local52_special_acct_bal_amount { get; set; }
         public Decimal local161_special_acct_bal_amount { get; set; }
         public int reference_id { get; set; }
         public string legacy_contribution_subtype { get; set; }
         public string record_freeze_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountRetirementContribution
    {
         person_account_retirement_contribution_id ,
         person_account_id ,
         transaction_date ,
         effective_date ,
         computational_year ,
         iap_balance_amount ,
         transaction_type_id ,
         transaction_type_description ,
         transaction_type_value ,
         ee_contribution_amount ,
         uvhp_amount ,
         ee_int_amount ,
         uvhp_int_amount ,
         local_frozen_benefit_amount ,
         contribution_type_value ,
         contribution_type_id ,
         contribution_type_description ,
         contribution_subtype_id ,
         contribution_subtype_description ,
         contribution_subtype_value ,
         local_pre_bis_amount ,
         local_post_bis_amount ,
         local52_special_acct_bal_amount ,
         local161_special_acct_bal_amount ,
         reference_id ,
         legacy_contribution_subtype ,
         record_freeze_flag ,
    }
}

