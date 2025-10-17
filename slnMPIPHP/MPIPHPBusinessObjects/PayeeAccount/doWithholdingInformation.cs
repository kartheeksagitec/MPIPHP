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
	/// Class MPIPHP.DataObjects.doWithholdingInformation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWithholdingInformation : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doWithholdingInformation() : base()
         {
         }
         public int withholding_information_id { get; set; }
         public Decimal withholding_percentage { get; set; }
         public Decimal withhold_flat_amount { get; set; }
         public DateTime withholding_date_from { get; set; }
         public DateTime withholding_date_to { get; set; }
         public int payee_account_id { get; set; }
    }
    [Serializable]
    public enum enmWithholdingInformation
    {
         withholding_information_id ,
         withholding_percentage ,
         withhold_flat_amount ,
         withholding_date_from ,
         withholding_date_to ,
         payee_account_id ,
    }
}

