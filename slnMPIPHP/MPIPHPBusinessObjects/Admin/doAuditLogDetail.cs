#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using Sagitec.DataObjects;
#endregion

namespace MPIPHP.DataObjects
{
    [Serializable]
    public class doAuditLogDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doAuditLogDetail() : base()
         {
         }

		public int audit_log_detail_id {get;set;}

		public int audit_log_id {get;set;}

		public string column_name {get;set;}

		public string old_value {get;set;}

		public string new_value {get;set;}

    }
    [Serializable]
    public enum enmAuditLogDetail
    {
        audit_log_detail_id,
        audit_log_id,
        column_name,
        old_value,
        new_value,
    }
}

