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
	/// Class MPIPHP.DataObjects.doBenefitProvisionBenefitOptionFactor:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionBenefitOptionFactor : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitProvisionBenefitOptionFactor() : base()
         {
         }
         public int benefit_provision_benefit_option_factor_id { get; set; }
         public int benefit_provision_id { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int plan_benefit_id { get; set; }
         public Decimal participant_age { get; set; }
         public Decimal spouse_age { get; set; }
         public Decimal benefit_option_factor { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionBenefitOptionFactor
    {
         benefit_provision_benefit_option_factor_id ,
         benefit_provision_id ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         plan_benefit_id ,
         participant_age ,
         spouse_age ,
         benefit_option_factor ,
    }
}

