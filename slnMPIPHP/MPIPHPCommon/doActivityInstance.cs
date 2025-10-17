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
	/// Class MPIPHP.DataObjects.doActivityInstance:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doActivityInstance : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doActivityInstance() : base()
         {
         }
         public int activity_instance_id { get; set; }
         public int process_instance_id { get; set; }
         public int activity_id { get; set; }
         public string checked_out_user { get; set; }
         public long reference_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime suspension_start_date { get; set; }
         public int suspension_minutes { get; set; }
         public DateTime suspension_end_date { get; set; }
         public string return_from_audit_flag { get; set; }
         public int resume_action_id { get; set; }
         public string resume_action_description { get; set; }
         public string resume_action_value { get; set; }
         public string comments { get; set; }
    }
    [Serializable]
    public enum enmActivityInstance
    {
         activity_instance_id ,
         process_instance_id ,
         activity_id ,
         checked_out_user ,
         reference_id ,
         status_id ,
         status_description ,
         status_value ,
         suspension_start_date ,
         suspension_minutes ,
         suspension_end_date ,
         return_from_audit_flag ,
         resume_action_id ,
         resume_action_description ,
         resume_action_value ,
         comments ,
    }
}

