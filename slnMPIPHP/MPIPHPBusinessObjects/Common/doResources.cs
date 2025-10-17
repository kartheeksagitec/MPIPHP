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
	/// Class MPIPHP.DataObjects.doResources:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doResources : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doResources() : base()
         {
         }
         public int resource_id { get; set; }
         public int resource_type_id { get; set; }
         public string resource_type_description { get; set; }
         public string resource_type_value { get; set; }
         public string resource_description { get; set; }
    }
    [Serializable]
    public enum enmResources
    {
         resource_id ,
         resource_type_id ,
         resource_type_description ,
         resource_type_value ,
         resource_description ,
    }
}
