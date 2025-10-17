#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.DataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.Bpm;


#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busRetirementApplication:
    /// Inherited from busRetirementApplicationGen, the class is used to customize the business object busRetirementApplicationGen.
    /// </summary>
    [Serializable]
    public class busRetirementApplication : busBenefitApplication
    {
        //CODE - COMMENTED BY AARTI PROPERTIES SHIFTED TO busBenefitApplication

        public int iintBPMPlanId { get; set; }

        public string istrSpouseNameMarraige { get; set; }
        public DateTime istrSpouseNameDeath { get; set; }

        bool iblnFlag { get; set; } //PIR-799 Rohan

        #region Public Methods

        public bool IsBPMSaveButtonVisible()
        {
            if (ibusBaseActivityInstance != null && this.icdoBenefitApplication.application_status_value==busConstant.BENEFIT_APPL_APPROVED)
            {
                return true;
            }
            return false;
        }

        public bool IsSubmitButtonVisible()
        {
            if (ibusBaseActivityInstance.IsNotNull())
            {
                busBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busBpmActivityInstance;
                if (lbusBpmActivityInstance.IsNotNull())
                {
                    if (lbusBpmActivityInstance.ibusBpmProcessInstance.IsNotNull() && lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.IsNotNull()
                        && lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.ibusBpmCase.icdoBpmCase.name == busConstant.PersonAccountMaintenance.CANCEL_SERVICE_RETIREMENT_BPM)
                    {
                        if (lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.CANCEL_APPLICATION)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    if (lbusBpmActivityInstance.ibusBpmActivity.IsNotNull() && lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.AUDIT_PAYEE_ACCOUNT)
                    {
                        return false;
                    }

                    return true;
                }
            }
            return false;
        }
        public bool IsBPMIAPWaitTimerVisible()
        {
            if (ibusBaseActivityInstance != null)
            {
                busSolBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busSolBpmActivityInstance;
                if (lbusBpmActivityInstance != null)
                {
                    if (lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.ibusBpmCase.icdoBpmCase.name == busConstant.PersonAccountMaintenance.APPLICATION_SERVICE_RETIREMENT_BPM)
                    {
                        iintBPMPlanId = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue("PlanId"));
                        if (iintBPMPlanId == busConstant.IAP_PLAN_ID)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool ISDivorcesExSpouseName1()
        {
            if (this.ibusBenefitApplicationChecklist.IsNotNull() &&  this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.IsNotNull() && this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.no_of_divorces_ex_spouse_name1.IsNotNullOrEmpty())
            {
                return true;
            }
            return false;
        }
        public bool ISDivorcesExSpouseName2()
        {
            if (this.ibusBenefitApplicationChecklist.IsNotNull() &&  this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.IsNotNull() && this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.no_of_divorces_ex_spouse_name2.IsNotNullOrEmpty())
            {
                return true;
            }
            return false;
        }
        public bool ISDivorcesExSpouseName3()
        {
            if (this.ibusBenefitApplicationChecklist.IsNotNull() && this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.IsNotNull() && this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.no_of_divorces_ex_spouse_name3.IsNotNullOrEmpty())
            {
                return true;
            }
            return false;
        }
        public bool ISDivorcesExSpouseName4()
        {
            if (this.ibusBenefitApplicationChecklist.IsNotNull() &&  this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.IsNotNull() && this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.no_of_divorces_ex_spouse_name4.IsNotNullOrEmpty())
            {
                return true;
            }
            return false;
        }
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
                DataTable ldtbListofPLans = DBFunction.DBSelect(lstrFinalQuery, iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);
                if (ldtbListofPLans.Rows.Count > 0)
                {
                    lColPlans = Sagitec.DataObjects.doBase.GetCollection<cdoPlan>(ldtbListofPLans);
                }
            }

            return lColPlans;
        }

        public Collection<cdoPlan> GetPlanValuesforCorrespondence()
        {
            Collection<cdoPlan> lColPlans = null;
            DataTable ldtbList = busMPIPHPBase.Select("cdoPersonAccount.GetPlanfromPersonIDforCorrespondence", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtbList.Rows.Count > 0)
            {
                lColPlans = Sagitec.DataObjects.doBase.GetCollection<cdoPlan>(ldtbList);
            }
            return lColPlans;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            GetPriorDates();
            DateTime ldtCurrentDate = System.DateTime.Now;
            istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
            //PIR 1002
            this.istrRetirementDate = busGlobalFunctions.ConvertDateIntoDifFormat(icdoBenefitApplication.retirement_date);

            if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES && this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES)
            {
                if (this.iclbBenefitApplicationDetail != null && this.iclbBenefitApplicationDetail.Count > 0)
                {
                    if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP).Count() > 0)
                    {
                        istrRetirementType = this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.IAP).First().icdoBenefitApplicationDetail.benefit_subtype_description;
                    }
                }
                if (this.aclbPersonWorkHistory_IAP != null && this.aclbPersonWorkHistory_IAP.Count > 0)
                {
                    iintVestedYears = this.aclbPersonWorkHistory_IAP.Last().vested_years_count;
                    idecVestedHours = (from item in this.aclbPersonWorkHistory_IAP select item.vested_hours).Sum();
                }
            }
            else if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES)
            {
                if (this.iclbBenefitApplicationDetail != null && this.iclbBenefitApplicationDetail.Count > 0)
                {
                    if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP).Count() > 0)
                    {
                        istrRetirementType = this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP).First().icdoBenefitApplicationDetail.benefit_subtype_description;
                    }
                }
                if (this.aclbPersonWorkHistory_MPI != null && this.aclbPersonWorkHistory_MPI.Count > 0)
                {
                    iintVestedYears = this.aclbPersonWorkHistory_MPI.Last().vested_years_count;
                    idecVestedHours = (from item in this.aclbPersonWorkHistory_MPI select item.vested_hours).Sum();
                }
            }
            //TICKET#69388
            #region RETR-0041 & RETR-0052
            if (astrTemplateName == busConstant.CANCELLATION_NOTIFICATION || astrTemplateName == busConstant.RETIREMENT_CANCELLATION_FORM)
            {
                if (icdoBenefitApplication.retirement_date != DateTime.MinValue)
                {
                    idtRetirementDate = icdoBenefitApplication.retirement_date;
                }
            }
            #endregion
            if (icdoBenefitApplication.retirement_date != DateTime.MinValue)
            {
                DateTime lRetirementDt = icdoBenefitApplication.retirement_date;
                DateTime lRetrDateTemp = lRetirementDt.AddMonths(-2);
                istrPriorToRetirement = Convert.ToString(lRetrDateTemp.AddDays(-1));
                istrPriorToRetirement = String.Format("{0:MMMM dd, yyyy}", lRetrDateTemp.AddDays(-1));
                istrRetirementDt = String.Format("{0:MMMM dd, yyyy}", icdoBenefitApplication.retirement_date);
                // PIR 584
                istrDtRetirementDt = icdoBenefitApplication.retirement_date;

                if (icdoBenefitApplication.disability_onset_date != DateTime.MinValue)
                {
                    if (icdoBenefitApplication.disability_onset_date < icdoBenefitApplication.retirement_date)
                    {
                        iblnSetDateFlag = true;
                    }
                }

            }
            if (icdoBenefitApplication.disability_conversion_date != DateTime.MinValue)
            {
                iblnDisConvDate = true;
            }

            #region RETR-0009 , RETR-0020
            if (astrTemplateName == busConstant.RETIREMENT_COUNSELING_SUMMARY_OF_PENDING_ITEMS || astrTemplateName == busConstant.MISSING_DOCUMENT_REQUEST)
            {
                int lintProcessID = busConstant.ZERO_INT;
                if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
                {
                    lintProcessID = busConstant.RETIREMENT_WORKFLOW_PROCESS_ID;
                    istrBenefitTypeDescription = busConstant.BENEFIT_TYPE_RETIREMENT_DESC;
                }
                else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                {
                    lintProcessID = busConstant.DISABILITY_WORKFLOW_PROCESS_ID;
                    istrBenefitTypeDescription = busConstant.BENEFIT_TYPE_DISABILITY_DESC;

                }
                else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                {
                    lintProcessID = busConstant.WITHDRAWAL_WORKFLOW_PROCESS_ID;
                    istrBenefitTypeDescription = busConstant.BENEFIT_TYPE_WITHDRAWAL_DESC;
                }
                else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                {
                    lintProcessID = busConstant.PRERETIREMENT_DEATH_WORKFLOW_PROCESS_ID;
                    istrBenefitTypeDescription = busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT_DESC;
                }
                else if (iobjPassInfo.istrFormName == busConstant.PAYEE_ACCOUNT_MAINTENANCE)
                {
                    lintProcessID = busConstant.Payee_Account_WORKFLOW_PROCESS_ID;
                    istrBenefitTypeDescription = busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT_DESC;
                }

                iclbDocumentsReceived = new Collection<busBenefitApplication>();
                DataTable ldtDocumentsReceived = busBase.Select("cdoBenefitApplication.GetDocumentsReceivedList", new object[2] { this.ibusPerson.icdoPerson.person_id, lintProcessID });
                if (ldtDocumentsReceived.Rows.Count > 0 && ldtDocumentsReceived.Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow ldrDocumentReceived in ldtDocumentsReceived.AsEnumerable())
                    {
                        busBenefitApplication lobjbusBenefitApplicationDocRecieved = new busBenefitApplication() { icdoBenefitApplication = new cdoBenefitApplication() };

                        lobjbusBenefitApplicationDocRecieved.icdoBenefitApplication.istrDocument = ldrDocumentReceived["DOCUMENT_NAME"].ToString();

                        iclbDocumentsReceived.Add(lobjbusBenefitApplicationDocRecieved);
                    }
                }

                iclbDocumentsPending = new Collection<busBenefitApplication>();
                DataTable ldtDocumentsPending = busBase.Select("cdoBenefitApplication.GetDocumentsPendingList", new object[2] { this.ibusPerson.icdoPerson.person_id, lintProcessID });
                if (ldtDocumentsPending.Rows.Count > 0 && ldtDocumentsPending.Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow ldrDocumentPending in ldtDocumentsPending.AsEnumerable())
                    {
                        busBenefitApplication lobjbusBenefitApplicationDocPending = new busBenefitApplication() { icdoBenefitApplication = new cdoBenefitApplication() };

                        lobjbusBenefitApplicationDocPending.icdoBenefitApplication.istrDocument = ldrDocumentPending["DOCUMENT_NAME"].ToString();

                        iclbDocumentsPending.Add(lobjbusBenefitApplicationDocPending);
                    }
                }
            }
            #endregion
            //PIR 1002
            //RequestID: 64733
            #region Payee-0011
            if ((astrTemplateName == busConstant.RETIREMENT_AFFIDAVIT_COVER_LETTER ||
                astrTemplateName == busConstant.RETIREMENT_AFFIDAVIT) && (iobjPassInfo.istrFormName == busConstant.BENEFIT_CALCULATION_RETIREMENT_MAINTENANCE || iobjPassInfo.istrFormName == busConstant.RETIREMENT_APPLICATION_MAINTAINENCE))
            {                
                DataTable ldtBenefitApplicationData = busBase.Select("cdoPayeeAccount.GetPayeeAccountForRetirementAffidavit", new object[1] {icdoBenefitApplication.person_id });
                if (ldtBenefitApplicationData.IsNotNull() && ldtBenefitApplicationData.Rows.Count > 0)
                {
                    icdoBenefitApplication.LoadData(ldtBenefitApplicationData.Rows[0]);
                    RetirementAffidavitCoverLetter();
                }
            }
            #endregion       
            
        }
        //PIR 1002
        public void RetirementAffidavitCoverLetter()
        {

            if (ibusParticipant == null)
            {
                ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                ibusParticipant.FindPerson(icdoBenefitApplication.person_id);
            }

            if (ibusPerson == null)
            {
                ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                ibusPerson.FindPerson(icdoBenefitApplication.person_id);
            }

            iblnIAPFactor = false;
            busIapAllocationFactor lbusIapAllocationFactor = new busIapAllocationFactor();
            int lintPreviousQuarter = busGlobalFunctions.GetPreviousQuarter(icdoBenefitApplication.retirement_date);

            if (lintPreviousQuarter == 0)
            {
                lbusIapAllocationFactor.LoadIAPAllocationFactorByPlanYear(icdoBenefitApplication.retirement_date.Year - 1);
                if (lbusIapAllocationFactor != null && Convert.ToString(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf4_factor).IsNotNullOrEmpty()
                    && Convert.ToDecimal(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf4_factor) != 0)
                {
                    iblnIAPFactor = true;
                }

            }
            else
            {
                lbusIapAllocationFactor.LoadIAPAllocationFactorByPlanYear(icdoBenefitApplication.retirement_date.Year);
                if (lbusIapAllocationFactor != null)
                {
                    if (lintPreviousQuarter == 1 && Convert.ToString(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf1_factor).IsNotNullOrEmpty()
                    && Convert.ToDecimal(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf1_factor) != 0)
                    {
                        iblnIAPFactor = true;
                    }
                    else if (lintPreviousQuarter == 2 && Convert.ToString(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf2_factor).IsNotNullOrEmpty()
                    && Convert.ToDecimal(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf2_factor) != 0)
                    {
                        iblnIAPFactor = true;
                    }
                    else if (lintPreviousQuarter == 3 && Convert.ToString(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf3_factor).IsNotNullOrEmpty()
                   && Convert.ToDecimal(lbusIapAllocationFactor.icdoIapAllocationFactor.alloc1_qf3_factor) != 0)
                    {
                        iblnIAPFactor = true;
                    }
                }
            }
        }

        public void GetPriorDates()
        {
            if (this.icdoBenefitApplication.retirement_date != DateTime.MinValue)
            {
                DateTime ldtRetire = this.icdoBenefitApplication.retirement_date;
                DateTime ldtRtrDt = ldtRetire.AddMonths(-2);
                DateTime ldtOneDayPriorRtDate = ldtRetire.AddDays(-1);
                ldtRtrDt = ldtRtrDt.AddDays(-1);
                istrSixtyDaysPriorDate = Convert.ToString(ldtRtrDt);
                istrSixtyDaysPriorDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtRtrDt);

                istrOneDayPriorRtDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtOneDayPriorRtDate);


                DateTime ldtRetire1 = this.icdoBenefitApplication.retirement_date;
                DateTime ldtRtrDt1 = ldtRetire1.AddMonths(-1);
                ldtRtrDt1 = ldtRtrDt1.AddDays(-1);
                istrThirtyDaysPriorDate = Convert.ToString(ldtRtrDt1);
                istrThirtyDaysPriorDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtRtrDt1);

                dtSixtyDaysPriorDate = ldtRetire.AddDays(-60);
                dtThirtyDaysPriorDate = ldtRetire.AddDays(-30);
            }
        }


        public int GetDateDifference()
        {
            TimeSpan ltsDifference = icdoBenefitApplication.retirement_date.Subtract(icdoBenefitApplication.application_received_date);
            return ltsDifference.Days;
        }

        public void btn_ConvertMinDistribution()
        {
            //icdoBenefitApplication.benefit_subtype_value = busConstant.RETIREMENT_TYPE_LATE;
            //icdoBenefitApplication.Update();
        }

        public void CheckDateOfMarriage(busRetirementApplication abusRetirementApplication, Hashtable ahstParams, ref ArrayList larrErrors)
        {
            utlError lobjError = null;
            string lstrBenefitOption = string.Empty;
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
            //if ((this.ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_MARRIED) && (this.CheckAllBenefitOptions(lstrBenefitOption)))
            //{
            //    DateTime lDateOfMarriage = DateTime.MinValue;
            //    DataTable ldtDateofMarr = Select("cdoRelationship.GetDateOfMarriage", new object[2] { this.ibusPerson.icdoPerson.person_id, lintJointAnnID });
            //    if (ldtDateofMarr.Rows.Count > 0  && !string.IsNullOrEmpty(Convert.ToString(ldtDateofMarr.Rows[0][enmRelationship.date_of_marriage.ToString()])))
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
        }

        public bool CheckAllBenefitOptions(string astrBenefitOptions)
        {
            if (astrBenefitOptions == "JS75" || astrBenefitOptions == "JP75" || astrBenefitOptions == "J100" || astrBenefitOptions == "JPOP")
            {
                return true;
            }
            return false;
        }


        public ArrayList btn_CalculateBenefitClick()
        {

            ArrayList iarrErrors = new ArrayList();

            //Call Eligibility Yet Again the Final Time Just Before doing Final Calculation
            this.LoadWorkHistoryandSetupPrerequisites_Retirement();
            //PIR 1053
            if (this.iclbBenefitApplicationDetail.Where(item => item.iintPlan_ID == busConstant.MPIPP_PLAN_ID && item.icdoBenefitApplicationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION).Count()>0 && 
                this.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES 
                && ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).Count()>0)
            {
                ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION;
            }

            //Flags to be used for making sure we donot calculate again if another IAP entry comes along
            bool lblnIAPCalculated = false;
            bool lblnMPIPPCalculated = false;

            int lintMPIPPHeaderId = 0;
            int lintIAPHeaderId = 0;




            foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
            {
                if (this.icdoBenefitApplication.converted_min_distribution_flag != busConstant.FLAG_YES || lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                {
                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.application_detail_status_value != busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED)
                    {
                        #region Initialize Calculation Needed Objects from Application
                        busBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
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

                            }else
                            {
                                if(lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
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
                           if(lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                            {
                                lbusBenefitCalculationRetirement.AfterPersistChanges();
                                SetWorkflowRelatedVariablesforFinalCalculation(lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.benefit_calculation_header_id);

                            }else
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
                    CreateLateRetirementCalculationFromMinimumDistribution(lbusBenefitApplicationDetail);
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

        #region Private Methods

        private void CreateLateRetirementCalculationFromMinimumDistribution(busBenefitApplicationDetail lbusBenefitAppDetail)
        {
            DataTable ldtPayeeAcc = Select("cdoBenefitApplication.GetMinDistributionAccForPlan", new object[2] { lbusBenefitAppDetail.iintPlan_ID, this.icdoBenefitApplication.person_id });
            if (ldtPayeeAcc.IsNotNull() && ldtPayeeAcc.Rows.Count > 0)
            {
                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lbusPayeeAccount.icdoPayeeAccount.LoadData(ldtPayeeAcc.Rows[0]);
                if (lbusBenefitAppDetail.iintPlan_ID != busConstant.IAP_PLAN_ID)
                {
                    if (lbusPayeeAccount.icdoPayeeAccount.benefit_calculation_detail_id == 0)
                    {
                        CreateLateFromMDForConversion(lbusPayeeAccount, lbusBenefitAppDetail);
                    }
                    else
                    {
                        CreateLateFromMDCalculation(lbusPayeeAccount, lbusBenefitAppDetail);
                    }
                }

            }
        }

        private void CreateLateFromMDForConversion(busPayeeAccount abusPayeeAccount, busBenefitApplicationDetail lbusBenefitApplicationDetail)
        {
            int lintpersonAccountId = 0;
            decimal ldecBenefitAmount = decimal.Zero;
            decimal ldecMea = decimal.Zero;
            decimal ldecNonTaxableAmount = decimal.Zero;
            decimal ldecTaxableAnount = decimal.Zero;
            decimal ldecSurvivorAmount = decimal.Zero;
            string lstrEEFlag = busConstant.FLAG_NO;
            string lstrUvHpFlag = busConstant.FLAG_NO;
            string lstrL52Flag = busConstant.FLAG_NO;
            string lstrL161Flag = busConstant.FLAG_NO;
            string lstrBenefitOptionValue = string.Empty;
            decimal ldecRemainingMinGuarantee = decimal.Zero;
            decimal ldecRemainingNonTaxableBegBalance = decimal.Zero;

            this.ibusPerson.LoadPersonAccounts();
            abusPayeeAccount.ibusPayee = this.ibusPerson;
            abusPayeeAccount.ibusPayee.iclbPersonAccount = this.ibusPerson.iclbPersonAccount;
            if (this.ibusPerson.iclbPersonAccount.Count > 0)
            {
                if (this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusBenefitApplicationDetail.iintPlan_ID).Count() > 0)
                {
                    lintpersonAccountId = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusBenefitApplicationDetail.iintPlan_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                }
            }

            busBenefitCalculationRetirement lbusbenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
            busCalculation lbusCalculation = new busCalculation();
            //PROD PIR 159
            lbusbenefitCalculationRetirement.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusbenefitCalculationRetirement.ibusBenefitApplication.FindBenefitApplication(icdoBenefitApplication.benefit_application_id);

            #region LoadAmountsFromExistingPayeeAccount
            lstrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue;
            lbusCalculation.GetTaxableAndNonTaxableAmountFromPaymentItemTypes(abusPayeeAccount, out ldecTaxableAnount, out ldecNonTaxableAmount);
            ldecMea = ldecNonTaxableAmount;
            ldecBenefitAmount = ldecTaxableAnount + ldecNonTaxableAmount;
            ldecSurvivorAmount = lbusCalculation.GetSurvivorAmountFromBenefitAmount(ldecBenefitAmount, lstrBenefitOptionValue);
            ldecRemainingMinGuarantee = abusPayeeAccount.GetRemainingMinimumGuaranteeTillDate(DateTime.Now);

            ldecRemainingNonTaxableBegBalance = abusPayeeAccount.GetRemainingNonTaxableBeginningBalanaceTillDate(DateTime.Now);
            if (ldecRemainingMinGuarantee == decimal.Zero)
            {
                ldecRemainingMinGuarantee = ldecRemainingNonTaxableBegBalance;
            }
            #endregion

            #region Benefit Calculation Header
            lbusbenefitCalculationRetirement.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id,
                                                             busConstant.BENEFIT_TYPE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.retirement_date,
                                                             this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID);
            lbusbenefitCalculationRetirement.icdoBenefitCalculationHeader.payee_account_id = abusPayeeAccount.icdoPayeeAccount.payee_account_id;

            #endregion Benefit Calculation Header

            #region Benefit Calculation Details

            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id;
            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id = lbusBenefitApplicationDetail.iintPlan_ID;
            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id = lintpersonAccountId;
            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_subtype_value = busConstant.RETIREMENT_TYPE_LATE;


            if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
            {
                //lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.ee_flag = lstrEEFlag;
                //lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.uvhp_flag = lstrUvHpFlag;
                //lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag = lstrL52Flag;
                //lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag = lstrL161Flag;
            }
            else
            {
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.monthly_exclusion_amount = ldecMea;
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.minimum_guarantee_amount = ldecRemainingMinGuarantee;
            }

            #endregion

            #region Benefit Calculation Options

            lbusBenefitCalculationDetail.iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();

            busBenefitCalculationOptions lbusBenefitCalculationOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.account_relationship_id = busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_CODE_ID;
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.account_relationship_value = busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER;

            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_relationship_id = busConstant.BENEFICIARY_RELATIONSHIP_CODE_ID;
            //lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_relationship_value = astrSurvivorRelation;


            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.plan_benefit_id = abusPayeeAccount.icdoPayeeAccount.plan_benefit_id;

            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount = ldecSurvivorAmount;
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount = ldecBenefitAmount;




            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount = ldecBenefitAmount;
            lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount = ldecSurvivorAmount;
            lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Add(lbusBenefitCalculationOptions);



            #endregion

            #region Insert Calculation Object
            InsertFinalCalculation(lbusbenefitCalculationRetirement, lbusBenefitCalculationDetail, lbusBenefitApplicationDetail);
            #endregion


        }

        private void CreateLateFromMDCalculation(busPayeeAccount abusPayeeAccount, busBenefitApplicationDetail lbusBenefitApplicationDetail)
        {
            #region Load Calculation Object From MD
            busCalculation lbusCalculation = new busCalculation();
            busBenefitCalculationRetirement lbusbenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
            if (lbusBenefitCalculationDetail.FindBenefitCalculationDetail(abusPayeeAccount.icdoPayeeAccount.benefit_calculation_detail_id))
            {

                lbusBenefitCalculationDetail.LoadBenefitCalculationOptionss();
                lbusBenefitCalculationDetail.LoadBenefitCalculationYearlyDetails();
            }
            lbusbenefitCalculationRetirement.FindBenefitCalculationHeader(lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_header_id);
            lbusbenefitCalculationRetirement.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id,
                                                             busConstant.BENEFIT_TYPE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.retirement_date,
                                                             this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID);
            lbusbenefitCalculationRetirement.icdoBenefitCalculationHeader.payee_account_id = abusPayeeAccount.icdoPayeeAccount.payee_account_id;
            #endregion

            InsertFinalCalculation(lbusbenefitCalculationRetirement, lbusBenefitCalculationDetail, lbusBenefitApplicationDetail);

        }

        private void InsertFinalCalculation(busBenefitCalculationHeader abusBenefitCalculationHeader, busBenefitCalculationDetail abusBenefitCalculationDetail, busBenefitApplicationDetail abusBenefitApplicationDetail)
        {
            #region Insert Header
            busCalculation lbusCalculation = new busCalculation();
            abusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id = 0;
            lbusCalculation.ResetAuditFields(abusBenefitCalculationHeader.icdoBenefitCalculationHeader);
            abusBenefitCalculationHeader.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
            abusBenefitCalculationHeader.icdoBenefitCalculationHeader.Insert();
            #endregion

            #region Insert Detail
            abusBenefitCalculationDetail.icdoBenefitCalculationDetail.ienuObjectState = ObjectState.Insert;
            abusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id = 0;
            abusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_application_detail_id = abusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id;
            lbusCalculation.ResetAuditFields(abusBenefitCalculationDetail.icdoBenefitCalculationDetail);
            abusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_subtype_value = busConstant.RETIREMENT_TYPE_LATE;
            abusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_header_id = abusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id;
            abusBenefitCalculationDetail.icdoBenefitCalculationDetail.Insert();
            #endregion

            #region Insert Option
            if (!abusBenefitCalculationDetail.iclbBenefitCalculationOptions.IsNullOrEmpty())
            {
                foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in abusBenefitCalculationDetail.iclbBenefitCalculationOptions)
                {
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.ienuObjectState = ObjectState.Insert;
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_option_id = 0;
                    lbusCalculation.ResetAuditFields(lbusBenefitCalculationOptions.icdoBenefitCalculationOptions);
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_detail_id = abusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.Insert();

                }
            }
            #endregion

            #region InsertYearlyDetail
            if (!abusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail.IsNullOrEmpty())
            {

                foreach (busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail in abusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail)
                {
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.ienuObjectState = ObjectState.Insert;
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id = 0;
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_calculation_detail_id = abusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id; ;
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.Insert();

                    #region Commented
                    if (lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail != null && lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail.Count > 0)
                    {
                        foreach (busBenefitCalculationNonsuspendibleDetail lbusBenefitCalculationNonsuspendibleDetail in lbusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail)
                        {
                            lbusBenefitCalculationNonsuspendibleDetail.icdoBenefitCalculationNonsuspendibleDetail.benefit_calculation_yearly_detail_id = lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_calculation_yearly_detail_id;
                            lbusBenefitCalculationNonsuspendibleDetail.icdoBenefitCalculationNonsuspendibleDetail.benefit_calculation_detail_id = abusBenefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                            lbusBenefitCalculationNonsuspendibleDetail.icdoBenefitCalculationNonsuspendibleDetail.Insert();
                        }
                    }
                    #endregion
                }
            }
            #endregion

            SetWorkflowRelatedVariablesforFinalCalculation(abusBenefitApplicationDetail.istrPlanCode, abusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id);

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

        #endregion

        #endregion

        # region override
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            if (iobjPassInfo.istrFormName == busConstant.Retirement_Application_Maintenance_Form_2)
            {
                //PIR-799
                if (iclbRetirementEligiblePlans == null)
                {
                    LoadBenefitApplicationEligiblePlans();
                }
                //PIR-799 Rohan
                if (iclbRetirementEligiblePlans.Where(obj => obj.istrIsSelected == busConstant.FLAG_YES).Count() > 0)
                {
                    if (ibusBenefitApplicationEligiblePlans == null)
                    {
                        ibusBenefitApplicationEligiblePlans = new busBenefitApplicationEligiblePlans { icdoBenefitApplicationEligiblePlans = new cdoBenefitApplicationEligiblePlans() };
                    }
                    foreach (busRetirementEligiblePlans lRetirementEligiblePlans in iclbRetirementEligiblePlans)
                    {
                        // set eligible plans flag
                        if (lRetirementEligiblePlans.istrPlanName == busConstant.IAP)
                        {
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.iap_eligibleflag = busConstant.FLAG_YES;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.MPIPP)
                        {
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.mpipp_eligibleflag = busConstant.FLAG_YES;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.Local_600)
                        {
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local600_eligibleflag = busConstant.FLAG_YES;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.Local_666)
                        {
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local666_eligibleflag = busConstant.FLAG_YES;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.LOCAL_700)
                        {
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local700_eligibleflag = busConstant.FLAG_YES;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.Local_161)
                        {
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local161_eligibleflag = busConstant.FLAG_YES;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.Local_52)
                        {
                            ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local52_eligibleflag = busConstant.FLAG_YES;
                        }

                        // set eligible selected plans flag
                        //PIR 799 Rohan
                        if (lRetirementEligiblePlans.istrPlanName == busConstant.IAP)
                        {
                            if (lRetirementEligiblePlans.istrIsSelected == busConstant.FLAG_YES)
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.iap_flag = busConstant.FLAG_YES;
                            else
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.iap_flag = busConstant.FLAG_NO;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.MPIPP)
                        {
                            if (lRetirementEligiblePlans.istrIsSelected == busConstant.FLAG_YES)
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.mpipp_flag = busConstant.FLAG_YES;
                            else
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.mpipp_flag = busConstant.FLAG_NO;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.Local_600)
                        {
                            if (lRetirementEligiblePlans.istrIsSelected == busConstant.FLAG_YES)
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local600_flag = busConstant.FLAG_YES;
                            else
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local600_flag = busConstant.FLAG_NO;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.Local_666)
                        {
                            if (lRetirementEligiblePlans.istrIsSelected == busConstant.FLAG_YES)
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local666_flag = busConstant.FLAG_YES;
                            else
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local666_flag = busConstant.FLAG_NO;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.LOCAL_700)
                        {
                            if (lRetirementEligiblePlans.istrIsSelected == busConstant.FLAG_YES)
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local700_flag = busConstant.FLAG_YES;
                            else
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local700_flag = busConstant.FLAG_NO;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.Local_161)
                        {
                            if (lRetirementEligiblePlans.istrIsSelected == busConstant.FLAG_YES)
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local161_flag = busConstant.FLAG_YES;
                            else
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local161_flag = busConstant.FLAG_NO;
                        }
                        else if (lRetirementEligiblePlans.istrPlanName == busConstant.Local_52)
                        {
                            if (lRetirementEligiblePlans.istrIsSelected == busConstant.FLAG_YES)
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local52_flag = busConstant.FLAG_YES;
                            else
                                ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local52_flag = busConstant.FLAG_NO;
                        }
                    }
                    ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.benefit_application_id = icdoBenefitApplication.benefit_application_id;
                    if (ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.benefit_application_eligible_id == 0)
                        ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.Insert();
                    else
                        ibusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.Update();
                }
                //PIR-799
                if (icdoBenefitApplication.benefit_application_id != 0)
                    iblnIsRtmtAppFirstTimeSaved = true;

                //PIR-799 Rohan
                if (iblnFlag)
                {
                    Hashtable lhshRequestParam = new Hashtable();
                    lhshRequestParam.Add("Retirement_Application_ID", icdoBenefitApplication.benefit_application_id);
                    busWorkflowHelper.InitializeWorkflow(busConstant.RETIREMENT_WORKFLOW_NAME, this.icdoBenefitApplication.person_id, 0, 0, lhshRequestParam);
                }
            }
            if (this.iclbBenefitApplicationAuditingChecklist.IsNullOrEmpty())
            {
                AddBenefitAuditingCheckList(this.icdoBenefitApplication.benefit_application_id);
            }

        }

        public override int UpdateDataObject(doBase aobjDataObject)
        {
            if (this.iobjPassInfo.istrSenderID == "btnSaveBPMChanges"&& this.iobjPassInfo.istrFormName==busConstant.RETIREMENT_APPLICATION_MAINTAINENCE && aobjDataObject is cdoBenefitApplication)
            {
                aobjDataObject.ienuObjectState = ObjectState.None;
            }

            return base.UpdateDataObject(aobjDataObject);   
        }
        //PIR-799
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

        //PIR-799 Rohan
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

        /// <summary>
        /// PIR-799
        /// </summary>
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

        /// <summary>
        /// PIR-799
        /// </summary>
        /// <param name="aintBenefitApplicationId"></param>
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

        public override void BeforePersistChanges()
        {
            if (this.iobjPassInfo.istrSenderID== "btnSaveBPMChanges" && this.iarrChangeLog.Contains(icdoBenefitApplication))
            {
                this.iarrChangeLog.Remove(icdoBenefitApplication);
            }

           if (this.icdoBenefitApplication.converted_min_distribution_flag != busConstant.FLAG_YES)
            {
                //PIR RID 63412
                busPersonOverview lbusPersonOverview = new busPersonOverview();
                bool liMPIApprovedAppExists = false;
                if (lbusPersonOverview.FindPerson(this.icdoBenefitApplication.person_id))
                {
                    lbusPersonOverview.LoadBenefitApplication();
                }

                if (lbusPersonOverview.iclbBenefitApplication != null &&
                       lbusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT
                        && item.icdoBenefitApplication.iintPlanId == busConstant.MPIPP_PLAN_ID && item.icdoBenefitApplication.converted_min_distribution_flag == busConstant.FLAG_YES
                        && item.icdoBenefitApplication.application_status_value == busConstant.BENEFIT_APPLICATION_STATUS_APPROVED).Count() > 0)
                {
                    liMPIApprovedAppExists = true;
                }
                //end of PIR RID 63412

                if (icdoBenefitApplication.ienuObjectState == ObjectState.Insert && icdoBenefitApplication.retirement_date != DateTime.MinValue && !liMPIApprovedAppExists && 
                    icdoBenefitApplication.retirement_date >= icdoBenefitApplication.min_distribution_date)
                {
                    icdoBenefitApplication.min_distribution_flag = busConstant.FLAG_YES;

                }

                if (Eligible_Plans != null && Eligible_Plans.Count > 0 && icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES)
                {
                    foreach (string lstrPlanCode in Eligible_Plans)
                    {
                        this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == lstrPlanCode).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION;
                    }
                }

                if (Eligible_Plans != null && Eligible_Plans.Count > 0 && this.iclbBenefitApplicationDetail != null && this.iclbBenefitApplicationDetail.Count > 0)
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in iclbBenefitApplicationDetail)
                    {
                        if (this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).Count() > 0)
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value =
                                this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).First().icdoPersonAccount.istrRetirementSubType;
                            if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ienuObjectState == ObjectState.Select || lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ienuObjectState == ObjectState.Update)
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.Update();
                            }

                        }
                    }
                }

                if (this.icdoBenefitApplication.retirement_date != DateTime.MinValue && (this.idecAge < 0 || this.idecAge.IsNull()))
                {
                    idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.retirement_date);
                }
            }
            //LoadWorkHistoryandSetupPrerequisites(); //Code-Abhishek //OTherwise we might call this method in Validate Hard Errors if we do not want to create the application at all

            if (this.icdoBenefitApplication.min_distribution_flag == "Y" && iintIsManager == 0)
                this.icdoBenefitApplication.retirement_date = this.icdoBenefitApplication.min_distribution_date;
            else
                this.icdoBenefitApplication.retirement_date = this.icdoBenefitApplication.retirement_date;

            base.BeforePersistChanges();
            this.icdoBenefitApplication.person_id = this.ibusPerson.icdoPerson.person_id;

            //PIR-799
            if (iobjPassInfo.istrFormName == busConstant.Retirement_Application_Maintenance_Form_2)
            {
                iblnFlag = false;
                if (this.icdoBenefitApplication.benefit_application_id == 0)
                {
                    iblnFlag = true;
                    //DataTable ldtbGetRTMApplication = null;
                    //if (ibusPerson.icdoPerson.person_id != 0)
                    //{
                    //    ldtbGetRTMApplication = busBase.Select("cdoBenefitApplication.GetRetirementWorkflow", new object[1] { ibusPerson.icdoPerson.person_id });
                    //}
                    //if (ldtbGetRTMApplication != null && ldtbGetRTMApplication.Rows.Count > 0)
                    //{
                    //    if (!string.IsNullOrEmpty(Convert.ToString(ldtbGetRTMApplication.Rows[0]["CHECKED_OUT_USER"])))
                    //    {
                    //        busWorkflowHelper.InitializeWorkflow(busConstant.RETIREMENT_WORKFLOW_NAME, this.icdoBenefitApplication.person_id, 0, 0, null);
                    //    }
                    //}
                    //else
                    //{
                    //    busWorkflowHelper.InitializeWorkflow(busConstant.RETIREMENT_WORKFLOW_NAME, this.icdoBenefitApplication.person_id, 0, 0, null);
                    //}
                }
            }


            //this.ibusPerson.iclbNotes.ForEach(item =>
            //{
            //    if (item.icdoNotes.person_id == 0)
            //        item.icdoNotes.person_id = this.icdoBenefitApplication.person_id;
            //    item.icdoNotes.form_id = busConstant.Form_ID;
            //    item.icdoNotes.form_value = busConstant.RETIREMENT_APPLICATION_MAINTAINENCE_FORM;
            //});

            if (this.iclbBenefitApplicationDetail.Count > 0)
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in iclbBenefitApplicationDetail)
                {
                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.application_detail_status_value == null)
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.application_detail_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                    }
                }
            }


            if (!this.iarrChangeLog.Contains(this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist) && this.icdoBenefitApplication.benefit_application_id > 0)
            {
                if (this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.IsNull())
                {
                    this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist = new doBenefitApplicationChecklist();
                    this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.benefit_application_id = this.icdoBenefitApplication.benefit_application_id;
                    this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.ienuObjectState = ObjectState.Insert;
                    this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.Insert();
                }
            }
        }

        public override busBase GetCorPerson()
        {
            ibusPerson.LoadPersonAddresss();
            ibusPerson.LoadPersonContacts();
            ibusPerson.LoadCorrAddress();
            return this.ibusPerson;
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
        //Code-Abhishek


        //Code-Abhishek
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
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

                //TEMPORARY COMMENTED - 10/10/2013
                //if (icdoBenefitApplication.retirement_date != DateTime.MinValue && icdoBenefitApplication.retirement_date < DateTime.Today && icdoBenefitApplication.min_distribution_flag != busConstant.FLAG_YES)
                //{
                //    if (iobjPassInfo.ienmPageMode == utlPageMode.New)
                //    {
                //        lobjError = AddError(5028, " ");
                //        this.iarrErrors.Add(lobjError);  
                //    }
                //    else if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
                //    { 
                //        if(icdoBenefitApplication.retirement_date != Convert.ToDateTime(icdoBenefitApplication.ihstOldValues[enmBenefitApplication.retirement_date.ToString()]))
                //        {
                //            lobjError = AddError(5028, " ");
                //            this.iarrErrors.Add(lobjError);  
                //        }
                //    }
                //}


                if (this.NotEligible.IsNotNull() && this.NotEligible)
                {
                    lobjError = AddError(5103, " ");
                    this.iarrErrors.Add(lobjError);
                }
                int lintPersonId = 0;
                if (this.icdoBenefitApplication.person_id>0)
                {
                    lintPersonId = this.icdoBenefitApplication.person_id;
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
                            //DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.CheckRetirementApplForMpiAndIAP", new object[1] { this.icdoBenefitApplication.person_id});
                            //if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
                            //{
                            //    lintCount =lintCount + Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]); 
                            //}

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

                if (this.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES && GetDateDifference() < 60)
                {
                    lobjError = AddError(5089, "");
                    this.iarrErrors.Add(lobjError);
                }

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
                if (icdoBenefitApplication.retirement_date != DateTime.MinValue && icdoBenefitApplication.retirement_date < DateTime.Today)
                {
                    lobjError = AddError(5028, " ");
                    this.iarrErrors.Add(lobjError);
                }
                // Disable Save button when app status is approved and validation for rtmt date should be 1st day of month
                if (this.icdoBenefitApplication != null && this.icdoBenefitApplication.retirement_date != DateTime.MinValue &&
                    this.icdoBenefitApplication.retirement_date.Day != 1 && this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
                {
                    lobjError = AddError(5088, " ");
                    this.iarrErrors.Add(lobjError);
                }
                //PIR-799
                if (this.icdoBenefitApplication != null && this.icdoBenefitApplication.retirement_date != DateTime.MinValue)
                {
                    DateTime ldtReceivedDate1 = DateTime.MinValue, ldtReceivedDate2 = DateTime.MinValue;
                    DateTime ldtCurrentDate = DateTime.MinValue;
                    ldtReceivedDate1 = this.icdoBenefitApplication.retirement_date.AddMonths(-2).AddDays(-1);
                    ldtReceivedDate2 = this.icdoBenefitApplication.retirement_date.AddMonths(-6).AddDays(-1);
                    ldtCurrentDate = DateTime.Today;
                    //if (ldtCurrentDate > ldtReceivedDate1 || ldtCurrentDate < ldtReceivedDate2)
                    if (!(ldtCurrentDate <= ldtReceivedDate1 && ldtCurrentDate >= ldtReceivedDate2))
                    {
                        lobjError = AddError(6232, " ");
                        this.iarrErrors.Add(lobjError);
                    }
                }
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


        public ArrayList btn_CancelGridBenAppDetail(int aintBenefitApplicationDetailID)
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;
            bool flagStatus = false;

           
            busBenefitApplicationDetail lbusBenefitApplicationDetail = new busBenefitApplicationDetail { icdoBenefitApplicationDetail = new cdoBenefitApplicationDetail() };
            lbusBenefitApplicationDetail = this.iclbBenefitApplicationDetail.Where(item => item.icdoBenefitApplicationDetail.benefit_application_detail_id == aintBenefitApplicationDetailID).FirstOrDefault();
            
            if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id > 0)
            {

                ////PIR-911: Allow Cancel if there is a disablity retirement application with status pending
                int lintCheckDisabililityApplication = (int)DBFunction.DBExecuteScalar("cdoBenefitApplicationDetail.GetDisabilityRetirementRecord",
                               new object[1] { this.ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (lintCheckDisabililityApplication <= 0)
                {
                    lobjError = AddError(6287, "");
                    //this.iarrErrors.Add(lobjError);
                    //return iarrErrors;
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }


                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.application_detail_status_value = busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED;
                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.Update();


                DataTable ldtbStatus = new DataTable();
                Collection<busBenefitCalculationRetirement> lclbBenefitCalculationHeader = new Collection<busBenefitCalculationRetirement>();
                ldtbStatus = Select("cdoBenefitApplicationDetail.GetBenCalHeaderDetails", new object[2] { lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_id, lbusBenefitApplicationDetail.iintPlan_ID });

                if (ldtbStatus.Rows.Count > 0)
                {
                    lclbBenefitCalculationHeader = GetCollection<busBenefitCalculationRetirement>(ldtbStatus, "icdoBenefitCalculationHeader");
                    foreach (busBenefitCalculationRetirement lbusBenefitCalculationHeader in lclbBenefitCalculationHeader)
                    {
                        lbusBenefitCalculationHeader.LoadBenefitCalculationDetails();
                        lbusBenefitCalculationHeader.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lbusBenefitCalculationHeader.ibusBenefitApplication = this;
                        lbusBenefitCalculationHeader.ibusPerson = this.ibusPerson;
                        lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount = this.ibusPerson.iclbPersonAccount;
                        lbusBenefitCalculationHeader.RevertMPIContributions();
                        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.status_value = busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED;
                        lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.Update();
                    }
                }

                ldtbStatus.Clear();

                Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
                ldtbStatus = Select("cdoBenefitApplicationDetail.GetPayeeAcntIdFromDetailId", new object[1] { lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id });

                if (ldtbStatus.IsNotNull() && ldtbStatus.Rows.Count > 0)
                {

                    lclbPayeeAccount = GetCollection<busPayeeAccount>(ldtbStatus, "icdoPayeeAccount");

                    foreach (busPayeeAccount lbusPayeeAccount in lclbPayeeAccount)
                    {
                        cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                        lcdoPayeeAccountStatus.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                        lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED;
                        lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                        lcdoPayeeAccountStatus.Insert();
                    }

                }
            }

            if (this.iclbBenefitApplicationDetail.Count > 0)
            {
                foreach (busBenefitApplicationDetail lobjBenefitApplicationDetail in iclbBenefitApplicationDetail)
                {
                    if (lobjBenefitApplicationDetail.icdoBenefitApplicationDetail.application_detail_status_value != busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED)
                    {
                        flagStatus = true;
                        break;
                    }
                }
            }
            if (flagStatus != true)
            {
                this.icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED;
                icdoBenefitApplication.Update();
                UpdateStatusHistoryValue();
                EvaluateInitialLoadRules();
            }
            lbusBenefitApplicationDetail.EvaluateInitialLoadRules();
            larrErrors.Add(this);
            return larrErrors;
        }
        # endregion

        public void CreateLateRetirementApplication()
        {
            if (this.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES)
            {
                #region Set Payee Account As Complete
                DataTable ldtPayeeAccount = new DataTable();
                ldtPayeeAccount = Select("cdoBenefitApplication.GetPayeeAccountsForApplication", new object[1] { this.icdoBenefitApplication.benefit_application_id });

                if (ldtPayeeAccount.IsNotNull() && ldtPayeeAccount.Rows.Count > 0)
                {
                    int lintPayeeAccountID = 0;
                    busPayeeAccountStatus lbusPayeeAccountStatus = null;
                    foreach (DataRow ldtRow in ldtPayeeAccount.Rows)
                    {
                        if (Convert.ToString(ldtRow[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty())
                        {
                            lintPayeeAccountID = Convert.ToInt32(ldtRow[enmPayeeAccount.payee_account_id.ToString()]);
                            lbusPayeeAccountStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
                            lbusPayeeAccountStatus.InsertValuesInPayeeAccountStatus(lintPayeeAccountID, busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED, DateTime.Now);
                        }
                    }
                }
                #endregion

                #region Load Existing Object
                this.ibusPerson = new busPerson();
                this.ibusPerson.FindPerson(this.icdoBenefitApplication.person_id);
                this.GetAgeAtRetirement(this.icdoBenefitApplication.retirement_date);
                this.ibusPerson.LoadPersonAccounts(); //Load Person Accounts //Code-Abhishek                
                this.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added Abhishek                
                this.LoadInitialData();
                this.LoadBenefitApplicationDetails();
                #endregion

                #region Create New Late Retirement Application
                this.icdoBenefitApplication.final_calc_flag = busConstant.FLAG_NO;
                this.icdoBenefitApplication.min_distribution_flag = busConstant.FLAG_NO;
                this.icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                this.icdoBenefitApplication.converted_min_distribution_flag = busConstant.FLAG_YES;
                this.icdoBenefitApplication.benefit_application_id = 0;
                this.icdoBenefitApplication.created_by = iobjPassInfo.istrUserID;
                this.icdoBenefitApplication.created_date = DateTime.Now;
                this.icdoBenefitApplication.modified_by = iobjPassInfo.istrUserID;
                this.icdoBenefitApplication.modified_date = DateTime.Now;
                this.icdoBenefitApplication.update_seq = 0;

                this.icdoBenefitApplication.ienuObjectState = ObjectState.Insert;
                this.icdoBenefitApplication.Insert();
                
                foreach (busBenefitApplicationDetail lbusBenefitAppDetail in this.iclbBenefitApplicationDetail)
                {
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.benefit_application_detail_id = 0;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.ienuObjectState = ObjectState.Insert;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.application_detail_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.benefit_subtype_value = busConstant.RETIREMENT_TYPE_LATE;
                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.benefit_application_id = this.icdoBenefitApplication.benefit_application_id;
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
      
        //for PIR-522
        public void CheckIfManagerLogin()
        {
            busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
            if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
                iintIsManager = 1;
            else
                iintIsManager = 2;

        }

        public void LoadPersonSpousDetails()
        {
            this.ibusPerson.LoadBeneficiaries();
            int lintSpousePersonID = 0;
            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.ibusPerson.iclbPersonAccountBeneficiary)
            {
                if (lbusPersonAccountBeneficiary.ibusRelationship.icdoRelationship.relationship_value == "SPOU")
                {
                    lintSpousePersonID = lbusPersonAccountBeneficiary.ibusRelationship.icdoRelationship.person_id;
                }
            }
            if (lintSpousePersonID > 0)
            {
                busPerson lbusSpouse = new busPerson { icdoPerson = new cdoPerson() };
                lbusSpouse.FindPerson(lintSpousePersonID);
                this.istrSpouseNameMarraige = lbusSpouse.icdoPerson.istrParticipantFullName;
                this.istrSpouseNameDeath = lbusSpouse.icdoPerson.date_of_death;
            }
        }

        public void LoadListOfDivorces()
        {
            DataTable ldtblist = busBase.Select("entBenefitApplication.GetListOfDivorces", new object[1] { this.icdoBenefitApplication.person_id });

            if (this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.IsNull())
            {
                this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist = new doBenefitApplicationChecklist(); 
            }
            
            if (ldtblist.Rows.Count > 0)
            {
                if (ldtblist.Rows.Count > 0)

                    this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.no_of_divorces_ex_spouse_name1 = Convert.ToString(ldtblist.Rows[0]["FullName"]);
                if (ldtblist.Rows.Count > 1)
                    this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.no_of_divorces_ex_spouse_name2 = Convert.ToString(ldtblist.Rows[1]["FullName"]);

                if (ldtblist.Rows.Count > 2)
                    this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.no_of_divorces_ex_spouse_name3 = Convert.ToString(ldtblist.Rows[2]["FullName"]);

                if (ldtblist.Rows.Count > 3)
                    this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.no_of_divorces_ex_spouse_name4 = Convert.ToString(ldtblist.Rows[3]["FullName"]);
            }
        }

        public void UpdateBenefitOptionValue(int intPlan_Id, string istrBenefitOptionValue, int benefit_application_detail_id)
        {

            busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
            var lplanbenefitId = lbusPlanBenefitXr.GetPlanBenefitId(intPlan_Id, istrBenefitOptionValue);
            // Collection<cdoPlanBenefitXr> lclcBenefitOptions = new Collection<cdoPlanBenefitXr>();


            var llbBenefitApplicationDetail = this.iclbBenefitApplicationDetail.Where(x => x.icdoBenefitApplicationDetail.benefit_application_detail_id == benefit_application_detail_id).FirstOrDefault();
            llbBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id = lplanbenefitId;
            llbBenefitApplicationDetail.icdoBenefitApplicationDetail.Update();
            //var lplanbenefitId = lclcBenefitOptions.Where(i => i.benefit_option_value == this.icdoBenefitApplicationDetail.istrBenefitOptionValue).Select(x => x.benefit_option_id).SingleOrDefault();
            //this.lbicdoBenefitApplicationDetail.plan_benefit_id = lplanbenefitId;
            //this.icdoBenefitApplicationDetail.Update();
        }
        public override ArrayList OnBpmSubmit()
        {
            SetBPMActivityInstanceParameters();
            return base.OnBpmSubmit();
        }

        public void SetBPMActivityInstanceParameters()
        {
            if (ibusBaseActivityInstance != null)
            {
                busSolBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busSolBpmActivityInstance;
                if (lbusBpmActivityInstance != null)
                {
                    busUser lbusUser = new busUser();
                    if (lbusUser.FindUser(lbusBpmActivityInstance.icdoBpmActivityInstance.checked_out_user))
                    {
                        lbusBpmActivityInstance.UpdateParameterValue("PreviousCheckOutUser", lbusUser.icdoUser.user_serial_id);
                    }

                    if (lbusBpmActivityInstance.ibusBpmProcessInstance.IsNotNull() && lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.IsNotNull() && lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.ibusBpmCase.IsNotNull()
                        && lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.ibusBpmCase.icdoBpmCase.name == busConstant.PersonAccountMaintenance.APPLICATION_SERVICE_RETIREMENT_BPM)
                    {
                        lbusBpmActivityInstance.UpdateParameterValue("LastActivityName", busConstant.PersonAccountMaintenance.APPLICATION_SERVICE_RETIREMENT_PROCESS);
                        lbusBpmActivityInstance.UpdateParameterValue("LastCheckOutUser", lbusBpmActivityInstance.icdoBpmActivityInstance.checked_out_user);

                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PAYEE_ACCOUNT_APPROVED, busConstant.FLAG_NO);
                        iintBPMPlanId = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue("PlanId"));
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.IAP_PLAN, busConstant.FLAG_NO);
                        if (iintBPMPlanId == busConstant.IAP_PLAN_ID)
                        {
                            SetIAPWaitTimer(lbusBpmActivityInstance);
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.IAP_PLAN, LoadIAPFlag(lbusBpmActivityInstance, this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.iap_wait_timer));
                        }
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_FLAG, QDROLegalReviewRequired());
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.BENEFIT_CALCULATION_APPROVED, IsBenefitCalculationApproved(lbusBpmActivityInstance));
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PAYEE_ACCOUNT_EXISTS, IsPayeeAccountAuditRequired(lbusBpmActivityInstance));
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PAYEE_ACCOUNT_ID, LoadPayeeAccountId(lbusBpmActivityInstance));
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PERSON_ACCOUNT_ID, LoadPersonAccountId());                  
                    }
                }
            }
        }
        public string QDROLegalReviewRequired()
        {
            DataTable ldtblist = busPerson.Select("entPersonAccount.QDROLegalReviewRequiredForApplicationMaintenace", new object[2] { this.icdoBenefitApplication.person_id, iintBPMPlanId });
            if (ldtblist.Rows.Count > 0)
            {
                int count = ldtblist.AsEnumerable().Count(row => row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES &&
                                                    row.IsNull("QDRO_REVIEW_COMPLETED_DATE"));
                if (count > 0)
                {
                    return busConstant.FLAG_YES;
                }
            }
            if (iintBPMPlanId == busConstant.IAP_PLAN_ID)
            {
              if (ldtblist.Rows.Count == 0)
                 {
                    if (iintBPMPlanId== busConstant.MPIPP_PLAN_ID)
                    {
                        DataTable ldtblist1 = busPerson.Select("entPersonAccount.QDROLegalReviewRequiredForApplicationMaintenace", new object[2] { this.icdoBenefitApplication.person_id, busConstant.MPIPP_PLAN_ID });
                        if (ldtblist1.Rows.Count > 0)
                        {
                            int count = ldtblist.AsEnumerable().Count(row => row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES &&
                                                       row.IsNull("QDRO_REVIEW_COMPLETED_DATE"));
                            if (count > 0)
                            {
                                return busConstant.FLAG_YES;
                            }
                        }
                    }
                    else if (iintBPMPlanId == busConstant.LOCAL_52_PLAN_ID)
                    {
                        DataTable ldtblist1 = busPerson.Select("entPersonAccount.QDROLegalReviewRequiredForApplicationMaintenace", new object[2] { this.icdoBenefitApplication.person_id, busConstant.LOCAL_52_PLAN_ID });
                        if (ldtblist1.Rows.Count > 0)
                        {
                            int count = ldtblist.AsEnumerable().Count(row => row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES &&
                                                       row.IsNull("QDRO_REVIEW_COMPLETED_DATE"));
                            if (count > 0)
                            {
                                return busConstant.FLAG_YES;
                            }
                        }
                    }
                    else if (iintBPMPlanId == busConstant.LOCAL_161_PLAN_ID)
                    {
                        DataTable ldtblist1 = busPerson.Select("entPersonAccount.QDROLegalReviewRequiredForApplicationMaintenace", new object[2] { this.icdoBenefitApplication.person_id, busConstant.LOCAL_161_PLAN_ID });
                        if (ldtblist1.Rows.Count > 0)
                        {
                            int count = ldtblist.AsEnumerable().Count(row => row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES &&
                                                       row.IsNull("QDRO_REVIEW_COMPLETED_DATE"));
                            if (count > 0)
                            {
                                return busConstant.FLAG_YES;
                            }
                        }
                    }
                    else if (iintBPMPlanId == busConstant.LOCAL_600_PLAN_ID)
                    {
                        DataTable ldtblist1 = busPerson.Select("entPersonAccount.QDROLegalReviewRequiredForApplicationMaintenace", new object[2] { this.icdoBenefitApplication.person_id, busConstant.LOCAL_600_PLAN_ID });
                        if (ldtblist1.Rows.Count > 0)
                        {
                            int count = ldtblist.AsEnumerable().Count(row => row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES &&
                                                       row.IsNull("QDRO_REVIEW_COMPLETED_DATE"));
                            if (count > 0)
                            {
                                return busConstant.FLAG_YES;
                            }
                        }
                    }
                    else if (iintBPMPlanId == busConstant.LOCAL_700_PLAN_ID)
                    {
                        DataTable ldtblist1 = busPerson.Select("entPersonAccount.QDROLegalReviewRequiredForApplicationMaintenace", new object[2] { this.icdoBenefitApplication.person_id, busConstant.LOCAL_700_PLAN_ID });
                        if (ldtblist1.Rows.Count > 0)
                        {
                            int count = ldtblist.AsEnumerable().Count(row => row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES &&
                                                       row.IsNull("QDRO_REVIEW_COMPLETED_DATE"));
                            if (count > 0)
                            {
                                return busConstant.FLAG_YES;
                            }
                        }
                    }
                    else if (iintBPMPlanId == busConstant.LOCAL_666_PLAN_ID)
                    {
                        DataTable ldtblist1 = busPerson.Select("entPersonAccount.QDROLegalReviewRequiredForApplicationMaintenace", new object[2] { this.icdoBenefitApplication.person_id, busConstant.LOCAL_666_PLAN_ID });
                        if (ldtblist1.Rows.Count > 0)
                        {
                            int count = ldtblist.AsEnumerable().Count(row => row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES &&
                                                       row.IsNull("QDRO_REVIEW_COMPLETED_DATE"));
                            if (count > 0)
                            {
                                return busConstant.FLAG_YES;
                            }
                        }
                    }
                }
            }
            return busConstant.FLAG_NO;
        }
        public string IsBenefitCalculationApproved(busSolBpmActivityInstance lbusBpmActivityInstance)
        {
            int iintApplicationId = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_ID));
            DataTable ldtblist = busPerson.Select("entBenefitCalculationHeader.BenefitCalculationApproved", new object[3] {  iintBPMPlanId, this.icdoBenefitApplication.person_id, iintApplicationId });
            if (ldtblist.Rows.Count > 0)
            {
                return busConstant.FLAG_YES;
            }
            return busConstant.FLAG_NO;
        }
        public string IsPayeeAccountAuditRequired(busSolBpmActivityInstance lbusBpmActivityInstance)
        {
            int iintReferenceId = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue("ReferenceId"));
            int iintApplicationId = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_ID));
            DataTable ldtblist = busPerson.Select("entPersonAccount.LoadPayeeAccountIdByPersonId", new object[4] { this.icdoBenefitApplication.person_id, iintBPMPlanId, iintApplicationId, iintReferenceId });
           if (ldtblist.Rows.Count > 0)
            {
                if (ldtblist.Rows[0]["STATUS_VALUE"].ToString() == busConstant.PayeeAccountStatusApproved)
                {
                    return busConstant.FLAG_NO;
                }
            }
            return busConstant.FLAG_YES;
        }

        public int LoadPayeeAccountId(busSolBpmActivityInstance lbusBpmActivityInstance)
        {
            int iintReferenceId = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue("ReferenceId"));
            int iintApplicationId = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_ID));
            DataTable ldtblist = busPerson.Select("entPersonAccount.LoadPayeeAccountIdByPersonId", new object[4] { this.icdoBenefitApplication.person_id, iintBPMPlanId, iintApplicationId, iintReferenceId });
            if (ldtblist.Rows.Count > 0)
            {
                return Convert.ToInt32(ldtblist.Rows[0]["PAYEE_ACCOUNT_ID"].ToString());
            }
            else
            {
                DataTable ldtblist1 = busPerson.Select("entPersonAccount.LoadPayeeAccountIdByDetailId", new object[2] { this.icdoBenefitApplication.person_id, iintReferenceId });
                if (ldtblist1.Rows.Count > 0)
                {
                    return Convert.ToInt32(ldtblist1.Rows[0]["PAYEE_ACCOUNT_ID"].ToString());
                }

            }
            return 0;
        }
        public int LoadPersonAccountId()
        {
            DataTable ldtblist = busPerson.Select("entPersonAccount.LoadPersonAccountbyPlanId", new object[2] { this.icdoBenefitApplication.person_id, iintBPMPlanId });
            if (ldtblist.Rows.Count > 0)
            {
                return Convert.ToInt32(ldtblist.Rows[0]["PERSON_ACCOUNT_ID"].ToString());
            }
            return 0;
        }
        public void SetIAPWaitTimer(busSolBpmActivityInstance lbusBpmActivityInstance)
        {
            if (this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.iap_wait_timer == DateTime.MinValue || this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.iap_wait_timer <= DateTime.Today)
            {
                if (lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.APPLICATION_SERVICE_RETIREMENT_PROCESS)
                {
                    int iintActivityId = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue(busConstant.PersonAccountMaintenance.ACTIVITY_INSTANCE_ID));
                    if (iintActivityId == 0)
                    {
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.ACTIVITY_INSTANCE_ID, lbusBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id);
                    }
                }
                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.IAP_WAIT_TIMER, DateTime.Now.AddSeconds(10));
            }
            else if (this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.iap_wait_timer.Date >= DateTime.Today)
            {
                if (lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.APPLICATION_SERVICE_RETIREMENT_PROCESS)
                {
                    int iintActivityId = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue(busConstant.PersonAccountMaintenance.ACTIVITY_INSTANCE_ID));
                    if (iintActivityId == 0)
                    {
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.ACTIVITY_INSTANCE_ID, lbusBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id);
                    }
                }
                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.IAP_WAIT_TIMER, this.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.iap_wait_timer);
            }
        }
        public string LoadIAPFlag(busSolBpmActivityInstance lbusBpmActivityInstance, DateTime WaitTimer)
        {
            DateTime WaitTime = DateTime.MinValue;
            int iintActivityInstanceID = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue(busConstant.PersonAccountMaintenance.ACTIVITY_INSTANCE_ID));

            DataTable ldtblist = busBase.Select("entBpmTimerActivityInstanceDetails.GetTimerByActivityInstanceId", new object[1] { iintActivityInstanceID });

            if (iintActivityInstanceID== lbusBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id)
            {
                if (WaitTimer >= DateTime.Now)
                {
                    return busConstant.Flag_Yes;
                }
            }
            if (iintActivityInstanceID != lbusBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id && ldtblist.Rows.Count > 0)
            {
                return busConstant.Flag_Yes;
            }
            return busConstant.FLAG_NO;
        }
        public void LoadBenefitAuditingCheckList(int aintAuditingBenefitChecklistId) 
        {
            DataTable ldtbList = Select<doBenefitApplicationAuditingChecklist>(
                new string[1] { enmBenefitApplicationAuditingChecklist.benefit_application_id.ToString() },
                new object[1] { this.icdoBenefitApplication.benefit_application_id }, null, null);
            iclbBenefitApplicationAuditingChecklist = GetCollection<busBenefitApplicationAuditingChecklist>(ldtbList, "icdoBenefitApplicationAuditingChecklist");
        }
        public void AddBenefitAuditingCheckList( int aintAuditingBenefitChecklistId)
        {
            this.iclbBenefitApplicationAuditingChecklist = new Collection<busBenefitApplicationAuditingChecklist>();
          


            for (int i = 0; i < 5; i++)
            {
                this.ibusBenefitApplicationAuditingChecklist = new busBenefitApplicationAuditingChecklist();
                this.ibusBenefitApplicationAuditingChecklist.icdoBenefitApplicationAuditingChecklist = new doBenefitApplicationAuditingChecklist();
                this.ibusBenefitApplicationAuditingChecklist.icdoBenefitApplicationAuditingChecklist.benefit_application_id = aintAuditingBenefitChecklistId;
                this.ibusBenefitApplicationAuditingChecklist.icdoBenefitApplicationAuditingChecklist.ienuObjectState = ObjectState.Insert;
                this.ibusBenefitApplicationAuditingChecklist.icdoBenefitApplicationAuditingChecklist.Insert();

                this.iclbBenefitApplicationAuditingChecklist.Add(this.ibusBenefitApplicationAuditingChecklist);
            }
        }
    }
}
