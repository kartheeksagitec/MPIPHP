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
	/// Class MPIPHP.DataObjects.doIapAllocation5Recalculation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIapAllocation5Recalculation : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doIapAllocation5Recalculation() : base()
         {
         }
         public int iap_allocation5_recalculation_id { get; set; }
         public int person_account_id { get; set; }
         public Decimal computational_year { get; set; }
         public string iap_allocation5_recalculate_flag { get; set; }
    }
    [Serializable]
    public enum enmIapAllocation5Recalculation
    {
         iap_allocation5_recalculation_id ,
         person_account_id ,
         computational_year ,
         iap_allocation5_recalculate_flag ,
    }
}

