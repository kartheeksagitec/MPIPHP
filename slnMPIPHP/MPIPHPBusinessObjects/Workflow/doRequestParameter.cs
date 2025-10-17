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
	/// Class MPIPHP.DataObjects.doRequestParameter:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doRequestParameter : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doRequestParameter() : base()
         {
         }
         public int request_parameter_id { get; set; }
         public int workflow_request_id { get; set; }
         public string parameter_name { get; set; }
         public string parameter_value { get; set; }
    }
    [Serializable]
    public enum enmRequestParameter
    {
         request_parameter_id ,
         workflow_request_id ,
         parameter_name ,
         parameter_value ,
    }
}

