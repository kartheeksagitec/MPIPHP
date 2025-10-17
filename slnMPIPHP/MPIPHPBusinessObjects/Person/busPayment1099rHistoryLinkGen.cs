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
    /// Class MPIPHP.BusinessObjects.busPayment1099rHistoryLinkGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayment1099rHistoryLink and its children table. 
    /// </summary>
	[Serializable]
	public class busPayment1099rHistoryLinkGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayment1099rHistoryLinkGen
        /// </summary>
		public busPayment1099rHistoryLinkGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayment1099rHistoryLinkGen.
        /// </summary>
		public cdoPayment1099rHistoryLink icdoPayment1099rHistoryLink { get; set; }




        /// <summary>
        /// MPIPHP.busPayment1099rHistoryLinkGen.FindPayment1099rHistoryLink():
        /// Finds a particular record from cdoPayment1099rHistoryLink with its primary key. 
        /// </summary>
        /// <param name="aintpayment1099rhistorylinkid">A primary key value of type int of cdoPayment1099rHistoryLink on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayment1099rHistoryLink(int aintpayment1099rhistorylinkid)
		{
			bool lblnResult = false;
			if (icdoPayment1099rHistoryLink == null)
			{
				icdoPayment1099rHistoryLink = new cdoPayment1099rHistoryLink();
			}
			if (icdoPayment1099rHistoryLink.SelectRow(new object[1] { aintpayment1099rhistorylinkid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
