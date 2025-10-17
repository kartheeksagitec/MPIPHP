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
	/// Class MPIPHP.DataObjects.doPirHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPirHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPirHistory() : base()
         {
         }
         public int pir_history_id { get; set; }
         public int pir_id { get; set; }
         public string long_description { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int assigned_to_id { get; set; }
    }
    [Serializable]
    public enum enmPirHistory
    {
         pir_history_id ,
         pir_id ,
         long_description ,
         status_id ,
         status_description ,
         status_value ,
         assigned_to_id ,
    }
}

