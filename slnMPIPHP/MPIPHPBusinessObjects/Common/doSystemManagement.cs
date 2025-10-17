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
	/// Class MPIPHP.DataObjects.doSystemManagement:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doSystemManagement : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doSystemManagement() : base()
         {
         }
         public int system_management_id { get; set; }
         public int current_cycle_no { get; set; }
         public int region_id { get; set; }
         public string region_description { get; set; }
         public string region_value { get; set; }
         public int system_availability_id { get; set; }
         public string system_availability_description { get; set; }
         public string system_availability_value { get; set; }
         public DateTime batch_date { get; set; }
         public string base_directory { get; set; }
         public string email_notification { get; set; }
         public string use_application_date { get; set; }
         public DateTime application_date { get; set; }
         public string system_flag { get; set; }
         public string data1 { get; set; }
         public string data2 { get; set; }
    }
    [Serializable]
    public enum enmSystemManagement
    {
         system_management_id ,
         current_cycle_no ,
         region_id ,
         region_description ,
         region_value ,
         system_availability_id ,
         system_availability_description ,
         system_availability_value ,
         batch_date ,
         base_directory ,
         email_notification ,
         use_application_date ,
         application_date ,
         system_flag ,
         data1 ,
         data2 ,
    }
}

