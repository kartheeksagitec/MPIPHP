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
	/// Class MPIPHP.DataObjects.doActivityInstanceHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doActivityInstanceHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doActivityInstanceHistory() : base()
         {
         }
         public int activity_instance_history_id { get; set; }
         public int activity_instance_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string action_user_id { get; set; }
         public DateTime start_time { get; set; }
         public DateTime end_time { get; set; }
         public string comments { get; set; }
    }
    [Serializable]
    public enum enmActivityInstanceHistory
    {
         activity_instance_history_id ,
         activity_instance_id ,
         status_id ,
         status_description ,
         status_value ,
         action_user_id ,
         start_time ,
         end_time ,
         comments ,
    }
}

