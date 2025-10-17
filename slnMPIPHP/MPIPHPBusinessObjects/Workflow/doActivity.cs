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
	/// Class MPIPHP.DataObjects.doActivity:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doActivity : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doActivity() : base()
         {
         }
         public int activity_id { get; set; }
         public int process_id { get; set; }
         public string name { get; set; }
         public string display_name { get; set; }
         public int standard_time_in_minutes { get; set; }
         public int role_id { get; set; }
         public int supervisor_role_id { get; set; }
         public int sort_order { get; set; }
         public string is_deleted_flag { get; set; }
         public string allow_independent_complete_ind { get; set; }
         public string mss_display_message { get; set; }
         public int plan_id { get; set; }
    }
    [Serializable]
    public enum enmActivity
    {
         activity_id ,
         process_id ,
         name ,
         display_name ,
         standard_time_in_minutes ,
         role_id ,
         supervisor_role_id ,
         sort_order ,
         is_deleted_flag ,
         allow_independent_complete_ind ,
         mss_display_message ,
         plan_id ,
    }
}

