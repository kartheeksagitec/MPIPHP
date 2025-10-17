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
	/// Class MPIPHP.DataObjects.doCustomAccessPolicy:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCustomAccessPolicy : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doCustomAccessPolicy() : base()
         {
         }
         public int custom_access_policy_id { get; set; }
         public int user_serial_id { get; set; }
         public int weekday_id { get; set; }
         public string weekday_description { get; set; }
         public string weekday_value { get; set; }
         public Decimal from_time { get; set; }
         public Decimal to_time { get; set; }
    }
    [Serializable]
    public enum enmCustomAccessPolicy
    {
         custom_access_policy_id ,
         user_serial_id ,
         weekday_id ,
         weekday_description ,
         weekday_value ,
         from_time ,
         to_time ,
    }
}

