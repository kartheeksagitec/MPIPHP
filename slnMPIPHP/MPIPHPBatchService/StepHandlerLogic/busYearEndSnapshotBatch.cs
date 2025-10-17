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
    public class busYearEndSnapshotBatch : busBatchHandler
    {
        #region Properties
        DataTable ldtbPeople4IAPSnapShot { get; set; }
        DataTable ldtbParticipantInfo4Category { get; set; }
        DataTable ldtbPayoutsAndUnallocableAmount { get; set; }
        DataTable ldtbFullInfoFromEADB { get; set; }

        DataTable ldtbFailedPeople4IAPSnapShot { get; set; }
        int iintTempTable { get; set; }

        //DataTable ldtblManualAdjs { get; set; } //PIR 630

        private object iobjLock = null;
        int lintCount = 0;
        int lintTotalCount = 0;
        decimal idecIAPAllocation2FactorData1;

        //PIR 630 Suresh
        public DateTime idtAdjCutoffToDate { get; set; }
        #endregion

        #region Constructor
        public busYearEndSnapshotBatch(busSystemManagement abusSystemManagement, UpdateProcessLog adlgUpdateProcessLog, utlPassInfo aobjPassInfo, busJobHeader abusJobHeader)
        {
            iobjSystemManagement = abusSystemManagement;
            idlgUpdateProcessLog = adlgUpdateProcessLog;
            iobjPassInfo = aobjPassInfo;
            istrProcessName = "Year End SnapShot Batch";
            ibusJobHeader = abusJobHeader;
        }
        #endregion


        public override void Process()
        {
            //DeletFromAllocationDetail();
            //PIR 630 Suresh
            RetrieveBatchParameters();
            FetchPopulationforSnapShot();


        }

        //PIR 630 Suresh
        private void RetrieveBatchParameters()
        {
            if (ibusJobHeader != null)
            {
                if (ibusJobHeader.iclbJobDetail == null)
                    ibusJobHeader.LoadJobDetail(true);

                foreach (busJobDetail lobjDetail in ibusJobHeader.iclbJobDetail)
                {
                    foreach (busJobParameters lobjParam in lobjDetail.iclbJobParameters)
                    {
                        switch (lobjParam.icdoJobParameters.param_name)
                        {
                            case busConstant.JobParamAdjCutoffToDate:
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty())
                                    idtAdjCutoffToDate = Convert.ToDateTime(lobjParam.icdoJobParameters.param_value);
                                break;
                            case "RunForSpecificParticipants":
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty()
                                    && Convert.ToString(lobjParam.icdoJobParameters.param_value).Trim() == busConstant.FLAG_YES)
                                {
                                    iintTempTable = 1;
                                }
                                else
                                {
                                    iintTempTable = 0;
                                }
                                break;
                        }
                    }
                }
            }
        }


        private void DeletFromAllocationDetail()
        {
            iobjPassInfo.BeginTransaction();

            DBFunction.DBNonQuery("cdoIapAllocationDetail.DeletePreviousYearData", new object[0] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            iobjPassInfo.Commit();
        }


        private void FetchPopulationforSnapShot()
        {
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            iobjLock = new object();

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
            lbusIapAllocationSummary.LoadLatestAllocationSummary();

            int lintComputationYear = lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year + 1;

            //PIR 630 Suresh           
            DataTable ldtIAPCutoffDates = busBase.Select("cdoIapAllocationCutoffDates.GetIAPAllocationCutoffDatesDetail", new object[1] { lintComputationYear });
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
                            lbusIapAllocationCutoffDates.icdoIapAllocationCutoffDates.Delete();
                        }
                    }
                }

                DateTime ldtFirstDayDate = new DateTime();
                DateTime ldtLastDayDate = new DateTime();
                DateTime ldtEACutoffFromDate = new DateTime();
                DateTime ldtEACutoffToDate = new DateTime();
                DateTime ldtAdjCutoffFromDate = new DateTime();

                //get record details
                ldtFirstDayDate = busGlobalFunctions.GetFirstDateOfComputationYear(lintComputationYear);
                ldtLastDayDate = busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear);

                DataTable ldtEACutoffDateFrom = busBase.Select("cdoIapAllocationCutoffDates.GetEACutoffFromDate", new object[1] { lintComputationYear - 1 });
                if (ldtEACutoffDateFrom != null && ldtEACutoffDateFrom.Rows.Count > 0)
                {
                    ldtEACutoffFromDate = Convert.ToDateTime(ldtEACutoffDateFrom.Rows[0][0]);
                }
                DataTable ldtEACutoffDateTo = busBase.Select("cdoIapAllocationCutoffDates.GetEACutoffToDate", new object[1] { lintComputationYear });
                if (ldtEACutoffDateTo != null && ldtEACutoffDateTo.Rows.Count > 0)
                {
                    ldtEACutoffToDate = Convert.ToDateTime(ldtEACutoffDateTo.Rows[0][0]);
                }
                DataTable ldtAdjCutoffDateFrom = busBase.Select("cdoIapAllocationCutoffDates.GetAdjCutoffFromDate", new object[1] { lintComputationYear - 1 });
                if (ldtAdjCutoffDateFrom != null && ldtAdjCutoffDateFrom.Rows.Count > 0)
                {
                    ldtAdjCutoffFromDate = Convert.ToDateTime(ldtAdjCutoffDateFrom.Rows[0][0]);
                }
                //assign field values
                lbusIapAllocationCutoffDates.icdoIapAllocationCutoffDates.computational_year = lintComputationYear;
                lbusIapAllocationCutoffDates.icdoIapAllocationCutoffDates.first_day = ldtFirstDayDate;
                lbusIapAllocationCutoffDates.icdoIapAllocationCutoffDates.last_day = ldtLastDayDate;
                lbusIapAllocationCutoffDates.icdoIapAllocationCutoffDates.ea_cutoff_date_from = ldtEACutoffFromDate;
                lbusIapAllocationCutoffDates.icdoIapAllocationCutoffDates.ea_cutoff_date_to = ldtEACutoffToDate;
                lbusIapAllocationCutoffDates.icdoIapAllocationCutoffDates.adj_cutoff_date_from = ldtAdjCutoffFromDate;
                if (Convert.ToString(idtAdjCutoffToDate).IsNotNullOrEmpty() && idtAdjCutoffToDate != DateTime.MinValue)
                {
                    lbusIapAllocationCutoffDates.icdoIapAllocationCutoffDates.adj_cutoff_date_to = idtAdjCutoffToDate;
                }
                else
                {
                    lbusIapAllocationCutoffDates.icdoIapAllocationCutoffDates.adj_cutoff_date_to = DateTime.Now;
                }
                //insert new records
                lbusIapAllocationCutoffDates.icdoIapAllocationCutoffDates.Insert();
            }

            #region GetDataWeNeedFromEADBin-1 SHOT
            SqlParameter[] parameters = new SqlParameter[1];

            SqlParameter param1 = new SqlParameter("@COMPUTATIONYEAR", DbType.Int32);

            param1.Value = lintComputationYear;
            parameters[0] = param1;

            DataTable ldtbInfoFromEADBintoOPUS = null;
            if (iintTempTable == 0)
                ldtbInfoFromEADBintoOPUS = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPSnapShotInfo", astrLegacyDBConnetion, null, parameters);

            utlLegacyDBConnetion = null;

            //DataTable ldtbInfoFromEADB = busBase.Select("cdoIapAllocationDetail.GetAllWorkHistory4IAPSnapShot", new object[0]);

            //PIR 630
            //ldtbFullInfoFromEADB = busBase.Select("cdoIapAllocationDetail.GetAllFullWorkHistory4IAPSnapShot", new object[0]);

            //PIR 630
            busBase.Select("cdoIapAllocationDetail.HoursForLastTwoYear", new object[1] { lintComputationYear });

            #endregion
            ldtbFailedPeople4IAPSnapShot = new DataTable();
            
            ldtbPeople4IAPSnapShot = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPSnapShotForPersonAccount", new object[1] { iintTempTable });
            ldtbParticipantInfo4Category = busBase.Select("cdoPersonAccountRetirementContribution.GetParticipantInfo4CategoryDetermination", new object[1] { iintTempTable });
            DateTime ldtLastBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate(busConstant.IAP_PLAN_ID);
            DateTime ldtNextBenefitPaymentDate = busGlobalFunctions.GetLastDayOfWeek(ldtLastBenefitPaymentDate.AddDays(1));


            //Ticket - 60382
            if (iintTempTable == 1)
                DBFunction.DBExecuteScalar("cdoIapAllocationDetail.DeleteSnapshotEntriesForSpecificParticipants", new object[1] { lintComputationYear }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);


            //PIR 630
            //ldtblManualAdjs = busBase.Select("cdoIapAllocationDetail.GetManualAdjsAfterCutoff", new object[0]);

            if (ldtbPeople4IAPSnapShot != null && ldtbPeople4IAPSnapShot.Rows.Count > 0)
            {
                ldtbFailedPeople4IAPSnapShot = ldtbFailedPeople4IAPSnapShot.Clone();

                ldtbPayoutsAndUnallocableAmount = busBase.Select("cdoPaymentHistoryHeader.GetPayoutandUnallocableAmt4YeadEndIapAlloc", new object[1] { lintComputationYear });

                ParallelOptions p = new ParallelOptions();

                idecIAPAllocation2FactorData1 = Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPAllocation2Factor));

                //Ticket - 60382
                p.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 2;

                PostInfoMessage("Total records : " + ldtbPeople4IAPSnapShot.Rows.Count);
                PostInfoMessage("MaxDegreeOfParallelism : " + p.MaxDegreeOfParallelism);

                Parallel.ForEach(ldtbPeople4IAPSnapShot.AsEnumerable(), p, (ldtrPersonYearlySnapshot, loopstate) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "SnapShot-Batch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    DataRow[] larrPayoutsAndUnallocable = ldtbPayoutsAndUnallocableAmount.Select("FUND_TYPE = '" + ldtrPersonYearlySnapshot["FUND_TYPE"].ToString() + "' AND PERSON_ACCOUNT_ID=" + ldtrPersonYearlySnapshot["PERSON_ACCOUNT_ID"].ToString());
                    //FilterTable(utlDataType.Numeric, "PERSON_ACCOUNT_ID", ldtrPersonYearlySnapshot["PERSON_ACCOUNT_ID"].ToString());

                    //PIR 630
                    bool lblnAdjpascutoff = false;
                    ////DataRow[] larrManualAdjs = ldtblManualAdjs.FilterTable(utlDataType.String, "SSN", ldtrPersonYearlySnapshot["SSN"].ToString());
                    // if (!larrManualAdjs.IsNullOrEmpty() && larrManualAdjs.Count() > 0)
                    // {
                    //     DataRow ldrManualAdjs = larrManualAdjs.FirstOrDefault();
                    //     if (ldtrPersonYearlySnapshot["FUND_TYPE"].ToString() == "IAP" &&
                    //         Convert.ToString(ldrManualAdjs[enmPersonAccountRetirementContribution.iap_balance_amount.ToString().ToUpper()]).IsNotNullOrEmpty() &&
                    //         Convert.ToDecimal(ldrManualAdjs[enmPersonAccountRetirementContribution.iap_balance_amount.ToString().ToUpper()]) != 0)
                    //     {
                    //         lblnAdjpascutoff = true;
                    //     }
                    //     else if (ldtrPersonYearlySnapshot["FUND_TYPE"].ToString() == "L052" &&
                    //         Convert.ToString(ldrManualAdjs[enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString().ToUpper()]).IsNotNullOrEmpty() &&
                    //         Convert.ToDecimal(ldrManualAdjs[enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString().ToUpper()]) != 0)
                    //     {
                    //         lblnAdjpascutoff = true;
                    //     }
                    //     else if (ldtrPersonYearlySnapshot["FUND_TYPE"].ToString() == "L161" &&
                    //        Convert.ToString(ldrManualAdjs[enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString().ToUpper()]).IsNotNullOrEmpty() &&
                    //        Convert.ToDecimal(ldrManualAdjs[enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString().ToUpper()]) != 0)
                    //     {
                    //         lblnAdjpascutoff = true;
                    //     }

                    // }

                    //PIR 885
                    DataTable ldtbInfoFromEADB = busBase.Select("cdoIapAllocationDetail.GetAllWorkHistory4IAPSnapShot", new object[1] { ldtrPersonYearlySnapshot["SSN"].ToString() });

                    if (ldtbInfoFromEADB.Rows.Count > 0)
                    {

                        DataRow[] larrRows = ldtbInfoFromEADB.Select();

                        if (!larrRows.IsNullOrEmpty())
                        {
                            GenerateSnapshot(ldtrPersonYearlySnapshot, larrRows, larrPayoutsAndUnallocable, lobjPassInfo, lblnAdjpascutoff);
                        }
                        else
                            GenerateSnapshot(ldtrPersonYearlySnapshot, null, larrPayoutsAndUnallocable, lobjPassInfo, lblnAdjpascutoff);
                    }
                    else
                        GenerateSnapshot(ldtrPersonYearlySnapshot, null, larrPayoutsAndUnallocable, lobjPassInfo, lblnAdjpascutoff);

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;

                });

                //Ticket - 60382
                if (ldtbFailedPeople4IAPSnapShot != null && ldtbFailedPeople4IAPSnapShot.Rows.Count > 0)
                {
                    String lstrMsg = "Processing Failed Records";
                    PostInfoMessage(lstrMsg);

                    p.MaxDegreeOfParallelism = 1;

                    Parallel.ForEach(ldtbFailedPeople4IAPSnapShot.AsEnumerable(), p, (ldtrPersonYearlySnapshot, loopstate) =>
                    {
                        utlPassInfo lobjPassInfo = new utlPassInfo();
                        lobjPassInfo.idictParams = ldictParams;
                        lobjPassInfo.idictParams["ID"] = "SnapShot-Batch";
                        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                        utlPassInfo.iobjPassInfo = lobjPassInfo;

                        DataRow[] larrPayoutsAndUnallocable = ldtbPayoutsAndUnallocableAmount.Select("FUND_TYPE = '" + ldtrPersonYearlySnapshot["FUND_TYPE"].ToString() + "' AND PERSON_ACCOUNT_ID=" + ldtrPersonYearlySnapshot["PERSON_ACCOUNT_ID"].ToString());

                        //PIR 630
                        bool lblnAdjpascutoff = false;

                        DataTable ldtbInfoFromEADB = busBase.Select("cdoIapAllocationDetail.GetAllWorkHistory4IAPSnapShot", new object[1] { ldtrPersonYearlySnapshot["SSN"].ToString() });

                        if (ldtbInfoFromEADB.Rows.Count > 0)
                        {

                            DataRow[] larrRows = ldtbInfoFromEADB.Select();

                            if (!larrRows.IsNullOrEmpty())
                            {
                                GenerateSnapshot(ldtrPersonYearlySnapshot, larrRows, larrPayoutsAndUnallocable, lobjPassInfo, lblnAdjpascutoff);
                            }
                            else
                                GenerateSnapshot(ldtrPersonYearlySnapshot, null, larrPayoutsAndUnallocable, lobjPassInfo, lblnAdjpascutoff);
                        }
                        else
                            GenerateSnapshot(ldtrPersonYearlySnapshot, null, larrPayoutsAndUnallocable, lobjPassInfo, lblnAdjpascutoff);

                        if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                        {
                            lobjPassInfo.iconFramework.Close();
                        }

                        lobjPassInfo.iconFramework.Dispose();
                        lobjPassInfo.iconFramework = null;

                    });
                }

                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;

                //Ticket - 60382
                PostInfoMessage("Creating copy of Snapshot Data in table SGT_IAP_ALLOCATION_DETAIL_SNAPSHOT_BACKUP");
                DBFunction.DBExecuteScalar("cdoIapAllocationDetail.CreateCopyForSnapshotTable", new object[1] { lintComputationYear }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }

        }

        //PIR 630
        private void GetIAPHoursUsingDeterminationdate(string astrSSN, DateTime adtDeterminationDate, DataTable adtbFullInfoFromEADB, ref cdoIapAllocationDetail acdoIapAllocationDetail)
        {
            //DataRow[] ldtWorkHistory = adtbFullInfoFromEADB.FilterTable(utlDataType.String, "SSN", astrSSN);
            //PIR 885
            DataRow[] ldtWorkHistory = null;
            DataTable ldtbFullWorkHistoryInfoFromEADB = busBase.Select("cdoIapAllocationDetail.GetAllFullWorkHistory4IAPSnapShot", new object[1] { astrSSN });
            if (ldtbFullWorkHistoryInfoFromEADB != null && ldtbFullWorkHistoryInfoFromEADB.Rows.Count > 0)
                ldtWorkHistory = ldtbFullWorkHistoryInfoFromEADB.Select();

            if (ldtWorkHistory != null && ldtWorkHistory.Length > 0)
            {
                Decimal ldecBeforeIAPHours = 0.0M;
                Decimal ldecAfterIAPHours = 0.0M;
                Decimal ldecBeforeIAPHoursA2 = 0.0M;
                Decimal ldecAfterIAPHoursA2 = 0.0M;
                foreach (DataRow dr in ldtWorkHistory.CopyToDataTable().Rows)
                {
                    DateTime ldtToDate = new DateTime();
                    Decimal ldecIAPHours = decimal.Zero;
                    Decimal ldecIAPHoursA2 = decimal.Zero;

                    if (Convert.ToString(dr["ToDate"]).IsNotNullOrEmpty())
                        ldtToDate = Convert.ToDateTime(dr["ToDate"]);

                    if (Convert.ToString(dr["IAPHours"]).IsNotNullOrEmpty())
                        ldecIAPHours = Convert.ToDecimal(dr["IAPHours"]);

                    if (Convert.ToString(dr["IAPHoursA2"]).IsNotNullOrEmpty())
                        ldecIAPHoursA2 = Convert.ToDecimal(dr["IAPHoursA2"]);

                    if (adtDeterminationDate != DateTime.MinValue)
                    {
                        if (ldtToDate != DateTime.MinValue && adtDeterminationDate >= ldtToDate)
                        {
                            ldecBeforeIAPHours += ldecIAPHours;
                            ldecBeforeIAPHoursA2 += ldecIAPHoursA2;
                        }
                        else
                        {
                            ldecAfterIAPHours += ldecIAPHours;
                            ldecAfterIAPHoursA2 += ldecIAPHoursA2;
                        }
                    }
                    else
                    {
                        ldecBeforeIAPHours += ldecIAPHours;
                        ldecBeforeIAPHoursA2 += ldecIAPHoursA2;
                    }
                }


                acdoIapAllocationDetail.hrs_before_det_date = ldecBeforeIAPHours;
                acdoIapAllocationDetail.hrs_after_det_date = ldecAfterIAPHours;


                acdoIapAllocationDetail.hrs_a2_before_det_date = ldecBeforeIAPHoursA2;
                acdoIapAllocationDetail.hrs_a2_after_det_date = ldecAfterIAPHoursA2;
            }
        }

        private Collection<utlWhereClause> BuildWhereClause(string astrQueryId, string astrSSNList)
        {
            Collection<utlWhereClause> lcolWhereClause = new Collection<utlWhereClause>();
            busMPIPHPBase lbusMPIPHPBase = new busMPIPHPBase();

            lcolWhereClause.Add(lbusMPIPHPBase.GetWhereClause(astrSSNList, "", "P.SSN", "string", "in", " ", astrQueryId));
            return lcolWhereClause;
        }


        private string DetermineCategory(DataRow adrContribution, DataTable ldtbParticipantInfo4Category, DataRow ldtRowRemployed, decimal ldecTotalIAPHours, decimal ldecAge,
            string astrSSN, DataTable adtbFullInfoFromEADB, ref cdoIapAllocationDetail acdoIAPAllocationDetail)
        {
            decimal ldecHoursForCurrYr = 0M;
            decimal ldecHoursForPrevYr = 0M;

            bool lblnAPSpecialAccount = false;
            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };

            DataTable ldtbHoursForLastTwoYear = busBase.Select("cdoIapAllocationDetail.GetHoursForLastTwoYearBySSN", new object[1] { astrSSN });

            DateTime ldtEndDate = busGlobalFunctions.GetLastDateOfComputationYear(iobjSystemManagement.icdoSystemManagement.batch_date.AddYears(-1).Year);

            if (ldtbHoursForLastTwoYear != null && ldtbHoursForLastTwoYear.Rows.Count > 0)
            {
                ldecHoursForCurrYr = Convert.ToDecimal(ldtbHoursForLastTwoYear.Rows[0]["CURRIAPHOURS"]);
                ldecHoursForPrevYr = Convert.ToDecimal(ldtbHoursForLastTwoYear.Rows[0]["PREVIAPHOURS"]);
            }


            #region Category Determination Code -- Abhishek
            string ltempIapAllocationCategory = string.Empty;
            int lintTotalCountofPayeeAccounts4Person = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                                        where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                                            && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                                        select row.Field<string>("SSN")).Count();

            if (ldtbParticipantInfo4Category.IsNotNull() && ldtbParticipantInfo4Category.Rows.Count > 0)
            {
                //In-Active Zero Balance - L052 // Need to check here IF we are not using anything from ldtbParticipantInfo4Category can we check directly from adtrContribution (why bother bringing them)
                if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                     where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                           && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED)
                           && (adrContribution["L52_SPECIAL_ACCOUNT_AMOUNT"].IsNull() || Convert.ToDecimal(adrContribution["L52_SPECIAL_ACCOUNT_AMOUNT"]) == Decimal.Zero)
                           && adrContribution["FUND_TYPE"].ToString() == busConstant.FundTypeLocal52SpecialAccount
                           && row.Field<string>("FUND_TYPE").ToString() == busConstant.FundTypeLocal52SpecialAccount
                     select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryInActiveZeroBalance;
                }

                //In-Active Zero Balance - L161 // Need to check here IF we are not using anything from ldtbParticipantInfo4Category can we check directly from adtrContribution (why bother bringing them)
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED)
                                && (adrContribution["L161_SPECIAL_ACCOUNT_AMOUNT"].IsNull() || Convert.ToDecimal(adrContribution["L161_SPECIAL_ACCOUNT_AMOUNT"]) == Decimal.Zero)
                                && adrContribution["FUND_TYPE"].ToString() == busConstant.FundTypeLocal161SpecialAccount
                                && row.Field<string>("FUND_TYPE").ToString() == busConstant.FundTypeLocal161SpecialAccount
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryInActiveZeroBalance;
                }

                 //Active - L052 
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE
                                || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED)
                                && ((row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull()) || (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 &&
                                    (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL &&
                                    (row.Field<string>("ACCOUNT_RELATION_VALUE").IsNull() || row.Field<string>("ACCOUNT_RELATION_VALUE") != busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE)) && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE").Year > Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                                || (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 &&
                                (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO))
                                ||
                                (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 &&
                                    (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL &&
                                    (row.Field<string>("ACCOUNT_RELATION_VALUE") == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE)))
                                )
                              //&& (row.Field<DateTime?>("DATE_OF_DEATH").IsNull() || (row.Field<DateTime?>("DATE_OF_DEATH") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                                && adrContribution["L52_SPECIAL_ACCOUNT_AMOUNT"].IsNotNull()
                                && Convert.ToDecimal(adrContribution["L52_SPECIAL_ACCOUNT_AMOUNT"]) > Decimal.Zero
                                && adrContribution["FUND_TYPE"].ToString() == busConstant.FundTypeLocal52SpecialAccount
                                && row.Field<string>("FUND_TYPE").ToString() == busConstant.FundTypeLocal52SpecialAccount
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryActive;
                }

                //Active - L161
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE ||
                                row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED)
                                 && ((row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull()) || (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 &&
                                    (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL &&
                                    (row.Field<string>("ACCOUNT_RELATION_VALUE").IsNull() || row.Field<string>("ACCOUNT_RELATION_VALUE") != busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE)) && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE").Year > Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                                || (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 &&
                                (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO))
                                ||
                                (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 &&
                                    (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL &&
                                    (row.Field<string>("ACCOUNT_RELATION_VALUE") == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE)))
                                )
                              //&& (row.Field<DateTime?>("DATE_OF_DEATH").IsNull() || (row.Field<DateTime?>("DATE_OF_DEATH") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                                && adrContribution["L161_SPECIAL_ACCOUNT_AMOUNT"].IsNotNull()
                                && Convert.ToDecimal(adrContribution["L161_SPECIAL_ACCOUNT_AMOUNT"]) > Decimal.Zero
                                && adrContribution["FUND_TYPE"].ToString() == busConstant.FundTypeLocal161SpecialAccount
                                && row.Field<string>("FUND_TYPE").ToString() == busConstant.FundTypeLocal161SpecialAccount
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryActive;
                }

                //Early Special Account WithDrawal - L052 - as Retiree 
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (
                                   (row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() && row.Field<DateTime?>("RETIREMENT_DATE").IsNull())
                                    ||
                                   (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                   )
                                && adrContribution["FUND_TYPE"].ToString() == busConstant.FundTypeLocal52SpecialAccount
                                && row.Field<string>("FUND_TYPE").ToString() == busConstant.FundTypeLocal52SpecialAccount
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryRetiree;
                    //PIR 630
                    var ldtTempDate = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                       where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                             && (
                                                (row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() && row.Field<DateTime?>("RETIREMENT_DATE").IsNull())
                                                 ||
                                                (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                                )
                                             && adrContribution["FUND_TYPE"].ToString() == busConstant.FundTypeLocal52SpecialAccount
                                             && row.Field<string>("FUND_TYPE").ToString() == busConstant.FundTypeLocal52SpecialAccount
                                       select new { istrRetirementDate = row.Field<DateTime?>("RETIREMENT_DATE") }).Distinct();
                    //PIR 630
                    if (ldtTempDate.IsNotNull() && ldtTempDate.Count() > 0)
                    {
                        foreach (var dr in ldtTempDate)
                        {
                            if (dr.istrRetirementDate.IsNotNull())
                            {
                                acdoIAPAllocationDetail.withdrawal_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                                acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                            }
                        }
                    }
                }


                //Early Special Account WithDrawal - L161 - as Retiree 
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (
                                   (row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() && row.Field<DateTime?>("RETIREMENT_DATE").IsNull())
                                    ||
                                   (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                   )
                                && adrContribution["FUND_TYPE"].ToString() == busConstant.FundTypeLocal161SpecialAccount
                                && row.Field<string>("FUND_TYPE").ToString() == busConstant.FundTypeLocal161SpecialAccount
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryRetiree;
                    //PIR 630
                    var ldtTempDate = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                       where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                             && (
                                                (row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() && row.Field<DateTime?>("RETIREMENT_DATE").IsNull())
                                                 ||
                                                (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                                )
                                             && adrContribution["FUND_TYPE"].ToString() == busConstant.FundTypeLocal161SpecialAccount
                                             && row.Field<string>("FUND_TYPE").ToString() == busConstant.FundTypeLocal161SpecialAccount
                                       //select row.Field<DateTime?>("RETIREMENT_DATE"));
                                       select new { istrRetirementDate = row.Field<DateTime?>("RETIREMENT_DATE") }).Distinct();
                    //PIR 630
                    if (ldtTempDate.IsNotNull() && ldtTempDate.Count() > 0)
                    {
                        foreach (var dr in ldtTempDate)
                        {
                            if (dr.istrRetirementDate.IsNotNull())
                            {
                                acdoIAPAllocationDetail.withdrawal_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                                acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                            }
                        }
                    }
                }

                //NEGATIVE BALANCES - L025
                else if (adrContribution["L52_SPECIAL_ACCOUNT_AMOUNT"].IsDBNull() != DBNull.Value && Convert.ToDecimal(adrContribution["L52_SPECIAL_ACCOUNT_AMOUNT"]) < Decimal.Zero && adrContribution["FUND_TYPE"].ToString() == busConstant.FundTypeLocal52SpecialAccount)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryNegBalance;
                }
                //NEGATIVE BALANCES - L161
                else if (adrContribution["L161_SPECIAL_ACCOUNT_AMOUNT"].IsDBNull() != DBNull.Value && Convert.ToDecimal(adrContribution["L161_SPECIAL_ACCOUNT_AMOUNT"]) < Decimal.Zero && adrContribution["FUND_TYPE"].ToString() == busConstant.FundTypeLocal161SpecialAccount)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryNegBalance;
                }

                //Reemployed Over 65 
                //HOurs can be checked while giving allocation
                // PIR 630
                else if (ldtRowRemployed.IsNotNull() && ldecAge > 65 && ldecTotalIAPHours > Decimal.Zero) //PIR 885
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryReempOver65;

                    if (Convert.ToString(ldtRowRemployed["RETIREMENT_DATE"]).IsNotNullOrEmpty())
                    {
                        acdoIAPAllocationDetail.retire_date = Convert.ToDateTime(ldtRowRemployed["RETIREMENT_DATE"]);
                        acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(ldtRowRemployed["RETIREMENT_DATE"]);
                        GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                    }
                }

                //Reemployed Under 65
                //HOurs can be checked while giving allocation
                // PIR 630
                //(ldtRowRemployed.IsNotNull() && ldecAge <= 65 && ldecTotalIAPHours > Decimal.Zero) //PIR 885
                //Ticket##128387 
               
               
                else if (((from row in ldtbParticipantInfo4Category.AsEnumerable()
                                        where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                              //&& (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED)
                                              // Commented because there PersonAccount status might be ACTIVE coz payments have NOT yet happened. Also needed to consider for OLD QDRO and RTMT in allocation Year
                                              && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                              && busGlobalFunctions.CalculatePersonAge(Convert.ToDateTime(row.Field<DateTime?>("DOB")), ldtEndDate) <= 65
                                              && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                              && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DISABILITY || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL))
                                              && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime?>("RETIREMENT_DATE") < busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                                        //&& (Convert.ToDateTime(row.Field<DateTime?>("RETIREMENT_DATE"))).Year == Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])
                                        //Categorizing here is still ok Since if he doesnt have any balances later on in CODE we are NOT inserting
                                        select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)) //PIR 885
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryReempUnder65;

                    if (ldtRowRemployed.IsNotNull() &&  Convert.ToString(ldtRowRemployed["RETIREMENT_DATE"]).IsNotNullOrEmpty())
                    {
                        acdoIAPAllocationDetail.retire_date = Convert.ToDateTime(ldtRowRemployed["RETIREMENT_DATE"]);
                        acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(ldtRowRemployed["RETIREMENT_DATE"]);
                        GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                    }
                }


                //In-Active Zero Balance - IAP // Need to check here IF we are not using anything from ldtbParticipantInfo4Category can we check directly from adtrContribution (why bother bringing them)
                //PIR 630
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE
                                || (!row.Field<DateTime?>("DATE_OF_DEATH").IsNull() && row.Field<DateTime?>("DATE_OF_DEATH") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                                && (adrContribution["SYSTEM_BEGINNING_BALANCE"].IsNull() || Convert.ToDecimal(adrContribution["SYSTEM_BEGINNING_BALANCE"]) == Decimal.Zero)//PIR 630
                                && ldecTotalIAPHours <= 0 // Total Hours for Allocation Year is ZERO or less than Zero -- IAP Hours //PIR 886 New 
                                && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                && row.Field<string>("FUND_TYPE") == busConstant.IAP
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryInActiveZeroBalance;

                    acdoIAPAllocationDetail.retire_date = DateTime.MinValue;
                    acdoIAPAllocationDetail.determination_date = DateTime.MinValue;
                    GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                }

                //Vested Break-In-Service 
                //PIR 630
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE || (ldecHoursForCurrYr < 200 && ldecHoursForPrevYr < 200)) //PIR 630
                                && (row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() ||
                                   (
                                     (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                     ||
                                     (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO) && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE").Year < Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                                   )
                                )
                              //&& row.Field<DateTime?>("DATE_OF_DEATH").IsNull() //PIR 630
                                && (row.Field<DateTime?>("DATE_OF_DEATH").IsNull() || (row.Field<DateTime?>("DATE_OF_DEATH") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                                && (!row.Field<DateTime?>("VESTED_DATE").IsNull() && row.Field<DateTime?>("VESTED_DATE") != DateTime.MinValue)
                                && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                && adrContribution["SYSTEM_BEGINNING_BALANCE"].IsNotNull() && Convert.ToDecimal(adrContribution["SYSTEM_BEGINNING_BALANCE"]) > Decimal.Zero
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryVstBIS;

                    acdoIAPAllocationDetail.retire_date = DateTime.MinValue;
                    acdoIAPAllocationDetail.determination_date = DateTime.MinValue;
                    GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                }

                //Non-Vested BreakInService
                //PIR 630
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE || (ldecHoursForCurrYr < 200 && ldecHoursForPrevYr < 200)) //PIR 630
                                && (row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() ||
                                    (
                                     (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                     ||
                                     (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 &&
                                     (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO) && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE").Year < Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                                    )
                                )
                              //&& row.Field<DateTime?>("DATE_OF_DEATH").IsNull()//PIR 630
                                && (row.Field<DateTime?>("DATE_OF_DEATH").IsNull() || (row.Field<DateTime?>("DATE_OF_DEATH") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                                && (row.Field<DateTime?>("VESTED_DATE").IsNull())
                                && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                && adrContribution["SYSTEM_BEGINNING_BALANCE"].IsNotNull() && Convert.ToDecimal(adrContribution["SYSTEM_BEGINNING_BALANCE"]) > Decimal.Zero
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryNonVstdBIS;

                    acdoIAPAllocationDetail.retire_date = DateTime.MinValue;
                    acdoIAPAllocationDetail.determination_date = DateTime.MinValue;
                    GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                }

                //Active Participant
                //TODO Participant got paid in first quarter of the next year, we need to give full allocations for allocation year
                //PIR 630
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED)
                                && ((row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() || (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                                     || ((row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull()) || (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 &&
                                     (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO) && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE").Year < Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                   )
                                && (row.Field<DateTime?>("DATE_OF_DEATH").IsNull() || (row.Field<DateTime?>("DATE_OF_DEATH") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                                && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                && adrContribution["SYSTEM_BEGINNING_BALANCE"].IsNotNull() && Convert.ToDecimal(adrContribution["SYSTEM_BEGINNING_BALANCE"]) > Decimal.Zero
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryActive;

                    acdoIAPAllocationDetail.retire_date = DateTime.MinValue;
                    acdoIAPAllocationDetail.determination_date = DateTime.MinValue;
                    GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                }

                //New Participant
                //PIR 630
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE
                                || ((row.Field<DateTime?>("DATE_OF_DEATH") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))))//PIR 630
                                && (row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() ||
                                   ((row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                    ||
                                    (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) &&
                                    (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO) && row.Field<DateTime>("RETIREMENT_DATE").Year < (Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                   )
                                   )
                                && (row.Field<DateTime?>("DATE_OF_DEATH").IsNull() || (row.Field<DateTime?>("DATE_OF_DEATH") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                                && ((adrContribution["SYSTEM_BEGINNING_BALANCE"].IsNull() || Convert.ToDecimal(adrContribution["SYSTEM_BEGINNING_BALANCE"]) <= Decimal.Zero) && ldecTotalIAPHours > 0)
                                && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryNewParticipants;

                    acdoIAPAllocationDetail.retire_date = DateTime.MinValue;
                    acdoIAPAllocationDetail.determination_date = DateTime.MinValue;
                    GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                }

                 //QDRO Active
                //TODO need to check whether any payee account for alternate payee
                //PIR 630
                else if (((from row in ldtbParticipantInfo4Category.AsEnumerable()
                           where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                 && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED)
                                 && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                 && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                 && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                           select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                        ||

                    ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                      where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                            && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED)
                            && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                            && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                            && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 &&
                            (
                            ((row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DISABILITY) && !row.Field<DateTime?>("RETIREMENT_DATE").IsNull() && row.Field<DateTime>("RETIREMENT_DATE") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                            ||
                            (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                            ))
                      select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0

                    &&

                    ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                      where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                            && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED)
                            && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                            && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                            && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                      select row.Field<string>("SSN")).Any()

                        ))
                    )
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryQDROActive;
                    //PIR 630
                    var ldtTemp = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                   where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                         && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                         && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                         && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO)
                                   //select row.Field<DateTime?>("RETIREMENT_DATE"));
                                   select new { istrRetirementDate = row.Field<DateTime?>("RETIREMENT_DATE") }).Distinct();
                    //PIR 630
                    if (ldtTemp.IsNotNull() && ldtTemp.Count() > 0)
                    {
                        foreach (var dr in ldtTemp)
                        {
                            if (dr.istrRetirementDate.IsNotNull())
                            {
                                acdoIAPAllocationDetail.qdro_commence_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                                acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                            }
                            GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                            break;
                        }
                    }
                }

                //Early IAP WithDrawal 
                //PIR 630
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                    && (((row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE
                                            || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE
                                            || ((row.Field<DateTime?>("DATE_OF_DEATH") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))))//PIR 630
                                            && row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL
                                            && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull())
                                            && row.Field<DateTime>("RETIREMENT_DATE").Year == (Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                    || (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED && row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryEarlyWithdrawal;

                    var ldtTemp = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                   where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                         && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                         && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                             && (((row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE
                                                     || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)
                                                     && row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL
                                                     && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull())
                                                     && row.Field<DateTime>("RETIREMENT_DATE").Year == (Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                             || (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED && row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                                   //select row.Field<DateTime?>("RETIREMENT_DATE"));
                                   select new { istrRetirementDate = row.Field<DateTime?>("RETIREMENT_DATE") }).Distinct();
                    //PIR 630
                    if (ldtTemp.IsNotNull() && ldtTemp.Count() > 0)
                    {
                        foreach (var dr in ldtTemp)
                        {
                            if (dr.istrRetirementDate.IsNotNull())
                            {
                                acdoIAPAllocationDetail.withdrawal_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                                acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                            }
                            GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                            break;
                        }
                    }
                }

                //Min Distribution New  -- Work on this 
                //TODO get the hours after the MD date for allocation
                //PIR 630
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (row.Field<DateTime?>("DATE_OF_DEATH").IsNull() || (row.Field<DateTime?>("DATE_OF_DEATH") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                              //&& (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)
                                && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT
                                && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                && row.Field<string>("RETIREMENT_TYPE") == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION
                                && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull() && Convert.ToDateTime(row.Field<DateTime?>("RETIREMENT_DATE")).Year == Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                          select row.Field<string>("SSN")).Count() > 0
                            &&

                        (from row in ldtbParticipantInfo4Category.AsEnumerable()
                         where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                             //&& (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)
                               && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT
                               && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                               && (Convert.ToString(row.Field<string>("RETIREMENT_TYPE")).IsNullOrEmpty() || row.Field<string>("RETIREMENT_TYPE") != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                               && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                               && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull() && Convert.ToDateTime(row.Field<DateTime?>("RETIREMENT_DATE")).Year <= Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                         select row.Field<string>("SSN")).Count() == 0
                            )
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryMDNew;


                    var ldtTemp = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                   where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                       //&& (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)
                                         && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT
                                         && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                         && row.Field<string>("RETIREMENT_TYPE") == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION
                                         && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                         && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull() && Convert.ToDateTime(row.Field<DateTime?>("RETIREMENT_DATE")).Year == Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                   //select row.Field<DateTime?>("RETIREMENT_DATE"));
                                   select new { istrRetirementDate = row.Field<DateTime?>("RETIREMENT_DATE") }).Distinct();
                    //PIR 630
                    if (ldtTemp.IsNotNull() && ldtTemp.Count() > 0)
                    {
                        foreach (var dr in ldtTemp)
                        {
                            if (dr.istrRetirementDate.IsNotNull())
                            {
                                acdoIAPAllocationDetail.md_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                                acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                            }
                            GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                            break;
                        }
                    }
                }

                //Min Distribution Active -- Work on this
                //PIR 630
                else if ((((from row in ldtbParticipantInfo4Category.AsEnumerable()
                            where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                  && (row.Field<DateTime?>("DATE_OF_DEATH").IsNull() || (row.Field<DateTime?>("DATE_OF_DEATH") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                                //&& (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)
                                  && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT
                                  && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                  && row.Field<string>("RETIREMENT_TYPE") == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION
                                  && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                  && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]) - 1).AddDays(1) > row.Field<DateTime>("RETIREMENT_DATE"))
                            select row.Field<string>("SSN")).Count() > 0)
                            &&
                            ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                              where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                  //&& (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)
                                    && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT
                                    && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                    && (Convert.ToString(row.Field<string>("RETIREMENT_TYPE")).IsNullOrEmpty() || row.Field<string>("RETIREMENT_TYPE") != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                                    && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                    && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull() && Convert.ToDateTime(row.Field<DateTime?>("RETIREMENT_DATE")).Year <= Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                              select row.Field<string>("SSN")).Count() == 0)
                            )
                    ||
                    ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                      where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                      && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED)
                      && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT
                       && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                       && row.Field<string>("RETIREMENT_TYPE") == busConstant.RETIREMENT_TYPE_LATE
                       && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                       && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])) < row.Field<DateTime>("RETIREMENT_DATE"))
                      select row.Field<string>("SSN")).Count() > 0
                        &&
                        (from row in ldtbParticipantInfo4Category.AsEnumerable()
                         where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                               && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT
                               && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                               && row.Field<string>("RETIREMENT_TYPE") == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION
                               && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                               && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]) - 1).AddDays(1) > row.Field<DateTime>("RETIREMENT_DATE"))
                         select row.Field<string>("SSN")).Any())
                    )
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryMDActiveReeval;

                    var ldtTemp = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                   where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                         && row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT
                                         && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                         && row.Field<string>("RETIREMENT_TYPE") == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION
                                         && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                         && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0)
                                   //select row.Field<DateTime?>("RETIREMENT_DATE"));
                                   select new { istrRetirementDate = row.Field<DateTime?>("RETIREMENT_DATE") }).Distinct();
                    //PIR 630
                    if (ldtTemp.IsNotNull() && ldtTemp.Count() > 0)
                    {
                        foreach (var dr in ldtTemp)
                        {
                            if (dr.istrRetirementDate.IsNotNull())
                            {
                                acdoIAPAllocationDetail.md_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                                acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                            }
                            GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                            break;
                        }
                    }
                }

                //Active Death No Payment
                //PIR 630
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                              //&& row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED
                                && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                && (row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() ||
                                    (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && ((row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT && !row.Field<DateTime?>("RETIREMENT_DATE").IsNull() && row.Field<DateTime>("RETIREMENT_DATE") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                     || (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO))))
                                && (!row.Field<DateTime?>("DATE_OF_DEATH").IsNull() && row.Field<DateTime?>("DATE_OF_DEATH") != DateTime.MinValue && row.Field<DateTime?>("DATE_OF_DEATH") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryActiveDeath;

                    var ldtTemp = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                   where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                       //&& row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED
                                         && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                         && (row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() ||
                                             (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && ((row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT && !row.Field<DateTime?>("RETIREMENT_DATE").IsNull() && row.Field<DateTime>("RETIREMENT_DATE") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                              || (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO))))
                                         && (!row.Field<DateTime?>("DATE_OF_DEATH").IsNull() && row.Field<DateTime?>("DATE_OF_DEATH") != DateTime.MinValue && row.Field<DateTime?>("DATE_OF_DEATH") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                   //select row.Field<DateTime?>("RETIREMENT_DATE"));
                                   select new { istrRetirementDate = row.Field<DateTime?>("RETIREMENT_DATE") }).Distinct();
                    //PIR 630
                    if (ldtTemp.IsNotNull() && ldtTemp.Count() > 0)
                    {
                        foreach (var dr in ldtTemp)
                        {
                            if (dr.istrRetirementDate.IsNotNull())
                            {
                                acdoIAPAllocationDetail.first_pmt_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                                acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                            }
                            GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                            break;
                        }
                    }
                }

                //Active Death, with payee account set up
                //PIR 630
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                              //&& row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED
                                && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() && row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO) && !row.Field<DateTime?>("RETIREMENT_DATE").IsNull() && row.Field<DateTime>("RETIREMENT_DATE") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                && (!row.Field<DateTime?>("DATE_OF_DEATH").IsNull() && row.Field<DateTime?>("DATE_OF_DEATH") != DateTime.MinValue && row.Field<DateTime?>("DATE_OF_DEATH") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryActiveDeathFirstPayment;

                    var ldtTemp = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                   where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                       //&& row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED
                                         && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                         && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull() && row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO) && !row.Field<DateTime?>("RETIREMENT_DATE").IsNull() && row.Field<DateTime>("RETIREMENT_DATE") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                         && (!row.Field<DateTime?>("DATE_OF_DEATH").IsNull() && row.Field<DateTime?>("DATE_OF_DEATH") != DateTime.MinValue && row.Field<DateTime?>("DATE_OF_DEATH") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                                   //select row.Field<DateTime?>("RETIREMENT_DATE"));
                                   select new { istrRetirementDate = row.Field<DateTime?>("RETIREMENT_DATE") }).Distinct();
                    //PIR 630
                    if (ldtTemp.IsNotNull() && ldtTemp.Count() > 0)
                    {
                        foreach (var dr in ldtTemp)
                        {
                            if (dr.istrRetirementDate.IsNotNull())
                            {
                                acdoIAPAllocationDetail.first_pmt_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                                acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                            }
                            GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                            break;
                        }
                    }
                }

                //Retiree 
                //PIR 630
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                              //&& (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED)
                              // Commented because there PersonAccount status might be ACTIVE coz payments have NOT yet happened. Also needed to consider for OLD QDRO and RTMT in allocation Year
                                && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DISABILITY || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL))
                                && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime?>("RETIREMENT_DATE") < busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                          //&& (Convert.ToDateTime(row.Field<DateTime?>("RETIREMENT_DATE"))).Year == Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])
                          //Categorizing here is still ok Since if he doesnt have any balances later on in CODE we are NOT inserting
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryRetiree;

                    var ldtTemp = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                   where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                       //&& (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED)
                                       // Commented because there PersonAccount status might be ACTIVE coz payments have NOT yet happened. Also needed to consider for OLD QDRO and RTMT in allocation Year
                                         && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                         && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                         && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0 && (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DISABILITY || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_QDRO || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_WITHDRAWAL))
                                         && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime?>("RETIREMENT_DATE") < busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                                   //&& (Convert.ToDateTime(row.Field<DateTime?>("RETIREMENT_DATE"))).Year == Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])
                                   //Categorizing here is still ok Since if he doesnt have any balances later on in CODE we are NOT inserting
                                   //select row.Field<DateTime?>("RETIREMENT_DATE"));
                                   select new { istrRetirementDate = row.Field<DateTime?>("RETIREMENT_DATE") }).Distinct();
                    //PIR 630
                    if (ldtTemp.IsNotNull() && ldtTemp.Count() > 0)
                    {
                        foreach (var dr in ldtTemp)
                        {
                            if (dr.istrRetirementDate.IsNotNull())
                            {
                                acdoIAPAllocationDetail.retire_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                                acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                            }
                            GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                            break;
                        }
                    }
                }

                //Retiree  -- DDPT guys where DDPT is in ALLOCATIONYEAR + 1
                //PIR 630
                else if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                          where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED)
                                && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0)
                                && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull())

                                && ((row.Field<DateTime?>("RETIREMENT_DATE") < busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                                     && (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DISABILITY))
                                     ||
                                     (row.Field<DateTime?>("RETIREMENT_DATE") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                                       && (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT))
                                   )
                          select row.Field<string>("SSN")).Count() == lintTotalCountofPayeeAccounts4Person && lintTotalCountofPayeeAccounts4Person > 0)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryRetiree;

                    var ldtTemp = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                   where row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                                         && (row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED || row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_DECEASED)
                                         && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()
                                         && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull())
                                         && (row.Field<int?>("PAYEE_ACCOUNT_ID") > 0)
                                         && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull())

                                         && ((row.Field<DateTime?>("RETIREMENT_DATE") < busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                                              && (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_RETIREMENT || row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DISABILITY))
                                              ||
                                              (row.Field<DateTime?>("RETIREMENT_DATE") > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                                                && (row.Field<string>("BENEFIT_TYPE") == busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT))
                                            )
                                   //select row.Field<DateTime?>("RETIREMENT_DATE"));
                                   select new { istrRetirementDate = row.Field<DateTime?>("RETIREMENT_DATE") }).Distinct();
                    //PIR 630
                    if (ldtTemp.IsNotNull() && ldtTemp.Count() > 0)
                    {
                        foreach (var dr in ldtTemp)
                        {
                            if (dr.istrRetirementDate.IsNotNull())
                            {
                                acdoIAPAllocationDetail.retire_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                                acdoIAPAllocationDetail.determination_date = Convert.ToDateTime(Convert.ToString(dr.istrRetirementDate));
                            }
                            GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                            break;
                        }
                    }
                }

                //NEGATIVE BALANCES - IAP
                else if (adrContribution["SYSTEM_BEGINNING_BALANCE"].IsDBNull() != DBNull.Value && Convert.ToDecimal(adrContribution["SYSTEM_BEGINNING_BALANCE"]) < Decimal.Zero && adrContribution["FUND_TYPE"].ToString() == busConstant.IAP)
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryNegBalance;

                    acdoIAPAllocationDetail.first_pmt_date = DateTime.MinValue;
                    acdoIAPAllocationDetail.determination_date = DateTime.MinValue;

                    GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                }

                // Misc  
                else
                {
                    ltempIapAllocationCategory = busConstant.IAPAllocationCategoryMisc;

                    acdoIAPAllocationDetail.first_pmt_date = DateTime.MinValue;
                    acdoIAPAllocationDetail.determination_date = DateTime.MinValue;

                    GetIAPHoursUsingDeterminationdate(astrSSN, acdoIAPAllocationDetail.determination_date, adtbFullInfoFromEADB, ref acdoIAPAllocationDetail);
                }
            }
            return ltempIapAllocationCategory;
            #endregion
        }

        private void GenerateSnapshot(DataRow adrContribution, DataRow[] adrHours, DataRow[] adrPayoutsAndUnallocableAmount, utlPassInfo aobjPassInfo, bool ablnAdjpascutoff)
        {
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

            aobjPassInfo.BeginTransaction();
            try
            {
                cdoIapAllocationDetail lcdoIAPAllocationDetail = new cdoIapAllocationDetail();
                busIAPAllocationHelper lobjIAPHelper = new busIAPAllocationHelper();
                string lstrIapAllocationCategory = string.Empty;
                decimal ldecTotalIAPHours = 0.0M;
                decimal ldecTotalIAPHoursA2 = 0.0M;
                decimal ldecIAPPercent = 0.00M;
                decimal ldecTotalIAPPercent = 0.00M;
                decimal ldecTotalEligibleIAPPercent = 0.00M;
                decimal ldecTotalOverlimit = 0.00M;
                DataRow ldtRowRemployed = null;
                decimal ldecAge = decimal.Zero;
                DateTime ldtRetirementDate = new DateTime();
                bool lblnRetiredGuy = false;

                lcdoIAPAllocationDetail.LoadData(adrContribution);

                if (adrHours != null && adrHours.Length > 0 && adrContribution["FUND_TYPE"].ToString() == busConstant.IAP)
                {
                    #region Re-Employed Categorization and Checking Hours Logic
                    DataTable ldtYearHours = new DataTable();
                    DataTable ldtYearHoursPriortoRetDate = new DataTable();
                    DataTable ldtblTemp = new DataTable();
                    bool lblnReEmployed = false;

                    ldtYearHours.Columns.Add("empaccountno", Type.GetType("System.Int32"));
                    ldtYearHours.Columns.Add("iaphours", Type.GetType("System.Decimal"));
                    ldtYearHours.Columns.Add("iaphoursa2", Type.GetType("System.Decimal"));
                    ldtYearHours.Columns.Add("iappercent", Type.GetType("System.Decimal"));
                    ldtYearHours.Columns.Add("late_flag", Type.GetType("System.String"));
                    ldtYearHours.Columns.Add("computationyear", Type.GetType("System.Int16"));

                    ldtYearHoursPriortoRetDate.Columns.Add("empaccountno", Type.GetType("System.Int32"));
                    ldtYearHoursPriortoRetDate.Columns.Add("iaphours", Type.GetType("System.Decimal"));
                    ldtYearHoursPriortoRetDate.Columns.Add("iaphoursa2", Type.GetType("System.Decimal"));
                    ldtYearHoursPriortoRetDate.Columns.Add("iappercent", Type.GetType("System.Decimal"));
                    ldtYearHoursPriortoRetDate.Columns.Add("late_flag", Type.GetType("System.String"));
                    ldtYearHoursPriortoRetDate.Columns.Add("computationyear", Type.GetType("System.Int16"));


                    ldtblTemp.Columns.Add("empaccountno", Type.GetType("System.Int32"));
                    ldtblTemp.Columns.Add("iaphours", Type.GetType("System.Decimal"));
                    ldtblTemp.Columns.Add("iaphoursa2", Type.GetType("System.Decimal"));
                    ldtblTemp.Columns.Add("iappercent", Type.GetType("System.Decimal"));
                    ldtblTemp.Columns.Add("late_flag", Type.GetType("System.String"));
                    ldtblTemp.Columns.Add("computationyear", Type.GetType("System.Int16"));


                    if (ldtbParticipantInfo4Category.IsNotNull() && ldtbParticipantInfo4Category.Rows.Count > 0)
                    {
                        if ((from row in ldtbParticipantInfo4Category.AsEnumerable()
                             where row.Field<string>("SSN") == adrContribution["SSN"].ToString() && ((!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull()) && row.Field<int>("PAYEE_ACCOUNT_ID") > 0)
                                 && row.Field<string>("FUND_TYPE") == busConstant.IAP && row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED
                                 && (Convert.ToString(row.Field<string>("RETIREMENT_TYPE")).IsNullOrEmpty() || row.Field<string>("RETIREMENT_TYPE") != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)//PIR 630
                                 && (row.Field<string>("BENEFIT_TYPE") != busConstant.BENEFIT_TYPE_QDRO)
                                 && (!row.Field<DateTime?>("RETIREMENT_DATE").IsNull()) && row.Field<DateTime>("RETIREMENT_DATE") <= busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))
                             select row).Any())
                        {
                            //if ((from row in ldtbParticipantInfo4Category.AsEnumerable() where row.Field<string>("SSN") == adrContribution["SSN"].ToString() && row.Field<int>("PERSON_ACCOUNT_ID") > 0 && row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED select row).Count() > 0)
                            //{
                            ldtRowRemployed = (from row in ldtbParticipantInfo4Category.AsEnumerable()
                                               where row.Field<string>("SSN") == adrContribution["SSN"].ToString()
                                                   && (!row.Field<int?>("PAYEE_ACCOUNT_ID").IsNull()) && row.Field<int>("PERSON_ACCOUNT_ID") > 0
                                                   && row.Field<string>("STATUS_VALUE") == busConstant.PERSON_ACCOUNT_STATUS_RETIRED
                                                   && (Convert.ToString(row.Field<string>("RETIREMENT_TYPE")).IsNullOrEmpty() || row.Field<string>("RETIREMENT_TYPE") != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)//PIR 630
                                                   && row.Field<string>("BENEFIT_TYPE") != busConstant.BENEFIT_TYPE_QDRO
                                                   && row.Field<string>("FUND_TYPE") == busConstant.IAP
                                               select row).FirstOrDefault();
                            if (ldtRowRemployed.IsNotNull() && ldtRowRemployed["RETIREMENT_DATE"] != DBNull.Value && ldtRowRemployed["RETIREMENT_DATE"].IsNotNull() && Convert.ToDateTime(ldtRowRemployed["RETIREMENT_DATE"]) != DateTime.MinValue)
                            {
                                ldtRetirementDate = Convert.ToDateTime(ldtRowRemployed["RETIREMENT_DATE"]);
                                DateTime ldtDOBDateTime = ldtRowRemployed["DOB"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(ldtRowRemployed["DOB"]);
                                DateTime ldtEndDate = busGlobalFunctions.GetLastDateOfComputationYear(iobjSystemManagement.icdoSystemManagement.batch_date.AddYears(-1).Year);
                                if (ldtDOBDateTime != DateTime.MinValue || ldtDOBDateTime.IsNotNull())
                                    ldecAge = busGlobalFunctions.CalculatePersonAge(ldtDOBDateTime, ldtEndDate);
                                else
                                    ldecAge = 0;

                                //Should Contain hours after retirement date.
                                //lenmCurrentYearHours = adrHours.AsEnumerable().Where(o => o.Field<string>("late_flag") == busConstant.FLAG_NO && o.Field<DateTime>("FromDate") > busGlobalFunctions.GetLastDayOfWeek(ldtRetirementDate).AddDays(1));
                                //lenmLateHours = adrHours.AsEnumerable().Where(o => o.Field<string>("late_flag") == busConstant.FLAG_YES && o.Field<DateTime>("FromDate") > busGlobalFunctions.GetLastDayOfWeek(ldtRetirementDate).AddDays(1));
                                lblnReEmployed = true;
                            }
                            //}
                        }
                    }

                    if (lblnReEmployed && ldtRetirementDate.IsNotNull() && ldtRetirementDate != DateTime.MinValue && ldtRetirementDate.Year == (Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()])))
                    {
                        lblnReEmployed = true;

                        var lYearlyHrs = from DataRow row in adrHours.AsEnumerable()
                                         where row.Field<DateTime>("FromDate") >= busGlobalFunctions.GetLastDayOfWeek(ldtRetirementDate).AddDays(1)
                                         group row by new { ID = row.Field<Int32>("empaccountno"), ID1 = row.Field<Int16>("computationyear") } into g
                                         select new
                                         {
                                             empaccountno = g.Key.ID,
                                             computationyear = g.Key.ID1,
                                             //iaphours = g.Sum(row => row.Field<decimal?>("iaphours") == null ? 0.0M : (decimal)row["iaphours"]),
                                             //iaphours2 = g.Sum(row => row.Field<decimal?>("iaphoursa2") == null ? 0.0M : (decimal)row["iaphours"]),
                                             //iappercent = g.Sum(row => row.Field<decimal?>("iappercent") == null ? 0.0M : (decimal)row["iaphours"]),
                                             iaphours = g.Sum(row => row["iaphours"] == DBNull.Value ? 0.0M : (decimal)row["iaphours"]),
                                             iaphoursa2 = g.Sum(row => row["iaphoursa2"] == DBNull.Value ? 0.0M : (decimal)row["iaphoursa2"]),
                                             iappercent = g.Sum(row => row["iappercent"] == DBNull.Value ? 0.0M : (decimal)row["iappercent"]),
                                             late_flag = g.Select(row => (string)row["late_flag"]).FirstOrDefault()

                                         };

                        foreach (var ldtGRow in lYearlyHrs)
                        {
                            DataRow ldtRow = ldtYearHours.NewRow();
                            ldtRow["empaccountno"] = ldtGRow.empaccountno;
                            ldtRow["computationyear"] = ldtGRow.computationyear;
                            ldtRow["iaphours"] = ldtGRow.iaphours;
                            ldtRow["iaphoursa2"] = ldtGRow.iaphoursa2;
                            ldtRow["iappercent"] = ldtGRow.iappercent;
                            ldtRow["late_flag"] = ldtGRow.late_flag;
                            ldtYearHours.Rows.Add(ldtRow);
                        }

                        var lYearlyHrsPriortoRetDate = from DataRow row in adrHours.AsEnumerable()
                                                       where row.Field<DateTime>("FromDate") < busGlobalFunctions.GetLastDayOfWeek(ldtRetirementDate).AddDays(1)
                                                       group row by new { ID = row.Field<Int32>("empaccountno"), ID1 = row.Field<Int16>("computationyear") } into g
                                                       select new
                                                       {
                                                           empaccountno = g.Key.ID,
                                                           computationyear = g.Key.ID1,
                                                           //iaphours = g.Sum(row => row.Field<decimal?>("iaphours") == null ? 0.0M : (decimal)row["iaphours"]),
                                                           //iaphours2 = g.Sum(row => row.Field<decimal?>("iaphoursa2") == null ? 0.0M : (decimal)row["iaphours"]),
                                                           //iappercent = g.Sum(row => row.Field<decimal?>("iappercent") == null ? 0.0M : (decimal)row["iaphours"]),
                                                           iaphours = g.Sum(row => row["iaphours"] == DBNull.Value ? 0.0M : (decimal)row["iaphours"]),
                                                           iaphoursa2 = g.Sum(row => row["iaphoursa2"] == DBNull.Value ? 0.0M : (decimal)row["iaphoursa2"]),
                                                           iappercent = g.Sum(row => row["iappercent"] == DBNull.Value ? 0.0M : (decimal)row["iappercent"]),
                                                           late_flag = g.Select(row => (string)row["late_flag"]).FirstOrDefault()

                                                       };

                        foreach (var ldtGRow in lYearlyHrsPriortoRetDate)
                        {
                            DataRow ldtRow = ldtYearHoursPriortoRetDate.NewRow();
                            ldtRow["empaccountno"] = ldtGRow.empaccountno;
                            ldtRow["computationyear"] = ldtGRow.computationyear;
                            ldtRow["iaphours"] = ldtGRow.iaphours;
                            ldtRow["iaphoursa2"] = ldtGRow.iaphoursa2;
                            ldtRow["iappercent"] = ldtGRow.iappercent;
                            ldtRow["late_flag"] = ldtGRow.late_flag;
                            ldtYearHoursPriortoRetDate.Rows.Add(ldtRow);
                        }


                    }
                    else
                    {
                        var lYearlyHrs = from DataRow row in adrHours.AsEnumerable()
                                         group row by new { ID = row.Field<Int32>("empaccountno"), ID1 = row.Field<Int16>("computationyear") } into g
                                         select new
                                         {
                                             empaccountno = g.Key.ID,
                                             computationyear = g.Key.ID1,
                                             //iaphours = g.Sum(row => row.Field<decimal?>("iaphours") == null ? 0.0M : (decimal)row["iaphours"]),
                                             //iaphours2 = g.Sum(row => row.Field<decimal?>("iaphoursa2") == null ? 0.0M : (decimal)row["iaphours"]),
                                             //iappercent = g.Sum(row => row.Field<decimal?>("iappercent") == null ? 0.0M : (decimal)row["iaphours"]),
                                             iaphours = g.Sum(row => row["iaphours"] == DBNull.Value ? 0.0M : (decimal)row["iaphours"]),
                                             iaphoursa2 = g.Sum(row => row["iaphoursa2"] == DBNull.Value ? 0.0M : (decimal)row["iaphoursa2"]),
                                             iappercent = g.Sum(row => row["iappercent"] == DBNull.Value ? 0.0M : (decimal)row["iappercent"]),
                                             late_flag = g.Select(row => (string)row["late_flag"]).FirstOrDefault()

                                         };

                        foreach (var ldtGRow in lYearlyHrs)
                        {
                            DataRow ldtRow = ldtYearHours.NewRow();
                            ldtRow["empaccountno"] = ldtGRow.empaccountno;
                            ldtRow["computationyear"] = ldtGRow.computationyear;
                            ldtRow["iaphours"] = ldtGRow.iaphours;
                            ldtRow["iaphoursa2"] = ldtGRow.iaphoursa2;
                            ldtRow["iappercent"] = ldtGRow.iappercent;
                            ldtRow["late_flag"] = ldtGRow.late_flag;
                            ldtYearHours.Rows.Add(ldtRow);
                            //adrYearHours.Add(row);
                            //Add(i++, row.computationyear, row.iaphours, row.iaphoursa2, row.iappercent);
                        }
                    }


                    IEnumerable<DataRow> lenmCurrentYearHours = ldtYearHours.AsEnumerable().Where(o => o.Field<string>("late_flag") == busConstant.FLAG_NO);

                    IEnumerable<DataRow> lenmLateHours = new DataTable().AsEnumerable();


                    if (ldtYearHoursPriortoRetDate != null && ldtYearHoursPriortoRetDate.AsEnumerable().Where(o => o.Field<string>("late_flag") == busConstant.FLAG_YES).Count() > 0)
                    {
                        foreach (DataRow ldrYearHoursPriortoRetDate in ldtYearHoursPriortoRetDate.AsEnumerable().Where(o => o.Field<string>("late_flag") == busConstant.FLAG_YES))
                        {
                            DataRow ldtRow = ldtblTemp.NewRow();
                            ldtRow["empaccountno"] = ldrYearHoursPriortoRetDate["empaccountno"];
                            ldtRow["computationyear"] = ldrYearHoursPriortoRetDate["computationyear"];
                            ldtRow["iaphours"] = ldrYearHoursPriortoRetDate["iaphours"];
                            ldtRow["iaphoursa2"] = ldrYearHoursPriortoRetDate["iaphoursa2"];
                            ldtRow["iappercent"] = ldrYearHoursPriortoRetDate["iappercent"];
                            ldtRow["late_flag"] = ldrYearHoursPriortoRetDate["late_flag"];
                            ldtblTemp.Rows.Add(ldtRow);
                        }
                    }

                    if (ldtYearHours != null && ldtYearHours.AsEnumerable().Where(o => o.Field<string>("late_flag") == busConstant.FLAG_YES).Count() > 0)
                    {
                        foreach (DataRow ldrYearHours in ldtYearHours.AsEnumerable().Where(o => o.Field<string>("late_flag") == busConstant.FLAG_YES))
                        {
                            DataRow ldtRow = ldtblTemp.NewRow();
                            ldtRow["empaccountno"] = ldrYearHours["empaccountno"];
                            ldtRow["computationyear"] = ldrYearHours["computationyear"];
                            ldtRow["iaphours"] = ldrYearHours["iaphours"];
                            ldtRow["iaphoursa2"] = ldrYearHours["iaphoursa2"];
                            ldtRow["iappercent"] = ldrYearHours["iappercent"];
                            ldtRow["late_flag"] = ldrYearHours["late_flag"];
                            ldtblTemp.Rows.Add(ldtRow);
                        }
                    }

                    lenmLateHours = ldtblTemp.AsEnumerable().OrderBy(item => item.Field<Int16>("computationyear"));

                    //lenmLateHours = ldtYearHoursPriortoRetDate.AsEnumerable().Where(o => o.Field<string>("late_flag") == busConstant.FLAG_YES);
                    //lenmLateHours = ldtYearHours.AsEnumerable().Where(o => o.Field<string>("late_flag") == busConstant.FLAG_YES);

                    #region For Ineligible Hours Before the Retirement Date in Allocation Year
                    if (ldtYearHoursPriortoRetDate.AsEnumerable().Where(yr => Convert.ToInt32(yr["computationyear"]) == (Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))).Any())
                    {
                        foreach (DataRow ldr in ldtYearHoursPriortoRetDate.AsEnumerable().Where(yr => Convert.ToInt32(yr["computationyear"]) == (Convert.ToInt32(adrContribution[enmIapAllocationDetail.computation_year.ToString()]))))
                        {
                            ldecTotalIAPHours += ldr["iaphours"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphours"]);
                            ldecTotalIAPHoursA2 += ldr["iaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphoursa2"]);
                            ldecTotalIAPPercent += ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"]);

                            ldecIAPPercent = lobjIAPHelper.CalculateAllocation4Amount(Convert.ToInt32(ldr["computationyear"]), null, adecIAPPercent: (ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"])));

                            if ((Convert.ToDecimal(ldr["iappercent"]) - ldecIAPPercent) > 0)
                            {
                                InsertIntoOverlimtTable(Convert.ToInt32(adrContribution["person_account_id"]), (ldr["empaccountno"] == DBNull.Value ? 0 : Convert.ToInt32(ldr["empaccountno"])), Convert.ToInt32(ldr["computationyear"]),
                                    (ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"])), (ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"])) - ldecIAPPercent);
                                ldecTotalOverlimit += Convert.ToDecimal(ldr["iappercent"]) - ldecIAPPercent;
                            }
                            ldecTotalEligibleIAPPercent += ldecIAPPercent;
                            ldecIAPPercent = 0.00M;
                        }

                        //lock (iobjLock)
                        //{
                        if ((ldecTotalIAPHours < 400))
                        {
                            lcdoIAPAllocationDetail.current_year_ineligible_hours = ldecTotalIAPHoursA2;
                            lcdoIAPAllocationDetail.current_year_ineligible_contribution_amount = ldecTotalIAPHoursA2 * idecIAPAllocation2FactorData1;//Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPAllocation2Factor));
                            lcdoIAPAllocationDetail.current_year_inelgibile_compensation_amount = ldecTotalIAPPercent;
                        }

                        lcdoIAPAllocationDetail.overlimit_contributions_amount = ldecTotalOverlimit;
                        //}

                        ldecTotalIAPHours = ldecTotalIAPHoursA2 = ldecTotalIAPPercent = ldecIAPPercent = ldecTotalEligibleIAPPercent = ldecTotalOverlimit = 0.0M;
                    }
                    #endregion


                    foreach (DataRow ldr in lenmCurrentYearHours)
                    {
                        ldecTotalIAPHours += ldr["iaphours"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphours"]);
                        ldecTotalIAPHoursA2 += ldr["iaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphoursa2"]);
                        ldecTotalIAPPercent += ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"]);

                        ldecIAPPercent = lobjIAPHelper.CalculateAllocation4Amount(Convert.ToInt32(ldr["computationyear"]), null, adecIAPPercent: (ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"])));

                        if ((Convert.ToDecimal(ldr["iappercent"]) - ldecIAPPercent) > 0)
                        {
                            InsertIntoOverlimtTable(Convert.ToInt32(adrContribution["person_account_id"]), (ldr["empaccountno"] == DBNull.Value ? 0 : Convert.ToInt32(ldr["empaccountno"])), Convert.ToInt32(ldr["computationyear"]),
                                (ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"])), (ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"])) - ldecIAPPercent);
                            ldecTotalOverlimit += Convert.ToDecimal(ldr["iappercent"]) - ldecIAPPercent;
                        }
                        ldecTotalEligibleIAPPercent += ldecIAPPercent;
                        ldecIAPPercent = 0.00M;
                    }

                    //lock (iobjLock)
                    //{
                    lcdoIAPAllocationDetail.total_iap_hours = ldecTotalIAPHours;

                    lcdoIAPAllocationDetail.overlimit_contributions_amount = lcdoIAPAllocationDetail.overlimit_contributions_amount + ldecTotalOverlimit;

                        if ((ldecTotalIAPHours < 400 && lblnReEmployed == false) || (ldecTotalIAPHours < 870 && lblnReEmployed))
                        {
                        lcdoIAPAllocationDetail.current_year_ineligible_hours =
                               lcdoIAPAllocationDetail.current_year_ineligible_hours + ldecTotalIAPHoursA2;
                        lcdoIAPAllocationDetail.current_year_ineligible_contribution_amount =
                            lcdoIAPAllocationDetail.current_year_ineligible_contribution_amount + (ldecTotalIAPHoursA2 * idecIAPAllocation2FactorData1);//Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPAllocation2Factor)));
                        lcdoIAPAllocationDetail.current_year_inelgibile_compensation_amount =
                                lcdoIAPAllocationDetail.current_year_inelgibile_compensation_amount + ldecTotalIAPPercent;
                        }
                        else
                        {
                        lcdoIAPAllocationDetail.eligible_hours = ldecTotalIAPHoursA2;
                        lcdoIAPAllocationDetail.hourly_contribution_amount = ldecTotalIAPHoursA2 * idecIAPAllocation2FactorData1;// Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPAllocation2Factor));
                        lcdoIAPAllocationDetail.percentage_of_compensation_amount = ldecTotalEligibleIAPPercent;
                        }
                    //}

                    ldecTotalIAPHours = ldecTotalIAPHoursA2 = 0.0M;
                    ldecTotalEligibleIAPPercent = ldecTotalIAPPercent = ldecTotalOverlimit = 0.00M;

                    utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                    string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                    SqlParameter[] parameters = new SqlParameter[3];
                    SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                    SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
                    SqlParameter param3 = new SqlParameter("@COMPUTATIONYEAR", DbType.Int32);

                    param1.Value = adrContribution["ssn"].ToString();
                    parameters[0] = param1;
                    param2.Value = busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToInt32(adrContribution["computation_year"]) - 1).AddDays(1);
                    parameters[1] = param2;
                    param3.Value = Convert.ToInt32(adrContribution["computation_year"]);
                    parameters[2] = param3;

                    DataTable ldtFullWorkHistory = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkhistoryForIAPAllocation", astrLegacyDBConnetion, null, parameters);
                    utlLegacyDBConnetion = null;

                    decimal ldecTotalLateIAPHours = 0.0M;
                    decimal ldecTotalLateIAPHoursA2 = 0.0M;
                    decimal ldecInEligibleLateIAPHours = 0.0M;
                    decimal ldecInEligibleLateIAPHoursA2 = 0.0M;
                    decimal ldecInEligibleLateIAPHoursPercent = 0.00M;
                    decimal ldecLateEligibleIAPPercent = 0.00M;
                    decimal ldecTotalLateIAPPercent = 0.00M;
                    int lintPrevYear = 0;

                    foreach (DataRow ldr in lenmLateHours)
                    {
                        if (lintPrevYear != 0 && lintPrevYear != Convert.ToInt32(ldr["computationyear"]))
                        {
                            DataRow[] ldrActualHoursForYear = ldtFullWorkHistory.FilterTable(utlDataType.Numeric, "computationyear", lintPrevYear);
                            foreach (DataRow ldrActual in ldrActualHoursForYear)
                            {
                                ldecTotalIAPHours += ldrActual["iaphours"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldrActual["iaphours"]);
                            }
                            if ((ldecTotalLateIAPHours + ldecTotalIAPHours) < 400)
                            {
                                ldecInEligibleLateIAPHours += ldecTotalLateIAPHours;
                                ldecInEligibleLateIAPHoursA2 += ldecTotalLateIAPHoursA2;
                                ldecInEligibleLateIAPHoursPercent += ldecTotalLateIAPPercent;
                            }
                            ldecTotalIAPHours = ldecTotalLateIAPHours = ldecTotalLateIAPHoursA2 = 0.0M;
                            ldecTotalLateIAPPercent = 0.00M;
                        }
                        ldecTotalLateIAPHours += ldr["iaphours"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphours"]);
                        ldecTotalLateIAPHoursA2 += ldr["iaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphoursa2"]);
                        ldecTotalLateIAPPercent += ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"]);

                        bool lblnEmpFound = false;

                        DataRow[] ldrFilteredWorkHistory = ldtFullWorkHistory.FilterTable(utlDataType.Numeric, "computationyear", Convert.ToInt32(ldr["computationyear"]));
                        if (!ldrFilteredWorkHistory.IsNullOrEmpty() && ldrFilteredWorkHistory.Count() > 0)
                        {
                            foreach (DataRow ldrFiltered in ldrFilteredWorkHistory)
                            {
                                if ((ldrFiltered["empaccountno"] == DBNull.Value && ldr["empaccountno"] == DBNull.Value) ||
                                    (ldrFiltered["empaccountno"] != DBNull.Value && ldr["empaccountno"] != DBNull.Value && Convert.ToInt32(ldrFiltered["empaccountno"]) == Convert.ToInt32(ldr["empaccountno"])))
                                {
                                    ldecLateEligibleIAPPercent =
                                    lobjIAPHelper.CalculateAllocation4Amount(Convert.ToInt32(ldr["computationyear"]), null, adecIAPPercent: ((ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"])) +
                                    (ldrFiltered["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldrFiltered["iappercent"])))) -
                                         lobjIAPHelper.CalculateAllocation4Amount(Convert.ToInt32(ldr["computationyear"]), null, (ldrFiltered["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldrFiltered["iappercent"])));
                                    lblnEmpFound = true;
                                    break;
                                }
                            }
                        }

                        if (lblnEmpFound == false || ldrFilteredWorkHistory.IsNullOrEmpty())
                        {
                            ldecLateEligibleIAPPercent =
                                   lobjIAPHelper.CalculateAllocation4Amount(Convert.ToInt32(ldr["computationyear"]), null, adecIAPPercent: ((ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"]))));

                        }

                        if ((Convert.ToDecimal(ldr["iappercent"]) - ldecLateEligibleIAPPercent) > 0)
                        {
                            InsertIntoOverlimtTable(Convert.ToInt32(adrContribution["person_account_id"]), (ldr["empaccountno"] == DBNull.Value ? 0 : Convert.ToInt32(ldr["empaccountno"])), Convert.ToInt32(ldr["computationyear"]),
                                (ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"])), (ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"])) - ldecLateEligibleIAPPercent);
                            ldecTotalOverlimit += (ldr["iappercent"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldr["iappercent"])) - ldecLateEligibleIAPPercent;
                        }
                        lintPrevYear = Convert.ToInt32(ldr["computationyear"]);
                        ldecLateEligibleIAPPercent = 0.00M;
                        lblnEmpFound = false;
                        ldrFilteredWorkHistory = null;
                    }
                    //for the last year in late allcation
                    if (lintPrevYear != 0)
                    {
                        DataRow[] ldrActualHoursForYear = ldtFullWorkHistory.FilterTable(utlDataType.Numeric, "computationyear", lintPrevYear);
                        foreach (DataRow ldrActual in ldrActualHoursForYear)
                        {
                            ldecTotalIAPHours += ldrActual["iaphours"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldrActual["iaphours"]);
                        }
                        if ((ldecTotalLateIAPHours + ldecTotalIAPHours) < 400)
                        {
                            ldecInEligibleLateIAPHours += ldecTotalLateIAPHours;
                            ldecInEligibleLateIAPHoursA2 += ldecTotalLateIAPHoursA2;
                            ldecInEligibleLateIAPHoursPercent += ldecTotalLateIAPPercent;
                        }
                        ldrActualHoursForYear = null;
                    }

                    //lock (iobjLock)
                    //{
                    lcdoIAPAllocationDetail.total_ineligible_iap_late_hours = ldecInEligibleLateIAPHours;
                    lcdoIAPAllocationDetail.late_ineligible_hours = ldecInEligibleLateIAPHoursA2;
                    lcdoIAPAllocationDetail.late_ineligible_hourly_contribution_amount = ldecInEligibleLateIAPHoursA2 * idecIAPAllocation2FactorData1;// Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPAllocation2Factor));
                    lcdoIAPAllocationDetail.late_inelgibile_compensation_amount = ldecInEligibleLateIAPHoursPercent;
                    lcdoIAPAllocationDetail.overlimit_contributions_amount = lcdoIAPAllocationDetail.overlimit_contributions_amount + ldecTotalOverlimit;
                    //}

                    ldtFullWorkHistory = null;
                    #endregion
                }

                string lstrSSN = string.Empty;


                if (ldtbParticipantInfo4Category != null && ldtbParticipantInfo4Category.AsEnumerable().Where(row => row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                         && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()).Count() > 0)
                {
                    DataRow ldrParticipantInfo4Category = ldtbParticipantInfo4Category.AsEnumerable().Where(row => row.Field<int>("PERSON_ACCOUNT_ID") == Convert.ToInt32(adrContribution["PERSON_ACCOUNT_ID"])
                         && row.Field<string>("FUND_TYPE") == adrContribution["FUND_TYPE"].ToString()).FirstOrDefault();


                    if (ldrParticipantInfo4Category != null && Convert.ToString(ldrParticipantInfo4Category["DOB"]).IsNotNullOrEmpty())
                    {
                        lcdoIAPAllocationDetail.birth_date = Convert.ToDateTime(ldrParticipantInfo4Category["DOB"]);
                    }

                    if (Convert.ToString(adrContribution["SSN"]).IsNotNullOrEmpty())
                    {
                        lstrSSN = Convert.ToString(adrContribution["SSN"]);
                    }

                    if (ldrParticipantInfo4Category != null && Convert.ToString(ldrParticipantInfo4Category[enmPerson.date_of_death.ToString().ToUpper()]).IsNotNullOrEmpty())
                    {
                        lcdoIAPAllocationDetail.deceased_date = Convert.ToDateTime(ldrParticipantInfo4Category[enmPerson.date_of_death.ToString().ToUpper()]);
                    }

                }
                lock (iobjLock)
                {
                    adrContribution[enmIapAllocationDetail.total_iap_hours.ToString()] = lcdoIAPAllocationDetail.total_iap_hours;
                    adrContribution[enmIapAllocationDetail.overlimit_contributions_amount.ToString()] = lcdoIAPAllocationDetail.overlimit_contributions_amount;
                    adrContribution[enmIapAllocationDetail.current_year_ineligible_hours.ToString()] = lcdoIAPAllocationDetail.current_year_ineligible_hours;
                    adrContribution[enmIapAllocationDetail.current_year_ineligible_contribution_amount.ToString()] = lcdoIAPAllocationDetail.current_year_ineligible_contribution_amount;
                    adrContribution[enmIapAllocationDetail.current_year_inelgibile_compensation_amount.ToString()] = lcdoIAPAllocationDetail.current_year_inelgibile_compensation_amount;
                    adrContribution[enmIapAllocationDetail.eligible_hours.ToString()] = lcdoIAPAllocationDetail.eligible_hours;
                    adrContribution[enmIapAllocationDetail.hourly_contribution_amount.ToString()] = lcdoIAPAllocationDetail.hourly_contribution_amount;
                    adrContribution[enmIapAllocationDetail.percentage_of_compensation_amount.ToString()] = lcdoIAPAllocationDetail.percentage_of_compensation_amount;
                    adrContribution[enmIapAllocationDetail.total_ineligible_iap_late_hours.ToString()] = lcdoIAPAllocationDetail.total_ineligible_iap_late_hours;
                    adrContribution[enmIapAllocationDetail.late_ineligible_hours.ToString()] = lcdoIAPAllocationDetail.late_ineligible_hours;
                    adrContribution[enmIapAllocationDetail.late_ineligible_hourly_contribution_amount.ToString()] = lcdoIAPAllocationDetail.late_ineligible_hourly_contribution_amount;
                    adrContribution[enmIapAllocationDetail.late_inelgibile_compensation_amount.ToString()] = lcdoIAPAllocationDetail.late_inelgibile_compensation_amount;
                    //adrContribution[enmIapAllocationDetail.overlimit_contributions_amount.ToString()] = lcdoIAPAllocationDetail.overlimit_contributions_amount;
                    //adrContribution[enmIapAllocationDetail.total_iap_hours.ToString()] = lcdoIAPAllocationDetail.total_iap_hours;

                    //adrContribution[enmIapAllocationDetail.current_year_ineligible_hours.ToString()] = lcdoIAPAllocationDetail.current_year_ineligible_hours;
                    //adrContribution[enmIapAllocationDetail.current_year_ineligible_contribution_amount.ToString()] = lcdoIAPAllocationDetail.current_year_ineligible_contribution_amount;
                    //adrContribution[enmIapAllocationDetail.current_year_inelgibile_compensation_amount.ToString()] = lcdoIAPAllocationDetail.current_year_inelgibile_compensation_amount;
                }
                //lock (iobjLock)
                //{
                //PIR 885 New
                lstrIapAllocationCategory = DetermineCategory(adrContribution, ldtbParticipantInfo4Category, ldtRowRemployed, lcdoIAPAllocationDetail.total_iap_hours, ldecAge, lstrSSN, ldtbFullInfoFromEADB, ref lcdoIAPAllocationDetail);
                //}
                

                    lcdoIAPAllocationDetail.LoadData(adrContribution);


                if (lstrIapAllocationCategory.IsNull() || lstrIapAllocationCategory == String.Empty)
                    lstrIapAllocationCategory = busConstant.IAPAllocationCategoryMisc;
                else
                    lcdoIAPAllocationDetail.iap_allocation_category_value = lstrIapAllocationCategory;

                if (adrPayoutsAndUnallocableAmount != null && adrPayoutsAndUnallocableAmount.Length > 0)
                {
                    lcdoIAPAllocationDetail.payouts = adrPayoutsAndUnallocableAmount[0]["payouts"] == DBNull.Value ? 0.0M : Convert.ToDecimal(adrPayoutsAndUnallocableAmount[0]["payouts"]) * -1;
                }

                if (lcdoIAPAllocationDetail.iap_allocation_category_value == busConstant.IAPAllocationCategoryActiveDeathFirstPayment || lcdoIAPAllocationDetail.iap_allocation_category_value == busConstant.IAPAllocationCategoryMDActiveReeval ||
                    lcdoIAPAllocationDetail.iap_allocation_category_value == busConstant.IAPAllocationCategoryEarlyWithdrawal || lcdoIAPAllocationDetail.iap_allocation_category_value == busConstant.IAPAllocationCategoryMisc
                    || lcdoIAPAllocationDetail.iap_allocation_category_value == busConstant.IAPAllocationCategoryNegBalance ||
                        lcdoIAPAllocationDetail.iap_allocation_category_value == busConstant.IAPAllocationCategoryMDNew || lcdoIAPAllocationDetail.iap_allocation_category_value == busConstant.IAPAllocationCategoryReempOver65 ||
                        (lcdoIAPAllocationDetail.iap_allocation_category_value == busConstant.IAPAllocationCategoryReempUnder65 && ldtRetirementDate.Year == lcdoIAPAllocationDetail.computation_year) ||
                        lcdoIAPAllocationDetail.iap_allocation_category_value == busConstant.IAPAllocationCategoryRetiree || ablnAdjpascutoff)
                {
                    lcdoIAPAllocationDetail.unallocable_amount = (lcdoIAPAllocationDetail.system_beginning_balance + lcdoIAPAllocationDetail.forfeited_balance + lcdoIAPAllocationDetail.quaterly_allocations_amount + lcdoIAPAllocationDetail.retirement_year_allocation2_amount +
                        lcdoIAPAllocationDetail.retirement_year_allocation4_amount + lcdoIAPAllocationDetail.payouts + lcdoIAPAllocationDetail.late_alloc1_amount + lcdoIAPAllocationDetail.late_alloc2_amount + lcdoIAPAllocationDetail.late_alloc3_amount +
                        lcdoIAPAllocationDetail.late_alloc4_amount + lcdoIAPAllocationDetail.late_alloc5_amount + lcdoIAPAllocationDetail.l52_special_account_amount + lcdoIAPAllocationDetail.l161_special_account_amount) * -1;//PIR 630
                }

                //Rohan
                if (lcdoIAPAllocationDetail.iap_allocation_category_value == busConstant.IAPAllocationCategoryActiveDeathFirstPayment)
                {
                    lcdoIAPAllocationDetail.hourly_contribution_amount = 0M;
                    lcdoIAPAllocationDetail.percentage_of_compensation_amount = 0M;
                }

                if (lcdoIAPAllocationDetail.fund_type == "IAP")
                {
                    if ((lcdoIAPAllocationDetail.system_beginning_balance + lcdoIAPAllocationDetail.forfeited_balance +
                            lcdoIAPAllocationDetail.late_alloc1_amount + lcdoIAPAllocationDetail.late_alloc2_amount +
                            lcdoIAPAllocationDetail.late_alloc3_amount + lcdoIAPAllocationDetail.late_alloc4_amount +
                            lcdoIAPAllocationDetail.late_alloc5_amount + lcdoIAPAllocationDetail.quaterly_allocations_amount +
                            lcdoIAPAllocationDetail.retirement_year_allocation2_amount + lcdoIAPAllocationDetail.retirement_year_allocation4_amount +
                            lcdoIAPAllocationDetail.payouts + lcdoIAPAllocationDetail.unallocable_amount) < 0)
                    {
                        lcdoIAPAllocationDetail.unallocable_amount = lcdoIAPAllocationDetail.unallocable_amount -
                            (lcdoIAPAllocationDetail.system_beginning_balance + lcdoIAPAllocationDetail.forfeited_balance +
                            lcdoIAPAllocationDetail.late_alloc1_amount + lcdoIAPAllocationDetail.late_alloc2_amount +
                            lcdoIAPAllocationDetail.late_alloc3_amount + lcdoIAPAllocationDetail.late_alloc4_amount +
                            lcdoIAPAllocationDetail.late_alloc5_amount + lcdoIAPAllocationDetail.quaterly_allocations_amount +
                            lcdoIAPAllocationDetail.retirement_year_allocation2_amount + lcdoIAPAllocationDetail.retirement_year_allocation4_amount +
                            lcdoIAPAllocationDetail.payouts + lcdoIAPAllocationDetail.unallocable_amount);
                    }
                }
                else if (lcdoIAPAllocationDetail.fund_type == "L052")
                {

                    if ((lcdoIAPAllocationDetail.l52_special_account_amount + lcdoIAPAllocationDetail.forfeited_balance +
                      lcdoIAPAllocationDetail.late_alloc1_amount + lcdoIAPAllocationDetail.late_alloc2_amount +
                       lcdoIAPAllocationDetail.late_alloc3_amount + lcdoIAPAllocationDetail.late_alloc4_amount +
                       lcdoIAPAllocationDetail.late_alloc5_amount + lcdoIAPAllocationDetail.quaterly_allocations_amount +
                       lcdoIAPAllocationDetail.retirement_year_allocation2_amount + lcdoIAPAllocationDetail.retirement_year_allocation4_amount +
                       lcdoIAPAllocationDetail.payouts + lcdoIAPAllocationDetail.unallocable_amount) < 0)
                    {
                        lcdoIAPAllocationDetail.unallocable_amount = lcdoIAPAllocationDetail.unallocable_amount -
                            (lcdoIAPAllocationDetail.l52_special_account_amount + lcdoIAPAllocationDetail.forfeited_balance +
                     lcdoIAPAllocationDetail.late_alloc1_amount + lcdoIAPAllocationDetail.late_alloc2_amount +
                      lcdoIAPAllocationDetail.late_alloc3_amount + lcdoIAPAllocationDetail.late_alloc4_amount +
                      lcdoIAPAllocationDetail.late_alloc5_amount + lcdoIAPAllocationDetail.quaterly_allocations_amount +
                      lcdoIAPAllocationDetail.retirement_year_allocation2_amount + lcdoIAPAllocationDetail.retirement_year_allocation4_amount +
                      lcdoIAPAllocationDetail.payouts + lcdoIAPAllocationDetail.unallocable_amount);
                    }
                }
                else if (lcdoIAPAllocationDetail.fund_type == "L161")
                {
                    if ((lcdoIAPAllocationDetail.l161_special_account_amount + lcdoIAPAllocationDetail.forfeited_balance +
                          lcdoIAPAllocationDetail.late_alloc1_amount + lcdoIAPAllocationDetail.late_alloc2_amount +
                           lcdoIAPAllocationDetail.late_alloc3_amount + lcdoIAPAllocationDetail.late_alloc4_amount +
                           lcdoIAPAllocationDetail.late_alloc5_amount + lcdoIAPAllocationDetail.quaterly_allocations_amount +
                           lcdoIAPAllocationDetail.retirement_year_allocation2_amount + lcdoIAPAllocationDetail.retirement_year_allocation4_amount +
                           lcdoIAPAllocationDetail.payouts + lcdoIAPAllocationDetail.unallocable_amount) < 0)
                    {
                        lcdoIAPAllocationDetail.unallocable_amount = lcdoIAPAllocationDetail.unallocable_amount -
                            (lcdoIAPAllocationDetail.l161_special_account_amount + lcdoIAPAllocationDetail.forfeited_balance +
                          lcdoIAPAllocationDetail.late_alloc1_amount + lcdoIAPAllocationDetail.late_alloc2_amount +
                          lcdoIAPAllocationDetail.late_alloc3_amount + lcdoIAPAllocationDetail.late_alloc4_amount +
                          lcdoIAPAllocationDetail.late_alloc5_amount + lcdoIAPAllocationDetail.quaterly_allocations_amount +
                          lcdoIAPAllocationDetail.retirement_year_allocation2_amount + lcdoIAPAllocationDetail.retirement_year_allocation4_amount +
                          lcdoIAPAllocationDetail.payouts + lcdoIAPAllocationDetail.unallocable_amount);
                    }
                }

                if (lcdoIAPAllocationDetail.system_beginning_balance != Decimal.Zero || lcdoIAPAllocationDetail.total_iap_hours != Decimal.Zero || lcdoIAPAllocationDetail.total_ineligible_iap_late_hours != Decimal.Zero
                    || lcdoIAPAllocationDetail.unallocable_amount != Decimal.Zero || lcdoIAPAllocationDetail.retirement_year_allocation4_amount != Decimal.Zero || lcdoIAPAllocationDetail.retirement_year_allocation2_amount != Decimal.Zero
                    || lcdoIAPAllocationDetail.quaterly_allocations_amount != Decimal.Zero || lcdoIAPAllocationDetail.percentage_of_compensation_amount != Decimal.Zero || lcdoIAPAllocationDetail.payouts != Decimal.Zero
                    || lcdoIAPAllocationDetail.overlimit_contributions_amount != Decimal.Zero || lcdoIAPAllocationDetail.late_ineligible_hours != Decimal.Zero || lcdoIAPAllocationDetail.late_ineligible_hourly_contribution_amount != Decimal.Zero
                    || lcdoIAPAllocationDetail.late_inelgibile_compensation_amount != Decimal.Zero || lcdoIAPAllocationDetail.late_alloc5_amount != Decimal.Zero || lcdoIAPAllocationDetail.late_alloc4_amount != Decimal.Zero
                    || lcdoIAPAllocationDetail.late_alloc3_amount != Decimal.Zero || lcdoIAPAllocationDetail.late_alloc2_amount != Decimal.Zero || lcdoIAPAllocationDetail.late_alloc1_amount != Decimal.Zero
                    || lcdoIAPAllocationDetail.l52_special_account_amount != Decimal.Zero || lcdoIAPAllocationDetail.l161_special_account_amount != Decimal.Zero || lcdoIAPAllocationDetail.hourly_contribution_amount != Decimal.Zero
                    || lcdoIAPAllocationDetail.forfeited_balance != Decimal.Zero || lcdoIAPAllocationDetail.current_year_ineligible_hours != Decimal.Zero || lcdoIAPAllocationDetail.current_year_ineligible_contribution_amount != Decimal.Zero
                    || lcdoIAPAllocationDetail.current_year_inelgibile_compensation_amount != Decimal.Zero)
                {
                    lcdoIAPAllocationDetail.Insert();
                }

                aobjPassInfo.Commit();
            }

            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    String lstrMsg = "Error while Executing Batch,Error Message For Person Account ID : " + adrContribution[enmIapAllocationDetail.person_account_id.ToString()] + ":" + e.ToString();
                    PostErrorMessage(lstrMsg);
                    ldtbFailedPeople4IAPSnapShot.ImportRow(adrContribution);
                }
                aobjPassInfo.Rollback();
            }
        }

        private void InsertIntoOverlimtTable(int aintPersonAccountID, int aintEmpAccountNo, int aintComputationYear, decimal adecTotalIAPContb, decimal adecOverlimt)
        {
            cdoPersonAccountOverlimitContribution lcdoOverlimit = new cdoPersonAccountOverlimitContribution();
            lcdoOverlimit.person_account_id = aintPersonAccountID;
            lcdoOverlimit.emp_account_no = aintEmpAccountNo;
            lcdoOverlimit.computation_year = aintComputationYear;
            lcdoOverlimit.total_contribution_amount = adecTotalIAPContb;
            lcdoOverlimit.excess_contribution_amount = adecOverlimt;
            lcdoOverlimit.Insert();
        }
    }
}
