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
	/// Class MPIPHP.DataObjects.doPaymentScheduleStep:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentScheduleStep : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPaymentScheduleStep() : base()
         {
         }
         public int payment_schedule_step_id { get; set; }
         public int payment_schedule_id { get; set; }
         public int payment_step_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
    }
    [Serializable]
    public enum enmPaymentScheduleStep
    {
         payment_schedule_step_id ,
         payment_schedule_id ,
         payment_step_id ,
         status_id ,
         status_description ,
         status_value ,
    }
}

