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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
    /// Class MPIPHP.BusinessObjects.busRepaymentSchedule:
	/// Inherited from busPaymentReimbursementDetailsGen, the class is used to customize the business object busPaymentReimbursementDetailsGen.
	/// </summary>
	[Serializable]
    public class busRepaymentSchedule : busRepaymentScheduleGen
	{
        public busPayeeAccountRetroPayment ibusPayeeAccountRetroPayment { get; set; }
        public Collection<busReimbursementDetails> iclbReimbursementDetails { get; set; }
        public busPayeeAccount ibusOtherPayeeAccount { get; set; }

        public decimal idecReimbersmentAmount { get; set; }
        public string istrEffectiveDate { get; set; }
        public decimal idecNextAmtDue { get; set; }
        public decimal idecMonthlyBeneiftAmount { get; set; }
        public string istrEstimateEndDate { get; set; }
        public int iintSuspendibleMonth { get; set; }
        public int iint1099RBatchRanYear { get; set; }
        public string istrEffectiveEndDate { get; set; }
        public string istrPlan { get; set; }
        public bool iblnLocalFlag { get; set; }
        public bool iblnMPIFlag { get; set; }
        public string istrLastBenefitPaymentDate { get; set; }
        public string istrFlatPercentage { get; set; }
        
        #region Overidden Methods
        
        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
           // decimal ldecTotalNextDueAmount = 0M;

            if (iclbReimbursementDetails != null && iclbReimbursementDetails.Count > 0)
            {
                decimal ldecAmountPaid = 0M;

                foreach (busReimbursementDetails lbusReimbursementDetails in iclbReimbursementDetails)
                {
                    ldecAmountPaid += lbusReimbursementDetails.icdoReimbursementDetails.amount_paid;

                    if (lbusReimbursementDetails.icdoReimbursementDetails.reimbursement_details_id == 0)
                        lbusReimbursementDetails.icdoReimbursementDetails.iblnInsert = true;
                }

                icdoRepaymentSchedule.idecRemainingOverPaymentAmount = icdoRepaymentSchedule.reimbursement_amount - ldecAmountPaid;

                icdoRepaymentSchedule.reimbursement_amount_paid = ldecAmountPaid;

                if (this.icdoRepaymentSchedule.payment_option_value == busConstant.REPAYMENT_PAYMENT_OPTION_PERSONAL_CHECK)
                {
                    if (icdoRepaymentSchedule.reimbursement_amount == ldecAmountPaid)
                    {
                        icdoRepaymentSchedule.reimbursement_status_value = busConstant.REIMBURSEMENT_STATUS_COMPLETED;
                    }
                    else
                    {
                        icdoRepaymentSchedule.reimbursement_status_value = busConstant.REIMBURSEMENT_STATUS_INPROGRESS;
                    }
                }
            }

           // decimal ldecAmount = 0M;
            if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_account_id == this.icdoRepaymentSchedule.payee_account_id)
            {
                //- Do Not Delete
                //if (icdoRepaymentSchedule.ihstOldValues != null && Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"]).IsNotNullOrEmpty() &&
                //    this.icdoRepaymentSchedule.payment_option_value != Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"])
                //    && Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"]) == busConstant.REPAYMENT_PAYMENT_OPTION_PLAN_BENEFIT)
                //{
                //    this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();

                //    busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                //    lbusPayeeAccountPaymentItemType = this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53 
                //        && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).FirstOrDefault();

                //    if (lbusPayeeAccountPaymentItemType != null)
                //    {
                //        if (Convert.ToString(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]).IsNotNullOrEmpty())
                //        {
                //            ldecAmount = lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount - Convert.ToDecimal(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]);
                //        }
                //    }

                //    if (ldecAmount > 0)
                //    {
                //        this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
                //        ldecAmount, null, 0, lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date, DateTime.MinValue, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
                  
                //    }
                //    else
                //    {

                //        if (lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date > DateTime.Now)
                //        {
                //            lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Delete();
                //        }
                //        else
                //        {
                //            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
                //                       0, null, 0, lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date, DateTime.Now, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
                //        }
                //    }
                //}

                this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadBenefitDetails();

                int lintPlanId = this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.iintPlanId;

                if (this.icdoRepaymentSchedule.effective_date < this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtNextBenefitPaymentDate 
                    && icdoRepaymentSchedule.next_amount_due > 0)
                {
                    this.icdoRepaymentSchedule.estimated_end_date = this.GetEstimatedEndDate(this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtNextBenefitPaymentDate, lintPlanId);
                }
                else if(icdoRepaymentSchedule.next_amount_due > 0)
                {
                    DateTime ldtNextBenefitPaymentDate = new DateTime();
                    ldtNextBenefitPaymentDate = this.GetNextBenefitPaymentDate(this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtNextBenefitPaymentDate, lintPlanId);
                    this.icdoRepaymentSchedule.estimated_end_date = this.GetEstimatedEndDate(ldtNextBenefitPaymentDate, lintPlanId);
                }
            }
            else
            {
                if (ibusOtherPayeeAccount == null)
                {
                    ibusOtherPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    ibusOtherPayeeAccount.FindPayeeAccount(this.icdoRepaymentSchedule.payee_account_id);
                    ibusOtherPayeeAccount.LoadNextBenefitPaymentDate();
                }

                //- Do Not Delete
                //if (Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"]).IsNotNullOrEmpty() &&
                //    this.icdoRepaymentSchedule.payment_option_value != Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"])
                //    && Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"]) == busConstant.REPAYMENT_PAYMENT_OPTION_PLAN_BENEFIT)
                //{
                //    ibusOtherPayeeAccount.LoadPayeeAccountPaymentItemType();
                //    busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                //    lbusPayeeAccountPaymentItemType = ibusOtherPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53
                //        && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).FirstOrDefault();

                //    if (lbusPayeeAccountPaymentItemType != null)
                //    {
                //        if (Convert.ToString(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]).IsNotNullOrEmpty())
                //        {
                //            ldecAmount = lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount - Convert.ToDecimal(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]);
                //        }
                //    }

                //    if (ldecAmount > 0)
                //    {
                //        ibusOtherPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
                //           ldecAmount, null, 0, lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date, DateTime.MinValue, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
                //    }
                //    else
                //    {

                //        if (lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date > DateTime.Now)
                //        {
                //            lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Delete();
                //        }
                //        else
                //        {
                //            ibusOtherPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
                //                       0, null, 0, lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date, DateTime.Now, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
                //        }
                //    }
                //}

                ibusOtherPayeeAccount.LoadBenefitDetails();

                int lintPlanId = ibusOtherPayeeAccount.icdoPayeeAccount.iintPlanId;

                if (this.icdoRepaymentSchedule.effective_date < ibusOtherPayeeAccount.idtNextBenefitPaymentDate)
                {
                    this.icdoRepaymentSchedule.estimated_end_date = this.GetEstimatedEndDate(ibusOtherPayeeAccount.idtNextBenefitPaymentDate, lintPlanId);
                }
                else
                {
                    if (icdoRepaymentSchedule.next_amount_due > 0)
                    {
                        DateTime ldtNextBenefitPaymentDate = new DateTime();
                        ldtNextBenefitPaymentDate = this.GetNextBenefitPaymentDate(ibusOtherPayeeAccount.idtNextBenefitPaymentDate, lintPlanId);
                        this.icdoRepaymentSchedule.estimated_end_date = this.GetEstimatedEndDate(ldtNextBenefitPaymentDate, lintPlanId);
                    }
                }
            }

            #region 1099R Business Rules 
            if (iclbReimbursementDetails != null && iclbReimbursementDetails.Count > 0)
            {
                if (icdoRepaymentSchedule.reimbursement_amount_paid ==
                    (icdoRepaymentSchedule.reimbursement_amount - (iclbReimbursementDetails.Sum(item=>item.icdoReimbursementDetails.state_tax) 
                    + iclbReimbursementDetails.Sum(item=>item.icdoReimbursementDetails.fed_tax))))
                {
                    busPayeeAccount lbusPayeeAccount = new busPayeeAccount();

                    if (iclbReimbursementDetails.Sum(item => item.icdoReimbursementDetails.state_tax) > 0)
                    {
                        lbusPayeeAccount.InsertIntoProviderReportPayment(busConstant.SUBSYSTEM_TYPE_REIMBURSEMENTS, icdoRepaymentSchedule.repayment_schedule_id,
                           this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.person_id, busConstant.STATE_TAX_PROVIDER_ORG_ID,
                           this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_account_id,
                            -(iclbReimbursementDetails.Sum(item => item.icdoReimbursementDetails.state_tax)), 0, 0);
                    }

                    if (iclbReimbursementDetails.Sum(item => item.icdoReimbursementDetails.fed_tax) > 0)
                    {
                        lbusPayeeAccount.InsertIntoProviderReportPayment(busConstant.SUBSYSTEM_TYPE_REIMBURSEMENTS, icdoRepaymentSchedule.repayment_schedule_id,
                      this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.person_id, busConstant.FED_TAX_PROVIDER_ORG_ID,
                      this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_account_id,
                       -(iclbReimbursementDetails.Sum(item => item.icdoReimbursementDetails.fed_tax)), 0, 0);
                    }

                    icdoRepaymentSchedule.reimbursement_status_value = busConstant.REIMBURSEMENT_STATUS_COMPLETED;
                }
                
            }
            #endregion 1099R Business Rules


            if (this.icdoRepaymentSchedule.payment_option_value == busConstant.REPAYMENT_PAYMENT_OPTION_PERSONAL_CHECK)
            {
                icdoRepaymentSchedule.next_amount_due = 0;
                icdoRepaymentSchedule.flat_percentage = 0;
                icdoRepaymentSchedule.estimated_end_date = DateTime.MinValue;
            }
            if (!this.iarrChangeLog.Contains(this.icdoRepaymentSchedule))
            {
                this.icdoRepaymentSchedule.ienuObjectState = ObjectState.Update;
                this.iarrChangeLog.Add(this.icdoRepaymentSchedule);
            }


            ////Post entry into Payee Account Payment Item Types - Create Pension Receivable Item - Do Not Delete
            //if (this.icdoRepaymentSchedule.payment_option_value == busConstant.REPAYMENT_PAYMENT_OPTION_PLAN_BENEFIT)
            //{

            //    if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_account_id == this.icdoRepaymentSchedule.payee_account_id)
            //    {
            //        ldecTotalNextDueAmount = this.icdoRepaymentSchedule.next_amount_due;

            //        DateTime ldtNextPaymentDate = new DateTime();

            //        ldtNextPaymentDate = this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtNextBenefitPaymentDate;

            //        this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();

            //        busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
            //        if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53
            //            && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).Count() > 0)
            //        {
            //            lbusPayeeAccountPaymentItemType = this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53
            //            && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).FirstOrDefault();
            //        }

            //        if (lbusPayeeAccountPaymentItemType != null)
            //        {
            //            ldecTotalNextDueAmount += lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount;


            //            if (Convert.ToString(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]).IsNotNullOrEmpty())
            //            {
            //                ldecTotalNextDueAmount -= Convert.ToDecimal(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]);
            //            }
                        

            //            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
            //              ldecTotalNextDueAmount, null, 0, lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date, DateTime.MinValue, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
            //        }
            //        else
            //        {
            //            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
            //             ldecTotalNextDueAmount, null, 0, ldtNextPaymentDate, DateTime.MinValue, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
            //        }
            //    }
            //    else
            //    {

            //        DateTime ldtNextPaymentDate = new DateTime();

            //        ldtNextPaymentDate = ibusOtherPayeeAccount.idtNextBenefitPaymentDate;

            //        ldecTotalNextDueAmount = this.icdoRepaymentSchedule.next_amount_due;

            //        ibusOtherPayeeAccount.LoadPayeeAccountPaymentItemType();

            //        busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
            //        if (ibusOtherPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53
            //            && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).Count() > 0)
            //        {
            //            lbusPayeeAccountPaymentItemType = ibusOtherPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53
            //            && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).FirstOrDefault();
            //        }

            //        if (lbusPayeeAccountPaymentItemType != null)
            //        {
            //            ldecTotalNextDueAmount += lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount;

            //            if (Convert.ToString(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]).IsNotNullOrEmpty())
            //            {
            //                ldecTotalNextDueAmount -= Convert.ToDecimal(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]);
            //            }

            //            ibusOtherPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
            //               ldecTotalNextDueAmount, null, 0, lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date, DateTime.MinValue, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
            //        }
            //        else
            //        {
            //            ibusOtherPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
            //                   ldecTotalNextDueAmount, null, 0, ldtNextPaymentDate, DateTime.MinValue, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
            //        }
            //    }


            //}
          
        }


        public void Load1099RBatchRanYear()
        {
            DataTable ldtbl1099RBatchRanYear = Select("cdoRepaymentSchedule.GetYear1099RBatchRan", new object[0] { });

            if (ldtbl1099RBatchRanYear != null && ldtbl1099RBatchRanYear.Rows.Count > 0 && Convert.ToString(ldtbl1099RBatchRanYear.Rows[0][0]).IsNotNullOrEmpty())
            {
                iint1099RBatchRanYear = Convert.ToInt32(ldtbl1099RBatchRanYear.Rows[0][0]);
            }
            else
            {
                iint1099RBatchRanYear = 0;
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            foreach (busReimbursementDetails lbusReimbursementDetails in iclbReimbursementDetails)
            {
                if (lbusReimbursementDetails.icdoReimbursementDetails.iblnInsert == true)
                {
                    if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                    {
                        busPersonAccountRetirementContribution lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

                        if (lbusReimbursementDetails.icdoReimbursementDetails.amount_paid != 0)
                        {
                            lobjRetrContribution.InsertPersonAccountRetirementContirbution(this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_account_id, ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.idtRetireMentDate, lbusReimbursementDetails.icdoReimbursementDetails.posted_date,
                                ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.idtRetireMentDate.Year, adecIAPBalanceAmount: lbusReimbursementDetails.icdoReimbursementDetails.amount_paid,
                            astrTransactionType: busConstant.RCTransactionTypePayment, astrContributionType: busConstant.RCContributionTypeAllocation1);
                        }
                    }
                }
            }

            LoadReimbursementDetails();
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {

             utlError lobjError = new utlError();
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            base.ValidateHardErrors(aenmPageMode);


            if (this.icdoRepaymentSchedule.payment_option_value == busConstant.REPAYMENT_PAYMENT_OPTION_PLAN_BENEFIT && icdoRepaymentSchedule.next_amount_due <= 0)
            {
                lobjError = AddError(6116, "");
                this.iarrErrors.Add(lobjError);
                return;
            }

            //PIR 953
            if (this.icdoRepaymentSchedule.effective_date.Day !=1)
            {
                lobjError = AddError(6285, "");
                this.iarrErrors.Add(lobjError);
                return;
            }

            if (this.icdoRepaymentSchedule.payment_option_value == busConstant.REPAYMENT_PAYMENT_OPTION_PLAN_BENEFIT)
            {
                if (icdoRepaymentSchedule.payee_account_id <= 0)
                {
                    lobjError = AddError(6117, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }
                else
                {
                    int lintCount = (int)DBFunction.DBExecuteScalar("cdoPayeeAccountStatus.CheckIfPayeeAccountIsActive", new object[1] { icdoRepaymentSchedule.payee_account_id },
                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (lintCount <= 0)
                    {
                        lobjError = AddError(6118, "");
                        this.iarrErrors.Add(lobjError);
                        return;
                    }
                    else
                    {

                        DateTime ldtNextBenefitPaymentDate = new DateTime();
                        if(this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_account_id == icdoRepaymentSchedule.payee_account_id)
                        {
                            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadNextBenefitPaymentDate();
                            ldtNextBenefitPaymentDate = this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtNextBenefitPaymentDate;
                        }
                        else
                        {
                            ibusOtherPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            ibusOtherPayeeAccount.FindPayeeAccount(icdoRepaymentSchedule.payee_account_id);
                            ibusOtherPayeeAccount.LoadNextBenefitPaymentDate();
                            ldtNextBenefitPaymentDate=ibusOtherPayeeAccount.idtNextBenefitPaymentDate;

                        }

                        DataTable ldtblNetBenefitAmount = Select("cdoRepaymentSchedule.GetNetBenefitAmount", new object[2] { icdoRepaymentSchedule.payee_account_id, ldtNextBenefitPaymentDate });

                        if (ldtblNetBenefitAmount.Rows.Count > 0 && Convert.ToString(ldtblNetBenefitAmount.Rows[0][0]).IsNotNullOrEmpty() && Convert.ToDecimal(ldtblNetBenefitAmount.Rows[0][0]) < icdoRepaymentSchedule.next_amount_due)
                        {
                            lobjError = AddError(6119, "");
                            this.iarrErrors.Add(lobjError);
                            return;
                        }
                    }

                }

            }


            //if (this.icdoRepaymentSchedule.payment_option_value == busConstant.REPAYMENT_PAYMENT_OPTION_PERSONAL_CHECK)
            //{
            //    if (this.iclbReimbursementDetails.Count == 0 || (this.iclbReimbursementDetails != null &&
            //        this.iclbReimbursementDetails.Count > 0 && this.iclbReimbursementDetails.LastOrDefault().icdoReimbursementDetails.payment_option_value != busConstant.REIMBURSEMENT_PAYMENT_OPTION_CHECK))
            //    {
            //        lobjError = AddError(6132, " ");
            //        this.iarrErrors.Add(lobjError);
            //        return;
            //    }
            //}

        
          
                if (this.icdoRepaymentSchedule.next_amount_due > this.icdoRepaymentSchedule.idecRemainingOverPaymentAmount)
                {
                    lobjError = AddError(6100, " ");
                    this.iarrErrors.Add(lobjError);
                    return;
                }
            

            if (this.icdoRepaymentSchedule.effective_date == DateTime.MinValue)
            {
                lobjError = AddError(6101, " ");
                this.iarrErrors.Add(lobjError);
                return;
            }

            if (this.icdoRepaymentSchedule.effective_date < DateTime.Now)
            {
                lobjError = AddError(6171, " ");
                this.iarrErrors.Add(lobjError);
                return;
            }


            if (this.icdoRepaymentSchedule.payee_account_id > 0)
            {
                bool flag = false;
                DataTable ldtblPayeeBenefitAccountID = Select("cdoPayeeAccount.GetPayeeBenefitAccountID",new object[1]{icdoRepaymentSchedule.payee_account_id});
                if (ldtblPayeeBenefitAccountID.Rows.Count > 0)
                {
                    if (icdoRepaymentSchedule.payee_account_retro_payment_id > 0)
                    {
                        if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id ==
                               Convert.ToInt32(ldtblPayeeBenefitAccountID.Rows[0][enmPayeeAccount.payee_benefit_account_id.ToString().ToUpper()]) ||
                            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.person_id ==
                           Convert.ToInt32(ldtblPayeeBenefitAccountID.Rows[0][enmPayeeAccount.person_id.ToString().ToUpper()]))
                        {
                            flag = true;
                        }

                        if (!flag)
                        {
                            busUserRoles lobjUserRoles = new busUserRoles();
                            if (!lobjUserRoles.FindUserRoles(Convert.ToInt32(this.iobjPassInfo.idictParams["UserSerialID"]), 200))
                            {
                                lobjError = AddError(6121, " ");
                                this.iarrErrors.Add(lobjError);
                                return;
                            }
                        }
                    }
                }
            
            }


            if (this.iclbReimbursementDetails != null && this.iclbReimbursementDetails.Count > 0)
            {
                foreach (busReimbursementDetails lbusReimbursementDetails in iclbReimbursementDetails)
                {
                    if (iclbReimbursementDetails.Where(item => item.icdoReimbursementDetails.check_number == lbusReimbursementDetails.icdoReimbursementDetails.check_number
                        && item.icdoReimbursementDetails.check_number.IsNotNullOrEmpty() && lbusReimbursementDetails.icdoReimbursementDetails.check_number.IsNotNullOrEmpty()).Count() > 1)
                    {
                        lobjError = AddError(0, "Check number cannot be same for different repayments");
                        this.iarrErrors.Add(lobjError);
                        return;
                    }
                }


                if (icdoRepaymentSchedule.reimbursement_amount < iclbReimbursementDetails.Sum(item => item.icdoReimbursementDetails.amount_paid))
                {
                    lobjError = AddError(0, "Amount Repaid is greater than total reimbursement amount");
                    this.iarrErrors.Add(lobjError);
                    return;
                }
            }


            if (this.iarrErrors.Count == 0)
            {
                CreatePensionReceivableItem(ref this.iarrErrors);
            }



        }


        #endregion Overidden Methods


        #region Public Methods

        public void CreatePensionReceivableItem(ref ArrayList iarrErrors)
        {

            utlError lobjError = new utlError();
            decimal ldecAmount = 0M;
            if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_account_id == this.icdoRepaymentSchedule.payee_account_id)
            {
                if (icdoRepaymentSchedule.ihstOldValues.Count > 0 && Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"]).IsNotNullOrEmpty() &&
                    this.icdoRepaymentSchedule.payment_option_value != Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"])
                    && Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"]) == busConstant.REPAYMENT_PAYMENT_OPTION_PLAN_BENEFIT)
                {
                    this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();

                    busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = null;// new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                    lbusPayeeAccountPaymentItemType = this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53
                        && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).FirstOrDefault();

                    if (lbusPayeeAccountPaymentItemType != null)
                    {
                        if (icdoRepaymentSchedule.ihstOldValues.Count > 0 && Convert.ToString(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]).IsNotNullOrEmpty())
                        {
                            ldecAmount = lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount - Convert.ToDecimal(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]);
                        }
                    }

                    if (ldecAmount > 0)
                    {
                        
                            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
                            ldecAmount, null, 0, this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
                    }
                    else
                    {

                        if (lbusPayeeAccountPaymentItemType != null && lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date > DateTime.Now)
                        {
                            lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Delete();
                        }
                        else
                        {
                            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
                                       0, null, 0, this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtNextBenefitPaymentDate, DateTime.Now, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
                        }
                    }
                }
            }
            else
            {
                if (ibusOtherPayeeAccount == null)
                {
                    ibusOtherPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    ibusOtherPayeeAccount.FindPayeeAccount(this.icdoRepaymentSchedule.payee_account_id);
                    ibusOtherPayeeAccount.LoadNextBenefitPaymentDate();
                }

                if (icdoRepaymentSchedule.ihstOldValues.Count > 0 && Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"]).IsNotNullOrEmpty() &&
                    this.icdoRepaymentSchedule.payment_option_value != Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"])
                    && Convert.ToString(icdoRepaymentSchedule.ihstOldValues["payment_option_value"]) == busConstant.REPAYMENT_PAYMENT_OPTION_PLAN_BENEFIT)
                {
                    ibusOtherPayeeAccount.LoadPayeeAccountPaymentItemType();
                    busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = null;// new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                    lbusPayeeAccountPaymentItemType = ibusOtherPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53
                        && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).FirstOrDefault();

                    if (lbusPayeeAccountPaymentItemType != null)
                    {
                        if (icdoRepaymentSchedule.ihstOldValues.Count > 0 && Convert.ToString(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]).IsNotNullOrEmpty())
                        {
                            ldecAmount = lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount - Convert.ToDecimal(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]);
                        }
                    }

                    if (ldecAmount > 0)
                    {
                        
                            ibusOtherPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
                              ldecAmount, null, 0, ibusOtherPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
                     
                    }
                    else
                    {

                        if (lbusPayeeAccountPaymentItemType != null && lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date > DateTime.Now)
                        {
                            lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Delete();
                        }
                        else
                        {
                            ibusOtherPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
                                       0, null, 0, ibusOtherPayeeAccount.idtNextBenefitPaymentDate, DateTime.Now, busConstant.FLAG_NO, busConstant.BOOL_FALSE);
                        }
                    }
                }
            }


            //Post entry into Payee Account Payment Item Types - Create Pension Receivable Item
            decimal ldecTotalNextDueAmount = 0M;
            if (this.icdoRepaymentSchedule.payment_option_value == busConstant.REPAYMENT_PAYMENT_OPTION_PLAN_BENEFIT)
            {

                if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_account_id == this.icdoRepaymentSchedule.payee_account_id)
                {
                    ldecTotalNextDueAmount = this.icdoRepaymentSchedule.next_amount_due;

                    DateTime ldtNextPaymentDate = new DateTime();

                    ldtNextPaymentDate = this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtNextBenefitPaymentDate;

                    this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();

                    busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = null; //new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                    if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53
                        && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).Count() > 0)
                    {
                        lbusPayeeAccountPaymentItemType = this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53
                        && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).FirstOrDefault();
                    }

                    if (lbusPayeeAccountPaymentItemType != null)
                    {
                        ldecTotalNextDueAmount += lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount;


                        if (icdoRepaymentSchedule.ihstOldValues.Count > 0 && Convert.ToString(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]).IsNotNullOrEmpty())
                        {
                            ldecTotalNextDueAmount -= Convert.ToDecimal(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]);
                        }

                    }


                    DataTable ldtblNetBenefitAmount = Select("cdoRepaymentSchedule.GetNetBenefitAmount", new object[2] { this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_account_id,
                        this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtNextBenefitPaymentDate});
                    if (ldtblNetBenefitAmount != null && ldtblNetBenefitAmount.Rows.Count > 0 && Convert.ToString(ldtblNetBenefitAmount.Rows[0][0]).IsNotNullOrEmpty())
                    {
                        if (ldecTotalNextDueAmount > Convert.ToDecimal(ldtblNetBenefitAmount.Rows[0][0]))
                        {
                            lobjError = AddError(6162, "");
                            this.iarrErrors.Add(lobjError);
                            return;
                        }
                    }


                    this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
                     ldecTotalNextDueAmount, null, 0, ldtNextPaymentDate, DateTime.MinValue, busConstant.FLAG_NO, busConstant.BOOL_FALSE);

                }
                else
                {

                    DateTime ldtNextPaymentDate = new DateTime();

                    ldtNextPaymentDate = ibusOtherPayeeAccount.idtNextBenefitPaymentDate;

                    ldecTotalNextDueAmount = this.icdoRepaymentSchedule.next_amount_due;

                    ibusOtherPayeeAccount.LoadPayeeAccountPaymentItemType();

                    busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = null;// new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                    if (ibusOtherPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53
                        && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).Count() > 0)
                    {
                        lbusPayeeAccountPaymentItemType = ibusOtherPayeeAccount.iclbPayeeAccountPaymentItemType.Where(item => item.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM53
                        && item.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue).FirstOrDefault();
                    }

                    if (lbusPayeeAccountPaymentItemType != null)
                    {
                        ldecTotalNextDueAmount += lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount;

                        if (icdoRepaymentSchedule.ihstOldValues.Count > 0 && Convert.ToString(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]).IsNotNullOrEmpty())
                        {
                            ldecTotalNextDueAmount -= Convert.ToDecimal(icdoRepaymentSchedule.ihstOldValues["next_amount_due"]);
                        }

                    }

                    DataTable ldtblNetBenefitAmount = Select("cdoRepaymentSchedule.GetNetBenefitAmount", new object[2] { ibusOtherPayeeAccount.icdoPayeeAccount.payee_account_id,
                        ibusOtherPayeeAccount.idtNextBenefitPaymentDate});
                    if (ldtblNetBenefitAmount != null && ldtblNetBenefitAmount.Rows.Count > 0 && Convert.ToString(ldtblNetBenefitAmount.Rows[0][0]).IsNotNullOrEmpty())
                    {
                        if (ldecTotalNextDueAmount > Convert.ToDecimal(ldtblNetBenefitAmount.Rows[0][0]))
                        {
                            lobjError = AddError(6162, "");
                            this.iarrErrors.Add(lobjError);
                            return;
                        }
                    }


                    ibusOtherPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM53,
                           ldecTotalNextDueAmount, null, 0, ldtNextPaymentDate, DateTime.MinValue, busConstant.FLAG_NO, busConstant.BOOL_FALSE);

                }


            }
        }


        public void LoadReimbursementDetails()
        {
            DataTable ldtbReimbursementDetails = Select("cdoReimbursementDetails.LoadReimbursementDetails",new object[1]{icdoRepaymentSchedule.repayment_schedule_id});
            if (ldtbReimbursementDetails.Rows.Count > 0)
            {
                iclbReimbursementDetails = GetCollection<busReimbursementDetails>(ldtbReimbursementDetails, "icdoReimbursementDetails");

            }
        }

       
        public DateTime GetNextBenefitPaymentDate(DateTime adtLastBenefitPaymentDate, int aintPlanId)
        {
            DateTime ldtNextBenefitPaymentDate = new DateTime();
            while (ldtNextBenefitPaymentDate < icdoRepaymentSchedule.effective_date)
            {
                if (aintPlanId != busConstant.IAP_PLAN_ID)
                {
                    ldtNextBenefitPaymentDate = adtLastBenefitPaymentDate.AddMonths(1);
                    adtLastBenefitPaymentDate = ldtNextBenefitPaymentDate;
                }
                else
                {
                    ldtNextBenefitPaymentDate = busGlobalFunctions.GetPaymentDayForIAP(adtLastBenefitPaymentDate.AddDays(7));
                    adtLastBenefitPaymentDate = ldtNextBenefitPaymentDate;
                }
            }
            return ldtNextBenefitPaymentDate;
        }

        public DateTime GetEstimatedEndDate(DateTime adtNextBenefitPaymentDate, int aintPlanId)
        {
            DateTime ldtEstimatedEndDate = new DateTime();

            if (aintPlanId == busConstant.IAP_PLAN_ID)
            {
                decimal ldecTempAmount = 0;
                ldecTempAmount = icdoRepaymentSchedule.next_amount_due;

                while (ldecTempAmount <= icdoRepaymentSchedule.idecRemainingOverPaymentAmount)
                {
                    if (ldecTempAmount == icdoRepaymentSchedule.idecRemainingOverPaymentAmount)
                    {
                        ldtEstimatedEndDate = adtNextBenefitPaymentDate;
                        break;
                    }
                    else
                    {
                        ldtEstimatedEndDate = busGlobalFunctions.GetPaymentDayForIAP(adtNextBenefitPaymentDate.AddDays(7));
                        adtNextBenefitPaymentDate = ldtEstimatedEndDate;
                        ldecTempAmount += icdoRepaymentSchedule.next_amount_due;
                    }
                }
                  
            }
            else
            {
                decimal ldecTempAmount = 0;
                ldecTempAmount = icdoRepaymentSchedule.next_amount_due;

                while (ldecTempAmount <= icdoRepaymentSchedule.idecRemainingOverPaymentAmount)
                {
                    if (ldecTempAmount == icdoRepaymentSchedule.idecRemainingOverPaymentAmount)
                    {
                        ldtEstimatedEndDate = adtNextBenefitPaymentDate;
                        break;
                    }
                    else
                    {
                        ldtEstimatedEndDate = adtNextBenefitPaymentDate.AddMonths(1);
                        adtNextBenefitPaymentDate = ldtEstimatedEndDate;
                        ldecTempAmount += icdoRepaymentSchedule.next_amount_due;
                    }
                }
            }

            return ldtEstimatedEndDate;
        }


        public ArrayList btn_DeleteReimbursementDetailClick(int aintReimbursementDetailID)
        {

            ArrayList larrList = new ArrayList();
            if (aintReimbursementDetailID > 0 && iclbReimbursementDetails.Where(item => item.icdoReimbursementDetails.reimbursement_details_id == aintReimbursementDetailID).Count() > 0)
            {
                busReimbursementDetails lbusReimbursementDetails = iclbReimbursementDetails.Where(item => item.icdoReimbursementDetails.reimbursement_details_id == aintReimbursementDetailID).FirstOrDefault();

                if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                {
                    busPersonAccountRetirementContribution lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

                    if (lbusReimbursementDetails.icdoReimbursementDetails.amount_paid != 0)
                    {
                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_account_id, ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.idtRetireMentDate, lbusReimbursementDetails.icdoReimbursementDetails.posted_date, ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.idtRetireMentDate.Year, adecIAPBalanceAmount: -lbusReimbursementDetails.icdoReimbursementDetails.amount_paid,
                        astrTransactionType: busConstant.RCTransactionTypePayment, astrContributionType: busConstant.RCContributionTypeAllocation1);
                    }
                }

                this.icdoRepaymentSchedule.reimbursement_amount_paid = this.icdoRepaymentSchedule.reimbursement_amount_paid - lbusReimbursementDetails.icdoReimbursementDetails.amount_paid;

                this.icdoRepaymentSchedule.idecRemainingOverPaymentAmount = this.icdoRepaymentSchedule.reimbursement_amount - this.icdoRepaymentSchedule.reimbursement_amount_paid;
               
                if (this.icdoRepaymentSchedule.reimbursement_status_value == busConstant.REIMBURSEMENT_STATUS_COMPLETED)
                {
                    this.icdoRepaymentSchedule.reimbursement_status_value = busConstant.REIMBURSEMENT_STATUS_INPROGRESS;
                }

                this.icdoRepaymentSchedule.Update();

                this.iclbReimbursementDetails.Remove(this.iclbReimbursementDetails.Where(item => item.icdoReimbursementDetails.reimbursement_details_id == aintReimbursementDetailID).FirstOrDefault());
                lbusReimbursementDetails.icdoReimbursementDetails.Delete();
            }
            
            larrList.Add(this);
            return larrList;
        }

        public override busBase GetCorPerson()
        {
            if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant == null)
            {
                this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant.FindPerson(this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.person_id);
            }
            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadBenefitDetails();
            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant.LoadPersonAddresss();
            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant.LoadPersonContacts();
            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant.LoadCorrAddress();
            return this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            DateTime ldtCurrentDate = System.DateTime.Now;
            this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
            this.iblnLocalFlag = false;
            this.iblnMPIFlag = false;

            //RID - 65575
            if (this.ibusPayeeAccountRetroPayment != null && this.ibusPayeeAccountRetroPayment.ibusPayeeAccount != null &&
                this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
            {
                this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadNextBenefitPaymentDate();
            }

                busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
            if (lbusPlanBenefitXr.FindPlanBenefitXr(this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.plan_benefit_id))
            {
                if (lbusPlanBenefitXr.icdoPlanBenefitXr.plan_id == busConstant.LOCAL_600_PLAN_ID)
                {
                    this.istrPlan = busConstant.Local_600.ToUpper();
                    this.iblnLocalFlag = true;
                }
                else if (lbusPlanBenefitXr.icdoPlanBenefitXr.plan_id == busConstant.LOCAL_666_PLAN_ID)
                {
                    this.istrPlan = busConstant.Local_666.ToUpper();
                    this.iblnLocalFlag = true;
                }
                else if (lbusPlanBenefitXr.icdoPlanBenefitXr.plan_id == busConstant.LOCAL_700_PLAN_ID)
                {
                    this.istrPlan = busConstant.LOCAL_700.ToUpper();
                    this.iblnLocalFlag = true;
                }
                else if (lbusPlanBenefitXr.icdoPlanBenefitXr.plan_id == busConstant.LOCAL_52_PLAN_ID)
                {
                    this.istrPlan = busConstant.Local_52.ToUpper();
                    this.iblnLocalFlag = true;
                }
                else if (lbusPlanBenefitXr.icdoPlanBenefitXr.plan_id == busConstant.LOCAL_161_PLAN_ID)
                {
                    this.istrPlan = busConstant.Local_161.ToUpper();
                    this.iblnLocalFlag = true;
                }
                else if (lbusPlanBenefitXr.icdoPlanBenefitXr.plan_id == busConstant.MPIPP_PLAN_ID)
                {
                    this.istrPlan = busConstant.MPIPP.ToUpper();
                    this.iblnMPIFlag = true;
                }
            }

            #region Payee-0028
            if (astrTemplateName == busConstant.RE_EMPLOYMENT_OVERPAYMENT_NOTICE_RE_PAYMENT_LETTER)
            {
                this.idecReimbersmentAmount = this.icdoRepaymentSchedule.reimbursement_amount;
                this.istrEffectiveDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.icdoRepaymentSchedule.effective_date);
                this.idecNextAmtDue = this.icdoRepaymentSchedule.next_amount_due;
                this.istrEstimateEndDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.icdoRepaymentSchedule.estimated_end_date);
                if (this.icdoRepaymentSchedule.flat_percentage > decimal.Zero)
                {
                    this.istrFlatPercentage = Convert.ToString(Convert.ToInt32(Math.Floor(this.icdoRepaymentSchedule.flat_percentage)));
                }
                if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtLastBenefitPaymentDate.IsNotNull() && this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtLastBenefitPaymentDate != DateTime.MinValue)
                {
                    istrLastBenefitPaymentDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtLastBenefitPaymentDate);
                }
                this.ibusPayeeAccountRetroPayment.LoadBenefitMonthwiseAdjustmentDetails();
                if (this.ibusPayeeAccountRetroPayment.iclbBenefitMonthwiseAdjustmentDetail != null)
                {
                    this.iintSuspendibleMonth = (from item in this.ibusPayeeAccountRetroPayment.iclbBenefitMonthwiseAdjustmentDetail
                                                 where item.icdoBenefitMonthwiseAdjustmentDetail.suspended_flag == busConstant.FLAG_YES
                                                 select item.icdoBenefitMonthwiseAdjustmentDetail.suspended_flag).Count();
                }

            }
            #endregion

            #region Payee-0019
            if (astrTemplateName == busConstant.OVERPAYMENT_REIMBURSEMENT_REQUEST)
            {

                if (this.ibusPayeeAccountRetroPayment == null)
                {
                    if (this.icdoRepaymentSchedule.payee_account_retro_payment_id != 0)
                    {
                        this.ibusPayeeAccountRetroPayment = new busPayeeAccountRetroPayment { icdoPayeeAccountRetroPayment = new cdoPayeeAccountRetroPayment() };
                        this.ibusPayeeAccountRetroPayment.FindPayeeAccountRetroPayment(this.icdoRepaymentSchedule.payee_account_retro_payment_id);

                    }
                }

                //RID - 65575
                if (this.ibusPayeeAccountRetroPayment != null && this.ibusPayeeAccountRetroPayment.ibusPayeeAccount == null)
                {
                    this.ibusPayeeAccountRetroPayment.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.FindPayeeAccount(this.ibusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payee_account_id);
                }
                this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadBreakDownDetails();

                if (this.icdoRepaymentSchedule.estimated_end_date != DateTime.MinValue)
                    istrEstimateEndDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.icdoRepaymentSchedule.estimated_end_date);
                if (this.icdoRepaymentSchedule.effective_date != DateTime.MinValue)
                    istrEffectiveDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.icdoRepaymentSchedule.effective_date);

                if (this.icdoRepaymentSchedule.flat_percentage > decimal.Zero)
                {
                    this.istrFlatPercentage = Convert.ToString(Math.Round(this.icdoRepaymentSchedule.flat_percentage,2));
                }

                if (this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtLastBenefitPaymentDate.IsNotNull() && this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtLastBenefitPaymentDate != DateTime.MinValue)
                {
                    istrLastBenefitPaymentDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idtLastBenefitPaymentDate);
                }

                this.idecMonthlyBeneiftAmount = this.ibusPayeeAccountRetroPayment.ibusPayeeAccount.idecNextGrossPaymentACH;
            }
            #endregion

        }
                
        #endregion Public Methods

    }
}
