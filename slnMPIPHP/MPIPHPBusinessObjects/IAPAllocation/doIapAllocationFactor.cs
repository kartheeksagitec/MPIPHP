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
	/// Class MPIPHP.DataObjects.doIapAllocationFactor:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIapAllocationFactor : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doIapAllocationFactor() : base()
         {
         }
         public int iap_allocation_factor_id { get; set; }
         public Decimal plan_year { get; set; }
         public Decimal alloc1_qf1_factor { get; set; }
         public Decimal alloc1_qf2_factor { get; set; }
         public Decimal alloc1_qf3_factor { get; set; }
         public Decimal alloc1_qf4_factor { get; set; }
         public Decimal alloc2_factor { get; set; }
         public Decimal alloc2_invst_factor { get; set; }
         public Decimal alloc2_frft_factor { get; set; }
         public Decimal alloc3_factor { get; set; }
         public Decimal alloc4_factor { get; set; }
         public Decimal alloc4_invst_factor { get; set; }
         public Decimal alloc4_frft_factor { get; set; }
         public Decimal alloc5_affl_factor { get; set; }
         public Decimal alloc5_nonaffl_factor { get; set; }
         public Decimal alloc5_both_factor { get; set; }
    }
    [Serializable]
    public enum enmIapAllocationFactor
    {
         iap_allocation_factor_id ,
         plan_year ,
         alloc1_qf1_factor ,
         alloc1_qf2_factor ,
         alloc1_qf3_factor ,
         alloc1_qf4_factor ,
         alloc2_factor ,
         alloc2_invst_factor ,
         alloc2_frft_factor ,
         alloc3_factor ,
         alloc4_factor ,
         alloc4_invst_factor ,
         alloc4_frft_factor ,
         alloc5_affl_factor ,
         alloc5_nonaffl_factor ,
         alloc5_both_factor ,
    }
}

