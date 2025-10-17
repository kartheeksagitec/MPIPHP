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
    /// Class MPIPHP.BusinessObjects.busIapAllocationSummary:
    /// Inherited from busIapAllocationSummaryGen, the class is used to customize the business object busIapAllocationSummaryGen.
    /// </summary>
    [Serializable]
    public class busIapAllocationSummary : busIapAllocationSummaryGen
    {
        /// <summary>
        /// Method to load the latest iap allocation summary
        /// </summary>
        public void LoadLatestAllocationSummary()
        {
            DataTable ldtResult = Select<cdoIapAllocationSummary>(new string[0] { }, new object[0] { }, null, "computation_year desc");
            icdoIapAllocationSummary = new cdoIapAllocationSummary();
            if (ldtResult != null && ldtResult.Rows.Count > 0)
                icdoIapAllocationSummary.LoadData(ldtResult.Rows[0]);
        }

        /// <summary>
        /// Method to load the latest iap allocation summary
        /// </summary>
        public void LoadLatestAllocationSummaryAsofYear(int aintYear)
        {
            DataTable ldtResult = Select<cdoIapAllocationSummary>(new string[0] { }, new object[0] { }, null, "computation_year desc");
            icdoIapAllocationSummary = new cdoIapAllocationSummary();
            if (ldtResult != null && ldtResult.Rows.Count > 0)
            {
                DataTable ldtFiltered = ldtResult.AsEnumerable().Where(o => o.Field<int>("computation_year") <= aintYear).AsDataTable();
                if (ldtFiltered != null && ldtFiltered.Rows.Count > 0)
                    icdoIapAllocationSummary.LoadData(ldtFiltered.Rows[0]);
            }
        }
        public int GetMaxAllocationYear()
        {
            DataTable ldtResult = busBase.Select("cdoIapAllocationDetail.GetMaxAllocationYear", new object[0]);
            if (ldtResult.Rows.Count > 0)
            {
                return Convert.ToInt32(ldtResult.Rows[0]["COMPUTATION_YEAR"]);
            }
            else
                return 0;
        }

        public DataTable GetIAPAllocationWorkHistory()
        {
             DataTable ldtResult = new DataTable();
             ldtResult = busBase.Select("cdoIapAllocationSummary.GetIAPSummaryWorkHistory", new object[0]);
             return ldtResult;            
        }
    }
}
