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

#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busDisabilityApplication : busBenefitApplication
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
                                                                iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);
                if (ldtbListofPLans.Rows.Count > 0)
                {
                    lColPlans = Sagitec.DataObjects.doBase.GetCollection<cdoPlan>(ldtbListofPLans);
                }
            }

            return lColPlans;   
        }

        public void checkMarriage(busDisabilityApplication abusDisabilityApplication, Hashtable ahstParams, ref ArrayList larrErrors)
        {
            utlError lobjError = null;
            string lstrBenefitOption = string.Empty;
            int lintPlanID = 0;
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["iintPlan_ID"])))
            {
                lintPlanID = Convert.ToInt32(ahstParams["iintPlan_ID"]);
            }
            int lintJointAnnID = 0;
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"])))
            {
                lintJointAnnID = Convert.ToInt32(ahstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"]);
            }

            if (ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"] != null)
            {
                lstrBenefitOption = Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"]);
            }
            else if (ahstParams["icdoBenefitApplicationDetail.plan_benefit_id"] != null)
            {
                busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                lbusPlanBenefitXr.FindPlanBenefitXr(Convert.ToInt32(ahstParams["icdoBenefitApplicationDetail.plan_benefit_id"]));
                lstrBenefitOption = lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;
            }


            //COMMENTED ACCORDING TO THE UAT PIR 113 where they want to REMOVE this VALDIATION ALL OTHER TYPES APART FROM DEATH
            //if ((this.ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_MARRIED) && (this.CheckBenefitOPtions(lstrBenefitOption)))
            //{
            //    DateTime lDateOfMarriage = DateTime.MinValue;
            //    DataTable ldtDateofMarr = Select("cdoRelationship.GetDateOfMarriage", new object[2] { this.ibusPerson.icdoPerson.person_id, lintJointAnnID });
            //    if (ldtDateofMarr.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(ldtDateofMarr.Rows[0][enmRelationship.date_of_marriage.ToString()])))
            //    {
            //        lDateOfMarriage = Convert.ToDateTime(ldtDateofMarr.Rows[0][enmRelationship.date_of_marriage.ToString()]);
            //    }
            //    TimeSpan ltsSpan = DateTime.Now.Subtract(lDateOfMarriage);
            //    if ((ltsSpan.Days <= 365 || (DateTime.IsLeapYear(DateTime.Now.Year) && ltsSpan.Days <= 366)) || lDateOfMarriage == DateTime.MinValue)
            //    {
            //        lobjError = AddError(5034, "");
            //        larrErrors.Add(lobjError);
            //    }
            //}
            //if ((lintPlanID != busConstant.LOCAL_700_PLAN_ID) && (this.ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_SINGLE) && (lstrBenefitOption == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY || lstrBenefitOption == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY))
            //{
            //    DataTable ldtbbenPar = Select("cdoBenefitApplication.GetChildBeneficaryOfParticipant", new object[1] { this.ibusPerson.icdoPerson.person_id });
            //    if (ldtbbenPar.Rows.Count > 0)
            //    {
            //        DataRow drRow = ldtbbenPar.Rows[0];
            //        int lintCount = Convert.ToInt32(drRow[0]);
            //        if (lintCount == 0)
            //        {
            //            lobjError = AddError(5024, "");
            //            larrErrors.Add(lobjError);
            //        }
            //    }
            //}
        }

        public void CheckDatesOnBenefitAppDetails(busDisabilityApplication abusDisabilityApplication, Hashtable ahstParams, ref ArrayList larrErrors)
        {
            utlError lobjError = null;
            string lstrBenefitOption = string.Empty;

            int lintPlanID = 0;
            if(!string.IsNullOrEmpty(Convert.ToString(ahstParams["iintPlan_ID"])))
            {
                lintPlanID = Convert.ToInt32(ahstParams["iintPlan_ID"]);
            }

            if (ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"] != null)
            {
                lstrBenefitOption = Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"]);
            }
            else if (ahstParams["icdoBenefitApplicationDetail.plan_benefit_id"] != null)
            {
                busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                lbusPlanBenefitXr.FindPlanBenefitXr(Convert.ToInt32(ahstParams["icdoBenefitApplicationDetail.plan_benefit_id"]));
                lstrBenefitOption = lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;
            }

            if (lintPlanID == busConstant.IAP_PLAN_ID)
            {
                if (ahstParams["icdoBenefitApplication.terminally_ill_flag"] != null && (Convert.ToChar(ahstParams["icdoBenefitApplication.terminally_ill_flag"]) != 'Y'))
                {
                    //151767
                    if (!IsOnlyOnePlanAllowed())
                    {
                        if ((Convert.ToString(ahstParams["icdoBenefitApplication.awarded_on_date"]) == string.Empty) || (Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]) == DateTime.MinValue))
                        {
                            lobjError = AddError(5104, "");
                            larrErrors.Add(lobjError);
                            return;
                        }
                    }
                    //
                    /*
                    if ((Convert.ToString(ahstParams["icdoBenefitApplication.disability_onset_date"]) == string.Empty) || (Convert.ToDateTime(ahstParams["icdoBenefitApplication.disability_onset_date"]) == DateTime.MinValue))
                    {
                        bool flag = false;
                        foreach (utlError err in larrErrors)
                        {
                            if (err.istrErrorID == "5052")
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            lobjError = AddError(5052, "");
                            larrErrors.Add(lobjError);
                        }
                        return;
                    }
                    if ((Convert.ToString(ahstParams["icdoBenefitApplication.ssa_application_date"]) == string.Empty) || (Convert.ToDateTime(ahstParams["icdoBenefitApplication.ssa_application_date"]) == DateTime.MinValue))
                    {
                        bool flag = false;
                        foreach (utlError err in larrErrors)
                        {
                            if (err.istrErrorID == "5122")
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            lobjError = AddError(5122, "");
                            larrErrors.Add(lobjError);
                        }
                        return;
                    }*/
                    if ((Convert.ToString(ahstParams["icdoBenefitApplication.retirement_date"]) == string.Empty) || (Convert.ToDateTime(ahstParams["icdoBenefitApplication.retirement_date"]) == DateTime.MinValue))
                    {
                        bool flag = false;
                        foreach (utlError err in larrErrors)
                        {
                            if (err.istrErrorID == "5027")
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            lobjError = AddError(5027, "");
                            larrErrors.Add(lobjError);
                        }
                        return;
                    }               
                    
                    DateTime ldate = new DateTime();
                    ldate = DateTime.Parse("06/26/2002");

                    if (!Eligible_Plans.Contains(busConstant.MPIPP))
                    {
                        if ((Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]) < ldate))
                        {
                            lobjError = AddError(5040, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                    if ((Convert.ToString(ahstParams["icdoBenefitApplication.awarded_on_date"]).IsNotNullOrEmpty() && Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]) != DateTime.MinValue)
                        && Convert.ToString(ahstParams["icdoBenefitApplication.disability_onset_date"]).IsNotNullOrEmpty()
                        && Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]) <= Convert.ToDateTime(ahstParams["icdoBenefitApplication.disability_onset_date"]))
                    {
                        lobjError = AddError(5126, "");
                        larrErrors.Add(lobjError);
                    }
                    if (Convert.ToString(ahstParams["icdoBenefitApplication.ssa_application_date"]).IsNotNullOrEmpty())
                        if (Convert.ToDateTime(ahstParams["icdoBenefitApplication.ssa_application_date"]) != DateTime.MinValue)
                        {
                            if (Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]) <= Convert.ToDateTime(ahstParams["icdoBenefitApplication.ssa_application_date"]))
                            {
                                lobjError = AddError(5127, "");
                                larrErrors.Add(lobjError);
                            }
                        }
                    //if ((Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]).IsNotNull() && Convert.ToDateTime(ahstParams["icdoBenefitApplication.retirement_date"]).IsNotNull()) 
                    //    && Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]) > Convert.ToDateTime(ahstParams["icdoBenefitApplication.retirement_date"]))
                    //{
                    //    lobjError = AddError(5132, "");
                    //    larrErrors.Add(lobjError);
                    //}                    
                }
            }
            //Validation Removed as Per UAT PIR : 264
            //if (lintPlanID == busConstant.MPIPP_PLAN_ID)
            //{
            //    if (!IsAppRcvdDateWithin2YearsPriorTOEarlyRetirementDate())
            //    {
            //        lobjError = AddError(5124, "");
            //        larrErrors.Add(lobjError);
            //    }
            //}

            //if (!this.iblnDisabilityConversion && lintPlanID == busConstant.MPIPP_PLAN_ID && IsSSDappDateGreaterThnEarlyRetrDate())
            //{
            //    lobjError = AddError(5149, "");
            //    larrErrors.Add(lobjError);
            //}
        }
        
        public void CheckAwardedOnDateOnSave(busDisabilityApplication abusDisabilityApplication, Hashtable ahstParams, ref ArrayList larrErrors)
        {
            if (Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]) != DateTime.MinValue)
            {
                utlError lobjError = null;
                DateTime ldate = new DateTime();
                ldate = DateTime.Parse("06/26/2002");

                if (!Eligible_Plans.Contains(busConstant.MPIPP))
                {
                    if ((Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]) < ldate))
                    {
                        lobjError = AddError(5040, "");
                        larrErrors.Add(lobjError);
                    }
                }
                if ((Convert.ToString(ahstParams["icdoBenefitApplication.awarded_on_date"]).IsNotNullOrEmpty() && Convert.ToString(ahstParams["icdoBenefitApplication.disability_onset_date"]).IsNotNullOrEmpty())
                    && Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]) <= Convert.ToDateTime(ahstParams["icdoBenefitApplication.disability_onset_date"]))
                {
                    lobjError = AddError(5126, "");
                    larrErrors.Add(lobjError);
                }
                 if (Convert.ToString(ahstParams["icdoBenefitApplication.ssa_application_date"]).IsNotNullOrEmpty())
                     if (Convert.ToDateTime(ahstParams["icdoBenefitApplication.ssa_application_date"]) != DateTime.MinValue)
                     {
                         if (Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]) <= Convert.ToDateTime(ahstParams["icdoBenefitApplication.ssa_application_date"]))
                         {
                             lobjError = AddError(5127, "");
                             larrErrors.Add(lobjError);
                         }
                     }
                //if (Convert.ToDateTime(ahstParams["icdoBenefitApplication.awarded_on_date"]) > Convert.ToDateTime(ahstParams["icdoBenefitApplication.retirement_date"]))
                //{
                //    lobjError = AddError(5132, "");
                //    larrErrors.Add(lobjError);
                //}
            }
        }               

        public bool CheckBenefitOPtions(string astrBenefitOptions)
        {
            if (astrBenefitOptions == "JS75" || astrBenefitOptions == "JP75" || astrBenefitOptions == "J100" || astrBenefitOptions == "JPOP")
            {
                return true;
            }
            return false;
        }

        public void CheckDuplicatePlan(Hashtable ahstParams,ref ArrayList larrErrors)
        {
            utlError lobjError = null;
            //if (ahstParams["icdoDisabilityBenefitHistory.plan_id"] == "")
            //{
            //    lobjError = AddError(1126, "");
            //    larrErrors.Add(lobjError);
            //    return;
            //}
            ahstParams["icdoDisabilityBenefitHistory.disability_cont_letter_date"] = ahstParams["disability_cont_letter_date"];
            ahstParams["icdoDisabilityBenefitHistory.received"] = ahstParams["received"];
            ahstParams["icdoDisabilityBenefitHistory.sent"] = ahstParams["sent"];

            if (ahstParams["icdoDisabilityBenefitHistory.disability_cont_letter_date"] == "")
            {
                lobjError = AddError(5045, "");
                larrErrors.Add(lobjError);
                return;
            }
            else
            {
                //int lintPlanid = Convert.ToInt32(ahstParams["icdoDisabilityBenefitHistory.plan_id"]);                                  
                DateTime ldtDisability = Convert.ToDateTime(ahstParams["icdoDisabilityBenefitHistory.disability_cont_letter_date"]);

                foreach (busDisabilityBenefitHistory lbusDisabilityBenefitHistory in iclbDisabilityBenefitHistory)
                {

                    //if (lbusDisabilityBenefitHistory.icdoDisabilityBenefitHistory.disability_cont_letter_date == ldtDisability && lbusDisabilityBenefitHistory.icdoDisabilityBenefitHistory.plan_id == lintPlanid)
                    //{
                    //    lobjError = AddError(5035, "");
                    //    larrErrors.Add(lobjError);
                    //    break;
                    //}
                    if (ldtDisability <= lbusDisabilityBenefitHistory.icdoDisabilityBenefitHistory.disability_cont_letter_date)
                    {
                        lobjError = AddError(5143, "");
                        larrErrors.Add(lobjError);
                        break;
                    }
                }                                    
                if (Convert.ToDateTime(ahstParams["icdoDisabilityBenefitHistory.disability_cont_letter_date"]) < Convert.ToDateTime(this.icdoBenefitApplication.disability_onset_date))
                {
                    lobjError = AddError(5049, "");
                    larrErrors.Add(lobjError);
                }
                if (Convert.ToString(ahstParams["icdoDisabilityBenefitHistory.received"]) == "Y" && Convert.ToDateTime(ahstParams["icdoDisabilityBenefitHistory.disability_cont_letter_date"]) > DateTime.Now)
                {
                    lobjError = AddError(6092, "");
                    larrErrors.Add(lobjError);
                }              
                if (Convert.ToString(ahstParams["icdoDisabilityBenefitHistory.received"]) == "Y" && Convert.ToString(ahstParams["icdoDisabilityBenefitHistory.sent"]) == "Y")
                {
                    lobjError = AddError(6093, "");
                    larrErrors.Add(lobjError);
                }
            }         
        }

        public void LoadCorrespondenceProperties()
        {
            iblnIsOnsetDateLessThanRetrDate = false;
            iblnDisConvDate = false;

            DateTime ldtCurrentDate = System.DateTime.Now;
            istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
          
            if (icdoBenefitApplication.disability_onset_date != DateTime.MinValue)
            {
                istrDisabilityOnsetDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.icdoBenefitApplication.disability_onset_date);

                if (icdoBenefitApplication.disability_onset_date < icdoBenefitApplication.retirement_date)
                {
                    iblnIsOnsetDateLessThanRetrDate = true;
                }
            }

            if (icdoBenefitApplication.disability_conversion_date != DateTime.MinValue)
            {
                iblnDisConvDate = true;
                istrDisabilityConvDate = busGlobalFunctions.ConvertDateIntoDifFormat(icdoBenefitApplication.disability_conversion_date);
            }

            if (icdoBenefitApplication.application_received_date != DateTime.MinValue)
            {
                istrApplicationReceivedDate = busGlobalFunctions.ConvertDateIntoDifFormat(icdoBenefitApplication.application_received_date);
            }

            if (icdoBenefitApplication.retirement_date != DateTime.MinValue)
            {
                DateTime lRetirementDt = icdoBenefitApplication.retirement_date;
                istrRetirementDt = Convert.ToString(lRetirementDt);
                istrRetirementDt = busGlobalFunctions.ConvertDateIntoDifFormat(lRetirementDt);
                iintRetrDateYear = lRetirementDt.Year;

                DateTime lRetrDateTemp = lRetirementDt.AddMonths(-1);
                idateRetirement = lRetrDateTemp.AddDays(-1);
                istrPriorToRetirement = Convert.ToString(idateRetirement);
                istrPriorToRetirement = busGlobalFunctions.ConvertDateIntoDifFormat(idateRetirement);
            }
            if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES && this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES)
            {
                if (this.aclbPersonWorkHistory_IAP != null && this.aclbPersonWorkHistory_IAP.Count > 0)
                {
                    iintVestedYears = this.aclbPersonWorkHistory_IAP.Last().vested_years_count;
                    idecVestedHours = (from item in this.aclbPersonWorkHistory_IAP select item.vested_hours).Sum();
                }
            }
            else if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES)
            {
                if (this.aclbPersonWorkHistory_MPI != null && this.aclbPersonWorkHistory_MPI.Count > 0)
                {
                    iintVestedYears = this.aclbPersonWorkHistory_MPI.Last().vested_years_count;
                    idecVestedHours = (from item in this.aclbPersonWorkHistory_MPI select item.vested_hours).Sum();
                }
            }
        }

        public ArrayList btn_CalculateBenefitClick()
        {
            ArrayList iarrErrors = new ArrayList();

            //Call Eligibility Yet Again the Final Time Just Before doing Final Calculation

            this.LoadWorkHistoryandSetupPrerequisites_Disability();
            foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
            {
                busDisabiltyBenefitCalculation lbusDisabiltyBenefitCalculation = new busDisabiltyBenefitCalculation { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                if (lbusDisabiltyBenefitCalculation.ibusCalculation.IsNull())
                {
                    lbusDisabiltyBenefitCalculation.ibusCalculation = new busCalculation();
                }

                lbusDisabiltyBenefitCalculation.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusDisabiltyBenefitCalculation.ibusBenefitApplication = this;
                lbusDisabiltyBenefitCalculation.ibusPerson = this.ibusPerson;
                lbusDisabiltyBenefitCalculation.ibusPerson.iclbPersonAccount = this.ibusPerson.iclbPersonAccount;
                lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.age = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.retirement_date);
                lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
                lbusDisabiltyBenefitCalculation.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                lbusDisabiltyBenefitCalculation.ibusBenefitApplicationRetirement = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusDisabiltyBenefitCalculation.ibusBenefitApplicationRetirement = this;
                lbusDisabiltyBenefitCalculation.ibusBenefitApplicationRetirement.LoadWorkHistoryandSetupPrerequisites_Retirement();
                lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.istrRetirementType = lbusDisabiltyBenefitCalculation.ibusBenefitApplicationRetirement.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusBenefitApplicationDetail.iintPlan_ID).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                lbusDisabiltyBenefitCalculation.GetEligiblityForRegularBenefit();

                //Related to Disability Conversion Part
                if (this.iclbPayeeAccount.IsNullOrEmpty())
                {
                    GetPayeeAccountsInReceivingSatus();
                }

                if (this.iclbPayeeAccount.Count > 0)
                {
                    this.iblnDisabilityConversion = true;
                    if (this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == lbusBenefitApplicationDetail.iintPlan_ID).Count() > 0)
                    {
                        lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.payee_account_id =
                            this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == lbusBenefitApplicationDetail.iintPlan_ID).FirstOrDefault().icdoPayeeAccount.payee_account_id;
                    }
                }


                //lbusDisabiltyBenefitCalculation.LoadAllRetirementContributions();
                if (!ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                        lbusDisabiltyBenefitCalculation.LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                   
                }
                else
                {
                        lbusDisabiltyBenefitCalculation.LoadAllRetirementContributions(null);

                }

               
                if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                {
                    lbusDisabiltyBenefitCalculation.iblnCalculateIAPBenefit = true;
                }

                if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusDisabiltyBenefitCalculation.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusDisabiltyBenefitCalculation.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                {
                    lbusDisabiltyBenefitCalculation.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id,
                                            busConstant.BENEFIT_TYPE_DISABILITY, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.retirement_date,
                                            this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID);
                    lbusDisabiltyBenefitCalculation.SetUpDisabilityVariablesForCalculation(this.icdoBenefitApplication.retirement_date);
                    this.CheckAlreadyVested(lbusBenefitApplicationDetail.istrPlanCode);



                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                    }

                    lbusDisabiltyBenefitCalculation.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                     this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                     lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                     lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue, this.icdoBenefitApplication.retirement_date);
                }
                else
                {
                    if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                    {
                        lbusDisabiltyBenefitCalculation.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id,
                                           busConstant.BENEFIT_TYPE_DISABILITY, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.retirement_date,
                                           this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID);
                        lbusDisabiltyBenefitCalculation.SetUpDisabilityVariablesForCalculation(this.icdoBenefitApplication.retirement_date);
                        this.CheckAlreadyVested(lbusBenefitApplicationDetail.istrPlanCode);



                        if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                        }

                        lbusDisabiltyBenefitCalculation.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                         this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                         lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                         lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue, this.icdoBenefitApplication.retirement_date);
                    }

                 }

                try
                {
                    if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusDisabiltyBenefitCalculation.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusDisabiltyBenefitCalculation.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                    {
                        lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                        lbusDisabiltyBenefitCalculation.PersistChanges();
                        lbusDisabiltyBenefitCalculation.AfterPersistChanges();
                        SetWorkflowRelatedVariablesforFinalCalculation(lbusBenefitApplicationDetail.istrPlanCode, lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                    }
                    else
                    {
                        if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                        {
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                            lbusDisabiltyBenefitCalculation.PersistChanges();
                            lbusDisabiltyBenefitCalculation.AfterPersistChanges();
                            SetWorkflowRelatedVariablesforFinalCalculation(lbusBenefitApplicationDetail.istrPlanCode, lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.benefit_calculation_header_id);

                        }
                    }
                        
                }
                catch
                {
                }
            }

            if (this.ibusBaseActivityInstance.IsNotNull())
            {
                this.SetProcessInstanceParameters();
            }
            this.icdoBenefitApplication.final_calc_flag = busConstant.FLAG_YES;
            this.icdoBenefitApplication.change_benefit_option_flag = busConstant.FLAG_NO;
            this.icdoBenefitApplication.Update();
            iarrErrors.Add(this);

            return iarrErrors;
        }

        #endregion

        public override busBase GetCorPerson()
        {
            ibusPerson.LoadPersonAddresss();
            ibusPerson.LoadPersonContacts();
            ibusPerson.LoadCorrAddress();
            return this.ibusPerson;
        }

        # region overriden Methods
        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            LoadCorrespondenceProperties();

            #region DIS-0008
            if (astrTemplateName == busConstant.DISABILITY_CONVERSION_COVER_LETTER)
            {
                                       
                DateTime idtLastBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate(this.icdoBenefitApplication.iintPlanId);

                if (this.icdoBenefitApplication.iintPlanId != busConstant.IAP_PLAN_ID)
                    idtNextBenefitPaymentDate = idtLastBenefitPaymentDate.AddMonths(1);
                else
                    idtNextBenefitPaymentDate = busGlobalFunctions.GetPaymentDayForIAP(idtLastBenefitPaymentDate.AddDays(7));

                istrNextBenefitPaymentDate = busGlobalFunctions.ConvertDateIntoDifFormat(idtNextBenefitPaymentDate);
            }
            #endregion

            #region DIS-0013
            if (astrTemplateName == busConstant.IAP_DISABILITY_BENEFIT_APPLICATION)
            {
                DateTime currentDate = System.DateTime.Now;
                idtRetirementDate = this.icdoBenefitApplication.retirement_date;

                DateTime ldtNextMonth = currentDate.AddMonths(1);
                ldtNextMonth = ldtNextMonth.GetFirstDayofMonth();

                idtDayOneOfNextMonth = ldtNextMonth;
                istrDayOneOfNextMonth = busGlobalFunctions.ConvertDateIntoDifFormat(idtDayOneOfNextMonth);

                idueDate = idtDayOneOfNextMonth.AddDays(-1);
                istrDueDate = busGlobalFunctions.ConvertDateIntoDifFormat(idueDate);
            }
            #endregion
            #region DIS-0017
            //PIR-827
            //if (astrTemplateName == busConstant.PROOF_OF_SSA_CONTINUOUS_DISABILITY || astrTemplateName == busConstant.PROOF_OF_SSA_CONTINUOUS_DISABILITY_ALTERNATE_PAYEE)
            if (astrTemplateName == busConstant.PROOF_OF_SSA_CONTINUOUS_DISABILITY)
            {
                if (this.icdoBenefitApplication.dro_application_id == 0)
                {
                    istrIsParticipant = busConstant.YES;
                    aintIsParticipant = 1;
                    this.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };
                    this.ibusAlternatePayee.FindPerson(this.icdoBenefitApplication.person_id);
                    this.ibusAlternatePayee.LoadInitialData();
                    this.ibusAlternatePayee.LoadPersonAddresss();
                    this.ibusAlternatePayee.LoadPersonContacts();
                    this.ibusAlternatePayee.LoadCorrAddress();
                    if (Convert.ToInt32(this.ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.USA)
                    {
                        istrIsUSA = "1";
                    }
                }
                else
                {
                    istrIsParticipant = busConstant.NO;
                    aintIsParticipant = 0;
                    this.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };
                    this.ibusAlternatePayee.FindPerson(this.icdoBenefitApplication.alternate_payee_id);
                    this.ibusAlternatePayee.LoadInitialData();
                    this.ibusAlternatePayee.LoadPersonAddresss();
                    this.ibusAlternatePayee.LoadPersonContacts();
                    this.ibusAlternatePayee.LoadCorrAddress();
                    if (Convert.ToInt32(this.ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.USA)
                    {
                        istrIsUSA = "1";
                    }
                }
                
            }
            #endregion

            #region RETR-0031
            if (astrTemplateName == busConstant.RETIREMENT_APPLICATION_DISABILITY_CONVERSION_CORR 
                || astrTemplateName == busConstant.Disability_Application_Packet_Conversion)
            {
                if (icdoBenefitApplication.disability_onset_date != DateTime.MinValue)
                {
                    if (icdoBenefitApplication.disability_onset_date < icdoBenefitApplication.retirement_date)
                    {
                        iblnSetDateFlag = true;
                    }
                }

            }

            #endregion
        }
       

        //Code-Abhishek
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (this.icdoBenefitApplication.retirement_date != DateTime.MinValue)
            {
                this.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.retirement_date);
                
                if (this.iobjPassInfo.ienmPageMode==utlPageMode.Update && Convert.ToDateTime(this.icdoBenefitApplication.ihstOldValues[enmBenefitApplication.retirement_date.ToString()].ToString()) != this.icdoBenefitApplication.retirement_date)                
                    this.LoadandProcessWorkHistory_ForAllPlans();                
                else
                    this.LoadandProcessWorkHistory_ForAllPlans();                   

                SetupPrerequisites();
            }

            base.BeforeValidate(aenmPageMode);
        }
        //Code-Abhishek

            
        public override void BeforePersistChanges()
        {
            utlDataControl lobjSenderControl = utlThreadStatic.Instance.iobjFormXml?.GetDataControl(utlPassInfo.iobjPassInfo.istrSenderID);
            if (lobjSenderControl.istrMethodName == "btnSaveIgnoreReadOnly_Click")
            {
                this.icdoBenefitApplication.Select();
            }

            if (Eligible_Plans != null && Eligible_Plans.Count > 0 && this.iclbBenefitApplicationDetail != null && this.iclbBenefitApplicationDetail.Count > 0)
            {
                string lstrDisabilityType = busConstant.DISABILITY_TYPE_SSA;
                if (this.icdoBenefitApplication.terminally_ill_flag == busConstant.FLAG_YES)
                {
                    lstrDisabilityType = busConstant.DISABILITY_TYPE_TERMINAL;
                }
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in iclbBenefitApplicationDetail)
                {
                    if (this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).Count() > 0)
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value = lstrDisabilityType;
                        
                    }
                }
            }
            if (this.icdoBenefitApplication.retirement_date != DateTime.MinValue && (this.idecAge <= 0 || this.idecAge.IsNull()))
            {
                this.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.retirement_date);
            }
            //PIR-911 
            if (this.iblnDisabilityConversion || this.icdoBenefitApplication.disability_conversion_date != DateTime.MinValue)
            {
                Eligible_Plans.Remove("IAP");
            }
            base.BeforePersistChanges();
            this.icdoBenefitApplication.person_id = this.ibusPerson.icdoPerson.person_id;

            //Need Confirmation On the below Validation BR-018-12
            //if (this.iblnDisabilityConversion)
            //{
            //    int lintQDROApplications = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.CheckWaivedDisabilityBenefits",
            //        new object[2] { this.ibusPerson.icdoPerson.person_id, this.ibusEarlyRetirementApplication.icdoBenefitApplication.retirement_date},
            //            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);

            //    if (lintQDROApplications > 0)
            //        busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.QDRO_WORKFLOW_NAME, this.icdoBenefitApplication.person_id, 0, 0, null);
            //}


            //if (this.ibusBaseActivityInstance.IsNotNull())
            //{
            //    this.SetProcessInstanceParameters();
            //}
        }

        public override ArrayList btn_Approved()
        {
            ArrayList iarrError = new ArrayList();

            iarrError = base.btn_Approved();
            //PIR-911 
            //Rules : SSA disablity on set date is on or after retirement date under previous early retrirement application.
            if (this.iblnDisabilityConversion)
            {
                if (this.ibusEarlyRetirementApplication != null)
                {
                    if (this.icdoBenefitApplication.disability_onset_date >= this.ibusEarlyRetirementApplication.icdoBenefitApplication.retirement_date)
                    {
                        utlError lobjError = AddError(5485, "");
                        iarrError.Add(lobjError);
                        return iarrError;
                    }
                }
            }
            //PIR 954
            if (!(this.icdoBenefitApplication.disability_onset_date == DateTime.MinValue) && !(this.icdoBenefitApplication.entitlement_date == DateTime.MinValue) &&
                   !(this.icdoBenefitApplication.terminally_ill_flag == "Y") && this.CheckEntitlementDate())
            {
                utlError lobjError = AddError(5039, "");
                iarrError.Add(lobjError);
                return iarrError;
            }

            if (!(this.icdoBenefitApplication.disability_onset_date == DateTime.MinValue) && !(this.icdoBenefitApplication.terminally_ill_flag == "Y") &&
                this.CheckRtrmntDt6MnthGreaterThanDisOnstDt())
            {
                utlError lobjError = AddError(5037, "");
                iarrError.Add(lobjError);
                return iarrError;
            }

            if (this.iclbPayeeAccount.IsNullOrEmpty())
            {
                GetPayeeAccountsInReceivingSatus();
            }

            return iarrError;
        }


        private void SetupPrerequisites()
        {
            if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.DetermineVesting();                
                this.DetermineBenefitSubTypeandEligibility_Disability();
            }
        }


        public bool IsAppRcvdDateWithin2YearsPriorTOEarlyRetirementDate()
        {
            if(icdoBenefitApplication.dtEarlyRetirementDate != DateTime.MinValue)
            {                
                TimeSpan ltsYearDiff = icdoBenefitApplication.application_received_date - icdoBenefitApplication.dtEarlyRetirementDate;
                int lintDays = ltsYearDiff.Days;
                if (lintDays < 735)
                {
                    if ((DateTime.IsLeapYear(icdoBenefitApplication.application_received_date.Year) || DateTime.IsLeapYear(icdoBenefitApplication.dtEarlyRetirementDate.Year))
                        && lintDays > 731)
                    {
                        return false;
                    }
                    else if (lintDays > 730)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsSSDappDateGreaterThnEarlyRetrDate()
        {
            if (icdoBenefitApplication.dtEarlyRetirementDate != DateTime.MinValue)
            {
                if (icdoBenefitApplication.ssa_application_date > icdoBenefitApplication.dtEarlyRetirementDate)
                {
                    return true;
                }
            }
            return false;
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {                        
            utlError lobjError = null;

            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            utlDataControl lobjSenderControl = utlThreadStatic.Instance.iobjFormXml?.GetDataControl(utlPassInfo.iobjPassInfo.istrSenderID);
            if (lobjSenderControl.istrMethodName == "btnSaveIgnoreReadOnly_Click")
            {
                return;
            }

            if (icdoBenefitApplication.retirement_date == DateTime.MinValue)
            {
                lobjError = AddError(5027,"");
                this.iarrErrors.Add(lobjError);
            }

            base.ValidateHardErrors(aenmPageMode);


            //if (icdoBenefitApplication.retirement_date != DateTime.MinValue && icdoBenefitApplication.retirement_date < DateTime.Today)
            //{
            //    if (iobjPassInfo.ienmPageMode == utlPageMode.New)
            //    {
            //        lobjError = AddError(5028, " ");
            //        this.iarrErrors.Add(lobjError);
            //    }
            //    else if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            //    {
            //        if (icdoBenefitApplication.retirement_date != Convert.ToDateTime(icdoBenefitApplication.ihstOldValues[enmBenefitApplication.retirement_date.ToString()]))
            //        {
            //            lobjError = AddError(5028, " ");
            //            this.iarrErrors.Add(lobjError);
            //        }
            //    }
            //}
         


            Hashtable lhstParams = new Hashtable();
            lhstParams["icdoBenefitApplication.awarded_on_date"] = icdoBenefitApplication.awarded_on_date;
            lhstParams["icdoBenefitApplication.disability_onset_date"] = icdoBenefitApplication.disability_onset_date;
            lhstParams["icdoBenefitApplication.ssa_application_date"] = icdoBenefitApplication.ssa_application_date;
            lhstParams["icdoBenefitApplication.retirement_date"] = icdoBenefitApplication.retirement_date;
            lhstParams["icdoBenefitApplication.terminally_ill_flag"] = icdoBenefitApplication.terminally_ill_flag;
           lhstParams["iintPlan_ID"]=0;

            this.iblnDisabilityConversion = busConstant.BOOL_FALSE;
            CheckAndValidateForDisabilityConversion();


            foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
            {               
                lhstParams["iintPlan_ID"] = lbusBenefitApplicationDetail.iintPlan_ID;
                lhstParams["icdoBenefitApplicationDetail.plan_benefit_id"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id;
                lhstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintJointAnnuaintID;
                lhstParams["icdoBenefitApplicationDetail.spousal_consent_flag"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.spousal_consent_flag;                
                               
                checkMarriage(this, lhstParams, ref iarrErrors);
                CheckDatesOnBenefitAppDetails(this, lhstParams, ref iarrErrors);
                this.iarrErrors = CheckErrorOnAddButton(this, lhstParams, ref iarrErrors, true,ablnDisabilityConversion:iblnDisabilityConversion);
            }

            #region Disability Conversion 

            if(icdoBenefitApplication.disability_conversion_date != DateTime.MinValue)
            {
                int lintLateRetirement = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.CheckIfApprovedLateRTMTAppl", new object[1] { icdoBenefitApplication.person_id},
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if (lintLateRetirement > 0)
                {
                    lobjError = AddError(6200, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }
            }


            if (this.iblnDisabilityConversion)
            {
                
                if (icdoBenefitApplication.disability_conversion_date == DateTime.MinValue)
                {
                    lobjError = AddError(5483, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }

                if (this.ibusBaseActivityInstance.IsNotNull())
                {
                    int lintProcessInstanceChecklist = (int)DBFunction.DBExecuteScalar("cdoProcessInstanceChecklist.GetProcessInstanceChecklistData", new object[2] { 
                        ((MPIPHP.BusinessObjects.busProcessInstanceGen)(((MPIPHP.BusinessObjects.busActivityInstanceGen)(ibusBaseActivityInstance)).ibusProcessInstance)).icdoProcessInstance.process_instance_id,
                        busConstant.RETIREMENT_APPLICATION_DISABILITY_CONVERSION},
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    if (lintProcessInstanceChecklist <= 0)
                    {
                        lobjError = AddError(6056, "");
                        this.iarrErrors.Add(lobjError);
                        return;
                    }
                }

                if (this.ibusEarlyRetirementApplication.icdoBenefitApplication.retirement_date != DateTime.MinValue)
                {
                    if (this.icdoBenefitApplication.ssa_application_date > this.ibusEarlyRetirementApplication.icdoBenefitApplication.retirement_date)
                    {
                        if (this.icdoBenefitApplication.ssa_application_date > this.ibusEarlyRetirementApplication.icdoBenefitApplication.retirement_date.AddYears(2))
                        {
                            lobjError = AddError(5484, "");
                            this.iarrErrors.Add(lobjError);

                        }
                    }
                    //else
                    //{
                    //    lobjError = AddError(5487, "");
                    //    this.iarrErrors.Add(lobjError);

                    //}
                    //PIR-911
                    if (this.icdoBenefitApplication.ssa_application_date == DateTime.MinValue)
                    {
                        lobjError = AddError(5122, "");
                        this.iarrErrors.Add(lobjError);
                        return;
                    }
                    //PIR-911 
                    //Rules : SSA disablity on set date is on or after retirement date under previous early retrirement application. 
                    if (this.icdoBenefitApplication.disability_onset_date >= this.ibusEarlyRetirementApplication.icdoBenefitApplication.retirement_date)
                    {
                        lobjError = AddError(5485, "");
                        this.iarrErrors.Add(lobjError);
                    }
                  
                    if (this.icdoBenefitApplication.ssa_application_date != DateTime.MinValue && icdoBenefitApplication.ssa_application_date > this.ibusEarlyRetirementApplication.icdoBenefitApplication.retirement_date)
                    {

                        if (this.icdoBenefitApplication.ssa_application_date > this.ibusEarlyRetirementApplication.icdoBenefitApplication.retirement_date.AddYears(2))
                        {
                            lobjError = AddError(5486, "");
                            this.iarrErrors.Add(lobjError);
                            return;
                        }
                    }
                    //else
                    //{
                    //    if (this.icdoBenefitApplication.ssa_application_date != DateTime.MinValue)
                    //    {
                    //        lobjError = AddError(5488, "");
                    //        this.iarrErrors.Add(lobjError);
                    //        return;
                    //    }
                    //}
                }

                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    foreach (busPayeeAccount lbusPayeeAccount in this.iclbPayeeAccount)
                    {
                        lbusPayeeAccount.LoadBenefitDetails();
                        //if (lbusBenefitApplicationDetail.iintPlan_ID == lbusPayeeAccount.icdoPayeeAccount.iintPlanId && lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID)
                        //{
                        //    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id != lbusPayeeAccount.icdoPayeeAccount.plan_benefit_id)
                        //    {
                        //        lobjError = AddError(5489, "");
                        //        this.iarrErrors.Add(lobjError);
                        //        return;
                        //    }
                        //}
                        if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID)
                        {
                            if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.MPIPP_PLAN_ID)
                            {
                                Collection<cdoPlanBenefitXr> lclbPlanBenefitXr = new Collection<cdoPlanBenefitXr>();
                                lclbPlanBenefitXr = GetBenefitOptionsforPlan(busConstant.MPIPP_PLAN_ID);

                                if (lclbPlanBenefitXr != null && lclbPlanBenefitXr.Count > 0 &&
                                    lclbPlanBenefitXr.Where(item => item.benefit_option_value == lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue).Count() > 0)
                                {
                                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue != lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue)
                                    {
                                        lobjError = AddError(5489, "");
                                        this.iarrErrors.Add(lobjError);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }


                ////BR-018-12
                DataTable ldtblQDROApplications = Select("cdoBenefitApplication.GetSeparateQDROInReceivingStatus_Disability",
                    new object[1] { this.ibusPerson.icdoPerson.person_id});

                if (ldtblQDROApplications != null && ldtblQDROApplications.Rows.Count > 0)
                {
                   
                            busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.QDRO_WORKFLOW_NAME, this.icdoBenefitApplication.person_id, 0,
                                0, null);
                      
                }

                if (this.iclbBenefitApplicationDetail.Count > 0 && this.iclbBenefitApplicationDetail.Where(item => item.iintPlan_ID == busConstant.IAP_PLAN_ID).Count() > 0)
                {
                    lobjError = AddError(5494, "");
                    this.iarrErrors.Add(lobjError);
                    return;
                }

            }
            #endregion Disability Conversion


            if (this.NotEligible.IsNotNull() && this.NotEligible)
            {
                lobjError = AddError(5434, " ");
                this.iarrErrors.Add(lobjError);
            }

            if (!this.iblnDisabilityConversion && Eligible_Plans.IsNotNull() && Eligible_Plans.Contains(busConstant.MPIPP) && Eligible_Plans.Contains(busConstant.IAP) && this.iclbBenefitApplicationDetail.Count > 0)
            {
                int count = 0;
                foreach (busBenefitApplicationDetail item in this.iclbBenefitApplicationDetail)
                {
                    if (item.iintPlan_ID == busConstant.IAP_PLAN_ID || item.iintPlan_ID == busConstant.MPIPP_PLAN_ID) //Plan ID is Hard-Coded here do not have any option
                    {
                        count++;
                    }
                }
                //151767
                if (count == 1 && !IsOnlyOnePlanAllowed())
                {
                    lobjError = AddError(5133, "");
                    this.iarrErrors.Add(lobjError);
                }
            }

            if (!this.iblnDisabilityConversion && this.iclbBenefitApplicationDetail.Count > 0 && this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP).Count() > 0 && this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP).Count() > 0)
            {
                if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP).First().icdoBenefitApplicationDetail.istrBenefitOptionValue != this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP).First().icdoBenefitApplicationDetail.istrBenefitOptionValue)
                {
                    if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP).First().icdoBenefitApplicationDetail.istrBenefitOptionValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY && this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP).First().icdoBenefitApplicationDetail.istrBenefitOptionValue == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                    {

                    }
                    else if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP).First().icdoBenefitApplicationDetail.istrBenefitOptionValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY && this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP).First().icdoBenefitApplicationDetail.istrBenefitOptionValue == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY)
                    {

                    }
                    else
                    {
                        if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP).First().icdoBenefitApplicationDetail.istrBenefitOptionValue != busConstant.LUMP_SUM)
                        {
                            lobjError = AddError(5163, "");
                            this.iarrErrors.Add(lobjError);
                        }
                    }

                }
            }

            if (Convert.ToInt32(lhstParams["iintPlan_ID"]) == 0 && Convert.ToChar(lhstParams["icdoBenefitApplication.terminally_ill_flag"]) != 'Y')
            {
                CheckAwardedOnDateOnSave(this, lhstParams, ref iarrErrors);
            }
        }

        public bool CheckRtrmntDt6MnthGreaterThanDisOnstDt()
        {
            if ((icdoBenefitApplication.retirement_date) <= (icdoBenefitApplication.disability_onset_date))
            {
                return true;
            }
            else
            {
                int lintMonths = ((icdoBenefitApplication.retirement_date.Year - icdoBenefitApplication.disability_onset_date.Year) * 12) + (icdoBenefitApplication.retirement_date.Month - icdoBenefitApplication.disability_onset_date.Month);

                if (lintMonths == 6)
                {
                    if (icdoBenefitApplication.retirement_date.Day < icdoBenefitApplication.disability_onset_date.Day)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (lintMonths < 6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CheckEntitlementDate()
        {
            if (icdoBenefitApplication.entitlement_date <= icdoBenefitApplication.disability_onset_date)
            {
                return true;
            }
            else
            {
                int lintMonths = ((icdoBenefitApplication.entitlement_date.Year - icdoBenefitApplication.disability_onset_date.Year) * 12) + (icdoBenefitApplication.entitlement_date.Month - icdoBenefitApplication.disability_onset_date.Month);

                if (lintMonths == 5)
                {
                    if (icdoBenefitApplication.entitlement_date.Day < icdoBenefitApplication.disability_onset_date.Day)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (lintMonths < 5)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        public void GetPayeeAccountsInReceivingSatus()
        {
            DataTable ldtblPayeeAccounts = null;
            if (this.icdoBenefitApplication.application_status_value != busConstant.BENEFIT_APPL_APPROVED)
            {
                //Loading Payee Accounts Not in cancelled or compl status
                ldtblPayeeAccounts = busBase.Select("cdoBenefitApplication.GetPayeeAccountinReceivedStatus", new object[1] { this.icdoBenefitApplication.person_id });

                if (icdoBenefitApplication.disability_conversion_date != DateTime.MinValue) //Bug 02162016
                {
                    ldtblPayeeAccounts = busBase.Select("cdoBenefitApplication.GetEarlyRetirementPayeeInComp", new object[1] { this.icdoBenefitApplication.person_id });
                }
            }
            else
            {
                //Loading Payee Accounts Not in  compl status
                ldtblPayeeAccounts = busBase.Select("cdoBenefitApplication.GetEarlyRetirementPayeeInComp", new object[1] { this.icdoBenefitApplication.person_id });
            }
            if (ldtblPayeeAccounts.IsNotNull() && ldtblPayeeAccounts.Rows.Count > 0)
            {
                iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtblPayeeAccounts, "icdoPayeeAccount");
            }
        }


        public void CheckAndValidateForDisabilityConversion()
        {
            utlError lobjError = null;
            bool lblnLoadPayeeAccount = false;
            if (this.icdoBenefitApplication.terminally_ill_flag != busConstant.FLAG_YES)
            {

                if (this.iclbBenefitApplicationDetail.Count == 1 && this.iclbPayeeAccount.Count > 0)
                {
                    if (this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == this.iclbBenefitApplicationDetail.First().iintPlan_ID).Count() > 0)
                    {
                        lblnLoadPayeeAccount = true;
                    }
                }
                else if (this.iclbBenefitApplicationDetail.Count > 1 && this.iclbPayeeAccount.Count > 0)
                {
                    Collection<busPayeeAccount> lclbbusPayeeAccount = new Collection<busPayeeAccount>();

                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                    {
                        foreach (busPayeeAccount lbusPayeeAccount in this.iclbPayeeAccount)
                        {
                            if (lbusBenefitApplicationDetail.iintPlan_ID == lbusPayeeAccount.icdoPayeeAccount.iintPlanId)
                            {
                                lclbbusPayeeAccount.Add(lbusPayeeAccount);
                                break;
                            }
                        }
                    }

                    if (lclbbusPayeeAccount.Count > 0)
                    {
                        foreach (busPayeeAccount lbusPayeeAccount in lclbbusPayeeAccount)
                        {
                            lbusPayeeAccount.LoadBenefitDetails();
                        }

                        if (lclbbusPayeeAccount.Where(item => item.icdoPayeeAccount.idtRetireMentDate == lclbbusPayeeAccount.First().icdoPayeeAccount.idtRetireMentDate).Count() == lclbbusPayeeAccount.Count)
                        {
                            lblnLoadPayeeAccount = true;
                        }
                        else
                        {
                            lobjError = AddError(5493, "");
                            this.iarrErrors.Add(lobjError);
                        }
                    }
                }

            }


            if (lblnLoadPayeeAccount)
            {
                if (this.iclbBenefitApplicationDetail.Count > 1)
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                    {
                        bool lblnFlag = false;
                        foreach (busPayeeAccount lbusPayeeAccount in this.iclbPayeeAccount)
                        {
                            if (lbusBenefitApplicationDetail.iintPlan_ID == lbusPayeeAccount.icdoPayeeAccount.iintPlanId)
                            {
                                lblnFlag = true;
                            }
                        }
                        if (!lblnFlag)
                        {
                            //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
                            //DataTable ldtbMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5509);
                            //string lstrMessage = ldtbMessageInfo.Rows[0]["display_message"].ToString();
                            utlMessageInfo lobjutlMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5509);
                            string lstrMessage = lobjutlMessageInfo.display_message;
                            lobjError = AddError(0, string.Format(lstrMessage, lbusBenefitApplicationDetail.istrPlanCode));
                            this.iarrErrors.Add(lobjError);
                            break;
                        }
                    }
                }
                //Get Payee Account in Received Status
                if (this.iclbBenefitApplicationDetail.First() != null)
                {
                    if (this.iclbPayeeAccount.Count > 0)
                    {
                        iblnDisabilityConversion = true;
                        this.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                        int lBenefitApplicationDetailId = this.iclbPayeeAccount.First().icdoPayeeAccount.benefit_application_detail_id;
                        this.ibusEarlyRetirementApplication = new busRetirementApplication();
                        busBenefitApplicationDetail lbusBenefitApplicationDetail = new busBenefitApplicationDetail();
                        if (lbusBenefitApplicationDetail.FindBenefitApplicationDetail(lBenefitApplicationDetailId))
                        {
                            this.ibusEarlyRetirementApplication.FindBenefitApplication(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_id);
                            this.ibusEarlyRetirementApplication.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();
                            this.ibusEarlyRetirementApplication.iclbBenefitApplicationDetail.Add(lbusBenefitApplicationDetail);
                        }
                    }
                }
            }

        }

        public void LoadForDisabilityConversion()
        {
            bool lblnLoadPayeeAccount = false;
            if (this.icdoBenefitApplication.terminally_ill_flag != busConstant.FLAG_YES)
            {

                if (this.iclbBenefitApplicationDetail.Count == 1 && this.iclbPayeeAccount.Count > 0)
                {
                    if (this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == this.iclbBenefitApplicationDetail.First().iintPlan_ID).Count() > 0)
                    {
                        lblnLoadPayeeAccount = true;
                    }
                }
                else if (this.iclbBenefitApplicationDetail.Count > 1 && this.iclbPayeeAccount.Count > 0)
                {
                    Collection<busPayeeAccount> lclbbusPayeeAccount = new Collection<busPayeeAccount>();

                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                    {
                        foreach (busPayeeAccount lbusPayeeAccount in this.iclbPayeeAccount)
                        {
                            if (lbusBenefitApplicationDetail.iintPlan_ID == lbusPayeeAccount.icdoPayeeAccount.iintPlanId)
                            {
                                lclbbusPayeeAccount.Add(lbusPayeeAccount);
                                break;
                            }
                        }
                    }

                    if (lclbbusPayeeAccount.Count > 0)
                    {
                        foreach (busPayeeAccount lbusPayeeAccount in lclbbusPayeeAccount)
                        {
                            lbusPayeeAccount.LoadBenefitDetails();
                        }

                        if (lclbbusPayeeAccount.Where(item => item.icdoPayeeAccount.idtRetireMentDate == lclbbusPayeeAccount.First().icdoPayeeAccount.idtRetireMentDate).Count() == lclbbusPayeeAccount.Count)
                        {
                            lblnLoadPayeeAccount = true;
                        }
                    }
                }

            }


            if (lblnLoadPayeeAccount)
            {
                if (this.iclbBenefitApplicationDetail.Count > 1)
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                    {
                        bool lblnFlag = false;
                        foreach (busPayeeAccount lbusPayeeAccount in this.iclbPayeeAccount)
                        {
                            if (lbusBenefitApplicationDetail.iintPlan_ID == lbusPayeeAccount.icdoPayeeAccount.iintPlanId)
                            {
                                lblnFlag = true;
                            }
                        }
                    }
                }
                //Get Payee Account in Received Status
                if (this.iclbBenefitApplicationDetail.First() != null)
                {
                    if (this.iclbPayeeAccount.Count > 0)
                    {
                        iblnDisabilityConversion = true;
                        this.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                        int lBenefitApplicationDetailId = this.iclbPayeeAccount.First().icdoPayeeAccount.benefit_application_detail_id;
                        this.ibusEarlyRetirementApplication = new busRetirementApplication();
                        busBenefitApplicationDetail lbusBenefitApplicationDetail = new busBenefitApplicationDetail();
                        if (lbusBenefitApplicationDetail.FindBenefitApplicationDetail(lBenefitApplicationDetailId))
                        {
                            this.ibusEarlyRetirementApplication.FindBenefitApplication(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_id);
                            this.ibusEarlyRetirementApplication.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();
                            this.ibusEarlyRetirementApplication.iclbBenefitApplicationDetail.Add(lbusBenefitApplicationDetail);
                        }
                    }
                }
            }

        }

        //151767
        public bool IsOnlyOnePlanAllowed()
        {
            if (icdoBenefitApplication.retirement_date >= busConstant.BenefitCalculation.DISABILITY_IAP_ONLY_EFFECTIVE_DATE)
            {
                return true;
            }
            return false;
        }
        # endregion


        public int aintIsParticipant { get; set; }
    }
}

       
