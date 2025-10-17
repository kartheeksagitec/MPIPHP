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
	/// Class NeoSpin.DataObjects.doReturnToWorkRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public partial class doReturnToWorkRequest : doBase
    {
         public doReturnToWorkRequest() : base()
         {
         }
         public int reemployment_notification_id { get; set; }
         public int person_id { get; set; }
         public DateTime reemployment_start_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int source_id { get; set; }
         public string source_description { get; set; }
         public string source_value { get; set; }
         public string employer_name { get; set; }
         public string job_classfication { get; set; }
         public string eligible_flag { get; set; }
         public Decimal estimated_hours_per_payroll_month { get; set; }
         public DateTime notification_form_created_on { get; set; }
         public int union_local_number { get; set; }
         public int request_type_id { get; set; }
         public string request_type_description { get; set; }
         public string request_type_value { get; set; }
         public DateTime payment_account_suspension_date { get; set; }
    }
    [Serializable]
    public partial class enmReturnToWorkRequest
    {
      public const string  reemployment_notification_id = "reemployment_notification_id";
      public const string  person_id = "person_id";
      public const string  reemployment_start_date = "reemployment_start_date";
      public const string  status_id = "status_id";
      public const string  status_description = "status_description";
      public const string  status_value = "status_value";
      public const string  source_id = "source_id";
      public const string  source_description = "source_description";
      public const string  source_value = "source_value";
      public const string  employer_name = "employer_name";
      public const string  job_classfication = "job_classfication";
      public const string  eligible_flag = "eligible_flag";
      public const string  estimated_hours_per_payroll_month = "estimated_hours_per_payroll_month";
      public const string  notification_form_created_on = "notification_form_created_on";
      public const string  union_local_number = "union_local_number";
      public const string  request_type_id = "request_type_id";
      public const string  request_type_description = "request_type_description";
      public const string  request_type_value = "request_type_value";
      public const string  payment_account_suspension_date = "payment_account_suspension_date";
    }
}
