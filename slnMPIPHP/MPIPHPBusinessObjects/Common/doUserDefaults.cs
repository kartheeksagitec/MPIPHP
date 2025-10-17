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
	/// Class MPIPHP.DataObjects.doUserDefaults:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doUserDefaults : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doUserDefaults() : base()
         {
         }
         public int user_default_id { get; set; }
         public int user_serial_id { get; set; }
         public string form_name { get; set; }
         public string group_control_id { get; set; }
         public string default_set_id { get; set; }
         public string data_field { get; set; }
         public string default_value { get; set; }
    }
    [Serializable]
    public enum enmUserDefaults
    {
         user_default_id ,
         user_serial_id ,
         form_name ,
         group_control_id ,
         default_set_id ,
         data_field ,
         default_value ,
    }
}
