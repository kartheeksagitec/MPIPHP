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
	/// Class MPIPHP.DataObjects.doPayeeAccountMinimumGuaranteeHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountMinimumGuaranteeHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccountMinimumGuaranteeHistory() : base()
         {
         }
         public int minimum_guarantee_history_id { get; set; }
         public int payee_account_id { get; set; }
         public Decimal old_minimum_guarantee_amount { get; set; }
         public Decimal old_non_taxable_beginning_amount { get; set; }
         public DateTime change_date { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountMinimumGuaranteeHistory
    {
         minimum_guarantee_history_id ,
         payee_account_id ,
         old_minimum_guarantee_amount ,
         old_non_taxable_beginning_amount ,
         change_date ,
    }
}

