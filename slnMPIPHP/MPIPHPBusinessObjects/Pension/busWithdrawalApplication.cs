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
    /// Class MPIPHP.BusinessObjects.busWithdrawalApplication:
    /// </summary>
    [Serializable]
    public class busWithdrawalApplication : busBenefitApplication
    {
        #region public Methods

        public Collection<cdoPlan> GetPlanValues()
        {
            Collection<cdoPlan> lColPlans = new Collection<cdoPlan>();
            if (!Eligible_Plans.IsNullOrEmpty())
            {
                StringBuilder xyz = new StringBuilder();

                foreach (string plan_code in Eligible_Plans)
                {
                    if (!string.IsNullOrEmpty(xyz.ToString()))
                        xyz.Append(",");
                    xyz.Append("'" + plan_code + "'");
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
            ArrayList iarrErrors = new ArrayList();
            bool lblnIAPCalculated = false;
            bool lblnMPIPPCalculated = false;

            bool lblnL52SplFlag = false;
            bool lblnL161SplFlag = false;
            bool lblnUVHPFlag = false;
            bool lblnNonVestedEEFlag = false;

            int lintMPIPPHeaderId = 0;
            int lintIAPHeaderId = 0;

            if (!this.iblnWithdrawalForAlternatePayee)
            {
                //Call Eligibility Yet Again the Final Time Just Before doing Final Calculation
                this.LoadWorkHistoryandSetupPrerequisites_Withdrawal();
                btn_Approved();



                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    #region Initialize Calculation Needed Objects
                    busBenefitCalculationWithdrawal lbusBenefitCalculationWithdrawal = new busBenefitCalculationWithdrawal { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    busPersonAccount lbusPersonAccount = null;
                    if (lbusBenefitCalculationWithdrawal.ibusCalculation.IsNull())
                    {
                        lbusBenefitCalculationWithdrawal.ibusCalculation = new busCalculation();
                    }
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication = this;
                    lbusBenefitCalculationWithdrawal.ibusPerson = this.ibusPerson;
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.icdoBenefitApplication.withdrawal_date;
                    lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount = this.ibusPerson.iclbPersonAccount;
                    lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.age = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.withdrawal_date);
                    lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
                    lbusBenefitCalculationWithdrawal.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();


                    if (!ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                    {
                        lbusBenefitCalculationWithdrawal.LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                    }

                    else
                    {
                        lbusBenefitCalculationWithdrawal.LoadAllRetirementContributions(null);
                    }

                    #endregion
                   
                    if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                    {
                        #region IAP PLAN FOUND IN GRID
                        if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty() && !lbusBenefitCalculationWithdrawal.iblnCalculateIAPBenefit)
                        {
                            lbusBenefitCalculationWithdrawal.iblnCalculateIAPBenefit = true;

                        }

                        else if (iclbBenefitApplicationDetail.Where(item => item.istrSubPlan == busConstant.L52_SPL_ACC && item.iintPlan_ID == busConstant.IAP_PLAN_ID).Count() > 0 && !lblnL52SplFlag)
                        {
                            lbusBenefitCalculationWithdrawal.iblnCalculateL52SplAccBenefit = true;
                            lblnL52SplFlag = true;
                        }

                        else if (iclbBenefitApplicationDetail.Where(item => item.istrSubPlan == busConstant.L161_SPL_ACC && item.iintPlan_ID == busConstant.IAP_PLAN_ID).Count() > 0 && !lblnL161SplFlag)
                        {
                            lbusBenefitCalculationWithdrawal.iblnCalculateL161SplAccBenefit = true;
                            lblnL161SplFlag = true;
                        }

                        #region Setting Up Header for IAP
                        if (!lblnIAPCalculated)
                        {
                            lbusBenefitCalculationWithdrawal.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id,
                                                                                         busConstant.BENEFIT_TYPE_WITHDRAWAL, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.withdrawal_date,
                                                                                         this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID);
                        }
                        else
                        {
                            if (lbusBenefitCalculationWithdrawal.FindBenefitCalculationHeader(lintIAPHeaderId))
                            {
                                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitApplicationDetail.iintPlan_ID;
                                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitApplication.withdrawal_date);
                            }
                        }
                        #endregion

                        #endregion
                    }

                    else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID)
                    {
                        #region MPIPP PLAN FOUNd in GRID
                        if (lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N")
                        {
                            if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty() && !lbusBenefitCalculationWithdrawal.iblnCalculateMPIPPBenefit)
                            {
                                lbusBenefitCalculationWithdrawal.iblnCalculateMPIPPBenefit = true;

                            }
                            else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag == busConstant.FLAG_YES ||
                                     lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag == busConstant.FLAG_YES)
                            {
                                lbusBenefitCalculationWithdrawal.iblnCalcualteUVHPBenefit = true;
                                lbusBenefitCalculationWithdrawal.iblnCalcualteNonVestedEEBenefit = true;
                                lblnNonVestedEEFlag = true;
                                lblnUVHPFlag = true;

                            }


                            //else if (iclbBenefitApplicationDetail.Where(item => item.istrSubPlan == busConstant.EE && item.iintPlan_ID == busConstant.MPIPP_PLAN_ID).Count() > 0 && !lblnNonVestedEEFlag)
                            //{
                            //    lbusBenefitCalculationWithdrawal.iblnCalcualteNonVestedEEBenefit = true;
                            //    lblnNonVestedEEFlag = true;
                            //}

                            #region Setting Up Header for MPIPP
                            if (!lblnMPIPPCalculated)
                            {
                                lbusBenefitCalculationWithdrawal.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id,
                                                                                             busConstant.BENEFIT_TYPE_WITHDRAWAL, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.withdrawal_date,
                                                                                             this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID);
                            }
                            else
                            {
                                if (lbusBenefitCalculationWithdrawal.FindBenefitCalculationHeader(lintMPIPPHeaderId))
                                {
                                    lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitApplicationDetail.iintPlan_ID;
                                    lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitApplication.retirement_date);
                                }
                            }
                        }
                        #endregion
                        #endregion
                    }
                    if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                    {
                        if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                        }

                        lbusBenefitCalculationWithdrawal.SpawnFinalWithdrawalCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                         this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                         lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);
                    }
                    else
                    {
                        if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                        {
                            if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                            }

                            lbusBenefitCalculationWithdrawal.SpawnFinalWithdrawalCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                             this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                             lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);
                        }
                    }
                    try
                    {
                        if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID && !lblnIAPCalculated)
                        {
                            lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                            lbusBenefitCalculationWithdrawal.PersistChanges();
                            lintIAPHeaderId = lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                            lblnIAPCalculated = true;
                        }
                        else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && !lblnMPIPPCalculated)
                        {
                            if (lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N")
                            {
                                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                                lbusBenefitCalculationWithdrawal.PersistChanges();
                                lintMPIPPHeaderId = lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                                lblnMPIPPCalculated = true;
                            }
                        }

                        if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                        {
                            lbusBenefitCalculationWithdrawal.AfterPersistChanges();
                            SetWorkflowRelatedVariablesforFinalCalculation(lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                        }
                        else
                        {
                            if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                            {
                                lbusBenefitCalculationWithdrawal.AfterPersistChanges();
                                SetWorkflowRelatedVariablesforFinalCalculation(lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                            }

                        }

                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    #region Initialize Calculation Needed Objects
                    busBenefitCalculationWithdrawal lbusBenefitCalculationWithdrawal = new busBenefitCalculationWithdrawal { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.dro_application_id = this.icdoBenefitApplication.dro_application_id;
                    if (lbusBenefitCalculationWithdrawal.ibusCalculation.IsNull())
                    {
                        lbusBenefitCalculationWithdrawal.ibusCalculation = new busCalculation();
                    }
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusQdroApplication = this.ibusQdroApplication;
                    lbusBenefitCalculationWithdrawal.ibusPerson = this.ibusPerson;
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.retirement_date = this.icdoBenefitApplication.withdrawal_date;
                    lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount = this.ibusPerson.iclbPersonAccount;
                    lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.age = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.withdrawal_date);
                    lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
                    lbusBenefitCalculationWithdrawal.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                                       

                        //Benefit Info Of Participant
                        // Initial Setup for Checking Eligbility
                        lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson = this.ibusQdroApplication.ibusParticipant;
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson.iclbPersonAccount = this.ibusQdroApplication.ibusParticipant.iclbPersonAccount;
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)


                    //Load Retirement Contribution For QDRO Participant
                    if (!this.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                    {
                        lbusBenefitCalculationWithdrawal.iclbPersonAccountRetirementContribution =
                            lbusBenefitCalculationWithdrawal.LoadAllRetirementContributions(this.ibusQdroApplication.ibusParticipant.icdoPerson.person_id,
                         this.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                    }
                    else
                    {
                        lbusBenefitCalculationWithdrawal.iclbPersonAccountRetirementContribution =
                            lbusBenefitCalculationWithdrawal.LoadAllRetirementContributions(this.ibusQdroApplication.ibusParticipant.icdoPerson.person_id, null);
                    }

                    #endregion
                   
                    if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                    {
                        #region IAP PLAN FOUND IN GRID
                        if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty() && !lbusBenefitCalculationWithdrawal.iblnCalculateIAPBenefit)
                        {
                            lbusBenefitCalculationWithdrawal.iblnCalculateIAPBenefit = true;

                        }

                        else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                        {
                            lbusBenefitCalculationWithdrawal.iblnCalculateL52SplAccBenefit = true;
                            lblnL52SplFlag = true;
                        }

                        else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                        {
                            lbusBenefitCalculationWithdrawal.iblnCalculateL161SplAccBenefit = true;
                            lblnL161SplFlag = true;
                        }

                        #region Setting Up Header for IAP
                        if (!lblnIAPCalculated)
                        {
                            lbusBenefitCalculationWithdrawal.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id,
                                                                                         busConstant.BENEFIT_TYPE_WITHDRAWAL, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.withdrawal_date,
                                                                                         this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID);
                        }
                        else
                        {
                            if (lbusBenefitCalculationWithdrawal.FindBenefitCalculationHeader(lintIAPHeaderId))
                            {
                                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitApplicationDetail.iintPlan_ID;
                                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitApplication.withdrawal_date);
                               
                            }
                        }
                        #endregion

                        #endregion
                    }

                    else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID)
                    {
                        if (lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N")
                        {

                            #region MPIPP PLAN FOUNd in GRID
                            if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty() && !lbusBenefitCalculationWithdrawal.iblnCalculateMPIPPBenefit)
                            {
                                lbusBenefitCalculationWithdrawal.iblnCalculateMPIPPBenefit = true;

                            }
                            else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag == busConstant.FLAG_YES)
                            {
                                lbusBenefitCalculationWithdrawal.iblnCalcualteUVHPBenefit = true;
                                lblnUVHPFlag = true;
                            }
                            else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag == busConstant.FLAG_YES)
                            {
                                lbusBenefitCalculationWithdrawal.iblnCalcualteNonVestedEEBenefit = true;
                                lblnNonVestedEEFlag = true;
                            }

                        }


                        #region Setting Up Header for MPIPP
                        if (lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N")
                        {
                            if (!lblnMPIPPCalculated)
                            {
                                lbusBenefitCalculationWithdrawal.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id,
                                                                                             busConstant.BENEFIT_TYPE_WITHDRAWAL, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.withdrawal_date,
                                                                                             this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID);
                            }
                            else
                            {
                                if (lbusBenefitCalculationWithdrawal.FindBenefitCalculationHeader(lintMPIPPHeaderId))
                                {
                                    lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitApplicationDetail.iintPlan_ID;
                                    lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitApplication.retirement_date);
                                }
                            }
                        }
                        #endregion
                        #endregion
                    }

                    if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                    {
                        if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                        }

                        lbusBenefitCalculationWithdrawal.SpawnFinalWithdrawalCalculationForAlternatePayee(this.ibusQdroApplication, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                         this.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                         lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue,
                                                                                         lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.dro_model_value);
                    }
                    else
                    {
                        if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                        {

                            if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                            }

                            lbusBenefitCalculationWithdrawal.SpawnFinalWithdrawalCalculationForAlternatePayee(this.ibusQdroApplication, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                             this.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                             lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue,
                                                                                             lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.dro_model_value);
                        }

                    }

                    try
                    {
                        if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID && !lblnIAPCalculated)
                        {
                            lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                            lbusBenefitCalculationWithdrawal.PersistChanges();
                            lintIAPHeaderId = lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                            lblnIAPCalculated = true;

                        }

                        else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && !lblnMPIPPCalculated)
                        {
                            if (lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N")
                            {
                                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                                lbusBenefitCalculationWithdrawal.PersistChanges();
                                lintMPIPPHeaderId = lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                                lblnMPIPPCalculated = true;

                            }

                        }
                        if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                        {
                            lbusBenefitCalculationWithdrawal.AfterPersistChanges();
                            SetWorkflowRelatedVariablesforFinalCalculation(lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                        }
                        else
                        {
                            if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                            {
                                lbusBenefitCalculationWithdrawal.AfterPersistChanges();
                                SetWorkflowRelatedVariablesforFinalCalculation(lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                            }

                        }

                    }
                    catch
                    {
                    }
                }
            }

            if (this.ibusBaseActivityInstance.IsNotNull())
            {
                this.SetProcessInstanceParameters();
            }
            this.icdoBenefitApplication.final_calc_flag = busConstant.FLAG_YES;
            this.icdoBenefitApplication.change_benefit_option_flag = busConstant.FLAG_NO;
            this.icdoBenefitApplication.Update();
            this.EvaluateInitialLoadRules();
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

        //Code-Abhishek
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (this.icdoBenefitApplication.withdrawal_date != DateTime.MinValue)
            {
                //PIR - 839            
                this.icdoBenefitApplication.retirement_date = DateTime.Now;
                //this.icdoBenefitApplication.retirement_date = this.icdoBenefitApplication.withdrawal_date;

                this.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.withdrawal_date);
                //PIR - 839
                //if (this.iobjPassInfo.ienmPageMode == utlPageMode.New)
                //{
                this.LoadandProcessWorkHistory_ForAllPlans();
                //}
                this.icdoBenefitApplication.retirement_date = this.icdoBenefitApplication.withdrawal_date; //PIR - 839  

                if (this.icdoBenefitApplication.dro_application_id <= 0)
                    SetupPrerequisites();
            }
            base.BeforeValidate(aenmPageMode);
        }
        //Code-Abhishek


        private void SetupPrerequisites()
        {
            if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                //PIR-531
                if ((this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).IsNotNull()) && (this.icdoBenefitApplication.child_support_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty())) //EmergencyOneTimePayment - 03/17/2020
                {
                    var a = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                    //EmergencyOneTimePayment - 03/17/2020
                    if (this.icdoBenefitApplication.child_support_flag == busConstant.FLAG_YES)
                    {
                        DataTable ldtbIAPAmt = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalance", new object[1] { a });
                        if (!Convert.ToBoolean(ldtbIAPAmt.Rows[0][0].IsDBNull()))
                        {
                            decimal ldecIAPBalanceAmt = Convert.ToDecimal(ldtbIAPAmt.Rows[0][0]);
                            if (ldecIAPBalanceAmt > 0)
                            {
                                Eligible_Plans.Clear();
                                Eligible_Plans.Add(busConstant.IAP);

                            }
                        }
                        else
                            this.iblnCheckForChildSupport = busConstant.BOOL_TRUE;
                    }
                    //EmergencyOneTimePayment - 03/17/2020
                    if (this.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty())
                    {
                        DataTable ldtbEmergencyPaymentSetupValue = new DataTable();
                        ldtbEmergencyPaymentSetupValue = busBase.Select("cdoEmergencyPaymentSetupValue.GetEmergencyPaymentSetupValue", new object[1] { DateTime.Now });
                        DataTable ldtbIAPAmt = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                                       new object[2] { a, Convert.ToInt32(ldtbEmergencyPaymentSetupValue.Rows[0]["IAP_BALANCE_AS_OF_YEAR"]) });

                        if (!Convert.ToBoolean(ldtbIAPAmt.Rows[0][0].IsDBNull()) && ldtbIAPAmt.Rows.Count > 0)
                        {
                            if (Convert.ToDecimal(ldtbIAPAmt.Rows[0]["IAP_BALANCE_AMOUNT"]) > 0 || Convert.ToDecimal(ldtbIAPAmt.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) > 0 || Convert.ToDecimal(ldtbIAPAmt.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]) > 0)
                            {
                                Eligible_Plans.Clear();
                                Eligible_Plans.Add(busConstant.IAP);
                            }
                        }
                    }
                }
                else
                {
                    this.DetermineVesting();
                    this.DetermineBenefitSubTypeandEligibility_Withdrawal();
                }
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

            //this.ibusPerson.iclbNotes.ForEach(item =>
            //{
            //    if (item.icdoNotes.person_id == 0)
            //        item.icdoNotes.person_id = this.icdoBenefitApplication.person_id;
            //    item.icdoNotes.form_id = busConstant.Form_ID;
            //    item.icdoNotes.form_value = busConstant.WITHDRAWL_APPLICATION_MAINTAINENCE_FORM;
            //});
        }


        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            this.EvaluateInitialLoadRules();

            base.ValidateHardErrors(aenmPageMode);
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            if (this.NotEligible.IsNotNull() && this.NotEligible)
            {
                lobjError = AddError(5153, " ");
                this.iarrErrors.Add(lobjError);
            }

            if (this.icdoBenefitApplication.dro_application_id > 0)
            {
                if (this.ibusQdroApplication.icdoDroApplication.dro_commencement_date == DateTime.MinValue)
                {
                    lobjError = AddError(5464, "");
                    this.iarrErrors.Add(lobjError);
                }
            }
            if (!this.iblnWithdrawalForAlternatePayee)
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    Hashtable lhstParams = new Hashtable();
                    lhstParams["iintPlan_ID"] = lbusBenefitApplicationDetail.iintPlan_ID;
                    lhstParams["icdoBenefitApplicationDetail.plan_benefit_id"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id;
                    lhstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintJointAnnuaintID;
                    lhstParams["icdoBenefitApplicationDetail.spousal_consent_flag"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.spousal_consent_flag;
                    lhstParams["istrSubPlan"] = lbusBenefitApplicationDetail.istrSubPlan;
                    this.iarrErrors = CheckErrorOnAddButton(this, lhstParams, ref iarrErrors, true);
                }
            }
            if (this.iblnWithdrawalForAlternatePayee)
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID
                        && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue != busConstant.LUMP_SUM)
                    {
                        lobjError = AddError(5465, "");
                        this.iarrErrors.Add(lobjError);
                        return;
                    }
                }
            }
            //EmergencyOneTimePayment - 03/17/2020
            if(this.icdoBenefitApplication.child_support_flag == "Y" && this.icdoBenefitApplication.emergency_onetime_payment_flag == "Y")
            {
                lobjError = AddError(6299, "");
                this.iarrErrors.Add(lobjError);
                return;
            }
            //EmergencyOneTimePayment - 03/17/2020
            if((this.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty()) && this.icdoBenefitApplication.covid_withdrawal_amount == 0)
            {
                lobjError = AddError(6300, "");
                this.iarrErrors.Add(lobjError);
                return;
            }

            //EmergencyOneTimePayment - 03/17/2020
            //The below validation is written to decide which plans(iap, iap-L52, iap-L161) to be used to withdraw the grossamount. The grossamount is calculated based on net amount.
            if ((this.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty()) && this.icdoBenefitApplication.covid_withdrawal_amount > 0)
            {
                DataTable ldtbEmergencyPaymentSetupValue = new DataTable();
                DataTable ldtbIAPBalance = new DataTable();
                DataTable ldtbIAPRetirement = new DataTable();
                DataTable ldtbIAPHardhsipWithdrawal = new DataTable();
                decimal ldecGrossAmount= 0, ldecFedTaxAmount = 0, ldecStateTaxAmount = 0;

                string lblnOption1IAPOnlyFlag = busConstant.FLAG_NO;
                string lblnOption2L52OnlyFlag = busConstant.FLAG_NO;
                string lblnOption3L161OnlyFlag = busConstant.FLAG_NO;
                string lblnOption4L52AndL161OnlyFlag = busConstant.FLAG_NO;
                string lblnOption5AllFlag = busConstant.FLAG_NO;

                var a = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                ldtbEmergencyPaymentSetupValue = busBase.Select("cdoEmergencyPaymentSetupValue.GetEmergencyPaymentSetupValue", new object[1] { DateTime.Now });
                ldtbIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceAsofYear",
                               new object[2] { a, Convert.ToInt32(ldtbEmergencyPaymentSetupValue.Rows[0]["IAP_BALANCE_AS_OF_YEAR"]) });
                

                ldtbIAPRetirement = busBase.Select("cdoBenefitApplication.CheckIfParticipantHasApprovedRetirementApplicationInIAP", new object[1] {this.icdoBenefitApplication.person_id});
                ldtbIAPHardhsipWithdrawal = busBase.Select("cdoBenefitApplication.CheckIfParticipantHasIAPHardshipWithdrawalApplication", new object[1] { this.icdoBenefitApplication.person_id });
                idecCovidIAP2018BalanceAmt = Math.Round(Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]), 2);


                if (ldtbIAPRetirement.Rows.Count > 0)
                {
                    lobjError = AddError(5153, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }

                if(ldtbIAPHardhsipWithdrawal.Rows.Count > 0)
                {
                    lobjError = AddError(5151, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }

                //Calculate gross amount, fed tax amount and State tax amount
                if (this.icdoBenefitApplication.covid_federal_tax_percentage != 0 || this.icdoBenefitApplication.covid_state_tax_percentage != 0)
                {
                    ldecGrossAmount = Math.Round(this.icdoBenefitApplication.covid_withdrawal_amount / ((100 - (this.icdoBenefitApplication.covid_federal_tax_percentage + this.icdoBenefitApplication.covid_state_tax_percentage))/100),2);
                }
                else
                {
                    ldecGrossAmount = Math.Round(this.icdoBenefitApplication.covid_withdrawal_amount,2);
                }

                //If the 2018 Total IAP Balance < MINLIMIT then show error message:  Minimum required balance is not available in the IAP Plan Account.
                if (idecCovidIAP2018BalanceAmt < Convert.ToDecimal(ldtbEmergencyPaymentSetupValue.Rows[0]["MINLIMIT"]))
                {
                    lobjError = AddError(6305, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }

                if (Convert.ToDecimal(this.icdoBenefitApplication.covid_withdrawal_amount) > Convert.ToDecimal(ldtbEmergencyPaymentSetupValue.Rows[0]["MAXLIMIT"]))
                {
                    lobjError = AddError(6298, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }

                if (Convert.ToDecimal(this.icdoBenefitApplication.covid_withdrawal_amount) > Math.Round(Math.Round(Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]), 2) * Convert.ToDecimal(ldtbEmergencyPaymentSetupValue.Rows[0]["PERCENTAGE"])/100,2))
                {
                    lobjError = AddError(6298, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }

                if (Convert.ToDecimal(this.icdoBenefitApplication.covid_federal_tax_percentage) != 0 && Convert.ToDecimal(this.icdoBenefitApplication.covid_federal_tax_percentage)!= 10 && Convert.ToDecimal(this.icdoBenefitApplication.covid_federal_tax_percentage) != 20)
                {
                    lobjError = AddError(6297, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }

                if (Convert.ToDecimal(this.icdoBenefitApplication.covid_state_tax_percentage) != 0 && Convert.ToDecimal(this.icdoBenefitApplication.covid_state_tax_percentage) != 1 && Convert.ToDecimal(this.icdoBenefitApplication.covid_state_tax_percentage) != 2)
                {
                    lobjError = AddError(6297, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }

                if(Convert.ToDecimal(this.icdoBenefitApplication.covid_state_tax_percentage) != 0 && Convert.ToDecimal(this.icdoBenefitApplication.covid_federal_tax_percentage) == 10 && Convert.ToDecimal(this.icdoBenefitApplication.covid_state_tax_percentage) != 1)
                {
                    lobjError = AddError(6297, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }

                if (Convert.ToDecimal(this.icdoBenefitApplication.covid_state_tax_percentage) != 0 && Convert.ToDecimal(this.icdoBenefitApplication.covid_federal_tax_percentage) == 20 && Convert.ToDecimal(this.icdoBenefitApplication.covid_state_tax_percentage) != 2)
                {
                    lobjError = AddError(6297, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }

                if (Convert.ToDecimal(this.icdoBenefitApplication.covid_state_tax_percentage) != 0)
                {
                    bool lblnCAState = false;
                    this.ibusPerson.LoadActiveAddressOfMember();

                    foreach (busPersonAddress lbusPersonAddress in this.ibusPerson.iclbPersonAddress)
                    {
                        if ((lbusPersonAddress.icdoPersonAddress.end_date >= DateTime.Now || lbusPersonAddress.icdoPersonAddress.end_date == DateTime.MinValue) &&
                           lbusPersonAddress.icdoPersonAddress.start_date <= DateTime.Now && lbusPersonAddress.icdoPersonAddress.start_date != lbusPersonAddress.icdoPersonAddress.end_date)
                        {
                            foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in lbusPersonAddress.iclcPersonAddressChklist)
                            {
                                if (lcdoPersonAddressChklist.address_type_value == busConstant.MAILING_ADDRESS || lcdoPersonAddressChklist.address_type_value == busConstant.PHYSICAL_AND_MAILING_ADDRESS)
                                {
                                    if (lbusPersonAddress.icdoPersonAddress.addr_state_value == busConstant.CALIFORNIA)
                                    {
                                        lblnCAState = true;                                        
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if(!lblnCAState)
                    {
                        lobjError = AddError(6304, "");
                        this.iarrErrors.Add(lobjError);
                        return;
                    }
                }

                //Check Option1 - IAP has enough money to withdraw the cash
                if (ldecGrossAmount <= Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]))
                {
                    lblnOption1IAPOnlyFlag = busConstant.FLAG_YES;
                    this.icdoBenefitApplication.covid_option_id = 7096;
                    this.icdoBenefitApplication.covid_option_value = busConstant.COVID_IAP_ONLY_OPTION;
                }

                //Check Option2 - IAP is lesser , but L52 has enough to match the gross amount.
                if (lblnOption1IAPOnlyFlag == "N" && ldecGrossAmount > Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) && ldecGrossAmount <= Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]))
                {
                    lblnOption2L52OnlyFlag = busConstant.FLAG_YES;
                    this.icdoBenefitApplication.covid_option_id = 7096;
                    this.icdoBenefitApplication.covid_option_value = busConstant.COVID_L52_SPL_AC_ONLY_OPTION;
                }

                //Check Option3 - IAP & L52 is lesser and L161 has enough to match the gross amount.
                if (lblnOption2L52OnlyFlag == "N" && ldecGrossAmount > Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) && ldecGrossAmount > Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) && ldecGrossAmount <= Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]))
                {
                    lblnOption3L161OnlyFlag = busConstant.FLAG_YES;
                    this.icdoBenefitApplication.covid_option_id = 7096;
                    this.icdoBenefitApplication.covid_option_value = busConstant.COVID_L161_SPL_AC_ONLY_OPTION;
                }

                //Check Option4 - IAP is lesser, but L52 & L161 has enough to match the gross amount.
                if(lblnOption2L52OnlyFlag == "N" && lblnOption3L161OnlyFlag == "N" && ldecGrossAmount > Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) && ldecGrossAmount <= (Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"])))
                {
                    lblnOption4L52AndL161OnlyFlag = busConstant.FLAG_YES;
                    this.icdoBenefitApplication.covid_option_id = 7096;
                    this.icdoBenefitApplication.covid_option_value = busConstant.COVID_L52_L161_SPL_AC_ONLY_OPTION;
                }

                //Check Option5 - All Plans needed to match the gross amount.
                if(lblnOption1IAPOnlyFlag == "N" && lblnOption2L52OnlyFlag == "N" && lblnOption3L161OnlyFlag == "N" && lblnOption4L52AndL161OnlyFlag == "N" &&
                    ldecGrossAmount > Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) && 
                   ldecGrossAmount > Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) && 
                   ldecGrossAmount > Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]) &&
                   ldecGrossAmount <= Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) + Convert.ToDecimal(ldtbIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"])
                   )
                {
                    lblnOption5AllFlag = busConstant.FLAG_YES;
                    this.icdoBenefitApplication.covid_option_id = 7096;
                    this.icdoBenefitApplication.covid_option_value = busConstant.COVID_ALL_OPTION;
                }

                if(lblnOption1IAPOnlyFlag == busConstant.FLAG_YES)
                {
                    if (iclbBenefitApplicationDetail != null && iclbBenefitApplicationDetail.Count > 0)
                    {
                        if (iclbBenefitApplicationDetail.Where(t => t.icdoBenefitApplicationDetail.l52_spl_acc_flag == busConstant.FLAG_YES  || t.icdoBenefitApplicationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).Count() > 0)
                        {
                            lobjError = AddError(6301, "");
                            this.iarrErrors.Add(lobjError);
                            return;
                        }
                    }
                }

                if(lblnOption2L52OnlyFlag == busConstant.FLAG_YES)
                {
                    if (iclbBenefitApplicationDetail != null && iclbBenefitApplicationDetail.Count > 0)
                    {
                        if (iclbBenefitApplicationDetail.Where(t => t.icdoBenefitApplicationDetail.l161_spl_acc_flag == busConstant.FLAG_YES || t.icdoBenefitApplicationDetail.l52_spl_acc_flag == busConstant.FLAG_NO || t.icdoBenefitApplicationDetail.l52_spl_acc_flag.IsNullOrEmpty()).Count() > 0)
                        {
                            lobjError = AddError(6302, "");
                            this.iarrErrors.Add(lobjError);
                            return;
                        }
                    }
                }

                if (lblnOption3L161OnlyFlag == busConstant.FLAG_YES)
                {
                    if (iclbBenefitApplicationDetail != null && iclbBenefitApplicationDetail.Count > 0)
                    {
                        if (iclbBenefitApplicationDetail.Where(t => t.icdoBenefitApplicationDetail.l52_spl_acc_flag == busConstant.FLAG_YES || t.icdoBenefitApplicationDetail.l161_spl_acc_flag == busConstant.FLAG_NO || t.icdoBenefitApplicationDetail.l161_spl_acc_flag.IsNullOrEmpty()).Count() > 0)
                        {
                            lobjError = AddError(6302, "");
                            this.iarrErrors.Add(lobjError);
                            return;
                        }
                    }
                }

                if (lblnOption4L52AndL161OnlyFlag == busConstant.FLAG_YES)
                {
                    if (iclbBenefitApplicationDetail != null && iclbBenefitApplicationDetail.Count > 0)
                    {
                        if (iclbBenefitApplicationDetail.Count == 1 ||
                            (iclbBenefitApplicationDetail.Count == 2 && (iclbBenefitApplicationDetail.Where(t => t.icdoBenefitApplicationDetail.l161_spl_acc_flag == busConstant.FLAG_NO || t.icdoBenefitApplicationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).Count() > 0)) ||
                            (iclbBenefitApplicationDetail.Count > 2 && iclbBenefitApplicationDetail.Where(t => t.icdoBenefitApplicationDetail.l161_spl_acc_flag == busConstant.FLAG_YES).Count() > 0 && iclbBenefitApplicationDetail.Where(t => t.icdoBenefitApplicationDetail.l52_spl_acc_flag == busConstant.FLAG_YES).Count() > 0)
                           )
                        {
                            lobjError = AddError(6303, "");
                            this.iarrErrors.Add(lobjError);
                            return;
                        }                        
                    }
                }
            }


            bool lblnIsMPIPlanAdded = false;
            busRetirementApplication lbusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusRetirementApplication.icdoBenefitApplication.person_id = icdoBenefitApplication.person_id;
            DateTime ldtRetirementDate = lbusRetirementApplication.GetRetirementDate();

            if (iclbBenefitApplicationDetail != null && iclbBenefitApplicationDetail.Count > 0)
            {
                if (iclbBenefitApplicationDetail.Where(t => t.iintPlan_ID == busConstant.MPIPP_PLAN_ID).Count() > 0)
                {
                    lblnIsMPIPlanAdded = true;
                }
            }

            if (ldtRetirementDate != DateTime.MinValue && ldtRetirementDate != icdoBenefitApplication.retirement_date && lblnIsMPIPlanAdded)
            {
                lobjError = AddError(6088, "");
                this.iarrErrors.Add(lobjError);
                return;
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
