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
	/// Class MPIPHP.DataObjects.doCode:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCode : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doCode() : base()
         {
         }
         public int code_id { get; set; }
         public string description { get; set; }
         public string data1_caption { get; set; }
         public string data1_type { get; set; }
         public string data2_caption { get; set; }
         public string data2_type { get; set; }
         public string data3_caption { get; set; }
         public string data3_type { get; set; }
         public string first_lookup_item { get; set; }
         public string first_maintenance_item { get; set; }
         public string comments { get; set; }
         public string legacy_code_id { get; set; }
    }
    [Serializable]
    public enum enmCode
    {
         code_id ,
         description ,
         data1_caption ,
         data1_type ,
         data2_caption ,
         data2_type ,
         data3_caption ,
         data3_type ,
         first_lookup_item ,
         first_maintenance_item ,
         comments ,
         legacy_code_id ,
    }
}

