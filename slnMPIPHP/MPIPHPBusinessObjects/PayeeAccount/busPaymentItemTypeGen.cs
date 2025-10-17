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
    /// Class MPIPHP.BusinessObjects.busPaymentItemTypeGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentItemType and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentItemTypeGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPaymentItemTypeGen
        /// </summary>
		public busPaymentItemTypeGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentItemTypeGen.
        /// </summary>
		public cdoPaymentItemType icdoPaymentItemType { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountPaymentItemType. 
        /// </summary>
		public Collection<busPayeeAccountPaymentItemType> iclbPayeeAccountPaymentItemType { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountRetroPaymentDetail. 
        /// </summary>
		public Collection<busPayeeAccountRetroPaymentDetail> iclbPayeeAccountRetroPaymentDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountTaxWithholdingItemDetail. 
        /// </summary>
		public Collection<busPayeeAccountTaxWithholdingItemDetail> iclbPayeeAccountTaxWithholdingItemDetail { get; set; }



        /// <summary>
        /// MPIPHP.busPaymentItemTypeGen.FindPaymentItemType():
        /// Finds a particular record from cdoPaymentItemType with its primary key. 
        /// </summary>
        /// <param name="aintPaymentItemTypeId">A primary key value of type int of cdoPaymentItemType on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentItemType(int aintPaymentItemTypeId)
		{
			bool lblnResult = false;
			if (icdoPaymentItemType == null)
			{
				icdoPaymentItemType = new cdoPaymentItemType();
			}
			if (icdoPaymentItemType.SelectRow(new object[1] { aintPaymentItemTypeId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busPaymentItemTypeGen.LoadPayeeAccountPaymentItemTypes():
        /// Loads Collection object iclbPayeeAccountPaymentItemType of type busPayeeAccountPaymentItemType.
        /// </summary>
		public virtual void LoadPayeeAccountPaymentItemTypes()
		{
			DataTable ldtbList = Select<cdoPayeeAccountPaymentItemType>(
				new string[1] { enmPayeeAccountPaymentItemType.payment_item_type_id.ToString() },
				new object[1] { icdoPaymentItemType.payment_item_type_id }, null, null);
			iclbPayeeAccountPaymentItemType = GetCollection<busPayeeAccountPaymentItemType>(ldtbList, "icdoPayeeAccountPaymentItemType");
		}

        /// <summary>
        /// MPIPHP.busPaymentItemTypeGen.LoadPayeeAccountRetroPaymentDetails():
        /// Loads Collection object iclbPayeeAccountRetroPaymentDetail of type busPayeeAccountRetroPaymentDetail.
        /// </summary>
		public virtual void LoadPayeeAccountRetroPaymentDetails()
		{
			DataTable ldtbList = Select<cdoPayeeAccountRetroPaymentDetail>(
				new string[1] { enmPayeeAccountRetroPaymentDetail.payment_item_type_id.ToString() },
				new object[1] { icdoPaymentItemType.payment_item_type_id }, null, null);
			iclbPayeeAccountRetroPaymentDetail = GetCollection<busPayeeAccountRetroPaymentDetail>(ldtbList, "icdoPayeeAccountRetroPaymentDetail");
		}

        /// <summary>
        /// MPIPHP.busPaymentItemTypeGen.LoadPayeeAccountTaxWithholdingItemDetails():
        /// Loads Collection object iclbPayeeAccountTaxWithholdingItemDetail of type busPayeeAccountTaxWithholdingItemDetail.
        /// </summary>
		public virtual void LoadPayeeAccountTaxWithholdingItemDetails()
		{
			DataTable ldtbList = Select<cdoPayeeAccountTaxWithholdingItemDetail>(
				new string[1] { enmPayeeAccountTaxWithholdingItemDetail.payment_item_type_id.ToString() },
				new object[1] { icdoPaymentItemType.payment_item_type_id }, null, null);
			iclbPayeeAccountTaxWithholdingItemDetail = GetCollection<busPayeeAccountTaxWithholdingItemDetail>(ldtbList, "icdoPayeeAccountTaxWithholdingItemDetail");
		}

	}
}
