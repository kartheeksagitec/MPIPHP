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
	/// Class MPIPHP.DataObjects.doJobScheduleDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doJobScheduleDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doJobScheduleDetail() : base()
         {
         }
         public int job_schedule_detail_id { get; set; }
         public int job_schedule_id { get; set; }
         public int step_no { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int order_number { get; set; }
         public int dependent_step_no { get; set; }
         public string dependent_step_return_value { get; set; }
         public int operator_id { get; set; }
         public string operator_description { get; set; }
         public string operator_value { get; set; }
         public int step_code { get; set; }
         public string step_name { get; set; }
    }
    [Serializable]
    public enum enmJobScheduleDetail
    {
         job_schedule_detail_id ,
         job_schedule_id ,
         step_no ,
         status_id ,
         status_description ,
         status_value ,
         order_number ,
         dependent_step_no ,
         dependent_step_return_value ,
         operator_id ,
         operator_description ,
         operator_value ,
         step_code ,
         step_name ,
    }
}

