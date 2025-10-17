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
	/// Class MPIPHP.DataObjects.doJobHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doJobHeader : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doJobHeader() : base()
         {
         }
         public int job_header_id { get; set; }
         public string job_name { get; set; }
         public int job_schedule_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime start_time { get; set; }
         public DateTime end_time { get; set; }
    }
    [Serializable]
    public enum enmJobHeader
    {
         job_header_id ,
         job_name ,
         job_schedule_id ,
         status_id ,
         status_description ,
         status_value ,
         start_time ,
         end_time ,
    }
}

