#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using System.Linq;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using MPIPHP.DataObjects;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.Interface;

#endregion

namespace MPIPHP.BusinessTier
{
    public class srvCalculation : srvMPIPHP
    {
        public srvCalculation()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public busBenefitCalculationHeader FindBenefitCalculationHeader(int aintbenefitcalculationheaderid)
        {
            busBenefitCalculationHeader lobjBenefitCalculationHeader = new busBenefitCalculationHeader();
            if (lobjBenefitCalculationHeader.FindBenefitCalculationHeader(aintbenefitcalculationheaderid))
            {
            }

            return lobjBenefitCalculationHeader;
        }

        public busBenefitCalculationDetail FindBenefitCalculationDetail(int aintbenefitcalculationdetailid)
        {
            busBenefitCalculationDetail lobjBenefitCalculationDetail = new busBenefitCalculationDetail();
            if (lobjBenefitCalculationDetail.FindBenefitCalculationDetail(aintbenefitcalculationdetailid))
            {
                lobjBenefitCalculationDetail.LoadBenefitCalculationHeader();
            }

            return lobjBenefitCalculationDetail;
        }

        public busBenefitCalculationDetailLookup LoadBenefitCalculationDetails(DataTable adtbSearchResult)
        {
            busBenefitCalculationDetailLookup lobjBenefitCalculationDetailLookup = new busBenefitCalculationDetailLookup();
            lobjBenefitCalculationDetailLookup.LoadBenefitCalculationDetails(adtbSearchResult);
            return lobjBenefitCalculationDetailLookup;
        }


        public busBenefitCalculationDetail LoadQDROOffsetDetails(int aintParticiapantID, int aintPlanId, string astrCalculationType,string astrBenefitTypeValue,
        string astrEEFlag, string astrUVHPFlag, string astrL52SplAccFlag, string astrL161SplAccFlag)
        {
            busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail();
            lbusBenefitCalculationDetail = lbusBenefitCalculationDetail.GetQDROOffsetDetails(aintParticiapantID, aintPlanId, astrCalculationType,astrBenefitTypeValue,
                    astrEEFlag, astrUVHPFlag, astrL52SplAccFlag, astrL161SplAccFlag);
            return lbusBenefitCalculationDetail;
        }

        //public busBenefitCalculationYearlyDetail FindBenefitCalculationYearlyDetail(int aintbenefitcalculationyearlydetailid)
        //{
        //    busBenefitCalculationYearlyDetail lobjBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail();
        //    if (lobjBenefitCalculationYearlyDetail.FindBenefitCalculationYearlyDetail(aintbenefitcalculationyearlydetailid))
        //    {
        //        lobjBenefitCalculationYearlyDetail.LoadBenefitCalculationDetail();
        //    }

        //    return lobjBenefitCalculationYearlyDetail;
        //}

        public busBenefitCalculationOptions FindBenefitCalculationOptions(int aintbenefitcalculationoptionid)
        {
            busBenefitCalculationOptions lobjBenefitCalculationOptions = new busBenefitCalculationOptions();
            if (lobjBenefitCalculationOptions.FindBenefitCalculationOptions(aintbenefitcalculationoptionid))
            {
                lobjBenefitCalculationOptions.LoadBenefitCalculationDetail();
            }

            return lobjBenefitCalculationOptions;
        }


        public busBenefitCalculationHeaderLookup LoadBenefitCalculationHeaders(DataTable adtbSearchResult)
        {
            busBenefitCalculationHeaderLookup lobjBenefitCalculationHeaderLookup = new busBenefitCalculationHeaderLookup();
            lobjBenefitCalculationHeaderLookup.LoadBenefitCalculationHeaders(adtbSearchResult);
            return lobjBenefitCalculationHeaderLookup;
        }

        #region Validate New
        //FM upgrade: 6.0.0.31 changes - public to protected
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = null;
            iobjPassInfo.iconFramework.Open();
            if (astrFormName == busConstant.BenefitCalculation.BENEFIT_CALCULATION_LOOKUP)
            {
                busBenefitCalculationHeader lbusBenefitCalculationHeader = new busBenefitCalculationHeader();
                larrErrors = lbusBenefitCalculationHeader.ValidateNew(ahstParam);
            }
            if (astrFormName == busConstant.DRO_CALCULATION_LOOKUP)
            {
                busQdroCalculationHeader lbusQdroCalculationHeader = new busQdroCalculationHeader();
                larrErrors = lbusQdroCalculationHeader.ValidateNew(ahstParam);
            }
            return larrErrors;
        }

        //FM upgrade: 6.0.0.31 changes - public to protected
        protected override ArrayList ValidateNewChild(string astrFormName, object aobjParentObject, Type atypBusObject, Hashtable ahstParams)
        {
            ArrayList larrErrors = new ArrayList();

            if (astrFormName == "wfmBenefitCalculationPostRetirementDeathMaintenance")
            {
                busBenefitCalculationPostRetirementDeath lbusBenefitCalculationPostRetirementDeath = aobjParentObject as busBenefitCalculationPostRetirementDeath;
                if (atypBusObject.Name == "busBenefitCalculationDetail")
                {
                    larrErrors = lbusBenefitCalculationPostRetirementDeath.CheckIfSpecialAccountAdded(lbusBenefitCalculationPostRetirementDeath, ahstParams,ref larrErrors);
                }
            }
            return larrErrors;
        }

        protected override ArrayList ValidateGridUpdateChild(string astrFormName, object aobjParentObject, object aobjChildObject, Hashtable ahstParams)
        {
            ArrayList iarrResult = new ArrayList();
            utlError lobjError = null;
            busMainBase lbusMainBase = new busMainBase();

            if (astrFormName == busConstant.DRO_CALCULATION_MAINTENANCE)
            {
                string lstrBenefitCalculationBasedOn = string.Empty; int lintBalanceAsofYear = 0;
                decimal ldectotalService = busConstant.ZERO_DECIMAL, ldecOverridenCommunityPropertyPeriod = busConstant.ZERO_DECIMAL,
                         ldecOverridenTotalService = busConstant.ZERO_DECIMAL, ldecFlatPercent = busConstant.ZERO_DECIMAL, ldecFlatAmount = busConstant.ZERO_DECIMAL,
                         ldecOverridenAccruedBenefitAmt = busConstant.ZERO_DECIMAL, ldecAccruedBenefitAmt = busConstant.ZERO_DECIMAL,
                         ldecCommunityPropertyPeriod = busConstant.ZERO_DECIMAL, ldecQdroPercent = busConstant.ZERO_DECIMAL;
                DateTime ldtNetInvestmentFromDate = new DateTime();
                DateTime ldtNetInvestmentToDate = new DateTime();
                DateTime ldtCommunityPropertyEndDate = new DateTime();

                lstrBenefitCalculationBasedOn = Convert.ToString(ahstParams["icdoQdroCalculationDetail.benefit_calculation_based_on_value"]);
                ldtNetInvestmentFromDate = Convert.ToDateTime(ahstParams["icdoQdroCalculationDetail.net_investment_from_date"]);
                ldtNetInvestmentToDate = Convert.ToDateTime(ahstParams["icdoQdroCalculationDetail.net_investment_to_date"]);
                ldtCommunityPropertyEndDate = Convert.ToDateTime(ahstParams["icdoQdroCalculationDetail.community_property_end_date"]);

                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoQdroCalculationDetail.total_service"])))
                {
                    ldectotalService = Convert.ToDecimal(ahstParams["icdoQdroCalculationDetail.total_service"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoQdroCalculationDetail.overriden_total_value"])))
                {
                    ldecOverridenTotalService = Convert.ToDecimal(ahstParams["icdoQdroCalculationDetail.overriden_total_value"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoQdroCalculationDetail.community_property_service"])))
                {
                    ldecCommunityPropertyPeriod = Convert.ToDecimal(ahstParams["icdoQdroCalculationDetail.community_property_service"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoQdroCalculationDetail.overriden_community_property_period"])))
                {
                    ldecOverridenCommunityPropertyPeriod = Convert.ToDecimal(ahstParams["icdoQdroCalculationDetail.overriden_community_property_period"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoQdroCalculationDetail.balance_as_of_plan_year"])))
                {
                    lintBalanceAsofYear = Convert.ToInt32(ahstParams["icdoQdroCalculationDetail.balance_as_of_plan_year"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoQdroCalculationDetail.flat_percent"])))
                {
                    ldecFlatPercent = Convert.ToDecimal(ahstParams["icdoQdroCalculationDetail.flat_percent"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoQdroCalculationDetail.flat_amount"])))
                {
                    ldecFlatAmount = Convert.ToDecimal(ahstParams["icdoQdroCalculationDetail.flat_amount"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoQdroCalculationDetail.qdro_percent"])))
                {
                    ldecQdroPercent = Convert.ToDecimal(ahstParams["icdoQdroCalculationDetail.qdro_percent"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoQdroCalculationDetail.accrued_benefit_amt"])))
                {
                    ldecOverridenAccruedBenefitAmt = Convert.ToDecimal(ahstParams["icdoQdroCalculationDetail.accrued_benefit_amt"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoQdroCalculationDetail.early_reduced_benefit_amount"])))
                {
                    ldecAccruedBenefitAmt = Convert.ToDecimal(ahstParams["icdoQdroCalculationDetail.early_reduced_benefit_amount"]);
                }

                if (ldecFlatPercent != 0 && ldecFlatPercent > 100)
                {
                    lobjError = lbusMainBase.AddError(1121, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (ldecQdroPercent != 0 && ldecQdroPercent > 100)
                {
                    lobjError = lbusMainBase.AddError(5475, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if ((ldecFlatPercent != 0 && ldecFlatPercent < 0) || (ldecQdroPercent != 0 && ldecQdroPercent < 0))
                {
                    lobjError = lbusMainBase.AddError(5059, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (ldecFlatAmount == 0 && ldecFlatPercent == 0 && ldecQdroPercent == 0)
                {
                    lobjError = lbusMainBase.AddError(5490, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if ((ldecOverridenAccruedBenefitAmt != 0 || ldecAccruedBenefitAmt != 0))
                {
                    busQdroCalculationHeader lbusQdroCalculationHeader = aobjParentObject as busQdroCalculationHeader;

                    decimal ldecAlternatePayeeFraction = 0, ldecTotalHoursWorked = 0, ldecHoursWorkedBetweenDateOfMArrAndSep = 0, ldecAltPayeeAmtBeforeCon = 0;
                    if (ldecOverridenCommunityPropertyPeriod != 0)
                        ldecHoursWorkedBetweenDateOfMArrAndSep = ldecOverridenCommunityPropertyPeriod;
                    else
                        ldecHoursWorkedBetweenDateOfMArrAndSep = ldecCommunityPropertyPeriod;

                    if (ldecOverridenTotalService != 0)
                        ldecTotalHoursWorked = ldecOverridenTotalService;
                    else
                        ldecTotalHoursWorked = ldectotalService;

                    if (ldecTotalHoursWorked != 0)
                    {
                        ldecAlternatePayeeFraction = Math.Round(((ldecHoursWorkedBetweenDateOfMArrAndSep / ldecTotalHoursWorked) * ldecQdroPercent) / 100, 3);
                        if (ldecOverridenAccruedBenefitAmt != 0)
                        {
                            ldecAltPayeeAmtBeforeCon = lbusQdroCalculationHeader.ibusCalculation.CalculateBenefitAmtBeforeConversion(ldecOverridenAccruedBenefitAmt, ldecAlternatePayeeFraction,
                                ldecFlatAmount, ldecFlatPercent);
                            if (ldecAltPayeeAmtBeforeCon > ldecOverridenAccruedBenefitAmt)
                            {
                                lobjError = lbusMainBase.AddError(5474, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                        }
                        else
                        {
                            ldecAltPayeeAmtBeforeCon = lbusQdroCalculationHeader.ibusCalculation.CalculateBenefitAmtBeforeConversion(ldecAccruedBenefitAmt, ldecAlternatePayeeFraction,
                              ldecFlatAmount, ldecFlatPercent);

                            if (ldecAltPayeeAmtBeforeCon > ldecAccruedBenefitAmt)
                            {
                                lobjError = lbusMainBase.AddError(5474, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                        }
                    }
                }
                if ((lstrBenefitCalculationBasedOn == busConstant.BenefitCalculation.CALCULATION_BASED_ON_DAYS ||
                        lstrBenefitCalculationBasedOn == busConstant.BenefitCalculation.CALCULATION_BASED_ON_MONTHS) &&
                        (ldecOverridenCommunityPropertyPeriod == 0 || ldecOverridenTotalService == 0))
                {
                    lobjError = lbusMainBase.AddError(5416, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (ldecOverridenTotalService < ldecOverridenCommunityPropertyPeriod)
                {
                    lobjError = lbusMainBase.AddError(5417, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate < ldtNetInvestmentFromDate))
                {
                    lobjError = lbusMainBase.AddError(5442, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (lintBalanceAsofYear != 0 && ldtNetInvestmentFromDate != DateTime.MinValue && (lintBalanceAsofYear > ldtNetInvestmentFromDate.Year))
                {
                    lobjError = lbusMainBase.AddError(5443, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (ldtNetInvestmentFromDate != DateTime.MinValue && (ldtNetInvestmentFromDate.Day != 1 || ldtNetInvestmentFromDate.Month != 1))
                {
                    lobjError = lbusMainBase.AddError(5444, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (ldtCommunityPropertyEndDate != DateTime.MinValue && ldtCommunityPropertyEndDate > DateTime.Now)
                {
                    lobjError = lbusMainBase.AddError(5447, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if ((ldecOverridenAccruedBenefitAmt != 0 && ldecOverridenAccruedBenefitAmt < ldecFlatAmount))
                {
                    lobjError = lbusMainBase.AddError(5449, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                else if (ldecOverridenAccruedBenefitAmt == 0 && ldecAccruedBenefitAmt < ldecFlatAmount)
                {
                    lobjError = lbusMainBase.AddError(5449, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 3 && ldtNetInvestmentToDate.Day != 31))
                {
                    lobjError = lbusMainBase.AddError(5445, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 6 && ldtNetInvestmentToDate.Day != 30))
                {
                    lobjError = lbusMainBase.AddError(5445, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 9 && ldtNetInvestmentToDate.Day != 30))
                {
                    lobjError = lbusMainBase.AddError(5445, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 12 && ldtNetInvestmentToDate.Day != 31))
                {
                    lobjError = lbusMainBase.AddError(5445, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 1 || ldtNetInvestmentToDate.Month == 2 ||
                    ldtNetInvestmentToDate.Month == 4 || ldtNetInvestmentToDate.Month == 5 || ldtNetInvestmentToDate.Month == 7 || ldtNetInvestmentToDate.Month == 8 ||
                    ldtNetInvestmentToDate.Month == 10 || ldtNetInvestmentToDate.Month == 11))
                {
                    lobjError = lbusMainBase.AddError(5445, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
            }
            return base.ValidateGridUpdateChild(astrFormName, aobjParentObject, aobjChildObject, ahstParams);
        }
       #endregion

        public busBenefitCalculationRetirement NewBenefitCalculationRetirement(string astr_person_mpi_id, string astr_benefit_type, int aint_plan_id) //, string astr_calculation_type_value
        {
            busBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            lbusBenefitCalculationRetirement.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationRetirement.ibusPerson.FindPerson(astr_person_mpi_id);
            lbusBenefitCalculationRetirement.ibusPerson.LoadPersonAccounts();

            //PIR 1035
            busCalculation lbusCalculation = new busCalculation();
            int lintAccountId = 0;

            decimal ldecNormalRetirementAge = lbusCalculation.GetNormalRetirementAge(aint_plan_id);           
            DateTime ldtNormalRetirementDate = lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(ldecNormalRetirementAge));
           
		   //Ticket - 69718
            if (lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount
                         .Where(t => t.icdoPersonAccount.plan_id == aint_plan_id).Count() > 0)
                lintAccountId = lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount
                    .Where(t => t.icdoPersonAccount.plan_id == aint_plan_id).FirstOrDefault().icdoPersonAccount.person_account_id;

            DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });

            if (ldtbPersonAccountEligibility != null && ldtbPersonAccountEligibility.Rows.Count > 0 &&
                      Convert.ToString(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]).IsNotNullOrEmpty())
            {
                if (Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]) > ldtNormalRetirementDate)
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate = Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]);
                else
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate;

                if (ldtNormalRetirementDate.Day != 1)
                {
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate.AddMonths(1).GetFirstDayofMonth();
                }
            }

            lbusBenefitCalculationRetirement.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            // Initial Setup for Checking Eligbility
            lbusBenefitCalculationRetirement.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationRetirement.ibusPerson;
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount;

            //PIR-799 For (coming from workflow -> checkout ->) if retirement application exists -> take latest application retirement date
            DataTable ldtbGetRTMApplication = busBase.Select("cdoBenefitApplication.GetLatestRetirementApplication", new object[1] { astr_person_mpi_id });
            if (ldtbGetRTMApplication != null && ldtbGetRTMApplication.Rows.Count > 0)
            {
                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date = Convert.ToDateTime(ldtbGetRTMApplication.Rows[0]["RETIREMENT_DATE"]);
                lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date;
            }
            else
            {
                lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            }

            lbusBenefitCalculationRetirement.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)

            lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();

            //IN NEW MODE WHATEVER VALUES WE KNOW WE COULD FILL THOSE ATLEAST WHILE COMING TO THE MNTN SCREEN
            lbusBenefitCalculationRetirement.PopulateInitialDataBenefitCalculationHeader(lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.person_id,
                                                                                        busConstant.ZERO_INT, busConstant.ZERO_INT, astr_benefit_type, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                                                                                        DateTime.MinValue,busConstant.ZERO_DECIMAL,aint_plan_id);
            lbusBenefitCalculationRetirement.EvaluateInitialLoadRules();

            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
            lbusBenefitCalculationRetirement.iarrChangeLog.Add(lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader);

            return lbusBenefitCalculationRetirement;
        }

        public busBenefitCalculationRetirement FindBenefitCalculationRetirement(int aintBenefitCalculationId)
        {
            busBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busBenefitCalculationRetirement();
            if (lbusBenefitCalculationRetirement.FindBenefitCalculationHeader(aintBenefitCalculationId))
            {
                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date);

                DataTable ldtbPersonMPID = busBase.Select("cdoPerson.GetPersonMPID", new object[1] { lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_id });
                if (ldtbPersonMPID.Rows.Count > 0)
                {
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrSurvivorMPID = ldtbPersonMPID.Rows[0][enmPerson.mpi_person_id.ToString()].ToString();
                }

                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.PopulateDescriptions();
                lbusBenefitCalculationRetirement.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();

                if(lbusBenefitCalculationRetirement.ibusPerson.FindPerson(lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.person_id))
                {
                    lbusBenefitCalculationRetirement.ibusPerson.LoadPacketCorrespondences();
                }

                lbusBenefitCalculationRetirement.ibusPerson.LoadPersonAccounts();
                lbusBenefitCalculationRetirement.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                if (!lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    lbusBenefitCalculationRetirement.LoadAllRetirementContributions(lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    lbusBenefitCalculationRetirement.LoadAllRetirementContributions(null);
                }
                lbusBenefitCalculationRetirement.LoadBenefitCalculationDetails();
                lbusBenefitCalculationRetirement.LoadDisabilityRetireeIncreases();

                // Initial Setup for Checking Eligbility
                lbusBenefitCalculationRetirement.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationRetirement.ibusPerson;
                lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount;
                lbusBenefitCalculationRetirement.ibusBenefitApplication.FindBenefitApplication(lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.benefit_application_id);
                lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date;
                lbusBenefitCalculationRetirement.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.idtDateofBirth, lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date);

                lbusBenefitCalculationRetirement.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)

                if (lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Count > 1)
                {
                    if (lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                        lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                    }
                    else
                    {
                        if (lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                    }
                }
                else if (lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Count > 0)
                {
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                }

                if (lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Count > 0)
                {
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrRetirementType = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value;

                    //PIR 1035
                    busCalculation lbusCalculation = new busCalculation();
                   
				   //Ticket - 69718
				    int lintAccountId = 0;

                    decimal ldecNormalRetirementAge;

                    if (lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                    {
                        ldecNormalRetirementAge = lbusCalculation.GetNormalRetirementAge(busConstant.MPIPP_PLAN_ID);

                        if (lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount
                            .Where(t => t.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                            lintAccountId = lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount
                                .Where(t => t.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                    }
                    else
                    {
                        ldecNormalRetirementAge = lbusCalculation.GetNormalRetirementAge(lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id);

                        if (lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount
                            .Where(t => t.icdoPersonAccount.plan_id == lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id).Count() > 0)
                            lintAccountId = lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount
                                .Where(t => t.icdoPersonAccount.plan_id == lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id).FirstOrDefault().icdoPersonAccount.person_account_id;
                    }

                    DateTime ldtNormalRetirementDate = lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(ldecNormalRetirementAge));

                    DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });

                    if (ldtbPersonAccountEligibility != null && ldtbPersonAccountEligibility.Rows.Count > 0 && 
                        Convert.ToString(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]).IsNotNullOrEmpty())
                    {
                        if (Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]) > ldtNormalRetirementDate)
                            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate = Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]);
                        else
                            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate;

                        if (ldtNormalRetirementDate.Day != 1)
                        {
                            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate.AddMonths(1).GetFirstDayofMonth();
                        }
                    }

                    //PIR 894
                    if (lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT &&
                        lbusBenefitCalculationRetirement.ibusBenefitApplication != null && lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id > 0
                        && lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions != null && lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions.Count() > 0
                        && lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.istrBenefitOptionDescription == busConstant.LIFE_ANNUTIY_DESCRIPTION)
                    {
                        //RID#89578
                        //DataTable ldtbBenefitOpValue = busBase.Select("cdoPayeeAccount.GetBenefitOptionValueFromBenefitApplId", new object[1] { lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id });
                        DataTable ldtbBenefitOpValue = busBase.Select("cdoPayeeAccount.GetJointBenefitOptionValueFromBenefitApplicationDetailId", new object[1] { lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.benefit_application_detail_id });
                        
                        if (ldtbBenefitOpValue != null && ldtbBenefitOpValue.Rows.Count > 0)
                        {
                            if (!Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPlanBenefitXr.benefit_option_value.ToString().ToUpper()]).IsNullOrEmpty())
                                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrOriginalBenefitOptionValue = Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPlanBenefitXr.benefit_option_value.ToString().ToUpper()]);

                            if (!Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]).IsNullOrEmpty())
                                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.idtJointAnnuitantDOD = Convert.ToDateTime(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]);

                            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iblnPopUpToLife = true;
                            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrMoreInformation = "Converted from J&S pop up.";
                        }
                    }
                }
                
                //PIR 811                                      
                lbusBenefitCalculationRetirement.LoadErrors();
            }
            return lbusBenefitCalculationRetirement;
        }

        public busBenefitCalculationWithdrawal NewBenefitCalculationWithdrawal(string astr_person_mpi_id, string astr_benefit_type, int aint_plan_id)
        {
            busBenefitCalculationWithdrawal lbusBenefitCalculationWithdrawal = new busBenefitCalculationWithdrawal { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            lbusBenefitCalculationWithdrawal.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationWithdrawal.ibusPerson.FindPerson(astr_person_mpi_id);
            lbusBenefitCalculationWithdrawal.ibusPerson.LoadPersonAccounts();

            lbusBenefitCalculationWithdrawal.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            // Initial Setup for Checking Eligbility
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationWithdrawal.ibusPerson;
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount;
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)


            lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();



            //IN NEW MODE WHATEVER VALUES WE KNOW WE COULD FILL THOSE ATLEAST WHILE COMING TO THE MNTN SCREEN
            lbusBenefitCalculationWithdrawal.PopulateInitialDataBenefitCalculationHeader(lbusBenefitCalculationWithdrawal.ibusPerson.icdoPerson.person_id,
                                                                                        busConstant.ZERO_INT, busConstant.ZERO_INT, astr_benefit_type, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                                                                                        DateTime.MinValue, busConstant.ZERO_DECIMAL, aint_plan_id);
            lbusBenefitCalculationWithdrawal.LoadPersonNotes();

            return lbusBenefitCalculationWithdrawal;
        }

        public busBenefitCalculationWithdrawal FindBenefitCalculationWithdrawal(int aintBenefitCalculationId)
        {
            busBenefitCalculationWithdrawal lbusBenefitCalculationWithdrawal = new busBenefitCalculationWithdrawal();
            if (lbusBenefitCalculationWithdrawal.FindBenefitCalculationHeader(aintBenefitCalculationId))
            {

                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.retirement_date);
                DataTable ldtbPersonMPID = busBase.Select("cdoPerson.GetPersonMPID", new object[1] { lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.beneficiary_person_id });
                if (ldtbPersonMPID.Rows.Count > 0)
                {
                    lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.istrSurvivorMPID = ldtbPersonMPID.Rows[0][enmPerson.mpi_person_id.ToString()].ToString();
                }

                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.PopulateDescriptions();
                lbusBenefitCalculationWithdrawal.LoadPersonNotes();

                lbusBenefitCalculationWithdrawal.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                lbusBenefitCalculationWithdrawal.ibusPerson.FindPerson(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.person_id);
                //IAP Age 65 retirement.
                if (lbusBenefitCalculationWithdrawal.ibusPerson.FindPerson(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.person_id))
                {
                    lbusBenefitCalculationWithdrawal.ibusPerson.LoadPacketCorrespondences();
                }

                if (lbusBenefitCalculationWithdrawal.ibusPerson.FindPerson(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.person_id))
                {
                    lbusBenefitCalculationWithdrawal.ibusPerson.LoadPacketCorrespondences();
                }

                if (lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.dro_application_id > 0)
                {
                    lbusBenefitCalculationWithdrawal.ibusQdroApplication = new busQdroApplication { icdoDroApplication= new cdoDroApplication()};
                    if (lbusBenefitCalculationWithdrawal.ibusQdroApplication.FindQdroApplication(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.dro_application_id))
                    {
                        lbusBenefitCalculationWithdrawal.ibusQdroApplication.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                        lbusBenefitCalculationWithdrawal.ibusQdroApplication.ibusParticipant.FindPerson(lbusBenefitCalculationWithdrawal.ibusQdroApplication.icdoDroApplication.person_id);
                        lbusBenefitCalculationWithdrawal.ibusQdroApplication.ibusParticipant.iclbPersonAccount = new Collection<busPersonAccount>();
                        lbusBenefitCalculationWithdrawal.ibusQdroApplication.ibusParticipant.LoadPersonAccounts();

                        lbusBenefitCalculationWithdrawal.ibusQdroApplication.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };
                        lbusBenefitCalculationWithdrawal.ibusQdroApplication.ibusAlternatePayee.FindPerson(lbusBenefitCalculationWithdrawal.ibusQdroApplication.icdoDroApplication.alternate_payee_id);

                        // Initial Setup for Checking Eligbility
                        lbusBenefitCalculationWithdrawal.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                        lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationWithdrawal.ibusQdroApplication.ibusParticipant;
                        lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationWithdrawal.ibusQdroApplication.ibusParticipant.iclbPersonAccount;
                        lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.retirement_date;
                        lbusBenefitCalculationWithdrawal.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)


                    }

                }
                else
                {
                    lbusBenefitCalculationWithdrawal.ibusPerson.LoadPersonAccounts();

                    // Initial Setup for Checking Eligbility
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationWithdrawal.ibusPerson;
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount;
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.retirement_date;
                    lbusBenefitCalculationWithdrawal.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)

                }

                lbusBenefitCalculationWithdrawal.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                if (!lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    lbusBenefitCalculationWithdrawal.LoadAllRetirementContributions(lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    lbusBenefitCalculationWithdrawal.LoadAllRetirementContributions(null);
                }

                lbusBenefitCalculationWithdrawal.LoadBenefitCalculationDetails();
                lbusBenefitCalculationWithdrawal.LoadDisabilityRetireeIncreases();

                if (lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.Count() > 0)
                {
                    lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                    lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.istrRetirementType = lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value;
                }


                //Temp For Correspondence : Need a better solution
                lbusBenefitCalculationWithdrawal.ibusBenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                lbusBenefitCalculationWithdrawal.ibusBenefitCalculationRetirement.icdoBenefitCalculationHeader = lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader;
                lbusBenefitCalculationWithdrawal.ibusBenefitCalculationRetirement.iclbBenefitCalculationDetail = lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail;
                lbusBenefitCalculationWithdrawal.ibusBenefitCalculationRetirement.ibusPerson = lbusBenefitCalculationWithdrawal.ibusPerson;
                lbusBenefitCalculationWithdrawal.ibusBenefitCalculationRetirement.ibusBenefitApplication = lbusBenefitCalculationWithdrawal.ibusBenefitApplication;

                //for PIR-531
                busBenefitApplication lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusBenefitApplication.FindBenefitApplication(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.benefit_application_id);
                lbusBenefitCalculationWithdrawal.istrChildSupportFlg = lbusBenefitApplication.icdoBenefitApplication.child_support_flag;
                lbusBenefitCalculationWithdrawal.istrEmergencyOneTimePaymentFlag = lbusBenefitApplication.icdoBenefitApplication.emergency_onetime_payment_flag; //EmergencyOneTimePayment - 03/17/2020
            }
            return lbusBenefitCalculationWithdrawal;
        }

        public busPersonAccountRetirementContribution FindPersonAccountRetirementContribution(int aintpersonaccountretirementcontributionid)
        {
            busPersonAccountRetirementContribution lobjPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
            if (lobjPersonAccountRetirementContribution.FindPersonAccountRetirementContribution(aintpersonaccountretirementcontributionid))
            {
            }

            return lobjPersonAccountRetirementContribution;
        }

        public busBenefitCalculationPreRetirementDeath NewBenefitCalculationPreRetirementDeath(string astr_person_mpi_id, string astr_benefit_type, int aint_plan_id)
        {
            busBenefitCalculationPreRetirementDeath lbusBenefitCalculationPreRetirementDeath = new busBenefitCalculationPreRetirementDeath { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            lbusBenefitCalculationPreRetirementDeath.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationPreRetirementDeath.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationPreRetirementDeath.ibusPerson.FindPerson(astr_person_mpi_id);
            lbusBenefitCalculationPreRetirementDeath.ibusPerson.LoadPersonAccounts();

            lbusBenefitCalculationPreRetirementDeath.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            // Initial Setup for Checking Eligbility
            lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationPreRetirementDeath.ibusPerson;
            lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationPreRetirementDeath.ibusPerson.iclbPersonAccount;
            lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();//Code-Added -Abhishek (Imp to have work history state in background)

            lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();


            //IN NEW MODE WHATEVER VALUES WE KNOW WE COULD FILL THOSE ATLEAST WHILE COMING TO THE MNTN SCREEN
            lbusBenefitCalculationPreRetirementDeath.PopulateInitialDataBenefitCalculationHeader(lbusBenefitCalculationPreRetirementDeath.ibusPerson.icdoPerson.person_id,
                                                                                        busConstant.ZERO_INT, busConstant.ZERO_INT, astr_benefit_type, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                                                                                        DateTime.MinValue, busConstant.ZERO_DECIMAL, aint_plan_id);


            lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.date_of_death = lbusBenefitCalculationPreRetirementDeath.ibusPerson.icdoPerson.date_of_death;
            lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.GetPayeeAccountsInApprovedOrReviewSatus();
            if (!lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.iclbPayeeAccount.IsNullOrEmpty() && lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.iclbPayeeAccount.Where(lbuspayeeacc => lbuspayeeacc.icdoPayeeAccount.iintPlanId == aint_plan_id
                && (lbuspayeeacc.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT || lbuspayeeacc.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)).Count() > 0)
            {
                lbusBenefitCalculationPreRetirementDeath.iblnCheckIfPreRetPostElection = true;
            }
            //string strPlanCode = lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(check=> check.icdoPersonAccount.plan_id == lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode;
            //if (lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.CheckAlreadyVested(strPlanCode) && lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
            //{
            //    lbusBenefitCalculationPreRetirementDeath.LoadPreRetirementDeathInitialData();
            //}

            lbusBenefitCalculationPreRetirementDeath.EvaluateInitialLoadRules();
            return lbusBenefitCalculationPreRetirementDeath;
        }

        public busBenefitCalculationPreRetirementDeath FindBenefitCalculationPreRetirementDeath(int aintBenefitCalculationId)
        {
            busBenefitCalculationPreRetirementDeath lbusBenefitCalculationPreRetirementDeath = new busBenefitCalculationPreRetirementDeath();
            if (lbusBenefitCalculationPreRetirementDeath.FindBenefitCalculationHeader(aintBenefitCalculationId))
            {
                #region LoadBenefeciary
                if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.organization_id == 0)
                {

                    lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrSurvivorTypeValue = busConstant.SURVIVOR_TYPE_PER;
                    if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
                    {
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.retirement_date);
                    }
                    DataTable ldtbPersonMPID = busBase.Select("cdoPerson.GetPersonMPID", new object[1] { lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.beneficiary_person_id });
                    if (ldtbPersonMPID.Rows.Count > 0)
                    {
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrSurvivorMPID = ldtbPersonMPID.Rows[0][enmPerson.mpi_person_id.ToString()].ToString();
                    }
                }
                else
                {
                    DataTable ldtbOrganizationMPID = busBase.Select("cdoOrganization.GetOrgDetailsByOrganizationId", new object[1] { lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.organization_id });
                    lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrSurvivorTypeValue = busConstant.SURVIVOR_TYPE_ORGN;

                    if (ldtbOrganizationMPID.Rows.Count > 0)
                    {
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrOrganizationId = ldtbOrganizationMPID.Rows[0][enmOrganization.mpi_org_id.ToString()].ToString();
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrOrganizationName = ldtbOrganizationMPID.Rows[0][enmOrganization.org_name.ToString()].ToString();
                    }
                }
                #endregion

                #region ParticipantDetails
                lbusBenefitCalculationPreRetirementDeath.icdoSurvivorDetails = new cdoPerson();
                lbusBenefitCalculationPreRetirementDeath.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusBenefitCalculationPreRetirementDeath.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                lbusBenefitCalculationPreRetirementDeath.ibusPerson.FindPerson(lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.person_id);
                lbusBenefitCalculationPreRetirementDeath.ibusPerson.LoadPersonAccounts();
                #endregion

                #region Load Collection Objects
                lbusBenefitCalculationPreRetirementDeath.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                if (!lbusBenefitCalculationPreRetirementDeath.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    lbusBenefitCalculationPreRetirementDeath.LoadAllRetirementContributions(
                    lbusBenefitCalculationPreRetirementDeath.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    lbusBenefitCalculationPreRetirementDeath.LoadAllRetirementContributions(null);
                }

                lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.PopulateDescriptions();
                lbusBenefitCalculationPreRetirementDeath.LoadBenefitCalculationDetails();
                lbusBenefitCalculationPreRetirementDeath.LoadDisabilityRetireeIncreases();

                if (lbusBenefitCalculationPreRetirementDeath.ibusPerson.FindPerson(lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.person_id))
                {
                    lbusBenefitCalculationPreRetirementDeath.ibusPerson.LoadPacketCorrespondences();
                }

                if (lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.Count > 1)
                {
                    if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                    }
                    else
                    {
                        if (lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                            lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                    }
                }
                else if (lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.Count > 0)
                {
                    lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                }

                #endregion

                #region Setup for Checking Eligbility
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationPreRetirementDeath.ibusPerson;
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationPreRetirementDeath.ibusPerson.iclbPersonAccount;
                if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.benefit_commencement_date != DateTime.MinValue)
                {
                    lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.benefit_commencement_date;
                }
                else
                {
                    lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.retirement_date = DateTime.Now.GetLastDayofMonth().AddDays(1);
                }
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.GetPayeeAccountsInApprovedOrReviewSatus();
                if (!lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.iclbPayeeAccount.IsNullOrEmpty())
                {
                    if (lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.iintPlanId
                        && (item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT || item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)).Count() > 0)
                    {
                        lbusBenefitCalculationPreRetirementDeath.iblnCheckIfPreRetPostElection = true;
                    }
                }
                //lbusBenefitCalculationPreRetirementDeath.CalculateTotalEEContributionAndInterest();
                if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.date_of_death != DateTime.MinValue)
                {
                    lbusBenefitCalculationPreRetirementDeath.idecSurvivorAgeAtDeath = busGlobalFunctions.CalculatePersonAge(lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.date_of_death);
                    lbusBenefitCalculationPreRetirementDeath.GetBeneficiaryDetails(lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrSurvivorMPID);
                    lbusBenefitCalculationPreRetirementDeath.GetAgeAsOfEarliestRetirementDate();
                    lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.iintParticipantAgeAtDeath = Convert.ToInt32(busGlobalFunctions.CalculatePersonAge(lbusBenefitCalculationPreRetirementDeath.ibusPerson.icdoPerson.idtDateofBirth, lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.date_of_death));

                    if (lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.Count() > 0)
                    {
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrRetirementType = lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value;
                    }
                }
                if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.modified_date != DateTime.MinValue)
                {
                    lbusBenefitCalculationPreRetirementDeath.GetAgeAsOfCalculationDate();
                }
                #endregion

                //10 Percent
                if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                {
                    if (lbusBenefitCalculationPreRetirementDeath != null && lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.Count() > 0
                        && lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions != null
                        && lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions.Count() > 0)
                    {
                        if (lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.survivor_amount != decimal.Zero)
                            lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.idecRemainingAmountToBePaid =
                                lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.survivor_amount -
                                lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.paid_amount;
                    }
                }

                //Temp For Correspondence : Need a better solution
                #region Load Object For Correspondence
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitCalculationRetirement.icdoBenefitCalculationHeader = lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader;
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitCalculationRetirement.ibusPerson = lbusBenefitCalculationPreRetirementDeath.ibusPerson;
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitCalculationRetirement.ibusBenefitApplication = lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication;
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitCalculationRetirement.iclbBenefitCalculationDetail = lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail;

                #endregion

                if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL &&
                    lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.status_value != busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_APPROVED)
                {
                    lbusBenefitCalculationPreRetirementDeath.CheckQualifiedSpouseExists();
                }
                lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Update;

                //PIR 811                                      
                lbusBenefitCalculationPreRetirementDeath.LoadErrors();
            }
            return lbusBenefitCalculationPreRetirementDeath;
        }

        public busDisabiltyBenefitCalculation NewDisabiltyBenefitCalculation(string astr_person_mpi_id, string astr_benefit_type, int aint_plan_id,int aint_benefit_application_id) //, string astr_calculation_type_value
        {
            busDisabiltyBenefitCalculation lbusDisabiltyBenefitCalculation = new busDisabiltyBenefitCalculation { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            lbusDisabiltyBenefitCalculation.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };

           
            if (lbusDisabiltyBenefitCalculation.ibusBenefitApplication.FindBenefitApplication(aint_benefit_application_id))
            {
                lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.benefit_application_id = lbusDisabiltyBenefitCalculation.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id;

                if (lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson.FindPerson(lbusDisabiltyBenefitCalculation.ibusBenefitApplication.icdoBenefitApplication.person_id))
                {
                    lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson.LoadPersonAccounts();
                    lbusDisabiltyBenefitCalculation.ibusPerson = lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson;

                    lbusDisabiltyBenefitCalculation.ibusPerson.iclbPersonAccount = lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson.iclbPersonAccount;


                    //PIR 1035
                    busCalculation lbusCalculation = new busCalculation();
					
					//Ticket - 69718
                    int lintAccountId = 0;

                    decimal ldecNormalRetirementAge = lbusCalculation.GetNormalRetirementAge(aint_plan_id);
                    DateTime ldtNormalRetirementDate = lbusDisabiltyBenefitCalculation.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(ldecNormalRetirementAge));

                    if (lbusDisabiltyBenefitCalculation.ibusPerson.iclbPersonAccount
                                 .Where(t => t.icdoPersonAccount.plan_id == aint_plan_id).Count() > 0)
                        lintAccountId = lbusDisabiltyBenefitCalculation.ibusPerson.iclbPersonAccount
                            .Where(t => t.icdoPersonAccount.plan_id == aint_plan_id).FirstOrDefault().icdoPersonAccount.person_account_id;

                    DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });

                    if (ldtbPersonAccountEligibility != null && ldtbPersonAccountEligibility.Rows.Count > 0 &&
                              Convert.ToString(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]).IsNotNullOrEmpty())
                    {
                        if (Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]) > ldtNormalRetirementDate)
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate = Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]);
                        else
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate;

                        if (ldtNormalRetirementDate.Day != 1)
                        {
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate.AddMonths(1).GetFirstDayofMonth();
                        }
                    }
                 
                    lbusDisabiltyBenefitCalculation.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                    //lbusDisabiltyBenefitCalculation.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();//Code-Added -Abhishek (Imp to have work history state in background)
                    lbusDisabiltyBenefitCalculation.ibusBenefitApplicationRetirement = lbusDisabiltyBenefitCalculation.ibusBenefitApplication;
                    lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
                    //IN NEW MODE WHATEVER VALUES WE KNOW WE COULD FILL THOSE ATLEAST WHILE COMING TO THE MNTN SCREEN
                    lbusDisabiltyBenefitCalculation.PopulateInitialDataBenefitCalculationHeader(lbusDisabiltyBenefitCalculation.ibusPerson.icdoPerson.person_id,
                                                                                                busConstant.ZERO_INT, busConstant.ZERO_INT, astr_benefit_type, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                                                                                                DateTime.MinValue, busConstant.ZERO_DECIMAL, aint_plan_id);


                    lbusDisabiltyBenefitCalculation.LoadPlanBenefitsForPlan(aint_plan_id);
                    lbusDisabiltyBenefitCalculation.CopyApplicationPropertiesToCalculation();
                    lbusDisabiltyBenefitCalculation.EvaluateInitialLoadRules();
                    lbusDisabiltyBenefitCalculation.LoadPersonNotes();
                    lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                    lbusDisabiltyBenefitCalculation.iarrChangeLog.Add(lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader);
                    if (lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.Count > 0)
                    {
                        busBenefitCalculationDetail benefitCalculationDetail = lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.FirstOrDefault();
                        lbusDisabiltyBenefitCalculation.sel_benefit_calculation_detail_id = benefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                    }
                }
            }
            return lbusDisabiltyBenefitCalculation;
        }

        public busDisabiltyBenefitCalculation FindDisabiltyBenefitCalculation(int aintBenefitCalculationId)
        {
            busDisabiltyBenefitCalculation lbusDisabiltyBenefitCalculation = new busDisabiltyBenefitCalculation();
            if (lbusDisabiltyBenefitCalculation.FindBenefitCalculationHeader(aintBenefitCalculationId))
            {
                lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.retirement_date);

                DataTable ldtbPersonMPID = busBase.Select("cdoPerson.GetPersonMPID", new object[1] { lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.beneficiary_person_id });
                if (ldtbPersonMPID.Rows.Count > 0)
                {
                    lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.istrSurvivorMPID = ldtbPersonMPID.Rows[0][enmPerson.mpi_person_id.ToString()].ToString();
                }
                lbusDisabiltyBenefitCalculation.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusDisabiltyBenefitCalculation.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                lbusDisabiltyBenefitCalculation.ibusPerson.FindPerson(lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.person_id);
                lbusDisabiltyBenefitCalculation.ibusPerson.LoadPersonAccounts();

                if (lbusDisabiltyBenefitCalculation.ibusPerson.FindPerson(lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.person_id))
                {
                    lbusDisabiltyBenefitCalculation.ibusPerson.LoadPacketCorrespondences();
                }

                lbusDisabiltyBenefitCalculation.iintAgeAtRetirement = Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAge(lbusDisabiltyBenefitCalculation.ibusPerson.icdoPerson.idtDateofBirth, lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.retirement_date)));

                // Initial Setup for Checking Eligbility
                lbusDisabiltyBenefitCalculation.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson = lbusDisabiltyBenefitCalculation.ibusPerson;
                lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusDisabiltyBenefitCalculation.ibusPerson.iclbPersonAccount;
                //RequestID: 72091
                lbusDisabiltyBenefitCalculation.ibusBenefitApplication.FindBenefitApplication(lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.benefit_application_id);
                //lbusDisabiltyBenefitCalculation.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); 

                lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.PopulateDescriptions();
                lbusDisabiltyBenefitCalculation.LoadPersonNotes();
                lbusDisabiltyBenefitCalculation.LoadBenefitCalculationDetails();
                lbusDisabiltyBenefitCalculation.LoadDisabilityRetireeIncreases();
                if (lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.Count > 1)
                {
                    if (lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL)
                    {
                        lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.iintPlanId = lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                    }
                    else
                    {
                        if (lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.iintPlanId = lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                        else if (lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.iintPlanId = lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                        else
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.iintPlanId = lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                    }
                }

                else if(lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.Count > 0)
                {
                    lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.iintPlanId = lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id;


                    //PIR 1035
                    busCalculation lbusCalculation = new busCalculation();
					
					//Ticket - 69718
                    int lintAccountId = 0;

                    decimal ldecNormalRetirementAge;

                    if (lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                    {
                        ldecNormalRetirementAge = lbusCalculation.GetNormalRetirementAge(busConstant.MPIPP_PLAN_ID);

                        if (lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson.iclbPersonAccount
                            .Where(t => t.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                            lintAccountId = lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson.iclbPersonAccount
                                .Where(t => t.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                    }
                    else
                    {
                        ldecNormalRetirementAge = lbusCalculation.GetNormalRetirementAge(lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id);

                        if (lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson.iclbPersonAccount
                            .Where(t => t.icdoPersonAccount.plan_id == lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id).Count() > 0)
                            lintAccountId = lbusDisabiltyBenefitCalculation.ibusBenefitApplication.ibusPerson.iclbPersonAccount
                                .Where(t => t.icdoPersonAccount.plan_id == lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id).FirstOrDefault().icdoPersonAccount.person_account_id;
                    }

                    DateTime ldtNormalRetirementDate = lbusDisabiltyBenefitCalculation.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(ldecNormalRetirementAge));

                    DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });

                    if (ldtbPersonAccountEligibility != null && ldtbPersonAccountEligibility.Rows.Count > 0 &&
                        Convert.ToString(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]).IsNotNullOrEmpty())
                    {
                        if (Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]) > ldtNormalRetirementDate)
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate = Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]);
                        else
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate;

                        if (ldtNormalRetirementDate.Day != 1)
                        {
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate.AddMonths(1).GetFirstDayofMonth();
                        }
                    }                   
                }

                lbusDisabiltyBenefitCalculation.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                if (!lbusDisabiltyBenefitCalculation.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    lbusDisabiltyBenefitCalculation.LoadAllRetirementContributions(
                    lbusDisabiltyBenefitCalculation.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    lbusDisabiltyBenefitCalculation.LoadAllRetirementContributions(null);
                }

                foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail)
                {
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode = lbusDisabiltyBenefitCalculation.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id).First().icdoPersonAccount.istrPlanCode;
                    //lbusBenefitCalculationDetail.istrDisabilityType = lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.istrDisabilityType;

                    lbusBenefitCalculationDetail.LoadBenefitCalculationOptionss();
                    lbusBenefitCalculationDetail.LoadBenefitCalculationYearlyDetails();
                    foreach (busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail in lbusBenefitCalculationDetail.iclbBenefitCalculationYearlyDetail)
                    {
                        lbusBenefitCalculationYearlyDetail.LoadBenefitCalculationNonsuspendibleDetails();
                    }
                    decimal ldecReductionFactor = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.early_reduction_factor;
                    if (ldecReductionFactor > 0)
                    {
                        lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.ForEach(option => option.idecRegularReductionFactor = ldecReductionFactor);
                    }
                }

                lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.idecParticipantFullAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusDisabiltyBenefitCalculation.ibusPerson.icdoPerson.idtDateofBirth, lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.retirement_date);
                //pragati:commented code removed 
                lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.istrRetirementType = lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value;
                lbusDisabiltyBenefitCalculation.LoadPlanBenefitsForPlan(lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.iintPlanId);
                if (lbusDisabiltyBenefitCalculation.CheckIfHoursReportedAfterDisabilityDate())
                {
                    if (lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.retirement_date_option_2 == DateTime.MinValue)
                    {
                        lbusDisabiltyBenefitCalculation.GetLastWorkingDate();
                        if (lbusDisabiltyBenefitCalculation.idtLastWorkingDate != DateTime.MinValue)
                        {
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.retirement_date_option_2 = lbusDisabiltyBenefitCalculation.idtLastWorkingDate.GetLastDayofMonth().AddDays(1);
                        }
                    }
                }

                //Related to Disability Conversion Part
                if (lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.payee_account_id > 0)
                {
                    lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.istrRetirementType = busConstant.DISABILITY_TYPE_SSA;
                }

                //Temp For Correspondence : Need a better solution
                lbusDisabiltyBenefitCalculation.ibusBenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                lbusDisabiltyBenefitCalculation.ibusBenefitCalculationRetirement.icdoBenefitCalculationHeader = lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader;
                lbusDisabiltyBenefitCalculation.ibusBenefitCalculationRetirement.iclbBenefitCalculationDetail = lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail;
                lbusDisabiltyBenefitCalculation.ibusBenefitCalculationRetirement.ibusPerson = lbusDisabiltyBenefitCalculation.ibusPerson;
                lbusDisabiltyBenefitCalculation.ibusBenefitCalculationRetirement.ibusBenefitApplication = lbusDisabiltyBenefitCalculation.ibusBenefitApplication;

                //RequestID: 72091
                if (lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT &&
                        lbusDisabiltyBenefitCalculation.ibusBenefitApplication != null && lbusDisabiltyBenefitCalculation.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id > 0
                        && lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions != null && lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions.Count() > 0
                        && lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.istrBenefitOptionDescription == busConstant.LIFE_ANNUTIY_DESCRIPTION)
                {
                    DataTable ldtbBenefitOpValue = busBase.Select("cdoPayeeAccount.GetBenefitOptionValueFromBenefitApplId", new object[1] { lbusDisabiltyBenefitCalculation.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id });

                    if (ldtbBenefitOpValue != null && ldtbBenefitOpValue.Rows.Count > 0)
                    {
                        if (!Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPlanBenefitXr.benefit_option_value.ToString().ToUpper()]).IsNullOrEmpty())
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.istrOriginalBenefitOptionValue = Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPlanBenefitXr.benefit_option_value.ToString().ToUpper()]);

                        if (!Convert.ToString(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]).IsNullOrEmpty())
                            lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.idtJointAnnuitantDOD = Convert.ToDateTime(ldtbBenefitOpValue.Rows[0][enmPerson.date_of_death.ToString().ToUpper()]);

                        lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.iblnPopUpToLife = true;
                        lbusDisabiltyBenefitCalculation.icdoBenefitCalculationHeader.istrMoreInformation = "Converted from J&S pop up.";
                    }
                }
                if (lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.Count > 0)
                {
                    busBenefitCalculationDetail benefitCalculationDetail = lbusDisabiltyBenefitCalculation.iclbBenefitCalculationDetail.FirstOrDefault();
                    lbusDisabiltyBenefitCalculation.sel_benefit_calculation_detail_id = benefitCalculationDetail.icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                }
                //PIR 811
                lbusDisabiltyBenefitCalculation.LoadErrors();
            }
            return lbusDisabiltyBenefitCalculation;
        }



        //public busBenefitProvisionBenefitOptionFactor FindBenefitProvisionBenefitOptionFactor(int aintbenefitprovisionbenefitoptionfactorid)
        //{
        //    busBenefitProvisionBenefitOptionFactor lobjBenefitProvisionBenefitOptionFactor = new busBenefitProvisionBenefitOptionFactor();
        //    if (lobjBenefitProvisionBenefitOptionFactor.FindBenefitProvisionBenefitOptionFactor(aintbenefitprovisionbenefitoptionfactorid))
        //    {
        //    }

        //    return lobjBenefitProvisionBenefitOptionFactor;
        //}

        // PIR-507 For New Estimate Calculation
        public busQdroCalculationHeader NewQdroCalculationHeader(int aintQdroApplicationId, int aintPlanId, string astr_person_mpi_id, string astr_alt_payee_id, string astrqdro_model_value)
        {
            busQdroCalculationHeader lbusQdroCalculationHeader = new busQdroCalculationHeader { icdoQdroCalculationHeader = new cdoQdroCalculationHeader() };
            lbusQdroCalculationHeader.iclbQdroCalculationDetail = new Collection<busQdroCalculationDetail>();
            lbusQdroCalculationHeader.ibusBenefitCalculationHeader = new busBenefitCalculationHeader();
            lbusQdroCalculationHeader.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
            lbusQdroCalculationHeader.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };
            lbusQdroCalculationHeader.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            lbusQdroCalculationHeader.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusQdroCalculationHeader.ibusBenefitApplicationForDisability = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };

            lbusQdroCalculationHeader.icdoQdroCalculationHeader.calculation_type_id = busConstant.BenefitCalculation.CALCULATION_TYPE_CODE_ID;
            lbusQdroCalculationHeader.icdoQdroCalculationHeader.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
            lbusQdroCalculationHeader.icdoQdroCalculationHeader.calculation_type_value = busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE;
            lbusQdroCalculationHeader.icdoQdroCalculationHeader.status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
            lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_application_id = aintQdroApplicationId;
            lbusQdroCalculationHeader.icdoQdroCalculationHeader.iintPlanId = aintPlanId;
            lbusQdroCalculationHeader.icdoQdroCalculationHeader.PopulateDescriptions();

            lbusQdroCalculationHeader.ibusQdroApplication = new busQdroApplication();

            if (lbusQdroCalculationHeader.ibusQdroApplication.FindQdroApplication(aintQdroApplicationId))
            {
                lbusQdroCalculationHeader.icdoQdroCalculationHeader.person_id = lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.person_id;
                lbusQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id = lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.alternate_payee_id;
                lbusQdroCalculationHeader.icdoQdroCalculationHeader.date_of_marriage = lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.date_of_marriage;
                lbusQdroCalculationHeader.icdoQdroCalculationHeader.date_of_seperation = lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.date_of_divorce;

                lbusQdroCalculationHeader.ibusQdroApplication.LoadBenefitDetails();

                if (lbusQdroCalculationHeader.ibusQdroApplication.iclbDroBenefitDetails != null &&
                    (lbusQdroCalculationHeader.ibusQdroApplication.iclbDroBenefitDetails.Count > 0))
                {
                    if (lbusQdroCalculationHeader.ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == 1).Count() != 0)
                        lbusQdroCalculationHeader.icdoQdroCalculationHeader.is_alt_payee_eligible_for_iap = busConstant.FLAG_YES;
                }

                lbusQdroCalculationHeader.ibusParticipant.FindPerson(lbusQdroCalculationHeader.icdoQdroCalculationHeader.person_id);
                lbusQdroCalculationHeader.ibusAlternatePayee.FindPerson(lbusQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id);

                lbusQdroCalculationHeader.icdoQdroCalculationHeader.person_id = lbusQdroCalculationHeader.ibusParticipant.icdoPerson.person_id;
                lbusQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id = lbusQdroCalculationHeader.ibusAlternatePayee.icdoPerson.person_id;

                if (lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.is_participant_disabled_flag == busConstant.FLAG_YES)
                    lbusQdroCalculationHeader.icdoQdroCalculationHeader.is_participant_disabled = busConstant.FLAG_YES;

                //if (lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.is_participant_dead_flag == busConstant.FLAG_YES)
                //    lbusQdroCalculationHeader.iblnIsParticipantDead = busConstant.FLAG_YES;        

            }
            else
            {
                if (aintPlanId == 1 || aintPlanId == 2)
                {
                    lbusQdroCalculationHeader.icdoQdroCalculationHeader.is_alt_payee_eligible_for_iap = busConstant.FLAG_YES;
                }

                lbusQdroCalculationHeader.ibusParticipant.FindPerson(astr_person_mpi_id);
                lbusQdroCalculationHeader.ibusAlternatePayee.FindPerson(astr_alt_payee_id);

                lbusQdroCalculationHeader.icdoQdroCalculationHeader.person_id = lbusQdroCalculationHeader.ibusParticipant.icdoPerson.person_id;
                lbusQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id = lbusQdroCalculationHeader.ibusAlternatePayee.icdoPerson.person_id;
            }

            lbusQdroCalculationHeader.ibusParticipant.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusQdroCalculationHeader.ibusParticipant.LoadPersonAccounts();

            lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();

            lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson = lbusQdroCalculationHeader.ibusParticipant;
            lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusQdroCalculationHeader.ibusParticipant.iclbPersonAccount;

            // PIR-507 For New Estimate Calculation            
            DateTime ldtLastWoringDayOfParticipant = lbusQdroCalculationHeader.ibusCalculation.GetLastWorkingDate(lbusQdroCalculationHeader.ibusParticipant.icdoPerson.ssn);
            if (!string.IsNullOrEmpty(astrqdro_model_value) && astrqdro_model_value == "STAF")
            {
                lbusQdroCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date =
                lbusQdroCalculationHeader.GetRetirementDateforCalculation() > ldtLastWoringDayOfParticipant ? DateTime.Now : lbusQdroCalculationHeader.GetRetirementDateforCalculation();
            }
            else
            {
                lbusQdroCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusQdroCalculationHeader.GetRetirementDateforCalculation();
            }

            lbusQdroCalculationHeader.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added Abhishek (Imp to have the Work-History Loaded)

            #region Set Data for Disabilty

            lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();

            lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson = lbusQdroCalculationHeader.ibusParticipant;
            lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson.iclbPersonAccount = lbusQdroCalculationHeader.ibusParticipant.iclbPersonAccount;
            lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.aclbPersonWorkHistory_MPI =
                                                                            lbusQdroCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI;
            lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.aclbPersonWorkHistory_IAP =
                                                                            lbusQdroCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_IAP;
            lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.Eligible_Plans =
                                                                            lbusQdroCalculationHeader.ibusBenefitApplication.Eligible_Plans;


            #endregion

            lbusQdroCalculationHeader.ibusBenefitApplication.DetermineVesting();
            lbusQdroCalculationHeader.iblnVestingHasBeenChecked = true;
            lbusQdroCalculationHeader.GetEarliestRetiremenDate();

            lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_commencement_date = lbusQdroCalculationHeader.icdoQdroCalculationHeader.retirement_date;

            lbusQdroCalculationHeader.icdoQdroCalculationHeader.ienuObjectState = ObjectState.Insert;
            lbusQdroCalculationHeader.iarrChangeLog.Add(lbusQdroCalculationHeader.icdoQdroCalculationHeader);

            lbusQdroCalculationHeader.EvaluateInitialLoadRules();
            return lbusQdroCalculationHeader;
        }

        public busQdroCalculationHeader FindQdroCalculationHeader(long aintQdroFileDetails,int aintQdroCalculationHeaderId)
        {
            busQdroCalculationHeader lobjQdroCalculationHeader = new busQdroCalculationHeader();
            if (lobjQdroCalculationHeader.FindQdroCalculationHeader(aintQdroCalculationHeaderId))
            {
                lobjQdroCalculationHeader.ibusQdroApplication = new busQdroApplication();
                lobjQdroCalculationHeader.ibusBenefitCalculationHeader = new busBenefitCalculationHeader();
                lobjQdroCalculationHeader.ibusParticipant = new busPerson();
                lobjQdroCalculationHeader.ibusAlternatePayee = new busPerson();
                lobjQdroCalculationHeader.ibusParticipant.iclbPersonAccount = new Collection<busPersonAccount>();
                lobjQdroCalculationHeader.ibusParticipant.iclbPersonContact = new Collection<busPersonContact>();

                if (lobjQdroCalculationHeader.ibusQdroApplication.FindQdroApplication(lobjQdroCalculationHeader.icdoQdroCalculationHeader.qdro_application_id))
                {
                    lobjQdroCalculationHeader.icdoQdroCalculationHeader.person_id = lobjQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.person_id;
                    lobjQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id = lobjQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.alternate_payee_id;

                }

                lobjQdroCalculationHeader.ibusParticipant.FindPerson(lobjQdroCalculationHeader.icdoQdroCalculationHeader.person_id);
                //

                if (lobjQdroCalculationHeader.ibusAlternatePayee.FindPerson(lobjQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id))
                {
                    lobjQdroCalculationHeader.ibusAlternatePayee.LoadPacketCorrespondences();
                }

                lobjQdroCalculationHeader.icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement =
                       Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAgeInDec(Convert.ToDateTime(lobjQdroCalculationHeader.ibusAlternatePayee.icdoPerson.idtDateofBirth),
                        lobjQdroCalculationHeader.GetRetirementDateforCalculation())));

                lobjQdroCalculationHeader.icdoQdroCalculationHeader.iintParticipantAtRetirement = Convert.ToInt32( Math.Floor(lobjQdroCalculationHeader.icdoQdroCalculationHeader.age));

                lobjQdroCalculationHeader.icdoQdroCalculationHeader.istrSurvivorMPID =
                        lobjQdroCalculationHeader.ibusAlternatePayee.icdoPerson.mpi_person_id;

                lobjQdroCalculationHeader.ibusParticipant.LoadPersonAccounts();
                lobjQdroCalculationHeader.LoadQdroCalculationDetails();
                lobjQdroCalculationHeader.LoadDisabilityRetireeIncreases();

                lobjQdroCalculationHeader.ibusParticipant.LoadPersonContacts();

                foreach (busQdroCalculationDetail lbusQdroCalculationDetail in lobjQdroCalculationHeader.iclbQdroCalculationDetail)
                {

                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.istrPlanCode =
                        lobjQdroCalculationHeader.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                            lbusQdroCalculationDetail.icdoQdroCalculationDetail.plan_id).First().icdoPersonAccount.istrPlanCode;

                    lbusQdroCalculationDetail.LoadPlanDescription();
                    lbusQdroCalculationDetail.LoadQdroCalculationOptionss();
                    lbusQdroCalculationDetail.LoadQdroCalculationYearlyDetails();
                    lbusQdroCalculationDetail.LoadQdroIapAllocationDetails();

                    if (lobjQdroCalculationHeader.icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES)
                        lbusQdroCalculationDetail.icdoQdroCalculationDetail.istrRetirementTypeDisability = busConstant.BENEFIT_TYPE_DISABILITY_DESC;
                }

                lobjQdroCalculationHeader.icdoQdroCalculationHeader.istrRetirementType = lobjQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.benefit_subtype_value;
                //Code to Fill Proper PLAN ID in the HEADER OBJECT
                if (lobjQdroCalculationHeader.iclbQdroCalculationDetail.Count > 1 && (!lobjQdroCalculationHeader.iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty()))
                {
                    lobjQdroCalculationHeader.icdoQdroCalculationHeader.iintPlanId = lobjQdroCalculationHeader.iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoQdroCalculationDetail.plan_id;
                }
                else
                {
                    lobjQdroCalculationHeader.icdoQdroCalculationHeader.iintPlanId =
                        lobjQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.plan_id;
                }

                if (!lobjQdroCalculationHeader.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    lobjQdroCalculationHeader.ibusBenefitCalculationHeader.LoadAllRetirementContributions(lobjQdroCalculationHeader.ibusParticipant.icdoPerson.person_id,
                        lobjQdroCalculationHeader.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    lobjQdroCalculationHeader.ibusBenefitCalculationHeader.LoadAllRetirementContributions(lobjQdroCalculationHeader.ibusParticipant.icdoPerson.person_id,null);
                }

                // Initial Setup for Checking Eligbility
                lobjQdroCalculationHeader.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lobjQdroCalculationHeader.ibusBenefitApplicationForDisability = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lobjQdroCalculationHeader.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjQdroCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                lobjQdroCalculationHeader.ibusBenefitApplication.ibusPerson = lobjQdroCalculationHeader.ibusParticipant;
                lobjQdroCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lobjQdroCalculationHeader.ibusParticipant.iclbPersonAccount;

                //lobjQdroCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lobjQdroCalculationHeader.GetRetirementDateforCalculation();
                // PIR-507 For Estimate Calculation                
                DateTime ldtLastWoringDayOfParticipant = lobjQdroCalculationHeader.ibusCalculation.GetLastWorkingDate(lobjQdroCalculationHeader.ibusParticipant.icdoPerson.ssn);
                if (lobjQdroCalculationHeader.iclbQdroCalculationDetail.Count > 0 &&
                    lobjQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.qdro_model_value == "STAF")
                {
                    lobjQdroCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date =
                        lobjQdroCalculationHeader.GetRetirementDateforCalculation() > ldtLastWoringDayOfParticipant ? DateTime.Now : lobjQdroCalculationHeader.GetRetirementDateforCalculation();
                }
                else
                {
                    lobjQdroCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lobjQdroCalculationHeader.GetRetirementDateforCalculation();
                }

                lobjQdroCalculationHeader.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added Abhishek (Imp to have the Work-History Loaded)

                //if(lobjQdroCalculationHeader.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                //    lobjQdroCalculationHeader.GetEarliestRetiremenDate(); 
                #region Set Data for disability
                if (lobjQdroCalculationHeader.icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES)
                {
                    lobjQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lobjQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();

                    lobjQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson = lobjQdroCalculationHeader.ibusParticipant;
                    lobjQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson.iclbPersonAccount = lobjQdroCalculationHeader.ibusParticipant.iclbPersonAccount;
                    lobjQdroCalculationHeader.ibusBenefitApplicationForDisability.aclbPersonWorkHistory_MPI =
                                                                     lobjQdroCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI;
                    lobjQdroCalculationHeader.ibusBenefitApplicationForDisability.aclbPersonWorkHistory_IAP =
                                                                                    lobjQdroCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_IAP;
                    lobjQdroCalculationHeader.ibusBenefitApplicationForDisability.Eligible_Plans =
                                                                                    lobjQdroCalculationHeader.ibusBenefitApplication.Eligible_Plans;
                }

                #endregion

                #region Check for Participant Payee Account //Rohan

                if (lobjQdroCalculationHeader.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                    lobjQdroCalculationHeader.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                {
                    if (lobjQdroCalculationHeader.iclbQdroCalculationDetail.Count > 0 &&
                     lobjQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.qdro_model_value == busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA)
                    {
                        lobjQdroCalculationHeader.GetParticipantsPayeeAccountForGivenPlan();
                        if (lobjQdroCalculationHeader.iclbParticipantsPayeeAccount != null && lobjQdroCalculationHeader.iclbParticipantsPayeeAccount.Count > 0)
                        {
                            lobjQdroCalculationHeader.iblnParticipantPayeeAccount = busConstant.BOOL_TRUE;
                            lobjQdroCalculationHeader.iclbParticipantsPayeeAccount.FirstOrDefault().LoadBenefitDetails();
                            lobjQdroCalculationHeader.iclbParticipantsPayeeAccount.FirstOrDefault().LoadNextBenefitPaymentDate();
                            DataTable ldtblParticipantAmount = busBase.Select("cdoQdroCalculationHeader.GetBenefitAmount", new object[] { lobjQdroCalculationHeader.iclbParticipantsPayeeAccount.FirstOrDefault().icdoPayeeAccount.payee_account_id,
                                 lobjQdroCalculationHeader.iclbParticipantsPayeeAccount.FirstOrDefault().idtNextBenefitPaymentDate});
                            if (ldtblParticipantAmount.Rows.Count > 0)
                            {
                                lobjQdroCalculationHeader.idecParticipantAmount = Convert.ToDecimal(ldtblParticipantAmount.Rows[0][0]);
                                lobjQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.idecParticipantAmount
                                    = lobjQdroCalculationHeader.idecParticipantAmount;

                                if (lobjQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().iclbQdroCalculationOptions != null && lobjQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().iclbQdroCalculationOptions.Count > 0)
                                {
                                    lobjQdroCalculationHeader.idecAlternatepayeeBenefitFraction =
                                        Math.Round((lobjQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().iclbQdroCalculationOptions.FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount) / lobjQdroCalculationHeader.idecParticipantAmount, 3);
                                }
                            }
                        }
                    }
                }

                #endregion
            }
            lobjQdroCalculationHeader.icdoQdroCalculationHeader.iintAPrimaryKey = aintQdroFileDetails;
            return lobjQdroCalculationHeader;
        }

        public busQdroCalculationLookup LoadQdroCalculations(DataTable adtbSearchResult)
        {
            busQdroCalculationLookup lobjQdroCalculationLookup = new busQdroCalculationLookup();
            lobjQdroCalculationLookup.LoadQdroCalculationHeaders(adtbSearchResult);
            return lobjQdroCalculationLookup;
        }

        public busQdroCalculationDetail FindQdroCalculationDetail(int aintQdroCalculationDetailId)
        {
            busQdroCalculationDetail lobjQdroCalculationDetail = new busQdroCalculationDetail();
            if (lobjQdroCalculationDetail.FindQdroCalculationDetail(aintQdroCalculationDetailId))
            {
            }

            return lobjQdroCalculationDetail;
        }

        public busQdroCalculationOptions FindQdroCalculationOptions(int aintQdroCalculationOptionId)
        {
            busQdroCalculationOptions lobjQdroCalculationOptions = new busQdroCalculationOptions();
            if (lobjQdroCalculationOptions.FindQdroCalculationOptions(aintQdroCalculationOptionId))
            {
            }

            return lobjQdroCalculationOptions;
        }

        public busQdroCalculationYearlyDetail FindQdroCalculationYearlyDetail(int aintQdroCalculationYearlyDetailId)
        {
            busQdroCalculationYearlyDetail lobjQdroCalculationYearlyDetail = new busQdroCalculationYearlyDetail();
            if (lobjQdroCalculationYearlyDetail.FindQdroCalculationYearlyDetail(aintQdroCalculationYearlyDetailId))
            {
            }

            return lobjQdroCalculationYearlyDetail;
        }

        public busBenefitCalculationNonsuspendibleDetail FindBenefitCalculationNonsuspendibleDetail(int aintBenefitCalculationYearlyDetailId)
        {
            busBenefitCalculationNonsuspendibleDetail lbusBenefitCalculationNonsuspendibleDetail = new busBenefitCalculationNonsuspendibleDetail();
            lbusBenefitCalculationNonsuspendibleDetail.ibusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail() { icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail() };
            //busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail() { icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail() };
            if (lbusBenefitCalculationNonsuspendibleDetail.ibusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail == null)
            {
                lbusBenefitCalculationNonsuspendibleDetail.ibusBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail = new Collection<busBenefitCalculationNonsuspendibleDetail>();
            }

            if (lbusBenefitCalculationNonsuspendibleDetail.ibusBenefitCalculationYearlyDetail.FindBenefitCalculationYearlyDetail(aintBenefitCalculationYearlyDetailId))
            {
                lbusBenefitCalculationNonsuspendibleDetail.ibusBenefitCalculationYearlyDetail.LoadBenefitCalculationDetail();
                lbusBenefitCalculationNonsuspendibleDetail.ibusBenefitCalculationYearlyDetail.ibusBenefitCalculationDetail.LoadBenefitCalculationHeader();
                lbusBenefitCalculationNonsuspendibleDetail.ibusBenefitCalculationYearlyDetail.ibusBenefitCalculationDetail.ibusBenefitCalculationHeader.LoadPerson();
                lbusBenefitCalculationNonsuspendibleDetail.ibusBenefitCalculationYearlyDetail.LoadBenefitCalculationNonsuspendibleDetails();
                //lbusBenefitCalculationNonsuspendibleDetail.ibusBenefitCalculationYearlyDetail = lbusBenefitCalculationYearlyDetail;
                //lbusBenefitCalculationNonsuspendibleDetail.ibusBenefitCalculationDetail = lbusBenefitCalculationYearlyDetail.ibusBenefitCalculationDetail;
            }

            return lbusBenefitCalculationNonsuspendibleDetail;
        }

        public busBenefitCalculationYearlyDetail FindBenefitCalculationYearlyDetail(int aintbenefitcalculationyearlydetailid)
        {
            busBenefitCalculationYearlyDetail lobjBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail();
            if (lobjBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail == null)
            {
                lobjBenefitCalculationYearlyDetail.iclbBenefitCalculationNonsuspendibleDetail = new Collection<busBenefitCalculationNonsuspendibleDetail>();
            }

            if (lobjBenefitCalculationYearlyDetail.FindBenefitCalculationYearlyDetail(aintbenefitcalculationyearlydetailid))
            {
                lobjBenefitCalculationYearlyDetail.LoadBenefitCalculationDetail();
                lobjBenefitCalculationYearlyDetail.ibusBenefitCalculationDetail.LoadBenefitCalculationHeader();
                lobjBenefitCalculationYearlyDetail.ibusBenefitCalculationDetail.ibusBenefitCalculationHeader.LoadPerson();
                lobjBenefitCalculationYearlyDetail.LoadBenefitCalculationNonsuspendibleDetails();
            }

            return lobjBenefitCalculationYearlyDetail;
        }

        //public busBenefitCalculationNonsuspendibleDetail FindBenefitCalculationNonsuspendibleDetail(int aintBenefitCalculationNonsuspendibleDetailId)
        //{
        //    busBenefitCalculationNonsuspendibleDetail lobjBenefitCalculationNonsuspendibleDetail = new busBenefitCalculationNonsuspendibleDetail();
        //    if (lobjBenefitCalculationNonsuspendibleDetail.FindBenefitCalculationNonsuspendibleDetail(aintBenefitCalculationNonsuspendibleDetailId))
        //    {
        //        lobjBenefitCalculationNonsuspendibleDetail.LoadBenefitCalculationYearlyDetail();
        //        lobjBenefitCalculationNonsuspendibleDetail.LoadBenefitCalculationDetail();
        //    }

        //    return lobjBenefitCalculationNonsuspendibleDetail;
        //}

        public busPersonAccountRetirementContribution FindPersonAccountRetirementContributionByPersonID(int aintpersonaccountretirementcontributionid, int aintPersonID)
        {
            busPersonAccountRetirementContribution lobjPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
            lobjPersonAccountRetirementContribution.ibusPerson = new busPerson();
            lobjPersonAccountRetirementContribution.ibusPerson.FindPerson(aintPersonID);
            if (lobjPersonAccountRetirementContribution.FindPersonAccountRetirementContribution(aintpersonaccountretirementcontributionid))
            {
                lobjPersonAccountRetirementContribution.ibusPerson = new busPerson();
                lobjPersonAccountRetirementContribution.ibusPerson.FindPerson(aintPersonID);
                lobjPersonAccountRetirementContribution.ibusPerson.LoadPersonAccounts();
            }

            return lobjPersonAccountRetirementContribution;
        }

        public busPersonAccountRetirementContribution NewPersonAccountRetirementContribution(int aintPersonId)
        {
            busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            lbusPersonAccountRetirementContribution.ibusPerson = new busPerson();
            lbusPersonAccountRetirementContribution.ibusPerson.FindPerson(aintPersonId);
            lbusPersonAccountRetirementContribution.ibusPerson.LoadPersonAccounts();

            return lbusPersonAccountRetirementContribution;
        }

        public busQdroIapAllocationDetail FindQdroIapAllocationDetail(int aintQdroIapAllocationDetailId)
        {
            busQdroIapAllocationDetail lobjQdroIapAllocationDetail = new busQdroIapAllocationDetail();
            if (lobjQdroIapAllocationDetail.FindQdroIapAllocationDetail(aintQdroIapAllocationDetailId))
            {
            }

            return lobjQdroIapAllocationDetail;
        }

        public busBenefitCalculationPostRetirementDeath FindBenefitCalculationPostRetirementDeath(int aintBenefitCalculationId)
        {
            busBenefitCalculationPostRetirementDeath lbusBenefitCalculationPostRetirementDeath = new busBenefitCalculationPostRetirementDeath { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            if (lbusBenefitCalculationPostRetirementDeath.FindBenefitCalculationHeader(aintBenefitCalculationId))
            {
                lbusBenefitCalculationPostRetirementDeath.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusBenefitCalculationPostRetirementDeath.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                lbusBenefitCalculationPostRetirementDeath.ibusPerson.FindPerson(lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.person_id);
                lbusBenefitCalculationPostRetirementDeath.ibusPerson.LoadPersonAccounts();
                lbusBenefitCalculationPostRetirementDeath.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                if (!lbusBenefitCalculationPostRetirementDeath.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    lbusBenefitCalculationPostRetirementDeath.LoadAllRetirementContributions(
                    lbusBenefitCalculationPostRetirementDeath.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    lbusBenefitCalculationPostRetirementDeath.LoadAllRetirementContributions(null);
                }

                lbusBenefitCalculationPostRetirementDeath.LoadBenefitCalculationDetails();
                lbusBenefitCalculationPostRetirementDeath.LoadDisabilityRetireeIncreases();
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationPostRetirementDeath.ibusPerson;
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationPostRetirementDeath.ibusPerson.iclbPersonAccount;
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.retirement_date;
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationPostRetirementDeath.ibusPerson.icdoPerson.idtDateofBirth, lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.retirement_date);

                lbusBenefitCalculationPostRetirementDeath.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)                     
                lbusBenefitCalculationPostRetirementDeath.idecSurvivorAgeAtDeath = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.date_of_death);
                lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.istrPersonMPID = lbusBenefitCalculationPostRetirementDeath.ibusPerson.icdoPerson.mpi_person_id;
                ////Ticket#137952
                if (lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Count() > 0)
                {
                    lbusBenefitCalculationPostRetirementDeath.idecOverriddenSurvivorAmount = lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Where(i => i.icdoBenefitCalculationOptions.overridden_benefit_amount != 0).Select(y => y.icdoBenefitCalculationOptions.overridden_benefit_amount).FirstOrDefault();

                }
                if (lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.beneficiary_person_id > 0)
                {
                    lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.istrSurvivorTypeValue = busConstant.PERSON;

                    DataTable ldtbSurvivrMPID = busBase.Select("cdoPerson.GetPersonMPID", new object[1] { lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.beneficiary_person_id });
                    if (ldtbSurvivrMPID.Rows.Count > 0)
                    {
                        lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.istrSurvivorMPID = ldtbSurvivrMPID.Rows[0][enmPerson.mpi_person_id.ToString()].ToString();
                    }
                }
                else
                {
                    lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.istrSurvivorTypeValue = busConstant.ORGANIZATION;
                    DataTable ldtbOrganizationMPID = busBase.Select("cdoOrganization.GetOrgDetailsByOrganizationId", new object[1] { lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.organization_id });

                    if (ldtbOrganizationMPID.Rows.Count > 0)
                    {
                        lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.istrOrganizationId = ldtbOrganizationMPID.Rows[0][enmOrganization.mpi_org_id.ToString()].ToString();
                        lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.istrOrganizationName = ldtbOrganizationMPID.Rows[0][enmOrganization.org_name.ToString()].ToString();
                    }
                }
                lbusBenefitCalculationPostRetirementDeath.istrmanger = lbusBenefitCalculationPostRetirementDeath.Checkrole();
                if (lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationDetail.Count > 0)
                {
                    lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id;

                    if (lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id ==
                        lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.iintPlanId).Count() > 0)
                    {
                        lbusBenefitCalculationPostRetirementDeath.istrPlanName = lbusBenefitCalculationPostRetirementDeath.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode;
                    }
                    lbusBenefitCalculationPostRetirementDeath.GetFund();
                    if (lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                    {
                        lbusBenefitCalculationPostRetirementDeath.LoadSpecialAccounts();
                        lbusBenefitCalculationPostRetirementDeath.LoadDetailsGrid();
                    }
                }
                else
                {
                    busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lbusPayeeAccount.FindPayeeAccount(lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.payee_account_id);
                    lbusPayeeAccount.LoadBenefitDetails();
                    lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader.iintPlanId = lbusPayeeAccount.icdoPayeeAccount.iintPlanId;

                }

                //Temp For Correspondence : Need a better solution
                #region Load Object For Correspondence
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitCalculationRetirement.icdoBenefitCalculationHeader = lbusBenefitCalculationPostRetirementDeath.icdoBenefitCalculationHeader;
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitCalculationRetirement.ibusPerson = lbusBenefitCalculationPostRetirementDeath.ibusPerson;
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitCalculationRetirement.ibusBenefitApplication = lbusBenefitCalculationPostRetirementDeath.ibusBenefitApplication;
                lbusBenefitCalculationPostRetirementDeath.ibusBenefitCalculationRetirement.iclbBenefitCalculationDetail = lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationDetail;
                #endregion

                //To display child grid - start
                lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationOptionsChildGrid = new Collection<busBenefitCalculationOptions>();
                //To load all childs from first parent.
                foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions ?? new Collection<busBenefitCalculationOptions>())
                {
                    lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationOptionsChildGrid.Add(lbusBenefitCalculationOptions);
                }
                //To load all childs from all parents.
                //foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationDetail)
                //{
                //    foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in lbusBenefitCalculationDetail.iclbBenefitCalculationOptions ?? new Collection<busBenefitCalculationOptions>())
                //    {
                //        lbusBenefitCalculationPostRetirementDeath.iclbBenefitCalculationOptionsChildGrid.Add(lbusBenefitCalculationOptions);
                //    }
                //}
                //To display child grid - end
            }
            return lbusBenefitCalculationPostRetirementDeath;
        }


        public busBenefitCalculationDetail LoadIAPBenefitAmountDetails(int aintBenefitCalculationHeaderID)
        {
            busBenefitCalculationHeader lbusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            lbusBenefitCalculationHeader.FindBenefitCalculationHeader(aintBenefitCalculationHeaderID);
            lbusBenefitCalculationHeader.ibusPerson = new busPerson();
            lbusBenefitCalculationHeader.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();

            lbusBenefitCalculationHeader.ibusPerson.FindPerson(lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.person_id);
            lbusBenefitCalculationHeader.ibusPerson.LoadPersonAccounts();
            lbusBenefitCalculationHeader.LoadBenefitCalculationDetails();

            busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
            lbusBenefitCalculationDetail.iclbIAPAmountDetails = new Collection<busIAPAmountDetails>();

            busCalculation lbusCalculation = new busCalculation();

            decimal ldecIAPHours4QtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPHoursA2forQtrAlloc = busConstant.ZERO_DECIMAL;
            decimal ldecIAPPercent4forQtrAlloc = busConstant.ZERO_DECIMAL;

            #region To Set Values for IAP QTR Allocations
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            SqlParameter[] parameters = new SqlParameter[3];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
            SqlParameter param3 = new SqlParameter("@TODATE", DbType.DateTime);

            param1.Value = lbusBenefitCalculationHeader.ibusPerson.icdoPerson.istrSSNNonEncrypted;
            parameters[0] = param1;

            busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
            lbusIapAllocationSummary.LoadLatestAllocationSummary();

            param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year);
            parameters[1] = param2;

            param3.Value = busGlobalFunctions.GetLastDayOfWeek(lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date);//PROD PIR 113
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


            lbusCalculation.GetIAPAndSpecialAccountBalance(busConstant.BOOL_FALSE, null, lbusBenefitCalculationHeader.iclbBenefitCalculationDetail, lbusBenefitCalculationHeader, null,
                                                      lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date, ldecIAPHours4QtrAlloc, ldecIAPHoursA2forQtrAlloc, ldecIAPPercent4forQtrAlloc);

            busIAPAmountDetails lbusIAPAmountDetails = new busIAPAmountDetails { icdoIAPAmountDetails = new cdoIAPAmountDetails() };
            lbusIAPAmountDetails.idecTotal = lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault()?.icdoBenefitCalculationDetail.idecTotal??0;
            lbusIAPAmountDetails.idecQuaterlyAllocation = lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault()?.icdoBenefitCalculationDetail.idecQuaterllyAllocation??0;
            lbusIAPAmountDetails.idecRate = Math.Round( lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault()?.icdoBenefitCalculationDetail.idecRate??0,5);
            lbusIAPAmountDetails.iintQuaterly = lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault()?.icdoBenefitCalculationDetail.iintQuater??0;
            lbusIAPAmountDetails.idecPrevYearEndingBalance = (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault()?.icdoBenefitCalculationDetail.idecTotal??0) - (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault()?.icdoBenefitCalculationDetail.idecQuaterllyAllocation??0);

            lbusBenefitCalculationDetail.iclbIAPAmountDetails.Add(lbusIAPAmountDetails);

            return lbusBenefitCalculationDetail;

        }


		public busBenefitInterestRateLookup LoadBenefitInterestRates(DataTable adtbSearchResult)
		{
			busBenefitInterestRateLookup lobjBenefitInterestRateLookup = new busBenefitInterestRateLookup();
			lobjBenefitInterestRateLookup.LoadBenefitInterestRates(adtbSearchResult);
			return lobjBenefitInterestRateLookup;
		}

        protected override void InitializeNewChildObject(object aobjParentObject, busBase aobjChildObject)
        {
            if (iobjPassInfo.istrFormName == "wfmBenefitCalculationPostRetirementDeathMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == "grvBenefitCalculationDetailIAP")
            {
                if (aobjChildObject is busBenefitCalculationDetail)
                {
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_grvBenefitCalculationDetailIAP"];

                    busBenefitCalculationDetail lbusBenefitCalculationDetail = (busBenefitCalculationDetail)aobjChildObject;
                    Hashtable lhstParam = new Hashtable();

                    switch (Param["istrSpecialAccount"].ToString())
                    {
                        case busConstant.L52_SPL_ACC:
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag = busConstant.FLAG_YES;
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccountDescrioption = "IAP (Local-52 Special Account)";
                            break;
                        case busConstant.L161_SPL_ACC:
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag = busConstant.FLAG_YES;
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccountDescrioption = "IAP (Local-161 Special Account)";
                            break;
                        default:
                            lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccountDescrioption = "IAP";
                            break;
                    }

                    lhstParam.Add("benefit_option_value", Param["istrBenefitOptionValue"].ToString());

                    string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvCalculation");
                    IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                    DataTable ldtbBenefitDescription = lsrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetBenefitOptionDescription", lhstParam, iobjPassInfo.idictParams);

                    if (ldtbBenefitDescription.Rows.Count > 0)
                    {
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrBenefitOptionValueDescrioption = Convert.ToString(ldtbBenefitDescription.Rows[0][0]);
                    }
                }
            }
            base.InitializeNewChildObject(aobjParentObject, aobjChildObject);
        }
    }
}
