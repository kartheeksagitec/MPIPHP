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
	/// Class MPIPHP.DataObjects.doProviderReportPayment:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProviderReportPayment : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doProviderReportPayment() : base()
         {
         }
         public int provider_report_payment_id { get; set; }
         public int subsystem_id { get; set; }
         public string subsystem_description { get; set; }
         public string subsystem_value { get; set; }
         public int subsystem_ref_id { get; set; }
         public int person_id { get; set; }
         public int provider_org_id { get; set; }
         public int payee_account_id { get; set; }
         public DateTime effective_date { get; set; }
         public decimal amount { get; set; }
         public int payment_schedule_id { get; set; }
         public int payment_history_header_id { get; set; }
         public int payment_item_type_id { get; set; }
    }
    [Serializable]
    public enum enmProviderReportPayment
    {
         provider_report_payment_id ,
         subsystem_id ,
         subsystem_description ,
         subsystem_value ,
         subsystem_ref_id ,
         person_id ,
         provider_org_id ,
         payee_account_id ,
         effective_date ,
         amount ,
         payment_schedule_id ,
         payment_history_header_id ,
         payment_item_type_id ,
    }
}

