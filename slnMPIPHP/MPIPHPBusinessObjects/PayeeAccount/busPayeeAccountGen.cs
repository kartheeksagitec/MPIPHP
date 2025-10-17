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
    /// Class MPIPHP.BusinessObjects.busPayeeAccountGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccount and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeAccountGen
        /// </summary>
		public busPayeeAccountGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountGen.
        /// </summary>
		public cdoPayeeAccount icdoPayeeAccount { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountAchDetail. 
        /// </summary>
		public Collection<busPayeeAccountAchDetail> iclbPayeeAccountAchDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountPaymentItemType. 
        /// </summary>
		public Collection<busPayeeAccountPaymentItemType> iclbPayeeAccountPaymentItemType { get; set; }

        public Collection<busPersonAddress> iclbParticipantAddress { get; set; }
        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountRetroPayment. 
        /// </summary>
		public Collection<busPayeeAccountRetroPayment> iclbPayeeAccountRetroPayment { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountRolloverDetail. 
        /// </summary>
		public Collection<busPayeeAccountRolloverDetail> iclbPayeeAccountRolloverDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountStatus. 
        /// </summary>
		public Collection<busPayeeAccountStatus> iclbPayeeAccountStatus { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountTaxWithholding. 
        /// </summary>
		public Collection<busPayeeAccountTaxWithholding> iclbPayeeAccountTaxWithholding { get; set; }


        public Collection<busPayeeAccountWireDetail> iclbPayeeAccountWireDetail { get; set; }


        /// <summary>
        /// Gets or sets the Sagitec.Common.utlCollection object of type cdoReemploymentHistory. 
        /// </summary>
        public utlCollection<cdoReemploymentHistory> iclcReemploymentHistory { get; set; }

        /// <summary>
        /// MPIPHP.busPayeeAccountGen.FindPayeeAccount():
        /// Finds a particular record from cdoPayeeAccount with its primary key. 
        /// </summary>
        /// <param name="aintPayeeAccountId">A primary key value of type int of cdoPayeeAccount on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccount(int aintPayeeAccountId)
		{
			bool lblnResult = false;
			if (icdoPayeeAccount == null)
			{
				icdoPayeeAccount = new cdoPayeeAccount();
			}
			if (icdoPayeeAccount.SelectRow(new object[1] { aintPayeeAccountId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busPayeeAccountGen.LoadPayeeAccountAchDetails():
        /// Loads Collection object iclbPayeeAccountAchDetail of type busPayeeAccountAchDetail.
        /// </summary>
		public virtual void LoadPayeeAccountAchDetails()
		{
			DataTable ldtbList = Select<cdoPayeeAccountAchDetail>(
				new string[1] { enmPayeeAccountAchDetail.payee_account_id.ToString() },
				new object[1] { icdoPayeeAccount.payee_account_id }, null, null);
			iclbPayeeAccountAchDetail = GetCollection<busPayeeAccountAchDetail>(ldtbList, "icdoPayeeAccountAchDetail");
		}

        /// <summary>
        /// MPIPHP.busPayeeAccountGen.LoadPayeeAccountPaymentItemTypes():
        /// Loads Collection object iclbPayeeAccountPaymentItemType of type busPayeeAccountPaymentItemType.
        /// </summary>
		public virtual void LoadPayeeAccountPaymentItemTypes()
		{
			DataTable ldtbList = Select<cdoPayeeAccountPaymentItemType>(
				new string[1] { enmPayeeAccountPaymentItemType.payee_account_id.ToString() },
				new object[1] { icdoPayeeAccount.payee_account_id }, null, null);
			iclbPayeeAccountPaymentItemType = GetCollection<busPayeeAccountPaymentItemType>(ldtbList, "icdoPayeeAccountPaymentItemType");
		}

        /// <summary>
        /// MPIPHP.busPayeeAccountGen.LoadPayeeAccountRetroPayments():
        /// Loads Collection object iclbPayeeAccountRetroPayment of type busPayeeAccountRetroPayment.
        /// </summary>
        //public virtual void LoadPayeeAccountRetroPayments()
        //{
        //    DataTable ldtbList = Select<cdoPayeeAccountRetroPayment>(
        //        new string[1] { enmPayeeAccountRetroPayment.payee_account_id.ToString() },
        //        new object[1] { icdoPayeeAccount.payee_account_id }, null, null);
        //    iclbPayeeAccountRetroPayment = GetCollection<busPayeeAccountRetroPayment>(ldtbList, "icdoPayeeAccountRetroPayment");
        //}

        /// <summary>
        /// MPIPHP.busPayeeAccountGen.LoadPayeeAccountRolloverDetails():
        /// Loads Collection object iclbPayeeAccountRolloverDetail of type busPayeeAccountRolloverDetail.
        /// </summary>
		public virtual void LoadPayeeAccountRolloverDetails()
		{
			DataTable ldtbList = Select<cdoPayeeAccountRolloverDetail>(
				new string[1] { enmPayeeAccountRolloverDetail.payee_account_id.ToString() },
				new object[1] { icdoPayeeAccount.payee_account_id }, null, null);
			iclbPayeeAccountRolloverDetail = GetCollection<busPayeeAccountRolloverDetail>(ldtbList, "icdoPayeeAccountRolloverDetail");
		}

        /// <summary>
        /// MPIPHP.busPayeeAccountGen.LoadPayeeAccountStatuss():
        /// Loads Collection object iclbPayeeAccountStatus of type busPayeeAccountStatus.
        /// </summary>
		public virtual void LoadPayeeAccountStatuss()
		{
			DataTable ldtbList = Select<cdoPayeeAccountStatus>(
				new string[1] { enmPayeeAccountStatus.payee_account_id.ToString() },
                new object[1] { icdoPayeeAccount.payee_account_id }, null, "status_effective_date desc,payee_account_status_id desc");
			iclbPayeeAccountStatus = GetCollection<busPayeeAccountStatus>(ldtbList, "icdoPayeeAccountStatus");
		}

        /// <summary>
        /// MPIPHP.busPayeeAccountGen.LoadPayeeAccountTaxWithholdings():
        /// Loads Collection object iclbPayeeAccountTaxWithholding of type busPayeeAccountTaxWithholding.
        /// </summary>
		public virtual void LoadPayeeAccountTaxWithholdings()
		{
			DataTable ldtbList = Select<cdoPayeeAccountTaxWithholding>(
				new string[1] { enmPayeeAccountTaxWithholding.payee_account_id.ToString() },
				new object[1] { icdoPayeeAccount.payee_account_id }, null, null);
			iclbPayeeAccountTaxWithholding = GetCollection<busPayeeAccountTaxWithholding>(ldtbList, "icdoPayeeAccountTaxWithholding");
		}

        public virtual void LoadPayeeAccountWireDetail()
        {
            DataTable ldtbList = Select<doPayeeAccountWireDetail>(
                new string[1] { enmPayeeAccountWireDetail.payee_account_id.ToString() },
                new object[1] { icdoPayeeAccount.payee_account_id }, null, null);
            iclbPayeeAccountWireDetail = GetCollection<busPayeeAccountWireDetail>(ldtbList, "icdoPayeeAccountWireDetail");
        }



        /// <summary>
        ///  MPIPHP.busPayeeAccountGen.LoadReemploymentHistorys():
        /// Loads Sagitec.Common.utlCollection object iclcReemploymentHistory of type cdoReemploymentHistory.
        /// </summary>
        public virtual void LoadReemploymentHistorys()
        {
            iclcReemploymentHistory = GetCollection<cdoReemploymentHistory>(
                new string[1] { enmReemploymentHistory.payee_account_id.ToString() },
                new object[1] { icdoPayeeAccount.payee_account_id }, null, enmReemploymentHistory.reemployed_flag_from_date.ToString());
        }

	}
}
