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
using System.Linq;
using Sagitec.DataObjects;
using System.Collections.Generic;
#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPayeeAccountBenefitOverpayment:
	/// Inherited from busPayeeAccountBenefitOverpaymentGen, the class is used to customize the business object busPayeeAccountBenefitOverpaymentGen.
	/// </summary>
	[Serializable]
	public class busPayeeAccountBenefitOverpayment : busPayeeAccountBenefitOverpaymentGen
	{
        public busPayeeAccountBenefitOverpayment()
        {
            if (this.ibusCalculation.IsNull())
            {
                this.ibusCalculation = new busCalculation();
            }
        }

        public busCalculation ibusCalculation { get; set; }
        public busPayeeAccount ibusPayeeAccount { get; set; }
        public Collection<busBenefitMonthwiseAdjustmentDetail> iclbBenefitMonthwiseAdjustmentDetail { get; set; }
        public Collection<busRepaymentSchedule> iclbRepaymentSchedule { get; set; }
        public busRepaymentSchedule ibusActiveRepaymentSchedule { get; set; }
        public bool iblnOverriddenAmountChanged { get; set; }

        public string istrStatusEffectiveDate { get; set; }
        public decimal idecGrossPaymentAmount { get; set; }
        public string istrLastBenefitPaymentDate { get; set; }
        public string istrAddress { get; set; }
        public string istrIsUSA { get; set; }
        
        public Collection<busRepaymentSchedule> iclbPreviousRepaymentSchedule { get; set; }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            base.ValidateHardErrors(aenmPageMode);
            if (!this.iclbRepaymentSchedule.IsNullOrEmpty() &&  !this.iclbBenefitMonthwiseAdjustmentDetail.IsNullOrEmpty())
            {
                if (this.iarrChangeLog.Count > 0)
                {
                    foreach (doBase ldoMonthWiseAdjustmentDetail in this.iarrChangeLog)
                    {
                        if (ldoMonthWiseAdjustmentDetail is cdoBenefitMonthwiseAdjustmentDetail)
                        {
                            lobjError = AddError(6160, " ");
                            this.iarrErrors.Add(lobjError);
                            break;
                        }
                    }
                }
            }
        }

        public override void BeforePersistChanges()
        {

            base.BeforePersistChanges();
            iblnOverriddenAmountChanged = false;
            if (this.iclbRepaymentSchedule.IsNullOrEmpty() && !this.iclbBenefitMonthwiseAdjustmentDetail.IsNullOrEmpty())
            {
                //if (this.iarrChangeLog.Count > 0)
                //{
                    foreach(doBase ldoMonthWiseAdjustmentDetail in this.iarrChangeLog)
                    {
                        if (ldoMonthWiseAdjustmentDetail is cdoBenefitMonthwiseAdjustmentDetail)
                        {
                            iblnOverriddenAmountChanged = true;
                            break;
                        }
                    }

                    if (!this.iarrChangeLog.Contains(this.icdoPayeeAccountRetroPayment))
                    {
                        this.icdoPayeeAccountRetroPayment.ienuObjectState = ObjectState.Update;
                        this.iarrChangeLog.Add(this.icdoPayeeAccountRetroPayment);
                    }
               // }



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
                        foreach (busPayeeAccountRetroPaymentDetail lbusPayeeAccountRetroPaymentDetail in iclbPayeeAccountRetroPaymentDetail)
                        {
                            if (lbusPayeeAccountRetroPaymentDetail.ibusPaymentItemType == null)
                                lbusPayeeAccountRetroPaymentDetail.LoadPaymentItemType();

                            if (lbusPayeeAccountRetroPaymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1)
                            {
                                if (lbusPayeeAccountRetroPaymentDetail.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.FLAG_YES)
                                {
                                    if (ldecNonTaxableAmount < decimal.Zero || ldecTaxableAmount < decimal.Zero)
                                    {
                                        lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount = ldecTaxableAmount + ldecNonTaxableAmount;
                                        break;
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
                                        break;
                                    }
                                    else
                                    {
                                        lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount = ldecNonTaxableAmount;
                                    }
                                }
                            }
                        }
                    }

                    if (this.icdoPayeeAccountRetroPayment.gross_payment_amount < decimal.Zero)
                    {
                        this.icdoPayeeAccountRetroPayment.is_overpayment_flag = busConstant.FLAG_YES;
                        this.icdoPayeeAccountRetroPayment.gross_payment_amount = Math.Abs(this.icdoPayeeAccountRetroPayment.gross_payment_amount);
                        this.icdoPayeeAccountRetroPayment.net_payment_amount = Math.Abs(this.icdoPayeeAccountRetroPayment.net_payment_amount);
                    }
                    else
                    {
                        this.icdoPayeeAccountRetroPayment.is_overpayment_flag = busConstant.FLAG_NO;
                        
                        if(iclbPayeeAccountRetroPaymentDetail != null && iclbPayeeAccountRetroPaymentDetail.Count > 0)
                        {
                            foreach(busPayeeAccountRetroPaymentDetail lbusPayeeAccountRetroPaymentDetail in iclbPayeeAccountRetroPaymentDetail)
                            {
                                lbusPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Delete();
                            }
                        }

                        if(ldecNonTaxableAmount < decimal.Zero || ldecTaxableAmount < decimal.Zero)
                        {
                            ldecTaxableAmount = ldecTaxableAmount + ldecNonTaxableAmount;
                            ldecNonTaxableAmount = decimal.Zero;
                        }
                            
                        
                        Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                        lclbBenefitMonthwiseAdjustmentDetail = iclbBenefitMonthwiseAdjustmentDetail;

                        ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                                                
                        CreateRetroPayments(ibusPayeeAccount, ibusPayeeAccount.idtNextBenefitPaymentDate,
                                        iclbBenefitMonthwiseAdjustmentDetail.First().icdoBenefitMonthwiseAdjustmentDetail.payment_date,
                                             iclbBenefitMonthwiseAdjustmentDetail.Last().icdoBenefitMonthwiseAdjustmentDetail.payment_date.GetLastDayofMonth(), ldecTaxableAmount, ldecNonTaxableAmount,
                                             ref lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH, null);
                    }                
                }                     
            }
         
            this.icdoPayeeAccountRetroPayment.payee_account_id = this.ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            #region Overriden Benefit Amounts
            if (iblnOverriddenAmountChanged)
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
                        this.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                        this.ibusPayeeAccount.ProcessTaxWithHoldingDetails();
                    }
                }
                 

                //Reloading the object.
                this.FindPayeeAccountBenefitOverpayment(this.icdoPayeeAccountRetroPayment.payee_account_retro_payment_id);
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
            }
            #endregion

            if (icdoPayeeAccountRetroPayment.is_overpayment_flag != busConstant.FLAG_YES)
            {
                iclbPayeeAccountRetroPaymentDetail = new Collection<busPayeeAccountRetroPaymentDetail>();
                iclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                icdoPayeeAccountRetroPayment.gross_payment_amount = decimal.Zero;
                icdoPayeeAccountRetroPayment.net_payment_amount = decimal.Zero;
            }
        }

        public void DeleteOverpaymentItems()
        {
        }

        public void LoadBenefitMonthwiseAdjustmentDetails()
        {
            DataTable ldtbBenefitMonthwiseAdjustmentDetails = Select("cdoPayeeAccountRetroPayment.LoadBenefitMonthwiseAdjustmentDetails", new object[1] { icdoPayeeAccountRetroPayment.payee_account_retro_payment_id });

            if (ldtbBenefitMonthwiseAdjustmentDetails.Rows.Count > 0)
            {
                iclbBenefitMonthwiseAdjustmentDetail = GetCollection<busBenefitMonthwiseAdjustmentDetail>(ldtbBenefitMonthwiseAdjustmentDetails, "icdoBenefitMonthwiseAdjustmentDetail");
            }
            decimal ldecCummulativeAmount = decimal.Zero;
            foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in this.iclbBenefitMonthwiseAdjustmentDetail)
            {
                ldecCummulativeAmount += lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_difference + lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_difference;
                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecMonthlyCummulativeTillDate = ldecCummulativeAmount;
            }

            //RequestID: 72091
            this.iclbBenefitMonthwiseAdjustmentDetail.OrderBy(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date).ToList();
        }

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;
            
            if(this.iclbRepaymentSchedule != null && this.iclbRepaymentSchedule.Count > 0)
            {
                lobjError = AddError(0, "Repayment schedule already exists");
                larrErrors.Add(lobjError);
                return larrErrors;
            }
          

            return larrErrors;
        }

        public void LoadRepaymentSchedule()
        {
            DataTable ldtbRepaymentSchedule = Select("cdoPayeeAccountRetroPayment.LoadRepaymentSchedule", new object[1] { icdoPayeeAccountRetroPayment.payee_account_retro_payment_id });

            if (ldtbRepaymentSchedule.Rows.Count > 0)
            {
                iclbRepaymentSchedule = GetCollection<busRepaymentSchedule>(ldtbRepaymentSchedule, "icdoRepaymentSchedule");
            }
        }

        public void LoadPreviousRepaymentSchedule()
        {
            DataTable ldtbRepaymentSchedule = Select("cdoPayeeAccountRetroPayment.LoadPreviousRepaymentSchedule", new object[1] { icdoPayeeAccountRetroPayment.payee_account_id });

            if (ldtbRepaymentSchedule.Rows.Count > 0)
            {
                iclbPreviousRepaymentSchedule = GetCollection<busRepaymentSchedule>(ldtbRepaymentSchedule, "icdoRepaymentSchedule");
            }
        }

        public void LoadRepaymentScheduleForCorr(decimal adecEEDerivedBenefit, decimal idecNextGrossPaymentACH, Dictionary<int, Dictionary<int, decimal>> adictHoursAfterRetirement, busPerson abusPerson)
        {
            this.iclbRepaymentSchedule = new Collection<busRepaymentSchedule>();
            DataTable ldtbRepaymentSchedule = Select("cdoPayeeAccountRetroPayment.LoadRepaymentSchedule", new object[1] { icdoPayeeAccountRetroPayment.payee_account_retro_payment_id });
            //iclbRepaymentSchedule = busBase.GetCollection<busRepaymentSchedule>(ldtbRepaymentSchedule, "icdoPayeeAccount");
            decimal ldecTemp = busConstant.ZERO_DECIMAL;
            decimal ldecReimbrAmt = busConstant.ZERO_DECIMAL;
            decimal ldecReimbramtPaid = busConstant.ZERO_DECIMAL;
            decimal ldecRemainingOverPaymentAmt = busConstant.ZERO_DECIMAL;
            decimal ldecTemp2 = busConstant.ZERO_DECIMAL;
            decimal ldecFlatPercent = busConstant.ZERO_DECIMAL;
            decimal ldecMonthlyBenefitReduced = busConstant.ZERO_DECIMAL;
            decimal ldecTemp3 = busConstant.ZERO_DECIMAL;
            decimal ldecTemp4 = busConstant.ZERO_DECIMAL;

            DateTime ldtPaymentDate;
            bool lblnTemp2Flag = false;

            abusPerson.iclbPersonSuspendibleMonth = new Collection<busPersonSuspendibleMonth>();
            abusPerson.LoadPersonSuspendibleMonth();


            foreach (DataRow ldtrRepaymentSchedule in ldtbRepaymentSchedule.AsEnumerable())
            {
                busRepaymentSchedule lbusRepaymentSchedule = new busRepaymentSchedule { icdoRepaymentSchedule = new cdoRepaymentSchedule() };
                lbusRepaymentSchedule.icdoRepaymentSchedule.LoadData(ldtrRepaymentSchedule);
                lbusRepaymentSchedule.LoadReimbursementDetails();

                if (!Convert.IsDBNull(ldtrRepaymentSchedule[enmRepaymentSchedule.reimbursement_amount.ToString().ToUpper()]))
                {
                    ldecReimbrAmt = Convert.ToInt32(ldtrRepaymentSchedule[enmRepaymentSchedule.reimbursement_amount.ToString().ToUpper()]);
                }
                if (!Convert.IsDBNull(ldtrRepaymentSchedule[enmRepaymentSchedule.reimbursement_amount_paid.ToString().ToUpper()]))
                {
                    ldecReimbramtPaid = Convert.ToInt32(ldtrRepaymentSchedule[enmRepaymentSchedule.reimbursement_amount_paid.ToString().ToUpper()]);
                }


                if (!Convert.IsDBNull(ldtrRepaymentSchedule[enmRepaymentSchedule.flat_percentage.ToString().ToUpper()]) && !Convert.IsDBNull(ldtrRepaymentSchedule[enmRepaymentSchedule.reimbursement_amount.ToString().ToUpper()]))
                {
                    ldecFlatPercent = Convert.ToDecimal(ldtrRepaymentSchedule[enmRepaymentSchedule.flat_percentage.ToString().ToUpper()]);
                    ldecMonthlyBenefitReduced = Convert.ToDecimal(ldtrRepaymentSchedule[enmRepaymentSchedule.next_amount_due.ToString().ToUpper()]);
                }

                lbusRepaymentSchedule.icdoRepaymentSchedule.idecFlatPercent = ldecFlatPercent;
                lbusRepaymentSchedule.icdoRepaymentSchedule.idecMonthlyAmtOfReduction = ldecMonthlyBenefitReduced;


                if (lbusRepaymentSchedule.iclbReimbursementDetails != null)
                {
                    foreach (busReimbursementDetails lbusReimbursementDetails in lbusRepaymentSchedule.iclbReimbursementDetails)
                    {
                        busRepaymentSchedule lobjbusRepaymentSchedule = new busRepaymentSchedule() { icdoRepaymentSchedule = new cdoRepaymentSchedule() };

                        lobjbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount = ldecReimbrAmt;

                        lobjbusRepaymentSchedule.icdoRepaymentSchedule.idecFlatPercent = ldecFlatPercent;
                        lobjbusRepaymentSchedule.icdoRepaymentSchedule.idecMonthlyAmtOfReduction = ldecMonthlyBenefitReduced;

                        ldtPaymentDate = new DateTime(lbusReimbursementDetails.icdoReimbursementDetails.posted_date.Year, lbusReimbursementDetails.icdoReimbursementDetails.posted_date.Month, 01);
                        lobjbusRepaymentSchedule.icdoRepaymentSchedule.idtPaymentDate = ldtPaymentDate;

                        decimal adecGrossAmount = idecNextGrossPaymentACH;
                        decimal ldecTotalGrossAmount = 0M;
                        DataTable ldtGross = busBase.Select("cdoPaymentHistoryDistribution.GetGrossAmountInAMonth", new object[2] { this.icdoPayeeAccountRetroPayment.payee_account_id, ldtPaymentDate });
                        if (ldtGross.Rows.Count > 0)
                        {
                            if (Convert.ToString(ldtGross.Rows[0]["Gross_Amount"]).IsNotNullOrEmpty())
                            {
                                adecGrossAmount = Convert.ToDecimal(ldtGross.Rows[0]["Gross_Amount"]);
                            }

                            if (Convert.ToString(ldtGross.Rows[0]["Total_Gross_Amount"]).IsNotNullOrEmpty())
                            {
                                ldecTotalGrossAmount = Convert.ToDecimal(ldtGross.Rows[0]["Total_Gross_Amount"]);
                            }

                        }

                        lobjbusRepaymentSchedule.icdoRepaymentSchedule.idecEEDerived = adecEEDerivedBenefit;
                        lobjbusRepaymentSchedule.icdoRepaymentSchedule.idecERDerived = ldecTotalGrossAmount - adecEEDerivedBenefit;

                        busCalculation lbusCalculation = new busCalculation();
                        if (lbusCalculation.CheckIfMonthIsSuspendible(adictHoursAfterRetirement, abusPerson.iclbPersonSuspendibleMonth, lbusReimbursementDetails.icdoReimbursementDetails.posted_date))
                        {
                            lobjbusRepaymentSchedule.icdoRepaymentSchedule.istrSuspendibleMonth = "Y";
                        }
                        else
                        {
                            lobjbusRepaymentSchedule.icdoRepaymentSchedule.istrSuspendibleMonth = "N";
                        }


                        lobjbusRepaymentSchedule.icdoRepaymentSchedule.idecTotal = lobjbusRepaymentSchedule.icdoRepaymentSchedule.idecEEDerived + lobjbusRepaymentSchedule.icdoRepaymentSchedule.idecERDerived;


                        decimal ldecAmountRepaid = 0M;
                        if (lbusRepaymentSchedule.iclbReimbursementDetails.Where(item => item.icdoReimbursementDetails.posted_date <= ldtPaymentDate).Count() > 0)
                        {
                            ldecAmountRepaid = lbusRepaymentSchedule.iclbReimbursementDetails.Where(item => item.icdoReimbursementDetails.posted_date <= ldtPaymentDate).Sum(item => item.icdoReimbursementDetails.amount_paid);
                        }

                        lobjbusRepaymentSchedule.icdoRepaymentSchedule.idecRepaymentToTheplan = lobjbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount - ldecAmountRepaid;

                        //Monthly Benefit for lbusReimbursementDetails.icdoReimbursementDetails.posted_date.month

                        lobjbusRepaymentSchedule.icdoRepaymentSchedule.idecPayableForTheMonth = adecGrossAmount; //Monthly Benefit for lbusReimbursementDetails.icdoReimbursementDetails.posted_date.month;
                        lobjbusRepaymentSchedule.icdoRepaymentSchedule.idecReimbrAmtInCurrentMonth = lbusReimbursementDetails.icdoReimbursementDetails.amount_paid;

                        if (iclbRepaymentSchedule != null && iclbRepaymentSchedule.Count() > 0 && iclbRepaymentSchedule.Sum(item => item.icdoRepaymentSchedule.idecReimbrAmtInCurrentMonth) > 0)
                        {
                            lobjbusRepaymentSchedule.icdoRepaymentSchedule.idecToDateReimbrAmount = iclbRepaymentSchedule.Sum(item => item.icdoRepaymentSchedule.idecReimbrAmtInCurrentMonth);
                        }


                        iclbRepaymentSchedule.Add(lobjbusRepaymentSchedule);
                    }
                }

            }

        }

        public void LoadBenefitMonthwiseAdjustmentDetailForCorr( decimal adecEEDerivedBenefit)
        {
            this.iclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
            DataTable ldtbBenefitMonthwiseAdjustmentDetail = Select("cdoPayeeAccountRetroPayment.GetBenefitMonthwiseAdjustmentDetails", new object[1] { icdoPayeeAccountRetroPayment.payee_account_retro_payment_id });
            decimal ldecNonTaxableAmtTobePaid = busConstant.ZERO_DECIMAL;
            decimal ldecTaxableAmtTobePaid = busConstant.ZERO_DECIMAL;
            decimal ldecNonTaxableAmtPaid = busConstant.ZERO_DECIMAL;
            decimal ldecTaxableAmtPaid = busConstant.ZERO_DECIMAL;
            decimal ldecNonTaxableAmtDiff = busConstant.ZERO_DECIMAL;
            decimal ldecTaxableAmtDiff = busConstant.ZERO_DECIMAL;
            decimal ldecOverUnderPaymentPerMonth = busConstant.ZERO_DECIMAL;
            decimal ldecToDateOverUnderPayment = busConstant.ZERO_DECIMAL;
            bool lblnTempFlag = true;

            foreach (DataRow ldtrBenefitMonthwiseAdjustmentDetail in ldtbBenefitMonthwiseAdjustmentDetail.AsEnumerable())
            {
                ldecNonTaxableAmtTobePaid = busConstant.ZERO_DECIMAL;
                ldecTaxableAmtTobePaid = busConstant.ZERO_DECIMAL;
                ldecNonTaxableAmtPaid = busConstant.ZERO_DECIMAL;
                ldecTaxableAmtPaid = busConstant.ZERO_DECIMAL;
                ldecNonTaxableAmtDiff = busConstant.ZERO_DECIMAL;
                ldecTaxableAmtDiff = busConstant.ZERO_DECIMAL;
                ldecOverUnderPaymentPerMonth = busConstant.ZERO_DECIMAL;
                ldecToDateOverUnderPayment = busConstant.ZERO_DECIMAL;

                if (!Convert.IsDBNull(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.non_taxable_amount_difference.ToString().ToUpper()]))
                {
                    ldecNonTaxableAmtDiff = Convert.ToDecimal(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.non_taxable_amount_difference.ToString().ToUpper()]);
                }
                if (!Convert.IsDBNull(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.taxable_amount_difference.ToString().ToUpper()]))
                {
                    ldecTaxableAmtDiff = Convert.ToDecimal(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.taxable_amount_difference.ToString().ToUpper()]);
                }
            
                ldecOverUnderPaymentPerMonth = ldecNonTaxableAmtDiff + ldecTaxableAmtDiff;
                ldecToDateOverUnderPayment = ldecOverUnderPaymentPerMonth;
    
                busBenefitMonthwiseAdjustmentDetail lobjBenefitMonthwiseAdjustmentDetail = new busBenefitMonthwiseAdjustmentDetail() { icdoBenefitMonthwiseAdjustmentDetail = new cdoBenefitMonthwiseAdjustmentDetail() };

                lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date = Convert.ToDateTime(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.payment_date.ToString().ToUpper()]);

                if (!Convert.IsDBNull(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.suspended_flag.ToString().ToUpper()]))
                {
                    lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.suspended_flag = Convert.ToString(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.suspended_flag.ToString().ToUpper()]);
                }
                else
                {
                    lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.suspended_flag = "N";
                }

          
                if (!Convert.IsDBNull(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid.ToString().ToUpper()]))
                {
                    ldecNonTaxableAmtTobePaid = Convert.ToDecimal(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid.ToString().ToUpper()]);
                }
                if (!Convert.IsDBNull(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid.ToString().ToUpper()]))
                {
                    ldecTaxableAmtTobePaid = Convert.ToDecimal(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid.ToString().ToUpper()]);
                }
                lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecTotalShouldHavePaid = ldecNonTaxableAmtTobePaid+ ldecTaxableAmtTobePaid;
                
                if (!Convert.IsDBNull(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid.ToString().ToUpper()]))
                {
                    ldecNonTaxableAmtPaid = Convert.ToDecimal(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid.ToString().ToUpper()]);
                }
                if (!Convert.IsDBNull(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.taxable_amount_paid.ToString().ToUpper()]))
                {
                    ldecTaxableAmtPaid = Convert.ToDecimal(ldtrBenefitMonthwiseAdjustmentDetail[enmBenefitMonthwiseAdjustmentDetail.taxable_amount_paid.ToString().ToUpper()]);
                }
                lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecAcctualPaidForTheMonth = ldecNonTaxableAmtPaid+ ldecTaxableAmtPaid;

                lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecOverUnderPaymentPerMonth = ldecOverUnderPaymentPerMonth;

                if (lblnTempFlag == true)
                {
                    lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecToDateOverUnderPayment = lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecOverUnderPaymentPerMonth;
                    lblnTempFlag = false;
                }
                else
                {
                    ldecToDateOverUnderPayment = ldecToDateOverUnderPayment + lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecOverUnderPaymentPerMonth;
                    lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecToDateOverUnderPayment = ldecToDateOverUnderPayment;
                }

                lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecEEDerivedShouldHavePaid = adecEEDerivedBenefit;
                lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecERDerivedShouldHavePaid = lobjBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.idecTotalShouldHavePaid - adecEEDerivedBenefit;

                iclbBenefitMonthwiseAdjustmentDetail.Add(lobjBenefitMonthwiseAdjustmentDetail);
               
            }
        }

        public bool CheckIfRepaymentSchedule()
        {
            if (iclbRepaymentSchedule != null && iclbRepaymentSchedule.Count > 0)
            {
                return true;
            }
            return false;
        }



        public void CreateRetroPayments(busPayeeAccount abusPayeeAccount, DateTime adtPaymentDate, DateTime adtEffectiveStartDate, DateTime adtEffectiveEndDate,
         decimal adecTaxableAmount, decimal adecNonTaxableAmount, ref Collection<busBenefitMonthwiseAdjustmentDetail> aclbBenefitMonthwiseAdjustmentDetail, string astrRetroPaymentType, Collection<busPayeeAccountRetroPayment> aclbbusPayeeAccountRetroPayment = null)
        {
            if (abusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                abusPayeeAccount.LoadNextBenefitPaymentDate();

            //Create Retro Payment Items 
            if (abusPayeeAccount.iclbRetroItemType.IsNullOrEmpty())
                abusPayeeAccount.LoadRetroItemType();
            if (abusPayeeAccount.iclbPaymentItemType.IsNullOrEmpty())
                abusPayeeAccount.LoadPaymentItemType();
            if (abusPayeeAccount.iclbPayeeAccountPaymentItemType.IsNullOrEmpty())
                abusPayeeAccount.LoadPayeeAccountPaymentItemType();
            if (abusPayeeAccount.iclbPayeeAccountRetroPayment.IsNullOrEmpty())
                abusPayeeAccount.LoadPayeeAccountRetroPayments();

            string lstrCorrespondingInitialRetroItemCodeTaxable = string.Empty;
            string lstrCorrespondingInitialRetroItemCodeNonTaxable = string.Empty;
            int lintOriginalPaymentTypeIdTaxable = 0;
            int lintOriginalPaymentTypeIdNonTaxable = 0;

            int lintPaymentTypeIdTaxable = 0;
            int lintPaymentTypeIdNonTaxable = 0;
            int lintPayeeAccountRetroPaymentId = 0;
            int lintTaxablePayeeAccountPaymentItemTypeId = 0;
            int lintNonTaxablePayeeAccountPaymentItemTypeId = 0;

            abusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive = (from item in abusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                      where busGlobalFunctions.CheckDateOverlapping(abusPayeeAccount.idtNextBenefitPaymentDate,
                                                                      item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                                                      && item.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != busConstant.RolloverItemReductionCheck
                                                                      select item).ToList().ToCollection<busPayeeAccountPaymentItemType>();

            if (adecTaxableAmount > 0.0M)
            {
                //Taxable Section For Retro
                if (abusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                {
                    lstrCorrespondingInitialRetroItemCodeTaxable = abusPayeeAccount.iclbRetroItemType.Where(item => item.icdoRetroItemType.from_item_type == busConstant.ITEM21 && item.icdoRetroItemType.retro_payment_type_value == busConstant.RETRO_PAYMENT_INITIAL).First().icdoRetroItemType.to_item_type;

                    lintOriginalPaymentTypeIdTaxable = abusPayeeAccount.iclbPaymentItemType.Where(item => item.icdoPaymentItemType.item_type_code == busConstant.ITEM21).First().icdoPaymentItemType.payment_item_type_id;
                    lintPaymentTypeIdTaxable = abusPayeeAccount.iclbPaymentItemType.Where(item => item.icdoPaymentItemType.item_type_code == lstrCorrespondingInitialRetroItemCodeTaxable).First().icdoPaymentItemType.payment_item_type_id;

                }
                else
                {
                    lstrCorrespondingInitialRetroItemCodeTaxable = abusPayeeAccount.iclbRetroItemType.Where(item => item.icdoRetroItemType.from_item_type == busConstant.ITEM1 && item.icdoRetroItemType.retro_payment_type_value == busConstant.RETRO_PAYMENT_INITIAL).First().icdoRetroItemType.to_item_type;

                    lintOriginalPaymentTypeIdTaxable = abusPayeeAccount.iclbPaymentItemType.Where(item => item.icdoPaymentItemType.item_type_code == busConstant.ITEM1).First().icdoPaymentItemType.payment_item_type_id;
                    lintPaymentTypeIdTaxable = abusPayeeAccount.iclbPaymentItemType.Where(item => item.icdoPaymentItemType.item_type_code == lstrCorrespondingInitialRetroItemCodeTaxable).First().icdoPaymentItemType.payment_item_type_id;
                }

                decimal ldecTotalTaxableAmount = 0;
                if (abusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Count > 0 && abusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id == lintPaymentTypeIdTaxable).Count() > 0)
                {
                    busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                    lbusPayeeAccountPaymentItemType = abusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id == lintPaymentTypeIdTaxable).FirstOrDefault();

                    ldecTotalTaxableAmount = lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount + adecTaxableAmount;

                }
                else
                {
                    //PROD PIR 490 -- Need to review
                    decimal idecTaxAmount = 0M;
                    if (adecNonTaxableAmount < 0)
                    {
                        idecTaxAmount = adecTaxableAmount + adecNonTaxableAmount;
                        ldecTotalTaxableAmount = idecTaxAmount;
                    }
                    else
                        ldecTotalTaxableAmount = adecTaxableAmount;
                }

                lintTaxablePayeeAccountPaymentItemTypeId = abusPayeeAccount.CreatePayeeAccountPaymentItemType(lstrCorrespondingInitialRetroItemCodeTaxable, ldecTotalTaxableAmount, null, 0, abusPayeeAccount.idtNextBenefitPaymentDate,
                    abusPayeeAccount.idtNextBenefitPaymentDate.GetLastDayofMonth(), busConstant.FLAG_NO, false);
            }

            if (adecNonTaxableAmount > 0.0M)
            {
                //Non-Taxable Section for Retro
                lstrCorrespondingInitialRetroItemCodeNonTaxable = abusPayeeAccount.iclbRetroItemType.Where(item => item.icdoRetroItemType.from_item_type == busConstant.ITEM2 && item.icdoRetroItemType.retro_payment_type_value == busConstant.RETRO_PAYMENT_INITIAL).First().icdoRetroItemType.to_item_type;

                lintOriginalPaymentTypeIdNonTaxable = abusPayeeAccount.iclbPaymentItemType.Where(item => item.icdoPaymentItemType.item_type_code == busConstant.ITEM2).First().icdoPaymentItemType.payment_item_type_id;
                lintPaymentTypeIdNonTaxable = abusPayeeAccount.iclbPaymentItemType.Where(item => item.icdoPaymentItemType.item_type_code == lstrCorrespondingInitialRetroItemCodeNonTaxable).First().icdoPaymentItemType.payment_item_type_id;

                decimal ldecTotalNonTaxableAmount = 0;
                if (abusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Count > 0 && abusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id == lintPaymentTypeIdNonTaxable).Count() > 0)
                {
                    busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                    lbusPayeeAccountPaymentItemType = abusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id == lintPaymentTypeIdNonTaxable).FirstOrDefault();

                    ldecTotalNonTaxableAmount = lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount + adecNonTaxableAmount;
                }
                else
                {
                    ldecTotalNonTaxableAmount = adecNonTaxableAmount;
                }

                lintNonTaxablePayeeAccountPaymentItemTypeId = abusPayeeAccount.CreatePayeeAccountPaymentItemType(lstrCorrespondingInitialRetroItemCodeNonTaxable, ldecTotalNonTaxableAmount, null, 0, abusPayeeAccount.idtNextBenefitPaymentDate,
                   abusPayeeAccount.idtNextBenefitPaymentDate.GetLastDayofMonth(), busConstant.FLAG_NO, false);
            }


            //Retro Summary Table Entry 
            busPayeeAccountRetroPayment lbusPayeeAccountRetroPayment = new busPayeeAccountRetroPayment();
            lintPayeeAccountRetroPaymentId = this.icdoPayeeAccountRetroPayment.payee_account_retro_payment_id;
            icdoPayeeAccountRetroPayment.start_date = abusPayeeAccount.idtNextBenefitPaymentDate;
            icdoPayeeAccountRetroPayment.end_date = abusPayeeAccount.idtNextBenefitPaymentDate.GetLastDayofMonth();


            if (aclbbusPayeeAccountRetroPayment != null && aclbbusPayeeAccountRetroPayment.Count > 0)
            {
                aclbbusPayeeAccountRetroPayment.FirstOrDefault().LoadPayeeAccountRetroPaymentDetails();
            }
            //Retro Item Detail Table Entries
            busPayeeAccountRetroPaymentDetail lbusPayeeAccountRetroPaymentDetail = new busPayeeAccountRetroPaymentDetail();
            if (adecTaxableAmount > 0.0M)
            {
                if (adecNonTaxableAmount < 0)
                    adecTaxableAmount = adecTaxableAmount + adecNonTaxableAmount; // Need to review and test PROD PIR 490
                lbusPayeeAccountRetroPaymentDetail.CreatePayeeAccountRetroPaymentDetail(lintPayeeAccountRetroPaymentId,
                                                            lintPaymentTypeIdTaxable, lintTaxablePayeeAccountPaymentItemTypeId, adecTaxableAmount, 0,
                                                            lintOriginalPaymentTypeIdTaxable, aclbbusPayeeAccountRetroPayment);
            }

            lbusPayeeAccountRetroPaymentDetail = new busPayeeAccountRetroPaymentDetail();
            if (adecNonTaxableAmount > 0.0M)
                lbusPayeeAccountRetroPaymentDetail.CreatePayeeAccountRetroPaymentDetail(lintPayeeAccountRetroPaymentId,
                                                                lintPaymentTypeIdNonTaxable, lintNonTaxablePayeeAccountPaymentItemTypeId, adecNonTaxableAmount, 0,
                                                                lintOriginalPaymentTypeIdNonTaxable, aclbbusPayeeAccountRetroPayment);
        }


        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);

            DateTime ldtCurrentDate = System.DateTime.Now;
            this.ibusPayeeAccount.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
            this.ibusPayeeAccount.LoadNextBenefitPaymentDate();
            istrLastBenefitPaymentDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.ibusPayeeAccount.idtLastBenefitPaymentDate);
             this.idecGrossPaymentAmount = this.icdoPayeeAccountRetroPayment.gross_payment_amount;
            #region Payee-0002
            if (astrTemplateName == busConstant.SSA_DISABILITY_STOP_OVERPAYMENT)
            {
                DataTable ldtbStatus = busBase.Select("cdoPayeeAccountStatus.GetLatestStatusEffectiveDate", new object[1] { this.icdoPayeeAccountRetroPayment.payee_account_id });
                if (ldtbStatus.Rows.Count > 0 && !Convert.ToBoolean(ldtbStatus.Rows[0][0].IsDBNull()))
                {
                    if (Convert.ToString(ldtbStatus.Rows[0]["STATUS_VALUE"]) == busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED)
                    {
                        this.istrStatusEffectiveDate = busGlobalFunctions.ConvertDateIntoDifFormat(Convert.ToDateTime(ldtbStatus.Rows[0]["STATUS_EFFECTIVE_DATE"]));
                    }
                }
                if (ibusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.PERSON_TYPE_PARTICIPANT)
                {
                    ibusPayeeAccount.aintPatrticpant = 1;
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

        public string istrEstimateEndDate { get; set; }

        public string istrEffectiveDate { get; set; }
    }
}

