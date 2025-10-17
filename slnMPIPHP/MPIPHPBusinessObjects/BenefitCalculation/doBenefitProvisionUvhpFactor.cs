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
	/// Class MPIPHP.DataObjects.doBenefitProvisionUvhpFactor:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionUvhpFactor : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitProvisionUvhpFactor() : base()
         {
         }
         public int benefit_provision_uvhp_factor_id { get; set; }
         public int plan_benefit_id { get; set; }
         public Decimal participant_age { get; set; }
         public Decimal spouse_age { get; set; }
         public Decimal benefit_option_factor { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionUvhpFactor
    {
         benefit_provision_uvhp_factor_id ,
         plan_benefit_id ,
         participant_age ,
         spouse_age ,
         benefit_option_factor ,
    }
}

