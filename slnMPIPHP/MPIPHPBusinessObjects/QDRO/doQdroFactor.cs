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
	/// Class MPIPHP.DataObjects.doQdroFactor:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doQdroFactor : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doQdroFactor() : base()
         {
         }
         public int qdro_factor_id { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public Decimal age { get; set; }
         public Decimal early_reduction_factor { get; set; }
         public Decimal early_reduction_factor_disability { get; set; }
    }
    [Serializable]
    public enum enmQdroFactor
    {
         qdro_factor_id ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         age ,
         early_reduction_factor ,
         early_reduction_factor_disability ,
    }
}

