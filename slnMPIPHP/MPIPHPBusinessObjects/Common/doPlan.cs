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
	/// Class MPIPHP.DataObjects.doPlan:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPlan : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPlan() : base()
         {
         }
         public int plan_id { get; set; }
         public string plan_code { get; set; }
         public string plan_name { get; set; }
         public int benefit_type_id { get; set; }
         public string benefit_type_description { get; set; }
         public string benefit_type_value { get; set; }
         public int benefit_provision_id { get; set; }
         public DateTime merger_date { get; set; }
    }
    [Serializable]
    public enum enmPlan
    {
         plan_id ,
         plan_code ,
         plan_name ,
         benefit_type_id ,
         benefit_type_description ,
         benefit_type_value ,
         benefit_provision_id ,
         merger_date ,
    }
}

