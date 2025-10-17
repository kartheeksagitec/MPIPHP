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
	/// Class MPIPHP.DataObjects.doBenefitProvisionBenefitType:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionBenefitType : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitProvisionBenefitType() : base()
         {
         }
         public int benefit_provision_benefit_type_id { get; set; }
         public int benefit_provision_id { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int benefit_account_subtype_id { get; set; }
         public string benefit_account_subtype_description { get; set; }
         public string benefit_account_subtype_value { get; set; }
         public Decimal age { get; set; }
         public Decimal early_reduction_factor { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionBenefitType
    {
         benefit_provision_benefit_type_id ,
         benefit_provision_id ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         benefit_account_subtype_id ,
         benefit_account_subtype_description ,
         benefit_account_subtype_value ,
         age ,
         early_reduction_factor ,
    }
}

