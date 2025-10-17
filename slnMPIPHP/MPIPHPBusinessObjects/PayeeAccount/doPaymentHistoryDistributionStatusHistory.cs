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
	/// Class MPIPHP.DataObjects.doPaymentHistoryDistributionStatusHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentHistoryDistributionStatusHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPaymentHistoryDistributionStatusHistory() : base()
         {
         }
         public int distribution_status_history_id { get; set; }
         public int payment_history_distribution_id { get; set; }
         public int payment_history_header_id { get; set; }
         public int distribution_status_id { get; set; }
         public string distribution_status_description { get; set; }
         public string distribution_status_value { get; set; }
         public DateTime transaction_date { get; set; }
         public string status_changed_by { get; set; }
         public int status_change_reason_id { get; set; }
         public string status_change_reason_description { get; set; }
         public string status_change_reason_value { get; set; }
    }
    [Serializable]
    public enum enmPaymentHistoryDistributionStatusHistory
    {
         distribution_status_history_id ,
         payment_history_distribution_id ,
         payment_history_header_id ,
         distribution_status_id ,
         distribution_status_description ,
         distribution_status_value ,
         transaction_date ,
         status_changed_by ,
         status_change_reason_id ,
         status_change_reason_description ,
         status_change_reason_value ,
    }
}

