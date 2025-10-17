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
	/// Class MPIPHP.DataObjects.doPirEffortsHours:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPirEffortsHours : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPirEffortsHours() : base()
         {
         }
         public int pir_efforts_hours_id { get; set; }
         public int pir_id { get; set; }
         public int task_code_id { get; set; }
         public string task_code_description { get; set; }
         public string task_code_value { get; set; }
         public int effort_hours { get; set; }
         public string effort_description { get; set; }
         public string user_id { get; set; }
         public DateTime effort_date { get; set; }
    }
    [Serializable]
    public enum enmPirEffortsHours
    {
         pir_efforts_hours_id ,
         pir_id ,
         task_code_id ,
         task_code_description ,
         task_code_value ,
         effort_hours ,
         effort_description ,
         user_id ,
         effort_date ,
    }
}

