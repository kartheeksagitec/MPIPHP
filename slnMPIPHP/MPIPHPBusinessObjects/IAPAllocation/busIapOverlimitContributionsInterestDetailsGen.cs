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
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP
{
    /// <summary>
    /// Class MPIPHP.busIapOverlimitContributionsInterestDetailsGen:
    /// Inherited from busBase, used to create new business object for main table cdoIapOverlimitContributionsInterestDetails and its children table. 
    /// </summary>
	[Serializable]
	public class busIapOverlimitContributionsInterestDetailsGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busIapOverlimitContributionsInterestDetailsGen
        /// </summary>
		public busIapOverlimitContributionsInterestDetailsGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busIapOverlimitContributionsInterestDetailsGen.
        /// </summary>
		public cdoIapOverlimitContributionsInterestDetails icdoIapOverlimitContributionsInterestDetails { get; set; }




        /// <summary>
        /// MPIPHP.busIapOverlimitContributionsInterestDetailsGen.FindIapOverlimitContributionsInterestDetails():
        /// Finds a particular record from cdoIapOverlimitContributionsInterestDetails with its primary key. 
        /// </summary>
        /// <param name="aintIapOverlimitContributionsInterestDetailsId">A primary key value of type int of cdoIapOverlimitContributionsInterestDetails on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindIapOverlimitContributionsInterestDetails(int aintIapOverlimitContributionsInterestDetailsId)
		{
			bool lblnResult = false;
			if (icdoIapOverlimitContributionsInterestDetails == null)
			{
				icdoIapOverlimitContributionsInterestDetails = new cdoIapOverlimitContributionsInterestDetails();
			}
			if (icdoIapOverlimitContributionsInterestDetails.SelectRow(new object[1] { aintIapOverlimitContributionsInterestDetailsId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
