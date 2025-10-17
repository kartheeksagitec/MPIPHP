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
using System.Collections.Generic;
using Sagitec.DataObjects;
#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busMssAnnualBenefitSummary : busAnnualBenefitSummaryOverview
    {
        public Collection<cdoDummyWorkData> aclbAnnualBenfitSummayOverviewTotal { get; set; }

        public string istrIsPersonVested { get; set; }

        /*
        public void LoadMssWorkHistory()
        {
            decimal ldecLateRetirementAmt = 0;
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

            lbusBenefitApplication.ibusPerson.LoadBenefitApplication();

            if ((!lbusBenefitApplication.ibusPerson.iclbBenefitApplication.IsNullOrEmpty() && lbusBenefitApplication.ibusPerson.iclbBenefitApplication.Count > 0) || (this.icdoPerson.date_of_death.IsNotNull() && this.icdoPerson.date_of_death != DateTime.MinValue))
            {
                if (lbusBenefitApplication.ibusPerson.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.benefit_type_value == "RTMT" || i.icdoBenefitApplication.benefit_type_value == "DSBL").Count() > 0)
                {
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitApplication.ibusPerson.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.benefit_type_value == "RTMT" || i.icdoBenefitApplication.benefit_type_value == "DSBL").OrderBy(i => i.icdoBenefitApplication.retirement_date).FirstOrDefault().icdoBenefitApplication.retirement_date;

                    if (this.icdoPerson.date_of_death.IsNotNull() && this.icdoPerson.date_of_death != DateTime.MinValue && this.icdoPerson.date_of_death < lbusBenefitApplication.icdoBenefitApplication.retirement_date)
                        lbusBenefitApplication.icdoBenefitApplication.retirement_date = this.icdoPerson.date_of_death;
                }
                else if (this.icdoPerson.date_of_death.IsNotNull() && this.icdoPerson.date_of_death != DateTime.MinValue)
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date = this.icdoPerson.date_of_death;
                else
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            }
            else
                lbusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);

            lbusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_PersonOverView();



        }

        */

        #region LoadingIapHours
        public void LoadMssWorkHistory()
        {
            this.LoadWorkHistory();
            foreach (cdoDummyWorkData lcdoDummyWork in lbusBenefitApplication.aclbPersonWorkHistory_IAP)
            {
                if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == lcdoDummyWork.year).Count() > 0)
                {
                    lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == lcdoDummyWork.year).FirstOrDefault().idecTotalIAPHours = lcdoDummyWork.qualified_hours;
                }
            }
        }

        #endregion

        public void LoadMssPlanDetails()
        {
            if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
            {

                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility{icdoPersonAccountEligibility = new cdoPersonAccountEligibility()};
                lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
                if (lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                {
                    this.istrIsPersonVested = busConstant.FLAG_YES;
                }
                busCalculation lbusCalculation = new busCalculation();
                this.aclbAnnualBenfitSummayOverviewTotal = new Collection<cdoDummyWorkData>();
                cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();

                DateTime ldtEffectiveDate = DateTime.MinValue;
                decimal ldecwihtodrawalhours = 0.0M;
                #region Check Withdrawal
                DataTable ldtbCheckPersonHasWithdrawal = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawal", new object[1] { lbusBenefitApplication.ibusPerson.icdoPerson.person_id });
                if (ldtbCheckPersonHasWithdrawal != null && ldtbCheckPersonHasWithdrawal.Rows.Count > 0)
                {
                    if (lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                        ldtEffectiveDate = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                            where item.Field<DateTime>("WITHDRAWAL_DATE") < lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date
                                            select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();
                    else
                        ldtEffectiveDate = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                            orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                                            select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();

                    if (ldtEffectiveDate != DateTime.MinValue)
                        ldecwihtodrawalhours = lbusCalculation.GetWorkDataAfterDate(lbusBenefitApplication.ibusPerson.icdoPerson.ssn, ldtEffectiveDate.Year, busConstant.MPIPP_PLAN_ID, ldtEffectiveDate);
                }
                #endregion

                if (lbusPersonAccountEligibility != null && lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date != DateTime.MinValue
                    && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year > lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year).Count() > 0
                    && lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date > ldtEffectiveDate)
                {
                    lcdoDummyWorkData.idecTotalPensionHours = (from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI where items.year > lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year select items.qualified_hours).Sum();
                }
                else
                {
                    lcdoDummyWorkData.idecTotalPensionHours = ldecwihtodrawalhours + (from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI where items.year > ldtEffectiveDate.Year select items.qualified_hours).Sum();
                }

                if (lbusPersonAccountEligibility != null && lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date != DateTime.MinValue
                   && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year > lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year).Count() > 0
                   && lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date > ldtEffectiveDate)
                {
                    lcdoDummyWorkData.idecTotalIAPHours = (from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI where items.year > lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year select items.idecTotalIAPHours).Sum();
                }
                else
                {
                    lcdoDummyWorkData.idecTotalIAPHours = ldecwihtodrawalhours + (from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI where items.year > ldtEffectiveDate.Year select items.idecTotalIAPHours).Sum();
                }


                lcdoDummyWorkData.idecTotalHealthHours = (from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI select items.idecTotalHealthHours).Sum();
                lcdoDummyWorkData.qualified_years_count = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                lcdoDummyWorkData.vested_years_count = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count;
                lcdoDummyWorkData.iintHealthCount = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().iintHealthCount;

                this.aclbAnnualBenfitSummayOverviewTotal.Add(lcdoDummyWorkData);


                Collection<cdoPersonAccountRetirementContribution> lclbRetCont = new Collection<cdoPersonAccountRetirementContribution>();

                busBenefitCalculationHeader lbusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                lbusBenefitCalculationHeader.ibusPerson = lbusBenefitApplication.ibusPerson;
                lbusBenefitCalculationHeader.LoadAllRetirementContributions(null);

                decimal ldecUnreducedBenefitAmount = LoadAccruedBenefitPerYear(lbusBenefitApplication, lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date);
                lcdoDummyWorkData.idecTotalAccruedBenefit = lbusCalculation.CalculateAccruedBenefitForPersonWithWithdrawal("", lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                            ldecUnreducedBenefitAmount, lbusBenefitApplication.ibusPerson, lbusBenefitApplication.ibusPerson.iclbPersonAccount,
                                                            lbusBenefitApplication.icdoBenefitApplication.retirement_date, lbusBenefitApplication.aclbPersonWorkHistory_MPI,
                                                            lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution, lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year, ref lclbRetCont);

                if (lcdoDummyWorkData.idecTotalAccruedBenefit > decimal.Zero)
                {
                    lcdoDummyWorkData.idecTotalAccruedBenefit = Math.Round(lcdoDummyWorkData.idecTotalAccruedBenefit, 2);
                }
                lcdoDummyWorkData.istrHealthEligibilty = busConstant.NO;
                if (this.icdoPerson.health_eligible_flag == busConstant.FLAG_YES)
                {
                    lcdoDummyWorkData.istrHealthEligibilty = busConstant.YES;
                }
                //else if(CheckForHealthEligibility())
                //{
                //    lcdoDummyWorkData.istrHealthEligibilty = busConstant.YES;
                //}
                //PER-0015
                //idecAccruedBenefit = lcdoDummyWorkData.idecTotalAccruedBenefit;
                //idecTotalQlfdYrs = lcdoDummyWorkData.qualified_years_count;
                //idecTotalQlfdHours = lcdoDummyWorkData.idecTotalPensionHours;
            }    



            //iclcdoPersonAccountOverview = new Collection<cdoPersonAccount>();
            //DataTable ldtplan = busBase.Select("cdoPlan.GetPlanDetailsForOverview", new object[1] { this.icdoPerson.person_id });
            //if (ldtplan.Rows.Count > 0)
            //{
            //    iclcdoPersonAccountOverview = doBase.LoadData<cdoPersonAccount>(ldtplan);
            //}

            //DateTime ldtWithdrawalDate = GetWithDrawaldateForPlanDetails();
            //iclcdoPersonAccountOverview.ForEach(item =>
            //{
            //    item.dtWithDrawlDate = ldtWithdrawalDate;
            //    if (item.istrPlan == busConstant.PENSION_PLAN && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
            //    {
            //        CheckAlreadyVestedOverview(item);

            //        item.istrTotalVestedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count;
            //        item.istrTotalQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;

            //        //PROD PIR 205
            //        ldtWihtdrawalEffectiveDate = DateTime.MinValue;
            //        decimal idecWithdrawalhours = CheckWithdrawalHours(busConstant.MPIPP_PLAN_ID);

            //        if (item.dtForfeitureDate != DateTime.MinValue && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(t => t.year > item.dtForfeitureDate.Year).Count() > 0
            //            && item.dtForfeitureDate > ldtWihtdrawalEffectiveDate)
            //        {
            //            item.istrTotalHours = (from itemss in lbusBenefitApplication.aclbPersonWorkHistory_MPI where itemss.year > item.dtForfeitureDate.Year select itemss.qualified_hours).Sum(); //MPI requested to see only Qualified HOurs
            //        }
            //        else
            //        {
            //            // PROD PIR 205
            //            //item.istrTotalHours = (from itemss in lbusBenefitApplication.aclbPersonWorkHistory_MPI select itemss.qualified_hours).Sum(); //MPI requested to see only Qualified HOurs
            //            item.istrTotalHours = idecWithdrawalhours + (from items in lbusBenefitApplication.aclbPersonWorkHistory_MPI where items.year > ldtWihtdrawalEffectiveDate.Year select items.qualified_hours).Sum();
            //        }

            //    }
            //    else if (item.istrPlan == busConstant.IAP_PLAN && lbusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
            //    {
            //        CheckAlreadyVestedOverview(item);
            //        item.istrTotalVestedYears = lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().vested_years_count;
            //        item.istrTotalQualifiedYears = lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().qualified_years_count;

            //        ldtWihtdrawalEffectiveDate = DateTime.MinValue;
            //        decimal idecWithdrawalhours = CheckWithdrawalHours(busConstant.IAP_PLAN_ID);

            //        if (item.dtForfeitureDate != DateTime.MinValue && lbusBenefitApplication.aclbPersonWorkHistory_IAP.Where(t => t.year > item.dtForfeitureDate.Year).Count() > 0
            //            && item.dtForfeitureDate > ldtWihtdrawalEffectiveDate)
            //        {
            //            item.istrTotalHours = (from itemss in lbusBenefitApplication.aclbPersonWorkHistory_IAP where itemss.year > item.dtForfeitureDate.Year select itemss.idecTotalIAPHours).Sum(); //MPI requested to see only Qualified HOurs
            //        }
            //        else
            //        {
            //            item.istrTotalHours = idecWithdrawalhours + (from items in lbusBenefitApplication.aclbPersonWorkHistory_IAP where items.year > ldtWihtdrawalEffectiveDate.Year select items.idecTotalIAPHours).Sum();
            //        }

            //    }
            //    else
            //    {
            //        CheckAlreadyVestedOverview(item);
            //    }
            //});
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

        public void CheckForHealthEligibilty()
        {
            
        }
    }
}
