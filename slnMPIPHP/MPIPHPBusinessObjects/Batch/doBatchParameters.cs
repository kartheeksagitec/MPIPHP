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
	/// Class MPIPHP.DataObjects.doBatchParameters:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBatchParameters : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBatchParameters() : base()
         {
         }
         public int batch_parameters_id { get; set; }
         public int step_no { get; set; }
         public string param_name { get; set; }
         public string param_datatype { get; set; }
         public string param_value { get; set; }
         public string required_flag { get; set; }
         public string readonly_flag { get; set; }
         public string requires_lookup_flag { get; set; }
         public string lookup_form { get; set; }
         public string return_field { get; set; }
    }
    [Serializable]
    public enum enmBatchParameters
    {
         batch_parameters_id ,
         step_no ,
         param_name ,
         param_datatype ,
         param_value ,
         required_flag ,
         readonly_flag ,
         requires_lookup_flag ,
         lookup_form ,
         return_field ,
    }
}

