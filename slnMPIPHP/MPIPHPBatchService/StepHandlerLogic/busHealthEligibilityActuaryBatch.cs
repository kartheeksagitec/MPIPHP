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
    public class busHealthEligibilityActuaryBatch : busBatchHandler
    {
        #region Properties

        //public Collection<cdoDummyWorkData> iclbHealthWorkHistory { get; set; }
        public Collection<cdoPerson> lclbPerson { get; set; }
        decimal idecAge { get; set; }
        public busMainBase ibusBaseActivityInstance { get; set; }
        private object iobjLock = null;
        public busSystemManagement iobjSystemManagement { get; set; }
        #endregion
        public DataTable rptRetireeHealthEligibility { get; set; }

        #region HEALTH_ELIGIBILITY_ACTUARY_BATCH
        public DataTable idtPersonAccount { get; set; }
        //public DataTable idtbHealthWorkHistory4AllParticipants { get; set; }
        //PIR 1024
        public int iintYear { get; set; }

        //PIR 1024
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
                            case busConstant.JobParamBatchYear:
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty())
                                    iintYear = Convert.ToInt32(lobjParam.icdoJobParameters.param_value);
                                break;
                        }
                    }
                }
            }
        }

        public void ProcessHealthEligibilityActuaryBatch()
        {
            int lintCount = 0;
            int lintTotalCount = 0;

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            busBase lobjBase = new busBase();
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnection = utlLegacyDBConnetion.istrConnectionString;
            iobjLock = new object();

            //PIR 1024
            RetrieveBatchParameters();

            //Abhi-Health Following 2 things can be same as BIS batch
            //DataTable ldtPersonList = busBase.Select("cdoPerson.GetParticipantsForHealthEligibilityCheck", new object[0] { });
            DataTable ldtPersonList = busBase.Select("cdoPerson.GetAllHealthParticipantsMPIPP", new object[0]);

            //load all person accounts
            //idtPersonAccount = busBase.Select<cdoPersonAccount>(new string[0] { }, new object[0] { }, null, null);
            idtPersonAccount = busBase.Select("cdoDataExtractionBatchInfo.GetAllPersonAccount",
                                                       new object[1] { busConstant.FLAG_NO }); //PIR 832 //PIR 1052
            //Abhi-Health Above 2 things can be same as BIS batch

            if (ldtPersonList.Rows.Count > 0)
            {
                //PIR 1024
                //SqlParameter[] lParameters = new SqlParameter[1];
                //SqlParameter param1 = new SqlParameter("@Year", DbType.Int32);
                //param1.Value = iintYear;
                //lParameters[0] = param1;

                //idtbHealthWorkHistory4AllParticipants = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetHealthWorkHistoryForAllMpippParticipant", astrLegacyDBConnection, null, lParameters);

                //DataTable ldtHealthEligibilityActuaryDataTable = new DataTable();
                //CreateTempTabletoStoreInfo(ldtHealthEligibilityActuaryDataTable);

                #region Changes after F/w Upgrade
                IDbConnection lconHELegacy = null;
                if (lconHELegacy == null)
                {
                    lconHELegacy = DBFunction.GetDBConnection("Legacy");
                }
                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                if (astrLegacyDBConnection != null)
                {
                    IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
                    lobjParameter.ParameterName = "@Year";
                    lobjParameter.DbType = DbType.Int32;
                    lobjParameter.Value = iintYear;
                    lcolParameters.Add(lobjParameter);
                }
                DBFunction.DBExecuteProcedure("usp_GetHealthWorkHistoryForAllMpippParticipant", lcolParameters, lconHELegacy, null);
                lconHELegacy.Close();
                #endregion

                //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 2;  //RID# 107457 changed ProcessCount * 4 to 2. Too many thread casing exceptions
                Parallel.ForEach(ldtPersonList.AsEnumerable(), po, (acdoPerson, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "HealthEligibilityActuaryBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    // if (Convert.ToInt32(acdoPerson["person_id"]) == 13607)
                    //{
                    CalculateHealthEligibilityandCheckNewWorkingPeople(acdoPerson, lobjPassInfo, lintCount, lintTotalCount);

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;
                    // }
                });

                #region NO need of BULK insert - IT DOESN'T WORK unless you synchronize LOCKing ON datatable
                //utlConnection utlCoreDBConnection = HelperFunction.GetDBConnectionProperties("core");
                //string astrCoreDBConnection = utlCoreDBConnection.istrConnectionString;
                //SqlBulkCopy lSqlBulCopy = new SqlBulkCopy(astrCoreDBConnection);

                //lSqlBulCopy.DestinationTableName = "SGT_HEALTH_ELIGIBILTY_ACTUARY_DATA";
                //lSqlBulCopy.WriteToServer(ldtHealthEligibilityActuaryDataTable);

                //#region Execute update queries to update Date fields
                //DBFunction.DBNonQuery("cdoHealthEligibiltyActuaryData.UpdatePersonDateOfDeath",
                //              new object[] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);

                //DBFunction.DBNonQuery("cdoHealthEligibiltyActuaryData.UpdateRetirementDate",
                //              new object[] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);

                //#endregion 
                //lSqlBulCopy.Close();
                #endregion

                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;

            }
        }
        #endregion

        #region Calculate Health Eligibility and Check NewWorking People
        private void CalculateHealthEligibilityandCheckNewWorkingPeople(DataRow acdoPerson, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        {
            ArrayList aarrResult;
            Hashtable ahtbQueryBkmarks;
            // THere must some replacement for this in idict (the Connection String for Legacy)
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            lock (iobjLock)
            {
                aintCount++;
                aintTotalCount++;
                if (aintCount == 100)
                {
                    String lstrMsg = aintTotalCount + " : " + " Records Has Been Processed";
                    PostInfoMessage(lstrMsg);
                    aintCount = 0;
                }
            }

            //For Correspondance Purpose 
            aarrResult = new ArrayList();
            ahtbQueryBkmarks = new Hashtable();
            busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };

            autlPassInfo.BeginTransaction();
            try
            {
                lbusPersonOverview.icdoPerson.LoadData(acdoPerson);
                string lstrSSNDecrypted = Convert.ToString(acdoPerson[enmPerson.ssn.ToString()]);
                DataTable idtbHealthWorkHistory4AllParticipants = busBase.Select("cdoDataExtractionBatchInfo.WorkHistoryforHealthEligibilityParticipant", new object[1] { lstrSSNDecrypted });

                if (idtbHealthWorkHistory4AllParticipants.IsNotNull() && idtbHealthWorkHistory4AllParticipants.Rows.Count > 0 && acdoPerson[enmPerson.ssn.ToString()] != DBNull.Value)
                {
                    //string lstrSSNDecrypted = Convert.ToString(acdoPerson[enmPerson.ssn.ToString()]);
                    //DataRow[] ldrTempHealthInfo = idtbHealthWorkHistory4AllParticipants.FilterTable(utlDataType.String, "ssn", lstrSSNDecrypted);

                    //if (!ldrTempHealthInfo.IsNullOrEmpty())
                    //{
                    //lbusPersonOverview.iclbHealthWorkHistory = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldrTempHealthInfo);
                    //F/w Upgrade changes
                    lbusPersonOverview.iclbHealthWorkHistory = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(idtbHealthWorkHistory4AllParticipants);
                    lbusPersonOverview.iclbHealthWorkHistory = lbusPersonOverview.iclbHealthWorkHistory.Where(item => item.year > 0).ToList().ToCollection();
                    lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    //lbusPersonOverview.lbusBenefitApplication.LoadApproveRetirementApplication(lbusPersonOverview.icdoPerson.person_id);
                    lbusPersonOverview.lbusBenefitApplication.LoadApproveRetirementApplicationForHealthActuary(lbusPersonOverview.icdoPerson.person_id); //Sid Jain 06152013
                    lbusPersonOverview.lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson = lbusPersonOverview.icdoPerson;
                    DataRow[] ldrPersonAccount = idtPersonAccount.FilterTable(utlDataType.Numeric, "person_id", lbusPersonOverview.icdoPerson.person_id);

                    if (!ldrPersonAccount.IsNullOrEmpty() && ldrPersonAccount.Count() > 0)
                    {
                        lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                        busBase lobjBase = new busBase();
                        lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount = lobjBase.GetCollection<busPersonAccount>(ldrPersonAccount, "icdoPersonAccount");
                        lbusPersonOverview.iobjSystemManagement = iobjSystemManagement;
                        lbusPersonOverview.iblnFromBatch = true;
                        lbusPersonOverview.idecAge = busGlobalFunctions.CalculatePersonAge(lbusPersonOverview.icdoPerson.date_of_birth, DateTime.Now);

                        lbusPersonOverview.lbusBenefitApplication.ProcessWorkHistoryforBISandForfieture(lbusPersonOverview.iclbHealthWorkHistory, string.Empty);
                        lbusPersonOverview.CheckRetireeHealthEligibilityAndUpdateFlag();

                        //iclbHealthWorkHistory = lbusPersonOverview.iclbHealthWorkHistory;

                        #region Third Part of Batch (Inserting values into SGT_HEALTH_ELIGIBILTY_ACTUARY_DATA table)

                        int lintPersonID = 0, lintPlanYear = 0, lintQualifiedYrsTillBatchDt = 0;
                        string lstrPersonSSN = string.Empty, lstrPersonFirstName = string.Empty, lstrPersonLastName = string.Empty,
                               lstrRetrHlthElgblFlg = string.Empty, lstrEligibleRule = string.Empty, lstrEnrldInLocalPlanFlg = string.Empty;
                        DateTime ldtPersonDOD = new DateTime();
                        DateTime ldtRetirementDate = new DateTime();
                        string lstrRetirementType = string.Empty;//PIR 508
                        DateTime ldtPersonDOB = new DateTime();
                        decimal ldecQualifiedHrsTillBatchDt = 0.0M, ldecCurrentHrs = 0.0M, ldecPriorHrs = 0.0M;

                        lintPersonID = lbusPersonOverview.icdoPerson.person_id;
                        lstrPersonSSN = lbusPersonOverview.icdoPerson.ssn;

                        //PIR 1024
                        lintPlanYear = iintYear;//iobjSystemManagement.icdoSystemManagement.batch_date.Year;
                        lstrPersonFirstName = lbusPersonOverview.icdoPerson.first_name;
                        lstrPersonLastName = lbusPersonOverview.icdoPerson.last_name;
                        ldtPersonDOB = lbusPersonOverview.icdoPerson.date_of_birth;

                        if (lbusPersonOverview.icdoPerson.date_of_death != null && lbusPersonOverview.icdoPerson.date_of_death != DateTime.MinValue)
                        {
                            ldtPersonDOD = lbusPersonOverview.icdoPerson.date_of_death;
                        }

                        if (lbusPersonOverview.lbusBenefitApplication != null && lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date != DateTime.MinValue
                            && lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.benefit_type_value != busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                        {
                            ldtRetirementDate = lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date;
                            lstrRetirementType = lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.benefit_type_description; //PIR 508
                        }

                        if (!lbusPersonOverview.iclbHealthWorkHistory.IsNullOrEmpty())
                        {

                            lintQualifiedYrsTillBatchDt = lbusPersonOverview.iclbHealthWorkHistory.Last().iintHealthCount;

                            ldecQualifiedHrsTillBatchDt = (from item in lbusPersonOverview.iclbHealthWorkHistory select item.idecTotalHealthHours).Sum();

                            if (lbusPersonOverview.iclbHealthWorkHistory.Where(item => Convert.ToInt32(item.year) == lintPlanYear).Count() > 0)
                            {
                                ldecCurrentHrs = lbusPersonOverview.iclbHealthWorkHistory.Where(item => Convert.ToInt32(item.year) == lintPlanYear).FirstOrDefault().idecTotalHealthHours;
                            }

                            if (lbusPersonOverview.iclbHealthWorkHistory.Where(item => Convert.ToInt32(item.year) == (lintPlanYear - 1)).Count() > 0)
                                ldecPriorHrs = lbusPersonOverview.iclbHealthWorkHistory.Where(item => Convert.ToInt32(item.year) == (lintPlanYear - 1)).FirstOrDefault().idecTotalHealthHours;
                        }

                        if (lbusPersonOverview.icdoPerson.health_eligible_flag != null)
                        {
                            lstrRetrHlthElgblFlg = lbusPersonOverview.icdoPerson.health_eligible_flag;
                        }

                        if (lstrRetrHlthElgblFlg == busConstant.FLAG_YES)
                        {
                            if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_1)
                            {
                                lstrEligibleRule = "15Y/20K";
                            }
                            else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_2)
                            {
                                lstrEligibleRule = "20Y/20K";
                            }
                            else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_3)
                            {
                                lstrEligibleRule = "30Y/55K";
                            }
                            else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_4)
                            {
                                lstrEligibleRule = "30Y/60K";
                            }
                            else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_5)
                            {
                                lstrEligibleRule = "10Y/10K";
                            }
                        }

                        if (lbusPersonOverview.lbusBenefitApplication != null && !(lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID ||
                                                                                                              item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID ||
                                                                                                              item.icdoPersonAccount.plan_id == busConstant.LOCAL_600_PLAN_ID ||
                                                                                                              item.icdoPersonAccount.plan_id == busConstant.LOCAL_666_PLAN_ID ||
                                                                                                              item.icdoPersonAccount.plan_id == busConstant.LOCAL_700_PLAN_ID)).IsNullOrEmpty())
                        {
                            lstrEnrldInLocalPlanFlg = busConstant.FLAG_YES;
                        }
                        else
                        {
                            lstrEnrldInLocalPlanFlg = busConstant.FLAG_NO;
                        }

                        //lock (iobjLock)
                        //{
                        //Framework Upgrade Fixes to process batch successfully
                        cdoHealthEligibiltyActuaryData lcdoHealthEligibiltyActuaryData = new cdoHealthEligibiltyActuaryData();
                        lcdoHealthEligibiltyActuaryData.person_id = lintPersonID;
                        lcdoHealthEligibiltyActuaryData.person_ssn = lstrPersonSSN;
                        lcdoHealthEligibiltyActuaryData.plan_year = lintPlanYear;
                        lcdoHealthEligibiltyActuaryData.person_first_name = lstrPersonFirstName;
                        lcdoHealthEligibiltyActuaryData.person_last_name = lstrPersonLastName;
                        lcdoHealthEligibiltyActuaryData.person_dob = ldtPersonDOB;
                        lcdoHealthEligibiltyActuaryData.person_dod = ldtPersonDOD;
                        lcdoHealthEligibiltyActuaryData.retirement_date = ldtRetirementDate;
                        lcdoHealthEligibiltyActuaryData.qualified_years_till_batch_date = lintQualifiedYrsTillBatchDt;
                        lcdoHealthEligibiltyActuaryData.qualified_hours_till_batch_date = ldecQualifiedHrsTillBatchDt;
                        lcdoHealthEligibiltyActuaryData.current_hours = ldecCurrentHrs;
                        lcdoHealthEligibiltyActuaryData.prior_hours = ldecPriorHrs;
                        lcdoHealthEligibiltyActuaryData.ret_health_elig_flag = lstrRetrHlthElgblFlg;
                        lcdoHealthEligibiltyActuaryData.eligible_rule = lstrEligibleRule;
                        lcdoHealthEligibiltyActuaryData.enrolled_in_local_plan_flag = lstrEnrldInLocalPlanFlg;
                        lcdoHealthEligibiltyActuaryData.retirement_type = lstrRetirementType;//PIR 508
                        lcdoHealthEligibiltyActuaryData.created_by = iobjPassInfo.istrUserID;
                        lcdoHealthEligibiltyActuaryData.modified_by = iobjPassInfo.istrUserID;
                        lcdoHealthEligibiltyActuaryData.created_date = DateTime.Now;
                        lcdoHealthEligibiltyActuaryData.modified_date = DateTime.Now;
                        lcdoHealthEligibiltyActuaryData.update_seq = 0;
                        //lcdoHealthEligibiltyActuaryData.LoadData(AddRowsInTempTable(adtHealthEligibilityActuaryDataTable, lintPersonID, lstrPersonSSN, lintPlanYear, lstrPersonFirstName, lstrPersonLastName, ldtPersonDOB, ldtPersonDOD, ldtRetirementDate,
                        //                   lintQualifiedYrsTillBatchDt, ldecQualifiedHrsTillBatchDt, ldecCurrentHrs, ldecPriorHrs, lstrRetrHlthElgblFlg, lstrEligibleRule, lstrEnrldInLocalPlanFlg, lstrRetirementType));//PIR 508
                        lcdoHealthEligibiltyActuaryData.Insert();
                        //}
                        #endregion
                    }
                    //}
                }

                autlPassInfo.Commit();
            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    String lstrMsg = "Error while Executing Batch,Error Message For MPID " + lbusPersonOverview.icdoPerson.mpi_person_id + ":" + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();
            }
        }
        #endregion

        #region Commneted after f/w upgrade change - not required
        //private DataTable CreateTempTabletoStoreInfo(DataTable adtHealthEligibilityActuaryDataTable)
        //{
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.health_actuary_data_id.ToString(), typeof(int));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.person_id.ToString(), typeof(int));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.person_ssn.ToString(), typeof(string));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.plan_year.ToString(), typeof(int));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.person_first_name.ToString(), typeof(string));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.person_last_name.ToString(), typeof(string));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.person_dob.ToString(), typeof(DateTime));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.person_dod.ToString(), typeof(DateTime));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.retirement_date.ToString(), typeof(DateTime));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.qualified_years_till_batch_date.ToString(), typeof(int));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.qualified_hours_till_batch_date.ToString(), typeof(decimal));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.current_hours.ToString(), typeof(decimal));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.prior_hours.ToString(), typeof(decimal));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.ret_health_elig_flag.ToString(), typeof(string));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.eligible_rule.ToString(), typeof(string));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.enrolled_in_local_plan_flag.ToString(), typeof(string));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add("MODIFIED_BY", typeof(string));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add("MODIFIED_DATE", typeof(DateTime));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add("CREATED_BY", typeof(string));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add("CREATED_DATE", typeof(DateTime));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add("UPDATE_SEQ", typeof(int));
        //    adtHealthEligibilityActuaryDataTable.Columns.Add(enmHealthEligibiltyActuaryData.retirement_type.ToString(), typeof(string)); //PIR 508

        //    return adtHealthEligibilityActuaryDataTable;
        //}

        //private DataRow AddRowsInTempTable(DataTable adtHealthEligibilityActuaryDataTable, int aintPersonID, string astrPersonSSN, int aintPlanYear, string astrPersonFirstName, string astrPersonLastName,
        //                                DateTime adtPersonDOB, DateTime adtPersonDOD, DateTime adtRetirementDate, int aintQualifiedYrsTillBatchDt, decimal adecQualifiedHrsTillBatchDt, decimal adecCurrentHrs,
        //                                decimal adecPriorHrs, string astrRetrHlthElgblFlg, string astrHealthEligbileRule, string astrEnrldInLocalPlanFlg, string astrRetirementType)//PIR 508
        //{

        //    DataRow ldrTempTableDataRow = adtHealthEligibilityActuaryDataTable.NewRow();

        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.person_id.ToString()] = aintPersonID;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.person_ssn.ToString()] = astrPersonSSN;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.plan_year.ToString()] = aintPlanYear;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.person_first_name.ToString()] = astrPersonFirstName;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.person_last_name.ToString()] = astrPersonLastName;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.person_dob.ToString()] = adtPersonDOB;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.person_dod.ToString()] = adtPersonDOD;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.retirement_date.ToString()] = adtRetirementDate;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.qualified_years_till_batch_date.ToString()] = aintQualifiedYrsTillBatchDt;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.qualified_hours_till_batch_date.ToString()] = adecQualifiedHrsTillBatchDt;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.current_hours.ToString()] = adecCurrentHrs;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.prior_hours.ToString()] = adecPriorHrs;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.ret_health_elig_flag.ToString()] = astrRetrHlthElgblFlg;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.eligible_rule.ToString()] = astrHealthEligbileRule;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.enrolled_in_local_plan_flag.ToString()] = astrEnrldInLocalPlanFlg;
        //    ldrTempTableDataRow[enmHealthEligibiltyActuaryData.retirement_type.ToString()] = astrRetirementType;//PIR 508

        //    ldrTempTableDataRow["created_by"] = iobjPassInfo.istrUserID;
        //    ldrTempTableDataRow["modified_by"] = iobjPassInfo.istrUserID;
        //    ldrTempTableDataRow["created_date"] = DateTime.Now;
        //    ldrTempTableDataRow["modified_date"] = DateTime.Now;
        //    ldrTempTableDataRow["update_seq"] = 0;

        //    return ldrTempTableDataRow;
        //    //adtHealthEligibilityActuaryDataTable.Rows.Add(ldrTempTableDataRow);
        //}
        #endregion

        #region Retiree Health Eligibility Report
        public void RetireeHealthEligibilityReport(bool thirtyDayReport)
        {

            int lintCount = 0;
            int lintTotalCount = 0;
            if(thirtyDayReport == false)
            {
                createTableDesignForRetireeHealthEligibility();
            }
            else
            {
                createTableDesignForRetireeHealthEligibility30Day();
            }
            
            DataSet idtstRetireeHealthEligibility = new DataSet();
            DataSet idtstRetireeHealthEligibility30Day = new DataSet();

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();

            DataTable ldtbReportData;
            if (!thirtyDayReport)
            {
               ldtbReportData = busBase.Select("cdoPayeeAccount.GetRetireeEligibilityList", new object[1] { busPayeeAccountHelper.GetLastBenefitPaymentDate(busConstant.MPIPP_PLAN_ID) });
               
            }
            else
            {
                ldtbReportData = busBase.Select("cdoCorTracking.LoadHealthEligibilityForNextMonth", new object[0] { });
            }         

            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 1;//System.Environment.ProcessorCount * 4;

            Parallel.ForEach(ldtbReportData.AsEnumerable(), po, (acdoPerson, loopState) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "PensionEligbilityBatch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;

                if (thirtyDayReport == false)
                {
                    createRetireeHealthEligibilityRecordset(acdoPerson, lobjPassInfo, lintCount, lintTotalCount, false);
                }
                else
                {
                    createRetireeHealthEligibilityRecordset(acdoPerson, lobjPassInfo, lintCount, lintTotalCount, true);
                }

                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;


            });

            if (thirtyDayReport == false)
            {
                idtstRetireeHealthEligibility.Tables.Add(rptRetireeHealthEligibility.Copy());
                idtstRetireeHealthEligibility.Tables[0].TableName = "ReportTable01";
                idtstRetireeHealthEligibility.DataSetName = "ReportTable01";
                if (idtstRetireeHealthEligibility.Tables[0].Rows.Count > 0)
                {
                    this.CreatePDFReport(idtstRetireeHealthEligibility, "rptRetireeHealthEligibilityList", busConstant.MPIPHPBatch.GENERATED_RITIREE_HEALTH_ELIGIBILITY_REPORT_PATH);

                }

                string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_RETIREE_HEALTH_ELIGIBLE + ".xlsx";
                string lstrRetireeHealthEligibilityListReportPath = "";
                lstrRetireeHealthEligibilityListReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_RITIREE_HEALTH_ELIGIBILITY_REPORT_PATH) + busConstant.REPORT_RETIREE_HEALTH_ELIGIBLE + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
                busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrRetireeHealthEligibilityListReportPath, "ReportTable01", idtstRetireeHealthEligibility);
            }
            else
            {
                idtstRetireeHealthEligibility30Day.Tables.Add(rptRetireeHealthEligibility.Copy());
                idtstRetireeHealthEligibility30Day.Tables[0].TableName = "ReportTable01";
                idtstRetireeHealthEligibility30Day.DataSetName = "ReportTable01";

                string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_RETIREE_HEALTH_ELIGIBLE_30_DAY + ".xlsx";
                string lstrRetireeHealthEligibilityListReportPath = "";
                lstrRetireeHealthEligibilityListReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_RITIREE_HEALTH_ELIGIBILITY_REPORT_PATH_30_DAY) + busConstant.REPORT_RETIREE_HEALTH_ELIGIBLE_30_DAY + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
                busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrRetireeHealthEligibilityListReportPath, "ReportTable01", idtstRetireeHealthEligibility30Day);
            }            

            lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        }


        private void createRetireeHealthEligibilityRecordset(DataRow acdoPerson, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount, bool thirtyDayReport)
        {
            string lstrEligibleRule = string.Empty, lstrRetrHlthElgblFlg = string.Empty;
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            busBase lbusBase = new busBase();

            lock (iobjLock)
            {
                aintCount++;
                aintTotalCount++;
                if (aintCount == 100)
                {
                    String lstrMsg = aintTotalCount + " : " + " Records Has Been Processed";
                    // PostInfoMessage(lstrMsg);
                    aintCount = 0;
                }
            }

            busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
            //  lbusPersonOverview.icdoPerson.LoadData(acdoPerson);
            //   lbusPersonOverview.icdoPerson.FindPerson(Convert.ToInt32(acdoPerson["PERSON_ID"]));
            lbusPersonOverview.FindPerson(Convert.ToInt32(acdoPerson["PERSON_ID"]));
            lbusPersonOverview.LoadPersonAddresss();
            lbusPersonOverview.LoadPersonContacts();
            lbusPersonOverview.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusPersonOverview.icdoPerson.idtDateofBirth, DateTime.Now);
            lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusPersonOverview.lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            //lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(iobjSystemManagement.icdoSystemManagement.batch_date).AddDays(1);
            lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson = lbusPersonOverview.icdoPerson;
            lbusPersonOverview.lbusBenefitApplication.ibusPerson.LoadPersonAccounts();
            lbusPersonOverview.lbusBenefitApplication.ibusPerson.LoadBenefitApplication();
            lbusPersonOverview.lbusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
           

            int leligilibiltyRule = 0;
            int agediff = 0;
            var lretirementDate = acdoPerson["RETIREMENT_DATE"];
           
            DateTime ldtEffectivedate = Convert.ToDateTime(lretirementDate).Date;
            decimal ldecAgeAsOfRetirement = Math.Floor(busGlobalFunctions.CalculatePersonAgeInDec(lbusPersonOverview.icdoPerson.idtDateofBirth, Convert.ToDateTime(lretirementDate)));
            //Ticket#96556
            // lbusPersonOverview.GetRetireeHealthHours();
            lbusPersonOverview.iclbHealthWorkHistory = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI;
            lbusPersonOverview.CheckRetireeHealthEligibilityAndUpdateFlag();

            var lvarHealthHours = (from item in lbusPersonOverview.iclbHealthWorkHistory select item.idecTotalHealthHours).Sum();
            var lvarHealthQualifiedYears = (from item in lbusPersonOverview.iclbHealthWorkHistory orderby item.year select item.iintHealthCount).Last();

            if (lbusPersonOverview.icdoPerson.health_eligible_flag != null)
            {
                lstrRetrHlthElgblFlg = lbusPersonOverview.icdoPerson.health_eligible_flag;
            }

            var ldtentitlementDate = acdoPerson["SSADATE"];

            if (lstrRetrHlthElgblFlg == busConstant.FLAG_YES)
            {
                if(acdoPerson["SSADATE"].ToString() !="")
                {
                    lstrEligibleRule = "10Y/10K@Dis";

                }
                else
                {
                    if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_1)
                    {
                        leligilibiltyRule = 62;
                        if (ldecAgeAsOfRetirement < leligilibiltyRule)
                        {
                            if (lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule).Day != 1)
                                ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule).GetLastDayofMonth().AddDays(1);
                            else
                                ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule);
                        }
                        lstrEligibleRule = "15Y/20K@62";
                    }
                    else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_2)
                    {
                        leligilibiltyRule = 62;
                        if (ldecAgeAsOfRetirement < leligilibiltyRule)
                        {
                            if (lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule).Day != 1)
                                ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule).GetLastDayofMonth().AddDays(1);
                            else
                                ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule);
                        }
                        lstrEligibleRule = "20Y/20K@62";
                    }
                    else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_3)
                    {
                        leligilibiltyRule = 61;
                        if (ldecAgeAsOfRetirement < leligilibiltyRule)
                        {
                            if (lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule).Day != 1)
                                ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule).GetLastDayofMonth().AddDays(1);
                            else
                                ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule);
                        }
                        lstrEligibleRule = "30Y/55K@61";
                    }
                    else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_4)
                    {
                        leligilibiltyRule = 60;
                        if (ldecAgeAsOfRetirement < leligilibiltyRule)
                        {
                            if (lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule).Day != 1)
                                ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule).GetLastDayofMonth().AddDays(1);
                            else
                                ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyRule);
                        }
                        lstrEligibleRule = "30Y/60K@60";
                    }
                    else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_5)
                    {
                        lstrEligibleRule = "10Y/10K@Dis";
                    }

                }
                
            }


            DataRow dr = rptRetireeHealthEligibility.NewRow();

            dr["MPID"] = acdoPerson["MPI_PERSON_ID"];
            dr["Name"] = acdoPerson["NAME"];
            dr["Address"] = acdoPerson["Address"];
            dr["DateofBirth"] = acdoPerson["DATE_OF_BIRTH"];
            dr["RetirementDate"] = acdoPerson["RETIREMENT_DATE"];
            dr["PaymentDate"] = acdoPerson["PAYMENTDATE"];
            dr["AgeatRetirement"] = Math.Round(ldecAgeAsOfRetirement);
            dr["UnionCode"] = acdoPerson["UNIONCODE"].ToString();
            dr["QualifiedYears"] = lvarHealthQualifiedYears;
            dr["HealthHours"] = lvarHealthHours;
            dr["EligibilityBasedOn"] = lstrEligibleRule.ToString();
            dr["SSADate"] = ldtentitlementDate;
            dr["EffectiveDate"] = ldtEffectivedate;
            //Ticket#76267
            dr["HealthEligible"] = acdoPerson["RETIREMENT_HEALTH_ONLY"];
            dr["MPIPERSONID"] = acdoPerson["MPIPERSONID"];
            dr["LASTNAME"] = acdoPerson["LASTNAME"];
            dr["FIRSTNAME"] = acdoPerson["FIRSTNAME"];
           dr["Address1"] = acdoPerson["ADDR_LINE_1"];
            dr["Address2"] = acdoPerson["ADDR_LINE_2"];
            dr["City"] = acdoPerson["ADDR_CITY"];
            dr["State"] = acdoPerson["ADDR_STATE_VALUE"];
            dr["ZipCode"] = acdoPerson["ADDR_ZIP_CODE"];
            if (thirtyDayReport)
            {
                dr["LateAdditionDate"] = acdoPerson["LATE_ADDITION_DATE"];
                dr["LateCancellationDate"] = acdoPerson["LATE_CANCELLATION_DATE"];
            }
            
            rptRetireeHealthEligibility.Rows.Add(dr);
        }

        public DataTable createTableDesignForRetireeHealthEligibility()
        {
            rptRetireeHealthEligibility = new DataTable();
            rptRetireeHealthEligibility.Columns.Add("Name", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("MPID", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("Address", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("DateofBirth", typeof(DateTime));
            rptRetireeHealthEligibility.Columns.Add("RetirementDate", typeof(DateTime));
            rptRetireeHealthEligibility.Columns.Add("PaymentDate", typeof(DateTime));
            rptRetireeHealthEligibility.Columns.Add("AgeatRetirement", typeof(decimal));
            rptRetireeHealthEligibility.Columns.Add("UnionCode", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("QualifiedYears", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("HealthHours", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("EligibilityBasedOn", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("SSADate", typeof(DateTime));
            rptRetireeHealthEligibility.Columns.Add("EffectiveDate", typeof(DateTime));
            rptRetireeHealthEligibility.Columns.Add("HealthEligible", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("MPIPERSONID", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("LASTNAME", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("FIRSTNAME", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("Address1", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("Address2", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("City", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("State", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("ZipCode", typeof(string));

            



            return rptRetireeHealthEligibility;
        }

        public DataTable createTableDesignForRetireeHealthEligibility30Day()
        {
            rptRetireeHealthEligibility = new DataTable();
            rptRetireeHealthEligibility.Columns.Add("Name", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("MPID", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("Address", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("DateofBirth", typeof(DateTime));
            rptRetireeHealthEligibility.Columns.Add("RetirementDate", typeof(DateTime));
            rptRetireeHealthEligibility.Columns.Add("PaymentDate", typeof(DateTime));
            rptRetireeHealthEligibility.Columns.Add("AgeatRetirement", typeof(decimal));
            rptRetireeHealthEligibility.Columns.Add("UnionCode", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("QualifiedYears", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("HealthHours", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("EligibilityBasedOn", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("SSADate", typeof(DateTime));
            rptRetireeHealthEligibility.Columns.Add("EffectiveDate", typeof(DateTime));
            rptRetireeHealthEligibility.Columns.Add("HealthEligible", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("MPIPERSONID", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("LASTNAME", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("FIRSTNAME", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("Address1", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("Address2", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("City", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("State", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("ZipCode", typeof(string));
            rptRetireeHealthEligibility.Columns.Add("LateAdditionDate", typeof(DateTime));
            rptRetireeHealthEligibility.Columns.Add("LateCancellationDate", typeof(DateTime));

            return rptRetireeHealthEligibility;
        }

        #endregion

    }
}
