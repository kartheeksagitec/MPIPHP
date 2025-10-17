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
	/// Class MPIPHP.DataObjects.doLocal700GuaranteedAmount:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doLocal700GuaranteedAmount : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doLocal700GuaranteedAmount() : base()
         {
         }
         public int local_700_guaranteed_amount_id { get; set; }
         public int person_account_id { get; set; }
         public Decimal guaranteed_amount { get; set; }
         public Decimal net_guaranteed_amount { get; set; }
    }
    [Serializable]
    public enum enmLocal700GuaranteedAmount
    {
         local_700_guaranteed_amount_id ,
         person_account_id ,
         guaranteed_amount ,
         net_guaranteed_amount ,
    }
}

