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
	/// Class NeoSpin.DataObjects.doUserActivityLogParameters:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doUserActivityLogParameters : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doUserActivityLogParameters() : base()
         {
         }
         public int user_activity_log_parameter_id { get; set; }
         public int user_activity_log_detail_id { get; set; }
         public string parameter_name { get; set; }
         public string parameter_value { get; set; }
    }
    [Serializable]
    public enum enmUserActivityLogParameters
    {
         user_activity_log_parameter_id ,
         user_activity_log_detail_id ,
         parameter_name ,
         parameter_value ,
    }
}

