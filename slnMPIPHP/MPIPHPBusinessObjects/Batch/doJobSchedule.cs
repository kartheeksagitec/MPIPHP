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
	/// Class MPIPHP.DataObjects.doJobSchedule:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doJobSchedule : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doJobSchedule() : base()
         {
         }
         public int job_schedule_id { get; set; }
         public string schedule_name { get; set; }
         public string active_flag { get; set; }
         public int frequency_type_id { get; set; }
         public string frequency_type_description { get; set; }
         public string frequency_type_value { get; set; }
         public int frequency_interval { get; set; }
         public int freq_subday_type { get; set; }
         public int freq_subday_interval { get; set; }
         public int freq_relative_interval { get; set; }
         public int freq_recurance_factor { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public DateTime start_time { get; set; }
         public DateTime end_time { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int priority_id { get; set; }
         public string priority_description { get; set; }
         public string priority_value { get; set; }
         public string cancel_procedure_notes { get; set; }
         public string restart_procedure_notes { get; set; }
         public string batch_status_email_recipient_groups { get; set; }
         public string batch_error_email_recipient_groups { get; set; }
         public string execute_on_org_holiday_flag { get; set; }
         public string schedule_code { get; set; }
         public string cancellable_job_flag { get; set; }
         public string is_annual_statement_generated_flag { get; set; }
         public int logical_id { get; set; }
    }
    [Serializable]
    public enum enmJobSchedule
    {
         job_schedule_id ,
         schedule_name ,
         active_flag ,
         frequency_type_id ,
         frequency_type_description ,
         frequency_type_value ,
         frequency_interval ,
         freq_subday_type ,
         freq_subday_interval ,
         freq_relative_interval ,
         freq_recurance_factor ,
         start_date ,
         end_date ,
         start_time ,
         end_time ,
         status_id ,
         status_description ,
         status_value ,
         priority_id ,
         priority_description ,
         priority_value ,
         cancel_procedure_notes ,
         restart_procedure_notes ,
         batch_status_email_recipient_groups ,
         batch_error_email_recipient_groups ,
         execute_on_org_holiday_flag ,
         schedule_code ,
         cancellable_job_flag ,
         is_annual_statement_generated_flag ,
         logical_id,
    }
}

