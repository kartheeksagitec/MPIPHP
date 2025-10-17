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
	/// Class MPIPHP.DataObjects.doBenefitProvisionBenefitOption:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionBenefitOption : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitProvisionBenefitOption() : base()
         {
         }
         public int benefit_provision_benefit_option_id { get; set; }
         public int benefit_provision_id { get; set; }
         public int benefit_account_id { get; set; }
         public int plan_benefit_id { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionBenefitOption
    {
         benefit_provision_benefit_option_id ,
         benefit_provision_id ,
         benefit_account_id ,
         plan_benefit_id ,
    }
}

