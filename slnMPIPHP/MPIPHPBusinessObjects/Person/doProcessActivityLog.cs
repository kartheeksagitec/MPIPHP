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
	/// Class MPIPHP.DataObjects.doProcessActivityLog:
	/// Inherited from doNeoTrackBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProcessActivityLog : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doProcessActivityLog() : base()
         {
         }
         public int person_id { get; set; }
         public int app_id { get; set; }
         public string plan { get; set; }
         public string benefit_type { get; set; }
         public string app_status { get; set; }
         public string app_created { get; set; }
         public string app_modified { get; set; }
         public string app_modified_by { get; set; }
         public int calc_id { get; set; }
         public string calc_type { get; set; }
         public string calc_status { get; set; }
         public string calc_created { get; set; }
         public string calc_modified { get; set; }
         public string calc_modified_by { get; set; }
         public int payee_acct { get; set; }
         public string pa_created { get; set; }
         public string pa_status { get; set; }
         public string status_dt { get; set; }
         public string status_by { get; set; }
         public int payment_id { get; set; }
         public string ba_benefit_type_value { get; set; }
         public string ch_benefit_type_value { get; set; }
    }
    [Serializable]
    public enum enmProcessActivityLog
    {
         person_id ,
         app_id ,
         plan ,
         benefit_type ,
         app_status ,
         app_created ,
         app_modified ,
         app_modified_by ,
         calc_id ,
         calc_type ,
         calc_status ,
         calc_created ,
         calc_modified ,
         calc_modified_by ,
         payee_acct ,
         pa_created ,
         pa_status ,
         status_dt ,
         status_by ,
         payment_id ,
         ba_benefit_type_value ,
         ch_benefit_type_value ,
    }
}

