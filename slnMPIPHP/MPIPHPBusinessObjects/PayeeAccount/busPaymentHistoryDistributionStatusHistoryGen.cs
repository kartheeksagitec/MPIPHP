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
    /// Class MPIPHP.BusinessObjects.busPaymentHistoryDistributionStatusHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentHistoryDistributionStatusHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentHistoryDistributionStatusHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPaymentHistoryDistributionStatusHistoryGen
        /// </summary>
		public busPaymentHistoryDistributionStatusHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentHistoryDistributionStatusHistoryGen.
        /// </summary>
		public cdoPaymentHistoryDistributionStatusHistory icdoPaymentHistoryDistributionStatusHistory { get; set; }




        /// <summary>
        /// MPIPHP.busPaymentHistoryDistributionStatusHistoryGen.FindPaymentHistoryDistributionStatusHistory():
        /// Finds a particular record from cdoPaymentHistoryDistributionStatusHistory with its primary key. 
        /// </summary>
        /// <param name="aintDistributionStatusHistoryId">A primary key value of type int of cdoPaymentHistoryDistributionStatusHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentHistoryDistributionStatusHistory(int aintDistributionStatusHistoryId)
		{
			bool lblnResult = false;
			if (icdoPaymentHistoryDistributionStatusHistory == null)
			{
				icdoPaymentHistoryDistributionStatusHistory = new cdoPaymentHistoryDistributionStatusHistory();
			}
			if (icdoPaymentHistoryDistributionStatusHistory.SelectRow(new object[1] { aintDistributionStatusHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
