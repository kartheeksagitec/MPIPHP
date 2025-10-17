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
	/// Class MPIPHP.DataObjects.doSystemPaths:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doSystemPaths : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doSystemPaths() : base()
         {
         }
         public int path_id { get; set; }
         public string path_description { get; set; }
         public string path_code { get; set; }
         public string path_value { get; set; }
    }
    [Serializable]
    public enum enmSystemPaths
    {
         path_id ,
         path_description ,
         path_code ,
         path_value ,
    }
}

