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
	/// Class MPIPHP.DataObjects.doQdroIapAllocationDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doQdroIapAllocationDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doQdroIapAllocationDetail() : base()
         {
         }
         public int qdro_iap_allocation_detail_id { get; set; }
         public int qdro_calculation_detail_id { get; set; }
         public int person_account_id { get; set; }
         public int computation_year { get; set; }
         public Decimal quarter_1_factor { get; set; }
         public Decimal quarter_2_factor { get; set; }
         public Decimal quarter_3_factor { get; set; }
         public Decimal quarter_4_factor { get; set; }
         public Decimal gain_loss_amount { get; set; }
         public Decimal balance_amount { get; set; }
    }
    [Serializable]
    public enum enmQdroIapAllocationDetail
    {
         qdro_iap_allocation_detail_id ,
         qdro_calculation_detail_id ,
         person_account_id ,
         computation_year ,
         quarter_1_factor ,
         quarter_2_factor ,
         quarter_3_factor ,
         quarter_4_factor ,
         gain_loss_amount ,
         balance_amount ,
    }
}

