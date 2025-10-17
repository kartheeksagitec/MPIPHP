#region Using directives

using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.Interface;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPayeeAccountStatus:
    /// Inherited from busPayeeAccountStatusGen, the class is used to customize the business object busPayeeAccountStatusGen.
    /// </summary>
    [Serializable]
    public class busPayeeAccountStatus : busPayeeAccountStatusGen
    {
        public Collection<busPayeeAccountPaymentItemType> iclbPayeeAccountPaymentItemTypeActive { get; set; }

        public bool IsStatusCompleted()
        {

            if (this.icdoPayeeAccountStatus.status_value == busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED)
                return true;
            return false;
        }

        public bool IsStatusCancelled()
        {
            if (this.icdoPayeeAccountStatus.status_value == busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED)
                return true;
            return false;
        }

        public ArrayList CheckErrorOnAddButton(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false)
        {
            utlError lobjError = null;
            string astrPrimaryFlag = string.Empty;
            int RollOverCount;
            DateTime idtDateTime;
            int iintTaxwithholdingCount = 0;
            busPayeeAccount lbusPayeeAccount = aobj as busPayeeAccount;
            busPayeeAccountStatus lbusPayeeAccountStatus = null;
            if (!lbusPayeeAccount.iclbPayeeAccountStatus.IsNullOrEmpty())
            {
                lbusPayeeAccountStatus = (from item in lbusPayeeAccount.iclbPayeeAccountStatus
                                          orderby item.icdoPayeeAccountStatus.status_effective_date descending
                                          select item).FirstOrDefault();
            }

            if (ahstParams.Count > 0)
            {
                ahstParams["icdoPayeeAccountStatus.status_value"] = ahstParams["status_value"];
                ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"] = ahstParams["suspension_status_reason_value"];
                ahstParams["icdoPayeeAccountStatus.terminated_status_reason_value"] = ahstParams["terminated_status_reason_value"];
                ahstParams["icdoPayeeAccountStatus.termination_reason_description"] = ahstParams["termination_reason_description"];
                ahstParams["icdoPayeeAccountStatus.suspension_reason_description"] = ahstParams["suspension_reason_description"];
                //PROD PIR 792
                if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED)
                {
                    int iintPaymentCount = (int)DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.GetPaymentCount", new object[3] { lbusPayeeAccount.icdoPayeeAccount.person_id, lbusPayeeAccount.icdoPayeeAccount.plan_benefit_id, lbusPayeeAccount.icdoPayeeAccount.benefit_calculation_detail_id },
                                                         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (iintPaymentCount > 0)
                    {
                        lobjError = AddError(6102, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }
                // PROD PIR 125
                if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED)
                {
                    string istrScheduleType = string.Empty;
                    if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId != 1)
                        istrScheduleType = busConstant.PaymentScheduleTypeMonthly;
                    else
                        istrScheduleType = busConstant.PaymentScheduleTypeWeekly;

                    //PIR 378
                    bool IsManager = false;
                    busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
                    if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
                    {
                        IsManager = true;
                    }


                    int lintCountSchedule = (int)DBFunction.DBExecuteScalar("cdoPerson.CheckPaymentSchedulebeforeApprovestatus", new object[1] { istrScheduleType }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (lintCountSchedule > 0 && !IsManager)
                    {
                        lobjError = AddError(6221, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                    lbusPayeeAccount.LoadPayeeAccountWireDetail();
                    if (lbusPayeeAccount.iclbPayeeAccountWireDetail.IsNotNull())
                    {
                        if (lbusPayeeAccount.iclbPayeeAccountWireDetail.Count() > 0)
                        {
                            if (lbusPayeeAccount.iclbPayeeAccountWireDetail.Where(i => (i.icdoPayeeAccountWireDetail.wire_end_date == DateTime.MinValue || i.icdoPayeeAccountWireDetail.wire_end_date >= DateTime.Now) && i.icdoPayeeAccountWireDetail.call_back_flag == "N").Count() > 0)
                            {
                                lobjError = AddError(6310,"");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;

                            }


                        }

                    }



                    //LA Sunset - Payment Directives
                    //Ticket - 72131
                    if (lbusPayeeAccount != null && lbusPayeeAccount.iclbPaymentDirectives != null && lbusPayeeAccount.iclbPaymentDirectives.Count > 0
                        && lbusPayeeAccount.iclbPaymentDirectives.Where(item => item.icdoPaymentDirectives.payment_cycle_date.Date < lbusPayeeAccount.idtNextBenefitPaymentDate.Date
                        && item.icdoPaymentDirectives.payment_cycle_date.Date >= DateTime.Today.Date).Count() > 0)
                    {
                        ;
                    }
                    else if (!(lbusPayeeAccount != null && lbusPayeeAccount.iclbPaymentDirectives != null && lbusPayeeAccount.iclbPaymentDirectives.Count > 0
                        && lbusPayeeAccount.iclbPaymentDirectives.Where(item => item.icdoPaymentDirectives.payment_cycle_date.Date >= lbusPayeeAccount.idtNextBenefitPaymentDate.Date).Count() > 0))
                    {
                        lobjError = AddError(0, "Please generate payment directive for the next payment cycle before approving the payee account");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }

                    //U24074
                    if (lbusPayeeAccount.ibusPayee.icdoPerson.adverse_interest_flag == "Y" && (lbusPayeeAccount.icdoPayeeAccount.adverse_interest_flag.IsNullOrEmpty() || lbusPayeeAccount.icdoPayeeAccount.adverse_interest_flag == "N"))
                    {
                        lobjError = AddError(0, "Please verify any possible Adverse Interest before approving.");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }
                // PIR PROD 73
                if ((iobjPassInfo.istrUserID.ToLower() == lbusPayeeAccountStatus.icdoPayeeAccountStatus.modified_by.ToLower() || iobjPassInfo.istrUserID.ToLower() == lbusPayeeAccountStatus.icdoPayeeAccountStatus.created_by.ToLower())
                    && (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED)
                    && (lbusPayeeAccount.icdoPayeeAccount.retiree_incr_flag == busConstant.FLAG_NO || Convert.ToString(lbusPayeeAccount.icdoPayeeAccount.retiree_incr_flag).IsNullOrEmpty()))
                {
                    lobjError = AddError(6219, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                #region PROD PIR 148
                if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED
                    && lbusPayeeAccount.ibusPayee.IsNotNull()
                    && lbusPayeeAccount.ibusPayee.icdoPerson.ssn.IsNullOrEmpty())
                {
                    lobjError = AddError(6222, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
                if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED
                    && lbusPayeeAccount.ibusPayee.IsNotNull()
                    && (lbusPayeeAccount.ibusPayee.icdoPerson.date_of_birth.IsNull() || lbusPayeeAccount.ibusPayee.icdoPerson.date_of_birth == DateTime.MinValue))
                {
                    lobjError = AddError(6222, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
                #endregion

                // Don't Delete. Uncomment Before UAT Start.
                if ((Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED
                    && lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.BENEFIT_TYPE_DISABILITY)
                    || Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING)
                {
                    lobjError = AddError(6134, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]).IsNullOrEmpty())
                {
                    lobjError = AddError(6011, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }


                if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]).IsNotNullOrEmpty())
                {

                    //Payment Adjustment BR-023-46
                    if ((Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED) &&
                       lbusPayeeAccount.iclbPayeeAccountBenefitOverpayment != null && lbusPayeeAccount.iclbPayeeAccountBenefitOverpayment.Count > 0)
                    {
                        lbusPayeeAccount.LoadNextBenefitPaymentDate();
                        DataTable ldtblBenefitAmount = Select("cdoRepaymentSchedule.GetBenefitAmount", new object[2] { lbusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                           lbusPayeeAccount.idtNextBenefitPaymentDate });

                        if (ldtblBenefitAmount.Rows.Count > 0)
                        {
                            foreach (busPayeeAccountBenefitOverpayment lbusPayeeAccountBenefitOverpayment in lbusPayeeAccount.iclbPayeeAccountBenefitOverpayment)
                            {
                                DataTable ldtblNextAmountDue = Select("cdoRepaymentSchedule.GetNextAmountDue", new object[1] { lbusPayeeAccountBenefitOverpayment.icdoPayeeAccountRetroPayment.payee_account_retro_payment_id });
                                if (ldtblNextAmountDue != null && ldtblNextAmountDue.Rows.Count > 0 && Convert.ToString(ldtblNextAmountDue.Rows[0][0]).IsNotNullOrEmpty())
                                {
                                    if (Convert.ToDecimal(ldtblNextAmountDue.Rows[0][0]) > Convert.ToDecimal(ldtblBenefitAmount.Rows[0][0]))
                                    {
                                        lobjError = AddError(6106, "");
                                        aarrErrors.Add(lobjError);
                                        return aarrErrors;
                                    }
                                }
                            }
                        }
                    }


                    //Disability Re-certification 
                    //If user is trying to approve a payee account, where the disability recert date
                    //is sent but no received date is there, then throw an error saying that Payee Account cannot be approved as Disability needs to be re-certified.
                    if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED
                        && lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        DataTable ldtblDisabilityBenefitHistory = Select("cdoPayeeAccountStatus.CheckDisabilityStatusHistory", new object[1] { lbusPayeeAccount.icdoPayeeAccount.benefit_application_detail_id });
                        if (ldtblDisabilityBenefitHistory != null && ldtblDisabilityBenefitHistory.Rows.Count > 0)
                        {
                            //PIR RID 70576 Added code to find payee age and only raise error if less than 65.
                            busPerson lbusPerson = new busPerson();
                            lbusPerson.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id);
                            decimal ldecAge = busGlobalFunctions.CalculatePersonAge(lbusPerson.icdoPerson.idtDateofBirth, DateTime.Now);
                            if (Convert.ToString(ldtblDisabilityBenefitHistory.Rows[0][enmDisabilityBenefitHistory.sent.ToString().ToUpper()]).IsNotNullOrEmpty()
                                && Convert.ToString(ldtblDisabilityBenefitHistory.Rows[0][enmDisabilityBenefitHistory.sent.ToString().ToUpper()]) == busConstant.FLAG_YES
                                && ldecAge < 65
                                )
                            {
                                lobjError = AddError(6137, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                        }
                        //// PROD PIR 214
                        //if (lbusPayeeAccount.icdoPayeeAccount.retiree_incr_flag != "Y")
                        //{
                        //    int lintSharedInterestDRO = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckIfSharedInterestDROExist", new object[1] { lbusPayeeAccount.icdoPayeeAccount.person_id },
                        //         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                        //    if (lintSharedInterestDRO > 0)
                        //    {
                        //        int lintSharedInterestPayeeAccount = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckIfSharedInterestPayeeAccountExist", new object[1] { lbusPayeeAccount.icdoPayeeAccount.payee_account_id },
                        //        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);

                        //        if (lintSharedInterestPayeeAccount == 0)
                        //        {
                        //            // PROD PIR 214
                        //            int lintSharedInterestPayeeAccountNotINCompleteOrCancelled = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckPayeeAccountNotINCompleteorCanclStatus", new object[2] { lbusPayeeAccount.icdoPayeeAccount.person_id, lbusPayeeAccount.icdoPayeeAccount.iintPlanId },
                        //                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                        //            if (lintSharedInterestPayeeAccountNotINCompleteOrCancelled > 0)
                        //            {
                        //                lobjError = AddError(6186, "");
                        //                aarrErrors.Add(lobjError);
                        //                return aarrErrors;
                        //            }
                        //        }
                        //    }
                        //}
                    }


                    if ((Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED) && (Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"]).IsNullOrEmpty()
                        || Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"]) == ""))
                    {
                        lobjError = AddError(6031, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }

                    if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) != busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED
                        && Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"]).IsNotNullOrEmpty())

                    {
                        lobjError = AddError(6041, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }

                    //PIR 930
                    if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED &&
                        Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"]).IsNotNullOrEmpty()
                        &&
                        (
                           Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"]) == busConstant.PAYEE_ACCOUNT_SUSPENSION_REASON_REEMPLOYED_RULE_1
                           ||
                           Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"]) == busConstant.PAYEE_ACCOUNT_SUSPENSION_REASON_REEMPLOYED_RULE_2
                           ||
                           Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"]) == busConstant.PAYEE_ACCOUNT_SUSPENSION_REASON_REEMPLOYED_RULE_3
                           ||
                           Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"]) == busConstant.PAYEE_ACCOUNT_SUSPENSION_REASON_REEMPLOYED_RULE_4
                           ||
                           Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"]) == busConstant.PAYEE_ACCOUNT_SUSPENSION_REASON_REEMPLOYED_RULE_5
                           ||
                           Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"]) == "REED"
                        ))
                    {
                        DateTime ldtVestedDt = new DateTime();
                        DateTime ldtMinDt = new DateTime();

                        DataTable ldtGetVestedDate = busBase.Select("cdoPersonAccountEligibility.GetVestedDateForMD", new object[1] { lbusPayeeAccount.ibusPayee.icdoPerson.person_id });
                        if (ldtGetVestedDate != null && ldtGetVestedDate.Rows.Count > 0 && (Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]) != DateTime.MinValue))
                        {
                            ldtVestedDt = Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]);
                        }

                        ldtMinDt = busGlobalFunctions.GetMinDistributionDate(lbusPayeeAccount.ibusPayee.icdoPerson.person_id, ldtVestedDt); // busConstant.BenefitCalculation.AGE_70_HALF


                        if (lbusPayeeAccount.idtNextBenefitPaymentDate.Date >= ldtMinDt.Date)
                        {

                            lobjError = AddError(6210, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }
                    }

                    if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.terminated_status_reason_value"]) == busConstant.CANCELLATION_REASON_OTHER && Convert.ToString(ahstParams["icdoPayeeAccountStatus.termination_reason_description"]).IsNullOrEmpty())
                    {
                        lobjError = AddError(6030, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }

                    if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_status_reason_value"]) == busConstant.CANCELLATION_REASON_OTHER && Convert.ToString(ahstParams["icdoPayeeAccountStatus.suspension_reason_description"]).IsNullOrEmpty())
                    {
                        lobjError = AddError(6031, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }

                    if (lbusPayeeAccount.iclbPayeeAccountStatus.IsNotNull())
                    {
                        if (lbusPayeeAccountStatus != null && lbusPayeeAccountStatus.icdoPayeeAccountStatus.status_value == Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]))
                        {
                            lobjError = AddError(6080, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }
                    }
                    if (lbusPayeeAccount.icdoPayeeAccount.reemployed_flag == busConstant.FLAG_YES && lbusPayeeAccountStatus.icdoPayeeAccountStatus.status_value == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED)
                    {
                        lobjError = AddError(6087, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;

                    }


                    if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_STATUS_APPROVED)
                    {
                        int iintRolloverCount = 0;
                        if (lbusPayeeAccount.iclbPayeeAccountTaxWithholding == null)
                            lbusPayeeAccount.LoadPayeeAccountTaxWithholdings();

                        // PROD PIR 518
                        if (lbusPayeeAccount.iclbActiveRolloverDetails == null)
                            lbusPayeeAccount.LoadActiveRolloverDetail();
                        if (lbusPayeeAccount.iclbActiveRolloverDetails.Count() > 0)
                        {
                            iintRolloverCount = lbusPayeeAccount.iclbActiveRolloverDetails.Where(item => item.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfGross
                                || item.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfTaxable).Count();
                        }

                        iintTaxwithholdingCount = lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Count();
                        if (iintTaxwithholdingCount == 0 && iintRolloverCount == 0 && lbusPayeeAccount.icdoPayeeAccount.transfer_org_id == 0)
                        {
                            lobjError = AddError(6034, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }
                        //PIR-810
                        if (lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY
                            && lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT
                            && lbusPayeeAccount.icdoPayeeAccount.family_relation_value != busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE
                            && lbusPayeeAccount.icdoPayeeAccount.org_id != 0)
                        {
                            if (lbusPayeeAccount.ibusOrganization != null
                                && string.IsNullOrEmpty(lbusPayeeAccount.ibusOrganization.icdoOrganization.federal_id))
                            {
                                lobjError = AddError(6234, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                        }
                    }

                    if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED)
                    {
                        //int lintCheckDistributionStatusValue = (int)DBFunction.DBExecuteScalar("cdoPaymentHistoryHeader.GetDistributionIDFromPayeeAccountID",
                        //        new object[1] { lbusPayeeAccount.icdoPayeeAccount.payee_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        //if (lintCheckDistributionStatusValue > 0)
                        //{
                        lobjError = AddError(6102, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                        // }
                    }
                }
                //ask to abhishek
                if (Convert.ToString(ahstParams["icdoPayeeAccountStatus.status_value"]) == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED)
                {
                    // PROD PIR 295 Fix.
                    #region Commented code
                    //decimal idecRemainingPercentage = 100M;

                    //if (lbusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    //    lbusPayeeAccount.LoadNextBenefitPaymentDate();

                    //if(lbusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                    //lbusPayeeAccount.LoadPayeeAccountPaymentItemType();

                    //lbusPayeeAccount.LoadBenefitDetails();
                    //lbusPayeeAccount.LoadDRODetails();

                    //if (lbusPayeeAccount.iclbWithholdingInformation == null)
                    //{
                    //    lbusPayeeAccount.LoadWithholdingInformation();
                    //}

                    //busWithholdingInformation lbusWithholdingInformation = (from item in lbusPayeeAccount.iclbWithholdingInformation
                    //                                                        where busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                    //                                                        item.icdoWithholdingInformation.withholding_date_from, item.icdoWithholdingInformation.withholding_date_to)
                    //                                                        select item).FirstOrDefault();
                    //if (lbusWithholdingInformation != null)
                    //{
                    //    if (lbusWithholdingInformation.icdoWithholdingInformation.withholding_percentage != null)
                    //        idecRemainingPercentage = (idecRemainingPercentage - lbusWithholdingInformation.icdoWithholdingInformation.withholding_percentage) / 100M;
                    //}
                    //else
                    //{
                    //    idecRemainingPercentage = 1M;
                    //}
                    //// Active Payee Account Payment Item type
                    //lbusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive = (from item in lbusPayeeAccount.iclbPayeeAccountPaymentItemType
                    //                                         where busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                    //                                         item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                    //                                         select item).ToList().ToCollection<busPayeeAccountPaymentItemType>();

                    //decimal idecTotalNetPaymentACH = (from obj in lbusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.AsEnumerable()
                    //                                select obj.icdoPayeeAccountPaymentItemType.amount * obj.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum()
                    //                                +
                    //                                ((from obj in lbusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.AsEnumerable()
                    //                                where obj.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolloverItemReductionCheck
                    //                                select obj.icdoPayeeAccountPaymentItemType.amount).Sum()) * idecRemainingPercentage;
                    #endregion

                    lbusPayeeAccount.LoadBreakDownDetails();
                    decimal idecTotalNetPaymentACH = lbusPayeeAccount.idecNextGrossPaymentACH + lbusPayeeAccount.idecRetroAdjustmentAmount + lbusPayeeAccount.idecNextGrossPaymentRollOver; //PROD PIR 389
                    if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                    {
                        if (idecTotalNetPaymentACH >= (Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(7034, busConstant.PensionMonthlyLimit)))
                            && (lbusPayeeAccount.icdoPayeeAccount.verified_flag.IsNullOrEmpty() || lbusPayeeAccount.icdoPayeeAccount.verified_flag == busConstant.FLAG_NO))
                        {
                            lobjError = AddError(6036, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }
                    }
                    else
                    {
                        if (idecTotalNetPaymentACH >= (Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(7034, busConstant.IAPMonthlyLimit)))
                            && (lbusPayeeAccount.icdoPayeeAccount.verified_flag.IsNullOrEmpty() || lbusPayeeAccount.icdoPayeeAccount.verified_flag == busConstant.FLAG_NO))
                        {
                            lobjError = AddError(6037, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }
                    }
                    if (lbusPayeeAccount.iclbPayeeAccountAchDetail != null)
                        lbusPayeeAccount.LoadPayeeAccountAchDetails();
                    int ValAchCount = lbusPayeeAccount.iclbPayeeAccountAchDetail.Where(item => busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                                                         item.icdoPayeeAccountAchDetail.ach_start_date, item.icdoPayeeAccountAchDetail.ach_end_date) && item.icdoPayeeAccountAchDetail.istrPreNoteFlag != "Y").Count();
                    if (ValAchCount == 0)
                    {
                        Boolean iboolMailAddspresent = false;
                        if (lbusPayeeAccount.ibusPayee != null)
                        {
                            lbusPayeeAccount.ibusPayee.LoadPersonAddresss();
                            foreach (busPersonAddress lbusPersonAddress in lbusPayeeAccount.ibusPayee.iclbPersonAddress)
                            {
                                if ((lbusPersonAddress.icdoPersonAddress.end_date > DateTime.Now || lbusPersonAddress.icdoPersonAddress.end_date == DateTime.MinValue
                                    || lbusPersonAddress.icdoPersonAddress.end_date.ToShortDateString() == DateTime.Now.ToShortDateString()) && // PROD PIR 329
                                   lbusPersonAddress.icdoPersonAddress.start_date <= DateTime.Now && lbusPersonAddress.icdoPersonAddress.start_date != lbusPersonAddress.icdoPersonAddress.end_date)
                                {
                                    foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in lbusPersonAddress.iclcPersonAddressChklist)
                                    {
                                        if (lcdoPersonAddressChklist.address_type_value == busConstant.MAILING_ADDRESS || lcdoPersonAddressChklist.address_type_value == busConstant.PHYSICAL_AND_MAILING_ADDRESS)
                                        {
                                            iboolMailAddspresent = true;
                                        }
                                    }
                                }
                            }
                        }
                        else if (lbusPayeeAccount.ibusOrganization != null)
                        {
                            lbusPayeeAccount.ibusOrganization.LoadOrgAddresss();
                            foreach (busOrgAddress lbusOrgAddress in lbusPayeeAccount.ibusOrganization.iclbOrgAddress)
                            {
                                if ((lbusOrgAddress.icdoOrgAddress.end_date >= DateTime.Now || lbusOrgAddress.icdoOrgAddress.end_date == DateTime.MinValue
                                    || lbusOrgAddress.icdoOrgAddress.end_date.ToShortDateString() == DateTime.Now.ToShortDateString()) && // PROD PIR 329
                                   lbusOrgAddress.icdoOrgAddress.start_date <= DateTime.Now && lbusOrgAddress.icdoOrgAddress.start_date != lbusOrgAddress.icdoOrgAddress.end_date)
                                {
                                    if (lbusOrgAddress.icdoOrgAddress.address_type_value == busConstant.MAILING_ADDRESS || lbusOrgAddress.icdoOrgAddress.address_type_value == busConstant.PHYSICAL_AND_MAILING_ADDRESS)
                                        iboolMailAddspresent = true;
                                }
                            }
                        }

                        //rohan 10212014 PIR 803
                        bool lblnFlag = true;
                        if (lbusPayeeAccount.idecNextNetPaymentACH == 0 && lbusPayeeAccount.iclbPayeeAccountRolloverDetail != null &&
                            lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Count > 0 && lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Where(t => t.icdoPayeeAccountRolloverDetail.status_value == "ACTV").Count() > 0)
                        {
                            lblnFlag = false;
                        }

                        if (!iboolMailAddspresent && lblnFlag)
                        {
                            lobjError = AddError(6216, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }



                    }
                    if (lbusPayeeAccount.iclbPayeeAccountDeduction == null)
                        lbusPayeeAccount.LoadPayeeAccountDeduction();
                    lbusPayeeAccount.iclbPayeeAccountDeductionActive = (from item in lbusPayeeAccount.iclbPayeeAccountDeduction
                                                                        where busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                                                       item.icdoPayeeAccountDeduction.start_date, item.icdoPayeeAccountDeduction.end_date)
                                                                        select item).ToList().ToCollection<busPayeeAccountDeduction>();
                    if (lbusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        lbusPayeeAccount.LoadNextBenefitPaymentDate();
                    foreach (busPayeeAccountDeduction lbusPayeeAccountDeduction in lbusPayeeAccount.iclbPayeeAccountDeductionActive)
                    {
                        if (!lbusPayeeAccountDeduction.CheckMailAddress(lbusPayeeAccount.idtNextBenefitPaymentDate))
                        {
                            lobjError = AddError(6217, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }
                    }
                }

            }
            return aarrErrors;
        }

        public void InsertValuesInPayeeAccountStatus(int aintPayeeAccountId, string astrStatus, DateTime adtEffectiveDate, string astrSuspensionReasonValue = "", int aintJobScheduleId = 0)
        {
            //PIR 1051
            if (icdoPayeeAccountStatus == null)
            {
                icdoPayeeAccountStatus = new cdoPayeeAccountStatus();
            }

            busPayeeAccount lbusTempPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            lbusTempPayeeAccount.ibusCurrentActivePayeeAccount = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };

            if (lbusTempPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lbusTempPayeeAccount.LoadPayeeAccountStatuss();
            }

            if (!lbusTempPayeeAccount.iclbPayeeAccountStatus.IsNullOrEmpty())
            {
                lbusTempPayeeAccount.ibusCurrentActivePayeeAccount = lbusTempPayeeAccount.iclbPayeeAccountStatus[0];
            }

            if (lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.icdoPayeeAccountStatus.status_value == astrStatus)
            {
                lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.icdoPayeeAccountStatus.suspension_status_reason_id = busConstant.Payee_Account_Suspension_Reason_id;
                lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.icdoPayeeAccountStatus.suspension_status_reason_value = astrSuspensionReasonValue;
                lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.icdoPayeeAccountStatus.terminated_status_reason_id = busConstant.Payee_Account_Terminated_Status_Reason_id;
                lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.icdoPayeeAccountStatus.status_effective_date = adtEffectiveDate;
                lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.icdoPayeeAccountStatus.Update();
            }
            else
            {
                this.icdoPayeeAccountStatus.payee_account_id = aintPayeeAccountId;
                this.icdoPayeeAccountStatus.status_id = busConstant.Payee_Account_Status_ID;
                this.icdoPayeeAccountStatus.status_value = astrStatus;
                this.icdoPayeeAccountStatus.suspension_status_reason_id = busConstant.Payee_Account_Suspension_Reason_id;
                this.icdoPayeeAccountStatus.suspension_status_reason_value = astrSuspensionReasonValue;
                this.icdoPayeeAccountStatus.terminated_status_reason_id = busConstant.Payee_Account_Terminated_Status_Reason_id;
                this.icdoPayeeAccountStatus.status_effective_date = adtEffectiveDate;
                this.icdoPayeeAccountStatus.Insert();
            }
            if (aintJobScheduleId == busConstant.MPIPHPBatch.REEMPLOYED_BATCH)
            {
                if (CheckIfInprogessBPMInstances(lbusTempPayeeAccount.icdoPayeeAccount.person_id))
                {
                    busReturnToWorkRequest lbusReturnToWorkRequest = new busReturnToWorkRequest { icdoReturnToWorkRequest = new cdoReturnToWorkRequest() };
                    lbusTempPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
                    lbusTempPayeeAccount.ibusPayee.FindPerson(lbusTempPayeeAccount.icdoPayeeAccount.person_id);

                    object larrTemp = lbusReturnToWorkRequest.NewReturnToWorkRequest(lbusTempPayeeAccount.ibusPayee.icdoPerson.mpi_person_id, busConstant.ReturnToWorkRequest.REQUEST_TYPE_RTW, busConstant.ReturnToWorkRequest.SOURCE_BATCH);
                }
            }
        }
        public bool CheckIfInprogessBPMInstances(int aintPersonId)
        {
            //Active process already exists for person.
            if (aintPersonId != 0)
            {
                Collection<IDbDataParameter> lclbParameters = new Collection<IDbDataParameter>();
                lclbParameters.Add(DBFunction.GetDBParameter("@PERSON_ID", "int", aintPersonId, iobjPassInfo.iconFramework));

                lclbParameters.Add(DBFunction.GetDBParameter("@CASE_NAME", "string", busConstant.ReturnToWorkRequest.MAP_RETURN_TO_WORK, iobjPassInfo.iconFramework));

                string lstrActiveInProgressInstanceQuery = "entFramework.CountActiveProcessForPersonByCaseName";

                object lobjActiveWfCount = DBFunction.DBExecuteScalar(lstrActiveInProgressInstanceQuery, lclbParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if (lobjActiveWfCount != null)
                {
                    if (Convert.ToInt32(lobjActiveWfCount) > 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
