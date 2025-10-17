using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using MPIPHP.Common;
using MPIPHP.DataObjects;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;


namespace MPIPHPJobService
{
    public class busConversionInterestPostingBatch : busBatchHandler
    {
        private object iobjLock = null;
        int iintCounter = busConstant.ZERO_INT, iintTotalCount = busConstant.ZERO_INT;

        public busConversionInterestPostingBatch()
        {
        }

        public override void Process()
        {
            base.Process();

            DBFunction.DBExecuteScalar("UPDATE STATISTICS dbo.SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION;",  iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);
            DBFunction.DBExecuteScalar("SET NOCOUNT ON;",  iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);

                
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            int lintEarliestComputationYear = GetEarliestComputationYear();
            int lintLastComputationYear = 2012;
            iobjLock = new object();

            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions lpoParallelOptions = new ParallelOptions();
            lpoParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

            for (int lintCompYr = lintEarliestComputationYear + 1; lintCompYr <= lintLastComputationYear; lintCompYr++)
            {
                decimal ldecRateOfInterest = 0;
                DataTable ldtbInterestRateInformation = busBase.Select<cdoBenefitInterestRate>(new string[1] { enmBenefitInterestRate.year.ToString() }, new object[1] { Math.Max(lintCompYr, 1975) }, null, null);

                if (ldtbInterestRateInformation.Rows.Count > 0)
                {
                    ldecRateOfInterest = Convert.ToDecimal(ldtbInterestRateInformation.Rows[0][enmBenefitInterestRate.rate_of_interest.ToString()]);
                }


                // Get the EE and UVHP Contribution Amounts from the table until the Computation Year lintCompYr
                DataTable ldtPersonRetirementContributionInformation = busBase.Select("cdoPersonAccountRetirementContribution.GetAllEE&UVHPContributionAmountsUptoCompYr",
                                                                                        new object[1] { lintCompYr - 1 });


                

                //DataTable ldtbPaymentInfo = busBase.Select("cdoPersonAccountRetirementContribution.GetAllEE&UVHPPaymentsUptoCompYr",
                //                                                                        new object[1] { lintCompYr });

                //DataTable ldtbCurrentYearContributions = busBase.Select("cdoPersonAccountRetirementContribution.GetAllEE&UVHPContributionAmountsForCurrentCompYr", new object[1] { lintCompYr });

                Parallel.ForEach(ldtPersonRetirementContributionInformation.AsEnumerable(), lpoParallelOptions, (ldrPersonRetirementContribution, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "ConversionInterestPostingBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    DateTime ldtCalculationDate = new DateTime();

                    if (Convert.ToString(ldrPersonRetirementContribution["retirement_date"]).IsNotNullOrEmpty())
                    {
                        ldtCalculationDate = Convert.ToDateTime(ldrPersonRetirementContribution["retirement_date"]);
                    }

                    if ((ldtCalculationDate == DateTime.MinValue && DateTime.Now.Year != lintCompYr) || (ldtCalculationDate != DateTime.MinValue && ldtCalculationDate.Year + 1 > lintCompYr))
                    {
                       // if (Convert.ToInt32(ldrPersonRetirementContribution[enmPersonAccount.person_account_id.ToString()]) == 84316)
                       // {
                            //DataRow[] ldrPaymentInfo = ldtbPaymentInfo.FilterTable(utlDataType.Numeric, enmPersonAccountRetirementContribution.person_account_id.ToString(),
                            //                            Convert.ToInt32(ldrPersonRetirementContribution[enmPersonAccountRetirementContribution.person_account_id.ToString()]));

                            //DataRow[] ldrCurrentYearContributions = ldtbCurrentYearContributions.FilterTable(utlDataType.Numeric, enmPersonAccountRetirementContribution.person_account_id.ToString(),
                            //                                        Convert.ToInt32(ldrPersonRetirementContribution[enmPersonAccountRetirementContribution.person_account_id.ToString()]));

                            //ComputeAndPostInterest(ldrPersonRetirementContribution, lobjPassInfo, ldecRateOfInterest, lintCompYr, ldrPaymentInfo, ldtCalculationDate, ldrCurrentYearContributions);
                            ComputeAndPostInterest(ldrPersonRetirementContribution, lobjPassInfo, ldecRateOfInterest, lintCompYr, ldtCalculationDate);
                        //}
                    }

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;

                });

                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;
            }
        }


        public int GetEarliestComputationYear()
        {
            object lobjComputationYear = null;
            lobjComputationYear = DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.GetEarliestComputationYear",
                                    new object[] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return Convert.ToInt32(lobjComputationYear);
        }

        //public void ComputeAndPostInterest(DataRow adrPersonRetirementContribution, utlPassInfo autlPassInfo, decimal adecRateOfInterest, int aintComputationYear, DataRow[] ldrPaymentInfo, DateTime adtCalculationDate, DataRow[] ldrCurrentYearContributions)
        public void ComputeAndPostInterest(DataRow adrPersonRetirementContribution, utlPassInfo autlPassInfo, decimal adecRateOfInterest, int aintComputationYear, DateTime adtCalculationDate)
        {
            lock (iobjLock)
            {
                iintCounter++;
                iintTotalCount++;
                if (iintCounter == 100)
                {
                    String lstrMsg = iintTotalCount + " : " + " Records Has Been Processed";
                    PostInfoMessage(lstrMsg);
                    iintCounter = 0;
                }
            }

            autlPassInfo.BeginTransaction();
            try
            {


                decimal ldecTotalPrevYearEEContribAmt = 0, ldecTotalPrevYearEEInterestAmt = 0, ldecTotalPrevYearUVHPContribAmt = 0, ldecTotalPrevYearUVHPInterestAmt = 0;
                decimal ldecTotalPrevYearUVHPPayment = 0, ldecTotalPrevYearUVHPInterestPayment = 0, ldecTotalPrevYearEEPayment = 0, ldecTotalPrevYearEEInterestPayment = 0;
                decimal ldecCurrentYearUVHPPayment = 0, ldecCurrentYearUVHPInterestPayment = 0, ldecCurrentYearEEPayment = 0, ldecCurrentYearEEInterestPayment = 0;
                decimal ldecCurrentYearEEContribution = 0, ldecCurrentYearUVHPContribution = 0, ldecCurrentYearEEInterest = 0, ldecCurrentYearUVHPInterest = 0;

                int lintPersonAccountId = Convert.ToInt32(adrPersonRetirementContribution[0]);

                busBase lobjBase = new busBase();
                int lintForfeitureYear =0,lintWithdrawalYear =0;
                if(adrPersonRetirementContribution["FORFEITURE_YEAR"] != null && Convert.ToString(adrPersonRetirementContribution["FORFEITURE_YEAR"]).IsNotNullOrEmpty())
                    lintForfeitureYear = Convert.ToInt32(adrPersonRetirementContribution["FORFEITURE_YEAR"]);
                if(adrPersonRetirementContribution["WDRL_TRANSACTION_DATE"] != null && Convert.ToString(adrPersonRetirementContribution["WDRL_TRANSACTION_DATE"]).IsNotNullOrEmpty())
                    lintWithdrawalYear = Convert.ToDateTime(adrPersonRetirementContribution["WDRL_TRANSACTION_DATE"]).Year;


                if ((lintForfeitureYear > 0 && lintWithdrawalYear > 0 && aintComputationYear > lintForfeitureYear && aintComputationYear <= lintWithdrawalYear)
                    || (lintWithdrawalYear > 0 &&  aintComputationYear <= lintWithdrawalYear))
                {
                    if (Convert.ToString(adrPersonRetirementContribution["CONTRIBUTION_SUBTYPE_VALUE"]) == busConstant.CONTRIBUTION_SUBTYPE_VESTED)
                        return;
                    else
                    {
                        DataTable ldtblContributionBetWithdrawalAndForteiture = busBase.Select("cdoPersonAccountRetirementContribution.GetContributionsBetweenfortritureAndWithdrawal", new object[2] { aintComputationYear, lintPersonAccountId });
                        Collection<busPersonAccountRetirementContribution> lclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                        lclbPersonAccountRetirementContribution = lobjBase.GetCollection<busPersonAccountRetirementContribution>(ldtblContributionBetWithdrawalAndForteiture, "icdoPersonAccountRetirementContribution");


                        foreach (busPersonAccountRetirementContribution lobjPersonAccountRetirementContribution in lclbPersonAccountRetirementContribution)
                        {
                            if (Convert.ToString(adrPersonRetirementContribution["CONTRIBUTION_TYPE_VALUE"]) == lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value)
                            {
                                lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value = busConstant.CONTRIBUTION_SUBTYPE_NON_VESTED;
                                lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Update();
                            }
                        }
                        


                        if (Convert.ToString(adrPersonRetirementContribution["CONTRIBUTION_TYPE_VALUE"]) == busConstant.CONTRIBUTION_TYPE_EE)
                        {
                            if (lclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_EE).Count() > 0)
                            {
                                ldecCurrentYearEEContribution = lclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_EE).Sum(t => t.icdoPersonAccountRetirementContribution.ee_contribution_amount);
                                ldecCurrentYearEEInterest = lclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_EE).Sum(t => t.icdoPersonAccountRetirementContribution.ee_int_amount);
                            }
                        }
                        else
                        {
                            if (lclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_UVHP).Count() > 0)
                            {
                                ldecCurrentYearUVHPContribution = lclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_UVHP).Sum(t => t.icdoPersonAccountRetirementContribution.uvhp_amount);
                                ldecCurrentYearUVHPInterest = lclbPersonAccountRetirementContribution.Where(t => t.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_UVHP).Sum(t => t.icdoPersonAccountRetirementContribution.uvhp_int_amount);
                            }
                        }
                    }
                }


                //int lintComputationalYear = 0;
              

                ldecTotalPrevYearEEContribAmt += Convert.ToDecimal(adrPersonRetirementContribution[1]);
                ldecTotalPrevYearEEInterestAmt += Convert.ToDecimal(adrPersonRetirementContribution[2]);
                ldecTotalPrevYearUVHPContribAmt += Convert.ToDecimal(adrPersonRetirementContribution[3]);
                ldecTotalPrevYearUVHPInterestAmt += Convert.ToDecimal(adrPersonRetirementContribution[4]);

                ldecCurrentYearEEContribution += Convert.ToDecimal(adrPersonRetirementContribution[5]);
                ldecCurrentYearEEInterest += Convert.ToDecimal(adrPersonRetirementContribution[6]);
                ldecCurrentYearUVHPContribution = Convert.ToDecimal(adrPersonRetirementContribution[7]);
                ldecCurrentYearUVHPInterest = Convert.ToDecimal(adrPersonRetirementContribution[8]);

                ldecTotalPrevYearEEPayment += Convert.ToDecimal(adrPersonRetirementContribution[9]);
                ldecTotalPrevYearEEInterestPayment += Convert.ToDecimal(adrPersonRetirementContribution[10]);
                ldecTotalPrevYearUVHPPayment += Convert.ToDecimal(adrPersonRetirementContribution[11]);
                ldecTotalPrevYearUVHPInterestPayment += Convert.ToDecimal(adrPersonRetirementContribution[12]);

                ldecCurrentYearEEPayment += Convert.ToDecimal(adrPersonRetirementContribution[13]);
                ldecCurrentYearEEInterestPayment += Convert.ToDecimal(adrPersonRetirementContribution[14]);
                ldecCurrentYearUVHPPayment += Convert.ToDecimal(adrPersonRetirementContribution[15]);
                ldecCurrentYearUVHPInterestPayment += Convert.ToDecimal(adrPersonRetirementContribution[16]);


                

                //if (ldrPaymentInfo.Count() > 0)
                //{
                //    ldecUVHPPayment = Convert.ToDecimal(ldrPaymentInfo[0][3]);
                //    ldecUVHPInterestPayment = Convert.ToDecimal(ldrPaymentInfo[0][4]);
                //    ldecEEPayment = Convert.ToDecimal(ldrPaymentInfo[0][1]);
                //    ldecEEInterestPayment = Convert.ToDecimal(ldrPaymentInfo[0][2]);
                //    lintComputationalYear = Convert.ToInt32(ldrPaymentInfo[0][7]);
                //}

                
                //if (ldrCurrentYearContributions.Count() > 0)
                //{
                //    ldecCurrentYearEEContribution = Convert.ToDecimal(ldrCurrentYearContributions[0][1]);
                //    ldecCurrentYearUVHPContribution = Convert.ToDecimal(ldrCurrentYearContributions[0][3]);
                //}



                if (adtCalculationDate != DateTime.MinValue && adtCalculationDate.Year == aintComputationYear)
                {
                    decimal ldecEEPartialInterestAmount = Math.Round(((ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEInterestAmt + ldecTotalPrevYearEEPayment + ldecTotalPrevYearEEInterestPayment) * adecRateOfInterest) / 12 * (adtCalculationDate.Month - 1), 2, MidpointRounding.AwayFromZero);
                    ldecTotalPrevYearEEInterestAmt = ldecEEPartialInterestAmount;

                    //decimal ldecEEPartialInterestPaymentAmount = Math.Round(((ldecTotalPrevYearEEPayment + ldecTotalPrevYearEEInterestPayment) * adecRateOfInterest) / 12 * (adtCalculationDate.Month - 1), 2, MidpointRounding.AwayFromZero);
                    //ldecTotalPrevYearEEInterestPayment = ldecEEPartialInterestPaymentAmount;

                    decimal ldecUVHPPartialInterestAmount = Math.Round(((ldecTotalPrevYearUVHPContribAmt + ldecTotalPrevYearUVHPInterestAmt + ldecTotalPrevYearUVHPPayment + ldecTotalPrevYearUVHPInterestPayment) * adecRateOfInterest) / 12 * (adtCalculationDate.Month - 1), 2, MidpointRounding.AwayFromZero);
                    ldecTotalPrevYearUVHPInterestAmt = ldecUVHPPartialInterestAmount;

                    //decimal ldecUVHPPartialInterestPaymentAmount = Math.Round(((ldecTotalPrevYearUVHPPayment + ldecTotalPrevYearUVHPInterestPayment) * adecRateOfInterest) / 12 * (adtCalculationDate.Month - 1), 2, MidpointRounding.AwayFromZero);
                    //ldecTotalPrevYearUVHPInterestPayment = ldecUVHPPartialInterestPaymentAmount;
                }

                busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution =
                    new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = lintPersonAccountId;

                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date = DateTime.Now;
                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_id = busConstant.CONTRIBUTION_TYPE_CODE_ID;
                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_id = busConstant.CONTRIBUTION_SUBTYPE_CODE_ID;
                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_id = busConstant.TRANSACTION_TYPE_CODE_ID;

                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value =
                            adrPersonRetirementContribution[enmPersonAccountRetirementContribution.contribution_subtype_value.ToString()].ToString();


                if (adrPersonRetirementContribution[enmPersonAccountRetirementContribution.contribution_type_value.ToString()].ToString() == busConstant.CONTRIBUTION_TYPE_UVHP &&
                    ((ldecTotalPrevYearUVHPContribAmt + ldecTotalPrevYearUVHPInterestAmt + ldecTotalPrevYearUVHPInterestPayment + ldecTotalPrevYearUVHPPayment + ldecCurrentYearUVHPPayment
                    + ldecCurrentYearUVHPInterestPayment) != busConstant.ZERO_DECIMAL))
                {
                    if (adtCalculationDate != DateTime.MinValue && adtCalculationDate.Year == aintComputationYear)
                    {
                        if (ldecTotalPrevYearUVHPInterestAmt > 0)
                        {
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount = ldecTotalPrevYearUVHPInterestAmt;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year = aintComputationYear;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear).AddDays(1);
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TRANSACTION_TYPE_INTEREST;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_UVHP;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount = busConstant.ZERO_DECIMAL;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
                        }
                    }
                    else
                    {
                        if (aintComputationYear < 1976)
                        {
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount = Math.Round((ldecTotalPrevYearUVHPContribAmt + ldecTotalPrevYearUVHPPayment) * adecRateOfInterest, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount = Math.Round((ldecTotalPrevYearUVHPContribAmt + ldecTotalPrevYearUVHPInterestAmt + ldecTotalPrevYearUVHPPayment + ldecTotalPrevYearUVHPInterestPayment) * adecRateOfInterest, 2, MidpointRounding.AwayFromZero);
                        }


                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year = aintComputationYear;
                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear).AddDays(1);
                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TRANSACTION_TYPE_INTEREST;
                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_UVHP;
                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount = busConstant.ZERO_DECIMAL;
                        if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount > 0)
                        {
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
                        }

                        if (//(lintComputationalYear == aintComputationYear) &&
                            //(Convert.ToInt32(ldecTotalPrevYearUVHPContribAmt + ldecTotalPrevYearUVHPPayment + ldecCurrentYearUVHPPayment) == 0) || (Convert.ToInt32(ldecTotalPrevYearUVHPContribAmt + ldecTotalPrevYearUVHPPayment + ldecCurrentYearUVHPPayment + ldecCurrentYearUVHPContribution) == 0)
                            //|| ((ldecTotalPrevYearUVHPInterestAmt + ldecCurrentYearUVHPInterest + ldecTotalPrevYearUVHPInterestPayment + ldecCurrentYearUVHPInterestPayment) < 0))
                            ldecCurrentYearUVHPPayment != 0.0m)
                        {
                            if ((ldecTotalPrevYearUVHPContribAmt + ldecTotalPrevYearUVHPPayment + ldecCurrentYearUVHPContribution + ldecCurrentYearUVHPPayment) < 0)
                            {
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_contribution_amount =
                                -(ldecTotalPrevYearUVHPContribAmt + ldecTotalPrevYearUVHPPayment + ldecCurrentYearUVHPContribution + ldecCurrentYearUVHPPayment);
                            }

                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount = Math.Round(-(ldecTotalPrevYearUVHPInterestAmt + ldecTotalPrevYearUVHPInterestPayment + ldecCurrentYearUVHPInterestPayment +
                                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount), 2, MidpointRounding.AwayFromZero);
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year = aintComputationYear;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear).AddDays(1);
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.RCTransactionTypeAdjustment;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_UVHP;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount = busConstant.ZERO_DECIMAL;
                            //if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount > 0)
                            //{
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
                            //}
                        }
                    }
                }

                if (adrPersonRetirementContribution[enmPersonAccountRetirementContribution.contribution_type_value.ToString()].ToString() == busConstant.CONTRIBUTION_TYPE_EE &&
               ((ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEInterestAmt + ldecTotalPrevYearEEPayment + ldecTotalPrevYearEEInterestPayment
               + ldecCurrentYearEEPayment + ldecCurrentYearEEInterestPayment) != busConstant.ZERO_DECIMAL))

                {
                    if (adtCalculationDate != DateTime.MinValue && adtCalculationDate.Year == aintComputationYear)
                    {
                        if (ldecTotalPrevYearEEInterestAmt > 0)
                        {
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount = ldecTotalPrevYearEEInterestAmt;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year = aintComputationYear;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear).AddDays(1);
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TRANSACTION_TYPE_INTEREST;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount = busConstant.ZERO_DECIMAL;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
                        }
                    }
                    else
                    {
                        if (aintComputationYear < 1976)
                        {
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount = Math.Round((ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEPayment) * adecRateOfInterest, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount = Math.Round((ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEInterestAmt + ldecTotalPrevYearEEPayment + ldecTotalPrevYearEEInterestPayment) * adecRateOfInterest, 2, MidpointRounding.AwayFromZero);
                        }


                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year = aintComputationYear;
                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear).AddDays(1);
                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TRANSACTION_TYPE_INTEREST;
                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                        lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount = busConstant.ZERO_DECIMAL;
                        if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount > 0)
                        {
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
                        }

                        if (//(lintComputationalYear == aintComputationYear) &&
                            //(Convert.ToInt32(ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEPayment + ldecCurrentYearEEPayment) == 0) || (Convert.ToInt32(ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEPayment + ldecCurrentYearEEPayment + ldecCurrentYearEEContribution) == 0)
                            //|| ((ldecTotalPrevYearEEInterestAmt + ldecCurrentYearEEInterest + ldecTotalPrevYearEEInterestPayment + ldecCurrentYearEEInterestPayment) < 0 ))
                            ldecCurrentYearEEPayment != 0.0m)
                        {
                            if ((ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEPayment + ldecCurrentYearEEContribution + ldecCurrentYearEEPayment) < 0)
                            {
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_contribution_amount =
                                -(ldecTotalPrevYearEEContribAmt + ldecTotalPrevYearEEPayment + ldecCurrentYearEEContribution + ldecCurrentYearEEPayment);
                            }
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year = aintComputationYear;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear).AddDays(1);
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount = Math.Round(-(ldecTotalPrevYearEEInterestAmt + ldecTotalPrevYearEEInterestPayment + ldecCurrentYearEEInterestPayment +
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount), 2, MidpointRounding.AwayFromZero);
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.RCTransactionTypeAdjustment;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value = busConstant.CONTRIBUTION_TYPE_EE;
                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount = busConstant.ZERO_DECIMAL;
                            //if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount > 0)
                            //{
                                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
                            //}
                        }
                    }
                }

                autlPassInfo.Commit();
            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    String lstrMsg = "Error while Executing Batch,Error Message:" + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();

            }
        }
    }
}
