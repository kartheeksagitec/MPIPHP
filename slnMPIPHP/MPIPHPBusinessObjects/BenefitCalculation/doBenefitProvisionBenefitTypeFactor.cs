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
	/// Class MPIPHP.DataObjects.doBenefitProvisionBenefitTypeFactor:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionBenefitTypeFactor : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitProvisionBenefitTypeFactor() : base()
         {
         }
         public int benefit_provision_benefit_type_factor_id { get; set; }
         public int benefit_provision_id { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int benefit_account_subtype_id { get; set; }
         public string benefit_account_subtype_description { get; set; }
         public string benefit_account_subtype_value { get; set; }
         public Decimal age { get; set; }
         public Decimal benefit_type_factor { get; set; }
         public Decimal plan_year { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionBenefitTypeFactor
    {
         benefit_provision_benefit_type_factor_id ,
         benefit_provision_id ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         benefit_account_subtype_id ,
         benefit_account_subtype_description ,
         benefit_account_subtype_value ,
         age ,
         benefit_type_factor ,
         plan_year ,
    }
}

