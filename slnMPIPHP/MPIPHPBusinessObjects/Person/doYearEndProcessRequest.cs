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
	/// Class MPIPHP.DataObjects.doYearEndProcessRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doYearEndProcessRequest : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doYearEndProcessRequest() : base()
         {
         }
         public int year_end_process_request_id { get; set; }
         public int year { get; set; }
         public int year_end_process_id { get; set; }
         public string year_end_process_description { get; set; }
         public string year_end_process_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime status_date { get; set; }
         public string regenerate_flag { get; set; }
         public DateTime processed_date { get; set; }
    }
    [Serializable]
    public enum enmYearEndProcessRequest
    {
         year_end_process_request_id ,
         year ,
         year_end_process_id ,
         year_end_process_description ,
         year_end_process_value ,
         status_id ,
         status_description ,
         status_value ,
         status_date ,
         regenerate_flag ,
         processed_date ,
    }
}

