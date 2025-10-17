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
	/// Class MPIPHP.DataObjects.doDisabilityRetireeIncrease:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDisabilityRetireeIncrease : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDisabilityRetireeIncrease() : base()
         {
         }
         public int disability_retiree_increase_id { get; set; }
         public int benefit_calculation_header_id { get; set; }
         public int qdro_calculation_header_id { get; set; }
         public Decimal retiree_increase_amount { get; set; }
         public DateTime retiree_increase_date { get; set; }
    }
    [Serializable]
    public enum enmDisabilityRetireeIncrease
    {
         disability_retiree_increase_id ,
         benefit_calculation_header_id ,
         qdro_calculation_header_id ,
         retiree_increase_amount ,
         retiree_increase_date ,
    }
}

