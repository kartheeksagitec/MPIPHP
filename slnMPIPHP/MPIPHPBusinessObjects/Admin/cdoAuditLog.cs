#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using System.Data;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoAuditLog:
	/// Inherited from doAuditLog, the class is used to customize the database object doAuditLog.
	/// </summary>
    [Serializable]
    public class cdoAuditLog : doAuditLog
    {
        public cdoAuditLog()
            : base()
        {
        }

        public string change_description { get; set; }

        //public override void LoadData(DataRow adtrRow);
    }
} 
