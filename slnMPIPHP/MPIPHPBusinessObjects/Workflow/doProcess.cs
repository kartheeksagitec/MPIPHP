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
	/// Class MPIPHP.DataObjects.doProcess:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProcess : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doProcess() : base()
         {
         }
         public int process_id { get; set; }
         public string description { get; set; }
         public string name { get; set; }
         public int priority { get; set; }
         public int type_id { get; set; }
         public string type_description { get; set; }
         public string type_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string use_new_map_flag { get; set; }
    }
    [Serializable]
    public enum enmProcess
    {
         process_id ,
         description ,
         name ,
         priority ,
         type_id ,
         type_description ,
         type_value ,
         status_id ,
         status_description ,
         status_value ,
         use_new_map_flag ,
    }
}

