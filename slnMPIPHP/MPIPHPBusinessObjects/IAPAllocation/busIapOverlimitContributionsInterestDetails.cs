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
	/// Class MPIPHP.busIapOverlimitContributionsInterestDetails:
	/// Inherited from busIapOverlimitContributionsInterestDetailsGen, the class is used to customize the business object busIapOverlimitContributionsInterestDetailsGen.
	/// </summary>
	[Serializable]
	public class busIapOverlimitContributionsInterestDetails : busIapOverlimitContributionsInterestDetailsGen
	{
        public void LoadIAPOverLimitContributionsInterestDetails(int aintYear)
        {
            DataTable ldtResult = Select<cdoIapOverlimitContributionsInterestDetails>(new string[1] {"Computation_Year" }, new object[1] { aintYear }, null, null);
            icdoIapOverlimitContributionsInterestDetails = new cdoIapOverlimitContributionsInterestDetails();
            if (ldtResult != null && ldtResult.Rows.Count > 0)
            {               
                    icdoIapOverlimitContributionsInterestDetails.LoadData(ldtResult.Rows[0]);
            }
        }
	}
}
