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
	/// Class MPIPHP.DataObjects.doBenefitApplicationStatusHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitApplicationStatusHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitApplicationStatusHistory() : base()
         {
         }
         public int benefit_application_status_history_id { get; set; }
         public int benefit_application_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime status_date { get; set; }
    }
    [Serializable]
    public enum enmBenefitApplicationStatusHistory
    {
         benefit_application_status_history_id ,
         benefit_application_id ,
         status_id ,
         status_description ,
         status_value ,
         status_date ,
    }
}

