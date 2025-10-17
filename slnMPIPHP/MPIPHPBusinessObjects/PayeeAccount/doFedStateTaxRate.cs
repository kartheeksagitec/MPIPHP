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
	/// Class MPIPHP.DataObjects.doFedStateTaxRate:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doFedStateTaxRate : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doFedStateTaxRate() : base()
         {
         }
         public int fed_state_tax_id { get; set; }
         public int tax_identifier_id { get; set; }
         public string tax_identifier_description { get; set; }
         public string tax_identifier_value { get; set; }
         public DateTime effective_date { get; set; }
         public int marital_status_id { get; set; }
         public string marital_status_description { get; set; }
         public string marital_status_value { get; set; }
         public Decimal minimum_amount { get; set; }
         public Decimal maximum_amount { get; set; }
         public Decimal tax_amount { get; set; }
         public Decimal percentage { get; set; }
         public Decimal allowance_amount { get; set; }
         public int allowance { get; set; }
         public Decimal offset_amount { get; set; }
         public Decimal annual_amount { get; set; }
    }
    [Serializable]
    public enum enmFedStateTaxRate
    {
         fed_state_tax_id ,
         tax_identifier_id ,
         tax_identifier_description ,
         tax_identifier_value ,
         effective_date ,
         marital_status_id ,
         marital_status_description ,
         marital_status_value ,
         minimum_amount ,
         maximum_amount ,
         tax_amount ,
         percentage ,
         allowance_amount ,
         allowance ,
         offset_amount ,
         annual_amount ,
    }
}

