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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPayeeAccountRetroPaymentGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountRetroPayment and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountRetroPaymentGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeAccountRetroPaymentGen
        /// </summary>
		public busPayeeAccountRetroPaymentGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountRetroPaymentGen.
        /// </summary>
		public cdoPayeeAccountRetroPayment icdoPayeeAccountRetroPayment { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountRetroPaymentDetail. 
        /// </summary>
		public Collection<busPayeeAccountRetroPaymentDetail> iclbPayeeAccountRetroPaymentDetail { get; set; }



        /// <summary>
        /// MPIPHP.busPayeeAccountRetroPaymentGen.FindPayeeAccountRetroPayment():
        /// Finds a particular record from cdoPayeeAccountRetroPayment with its primary key. 
        /// </summary>
        /// <param name="aintPayeeAccountRetroPaymentId">A primary key value of type int of cdoPayeeAccountRetroPayment on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountRetroPayment(int aintPayeeAccountRetroPaymentId)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountRetroPayment == null)
			{
				icdoPayeeAccountRetroPayment = new cdoPayeeAccountRetroPayment();
			}
			if (icdoPayeeAccountRetroPayment.SelectRow(new object[1] { aintPayeeAccountRetroPaymentId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busPayeeAccountRetroPaymentGen.LoadPayeeAccountRetroPaymentDetails():
        /// Loads Collection object iclbPayeeAccountRetroPaymentDetail of type busPayeeAccountRetroPaymentDetail.
        /// </summary>
		public virtual void LoadPayeeAccountRetroPaymentDetails()
		{
			DataTable ldtbList = Select<cdoPayeeAccountRetroPaymentDetail>(
				new string[1] { enmPayeeAccountRetroPaymentDetail.payee_account_retro_payment_id.ToString() },
				new object[1] { icdoPayeeAccountRetroPayment.payee_account_retro_payment_id }, null, null);
			iclbPayeeAccountRetroPaymentDetail = GetCollection<busPayeeAccountRetroPaymentDetail>(ldtbList, "icdoPayeeAccountRetroPaymentDetail");
		}

	}
}
