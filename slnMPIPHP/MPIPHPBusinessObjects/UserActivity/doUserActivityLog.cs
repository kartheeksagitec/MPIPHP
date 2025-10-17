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
	/// Class NeoSpin.DataObjects.doUserActivityLog:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doUserActivityLog : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doUserActivityLog() : base()
         {
         }
         public int user_activity_log_id { get; set; }
         public string logged_user_id { get; set; }
         public string machine_name { get; set; }
         public string ip_address { get; set; }
         public string mac_address { get; set; }
         public string session_id { get; set; }
         public DateTime login_time { get; set; }
         public DateTime logoff_time { get; set; }
         public string window_name { get; set; }
         public string application_name { get; set; }
         public string invalid_login_flag { get; set; }
         public string os_name { get; set; }
         public string browser_name { get; set; }
         public string is_mobile_device { get; set; }
         public string extra_details { get; set; }
    }
    [Serializable]
    public enum enmUserActivityLog
    {
         user_activity_log_id ,
         logged_user_id ,
         machine_name ,
         ip_address ,
         mac_address ,
         session_id ,
         login_time ,
         logoff_time ,
         window_name ,
         application_name ,
         invalid_login_flag ,
         os_name ,
         browser_name ,
         is_mobile_device ,
         extra_details ,
    }
}

