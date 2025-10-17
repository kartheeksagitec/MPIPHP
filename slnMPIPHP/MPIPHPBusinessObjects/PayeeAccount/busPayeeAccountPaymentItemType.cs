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
	/// Class MPIPHP.BusinessObjects.busPayeeAccountPaymentItemType:
	/// Inherited from busPayeeAccountPaymentItemTypeGen, the class is used to customize the business object busPayeeAccountPaymentItemTypeGen.
	/// </summary>
	[Serializable]
	public class busPayeeAccountPaymentItemType : busPayeeAccountPaymentItemTypeGen
	{
        public busPaymentItemType ibusPaymentItemType { get; set; }
        
        public busOrganization ibusVendor { get; set; }

        #region Load Payment Item Type
       
        public void LoadPaymentItemType()
        {
            if (ibusPaymentItemType == null)
                ibusPaymentItemType = new busPaymentItemType();
            ibusPaymentItemType.FindPaymentItemType(icdoPayeeAccountPaymentItemType.payment_item_type_id);
        }

        public int LoadAmountItemTypeIdAndPayeeAccountId(int aintPayeeAccountId)
        {
            DataTable ldtbList = Select<cdoPayeeAccountPaymentItemType>(
                new string[2] { enmPayeeAccountPaymentItemType.payee_account_id.ToString(), enmPayeeAccountPaymentItemType.payment_item_type_id.ToString() },
                new object[2] { aintPayeeAccountId, 48 }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                return Convert.ToInt32(ldtbList.Rows[0][enmPayeeAccountPaymentItemType.amount.ToString()]);
            }
            else
            {
                return 0;
            }
            
        }
        #endregion
    }
}
