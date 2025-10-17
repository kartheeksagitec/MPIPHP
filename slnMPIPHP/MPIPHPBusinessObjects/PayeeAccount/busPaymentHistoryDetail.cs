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
	/// Class MPIPHP.BusinessObjects.busPaymentHistoryDetail:
	/// Inherited from busPaymentHistoryDetailGen, the class is used to customize the business object busPaymentHistoryDetailGen.
	/// </summary>
	[Serializable]
	public class busPaymentHistoryDetail : busPaymentHistoryDetailGen
	{
        public busPaymentItemType ibusPaymentItemType { get; set; }

        #region Load Payment Item Type
        public void LoadPaymentItemType()
        {
            if (ibusPaymentItemType == null)
                ibusPaymentItemType = new busPaymentItemType();
            ibusPaymentItemType.FindPaymentItemType(icdoPaymentHistoryDetail.payment_item_type_id);
        }
        #endregion
	}
}
