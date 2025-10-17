using Sagitec.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPIPHP.BusinessObjects
{
	[Serializable]
    public class busMPIFullAudit : busFullAuditLog
    {
        public busMPIFullAudit()
        {
            iblnNoMainCDO = true;
        }

        //public busPerson ibusPerson { get; set; }

        public Collection<busMPIFullAudit> iclbFullAuditLog { get; set; }

        public void LoadPerson(int aintPrimaryKey)
        {
            if (ibusPerson.IsNull())
            {
                ibusPerson = new busPerson();
            }
            ibusPerson.FindPerson(aintPrimaryKey);
        }

        public void LoadFullAuditLogs(int aintPrimaryKey, string astrFormName)
        {
            iclbFullAuditLog = new Collection<busMPIFullAudit>();

            string istrTempModifiedby = string.Empty;
            DataTable ldtAuditLog = new DataTable();

            if (astrFormName == "wfmPersonOverviewMaintenance")
            {
                DataTable ldtAuditLogforPersonOvrvw = busBase.Select("cdoFullAuditLog.GetAuditHistoryForPersonOverview", new object[2] { aintPrimaryKey, astrFormName });
                ldtAuditLog = ldtAuditLogforPersonOvrvw;
            }
            else
            {
                ldtAuditLog = busBase.Select("cdoFullAuditLog.GetAuditHistory", new object[2] { aintPrimaryKey, astrFormName });
            }

            ldtAuditLog.Columns.Add("AUDIT_DETAILS");

            foreach (DataRow row in ldtAuditLog.Rows)
            {
                if (row["istrMODIFIED_BY"] != DBNull.Value)
                {
                    istrTempModifiedby = Convert.ToString(row["istrMODIFIED_BY"]);
                }
                row["istrMODIFIED_BY"] = istrTempModifiedby;

                var lstrAuditDetails = new
                {
                    column_name = row["COLUMN_NAME"] == DBNull.Value ? string.Empty : row["COLUMN_NAME"],
                    old_value = row["OLD_VALUE"] == DBNull.Value ? string.Empty : row["OLD_VALUE"],
                    new_value = row["NEW_VALUE"] == DBNull.Value ? string.Empty : row["NEW_VALUE"],
                };
                string lsrtJSONAuditDetails = Newtonsoft.Json.JsonConvert.SerializeObject(lstrAuditDetails);
                row["AUDIT_DETAILS"] = lsrtJSONAuditDetails;
            }

            //ldtAuditLog.DefaultView.Sort = "AUDIT_LOG_DETAIL_ID DESC";
            ldtAuditLog = ldtAuditLog.DefaultView.ToTable();
            iclbFullAuditLog = GetCollection<busMPIFullAudit>(ldtAuditLog, "icdoFullAuditLog");
        }
    }
}
