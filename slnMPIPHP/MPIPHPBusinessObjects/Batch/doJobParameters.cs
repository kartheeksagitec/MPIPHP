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
	/// Class MPIPHP.DataObjects.doJobParameters:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doJobParameters : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doJobParameters() : base()
         {
         }
         public int job_parameters_id { get; set; }
         public int job_detail_id { get; set; }
         public string param_name { get; set; }
         public string param_value { get; set; }
    }
    [Serializable]
    public enum enmJobParameters
    {
         job_parameters_id ,
         job_detail_id ,
         param_name ,
         param_value ,
    }
}

