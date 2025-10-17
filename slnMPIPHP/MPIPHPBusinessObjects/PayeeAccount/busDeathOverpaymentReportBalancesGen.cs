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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP
{
    /// <summary>
    /// Class MPIPHP.busDeathOveraymentReportBalancesGen:
    /// Inherited from busBase, used to create new business object for main table cdoDeathOveraymentReportBalances and its children table. 
    /// </summary>
	[Serializable]
	public class busDeathOverpaymentReportBalancesGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busDeathOveraymentReportBalancesGen
        /// </summary>
		public busDeathOverpaymentReportBalancesGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busDeathOveraymentReportBalancesGen.
        /// </summary>
		public cdoDeathOverpaymentReportBalances icdoDeathOverpaymentReportBalances { get; set; }




        /// <summary>
        /// MPIPHP.busDeathOveraymentReportBalancesGen.FindDeathOveraymentReportBalances():
        /// Finds a particular record from cdoDeathOveraymentReportBalances with its primary key. 
        /// </summary>
        /// <param name="a">A primary key value of type  of cdoDeathOveraymentReportBalances on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindDeathOverpaymentReportBalances(int aintDeathOverpaymentReportBalancesId)
		{
			bool lblnResult = false;
			if (icdoDeathOverpaymentReportBalances == null)
			{
				icdoDeathOverpaymentReportBalances = new cdoDeathOverpaymentReportBalances();
			}
			if (icdoDeathOverpaymentReportBalances.SelectRow(new object[1] { aintDeathOverpaymentReportBalancesId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
