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
	/// Class MPIPHP.DataObjects.doProcessLog:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProcessLog : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doProcessLog() : base()
         {
         }
         public int process_log_id { get; set; }
         public int cycle_no { get; set; }
         public string process_name { get; set; }
         public int message_type_id { get; set; }
         public string message_type_description { get; set; }
         public string message_type_value { get; set; }
         public string message { get; set; }
         public string subsystem_reference_id { get; set; }
         public string subsystem_table_name { get; set; }
         public int job_header_id { get; set; }

        [Serializable]
        public enum enmProcessLog
        {
            process_log_id,
            cycle_no,
            process_name,
            message_type_id,
            message_type_description,
            message_type_value,
            message,
            subsystem_reference_id,
            subsystem_table_name,
            job_header_id,
        }
    }
    
}

