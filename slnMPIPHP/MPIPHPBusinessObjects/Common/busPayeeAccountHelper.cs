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
    [Serializable]
    public static class busPayeeAccountHelper
    {
        static int ZERO = 0;
        public static Collection<busFedStateTaxRate> iclbFedStateTax { get; set; }
        //public static Collection<busFedStateFlatTaxRate> iclbFedStatFlatTax { get; set; }
        public static Collection<busFedStateDeduction> iclbFedStateTaxDecuction { get; set; }
        /// <summary>
        /// Returns maximum of Payment Date
        /// </summary>
        /// <returns>Last Benefit Payment Date</returns>
        public static DateTime GetLastBenefitPaymentDate()
        {
            busBase lobjBase = new busBase();
            return Convert.ToDateTime(DBFunction.DBExecuteScalar("cdoPaymentSchedule.GetRecentPaymentDate", new object[] { },
                                                lobjBase.iobjPassInfo.iconFramework, lobjBase.iobjPassInfo.itrnFramework));
        }
        public static DateTime GetPaymentSetUpCuttOffDate(int iintPlanId)
        {
            string astrScheduleType = string.Empty;
            if (iintPlanId == busConstant.IAP_PLAN_ID)
                astrScheduleType = busConstant.PaymentScheduleTypeWeekly;
            else
                astrScheduleType = busConstant.PaymentScheduleTypeMonthly;
            busBase lobjBase = new busBase();
            return Convert.ToDateTime(DBFunction.DBExecuteScalar("cdoPaymentSchedule.GetRecentCuttOffDateByScheduleType", new object[1] { astrScheduleType },
                     lobjBase.iobjPassInfo.iconFramework, lobjBase.iobjPassInfo.itrnFramework));
        }
        public static DateTime GetLastBenefitPaymentDate(int iintPlanId)
        {
            string astrScheduleType = string.Empty;

            if (iintPlanId == busConstant.IAP_PLAN_ID)
                astrScheduleType = busConstant.PaymentScheduleTypeWeekly;
            else
                astrScheduleType = busConstant.PaymentScheduleTypeMonthly;
            busBase lobjBase = new busBase();
            return Convert.ToDateTime(DBFunction.DBExecuteScalar("cdoPaymentSchedule.GetRecentPaymentDateByScheduleType", new object[1] { astrScheduleType },

            lobjBase.iobjPassInfo.iconFramework, lobjBase.iobjPassInfo.itrnFramework));
        }
        public static DateTime GetLastBenefitPaymentDate(string astrScheduleType)
        {

            busBase lobjBase = new busBase();
            return Convert.ToDateTime(DBFunction.DBExecuteScalar("cdoPaymentSchedule.GetRecentPaymentDateByScheduleType", new object[1] { astrScheduleType },

            lobjBase.iobjPassInfo.iconFramework, lobjBase.iobjPassInfo.itrnFramework));

        }


        public static int IsBenefitAccountExists(int aintPersonAccountID, string astrBenefitAccountType, string astrFundsType,
                                                int aintBenefitApplicationDetailId = 0, int aintDROApplicationDetailId = 0)
        {
            busBase lobjBase = new busBase();
            int lintBenefitAccountID = 0;
            if (astrFundsType.IsNotNullOrEmpty())
            {
                lintBenefitAccountID = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPayeeBenefitAccount.GetBenefitAccountID",
                                      new object[2] { aintPersonAccountID, astrFundsType },
                                      lobjBase.iobjPassInfo.iconFramework, lobjBase.iobjPassInfo.itrnFramework)); //R3view  the Query && ADD to MPI
            }

            else
            {
                lintBenefitAccountID = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPayeeBenefitAccount.GetBenefitAccountIdWithoutFundType",
                                         new object[3] { aintPersonAccountID, aintBenefitApplicationDetailId, aintDROApplicationDetailId },
                                         lobjBase.iobjPassInfo.iconFramework, lobjBase.iobjPassInfo.itrnFramework)); //R3view  the Query && ADD to MPI
            }

            return lintBenefitAccountID;
        }


        // Returns the PayeeAccountid if Payee Account already exists for the given Person ID.  If none, then returns 0
        public static int IsPayeeAccountExists(int aintPayeeID, int aintPayeeBenefitAccountID, string astrAccountRelationValue,
                                                string astrBenefitAccountTypeValue, bool ablnIsPayeeOrg, int aintPlanID, string astrBenefitOption = null, int aintDROApplicationDetailId = 0,
                                                string astrRetirementType = null,int aintBenefitApplicationDetailId = 0) //PROD PIR 569 //RID 60954
        {
            DataTable ldtbResult = new DataTable();
            int lintPayeeAccountid = 0;

            if (!ablnIsPayeeOrg)
            {
                if (aintDROApplicationDetailId != 0) //PROD PIR 569
                    ldtbResult = busBase.Select<cdoPayeeAccount>(
                                    new string[5] { "PERSON_ID", "ACCOUNT_RELATION_VALUE", "PAYEE_BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", "dro_application_detail_id" },
                                    new object[5] { aintPayeeID, astrAccountRelationValue, aintPayeeBenefitAccountID, astrBenefitAccountTypeValue, aintDROApplicationDetailId }, null, null);
                else if (aintBenefitApplicationDetailId != 0)
                {
                    ldtbResult = busBase.Select<cdoPayeeAccount>(
                                new string[5] { "PERSON_ID", "ACCOUNT_RELATION_VALUE", "PAYEE_BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", "BENEFIT_APPLICATION_DETAIL_ID" },
                                new object[5] { aintPayeeID, astrAccountRelationValue, aintPayeeBenefitAccountID, astrBenefitAccountTypeValue, aintBenefitApplicationDetailId }, null, null);

                }
                else
                {
                    ldtbResult = busBase.Select<cdoPayeeAccount>(
                                new string[4] { "PERSON_ID", "ACCOUNT_RELATION_VALUE", "PAYEE_BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE" },
                                new object[4] { aintPayeeID, astrAccountRelationValue, aintPayeeBenefitAccountID, astrBenefitAccountTypeValue }, null, null);
                }
                   
                    
            }
            else
            {
                ldtbResult = busBase.Select<cdoPayeeAccount>(
                            new string[4] { "ORG_ID", "ACCOUNT_RELATION_VALUE", "PAYEE_BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE" },
                            new object[4] { aintPayeeID, astrAccountRelationValue, aintPayeeBenefitAccountID, astrBenefitAccountTypeValue }, null, null);
            }

            foreach (DataRow dr in ldtbResult.Rows)
            {
                busPayeeAccount lbusTempPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lbusTempPayeeAccount.icdoPayeeAccount.LoadData(dr);

                if (lbusTempPayeeAccount.ibusPayee.IsNull())
                {
                    lbusTempPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
                    lbusTempPayeeAccount.ibusPayee.FindPerson(aintPayeeID);
                }

                if (lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.IsNull())
                    lbusTempPayeeAccount.ibusCurrentActivePayeeAccount = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
                lbusTempPayeeAccount.LoadPayeeAccountStatuss();

                if (!lbusTempPayeeAccount.iclbPayeeAccountStatus.IsNullOrEmpty())
                {
                    lbusTempPayeeAccount.ibusCurrentActivePayeeAccount = lbusTempPayeeAccount.iclbPayeeAccountStatus[0];
                    if (aintPlanID != busConstant.IAP_PLAN_ID)
                    {
                        //10 Percent
                        if (lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.IsStatusCompleted() && astrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION &&
                            (astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT || astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_RETIREMENT)
                            && lbusTempPayeeAccount.icdoPayeeAccount.retiree_incr_flag != busConstant.FLAG_YES)
                        {
                            lintPayeeAccountid = lbusTempPayeeAccount.icdoPayeeAccount.payee_account_id;
                            break;
                        }

                        if (!((lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.IsStatusCancelled()) || (lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.IsStatusCompleted())))
                        {
                            lintPayeeAccountid = lbusTempPayeeAccount.icdoPayeeAccount.payee_account_id;
                            break;
                        }

                        //PIR 853
                        if (lbusTempPayeeAccount.ibusPayee != null && lbusTempPayeeAccount.ibusPayee.icdoPerson.person_id > 0 &&
                            lbusTempPayeeAccount.ibusPayee.icdoPerson.date_of_death != DateTime.MinValue && lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.IsStatusCompleted()
                            && lbusTempPayeeAccount.icdoPayeeAccount.retiree_incr_flag != busConstant.FLAG_YES 
                            && (astrRetirementType.IsNullOrEmpty() || (!astrRetirementType.IsNullOrEmpty() && lbusTempPayeeAccount.icdoPayeeAccount.retirement_type_value == astrRetirementType)))//RID 60954
                        {
                            lintPayeeAccountid = lbusTempPayeeAccount.icdoPayeeAccount.payee_account_id;
                            break;
                        }
                    }
                    else
                    {
                        if (!(lbusTempPayeeAccount.ibusCurrentActivePayeeAccount.IsStatusCancelled()))
                        {
                            lintPayeeAccountid = lbusTempPayeeAccount.icdoPayeeAccount.payee_account_id;
                            break;
                        }
                    }
                }
            }

            return lintPayeeAccountid;
        }


    

        public static void CalculateMonthlyPaymentComponents(DateTime adtCalculationDate, decimal adecMonthlyFinalAmount, ref decimal ldecNonTaxableAmount, ref decimal ldecTaxableAmount,
                                           decimal adecNonTaxablePortion)
        {

            //Initializing the Reference Parameters
            ldecNonTaxableAmount = 0.0M;
            ldecTaxableAmount = 0.0M;

            ldecNonTaxableAmount = Math.Round(adecNonTaxablePortion, 2, MidpointRounding.AwayFromZero);

            if (ldecNonTaxableAmount > 0)
            {
                ldecTaxableAmount = Math.Round(adecMonthlyFinalAmount - ldecNonTaxableAmount, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                ldecTaxableAmount = Math.Round(adecMonthlyFinalAmount, 2, MidpointRounding.AwayFromZero);
            }
        }

        public static DateTime GetBenefitBeginDate(DateTime adtRetirementDate, int aintPlanId, string astrBenefitSubType = "")
        {
            //Prod PIR 229 
            if (aintPlanId == busConstant.IAP_PLAN_ID && astrBenefitSubType != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                return adtRetirementDate.AddMonths(2);
            else
                return adtRetirementDate;
        }

        public static int IsTermCertainBenefitOption(int aintPlanBenefitId, Collection<busPlanBenefitXr> aclbPlanBenefitXr)
        {
            if (!aclbPlanBenefitXr.IsNullOrEmpty() && (!aclbPlanBenefitXr.Where(item => item.icdoPlanBenefitXr.plan_benefit_id == aintPlanBenefitId).IsNullOrEmpty()))
            {
                string lstrBenOpValue = aclbPlanBenefitXr.Where(item => item.icdoPlanBenefitXr.plan_benefit_id == aintPlanBenefitId).First().icdoPlanBenefitXr.benefit_option_value;

                if (lstrBenOpValue == busConstant.TEN_YEARS_TERM_CERTAIN || lstrBenOpValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                {
                    return busConstant.TEN_YEAR_CERTAIN_MONTHS;
                }
                else if (lstrBenOpValue == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                {
                    return busConstant.FIVE_YEAR_CERTAIN_MONTHS;
                }
                else if (lstrBenOpValue == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                {
                    return busConstant.THREE_YEAR_CERTAIN_MONTHS;
                }
                else if (lstrBenOpValue == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY)
                {
                    return busConstant.TWO_YEAR_CERTAIN_MONTHS;
                }
            }
            return 0;
        }


        #region Calculate Fed Or StateTax
        public static decimal CalculateFedOrStateTax(decimal adecTaxableAmount, int aintNoOfAllowances, DateTime adtPaymentDate, string astrMaritalStatus,
          string astrTaxIdentifier, decimal adecAdditionalTax, busPayeeAccountTaxWithholding taxwitholding = null, bool standaloneflag=false, string istrCalScreen = null, bool ablnActiveRetireeBatch = false)
        {
            decimal ldecFinalTaxAmount = 0.00M, ldecResult4 = 0.0M;
            decimal ldecAdjustableTaxableAmount = 0.00M;
           // if ((astrMaritalStatus != null) && (astrTaxIdentifier != null))
           if (astrTaxIdentifier != null)
            {
                if (iclbFedStateTax == null)
                    LoadFedStateTaxRates();

                if (iclbFedStateTaxDecuction == null)
                    LoadFedStateTaxDeductions();
                if(istrCalScreen == "Y")
                {
                    DataTable ldtFEDTaxCaluationFinalAmount = busBase.Select("cdoPayeeAccountTaxWithholding.GetFederalTaxWithHoldingCalculation", new object[8] { adecTaxableAmount, astrMaritalStatus, taxwitholding.idecStep2b3, taxwitholding.idecStep3,  taxwitholding.idecStep4A, taxwitholding.idecStep4B, taxwitholding.idecStep4C, "" });
                    if (ldtFEDTaxCaluationFinalAmount.Rows.Count > 0)
                    {
                        taxwitholding.idecFedFinalTaxAmount = Convert.ToDecimal(ldtFEDTaxCaluationFinalAmount.Rows[0][0]);
                        ldecFinalTaxAmount = Convert.ToDecimal(ldtFEDTaxCaluationFinalAmount.Rows[0][0]);

                    }

                }
                else
                {
                    if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && (astrMaritalStatus == "MQ" || astrMaritalStatus == "SM" || astrMaritalStatus == "HH") && taxwitholding != null && (taxwitholding.icdoPayeeAccountTaxWithholding.end_date != DateTime.MinValue || taxwitholding.icdoPayeeAccountTaxWithholding.end_date != null))
                    {
                        //if (ablnActiveRetireeBatch)
                        //{
                        //    if (adecTaxableAmount >= 750)
                        //    {
                        //        taxwitholding.idecFedFinalTaxAmount = adecTaxableAmount * 0.2M;
                        //        ldecFinalTaxAmount = adecTaxableAmount * 0.2M;
                        //    }
                        //}
                        //else
                        //{
                            DataTable ldtFEDTaxCaluationFinalAmount = busBase.Select("cdoPayeeAccountTaxWithholding.GetFederalTaxWithHoldingCalculation", new object[8] { adecTaxableAmount, astrMaritalStatus, taxwitholding.icdoPayeeAccountTaxWithholding.step_2_b_3, taxwitholding.icdoPayeeAccountTaxWithholding.step_3_amount, taxwitholding.icdoPayeeAccountTaxWithholding.step_4_a, taxwitholding.icdoPayeeAccountTaxWithholding.step_4_b, taxwitholding.icdoPayeeAccountTaxWithholding.step_4_c, taxwitholding.icdoPayeeAccountTaxWithholding.tax_option_value });
                            if (ldtFEDTaxCaluationFinalAmount.Rows.Count > 0)
                            {
                                taxwitholding.idecFedFinalTaxAmount = Convert.ToDecimal(ldtFEDTaxCaluationFinalAmount.Rows[0][0]);
                                ldecFinalTaxAmount = Convert.ToDecimal(ldtFEDTaxCaluationFinalAmount.Rows[0][0]);

                            }
                        //}
                    }

                }


                if (astrTaxIdentifier == busConstant.GA_STATE_TAX)
                {
                    DataTable dtStateTax = busBase.Select("cdoPayeeAccountTaxWithholding.GetStateTaxWithHoldingCalculationForGAST", new object[5] { astrMaritalStatus, adecTaxableAmount, 0, adecAdditionalTax, aintNoOfAllowances });
                    if (dtStateTax.Rows.Count > 0)
                    {
                        ldecFinalTaxAmount = Convert.ToDecimal(dtStateTax.Rows[0]["TAX_WITHHOLDING_AMOUNT"]);
                        taxwitholding.idecStatePFinalTaxAmount = ldecFinalTaxAmount;
                    }
                }
                else if (astrTaxIdentifier == busConstant.OR_STATE_TAX)
                {
                    if (taxwitholding.icdoPayeeAccountTaxWithholding.payee_account_id >0)
                    {
                        DataTable dtFedTax = busBase.Select("cdoPayeeAccountTaxWithholding.GetFedTaxWithHoldingAmount", new object[1] { taxwitholding.icdoPayeeAccountTaxWithholding.payee_account_id });
                        if (dtFedTax.Rows.Count > 0)
                            taxwitholding.idecFedFinalTaxAmount = Convert.ToDecimal(dtFedTax.Rows[0]["AMOUNT"]);

                    }
                       
                    DataTable dtStateTax = busBase.Select("cdoPayeeAccountTaxWithholding.GetStateTaxWithHoldingCalculationForORST", new object[6] { astrMaritalStatus, adecTaxableAmount, taxwitholding.idecFedFinalTaxAmount, 0, adecAdditionalTax, aintNoOfAllowances });
                    if (dtStateTax.Rows.Count > 0)
                    {
                        ldecFinalTaxAmount = Convert.ToDecimal(dtStateTax.Rows[0]["TAX_WITHHOLDING_AMOUNT"]);
                        taxwitholding.idecStatePFinalTaxAmount = ldecFinalTaxAmount;
                    }
                }
                else if (astrTaxIdentifier == busConstant.NC_STATE_TAX)
                {
                    DataTable dtStateTax = busBase.Select("cdoPayeeAccountTaxWithholding.GetStateTaxWithHoldingCalculationForNCST", new object[5] { astrMaritalStatus, adecTaxableAmount, 0, adecAdditionalTax, aintNoOfAllowances });
                    if (dtStateTax.Rows.Count > 0)
                    {
                        ldecFinalTaxAmount = Convert.ToDecimal(dtStateTax.Rows[0]["TAX_WITHHOLDING_AMOUNT"]);
                        taxwitholding.idecStatePFinalTaxAmount = ldecFinalTaxAmount;
                    }
                }
                else if (astrTaxIdentifier == busConstant.VA_STATE_TAX)
                {                    
                    DataTable dtStateTax = busBase.Select("cdoPayeeAccountTaxWithholding.GetStateTaxWithHoldingCalculationForVAST", new object[5] { adecTaxableAmount, 0, adecAdditionalTax, taxwitholding.icdoPayeeAccountTaxWithholding.personal_exemptions, taxwitholding.icdoPayeeAccountTaxWithholding.age_and_blindness_exemptions });
                    if (dtStateTax.Rows.Count > 0)
                    {
                        ldecFinalTaxAmount = Convert.ToDecimal(dtStateTax.Rows[0]["TAX_WITHHOLDING_AMOUNT"]);
                        taxwitholding.idecStatePFinalTaxAmount = ldecFinalTaxAmount;
                    }
                }
                else if (iclbFedStateTax.Where(o => o.icdoFedStateTaxRate.marital_status_value == astrMaritalStatus
                                      && o.icdoFedStateTaxRate.tax_identifier_value == astrTaxIdentifier
                                      && o.icdoFedStateTaxRate.effective_date <= adtPaymentDate).Any()
                    && iclbFedStateTaxDecuction.Where(o => o.icdoFedStateDeduction.tax_identifier_value == astrTaxIdentifier
                                      && o.icdoFedStateDeduction.effective_date <= adtPaymentDate).Any())
                {
                    var lclbFedStatTax = from lbusFedStateTaxRate in iclbFedStateTax
                                         where lbusFedStateTaxRate.icdoFedStateTaxRate.marital_status_value == astrMaritalStatus
                                         && lbusFedStateTaxRate.icdoFedStateTaxRate.tax_identifier_value == astrTaxIdentifier
                                         && (lbusFedStateTaxRate.icdoFedStateTaxRate.effective_date ==
                                         (from lobjFedStateTaxRate in iclbFedStateTax
                                          where lobjFedStateTaxRate.icdoFedStateTaxRate.marital_status_value == astrMaritalStatus
                                          && lobjFedStateTaxRate.icdoFedStateTaxRate.tax_identifier_value == astrTaxIdentifier
                                          && lobjFedStateTaxRate.icdoFedStateTaxRate.effective_date <= adtPaymentDate
                                          select lobjFedStateTaxRate.icdoFedStateTaxRate.effective_date).Max())
                                         select lbusFedStateTaxRate;

                    var lclbFedStatDedcutionTax = from lbusFedStateDeductionRate in iclbFedStateTaxDecuction
                                                  where lbusFedStateDeductionRate.icdoFedStateDeduction.tax_identifier_value == astrTaxIdentifier
                                         && (lbusFedStateDeductionRate.icdoFedStateDeduction.effective_date ==
                                         (from lobjFedStateDeductionRate in iclbFedStateTaxDecuction
                                          where lobjFedStateDeductionRate.icdoFedStateDeduction.tax_identifier_value == astrTaxIdentifier
                                          && lobjFedStateDeductionRate.icdoFedStateDeduction.effective_date <= adtPaymentDate
                                          select lobjFedStateDeductionRate.icdoFedStateDeduction.effective_date).Max())
                                                  select lbusFedStateDeductionRate;

                    if (astrTaxIdentifier == busConstant.CA_STATE_TAX)
                    {
                        lclbFedStatDedcutionTax = lclbFedStatDedcutionTax.Where(item => item.icdoFedStateDeduction.marital_status_value == astrMaritalStatus);
                    }

                    if (lclbFedStatTax.Count() > 0 && astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrMaritalStatus != "MQ" && astrMaritalStatus != "SM" && astrMaritalStatus != "HH")
                    {
                        ////PIR 547 - change Rohan
                        if (lclbFedStatDedcutionTax.IsNotNull() && lclbFedStatDedcutionTax.Where(item => item.icdoFedStateDeduction.allowance == aintNoOfAllowances).Count() > 0)
                            lclbFedStatDedcutionTax = lclbFedStatDedcutionTax.Where(item => item.icdoFedStateDeduction.allowance == aintNoOfAllowances);
                        //PROD PIR 842
                        else if (aintNoOfAllowances > 2 && lclbFedStatDedcutionTax.IsNotNull() && lclbFedStatDedcutionTax.Where(item => item.icdoFedStateDeduction.allowance == 2).Count() > 0)
                            lclbFedStatDedcutionTax = lclbFedStatDedcutionTax.Where(item => item.icdoFedStateDeduction.allowance == 2);

                        ldecAdjustableTaxableAmount = adecTaxableAmount - (aintNoOfAllowances * (lclbFedStatDedcutionTax.FirstOrDefault() != null ? lclbFedStatDedcutionTax.FirstOrDefault().icdoFedStateDeduction.allowance_amount : 0));


                        busFedStateTaxRate lobjFedStateTax = new busFedStateTaxRate();
                        /*Get exact rate for given taxable amount */
                        foreach (busFedStateTaxRate lobjFedStateTaxcol in lclbFedStatTax)
                        {
                            if ((ldecAdjustableTaxableAmount >= lobjFedStateTaxcol.icdoFedStateTaxRate.minimum_amount) &&
                                (ldecAdjustableTaxableAmount < lobjFedStateTaxcol.icdoFedStateTaxRate.maximum_amount))
                            {
                                lobjFedStateTax = lobjFedStateTaxcol;
                                break;
                            }
                        }
                        if (lobjFedStateTax.icdoFedStateTaxRate != null)
                        {
                            decimal ldecNewAdjustableTaxableAmount = ldecAdjustableTaxableAmount - (lobjFedStateTax.icdoFedStateTaxRate.minimum_amount > 0 ?
                                (lobjFedStateTax.icdoFedStateTaxRate.minimum_amount - Convert.ToDecimal(0.01)) : lobjFedStateTax.icdoFedStateTaxRate.minimum_amount);//change Rohan PIR 547,//PROD PIR 842;
                            decimal ldecFerdralTaxAmount = ldecNewAdjustableTaxableAmount * lobjFedStateTax.icdoFedStateTaxRate.percentage;
                            ldecResult4 = ldecFerdralTaxAmount + lobjFedStateTax.icdoFedStateTaxRate.tax_amount;
                            ldecFinalTaxAmount = ldecResult4 + adecAdditionalTax;

                            if (standaloneflag == true)
                            {
                                if (taxwitholding != null)
                                {
                                    taxwitholding.idecTaxAmount = lobjFedStateTax.icdoFedStateTaxRate.tax_amount;
                                    taxwitholding.idecFedExemptionAmount = (aintNoOfAllowances * (lclbFedStatDedcutionTax.FirstOrDefault() != null ? lclbFedStatDedcutionTax.FirstOrDefault().icdoFedStateDeduction.allowance_amount : 0));
                                    taxwitholding.idecAmtWithHolding = ldecAdjustableTaxableAmount;
                                    taxwitholding.idecFerdralTaxAmount = ldecFerdralTaxAmount;
                                    taxwitholding.idecFedPercentage = lobjFedStateTax.icdoFedStateTaxRate.percentage;
                                    taxwitholding.idecFedFinalTaxAmount = ldecFinalTaxAmount;
                                    taxwitholding.idecFedNewAdjustableTaxableAmount = ldecNewAdjustableTaxableAmount;
                                    if (aintNoOfAllowances == 0)
                                    {
                                        taxwitholding.iintFedAllowanceNumber = 0;

                                    }


                                }

                            }


                        }
                    }

                    if (lclbFedStatTax.Count() > 0 && astrTaxIdentifier == busConstant.CA_STATE_TAX)
                    {
                        ////PIR 547 - change Rohan
                        if (lclbFedStatDedcutionTax.IsNotNull() && lclbFedStatDedcutionTax.Where(item => item.icdoFedStateDeduction.allowance == aintNoOfAllowances).Count() > 0)
                            lclbFedStatDedcutionTax = lclbFedStatDedcutionTax.Where(item => item.icdoFedStateDeduction.allowance == aintNoOfAllowances);
                        //PROD PIR 842
                        else if (aintNoOfAllowances > 2 && lclbFedStatDedcutionTax.IsNotNull() && lclbFedStatDedcutionTax.Where(item => item.icdoFedStateDeduction.allowance == 2).Count() > 0)
                            lclbFedStatDedcutionTax = lclbFedStatDedcutionTax.Where(item => item.icdoFedStateDeduction.allowance == 2);

                        if (adecTaxableAmount > lclbFedStatDedcutionTax.FirstOrDefault().icdoFedStateDeduction.low_income_exemption)
                        {
                            ldecAdjustableTaxableAmount = adecTaxableAmount - lclbFedStatDedcutionTax.FirstOrDefault().icdoFedStateDeduction.standard_deduction;

                            busFedStateTaxRate lobjFedStateTax = new busFedStateTaxRate();
                            /*Get exact rate for given taxable amount */
                            foreach (busFedStateTaxRate lobjFedStateTaxcol in lclbFedStatTax)
                            {
                                if ((ldecAdjustableTaxableAmount >= lobjFedStateTaxcol.icdoFedStateTaxRate.minimum_amount) &&
                                    (ldecAdjustableTaxableAmount < lobjFedStateTaxcol.icdoFedStateTaxRate.maximum_amount))
                                {
                                    lobjFedStateTax = lobjFedStateTaxcol;
                                    break;
                                }
                            }
                            if (lobjFedStateTax.icdoFedStateTaxRate != null)
                            {
                                decimal ldecNewAdjustableTaxableAmount = ldecAdjustableTaxableAmount - (lobjFedStateTax.icdoFedStateTaxRate.minimum_amount > 0 ?
                                    (lobjFedStateTax.icdoFedStateTaxRate.minimum_amount - Convert.ToDecimal(0.01)) : lobjFedStateTax.icdoFedStateTaxRate.minimum_amount);//change Rohan PIR 547//PROD PIR 842;
                                decimal ldecStateTaxAmount = ldecNewAdjustableTaxableAmount * lobjFedStateTax.icdoFedStateTaxRate.percentage;
                                ldecResult4 = ldecStateTaxAmount + lobjFedStateTax.icdoFedStateTaxRate.tax_amount;
                                ldecFinalTaxAmount = Math.Round(ldecResult4, 2, MidpointRounding.AwayFromZero) + adecAdditionalTax - (aintNoOfAllowances * (lclbFedStatDedcutionTax.FirstOrDefault() != null ? lclbFedStatDedcutionTax.FirstOrDefault().icdoFedStateDeduction.allowance_amount : 0));

                                if (standaloneflag == true)
                                {
                                    if (taxwitholding != null)
                                    {
                                        taxwitholding.idecStateTaxAmount = lclbFedStatDedcutionTax.FirstOrDefault().icdoFedStateDeduction.standard_deduction;
                                        taxwitholding.idecStateExemptionAmount = (aintNoOfAllowances * (lclbFedStatDedcutionTax.FirstOrDefault() != null ? lclbFedStatDedcutionTax.FirstOrDefault().icdoFedStateDeduction.allowance_amount : 0));
                                        taxwitholding.idecStateAmtWithHolding = ldecAdjustableTaxableAmount;
                                        taxwitholding.idecStatePTaxAmount = ldecStateTaxAmount;
                                        taxwitholding.idecStatePercentage = lobjFedStateTax.icdoFedStateTaxRate.percentage;
                                        taxwitholding.idecStatePFinalTaxAmount = ldecFinalTaxAmount;
                                        taxwitholding.idecFedTaxRateAmount=lobjFedStateTax.icdoFedStateTaxRate.tax_amount;
                                        taxwitholding.idecStatePNewAdjustableTaxableAmount = ldecNewAdjustableTaxableAmount;
                                        if (aintNoOfAllowances == 0)
                                        {
                                            taxwitholding.iintFedAllowanceNumber = 0;

                                        }
                                    }

                                }

                            }
                        }
                    }

                }
            }
            //Added due to minus tax is getting calculated due to irs table
            if (ldecFinalTaxAmount > 0)
                return Math.Round(ldecFinalTaxAmount, 2, MidpointRounding.AwayFromZero);
            else
                return Math.Round(adecAdditionalTax, 2, MidpointRounding.AwayFromZero);
        }
        #endregion

        #region Load Fed State Tax Rate & Deduction
        public static void LoadFedStateTaxRates()
        {
            busBase lobjBase = new busBase();
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;

            DataTable tax = lobjPassInfo.isrvDBCache.GetCacheData("sgt_fed_state_tax_rate", null);

            iclbFedStateTax = lobjBase.GetCollection<busFedStateTaxRate>(tax, "icdoFedStateTaxRate");
        }

        public static void LoadFedStateTaxDeductions()
        {
            busBase lobjBase = new busBase();
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
            DataTable tax = lobjPassInfo.isrvDBCache.GetCacheData("SGT_FED_STATE_DEDUCTION", null);
            iclbFedStateTaxDecuction = lobjBase.GetCollection<busFedStateDeduction>(tax, "icdoFedStateDeduction");
        }
        #endregion
    }
}
