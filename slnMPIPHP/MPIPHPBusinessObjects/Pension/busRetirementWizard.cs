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
using Sagitec.DataObjects;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busRetirementWizard:
    /// </summary>
    [Serializable]
    public class busRetirementWizard : busBenefitApplication
    {


        #region Properties
        bool iblnFlag { get; set; }
        public string strPlanDescription { get; set; }
        public string strIAPPlanDescription { get; set; }
      
        public string strBenefitOptionValue { get; set; }
        public string strIAPBenefitOptionValue { get; set; }
       

        public int intPlan_Id { get; set; }
        public int intIAPPlan_Id { get; set; }

        public int intEligiblePlanId { get; set; }
        public int intIAPEligiblePlanId { get; set; }
       

        public string strSpouseConsent { get; set; }
        public string strIAPSpouseConsent { get; set; }
       

        public int intJointAnnuiant { get; set; }
        public int intIAPJointAnnuiant { get; set; }

        public int PayeeAccountId { get; set; }
       

        //  public Collection<busRetirementWizard> iclbBenefitElectionDetails { get; set; }

        /// <summary>
        /// Gets or sets the withdrawal application object.
        /// </summary>
        public cdoBenefitApplication icdoBenefitApplicationWithdrawal { get; set; }
        public cdoBenefitApplicationDetail icdoBenefitApplicationWithdrawalDetail { get; set; }

        #endregion

        #region public Methods


        public Collection<busBenefitApplicationDetail> LoadBenefitElectionDetails(int personId,int PlanId)
        {
            busBase lobjBase = new busBase();
            DataTable ldtbBenefitPlan = busBase.Select("cdoPerson.LoadBenefitElectionPlanDetails", new object[2] {personId,PlanId});

            iclbBenefitApplicationDetail = lobjBase.GetCollection<busBenefitApplicationDetail>(ldtbBenefitPlan, "icdoBenefitApplicationDetail");

            return iclbBenefitApplicationDetail;
        }

        public Collection<cdoPlan> GetPlanValues()
        {
            Collection<cdoPlan> lColPlans = new Collection<cdoPlan>();
            if (!Eligible_Plans.IsNullOrEmpty())
            {
                StringBuilder xyz = new StringBuilder();

                foreach (string plan_code in Eligible_Plans)
                {
                    if(plan_code != "IAP")
                    {
                        if (!string.IsNullOrEmpty(xyz.ToString()))
                            xyz.Append(",");

                        xyz.Append("'" + plan_code + "'");

                    }
                   
                }

                string lstrFinalQuery = "select * from sgt_plan where plan_code in (" + xyz + ")";
                DataTable ldtbListofPLans = DBFunction.DBSelect(lstrFinalQuery,
                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldtbListofPLans.Rows.Count > 0)
                {
                    lColPlans = Sagitec.DataObjects.doBase.GetCollection<cdoPlan>(ldtbListofPLans);
                }
            }

            return lColPlans;
        }

        public void GetPriorDates()
        {
            if (this.icdoBenefitApplication.withdrawal_date != DateTime.MinValue)
            {
                DateTime ldtWithdraw = this.icdoBenefitApplication.withdrawal_date;
                dtThirtyDaysPriorDate = ldtWithdraw.AddDays(-30);
            }
        }

        public bool IsDead()
        {
            if (this.ibusPerson.icdoPerson.date_of_death == DateTime.MinValue)
            {
                return false;
            }
            return true;
        }

        //UAT PIR 160 : Withdrawal date validation
        public bool IsWithdrawalDateNextMonthAfterReceivedDate()
        {
            bool lblnResult = false;

            if (icdoBenefitApplication.withdrawal_date == icdoBenefitApplication.application_received_date.GetLastDayofMonth().AddDays(1))
                lblnResult = true;

            return lblnResult;
        }

        public ArrayList btn_CalculateBenefitClick()
        {


            //CreateNormalRetirementApplication();

            //btn_Approved();

            ArrayList iarrErrors = new ArrayList();

            //Call Eligibility Yet Again the Final Time Just Before doing Final Calculation
            this.LoadWorkHistoryandSetupPrerequisites_Retirement();
            //PIR 1053
            if (this.iclbBenefitApplicationDetail.Where(item => item.iintPlan_ID == busConstant.MPIPP_PLAN_ID && item.icdoBenefitApplicationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION).Count() > 0 &&
                this.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES
                && ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).Count() > 0)
            {
                ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION;
            }

            //Flags to be used for making sure we donot calculate again if another IAP entry comes along
            bool lblnIAPCalculated = false;
            bool lblnMPIPPCalculated = false;

            int lintMPIPPHeaderId = 0;
            int lintIAPHeaderId = 0;



            busBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail.OrderBy(i => i.icdoBenefitApplicationDetail.iintPlan_id))
            {
                if (this.icdoBenefitApplication.converted_min_distribution_flag != busConstant.FLAG_YES || lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                {
                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.application_detail_status_value != busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED)
                    {
                        #region Initialize Calculation Needed Objects from Application
                     //   busBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                        if (lbusBenefitCalculationRetirement.ibusCalculation.IsNull())
                        {
                            lbusBenefitCalculationRetirement.ibusCalculation = new busCalculation();
                        }
                        lbusBenefitCalculationRetirement.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lbusBenefitCalculationRetirement.ibusBenefitApplication = this;
                        lbusBenefitCalculationRetirement.ibusPerson = this.ibusPerson;
                        lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount = this.ibusPerson.iclbPersonAccount;
                        lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.age = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.retirement_date);
                        lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
                        lbusBenefitCalculationRetirement.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                        //lbusBenefitCalculationRetirement.LoadAllRetirementContributions();
                        lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrRetirementType = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value;

                        if (!ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                        {
                            lbusBenefitCalculationRetirement.LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                        }
                        else
                        {
                            lbusBenefitCalculationRetirement.LoadAllRetirementContributions(null);
                        }

                        #endregion


                        if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                        {
                            lbusBenefitCalculationRetirement.iblnCalculateIAPBenefit = true;

                            #region Setting Up Header for IAP
                            if (!lblnIAPCalculated)
                            {
                                lbusBenefitCalculationRetirement.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id,
                                                                                             busConstant.BENEFIT_TYPE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.retirement_date,
                                                                                             this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID);
                            }
                            else
                            {
                                if (lbusBenefitCalculationRetirement.FindBenefitCalculationHeader(lintIAPHeaderId))
                                {
                                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitApplicationDetail.iintPlan_ID;
                                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitApplication.retirement_date);
                                }
                            }
                            #endregion
                        }

                        else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                        {

                            lbusBenefitCalculationRetirement.iblnCalculateMPIPPBenefit = true;

                            #region Setting Up Header for MPIPP
                            if (!lblnMPIPPCalculated)
                            {
                                lbusBenefitCalculationRetirement.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id,
                                                                                             busConstant.BENEFIT_TYPE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.retirement_date,
                                                                                             this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID);
                            }
                            else
                            {
                                if (lbusBenefitCalculationRetirement.FindBenefitCalculationHeader(lintMPIPPHeaderId))
                                {
                                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitApplicationDetail.iintPlan_ID;
                                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitApplication.retirement_date);
                                }
                            }
                            #endregion

                        }

                        else
                        {
                            #region Local Plans Found
                            if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                            {
                                lbusBenefitCalculationRetirement.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id,
                                                   busConstant.BENEFIT_TYPE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.retirement_date,
                                                   this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID);

                            }

                            #endregion
                        }

                        if (this.CheckAlreadyVested(lbusBenefitApplicationDetail.istrPlanCode))
                        {
                            if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                            {
                                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                                {
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                                }

                                lbusBenefitCalculationRetirement.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                                 this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                                 lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                                 lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);

                            }
                            else
                            {
                                if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                                {
                                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                                    {
                                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                                    }

                                    lbusBenefitCalculationRetirement.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                                     this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                                     lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                                     lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);

                                }


                            }


                        }

                        try
                        {
                            if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID && !lblnIAPCalculated)
                            {
                                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                                lbusBenefitCalculationRetirement.PersistChanges();
                                lintIAPHeaderId = lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                                lblnIAPCalculated = true;

                            }

                            else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && !lblnMPIPPCalculated && (lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                            {
                                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                                lbusBenefitCalculationRetirement.PersistChanges();
                                lintMPIPPHeaderId = lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                                lblnMPIPPCalculated = true;
                            }

                            else if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.IAP_PLAN_ID && lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                            {
                                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                                lbusBenefitCalculationRetirement.PersistChanges();

                            }
                            if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                            {
                                lbusBenefitCalculationRetirement.AfterPersistChanges();
                                SetWorkflowRelatedVariablesforFinalCalculation(lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.benefit_calculation_header_id);

                            }
                            else
                            {
                                lbusBenefitCalculationRetirement.AfterPersistChanges();

                                SetWorkflowRelatedVariablesforFinalCalculation(lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                            }


                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                   // CreateLateRetirementCalculationFromMinimumDistribution(lbusBenefitApplicationDetail);
                }
            }

            if ((this.ibusBaseActivityInstance.IsNotNull() && ((Sagitec.Bpm.busBpmActivityInstance)this.ibusBaseActivityInstance).ibusBpmActivity.icdoBpmActivity.name != busConstant.PersonAccountMaintenance.ENTER_RETIREMENT_APPLICATION))
            {
                this.SetProcessInstanceParameters();
            }
            this.icdoBenefitApplication.final_calc_flag = busConstant.FLAG_YES;
            this.icdoBenefitApplication.change_benefit_option_flag = busConstant.FLAG_NO;
            this.icdoBenefitApplication.Update();
            
            lbusBenefitCalculationRetirement.btn_ApproveCalculation();


            this.PayeeAccountId = Convert.ToInt32(this.iobjPassInfo.idictParams["aintPayeeAccountId"]);


            //utlPassInfo lobMainPassinfo =lbusBenefitCalculationRetirement.iobjPassInfo;
            //// this. = lbusBenefitCalculationRetirement.iobjPassInfo;
            //this.iobjPassInfo = lobMainPassinfo;
            iarrErrors.Add(this);
          
            return iarrErrors;
        }

        public bool IsWithdrawalDateLessThanTodaysDate()
        {
            bool lblnResult = false;
            if (icdoBenefitApplication.withdrawal_date != DateTime.MinValue && icdoBenefitApplication.withdrawal_date < DateTime.Now)
            {
                lblnResult = true;
            }
            return lblnResult;

        }

        public void SetSubPlanDescription(string astrIsIAPSpecial, string astrSubPlan, string astrIsSpecialAcnt)
        {
            istrIsIAPSpecial = astrIsIAPSpecial;
            istrIsSpecialAcnt = astrIsSpecialAcnt;
            istrSubPlanDesc = astrSubPlan;
        }

        #endregion

        # region overriden Methods

        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(System.DateTime.Now);
            GetPriorDates();
            if (icdoBenefitApplication.withdrawal_date != DateTime.MinValue)
            {
                idtWithdrawalDate = icdoBenefitApplication.withdrawal_date;
            }
            if (idtWithdrawalDate != DateTime.MinValue)
            {
                idtDayBeforeWidrwlDate = idtWithdrawalDate.AddDays(-1);
            }
            if (astrTemplateName == busConstant.EE_CONTRIBUTIONS_AND_UVHP_REFUND_COVER_LETTER
                || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Packet || astrTemplateName == busConstant.Withdrawal_EE_UVHP_Retirement_Disablity_Packet)
            {
                LoadAllRetirementContributions();
            }

            if(astrTemplateName == busConstant.Withdrawal_COVID_Application)
            {
                
                    var a = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                    DataTable ldtbEmergencyPaymentSetupValue = busBase.Select("cdoEmergencyPaymentSetupValue.GetEmergencyPaymentSetupValue", new object[1] { DateTime.Now });
                    DataTable ldtbIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                                   new object[2] { a, Convert.ToInt32(ldtbEmergencyPaymentSetupValue.Rows[0]["IAP_BALANCE_AS_OF_YEAR"]) });

                idecCovidIAP2018BalanceAmt = Math.Round(Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]), 2);
                idecCovidIAPMaxAllowedWithdrawalAmt = Math.Round(Math.Round(Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]), 2) * Convert.ToDecimal(ldtbEmergencyPaymentSetupValue.Rows[0]["PERCENTAGE"]) / 100, 2);
                if(idecCovidIAPMaxAllowedWithdrawalAmt > Convert.ToDecimal(ldtbEmergencyPaymentSetupValue.Rows[0]["MAXLIMIT"]))
                {
                    idecCovidIAPMaxAllowedWithdrawalAmt = Convert.ToDecimal(ldtbEmergencyPaymentSetupValue.Rows[0]["MAXLIMIT"]);
                }
                istrPersonEmailID = this.ibusPerson.icdoPerson.email_address_1;
                string Phonenumber = (this.ibusPerson.icdoPerson.home_phone_no.IsNotNullOrEmpty())? Regex.Replace(this.ibusPerson.icdoPerson.home_phone_no, "[^.0-9]", "") : "";
                if (Phonenumber.Length == 10)
                {
                    istrPhoneAreaCode = Phonenumber.Substring(0, 3);
                    istrPhoneNumber = Phonenumber.Substring(3, 3) + "-" + Phonenumber.Substring(6, 4);
                }
                else
                {
                    istrPhoneNumber = string.Empty;
                    istrPhoneNumber = string.Empty;
                }

                if(this.ibusPerson.icdoPerson.gender_value == "M" || this.ibusPerson.icdoPerson.gender_value == "F")
                {
                    istrGender = this.ibusPerson.icdoPerson.gender_description;
                }
                else
                {
                    istrGender = string.Empty;
                }

                idtWithdrawalDate = DateTime.Now.GetFirstDayofMonth();

                
                    
                
            }
        }

        public void LoadAllRetirementContributions()
        {
            DataTable ldtbList = busBase.Select("cdoPersonAccountRetirementContribution.GetRetirementContributionsofPerson", new object[1] { this.icdoBenefitApplication.person_id });
            Collection<busPersonAccountRetirementContribution> lclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");

            int lintPersonAccountID = lclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.plan_id == busConstant.MPIPP_PLAN_ID &&
                                                            item.icdoPersonAccountRetirementContribution.person_id == this.icdoBenefitApplication.person_id).FirstOrDefault().icdoPersonAccountRetirementContribution.person_account_id;
            if (lintPersonAccountID > 0)
            {
                decimal ldecBenefitInterestRate = decimal.One;

                decimal ldecPartialEE = decimal.Zero;
                decimal ldecPartialUVHP = decimal.Zero;

                lclbPersonAccountRetirementContribution = lclbPersonAccountRetirementContribution.OrderBy(item => item.icdoPersonAccountRetirementContribution.effective_date).ToList().ToCollection();

                object lobjBenefitInterestRate = DBFunction.DBExecuteScalar("cdoBenefitInterestRate.GetBenefitInterestRate", new object[1] { this.icdoBenefitApplication.withdrawal_date.Year },
                                              iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (lobjBenefitInterestRate.IsNotNull())
                {
                    ldecBenefitInterestRate = (Decimal)lobjBenefitInterestRate;
                }

                decimal ldecNonVestedEE = lclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID &&
                                    item.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).
                                    Sum(item => item.icdoPersonAccountRetirementContribution.ee_contribution_amount);

                decimal ldecNonVestedEEInterest = lclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID &&
                                    item.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).
                                                                    Sum(item => item.icdoPersonAccountRetirementContribution.ee_int_amount);

                ldecPartialEE = Math.Round(((ldecNonVestedEE + ldecNonVestedEEInterest) * ldecBenefitInterestRate) / 12 * (this.icdoBenefitApplication.withdrawal_date.Month - 1), 2);

                decimal ldecUVHPAmount = (from contribution in lclbPersonAccountRetirementContribution
                                          where contribution.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID &&
                                              contribution.icdoPersonAccountRetirementContribution.computational_year <= this.icdoBenefitApplication.withdrawal_date.Year

                                          select contribution.icdoPersonAccountRetirementContribution.uvhp_amount).Sum();

                decimal ldecUVHPinterest =
                    (from contribution in lclbPersonAccountRetirementContribution
                     where contribution.icdoPersonAccountRetirementContribution.person_account_id == lintPersonAccountID &&
                         contribution.icdoPersonAccountRetirementContribution.computational_year <= this.icdoBenefitApplication.withdrawal_date.Year
                     select contribution.icdoPersonAccountRetirementContribution.uvhp_int_amount).Sum();

                ldecPartialUVHP = Math.Round(((ldecUVHPAmount + ldecUVHPinterest) * ldecBenefitInterestRate) / 12 * (this.icdoBenefitApplication.withdrawal_date.Month - 1), 2);

                idecNonVestedEE = ldecNonVestedEE;
                idecNonVestedEEInterest = ldecNonVestedEEInterest + ldecPartialEE;

                idecUVHPAmount = ldecUVHPAmount;
                idecUVHPInterest = ldecUVHPinterest + ldecPartialUVHP;

                idecTOTALInterest = idecUVHPInterest + idecNonVestedEEInterest;

                idecTotalEEUVHP = idecNonVestedEE + idecNonVestedEEInterest + idecUVHPInterest + idecUVHPAmount;

                if (ldecNonVestedEE > decimal.Zero && ldecUVHPAmount > decimal.Zero)
                {
                    istrBoth = busConstant.FLAG_YES;
                }
                else if (ldecNonVestedEE > decimal.Zero)
                {
                    istrEEFlag = busConstant.FLAG_YES;
                }
                else if (ldecUVHPAmount > decimal.Zero)
                {
                    istrUVHPFlag = busConstant.FLAG_YES;
                }
            }
        }
        public void CreateNormalRetirementApplication()
        {
            if (this.icdoBenefitApplication.benefit_application_id == 0)
            {


                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();



                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id != 1)
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = this.strBenefitOptionValue.ToString();
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id = lbusPlanBenefitXr.GetPlanBenefitId(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id, this.strBenefitOptionValue.ToString());
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.spousal_consent_flag = strSpouseConsent.ToString();
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id = this.intJointAnnuiant;
                    }
                    else
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = this.strIAPBenefitOptionValue.ToString();
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id = lbusPlanBenefitXr.GetPlanBenefitId(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id, this.strIAPBenefitOptionValue.ToString());
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id = this.intIAPJointAnnuiant;



                    }
                }




                #region Create New Normal Retirement Application
                //  this.icdoBenefitApplication.final_calc_flag = busConstant.FLAG_NO;
                this.icdoBenefitApplication.min_distribution_flag = busConstant.FLAG_NO;
                this.icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                //  this.icdoBenefitApplication.converted_min_distribution_flag = busConstant.FLAG_YES;
                this.icdoBenefitApplication.benefit_application_id = 0;
                this.icdoBenefitApplication.created_by = iobjPassInfo.istrUserID;
                this.icdoBenefitApplication.created_date = DateTime.Now;
                this.icdoBenefitApplication.modified_by = iobjPassInfo.istrUserID;
                this.icdoBenefitApplication.modified_date = DateTime.Now;
                this.icdoBenefitApplication.update_seq = 0;

                this.icdoBenefitApplication.ienuObjectState = ObjectState.Insert;
                this.icdoBenefitApplication.Insert();

                if (icdoBenefitApplication.benefit_application_id > 0)
                {

                    foreach (busBenefitApplicationDetail lbusBenefitAppDetail in this.iclbBenefitApplicationDetail)
                    {
                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.benefit_application_detail_id = 0;
                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.ienuObjectState = ObjectState.Insert;
                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.application_detail_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.benefit_subtype_value = busConstant.RETIREMENT_TYPE_NORMAL;
                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.benefit_application_id = icdoBenefitApplication.benefit_application_id;
                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.created_by = iobjPassInfo.istrUserID;
                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.created_date = DateTime.Now;
                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.modified_by = iobjPassInfo.istrUserID;
                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.modified_date = DateTime.Now;
                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.update_seq = 0;

                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.Insert();
                    }
                    #endregion
                }
            }

            if (this.icdoBenefitApplication.retirement_date != DateTime.MinValue && this.icdoBenefitApplication.converted_min_distribution_flag != busConstant.FLAG_YES)
            {
                this.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.retirement_date);
                if (this.iobjPassInfo.ienmPageMode == utlPageMode.New)
                {
                    this.LoadandProcessWorkHistory_ForAllPlans();
                }
                SetupPrerequisites();
            }


        }
        //Code-Abhishek
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {

            //DataTable ldtbPlanBenefitRate = busBase.Select("cdoPlanBenefitRate.Lookup", new object[] { });
            //// ChangeID: 57284
            //Collection<cdoPlanBenefitRate> lclbcdoPlanBenefitRate = cdoDummyWorkData.GetCollection<cdoPlanBenefitRate>(ldtbPlanBenefitRate);


            this.LoadBenefitElectionDetails(this.ibusPerson.icdoPerson.person_id,this.intEligiblePlanId);

          
            if (this.icdoBenefitApplication.retirement_date != DateTime.MinValue && this.icdoBenefitApplication.converted_min_distribution_flag != busConstant.FLAG_YES)
            {
                this.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.retirement_date);
                if (this.iobjPassInfo.ienmPageMode == utlPageMode.New)
                {
                    this.LoadandProcessWorkHistory_ForAllPlans();
                }
                SetupPrerequisites();
            }
            base.BeforeValidate(aenmPageMode);
        }
        //Code-Abhishek


        private void SetupPrerequisites()
        {
            if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.DetermineVesting();
                this.DetermineBenefitSubTypeandEligibility_Retirement();
            }
        }


        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            this.icdoBenefitApplication.person_id = this.ibusPerson.icdoPerson.person_id;

            if (this.icdoBenefitApplication.benefit_application_id > 0 && this.iblnWithdrawalForAlternatePayee)
            {
                int i = DBFunction.DBNonQuery("cdoBenefitApplicationDetail.DeleteBenefitDetails", new object[1] { this.icdoBenefitApplication.benefit_application_id },
                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }



            //PIR 999
            if (iclbBenefitApplicationDetail != null && iclbBenefitApplicationDetail.Count > 0
                && iclbBenefitApplicationDetail.Where(t => t.istrPlanCode == busConstant.MPIPP && t.icdoBenefitApplicationDetail.ee_flag == busConstant.FLAG_YES).Count() > 0)
            {
                //PIR 999
                if (ibusPerson.iclbPersonAccountRetirementContribution == null)
                {
                    DataTable ldtbList = busBase.Select("cdoPersonAccountRetirementContribution.GetRetirementContributionsofPerson", new object[1] { this.ibusPerson.icdoPerson.person_id });
                    ibusPerson.iclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");
                    busBenefitCalculationWithdrawal ibusBenefitCalculationWithdrawal = new busBenefitCalculationWithdrawal();
                    ibusBenefitCalculationWithdrawal.LoadVestedNonVestedEE(ibusPerson.iclbPersonAccountRetirementContribution, this.ibusPerson.icdoPerson.person_id,
                        ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }

                if (iobjPassInfo.ienmPageMode == utlPageMode.New || (iobjPassInfo.ienmPageMode == utlPageMode.Update && icdoBenefitApplication.effective_date == DateTime.MinValue))
                {
                    icdoBenefitApplication.effective_date = ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                }
                //PIR 999 New Changes
                if (((ibusPerson.iclbPersonAccount != null && ibusPerson.iclbPersonAccount.Count() > 0 &&
                    (ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.idecNonVestedEE +
                    ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.idecNonVestedEE) <= 0)
                     || icdoBenefitApplication.dro_application_id > 0) &&
                    icdoBenefitApplication.effective_date == DateTime.MinValue)
                {
                    icdoBenefitApplication.effective_date = icdoBenefitApplication.withdrawal_date;
                }
            }
                       
        }


        public override void ValidateHardErrors(utlPageMode aenmPageMode) //Code-Abhishek // We might want to put CHECK IF ELIGIBLE here since if he not vested or Eligible nothing else matters 
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();
            if (iobjPassInfo.istrFormName != busConstant.Retirement_Application_Maintenance_Form_2)
            {
                this.EvaluateInitialLoadRules();
                if (icdoBenefitApplication.retirement_date == DateTime.MinValue)
                {
                    lobjError = AddError(5027, " ");
                    this.iarrErrors.Add(lobjError);
                }
                if (icdoBenefitApplication.application_received_date == DateTime.MinValue)
                {
                    lobjError = AddError(5026, " ");
                    this.iarrErrors.Add(lobjError);
                }

                if(this.intEligiblePlanId == 0)
                {
                    lobjError = AddError(0, "Select Elegible Plan to create Benefit Election.");
                    this.iarrErrors.Add(lobjError);

                }
                if (icdoBenefitApplication.retirement_date != DateTime.MinValue && icdoBenefitApplication.retirement_date < DateTime.Today)
                {
                    lobjError = AddError(5028, " ");
                    this.iarrErrors.Add(lobjError);
                }

                //if (this.icdoBenefitApplication != null && this.icdoBenefitApplication.retirement_date != DateTime.MinValue)
                //{
                //    DateTime ldtReceivedDate1 = DateTime.MinValue, ldtReceivedDate2 = DateTime.MinValue;
                //    DateTime ldtCurrentDate = DateTime.MinValue;
                //    ldtReceivedDate1 = this.icdoBenefitApplication.retirement_date.AddMonths(-2).AddDays(-1);
                //    ldtReceivedDate2 = this.icdoBenefitApplication.retirement_date.AddMonths(-6).AddDays(-1);
                //    ldtCurrentDate = DateTime.Today;
                //    //if (ldtCurrentDate > ldtReceivedDate1 || ldtCurrentDate < ldtReceivedDate2)
                //    if (!(ldtCurrentDate <= ldtReceivedDate1 && ldtCurrentDate >= ldtReceivedDate2))
                //    {
                //        lobjError = AddError(6232, " ");
                //        this.iarrErrors.Add(lobjError);
                //    }
                //}


                if (this.NotEligible.IsNotNull() && this.NotEligible)
                {
                    lobjError = AddError(5103, " ");
                    this.iarrErrors.Add(lobjError);
                }
                int lintPersonId = 0;
                if (this.icdoBenefitApplication.person_id > 0)
                {
                    lintPersonId = this.icdoBenefitApplication.person_id;
                }

                if (this.GetDateDifference() < 60)
                {
                    lobjError = AddError(5089, "");
                    this.iarrErrors.Add(lobjError);
                }

                if (this.icdoBenefitApplication != null && this.icdoBenefitApplication.retirement_date != DateTime.MinValue &&
                   this.icdoBenefitApplication.retirement_date.Day != 1 && this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
                {
                    lobjError = AddError(5088, " ");
                    this.iarrErrors.Add(lobjError);
                }

                //PIR-911 Issue 3 part 2
                int lintPayeeAccountswithIAPPlan = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.GetActiveDisabilityPayeeAccountsWithIAPPlan", new object[1] { lintPersonId },
                 iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (lintPayeeAccountswithIAPPlan > 0)
                {
                    Eligible_Plans.Remove("IAP");
                }

                if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES && this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES && this.iclbBenefitApplicationDetail.Count > 0 && this.icdoBenefitApplication.min_distribution_flag != busConstant.FLAG_YES
                    && this.icdoBenefitApplication.converted_min_distribution_flag != busConstant.FLAG_YES)
                {
                    int count = 0;
                    int lintCount = 0;
                    foreach (busBenefitApplicationDetail item in this.iclbBenefitApplicationDetail)
                    {
                        if ((item.iintPlan_ID == busConstant.IAP_PLAN_ID && item.istrSubPlan.IsNullOrEmpty()) || (item.iintPlan_ID == busConstant.MPIPP_PLAN_ID && item.istrSubPlan.IsNullOrEmpty()))
                        {
                            

                            count++;
                        }
                    }
                    if (count > 0 && count < 2)// && lintCount >= 0 && lintCount <2)
                    {
                        //PIR RID 63412
                        busPersonOverview ibusPersonOverview = new busPersonOverview();
                        bool liMPIApprovedAppExists = false;
                        if (ibusPersonOverview.FindPerson(this.icdoBenefitApplication.person_id))
                        {
                            ibusPersonOverview.LoadBenefitApplication();
                        }

                        if (ibusPersonOverview.iclbBenefitApplication != null &&
                               ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT
                                && item.icdoBenefitApplication.iintPlanId == busConstant.MPIPP_PLAN_ID && item.icdoBenefitApplication.converted_min_distribution_flag == busConstant.FLAG_YES
                                && item.icdoBenefitApplication.application_status_value == busConstant.BENEFIT_APPLICATION_STATUS_APPROVED).Count() > 0)
                        {
                            liMPIApprovedAppExists = true;
                        }
                        //end of PIR RID 63412
                        if (lintPayeeAccountswithIAPPlan <= 0 && !liMPIApprovedAppExists)    //PIR RID 63412 MPI application check added to the condition. 
                        {
                            lobjError = AddError(5105, "");
                            this.iarrErrors.Add(lobjError);
                        }
                    }
                }

                if (this.iclbBenefitApplicationDetail.Count > 0 && this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP && item.istrSubPlan.IsNullOrEmpty()).Count() > 0 && this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP && item.istrSubPlan.IsNullOrEmpty()).Count() > 0)
                {
                    if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP && item.istrSubPlan.IsNullOrEmpty()).First().icdoBenefitApplicationDetail.istrBenefitOptionValue != this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP && item.istrSubPlan.IsNullOrEmpty()).First().icdoBenefitApplicationDetail.istrBenefitOptionValue)
                    {
                        if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP && item.istrSubPlan.IsNullOrEmpty()).First().icdoBenefitApplicationDetail.istrBenefitOptionValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY && this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP && item.istrSubPlan.IsNullOrEmpty()).First().icdoBenefitApplicationDetail.istrBenefitOptionValue == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                        {

                        }
                        else if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP && item.istrSubPlan.IsNullOrEmpty()).First().icdoBenefitApplicationDetail.istrBenefitOptionValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY && this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP && item.istrSubPlan.IsNullOrEmpty()).First().icdoBenefitApplicationDetail.istrBenefitOptionValue == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                        {

                        }
                        else
                        {
                            if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP && item.istrSubPlan.IsNullOrEmpty()).First().icdoBenefitApplicationDetail.istrBenefitOptionValue != busConstant.LUMP_SUM)
                            {
                                lobjError = AddError(5163, "");
                                this.iarrErrors.Add(lobjError);
                            }
                        }
                    }
                }

                base.ValidateHardErrors(aenmPageMode);

              

                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    Hashtable lhstParams = new Hashtable();
                    lhstParams["iintPlan_ID"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id;
                    lhstParams["icdoBenefitApplicationDetail.plan_benefit_id"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id;
                    lhstParams["istrSubPlan"] = lbusBenefitApplicationDetail.istrSubPlan;
                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id != 1)
                    {
                        lhstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"] = Convert.ToString(this.strBenefitOptionValue);
                        lhstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"] = this.intJointAnnuiant;
                        lhstParams["icdoBenefitApplicationDetail.spousal_consent_flag"] = Convert.ToString(this.strSpouseConsent);
                    }
                    else
                    {
                        lhstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"] = Convert.ToString(this.strIAPBenefitOptionValue);
                        lhstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"] = this.intIAPJointAnnuiant;
                        lhstParams["icdoBenefitApplicationDetail.spousal_consent_flag"] = Convert.ToString(this.strIAPSpouseConsent);

                    }
                                       
                    this.iarrErrors = CheckErrorOnAddButton(this, lhstParams, ref iarrErrors, true);
                }
            }
            else
            {
                if (this.NotEligible.IsNotNull() && this.NotEligible)
                {
                    lobjError = AddError(5103, " ");
                    this.iarrErrors.Add(lobjError);
                }
                //PROD PIR 799
                if (icdoBenefitApplication.retirement_date == DateTime.MinValue)
                {
                    lobjError = AddError(5027, " ");
                    this.iarrErrors.Add(lobjError);
                }
               

                


                // Disable Save button when app status is approved and validation for rtmt date should be 1st day of month
               
                //PIR-799
               
                //PIR-799
                if (iclbRetirementEligiblePlans != null)
                {
                    if (iclbRetirementEligiblePlans.Where(obj => obj.istrIsSelected == busConstant.FLAG_YES).Count() > 0)
                    {
                        bool iblnIsIAPSelected = false;
                        bool iblnIsMPIPPSelected = false;
                        foreach (busRetirementEligiblePlans lRetirementEligiblePlans in iclbRetirementEligiblePlans)
                        {
                            if (lRetirementEligiblePlans.istrPlanName == busConstant.IAP && lRetirementEligiblePlans.istrIsSelected == busConstant.FLAG_YES)
                            {
                                iblnIsIAPSelected = true;
                            }
                            else if (lRetirementEligiblePlans.istrPlanName == busConstant.MPIPP && lRetirementEligiblePlans.istrIsSelected == busConstant.FLAG_YES)
                            {
                                iblnIsMPIPPSelected = true;
                            }
                        }
                        // check if MPIPP selected and IAP not selected or IAP selected and MPIPP not selected
                        if ((iblnIsIAPSelected && !iblnIsMPIPPSelected) || (!iblnIsIAPSelected && iblnIsMPIPPSelected))
                        {
                            lobjError = AddError(6233, " ");
                            this.iarrErrors.Add(lobjError);
                        }
                    }
                }
                this.EvaluateInitialLoadRules();
            }
        }

        public Collection<cdoPlanBenefitXr> GetWizardBenefitOptionsforPlan(int aintPlan_Id,string istrBOptionValue)
        {
            DataTable ldtbResult = null;
            Collection<cdoPlanBenefitXr> lclcBenefitOptions = new Collection<cdoPlanBenefitXr>();


            if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
            {
                if (this.ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_SINGLE || this.ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_DIVORCED)
                {
                    ldtbResult = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForRetrifSinlgleorDivorced", new object[1] { aintPlan_Id });
                    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                }
                else //if (aintPlan_Id == 2 &&  GetSubPlan(aintPlan_Id).Count() > 0 && this.intIAPPlan_Id == busConstant.IAP_PLAN_ID)
                {
                    string astrBenefitOption = istrBOptionValue; // this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP && item.istrSubPlan.IsNullOrEmpty()).First().icdoBenefitApplicationDetail.istrBenefitOptionValue;
                    if (astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                        astrBenefitOption = busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY;
                    else if (astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                        astrBenefitOption = busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY;
                    ldtbResult = busBase.Select("cdoBenefitApplicationDetail.GetBenefitFromPlanForRetrIAP", new object[2] { aintPlan_Id, astrBenefitOption });
                    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                }
                //else
                //{
                //    ldtbResult = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForRetr", new object[1] { this.intIAPPlan_Id });
                //    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                    
                //}
            }
           return lclcBenefitOptions;
        }

        public DateTime GetRetirementDate()
        {
            DateTime ldtRetirementDate = new DateTime();

            DataTable ldtbList = Select<cdoBenefitApplication>(
               new string[3] { enmBenefitApplication.benefit_type_value.ToString(), enmBenefitApplication.person_id.ToString(),
                                enmBenefitApplication.application_status_value.ToString() },
               new object[3] { busConstant.BENEFIT_TYPE_RETIREMENT, icdoBenefitApplication.person_id,
                               busConstant.BENEFIT_APPLICATION_STATUS_APPROVED }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                ldtRetirementDate = Convert.ToDateTime(ldtbList.Rows[0][enmBenefitApplication.retirement_date.ToString()]);
            }

            return ldtRetirementDate;
        }
        public int GetDateDifference()
        {
            TimeSpan ltsDifference = icdoBenefitApplication.retirement_date.Subtract(icdoBenefitApplication.application_received_date);
            return ltsDifference.Days;
        }
        public void LoadBenefitApplicationEligiblePlans()
        {
            if (icdoBenefitApplication != null)
            {
                ibusBenefitApplicationEligiblePlans = new busBenefitApplicationEligiblePlans { icdoBenefitApplicationEligiblePlans = new cdoBenefitApplicationEligiblePlans() };
                DataTable ldtbList = Select<cdoBenefitApplicationEligiblePlans>(
                    new string[1] { enmBenefitApplicationEligiblePlans.benefit_application_id.ToString() },
                    new object[1] { icdoBenefitApplication.benefit_application_id }, null, null);
                if (ldtbList.Rows.Count > 0)
                    ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.LoadData(ldtbList.Rows[0]);

                //PIR-799
                if (icdoBenefitApplication.benefit_application_id != 0)
                    iblnIsRtmtAppFirstTimeSaved = true;
            }
            LoadRetirementEligiblePlans();
            AlreadyRetiredEligiblePlans();
        }


        public void AlreadyRetiredEligiblePlans()
        {
            DataTable ldtbGetRTMApplication = null;
            if (!string.IsNullOrEmpty(ibusPerson.icdoPerson.mpi_person_id))
            {
                ldtbGetRTMApplication = busBase.Select("cdoBenefitApplication.GetApprovedRetirementApplication", new object[1] { ibusPerson.icdoPerson.mpi_person_id });
            }
            if (ldtbGetRTMApplication != null && ldtbGetRTMApplication.Rows.Count > 0)
            {
                if (iclbRetirementEligiblePlans.Where(obj => obj.istrPlanName != string.Empty).Count() > 0)
                {
                    for (int iint = 0; iint < ldtbGetRTMApplication.Rows.Count; iint++)
                    {
                        LoadApprovedBenefitAppDetails(Convert.ToInt32(ldtbGetRTMApplication.Rows[iint]["BENEFIT_APPLICATION_ID"]));
                        foreach (busRetirementEligiblePlans lRetirementEligiblePlans in iclbRetirementEligiblePlans)
                        {
                            foreach (busBenefitApplicationDetail item in this.iclbApprovedBenefitApplicationDetail)
                            {
                                // check previously retired application eligible plans
                                if (item.istrPlanCode == lRetirementEligiblePlans.istrPlanName)
                                {
                                    lRetirementEligiblePlans.istrIsPreviouslySelected = busConstant.FLAG_YES;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void LoadRetirementEligiblePlans()
        {
            if (Eligible_Plans != null && Eligible_Plans.Count > 0)
            {
                iclbRetirementEligiblePlans = new Collection<busRetirementEligiblePlans>();
                foreach (string istrplan in Eligible_Plans)
                {
                    string lstrSelected = busConstant.FLAG_YES;
                    if (iclbRetirementEligiblePlans.Where(obj => obj.istrPlanName == istrplan).Count() == 0)
                    {
                        if (istrplan == busConstant.MPIPP && ibusBenefitApplicationEligiblePlans != null &&
                             ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.mpipp_flag == busConstant.FLAG_NO)
                        {
                            lstrSelected = busConstant.FLAG_NO;
                        }
                        else if (istrplan == busConstant.IAP && ibusBenefitApplicationEligiblePlans != null &&
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.iap_flag == busConstant.FLAG_NO)
                        {
                            lstrSelected = busConstant.FLAG_NO;
                        }
                        else if (istrplan == busConstant.Local_600 && ibusBenefitApplicationEligiblePlans != null &&
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local600_flag == busConstant.FLAG_NO)
                        {
                            lstrSelected = busConstant.FLAG_NO;
                        }
                        else if (istrplan == busConstant.Local_666 && ibusBenefitApplicationEligiblePlans != null &&
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local666_flag == busConstant.FLAG_NO)
                        {
                            lstrSelected = busConstant.FLAG_NO;
                        }
                        else if (istrplan == busConstant.LOCAL_700 && ibusBenefitApplicationEligiblePlans != null &&
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local700_flag == busConstant.FLAG_NO)
                        {
                            lstrSelected = busConstant.FLAG_NO;
                        }
                        else if (istrplan == busConstant.Local_161 && ibusBenefitApplicationEligiblePlans != null &&
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local161_flag == busConstant.FLAG_NO)
                        {
                            lstrSelected = busConstant.FLAG_NO;
                        }
                        else if (istrplan == busConstant.Local_52 && ibusBenefitApplicationEligiblePlans != null &&
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local52_flag == busConstant.FLAG_NO)
                        {
                            lstrSelected = busConstant.FLAG_NO;
                        }

                        busRetirementEligiblePlans lRetirementEligiblePlans = new busRetirementEligiblePlans { istrPlanName = istrplan, istrIsSelected = lstrSelected };
                        iclbRetirementEligiblePlans.Add(lRetirementEligiblePlans);
                    }
                }
            }
        }
        public void LoadApprovedBenefitAppDetails(int aintBenefitApplicationId)
        {
            this.LoadApprovedBenefitApplicationDetails(aintBenefitApplicationId);
            foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in iclbApprovedBenefitApplicationDetail)
            {
                DataTable ldtbPlanBen = Select("cdoBenefitApplicationDetail.GetPlan&BenefitFromID", new object[1] { lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id });
                if (ldtbPlanBen.Rows.Count > 0)
                {
                    DataRow ldtrRow = ldtbPlanBen.Rows[0];
                    lbusBenefitApplicationDetail.istrPlanName = Convert.ToString(ldtrRow[enmPlan.plan_name.ToString()]);
                    lbusBenefitApplicationDetail.iintPlan_ID = Convert.ToInt32(ldtrRow[enmPlan.plan_id.ToString()]);
                    lbusBenefitApplicationDetail.istrPlanCode = Convert.ToString(ldtrRow[enmPlan.plan_code.ToString()]);
                    lbusBenefitApplicationDetail.istrPlanBenefitDescription = Convert.ToString(ldtrRow["DESCRIPTION"]);
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = Convert.ToString(ldtrRow["CODE_VALUE"]);
                }
            }
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

           

            this.LoadBenefitElectionDetails(this.ibusPerson.icdoPerson.person_id, this.intEligiblePlanId);

            this.CheckEligibility_Retirement();

            foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
            {
                busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();



                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id != 1)
                {
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = Convert.ToString(this.strBenefitOptionValue);
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id = lbusPlanBenefitXr.GetPlanBenefitId(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id, Convert.ToString(this.strBenefitOptionValue));
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.spousal_consent_flag = Convert.ToString(strSpouseConsent);
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id = this.intJointAnnuiant;
                    lbusBenefitApplicationDetail.istrPlanCode = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrPlanCode;
                    lbusBenefitApplicationDetail.iintPlan_ID = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id;
                    this.GetSubPlan(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id);
                }
                else
                {
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = Convert.ToString(this.strIAPBenefitOptionValue);
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id = lbusPlanBenefitXr.GetPlanBenefitId(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id, Convert.ToString(this.strIAPBenefitOptionValue));
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id = this.intIAPJointAnnuiant;
                    lbusBenefitApplicationDetail.istrPlanCode = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrPlanCode;
                    lbusBenefitApplicationDetail.iintPlan_ID = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id;
                    this.GetSubPlan(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintPlan_id);



                }
            }

            if (icdoBenefitApplication.benefit_application_id > 0)
            {
                
                foreach (busBenefitApplicationDetail lbusBenefitAppDetail in this.iclbBenefitApplicationDetail)
                {
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.benefit_application_detail_id = 0;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.ienuObjectState = ObjectState.Insert;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.application_detail_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.benefit_subtype_value = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusBenefitAppDetail.icdoBenefitApplicationDetail.iintPlan_id).First().icdoPersonAccount.istrRetirementSubType;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.benefit_application_id = icdoBenefitApplication.benefit_application_id;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.created_by = iobjPassInfo.istrUserID;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.created_date = DateTime.Now;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.modified_by = iobjPassInfo.istrUserID;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.modified_date = DateTime.Now;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.update_seq = 0;

                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.Insert();
                }
                this.btn_Approved();
                this.btn_CalculateBenefitClick();

                                        
            }

            //Create Withdrawal application for this benefit application
            this.icdoBenefitApplication.withdrawal_date = icdoBenefitApplication.retirement_date;
            this.DetermineBenefitSubTypeandEligibility_Withdrawal();
            // Create Withdrawal Application autiomatically after cretaing the retirement application
            if (icdoBenefitApplication.benefit_application_id > 0 && (this.EligibileforL161Spl == true || this.EligibileforL52Spl == true))
            {
                icdoBenefitApplicationWithdrawal = new cdoBenefitApplication();
                icdoBenefitApplicationWithdrawal.person_id = this.ibusPerson.icdoPerson.person_id;
                icdoBenefitApplicationWithdrawal.withdrawal_date = icdoBenefitApplication.retirement_date;
                icdoBenefitApplicationWithdrawal.application_received_date = icdoBenefitApplication.application_received_date;

                icdoBenefitApplicationWithdrawal.benefit_type_id = busConstant.BENEFIT_TYPE_CODE_ID;
                icdoBenefitApplicationWithdrawal.benefit_type_value = busConstant.BENEFIT_TYPE_WITHDRAWAL;
                icdoBenefitApplicationWithdrawal.application_status_id = busConstant.BENEFIT_APPLICATION_STATUS_CODE_ID;
                icdoBenefitApplicationWithdrawal.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                icdoBenefitApplicationWithdrawal.application_status_description = busConstant.BENEFIT_APPLICATION_STATUS_PENDING_DESC;
                icdoBenefitApplicationWithdrawal.terminally_ill_flag = icdoBenefitApplication.terminally_ill_flag;
                icdoBenefitApplicationWithdrawal.cancellation_reason_id = icdoBenefitApplication.cancellation_reason_id;
                icdoBenefitApplicationWithdrawal.created_by = icdoBenefitApplication.created_by;

                if (icdoBenefitApplicationWithdrawal.Insert() > 0)
                {
                    busBenefitApplicationStatusHistory lbusBenefitApplicationStatusHistory = new busBenefitApplicationStatusHistory();
                    lbusBenefitApplicationStatusHistory.InsertStatusHistory(icdoBenefitApplicationWithdrawal.benefit_application_id, icdoBenefitApplicationWithdrawal.application_status_value,
                       icdoBenefitApplicationWithdrawal.modified_date, icdoBenefitApplicationWithdrawal.modified_by);

                    //Insert data into the benefit application detail
                    icdoBenefitApplicationWithdrawalDetail = new cdoBenefitApplicationDetail();
                    icdoBenefitApplicationWithdrawalDetail.benefit_application_id = icdoBenefitApplicationWithdrawal.benefit_application_id;
                    icdoBenefitApplicationWithdrawalDetail.spousal_consent_flag = this.strSpouseConsent;
                    if(this.EligibileforL161Spl == true)
                        icdoBenefitApplicationWithdrawalDetail.l161_spl_acc_flag = busConstant.FLAG_YES;
                    else if(this.EligibileforL52Spl == true)
                        icdoBenefitApplicationWithdrawalDetail.l52_spl_acc_flag = busConstant.FLAG_YES;
                    icdoBenefitApplicationWithdrawalDetail.application_detail_status_id = busConstant.BENEFIT_APPLICATION_STATUS_CODE_ID;
                    icdoBenefitApplicationWithdrawalDetail.plan_benefit_id = busConstant.LIFE_PLAN_ID;
                    icdoBenefitApplicationWithdrawalDetail.created_by = icdoBenefitApplicationWithdrawal.created_by;
                    icdoBenefitApplicationWithdrawalDetail.Insert();

                }
            }
        }
        //PIR 999
        public bool CheckIfMPIPP()
        {
            if (iclbBenefitApplicationDetail != null && iclbBenefitApplicationDetail.Count > 0
                && iclbBenefitApplicationDetail.Where(t => t.istrPlanCode == busConstant.MPIPP && t.icdoBenefitApplicationDetail.ee_flag == busConstant.FLAG_YES).Count() > 0)
            {
                return true;
            }
            return false;
        }

        public override busBase GetCorPerson()
        {
            ibusPerson.LoadPersonAddresss();
            ibusPerson.LoadPersonContacts();
            ibusPerson.LoadCorrAddress();
            return this.ibusPerson;
        }
        # endregion


    }
}
