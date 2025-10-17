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
using Sagitec.CustomDataObjects;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;


    namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busBenefitCalculationPostRetirementDeath : busBenefitCalculationHeader
    {
        #region Properties
        public busPayeeAccount iintPayeeAcntID { get; set; }
        public string istrPlanName { get; set; }
        public string istrSurvivorType { get; set; }        
        public decimal idecSurvivorAgeAtDeath { get; set; }
        public decimal idecLumpSumBenefitAmount = busConstant.ZERO_DECIMAL;
        public decimal idecEEContributionAmount = busConstant.ZERO_DECIMAL;
        public decimal idecEEInterestAmount = busConstant.ZERO_DECIMAL;
        public decimal idecAgeDiff = busConstant.ZERO_DECIMAL;
        public decimal idecQualifiedJointAndSurvivorAnnuity50;
        public bool lblIsNew { get; set; }
        public decimal idecOverriddenSurvivorAmount { get; set; }

        public Collection<cdoPlanBenefitRate> iclbcdoPlanBenefitRate { get; set; }
        public busBenefitCalculationHeader lbusBenefitCalculationHeader { get; set; }
        public Collection<busBenefitCalculationOptions> iclbBenefitCalculationOptionsChildGrid { get; set; }
        public string istrFund { get; set; }

        public decimal idecL52Balance { get; set; }
        public decimal idecL161Balance { get; set; }
        public int PAYEE_ACCOUNT_ID { get; set; }
        public string istrmanger { get; set; }

       #endregion    

      
        public void FlushOptions()
        {
            foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
            {
                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES &&
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                    continue;
                if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
                {
                   
                    foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in lbusBenefitCalculationDetail.iclbBenefitCalculationOptions)
                    {
                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.Delete();
                    }

                    lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Clear();
                }

            }
        }
        public string Checkrole()
        {
            busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
            if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
            {
                return "Y";
            }
            return "N";
        }
        public override void BeforePersistChanges()
        {
            LoadSpecialAccountDetails();
            base.BeforePersistChanges();
        }

        public void LoadSpecialAccountDetails()
        {
            int lintPersonAccountID = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.person_account_id;
            foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
            {
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.l52_spl_acc_flag != "Y" &&
                    item.icdoBenefitCalculationDetail.l161_spl_acc_flag != "Y").Count() > 0)
                {
                    if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.l52_spl_acc_flag != "Y" &&
                        item.icdoBenefitCalculationDetail.l161_spl_acc_flag != "Y").First().iclbBenefitCalculationOptions.Count > 0)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSurvivorRelationshipDescription = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.l52_spl_acc_flag != "Y" &&
                            item.icdoBenefitCalculationDetail.l161_spl_acc_flag != "Y").First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_relationship_description;
                    }
                    else
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_header_id = this.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                    }
                }
                
                if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
                {
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id = busConstant.IAP_PLAN_ID;

                    if (!string.IsNullOrEmpty(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccount))
                    {
                      

                        if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id == 0)
                        {
                            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.l52_spl_acc_flag != "Y" &&
                                        item.icdoBenefitCalculationDetail.l161_spl_acc_flag != "Y").Count() > 0)
                            {
                                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_subtype_value = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.l52_spl_acc_flag != "Y" &&
                                        item.icdoBenefitCalculationDetail.l161_spl_acc_flag != "Y").FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value;

                            }

                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id = busConstant.IAP_PLAN_ID;
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id = lintPersonAccountID;
                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccount == busConstant.L52_SPL_ACC)
                            {
                                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag = busConstant.FLAG_YES;
                                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount = idecL52Balance;
                                InsertBenefitOptionsForSpecialAccounts(lbusBenefitCalculationDetail);
                            }
                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccount == busConstant.L161_SPL_ACC)
                            {
                                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag = busConstant.FLAG_YES;
                                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount = idecL161Balance;
                                InsertBenefitOptionsForSpecialAccounts(lbusBenefitCalculationDetail);
                            }

                        }
                    }
                }

            }
        }

        public void InsertBenefitOptionsForSpecialAccounts(busBenefitCalculationDetail abusBenefitCalculationDetail)
        {
            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            if (abusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccount == busConstant.L52_SPL_ACC)
            {
                abusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag = busConstant.FLAG_YES;
                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                LoadBenefitOptionForSpecialAccounts(abusBenefitCalculationDetail, true, false, abusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrBenefitOptionValue, abusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount);
            }
            if (abusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccount == busConstant.L161_SPL_ACC)
            {
                abusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag = busConstant.FLAG_YES;
                lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                LoadBenefitOptionForSpecialAccounts(abusBenefitCalculationDetail, false, true, abusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrBenefitOptionValue, abusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount);
            }
        }

        public void LoadBenefitOptionForSpecialAccounts(busBenefitCalculationDetail abusBenefitCalculationDetail, bool ablnL52Flag,bool ablnL161Flag,string astrBenefitOptionValue,decimal adecAccountBalance)
        {
            int lintPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, astrBenefitOptionValue);

            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            decimal ldecBenefitOptionFactor = decimal.One;

            decimal ldecAnnunityAdjustmentMultiplier = this.GetAnnunityMultiplier();

            int lintRemainder = 0;
            decimal ldecSpecialAccountAnnuity = adecAccountBalance;
            if (astrBenefitOptionValue == busConstant.LIFE_ANNUTIY)
            {
                int lintAge = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAgeInDec(this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date)));

               ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(busConstant.BENEFIT_TYPE_WITHDRAWAL, ibusCalculation.GetPlanBenefitId(busConstant.IAP_PLAN_ID, busConstant.LIFE),
                            lintAge, busConstant.ZERO_INT) * 12;

                ldecSpecialAccountAnnuity = Math.Round(adecAccountBalance / ldecBenefitOptionFactor);
                Math.DivRem(Convert.ToInt32(ldecSpecialAccountAnnuity), 10, out lintRemainder);
                if (lintRemainder > 0)
                {
                    ldecSpecialAccountAnnuity = ldecSpecialAccountAnnuity - lintRemainder;
                }
            }
            if (this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, astrBenefitOptionValue == busConstant.LIFE_ANNUTIY ? ldecAnnunityAdjustmentMultiplier != 0 ? ldecSpecialAccountAnnuity * ldecAnnunityAdjustmentMultiplier : ldecSpecialAccountAnnuity: ldecSpecialAccountAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, astrBenefitOptionValue,
                                        astrBenefitOptionValue == busConstant.LIFE_ANNUTIY ? ldecAnnunityAdjustmentMultiplier != 0 ? ldecSpecialAccountAnnuity * ldecAnnunityAdjustmentMultiplier : ldecSpecialAccountAnnuity: ldecSpecialAccountAnnuity, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, ablnL52Flag, ablnL161Flag);
            }
            else
            {
                lbusBenefitCalculationOptions.LoadData(lintPlanBenefitId, ldecBenefitOptionFactor, ldecSpecialAccountAnnuity, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, astrBenefitOptionValue,
                                        ldecSpecialAccountAnnuity, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, ablnL52Flag, ablnL161Flag);
            }

            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.l52_spl_acc_flag != "Y" &&
                        item.icdoBenefitCalculationDetail.l52_spl_acc_flag != "Y").Count() > 0)
            {
                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_relationship_value = this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.l52_spl_acc_flag != "Y" &&
                            item.icdoBenefitCalculationDetail.l52_spl_acc_flag != "Y").First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_relationship_value;
            }

            abusBenefitCalculationDetail.iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();

            abusBenefitCalculationDetail.iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

        }

        public void GetFund()
        {
            if (this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES && this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
            {
                istrFund = busConstant.EE_UVHP;
            }
            if (this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES)
            {
                istrFund = busConstant.EE;
            }
            if (this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
            {
                istrFund = busConstant.UVHP;
            }
            if (this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
            {
                istrFund = busConstant.LOCAL_52_SPECIAL_ACCOUNT;
                icdoBenefitCalculationHeader.istrBenefitOptionValue =
                    this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;
            }
            if (this.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
            {
                istrFund = busConstant.LOCAL1_161_SPECIAL_ACCOUNT;
                icdoBenefitCalculationHeader.istrBenefitOptionValue =
                this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;
            }
        }
        
        public ArrayList btn_RefreshCalculation()
        {
            ArrayList larrList = new ArrayList();

            #region FLAGS
            //Flags to be used for making sure we donot calculate again if another IAP entry comes along

            bool lblnL52SplFlag = false;
            bool lblnL161SplFlag = false;
            bool lblnUVHPFlag = false;

            this.iblnCalcualteUVHPBenefit = this.iblnCalculateIAPBenefit = this.iblnCalculateL161SplAccBenefit = this.iblnCalculateL52SplAccBenefit = this.iblnCalculateMPIPPBenefit = false;
            #endregion                                           

            //this.SetupPreRequisites_RetirementCalculations();
            string lstrBenefitOptionValue = string.Empty;
            this.ValidateHardErrors(utlPageMode.Update);

            if (this.iclbBenefitCalculationDetail.Count() > 0)
            {
                this.icdoBenefitCalculationHeader.iintPlanId = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id;
            }

            if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).Count() > 0
                || this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).Count() > 0)
            {
                lstrBenefitOptionValue = this.icdoBenefitCalculationHeader.istrBenefitOptionValue;
            }
           

            if (!lblIsNew)
                FlushOlderCalculations();

            if (this.ibusBenefitApplication.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id))
            {
                this.ibusBenefitApplication.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();
                this.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                this.ibusPerson.LoadPersonAccounts();
                this.ibusBenefitApplication.LoadBenefitApplicationDetails();
                //this.LoadAllRetirementContributions();

                if (!ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    LoadAllRetirementContributions(null);
                }



                if (!this.ibusBenefitApplication.iclbBenefitApplicationDetail.IsNullOrEmpty())
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.ibusBenefitApplication.iclbBenefitApplicationDetail)
                    {
                        if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
                        {
                            #region IAP plan found in grid
                            if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty() && !iblnCalculateIAPBenefit)
                            {
                                this.iblnCalculateIAPBenefit = true;
                            }
                            else if (this.ibusBenefitApplication.iclbBenefitApplicationDetail.Where(item => item.istrSubPlan == busConstant.L161_SPL_ACC && item.iintPlan_ID == busConstant.IAP_PLAN_ID).Count() > 0 && !lblnL161SplFlag)
                            {
                                this.iblnCalculateL161SplAccBenefit = true;
                                lblnL161SplFlag = true;
                            }
                            else if (this.ibusBenefitApplication.iclbBenefitApplicationDetail.Where(item => item.istrSubPlan == busConstant.L52_SPL_ACC && item.iintPlan_ID == busConstant.IAP_PLAN_ID).Count() > 0 && !lblnL52SplFlag)
                            {
                                this.iblnCalculateL52SplAccBenefit = true;
                                lblnL52SplFlag = true;
                            }
                            if (this.ibusBenefitApplication.CheckAlreadyVested(lbusBenefitApplicationDetail.istrPlanCode))
                            {
                                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                                {
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                                }

                                if (this.iblnCalculateL52SplAccBenefit || this.iblnCalculateL161SplAccBenefit)
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lstrBenefitOptionValue;

                                this.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                                 this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                                 lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                                 lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);
                                try
                                {
                                    this.AfterPersistChanges();
                                }
                                catch
                                {
                                }
                            }
                            #endregion
                        }
                        else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && this.icdoBenefitCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
                        {
                            #region MPIPP PLAN FOUNd in GRID
                            if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty() && !this.iblnCalculateMPIPPBenefit)
                            {
                                this.iblnCalculateMPIPPBenefit = true;

                            }

                            else if (this.ibusBenefitApplication.iclbBenefitApplicationDetail.Where(item => item.istrSubPlan == busConstant.UVHP && item.iintPlan_ID == busConstant.MPIPP_PLAN_ID).Count() > 0 && !lblnUVHPFlag)
                            {
                                this.iblnCalcualteUVHPBenefit = true;
                                lblnUVHPFlag = true;
                            }

                            if (this.ibusBenefitApplication.CheckAlreadyVested(lbusBenefitApplicationDetail.istrPlanCode))
                            {

                                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                                {
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                                }



                                this.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                                 this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                                 lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                                 lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);
                                try
                                {
                                    this.AfterPersistChanges();
                                }
                                catch
                                {
                                }
                            }

                            #endregion
                        }

                        else if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID && lbusBenefitApplicationDetail.iintPlan_ID != busConstant.IAP_PLAN_ID)
                        {
                            #region LOCAL PLAN FOUND
                            if (this.ibusBenefitApplication.CheckAlreadyVested(lbusBenefitApplicationDetail.istrPlanCode))
                            {

                                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                                {
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                                }

                                this.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                                 this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                                 lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                                 lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);
                                try
                                {
                                    this.AfterPersistChanges();
                                }
                                catch
                                {
                                }
                            }

                            #endregion
                        }
                    }
                }
            }
            this.EvaluateInitialLoadRules();
            if (this.iclbBenefitCalculationDetail.Count() > 0)
            {
                this.iclbBenefitCalculationDetail.First().EvaluateInitialLoadRules();
                this.iclbBenefitCalculationDetail.First().iobjMainCDO = this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail;
            }

            larrList.Add(this);
            return larrList;
        }

        public void SetupPreRequisites_RetirementCalculations()
        {
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.ForEach(a => a.icdoPersonAccount.istrRetirementSubType = string.Empty);
                this.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Retirement();

                if (this.icdoBenefitCalculationHeader.iintPlanId.IsNotNull() && this.icdoBenefitCalculationHeader.iintPlanId > 0 && this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType.IsNotNullOrEmpty())
                {
                    this.icdoBenefitCalculationHeader.istrRetirementType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                }
            }
        }

        public override void ValidateHardErrors(Sagitec.Common.utlPageMode aenmPageMode)
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            this.EvaluateInitialLoadRules();

            if (this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth != DateTime.MinValue && this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth >= DateTime.Now)
            {
                lobjError = AddError(1131, " ");
                this.iarrErrors.Add(lobjError);
            }          
             
            base.ValidateHardErrors(aenmPageMode);
        }

        public void SpawnFinalRetirementCalculation(int aintBenefitApplicationDetailId, int aintPersonAccountId, int aintPlanId, string astrPlanCode,
                                      DateTime adtVestedDate, string astrBenefitSubTypeValue, string astrBenefitOptionValue)
        {
            decimal ldecLateAdjustmentAmt = busConstant.ZERO_DECIMAL;
            //HEADER LOADING WLL BE DONE SEPARATELY FROM OUTISDE in busBenefitApplication itself
            //This method would accpet many of those parameters 
            // Load Benefit Calculation Detail.
            //Over here we need to make sure both the OBJECTS above are filled just like as if they were filled in CALCULATE AND SAVE          
            this.PopulateInitialDataBenefitCalculationDetails(aintBenefitApplicationDetailId, aintPersonAccountId, aintPlanId, astrPlanCode, adtVestedDate, astrBenefitSubTypeValue);

            //SetupPreRequisites_RetirementCalculations(); //WHY IS THIS REQUIRED ??

            #region SWITCH CASE - INITIATE CALCULATION BASED ON THE PLAN
            if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                decimal ldecTotalBenefitAmount;

                switch (astrPlanCode)
                {
                    case busConstant.Local_161:
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal161(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType,
                                                                                 this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                 this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                                 false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                                 this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                                 null, this.iclbBenefitCalculationDetail,
                                                                                 Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                                 this.iclbPersonAccountRetirementContribution, busConstant.BOOL_TRUE, this.ibusPerson.icdoPerson.person_id);



                        if (ibusBenefitApplication.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES)//Mahua Min distribution realted changes
                        {
                            CalculateFinalBenefitForPensionBenefitOptions(ldecTotalBenefitAmount, astrBenefitOptionValue);
                        }
                        else
                        {
                            CalculateLocal161BenefitOptions(astrBenefitOptionValue, ldecTotalBenefitAmount);
                        }
                        break;

                    case busConstant.Local_52:
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal52(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType,
                                                     this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                     this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                     false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                     this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                     null, this.iclbBenefitCalculationDetail,
                                                     Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                     this.iclbPersonAccountRetirementContribution, busConstant.BOOL_TRUE, this.ibusPerson.icdoPerson.person_id);

                        if (ibusBenefitApplication.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES)//Mahua Min distribution realted changes
                        {
                            CalculateFinalBenefitForPensionBenefitOptions(ldecTotalBenefitAmount, astrBenefitOptionValue);
                        }
                        else
                        {
                            CalculateLocal52BenefitOptions(astrBenefitOptionValue, ldecTotalBenefitAmount);
                        }
                        break;

                    case busConstant.Local_600:
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal600(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType,
                                                                                                          this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                                                          this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                                                           false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                                                           this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                                                                           null, this.iclbBenefitCalculationDetail,
                                                                                                           Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                                                           this.iclbPersonAccountRetirementContribution, busConstant.BOOL_TRUE, this.ibusPerson.icdoPerson.person_id);


                        if (ibusBenefitApplication.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES)//Mahua Min distribution realted changes
                        {
                            CalculateFinalBenefitForPensionBenefitOptions(ldecTotalBenefitAmount, astrBenefitOptionValue);
                        }
                        else
                        {
                            CalculateLocal600BenefitOptions(astrBenefitOptionValue, ldecTotalBenefitAmount);
                        }
                        break;

                    case busConstant.Local_666:
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal666(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType,
                                                                              this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                                              this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                                               false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                                               this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount), this.icdoBenefitCalculationHeader.age,
                                                                               null, this.iclbBenefitCalculationDetail,
                                                                               Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                                               this.iclbPersonAccountRetirementContribution, busConstant.BOOL_TRUE, this.ibusPerson.icdoPerson.person_id);


                        if (ibusBenefitApplication.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES)//Mahua Min distribution realted changes
                        {
                            CalculateFinalBenefitForPensionBenefitOptions(ldecTotalBenefitAmount, astrBenefitOptionValue);
                        }
                        else
                        {
                            CalculateLocal666BenefitOptions(astrBenefitOptionValue, ldecTotalBenefitAmount);
                        }
                        break;

                    case busConstant.LOCAL_700:
                        ldecTotalBenefitAmount = this.ibusCalculation.CalculateTotalBenefitAmtForLocal700(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType,
                                                  this.icdoBenefitCalculationHeader.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth,
                                                  this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount),
                                                   false, this.ibusBenefitApplication, this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_pre_bis_amount),
                                                   this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Sum(item => item.icdoPersonAccountRetirementContribution.local_post_bis_amount),
                                                   null, this.iclbBenefitCalculationDetail,
                                                   Convert.ToInt32(this.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.person_account_id == aintPersonAccountId).Max(item => item.icdoPersonAccountRetirementContribution.computational_year)),
                                                   this.iclbPersonAccountRetirementContribution, busConstant.BOOL_TRUE, this.ibusPerson.icdoPerson.person_id);


                        if (ibusBenefitApplication.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES)//Mahua Min distribution realted changes
                        {
                            CalculateFinalBenefitForPensionBenefitOptions(ldecTotalBenefitAmount, astrBenefitOptionValue);
                        }
                        else
                        {
                            CalculateLocal700BenefitOptions(astrBenefitOptionValue, ldecTotalBenefitAmount);
                        }
                        break;

                    case busConstant.MPIPP:
                        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                        {
                            if (this.iblnCalculateMPIPPBenefit)
                            {
                                busPersonAccount lbusPersonAccount = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).FirstOrDefault();
                                decimal ldecBenefitAmt = ibusCalculation.CalculateBenefitAmtForPension(ibusPerson, busConstant.BENEFIT_TYPE_RETIREMENT, icdoBenefitCalculationHeader.age, icdoBenefitCalculationHeader.retirement_date,
                                    this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                    this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).FirstOrDefault(),
                                    this.ibusBenefitApplication, false, iclbBenefitCalculationDetail, this.iclbPersonAccountRetirementContribution, null, true, lbusPersonAccount.icdoPersonAccount.istrRetirementSubType, ref ldecLateAdjustmentAmt, this.ibusPerson.icdoPerson.person_id);

                                #region Check if Withdrawal History Exists: Then Acrrued benefit = Accrued Benefit - EE derived

                                //ldecBenefitAmt = ibusCalculation.CalculateAccruedBenefitForPersonWithWithdrawal(
                                //                            this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                //                            ldecBenefitAmt,this.ibusBenefitApplication.ibusPerson, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount, icdoBenefitCalculationHeader.retirement_date, 
                                //                            this.ibusBenefitApplication.aclbPersonWorkHistory_MPI, this.ibusBenefitApplication.ibusPerson.iclbPersonAccountRetirementContribution,
                                //                            this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year);
                                                           
                                this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).
                                                                                              FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount = ldecBenefitAmt;
                                #endregion

                                if (ldecLateAdjustmentAmt == 0)
                                {
                                    CalculateFinalBenefitForPensionBenefitOptions(ldecBenefitAmt, astrBenefitOptionValue);
                                }
                                else
                                {
                                    CalculateFinalBenefitForPensionBenefitOptions(ldecLateAdjustmentAmt, astrBenefitOptionValue);
                                }
                            }

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

        private decimal CalculateFinalBenefitForPensionBenefitOptions(decimal adecFinalAccruedBenefitAmount, string astrBenefitOptionValue)
        {

            int lintParticipantAge = Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge);
            int lintSurvivorAge = Convert.ToInt32(Math.Floor(icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement));
            decimal ldecBenefitOptionFactor = busConstant.ZERO_DECIMAL;
            decimal ldecFinalBenefitAmount = busConstant.ZERO_DECIMAL;
            busBenefitCalculationOptions lbusBenefitCalculationOptions;


            // Calculate the total benefit amount of MPIPP & Locals
            // Need to call the Local Code to fetch the Local Frozen Benefit Amount with ERF
            decimal ldecTotalLocalLumpsumAmount = ibusCalculation.GetLocalLumpsumBenefitAmount(icdoBenefitCalculationHeader.age, ibusBenefitApplication, ibusPerson,
                                       icdoBenefitCalculationHeader.retirement_date, iclbPersonAccountRetirementContribution);
            // Calculate the Monthly Exclusion Amount & Minimum Guarantee
            //int lintPersonAccountId = ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id;
            ibusCalculation.CalculateMEAAndMG(adecFinalAccruedBenefitAmount, ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First(),
                                    this.idecLumpSumBenefitAmount, this.icdoBenefitCalculationHeader.iintPlanId,
                                    Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge), this.icdoBenefitCalculationHeader.retirement_date, Convert.ToInt32(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement),
                                    this.ibusBenefitApplication.QualifiedSpouseExists, busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail, 
                                    this.iclbPersonAccountRetirementContribution,icdoBenefitCalculationHeader.calculation_type_value,
                                    this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, string.Empty);


            // Calculate the Final Benefit Amounts for all Benefit Options

            // Calculate the Lumpsum Benefit Option
            //Ticket - 61531
            ldecBenefitOptionFactor = Math.Round(this.GetLumpsumBenefitFactor((int)this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.retirement_date.Year) * 12,3);
            decimal ldecLumpSumBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));

            switch (astrBenefitOptionValue)
            {
                case busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY:
                    // Qualified Joint And 50% Survivor Annuity Benefit Option
                    ldecBenefitOptionFactor = 1;
                    double ldblBenefitOptionFactorJnS50 = 0.86 + 0.005 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
                    ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS50, 3));
                    //PIR-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor * 0.50m, 2)));

                    // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit option
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    // Qualified Joint And 75% Survivor Annuity Benefit Option
                    ldecBenefitOptionFactor = 1;
                    double ldblBenefitOptionFactorJnS75 = 0.80 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
                    ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS75, 3));
                    //PIR-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor * 0.75m, 2)));

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY:
                    // Qualified Joint And 100% Survivor Annuity Benefit Option
                    ldecBenefitOptionFactor = 1;
                    double ldblBenefitOptionFactorJnS100 = 0.75 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
                    ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS100, 3));
                    //PIR-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.JOINT_50_PERCENT_POPUP_ANNUITY:
                    // Joint And 50% Survivor Pop-up Annuity Benefit Option
                    ldecBenefitOptionFactor = 1;
                    double ldblBenefitOptionFactorJnS50Pop = 0.83 + 0.007 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
                    ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS50Pop, 3));
                    //PIR-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor * 0.50m, 2)));

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.JOINT_100_PERCENT_POPUP_ANNUITY:
                    // Joint And 100% Survivor Pop-up Annuity Benefit Option
                    ldecBenefitOptionFactor = 1;
                    double ldblBenefitOptionFactorJnS100Pop = 0.71 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.008 * (65 - lintParticipantAge);
                    ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactorJnS100Pop, 3));
                    //PIR-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY), ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    // Annuity Benefit Option
                    int lPlanBenefitId = ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY);
                    ldecBenefitOptionFactor = ibusCalculation.GetBenefitOptionFactor(this.icdoBenefitCalculationHeader.benefit_type_value, lPlanBenefitId, lintParticipantAge, busConstant.ZERO_INT);
                    //PIR-940
                    if (ldecBenefitOptionFactor > 1)
                    {
                        ldecBenefitOptionFactor = 1;
                    }
                    ldecFinalBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(lPlanBenefitId, ldecBenefitOptionFactor, ldecFinalBenefitAmount,
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.LUMP_SUM:
                    // Lumpsum Benefit Option
                    ldecFinalBenefitAmount = this.idecLumpSumBenefitAmount;
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LUMP_SUM), ldecBenefitOptionFactor,
                                                            this.idecLumpSumBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                case busConstant.LIFE_ANNUTIY:
                    // Life Annuity – reduction factors applied if taking any early retirement benefit other that un-reduced
                    ldecBenefitOptionFactor = 1;
                    ldecFinalBenefitAmount = adecFinalAccruedBenefitAmount;
                    lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.LIFE_ANNUTIY), ldecBenefitOptionFactor,
                                                            adecFinalAccruedBenefitAmount, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id, busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
                    break;

                default:
                    break;
            }

            return ldecFinalBenefitAmount;
        }

        public void CalculateLocal161BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;

            #endregion


            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);


            #region Swtich Case to determine the Factors and Amounts
            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 161 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                    //Query to SGT_PLAN_BENEFIT_XR to get PLAN_BENEFIT_ID
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.istrBenefitOptionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1504, busConstant.LUMP_SUM);
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.plan_benefit_id = ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM);
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_option_factor = ldecFactor;
                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount = Convert.ToDecimal(Math.Round(ldecTotalBenefitAmount * ldecFactor, 2, MidpointRounding.AwayFromZero));

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(adecTotalBenefitAmount * ldecFactor),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);



                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_SURVIVOR_ANNUITY
                            //aarti
                            int lintYears;
                            int lintMonths;
                            int lintDays;
                            int lintBeneficiaryYears;
                            int lintBeneficiaryMonth;
                            int lintBeneficiaryDays;

                            busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth, out lintYears, out lintMonths, out lintDays);
                            busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, out lintBeneficiaryYears, out lintBeneficiaryMonth, out lintBeneficiaryDays);

                            ldecFactor = ibusCalculation.GetBenefitProvisionBenefitOptionFactor(busConstant.LOCAL_161_PLAN_ID, busConstant.BENEFIT_TYPE_RETIREMENT, ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.QJ50), lintYears, lintMonths, lintBeneficiaryYears, lintBeneficiaryMonth);

                            //ldecFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Math.Round(adecTotalBenefitAmount * ldecFactor, 2);
                            //PIR 229 : Local 161 
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                            Math.Round(adecTotalBenefitAmount * ldecFactor, 2),
                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                            busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Math.Round(this.idecQualifiedJointAndSurvivorAnnuity50 * 0.5M, 2));

                            // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit Option
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            ldecFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                        Convert.ToDecimal(adecTotalBenefitAmount * ldecFactor),
                                        busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                        busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 75 / 100)));


                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(adecTotalBenefitAmount * Decimal.One),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }

                    break;

                case busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {

                        int lintYears;
                        int lintMonths;
                        int lintDays;
                        int lintBeneficiaryYears;
                        int lintBeneficiaryMonths;
                        int lintBeneficiaryDays;

                        busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.ibusPerson.icdoPerson.idtDateofBirth, out lintYears, out lintMonths, out lintDays);
                        busGlobalFunctions.GetDetailTimeSpan(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, out lintBeneficiaryYears, out lintBeneficiaryMonths, out lintBeneficiaryDays);

                        ldecFactor = ibusCalculation.GetBenefitProvisionBenefitOptionFactor(busConstant.LOCAL_161_PLAN_ID, busConstant.BENEFIT_TYPE_RETIREMENT, ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.QJ50), lintYears, lintMonths, lintBeneficiaryYears, lintBeneficiaryMonths);

                        //ldecFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Math.Round(adecTotalBenefitAmount * ldecFactor, 2),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Math.Round(Math.Round(adecTotalBenefitAmount * ldecFactor, 2) * 50 / 100, 2));

                        // No need to show the Relative Value for this Benefit Option
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.Local_161, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 75 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

                case busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * Decimal.One)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_161_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;

            }

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
            #endregion
        }

        public void CalculateLocal52BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {

            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region SwitchCase Based on BenefitOptions to be Calculated
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 52 LUMP SUM
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                            // PIR - 371
                            ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor));
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 50 / 100)));

                            // No need to show the Relative Value for this Benefit Option
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.006)) + Convert.ToDecimal(0.895), Convert.ToDecimal(0.99));
                            // PIR - 371
                            ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) * 75 / 100));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_100_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.006)) + Convert.ToDecimal(0.85), Convert.ToDecimal(0.99));
                            // PIR - 371
                            ldecFactor = ibusCalculation.GetRetirementL52Factor(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) * 100 / 100));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }
                    break;
                case busConstant.JOINT_50_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 50 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.006)) + Convert.ToDecimal(0.895), Convert.ToDecimal(0.99));
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 75 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.JOINT_100_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.006)) + Convert.ToDecimal(0.85), Convert.ToDecimal(0.99));
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 100 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * Decimal.One)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_52_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
        }

        public void CalculateLocal600BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region SwitchCase Based on BenefitOptions to be Calculated
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 600 LUMP SUM
                    //Ticket : 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                        Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor));
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 50 / 100)));


                            // No  need to show the Relative Value for this Benefit Option.
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.85), Convert.ToDecimal(1.0));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 75 / 100)));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }
                    break;
                case busConstant.JOINT_50_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.005)) + Convert.ToDecimal(0.94), Convert.ToDecimal(0.99));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 50 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.006)) + Convert.ToDecimal(0.895), Convert.ToDecimal(0.99));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL600Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) * 75 / 100)));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), Decimal.One,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * Decimal.One)),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_600_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion

            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_600_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;
        }

        public void CalculateLocal666BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region Switch Case Based on Benefit Options We have to Calculate
            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            switch (astrBenefitOption)
            {
                case busConstant.CodeValueAll:

                    #region Local 666 LUMP SUM
                    //Ticket - 61531
                    ldecFactor =Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum <= busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    #endregion

                    if (ldecTotalLumpSum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        if (lblnCheckIfSpouse)
                        {
                            #region JOINT_50_PERCENT_POPUP_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.89), Convert.ToDecimal(0.99));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M);
                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                            Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                            busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 0.5M));

                            // No need to set Relative Value for this Benefit option
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion

                            #region JOINT_75_PERCENT_SURVIVOR_ANNUITY
                            //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.81), Convert.ToDecimal(1));
                            // PIR - 760
                            ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                            lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                            lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 0.75M));

                            //if (Math.Round(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) / this.idecQualifiedJointAndSurvivorAnnuity50, 2) == 0.95m)
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = busConstant.BenefitCalculation.RELATIVE_VALUE_APPROX_EQUAL;
                            //}
                            //else
                            //{
                            //    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = Math.Round(Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)) / this.idecQualifiedJointAndSurvivorAnnuity50 * 100, 2).ToString() + " %";
                            //}

                            lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                            #endregion
                        }

                        #region THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY), Decimal.One,
                                                                Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.ZERO_DECIMAL);


                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                        #endregion
                    }
                    break;

                case busConstant.JOINT_50_PERCENT_POPUP_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.89), Convert.ToDecimal(0.99));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 50 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        //ldecFactor = Math.Min((Math.Round((this.icdoBenefitCalculationHeader.age - this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), 0) * Convert.ToDecimal(0.004)) + Convert.ToDecimal(0.81), Convert.ToDecimal(1));
                        // PIR - 760
                        ldecFactor = ibusCalculation.GetRetirementL666Factor(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 75 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY), Decimal.One,
                                                            Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount / 0.5M) * 0.5M),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.ZERO_DECIMAL);

                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_666_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion
             decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_666_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;

        }

        public void CalculateLocal700BenefitOptions(string astrBenefitOption, decimal adecTotalBenefitAmount)
        {
            #region Variables Required in Switch Case
            decimal ldecFactor = new decimal();
            busBenefitCalculationOptions lbusBenefitCalculationOption;
            #endregion

            #region Switch Case Based on Benefit Options We have to Calculate

            bool lblnCheckIfSpouse = busConstant.BOOL_FALSE;
            lblnCheckIfSpouse = this.ibusCalculation.CheckIfSpouse(this.icdoBenefitCalculationHeader.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id);

            switch (astrBenefitOption)
            {
                case busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 50 / 100));

                        // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit Option
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 44 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_66_2by3_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                                                Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                                busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                                this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                                busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 75 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY:
                    if (lblnCheckIfSpouse)
                    {
                        ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                        lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                        lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY), ldecFactor,
                                        Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                        busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                        this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                        busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M) * 100 / 100));

                        lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    } break;

                case busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY:
                    ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY), ldecFactor,
                                                            Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * ldecFactor) / 0.5M) * 0.5M),
                                                            busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                                            this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                            busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY:
                    ldecFactor = GetBenefitFactorLocal(busConstant.LOCAL_700, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.BENEFIT_TYPE_RETIREMENT, this.icdoBenefitCalculationHeader.idecParticipantFullAge);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount / ldecFactor) / 0.5M) * 0.5M),  //PIR 833 should be divide instead of multiply by factor
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, busConstant.ZERO_DECIMAL);

                    //lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LIFE_ANNUTIY:
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LIFE_ANNUTIY), Decimal.One,
                                    Convert.ToDecimal(Math.Ceiling((adecTotalBenefitAmount * Decimal.One) / 0.5M) * 0.5M),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LIFE_ANNUTIY, busConstant.ZERO_DECIMAL);

                    lbusBenefitCalculationOption.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.LIFE_ANNUTIY, this.icdoBenefitCalculationHeader.idecParticipantFullAge, this.icdoBenefitCalculationHeader.idecSurvivorFullAge);
                    this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    break;

                case busConstant.LUMP_SUM:
                    //Ticket - 61531
                    ldecFactor = Math.Round(ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12,3);
                    lbusBenefitCalculationOption = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };

                    lbusBenefitCalculationOption.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.LOCAL_700_PLAN_ID, busConstant.LUMP_SUM), ldecFactor,
                                    Convert.ToDecimal(Math.Ceiling(adecTotalBenefitAmount * ldecFactor)),
                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER,
                                    this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                    busConstant.LUMP_SUM, busConstant.ZERO_DECIMAL);

                    decimal ldecTotalLumpSum = GetMPIPHPLumpSum() + lbusBenefitCalculationOption.icdoBenefitCalculationOptions.benefit_amount;
                    if (ldecTotalLumpSum < busConstant.BenefitCalculation.LUMP_SUM_LIMIT)
                    {
                        this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOption);
                    }
                    break;
            }
            #endregion
           
            decimal ldecLumpSumFactor = ldecFactor = ibusCalculation.GetLumpsumBenefitFactorByAGE(Convert.ToInt32(this.icdoBenefitCalculationHeader.idecParticipantFullAge)) * 12;
            ldecLumpSumFactor = Math.Round(ldecLumpSumFactor, 3);
            decimal ldecPresentValue = Math.Round(adecTotalBenefitAmount * ldecLumpSumFactor, 3);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.LOCAL_700_PLAN_ID).First().icdoBenefitCalculationDetail.present_value_amount = ldecPresentValue;

        }

        /// <summary>
        /// Iap Balance will be fetched from retirement's payee account attached.
        /// Special Accounts needs to be calculaculated.
        /// Discuss once when will we push special accounts allocation in Retirement contribution table.
        /// </summary>
        private void CalculateIAPBenefitAmount()
        {
            decimal ldecIAPHours4QtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHoursA2forQtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPPercent4forQtrAlloc = busConstant.ZERO_DECIMAL;

            decimal ldecAnnunityAdjustmentMultiplier = this.GetAnnunityMultiplier();

            #region To Set Values for IAP QTR Allocations
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            SqlParameter[] parameters = new SqlParameter[3];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
            SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);

            param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
            parameters[0] = param1;

            busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
            lbusIapAllocationSummary.LoadLatestAllocationSummary();

            param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year);
            parameters[1] = param2;

            param3.Value = busGlobalFunctions.GetLastDayOfWeek(this.ibusBenefitApplication.icdoBenefitApplication.retirement_date); //PROD PIR 113
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
                        busCalculation lbusCalculation = new busCalculation();
                        lbusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_FALSE, null, this.iclbBenefitCalculationDetail, this, null,
                                                                       this.ibusBenefitApplication.icdoBenefitApplication.retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc);
                    }
                }
            }
        }

        private void CalculateJointAndSurvivorBenefitOptions(decimal adecFinalAccruedBenefitAmount)
        {
            decimal ldecBenefitOptionFactor = busConstant.ZERO_DECIMAL;
            decimal ldecBenefitAmount = busConstant.ZERO_DECIMAL;
            busBenefitCalculationOptions lbusBenefitCalculationOptions;
            int lintParticipantAge = Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.age));
            int lintSurvivorAge = Convert.ToInt32(Math.Floor(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement));

            // Resetting the common variables
            ldecBenefitOptionFactor = 1;
            double ldblBenefitOptionFactor = 1;

            // Qualified Joint And 50% Survivor Annuity Benefit Option
            ldblBenefitOptionFactor = 0.86 + 0.005 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
            ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactor, 6));
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            this.idecQualifiedJointAndSurvivorAnnuity50 = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, this.idecQualifiedJointAndSurvivorAnnuity50,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor * 0.50m, 2)));

            // No Need to show the Relative Value for JOINT_50_PERCENT_SURVIVOR_ANNUITY Benefit Option
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            // Resetting the common variables
            ldecBenefitOptionFactor = 1;
            ldblBenefitOptionFactor = 1;
            ldecBenefitAmount = busConstant.ZERO_DECIMAL;

            // Qualified Joint And 75% Survivor Annuity Benefit Option
            ldblBenefitOptionFactor = 0.80 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
            ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactor, 6));
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            ldecBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, ldecBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor * 0.75m, 2)));


            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_75_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);


            // Resetting the common variables
            ldecBenefitOptionFactor = 1;
            ldblBenefitOptionFactor = 1;
            ldecBenefitAmount = busConstant.ZERO_DECIMAL;

            // Qualified Joint And 100% Survivor Annuity Benefit Option
            ldblBenefitOptionFactor = 0.75 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
            ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactor, 6));
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            ldecBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY), ldecBenefitOptionFactor, ldecBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));


            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            // Resetting the common variables
            ldecBenefitOptionFactor = 1;
            ldblBenefitOptionFactor = 1;
            ldecBenefitAmount = busConstant.ZERO_DECIMAL;

            // Joint And 50% Survivor Pop-up Annuity Benefit Option
            ldblBenefitOptionFactor = 0.83 + 0.007 * (lintSurvivorAge - lintParticipantAge) + 0.006 * (65 - lintParticipantAge);
            ldecBenefitOptionFactor = Convert.ToDecimal(Math.Round(ldblBenefitOptionFactor, 6));
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            ldecBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_50_PERCENT_POPUP_ANNUITY), ldecBenefitOptionFactor, ldecBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                     busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Round(ldecBenefitAmount * 0.50m, 2)));


            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_50_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);

            // Resetting the common variables
            ldecBenefitOptionFactor = 1;
            ldblBenefitOptionFactor = 1;
            ldecBenefitAmount = busConstant.ZERO_DECIMAL;

            // Joint And 100% Survivor Pop-up Annuity Benefit Option
            ldblBenefitOptionFactor = 0.71 + 0.01 * (lintSurvivorAge - lintParticipantAge) + 0.008 * (65 - lintParticipantAge);
            ldecBenefitOptionFactor = Convert.ToDecimal(ldblBenefitOptionFactor);
            lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            ldecBenefitAmount = Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2));
            lbusBenefitCalculationOptions.LoadData(ibusCalculation.GetPlanBenefitId(busConstant.MPIPP_PLAN_ID, busConstant.JOINT_100_PERCENT_POPUP_ANNUITY), ldecBenefitOptionFactor, ldecBenefitAmount,
                                                    busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER, this.ibusBenefitApplication.ibusPerson.icdoPerson.person_id, this.icdoBenefitCalculationHeader.beneficiary_person_id,
                                                    busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, Convert.ToDecimal(Math.Round(adecFinalAccruedBenefitAmount * ldecBenefitOptionFactor, 2)));


            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value = ibusCalculation.GetRelativeValue(busConstant.JOINT_100_PERCENT_POPUP_ANNUITY, this.icdoBenefitCalculationHeader.age, this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
            this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);
        }

        private decimal GetMPIPHPLumpSum()
        {
            //#region DONE specially for Calculating Accured Benefit -- We need this code here
            //if (!this.ibusBenefitApplication.iclbWorkData4RetirementYearMPIPP.IsNullOrEmpty())
            //{
            //    foreach (cdoDummyWorkData item in this.ibusBenefitApplication.iclbWorkData4RetirementYearMPIPP)
            //    {
            //        this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Add(item);
            //    }
            //}
            //#endregion
            decimal ldecERF = busConstant.ZERO_DECIMAL;
            decimal ldecFinalAccruedBenefitAmount = busConstant.ZERO_DECIMAL;
            decimal ldecUnreducedAccruedBenefitAmount = busConstant.ZERO_DECIMAL;
            decimal ldecLateAdjustmentAmt = busConstant.ZERO_DECIMAL;
            int lintMPIQualifiedYear = busConstant.ZERO_INT;

            string lstrRateType = string.Empty;
            lstrRateType = ibusCalculation.DeterminePlanRate(this.ibusBenefitApplication.ibusPerson, this.ibusBenefitApplication.aclbPersonWorkHistory_MPI, icdoBenefitCalculationHeader.retirement_date);

            if (iclbcdoPlanBenefitRate == null)
                iclbcdoPlanBenefitRate = new Collection<cdoPlanBenefitRate>();

            DataTable ldtbPlanBenefitRate = busBase.Select("cdoPlanBenefitRate.Lookup", new object[] { });
            iclbcdoPlanBenefitRate = cdoDummyWorkData.GetCollection<cdoPlanBenefitRate>(ldtbPlanBenefitRate);
            //PIR 355
            iclbcdoPlanBenefitRate = ibusCalculation.BenefitRateScheduleSpecialCase(this.icdoBenefitCalculationHeader.retirement_date, iclbcdoPlanBenefitRate);

            string lstrRetirementSubType = this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType;
            this.ibusBenefitApplication.aclbPersonWorkHistory_MPI = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderByDescending(item => item.year).ToList().ToCollection();
            ibusCalculation.GetRateForBenefitCalculation(this.ibusPerson, this.ibusPerson.icdoPerson.istrSSNNonEncrypted, lstrRateType, this.icdoBenefitCalculationHeader.retirement_date,
                                                         this.iclbcdoPlanBenefitRate, this.ibusBenefitApplication.aclbPersonWorkHistory_MPI, lstrRetirementSubType,this.icdoBenefitCalculationHeader.benefit_type_value,
                                                         this.ibusBenefitApplication.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES? true :false);//10 Percent RMD


            if (lstrRetirementSubType != busConstant.RETIREMENT_TYPE_LATE &&
                lstrRetirementSubType != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
            {
                foreach (cdoDummyWorkData lcdoDummyWorkData in this.ibusBenefitApplication.aclbPersonWorkHistory_MPI)
                {
                    if (lcdoDummyWorkData.vested_hours >= 400)
                    {
                        lintMPIQualifiedYear += 1;

                    }
                    else if (lcdoDummyWorkData.year == 2023 && this.icdoBenefitCalculationHeader.retirement_date.Year >= 2023  && (lcdoDummyWorkData.qualified_hours >= 65 && lcdoDummyWorkData.qualified_hours < 400))
                    {
                        lintMPIQualifiedYear += 1;

                    }


                    if (lcdoDummyWorkData.bis_years_count > lcdoDummyWorkData.vested_years_count)
                    {
                        // Participant has forfeited his Benefit.
                        // Reset the Total Benefit Amount to Zero.
                        lcdoDummyWorkData.idecBenefitAmount = busConstant.ZERO_DECIMAL;
                        ldecUnreducedAccruedBenefitAmount = busConstant.ZERO_DECIMAL;
                        ldecFinalAccruedBenefitAmount = busConstant.ZERO_DECIMAL;
                    }
                    // “Credited Pension QY in 2023 per MOA 2024” --Ticket#153518
                    else if (lcdoDummyWorkData.year == 2023 && this.icdoBenefitCalculationHeader.retirement_date.Year >= 2023 && lintMPIQualifiedYear < 20 && (lcdoDummyWorkData.qualified_hours >= 65 && lcdoDummyWorkData.qualified_hours < 400))
                    {
                        lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);

                    }
                    else if (lcdoDummyWorkData.qualified_years_count < Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_20) &&
                        lcdoDummyWorkData.qualified_hours < busConstant.MIN_HOURS_FOR_VESTED_YEAR)
                    {
                        lcdoDummyWorkData.idecBenefitAmount = busConstant.ZERO_DECIMAL;
                    }
                    else
                    {
                        if (lcdoDummyWorkData.qualified_years_count > Convert.ToInt32(busConstant.BenefitCalculation.QUALIFIED_YEARS_20) &&
                            lcdoDummyWorkData.qualified_hours < busConstant.MIN_HOURS_FOR_VESTED_YEAR)
                        {
                            if (lintMPIQualifiedYear > 20)
                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                            else
                            {
                                #region To Check If Local Merged - If yes give Benefit Else NO
                                if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains("Local")).Count() > 0)
                                {
                                    if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains(busConstant.Local_52)).Count() > 0)
                                    {
                                        if (lcdoDummyWorkData.year >= this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode == busConstant.Local_52).FirstOrDefault().icdoPersonAccount.idtMergerDate.Year)
                                            if (lcdoDummyWorkData.qualified_years_count > 20)
                                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                                    }

                                    if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains(busConstant.Local_161)).Count() > 0)
                                    {
                                        if (lcdoDummyWorkData.year >= this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode == busConstant.Local_161).FirstOrDefault().icdoPersonAccount.idtMergerDate.Year)
                                            if (lcdoDummyWorkData.qualified_years_count > 20)
                                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                                    }

                                    if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains(busConstant.Local_600)).Count() > 0)
                                    {
                                        if (lcdoDummyWorkData.year >= this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode == busConstant.Local_600).FirstOrDefault().icdoPersonAccount.idtMergerDate.Year)
                                            if (lcdoDummyWorkData.qualified_years_count > 20)
                                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                                    }

                                    if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains(busConstant.Local_666)).Count() > 0)
                                    {
                                        if (lcdoDummyWorkData.year >= this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode == busConstant.Local_666).FirstOrDefault().icdoPersonAccount.idtMergerDate.Year)
                                            if (lcdoDummyWorkData.qualified_years_count > 20)
                                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                                    }

                                    if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains(busConstant.LOCAL_700)).Count() > 0)
                                    {
                                        if (lcdoDummyWorkData.year >= this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).FirstOrDefault().icdoPersonAccount.idtMergerDate.Year)
                                            if (lcdoDummyWorkData.qualified_years_count > 20)
                                                lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                                    }

                                }
                                else
                                    lcdoDummyWorkData.idecBenefitAmount = busConstant.ZERO_DECIMAL;
                                #endregion
                            }
                        }
                        else
                        {
                            lcdoDummyWorkData.idecBenefitAmount = Math.Round(lcdoDummyWorkData.qualified_hours * lcdoDummyWorkData.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    //}

                    ldecUnreducedAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount + lcdoDummyWorkData.idecBenefitAmount;
                }
            }

            switch (lstrRetirementSubType)
            {
                case busConstant.RETIREMENT_TYPE_REDUCED_EARLY:
                case busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY:
                    ldecERF = ibusCalculation.GetEarlyReductionFactor(this.icdoBenefitCalculationHeader.iintPlanId, this.icdoBenefitCalculationHeader.benefit_type_value,
                                                                    lstrRetirementSubType, Convert.ToInt32(this.icdoBenefitCalculationHeader.age));
                    ldecFinalAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount * ldecERF;
                    break;

                case busConstant.RETIREMENT_TYPE_NORMAL:
                case busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY:
                    ldecFinalAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount;
                    break;

                case busConstant.RETIREMENT_TYPE_LATE:
                case busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION:
                    this.ibusBenefitApplication.aclbPersonWorkHistory_MPI = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(item => item.year).ToList().ToCollection();
                    ldecUnreducedAccruedBenefitAmount = ibusCalculation.CalculateLateRetirementAccruedBenefitAmount(
                                            this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First(),
                        //icdoBenefitCalculationHeader.iintPlanId,      // We cannot pass the Local Plan Id. We need to pass the MPI Plan Id
                                            busConstant.MPIPP_PLAN_ID,
                                            busConstant.BENEFIT_TYPE_RETIREMENT, ibusPerson, ibusBenefitApplication.aclbPersonWorkHistory_MPI,
                                            busConstant.BOOL_FALSE, iclbBenefitCalculationDetail, null, ref ldecLateAdjustmentAmt, this.icdoBenefitCalculationHeader.retirement_date, busConstant.BOOL_FALSE,
                                            this.ibusBenefitApplication.icdoBenefitApplication.adtMPIVestingDate, iclbPersonAccountRetirementContribution);
                    ldecFinalAccruedBenefitAmount = ldecUnreducedAccruedBenefitAmount;
                    break;

                default:
                    break;
            }
            //Ticket - 61531
            decimal ldecLumpSumBenefitOptionFactor = Math.Round(this.GetLumpsumBenefitFactor(Convert.ToInt32(this.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement), this.icdoBenefitCalculationHeader.retirement_date.Year),3);
            decimal ldecimalMPIPHPLumpSum = Convert.ToDecimal(Math.Round(ldecFinalAccruedBenefitAmount * ldecLumpSumBenefitOptionFactor, 2));

            //#region DONE specially for Calculating Accured Benefit -- We need this code here
            //if (!this.ibusBenefitApplication.iclbWorkData4RetirementYearMPIPP.IsNullOrEmpty())
            //{
            //    foreach (cdoDummyWorkData item in this.ibusBenefitApplication.iclbWorkData4RetirementYearMPIPP)
            //    {
            //        if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Contains(item))
            //        {
            //            this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Remove(item);
            //        }
            //    }
            //}
            //#endregion
            this.ibusBenefitApplication.aclbPersonWorkHistory_MPI = this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(item => item.year).ToList().ToCollection();
            return ldecimalMPIPHPLumpSum;
        }

        public override void AfterPersistChanges()
        {
            decimal ldecLocal700GauranteedAmt = 0;
            busActiveRetireeIncreaseContract lbusActiveRetireeIncreaseContract = new busActiveRetireeIncreaseContract();
            Collection<busActiveRetireeIncreaseContract> lclbActiveRetireeIncreaseContract = new Collection<busActiveRetireeIncreaseContract>();

            foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in iclbBenefitCalculationDetail)
            {
                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES &&
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                    continue;
                if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
                {
                    foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in lbusBenefitCalculationDetail.iclbBenefitCalculationOptions)
                    {
                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_detail_id = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_option_id == 0)
                        {
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.Insert();
                        }
                        //lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.Update();

                        #region Calculate Retiree Increase

                        //PROD PIR 127
                        if ((icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT
                            || icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL) &&
                            icdoBenefitCalculationHeader.retirement_date < DateTime.Now
                            && icdoBenefitCalculationHeader.iintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            //For Death Retiree Inc Should get created from the benefit begin date.
                            DateTime ldteBenefitBeginDate = new DateTime();
                            if (this.icdoBenefitCalculationHeader.date_of_death.Day == 1)
                            {
                                ldteBenefitBeginDate = this.icdoBenefitCalculationHeader.date_of_death.AddMonths(1);
                            }
                            else
                            {
                                ldteBenefitBeginDate = this.icdoBenefitCalculationHeader.date_of_death.GetLastDayofMonth().AddDays(1);
                            }
                            lclbActiveRetireeIncreaseContract = lbusActiveRetireeIncreaseContract.LoadActiveRetireeIncContractByRetirementDate(this.icdoBenefitCalculationHeader.retirement_date);

                            foreach (busActiveRetireeIncreaseContract lbusRetireeIncreaseContract in lclbActiveRetireeIncreaseContract)
                            {
                                DataTable ldtbLoadApprovedQDRO = Select("cdoDroApplication.LoadApprovedQDRO", new object[2] { this.icdoBenefitCalculationHeader.person_id, icdoBenefitCalculationHeader.iintPlanId });
                                DateTime ldtDROCommencementDate = new DateTime();
                                string lstrIsAlternatePayeeEntitledToPartBenefit = string.Empty;

                                if (ldtbLoadApprovedQDRO.Rows.Count > 0)
                                {
                                    ldtDROCommencementDate = Convert.ToDateTime(ldtbLoadApprovedQDRO.Rows[0][enmDroApplication.dro_commencement_date.ToString()]);
                                    lstrIsAlternatePayeeEntitledToPartBenefit = Convert.ToString(ldtbLoadApprovedQDRO.Rows[0][enmDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase.ToString()]);
                                }
                                DateTime ldtRetireeIncreaseDate = new DateTime(lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.plan_year, 11, 01);

                                if (ldtRetireeIncreaseDate >= ldteBenefitBeginDate)
                                {

                                    if (ldtRetireeIncreaseDate >= icdoBenefitCalculationHeader.retirement_date && (ldtDROCommencementDate == DateTime.MinValue ||
                                        (ldtDROCommencementDate != DateTime.MinValue && ldtRetireeIncreaseDate < ldtDROCommencementDate) ||
                                        (ldtDROCommencementDate != DateTime.MinValue && ldtRetireeIncreaseDate > ldtDROCommencementDate && lstrIsAlternatePayeeEntitledToPartBenefit != busConstant.FLAG_YES)))
                                    {
                                        if (icdoBenefitCalculationHeader.iintPlanId == busConstant.LOCAL_700_PLAN_ID)
                                        {
                                            ldecLocal700GauranteedAmt =
                                                ibusCalculation.GetLocal700GuarentedAmt(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id);
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

                                        ibusCalculation.CalculateAndCreateRetireeIncreasePayeeAccount(lbusBenefitCalculationOptions, null, lbusRetireeIncreaseContract, icdoBenefitCalculationHeader.retirement_date.Year,
                                            icdoBenefitCalculationHeader.benefit_calculation_header_id, 0, ldecLocal700GauranteedAmt,
                                            Convert.ToDecimal(lbusRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.percent_increase_value), busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT);
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                }
            }
            this.LoadDetailsGrid();
            base.AfterPersistChanges();

            if (this.ibusBaseActivityInstance.IsNotNull())
                this.SetProcessInstanceParameters();

        }

        public ArrayList btn_ApproveCalculation()
        {
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            this.iarrErrors = base.btn_ApproveCalculation();

            if (!this.iarrErrors.IsNullOrEmpty())
                return this.iarrErrors;

            if (this.iarrErrors.Count == 0 && this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                #region PAYEE ACCOUNT RELATED LOGIC (PAYMENT - SPRINT 3.0)
                int flag = 0;
                if (flag != 1)  // DONE ON PURPOSE TO AVOID PAYEE ACCOUNT CODE TO BE EXECUTED FOR NOW
                {
                    //if (!this.iclbBenefitCalculationDetail.IsNullOrEmpty())
                    //{
                    //    if (!this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                    //    {
                    //        busBenefitCalculationOptions lbusBenefitCalculationOptions = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault();
                    //        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.overridden_benefit_amount > decimal.Zero)
                    //        {

                    //            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount = Math.Round(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.overridden_benefit_amount, 2);


                    //            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.Update();
                    //        }
                    //    }
                    //}

                    foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                    {


                        if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty() && lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount <= Decimal.Zero)
                        {
                            utlError lobjError = new utlError();
                            lobjError = AddError(6084, "");//R3view 
                            this.iarrErrors.Add(lobjError);
                        }
                        else if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount > Decimal.Zero || lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount > Decimal.Zero)
                        {
                            int lintBenefitAccountID = 0;
                            int lintPayeeAccountID = 0;
                            string lstrFundsType = String.Empty;


                            //Benefit Account Related
                            decimal ldecAccountOwnerStartingTaxableAmount = 0.0M;
                            decimal ldecAccountOwnerStartingNonTaxableAmount = 0.0M;
                            decimal ldecAccountOwnerStartingGrossAmount = 0.0M;

                            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            busPayeeBenefitAccount lbusPayeeBenefitAccount = new busPayeeBenefitAccount { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };

                            busPayeeAccount lbusPayeeAccountParticipant = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            lbusPayeeAccountParticipant.FindPayeeAccount(this.icdoBenefitCalculationHeader.payee_account_id);
                            lbusPayeeAccountParticipant.LoadPaymentHistoryHeaderDetails();
                            lbusPayeeAccountParticipant.LoadNextBenefitPaymentDate();

                            //Need To check
                            DateTime ldtBenefitBeginDate = new DateTime();
                            if (this.icdoBenefitCalculationHeader.date_of_death.Day == 1)
                            {
                                ldtBenefitBeginDate = this.icdoBenefitCalculationHeader.date_of_death.AddMonths(1);
                            }
                            else
                            {
                                ldtBenefitBeginDate = this.icdoBenefitCalculationHeader.date_of_death.GetLastDayofMonth().AddDays(1);
                            }

                            //PIR 853
                            if (this.icdoBenefitCalculationHeader.retirement_date > ldtBenefitBeginDate)
                            {
                                ldtBenefitBeginDate = this.icdoBenefitCalculationHeader.retirement_date;
                            }

                            busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                            lbusPlanBenefitXr.FindPlanBenefitXr(lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id);
                            lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue = lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;

                            switch (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id)
                            {
                                case busConstant.MPIPP_PLAN_ID:
                                    DataTable ldtblPayeeBenefitAccount = Select("cdoPayeeBenefitAccount.GetPayeeBenefitAccount", new object[1] { lbusPayeeAccountParticipant.icdoPayeeAccount.payee_benefit_account_id });
                                    if (ldtblPayeeBenefitAccount.Rows.Count > 0)
                                    {
                                        if (Convert.ToString(ldtblPayeeBenefitAccount.Rows[0][enmPayeeBenefitAccount.starting_taxable_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            ldecAccountOwnerStartingTaxableAmount = Convert.ToDecimal(ldtblPayeeBenefitAccount.Rows[0][enmPayeeBenefitAccount.starting_taxable_amount.ToString().ToUpper()]);

                                        if (Convert.ToString(ldtblPayeeBenefitAccount.Rows[0][enmPayeeBenefitAccount.starting_nontaxable_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            ldecAccountOwnerStartingNonTaxableAmount = Convert.ToDecimal(ldtblPayeeBenefitAccount.Rows[0][enmPayeeBenefitAccount.starting_nontaxable_amount.ToString().ToUpper()]);

                                        ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount + ldecAccountOwnerStartingNonTaxableAmount;
                                        lstrFundsType = Convert.ToString(ldtblPayeeBenefitAccount.Rows[0][enmPayeeBenefitAccount.funds_type_value.ToString().ToUpper()]);
                                    }
                                    break;
                                case busConstant.IAP_PLAN_ID:
                                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                                    {
                                        ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_balance_amount;
                                        ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                    }
                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                    {

                                        ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount;
                                        ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                        lstrFundsType = busConstant.FundTypeLocal52SpecialAccount;
                                    }
                                    else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                    {
                                        ldecAccountOwnerStartingTaxableAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount;
                                        ldecAccountOwnerStartingGrossAmount = ldecAccountOwnerStartingTaxableAmount;
                                        lstrFundsType = busConstant.FundTypeLocal161SpecialAccount;
                                    }
                                    break;

                            }

                            //Benefit Account
                            lintBenefitAccountID = busPayeeAccountHelper.IsBenefitAccountExists(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id,
                                                                                                 busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT, lstrFundsType,
                                                                                                 this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id, 0);  //R3view  the Query and code for this one.

                            lintBenefitAccountID = lbusPayeeBenefitAccount.ManagePayeeBenefitAccount(lintBenefitAccountID, this.icdoBenefitCalculationHeader.person_id,
                                                                              lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id,
                                                                              ldecAccountOwnerStartingTaxableAmount, ldecAccountOwnerStartingNonTaxableAmount, ldecAccountOwnerStartingGrossAmount, lstrFundsType);



                            //Payee Account //R3view this code
                            if (this.icdoBenefitCalculationHeader.beneficiary_person_id > 0)
                                lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(this.icdoBenefitCalculationHeader.beneficiary_person_id, lintBenefitAccountID, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT, false, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id,
                                        null, 0, null, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id);

                            else if (this.icdoBenefitCalculationHeader.organization_id > 0)
                                lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(this.icdoBenefitCalculationHeader.organization_id, lintBenefitAccountID, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT, true, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id,
                                        null, 0, null, this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.benefit_application_detail_id);

                            decimal ldecNonTaxableBeginningBalance = 0.0M;
                            decimal ldecBenPercentage = 0M;
                            decimal ldecMinimumGuaranteeAmount = 0M;

                            DataTable ldtblBenPercentage = Select("cdoPersonAccountBeneficiary.GetPercentageOfBen", new object[3] { lbusPayeeAccountParticipant.icdoPayeeAccount.person_id,
                            this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id , this.icdoBenefitCalculationHeader.beneficiary_person_id});

                            if (ldtblBenPercentage != null && ldtblBenPercentage.Rows.Count > 0 && Convert.ToString(ldtblBenPercentage.Rows[0][0]).IsNotNullOrEmpty())
                            {
                                ldecBenPercentage = Convert.ToDecimal(ldtblBenPercentage.Rows[0][0]);
                            }

                            if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                lbusPayeeAccountParticipant.idecRemainingNonTaxableBeginningBalance > 0)
                            {
                                if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.LUMP_SUM || lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY ||
                                    lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY || lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY ||
                                    lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY || lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.LIFE_ANNUTIY) //RASHMI - added last condition for PIR-789
                                {
                                    ldecNonTaxableBeginningBalance = (lbusPayeeAccountParticipant.idecRemainingNonTaxableBeginningBalance / 100) * ldecBenPercentage;
                                }
                                else
                                {
                                    ldecNonTaxableBeginningBalance = ibusCalculation.GetSurvivorAmountFromBenefitAmount(lbusPayeeAccountParticipant.idecRemainingNonTaxableBeginningBalance, lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue);
                                }
                            }

                            string lstrAccountRelationShip = busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY;
                            if (this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                               lbusPayeeAccountParticipant.idecRemainingMinimumGuaranteeAmount > 0)
                            {
                                if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.LUMP_SUM || lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY ||
                                    lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY || lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY ||
                                    lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY || lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.LIFE_ANNUTIY) //RASHMI - added last condition for PIR-789
                                {
                                    ldecMinimumGuaranteeAmount = (lbusPayeeAccountParticipant.idecRemainingMinimumGuaranteeAmount / 100) * ldecBenPercentage;
                                }
                                else
                                {
                                    lstrAccountRelationShip = busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_JOINT_ANNUITANT;
                                    ldecMinimumGuaranteeAmount = ibusCalculation.GetSurvivorAmountFromBenefitAmount(lbusPayeeAccountParticipant.idecRemainingMinimumGuaranteeAmount, lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue);
                                }
                            }

                            DateTime ldteTermCertainEndDate = new DateTime();
                            string lstrFamilyRelationshipValue = string.Empty;
                            lstrFamilyRelationshipValue = this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_relationship_value;


                            //lbusPayeeAccountParticipant.icdoPayeeAccount.term_certain_end_date;

                            //R3view -- IF Term Year Certain Option FIND the end Date 
                            LoadPlanBenefitsForPlan(this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.plan_id);
                            iintTermCertainMonths = busConstant.ZERO_INT;
                            iintTermCertainMonths = busPayeeAccountHelper.IsTermCertainBenefitOption(this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id, this.iclbcdoPlanBenefit);
                            if (iintTermCertainMonths > 0)
                            {
                                //ldteTermCertainEndDate = ldteBenefitBeginDate.AddMonths(iintTermCertainMonths);
                                ldteTermCertainEndDate = lbusPayeeAccountParticipant.icdoPayeeAccount.term_certain_end_date;
                                //if (ldteTermCertainEndDate != DateTime.MinValue)
                                //    ldteTermCertainEndDate = ldteTermCertainEndDate.AddDays(-1);
                            }

                            bool lblnAdjustmentPaymentFlag = false;
                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjustment_iap_payment_flag == busConstant.FLAG_YES ||
                                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjustment_l52spl_payment_flag == busConstant.FLAG_YES ||
                                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.adjustment_l161spl_payment_flag == busConstant.FLAG_YES)
                            {
                                lblnAdjustmentPaymentFlag = true;
                            }


                            //R3view -- NonTaxable Beginning Balance   
                            if (this.icdoBenefitCalculationHeader.beneficiary_person_id > 0)
                                lintPayeeAccountID = lbusPayeeAccount.ManagePayeeAccount(lintPayeeAccountID, this.icdoBenefitCalculationHeader.beneficiary_person_id, 0,
                                                                                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id,
                                                                                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id,
                                                                                        0, 0, lintBenefitAccountID, busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT, lbusPayeeAccountParticipant.icdoPayeeAccount.retirement_type_value,
                                                                                        ldtBenefitBeginDate, DateTime.MinValue, lstrAccountRelationShip, lstrFamilyRelationshipValue,
                                                                                        ldecMinimumGuaranteeAmount, ldecNonTaxableBeginningBalance, lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id,
                                                                                        ldteTermCertainEndDate, busConstant.FLAG_NO, busConstant.FLAG_NO, lblnAdjustmentPaymentFlag);

                            else if (this.icdoBenefitCalculationHeader.organization_id > 0)
                                lintPayeeAccountID = lbusPayeeAccount.ManagePayeeAccount(lintPayeeAccountID, 0, this.icdoBenefitCalculationHeader.organization_id,
                                                                                         lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id,
                                                                                         lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id,
                                                                                         0, 0, lintBenefitAccountID, busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT, lbusPayeeAccountParticipant.icdoPayeeAccount.retirement_type_value,
                                                                                         ldtBenefitBeginDate, DateTime.MinValue, busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY, lstrFamilyRelationshipValue,
                                                                                         ldecMinimumGuaranteeAmount, ldecNonTaxableBeginningBalance, this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.plan_benefit_id,
                                                                                         ldteTermCertainEndDate, busConstant.FLAG_NO, busConstant.FLAG_NO, lblnAdjustmentPaymentFlag);

                            lbusPayeeAccount.LoadNextBenefitPaymentDate();
                            DateTime ldteNextBenefitPaymentDate = lbusPayeeAccount.idtNextBenefitPaymentDate;//R3vview this once with Vinovin

                            decimal ldecTaxableAmount = 0M;
                            decimal ldecNonTaxableAmount = 0M;

                            decimal ldecUpdatedSurvivorAmount = 0M;

                            if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount > 0)
                            {
                                ldecUpdatedSurvivorAmount = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount;
                            }
                            else
                            {
                                ldecUpdatedSurvivorAmount = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount;
                            }

                            //R3view -- First Parameter Should be maybe Retirement or Payment Date Review the Function too 

                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag == busConstant.FLAG_YES || lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                            {

                                busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, ldecUpdatedSurvivorAmount,
                                                      ref ldecNonTaxableAmount, ref ldecTaxableAmount, ldecNonTaxableBeginningBalance);
                            }
                            else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                            {

                                //busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.final_monthly_benefit_amount,
                                //                                           ref ldecNonTaxableAmount, ref ldecTaxableAmount, 0);

                                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES
                                    && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                                {
                                    if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount > 0)
                                    {
                                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_balance_amount = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.overridden_benefit_amount;

                                    }

                                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue,
                                        ((lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_balance_amount - lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset) / 100) * ldecBenPercentage,//PIR 985 10262015
                                                                             ref ldecNonTaxableAmount, ref ldecTaxableAmount, 0);
                                }
                                else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                {
                                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue,
                                        ((lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount - lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_qdro_offset) / 100) * ldecBenPercentage,//PIR 985 10262015
                                                                            ref ldecNonTaxableAmount, ref ldecTaxableAmount, 0);
                                }
                                else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                {
                                    busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue,
                                        ((lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount - lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_qdro_offset) / 100) * ldecBenPercentage,//PIR 985 10262015
                                                                            ref ldecNonTaxableAmount, ref ldecTaxableAmount, 0);
                                }
                            }
                            //PROD PIR 631
                            else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID &&
                                lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription == busConstant.LUMP_SUM_DESCRIPTION)
                            {

                                if (lbusPayeeAccountParticipant.idecRemainingMinimumGuaranteeAmount > 0 || lbusPayeeAccountParticipant.idecRemainingNonTaxableBeginningBalance > 0)
                                {
                                    ldecTaxableAmount = ldecUpdatedSurvivorAmount - //lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount -
                                        ((lbusPayeeAccountParticipant.idecRemainingNonTaxableBeginningBalance / 100) * ldecBenPercentage);

                                    if (ldecTaxableAmount > 0)
                                        ldecNonTaxableAmount = (lbusPayeeAccountParticipant.idecRemainingNonTaxableBeginningBalance / 100) * ldecBenPercentage;
                                    else
                                    {
                                        ldecTaxableAmount = ldecUpdatedSurvivorAmount; // lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount;
                                        ldecNonTaxableAmount = 0M;
                                    }
                                }
                                else
                                {
                                    ldecTaxableAmount += ldecUpdatedSurvivorAmount; //lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.survivor_amount;
                                }
                            }
                            else
                            {
                                busPayeeAccountHelper.CalculateMonthlyPaymentComponents(DateTime.MinValue, ldecUpdatedSurvivorAmount,
                                    ref ldecNonTaxableAmount, ref ldecTaxableAmount, lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.monthly_exclusion_amount.IsNull() ? 0 : this.iclbBenefitCalculationDetail.First().icdoBenefitCalculationDetail.monthly_exclusion_amount);
                            }


                            if (ldecTaxableAmount > 0M)
                            {
                                if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)
                                    || lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM21", ldecTaxableAmount, "0", 0,
                                                                ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                else
                                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM1", ldecTaxableAmount, "0", 0,
                                                                    ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                            }
                            if (ldecNonTaxableAmount > 0M)
                            {
                                if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION)
                                    || lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM22", ldecNonTaxableAmount, "0", 0,
                                                                ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                                else
                                    lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM2", ldecNonTaxableAmount, "0", 0,
                                                                    ldteNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                            }



                            //Create Payee Account in Review
                            if (this.icdoBenefitCalculationHeader.calculation_type_value != busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT
                            || (lbusPayeeAccount.iclbPayeeAccountStatus == null && this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT))
                            {
                                lbusPayeeAccount.CreateReviewPayeeAccountStatus();
                            }

                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                        this.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                            {
                                lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);
                            }

                            //Retro Calculation Items to be Created
                            if (this.icdoBenefitCalculationHeader.retirement_date < ldteNextBenefitPaymentDate && (!this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                                && lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID)
                            {
                                // PROD PIR 127
                                this.CreatePayeeAccountForRetireeIncrease(lbusPayeeAccount, lintBenefitAccountID, lstrFamilyRelationshipValue, ldecNonTaxableBeginningBalance, ldteNextBenefitPaymentDate, 0, busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT, lbusPayeeAccount.icdoPayeeAccount.account_relation_value);
                            }

                            //PIR 993  added check for Lumpsum benefit type for MPIPP plan.
                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id != busConstant.IAP_PLAN_ID &&
                                (!this.iclbBenefitCalculationDetail.First().iclbBenefitCalculationOptions.First().icdoBenefitCalculationOptions.istrBenefitOptionDescription.Contains(busConstant.LUMP_SUM_DESCRIPTION))
                                )
                            {
                                lbusPayeeAccount.CreateReviewPayeeAccountStatus(ablnFromApprovedCalc: true);// PIR 1055

                                decimal ldecParticipantMEA = 0;
                                decimal ldecParticipantsBenefitAmount = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;

                                if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.LUMP_SUM && lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY || lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY ||
                                  lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY || lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                                {
                                    ldecParticipantMEA = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.monthly_exclusion_amount;
                                }
                                else
                                {
                                    ldecParticipantMEA = ibusCalculation.GetBenefitAmountFromSurvivorAmount(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.monthly_exclusion_amount, lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue);
                                }

                                DateTime ldtRetroStartDate = ldtBenefitBeginDate;

                                bool lblnAdjustmentCalculationForRetireeIncrease = false;
                                if (iclbDisabilityRetireeIncrease != null && iclbDisabilityRetireeIncrease.Count > 0)
                                    lblnAdjustmentCalculationForRetireeIncrease = true;

                                Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetailParticipant = new Collection<busBenefitMonthwiseAdjustmentDetail>();

                                //PIR 853
                                lbusPayeeAccountParticipant.LoadPayeeAccountRetroPayments();
                                if (lbusPayeeAccountParticipant.iclbPayeeAccountRetroPayment != null && lbusPayeeAccountParticipant.iclbPayeeAccountRetroPayment.Count() > 0
                                    && lbusPayeeAccountParticipant.iclbPayeeAccountRetroPayment.Where(t => t.icdoPayeeAccountRetroPayment.is_overpayment_flag != busConstant.FLAG_YES && t.icdoPayeeAccountRetroPayment.start_date >= lbusPayeeAccount.idtNextBenefitPaymentDate).Count() > 0)
                                {
                                    busPayeeAccountRetroPayment lbusPayeeAccountRetroPayment = lbusPayeeAccountParticipant.iclbPayeeAccountRetroPayment.Where(t => t.icdoPayeeAccountRetroPayment.is_overpayment_flag != busConstant.FLAG_YES && t.icdoPayeeAccountRetroPayment.start_date >= lbusPayeeAccount.idtNextBenefitPaymentDate).FirstOrDefault();
                                    lbusPayeeAccountRetroPayment.LoadBenefitMonthwiseAdjustmentDetails();
                                    lclbBenefitMonthwiseAdjustmentDetailParticipant = lbusPayeeAccountRetroPayment.iclbBenefitMonthwiseAdjustmentDetail;
                                    if (lbusPayeeAccountRetroPayment.iclbBenefitMonthwiseAdjustmentDetail != null && lbusPayeeAccountRetroPayment.iclbBenefitMonthwiseAdjustmentDetail.Count > 0)
                                        ldtRetroStartDate = lbusPayeeAccountRetroPayment.iclbBenefitMonthwiseAdjustmentDetail.OrderBy(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.payment_date;
                                    else
                                        ldtRetroStartDate = lbusPayeeAccountParticipant.icdoPayeeAccount.benefit_begin_date;
                                }


                                Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();
                                lclbBenefitMonthwiseAdjustmentDetail = ibusCalculation.GetAmountActuallyPaid(lbusPayeeAccount, ldtRetroStartDate, lbusPayeeAccount.idtLastBenefitPaymentDate, ablnIsAdjustmentCalculation: lblnAdjustmentCalculationForRetireeIncrease);
                                ibusCalculation.CalculateAmountShouldHaveBeenPaid(lbusPayeeAccount, ref lclbBenefitMonthwiseAdjustmentDetail);

                                if (lclbBenefitMonthwiseAdjustmentDetailParticipant != null && lclbBenefitMonthwiseAdjustmentDetailParticipant.Count > 0)
                                {
                                    if (lclbBenefitMonthwiseAdjustmentDetail != null && lclbBenefitMonthwiseAdjustmentDetail.Count > 0
                                    && lclbBenefitMonthwiseAdjustmentDetail.Where(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date < ldtBenefitBeginDate).Count() > 0)
                                    {
                                        lclbBenefitMonthwiseAdjustmentDetail.Where(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date < ldtBenefitBeginDate)
                                            .ForEach(t => t.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = decimal.Zero);

                                        lclbBenefitMonthwiseAdjustmentDetail.Where(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date < ldtBenefitBeginDate)
                                            .ForEach(t => t.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = decimal.Zero);
                                    }
                                }

                                if (lclbBenefitMonthwiseAdjustmentDetailParticipant != null && lclbBenefitMonthwiseAdjustmentDetailParticipant.Count > 0)
                                {
                                    foreach (busBenefitMonthwiseAdjustmentDetail lbusBenefitMonthwiseAdjustmentDetail in lclbBenefitMonthwiseAdjustmentDetail)
                                    {
                                        if (lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                            lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).Count() > 0)
                                        {
                                            if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.LUMP_SUM && lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY || lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY ||
                                               lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY || lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                                            {

                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid = (
                                                    lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid / 100) * ldecBenPercentage;

                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid = (
                                                    lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid / 100) * ldecBenPercentage;


                                                if (lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                               lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.override_flag == busConstant.FLAG_YES)
                                                {
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = (
                                                      lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                  lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.overriden_taxable_amount / 100) * ldecBenPercentage;

                                                }
                                                else
                                                {
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid = (
                                                      lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                  lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid / 100) * ldecBenPercentage;
                                                }


                                                if (lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.override_flag == busConstant.FLAG_YES)
                                                {
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = (
                                                      lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                  lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.overriden_non_taxable_amount / 100) * ldecBenPercentage;

                                                }
                                                else
                                                {
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid = (
                                                    lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid / 100) * ldecBenPercentage;
                                                }

                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.amount_repaid = (
                                                  lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.amount_repaid / 100) * ldecBenPercentage;


                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.suspended_flag =
                                                  lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.suspended_flag;

                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.hours =
                                                  lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.hours;


                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_history_header_id =
                                                  lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.payment_history_header_id;
                                            }
                                            else
                                            {
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid =
                                                   lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                               lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_paid;

                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid =
                                                    lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_paid;

                                                if (lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                               lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.override_flag == busConstant.FLAG_YES)
                                                {
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid =
                                                      lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                  lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.overriden_taxable_amount;

                                                }
                                                else
                                                {
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid =
                                                      lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                  lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.taxable_amount_to_be_paid;
                                                }


                                                if (lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.override_flag == busConstant.FLAG_YES)
                                                {
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid =
                                                      lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                  lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.overriden_non_taxable_amount;

                                                }
                                                else
                                                {
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid =
                                                    lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                        lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.non_taxable_amount_to_be_paid;
                                                }

                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.amount_repaid =
                                                  lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.amount_repaid;

                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.suspended_flag =
                                                  lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.suspended_flag;

                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.hours =
                                                  lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.hours;

                                                lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_history_header_id =
                                                  lclbBenefitMonthwiseAdjustmentDetailParticipant.Where(item => item.icdoBenefitMonthwiseAdjustmentDetail.payment_date ==
                                                    lbusBenefitMonthwiseAdjustmentDetail.icdoBenefitMonthwiseAdjustmentDetail.payment_date).FirstOrDefault().icdoBenefitMonthwiseAdjustmentDetail.payment_history_header_id;

                                            }
                                        }

                                    }
                                }

                                ibusCalculation.CalculateRetireeIncreaseAmountShouldHaveBeenPaid(lbusPayeeAccount, iclbDisabilityRetireeIncrease, ref lclbBenefitMonthwiseAdjustmentDetail);// PROD PIR 581
                                lclbBenefitMonthwiseAdjustmentDetail = lclbBenefitMonthwiseAdjustmentDetail.OrderBy(t => t.icdoBenefitMonthwiseAdjustmentDetail.payment_date).ToList().ToCollection();//PIR 853
                                ibusCalculation.CreateOverpaymentUnderPayment(lbusPayeeAccount, lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH, ablnPostDeath: true);

                                if (this.ibusBaseActivityInstance.IsNotNull())
                                {
                                    PAYEE_ACCOUNT_ID = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                                    SetProcessInstanceParameters();
                                }
                            }
                        }
                    }
                }

                #endregion
            }
            return this.iarrErrors;
        }


        public Collection<cdoPlan> LoadSpecialAccounts()
        {
            Collection<cdoPlan> lclbSubPlans = new Collection<cdoPlan>();
            this.idecL52Balance = decimal.Zero;
            this.idecL161Balance = decimal.Zero;
           int lintpersonAccountId = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoBenefitCalculationHeader.iintPlanId).FirstOrDefault().icdoPersonAccount.person_account_id;
            DataTable ldtbIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                   new object[2] { lintpersonAccountId,
                                       DateTime.Now.Year});
            if (ldtbIAPBalance.IsNotNull() && ldtbIAPBalance.Rows.Count > 0)
            {
                this.idecL52Balance = Convert.ToDecimal(Convert.ToBoolean(ldtbIAPBalance.Rows[0][1].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbIAPBalance.Rows[0][1]);
                this.idecL161Balance = Convert.ToDecimal(Convert.ToBoolean(ldtbIAPBalance.Rows[0][2].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbIAPBalance.Rows[0][2]);

            }
            if (this.idecL52Balance > 0)
            {
                cdoPlan lcdoPlan = new cdoPlan();
                lcdoPlan.plan_code = busConstant.L52_SPL_ACC;
                lcdoPlan.plan_name = "LOCAL-52 SPECIAL ACCOUNT";
                lclbSubPlans.Add(lcdoPlan);
            }
            if (this.idecL161Balance > 0)
            {
                cdoPlan lcdoPlan1 = new cdoPlan();
                lcdoPlan1.plan_code = busConstant.L161_SPL_ACC;
                lcdoPlan1.plan_name = "LOCAL-161 SPECIAL ACCOUNT";
                lclbSubPlans.Add(lcdoPlan1);
            }
            //DataTable ldtblLocal52SpAccountPayeeAccount = Select("cdoBenefitCalculationHeader.GetL52SpAccntPayeeAccount", new object[1] { this.icdoBenefitCalculationHeader.person_id });
            //if (ldtblLocal52SpAccountPayeeAccount.Rows.Count > 0)
            //{
            //    cdoPlan lcdoPlan = new cdoPlan();
            //    lcdoPlan.plan_code = busConstant.L52_SPL_ACC;
            //    lcdoPlan.plan_name = "LOCAL-52 SPECIAL ACCOUNT";
            //    lclbSubPlans.Add(lcdoPlan);
            //}
            //else
            //{
            //    //Need to Check Eligibility
            //    if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID && item.icdoPersonAccount.special_account == busConstant.FLAG_YES).Count() > 0)
            //    {
            //        //DataTable ldtblCount = busBase.Select("cdoPersonAccount.CheckPersonHasSpecialAccount", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoPersonAccount.person_account_id });
            //        //if (ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0)
            //        //{
            //            cdoPlan lcdoPlan = new cdoPlan();
            //            lcdoPlan.plan_code = busConstant.L52_SPL_ACC;
            //            lcdoPlan.plan_name = "LOCAL-52 SPECIAL ACCOUNT";
            //            lclbSubPlans.Add(lcdoPlan);
            //        //}
            //    }
            //}

            //DataTable ldtblLocal161SpAccountPayeeAccount = Select("cdoBenefitCalculationHeader.GetL161SpAccntPayeeAccount", new object[1] { this.icdoBenefitCalculationHeader.person_id });
            //if (ldtblLocal161SpAccountPayeeAccount.Rows.Count > 0)
            //{
            //    cdoPlan lcdoPlan1 = new cdoPlan();
            //    lcdoPlan1.plan_code = busConstant.L161_SPL_ACC;
            //    lcdoPlan1.plan_name = "LOCAL-161 SPECIAL ACCOUNT";
            //    lclbSubPlans.Add(lcdoPlan1);
            //}
            //else
            //{
            //    //Need to Check Eligibility
            //    if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID && item.icdoPersonAccount.special_account == busConstant.FLAG_YES).Count() > 0)
            //    {
            //        //DataTable ldtblCount = busBase.Select("cdoPersonAccount.CheckPersonHasSpecialAccount", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoPersonAccount.person_account_id });
            //        //if (ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0)
            //        //{
            //            cdoPlan lcdoPlan1 = new cdoPlan();
            //            lcdoPlan1.plan_code = busConstant.L161_SPL_ACC;
            //            lcdoPlan1.plan_name = "LOCAL-161 SPECIAL ACCOUNT";
            //            lclbSubPlans.Add(lcdoPlan1);
            //        //}
            //    }
            //}
            return lclbSubPlans;
        }

        public bool checkForSpecialAccounts()
        {

            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
            {
                return true;
            }
            
            return false;

        }

        public bool checkForL52SpecialAccountBal()
        {

            if (this.iclbBenefitCalculationDetail != null)
            {
                foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                {
                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount > 0)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        public bool checkForL161SpecialAccountBal()
        {

            if (this.iclbBenefitCalculationDetail != null)
            {
                foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                {
                    if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount > 0)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        public void LoadDetailsGrid()
        {
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
            {
                foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                {

                    if (!lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
                    {
                        busBenefitCalculationOptions lbusBenefitCalculationOptions = lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.First();
                        if (lbusBenefitCalculationOptions.ibusPlanBenefitXr == null)
                            lbusBenefitCalculationOptions.LoadPlanBenefitXr();
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrBenefitOptionValueDescrioption = lbusBenefitCalculationOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_description;

                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrBenefitOptionValue = lbusBenefitCalculationOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;

                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)
                        {
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccount = busConstant.L161_SPL_ACC;
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccountDescrioption = "IAP (Local-161 Special Account)";
                            //lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;

                        }
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES)
                        {
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccount = busConstant.L52_SPL_ACC;
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccountDescrioption = "IAP (Local-52 Special Account)";
                            //lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;
                        }
                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.local161_special_acct_bal_flag != busConstant.FLAG_YES &&
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.local52_special_acct_bal_flag != busConstant.FLAG_YES)
                        {
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccountDescrioption = "IAP";
                            //lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.idecSurvivorAmount = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;
                        }
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.idecSurvivorAmount = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSurvivorRelationshipDescription = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_relationship_description;
                    }
                }
            }
        }

        public Collection<cdoBenefitCalculationDetail> LoadSpecialAccount()
        {
            Collection<cdoBenefitCalculationDetail> lcolDetail = new Collection<cdoBenefitCalculationDetail>();
            DataTable ldtblLocal52SpAccountPayeeAccount = new DataTable();
            DataTable ldtblLocal161SpAccountPayeeAccount = new DataTable();
            cdoBenefitCalculationDetail lcdoBenefitCalculationDetail;
            if (this.icdoBenefitCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
            {
                ldtblLocal52SpAccountPayeeAccount = Select("cdoBenefitCalculationHeader.GetL52SpAccntPayeeAccount", new object[1] { this.icdoBenefitCalculationHeader.person_id });
                if (ldtblLocal52SpAccountPayeeAccount.Rows.Count >= 0)
                {
                    lcdoBenefitCalculationDetail = new cdoBenefitCalculationDetail();
                    lcdoBenefitCalculationDetail.istrSpecialAccount = busConstant.LOCAL_52_SPECIAL_ACCOUNT;
                    lcolDetail.Add(lcdoBenefitCalculationDetail);
                }

                ldtblLocal161SpAccountPayeeAccount = Select("cdoBenefitCalculationHeader.GetL161SpAccntPayeeAccount", new object[1] { this.icdoBenefitCalculationHeader.person_id });
                if (ldtblLocal161SpAccountPayeeAccount.Rows.Count >= 0)
                {
                    lcdoBenefitCalculationDetail = new cdoBenefitCalculationDetail();
                    lcdoBenefitCalculationDetail.istrSpecialAccount = busConstant.LOCAL1_161_SPECIAL_ACCOUNT;
                    lcolDetail.Add(lcdoBenefitCalculationDetail);

                }
            }
            return lcolDetail;
        }

        public ArrayList CheckIfSpecialAccountAdded(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors)
        {
            utlError lobjError = null;
            busBenefitCalculationPostRetirementDeath lbusDeathPreRetirement = aobj as busBenefitCalculationPostRetirementDeath;

            string lstrSpecialAccount = Convert.ToString(ahstParams["istrSpecialAccount"]);
            if (string.IsNullOrEmpty(lstrSpecialAccount))
            {
                lobjError = AddError(5506, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }
            else
            {
                if (this.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.istrSpecialAccount == lstrSpecialAccount).Count() > 0)
                {
                    lobjError = AddError(5508, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }
            return aarrErrors;
        }
        ////Ticket#137952
        public ArrayList OverrideSurvivorAmount()
        {
            ArrayList larr = new ArrayList();
            //EmergencyOneTimePayment - 03/17/2017
            busBenefitApplication lbusBenApp = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenApp.FindBenefitApplication(this.icdoBenefitCalculationHeader.benefit_application_id);

            if (!this.iclbBenefitCalculationDetail.IsNullOrEmpty())
            {
                if (!this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                {
                    busBenefitCalculationOptions lbusBenefitCalculationOptions = this.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault();
                    if (this.idecOverriddenSurvivorAmount > decimal.Zero)
                    {

                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.overridden_benefit_amount = Math.Round(this.idecOverriddenSurvivorAmount, 2);

                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount = Math.Round(this.idecOverriddenSurvivorAmount, 2);
                        
                        lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.Update();

                       // this.idecOverriddenSurvivorAmount = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.overridden_benefit_amount;
                        
                        foreach(busBenefitCalculationDetail lbusBenefitCalculationDetail in this.iclbBenefitCalculationDetail)
                        {
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.idecSurvivorAmount = Math.Round(this.idecOverriddenSurvivorAmount, 2);
                        }

                        if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.overridden_benefit_amount > 0)
                        {
                            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.overridden_benefit_amount;
                            
                        }
                    }
                }
            }

            larr.Add(this);
            return larr;
        }

        public decimal ldecNonTaxableBeginningBalance { get; set; }
    }
}
