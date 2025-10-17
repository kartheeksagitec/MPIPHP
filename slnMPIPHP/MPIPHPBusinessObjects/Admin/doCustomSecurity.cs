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
	/// Class MPIPHP.DataObjects.doCustomSecurity:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCustomSecurity : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doCustomSecurity() : base()
         {
         }
         public int custom_security_id { get; set; }
         public int user_serial_id { get; set; }
         public int resource_id { get; set; }
         public int old_security_value { get; set; }
         public int custom_security_level_id { get; set; }
         public string custom_security_level_description { get; set; }
         public string custom_security_level_value { get; set; }
    }
    [Serializable]
    public enum enmCustomSecurity
    {
         custom_security_id ,
         user_serial_id ,
         resource_id ,
         old_security_value ,
         custom_security_level_id ,
         custom_security_level_description ,
         custom_security_level_value ,
    }
}

