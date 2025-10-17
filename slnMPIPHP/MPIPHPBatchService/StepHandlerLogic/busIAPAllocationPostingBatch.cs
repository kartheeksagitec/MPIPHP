using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;
using MPIPHP.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using System.Linq;
using MPIPHPJobService;
using Sagitec.CustomDataObjects;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.Common;
using System.Threading.Tasks;
using Sagitec.ExceptionPub;

namespace MPIPHPJobService
{
    public class busIAPAllocationPostingBatch : busBatchHandler
    {
        //property to contain the iap allocation details
        public DataTable idtIAPAllocationDetail { get; set; }
        //property to contain allocation year
        public int iintAllocationYear { get; set; }
        //property to contain previous year iap allocation summary
        public busIapAllocationSummary ibusPrevYearAllocationSummary { get; set; }

        private object iobjLock = null;
        int lintCounter = 0;
        int lintCount = 0;
        int lintTotalCount = 0;

        public override void Process()
        {
            try
            {
                LoadIAPAllocationYear();
                LoadIAPAllocationDetail();
                PostIAPAllocationIntoContribution();
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                String lstrMsg = "Error while executing the batch: " + ex.ToString();
                PostErrorMessage(lstrMsg);
            }
        }

        /// <summary>
        /// method to load the current iap allocation year
        /// </summary>
        private void LoadIAPAllocationYear()
        {
            if (ibusPrevYearAllocationSummary == null)
                LoadPreviousYearAllocationSummary();
            iintAllocationYear = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.computation_year;
        }

        /// <summary>
        /// Method to load the previous year iap allocation summary
        /// </summary>
        private void LoadPreviousYearAllocationSummary()
        {
            ibusPrevYearAllocationSummary = new busIapAllocationSummary();
            ibusPrevYearAllocationSummary.LoadLatestAllocationSummary();
        }

        /// <summary>
        /// method to load the iap allocation details
        /// </summary>
        private void LoadIAPAllocationDetail()
        {
            idtIAPAllocationDetail = busBase.Select("cdoIapAllocationDetail.LoadIAPAllocationDetailForPosting", new object[1] { iintAllocationYear });
        }

        /// <summary>
        /// method to post the the allocation amounts from staging table to contribution table
        /// </summary>
        private void PostIAPAllocationIntoContribution()
        {
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            iobjLock = new object();

            int lintPersonAccountID, lintComputationYear;
            lintComputationYear = lintPersonAccountID = 0;
            busPersonAccountRetirementContribution lobjContribution = new busPersonAccountRetirementContribution();
            DateTime ldtTransactionDate = new DateTime();
            //PIR 630
            DataTable ldtIAPCutoffDates = busBase.Select("cdoIapAllocationCutoffDates.GetIAPAllocationCutoffDatesDetail", new object[1] { DateTime.Now.Year - 1 });
            if (ldtIAPCutoffDates != null)
            {
                busIapAllocationCutoffDates lbusIapAllocationCutoffDates = new busIapAllocationCutoffDates { icdoIapAllocationCutoffDates = new cdoIapAllocationCutoffDates() };
                //check if already exists records for this(lintComputationYear) computation year in sgt_iap_allocation_cutoff_dates table, if exists then delete and insert new
                if (ldtIAPCutoffDates.Rows.Count > 0)
                {
                    int lintiapallocationcutoffdatesid = 0;
                    foreach (DataRow dr in ldtIAPCutoffDates.Rows)
                    {
                        lintiapallocationcutoffdatesid = Convert.ToInt32(dr["IAP_ALLOCATION_CUTOFF_DATES_ID"].ToString());
                        if (lbusIapAllocationCutoffDates.FindIapAllocationCutoffDates(lintiapallocationcutoffdatesid))
                        {
                            ldtTransactionDate = lbusIapAllocationCutoffDates.icdoIapAllocationCutoffDates.adj_cutoff_date_to;
                        }
                    }
                }
            }

            if (ldtTransactionDate == DateTime.MinValue)
            {
                ldtTransactionDate = DateTime.Now;
            }


            ParallelOptions p = new ParallelOptions();
            p.MaxDegreeOfParallelism = 1;

            Parallel.ForEach(idtIAPAllocationDetail.AsEnumerable(), p, (ldrIAP, loopstate) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "IAPYearEndPosting-Batch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;

                lock (iobjLock)
                {
                    lintCount++;
                    lintTotalCount++;
                    if (lintCount == 100)
                    {
                        String lstrMsg = lintTotalCount + " : " + " Records Has Been Processed";
                        PostInfoMessage(lstrMsg);
                        lintCount = 0;
                    }
                }

                lobjPassInfo.BeginTransaction();
                try
                {
                    //posting allocation 1 amount for iap
                    if (!Convert.ToBoolean(ldrIAP["allocation1_amount"].IsDBNull()) && Convert.ToDecimal(ldrIAP["allocation1_amount"]) != 0)
                    {
                        lobjContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldrIAP["person_account_id"]),
                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(ldrIAP["computation_year"])),
                                                                                    ldtTransactionDate,
                                                                                    Convert.ToInt32(ldrIAP["computation_year"]),
                                                                                    adecIAPBalanceAmount: Convert.ToDecimal(ldrIAP["allocation1_amount"]),
                                                                                    astrTransactionType: busConstant.RCTransactionTypeYearEndAllocation,
                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation1);
                    }
                    //posting allocation 1 amount for l52
                    if (!Convert.ToBoolean(ldrIAP["l52_allocation1_amount"].IsDBNull()) && Convert.ToDecimal(ldrIAP["l52_allocation1_amount"]) != 0)
                    {
                        lobjContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldrIAP["person_account_id"]),
                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(ldrIAP["computation_year"])),
                                                                                    ldtTransactionDate,
                                                                                    Convert.ToInt32(ldrIAP["computation_year"]),
                                                                                    adec52SplAccountBalance : Convert.ToDecimal(ldrIAP["l52_allocation1_amount"]),
                                                                                    astrTransactionType: busConstant.RCTransactionTypeYearEndAllocation,
                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation1);
                    }
                    //posting allocation 1 amount for l161
                    if (!Convert.ToBoolean(ldrIAP["l161_allocation1_amount"].IsDBNull()) && Convert.ToDecimal(ldrIAP["l161_allocation1_amount"]) != 0)
                    {
                        lobjContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldrIAP["person_account_id"]),
                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(ldrIAP["computation_year"])),
                                                                                    ldtTransactionDate,
                                                                                    Convert.ToInt32(ldrIAP["computation_year"]),
                                                                                    adec161SplAccountBalance: Convert.ToDecimal(ldrIAP["l161_allocation1_amount"]),
                                                                                    astrTransactionType: busConstant.RCTransactionTypeYearEndAllocation,
                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation1);
                    }
                    //posting allocation 2 amount
                    if (!Convert.ToBoolean(ldrIAP["allocation2_amount"].IsDBNull()) && Convert.ToDecimal(ldrIAP["allocation2_amount"]) != 0)
                    {
                        lobjContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldrIAP["person_account_id"]),
                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(ldrIAP["computation_year"])),
                                                                                    ldtTransactionDate,
                                                                                    Convert.ToInt32(ldrIAP["computation_year"]),
                                                                                    adecIAPBalanceAmount: Convert.ToDecimal(ldrIAP["allocation2_amount"]),
                                                                                    astrTransactionType: busConstant.RCTransactionTypeYearEndAllocation,
                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation2);
                    }
                    //posting allocation 2 investment amount
                    if (!Convert.ToBoolean(ldrIAP["allocation2_invst_amount"].IsDBNull()) && Convert.ToDecimal(ldrIAP["allocation2_invst_amount"]) != 0)
                    {
                        lobjContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldrIAP["person_account_id"]),
                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(ldrIAP["computation_year"])),
                                                                                    ldtTransactionDate,
                                                                                    Convert.ToInt32(ldrIAP["computation_year"]),
                                                                                    adecIAPBalanceAmount: Convert.ToDecimal(ldrIAP["allocation2_invst_amount"]),
                                                                                    astrTransactionType: busConstant.RCTransactionTypeYearEndAllocation,
                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation2,
                                                                                    astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
                    }
                    //posting allocation 2 forfeiture amount
                    if (!Convert.ToBoolean(ldrIAP["allocation2_frft_amount"].IsDBNull()) && Convert.ToDecimal(ldrIAP["allocation2_frft_amount"]) != 0)
                    {
                        lobjContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldrIAP["person_account_id"]),
                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(ldrIAP["computation_year"])),
                                                                                    ldtTransactionDate,
                                                                                    Convert.ToInt32(ldrIAP["computation_year"]),
                                                                                    adecIAPBalanceAmount: Convert.ToDecimal(ldrIAP["allocation2_frft_amount"]),
                                                                                    astrTransactionType: busConstant.RCTransactionTypeYearEndAllocation,
                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation2,
                                                                                    astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
                    }
                    //posting allocation 4 amount
                    if (!Convert.ToBoolean(ldrIAP["allocation4_amount"].IsDBNull()) && Convert.ToDecimal(ldrIAP["allocation4_amount"]) != 0)
                    {
                        lobjContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldrIAP["person_account_id"]),
                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(ldrIAP["computation_year"])),
                                                                                    ldtTransactionDate,
                                                                                    Convert.ToInt32(ldrIAP["computation_year"]),
                                                                                    adecIAPBalanceAmount: Convert.ToDecimal(ldrIAP["allocation4_amount"]),
                                                                                    astrTransactionType: busConstant.RCTransactionTypeYearEndAllocation,
                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation4);
                    }
                    //posting allocation 4 investment amount
                    if (!Convert.ToBoolean(ldrIAP["allocation4_invst_amount"].IsDBNull()) && Convert.ToDecimal(ldrIAP["allocation4_invst_amount"]) != 0)
                    {
                        lobjContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldrIAP["person_account_id"]),
                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(ldrIAP["computation_year"])),
                                                                                    ldtTransactionDate,
                                                                                    Convert.ToInt32(ldrIAP["computation_year"]),
                                                                                    adecIAPBalanceAmount: Convert.ToDecimal(ldrIAP["allocation4_invst_amount"]),
                                                                                    astrTransactionType: busConstant.RCTransactionTypeYearEndAllocation,
                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation4,
                                                                                    astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
                    }
                    //posting allocation 4 forfeiture amount
                    if (!Convert.ToBoolean(ldrIAP["allocation4_frft_amount"].IsDBNull()) && Convert.ToDecimal(ldrIAP["allocation4_frft_amount"]) != 0)
                    {
                        lobjContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldrIAP["person_account_id"]),
                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(ldrIAP["computation_year"])),
                                                                                    ldtTransactionDate,
                                                                                    Convert.ToInt32(ldrIAP["computation_year"]),
                                                                                    adecIAPBalanceAmount: Convert.ToDecimal(ldrIAP["allocation4_frft_amount"]),
                                                                                    astrTransactionType: busConstant.RCTransactionTypeYearEndAllocation,
                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation4,
                                                                                    astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
                    }
                    lobjPassInfo.Commit();
                }
                catch (Exception ex)
                {
                    lock (iobjLock)
                    {
                        ExceptionManager.Publish(ex);
                        String lstrMsg = "Error while posting into contribution for PersonAccount ID " + ldrIAP["person_account_id"].ToString() + " : " + ex.ToString();
                        PostErrorMessage(lstrMsg);
                    }
                    lobjPassInfo.Rollback();
                    throw ex;
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
}
