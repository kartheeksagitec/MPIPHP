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
	/// Class MPIPHP.DataObjects.doJobScheduleParams:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doJobScheduleParams : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doJobScheduleParams() : base()
         {
         }
         public int job_schedule_params_id { get; set; }
         public int job_schedule_detail_id { get; set; }
         public string param_name { get; set; }
         public string param_value { get; set; }
    }
    [Serializable]
    public enum enmJobScheduleParams
    {
         job_schedule_params_id ,
         job_schedule_detail_id ,
         param_name ,
         param_value ,
    }
}

