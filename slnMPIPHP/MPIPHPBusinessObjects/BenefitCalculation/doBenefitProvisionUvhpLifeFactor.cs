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
	/// Class MPIPHP.DataObjects.doBenefitProvisionUvhpLifeFactor:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionUvhpLifeFactor : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitProvisionUvhpLifeFactor() : base()
         {
         }
         public int benefit_provision_uvhp_life_id { get; set; }
         public Decimal participant_age { get; set; }
         public int year { get; set; }
         public Decimal uvhp_life_factor { get; set; }
         public Decimal def_to_age_factor { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionUvhpLifeFactor
    {
         benefit_provision_uvhp_life_id ,
         participant_age ,
         year ,
         uvhp_life_factor ,
         def_to_age_factor ,
    }
}

