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
	/// Class MPIPHP.DataObjects.doBatchNotification:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBatchNotification : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBatchNotification() : base()
         {
         }
         public int batch_notification_id { get; set; }
         public string description { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int notification_id { get; set; }
         public int notification_timer_days { get; set; }
         public string sql_query_name { get; set; }
    }
    [Serializable]
    public enum enmBatchNotification
    {
         batch_notification_id ,
         description ,
         status_id ,
         status_description ,
         status_value ,
         notification_id ,
         notification_timer_days ,
         sql_query_name ,
    }
}

