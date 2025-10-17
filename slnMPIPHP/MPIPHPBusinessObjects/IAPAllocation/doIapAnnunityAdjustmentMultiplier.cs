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
	/// Class MPIPHP.DataObjects.doIapAnnunityAdjustmentMultiplier:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIapAnnunityAdjustmentMultiplier : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doIapAnnunityAdjustmentMultiplier() : base()
         {
         }
         public int iap_annunity_adjustment_multiplier_id { get; set; }
         public Decimal multiplier { get; set; }
         public DateTime end_date { get; set; }
    }
    [Serializable]
    public enum enmIapAnnunityAdjustmentMultiplier
    {
         iap_annunity_adjustment_multiplier_id ,
         multiplier ,
         end_date ,
    }
}

