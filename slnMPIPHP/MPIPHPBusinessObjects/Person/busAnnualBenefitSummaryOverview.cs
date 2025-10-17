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
using System.Linq;
using System.Data.SqlClient;
using Sagitec.DataObjects;
using System.Collections.Generic;
#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busAnnualBenefitSummaryOverview : busPerson
    {
        public bool istrPersonVested { get; set; }
        public busBenefitApplication lbusBenefitApplication { get; set; }
        public busPersonOverview ibusPersonOverview { get; set; }
        public Collection<cdoPersonAccount> iclcdoPersonAccountOverview { get; set; }
        public busPersonAccountEligibility lbusPersonAccountEligibility { get; set; }
        public DateTime ldtEffectiveDate { get; set; }
        public int ldtlatestForfietureDate { get; set; }
        public decimal ldecwihtodrawalhours { get; set; }
        public bool iblFlagYes = false;
        public string iintIAPAllocationYear { get; set; }

        public bool iEEUVHPReclaimFlag { get; set; }
        // for annual benifit summary overview
        public Collection<cdoDummyWorkData> aclbAnnualBenfitSummayOverviewTotal { get; set; }
        public Collection<cdoDummyWorkData> aclbPersonWorkHistory_Local { get; set; }
        private DataTable ldtAnnualBenfitSummayOverview { get; set; }

        public decimal ldecTotalPensionHours { get; set; }
        public decimal ldecTotalPensionQualifiedYears { get; set; }
        public decimal ldecTotalPensionVestedYears { get; set; }

        public decimal ldecTotalIAPHours { get; set; }
        public decimal ldecTotalIAPQualifiedYears { get; set; }
        public decimal ldecTotalIAPVestedYears { get; set; }

        public decimal ldecTotalHealthHours { get; set; }
        public decimal ldecTotalHealthYears { get; set; }

        //PER-0015
        public decimal idecAccruedBenefit { get; set; }
        public decimal idecTotalQlfdYrs { get; set; }
        public decimal idecTotalQlfdHours { get; set; }

        public int iintPersonAccountId { get; set; }

        public busBenefitCalculationHeader lbusBenefitCalculationHeader { get; set; }
        public cdoDummyWorkData lclbLocalMergerCdoDummyWorkdata { get; set; }
        public decimal lintTotalqualifiedYearsCount { get; set; }
        public decimal lintTotalvestedYearsCount { get; set; }
        public decimal lintTotalHealthYearsCount { get; set; }
        public int lintBISYearCount { get; set; }
        public decimal ldecTotalCreditedHours { get; set; }
        public decimal ldecTotalVestedHours { get; set; }
        public decimal ldecTotalQualifiedIAPHours { get; set; }
        public decimal ldecTotalQualifiedAccuruedBenefit { get; set; }
        public decimal ldecTotalQualifiedAccruedBenefitLocal { get; set; }
        public decimal ldecTotalEEContribution { get; set; }
        public decimal ldecTotalEEInterestContribution { get; set; }
        public decimal ldecTotalUVHPContribution { get; set; }
        public decimal ldecTotalUVHPInterestContribution { get; set; }
        public decimal ldecWithdrawalHoursSum { get; set; }
        public decimal ldecTotalHealthHoursSum { get; set; }

        public decimal lintNonQualifiedYears { get; set; } //PIR 970

        public bool iblnEligibleForActiveIncrease { get; set; } //Temporary

        public void LoadWorkHistory(bool flagYes = false, int iintMSS = 0, bool ablnFromService = false)  //For CRM Bug 9922
        {
         
            lbusBenefitApplication = new busBenefitApplication();
            lbusBenefitApplication.ibusPerson = new busPerson();
            lbusBenefitApplication.ibusPerson.FindPerson(this.icdoPerson.person_id);
            lbusBenefitApplication.ibusPerson.LoadPersonAccounts();
            lbusBenefitApplication.ibusPerson.LoadRetirementContributionsForEE(lbusBenefitApplication.ibusPerson.icdoPerson.person_id);
            //IMP- STMT required to make the VISBILE RULES WORK INHERITED FROM BUSPERSON.XML
            this.iclbPersonAccount = lbusBenefitApplication.ibusPerson.iclbPersonAccount;

            lbusBenefitApplication.aclbPersonWorkHistory_MPI = new Collection<cdoDummyWorkData>();
            lbusBenefitApplication.aclbPersonWorkHistory_IAP = new Collection<cdoDummyWorkData>();
            lbusBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
            lbusBenefitApplication.lbl4PersonOverviewSummary = true;
            bool flagRetired = busConstant.BOOL_FALSE;
            istrPersonVested = busConstant.BOOL_FALSE;
            lbusBenefitApplication.ibusPerson.LoadBenefitApplication();

            if ((!lbusBenefitApplication.ibusPerson.iclbBenefitApplication.IsNullOrEmpty() && lbusBenefitApplication.ibusPerson.iclbBenefitApplication.Count > 0) || (this.icdoPerson.date_of_death.IsNotNull() && this.icdoPerson.date_of_death != DateTime.MinValue))
            {
                if (lbusBenefitApplication.ibusPerson.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.benefit_type_value == "RTMT" || i.icdoBenefitApplication.benefit_type_value == "DSBL").Count() > 0)
                {
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitApplication.ibusPerson.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.benefit_type_value == "RTMT" || i.icdoBenefitApplication.benefit_type_value == "DSBL").OrderBy(i => i.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitApplication.retirement_date;
                    flagRetired = busConstant.BOOL_TRUE;
                    if (this.icdoPerson.date_of_death.IsNotNull() && this.icdoPerson.date_of_death != DateTime.MinValue && this.icdoPerson.date_of_death < lbusBenefitApplication.icdoBenefitApplication.retirement_date)
                        lbusBenefitApplication.icdoBenefitApplication.retirement_date = this.icdoPerson.date_of_death;
                }
                else if (this.icdoPerson.date_of_death.IsNotNull() && this.icdoPerson.date_of_death != DateTime.MinValue)
                {
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date = DateTime.Now;
                }
                else
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            }
            else
                lbusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);

            //PIR-857            
            foreach (busPersonAccount lbusPersonAccount in this.iclbPersonAccount)
            {
                //busPersonAccountEligibility lbusPersonAccntEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                DataTable ldtbPersnAcntEligiblityData = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lbusPersonAccount.icdoPersonAccount.person_account_id });
                if (ldtbPersnAcntEligiblityData.Rows.Count > 0)
                {
                    if (lbusPersonAccount.icdoPersonAccount.plan_id == 1)
                    {
                        lbusBenefitApplication.idtVestingDtIAP = Convert.ToDateTime(Convert.ToBoolean(ldtbPersnAcntEligiblityData.Rows[0]["vested_date"].IsDBNull()) ?
                                                                                  DateTime.MinValue : ldtbPersnAcntEligiblityData.Rows[0]["vested_date"]);
                        lbusBenefitApplication.idtForfeitureDtIAP = Convert.ToDateTime(Convert.ToBoolean(ldtbPersnAcntEligiblityData.Rows[0]["forfeiture_date"].IsDBNull()) ?
                                                                                  DateTime.MinValue : ldtbPersnAcntEligiblityData.Rows[0]["forfeiture_date"]);
                    }
                    else if (lbusPersonAccount.icdoPersonAccount.plan_id == 2)
                    {
                        lbusBenefitApplication.idtVestingDtMPI = Convert.ToDateTime(Convert.ToBoolean(ldtbPersnAcntEligiblityData.Rows[0]["vested_date"].IsDBNull()) ?
                                                                                     DateTime.MinValue : ldtbPersnAcntEligiblityData.Rows[0]["vested_date"]);
                        lbusBenefitApplication.idtForfeitureDtMPI = Convert.ToDateTime(Convert.ToBoolean(ldtbPersnAcntEligiblityData.Rows[0]["forfeiture_date"].IsDBNull()) ?
                                                                                     DateTime.MinValue : ldtbPersnAcntEligiblityData.Rows[0]["forfeiture_date"]);
                    }
                }
            }

            lbusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_PersonOverView(ablnFromService);  //For CRM Bug 9922

            //PIR-857
            foreach (busPersonAccount lbusPersonAccount in this.iclbPersonAccount)
            {
                DataTable ldtbPersnAcntEligiblityData = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lbusPersonAccount.icdoPersonAccount.person_account_id });
                if (ldtbPersnAcntEligiblityData.Rows.Count > 0)
                {
                    if (lbusPersonAccount.icdoPersonAccount.plan_id == 1)
                    {
                        lbusBenefitApplication.idtVestingDtIAPaftrFlg = Convert.ToDateTime(Convert.ToBoolean(ldtbPersnAcntEligiblityData.Rows[0]["vested_date"].IsDBNull()) ?
                                                                                         DateTime.MinValue : ldtbPersnAcntEligiblityData.Rows[0]["vested_date"]);
                        lbusBenefitApplication.idtForfeitureDtIAPaftrFlg = Convert.ToDateTime(Convert.ToBoolean(ldtbPersnAcntEligiblityData.Rows[0]["forfeiture_date"].IsDBNull()) ?
                                                                                         DateTime.MinValue : ldtbPersnAcntEligiblityData.Rows[0]["forfeiture_date"]);
                    }
                    else if (lbusPersonAccount.icdoPersonAccount.plan_id == 2)
                    {
                        lbusBenefitApplication.idtVestingDtMPIaftrFlg = Convert.ToDateTime(Convert.ToBoolean(ldtbPersnAcntEligiblityData.Rows[0]["vested_date"].IsDBNull()) ?
                                                                                         DateTime.MinValue : ldtbPersnAcntEligiblityData.Rows[0]["vested_date"]);
                        lbusBenefitApplication.idtForfeitureDtMPIaftrFlg = Convert.ToDateTime(Convert.ToBoolean(ldtbPersnAcntEligiblityData.Rows[0]["forfeiture_date"].IsDBNull()) ?
                                                                                            DateTime.MinValue : ldtbPersnAcntEligiblityData.Rows[0]["forfeiture_date"]);
                    }
                }
            }

            #region Enhancement Logic
            if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
            {
                busCalculation lbusCalculation = new busCalculation();
                cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                this.aclbAnnualBenfitSummayOverviewTotal = new Collection<cdoDummyWorkData>();
                aclbPersonWorkHistory_Local = new Collection<cdoDummyWorkData>();
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                Collection<cdoPersonAccountRetirementContribution> lclbRetCont = new Collection<cdoPersonAccountRetirementContribution>();
                lbusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                DateTime idtVestedDate = DateTime.MinValue;

                if (lbusBenefitApplication.ibusPerson.iclbPersonAccount.Count() > 0)
                {
                    lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
                    if (lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                    {
                        idtVestedDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                        this.istrPersonVested = busConstant.BOOL_TRUE;
                    }
                    lbusBenefitCalculationHeader.ibusPerson = lbusBenefitApplication.ibusPerson;
                    lbusBenefitCalculationHeader.LoadAllRetirementContributions(null);
                    if (this.icdoPerson.date_of_death != DateTime.MinValue)
                    {
                        //Removing padding on basis of date of death or Latest EE contribution and interest reported
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

                    if (flagYes)
                    {
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
                    }
                    decimal ldecUnreducedBenefitAmount = LoadAccruedBenefitPerYear(lbusBenefitApplication, lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date);
                }

                #region PROD PIR 205
                DateTime ldtEffectiveDateforUVHP = DateTime.MinValue;
                DateTime ldtTransactionDateforEE = DateTime.MinValue; //PIR 999
                DateTime ldtWdrlDateBefore1976 = DateTime.MinValue;
                int tempLocalMergerYear = 0;
              

                DataTable ldtbCheckPersonHasWithdrawal = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawal", new object[1] { lbusBenefitApplication.ibusPerson.icdoPerson.person_id });
                if (ldtbCheckPersonHasWithdrawal != null && ldtbCheckPersonHasWithdrawal.Rows.Count > 0)
                {
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

                    //PIR 999
                    ldtTransactionDateforEE = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                               where item.Field<string>("CONTRIBUTION_TYPE_VALUE") == "EE"
                                               orderby item.Field<DateTime>("TRANSACTION_DATE") descending
                                               select item.Field<DateTime>("TRANSACTION_DATE")).FirstOrDefault();

                    if (ldtEffectiveDate != DateTime.MinValue)
                    {
                        ldecwihtodrawalhours = lbusCalculation.GetWorkDataAfterDate(lbusBenefitApplication.ibusPerson.icdoPerson.ssn, ldtEffectiveDate.Year, busConstant.MPIPP_PLAN_ID, ldtEffectiveDate);
                    }
                }
                #endregion

                int iintLocalQualifiedYearsCount = 0;
                Dictionary<string, int> idtLocalQualifiedYearsCount = new Dictionary<string, int>();
                Dictionary<string, int> idtLocalQualifiedHours = new Dictionary<string, int>();
                decimal idecqualified_hours = 0;
                if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count() > 0)
                {
                    #region EE_UVHP_INTEREST_CONTRIBUTION and Withdrawals

                    #region Padding
                    if ((ldtEffectiveDate != DateTime.MinValue && ldtEffectiveDate != null && ldtEffectiveDate.Year > lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year)
                        || (ldtTransactionDateforEE != DateTime.MinValue && ldtTransactionDateforEE != null && ldtTransactionDateforEE.Year > lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year) //PIR 999
                        || (ldtEffectiveDateforUVHP != DateTime.MinValue && ldtEffectiveDateforUVHP != null && ldtEffectiveDateforUVHP.Year > lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year)
                        || (ldtEffectiveDate == DateTime.MinValue && ldtEffectiveDateforUVHP == DateTime.MinValue && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year < DateTime.Now.Year && !flagRetired))
                    {
                        Collection<cdoDummyWorkData> lclbtempcdoDummyWorkDataforPadding = new Collection<cdoDummyWorkData>();
                        DateTime lstTempEffectiveDateTime = DateTime.MinValue;
                        if (ldtEffectiveDate == DateTime.MinValue && ldtEffectiveDateforUVHP == DateTime.MinValue && ldtTransactionDateforEE == DateTime.MinValue) //PIR 999
                            lstTempEffectiveDateTime = DateTime.Now;
                        else
                            lstTempEffectiveDateTime = new[] { ldtEffectiveDate, ldtEffectiveDateforUVHP, ldtTransactionDateforEE }.Max(); //PIR 999//ldtEffectiveDate.Date > ldtEffectiveDateforUVHP.Date ? ldtEffectiveDate : ldtEffectiveDateforUVHP;//PIR 999

                        if (lstTempEffectiveDateTime != DateTime.MinValue)
                        {
                            
                            for (int i = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year; i < lstTempEffectiveDateTime.Year; i++)
                            {
                                cdoDummyWorkData tempcdoDummyWorkDataforPadding = new cdoDummyWorkData();
                                tempcdoDummyWorkDataforPadding.year = i + 1;
                                
                                tempcdoDummyWorkDataforPadding.iintHealthCount = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().iintHealthCount;
                                tempcdoDummyWorkDataforPadding.qualified_years_count = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                tempcdoDummyWorkDataforPadding.vested_years_count = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count;
                                if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == i).Count() > 0)
                                {
                                    tempcdoDummyWorkDataforPadding.iintNonQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == i).FirstOrDefault().iintNonQualifiedYears + 1;
                                    if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == i).FirstOrDefault().comments.IsNotNullOrEmpty()
                                        && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == i).FirstOrDefault().comments.Contains("(MPI_PLAN)")
                                        && !lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == i).FirstOrDefault().comments.Contains("FORFEITURE"))
                                        tempcdoDummyWorkDataforPadding.comments = busConstant.BIS_PARTICIPANT + "(MPI_PLAN)";
                                }
                                tempcdoDummyWorkDataforPadding.qualified_hours = 0.0M;
                                tempcdoDummyWorkDataforPadding.idecTotalHealthHours = 0.0M;

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
                    DateTime ldtPersonTransactionDate = DateTime.MinValue;
                    lclbtempCdoDummyWorkdata = new Collection<cdoDummyWorkData>();
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
                            ldtPersonTransactionDate = (from itemWithdrawal in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                                        where itemWithdrawal.Field<string>("CONTRIBUTION_TYPE_VALUE") == "EE"
                                                        && itemWithdrawal.Field<DateTime>("WITHDRAWAL_DATE").Year == item.year
                                                        orderby itemWithdrawal.Field<DateTime>("WITHDRAWAL_DATE") descending
                                                        select itemWithdrawal.Field<DateTime>("TRANSACTION_DATE")).FirstOrDefault();
                        }

                        adectotalaccuredbenefitamount = adectotalaccuredbenefitamount + item.idecBenefitAmount;
                        item.idectotalBenefitAmount = adectotalaccuredbenefitamount;
                        item.idecTempqualified_hours = item.qualified_hours;

                        if (ldtPersonWithdrawalDate == DateTime.MinValue)
                        {
                            if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0 &&
                                lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(i => i.icdoPersonAccountRetirementContribution.computational_year == item.year).Count() > 0)
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

                                if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0 &&
                                    lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(i => i.icdoPersonAccountRetirementContribution.transaction_type_value == "PMNT" && i.icdoPersonAccountRetirementContribution.contribution_type_value == "EE").Count() > 0)
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

                                if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0 &&
                                    lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(i => i.icdoPersonAccountRetirementContribution.computational_year == item.year).Count() > 0)
                                {
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
                                }

                                item.vested_years_count = 0;
                                item.idecBenefitAmount = Math.Round((item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal) * item.idecBenefitRate, 2, MidpointRounding.AwayFromZero);
                                item.idecTotalHealthHours = Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero) < 0 ? 0 : Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero);
                                item.idecTotalIAPHours = Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero) < 0 ? 0 : Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero);
                                if (item.comments.IsNotNullOrEmpty() && item.comments.Contains("BRIDGED SERVICE"))
                                { }
                                else
                                    item.vested_hours = Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero) < 0 ? 0 : Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero);
                                item.qualified_hours = Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero) < 0 ? 0 : Math.Round(item.qualified_hours - lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero);
                                //item.comments = "Withdrawn on " + ldtPersonWithdrawalDate.ToShortDateString();
                                string astrComments = string.Empty;
                                if (iintMSS != 0) //Changed for MSS
                                {
                                    if (item.comments.IsNotNullOrEmpty() && item.comments.Contains("FORFEITURE AT THE END OF THE YEAR"))
                                    {
                                        item.comments = "Withdrawn on " + ldtPersonTransactionDate.ToShortDateString();
                                        astrComments = "FORFEITURE AT THE END OF THE YEAR";
                                    }
                                    else
                                        item.comments = "Withdrawn on " + ldtPersonTransactionDate.ToShortDateString();
                                }
                                #endregion

                                decimal idecEEContribution = 0.0M, idecEEInterest = 0.0M, idecUVHPContribution = 0.0M, idecUVHPInterest = 0.0M;


                                if (iintMSS == 0) //Changed for MSS
                                {
                                    #region Actual Withdrawal
                                    if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0 &&
                                        lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(i => i.icdoPersonAccountRetirementContribution.computational_year == item.year).Count() > 0)
                                    {
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
                                            iEEUVHPReclaimFlag = false;
                                            if(idecEEContribution != 0 || idecEEInterest != 0 || idecUVHPContribution !=0 || idecUVHPInterest !=0)
                                            {
                                                iEEUVHPReclaimFlag = true;

                                            }
                                            if(iEEUVHPReclaimFlag)
                                            astrComments = "Withdrawn on " + ldtPersonTransactionDate.ToShortDateString();
                                        }
                                    }
                                    //tusharchandak
                                    bool IsWithdrawnAfterForFietureDate = false;
                                    if (item.comments.IsNotNullOrEmpty() && item.comments.Contains("FORFEITURE AT THE END OF THE YEAR"))
                                    {
                                        DateTime ForFietureDate = busGlobalFunctions.GetLastDateOfComputationYear(item.year);
                                        if (ldtPersonWithdrawalDate >= ForFietureDate)
                                        {
                                            IsWithdrawnAfterForFietureDate = true;
                                        }
                                    }

                                    if (!IsWithdrawnAfterForFietureDate)
                                    {
                                        AddItemsINWithdrawalCollection(Decimal.Zero, Decimal.Zero, item.year, Decimal.Zero, 0, Decimal.Zero, item.iintHealthCount, Decimal.Zero, Decimal.Zero, idecEEContribution, idecEEInterest, idecUVHPContribution, idecUVHPInterest, astrComments, item.bis_years_count, intSequenceNumber + 0.25M);
                                    }
                                    else
                                    {
                                        AddItemsINWithdrawalCollection(Decimal.Zero, Decimal.Zero, item.year, Decimal.Zero, 0, Decimal.Zero, item.iintHealthCount, Decimal.Zero, Decimal.Zero, idecEEContribution, idecEEInterest, idecUVHPContribution, idecUVHPInterest, astrComments, item.bis_years_count, intSequenceNumber + 0.33M);
                                    }

                                    #endregion
                                }

                                #region Hours After Withdrawal
                                if (lbusCalculation.ldecHoursAfterWithdrawal > 0)
                                {
                                    if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0 &&
                                        lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(i => i.icdoPersonAccountRetirementContribution.computational_year == item.year).Count() > 0)
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
                                            if (iintMSS != 0 && astrComments.IsNotNullOrEmpty())
                                            { }
                                            else
                                            {
                                                if (item.comments.IsNotNullOrEmpty() && item.comments.Contains("FORFEITURE AT THE END OF THE YEAR"))
                                                {
                                                    astrComments = "FORFEITURE AT THE END OF THE YEAR";
                                                    item.comments = string.Empty;
                                                }
                                                else
                                                    astrComments = string.Empty;
                                            }
                                        }
                                    }
                                    decimal templdecHoursAfterWDRL = Math.Round(lbusCalculation.ldecHoursAfterWithdrawal, 2, MidpointRounding.AwayFromZero);
                                    if (ldtEffectiveDate.Year > item.year)
                                        AddItemsINWithdrawalCollection(templdecHoursAfterWDRL, Math.Round(templdecHoursAfterWDRL * item.idecBenefitRate, 2, MidpointRounding.AwayFromZero)
                                                                    , item.year, adectotalaccuredbenefitamount, 0, Decimal.Zero, item.iintHealthCount, templdecHoursAfterWDRL, templdecHoursAfterWDRL, idecEEContribution, idecEEInterest, idecUVHPContribution, idecUVHPInterest, astrComments, item.bis_years_count, intSequenceNumber + 0.50M);
                                    else
                                    {
                                        if (astrComments == "FORFEITURE AT THE END OF THE YEAR")
                                            AddItemsINWithdrawalCollection(templdecHoursAfterWDRL, Math.Round(templdecHoursAfterWDRL * item.idecBenefitRate, 2, MidpointRounding.AwayFromZero)
                                                                        , item.year, adectotalaccuredbenefitamount, 0, templdecHoursAfterWDRL, item.iintHealthCount, templdecHoursAfterWDRL, templdecHoursAfterWDRL, idecEEContribution, idecEEInterest, idecUVHPContribution, idecUVHPInterest, astrComments, item.bis_years_count, intSequenceNumber + 0.50M);
                                        else
                                            AddItemsINWithdrawalCollection(templdecHoursAfterWDRL, Math.Round(templdecHoursAfterWDRL * item.idecBenefitRate, 2, MidpointRounding.AwayFromZero)
                                                                    , item.year, adectotalaccuredbenefitamount, 0, templdecHoursAfterWDRL, item.iintHealthCount, templdecHoursAfterWDRL, templdecHoursAfterWDRL, idecEEContribution, idecEEInterest, idecUVHPContribution, idecUVHPInterest, astrComments, item.bis_years_count, intSequenceNumber + 0.50M, 1);
                                    }
                                }
                                else
                                {
                                    if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0 &&
                                        lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Where(i => i.icdoPersonAccountRetirementContribution.computational_year == item.year).Count() > 0)
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

                                            if (iintMSS != 0 && astrComments.IsNotNullOrEmpty())
                                            { }
                                            else
                                            {
                                                if (item.comments.IsNotNullOrEmpty() && item.comments.Contains("FORFEITURE AT THE END OF THE YEAR"))
                                                {
                                                    astrComments = "FORFEITURE AT THE END OF THE YEAR";
                                                    item.comments = string.Empty;
                                                }
                                                else
                                                    astrComments = string.Empty;
                                            }
                                        }
                                    }
                                    AddItemsINWithdrawalCollection(Decimal.Zero, Decimal.Zero, item.year, Decimal.Zero, 0, Decimal.Zero, item.iintHealthCount, Decimal.Zero, Decimal.Zero, idecEEContribution, idecEEInterest, idecUVHPContribution, idecUVHPInterest, astrComments, item.bis_years_count, intSequenceNumber + 0.50M);
                                }
                                #endregion
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region if hours reported after withdrawal
                    if (lclbtempCdoDummyWorkdata.Count() > 0)
                    {
                        //tusharchandak
                        bool IsWithdrawnAfterForFietureDate = false;
                        cdoDummyWorkData tcdoDummyWorkData = new cdoDummyWorkData();

                        foreach (cdoDummyWorkData item in lclbtempCdoDummyWorkdata)
                        {
                            if (IsWithdrawnAfterForFietureDate == true)
                            {
                                lbusBenefitApplication.aclbPersonWorkHistory_MPI.Add(item);
                                lbusBenefitApplication.aclbPersonWorkHistory_MPI.Add(tcdoDummyWorkData);

                            }
                            if ((item.intSequenceNumber != 0 && (item.intSequenceNumber - 0.33M) / (int)item.intSequenceNumber == 1) || item.intSequenceNumber - 0.33M == 0)
                            {
                                IsWithdrawnAfterForFietureDate = true;
                                tcdoDummyWorkData = item;
                            }
                            else if (IsWithdrawnAfterForFietureDate == true)
                            {

                                IsWithdrawnAfterForFietureDate = false;
                            }
                            else
                            {
                                lbusBenefitApplication.aclbPersonWorkHistory_MPI.Add(item);
                            }

                        }
                        lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(i => i.year).ToList().ToCollection();
                    }

                    if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.comments.IsNotNullOrEmpty()
                                                                                                       && item.comments.Contains("FORFEITURE AT THE END OF THE YEAR")).OrderByDescending(m => m.year).Count() > 0)

                        ldtlatestForfietureDate = Convert.ToInt32(lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.comments.IsNotNullOrEmpty()
                                                                                                           && item.comments.Contains("FORFEITURE AT THE END OF THE YEAR")).OrderByDescending(m => m.year).FirstOrDefault().year);

                    int iintCheckMaxEffectiveDate = 0;
                   
                    if (ldtEffectiveDate != DateTime.MinValue && ldtEffectiveDate.Year > ldtlatestForfietureDate)
                    {

                        if (ldtbCheckPersonHasWithdrawal != null && ldtbCheckPersonHasWithdrawal.Rows.Count > 0)
                        {
                            DateTime ldtVestedEffectiveDate = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                                               where item.Field<string>("CONTRIBUTION_TYPE_VALUE") == "EE"
                                                               && (item.Field<string>("CONTRIBUTION_SUBTYPE_VALUE") != null && item.Field<string>("CONTRIBUTION_SUBTYPE_VALUE") == "VEST")
                                                               orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                                                               select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();

                            if (ldtVestedEffectiveDate == DateTime.MinValue)
                            {
                                ldtVestedEffectiveDate = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                                          where item.Field<string>("CONTRIBUTION_TYPE_VALUE") == "EE"
                                                          && (item.Field<string>("CONTRIBUTION_SUBTYPE_VALUE") != null && item.Field<string>("CONTRIBUTION_SUBTYPE_VALUE") == "NVES")
                                                          orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                                                          select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();

                                if (ldtVestedEffectiveDate != DateTime.MinValue)
                                {
                                    //for Non vested wdrl PROD PIR 769
                                    DataTable ldtbLastNonVestedEEContrDate = busBase.Select("cdoBenefitApplication.GetLastNonVestedContributionDate", new object[2] { this.icdoPerson.person_id, ldtEffectiveDate });
                                    if (ldtbLastNonVestedEEContrDate.Rows.Count > 0 && ldtbLastNonVestedEEContrDate.Rows[0][0] != DBNull.Value)
                                    {
                                        iintCheckMaxEffectiveDate = Convert.ToDateTime(ldtbLastNonVestedEEContrDate.Rows[0][0]).Year;
                                        ldtEffectiveDate = Convert.ToDateTime(ldtbLastNonVestedEEContrDate.Rows[0][0]);
                                    }
                                }
                                else
                                    iintCheckMaxEffectiveDate = ldtVestedEffectiveDate.Year;
                            }
                            else
                                iintCheckMaxEffectiveDate = ldtVestedEffectiveDate.Year;

                        }
                    }
                    if (ldtEffectiveDate != DateTime.MinValue && ldtEffectiveDate.Year > ldtlatestForfietureDate)
                    { }
                    else
                        iintCheckMaxEffectiveDate = ldtlatestForfietureDate;


                    decimal ldecCummBenefit = 0;
                    foreach (cdoDummyWorkData item in lbusBenefitApplication.aclbPersonWorkHistory_MPI)
                    {
                        if (item.year <= iintCheckMaxEffectiveDate && item.aintAfterWDRLCount < 1)
                        {
                            item.idecWithdrawalHours = item.qualified_hours;
                            item.qualified_hours = decimal.Zero;
                            item.idecBenefitAmount = 0;
                        }
                        if (ldtEffectiveDateforUVHP != DateTime.MinValue && item.year == ldtEffectiveDateforUVHP.Year)
                        {
                            var EEUVHPContributionandInterestforActualWDRL = (from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                              where items.icdoPersonAccountRetirementContribution.computational_year == item.year
                                                                              && items.icdoPersonAccountRetirementContribution.transaction_type_value == "PMNT"
                                                                          //    && items.icdoPersonAccountRetirementContribution.effective_date == ldtEffectiveDateforUVHP
                                                                              select items);
                            var lidecEEContribution = EEUVHPContributionandInterestforActualWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                            var lidecEEInterest = EEUVHPContributionandInterestforActualWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.ee_int_amount);
                            var lidecUVHPContribution = EEUVHPContributionandInterestforActualWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_amount);
                            var lidecUVHPInterest = EEUVHPContributionandInterestforActualWDRL.Sum(obj => obj.icdoPersonAccountRetirementContribution.uvhp_int_amount);
                            iEEUVHPReclaimFlag = false;
                            if (lidecEEContribution != 0 || lidecEEInterest != 0 || lidecUVHPContribution != 0 || lidecUVHPInterest != 0)
                            {
                                iEEUVHPReclaimFlag = true;

                            }
                            if (iEEUVHPReclaimFlag)
                            item.comments = "UVHP Withdrawn on " + ldtEffectiveDateforUVHP.ToShortDateString();
                        }

                        if (idtVestedDate == DateTime.MinValue)
                        {
                            if (item.istrForfietureFlag == busConstant.FLAG_YES)
                            {
                                ldecCummBenefit = 0;
                            }
                            else
                            {
                                ldecCummBenefit += item.idecBenefitAmount;
                            }
                        }
                        else
                        {
                            if (item.year >= idtVestedDate.Year)
                            {
                                ldecCummBenefit += item.idecBenefitAmount;
                            }
                            else if (item.istrForfietureFlag == busConstant.FLAG_YES)
                            {
                                ldecCummBenefit = 0;
                            }
                            else
                            {
                                ldecCummBenefit += item.idecBenefitAmount;
                            }
                        }

                        item.idecCummBalance = ldecCummBenefit;

                    }
                    //}


                    #endregion

                    #region Local Merge
                    if (this.iclbPersonAccount.IsNotNull() && this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode != busConstant.MPIPP && item.icdoPersonAccount.istrPlanCode != busConstant.IAP).Count() > 0)
                    {
                        this.iclbPersonAccount = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode != busConstant.MPIPP && item.icdoPersonAccount.istrPlanCode != busConstant.IAP).ToList().ToCollection();
                        string istrPlanCode = string.Empty;
                        decimal idecLocalHours = 0.0M;

                        int tempFirstQualifiedYear = (from item in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                      where item.qualified_hours != 0 || item.idecWithdrawalHours != 0
                                                      select item.year - 1).FirstOrDefault();

                        busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();

                        foreach (busPersonAccount lobjPersonAccount in this.iclbPersonAccount)
                        {
                            lclbLocalMergerCdoDummyWorkdata = new cdoDummyWorkData();
                            if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lobjPersonAccount.icdoPersonAccount.person_account_id).Count() > 0)
                            {
                                switch (lobjPersonAccount.icdoPersonAccount.plan_id)
                                {
                                    case busConstant.LOCAL_600_PLAN_ID:
                                        lclbLocalMergerCdoDummyWorkdata.year = (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_600).Year;
                                        tempLocalMergerYear = (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_600).Year;
                                        
                                        break;
                                    case busConstant.LOCAL_666_PLAN_ID:
                                        lclbLocalMergerCdoDummyWorkdata.year = (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_666).Year;
                                        tempLocalMergerYear = (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_666).Year;
                                        
                                        break;
                                    case busConstant.LOCAL_52_PLAN_ID:
                                        lclbLocalMergerCdoDummyWorkdata.year = (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52).Year;
                                        tempLocalMergerYear = (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52).Year;
                                       
                                        break;
                                    case busConstant.LOCAL_161_PLAN_ID:
                                        lclbLocalMergerCdoDummyWorkdata.year = (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_161).Year;
                                        tempLocalMergerYear = (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_161).Year;
                                      
                                        break;
                                    case busConstant.LOCAL_700_PLAN_ID:
                                        lclbLocalMergerCdoDummyWorkdata.year = (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_700).Year;
                                        tempLocalMergerYear = (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_700).Year;
                                       
                                        break;

                                }

                                lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.iclbPersonAccount.Where(item => item.icdoPersonAccount.person_account_id == lobjPersonAccount.icdoPersonAccount.person_account_id).FirstOrDefault().icdoPersonAccount.person_account_id);
                                if (lbusLocalPersonAccountEligibility != null)
                                {
                                    iintLocalQualifiedYearsCount = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                    idecLocalHours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours; //PROD PIR 774
                                }
                            }

                            lclbLocalMergerCdoDummyWorkdata.qualified_years_count = iintLocalQualifiedYearsCount;
                            lclbLocalMergerCdoDummyWorkdata.vested_years_count = iintLocalQualifiedYearsCount;
                            lclbLocalMergerCdoDummyWorkdata.iintHealthCount = iintLocalQualifiedYearsCount;
                            //lclbLocalMergerCdoDummyWorkdata.comments = "Local Frozen Service";
                            lclbLocalMergerCdoDummyWorkdata.qualified_hours = idecLocalHours;
                            lclbLocalMergerCdoDummyWorkdata.vested_hours = idecLocalHours;
                            lclbLocalMergerCdoDummyWorkdata.idecTotalHealthHours = idecLocalHours;
                            lclbLocalMergerCdoDummyWorkdata.istrPlanCode = lobjPersonAccount.icdoPersonAccount.istrPlanCode;
                            if (istrPlanCode.IsNotNullOrEmpty())
                                istrPlanCode += ", " + lobjPersonAccount.icdoPersonAccount.istrPlanCode;
                            else
                                istrPlanCode = lobjPersonAccount.icdoPersonAccount.istrPlanCode;
                            
                            if (iintMSS == 0)
                            {
                                lclbLocalMergerCdoDummyWorkdata.idecBenefitAmount = 
                                    (from obj in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                     where obj.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT"
                                     && obj.icdoPersonAccountRetirementContribution.transaction_type_value != "CNCA"
                                      && obj.icdoPersonAccountRetirementContribution.plan_id == lobjPersonAccount.icdoPersonAccount.plan_id
                                     select obj.icdoPersonAccountRetirementContribution.local_frozen_benefit_amount).Sum();
                            }
                            aclbPersonWorkHistory_Local.Add(lclbLocalMergerCdoDummyWorkdata);
                        }
                        if (aclbPersonWorkHistory_Local != null && aclbPersonWorkHistory_Local.Count() > 0 && aclbPersonWorkHistory_Local.Select(item => item.year).Distinct().Count() == 1)
                        {
                            cdoDummyWorkData lcdoLocalDummyWorkData = new cdoDummyWorkData();
                            lcdoLocalDummyWorkData.qualified_years_count = (from item in aclbPersonWorkHistory_Local select item.qualified_years_count).Sum();
                            lcdoLocalDummyWorkData.vested_years_count = (from item in aclbPersonWorkHistory_Local select item.vested_years_count).Sum();
                            lcdoLocalDummyWorkData.iintHealthCount = (from item in aclbPersonWorkHistory_Local select item.iintHealthCount).Sum();
                            iintLocalQualifiedYearsCount = (from item in aclbPersonWorkHistory_Local select item.qualified_years_count).Sum();
                            idecqualified_hours = (from item in aclbPersonWorkHistory_Local select item.qualified_hours).Sum(); //PROD PIR 774
                            lcdoLocalDummyWorkData.comments = istrPlanCode + " Frozen Service";
                            lcdoLocalDummyWorkData.qualified_hours = (from item in aclbPersonWorkHistory_Local select item.qualified_hours).Sum();
                            lcdoLocalDummyWorkData.vested_hours = (from item in aclbPersonWorkHistory_Local select item.vested_hours).Sum();
                            lcdoLocalDummyWorkData.idecTotalHealthHours = (from item in aclbPersonWorkHistory_Local select item.idecTotalHealthHours).Sum();
                            lcdoLocalDummyWorkData.year = (from item in aclbPersonWorkHistory_Local select item.year).FirstOrDefault();
                            lcdoLocalDummyWorkData.idecBenefitAmountLocal = (from item in aclbPersonWorkHistory_Local select item.idecBenefitAmount).FirstOrDefault();

                            if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == lclbLocalMergerCdoDummyWorkdata.year).Count() > 0)      
                                lcdoLocalDummyWorkData.intSequenceNumber = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == lcdoLocalDummyWorkData.year).FirstOrDefault().intSequenceNumber - 0.5M;
                            //PIR 873
                            else if(lbusBenefitApplication.aclbPersonWorkHistory_MPI.Min(item => item.year) > lclbLocalMergerCdoDummyWorkdata.year)
                                lcdoLocalDummyWorkData.intSequenceNumber = lbusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(item=>item.year).FirstOrDefault().intSequenceNumber - 0.5M;
                            else
                                lcdoLocalDummyWorkData.intSequenceNumber = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().intSequenceNumber + 0.5M;
                                                        
                            lbusBenefitApplication.aclbPersonWorkHistory_MPI.Add(lcdoLocalDummyWorkData);

                            if (lcdoLocalDummyWorkData.year <= tempFirstQualifiedYear || tempFirstQualifiedYear == 0) //PROD PIR 774
                                lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >= lcdoLocalDummyWorkData.year).ToList().ToCollection();
                            else
                                lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year > tempFirstQualifiedYear).ToList().ToCollection();
                        }
                        //tusharchandak
                        else
                            if (aclbPersonWorkHistory_Local != null && aclbPersonWorkHistory_Local.Count() > 0)
                        {
                            //PIR 1004 
                            aclbPersonWorkHistory_Local = aclbPersonWorkHistory_Local.OrderBy(t => t.year).ToList().ToCollection();
                            foreach (cdoDummyWorkData tcdoDummyWorkData in aclbPersonWorkHistory_Local)
                            {
                                cdoDummyWorkData lcdoLocalDummyWorkData = new cdoDummyWorkData();
                                lcdoLocalDummyWorkData.qualified_years_count = tcdoDummyWorkData.qualified_years_count;
                                lcdoLocalDummyWorkData.vested_years_count = tcdoDummyWorkData.vested_years_count;
                                lcdoLocalDummyWorkData.iintHealthCount = tcdoDummyWorkData.iintHealthCount;
                                idtLocalQualifiedYearsCount.Add(tcdoDummyWorkData.istrPlanCode, tcdoDummyWorkData.qualified_years_count);
                                iintLocalQualifiedYearsCount = tcdoDummyWorkData.qualified_years_count;
                                idtLocalQualifiedHours.Add(tcdoDummyWorkData.istrPlanCode, Convert.ToInt32(tcdoDummyWorkData.qualified_hours));
                               
                                lcdoLocalDummyWorkData.comments = tcdoDummyWorkData.istrPlanCode + " Frozen Service";
                                lcdoLocalDummyWorkData.qualified_hours = tcdoDummyWorkData.qualified_hours;
                                lcdoLocalDummyWorkData.vested_hours = tcdoDummyWorkData.vested_hours;
                                lcdoLocalDummyWorkData.idecTotalHealthHours = tcdoDummyWorkData.idecTotalHealthHours;
                                lcdoLocalDummyWorkData.year = tcdoDummyWorkData.year;
                                lcdoLocalDummyWorkData.idecBenefitAmountLocal = tcdoDummyWorkData.idecBenefitAmount;

                                //PIR 1004
                                if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == lcdoLocalDummyWorkData.year).Count() > 0)
                                    lcdoLocalDummyWorkData.intSequenceNumber = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == lcdoLocalDummyWorkData.year).FirstOrDefault().intSequenceNumber - 0.5M;
                                //PIR 873
                                else if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Min(item => item.year) > lclbLocalMergerCdoDummyWorkdata.year)
                                    lcdoLocalDummyWorkData.intSequenceNumber = lbusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(item => item.year).FirstOrDefault().intSequenceNumber - 0.5M;
                                else
                                    lcdoLocalDummyWorkData.intSequenceNumber = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().intSequenceNumber + 0.5M;
                                lbusBenefitApplication.aclbPersonWorkHistory_MPI.Add(lcdoLocalDummyWorkData);
                                //PIR 1004 
                                if (lcdoLocalDummyWorkData.year <= tempFirstQualifiedYear || tempFirstQualifiedYear == 0) //PROD PIR 774
                                {
                                    lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >= lcdoLocalDummyWorkData.year).ToList().ToCollection();
                                    tempFirstQualifiedYear = lcdoLocalDummyWorkData.year - 1;
                                }
                                else
                                {
                                    lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year > tempFirstQualifiedYear).ToList().ToCollection();
                                }
                            }
                        }
                        lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(i => i.intSequenceNumber).ToList().ToCollection();
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
                            if (item.comments.IsNotNullOrEmpty() && item.comments.Contains("Frozen Service") && idtLocalQualifiedYearsCount != null && idtLocalQualifiedYearsCount.Count() > 0)
                            {
                                string Plan = item.comments.Replace("Frozen Service", "").Trim();
                                if (idtLocalQualifiedYearsCount.Keys.Contains(Plan))
                                {
                                    iintLocalQualifiedYearsCount = idtLocalQualifiedYearsCount[Plan];
                                }
                                if (idtLocalQualifiedHours.Keys.Contains(Plan))
                                {
                                    idecqualified_hours = idtLocalQualifiedHours[Plan];
                                }
                            }
                            if (iintLocalQualifiedYearsCount != 0 && idecqualified_hours != 0.0M && idecqualified_hours == item.qualified_hours) //PIR 753
                            {
                                item.qualified_years_count = Convert.ToInt32(lintTotalqualifiedYearsCount) + iintLocalQualifiedYearsCount;
                                item.vested_years_count = Convert.ToInt32(lintTotalvestedYearsCount) + iintLocalQualifiedYearsCount;
                                item.iintHealthCount = Convert.ToInt32(lintTotalHealthYearsCount) + iintLocalQualifiedYearsCount;
                                lintTotalqualifiedYearsCount += iintLocalQualifiedYearsCount;
                                lintTotalvestedYearsCount += iintLocalQualifiedYearsCount;
                                lintTotalHealthYearsCount += iintLocalQualifiedYearsCount;
                                iintLocalQualifiedYearsCount = 0;
                            }
                            //tusharchandak
                            else if (iintLocalQualifiedYearsCount == 0 && (item.comments.IsNotNullOrEmpty() && item.comments.Contains("Frozen Service")))
                            {
                                tempYear = 0;
                                item.qualified_years_count = Convert.ToInt32(lintTotalqualifiedYearsCount) + iintLocalQualifiedYearsCount;
                                item.vested_years_count = Convert.ToInt32(lintTotalvestedYearsCount) + iintLocalQualifiedYearsCount;
                                item.iintHealthCount = Convert.ToInt32(lintTotalHealthYearsCount) + iintLocalQualifiedYearsCount;
                            }
                            else
                            {
                                if (item.comments.IsNotNullOrEmpty() && item.comments.Contains("FORFEITURE AT THE END OF THE YEAR") && tempYear != item.year)
                                {
                                    lintTotalvestedYearsCount = 0;
                                    lintTotalqualifiedYearsCount = 0;
                                }
                                else
                                {
                                    //PROD PIR 374
                                    if (ldtWdrlDateBefore1976.Year >= item.year && tempYear != item.year)
                                    {
                                        if ((item.idecTempqualified_hours - item.idecWithdrawalHours) >= 400)
                                        {
                                            lintTotalqualifiedYearsCount = 1;
                                            item.qualified_years_count = 1;
                                            lintTotalvestedYearsCount = 1;
                                            item.vested_years_count = 1;
                                        }
                                        else
                                        {
                                            lintTotalqualifiedYearsCount = 0;
                                            item.qualified_years_count = 0;
                                            lintTotalvestedYearsCount = 0;
                                            item.vested_years_count = 0;
                                        }
                                    }
                                    //Mailed issue vested year count is wrong on 10/08/2015 (Related to PIR 374)
                                    else if ((item.qualified_hours >= 400 || item.idecTempqualified_hours >= 400 || item.vested_hours >= 400) && tempYear != item.year)
                                    {
                                        if (item.qualified_hours >= 400 || item.idecTempqualified_hours >= 400)
                                            lintTotalqualifiedYearsCount++;
                                        item.qualified_years_count = Convert.ToInt32(lintTotalqualifiedYearsCount);
                                        if (item.vested_hours >= 400 || item.idecTempqualified_hours >= 400)
                                            lintTotalvestedYearsCount++;
                                        item.vested_years_count = Convert.ToInt32(lintTotalvestedYearsCount);
                                    }
                                   
                                    else
                                    {
                                        if((lbusBenefitApplication.icdoBenefitApplication.retirement_date == DateTime.MinValue ||lbusBenefitApplication.icdoBenefitApplication.retirement_date.Year >= 2023) && item.year == 2023 && item.qualified_hours >=65 && item.qualified_hours < 400)
                                        {
                                            lintTotalqualifiedYearsCount++;
                                            item.qualified_years_count = Convert.ToInt32(lintTotalqualifiedYearsCount);
                                            item.vested_years_count = Convert.ToInt32(lintTotalvestedYearsCount);

                                        }
                                        else
                                        {
                                            item.qualified_years_count = Convert.ToInt32(lintTotalqualifiedYearsCount);
                                            item.vested_years_count = Convert.ToInt32(lintTotalvestedYearsCount);

                                        }
                                       
                                    }
                                    
                                    if ((item.idecTotalHealthHours >= 400 || item.idecTempqualified_hours >= 400) && tempYear != item.year)
                                    {
                                        lintTotalHealthYearsCount++;
                                        item.iintHealthCount = Convert.ToInt32(lintTotalHealthYearsCount);
                                    }
                                    else
                                    {
                                        item.iintHealthCount = Convert.ToInt32(lintTotalHealthYearsCount);
                                    }

                                }
                                tempYear = item.year;
                            }
                        }
                        else
                        {
                            if (ldtWdrlDateBefore1976.Year >= item.year)
                            {
                                lintTotalqualifiedYearsCount = 0;
                                item.qualified_years_count = 0;
                                lintTotalvestedYearsCount = 0;
                                item.vested_years_count = 0;
                                item.vested_hours = 0.0M;
                            }
                            if (item.idecTotalHealthHours >= 400)
                            {
                                lintTotalHealthYearsCount++;
                                item.iintHealthCount = Convert.ToInt32(lintTotalHealthYearsCount);
                            }
                            else
                            {
                                item.iintHealthCount = Convert.ToInt32(lintTotalHealthYearsCount);
                            }
                        }
                    }
                }
                #endregion

                #region Summary Grid
                if (lbusPersonAccountEligibility != null && lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date != DateTime.MinValue)
                {
                    ldecTotalCreditedHours = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI where items.year > lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year select items.qualified_hours).Sum(), 2);
                    ldecTotalVestedHours = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI where items.year > lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year select items.vested_hours).Sum(), 2);
                    ldecTotalQualifiedIAPHours = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI where items.year > lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year select items.idecTotalIAPHours).Sum(), 2);
                    ldecTotalQualifiedAccuruedBenefit = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                    where items.year > lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year
                                                                    select items.idecBenefitAmount).Sum(), 2);
                    ldecTotalQualifiedAccruedBenefitLocal = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                        where items.year > lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year
                                                                        select items.idecBenefitAmountLocal).Sum(), 2);

                }
                else
                {
                    ldecTotalCreditedHours = Math.Round((from item in lbusBenefitApplication.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum(), 2);
                    ldecTotalVestedHours = Math.Round((from item in lbusBenefitApplication.aclbPersonWorkHistory_MPI select item.vested_hours).Sum(), 2);
                    ldecTotalQualifiedIAPHours = Math.Round((from item in lbusBenefitApplication.aclbPersonWorkHistory_MPI select item.idecTotalIAPHours).Sum(), 2);
                    ldecTotalQualifiedAccuruedBenefit = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                    select items.idecBenefitAmount).Sum(), 2);
                    ldecTotalQualifiedAccruedBenefitLocal = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                        select items.idecBenefitAmountLocal).Sum(), 2);
                }

                lintTotalHealthYearsCount = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().iintHealthCount;
                lintTotalqualifiedYearsCount = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                lintNonQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count == 0 ? 0 : lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().iintNonQualifiedYears;//PIR 970
                lintTotalvestedYearsCount = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count == 0 ? 0 : lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count;
               
                ldecTotalEEContribution = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                      select items.idecEEContribution).Sum(), 2);
                ldecTotalEEInterestContribution = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                              select items.idecEEInterest).Sum(), 2);
                ldecTotalUVHPContribution = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                        select items.idecUVHPContribution).Sum(), 2);
                ldecTotalUVHPInterestContribution = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                select items.idecUVHPInterest).Sum(), 2);

                ldecWithdrawalHoursSum = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                select items.idecWithdrawalHours).Sum(), 2);

                ldecTotalHealthHoursSum = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                     select items.idecTotalHealthHours).Sum(), 2);
                #endregion
            }
            #endregion

            #region Person Overview Summary
            if (flagYes)
            {
                LoadPlanDetails();
            }
            #endregion
        }

        public void GetTotalHours()
        {
            if (!lbusBenefitApplication.aclbPersonWorkHistory_IAP.IsNullOrEmpty())
            {
                ldecTotalIAPHours = (from items in lbusBenefitApplication.aclbPersonWorkHistory_IAP select items.qualified_hours).Sum();
                ldecTotalIAPQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().qualified_years_count;
                ldecTotalIAPVestedYears = lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().vested_years_count;
            }

            if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
            {
                ldecTotalPensionHours = (from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.qualified_hours).Sum();
                ldecTotalPensionQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                ldecTotalPensionVestedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count;
            }
        }

        public decimal LoadAccruedBenefitPerYear(busBenefitApplication abusBenefitApplication, DateTime adtForfeitureDate)
        {
            busCalculation lbusCalculation = new busCalculation();
            busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            decimal ldecUnreducedBenefitAmount = 0;

            if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
            {
                lbusPersonAccount = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();

                ldecUnreducedBenefitAmount = lbusCalculation.CalculateUnReducedBenefitAmtForPension(abusBenefitApplication.ibusPerson, abusBenefitApplication.idecAge,
                                            abusBenefitApplication.icdoBenefitApplication.retirement_date, lbusPersonAccount, this.lbusBenefitApplication,
                                            false, null, null, abusBenefitApplication.aclbPersonWorkHistory_MPI, string.Empty, string.Empty);
                if (adtForfeitureDate != DateTime.MinValue)
                {
                    //abusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >
                    //        lbusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year).ToList().ToCollection();

                    ldecUnreducedBenefitAmount = abusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >
                                                 adtForfeitureDate.Year).Sum(item => item.idecBenefitAmount);

                }
            }

            return ldecUnreducedBenefitAmount;
        }


        #region Recalculate EE Interest

        public void LoadEEcontributions()
        {
            DateTime ldtVestingDate = new DateTime();
            DateTime ldtForfietureDate = new DateTime();
            DateTime ldtWithdrawalDate = new DateTime();
            int lintPersonAccountId = 0;
            iclbPersonAccount = new Collection<busPersonAccount>();
            iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            LoadPersonAccounts();

            busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
            lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
            if (lbusPersonAccountEligibility != null)
            {
                ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                ldtForfietureDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
            }
            icdoPerson.idtForfietureDate = ldtForfietureDate;
            icdoPerson.idtVestingDate = ldtVestingDate;


            if (iclbPersonAccount != null && iclbPersonAccount.Count > 0 &&
                iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
            {
                lintPersonAccountId = iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).
                    FirstOrDefault().icdoPersonAccount.person_account_id;

                iintPersonAccountId = lintPersonAccountId;

                Collection<busPersonAccountRetirementContribution> lcblPersonAccountRetirementContributionOPUSInterest = new Collection<busPersonAccountRetirementContribution>();

                DataTable ldtbList = Select<cdoPersonAccountRetirementContribution>(
                new string[2] { enmPersonAccountRetirementContribution.person_account_id.ToString(), enmPersonAccountRetirementContribution.contribution_type_value.ToString() },
                new object[2] { lintPersonAccountId, "EE" }, null, "computational_year");
                iclbPersonAccountRetirementContribution =
                    GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");

                lcblPersonAccountRetirementContributionOPUSInterest =
                    GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");

                iclbPersonAccountRetirementContribution =
                    iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.transaction_type_value !=
                        busConstant.TRANSACTION_TYPE_INTEREST && item.icdoPersonAccountRetirementContribution.transaction_type_value != busConstant.RCTransactionTypeAdjustment).ToList().ToCollection();

                iclbPersonAccountRetirementContribution =
                    iclbPersonAccountRetirementContribution.OrderBy(t => t.icdoPersonAccountRetirementContribution.computational_year).ToList().ToCollection();

                Collection<busPersonAccountRetirementContribution> lclbContributionPayments = new Collection<busPersonAccountRetirementContribution>();

                //PIR 1094
                if (iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.transaction_type_value ==
                        busConstant.RCTransactionTypePayment && 
                        (item.icdoPersonAccountRetirementContribution.ee_contribution_amount != decimal.Zero || item.icdoPersonAccountRetirementContribution.ee_int_amount != decimal.Zero)).Count() > 0)
                {
                    lclbContributionPayments = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.transaction_type_value ==
                            busConstant.RCTransactionTypePayment &&
                        (item.icdoPersonAccountRetirementContribution.ee_contribution_amount != decimal.Zero || item.icdoPersonAccountRetirementContribution.ee_int_amount != decimal.Zero)).ToList().ToCollection();

                    ldtWithdrawalDate = lclbContributionPayments.OrderByDescending(t => t.icdoPersonAccountRetirementContribution.computational_year).
                        ThenByDescending(item => item.icdoPersonAccountRetirementContribution.effective_date).FirstOrDefault().icdoPersonAccountRetirementContribution.effective_date;

                    icdoPerson.idtWithdrawalDate = ldtWithdrawalDate;

                }


                if (ldtForfietureDate == DateTime.MinValue && ldtWithdrawalDate == DateTime.MinValue && ldtVestingDate != DateTime.MinValue)
                {

                    foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in iclbPersonAccountRetirementContribution)
                    {
                        if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT")
                        {
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                        }
                    }
                }
                else if (ldtForfietureDate == DateTime.MinValue && ldtWithdrawalDate != DateTime.MinValue && ldtVestingDate != DateTime.MinValue &&
                    ldtVestingDate.Year < ldtWithdrawalDate.Year)
                {
                    foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in iclbPersonAccountRetirementContribution)
                    {
                        //PIR 1094
                        //if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT")
                        //{
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                        //}
                    }
                }
                else if (ldtForfietureDate == DateTime.MinValue && ldtWithdrawalDate != DateTime.MinValue && ldtVestingDate != DateTime.MinValue &&
                     ldtVestingDate.Year >= ldtWithdrawalDate.Year)
                {
                    foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in iclbPersonAccountRetirementContribution)
                    {
                        if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT")
                        {
                            if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year <= ldtWithdrawalDate.Year)
                            {
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;
                            }
                            else
                            {
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                            }
                        }
                    }
                }
                else if (ldtForfietureDate != DateTime.MinValue && ldtWithdrawalDate == DateTime.MinValue && ldtVestingDate != DateTime.MinValue &&
                                ldtVestingDate.Year > ldtForfietureDate.Year)
                {
                    foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in iclbPersonAccountRetirementContribution)
                    {
                        if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT")
                        {
                            if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year <= ldtForfietureDate.Year)
                            {
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;
                            }
                            else
                            {
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                            }
                        }
                    }
                }
                else if (ldtForfietureDate != DateTime.MinValue && ldtWithdrawalDate != DateTime.MinValue && ldtVestingDate != DateTime.MinValue &&
                               ldtVestingDate.Year > ldtForfietureDate.Year && ldtVestingDate.Year > ldtWithdrawalDate.Year)
                {
                    foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in iclbPersonAccountRetirementContribution)
                    {
                        if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT")
                        {
                            if (ldtForfietureDate.Year >= ldtWithdrawalDate.Year)
                            {
                                if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year <= ldtForfietureDate.Year)
                                {
                                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;
                                }
                                else
                                {
                                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                                }
                            }
                            else
                            {
                                if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year <= ldtWithdrawalDate.Year)
                                {
                                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;
                                }
                                else
                                {
                                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                                }
                            }
                        }
                    }
                }
                //Rohan 05152014
                else if (ldtForfietureDate != DateTime.MinValue && ldtWithdrawalDate != DateTime.MinValue && ldtVestingDate != DateTime.MinValue &&
                              ldtVestingDate.Year > ldtForfietureDate.Year && ldtWithdrawalDate.Year > ldtVestingDate.Year)
                {
                    foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in iclbPersonAccountRetirementContribution)
                    {
                        if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT")
                        {
                            if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year <= ldtForfietureDate.Year)
                            {
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;
                            }
                            else
                            {
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                            }
                        }

                    }
                }
                else if (ldtForfietureDate != DateTime.MinValue && ldtWithdrawalDate != DateTime.MinValue && ldtVestingDate == DateTime.MinValue)
                {
                    foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in iclbPersonAccountRetirementContribution)
                    {
                        if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value != "PMNT")
                        {
                            if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year <= ldtForfietureDate.Year)
                            {
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;
                            }
                            else
                            {
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                            }
                        }

                    }
                }

                decimal ldecTotalPrevYearNonVestedEEContribAmt = 0, ldecTotalPrevYearNonVestedEEInterestAmt = 0;
                decimal ldecTotalPrevYearNonVestedEEPayment = 0, ldecTotalPrevYearNonVestedEEInterestPayment = 0;
                decimal ldecCurrentYearNonVestedEEPayment = 0, ldecCurrentYearNonVestedEEInterestPayment = 0;
                decimal ldecCurrentYearNonVestedEEContribution = 0, ldecCurrentYearNonVestedEEInterest = 0;

                decimal ldecTotalPrevYearVestedEEContribAmt = 0, ldecTotalPrevYearVestedEEInterestAmt = 0;
                decimal ldecTotalPrevYearVestedEEPayment = 0, ldecTotalPrevYearVestedEEInterestPayment = 0;
                decimal ldecCurrentYearVestedEEPayment = 0, ldecCurrentYearVestedEEInterestPayment = 0;
                decimal ldecCurrentYearVestedEEContribution = 0, ldecCurrentYearVestedEEInterest = 0;


                int lintEarliestComputationYear = GetEarliestComputationYear();
                int lintLastComputationYear = 0;

                DataTable ldtbRetirementDate = Select("cdoPerson.GetRetirementDate", new object[1] { icdoPerson.person_id });
                if (ldtbRetirementDate != null && ldtbRetirementDate.Rows.Count > 0 && Convert.ToString(ldtbRetirementDate.Rows[0][0]).IsNotNullOrEmpty())
                {
                    icdoPerson.idtLastDate = Convert.ToDateTime(ldtbRetirementDate.Rows[0][0]);
                    lintLastComputationYear = icdoPerson.idtLastDate.Year;
                }
                else if (icdoPerson.date_of_death != DateTime.MinValue)
                {
                    DataTable ldtbFirstPaymentDate = Select("cdoPerson.GetFirstPaymentDateAfterActiveDate", new object[1] { icdoPerson.person_id });
                    if (ldtbFirstPaymentDate != null && ldtbFirstPaymentDate.Rows.Count > 0 && Convert.ToString(ldtbFirstPaymentDate.Rows[0][0]).IsNotNullOrEmpty())
                    {
                        icdoPerson.idtLastDate = Convert.ToDateTime(ldtbFirstPaymentDate.Rows[0][0]);
                    }
                    else
                    {
                        //icdoPerson.idtLastDate = icdoPerson.date_of_death;
                        icdoPerson.idtLastDate = busGlobalFunctions.GetLastDateOfComputationYear(DateTime.Now.Year - 1);
                    }
                    lintLastComputationYear = icdoPerson.idtLastDate.Year;

                }
                else
                {
                    icdoPerson.idtLastDate = busGlobalFunctions.GetLastDateOfComputationYear(DateTime.Now.Year - 1);
                    lintLastComputationYear = icdoPerson.idtLastDate.Year;
                }


                for (int lintCompYr = lintEarliestComputationYear + 1; lintCompYr <= lintLastComputationYear; lintCompYr++)
                {

                    ldecCurrentYearNonVestedEEPayment = 0; ldecCurrentYearNonVestedEEInterestPayment = 0;
                    ldecCurrentYearNonVestedEEContribution = 0; ldecCurrentYearNonVestedEEInterest = 0;
                    ldecCurrentYearVestedEEPayment = 0; ldecCurrentYearVestedEEInterestPayment = 0;
                    ldecCurrentYearVestedEEContribution = 0; ldecCurrentYearVestedEEInterest = 0;


                    // busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == lintCompYr).First();
                    int aintComputationYear = Convert.ToInt32(lintCompYr);


                    decimal ldecRateOfInterest = 0;
                    DataTable ldtbInterestRateInformation = busBase.Select<cdoBenefitInterestRate>(new string[1] { enmBenefitInterestRate.year.ToString() }, new object[1] { Math.Max(aintComputationYear, 1975) }, null, null);

                    if (ldtbInterestRateInformation.Rows.Count > 0)
                    {
                        ldecRateOfInterest = Convert.ToDecimal(ldtbInterestRateInformation.Rows[0][enmBenefitInterestRate.rate_of_interest.ToString()]);
                    }


                    #region Non Vested
                    //Non Vested Contribution
                    if (iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                        && t.icdoPersonAccountRetirementContribution.transaction_type_value == null
                        && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).Count() > 0)
                    {
                        ldecCurrentYearNonVestedEEContribution = iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                       && t.icdoPersonAccountRetirementContribution.transaction_type_value == null
                       && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                    }

                    //Non Vested Interest
                    if (iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                      && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TRANSACTION_TYPE_INTEREST
                      && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).Count() > 0)
                    {
                        ldecCurrentYearNonVestedEEInterest = iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                       && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TRANSACTION_TYPE_INTEREST
                       && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).Sum(t => t.icdoPersonAccountRetirementContribution.ee_int_amount);
                    }


                    //Non Vested Payment
                    if (iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                      && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypePayment
                      && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).Count() > 0)
                    {
                        ldecCurrentYearNonVestedEEPayment = iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                       && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypePayment
                       && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                    }

                    //Non Vested Payment Interest
                    if (iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                      && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypePayment
                      && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).Count() > 0)
                    {
                        ldecCurrentYearNonVestedEEInterestPayment = iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                       && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypePayment
                       && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED).Sum(t => t.icdoPersonAccountRetirementContribution.ee_int_amount);
                    }
                    #endregion Non Vested

                    #region Vested
                    //Vested Contribution
                    if (iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                        && t.icdoPersonAccountRetirementContribution.transaction_type_value == null
                        && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Count() > 0)
                    {
                        ldecCurrentYearVestedEEContribution = iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                       && t.icdoPersonAccountRetirementContribution.transaction_type_value == null
                       && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                    }

                    //Vested Interest
                    if (iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                      && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TRANSACTION_TYPE_INTEREST
                      && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Count() > 0)
                    {
                        ldecCurrentYearVestedEEInterest = iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                       && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TRANSACTION_TYPE_INTEREST
                       && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Sum(t => t.icdoPersonAccountRetirementContribution.ee_int_amount);
                    }


                    //Vested Payment
                    if (iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                      && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypePayment
                      && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Count() > 0)
                    {
                        //PROD PIR 400
                        ldecCurrentYearVestedEEPayment = iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                       && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypePayment
                       && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                    }

                    //Vested Payment Interest
                    if (iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                      && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypePayment
                      && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Count() > 0)
                    {
                        ldecCurrentYearVestedEEInterestPayment = iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == aintComputationYear
                       && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypePayment
                       && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Sum(t => t.icdoPersonAccountRetirementContribution.ee_int_amount);
                    }
                    #endregion Vested

                    CalculateEEInterest(aintComputationYear, ldecRateOfInterest, ldecTotalPrevYearNonVestedEEContribAmt, ldecTotalPrevYearNonVestedEEInterestAmt, ldecTotalPrevYearNonVestedEEPayment,
                        ldecTotalPrevYearNonVestedEEInterestPayment, ldecCurrentYearNonVestedEEPayment,
                        ldecCurrentYearNonVestedEEInterestPayment, ref ldecCurrentYearNonVestedEEContribution, ref ldecCurrentYearNonVestedEEInterest, false);

                    //if (lintCompYr == ldtForfietureDate.Year)
                    //{
                    //    ldecTotalPrevYearNonVestedEEContribAmt = 0;
                    //    ldecTotalPrevYearNonVestedEEInterestAmt = 0;
                    //    ldecTotalPrevYearNonVestedEEPayment = 0;
                    //    ldecTotalPrevYearNonVestedEEInterestPayment = 0;
                    //}
                    //else
                    //{
                    ldecTotalPrevYearNonVestedEEContribAmt += ldecCurrentYearNonVestedEEContribution;
                    ldecTotalPrevYearNonVestedEEInterestAmt += ldecCurrentYearNonVestedEEInterest;
                    ldecTotalPrevYearNonVestedEEPayment += ldecCurrentYearNonVestedEEPayment;
                    ldecTotalPrevYearNonVestedEEInterestPayment += ldecCurrentYearNonVestedEEInterestPayment;
                    //}


                    CalculateEEInterest(aintComputationYear, ldecRateOfInterest, ldecTotalPrevYearVestedEEContribAmt, ldecTotalPrevYearVestedEEInterestAmt, ldecTotalPrevYearVestedEEPayment,
                   ldecTotalPrevYearVestedEEInterestPayment, ldecCurrentYearVestedEEPayment,
                   ldecCurrentYearVestedEEInterestPayment, ref ldecCurrentYearVestedEEContribution, ref ldecCurrentYearVestedEEInterest, true);

                    ldecTotalPrevYearVestedEEContribAmt += ldecCurrentYearVestedEEContribution;
                    ldecTotalPrevYearVestedEEInterestAmt += ldecCurrentYearVestedEEInterest;
                    ldecTotalPrevYearVestedEEPayment += ldecCurrentYearVestedEEPayment;
                    ldecTotalPrevYearVestedEEInterestPayment += ldecCurrentYearVestedEEInterestPayment;


                    if (lintCompYr == ldtWithdrawalDate.Year && ldtForfietureDate.Year < ldtWithdrawalDate.Year)
                    {

                        //1.Insert a neagtive Adjustment Entry for non vested remaining contribution(zero out remaining bal after withdrawal)
                        //2.Insert positive entry for vested remaining contributions(remaining balance after withdrawal
                        //3.If current year's non vested contribution is less than the remaining balance then 
                        //deduction current year's non vested contribution  from  remaining withdrawal balance and compare remaining withdrawal balance
                        //with previous year's non vested contribution 
                        //4.Calculate vested interest for all the previous years upto the current year. 

                        ArrayList larrYears = new ArrayList();
                        int lintTemp = lintCompYr;
                        decimal ldecTotalAmount = ldecTotalPrevYearNonVestedEEContribAmt + ldecTotalPrevYearNonVestedEEPayment;


                        while (ldecTotalAmount > 0)
                        {


                            decimal ldecCurrentYearContribution = 0;

                            //Change
                            if (iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.transaction_type_value == null
                            && t.icdoPersonAccountRetirementContribution.computational_year == lintTemp).Count() > 0)
                            {
                                ldecCurrentYearContribution = iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.transaction_type_value == null
                                    && t.icdoPersonAccountRetirementContribution.computational_year == lintTemp).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount); //RID 53417 
                            }

                            if (ldecCurrentYearContribution > ldecTotalAmount)
                            {
                                ldecCurrentYearContribution = ldecTotalAmount;
                            }

                            larrYears.Add(lintTemp);

                            busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date = DateTime.Now;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = iintPersonAccountId;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_contribution_amount = -(ldecCurrentYearContribution);
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year = lintTemp;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = busGlobalFunctions.GetLastDateOfComputationYear(lintTemp).AddDays(1);
                            //lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.ee_int_amount = 
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.RCTransactionTypeAdjustment;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;
                            iclbPersonAccountRetirementContribution.Add(lbusPersonAccountRetirementContribution);


                            busPersonAccountRetirementContribution lbusPersonAccountRetirementContributionVested = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                            lbusPersonAccountRetirementContributionVested.icdoPersonAccountRetirementContribution.transaction_date = DateTime.Now;
                            lbusPersonAccountRetirementContributionVested.icdoPersonAccountRetirementContribution.person_account_id = iintPersonAccountId;
                            lbusPersonAccountRetirementContributionVested.icdoPersonAccountRetirementContribution.ee_contribution_amount = ldecCurrentYearContribution;
                            lbusPersonAccountRetirementContributionVested.icdoPersonAccountRetirementContribution.computational_year = lintTemp;
                            lbusPersonAccountRetirementContributionVested.icdoPersonAccountRetirementContribution.effective_date = busGlobalFunctions.GetLastDateOfComputationYear(lintTemp).AddDays(1);
                            //lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.ee_int_amount = 
                            lbusPersonAccountRetirementContributionVested.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.RCTransactionTypeAdjustment;
                            lbusPersonAccountRetirementContributionVested.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                            lbusPersonAccountRetirementContributionVested.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                            iclbPersonAccountRetirementContribution.Add(lbusPersonAccountRetirementContributionVested);


                            ldecTotalAmount -= ldecCurrentYearContribution;

                            lintTemp--;

                        }


                        larrYears.Sort();
                        foreach (int lintYear in larrYears)
                        {
                            //Vested Contribution
                            if (iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == lintYear
                                && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypeAdjustment
                                && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Count() > 0)
                            {
                                ldecCurrentYearVestedEEContribution = iclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == lintYear
                               && t.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypeAdjustment
                               && t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                            }


                            CalculateEEInterest(lintYear, ldecRateOfInterest, ldecTotalPrevYearVestedEEContribAmt, ldecTotalPrevYearVestedEEInterestAmt, ldecTotalPrevYearVestedEEPayment,
                            ldecTotalPrevYearVestedEEInterestPayment, ldecCurrentYearVestedEEPayment,
                            ldecCurrentYearVestedEEInterestPayment, ref ldecCurrentYearVestedEEContribution, ref ldecCurrentYearVestedEEInterest, true);

                            ldecTotalPrevYearVestedEEContribAmt += ldecCurrentYearVestedEEContribution;
                            ldecTotalPrevYearVestedEEInterestAmt += ldecCurrentYearVestedEEInterest;
                            ldecTotalPrevYearVestedEEPayment += ldecCurrentYearVestedEEPayment;
                            ldecTotalPrevYearVestedEEInterestPayment += ldecCurrentYearVestedEEInterestPayment;
                        }



                        //ldecTotalPrevYearVestedEEContribAmt += ldecTotalPrevYearNonVestedEEContribAmt + ldecTotalPrevYearNonVestedEEPayment;
                        ldecTotalPrevYearNonVestedEEContribAmt = 0;
                        ldecTotalPrevYearNonVestedEEPayment = 0;

                    }
                }

                iclbPersonAccountRetirementContribution = iclbPersonAccountRetirementContribution.OrderBy(t => t.icdoPersonAccountRetirementContribution.computational_year).ToList().ToCollection<busPersonAccountRetirementContribution>();

                iclbEEContributionInterest = new Collection<busPersonAccountRetirementContribution>();

                int lintCurrentYear = 0;
                foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in iclbPersonAccountRetirementContribution)
                {
                    //if (ldtForfietureDate == DateTime.MinValue || (ldtForfietureDate != DateTime.MinValue && lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year))
                    //{
                    //if (lintCurrentYear != lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year)
                    //{
                    lintCurrentYear = Convert.ToInt32(lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year);

                    busPersonAccountRetirementContribution lEEContributionInterest = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                    lEEContributionInterest.icdoPersonAccountRetirementContribution.computational_year = lintCurrentYear;
                    lEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                    lEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_subtype_value = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value;

                    if (lEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED)
                        lEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_subtype_description = "Vested";
                    else
                        lEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_subtype_description = "Non Vested";


                    if (iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == lintCurrentYear
                        && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value).Count() > 0)
                    {
                        lEEContributionInterest.icdoPersonAccountRetirementContribution.ee_contribution_amount =
                            iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == lintCurrentYear
                            && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);

                    }

                    if (iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == lintCurrentYear
                     && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value).Count() > 0)
                    {
                        lEEContributionInterest.icdoPersonAccountRetirementContribution.ee_int_amount =
                     iclbPersonAccountRetirementContribution.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == lintCurrentYear
                     && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value).Sum(t => t.icdoPersonAccountRetirementContribution.ee_int_amount);
                    }

                    if (iclbEEContributionInterest.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == lintCurrentYear &&
                        t.icdoPersonAccountRetirementContribution.contribution_subtype_value == lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value).Count() <= 0)
                    {
                        iclbEEContributionInterest.Add(lEEContributionInterest);
                    }

                    //  }
                    // }

                }

                iclbTotalEEContributionInterest = new Collection<busPersonAccountRetirementContribution>();

                busPersonAccountRetirementContribution lTotalEEContributionInterest = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

                lTotalEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                lTotalEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                lTotalEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_subtype_description = "Vested";

                if (iclbEEContributionInterest.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED && t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year).Count() > 0)
                {
                    lTotalEEContributionInterest.icdoPersonAccountRetirementContribution.ee_contribution_amount =
                        iclbEEContributionInterest.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED && t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);


                    lTotalEEContributionInterest.icdoPersonAccountRetirementContribution.ee_int_amount =
                        iclbEEContributionInterest.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED && t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year).Sum(t => t.icdoPersonAccountRetirementContribution.ee_int_amount);
                }

                iclbTotalEEContributionInterest.Add(lTotalEEContributionInterest);

                iclbTotalEEContributionInterest = iclbTotalEEContributionInterest.OrderBy(t => t.icdoPersonAccountRetirementContribution.computational_year).ToList().ToCollection<busPersonAccountRetirementContribution>();

                busPersonAccountRetirementContribution lTotalEEContributionInterestNonVested = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

                lTotalEEContributionInterestNonVested.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                lTotalEEContributionInterestNonVested.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;
                lTotalEEContributionInterestNonVested.icdoPersonAccountRetirementContribution.contribution_subtype_description = "Non Vested";

                //Change
                if (iclbEEContributionInterest.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED //&& t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year
                    ).Count() > 0)
                {
                    lTotalEEContributionInterestNonVested.icdoPersonAccountRetirementContribution.ee_contribution_amount =
                        iclbEEContributionInterest.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED //&& t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year
                        ).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);


                    lTotalEEContributionInterestNonVested.icdoPersonAccountRetirementContribution.ee_int_amount =
                        iclbEEContributionInterest.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED //&& t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year
                        ).Sum(t => t.icdoPersonAccountRetirementContribution.ee_int_amount);
                }

                iclbTotalEEContributionInterest.Add(lTotalEEContributionInterestNonVested);

                lcblPersonAccountRetirementContributionOPUSInterest = lcblPersonAccountRetirementContributionOPUSInterest.OrderBy(t => t.icdoPersonAccountRetirementContribution.computational_year).ToList().ToCollection<busPersonAccountRetirementContribution>();

                iclbEEContributionInterestOPUS = new Collection<busPersonAccountRetirementContribution>();

                lintCurrentYear = 0;
                foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in lcblPersonAccountRetirementContributionOPUSInterest)
                {
                    lintCurrentYear = Convert.ToInt32(lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year);

                    busPersonAccountRetirementContribution lEEContributionInterest = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                    lEEContributionInterest.icdoPersonAccountRetirementContribution.computational_year = lintCurrentYear;
                    lEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                    lEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_subtype_value = lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value;

                    if (lEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED)
                        lEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_subtype_description = "Vested";
                    else
                        lEEContributionInterest.icdoPersonAccountRetirementContribution.contribution_subtype_description = "Non Vested";


                    if (lcblPersonAccountRetirementContributionOPUSInterest.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == lintCurrentYear
                        && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value).Count() > 0)
                    {
                        lEEContributionInterest.icdoPersonAccountRetirementContribution.ee_contribution_amount =
                            lcblPersonAccountRetirementContributionOPUSInterest.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == lintCurrentYear
                            && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);

                    }

                    if (lcblPersonAccountRetirementContributionOPUSInterest.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == lintCurrentYear
                     && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value).Count() > 0)
                    {
                        lEEContributionInterest.icdoPersonAccountRetirementContribution.ee_int_amount =
                     lcblPersonAccountRetirementContributionOPUSInterest.Where(item => item.icdoPersonAccountRetirementContribution.computational_year == lintCurrentYear
                     && item.icdoPersonAccountRetirementContribution.contribution_subtype_value == lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value).Sum(t => t.icdoPersonAccountRetirementContribution.ee_int_amount);
                    }

                    if (iclbEEContributionInterestOPUS.Where(t => t.icdoPersonAccountRetirementContribution.computational_year == lintCurrentYear &&
                        t.icdoPersonAccountRetirementContribution.contribution_subtype_value == lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value).Count() <= 0)
                    {
                        iclbEEContributionInterestOPUS.Add(lEEContributionInterest);
                    }
                }


                iclbTotalEEContributionInterestOPUS = new Collection<busPersonAccountRetirementContribution>();

                busPersonAccountRetirementContribution lTotalEEContributionInterestOPUS = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

                lTotalEEContributionInterestOPUS.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                lTotalEEContributionInterestOPUS.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                lTotalEEContributionInterestOPUS.icdoPersonAccountRetirementContribution.contribution_subtype_description = "Vested";

                if (iclbEEContributionInterestOPUS.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED && t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year).Count() > 0)
                {
                    lTotalEEContributionInterestOPUS.icdoPersonAccountRetirementContribution.ee_contribution_amount =
                        iclbEEContributionInterestOPUS.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED && t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);


                    lTotalEEContributionInterestOPUS.icdoPersonAccountRetirementContribution.ee_int_amount =
                        iclbEEContributionInterestOPUS.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_VESTED && t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year).Sum(t => t.icdoPersonAccountRetirementContribution.ee_int_amount);


                }

                iclbTotalEEContributionInterestOPUS.Add(lTotalEEContributionInterestOPUS);

                iclbTotalEEContributionInterestOPUS = iclbTotalEEContributionInterestOPUS.OrderBy(t => t.icdoPersonAccountRetirementContribution.computational_year).ToList().ToCollection<busPersonAccountRetirementContribution>();

                busPersonAccountRetirementContribution lTotalEEContributionInterestNonVestedOPUS = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

                lTotalEEContributionInterestNonVestedOPUS.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                lTotalEEContributionInterestNonVestedOPUS.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;
                lTotalEEContributionInterestNonVestedOPUS.icdoPersonAccountRetirementContribution.contribution_subtype_description = "Non Vested";

                //Change
                if (iclbEEContributionInterestOPUS.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED //&& t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year
                    ).Count() > 0)
                {
                    lTotalEEContributionInterestNonVestedOPUS.icdoPersonAccountRetirementContribution.ee_contribution_amount =
                        iclbEEContributionInterestOPUS.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED //&& t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year
                        ).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);


                    lTotalEEContributionInterestNonVestedOPUS.icdoPersonAccountRetirementContribution.ee_int_amount =
                        iclbEEContributionInterestOPUS.Where(t => t.icdoPersonAccountRetirementContribution.contribution_subtype_value == busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED //&& t.icdoPersonAccountRetirementContribution.computational_year > ldtForfietureDate.Year
                        ).Sum(t => t.icdoPersonAccountRetirementContribution.ee_int_amount);
                }

                iclbTotalEEContributionInterestOPUS.Add(lTotalEEContributionInterestNonVestedOPUS);

            }
        }


        public int GetEarliestComputationYear()
        {
            object lobjComputationYear = null;
            lobjComputationYear = DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.GetEarliestComputationYear",
                                    new object[] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return Convert.ToInt32(lobjComputationYear);
        }


        public void CalculateEEInterest(int aintComputationYear, decimal adecRate, decimal ldecTotalPrevYearEEContribAmt, decimal ldecTotalPrevYearEEInterestAmt,
                decimal ldecTotalPrevYearEEPayment, decimal ldecTotalPrevYearEEInterestPayment, decimal ldecCurrentYearEEPayment, decimal ldecCurrentYearEEInterestPayment,
           ref decimal ldecCurrentYearEEContribution, ref decimal ldecCurrentYearEEInterest, bool IsVestedFlag)
        {


            busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date = DateTime.Now;
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = iintPersonAccountId;

            DataTable ldtbInterestRateInformation = busBase.Select<cdoBenefitInterestRate>(new string[1] { enmBenefitInterestRate.year.ToString() }, new object[1] { Math.Max(aintComputationYear, 1975) }, null, null);

            if (ldtbInterestRateInformation.Rows.Count > 0)
            {
                adecRate = Convert.ToDecimal(ldtbInterestRateInformation.Rows[0][enmBenefitInterestRate.rate_of_interest.ToString()]);
            }


            if (icdoPerson.idtLastDate != DateTime.MinValue && icdoPerson.idtLastDate.Year == aintComputationYear)
            {
                //Change
                decimal ldecEEPartialInterestAmount = 0;
                if (icdoPerson.idtLastDate.Month == 12)
                {
                    ldecEEPartialInterestAmount = Math.Round(((ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEInterestAmt + ldecTotalPrevYearEEPayment + ldecTotalPrevYearEEInterestPayment) * adecRate), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    ldecEEPartialInterestAmount = Math.Round(((ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEInterestAmt + ldecTotalPrevYearEEPayment + ldecTotalPrevYearEEInterestPayment) * adecRate) / 12 * (icdoPerson.idtLastDate.Month - 1)
                       , 2, MidpointRounding.AwayFromZero);
                }
                ldecTotalPrevYearEEInterestAmt = ldecEEPartialInterestAmount;
            }

            if (icdoPerson.idtLastDate != DateTime.MinValue && icdoPerson.idtLastDate.Year == aintComputationYear)
            {
                if (ldecTotalPrevYearEEInterestAmt > 0)
                {
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount = ldecTotalPrevYearEEInterestAmt;
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year = aintComputationYear;
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date =
                        busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(aintComputationYear)).AddDays(1);
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TRANSACTION_TYPE_INTEREST;
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                    if (IsVestedFlag)
                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                    else
                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;

                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount = busConstant.ZERO_DECIMAL;
                    if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount > 0)
                    {
                        ldecCurrentYearEEInterest += lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount;
                        iclbPersonAccountRetirementContribution.Add(lbusPersonAccountRetirementContribution);
                    }
                    //lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
                }
            }
            else
            {
                if (aintComputationYear < 1976)
                {
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount = Math.Round((ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEPayment) * adecRate, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount = Math.Round((ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEInterestAmt + ldecTotalPrevYearEEPayment + ldecTotalPrevYearEEInterestPayment) * adecRate, 2, MidpointRounding.AwayFromZero);
                }


                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year = aintComputationYear;
                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear).AddDays(1);
                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TRANSACTION_TYPE_INTEREST;
                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                if (IsVestedFlag)
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                else
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;

                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount = busConstant.ZERO_DECIMAL;
                if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount > 0)
                {
                    ldecCurrentYearEEInterest += lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount;
                    iclbPersonAccountRetirementContribution.Add(lbusPersonAccountRetirementContribution);
                    //lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
                }
                
                if (ldecCurrentYearEEPayment != 0.0m)
                {
                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContributionPayment = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                    lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.transaction_date = DateTime.Now;
                    lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.person_account_id = iintPersonAccountId;

                    if ((ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEPayment + ldecCurrentYearEEContribution + ldecCurrentYearEEPayment) < 0)
                    {
                        lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.ee_contribution_amount =
                        -(ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEPayment + ldecCurrentYearEEContribution + ldecCurrentYearEEPayment);
                    }
                    lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.computational_year = aintComputationYear;
                    lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.effective_date = busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear).AddDays(1);
                    lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.ee_int_amount = Math.Round(-(ldecTotalPrevYearEEInterestAmt + ldecTotalPrevYearEEInterestPayment + ldecCurrentYearEEInterestPayment +
                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount), 2, MidpointRounding.AwayFromZero);
                    lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.RCTransactionTypeAdjustment;
                    lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                    if (IsVestedFlag)
                        lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_VESTED;
                    else
                        lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;

                    lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.uvhp_int_amount = busConstant.ZERO_DECIMAL;
                    //Ask puneet
                    ldecCurrentYearEEInterest += lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.ee_int_amount;
                    if ((ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEPayment + ldecCurrentYearEEContribution + ldecCurrentYearEEPayment) < 0)
                    {
                        ldecCurrentYearEEContribution += lbusPersonAccountRetirementContributionPayment.icdoPersonAccountRetirementContribution.ee_contribution_amount;
                    }
                    iclbPersonAccountRetirementContribution.Add(lbusPersonAccountRetirementContributionPayment);
                    //lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Insert();

                }
            }
        }


        public ArrayList btn_PostEEInterest()
        {
            ArrayList iarrError = new ArrayList();
            utlError lobjError = new utlError();

            Collection<busPersonAccountRetirementContribution> lclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            DataTable ldtbList = Select<cdoPersonAccountRetirementContribution>(
              new string[2] { enmPersonAccountRetirementContribution.person_account_id.ToString(), enmPersonAccountRetirementContribution.contribution_type_value.ToString() },
              new object[2] { iintPersonAccountId, "EE" }, null, "computational_year");
            lclbPersonAccountRetirementContribution =
                GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");

            foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in lclbPersonAccountRetirementContribution)
            {
                if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TRANSACTION_TYPE_INTEREST
                    || lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypeAdjustment)
                {
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Delete();
                }
            }


            foreach (busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution in iclbPersonAccountRetirementContribution)
            {
                if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value == null
                    || lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value == string.Empty
                    || lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.RCTransactionTypePayment)
                {
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Update();
                }
                else
                {
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
                }
            }



            iarrError.Add(this);
            return iarrError;

        }

        #endregion

        #region Add Items IN Withdrawal Collection
        public Collection<cdoDummyWorkData> lclbtempCdoDummyWorkdata { get; set; }

        public void AddItemsINWithdrawalCollection(decimal adecQualifiedHours, decimal adecBenefitAmount, int aintYear, decimal adecTotalBenefitAmount,
                                                   int adecVestedYearsCount, decimal adecVestedHours, int aintHealthCount, decimal adecTotalHealthHours, decimal adecTotalIAPHours,
                                                   decimal adecEEContr, decimal adecEEInt, decimal adecUVHPContr, decimal adecUVHPIntr, string astrComments, int aBisYearCount, decimal antSequenceNumber, int aintAfterWDRLCount = 0)
        {
            cdoDummyWorkData ltempcdoDummyWorkData = new cdoDummyWorkData();
            ltempcdoDummyWorkData.qualified_hours = adecQualifiedHours;
            ltempcdoDummyWorkData.idecBenefitAmount = adecBenefitAmount;
            ltempcdoDummyWorkData.year = aintYear;
            ltempcdoDummyWorkData.idectotalBenefitAmount = adecTotalBenefitAmount + adecBenefitAmount;
            ltempcdoDummyWorkData.vested_years_count = adecVestedYearsCount;
            ltempcdoDummyWorkData.vested_hours = adecVestedHours;
            ltempcdoDummyWorkData.iintHealthCount = aintHealthCount;
            ltempcdoDummyWorkData.idecTotalHealthHours = adecTotalHealthHours;
            ltempcdoDummyWorkData.idecTotalIAPHours = adecTotalIAPHours;
            ltempcdoDummyWorkData.idecEEContribution = adecEEContr;
            ltempcdoDummyWorkData.idecEEInterest = adecEEInt;
            ltempcdoDummyWorkData.idecUVHPContribution = adecUVHPContr;
            ltempcdoDummyWorkData.idecUVHPInterest = adecUVHPIntr;
            ltempcdoDummyWorkData.comments = astrComments;
            ltempcdoDummyWorkData.bis_years_count = aBisYearCount;
            ltempcdoDummyWorkData.aintAfterWDRLCount = aintAfterWDRLCount;
            ltempcdoDummyWorkData.intSequenceNumber = antSequenceNumber;
            lclbtempCdoDummyWorkdata.Add(ltempcdoDummyWorkData);
        }
        #endregion

        public void LoadPlanDetails()
        {
            #region Dont Delete
            
            decimal idecMonthlyGrossAmount = Decimal.Zero;
            iclcdoPersonAccountOverview = new Collection<cdoPersonAccount>();
            DataTable ldtplan = busBase.Select("cdoPlan.GetPlanDetailsForOverview", new object[1] { this.icdoPerson.person_id });
            if (ldtplan.Rows.Count > 0)
            {
                iclcdoPersonAccountOverview = doBase.LoadData<cdoPersonAccount>(ldtplan);
            }
            iclcdoPersonAccountOverview.ForEach(item =>
            {


                if (item.istrPlan == busConstant.PENSION_PLAN && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                {
                    CheckAlreadyVestedOverview(item);

                    if (item.status_value == "RETR")
                    {
                        idecMonthlyGrossAmount = (Decimal)DBFunction.DBExecuteScalar("cdoPersonAccount.GetPayeeAccountwithGrossPayment", new object[2] { item.person_account_id, item.plan_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    }

                    item.istrTotalVestedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count;
                   
                    if (aclbPersonWorkHistory_Local != null && aclbPersonWorkHistory_Local.Count() > 0)
                    {

                        //tusharchandak Local row forfeture ke before hai so no need to minus
                        if (lclbLocalMergerCdoDummyWorkdata.year > item.dtForfeitureDate.Year)
                        {
                            item.istrTotalHours = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI where items.year > item.dtForfeitureDate.Year select items.qualified_hours).Sum()
                                                            - (from LocalItem in aclbPersonWorkHistory_Local
                                                               select LocalItem.qualified_hours).Sum(), 2);
                        }
                        else
                        {
                            item.istrTotalHours = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI where items.year > item.dtForfeitureDate.Year select items.qualified_hours).Sum()
                                                   , 2);
                        }
                        item.istrTotalQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count - (from LocalItem in aclbPersonWorkHistory_Local
                                                                                                                                        select LocalItem.qualified_years_count).Sum();
                        item.istrTotalHealthYearsPO = Convert.ToString(lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().iintHealthCount);
                        item.istrHealthHoursPO = Convert.ToString((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum());
                        if (idecMonthlyGrossAmount.IsNotNull() && idecMonthlyGrossAmount != Decimal.Zero)
                            item.idecTotalAccruedBenefit = Convert.ToString(Math.Round(idecMonthlyGrossAmount, 2));
                        else
                        {
                            //Rohan
                            if (item.dtForfeitureDate != DateTime.MinValue)
                                item.idecTotalAccruedBenefit = Convert.ToString(Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                            where items.year > item.dtForfeitureDate.Year                                                                                         
                                                                                            select items.idecBenefitAmount).Sum(), 2));
                            else
                                //TusharChandak
                                item.idecTotalAccruedBenefit = Convert.ToString(Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                            select items.idecBenefitAmount).Sum(), 2));
                            //TusharChandak
                            //item.idecTotalAccruedBenefit = Convert.ToString(Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                            //                                                            where items.year >= lclbLocalMergerCdoDummyWorkdata.year
                            //                                                            select items.idecBenefitAmount).Sum(), 2));
                        }
                    }
                    else
                    {
                        item.istrTotalHours = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI where items.year > item.dtForfeitureDate.Year select items.qualified_hours).Sum(), 2);
                        item.istrTotalQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                        item.istrHealthHoursPO = Convert.ToString((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum());
                        item.istrTotalHealthYearsPO = Convert.ToString(lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().iintHealthCount);

                        if (idecMonthlyGrossAmount.IsNotNull() && idecMonthlyGrossAmount != Decimal.Zero)
                            item.idecTotalAccruedBenefit = Convert.ToString(Math.Round(idecMonthlyGrossAmount, 2));
                        else
                        {
                            if (item.dtForfeitureDate != DateTime.MinValue)
                                item.idecTotalAccruedBenefit = Convert.ToString(Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                            where items.year > item.dtForfeitureDate.Year
                                                                                            select items.idecBenefitAmount).Sum(), 2));
                            else
                                item.idecTotalAccruedBenefit = Convert.ToString(Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                            select items.idecBenefitAmount).Sum(), 2));
                        }

                    }
                    item.istrAllocationEndYear = "N/A";
                    idecMonthlyGrossAmount = Decimal.Zero;
                }
                else if (item.istrPlan == busConstant.IAP_PLAN && lbusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                {
                    CheckAlreadyVestedOverview(item);
                    item.idecTotalAccruedBenefit = "N/A";
                    item.istrTotalVestedYears = lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().vested_years_count;

                    if (aclbPersonWorkHistory_Local != null && aclbPersonWorkHistory_Local.Count() > 0)
                        item.istrTotalQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count - (from LocalItem in aclbPersonWorkHistory_Local
                                                                                                                                        select LocalItem.qualified_years_count).Sum();
                    else
                    {
                        //if (item.dtVestedDate != DateTime.MinValue && lbusBenefitApplication.idtForfeitureDtMPIaftrFlg != DateTime.MinValue 
                        //        && lbusBenefitApplication.idtVestingDtMPIaftrFlg == DateTime.MinValue)
                        //{
                        //    item.istrTotalQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().qualified_years_count;
                        //}


                        item.istrTotalQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().qualified_years_count;
                        //else
                        //{ 
                        //    if(lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(i=>i.year == 2023 && i.qualified_hours >= 65 && i.qualified_hours < 400 ).Count() > 0)
                        //    {
                        //        item.istrTotalQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().qualified_years_count;
                        //    }
                        //    else
                        //    {
                        //        item.istrTotalQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;

                        //    }      

                        //}
                    }

                    
                    if (ldtlatestForfietureDate > item.dtForfeitureDate.Year)
                        item.dtForfeitureDate = new DateTime(ldtlatestForfietureDate, 12, 01);

                   
                    if (item.dtVestedDate != DateTime.MinValue && item.dtForfeitureDate > item.dtVestedDate)
                        item.dtForfeitureDate = DateTime.MinValue;

                    item.istrTotalHours = Math.Round((from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                      where items.year > item.dtForfeitureDate.Year                                                     
                                                      select items.idecTotalIAPHours).Sum(), 2);

                   
                    if (ldtEffectiveDate == DateTime.MinValue && item.dtVestedDate != DateTime.MinValue && lbusBenefitApplication.idtForfeitureDtMPIaftrFlg != DateTime.MinValue
                                && lbusBenefitApplication.idtVestingDtMPIaftrFlg == DateTime.MinValue)
                    {
                        item.istrTotalHours = (from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum();
                    }

                    if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0)
                        item.idecSpecialAccountBalance = Convert.ToString(Math.Round((from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                                      where items.icdoPersonAccountRetirementContribution.iap_balance_amount != Decimal.Zero
                                                                                      select items.icdoPersonAccountRetirementContribution.iap_balance_amount).Sum(), 2));
                    item.istrHealthHoursPO = "N/A";
                    item.istrTotalHealthYearsPO = "N/A";
                    item.istrAllocationEndYear = iintIAPAllocationYear;
                }
                else
                {
                    CheckAlreadyVestedOverview(item);

                    if (aclbPersonWorkHistory_Local != null && aclbPersonWorkHistory_Local.Count() > 0)
                    {

                        if (item.status_value == "RETR")
                        {
                            idecMonthlyGrossAmount = (Decimal)DBFunction.DBExecuteScalar("cdoPersonAccount.GetPayeeAccountwithGrossPayment", new object[2] { item.person_account_id, item.plan_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        }

                        item.istrTotalQualifiedYears = (from LocalItem in aclbPersonWorkHistory_Local
                                                        where LocalItem.istrPlanCode == item.istrPlanCode
                                                        select LocalItem.qualified_years_count).FirstOrDefault();

                        item.istrTotalHours = (from LocalItem in aclbPersonWorkHistory_Local
                                               where LocalItem.istrPlanCode == item.istrPlanCode
                                               select LocalItem.qualified_hours).FirstOrDefault();

                        if (idecMonthlyGrossAmount.IsNotNull() && idecMonthlyGrossAmount != Decimal.Zero)
                        {
                            item.idecTotalAccruedBenefit = Convert.ToString(Math.Round(idecMonthlyGrossAmount, 2));
                            idecMonthlyGrossAmount = Decimal.Zero;
                        }
                        else

                            item.idecTotalAccruedBenefit
                                = Convert.ToString(Math.Round((from LocalItem in aclbPersonWorkHistory_Local
                                                               where LocalItem.istrPlanCode == item.istrPlanCode
                                                               select LocalItem.idecBenefitAmount).FirstOrDefault(), 2));

                        if (item.istrPlan == "Local 52 Pension Plan")
                        {
                            if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0)
                                item.idecSpecialAccountBalance = Convert.ToString(Math.Round((from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                                              where items.icdoPersonAccountRetirementContribution.local52_special_acct_bal_amount != Decimal.Zero
                                                                                              select items.icdoPersonAccountRetirementContribution.local52_special_acct_bal_amount).Sum(), 2));
                            if ((lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0) &&
                                (from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                 where items.icdoPersonAccountRetirementContribution.local52_special_acct_bal_amount != Decimal.Zero
                                 select items.icdoPersonAccountRetirementContribution.computational_year).Count() > 0)
                                item.istrAllocationEndYear = Convert.ToString(((from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                                where items.icdoPersonAccountRetirementContribution.local52_special_acct_bal_amount != Decimal.Zero
                                                                                select items.icdoPersonAccountRetirementContribution.computational_year).Last()));
                        }
                        else if (item.istrPlan == "Local 161 Pension Plan")
                        {
                            if (lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0)
                                item.idecSpecialAccountBalance = Convert.ToString(Math.Round((from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                                              where items.icdoPersonAccountRetirementContribution.local161_special_acct_bal_amount != Decimal.Zero
                                                                                              select items.icdoPersonAccountRetirementContribution.local161_special_acct_bal_amount).Sum(), 2));
                            if ((lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.IsNotNull() && lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution.Count() > 0) &&
                                (from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                 where items.icdoPersonAccountRetirementContribution.local161_special_acct_bal_amount != Decimal.Zero
                                 select items.icdoPersonAccountRetirementContribution.computational_year).Count() > 0)
                                item.istrAllocationEndYear = Convert.ToString(((from items in lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution
                                                                                where items.icdoPersonAccountRetirementContribution.local161_special_acct_bal_amount != Decimal.Zero
                                                                                select items.icdoPersonAccountRetirementContribution.computational_year).Last()));
                        }

                        item.istrHealthHoursPO = "N/A";
                        item.istrTotalHealthYearsPO = "N/A";
                    }
                }
                if (item.idecSpecialAccountBalance == string.Empty || item.idecSpecialAccountBalance.IsNullOrEmpty())
                {
                    item.idecSpecialAccountBalance = "N/A";
                    item.istrAllocationEndYear = "N/A";
                }
                //PIR 753
                if (this.lbusPersonAccountEligibility != null)
                {
                    if (item.istrTotalHours == 0 && this.lbusPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours > 0.0M)
                    {
                        item.istrTotalHours = this.lbusPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                    }
                    if (item.istrTotalQualifiedYears == 0 && this.lbusPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years > 0.0M)
                    {
                        item.istrTotalQualifiedYears = this.lbusPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                    }
                    if (item.istrHealthHoursPO.IsNullOrEmpty())
                        item.istrHealthHoursPO = "N/A";
                    if (item.istrTotalHealthYearsPO.IsNullOrEmpty())
                        item.istrTotalHealthYearsPO = "N/A";
                }
            });
            #endregion
        }

        protected void CheckAlreadyVestedOverview(cdoPersonAccount objcdoPersonAccount)
        {
            this.lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
            int lintAccountId = iclcdoPersonAccountOverview.Where(plan => plan.istrPlanCode == objcdoPersonAccount.istrPlanCode).First().person_account_id;
            if (lintAccountId > 0)
            {
                DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });
                if (ldtbPersonAccountEligibility.Rows.Count > 0)
                {
                    this.lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                    if (this.lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.IsNotNull() && this.lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                    {
                        objcdoPersonAccount.istrVested = true;
                        objcdoPersonAccount.dtVestedDate = this.lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                    }
                    if (this.lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.IsNotNull() && this.lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date != DateTime.MinValue)
                    {
                        objcdoPersonAccount.dtForfeitureDate = this.lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                    }
                }

            }
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

        public bool CheckIfRetiree()
        {

            int lintRetireePayeeAccountCount = (int)DBFunction.DBExecuteScalar("cdoPerson.CheckIfRetiredFromMSS", new object[1] { icdoPerson.person_id },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (lintRetireePayeeAccountCount > 0)
                return true;

            return false;
        }
        public bool ShowFooter()
        {
            if (this.lbusBenefitApplication.IsNotNull() && this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNotNull() && this.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}

