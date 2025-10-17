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
	/// Class MPIPHP.DataObjects.doFullAuditLogDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doFullAuditLogDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doFullAuditLogDetail() : base()
         {
         }
         public int audit_log_detail_id { get; set; }
         public int audit_log_id { get; set; }
         public string column_name { get; set; }
         public string old_value { get; set; }
         public string new_value { get; set; }
    }
    [Serializable]
    public enum enmFullAuditLogDetail
    {
         audit_log_detail_id ,
         audit_log_id ,
         column_name ,
         old_value ,
         new_value ,
    }
}

