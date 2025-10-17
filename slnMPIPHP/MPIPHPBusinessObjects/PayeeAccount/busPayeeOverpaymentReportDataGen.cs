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
    /// Class MPIPHP.busPayeeOverpaymentReportDataGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeOverpaymentReportData and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeOverpaymentReportDataGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busPayeeOverpaymentReportDataGen
        /// </summary>
		public busPayeeOverpaymentReportDataGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeOverpaymentReportDataGen.
        /// </summary>
		public cdoPayeeOverpaymentReportData icdoPayeeOverpaymentReportData { get; set; }




        /// <summary>
        /// MPIPHP.busPayeeOverpaymentReportDataGen.FindPayeeOverpaymentReportData():
        /// Finds a particular record from cdoPayeeOverpaymentReportData with its primary key. 
        /// </summary>
        /// <param name="aintPayeeOverpaymentReportDataId">A primary key value of type int of cdoPayeeOverpaymentReportData on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeOverpaymentReportData(int aintPayeeOverpaymentReportDataId)
		{
			bool lblnResult = false;
			if (icdoPayeeOverpaymentReportData == null)
			{
				icdoPayeeOverpaymentReportData = new cdoPayeeOverpaymentReportData();
			}
			if (icdoPayeeOverpaymentReportData.SelectRow(new object[1] { aintPayeeOverpaymentReportDataId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
