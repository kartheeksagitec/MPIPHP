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
    /// Class MPIPHP.BusinessObjects.busPayeeAccountPaymentItemTypeGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountPaymentItemType and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountPaymentItemTypeGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeAccountPaymentItemTypeGen
        /// </summary>
		public busPayeeAccountPaymentItemTypeGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountPaymentItemTypeGen.
        /// </summary>
		public cdoPayeeAccountPaymentItemType icdoPayeeAccountPaymentItemType { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountRolloverItemDetail. 
        /// </summary>
		public Collection<busPayeeAccountRolloverItemDetail> iclbPayeeAccountRolloverItemDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountTaxWithholdingItemDetail. 
        /// </summary>
		public Collection<busPayeeAccountTaxWithholdingItemDetail> iclbPayeeAccountTaxWithholdingItemDetail { get; set; }



        /// <summary>
        /// MPIPHP.busPayeeAccountPaymentItemTypeGen.FindPayeeAccountPaymentItemType():
        /// Finds a particular record from cdoPayeeAccountPaymentItemType with its primary key. 
        /// </summary>
        /// <param name="aintPayeeAccountPaymentItemTypeId">A primary key value of type int of cdoPayeeAccountPaymentItemType on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountPaymentItemType(int aintPayeeAccountPaymentItemTypeId)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountPaymentItemType == null)
			{
				icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
			}
			if (icdoPayeeAccountPaymentItemType.SelectRow(new object[1] { aintPayeeAccountPaymentItemTypeId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busPayeeAccountPaymentItemTypeGen.LoadPayeeAccountRolloverItemDetails():
        /// Loads Collection object iclbPayeeAccountRolloverItemDetail of type busPayeeAccountRolloverItemDetail.
        /// </summary>
		public virtual void LoadPayeeAccountRolloverItemDetails()
		{
			DataTable ldtbList = Select<cdoPayeeAccountRolloverItemDetail>(
				new string[1] { enmPayeeAccountRolloverItemDetail.payee_account_payment_item_type_id.ToString() },
				new object[1] { icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id }, null, null);
			iclbPayeeAccountRolloverItemDetail = GetCollection<busPayeeAccountRolloverItemDetail>(ldtbList, "icdoPayeeAccountRolloverItemDetail");
		}

        /// <summary>
        /// MPIPHP.busPayeeAccountPaymentItemTypeGen.LoadPayeeAccountTaxWithholdingItemDetails():
        /// Loads Collection object iclbPayeeAccountTaxWithholdingItemDetail of type busPayeeAccountTaxWithholdingItemDetail.
        /// </summary>
		public virtual void LoadPayeeAccountTaxWithholdingItemDetails()
		{
			DataTable ldtbList = Select<cdoPayeeAccountTaxWithholdingItemDetail>(
				new string[1] { enmPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id.ToString() },
				new object[1] { icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id }, null, null);
			iclbPayeeAccountTaxWithholdingItemDetail = GetCollection<busPayeeAccountTaxWithholdingItemDetail>(ldtbList, "icdoPayeeAccountTaxWithholdingItemDetail");
		}

	}
}
