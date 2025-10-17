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
	/// Class MPIPHP.DataObjects.doPersonSuspendibleMonth:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonSuspendibleMonth : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonSuspendibleMonth() : base()
         {
         }
         public int person_suspendible_month_id { get; set; }
         public int person_id { get; set; }
         public Decimal plan_year { get; set; }
         public int suspendible_month_id { get; set; }
         public string suspendible_month_description { get; set; }
         public string suspendible_month_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
    }
    [Serializable]
    public enum enmPersonSuspendibleMonth
    {
         person_suspendible_month_id ,
         person_id ,
         plan_year ,
         suspendible_month_id ,
         suspendible_month_description ,
         suspendible_month_value ,
         status_id ,
         status_description ,
         status_value ,
    }
}

