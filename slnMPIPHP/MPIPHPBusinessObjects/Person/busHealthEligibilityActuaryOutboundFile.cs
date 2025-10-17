using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Linq;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Data.SqlClient;
using Sagitec.DataObjects;
using System.Collections;

namespace MPIPHP.BusinessObjects.Person
{
    [Serializable]
    public class busHealthEligibilityActuaryOutboundFile : busFileBaseOut
    {
        busBase lobjBase = new busBase();
        public busSystemManagement iobjSystemManagement { get; set; }
        public Collection<busHealthEligibiltyActuaryData> iclbHealthEligibiltyActuaryData { get; set; }

        public void LoadHealthEligibiltyActuaryData(DataTable ldtHealthEligibiltyActuaryData)
        {
            if (iobjSystemManagement == null)
            {
                iobjSystemManagement = new busSystemManagement();
                iobjSystemManagement.FindSystemManagement();
            }

            ldtHealthEligibiltyActuaryData = busBase.Select("cdoHealthEligibiltyActuaryData.GetHealthEligibilityActuaryData", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date.Year });
            if (ldtHealthEligibiltyActuaryData.Rows.Count > 0)
            {
                string lstrFilterExpression = string.Empty;
                DataRow[] filteredRows = null;
                foreach (DataColumn ldtColumn in ldtHealthEligibiltyActuaryData.Columns)
                {
                    if (ldtColumn.DataType == typeof(string))
                    {
                        lstrFilterExpression = ldtColumn.ColumnName + " like '%,%'";
                        filteredRows = ldtHealthEligibiltyActuaryData.Select(lstrFilterExpression);
                        if (filteredRows.Count() > 0)
                        {
                            foreach (DataRow ldtRow in filteredRows)
                            {
                                ldtRow[ldtColumn.ColumnName] = "\"" + ldtRow[ldtColumn.ColumnName].ToString() + "\"";
                            }
                        }
                    }
                }
                iclbHealthEligibiltyActuaryData = lobjBase.GetCollection<busHealthEligibiltyActuaryData>(ldtHealthEligibiltyActuaryData, "icdoHealthEligibiltyActuaryData");
            }

        }
    }

}
