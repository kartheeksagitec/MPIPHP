#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using MPIPHP.BusinessObjects;
using Sagitec.DataObjects;
#endregion

namespace MPIPHP.DataObjects
{
    [Serializable]
    public class doAuditLog : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doAuditLog() : base()
         {
         }

		public int audit_log_id {get;set;}

    	public string form_name {get;set;}

		public string table_name {get;set;}

		public int primary_key {get;set;}

		public int person_id {get;set;}

		public int org_id {get;set;}

		public int org_plan_id {get;set;}

		public string change_type {get;set;}

    }
    [Serializable]
    public enum enmAuditLog
    {
        audit_log_id,
        form_name,
        table_name,
        primary_key,
        person_id,
        org_id,
        org_plan_id,
        change_type,
    }
}

