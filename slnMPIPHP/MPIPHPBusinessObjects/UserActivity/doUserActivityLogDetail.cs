#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace NeoSpin.DataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doUserActivityLogDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doUserActivityLogDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doUserActivityLogDetail() : base()
         {
         }
         public int user_activity_log_detail_id { get; set; }
         public int user_activity_log_id { get; set; }
         public string action_name { get; set; }
         public string action_name_disp { get; set; }
         public string action_location { get; set; }
         public string action_location_disp { get; set; }
         public string target_form { get; set; }
         public string target_form_disp { get; set; }
         public string response { get; set; }
         public DateTime start_time { get; set; }
         public DateTime end_time { get; set; }
         public Guid transaction_id { get; set; }
    }
    [Serializable]
    public enum enmUserActivityLogDetail
    {
         user_activity_log_detail_id ,
         user_activity_log_id ,
         action_name ,
         action_name_disp ,
         action_location ,
         action_location_disp ,
         target_form ,
         target_form_disp ,
         response ,
         start_time ,
         end_time ,
         transaction_id ,
    }
}

