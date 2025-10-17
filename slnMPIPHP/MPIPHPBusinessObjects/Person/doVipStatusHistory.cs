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
	/// Class MPIPHP.DataObjects.doVipStatusHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doVipStatusHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doVipStatusHistory() : base()
         {
         }
         public int vip_status_history_id { get; set; }
         public int person_id { get; set; }
         public string vip_flag_old_value { get; set; }
         public DateTime vip_flag_old_value_modified_date { get; set; }
         public string vip_flag_old_value_modified_by { get; set; }
         public string vip_flag_new_value { get; set; }
         public string message { get; set; }
    }
    [Serializable]
    public enum enmVipStatusHistory
    {
         vip_status_history_id ,
         person_id ,
         vip_flag_old_value ,
         vip_flag_old_value_modified_date ,
         vip_flag_old_value_modified_by ,
         vip_flag_new_value ,
         message ,
    }
}

