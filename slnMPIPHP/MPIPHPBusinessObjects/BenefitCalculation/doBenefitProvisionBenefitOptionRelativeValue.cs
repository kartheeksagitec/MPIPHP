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
	/// Class MPIPHP.DataObjects.doBenefitProvisionBenefitOptionRelativeValue:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionBenefitOptionRelativeValue : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitProvisionBenefitOptionRelativeValue() : base()
         {
         }
         public int benefit_provision_benefit_option_relative_value_id { get; set; }
         public int benefit_provision_id { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public Decimal participant_age { get; set; }
         public Decimal spouse_age { get; set; }
         public string relative_value { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionBenefitOptionRelativeValue
    {
         benefit_provision_benefit_option_relative_value_id ,
         benefit_provision_id ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         participant_age ,
         spouse_age ,
         relative_value ,
    }
}

