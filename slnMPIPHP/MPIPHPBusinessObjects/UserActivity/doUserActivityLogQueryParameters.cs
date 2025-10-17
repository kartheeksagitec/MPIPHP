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
	/// Class NeoSpin.DataObjects.doUserActivityLogQueryParameters:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doUserActivityLogQueryParameters : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doUserActivityLogQueryParameters() : base()
         {
         }
         public int user_activity_log_query_parameter_id { get; set; }
         public int user_activity_log_query_id { get; set; }
         public string parameter_name { get; set; }
         public string parameter_type { get; set; }
         public string parameter_value { get; set; }
    }
    [Serializable]
    public enum enmUserActivityLogQueryParameters
    {
         user_activity_log_query_parameter_id ,
         user_activity_log_query_id ,
         parameter_name ,
         parameter_type ,
         parameter_value ,
    }
}

