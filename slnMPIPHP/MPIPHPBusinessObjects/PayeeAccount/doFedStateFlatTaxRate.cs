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
	/// Class MPIPHP.DataObjects.doFedStateFlatTaxRate:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doFedStateFlatTaxRate : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doFedStateFlatTaxRate() : base()
         {
         }
         public int fed_state_flat_tax_id { get; set; }
         public int tax_identifier_id { get; set; }
         public string tax_identifier_description { get; set; }
         public string tax_identifier_value { get; set; }
         public DateTime effective_date { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int retirement_type_id { get; set; }
         public string retirement_type_description { get; set; }
         public string retirement_type_value { get; set; }
         public int account_relation_id { get; set; }
         public string account_relation_description { get; set; }
         public string account_relation_value { get; set; }
         public Decimal flat_tax_percentage { get; set; }
         public int family_relation_id { get; set; }
         public string family_relation_description { get; set; }
         public string family_relation_value { get; set; }
         public string supl_check_flag { get; set; }
    }
    [Serializable]
    public enum enmFedStateFlatTaxRate
    {
         fed_state_flat_tax_id ,
         tax_identifier_id ,
         tax_identifier_description ,
         tax_identifier_value ,
         effective_date ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         retirement_type_id ,
         retirement_type_description ,
         retirement_type_value ,
         account_relation_id ,
         account_relation_description ,
         account_relation_value ,
         flat_tax_percentage ,
         family_relation_id ,
         family_relation_description ,
         family_relation_value ,
         supl_check_flag ,
    }
}

