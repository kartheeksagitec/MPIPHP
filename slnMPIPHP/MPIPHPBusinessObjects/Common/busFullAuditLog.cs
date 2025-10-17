#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busFullAuditLog:
	/// Inherited from busFullAuditLogGen, the class is used to customize the business object busFullAuditLogGen.
	/// </summary>
	[Serializable]
	public class busFullAuditLog : busFullAuditLogGen
	{
        public busPerson ibusPerson { get; set; }
        
        public void GetFullAuditLog(int aintPrimaryKey, string astrFomrName)
        {
            string istrTempModifiedby = string.Empty;
            DataTable ldtAuditLog = new DataTable();

             //for PIR-857
            if (astrFomrName == "wfmPersonOverviewMaintenance")
            {
                DataTable ldtAuditLogforPersonOvrvw = busBase.Select("cdoFullAuditLog.GetAuditHistoryForPersonOverview", new object[2] { aintPrimaryKey, astrFomrName });
                ldtAuditLog = ldtAuditLogforPersonOvrvw;
            }
            else
            {
                ldtAuditLog = busBase.Select("cdoFullAuditLog.GetAuditHistory", new object[2] { aintPrimaryKey, astrFomrName });
            }
            foreach (DataRow row in ldtAuditLog.Rows)
            {
                if (row["istrMODIFIED_BY"] != DBNull.Value)
                {
                    istrTempModifiedby = Convert.ToString(row["istrMODIFIED_BY"]);
                }
                row["istrMODIFIED_BY"] = istrTempModifiedby;
            }

            ldtAuditLog.DefaultView.Sort = "AUDIT_LOG_DETAIL_ID DESC";
            ldtAuditLog = ldtAuditLog.DefaultView.ToTable();
            iclbFullAuditLogDetail = GetCollection<busFullAuditLogDetail>(ldtAuditLog, "icdoFullAuditLogDetail");
            
            this.ibusPerson = new busPerson();
            if (aintPrimaryKey != 0)
            {
                this.ibusPerson.FindPerson(aintPrimaryKey);
            }
        }
	}
}
