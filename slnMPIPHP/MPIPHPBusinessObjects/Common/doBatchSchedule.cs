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
	/// Class MPIPHP.DataObjects.doBatchSchedule:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBatchSchedule : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBatchSchedule() : base()
         {
         }
         public int batch_schedule_id { get; set; }
         public int step_no { get; set; }
         public string step_name { get; set; }
         public string step_description { get; set; }
         public int frequency_in_days { get; set; }
         public int frequency_in_months { get; set; }
         public DateTime next_run_date { get; set; }
         public string step_parameters { get; set; }
         public string active_flag { get; set; }
         public string requires_transaction_flag { get; set; }
         public string email_notification { get; set; }
         public string order_no { get; set; }
         public string cutoff_start { get; set; }
         public string cutoff_end { get; set; }
    }
    [Serializable]
    public enum enmBatchSchedule
    {
         batch_schedule_id ,
         step_no ,
         step_name ,
         step_description ,
         frequency_in_days ,
         frequency_in_months ,
         next_run_date ,
         step_parameters ,
         active_flag ,
         requires_transaction_flag ,
         email_notification ,
         order_no ,
         cutoff_start ,
         cutoff_end ,
    }
}

