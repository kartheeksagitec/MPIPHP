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
	/// Class MPIPHP.DataObjects.doWithholdingInformationHistoryDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWithholdingInformationHistoryDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doWithholdingInformationHistoryDetail() : base()
         {
         }
         public int withholding_information_history_detail_id { get; set; }
         public int withholding_information_id { get; set; }
         public int payee_account_id { get; set; }
         public int payment_history_detail_id { get; set; }
         public Decimal withholding_percentage { get; set; }
         public Decimal withhold_flat_amount { get; set; }
         public DateTime withholding_date { get; set; }
    }
    [Serializable]
    public enum enmWithholdingInformationHistoryDetail
    {
         withholding_information_history_detail_id ,
         withholding_information_id ,
         payee_account_id ,
         payment_history_detail_id ,
         withholding_percentage ,
         withhold_flat_amount ,
         withholding_date ,
    }
}

