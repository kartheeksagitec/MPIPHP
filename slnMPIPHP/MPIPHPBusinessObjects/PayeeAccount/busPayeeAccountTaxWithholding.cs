#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.DataObjects;
using System.Linq;
#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPayeeAccountTaxWithholding:
    /// Inherited from busPayeeAccountTaxWithholdingGen, the class is used to customize the business object busPayeeAccountTaxWithholdingGen.
    /// </summary>
    [Serializable]
    public class busPayeeAccountTaxWithholding : busPayeeAccountTaxWithholdingGen
    {

        public decimal idecMonthlyGrossAmount { get; set; }
        public decimal idecExclusionAmount { get; set; }

        public decimal idecFedAdditionalTaxAmount { get; set; }

        public decimal idecStateAdditionalTaxAmount { get; set; }

        public int iintFedAllowanceNumber { get; set; }

        public int iintStateAllowanceNumber { get; set; }

        public string istrFedStatus { get; set; }

        public string istrMtlStatus { get; set; }

        public string istrStateStatus { get; set; }

        public string istrTaxYear { get; set; }

        public decimal idecTaxAmount { get; set; }

        public decimal idecFedTaxRateAmount { get; set; }

        public bool iblnIsCalculateButtonClicked = false;
        public busPayeeAccount ibusPayeeAccount { get; set; }
        public bool iblnIsUpdateMode = false;

        public decimal idecAmtWithHolding { get; set; }
        public decimal idecFedExemptionAmount { get; set; }
        public decimal idecFedPercentage { get; set; }

        public decimal idecFerdralTaxAmount { get; set; }

        public decimal idecFedFinalTaxAmount { get; set; }

        public decimal idecFedNewAdjustableTaxableAmount { get; set; }

        public decimal idecStateTaxAmount { get; set; }
        public decimal idecStateAmtWithHolding { get; set; }
        public decimal idecStateExemptionAmount { get; set; }

        public decimal idecStatePercentage { get; set; }

        public decimal idecStatePTaxAmount { get; set; }

        public decimal idecStatePFinalTaxAmount { get; set; }

        public decimal idecStatePNewAdjustableTaxableAmount { get; set; }

        public decimal idecStep2b3 { get; set; }

        public decimal idecStep3 { get; set; }

        public decimal idecStep4A { get; set; }

        public decimal idecStep4B { get; set; }

        public decimal idecStep4C { get; set; }

        public string istrMaritalStatus { get; set; }

        public string code_value { get; set; }

        public string description { get; set; }

        public int iintage_and_blindness_exemptions { get; set; }

        public int iintpersonal_exemptions { get; set; }



        public busPayeeAccountTaxWithholding()
        {

        }

        public bool IsStartDateNull()
        {
            if (this.icdoPayeeAccountTaxWithholding.start_date == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        public bool IsENDDateNull()
        {
            if (this.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        #region Update Mode End Date
        public bool IsUpdateModeEndDateNotNull()
        {
            if ((iblnIsUpdateMode) &&
                    (icdoPayeeAccountTaxWithholding.end_date != DateTime.MinValue))
            {
                return true;
            }
            return false;
        }
        #endregion

        public override void BeforePersistChanges()
        {

            if (this.icdoPayeeAccountTaxWithholding.end_date != null)
            {
                //Safe check value cannot be null
                if (icdoPayeeAccountTaxWithholding.ihstOldValues["end_date"] == null)
                {

                    this.icdoPayeeAccountTaxWithholding.marital_status_value = Convert.ToString(icdoPayeeAccountTaxWithholding.ihstOldValues["marital_status_value"]);
                    //if (this.icdoPayeeAccount.transfer_org_id != ainttransferorgid)
                    //{
                    //    CreateReviewPayeeAccountStatus();
                    //    aboolTransChange = true;
                    //}
                }
            }


        }

        #region Check Error On Add Button
        public ArrayList CheckErrorOnAddButton(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false)
        {
            utlError lobjError = null;
            ahstParams["icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id"] = ahstParams["payee_account_tax_withholding_id"];
            ahstParams["icdoPayeeAccountTaxWithholding.tax_percentage"] = ahstParams["tax_percentage"];
            ahstParams["icdoPayeeAccountTaxWithholding.end_date"] = ahstParams["end_date"];
            ahstParams["icdoPayeeAccountTaxWithholding.start_date"] = ahstParams["start_date"];
            ahstParams["icdoPayeeAccountTaxWithholding.tax_allowance"] = ahstParams["tax_allowance"];
            ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"] = ahstParams["additional_tax_amount"];
            ahstParams["icdoPayeeAccountTaxWithholding.benefit_distribution_type_value"] = ahstParams["benefit_distribution_type_value"];

            string astrMaritalStatus =Convert.ToString(ahstParams["marital_status_value"]);
            string astrTaxOption = Convert.ToString(ahstParams["tax_option_value"]);
            string astrTaxIdentifier = Convert.ToString(ahstParams["tax_identifier_value"]);
            string astrBenefitDistributionType = Convert.ToString(ahstParams["benefit_distribution_type_value"]);
            decimal ldectaxPercentage = 0.0m;
            int ainttaxwithholdingid = 0;
            decimal ldecAdditionalTaxAmount = 0.0m;

            busPayeeAccount lbusPayeeAccount = aobj as busPayeeAccount;
            //Ticket#73404
            lbusPayeeAccount.icdoPayeeAccount.istrTaxIdentifier = astrTaxIdentifier;
            lbusPayeeAccount.icdoPayeeAccount.istrBenefitDistributionType = astrBenefitDistributionType;
            lbusPayeeAccount.icdoPayeeAccount.istrTaxOption = astrTaxOption;
            lbusPayeeAccount.icdoPayeeAccount.istrSavingMode = "ADD";

            if (ahstParams.Count > 0)
            {
                if (Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id"]).IsNullOrEmpty())
                    ainttaxwithholdingid = 0;
                else
                    ainttaxwithholdingid = Convert.ToInt32(ahstParams["icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id"]);

                if (Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.tax_percentage"]).IsNullOrEmpty()
                    || Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.tax_percentage"]) == "")
                    ldectaxPercentage = Decimal.Zero;
                else
                    ldectaxPercentage = Convert.ToDecimal(ahstParams["icdoPayeeAccountTaxWithholding.tax_percentage"]);

                if (astrTaxIdentifier.IsNullOrEmpty())
                {
                    lobjError = AddError(6042, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION)
                {
                    //EmergencyOneTimePayment - 03/17/2020
                    busBenefitApplication lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lbusBenefitApplication.FindBenefitApplication(lbusPayeeAccount.icdoPayeeAccount.iintBenefitApplicationID);

                    if (lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER
                        || lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE)
                    {
                        //PIR 786-TusharT (Revised Rule of PIR-176)
                        if (lbusPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.BENEFICIARY_RELATIONSHIP_EXSPOUSE)
                        {
                            if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage != 10 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                            {
                                lobjError = AddError(6044, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                            else if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage < 10 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                            {
                                lobjError = AddError(6044, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                        }
                        else
                        {
                            //PROD PIR 193
                            if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage != 20 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                            {
                                lobjError = AddError(6043, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                            else if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                            {
                                if ((lbusBenefitApplication.icdoBenefitApplication.emergency_onetime_payment_flag.IsNullOrEmpty() && lbusBenefitApplication.icdoBenefitApplication.withdrawal_type_value.IsNullOrEmpty() ) && ldectaxPercentage < 20)
                                {
                                    lobjError = AddError(6043, "");
                                    aarrErrors.Add(lobjError);
                                    return aarrErrors;
                                }
                                else if((lbusBenefitApplication.icdoBenefitApplication.emergency_onetime_payment_flag == "Y" || lbusBenefitApplication.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty()) && ldectaxPercentage != 10 && ldectaxPercentage != 20)
                                {
                                    lobjError = AddError(6297, "");
                                    aarrErrors.Add(lobjError);
                                    return aarrErrors;
                                }
                            }
                            else if((lbusBenefitApplication.icdoBenefitApplication.emergency_onetime_payment_flag == "Y" || lbusBenefitApplication.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty()) && astrTaxIdentifier == busConstant.CA_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage != 1 && ldectaxPercentage != 2)
                            {
                                lobjError = AddError(6297, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }

                        }
                    }

                    else if ((lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY
                         || lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_JOINT_ANNUITANT)
                         && lbusPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE)
                    {
                        //PROD PIR 193
                        if (lbusPayeeAccount.iblnIsQualifiedSpouse && astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage != 20 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                        {

                            lobjError = AddError(6043, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }
                        else if (lbusPayeeAccount.iblnIsQualifiedSpouse && astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage < 20 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                        {

                            lobjError = AddError(6043, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }
                    }

                    else
                    {
                        if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT)
                        {

                            if (lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY
                                && lbusPayeeAccount.icdoPayeeAccount.family_relation_value != busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE)
                            {

                            }
                            else if (ldectaxPercentage != 10)
                            {

                                lobjError = AddError(6044, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                        }
                    }

                }
                // PROD PIR 176
                if (astrBenefitDistributionType == busConstant.Benefit_Distribution_Type_LumpSum && astrTaxIdentifier == busConstant.CA_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT)
                {
                    //PIR 876
                    //if (ldectaxPercentage != 2 && (lbusPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE
                    //    || lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.PERSON_TYPE_PARTICIPANT))
                    //{
                    //    lobjError = AddError(6045, "");
                    //    aarrErrors.Add(lobjError);
                    //    return aarrErrors;
                    //}
                    //else if (ldectaxPercentage != 2 && lbusPayeeAccount.icdoPayeeAccount.family_relation_value != busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE &&
                    //    lbusPayeeAccount.icdoPayeeAccount.family_relation_value != busConstant.BENEFICIARY_RELATIONSHIP_EXSPOUSE
                    //    && lbusPayeeAccount.icdoPayeeAccount.account_relation_value != busConstant.PERSON_TYPE_PARTICIPANT) // PROD PIR 176
                    //{
                    //    lobjError = AddError(6045, "");
                    //    aarrErrors.Add(lobjError);
                    //    return aarrErrors;
                    //}
                    //PIR 786-TusharT (Revised Rule of PIR-176)

                    //PIR 945
                    //if (ldectaxPercentage != 1 && lbusPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.BENEFICIARY_RELATIONSHIP_EXSPOUSE)
                    //{
                    //    lobjError = AddError(6046, "");
                    //    aarrErrors.Add(lobjError);
                    //    return aarrErrors;
                    //}
                    //PIR 876
                    if (ldectaxPercentage <= 0)
                    {

                        lobjError = AddError(6029, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }

                if(lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Where(i=>i.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.FEDRAL_STATE_TAX && (i.icdoPayeeAccountTaxWithholding.end_date == null || i.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue) && i.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NO_FEDRAL_TAX).Count() > 0)
                {
                    if(astrTaxIdentifier == busConstant.VA_STATE_TAX && astrTaxOption != busConstant.FLAT_DOLLAR && astrTaxOption != busConstant.NO_STATE_TAX)
                    {
                        lobjError = AddError(6318, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;

                    }
                    
                }

                if (lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Count > 0 && lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION)
                {
                    //PIR 876
                    if (astrTaxIdentifier == busConstant.CA_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage <= 0)
                    {

                        //PIR 876
                        lobjError = AddError(6029, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;

                        //if (lbusPayeeAccount.idtNextBenefitPaymentDate == null)
                        //    lbusPayeeAccount.LoadNextBenefitPaymentDate();

                        //int iintCheckFlatPercentage = 0;

                        //iintCheckFlatPercentage = (from item in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                        //                           where busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                        //                           item.icdoPayeeAccountTaxWithholding.start_date, item.icdoPayeeAccountTaxWithholding.end_date)
                        //                           && item.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.FEDRAL_STATE_TAX
                        //                           && item.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_PERCENT
                        //                           && item.icdoPayeeAccountTaxWithholding.tax_percentage >= 20
                        //                           select item).Count();
                        //// PROD PIR 193
                        //if (iintCheckFlatPercentage > 0 && ldectaxPercentage != 2 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                        //{
                        //    lobjError = AddError(6045, "");
                        //    aarrErrors.Add(lobjError);
                        //    return aarrErrors;
                        //}

                        //iintCheckFlatPercentage = (from item in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                        //                           where busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                        //                           item.icdoPayeeAccountTaxWithholding.start_date, item.icdoPayeeAccountTaxWithholding.end_date)
                        //                           && item.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.FEDRAL_STATE_TAX
                        //                           && item.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_PERCENT
                        //                           && item.icdoPayeeAccountTaxWithholding.tax_percentage < 20
                        //                           select item).Count();

                        //if (iintCheckFlatPercentage > 0 && ldectaxPercentage != 1)
                        //{
                        //    lobjError = AddError(6046, "");
                        //    aarrErrors.Add(lobjError);
                        //    return aarrErrors;
                        //}
                    }
                }
                if(astrTaxIdentifier != busConstant.FEDRAL_STATE_TAX)
                {
                    if (lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Where(i => i.icdoPayeeAccountTaxWithholding.tax_identifier_value != "FDRL" && i.icdoPayeeAccountTaxWithholding.tax_identifier_value != astrTaxIdentifier && (i.icdoPayeeAccountTaxWithholding.end_date == null || i.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue)).Count() > 0)
                    {
                        lobjError = AddError(6317, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;

                    }

                }
               

                if(astrTaxIdentifier == busConstant.OR_STATE_TAX)
                {
                    if (lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Where(i => i.icdoPayeeAccountTaxWithholding.tax_identifier_value == "FDRL" && i.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue).Count() > 0)
                    {

                    }
                    else
                    {

                        lobjError = AddError(6319, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;

                    }

                }
                

                if (astrTaxIdentifier.IsNotNullOrEmpty() && astrBenefitDistributionType.IsNullOrEmpty())
                {

                    lobjError = AddError(6047, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                //PIR 803
                if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION && astrBenefitDistributionType != busConstant.Benefit_Distribution_Type_LumpSum
                    && lbusPayeeAccount.icdoPayeeAccount.retiree_incr_flag != busConstant.FLAG_YES)
                {

                    lobjError = AddError(6048, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption != busConstant.LUMP_SUM_DESCRIPTION && astrBenefitDistributionType != busConstant.Benefit_Distribution_Type_Monthly_Benefit)
                {

                    lobjError = AddError(6063, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (astrTaxIdentifier.IsNotNullOrEmpty() && astrTaxOption.IsNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNullOrEmpty())
                {



                    lobjError = AddError(6049, "");
                    aarrErrors.Add(lobjError);
                }

                //if (astrTaxIdentifier.IsNotNullOrEmpty() && astrTaxIdentifier == busConstant.CA_STATE_TAX )
                //{

                //    lobjError = AddError(6309, "");
                //    aarrErrors.Add(lobjError);
                //}
                //if (!lbusPayeeAccount.iclbPayeeAccountTaxWithholding.IsNullOrEmpty())
                //{
                //    int CountRecordwithMaritalStatus = (from obj in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                //                                        where obj.icdoPayeeAccountTaxWithholding.end_date == null || (obj.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue)
                //                                        && obj.icdoPayeeAccountTaxWithholding.tax_identifier_value == astrTaxIdentifier && obj.icdoPayeeAccountTaxWithholding.marital_status_value == astrMaritalStatus
                //                                        && obj.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id != ainttaxwithholdingid
                //                                        select obj).Count();
                //    if (CountRecordwithMaritalStatus == 0)
                //    {
                //        lobjError = AddError(6309, "");
                //        aarrErrors.Add(lobjError);
                //    }
                //}

                if (!lbusPayeeAccount.iclbPayeeAccountTaxWithholding.IsNullOrEmpty())
                {
                    int CountRecordwithEndDateNull = (from obj in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                                                      where (obj.icdoPayeeAccountTaxWithholding.end_date == null || (obj.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue))
                                                      && obj.icdoPayeeAccountTaxWithholding.tax_identifier_value == astrTaxIdentifier
                                                      //PROD PIR 804   //&& obj.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id == ainttaxwithholdingid
                                                      select obj).Count();
                    if (CountRecordwithEndDateNull > 0)
                    {
                        lobjError = AddError(6050, "");
                        aarrErrors.Add(lobjError);
                    }
                    if ((from obj in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                         where obj.icdoPayeeAccountTaxWithholding.tax_identifier_value == astrTaxIdentifier
                         select obj).Count() > 0)
                    {
                        var MaxEndDate = (from obj in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                                          where obj.icdoPayeeAccountTaxWithholding.tax_identifier_value == astrTaxIdentifier
                                          select obj.icdoPayeeAccountTaxWithholding.end_date).Max();
                        //if (Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNotNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]).IsNotNullOrEmpty()
                        //    && MaxEndDate > Convert.ToDateTime(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]))
                        //PROD PIR 804
                        if (Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]).IsNotNullOrEmpty()
                                && MaxEndDate >= Convert.ToDateTime(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]))
                        {
                            lobjError = AddError(6051, "");
                            aarrErrors.Add(lobjError);
                        }
                    }
                }
                if (astrTaxIdentifier != busConstant.VA_STATE_TAX)
                {
                    if (astrMaritalStatus == busConstant.MARITAL_STATUS_MARRIED && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]).IsNullOrEmpty())
                    {
                        lobjError = AddError(5482, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }

                }

              
                    if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX || astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE) &&
                       astrMaritalStatus.IsNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNullOrEmpty())
                    {
                        lobjError = AddError(5481, "");
                        aarrErrors.Add(lobjError);
                    }

               
                   
               

                if (astrTaxIdentifier != busConstant.VA_STATE_TAX && astrTaxIdentifier != busConstant.FEDRAL_STATE_TAX)
                {
                   if(astrTaxOption != busConstant.NO_STATE_TAX && astrTaxOption != busConstant.FLAT_DOLLAR && astrTaxOption != busConstant.FLAT_PERCENT)
                    {
                        if (astrMaritalStatus.IsNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNullOrEmpty())
                        {
                            lobjError = AddError(5481, "");
                            aarrErrors.Add(lobjError);

                        }

                    }
                    

                }

                    //Ticket#98349
                    if (astrTaxIdentifier != busConstant.VA_STATE_TAX)
                {
                    if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX || astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE || astrTaxOption == busConstant.StateTaxOptionFedTaxBasedOnIRS || astrTaxOption == busConstant.StateTaxOptionFedTaxBasedOnIRSAndAdditional) &&
                    Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]).IsNullOrEmpty())
                    {
                        lobjError = AddError(5482, "");
                        aarrErrors.Add(lobjError);
                    }
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]) == "0")
                    ldecAdditionalTaxAmount = 0;
                else
                    ldecAdditionalTaxAmount = Convert.ToDecimal(ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]);
                if (astrTaxIdentifier != busConstant.VA_STATE_TAX)
                {
                    if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX || astrTaxOption == busConstant.StateTaxOptionFedTaxBasedOnIRSAndAdditional
                    || astrTaxOption == busConstant.FLAT_DOLLAR)
                    && (Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]) == "0"))
                    {
                        lobjError = AddError(6060, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }

                if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE || astrTaxOption == busConstant.StateTaxOptionFedTaxBasedOnIRS || astrTaxOption == busConstant.FLAT_PERCENT)
                    && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]).IsNotNullOrEmpty() && ldecAdditionalTaxAmount != 0)
                {
                    lobjError = AddError(6076, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                //if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE || astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX || astrTaxOption == busConstant.StateTaxOptionFedTaxBasedOnIRS || astrTaxOption == busConstant.StateTaxOptionFedTaxBasedOnIRSAndAdditional)
                //   && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]).IsNullOrEmpty())
                //{
                //    lobjError = AddError(5482, "");
                //    aarrErrors.Add(lobjError);
                //    return aarrErrors;
                //}

                if (lbusPayeeAccount.icdoPayeeAccount.person_id > 0)
                {
                    bool lblStateFlag = false;
                    lbusPayeeAccount.ibusPayee.ibusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                    lbusPayeeAccount.ibusPayee.LoadPersonAddresss();
                    string lstrTaxIdentifier = string.Empty;
                    if (astrTaxIdentifier != busConstant.FEDRAL_STATE_TAX)
                    {
                        if (astrTaxIdentifier == busConstant.CA_STATE_TAX)// && lbusPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value != lstrTaxIdentifier.Substring(0, 2).ToString()).Count() > 0)
                        {
                            lstrTaxIdentifier = "CA";
                        }
                        else
                        {
                            lstrTaxIdentifier = astrTaxIdentifier;
                        }

                        if (lstrTaxIdentifier == "CA" && (lbusPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "VA").Count() > 0
                                                    || lbusPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "NC").Count() > 0 ||
                                                    lbusPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "OR").Count() > 0 ||
                                                    lbusPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "GA").Count() > 0) && astrBenefitDistributionType != busConstant.Benefit_Distribution_Type_LumpSum)
                        {
                            lblStateFlag = true;
                        }
                       else if(lstrTaxIdentifier == "CA" && (lbusPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "VA").Count() > 0
                            || lbusPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "NC").Count() > 0||
                            lbusPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "OR").Count() > 0 ||
                            lbusPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "GA").Count() > 0 && astrBenefitDistributionType != busConstant.Benefit_Distribution_Type_LumpSum))
                        {
                            lblStateFlag = false;

                        }

                        if (lblStateFlag)
                        {
                            lobjError = AddError(6315, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }


                        ////if (lbusPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value != lstrTaxIdentifier.Substring(0, 2).ToString()).Count() > 0)
                        ////{
                        //    lobjError = AddError(6315, "");
                        //    aarrErrors.Add(lobjError);
                        //    return aarrErrors;

                        ////}

                    }
                }

                if (ldecAdditionalTaxAmount > 0)
                {
                    if (lbusPayeeAccount.idtNextBenefitPaymentDate == null)
                        lbusPayeeAccount.LoadNextBenefitPaymentDate();
                    lbusPayeeAccount.LoadTaxableAmountForVariableTax(lbusPayeeAccount.idtNextBenefitPaymentDate);
                    if (ldecAdditionalTaxAmount > lbusPayeeAccount.idecTotalTaxableAmountForVariableTax)
                    {
                        lobjError = AddError(6081, "");
                        aarrErrors.Add(lobjError);
                    }
                }

                //if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption != busConstant.LUMP_SUM_DESCRIPTION && astrBenefitDistributionType == busConstant.Benefit_Distribution_Type_Monthly_Benefit)
                //{
                //    if (Convert.ToInt32(ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]) > 0 && Convert.ToInt32(ahstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]) == 0)
                //    {
                //        lobjError = AddError(6314, "");
                //        aarrErrors.Add(lobjError);
                //        return aarrErrors;

                //    }


                //}
                if (astrTaxOption != busConstant.NO_FEDRAL_TAX && astrTaxOption != busConstant.NO_STATE_TAX)
                {
                    if (astrTaxOption != busConstant.FLAT_PERCENT && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.tax_percentage"]).IsNotNullOrEmpty() && ldectaxPercentage != 0)
                    {
                        lobjError = AddError(6077, "");
                        aarrErrors.Add(lobjError);
                    }
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]).IsNullOrEmpty())
                {
                    lobjError = AddError(5113, "");
                    aarrErrors.Add(lobjError);
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]).IsNotNullOrEmpty() && Convert.ToDateTime(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]) < DateTime.Today)
                {
                    lobjError = AddError(5112, "");
                    aarrErrors.Add(lobjError);
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNotNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]).IsNotNullOrEmpty()
                    && Convert.ToDateTime(ahstParams["icdoPayeeAccountTaxWithholding.end_date"]) <= Convert.ToDateTime(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]))
                {
                    lobjError = AddError(5111, "");
                    aarrErrors.Add(lobjError);
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNotNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]).IsNotNullOrEmpty()
                    && Convert.ToDateTime(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]).Date > Convert.ToDateTime(ahstParams["icdoPayeeAccountTaxWithholding.end_date"]))
                {
                    lobjError = AddError(5139, "");
                    aarrErrors.Add(lobjError);
                }

                if (astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage == 0)
                {
                    lobjError = AddError(6061, "");
                    aarrErrors.Add(lobjError);
                }
                //if (astrTaxOption == busConstant.FLAT_DOLLAR && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]).IsNullOrEmpty())
                //{
                //    lobjError = AddError(6060, "");
                //    aarrErrors.Add(lobjError);
                //}

                if (astrBenefitDistributionType == busConstant.Benefit_Distribution_Type_LumpSum && astrMaritalStatus.IsNotNullOrEmpty() && lbusPayeeAccount.icdoPayeeAccount.retiree_incr_flag != busConstant.FLAG_YES)
                {
                    lobjError = AddError(6107, "");
                    aarrErrors.Add(lobjError);
                }
                if (astrBenefitDistributionType == busConstant.Benefit_Distribution_Type_LumpSum && !Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]).IsNullOrEmpty() && Convert.ToDecimal(ahstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]) != Decimal.Zero)
                {
                    lobjError = AddError(6108, "");
                    aarrErrors.Add(lobjError);
                }

                lbusPayeeAccount.LoadBreakDownDetails();

                if (lbusPayeeAccount.idecNextGrossPaymentACH <= 0)
                {
                    if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail == null)
                    {
                        lbusPayeeAccount.LoadPayeeAccountRolloverDetails();
                    }
                    if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Count() > 0)
                    {
                        if (!astrTaxOption.IsNullOrEmpty())
                        {
                            if (astrTaxOption != busConstant.NO_STATE_TAX && astrTaxOption != busConstant.NO_FEDRAL_TAX)
                            {
                                lobjError = AddError(6179, "");
                                aarrErrors.Add(lobjError);
                            }
                        }
                    }
                }
            }
            lbusPayeeAccount.icdoPayeeAccount.istrTaxIdentifier = astrTaxIdentifier;
            return aarrErrors;
        }
        #endregion

        #region Create Payee Account TaxWithholding Detail
        public void CreateorUpdatePayeeAccoutTaxWithholdingDetail(int aintPaymentItemTypeID, decimal adecAmount, int aintPayeeAccountPaymentItemID, int aintPayeeAccountTaxWithHoldingDetailId = 0)
        {
            if (aintPayeeAccountTaxWithHoldingDetailId == 0)
            {
                if (iclbPayeeAccountTaxWithholdingItemDetail == null)
                    iclbPayeeAccountTaxWithholdingItemDetail = new Collection<busPayeeAccountTaxWithholdingItemDetail>();

                busPayeeAccountTaxWithholdingItemDetail lobjPayeeAccountTaxWithholdingItemDetail = new busPayeeAccountTaxWithholdingItemDetail { icdoPayeeAccountTaxWithholdingItemDetail = new cdoPayeeAccountTaxWithholdingItemDetail() };
                lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_id = icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id;
                lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id = aintPayeeAccountPaymentItemID;
                lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payment_item_type_id = aintPaymentItemTypeID;
                lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.amount = adecAmount;
                //if (iblnIsCalculateButtonClicked) //Re3view Vinovin
                //    iclbPayeeAccountTaxWithholdingItemDetail.Clear();
                iclbPayeeAccountTaxWithholdingItemDetail.Add(lobjPayeeAccountTaxWithholdingItemDetail);
                lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.Insert();
            }

            else if (aintPayeeAccountTaxWithHoldingDetailId > 0 && (!iclbPayeeAccountTaxWithholdingItemDetail.IsNullOrEmpty()))
            {
                this.iclbPayeeAccountTaxWithholdingItemDetail.Where(o => o.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_item_dtl_id == aintPayeeAccountTaxWithHoldingDetailId).First().icdoPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_id = this.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id;
                this.iclbPayeeAccountTaxWithholdingItemDetail.Where(o => o.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_item_dtl_id == aintPayeeAccountTaxWithHoldingDetailId).First().icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id = aintPayeeAccountPaymentItemID;
                this.iclbPayeeAccountTaxWithholdingItemDetail.Where(o => o.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_item_dtl_id == aintPayeeAccountTaxWithHoldingDetailId).First().icdoPayeeAccountTaxWithholdingItemDetail.payment_item_type_id = aintPaymentItemTypeID;
                this.iclbPayeeAccountTaxWithholdingItemDetail.Where(o => o.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_item_dtl_id == aintPayeeAccountTaxWithHoldingDetailId).First().icdoPayeeAccountTaxWithholdingItemDetail.amount = adecAmount;
                this.iclbPayeeAccountTaxWithholdingItemDetail.Where(o => o.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_item_dtl_id == aintPayeeAccountTaxWithHoldingDetailId).First().icdoPayeeAccountTaxWithholdingItemDetail.Update();
            }
        }
        #endregion

        /// <summary>
        /// Insert values in tax withholdings
        /// </summary>
        /// <param name="aintPayeeAccountId"></param>
        /// <param name="astrTaxType"></param>
        /// <param name="astrBenefitDistributionType"></param>
        /// <param name="adtStartDate"></param>
        /// <param name="adtEndDate"></param>
        /// <param name="astrTaxOption"></param>
        /// <param name="aintTaxAllowance"></param>
        /// <param name="astrMaritalStatus"></param>
        /// <param name="adecAdditionalTaxAmt"></param>
        /// <param name="adecTaxPercent"></param>
        public void InsertValuesInTaxWithHolding(int aintPayeeAccountId, string astrTaxType, string astrBenefitDistributionType, DateTime adtStartDate, DateTime adtEndDate, string astrTaxOption,
                                                   int aintTaxAllowance, string astrMaritalStatus, decimal adecAdditionalTaxAmt, decimal adecTaxPercent,
                                                   decimal adecStep2b3 = 0, decimal adecStep3 = 0, decimal adecStep4A = 0, decimal adecStep4B = 0, decimal adecStep4C = 0)
        {
            //2022 FDRL tax withholding. Added aditional fields for new federal tax withholding method.

            if (icdoPayeeAccountTaxWithholding == null)
            {
                icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
            }

            icdoPayeeAccountTaxWithholding.payee_account_id = aintPayeeAccountId;
            icdoPayeeAccountTaxWithholding.tax_identifier_id = busConstant.TAX_IDENTIFIER_ID;
            icdoPayeeAccountTaxWithholding.tax_identifier_value = astrTaxType;
            icdoPayeeAccountTaxWithholding.benefit_distribution_type_id = busConstant.BENEFIT_DISTRIBUTION_ID;
            icdoPayeeAccountTaxWithholding.benefit_distribution_type_value = astrBenefitDistributionType;
            icdoPayeeAccountTaxWithholding.start_date = adtStartDate;
            icdoPayeeAccountTaxWithholding.end_date = adtEndDate;
            icdoPayeeAccountTaxWithholding.tax_option_id = busConstant.TAX_OPTION_ID;
            icdoPayeeAccountTaxWithholding.tax_option_value = astrTaxOption;
            icdoPayeeAccountTaxWithholding.tax_allowance = aintTaxAllowance;
            icdoPayeeAccountTaxWithholding.marital_status_id = busConstant.MARITIAL_STATUS_ID;
            icdoPayeeAccountTaxWithholding.marital_status_value = astrMaritalStatus;
            icdoPayeeAccountTaxWithholding.additional_tax_amount = adecAdditionalTaxAmt;
            icdoPayeeAccountTaxWithholding.tax_percentage = adecTaxPercent;
            icdoPayeeAccountTaxWithholding.step_2_b_3 = adecStep2b3;
            icdoPayeeAccountTaxWithholding.step_3_amount = adecStep3;
            icdoPayeeAccountTaxWithholding.step_4_a = adecStep4A;
            icdoPayeeAccountTaxWithholding.step_4_b = adecStep4B;
            icdoPayeeAccountTaxWithholding.step_4_c = adecStep4C;
            icdoPayeeAccountTaxWithholding.Insert();
        }


        public busPayeeAccountTaxWithholding LoadTaxWithHoldingByPayeeAccountIdAndTaxType(int aintPayeeAccountId, string astrTaxType)
        {
            DataTable ldtbList = Select<cdoPayeeAccountTaxWithholding>(
                new string[2] { enmPayeeAccountTaxWithholding.payee_account_id.ToString(), enmPayeeAccountTaxWithholding.tax_identifier_value.ToString() },
                new object[2] { aintPayeeAccountId, astrTaxType }, null, null);
            Collection<busPayeeAccountTaxWithholding> lclbBenefitCalculationDetail = GetCollection<busPayeeAccountTaxWithholding>(ldtbList, "icdoPayeeAccountTaxWithholding");

            //Rohan - Active Retiree Increase Batch
            if (lclbBenefitCalculationDetail != null && lclbBenefitCalculationDetail.Count() > 0)
            {
                if (lclbBenefitCalculationDetail.Where(item => item.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue || item.icdoPayeeAccountTaxWithholding.end_date > DateTime.Today).Count() > 0)
                    return lclbBenefitCalculationDetail.Where(item => item.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue || item.icdoPayeeAccountTaxWithholding.end_date > DateTime.Today).FirstOrDefault();
            }

            //if (ldtbList.Rows.Count > 0)
            //{
            //    return lclbBenefitCalculationDetail[0];
            //}
            return null;
        }

        public ArrayList btn_CalculatingTaxWithHolding()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = null;

            //  ibusPayeeAccount = new busPayeeAccount();
            //  ibusPayeeAccount.CalculateTaxForBenefit(this,false);
            DateTime adtPaymentDate = DateTime.ParseExact("" + istrTaxYear + "-12-01", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            decimal adecTaxableAmount = this.idecMonthlyGrossAmount - this.idecExclusionAmount;

            if(this.istrStateStatus == "" || this.istrStateStatus == null){

                lobjError = AddError(0," Select State Tax Identifier.");
                larrList.Add(lobjError);
                return larrList;

            }

            if ((this.istrMtlStatus == "" || this.istrMtlStatus == null) && this.istrStateStatus != "VAST")
            {

                lobjError = AddError(0, " Select Marital Status.");
                larrList.Add(lobjError);
                return larrList;

            }

            if (iintpersonal_exemptions > 0) {

                this.icdoPayeeAccountTaxWithholding.personal_exemptions = iintpersonal_exemptions;

            }
            
            if (iintage_and_blindness_exemptions > 0)
            {

                this.icdoPayeeAccountTaxWithholding.age_and_blindness_exemptions = iintage_and_blindness_exemptions;
            }


            busPayeeAccountHelper.CalculateFedOrStateTax(adecTaxableAmount, iintFedAllowanceNumber, adtPaymentDate, this.istrFedStatus, "FDRL", idecFedAdditionalTaxAmount, this, true, "Y");

            if (istrStateStatus != "" || istrStateStatus != "null")
            {
                busPayeeAccountHelper.CalculateFedOrStateTax(adecTaxableAmount, iintStateAllowanceNumber, adtPaymentDate, this.istrMtlStatus, this.istrStateStatus,  idecStateAdditionalTaxAmount, this, true, null);

            }
            larrList.Add(this);
            return larrList;

        }




        public void LoadTaxYear()
        {

            DataTable ldtblTaxYear = Select("cdoWithholdingInformation.GetTaxYear", new object[0] { });

            istrTaxYear = Convert.ToString(ldtblTaxYear.Rows[0]["TaxYear"]);
        }

        //public void GetMaritalStatusforTaxCalculation()
        //{
        //    this.ibusPayeeAccount = new busPayeeAccount();
        //    this.ibusPayeeAccount.GetMaritalStatusByTaxIdentifier("FDRL");


        //}

        public Collection<cdoCodeValue> GetMaritalStatusforTaxCalculation()
        {
            Collection<cdoCodeValue> lclbMaritalStatus = new Collection<cdoCodeValue>();
            DataTable ldtbResultMaritalStatus = busBase.Select("cdoPayeeAccount.GetMaritalStatus", new object[1] { "FDRL" });
            if (ldtbResultMaritalStatus.IsNotNull() && ldtbResultMaritalStatus.Rows.Count > 0)
            {
                lclbMaritalStatus = doBase.GetCollection<cdoCodeValue>(ldtbResultMaritalStatus);
            }

            return lclbMaritalStatus;
        }

        public Collection<cdoCodeValue> GetStateTaxIdentifierForCalculation()
        {
            Collection<cdoCodeValue> lclbStateTaxIdentifier = new Collection<cdoCodeValue>();
            DataTable ldtbResultStateTaxIdentifier = busBase.Select("cdoPayeeAccount.GetStateTaxIdentifier", new object[0] { });
            if (ldtbResultStateTaxIdentifier.IsNotNull() && ldtbResultStateTaxIdentifier.Rows.Count > 0)
            {
                lclbStateTaxIdentifier = doBase.GetCollection<cdoCodeValue>(ldtbResultStateTaxIdentifier);
            }

            return lclbStateTaxIdentifier;
        }

        public Collection<cdoCodeValue> GetMaritalStatusByTaxIdentifier(string astrCodeValue)
        {
            Collection<cdoCodeValue> lclbMaritalStatus = new Collection<cdoCodeValue>();

            if (astrCodeValue.IsNotNullOrEmpty())
            {
                DataTable ldtbResultMaritalStatus = busBase.Select("cdoPayeeAccount.GetMaritalStatus", new object[1] { astrCodeValue });
                if (ldtbResultMaritalStatus.IsNotNull() && ldtbResultMaritalStatus.Rows.Count > 0)
                {
                    lclbMaritalStatus = doBase.GetCollection<cdoCodeValue>(ldtbResultMaritalStatus);
                }
            }
            return lclbMaritalStatus;
        }

        public void GetTaxIdentifierValue()
        {
            string istrTaxIdentifier = icdoPayeeAccountTaxWithholding.tax_identifier_value.ToString();
        }

        public Collection<cdoCodeValue> GetTaxIdentifierStatesandFedral()
        {
            Collection<cdoCodeValue> lclbTaxIdentifierStatesandFedral = new Collection<cdoCodeValue>();


            DataTable ldtbTaxIdentifierStatesandFedral = busBase.Select("cdoPayeeAccount.LoadTaxIdentifiersvalues", new object[1] { icdoPayeeAccountTaxWithholding.payee_account_id });
            if (ldtbTaxIdentifierStatesandFedral.IsNotNull() && ldtbTaxIdentifierStatesandFedral.Rows.Count > 0)
            {
                lclbTaxIdentifierStatesandFedral = doBase.GetCollection<cdoCodeValue>(ldtbTaxIdentifierStatesandFedral);
            }

            return lclbTaxIdentifierStatesandFedral;
        }

    }
}
