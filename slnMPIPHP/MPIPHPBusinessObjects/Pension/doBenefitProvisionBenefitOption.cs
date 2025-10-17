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
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public string retirement_flag { get; set; }
         public string withdrawal_flag { get; set; }
         public string disability_flag { get; set; }
         public string death_flag { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionBenefitOption
    {
         benefit_provision_benefit_option_id ,
         benefit_provision_id ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         retirement_flag ,
         withdrawal_flag ,
         disability_flag ,
         death_flag ,
    }
}

