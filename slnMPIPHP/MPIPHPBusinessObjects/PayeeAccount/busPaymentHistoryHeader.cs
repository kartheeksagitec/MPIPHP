#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Linq;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPaymentHistoryHeader:
    /// Inherited from busPaymentHistoryHeaderGen, the class is used to customize the business object busPaymentHistoryHeaderGen.
    /// </summary>
    [Serializable]
    public class busPaymentHistoryHeader : busPaymentHistoryHeaderGen
    {
        public busPerson ibusPayee { get; set; }
        public busOrganization ibusOrganization { get; set; }
        public busPayeeAccount ibusPayeeAccount { get; set; }
        public int istrPaymentHistoryDistributionId { get; set; }
       

        #region Public Methods

        public ArrayList btn_CreateBenefitOverpayment()
        {
            ArrayList larrList = new ArrayList();
            decimal ldecTaxableAmt = 0, ldecNonTaxableAmt = 0;
            bool lblnIsOverpaymentAlreadyExists = false;
            int lintPayeeAccRetroPaymentId = 0;


            DataTable ldtblOverPaymentInfo = Select("cdoPaymentHistoryHeader.GetExistingOverPaymentInfo", new object[1] { icdoPaymentHistoryHeader.payee_account_id });
            if (ldtblOverPaymentInfo != null && ldtblOverPaymentInfo.Rows.Count > 0)
            {
                lblnIsOverpaymentAlreadyExists = true;
                if (Convert.ToString(ldtblOverPaymentInfo.Rows[0][enmPayeeAccountRetroPayment.payee_account_retro_payment_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                    lintPayeeAccRetroPaymentId = Convert.ToInt32(ldtblOverPaymentInfo.Rows[0][enmPayeeAccountRetroPayment.payee_account_retro_payment_id.ToString().ToUpper()]);
            }

            if (this.iclbPaymentHistoryDistribution != null && this.iclbPaymentHistoryDistribution.Count() > 0)
            {
                foreach (busPaymentHistoryDistribution lbusPaymentHistoryDistribution in iclbPaymentHistoryDistribution)
                {
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_OVERPAID;
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();

                    lbusPaymentHistoryDistribution.InsertPaymentDistributionStatusHistory(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id,
                       lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.PAYMENT_DISTRIBUTION_STATUS_OVERPAID, DateTime.Now);
                }
            }

            foreach (busPaymentHistoryDetail lbusPaymentHistoryDetail in iclbPaymentHistoryDetail)
            {
                busPaymentItemType lbusPaymentItemType = new busPaymentItemType();
                lbusPaymentItemType.FindPaymentItemType(lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.payment_item_type_id);

                if (lbusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                    lbusPaymentItemType.icdoPaymentItemType.receivable_creation_1099r_value == busConstant.RECEIVABLE_CREATION_WITH_OR_WITHOUT_1099R)
                {
                    if (lbusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.FLAG_YES)
                        ldecTaxableAmt = ldecTaxableAmt + lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.amount;
                    else
                        ldecNonTaxableAmt = ldecNonTaxableAmt + lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.amount;
                }
            }

            busPayeeAccountRetroPayment lbusPayeeAccountRetroPayment = new busPayeeAccountRetroPayment();
            if (!lblnIsOverpaymentAlreadyExists)
            {

                lintPayeeAccRetroPaymentId = lbusPayeeAccountRetroPayment.CreatePayeeAccountRetroPayment(icdoPaymentHistoryHeader.payee_account_id, busConstant.RETRO_PAYMENT_BENEFIT_OVERPAYMENT,
                                              DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue,
                                              null, ldecTaxableAmt + ldecNonTaxableAmt, 0, null, null, busConstant.FLAG_YES, this.icdoPaymentHistoryHeader.payment_history_header_id);
            }
            else
            {
                if (lbusPayeeAccountRetroPayment.FindPayeeAccountRetroPayment(lintPayeeAccRetroPaymentId))
                {
                    lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.gross_payment_amount += (ldecTaxableAmt + ldecNonTaxableAmt);
                    lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.Update();

                    DataTable ldtblExistingRepaymentSchedule = Select("cdoRepaymentSchedule.GetExistingRepaymentSchedule", new object[1] { lintPayeeAccRetroPaymentId });

                    if (ldtblExistingRepaymentSchedule != null && ldtblExistingRepaymentSchedule.Rows.Count > 0)
                    {
                        foreach (DataRow ldrExistingRepaymentSchedule in ldtblExistingRepaymentSchedule.Rows)
                        {
                            busRepaymentSchedule lbusRepaymentSchedule = new busRepaymentSchedule { icdoRepaymentSchedule = new cdoRepaymentSchedule() };
                            lbusRepaymentSchedule.icdoRepaymentSchedule.LoadData(ldrExistingRepaymentSchedule);
                            lbusRepaymentSchedule.icdoRepaymentSchedule.original_reimbursement_amount = lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount;
                            lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount =
                                lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.gross_payment_amount;
                            lbusRepaymentSchedule.icdoRepaymentSchedule.Update();
                        }
                    }
                }
            }

            #region PROCESS_OVERPAYMENT_WORKFLOW

            if (this.ibusPayeeAccount == null)
            {
                this.ibusPayeeAccount = new busPayeeAccount();
                this.ibusPayeeAccount.FindPayeeAccount(icdoPaymentHistoryHeader.payee_account_id);
            }
            busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PROCESS_OVERPAYMENT_WORKFLOW, this.ibusPayeeAccount.icdoPayeeAccount.person_id,
                  0, this.ibusPayeeAccount.icdoPayeeAccount.payee_account_id, null);

            #endregion

            foreach (busPaymentHistoryDetail lbusPaymentHistoryDetail in iclbPaymentHistoryDetail)
            {
                busPayeeAccountRetroPaymentDetail lbusPayeeAccountRetroPaymentDetail = new busPayeeAccountRetroPaymentDetail();
                lbusPayeeAccountRetroPaymentDetail.CreatePayeeAccountRetroPaymentDetail(lintPayeeAccRetroPaymentId, lbusPaymentHistoryDetail);
            }

            #region Insert data in SGT_BENEFIT_MONTHWISE_ADJUSTMENT_DETAIL

            busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail = lbusBenefitMonthwiseAdjustmentDetail = new busBenefitMonthwiseAdjustmentDetail();
            lbusBenefitMonthwiseAdjustmentDetail.CreateMonthwiseAdjustmentDetail(lintPayeeAccRetroPaymentId, icdoPaymentHistoryHeader.payment_date, 0, 0,
                                ldecTaxableAmt, ldecNonTaxableAmt, 0, null, this.icdoPaymentHistoryHeader.payment_history_header_id);

            #endregion Insert data in SGT_BENEFIT_MONTHWISE_ADJUSTMENT_DETAIL


            this.EvaluateInitialLoadRules();
            return larrList;
        }

        public ArrayList btn_BuyBack()
        {
            ArrayList larrList = new ArrayList();
            busCalculation lbusCalculation = new busCalculation();

            decimal ldecTaxableAmt = 0, ldecNonTaxableAmt = 0;

            if (this.iclbPaymentHistoryDistribution != null && this.iclbPaymentHistoryDistribution.Count() > 0)
            {
                foreach (busPaymentHistoryDistribution lbusPaymentHistoryDistribution in iclbPaymentHistoryDistribution)
                {
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_OVERPAID;
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();

                    lbusPaymentHistoryDistribution.InsertPaymentDistributionStatusHistory(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id,
                       lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.PAYMENT_DISTRIBUTION_STATUS_OVERPAID, DateTime.Now);
                }
            }

            Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
            busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail =
                new busBenefitMonthwiseAdjustmentDetail { icdoBenefitMonthwiseAdjustmentDetail = new cdoBenefitMonthwiseAdjustmentDetail() };

            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date = this.icdoPaymentHistoryHeader.payment_date;

            if (this.ibusPayeeAccount.ibusPayeeBenefitAccount == null)
            {
                this.ibusPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount();
                this.ibusPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(this.ibusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
            }

            DataTable ldtblEEContributions = Select("cdoPersonAccountRetirementContribution.GetEEContributionAndInterest", new object[1]{
                    this.ibusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_account_id});

            if (ldtblEEContributions != null && ldtblEEContributions.Rows.Count > 0)
            {
                if (Convert.ToString(ldtblEEContributions.Rows[0][enmPersonAccountRetirementContribution.ee_int_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                {
                    if (Convert.ToDecimal(ldtblEEContributions.Rows[0][enmPersonAccountRetirementContribution.ee_int_amount.ToString().ToUpper()]) < 0)
                        ldecTaxableAmt = -Convert.ToDecimal(ldtblEEContributions.Rows[0][enmPersonAccountRetirementContribution.ee_int_amount.ToString().ToUpper()]);
                    else
                        ldecTaxableAmt = Convert.ToDecimal(ldtblEEContributions.Rows[0][enmPersonAccountRetirementContribution.ee_int_amount.ToString().ToUpper()]);
                }

                if (Convert.ToString(ldtblEEContributions.Rows[0][enmPersonAccountRetirementContribution.ee_contribution_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                {
                    if (Convert.ToDecimal(ldtblEEContributions.Rows[0][enmPersonAccountRetirementContribution.ee_contribution_amount.ToString().ToUpper()]) < 0)
                        ldecNonTaxableAmt = -Convert.ToDecimal(ldtblEEContributions.Rows[0][enmPersonAccountRetirementContribution.ee_contribution_amount.ToString().ToUpper()]);
                    else
                        ldecNonTaxableAmt = Convert.ToDecimal(ldtblEEContributions.Rows[0][enmPersonAccountRetirementContribution.ee_contribution_amount.ToString().ToUpper()]);
                }

                DateTime ldtWithdrawalDate = new DateTime();

                ldtWithdrawalDate = this.ibusPayeeAccount.icdoPayeeAccount.idtRetireMentDate;


                ldecTaxableAmt += CalculateEEInterestForWithdrawalBuyBack(ldtWithdrawalDate, (ldecTaxableAmt + ldecNonTaxableAmt));

                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid = ldecTaxableAmt;
                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid = ldecNonTaxableAmt;

                lclbBenefitMonthwiseAdjustmentDetail.Add(lbusBenefitMonthwiseAdjustmentDetail);

            }

            busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
            lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(this.ibusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_account_id,
                this.icdoPaymentHistoryHeader.payment_date, this.icdoPaymentHistoryHeader.payment_date, this.icdoPaymentHistoryHeader.payment_date.Year,
                astrTransactionType: busConstant.RCTransactionTypeAdjustment, adecEEContrAmount: ldecNonTaxableAmt, adecEEInterestAmount: ldecTaxableAmt, astrContributionType: busConstant.RCContributionTypeEEContr,
                astrContributionSubtype: busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED);

            lbusCalculation.CreateOverpaymentUnderPayment(this.ibusPayeeAccount, lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_WITHDRAWAL_BUY_BACK);

            this.EvaluateInitialLoadRules();
            return larrList;
        }



        public decimal CalculateEEInterestForWithdrawalBuyBack(DateTime adtWithdrawalDate, decimal adecTotalEEAmount)
        {
            decimal ldecEEPartialInterestAmount = busConstant.ZERO_DECIMAL;
            decimal ldecEETInterestAmountTillDate = busConstant.ZERO_DECIMAL;
            decimal ldecBenefitInterestRate = 1.0m;

            int lintYearDiff = this.icdoPaymentHistoryHeader.payment_date.Year - adtWithdrawalDate.Year;
            int lintCurrentYear = 0;

            if (lintCurrentYear == 0)
                lintCurrentYear = adtWithdrawalDate.Year;

            while (lintCurrentYear <= this.icdoPaymentHistoryHeader.payment_date.Year)
            {
                // Get the Benefit Interest Rate

                adecTotalEEAmount = adecTotalEEAmount + ldecEEPartialInterestAmount;

                object lobjBenefitInterestRate = DBFunction.DBExecuteScalar("cdoBenefitInterestRate.GetBenefitInterestRate", new object[1] { lintCurrentYear },
                                                 iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if (lobjBenefitInterestRate.IsNotNull())
                {
                    ldecBenefitInterestRate = (Decimal)lobjBenefitInterestRate;
                }

                // Calculate the Partial Interest Amount
                if (adtWithdrawalDate.Year == this.icdoPaymentHistoryHeader.payment_date.Year)
                {
                    ldecEEPartialInterestAmount = Math.Round((adecTotalEEAmount * ldecBenefitInterestRate) / 12 * (this.icdoPaymentHistoryHeader.payment_date.Month - adtWithdrawalDate.Month), 2);
                }
                else
                {
                    if (this.icdoPaymentHistoryHeader.payment_date.Year == lintCurrentYear)
                    {
                        ldecEEPartialInterestAmount = Math.Round((adecTotalEEAmount * ldecBenefitInterestRate) / 12 * (this.icdoPaymentHistoryHeader.payment_date.Month - 1), 2);

                    }
                    else if (adtWithdrawalDate.Year == lintCurrentYear)
                    {
                        ldecEEPartialInterestAmount = Math.Round((adecTotalEEAmount * ldecBenefitInterestRate) / 12 * (12 - adtWithdrawalDate.Month + 1), 2);
                    }
                    else
                    {
                        ldecEEPartialInterestAmount = Math.Round((adecTotalEEAmount * ldecBenefitInterestRate), 2);
                    }
                }

                ldecEETInterestAmountTillDate += ldecEEPartialInterestAmount;

                lintCurrentYear++;
            }


            return ldecEETInterestAmountTillDate;
        }


        public int GetPaymentHistoryHeaderId(int aintPayeeAccountId)
        {
            DataTable ldtbList = Select("cdoPaymentHistoryHeader.GetPaymentHistryHeaderId", new object[1] { aintPayeeAccountId });

            if (ldtbList.Rows.Count > 0)
            {
                return Convert.ToInt32(ldtbList.Rows[0][enmPaymentHistoryHeader.payment_history_header_id.ToString()]);
            }

            return 0;
        }


        public ArrayList btn_CancelPayment()
        {
            ArrayList iarrError = new ArrayList();

            if (this.iclbPaymentHistoryDistribution != null && this.iclbPaymentHistoryDistribution.Count > 0)
            {
                foreach (busPaymentHistoryDistribution lbusPaymentHistoryDistribution in this.iclbPaymentHistoryDistribution)
                {
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_CANCELLED;
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();

                    lbusPaymentHistoryDistribution.InsertPaymentDistributionStatusHistory(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id,
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.PAYMENT_DISTRIBUTION_STATUS_CANCELLED, DateTime.Now);


                    #region BR -23-010 Payment Adjustment
                    if ((lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ACH ||
                        lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_CHECK || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_WIRE)
                         && this.icdoPaymentHistoryHeader.person_id == this.ibusPayeeAccount.icdoPayeeAccount.person_id)
                    {
                        Collection<cdoProviderReportPayment> lclbProviderReportPayment = new Collection<cdoProviderReportPayment>();
                        DataTable ldtlGetTaxes = Select("cdoPaymentHistoryHeader.GetTaxes", new object[1] { icdoPaymentHistoryHeader.payment_history_header_id });
                        if (ldtlGetTaxes.Rows.Count > 0)
                        {
                            lclbProviderReportPayment = cdoProviderReportPayment.GetCollection<cdoProviderReportPayment>(ldtlGetTaxes);


                            foreach (cdoProviderReportPayment lcdoProviderReportPayment in lclbProviderReportPayment)
                            {
                                this.ibusPayeeAccount.InsertIntoProviderReportPayment(busConstant.SUSSYSTEM_TYPE_PAYMENT_CANCEL, lcdoProviderReportPayment.subsystem_ref_id,
                                    lcdoProviderReportPayment.person_id, lcdoProviderReportPayment.provider_org_id, lcdoProviderReportPayment.payee_account_id,
                                   -(lcdoProviderReportPayment.amount), lcdoProviderReportPayment.payment_history_header_id, lcdoProviderReportPayment.payment_item_type_id);
                            }
                            break;
                        }
                    }
                }
                #endregion BR -23-010 Payment Adjustment


                #region BR -23-011 Payment Adjustment

                foreach (busPaymentHistoryDistribution lbusPaymentHistoryDistribution in this.iclbPaymentHistoryDistribution)
                {
                    //PIR 866 Regarding Cancel Payment Button
                    if (icdoPaymentHistoryHeader.plan_id == busConstant.IAP_PLAN_ID &&
                        (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ACH
                        || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_CHECK
                        || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_WIRE
                        || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_CHECK
                        || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_ACH))
                    {
                        Collection<busPersonAccountRetirementContribution> lclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                        //PIR 866 Regarding Cancel Payment Button
                        DataTable ldtlPersonAccountRetirementContribution = Select("cdoPersonAccountRetirementContribution.GetPersonAccountRetirementContributionForIAP", new object[1] { icdoPaymentHistoryHeader.payment_history_header_id });
                        if (ldtlPersonAccountRetirementContribution.Rows.Count > 0)
                        {
                            lclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtlPersonAccountRetirementContribution, "icdoPersonAccountRetirementContribution");

                            foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in lclbPersonAccountRetirementContribution)
                            {
                                cdoPersonAccountRetirementContribution lcdoRetirementContribution = new cdoPersonAccountRetirementContribution();
                                lcdoRetirementContribution = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution;
                                //PIR 866 Regarding Cancel Payment Button
                                lcdoRetirementContribution.iap_balance_amount = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.iap_balance_amount;
                                lcdoRetirementContribution.created_by = utlPassInfo.iobjPassInfo.istrUserID;
                                lcdoRetirementContribution.created_date = DateTime.Now;
                                lcdoRetirementContribution.modified_by = utlPassInfo.iobjPassInfo.istrUserID;
                                lcdoRetirementContribution.modified_date = DateTime.Now;
                                lcdoRetirementContribution.Insert();
                            }
                            break;
                        }
                    }
                }
                #endregion BR -23-011 Payment Adjustment

                foreach (busPaymentHistoryDistribution lbusPaymentHistoryDistribution in this.iclbPaymentHistoryDistribution)
                {
                    //PIR 999 Regarding Cancel Payment Button
                    if (icdoPaymentHistoryHeader.plan_id == busConstant.MPIPP_PLAN_ID &&
                        (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ACH
                        || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_CHECK
                        || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_WIRE
                        || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_CHECK
                        || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_ACH))
                    {
                        Collection<busPersonAccountRetirementContribution> lclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                        //PIR 999 Regarding Cancel Payment Button
                        DataTable ldtlPersonAccountRetirementContribution = Select("cdoPersonAccountRetirementContribution.GetPersonAccountRetirementContributionEEUVHP", new object[1] { icdoPaymentHistoryHeader.payment_history_header_id });
                        if (ldtlPersonAccountRetirementContribution.Rows.Count > 0)
                        {
                            lclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtlPersonAccountRetirementContribution, "icdoPersonAccountRetirementContribution");

                            foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in lclbPersonAccountRetirementContribution)
                            {
                                cdoPersonAccountRetirementContribution lcdoRetirementContribution = new cdoPersonAccountRetirementContribution();
                                lcdoRetirementContribution = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution;
                                //PIR 999 Regarding Cancel Payment Button
                                lcdoRetirementContribution.ee_contribution_amount = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_contribution_amount;
                                lcdoRetirementContribution.ee_int_amount = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount;
                                lcdoRetirementContribution.uvhp_amount = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_amount;
                                lcdoRetirementContribution.uvhp_int_amount = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount;
                                lcdoRetirementContribution.transaction_date = DateTime.Now;
                                lcdoRetirementContribution.created_by = utlPassInfo.iobjPassInfo.istrUserID;
                                lcdoRetirementContribution.created_date = DateTime.Now;
                                lcdoRetirementContribution.modified_by = utlPassInfo.iobjPassInfo.istrUserID;
                                lcdoRetirementContribution.modified_date = DateTime.Now;
                                lcdoRetirementContribution.Insert();
                            }
                            break;
                        }
                    }
                }

            }



            iarrError.Add(this);
            this.EvaluateInitialLoadRules();
            return iarrError;
        }

        #endregion

        public ArrayList btn_StopPayment()
        {
            ArrayList iarrError = new ArrayList();
            if (this.iclbPaymentHistoryDistribution != null && this.iclbPaymentHistoryDistribution.Count() > 0)
            {
                foreach (busPaymentHistoryDistribution lbusPaymentHistoryDistribution in this.iclbPaymentHistoryDistribution)
                {
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_STOP_PAYMENT;
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();

                    lbusPaymentHistoryDistribution.InsertPaymentDistributionStatusHistory(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id,
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.PAYMENT_DISTRIBUTION_STATUS_STOP_PAYMENT, DateTime.Now);

                }

            }
            iarrError.Add(this);
            this.EvaluateInitialLoadRules();
            return iarrError;

        }
        public ArrayList btn_Outstanding()
        {
            ArrayList iarrError = new ArrayList();
            if (this.iclbPaymentHistoryDistribution != null && this.iclbPaymentHistoryDistribution.Count() > 0)
            {
                foreach (busPaymentHistoryDistribution lbusPaymentHistoryDistribution in this.iclbPaymentHistoryDistribution)
                {
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_OUTSTANDING;
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();

                    lbusPaymentHistoryDistribution.InsertPaymentDistributionStatusHistory(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id,
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.PAYMENT_DISTRIBUTION_STATUS_OUTSTANDING, DateTime.Now);

                }

            }

            iarrError.Add(this);
            this.EvaluateInitialLoadRules();
            return iarrError;


        }
        public ArrayList btn_CreateReclamation()
        {
            ArrayList iarrError = new ArrayList();
            if (this.iclbPaymentHistoryDistribution != null && this.iclbPaymentHistoryDistribution.Count() > 0)
            {
                foreach (busPaymentHistoryDistribution lbusPaymentHistoryDistribution in this.iclbPaymentHistoryDistribution)
                {
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_RECLAMATION_PENDING;
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();

                    lbusPaymentHistoryDistribution.InsertPaymentDistributionStatusHistory(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id,
                    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.PAYMENT_DISTRIBUTION_STATUS_RECLAMATION_PENDING, DateTime.Now);

                    //Ticket#89470
                    Collection<busPersonAccountRetirementContribution> lclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                    DataTable ldtlPersonAccountRetirementContribution = Select("cdoPersonAccountRetirementContribution.GetPersonAccountRetirementContributionEEUVHP", new object[1] { icdoPaymentHistoryHeader.payment_history_header_id });
                    if (ldtlPersonAccountRetirementContribution.Rows.Count > 0)
                    {
                        lclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtlPersonAccountRetirementContribution, "icdoPersonAccountRetirementContribution");

                        foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in lclbPersonAccountRetirementContribution)
                        {
                            cdoPersonAccountRetirementContribution lcdoRetirementContribution = new cdoPersonAccountRetirementContribution();
                            lcdoRetirementContribution = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution;

                            lcdoRetirementContribution.ee_contribution_amount = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_contribution_amount;
                            lcdoRetirementContribution.ee_int_amount = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount;
                            lcdoRetirementContribution.uvhp_amount = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_amount;
                            lcdoRetirementContribution.uvhp_int_amount = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount;
                            lcdoRetirementContribution.transaction_date = DateTime.Now;
                            lcdoRetirementContribution.created_by = utlPassInfo.iobjPassInfo.istrUserID;
                            lcdoRetirementContribution.created_date = DateTime.Now;
                            lcdoRetirementContribution.modified_by = utlPassInfo.iobjPassInfo.istrUserID;
                            lcdoRetirementContribution.modified_date = DateTime.Now;
                            lcdoRetirementContribution.Insert();
                        }
                        break;
                    }
                }

                //Collection<cdoProviderReportPayment> lclbProviderReportPayment = new Collection<cdoProviderReportPayment>();
                //DataTable ldtlGetTaxes = Select("cdoPaymentHistoryHeader.GetTaxes", new object[1] { icdoPaymentHistoryHeader.payment_history_header_id });
                //if (ldtlGetTaxes.Rows.Count > 0)
                //{
                //    lclbProviderReportPayment = cdoProviderReportPayment.GetCollection<cdoProviderReportPayment>(ldtlGetTaxes);


                //    foreach (cdoProviderReportPayment lcdoProviderReportPayment in lclbProviderReportPayment)
                //    {
                //        this.ibusPayeeAccount.InsertIntoProviderReportPayment(busConstant.SUSSYSTEM_TYPE_PAYMENT_RECLAIMED, lcdoProviderReportPayment.subsystem_ref_id,
                //            lcdoProviderReportPayment.person_id, lcdoProviderReportPayment.provider_org_id, lcdoProviderReportPayment.payee_account_id,
                //            -(lcdoProviderReportPayment.amount), lcdoProviderReportPayment.payment_history_header_id, lcdoProviderReportPayment.payment_item_type_id);
                //    }
                //}

            }

            iarrError.Add(this);
            this.EvaluateInitialLoadRules();
            return iarrError;
        }


        #region Business rules

        public bool ShowCreateOverpaymentButton()
        {
            if (iclbPaymentHistoryDistribution != null && iclbPaymentHistoryDistribution.Count() > 0)
            {
                busPaymentHistoryDistribution lbusPaymentHistoryDistribution = iclbPaymentHistoryDistribution.First();
                if ((lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_CHECK) && (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_CLEARED))
                    return true;

                else if (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ACH && lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_CLEARED)
                    return true;
                else if (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_WIRE && lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_CLEARED)
                    return true;
            }
            return false;

        }
        public bool ShowCancelPaymentButton()
        {
            if (iclbPaymentHistoryDistribution != null && iclbPaymentHistoryDistribution.Count() > 0)
            {
                if (iclbPaymentHistoryDistribution.Where(item => item.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_CHECK).Count() > 0)
                {       //Ticket#71644
                    if (iclbPaymentHistoryDistribution.Where(item => item.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_STOP_PAYMENT || item.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_STALE || item.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_3YRS).Count() > 0)
                        return true;

                }

                else if (iclbPaymentHistoryDistribution.Where(item => item.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ACH && item.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_STOP_PAYMENT).Count() > 0)
                    return true;
                else if (iclbPaymentHistoryDistribution.Where(item => item.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_WIRE && item.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_CLEARED).Count() > 0)
                    return true;
                //PIR 866 Regarding Cancel Payment Button
                else if (iclbPaymentHistoryDistribution.Where(item =>
                     (item.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_ACH
                     || item.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_CHECK)
                     && item.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_STOP_PAYMENT).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool ShowReissuePaymentButton()
        {
            if (iclbPaymentHistoryDistribution != null && iclbPaymentHistoryDistribution.Count() > 0)
            {
                busPaymentHistoryDistribution lbusPaymentHistoryDistribution = iclbPaymentHistoryDistribution.First();
                if ((lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_CHECK || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_CHECK) && (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_STOP_PAYMENT || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_STALE || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_REISSUE))
                    return true;
                else if ((lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ACH || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_ACH) && (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_STOP_PAYMENT || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_RECLAIMED || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_REISSUE))
                    return true;
                //1040: Reissue button should visible for payment greater than 3 yrs status.
                else if ((lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_CHECK || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_CHECK) && (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_STOP_PAYMENT || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_3YRS || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_REISSUE))
                    return true;


            }
            return false;
        }
        public bool ShowStopPaymentButton()
        {
            if (iclbPaymentHistoryDistribution != null && iclbPaymentHistoryDistribution.Count() > 0)
            {
                busPaymentHistoryDistribution lbusPaymentHistoryDistribution = iclbPaymentHistoryDistribution.First();
                if ((lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ACH || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_ACH) && lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_OUTSTANDING)
                    return true;
                else if ((lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_CHECK || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_CHECK) && lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_OUTSTANDING)
                    return true;
                else if ((lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value != busConstant.PAYMENT_METHOD_WIRE || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_CHECK) && lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_OUTSTANDING)
                    return true;
            }
            return false;
        }
        public bool ShowOutstandingButton()
        {
            if (iclbPaymentHistoryDistribution != null && iclbPaymentHistoryDistribution.Count() > 0)
            {
                busPaymentHistoryDistribution lbusPaymentHistoryDistribution = iclbPaymentHistoryDistribution.First();
                if (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_CHECK || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ACH || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_WIRE)
                {
                    if (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_STOP_PAYMENT || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_REISSUE)
                        return true;
                }
            }
            return false;
        }
        public bool ShowReclamationButtton()
        {
            if (iclbPaymentHistoryDistribution != null && iclbPaymentHistoryDistribution.Count() > 0)
            {
                busPaymentHistoryDistribution lbusPaymentHistoryDistribution = iclbPaymentHistoryDistribution.First();
                if (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ACH)
                {
                    if ((lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_OUTSTANDING || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_CLEARED) && (this.icdoPaymentHistoryHeader.payment_date <= DateTime.Now))
                        return true;

                }
                if (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value != busConstant.PAYMENT_METHOD_WIRE)
                {
                    if ((lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_OUTSTANDING || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_CLEARED) && (this.icdoPaymentHistoryHeader.payment_date <= DateTime.Now))
                        return true;

                }

            }
            return false;
        }



        #endregion
    }
}

