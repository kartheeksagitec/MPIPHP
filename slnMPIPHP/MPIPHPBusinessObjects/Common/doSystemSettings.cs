#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion
namespace NeoSpin.DataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doSystemSettings:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doSystemSettings : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doSystemSettings() : base()
         {
         }
         public int system_setting_id { get; set; }
         public string setting_name { get; set; }
         public string setting_type { get; set; }
         public string setting_value { get; set; }
         public string encrypted_flag { get; set; }
         public string refreshable { get; set; }
    }
    [Serializable]
    public enum enmSystemSettings
    {
         system_setting_id ,
         setting_name ,
         setting_type ,
         setting_value ,
         encrypted_flag ,
         refreshable ,
    }
}
