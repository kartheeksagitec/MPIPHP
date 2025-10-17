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
	/// Class MPIPHP.DataObjects.doPaymentSchedule:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentSchedule : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPaymentSchedule() : base()
         {
         }
         public int payment_schedule_id { get; set; }
         public DateTime payment_date { get; set; }
         public DateTime process_date { get; set; }
         public DateTime effective_date { get; set; }
         public int schedule_type_id { get; set; }
         public string schedule_type_description { get; set; }
         public string schedule_type_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string check_message { get; set; }
         public string notes { get; set; }
         public DateTime process_start_time { get; set; }
         public DateTime process_end_time { get; set; }
         public string backup_table_prefix { get; set; }
         public DateTime retriment_notice_due_date { get; set; }
         public DateTime document_due_date { get; set; }
         public DateTime payment_setup_cutoff_date { get; set; }
         public DateTime check_file_release_date { get; set; }
         public DateTime abf_processing_date { get; set; }
         public DateTime check_mailing_date { get; set; }
         public DateTime ach_effective_date { get; set; }
         public int schedule_sub_type_id { get; set; }
         public string schedule_sub_type_description { get; set; }
         public string schedule_sub_type_value { get; set; }
    }
    [Serializable]
    public enum enmPaymentSchedule
    {
         payment_schedule_id ,
         payment_date ,
         process_date ,
         effective_date ,
         schedule_type_id ,
         schedule_type_description ,
         schedule_type_value ,
         status_id ,
         status_description ,
         status_value ,
         check_message ,
         notes ,
         process_start_time ,
         process_end_time ,
         backup_table_prefix ,
         retriment_notice_due_date ,
         document_due_date ,
         payment_setup_cutoff_date ,
         check_file_release_date ,
         abf_processing_date ,
         check_mailing_date ,
         ach_effective_date ,
         schedule_sub_type_id ,
         schedule_sub_type_description ,
         schedule_sub_type_value ,
    }
}

