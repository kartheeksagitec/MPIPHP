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
	/// Class MPIPHP.DataObjects.doPersonAccountStatusBeforeDeath:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountStatusBeforeDeath : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonAccountStatusBeforeDeath() : base()
         {
         }
         public int person_account_status_before_death_id { get; set; }
         public int death_notification_id { get; set; }
         public int plan_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountStatusBeforeDeath
    {
         person_account_status_before_death_id ,
         death_notification_id ,
         plan_id ,
         start_date ,
         end_date ,
         status_id ,
         status_description ,
         status_value ,
    }
}

