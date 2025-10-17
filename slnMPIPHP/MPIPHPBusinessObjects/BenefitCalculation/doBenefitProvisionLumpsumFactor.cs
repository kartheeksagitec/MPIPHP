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
	/// Class MPIPHP.DataObjects.doBenefitProvisionLumpsumFactor:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionLumpsumFactor : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitProvisionLumpsumFactor() : base()
         {
         }
         public int benefit_factor_lumpsum_id { get; set; }
         public Decimal participant_age { get; set; }
         public int year { get; set; }
         public Decimal deferred_factor { get; set; }
         public Decimal life_factor { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionLumpsumFactor
    {
         benefit_factor_lumpsum_id ,
         participant_age ,
         year ,
         deferred_factor ,
         life_factor ,
    }
}

