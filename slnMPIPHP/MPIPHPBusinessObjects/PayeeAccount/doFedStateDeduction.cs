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
	/// Class MPIPHP.DataObjects.doFedStateDeduction:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doFedStateDeduction : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doFedStateDeduction() : base()
         {
         }
         public int fed_state_deduction_id { get; set; }
         public int tax_identifier_id { get; set; }
         public string tax_identifier_description { get; set; }
         public string tax_identifier_value { get; set; }
         public DateTime effective_date { get; set; }
         public int marital_status_id { get; set; }
         public string marital_status_description { get; set; }
         public string marital_status_value { get; set; }
         public int allowance { get; set; }
         public Decimal allowance_amount { get; set; }
         public Decimal low_income_exemption { get; set; }
         public Decimal standard_deduction { get; set; }
         public Decimal flat_tax_rate { get; set; }
         public Decimal personal_allowance_amount { get; set; }
         public Decimal dependent_allowance_amount { get; set; }
         public Decimal age_and_blindness_amount { get; set; }
    }
    [Serializable]
    public enum enmFedStateDeduction
    {
         fed_state_deduction_id ,
         tax_identifier_id ,
         tax_identifier_description ,
         tax_identifier_value ,
         effective_date ,
         marital_status_id ,
         marital_status_description ,
         marital_status_value ,
         allowance ,
         allowance_amount ,
         low_income_exemption ,
         standard_deduction ,
         flat_tax_rate ,
         personal_allowance_amount ,
         dependent_allowance_amount ,
         age_and_blindness_amount ,
    }
}

