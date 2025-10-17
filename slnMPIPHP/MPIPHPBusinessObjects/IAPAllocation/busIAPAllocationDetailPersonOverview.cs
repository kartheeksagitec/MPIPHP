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
using Sagitec.CustomDataObjects;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;
using Microsoft.Reporting.WinForms;
using System.IO;



#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busIAPAllocationDetailPersonOverview : busPerson
    {
        public Collection<busIapAllocationDetailCalculation> iclbIAPAllocationDetailPersonOverview { get; set; }
        //  IAP Enhancements #14 : Special Account Overpayment
        public Collection<busPersonAccountRetirementContribution> iclbPersonAccountRetirementYearlyAllocation { get; set; }
        public Collection<busBenefitApplicationDetail> iclbbusBenefitApplicationDetailID { get; set; }

        public Collection<busIapAllocationDetailCalculation> iclbL52SpecialAccountDetailPersonOverview { get; set; }
        public Collection<busIapAllocationDetailCalculation> iclbL161SpecialAccountDetailPersonOverview { get; set; }

        public Collection<busIapAllocationDetailCalculation> iclbTotalIAPAllocationDetailPersonOverview { get; set; }
        public Collection<busIapAllocationDetailCalculation> iclbPaidIAPAllocationDetailPersonOverview { get; set; }

        public Collection<busIapAllocationDetailCalculation> iclbNSRecordsIAPAllocationDetailPersonOverview { get; set; }

        //PIR 19
        public Collection<busIapAllocationDetailCalculation> iclbSpAccntAllocDetailPersonOverview { get; set; }

        //PIR 19
        public Collection<busIapAllocationDetailCalculation> iclbTotalSpAccntAllocDetailPersonOverview { get; set; }
        public Collection<busIapAllocationDetailCalculation> iclbPaidSpAccntAllocDetailPersonOverview { get; set; }
        public Collection<busIapAllocationDetailCalculation> iclbSpAccntAllocDetailComputed { get; set; }

        public Collection<busIapRecalculationCopy> iclbIAPRecalculationCopy { get; set; }

        public busIapRecalculationCopy ibusIapRecalculationCopy { get; set; }

        public busPersonOverview ibusPersonOverview { get; set; }
        public Collection<cdoIapAllocation5Recalculation> iclbIapAllocation5Recalculation { get; set; }
        public busIapAllocationSummary ibusAllocationSummary { get; set; }

        //PIR 1058
        public Collection<busIapAllocationDetailCalculation> iclbIAPAllocDetailPersonOverviewBeforeLateHour { get; set; }
        public Collection<busIapAllocationDetailCalculation> iclbPaidIAPAllocDetailPersonOverviewBeforeLateHour { get; set; }

        public busPersonAccount ibusPersonAccount { get; set; }

        public DataTable rptRecalculateIAPAlloc { get; set; }

        public DataTable rptRecalculateTotalIAPAlloc { get; set; }

        public bool iblnParticipant { get; set; }
        public bool iblnActive { get; set; }
        public bool iblnRetiree { get; set; }
        public bool iblnDead { get; set; }
        public bool iblnReemployed { get; set; }
        public bool iblnMDParticipant { get; set; }

        public bool isblnOverpaid { get; set; }
        public DateTime idtRetirementDate { get; set; }
        public ArrayList iarrAlloc5Years { get; set; }
        public DateTime idtMDdate { get; set; }

     //   public bool lblnLateHourBatch { get; set; }
        public bool iblnIAPLclSpAccWithdrwal { get; set; } //PIR 19
        public DateTime idtWithdrawalDate { get; set; } //PIR 19

        public string lstrSplAccntFlag { get; set; }
        public DateTime lWDRLeffectiveDate { get; set; }

        public bool iblnEligibleForQYRYAllocations { get; set; }//PIR 985

        public string IsRecalculateRule1Checked { get; set; }

        public int lintPersonAccountId { get; set; }
        public decimal idecThru79Hours { get; set; }

        public decimal idecIAPAccountBalance { get; set; }
        public DateTime idtIAPLatestAllocationDate { get; set; }
        public decimal idecL52SplAccountBalance { get; set; }
        public DateTime idtL52SplLatestAllocationDate { get; set; }
        public decimal idecL161SplAccountBalance { get; set; }
        public DateTime idtL161SplLatestAllocationDate { get; set; }
        //  IAP Enhancements #14 : Special Account Overpayment
        public decimal ldecIapBalanceAmount { get; set; }
        public decimal ldecLocal52BalanceAmount { get; set; }
        public decimal ldecLocal161BalanceAmount { get; set; }


        public DateTime idtEffectiveDate { get; set; }
        public DateTime idtEffectiveDateForReemployment { get; set; }
        //  IAP Enhancements #14 : Special Account Overpayment
        public DateTime leffectiveDate { get; set; }

        public decimal idecIAPAccountBalanceAsOfAwardedOnDate { get; set; }
        public Collection<busIapAllocationDetailCalculation> iclbIAPAllocationDetailPersonOverviewAsOfAwardedOnDate { get; set; }

        //PIR 1014
        public DateTime idtMinimumDistributionDate { get; set; }

        //PIR 628 
        public DateTime idtEACutOffDate { get; set; }
        public int iintYearofLateHour { get; set; } //PIR 1058

     //   public DateTime idtlateBatchRetirementDate { get; set; }

        public busPersonT79hours ibusPersonT79hours { get; set; }

        public bool iblnInLateBatch = false;

        ///Parameters adtEACutOffDate & aintComputationYear are strictly used for Late IAP Alloation Batch.
        ///These parameters has no significance in IAP Recalculation Functionality.Please do not use these parameters.
        public void LoadRecalculateAndPostIAPAllocationDetail(int aintPersonId, DateTime adtEACutOffDate, int aintComputationYear)
        {
            if (this.FindPerson(aintPersonId))
            {
                DataTable ldtblist = busPerson.Select("cdoPersonAccount.LoadPersonAccountbyPlanId", new object[2] { this.icdoPerson.person_id, busConstant.IAP_PLAN_ID });
                if (ldtblist.Rows.Count > 0)
                {
                    ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                    ibusPersonAccount.FindPersonAccount((Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()])));
                   // lblnLateHourBatch = lisLateHourBatch;
                    this.iclbIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
                    //LA Sunset - Release 3
                    this.LoadIAPNSRecords(Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()]));
                    this.LoadPaidIAPAllocationDetailsFromOpus(Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()]));
                    this.LoadIAPAllocation5Information(Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()]));
                    this.RecalculateIAPAllocationDetails(false, adtEACutOffDate, aintComputationYear);//PIR 628 extended
                    this.LoadIAPAllocationDetailsFromOpus(Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()]));
    
                    //if (lisLateHourBatch)
                    //
                    //    idtlateBatchRetirementDate = adtRetirementDate;
                    //}

                    //PIR 628 Extended
                    if (adtEACutOffDate != null && adtEACutOffDate != DateTime.MinValue)
                    {
                        //PIR 1058
                        if (iclbIAPAllocationDetailPersonOverview.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year < aintComputationYear).Count() > 0)
                        {
                            iclbIAPAllocDetailPersonOverviewBeforeLateHour = iclbIAPAllocationDetailPersonOverview.
                                Where(t => t.icdoIapallocationDetailPersonoverview.computational_year < aintComputationYear).ToList().ToCollection();
                        }
                        //PIR 1058
                        if (iclbPaidIAPAllocationDetailPersonOverview.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year < aintComputationYear).Count() > 0)
                        {
                            iclbPaidIAPAllocDetailPersonOverviewBeforeLateHour = iclbPaidIAPAllocationDetailPersonOverview.
                                Where(t => t.icdoIapallocationDetailPersonoverview.computational_year < aintComputationYear).ToList().ToCollection();
                        }

                        if (iclbIAPAllocationDetailPersonOverview.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year >= aintComputationYear).Count() > 0)
                        {
                            iclbIAPAllocationDetailPersonOverview = iclbIAPAllocationDetailPersonOverview.
                                Where(t => t.icdoIapallocationDetailPersonoverview.computational_year >= aintComputationYear).ToList().ToCollection();
                        }

                        if (iclbPaidIAPAllocationDetailPersonOverview.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year >= aintComputationYear).Count() > 0)
                        {
                            iclbPaidIAPAllocationDetailPersonOverview = iclbPaidIAPAllocationDetailPersonOverview.
                                Where(t => t.icdoIapallocationDetailPersonoverview.computational_year >= aintComputationYear).ToList().ToCollection();
                        }
                    }
                    this.GetDifferenceIAPAllocationDetailsReemployment();
                    this.GetDifferenceIAPAllocationDetails();
                 
                }
                if (adtEACutOffDate != DateTime.MinValue && ldtblist.Rows.Count > 0)
                {
                    idtEACutOffDate = adtEACutOffDate;
                    iintYearofLateHour = aintComputationYear; //PIR 1058
                    btn_PostAllocations();
                    idtEACutOffDate = DateTime.MinValue;
                    iintYearofLateHour = 0;  //PIR 1058
                }
            }
        }

        public void LoadIAPAllocationDetails(int aintPersonAccountId)
        {
            this.lintPersonAccountId = aintPersonAccountId; //Saving the IAP Person Account in a Variable to Avoid a DB hit while NAVIGATING TO THE NEXT SCREEN
            iclbIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();

            DataTable ldtbIAPAllocationDetail = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationToBeShownOnPersonO", new object[1] { aintPersonAccountId });
            if (ldtbIAPAllocationDetail.Rows.Count > 0)
            {
                iclbIAPAllocationDetailPersonOverview = GetCollection<busIapAllocationDetailCalculation>(ldtbIAPAllocationDetail, "icdoIapallocationDetailPersonoverview");
            }
            //  IAP Enhancements #14 : Special Account Overpayment
            this.GetBalanceforOverpaymentAdjustment(aintPersonAccountId);
            decimal ldecTotalIAPBalanceAmt = 0;
            decimal ldecRunningIAPAllocationBalance = 0;  //PIR RID 73750

            if (iclbPersonAccount == null)
                LoadPersonAccountsByPlanId(busConstant.IAP_PLAN_ID);
            busBenefitApplication lobjApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            busIAPAllocationHelper lobjIAPHelper = new busIAPAllocationHelper();
            lobjApplication.ibusPerson = this;
            lobjApplication.LoadandProcessWorkHistory_ForAllPlans();
            cdoDummyWorkData lcdoWorkData = new cdoDummyWorkData();





            //if (lobjApplication.aclbPersonWorkHistory_IAP != null && lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
            //    idecThru79Hours = lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Sum(o => o.qualified_hours);


            #region IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979

            //PIR 0971
            busPersonT79hours lbusPersonT79hours = new busPersonT79hours { icdoPersonT79hours = new cdoPersonT79hours() };
            if (lbusPersonT79hours.FindPersonT79hours(lintPersonAccountId) && lbusPersonT79hours.icdoPersonT79hours.approved_flag == busConstant.FLAG_YES)
            {
                idecThru79Hours = lbusPersonT79hours.icdoPersonT79hours.t79_hours;
            }
            else
            {

                //Remove history for any forfieture year 1979
                if (lobjApplication.aclbPersonWorkHistory_IAP != null && lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
                {
                    if (lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.istrForfietureFlag == busConstant.FLAG_YES).Count() > 0)
                    {
                        int lintMaxForfietureYearBefore1979 = lobjApplication.aclbPersonWorkHistory_IAP.Where(item => item.year <= busConstant.BenefitCalculation.YEAR_1979 && item.istrForfietureFlag == busConstant.FLAG_YES).Max(t => t.year);
                        lobjApplication.aclbPersonWorkHistory_IAP = lobjApplication.aclbPersonWorkHistory_IAP.Where(item => item.year > lintMaxForfietureYearBefore1979).ToList().ToCollection();
                    }
                }

                if (lobjApplication.aclbPersonWorkHistory_IAP != null && lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
                {
                    decimal ldecPreviousYearPaidIAPAccountBalance = 0M;
                    cdoDummyWorkData lcdoWorkData1979 = lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).FirstOrDefault();
                    //IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
                    if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
                    {
                        int lintPaymentYear = 0;
                        DataTable ldtblPaymentYear = busBase.Select("cdoPersonAccountRetirementContribution.GetMaxPaymentYearOnOrBefore1979", new object[1] { lobjApplication.icdoBenefitApplication.person_id });
                        if (ldtblPaymentYear != null && ldtblPaymentYear.Rows.Count > 0 && Convert.ToString(ldtblPaymentYear.Rows[0][0]).IsNotNullOrEmpty())
                        {
                            lintPaymentYear = Convert.ToInt32(ldtblPaymentYear.Rows[0][0]);
                        }
                        if (lintPaymentYear == 0)
                        {

                            idecThru79Hours = lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Sum(o => o.qualified_hours);

                        }
                        else
                        {
                            if (lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Count() > 0)
                            {
                                idecThru79Hours = lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Sum(o => o.qualified_hours);
                            }
                        }

                        idecThru79Hours += ldecPreviousYearPaidIAPAccountBalance;
                        if (idecThru79Hours < 0)
                            idecThru79Hours = 0;
                    }
                }
            }

            if (lobjApplication.aclbPersonWorkHistory_IAP != null && lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
            {
                lobjApplication.aclbPersonWorkHistory_IAP = lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).ToList().ToCollection();
            }

            #endregion


            foreach (busIapAllocationDetailCalculation item in iclbIAPAllocationDetailPersonOverview)
            {
                lcdoWorkData = new cdoDummyWorkData();
                if (lobjApplication.aclbPersonWorkHistory_IAP.IsNotNull())
                    lcdoWorkData = lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == Convert.ToInt32(item.icdoIapallocationDetailPersonoverview.computational_year)).FirstOrDefault();
                if (lcdoWorkData != null)
                {
                    item.icdoIapallocationDetailPersonoverview.ytdhours = lcdoWorkData.qualified_hours;
                    item.icdoIapallocationDetailPersonoverview.idecYTDHoursA2 = lcdoWorkData.IAP_HOURSA2;
                }


                //if (lobjIAPHelper.CheckParticipantIsAffiliate(Convert.ToInt32(item.computational_year), icdoPerson.istrSSNNonEncrypted))
                //    item.affiliate = busConstant.FLAG_YES;
                //else
                //    item.affiliate = busConstant.FLAG_NO;
                if (item.icdoIapallocationDetailPersonoverview.effective_date != DateTime.MinValue)
                {

                    //PROD PIR 74 //Ticket#77788
                    //Ticket#81049
                    if (item.icdoIapallocationDetailPersonoverview.computational_year == iclbIAPAllocationDetailPersonOverview.Where(i => i.icdoIapallocationDetailPersonoverview.alloc1 != 0).Select(Y => Y.icdoIapallocationDetailPersonoverview.computational_year).LastOrDefault())
                    {
                        //PROD PIR 74
                        if (item.icdoIapallocationDetailPersonoverview.effective_date == busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(item.icdoIapallocationDetailPersonoverview.computational_year)))
                        {
                            item.icdoIapallocationDetailPersonoverview.quarter = 4;
                        }
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 1 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 3)
                            item.icdoIapallocationDetailPersonoverview.quarter = 4;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 4 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 6)
                            item.icdoIapallocationDetailPersonoverview.quarter = 1;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 7 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 9)
                            item.icdoIapallocationDetailPersonoverview.quarter = 2;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 10 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 12)
                            item.icdoIapallocationDetailPersonoverview.quarter = 3;
                    }
                    else if (item.icdoIapallocationDetailPersonoverview.total_payment == 0)
                    {

                        if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 1 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 3)
                            item.icdoIapallocationDetailPersonoverview.quarter = 1;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 4 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 6)
                            item.icdoIapallocationDetailPersonoverview.quarter = 2;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 7 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 9)
                            item.icdoIapallocationDetailPersonoverview.quarter = 3;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 10 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 12)
                            item.icdoIapallocationDetailPersonoverview.quarter = 4;
                    }

                }
                item.icdoIapallocationDetailPersonoverview.total = item.icdoIapallocationDetailPersonoverview.alloc1 + item.icdoIapallocationDetailPersonoverview.alloc2 + item.icdoIapallocationDetailPersonoverview.alloc2_invt + item.icdoIapallocationDetailPersonoverview.alloc2_frft + item.icdoIapallocationDetailPersonoverview.alloc3 + item.icdoIapallocationDetailPersonoverview.alloc4 + item.icdoIapallocationDetailPersonoverview.alloc4_frft + item.icdoIapallocationDetailPersonoverview.alloc4_invt + item.icdoIapallocationDetailPersonoverview.alloc5;
                ldecTotalIAPBalanceAmt += item.icdoIapallocationDetailPersonoverview.total + item.icdoIapallocationDetailPersonoverview.total_payment;  //PIR 989
                item.icdoIapallocationDetailPersonoverview.iap_account_balance = ldecTotalIAPBalanceAmt;
                ldecRunningIAPAllocationBalance += item.icdoIapallocationDetailPersonoverview.total;  //PIR RID 73750
                item.icdoIapallocationDetailPersonoverview.idecRunningIAPAllocationBalance = ldecRunningIAPAllocationBalance; //PIR RID 73750
            }
            idecIAPAccountBalance = ldecTotalIAPBalanceAmt;
            if (iclbIAPAllocationDetailPersonOverview != null && iclbIAPAllocationDetailPersonOverview.Count > 0)
                idtIAPLatestAllocationDate = iclbIAPAllocationDetailPersonOverview.Last().icdoIapallocationDetailPersonoverview.effective_date;
            // For Local52 Special Account Grid
            iclbL52SpecialAccountDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
            DataTable ldtbLocal52Details = busBase.Select("cdoPersonAccountRetirementContribution.GetLocal52SpecialAccountDetailsForPersonOverview", new object[1] { aintPersonAccountId });
            if (ldtbLocal52Details.Rows.Count > 0)
            {
                iclbL52SpecialAccountDetailPersonOverview = GetCollection<busIapAllocationDetailCalculation>(ldtbLocal52Details, "icdoIapallocationDetailPersonoverview");
            }

            //PIR RID 81624 Initializing for Special Account Detail Screen
            ldecTotalIAPBalanceAmt = 0;
            ldecRunningIAPAllocationBalance = 0;

            foreach (busIapAllocationDetailCalculation item in iclbL52SpecialAccountDetailPersonOverview)
            {


                if (item.icdoIapallocationDetailPersonoverview.effective_date != DateTime.MinValue)
                {
                    //PROD PIR 74  //Ticket#77788

                    if (item.icdoIapallocationDetailPersonoverview.computational_year == iclbL52SpecialAccountDetailPersonOverview.Where(i => i.icdoIapallocationDetailPersonoverview.alloc1 != 0).Select(Y => Y.icdoIapallocationDetailPersonoverview.computational_year).LastOrDefault())
                    {
                        //PROD PIR 74
                        if (item.icdoIapallocationDetailPersonoverview.effective_date == busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(item.icdoIapallocationDetailPersonoverview.computational_year)))
                        {
                            item.icdoIapallocationDetailPersonoverview.quarter = 4;
                        }
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 1 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 3)
                            item.icdoIapallocationDetailPersonoverview.quarter = 4;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 4 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 6)
                            item.icdoIapallocationDetailPersonoverview.quarter = 1;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 7 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 9)
                            item.icdoIapallocationDetailPersonoverview.quarter = 2;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 10 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 12)
                            item.icdoIapallocationDetailPersonoverview.quarter = 3;
                    }
                    else if (item.icdoIapallocationDetailPersonoverview.total_payment == 0)
                    {
                        if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 1 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 3)
                            item.icdoIapallocationDetailPersonoverview.quarter = 1;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 4 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 6)
                            item.icdoIapallocationDetailPersonoverview.quarter = 2;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 7 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 9)
                            item.icdoIapallocationDetailPersonoverview.quarter = 3;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 10 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 12)
                            item.icdoIapallocationDetailPersonoverview.quarter = 4;
                    }

                }
                item.icdoIapallocationDetailPersonoverview.total = item.icdoIapallocationDetailPersonoverview.alloc1 + item.icdoIapallocationDetailPersonoverview.alloc4;
                ldecTotalIAPBalanceAmt += item.icdoIapallocationDetailPersonoverview.total + item.icdoIapallocationDetailPersonoverview.total_payment; //PIR 989
                item.icdoIapallocationDetailPersonoverview.iap_account_balance = ldecTotalIAPBalanceAmt;
                ldecRunningIAPAllocationBalance += item.icdoIapallocationDetailPersonoverview.total;  //PIR RID 73750
                item.icdoIapallocationDetailPersonoverview.idecRunningIAPAllocationBalance = ldecRunningIAPAllocationBalance; //PIR RID 73750
            }
            idecL52SplAccountBalance = ldecTotalIAPBalanceAmt;
            if (iclbL52SpecialAccountDetailPersonOverview != null && iclbL52SpecialAccountDetailPersonOverview.Count > 0)
                idtL52SplLatestAllocationDate = iclbL52SpecialAccountDetailPersonOverview.Last().icdoIapallocationDetailPersonoverview.effective_date;
            // For Local161 Special Account Grid
            iclbL161SpecialAccountDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
            DataTable ldtbLocal161Details = busBase.Select("cdoPersonAccountRetirementContribution.GetLocal161SpecialAccountDetailsForPersonOverview", new object[1] { aintPersonAccountId });
            if (ldtbLocal161Details.Rows.Count > 0)
            {
                iclbL161SpecialAccountDetailPersonOverview = GetCollection<busIapAllocationDetailCalculation>(ldtbLocal161Details, "icdoIapallocationDetailPersonoverview");
            }

            //PIR RID 81624 Initializing for Special Account Detail Screen
            ldecTotalIAPBalanceAmt = 0;
            ldecRunningIAPAllocationBalance = 0;


            foreach (busIapAllocationDetailCalculation item in iclbL161SpecialAccountDetailPersonOverview)
            {

                if (item.icdoIapallocationDetailPersonoverview.effective_date != DateTime.MinValue)
                {
                    //PROD PIR 74 //Ticket#77788
                    if (item.icdoIapallocationDetailPersonoverview.computational_year == iclbL161SpecialAccountDetailPersonOverview.Where(i => i.icdoIapallocationDetailPersonoverview.alloc1 != 0).Select(Y => Y.icdoIapallocationDetailPersonoverview.computational_year).LastOrDefault())
                    {
                        //PROD PIR 74
                        if (item.icdoIapallocationDetailPersonoverview.effective_date == busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(item.icdoIapallocationDetailPersonoverview.computational_year)))
                        {
                            item.icdoIapallocationDetailPersonoverview.quarter = 4;
                        }
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 1 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 3)
                            item.icdoIapallocationDetailPersonoverview.quarter = 4;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 4 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 6)
                            item.icdoIapallocationDetailPersonoverview.quarter = 1;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 7 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 9)
                            item.icdoIapallocationDetailPersonoverview.quarter = 2;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 10 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 12)
                            item.icdoIapallocationDetailPersonoverview.quarter = 3;
                    }
                    else if (item.icdoIapallocationDetailPersonoverview.total_payment == 0)
                    {
                        if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 1 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 3)
                            item.icdoIapallocationDetailPersonoverview.quarter = 1;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 4 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 6)
                            item.icdoIapallocationDetailPersonoverview.quarter = 2;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 7 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 9)
                            item.icdoIapallocationDetailPersonoverview.quarter = 3;
                        else if (item.icdoIapallocationDetailPersonoverview.effective_date.Month >= 10 && item.icdoIapallocationDetailPersonoverview.effective_date.Month <= 12)
                            item.icdoIapallocationDetailPersonoverview.quarter = 4;
                    }

                }
                item.icdoIapallocationDetailPersonoverview.total = item.icdoIapallocationDetailPersonoverview.alloc1 + item.icdoIapallocationDetailPersonoverview.alloc4;
                ldecTotalIAPBalanceAmt += item.icdoIapallocationDetailPersonoverview.total + item.icdoIapallocationDetailPersonoverview.total_payment;  //PIR 989
                item.icdoIapallocationDetailPersonoverview.iap_account_balance = ldecTotalIAPBalanceAmt;
                ldecRunningIAPAllocationBalance += item.icdoIapallocationDetailPersonoverview.total;  //PIR RID 73750
                item.icdoIapallocationDetailPersonoverview.idecRunningIAPAllocationBalance = ldecRunningIAPAllocationBalance; //PIR RID 73750
            }
            idecL161SplAccountBalance = ldecTotalIAPBalanceAmt;
            if (iclbL161SpecialAccountDetailPersonOverview != null && iclbL161SpecialAccountDetailPersonOverview.Count > 0)
                idtL161SplLatestAllocationDate = iclbL161SpecialAccountDetailPersonOverview.Last().icdoIapallocationDetailPersonoverview.effective_date;

            LoadIAPRecalculationCopy();
        }

        //PIR 628 extended
        public DataTable LoadIAPAllocationDetailsFromOpusUptoYear(int aintPersonAccountId, DateTime adtEffectiveDate, int aintComputationalYear)
        {
            DataTable ldtbIAPAllocationDetail = new DataTable();
            if (iblnRetiree && iblnReemployed)
            {
                DateTime ldtEffectiveDate = new DateTime();
                ldtEffectiveDate = idtEffectiveDate;
                if (!this.ibusPersonOverview.iclbPayeeAccount.IsNullOrEmpty())
                {
                    if (this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).Count() > 0)
                    {
                        DateTime ldtAwrded = this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitApplication.awarded_on_date;
                        DateTime ldtRet = this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitApplication.retirement_date;

                        if (ldtAwrded > ldtRet)
                            ldtEffectiveDate = ldtAwrded;
                        else
                            ldtEffectiveDate = ldtRet;

                    }
                }
                ldtbIAPAllocationDetail = busBase.Select("cdoIapAllocationDetail.GetIAPBalanceUptoYear", new object[5] { aintPersonAccountId, ldtEffectiveDate, aintComputationalYear, 1, 0 });

                if (iblnReemployed)
                {
                    ldtbIAPAllocationDetail = busBase.Select("cdoIapAllocationDetail.GetIAPBalanceUptoYear", new object[5] { aintPersonAccountId, ldtEffectiveDate, aintComputationalYear, 0, 1 });
                }
            }
            else
            {
                ldtbIAPAllocationDetail = busBase.Select("cdoIapAllocationDetail.GetIAPBalanceUptoYear", new object[5] { aintPersonAccountId, DateTime.Today, aintComputationalYear, 0, 0 });
            }

            return ldtbIAPAllocationDetail;
        }

        public void LoadIAPAllocationDetailsFromOpus(int aintPersonAccountId)
        {
            this.lintPersonAccountId = aintPersonAccountId;

            if (iclbIAPAllocationDetailPersonOverview == null)
                iclbIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
            //iclbIAPAllocationDetailFromOpus = new Collection<cdoIapallocationDetailPersonoverview>();

            //PIR 869
            //PIR 985
            if (iblnRetiree && iblnReemployed)
            {
                DateTime ldtEffectiveDate = new DateTime();
                ldtEffectiveDate = idtEffectiveDate;
                if (!this.ibusPersonOverview.iclbPayeeAccount.IsNullOrEmpty())
                {
                    if (this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).Count() > 0)
                    {
                        DateTime ldtAwrded = this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitApplication.awarded_on_date;
                        DateTime ldtRet = this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitApplication.retirement_date;

                        if (ldtAwrded > ldtRet)
                            ldtEffectiveDate = ldtAwrded;
                        else
                            ldtEffectiveDate = ldtRet;

                    }
                }
                DataTable ldtbIAPAllocationDetailRetirement = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationsDetailsForPersonOverviewBeforeRetirement", new object[2] { aintPersonAccountId, ldtEffectiveDate });
                if (ldtbIAPAllocationDetailRetirement.Rows.Count > 0)
                {
                    Collection<busIapAllocationDetailCalculation> lclbIAPAllocationDetailPersonOverviewRetirement = GetCollection<busIapAllocationDetailCalculation>(ldtbIAPAllocationDetailRetirement, "icdoIapallocationDetailPersonoverview");

                    foreach (busIapAllocationDetailCalculation lbusIapAllocationDetailCalculation in lclbIAPAllocationDetailPersonOverviewRetirement)
                    {
                        lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSource = "OPUS";
                        lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSSN = icdoPerson.ssn;
                        lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag = false;
                        iclbIAPAllocationDetailPersonOverview.Add(lbusIapAllocationDetailCalculation);
                    }

                }

                if (iblnReemployed)
                {
                    DataTable ldtbIAPAllocationDetailReemployment = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationsDetailsForPersonOverviewAfterRetirement", new object[2] { aintPersonAccountId, ldtEffectiveDate });
                    if (ldtbIAPAllocationDetailReemployment.Rows.Count > 0)
                    {
                        Collection<busIapAllocationDetailCalculation> lclbIAPAllocationDetailPersonOverviewReemployment = GetCollection<busIapAllocationDetailCalculation>(ldtbIAPAllocationDetailReemployment, "icdoIapallocationDetailPersonoverview");

                        foreach (busIapAllocationDetailCalculation lbusIapAllocationDetailCalculation in lclbIAPAllocationDetailPersonOverviewReemployment)
                        {
                            lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSource = "OPUS";
                            lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSSN = icdoPerson.ssn;
                            lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag = true;
                            iclbIAPAllocationDetailPersonOverview.Add(lbusIapAllocationDetailCalculation);

                        }
                    }

                }
            }
            else
            {

                DataTable ldtbIAPAllocationDetailActive = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationsDetailsForPersonOverview", new object[1] { aintPersonAccountId });
                if (ldtbIAPAllocationDetailActive.Rows.Count > 0)
                {
                    Collection<busIapAllocationDetailCalculation> lclbIAPAllocationDetailPersonOverviewActive = GetCollection<busIapAllocationDetailCalculation>(ldtbIAPAllocationDetailActive, "icdoIapallocationDetailPersonoverview");


                    foreach (busIapAllocationDetailCalculation lbusIapAllocationDetailCalculation in lclbIAPAllocationDetailPersonOverviewActive)
                    {
                        lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSource = "OPUS";
                        lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSSN = icdoPerson.ssn;
                        lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag = false;
                        iclbIAPAllocationDetailPersonOverview.Add(lbusIapAllocationDetailCalculation);
                    }
                }
            }
        }

        //PIR 19
        public void LoadSpAccntAllocDetailsFromOpus(int aintPersonAccountId, string astrFundType)
        {
            this.lintPersonAccountId = aintPersonAccountId;

            if (iclbSpAccntAllocDetailPersonOverview == null)
                iclbSpAccntAllocDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();


            if (astrFundType == busConstant.FundTypeLocal52SpecialAccount)
            {
                DataTable ldtbL52AllocActive = busBase.Select("cdoPersonAccountRetirementContribution.GetL52AllocDetailsForRecalculation", new object[1] { aintPersonAccountId });
                if (ldtbL52AllocActive.Rows.Count > 0)
                {
                    Collection<busIapAllocationDetailCalculation> lclbL52AllocationDetail = GetCollection<busIapAllocationDetailCalculation>(ldtbL52AllocActive, "icdoIapallocationDetailPersonoverview");


                    foreach (busIapAllocationDetailCalculation lbusL52AllocationDetailCalculation in lclbL52AllocationDetail)
                    {
                        lbusL52AllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSource = "OPUS";
                        lbusL52AllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSSN = icdoPerson.ssn;
                        lbusL52AllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrFundType = busConstant.FundTypeLocal52SpecialAccount;
                        iclbSpAccntAllocDetailPersonOverview.Add(lbusL52AllocationDetailCalculation);
                    }
                }
            }
            else if (astrFundType == busConstant.FundTypeLocal161SpecialAccount)
            {
                DataTable ldtbL161AllocActive = busBase.Select("cdoPersonAccountRetirementContribution.GetL161AllocDetailsForRecalculation", new object[1] { aintPersonAccountId });
                if (ldtbL161AllocActive.Rows.Count > 0)
                {
                    Collection<busIapAllocationDetailCalculation> lclbL161AllocationDetail = GetCollection<busIapAllocationDetailCalculation>(ldtbL161AllocActive, "icdoIapallocationDetailPersonoverview");


                    foreach (busIapAllocationDetailCalculation lbusL161AllocationDetailCalculation in lclbL161AllocationDetail)
                    {
                        lbusL161AllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSource = "OPUS";
                        lbusL161AllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSSN = icdoPerson.ssn;
                        lbusL161AllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrFundType = busConstant.FundTypeLocal161SpecialAccount;
                        iclbSpAccntAllocDetailPersonOverview.Add(lbusL161AllocationDetailCalculation);
                    }
                }
            }
        }


        public void LoadPaidIAPAllocationDetailsFromOpus(int aintPersonAccountId)
        {
            this.lintPersonAccountId = aintPersonAccountId;

            if (iclbPaidIAPAllocationDetailPersonOverview == null)
                iclbPaidIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();

            DataTable ldtbPaidIAPAllocationDetail = busBase.Select("cdoPersonAccountRetirementContribution.GetPaidIAPAllocationsDetailsForPersonOverview", new object[1] { aintPersonAccountId });
            if (ldtbPaidIAPAllocationDetail.Rows.Count > 0)
            {
                iclbPaidIAPAllocationDetailPersonOverview = GetCollection<busIapAllocationDetailCalculation>(ldtbPaidIAPAllocationDetail, "icdoIapallocationDetailPersonoverview");
            }
        }

        //LA Sunset - Release 3
        public void LoadIAPNSRecords(int aintPersonAccountId)
        {
            this.lintPersonAccountId = aintPersonAccountId;

            if (iclbNSRecordsIAPAllocationDetailPersonOverview == null)
                iclbNSRecordsIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();

            DataTable ldtbNSRecordsIAPAllocationDetail = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPNSRecords", new object[1] { aintPersonAccountId });
            if (ldtbNSRecordsIAPAllocationDetail.Rows.Count > 0)
            {
                iclbNSRecordsIAPAllocationDetailPersonOverview = GetCollection<busIapAllocationDetailCalculation>(ldtbNSRecordsIAPAllocationDetail, "icdoIapallocationDetailPersonoverview");
            }
        }

        //PIR 19
        public void LoadPaidSpAccntAllocDetailsFromOpus(int aintPersonAccountId, string astrFundType)
        {
            this.lintPersonAccountId = aintPersonAccountId;

            if (iclbPaidSpAccntAllocDetailPersonOverview == null)
                iclbPaidSpAccntAllocDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();

            if (astrFundType == busConstant.FundTypeLocal52SpecialAccount)
            {
                DataTable ldtbPaidL52AllocationDetail = busBase.Select("cdoPersonAccountRetirementContribution.GetPaidL52AllocDetailsForRecalculation", new object[1] { aintPersonAccountId });
                if (ldtbPaidL52AllocationDetail.Rows.Count > 0)
                {
                    iclbPaidSpAccntAllocDetailPersonOverview = GetCollection<busIapAllocationDetailCalculation>(ldtbPaidL52AllocationDetail, "icdoIapallocationDetailPersonoverview");
                    iclbPaidSpAccntAllocDetailPersonOverview.ForEach(t => t.icdoIapallocationDetailPersonoverview.istrFundType = busConstant.FundTypeLocal52SpecialAccount);
                }
            }
            else if (astrFundType == busConstant.FundTypeLocal161SpecialAccount)
            {
                DataTable ldtbPaidL161AllocationDetail = busBase.Select("cdoPersonAccountRetirementContribution.GetPaidL161AllocDetailsForRecalculation", new object[1] { aintPersonAccountId });
                if (ldtbPaidL161AllocationDetail.Rows.Count > 0)
                {
                    iclbPaidSpAccntAllocDetailPersonOverview = GetCollection<busIapAllocationDetailCalculation>(ldtbPaidL161AllocationDetail, "icdoIapallocationDetailPersonoverview");
                    iclbPaidSpAccntAllocDetailPersonOverview.ForEach(t => t.icdoIapallocationDetailPersonoverview.istrFundType = busConstant.FundTypeLocal161SpecialAccount);
                }
            }
        }


        public void LoadIAPAllocation5Information(int aintPersonAccountId)
        {
            this.lintPersonAccountId = aintPersonAccountId;

            if (iclbIapAllocation5Recalculation == null)
                iclbIapAllocation5Recalculation = new Collection<cdoIapAllocation5Recalculation>();


            DataTable ldtblIapAllocation5Recalculation = busBase.Select("cdoIapAllocation5Recalculation.GetAllocation5Information", new object[1] { aintPersonAccountId });
            if (ldtblIapAllocation5Recalculation.Rows.Count > 0)
            {
                iclbIapAllocation5Recalculation = cdoIapAllocation5Recalculation.GetCollection<cdoIapAllocation5Recalculation>(ldtblIapAllocation5Recalculation);
            }

        }

        ///Parameters adtEACutOffDate & aintComputationYear are strictly used for Late IAP Alloation Batch.
        ///These parameters has no significance in IAP Recalculation Functionality.Please do not use these parameters.
        public void RecalculateIAPAllocationDetails(bool ablnIsRefreshAllocations = false, DateTime? adtEACutOffDate = null, int aintComputationalYear = 0) //PIR 628
        {
            ibusPersonT79hours = new busPersonT79hours { icdoPersonT79hours = new cdoPersonT79hours() };

            DataTable ldtIAPContributions = new DataTable();
            DataTable ldtIAPFiltered = new DataTable();

            decimal ldecThru79Hours;
            ldecThru79Hours = 0.0M;
            int lintPersonAccountId = 0;
            busBenefitApplication lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };

            #region LoadLatestAllocationSummaryYear
            ibusAllocationSummary = new busIapAllocationSummary { icdoIapAllocationSummary = new cdoIapAllocationSummary() };
            ibusAllocationSummary.LoadLatestAllocationSummary();
            #endregion

            #region Load IAP Work History (usp_PensionInterface4OPUS)

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            DataTable ldtIAPHoursDetails = new DataTable();
            DataTable ldtIAPHoursDetailsForReeemployment = new DataTable();

            SqlParameter[] parameters = new SqlParameter[1];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);

            param1.Value = this.icdoPerson.istrSSNNonEncrypted;
            parameters[0] = param1;

            ldtIAPHoursDetails = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_PensionInterface4OPUS", astrLegacyDBConnetion, null, parameters);
            if (ldtIAPHoursDetails != null && ldtIAPHoursDetails.Rows.Count > 0)
            {
                if (ldtIAPHoursDetails.AsEnumerable().Where(item => item.Field<Int16>("PensionPlan") == 2).Count() > 0)
                {
                    ldtIAPHoursDetails = ldtIAPHoursDetails.FilterTable(utlDataType.Numeric, "PensionPlan", 2).CopyToDataTable();
                    ldtIAPHoursDetailsForReeemployment = ldtIAPHoursDetails;
                }
            }
            DataTable ldtIapHoursDetailsDuplicate = ldtIAPHoursDetails.AsEnumerable().CopyToDataTable();
            #endregion

            DeterminePersonTypeAndLoadWorkHistory(ref lobjBenefitApplication, ref ldtIAPHoursDetails, adtEACutOffDate); //PIR 628

          //  istrIspersonIAPVested = lobjBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinIAP;
            lintPersonAccountId = lobjBenefitApplication.ibusPerson.iclbPersonAccount.FirstOrDefault().icdoPersonAccount.person_account_id;

            ibusPersonT79hours.FindPersonT79hours(lintPersonAccountId);

            DataTable ldtIAPAllContributions = new DataTable();
            if (lobjBenefitApplication.aclbPersonWorkHistory_IAP != null && lobjBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
            {
                if (!lobjBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
                {
                    this.idecAge = busGlobalFunctions.CalculatePersonAge(lobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, DateTime.Now);
                    lobjBenefitApplication.DetermineVesting();
                }
               
                //Method to load IAP allocations from sgt_person_account_contribution table
                
                    ldtIAPAllContributions = LoadIAPContributions(lintPersonAccountId, lobjBenefitApplication.aclbPersonWorkHistory_IAP.FirstOrDefault().year);


                bool lblnCheckForDisability = false;

                #region  Code for Disability

                if (iblnRetiree)
                {
                    if (!this.ibusPersonOverview.iclbPayeeAccount.IsNullOrEmpty())
                    {
                        if (this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).Count() > 0)
                        {
                            int lintApplicationID = this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitApplication.benefit_application_id;
                            if (this.ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintBenefitApplicationID == lintApplicationID).Count() > 0)
                            {
                                DateTime ldtAwrded = this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitApplication.awarded_on_date;
                                DateTime ldtRet = this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitApplication.retirement_date;
                                if (ldtAwrded != ldtRet)
                                {
                                    #region Awarded On Date
                                    lblnCheckForDisability = true;
                                    idtRetirementDate = ldtAwrded;

                                    //PIR 869
                                    lobjBenefitApplication.icdoBenefitApplication.retirement_date = ldtAwrded;
                                    //PIR 628 New
                                    if (adtEACutOffDate != DateTime.MinValue && adtEACutOffDate != null)
                                        lobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(false, null, adtEACutOffDate);
                                    else
                                        lobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                                    //lobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();

                                    //PIR 628 Extended
                                    DataTable ldtblIAPBalanceUptoYear = null;
                                    if (adtEACutOffDate != null && adtEACutOffDate != DateTime.MinValue)
                                        ldtblIAPBalanceUptoYear = LoadIAPAllocationDetailsFromOpusUptoYear(lintPersonAccountId, ldtAwrded, aintComputationalYear);

                                    if (ldtIapHoursDetailsDuplicate != null && ldtIapHoursDetailsDuplicate.Rows.Count > 0 &&
                                ldtIapHoursDetailsDuplicate.AsEnumerable().Where(item => item.Field<DateTime>("ToDate") <= busGlobalFunctions.GetLastDayOfWeek(idtRetirementDate)).Count() > 0)
                                    {
                                        ldtIAPHoursDetails = ldtIapHoursDetailsDuplicate.AsEnumerable().Where(item => item.Field<DateTime>("ToDate") <= busGlobalFunctions.GetLastDayOfWeek(idtRetirementDate)).CopyToDataTable();
                                    }

                                    //PIR 628 Extended
                                    RecalculateAndFillAllocationDetails(lobjBenefitApplication.aclbPersonWorkHistory_IAP, ldtIAPHoursDetails, lobjBenefitApplication, lintPersonAccountId, ldtIAPAllContributions, ablnIsRefreshAllocations, ldtblIAPBalanceUptoYear);//PIR 628 Extended
                                    idecIAPAccountBalanceAsOfAwardedOnDate = (from obj in iclbIAPAllocationDetailPersonOverview where obj.icdoIapallocationDetailPersonoverview.istrSource == "Computed" select obj.icdoIapallocationDetailPersonoverview.idecTotal).Sum();
                                    iclbIAPAllocationDetailPersonOverviewAsOfAwardedOnDate = new Collection<busIapAllocationDetailCalculation>();
                                    foreach (busIapAllocationDetailCalculation lobj in iclbIAPAllocationDetailPersonOverview)
                                    {
                                        iclbIAPAllocationDetailPersonOverviewAsOfAwardedOnDate.Add(lobj);
                                    }
                                    #endregion

                                    #region Retirement
                                    iclbIAPAllocationDetailPersonOverview = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource != "Computed").ToList().ToCollection();
                                    idtRetirementDate = ldtRet;

                                    //PIR 869
                                    lobjBenefitApplication.icdoBenefitApplication.retirement_date = ldtRet;

                                    //PIR 628 New
                                    if (adtEACutOffDate != DateTime.MinValue && adtEACutOffDate != null)
                                        lobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(false, null, adtEACutOffDate);
                                    else
                                        lobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                                    //lobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();

                                    //PIR 628 Extended
                                    ldtblIAPBalanceUptoYear = null;
                                    if (adtEACutOffDate != null && adtEACutOffDate != DateTime.MinValue)
                                        ldtblIAPBalanceUptoYear = LoadIAPAllocationDetailsFromOpusUptoYear(lintPersonAccountId, ldtRet, aintComputationalYear);

                                    if (ldtIapHoursDetailsDuplicate != null && ldtIapHoursDetailsDuplicate.Rows.Count > 0 &&
                              ldtIapHoursDetailsDuplicate.AsEnumerable().Where(item => item.Field<DateTime>("ToDate") <= busGlobalFunctions.GetLastDayOfWeek(idtRetirementDate)).Count() > 0)
                                    {
                                        ldtIAPHoursDetails = ldtIapHoursDetailsDuplicate.AsEnumerable().Where(item => item.Field<DateTime>("ToDate") <= busGlobalFunctions.GetLastDayOfWeek(idtRetirementDate)).CopyToDataTable();
                                    }
                                    RecalculateAndFillAllocationDetails(lobjBenefitApplication.aclbPersonWorkHistory_IAP, ldtIAPHoursDetails, lobjBenefitApplication, lintPersonAccountId, ldtIAPAllContributions, ablnIsRefreshAllocations, ldtblIAPBalanceUptoYear);//PIR 628 Extended
                                    #endregion

                                    #region Comparison
                                    if ((from obj in iclbIAPAllocationDetailPersonOverview where obj.icdoIapallocationDetailPersonoverview.istrSource == "Computed" select obj.icdoIapallocationDetailPersonoverview.idecTotal).Sum() < idecIAPAccountBalanceAsOfAwardedOnDate)
                                    {
                                        iclbIAPAllocationDetailPersonOverview = iclbIAPAllocationDetailPersonOverviewAsOfAwardedOnDate;
                                        idtEffectiveDate = ldtAwrded;//Rohan 489
                                    }

                                    if ((from obj in iclbIAPAllocationDetailPersonOverview where obj.icdoIapallocationDetailPersonoverview.istrSource == "Computed" select obj.icdoIapallocationDetailPersonoverview.idecTotal).Sum() == idecIAPAccountBalanceAsOfAwardedOnDate)
                                    {
                                        idtEffectiveDate = ldtAwrded > ldtRet ? ldtAwrded : ldtRet;
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
                #endregion

                if (!lblnCheckForDisability)
                {

                    //PIR 628 Extended
                    DataTable ldtblIAPBalanceUptoYear = null;
                    if (adtEACutOffDate != null && adtEACutOffDate != DateTime.MinValue)
                        ldtblIAPBalanceUptoYear = LoadIAPAllocationDetailsFromOpusUptoYear(lintPersonAccountId, idtEffectiveDate, aintComputationalYear);
                    RecalculateAndFillAllocationDetails(lobjBenefitApplication.aclbPersonWorkHistory_IAP, ldtIAPHoursDetails, lobjBenefitApplication, lintPersonAccountId, ldtIAPAllContributions, ablnIsRefreshAllocations, ldtblIAPBalanceUptoYear);
                }
            }

            if (iblnRetiree)
            {
                DetermineIfpersonIsReemployedAndLoadWorkHistory(ref lobjBenefitApplication, ref ldtIAPHoursDetailsForReeemployment);

                if (iblnReemployed)
                {
                    if (lobjBenefitApplication.aclbPersonWorkHistory_IAP != null && lobjBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                    {
                        if (!lobjBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
                        {
                            this.idecAge = busGlobalFunctions.CalculatePersonAge(lobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, DateTime.Now);
                            //Check with Rohan : No need to calculate vesting as the participant is already retired and is vested
                            //lobjBenefitApplication.DetermineVesting();
                        }

                        //PIR 628 Extended
                        DataTable ldtblIAPBalanceUptoYear = null;
                        if (adtEACutOffDate != null && adtEACutOffDate != DateTime.MinValue)
                            ldtblIAPBalanceUptoYear = LoadIAPAllocationDetailsFromOpusUptoYear(lintPersonAccountId, idtEffectiveDate, aintComputationalYear);

                        RecalculateAndFillAllocationDetails(lobjBenefitApplication.aclbPersonWorkHistory_IAP, ldtIAPHoursDetailsForReeemployment, lobjBenefitApplication, lintPersonAccountId, ldtIAPAllContributions, ablnIsRefreshAllocations, ldtblIAPBalanceUptoYear);

                    }
                }
            }
        }

        public void DeterminePersonTypeAndLoadWorkHistory(ref busBenefitApplication aobjBenefitApplication, ref DataTable adtIAPHoursDetails, DateTime? adtEACutOffDate = null) //PIR 628
        {

            iblnParticipant = false;
            iblnActive = false;
            iblnRetiree = false;
            iblnDead = false;
            iblnReemployed = false;
            iblnMDParticipant = false;
            idtRetirementDate = DateTime.MinValue;
            idtMDdate = DateTime.MinValue;

            iblnEligibleForQYRYAllocations = false;//PIR 985

            bool lblnIAPOnlyWithdrawal = false;//PIR 869

            #region Load Person Overview Details
            ibusPersonOverview = new busPersonOverview();
            if (ibusPersonOverview.FindPerson(this.icdoPerson.person_id))
            {
                ibusPersonOverview.LoadInitialData();
                ibusPersonOverview.GetCurrentAge();
                ibusPersonOverview.LoadPersonDROApplications();
                ibusPersonOverview.LoadDeathNotifications();
                ibusPersonOverview.LoadBenefitApplication();
                ibusPersonOverview.LoadPayeeAccount();
            }
            #endregion Load Person Overview Details

            aobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            aobjBenefitApplication.icdoBenefitApplication.person_id = Convert.ToInt32(this.icdoPerson.person_id);
            aobjBenefitApplication.LoadPerson();
            aobjBenefitApplication.ibusPerson.LoadPersonAccountsByPlanId(busConstant.IAP_PLAN_ID);

            int lintLastYear = 0;
            DataTable ldtbIAPAllocation = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationsDetailsForPersonOverview", new object[1] { aobjBenefitApplication.ibusPerson.iclbPersonAccount.FirstOrDefault().icdoPersonAccount.person_account_id });
            if (ldtbIAPAllocation.Rows.Count > 0)
            {
                lintLastYear = Convert.ToInt32(ldtbIAPAllocation.Rows[ldtbIAPAllocation.Rows.Count - 1]["COMPUTATIONAL_YEAR"]);
            }
            else  //PIR 628
            {
                lintLastYear = ibusAllocationSummary.icdoIapAllocationSummary.computation_year;
            }
            //PIR 965 //RequestID:99406 - HV
            if (ibusPersonOverview.iclbPayeeAccount != null && ibusPersonOverview.iclbPayeeAccount.Count > 0
                && (ibusPersonOverview.iclbPayeeAccount.Where(t => t.icdoPayeeAccount.istrChildSupportFlag == busConstant.FLAG_YES).Count() > 0
                    ||
                    ibusPersonOverview.iclbPayeeAccount.Where(t => t.icdoPayeeAccount.istrCovidFlag == busConstant.FLAG_YES).Count() > 0
                    ||
                    ibusPersonOverview.iclbPayeeAccount.Where(t => t.icdoPayeeAccount.istrHardshipWithdrawal == busConstant.FLAG_YES).Count() > 0
                   )
               )
            {
                
                ibusPersonOverview.iclbPayeeAccount = ibusPersonOverview.iclbPayeeAccount.Where(t => t.icdoPayeeAccount.istrChildSupportFlag != busConstant.FLAG_YES).ToList().ToCollection();
                ibusPersonOverview.iclbPayeeAccount = ibusPersonOverview.iclbPayeeAccount.Where(t => t.icdoPayeeAccount.istrCovidFlag != busConstant.FLAG_YES).ToList().ToCollection();
                ibusPersonOverview.iclbPayeeAccount = ibusPersonOverview.iclbPayeeAccount.Where(t => t.icdoPayeeAccount.istrHardshipWithdrawal != busConstant.FLAG_YES).ToList().ToCollection();
            }

            //PIR 869
            if (ibusPersonOverview.iblnParticipant == busConstant.YES
              && ibusPersonOverview.istrRetiree == busConstant.NO && ibusPersonOverview.iclbPayeeAccount.Where(item =>
                  item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                  item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() && //PIR 969
                 (item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)).Count() > 0 && icdoPerson.date_of_death == DateTime.MinValue)
            {
                lblnIAPOnlyWithdrawal = true;
            }
            //PIR 869
            if (ibusPersonOverview.iblnParticipant == busConstant.YES
                && ibusPersonOverview.istrRetiree == busConstant.NO && icdoPerson.date_of_death == DateTime.MinValue && !lblnIAPOnlyWithdrawal)
            {
                iblnParticipant = true;
                iblnActive = true;
                //MM Issue reported in RID 71816 and 72814 NonRetired participant has >400 hours but not getting allocation.
                //For non-retired participant we should not set the retirement date. It is not picking all the IAP workhistory.
                //DateTime ldtRetirementDate = busGlobalFunctions.GetLastDateOfComputationYear(lintLastYear);
                //aobjBenefitApplication.icdoBenefitApplication.retirement_date = ldtRetirementDate;
                //PIR 628
                if (adtEACutOffDate != DateTime.MinValue && adtEACutOffDate != null)
                    aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(false, null, adtEACutOffDate);
                else
                    aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
            }
            else if (ibusPersonOverview.iblnParticipant == busConstant.YES
                   && (ibusPersonOverview.istrRetiree == busConstant.NO ||
                   (ibusPersonOverview.iclbPayeeAccount != null &&
                     ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT &&
                      item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED &&   //PIR 1016
                    item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID && item.icdoPayeeAccount.istrFundType.IsNullOrEmpty()  //PIR 969
                    ).Count() > 0)) //PIR 1016 for MD participant this flag may turn on because of MPI Plan, but they were not retire in IAP.
                    && icdoPerson.date_of_death != DateTime.MinValue)
            {

                iblnParticipant = true;
                iblnDead = true;

                DateTime ldtDate = new DateTime();
                ldtDate = icdoPerson.date_of_death;

                if (ibusPersonOverview.iclbPayeeAccount != null &&
                     ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT &&
                      item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED &&   //PIR 1016
                    item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID && item.icdoPayeeAccount.istrFundType.IsNullOrEmpty()  //PIR 969
                    ).Count() > 0)
                {
                    //PIR 985
                    iblnEligibleForQYRYAllocations = CheckIfEligibleToGetQuarterly(ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT &&
                    item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                     item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED    //PIR 1016
                    && item.icdoPayeeAccount.istrFundType.IsNullOrEmpty()).FirstOrDefault().icdoPayeeAccount.benefit_application_detail_id);

                    //PIR 869
                    DataTable ldtblFirstPaymentDate = Select("cdoIAPAllocationDetailPersonOverview.GetFirstPaymentDate", new object[1] { this.icdoPerson.person_id });
                    if (ldtblFirstPaymentDate != null && ldtblFirstPaymentDate.Rows.Count > 0)
                    {
                        if (Convert.ToString(ldtblFirstPaymentDate.Rows[0][enmPaymentHistoryHeader.payment_date.ToString().ToUpper()]).IsNotNullOrEmpty())
                            ldtDate = Convert.ToDateTime(ldtblFirstPaymentDate.Rows[0][enmPaymentHistoryHeader.payment_date.ToString().ToUpper()]);
                    }
                    //PIR 1016 if eligible for qurterly and there is no first payment then use benefit commencement date.
                    else if (iblnEligibleForQYRYAllocations)
                    {
                        ldtDate = ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT &&
                                             item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED &&
                                             item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID && item.icdoPayeeAccount.istrFundType.IsNullOrEmpty()).FirstOrDefault().icdoPayeeAccount.idtRetireMentDate;

                    }
                    else
                    {
                        //PIR 836
                        ldtDate = busGlobalFunctions.GetLastDateOfComputationYear(lintLastYear);
                        iblnParticipant = true;
                        iblnActive = true;
                        iblnDead = false;
                    }
                }
                else
                {
                    //PIR 836
                    ldtDate = busGlobalFunctions.GetLastDateOfComputationYear(lintLastYear);
                    iblnParticipant = true;
                    iblnActive = true;
                    iblnDead = false;
                }

                idtRetirementDate = ldtDate;
                aobjBenefitApplication.icdoBenefitApplication.retirement_date = ldtDate;
                //PIR 628 New
                if (adtEACutOffDate != DateTime.MinValue && adtEACutOffDate != null)
                    aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(false, null, adtEACutOffDate);
                else
                    aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();

                idtEffectiveDate = idtRetirementDate;

                if (adtIAPHoursDetails != null && adtIAPHoursDetails.Rows.Count > 0 &&
                  adtIAPHoursDetails.AsEnumerable().Where(item => item.Field<DateTime>("ToDate") <= ldtDate).Count() > 0)
                {
                    adtIAPHoursDetails = adtIAPHoursDetails.AsEnumerable().Where(item => item.Field<DateTime>("ToDate") <= ldtDate).CopyToDataTable();
                }

            }
            else if (ibusPersonOverview.iblnParticipant == busConstant.YES
                && ibusPersonOverview.istrRetiree == busConstant.YES)
            {
                //Prod Pir 283 : M01542951 : Specific check for plan MD date different for IAP & MPI plan.
                //Mpid : m01542951 : md date diff for mpi and iap plan
                //cannot check for completed status as iap will always be cpompleted
                if (ibusPersonOverview.iclbPayeeAccount != null
                    &&
                    ibusPersonOverview.iclbPayeeAccount.Where(item =>
                    item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                     item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() && //PIR 969
                    item.icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION).Count() > 0
                    &&
                    //PIR 1014
                    ibusPersonOverview.iclbPayeeAccount.Where(item =>
                    item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                    item.icdoPayeeAccount.account_relation_value == busConstant.PERSON_TYPE_PARTICIPANT &&    //PIR RID 63412
                    item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() && (item.icdoPayeeAccount.retirement_type_value.IsNullOrEmpty() ||
                    item.icdoPayeeAccount.retirement_type_value != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)).Count() <= 0
                    )
                {
                    iblnParticipant = true;
                    iblnMDParticipant = true;

                    //PIR 985
                    iblnEligibleForQYRYAllocations = CheckIfEligibleToGetQuarterly(ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED &&
                    item.icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                     item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() //PIR 969
                    ).
                    FirstOrDefault().icdoPayeeAccount.benefit_application_detail_id);

                    idtMDdate = ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED &&
                    item.icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                     item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() //PIR 969
                    ).
                    FirstOrDefault().icdoPayeeAccount.idtRetireMentDate;

                    DateTime ldtRetirementDate = busGlobalFunctions.GetLastDateOfComputationYear(lintLastYear);

                    iblnRetiree = false;
                    iblnActive = true;

                    idtRetirementDate = ldtRetirementDate;
                    idtEffectiveDate = idtRetirementDate;
                    if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= idtMDdate.Year) //MD logic
                    {
                        ldtRetirementDate = busGlobalFunctions.GetLastDateOfComputationYear(ibusAllocationSummary.icdoIapAllocationSummary.computation_year);
                        aobjBenefitApplication.icdoBenefitApplication.retirement_date = ldtRetirementDate;
                        idtRetirementDate = ldtRetirementDate;
                        idtEffectiveDate = idtRetirementDate;
                    }
                    else
                    {
                        aobjBenefitApplication.icdoBenefitApplication.retirement_date = idtMDdate;//PIR 283 it should be set as md date
                    }

                    //PIR 628 New
                    if (adtEACutOffDate != DateTime.MinValue && adtEACutOffDate != null)
                        aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(false, null, adtEACutOffDate);
                    else
                        aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                }
                else if (ibusPersonOverview.iclbPayeeAccount != null && ibusPersonOverview.iclbPayeeAccount.Where(item =>
                    item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                    item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() && //PIR 969
                   (item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT
                  || item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)).Count() > 0)
                {
                    //Prod Pir 283 : M01542951 : Specific check for pl date different for IAP & MPI plan.
                    iblnParticipant = true;
                    iblnRetiree = true;
                    idtMinimumDistributionDate = DateTime.MinValue;

                    //PIR 1014
                    if (ibusPersonOverview.iclbBenefitApplication != null &&
                        ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT
                        && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && item.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES
                        && item.icdoBenefitApplication.min_distribution_date != DateTime.MinValue
                        && item.icdoBenefitApplication.application_status_value == busConstant.BENEFIT_APPLICATION_STATUS_APPROVED).Count() > 0)
                    {
                        idtMinimumDistributionDate = ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT
                        && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && item.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES
                        && item.icdoBenefitApplication.min_distribution_date != DateTime.MinValue
                        && item.icdoBenefitApplication.application_status_value == busConstant.BENEFIT_APPLICATION_STATUS_APPROVED).FirstOrDefault().icdoBenefitApplication.min_distribution_date;
                    }

                    DateTime ldtRetirementDate = new DateTime();
                    if (ibusPersonOverview.iclbPayeeAccount.Where(item => /*item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED &&*/
                    item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                    item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() && //PIR 969
                   (item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT
                  || item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)).Count() > 0)
                    {

                        //PIR 985
                        iblnEligibleForQYRYAllocations = CheckIfEligibleToGetQuarterly(ibusPersonOverview.iclbPayeeAccount.Where(item =>
                    item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                    item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() &&
                   (item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT
                  || item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)).FirstOrDefault().icdoPayeeAccount.benefit_application_detail_id);


                        ldtRetirementDate = ibusPersonOverview.iclbPayeeAccount.Where(item => /*item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED &&*/
                    item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                    item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() && //PIR 969
                   (item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT
                  || item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)).FirstOrDefault().icdoPayeeAccount.idtRetireMentDate;

                    }
                    else
                    {
                        ldtRetirementDate = busGlobalFunctions.GetLastDateOfComputationYear(lintLastYear);

                        iblnRetiree = false;
                        iblnActive = true;
                    }
                    idtRetirementDate = ldtRetirementDate;
                    aobjBenefitApplication.icdoBenefitApplication.retirement_date = ldtRetirementDate;

                    //PIR 628 New
                    if (adtEACutOffDate != DateTime.MinValue && adtEACutOffDate != null)
                        aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(false, null, adtEACutOffDate);
                    else
                        aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                    //aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                    idtEffectiveDate = idtRetirementDate;

                    if (adtIAPHoursDetails != null && adtIAPHoursDetails.Rows.Count > 0 &&
                        adtIAPHoursDetails.AsEnumerable().Where(item => item.Field<DateTime>("ToDate") <= busGlobalFunctions.GetLastDayOfWeek(ldtRetirementDate)).Count() > 0)
                    {
                        adtIAPHoursDetails = adtIAPHoursDetails.AsEnumerable().Where(item => item.Field<DateTime>("ToDate") <= busGlobalFunctions.GetLastDayOfWeek(ldtRetirementDate)).CopyToDataTable();
                    }
                }
                else
                {
                    idtMDdate = GetBenefitMPIMDApplication(this.icdoPerson.person_id); //PROD PIR 504
                    iblnParticipant = true;
                    iblnActive = true;
                    DateTime ldtRetirementDate = busGlobalFunctions.GetLastDateOfComputationYear(lintLastYear);
                    aobjBenefitApplication.icdoBenefitApplication.retirement_date = ldtRetirementDate;
                    //PIR 628 New
                    if (adtEACutOffDate != DateTime.MinValue && adtEACutOffDate != null)
                        aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(false, null, adtEACutOffDate);
                    else
                        aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                    //aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                }
            }
            //PIR 869
            else if (ibusPersonOverview.iblnParticipant == busConstant.YES
               && ibusPersonOverview.istrRetiree == busConstant.NO && ibusPersonOverview.iclbPayeeAccount.Where(item =>
                   item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                    item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() && //PIR 969
                  (item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)).Count() > 0)
            {
                iblnParticipant = true;
                //  iblnRetiree = true;
                iblnActive = true;

                DateTime ldtRetirementDate = new DateTime();
                if (ibusPersonOverview.iclbPayeeAccount.Where(item =>
                   item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                    item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() && //PIR 969
                  (item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)).Count() > 0 
                  && busGlobalFunctions.GetVestedDate(icdoPerson.person_id, busConstant.MPIPP_PLAN_ID) == DateTime.MinValue)
                {

                    //PIR 985
                    iblnEligibleForQYRYAllocations = CheckIfEligibleToGetQuarterly(ibusPersonOverview.iclbPayeeAccount.Where(item =>
                   item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                    item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() &&
                   (item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)).FirstOrDefault().icdoPayeeAccount.benefit_application_detail_id);
                    
                  //      ldtRetirementDate = ibusPersonOverview.iclbPayeeAccount.Where(item =>
                  //item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                  // item.icdoPayeeAccount.istrFundType.IsNullOrEmpty() && //PIR 969
                  //(item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)).FirstOrDefault().icdoPayeeAccount.idtRetireMentDate;

                                  

                }
                else
                {
                    ldtRetirementDate = busGlobalFunctions.GetLastDateOfComputationYear(lintLastYear);

                    iblnRetiree = false;
                    iblnActive = true;
                }
                idtRetirementDate = ldtRetirementDate;
                aobjBenefitApplication.icdoBenefitApplication.retirement_date = ldtRetirementDate;

                //PIR 628 New
                if (adtEACutOffDate != DateTime.MinValue && adtEACutOffDate != null)
                    aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(false, null, adtEACutOffDate);
                else
                    aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                //aobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                idtEffectiveDate = idtRetirementDate;

                if (adtIAPHoursDetails != null && adtIAPHoursDetails.Rows.Count > 0 &&
                    adtIAPHoursDetails.AsEnumerable().Where(item => item.Field<DateTime>("ToDate") <= busGlobalFunctions.GetLastDayOfWeek(ldtRetirementDate)).Count() > 0)
                {
                    adtIAPHoursDetails = adtIAPHoursDetails.AsEnumerable().Where(item => item.Field<DateTime>("ToDate") <= busGlobalFunctions.GetLastDayOfWeek(ldtRetirementDate)).CopyToDataTable();
                }

            }

        }

        public void DetermineIfpersonIsReemployedAndLoadWorkHistory(ref busBenefitApplication aobjBenefitApplication, ref DataTable ldtIAPHoursDetailsForReeemployment, DateTime? adtEACutOffDate = null)//PIR 628 New
        {
            aobjBenefitApplication.aclbPersonWorkHistory_IAP = new Collection<cdoDummyWorkData>();
            idtRetirementDate = DateTime.MinValue;

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            SqlParameter[] lsqlParameters = new SqlParameter[7];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@FROM_DATE", DbType.DateTime);
            SqlParameter param3 = new SqlParameter("@TO_DATE", DbType.DateTime);
            SqlParameter param4 = new SqlParameter("@PLANCODE", DbType.String);
            SqlParameter param5 = new SqlParameter("@PROCESSED_FROM_DATE", DbType.DateTime);
            SqlParameter param6 = new SqlParameter("@PROCESSED_TO_DATE", DbType.DateTime);

            SqlParameter returnvalue = new SqlParameter("@RETURN_VALUE", DbType.Int32);
            returnvalue.Direction = ParameterDirection.ReturnValue;

            param1.Value = icdoPerson.istrSSNNonEncrypted;
            lsqlParameters[0] = param1;

            param2.Value = busGlobalFunctions.GetLastDayOfWeek(idtEffectiveDate).AddDays(1);//PIR 869
            lsqlParameters[1] = param2;

            param3.Value = DateTime.Now;
            lsqlParameters[2] = param3;

            param4.Value = "IAP";
            lsqlParameters[3] = param4;

            param5.Value = DBNull.Value;
            lsqlParameters[4] = param5;

            param6.Value = DBNull.Value;
            lsqlParameters[5] = param6;

            lsqlParameters[6] = returnvalue;
            //lsqlParameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

            DataTable ldtReemployedWorkHistory = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataBetweenTwoDates", lstrLegacyDBConnetion, null, lsqlParameters);
            if (ldtReemployedWorkHistory != null && ldtReemployedWorkHistory.Rows.Count > 0 && busGlobalFunctions.GetVestedDate(icdoPerson.person_id, busConstant.MPIPP_PLAN_ID) != DateTime.MinValue)
            {
                iblnReemployed = true;

                aobjBenefitApplication.aclbPersonWorkHistory_IAP = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtReemployedWorkHistory);
                if (aobjBenefitApplication.aclbPersonWorkHistory_IAP != null && aobjBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                {
                    idtEffectiveDateForReemployment = busGlobalFunctions.GetLastDateOfComputationYear(aobjBenefitApplication.aclbPersonWorkHistory_IAP.LastOrDefault().year);
                }

                DateTime ldtDate = busGlobalFunctions.GetLastDayOfWeek(aobjBenefitApplication.icdoBenefitApplication.retirement_date).AddDays(1);
                if (ldtIAPHoursDetailsForReeemployment != null && ldtIAPHoursDetailsForReeemployment.Rows.Count > 0 &&
                  ldtIAPHoursDetailsForReeemployment.AsEnumerable().Where(item => item.Field<DateTime>("FromDate") >= ldtDate).Count() > 0)
                {
                    ldtIAPHoursDetailsForReeemployment = ldtIAPHoursDetailsForReeemployment.AsEnumerable().Where(item => item.Field<DateTime>("FromDate") >= ldtDate).CopyToDataTable();
                }
            }

        }

        //PIR 985
        public bool CheckIfEligibleToGetQuarterly(int aintBenefitApplicationDetailId)
        {
            DataTable ldtblBenefitCalculationHeader = Select("cdoIAPAllocationDetailPersonOverview.GetFinalCalcualtionFromBenefitDetailId", new object[1] { aintBenefitApplicationDetailId });
            if (ldtblBenefitCalculationHeader != null && ldtblBenefitCalculationHeader.Rows.Count > 0)
            {
                if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= DateTime.Now.Year - 1)
                {
                    return true;
                }
            }
            return false;
        }

        ///Parameters adtEACutOffDate & aintComputationYear are strictly used for Late IAP Alloation Batch.
        ///These parameters has no significance in IAP Recalculation Functionality.Please do not use these parameters.
        public void RecalculateAndFillAllocationDetails(Collection<cdoDummyWorkData> aclbPersonWorkHistory_IAP, DataTable adtIAPHoursDetails, busBenefitApplication aobjBenefitApplication, int aintPersonAccountId, DataTable adtIAPAllContributions, bool ablnIsRefreshAllocations = false, DataTable adtblIAPBalanceUptoYear = null)
        {

            busIAPAllocationHelper lobjIAPAllocationHelper = new busIAPAllocationHelper();
            lobjIAPAllocationHelper.LoadIAPAllocationFactor();

            int lintComputationYear;
            lintComputationYear = 0;
            decimal ldecTotalYTDHours, ldecThru79Hours;
            ldecThru79Hours = ldecTotalYTDHours = 0.0M;
            decimal ldecAllocation4Amount, ldecIAPAccountBalance = 0.00M, ldecAlloc1Amount, ldecAlloc2Amount, ldecAlloc2InvstAmount, ldecAlloc2FrftAmount, ldecAlloc3Amount, ldecAlloc4InvstAmount, ldecAlloc4FrftAmount, ldecAlloc5AfflAmount,
                ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount;
            decimal ldecFactor = 0;

            bool lblnAgeAtReemploymentGreaterThan65 = false;

            #region IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979

            //PIR 0971
            if (ibusPersonT79hours != null && ibusPersonT79hours.icdoPersonT79hours.approved_flag == busConstant.FLAG_YES)
            {
                idecThru79Hours = ibusPersonT79hours.icdoPersonT79hours.t79_hours;
                ldecThru79Hours = idecThru79Hours;
            }
            else
            {
                //Remove history for any forfieture year 1979
                if (aclbPersonWorkHistory_IAP != null && aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
                {
                    if (aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.istrForfietureFlag == busConstant.FLAG_YES).Count() > 0)
                    {
                        int lintMaxForfietureYearBefore1979 = aclbPersonWorkHistory_IAP.Where(item => item.year <= busConstant.BenefitCalculation.YEAR_1979 && item.istrForfietureFlag == busConstant.FLAG_YES).Max(t => t.year);
                        aclbPersonWorkHistory_IAP = aclbPersonWorkHistory_IAP.Where(item => item.year > lintMaxForfietureYearBefore1979).ToList().ToCollection();
                    }
                }

                if (aclbPersonWorkHistory_IAP != null && aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
                {
                    decimal ldecPreviousYearPaidIAPAccountBalance = 0M;
                    cdoDummyWorkData lcdoWorkData1979 = aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).FirstOrDefault();
                    //IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
                    //PIR 836
                    if ((lcdoWorkData1979 != null && aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979 - 1).Count() == 0)
                       || (aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979 - 1).Count() > 0 &&
                                   aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979 - 1).FirstOrDefault().bis_years_count < 2))
                    {
                        int lintPaymentYear = 0;
                        DataTable ldtblPaymentYear = Select("cdoPersonAccountRetirementContribution.GetMaxPaymentYearOnOrBefore1979", new object[1] { aobjBenefitApplication.icdoBenefitApplication.person_id });
                        if (ldtblPaymentYear != null && ldtblPaymentYear.Rows.Count > 0 && Convert.ToString(ldtblPaymentYear.Rows[0][0]).IsNotNullOrEmpty())
                        {
                            lintPaymentYear = Convert.ToInt32(ldtblPaymentYear.Rows[0][0]);
                        }
                        if (lintPaymentYear == 0)
                        {

                            idecThru79Hours = aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Sum(o => o.qualified_hours);

                        }
                        else
                        {
                            if (aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Count() > 0)
                            {
                                idecThru79Hours = aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Sum(o => o.qualified_hours);
                            }
                        }

                        idecThru79Hours += ldecPreviousYearPaidIAPAccountBalance;
                        if (idecThru79Hours < 0)
                            idecThru79Hours = 0;
                        ldecThru79Hours = idecThru79Hours;
                    }
                }
            }

            if (aclbPersonWorkHistory_IAP != null && aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
            {
                aclbPersonWorkHistory_IAP = aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).ToList().ToCollection();
            }

            #endregion


            bool lblnAgeFlag = false;
            lblnAgeFlag = busGlobalFunctions.CalculatePersonAge(aobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, Convert.ToDateTime(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPInceptionDate))) < 55 ? true : false;

            #region Pad the missing Years and for active Participants bring work history till last allocation

            int lintFirstYear = 0;
            int lintLastYear = 0;

            if (aclbPersonWorkHistory_IAP != null && aclbPersonWorkHistory_IAP.Count > 0)
            {

                lintFirstYear = aclbPersonWorkHistory_IAP.FirstOrDefault().year;

                DataTable ldtbIAPAllocation = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationsDetailsForPersonOverview", new object[1] { aintPersonAccountId });
                if (ldtbIAPAllocation.Rows.Count > 0)
                {
                    if (iblnActive && !iblnDead && !iblnRetiree && !iblnMDParticipant)
                        lintLastYear = lintLastYear = this.ibusAllocationSummary.icdoIapAllocationSummary.computation_year;
                    else
                        lintLastYear = Convert.ToInt32(ldtbIAPAllocation.Rows[ldtbIAPAllocation.Rows.Count - 1]["COMPUTATIONAL_YEAR"]);

                    //PIR 985
                    if (!iblnMDParticipant && iblnEligibleForQYRYAllocations && idtRetirementDate.Year > lintLastYear && idtRetirementDate.Year >= ibusAllocationSummary.icdoIapAllocationSummary.computation_year)
                    {
                        lintLastYear = idtRetirementDate.Year;
                    }
                    //Prod Pir 283 : For Reemplyment Recalculate Method gets called twice 1st time it should only get calculated till retirement year.
                    else if (iblnRetiree && !iblnReemployed && lintLastYear > idtRetirementDate.Year)
                    {
                        lintLastYear = idtRetirementDate.Year;
                    }
                    else if (iblnMDParticipant)//Prod PIR 283 (contribution till 2012 but md date in 2013)
                    {
                        if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= idtMDdate.Year) //MD logic
                        {
                            lintLastYear = ibusAllocationSummary.icdoIapAllocationSummary.computation_year;
                        }
                        else if (lintLastYear < idtMDdate.Year)
                        {
                            lintLastYear = idtMDdate.Year;
                        }
                    }
                    else if (iblnReemployed)  //this condition added for PIR 1093
                    {
                        lintLastYear = this.ibusAllocationSummary.icdoIapAllocationSummary.computation_year;
                    }
                    //PIR 985 
                    //else if (iblnReemployed)
                    //{
                    //    ldecIAPAccountBalance = (from obj in iclbIAPAllocationDetailPersonOverview where obj.icdoIapallocationDetailPersonoverview.istrSource == "Computed" select obj.icdoIapallocationDetailPersonoverview.idecTotal).Sum();
                    //}
                }
                else  //PIR 628
                {
                    lintLastYear = this.ibusAllocationSummary.icdoIapAllocationSummary.computation_year;
                }

                //for active Participants bring work history till last allocation
                if (iblnActive && aclbPersonWorkHistory_IAP != null && aclbPersonWorkHistory_IAP.Count > 0)
                {
                    if (aclbPersonWorkHistory_IAP.Where(item => item.year <= lintLastYear).Count() > 0)
                        aclbPersonWorkHistory_IAP = aclbPersonWorkHistory_IAP.Where(item => item.year <= lintLastYear).ToList().ToCollection();

                    idtEffectiveDate = busGlobalFunctions.GetLastDateOfComputationYear(lintLastYear);
                }

                if (lintFirstYear > 0 && lintLastYear > 0)
                {
                    for (int i = lintFirstYear; i <= lintLastYear; i++)
                    {
                        if (aclbPersonWorkHistory_IAP.Where(item => item.year == i).Count() > 0)
                            continue;
                        int lintWorkHistoryYear = i;
                        //LineNo 22 # IAP Enhancement project-
                        if (this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).Count() > 0)
                        {
                            if (CheckIfFactorAvailableForIapAllocation())
                            {
                                cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                                lcdoDummyWorkData.year = lintWorkHistoryYear;
                                lcdoDummyWorkData.qualified_hours = 0M;
                                lcdoDummyWorkData.IAP_HOURSA2 = 0M;
                                lcdoDummyWorkData.IAP_PERCENT = 0M;
                                aclbPersonWorkHistory_IAP.Add(lcdoDummyWorkData);
                            }
                        }
                        else
                        {
                            cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                            lcdoDummyWorkData.year = lintWorkHistoryYear;
                            lcdoDummyWorkData.qualified_hours = 0M;
                            lcdoDummyWorkData.IAP_HOURSA2 = 0M;
                            lcdoDummyWorkData.IAP_PERCENT = 0M;
                            aclbPersonWorkHistory_IAP.Add(lcdoDummyWorkData);
                        }
                    }
                }

                aclbPersonWorkHistory_IAP = aclbPersonWorkHistory_IAP.OrderBy(item => item.year).ToList().ToCollection();
            }
            #endregion Pad the missing Years

            #region Check If Retirement Year Hours if not then pad the Retirement Year
            if ((iblnRetiree //|| iblnMDParticipant
                ) && !iblnReemployed)
            {
                int lintWorkHistoryYear = aclbPersonWorkHistory_IAP.LastOrDefault().year + 1;
                while (lintWorkHistoryYear <= idtRetirementDate.Year)
                {
                    if (lintWorkHistoryYear == idtRetirementDate.Year && (idtRetirementDate.Month == 1 || idtRetirementDate.Month == 2 || idtRetirementDate.Month == 3))
                        break;
                    //LineNo 22 # IAP Enhancement project-
                    if (this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).Count() > 0)
                    {
                        if (CheckIfFactorAvailableForIapAllocation())
                        {
                            cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                            lcdoDummyWorkData.year = lintWorkHistoryYear;
                            lcdoDummyWorkData.qualified_hours = 0M;
                            lcdoDummyWorkData.IAP_HOURSA2 = 0M;
                            lcdoDummyWorkData.IAP_PERCENT = 0M;
                            aclbPersonWorkHistory_IAP.Add(lcdoDummyWorkData);
                        }
                    }
                    else
                    {
                        cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                        lcdoDummyWorkData.year = lintWorkHistoryYear;
                        lcdoDummyWorkData.qualified_hours = 0M;
                        lcdoDummyWorkData.IAP_HOURSA2 = 0M;
                        lcdoDummyWorkData.IAP_PERCENT = 0M;
                        aclbPersonWorkHistory_IAP.Add(lcdoDummyWorkData);
                    }
                    lintWorkHistoryYear++;
                }
            }
            #endregion
            int lintPreviousYear = 0;

            //PIR 969
            DataTable ldtblQDROChildSupportPayments = busBase.Select("entIAPAllocationDetailPersonOverview.GetQDROChildSupportPayments", new object[1] { aintPersonAccountId });
            //IAP Enhancement Project
            //DataTable ldtbForFeitureDate = busBase.Select("cdoPersonAccount.GetForFeitureDate", new object[2] { this.icdoPerson.person_id, busConstant.MPIPP_PLAN_ID });
            //if (ldtbForFeitureDate.Rows.Count > 0)
            //{
            //    if (ldtbForFeitureDate.Rows[0][0].ToString() != string.Empty)
            //    {
            //        DateTime dt = Convert.ToDateTime(ldtbForFeitureDate.Rows[0][0]);
            //        int dtYear = dt.Year;
            //        if (aclbPersonWorkHistory_IAP.Where(i => i.istrForfietureFlag == busConstant.FLAG_YES).Count() == 0)
            //        {
            //            foreach (cdoDummyWorkData item in aclbPersonWorkHistory_IAP)
            //            {
            //                if (item.year == dtYear)
            //                {
            //                    item.istrForfietureFlag = busConstant.FLAG_YES;

            //                }

            //            }

            //        }

            //    }

            //}

            foreach (cdoDummyWorkData lcdoIAPHours in aclbPersonWorkHistory_IAP)
            {
                //PIR 885
                ldecFactor = 0;
                ldecTotalYTDHours = 0.0M;
                lintComputationYear = 0;
                //lblnAgeFlag = false;
                ldecAlloc1Amount = ldecAlloc2Amount = ldecAlloc2InvstAmount = ldecAlloc2FrftAmount = ldecAlloc3Amount = ldecAllocation4Amount = ldecAlloc4InvstAmount = ldecAlloc4FrftAmount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = 0.00M;
                lintComputationYear = Convert.ToInt32(lcdoIAPHours.year);
                string lstrCalculateAlloc5 = busConstant.FLAG_NO;
                string lstrForfietureFlag = busConstant.FLAG_NO;

                DataTable ldtIAPHoursFiltered = new DataTable();
                if (adtIAPHoursDetails.AsEnumerable().Where(item => item.Field<Int16>("ComputationYear") == lintComputationYear).Count() > 0)
                {
                    ldtIAPHoursFiltered = adtIAPHoursDetails.FilterTable(utlDataType.Numeric, "ComputationYear", lintComputationYear).CopyToDataTable();
                }

                //PIR 1014
                if (iblnRetiree && !iblnReemployed && idtMinimumDistributionDate != DateTime.MinValue && idtMinimumDistributionDate.Year == lintComputationYear)
                {
                    idtMDdate = idtMinimumDistributionDate;
                    iblnMDParticipant = true;
                }
                else if (iblnRetiree && !iblnReemployed && idtMinimumDistributionDate != DateTime.MinValue && idtMinimumDistributionDate.Year != lintComputationYear && iblnMDParticipant)
                {
                    idtMDdate = DateTime.MinValue;
                    iblnMDParticipant = false;
                }

                //PIR 628 extended
                if (adtblIAPBalanceUptoYear != null && adtblIAPBalanceUptoYear.Rows.Count > 0)
                {
                    if (Convert.ToString(adtblIAPBalanceUptoYear.Rows[0][enmPersonAccountRetirementContribution.computational_year.ToString().ToUpper()]).IsNotNullOrEmpty()
                        && Convert.ToInt32(adtblIAPBalanceUptoYear.Rows[0][enmPersonAccountRetirementContribution.computational_year.ToString().ToUpper()]) + 1 == lintComputationYear)
                    {
                        ldecIAPAccountBalance = adtblIAPBalanceUptoYear.Rows[0][enmPersonAccountRetirementContribution.iap_balance_amount.ToString().ToUpper()] == null ?
                            0 : Convert.ToDecimal(adtblIAPBalanceUptoYear.Rows[0][enmPersonAccountRetirementContribution.iap_balance_amount.ToString().ToUpper()]);

                        if (iclbPaidIAPAllocationDetailPersonOverview != null && iclbPaidIAPAllocationDetailPersonOverview.Count() > 0
                            && iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear - 1).Count() > 0)
                        {
                            decimal ldecPaidIAPAccountBalance = 0M;
                            if (ldtblQDROChildSupportPayments != null && ldtblQDROChildSupportPayments.Rows.Count > 0
                                && ldtblQDROChildSupportPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear - 1).Count() > 0)
                            {
                                ldecPaidIAPAccountBalance = ldtblQDROChildSupportPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear - 1).Sum(t => t.Field<decimal>("AMOUNT"));
                                ldecIAPAccountBalance -= ldecPaidIAPAccountBalance;
                            }
                        }
                    }
                }


                if (iblnReemployed)
                {
                    //PIR 985
                    //if (ldtIAPHoursFiltered != null && ldtIAPHoursFiltered.Rows.Count > 0)  //PIR 1093 - Age 65 flag should always be checked in case of re-employement
                    //{
                    if (lintComputationYear <= ibusAllocationSummary.icdoIapAllocationSummary.computation_year)
                    {
                        DateTime ldtDeterminationDate = new DateTime();
                        if (aobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth.Month == 1)
                            ldtDeterminationDate = new DateTime(lintComputationYear,
                                aobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth.Month, 01);
                        //PIR 985
                        else
                            ldtDeterminationDate = new DateTime(lintComputationYear,
                               aobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth.Month, aobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth.Day).GetLastDayofMonth().AddDays(1);

                        lblnAgeAtReemploymentGreaterThan65 = busGlobalFunctions.CalculatePersonAge(aobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, ldtDeterminationDate) > 65 ? true : false;
                    }
                    //}
                }

                ldecTotalYTDHours = lcdoIAPHours.IAP_HOURSA2;

                //change 01302014
                #region Negate Alloction Amount Paid if any for Previous year(This will handle withdrawal as well as QDRO if any)


                decimal ldecQDROPayments = 0M;

                //LA SunSet -- Code to Toggle between Rule 1 and Rule2
                //Ticket#79099
                if (this.ibusPersonAccount.icdoPersonAccount.recalculate_rule1_flag == "N" || this.ibusPersonAccount.icdoPersonAccount.recalculate_rule1_flag.IsNullOrEmpty())
                {
                    //Rule 2
                    //PIR 969
                    if (iclbPaidIAPAllocationDetailPersonOverview != null && iclbPaidIAPAllocationDetailPersonOverview.Count() > 0
                   && iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).Count() > 0)
                    {
                        ldecQDROPayments = 0M;
                        if (ldtblQDROChildSupportPayments != null && ldtblQDROChildSupportPayments.Rows.Count > 0
                            && ldtblQDROChildSupportPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear).Count() > 0)
                        {
                            ldecQDROPayments = ldtblQDROChildSupportPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear).Sum(t => t.Field<decimal>("AMOUNT"));
                            ldecIAPAccountBalance -= ldecQDROPayments;
                        }
                    }
                }
                else
                {
                    //Rule 1
                    if (iclbPaidIAPAllocationDetailPersonOverview != null && iclbPaidIAPAllocationDetailPersonOverview.Count() > 0
                          && iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear - 1).Count() > 0)
                    {
                        ldecQDROPayments = 0M;
                        if (ldtblQDROChildSupportPayments != null && ldtblQDROChildSupportPayments.Rows.Count > 0
                            && ldtblQDROChildSupportPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear - 1).Count() > 0)
                        {
                            ldecQDROPayments = ldtblQDROChildSupportPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear - 1).Sum(t => t.Field<decimal>("AMOUNT"));
                            ldecIAPAccountBalance -= ldecQDROPayments;
                        }
                    }

                }

                decimal ldecPreviousYearPaidIAPAccountBalance = 0M;
                if ((iclbPaidIAPAllocationDetailPersonOverview != null && iclbPaidIAPAllocationDetailPersonOverview.Count() > 0
                    && iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear - 1).Count() > 0
                    && !iblnReemployed) ||
                    (iblnReemployed && iclbPaidIAPAllocationDetailPersonOverview != null && iclbPaidIAPAllocationDetailPersonOverview.Count() > 0
                    && iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear - 1).Count() > 0
                    && idtEffectiveDate.Year < lintComputationYear - 1 &&
                    (iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear - 1).
                         FirstOrDefault().icdoIapallocationDetailPersonoverview.effective_date > idtEffectiveDate ||
                        (ldtblQDROChildSupportPayments != null && ldtblQDROChildSupportPayments.Rows.Count > 0 &&
                        ldtblQDROChildSupportPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear - 1).Count() > 0)) //PIR 628 Extended
                    ))
                {
                    busIapAllocationDetailCalculation lcdoIAPPaid = iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear - 1).FirstOrDefault();
                    ldecPreviousYearPaidIAPAccountBalance = lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc1 + lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc2 + lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc2_invt + lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc2_frft + lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc3 + lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc4 + lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc4_invt + lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc4_frft + lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc5;
                    decimal ldecQDROChildSupportPayments = 0M;

                    //PIR 969
                    if (iclbPaidIAPAllocationDetailPersonOverview != null && iclbPaidIAPAllocationDetailPersonOverview.Count() > 0
                    && iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear - 1).Count() > 0)
                    {
                        if (ldtblQDROChildSupportPayments != null && ldtblQDROChildSupportPayments.Rows.Count > 0
                            && ldtblQDROChildSupportPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear - 1).Count() > 0)
                        {
                            ldecQDROChildSupportPayments = ldtblQDROChildSupportPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear - 1).Sum(t => t.Field<decimal>("AMOUNT"));
                        }
                    }

                    ldecIAPAccountBalance += ldecPreviousYearPaidIAPAccountBalance;

                    //PIR 969
                    ldecIAPAccountBalance += ldecQDROChildSupportPayments;

                }
                #endregion Negate Alloction Amount Paid if any for current year


                #region If there is any Forfieture for previous year then Balance amount will become zero
                if (aclbPersonWorkHistory_IAP != null && aclbPersonWorkHistory_IAP.Count() > 0
                  && aclbPersonWorkHistory_IAP.Where(item => item.year == lintComputationYear - 1 && item.istrForfietureFlag == busConstant.FLAG_YES).Count() > 0)
                {
                    ldecIAPAccountBalance = 0M;
                    ldecThru79Hours = 0M;
                    if (aclbPersonWorkHistory_IAP.Where(item => item.year <= lintComputationYear - 1).Count() > 0)
                    {
                        aclbPersonWorkHistory_IAP.Where(item => item.year <= lintComputationYear - 1).ForEach(t => t.qualified_hours = 0);
                        aclbPersonWorkHistory_IAP.Where(item => item.year <= lintComputationYear - 1).ForEach(t => t.qualified_years_count = 0);
                        aclbPersonWorkHistory_IAP.Where(item => item.year <= lintComputationYear - 1).ForEach(t => t.anniversary_years_count = 0);
                        aclbPersonWorkHistory_IAP.Where(item => item.year <= lintComputationYear - 1).ForEach(t => t.vested_years_count = 0);
                    }
                }

                #endregion

                //Check if Withdrawal
                //DateTime ldtPaymentEffectiveDate = new DateTime();
                //if (iclbPaidIAPAllocationDetailPersonOverview != null && iclbPaidIAPAllocationDetailPersonOverview.Count() > 0
                //   && iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).Count() > 0)
                //{
                //    ldtPaymentEffectiveDate = iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).FirstOrDefault().icdoIapallocationDetailPersonoverview.effective_date;
                //}
                DataTable ldtbWithdrawalApplicationforQuarterlyAllocation = busBase.Select("cdoBenefitApplication.CheckWithdrawalApplicationforQuarterlyAllocation", new object[2] { aobjBenefitApplication.icdoBenefitApplication.person_id, lintComputationYear });
                DateTime lintWithDrawalDate = new DateTime();
                if (ldtbWithdrawalApplicationforQuarterlyAllocation.Rows.Count > 0)
                    lintWithDrawalDate = Convert.ToDateTime(ldtbWithdrawalApplicationforQuarterlyAllocation.Rows[0]["WITHDRAWAL_DATE"]);
               

                if ((!iblnReemployed || (iblnReemployed && !lblnAgeAtReemploymentGreaterThan65)) && lcdoIAPHours.istrForfietureFlag != busConstant.FLAG_YES && aobjBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinIAP=="Y")//PIR 985
                {
                    if (!iblnMDParticipant)
                    {
                        //Rohan - 10062014 Pre - Retirement Death Example PIR 764
                        if (!iblnReemployed && ((iblnRetiree || iblnDead) && lintComputationYear == idtRetirementDate.Year))
                        {
                            int lintQuarter = 0;
                            lintQuarter = busGlobalFunctions.GetPreviousQuarter(idtRetirementDate);
                            ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecIAPAccountBalance, lintQuarter, ref ldecFactor);
                        }
                        //Check if Withdrawal
                        //else if (ldtPaymentEffectiveDate != DateTime.MinValue)
                        //{
                        //    int lintQuarter = 0;
                        //    lintQuarter = busGlobalFunctions.GetPreviousQuarter(ldtPaymentEffectiveDate);
                        //    ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecIAPAccountBalance, lintQuarter, ref ldecFactor);
                        //}
                        else if (ldtbWithdrawalApplicationforQuarterlyAllocation.Rows.Count > 0 && lintComputationYear == lintWithDrawalDate.Year)
                        {
                                int lintQuarter = 0;
                                lintQuarter = busGlobalFunctions.GetPreviousQuarter(lintWithDrawalDate);
                                
                                ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecIAPAccountBalance, lintQuarter, ref ldecFactor);
                            
                        }
                        else if (!iblnReemployed && (((iblnRetiree && lcdoIAPHours.year >= ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1 && idtRetirementDate.Year > ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1)
                            || (iblnDead && lcdoIAPHours.year >= aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year
                                        && aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year > ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1)))) //PIR 1049
                        {
                            ldecAlloc1Amount = 0.0M;
                        }
                        else
                        {
                            ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecIAPAccountBalance, 4, ref ldecFactor);
                        }
                    }
                    else if (iblnMDParticipant)
                    {
                        if (lintComputationYear < idtMDdate.Year)
                        {
                            ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecIAPAccountBalance, 4, ref ldecFactor);
                        }
                        else
                        {
                            if (iblnMDParticipant
                                 && lintComputationYear == idtMDdate.Year)
                            {
                                int lintQuarter = 0;
                                lintQuarter = busGlobalFunctions.GetPreviousQuarter(idtMDdate);
                                ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecIAPAccountBalance, lintQuarter, ref ldecFactor);
                            }
                        }
                    }

                }
                // ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecIAPAccountBalance, 4, ref ldecFactor);

                if (iblnMDParticipant && lintComputationYear >= idtMDdate.Year && ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= idtMDdate.Year
                    && !iblnReemployed && lcdoIAPHours.qualified_hours >= Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.QualifiedYearHours)) && aobjBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinIAP == "Y")
                {
                    //PIR 283 : Alloc 2 & 4 will be calculated by year end allocation batch for MD partiicipants
                    if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= lintComputationYear)
                    {
                        // PROD PIR 544
                        if ((lcdoIAPHours.year == idtRetirementDate.Year || (iblnDead && lcdoIAPHours.year == aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year))//PIR 836
                                && (ibusAllocationSummary.icdoIapAllocationSummary.computation_year < idtRetirementDate.Year ||
                                 (iblnDead && ibusAllocationSummary.icdoIapAllocationSummary.computation_year < aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year)))
                        {
                            ldecAlloc2InvstAmount = 0.00M; ldecAlloc2FrftAmount = 0.00M; ldecAlloc4InvstAmount = 0.00M; ldecAlloc4FrftAmount = 0.00M;
                        }
                        else
                        {
                            //method to calculate allocation 2 amount
                            ldecAlloc2Amount = lobjIAPAllocationHelper.CalculateAllocation2Amount(lintComputationYear, ldecThru79Hours, ldecTotalYTDHours, DateTime.MinValue,
                                                                                   DateTime.MinValue, DateTime.MinValue); //PIR 985

                            //method to calculate allocation 2 investment amount
                            ldecAlloc2InvstAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
                                                                                   DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationInvestmentFlag); //PIR 985
                            //method to calculate allocation 2 forfeiture amount
                            ldecAlloc2FrftAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
                                                                                   DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationForfeitureFlag); //PIR 985


                            var ldtIAPPercent = from DataRow row in ldtIAPHoursFiltered.AsEnumerable()
                                                group row by new { ID = row.Field<string>("OldEmployerNum") } into g
                                                select new
                                                {
                                                    iappercent = g.Sum(row => row["iappercent"] == DBNull.Value ? 0.0M : (decimal)row["iappercent"])
                                                };

                            foreach (var ldrIAPPercent in ldtIAPPercent)
                            {
                                ldecAllocation4Amount += lobjIAPAllocationHelper.CalculateAllocation4Amount(lintComputationYear, null, ldrIAPPercent.iappercent);
                            }

                            //method to calculate allocation 4 investment amount
                            ldecAlloc4InvstAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationInvestmentFlag);
                            //method to calculate allocation 4 forfeiture amount
                            ldecAlloc4FrftAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationForfeitureFlag);
                        }
                    }
                }
                else if (!iblnReemployed && lcdoIAPHours.qualified_hours >= Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.QualifiedYearHours)))
                {

                    //PIR- 791: Last 2 paramters passed to below method was 'DateTime.MinValue'
                    //method to calculate allocation 2 amount

                    //PIR 985
                    if ((iblnMDParticipant && lcdoIAPHours.year == idtMDdate.Year && ibusAllocationSummary.icdoIapAllocationSummary.computation_year < idtMDdate.Year)
                        ||
                        ((iblnRetiree && lcdoIAPHours.year >= ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1 && idtRetirementDate.Year > ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1)
                            || (iblnDead && lcdoIAPHours.year >= aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year
                                        && aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year > ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1)||(aobjBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinIAP != "Y"))//PIR 1049
                        )
                    {
                        ldecAlloc2Amount = 0;
                    }
                    else //if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= lintComputationYear) //PIR 1049 
                    {
                        ldecAlloc2Amount = lobjIAPAllocationHelper.CalculateAllocation2Amount(lintComputationYear, ldecThru79Hours, ldecTotalYTDHours, idtRetirementDate,
                                                                                idtMDdate, aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death);
                    }

                    //PIR 283 : Alloc 2 & 4 will be calculated by year end allocation batch for MD partiicipants
                    //02242014
                    //if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= lintComputationYear)
                    // {
                    if (iblnMDParticipant)
                    {
                        //PROD PIR 544
                        //PIR 985
                        if ((lcdoIAPHours.year == idtMDdate.Year || (iblnDead && lcdoIAPHours.year == aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year))//PIR 836
                            && (ibusAllocationSummary.icdoIapAllocationSummary.computation_year < idtMDdate.Year ||
                                 (iblnDead && ibusAllocationSummary.icdoIapAllocationSummary.computation_year < aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year)))
                        {
                            ldecAlloc2InvstAmount = 0.00M; ldecAlloc2FrftAmount = 0.00M;
                        }

                        else if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= lintComputationYear)
                        {
                            //Prod Pir 283 : Invst & forfetiure needs to be calculated for all years including md date year.
                            //method to calculate allocation 2 investment amount
                            ldecAlloc2InvstAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
                                                                                    DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationInvestmentFlag);
                            //method to calculate allocation 2 forfeiture amount
                            ldecAlloc2FrftAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
                                                                                    DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationForfeitureFlag);
                        }
                    }
                    else
                    {    //change 01302014
                        //if ((iblnRetiree || iblnDead) && lintComputationYear == idtRetirementDate.Year)
                        //{
                        //    ;
                        //}
                        //PROD PIR 544
                        //PIR 985
                        //PIR 1016 commented iblnDead flat in condition
                        if (((iblnRetiree && lcdoIAPHours.year == idtRetirementDate.Year) || (/*iblnDead &&*/ lcdoIAPHours.year == aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year))
                            || ((iblnRetiree && lcdoIAPHours.year >= ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1 && idtRetirementDate.Year > ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1)
                            || (/*iblnDead &&*/ lcdoIAPHours.year >= aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year
                                        && aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year > ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1))|| (aobjBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinIAP != "Y"))//PIR 836 //PIR 1049 
                        //&& (ibusAllocationSummary.icdoIapAllocationSummary.computation_year < idtRetirementDate.Year ||
                        //     (iblnDead && ibusAllocationSummary.icdoIapAllocationSummary.computation_year < aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year)))
                        {
                            ldecAlloc2InvstAmount = 0.00M; ldecAlloc2FrftAmount = 0.00M;
                        }
                        else //if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= lintComputationYear) //PIR 1049 
                        {
                            //method to calculate allocation 2 investment amount
                            ldecAlloc2InvstAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, idtRetirementDate,
                                                                                    DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationInvestmentFlag);
                            //method to calculate allocation 2 forfeiture amount
                            ldecAlloc2FrftAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, idtRetirementDate,
                                                                                    DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationForfeitureFlag);
                        }
                    }//method to calculate allocation 3 amount
                    if(aobjBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinIAP == "Y")
                    {
                        ldecAlloc3Amount = lobjIAPAllocationHelper.CalculateAllocation3Amount(lintComputationYear, ldecThru79Hours, ldecTotalYTDHours);

                    }
                   

                    //PIR 985
                    if ((iblnMDParticipant && lcdoIAPHours.year == idtMDdate.Year && ibusAllocationSummary.icdoIapAllocationSummary.computation_year < idtMDdate.Year)
                        || ((iblnRetiree && lcdoIAPHours.year >= ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1 && idtRetirementDate.Year > ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1)
                            || (iblnDead && lcdoIAPHours.year >= aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year
                                        && aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year > ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1))|| (aobjBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinIAP != "Y")) //PIR 1049
                    {
                        ldecAllocation4Amount = 0;
                    }
                    //WI 22862 Ticket 141846 checking parameter which is passed when it is called by Late IAP allocation batch, and year end is not done
                    //else if ((adtblIAPBalanceUptoYear != null && adtblIAPBalanceUptoYear.Rows.Count > 0) && lcdoIAPHours.year >= ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1)
                    //{
                    //    ldecAllocation4Amount = 0;
                    //}
                    //Ticket#109805
                    else //if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= lintComputationYear) //PIR 1049 
                    {
                        var ldtIAPPercent = from DataRow row in ldtIAPHoursFiltered.AsEnumerable()
                                            group row by new { ID = row.Field<string>("OldEmployerNum") } into g
                                            select new
                                            {
                                                iappercent = g.Sum(row => row["iappercent"] == DBNull.Value ? 0.0M : (decimal)row["iappercent"])
                                            };

                        foreach (var ldrIAPPercent in ldtIAPPercent)
                        {
                            ldecAllocation4Amount += lobjIAPAllocationHelper.CalculateAllocation4Amount(lintComputationYear, null, ldrIAPPercent.iappercent);
                        }
                    }

                    //change 02242014
                    if (iblnMDParticipant)
                    {
                        //PROD PIR 544
                        //PIR 985
                        if ((lcdoIAPHours.year == idtMDdate.Year || (iblnDead && lcdoIAPHours.year == aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year))//PIR 836
                            && (ibusAllocationSummary.icdoIapAllocationSummary.computation_year < idtMDdate.Year ||
                                 (iblnDead && ibusAllocationSummary.icdoIapAllocationSummary.computation_year < aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year)))
                        {
                            ldecAlloc4InvstAmount = 0.00M; ldecAlloc4FrftAmount = 0.00M;
                        }
                        else if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= lintComputationYear)
                        {
                            //method to calculate allocation 4 investment amount
                            ldecAlloc4InvstAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationInvestmentFlag);
                            //method to calculate allocation 4 forfeiture amount
                            ldecAlloc4FrftAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationForfeitureFlag);
                        }
                    }
                    else
                    {
                        //change 01302014
                        //if ((iblnRetiree || iblnDead) && lintComputationYear == idtRetirementDate.Year)
                        //{
                        //    ;
                        //}
                        //PROD PIR 544
                        //PIR 985
                        //PIR 1016 commented iblnDead flat in condition
                        if (((iblnRetiree && lcdoIAPHours.year == idtRetirementDate.Year) || (/*iblnDead &&*/ lcdoIAPHours.year == aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year))
                            || ((iblnRetiree && lcdoIAPHours.year >= ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1 && idtRetirementDate.Year > ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1)
                            || (/*iblnDead &&*/ lcdoIAPHours.year >= aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year
                                        && aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year > ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1)) || (aobjBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinIAP != "Y"))//PIR 836 //PIR 1049
                        {
                            ldecAlloc4InvstAmount = 0.00M; ldecAlloc4FrftAmount = 0.00M;
                        }
                        else //if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= lintComputationYear) //PIR 1049 
                        {

                            //method to calculate allocation 4 investment amount
                            ldecAlloc4InvstAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationInvestmentFlag);
                            //method to calculate allocation 4 forfeiture amount
                            ldecAlloc4FrftAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationForfeitureFlag);
                        }
                    }
                    //Block to calculate allocation 5 amount

                    if (lintComputationYear >= 1996 && lintComputationYear <= 2001)
                    {
                        if (ablnIsRefreshAllocations)
                        {
                            if (iarrAlloc5Years != null && iarrAlloc5Years.Contains(lintComputationYear))
                                lstrCalculateAlloc5 = busConstant.FLAG_YES;
                            else
                                lstrCalculateAlloc5 = busConstant.FLAG_NO;
                        }
                        else
                        {
                            if (iclbIapAllocation5Recalculation != null && iclbIapAllocation5Recalculation.Where(item => item.computational_year == lintComputationYear).Count() > 0)
                            {
                                lstrCalculateAlloc5 = iclbIapAllocation5Recalculation.Where(item => item.computational_year == lintComputationYear).FirstOrDefault().iap_allocation5_recalculate_flag;
                            }
                            else if (ldecAllocation4Amount != 0.00M)
                            {
                                lstrCalculateAlloc5 = busConstant.FLAG_YES;
                            }
                        }
                        //IAP Enhancement Project -- LineItem#23.
                        if (iclbIapAllocation5Recalculation.Count == 0)
                        {
                            if (lintComputationYear == 1996)
                            {
                                if (lcdoIAPHours.qualified_hours >= 400)
                                {
                                    lstrCalculateAlloc5 = "Y";
                                }
                            }
                        }

                        if (lstrCalculateAlloc5 == busConstant.FLAG_YES && aobjBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinIAP == "Y")
                        {
                            if (lintComputationYear == 1996)
                            {
                                ldecAlloc5AfflAmount = lobjIAPAllocationHelper.CalcuateAllocation5AffliatesAmount(lintComputationYear, aclbPersonWorkHistory_IAP, lblnAgeFlag);
                            }
                            else
                            {
                                if (lobjIAPAllocationHelper.CheckParticipantIsAffiliate(lintComputationYear, aobjBenefitApplication.ibusPerson.icdoPerson.istrSSNNonEncrypted))
                                    ldecAlloc5AfflAmount = lobjIAPAllocationHelper.CalcuateAllocation5AffliatesAmount(lintComputationYear, aclbPersonWorkHistory_IAP, lblnAgeFlag);
                                else
                                    ldecAlloc5NonAfflAmount = lobjIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(lintComputationYear, ldecTotalYTDHours, busConstant.IAPAllocationNonAffiliatesFlag);
                                ldecAlloc5BothAmount = lobjIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(lintComputationYear, ldecTotalYTDHours, busConstant.IAPAllocationBothAffAndNonAffFlag);
                            }
                        }
                    }
                    // }
                }
                else if (iblnReemployed && lcdoIAPHours.qualified_hours >= 870)
                {
                    // PROD PIR 544
                    if ((lcdoIAPHours.year == idtRetirementDate.Year || (iblnDead && lcdoIAPHours.year == aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year))
                        && (ibusAllocationSummary.icdoIapAllocationSummary.computation_year < idtRetirementDate.Year ||
                                 (iblnDead && ibusAllocationSummary.icdoIapAllocationSummary.computation_year < aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year)))
                    {
                        ldecAlloc2InvstAmount = 0.00M; ldecAlloc2FrftAmount = 0.00M; ldecAlloc4InvstAmount = 0.00M; ldecAlloc4FrftAmount = 0.00M;
                    }
                    //change 01302014
                    else if (ibusAllocationSummary.icdoIapAllocationSummary.computation_year >= lintComputationYear)
                    {

                        //method to calculate allocation 2 amount
                        ldecAlloc2Amount = lobjIAPAllocationHelper.CalculateAllocation2Amount(lintComputationYear, ldecThru79Hours, ldecTotalYTDHours, DateTime.MinValue,
                                                                                DateTime.MinValue, DateTime.MinValue);
                        //method to calculate allocation 2 investment amount
                        ldecAlloc2InvstAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
                                                                                DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationInvestmentFlag);
                        //method to calculate allocation 2 forfeiture amount
                        ldecAlloc2FrftAmount = lobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
                                                                                DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationForfeitureFlag);

                        var ldtIAPPercent = from DataRow row in ldtIAPHoursFiltered.AsEnumerable()
                                            group row by new { ID = row.Field<string>("OldEmployerNum") } into g
                                            select new
                                            {
                                                iappercent = g.Sum(row => row["iappercent"] == DBNull.Value ? 0.0M : (decimal)row["iappercent"])
                                            };

                        foreach (var ldrIAPPercent in ldtIAPPercent)
                        {
                            ldecAllocation4Amount += lobjIAPAllocationHelper.CalculateAllocation4Amount(lintComputationYear, null, ldrIAPPercent.iappercent);
                        }

                        //method to calculate allocation 4 investment amount
                        ldecAlloc4InvstAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationInvestmentFlag);
                        //method to calculate allocation 4 forfeiture amount
                        ldecAlloc4FrftAmount = lobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationForfeitureFlag);
                    }
                }
               

                //PIR 623
                if (lcdoIAPHours.year < 2000 && (lcdoIAPHours.year == idtRetirementDate.Year || (iblnDead && lcdoIAPHours.year == aobjBenefitApplication.ibusPerson.icdoPerson.date_of_death.Year) || lcdoIAPHours.year == idtMDdate.Year))
                {
                    ldecAllocation4Amount = 0.00M;
                }

                decimal ldecNSRecords = 0M;
                //LA Sunset - Release 3
                if (iclbNSRecordsIAPAllocationDetailPersonOverview != null &&
                    iclbNSRecordsIAPAllocationDetailPersonOverview.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).Count() > 0)
                {
                    ldecNSRecords = iclbNSRecordsIAPAllocationDetailPersonOverview.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).Sum(t => t.icdoIapallocationDetailPersonoverview.alloc4);
                    ldecAllocation4Amount += ldecNSRecords;
                }

                ldecIAPAccountBalance += ldecAlloc1Amount + ldecAlloc2Amount + ldecAlloc2InvstAmount + ldecAlloc2FrftAmount + ldecAlloc3Amount + ldecAllocation4Amount +
                                                ldecAlloc4InvstAmount + ldecAlloc4FrftAmount + ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount;

                decimal ldecTotal = ldecAlloc1Amount + ldecAlloc2Amount + ldecAlloc2InvstAmount + ldecAlloc2FrftAmount + ldecAlloc3Amount + ldecAllocation4Amount +
                                                ldecAlloc4InvstAmount + ldecAlloc4FrftAmount + ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount;

                decimal ldecAlloc5Amount = (ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount);
                lstrForfietureFlag = lcdoIAPHours.istrForfietureFlag;

                lintPreviousYear = lintComputationYear;


                FillIAPAllocationDetails(this.icdoPerson.ssn, "Computed", lintComputationYear, ldecAlloc1Amount, ldecAlloc2Amount, ldecAlloc2InvstAmount, ldecAlloc2FrftAmount, ldecAlloc3Amount,
                    ldecAllocation4Amount, ldecAlloc4InvstAmount, ldecAlloc4FrftAmount, ldecAlloc5Amount, ldecTotal, iblnReemployed, lcdoIAPHours.qualified_hours, lcdoIAPHours.IAP_HOURSA2, lstrCalculateAlloc5, lstrForfietureFlag);
            }

            if (iblnParticipant && iblnActive)
                PostAllocation1And5ForRemainingYears(lintPreviousYear, ldecIAPAccountBalance, adtIAPAllContributions, lobjIAPAllocationHelper, aobjBenefitApplication, aclbPersonWorkHistory_IAP,
                    lblnAgeFlag, aobjBenefitApplication.icdoBenefitApplication.retirement_date, iclbIapAllocation5Recalculation);

        }

        private void PostAllocation1And5ForRemainingYears(int aintPrevComputationYear, decimal adecPrevYearAccountBalance, DataTable adtIAPContributions, busIAPAllocationHelper aobjIAPHelper,
        busBenefitApplication aobjBenefitApplication, Collection<cdoDummyWorkData> aclbPersonWorkHistory_IAP, bool ablnAgeFlag, DateTime aintRetirementDate, Collection<cdoIapAllocation5Recalculation> aclbIapAllocation5Recalculation)
        {

            IEnumerable<DataRow> lenmRemainingContributions = adtIAPContributions.AsEnumerable().Where(o => o.Field<decimal>("computational_year") > Convert.ToDecimal(aintPrevComputationYear));
            decimal ldecAlloc1Amount, ldecAlloc5AfflAmount, ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount, ldecFactor;
            ldecAlloc1Amount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = ldecFactor = 0.00M;

            DateTime lintWithDrawalDate = new DateTime();

            foreach (DataRow ldr in lenmRemainingContributions)
            {
                
                    DataTable ldtbWithdrawalApplicationforQuarterlyAllocation = busBase.Select("cdoBenefitApplication.CheckWithdrawalApplicationforQuarterlyAllocation", new object[2] { aobjBenefitApplication.icdoBenefitApplication.person_id, Convert.ToInt32(ldr["computational_year"]) });

                    if (ldtbWithdrawalApplicationforQuarterlyAllocation.Rows.Count > 0)
                    {
                        lintWithDrawalDate = Convert.ToDateTime(ldtbWithdrawalApplicationforQuarterlyAllocation.Rows[0]["WITHDRAWAL_DATE"]);

                    }
                string lstrCalculateAlloc5 = busConstant.FLAG_NO;
                //method to calculate allocation 1 amount
                if (Convert.ToInt32(ldr["computational_year"]) == lintWithDrawalDate.Year)
                {
                    int lintQuarter = 0;
                    lintQuarter = busGlobalFunctions.GetPreviousQuarter(lintWithDrawalDate);
                    ldecAlloc1Amount = aobjIAPHelper.CalculateAllocation1Amount(Convert.ToInt32(ldr["computational_year"]), adecPrevYearAccountBalance, lintQuarter, ref ldecFactor);
                }
                else if (Convert.ToInt32(ldr["computational_year"]) == aintRetirementDate.Year)
                {
                    int lintQuarter = 0;
                    lintQuarter = busGlobalFunctions.GetPreviousQuarter(aintRetirementDate);
                    ldecAlloc1Amount = aobjIAPHelper.CalculateAllocation1Amount(Convert.ToInt32(ldr["computational_year"]), adecPrevYearAccountBalance, lintQuarter, ref ldecFactor);
                }
                else
                {
                    ldecAlloc1Amount = aobjIAPHelper.CalculateAllocation1Amount(Convert.ToInt32(ldr["computational_year"]), adecPrevYearAccountBalance, 4, ref ldecFactor);
                }

                //Block to calculate allocation 5 amount
                if (Convert.ToInt32(ldr["computational_year"]) >= 1996 && Convert.ToInt32(ldr["computational_year"]) <= 2001)
                {

                    if (aclbIapAllocation5Recalculation != null && aclbIapAllocation5Recalculation.Where(item => item.computational_year == Convert.ToInt32(ldr["computational_year"])).Count() > 0)
                    {
                        lstrCalculateAlloc5 = aclbIapAllocation5Recalculation.Where(item => item.computational_year == Convert.ToInt32(ldr["computational_year"])).FirstOrDefault().iap_allocation5_recalculate_flag;
                    }
                    else if (Convert.ToDecimal(ldr["alloc4"]) != 0.00M)
                    {
                        lstrCalculateAlloc5 = busConstant.FLAG_YES;
                    }

                    if (lstrCalculateAlloc5 == busConstant.FLAG_YES)
                    {
                        decimal ldecYTDHours = aclbPersonWorkHistory_IAP.Where(o => o.year == Convert.ToInt32(ldr["computational_year"])).Sum(o => o.qualified_hours);

                        if (aobjIAPHelper.CheckParticipantIsAffiliate(Convert.ToInt32(ldr["computational_year"]), aobjBenefitApplication.ibusPerson.icdoPerson.istrSSNNonEncrypted))
                            ldecAlloc5AfflAmount = aobjIAPHelper.CalcuateAllocation5AffliatesAmount(Convert.ToInt32(ldr["computational_year"]), aclbPersonWorkHistory_IAP, ablnAgeFlag);
                        else
                            ldecAlloc5NonAfflAmount = aobjIAPHelper.CalcuateAllocation5NonAffOrBothAmount(Convert.ToInt32(ldr["computational_year"]), ldecYTDHours, busConstant.IAPAllocationNonAffiliatesFlag);
                        ldecAlloc5BothAmount = aobjIAPHelper.CalcuateAllocation5NonAffOrBothAmount(Convert.ToInt32(ldr["computational_year"]), ldecYTDHours, busConstant.IAPAllocationBothAffAndNonAffFlag);
                    }
                }
                //method to post the difference amount into contribution table
                //updating iap account balance

                decimal ldecQualifiedHours = 0M;
                decimal ldecIAPHoursA2 = 0M;
                if (aclbPersonWorkHistory_IAP != null && aclbPersonWorkHistory_IAP.Where(t => t.year == Convert.ToInt32(ldr["computational_year"])).Count() > 0)
                {
                    ldecQualifiedHours = aclbPersonWorkHistory_IAP.Where(t => t.year == Convert.ToInt32(ldr["computational_year"])).FirstOrDefault().qualified_hours;
                    ldecIAPHoursA2 = aclbPersonWorkHistory_IAP.Where(t => t.year == Convert.ToInt32(ldr["computational_year"])).FirstOrDefault().IAP_HOURSA2;
                }

                decimal ldecTotal = (ldecAlloc1Amount + (ldr["alloc2"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2"]) : 0.0M) +
                    (ldr["alloc2_invt"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2_invt"]) : 0.0M) + (ldr["alloc2_frft"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2_frft"]) : 0.0M) +
                        (ldr["alloc3"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc3"]) : 0.0M) + (ldr["alloc4"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4"]) : 0.0M) +
                    (ldr["alloc4_invt"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4_invt"]) : 0.0M) +
                        (ldr["alloc4_frft"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4_frft"]) : 0.0M) + ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount);

                adecPrevYearAccountBalance += (ldecAlloc1Amount + (ldr["alloc2"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2"]) : 0.0M) +
                    (ldr["alloc2_invt"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2_invt"]) : 0.0M) + (ldr["alloc2_frft"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc2_frft"]) : 0.0M) +
                        (ldr["alloc3"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc3"]) : 0.0M) + (ldr["alloc4"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4"]) : 0.0M) +
                    (ldr["alloc4_invt"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4_invt"]) : 0.0M) +
                        (ldr["alloc4_frft"] != DBNull.Value ? Convert.ToDecimal(ldr["alloc4_frft"]) : 0.0M) + ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount);


                FillIAPAllocationDetails(this.icdoPerson.ssn, "Computed", Convert.ToInt32(ldr["computational_year"]), ldecAlloc1Amount, 0M, 0M, 0M, 0M,
                   0M, 0M, 0M, (ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount), ldecTotal, iblnReemployed, ldecQualifiedHours, ldecIAPHoursA2, lstrCalculateAlloc5);


                ldecAlloc1Amount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = 0.00M;
            }
        }


        public void GetDifferenceIAPAllocationDetailsReemployment()
        {
            ArrayList larrComputationYear = new ArrayList();
            foreach (busIapAllocationDetailCalculation lcdoIapallocationDetailPersonoverview in iclbIAPAllocationDetailPersonOverview)
            {
                if (!larrComputationYear.Contains(lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year) && lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                    larrComputationYear.Add(lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year);

                //lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecTotal = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc1 + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2 + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2_frft
                //    + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2_invt + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc3 + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4 +
                //    lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4_frft + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4_invt + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc5;
            }


            foreach (decimal lintComputationYear in larrComputationYear)
            {

                decimal ldecNewAllocation4Amount = 0M, ldecNewAlloc1Amount = 0M, ldecNewAlloc2Amount = 0M, ldecNewAlloc2InvstAmount = 0M, ldecNewAlloc2FrftAmount = 0M, ldecNewAlloc3Amount = 0M, ldecNewAlloc4InvstAmount = 0M, ldecNewAlloc4FrftAmount = 0M, ldecNewAlloc5Amount = 0M, ldecNewAlloc5AfflAmount = 0M, ldecNewAlloc5NonAfflAmount = 0M, ldecNewAlloc5BothAmount = 0M, ldecNewTotal = 0M;
                decimal ldecOldAllocation4Amount = 0M, ldecOldAlloc1Amount = 0M, ldecOldAlloc2Amount = 0M, ldecOldAlloc2InvstAmount = 0M, ldecOldAlloc2FrftAmount = 0M, ldecOldAlloc3Amount = 0M, ldecOldAlloc4InvstAmount = 0M, ldecOldAlloc4FrftAmount = 0M, ldecOldAlloc5Amount = 0M, ldecOldAlloc5AfflAmount = 0M, ldecOldAlloc5NonAfflAmount = 0M, ldecOldAlloc5BothAmount = 0M, ldecOldTotal = 0M;


                if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear
                    && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).Count() > 0)
                {
                  
                     ldecNewAlloc1Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                    .icdoIapallocationDetailPersonoverview.alloc1;
                    

                    ldecNewAlloc2Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2;

                    ldecNewAlloc2InvstAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2_invt;

                    ldecNewAlloc2FrftAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2_frft;

                    ldecNewAlloc3Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.alloc3;

                    ldecNewAllocation4Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4;

                    ldecNewAlloc4InvstAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4_invt;

                    ldecNewAlloc4FrftAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4_frft;

                    ldecNewAlloc5AfflAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                    .icdoIapallocationDetailPersonoverview.alloc5_affl;

                    ldecNewAlloc5NonAfflAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                    .icdoIapallocationDetailPersonoverview.alloc5_nonaffl;

                    ldecNewAlloc5BothAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                    .icdoIapallocationDetailPersonoverview.alloc5_both;

                    ldecNewAlloc5Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.alloc5;
                   
                        ldecNewTotal = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.idecTotal;
                   
                }

                if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).Count() > 0)
                {

                    
                        ldecOldAlloc1Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc1;
                  

                    ldecOldAlloc2Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2;

                    ldecOldAlloc2InvstAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2_invt;

                    ldecOldAlloc2FrftAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2_frft;

                    ldecOldAlloc3Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.alloc3;

                    ldecOldAllocation4Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4;

                    ldecOldAlloc4InvstAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4_invt;

                    ldecOldAlloc4FrftAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4_frft;

                    ldecOldAlloc5AfflAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                  .icdoIapallocationDetailPersonoverview.alloc5_affl;

                    ldecOldAlloc5NonAfflAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                    .icdoIapallocationDetailPersonoverview.alloc5_nonaffl;

                    ldecOldAlloc5BothAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                    .icdoIapallocationDetailPersonoverview.alloc5_both;

                    ldecOldAlloc5Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.alloc5;
                  
                        ldecOldTotal = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.idecTotal;
                   
                }


                FillIAPAllocationDetails(icdoPerson.ssn, "Difference", lintComputationYear, ldecNewAlloc1Amount - ldecOldAlloc1Amount, ldecNewAlloc2Amount - ldecOldAlloc2Amount, ldecNewAlloc2InvstAmount - ldecOldAlloc2InvstAmount,
                    ldecNewAlloc2FrftAmount - ldecOldAlloc2FrftAmount, ldecNewAlloc3Amount - ldecOldAlloc3Amount, ldecNewAllocation4Amount - ldecOldAllocation4Amount,
                    ldecNewAlloc4InvstAmount - ldecOldAlloc4InvstAmount, ldecNewAlloc4FrftAmount - ldecOldAlloc4FrftAmount, ldecNewAlloc5Amount - ldecOldAlloc5Amount, ldecNewTotal - ldecOldTotal, ablnReemploymentFlag: true);
            }

            if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Count() > 0)
            {
                foreach (busIapAllocationDetailCalculation lcdoIapallocationDetailPersonoverview in iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag))
                {
                    if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).Count() > 0)
                    {
                        iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                            .FirstOrDefault().icdoIapallocationDetailPersonoverview.ytdhours = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.ytdhours;
                        iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                          .FirstOrDefault().icdoIapallocationDetailPersonoverview.idecYTDHoursA2 = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecYTDHoursA2;
                    }
                    if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).Count() > 0)
                    {
                            iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                                .FirstOrDefault().icdoIapallocationDetailPersonoverview.ytdhours = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.ytdhours;
                            iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                              .FirstOrDefault().icdoIapallocationDetailPersonoverview.idecYTDHoursA2 = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecYTDHoursA2;
                    }
                }
            }

            //iclbIAPAllocationDetailPersonOverview = iclbIAPAllocationDetailPersonOverview.OrderBy(item => item.icdoIapallocationDetailPersonoverview.computational_year).ThenBy(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" ? 1 : item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" ? 2 : 3).ToList().ToCollection<busIapAllocationDetailCalculation>();

        }

        public void GetDifferenceIAPAllocationDetails()
        {
            ArrayList larrComputationYear = new ArrayList();
            foreach (busIapAllocationDetailCalculation lcdoIapallocationDetailPersonoverview in iclbIAPAllocationDetailPersonOverview)
            {
                if (!larrComputationYear.Contains(lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year) && !lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                    larrComputationYear.Add(lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year);

                lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecTotal = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc1 + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2 + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2_frft
                    + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2_invt + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc3 + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4 +
                    lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4_frft + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4_invt + lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc5;
            }


            foreach (decimal lintComputationYear in larrComputationYear)
            {

                decimal ldecNewAllocation4Amount = 0M, ldecNewAlloc1Amount = 0M, ldecNewAlloc2Amount = 0M, ldecNewAlloc2InvstAmount = 0M, ldecNewAlloc2FrftAmount = 0M, ldecNewAlloc3Amount = 0M, ldecNewAlloc4InvstAmount = 0M, ldecNewAlloc4FrftAmount = 0M, ldecNewAlloc5Amount = 0M, ldecNewAlloc5AfflAmount = 0M, ldecNewAlloc5NonAfflAmount = 0M, ldecNewAlloc5BothAmount = 0M, ldecNewTotal = 0M;
                decimal ldecOldAllocation4Amount = 0M, ldecOldAlloc1Amount = 0M, ldecOldAlloc2Amount = 0M, ldecOldAlloc2InvstAmount = 0M, ldecOldAlloc2FrftAmount = 0M, ldecOldAlloc3Amount = 0M, ldecOldAlloc4InvstAmount = 0M, ldecOldAlloc4FrftAmount = 0M, ldecOldAlloc5Amount = 0M, ldecOldAlloc5AfflAmount = 0M, ldecOldAlloc5NonAfflAmount = 0M, ldecOldAlloc5BothAmount = 0M, ldecOldTotal = 0M;


                if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear
                    && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).Count() > 0)
                {
                   
                  ldecNewAlloc1Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                  .icdoIapallocationDetailPersonoverview.alloc1;
                   

                    ldecNewAlloc2Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2;

                    ldecNewAlloc2InvstAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2_invt;

                    ldecNewAlloc2FrftAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2_frft;

                    ldecNewAlloc3Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.alloc3;

                    ldecNewAllocation4Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4;

                    ldecNewAlloc4InvstAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4_invt;

                    ldecNewAlloc4FrftAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4_frft;

                    ldecNewAlloc5AfflAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                    .icdoIapallocationDetailPersonoverview.alloc5_affl;

                    ldecNewAlloc5NonAfflAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                    .icdoIapallocationDetailPersonoverview.alloc5_nonaffl;

                    ldecNewAlloc5BothAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                    .icdoIapallocationDetailPersonoverview.alloc5_both;

                    ldecNewAlloc5Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.alloc5;
                   
                        ldecNewTotal = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.idecTotal;
                   
                }

                if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).Count() > 0)
                {
                  
                        ldecOldAlloc1Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc1;
                   

                    ldecOldAlloc2Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2;

                    ldecOldAlloc2InvstAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2_invt;

                    ldecOldAlloc2FrftAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc2_frft;

                    ldecOldAlloc3Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.alloc3;

                    ldecOldAllocation4Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4;

                    ldecOldAlloc4InvstAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4_invt;

                    ldecOldAlloc4FrftAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4_frft;

                    ldecOldAlloc5AfflAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                  .icdoIapallocationDetailPersonoverview.alloc5_affl;

                    ldecOldAlloc5NonAfflAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                    .icdoIapallocationDetailPersonoverview.alloc5_nonaffl;

                    ldecOldAlloc5BothAmount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                    .icdoIapallocationDetailPersonoverview.alloc5_both;

                    ldecOldAlloc5Amount = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.alloc5;
                   
                        ldecOldTotal = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.idecTotal;
                   
                }

              
                    FillIAPAllocationDetails(icdoPerson.ssn, "Difference", lintComputationYear, ldecNewAlloc1Amount - ldecOldAlloc1Amount, ldecNewAlloc2Amount - ldecOldAlloc2Amount, ldecNewAlloc2InvstAmount - ldecOldAlloc2InvstAmount,
                    ldecNewAlloc2FrftAmount - ldecOldAlloc2FrftAmount, ldecNewAlloc3Amount - ldecOldAlloc3Amount, ldecNewAllocation4Amount - ldecOldAllocation4Amount,
                    ldecNewAlloc4InvstAmount - ldecOldAlloc4InvstAmount, ldecNewAlloc4FrftAmount - ldecOldAlloc4FrftAmount, ldecNewAlloc5Amount - ldecOldAlloc5Amount, ldecNewTotal - ldecOldTotal, ablnReemploymentFlag: false);
               
            }

            if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Count() > 0)
            {
                foreach (busIapAllocationDetailCalculation lcdoIapallocationDetailPersonoverview in iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag))
                {
                    if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).Count() > 0)
                    {
                        iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                            .FirstOrDefault().icdoIapallocationDetailPersonoverview.ytdhours = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.ytdhours;
                        iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                          .FirstOrDefault().icdoIapallocationDetailPersonoverview.idecYTDHoursA2 = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecYTDHoursA2;
                    }
                  
                        if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag).Count() > 0)
                        {
                            iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                                .FirstOrDefault().icdoIapallocationDetailPersonoverview.ytdhours = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.ytdhours;
                            iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year && !item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                              .FirstOrDefault().icdoIapallocationDetailPersonoverview.idecYTDHoursA2 = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecYTDHoursA2;
                        }
                   
                }
            }

            foreach (busIapAllocationDetailCalculation lbusIapAllocationDetailCalculation in iclbPaidIAPAllocationDetailPersonOverview)
            {
                //  IAP Enhancements #14 : Special Account Overpayment
                if (lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.record_freeze_flag == "Y")
                {
                    lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSource = "Adjusted Balance";
                    lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSSN = iclbIAPAllocationDetailPersonOverview.FirstOrDefault().icdoIapallocationDetailPersonoverview.istrSSN;
                    lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.idecTotal = lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc1 +
                        lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc2 + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc2_frft + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc2_invt
                        + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc3 + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc4 +
                        lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc4_frft + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc4_invt + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc5;


                }
                else
                {
                    lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSource = "Withdrawal";
                    lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrSSN = iclbIAPAllocationDetailPersonOverview.FirstOrDefault().icdoIapallocationDetailPersonoverview.istrSSN;
                    lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.idecTotal = lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc1 +
                        lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc2 + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc2_frft + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc2_invt
                        + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc3 + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc4 +
                        lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc4_frft + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc4_invt + lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.alloc5;


                }
                iclbIAPAllocationDetailPersonOverview.Add(lbusIapAllocationDetailCalculation);

                lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.idecTotal = lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.idecTotal;
            }
            //  IAP Enhancements #14 : Special Account Overpayment
            iclbIAPAllocationDetailPersonOverview = iclbIAPAllocationDetailPersonOverview.OrderBy(item => item.icdoIapallocationDetailPersonoverview.computational_year).ThenBy(item => item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag == false ? 1 : 2).
                ThenBy(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" ? 1 : item.icdoIapallocationDetailPersonoverview.istrSource == "Withdrawal" ? 2 : item.icdoIapallocationDetailPersonoverview.istrSource == "Adjusted Balance" ? 3 : item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" ? 4 : 5).ToList().ToCollection<busIapAllocationDetailCalculation>();
            
                LoadTotalIAPAllocationDetails();
        
        }


        public void LoadTotalIAPAllocationDetails()
        {
            decimal ldecForfietureYear = 0;

            if (iclbTotalIAPAllocationDetailPersonOverview == null)
                iclbTotalIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();

            decimal ldecAllocation4AmountComputed = 0M, ldecAlloc1AmountComputed = 0M, ldecAlloc2AmountComputed = 0M, ldecAlloc2InvstAmountComputed = 0M, ldecAlloc2FrftAmountComputed = 0M, ldecAlloc3AmountComputed = 0M, ldecAlloc4InvstAmountComputed = 0M, ldecAlloc4FrftAmountComputed = 0M, ldecAlloc5AmountComputed = 0M, ldecTotalComputed = 0M;
            if (iclbIAPAllocationDetailPersonOverview != null && iclbIAPAllocationDetailPersonOverview.Count > 0
                && iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Count() > 0)
            {



                if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.istrForfietureFlag == busConstant.FLAG_YES).Count() > 0)
                {
                    ldecForfietureYear = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed"
                        && item.icdoIapallocationDetailPersonoverview.istrForfietureFlag == busConstant.FLAG_YES).Max(t => t.icdoIapallocationDetailPersonoverview.computational_year);
                }


                if (ldecForfietureYear == 0)
                {
                    ldecAlloc1AmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1);
                    ldecAlloc2AmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2);
                    ldecAlloc2InvstAmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_invt);
                    ldecAlloc2FrftAmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_frft);
                    ldecAlloc3AmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc3);
                    ldecAllocation4AmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4);
                    ldecAlloc4InvstAmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_invt);
                    ldecAlloc4FrftAmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_frft);
                    ldecAlloc5AmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc5);
                    ldecTotalComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.idecTotal);

                }
                else
                {
                    ldecAlloc1AmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1);
                    ldecAlloc2AmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2);
                    ldecAlloc2InvstAmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_invt);
                    ldecAlloc2FrftAmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_frft);
                    ldecAlloc3AmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc3);
                    ldecAllocation4AmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4);
                    ldecAlloc4InvstAmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_invt);
                    ldecAlloc4FrftAmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_frft);
                    ldecAlloc5AmountComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc5);
                    ldecTotalComputed = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.idecTotal);

                }


                if (ldecForfietureYear == 0)
                {
                    //PIR 985
                    //foreach (busIapAllocationDetailCalculation lbusIapAllocationDetailCalculation in iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed"))
                    //{
                    if (iclbPaidIAPAllocationDetailPersonOverview != null && iclbPaidIAPAllocationDetailPersonOverview.Count > 0)
                    {
                        foreach (busIapAllocationDetailCalculation lbusPaidIAPAllocationDetailPersonOverview in iclbPaidIAPAllocationDetailPersonOverview)
                        {
                            //if (lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.computational_year == lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.computational_year)
                            //{
                            if(lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.record_freeze_flag != "Y")
                            {
                                ldecAlloc1AmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc1;

                            }
                           
                            ldecAlloc2AmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc2;
                            ldecAlloc2InvstAmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc2_invt;
                            ldecAlloc2FrftAmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc2_frft;
                            ldecAlloc3AmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc3;
                            ldecAllocation4AmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc4;
                            ldecAlloc4InvstAmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc4_invt;
                            ldecAlloc4FrftAmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc4_frft;
                            ldecAlloc5AmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc5;
                            ldecTotalComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.idecTotal;
                            //}
                        }
                    }

                    //}
                }
                else
                {
                    //PIR 985
                    //foreach (busIapAllocationDetailCalculation lbusIapAllocationDetailCalculation in iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear))
                    //{

                    if (iclbPaidIAPAllocationDetailPersonOverview != null && iclbPaidIAPAllocationDetailPersonOverview.Count > 0)
                    {
                        foreach (busIapAllocationDetailCalculation lbusPaidIAPAllocationDetailPersonOverview in iclbPaidIAPAllocationDetailPersonOverview)
                        {
                            if (lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear)
                            {
                                ldecAlloc1AmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc1;
                                ldecAlloc2AmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc2;
                                ldecAlloc2InvstAmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc2_invt;
                                ldecAlloc2FrftAmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc2_frft;
                                ldecAlloc3AmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc3;
                                ldecAllocation4AmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc4;
                                ldecAlloc4InvstAmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc4_invt;
                                ldecAlloc4FrftAmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc4_frft;
                                ldecAlloc5AmountComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc5;
                                ldecTotalComputed += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.idecTotal;
                            }
                        }
                    }

                    // }
                }
               
                    FillTotalIAPAllocationDetails(icdoPerson.ssn, "Computed", 0, ldecAlloc1AmountComputed, ldecAlloc2AmountComputed, ldecAlloc2InvstAmountComputed, ldecAlloc2FrftAmountComputed,
                    ldecAlloc3AmountComputed, ldecAllocation4AmountComputed, ldecAlloc4InvstAmountComputed, ldecAlloc4FrftAmountComputed, ldecAlloc5AmountComputed, ldecTotalComputed);
               
            }




            if (iclbIAPAllocationDetailPersonOverview != null && iclbIAPAllocationDetailPersonOverview.Count > 0
                    && iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Count() > 0)
            {
                decimal ldecAllocation4AmountOpus = 0M, ldecAlloc1AmountOpus = 0M, ldecAlloc2AmountOpus = 0M, ldecAlloc2InvstAmountOpus = 0M, ldecAlloc2FrftAmountOpus = 0M, ldecAlloc3AmountOpus = 0M, ldecAlloc4InvstAmountOpus = 0M, ldecAlloc4FrftAmountOpus = 0M, ldecAlloc5AmountOpus = 0M, ldecTotalOpus = 0M;
                ldecAlloc1AmountOpus = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1);
                ldecAlloc2AmountOpus = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2);
                ldecAlloc2InvstAmountOpus = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_invt);
                ldecAlloc2FrftAmountOpus = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_frft);
                ldecAlloc3AmountOpus = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc3);
                ldecAllocation4AmountOpus = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4);
                ldecAlloc4InvstAmountOpus = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_invt);
                ldecAlloc4FrftAmountOpus = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_frft);
                ldecAlloc5AmountOpus = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc5);
                ldecTotalOpus = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.idecTotal);


                foreach (busIapAllocationDetailCalculation lbusIapAllocationDetailCalculation in iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS"))
                {
                    if (iclbPaidIAPAllocationDetailPersonOverview != null && iclbPaidIAPAllocationDetailPersonOverview.Count > 0)
                    {
                        foreach (busIapAllocationDetailCalculation lbusPaidIAPAllocationDetailPersonOverview in iclbPaidIAPAllocationDetailPersonOverview)
                        {
                            if (lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.computational_year == lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.computational_year)
                            {
                                ldecAlloc1AmountOpus += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc1;
                                ldecAlloc2AmountOpus += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc2;
                                ldecAlloc2InvstAmountOpus += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc2_invt;
                                ldecAlloc2FrftAmountOpus += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc2_frft;
                                ldecAlloc3AmountOpus += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc3;
                                ldecAllocation4AmountOpus += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc4;
                                ldecAlloc4InvstAmountOpus += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc4_invt;
                                ldecAlloc4FrftAmountOpus += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc4_frft;
                                ldecAlloc5AmountOpus += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc5;
                                ldecTotalOpus += lbusPaidIAPAllocationDetailPersonOverview.icdoIapallocationDetailPersonoverview.idecTotal;
                            }
                        }
                    }

                }



                FillTotalIAPAllocationDetails(icdoPerson.ssn, "OPUS", 0, ldecAlloc1AmountOpus, ldecAlloc2AmountOpus, ldecAlloc2InvstAmountOpus, ldecAlloc2FrftAmountOpus,
                    ldecAlloc3AmountOpus, ldecAllocation4AmountOpus, ldecAlloc4InvstAmountOpus, ldecAlloc4FrftAmountOpus, ldecAlloc5AmountOpus, ldecTotalOpus);
            }


            if (iclbIAPAllocationDetailPersonOverview != null && iclbIAPAllocationDetailPersonOverview.Count > 0
                  && iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Count() > 0)
            {
                decimal ldecAllocation4AmountDiff = 0M, ldecAlloc1AmountDiff = 0M, ldecAlloc2AmountDiff = 0M, ldecAlloc2InvstAmountDiff = 0M, ldecAlloc2FrftAmountDiff = 0M, ldecAlloc3AmountDiff = 0M, ldecAlloc4InvstAmountDiff = 0M, ldecAlloc4FrftAmountDiff = 0M, ldecAlloc5AmountDiff = 0M, ldecTotalDiff = 0M;

                if (ldecForfietureYear == 0)
                {
                    ldecAlloc1AmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1);
                    ldecAlloc2AmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2);
                    ldecAlloc2InvstAmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_invt);
                    ldecAlloc2FrftAmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_frft);
                    ldecAlloc3AmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc3);
                    ldecAllocation4AmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4);
                    ldecAlloc4InvstAmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_invt);
                    ldecAlloc4FrftAmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_frft);
                    ldecAlloc5AmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc5);
                    ldecTotalDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.idecTotal);
                    //  IAP Enhancements #14 : Special Account Overpayment
                    //if (iclbIAPAllocationDetailPersonOverview.Where(i => i.icdoIapallocationDetailPersonoverview.record_freeze_flag != "Y").IsNullOrEmpty())
                    //{
                    //    ldecTotalDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.idecTotal);


                    //}
                }
                else
                {
                    ldecAlloc1AmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1);
                    ldecAlloc2AmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2);
                    ldecAlloc2InvstAmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_invt);
                    ldecAlloc2FrftAmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_frft);
                    ldecAlloc3AmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc3);
                    ldecAllocation4AmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4);
                    ldecAlloc4InvstAmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_invt);
                    ldecAlloc4FrftAmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_frft);
                    ldecAlloc5AmountDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.alloc5);
                    ldecTotalDiff = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && item.icdoIapallocationDetailPersonoverview.computational_year > ldecForfietureYear).Sum(item => item.icdoIapallocationDetailPersonoverview.idecTotal);

                }
                
                    FillTotalIAPAllocationDetails(icdoPerson.ssn, "Difference", 0, ldecAlloc1AmountDiff, ldecAlloc2AmountDiff, ldecAlloc2InvstAmountDiff, ldecAlloc2FrftAmountDiff,
                    ldecAlloc3AmountDiff, ldecAllocation4AmountDiff, ldecAlloc4InvstAmountDiff, ldecAlloc4FrftAmountDiff, ldecAlloc5AmountDiff, ldecTotalDiff);
              
            }
        }

        public void FillIAPAllocationDetails(string istrSSN, string istrSource, decimal aintComputationYear, decimal adecDiffAlloc1,
             decimal ldecAlloc2Amount, decimal ldecAlloc2InvstAmount, decimal ldecAlloc2FrftAmount, decimal ldecAlloc3Amount, decimal ldecAllocation4Amount,
            decimal ldecAlloc4InvstAmount, decimal ldecAlloc4FrftAmount, decimal ldecAlloc5Amount, decimal adecTotal, bool ablnReemploymentFlag, decimal ldecYTDHours = 0M, decimal ldecYTDHoursA2 = 0M, string astrCalculateAlloc5 = "", string astrForfietureFlag = "")
        {
            busIapAllocationDetailCalculation lcdoIapallocationDetailPersonoverview = new busIapAllocationDetailCalculation { icdoIapallocationDetailPersonoverview = new cdoIapallocationDetailPersonoverview() };
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrSSN = istrSSN;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrSource = istrSource;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year = aintComputationYear;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc1 = adecDiffAlloc1;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2 = ldecAlloc2Amount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2_invt = ldecAlloc2InvstAmount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2_frft = ldecAlloc2FrftAmount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc3 = ldecAlloc3Amount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4 = ldecAllocation4Amount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4_invt = ldecAlloc4InvstAmount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4_frft = ldecAlloc4FrftAmount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc5 = ldecAlloc5Amount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecTotal = adecTotal;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.ytdhours = ldecYTDHours;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecYTDHoursA2 = ldecYTDHoursA2;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrCalculateAlloc5 = astrCalculateAlloc5;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrForfietureFlag = astrForfietureFlag;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag = ablnReemploymentFlag;
            iclbIAPAllocationDetailPersonOverview.Add(lcdoIapallocationDetailPersonoverview);
        }

        public void FillTotalIAPAllocationDetails(string istrSSN, string istrSource, decimal aintComputationYear, decimal adecDiffAlloc1,
           decimal ldecAlloc2Amount, decimal ldecAlloc2InvstAmount, decimal ldecAlloc2FrftAmount, decimal ldecAlloc3Amount, decimal ldecAllocation4Amount,
          decimal ldecAlloc4InvstAmount, decimal ldecAlloc4FrftAmount, decimal ldecAlloc5Amount, decimal adecTotal)
        {
            busIapAllocationDetailCalculation lcdoIapallocationDetailPersonoverview = new busIapAllocationDetailCalculation { icdoIapallocationDetailPersonoverview = new cdoIapallocationDetailPersonoverview() };
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrSSN = istrSSN;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrSource = istrSource;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year = aintComputationYear;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc1 = adecDiffAlloc1;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2 = ldecAlloc2Amount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2_invt = ldecAlloc2InvstAmount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2_frft = ldecAlloc2FrftAmount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc3 = ldecAlloc3Amount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4 = ldecAllocation4Amount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4_invt = ldecAlloc4InvstAmount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4_frft = ldecAlloc4FrftAmount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc5 = ldecAlloc5Amount;
            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecTotal = adecTotal;

            iclbTotalIAPAllocationDetailPersonOverview.Add(lcdoIapallocationDetailPersonoverview);
        }

        private DataTable LoadIAPContributions(int aintPersonAccountID, int aintComputationYear)
        {
            return busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationsForPersonAccount", new object[2] { aintPersonAccountID, aintComputationYear });
        }


        public ArrayList btn_RefreshAllocations()
        {
            ArrayList iarrError = new ArrayList();
            iarrAlloc5Years = new ArrayList();

            #region Get Years For which Allocation 5 should get Calculated
            if (iclbIAPAllocationDetailPersonOverview != null && iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year >= 1996
                && item.icdoIapallocationDetailPersonoverview.computational_year <= 2001 && item.icdoIapallocationDetailPersonoverview.istrCalculateAlloc5 == busConstant.FLAG_YES).Count() > 0)
            {
                foreach (busIapAllocationDetailCalculation lcdoIapallocationDetailPersonoverview in iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year >= 1996
                && item.icdoIapallocationDetailPersonoverview.computational_year <= 2001 && item.icdoIapallocationDetailPersonoverview.istrCalculateAlloc5 == busConstant.FLAG_YES))
                {
                    iarrAlloc5Years.Add(Convert.ToInt32(lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year));
                }
            }
            #endregion Get Years For which Allocation 5 should get Calculated


            if (ibusPersonAccount != null)
            {
                if (ibusPersonAccount.icdoPersonAccount.recalculate_rule1_flag == "Y")
                {
                    ibusPersonAccount.icdoPersonAccount.recalculate_rule1_flag = "Y";
                    ibusPersonAccount.icdoPersonAccount.modified_by = iobjPassInfo.istrUserID;
                    ibusPersonAccount.icdoPersonAccount.modified_date = DateTime.Now;
                    ibusPersonAccount.icdoPersonAccount.Update();
                }
                else
                {
                    ibusPersonAccount.icdoPersonAccount.recalculate_rule1_flag = "N";
                    ibusPersonAccount.icdoPersonAccount.modified_by = iobjPassInfo.istrUserID;
                    ibusPersonAccount.icdoPersonAccount.modified_date = DateTime.Now;
                    ibusPersonAccount.icdoPersonAccount.Update();
                }

            }

            iclbIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
            iclbTotalIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
            iclbPaidIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
            iclbIapAllocation5Recalculation = new Collection<cdoIapAllocation5Recalculation>();
            iclbNSRecordsIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
            //LA Sunset - Release 3
            LoadIAPNSRecords(lintPersonAccountId);
            LoadPaidIAPAllocationDetailsFromOpus(lintPersonAccountId);
            LoadIAPAllocation5Information(lintPersonAccountId);
            RecalculateIAPAllocationDetails(ablnIsRefreshAllocations: true);
            LoadIAPAllocationDetailsFromOpus(lintPersonAccountId);
            GetDifferenceIAPAllocationDetailsReemployment();
            GetDifferenceIAPAllocationDetails();

            iclbIAPAllocationDetailPersonOverview.ForEach(item => item.EvaluateInitialLoadRules());







            iarrError.Add(this);
            return iarrError;
        }

        public ArrayList btn_ApproveThru79Hours()
        {
            ArrayList iarrError = new ArrayList();

            ibusPersonT79hours.FindPersonT79hours(lintPersonAccountId);

            if (ibusPersonT79hours.icdoPersonT79hours.person_t97_id > 0)
            {
                ibusPersonT79hours.icdoPersonT79hours.approved_flag = busConstant.FLAG_YES;
                ibusPersonT79hours.icdoPersonT79hours.t79_hours = idecThru79Hours;
                ibusPersonT79hours.icdoPersonT79hours.modified_by = iobjPassInfo.istrUserID;
                ibusPersonT79hours.icdoPersonT79hours.modified_date = DateTime.Now;
                ibusPersonT79hours.icdoPersonT79hours.Update();
            }
            else
            {
                ibusPersonT79hours.icdoPersonT79hours.person_account_id = lintPersonAccountId;
                ibusPersonT79hours.icdoPersonT79hours.approved_flag = busConstant.FLAG_YES;
                ibusPersonT79hours.icdoPersonT79hours.t79_hours = idecThru79Hours;
                ibusPersonT79hours.icdoPersonT79hours.Insert();
            }



            return iarrError;
        }


        public ArrayList btn_PostAllocations()
        {
            ArrayList iarrError = new ArrayList();
            utlError lobjError = new utlError();
            decimal ldecOverriddenTotalAmount = 0M;
            ArrayList larrForfeitureYears = new ArrayList();

            if (iclbIAPAllocationDetailPersonOverview != null && iclbIAPAllocationDetailPersonOverview.Count > 0
           && iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year >= 1996 && item.icdoIapallocationDetailPersonoverview.computational_year <= 2001).Count() > 0)
            {
                LoadIAPAllocation5Information(lintPersonAccountId);
                foreach (busIapAllocationDetailCalculation lcdoIapallocationDetailPersonoverview in iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year >= 1996 && item.icdoIapallocationDetailPersonoverview.computational_year <= 2001))
                {
                    if (iclbIapAllocation5Recalculation.Where(item => item.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year).Count() > 0)
                    {
                        cdoIapAllocation5Recalculation lcdoIapAllocation5Recalculation = iclbIapAllocation5Recalculation.Where(item => item.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year).FirstOrDefault();
                        lcdoIapAllocation5Recalculation.iap_allocation5_recalculate_flag = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrCalculateAlloc5;
                        lcdoIapAllocation5Recalculation.Update();
                    }
                    else
                    {
                        cdoIapAllocation5Recalculation lcdoIapAllocation5Recalculation = new cdoIapAllocation5Recalculation();
                        lcdoIapAllocation5Recalculation.person_account_id = lintPersonAccountId;
                        lcdoIapAllocation5Recalculation.computational_year = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year;
                        lcdoIapAllocation5Recalculation.iap_allocation5_recalculate_flag = lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrCalculateAlloc5;
                        lcdoIapAllocation5Recalculation.Insert();
                    }
                }
            }

            #region Code to exclude from late batch run to avoid out of memory exception.
            if (!iblnInLateBatch)
            {
                if (iclbTotalIAPAllocationDetailPersonOverview.Where(x => x.icdoIapallocationDetailPersonoverview.istrSource == "Difference" && (decimal.Round(x.icdoIapallocationDetailPersonoverview.idecTotal, 2, MidpointRounding.AwayFromZero) > 0 || decimal.Round(x.icdoIapallocationDetailPersonoverview.idecTotal, 2, MidpointRounding.AwayFromZero) < 0)).Count() > 0)
                {

                    CreateTableDesignForRecalculateIAPAlloc();
                    CreateTableDesignForRecalculateTotalIAPAlloc();
                    DataSet idtstRecalculateIAPAlloc = new DataSet();

                    if (iclbIAPAllocationDetailPersonOverview.Count > 0)
                    {
                        LoadIAPAllocation5Information(lintPersonAccountId);
                        foreach (var item in iclbIAPAllocationDetailPersonOverview)
                        {
                            DataRow dr = rptRecalculateIAPAlloc.NewRow();

                            dr["Source"] = item.icdoIapallocationDetailPersonoverview.istrSource.ToString();
                            dr["Year"] = item.icdoIapallocationDetailPersonoverview.computational_year;
                            dr["YTDHours"] = item.icdoIapallocationDetailPersonoverview.ytdhours;
                            dr["YTDHoursA2"] = item.icdoIapallocationDetailPersonoverview.idecYTDHoursA2;
                            dr["alloc1"] = item.icdoIapallocationDetailPersonoverview.alloc1;
                            dr["alloc2"] = item.icdoIapallocationDetailPersonoverview.alloc2;
                            dr["alloc2_invt"] = item.icdoIapallocationDetailPersonoverview.alloc2_invt;
                            dr["alloc2_frft"] = item.icdoIapallocationDetailPersonoverview.alloc2_frft;
                            dr["alloc3"] = item.icdoIapallocationDetailPersonoverview.alloc3;
                            dr["alloc4"] = item.icdoIapallocationDetailPersonoverview.alloc4;
                            dr["alloc4_invt"] = item.icdoIapallocationDetailPersonoverview.alloc4_invt;
                            dr["alloc4frft"] = item.icdoIapallocationDetailPersonoverview.alloc4_frft;

                            if (iclbIapAllocation5Recalculation.Count > 0)
                            {
                                foreach (var itm in iclbIapAllocation5Recalculation)
                                {
                                    if (itm.computational_year == item.icdoIapallocationDetailPersonoverview.computational_year)
                                    {
                                        if (item.icdoIapallocationDetailPersonoverview.istrSource == "Computed")
                                        {
                                            dr["Computealloc5"] = itm.iap_allocation5_recalculate_flag;

                                        }

                                    }

                                }

                            }

                            dr["alloc5"] = item.icdoIapallocationDetailPersonoverview.alloc5;
                            dr["total"] = item.icdoIapallocationDetailPersonoverview.idecTotal;

                            rptRecalculateIAPAlloc.Rows.Add(dr);
                        }

                    }

                    if (iclbTotalIAPAllocationDetailPersonOverview.Count > 0)
                    {
                        foreach (var item in iclbTotalIAPAllocationDetailPersonOverview)
                        {
                            DataRow dr = rptRecalculateTotalIAPAlloc.NewRow();

                            dr["TSource"] = item.icdoIapallocationDetailPersonoverview.istrSource.ToString();
                            dr["Talloc1"] = item.icdoIapallocationDetailPersonoverview.alloc1;
                            dr["Talloc2"] = item.icdoIapallocationDetailPersonoverview.alloc2;
                            dr["Talloc2_invt"] = item.icdoIapallocationDetailPersonoverview.alloc2_invt;
                            dr["Talloc2_frft"] = item.icdoIapallocationDetailPersonoverview.alloc2_frft;
                            dr["Talloc3"] = item.icdoIapallocationDetailPersonoverview.alloc3;
                            dr["Talloc4"] = item.icdoIapallocationDetailPersonoverview.alloc4;
                            dr["Talloc4_invt"] = item.icdoIapallocationDetailPersonoverview.alloc4_invt;
                            dr["Talloc4frft"] = item.icdoIapallocationDetailPersonoverview.alloc4_frft;
                            dr["Talloc5"] = item.icdoIapallocationDetailPersonoverview.alloc5;
                            dr["Ttotal"] = item.icdoIapallocationDetailPersonoverview.idecTotal;
                            dr["TOverriddentTotal"] = item.icdoIapallocationDetailPersonoverview.idecOverrideTotal;
                            dr["TMPID"] = icdoPerson.mpi_person_id;
                            dr["TFullName"] = icdoPerson.istrFullName;
                            dr["TSSN"] = icdoPerson.istrLast4DigitsofSSN;
                            dr["TDOB"] = icdoPerson.date_of_birth.Date;
                            dr["THRU79"] = idecThru79Hours;
                            dr["TRecalculateFlag"] = ibusPersonAccount.icdoPersonAccount.recalculate_rule1_flag;


                            rptRecalculateTotalIAPAlloc.Rows.Add(dr);
                        }

                    }


                    idtstRecalculateIAPAlloc.Tables.Add(rptRecalculateIAPAlloc.Copy());
                    idtstRecalculateIAPAlloc.Tables[0].TableName = "ReportTable01";
                    idtstRecalculateIAPAlloc.Tables.Add(rptRecalculateTotalIAPAlloc.Copy());
                    idtstRecalculateIAPAlloc.Tables[1].TableName = "ReportTable02";
                    idtstRecalculateIAPAlloc.DataSetName = "ReportTable01";
                    busCreateReports lobjCreateReports = new busCreateReports();
                    if (idtstRecalculateIAPAlloc.Tables[0].Rows.Count > 0)
                    {
                        try
                        {

                            lobjCreateReports.CreateIAPRecalcPDFReport(idtstRecalculateIAPAlloc, "RecalculateIAPAllocationDetail", busConstant.MPIPHPBatch.GENERATED_RECALCULATE_IAP_ALLOCATION, icdoPerson.mpi_person_id);
                            //lobjCreateReports.CreateIAPRecalcPDFReport(idtstRecalculateIAPAlloc, "rpt_IAPRecalculateSnapshot", busConstant.MPIPHPBatch.GENERATED_RECALCULATE_IAP_ALLOCATION, icdoPerson.mpi_person_id);

                            ibusIapRecalculationCopy = new busIapRecalculationCopy { icdoIapRecalculationCopy = new cdoIapRecalculationCopy() };
                            ibusIapRecalculationCopy.icdoIapRecalculationCopy.person_id = icdoPerson.person_id;
                            ibusIapRecalculationCopy.icdoIapRecalculationCopy.mpi_person_id = icdoPerson.mpi_person_id;
                            ibusIapRecalculationCopy.icdoIapRecalculationCopy.file_name = lobjCreateReports.istrFullFileName;
                            ibusIapRecalculationCopy.icdoIapRecalculationCopy.recalc_date = DateTime.Now;
                            ibusIapRecalculationCopy.icdoIapRecalculationCopy.path_code = busConstant.MPIPHPBatch.GENERATED_RECALCULATE_IAP_ALLOCATION;
                            ibusIapRecalculationCopy.icdoIapRecalculationCopy.Insert();

                        }
                        catch (Exception ex)
                        {

                        }


                    }
                }
            }
            #endregion Code to exclude from late batch run to avoid out of memory exception.

            if (iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.istrForfietureFlag == busConstant.FLAG_YES).Count() > 0)
            {
                foreach (busIapAllocationDetailCalculation lcdoIapallocationDetailPersonoverview in iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.istrForfietureFlag == busConstant.FLAG_YES))
                {
                    larrForfeitureYears.Add(lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year);
                }
            }


            if (iclbTotalIAPAllocationDetailPersonOverview != null && iclbTotalIAPAllocationDetailPersonOverview.Count > 0
            && iclbTotalIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Count() > 0)
            {
                if (iclbTotalIAPAllocationDetailPersonOverview != null && iclbTotalIAPAllocationDetailPersonOverview.Count > 0
              && iclbTotalIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").FirstOrDefault().icdoIapallocationDetailPersonoverview.idecTotal != 0)
                {
                    if (iclbTotalIAPAllocationDetailPersonOverview != null && iclbTotalIAPAllocationDetailPersonOverview.Count > 0
                         && iclbTotalIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").FirstOrDefault().icdoIapallocationDetailPersonoverview.idecTotal > 0M)
                    {
                        if (iclbTotalIAPAllocationDetailPersonOverview != null && iclbTotalIAPAllocationDetailPersonOverview.Count > 0
                      && iclbTotalIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").FirstOrDefault().icdoIapallocationDetailPersonoverview.idecOverrideTotal != 0M)
                        {
                            ldecOverriddenTotalAmount = iclbTotalIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").FirstOrDefault().icdoIapallocationDetailPersonoverview.idecOverrideTotal;
                            if (iclbTotalIAPAllocationDetailPersonOverview != null && iclbTotalIAPAllocationDetailPersonOverview.Count > 0
                           && iclbTotalIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").FirstOrDefault().icdoIapallocationDetailPersonoverview.idecOverrideTotal > 0.1M)
                            {
                                lobjError = AddError(0, "Overridden Amount cannot be greater than 10 cents");
                                iarrError.Add(lobjError);
                                return iarrError;
                            }
                        }
                    }
                }
            }

            if (iclbIAPAllocationDetailPersonOverview != null && iclbIAPAllocationDetailPersonOverview.Count > 0
                && iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Count() > 0)
            {
                foreach (busIapAllocationDetailCalculation lcdoIapallocationDetailPersonoverview in iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference"))
                {
                    DateTime lEffectiveDate = new DateTime();
                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };


                    //if (lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                    //    lEffectiveDate = idtEffectiveDateForReemployment;
                    //else
                    //    lEffectiveDate = idtEffectiveDate;

                    if (lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag)
                    {
                        // lEffectiveDate = idtEffectiveDateForReemployment;
                        if (lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year == idtEffectiveDateForReemployment.Year)
                        {
                            lEffectiveDate = idtEffectiveDateForReemployment;
                        }
                        else
                        {
                            lEffectiveDate = busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year));
                        }
                    }
                    else
                    {
                        if (lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year == idtEffectiveDate.Year)
                        {
                            lEffectiveDate = idtEffectiveDate;
                        }
                        else
                        {
                            lEffectiveDate = busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year));
                        }
                    }

                    //PIR 1014
                    if (iblnRetiree && !iblnReemployed && idtMDdate != DateTime.MinValue && idtMinimumDistributionDate.Year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year
                        && lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc1 != 0M)
                    {
                        lEffectiveDate = idtMinimumDistributionDate;
                    }

                    if (iclbIAPAllocationDetailPersonOverview.LastOrDefault().icdoIapallocationDetailPersonoverview.computational_year == lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year)
                    {
                        lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc1 += ldecOverriddenTotalAmount;
                    }

                    if (larrForfeitureYears != null && larrForfeitureYears.Count > 0 && larrForfeitureYears.Contains(lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year))
                    {
                        PostForfeitureAmountIntoContribution(larrForfeitureYears, lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year, lEffectiveDate);
                    }
                    else
                    {

                        PostDifferenceAmountIntoContribution(Convert.ToInt32(lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year), lintPersonAccountId, lEffectiveDate, lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc1,
                            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2, lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2_invt, lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc2_frft,
                            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc3, lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4, lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4_invt,
                            lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4_frft, lcdoIapallocationDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc5);
                    }
                }
            }


            #region Temporary Commented
            //if (ibusPersonOverview.iclbPayeeAccount != null
            //               && ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID
            //              && item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED).Count() > 0)
            //{
            //    if (iclbTotalIAPAllocationDetailPersonOverview != null && iclbTotalIAPAllocationDetailPersonOverview.Count > 0
            //      && iclbTotalIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Count() > 0)
            //    {
            //        if (iclbTotalIAPAllocationDetailPersonOverview != null && iclbTotalIAPAllocationDetailPersonOverview.Count > 0
            //      && iclbTotalIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").FirstOrDefault().icdoIapallocationDetailPersonoverview.idecTotal != 0)
            //        {
            //            if (iclbTotalIAPAllocationDetailPersonOverview != null && iclbTotalIAPAllocationDetailPersonOverview.Count > 0
            //                 && iclbTotalIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").FirstOrDefault().icdoIapallocationDetailPersonoverview.idecTotal > 0M)
            //            {

            //                decimal ldecAmount = iclbTotalIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").FirstOrDefault().icdoIapallocationDetailPersonoverview.idecTotal;
            //                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            //                lbusPayeeAccount = ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID
            //                && item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED).FirstOrDefault();
            //                lbusPayeeAccount.LoadNextBenefitPaymentDate();
            //                //Prod PIR 488- Commented: Payment item type and review code
            //                //lbusPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM21", ldecAmount, "0", 0,
            //                //                                                       lbusPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, "N", false);
            //                lbusPayeeAccount.ProcessTaxWithHoldingDetails();

            //                //lbusPayeeAccount.CreateReviewStatus();
            //                busWorkflowHelper.InitializeWorkflow(busConstant.UPDATE_PAYEE_ACCOUNT, lbusPayeeAccount.icdoPayeeAccount.person_id,
            //                                0, lbusPayeeAccount.icdoPayeeAccount.payee_account_id, null);

            //            }
            //            else
            //            {

            //                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            //                lbusPayeeAccount = ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID
            //                && item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED).FirstOrDefault();
            //                lbusPayeeAccount.LoadNextBenefitPaymentDate();
            //                lbusPayeeAccount.CreateOverPayments(lbusPayeeAccount, lbusPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, DateTime.MinValue, -(iclbTotalIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").FirstOrDefault().icdoIapallocationDetailPersonoverview.idecTotal),
            //                    0M, busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH);
            //                lbusPayeeAccount.ProcessTaxWithHoldingDetails();
            //                //lbusPayeeAccount.CreateReviewStatus();
            //                busWorkflowHelper.InitializeWorkflow(busConstant.PROCESS_OVERPAYMENT_WORKFLOW, lbusPayeeAccount.icdoPayeeAccount.person_id,
            //                                     0, lbusPayeeAccount.icdoPayeeAccount.payee_account_id, null);

            //            }

            //        }
            //    }
            //}
            #endregion Temporary Commented



            iclbIAPAllocationDetailPersonOverview.ForEach(item => item.EvaluateInitialLoadRules());
            iarrError.Add(this);
            // PIR - 752
            btn_RefreshAllocations();
            return iarrError;
        }

        public void PostForfeitureAmountIntoContribution(ArrayList aarrForfeitureYears, decimal aintCurrentYear, DateTime adtEffectiveDate)
        {
            decimal lintPreviousForfeitureYear = 0;

            Collection<busIapAllocationDetailCalculation> lclbForfeitureYearIapallocationDetail = new Collection<busIapAllocationDetailCalculation>();
            foreach (decimal lintYear in aarrForfeitureYears)
            {
                if (lintYear < aintCurrentYear)
                    lintPreviousForfeitureYear = lintYear;
            }

            if (lintPreviousForfeitureYear == 0)
            {
                //PIR 1058
                if (idtEACutOffDate != null)
                {
                    lclbForfeitureYearIapallocationDetail = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year <= aintCurrentYear && item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").ToList().ToCollection();
                    if (iclbIAPAllocDetailPersonOverviewBeforeLateHour != null && iclbIAPAllocDetailPersonOverviewBeforeLateHour.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year < iintYearofLateHour && item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Count() > 0)
                    {
                        foreach (busIapAllocationDetailCalculation lbusIapAllocationDetailCalculation in iclbIAPAllocDetailPersonOverviewBeforeLateHour.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year < iintYearofLateHour && item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS"))
                        {
                            lclbForfeitureYearIapallocationDetail.Add(lbusIapAllocationDetailCalculation);
                        }
                        //lclbForfeitureYearIapallocationDetail.Add(iclbIAPAllocDetailPersonOverviewBeforeLateHour.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year < iintYearofLateHour && item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").ToList());
                    }
                    lclbForfeitureYearIapallocationDetail = lclbForfeitureYearIapallocationDetail.OrderBy(t => t.icdoIapallocationDetailPersonoverview.computational_year).ToList().ToCollection();
                }
                else
                    lclbForfeitureYearIapallocationDetail = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year <= aintCurrentYear && item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").ToList().ToCollection();
            }
            else
            {
                //PIR 1058
                if (idtEACutOffDate != null)
                {
                    lclbForfeitureYearIapallocationDetail = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year > lintPreviousForfeitureYear && item.icdoIapallocationDetailPersonoverview.computational_year <= aintCurrentYear && item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").ToList().ToCollection();
                    if (iclbIAPAllocDetailPersonOverviewBeforeLateHour != null && iclbIAPAllocDetailPersonOverviewBeforeLateHour.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year > lintPreviousForfeitureYear && item.icdoIapallocationDetailPersonoverview.computational_year < iintYearofLateHour && item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Count() > 0)
                    {
                        foreach (busIapAllocationDetailCalculation lbusIapAllocationDetailCalculation in iclbIAPAllocDetailPersonOverviewBeforeLateHour.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year > lintPreviousForfeitureYear && item.icdoIapallocationDetailPersonoverview.computational_year < iintYearofLateHour && item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS"))
                        {
                            lclbForfeitureYearIapallocationDetail.Add(lbusIapAllocationDetailCalculation);
                        }
                        //lclbForfeitureYearIapallocationDetail.Add(iclbIAPAllocDetailPersonOverviewBeforeLateHour.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year > lintPreviousForfeitureYear && item.icdoIapallocationDetailPersonoverview.computational_year < iintYearofLateHour && item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS"));
                    }
                    lclbForfeitureYearIapallocationDetail = lclbForfeitureYearIapallocationDetail.OrderBy(t => t.icdoIapallocationDetailPersonoverview.computational_year).ToList().ToCollection();
                }
                else
                    lclbForfeitureYearIapallocationDetail = iclbIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year > lintPreviousForfeitureYear && item.icdoIapallocationDetailPersonoverview.computational_year <= aintCurrentYear && item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").ToList().ToCollection();
            }


            //For only Forfieture Allocations for forfieture year
            Collection<busIapAllocationDetailCalculation> lclbForfeitureYearIapallocationDetailOPUS = new Collection<busIapAllocationDetailCalculation>();
            DataTable ldtblForfeitureYearIapallocationDetailOPUS = Select("cdoPersonAccountRetirementContribution.GetIAPAllocationDetailsForForfietureYear", new object[2] { lintPersonAccountId, aintCurrentYear });
            if (ldtblForfeitureYearIapallocationDetailOPUS != null && ldtblForfeitureYearIapallocationDetailOPUS.Rows.Count > 0)
            {
                lclbForfeitureYearIapallocationDetailOPUS = GetCollection<busIapAllocationDetailCalculation>(ldtblForfeitureYearIapallocationDetailOPUS, "icdoIapallocationDetailPersonoverview");
            }

            //For allocations apart from Forfieture Allocations for forfieture year
            Collection<busIapAllocationDetailCalculation> lclbForfeitureYearIapallocationDetailOPUSOtherAlloc = new Collection<busIapAllocationDetailCalculation>();
            DataTable ldtblForfeitureYearIapallocationDetailOPUSOtherAlloc = Select("cdoPersonAccountRetirementContribution.GetIAPAllocationDetailsForForfietureYearOtherAllocations", new object[2] { lintPersonAccountId, aintCurrentYear });
            if (ldtblForfeitureYearIapallocationDetailOPUSOtherAlloc != null && ldtblForfeitureYearIapallocationDetailOPUSOtherAlloc.Rows.Count > 0)
            {
                lclbForfeitureYearIapallocationDetailOPUSOtherAlloc = GetCollection<busIapAllocationDetailCalculation>(ldtblForfeitureYearIapallocationDetailOPUSOtherAlloc, "icdoIapallocationDetailPersonoverview");
            }

            decimal ldecOtherAllocAllocation4Amount = 0M, ldecOtherAllocAlloc1Amount = 0M, ldecOtherAllocAlloc2Amount = 0M, ldecOtherAllocAlloc2InvstAmount = 0M, ldecOtherAllocAlloc2FrftAmount = 0M, ldecOtherAllocAlloc3Amount = 0M, ldecOtherAllocAlloc4InvstAmount = 0M, ldecOtherAllocAlloc4FrftAmount = 0M, ldecOtherAllocAlloc5Amount = 0M;
            if (lclbForfeitureYearIapallocationDetailOPUSOtherAlloc != null && lclbForfeitureYearIapallocationDetailOPUSOtherAlloc.Count > 0)
            {
                ldecOtherAllocAlloc1Amount = lclbForfeitureYearIapallocationDetailOPUSOtherAlloc.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1);
                ldecOtherAllocAlloc2Amount = lclbForfeitureYearIapallocationDetailOPUSOtherAlloc.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2);
                ldecOtherAllocAlloc2InvstAmount = lclbForfeitureYearIapallocationDetailOPUSOtherAlloc.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_invt);
                ldecOtherAllocAlloc2FrftAmount = lclbForfeitureYearIapallocationDetailOPUSOtherAlloc.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_frft);
                ldecOtherAllocAlloc3Amount = lclbForfeitureYearIapallocationDetailOPUSOtherAlloc.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc3);
                ldecOtherAllocAllocation4Amount = lclbForfeitureYearIapallocationDetailOPUSOtherAlloc.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4);
                ldecOtherAllocAlloc4InvstAmount = lclbForfeitureYearIapallocationDetailOPUSOtherAlloc.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_invt);
                ldecOtherAllocAlloc4FrftAmount = lclbForfeitureYearIapallocationDetailOPUSOtherAlloc.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_frft);
                ldecOtherAllocAlloc5Amount = lclbForfeitureYearIapallocationDetailOPUSOtherAlloc.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc5);
            }

            if (lclbForfeitureYearIapallocationDetail != null && lclbForfeitureYearIapallocationDetail.Count > 0)
            {
                busIapAllocationDetailCalculation lcdoIAPPaid = null;
                if (iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == aintCurrentYear).Count() > 0)
                    lcdoIAPPaid = iclbPaidIAPAllocationDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == aintCurrentYear).FirstOrDefault();

                if (lclbForfeitureYearIapallocationDetailOPUS != null && lclbForfeitureYearIapallocationDetailOPUS.Count > 0 && lcdoIAPPaid != null)
                {
                    PostDifferenceAmountIntoContribution(Convert.ToInt32(aintCurrentYear), lintPersonAccountId, adtEffectiveDate,
                       -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1) + ldecOtherAllocAlloc1Amount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1) - lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc1,
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2) + ldecOtherAllocAlloc2Amount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2) - lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc2,
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_invt) + ldecOtherAllocAlloc2InvstAmount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_invt) - lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc2_invt,
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_frft) + ldecOtherAllocAlloc2FrftAmount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_frft) - lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc2_frft,
                                -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc3) + ldecOtherAllocAlloc3Amount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc3) - lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc3,
                                -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4) + ldecOtherAllocAllocation4Amount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4) - lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc4,
                                -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_invt) + ldecOtherAllocAlloc4InvstAmount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_invt) - lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc4_invt,
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_frft) + ldecOtherAllocAlloc4FrftAmount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_frft) - lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc4_frft,
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc5) + ldecOtherAllocAlloc5Amount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc5) - lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc5
                                 , ablnIsForfeiture: true);
                }
                else if (lclbForfeitureYearIapallocationDetailOPUS != null && lclbForfeitureYearIapallocationDetailOPUS.Count > 0)
                {
                    PostDifferenceAmountIntoContribution(Convert.ToInt32(aintCurrentYear), lintPersonAccountId, adtEffectiveDate,
                       -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1) + ldecOtherAllocAlloc1Amount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1),
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2) + ldecOtherAllocAlloc2Amount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2),
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_invt) + ldecOtherAllocAlloc2InvstAmount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_invt),
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_frft) + ldecOtherAllocAlloc2FrftAmount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_frft),
                                -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc3) + ldecOtherAllocAlloc3Amount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc3),
                                -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4) + ldecOtherAllocAllocation4Amount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4),
                                -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_invt) + ldecOtherAllocAlloc4InvstAmount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_invt),
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_frft) + ldecOtherAllocAlloc4FrftAmount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_frft),
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc5) + ldecOtherAllocAlloc5Amount) - lclbForfeitureYearIapallocationDetailOPUS.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc5)
                                 , ablnIsForfeiture: true);

                }
                else
                {
                    PostDifferenceAmountIntoContribution(Convert.ToInt32(aintCurrentYear), lintPersonAccountId, adtEffectiveDate, -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1) + ldecOtherAllocAlloc1Amount),
                                  -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2) + ldecOtherAllocAlloc2Amount),
                                  -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_invt) + ldecOtherAllocAlloc2InvstAmount),
                                  -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc2_frft) + ldecOtherAllocAlloc2FrftAmount),
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc3) + ldecOtherAllocAlloc3Amount),
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4) + ldecOtherAllocAllocation4Amount),
                                 -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_invt) + ldecOtherAllocAlloc4InvstAmount),
                                  -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4_frft) + ldecOtherAllocAlloc4FrftAmount),
                                  -(lclbForfeitureYearIapallocationDetail.Sum(item => item.icdoIapallocationDetailPersonoverview.alloc5) + ldecOtherAllocAlloc5Amount), ablnIsForfeiture: true);
                }
            }
        }
        ///Parameters adtEACutOffDate & aintComputationYear are strictly used for Late IAP Alloation Batch.
        ///These parameters has no significance in IAP Recalculation Functionality.Please do not use these parameters.
        public void PostDifferenceAmountIntoContribution(int aintComputationYear, int aintPersonAccountID, DateTime adtEffectiveDate, decimal adecAlloc1Amount, decimal adecAlloc2Amount, decimal adecAlloc2InvstAmount, decimal adecAlloc2FrftAmount,
         decimal adecAlloc3Amount, decimal adecAllocation4Amount, decimal adecAlloc4InvstAmount, decimal adecAlloc4FrftAmount, decimal adecAlloc5Amount, bool ablnIsForfeiture = false)
        {
            //PIR 628 Extended
            DateTime ldtTransactionDate = new DateTime();
            if (idtEACutOffDate != DateTime.MinValue)
                ldtTransactionDate = idtEACutOffDate;
            else
                ldtTransactionDate = DateTime.Now;

            string lstrTransactionType = busConstant.RCTransactionTypeAdjustment;
            //PIR 628
            if (idtEACutOffDate != DateTime.MinValue)
                lstrTransactionType = busConstant.RCTransactionTypeLateAllocation;

            if (ablnIsForfeiture)
                lstrTransactionType = busConstant.RCTransactionTypeForfeiture;
            //         IAP Enhancement item #8
            // Mazher: - Please uncomment below snipphet 
            //if (lblnLateHourBatch)
            //{

            //    if (!iblnReemployed)
            //    {
            //        if (adecAlloc1Amount != 0)
            //        {
            //            if (idtlateBatchRetirementDate != DateTime.MinValue)
            //            {
            //                adtEffectiveDate = idtlateBatchRetirementDate;
            //                ldtTransactionDate = DateTime.Now;
            //                aintComputationYear = adtEffectiveDate.Year;
            //            }

            //        }

            //    }
            //}




            busPersonAccountRetirementContribution lobjRetrContribution;
            //block to insert the allocation 1 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adecAlloc1Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adecIAPBalanceAmount: adecAlloc1Amount,
                astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation1);//PIR 628 Extended
            }
            //block to insert the allocation 2 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adecAlloc2Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adecIAPBalanceAmount: adecAlloc2Amount,
                    astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation2);//PIR 628 Extended
            }
            //block to insert the allocation 2 invst difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adecAlloc2InvstAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adecIAPBalanceAmount: adecAlloc2InvstAmount,
                astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation2, astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);//PIR 628 Extended
            }
            //block to insert the allocation 2 frft difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };


            if (adecAlloc2FrftAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adecIAPBalanceAmount: adecAlloc2FrftAmount,
                astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation2, astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);//PIR 628 Extended
            }
            //block to insert the allocation 3 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };


            if (adecAlloc3Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adecIAPBalanceAmount: adecAlloc3Amount,
                astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation3);//PIR 628 Extended
            }
            //block to insert the allocation 4 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
            //141846
            if (iblnInLateBatch && aintComputationYear > ibusAllocationSummary.icdoIapAllocationSummary.computation_year)
            {
                adecAllocation4Amount = 0;
            }
            if (adecAllocation4Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adecIAPBalanceAmount: adecAllocation4Amount,
                astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation4);//PIR 628 Extended
            }
            //block to insert the allocation 4 invt difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };


            if (adecAlloc4InvstAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adecIAPBalanceAmount: adecAlloc4InvstAmount,
                astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation4, astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);//PIR 628 Extended
            }
            //block to insert the allocation 4 forft difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };


            if (adecAlloc4FrftAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adecIAPBalanceAmount: adecAlloc4FrftAmount,
                astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation4, astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);//PIR 628 Extended
            }
            ////block to insert the allocation 5 affl difference amount            
            //lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            //if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc5_affl"].IsDBNull()))
            //    adecAlloc5AfflAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc5_affl"]);
            //if (adecAlloc5AfflAmount != 0)
            //{
            //    lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc5AfflAmount,
            //    astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation5, astrContributionSubtype: busConstant.RCContributionSubTypeAffiliates);
            //}
            ////block to insert the allocation 5 non affl difference amount            
            //lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            //if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc5alloc4_both_nonaffl"].IsDBNull()))
            //    adecAlloc5NonAfflAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc5_nonaffl"]);
            //if (adecAlloc5NonAfflAmount != 0)
            //{
            //    lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc5NonAfflAmount,
            //    astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation5, astrContributionSubtype: busConstant.RCContributionSubTypeNonAffiliates);
            //}
            ////block to insert the allocation 5 affl & non affl difference amount            
            //lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            //if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc5_both"].IsDBNull()))
            //    adecAlloc5BothAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc5_both"]);
            //if (adecAlloc5BothAmount != 0)
            //{
            //    lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc5BothAmount,
            //    astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation5, astrContributionSubtype: busConstant.RCContributionSubTypeBoth);
            //}

            //block to insert the allocation 5 affl & non affl difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
            decimal ldecTotalAlloc5 = adecAlloc5Amount;
            //         IAP Enhancement item #8
            // Mazher: - Please uncomment below snipphet
            //if (aintComputationYear >= 1996 && aintComputationYear <= 2001)
            //{
            //    if (lblnLateHourBatch)
            //    {
            //        DataTable ldtblRetirementyear = busBase.Select("cdoIapAllocationDetail.CheckRetiremntYearRU65", new object[1] { aintPersonAccountID });

            //        if (ldtblRetirementyear != null && ldtblRetirementyear.Rows.Count > 0 &&
            //            Convert.ToString(ldtblRetirementyear.Rows[0][0]).IsNotNullOrEmpty())
            //        {
            //            if (Convert.ToInt32(ldtblRetirementyear.Rows[0][0]) == aintComputationYear)
            //                ldecTotalAlloc5 = 0;
            //        }
            //    }

            //}

            if (ldecTotalAlloc5 != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adecIAPBalanceAmount: ldecTotalAlloc5,
                astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation5);//PIR 628 Extended
            }
        }



        public void PostSpAccntDiffAmountIntoContribution(string astrFundType, int aintComputationYear, int aintPersonAccountID, DateTime adtEffectiveDate, decimal adecAlloc1Amount, decimal adecAllocation4Amount)
        {
            DateTime ldtTransactionDate = new DateTime();

            ldtTransactionDate = DateTime.Now;

            string lstrTransactionType = busConstant.RCTransactionTypeAdjustment;

            busPersonAccountRetirementContribution lobjRetrContribution;
            //block to insert the allocation 1 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };



            if (adecAlloc1Amount != 0)
            {
                if (astrFundType == busConstant.FundTypeLocal52SpecialAccount)
                {
                    lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adec52SplAccountBalance: adecAlloc1Amount,
                    astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation1);
                }
                else if (astrFundType == busConstant.FundTypeLocal161SpecialAccount)
                {
                    lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adec161SplAccountBalance: adecAlloc1Amount,
                    astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation1);
                }
            }

            //block to insert the allocation 4 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adecAllocation4Amount != 0)
            {
                if (astrFundType == busConstant.FundTypeLocal52SpecialAccount)
                {
                    lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adec52SplAccountBalance: adecAllocation4Amount,
                    astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation4);
                }
                else if (astrFundType == busConstant.FundTypeLocal161SpecialAccount)
                {
                    lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, adtEffectiveDate, ldtTransactionDate, aintComputationYear, adec161SplAccountBalance: adecAllocation4Amount,
                    astrTransactionType: lstrTransactionType, astrContributionType: busConstant.RCContributionTypeAllocation4);
                }
            }

        }


        /// <summary>
        /// GET MD DATE IF participant is Active in IAP plan and Payee Account not Exist for IAP PLAN but Exist for MPI Plan.
        /// pick approved mpi md application date as md date
        /// Exclude Convert MD to Late retirement 
        /// </summary>
        private DateTime GetBenefitMPIMDApplication(int aintPersonID)
        {
            DateTime idtMDApplicationDate = DateTime.MinValue;
            DataTable ldtMDApplicationDetail = busBase.Select("cdoIapAllocationSummary.GetMDDateForIAPAlloactionYerlyDetail", new object[1] { aintPersonID });
            if (ldtMDApplicationDetail.Rows.Count > 0 && ldtMDApplicationDetail != null)
            {
                idtMDApplicationDate = Convert.ToDateTime(ldtMDApplicationDetail.Rows[0]["MIN_DISTRIBUTION_DATE"]);
                return idtMDApplicationDate;
            }
            else
                return idtMDApplicationDate;

        }



        #region IAP Local Special Accounts Recalculation


        //PIR 19
        public void LoadRecalculateAndPostSpAccntAllocDetail(int aintPersonId, string astrFundType)
        {
            iclbSpAccntAllocDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
            iclbTotalSpAccntAllocDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
            iclbPaidSpAccntAllocDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();

            if (this.FindPerson(aintPersonId))
            {
                DataTable ldtblist = busPerson.Select("cdoPersonAccount.LoadPersonAccountbyPlanId", new object[2] { this.icdoPerson.person_id, busConstant.IAP_PLAN_ID });
                if (ldtblist.Rows.Count > 0)
                {
                    this.iclbSpAccntAllocDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
                    this.LoadPaidSpAccntAllocDetailsFromOpus(Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()]), astrFundType);
                    this.RecalculateSpAccntAllocDetails(astrFundType, false);
                    this.LoadSpAccntAllocDetailsFromOpus(Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()]), astrFundType);
                    this.GetDifferenceSpAccntAllocDetails();
                }
            }
        }

        //PIR 19
        public void RecalculateSpAccntAllocDetails(string astrFundType, bool ablnIsRefreshAllocations = false)
        {

            int lintPersonAccountId = 0;
            busBenefitApplication lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };

            ibusAllocationSummary = new busIapAllocationSummary { icdoIapAllocationSummary = new cdoIapAllocationSummary() };
            ibusAllocationSummary.LoadLatestAllocationSummary();

            DeterminePersonTypeForSpAccntAlloc(astrFundType, ref lobjBenefitApplication);
            lintPersonAccountId = lobjBenefitApplication.ibusPerson.iclbPersonAccount.FirstOrDefault().icdoPersonAccount.person_account_id;


            RecalculateAndFillSpAccntDetails(astrFundType, lobjBenefitApplication, lintPersonAccountId, false);

        }

        //PIR 19
        public void RecalculateAndFillSpAccntDetails(string astrFundType, busBenefitApplication aobjBenefitApplication, int aintPersonAccountId, bool ablnIsRefreshAllocations = false)
        {

            busIAPAllocationHelper lobjIAPAllocationHelper = new busIAPAllocationHelper();
            lobjIAPAllocationHelper.LoadIAPAllocationFactor();

            int lintComputationYear;
            lintComputationYear = 0;

            decimal ldecAllocation4Amount, ldecSpAccountBalance = 0.00M, ldecAlloc1Amount;
            decimal ldecFactor = 0;


            #region Pad the missing Years and for active Participants bring work history till last allocation

            int lintFirstYear = busConstant.GoLiveYear; //OPUS Go Live Year
            int lintLastYear = 0;

            iclbSpAccntAllocDetailComputed = new Collection<busIapAllocationDetailCalculation>();

            DataTable ldtbIAPAllocation = null;
            if (astrFundType == busConstant.FundTypeLocal52SpecialAccount)
            {
                ldtbIAPAllocation = busBase.Select("cdoPersonAccountRetirementContribution.GetL52AllocDetailsForRecalculation", new object[1] { aintPersonAccountId });

                DataTable ldtbLocal52Details = busBase.Select("cdoPersonAccountRetirementContribution.GetLocal52SpecialAccountDetailsForReCalculation", new object[1] { aintPersonAccountId }); //989
                if (ldtbLocal52Details.Rows.Count > 0)
                {
                    iclbSpAccntAllocDetailComputed = GetCollection<busIapAllocationDetailCalculation>(ldtbLocal52Details, "icdoIapallocationDetailPersonoverview");
                }
            }
            else
            {
                ldtbIAPAllocation = busBase.Select("cdoPersonAccountRetirementContribution.GetL161AllocDetailsForRecalculation", new object[1] { aintPersonAccountId });

                DataTable ldtbLocal161Details = busBase.Select("cdoPersonAccountRetirementContribution.GetLocal161SpecialAccountDetailsForReCalculation", new object[1] { aintPersonAccountId }); //989
                if (ldtbLocal161Details.Rows.Count > 0)
                {
                    iclbSpAccntAllocDetailComputed = GetCollection<busIapAllocationDetailCalculation>(ldtbLocal161Details, "icdoIapallocationDetailPersonoverview");
                }
            }

            if (ldtbIAPAllocation.Rows.Count > 0)
            {
                lintLastYear = Convert.ToInt32(ldtbIAPAllocation.Rows[ldtbIAPAllocation.Rows.Count - 1]["COMPUTATIONAL_YEAR"]);

                if (idtWithdrawalDate.Year > lintLastYear && idtWithdrawalDate.Year >= ibusAllocationSummary.icdoIapAllocationSummary.computation_year)
                {
                    lintLastYear = idtWithdrawalDate.Year;
                }

                else if (iblnIAPLclSpAccWithdrwal && lintLastYear > idtWithdrawalDate.Year)
                {
                    lintLastYear = idtWithdrawalDate.Year;
                }
            }
            else
            {
                lintLastYear = this.ibusAllocationSummary.icdoIapAllocationSummary.computation_year;
            }


            if (lintFirstYear > 0 && lintLastYear > 0)
            {

                for (int i = lintFirstYear; i <= lintLastYear; i++)
                {
                    if (iclbSpAccntAllocDetailComputed.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == i).Count() > 0)
                        continue;

                    int lintWorkHistoryYear = i;
                    busIapAllocationDetailCalculation lbusSpAccntAllocationDetailCalculation = new busIapAllocationDetailCalculation { icdoIapallocationDetailPersonoverview = new cdoIapallocationDetailPersonoverview() };
                    lbusSpAccntAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.computational_year = lintWorkHistoryYear;
                    lbusSpAccntAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrFundType = astrFundType;
                    iclbSpAccntAllocDetailComputed.Add(lbusSpAccntAllocationDetailCalculation);
                }
            }


            #endregion Pad the missing Years


            if (iblnIAPLclSpAccWithdrwal && iclbSpAccntAllocDetailComputed != null && iclbSpAccntAllocDetailComputed.Count() > 0)
            {
                int lintWorkHistoryYear = Convert.ToInt16(iclbSpAccntAllocDetailComputed.LastOrDefault().icdoIapallocationDetailPersonoverview.computational_year) + 1;
                while (lintWorkHistoryYear <= idtWithdrawalDate.Year)
                {
                    if (lintWorkHistoryYear == idtWithdrawalDate.Year && (idtWithdrawalDate.Month == 1 || idtWithdrawalDate.Month == 2 || idtWithdrawalDate.Month == 3))
                        break;

                    busIapAllocationDetailCalculation lbusSpAccntAllocationDetailCalculation = new busIapAllocationDetailCalculation { icdoIapallocationDetailPersonoverview = new cdoIapallocationDetailPersonoverview() };
                    lbusSpAccntAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.computational_year = lintWorkHistoryYear;
                    lbusSpAccntAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.istrFundType = astrFundType;
                    iclbSpAccntAllocDetailComputed.Add(lbusSpAccntAllocationDetailCalculation);
                    lintWorkHistoryYear++;
                }
            }

            if (iclbSpAccntAllocDetailComputed != null && iclbSpAccntAllocDetailComputed.Count() > 0
                && iclbSpAccntAllocDetailComputed.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year < busConstant.GoLiveYear).Count() > 0)
            {
                ldecSpAccountBalance = iclbSpAccntAllocDetailComputed.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year < busConstant.GoLiveYear).Sum(t => t.icdoIapallocationDetailPersonoverview.alloc1)
                            + iclbSpAccntAllocDetailComputed.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year < busConstant.GoLiveYear).Sum(t => t.icdoIapallocationDetailPersonoverview.alloc4);
            }
            int lintPreviousYear = 0;

            DataTable ldtblQDROPayments = busBase.Select("entIAPAllocationDetailPersonOverview.GetQDROPaymentsForSpAccnt", new object[2] { aintPersonAccountId, astrFundType });

            foreach (busIapAllocationDetailCalculation lbusSpAccntAllocationDetailCalculation in iclbSpAccntAllocDetailComputed)
            {

                ldecFactor = 0;

                lintComputationYear = 0;

                ldecAlloc1Amount = ldecAllocation4Amount = 0.00M;
                lintComputationYear = Convert.ToInt32(lbusSpAccntAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.computational_year);

                if (lintComputationYear < busConstant.GoLiveYear || lintLastYear < lintComputationYear)
                    continue;


                #region Negate Alloction Amount Paid if any for Previous year(This will handle withdrawal as well as QDRO if any)

                if (iclbPaidSpAccntAllocDetailPersonOverview != null && iclbPaidSpAccntAllocDetailPersonOverview.Count() > 0
                   && iclbPaidSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).Count() > 0)
                {
                    decimal ldecPaidIAPAccountBalance = 0M;
                    if (ldtblQDROPayments != null && ldtblQDROPayments.Rows.Count > 0
                        && ldtblQDROPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear).Count() > 0)
                    {
                        ldecPaidIAPAccountBalance = ldtblQDROPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear).Sum(t => t.Field<decimal>("AMOUNT"));
                        ldecSpAccountBalance -= ldecPaidIAPAccountBalance;
                    }
                }

                decimal ldecPreviousYearPaidSpAccountBalance = 0M;
                if ((iclbPaidSpAccntAllocDetailPersonOverview != null && iclbPaidSpAccntAllocDetailPersonOverview.Count() > 0
                    && iclbPaidSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear - 1).Count() > 0))
                {
                    busIapAllocationDetailCalculation lcdoIAPPaid = iclbPaidSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear - 1).FirstOrDefault();
                    ldecPreviousYearPaidSpAccountBalance = lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc1 + lcdoIAPPaid.icdoIapallocationDetailPersonoverview.alloc4;
                    decimal ldecQDROChildSupportPayments = 0M;


                    if (iclbPaidSpAccntAllocDetailPersonOverview != null && iclbPaidSpAccntAllocDetailPersonOverview.Count() > 0
                    && iclbPaidSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear - 1).Count() > 0)
                    {
                        if (ldtblQDROPayments != null && ldtblQDROPayments.Rows.Count > 0
                            && ldtblQDROPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear - 1).Count() > 0)
                        {
                            ldecQDROChildSupportPayments = ldtblQDROPayments.AsEnumerable().Where(t => t.Field<Int32>("YEAR") == lintComputationYear - 1).Sum(t => t.Field<decimal>("AMOUNT"));
                        }
                    }

                    ldecSpAccountBalance += ldecPreviousYearPaidSpAccountBalance;
                    ldecSpAccountBalance += ldecQDROChildSupportPayments;

                }
                #endregion Negate Alloction Amount Paid if any for current year


                if ((iblnIAPLclSpAccWithdrwal || iblnDead) && lintComputationYear >= ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1
                    && idtWithdrawalDate.Year > ibusAllocationSummary.icdoIapAllocationSummary.computation_year + 1)
                {
                    ldecAlloc1Amount = 0;
                }
                if ((iblnIAPLclSpAccWithdrwal || iblnDead) && lintComputationYear == idtWithdrawalDate.Year)
                {
                    int lintQuarter = 0;
                    lintQuarter = busGlobalFunctions.GetPreviousQuarter(idtWithdrawalDate);
                    ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecSpAccountBalance, lintQuarter, ref ldecFactor);
                }
                else
                {
                    ldecAlloc1Amount = lobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecSpAccountBalance, 4, ref ldecFactor);
                }


                ldecSpAccountBalance += ldecAlloc1Amount + ldecAllocation4Amount;

                decimal ldecTotal = ldecAlloc1Amount + ldecAllocation4Amount;
                lintPreviousYear = lintComputationYear;

                FillSpAccntAllocationDetails(this.icdoPerson.ssn, "Computed", lintComputationYear, ldecAlloc1Amount, ldecAllocation4Amount, ldecTotal);
            }
        }

        public void DeterminePersonTypeForSpAccntAlloc(string astrFundType, ref busBenefitApplication aobjBenefitApplication)
        {

            iblnParticipant = false;
            iblnActive = false;
            iblnIAPLclSpAccWithdrwal = false;
            iblnDead = false;

            iblnEligibleForQYRYAllocations = false;

            idtWithdrawalDate = DateTime.MinValue;

            ibusPersonOverview = new busPersonOverview();
            if (ibusPersonOverview.FindPerson(this.icdoPerson.person_id))
            {
                ibusPersonOverview.LoadInitialData();
                ibusPersonOverview.GetCurrentAge();
                ibusPersonOverview.LoadPersonDROApplications();
                ibusPersonOverview.LoadDeathNotifications();
                ibusPersonOverview.LoadBenefitApplication();
                ibusPersonOverview.LoadPayeeAccount();
            }

            aobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            aobjBenefitApplication.icdoBenefitApplication.person_id = Convert.ToInt32(this.icdoPerson.person_id);
            aobjBenefitApplication.LoadPerson();
            aobjBenefitApplication.ibusPerson.LoadPersonAccountsByPlanId(busConstant.IAP_PLAN_ID);

            int lintLastYear = 0;
            DataTable ldtbIAPAllocation = new DataTable();

            if (astrFundType == busConstant.FundTypeLocal52SpecialAccount)
                ldtbIAPAllocation = busBase.Select("cdoPersonAccountRetirementContribution.GetL52AllocDetailsForRecalculation", new object[1] { aobjBenefitApplication.ibusPerson.iclbPersonAccount.FirstOrDefault().icdoPersonAccount.person_account_id });
            else
                ldtbIAPAllocation = busBase.Select("cdoPersonAccountRetirementContribution.GetL161AllocDetailsForRecalculation", new object[1] { aobjBenefitApplication.ibusPerson.iclbPersonAccount.FirstOrDefault().icdoPersonAccount.person_account_id });

            if (ldtbIAPAllocation.Rows.Count > 0)
            {
                lintLastYear = Convert.ToInt32(ldtbIAPAllocation.Rows[ldtbIAPAllocation.Rows.Count - 1]["COMPUTATIONAL_YEAR"]);
            }
            else
            {
                lintLastYear = ibusAllocationSummary.icdoIapAllocationSummary.computation_year;
            }


            if (ibusPersonOverview.iblnParticipant == busConstant.YES && icdoPerson.date_of_death != DateTime.MinValue
                && ibusPersonOverview.iclbPayeeAccount != null &&
                     ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT &&
                    item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID && item.icdoPayeeAccount.istrFundType == astrFundType).Count() > 0)
            {

                iblnParticipant = true;
                iblnDead = true;

                DateTime ldtDate = new DateTime();
                ldtDate = icdoPerson.date_of_death;

                iblnEligibleForQYRYAllocations = CheckIfEligibleToGetQuarterly(ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT &&
                    item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID && item.icdoPayeeAccount.istrFundType == astrFundType).FirstOrDefault().icdoPayeeAccount.benefit_application_detail_id);


                DataTable ldtblFirstPaymentDate = Select("cdoIAPAllocationDetailPersonOverview.GetFirstPaymentDateForSpAccnts", new object[2] { this.icdoPerson.person_id, astrFundType });
                if (ldtblFirstPaymentDate != null && ldtblFirstPaymentDate.Rows.Count > 0)
                {
                    if (Convert.ToString(ldtblFirstPaymentDate.Rows[0][enmPaymentHistoryHeader.payment_date.ToString().ToUpper()]).IsNotNullOrEmpty())
                        ldtDate = Convert.ToDateTime(ldtblFirstPaymentDate.Rows[0][enmPaymentHistoryHeader.payment_date.ToString().ToUpper()]);
                }
                else
                {
                    ldtDate = busGlobalFunctions.GetLastDateOfComputationYear(lintLastYear);
                    iblnParticipant = true;
                    iblnActive = true;
                    iblnDead = false;
                }

                idtWithdrawalDate = ldtDate;
                idtEffectiveDate = idtWithdrawalDate;
            }
            else if (ibusPersonOverview.iblnParticipant == busConstant.YES && (ibusPersonOverview.iclbPayeeAccount != null && ibusPersonOverview.iclbPayeeAccount.Where(item =>
                    item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                    item.icdoPayeeAccount.istrFundType == astrFundType &&
                   item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && item.icdoPayeeAccount.account_relation_value != busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE).Count() > 0))
            {

                iblnParticipant = true;
                iblnIAPLclSpAccWithdrwal = true;


                iblnEligibleForQYRYAllocations = CheckIfEligibleToGetQuarterly(ibusPersonOverview.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                    item.icdoPayeeAccount.istrFundType == astrFundType && item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && item.icdoPayeeAccount.account_relation_value != busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE).FirstOrDefault().icdoPayeeAccount.benefit_application_detail_id);


                DateTime ldtWithdrawalDate = new DateTime();
                if (ibusPersonOverview.iclbPayeeAccount != null && ibusPersonOverview.iclbPayeeAccount.Where(item =>
                item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                item.icdoPayeeAccount.istrFundType == astrFundType &&
               item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && item.icdoPayeeAccount.account_relation_value != busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE).Count() > 0)
                {


                    ldtWithdrawalDate = ibusPersonOverview.iclbPayeeAccount.Where(item =>
                    item.icdoPayeeAccount.istrStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED && item.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID &&
                    item.icdoPayeeAccount.istrFundType == astrFundType &&
                   item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && item.icdoPayeeAccount.account_relation_value != busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE).FirstOrDefault().icdoPayeeAccount.idtRetireMentDate;
                }
                else
                {
                    ldtWithdrawalDate = busGlobalFunctions.GetLastDateOfComputationYear(lintLastYear);

                    iblnIAPLclSpAccWithdrwal = false;
                    iblnActive = true;
                }
                idtWithdrawalDate = ldtWithdrawalDate;
                idtEffectiveDate = idtWithdrawalDate;
            }
            else if (ibusPersonOverview.iblnParticipant == busConstant.YES && icdoPerson.date_of_death == DateTime.MinValue)
            {
                iblnParticipant = true;
                iblnActive = true;
                idtWithdrawalDate = busGlobalFunctions.GetLastDateOfComputationYear(lintLastYear);
                idtEffectiveDate = idtWithdrawalDate;
            }

        }


        public void GetDifferenceSpAccntAllocDetails()
        {
            ArrayList larrComputationYear = new ArrayList();
            foreach (busIapAllocationDetailCalculation lcdoSpAccntAllocDetailPersonoverview in iclbSpAccntAllocDetailPersonOverview)
            {
                if (!larrComputationYear.Contains(lcdoSpAccntAllocDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year))
                    larrComputationYear.Add(lcdoSpAccntAllocDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year);

                lcdoSpAccntAllocDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecTotal = lcdoSpAccntAllocDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc1 + lcdoSpAccntAllocDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4;
            }


            foreach (decimal lintComputationYear in larrComputationYear)
            {

                decimal ldecNewAllocation4Amount = 0M, ldecNewAlloc1Amount = 0M, ldecNewTotal = 0M;
                decimal ldecOldAllocation4Amount = 0M, ldecOldAlloc1Amount = 0M, ldecOldTotal = 0M;


                if (iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).Count() > 0)
                {
                    
                        ldecNewAlloc1Amount = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc1;
                   

                    ldecNewAllocation4Amount = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4;
                   
                        ldecNewTotal = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.idecTotal;
                   
                }

                if (iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).Count() > 0)
                {
                    
                    
                        ldecOldAlloc1Amount = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc1;

                   

                    ldecOldAllocation4Amount = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).FirstOrDefault()
                        .icdoIapallocationDetailPersonoverview.alloc4;
                  
                   
                        ldecOldTotal = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" && item.icdoIapallocationDetailPersonoverview.computational_year == lintComputationYear).FirstOrDefault()
                     .icdoIapallocationDetailPersonoverview.idecTotal;
                    
                }

                if (lintComputationYear >= busConstant.GoLiveYear)
                    FillSpAccntAllocationDetails(icdoPerson.ssn, "Difference", lintComputationYear, ldecNewAlloc1Amount - ldecOldAlloc1Amount, ldecNewAllocation4Amount - ldecOldAllocation4Amount, ldecNewTotal - ldecOldTotal);
            }


            foreach (busIapAllocationDetailCalculation lbusSpAccntAllocDetailCalculation in iclbPaidSpAccntAllocDetailPersonOverview)
            {
                //  IAP Enhancements #14 : Special Account Overpayment
                if (lbusSpAccntAllocDetailCalculation.icdoIapallocationDetailPersonoverview.record_freeze_flag == "Y")
                {
                    lbusSpAccntAllocDetailCalculation.icdoIapallocationDetailPersonoverview.istrSource = "Adjusted Balance";
                    lbusSpAccntAllocDetailCalculation.icdoIapallocationDetailPersonoverview.istrSSN = iclbSpAccntAllocDetailPersonOverview.FirstOrDefault().icdoIapallocationDetailPersonoverview.istrSSN;
                    lbusSpAccntAllocDetailCalculation.icdoIapallocationDetailPersonoverview.idecTotal = lbusSpAccntAllocDetailCalculation.icdoIapallocationDetailPersonoverview.alloc1 + lbusSpAccntAllocDetailCalculation.icdoIapallocationDetailPersonoverview.alloc4;
                }
                else
                {
                    lbusSpAccntAllocDetailCalculation.icdoIapallocationDetailPersonoverview.istrSource = "Withdrawal";
                    lbusSpAccntAllocDetailCalculation.icdoIapallocationDetailPersonoverview.istrSSN = iclbSpAccntAllocDetailPersonOverview.FirstOrDefault().icdoIapallocationDetailPersonoverview.istrSSN;
                    lbusSpAccntAllocDetailCalculation.icdoIapallocationDetailPersonoverview.idecTotal = lbusSpAccntAllocDetailCalculation.icdoIapallocationDetailPersonoverview.alloc1 + lbusSpAccntAllocDetailCalculation.icdoIapallocationDetailPersonoverview.alloc4;


                }
                iclbSpAccntAllocDetailPersonOverview.Add(lbusSpAccntAllocDetailCalculation);

                //lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.idecTotal = lbusIapAllocationDetailCalculation.icdoIapallocationDetailPersonoverview.idecTotal;
            }
            //  IAP Enhancements #14 : Special Account Overpayment
            iclbSpAccntAllocDetailPersonOverview = iclbSpAccntAllocDetailPersonOverview.OrderBy(item => item.icdoIapallocationDetailPersonoverview.computational_year).ThenBy(item => item.icdoIapallocationDetailPersonoverview.iblnReemploymentFlag == false ? 1 : 2).
                 ThenBy(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed" ? 1 : item.icdoIapallocationDetailPersonoverview.istrSource == "Withdrawal" ? 2 : item.icdoIapallocationDetailPersonoverview.istrSource == "Adjusted Balance" ? 3 : item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS" ? 4 : 5).ToList().ToCollection<busIapAllocationDetailCalculation>();

            LoadTotalSpAccntDetails();
        }


        public ArrayList btn_SpAccntRefreshAllocations()
        {
            ArrayList iarrError = new ArrayList();
            LoadRecalculateAndPostSpAccntAllocDetail(this.icdoPerson.person_id, iclbSpAccntAllocDetailPersonOverview.FirstOrDefault().icdoIapallocationDetailPersonoverview.istrFundType);

            iclbSpAccntAllocDetailPersonOverview.ForEach(item => item.EvaluateInitialLoadRules());
            iarrError.Add(this);
            return iarrError;
        }

        //PIR 19
        public ArrayList btn_SpAccntPostAllocations()
        {
            ArrayList iarrError = new ArrayList();
            utlError lobjError = new utlError();


            if (iclbSpAccntAllocDetailPersonOverview != null && iclbSpAccntAllocDetailPersonOverview.Count > 0
                && iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Count() > 0)
            {
                foreach (busIapAllocationDetailCalculation lcdoSpAccnDetailPersonoverview in iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference"))
                {
                    DateTime lEffectiveDate = new DateTime();
                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

                    if (lcdoSpAccnDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year == idtEffectiveDate.Year)
                    {
                        lEffectiveDate = idtEffectiveDate;
                    }
                    else
                    {
                        lEffectiveDate = busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(lcdoSpAccnDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year));
                    }

                    PostSpAccntDiffAmountIntoContribution(iclbSpAccntAllocDetailPersonOverview.FirstOrDefault().icdoIapallocationDetailPersonoverview.istrFundType, Convert.ToInt32(lcdoSpAccnDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year), lintPersonAccountId, lEffectiveDate, lcdoSpAccnDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc1,
                                                           lcdoSpAccnDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4);

                }
            }

            iclbSpAccntAllocDetailPersonOverview.ForEach(item => item.EvaluateInitialLoadRules());
            iarrError.Add(this);

            btn_SpAccntRefreshAllocations();
            return iarrError;
        }

        public void LoadTotalSpAccntDetails()
        {
            if (iclbTotalSpAccntAllocDetailPersonOverview == null)
                iclbTotalSpAccntAllocDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();

            decimal ldecAllocation4AmountComputed = 0M, ldecAlloc1AmountComputed = 0M, ldecTotalComputed = 0M;

            if (iclbSpAccntAllocDetailPersonOverview != null && iclbSpAccntAllocDetailPersonOverview.Count > 0
                && iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Count() > 0)
            {
                ldecAlloc1AmountComputed = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1);
                ldecAllocation4AmountComputed = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4);
                ldecTotalComputed = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Computed").Sum(item => item.icdoIapallocationDetailPersonoverview.idecTotal);


                if (iclbPaidSpAccntAllocDetailPersonOverview != null && iclbPaidSpAccntAllocDetailPersonOverview.Count > 0)
                {
                    foreach (busIapAllocationDetailCalculation lbusPaidSpAccntAllocDetailPersonOverview in iclbPaidSpAccntAllocDetailPersonOverview)
                    {
                        if (lbusPaidSpAccntAllocDetailPersonOverview.icdoIapallocationDetailPersonoverview.computational_year >= busConstant.GoLiveYear)
                        {
                            ldecAlloc1AmountComputed += lbusPaidSpAccntAllocDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc1;
                            ldecAllocation4AmountComputed += lbusPaidSpAccntAllocDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc4;
                            ldecTotalComputed += lbusPaidSpAccntAllocDetailPersonOverview.icdoIapallocationDetailPersonoverview.idecTotal;
                        }
                    }
                }

                if (iclbSpAccntAllocDetailComputed != null && iclbSpAccntAllocDetailComputed.Count() > 0
                    && iclbSpAccntAllocDetailComputed.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year < busConstant.GoLiveYear).Count() > 0)
                {
                    ldecAlloc1AmountComputed += iclbSpAccntAllocDetailComputed.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year < busConstant.GoLiveYear).Sum(t => t.icdoIapallocationDetailPersonoverview.alloc1);
                    ldecAllocation4AmountComputed += iclbSpAccntAllocDetailComputed.Where(t => t.icdoIapallocationDetailPersonoverview.computational_year < busConstant.GoLiveYear).Sum(t => t.icdoIapallocationDetailPersonoverview.alloc4);
                    ldecTotalComputed = ldecAlloc1AmountComputed + ldecAllocation4AmountComputed;
                }

                FillTotalSpAccntAllocDetails(icdoPerson.ssn, "Computed", 0, ldecAlloc1AmountComputed, ldecAllocation4AmountComputed, ldecTotalComputed);
            }


            if (iclbSpAccntAllocDetailPersonOverview != null && iclbSpAccntAllocDetailPersonOverview.Count > 0
                    && iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Count() > 0)
            {
                decimal ldecAllocation4AmountOpus = 0M, ldecAlloc1AmountOpus = 0M, ldecTotalOpus = 0M;
                ldecAlloc1AmountOpus = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1);
                ldecAllocation4AmountOpus = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4);
                ldecTotalOpus = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS").Sum(item => item.icdoIapallocationDetailPersonoverview.idecTotal);

                foreach (busIapAllocationDetailCalculation lbusSpAccntAllocDDetailCalculation in iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "OPUS"))
                {
                    if (iclbPaidSpAccntAllocDetailPersonOverview != null && iclbPaidSpAccntAllocDetailPersonOverview.Count > 0)
                    {
                        foreach (busIapAllocationDetailCalculation lbusPaidPaidSpAccntAllocDetailPersonOverview in iclbPaidSpAccntAllocDetailPersonOverview)
                        {
                            if (lbusSpAccntAllocDDetailCalculation.icdoIapallocationDetailPersonoverview.computational_year == lbusPaidPaidSpAccntAllocDetailPersonOverview.icdoIapallocationDetailPersonoverview.computational_year)
                            {
                                ldecAlloc1AmountOpus += lbusPaidPaidSpAccntAllocDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc1;
                                ldecAllocation4AmountOpus += lbusPaidPaidSpAccntAllocDetailPersonOverview.icdoIapallocationDetailPersonoverview.alloc4;
                                ldecTotalOpus += lbusPaidPaidSpAccntAllocDetailPersonOverview.icdoIapallocationDetailPersonoverview.idecTotal;
                            }
                        }
                    }
                }

                FillTotalSpAccntAllocDetails(icdoPerson.ssn, "OPUS", 0, ldecAlloc1AmountOpus, ldecAllocation4AmountOpus, ldecTotalOpus);
            }


            if (iclbSpAccntAllocDetailPersonOverview != null && iclbSpAccntAllocDetailPersonOverview.Count > 0
                  && iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Count() > 0)
            {
                decimal ldecAllocation4AmountDiff = 0M, ldecAlloc1AmountDiff = 0M, ldecTotalDiff = 0M;

                ldecAlloc1AmountDiff = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc1);
                ldecAllocation4AmountDiff = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.alloc4);
                ldecTotalDiff = iclbSpAccntAllocDetailPersonOverview.Where(item => item.icdoIapallocationDetailPersonoverview.istrSource == "Difference").Sum(item => item.icdoIapallocationDetailPersonoverview.idecTotal);

                FillTotalSpAccntAllocDetails(icdoPerson.ssn, "Difference", 0, ldecAlloc1AmountDiff, ldecAllocation4AmountDiff, ldecTotalDiff);
            }
        }


        public void FillSpAccntAllocationDetails(string istrSSN, string istrSource, decimal aintComputationYear, decimal adecDiffAlloc1, decimal ldecAllocation4Amount,
            decimal adecTotal)
        {
            busIapAllocationDetailCalculation lcdoSpAccntDetailPersonoverview = new busIapAllocationDetailCalculation { icdoIapallocationDetailPersonoverview = new cdoIapallocationDetailPersonoverview() };
            lcdoSpAccntDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrSSN = istrSSN;
            lcdoSpAccntDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrSource = istrSource;
            lcdoSpAccntDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year = aintComputationYear;
            lcdoSpAccntDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc1 = adecDiffAlloc1;
            lcdoSpAccntDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4 = ldecAllocation4Amount;
            lcdoSpAccntDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecTotal = adecTotal;
            iclbSpAccntAllocDetailPersonOverview.Add(lcdoSpAccntDetailPersonoverview);
        }

        public void FillTotalSpAccntAllocDetails(string istrSSN, string istrSource, decimal aintComputationYear, decimal adecDiffAlloc1,
           decimal ldecAllocation4Amount, decimal adecTotal)
        {
            busIapAllocationDetailCalculation lcdoSpAccntAllocDetailPersonoverview = new busIapAllocationDetailCalculation { icdoIapallocationDetailPersonoverview = new cdoIapallocationDetailPersonoverview() };
            lcdoSpAccntAllocDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrSSN = istrSSN;
            lcdoSpAccntAllocDetailPersonoverview.icdoIapallocationDetailPersonoverview.istrSource = istrSource;
            lcdoSpAccntAllocDetailPersonoverview.icdoIapallocationDetailPersonoverview.computational_year = aintComputationYear;
            lcdoSpAccntAllocDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc1 = adecDiffAlloc1;
            lcdoSpAccntAllocDetailPersonoverview.icdoIapallocationDetailPersonoverview.alloc4 = ldecAllocation4Amount;
            lcdoSpAccntAllocDetailPersonoverview.icdoIapallocationDetailPersonoverview.idecTotal = adecTotal;

            iclbTotalSpAccntAllocDetailPersonOverview.Add(lcdoSpAccntAllocDetailPersonoverview);
        }

        #endregion


        public bool CheckIfManager()
        {
            busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
            if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
            {
                return true;
            }
            return false;
        }


        public DataTable CreateTableDesignForRecalculateIAPAlloc()
        {
            rptRecalculateIAPAlloc = new DataTable();
            rptRecalculateIAPAlloc.Columns.Add("Source", typeof(string));
            rptRecalculateIAPAlloc.Columns.Add("Year", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("YTDHours", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("YTDHoursA2", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("alloc1", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("alloc2", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("alloc2_invt", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("alloc2_frft", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("alloc3", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("alloc4", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("alloc4_invt", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("alloc4frft", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("Computealloc5", typeof(string));
            rptRecalculateIAPAlloc.Columns.Add("alloc5", typeof(decimal));
            rptRecalculateIAPAlloc.Columns.Add("total", typeof(decimal));
            //rptRecalculateIAPAlloc.Columns.Add("MPID", typeof(string));
            //rptRecalculateIAPAlloc.Columns.Add("FullName", typeof(string));
            //rptRecalculateIAPAlloc.Columns.Add("SSN", typeof(string));
            //rptRecalculateIAPAlloc.Columns.Add("DOB", typeof(string));
            return rptRecalculateIAPAlloc;
        }

        public DataTable CreateTableDesignForRecalculateTotalIAPAlloc()
        {
            rptRecalculateTotalIAPAlloc = new DataTable();
            rptRecalculateTotalIAPAlloc.Columns.Add("TSource", typeof(string));
            rptRecalculateTotalIAPAlloc.Columns.Add("Talloc1", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("Talloc2", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("Talloc2_invt", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("Talloc2_frft", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("Talloc3", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("Talloc4", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("Talloc4_invt", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("Talloc4frft", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("Talloc5", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("Ttotal", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("TOverriddentTotal", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("TMPID", typeof(string));
            rptRecalculateTotalIAPAlloc.Columns.Add("TFullName", typeof(string));
            rptRecalculateTotalIAPAlloc.Columns.Add("TSSN", typeof(string));
            rptRecalculateTotalIAPAlloc.Columns.Add("TDOB", typeof(string));
            rptRecalculateTotalIAPAlloc.Columns.Add("THRU79", typeof(decimal));
            rptRecalculateTotalIAPAlloc.Columns.Add("TRecalculateFlag", typeof(string));
            return rptRecalculateTotalIAPAlloc;
        }

        private void LoadIAPRecalculationCopy()
        {
            DataTable ldtIapRecalculationCopy = busBase.Select<cdoIapRecalculationCopy>(new string[1] { enmIapRecalculationCopy.person_id.ToString() }, new object[1] { icdoPerson.person_id }, null, null);
            busBase lobjBase = new busBase();
            iclbIAPRecalculationCopy = lobjBase.GetCollection<busIapRecalculationCopy>(ldtIapRecalculationCopy, "icdoIapRecalculationCopy");


        }
        //LineNo 22 # IAP Enhancement project-
        public bool CheckIfFactorAvailableForIapAllocation()
        {
            bool lblnFactorPresent = false;
            busIAPAllocationHelper lobjIAPHelper = new busIAPAllocationHelper();
            if (this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).Count() > 0)
            {
                DateTime ldtAwrded = this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitApplication.awarded_on_date;
                DateTime ldtRet = this.ibusPersonOverview.iclbBenefitApplication.Where(item => item.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && item.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitApplication.retirement_date;
                int lintQuarterAsOfAwrdedOnDate = 0; bool lblnFactorAwardedOndate = true; decimal ldecFactorAsOfAwardedOnDate = decimal.Zero;
                lintQuarterAsOfAwrdedOnDate = busGlobalFunctions.GetPreviousQuarter(ldtAwrded);

                int lintQuarterAsOfRetirementDate = 0; bool lblnFactorRetirementDate = true; decimal ldecFactorAsOfRetirementDate = decimal.Zero;
                lintQuarterAsOfRetirementDate = busGlobalFunctions.GetPreviousQuarter(ldtRet);
                if (lintQuarterAsOfAwrdedOnDate != 0)
                {
                    lblnFactorAwardedOndate = false;
                    lobjIAPHelper.CalculateAllocation1Amount(ldtAwrded.Year, decimal.Zero, lintQuarterAsOfAwrdedOnDate, ref ldecFactorAsOfAwardedOnDate);

                    if (ldecFactorAsOfAwardedOnDate != decimal.Zero)
                    {
                        lblnFactorAwardedOndate = true;
                    }
                }
                if (lintQuarterAsOfRetirementDate != 0)
                {
                    lblnFactorRetirementDate = false;
                    lobjIAPHelper.CalculateAllocation1Amount(ldtRet.Year, decimal.Zero, lintQuarterAsOfRetirementDate, ref ldecFactorAsOfRetirementDate);

                    if (ldecFactorAsOfRetirementDate != decimal.Zero)
                    {
                        lblnFactorRetirementDate = true;
                    }
                }
                if (lblnFactorAwardedOndate && lblnFactorRetirementDate)
                {
                    lblnFactorPresent = true;
                }

            }
            return lblnFactorPresent;
        }
        //  IAP Enhancements #14 : Special Account Overpayment
        public void GetBalanceforOverpaymentAdjustment(int aintPersonAccountId)
        {
            DataTable ldtbBalanceforOverpaymentAdjustment = busBase.Select("cdoPersonAccountRetirementContribution.GetBalanceforOverpaymentAjustment", new object[1] { aintPersonAccountId });

            if (ldtbBalanceforOverpaymentAdjustment.Rows.Count > 0)
            {
                ldecIapBalanceAmount = Convert.ToDecimal(ldtbBalanceforOverpaymentAdjustment.Rows[0]["IAP_BALANCE_AMOUNT"]);
                ldecLocal52BalanceAmount = Convert.ToDecimal(ldtbBalanceforOverpaymentAdjustment.Rows[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]);
                ldecLocal161BalanceAmount = Convert.ToDecimal(ldtbBalanceforOverpaymentAdjustment.Rows[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]);

                if (ldecIapBalanceAmount > 0 && ldecLocal52BalanceAmount < 0)
                {
                    isblnOverpaid = true;

                }
                else if (ldecIapBalanceAmount > 0 && ldecLocal161BalanceAmount < 0)
                {
                    isblnOverpaid = true;
                }
                else if (ldecIapBalanceAmount < 0 && ldecLocal161BalanceAmount > 0)
                {
                    isblnOverpaid = true;

                }
                else if (ldecIapBalanceAmount < 0 && ldecLocal52BalanceAmount > 0)
                {
                    isblnOverpaid = true;
                }

            }

        }
       // IAP Enhancements #14 : Special Account Overpayment
        public ArrayList btn_AdjustOverPayment()
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            ibusPersonOverview = new busPersonOverview();
            if (ibusPersonOverview.FindPerson(this.icdoPerson.person_id))
            {
                ibusPersonOverview.LoadBenefitApplication();

            }


            if (!ibusPersonOverview.iclbBenefitApplication.Where(x => x.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).IsNullOrEmpty())
            {

                LoadBenefitApplicationWithdrawalDates(ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).LastOrDefault().icdoBenefitApplication.person_id);

                //if ((ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && (i.icdoBenefitApplication.benefit_type_value == "RTMT" || i.icdoBenefitApplication.benefit_type_value == "DSBL")).Count() > 0) &&
                //     (ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && i.icdoBenefitApplication.benefit_type_value == "WDRL").Count() > 0) &&
                //    (ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && (i.icdoBenefitApplication.benefit_type_value == "DDPR" || i.icdoBenefitApplication.benefit_type_value == "DDPT")).Count() > 0))
                //{
                //    // this.LoadBenefitApplicationWithdrawalDates(ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && i.icdoBenefitApplication.benefit_type_value == "WDRL").LastOrDefault().icdoBenefitApplication.person_id);
                //    // leffectiveDate = ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && i.icdoBenefitApplication.benefit_type_value == "RTMT").LastOrDefault().icdoBenefitApplication.retirement_date;
                //    LoadBenefitApplicationWithdrawalDates(ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).LastOrDefault().icdoBenefitApplication.person_id);
                //}
                //else if ((ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && i.icdoBenefitApplication.benefit_type_value == "WDRL").Count() > 0) &&
                //    (ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && (i.icdoBenefitApplication.benefit_type_value == "DDPR" || i.icdoBenefitApplication.benefit_type_value == "DDPT")).Count() > 0))
                //{
                //    //leffectiveDate = ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && i.icdoBenefitApplication.benefit_type_value == "WDRL").LastOrDefault().icdoBenefitApplication.withdrawal_date;
                //    LoadBenefitApplicationWithdrawalDates(ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).LastOrDefault().icdoBenefitApplication.person_id);

                //}
                //else if ((ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && (i.icdoBenefitApplication.benefit_type_value == "DDPR" || i.icdoBenefitApplication.benefit_type_value == "DDPT")).Count() > 0))
                //{
                //    // leffectiveDate = ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && (i.icdoBenefitApplication.benefit_type_value == "DDPR" || i.icdoBenefitApplication.benefit_type_value == "DDPT")).LastOrDefault().icdoBenefitApplication.retirement_date;
                //    LoadBenefitApplicationWithdrawalDates(ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).LastOrDefault().icdoBenefitApplication.person_id);

                //}

                //else if ((ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && (i.icdoBenefitApplication.benefit_type_value == "RTMT" || i.icdoBenefitApplication.benefit_type_value == "DSBL")).Count() > 0) &&
                //    (ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && i.icdoBenefitApplication.benefit_type_value == "WDRL").Count() > 0))
                //{
                //    LoadBenefitApplicationWithdrawalDates(ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).LastOrDefault().icdoBenefitApplication.person_id);
                //    //  this.LoadBenefitApplicationWithdrawalDates(ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && i.icdoBenefitApplication.benefit_type_value == "WDRL").LastOrDefault().icdoBenefitApplication.person_id);
                //    //  leffectiveDate = ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && (i.icdoBenefitApplication.benefit_type_value == "RTMT" || i.icdoBenefitApplication.benefit_type_value == "DSBL")).LastOrDefault().icdoBenefitApplication.retirement_date;
                //}
                //else if (ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && (i.icdoBenefitApplication.benefit_type_value == "RTMT" || i.icdoBenefitApplication.benefit_type_value == "DSBL")).Count() > 0)
                //{
                //    //  leffectiveDate = ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && (i.icdoBenefitApplication.benefit_type_value == "RTMT" || i.icdoBenefitApplication.benefit_type_value == "DSBL")).LastOrDefault().icdoBenefitApplication.retirement_date;
                //    LoadBenefitApplicationWithdrawalDates(ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).LastOrDefault().icdoBenefitApplication.person_id);
                //}
                //else
                //{
                //    //this.LoadBenefitApplicationWithdrawalDates(ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID && i.icdoBenefitApplication.benefit_type_value == "WDRL").LastOrDefault().icdoBenefitApplication.person_id);
                //    LoadBenefitApplicationWithdrawalDates(ibusPersonOverview.iclbBenefitApplication.Where(i => i.icdoBenefitApplication.iintPlanId == busConstant.IAP_PLAN_ID).LastOrDefault().icdoBenefitApplication.person_id);
                //}


            }
            else
            {
                LoadRetirementContributionsbyAccountId(lintPersonAccountId);
                leffectiveDate = iclbPersonAccountRetirementYearlyAllocation.Where(i => i.icdoPersonAccountRetirementContribution.transaction_type_value == "YREA").Max(z => z.icdoPersonAccountRetirementContribution.effective_date);
                lWDRLeffectiveDate = iclbPersonAccountRetirementYearlyAllocation.Where(i => i.icdoPersonAccountRetirementContribution.transaction_type_value == "YREA").Max(z => z.icdoPersonAccountRetirementContribution.effective_date);

            }



            if (isblnOverpaid)
            {
                busPersonAccountRetirementContribution lobjRetrContribution;
                //block to insert the allocation 1 difference amount            
                lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };



                if (ldecIapBalanceAmount > 0 && ldecLocal52BalanceAmount < 0)
                {
                    if (ldecIapBalanceAmount >= ldecLocal52BalanceAmount * -1)
                    {
                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, leffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(leffectiveDate.Year), adecIAPBalanceAmount: ldecLocal52BalanceAmount,
                   astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, lWDRLeffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(lWDRLeffectiveDate.Year), adec52SplAccountBalance: ldecLocal52BalanceAmount * -1,
                               astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                    }
                    else
                    {
                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, leffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(leffectiveDate.Year), adecIAPBalanceAmount: ldecIapBalanceAmount,
                    astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, lWDRLeffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(lWDRLeffectiveDate.Year), adec52SplAccountBalance: -ldecIapBalanceAmount,
                               astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                    }

                }
                else if (ldecIapBalanceAmount > 0 && ldecLocal161BalanceAmount < 0)
                {
                    if (ldecIapBalanceAmount >= ldecLocal161BalanceAmount * -1)
                    {
                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, leffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(leffectiveDate.Year), adecIAPBalanceAmount: ldecLocal161BalanceAmount,
                    astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, lWDRLeffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(lWDRLeffectiveDate.Year), adec161SplAccountBalance: ldecLocal161BalanceAmount * -1,
                              astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");
                    }
                    else
                    {
                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, leffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(leffectiveDate.Year), adecIAPBalanceAmount: ldecIapBalanceAmount,
                    astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, lWDRLeffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(lWDRLeffectiveDate.Year), adec161SplAccountBalance: -ldecIapBalanceAmount,
                              astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");


                    }
                }
                else if (ldecIapBalanceAmount < 0 && ldecLocal52BalanceAmount > 0)
                {
                    if (ldecLocal52BalanceAmount >= ldecIapBalanceAmount * -1)
                    {
                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, leffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(leffectiveDate.Year), adecIAPBalanceAmount: ldecIapBalanceAmount * -1,
                   astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, lWDRLeffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(lWDRLeffectiveDate.Year), adec52SplAccountBalance: ldecIapBalanceAmount,
                              astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");
                    }
                    else
                    {

                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, leffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(leffectiveDate.Year), adecIAPBalanceAmount: ldecLocal52BalanceAmount,
                 astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, lWDRLeffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(lWDRLeffectiveDate.Year), adec52SplAccountBalance: -ldecLocal52BalanceAmount,
                              astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                    }

                }
                else if (ldecIapBalanceAmount < 0 && ldecLocal161BalanceAmount > 0)
                {
                    if (ldecLocal161BalanceAmount >= ldecIapBalanceAmount * -1)
                    {
                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, leffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(leffectiveDate.Year), adecIAPBalanceAmount: ldecIapBalanceAmount * -1,
                   astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, lWDRLeffectiveDate, DateTime.Now, aintComputationYear:  Convert.ToInt32(lWDRLeffectiveDate.Year), adec161SplAccountBalance: ldecIapBalanceAmount,
                              astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");
                    }
                    else
                    {
                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, leffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(leffectiveDate.Year), adecIAPBalanceAmount: ldecLocal161BalanceAmount,
                    astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                        lobjRetrContribution.InsertPersonAccountRetirementContirbution(lintPersonAccountId, lWDRLeffectiveDate, DateTime.Now, aintComputationYear: Convert.ToInt32(lWDRLeffectiveDate.Year), adec161SplAccountBalance: -ldecLocal161BalanceAmount,
                              astrTransactionType: busConstant.RCTransactionTypeAdjustment, astrContributionType: busConstant.RCContributionTypeAllocation1, astrRecordFreezeFlag: "Y");

                    }

                }
            }
            return iarrErrors;
        }
      //  IAP Enhancements #14 : Special Account Overpayment
        public void LoadRetirementContributionsbyAccountId(int aintPersonAccountId)
        {
            DataTable ldtbList = busBase.Select("cdoPersonAccountRetirementContribution.GetRetirementContributionbyAccountId", new object[1] { aintPersonAccountId });
            iclbPersonAccountRetirementYearlyAllocation = GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");
        }
        //  IAP Enhancements #14 : Special Account Overpayment
        public void LoadBenefitApplicationWithdrawalDates(int person_id)
        {
            lstrSplAccntFlag = "";
            DataTable ldtbBenefitApplicationDetail = Select("cdoBenefitApplication.LoadBenefitApplicationWithdrawalDates", new object[1] { person_id });
            // iclbbusBenefitApplicationDetailID = GetCollection<busBenefitApplicationDetail>(ldtbBenefitApplicationDetail, "icdoBenefitApplicationDetail");
            if (ldtbBenefitApplicationDetail.Rows.Count > 0)
            {
                foreach (DataRow ldrRow in ldtbBenefitApplicationDetail.Rows)
                {
                    if (Convert.ToString(ldrRow["L161_SPL_ACC_FLAG"]) == "Y" || Convert.ToString(ldrRow["L52_SPL_ACC_FLAG"]) == "Y")
                    {
                        lstrSplAccntFlag = "Y";
                        if(ldrRow["idtWithdrawalDate"] != DBNull.Value)
                        {
                            lWDRLeffectiveDate = Convert.ToDateTime(ldrRow["idtWithdrawalDate"]);

                        }else if (ldrRow["idtRetirementDate"] != DBNull.Value)
                        {
                            lWDRLeffectiveDate = Convert.ToDateTime(ldrRow["idtRetirementDate"]);
                        }
                       
                    }
                    else
                    {
                        if (ldrRow["idtWithdrawalDate"] != DBNull.Value)
                        {
                            leffectiveDate = Convert.ToDateTime(ldrRow["idtWithdrawalDate"]);

                        }
                        else if (ldrRow["idtRetirementDate"] != DBNull.Value)
                        {
                            leffectiveDate = Convert.ToDateTime(ldrRow["idtRetirementDate"]);
                        }
                    }

                }

                 if (lWDRLeffectiveDate == DateTime.MinValue)
                 {
                    LoadRetirementContributionsbyAccountId(lintPersonAccountId);
                    lWDRLeffectiveDate = iclbPersonAccountRetirementYearlyAllocation.Where(i => i.icdoPersonAccountRetirementContribution.transaction_type_value == "YREA").Max(z => z.icdoPersonAccountRetirementContribution.effective_date);

                }
                if (leffectiveDate == DateTime.MinValue)
                {
                    LoadRetirementContributionsbyAccountId(lintPersonAccountId);
                    leffectiveDate = iclbPersonAccountRetirementYearlyAllocation.Where(i => i.icdoPersonAccountRetirementContribution.transaction_type_value == "YREA").Max(z => z.icdoPersonAccountRetirementContribution.effective_date);


                }

            }

        }

    }
}
