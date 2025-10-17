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
using System.Data.SqlClient;
using NeoSpin.BusinessObjects;
using Sagitec.Bpm;

#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busPersonOverview : busPerson
    {
        public busAnnualBenefitSummaryOverview lbusAnnualBenefitSummaryOverview { get; set; }
        public Collection<busQdroApplication> iclbQdroApplication { get; set; }
        public Collection<busDeathNotification> iclbDeathNotifications { get; set; }
        //public Collection<busBenefitApplication> iclbBenefitApplication { get; set; }
        public Collection<busActivityInstance> iclbPersonWorkflows { get; set; }
        public Collection<busBpmActivityInstanceHistory> iclbPersonBPMflows { get; set; }

        public busBenefitApplication lbusBenefitApplication { get; set; }
        public Collection<cdoPersonAccount> iclcdoPersonAccountOverview { get; set; }
        public Collection<busPersonAccount> iclbPersonAccountOverview { get; set; }
        public busPersonAccountEligibility ibusPersonAccountEligibilityOverview { get; set; }
        public Collection<busPayeeAccount> iclbPayeeAccount { get; set; }

        public cdoDummyWorkData lclbLocalMergerCdoDummyWorkdata { get; set; }
        public busBenefitCalculationHeader lbusBenefitCalculationHeader { get; set; }
        public string istrAppDeadline { get; set; }
        public string istrRetirement_Date { get; set; }
        public busPerson ibusPerson { get; set; }
        public int iintLocalQualifiedYearCount { get; set; }
        public string istrCurrentDate { get; set; }
        public int iintYear { get; set; }
        public int iintNextConsecutiveYear { get; set; }
        //public decimal idecAccruedBenefit { get; set; }
        public string  istrLastBisYears { get; set; }
        public Collection<cdoDummyWorkData> aclbPersonWorkHistory_Local { get; set; }
        public DateTime ldtWihtdrawalEffectiveDate { get; set; }
        DateTime ldtEffectiveDate = DateTime.MinValue;
        public string istrIsEligibleForIAP { get; set; }
        public decimal ldecHoursAfterWithdrawal { get; set; }
        public busPersonAccount ibusPersonAccount { get; set; }

        public DateTime ldtLastReportedDate { get; set; }

        // Corr PER-0014
        public decimal ldecTotalQualifiedAccuruedBft { get; set; }
        public decimal ldecTotalCreditedHrs { get; set; }
        public decimal lintTotalqualifiedYearsCnt { get; set; }
        public int lintBISYearCnt { get; set; }

        public string ldecTotalAccuruedBftAmt { get; set; }
        public string ldecTotalSpecialAmt { get; set; }

        public string ldecIsvested { get; set; }

        public string ldecTotalEEUVHPAmt { get; set; }

        //public int ldecHealthQualifiedYears { get; set; }

        //public decimal ldecHealthQualifiedHours { get; set; }

        public bool iblnEligibleForActiveIncrease { get; set; } //Temporary

        //ID 68932
        //public busPensionVerificationHistory lbusPensionVerificationHistory { get; set; }
        public Collection<busPensionVerificationHistory> iclbPensionVerificationHistory { get; set; }
        public DateTime ldtResumptionDate { get; set; }

        public bool IsCancelButtonVisible()
        {
            if (this.icdoPerson.IsNotNull())
            {
               DataTable ldtblist = busPerson.Select("entPerson.CancelButtonVisibility", new object[1] { this.icdoPerson.person_id });
               if (ldtblist.Rows.Count > 0)
                 {
                    return true;
                 }
            }
            return false;
        }

        #region Public Methods
        //PIR-933
        public void ReCalculateVesting(int aintpersonid)
        {
            if (this.icdoPerson.IsNotNull())
            {
                this.icdoPerson.recalculate_vesting_flag = busConstant.FLAG_YES;
                this.icdoPerson.Update();
            }
        }

        public void LoadActiveContacts()
        {
            DataTable ldtTable = Select("cdoPersonContact.GetAllActiveContacts", new object[1] { this.icdoPerson.person_id });
            this.iclbPersonContact = GetCollection<busPersonContact>(ldtTable, "icdoPersonContact");
        }

        public void LoadBeneficiariesForOverview()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadBeneficaryForOverview", new object[1] { this.icdoPerson.person_id });
            iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtblist, "icdoPersonAccountBeneficiary");
            //int i = 1;
            foreach (busPersonAccountBeneficiary objbusPersonAccountBeneficiary in iclbPersonAccountBeneficiary)
            {
                busRelationship objbusRelationship = new busRelationship();
                objbusRelationship.FindRelationship(objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_relationship_id);
                objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dtdateOfMarriage = objbusRelationship.icdoRelationship.date_of_marriage;
                if (objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrDOB != DateTime.MinValue)
                {
                    objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dtdateOfbirth = objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrDOB;
                    objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.idecage = busGlobalFunctions.CalculatePersonAge(objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dtdateOfbirth, DateTime.Today);
                }
                //objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iintAPrimaryKey = i;
                //i++;
            }
        }

        public override busBase GetCorPerson()
        {
            return this;
        }

        public void GetRetirementAndApplDeadlineDate()
        {
            DateTime ldtDateOfBirth = new DateTime();
            DateTime ldtDateAtAge65 = new DateTime();
            DateTime ldtRetirementDate = new DateTime();
            DateTime ldtApplDeadlineDate = new DateTime();
            DateTime ldtBatchRunDate = DateTime.Now;

            ldtDateOfBirth = this.icdoPerson.idtDateofBirth;
            if (ldtDateOfBirth != DateTime.MinValue)
            {
                ldtDateAtAge65 = ldtDateOfBirth.AddYears(65);
            }
            int lintYears = 0; int lintMonths = 0; int lintDays = 0;
            busGlobalFunctions.GetDetailTimeSpan(DateTime.Now, ldtDateOfBirth, out lintYears, out lintMonths, out lintDays);
            ldtRetirementDate = DateTime.MinValue;

            if (lintYears == 64 && (lintMonths >= 6 && lintMonths <= 9))
            {
                if (ldtDateOfBirth.Day == 1)
                {
                    ldtRetirementDate = busGlobalFunctions.FirstDayOfMonthFromDateTime(ldtDateAtAge65);
                }
                else
                {
                    ldtRetirementDate = busGlobalFunctions.FirstDayOfMonthFromDateTime(ldtDateAtAge65).AddMonths(1);
                }
                ldtApplDeadlineDate = ldtRetirementDate.AddMonths(-3).GetLastDayofMonth();
            }
            else if (lintYears >= 64 && lintYears <= 70)
            {
                if ((lintYears == 64 && lintMonths >= 10) || (lintYears > 64 && lintYears < 70) || (lintYears == 70 && lintMonths <= 6))
                {
                    ldtApplDeadlineDate = busGlobalFunctions.LastDayOfMonthFromDateTime(ldtBatchRunDate);
                    ldtRetirementDate = busGlobalFunctions.FirstDayOfMonthFromDateTime(ldtApplDeadlineDate.AddMonths(2).AddDays(1));
                }
            }

            //istrRetirement_Date = Convert.ToString(ldtRetirementDate);
            if (ldtRetirementDate == DateTime.MinValue)
                istrRetirement_Date = string.Empty;
            else
                istrRetirement_Date = busGlobalFunctions.ConvertDateIntoDifFormat(ldtRetirementDate);

            if (ldtApplDeadlineDate == DateTime.MinValue)
                istrAppDeadline = string.Empty;
            else
                istrAppDeadline = busGlobalFunctions.ConvertDateIntoDifFormat(ldtApplDeadlineDate);
        }

        public void GetYears(string astrTemplateName)
        {
            if(astrTemplateName != busConstant.RETIREMENT_APPLICATION_CANCELLATION_NOTICE)
            {
                if (!this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                {
                    if (astrTemplateName == busConstant.ONE_YEAR_BREAK_NOTIFICATION)
                    {
                        if ((this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count >= 2
                            && (this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_hours < 200)))
                        {
                            if (this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year == DateTime.Now.Year)
                            {
                                iintYear = this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year;
                                iintNextConsecutiveYear = DateTime.Now.Year + 1;
                            }
                            else
                            {
                                iintYear = this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year;
                                iintNextConsecutiveYear = DateTime.Now.Year;
                            }
                        }
                    }
                    else
                    {
                        if ((this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count >= 2
                            && this.lbusBenefitApplication.aclbPersonWorkHistory_MPI[this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IndexOf(this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last()) - 1].vested_hours < 200) && (this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_hours < 200))
                        {
                            iintYear = this.lbusBenefitApplication.aclbPersonWorkHistory_MPI[this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IndexOf(this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last()) - 1].year;
                            iintNextConsecutiveYear = this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year;
                            //Ticket #105926   
                            int lintLatestBisYear2 = 0;     //Latest BIS Year 2, if applicable   
                            int intNextQualifiedYr = 0;     //Next Qualified Year After Latest BIS Year 2, if applicable
                            cdoDummyWorkData lcdoDummyWorkData = this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(a => a.bis_years_count == 2).LastOrDefault();
                            if (lcdoDummyWorkData != null)
                            {
                                lintLatestBisYear2 = lcdoDummyWorkData.year;
                                cdoDummyWorkData lcdoDummyWorkData2 = this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(b => b.year > lintLatestBisYear2 && b.vested_hours >= 200).FirstOrDefault();
                                if (lcdoDummyWorkData2 != null)
                                {
                                    intNextQualifiedYr = lcdoDummyWorkData2.year;
                                }
                                if (lintLatestBisYear2 > 0 && intNextQualifiedYr == 0)
                                {
                                    istrLastBisYears = (lintLatestBisYear2 - 1).ToString() + "-" + (lintLatestBisYear2).ToString();
                                }
                            }
                        }
                    }
                }

            }
        }

        public void LoadPersonDROApplications()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadPersonDROApplication", new object[1] { this.icdoPerson.person_id });
            iclbQdroApplication = GetCollection<busQdroApplication>(ldtblist, "icdoDroApplication");

            iclbQdroApplication.ForEach(item =>
            {
                if (string.IsNullOrEmpty(item.icdoDroApplication.istrMemeber_Mpid))
                {
                    item.icdoDroApplication.istrMemeber_Mpid = this.icdoPerson.mpi_person_id;
                    item.icdoDroApplication.istrMemeber_Fullname = this.icdoPerson.istrFullName;
                }
                else if (string.IsNullOrEmpty(item.icdoDroApplication.istrAlternate_Payee_Mpid))
                {
                    item.icdoDroApplication.istrAlternate_Payee_Mpid = this.icdoPerson.mpi_person_id;
                    item.icdoDroApplication.istrAlternate_Payee_Fullname = this.icdoPerson.istrFullName;
                }
            });
        }

        public void LoadDeathNotifications()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadDeathNotificationDetails", new object[1] { this.icdoPerson.person_id });
            iclbDeathNotifications = GetCollection<busDeathNotification>(ldtblist, "icdoDeathNotification");
        }


        public void LoadParticipantPlan()
        {
            DataTable ldtblist = busPerson.Select("cdoPersonAccount.GetPlanForPersonOverview", new object[1] { this.icdoPerson.person_id });
            iclbPersonAccount = GetCollection<busPersonAccount>(ldtblist, "icdoPersonAccount");
        }

        public void LoadParticipantWorkFlows()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.GetBPMDetailsforPerson", new object[1] { this.icdoPerson.person_id });
            iclbPersonBPMflows = GetCollection<busBpmActivityInstanceHistory>(ldtblist, "icdoBpmActivityInstanceHistory");
            foreach (busBpmActivityInstanceHistory lbusActivityInstancehistory in iclbPersonBPMflows)
            {
                lbusActivityInstancehistory.LoadBpmActivityInstance();
                lbusActivityInstancehistory.ibusBpmActivityInstance.LoadBpmActivity();
                lbusActivityInstancehistory.ibusBpmActivityInstance.LoadBpmProcessInstance();
                lbusActivityInstancehistory.ibusBpmActivityInstance.ibusBpmProcessInstance.LoadBpmProcess();
            }
        }

        public void LoadPersonNotes()
        {
            iclbNotes = new Collection<busNotes>();
            DataTable ldtblist = busPerson.Select("cdoNotes.GetNotesforPersonOverview", new object[1] { this.icdoPerson.person_id });
            iclbNotes = GetCollection<busNotes>(ldtblist, "icdoNotes");
            if (iclbNotes != null)
                iclbNotes = iclbNotes.OrderByDescending(obj => obj.icdoNotes.created_date).ToList().ToCollection<busNotes>();
        }

        public void LoadWorkHistory()
        {
            lbusBenefitApplication = new busBenefitApplication();
            lbusBenefitApplication.ibusPerson = new busPerson();
            lbusBenefitApplication.ibusPerson.FindPerson(this.icdoPerson.person_id);
            lbusBenefitApplication.ibusPerson.LoadPersonAccounts();

            lbusBenefitApplication.aclbPersonWorkHistory_MPI = new Collection<cdoDummyWorkData>();
            lbusBenefitApplication.aclbPersonWorkHistory_IAP = new Collection<cdoDummyWorkData>();
            lbusBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();

            if ((!this.iclbBenefitApplication.IsNullOrEmpty() && this.iclbBenefitApplication.Count > 0) || (this.icdoPerson.date_of_death.IsNotNull() && this.icdoPerson.date_of_death != DateTime.MinValue))
            {
                //Ticket#132767
                if (this.iclbBenefitApplication.IsNotNull() && this.iclbBenefitApplication.Where(i => (i.icdoBenefitApplication.benefit_type_value == "RTMT" || i.icdoBenefitApplication.benefit_type_value == "DSBL") && i.icdoBenefitApplication.min_distribution_flag != busConstant.Flag_Yes).Count() > 0)
                {
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date = this.iclbBenefitApplication.Where(i => (i.icdoBenefitApplication.benefit_type_value == "RTMT" || i.icdoBenefitApplication.benefit_type_value == "DSBL") && i.icdoBenefitApplication.min_distribution_flag != busConstant.Flag_Yes).OrderBy(i => i.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitApplication.retirement_date;

                    if (this.icdoPerson.date_of_death.IsNotNull() && this.icdoPerson.date_of_death != DateTime.MinValue && this.icdoPerson.date_of_death < lbusBenefitApplication.icdoBenefitApplication.retirement_date)
                        lbusBenefitApplication.icdoBenefitApplication.retirement_date = this.icdoPerson.date_of_death;
                }
                else if (this.icdoPerson.date_of_death.IsNotNull() && this.icdoPerson.date_of_death != DateTime.MinValue)
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date = DateTime.Now;
                else
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            }
            else
                lbusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);

            lbusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_PersonOverView();
        }

        public void LoadPlanDetails(bool ablnFromService = false)  //added parameter For CRM Bug 9922
        {
            busAnnualBenefitSummaryOverview lbusAnnualBenefitSummaryOverview = new busAnnualBenefitSummaryOverview();
            lbusAnnualBenefitSummaryOverview.icdoPerson = this.icdoPerson;

            lbusAnnualBenefitSummaryOverview.LoadWorkHistory(true,0, ablnFromService);   //For CRM Bug 9922
            iblnEligibleForActiveIncrease = lbusAnnualBenefitSummaryOverview.iblnEligibleForActiveIncrease;//Temp
            this.lbusBenefitApplication = new busBenefitApplication();
            this.lbusBenefitApplication = lbusAnnualBenefitSummaryOverview.lbusBenefitApplication;
            iclcdoPersonAccountOverview = lbusAnnualBenefitSummaryOverview.iclcdoPersonAccountOverview;

            iclbPersonAccountOverview = new Collection<busPersonAccount>();
            foreach (cdoPersonAccount lcdoPersonAccount in iclcdoPersonAccountOverview)
            {
                iclbPersonAccountOverview.Add(new busPersonAccount() { icdoPersonAccount = lcdoPersonAccount });
            }
        }

        public DateTime GetWithDrawaldateForPlanDetails() // Fetching the withdrawal date of approved withdrawal apllication.
        {
            foreach (busBenefitApplication objbusBenefitApplication in iclbBenefitApplication)
            {
                if (objbusBenefitApplication.icdoBenefitApplication.benefit_type_value == "WDRL" && objbusBenefitApplication.icdoBenefitApplication.application_status_value == "APPR")
                {
                    return objbusBenefitApplication.icdoBenefitApplication.withdrawal_date;
                }
            }
            return DateTime.MinValue;
        }

        public bool IsShowAllWorkFlowBtnVisisble()
        {
            DataTable ldtblist = busPerson.Select("cdoActivityInstance.CountAllProcessInstanceHistoryByPerson", new object[1] { this.icdoPerson.person_id });
            if (ldtblist.Rows.Count > 0)
            {
                DataRow dtrow = ldtblist.Rows[0];
                int lintCount = Convert.ToInt16(dtrow[0]);
                if (lintCount > 100)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public bool IsDeathPreRetrAppExists()
        {
            busBenefitApplication lbusBenefitApplication = null;
            if (this.iclbBenefitApplication != null && this.iclbBenefitApplication.Count > 0 &&
                this.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT).Count() > 0)
                lbusBenefitApplication = this.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT).FirstOrDefault();
            if (lbusBenefitApplication == null)
                return false;
            return true;
        }

        public void LoadPayeeAccount()
        {
            DataTable ldtblistPayeeAcnt = busPerson.Select("cdoPayeeAccount.GetPayeeAccountForPersonOverview", new object[1] { this.icdoPerson.person_id });
            iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtblistPayeeAcnt, "icdoPayeeAccount");

            foreach (busPayeeAccount lbusPayeeAccount in iclbPayeeAccount)
            {
                if (lbusPayeeAccount.icdoPayeeAccount.istrPlanDescription == busConstant.IAP_PLAN)
                {
                    if (lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT ||
                        lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL ||
                        lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate = lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate;
                    }
                }
                else
                {
                    lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate = lbusPayeeAccount.icdoPayeeAccount.idtRTMTDate;
                }
            }
        }

        #region Commented Code (Added by Gagan not required here)
        //public string GetPlanParticipationStatus()   // To get the plan particicpant status.
        //{
        //    string lstrStatus = string.Empty;
        //    foreach (busBenefitApplication objbusBenefitApplication in iclbBenefitApplication)  // if any retirement found with approved status
        //    {
        //        if (objbusBenefitApplication.icdoBenefitApplication.benefit_type_value == "RTMT" && objbusBenefitApplication.icdoBenefitApplication.application_status_value == "APPR")
        //        {
        //           lstrStatus =  "Retired";
        //           return lstrStatus;
        //        }
        //        else if (objbusBenefitApplication.icdoBenefitApplication.benefit_type_value == "DDPR" && objbusBenefitApplication.icdoBenefitApplication.application_status_value == "APPR")
        //        {
        //            lstrStatus = "Deceased ";                     // if any pre-death notification found with approved status
        //            return lstrStatus;
        //        }
        //    }

        //    if (string.IsNullOrEmpty(lstrStatus))
        //    {                                                 // if any death notification found with approved status
        //        DataTable ldtdeathNotificationdate = busBase.Select("cdoDeathNotification.GetDateFromDeathNotification", new object[1] { this.icdoPerson.person_id });
        //        if (ldtdeathNotificationdate.Rows.Count > 0)
        //        {
        //            lstrStatus = "Deceased ";
        //            return lstrStatus;
        //        }
        //    }
        //    return lstrStatus;
        //}
        #endregion

        //ID:68932        
        public void LoadPensionVerificationHistory()
        {
            DataTable ldtbList = Select<cdoPensionVerificationHistory>(new string[1] {enmPerson.person_id.ToString() }, new object[1] { icdoPerson.person_id },null,null);
            iclbPensionVerificationHistory = GetCollection<busPensionVerificationHistory>(ldtbList, "icdoPensionVerificationHistory");
        }

        public void CheckDuplicatePlan(Hashtable ahstParams, ref ArrayList larrErrors)
        {
            utlError lobjError = null;

            if (ahstParams["icdoPensionVerificationHistory.verification_confirmation_letter_sent"] == "")
            {
                lobjError = AddError(5045, "");
                larrErrors.Add(lobjError);
                return;
            }
            else
            {
                DateTime ldtCurrentVerificationConfirmationLetterSent = Convert.ToDateTime(ahstParams["icdoPensionVerificationHistory.verification_confirmation_letter_sent"]);
                if (iclbPensionVerificationHistory != null && iclbPensionVerificationHistory.Count > 0)
                {
                    foreach (busPensionVerificationHistory lbusPensionVerificationHistory in iclbPensionVerificationHistory)
                    {
                        if (ldtCurrentVerificationConfirmationLetterSent <= lbusPensionVerificationHistory.icdoPensionVerificationHistory.verification_confirmation_letter_sent)
                        {
                            lobjError = AddError(5143, "");
                            larrErrors.Add(lobjError);
                            break;
                        }
                    }
                }

                if (Convert.ToString(ahstParams["icdoPensionVerificationHistory.received"]) == "Y" && Convert.ToDateTime(ahstParams["icdoPensionVerificationHistory.verification_confirmation_letter_sent"]) > DateTime.Now)
                {
                    lobjError = AddError(6092, "");
                    larrErrors.Add(lobjError);
                }
                if (Convert.ToString(ahstParams["icdoPensionVerificationHistory.received"]) == "Y" && Convert.ToString(ahstParams["icdoPensionVerificationHistory.sent"]) == "Y")
                {
                    lobjError = AddError(6093, "");
                    larrErrors.Add(lobjError);
                }
            }
        }

        #endregion
        protected void CheckAlreadyVestedOverview(cdoPersonAccount objcdoPersonAccount)
        {
            this.ibusPersonAccountEligibilityOverview = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
            int lintAccountId = iclcdoPersonAccountOverview.Where(plan => plan.istrPlanCode == objcdoPersonAccount.istrPlanCode).First().person_account_id;
            if (lintAccountId > 0)
            {
                DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });
                if (ldtbPersonAccountEligibility.Rows.Count > 0)
                {
                    this.ibusPersonAccountEligibilityOverview.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                    if (this.ibusPersonAccountEligibilityOverview.icdoPersonAccountEligibility.vested_date.IsNotNull() && this.ibusPersonAccountEligibilityOverview.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                    {
                        objcdoPersonAccount.istrVested = true;
                        objcdoPersonAccount.dtVestedDate = this.ibusPersonAccountEligibilityOverview.icdoPersonAccountEligibility.vested_date;
                    }
                    if (this.ibusPersonAccountEligibilityOverview.icdoPersonAccountEligibility.forfeiture_date.IsNotNull() && this.ibusPersonAccountEligibilityOverview.icdoPersonAccountEligibility.forfeiture_date != DateTime.MinValue)
                    {
                        objcdoPersonAccount.dtForfeitureDate = this.ibusPersonAccountEligibilityOverview.icdoPersonAccountEligibility.forfeiture_date;
                    }
                }

            }
        }

        #region override
        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            GetRetirementAndApplDeadlineDate();
            GetYears(astrTemplateName);
            DateTime ldtCurrentDate = System.DateTime.Now;
            istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
            if (iclbPersonAccount == null)
            {
                LoadPersonAccounts();
            }
            if (this.iclbPersonAccount != null && this.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
            {
                EmployeeContribution(this.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
            }
            if (this.iclbPersonAccount != null && this.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
            {
                IAPBalace(this.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
            }

            //if (astrTemplateName == busConstant.RETIREE_HEALTH_PACKET)
            //{
            //    GetRetireeHealthHours();

            //    if(this.iclbHealthWorkHistory.Count > 0)
            //    {
            //        ldecHealthQualifiedHours = (from item in this.iclbHealthWorkHistory select item.idecTotalHealthHours).Sum();
            //        ldecHealthQualifiedYears = (from item in this.iclbHealthWorkHistory orderby item.year select item.iintHealthCount).Last();

            //    }
            //}

            if (astrTemplateName == busConstant.Disability_Retirement_Not_Enough_HRS_YRS)
            {
                PopulateAnnualBenefitSummaryOverview();
            }

            if (astrTemplateName == busConstant.SUBPOENA_RESPONSE_CERTIFICATION_OF_RECORDS)
            {
                PopulateAnnualBenefitSummaryOverview();
                if (ibusAnnualBenefitSummaryOverview.ibusBenefitApplication == null)
                {
                    ibusAnnualBenefitSummaryOverview.ibusBenefitApplication = new busBenefitApplication();
                }
                ibusIAPAllocationDetailPersonOverview = new busIAPAllocationDetailPersonOverview();

                if (ibusIAPAllocationDetailPersonOverview.FindPerson(icdoPerson.person_id))
                {
                    DataTable ldtblist = busPerson.Select("cdoPersonAccount.LoadPersonAccountbyPlanId", new object[2] { ibusIAPAllocationDetailPersonOverview.icdoPerson.person_id, busConstant.IAP_PLAN_ID });

                    if (ldtblist.Rows.Count > 0)
                    {
                        ibusIAPAllocationDetailPersonOverview.LoadIAPAllocationDetails(Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()]));
                    }
                    else
                    {
                        ibusIAPAllocationDetailPersonOverview.iclbIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
                    }
                }
                //if (ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI != null)
                //{
                //    foreach (cdoDummyWorkData lcdoDummyWorkData in ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI)
                //    {
                //        if (lcdoDummyWorkData.year != 0)
                //        {
                //            lcdoDummyWorkData.BeginingDate = busGlobalFunctions.GetFirstDateOfComputationYear(lcdoDummyWorkData.year);
                //            lcdoDummyWorkData.EndingDate = busGlobalFunctions.GetLastDateOfComputationYear(lcdoDummyWorkData.year);
                //        }
                //    }
                //}
            }

            //RequestID: 68932
            if (astrTemplateName == busConstant.PENSION_VERIFICATION_HISTORY_RESUMPTION_LETTER)
            {
                ldtResumptionDate = busGlobalFunctions.GetFirstDayofMonth(DateTime.Now.AddMonths(1));
            }

            if (astrTemplateName == busConstant.PENSION_VERIFICATION_HISTORY_THIRTY_DAYS_LETTER)
            {
                ldtResumptionDate = Convert.ToDateTime(DateTime.Now.Month.ToString() + "/15/" + DateTime.Now.Year.ToString());  //rid 80600
            }
        }

        private void PopulateAnnualBenefitSummaryOverview()
        {
            ibusAnnualBenefitSummaryOverview = new busAnnualBenefitSummaryOverview();
            if (ibusAnnualBenefitSummaryOverview.FindPerson(icdoPerson.person_id))
            {
                //lobjbusAnnualBenefitSummaryOverview.LoadInitialData();
                ibusAnnualBenefitSummaryOverview.LoadWorkHistory();

                lintTotalqualifiedYearsCnt = ibusAnnualBenefitSummaryOverview.lintTotalqualifiedYearsCount;
                ldecTotalQualifiedAccuruedBft = ibusAnnualBenefitSummaryOverview.ldecTotalQualifiedAccuruedBenefit;
                ldecTotalCreditedHrs = ibusAnnualBenefitSummaryOverview.ldecTotalCreditedHours;
                lintBISYearCnt = ibusAnnualBenefitSummaryOverview.lintBISYearCount;

                ibusAnnualBenefitSummaryOverview.GetTotalHours();
                //lobjbusAnnualBenefitSummaryOverview.LoadAnnualBenefitSummaryOverview();
            }
        }

        public void EmployeeContribution(int aintPersonAccountId)
        {
            decimal ldecEEContributionAmount = busConstant.ZERO_DECIMAL;
            decimal ldecEEInterestAmount = busConstant.ZERO_DECIMAL;
            decimal ldecUVHPAmount = busConstant.ZERO_DECIMAL;
            decimal ldecUVHPInterestAmount = busConstant.ZERO_DECIMAL;
            DateTime ldtTransactionDate;

            DataTable ldtbEEandUVHPContributionAmount = busBase.Select("cdoPersonAccountRetirementContribution.GetTotalEEandUVHPContributionForMPIPlan", new object[] { aintPersonAccountId });
            if (ldtbEEandUVHPContributionAmount.Rows.Count > 0)
            {
                ldecEEContributionAmount = Convert.ToDecimal(Convert.ToBoolean(ldtbEEandUVHPContributionAmount.Rows[0][0].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbEEandUVHPContributionAmount.Rows[0][0]);

                ldecEEInterestAmount = Convert.ToDecimal(Convert.ToBoolean(ldtbEEandUVHPContributionAmount.Rows[0][1].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbEEandUVHPContributionAmount.Rows[0][1]);

                ldecUVHPAmount = Convert.ToDecimal(Convert.ToBoolean(ldtbEEandUVHPContributionAmount.Rows[0][2].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbEEandUVHPContributionAmount.Rows[0][2]);

                ldecUVHPInterestAmount = Convert.ToDecimal(Convert.ToBoolean(ldtbEEandUVHPContributionAmount.Rows[0][3].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtbEEandUVHPContributionAmount.Rows[0][3]);

                idecEEContributionAmount = ldecEEContributionAmount;
                idecSumEEUVHPAmount = ldecEEContributionAmount + ldecEEInterestAmount + ldecUVHPAmount + ldecUVHPInterestAmount;

                ldtTransactionDate = Convert.ToDateTime(Convert.ToBoolean(ldtbEEandUVHPContributionAmount.Rows[0][4].IsDBNull()) ? DateTime.MinValue : ldtbEEandUVHPContributionAmount.Rows[0][4]);

                istrTransactionDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtTransactionDate);

            }
        }
        public void IAPBalace(int aintPersonAccountId)
        {


            DataTable ldtIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetTotalIAPAccountBalanceForIap", new object[] { aintPersonAccountId });
            if (ldtIAPBalance.Rows.Count > 0)
            {
                idecIAPBalance = Convert.ToDecimal(Convert.ToBoolean(ldtIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]);
                //HS23
                idecTotalIAPBalance = (Convert.ToDecimal(Convert.ToBoolean(ldtIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) +
                    Convert.ToDecimal(Convert.ToBoolean(ldtIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtIAPBalance.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]) +
                    Convert.ToDecimal(Convert.ToBoolean(ldtIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : ldtIAPBalance.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]));
            }
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busPersonAccountBeneficiary)
            {
                busPersonAccountBeneficiary lbusPersonAccountBeneficiary = (busPersonAccountBeneficiary)aobjBus;

                lbusPersonAccountBeneficiary.ibusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
                lbusPersonAccountBeneficiary.ibusRelationship.icdoRelationship.LoadData(adtrRow);
            }
            else if (aobjBus is busPersonDependent)
            {
                busPersonDependent lbusPersonDependent = (busPersonDependent)aobjBus;
                lbusPersonDependent.ibusPersonDependent = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonDependent.ibusPersonDependent.icdoPerson.LoadData(adtrRow);
            }

            if (aobjBus is busActivityInstance)
            {
                busActivityInstance lbusActivityInstance = (busActivityInstance)aobjBus;

                lbusActivityInstance.ibusActivity = new busActivity { icdoActivity = new cdoActivity() };
                lbusActivityInstance.ibusActivity.ibusRoles = new busRoles { icdoRoles = new cdoRoles() };

                if (!Convert.IsDBNull(adtrRow["Display_Name"]))
                {
                    lbusActivityInstance.ibusActivity.icdoActivity.display_name = adtrRow["Display_Name"].ToString();
                }

                lbusActivityInstance.ibusProcessInstance = new busProcessInstance { icdoProcessInstance = new cdoProcessInstance() };
                lbusActivityInstance.ibusProcessInstance.icdoProcessInstance.process_instance_id = lbusActivityInstance.icdoActivityInstance.process_instance_id;
                lbusActivityInstance.ibusProcessInstance.ibusProcess = new busProcess { icdoProcess = new cdoProcess() };
                //lobjActivityInstance.ibusProcessInstance.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                //lobjActivityInstance.ibusProcessInstance.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };

                lbusActivityInstance.ibusProcessInstance.ibusWorkflowRequest = new busWorkflowRequest { icdoWorkflowRequest = new cdoWorkflowRequest() };


                if (!Convert.IsDBNull(adtrRow["Process_Name"]))
                {
                    lbusActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.name = adtrRow["Process_Name"].ToString();
                    lbusActivityInstance.icdoActivityInstance.istrProcessName = adtrRow["Process_Name"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["Process_Description"]))
                {
                    lbusActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.description = adtrRow["Process_Description"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["Source_Description"]))
                {
                    lbusActivityInstance.ibusProcessInstance.ibusWorkflowRequest.icdoWorkflowRequest.source_description = adtrRow["Source_Description"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["STATUS_DESCRIPTION"]))
                {
                    lbusActivityInstance.ibusProcessInstance.icdoProcessInstance.status_description = adtrRow["STATUS_DESCRIPTION"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["START_DATE"]))
                {
                    lbusActivityInstance.icdoActivityInstance.START_DATE = Convert.ToDateTime(adtrRow["START_DATE"]);
                }


                if (!Convert.IsDBNull(adtrRow["END_DATE"]))
                {
                    lbusActivityInstance.icdoActivityInstance.END_DATE = Convert.ToDateTime(adtrRow["END_DATE"]);
                }


                if (!Convert.IsDBNull(adtrRow["UserId"]))
                    lbusActivityInstance.icdoActivityInstance.UserId = adtrRow["UserId"].ToString();
                {
                }
            }
        }

        #endregion

        public override void BeforePersistChanges()
        {
            this.iclbNotes.ForEach(item =>
            {
                if (item.icdoNotes.person_id == 0)
                    item.icdoNotes.person_id = this.icdoPerson.person_id;
                item.icdoNotes.form_id = busConstant.Form_ID;
                item.icdoNotes.form_value = busConstant.PERSON_OVERVIEW_MAINTAINANCE_FORM;
            });
        }

        public Collection<cdoDummyWorkData> iclbHealthWorkHistory { get; set; }
        public busSystemManagement iobjSystemManagement { get; set; }
        public bool iblnFromBatch { get; set; }

        public void GetRetireeHealthHours()
        {
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            SqlParameter[] parameters = new SqlParameter[1];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            param1.Value = icdoPerson.istrSSNNonEncrypted;
            parameters[0] = param1;

            //SqlParameter param2 = new SqlParameter("@BATCH_RUN_DATE", DbType.String);
            //param2.Value = iobjSystemManagement.icdoSystemManagement.batch_date;
            //parameters[1] = param2;            

            DataTable ldtPersonHealthWorkHistory = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetHealthWorkData", lstrLegacyDBConnetion, null, parameters);
            if (ldtPersonHealthWorkHistory.Rows.Count > 0)
            {
                busPersonBatchFlags lbusPersonBatchFlags = new busPersonBatchFlags { icdoPersonBatchFlags = new cdoPersonBatchFlags() };
                iclbHealthWorkHistory = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtPersonHealthWorkHistory);

                if (lbusBenefitApplication == null || lbusBenefitApplication.ibusPerson == null)
                {
                    lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusBenefitApplication.ibusPerson.FindPerson(icdoPerson.person_id);
                }

                if (lbusBenefitApplication.idecAge.IsNull() || lbusBenefitApplication.idecAge == 0)
                {
                    lbusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(icdoPerson.idtDateofBirth, DateTime.Now);
                }

                lbusBenefitApplication.ProcessWorkHistoryforBISandForfieture(iclbHealthWorkHistory, busConstant.MPIPP);
            }
        }

        //Abhi-Health Changes
        public void CheckRetireeHealthEligibilityAndUpdateFlag()        //PROD PIR 59 Changes in Rules and order of rules
        {
            if (!iclbHealthWorkHistory.IsNullOrEmpty())
            {
                #region Check for Health Eligibility 
                bool lblnHealthElgible = CheckForHealthEligibility();
                if (lblnHealthElgible)
                {
                    icdoPerson.health_eligible_flag = busConstant.FLAG_YES;
                    icdoPerson.Update();
                }
                else if (!lblnHealthElgible && this.icdoPerson.health_eligible_flag != busConstant.FLAG_NO)
                {
                    icdoPerson.health_eligible_flag = busConstant.FLAG_NO;
                    icdoPerson.Update();
                }
                #endregion

                #region Second Part of Batch (Checking if Person Account Exists)
                if (iclbHealthWorkHistory.Where(item => item.idcPensionHours_healthBatch > 0).Count() >= 1 || iclbHealthWorkHistory.Where(item => item.idcIAPHours_healthBatch > 0).Count() >= 1
                    || iclbHealthWorkHistory.Where(item => item.idecTotalHealthHours > 0).Count() >= 1)
                {

                    if (iclbHealthWorkHistory.Where(item => item.idcPensionHours_healthBatch > 0).Count() >= 1 &&
                        lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(o => o.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault() == null)
                    {
                        busPersonAccount lbusPersonAccount = new busPersonAccount();
                        lbusPersonAccount.InsertDataInPersonAccount(icdoPerson.person_id, busConstant.MPIPP_PLAN_ID,
                                            iobjSystemManagement.icdoSystemManagement.batch_date);
                    }

                    if (iclbHealthWorkHistory.Where(item => item.idcIAPHours_healthBatch > 0).Count() >= 1 &&
                        lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(o => o.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault() == null)
                    {
                        busPersonAccount lbusPersonAccount = new busPersonAccount();
                        lbusPersonAccount.InsertDataInPersonAccount(icdoPerson.person_id, busConstant.IAP_PLAN_ID,
                                                    iobjSystemManagement.icdoSystemManagement.batch_date);
                    }

                    if (iclbHealthWorkHistory.Where(item => item.idecTotalHealthHours > 0).Count() >= 1 &&
                        lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(o => o.icdoPersonAccount.plan_id == busConstant.LIFE_PLAN_ID).FirstOrDefault() == null && iblnFromBatch)
                    {
                        busPersonAccount lbusPersonAccount = new busPersonAccount();
                        lbusPersonAccount.InsertDataInPersonAccount(icdoPerson.person_id, busConstant.LIFE_PLAN_ID,
                                                   iobjSystemManagement.icdoSystemManagement.batch_date);
                    }
                    else if (iclbHealthWorkHistory.Where(item => item.idecTotalHealthHours > 0).Count() >= 1 && !iblnFromBatch)
                    {
                        DataTable ldtPersonAccount = SelectWithOperator<cdoPersonAccount>(new string[2] { "person_id", "plan_id" }, new string[2] { "=", "=" }, new object[2] { icdoPerson.person_id, busConstant.LIFE_PLAN_ID }, null);
                        if (ldtPersonAccount == null || ldtPersonAccount.Rows.Count <= 0)
                        {
                            busPersonAccount lbusPersonAccount = new busPersonAccount();
                            lbusPersonAccount.InsertDataInPersonAccount(icdoPerson.person_id, busConstant.LIFE_PLAN_ID,
                                                  DateTime.Now);
                        }
                    }

                }
                #endregion
            }
            else
            {
                icdoPerson.health_eligible_flag = busConstant.FLAG_NO;
                icdoPerson.Update();
            }
        }

        public bool CheckForHealthEligibility()
        {
            #region 5 Rules for HEALTH ELIGIBILITY
            DateTime ldtRuleAge = icdoPerson.date_of_birth;
            bool lblnHealthElgible = false;
            if (!lblnHealthElgible)
            {
                #region RULE 4
                if (iclbHealthWorkHistory.Last().iintHealthCount >= 30 && (from item in iclbHealthWorkHistory select item.idecTotalHealthHours).Sum() >= 60000)
                {
                    lblnHealthElgible = true;
                    icdoPerson.istrRule = busConstant.RULE_4;
                }
                #endregion
            }
            if (!lblnHealthElgible)
            {
                #region RULE 3
                if (iclbHealthWorkHistory.Last().iintHealthCount >= 30 && (from item in iclbHealthWorkHistory select item.idecTotalHealthHours).Sum() >= 55000 && (from item in iclbHealthWorkHistory select item.idecTotalHealthHours).Sum() < 60000)
                {
                    lblnHealthElgible = true;
                    icdoPerson.istrRule = busConstant.RULE_3;
                }
                #endregion
            }
            if (!lblnHealthElgible)
            {
                #region RULE 2
                if (iclbHealthWorkHistory.Last().iintHealthCount >= 20 //&& iclbHealthWorkHistory.Last().iintHealthCount < 30 
                    && (from item in iclbHealthWorkHistory select item.idecTotalHealthHours).Sum() >= 20000)
                {
                    lblnHealthElgible = true;
                    icdoPerson.istrRule = busConstant.RULE_2;
                }
                #endregion
            }

            if (!lblnHealthElgible)
            {
                #region RULE 1
                if (iclbHealthWorkHistory.Last().iintHealthCount >= 15 //&& iclbHealthWorkHistory.Last().iintHealthCount < 20 
                    && (from item in iclbHealthWorkHistory select item.idecTotalHealthHours).Sum() >= 20000)
                {
                    //Prod PIR 221 : Rule to be added to take Qualified years after year in which person turns 40
                    //Ticket 143320 - Adding upper year limit for one qualified year (2000 - 2015)
                    int lint40Year = this.icdoPerson.date_of_birth.AddYears(40).Year;
                    if (iclbHealthWorkHistory.Where(item => item.year > lint40Year && item.idecTotalHealthHours >= 400).Count() >= 3 && iclbHealthWorkHistory.Where(item => item.year > 1999 && item.year < 2016 && item.idecTotalHealthHours >= 400).Count() >= 1)
                    {
                        lblnHealthElgible = true;
                        icdoPerson.istrRule = busConstant.RULE_1;
                    }
                    //if (iclbHealthWorkHistory.Where(item => item.age >= 40 && item.idecTotalHealthHours >= 400).Count() >= 3 && iclbHealthWorkHistory.Where(item => item.year > 1999 && item.idecTotalHealthHours >= 400).Count() >= 1)
                    //{
                    //    lblnHealthElgible = true;
                    //    icdoPerson.istrRule = busConstant.RULE_1;
                    //}
                }
                #endregion
            }
            if (!lblnHealthElgible)
            {
                #region RULE 5
                //added condition for IF Disabled then 
                if (iclbHealthWorkHistory.Last().iintHealthCount >= 10 && (from item in iclbHealthWorkHistory select item.idecTotalHealthHours).Sum() >= 10000)
                {
                    //int lintCount = (int)DBFunction.DBExecuteScalar("cdoPerson.GetApprovedDisabilityAppl", new object[1] { lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                    //if (lintCount >= 1)
                    DataTable ldtblistCount = busPerson.Select("cdoPerson.GetApprovedDisabilityAppl", new object[1] { lbusBenefitApplication.ibusPerson.icdoPerson.person_id });
                    if (ldtblistCount.Rows.Count > 0 && Convert.ToInt32(ldtblistCount.Rows[0][0]) >= 1)
                    {
                        lblnHealthElgible = true;
                        icdoPerson.istrRule = busConstant.RULE_5;
                    }

                }

                #endregion
            }

            return lblnHealthElgible;

            #endregion

        }

        public decimal CheckWithdrawalHours(int aintPlanID)
        {
            #region // PROD PIR 205
            decimal ldecwithdrawalhours = 0.0M;

            DataTable ldtbCheckPersonHasWithdrawal = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawal", new object[1] { lbusBenefitApplication.ibusPerson.icdoPerson.person_id });
            if (ldtbCheckPersonHasWithdrawal != null && ldtbCheckPersonHasWithdrawal.Rows.Count > 0)
            {
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == aintPlanID).FirstOrDefault().icdoPersonAccount.person_account_id);

                ldtWihtdrawalEffectiveDate = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                              orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                                              select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();

                if (ldtWihtdrawalEffectiveDate != DateTime.MinValue)
                {
                    busCalculation lbusCalculation = new busCalculation();
                    ldecwithdrawalhours = lbusCalculation.GetWorkDataAfterDate(lbusBenefitApplication.ibusPerson.icdoPerson.ssn, ldtWihtdrawalEffectiveDate.Year, busConstant.MPIPP_PLAN_ID, ldtWihtdrawalEffectiveDate);
                }
            }
            #endregion

            return ldecwithdrawalhours;
        }

        //public decimal idecIAPBalance { get; set; }
        //public busAnnualBenefitSummaryOverview ibusAnnualBenefitSummaryOverview { get; set; }
        public busIAPAllocationDetailPersonOverview ibusIAPAllocationDetailPersonOverview { get; set; }
        public int lintTotalHealthYearsCount { get; set; }
        public int lintTotalqualifiedYearsCount { get; set; }
        public int lintTotalvestedYearsCount { get; set; }
        public string iintIAPAllocationYear { get; set; }

        #region Process WorkHistory For Person OverView Summary
        public void ProcessWorkHistoryForPersonOverviewSummary()
        {
            #region Enhancement Logic
            if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
            {
                busCalculation lbusCalculation = new busCalculation();
                busAnnualBenefitSummaryOverview lobjbusAnnualBenefitSummaryOverview = new busAnnualBenefitSummaryOverview();
                lbusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                aclbPersonWorkHistory_Local = new Collection<cdoDummyWorkData>();
                cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                lclbLocalMergerCdoDummyWorkdata = new cdoDummyWorkData();
                Collection<cdoPersonAccountRetirementContribution> lclbRetCont = new Collection<cdoPersonAccountRetirementContribution>();

                int iintLocalQualifiedYearsCount = 0;

                #region Total Accured Benefit
                if (lbusBenefitApplication.ibusPerson.iclbPersonAccount.Count() > 0)
                {
                    lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);

                    lbusBenefitCalculationHeader.ibusPerson = lbusBenefitApplication.ibusPerson;
                    lobjbusAnnualBenefitSummaryOverview.lbusBenefitApplication = new busBenefitApplication();
                    lobjbusAnnualBenefitSummaryOverview.lbusBenefitApplication = lbusBenefitApplication;
                    lbusBenefitCalculationHeader.LoadAllRetirementContributions(null);
                    lobjbusAnnualBenefitSummaryOverview.iclbPersonAccount = lbusBenefitApplication.ibusPerson.iclbPersonAccount;

                    if (this.icdoPerson.date_of_death != DateTime.MinValue)
                    {
                        if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0)
                        {
                            DateTime ldtMaxEffectiveDate = DateTime.MinValue;
                            if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.contribution_type_value == "EE" ||
                                    item.icdoPersonAccountRetirementContribution.contribution_type_value == "UVHP").Count() > 0)
                                ldtMaxEffectiveDate = lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.contribution_type_value == "EE" ||
                                       item.icdoPersonAccountRetirementContribution.contribution_type_value == "UVHP").Max(obj => obj.icdoPersonAccountRetirementContribution.effective_date);
                            if (ldtMaxEffectiveDate.Year <= this.icdoPerson.date_of_death.Year)
                            {
                                if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year > this.icdoPerson.date_of_death.Year).Count() > 0)
                                    lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year <= this.icdoPerson.date_of_death.Year).ToList().ToCollection();
                            }
                        }
                    }

                    if ((from obj in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                         where (obj.icdoPersonAccountRetirementContribution.contribution_type_value != "EE"
                         && obj.icdoPersonAccountRetirementContribution.contribution_type_value != "UVHP"
                         && obj.icdoPersonAccountRetirementContribution.contribution_type_value != "LCL")
                         select obj.icdoPersonAccountRetirementContribution.computational_year).Count() > 0)
                        iintIAPAllocationYear = Convert.ToString((from obj in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                  where (obj.icdoPersonAccountRetirementContribution.contribution_type_value != "EE"
                                                                  && obj.icdoPersonAccountRetirementContribution.contribution_type_value != "UVHP"
                                                                  && obj.icdoPersonAccountRetirementContribution.contribution_type_value != "LCL")
                                                                  select new { obj.icdoPersonAccountRetirementContribution.computational_year }).OrderBy(x => x.computational_year).Last().computational_year);

                    decimal ldecUnreducedBenefitAmount = lobjbusAnnualBenefitSummaryOverview.LoadAccruedBenefitPerYear(lbusBenefitApplication, lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date);
                }
                #endregion

                #region PROD PIR 205
                DateTime ldtEffectiveDate = DateTime.MinValue;
                DateTime ldtEffectiveDateforUVHP = DateTime.MinValue;
                DateTime ldtWdrlDateBefore1976 = DateTime.MinValue;
                decimal ldecwihtodrawalhours = 0.0M;
                int tempLocalMergerYear = 0;
                decimal ldecActualHoursBeforeMerger = 0.0M;


                DataTable ldtbCheckPersonHasWithdrawal = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawal", new object[1] { lbusBenefitApplication.ibusPerson.icdoPerson.person_id });
                if (ldtbCheckPersonHasWithdrawal != null && ldtbCheckPersonHasWithdrawal.Rows.Count > 0)
                {
                    #region //Removed as per request from MPI : Only SHOW the hours after withdrawal irrespective of the vested date
                    //if(lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                    //    ldtEffectiveDate = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                    //                        where item.Field<DateTime>("WITHDRAWAL_DATE") < lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date
                    //                        select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();
                    //else 
                    #endregion

                    ldtEffectiveDate = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                        where item.Field<string>("CONTRIBUTION_TYPE_VALUE") == "EE"
                                        orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                                        select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();

                    ldtEffectiveDateforUVHP = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                               where item.Field<string>("CONTRIBUTION_TYPE_VALUE") == "UVHP"
                                               orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                                               select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();

                    ldtWdrlDateBefore1976 = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                             where item.Field<DateTime>("WITHDRAWAL_DATE").Year < 1976
                                             orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                                             select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();

                    if (ldtEffectiveDate != DateTime.MinValue)
                    {
                        ldecwihtodrawalhours = lbusCalculation.GetWorkDataAfterDate(lbusBenefitApplication.ibusPerson.icdoPerson.ssn, ldtEffectiveDate.Year, busConstant.MPIPP_PLAN_ID, ldtEffectiveDate);
                    }
                }
                #endregion

                // int iintLocalQualifiedYearsCount = 0;
                if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0
                    && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count() > 0)
                {
                    #region EE_UVHP_INTEREST_CONTRIBUTION and Wihtdrawals
                    #region Padding
                    if ((ldtEffectiveDate != DateTime.MinValue && ldtEffectiveDate != null && ldtEffectiveDate.Year > lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year)
                        || (ldtEffectiveDateforUVHP != DateTime.MinValue && ldtEffectiveDateforUVHP != null && ldtEffectiveDateforUVHP.Year > lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year))
                    {
                        Collection<cdoDummyWorkData> lclbtempcdoDummyWorkDataforPadding = new Collection<cdoDummyWorkData>();

                        DateTime lstTempEffectiveDateTime = ldtEffectiveDate != DateTime.MinValue ? ldtEffectiveDate : ldtEffectiveDateforUVHP;
                        if (lstTempEffectiveDateTime != DateTime.MinValue)
                        {
                            int healthcount = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                            for (int i = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year; i < lstTempEffectiveDateTime.Year; i++)
                            {
                                cdoDummyWorkData tempcdoDummyWorkDataforPadding = new cdoDummyWorkData();
                                tempcdoDummyWorkDataforPadding.year = i + 1;
                                tempcdoDummyWorkDataforPadding.iintHealthCount = healthcount;
                                tempcdoDummyWorkDataforPadding.qualified_years_count = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                tempcdoDummyWorkDataforPadding.vested_years_count = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count;
                                if (tempcdoDummyWorkDataforPadding.year == lstTempEffectiveDateTime.Year
                                    && (ldtEffectiveDateforUVHP != DateTime.MinValue && ldtEffectiveDateforUVHP.Year == tempcdoDummyWorkDataforPadding.year))
                                {
                                    tempcdoDummyWorkDataforPadding.comments = "UVHP Withdrawn on " + lstTempEffectiveDateTime.ToShortDateString();
                                }
                                lbusBenefitApplication.aclbPersonWorkHistory_MPI.Add(tempcdoDummyWorkDataforPadding);
                            }
                        }
                        lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(i => i.year).ToList().ToCollection();
                    }
                    #endregion

                    decimal adectotalaccuredbenefitamount = 0.0M;
                    int intSequenceNumber = 0;
                    DateTime ldtPersonWithdrawalDate = DateTime.MinValue;
                    busAnnualBenefitSummaryOverview lbusAnnualBenefitSummaryOverview = new busAnnualBenefitSummaryOverview();
                    lbusAnnualBenefitSummaryOverview.lclbtempCdoDummyWorkdata = new Collection<cdoDummyWorkData>();

                    foreach (cdoDummyWorkData item in lbusBenefitApplication.aclbPersonWorkHistory_MPI)
                    {
                        intSequenceNumber++;
                        item.intSequenceNumber = intSequenceNumber;

                        if (ldtbCheckPersonHasWithdrawal.Rows.Count > 0 && ldtbCheckPersonHasWithdrawal != null)
                        {
                            ldtPersonWithdrawalDate = (from itemWithdrawal in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                                       where itemWithdrawal.Field<string>("CONTRIBUTION_TYPE_VALUE") == "EE"
                                                       && itemWithdrawal.Field<DateTime>("WITHDRAWAL_DATE").Year == item.year
                                                       orderby itemWithdrawal.Field<DateTime>("WITHDRAWAL_DATE") descending
                                                       select itemWithdrawal.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();

                        }

                        adectotalaccuredbenefitamount = adectotalaccuredbenefitamount + item.idecBenefitAmount;
                        item.idectotalBenefitAmount = adectotalaccuredbenefitamount;
                        item.idecTempqualified_hours = item.qualified_hours;


                        if (ldtPersonWithdrawalDate == DateTime.MinValue)
                        {
                            if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(i => i.icdoPersonAccountRetirementContribution.computational_year == item.year).Count() > 0)
                            {
                                var EEUVHPContributionandInterest = (from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                     where items.icdoPersonAccountRetirementContribution.computational_year == item.year
                                                                     select items);
                                if (EEUVHPContributionandInterest != null)
                                {
                                    item.idecEEContribution = EEUVHPContributionandInterest.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                                    item.idecEEInterest = EEUVHPContributionandInterest.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_int_amount);
                                    item.idecUVHPContribution = EEUVHPContributionandInterest.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_amount);
                                    item.idecUVHPInterest = EEUVHPContributionandInterest.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_int_amount);
                                }
                            }
                        }

                        #region Person Account with Withdrawal year
                        else
                        {

                            if (lbusPersonAccountEligibility != null)
                            {
                                #region Pre-Withdrawal Item
                                decimal idecAccuredBenefitTillWithdrawalDate = 0.0M;
                                decimal idecEEDerivedTillWithdrawalDate = 0.0M;

                                if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(i => i.icdoPersonAccountRetirementContribution.transaction_type_value == "PMNT" && i.icdoPersonAccountRetirementContribution.contribution_type_value == "EE").Count() > 0)
                                {
                                    Collection<busPersonAccountRetirementContribution> lclbRetContributionTiedToWithdrawal = null;
                                    lclbRetContributionTiedToWithdrawal = lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(ii => ii.icdoPersonAccountRetirementContribution.transaction_type_value == "PMNT" && ii.icdoPersonAccountRetirementContribution.contribution_type_value == "EE" && ii.icdoPersonAccountRetirementContribution.effective_date == ldtPersonWithdrawalDate).ToList().ToCollection();

                                    if (lclbRetContributionTiedToWithdrawal != null && lclbRetContributionTiedToWithdrawal.Count > 0)
                                    {
                                        busPersonAccountRetirementContribution lbusPersonAccountRetCont = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                                        lbusPersonAccountRetCont = lclbRetContributionTiedToWithdrawal.FirstOrDefault();
                                        idecAccuredBenefitTillWithdrawalDate = lbusCalculation.GetAccruedBenefitTillWithdrawalDate(lbusBenefitApplication.aclbPersonWorkHistory_MPI, lbusBenefitApplication.ibusPerson, lbusPersonAccountRetCont, lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year);
                                    }
                                }
                                idecEEDerivedTillWithdrawalDate = lbusCalculation.CalculateEEDerivedTillWithdrawalDate(item.idecEEContribution, item.idecEEInterest, item.age, ldtPersonWithdrawalDate);
                                item.idectotalBenefitAmount = (idecAccuredBenefitTillWithdrawalDate + idecEEDerivedTillWithdrawalDate) + item.idecBenefitAmount;
                                adectotalaccuredbenefitamount = item.idectotalBenefitAmount;

                                var EEUVHPContributionandInterestforPreWDRL = (from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                               where items.icdoPersonAccountRetirementContribution.computational_year == item.year
                                                                               && items.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT"
                                                                               && items.icdoPersonAccountRetirementContribution.effective_date < ldtPersonWithdrawalDate
                                                                               select items);

                                if (EEUVHPContributionandInterestforPreWDRL != null)
                                {
                                    item.idecEEContribution = EEUVHPContributionandInterestforPreWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                                    item.idecEEInterest = EEUVHPContributionandInterestforPreWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_int_amount);
                                    item.idecUVHPContribution = EEUVHPContributionandInterestforPreWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_amount);
                                    item.idecUVHPInterest = EEUVHPContributionandInterestforPreWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_int_amount);
                                }

                                item.vested_years_count = 0;
                                item.idecTotalHealthHours = Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero) < 0 ? 0 : Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero);
                                item.idecTotalIAPHours = Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero) < 0 ? 0 : Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero);
                                item.vested_hours = Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero) < 0 ? 0 : Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero);
                                item.qualified_hours = Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero) < 0 ? 0 : Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero);
                                //item.comments = "Withdrawn on " + ldtPersonWithdrawalDate.ToShortDateString();

                                #endregion

                                decimal idecEEContribution = 0.0M, idecEEInterest = 0.0M, idecUVHPContribution = 0.0M, idecUVHPInterest = 0.0M;
                                string astrComments = string.Empty;

                                #region Actual Withdrawal
                                var EEUVHPContributionandInterestforActualWDRL = (from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                                  where items.icdoPersonAccountRetirementContribution.computational_year == item.year
                                                                                  && items.icdoPersonAccountRetirementContribution.transaction_type_value == "PMNT"
                                                                                  //&& items.icdoPersonAccountRetirementContribution.effective_date == ldtPersonWithdrawalDate
                                                                                  select items);

                                if (EEUVHPContributionandInterestforActualWDRL != null)
                                {
                                    idecEEContribution = EEUVHPContributionandInterestforActualWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                                    idecEEInterest = EEUVHPContributionandInterestforActualWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_int_amount);
                                    idecUVHPContribution = EEUVHPContributionandInterestforActualWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_amount);
                                    idecUVHPInterest = EEUVHPContributionandInterestforActualWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_int_amount);
                                    astrComments = "Withdrawn on " + ldtPersonWithdrawalDate.ToShortDateString();
                                }
                                lbusAnnualBenefitSummaryOverview.AddItemsINWithdrawalCollection(Decimal.Zero, Decimal.Zero, item.year, Decimal.Zero, 0, Decimal.Zero, item.iintHealthCount, Decimal.Zero, Decimal.Zero, idecEEContribution, idecEEInterest, idecUVHPContribution, idecUVHPInterest, astrComments, item.bis_years_count, intSequenceNumber + 0.25M);
                                #endregion

                                #region Hours After Withdrawal
                                if (lbusCalculation.ldecHoursAfterWithdrawal > 0)
                                {
                                    var EEUVHPContributionandInterestforAfterWDRL = (from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                                     where items.icdoPersonAccountRetirementContribution.computational_year == item.year
                                                                                     && items.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT"
                                                                                     && items.icdoPersonAccountRetirementContribution.effective_date >= ldtPersonWithdrawalDate
                                                                                     select items);
                                    if (EEUVHPContributionandInterestforAfterWDRL != null)
                                    {
                                        idecEEContribution = EEUVHPContributionandInterestforAfterWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                                        idecEEInterest = EEUVHPContributionandInterestforAfterWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_int_amount);
                                        idecUVHPContribution = EEUVHPContributionandInterestforAfterWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_amount);
                                        idecUVHPInterest = EEUVHPContributionandInterestforAfterWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_int_amount);
                                        astrComments = string.Empty;
                                    }
                                    decimal templdecHoursAfterWDRL = Math.Round(lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero);
                                    if (ldtEffectiveDate.Year > item.year)
                                        lbusAnnualBenefitSummaryOverview.AddItemsINWithdrawalCollection(templdecHoursAfterWDRL, Math.Round(templdecHoursAfterWDRL * item.idecBenefitRate, 2, MidpointRounding.AwayFromZero)
                                                                    , item.year, adectotalaccuredbenefitamount, 0, Decimal.Zero, item.iintHealthCount, templdecHoursAfterWDRL, templdecHoursAfterWDRL, idecEEContribution, idecEEInterest, idecUVHPContribution, idecUVHPInterest, astrComments, item.bis_years_count, intSequenceNumber + 0.50M);
                                    else
                                    {
                                        lbusAnnualBenefitSummaryOverview.AddItemsINWithdrawalCollection(templdecHoursAfterWDRL, Math.Round(templdecHoursAfterWDRL * item.idecBenefitRate, 2, MidpointRounding.AwayFromZero)
                                                                    , item.year, adectotalaccuredbenefitamount, 0, templdecHoursAfterWDRL, item.iintHealthCount, templdecHoursAfterWDRL, templdecHoursAfterWDRL, idecEEContribution, idecEEInterest, idecUVHPContribution, idecUVHPInterest, astrComments, item.bis_years_count, intSequenceNumber + 0.50M, 1);
                                    }
                                }
                                else
                                {
                                    var EEUVHPContributionandInterestforAfterWDRL = (from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                                     where items.icdoPersonAccountRetirementContribution.computational_year == item.year
                                                                                     && items.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT"
                                                                                     && items.icdoPersonAccountRetirementContribution.effective_date >= ldtPersonWithdrawalDate
                                                                                     select items);
                                    if (EEUVHPContributionandInterestforAfterWDRL != null)
                                    {
                                        idecEEContribution = EEUVHPContributionandInterestforAfterWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                                        idecEEInterest = EEUVHPContributionandInterestforAfterWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_int_amount);
                                        idecUVHPContribution = EEUVHPContributionandInterestforAfterWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_amount);
                                        idecUVHPInterest = EEUVHPContributionandInterestforAfterWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_int_amount);
                                        astrComments = string.Empty;
                                    }
                                    lbusAnnualBenefitSummaryOverview.AddItemsINWithdrawalCollection(Decimal.Zero, Decimal.Zero, item.year, Decimal.Zero, 0, Decimal.Zero, item.iintHealthCount, Decimal.Zero, Decimal.Zero, idecEEContribution, idecEEInterest, idecUVHPContribution, idecUVHPInterest, astrComments, item.bis_years_count, intSequenceNumber + 0.50M);
                                }
                                #endregion
                            }


                        }
                        #endregion
                    }
                    #endregion

                    #region if hours reported after withdrawal
                    if (lbusAnnualBenefitSummaryOverview.lclbtempCdoDummyWorkdata.Count() > 0)
                    {
                        foreach (cdoDummyWorkData item in lbusAnnualBenefitSummaryOverview.lclbtempCdoDummyWorkdata)
                        {
                            lbusBenefitApplication.aclbPersonWorkHistory_MPI.Add(item);
                        }
                        lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(i => i.year).ToList().ToCollection();
                    }

                    //Removing withdwan hours, vested hours, qualified year count , vested year count, accured benefit from collection
                    if (ldtEffectiveDate != DateTime.MinValue)
                    {
                        DataTable ldtbLastNonVestedEEContrDate = busBase.Select("cdoBenefitApplication.GetLastNonVestedContributionDate", new object[1] { this.icdoPerson.person_id });
                        if (ldtbLastNonVestedEEContrDate.Rows.Count > 0 && ldtbLastNonVestedEEContrDate.Rows[0][0] != DBNull.Value)
                        {
                            foreach (cdoDummyWorkData item in lbusBenefitApplication.aclbPersonWorkHistory_MPI)
                            {
                                if (item.year <= Convert.ToInt32(ldtbLastNonVestedEEContrDate.Rows[0][0]) && item.aintAfterWDRLCount < 1)
                                {
                                    item.idecWithdrawalHours = item.qualified_hours;
                                    item.qualified_hours = decimal.Zero;
                                    item.idecBenefitAmount = 0;
                                }
                            }
                        }
                    }
                    #endregion

                    #region Local Merge
                    if (lbusBenefitApplication.ibusPerson.iclbPersonAccount.IsNotNull() && lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode != busConstant.MPIPP && item.icdoPersonAccount.istrPlanCode != busConstant.IAP).Count() > 0)
                    {
                        lbusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode != busConstant.MPIPP && item.icdoPersonAccount.istrPlanCode != busConstant.IAP).ToList().ToCollection();
                        foreach (busPersonAccount lobjPersonAccount in lbusBenefitApplication.ibusPerson.iclbPersonAccount.AsEnumerable())
                        {
                            decimal idecLocalHours = 0.0M;
                            lclbLocalMergerCdoDummyWorkdata = new cdoDummyWorkData();

                            switch (lobjPersonAccount.icdoPersonAccount.istrPlanCode)
                            {
                                case busConstant.LOCAL_700:
                                    idecLocalHours = (from item in lbusBenefitApplication.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum();
                                    break;
                                case busConstant.Local_600:
                                    idecLocalHours = (from item in lbusBenefitApplication.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum();
                                    break;
                                case busConstant.Local_666:
                                    idecLocalHours = (from item in lbusBenefitApplication.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum();
                                    break;
                                case busConstant.Local_52:
                                    idecLocalHours = (from item in lbusBenefitApplication.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();
                                    break;
                                case busConstant.Local_161:
                                    idecLocalHours = (from item in lbusBenefitApplication.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();
                                    break;
                            }

                            lclbLocalMergerCdoDummyWorkdata.qualified_hours = idecLocalHours;
                            lclbLocalMergerCdoDummyWorkdata.vested_hours = idecLocalHours;
                            lclbLocalMergerCdoDummyWorkdata.year = (from item in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                    where item.qualified_hours != 0
                                                                    select item.year - 1).FirstOrDefault();

                            busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();
                            lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lobjPersonAccount.icdoPersonAccount.person_account_id).FirstOrDefault().icdoPersonAccount.person_account_id);
                            if (lbusLocalPersonAccountEligibility != null)
                            {
                                lclbLocalMergerCdoDummyWorkdata.qualified_years_count = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                lclbLocalMergerCdoDummyWorkdata.vested_years_count = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                iintLocalQualifiedYearCount = iintLocalQualifiedYearCount + lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years; ;
                            }

                            if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution != null && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0)
                            {
                                lclbLocalMergerCdoDummyWorkdata.idecBenefitAmount = (from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                                     where items.icdoPersonAccountRetirementContribution.person_account_id == lobjPersonAccount.icdoPersonAccount.person_account_id
                                                                                     select items.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount).Sum();
                            }
                            lclbLocalMergerCdoDummyWorkdata.istrPlanCode = lobjPersonAccount.icdoPersonAccount.istrPlanCode;
                            aclbPersonWorkHistory_Local.Add(lclbLocalMergerCdoDummyWorkdata);
                        }
                        lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year > lclbLocalMergerCdoDummyWorkdata.year).ToList().ToCollection();
                        lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(i => i.year).ToList().ToCollection();
                    }
                    #endregion
                }

                #region Required count setting because of Local Merge and Withdrawal Splits --  need to be review once
                if ((ldtEffectiveDate != DateTime.MinValue && ldtEffectiveDate != null) || iintLocalQualifiedYearsCount != 0)
                {
                    int tempYear = 0;

                    foreach (cdoDummyWorkData item in lbusBenefitApplication.aclbPersonWorkHistory_MPI)
                    {
                        if (item.year > lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year)
                        {
                            if (iintLocalQualifiedYearsCount != 0 && lclbLocalMergerCdoDummyWorkdata.qualified_hours == item.qualified_hours)
                            {
                                item.qualified_years_count = lintTotalqualifiedYearsCount + iintLocalQualifiedYearsCount;
                                item.vested_years_count = lintTotalvestedYearsCount + iintLocalQualifiedYearsCount;
                                lintTotalqualifiedYearsCount += iintLocalQualifiedYearsCount;
                                lintTotalvestedYearsCount += iintLocalQualifiedYearsCount;
                            }
                            else if (iintLocalQualifiedYearsCount == 0 && (item.comments.IsNotNullOrEmpty() && item.comments.Contains("Local Frozen Service")))
                            {
                                tempYear = 0;
                            }
                            else
                            {
                                if ((item.qualified_hours >= 400 || item.idecTempqualified_hours >= 400) && tempYear != item.year)
                                {
                                    lintTotalqualifiedYearsCount++;
                                    item.qualified_years_count = lintTotalqualifiedYearsCount;
                                }
                                else
                                {
                                    item.qualified_years_count = lintTotalqualifiedYearsCount;
                                }

                                if ((item.vested_hours >= 400 || item.idecTempqualified_hours >= 400) && tempYear != item.year)
                                {
                                    lintTotalvestedYearsCount++;
                                    item.vested_years_count = lintTotalvestedYearsCount;
                                }
                                else
                                {
                                    item.vested_years_count = lintTotalvestedYearsCount;
                                }

                                if (ldtWdrlDateBefore1976.Year >= item.year)
                                {
                                    lintTotalqualifiedYearsCount = 0;
                                }

                                if (item.idecTotalHealthHours >= 400 && tempYear != item.year)
                                {
                                    lintTotalHealthYearsCount++;
                                    item.iintHealthCount = lintTotalHealthYearsCount;
                                }
                                else
                                {
                                    item.iintHealthCount = lintTotalHealthYearsCount;
                                }
                                tempYear = item.year;
                            }
                        }
                        else
                        {
                            if (item.idecTotalHealthHours >= 400)
                            {
                                lintTotalHealthYearsCount++;
                                item.iintHealthCount = lintTotalHealthYearsCount;
                            }
                            else
                            {
                                item.iintHealthCount = lintTotalHealthYearsCount;
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion
        }
        #endregion

        //for PIR-857
        public void AuditLogHistoryForVestingDtPersnOvervw(DateTime adtVestedDt, DateTime adtNewVestedeDt, string astrPlan)
        {
            cdoFullAuditLog lcdoFullAuditLog = new cdoFullAuditLog();
            lcdoFullAuditLog.person_id = this.icdoPerson.person_id;
            lcdoFullAuditLog.primary_key = this.icdoPerson.person_id;
            lcdoFullAuditLog.form_name = "wfmPersonOverviewMaintenance";
            lcdoFullAuditLog.table_name = "sgt_person_account_eligibility";
            //lcdoFullAuditLog.Insert();

            //cdoFullAuditLogDetail lcdoFullAuditLogDetail = new cdoFullAuditLogDetail();
            //lcdoFullAuditLogDetail.audit_log_id = lcdoFullAuditLog.audit_log_id;
            //if (adtVestedDt != DateTime.MinValue)
            //{
            //    lcdoFullAuditLogDetail.old_value = Convert.ToString(adtVestedDt);
            //}
            //else
            //{
            //    lcdoFullAuditLogDetail.old_value = string.Empty;
            //}

            //if (adtNewVestedeDt != DateTime.MinValue)
            //{
            //    lcdoFullAuditLogDetail.new_value = Convert.ToString(adtNewVestedeDt);
            //}
            //else
            //{
            //    lcdoFullAuditLogDetail.new_value = string.Empty;
            //}

            //lcdoFullAuditLogDetail.column_name = "VESTED_DATE(" + astrPlan + ")";
            //lcdoFullAuditLogDetail.Insert();

            //Fw upgrade: PIR ID : 28660: New implementation of Audit History using audit_details
            string old_value = null;
            if (adtVestedDt != DateTime.MinValue)
            {
                old_value = Convert.ToString(adtVestedDt);
            }
            else
            {
                old_value = string.Empty;
            }

            string new_value = null;
            if (adtNewVestedeDt != DateTime.MinValue)
            {
                new_value = Convert.ToString(adtNewVestedeDt);
            }
            else
            {
                new_value = string.Empty;
            }

            var lcdoFullAuditLogDetail = new
            {
                column_name = "VESTED_DATE(" + astrPlan + ")",
                old_value = old_value,
                new_value = new_value,
            };
            string lsrtJSONAuditDetails = Newtonsoft.Json.JsonConvert.SerializeObject(lcdoFullAuditLogDetail);
            lcdoFullAuditLog.audit_details = lsrtJSONAuditDetails;
            lcdoFullAuditLog.Insert();
        }
        public void AuditLogHistoryForfeitureDatePersnOvervw(DateTime adtForfeitureDt, DateTime adtNewForfeitureDt, string astrPlan)
        {
            cdoFullAuditLog lcdoFullAuditLog = new cdoFullAuditLog();
            lcdoFullAuditLog.person_id = this.icdoPerson.person_id;
            lcdoFullAuditLog.primary_key = this.icdoPerson.person_id;
            lcdoFullAuditLog.form_name = "wfmPersonOverviewMaintenance";
            lcdoFullAuditLog.table_name = "sgt_person_account_eligibility";
            //lcdoFullAuditLog.Insert();

            //cdoFullAuditLogDetail lcdoFullAuditLogDetail = new cdoFullAuditLogDetail();
            //lcdoFullAuditLogDetail.audit_log_id = lcdoFullAuditLog.audit_log_id;

            //if (adtForfeitureDt != DateTime.MinValue)
            //{
            //    lcdoFullAuditLogDetail.old_value = Convert.ToString(adtForfeitureDt);
            //}
            //else
            //{
            //    lcdoFullAuditLogDetail.old_value = string.Empty;
            //}

            //if (adtNewForfeitureDt != DateTime.MinValue)
            //{
            //    lcdoFullAuditLogDetail.new_value = Convert.ToString(adtNewForfeitureDt);
            //}
            //else
            //{
            //    lcdoFullAuditLogDetail.new_value = string.Empty;
            //}

            //lcdoFullAuditLogDetail.column_name = "FORFEITURE_DATE(" + astrPlan + ")";
            //lcdoFullAuditLogDetail.Insert();

            //Fw upgrade: PIR ID : 28660: New implementation of Audit History using audit_details
            string old_value = null;
            if (adtForfeitureDt != DateTime.MinValue)
            {
                old_value = Convert.ToString(adtForfeitureDt);
            }
            else
            {
                old_value = string.Empty;
            }

            string new_value = null;
            if (adtNewForfeitureDt != DateTime.MinValue)
            {
                new_value = Convert.ToString(adtNewForfeitureDt);
            }
            else
            {
                new_value = string.Empty;
            }

            var lcdoFullAuditLogDetail = new
            {
                column_name = "FORFEITURE_DATE(" + astrPlan + ")",
                old_value = old_value,
                new_value = new_value,
            };
            string lsrtJSONAuditDetails = Newtonsoft.Json.JsonConvert.SerializeObject(lcdoFullAuditLogDetail);
            lcdoFullAuditLog.audit_details = lsrtJSONAuditDetails;
            lcdoFullAuditLog.Insert();
        }


        //Comment - Temporary message for 10 Percent Increase
        public bool IsEligibleForActiveIncrease()
        {
            iblnEligibleForActiveIncrease = false;
            int lintRetireePayeeAccountCount = (int)DBFunction.DBExecuteScalar("cdoPerson.CheckIfRetiredFromMSS", new object[1] { icdoPerson.person_id },
                                                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (lintRetireePayeeAccountCount <= 0)
            {
                decimal ldecIncreaseYear = 2015;
                DataTable ldtbPlanBenefitRate = busBase.Select("cdoPlanBenefitRate.Lookup", new object[] { });
                Collection<cdoPlanBenefitRate> lclbcdoPlanBenefitRate = cdoDummyWorkData.GetCollection<cdoPlanBenefitRate>(ldtbPlanBenefitRate);

                if (lclbcdoPlanBenefitRate != null && lclbcdoPlanBenefitRate.Count > 0 && lclbcdoPlanBenefitRate.Where(t => t.rate_type_value == busConstant.BenefitCalculation.PLAN_B && t.plan_year == ldecIncreaseYear).Count() > 0)
                {
                    decimal ldecBenefitRateupto10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == ldecIncreaseYear && t.qualified_year_limit_value == "10").FirstOrDefault().rate;
                    decimal ldecBenefitRateafter10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == ldecIncreaseYear && t.qualified_year_limit_value == "20").FirstOrDefault().rate;

                    if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(t => (t.idecBenefitRate == ldecBenefitRateupto10QY || t.idecBenefitRate == ldecBenefitRateafter10QY) && t.idecBenefitAmount > 0).Count() > 0)
                    {
                        iblnEligibleForActiveIncrease = true;
                    }
                }
            }
            return iblnEligibleForActiveIncrease;
        }

        public byte[] MonthOfSuspendibleServiceReport(int aintPersonId)
        {
            byte[] lbyteFile = null;

            DateTime ldtEffectiveDate = new DateTime();
            DataSet ldsResult = new DataSet();
            DataTable ldResult = new DataTable();

            DataTable ldtblRetirementDate = busBase.Select("cdoPerson.GetRetirementDate", new object[1] { icdoPerson.person_id });
            if (ldtblRetirementDate != null && ldtblRetirementDate.Rows.Count > 0 &&
                Convert.ToString(ldtblRetirementDate.Rows[0][enmBenefitApplication.retirement_date.ToString().ToUpper()]).IsNotNullOrEmpty())
                ldtEffectiveDate = Convert.ToDateTime(ldtblRetirementDate.Rows[0][enmBenefitApplication.retirement_date.ToString().ToUpper()]);

            if (ldtEffectiveDate != DateTime.MinValue && ldtEffectiveDate > icdoPerson.date_of_birth.AddYears(65))
                ldtEffectiveDate = icdoPerson.date_of_birth.AddYears(65);

            if (ldtEffectiveDate != DateTime.MinValue)
                ldResult = busBase.Select("cdoPerson.MonthOfSuspendibleServiceReport", new object[2] { ldtEffectiveDate.Year, icdoPerson.ssn });

            ldResult.TableName = "ReportTable01";
            ldsResult.Tables.Add(ldResult.Copy());

            if (ldsResult != null)
            {
                busCreateReports lbusCreateReports = new busCreateReports();
                lbyteFile = lbusCreateReports.CreateDynamicReport(ldsResult, "rpt_MonthOfSuspendibleServiceReport");
            }

            return lbyteFile;
        }
    }
}
