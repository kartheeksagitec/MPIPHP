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
	/// Class MPIPHP.DataObjects.doDroModelPlanXr:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDroModelPlanXr : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDroModelPlanXr() : base()
         {
         }
         public int dro_model_plan_id { get; set; }
         public int dro_model_id { get; set; }
         public string dro_model_description { get; set; }
         public string dro_model_value { get; set; }
         public int plan_id { get; set; }
    }
    [Serializable]
    public enum enmDroModelPlanXr
    {
         dro_model_plan_id ,
         dro_model_id ,
         dro_model_description ,
         dro_model_value ,
         plan_id ,
    }
}

