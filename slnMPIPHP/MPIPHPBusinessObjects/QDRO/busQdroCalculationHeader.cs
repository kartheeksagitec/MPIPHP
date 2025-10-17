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
using System.Data.SqlClient;
using System.Windows.Forms;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busQdroCalculationHeader:
    /// Inherited from busQdroCalculationHeaderGen, the class is used to customize the business object busQdroCalculationHeaderGen.
    /// </summary>
    [Serializable]
    public class busQdroCalculationHeader : busQdroCalculationHeaderGen
    {
        #region Public Properties

        public busBenefitApplication ibusBenefitApplication { get; set; }
        public busBenefitApplication ibusBenefitApplicationForDisability { get; set; }
        public busBenefitCalculationHeader ibusBenefitCalculationHeader { get; set; }
        public Collection<busPersonAccountRetirementContribution> iclbPersonAccountRetirementContribution { get; set; }
        public Collection<busPayeeAccountStatus> iclbPayeeAccountStatusByCalculation { get; set; }
        public bool iblIsNew { get; set; }
        public busPerson ibusParticipant { get; set; }
        public busPerson ibusAlternatePayee { get; set; }
        public decimal idecLumpSumBenefitAmount = busConstant.ZERO_DECIMAL;
        public busCalculation ibusCalculation { get; set; }
        public bool iblnIsParticipantDead = false;
        public bool iblnIsParticipantVested = false;
        public bool iblnIsParticipantVestedinIAP = false;
        public bool iblnParticipantPayeeAccount = false;
        public decimal idecParticipantAmount { get; set; }
        public decimal idecParticipantMea { get; set; }


        public decimal idecIAP1979Hours { get; set; }
        public decimal idecThru79Hours { get; set; }
        public bool iblnVestingHasBeenChecked = false;
        public int iintTermCertainMonths { get; set; }
        public Collection<busPlanBenefitXr> iclbPlanBenefitXr { get; set; }
        public Collection<busPayeeAccount> iclbParticipantsPayeeAccount { get; set; }
        public Collection<busDisabilityRetireeIncrease> iclbDisabilityRetireeIncrease { get; set; }
        public decimal idecAlternatepayeeBenefitFraction { get; set; }

        public int iintIAPPayeeAccountID { get; set; }
        public int iintL52SplAccPayeeAccountID { get; set; }
        public int iintL161SplAccPayeeAccountID { get; set; }
        public int iintMPIPayeeAccountID { get; set; }
        public int iintL52PayeeAccountID { get; set; }
        public int iintL161PayeeAccountID { get; set; }
        public int iintL600PayeeAccountID { get; set; }
        public int iintL666PayeeAccountID { get; set; }
        public int iintL700PayeeAccountID { get; set; }
        public int iintEEUVHPPayeeAccountID { get; set; }
        public string astrFundName { get; set; }
        public string istrIsUSA { get; set; }

        public string istrParticipantFullName { get; set; }
        public int iintOriginalPayeeAccountId { get; set; }
        public int iintParticipantPayeeAccountId { get; set; }

        //Template DRO-0033
        public string istrLumpSumBenefitAmount { get; set; }
        public string istrPlan { get; set; }
        public int iintLumpSumYearByAge { get; set; }
        public string istrParticipantMPIID { get; set; }

        #endregion

        #region correspondence properties

        public Collection<busAttorney> iclbAttorney { get; set; }

        // List of Qualified Plans for the same payee &  alternate payee 
        public Collection<busDroBenefitDetails> iclbOtherQLFDDroBenefitDetails { get; set; }

        public string istrAttorneyName { get; set; }
        public string istrApprovedByUser { get; set; }
        public string istrApprovedByUserInitials { get; set; }
        public string istrBenefitSubType { get; set; }
        public DateTime dtEstimateRetrDate { get; set; }

        public decimal idecTotalIAPBalanceAmount { get; set; } // Part + IAP
        public decimal idecParticipantIAPBalanceAmount { get; set; }
        public decimal idecIAPAlternatePayeeLumpSumAmount { get; set; }

        public string istrIsAmountTaxabale { get; set; } //Will be set to Y if IAP Amount is greater than 200.
        public decimal iintIAPAsOfYear { get; set; }

        public decimal idecTotalAccruedBenefit { get; set; } //Part + ALTP
        public decimal idecParticipantAccruedBenefit { get; set; }
        public decimal idecAlternatePayeeBenefitBeforeConversion { get; set; }

        public decimal idecIAPAlternatePayeeAmount { get; set; }
        public decimal idecIAPParticipantAmount { get; set; }
        public decimal idecNetTotalBenefit { get; set; }
        public decimal idecMPIAlternatePayeeAmount { get; set; }
        public decimal idecMPIParticipantAmount { get; set; }
        public int iintyear { get; set; }
        public decimal idecMPIAltPayeeAmtTenYearCerAndLife { get; set; }


        public DateTime idtCommencementDate { get; set; }
        public string istrCurrentDate { get; set; }
        public string istrCommencementDate { get; set; }
        public string istrBeforeCommencementDate { get; set; }
        public string istrCurrentDateInDiffFormat { get; set; }

        //QDRO-0019
        public decimal idecUVHPContri { get; set; }
        public decimal idecUVHPInt{ get; set; }
        public decimal idecUVHPTotal { get; set; }

        public string istrIsRetireeModel { get; set; }
        #endregion correspondence properties

        #region Overidden Methods

        public override void BeforeValidate(Sagitec.Common.utlPageMode aenmPageMode)
        {
            if (!ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
            {
                iclbPersonAccountRetirementContribution = ibusBenefitCalculationHeader.LoadAllRetirementContributions(icdoQdroCalculationHeader.person_id,
                            ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
            }
            else
            {
                iclbPersonAccountRetirementContribution = ibusBenefitCalculationHeader.LoadAllRetirementContributions(icdoQdroCalculationHeader.person_id, null);
            }

            this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = GetRetirementDateforCalculation();

            this.ibusBenefitApplication.idecAge =
                    busGlobalFunctions.CalculatePersonAgeInDec(ibusParticipant.icdoPerson.idtDateofBirth,
                                                                GetRetirementDateforCalculation());

            this.icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement =
                                    Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(ibusAlternatePayee.icdoPerson.idtDateofBirth,
                                    GetRetirementDateforCalculation()));

            //We need to calculate participant age everytime based on Retirement date
            this.icdoQdroCalculationHeader.age = this.ibusBenefitApplication.idecAge; //Load the AGE OF THE MAIN HEADER OBJECT AS WELL


            if (this.icdoQdroCalculationHeader.ienuObjectState == ObjectState.Insert || icdoQdroCalculationHeader.qdro_calculation_header_id == 0)
            {
                iblIsNew = true;
                //int lintCounter = 0;
                //foreach (cdoDummyWorkData lcdoDummyWorkData in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI)
                //{
                //    lintCounter++;

                //    if (lintCounter > 1)
                //        lcdoDummyWorkData.age = lcdoDummyWorkData.age + lintCounter - 1;
                //}
            }
            else
            {
                iblIsNew = false;
            }   

            if (!iblnVestingHasBeenChecked)
            {
                this.ibusBenefitApplication.DetermineVesting();
                this.iblnVestingHasBeenChecked = true;
            }

            if (iblnIsParticipantDead == true)
            {
                this.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_PreRetirementDeath();
            }
            else if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.ForEach(a => a.icdoPersonAccount.istrRetirementSubType = string.Empty);
                this.ibusBenefitApplication.DetermineBenefitSubTypeandEligibility_Retirement();
            }

            if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES)
            {
                this.ibusBenefitApplicationForDisability.icdoBenefitApplication.retirement_date = GetRetirementDateforCalculation();
                this.ibusBenefitApplicationForDisability.DetermineBenefitSubTypeandEligibility_Disability();
            }

            #region Load QDRO Hours

            if (icdoQdroCalculationHeader.date_of_marriage != DateTime.MinValue && icdoQdroCalculationHeader.date_of_seperation != DateTime.MinValue)
            {
                if (icdoQdroCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                   LoadQDROHoursForMPIPPPlan();
                 
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
                {
                    LoadQDROHoursForLocal52Plan();
                   
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID)
                {
                      LoadQDROHoursForLocal161Plan();
                   
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID)
                {
                     LoadQDROHoursForLocal600Plan();
                   
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID)
                {
                    LoadQDROHoursForLocal666Plan();
                   
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                {
                     LoadQDROHoursForLocal700Plan();
                   
                }
            }

            #endregion


            //  this.iclbPersonAccountRetirementContribution = this.ibusBenefitCalculationHeader.LoadAllRetirementContributions(
            // this.ibusParticipant.icdoPerson.person_id);

            base.BeforeValidate(aenmPageMode);
        }
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);

            if (this.icdoQdroCalculationHeader.qdro_commencement_date !=DateTime.MinValue && this.icdoQdroCalculationHeader.qdro_commencement_date < Convert.ToDateTime("1/1/1753"))
            {
              this.iarrErrors.Add(GetErrorObject(utlMessageType.Framework, 115, "Qdro Commencement Date", this.icdoQdroCalculationHeader.qdro_commencement_date.ToString("dd/M/yyyy"), "1/1/1753"));
            }
        }

        public void CheckIfSharedDRO()
        {
            GetParticipantsPayeeAccountForGivenPlan();
            if (iclbParticipantsPayeeAccount != null && iclbParticipantsPayeeAccount.Count > 0)
            {
                iblnParticipantPayeeAccount = busConstant.BOOL_TRUE;
                iclbParticipantsPayeeAccount.FirstOrDefault().LoadBenefitDetails();
                iclbParticipantsPayeeAccount.FirstOrDefault().LoadNextBenefitPaymentDate();
                DataTable ldtblParticipantAmount = busBase.Select("cdoQdroCalculationHeader.GetBenefitAmount", new object[] { iclbParticipantsPayeeAccount.FirstOrDefault().icdoPayeeAccount.payee_account_id,
                                 iclbParticipantsPayeeAccount.FirstOrDefault().idtNextBenefitPaymentDate});
                if (ldtblParticipantAmount.Rows.Count > 0)
                {
                    idecParticipantAmount = Convert.ToDecimal(ldtblParticipantAmount.Rows[0][0]);
                    if (iclbQdroCalculationDetail != null && iclbQdroCalculationDetail.Count > 0)
                    {
                        iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.idecParticipantAmount
                            = idecParticipantAmount;

                        if (iclbQdroCalculationDetail.FirstOrDefault().iclbQdroCalculationOptions != null && iclbQdroCalculationDetail.FirstOrDefault().iclbQdroCalculationOptions.Count > 0)
                        {
                            idecAlternatepayeeBenefitFraction =
                                Math.Round((iclbQdroCalculationDetail.FirstOrDefault().iclbQdroCalculationOptions.FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount) / idecParticipantAmount, 3);
                        }
                    }
                }
            }
        }

        public void CheckIfSharedInterestDro()
        {
            #region Shared Interest DRO
            bool lblnParticipantPayeeAccount = busConstant.BOOL_FALSE;
            busPayeeAccount lbusPayeeAccount = null;
            decimal ldecParticipantAmount = new decimal();
            DataTable ldtblPayeeAccounts = null;
            busDroBenefitDetails lbusDroBenefitDetail = null;
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
            if (this.ibusQdroApplication.icdoDroApplication.dro_application_id > 0)
            {
                if (this.ibusQdroApplication.iclbDroBenefitDetails.IsNullOrEmpty())
                    this.ibusQdroApplication.LoadBenefitDetails();
                if (!this.ibusQdroApplication.iclbDroBenefitDetails.IsNullOrEmpty())
                {
                    ldtblPayeeAccounts = busBase.Select("cdoDroApplication.GetParticipantsPayeeAccountForGivenPlan", new object[] { this.icdoQdroCalculationHeader.person_id, this.icdoQdroCalculationHeader.iintPlanId });

                    Collection<busDroBenefitDetails> lclbDroBenefitDetails = new Collection<busDroBenefitDetails>();

                    lclbDroBenefitDetails = this.ibusQdroApplication.iclbDroBenefitDetails;

                    if (this.ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId && item.icdoDroBenefitDetails.dro_model_value == busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA).Count() > 0)
                    {
                        lbusDroBenefitDetail = this.ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId).FirstOrDefault();
                        lclbPayeeAccount = GetCollection<busPayeeAccount>(ldtblPayeeAccounts, "icdoPayeeAccount");
                    }
                }
            }
            else
            {
                DataTable ldtDroApplication = busBase.Select("cdoDroApplication.GetRetireeModelDroForPartandAlt", new object[3] { this.icdoQdroCalculationHeader.person_id, this.icdoQdroCalculationHeader.alternate_payee_id, this.icdoQdroCalculationHeader.iintPlanId });
                if (ldtDroApplication.Rows.Count > 0)
                {
                    ldtblPayeeAccounts = busBase.Select("cdoDroApplication.GetParticipantsPayeeAccountForGivenPlan", new object[] { this.icdoQdroCalculationHeader.person_id, this.icdoQdroCalculationHeader.iintPlanId });
                    if (ldtblPayeeAccounts.Rows.Count > 0)
                    {
                        lclbPayeeAccount = GetCollection<busPayeeAccount>(ldtblPayeeAccounts, "icdoPayeeAccount");
                    }
                }
            }
            if (!lclbPayeeAccount.IsNullOrEmpty())
            {
                lblnParticipantPayeeAccount = busConstant.BOOL_TRUE;
                lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                if (lclbPayeeAccount.Count() > 0)
                {
                    lbusPayeeAccount = lclbPayeeAccount.FirstOrDefault();
                    lbusPayeeAccount.LoadBenefitDetails();
                    lbusPayeeAccount.LoadNextBenefitPaymentDate();
                    DataTable ldtblParticipantAmount = busBase.Select("cdoQdroCalculationHeader.GetBenefitAmount", new object[] { lbusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                        lbusPayeeAccount.idtNextBenefitPaymentDate});
                    if (ldtblParticipantAmount.Rows.Count > 0)
                    {
                        if (Convert.ToString(ldtblParticipantAmount.Rows[0][0]).IsNotNullOrEmpty())
                        {
                            ldecParticipantAmount = Convert.ToDecimal(ldtblParticipantAmount.Rows[0][0]);
                        }
                    }
                }
            }

            //Post Retirement DRO
            if (lblnParticipantPayeeAccount)
            {
                this.idecAlternatepayeeBenefitFraction = decimal.Zero;
                this.iblnParticipantPayeeAccount = lblnParticipantPayeeAccount;
                this.idecParticipantAmount = ldecParticipantAmount;
                this.iclbParticipantsPayeeAccount = new Collection<busPayeeAccount>();
                this.iclbParticipantsPayeeAccount.Add(lbusPayeeAccount);

            }
            #endregion

        }

        public override void BeforePersistChanges()
        {
            if (!this.iblIsNew)
            {
                FlushOlderCalculations();
            }
            CheckIfSharedInterestDro();
            Setup_QDRO_Calculations();
            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            decimal ldecLocal700GauranteedAmt = 0;
            busActiveRetireeIncreaseContract lbusActiveRetireeIncreaseContract = new busActiveRetireeIncreaseContract();
            Collection<busActiveRetireeIncreaseContract> lclbActiveRetireeIncreaseContract = new Collection<busActiveRetireeIncreaseContract>();

            foreach (busQdroCalculationDetail lbusQdroCalculationDetail in iclbQdroCalculationDetail)
            {
                if ((iblIsNew == true || lbusQdroCalculationDetail.icdoQdroCalculationDetail.iblnIsNewRecord == true) && (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                    icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT ||
                    icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE) )
                {
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id = this.icdoQdroCalculationHeader.qdro_calculation_header_id;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.benefit_calculation_based_on_id = busConstant.BenefitCalculation.CALCULATION_BASED_ON_ID;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.benefit_calculation_based_on_value = busConstant.BenefitCalculation.CALCULATION_BASED_ON_HOUR;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.Insert();
                }
                else
                {
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.Update();
                }

                if (!lbusQdroCalculationDetail.iclbQdroCalculationOptions.IsNullOrEmpty())
                {
                    foreach (busQdroCalculationOptions lbusQdroCalculationOptions in lbusQdroCalculationDetail.iclbQdroCalculationOptions)
                    {
                        lbusQdroCalculationOptions.icdoQdroCalculationOptions.qdro_calculation_detail_id =
                                                        lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_detail_id;
                        lbusQdroCalculationOptions.icdoQdroCalculationOptions.Insert();

                        #region Calculate Retiree Increase

                        int lintPayementCount = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.GetCountOfPaymentMade",
                                                    new object[2] { this.iintOriginalPayeeAccountId, icdoQdroCalculationHeader.alternate_payee_id }, iobjPassInfo.iconFramework,
                                                    iobjPassInfo.itrnFramework);

                        //PROD PIR 127
                        if (((icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT
                            && lintPayementCount >= 1) || icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL) &&
                            icdoQdroCalculationHeader.retirement_date < DateTime.Now
                            && icdoQdroCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID &&
                            ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id ==
                                lbusQdroCalculationDetail.icdoQdroCalculationDetail.plan_id).FirstOrDefault().icdoDroBenefitDetails.alt_payee_increase == busConstant.FLAG_YES)
                        {
                            lclbActiveRetireeIncreaseContract = lbusActiveRetireeIncreaseContract.LoadActiveRetireeIncContractByRetirementDate(
                                                                GetRetirementDateforCalculation());
                            
                            foreach (busActiveRetireeIncreaseContract lbusRetireeIncreaseContract in lclbActiveRetireeIncreaseContract)
                            {
                                DateTime ldtRetireeIncreaseDate = new DateTime(lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.plan_year, 11, 01);

                                if (ldtRetireeIncreaseDate >= GetRetirementDateforCalculation())
                                {
                                    if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                                    {
                                        ldecLocal700GauranteedAmt =
                                            ibusCalculation.GetLocal700GuarentedAmt(lbusQdroCalculationDetail.icdoQdroCalculationDetail.person_account_id);
                                    }


                                    //Delete All Records from Disability Retiree Increase table for this calc id
                                    if (iclbDisabilityRetireeIncrease != null && iclbDisabilityRetireeIncrease.Count > 0)
                                    {
                                        foreach (busDisabilityRetireeIncrease lbusDisabilityRetireeIncrease in iclbDisabilityRetireeIncrease)
                                        {
                                            lbusDisabilityRetireeIncrease.icdoDisabilityRetireeIncrease.Delete();
                                        }
                                        iclbDisabilityRetireeIncrease.Clear();
                                    }

                                    ibusCalculation.CalculateAndCreateRetireeIncreasePayeeAccount(null, lbusQdroCalculationOptions, lbusRetireeIncreaseContract,
                                                icdoQdroCalculationHeader.retirement_date.Year,0, icdoQdroCalculationHeader.qdro_calculation_header_id,
                                                ldecLocal700GauranteedAmt, Convert.ToDecimal(lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.percent_increase_value)
                                                , busConstant.BENEFIT_TYPE_QDRO);
                                }
                            }

                            if (ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id ==
                                lbusQdroCalculationDetail.icdoQdroCalculationDetail.plan_id).FirstOrDefault().icdoDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase == busConstant.FLAG_YES)
                            {
                                //Check if existing pending Retirement or disability or death pre retirement or post retirement adjustment calculation exists
                                DataTable ldtbParticipantCalculationRecord = busBase.Select("cdoBenefitCalculationHeader.GetCountOfPendingRetOrDisbOrDeathAdjCalc",
                                                                        new object[] { icdoQdroCalculationHeader.person_id, lbusQdroCalculationDetail.icdoQdroCalculationDetail.plan_id });

                                if (ldtbParticipantCalculationRecord.Rows.Count > 0)
                                {
                                    decimal ldecParticipantLocal700GauranteedAmt = 0;
                                    int lintParticipantPaymentCount = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.GetCountOfPaymentMade",
                                                        new object[2] { this.iintParticipantPayeeAccountId, icdoQdroCalculationHeader.person_id }, iobjPassInfo.iconFramework,
                                                        iobjPassInfo.itrnFramework);

                                    foreach (busActiveRetireeIncreaseContract lbusRetireeIncreaseContract in lclbActiveRetireeIncreaseContract)
                                    {
                                        DateTime ldtParticipantRetireeIncreaseDate = new DateTime(lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.plan_year, 11, 01);
                                        if (lintParticipantPaymentCount > 0 && ldtParticipantRetireeIncreaseDate >= 
                                                Convert.ToDateTime(ldtbParticipantCalculationRecord.Rows[0][enmBenefitCalculationHeader.retirement_date.ToString()]))
                                        {
                                            //Need to do calculation for particpant

                                            if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                                            {
                                                ldecParticipantLocal700GauranteedAmt =
                                                    ibusCalculation.GetLocal700GuarentedAmt(Convert.ToInt32(ldtbParticipantCalculationRecord.Rows[0][enmBenefitCalculationDetail.person_account_id.ToString()]));
                                            }

                                            busBenefitCalculationOptions lbusParticipantBenefitCalculationOptions = new busBenefitCalculationOptions();
                                            lbusParticipantBenefitCalculationOptions.icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions();
                                            lbusParticipantBenefitCalculationOptions.icdoBenefitCalculationOptions.LoadData(ldtbParticipantCalculationRecord.Rows[0]);

                                            ibusCalculation.CalculateAndCreateRetireeIncreasePayeeAccount(lbusParticipantBenefitCalculationOptions, null, lbusRetireeIncreaseContract,
                                                        Convert.ToDateTime(ldtbParticipantCalculationRecord.Rows[0][enmBenefitCalculationHeader.retirement_date.ToString()]).Year,0,
                                                        icdoQdroCalculationHeader.qdro_calculation_header_id,
                                                        ldecParticipantLocal700GauranteedAmt, Convert.ToDecimal(lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.percent_increase_value)
                                                        , busConstant.BENEFIT_TYPE_QDRO);
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                }

                if (!lbusQdroCalculationDetail.iclbQdroCalculationYearlyDetail.IsNullOrEmpty())
                {
                    foreach (busQdroCalculationYearlyDetail lbusQdroCalculationYearlyDetail in lbusQdroCalculationDetail.iclbQdroCalculationYearlyDetail)
                    {
                        lbusQdroCalculationYearlyDetail.icdoQdroCalculationYearlyDetail.qdro_calculation_detail_id =
                                                lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_detail_id;
                        lbusQdroCalculationYearlyDetail.icdoQdroCalculationYearlyDetail.Insert();
                    }
                }

                if (!lbusQdroCalculationDetail.iclbQdroIapAllocationDetail.IsNullOrEmpty())
                {
                    foreach (busQdroIapAllocationDetail lbusQdroIapAllocationDetail in lbusQdroCalculationDetail.iclbQdroIapAllocationDetail)
                    {
                        lbusQdroIapAllocationDetail.icdoQdroIapAllocationDetail.qdro_calculation_detail_id =
                                                lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_detail_id;
                        lbusQdroIapAllocationDetail.icdoQdroIapAllocationDetail.Insert();
                    }
                }
            }

            icdoQdroCalculationHeader.iintParticipantAtRetirement = Convert.ToInt32(Math.Floor(icdoQdroCalculationHeader.age));

            icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAgeInDec(
                                    ibusAlternatePayee.icdoPerson.idtDateofBirth, GetRetirementDateforCalculation())));

            base.AfterPersistChanges();

            if (this.ibusBaseActivityInstance.IsNotNull())
            {
                this.SetProcessInstanceParameters();
            }
        }

        #region Validate New

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();

            string lstrMPIPersonId = Convert.ToString(ahstParam["astr_person_mpi_id"]);
            string lstrMPIAltPayeeId = Convert.ToString(ahstParam["astr_alt_payee_id"]);
            int lintPlanID = Convert.ToInt32(ahstParam["aint_plan_id"]);
            int lintQdroAppID = 0;

            if (ahstParam["aint_qdro_application_id"].ToString() != "")
            {
                lintQdroAppID = Convert.ToInt32(ahstParam["aint_qdro_application_id"]);
            }
            else
            {
                lintQdroAppID = 0;
            }

            if (lstrMPIPersonId.IsNullOrEmpty() && lintQdroAppID == 0)
            {
                utlError lobjError = null;
                lobjError = AddError(4075, "");
                larrErrors.Add(lobjError);
            }
            if (lintPlanID == 0)
            {
                utlError lobjError = null;
                lobjError = AddError(5408, "");
                larrErrors.Add(lobjError);
            }
            if (lstrMPIAltPayeeId.IsNullOrEmpty() && lintQdroAppID == 0)
            {
                utlError lobjError = null;
                lobjError = AddError(1103, "");
                larrErrors.Add(lobjError);
            }

            if (lstrMPIPersonId.IsNotNullOrEmpty() && lstrMPIAltPayeeId.IsNotNullOrEmpty())
            {
                DataTable ldtbPersonIDs = busBase.Select("cdoDroApplication.GetPersonID", new object[2] { lstrMPIPersonId, lstrMPIAltPayeeId });
                if (ldtbPersonIDs.Rows.Count > 0)
                {
                    if (ldtbPersonIDs.Rows.Count == 1)
                    {
                        string lstrxyz = ldtbPersonIDs.Rows[0]["MPI_PERSON_ID"].ToString();
                        if (lstrxyz == lstrMPIPersonId)
                        {

                            utlError lobjError1 = null;
                            lobjError1 = AddError(2005, "");
                            larrErrors.Add(lobjError1);
                        }
                        else
                        {
                            utlError lobjError = null;
                            lobjError = AddError(2004, "");
                            larrErrors.Add(lobjError);

                        }
                        return larrErrors;
                    }
                }
                else
                {
                    utlError lobjError = null;
                    lobjError = AddError(2004, "");
                    larrErrors.Add(lobjError);

                    utlError lobjError1 = null;
                    lobjError1 = AddError(2005, "");
                    larrErrors.Add(lobjError1);

                    return larrErrors;
                }
            }

            if (lstrMPIPersonId.IsNotNullOrEmpty() && (lintPlanID != 0))
            {
                object ldtplancount = DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.CheckEnrolledPlan", new object[2] { lstrMPIPersonId, lintPlanID },
                                                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (Convert.ToInt32(ldtplancount) == 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(5412, "");
                    larrErrors.Add(lobjError);
                }
            }
            if (lintPlanID != 0 && lintQdroAppID != 0)
            {
                object ldtplancount = DBFunction.DBExecuteScalar("cdoDroBenefitDetails.CheckPlanIsAddedForParticipant", new object[2] { lintPlanID, lintQdroAppID },
                                                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (Convert.ToInt32(ldtplancount) == 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(5414, "");
                    larrErrors.Add(lobjError);
                }
            }
            if (lstrMPIPersonId.IsNotNullOrEmpty() && lstrMPIAltPayeeId.IsNotNullOrEmpty() && lstrMPIPersonId.Equals(lstrMPIAltPayeeId))
            {
                utlError lobjError = null;
                lobjError = AddError(2008, "");
                larrErrors.Add(lobjError);
            }
            return larrErrors;
        }

        #endregion

        #endregion

        #region Public Methods

        public busQdroCalculationHeader()
        {
            ibusCalculation = new busCalculation();
        }

        public override long iintPrimaryKey
        {
            get
            {
                if (iobjPassInfo?.idictParams != null && (iobjPassInfo.istrSenderID == "btnSave"
                    || iobjPassInfo.istrSenderID == "btnApprove" || iobjPassInfo.istrSenderID == "btnExecuteRefreshFromObject" || iobjPassInfo.istrSenderID == "btnCancel"))
                {
                    return icdoQdroCalculationHeader.qdro_calculation_header_id;
                }
                else return icdoQdroCalculationHeader.iintAPrimaryKey;
            }
        }

        /// <summary>
        /// If benefit commmencement date is given then consider this date else QDRO Determination date
        /// </summary>
        /// <returns></returns>
        public DateTime GetRetirementDateforCalculation()
        {
            if (icdoQdroCalculationHeader.benefit_comencement_date != DateTime.MinValue)
            {
                return icdoQdroCalculationHeader.benefit_comencement_date;
            }
            else if (ibusQdroApplication != null || icdoQdroCalculationHeader.qdro_commencement_date != DateTime.MinValue)
            {
                if (ibusQdroApplication != null && ibusQdroApplication.icdoDroApplication.dro_commencement_date != DateTime.MinValue)
                    return ibusQdroApplication.icdoDroApplication.dro_commencement_date;
                else if(icdoQdroCalculationHeader.qdro_commencement_date != DateTime.MinValue)
                    return icdoQdroCalculationHeader.qdro_commencement_date;
            }
            else
            {
                return icdoQdroCalculationHeader.retirement_date;
            }

            return icdoQdroCalculationHeader.retirement_date;
        }


        public virtual void LoadPlanBenefitsForPlan(int aintPlanId)
        {
            DataTable ldtbList = Select<cdoPlanBenefitXr>(
                new string[1] { enmPlanBenefitXr.plan_id.ToString() },
                new object[1] { aintPlanId }, null, null);
            iclbPlanBenefitXr = GetCollection<busPlanBenefitXr>(ldtbList, "icdoPlanBenefitXr");
        }

        /// <summary>
        /// Load Initial Data for QDRO Header
        /// </summary>
        /// <param name="aintPersonId"></param>
        /// <param name="aintQdroApplicationId"></param>
        /// <param name="abusAlternatePayee"></param>
        /// <param name="adtRetirementDate"></param>
        /// <param name="aintPlanId"></param>
        /// <param name="adtDateofMarr"></param>
        /// <param name="adtDateOfDivorce"></param>
        public void PopulateInitialDataQdroCalculationHeader(int aintPersonId, int aintQdroApplicationId, busPerson abusAlternatePayee, DateTime adtRetirementDate, int aintPlanId,
                                                                DateTime adtDateofMarr, DateTime adtDateOfDivorce, string astrIsParticipantDisabled, string astrCalculationType)
        {
            this.icdoQdroCalculationHeader.person_id = aintPersonId;
            this.icdoQdroCalculationHeader.qdro_application_id = aintQdroApplicationId;
            this.icdoQdroCalculationHeader.alternate_payee_id = abusAlternatePayee.icdoPerson.person_id;//lbusPerson.icdoPerson.person_id;
            this.icdoQdroCalculationHeader.alternate_payee_date_of_birth = abusAlternatePayee.icdoPerson.idtDateofBirth;
            this.icdoQdroCalculationHeader.alternate_payee_name = abusAlternatePayee.icdoPerson.istrFullName;
            this.icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement =
                                               Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(abusAlternatePayee.icdoPerson.idtDateofBirth,
                                               adtRetirementDate));
            this.icdoQdroCalculationHeader.qdro_commencement_date = adtRetirementDate;
            this.icdoQdroCalculationHeader.benefit_comencement_date = adtRetirementDate;
            this.icdoQdroCalculationHeader.calculation_type_id = busConstant.BenefitCalculation.CALCULATION_TYPE_CODE_ID;
            this.icdoQdroCalculationHeader.calculation_type_value = astrCalculationType;
            this.icdoQdroCalculationHeader.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
            this.icdoQdroCalculationHeader.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
            this.icdoQdroCalculationHeader.age = Math.Floor(busGlobalFunctions.CalculatePersonAgeInDec(ibusParticipant.icdoPerson.idtDateofBirth, adtRetirementDate));
            this.icdoQdroCalculationHeader.date_of_marriage = adtDateofMarr;
            this.icdoQdroCalculationHeader.date_of_seperation = adtDateOfDivorce;
            this.icdoQdroCalculationHeader.is_participant_disabled = astrIsParticipantDisabled;
            this.icdoQdroCalculationHeader.PopulateDescriptions();
            this.icdoQdroCalculationHeader.iintPlanId = aintPlanId;
        }

        /// <summary>
        /// Final Calculation code
        /// </summary>
        /// <param name="aintDroApplicationDetailId"></param>
        /// <param name="aintPersonAccountId"></param>
        /// <param name="aintPlanId"></param>
        /// <param name="astrPlanCode"></param>
        /// <param name="adtVestedDate"></param>
        /// <param name="astrRetirementSubType"></param>
        /// <param name="astrBenefitOptionValue"></param>
        public void SpawnFinalQdroCalculation(int aintDroApplicationDetailId, int aintPersonAccountId, int aintPlanId, string astrPlanCode,
                                                DateTime adtVestedDate, string astrRetirementSubType, string astrBenefitOptionValue, bool ablnParticipantPayeeAccount)
        {

            this.PopulateInitialDataQdroCalculationDetails(aintDroApplicationDetailId, aintPersonAccountId, astrRetirementSubType, adtVestedDate, aintPlanId, astrPlanCode);

            #region SWITCH CASE - INITIATE CALCULATION BASED ON THE PLAN
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                switch (astrPlanCode)
                {
                    case busConstant.Local_161:
                        LoadQDROHoursForLocal161Plan();
                        CalculateLocal161BenefitAmount(busConstant.BOOL_TRUE, astrBenefitOptionValue);
                        break;

                    case busConstant.Local_52:
                         LoadQDROHoursForLocal52Plan();
                          CalculateLocal52BenefitAmount(busConstant.BOOL_TRUE, astrBenefitOptionValue);
                        break;

                    case busConstant.Local_600:
                        LoadQDROHoursForLocal600Plan();
                        CalculateLocal600BenefitAmount(busConstant.BOOL_TRUE, astrBenefitOptionValue);
                        break;

                    case busConstant.Local_666:
                         LoadQDROHoursForLocal666Plan();
                      
                        CalculateLocal666BenefitAmount(busConstant.BOOL_TRUE, astrBenefitOptionValue);
                        break;

                    case busConstant.LOCAL_700:
                         LoadQDROHoursForLocal700Plan();
                       
                        CalculateLocal700BenefitAmount(busConstant.BOOL_TRUE, astrBenefitOptionValue);
                        break;


                    case busConstant.MPIPP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                        {
                            this.iblnIsParticipantVested = true;
                            LoadQDROHoursForMPIPPPlan();
                          
                            this.CalculateFinalBenefitForPension(astrBenefitOptionValue);
                        }
                        //else if (this.iblnParticipantPayeeAccount)
                        //{
                        //    this.CalculateFinalBenefitForPension(astrBenefitOptionValue);
                        //}
                        break;

                    case busConstant.IAP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                        {
                            this.CalculateIAPBenefitAmount();
                        }
                        break;
                }
            }
            #endregion
        }

        /// <summary>
        /// Method used for loading detail object in final Calculation
        /// </summary>
        /// <param name="aintDroBenefitDetailId"></param>
        /// <param name="aintPersonAccountId"></param>
        /// <param name="astrRetirementSubTypeValue"></param>
        /// <param name="adtVestedDate"></param>
        /// <param name="aintPlanId"></param>
        /// <param name="astrPlanCode"></param>
        public void PopulateInitialDataQdroCalculationDetails(int aintDroBenefitDetailId, int aintPersonAccountId,
                                                                string astrRetirementSubTypeValue, DateTime adtVestedDate, int aintPlanId, string astrPlanCode)
        {
            switch (astrRetirementSubTypeValue)
            {
                case busConstant.CodeValueAll:
                    // Create a collection of benefit calc detail objects

                    break;
                default:
                    busQdroCalculationDetail lbusQdroCalculationDetail = new busQdroCalculationDetail()
                    {
                        icdoQdroCalculationDetail = new cdoQdroCalculationDetail()
                    };

                    lbusQdroCalculationDetail.iclbQdroCalculationOptions = new Collection<busQdroCalculationOptions>();
                    lbusQdroCalculationDetail.iclbQdroCalculationYearlyDetail = new Collection<busQdroCalculationYearlyDetail>();
                    lbusQdroCalculationDetail.iclbQdroIapAllocationDetail = new Collection<busQdroIapAllocationDetail>();

                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_application_detail_id = aintDroBenefitDetailId;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.person_account_id = aintPersonAccountId;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.plan_id = aintPlanId;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.istrPlanCode = astrPlanCode;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.vested_date = adtVestedDate;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.benefit_subtype_id = busConstant.RETIREMENT_TYPE_CODE_ID;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.benefit_subtype_value = astrRetirementSubTypeValue;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;

                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.uvhp_flag = busConstant.FLAG_NO;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.l52_spl_acc_flag = busConstant.FLAG_NO;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.l161_spl_acc_flag = busConstant.FLAG_NO;
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.ee_flag = busConstant.FLAG_NO;

                    //Post Retirement DRO
                    if (iblnParticipantPayeeAccount)
                    {
                        lbusQdroCalculationDetail.icdoQdroCalculationDetail.retired_participant_amount = idecParticipantAmount;

                        if (this.iclbParticipantsPayeeAccount != null && this.iclbParticipantsPayeeAccount.Count > 0)
                            lbusQdroCalculationDetail.icdoQdroCalculationDetail.referenece_participant_payee_account_id
                                = this.iclbParticipantsPayeeAccount.First().icdoPayeeAccount.payee_account_id;
                    }

                    this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetail);
                    break;
            }
        }
        /// <summary>
        /// Cancel existing Calculation on click of cancel button
        /// </summary>
        public ArrayList btn_CancelCalculation()
        {
            //PIR-843: Allow Cancel if all the records are Cancelled or Reclaimed status
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();
        
            if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
            {
                
                if (this.icdoQdroCalculationHeader.qdro_calculation_header_id > 0)
                {
                    utlError lobjError = null;
                    object lobjCheckDistributionStatusValue = null;
                    int lintCheckDistributionStatusValue = 0;
                    lobjCheckDistributionStatusValue= DBFunction.DBExecuteScalar("cdoPaymentHistoryHeader.GetQDROActiveDistributionRecord",
                                new object[1] { this.icdoQdroCalculationHeader.qdro_calculation_header_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (lobjCheckDistributionStatusValue != null)
                    {
                        lintCheckDistributionStatusValue = ((int)lobjCheckDistributionStatusValue);
                    }
                    if (lintCheckDistributionStatusValue > 0)
                    {
                        lobjError = AddError(6102, "");
                        iarrErrors.Add(lobjError);
                        return iarrErrors;
                    }
                }
            }
            this.icdoQdroCalculationHeader.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CANCELED;
            this.icdoQdroCalculationHeader.Update();
            if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
            {
                int lintQDROBenefitId = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == icdoQdroCalculationHeader.iintPlanId).First().
                         icdoQdroCalculationDetail.qdro_application_detail_id;

                busDroBenefitDetails lbusDroBenefitDetails = new busDroBenefitDetails();
                lbusDroBenefitDetails.FindDroBenefitDetails(lintQDROBenefitId);

                lbusDroBenefitDetails.icdoDroBenefitDetails.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CANCELED;
                lbusDroBenefitDetails.icdoDroBenefitDetails.Update();

                if (ibusQdroApplication != null)
                {
                    ibusQdroApplication.icdoDroApplication.is_disability_conversion = busConstant.FLAG_NO;
                    ibusQdroApplication.icdoDroApplication.Update();
                }
                if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                {
                    foreach (busQdroCalculationDetail lobjBenefitCalculationDetail in this.iclbQdroCalculationDetail)
                    {
                        this.LoadPayeeAccountStatusByQDROCalculationDetailID(lobjBenefitCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_detail_id);
                        foreach (busPayeeAccountStatus lobjPayeeAccountStatus in this.iclbPayeeAccountStatusByCalculation)
                        {
                            lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED;
                            lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                            lobjPayeeAccountStatus.icdoPayeeAccountStatus.Insert();
                        }
                    }

                }
                
            }
            return this.iarrErrors;
        }

        public void LoadPayeeAccountStatusByQDROCalculationDetailID(int aintCalculationDetailID)
        {
            DataTable ldtbList = busBase.Select("cdoPayeeAccount.GetQDROPayeeAccountStatusByCalculationDetailID", new object[1] { aintCalculationDetailID });
            iclbPayeeAccountStatusByCalculation = GetCollection<busPayeeAccountStatus>(ldtbList, "icdoPayeeAccountStatus");
        }

        /// <summary>
        /// Approve existing calculation on click of approve button
        /// </summary>
        /// <returns></returns>
        public ArrayList btn_ApproveCalculation()
        {
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();
            DataTable ldtbFinalApprovedEstimates = new DataTable();

            //PIR 210
            if (this.icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                foreach (busQdroCalculationDetail lbusQDROCalculationDetail in this.iclbQdroCalculationDetail)
                {
                    if (lbusQDROCalculationDetail.iclbQdroCalculationOptions.Count == 0)
                    {
                        utlError lobjError = AddError(6181, " ");
                        this.iarrErrors.Add(lobjError);
                        return this.iarrErrors;
                    }
                }
            }

            if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                ldtbFinalApprovedEstimates = busBase.Select("cdoQdroCalculationHeader.CheckIfApprovedEstimateExists",
                         new object[] { ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, icdoQdroCalculationHeader.iintPlanId });
            }

            if (ldtbFinalApprovedEstimates.Rows.Count > 0)
            {
                // Person Already has a Final Approved Estimate for the same plan throw hard error
                utlError lobjError = AddError(5181, " ");
                this.iarrErrors.Add(lobjError);
            }

            else
            {

                this.icdoQdroCalculationHeader.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_APPROVED;
                this.icdoQdroCalculationHeader.Update();

                if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                    icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                {
                    int lintQDROBenefitId = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == icdoQdroCalculationHeader.iintPlanId).First().
                             icdoQdroCalculationDetail.qdro_application_detail_id;

                    busDroBenefitDetails lbusDroBenefitDetails = new busDroBenefitDetails();
                    lbusDroBenefitDetails.FindDroBenefitDetails(lintQDROBenefitId);

                    lbusDroBenefitDetails.icdoDroBenefitDetails.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_APPROVED;
                    lbusDroBenefitDetails.icdoDroBenefitDetails.Update();


                    //Calculation Related to Retro Paymnet of Withheld Amount
                  
                    DateTime ldtWithholdingFromDate = new DateTime();
                    decimal ldecNonTaxableBeginningBalAlternatePayee = 0M;
                    decimal ldecRemainingNonTaxableBeginningBalParticipantTillDate = 0M;
                    DataTable ldtblTotalWithheldAmount = new DataTable();

                    Collection<busBenefitMonthwiseAdjustmentDetail> lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail = null;
                   
                    #region Post Retirement DRO
                    if (iblnParticipantPayeeAccount)
                    {
                        iclbParticipantsPayeeAccount.FirstOrDefault().LoadNextBenefitPaymentDate();
                        iclbParticipantsPayeeAccount.FirstOrDefault().LoadPaymentHistoryHeaderDetails();
                        iclbParticipantsPayeeAccount.FirstOrDefault().LoadWithholdingInformation();

                        if (iclbParticipantsPayeeAccount.FirstOrDefault().iclbWithholdingInformation.Count() > 0 
                            && iclbParticipantsPayeeAccount.FirstOrDefault().iclbWithholdingInformation.Where(item=>item.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue).Count() > 0
                           && iclbParticipantsPayeeAccount.FirstOrDefault().iclbWithholdingInformation.Where(item => item.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue)
                           .FirstOrDefault().icdoWithholdingInformation.withholding_date_from
                            < this.ibusQdroApplication.icdoDroApplication.dro_commencement_date)
                        {
                            //ldtWithholdingFromDate = iclbParticipantsPayeeAccount.FirstOrDefault().iclbWithholdingInformation.Min(item => item.icdoWithholdingInformation.withholding_date_from);

                            ldtWithholdingFromDate = iclbParticipantsPayeeAccount.FirstOrDefault().iclbWithholdingInformation
                                .Where(item => item.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue).FirstOrDefault().icdoWithholdingInformation.withholding_date_from;
                           
                            ldtblTotalWithheldAmount = Select("cdoWithholdingInformation.GetTotalWithheldAmount", new object[3] { this.iclbParticipantsPayeeAccount.FirstOrDefault().icdoPayeeAccount.payee_account_id ,
                            ldtWithholdingFromDate,this.ibusQdroApplication.icdoDroApplication.dro_commencement_date});


                            if (ldtblTotalWithheldAmount != null && ldtblTotalWithheldAmount.Rows.Count > 0)
                            {
                                lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();

                                foreach (DataRow ldrWithheldAmount in ldtblTotalWithheldAmount.Rows)
                                {
                                    busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail = new busBenefitMonthwiseAdjustmentDetail();
                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail = new cdoBenefitMonthwiseAdjustmentDetail();

                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date = Convert.ToDateTime(ldrWithheldAmount[enmPaymentHistoryHeader.payment_date.ToString().ToUpper()]);
                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid =
                                        (Convert.ToDecimal(ldrWithheldAmount["TAXABLE_WITHHOLD_FLAT_AMOUNT"]) / 100) * lbusDroBenefitDetails.icdoDroBenefitDetails.dro_withheld_perc;
                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid =
                                        (Convert.ToDecimal(ldrWithheldAmount["NON_TAXABLE_WITHHOLD_FLAT_AMOUNT"]) / 100) * lbusDroBenefitDetails.icdoDroBenefitDetails.dro_withheld_perc;

                                    lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Add(lbusBenefitMonthwiseAdjustmentDetail);
                                }
                            }
                        }

                        ldecRemainingNonTaxableBeginningBalParticipantTillDate =
                            iclbParticipantsPayeeAccount.FirstOrDefault().GetRemainingNonTaxableBeginningBalanaceTillDate(this.ibusQdroApplication.icdoDroApplication.dro_commencement_date.AddDays(-1));

                    }
                    #endregion
                    else
                    {
                        Collection<busPayeeAccount>  lclbParticipantPayeeAccountSeparateInterestDRO  = null;
                        lclbParticipantPayeeAccountSeparateInterestDRO = GetParticipantsPayeeAccountForSeparateInterestDRO();
                        if (lclbParticipantPayeeAccountSeparateInterestDRO.Count > 0)
                        {
                            lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().LoadPaymentHistoryHeaderDetails();
                            lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().LoadWithholdingInformation();
                            lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().LoadNextBenefitPaymentDate();

                            if (lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().iclbWithholdingInformation.Count() > 0
                                && lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().iclbWithholdingInformation
                                .Where(item=>item.icdoWithholdingInformation.withholding_date_to ==DateTime.MinValue).Count() > 0)
                            {
                                //ldtWithholdingFromDate = lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().iclbWithholdingInformation.Min(item => item.icdoWithholdingInformation.withholding_date_from);

                                ldtWithholdingFromDate = lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().iclbWithholdingInformation
                                .Where(item => item.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue).FirstOrDefault().icdoWithholdingInformation.withholding_date_from;

                                ldtblTotalWithheldAmount = Select("cdoWithholdingInformation.GetTotalWithheldAmount", new object[3] {lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().icdoPayeeAccount.payee_account_id ,
                            ldtWithholdingFromDate,lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().idtNextBenefitPaymentDate});

                                if (ldtblTotalWithheldAmount != null && ldtblTotalWithheldAmount.Rows.Count > 0)
                                {
                                    lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();

                                    foreach (DataRow ldrWithheldAmount in ldtblTotalWithheldAmount.Rows)
                                    {
                                        busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail = new busBenefitMonthwiseAdjustmentDetail();
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail = new cdoBenefitMonthwiseAdjustmentDetail();

                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date = Convert.ToDateTime(ldrWithheldAmount[enmPaymentHistoryHeader.payment_date.ToString().ToUpper()]);
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid =
                                            (Convert.ToDecimal(ldrWithheldAmount["TAXABLE_WITHHOLD_FLAT_AMOUNT"]) / 100) * lbusDroBenefitDetails.icdoDroBenefitDetails.dro_withheld_perc;
                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid =
                                            (Convert.ToDecimal(ldrWithheldAmount["NON_TAXABLE_WITHHOLD_FLAT_AMOUNT"]) / 100) * lbusDroBenefitDetails.icdoDroBenefitDetails.dro_withheld_perc;

                                        lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Add(lbusBenefitMonthwiseAdjustmentDetail);

                                    }

                                    //Withholding Date 
                                    foreach (busWithholdingInformation lbusWithholdingInfo in lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().iclbWithholdingInformation)
                                    {
                                        if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue)
                                        {
                                            if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from > lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().idtNextBenefitPaymentDate.AddDays(-1))
                                            {
                                                lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from;
                                            }
                                            else
                                            {
                                                lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lclbParticipantPayeeAccountSeparateInterestDRO.FirstOrDefault().idtNextBenefitPaymentDate.AddDays(-1);
                                            }

                                            lbusWithholdingInfo.icdoWithholdingInformation.Update();
                                        }
                                    }

                                }
                            }
                        }
 
                    }

                    #region PAYEE ACCOUNT RELATED LOGIC (PAYMENT - SPRINT 3.0) -- Abhishek
                    //check if this person is having a receiving payee account for Early retirement application  
                    int flag = 0;
                    if (flag != 1)  // DONE ON PURPOSE TO AVOID PAYEE ACCOUNT CODE TO BE EXECUTED FOR NOW
                    {
                        //this.ValidateHardErrors(utlPageMode.All);

                        //R3view - The Logic of this IF condition and whether the LAMBDA is safe 
                        if (this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.Count() > 0 &&
                            this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.alt_payee_benefit_amount <= Decimal.Zero)
                        {
                            utlError lobjError = new utlError();
                            lobjError = AddError(6057, "");//R3view 
                            this.iarrErrors.Add(lobjError);
                        }

                        else if (this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.Count() > 0 &&
                            this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.alt_payee_benefit_amount > Decimal.Zero)
                        {
                            if (this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                            {
                                this.ibusBenefitCalculationHeader.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                                iclbPersonAccountRetirementContribution = ibusBenefitCalculationHeader.LoadAllRetirementContributions(icdoQdroCalculationHeader.person_id,
                                                                          ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                            }
                            int lintBenefitAccountID = 0;
                            int lintPayeeAccountID = 0;
                            string lstrFundsType = String.Empty;

                            //Benefit Account Related
                            decimal ldecAccountOwnerStartingTaxableAmount = 0.0M;
                            decimal ldecAccountOwnerStartingNonTaxableAmount = 0.0M;
                            decimal ldecAccountOwnerStartingGrossAmount = 0.0M;




                            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            busPayeeBenefitAccount lbusPayeeBenefitAccount = new busPayeeBenefitAccount { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };

                            DateTime ldteBenefitBeginDate = this.icdoQdroCalculationHeader.qdro_commencement_date;




                            switch (this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.plan_id)
                            {
                                //R3view - Based on Per Plan we need to set the TAXABLE and NON-TAXABLE ITEMS
                                case busConstant.MPIPP_PLAN_ID:
                                    if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                                    {
                                       
                                        ldecAccountOwnerStartingNonTaxableAmount =
                                        ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.idecVestedEE;

                                        //Below if loop ,as a fix to UAT PIR 1004
                                        if (iblnParticipantPayeeAccount)
                                        {
                                            ldecAccountOwnerStartingTaxableAmount = ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.idecVestedEEInterest;
                                        }
                                        else
                                        {
                                            ldecAccountOwnerStartingTaxableAmount = this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.vested_ee_interest;
                                        }
                                       
                                       

                                        ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingNonTaxableAmount + ldecAccountOwnerStartingTaxableAmount;
                                    }
                                    // Visible only for MPI
                                    break;

                                case busConstant.IAP_PLAN_ID:
                                    //GROSS - IAP ACCOUNT BALANCE  - TILL DATE
                                    if (this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                                    {
                                        ldecAccountOwnerStartingGrossAmount = (from contribution in this.iclbPersonAccountRetirementContribution
                                                                               where contribution.icdoPersonAccountRetirementContribution.person_account_id == this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.person_account_id
                                                                               select contribution.icdoPersonAccountRetirementContribution.iap_balance_amount).Sum();

                                    }
                                    break;

                                case busConstant.LOCAL_161_PLAN_ID:

                                    break;

                                case busConstant.LOCAL_52_PLAN_ID:

                                    break;

                                case busConstant.LOCAL_600_PLAN_ID:

                                    break;

                                case busConstant.LOCAL_666_PLAN_ID:

                                    break;

                                case busConstant.LOCAL_700_PLAN_ID:

                                    break;

                            }

                            //Payee Account
                            //R3view this code
                            int iintDROApplicationDetailId = 0; // PROD PIR 569
                            if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == icdoQdroCalculationHeader.iintPlanId).Count() > 0)
                            {
                                iintDROApplicationDetailId = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == icdoQdroCalculationHeader.iintPlanId).First().
                              icdoQdroCalculationDetail.qdro_application_detail_id;
                            }
                            //Benefit Account
                            lintBenefitAccountID = busPayeeAccountHelper.IsBenefitAccountExists(this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                                                                                                 busConstant.BENEFIT_TYPE_QDRO, lstrFundsType,
                                                                                                 0, iintDROApplicationDetailId);  //R3view  sHOULD WE TAKE BENEFIT TYPE QDRO FOR THE SAKE OF PAYEE ACCOUNT

                            lintBenefitAccountID = lbusPayeeBenefitAccount.ManagePayeeBenefitAccount(lintBenefitAccountID, this.icdoQdroCalculationHeader.person_id,
                                                                              this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                                                                              ldecAccountOwnerStartingTaxableAmount, ldecAccountOwnerStartingNonTaxableAmount, ldecAccountOwnerStartingGrossAmount, lstrFundsType);

                            lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(this.icdoQdroCalculationHeader.alternate_payee_id, lintBenefitAccountID, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE, busConstant.BENEFIT_TYPE_QDRO, false, this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.plan_id,aintDROApplicationDetailId : iintDROApplicationDetailId);

                            decimal ldecMinimumGuarantee = 0.0M;
                            decimal ldecNonTaxableBeginningBalance = 0.0M;
                            DateTime ldteTermCertainEndDate = new DateTime();
                            //PIR-786
                            string lstrFamilyRelationshipValue = string.Empty;
                            lstrFamilyRelationshipValue = this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.alt_payee_relationship_value;

                            //R3view -- IF Term Year Certain Option FIND the end Date 
                            LoadPlanBenefitsForPlan(this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.plan_id);
                            iintTermCertainMonths = busConstant.ZERO_INT;
                            iintTermCertainMonths = busPayeeAccountHelper.IsTermCertainBenefitOption(this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.plan_benefit_id, this.iclbPlanBenefitXr);
                            if (iintTermCertainMonths > 0)
                            {
                                ldteTermCertainEndDate = ldteBenefitBeginDate.AddMonths(iintTermCertainMonths);
                                if (ldteTermCertainEndDate != DateTime.MinValue)
                                    ldteTermCertainEndDate = ldteTermCertainEndDate.AddDays(-1);
                            }


                            if (this.iblnParticipantPayeeAccount)
                            {
                                this.iclbParticipantsPayeeAccount.FirstOrDefault().LoadPaymentHistoryHeaderDetails();

                                ldecNonTaxableBeginningBalance = (ldecRemainingNonTaxableBeginningBalParticipantTillDate * idecAlternatepayeeBenefitFraction);
                                ldecNonTaxableBeginningBalAlternatePayee = ldecNonTaxableBeginningBalance;
                            }
                            else
                            {
                                ldecNonTaxableBeginningBalance = this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.alt_payee_ee_contribution;
                            }

                            bool lblnAdjustmentPaymentFlag = false;
                            if (this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.adjustment_iap_payment_flag == busConstant.FLAG_YES)
                            {
                                lblnAdjustmentPaymentFlag = true;
                            }

                            //rid 82043
                            if (this.icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES)
                            {
                                lintPayeeAccountID = lbusPayeeAccount.ManagePayeeAccount(lintPayeeAccountID, this.icdoQdroCalculationHeader.alternate_payee_id, 0, 0, 0,
                                                                                      this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.qdro_application_detail_id,
                                                                                      this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.qdro_calculation_detail_id,
                                                                                       lintBenefitAccountID, busConstant.BENEFIT_TYPE_QDRO, busConstant.DISABILITY_TYPE_SSA, //.BENEFIT_TYPE_DISABILITY,  RID 89088
                                                                                      this.icdoQdroCalculationHeader.benefit_comencement_date, DateTime.MinValue, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE, lstrFamilyRelationshipValue,
                                                                                      this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.alt_payee_minimum_guarantee_amount,
                                                                                      ldecNonTaxableBeginningBalance, this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.plan_benefit_id,
                                                                                      ldteTermCertainEndDate, busConstant.FLAG_NO, busConstant.FLAG_NO, lblnAdjustmentPaymentFlag);
                            }
                            else
                            {
                                //R3view -- NonTaxable Beginning Balance
                                lintPayeeAccountID = lbusPayeeAccount.ManagePayeeAccount(lintPayeeAccountID, this.icdoQdroCalculationHeader.alternate_payee_id, 0, 0, 0,
                                                                                          this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.qdro_application_detail_id,
                                                                                          this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.qdro_calculation_detail_id,
                                                                                           lintBenefitAccountID, busConstant.BENEFIT_TYPE_QDRO, this.icdoQdroCalculationHeader.istrRetirementType,
                                                                                          ldteBenefitBeginDate, DateTime.MinValue, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE, lstrFamilyRelationshipValue,
                                                                                          this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.alt_payee_minimum_guarantee_amount,
                                                                                          ldecNonTaxableBeginningBalance, this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.plan_benefit_id,
                                                                                          ldteTermCertainEndDate, busConstant.FLAG_NO, busConstant.FLAG_NO, lblnAdjustmentPaymentFlag);
                            }

                            lbusPayeeAccount.LoadNextBenefitPaymentDate();
                            DateTime ldteNextBenefitPaymentDate = lbusPayeeAccount.idtNextBenefitPaymentDate;//R3vview this once with Vinovin
                            decimal ldecTaxableAmount = 0M;
                            decimal ldecNonTaxableAmount = 0M;

                            //R3view -- First Parameter Should be maybe Retirement or Payment Date Review the Function too       
                            if (this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)
                                && this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID)
                            {
                                busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.alt_payee_benefit_amount,
                                                                                        ref ldecNonTaxableAmount, ref ldecTaxableAmount, this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.alt_payee_ee_contribution);
                            }
                            else if (this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                            {
                                //Prod Pir 335 : In Qdro benefit option created is always of type lump sum : It contains the final amount after adjsuting net investment amounts
                                busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.alt_payee_benefit_amount,
                                                                         ref ldecNonTaxableAmount, ref ldecTaxableAmount, 0);
                            }
                            else
                            {
                                busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.alt_payee_benefit_amount,
                                                              ref ldecNonTaxableAmount, ref ldecTaxableAmount, this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.alt_payee_member_exclusion_amount);
                            }


                            if (ldecTaxableAmount > 0M)
                            {
                                if (this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM21", ldecTaxableAmount, "0", 0,
                                                                ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                else
                                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM1", ldecTaxableAmount, "0", 0,
                                                                ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                            }
                            if (ldecNonTaxableAmount > 0M)
                            {
                                if (this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM22", ldecNonTaxableAmount, "0", 0,
                                                               ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                else
                                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM2", ldecNonTaxableAmount, "0", 0,
                                                               ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                            }


                            //Create Payee Account in Review
                            if (this.icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                lbusPayeeAccount.CreateReviewPayeeAccountStatus();

                            //Retro Calculation Items to be Created 
                            if ((!this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                                && this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.plan_id != busConstant.IAP_PLAN_ID
                                && this.icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                            {
                                if (this.icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT && !iblnParticipantPayeeAccount)
                                {
                                    lbusPayeeAccount.LoadPayeeAccountPaymentItemType();

                                    DateTime ldtPaymentDateFrom = this.icdoQdroCalculationHeader.qdro_commencement_date;
                                    DateTime ldtPaymentDateTo = lbusPayeeAccount.idtLastBenefitPaymentDate;

                                    //Early To Disability : If Completed Separate Interest DRO exists and there are payments for that DRO after current DRO's DRO commencement date
                                    //there those will be considered as overpayment and will get adjusted here
                                    Collection<busPayeeAccount> lclbQDROCompletedPayeeAccounts = null;
                                    if (lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.IsNull())
                                        lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();

                                    //PROD PIR 96
                                    DataTable ldtblQDROCompletedPayeeAccounts = Select("cdoDroApplication.GetQDROCompletedPayeeAccounts",
                                        new object[3] { ibusQdroApplication.icdoDroApplication.person_id, ibusQdroApplication.icdoDroApplication.alternate_payee_id, lbusPayeeAccount.icdoPayeeAccount.iintPlanId });
                                   
                                    if (ldtblQDROCompletedPayeeAccounts != null && ldtblQDROCompletedPayeeAccounts.Rows.Count > 0)
                                    {
                                        lclbQDROCompletedPayeeAccounts = new Collection<busPayeeAccount>();
                                        lclbQDROCompletedPayeeAccounts = GetCollection<busPayeeAccount>(ldtblQDROCompletedPayeeAccounts, "icdoPayeeAccount");

                                        foreach (busPayeeAccount lbusQDROCompletedPayeeAccounts in lclbQDROCompletedPayeeAccounts)
                                        {
                                            lbusQDROCompletedPayeeAccounts.LoadBenefitDetails();
                                            lbusQDROCompletedPayeeAccounts.LoadDRODetails();
                                            lbusQDROCompletedPayeeAccounts.LoadNextBenefitPaymentDate();

                                            Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                                            lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusQDROCompletedPayeeAccounts, lbusQDROCompletedPayeeAccounts.icdoPayeeAccount.benefit_begin_date, lbusPayeeAccount.idtLastBenefitPaymentDate);

                                            foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                            {
                                                if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date >= this.icdoQdroCalculationHeader.qdro_commencement_date)
                                                {
                                                    if (lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail != null && lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Count > 0
                                                  && lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date == lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).Count() > 0)
                                                    {
                                                        lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date == lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date)
                                                            .FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid += lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid;

                                                        lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date == lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date)
                                                            .FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid += lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid;
                                                    }
                                                    else
                                                    {
                                                        lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Add(lbusBenefitMonthwiseAdjustmentDetail);
                                                    }

                                                }
                                            }

                                        }


                                        foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail)
                                        {
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmount;
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                        }

                                        busBenefitMonthwiseAdjustmentDetail lMissingbusBenefitMonthwiseAdjustmentDetail = null;
                                        while (ldtPaymentDateFrom <= ldtPaymentDateTo)
                                        {
                                            if (lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date == ldtPaymentDateFrom).Count() == 0)
                                            {
                                                lMissingbusBenefitMonthwiseAdjustmentDetail = new busBenefitMonthwiseAdjustmentDetail { icdoBenefitMonthwiseAdjustmentDetail = new cdoBenefitMonthwiseAdjustmentDetail() };
                                                lMissingbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmount;
                                                lMissingbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                                lMissingbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date = ldtPaymentDateFrom;
                                                lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Add(lMissingbusBenefitMonthwiseAdjustmentDetail);
                                            }
                                            ldtPaymentDateFrom = ldtPaymentDateFrom.AddMonths(1);
                                        }
                                        ibusCalculation.CreateOverpaymentUnderPayment(lbusPayeeAccount, lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_INITIAL);
                                    }
                                    else
                                    {
                                        lbusPayeeAccount.CreateRetroPayments(lbusPayeeAccount, ldteNextBenefitPaymentDate, this.icdoQdroCalculationHeader.qdro_commencement_date, lintPayeeAccountID, busConstant.RETRO_PAYMENT_INITIAL);
                                    }
                                }

                                #region Create Retro Payment Items For Alternate Payee's Withheld Amount
                                else if (this.iblnParticipantPayeeAccount && this.icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT/*&& lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail != null && lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Count > 0 
                                && lbusDroBenefitDetails.icdoDroBenefitDetails.dro_withheld_perc != 0*/)
                                {
                                    DateTime ldtPaymentDateFrom = new DateTime();
                                    DateTime ldtPaymentDateTo = new DateTime();

                                    if (lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.IsNull())
                                        lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();

                                    //If there was shared qdro linked with early retirement payee account 
                                    DataTable ldtbEarlyRetrQDROPayeeAccountId = busBase.Select("cdoDroApplication.GetQDROPayeeAccountLinkedWithEarlyRetr", new object[1] { lbusDroBenefitDetails.icdoDroBenefitDetails.dro_benefit_id });
                                    if (ldtbEarlyRetrQDROPayeeAccountId != null && ldtbEarlyRetrQDROPayeeAccountId.Rows.Count > 0
                                        && Convert.ToString(ldtbEarlyRetrQDROPayeeAccountId.Rows[0][enmPayeeAccount.payee_account_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                                    {
                                        busPayeeAccount lbusEarlyRetrQDROPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                        if (lbusEarlyRetrQDROPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldtbEarlyRetrQDROPayeeAccountId.Rows[0][enmPayeeAccount.payee_account_id.ToString().ToUpper()])))
                                            lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusEarlyRetrQDROPayeeAccount, lbusEarlyRetrQDROPayeeAccount.icdoPayeeAccount.benefit_begin_date, lbusPayeeAccount.idtLastBenefitPaymentDate);

                                        ldtPaymentDateFrom = lbusEarlyRetrQDROPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                                        ldtPaymentDateTo = lbusPayeeAccount.idtLastBenefitPaymentDate;

                                        foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail)
                                        {
                                            if (lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date >= this.icdoQdroCalculationHeader.qdro_commencement_date)
                                            {

                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmount;
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date = ldtPaymentDateFrom;
                                            }
                                        }

                                    }
                                    else
                                    {

                                        ldtPaymentDateFrom = this.icdoQdroCalculationHeader.qdro_commencement_date;
                                        ldtPaymentDateTo = lbusPayeeAccount.idtLastBenefitPaymentDate;
                                    }

                                    busBenefitMonthwiseAdjustmentDetail lMissingbusBenefitMonthwiseAdjustmentDetail = null;
                                    while (ldtPaymentDateFrom <= ldtPaymentDateTo)
                                    {
                                        if (lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date == ldtPaymentDateFrom).Count() == 0)
                                        {
                                            lMissingbusBenefitMonthwiseAdjustmentDetail = new busBenefitMonthwiseAdjustmentDetail { icdoBenefitMonthwiseAdjustmentDetail = new cdoBenefitMonthwiseAdjustmentDetail() };
                                            lMissingbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = ldecTaxableAmount;
                                            lMissingbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = ldecNonTaxableAmount;
                                            lMissingbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date = ldtPaymentDateFrom;
                                            lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Add(lMissingbusBenefitMonthwiseAdjustmentDetail);
                                        }
                                        ldtPaymentDateFrom = ldtPaymentDateFrom.AddMonths(1);
                                    }
                                    ibusCalculation.CreateOverpaymentUnderPayment(lbusPayeeAccount, lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_INITIAL); 
                                }

                                //PIR RID 71870
                                //if (lbusEarlyRetirementPayeeAccount != null && this.icdoBenefitCalculationHeader.payee_account_id > 0)
                                //{
                                //    Early_Retirement_Benefit_Application_Id = lbusEarlyRetirementPayeeAccount.icdoPayeeAccount.iintBenefitApplicationID;
                                //}
                                //this.CreatePayeeAccountForRetireeIncrease(lbusPayeeAccount, lintBenefitAccountID, lstrFamilyRelationshipValue, ldecNonTaxableBeginningBalance, ldteNextBenefitPaymentDate, Early_Retirement_Benefit_Application_Id, busConstant.BENEFIT_TYPE_DISABILITY);
                                ibusCalculation.CreatePayeeAccountForRetireeIncrease(lintBenefitAccountID, null, ldecNonTaxableBeginningBalance, ldteNextBenefitPaymentDate,
                                    lintPayeeAccountID, iclbDisabilityRetireeIncrease, null, this, null, iclbQdroCalculationDetail);

                            }


                            #endregion

                            if (this.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                            {

                                lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);//PIR 1055
                                //PIR 993  added check for Lumpsum benefit type for MPIPP plan.
                                if ((!this.iclbQdroCalculationDetail.First().iclbQdroCalculationOptions.First().icdoQdroCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)) &&
                                    this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.plan_id != busConstant.IAP_PLAN_ID)
                                {
                                    //BR : 
                                    //Payment Adjustments - Benefit Adjustment Batch
                                    bool lblnAdjustmentCalculationForRetireeIncrease = false;
                                    if (iclbDisabilityRetireeIncrease != null && iclbDisabilityRetireeIncrease.Count > 0)
                                        lblnAdjustmentCalculationForRetireeIncrease = true;

                                    lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                                    lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date, lbusPayeeAccount.idtLastBenefitPaymentDate, lblnAdjustmentCalculationForRetireeIncrease);
                                    ibusCalculation.CalculateAmountShouldHaveBeenPaid(lbusPayeeAccount, ref lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail);
                                    ibusCalculation.CalculateRetireeIncreaseAmountShouldHaveBeenPaid(lbusPayeeAccount, iclbDisabilityRetireeIncrease, ref lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail);// PROD PIR 581
                                    ibusCalculation.CreateOverpaymentUnderPayment(lbusPayeeAccount, lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH);
                                    //PIR RID 71870
                                    ibusCalculation.CreatePayeeAccountForRetireeIncrease(lintBenefitAccountID, null, ldecNonTaxableBeginningBalance, ldteNextBenefitPaymentDate,
                                        lintPayeeAccountID, iclbDisabilityRetireeIncrease, null, this, null, iclbQdroCalculationDetail);
                                }
                                // ibusCalculation.CreatePayeeAccountForRetireeIncrease(lintBenefitAccountID, null, ldecNonTaxableBeginningBalance, ldteNextBenefitPaymentDate,
                                //    lintPayeeAccountID, iclbDisabilityRetireeIncrease, null, this, null, iclbQdroCalculationDetail);
                            }

                            if (this.ibusBaseActivityInstance.IsNotNull())
                            {
                                this.SetWFVariables4PayeeAccount(lintPayeeAccountID, this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.plan_id);
                                this.SetProcessInstanceParameters();
                            }

                            #region UPDATE PAYMENT ITEMS FOR PARTICIPANT'S PAYEE ACCOUNT


                            if (this.iblnParticipantPayeeAccount && icdoQdroCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)    //Need to check for this IF condition
                            {
                                decimal ldecParticipantsPrevTaxableAmt = busConstant.ZERO_DECIMAL;
                                decimal ldecParticipantsPrevNonTaxableAmt = busConstant.ZERO_DECIMAL;

                                busPayeeAccount lbusParticipantPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                lbusParticipantPayeeAccount = this.iclbParticipantsPayeeAccount.FirstOrDefault();

                                lbusParticipantPayeeAccount.LoadBenefitDetails();

                                //PIR 993  added check for Lumpsum benefit type for MPIPP plan.
                                if (lbusParticipantPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION)
                                    return this.iarrErrors;

                                //Create Payee Account Minimum Guarantee History
                                if (lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance > 0)
                                {
                                    lbusParticipantPayeeAccount.CreatePayeeAccountMinimumGuaranteeHistory(lbusParticipantPayeeAccount.icdoPayeeAccount.payee_account_id, lbusParticipantPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount,
                                        lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance, this.ibusQdroApplication.icdoDroApplication.dro_commencement_date);
                                }
                                else
                                {
                                    lbusParticipantPayeeAccount.CreatePayeeAccountMinimumGuaranteeHistory(lbusParticipantPayeeAccount.icdoPayeeAccount.payee_account_id, lbusParticipantPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount,
                                       lbusParticipantPayeeAccount.icdoPayeeAccount.remaining_non_taxable_from_conversion, this.ibusQdroApplication.icdoDroApplication.dro_commencement_date);
                                }

                                //Minimum Gaurentee Amount
                                lbusParticipantPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount =
                                    lbusParticipantPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount - iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.alt_payee_minimum_guarantee_amount;

                                //Non Taxable Beginning Balance
                                if (lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance > 0)
                                {
                                    lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance =
                                      lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance - ldecNonTaxableBeginningBalAlternatePayee;
                                }
                                else
                                {
                                    lbusParticipantPayeeAccount.icdoPayeeAccount.remaining_non_taxable_from_conversion =
                                      lbusParticipantPayeeAccount.icdoPayeeAccount.remaining_non_taxable_from_conversion - ldecNonTaxableBeginningBalAlternatePayee;
                                }

                                lbusParticipantPayeeAccount.LoadPaymentItemType();
                                lbusParticipantPayeeAccount.LoadPayeeAccountPaymentItemType();
                                lbusParticipantPayeeAccount.LoadNextBenefitPaymentDate();
                                lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive = (from item in lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                                                     where busGlobalFunctions.CheckDateOverlapping(lbusParticipantPayeeAccount.idtNextBenefitPaymentDate,
                                                                         item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                                                                          select item).ToList().ToCollection<busPayeeAccountPaymentItemType>();

                                if (icdoQdroCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                                {

                                    if (lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                        lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                                    {
                                        ldecParticipantsPrevTaxableAmt = lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                             lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;

                                        if ((ldecParticipantsPrevTaxableAmt - ldecTaxableAmount) > 0)
                                        {
                                            lbusParticipantPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM1", ldecParticipantsPrevTaxableAmt - ldecTaxableAmount, "0", 0,
                                                lbusParticipantPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                        }

                                    }

                                    if (lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                         lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM2).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                                    {
                                        ldecParticipantsPrevNonTaxableAmt = lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                             lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM2).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;

                                        if ((ldecParticipantsPrevNonTaxableAmt - ldecNonTaxableAmount) > 0)
                                        {
                                            lbusParticipantPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM2", ldecParticipantsPrevNonTaxableAmt - ldecNonTaxableAmount, "0", 0,
                                             lbusParticipantPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                        }
                                    }

                                }
                                else
                                {
                                    if (lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                         lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                                    {
                                        ldecParticipantsPrevTaxableAmt = lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                             lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;

                                        if ((ldecParticipantsPrevTaxableAmt - ldecTaxableAmount) > 0)
                                        {
                                            lbusParticipantPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM1", ldecParticipantsPrevTaxableAmt - ldecTaxableAmount, "0", 0,
                                                lbusParticipantPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, "N", false, aintPayeeAccountPaymentItemTypeID: lbusParticipantPayeeAccount.icdoPayeeAccount.payee_account_id);
                                        }
                                    }
                                }

                                //Create Retro Payment Items For Remaining Withheld Amount(Under Payment)

                                Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();

                                if (this.icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                {
                                    if (iclbParticipantsPayeeAccount.FirstOrDefault().iclbWithholdingInformation != null
                                        && iclbParticipantsPayeeAccount.FirstOrDefault().iclbWithholdingInformation.Count > 0
                                        && iclbParticipantsPayeeAccount.FirstOrDefault().iclbWithholdingInformation.Where(item => item.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue).Count() > 0)
                                    {
                                        DateTime ldtWithholdingStartDate = DateTime.MinValue;
                                        ldtWithholdingStartDate = iclbParticipantsPayeeAccount.FirstOrDefault().
                                            iclbWithholdingInformation.Where(item => item.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue).FirstOrDefault().icdoWithholdingInformation.withholding_date_from;

                                        if (ibusQdroApplication.icdoDroApplication.dro_commencement_date < ldtWithholdingStartDate)
                                        {
                                            lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusParticipantPayeeAccount, ibusQdroApplication.icdoDroApplication.dro_commencement_date, lbusParticipantPayeeAccount.idtLastBenefitPaymentDate);
                                        }
                                        else
                                        {
                                            lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusParticipantPayeeAccount, ldtWithholdingStartDate
                                             , lbusParticipantPayeeAccount.idtLastBenefitPaymentDate);
                                        }
                                    }
                                    else
                                    {
                                        lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusParticipantPayeeAccount, ibusQdroApplication.icdoDroApplication.dro_commencement_date, lbusParticipantPayeeAccount.idtLastBenefitPaymentDate);
                                    }
                                    //Withholding Date 
                                    foreach (busWithholdingInformation lbusWithholdingInfo in iclbParticipantsPayeeAccount.FirstOrDefault().iclbWithholdingInformation)
                                    {
                                        if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue)
                                        {
                                            if (lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from > lbusParticipantPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1))
                                            {
                                                lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_from;
                                            }
                                            else
                                            {
                                                lbusWithholdingInfo.icdoWithholdingInformation.withholding_date_to = lbusParticipantPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1);
                                            }

                                            lbusWithholdingInfo.icdoWithholdingInformation.Update();
                                        }
                                    }
                                }
                                else
                                {
                                    lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusParticipantPayeeAccount, ibusQdroApplication.icdoDroApplication.dro_commencement_date, lbusParticipantPayeeAccount.idtLastBenefitPaymentDate);
                                }

                                if (lclbBenefitMonthwiseAdjustmentDetail.Count > 0)
                                {
                                    //decimal ldecMEA = 0M;
                                    decimal ldecNonTaxableBeginningBalanceLeft =
                                        iclbParticipantsPayeeAccount.FirstOrDefault().GetRemainingNonTaxableBeginningBalanaceTillDate(lclbBenefitMonthwiseAdjustmentDetail.FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.payment_date);

                                    //If there was shared qdro linked with early retirement payee account now Get Early Retiremnt Payee Account
                                    // DataTable ldtbEarlyRetrPayeeAccountId = busBase.Select("cdoDroApplication.GetEarlyRetrCompletedPayeeAccountId", new object[1] { lbusDroBenefitDetails.icdoDroBenefitDetails.dro_benefit_id });

                                    if (lbusParticipantPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                                    //&& ldtbEarlyRetrPayeeAccountId.Rows.Count > 0
                                    //&& Convert.ToString(ldtbEarlyRetrPayeeAccountId.Rows[0][enmQdroCalculationDetail.referenece_participant_payee_account_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                                    {
                                        busBenefitCalculationHeader lbusDisabilityBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                                        if (lbusDisabilityBenefitCalculationHeader.FindBenefitCalculationHeader(lbusParticipantPayeeAccount.icdoPayeeAccount.iintBenefitCalculationID))
                                        {
                                            busPayeeAccount lParticipantERPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                            if (lParticipantERPayeeAccount.FindPayeeAccount(lbusDisabilityBenefitCalculationHeader.icdoBenefitCalculationHeader.payee_account_id))
                                            {
                                                Collection<busBenefitMonthwiseAdjustmentDetail> lclbERBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                                                lParticipantERPayeeAccount.LoadNextBenefitPaymentDate();
                                                lclbERBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lParticipantERPayeeAccount, ibusQdroApplication.icdoDroApplication.dro_commencement_date, lParticipantERPayeeAccount.idtLastBenefitPaymentDate);

                                                if (lclbERBenefitMonthwiseAdjustmentDetail != null && lclbERBenefitMonthwiseAdjustmentDetail.Count > 0)
                                                {
                                                    foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                                    {
                                                        if (lclbERBenefitMonthwiseAdjustmentDetail.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).Count() > 0)
                                                        {
                                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid +=
                                                                lclbERBenefitMonthwiseAdjustmentDetail.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid;

                                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid +=
                                                               lclbERBenefitMonthwiseAdjustmentDetail.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                           lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid;

                                                        }
                                                    }
                                                }
                                            }
                                        }


                                    }
                                    


                                    foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                    {

                                            if (lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date
                                                == lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).Count() > 0)
                                            {
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid =
                                                ldecParticipantsPrevNonTaxableAmt
                                                - lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date
                                                == lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid;


                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid =
                                                  ldecParticipantsPrevTaxableAmt
                                                - lclbAlternatePayeesBenefitMonthwiseAdjustmentDetail.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date
                                                == lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid;

                                            }
                                            else
                                            {
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid =
                                                ldecParticipantsPrevNonTaxableAmt;

                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid =
                                                 ldecParticipantsPrevTaxableAmt;

                                            }

              
                                    }
                                }


                                ibusCalculation.CreateOverpaymentUnderPayment(lbusParticipantPayeeAccount, lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_QDRO);

                                if (this.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                {
                                    lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);//PIR 1055
                                }
                                if (this.icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                                {
                                    lbusParticipantPayeeAccount.CreateReviewPayeeAccountStatus();
                                }

                                lbusParticipantPayeeAccount.icdoPayeeAccount.Update();
                            }


                            #endregion

                        }
                    }

                    #endregion

                }

            }

            return this.iarrErrors;
        }


        public ArrayList btn_RefreshCalculation()
        {
            ArrayList larrList = new ArrayList();

            if (this.iclbQdroCalculationDetail.Count() > 0)
            {
                this.icdoQdroCalculationHeader.iintPlanId = this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail.plan_id;
            }

            // PIR-507 For Final Calculation Refresh Button
            DateTime ldtLastWoringDayOfParticipant = this.ibusCalculation.GetLastWorkingDate(this.ibusParticipant.icdoPerson.ssn);
            if (iclbQdroCalculationDetail.Count > 0 &&
                   iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.qdro_model_value == "STAF")
            {
                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = GetRetirementDateforCalculation() > ldtLastWoringDayOfParticipant ? DateTime.Now : GetRetirementDateforCalculation();
            }
            else
            {
                this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = GetRetirementDateforCalculation();
            }
            //Ticket#84882
            if (!this.iblIsNew)
                FlushOlderCalculationsForRefreshCalculation();
            if (!ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
            {
                iclbPersonAccountRetirementContribution = ibusBenefitCalculationHeader.LoadAllRetirementContributions(icdoQdroCalculationHeader.person_id,
                            ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
            }
            else
            {
                iclbPersonAccountRetirementContribution = ibusBenefitCalculationHeader.LoadAllRetirementContributions(icdoQdroCalculationHeader.person_id, null);
            }

            //this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = GetRetirementDateforCalculation();

            this.ibusBenefitApplication.idecAge =
                    busGlobalFunctions.CalculatePersonAgeInDec(ibusParticipant.icdoPerson.idtDateofBirth,
                                                                GetRetirementDateforCalculation());

            this.icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement =
                                    Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(ibusAlternatePayee.icdoPerson.idtDateofBirth,
                                    GetRetirementDateforCalculation()));

            //We need to calculate participant age everytime based on Retirement date
            this.icdoQdroCalculationHeader.age = this.ibusBenefitApplication.idecAge;
            if (!this.iblnParticipantPayeeAccount)
            {
                ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
            }

            if (!iblnVestingHasBeenChecked)
            {
                this.ibusBenefitApplication.DetermineVesting();
                this.iblnVestingHasBeenChecked = true;
            }

            if (iblnIsParticipantDead == true)
            {
                this.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_PreRetirementDeath();
            }
            else if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty() && !this.iblnParticipantPayeeAccount)
            {
                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.ForEach(a => a.icdoPersonAccount.istrRetirementSubType = string.Empty);
                this.ibusBenefitApplication.DetermineBenefitSubTypeandEligibility_Retirement();
            }

            if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES && !this.iblnParticipantPayeeAccount)
            {
                this.ibusBenefitApplicationForDisability.icdoBenefitApplication.retirement_date = GetRetirementDateforCalculation();
                this.ibusBenefitApplicationForDisability.DetermineBenefitSubTypeandEligibility_Disability();
            }

            #region Execute Spawn Final method

            int lintPlanBenefitId = 0;

            ibusQdroApplication.LoadBenefitDetails();

            busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
            lbusPlanBenefitXr.FindPlanBenefitXr(lintPlanBenefitId);

            this.SpawnFinalQdroCalculation(ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == icdoQdroCalculationHeader.iintPlanId
                && item.istrSubPlan.IsNullOrEmpty()).FirstOrDefault().icdoDroBenefitDetails.dro_benefit_id,
                this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == icdoQdroCalculationHeader.iintPlanId).
                        FirstOrDefault().icdoPersonAccount.person_account_id,
                this.icdoQdroCalculationHeader.iintPlanId,
                ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == icdoQdroCalculationHeader.iintPlanId
                && item.istrSubPlan.IsNullOrEmpty()).FirstOrDefault().icdoDroBenefitDetails.istrPlanCode,
                ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == icdoQdroCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType,
                ibusQdroApplication.iclbDroBenefitDetails[0].icdoDroBenefitDetails.istrBenefitOptionValue, false);

            #endregion

            try
            {
                this.AfterPersistChanges();
            }
            catch
            {

            }


            LoadQdroCalculationDetails();

            foreach (busQdroCalculationDetail lbusQdroCalculationDetail in iclbQdroCalculationDetail)
            {

                lbusQdroCalculationDetail.icdoQdroCalculationDetail.istrPlanCode =
                    ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                        lbusQdroCalculationDetail.icdoQdroCalculationDetail.plan_id).First().icdoPersonAccount.istrPlanCode;

                lbusQdroCalculationDetail.LoadPlanDescription();
                lbusQdroCalculationDetail.LoadQdroCalculationOptionss();
                lbusQdroCalculationDetail.LoadQdroCalculationYearlyDetails();

                if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES)
                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.istrRetirementTypeDisability = busConstant.BENEFIT_TYPE_DISABILITY_DESC;
            }

            this.EvaluateInitialLoadRules();
            if (this.iclbQdroCalculationDetail.Count() > 0)
            {
                this.iclbQdroCalculationDetail.First().EvaluateInitialLoadRules();
                this.iclbQdroCalculationDetail.First().iobjMainCDO = this.iclbQdroCalculationDetail.First().icdoQdroCalculationDetail;
            }
            this.ValidateHardErrors(utlPageMode.Update);

            larrList.Add(this);
            return larrList;
        }

        /// <summary>
        /// Calculate Earliest eligible date to retire for a participant
        /// </summary>
        public void GetEarliestRetiremenDate()
        {
            decimal ldecParticipantCurrentAge = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusParticipant.icdoPerson.idtDateofBirth, DateTime.Now);

            #region To GET EARLIEST RTMT DATE AND CHECK ELIGBILITY if PEROSN IS < 66

            string lstrPlanCode = String.Empty;
            DataTable ldtbPlanCode = busBase.Select("cdoPlan.GetPlanCodebyId", new object[1] { this.icdoQdroCalculationHeader.iintPlanId });
            if (ldtbPlanCode.Rows.Count > 0)
            {
                lstrPlanCode = ldtbPlanCode.Rows[0][0].ToString();
            }

            #region Seed for getting Earliest RTMT date

            int lintAgeToStartChecking = 55;

            if (this.icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)// && lintAgeToStartChecking <= 52)
            {
                lintAgeToStartChecking = 52;
            }
            #endregion

            for (int i = lintAgeToStartChecking; i <= 66; i++)
            {
                if (this.ibusParticipant.icdoPerson.idtDateofBirth.Day == 1) //Sid Jain 05222012
                {
                    this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.ibusParticipant.icdoPerson.idtDateofBirth.AddYears(i);
                }
                else
                {
                    this.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(this.ibusParticipant.icdoPerson.idtDateofBirth.AddYears(i)).AddDays(1);
                }

                this.ibusBenefitApplication.idecAge = i;
                this.ibusBenefitApplication.DetermineBenefitSubTypeandEligibility_Retirement();

                if (!(this.ibusBenefitApplication.NotEligible) && this.ibusBenefitApplication.iclbEligiblePlans.Where(plan => plan == lstrPlanCode).Count() > 0)
                {
                    this.icdoQdroCalculationHeader.istrRetirementType =
                        this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                            this.icdoQdroCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                    break;
                }
            }
            #endregion

            this.icdoQdroCalculationHeader.retirement_date = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
            this.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusParticipant.icdoPerson.idtDateofBirth, this.icdoQdroCalculationHeader.retirement_date);
            this.icdoQdroCalculationHeader.iintParticipantAtRetirement = Convert.ToInt32(Math.Floor(this.ibusBenefitApplication.idecAge));
            this.icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement =
                   Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(this.ibusAlternatePayee.icdoPerson.idtDateofBirth, this.icdoQdroCalculationHeader.retirement_date));
            this.icdoQdroCalculationHeader.age = this.ibusBenefitApplication.idecAge;
            this.icdoQdroCalculationHeader.iintParticipantAtRetirement = Convert.ToInt32(Math.Floor(this.ibusBenefitApplication.idecAge));
        }

        /// <summary>
        /// Load qro calculation detail object
        /// </summary>
        public void LoadQdroCalculationDetails()
        {
            DataTable ldtbList = Select<cdoQdroCalculationDetail>(
                new string[1] { enmQdroCalculationDetail.qdro_calculation_header_id.ToString() },
                new object[1] { icdoQdroCalculationHeader.qdro_calculation_header_id }, null, "plan_id");
            iclbQdroCalculationDetail = GetCollection<busQdroCalculationDetail>(ldtbList, "icdoQdroCalculationDetail");

            //10 Percent
            if ((this.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT ||
                    this.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                    && iclbQdroCalculationDetail != null && iclbQdroCalculationDetail.Count() > 0 && iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.qdro_calculation_detail_id > 0)
            {
                DataTable ldtbPayeeAccountId = busBase.Select("cdoBenefitCalculationHeader.GetPayeeAccountIdFromCalcId", new object[2]
                { busConstant.BENEFIT_TYPE_QDRO,iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.qdro_calculation_detail_id  });

                if (ldtbPayeeAccountId != null && ldtbPayeeAccountId.Rows.Count > 0 && Convert.ToString(ldtbPayeeAccountId.Rows[0][0]).IsNotNullOrEmpty())
                {
                    icdoQdroCalculationHeader.iintPayeeAccountId = Convert.ToInt32(ldtbPayeeAccountId.Rows[0][0]);
                }
            }
        }

        public void GetParticipantsPayeeAccountForGivenPlan()
        {
            DataTable ldtblPayeeAccounts = busBase.Select("cdoDroApplication.GetParticipantsPayeeAccountForGivenPlan", new object[] { this.icdoQdroCalculationHeader.person_id,
                            this.icdoQdroCalculationHeader.iintPlanId });
            if (ldtblPayeeAccounts.Rows.Count > 0)
            {
                iclbParticipantsPayeeAccount = GetCollection<busPayeeAccount>(ldtblPayeeAccounts, "icdoPayeeAccount");
            }
        }

        public Collection<busPayeeAccount> GetParticipantsPayeeAccountForSeparateInterestDRO()
        {
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
            DataTable ldtblPayeeAccounts = busBase.Select("cdoDroApplication.GetParticipantsPayeeAccountForGivenPlan", new object[] { this.icdoQdroCalculationHeader.person_id,
                            this.icdoQdroCalculationHeader.iintPlanId });
            if (ldtblPayeeAccounts.Rows.Count > 0)
            {
                lclbPayeeAccount = GetCollection<busPayeeAccount>(ldtblPayeeAccounts, "icdoPayeeAccount");
            }

            return lclbPayeeAccount;
        }


        public virtual void LoadDisabilityRetireeIncreases()
        {
            DataTable ldtbList = Select<cdoDisabilityRetireeIncrease>(
                new string[1] { enmDisabilityRetireeIncrease.qdro_calculation_header_id.ToString() },
                new object[1] { icdoQdroCalculationHeader.qdro_calculation_header_id }, null, null);
            iclbDisabilityRetireeIncrease = GetCollection<busDisabilityRetireeIncrease>(ldtbList, "icdoDisabilityRetireeIncrease");
        }

        /// <summary>
        //CHECK IF RETIREMENT DATE IS AFTER OR EQUAL DRO DETERMINATION DATE
        //THEN CHECK FOR PRE RETIREMENT DRO
        //CHECK IF RETIREMENT DATE IS AFTER DRO DETERMINATION DATE
        //CHECK FOR POST RETIREMENT DRO
        /// </summary>
        public void InitiatePreAndPostRetirementDRO(int aintPersonId, busQdroApplication abusQdroApplication)
        {
            //DateTime ldtRetirementDate = DateTime.Now;

            //DataTable ldtbList = Select<cdoBenefitApplication>(
            //    new string[2] { enmBenefitApplication.person_id.ToString(), enmBenefitApplication.benefit_type_value.ToString() },
            //    new object[2] { aintPersonId, busConstant.BENEFIT_TYPE_RETIREMENT }, null, null);

            //if (ldtbList.Rows.Count > 0)
            //{
            //    ldtRetirementDate = Convert.ToDateTime(ldtbList.Rows[0][enmBenefitApplication.retirement_date.ToString()]);
            //}

            //two collection retiree model

            //non retiree model 
        }

        #endregion

        #region Private Methods

        private void Setup_QDRO_Calculations()
        {
            DateTime ldtVestedDate = DateTime.MinValue;
            int lintDROApplicationDetailId = 0;

            #region SETUP BENEFIT CALCULATION DETAIL BASED ON WHAT ESTIMATE HAS BEEN ASKED FOR

            if (this.iclbQdroCalculationDetail == null)
            {
                this.iclbQdroCalculationDetail = new Collection<busQdroCalculationDetail>();
            }

            if (this.icdoQdroCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                #region Setup Detail Records for MPIs Estimate

                // Create one Detail Record for MPIPP
                ldtVestedDate = DateTime.MinValue;

                //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP))
                {
                    ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    //Abhi-Code Added since if Participant is not NOT-VESTED how can you pay Alternate Payee from that
                    iblnIsParticipantVested = true;
                }

                if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0 &&
                    (!ibusQdroApplication.iclbDroBenefitDetails.Where(
                       item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty()).IsNullOrEmpty()))
                {
                    lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                       item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty()).First().icdoDroBenefitDetails.dro_benefit_id;
                }

                if ((iblIsNew && icdoQdroCalculationHeader.qdro_application_id == 0) || (iblIsNew && icdoQdroCalculationHeader.qdro_application_id != 0 && lintDROApplicationDetailId > 0))
                {
                    busQdroCalculationDetail lbusQdroCalculationDetail = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };
                    lbusQdroCalculationDetail.iobjMainCDO = lbusQdroCalculationDetail.icdoQdroCalculationDetail;

                    lbusQdroCalculationDetail.LoadData(this.icdoQdroCalculationHeader.qdro_calculation_header_id, this.icdoQdroCalculationHeader.iintPlanId, busConstant.MPIPP,
                    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                        this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                    ldtVestedDate, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.istrRetirementSubType, lintDROApplicationDetailId,
                    busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, true,this.idecParticipantAmount);

                    this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetail);
                }

                else if (!iclbQdroCalculationDetail.Where(
                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).IsNullOrEmpty())
                {
                    iclbQdroCalculationDetail.Where(
                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.retired_participant_amount = this.idecParticipantAmount;
                    iclbQdroCalculationDetail.Where(
                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.vested_date = ldtVestedDate;

                    iclbQdroCalculationDetail.Where(
                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                    iclbQdroCalculationDetail.Where(
                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.benefit_subtype_value =
                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).
                            First().icdoPersonAccount.istrRetirementSubType;
                }

                if (this.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE &&
                    icdoQdroCalculationHeader.is_alt_payee_eligible_for_iap == busConstant.FLAG_YES)
                {
                    // Create one Detail Record for IAP Plan as well
                    ldtVestedDate = DateTime.MinValue;

                    //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                    if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                    {
                        ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                        iblnIsParticipantVestedinIAP = true;
                    }

                    if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0 &&
                         (!ibusQdroApplication.iclbDroBenefitDetails.Where(
                           item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                               item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty()).IsNullOrEmpty()))
                    {
                        lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                           item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                               item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty()).First().icdoDroBenefitDetails.dro_benefit_id;
                    }


                    if ((iblIsNew && icdoQdroCalculationHeader.qdro_application_id == 0) || (iblIsNew && icdoQdroCalculationHeader.qdro_application_id != 0 && lintDROApplicationDetailId > 0))
                    {
                        busQdroCalculationDetail lbusQdroCalculationDetailIAP = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };
                        lbusQdroCalculationDetailIAP.iobjMainCDO = lbusQdroCalculationDetailIAP.icdoQdroCalculationDetail;

                        lbusQdroCalculationDetailIAP.LoadData(this.icdoQdroCalculationHeader.qdro_calculation_header_id, busConstant.IAP_PLAN_ID, busConstant.IAP,
                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.person_account_id,
                            ldtVestedDate, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.istrRetirementSubType, lintDROApplicationDetailId,
                            busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, true);

                        this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetailIAP);
                    }
                    else if (!iclbQdroCalculationDetail.Where(
                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty())
                    {
                        iclbQdroCalculationDetail.Where(
                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.vested_date = ldtVestedDate;

                        iclbQdroCalculationDetail.Where(
                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                        iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.benefit_subtype_value =
                                          this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrRetirementSubType;
                    }
                }
                else if (!iblIsNew && this.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE &&
                    icdoQdroCalculationHeader.is_alt_payee_eligible_for_iap == busConstant.FLAG_NO)
                {
                    if (!iclbQdroCalculationDetail.Where(
                      item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO &&
                          item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty())
                    {
                        iclbQdroCalculationDetail.Where(
                          item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO &&
                              item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.iblnIsNewRecord = false;
                    }

                    if (!iclbQdroCalculationDetail.Where(
                      item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty())
                    {
                        iclbQdroCalculationDetail.Where(
                          item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                          .First().icdoQdroCalculationDetail.iblnIsNewRecord = false;
                    }

                    if (!iclbQdroCalculationDetail.Where(
                      item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty())
                    {
                        iclbQdroCalculationDetail.Where(
                          item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                          .First().icdoQdroCalculationDetail.iblnIsNewRecord = false;
                    }
                    //iblnNewIAPRecord = false;
                }

                #endregion
            }
            else if (this.icdoQdroCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
            {

                #region Setup Detail Record for IAPs Estimate

                // Create one Detail Record for IAP Plan 
                ldtVestedDate = DateTime.MinValue;
                busQdroCalculationDetail lbusQdroCalculationDetailIAP = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };

                lbusQdroCalculationDetailIAP.iclbQdroCalculationOptions = new Collection<busQdroCalculationOptions>();
                lbusQdroCalculationDetailIAP.iobjMainCDO = lbusQdroCalculationDetailIAP.icdoQdroCalculationDetail;

                //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                {
                    ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    iblnIsParticipantVestedinIAP = true;
                }

                if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0 &&
                    (!(ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty())).IsNullOrEmpty()))
                {
                    lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                       item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty()).First().icdoDroBenefitDetails.dro_benefit_id;
                }

                if ((iblIsNew && icdoQdroCalculationHeader.qdro_application_id == 0) || (iblIsNew && icdoQdroCalculationHeader.qdro_application_id != 0 && lintDROApplicationDetailId > 0))
                {
                    lbusQdroCalculationDetailIAP.LoadData(this.icdoQdroCalculationHeader.qdro_calculation_header_id, this.icdoQdroCalculationHeader.iintPlanId, busConstant.IAP,
                        this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                        ldtVestedDate, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.istrRetirementSubType, lintDROApplicationDetailId,
                        busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, true);

                    this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetailIAP);
                }
                else if (!iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty())
                {
                    iclbQdroCalculationDetail.Where(
                                               item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.vested_date = ldtVestedDate;
                    iclbQdroCalculationDetail.Where(
                                               item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                    iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.benefit_subtype_value =
                                          this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrRetirementSubType;

                }
                #endregion
            }
            else
            {
                #region Setup Detail Record for Locals

                if (this.ibusBenefitApplication.CheckAlreadyVested(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode))
                {
                    //Abhi-Code Added since if Participant is not NOT-VESTED how can you pay Alternate Payee from that
                    iblnIsParticipantVested = true;

                    if (iblIsNew)
                    {
                        if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0)
                        {
                            lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                               item => item.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId &&
                                   item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().icdoDroBenefitDetails.dro_benefit_id;
                        }

                        busQdroCalculationDetail lbusQdroCalculationDetail = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };
                        lbusQdroCalculationDetail.iobjMainCDO = lbusQdroCalculationDetail.icdoQdroCalculationDetail;
                        lbusQdroCalculationDetail.LoadData(icdoQdroCalculationHeader.qdro_calculation_header_id, icdoQdroCalculationHeader.iintPlanId,
                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode,
                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                            this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrRetirementSubType,
                            lintDROApplicationDetailId, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, true);

                        this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetail);
                    }
                    else
                    {
                        if (this.ibusBenefitApplication.CheckAlreadyVested(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(
                                                                        item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode))
                            iclbQdroCalculationDetail.Where(
                                               item => item.icdoQdroCalculationDetail.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoQdroCalculationDetail.vested_date =
                                               this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                        iclbQdroCalculationDetail.Where(
                                                   item => item.icdoQdroCalculationDetail.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                        iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoQdroCalculationDetail.benefit_subtype_value =
                                          this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().
                                          icdoPersonAccount.istrRetirementSubType;
                    }
                }
                #endregion
            }

            #endregion

            #region SWITCH CASE - INITIATE CALCULATION BASED ON THE REQUIRED PLAN'S ESTIMATE
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                {
                    switch (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode)
                    {
                        case busConstant.Local_161:
                            CalculateLocal161BenefitAmount(busConstant.BOOL_FALSE, busConstant.CodeValueAll);
                            break;

                        case busConstant.Local_52:
                            CalculateLocal52BenefitAmount(busConstant.BOOL_FALSE, busConstant.CodeValueAll);
                            break;

                        case busConstant.Local_600:
                            CalculateLocal600BenefitAmount(busConstant.BOOL_FALSE, busConstant.CodeValueAll);
                            break;

                        case busConstant.Local_666:
                            CalculateLocal666BenefitAmount(busConstant.BOOL_FALSE, busConstant.CodeValueAll);
                            break;

                        case busConstant.LOCAL_700:
                            CalculateLocal700BenefitAmount(busConstant.BOOL_FALSE, busConstant.CodeValueAll);
                            break;
                    }
                }

                switch (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode)
                {

                    case busConstant.MPIPP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                        {
                            this.CalculateFinalBenefitForPension(busConstant.CodeValueAll);
                        }

                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0 &&
                              this.icdoQdroCalculationHeader.is_alt_payee_eligible_for_iap == busConstant.FLAG_YES)
                        {
                            this.CalculateIAPBenefitAmount();
                        }
                        break;

                    case busConstant.IAP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                        {
                            this.CalculateIAPBenefitAmount();
                        }
                        break;
                }
            }
            #endregion
        }

        /// <summary>
        /// Calculate MPIPP Plan Amount
        /// </summary>
        /// <param name="astrBenefitOptionValue"></param>
        protected void CalculateFinalBenefitForPension(string astrBenefitOptionValue)
        {
            decimal ldecBenefitOptionFactor = busConstant.ZERO_DECIMAL;
            decimal ldecFinalAccruedBenefitAmount = busConstant.ZERO_DECIMAL, ldecLateAdjustmentAmt = busConstant.ZERO_DECIMAL;
            string lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().
                                        icdoPersonAccount.istrRetirementSubType;
            busPersonAccount lbusPersonAccount = ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();
            string lstrDROModel = string.Empty;

            if (this.iblnParticipantPayeeAccount)
            {
                this.idecLumpSumBenefitAmount = idecParticipantAmount;
                ldecFinalAccruedBenefitAmount = idecParticipantAmount;

                decimal ldecAltPayeeFraction = 0;
                //Ticket#84882
                if (astrBenefitOptionValue == "All" && icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    astrBenefitOptionValue = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId).FirstOrDefault().icdoDroBenefitDetails.istrBenefitOptionValue;
                }
                switch (astrBenefitOptionValue)
                {
                    case busConstant.CodeValueAll:

                        if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                        {
                            #region Calculate UVHP Amount

                            CalculateUVHPAmount(lbusPersonAccount.icdoPersonAccount.person_account_id);

                            if (lbusPersonAccount.icdoPersonAccount.idecNonVestedEE != 0)
                                CalculateEEAmount(lbusPersonAccount);

                            #endregion
                        }
                        #region MPI Plan Benefit Options

                        if (ldecFinalAccruedBenefitAmount != 0)
                        {
                            
                            busPayeeAccount lbusPayeeAccount = this.iclbParticipantsPayeeAccount.FirstOrDefault();
                            lbusPayeeAccount.LoadBenefitDetails();

                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.MPIPP_PLAN_ID, busConstant.MPIPP,
                                 this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.iblnIsNewRecord);

                             lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.qdro_model_value;

                            //if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.LUMP_SUM)
                            //{
                            //    CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP,
                            //                               busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            //}
                            //else
                            //{
                            //    CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            //    CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            //}
                            //Ticket#84882
                            if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                            {
                                CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                                //CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                                //CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                                CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                                CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            }



                        }

                        #endregion
                        break;


                    case busConstant.LUMP_SUM:
                        if (ldecFinalAccruedBenefitAmount != 0)
                        {
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.MPIPP_PLAN_ID, busConstant.MPIPP, true);

                        }
                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    case busConstant.LIFE_ANNUTIY:

                        if (ldecFinalAccruedBenefitAmount != 0)
                        {
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.MPIPP_PLAN_ID, busConstant.MPIPP, true);
                        }

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                        if (ldecFinalAccruedBenefitAmount != 0)
                        {
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.MPIPP_PLAN_ID, busConstant.MPIPP, true);
                        }

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.MPIPP,
                                                             busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                }


                //Ticket - 61531
                ldecBenefitOptionFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactor(icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement, GetRetirementDateforCalculation().Year),3);
                if (ldecLateAdjustmentAmt == 0)
                {
                    this.idecLumpSumBenefitAmount = Convert.ToDecimal(Math.Ceiling(ldecFinalAccruedBenefitAmount * ldecBenefitOptionFactor));
                }
                else
                {
                    this.idecLumpSumBenefitAmount = Convert.ToDecimal(Math.Ceiling(ldecLateAdjustmentAmt * ldecBenefitOptionFactor));
                }

                #region Code for MEA and MG amount of ALTERNATE PAYEE


                if (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                             item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                             item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).IsNullOrEmpty())
                {
                    CalculateAltPayeeMEAAndMGAmountPostRetirement();
                }

                #endregion
            }
            else
            {


                if ((!iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).IsNullOrEmpty()))
                {
                    if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES && lstrRetirementType != busConstant.RETIREMENT_TYPE_LATE)
                    {
                        #region Ticket#63551 - Commented
                        //busPersonAccount lbusAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).ToList().ToCollection()[0];
                        //ldecFinalAccruedBenefitAmount = ibusCalculation.CalculateUnReducedBenefitAmtForPension(ibusParticipant,
                        //icdoQdroCalculationHeader.age, GetRetirementDateforCalculation(),
                        //lbusAccount,
                        //ibusBenefitApplication, true, null, iclbQdroCalculationDetail, ibusBenefitApplication.aclbPersonWorkHistory_MPI, lbusPersonAccount.icdoPersonAccount.istrRetirementSubType);



                        ////#region Check if Withdrawal History Exists: Then Acrrued benefit = Accrued Benefit - EE derived
                        //busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                        //lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusAccount.icdoPersonAccount.person_account_id);
                        //  Collection<cdoPersonAccountRetirementContribution> lclbRetCont = new Collection<cdoPersonAccountRetirementContribution>();
                        //ldecFinalAccruedBenefitAmount = ibusCalculation.CalculateAccruedBenefitForPersonWithWithdrawal(lbusPersonAccount.icdoPersonAccount.istrRetirementSubType,
                        //                                   lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                        //                                   ldecFinalAccruedBenefitAmount, this.ibusBenefitApplication.ibusPerson, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount, 
                        //                                   GetRetirementDateforCalculation(),
                        //                                   this.ibusBenefitApplication.aclbPersonWorkHistory_MPI, this.ibusBenefitApplication.ibusPerson.iclbPersonAccountRetirementContribution,
                        //                                   lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year,ref lclbRetCont);
                        #endregion Ticket#63551 - Commented

                        //Ticket#63551
                        ldecFinalAccruedBenefitAmount = ibusCalculation.CalculateBenefitAmtForPension(ibusParticipant, busConstant.BENEFIT_TYPE_RETIREMENT, icdoQdroCalculationHeader.age, GetRetirementDateforCalculation(),
                                                                          this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoQdroCalculationDetail.vested_date,
                                                                          lbusPersonAccount,
                                                                          ibusBenefitApplication, busConstant.BOOL_TRUE,
                                                                          null, this.iclbPersonAccountRetirementContribution, iclbQdroCalculationDetail, true, lbusPersonAccount.icdoPersonAccount.istrRetirementSubType, ref ldecLateAdjustmentAmt);

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).
                                                   FirstOrDefault().icdoQdroCalculationDetail.early_reduced_benefit_amount = ldecFinalAccruedBenefitAmount;
                        
                        //#endregion

                        if (iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).FirstOrDefault().icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                        {
                            iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).FirstOrDefault().icdoQdroCalculationDetail.early_reduced_benefit_amount = 0;
                        }
                        else
                        {
                            iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).FirstOrDefault().icdoQdroCalculationDetail.early_reduced_benefit_amount = ldecFinalAccruedBenefitAmount;
                        }
                        ldecFinalAccruedBenefitAmount = GetAccruedBenefitForCalculation(busConstant.MPIPP_PLAN_ID, false, false, false, false);
                    }
                    else
                    {
                        if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES || this.iblnIsParticipantVested)
                        {
                            ldecFinalAccruedBenefitAmount = ibusCalculation.CalculateBenefitAmtForPension(ibusParticipant, busConstant.BENEFIT_TYPE_RETIREMENT, icdoQdroCalculationHeader.age, GetRetirementDateforCalculation(),
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoQdroCalculationDetail.vested_date,
                            lbusPersonAccount,
                            ibusBenefitApplication, busConstant.BOOL_TRUE,
                            null, this.iclbPersonAccountRetirementContribution, iclbQdroCalculationDetail, true, lbusPersonAccount.icdoPersonAccount.istrRetirementSubType, ref ldecLateAdjustmentAmt);

                           

                            //ldecFinalAccruedBenefitAmount = ibusCalculation.CalculateAccruedBenefitForPersonWithWithdrawal(
                            //                                this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                            //                                ldecFinalAccruedBenefitAmount, this.ibusBenefitApplication.ibusPerson, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount,
                            //                                GetRetirementDateforCalculation(), this.ibusBenefitApplication.aclbPersonWorkHistory_MPI,
                            //                                this.ibusBenefitApplication.ibusPerson.iclbPersonAccountRetirementContribution,
                            //                                this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).
                                                       FirstOrDefault().icdoQdroCalculationDetail.early_reduced_benefit_amount = ldecFinalAccruedBenefitAmount;

                            if (iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).
                                            FirstOrDefault().icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                            {
                                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).
                                            FirstOrDefault().icdoQdroCalculationDetail.early_reduced_benefit_amount = 0;
                            }
                            else
                            {
                                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).FirstOrDefault().icdoQdroCalculationDetail.early_reduced_benefit_amount = ldecFinalAccruedBenefitAmount;
                            }

                            ldecFinalAccruedBenefitAmount = GetAccruedBenefitForCalculation(busConstant.MPIPP_PLAN_ID, false, false, false, false);
                        }
                    }
                }

                if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES || this.iblnIsParticipantVested)
                {
                    this.idecLumpSumBenefitAmount = ldecFinalAccruedBenefitAmount;
                    decimal ldecAltPayeeFraction = 0;
                    
                    switch (astrBenefitOptionValue)
                    {
                        case busConstant.CodeValueAll:

                            if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                            {
                                #region Calculate UVHP Amount

                                CalculateUVHPAmount(lbusPersonAccount.icdoPersonAccount.person_account_id);

                                if (lbusPersonAccount.icdoPersonAccount.idecNonVestedEE != 0)
                                    CalculateEEAmount(lbusPersonAccount);

                                #endregion
                            }

                            #region MPI Plan Benefit Options

                            if (ldecFinalAccruedBenefitAmount != 0)
                            {
                                ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.MPIPP_PLAN_ID, busConstant.MPIPP,
                                     this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.iblnIsNewRecord);

                                lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.qdro_model_value;
                                //Ticket#84882
                                if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                                {
                                    CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                                    CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                                    CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                                }
                                else
                                {
                                    foreach (var lQdroApplicationDetails in ibusQdroApplication.iclbDroBenefitDetails)
                                    {
                                        if (lQdroApplicationDetails.icdoDroBenefitDetails.istrBenefitOptionValue == busConstant.LUMP_SUM && lQdroApplicationDetails.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId)
                                        {
                                            CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                                        }
                                        if (lQdroApplicationDetails.icdoDroBenefitDetails.istrBenefitOptionValue == busConstant.LIFE_ANNUTIY && lQdroApplicationDetails.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId)
                                        {
                                            CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                                        }

                                        if (lQdroApplicationDetails.icdoDroBenefitDetails.istrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY && lQdroApplicationDetails.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId)
                                        {
                                            CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                                        }

                                    }

                                }
                                
                                

                                //CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                                //CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            }

                            #endregion

                            break;


                        case busConstant.LUMP_SUM:
                            if (ldecFinalAccruedBenefitAmount != 0)
                            {
                                ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.MPIPP_PLAN_ID, busConstant.MPIPP, true);
                            }

                            lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.qdro_model_value;
                            CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            break;
                        case busConstant.LIFE_ANNUTIY:

                            if (ldecFinalAccruedBenefitAmount != 0)
                            {
                                ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.MPIPP_PLAN_ID, busConstant.MPIPP, true);
                            }

                            lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.qdro_model_value;
                            CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            break;
                        case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                            if (ldecFinalAccruedBenefitAmount != 0)
                            {
                                ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.MPIPP_PLAN_ID, busConstant.MPIPP, true);
                            }

                            lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.qdro_model_value;
                            CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            break;
                    }

                    #region For Vested Participant EE goes along with MEA MG
                    //Ticket - 61531
                    ldecBenefitOptionFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactor(icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement, GetRetirementDateforCalculation().Year),3);
                    if (ldecLateAdjustmentAmt == 0)
                    {
                        this.idecLumpSumBenefitAmount = Convert.ToDecimal(Math.Ceiling(ldecFinalAccruedBenefitAmount * ldecBenefitOptionFactor));
                    }
                    else
                    {
                        this.idecLumpSumBenefitAmount = Convert.ToDecimal(Math.Ceiling(ldecLateAdjustmentAmt * ldecBenefitOptionFactor));
                    }

                    #region Code for MEA and MG amount of ALTERNATE PAYEE


                    if (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                 item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                                 item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).IsNullOrEmpty())
                    {



                        CalculateAltPayeeMEAAmount(ldecFinalAccruedBenefitAmount, lbusPersonAccount,
                                 (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                     item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                                     item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.alt_payee_fraction), lstrDROModel);

                    }


                    #endregion

                    #endregion
                }
                else
                {
                    #region Calculate UVHP Amount

                    CalculateUVHPAmount(lbusPersonAccount.icdoPersonAccount.person_account_id);

                    #endregion

                    #region Calculate EE Amount

                    CalculateEEAmount(lbusPersonAccount);

                    #endregion

                }
            }
        }

        /// <summary>
        /// Calculate MEA and MG amount
        /// </summary>
        /// <param name="adecFinalAccruedBenefitAmount"></param>
        /// <param name="aintPersonAccountId"></param>
        /// <param name="adecAltPayeeFraction"></param>
        private void CalculateAltPayeeMEAAmount(decimal adecFinalAccruedBenefitAmount, busPersonAccount abusPersonAccount, decimal adecAltPayeeFraction, string astrDROModel)
        {
            ibusCalculation.CalculateMEAAndMG(adecFinalAccruedBenefitAmount, abusPersonAccount, this.idecLumpSumBenefitAmount, busConstant.MPIPP_PLAN_ID,
                                                 Convert.ToInt32(Math.Floor(icdoQdroCalculationHeader.age)), GetRetirementDateforCalculation(),
                                                 icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement,
                                                 this.ibusBenefitApplication.QualifiedSpouseExists, true,
                                                 iclbQdroCalculationDetail, null, this.iclbPersonAccountRetirementContribution, icdoQdroCalculationHeader.calculation_type_value);
            decimal ldecVestedEEContribution = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).FirstOrDefault().icdoQdroCalculationDetail.vested_ee_amount;
            decimal ldecVestedMemberEEInterest = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).FirstOrDefault().icdoQdroCalculationDetail.vested_ee_interest;

            if (ldecVestedEEContribution + ldecVestedMemberEEInterest != 0)
            {
                #region Calculate MEA amount

                CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID,
                                            adecAltPayeeFraction, astrDROModel,false, false, false, false, true, false, false, false, true);

                decimal ldecAltPayeeEEContribution = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID
                        && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().
                                icdoQdroCalculationDetail.alt_payee_ee_contribution;

                CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, adecAltPayeeFraction,astrDROModel,
                                                    false, false, false, false, false, true, false, false, true);

                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                         item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.alt_payee_member_exclusion_amount =
                  Math.Round(ldecAltPayeeEEContribution / GetRecoveryMonths(icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement), 2);

                //Calcualte Participant MEA share
                decimal ldecParticipantEEShare = ldecVestedEEContribution - ldecAltPayeeEEContribution;
                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                          item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.member_exclusion_amount =
                   Math.Round(ldecParticipantEEShare / GetRecoveryMonths(Convert.ToInt32(icdoQdroCalculationHeader.age)), 2);

                #endregion

                #region Calculate MGA

                CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID,
                                           adecAltPayeeFraction,astrDROModel, false, false, false, false, false, false, false, true);

                decimal ldecAltPayeeMGAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                           item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.alt_payee_minimum_guarantee_amount;

                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                          item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.minimum_guarantee_amount =
                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                          item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.minimum_guarantee_amount - ldecAltPayeeMGAmount;

                #endregion
            }
        }

        private void CalculateAltPayeeMEAAndMGAmountPostRetirement()
        {
            decimal ldecParticipantsBenefitAmountPaid = 0;

            if (!this.iclbQdroCalculationDetail.IsNullOrEmpty())
            {

                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && !(item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                    && !(item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES)).Count() > 0)
                {
                    busQdroCalculationDetail lbusQDRoCal = iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag != busConstant.FLAG_YES && item.icdoQdroCalculationDetail.uvhp_flag != busConstant.FLAG_YES).FirstOrDefault();
                    decimal ldecLifeAnnuityAmount = lbusQDRoCal.icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                    decimal ldecLifeConversionFactor = decimal.One;
                    if (this.icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                    {
                        ldecLifeConversionFactor = lbusQDRoCal.iclbQdroCalculationOptions.FirstOrDefault().icdoQdroCalculationOptions.life_conversion_factor;
                        ldecLifeAnnuityAmount = Math.Round(ldecLifeAnnuityAmount * ldecLifeConversionFactor, 2);
                        idecAlternatepayeeBenefitFraction = Math.Round(ldecLifeAnnuityAmount / idecParticipantAmount, 3);
                    }
                    else if (this.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                    {
                        int lintPlanBenefitID = ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE_ANNUTIY);
                        if (!lbusQDRoCal.iclbQdroCalculationOptions.IsNullOrEmpty())
                        {
                            if (lbusQDRoCal.iclbQdroCalculationOptions.Where(item => item.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitID).Count() > 0)
                            {
                                idecAlternatepayeeBenefitFraction = Math.Round(lbusQDRoCal.iclbQdroCalculationOptions.Where(item => item.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitID).FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount / idecParticipantAmount, 3);
                            }
                            else
                            {
                                idecAlternatepayeeBenefitFraction = Math.Round(lbusQDRoCal.iclbQdroCalculationOptions.FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount / idecParticipantAmount, 3);

                            }
                        }
                    }
                    busDroBenefitDetails lbusDroBenefitDetails = new busDroBenefitDetails { icdoDroBenefitDetails = new cdoDroBenefitDetails() };
                    if (this.iclbQdroCalculationDetail != null && this.iclbQdroCalculationDetail.Count > 0)
                    {
                        lbusDroBenefitDetails.FindDroBenefitDetails(this.iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.qdro_application_detail_id);
                    }

                    decimal ldecRemainingMinimumGuaranteeAmount = decimal.Zero;
                    if (!this.iclbParticipantsPayeeAccount.IsNullOrEmpty())
                    {
                        this.iclbParticipantsPayeeAccount.FirstOrDefault().LoadNextBenefitPaymentDate();

                        DataTable ldtblParticipantsBenefitAmount = Select("cdoRepaymentSchedule.GetBenefitAmount", new object[2] { this.iclbParticipantsPayeeAccount.FirstOrDefault().icdoPayeeAccount.payee_account_id,
                                                    this.iclbParticipantsPayeeAccount.FirstOrDefault().idtNextBenefitPaymentDate});

                        if (ldtblParticipantsBenefitAmount.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(ldtblParticipantsBenefitAmount.Rows[0][0])))
                        {
                            ldecParticipantsBenefitAmountPaid = Convert.ToDecimal(ldtblParticipantsBenefitAmount.Rows[0][0]);
                        }
                        if (this.ibusQdroApplication.IsNotNull() && this.ibusQdroApplication.icdoDroApplication.IsNotNull() && this.ibusQdroApplication.icdoDroApplication.dro_commencement_date != DateTime.MinValue)
                        {
                            ldecRemainingMinimumGuaranteeAmount = this.iclbParticipantsPayeeAccount.FirstOrDefault().GetRemainingMinimumGuaranteeTillDate(this.ibusQdroApplication.icdoDroApplication.dro_commencement_date.AddDays(-1));
                            if (ldecRemainingMinimumGuaranteeAmount == decimal.Zero)
                            {
                                ldecRemainingMinimumGuaranteeAmount = this.iclbParticipantsPayeeAccount.FirstOrDefault().GetRemainingNonTaxableBeginningBalanaceTillDate(this.ibusQdroApplication.icdoDroApplication.dro_commencement_date.AddDays(-1));
                            }
                        }
                        else
                        {
                            ldecRemainingMinimumGuaranteeAmount = this.iclbParticipantsPayeeAccount.FirstOrDefault().GetRemainingMinimumGuaranteeTillDate(GetRetirementDateforCalculation().AddDays(-1));
                            if (ldecRemainingMinimumGuaranteeAmount == decimal.Zero)
                            {
                                ldecRemainingMinimumGuaranteeAmount = this.iclbParticipantsPayeeAccount.FirstOrDefault().GetRemainingNonTaxableBeginningBalanaceTillDate(GetRetirementDateforCalculation().AddDays(-1));
                            }
                        }
                    }
                    if (idecAlternatepayeeBenefitFraction != 0 && ldecParticipantsBenefitAmountPaid != 0)
                    {

                        //alt_payee_minimum_guarantee_amount
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                               item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.alt_payee_minimum_guarantee_amount =
                                     (ldecRemainingMinimumGuaranteeAmount * idecAlternatepayeeBenefitFraction);

                        //minimum_guarantee_amount
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                             item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.minimum_guarantee_amount =
                                  ldecRemainingMinimumGuaranteeAmount - this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                               item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.alt_payee_minimum_guarantee_amount;

                        if (this.idecParticipantMea > decimal.Zero)
                        {
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                               item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.alt_payee_member_exclusion_amount =
                                     Convert.ToDecimal(this.idecParticipantMea * idecAlternatepayeeBenefitFraction);
                            //member_exclusion_amount
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                           item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.member_exclusion_amount =
                                 this.idecParticipantMea - this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.alt_payee_member_exclusion_amount;
                        }
                        else
                        {
                            DataTable ldtblParticipantsMEA = Select("cdoPayeeAccount.GetParticipantsMEA", new object[1] { this.iclbParticipantsPayeeAccount.FirstOrDefault().icdoPayeeAccount.payee_account_id });
                            if (ldtblParticipantsMEA.Rows.Count > 0 && Convert.ToString(ldtblParticipantsMEA.Rows[0][0]).IsNotNullOrEmpty())
                            {
                                //alt_payee_member_exclusion_amount
                                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.alt_payee_member_exclusion_amount =
                                      (Convert.ToDecimal(ldtblParticipantsMEA.Rows[0][0]) * idecAlternatepayeeBenefitFraction);
                                //member_exclusion_amount
                                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                               item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.member_exclusion_amount =
                                     Convert.ToDecimal(ldtblParticipantsMEA.Rows[0][0]) - this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.alt_payee_member_exclusion_amount;
                            }
                        }
                    }

                }
            }
        }

        private int GetRecoveryMonths(int adecAge)
        {
            if (adecAge <= 55)
            {
                return 360;
            }
            else if (adecAge <= 60)
            {
                return 310;
            }
            else if (adecAge <= 65)
            {
                return 260;
            }
            else if (adecAge <= 70)
            {
                return 210;
            }
            else
            {
                return 160;
            }
        
        }

        private void CalculateQDROFactorAndAmount(string astrBenefitOptions, string astrPlanCode, int aintPlanId, decimal adecAltPayeeFraction, string astrDROModel,
                                                    bool ablnNonVestedEEFlag = false, bool ablnUVHPFlag = false,
                                                    bool ablnL52SpecialAccountFlag = false, bool ablnL161SpecialAccountFlag = false, bool ablnAmtFlag = false,
                                                    bool ablnInterestFlag = false, bool ablnCalculateBenefitOptions = true, bool ablnMGACalculation = false,
                                                    bool ablnMEACalculation = false)
        {

            decimal ldecLifeAnnuityForAlternatePayee = busConstant.ZERO_DECIMAL, ldecLifeConversionFactor = 1, ldecEarlyReductionFactor = 1;


            busQdroCalculationOptions lbusQdroCalculationOptions = new busQdroCalculationOptions { icdoQdroCalculationOptions = new cdoQdroCalculationOptions() };

            #region Calculate Alt Payee Benefit Before conversion

            decimal ldecBenefitBeforeConversion = CalculateAltPayeeBenefitBeforeConversion(ablnNonVestedEEFlag, ablnUVHPFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag,
                                                    ablnAmtFlag, ablnInterestFlag, aintPlanId, adecAltPayeeFraction, ablnMGACalculation, ablnMEACalculation);

            #endregion

            #region Calculate Life Conversion Factor


            if (astrDROModel != busConstant.DRO_MODEL_VALUE_CHILD_SUPPORT && astrDROModel != busConstant.DRO_MODEL_VALUE_SPOUSAL_SUPPORT &&
            !this.iblnParticipantPayeeAccount && ((icdoQdroCalculationHeader.qdro_application_id != 0 && ibusQdroApplication.icdoDroApplication.life_conversion_factor_flag == busConstant.FLAG_YES) ||
                (icdoQdroCalculationHeader.qdro_application_id == 0 && icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)))
            {
                if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES)
                    ldecLifeConversionFactor = ibusCalculation.CalculateLifeConversionFactor(Convert.ToInt32(Math.Floor(icdoQdroCalculationHeader.age)), Convert.ToInt32(icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement), true);
                else
                    ldecLifeConversionFactor = ibusCalculation.CalculateLifeConversionFactor(Convert.ToInt32(Math.Floor(icdoQdroCalculationHeader.age)), Convert.ToInt32(icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement), false);
            }

            #endregion

            #region Calculate Participant's benefit amount

            decimal ldecParticipantBenefitAmt = CalculateParticipantBenefitAmount(ablnNonVestedEEFlag, ablnUVHPFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag, aintPlanId, ldecBenefitBeforeConversion,
                                                  ablnMGACalculation, ablnMEACalculation);

            #endregion

            if (ablnCalculateBenefitOptions)
            {
                #region Calculate Life Annuity Amount of Alternate Payee

                ldecLifeAnnuityForAlternatePayee = ldecBenefitBeforeConversion * ldecLifeConversionFactor;

                #endregion

                #region Apply Benefit Options and find Alternate Payee's Portion

                icdoQdroCalculationHeader.idecAlternatePayeeAgeAtRetirement = icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement;

                if (!this.iblnParticipantPayeeAccount && icdoQdroCalculationHeader.is_participant_disabled != busConstant.FLAG_YES)
                    ldecEarlyReductionFactor = GetEarlyReductionFactor(aintPlanId, ablnNonVestedEEFlag, ablnUVHPFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag);


                switch (astrBenefitOptions)
                {
                    case busConstant.LIFE_ANNUTIY:
                        lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.LIFE_ANNUTIY);

                        #region Check Flags and Load
                        if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag)
                        {
                            decimal ldecLifeAnnuityAmount = Math.Round((ldecEarlyReductionFactor != 0)? ldecLifeAnnuityForAlternatePayee * ldecEarlyReductionFactor: ldecLifeAnnuityForAlternatePayee, 2);
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LIFE_ANNUTIY), 1,//ldecLifeConversionFactor,
                              ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id,
                              ldecLifeAnnuityAmount, adecLifeConversionFactor: ldecLifeConversionFactor, adecEarlyReductionfactor: ldecEarlyReductionFactor);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }

                        if (ablnNonVestedEEFlag && ldecBenefitBeforeConversion > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LIFE_ANNUTIY),1,// ldecLifeConversionFactor,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLifeAnnuityForAlternatePayee, true
                                                                , adecLifeConversionFactor: ldecLifeConversionFactor);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }
                        if (ablnUVHPFlag && ldecBenefitBeforeConversion > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LIFE_ANNUTIY), 1,//ldecLifeConversionFactor,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLifeAnnuityForAlternatePayee, false, true,
                                                                 adecLifeConversionFactor: ldecLifeConversionFactor);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                        && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }

                        #endregion

                        break;
                        
                    case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                        //Ticket#68161
                        decimal ldecTenYearCertainFactor = 0.00m;
                        if (astrPlanCode.Contains("Local") && astrPlanCode != busConstant.LOCAL_700)
                        {
                            ldecTenYearCertainFactor = 1.0000m;
                        }
                        else
                        {
                            ldecTenYearCertainFactor = ibusCalculation.GetQDROFactor(Convert.ToInt32(icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement), busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, false);
                         }
                        

                        lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);

                        #region Check Flags and Load

                        if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag)
                        {
                            //Ticket#84882
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), ldecTenYearCertainFactor, 
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, (ldecEarlyReductionFactor != 0)? ldecLifeAnnuityForAlternatePayee * ldecEarlyReductionFactor * ldecTenYearCertainFactor: ldecLifeAnnuityForAlternatePayee * ldecTenYearCertainFactor,
                                                                 adecLifeConversionFactor: ldecLifeConversionFactor, adecEarlyReductionfactor: ldecEarlyReductionFactor);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }

                        if (ablnNonVestedEEFlag)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), ldecTenYearCertainFactor,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLifeAnnuityForAlternatePayee * ldecTenYearCertainFactor, true,
                                                                 adecLifeConversionFactor: ldecLifeConversionFactor);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }
                        if (ablnUVHPFlag)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), ldecTenYearCertainFactor,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLifeAnnuityForAlternatePayee * ldecTenYearCertainFactor, false, true,
                                                                 adecLifeConversionFactor: ldecLifeConversionFactor);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                    && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }

                        #endregion

                        break;

                    case busConstant.LUMP_SUM:
                        lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.LUMP_SUM);

                        decimal ldecLumpSumFactor = 1;

                        if (astrDROModel != busConstant.DRO_MODEL_VALUE_CHILD_SUPPORT && astrDROModel != busConstant.DRO_MODEL_VALUE_SPOUSAL_SUPPORT)
                        {
                           ldecLumpSumFactor = ibusCalculation.GetLumpsumBenefitFactor(Convert.ToInt32(icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement), GetRetirementDateforCalculation().Year);
                           ldecLumpSumFactor = Math.Round(ldecLumpSumFactor * 12, 3);
                        }

                        #region Check Flags and Load

                        if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag)
                        {
                            //Ticket#84882
                            decimal ldecLifeAnnuityAmount = Math.Round((ldecEarlyReductionFactor != 0) ? ldecLifeAnnuityForAlternatePayee *  ldecEarlyReductionFactor: ldecLifeAnnuityForAlternatePayee, 2);
                            decimal ldecLumpSumAmount = Math.Round(ldecLifeAnnuityAmount * ldecLumpSumFactor, 2);
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LUMP_SUM), ldecLumpSumFactor,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id,
                                                                ldecLumpSumAmount,
                                                                 adecLifeConversionFactor: ldecLifeConversionFactor, adecEarlyReductionfactor: ldecEarlyReductionFactor);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }

                        if (ablnNonVestedEEFlag)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LUMP_SUM), 1,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id,
                                                                 ldecBenefitBeforeConversion, true);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                        && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }
                        if (ablnUVHPFlag)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.LUMP_SUM), 1,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id,
                                                                 ldecBenefitBeforeConversion, false, true);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                    && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }

                        #endregion

                        break;

                    case busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY:

                        //Ticket#68161
                        decimal ldecFiveYearCertainFactor = 0.00m;
                        if (astrPlanCode.Contains("Local"))
                        {
                            ldecFiveYearCertainFactor = 1.0000m;
                        }
                        else
                        {
                            ldecFiveYearCertainFactor = ibusCalculation.GetQDROFactor(Convert.ToInt32(icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement), busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY, false);
                        }
                                             

                        lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY);

                        #region Check Flags and Load

                        if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY), ldecFiveYearCertainFactor,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, (ldecEarlyReductionFactor != 0) ? ldecLifeAnnuityForAlternatePayee * ldecEarlyReductionFactor * ldecFiveYearCertainFactor: ldecLifeAnnuityForAlternatePayee * ldecFiveYearCertainFactor,
                                                                 adecLifeConversionFactor: ldecLifeConversionFactor, adecEarlyReductionfactor: ldecEarlyReductionFactor);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }

                        if (ablnNonVestedEEFlag)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY), ldecFiveYearCertainFactor,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLifeAnnuityForAlternatePayee * ldecFiveYearCertainFactor, true,
                                                                 adecLifeConversionFactor: ldecLifeConversionFactor);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }
                        if (ablnUVHPFlag)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY), ldecFiveYearCertainFactor,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLifeAnnuityForAlternatePayee * ldecFiveYearCertainFactor, false, true,
                                                                 adecLifeConversionFactor: ldecLifeConversionFactor);
                                                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                    && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }

                        #endregion

                        break;

                    //PIR 554
                    case busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY:
                        if (astrPlanCode == busConstant.Local_666)
                        {
                            //Ticket#68161
                            decimal ldecThreeYearCertainFactor = 1.0000m;
                           // decimal ldecThreeYearCertainFactor = ibusCalculation.GetQDROFactor(Convert.ToInt32(icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement), busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY, false);
                            lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY);

                            #region Check Flags and Load

                            if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag)
                            {
                                //Ticket#84882
                                decimal ldecThreeYearCertainAmount = (ldecEarlyReductionFactor != 0) ? ldecLifeAnnuityForAlternatePayee * ldecEarlyReductionFactor * ldecThreeYearCertainFactor: ldecLifeAnnuityForAlternatePayee * ldecThreeYearCertainFactor;

                                //It is rounded to the next higher $0.50 multiple for the remainder of her lifetime
                                ldecThreeYearCertainAmount = Math.Ceiling(ldecThreeYearCertainAmount * 2) / 2;

                                lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY), ldecThreeYearCertainFactor,
                                                                    ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecThreeYearCertainAmount,
                                                                     adecLifeConversionFactor: ldecLifeConversionFactor, adecEarlyReductionfactor: ldecEarlyReductionFactor);

                                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                            }

                            if (ablnNonVestedEEFlag)
                            {
                                lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY), ldecThreeYearCertainFactor,
                                                                    ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLifeAnnuityForAlternatePayee * ldecThreeYearCertainFactor, true,
                                                                     adecLifeConversionFactor: ldecLifeConversionFactor);

                                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                    && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                            }
                            if (ablnUVHPFlag)
                            {
                                lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY), ldecThreeYearCertainFactor,
                                                                    ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLifeAnnuityForAlternatePayee * ldecThreeYearCertainFactor, false, true,
                                                                     adecLifeConversionFactor: ldecLifeConversionFactor);

                                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                        && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                            }

                            #endregion
                        }
                        break;


                    case busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY:

                        //Ticket#68161
                        decimal ldecTwoYearCertainFactor = 0.00m;
                        if (astrPlanCode.Contains("Local")&& astrPlanCode != busConstant.LOCAL_700)
                        {
                            ldecTwoYearCertainFactor = 1.0000m;
                        }
                        else
                        {
                            ldecTwoYearCertainFactor = ibusCalculation.GetQDROFactor(Convert.ToInt32(icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement), busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, false);
                        }


                        lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY);

                        #region Check Flags and Load

                        if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY), ldecTwoYearCertainFactor,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, (ldecEarlyReductionFactor != 0)? ldecLifeAnnuityForAlternatePayee * ldecEarlyReductionFactor * ldecTwoYearCertainFactor: ldecLifeAnnuityForAlternatePayee * ldecTwoYearCertainFactor,
                                                                 adecLifeConversionFactor: ldecLifeConversionFactor, adecEarlyReductionfactor: ldecEarlyReductionFactor);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }

                        if (ablnNonVestedEEFlag)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY), ldecTwoYearCertainFactor,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLifeAnnuityForAlternatePayee * ldecTwoYearCertainFactor, true,
                                                                 adecLifeConversionFactor: ldecLifeConversionFactor);

                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }
                        if (ablnUVHPFlag)
                        {
                            lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(aintPlanId, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY), ldecTwoYearCertainFactor,
                                                                ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLifeAnnuityForAlternatePayee * ldecTwoYearCertainFactor, false, true,
                                                                 adecLifeConversionFactor: ldecLifeConversionFactor);
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                        && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                        }

                        #endregion

                        break;
                }

                #endregion
            }
        }

        /// <summary>
        /// get Early reduction factor. This is not applicable for sub plans
        /// </summary>
        /// <param name="aintPlanID"></param>
        /// <param name="ablnNonVestedEEFlag"></param>
        /// <param name="ablnUVHPFlag"></param>
        /// <param name="ablnL52SpecialAccountFlag"></param>
        /// <param name="ablnL161SpecialAccountFlag"></param>
        /// <returns></returns>
        private decimal GetEarlyReductionFactor(int aintPlanID, bool ablnNonVestedEEFlag, bool ablnUVHPFlag,
                                                    bool ablnL52SpecialAccountFlag, bool ablnL161SpecialAccountFlag)
        {
            if (!ablnL161SpecialAccountFlag && !ablnL52SpecialAccountFlag && !ablnNonVestedEEFlag && !ablnUVHPFlag && (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanID &&
                        item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                        item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty()))
            {
                return this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanID &&
                        item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                        item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.early_reduction_factor;
            }

            return 1;
        }

        private decimal CalculateAltPayeeBenefitBeforeConversion(bool ablnNonVestedEEFlag, bool ablnUVHPFlag, bool ablnL52SpecialAccountFlag, bool ablnL161SpecialAccountFlag,
                                                                 bool ablnAmtFlag, bool ablnInterestFlag, int aintPlanId, decimal adecAltPayeeFraction, bool ablnMGACalculation, bool ablnMEACalculation)
        {
            decimal ldecBenefitBeforeConversion = 0, ldecflatAmount = 0, ldecFlatPercent = 0;

            if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag &&
                (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                    && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty()))
            {
                ldecflatAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                    && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.flat_amount;

                ldecFlatPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                    && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.flat_percent;

                ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(GetAccruedBenefitForCalculation(aintPlanId, false, false, false, false), adecAltPayeeFraction,
                                                 ldecflatAmount, ldecFlatPercent);

                if (!ablnMEACalculation && !ablnMGACalculation)
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                                icdoQdroCalculationDetail.alt_payee_amt_before_conversion = ldecBenefitBeforeConversion;

                if (ablnAmtFlag)
                {
                    ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(
                                                 this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.vested_ee_amount,
                                                 adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().
                                icdoQdroCalculationDetail.alt_payee_ee_contribution = ldecBenefitBeforeConversion;

                    if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.non_vested_ee_amount != 0)
                    {
                        ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(
                                                 this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.non_vested_ee_amount,
                                                 adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().
                                icdoQdroCalculationDetail.non_vested_altpayee_ee_amount = ldecBenefitBeforeConversion;
                    }
                }
                else if (ablnInterestFlag)
                {

                    ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(
                                             this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                             item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().
                                             icdoQdroCalculationDetail.vested_ee_interest, adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                         item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().
                           icdoQdroCalculationDetail.alt_payee_interest_amount = ldecBenefitBeforeConversion;

                    if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.non_vested_ee_interest != 0)
                    {
                        ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(
                                                 this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.non_vested_ee_interest,
                                                 adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().
                                icdoQdroCalculationDetail.non_vested_altpayee_ee_interest = ldecBenefitBeforeConversion;
                    }
                }
                else if (ablnMGACalculation)
                {
                    ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(
                                             this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                             item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().
                                             icdoQdroCalculationDetail.vested_ee_interest +
                                             this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                             item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.vested_ee_amount,
                                            adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                         item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().
                           icdoQdroCalculationDetail.alt_payee_minimum_guarantee_amount = ldecBenefitBeforeConversion;
                }
            }
            else if (ablnNonVestedEEFlag && (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                   && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
            {
                ldecflatAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                   && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_amount;

                ldecFlatPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                   && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_percent;

                if (ablnAmtFlag)
                {
                    if (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).IsNullOrEmpty())
                    {
                        ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(
                                              this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                              item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.non_vested_ee_amount,
                                              adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                                icdoQdroCalculationDetail.alt_payee_ee_contribution = ldecBenefitBeforeConversion;
                    }
                }
                else if (ablnInterestFlag)
                {
                    if (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                          item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).IsNullOrEmpty())
                    {
                        ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(
                                              this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                              item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.non_vested_ee_interest,
                                              adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                               icdoQdroCalculationDetail.alt_payee_interest_amount = ldecBenefitBeforeConversion;
                    }
                }
                //else
                //{
                ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(
                                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                            item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.early_reduced_benefit_amount,
                                            adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.alt_payee_amt_before_conversion = ldecBenefitBeforeConversion;
                //}
            }
            else if (ablnUVHPFlag)
            {
                ldecflatAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                  && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_amount;

                ldecFlatPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                   && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_percent;

                if (ablnAmtFlag)
                {
                    ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(GetUVHPContributionAmount(),
                                          adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.alt_payee_uvhp = ldecBenefitBeforeConversion;
                }
                else if (ablnInterestFlag)
                {
                    ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(GetUVHPInterestAmount(),
                                          adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                           icdoQdroCalculationDetail.alt_payee_uvhp_interest = ldecBenefitBeforeConversion;
                }
                //else
                //{
                ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(GetUVHPContributionAmount() + GetUVHPInterestAmount(),
                                           adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.alt_payee_amt_before_conversion = ldecBenefitBeforeConversion;
                // }
            }
            else if (ablnL161SpecialAccountFlag)
            {
                ldecflatAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                  && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_amount;

                ldecFlatPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                   && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_percent;

                ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(GetAccruedBenefitForCalculation(aintPlanId, false, false, false, true),
                                            adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.alt_payee_amt_before_conversion = ldecBenefitBeforeConversion;
            }
            else if (ablnL52SpecialAccountFlag)
            {
                ldecflatAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                  && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_amount;

                ldecFlatPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                   && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_percent;

                ldecBenefitBeforeConversion = ibusCalculation.CalculateBenefitAmtBeforeConversion(GetAccruedBenefitForCalculation(aintPlanId, ablnUVHPFlag, ablnNonVestedEEFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag),
                                            adecAltPayeeFraction, ldecflatAmount, ldecFlatPercent);

                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.alt_payee_amt_before_conversion = ldecBenefitBeforeConversion;
            }

            return ldecBenefitBeforeConversion;
        }

        private decimal CalculateParticipantBenefitAmount(bool ablnNonVestedEEFlag, bool ablnUVHPFlag, bool ablnL52SpecialAccountFlag, bool ablnL161SpecialAccountFlag, int aintPlanId,
                                                            decimal adecBenefitBeforeConversion, bool ablnMGACalculation, bool ablnMEACalculation)
        {
            decimal ldecParticipantBenefitAmt = 0;

            if (this.iblnParticipantPayeeAccount)
            {
                if (this.iclbQdroCalculationDetail != null && this.iclbQdroCalculationDetail.Count > 0)
                {
                    if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag && !ablnMEACalculation && !ablnMGACalculation)
                    {
                        ldecParticipantBenefitAmt = this.idecParticipantAmount - adecBenefitBeforeConversion;

                        if (ldecParticipantBenefitAmt != 0)
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.participant_benefit_amount = ldecParticipantBenefitAmt;
                    }

                }
                return ldecParticipantBenefitAmt;
            }

            if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag && !ablnMEACalculation && !ablnMGACalculation)
            {
                ldecParticipantBenefitAmt = GetAccruedBenefitForCalculation(aintPlanId, ablnUVHPFlag, ablnNonVestedEEFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag) - adecBenefitBeforeConversion;

                if (ldecParticipantBenefitAmt != 0)
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.participant_benefit_amount = ldecParticipantBenefitAmt;
                else
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                               item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                               item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.participant_benefit_amount = 0;
            }
            else if (ablnNonVestedEEFlag)
            {
                ldecParticipantBenefitAmt = GetAccruedBenefitForCalculation(aintPlanId, ablnUVHPFlag, ablnNonVestedEEFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag) - adecBenefitBeforeConversion;

                if (ldecParticipantBenefitAmt != 0)
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                       && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.participant_benefit_amount = ldecParticipantBenefitAmt;
                else
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                   && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.participant_benefit_amount = 0;

            }
            else if (ablnUVHPFlag)
            {
                ldecParticipantBenefitAmt = GetTotalUVHPAmountForCalculation(this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                       && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.person_account_id) - adecBenefitBeforeConversion;//GetAccruedBenefitForCalculation(aintPlanId, ablnUVHPFlag, ablnNonVestedEEFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag) - adecBenefitBeforeConversion;

                if (ldecParticipantBenefitAmt != 0)
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                       && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.participant_benefit_amount = ldecParticipantBenefitAmt;
                else
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                  && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.participant_benefit_amount = 0;
            }
            else if (ablnL161SpecialAccountFlag)
            {
                ldecParticipantBenefitAmt = GetAccruedBenefitForCalculation(aintPlanId, ablnUVHPFlag, ablnNonVestedEEFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag) - adecBenefitBeforeConversion;

                if (ldecParticipantBenefitAmt != 0)
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                        && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.participant_benefit_amount = ldecParticipantBenefitAmt;
                else
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                    && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.participant_benefit_amount = 0;
            }
            else if (ablnL52SpecialAccountFlag)
            {
                ldecParticipantBenefitAmt = GetAccruedBenefitForCalculation(aintPlanId, ablnUVHPFlag, ablnNonVestedEEFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag) - adecBenefitBeforeConversion;
                if (ldecParticipantBenefitAmt != 0)
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                        && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.participant_benefit_amount = ldecParticipantBenefitAmt;
                else
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.participant_benefit_amount = 0;
            }

            return ldecParticipantBenefitAmt;
        }

        private decimal CalculateAltPayeeFraction(int aintPlanId, string astrPlanCode, bool ablnNewRecord, bool ablnNonVestedEEFlag = false, bool ablnUVHPFlag = false,
                                                  bool ablnL52SpecialAccountFlag = false, bool ablnL161SpecialAccountFlag = false)
        {
            decimal ldecTotalHoursWorked = busConstant.ZERO_DECIMAL, ldecHoursWorkedBetweenDates = busConstant.ZERO_DECIMAL, ldecAltPayeeFraction = busConstant.ZERO_DECIMAL, ldecFlatAmount = busConstant.ZERO_DECIMAL,
                    ldecQdroPercent = busConstant.ZERO_DECIMAL, ldecFlatPercent = busConstant.ZERO_DECIMAL;

            if (((iblIsNew || ablnNewRecord) && icdoQdroCalculationHeader.qdro_application_id != 0 &&
                        icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE) ||
                        icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                        icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
            {
                if (ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0)
                {
                    if (ablnUVHPFlag && (!ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId &&
                                            item.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
                    {
                        ldecFlatAmount = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_amt;
                        ldecQdroPercent = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_perc;
                        ldecFlatPercent = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_flat_perc;

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_model_value =
                            ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoDroBenefitDetails.dro_model_value;

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_amount = ldecFlatAmount;
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_percent = ldecFlatPercent;
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_percent = ldecQdroPercent;
                    }
                    else if (ablnNonVestedEEFlag && (!ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId &&
                                    item.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
                    {
                        ldecFlatAmount = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_amt;
                        ldecQdroPercent = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_perc;
                        ldecFlatPercent = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_flat_perc;

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_model_value =
                            ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES).FirstOrDefault().icdoDroBenefitDetails.dro_model_value;

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_amount = ldecFlatAmount;
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_percent = ldecFlatPercent;
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_percent = ldecQdroPercent;
                    }
                    else if (ablnL52SpecialAccountFlag && !ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId
                                && item.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty())
                    {
                        ldecFlatAmount = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_amt;
                        ldecQdroPercent = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_perc;
                        ldecFlatPercent = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_flat_perc;

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_model_value =
                            ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoDroBenefitDetails.dro_model_value;

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_amount = ldecFlatAmount;
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_percent = ldecFlatPercent;
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_percent = ldecQdroPercent;
                    }
                    else if (ablnL161SpecialAccountFlag && (!ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId &&
                                item.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
                    {
                        ldecFlatAmount = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_amt;
                        ldecQdroPercent = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_perc;
                        ldecFlatPercent = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.benefit_flat_perc;

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_model_value =
                            ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoDroBenefitDetails.dro_model_value;

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_amount = ldecFlatAmount;
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.flat_percent = ldecFlatPercent;
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_percent = ldecQdroPercent;
                    }
                    else if (!ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.istrSubPlan.IsNullOrEmpty()).IsNullOrEmpty())
                    {
                        ldecFlatAmount = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.istrSubPlan.IsNullOrEmpty()).First().icdoDroBenefitDetails.benefit_amt;
                        ldecQdroPercent = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.istrSubPlan.IsNullOrEmpty()).First().icdoDroBenefitDetails.benefit_perc;
                        ldecFlatPercent = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.istrSubPlan.IsNullOrEmpty()).First().icdoDroBenefitDetails.benefit_flat_perc;

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.adjustment_amt =
                           ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.istrSubPlan.IsNullOrEmpty()).FirstOrDefault().icdoDroBenefitDetails.adjustment_amt;

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.qdro_model_value =
                           ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == aintPlanId && item.istrSubPlan.IsNullOrEmpty()).FirstOrDefault().icdoDroBenefitDetails.dro_model_value;

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.flat_amount = ldecFlatAmount;
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.flat_percent = ldecFlatPercent;
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.qdro_percent = ldecQdroPercent;
                    }
                }
            }
            else if ((iblIsNew || ablnNewRecord) && icdoQdroCalculationHeader.qdro_application_id == 0)
            {
                ldecQdroPercent = busConstant.DEFAULT_QDRO_PERCENT;

                if (ablnUVHPFlag &&
                    (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                         icdoQdroCalculationDetail.qdro_percent = ldecQdroPercent;
                }
                else if (ablnNonVestedEEFlag &&
                    (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                        icdoQdroCalculationDetail.qdro_percent = ldecQdroPercent;
                }
                else if (ablnL161SpecialAccountFlag &&
                    (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                       icdoQdroCalculationDetail.qdro_percent = ldecQdroPercent;
                }
                else if (ablnL52SpecialAccountFlag &&
                    (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                       icdoQdroCalculationDetail.qdro_percent = ldecQdroPercent;
                }
                else if (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).IsNullOrEmpty())
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().
                      icdoQdroCalculationDetail.qdro_percent = ldecQdroPercent;
                }
            }
            else if (!iblIsNew && ablnUVHPFlag)
            {
                ldecFlatAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                                    icdoQdroCalculationDetail.flat_amount;
                ldecFlatPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                        icdoQdroCalculationDetail.flat_percent;
                ldecQdroPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                        icdoQdroCalculationDetail.qdro_percent;
            }
            else if (!iblIsNew && ablnNonVestedEEFlag)
            {
                ldecFlatAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                                    icdoQdroCalculationDetail.flat_amount;
                ldecFlatPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                        icdoQdroCalculationDetail.flat_percent;
                ldecQdroPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                        icdoQdroCalculationDetail.qdro_percent;
            }
            else if (!iblIsNew && ablnL161SpecialAccountFlag)
            {
                ldecFlatAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                                    icdoQdroCalculationDetail.flat_amount;
                ldecFlatPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                        icdoQdroCalculationDetail.flat_percent;
                ldecQdroPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                        icdoQdroCalculationDetail.qdro_percent;
            }
            else if (!iblIsNew && ablnL52SpecialAccountFlag)
            {
                ldecFlatAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                                   icdoQdroCalculationDetail.flat_amount;
                ldecFlatPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                        icdoQdroCalculationDetail.flat_percent;
                ldecQdroPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                        icdoQdroCalculationDetail.qdro_percent;
            }
            else if (!iblIsNew && !ablnUVHPFlag && !ablnNonVestedEEFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag)
            {
                ldecFlatAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.flat_amount;

                ldecFlatPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.flat_percent;

                ldecQdroPercent = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.qdro_percent;
            }


            if (ldecQdroPercent != 0)
            {
                #region Apply Brown's Formula - Alternate Payee Fraction
                //if (!iblnParticipantPayeeAccount)
                //{
                    #region Hours Worked

                    CalculateWorkingHrsBasedonUserInput(ref ldecHoursWorkedBetweenDates, ref ldecTotalHoursWorked, astrPlanCode, aintPlanId, ablnNonVestedEEFlag, ablnUVHPFlag,
                                                            ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag);
                    #endregion

                    if (ldecTotalHoursWorked != 0)
                    {

                        ldecAltPayeeFraction = Math.Round(((ldecHoursWorkedBetweenDates / ldecTotalHoursWorked) * ldecQdroPercent) / 100, 3);

                        if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag)
                        {
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                                icdoQdroCalculationDetail.alt_payee_fraction = ldecAltPayeeFraction;
                        }
                        else if (ablnNonVestedEEFlag)
                        {
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                                icdoQdroCalculationDetail.alt_payee_fraction = ldecAltPayeeFraction;
                        }

                        else if (ablnUVHPFlag)
                        {
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                                icdoQdroCalculationDetail.alt_payee_fraction = ldecAltPayeeFraction;
                        }

                        else if (ablnL161SpecialAccountFlag)
                        {
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                                   icdoQdroCalculationDetail.alt_payee_fraction = ldecAltPayeeFraction;
                        }

                        else if (ablnL52SpecialAccountFlag)
                        {
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                                   icdoQdroCalculationDetail.alt_payee_fraction = ldecAltPayeeFraction;
                        }
                    }
                    else
                    {
                        SetAlternatePayeeFractionValueToZero(aintPlanId, ablnNonVestedEEFlag, ablnUVHPFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag);
                    }
                //}
                #endregion
            }
            else // in the update mode alt_payee raction is not set to zero
            {
                SetAlternatePayeeFractionValueToZero(aintPlanId, ablnNonVestedEEFlag, ablnUVHPFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag);
            }

            return ldecAltPayeeFraction;
        }

        private void SetAlternatePayeeFractionValueToZero(int aintPlanId, bool ablnNonVestedEEFlag, bool ablnUVHPFlag, bool ablnL52SpecialAccountFlag, bool ablnL161SpecialAccountFlag)
        {
            if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag)
            {
                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                    icdoQdroCalculationDetail.alt_payee_fraction = 0;
            }
            else if (ablnNonVestedEEFlag)
            {
                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                    icdoQdroCalculationDetail.alt_payee_fraction = 0;
            }

            else if (ablnUVHPFlag)
            {
                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                    icdoQdroCalculationDetail.alt_payee_fraction = 0;
            }

            else if (ablnL161SpecialAccountFlag)
            {
                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                       icdoQdroCalculationDetail.alt_payee_fraction = 0;
            }

            else if (ablnL52SpecialAccountFlag)
            {
                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                       icdoQdroCalculationDetail.alt_payee_fraction = 0;
            }
        }

        /// <summary>
        /// Delete detail benefit options, yearly detail and iap allocation records when clicked on calculate and save again
        /// </summary>
        private void FlushOlderCalculations()
        {
            foreach (busQdroCalculationDetail lbusQdroCalculationDetail in iclbQdroCalculationDetail)
            {
                if (!lbusQdroCalculationDetail.iclbQdroCalculationOptions.IsNullOrEmpty())
                {
                    foreach (busQdroCalculationOptions lbusQdroCalculationOptions in lbusQdroCalculationDetail.iclbQdroCalculationOptions)
                    {
                        lbusQdroCalculationOptions.icdoQdroCalculationOptions.Delete();
                    }
                    lbusQdroCalculationDetail.iclbQdroCalculationOptions.Clear();
                }

                if (!lbusQdroCalculationDetail.iclbQdroCalculationYearlyDetail.IsNullOrEmpty())
                {
                    foreach (busQdroCalculationYearlyDetail lbusQdroCalculationYearlyDetail in lbusQdroCalculationDetail.iclbQdroCalculationYearlyDetail)
                    {
                        lbusQdroCalculationYearlyDetail.icdoQdroCalculationYearlyDetail.Delete();
                    }
                    lbusQdroCalculationDetail.iclbQdroCalculationYearlyDetail.Clear();
                }
                if (!lbusQdroCalculationDetail.iclbQdroIapAllocationDetail.IsNullOrEmpty())
                {
                    foreach (busQdroIapAllocationDetail lbusQdroIapAllocationDetail in lbusQdroCalculationDetail.iclbQdroIapAllocationDetail)
                    {
                        lbusQdroIapAllocationDetail.icdoQdroIapAllocationDetail.Delete();
                    }
                    lbusQdroCalculationDetail.iclbQdroIapAllocationDetail.Clear();
                }
            }
        }

        public void FlushOlderCalculationsForRefreshCalculation()
        {
            foreach (busQdroCalculationDetail lbusQdroCalculationDetail in this.iclbQdroCalculationDetail)
            {
                if (!lbusQdroCalculationDetail.iclbQdroCalculationOptions.IsNullOrEmpty())
                {
                    foreach (busQdroCalculationOptions lbusQdroCalculationOptions in lbusQdroCalculationDetail.iclbQdroCalculationOptions)
                    {
                        lbusQdroCalculationOptions.icdoQdroCalculationOptions.Delete();
                    }
                    lbusQdroCalculationDetail.iclbQdroCalculationOptions.Clear();
                }

                if (!lbusQdroCalculationDetail.iclbQdroCalculationYearlyDetail.IsNullOrEmpty())
                {
                    foreach (busQdroCalculationYearlyDetail lbusQdroCalculationYearlyDetail in lbusQdroCalculationDetail.iclbQdroCalculationYearlyDetail)
                    {
                        lbusQdroCalculationYearlyDetail.icdoQdroCalculationYearlyDetail.Delete();
                    }
                    lbusQdroCalculationDetail.iclbQdroCalculationYearlyDetail.Clear();
                }
                if (!lbusQdroCalculationDetail.iclbQdroIapAllocationDetail.IsNullOrEmpty())
                {
                    foreach (busQdroIapAllocationDetail lbusQdroIapAllocationDetail in lbusQdroCalculationDetail.iclbQdroIapAllocationDetail)
                    {
                        lbusQdroIapAllocationDetail.icdoQdroIapAllocationDetail.Delete();
                    }
                    lbusQdroCalculationDetail.iclbQdroIapAllocationDetail.Clear();
                }

                lbusQdroCalculationDetail.icdoQdroCalculationDetail.Delete();
            }

            this.iclbQdroCalculationDetail.Clear();
            //Ticket#84882
            this.iblIsNew = true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void CalculateIAPBenefitAmount()
        {
            cdoDummyWorkData lcdoDummyworkData1979 = LoadQDROHoursForIAPPlan();
            decimal ldecIAPHours4QtrAlloc = busConstant.ZERO_DECIMAL, ldecIAPHoursA2forQtrAlloc = busConstant.ZERO_DECIMAL,
                ldecIAPPercent4forQtrAlloc = busConstant.ZERO_DECIMAL, ldecAltPayeeIAPBalance = busConstant.ZERO_DECIMAL;
            bool lblnExecuteIAPQuaterlyAllocation = true;
            DateTime ldtNetInvestmentFromDate = new DateTime();
            DateTime ldtNetInvestmentToDate = new DateTime();
            int lintBalanceAsOfPlanYear = 0;

            if (icdoQdroCalculationHeader.qdro_application_id != 0 &&
                      (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                       icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))
            {
                if (ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0 && 
                    (!ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                                            item.icdoDroBenefitDetails.l161_spl_acc_flag != busConstant.FLAG_YES &&
                                            item.icdoDroBenefitDetails.l52_spl_acc_flag != busConstant.FLAG_YES).IsNullOrEmpty()))
                   
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                           item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                           item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.net_investment_from_date =
                   ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                                                item.icdoDroBenefitDetails.l161_spl_acc_flag != busConstant.FLAG_YES &&
                                            item.icdoDroBenefitDetails.l52_spl_acc_flag != busConstant.FLAG_YES).First().icdoDroBenefitDetails.net_investment_from_date;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                                item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).
                                                        First().icdoQdroCalculationDetail.net_investment_to_date =
                    ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                                                 item.icdoDroBenefitDetails.l161_spl_acc_flag != busConstant.FLAG_YES &&
                                            item.icdoDroBenefitDetails.l52_spl_acc_flag != busConstant.FLAG_YES).First().icdoDroBenefitDetails.net_investment_to_date;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                                item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).
                                                        First().icdoQdroCalculationDetail.balance_as_of_plan_year =
                    ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                                                item.icdoDroBenefitDetails.l161_spl_acc_flag != busConstant.FLAG_YES &&
                                            item.icdoDroBenefitDetails.l52_spl_acc_flag != busConstant.FLAG_YES).First().icdoDroBenefitDetails.balance_as_of_plan_year;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                        item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.alt_payee_benefit_cap_year =
                        ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                                                 item.icdoDroBenefitDetails.l161_spl_acc_flag != busConstant.FLAG_YES &&
                                            item.icdoDroBenefitDetails.l52_spl_acc_flag != busConstant.FLAG_YES).First().icdoDroBenefitDetails.alt_payee_benefit_cap_year;
                   }
            }

            if (iclbQdroCalculationDetail != null && iclbQdroCalculationDetail.Count > 0 &&
                    (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty()))
            {
                ldtNetInvestmentFromDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                            item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.net_investment_from_date;

                ldtNetInvestmentToDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                            item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).
                                                    First().icdoQdroCalculationDetail.net_investment_to_date;

                lintBalanceAsOfPlanYear = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                            item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).
                                                    First().icdoQdroCalculationDetail.balance_as_of_plan_year;
            }

            DateTime ldtDateForCalculationIAPBeneift = new DateTime();
            //PIR - 1015
            busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
            lbusIapAllocationSummary.LoadLatestAllocationSummary();

            if (lintBalanceAsOfPlanYear != 0)
            {
                if (lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year < lintBalanceAsOfPlanYear)
                {
                    ldtDateForCalculationIAPBeneift = busGlobalFunctions.GetLastDateOfComputationYear(lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year);
                }
                else
                {
                    ldtDateForCalculationIAPBeneift = busGlobalFunctions.GetLastDateOfComputationYear(lintBalanceAsOfPlanYear);
                }                    
            }
            else
            {
                ldtDateForCalculationIAPBeneift = GetRetirementDateforCalculation();
            }

            #region To Set Values for IAP QTR Allocations

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            SqlParameter[] parameters = new SqlParameter[3];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
            SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);

            param1.Value = this.ibusParticipant.icdoPerson.istrSSNNonEncrypted;
            parameters[0] = param1;
            
            param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year);
            parameters[1] = param2;

            param3.Value = busGlobalFunctions.GetLastDayOfWeek(GetRetirementDateforCalculation());//PROD PIR 113
            parameters[2] = param3;

            DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPHourInfoForQuarterlyAllocation", astrLegacyDBConnetion, null, parameters);
            if (ldtbIAPInfo.Rows.Count > 0)
            {
                //if (ldtbIAPInfo.Rows[0]["IAPHours"] != DBNull.Value)
                //    ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHours"]);

                //if (ldtbIAPInfo.Rows[0]["IAPHoursA2"] != DBNull.Value)
                //    ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHoursA2"]);

                //if (ldtbIAPInfo.Rows[0]["IAPPercent"] != DBNull.Value)
                //    ldecIAPPercent4forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPPercent"]);

                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")) > 0)
                    ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")));

                if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")) > 0)
                    ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")));

                DataTable ldtIAPFiltered;
                busIAPAllocationHelper aobjIAPAllocationHelper = new busIAPAllocationHelper();
                foreach (DataRow ldrIAPPercent in ldtbIAPInfo.Rows)
                {
                    if (ldrIAPPercent["IAPPercent"] != DBNull.Value && Convert.ToString(ldrIAPPercent["IAPPercent"]).IsNotNullOrEmpty() &&
                        Convert.ToDecimal(ldrIAPPercent["IAPPercent"]) > 0)
                    {
                        ldtIAPFiltered = new DataTable();
                        ldtIAPFiltered = ldtbIAPInfo.AsEnumerable().Where(o => o.Field<Int16?>("ComputationYear") == Convert.ToInt16(ldrIAPPercent["ComputationYear"])
                            && o.Field<int?>("EmpAccountNo") == Convert.ToInt32(ldrIAPPercent["EmpAccountNo"])).CopyToDataTable();

                        ldecIAPPercent4forQtrAlloc += aobjIAPAllocationHelper.CalculateAllocation4Amount(Convert.ToInt32(ldrIAPPercent["ComputationYear"]), ldtIAPFiltered);
                    }

                }
            }
            #endregion

            if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
            {
                if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                {
                    int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id;
                    if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                    {
                        if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE &&
                            (lintBalanceAsOfPlanYear != 0 && lintBalanceAsOfPlanYear <= icdoQdroCalculationHeader.qdro_commencement_date.Year && (ldtNetInvestmentFromDate != DateTime.MinValue && ldtNetInvestmentToDate != DateTime.MinValue)))
                        {
                            lblnExecuteIAPQuaterlyAllocation = false;
                        }

                        ibusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_TRUE, this.iclbQdroCalculationDetail, null, null, this, ldtDateForCalculationIAPBeneift, ldecIAPHours4QtrAlloc,
                            ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, lblnExecuteIAPQuaterlyAllocation,aintQDROBalanceAsOfYear: lintBalanceAsOfPlanYear); //PIR -1015
                    }
                }
            }

            if ((!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty()))
            {
                ldecAltPayeeIAPBalance = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).FirstOrDefault().icdoQdroCalculationDetail.iap_balance_amount;


                busQdroCalculationOptions lbusQdroCalculationOptions = new busQdroCalculationOptions();
                DataTable ldtbIAPBalance;


                //PIR 965
                decimal ldecQdroPayementAmount = 0M;

                //PIR - 1015
                DataTable ldtblFirstPaymentDate = busBase.Select("cdoQdroCalculationHeader.GetQDROPaymentAmountAndFirstPayDate",
                           new object[3] { icdoQdroCalculationHeader.person_id, icdoQdroCalculationHeader.alternate_payee_id,busConstant.IAP_PLAN_ID });

                if (ldtblFirstPaymentDate != null && ldtblFirstPaymentDate.Rows.Count > 0 &&
                    Convert.ToString(ldtblFirstPaymentDate.Rows[0][enmPaymentHistoryDetail.amount.ToString().ToUpper()]).IsNotNullOrEmpty()) //fixed 01082016 PIR 1012
                {
                    ldecQdroPayementAmount = Convert.ToDecimal(ldtblFirstPaymentDate.Rows[0][enmPaymentHistoryDetail.amount.ToString().ToUpper()]);
                }


                //if (ldtNetInvestmentFromDate == DateTime.MinValue || ldtNetInvestmentToDate == DateTime.MinValue)
                //{

                //    //PIR - 1015
                //    //if (ldtNetInvestmentFromDate == DateTime.MinValue)
                //    //  ldtNetInvestmentFromDate = icdoQdroCalculationHeader.benefit_comencement_date;


                //    // if (ldtNetInvestmentToDate == DateTime.MinValue)
                //    //{
                //       /*if (ldtblFirstPaymentDate != null && ldtblFirstPaymentDate.Rows.Count > 0 &&
                //           Convert.ToString(ldtblFirstPaymentDate.Rows[0][enmPaymentHistoryHeader.payment_date.ToString().ToUpper()]).IsNotNullOrEmpty()) //fixed 01082016 PIR 1012
                //        {

                //            ldtNetInvestmentToDate = Convert.ToDateTime(ldtblFirstPaymentDate.Rows[0][enmPaymentHistoryHeader.payment_date.ToString().ToUpper()]);

                //        }*/
                //    //}
                //}//PIR 965 


                if (ldtNetInvestmentFromDate != DateTime.MinValue && ldtNetInvestmentToDate != DateTime.MinValue && ldecAltPayeeIAPBalance != 0)
                {
                    #region Logic to show Participant benefit amount. Applicable only for IAP allocation

                    //PIR 965
                    int lintComputationYear = 0;
                    decimal ldecParticipantLatestIAPBal = 0;
                    if (ldtNetInvestmentToDate != DateTime.MinValue)
                    {
                        lintComputationYear = ldtNetInvestmentToDate.Year;
                    }
                    //else
                    //{
                    //    lintComputationYear = DateTime.Now.Year;
                    //}

                    ldtbIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                     new object[2] { iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoQdroCalculationDetail.person_account_id,
                                    lintComputationYear});

                    if (ldtbIAPBalance.Rows.Count > 0 && (!string.IsNullOrEmpty(ldtbIAPBalance.Rows[0][enmPersonAccountRetirementContribution.iap_balance_amount.ToString()].ToString())))
                        ldecParticipantLatestIAPBal = Convert.ToDecimal(ldtbIAPBalance.Rows[0][enmPersonAccountRetirementContribution.iap_balance_amount.ToString()]);

                    #endregion

                    decimal ldecAlternatePayeeFraction = CalculateAltPayeeFraction(busConstant.IAP_PLAN_ID, busConstant.IAP, iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                  item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.iblnIsNewRecord);

                    ldecAltPayeeIAPBalance = CalculateIAPAllocation(ldecAltPayeeIAPBalance, ldtNetInvestmentFromDate, ldtNetInvestmentToDate);
                    decimal ldecAdjustmentAmt = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.adjustment_amt;
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.participant_benefit_amount = ldecParticipantLatestIAPBal - ldecAltPayeeIAPBalance + ldecAdjustmentAmt;

                    //PIR 965
                    #region Logic to show alternate payee amount if it exceeds participants benefit

                    //if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.alt_payee_benefit_cap_year != 0)
                    //{
                    //    DataTable ldtbParticipantIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                    //      new object[2] { iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoQdroCalculationDetail.person_account_id, 
                    //                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoQdroCalculationDetail.alt_payee_benefit_cap_year});
                    //    if (ldtbParticipantIAPBalance.IsNotNull() && ldtbParticipantIAPBalance.Rows.Count > 0)
                    //    {
                    //        decimal ldecParticipantIAPBalance = Convert.ToDecimal(Convert.ToBoolean(ldtbParticipantIAPBalance.Rows[0][0].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbParticipantIAPBalance.Rows[0][0]);
                    //        if (ldecParticipantIAPBalance < ldecAltPayeeIAPBalance)
                    //        {
                    //            ldecAltPayeeIAPBalance = ldecParticipantIAPBalance;
                    //        }
                    //    }
                    //}

                    #endregion
                    decimal lQdroFirstPayment = 0;

                    decimal lQdroSecondPayment = 0;
                    //PIR 965
                    if (ldecAltPayeeIAPBalance - ldecQdroPayementAmount > 0)
                         ldecAltPayeeIAPBalance -= ldecQdroPayementAmount;
                        
                    if(icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT||
                        icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                    {


                        DataTable ldtbPersonMPID = busBase.Select("cdoQdroCalculationHeader.GetQDROFirstPayment", new object[1] { this.ibusParticipant.icdoPerson.person_id });
                        if (ldtbPersonMPID.Rows.Count > 0)
                        {
                             lQdroFirstPayment = Convert.ToDecimal(ldtbPersonMPID.Rows[0][0]);
                        }

                        if (ldecAdjustmentAmt == 0)
                        {                          
                                lQdroSecondPayment =  ldecAltPayeeIAPBalance - ldecAdjustmentAmt;
                           
                        }
                        else
                        {
                           
                            if (ldecAltPayeeIAPBalance - ldecQdroPayementAmount > 0)
                            {
                                ldecAltPayeeIAPBalance -= ldecQdroPayementAmount;
                                lQdroSecondPayment = ldecAltPayeeIAPBalance;

                            }else
                            {
                                lQdroSecondPayment = 0;
                            }
                        }

                        ////      lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM), 1,
                        //                                                 ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, lQdroSecondPayment, false);
                        lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM), 1,
                                                                          ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, lQdroSecondPayment, false);


                    }
                    else
                    {
                        lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM), 1,
                                                                    ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecAltPayeeIAPBalance - ldecAdjustmentAmt, false);
                    }
                    //   lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM), 1,
                    //                                                 ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecAltPayeeIAPBalance, false);
                    lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.LUMP_SUM);
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);

                }
                //PIR - 1015
                else
                {
                    ldecAltPayeeIAPBalance = 0;
                    decimal ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.IAP_PLAN_ID, busConstant.IAP,
                                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.iblnIsNewRecord);

                    string lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;

                    CalculateQDROFactorAndAmount(busConstant.CodeValueAll, busConstant.IAP, busConstant.IAP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel, false, false, false, false, false, false, true);
                    decimal ldecAdjustmentAmt = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.adjustment_amt;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.participant_benefit_amount += ldecAdjustmentAmt;
                    ldecAltPayeeIAPBalance = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.alt_payee_amt_before_conversion - ldecAdjustmentAmt;

                    ldecAltPayeeIAPBalance -= ldecQdroPayementAmount;


                    lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM), 1,
                                    ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecAltPayeeIAPBalance, false);

                    lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.LUMP_SUM);
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);

                }

                if (lcdoDummyworkData1979 != null)
                {
                    busQdroCalculationYearlyDetail lbusQdroCalculationYearlyDetail = new busQdroCalculationYearlyDetail();

                    lbusQdroCalculationYearlyDetail.LoadData(lcdoDummyworkData1979.qualified_hours, lcdoDummyworkData1979.bis_years_count, lcdoDummyworkData1979.year,
                                                                lcdoDummyworkData1979.qualified_years_count, lcdoDummyworkData1979.vested_hours, lcdoDummyworkData1979.vested_years_count, lcdoDummyworkData1979.idecBenefitRate,
                                                                lcdoDummyworkData1979.idecBenefitAmount, lcdoDummyworkData1979.idecTotalHealthHours, lcdoDummyworkData1979.iintHealthCount, 0,
                                                                lcdoDummyworkData1979.idecQdroHours, idecThru79Hours, ldecAltPayeeIAPBalance);
                    iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().iclbQdroCalculationYearlyDetail.Add(lbusQdroCalculationYearlyDetail);
                }
            }

            #region Special Account Calculation

            if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                CalculateLocalIAP52SpecialAccount(lcdoDummyworkData1979);
                CalculateLocalIAP5161pecialAccount(lcdoDummyworkData1979);
            }

            #endregion
        }

        private void CalculateWorkingHrsBasedonUserInput(ref decimal adecServiceBetweenDates, ref decimal adecTotalService,
                                                          string astrPlanCode, int aintPlanId, bool ablnNonVestedEEFlag = false, bool ablnUVHPFlag = false,
                                                          bool ablnL52SpecialAccountFlag = false, bool ablnL161SpecialAccountFlag = false)
        {
            #region Set Overriden values

            decimal ldecOverridenCommunityPropertyPeriod = 0, ldecOverridenTotalValue = 0;
            DateTime ldtCommunityPropertyEndDate = new DateTime();

            if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL161SpecialAccountFlag && !ablnL52SpecialAccountFlag)
            {
                ldecOverridenCommunityPropertyPeriod = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId).First().
                           icdoQdroCalculationDetail.overriden_community_property_period;
                ldecOverridenTotalValue = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId).First().
                           icdoQdroCalculationDetail.overriden_total_value;
                ldtCommunityPropertyEndDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId).First().
                           icdoQdroCalculationDetail.community_property_end_date;
            }
            else if (ablnNonVestedEEFlag)
            {
                ldecOverridenCommunityPropertyPeriod = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_community_property_period;
                ldecOverridenCommunityPropertyPeriod = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_community_property_period;
                ldecOverridenTotalValue = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_total_value;
                ldtCommunityPropertyEndDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date;
            }
            else if (ablnUVHPFlag)
            {
                ldecOverridenCommunityPropertyPeriod = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_community_property_period;
                ldecOverridenCommunityPropertyPeriod = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_community_property_period;
                ldecOverridenTotalValue = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_total_value;
                ldtCommunityPropertyEndDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date;
            }
            else if (ablnL161SpecialAccountFlag)
            {
                ldecOverridenCommunityPropertyPeriod = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_community_property_period;
                ldecOverridenCommunityPropertyPeriod = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_community_property_period;
                ldecOverridenTotalValue = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_total_value;
                ldtCommunityPropertyEndDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date;
            }
            else if (ablnL52SpecialAccountFlag)
            {
                ldecOverridenCommunityPropertyPeriod = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_community_property_period;
                ldecOverridenCommunityPropertyPeriod = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_community_property_period;
                ldecOverridenTotalValue = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_total_value;
                ldtCommunityPropertyEndDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId
                                                       && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date;
            }

            #endregion

            #region Hours Worked

            if ((ldecOverridenCommunityPropertyPeriod == 0 && ldecOverridenTotalValue == 0) || icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL
                || icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
            {
                DateTime ldtForeitureDate = new DateTime();
                DateTime ldtVestedDate = new DateTime();
                if (this.ibusBenefitApplication.ibusTempPersonAccountEligibility != null)
                {
                    ldtForeitureDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                    ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                }

                //Prod PIR : 81 : 01/21/2013
                #region Check For WithdrawalDate 
                //ChangeID:55941 Uncomment the code as part of PIR - 1015
                //PIR : 1015 Complete code commented based on Teresa's explanation sample mpid: M01434784 & AP ID: M01495128
                if ((astrPlanCode == busConstant.IAP || astrPlanCode == busConstant.MPIPP) && !iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                {
                    Collection<busPersonAccountRetirementContribution> lclbPersonAccountRetirementContribution =
                                              new Collection<busPersonAccountRetirementContribution>();
                    busPersonAccountRetirementContribution lbusPersonAccountRetCont = null;

                    if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.transaction_type_value == "PMNT" && item.icdoPersonAccountRetirementContribution.contribution_type_value == "EE").Count() > 0)
                    {
                        lclbPersonAccountRetirementContribution = this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.transaction_type_value == "PMNT" && item.icdoPersonAccountRetirementContribution.contribution_type_value == "EE").OrderByDescending(item => item.icdoPersonAccountRetirementContribution.effective_date.Year).ToList().ToCollection();
                    }

                    if (!lclbPersonAccountRetirementContribution.IsNullOrEmpty())
                    {
                        if (ldtVestedDate != DateTime.MinValue)
                        {
                            if (lclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.effective_date < ldtVestedDate).Count() > 0)
                            {
                                Collection<busPersonAccountRetirementContribution> lclbRetContributionBeforeVesting = lclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.transaction_type_value == "PMNT" && item.icdoPersonAccountRetirementContribution.contribution_type_value == "EE" &&
                                     item.icdoPersonAccountRetirementContribution.effective_date < ldtVestedDate).OrderByDescending(item => item.icdoPersonAccountRetirementContribution.effective_date).ToList().ToCollection();
                                if (lclbRetContributionBeforeVesting.FirstOrDefault().icdoPersonAccountRetirementContribution.effective_date.Year >= 1976)
                                {
                                    lbusPersonAccountRetCont = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                    lbusPersonAccountRetCont = lclbRetContributionBeforeVesting.FirstOrDefault();
                                    if (ldtForeitureDate < lbusPersonAccountRetCont.icdoPersonAccountRetirementContribution.effective_date)
                                    {
                                        ldtForeitureDate = lbusPersonAccountRetCont.icdoPersonAccountRetirementContribution.effective_date;
                                    }
                                }
                            }
                        }
                        else
                        {
                            lbusPersonAccountRetCont = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                            lbusPersonAccountRetCont = lclbPersonAccountRetirementContribution.FirstOrDefault();
                            if (ldtForeitureDate < lbusPersonAccountRetCont.icdoPersonAccountRetirementContribution.effective_date)
                            {
                                ldtForeitureDate = lbusPersonAccountRetCont.icdoPersonAccountRetirementContribution.effective_date;
                            }
                        }
                    }
                }

                #endregion

                if (ldtForeitureDate != DateTime.MinValue && ldtForeitureDate.Year >= 1979)
                {
                    idecIAP1979Hours = 0;
                }

                //ibusCalculation.GetTotalHoursWorked(ibusParticipant.icdoPerson.istrSSNNonEncrypted.ToString(), astrPlanCode,
                //                                   GetRetirementDateforCalculation(), ldtForeitureDate, idecIAP1979Hours, ref adecTotalService);
                // this.ibusParticipant.iclbPersonAccount
                
                   
                        ibusCalculation.GetTotalHoursWorked(ibusParticipant.icdoPerson.istrSSNNonEncrypted.ToString(), astrPlanCode,
                                                   GetRetirementDateforCalculation(), ldtForeitureDate, idecIAP1979Hours, ref adecTotalService);

                //Prodfix_02/07/2013_4
                //Prod PIR : 81 : 01/21/2013
                //Ticket#84882
                if (ldecOverridenTotalValue > 0)
                {
                    adecTotalService = ldecOverridenTotalValue;
                }

                if(astrPlanCode != busConstant.IAP)
                {
                    this.LoadQDROHoursForLOCALandMPIPPPlan();
                }
                adecServiceBetweenDates = ibusCalculation.GetProratedHoursBetweenTwoDates(icdoQdroCalculationHeader.date_of_marriage,
                                          GetDateOfSeperationForCalculation(aintPlanId, ablnNonVestedEEFlag, ablnUVHPFlag, ablnL52SpecialAccountFlag, ablnL161SpecialAccountFlag),
                                                (astrPlanCode == busConstant.IAP) ? this.ibusBenefitApplication.aclbPersonWorkHistory_IAP : this.ibusBenefitApplication.aclbPersonWorkHistory_MPI, astrPlanCode, ldtForeitureDate,Convert.ToString(ibusParticipant.icdoPerson.istrSSNNonEncrypted),aintPlanId);
                //Ticket#84882
                if (ldecOverridenCommunityPropertyPeriod > 0)
                {
                    adecServiceBetweenDates = ldecOverridenCommunityPropertyPeriod;

                }

                if (ablnNonVestedEEFlag)
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                        icdoQdroCalculationDetail.total_service = adecTotalService;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.community_property_service = adecServiceBetweenDates;

                    if (iblIsNew)
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                              icdoQdroCalculationDetail.community_property_end_date = icdoQdroCalculationHeader.date_of_seperation;
                }
                else if (ablnUVHPFlag)
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                         icdoQdroCalculationDetail.total_service = adecTotalService;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.community_property_service = adecServiceBetweenDates;

                    if (iblIsNew)
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.community_property_end_date = icdoQdroCalculationHeader.date_of_seperation;
                }
                else if (ablnL161SpecialAccountFlag)
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                          icdoQdroCalculationDetail.total_service = adecTotalService;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.community_property_service = adecServiceBetweenDates;

                    if (iblIsNew)
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.community_property_end_date = icdoQdroCalculationHeader.date_of_seperation;
                }
                else if (ablnL52SpecialAccountFlag)
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                           icdoQdroCalculationDetail.total_service = adecTotalService;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.community_property_service = adecServiceBetweenDates;

                    if (iblIsNew)
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.community_property_end_date = icdoQdroCalculationHeader.date_of_seperation;
                }
                else if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL161SpecialAccountFlag && !ablnL52SpecialAccountFlag)
                {
                   this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                              item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                              item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                            icdoQdroCalculationDetail.total_service = adecTotalService;

                  this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                              item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                              item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                            icdoQdroCalculationDetail.community_property_service = adecServiceBetweenDates;

                    if (iblIsNew)
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                             item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                             item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                           icdoQdroCalculationDetail.community_property_end_date = icdoQdroCalculationHeader.date_of_seperation;
                }
            }
            else if (!iblIsNew && ldecOverridenCommunityPropertyPeriod != 0 && ldecOverridenTotalValue != 0)
            {
                if (ablnUVHPFlag)
                {
                    adecServiceBetweenDates = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                                item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                                                icdoQdroCalculationDetail.overriden_community_property_period;

                    adecTotalService = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                                item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                                                icdoQdroCalculationDetail.overriden_total_value;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                           item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                    icdoQdroCalculationDetail.total_service = 0;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                           item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                           icdoQdroCalculationDetail.community_property_service = 0;
                }
                else if (ablnNonVestedEEFlag)
                {
                    adecServiceBetweenDates = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                                item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_community_property_period;

                    adecTotalService = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                                item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_total_value;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                          item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.total_service = 0;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                           item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_service = 0;
                }
                else if (ablnL161SpecialAccountFlag)
                {
                    adecServiceBetweenDates = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                                item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_community_property_period;

                    adecTotalService = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                                item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_total_value;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                         item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.total_service = 0;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                           item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_service = 0;
                }
                else if (ablnL52SpecialAccountFlag)
                {
                    adecServiceBetweenDates = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                                item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_community_property_period;

                    adecTotalService = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                                item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.overriden_total_value;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                      item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.total_service = 0;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                           item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_service = 0;
                }
                else if (!ablnUVHPFlag && !ablnNonVestedEEFlag && !ablnL161SpecialAccountFlag && !ablnL52SpecialAccountFlag)
                {
                    adecServiceBetweenDates = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                            item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.overriden_community_property_period;

                    adecTotalService = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                            item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                           icdoQdroCalculationDetail.overriden_total_value;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                            item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                     icdoQdroCalculationDetail.total_service = 0;

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                            item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                           icdoQdroCalculationDetail.community_property_service = 0;
                }
            }

            #endregion
        }

        public string GetPlanCode()
        {
            string lstrPlancode = string.Empty;
            DataTable ldtbList = Select<cdoPlan>(new string[1] { enmPlan.plan_id.ToString() },
                                    new object[1] { icdoQdroCalculationHeader.iintPlanId }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                lstrPlancode = ldtbList.Rows[0][enmPlan.plan_code.ToString()].ToString();
            }

            return lstrPlancode;
        }

        #region Calculate Local Benefit Amount

        protected void CalculateLocal161BenefitAmount(bool ablnFinalCalc, string astrBenefitOptions)
        {
            decimal ldecLocalBenefitAmt = 0;
            int lintPersonAccountId = ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id;
            string lstrRetirementSubType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType;
            decimal ldecPreBISBenefitAmt = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_pre_bis_amount;
            decimal ldecPostBISBenefitAmt = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_post_bis_amount;
            //Ticket#84882
            if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES && lstrRetirementSubType != busConstant.RETIREMENT_TYPE_LATE)
            {
                ldecLocalBenefitAmt = this.iclbPersonAccountRetirementContribution.Where(
                   item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
            }
            else if (iblnIsParticipantDead == true)
            {
                ldecLocalBenefitAmt = ibusCalculation.CalculateTotalBenefitAmtForDeathLocals(lstrRetirementSubType, GetRetirementDateforCalculation(),
                ibusParticipant.icdoPerson.idtDateofBirth,
                this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                busConstant.BOOL_TRUE, ibusBenefitApplication, ldecPreBISBenefitAmt, ldecPostBISBenefitAmt,
                iclbQdroCalculationDetail, null, busConstant.LOCAL_161_PLAN_ID, Convert.ToInt32(Math.Floor(icdoQdroCalculationHeader.age)),
                Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(
                                item => item.icdoPersonAccountRetirementContribution.computational_year)), this.iclbPersonAccountRetirementContribution, ablnFinalCalc);
            }
            else
            {

                ldecLocalBenefitAmt = ibusCalculation.CalculateTotalBenefitAmtForLocal161(lstrRetirementSubType, GetRetirementDateforCalculation(),
                                      ibusParticipant.icdoPerson.idtDateofBirth,
                this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                busConstant.BOOL_TRUE, ibusBenefitApplication, ldecPreBISBenefitAmt, ldecPostBISBenefitAmt,
                iclbQdroCalculationDetail, null,
                Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                this.iclbPersonAccountRetirementContribution, ablnFinalCalc);
            }

            CalculateLocal161BenefitOption(astrBenefitOptions, ldecLocalBenefitAmt);
        }

        protected void CalculateLocal52BenefitAmount(bool ablnFinalCalc, string astrBenefitOptions)
        {
            int lintPersonAccountId = ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id;
            string lstrRetirementSubType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType;
            decimal ldecLocalBenefitAmt = 0;
            decimal ldecPreBISBenefitAmt = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_pre_bis_amount;
            decimal ldecPostBISBenefitAmt = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_post_bis_amount;
            //Ticket#84882
            if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES && lstrRetirementSubType != busConstant.RETIREMENT_TYPE_LATE)
            {
                ldecLocalBenefitAmt = this.iclbPersonAccountRetirementContribution.Where(
                    item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i =>i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
            }
            else if (iblnIsParticipantDead == true)
            {
                ldecLocalBenefitAmt = ibusCalculation.CalculateTotalBenefitAmtForDeathLocals(lstrRetirementSubType, GetRetirementDateforCalculation(),
                ibusParticipant.icdoPerson.idtDateofBirth,
                this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                busConstant.BOOL_TRUE, ibusBenefitApplication, ldecPreBISBenefitAmt, ldecPostBISBenefitAmt,
                iclbQdroCalculationDetail, null, busConstant.LOCAL_52_PLAN_ID, Convert.ToInt32(Math.Floor(icdoQdroCalculationHeader.age)),
                Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(
                                item => item.icdoPersonAccountRetirementContribution.computational_year)), this.iclbPersonAccountRetirementContribution, ablnFinalCalc);
            }
            else
            {
                ldecLocalBenefitAmt = ibusCalculation.CalculateTotalBenefitAmtForLocal52(lstrRetirementSubType, GetRetirementDateforCalculation(),
                ibusParticipant.icdoPerson.idtDateofBirth,
                this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                busConstant.BOOL_TRUE, ibusBenefitApplication, ldecPreBISBenefitAmt, ldecPostBISBenefitAmt,
                iclbQdroCalculationDetail, null,
                Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                this.iclbPersonAccountRetirementContribution, ablnFinalCalc);
            }

            CalculateLocal52BenefitOption(astrBenefitOptions, ldecLocalBenefitAmt);
        }

        protected void CalculateLocal600BenefitAmount(bool ablnFinalCalc, string astrBenefitOptions)
        {
            int lintPersonAccountId = ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id;
            string lstrRetirementSubType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType;
            decimal ldecLocalBenefitAmt;
            decimal ldecPreBISBenefitAmt = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_pre_bis_amount;
            decimal ldecPostBISBenefitAmt = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_post_bis_amount;
            //Ticket#84882
            if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES && lstrRetirementSubType != busConstant.RETIREMENT_TYPE_LATE)
            {
                ldecLocalBenefitAmt = this.iclbPersonAccountRetirementContribution.Where(
                    item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
            }
            else if (iblnIsParticipantDead == true)
            {
                ldecLocalBenefitAmt = ibusCalculation.CalculateTotalBenefitAmtForDeathLocals(lstrRetirementSubType, GetRetirementDateforCalculation(),
                ibusParticipant.icdoPerson.idtDateofBirth,
                this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                busConstant.BOOL_TRUE, ibusBenefitApplication, ldecPreBISBenefitAmt, ldecPostBISBenefitAmt,
                iclbQdroCalculationDetail, null, busConstant.LOCAL_600_PLAN_ID, Convert.ToInt32(Math.Floor(icdoQdroCalculationHeader.age)),
                Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(
                                item => item.icdoPersonAccountRetirementContribution.computational_year)), this.iclbPersonAccountRetirementContribution, ablnFinalCalc);
            }
            else
            {
                ldecLocalBenefitAmt = ibusCalculation.CalculateTotalBenefitAmtForLocal600(lstrRetirementSubType, GetRetirementDateforCalculation(),
                ibusParticipant.icdoPerson.idtDateofBirth,
                this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                busConstant.BOOL_TRUE, ibusBenefitApplication, ldecPreBISBenefitAmt, ldecPostBISBenefitAmt,
                iclbQdroCalculationDetail, null,
                Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                this.iclbPersonAccountRetirementContribution, ablnFinalCalc);
            }

            CalculateLocal600BenefitOption(astrBenefitOptions, ldecLocalBenefitAmt);

        }

        protected void CalculateLocal666BenefitAmount(bool ablnFinalCalc, string astrBenefitOptions)
        {
            int lintPersonAccountId = ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id;
            string lstrRetirementSubType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType;
            decimal ldecLocalBenefitAmt = 0;
            decimal ldecPreBISBenefitAmt = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_pre_bis_amount;
            decimal ldecPostBISBenefitAmt = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_post_bis_amount;
            //Ticket#84882
            if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES && lstrRetirementSubType != busConstant.RETIREMENT_TYPE_LATE)
            {
                ldecLocalBenefitAmt = this.iclbPersonAccountRetirementContribution.Where(
                    item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
            }
            else if (iblnIsParticipantDead == true)
            {
                ldecLocalBenefitAmt = ibusCalculation.CalculateTotalBenefitAmtForDeathLocals(lstrRetirementSubType, GetRetirementDateforCalculation(),
                ibusParticipant.icdoPerson.idtDateofBirth,
                this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                busConstant.BOOL_TRUE, ibusBenefitApplication, ldecPreBISBenefitAmt, ldecPostBISBenefitAmt,
                iclbQdroCalculationDetail, null, busConstant.LOCAL_666_PLAN_ID, Convert.ToInt32(Math.Floor(icdoQdroCalculationHeader.age)),
                Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(
                                item => item.icdoPersonAccountRetirementContribution.computational_year)), this.iclbPersonAccountRetirementContribution, ablnFinalCalc);
            }
            else
            {
                ldecLocalBenefitAmt = ibusCalculation.CalculateTotalBenefitAmtForLocal666(lstrRetirementSubType, GetRetirementDateforCalculation(),
                 ibusParticipant.icdoPerson.idtDateofBirth,
                 this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                 busConstant.BOOL_TRUE, ibusBenefitApplication, ldecPreBISBenefitAmt, ldecPostBISBenefitAmt, icdoQdroCalculationHeader.age,
                 iclbQdroCalculationDetail, null,
                 Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                 this.iclbPersonAccountRetirementContribution, ablnFinalCalc);
            }

            CalculateLocal666BenefitOption(astrBenefitOptions, ldecLocalBenefitAmt);
        }

        protected void CalculateLocal700BenefitAmount(bool ablnFinalCalc, string astrBenefitOptions)
        {
            int lintPersonAccountId = ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id;
            string lstrRetirementSubType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType;
            decimal ldecLocalBenefitAmt = 0;
            decimal ldecPreBISBenefitAmt = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_pre_bis_amount;
            decimal ldecPostBISBenefitAmt = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).First().icdoPersonAccountRetirementContribution.local_post_bis_amount;
            //Ticket#84882
            if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES && lstrRetirementSubType != busConstant.RETIREMENT_TYPE_LATE)
            {
                ldecLocalBenefitAmt = this.iclbPersonAccountRetirementContribution.Where(
                    item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount);
            }
            else if (iblnIsParticipantDead == true)
            {
                ldecLocalBenefitAmt = ibusCalculation.CalculateTotalBenefitAmtForDeathLocals(lstrRetirementSubType, GetRetirementDateforCalculation(),
                ibusParticipant.icdoPerson.idtDateofBirth,
                this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                busConstant.BOOL_TRUE, ibusBenefitApplication, ldecPreBISBenefitAmt, ldecPostBISBenefitAmt,
                iclbQdroCalculationDetail, null, busConstant.LOCAL_700_PLAN_ID, Convert.ToInt32(Math.Floor(icdoQdroCalculationHeader.age)),
                Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(
                                item => item.icdoPersonAccountRetirementContribution.computational_year)), this.iclbPersonAccountRetirementContribution, ablnFinalCalc);
            }
            else
            {
                ldecLocalBenefitAmt = ibusCalculation.CalculateTotalBenefitAmtForLocal700(lstrRetirementSubType, GetRetirementDateforCalculation(),
                ibusParticipant.icdoPerson.idtDateofBirth,
                this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Sum(i => i.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                busConstant.BOOL_TRUE, ibusBenefitApplication, ldecPreBISBenefitAmt, ldecPostBISBenefitAmt,
                iclbQdroCalculationDetail, null,
                Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                this.iclbPersonAccountRetirementContribution, ablnFinalCalc);
            }

            CalculateLocal700BenefitOption(astrBenefitOptions, ldecLocalBenefitAmt);
        }

        #endregion

        #region Calculate Local Benefit options

        private void CalculateLocal161BenefitOption(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            busPersonAccount lbusPersonAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).ToList().ToCollection()[0];

            if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.Local_161))
            {
                DateTime ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                string lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType;
                decimal ldecAltPayeeFraction = 0;
                string lstrDROModel = string.Empty;
                //Ticket#84882
                if (astrBenefitOption == "All" && icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    astrBenefitOption = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId).FirstOrDefault().icdoDroBenefitDetails.istrBenefitOptionValue;
                }
                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().
                               icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoQdroCalculationDetail.early_reduced_benefit_amount = 0;
                    adecTotalBenefitAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().
                        icdoQdroCalculationDetail.accrued_benefit_amt;
                }
                else
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoQdroCalculationDetail.early_reduced_benefit_amount = adecTotalBenefitAmount;

                switch (astrBenefitOption)
                {
                    case busConstant.CodeValueAll:

                        
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_161_PLAN_ID, busConstant.Local_161,
                                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoQdroCalculationDetail.iblnIsNewRecord);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        if(icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                        {
                            CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.Local_161, busConstant.LOCAL_161_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.Local_161, busConstant.LOCAL_161_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            CalculateQDROFactorAndAmount(busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_161, busConstant.LOCAL_161_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        }

                        break;
                    case busConstant.LUMP_SUM:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_161_PLAN_ID, busConstant.Local_161, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.Local_161, busConstant.LOCAL_161_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    //case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    //    if (adecTotalBenefitAmount > 0)
                    //        ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_161_PLAN_ID, busConstant.Local_161, true);
                    // lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                    // CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_161, busConstant.LOCAL_161_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                    //break;


                    case busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                     if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_161_PLAN_ID, busConstant.Local_161, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_161, busConstant.LOCAL_161_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        

                        break;
                    case busConstant.LIFE_ANNUTIY:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_161_PLAN_ID, busConstant.Local_161, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.Local_161, busConstant.LOCAL_161_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;

                }
            }
        }

        private void CalculateLocal52BenefitOption(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            busPersonAccount lbusPersonAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).ToList().ToCollection()[0];
            decimal ldecSpecialYears = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >= busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52.Year && item.qualified_hours >= 400).Count() + this.ibusBenefitApplication.Local52_PensionCredits;
            if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.Local_52))
            {
                DateTime ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                string lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType;
                decimal ldecAltPayeeFraction = 0;
                string lstrDROModel = string.Empty;
                //Ticket#84882
                if (astrBenefitOption == "All" && icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    astrBenefitOption = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId).FirstOrDefault().icdoDroBenefitDetails.istrBenefitOptionValue;
                }
                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().
                             icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoQdroCalculationDetail.early_reduced_benefit_amount = 0;
                    adecTotalBenefitAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().
                        icdoQdroCalculationDetail.accrued_benefit_amt;
                }
                else
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoQdroCalculationDetail.early_reduced_benefit_amount = adecTotalBenefitAmount;


                switch (astrBenefitOption)
                {
                    case busConstant.CodeValueAll:

                       
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_52_PLAN_ID, busConstant.Local_52,
                                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoQdroCalculationDetail.iblnIsNewRecord);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                       if(icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                        {
                            CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                        }
                        //     CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        //if (ldecSpecialYears < 15)
                        //{
                        //    CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                        //}
                        ////Ticket#68161 
                        //if (ldecSpecialYears >= 15)
                        //{
                        //    CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                        //}
                            

                        break;
                    case busConstant.LUMP_SUM:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_52_PLAN_ID, busConstant.Local_52, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                        //Ticket#68161
                        //if (ldecSpecialYears >= 15)
                        //{
                            if (adecTotalBenefitAmount > 0)
                                ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_52_PLAN_ID, busConstant.Local_52, true);

                            lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                            CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                        //}
                         
                        break;
                    case busConstant.LIFE_ANNUTIY:
                        //Ticket#68161
                        //if (ldecSpecialYears < 15)
                        //{
                            if (adecTotalBenefitAmount > 0)
                                ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_52_PLAN_ID, busConstant.Local_52, true);

                            lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                            CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                        //}
                           
                        break;

                }
            }
        }

        private void CalculateLocal600BenefitOption(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            decimal ldecAltPayeeFraction = 0;

            busPersonAccount lbusPersonAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).ToList().ToCollection()[0];
            if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.Local_600))
            {
                DateTime ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                string lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType;
                string lstrDROModel = string.Empty;

                //Ticket#84882
                if (astrBenefitOption == "All" && icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    astrBenefitOption = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId).FirstOrDefault().icdoDroBenefitDetails.istrBenefitOptionValue;
                }
                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().
                             icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoQdroCalculationDetail.early_reduced_benefit_amount = 0;
                    adecTotalBenefitAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().
                        icdoQdroCalculationDetail.accrued_benefit_amt;
                }
                else
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoQdroCalculationDetail.early_reduced_benefit_amount = adecTotalBenefitAmount;

                switch (astrBenefitOption)
                {
                    case busConstant.CodeValueAll:

                        
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_600_PLAN_ID, busConstant.Local_600,
                                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoQdroCalculationDetail.iblnIsNewRecord);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                        {
                            CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.Local_600, busConstant.LOCAL_600_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.Local_600, busConstant.LOCAL_600_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_600, busConstant.LOCAL_600_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                        }
                            //Ticket#68161
                            //    CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.Local_600, busConstant.LOCAL_600_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            //    CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.Local_600, busConstant.LOCAL_600_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                            CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_600, busConstant.LOCAL_600_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    case busConstant.LUMP_SUM:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_600_PLAN_ID, busConstant.Local_600, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.Local_600, busConstant.LOCAL_600_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_600_PLAN_ID, busConstant.Local_600, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                       
                        CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_600, busConstant.LOCAL_600_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    case busConstant.LIFE_ANNUTIY:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_600_PLAN_ID, busConstant.Local_600, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.Local_600, busConstant.LOCAL_600_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;

                }
            }
        }

        private void CalculateLocal666BenefitOption(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            decimal ldecAltPayeeFraction = 0;

            busPersonAccount lbusPersonAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).ToList().ToCollection()[0];
            if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.Local_666))
            {
                DateTime ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                string lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType;
                string lstrDROModel = string.Empty;
                //Ticket#84882
                if (astrBenefitOption == "All" && icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    astrBenefitOption = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId).FirstOrDefault().icdoDroBenefitDetails.istrBenefitOptionValue;
                }
                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().
                               icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoQdroCalculationDetail.early_reduced_benefit_amount = 0;
                    adecTotalBenefitAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().
                        icdoQdroCalculationDetail.accrued_benefit_amt;
                }
                else
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoQdroCalculationDetail.early_reduced_benefit_amount = adecTotalBenefitAmount;

                switch (astrBenefitOption)
                {
                    case busConstant.CodeValueAll:

                       
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_666_PLAN_ID, busConstant.Local_666,
                                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoQdroCalculationDetail.iblnIsNewRecord);
                       
                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        //PIR 554
                        //Ticket#68161
                        if(icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                        {
                            CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.Local_666, busConstant.LOCAL_666_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.Local_666, busConstant.LOCAL_666_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_666, busConstant.LOCAL_666_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            CalculateQDROFactorAndAmount(busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.Local_666, busConstant.LOCAL_666_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);


                        }
                        break;
                    case busConstant.LUMP_SUM:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_666_PLAN_ID, busConstant.Local_666, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.Local_666, busConstant.LOCAL_666_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_666_PLAN_ID, busConstant.Local_666, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.Local_666, busConstant.LOCAL_666_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    case busConstant.LIFE_ANNUTIY:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_666_PLAN_ID, busConstant.Local_666, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.Local_666, busConstant.LOCAL_666_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    //PIR 554
                    case busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_666_PLAN_ID, busConstant.Local_666, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.Local_666, busConstant.LOCAL_666_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;

                }
            }
        }

        private void CalculateLocal700BenefitOption(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            decimal ldecAltPayeeFraction = 0;

            busPersonAccount lbusPersonAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).ToList().ToCollection()[0];
            if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.LOCAL_700))
            {
                DateTime ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                string lstrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType;
                string lstrDROModel = string.Empty;
                //Ticket#84882
                if (astrBenefitOption == "All" && icdoQdroCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    astrBenefitOption = ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId).FirstOrDefault().icdoDroBenefitDetails.istrBenefitOptionValue;
                }
                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().
                            icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoQdroCalculationDetail.early_reduced_benefit_amount = 0;
                    adecTotalBenefitAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().
                        icdoQdroCalculationDetail.accrued_benefit_amt;
                }
                else
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoQdroCalculationDetail.early_reduced_benefit_amount = adecTotalBenefitAmount;

                switch (astrBenefitOption)
                {
                    case busConstant.CodeValueAll:

                       
                        if (adecTotalBenefitAmount > 0)
                        {
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_700_PLAN_ID, busConstant.LOCAL_700,
                                 this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoQdroCalculationDetail.iblnIsNewRecord);

                            lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                            //Ticket#68161
                            //  CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            //CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            //CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            //CalculateQDROFactorAndAmount(busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                            if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                            {
                                CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                                CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                                CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                                CalculateQDROFactorAndAmount(busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                            }
                            else
                            {
                                foreach (var lQdroApplicationDetails in ibusQdroApplication.iclbDroBenefitDetails)
                                {
                                    if (lQdroApplicationDetails.icdoDroBenefitDetails.istrBenefitOptionValue == busConstant.LIFE_ANNUTIY && lQdroApplicationDetails.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId)
                                    {
                                        CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                                    }
                                    if (lQdroApplicationDetails.icdoDroBenefitDetails.istrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY && lQdroApplicationDetails.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId)
                                    {
                                        CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                                    }

                                    if (lQdroApplicationDetails.icdoDroBenefitDetails.istrBenefitOptionValue == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY && lQdroApplicationDetails.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId)
                                    {
                                        CalculateQDROFactorAndAmount(busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);

                                    }

                                }

                            }

                        }

                        break;
                    case busConstant.LUMP_SUM:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_700_PLAN_ID, busConstant.LOCAL_700, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_700_PLAN_ID, busConstant.LOCAL_700, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    //Ticket#68161
                    case busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_700_PLAN_ID, busConstant.LOCAL_700, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                    case busConstant.LIFE_ANNUTIY:
                        if (adecTotalBenefitAmount > 0)
                            ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.LOCAL_700_PLAN_ID, busConstant.LOCAL_700, true);

                        lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoQdroCalculationDetail.qdro_model_value;
                        CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID, ldecAltPayeeFraction, lstrDROModel);
                        break;
                }
            }
        }

        #endregion

        #region Calculate QDRO Hours to show on Annual Benefit summary Grid

        /// <summary>
        /// Get QDRO Hour
        /// </summary>
        private DataTable CalculateQDROHours(string astrPlanCode, int aintPlanId)
        {
            IDbConnection lconLegacy = DBFunction.GetDBConnection("OPUS");
            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
            IDataReader lDataReader = null;

            IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
            lobjParameter.ParameterName = "@SSN";
            lobjParameter.DbType = DbType.String;
            lobjParameter.Value = this.ibusParticipant.icdoPerson.istrSSNNonEncrypted.ToString();
            lcolParameters.Add(lobjParameter);

            IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
            lobjParameter1.ParameterName = "@PLANCODE";
            lobjParameter1.DbType = DbType.String;
            lobjParameter1.Value = astrPlanCode;
            lcolParameters.Add(lobjParameter1);

            IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
            lobjParameter2.ParameterName = "@DATE_OF_MARRIAGE";
            lobjParameter2.DbType = DbType.DateTime;
            lobjParameter2.Value = icdoQdroCalculationHeader.date_of_marriage;
            lcolParameters.Add(lobjParameter2);

            IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
            lobjParameter3.ParameterName = "@END_OF_DATE_OF_MARRIAGE_COMP_YEAR";
            lobjParameter3.DbType = DbType.DateTime;
            lobjParameter3.Value = busGlobalFunctions.GetLastDateOfComputationYear(icdoQdroCalculationHeader.date_of_marriage.Year);
            lcolParameters.Add(lobjParameter3);

            IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
            lobjParameter4.ParameterName = "@DATE_OF_SEPARATION";
            lobjParameter4.DbType = DbType.DateTime;
            lobjParameter4.Value = GetDateOfSeperationForCalculation(aintPlanId);
            lcolParameters.Add(lobjParameter4);

            IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
            lobjParameter5.ParameterName = "@START_OF_DATE_OF_SEPARATION_COMP_YEAR";
            lobjParameter5.DbType = DbType.DateTime;
            lobjParameter5.Value = busGlobalFunctions.GetFirstDateOfComputationYear(GetDateOfSeperationForCalculation(aintPlanId).Year);
            lcolParameters.Add(lobjParameter5);
           
            lDataReader  =  DBFunction.DBExecuteProcedureResult("usp_GetCommunityServiceHours", lcolParameters, lconLegacy, null);
            DataTable ldataTable = new DataTable();

            if (lDataReader != null)
            {
                ldataTable.Load(lDataReader);
                return ldataTable;
            }
            else
            {
                return null;
            }
        }

        private void LoadQDROHoursForLOCALandMPIPPPlan()
        {
            this.ibusParticipant.LoadParticipantPlan();
            ibusBenefitApplication.aclbPersonWorkHistory_MPI.ForEach(t => t.idecQdroHours = 0);

            foreach (var lpersonAccount in this.ibusParticipant.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id != busConstant.IAP_PLAN_ID))
            {


                DataTable ldtbList = new DataTable();
                if (icdoQdroCalculationHeader.date_of_marriage != DateTime.MinValue && GetDateOfSeperationForCalculation(lpersonAccount.icdoPersonAccount.plan_id) != DateTime.MinValue)
                {
                    // ldtbList = CalculateQDROHours(busConstant.Local_600, busConstant.LOCAL_600_PLAN_ID);
                    ldtbList = CalculateQDROHours(lpersonAccount.icdoPersonAccount.istrPlanCode, lpersonAccount.icdoPersonAccount.plan_id);


                    foreach (cdoDummyWorkData lcdoDummyWorkData in ibusBenefitApplication.aclbPersonWorkHistory_MPI)
                    {
                        if (lcdoDummyWorkData.year >= icdoQdroCalculationHeader.date_of_marriage.Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours += Convert.ToDecimal(ldrData[0][1]);
                        }
                        else if (lcdoDummyWorkData.year == GetDateOfSeperationForCalculation(lpersonAccount.icdoPersonAccount.plan_id).Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours += Convert.ToDecimal(ldrData[0][1]);
                            break;
                        }
                    }
                }
            }
        }
        private void LoadQDROHoursForMPIPPPlan()
        {
            DataTable ldtbList = new DataTable();
            if (icdoQdroCalculationHeader.date_of_marriage != DateTime.MinValue && GetDateOfSeperationForCalculation(busConstant.MPIPP_PLAN_ID) != DateTime.MinValue)
            {
                ldtbList = CalculateQDROHours(busConstant.MPIPP, busConstant.MPIPP_PLAN_ID);
                if (ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNotNull())
                {
                    foreach (cdoDummyWorkData lcdoDummyWorkData in ibusBenefitApplication.aclbPersonWorkHistory_MPI)
                    {
                        if (lcdoDummyWorkData.year >= icdoQdroCalculationHeader.date_of_marriage.Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                        }
                        else if (lcdoDummyWorkData.year == GetDateOfSeperationForCalculation(busConstant.MPIPP_PLAN_ID).Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                            break;
                        }
                    }
                }
            }
        }

        private cdoDummyWorkData LoadQDROHoursForIAPPlan()
        {
            DataTable ldtbList = new DataTable();

            if (icdoQdroCalculationHeader.date_of_marriage != DateTime.MinValue && GetDateOfSeperationForCalculation(busConstant.IAP_PLAN_ID) != DateTime.MinValue)
                ldtbList = CalculateQDROHours(busConstant.IAP, busConstant.IAP_PLAN_ID);

            cdoDummyWorkData lcdoWorkData1979 = null;
            //cdoDummyWorkData lcdoWorkData1979 = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).FirstOrDefault();
            ////IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
            //if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
            //{
            //    idecIAP1979Hours = lcdoWorkData1979.qualified_hours;


            #region IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979

            //Remove history for any forfieture year 1979
            if (ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
            {
                if (ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.istrForfietureFlag == busConstant.FLAG_YES).Count() > 0)
                {
                    int lintMaxForfietureYearBefore1979 = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(item => item.year <= busConstant.BenefitCalculation.YEAR_1979 && item.istrForfietureFlag == busConstant.FLAG_YES).Max(t => t.year);
                    ibusBenefitApplication.aclbPersonWorkHistory_IAP = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(item => item.year > lintMaxForfietureYearBefore1979).ToList().ToCollection();
                }
            }

            if (ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
            {
                decimal ldecPreviousYearPaidIAPAccountBalance = 0M;
                lcdoWorkData1979 = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).FirstOrDefault();
                //IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
                if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
                {
                    int lintPaymentYear = 0;
                    DataTable ldtblPaymentYear = busBase.Select("cdoPersonAccountRetirementContribution.GetMaxPaymentYearOnOrBefore1979", new object[1] { ibusBenefitApplication.ibusPerson.icdoPerson.person_id });
                    if (ldtblPaymentYear != null && ldtblPaymentYear.Rows.Count > 0 && Convert.ToString(ldtblPaymentYear.Rows[0][0]).IsNotNullOrEmpty())
                    {
                        lintPaymentYear = Convert.ToInt32(ldtblPaymentYear.Rows[0][0]);
                    }
                    if (lintPaymentYear == 0)
                    {

                        idecThru79Hours = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Sum(o => o.qualified_hours);

                    }
                    else
                    {
                        if (ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Count() > 0)
                        {
                            idecThru79Hours = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Sum(o => o.qualified_hours);
                        }
                    }

                    idecThru79Hours += ldecPreviousYearPaidIAPAccountBalance;
                    if (idecThru79Hours < 0)
                        idecThru79Hours = 0;
                }

                if (ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
                {
                    idecIAP1979Hours = lcdoWorkData1979.qualified_hours;
                }
                else
                {
                    lcdoWorkData1979 = null;
                }
            }

           
            #endregion


            //idecThru79Hours = ibusBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Sum(o => o.qualified_hours);

            if (lcdoWorkData1979 != null && icdoQdroCalculationHeader.date_of_marriage != DateTime.MinValue && GetDateOfSeperationForCalculation(busConstant.IAP_PLAN_ID) != DateTime.MinValue)
            {
                if (icdoQdroCalculationHeader.date_of_marriage.Year <= busConstant.BenefitCalculation.YEAR_1979)
                {
                    foreach (DataRow ldrData in ldtbList.Rows)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(ldrData[1])) && (!string.IsNullOrEmpty(Convert.ToString(ldrData[0]))) && Convert.ToInt32(ldrData[0]) < busConstant.BenefitCalculation.YEAR_1979)
                            lcdoWorkData1979.idecQdroHours = lcdoWorkData1979.idecQdroHours + Convert.ToDecimal(ldrData[1]);
                        if (icdoQdroCalculationHeader.date_of_marriage.Year == busConstant.BenefitCalculation.YEAR_1979)
                            break;
                    }
                }

                if (GetDateOfSeperationForCalculation(busConstant.IAP_PLAN_ID).Year <= busConstant.BenefitCalculation.YEAR_1979)
                {
                    foreach (DataRow ldrData in ldtbList.Rows)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(ldrData[1])) && (!string.IsNullOrEmpty(Convert.ToString(ldrData[0]))) && Convert.ToInt32(ldrData[0]) < busConstant.BenefitCalculation.YEAR_1979)
                            lcdoWorkData1979.idecQdroHours = lcdoWorkData1979.idecQdroHours + Convert.ToDecimal(ldrData[1]);
                        if (GetDateOfSeperationForCalculation(busConstant.IAP_PLAN_ID).Year == busConstant.BenefitCalculation.YEAR_1979)
                            break;
                    }
                }
            }

            return lcdoWorkData1979;
        }

        private void LoadQDROHoursForLocal52Plan()
        {
             DataTable ldtbList = new DataTable();
            if (icdoQdroCalculationHeader.date_of_marriage != DateTime.MinValue && GetDateOfSeperationForCalculation(busConstant.LOCAL_52_PLAN_ID) != DateTime.MinValue)
            {
                ldtbList = CalculateQDROHours(busConstant.Local_52, busConstant.LOCAL_52_PLAN_ID);
                if (ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNotNull())
                {
                    foreach (cdoDummyWorkData lcdoDummyWorkData in ibusBenefitApplication.aclbPersonWorkHistory_MPI)
                    {
                        if (lcdoDummyWorkData.year >= icdoQdroCalculationHeader.date_of_marriage.Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                        }
                        else if (lcdoDummyWorkData.year == GetDateOfSeperationForCalculation(busConstant.LOCAL_52_PLAN_ID).Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                            break;
                        }
                    }
                }
            }
        }

        private void LoadQDROHoursForLocal600Plan()
        {
            DataTable ldtbList = new DataTable();
            if (icdoQdroCalculationHeader.date_of_marriage != DateTime.MinValue && GetDateOfSeperationForCalculation(busConstant.LOCAL_600_PLAN_ID) != DateTime.MinValue)
            {
                ldtbList = CalculateQDROHours(busConstant.Local_600, busConstant.LOCAL_600_PLAN_ID);
                if (ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNotNull())
                {
                    foreach (cdoDummyWorkData lcdoDummyWorkData in ibusBenefitApplication.aclbPersonWorkHistory_MPI)
                    {
                        if (lcdoDummyWorkData.year >= icdoQdroCalculationHeader.date_of_marriage.Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                        }
                        else if (lcdoDummyWorkData.year == GetDateOfSeperationForCalculation(busConstant.LOCAL_600_PLAN_ID).Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                            break;
                        }
                    }
                }
            }
        }

        private void LoadQDROHoursForLocal700Plan()
        {
            DataTable ldtbList = new DataTable();
            if (icdoQdroCalculationHeader.date_of_marriage != DateTime.MinValue && GetDateOfSeperationForCalculation(busConstant.LOCAL_700_PLAN_ID) != DateTime.MinValue)
            {
                ldtbList = CalculateQDROHours(busConstant.LOCAL_700, busConstant.LOCAL_700_PLAN_ID);
                if (ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNotNull())
                {
                    foreach (cdoDummyWorkData lcdoDummyWorkData in ibusBenefitApplication.aclbPersonWorkHistory_MPI)
                    {
                        if (lcdoDummyWorkData.year >= icdoQdroCalculationHeader.date_of_marriage.Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                        }
                        else if (lcdoDummyWorkData.year == GetDateOfSeperationForCalculation(busConstant.LOCAL_700_PLAN_ID).Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                            break;
                        }
                    }
                }
            }
        }

        private void LoadQDROHoursForLocal666Plan()
        {
            DataTable ldtbList = new DataTable();
            if (icdoQdroCalculationHeader.date_of_marriage != DateTime.MinValue && GetDateOfSeperationForCalculation(busConstant.LOCAL_666_PLAN_ID) != DateTime.MinValue)
            {
                ldtbList = CalculateQDROHours(busConstant.Local_666, busConstant.LOCAL_666_PLAN_ID);
                if (ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNotNull())
                {
                    foreach (cdoDummyWorkData lcdoDummyWorkData in ibusBenefitApplication.aclbPersonWorkHistory_MPI)
                    {
                        if (lcdoDummyWorkData.year >= icdoQdroCalculationHeader.date_of_marriage.Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                        }
                        else if (lcdoDummyWorkData.year == GetDateOfSeperationForCalculation(busConstant.LOCAL_666_PLAN_ID).Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                            break;
                        }
                    }
                }
            }
        }

        private void LoadQDROHoursForLocal161Plan()
        {

            DataTable ldtbList = new DataTable();
            if (icdoQdroCalculationHeader.date_of_marriage != DateTime.MinValue && GetDateOfSeperationForCalculation(busConstant.LOCAL_161_PLAN_ID) != DateTime.MinValue)
            {
                ldtbList = CalculateQDROHours(busConstant.Local_161, busConstant.LOCAL_161_PLAN_ID);
                if (ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNotNull())
                {
                    foreach (cdoDummyWorkData lcdoDummyWorkData in ibusBenefitApplication.aclbPersonWorkHistory_MPI)
                    {
                        if (lcdoDummyWorkData.year >= icdoQdroCalculationHeader.date_of_marriage.Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                        }
                        else if (lcdoDummyWorkData.year == GetDateOfSeperationForCalculation(busConstant.LOCAL_161_PLAN_ID).Year)
                        {
                            DataRow[] ldrData = ldtbList.Select("Year =" + lcdoDummyWorkData.year);
                            if (ldrData.Count() > 0 && !string.IsNullOrEmpty(Convert.ToString(ldrData[0][1])))
                                lcdoDummyWorkData.idecQdroHours = Convert.ToDecimal(ldrData[0][1]);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Get date of seperation for Calculation
        /// </summary>
        /// <returns></returns>
        private DateTime GetDateOfSeperationForCalculation(int aintPlanId, bool ablnNonVestedEEFlag = false, bool ablnUVHPFlag = false, bool ablnL52SpecialAccountFlag = false, bool ablnL161SpecialAccountFlag = false)
        {
            DateTime ldtSeperationDate = new DateTime();

            if (!ablnNonVestedEEFlag && !ablnUVHPFlag && !ablnL52SpecialAccountFlag && !ablnL161SpecialAccountFlag)
            {
                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                        item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                        item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).FirstOrDefault().IsNotNull() &&

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                        item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                        item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.community_property_end_date != DateTime.MinValue)
                {
                    ldtSeperationDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                            item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO &&
                                            item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.community_property_end_date;
                }
            }
            else if (ablnNonVestedEEFlag)
            {
                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                        item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).FirstOrDefault().IsNotNull() &&

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                        item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date != DateTime.MinValue)
                {
                    ldtSeperationDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                             item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date;
                }
            }
            else if (ablnUVHPFlag)
            {
                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().IsNotNull() &&

                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date != DateTime.MinValue)
                {
                    ldtSeperationDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                             item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date;
                }
            }
            else if (ablnL52SpecialAccountFlag)
            {
                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                       item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().IsNotNull() &&

                       this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                       item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date != DateTime.MinValue)
                {
                    ldtSeperationDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                             item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date;
                }
            }
            else if (ablnL161SpecialAccountFlag)
            {
                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                       item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().IsNotNull() &&

                       this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                       item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date != DateTime.MinValue)
                {
                    ldtSeperationDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                             item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.community_property_end_date;
                }
            }

            if (ldtSeperationDate == DateTime.MinValue)
                ldtSeperationDate = icdoQdroCalculationHeader.date_of_seperation;

            return ldtSeperationDate;
        }

        private decimal GetTotalUVHPAmountForCalculation(int aintPersonAccountId)
        {
            decimal ldecOverridenUVHPAmount = iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID
                                                && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.overriden_uvhp_amount;

            decimal ldecOverridenUVHPIntAmount = iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID
                                                    && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.overriden_uv_uvhp_int_amount;
            decimal ldecTotalUVHP = 0;

            ldecTotalUVHP = ibusCalculation.FetchUVHPAmountandInterest(true, this.iclbQdroCalculationDetail, null, null, this, aintPersonAccountId, GetRetirementDateforCalculation());

            if (ldecOverridenUVHPAmount != 0 && ldecOverridenUVHPIntAmount != 0)
            {
                ldecTotalUVHP = ldecOverridenUVHPAmount + ldecOverridenUVHPIntAmount;
            }

            iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID
                 && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.early_reduced_benefit_amount = ldecTotalUVHP;

            return ldecTotalUVHP;
        }

        private decimal GetUVHPContributionAmount()
        {
            if (iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.overriden_uvhp_amount != 0)
                return iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.overriden_uvhp_amount;
            else
                return iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.total_uvhp_contribution_amount;
        }

        private decimal GetUVHPInterestAmount()
        {
            if (iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.overriden_uv_uvhp_int_amount != 0)
                return iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.overriden_uv_uvhp_int_amount;
            else
                return iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.total_uvhp_interest_amount;
        }

        /// <summary>
        /// Calculate Allocation1 amount for the given date Range(Net Investment Income from date to Net Investment Income to date
        /// </summary>
        /// <param name="adecIAPBalance"></param>
        /// <returns></returns>
        private decimal CalculateIAPAllocation(decimal adecIAPBalance, DateTime adtFromDate, DateTime adtToDate, bool ablnL52SpecialAccount = false, bool ablnL161SpecialAccount = false)
        {
            int lintQuarter = 0, lintPersonAccountId = 0;
            decimal ldecGainLossAmt = 0, ldecFactor = 0, ldecBalanceAmount = 0;

            if (!ablnL52SpecialAccount && !ablnL161SpecialAccount)
            {
                string lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO
                                    && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).FirstOrDefault().icdoQdroCalculationDetail.qdro_model_value;

                CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.IAP, busConstant.IAP_PLAN_ID,
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO
                                    && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).FirstOrDefault().icdoQdroCalculationDetail.alt_payee_fraction,lstrDROModel,
                                    false, false, false, false, false, false, false);

                ldecBalanceAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO
                                    && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).FirstOrDefault().icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                lintPersonAccountId = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO
                                    && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).FirstOrDefault().icdoQdroCalculationDetail.person_account_id;
            }
            else if (ablnL161SpecialAccount)
            {
                string lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.qdro_model_value;

                CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.IAP, busConstant.IAP_PLAN_ID,
                     this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.alt_payee_fraction,lstrDROModel,
                                    false, false, false, true, false, false, false);

                ldecBalanceAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                lintPersonAccountId = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.person_account_id;
            }
            else if (ablnL52SpecialAccount)
            {
                string lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.qdro_model_value;

                CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.IAP, busConstant.IAP_PLAN_ID,
                                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.alt_payee_fraction,lstrDROModel,
                                    false, false, true, false, false, false, false);

                ldecBalanceAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                lintPersonAccountId = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.person_account_id;
            }

            busIAPAllocationHelper lbusIAPAllocationHelper = new busIAPAllocationHelper();

            #region Get Quarter

            if (adtToDate.Month <= 3)
            {
                lintQuarter = 1;
            }
            else if (adtToDate.Month > 3 && adtToDate.Month <= 6)
            {
                lintQuarter = 2;
            }
            else if (adtToDate.Month > 6 && adtToDate.Month <= 9)
            {
                lintQuarter = 3;
            }
            else if (adtToDate.Month > 9 && adtToDate.Month <= 12)
            {
                lintQuarter = 4;
            }

            #endregion

            #region Get Gain Loss Amt

            for (int lintStartYear = adtFromDate.Year; lintStartYear <= adtToDate.Year; lintStartYear++)
            {
                busQdroIapAllocationDetail lbusQdroIapAllocationDetail = new busQdroIapAllocationDetail();

                if (lintStartYear == adtToDate.Year)
                {
                    ldecGainLossAmt = lbusIAPAllocationHelper.CalculateAllocation1Amount(lintStartYear, ldecBalanceAmount, lintQuarter, ref ldecFactor);
                    if (ldecFactor == decimal.Zero)
                    {
                        if (!ablnL52SpecialAccount && !ablnL161SpecialAccount)
                        {
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                  ((item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO)
                                || (item.icdoQdroCalculationDetail.l52_spl_acc_flag.IsNullOrEmpty() && item.icdoQdroCalculationDetail.l161_spl_acc_flag.IsNullOrEmpty()))).FirstOrDefault().icdoQdroCalculationDetail.adjustment_iap_payment_flag = busConstant.FLAG_YES;
                        }
                        else if (ablnL161SpecialAccount)
                        {
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                       item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.adjustment_l161spl_payment_flag = busConstant.FLAG_YES;

                        }
                        else if (ablnL52SpecialAccount)
                        {
                            this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.adjustment_l52spl_payment_flag = busConstant.FLAG_YES;
                        }
                    }
                    ldecBalanceAmount = ldecBalanceAmount + ldecGainLossAmt;
                    if (lintQuarter == 1)
                    {
                        lbusQdroIapAllocationDetail.LoadData(lintPersonAccountId, lintStartYear, ldecFactor, 0, 0, 0, ldecGainLossAmt, ldecBalanceAmount);
                    }
                    if (lintQuarter == 2)
                    {
                        lbusQdroIapAllocationDetail.LoadData(lintPersonAccountId, lintStartYear, 0, ldecFactor, 0, 0, ldecGainLossAmt, ldecBalanceAmount);
                    }
                    if (lintQuarter == 3)
                    {
                        lbusQdroIapAllocationDetail.LoadData(lintPersonAccountId, lintStartYear, 0, 0, ldecFactor, 0, ldecGainLossAmt, ldecBalanceAmount);
                    }
                    if (lintQuarter == 4)
                    {
                        lbusQdroIapAllocationDetail.LoadData(lintPersonAccountId, lintStartYear, 0, 0, 0, ldecFactor, ldecGainLossAmt, ldecBalanceAmount);
                    }
                }
                else
                {
                    ldecGainLossAmt = lbusIAPAllocationHelper.CalculateAllocation1Amount(lintStartYear, ldecBalanceAmount, 4, ref ldecFactor);
                    ldecBalanceAmount = ldecBalanceAmount + ldecGainLossAmt;

                    lbusQdroIapAllocationDetail.LoadData(lintPersonAccountId, lintStartYear, 0, 0, 0, ldecFactor, ldecGainLossAmt, ldecBalanceAmount);
                }

                if (lintQuarter != 0)
                {
                    if (!ablnL52SpecialAccount && !ablnL161SpecialAccount)
                    {
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO &&
                            item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).First().iclbQdroIapAllocationDetail.Add(lbusQdroIapAllocationDetail);
                    }
                    else if (ablnL161SpecialAccount)
                    {
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().iclbQdroIapAllocationDetail.Add(lbusQdroIapAllocationDetail);
                    }
                    else if (ablnL52SpecialAccount)
                    {
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().iclbQdroIapAllocationDetail.Add(lbusQdroIapAllocationDetail);
                    }
                }
            }

            #endregion

            return ldecBalanceAmount;
        }

        private decimal GetAccruedBenefitForCalculation(int aintPlanId, bool ablnUVHPFlag, bool ablnEEFlag, bool ablnL52SpecialAccount, bool ablnL161SpecialAccount)
        {
            if (ablnUVHPFlag && (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
            {

                if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                {
                    return this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                                icdoQdroCalculationDetail.accrued_benefit_amt;
                }
                else
                {
                    return this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().
                               icdoQdroCalculationDetail.early_reduced_benefit_amount;
                }
            }
            else if (ablnEEFlag && (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                    item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
            {
                if (this.iblnParticipantPayeeAccount)
                {
                    return this.idecParticipantAmount;
                }
                else if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                {
                    return this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                                icdoQdroCalculationDetail.accrued_benefit_amt;
                }
                else
                {
                    return this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().
                               icdoQdroCalculationDetail.early_reduced_benefit_amount;
                }
            }
            else if (ablnL52SpecialAccount && (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                   item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
            {
                if (this.iblnParticipantPayeeAccount)
                {
                    return this.idecParticipantAmount;
                }
                else if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                {
                    return this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                                icdoQdroCalculationDetail.accrued_benefit_amt;
                }
                else
                {
                    return this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().
                               icdoQdroCalculationDetail.early_reduced_benefit_amount;
                }
            }
            else if (ablnL161SpecialAccount && (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId &&
                                   item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
            {
                if (this.iblnParticipantPayeeAccount)
                {
                    return this.idecParticipantAmount;
                }
                else if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                            icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                {
                    return this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                                icdoQdroCalculationDetail.accrued_benefit_amt;
                }
                else
                {
                    return this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().
                               icdoQdroCalculationDetail.early_reduced_benefit_amount;
                }
            }
            else if (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty())
            {
                if (this.iblnParticipantPayeeAccount)
                {
                    return this.idecParticipantAmount;
                }
                else if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                   item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                           icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                {
                    return this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                                icdoQdroCalculationDetail.accrued_benefit_amt;
                }
                else
                {
                    return this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == aintPlanId && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().
                               icdoQdroCalculationDetail.early_reduced_benefit_amount;
                }
            }

            return 0;
        }

        #region Sub Plan Calculations

        private void CalculateUVHPAmount(int aintPersonAccountId)
        {
            bool lblnApplyBenefitOptions = false;
            decimal ldecAltPayeeFraction = 0;
            int lintDROApplicationDetailId = 0;
            DateTime ldtVestedDate = new DateTime();

            if (iclbQdroCalculationDetail != null && iclbQdroCalculationDetail.Count > 0 &&
                (!iclbQdroCalculationDetail.Where(
                                     item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                     item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).IsNullOrEmpty()))
            {
                ldtVestedDate = iclbQdroCalculationDetail.Where(
                                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.vested_date;
            }
            else
            {
                //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP))
                {
                    ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    iblnIsParticipantVested = true;
                }
            }

            if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0 &&
                  (!ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                       item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id &&
                       item.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
            {
                lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                   item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                       item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id &&
                       item.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.dro_benefit_id;
            }

            if (((icdoQdroCalculationHeader.qdro_application_id != 0 && lintDROApplicationDetailId > 0) || icdoQdroCalculationHeader.qdro_application_id == 0) &&
                ((!iblIsNew && iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).IsNullOrEmpty()) || iblIsNew))
            {
                busQdroCalculationDetail lbusQdroCalculationDetail = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };
                lbusQdroCalculationDetail.iobjMainCDO = lbusQdroCalculationDetail.icdoQdroCalculationDetail;

                lbusQdroCalculationDetail.LoadData(this.icdoQdroCalculationHeader.qdro_calculation_header_id, this.icdoQdroCalculationHeader.iintPlanId, busConstant.MPIPPUVHP,
                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                    this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id, ldtVestedDate,
                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.istrRetirementSubType, lintDROApplicationDetailId,
                    busConstant.FLAG_YES, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, true);

                this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetail);
                lblnApplyBenefitOptions = true;
            }
            else if (!iblIsNew && !iclbQdroCalculationDetail.Where(
                                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).IsNullOrEmpty())
            {
                iclbQdroCalculationDetail.Where(
                                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).
                                        First().icdoQdroCalculationDetail.vested_date = ldtVestedDate;

                iclbQdroCalculationDetail.Where(
                                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).
                                        First().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).
                    First().icdoQdroCalculationDetail.benefit_subtype_value = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(
                    item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrRetirementSubType;
                lblnApplyBenefitOptions = true;
            }

            #region UVHP Benefit Options

            if (lblnApplyBenefitOptions)
            {

                decimal ldecTotalUVHP = GetTotalUVHPAmountForCalculation(aintPersonAccountId);
                decimal ldecNonVestedEEAndUVHP = ldecTotalUVHP;
                string lstrDROModel = string.Empty;

                if (ldecTotalUVHP != 0)
                {
                    ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.MPIPP_PLAN_ID, busConstant.MPIPP, iclbQdroCalculationDetail.Where(
                                            item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).
                                            First().icdoQdroCalculationDetail.iblnIsNewRecord, false, true);
                    lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                             item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_model_value;
                }

                if (GetUVHPContributionAmount() != 0)
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                             item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.total_uvhp_contribution_amount = GetUVHPContributionAmount();
                    

                    CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel,
                        false, true, false, false, true, false, false);
                }

                if (GetUVHPInterestAmount() != 0)
                {
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.total_uvhp_interest_amount = GetUVHPInterestAmount();
                  
                    CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction, lstrDROModel, false, true, false, false, false, true, false);
                }

                #region Lumpsum

                if (ldecTotalUVHP != 0)
                {
                    CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction,lstrDROModel, false, true, false, false, false, false, true);
                }
                #endregion

                #region LIFE ANNUITY

                CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction,lstrDROModel, false, true, false, false, false, true, false);

                if (ldecTotalUVHP != 0)
                    CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction,lstrDROModel, false, true, false, false, false, false, true);

                #endregion

            }
            #endregion
        }

        private void CalculateEEAmount(busPersonAccount abusPersonAccount)
        {
            bool lblnApplyBenefitOptions = false;
            decimal ldecAltPayeeFraction = 0;
            int lintDROApplicationDetailId = 0;
            DateTime ldtVestedDate = new DateTime();
            string lstrDROModel = string.Empty;

            if (iclbQdroCalculationDetail != null && iclbQdroCalculationDetail.Count > 0 &&
                (!iclbQdroCalculationDetail.Where(
                                     item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                     item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).IsNullOrEmpty()))
            {
                ldtVestedDate = iclbQdroCalculationDetail.Where(
                                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.vested_date;
            }
            else
            {
                //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP))
                {
                    ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    iblnIsParticipantVested = true;
                }
            }

            if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0 &&
                (!ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                       item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id &&
                       item.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES).IsNullOrEmpty()))
            {
                lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                   item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                       item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id &&
                       item.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES).First().icdoDroBenefitDetails.dro_benefit_id;
            }


            if (((icdoQdroCalculationHeader.qdro_application_id != 0 && lintDROApplicationDetailId > 0) || icdoQdroCalculationHeader.qdro_application_id == 0) &&
               ((!iblIsNew && iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                   item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).IsNullOrEmpty()) || iblIsNew))
            {
                busQdroCalculationDetail lbusQdroCalculationDetail = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };
                lbusQdroCalculationDetail.iobjMainCDO = lbusQdroCalculationDetail.icdoQdroCalculationDetail;

                lbusQdroCalculationDetail.LoadData(this.icdoQdroCalculationHeader.qdro_calculation_header_id, this.icdoQdroCalculationHeader.iintPlanId, busConstant.MPIPPEE,
                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                    this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id, ldtVestedDate,
                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.istrRetirementSubType, lintDROApplicationDetailId,
                    busConstant.FLAG_NO, busConstant.FLAG_YES, busConstant.FLAG_NO, busConstant.FLAG_NO, true);

                this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetail);
                lblnApplyBenefitOptions = true;
            }
            else if (!iblIsNew && !iclbQdroCalculationDetail.Where(
                                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).IsNullOrEmpty())
            {
                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).
                                FirstOrDefault().icdoQdroCalculationDetail.vested_date = ldtVestedDate;
                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).
                               FirstOrDefault().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).
                        FirstOrDefault().icdoQdroCalculationDetail.benefit_subtype_value =
                        this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).
                        FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                lblnApplyBenefitOptions = true;
            }

            if (lblnApplyBenefitOptions)
            {
                decimal ldecTotalEE = ibusCalculation.FetchEEAmountandInterest(true, this.iclbQdroCalculationDetail, null, null, this,
                                      abusPersonAccount, GetRetirementDateforCalculation(), iclbPersonAccountRetirementContribution, icdoQdroCalculationHeader.calculation_type_value);

                if (ldecTotalEE != 0)
                {
                    ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.MPIPP_PLAN_ID, busConstant.MPIPP, iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID
                        && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.iblnIsNewRecord, true);

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                            item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.early_reduced_benefit_amount = ldecTotalEE;
                    lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                            item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_model_value;

                    CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction,lstrDROModel, true, false, false, false, true, false, false);

                    CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction,lstrDROModel, true, false, false, false, false, true, false);
                }

                #region LUMP_SUM

                if (ldecTotalEE != 0)
                {
                    CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction,lstrDROModel,
                        true, false, false, false, false, false, true);
                }

                #endregion

                #region LIFE ANNUNITY

                if (ldecTotalEE != 0)
                    CalculateQDROFactorAndAmount(busConstant.LIFE_ANNUTIY, busConstant.MPIPP, busConstant.MPIPP_PLAN_ID, ldecAltPayeeFraction,lstrDROModel,
                                                    true, false, false, false, false, false, true);

                #endregion
            }

        }

        private void CalculateLocalIAP52SpecialAccount(cdoDummyWorkData acdoDummyWorkData)
        {
            bool lblnApplyBenefitOptions = false;
            decimal ldecAltPayeeFraction = 0, ldecLocal52SpecialAccountBalance = 0;
            int lintDROApplicationDetailId = 0, lintBalanceAsOfPlanYear = 0;
            DateTime ldtVestedDate = new DateTime();
            DateTime ldtNetInvestmentFromDate = new DateTime();
            DateTime ldtNetInvestmentToDate = new DateTime();

            if (iclbQdroCalculationDetail != null && iclbQdroCalculationDetail.Count > 0 &&
                (!iclbQdroCalculationDetail.Where(
                                     item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                     item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                     item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty()))
            {
                ldtVestedDate = iclbQdroCalculationDetail.Where(
                                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.vested_date;
            }
            else
            {
                //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                {
                    ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    iblnIsParticipantVestedinIAP = true;
                }
            }

            busQdroCalculationDetail lbusQdroCalculationDetail = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };

            lbusQdroCalculationDetail.iclbQdroCalculationOptions = new Collection<busQdroCalculationOptions>();
            lbusQdroCalculationDetail.iobjMainCDO = lbusQdroCalculationDetail.icdoQdroCalculationDetail;

            if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0 &&
                (!ibusQdroApplication.iclbDroBenefitDetails.Where(
                                            item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID && item.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES
                       && item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).IsNullOrEmpty()))
            {
                lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                                            item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID && item.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES
                       && item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().icdoDroBenefitDetails.dro_benefit_id;
            }

            if (((icdoQdroCalculationHeader.qdro_application_id != 0 && lintDROApplicationDetailId > 0) || icdoQdroCalculationHeader.qdro_application_id == 0) &&
               ((!iblIsNew && iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                   item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty()) || iblIsNew))
            {
                lbusQdroCalculationDetail.LoadData(this.icdoQdroCalculationHeader.qdro_calculation_header_id, busConstant.IAP_PLAN_ID, busConstant.IAPLOCAL52,
                    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.person_account_id,
                    ldtVestedDate, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.istrRetirementSubType, lintDROApplicationDetailId,
                    busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_YES, busConstant.FLAG_NO, true);

                this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetail);
                lblnApplyBenefitOptions = true;
            }
            else if (!iblIsNew && !iclbQdroCalculationDetail.Where(
                                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty())
            {
                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                               item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.vested_date = ldtVestedDate;

                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                               item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.benefit_subtype_value =
                                      this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().
                                      icdoPersonAccount.istrRetirementSubType;

                lblnApplyBenefitOptions = true;
            }


            if (lblnApplyBenefitOptions)
            {
                #region Get values for Local 52 Special Account Allocation

                if (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                               item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty())
                {
                    ldtNetInvestmentFromDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                               item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.net_investment_from_date;

                    ldtNetInvestmentToDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.net_investment_to_date;

                    lintBalanceAsOfPlanYear = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.balance_as_of_plan_year;
                }

                #endregion

                #region Special Account Allocation

                //if (!iblIsNew)
                //{
                bool lblnExecuteIAPQuaterlyAllocation = true;
                DateTime ldtDateForCalculationIAPBeneift = new DateTime();

                if (lintBalanceAsOfPlanYear != 0)
                {
                    ldtDateForCalculationIAPBeneift = busGlobalFunctions.GetLastDateOfComputationYear(lintBalanceAsOfPlanYear);
                }
                else
                {
                    ldtDateForCalculationIAPBeneift = GetRetirementDateforCalculation();
                }

                #region To Set Values for IAP QTR Allocations

                decimal ldecIAPHours4QtrAlloc = 0, ldecIAPHoursA2forQtrAlloc = 0, ldecIAPPercent4forQtrAlloc = 0;
                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                SqlParameter[] parameters = new SqlParameter[3];
                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
                SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);

                param1.Value = this.ibusParticipant.icdoPerson.istrSSNNonEncrypted;
                parameters[0] = param1;

                busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
                lbusIapAllocationSummary.LoadLatestAllocationSummary();

                param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year);
                parameters[1] = param2;

                param3.Value = busGlobalFunctions.GetLastDayOfWeek(GetRetirementDateforCalculation()); //PROD PIR 113
                parameters[2] = param3;

                DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPHourInfoForQuarterlyAllocation", astrLegacyDBConnetion, null, parameters);
                if (ldtbIAPInfo.Rows.Count > 0)
                {
                    //if (ldtbIAPInfo.Rows[0]["IAPHours"] != DBNull.Value)
                    //    ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHours"]);

                    //if (ldtbIAPInfo.Rows[0]["IAPHoursA2"] != DBNull.Value)
                    //    ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHoursA2"]);

                    //if (ldtbIAPInfo.Rows[0]["IAPPercent"] != DBNull.Value)
                    //    ldecIAPPercent4forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPPercent"]);

                    if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")) > 0)
                        ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")));

                    if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")) > 0)
                        ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")));

                    DataTable ldtIAPFiltered;
                    busIAPAllocationHelper aobjIAPAllocationHelper = new busIAPAllocationHelper();
                    foreach (DataRow ldrIAPPercent in ldtbIAPInfo.Rows)
                    {
                        if (ldrIAPPercent["IAPPercent"] != DBNull.Value && Convert.ToString(ldrIAPPercent["IAPPercent"]).IsNotNullOrEmpty() &&
                            Convert.ToDecimal(ldrIAPPercent["IAPPercent"]) > 0)
                        {
                            ldtIAPFiltered = new DataTable();
                            ldtIAPFiltered = ldtbIAPInfo.AsEnumerable().Where(o => o.Field<Int16?>("ComputationYear") == Convert.ToInt16(ldrIAPPercent["ComputationYear"])
                                && o.Field<int?>("EmpAccountNo") == Convert.ToInt32(ldrIAPPercent["EmpAccountNo"])).CopyToDataTable();

                            ldecIAPPercent4forQtrAlloc += aobjIAPAllocationHelper.CalculateAllocation4Amount(Convert.ToInt32(ldrIAPPercent["ComputationYear"]), ldtIAPFiltered);
                        }

                    }
                }
                #endregion

                if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                {
                    if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                    {
                        int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id;
                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                        {
                            if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE &&
                                (lintBalanceAsOfPlanYear != 0 && lintBalanceAsOfPlanYear <= icdoQdroCalculationHeader.qdro_commencement_date.Year && (ldtNetInvestmentFromDate != DateTime.MinValue && ldtNetInvestmentToDate != DateTime.MinValue)))
                            {
                                lblnExecuteIAPQuaterlyAllocation = false;
                            }

                            ibusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_TRUE, this.iclbQdroCalculationDetail, null, null, this, ldtDateForCalculationIAPBeneift, ldecIAPHours4QtrAlloc,
                                ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, lblnExecuteIAPQuaterlyAllocation, true, aintQDROBalanceAsOfYear: lintBalanceAsOfPlanYear); //PIR:1015
                        }
                    }
                }
                //}

                #endregion

                if (iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                {
                    ldecLocal52SpecialAccountBalance = iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.accrued_benefit_amt;
                }
                else
                {
                    ldecLocal52SpecialAccountBalance = iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                            item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.early_reduced_benefit_amount;
                }

                busQdroCalculationOptions lbusQdroCalculationOptions = new busQdroCalculationOptions { icdoQdroCalculationOptions = new cdoQdroCalculationOptions() };
                DataTable ldtbLocal52Balance;
                if (ldtNetInvestmentFromDate != DateTime.MinValue && ldtNetInvestmentToDate != DateTime.MinValue)
                {
                    #region Logic to show Participant benefit amount. Applicable only for IAP allocation

                    int lintComputationYear = 0;
                    decimal ldecParticipantLocal52Bal = 0;
                    if (icdoQdroCalculationHeader.benefit_comencement_date != DateTime.MinValue)
                    {
                        lintComputationYear = icdoQdroCalculationHeader.benefit_comencement_date.Year;
                    }
                    else
                    {
                        lintComputationYear = DateTime.Now.Year;
                    }

                    ldtbLocal52Balance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                     new object[2] { iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID 
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.person_account_id, 
                                    lintComputationYear});

                    if (ldtbLocal52Balance.Rows.Count > 0 && (!string.IsNullOrEmpty(ldtbLocal52Balance.Rows[0][enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()].ToString())))
                        ldecParticipantLocal52Bal = Convert.ToDecimal(ldtbLocal52Balance.Rows[0][enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()]);

                    #endregion


                    ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.IAP_PLAN_ID, busConstant.IAP, iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                      item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.iblnIsNewRecord, false, false, true, false);
                    ldecLocal52SpecialAccountBalance = CalculateIAPAllocation(ldecLocal52SpecialAccountBalance, ldtNetInvestmentFromDate, ldtNetInvestmentToDate, true);

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                            && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.participant_benefit_amount =
                                    ldecParticipantLocal52Bal - ldecLocal52SpecialAccountBalance;

                    #region Logic to show alternate payee amount if it exceeds participants benefit

                    if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                            && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.alt_payee_benefit_cap_year != 0)
                    {
                        DataTable ldtbParticipantLocal52Balance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                          new object[2] { iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID  
                              && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.person_account_id, 
                                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                        && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.alt_payee_benefit_cap_year});

                        if (ldtbParticipantLocal52Balance.IsNotNull() && ldtbParticipantLocal52Balance.Rows.Count > 0)
                        {
                            decimal ldecParticipant161Balance = Convert.ToDecimal(Convert.ToBoolean(ldtbParticipantLocal52Balance.Rows[0][1].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbParticipantLocal52Balance.Rows[0][1]);
                            if (ldecParticipant161Balance < ldecLocal52SpecialAccountBalance)
                            {
                                ldecLocal52SpecialAccountBalance = ldecParticipant161Balance;
                            }
                        }
                    }

                    #endregion

                    lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.LUMP_SUM);
                    lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM), 1,
                                                                    ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLocal52SpecialAccountBalance, false, false, true, false);
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);

                }
                else
                {
                    ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.IAP_PLAN_ID, busConstant.IAP, iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                       item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.iblnIsNewRecord, false, false, true, false);

                    lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.LUMP_SUM);

                    string lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_model_value;

                    CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.IAP, busConstant.IAP_PLAN_ID, ldecAltPayeeFraction,lstrDROModel,
                                                    false, false, true, false, false, false, true);
                    lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM), 1,
                    ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id,
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.alt_payee_amt_before_conversion,
                    false, false, true, false);
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                }

                if (acdoDummyWorkData != null)
                {
                    busQdroCalculationYearlyDetail lbusQdroCalculationYearlyDetail = new busQdroCalculationYearlyDetail();

                    lbusQdroCalculationYearlyDetail.LoadData(acdoDummyWorkData.qualified_hours, acdoDummyWorkData.bis_years_count, acdoDummyWorkData.year,
                                                                acdoDummyWorkData.qualified_years_count, acdoDummyWorkData.vested_hours, acdoDummyWorkData.vested_years_count, acdoDummyWorkData.idecBenefitRate,
                                                                acdoDummyWorkData.idecBenefitAmount, acdoDummyWorkData.idecTotalHealthHours, acdoDummyWorkData.iintHealthCount, 0,
                                                                acdoDummyWorkData.idecQdroHours, idecThru79Hours, ldecLocal52SpecialAccountBalance);
                    iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                        item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbQdroCalculationYearlyDetail.Add(lbusQdroCalculationYearlyDetail);
                }
            }
        }

        private void CalculateLocalIAP5161pecialAccount(cdoDummyWorkData acdoDummyWorkData)
        {
            bool lblnApplyBenefitOptions = false;
            decimal ldecAltPayeeFraction = 0, ldecLocal161SpecialAccountBalance = 0;
            int lintDROApplicationDetailId = 0, lintBalanceAsOfPlanYear = 0;
            DateTime ldtVestedDate = new DateTime();
            DateTime ldtNetInvestmentFromDate = new DateTime();
            DateTime ldtNetInvestmentToDate = new DateTime();

            if (iclbQdroCalculationDetail != null && iclbQdroCalculationDetail.Count > 0 &&
                (!iclbQdroCalculationDetail.Where(
                                     item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                     item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                     item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty()))
            {
                ldtVestedDate = iclbQdroCalculationDetail.Where(
                                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                       item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.vested_date;
            }
            else
            {
                //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                {
                    ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    iblnIsParticipantVestedinIAP = true;
                }
            }

            busQdroCalculationDetail lbusQdroCalculationDetail = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };

            lbusQdroCalculationDetail.iclbQdroCalculationOptions = new Collection<busQdroCalculationOptions>();
            lbusQdroCalculationDetail.iobjMainCDO = lbusQdroCalculationDetail.icdoQdroCalculationDetail;

            if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0 &&
                (!ibusQdroApplication.iclbDroBenefitDetails.Where(
                                            item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID && item.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES
                       && item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).IsNullOrEmpty()))
            {
                lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                                            item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID && item.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES
                       && item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().icdoDroBenefitDetails.dro_benefit_id;
            }

            if (((icdoQdroCalculationHeader.qdro_application_id != 0 && lintDROApplicationDetailId > 0) || icdoQdroCalculationHeader.qdro_application_id == 0) &&
               ((!iblIsNew && iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                   item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty()) || iblIsNew))
            {
                lbusQdroCalculationDetail.LoadData(this.icdoQdroCalculationHeader.qdro_calculation_header_id, busConstant.IAP_PLAN_ID, busConstant.IAPLOCAL161,
                    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.person_account_id,
                    ldtVestedDate, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.istrRetirementSubType, lintDROApplicationDetailId,
                    busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_YES, true);

                this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetail);
                lblnApplyBenefitOptions = true;
            }
            else if (!iblIsNew && !iclbQdroCalculationDetail.Where(
                                       item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty())
            {
                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                               item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.vested_date = ldtVestedDate;

                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                               item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.benefit_subtype_value =
                                      this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().
                                      icdoPersonAccount.istrRetirementSubType;

                lblnApplyBenefitOptions = true;
            }


            if (lblnApplyBenefitOptions)
            {
                #region Get values for Local 161 Special Account Allocation

                if (!this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                               item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).IsNullOrEmpty())
                {
                    ldtNetInvestmentFromDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                               item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.net_investment_from_date;

                    ldtNetInvestmentToDate = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.net_investment_to_date;

                    lintBalanceAsOfPlanYear = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.balance_as_of_plan_year;
                }

                #endregion

                #region Special Account Allocation

                //if (!iblIsNew)
                //{
                bool lblnExecuteIAPQuaterlyAllocation = true;
                DateTime ldtDateForCalculationIAPBeneift = new DateTime();

                if (lintBalanceAsOfPlanYear != 0)
                {
                    ldtDateForCalculationIAPBeneift = busGlobalFunctions.GetLastDateOfComputationYear(lintBalanceAsOfPlanYear);
                }
                else
                {
                    ldtDateForCalculationIAPBeneift = GetRetirementDateforCalculation();
                }

                #region To Set Values for IAP QTR Allocations

                decimal ldecIAPHours4QtrAlloc = 0, ldecIAPHoursA2forQtrAlloc = 0, ldecIAPPercent4forQtrAlloc = 0;
                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                SqlParameter[] parameters = new SqlParameter[3];
                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
                SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);

                param1.Value = this.ibusParticipant.icdoPerson.istrSSNNonEncrypted;
                parameters[0] = param1;

                busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
                lbusIapAllocationSummary.LoadLatestAllocationSummary();

                param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year);
                parameters[1] = param2;

                param3.Value = busGlobalFunctions.GetLastDayOfWeek(GetRetirementDateforCalculation());//PROD PIR 113
                parameters[2] = param3;

                DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPHourInfoForQuarterlyAllocation", astrLegacyDBConnetion, null, parameters);
                if (ldtbIAPInfo.Rows.Count > 0)
                {
                    //if (ldtbIAPInfo.Rows[0]["IAPHours"] != DBNull.Value)
                    //    ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHours"]);

                    //if (ldtbIAPInfo.Rows[0]["IAPHoursA2"] != DBNull.Value)
                    //    ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPHoursA2"]);

                    //if (ldtbIAPInfo.Rows[0]["IAPPercent"] != DBNull.Value)
                    //    ldecIAPPercent4forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.Rows[0]["IAPPercent"]);

                    if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")) > 0)
                        ldecIAPHours4QtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHours")));

                    if (ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")) > 0)
                        ldecIAPHoursA2forQtrAlloc = Convert.ToDecimal(ldtbIAPInfo.AsEnumerable().Sum(item => item.Field<decimal>("IAPHoursA2")));

                    DataTable ldtIAPFiltered;
                    busIAPAllocationHelper aobjIAPAllocationHelper = new busIAPAllocationHelper();
                    foreach (DataRow ldrIAPPercent in ldtbIAPInfo.Rows)
                    {
                        if (ldrIAPPercent["IAPPercent"] != DBNull.Value && Convert.ToString(ldrIAPPercent["IAPPercent"]).IsNotNullOrEmpty() &&
                            Convert.ToDecimal(ldrIAPPercent["IAPPercent"]) > 0)
                        {
                            ldtIAPFiltered = new DataTable();
                            ldtIAPFiltered = ldtbIAPInfo.AsEnumerable().Where(o => o.Field<Int16?>("ComputationYear") == Convert.ToInt16(ldrIAPPercent["ComputationYear"])
                                && o.Field<int?>("EmpAccountNo") == Convert.ToInt32(ldrIAPPercent["EmpAccountNo"])).CopyToDataTable();

                            ldecIAPPercent4forQtrAlloc += aobjIAPAllocationHelper.CalculateAllocation4Amount(Convert.ToInt32(ldrIAPPercent["ComputationYear"]), ldtIAPFiltered);
                        }

                    }
                }
                #endregion

                if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                {
                    if (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                    {
                        int lintPersonAccountId = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id;
                        if (this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountId).Count() > 0)
                        {
                            if (icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE &&
                                (lintBalanceAsOfPlanYear != 0 && lintBalanceAsOfPlanYear <= icdoQdroCalculationHeader.qdro_commencement_date.Year && (ldtNetInvestmentFromDate != DateTime.MinValue && ldtNetInvestmentToDate != DateTime.MinValue)))
                            {
                                lblnExecuteIAPQuaterlyAllocation = false;
                            }

                            ibusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_TRUE, this.iclbQdroCalculationDetail, null, null, this, ldtDateForCalculationIAPBeneift, ldecIAPHours4QtrAlloc,
                                ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc, lblnExecuteIAPQuaterlyAllocation, false, true, aintQDROBalanceAsOfYear: lintBalanceAsOfPlanYear);//PIR - 1015
                        }
                    }
                }
                //}

                #endregion

                busQdroCalculationOptions lbusQdroCalculationOptions = new busQdroCalculationOptions { icdoQdroCalculationOptions = new cdoQdroCalculationOptions() };
                DataTable ldtbLocal161Balance;
                if (ldtNetInvestmentFromDate != DateTime.MinValue && ldtNetInvestmentToDate != DateTime.MinValue)
                {
                    #region Logic to show Participant benefit amount. Applicable only for IAP allocation

                    int lintComputationYear = 0;
                    decimal ldecParticipantLocal161Bal = 0;
                    if (icdoQdroCalculationHeader.benefit_comencement_date != DateTime.MinValue)
                    {
                        lintComputationYear = icdoQdroCalculationHeader.benefit_comencement_date.Year;
                    }
                    else
                    {
                        lintComputationYear = DateTime.Now.Year;
                    }

                    ldtbLocal161Balance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                     new object[2] { iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID 
                                    && item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.person_account_id, 
                                    lintComputationYear});

                    if (ldtbLocal161Balance.Rows.Count > 0 && (!string.IsNullOrEmpty(ldtbLocal161Balance.Rows[0][enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()].ToString())))
                        ldecParticipantLocal161Bal = Convert.ToDecimal(ldtbLocal161Balance.Rows[0][enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()]);

                    #endregion

                    if (iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.accrued_benefit_amt != 0)
                    {
                        ldecLocal161SpecialAccountBalance = iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.accrued_benefit_amt;
                    }
                    else
                    {
                        ldecLocal161SpecialAccountBalance = iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.early_reduced_benefit_amount;
                    }
                    ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.IAP_PLAN_ID, busConstant.IAP, iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                      item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.iblnIsNewRecord, false, false, false, true);
                    ldecLocal161SpecialAccountBalance = CalculateIAPAllocation(ldecLocal161SpecialAccountBalance, ldtNetInvestmentFromDate, ldtNetInvestmentToDate, false, true);

                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                            && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.participant_benefit_amount =
                                    ldecParticipantLocal161Bal - ldecLocal161SpecialAccountBalance;

                    #region Logic to show alternate payee amount if it exceeds participants benefit

                    if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                            && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.alt_payee_benefit_cap_year != 0)
                    {
                        DataTable ldtbParticipantLocal161Balance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                          new object[2] { iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID  
                              && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.person_account_id, 
                                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                        && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.alt_payee_benefit_cap_year});

                        if (ldtbParticipantLocal161Balance.IsNotNull() && ldtbParticipantLocal161Balance.Rows.Count > 0)
                        {
                            decimal ldecParticipant161Balance = Convert.ToDecimal(Convert.ToBoolean(ldtbParticipantLocal161Balance.Rows[0][2].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbParticipantLocal161Balance.Rows[0][2]);
                            if (ldecParticipant161Balance < ldecLocal161SpecialAccountBalance)
                            {
                                ldecLocal161SpecialAccountBalance = ldecParticipant161Balance;
                            }
                        }
                    }

                    #endregion

                    lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.LUMP_SUM);
                    lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM), 1,
                                                                    ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, ldecLocal161SpecialAccountBalance, false, false, false, true);
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID
                                && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);

                }
                else
                {
                    ldecAltPayeeFraction = CalculateAltPayeeFraction(busConstant.IAP_PLAN_ID, busConstant.IAP, iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                       item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.iblnIsNewRecord, false, false, false, true);

                    string lstrDROModel = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                                       item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.qdro_model_value;

                    lbusQdroCalculationOptions.icdoQdroCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.LUMP_SUM);

                    CalculateQDROFactorAndAmount(busConstant.LUMP_SUM, busConstant.IAP, busConstant.IAP_PLAN_ID, ldecAltPayeeFraction,lstrDROModel,
                                                    false, false, false, true, false, false, true);
                    lbusQdroCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM), 1,
                    ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id,
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().icdoQdroCalculationDetail.alt_payee_amt_before_conversion,
                    false, false, false, true);
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).First().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
                }

                if (acdoDummyWorkData != null)
                {
                    busQdroCalculationYearlyDetail lbusQdroCalculationYearlyDetail = new busQdroCalculationYearlyDetail();

                    lbusQdroCalculationYearlyDetail.LoadData(acdoDummyWorkData.qualified_hours, acdoDummyWorkData.bis_years_count, acdoDummyWorkData.year,
                                                                acdoDummyWorkData.qualified_years_count, acdoDummyWorkData.vested_hours, acdoDummyWorkData.vested_years_count, acdoDummyWorkData.idecBenefitRate,
                                                                acdoDummyWorkData.idecBenefitAmount, acdoDummyWorkData.idecTotalHealthHours, acdoDummyWorkData.iintHealthCount, 0,
                                                                acdoDummyWorkData.idecQdroHours, idecThru79Hours, ldecLocal161SpecialAccountBalance);
                    iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                        item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbQdroCalculationYearlyDetail.Add(lbusQdroCalculationYearlyDetail);
                }
            }
        }

        #endregion

        #endregion

        #region Overriden Methods
        //CODE - AARTI
        public override busBase GetCorPerson()
        {
            ibusParticipant.LoadPersonAddresss();
            ibusParticipant.LoadPersonContacts();
            ibusParticipant.LoadCorrAddress();
            return this.ibusParticipant;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);

            if (astrTemplateName != busConstant.DRO_LUMP_SUM_DISTRIBUTION_ELECTION)
                FillBenefitAmountsForCorrespondence(astrTemplateName);

            this.istrApprovedByUser = this.ibusQdroApplication.icdoDroApplication.approved_by_user;
            string Firstinitial = string.Empty;
            string SecondInitial = string.Empty;

            istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(System.DateTime.Now);

            if (!string.IsNullOrEmpty(this.ibusQdroApplication.istrApprovedByUser))
            {
                if (this.istrApprovedByUser.Contains(" ") || this.istrApprovedByUser.Contains("."))
                {
                    string[] split = this.ibusQdroApplication.istrApprovedByUser.Split(new Char[] { ' ', '.' });
                    if (split.Length > 0 && !string.IsNullOrEmpty(split[0]))
                        Firstinitial = split[0].Substring(0, 1);
                    if (split.Length > 1 && !string.IsNullOrEmpty(split[1]))
                        SecondInitial = split[1].Substring(0, 1);

                    this.istrApprovedByUserInitials = Firstinitial.ToUpper() + SecondInitial.ToUpper();
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.ibusQdroApplication.istrApprovedByUser))
                    {
                        this.istrApprovedByUserInitials = this.istrApprovedByUser.Substring(0, 2).ToUpper();
                    }
                }
            }

            foreach (busPersonContact lbusPersonContact in this.ibusParticipant.iclbPersonContact)
            {
                if (lbusPersonContact.icdoPersonContact.contact_type_value == "ATRN")
                {
                    if ((lbusPersonContact.icdoPersonContact.effective_end_date == DateTime.MinValue && lbusPersonContact.icdoPersonContact.effective_start_date <= DateTime.Today) ||
                        lbusPersonContact.icdoPersonContact.effective_start_date <= DateTime.Today && lbusPersonContact.icdoPersonContact.effective_end_date > DateTime.Today)
                    {
                        this.istrAttorneyName = lbusPersonContact.icdoPersonContact.contact_name;

                    }
                }
            }

            if (astrTemplateName == busConstant.JOINDER_COVER_LETTER_TO_COURT || astrTemplateName == busConstant.NOTICE_OF_APPEARANCE_AND_RESPONSE_OF_EMPLOYEE_BENEFIT_PLAN_JOINDER || astrTemplateName == busConstant.QDRO_ALTERNATE_PAYEE_PENSION_PACKAGE_COVER_LETTER || astrTemplateName == busConstant.QDRO_PENSION_BENEFIT_ELECTION_FORM_ALTERNATE_PAYEE_RETIREMENT_BENEFIT_ELECTION_FORM_QDRO
                || astrTemplateName == busConstant.QDRO_UVHP_PACKAGE_COVER_LETTER || astrTemplateName == busConstant.QDRO_ALTERNATE_PAYEE_IAP_PACKAGE_COVER_LETTER||astrTemplateName == busConstant.QDRO_BENEFIT_ELECTION_PACKET || astrTemplateName == busConstant.QDRO_IAP_SPECIAL_ACCOUNT_PACKET)
            {
                this.iintyear = DateTime.Now.Year;
                istrCommencementDate = busGlobalFunctions.ConvertDateIntoDifFormat(icdoQdroCalculationHeader.benefit_comencement_date);
                this.ibusParticipant.LoadCourtAddress();
                this.ibusAlternatePayee.LoadInitialData();
                this.ibusAlternatePayee.LoadPersonAddresss();
                this.ibusAlternatePayee.LoadPersonContacts();
                this.ibusAlternatePayee.LoadCorrAddress();
                this.istrParticipantFullName = ibusParticipant.icdoPerson.first_name + " " + ibusParticipant.icdoPerson.last_name;
                if (Convert.ToInt32(this.ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.USA)
                {
                    istrIsUSA = "1";
                }
            }
            if (astrTemplateName == busConstant.QDRO_ALTERNATE_PAYEE_PENSION_PACKAGE_COVER_LETTER || astrTemplateName == busConstant.QDRO_BENEFIT_ELECTION_PACKET || astrTemplateName == busConstant.QDRO_IAP_SPECIAL_ACCOUNT_PACKET)
            {
                this.iintyear = DateTime.Now.Year;
                if (this.iblnParticipantPayeeAccount)
                    istrIsRetireeModel = busConstant.FLAG_YES;
                else if (this.iclbQdroCalculationDetail.Count > 0 && this.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                {
                    if (this.iclbQdroCalculationDetail.Where(item => !(item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                        && !(item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES)).Count() > 0)
                    {
                        //If Retiree Model Only 1 option will be available in estimates
                        if (this.iclbQdroCalculationDetail.Where(item => !(item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                          && !(item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES)).FirstOrDefault().iclbQdroCalculationOptions.Count == 1)
                        {
                            CheckIfSharedInterestDro();
                            if (this.iblnParticipantPayeeAccount)
                                istrIsRetireeModel = busConstant.FLAG_YES;
                        }
                    }
                }
            }
            if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
            {
                istrBenefitSubType = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoQdroCalculationDetail.benefit_subtype_description;
                dtEstimateRetrDate = this.GetRetirementDateforCalculation();
            }
            if (this.icdoQdroCalculationHeader.date_of_seperation != DateTime.MinValue)
            {
                iintyear = this.icdoQdroCalculationHeader.date_of_seperation.Year;
            }
            if (astrTemplateName == busConstant.QDRO_ALTERNATE_PAYEE_PENSION_PACKAGE_COVER_LETTER || astrTemplateName == busConstant.QDRO_PENSION_BENEFIT_ELECTION_FORM_ALTERNATE_PAYEE_RETIREMENT_BENEFIT_ELECTION_FORM_QDRO || astrTemplateName == busConstant.QDRO_BENEFIT_ELECTION_PACKET || astrTemplateName == busConstant.QDRO_IAP_SPECIAL_ACCOUNT_PACKET)
            {
                this.iintyear = DateTime.Now.Year;
                istrCommencementDate = busGlobalFunctions.ConvertDateIntoDifFormat(icdoQdroCalculationHeader.benefit_comencement_date);
                idtCommencementDate = GetRetirementDateforCalculation();
                istrBeforeCommencementDate = Convert.ToString(busGlobalFunctions.ConvertDateIntoDifFormat(idtCommencementDate.AddDays(-1)));
            }
            istrCurrentDateInDiffFormat = busGlobalFunctions.ConvertDateIntoDifFormat(DateTime.Today);

            if (astrTemplateName == busConstant.QDRO_UVHP_PACKAGE_COVER_LETTER || astrTemplateName == busConstant.QDRO_BENEFIT_ELECTION_PACKET || astrTemplateName == busConstant.QDRO_IAP_SPECIAL_ACCOUNT_PACKET)
            {
                this.iintyear = DateTime.Now.Year;

                foreach (busQdroCalculationDetail lbusQdroCalculationDetail in this.iclbQdroCalculationDetail)
                {
                    idecUVHPContri = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_uvhp;
                    idecUVHPInt = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_uvhp_interest;
                    idecUVHPTotal = idecUVHPContri + idecUVHPInt;
                }
            }
            if (astrTemplateName == busConstant.DRO_LUMP_SUM_DISTRIBUTION_ELECTION || astrTemplateName == busConstant.QDRO_BENEFIT_ELECTION_PACKET || astrTemplateName == busConstant.QDRO_IAP_SPECIAL_ACCOUNT_PACKET)
            {
                this.iintyear = DateTime.Now.Year;
                FillLumpSumDistributionAmount();

                iintLumpSumYearByAge = GetRetirementDateforCalculation().Year;

                if (istrPlan.IsNotNullOrEmpty() && istrPlan.Contains(busConstant.IAP) && this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES && item.icdoQdroCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES).Count() > 0 &&
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES &&
                    item.icdoQdroCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.balance_as_of_plan_year != 0)
                {

                    iintLumpSumYearByAge = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES &&
                    item.icdoQdroCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.balance_as_of_plan_year;

                }
                else if (istrPlan.IsNotNullOrEmpty() && istrPlan.Contains(busConstant.IAP) && this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).Count() > 0 &&
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.balance_as_of_plan_year != 0)
                {
                    iintLumpSumYearByAge = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.balance_as_of_plan_year;

                }
                else if (istrPlan.IsNotNullOrEmpty() && istrPlan.Contains(busConstant.IAP) && this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).Count() > 0 &&
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.balance_as_of_plan_year != 0)
                {
                    iintLumpSumYearByAge = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationDetail.balance_as_of_plan_year;

                }
                //ProdFix_02/07/2013_PIR-6
                ibusParticipant = ibusAlternatePayee;
            }

            istrParticipantMPIID = ibusParticipant.icdoPerson.mpi_person_id;
        }

        void FillBenefitAmountsForCorrespondence(string astrTemplateName)
        {
            Collection<busQdroCalculationOptions> lclbDroOptions = new Collection<busQdroCalculationOptions>();
            if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && !(item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES) && !(item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)).Count() > 0)
            {
                idecTotalAccruedBenefit = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && !(item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES) && !(item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)).First().icdoQdroCalculationDetail.early_reduced_benefit_amount;
                idecParticipantAccruedBenefit = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && !(item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES) && !(item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)).FirstOrDefault().icdoQdroCalculationDetail.participant_benefit_amount;
                idecAlternatePayeeBenefitBeforeConversion = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && !(item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES) && !(item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)).FirstOrDefault().icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                lclbDroOptions = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID && !(item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES) && !(item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)).First().iclbQdroCalculationOptions.ToList().ToCollection();
                
                if (lclbDroOptions != null && lclbDroOptions.Count > 0)
                {

                    if (lclbDroOptions.Where(option => option.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE).Count() > 0)
                    {
                        idecMPIAlternatePayeeAmount = lclbDroOptions.Where(option => option.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE).FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount;
                        idecNetTotalBenefit = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.istrPlanCode == busConstant.MPIPP).FirstOrDefault().icdoQdroCalculationDetail.unreduced_benefit_amount;
                        idecMPIParticipantAmount = idecNetTotalBenefit - idecMPIAlternatePayeeAmount;
                    }

                    if (lclbDroOptions.Where(option => option.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY).Count() > 0)
                    {
                        idecMPIAltPayeeAmtTenYearCerAndLife = lclbDroOptions.Where(option => option.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY).FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount;
                    }                    
                    if (idecMPIAlternatePayeeAmount == decimal.Zero && this.iblnParticipantPayeeAccount)
                    {
                        idecMPIAlternatePayeeAmount = lclbDroOptions.FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount;
                    }
                }
            }
            if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
            {
                lclbDroOptions = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().iclbQdroCalculationOptions.ToList().ToCollection();
                if (lclbDroOptions != null && lclbDroOptions.Count > 0)
                {
                    if (lclbDroOptions.Where(option => option.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE).Count() > 0)
                    {
                        idecIAPAlternatePayeeAmount = lclbDroOptions.Where(option => option.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value == busConstant.LIFE).FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount;
                        idecIAPParticipantAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoQdroCalculationDetail.unreduced_benefit_amount;
                        idecIAPParticipantAmount = idecMPIParticipantAmount - idecMPIAlternatePayeeAmount;
                    }
                    if (this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && !(item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                        && !(item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)).Count() > 0)
                    {
                        iintIAPAsOfYear =  this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && !(item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                             && !(item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)).First().icdoQdroCalculationDetail.balance_as_of_plan_year;
                        if (iintIAPAsOfYear == 0)
                        {
                            iintIAPAsOfYear = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && !(item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                 && !(item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)).First().icdoQdroCalculationDetail.iap_as_of_date.Year;
                        }
                        idecTotalIAPBalanceAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && !(item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                             && !(item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)).First().icdoQdroCalculationDetail.early_reduced_benefit_amount;

                        idecParticipantIAPBalanceAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && !(item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                             && !(item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)).First().icdoQdroCalculationDetail.participant_benefit_amount;

                        idecIAPAlternatePayeeLumpSumAmount = this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && !(item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                            && !(item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)).First().icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                        if (idecIAPAlternatePayeeLumpSumAmount > 200)
                        {
                            istrIsAmountTaxabale = busConstant.FLAG_YES;
                        }
                    }
                }
            }

        }

        private void FillLumpSumDistributionAmount()
        {
            int lintPlanBenefitId = 0; 
            decimal ldecLumpSumBenefitAmount = 0;

            if (istrPlan.IsNullOrEmpty())
            {
                if (icdoQdroCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID || icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID ||
                    icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID || icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID ||
                    icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID || icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID ||
                    icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                {
                    istrPlan = busConstant.MPIPP;
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
                {
                    istrPlan = busConstant.IAP;
                }
            }

            if (istrPlan.IsNotNullOrEmpty() && (istrPlan.Contains(busConstant.MPIPP) || istrPlan.Contains("EE") || istrPlan.Contains("UVHP")))
            {
                Collection<busQdroCalculationOptions> lclbQdroCalculationOptions = new Collection<busQdroCalculationOptions>();

                #region set Benefit option depending on plan

                if (icdoQdroCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID && !istrPlan.Contains("EE") && !istrPlan.Contains("UVHP") &&
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                            item.icdoQdroCalculationDetail.ee_flag != busConstant.FLAG_YES && item.icdoQdroCalculationDetail.uvhp_flag != busConstant.FLAG_YES).Count() > 0)
                {
                    lclbQdroCalculationOptions =
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                            item.icdoQdroCalculationDetail.ee_flag != busConstant.FLAG_YES && item.icdoQdroCalculationDetail.uvhp_flag != busConstant.FLAG_YES).
                            FirstOrDefault().iclbQdroCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID && istrPlan.Contains("EE") &&
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                            item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).Count() > 0)
                {
                    lclbQdroCalculationOptions =
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                            item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES).FirstOrDefault().iclbQdroCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID && istrPlan.Contains("UVHP") &&
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).Count() > 0)
                {
                    lclbQdroCalculationOptions =
                        this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                            item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().iclbQdroCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_161_PLAN_ID)
                {
                    lclbQdroCalculationOptions =
                       this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).FirstOrDefault().iclbQdroCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_600_PLAN_ID)
                {
                    lclbQdroCalculationOptions =
                       this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).FirstOrDefault().iclbQdroCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_666_PLAN_ID)
                {
                    lclbQdroCalculationOptions =
                       this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).FirstOrDefault().iclbQdroCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_52_PLAN_ID)
                {
                    lclbQdroCalculationOptions =
                       this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).FirstOrDefault().iclbQdroCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LUMP_SUM);
                }
                else if (icdoQdroCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                {
                    lclbQdroCalculationOptions =
                       this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).FirstOrDefault().iclbQdroCalculationOptions.ToList().ToCollection();

                    lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM);
                }

                #endregion       

                if (lclbQdroCalculationOptions.Where(option => option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoQdroCalculationOptions.ee_flag.IsNullOrEmpty() &&
                            option.icdoQdroCalculationOptions.uvhp_flag.IsNullOrEmpty()).Count() > 0 && istrPlan.Contains("MPIPP"))
                {
                    ldecLumpSumBenefitAmount = lclbQdroCalculationOptions.Where(option =>
                          option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId && option.icdoQdroCalculationOptions.ee_flag != busConstant.FLAG_YES &&
                          option.icdoQdroCalculationOptions.uvhp_flag != busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount;
                    istrLumpSumBenefitAmount = AppendDoller(ldecLumpSumBenefitAmount);
                }
                else if (lclbQdroCalculationOptions.Where(option => option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                           option.icdoQdroCalculationOptions.uvhp_flag == busConstant.FLAG_YES).Count() > 0 && istrPlan.Contains("UVHP"))
                {
                    ldecLumpSumBenefitAmount = lclbQdroCalculationOptions.Where(option =>
                          option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                          option.icdoQdroCalculationOptions.uvhp_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount;
                    istrLumpSumBenefitAmount = AppendDoller(ldecLumpSumBenefitAmount);
                }
                else if (lclbQdroCalculationOptions.Where(option => option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                           option.icdoQdroCalculationOptions.ee_flag == busConstant.FLAG_YES).Count() > 0 && istrPlan.Contains("EE"))
                {
                    ldecLumpSumBenefitAmount = lclbQdroCalculationOptions.Where(option =>
                          option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                          option.icdoQdroCalculationOptions.ee_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount;
                    istrLumpSumBenefitAmount = AppendDoller(ldecLumpSumBenefitAmount);
                }
            }
            else if (istrPlan.IsNotNullOrEmpty() && istrPlan.Contains(busConstant.IAP) &&
                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                        item.icdoQdroCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES &&
                        item.icdoQdroCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES).Count() > 0)
            {
                Collection<busQdroCalculationOptions> lclbQdroCalculationOptions = new Collection<busQdroCalculationOptions>();

                lclbQdroCalculationOptions =
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                        item.icdoQdroCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES && 
                        item.icdoQdroCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES).FirstOrDefault().iclbQdroCalculationOptions.ToList().ToCollection();

                lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                if (lclbQdroCalculationOptions.Where(option => option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                        option.icdoQdroCalculationOptions.l161_spl_acc_flag != busConstant.FLAG_YES &&
                        option.icdoQdroCalculationOptions.l52_spl_acc_flag != busConstant.FLAG_YES).Count() > 0)
                {
                    ldecLumpSumBenefitAmount = lclbQdroCalculationOptions.Where(option => option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                        option.icdoQdroCalculationOptions.l161_spl_acc_flag != busConstant.FLAG_YES &&
                        option.icdoQdroCalculationOptions.l161_spl_acc_flag != busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount;
                    istrLumpSumBenefitAmount = AppendDoller(ldecLumpSumBenefitAmount);

                }
            }
            else if (istrPlan.IsNotNullOrEmpty() && istrPlan.Contains("161") &&
                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).Count() > 0)
            {
                Collection<busQdroCalculationOptions> lclbQdroCalculationOptions = new Collection<busQdroCalculationOptions>();

                lclbQdroCalculationOptions =
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbQdroCalculationOptions.ToList().ToCollection();

                lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                if (lclbQdroCalculationOptions.Where(option => option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoQdroCalculationOptions.l161_spl_acc_flag == busConstant.FLAG_YES).Count() > 0)
                {
                    ldecLumpSumBenefitAmount = lclbQdroCalculationOptions.Where(option => option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoQdroCalculationOptions.l161_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount;
                    istrLumpSumBenefitAmount = AppendDoller(ldecLumpSumBenefitAmount);

                }
            }
            else if (istrPlan.IsNotNullOrEmpty() && istrPlan.Contains("52") &&
                   this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                   item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).Count() > 0)
            {
                Collection<busQdroCalculationOptions> lclbQdroCalculationOptions = new Collection<busQdroCalculationOptions>();

                lclbQdroCalculationOptions =
                    this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                   item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().iclbQdroCalculationOptions.ToList().ToCollection();

                lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LUMP_SUM);
                if (lclbQdroCalculationOptions.Where(option => option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoQdroCalculationOptions.l52_spl_acc_flag == busConstant.FLAG_YES).Count() > 0)
                {
                    ldecLumpSumBenefitAmount = lclbQdroCalculationOptions.Where(option => option.icdoQdroCalculationOptions.plan_benefit_id == lintPlanBenefitId &&
                            option.icdoQdroCalculationOptions.l52_spl_acc_flag == busConstant.FLAG_YES).FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount;
                    istrLumpSumBenefitAmount = AppendDoller(ldecLumpSumBenefitAmount);

                }
            }
        }

        public string AppendDoller(decimal adecAmount)
        {
            if (adecAmount.IsNotNull() || adecAmount != 0)
            {
                string lstrAmount = String.Format("{0:c}", adecAmount);
                return lstrAmount;
            }
            else
            {
                return "n/a";
            }            
        }

        #endregion

        #region Methods used in validations

        /// <summary>
        /// Check if DRO Commencement date is Less than Earliest Retirement Date
        /// </summary>
        /// <returns></returns>
        public bool CheckCommnencementDateLessThanERD()
        {
            if (icdoQdroCalculationHeader.qdro_commencement_date != DateTime.MinValue &&
                icdoQdroCalculationHeader.qdro_commencement_date < icdoQdroCalculationHeader.retirement_date)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// check if Participant is eligible 
        /// </summary>
        /// <returns></returns>
        public bool CheckEligiblePlansForDisability()
        {
            if (icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES &&
                        (!ibusBenefitApplicationForDisability.iclbEligiblePlans.Contains(GetPlanCode())))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Benefit commencement date cannot be greater than min distribution date
        /// </summary>
        /// <returns></returns>
        public bool CheckValidBenefitCommencementDate()
        {
            if (icdoQdroCalculationHeader.age >= busConstant.BenefitCalculation.AGE_70_HALF &&
                (icdoQdroCalculationHeader.benefit_comencement_date != DateTime.MinValue || icdoQdroCalculationHeader.qdro_commencement_date != DateTime.MinValue))
            {
                DateTime ldtMindistributionDate = new DateTime();
                //if (!string.IsNullOrEmpty(busGlobalFunctions.CalculateMinDistributionDate(ibusParticipant.icdoPerson.idtDateofBirth)))
                //    ldtMindistributionDate = Convert.ToDateTime(busGlobalFunctions.CalculateMinDistributionDate(ibusParticipant.icdoPerson.idtDateofBirth));
                //RMD72Project
                int lintPersonId = ibusParticipant.icdoPerson.person_id;
                DateTime ldtVestedDate = busGlobalFunctions.GetVestedDate(lintPersonId, icdoQdroCalculationHeader.iintPlanId);
                ldtMindistributionDate = busGlobalFunctions.GetMinDistributionDate(lintPersonId, ldtVestedDate);  //calculate MD Date based on participant MD age option

                if (ldtMindistributionDate != DateTime.MinValue && (icdoQdroCalculationHeader.benefit_comencement_date > ldtMindistributionDate ||
                    icdoQdroCalculationHeader.qdro_commencement_date > ldtMindistributionDate))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Date of marriage is required only if Qdro percent is given
        /// </summary>
        /// <returns></returns>
        public bool CheckIfDateOfMarriageIsRequired()
        {
            if (icdoQdroCalculationHeader.qdro_calculation_header_id != 0)
            {
                foreach (busQdroCalculationDetail lbusQdroCalculationDetail in iclbQdroCalculationDetail)
                {
                    if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_percent != 0 && icdoQdroCalculationHeader.date_of_marriage == DateTime.MinValue)
                    {
                        return true;
                    }
                }
            }
            else if (icdoQdroCalculationHeader.qdro_calculation_header_id == 0 && icdoQdroCalculationHeader.qdro_application_id != 0)
            {
                if (icdoQdroCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    if (ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().IsNotNull() &&
                    ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().icdoDroBenefitDetails.benefit_perc != 0 &&
                            icdoQdroCalculationHeader.date_of_marriage == DateTime.MinValue)
                    {
                        return true;
                    }

                    if (icdoQdroCalculationHeader.is_alt_payee_eligible_for_iap == busConstant.FLAG_YES &&
                        ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().IsNotNull() &&
                    ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().icdoDroBenefitDetails.benefit_perc != 0 &&
                        icdoQdroCalculationHeader.date_of_marriage == DateTime.MinValue)
                    {
                        return true;
                    }
                }
                else
                {
                    if (ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == icdoQdroCalculationHeader.iintPlanId &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().IsNotNull() &&
                    ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == icdoQdroCalculationHeader.iintPlanId &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().icdoDroBenefitDetails.benefit_perc != 0 &&
                        icdoQdroCalculationHeader.date_of_marriage == DateTime.MinValue)
                    {
                        return true;
                    }
                }
            }
            else if (icdoQdroCalculationHeader.qdro_calculation_header_id == 0 && icdoQdroCalculationHeader.qdro_application_id == 0 && icdoQdroCalculationHeader.date_of_marriage == DateTime.MinValue)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Date of separation is required only if Qdro percent is given
        /// </summary>
        /// <returns></returns>
        public bool CheckIfDateOfSeperationIsRequired()
        {
            if (icdoQdroCalculationHeader.qdro_calculation_header_id != 0)
            {
                foreach (busQdroCalculationDetail lbusQdroCalculationDetail in iclbQdroCalculationDetail)
                {
                    if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_percent != 0 && icdoQdroCalculationHeader.date_of_seperation == DateTime.MinValue &&
                            lbusQdroCalculationDetail.icdoQdroCalculationDetail.community_property_end_date == DateTime.MinValue)
                    {
                        return true;
                    }
                }
            }
            else if (icdoQdroCalculationHeader.qdro_calculation_header_id == 0 && icdoQdroCalculationHeader.qdro_application_id != 0)
            {
                if (icdoQdroCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    if (ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().IsNotNull() &&
                    ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().icdoDroBenefitDetails.benefit_perc != 0 &&
                        icdoQdroCalculationHeader.date_of_seperation == DateTime.MinValue)
                    {
                        return true;
                    }

                    if (icdoQdroCalculationHeader.is_alt_payee_eligible_for_iap == busConstant.FLAG_YES &&
                        ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().IsNotNull() &&
                    ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().icdoDroBenefitDetails.benefit_perc != 0 &&
                        icdoQdroCalculationHeader.date_of_seperation == DateTime.MinValue)
                    {
                        return true;
                    }
                }
                else
                {
                    if (ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == icdoQdroCalculationHeader.iintPlanId &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().IsNotNull() &&
                    ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == icdoQdroCalculationHeader.iintPlanId &&
                           item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().icdoDroBenefitDetails.benefit_perc != 0 &&
                        icdoQdroCalculationHeader.date_of_seperation == DateTime.MinValue)
                    {
                        return true;
                    }
                }
            }
            else if (icdoQdroCalculationHeader.qdro_calculation_header_id == 0 && icdoQdroCalculationHeader.qdro_application_id == 0 && icdoQdroCalculationHeader.date_of_seperation == DateTime.MinValue)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Community Property end date cannot be less than date of marriage
        /// </summary>
        /// <returns></returns>
        public bool CheckValidCommunityPropertyEndDate()
        {
            if (icdoQdroCalculationHeader.qdro_calculation_header_id != 0)
            {
                foreach (busQdroCalculationDetail lbusQdroCalculationDetail in iclbQdroCalculationDetail)
                {
                    if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.community_property_end_date != DateTime.MinValue && icdoQdroCalculationHeader.date_of_marriage != DateTime.MinValue &&
                        lbusQdroCalculationDetail.icdoQdroCalculationDetail.community_property_end_date < icdoQdroCalculationHeader.date_of_marriage)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CheckValidDateOfSeperation()
        {
            if (icdoQdroCalculationHeader.qdro_calculation_header_id == 0 && icdoQdroCalculationHeader.date_of_marriage > icdoQdroCalculationHeader.date_of_seperation)
            {
                return false;
            }
            else if (icdoQdroCalculationHeader.qdro_calculation_header_id != 0)
            {
                foreach (busQdroCalculationDetail lbusQdroCalculationDetail in iclbQdroCalculationDetail)
                {
                    if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.community_property_end_date != DateTime.MinValue &&
                            icdoQdroCalculationHeader.date_of_marriage > lbusQdroCalculationDetail.icdoQdroCalculationDetail.community_property_end_date)
                    {
                        return false;
                    }
                    else if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.community_property_end_date == DateTime.MinValue &&
                        icdoQdroCalculationHeader.date_of_marriage > icdoQdroCalculationHeader.date_of_seperation)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// check if final estimate exists for the participant, alternate payee and plan
        /// </summary>
        /// <returns></returns>
        public bool CheckIfFinalEstimateExists()
        {

            DataTable ldtbFinalApprovedEstimates = busBase.Select("cdoQdroCalculationHeader.CheckIfApprovedEstimateExists",
                new object[] { ibusParticipant.icdoPerson.person_id, ibusAlternatePayee.icdoPerson.person_id, icdoQdroCalculationHeader.iintPlanId,
                this.iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == icdoQdroCalculationHeader.iintPlanId).First().
                        icdoQdroCalculationDetail.qdro_model_value});

            if (ldtbFinalApprovedEstimates.Rows.Count > 0)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region SharedInterest
        public void FillHeaderObjectForSharedInterest(busBenefitCalculationDetail abusBenefitCalculationDetail, busBenefitCalculationOptions abusBenefitCalculationOptions, busDroBenefitDetails abusDroBenefitDetails, string astrParticipantDisabled)
        {
            decimal ldecAltPayeeBenefitAmount = decimal.Zero;
            decimal ldecAltPayeeMea = decimal.Zero;
            decimal ldecAltMinimumGuarnatee = decimal.Zero;

            decimal ldecPartBenAmount = decimal.Zero;
            decimal ldecPartMea = abusBenefitCalculationDetail.icdoBenefitCalculationDetail.monthly_exclusion_amount;
            decimal ldecPartMG = abusBenefitCalculationDetail.icdoBenefitCalculationDetail.minimum_guarantee_amount;
            if (astrParticipantDisabled == busConstant.FLAG_YES)
            {
                ldecPartBenAmount = abusBenefitCalculationOptions.icdoBenefitCalculationOptions.participant_amount;
            }
            else
            {
                ldecPartBenAmount = abusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
            }


            this.GetAlternatePayeeObjectsForSharedInterest(abusDroBenefitDetails.icdoDroBenefitDetails.benefit_flat_perc, abusDroBenefitDetails.icdoDroBenefitDetails.benefit_amt, ldecPartBenAmount, ldecPartMea, ldecPartMG, out ldecAltPayeeBenefitAmount, out ldecAltPayeeMea, out ldecAltMinimumGuarnatee);

            this.FillDroClaculationDetailObject(ldecAltPayeeMea, ldecAltMinimumGuarnatee, abusDroBenefitDetails);

            this.FillDroCalculationOptionsObject(ldecAltPayeeBenefitAmount, abusDroBenefitDetails.icdoDroBenefitDetails.plan_benefit_id);
        }


        public void GetAlternatePayeeObjectsForSharedInterest(decimal adecFlatPercentage, decimal adecFlatAmount, decimal adecParticipantAmount, decimal adecMea, decimal adecMinimumGuarantee,
           out decimal adecAltPayeeAmount, out decimal adecAltPayeeMea, out decimal adecAltPayeeMinimumGuarantee)
        {
            adecAltPayeeAmount = ibusCalculation.CalculateBenefitAmtBeforeConversion(adecParticipantAmount, decimal.Zero, adecFlatAmount, adecFlatPercentage);

            adecAltPayeeMea = ibusCalculation.CalculateBenefitAmtBeforeConversion(adecMea, decimal.Zero, adecFlatAmount, adecFlatPercentage);

            adecAltPayeeMinimumGuarantee = ibusCalculation.CalculateBenefitAmtBeforeConversion(adecMinimumGuarantee, decimal.Zero, adecFlatAmount, adecFlatPercentage);

        }

        public busQdroCalculationDetail FillDroClaculationDetailObject(decimal adecAltPayeeMea, decimal adecAltPayeeMinimumGuarantee, busDroBenefitDetails abusDroBenefitDetails)
        {
            busQdroCalculationDetail lbusQdroCalculationDetail = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };
            lbusQdroCalculationDetail.icdoQdroCalculationDetail.member_exclusion_amount = adecAltPayeeMea;
            lbusQdroCalculationDetail.icdoQdroCalculationDetail.minimum_guarantee_amount = adecAltPayeeMinimumGuarantee;
            lbusQdroCalculationDetail.icdoQdroCalculationDetail.flat_amount = abusDroBenefitDetails.icdoDroBenefitDetails.benefit_amt;
            lbusQdroCalculationDetail.icdoQdroCalculationDetail.flat_percent = abusDroBenefitDetails.icdoDroBenefitDetails.benefit_flat_perc;
            lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_model_value = abusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value;
            this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetail);
            return lbusQdroCalculationDetail;
        }

        public busQdroCalculationOptions FillDroCalculationOptionsObject(decimal adecNetBenefitAmount, int ainPlanBenefitId)
        {
            busQdroCalculationOptions lbusQdroCalculationOptions = new busQdroCalculationOptions { icdoQdroCalculationOptions = new cdoQdroCalculationOptions() };
            lbusQdroCalculationOptions.icdoQdroCalculationOptions.alt_payee_benefit_amount = adecNetBenefitAmount;
            lbusQdroCalculationOptions.icdoQdroCalculationOptions.plan_benefit_id = ainPlanBenefitId;
            if (this.iclbQdroCalculationDetail.Count > 0)
            {
                this.iclbQdroCalculationDetail.FirstOrDefault().iclbQdroCalculationOptions = new Collection<busQdroCalculationOptions>();
                this.iclbQdroCalculationDetail.FirstOrDefault().iclbQdroCalculationOptions.Add(lbusQdroCalculationOptions);
            }
            return lbusQdroCalculationOptions;
        }
        #endregion

        public void SetWFVariables4PayeeAccount(int aintPayeeAccountId, int aintPlanId, bool ablnEEFlag = false, bool ablnUVHPFlag = false, bool ablnL52SplAccFlag = false, bool ablnL161SplAccFlag = false)
        {
            switch (aintPlanId)
            {
                case busConstant.MPIPP_PLAN_ID:
                    if (ablnEEFlag && ablnUVHPFlag)
                    {
                        iintEEUVHPPayeeAccountID = aintPayeeAccountId;
                        astrFundName = "EEUVHP";
                        //iblnEEUVHPPayeeAccountCreated = true;
                    }
                    else
                    {
                        iintMPIPayeeAccountID = aintPayeeAccountId;
                        //iblnMPIPayeeAccountCreated = true;
                    }
                    break;

                case busConstant.IAP_PLAN_ID:
                    if (ablnL52SplAccFlag)
                    {
                        iintL52SplAccPayeeAccountID = aintPayeeAccountId;
                        astrFundName = "L52SPLACC";
                        //iblnL52SpecialAccPayeeAccountCreated = true;
                    }
                    else if (ablnL161SplAccFlag)
                    {
                        iintL161SplAccPayeeAccountID = aintPayeeAccountId;
                        astrFundName = "L161SPLACC";
                        //iblnL161SpecialAccPayeeAccountCreated = true;
                    }
                    else
                    {
                        iintIAPPayeeAccountID = aintPayeeAccountId;
                        //iblnIAPPayeeAccntCreated = true;
                    }
                    break;

                case busConstant.LOCAL_52_PLAN_ID:
                    iintL52PayeeAccountID = aintPayeeAccountId;
                    break;

                case busConstant.LOCAL_161_PLAN_ID:
                    iintL161PayeeAccountID = aintPayeeAccountId;
                    break;

                case busConstant.LOCAL_600_PLAN_ID:
                    iintL600PayeeAccountID = aintPayeeAccountId;
                    break;

                case busConstant.LOCAL_666_PLAN_ID:
                    iintL666PayeeAccountID = aintPayeeAccountId;
                    break;

                case busConstant.LOCAL_700_PLAN_ID:
                    iintL700PayeeAccountID = aintPayeeAccountId;
                    break;
            }

        }
        public override void AddToResponse(utlResponseData aobjResponseData)
        {
            base.AddToResponse(aobjResponseData);
            aobjResponseData.ConcurrentKeysData[utlConstants.istrPrimaryKey] = Convert.ToString(icdoQdroCalculationHeader.iintAPrimaryKey > 0 ? icdoQdroCalculationHeader.iintAPrimaryKey : iintPrimaryKey);
           
        }

    }
}


