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
    /// Class MPIPHP.BusinessObjects.busPaymentReissueDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentReissueDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentReissueDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPaymentReissueDetailGen
        /// </summary>
		public busPaymentReissueDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentReissueDetailGen.
        /// </summary>
		public cdoPaymentReissueDetail icdoPaymentReissueDetail { get; set; }




        /// <summary>
        /// MPIPHP.busPaymentReissueDetailGen.FindPaymentReissueDetail():
        /// Finds a particular record from cdoPaymentReissueDetail with its primary key. 
        /// </summary>
        /// <param name="aintPaymentReissueDetailId">A primary key value of type int of cdoPaymentReissueDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentReissueDetail(int aintPaymentReissueDetailId)
		{
			bool lblnResult = false;
			if (icdoPaymentReissueDetail == null)
			{
				icdoPaymentReissueDetail = new cdoPaymentReissueDetail();
			}
			if (icdoPaymentReissueDetail.SelectRow(new object[1] { aintPaymentReissueDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
