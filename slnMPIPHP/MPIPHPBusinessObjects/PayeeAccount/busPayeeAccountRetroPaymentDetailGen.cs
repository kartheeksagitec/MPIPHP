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
    /// Class MPIPHP.BusinessObjects.busPayeeAccountRetroPaymentDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountRetroPaymentDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountRetroPaymentDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeAccountRetroPaymentDetailGen
        /// </summary>
		public busPayeeAccountRetroPaymentDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountRetroPaymentDetailGen.
        /// </summary>
		public cdoPayeeAccountRetroPaymentDetail icdoPayeeAccountRetroPaymentDetail { get; set; }




        /// <summary>
        /// MPIPHP.busPayeeAccountRetroPaymentDetailGen.FindPayeeAccountRetroPaymentDetail():
        /// Finds a particular record from cdoPayeeAccountRetroPaymentDetail with its primary key. 
        /// </summary>
        /// <param name="aintPayeeAccountRetroPaymentDetailId">A primary key value of type int of cdoPayeeAccountRetroPaymentDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountRetroPaymentDetail(int aintPayeeAccountRetroPaymentDetailId)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountRetroPaymentDetail == null)
			{
				icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail();
			}
			if (icdoPayeeAccountRetroPaymentDetail.SelectRow(new object[1] { aintPayeeAccountRetroPaymentDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
