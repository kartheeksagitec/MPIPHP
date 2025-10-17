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
	/// Class MPIPHP.DataObjects.doPaymentStepRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentStepRef : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPaymentStepRef() : base()
         {
         }
         public int payment_step_id { get; set; }
         public string step_name { get; set; }
         public string active_flag { get; set; }
         public int schedule_type_id { get; set; }
         public string schedule_type_description { get; set; }
         public string schedule_type_value { get; set; }
         public string trial_run_flag { get; set; }
         public int main_step_id { get; set; }
         public int run_sequence { get; set; }
         public int batch_schedule_id { get; set; }
    }
    [Serializable]
    public enum enmPaymentStepRef
    {
         payment_step_id ,
         step_name ,
         active_flag ,
         schedule_type_id ,
         schedule_type_description ,
         schedule_type_value ,
         trial_run_flag ,
         main_step_id ,
         run_sequence ,
         batch_schedule_id ,
    }
}

