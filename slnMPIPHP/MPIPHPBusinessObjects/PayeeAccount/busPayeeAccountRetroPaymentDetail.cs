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
using System.Linq;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPayeeAccountRetroPaymentDetail:
	/// Inherited from busPayeeAccountRetroPaymentDetailGen, the class is used to customize the business object busPayeeAccountRetroPaymentDetailGen.
	/// </summary>
	[Serializable]
	public class busPayeeAccountRetroPaymentDetail : busPayeeAccountRetroPaymentDetailGen
	{
        public busPaymentItemType ibusPaymentItemType { get; set; }
        public string PaymentItemTypedesription { get; set; }
        public busPaymentItemType ibusOriginalPaymentItemType { get; set; }
        
        #region Public Methods

        #region Load Payment Item Type
        public void LoadPaymentItemType()
        {
            if (ibusPaymentItemType == null)
                ibusPaymentItemType = new busPaymentItemType();
            ibusPaymentItemType.FindPaymentItemType(icdoPayeeAccountRetroPaymentDetail.payment_item_type_id);
        }
        #endregion

        #region Load Original Payment ItemType
        public void LoadOriginalPaymentItemType()
        {
            if (ibusOriginalPaymentItemType == null)
                ibusOriginalPaymentItemType = new busPaymentItemType();
            ibusOriginalPaymentItemType.FindPaymentItemType(icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id);
        }
        #endregion

        public void CreatePayeeAccountRetroPaymentDetail(int aintPayeeAccountRetroPaymentId, busPaymentHistoryDetail abusPaymentHistoryDetail)
        {
            if (icdoPayeeAccountRetroPaymentDetail == null)
            {
                icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail();
            }

            icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_id = aintPayeeAccountRetroPaymentId;
            icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = abusPaymentHistoryDetail.icdoPaymentHistoryDetail.payment_item_type_id;
            icdoPayeeAccountRetroPaymentDetail.amount = abusPaymentHistoryDetail.icdoPaymentHistoryDetail.amount;
            icdoPayeeAccountRetroPaymentDetail.Insert();
        }

        public int CreatePayeeAccountRetroPaymentDetail(int aintPayeeAccountRetroPaymentId, int aintPaymentItemId, int iintPayeeAccountPaymentItemTypeId,
                                                        Decimal adecAmount, int aintVendorOrgId, int aintOriginalPaymentTypeId,
                                                        Collection<busPayeeAccountRetroPayment> aclbPayeeAccountRetroPayment)
        {
            if (aclbPayeeAccountRetroPayment != null && aclbPayeeAccountRetroPayment.Count > 0 && adecAmount > 0)
            {
                busPayeeAccountRetroPayment ibojbusPayeeAccountRetroPayment = aclbPayeeAccountRetroPayment.FirstOrDefault();
                busPayeeAccountRetroPaymentDetail iobjbusPayeeAccountRetroPaymentDetail = (from obj in ibojbusPayeeAccountRetroPayment.iclbPayeeAccountRetroPaymentDetail.AsEnumerable()
                                                                                           where obj.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id == aintPaymentItemId
                                                                                           select obj).FirstOrDefault();
                if (iobjbusPayeeAccountRetroPaymentDetail != null)
                {
                    iobjbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_id = aintPayeeAccountRetroPaymentId;
                    iobjbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = aintPaymentItemId;
                    iobjbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount = adecAmount;
                    iobjbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.vendor_org_id = aintVendorOrgId;
                    iobjbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = aintOriginalPaymentTypeId;
                    iobjbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payee_account_payment_item_type_id = iintPayeeAccountPaymentItemTypeId;
                    iobjbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Update();
                    return iobjbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_detail_id;
                }
                else if (icdoPayeeAccountRetroPaymentDetail == null && adecAmount > 0)
                {
                    icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail();

                    icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_id = aintPayeeAccountRetroPaymentId;
                    icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = aintPaymentItemId;
                    icdoPayeeAccountRetroPaymentDetail.amount = adecAmount;
                    icdoPayeeAccountRetroPaymentDetail.vendor_org_id = aintVendorOrgId;
                    icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = aintOriginalPaymentTypeId;
                    icdoPayeeAccountRetroPaymentDetail.payee_account_payment_item_type_id = iintPayeeAccountPaymentItemTypeId;
                    icdoPayeeAccountRetroPaymentDetail.Insert();
                    return icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_detail_id;
                }

            }
            else if (icdoPayeeAccountRetroPaymentDetail == null && adecAmount > 0)
            {
                icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail();

                icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_id = aintPayeeAccountRetroPaymentId;
                icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = aintPaymentItemId;
                icdoPayeeAccountRetroPaymentDetail.amount = adecAmount;
                icdoPayeeAccountRetroPaymentDetail.vendor_org_id = aintVendorOrgId;
                icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = aintOriginalPaymentTypeId;
                icdoPayeeAccountRetroPaymentDetail.payee_account_payment_item_type_id = iintPayeeAccountPaymentItemTypeId;
                icdoPayeeAccountRetroPaymentDetail.Insert();
                return icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_detail_id;
            }
           

            return 0;
        }

        #endregion
    }
}
