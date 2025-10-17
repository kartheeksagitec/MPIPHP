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
	/// Class MPIPHP.DataObjects.doBenefitProvisionUvhpAnnuityFactor:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionUvhpAnnuityFactor : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitProvisionUvhpAnnuityFactor() : base()
         {
         }
         public int benefit_provision_uvhp_annuity_factor_id { get; set; }
         public Decimal participant_age { get; set; }
         public Decimal uvhp_annuity_factor { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionUvhpAnnuityFactor
    {
         benefit_provision_uvhp_annuity_factor_id ,
         participant_age ,
         uvhp_annuity_factor ,
    }
}

