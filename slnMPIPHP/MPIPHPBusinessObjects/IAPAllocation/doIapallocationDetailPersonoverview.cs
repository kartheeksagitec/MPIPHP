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
	/// Class MPIPHP.DataObjects.doIapallocationDetailPersonoverview:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIapallocationDetailPersonoverview : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doIapallocationDetailPersonoverview() : base()
         {
         }
         public Decimal computational_year { get; set; }
         public int quarter { get; set; }
         public Decimal alloc1 { get; set; }
         public Decimal alloc2 { get; set; }
         public Decimal alloc2_invt { get; set; }
         public Decimal alloc2_frft { get; set; }
         public Decimal alloc3 { get; set; }
         public Decimal alloc4 { get; set; }
         public Decimal alloc4_invt { get; set; }
         public Decimal alloc4_frft { get; set; }
         public Decimal alloc5_affl { get; set; }
         public Decimal alloc5_nonaffl { get; set; }
         public Decimal alloc5_both { get; set; }
         public Decimal total { get; set; }
         public Decimal total_payment { get; set; }  //PIR 989
         public Decimal iap_account_balance { get; set; }
    }
    [Serializable]
    public enum enmIapallocationDetailPersonoverview
    {
         computational_year ,
         quarter ,
         alloc1 ,
         alloc2 ,
         alloc2_invt ,
         alloc2_frft ,
         alloc3 ,
         alloc4 ,
         alloc4_invt ,
         alloc4_frft ,
         alloc5_affl ,
         alloc5_nonaffl ,
         alloc5_both ,
         total ,
         total_payment , 
         iap_account_balance ,
    }
}

