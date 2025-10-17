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
using Sagitec.DataObjects;
#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPayeeAccountRetroPayment:
	/// Inherited from busPayeeAccountRetroPaymentGen, the class is used to customize the business object busPayeeAccountRetroPaymentGen.
	/// </summary>
	[Serializable]
	public class busPayeeAccountRetroPayment : busPayeeAccountRetroPaymentGen
	{

        public busPayeeAccountRetroPayment()
        {
            if (this.ibusCalculation.IsNull())
            {
                this.ibusCalculation = new busCalculation();
            }
        }

        public busRepaymentSchedule ibusRepaymentSchedule { get; set; }
        public bool iblnOverriddenAmountChanged { get; set; }
        public busCalculation ibusCalculation { get; set; }
        public busPayeeAccount ibusPayeeAccount { get; set; }
        public Collection<busBenefitMonthwiseAdjustmentDetail> iclbBenefitMonthwiseAdjustmentDetail { get; set; }
        public busPayeeAccountPaymentItemType ibusPayeeAccountPaymentItemType { get; set; }
        //public Collection<bus

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            iblnOverriddenAmountChanged = false;
            if (!this.iclbBenefitMonthwiseAdjustmentDetail.IsNullOrEmpty())
            {
                if (!this.iarrChangeLog.Contains(this.icdoPayeeAccountRetroPayment))
                {
                    this.icdoPayeeAccountRetroPayment.ienuObjectState = ObjectState.Update;
                    this.iarrChangeLog.Add(this.icdoPayeeAccountRetroPayment);
                }


                //RID 83533
                string lstrOldPaymentOption = string.Empty;
                if (Convert.ToString(icdoPayeeAccountRetroPayment.ihstOldValues["payment_option_value"]) != string.Empty)
                {
                    lstrOldPaymentOption = Convert.ToString(icdoPayeeAccountRetroPayment.ihstOldValues["payment_option_value"]);
                }
                if (this.icdoPayeeAccountRetroPayment.payment_option_value != string.Empty)
                {
                    if ((this.icdoPayeeAccountRetroPayment.payment_option_value == "SPCK" && lstrOldPaymentOption == "REGL")
                        || (this.icdoPayeeAccountRetroPayment.payment_option_value == "REGL" && lstrOldPaymentOption == "SPCK"))
                    {
                        //Change payee account to review status
                        if (this.ibusPayeeAccount.iclbPayeeAccountStatus == null)
                            this.ibusPayeeAccount.LoadPayeeAccountStatuss();

                        if (this.ibusPayeeAccount.iclbPayeeAccountStatus != null && this.ibusPayeeAccount.iclbPayeeAccountStatus.Count > 0)
                        {
                            if (this.ibusPayeeAccount.iclbPayeeAccountStatus.FirstOrDefault().icdoPayeeAccountStatus.status_value == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED
                                || this.ibusPayeeAccount.iclbPayeeAccountStatus.FirstOrDefault().icdoPayeeAccountStatus.status_value == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING)
                            {
                                this.ibusPayeeAccount.CreateReviewPayeeAccountStatus();
                            }
                        }

                    }
                }

                iblnOverriddenAmountChanged = true;
                if (iclbBenefitMonthwiseAdjustmentDetail != null && iclbBenefitMonthwiseAdjustmentDetail.Count > 0)
                {
                    this.icdoPayeeAccountRetroPayment.gross_payment_amount = 0M;
                    this.icdoPayeeAccountRetroPayment.net_payment_amount = 0M;
                    decimal ldecTaxableAmount = 0M;
                    decimal ldecNonTaxableAmount = 0M;

                    foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in iclbBenefitMonthwiseAdjustmentDetail)
                    {
                        if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.override_flag == busConstant.FLAG_YES)
                        {
                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_difference = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.overriden_taxable_amount - (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid - lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.amount_repaid);
                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_difference = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.overriden_non_taxable_amount - lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid;

                            ldecTaxableAmount += lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_difference;
                            ldecNonTaxableAmount += lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_difference;
                        }
                        else
                        {
                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.overriden_taxable_amount = decimal.Zero;
                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.overriden_non_taxable_amount = decimal.Zero;

                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_difference = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid - (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid - lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.amount_repaid);
                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_difference = lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid - lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid;

                            ldecTaxableAmount += lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_difference;
                            ldecNonTaxableAmount += lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_difference;
                        }
                    }

                   
                    this.icdoPayeeAccountRetroPayment.gross_payment_amount = ldecTaxableAmount + ldecNonTaxableAmount;
                    this.icdoPayeeAccountRetroPayment.net_payment_amount = ldecTaxableAmount + ldecNonTaxableAmount;

                    if (iclbPayeeAccountRetroPaymentDetail != null && iclbPayeeAccountRetroPaymentDetail.Count > 0)
                    {

                        if (ldecTaxableAmount > 0 && (iclbPayeeAccountRetroPaymentDetail.Where(t => t.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                                    t.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.FLAG_YES).Count() == 0))
                        {
                            busPayeeAccountRetroPaymentDetail lbusPayeeAccountRetroPaymentDetail = new busPayeeAccountRetroPaymentDetail { icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail() };
                            lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_id = icdoPayeeAccountRetroPayment.payee_account_retro_payment_id;
                            lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = 7;
                            lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = 1;
                            iclbPayeeAccountRetroPaymentDetail.Add(lbusPayeeAccountRetroPaymentDetail);

                        }

                        if (ldecNonTaxableAmount > 0 && (iclbPayeeAccountRetroPaymentDetail.Where(t => t.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                                    t.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.FLAG_NO).Count() == 0))
                        {

                            busPayeeAccountRetroPaymentDetail lbusPayeeAccountRetroPaymentDetail = new busPayeeAccountRetroPaymentDetail { icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail() };
                            lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_id = icdoPayeeAccountRetroPayment.payee_account_retro_payment_id;
                            lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = 8;
                            lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = 2;
                            lbusPayeeAccountRetroPaymentDetail.LoadPaymentItemType();
                            iclbPayeeAccountRetroPaymentDetail.Add(lbusPayeeAccountRetroPaymentDetail);
                        }


                        foreach (busPayeeAccountRetroPaymentDetail lbusPayeeAccountRetroPaymentDetail in iclbPayeeAccountRetroPaymentDetail)
                        {
                           if (lbusPayeeAccountRetroPaymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1)
                            {
                                if (lbusPayeeAccountRetroPaymentDetail.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.FLAG_YES)
                                {
                                    if (ldecNonTaxableAmount < decimal.Zero || ldecTaxableAmount < decimal.Zero)
                                    {
                                        lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount = ldecTaxableAmount + ldecNonTaxableAmount;
                                    }
                                    else
                                    {
                                        lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount = ldecTaxableAmount;
                                    }
                                }
                                else
                                {
                                    if (ldecNonTaxableAmount < decimal.Zero || ldecTaxableAmount < decimal.Zero)
                                    {
                                        lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount = decimal.Zero;
                                    }
                                    else
                                    {
                                        lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount = ldecNonTaxableAmount;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if(this.icdoPayeeAccountRetroPayment.gross_payment_amount < decimal.Zero)
            {
                this.icdoPayeeAccountRetroPayment.is_overpayment_flag = busConstant.FLAG_YES;
                this.icdoPayeeAccountRetroPayment.gross_payment_amount = Math.Abs(this.icdoPayeeAccountRetroPayment.gross_payment_amount);
                this.icdoPayeeAccountRetroPayment.net_payment_amount = Math.Abs(this.icdoPayeeAccountRetroPayment.net_payment_amount);
            }
            else
            {
                this.icdoPayeeAccountRetroPayment.is_overpayment_flag = busConstant.FLAG_NO;
            }

            ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            foreach (busPayeeAccountRetroPaymentDetail lobjPayeeAccountRetroPaymentDetail in iclbPayeeAccountRetroPaymentDetail)
            {
                if (lobjPayeeAccountRetroPaymentDetail.ibusOriginalPaymentItemType == null)
                    lobjPayeeAccountRetroPaymentDetail.LoadOriginalPaymentItemType();
                if (ibusPayeeAccount.iclbRetroItemType == null)
                    ibusPayeeAccount.LoadRetroItemType();
                busRetroItemType lobjRetroItemType = ibusPayeeAccount.iclbRetroItemType.Where(o =>
                    o.icdoRetroItemType.from_item_type == lobjPayeeAccountRetroPaymentDetail.ibusOriginalPaymentItemType.icdoPaymentItemType.item_type_code &&
                    o.icdoRetroItemType.payment_option_value == icdoPayeeAccountRetroPayment.payment_option_value).FirstOrDefault();
                if (lobjRetroItemType != null)
                {
                    if (ibusPayeeAccount.iclbPaymentItemType == null)
                        ibusPayeeAccount.LoadPaymentItemType();
                    busPaymentItemType lobjPaymentItemType = ibusPayeeAccount.iclbPaymentItemType.Where(o =>
                        o.icdoPaymentItemType.item_type_code == lobjRetroItemType.icdoRetroItemType.to_item_type).FirstOrDefault();
                    if (lobjPaymentItemType != null)
                    {
                        //rid 81331
                        string lstrItemTypecode = this.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RETRO_PAYMENT_REACTIVATION ? GetItemTypeCode(lobjPayeeAccountRetroPaymentDetail, lobjPaymentItemType) : lobjPaymentItemType.icdoPaymentItemType.item_type_code;
                        int aintPayeeAccountPaymentItemTypeId = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lstrItemTypecode,
                        lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount, string.Empty,
                        lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.vendor_org_id,
                        icdoPayeeAccountRetroPayment.start_date, icdoPayeeAccountRetroPayment.end_date, busConstant.FLAG_NO, false, false,
                        lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payee_account_payment_item_type_id);                                             

                        if (aintPayeeAccountPaymentItemTypeId != 0)
                        {
                            busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };

                            if (this.icdoPayeeAccountRetroPayment.is_overpayment_flag == busConstant.FLAG_YES)
                            {
                                lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id;
                                lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payee_account_payment_item_type_id = 0;
                                lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Update();

                                if (lbusPayeeAccountPaymentItemType.FindPayeeAccountPaymentItemType(aintPayeeAccountPaymentItemTypeId))
                                {
                                    lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Delete();
                                }
                            }
                            else
                            {
                                lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id;
                                lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payee_account_payment_item_type_id = aintPayeeAccountPaymentItemTypeId;
                                if (lbusPayeeAccountPaymentItemType.FindPayeeAccountPaymentItemType(aintPayeeAccountPaymentItemTypeId))
                                {
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount = lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount;
                                }

                                if (lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_detail_id == 0)
                                {
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Insert();
                                } //rid 81331
                                else if (this.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RETRO_PAYMENT_REACTIVATION && this.icdoPayeeAccountRetroPayment.payment_option_value == "SPCK" &&
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id == 13)
                                {
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = 39;
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Update();
                                }
                                else if (this.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RETRO_PAYMENT_REACTIVATION && this.icdoPayeeAccountRetroPayment.payment_option_value == "REGL" &&
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id == 13)
                                {
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = 19;
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Update();
                                }
                                else if (this.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RETRO_PAYMENT_REACTIVATION && icdoPayeeAccountRetroPayment.payment_option_value == "SPCK" &&
                                   lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id == 14)
                                {
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = 40;
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Update();
                                }
                                else if (this.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RETRO_PAYMENT_REACTIVATION && this.icdoPayeeAccountRetroPayment.payment_option_value == "REGL" &&
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id == 14)
                                {
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = 20;
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Update();
                                }
                                else
                                {
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Update();
                                }

                            }
                        }
                        
                    }
                }
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (this.iblnOverriddenAmountChanged)
            {
                this.ibusPayeeAccount.LoadNextBenefitPaymentDate();
                if (this.icdoPayeeAccountRetroPayment.end_date > this.ibusPayeeAccount.idtNextBenefitPaymentDate)
                {
                    decimal ldecCummulativeAmount = decimal.Zero;
                    foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in this.iclbBenefitMonthwiseAdjustmentDetail)
                    {
                        ldecCummulativeAmount += lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_difference + lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_difference;
                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecMonthlyCummulativeTillDate = ldecCummulativeAmount;
                    }

                    if (icdoPayeeAccountRetroPayment.is_overpayment_flag != busConstant.FLAG_YES)
                    {
                        //PIR 945
                        this.ibusPayeeAccount.LoadPayeeAccountRetroPayments();
                        this.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                        this.ibusPayeeAccount.ProcessTaxWithHoldingDetails();
                    }
                }
            }

            //Reloading the object.
            this.FindPayeeAccountRetroPayment(this.icdoPayeeAccountRetroPayment.payee_account_retro_payment_id);
            this.LoadPayeeAccountRetroPaymentDetails();
            if (this.iclbPayeeAccountRetroPaymentDetail != null)
            {
                foreach (busPayeeAccountRetroPaymentDetail lobjPayeeAccountRetroPaymentDetail in this.iclbPayeeAccountRetroPaymentDetail)
                {
                    lobjPayeeAccountRetroPaymentDetail.LoadPaymentItemType();
                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.istrPaymentItemTypedesription = lobjPayeeAccountRetroPaymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_description;
                }
            }
            this.LoadBenefitMonthwiseAdjustmentDetails();

            if(icdoPayeeAccountRetroPayment.is_overpayment_flag == busConstant.FLAG_YES)
            {
                
                    iclbPayeeAccountRetroPaymentDetail = new Collection<busPayeeAccountRetroPaymentDetail>();
                    iclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                    icdoPayeeAccountRetroPayment.gross_payment_amount = decimal.Zero;
                    icdoPayeeAccountRetroPayment.net_payment_amount = decimal.Zero;
                
            }

        }

        public void LoadBenefitMonthwiseAdjustmentDetails()
        {
            DataTable ldtbBenefitMonthwiseAdjustmentDetails = Select("cdoPayeeAccountRetroPayment.LoadBenefitMonthwiseAdjustmentDetails", new object[1] { icdoPayeeAccountRetroPayment.payee_account_retro_payment_id });
            

            if (ldtbBenefitMonthwiseAdjustmentDetails.Rows.Count > 0)
            {
                iclbBenefitMonthwiseAdjustmentDetail = GetCollection<busBenefitMonthwiseAdjustmentDetail>(ldtbBenefitMonthwiseAdjustmentDetails, "icdoBenefitMonthwiseAdjustmentDetail");
                iclbBenefitMonthwiseAdjustmentDetail = iclbBenefitMonthwiseAdjustmentDetail.OrderBy(t => t.icdoBenefitMonthwiseAdjustmentDetail.benefit_monthwise_adjustment_detail_id).ToList().ToCollection();//PIR 1035
            }
            decimal ldecCummulativeAmount = decimal.Zero;
            if (!iclbBenefitMonthwiseAdjustmentDetail.IsNullOrEmpty())
            {
                foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in this.iclbBenefitMonthwiseAdjustmentDetail)
                {
                    ldecCummulativeAmount += lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_difference + lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_difference;
                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecMonthlyCummulativeTillDate = ldecCummulativeAmount;
                }
            }
        }

        public int CreatePayeeAccountRetroPayment(int aintPayeeAccountId, string astrRetroPaymentTypeValue, DateTime ldtEffectiveSD, DateTime ldtEffectiveED, DateTime ldtStartDate, DateTime ldtEndDate,
                                                  string astrPaymentOptionValue, Decimal adecGrossPaymentAmount, Decimal adecNetPaymentAmount,
                                                  string astrApprovedFlag, Collection<busPayeeAccountRetroPayment> aclbPayeeAccountRetroPayment,
                                                  string astrIsOverPayment, int aintPaymentHistoryHeaderId = 0)
        {
            //this.LoadPayeeAccountRetroPayments();
            if (aclbPayeeAccountRetroPayment != null && aclbPayeeAccountRetroPayment.Count > 0)
            {
                aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.retro_payment_type_value = astrRetroPaymentTypeValue;
                aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.effective_start_date = ldtEffectiveSD;
                aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.effective_end_date = ldtEffectiveED;
                aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.start_date = ldtStartDate;
                aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.end_date = ldtEndDate;
                aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.payment_option_value = astrPaymentOptionValue;
                aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.gross_payment_amount = adecGrossPaymentAmount;
                aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.net_payment_amount = adecNetPaymentAmount;
                aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.approved_flag = astrApprovedFlag;
                aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.is_overpayment_flag = astrIsOverPayment;

                if (aintPaymentHistoryHeaderId > 0)
                    aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.payment_history_header_id = aintPaymentHistoryHeaderId;

                aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.Update();
                return aclbPayeeAccountRetroPayment.FirstOrDefault().icdoPayeeAccountRetroPayment.payee_account_retro_payment_id;
            }
            else
            {
                busPayeeAccountRetroPayment lbusPayeeAccountRetroPayment = new busPayeeAccountRetroPayment { icdoPayeeAccountRetroPayment = new cdoPayeeAccountRetroPayment() };
                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payee_account_id = aintPayeeAccountId;
                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_value = astrRetroPaymentTypeValue;
                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_start_date = ldtEffectiveSD;
                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_end_date = ldtEffectiveED;
                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.start_date = ldtStartDate;
                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.end_date = ldtEndDate;
                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payment_option_value = astrPaymentOptionValue;
                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.gross_payment_amount = adecGrossPaymentAmount;
                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.net_payment_amount = adecNetPaymentAmount;
                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.approved_flag = astrApprovedFlag;
                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.is_overpayment_flag = astrIsOverPayment;


                if (aintPaymentHistoryHeaderId > 0)
                    lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payment_history_header_id = aintPaymentHistoryHeaderId;

                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.Insert();
                return lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payee_account_retro_payment_id;
            }
        }


        public busPayeeAccountRetroPayment LoadRetroActiveMonthWiseCollectionObject(DataTable adtPaymentHistory)
        {
            busPayeeAccountRetroPayment lbusPayeeAccountRetroPayment = new busPayeeAccountRetroPayment { icdoPayeeAccountRetroPayment = new cdoPayeeAccountRetroPayment() };
            lbusPayeeAccountRetroPayment.iclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();

            busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail;
            decimal ldecMonthlyTaxableAmountPaid = decimal.Zero;
            decimal ldecMonthlyNonTaxableAmountPaid = decimal.Zero;
            DateTime ldtPaymentDate = new DateTime();
            decimal ldecTaxableAmtToBePaid = decimal.Zero;
            decimal ldecNonTaxableAmtToBePaid = decimal.Zero;
            decimal ldecHours = decimal.Zero;
            string astrSuspendedFlag = string.Empty;
            int aintPaymentHistoryHeaderId = 0;


            foreach (DataRow ldtRow in adtPaymentHistory.Rows)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(ldtRow["Non_Taxable_Amount"])))
                {
                    ldecMonthlyNonTaxableAmountPaid = Convert.ToDecimal(ldtRow["Non_Taxable_Amount"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ldtRow["Taxable_Amount"])))
                {
                    ldecMonthlyTaxableAmountPaid = Convert.ToDecimal(ldtRow["Taxable_Amount"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ldtRow["PAYMENT_DATE"])))
                {
                    ldtPaymentDate = Convert.ToDateTime(ldtRow["PAYMENT_DATE"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ldtRow["PAYMENT_HISTORY_HEADER_ID"])))
                {
                    aintPaymentHistoryHeaderId = Convert.ToInt32(ldtRow["PAYMENT_HISTORY_HEADER_ID"]);
                }
                lbusBenefitMonthwiseAdjustmentDetail = new busBenefitMonthwiseAdjustmentDetail { icdoBenefitMonthwiseAdjustmentDetail = new cdoBenefitMonthwiseAdjustmentDetail() };
                lbusBenefitMonthwiseAdjustmentDetail = lbusBenefitMonthwiseAdjustmentDetail.FillMonthWiseAdjustmentDetail(ldtPaymentDate, ldecTaxableAmtToBePaid, ldecNonTaxableAmtToBePaid, ldecMonthlyTaxableAmountPaid, ldecMonthlyNonTaxableAmountPaid, decimal.Zero, null, aintPaymentHistoryHeaderId);
                lbusPayeeAccountRetroPayment.iclbBenefitMonthwiseAdjustmentDetail.Add(lbusBenefitMonthwiseAdjustmentDetail);
            }
            return lbusPayeeAccountRetroPayment;
        }


        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            DateTime ldtCurrentDate = System.DateTime.Now;
            this.ibusPayeeAccount.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);

            #region PAYEE-0020
            if (astrTemplateName == busConstant.PENSION_ADJUSTMENT_NOTIFICATION)
            {
                this.ibusPayeeAccount.istrReasonForUnderPayment = this.icdoPayeeAccountRetroPayment.retro_payment_type_description;

                busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail();
                if (lbusBenefitCalculationDetail.FindBenefitCalculationDetail(this.ibusPayeeAccount.icdoPayeeAccount.benefit_calculation_detail_id))
                { 
                 this.ibusPayeeAccount.idecFinalAccuredBenefitAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount;
                }
                this.ibusPayeeAccount.LoadBreakDownDetails();
                this.ibusPayeeAccount.idecMonthlyBenefitPlusRetroActive = this.ibusPayeeAccount.idecNextGrossPaymentACH + this.ibusPayeeAccount.idecRetroAdjustmentAmount;

                this.ibusPayeeAccount.LoadNextBenefitPaymentDate();
                if (this.ibusPayeeAccount.idtNextBenefitPaymentDate != DateTime.MinValue)
                {
                    this.ibusPayeeAccount.istrNextBenefitPaymentDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.ibusPayeeAccount.idtNextBenefitPaymentDate);
                }
            }
            #endregion
        }

        public override busBase GetCorPerson()
        {
            if (this.ibusPayeeAccount.ibusParticipant == null)
            {
                this.ibusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                this.ibusPayeeAccount.ibusParticipant.FindPerson(this.ibusPayeeAccount.icdoPayeeAccount.person_id);
            }
            this.ibusPayeeAccount.ibusParticipant.LoadPersonAddresss();
            this.ibusPayeeAccount.ibusParticipant.LoadPersonContacts();
            this.ibusPayeeAccount.ibusParticipant.LoadCorrAddress();
            return this.ibusPayeeAccount.ibusParticipant;
        }

        private string GetItemTypeCode(busPayeeAccountRetroPaymentDetail objPayeeAccountRetroPaymentDetail, busPaymentItemType objPaymentItemType)
        {
            string lstrItemTypecode = null;// lobjPaymentItemType.icdoPaymentItemType.item_type_code;
            if (this.icdoPayeeAccountRetroPayment.payment_option_value == "SPCK" && objPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id == 19)
            {
                lstrItemTypecode = "ITEM39";
            }
            else if (this.icdoPayeeAccountRetroPayment.payment_option_value == "SPCK" && objPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id == 20)
            {
                lstrItemTypecode = "ITEM40";
            }
            else if (this.icdoPayeeAccountRetroPayment.payment_option_value == "REGL" && objPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id == 39)
            {
                lstrItemTypecode = "ITEM19";
            }
            else if (this.icdoPayeeAccountRetroPayment.payment_option_value == "REGL" && objPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id == 40)
            {
                lstrItemTypecode = "ITEM20";
            }
            else
            {
                lstrItemTypecode = objPaymentItemType.icdoPaymentItemType.item_type_code;
            }
            return lstrItemTypecode;
        }

    }
}
