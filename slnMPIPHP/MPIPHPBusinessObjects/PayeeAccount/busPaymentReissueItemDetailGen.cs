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
    /// Class MPIPHP.BusinessObjects.busPaymentReissueItemDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentReissueItemDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentReissueItemDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPaymentReissueItemDetailGen
        /// </summary>
		public busPaymentReissueItemDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentReissueItemDetailGen.
        /// </summary>
		public cdoPaymentReissueItemDetail icdoPaymentReissueItemDetail { get; set; }




        /// <summary>
        /// MPIPHP.busPaymentReissueItemDetailGen.FindPaymentReissueItemDetail():
        /// Finds a particular record from cdoPaymentReissueItemDetail with its primary key. 
        /// </summary>
        /// <param name="aintPaymentReissueItemDetailId">A primary key value of type int of cdoPaymentReissueItemDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentReissueItemDetail(int aintPaymentReissueItemDetailId)
		{
			bool lblnResult = false;
			if (icdoPaymentReissueItemDetail == null)
			{
				icdoPaymentReissueItemDetail = new cdoPaymentReissueItemDetail();
			}
			if (icdoPaymentReissueItemDetail.SelectRow(new object[1] { aintPaymentReissueItemDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
