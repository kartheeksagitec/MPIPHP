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
	/// Class MPIPHP.DataObjects.doDeathNotification:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDeathNotification : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDeathNotification() : base()
         {
         }
         public int death_notification_id { get; set; }
         public int person_id { get; set; }
         public int death_notification_status_id { get; set; }
         public string death_notification_status_description { get; set; }
         public string death_notification_status_value { get; set; }
         public DateTime date_of_death { get; set; }
         public DateTime death_notification_received_date { get; set; }
         public DateTime notification_change_date { get; set; }
         public string reported_by { get; set; }
         public string relationship { get; set; }
         public string phone_number { get; set; }
    }
    [Serializable]
    public enum enmDeathNotification
    {
         death_notification_id ,
         person_id ,
         death_notification_status_id ,
         death_notification_status_description ,
         death_notification_status_value ,
         date_of_death ,
         death_notification_received_date ,
         notification_change_date ,
         reported_by ,
         relationship ,
         phone_number ,
    }
}

