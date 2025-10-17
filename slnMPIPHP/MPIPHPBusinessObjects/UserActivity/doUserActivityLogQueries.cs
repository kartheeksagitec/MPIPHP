#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace NeoSpin.DataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doUserActivityLogQueries:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doUserActivityLogQueries : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doUserActivityLogQueries() : base()
         {
         }
         public int user_activity_log_query_id { get; set; }
         public string query { get; set; }
         public string result { get; set; }
         public DateTime start_time { get; set; }
         public DateTime end_time { get; set; }
         public Guid transaction_id { get; set; }
         public string query_id { get; set; }
    }
    [Serializable]
    public enum enmUserActivityLogQueries
    {
         user_activity_log_query_id ,
         query ,
         result ,
         start_time ,
         end_time ,
         transaction_id ,
         query_id ,
    }
}

