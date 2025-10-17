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
	/// Class MPIPHP.BusinessObjects.busBenefitMonthwiseAdjustmentDetail:
	/// Inherited from busBenefitMonthwiseAdjustmentDetailGen, the class is used to customize the business object busBenefitMonthwiseAdjustmentDetailGen.
	/// </summary>
	[Serializable]
	public class busBenefitMonthwiseAdjustmentDetail : busBenefitMonthwiseAdjustmentDetailGen
    {
        #region Public Methods

        public void CreateMonthwiseAdjustmentDetail(int aintPayeeAccountRetroPaymentId, DateTime adtPaymentDate, decimal adecTaxableAmtToBePaid, decimal adecNonTaxableAmtToBePaid,
                                                    decimal adecTaxableAmtPaid, decimal adecNonTaxableAmtPaid, decimal adecHours, string astrSuspendedFlag, int aintPaymentHistoryHeaderId, decimal ldecAmountRepaid = 0M)
        {
            if (icdoBenefitMonthwiseAdjustmentDetail == null)
            {
                icdoBenefitMonthwiseAdjustmentDetail = new cdoBenefitMonthwiseAdjustmentDetail();
            }

            icdoBenefitMonthwiseAdjustmentDetail.payee_account_retro_payment_id = aintPayeeAccountRetroPaymentId;
            icdoBenefitMonthwiseAdjustmentDetail.payment_date = adtPaymentDate;
            icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = adecTaxableAmtToBePaid;
            icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = adecNonTaxableAmtToBePaid;
            icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid = adecTaxableAmtPaid;
            icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid = adecNonTaxableAmtPaid;
            icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_difference = adecTaxableAmtToBePaid - (adecTaxableAmtPaid - ldecAmountRepaid);
            icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_difference = adecNonTaxableAmtToBePaid - adecNonTaxableAmtPaid;
            icdoBenefitMonthwiseAdjustmentDetail.hours = adecHours;
            icdoBenefitMonthwiseAdjustmentDetail.suspended_flag = astrSuspendedFlag;
            icdoBenefitMonthwiseAdjustmentDetail.payment_history_header_id = aintPaymentHistoryHeaderId;
            icdoBenefitMonthwiseAdjustmentDetail.Insert();
        }

        public busBenefitMonthwiseAdjustmentDetail FillMonthWiseAdjustmentDetail(DateTime adtPaymentDate, decimal adecTaxableAmtToBePaid, decimal adecNonTaxableAmtToBePaid,
                                                    decimal adecTaxableAmtPaid, decimal adecNonTaxableAmtPaid, decimal adecHours, string astrSuspendedFlag,int aintPaymentHistoryHeaderId)
        {

           // busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail = new busBenefitMonthwiseAdjustmentDetail { icdoBenefitMonthwiseAdjustmentDetail = new cdoBenefitMonthwiseAdjustmentDetail() };

            icdoBenefitMonthwiseAdjustmentDetail.payment_date = adtPaymentDate;
            icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = adecTaxableAmtToBePaid;
            icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = adecNonTaxableAmtToBePaid;
            icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid = adecTaxableAmtPaid;
            icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid = adecNonTaxableAmtPaid;
            icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_difference = adecTaxableAmtToBePaid - adecTaxableAmtPaid;
            icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_difference = adecNonTaxableAmtToBePaid - adecNonTaxableAmtPaid;
            icdoBenefitMonthwiseAdjustmentDetail.hours = adecHours;
            icdoBenefitMonthwiseAdjustmentDetail.suspended_flag = astrSuspendedFlag;
            icdoBenefitMonthwiseAdjustmentDetail.payment_history_header_id = aintPaymentHistoryHeaderId;

            return this;
        }

        public bool CheckIfManager() //PIR 1055
        {
            busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
            if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
