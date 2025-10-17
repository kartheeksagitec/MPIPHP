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
    /// Class MPIPHP.BusinessObjects.busPaymentHistoryDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentHistoryDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentHistoryDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPaymentHistoryDetailGen
        /// </summary>
		public busPaymentHistoryDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentHistoryDetailGen.
        /// </summary>
		public cdoPaymentHistoryDetail icdoPaymentHistoryDetail { get; set; }




        /// <summary>
        /// MPIPHP.busPaymentHistoryDetailGen.FindPaymentHistoryDetail():
        /// Finds a particular record from cdoPaymentHistoryDetail with its primary key. 
        /// </summary>
        /// <param name="aintPaymentHistoryDetailId">A primary key value of type int of cdoPaymentHistoryDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentHistoryDetail(int aintPaymentHistoryDetailId)
		{
			bool lblnResult = false;
			if (icdoPaymentHistoryDetail == null)
			{
				icdoPaymentHistoryDetail = new cdoPaymentHistoryDetail();
			}
			if (icdoPaymentHistoryDetail.SelectRow(new object[1] { aintPaymentHistoryDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
