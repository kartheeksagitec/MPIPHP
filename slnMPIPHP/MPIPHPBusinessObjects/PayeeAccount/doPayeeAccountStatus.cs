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
	/// Class MPIPHP.DataObjects.doPayeeAccountStatus:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountStatus : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccountStatus() : base()
         {
         }
         public int payee_account_status_id { get; set; }
         public int payee_account_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime status_effective_date { get; set; }
         public int suspension_status_reason_id { get; set; }
         public string suspension_status_reason_description { get; set; }
         public string suspension_status_reason_value { get; set; }
         public int terminated_status_reason_id { get; set; }
         public string terminated_status_reason_description { get; set; }
         public string terminated_status_reason_value { get; set; }
         public string termination_reason_description { get; set; }
         public string suspension_reason_description { get; set; }
         public int review_status_reason_id { get; set; }
         public string review_status_reason_description { get; set; }
         public string review_status_reason_value { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountStatus
    {
         payee_account_status_id ,
         payee_account_id ,
         status_id ,
         status_description ,
         status_value ,
         status_effective_date ,
         suspension_status_reason_id ,
         suspension_status_reason_description ,
         suspension_status_reason_value ,
         terminated_status_reason_id ,
         terminated_status_reason_description ,
         terminated_status_reason_value ,
         termination_reason_description ,
         suspension_reason_description ,
         review_status_reason_id ,
         review_status_reason_description ,
         review_status_reason_value ,
    }
}

