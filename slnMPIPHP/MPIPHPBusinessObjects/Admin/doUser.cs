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
	/// Class MPIPHP.DataObjects.doUser:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doUser : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doUser() : base()
         {
         }
         public int user_serial_id { get; set; }
         public string user_id { get; set; }
         public string password { get; set; }
         public string first_name { get; set; }
         public string middle_initial { get; set; }
         public string last_name { get; set; }
         public DateTime begin_date { get; set; }
         public DateTime end_date { get; set; }
         public string email_address { get; set; }
         public int user_type_id { get; set; }
         public string user_type_description { get; set; }
         public string user_type_value { get; set; }
         public int user_status_id { get; set; }
         public string user_status_description { get; set; }
         public string user_status_value { get; set; }
         public int person_id { get; set; }
         public string log_activity_flag { get; set; }
         public string initial_page { get; set; }
         public string color_scheme { get; set; }
         public int division_id { get; set; }
         public string division_description { get; set; }
         public string division_value { get; set; }
         public int access_type_id { get; set; }
         public string access_type_description { get; set; }
         public string access_type_value { get; set; }
         public int activity_log_level_id { get; set; }
         public string activity_log_level_description { get; set; }
         public string activity_log_level_value { get; set; }
         public string log_select_query_flag { get; set; }
         public string log_insert_query_flag { get; set; }
         public string log_update_query_flag { get; set; }
         public string log_delete_query_flag { get; set; }
         public int supervisor_id { get; set; }

    }
    [Serializable]
    public enum enmUser
    {
         user_serial_id ,
         user_id ,
         password ,
         first_name ,
         middle_initial ,
         last_name ,
         begin_date ,
         end_date ,
         email_address ,
         user_type_id ,
         user_type_description ,
         user_type_value ,
         user_status_id ,
         user_status_description ,
         user_status_value ,
         person_id ,
         log_activity_flag ,
         initial_page ,
         color_scheme ,
         division_id ,
         division_description ,
         division_value ,
         access_type_id ,
         access_type_description ,
         access_type_value ,
         activity_log_level_id ,
         activity_log_level_description ,
         activity_log_level_value ,
         log_select_query_flag ,
         log_insert_query_flag ,
         log_update_query_flag ,
         log_delete_query_flag ,
         supervisor_id,
    }
}

