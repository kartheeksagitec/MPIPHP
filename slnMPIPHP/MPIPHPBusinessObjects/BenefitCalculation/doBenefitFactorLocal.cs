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
	/// Class MPIPHP.DataObjects.doBenefitFactorLocal:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitFactorLocal : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitFactorLocal() : base()
         {
         }
         public int benefit_factor_local_id { get; set; }
         public int plan_id { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public Decimal participant_age { get; set; }
         public Decimal spouse_age { get; set; }
         public Decimal factor { get; set; }
    }
    [Serializable]
    public enum enmBenefitFactorLocal
    {
         benefit_factor_local_id ,
         plan_id ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         participant_age ,
         spouse_age ,
         factor ,
    }
}

