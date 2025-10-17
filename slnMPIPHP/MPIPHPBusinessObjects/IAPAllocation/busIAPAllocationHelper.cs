using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using MPIPHP.CustomDataObjects;
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busIAPAllocationHelper : busMPIPHPBase
    {
        //property to contain IAP Allocation factors
        public DataTable idtIAPAllocationFactor { get; set; }

        /// <summary>
        /// method to load the iap allocation factors
        /// </summary>
        public void LoadIAPAllocationFactor()
        {
            idtIAPAllocationFactor = new DataTable();
            idtIAPAllocationFactor = Select("cdoIapAllocationFactor.LookUp", new object[0] { });
        }

        /// <summary>
        /// Method to calculate IAP Allocation 1 amount
        /// </summary>
        /// <param name="aintPlanYear">Plan Year</param>
        /// <param name="adecPrevYearAccountBalance">Pervious Year IAP Account balance</param>
        /// <returns>Allocation 1 amount</returns>
        public decimal CalculateAllocation1Amount(int aintPlanYear, decimal adecPrevYearAccountBalance, int aintQuarter, ref decimal adecAllocationFactor)
        {
            decimal ldecAllocationAmount = 0.00M;
            //decimal ldecAllocationFactor = 0.0000000000M;

            if (idtIAPAllocationFactor == null)
                LoadIAPAllocationFactor();

            DataRow[] ldrIAPFactor = idtIAPAllocationFactor.FilterTable(utlDataType.Numeric, "plan_year", aintPlanYear);

            if (ldrIAPFactor.Length > 0)
            {
                switch (aintQuarter)
                {
                    case 1: adecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc1_qf1_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc1_qf1_factor"]) : 0.0000000000M;
                        break;
                    case 2: adecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc1_qf2_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc1_qf2_factor"]) : 0.0000000000M;
                        break;
                    case 3: adecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc1_qf3_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc1_qf3_factor"]) : 0.0000000000M;
                        break;
                    case 4: adecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc1_qf4_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc1_qf4_factor"]) : 0.0000000000M;
                        break;
                }
                ldecAllocationAmount = Math.Round(adecAllocationFactor * adecPrevYearAccountBalance, 2, MidpointRounding.AwayFromZero);
            }

            return ldecAllocationAmount;
        }

        /// <summary>
        /// Method to calculate Allocation 2 amount
        /// </summary>
        /// <param name="aintPlanYear">Plan Year</param>
        /// <param name="adecThru79Hours">Pension Prior to 1979, including 1979</param>
        /// <param name="adecYTDHours">Compuation Year's total hours</param>
        /// <param name="adtRetirementDate">Retirement Date</param>
        /// <param name="adtMinimumDistributionDate">Minimum distribution Date</param>
        /// <param name="adtDateofDeath">Date of death</param>
        /// <returns>Allocation 2 Amount</returns>
        public decimal CalculateAllocation2Amount(int aintPlanYear, decimal adecThru79Hours, decimal adecYTDHours, DateTime adtRetirementDate, DateTime adtMinimumDistributionDate, DateTime adtDateofDeath)
        {
            decimal ldecAllocationAmount = 0.00M;
            decimal ldecAllocationFactor = 0.0000000000M;

            if (idtIAPAllocationFactor == null)
                LoadIAPAllocationFactor();

            DataRow[] ldrIAPFactor = idtIAPAllocationFactor.FilterTable(utlDataType.Numeric, "plan_year", aintPlanYear);

            if (ldrIAPFactor.Length > 0)
            {
                ldecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc2_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc2_factor"]) : 0.0000000000M;

                if (aintPlanYear >= 2000 && (aintPlanYear == adtRetirementDate.Year || aintPlanYear == adtDateofDeath.Year))
                    ldecAllocationAmount = Math.Round(0.305M * adecYTDHours, 2, MidpointRounding.AwayFromZero);
                else if (aintPlanYear < 2000 && (aintPlanYear == adtRetirementDate.Year || aintPlanYear == adtDateofDeath.Year || aintPlanYear == adtMinimumDistributionDate.Year))
                    ldecAllocationAmount = 0.00M;
                //Rohan MD-Re
                else if (aintPlanYear <= 1990)
                    ldecAllocationAmount = Math.Round(ldecAllocationFactor * (adecThru79Hours + adecYTDHours), 2, MidpointRounding.AwayFromZero);
                else
                    ldecAllocationAmount = Math.Round(ldecAllocationFactor * adecYTDHours, 2, MidpointRounding.AwayFromZero);
            }

            return ldecAllocationAmount;
        }

        /// <summary>
        /// Method to calculate Allocation 2 Investment or forfeited amount
        /// </summary>
        /// <param name="aintPlanYear">Plan Year</param>
        /// <param name="adecYTDHours">Compuation Year's total hours</param>
        /// <param name="adtRetirementDate">Retirement Date</param>
        /// <param name="adtMinimumDistributionDate">Minimum distribution Date</param>
        /// <param name="adtDateofDeath">Date of death</param>
        /// <param name="astrFlag">Flag to identify whether to calculate for Investment or Forfeiture</param>
        /// <returns>Allocation 2 Investment or forfeited Amount</returns>
        public decimal CalculateAllocation2InvstOrFrftAmount(int aintPlanYear, decimal adecYTDHours, DateTime adtRetirementDate, DateTime adtMinimumDistributionDate, DateTime adtDateofDeath, string astrFlag)
        {
            decimal ldecAllocationAmount = 0.00M;
            decimal ldecAllocationFactor = 0.0000000000M;

            if (idtIAPAllocationFactor == null)
                LoadIAPAllocationFactor();

            DataRow[] ldrIAPFactor = idtIAPAllocationFactor.FilterTable(utlDataType.Numeric, "plan_year", aintPlanYear);

            if (ldrIAPFactor.Length > 0)
            {
                if (astrFlag.ToUpper() == busConstant.IAPAllocationInvestmentFlag.ToUpper())
                    ldecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc2_invst_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc2_invst_factor"]) : 0.0000000000M;
                else if (astrFlag.ToUpper() == busConstant.IAPAllocationForfeitureFlag.ToUpper())
                    ldecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc2_frft_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc2_frft_factor"]) : 0.0000000000M;

                if (aintPlanYear >= 2008 && aintPlanYear != adtRetirementDate.Year && aintPlanYear != adtDateofDeath.Year && aintPlanYear != adtMinimumDistributionDate.Year)
                    ldecAllocationAmount = Math.Round(ldecAllocationFactor * adecYTDHours, 2, MidpointRounding.AwayFromZero);
            }

            return ldecAllocationAmount;
        }

        /// <summary>
        /// Method to calculate Allocation 3 amount
        /// </summary>
        /// <param name="aintPlanYear">Plan Year</param>
        /// <param name="adecThru79Hours">Pension Prior to 1979, including 1979</param>
        /// <param name="adecYTDHours">Compuation Year's total hours</param>
        /// <returns>Allocation 3 Amount</returns>
        public decimal CalculateAllocation3Amount(int aintPlanYear, decimal adecThru79Hours, decimal adecYTDHours)
        {
            decimal ldecAllocationAmount = 0.00M;
            decimal ldecAllocationFactor = 0.0000000000M;

            if (idtIAPAllocationFactor == null)
                LoadIAPAllocationFactor();

            DataRow[] ldrIAPFactor = idtIAPAllocationFactor.FilterTable(utlDataType.Numeric, "plan_year", aintPlanYear);

            if (ldrIAPFactor.Length > 0)
            {
                ldecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc3_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc3_factor"]) : 0.0000000000M;
                ldecAllocationAmount = Math.Round(ldecAllocationFactor * (adecThru79Hours + adecYTDHours), 2, MidpointRounding.AwayFromZero);
            }

            return ldecAllocationAmount;
        }

        /// <summary>
        /// Method to calculate Allocation 4 Investment or Forfeiture amount
        /// </summary>
        /// <param name="aintPlanYear">Computation Year</param>
        /// <param name="adecAllocation4Amount">Allocation 4 Amount</param>
        /// <param name="astrFlag">flag to identify whether to calculate investment or forfeiture</param>
        /// <returns>IAP Allocation 4 Investment or forfeiture amount</returns>
        public decimal CalculateAllocation4InvstOrFrftAmount(int aintPlanYear, decimal adecAllocation4Amount, string astrFlag)
        {
            decimal ldecAllocationAmount = 0.00M;
            decimal ldecAllocationFactor = 0.0000000000M;

            if (idtIAPAllocationFactor == null)
                LoadIAPAllocationFactor();

            DataRow[] ldrIAPFactor = idtIAPAllocationFactor.FilterTable(utlDataType.Numeric, "plan_year", aintPlanYear);

            if (ldrIAPFactor.Length > 0)
            {
                if (astrFlag.ToUpper() == busConstant.IAPAllocationInvestmentFlag.ToUpper())
                    ldecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc4_invst_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc4_invst_factor"]) : 0.0000000000M;
                else if (astrFlag.ToUpper() == busConstant.IAPAllocationForfeitureFlag.ToUpper())
                    ldecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc4_frft_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc4_frft_factor"]) : 0.0000000000M;

                ldecAllocationAmount = Math.Round(ldecAllocationFactor * adecAllocation4Amount, 2, MidpointRounding.AwayFromZero);
            }

            return ldecAllocationAmount;
        }

        /// <summary>
        /// Method to calculate IAP allocation 5 amount
        /// </summary>
        /// <param name="aintPlanYear">Computation year</param>
        /// <param name="aclbWorkHistory">IAP workhistory</param>
        /// <param name="ablnAgeLessThan55AsOfInceptionDate">Boolean flag to say age of participant less than 55 as of 08/01/1979</param>
        /// <returns>IAP Allocation amount for Affiliates</returns>
        public decimal CalcuateAllocation5AffliatesAmount(int aintPlanYear, Collection<cdoDummyWorkData> aclbWorkHistory, bool ablnAgeLessThan55AsOfInceptionDate)
        {
            decimal ldecAllocationAmount = 0.00M;
            decimal ldecAllocationFactor = 0.0000000000M;
            decimal ldecCummHours1, ldecCummHours2;
            ldecCummHours1 = ldecCummHours2 = 0.0M;
            cdoDummyWorkData lcdoDummyData1, lcdoDummyData2;

            if (idtIAPAllocationFactor == null)
                LoadIAPAllocationFactor();

            DataRow[] ldrIAPFactor = idtIAPAllocationFactor.FilterTable(utlDataType.Numeric, "plan_year", aintPlanYear);

            if (ldrIAPFactor.Length > 0)
            {
                ldecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc5_affl_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc5_affl_factor"]) : 0.0000000000M;

                if (aintPlanYear == 1996 && ablnAgeLessThan55AsOfInceptionDate)
                {
                    lcdoDummyData1 = aclbWorkHistory.Where(o => o.year == 1996).FirstOrDefault();
                    lcdoDummyData2 = aclbWorkHistory.Where(o => o.year == 1979).FirstOrDefault();
                    ldecAllocationAmount = Math.Round(ldecAllocationFactor * ((lcdoDummyData1 != null ? lcdoDummyData1.qualified_years_count : 0) - (lcdoDummyData2 != null ? lcdoDummyData2.qualified_years_count : 0)), 2, MidpointRounding.AwayFromZero);
                }
                else if (aintPlanYear == 1996 && !ablnAgeLessThan55AsOfInceptionDate)
                {
                    lcdoDummyData1 = aclbWorkHistory.Where(o => o.year == 1996).FirstOrDefault();
                    lcdoDummyData2 = aclbWorkHistory.Where(o => o.year == 1989).FirstOrDefault();
                    ldecAllocationAmount = Math.Round(ldecAllocationFactor * ((lcdoDummyData1 != null ? lcdoDummyData1.qualified_years_count : 0) - (lcdoDummyData2 != null ? lcdoDummyData2.qualified_years_count : 0)), 2, MidpointRounding.AwayFromZero);
                }
                else if (aintPlanYear > 1996)
                {
                    ldecCummHours1 = aclbWorkHistory.Where(o => o.year <= aintPlanYear).Sum(o => o.qualified_hours);
                    ldecCummHours2 = aclbWorkHistory.Where(o => o.year <= 1979).Sum(o => o.qualified_hours);
                    ldecAllocationAmount = Math.Round(ldecAllocationFactor * (ldecCummHours1 - ldecCummHours2), 2, MidpointRounding.AwayFromZero);
                }
            }

            return ldecAllocationAmount;
        }

        /// <summary>
        /// Method to calculate IAP Allocation amount for Non affiliates or Both
        /// </summary>
        /// <param name="aintPlanYear">Computation Year</param>
        /// <param name="adecYTDHours">Compuation Year's total hours</param>
        /// <param name="astrFlag">Flag to identify whether to calculate Non affl. amount or both</param>
        /// <returns>IAP Allocation amount</returns>
        public decimal CalcuateAllocation5NonAffOrBothAmount(int aintPlanYear, decimal adecYTDHours, string astrFlag)
        {
            decimal ldecAllocationAmount = 0.00M;
            decimal ldecAllocationFactor = 0.0000000000M;

            if (idtIAPAllocationFactor == null)
                LoadIAPAllocationFactor();

            DataRow[] ldrIAPFactor = idtIAPAllocationFactor.FilterTable(utlDataType.Numeric, "plan_year", aintPlanYear);

            if (ldrIAPFactor.Length > 0)
            {
                if (astrFlag.ToUpper() == busConstant.IAPAllocationNonAffiliatesFlag.ToUpper())
                    ldecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc5_nonaffl_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc5_nonaffl_factor"]) : 0.0000000000M;
                else if (astrFlag.ToUpper() == busConstant.IAPAllocationBothAffAndNonAffFlag.ToUpper())
                    ldecAllocationFactor = !Convert.ToBoolean(ldrIAPFactor[0]["alloc5_both_factor"].IsDBNull()) ? Convert.ToDecimal(ldrIAPFactor[0]["alloc5_both_factor"]) : 0.0000000000M;

                ldecAllocationAmount = Math.Round(ldecAllocationFactor * adecYTDHours, 2, MidpointRounding.AwayFromZero);
            }

            return ldecAllocationAmount;
        }

        /// <summary>
        /// Method to check whether the participant in affiliate or not
        /// </summary>
        /// <param name="aintComputationYear">Computation year</param>
        /// <param name="astrSSN">SSN</param>
        /// <returns>Boolean value</returns>
        public bool CheckParticipantIsAffiliate(int aintComputationYear, string astrSSN, utlPassInfo aobjPassInfo = null)
        {
            bool lblnResult = true;
            IDbConnection lconLegacy = null;
            if (lconLegacy == null && aobjPassInfo == null)
            {
                lconLegacy = DBFunction.GetDBConnection("Legacy");
            }
            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

            IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
            lobjParameter.ParameterName = "@SSN";
            lobjParameter.DbType = DbType.String;
            lobjParameter.Value = astrSSN.ToLower();
            lcolParameters.Add(lobjParameter);

            IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
            lobjParameter1.ParameterName = "@PLYR";
            lobjParameter1.DbType = DbType.Int32;
            lobjParameter1.Value = aintComputationYear;
            lcolParameters.Add(lobjParameter1);


            IDbDataParameter returnvalue = DBFunction.GetDBParameter();
            returnvalue.ParameterName = "@RETURN_VALUE";
            returnvalue.DbType = DbType.String;
            returnvalue.Direction = ParameterDirection.ReturnValue;
            lcolParameters.Add(returnvalue);

            IDataReader idt;


            //DataTable ldt = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataBetweenTwoDates", lconLegacy, null, lcolParameters);
            if (aobjPassInfo.IsNotNull())
                idt = DBFunction.DBExecuteProcedureResult("USP_OPUS_GET_NONAFFILIATE", lcolParameters, aobjPassInfo.iconFramework, null);
            else
                idt = DBFunction.DBExecuteProcedureResult("USP_OPUS_GET_NONAFFILIATE", lcolParameters, lconLegacy, null);
            DataTable ldt = new DataTable();
            ldt.Load(idt);
            if (lconLegacy.IsNotNull() && lconLegacy.State == ConnectionState.Open)
                lconLegacy.Close();

            if (ldt.IsNotNull() && ldt.Rows.Count > 0)
            {
                if (Convert.ToString(ldt.Rows[0][0]) == "Y")
                {
                    lblnResult = false;
                }
            }



            ////method to get the true union code for an year 
            //string lstrTrueUnionCode = GetTrueUnionCodeBySSNAndPlanYear(busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear - 1).AddDays(1), busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), astrSSN,aobjPassInfo);
            //string[] lstrNonAffiliateUnionCodes = HelperUtil.GetData1ByCodeValue(52, busConstant.NonAffiliateUnionCodes).Split(",");

            //foreach (string lstrUnionCode in lstrNonAffiliateUnionCodes)
            //{
            //    if (lstrTrueUnionCode == lstrUnionCode)
            //    {
            //        lblnResult = false;
            //        break;
            //    }
            //}

            return lblnResult;
        }

        /// <summary>
        /// Method to get the true union code by plan year and ssn
        /// </summary>
        /// <param name="aintPlanYear">Plan Year</param>
        /// <param name="astrSSN">SSN</param>
        /// <returns>True Union Code</returns>
        public string GetTrueUnionCodeBySSNAndPlanYear(DateTime adtPlanYearBeginDate, DateTime adtPlanYearEndDate, string astrSSN, utlPassInfo aobjPassInfo = null)
        {
            IDbConnection lconLegacy = null;
            if (lconLegacy == null && aobjPassInfo == null)
            {
                lconLegacy = DBFunction.GetDBConnection("Legacy");
            }
            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

            IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
            lobjParameter.ParameterName = "@SSN";
            lobjParameter.DbType = DbType.String;
            lobjParameter.Value = astrSSN.ToLower();
            lcolParameters.Add(lobjParameter);

            IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
            lobjParameter1.ParameterName = "@FromDate";
            lobjParameter1.DbType = DbType.DateTime;
            lobjParameter1.Value = adtPlanYearBeginDate;
            lcolParameters.Add(lobjParameter1);

            IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
            lobjParameter2.ParameterName = "@ToDate";
            lobjParameter2.DbType = DbType.DateTime;
            lobjParameter2.Value = adtPlanYearEndDate;
            lcolParameters.Add(lobjParameter2);

            IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
            lobjParameter3.ParameterName = "@TrueUnion";
            lobjParameter3.DbType = DbType.Int32;
            lobjParameter3.Direction = ParameterDirection.ReturnValue;
            lcolParameters.Add(lobjParameter3);

            if (aobjPassInfo.IsNotNull())
                DBFunction.DBExecuteProcedure("fn_GetTrueUnionBy_SSN_N_Date_OldWay", lcolParameters, aobjPassInfo.iconFramework, null);
            else
                DBFunction.DBExecuteProcedure("fn_GetTrueUnionBy_SSN_N_Date_OldWay", lcolParameters, lconLegacy, null);

            if (lconLegacy.IsNotNull() && lconLegacy.State == ConnectionState.Open)
                lconLegacy.Close();

            return lcolParameters[3].Value.ToString();
        }


        /// <summary>
        /// Method to calculate Allocation 4 Amount
        /// </summary>
        /// <param name="aintComputationYear">Comnputation Year</param>
        /// <param name="aintPersonAccountID">Person Account ID</param>
        /// <param name="adtIAPPercent">Datatable containing IAP contributions</param>
        /// <returns>Allocation Amount for the year</returns>
        public decimal CalculateAllocation4Amount(int aintComputationYear, DataTable adtIAPPercent, decimal adecIAPPercent = 0.00M)
        {
            decimal ldecAllocationAmount, ldecTotalIAPPercent;
            ldecAllocationAmount = ldecTotalIAPPercent = 0.00M;
            decimal ldecIRSLimit = 0.00M;
            DataTable ldtIAPLimit = iobjPassInfo.isrvDBCache.GetCacheData("sgt_iap_contribution_limit", null);
            DataRow[] ldrFiltered = ldtIAPLimit.FilterTable(utlDataType.Numeric, "plan_year", aintComputationYear);
            if (ldrFiltered.Length > 0)
            {
                ldecIRSLimit = ldrFiltered[0]["contribution_limit"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldrFiltered[0]["contribution_limit"]);
            }

            if (adecIAPPercent != 0.00M)
            {
                if (adecIAPPercent > ldecIRSLimit)
                    ldecAllocationAmount = ldecIRSLimit;
                else
                    ldecAllocationAmount = adecIAPPercent;
            }
            else if (adtIAPPercent != null)
            {
                foreach (DataRow ldr in adtIAPPercent.Rows)
                {
                    if (ldr.Table.Columns.Contains("lateiappercent"))
                    {
                        ldecTotalIAPPercent += (ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"])) + (ldr["lateiappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["lateiappercent"]));
                    }
                    else
                    {
                        ldecTotalIAPPercent += (ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"]));
                    }

                }

                if (ldecTotalIAPPercent > ldecIRSLimit)
                    ldecAllocationAmount = ldecIRSLimit;
                else
                    ldecAllocationAmount = ldecTotalIAPPercent;
            }

            return ldecAllocationAmount;
        }
    }
}
