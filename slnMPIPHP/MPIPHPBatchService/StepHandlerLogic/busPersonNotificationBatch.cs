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
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace MPIPHPJobService
{
    public class busPersonNotificationBatch : busBatchHandler 
    {

        #region Properties
        public Collection<cdoDummyWorkData> iclbHealthWorkHistory { get; set; }
        public Collection<cdoPerson> lclbPerson { get; set; }
        decimal idecAge { get; set; }
        public busMainBase ibusBaseActivityInstance { get; set; }
        private object iobjLock = null;
        //int lintCount = 0;
        //int lintTotalCount = 0;
        public int PAYEE_ACCOUNT_ID { get; set; }
        
        private DateTime idtNextMDDate = new DateTime(System.DateTime.Now.Year, 04, 01);

        #endregion

        #region busPerson Notification Batch
        public busPersonNotificationBatch()
        {
        }
        #endregion

        #region PreNotification Break In Service batch (with ParallelOptions)
        public void PreNotificationBreakInServiceBatch()
        {
            int lintCount = 0;
            int lintTotalCount = 0;

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnection = utlLegacyDBConnetion.istrConnectionString;

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            SqlParameter[] lParameters = new SqlParameter[2];
            SqlParameter param1 = new SqlParameter("@Year", DbType.Int32);
            SqlParameter param2 = new SqlParameter("@ISBIS", DbType.String);
            if (this.iobjSystemManagement.IsNotNull())
                param1.Value = this.iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1;
            else
                param1.Value = DateTime.Now.Year - 1;
            lParameters[0] = param1;

            param2.Value = busConstant.FLAG_NO;
            lParameters[1] = param2;

            //PROD PIR 845
            this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;

            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();

            DataTable ldtPersonList = busBase.Select("cdoPerson.GetActiveParticiapantsMPIPPPreBIS", new object[0]);

            if (ldtPersonList != null && ldtPersonList.Rows.Count > 0)
            {
                createTableDesignForBreakInServiceReport();
                ldtbPersonAccounts = busBase.Select("cdoPerson.GetAllParticipantPersonAccounts", new object[0]);
                ldtbWorkInformationForAllParticipants = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkHistoryForAllMpippParticipant", astrLegacyDBConnection, null, lParameters);
                //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

                Parallel.ForEach(ldtPersonList.AsEnumerable(), po, (acdoPerson, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "PreNotificationBreakInServiceBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    //PIR 850
                    string lstrSSNDecrypted = Convert.ToString(acdoPerson[enmPerson.ssn.ToString()]);
                    DataRow[] ldrTempWorkInfo = ldtbWorkInformationForAllParticipants.FilterTable(utlDataType.String, "ssn", lstrSSNDecrypted);

                    if (!ldrTempWorkInfo.IsNullOrEmpty())
                    {
                        PreNotificationBreakInServiceProcess(acdoPerson, lobjPassInfo, lintCount, lintTotalCount);
                    }

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;
                });

                utlPassInfo.iobjPassInfo = lobjMainPassInfo;

                //BIS Report
                if (idtbBreakInServiceRecords.IsNotNull() && idtbBreakInServiceRecords.Rows.Count > 0)
                {
                    try
                    {
                        idtbBreakInServiceRecords.TableName = "ReportTable01";
                        CreatePDFReport(idtbBreakInServiceRecords, "rpt_PreNotificationBISBatchReport", busConstant.Report_Path);  //PIR 978

                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                    }
                }
                //PROD PIR 845
                MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Correspondence_Path, iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path);
            }
        }
        #endregion

        #region PreNotification Break In Service Batch

        public void PreNotificationBreakInServiceProcess(DataRow acdoPerson, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        {
            ArrayList aarrResult;
            Hashtable ahtbQueryBkmarks;
            busBase lobjBase = new busBase();
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            lock (iobjLock)
            {
                aintCount++;
                aintTotalCount++;
                if (aintCount == 100)
                {
                    string lstrMsg = aintTotalCount + " : " + " Records Has Been Processed";
                    PostInfoMessage(lstrMsg);
                    aintCount = 0;
                }
            }

            autlPassInfo.BeginTransaction();

            busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
            lbusPersonOverview.icdoPerson.LoadData(acdoPerson);          //need to fill busPersonOverview object in future

            try
            {

                aarrResult = new ArrayList();
                ahtbQueryBkmarks = new Hashtable();
                lock (iobjLock)
                {

                    if (ldtbWorkInformationForAllParticipants.Rows.Count > 0 && ldtbWorkInformationForAllParticipants.IsNotNull() && acdoPerson[enmPerson.ssn.ToString()] != DBNull.Value)
                    {

                        string lstrSSNDecrypted = Convert.ToString(acdoPerson[enmPerson.ssn.ToString()]);
                        DataRow[] ldrTempWorkInfo = ldtbWorkInformationForAllParticipants.FilterTable(utlDataType.String, "ssn", lstrSSNDecrypted);

                        if (!ldrTempWorkInfo.IsNullOrEmpty())
                        {


                            lbusPersonOverview.LoadPersonAddresss();
                            lbusPersonOverview.LoadPersonContacts();
                            lbusPersonOverview.LoadCorrAddress();

                            lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                            lbusPersonOverview.lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                            lbusPersonOverview.lbusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusPersonOverview.icdoPerson.idtDateofBirth, iobjSystemManagement.icdoSystemManagement.batch_date);
                            lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson = lbusPersonOverview.icdoPerson;

                            DataRow[] ldrPersonAccounts = ldtbPersonAccounts.FilterTable(utlDataType.String, "person_id", lbusPersonOverview.icdoPerson.person_id.ToString());
                            busPersonAccount lbusPersonAccount = new busPersonAccount();
                            lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount = lbusPersonAccount.GetCollection<busPersonAccount>(ldrPersonAccounts, "icdoPersonAccount");


                            lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldrTempWorkInfo);
                            lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP = new Collection<cdoDummyWorkData>();

                            if (lbusPersonOverview.icdoPerson.date_of_death.IsNotNull() && lbusPersonOverview.icdoPerson.date_of_death != DateTime.MinValue)
                                lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date = lbusPersonOverview.icdoPerson.date_of_death;
                            else
                                lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date = DateTime.Now;

                            lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusPersonOverview.lbusBenefitApplication.PaddingForBridgingService(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI);
                            lbusPersonOverview.lbusBenefitApplication.ProcessWorkHistoryPadding(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI, busConstant.MPIPP);
                            lbusPersonOverview.lbusBenefitApplication.ProcessWorkHistoryforBISandForfieture(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI, string.Empty);

                            if ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()) &&
                            lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count() > 0 &&
                            lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year == DateTime.Now.Year)
                            {
                                DateTime SatBeforeLastThrusday = busGlobalFunctions.GetLastDateOfComputationYear(DateTime.Now.Year);
                                if (DateTime.Now <= SatBeforeLastThrusday)
                                {
                                    lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.RemoveAt(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.
                                        IndexOf(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last()));
                                }
                            }

                            aarrResult.Add(lbusPersonOverview);

                            busPersonBatchFlags lbusPersonBatchFlags = new busPersonBatchFlags { icdoPersonBatchFlags = new cdoPersonBatchFlags() };
                            DataTable ldtPersonBatchFlag = busBase.Select("cdoPersonBatchFlags.GetBatchFlagsbyPersonID", new object[1] { lbusPersonOverview.icdoPerson.person_id });

                            if ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()) && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_hours < 200
                                  && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().bis_years_count == 1
                                  && (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrBisParticipantFlag.IsNull() || lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrBisParticipantFlag != busConstant.FLAG_YES)
                                  && (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrForfietureFlag.IsNull() || lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrForfietureFlag != busConstant.FLAG_YES)
                                    && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count > 0)
                            {
                                if (ldtPersonBatchFlag.Rows.Count > 0)
                                {
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.LoadData(ldtPersonBatchFlag.Rows[0]);
                                    if ((string.IsNullOrEmpty(lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag) ||
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag == busConstant.FLAG_NO) && (lbusPersonOverview.icdoPerson.IS_VALID_ADDRESS_PRESENT == busConstant.FLAG_YES))
                                    {
                                        //GENERATE CORR CODE
                                        string str = this.CreateCorrespondence(busConstant.ONE_YEAR_BREAK_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE); //PROD PIR 845
                                        //IF CORR IS SUCCESSFULL

                                    }

                                    lock (iobjLock)
                                    {
                                        //BIS Report
                                        #region Insert Records for Reports
                                        DataRow ldrNewRow = idtbBreakInServiceRecords.NewRow();
                                        ldrNewRow["MPI_PERSON_ID"] = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
                                        //ldrNewRow["FIRST_NAME"] = Convert.ToString(acdoPerson["FIRST_NAME"]);
                                        //ldrNewRow["LAST_NAME"] = Convert.ToString(acdoPerson["LAST_NAME"]);
                                        ldrNewRow["PARTICIPANT_NAME"] = lbusPersonOverview.icdoPerson.first_name + " " + lbusPersonOverview.icdoPerson.last_name;
                                        //ldrNewRow["RETIREMENT_DATE"] = Convert.ToDateTime(acdoPerson["RETIREMENT_DATE"]);
                                        ldrNewRow["QUALIFIED_YEAR"] = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                        ldrNewRow["BIS_FLAG"] = lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag;
                                        ldrNewRow["DOB"] = lbusPersonOverview.icdoPerson.date_of_birth;
                                        ldrNewRow["DOD"] = lbusPersonOverview.icdoPerson.date_of_death == DateTime.MinValue ? "" : Convert.ToString(lbusPersonOverview.icdoPerson.date_of_death);
                                        ldrNewRow["VALID_ADDRESS_PRESENT"] = lbusPersonOverview.icdoPerson.IS_VALID_ADDRESS_PRESENT;
                                        if ((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                             where obj.year == DateTime.Now.Year - 1
                                             orderby obj.year descending
                                             select obj.qualified_hours).Count() > 0)
                                        {

                                            ldrNewRow["LAST_YEAR_HOURS"] = Convert.ToDecimal((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                              where obj.year == DateTime.Now.Year - 1
                                                                                              orderby obj.year descending
                                                                                              select obj.qualified_hours).FirstOrDefault());

                                            ldrNewRow["SECOND_LAST_YEAR_HOURS"] = Convert.ToDecimal((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                                     where obj.year == DateTime.Now.Year - 2
                                                                                                     orderby obj.year descending
                                                                                                     select obj.qualified_hours).FirstOrDefault());
                                        }

                                        //ldrNewRow["PAYEE_ACCOUNT_ID"] = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                                        idtbBreakInServiceRecords.Rows.Add(ldrNewRow);
                                        #endregion
                                    }

                                    lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag = busConstant.FLAG_YES;
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.Update();
                                }
                                else
                                {
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.person_id = lbusPersonOverview.icdoPerson.person_id;
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag = busConstant.FLAG_YES;
                                    //GENERATE CORR CODE
                                    if (lbusPersonOverview.icdoPerson.IS_VALID_ADDRESS_PRESENT == busConstant.FLAG_YES)
                                        this.CreateCorrespondence(busConstant.ONE_YEAR_BREAK_NOTIFICATION, iobjPassInfo.istrUserID, iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE); //PROD PIR 845
                                    //IF CORR IS SUCCESSFULL
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.Insert();

                                    lock (iobjLock)
                                    {
                                        //BIS Report
                                        #region Insert Records for Reports
                                        DataRow ldrNewRow = idtbBreakInServiceRecords.NewRow();
                                        ldrNewRow["MPI_PERSON_ID"] = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
                                        //ldrNewRow["FIRST_NAME"] = Convert.ToString(acdoPerson["FIRST_NAME"]);
                                        //ldrNewRow["LAST_NAME"] = Convert.ToString(acdoPerson["LAST_NAME"]);
                                        ldrNewRow["PARTICIPANT_NAME"] = lbusPersonOverview.icdoPerson.first_name + " " + lbusPersonOverview.icdoPerson.last_name;
                                        //ldrNewRow["RETIREMENT_DATE"] = Convert.ToDateTime(acdoPerson["RETIREMENT_DATE"]);
                                        ldrNewRow["QUALIFIED_YEAR"] = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count.ToString();
                                        ldrNewRow["BIS_FLAG"] = lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag;
                                        ldrNewRow["DOB"] = lbusPersonOverview.icdoPerson.date_of_birth;
                                        ldrNewRow["DOD"] = lbusPersonOverview.icdoPerson.date_of_death == DateTime.MinValue ? "" : Convert.ToString(lbusPersonOverview.icdoPerson.date_of_death);
                                        ldrNewRow["VALID_ADDRESS_PRESENT"] = lbusPersonOverview.icdoPerson.IS_VALID_ADDRESS_PRESENT;
                                        if ((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                             where obj.year == DateTime.Now.Year - 1
                                             orderby obj.year descending
                                             select obj.qualified_hours).Count() > 0)
                                        {

                                            ldrNewRow["LAST_YEAR_HOURS"] = Convert.ToDecimal((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                              where obj.year == DateTime.Now.Year - 1
                                                                                              orderby obj.year descending
                                                                                              select obj.qualified_hours).FirstOrDefault());

                                            ldrNewRow["SECOND_LAST_YEAR_HOURS"] = Convert.ToDecimal((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                                     where obj.year == DateTime.Now.Year - 2
                                                                                                     orderby obj.year descending
                                                                                                     select obj.qualified_hours).FirstOrDefault());
                                        }

                                        //ldrNewRow["PAYEE_ACCOUNT_ID"] = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                                        idtbBreakInServiceRecords.Rows.Add(ldrNewRow);
                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                if (ldtPersonBatchFlag.Rows.Count > 0)
                                {
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.LoadData(ldtPersonBatchFlag.Rows[0]);
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag = busConstant.FLAG_NO;
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.Update();
                                }
                                else
                                {
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.person_id = lbusPersonOverview.icdoPerson.person_id;
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag = busConstant.FLAG_NO;
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.Insert();
                                }
                            }
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
                    string lstrMsg = "For MPID : " + lbusPersonOverview.icdoPerson.mpi_person_id + ".Error while Executing Batch, Error Message: " + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();
            }
        }
        #endregion

        #region Commented Out Section
        //#region PreNotification Break I nService Batch

        //public void PreNotificationBreakInServiceBatch()
        //{
        //    ArrayList aarrResult;
        //    Hashtable ahtbQueryBkmarks;
        //    int lintCount = 0;
        //    int lintTotalCount = 0;
        //    busBase lobjBase = new busBase();
        //    utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
        //    string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
        //    iobjLock = new object();


        //    DataTable ldtPersonList = busBase.Select("cdoPerson.GetAllActiveParticipantsMPIPP", new object[0]);
        //    Collection<busPerson> lclbPerson = new Collection<busPerson>();
        //    //lclbPerson = lobjBase.GetCollection<busPerson>(ldtPersonList, "icdoPerson"); //not required

        //    SqlParameter[] parameters = new SqlParameter[2];
        //    SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
        //    SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);

        //    foreach (DataRow ldrPerson in ldtPersonList.AsEnumerable())
        //    {
        //        lintCount++;
        //        lintTotalCount++;
        //        if (lintCount == 100)
        //        {
        //            String lstrMsg = lintTotalCount + " : " + " Records Has Been Processed";
        //            PostInfoMessage(lstrMsg);
        //            lintCount = 0;
        //        }

        //        aarrResult = new ArrayList();
        //        ahtbQueryBkmarks = new Hashtable();
        //        busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
        //        lbusPersonOverview.icdoPerson.LoadData(ldrPerson);          //need to fill busPersonOverview object in future
        //        lbusPersonOverview.LoadPersonAddresss();
        //        lbusPersonOverview.LoadPersonContacts();
        //        lbusPersonOverview.LoadCorrAddress();

        //        lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
        //        lbusPersonOverview.lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
        //        lbusPersonOverview.lbusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusPersonOverview.icdoPerson.idtDateofBirth, iobjSystemManagement.icdoSystemManagement.batch_date);
        //        lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson = lbusPersonOverview.icdoPerson;

        //        DataTable ldtPersonAccountList = busBase.Select("cdoPerson.LoadPersonAccountForMPIPP", new object[1] { lbusPersonOverview.icdoPerson.person_id });
        //        if (ldtPersonAccountList.Rows.Count > 0)
        //        {
        //            lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount = lobjBase.GetCollection<busPersonAccount>(ldtPersonAccountList, "icdoPersonAccount");
        //        }

        //        lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date = DateTime.Now;
        //        lbusPersonOverview.lbusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();

        //        if ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()) &&
        //        lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count() > 0 &&
        //        lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year == DateTime.Now.Year)
        //        {
        //            DateTime SatBeforeLastThrusday = busGlobalFunctions.GetLastDateOfComputationYear(DateTime.Now.Year);
        //            if (DateTime.Now <= SatBeforeLastThrusday)
        //            {
        //                lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.RemoveAt(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.
        //                    IndexOf(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last()));
        //            }
        //        }

        //        aarrResult.Add(lbusPersonOverview);

        //        //lbusPersonOverview.lbusBenefitApplication.DetermineVesting();

        //        busPersonBatchFlags lbusPersonBatchFlags = new busPersonBatchFlags { icdoPersonBatchFlags = new cdoPersonBatchFlags() };
        //        DataTable ldtPersonBatchFlag = busBase.Select("cdoPersonBatchFlags.GetBatchFlagsbyPersonID", new object[1] { lbusPersonOverview.icdoPerson.person_id });

        //        if ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()) && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_hours < 200
        //              && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().bis_years_count == 1 && (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrForfietureFlag.IsNull() || lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrForfietureFlag != busConstant.FLAG_YES))
        //        {
        //            if (ldtPersonBatchFlag.Rows.Count > 0)
        //            {
        //                lbusPersonBatchFlags.icdoPersonBatchFlags.LoadData(ldtPersonBatchFlag.Rows[0]);
        //                if (string.IsNullOrEmpty(lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag) ||
        //                    lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag == busConstant.FLAG_NO && lbusPersonOverview.icdoPerson.IS_VALID_ADDRESS_PRESENT == busConstant.FLAG_YES)
        //                {
        //                    //GENERATE CORR CODE
        //                    string str = this.CreateCorrespondence(busConstant.ONE_YEAR_BREAK_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
        //                    //IF CORR IS SUCCESSFULL
        //                }
        //                lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag = busConstant.FLAG_YES;
        //                lbusPersonBatchFlags.icdoPersonBatchFlags.Update();
        //            }
        //            else
        //            {
        //                lbusPersonBatchFlags.icdoPersonBatchFlags.person_id = lbusPersonOverview.icdoPerson.person_id;
        //                lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag = busConstant.FLAG_YES;
        //                //GENERATE CORR CODE
        //                if (lbusPersonOverview.icdoPerson.IS_VALID_ADDRESS_PRESENT == busConstant.FLAG_YES)
        //                   this.CreateCorrespondence(busConstant.ONE_YEAR_BREAK_NOTIFICATION, iobjPassInfo.istrUserID, iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
        //                //IF CORR IS SUCCESSFULL
        //                lbusPersonBatchFlags.icdoPersonBatchFlags.Insert();

        //            }
        //        }
        //        else
        //        {

        //            if (ldtPersonBatchFlag.Rows.Count > 0 && lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag == busConstant.FLAG_YES)
        //            {

        //                lbusPersonBatchFlags.icdoPersonBatchFlags.LoadData(ldtPersonBatchFlag.Rows[0]);
        //                lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag = busConstant.FLAG_NO;
        //                lbusPersonBatchFlags.icdoPersonBatchFlags.Update();
        //            }
        //            else
        //            {
        //                lbusPersonBatchFlags.icdoPersonBatchFlags.person_id = lbusPersonOverview.icdoPerson.person_id;
        //                lbusPersonBatchFlags.icdoPersonBatchFlags.pre_notification_bis_flag = busConstant.FLAG_NO;
        //                lbusPersonBatchFlags.icdoPersonBatchFlags.Insert();

        //            }

        //        }
        //    }

        //}
        //#endregion
        #endregion

        #region Notification Break In Service Batch
        DataTable ldtbWorkInformationForAllParticipants { get; set; }
        DataTable ldtbWorkInformationForAllParticipantsWeekly { get; set; }
        DataTable ldtPersonBatchFlag { get; set; }
        DataTable ldtbPersonAccounts { get; set; }

        public void NotificationBreakInServiceBatch()
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

            SqlParameter[] lParameters = new SqlParameter[1];
            SqlParameter param1 = new SqlParameter("@Year", DbType.Int32);
            if (this.iobjSystemManagement.IsNotNull())
                param1.Value = this.iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1;
            else
                param1.Value = DateTime.Now.Year - 1;
            lParameters[0] = param1;

            //PROD PIR 845 -- RASHMI(For BIS batch)
            this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;

            iobjLock = new object();
            createTableDesignForBreakInServiceReport();
            createTableDesignForVestingForfeitureChanges();//PIR 1030

            //PIR 1030
            ldtbWorkInformationForAllParticipants = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkHistoryForAllMpippParticipant", astrLegacyDBConnection, null, lParameters);

            DataTable ldtPersonList = busBase.Select("cdoPerson.GetAllActiveParticipantsMPIPP", new object[0]);

            ldtbPersonAccounts = busBase.Select("cdoPerson.GetAllParticipantPersonAccounts", new object[0]);
            ldtPersonBatchFlag = busBase.Select("cdoPersonBatchFlags.GetBatchFlags", new object[0]);

            if (ldtPersonList.Rows.Count > 0)
            {
                //ldtbWorkInformationForAllParticipants = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkHistoryForAllMpippParticipant", astrLegacyDBConnection, null, lParameters);

                //ldtbWorkInformationForAllParticipantsWeekly = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkHistoryForAllMpippParticipantWeekly", astrLegacyDBConnection, null, lParameters);

                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

                Parallel.ForEach(ldtPersonList.AsEnumerable(), po, (acdoPerson, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "NotificationBreakInServiceBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    //Changed Criteria in SP to pull only those participants who reported hours in last 2 years and whose Recalculate vesting flag in 'Y'
                    string lstrSSNDecrypted = Convert.ToString(acdoPerson[enmPerson.ssn.ToString()]);
                    DataRow[] ldrTempWorkInfo = ldtbWorkInformationForAllParticipants.FilterTable(utlDataType.String, "ssn", lstrSSNDecrypted);

                    if (!ldrTempWorkInfo.IsNullOrEmpty())
                    {
                        ProcessNotificationBreakInServiceBatch(acdoPerson, lobjPassInfo, lintCount, lintTotalCount);
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

                //BIS Report
                //PIR-868
                string lstrBatchName = busConstant.NOTIFICATIONBIS_BATCH_NAME;
                if (idtbBreakInServiceRecords.IsNotNull() && idtbBreakInServiceRecords.Rows.Count > 0)
                {
                    try
                    {
                        idtbBreakInServiceRecords.TableName = "ReportTable01";
                        CreatePDFReport(idtbBreakInServiceRecords, "rpt_NotificationBISBatchReport", lstrBatchName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                    }
                }

                //PIR 1030
                if (idtblVestingForfeitureChanges.IsNotNull() && idtblVestingForfeitureChanges.Rows.Count > 0)
                {
                    try
                    {
                        idtblVestingForfeitureChanges.TableName = "ReportTable01";
                        CreatePDFReport(idtblVestingForfeitureChanges, "rpt25_VestingForfeitureChanges", lstrBatchName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                    }
                }


                //PROD PIR 845 - RASHMI
                MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Correspondence_Path, iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_BIS, busConstant.BOOL_TRUE);
            }

            #region Previous Code Commented NOw- Can be deleted once stable
            //ArrayList aarrResult;
            //Hashtable ahtbQueryBkmarks;

            //int lintCount = 0;
            //int lintTotalCount = 0;
            //int lintComputationYear = 0;
            //busBase lobjBase = new busBase();
            //utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            //string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            //iobjLock = new object();

            //DataTable ldtPersonList = busBase.Select("cdoPerson.GetAllActiveParticipantsMPIPP", new object[0]);
            //Collection<busPerson> lclbPerson = new Collection<busPerson>();
            ////lclbPerson = lobjBase.GetCollection<busPerson>(ldtPersonList, "icdoPerson");

            //SqlParameter[] parameters = new SqlParameter[2];
            //SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            //SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);



            //foreach (DataRow ldrPerson in ldtPersonList.AsEnumerable())
            //{
            //    lintCount++;
            //    lintTotalCount++;
            //    lintComputationYear = 0;
            //    if (lintCount == 100)
            //    {
            //        String lstrMsg = lintTotalCount + " : " + " Records Has Been Processed";
            //        PostInfoMessage(lstrMsg);
            //        lintCount = 0;
            //    }

            //    bool BISFlag = false;
            //    aarrResult = new ArrayList();
            //    ahtbQueryBkmarks = new Hashtable();
            //    busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson()};
            //    lbusPersonOverview.icdoPerson.LoadData(ldrPerson);          //need to fill busPersonOverview object in future
            //    lbusPersonOverview.LoadPersonAddresss();
            //    lbusPersonOverview.LoadPersonContacts();
            //    lbusPersonOverview.LoadCorrAddress(); 

            //    lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            //    lbusPersonOverview.lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            //    lbusPersonOverview.lbusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusPersonOverview.icdoPerson.idtDateofBirth, iobjSystemManagement.icdoSystemManagement.batch_date);
            //    lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson = lbusPersonOverview.icdoPerson;

            //    DataTable ldtPersonAccountList = busBase.Select("cdoPerson.LoadPersonAccountForMPIPP", new object[1] { lbusPersonOverview.icdoPerson.person_id });
            //    if (ldtPersonAccountList.Rows.Count > 0)
            //    {
            //        lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount = lobjBase.GetCollection<busPersonAccount>(ldtPersonAccountList, "icdoPersonAccount");
            //    }

            //    lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date = DateTime.Now;
            //    lbusPersonOverview.lbusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();

            //    if ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()) &&
            //    lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count() > 0 &&
            //    lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year == DateTime.Now.Year)
            //    {
            //        DateTime SatBeforeLastThrusday = busGlobalFunctions.GetLastDateOfComputationYear(DateTime.Now.Year);
            //        if (DateTime.Now <= SatBeforeLastThrusday)
            //        {
            //            lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.RemoveAt(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.
            //                IndexOf(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last()));
            //        }
            //    }
            //    //lbusbenefitApplication.DetermineVesting();
            //    aarrResult.Add(lbusPersonOverview);

            //    busPersonBatchFlags lbusPersonBatchFlags = new busPersonBatchFlags { icdoPersonBatchFlags = new cdoPersonBatchFlags() };
            //    DataTable ldtPersonBatchFlag = busBase.Select("cdoPersonBatchFlags.GetBatchFlagsbyPersonID", new object[1] { lbusPersonOverview.icdoPerson.person_id });

            //    if (!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
            //    {
            //        #region Code for BIS detection and generation of Notification Correspondance if NOT already sent to Person - Updating the TABLE OF BATCH FLAGS -- CODE SHOULD BE OPTIMIZED
            //        if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count >= 2
            //            && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI[lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IndexOf(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last()) - 1].vested_hours < 200)
            //        {
            //            BISFlag = true;
            //        }

            //        if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_hours < 200 && BISFlag)
            //        {
            //            int lintVestedCount=0;
            //            DataTable ldtblVestedCount = busPerson.Select("cdoPersonAccountEligibility.CheckIfPersonIsVested", new object[1] { lbusPersonOverview.icdoPerson.person_id });
            //            if (ldtblVestedCount.Rows.Count > 0 && Convert.ToString(ldtblVestedCount.Rows[0][0]).IsNotNullOrEmpty())
            //            {
            //                lintVestedCount = Convert.ToInt32(ldtblVestedCount.Rows[0][0]);
            //            }

            //            if (ldtPersonBatchFlag.Rows.Count > 0)
            //            {
            //                lbusPersonBatchFlags.icdoPersonBatchFlags.LoadData(ldtPersonBatchFlag.Rows[0]);
            //                if (string.IsNullOrEmpty(lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag) ||
            //                    lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag == busConstant.FLAG_NO)
            //                {
            //                    string str = string.Empty;

            //                    if (ldtblVestedCount.Rows.Count > 0 && Convert.ToInt32(ldtblVestedCount.Rows[0][0]) > 0)
            //                    {
            //                        str = this.CreateCorrespondence(busConstant.BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
            //                    }
            //                    else
            //                    {
            //                        str = this.CreateCorrespondence(busConstant.NON_VESTED_BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
            //                    }

            //                }
            //                lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_YES;
            //                lbusPersonBatchFlags.icdoPersonBatchFlags.Update();
            //            }
            //            else
            //            {
            //                lbusPersonBatchFlags.icdoPersonBatchFlags.person_id = lbusPersonOverview.icdoPerson.person_id;
            //                lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_YES;
            //                string str = string.Empty;

            //                if (lintVestedCount > 0)
            //                {
            //                    str = this.CreateCorrespondence(busConstant.BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
            //                }
            //                else
            //                {
            //                    str = this.CreateCorrespondence(busConstant.NON_VESTED_BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
            //                }

            //                lbusPersonBatchFlags.icdoPersonBatchFlags.Insert();

            //            }
            //        }
            //        else
            //        {

            //            if (ldtPersonBatchFlag.Rows.Count > 0 && lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag == busConstant.FLAG_YES)
            //            {

            //                lbusPersonBatchFlags.icdoPersonBatchFlags.LoadData(ldtPersonBatchFlag.Rows[0]);
            //                lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_NO;
            //                lbusPersonBatchFlags.icdoPersonBatchFlags.Update();
            //            }
            //            else
            //            {
            //                lbusPersonBatchFlags.icdoPersonBatchFlags.person_id = lbusPersonOverview.icdoPerson.person_id;
            //                lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_NO;
            //                lbusPersonBatchFlags.icdoPersonBatchFlags.Insert();

            //            }
            //        }
            //        #endregion

            //        #region Code 4 Yearly Forfeiture Batch to POST NEGATIVE ENTRIES FOR THE PERSON

            //        if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count() > 0 && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().comments != null)
            //        {
            //            if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().comments.Contains(busConstant.FORFEITURE_COMMENT))
            //            {
            //                DataTable ldtbIAPAllocationDetail = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPRetriementContribution", new object[1] { lbusPersonOverview.icdoPerson.person_id });
            //                if (ldtbIAPAllocationDetail.Rows.Count > 0)
            //                {
            //                    lintComputationYear = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year;

            //                    #region Making Negative Entries in Retirement Contribution Due to FORFEITURE
            //                    if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc1.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc1.ToString()]) != busConstant.ZERO_DECIMAL)
            //                    {
            //                        busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                        lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date,
            //                                                                                    lintComputationYear,
            //                                                                                    adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc1.ToString()]),
            //                                                                                    astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation1);
            //                    }

            //                    if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2.ToString()]) != busConstant.ZERO_DECIMAL)
            //                    {
            //                        busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                        lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date,
            //                                                                                    lintComputationYear,
            //                                                                                    adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2.ToString()]),
            //                                                                                    astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation2);
            //                    }

            //                    if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_invt.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_invt.ToString()]) != busConstant.ZERO_DECIMAL)
            //                    {
            //                        busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                        lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date,
            //                                                                                    lintComputationYear,
            //                                                                                    adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_invt.ToString()]),
            //                                                                                    astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation2,
            //                                                                                    astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
            //                    }

            //                    if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_frft.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_frft.ToString()]) != busConstant.ZERO_DECIMAL)
            //                    {
            //                        busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                        lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date,
            //                                                                                    lintComputationYear,
            //                                                                                    adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_frft.ToString()]),
            //                                                                                    astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation2,
            //                                                                                    astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
            //                    }

            //                    if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc3.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc3.ToString()]) != busConstant.ZERO_DECIMAL)
            //                    {
            //                        busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                        lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date,
            //                                                                                    lintComputationYear,
            //                                                                                    adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc3.ToString()]),
            //                                                                                    astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation3);
            //                    }

            //                    if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4.ToString()]) != busConstant.ZERO_DECIMAL)
            //                    {
            //                        busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                        lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date,
            //                                                                                    lintComputationYear,
            //                                                                                    adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4.ToString()]),
            //                                                                                    astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation4);
            //                    }

            //                    if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_frft.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_frft.ToString()]) != busConstant.ZERO_DECIMAL)
            //                    {
            //                        busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                        lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date,
            //                                                                                    lintComputationYear,
            //                                                                                    adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_frft.ToString()]),
            //                                                                                    astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation4,
            //                                                                                    astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
            //                    }

            //                    if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_invt.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_invt.ToString()]) != busConstant.ZERO_DECIMAL)
            //                    {
            //                        busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                        lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date,
            //                                                                                    lintComputationYear,
            //                                                                                    adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_invt.ToString()]),
            //                                                                                    astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation4,
            //                                                                                    astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
            //                    }

            //                    //if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_affl.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_affl.ToString()]) != busConstant.ZERO_DECIMAL)
            //                    //{
            //                    //    busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                    //    lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                    //                                                                busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                    //                                                                iobjSystemManagement.icdoSystemManagement.batch_date,
            //                    //                                                                lintComputationYear,
            //                    //                                                                adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_affl.ToString()]),
            //                    //                                                                astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                    //                                                                astrContributionType: busConstant.RCContributionTypeAllocation5,
            //                    //                                                                astrContributionSubtype: busConstant.RCContributionSubTypeAffiliates);
            //                    //}

            //                    //if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_both.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_both.ToString()]) != busConstant.ZERO_DECIMAL)
            //                    //{
            //                    //    busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                    //    lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                    //                                                                busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                    //                                                                iobjSystemManagement.icdoSystemManagement.batch_date,
            //                    //                                                                lintComputationYear,
            //                    //                                                                adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_both.ToString()]),
            //                    //                                                                astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                    //                                                                astrContributionType: busConstant.RCContributionTypeAllocation5,
            //                    //                                                                astrContributionSubtype: busConstant.RCContributionSubTypeBoth);
            //                    //}

            //                    //if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_nonaffl.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_nonaffl.ToString()]) != busConstant.ZERO_DECIMAL)
            //                    //{
            //                    //    busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                    //    lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                    //                                                                busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                    //                                                                iobjSystemManagement.icdoSystemManagement.batch_date,
            //                    //                                                                lintComputationYear,
            //                    //                                                                adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_nonaffl.ToString()]),
            //                    //                                                                astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                    //                                                                astrContributionType: busConstant.RCContributionTypeAllocation5,
            //                    //                                                                astrContributionSubtype: busConstant.RCContributionSubTypeNonAffiliates);
            //                    //}

            //                    if (ldtbIAPAllocationDetail.Rows[0]["alloc5"] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["alloc5"]) != busConstant.ZERO_DECIMAL)
            //                    {
            //                        busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                        lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date,
            //                                                                                    lintComputationYear,
            //                                                                                    adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["alloc5"]),
            //                                                                                    astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation5);
            //                    }

            //                    if (ldtbIAPAllocationDetail.Rows[0]["L52ALLOC1"] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["L52ALLOC1"]) != busConstant.ZERO_DECIMAL)
            //                    {
            //                        busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                        lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date,
            //                                                                                    lintComputationYear,
            //                                                                                    adec52SplAccountBalance: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["L52ALLOC1"]),
            //                                                                                    astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation1);
            //                    }

            //                    if (ldtbIAPAllocationDetail.Rows[0]["L161ALLOC1"] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["L161ALLOC1"]) != busConstant.ZERO_DECIMAL)
            //                    {
            //                        busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            //                        lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
            //                                                                                    busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date,
            //                                                                                    lintComputationYear,
            //                                                                                    adec161SplAccountBalance: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["L161ALLOC1"]),
            //                                                                                    astrTransactionType: busConstant.RCTransactionTypeForfeiture,
            //                                                                                    astrContributionType: busConstant.RCContributionTypeAllocation1);
            //                    }
            //                    #endregion
            //                }
            //            }
            //        }

            //        #endregion 
            //    }

            //}
            #endregion
        }
        #endregion

        #region Process Retiree Health Elgibility
        public DataTable idtPersonAccount { get; set; }
        public void ProcessRetireeHealthElgibility()
        {
            //Get all Participant Info for those people WHOES Health Eligibile Flag not already "Y"

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
            iobjLock = new object();

            //Get the List of Person's We need to Pick for Checking Health Eligibility
            DataTable ldtPersonList = busBase.Select("cdoPerson.GetParticipantsForHealthEligibilityCheck", new object[0] { });

            //load all person accounts
            idtPersonAccount = busBase.Select<cdoPersonAccount>(new string[0] { }, new object[0] { }, null, null);
            if (ldtPersonList.Rows.Count > 0)
            {
                //{
                //    lclbPerson = cdoPerson.GetCollection<cdoPerson>(ldtPersonList);
                //}

                //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

                Parallel.ForEach(ldtPersonList.AsEnumerable(), po, (acdoPerson, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "HealthEligbilityBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    CalculateHealthEligibilityandCheckNewWorkingPeople(acdoPerson, lobjPassInfo, lintCount, lintTotalCount);


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
            lbusPersonOverview.icdoPerson.LoadData(acdoPerson);

            autlPassInfo.BeginTransaction();
            try
            {

                lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusPersonOverview.lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson = lbusPersonOverview.icdoPerson;
                DataRow[] ldrPersonAccount = idtPersonAccount.FilterTable(utlDataType.Numeric, "person_id", lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson.person_id);
                lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                busBase lobjBase = new busBase();
                lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount = lobjBase.GetCollection<busPersonAccount>(ldrPersonAccount, "icdoPersonAccount");
                lbusPersonOverview.iobjSystemManagement = iobjSystemManagement;
                lbusPersonOverview.iblnFromBatch = true;
                lbusPersonOverview.CheckRetireeHealthEligibilityAndUpdateFlag();

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

        #region Pension Eligibility Batch

        //Ticket# 72507
        public void PensionEligibilityBatch()
        {

            //Get all Participant Info for those people WHOES Health Eligibile Flag not already "Y"
            int lintCount = 0;
            int lintTotalCount = 0;
            DataSet idtbstPensionEligibility = new DataSet();
            createTableDesignForPensionEligibility();
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();
          
            DataTable ldtPersonInformation = busBase.Select("cdoPerson.GetAllEligibleActiveParticipantForPension", new object[0]);
            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

            Parallel.ForEach(ldtPersonInformation.AsEnumerable(), po, (acdoPerson, loopState) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "PensionEligbilityBatch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;

                CalculatePensionEligibilityAndGenerateCorr(acdoPerson, lobjPassInfo, lintCount, lintTotalCount);

                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;


            });

            MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Correspondence_Path,
                           iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_PensionEligibility, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
            //rid 76227 sort by last name and first name
            if (idtbPensionEligibility.IsNotNull() && idtbPensionEligibility.Rows.Count > 0)
            {
                idtbstPensionEligibility.Tables.Add(idtbPensionEligibility.AsEnumerable()
                        .OrderBy(r => r.Field<string>("LAST_NAME"))
                        .ThenBy(r => r.Field<string>("FIRST_NAME"))
                        .CopyToDataTable());

                //idtbstPensionEligibility.Tables.Add(idtbPensionEligibility.Copy());
                idtbstPensionEligibility.Tables[0].TableName = "ReportTable01";
                idtbstPensionEligibility.DataSetName = "ReportTable01";

                if (idtbstPensionEligibility.Tables[0].Rows.Count > 0)
                {
                    //rid 76227
                    string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.PENSION_ELIGIBILITY_BATCH_REPORT + ".xlsx";
                    string lstrPensionEligibilityReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_PENSION_ELIGIBILITY_REPORT_PATH) + busConstant.PENSION_ELIGIBILITY_BATCH_REPORT + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

                    busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                    lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrPensionEligibilityReportPath, "pension_eligibility", idtbstPensionEligibility);

                    this.CreatePDFReport(idtbstPensionEligibility, "rptPensionEligibilityBatchReport", busConstant.MPIPHPBatch.GENERATED_PENSION_ELIGIBILITY_REPORT_PATH);
                }
            }
            
            lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        }
        #endregion

        #region Calculate Pension Eligibility And Generate Corr

        //Ticket# 72507
        private void CalculatePensionEligibilityAndGenerateCorr(DataRow acdoPerson, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        {

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            string istrBisPartFlag = null;
            busBase lbusBase = new busBase();

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

            busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
            lbusPersonOverview.icdoPerson.LoadData(acdoPerson);
            lbusPersonOverview.FindPerson(lbusPersonOverview.icdoPerson.person_id);
            lbusPersonOverview.LoadPersonAddresss();
            lbusPersonOverview.LoadPersonContacts();
            lbusPersonOverview.LoadCorrAddress();
            this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;

            autlPassInfo.BeginTransaction();
            try
            {
                ArrayList aarrResult;
                Hashtable ahtbQueryBkmarks;

                aarrResult = new ArrayList();
                ahtbQueryBkmarks = new Hashtable();

                lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusPersonOverview.lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(iobjSystemManagement.icdoSystemManagement.batch_date).AddDays(1);
                lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson = lbusPersonOverview.icdoPerson;

              //  lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date = DateTime.Now;

                busAnnualBenefitSummaryOverview lbusAnnualBenefitSummaryOverView = new busAnnualBenefitSummaryOverview();
                lbusAnnualBenefitSummaryOverView.icdoPerson = lbusPersonOverview.icdoPerson;
                lbusAnnualBenefitSummaryOverView.LoadWorkHistory(true);

                if (!lbusAnnualBenefitSummaryOverView.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                {
                    lbusPersonOverview.icdoPerson.idtDateofBirth = lbusAnnualBenefitSummaryOverView.lbusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth;
                    lbusPersonOverview.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusPersonOverview.icdoPerson.idtDateofBirth, iobjSystemManagement.icdoSystemManagement.batch_date);
                    lbusPersonOverview.istrCurrentDate = Convert.ToString(DateTime.Now);
                    lbusPersonOverview.ldtLastReportedDate = GetLastWorkingDate(lbusPersonOverview.icdoPerson.istrSSNNonEncrypted);
                    var lvardecAge = lbusPersonOverview.idecAge;



                    var lvarMPID = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
                    var lvarIapBalance = Convert.ToDecimal(acdoPerson["IAP_BALANCE"]);
                    var lvarEeUVhpbalance = Convert.ToDecimal(acdoPerson["EE_UVHP_AMOUNT"]);
                    var lvarVestedDate = acdoPerson["VESTED_DATE"];
                    var lvarIsvested = Convert.ToString(acdoPerson["ISVESTED"]);
                    var lvarVestedRule = Convert.ToString(acdoPerson["VESTING_RULE"]);



                    var accruredBenefitAmount = 0.00m;
                    var qualifyYearTotal = 0;
                    var localPlan = "N";
                    foreach (var lbPersonAccountOverview in lbusAnnualBenefitSummaryOverView.iclcdoPersonAccountOverview)
                    {

                        if (lbPersonAccountOverview.idecTotalAccruedBenefit.ToString().IsDecimal())
                        {
                            accruredBenefitAmount += Convert.ToDecimal(lbPersonAccountOverview.idecTotalAccruedBenefit);

                        }
                        if (!lbPersonAccountOverview.istrPlanCode.Contains("IAP"))
                        {
                            qualifyYearTotal += Convert.ToInt32(lbPersonAccountOverview.istrTotalQualifiedYears);

                        }

                        if (lbPersonAccountOverview.istrPlanCode.ToString() != "IAP" && lbPersonAccountOverview.istrPlanCode.ToString() != "MPIPP")
                        {
                            localPlan = "Y";

                        }
                        if(lbPersonAccountOverview.istrAllocationEndYear != null)
                        {
                            if (lbPersonAccountOverview.istrPlanCode.Contains("IAP") && lbPersonAccountOverview.istrAllocationEndYear.ToString().IsNumeric())
                            {
                                lbusPersonOverview.iintYear = Convert.ToInt32(lbPersonAccountOverview.istrAllocationEndYear);
                            }

                        }
                   }

                    if (accruredBenefitAmount > 0 && lvarIapBalance > 0)
                    {
                        if (lbusAnnualBenefitSummaryOverView.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(i => i.year == Convert.ToInt32(DateTime.Now.Year) - 1).Count()> 0)
                        {
                             istrBisPartFlag = lbusAnnualBenefitSummaryOverView.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(i => i.year == Convert.ToInt32(DateTime.Now.Year) - 1).FirstOrDefault().istrBisParticipantFlag;
                        }

                        if ((lbusPersonOverview.idecAge >= 64.5M && lbusPersonOverview.idecAge < 65) && (istrBisPartFlag == "N" || istrBisPartFlag.IsNullOrEmpty()) && (lvarVestedDate.ToString().IsNullOrEmpty()))
                        {
                            return;
                        }

                        lbusPersonOverview.ldecTotalAccuruedBftAmt = Convert.ToString(accruredBenefitAmount);
                        lbusPersonOverview.ldecTotalEEUVHPAmt = Convert.ToString(lvarEeUVhpbalance);
                        lbusPersonOverview.ldecTotalSpecialAmt = Convert.ToString(lvarIapBalance);
                        lbusPersonOverview.ldecIsvested = lvarIsvested;
                        aarrResult.Add(lbusPersonOverview);

                        DataRow dr = idtbPensionEligibility.NewRow();
                        dr["MPI_PERSON_ID"] = lvarMPID;
                        dr["NAME"] = lbusPersonOverview.icdoPerson.first_name + " " + lbusPersonOverview.icdoPerson.last_name;
                        dr["ADDRESS1"] = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_1; //rid 124432
                        dr["ADDRESS2"] = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_2; //rid 124432
                        dr["CITY"] = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_city;  //rid 124432
                        dr["STATE"] = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_value;  //rid 124432
                        dr["ZIPCODE"] = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_code;  //rid 124432
                        dr["EMAIL_ADDRESS"] = lbusPersonOverview.icdoPerson.email_address_1; //rid 124432
                        dr["DATE_of_Birth"] = lbusPersonOverview.icdoPerson.date_of_birth;
                        dr["Age"] = Math.Round(lvardecAge); // lbusPersonOverview.idecAge;
                        dr["RETIREMENT_DATE"] = lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date;
                        dr["MPI_Accrued_Benefit"] = String.Format("{0:C}", accruredBenefitAmount);
                        dr["IAP_Balance"] = String.Format("{0:C}", lvarIapBalance);
                        dr["EE_UVHP"] = String.Format("{0:C}", lvarEeUVhpbalance);
                        dr["QY_Total"] = qualifyYearTotal;
                        dr["Vested_Date"] = lvarVestedDate;
                        dr["Vested_Status"] = lvarVestedRule;
                        //rid 76227
                        //Ticket 85361
                        dr["BAD_ADDRESS_FLAG"] = IsBadAddress(lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress);
                        dr["FIRST_NAME"] = lbusPersonOverview.icdoPerson.first_name;
                        dr["LAST_NAME"] = lbusPersonOverview.icdoPerson.last_name;

                        if (istrBisPartFlag.IsNotNullOrEmpty())
                        {
                            dr["BIS"] = istrBisPartFlag;
                        }
                        else
                        {
                            dr["BIS"] = "N";
                        }

                        dr["Local_Plan"] = localPlan;
                        
                        //rid 78399
                        int basedMonth = DateTime.Now.AddMonths(4).Month;
                        if (basedMonth == lbusPersonOverview.icdoPerson.date_of_birth.Month && Math.Round(lvardecAge) == 65)
                        {                            
                            idtbPensionEligibility.Rows.Add(dr);

                            this.CreateCorrespondence(busConstant.NOTIFICATION_OF_PENSION_ELIGIBILITY, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, ablnIsPDF: true);
                            lbusPersonOverview.icdoPerson.pension_eligible_notification_flag = busConstant.FLAG_YES;
                            lbusPersonOverview.icdoPerson.Update();
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
                    String lstrMsg = "Error while Executing Batch,Error Message For MPID " + lbusPersonOverview.icdoPerson.mpi_person_id + ":" + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();

            }

        }
        
        //Ticket 85361
        private string IsBadAddress(cdoPersonAddress personAddress)
        {
            string lisBadAddress = "N";
            if (personAddress.addr_country_value == "0001")
            {
                if (personAddress.addr_line_1.IsNotNullOrEmpty() && personAddress.addr_city.IsNotNullOrEmpty() &&
                    personAddress.addr_state_value.IsNotNullOrEmpty() && personAddress.addr_zip_code.IsNotNullOrEmpty() &&
                    (personAddress.end_date.ToString().IsNullOrEmpty() || personAddress.end_date == DateTime.MinValue))
                {
                    lisBadAddress = "N";
                }
                else
                {
                    lisBadAddress = "Y";
                }
            }
            else
            {
                if (personAddress.addr_line_1.IsNotNullOrEmpty() && personAddress.addr_city.IsNotNullOrEmpty() &&
                    personAddress.istrForeignProvince.IsNotNullOrEmpty() && (personAddress.end_date.ToString().IsNullOrEmpty() ||
                    personAddress.end_date == DateTime.MinValue))
                {
                    lisBadAddress = "N";
                }
                else
                {
                    lisBadAddress = "Y";
                }
            }
            return lisBadAddress;
        }

        private Collection<busRetirementApplication> GetCollection<T1>(DataTable ldtbList, string p)
        {
            throw new NotImplementedException();
        }
      //  #endregion

        #region Minimum Distribution Batch
        public void MinimumDistributionBatch()
        {
            ArrayList larrPersonIAPAdded = new ArrayList();
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            int lintCount = 0;
            int lintTotalCount = 0;

            DateTime ldtCurrentDate = System.DateTime.Now;
            idtNextMDDate = new DateTime(ldtCurrentDate.Year, 04, 01);
            if (idtNextMDDate < ldtCurrentDate)
                idtNextMDDate = new DateTime(ldtCurrentDate.Year + 1, 04, 01);

            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();

            //PROD PIR 854 - MD_BATCH_REPORT
            createTableDesignForMDParticipantsReport();

            //Ticket 79238 - MD Participant Address Report
            createTableDesignForMDParticipantAddressReport();

            //PROD PIR 845 - MD_BATCH
            this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;

            //Get all Participant whose age is greater than 70.5 years 
            DataTable ldtPersonInformation = busBase.Select("cdoPerson.LoadPersonDetailsforMinimumDistribution", new object[0]);

            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions po = new ParallelOptions();
            //PIR 861 Do not run this batch with multiple threads
            po.MaxDegreeOfParallelism = 1;// System.Environment.ProcessorCount * 4;

            #region New People becoming MD on next April.
            Parallel.ForEach(ldtPersonInformation.AsEnumerable(), po, (acdoPerson, loopState) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "MinimumDistributionBatch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;
                //WI 23550 Ticket 143336 added RMD73_DATE
                if (!((Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD72_DATE"]) == idtNextMDDate) ||
                      (Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD73_DATE"]) == idtNextMDDate) ||
                      (Convert.ToDateTime(acdoPerson["MD_DATE"]) < idtNextMDDate)))
                {
                    CalculateMinimumDistributionAndGenerateCorr(acdoPerson, lobjPassInfo, larrPersonIAPAdded, lintCount, lintTotalCount);
                }

                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;
            });

            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
            //PROD PIR 868 - MD_BATCH_REPORT
            if (idtbMDParticipantsRecords.IsNotNull() && idtbMDParticipantsRecords.Rows.Count > 0)
            {
                try
                {
                    string lstrBatchName = busConstant.MD_BATCH_NAME;
                    idtbMDParticipantsRecords.TableName = "ReportTable01";
                    CreatePDFReport(idtbMDParticipantsRecords, "rpt23_MDParticipantReport", lstrBatchName, "NewMDs");

                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                }
            }
            #region Ticket 79238 - Create Excel file with participant mailing addresses
            DataSet idtbstMDParticipantAddresses = new DataSet();
            idtbstMDParticipantAddresses.Tables.Add(idtbMDParticipantAddressRecords.AsEnumerable()
                   .OrderBy(r => r.Field<string>("LAST_NAME"))
                   .ThenBy(r => r.Field<string>("FIRST_NAME"))
                   .CopyToDataTable());

            idtbstMDParticipantAddresses.Tables[0].TableName = "ReportTable01";
            idtbstMDParticipantAddresses.DataSetName = "ReportTable01";
            if (idtbstMDParticipantAddresses.Tables[0].Rows.Count > 0)
            {
                string outPutFilePathMD = string.Empty;
                string outPutReportPathMD = string.Empty;
                outPutFilePathMD = iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_MD + DateTime.Today.Year;
                try
                {
                    if (!Directory.Exists(outPutFilePathMD))
                        Directory.CreateDirectory(outPutFilePathMD);
                    if (Directory.Exists(outPutFilePathMD))
                    {
                        outPutReportPathMD = outPutFilePathMD + '\\' + "Report";
                        if (!Directory.Exists(outPutReportPathMD))
                            Directory.CreateDirectory(outPutReportPathMD);
                        outPutReportPathMD = outPutFilePathMD + '\\' + "Report"+'\\'+"NewMDs";
                        if (!Directory.Exists(outPutReportPathMD))
                            Directory.CreateDirectory(outPutReportPathMD);
                    }
                    string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.MD_PARTICIPANT_ADDRESS_BATCH_REPORT + ".xlsx";
                    string lstrMDParticipantAddressReportPath = outPutReportPathMD + '\\' + "MD_BATCH_" + busConstant.MD_PARTICIPANT_ADDRESS_BATCH_REPORT + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

                    busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                    lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrMDParticipantAddressReportPath, "participant_addresses", idtbstMDParticipantAddresses);
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                }
            }
            #endregion

            //PROD PIR 845 - MD_BATCH
            MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Correspondence_Path,
                              iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_MD, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
            #endregion New People becoming MD on next April.


            #region MD Age Differ participants or Previous years MD dates
            if (idtbMDParticipantsRecords.IsNotNull() && idtbMDParticipantsRecords.Rows.Count > 0)
            {
                idtbMDParticipantsRecords.Rows.Clear();
            }
            if (idtbMDParticipantAddressRecords.IsNotNull() && idtbMDParticipantAddressRecords.Rows.Count > 0)
            {
                idtbMDParticipantAddressRecords.Rows.Clear();
            }

            Parallel.ForEach(ldtPersonInformation.AsEnumerable(), po, (acdoPerson, loopState) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "MinimumDistributionBatch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;
                //WI 23550 Ticket 143336 added RMD73_DATE
                if ((Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD72_DATE"]) == idtNextMDDate) ||
                    (Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD73_DATE"]) == idtNextMDDate) ||
                    (Convert.ToDateTime(acdoPerson["MD_DATE"]) < idtNextMDDate))
                {
                    //New template for MD Age Differ participants or Previous years MD dates
                    CalculateMinimumDistributionAndGenerateCorr(acdoPerson, lobjPassInfo, larrPersonIAPAdded, lintCount, lintTotalCount);
                }

                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;
            });

            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
            //PROD PIR 868 - MD_BATCH_REPORT
            if (idtbMDParticipantsRecords.IsNotNull() && idtbMDParticipantsRecords.Rows.Count > 0)
            {
                try
                {
                    string lstrBatchName = busConstant.MD_BATCH_NAME;
                    idtbMDParticipantsRecords.TableName = "ReportTable01";
                    CreatePDFReport(idtbMDParticipantsRecords, "rpt23_MDParticipantReport", lstrBatchName, "PriorMDs");

                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                }
            }
            #region Ticket 79238 - Create Excel file with participant mailing addresses
            if (idtbMDParticipantsRecords.IsNotNull() && idtbMDParticipantsRecords.Rows.Count > 0)
            {

                DataSet idtbstMDDifferParticipantAddresses = new DataSet();
                idtbstMDDifferParticipantAddresses.Tables.Add(idtbMDParticipantAddressRecords.AsEnumerable()
                       .OrderBy(r => r.Field<string>("LAST_NAME"))
                       .ThenBy(r => r.Field<string>("FIRST_NAME"))
                       .CopyToDataTable());

                idtbstMDDifferParticipantAddresses.Tables[0].TableName = "ReportTable01";
                idtbstMDDifferParticipantAddresses.DataSetName = "ReportTable01";
                if (idtbstMDDifferParticipantAddresses.Tables[0].Rows.Count > 0)
                {
                    string outPutFilePathMD = string.Empty;
                    string outPutReportPathMD = string.Empty;
                    outPutFilePathMD = iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_MD + DateTime.Today.Year;
                    try
                    {
                        if (!Directory.Exists(outPutFilePathMD))
                            Directory.CreateDirectory(outPutFilePathMD);
                        if (Directory.Exists(outPutFilePathMD))
                        {
                            outPutReportPathMD = outPutFilePathMD + '\\' + "Report";
                            if (!Directory.Exists(outPutReportPathMD))
                                Directory.CreateDirectory(outPutReportPathMD);
                            outPutReportPathMD = outPutFilePathMD + '\\' + "Report" + '\\' + "PriorMDs";
                            if (!Directory.Exists(outPutReportPathMD))
                                Directory.CreateDirectory(outPutReportPathMD);
                        }
                        string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.MD_PARTICIPANT_ADDRESS_BATCH_REPORT + ".xlsx";
                        string lstrMDParticipantAddressReportPath = outPutReportPathMD + '\\' + "MD_BATCH_" + busConstant.MD_PARTICIPANT_ADDRESS_BATCH_REPORT + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

                        busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                        lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrMDParticipantAddressReportPath, "participant_addresses", idtbstMDDifferParticipantAddresses);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                    }
                }
                #endregion
            }

            if (idtbMDParticipantsRecords.IsNotNull() && idtbMDParticipantsRecords.Rows.Count > 0)
            {
                //PROD PIR 845 - MD_BATCH
                MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Correspondence_Path,
                              iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_MD, busConstant.BOOL_FALSE, busConstant.BOOL_TRUE);
            }
            #endregion MD Age Differ participants or Previous years MD dates

        }
        #endregion

        #region Calculate Minimum Distribution And GenerateCorr
        private void CalculateMinimumDistributionAndGenerateCorr(DataRow acdoPerson, utlPassInfo autlPassInfo, ArrayList arrPersonIAPAdded, int aintCount,
                                                                    int aintTotalCount)
        {
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
                    PostInfoMessage(lstrMsg);
                    aintCount = 0;
                }
            }

            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusPerson.icdoPerson.LoadData(acdoPerson);

            if (lbusPerson.icdoPerson.idtDateofBirth != DateTime.MinValue)
            {
                autlPassInfo.BeginTransaction();
                try
                {
                    lbusPerson.LoadCorrAddress();
                    ArrayList aarrPerson = new ArrayList();
                    Hashtable ahtbQueryBkmarks1 = new Hashtable();

                    //PIR - 1031
                    if (!Convert.ToString(acdoPerson["VESTED_DATE"]).IsNullOrEmpty())
                        lbusPerson.icdoPerson.idtVestingDate = Convert.ToDateTime(acdoPerson["VESTED_DATE"]);



                    //Calculate minimum distribution
                    busBenefitCalculationRetirement lbusBenefitCalculationHeader = new busBenefitCalculationRetirement();
                    string lstrPlanCode = acdoPerson["istrPLANCODE"].ToString();

                    lbusBenefitCalculationHeader = lbusBenefitCalculationHeader.GenerateMinDistributionEstiFromBatch(lbusPerson,
                                                    iobjSystemManagement.icdoSystemManagement.batch_date, lstrPlanCode);

                    //Ticket# 68545
                    lbusBenefitCalculationHeader.icdoPerson = lbusPerson.icdoPerson;


                    //Ticket# 68545
                    aarrPerson.Add(lbusBenefitCalculationHeader);

                    #region Generated Correspondence and WorkFlow
                    //Generate COrrespondance for each of those people 

                    if (lbusBenefitCalculationHeader != null)
                    {
                        ArrayList aarrResult = new ArrayList();
                        Hashtable ahtbQueryBkmarks = new Hashtable();
                        aarrResult.Add(lbusBenefitCalculationHeader);

                        if (lbusPerson.icdoPerson.iintPlanId == busConstant.MPIPP_PLAN_ID &&
                                    (!lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()))
                        {
                            if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.IsNullOrEmpty())
                            {
                                if (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                                {
                                    if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                                    {   //WI 23550 Ticket 143336 added RMD73_DATE
                                        if ((Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD72_DATE"]) == idtNextMDDate) ||
                                            (Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD73_DATE"]) == idtNextMDDate) ||
                                            (Convert.ToDateTime(acdoPerson["MD_DATE"]) < idtNextMDDate))
                                        {
                                            //New template for MD Age Differ participants or Previous years MD dates
                                            lbusPerson.istrIsVested = busConstant.FLAG_YES;
                                            this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER_REVISED, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);
                                            this.CreateCorrespondence(busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_MINIMUM_DISTRIBUTION_REVISED, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);
                                        }
                                        else {
                                            lbusPerson.istrIsVested = busConstant.FLAG_YES;
                                            this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                            this.CreateCorrespondence(busConstant.AGE_72_RMD_ELECTION_FORM, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE); //RID 118418 AGE_72_RMD_ELECTION_FORM
                                            this.CreateCorrespondence(busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_MINIMUM_DISTRIBUTION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                        }
                                    }
                                }
                            }
                        }
                        else if (lbusPerson.icdoPerson.iintPlanId == busConstant.LOCAL_161_PLAN_ID && (!lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()))
                        {
                            if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.IsNullOrEmpty())
                            {
                                if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                                {   //WI 23550 Ticket 143336 added RMD73_DATE
                                    if ((Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD72_DATE"]) == idtNextMDDate) ||
                                        (Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD73_DATE"]) == idtNextMDDate) ||
                                        (Convert.ToDateTime(acdoPerson["MD_DATE"]) < idtNextMDDate))
                                    {
                                        //New template for MD Age Differ participants or Previous years MD dates
                                        this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER_REVISED, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                    }
                                    else
                                    {
                                        this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                        this.CreateCorrespondence(busConstant.AGE_72_RMD_ELECTION_FORM, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE); //RID 118418 AGE_72_RMD_ELECTION_FORM
                                    }
                                    this.CreateCorrespondence(busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_161, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                    //this.CreateCorrespondence("RETR-0034", this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                                }
                            }
                        }
                        else if (lbusPerson.icdoPerson.iintPlanId == busConstant.LOCAL_52_PLAN_ID && (!lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()))
                        {
                            if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.IsNullOrEmpty())
                            {
                                if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                                {   //WI 23550 Ticket 143336 added RMD73_DATE
                                    if ((Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD72_DATE"]) == idtNextMDDate) ||
                                        (Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD73_DATE"]) == idtNextMDDate) ||
                                        (Convert.ToDateTime(acdoPerson["MD_DATE"]) < idtNextMDDate))
                                    {
                                        //New template for MD Age Differ participants or Previous years MD dates
                                        this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER_REVISED, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                    }
                                    else
                                    {
                                        this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                        this.CreateCorrespondence(busConstant.AGE_72_RMD_ELECTION_FORM, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE); //RID 118418 AGE_72_RMD_ELECTION_FORM
                                    }
                                    this.CreateCorrespondence(busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_52, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                }
                            }
                        }
                        else if (lbusPerson.icdoPerson.iintPlanId == busConstant.LOCAL_600_PLAN_ID && (!lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()))
                        {
                            if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.IsNullOrEmpty())
                            {
                                if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                                {   //WI 23550 Ticket 143336 added RMD73_DATE
                                    if ((Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD72_DATE"]) == idtNextMDDate) ||
                                        (Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD73_DATE"]) == idtNextMDDate) ||
                                        (Convert.ToDateTime(acdoPerson["MD_DATE"]) < idtNextMDDate))
                                    {
                                        //New template for MD Age Differ participants or Previous years MD dates
                                        this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER_REVISED, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                    }
                                    else
                                    {
                                        this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                        this.CreateCorrespondence(busConstant.AGE_72_RMD_ELECTION_FORM, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE); //RID 118418 AGE_72_RMD_ELECTION_FORM
                                    }
                                    this.CreateCorrespondence(busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_600, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                }
                            }
                        }
                        else if (lbusPerson.icdoPerson.iintPlanId == busConstant.LOCAL_666_PLAN_ID && (!lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()))
                        {
                            if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.IsNullOrEmpty())
                            {
                                if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                                {   //WI 23550 Ticket 143336 added RMD73_DATE
                                    if ((Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD72_DATE"]) == idtNextMDDate) ||
                                        (Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD73_DATE"]) == idtNextMDDate) ||
                                        (Convert.ToDateTime(acdoPerson["MD_DATE"]) < idtNextMDDate))
                                    {
                                        //New template for MD Age Differ participants or Previous years MD dates
                                        this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER_REVISED, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                    }
                                    else
                                    {
                                        this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                        this.CreateCorrespondence(busConstant.AGE_72_RMD_ELECTION_FORM, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE); //RID 118418 AGE_72_RMD_ELECTION_FORM
                                    }
                                    this.CreateCorrespondence(busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_666, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                }
                            }
                        }
                        else if (lbusPerson.icdoPerson.iintPlanId == busConstant.LOCAL_700_PLAN_ID && (!lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()))
                        {
                            if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.IsNullOrEmpty())
                            {
                                if (!lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.IsNullOrEmpty())
                                {   //WI 23550 Ticket 143336 added RMD73_DATE
                                    if ((Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD72_DATE"]) == idtNextMDDate) ||
                                        (Convert.ToDateTime(acdoPerson["MD_DATE"]) == idtNextMDDate && Convert.ToDateTime(acdoPerson["RMD73_DATE"]) == idtNextMDDate) ||
                                        (Convert.ToDateTime(acdoPerson["MD_DATE"]) < idtNextMDDate))
                                    {
                                        //New template for MD Age Differ participants or Previous years MD dates
                                        this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER_REVISED, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                    }
                                    else
                                    {
                                        this.CreateCorrespondence(busConstant.MINIMUM_DISTRIBUTION_COVER_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                        this.CreateCorrespondence(busConstant.AGE_72_RMD_ELECTION_FORM, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrPerson, ahtbQueryBkmarks1, busConstant.BOOL_TRUE); //RID 118418 AGE_72_RMD_ELECTION_FORM
                                    }
                                    this.CreateCorrespondence(busConstant.RETIREMENT_BENEFIT_ELECTION_FORM_LOCAL_700, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);//PROD PIR 845 - MD_BATCH
                                }
                            }
                        }

                        //PIR 861 WORKFLOW SHOULD BE INITIATED AFTER SCANNING THE BENEFIT ELECTION FORM DOCUMENT
                        //if (!lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty() || !lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_IAP.IsNullOrEmpty())
                        //    busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.RETIREMENT_WORKFLOW_NAME, lbusPerson.icdoPerson.person_id, 0, 0, null);

                        //PROD PIR 854 - MD_BATCH_REPORT                      
                        #region Insert Records for MD Participant Report
                        //PROD PIR 854
                        if (lbusBenefitCalculationHeader != null && lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id > 0)
                        {
                            DataRow ldrNewRow = idtbMDParticipantsRecords.NewRow();
                            ldrNewRow["MPI_PERSON_ID"] = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
                            //Ticket 79238 - Break up Full Participant Name into First and Last.
                            //ldrNewRow["PARTICIPANT_NAME"] = Convert.ToString(acdoPerson["FIRST_NAME"]) + " " + Convert.ToString(acdoPerson["LAST_NAME"]);
                            ldrNewRow["LAST_NAME"] = Convert.ToString(acdoPerson["LAST_NAME"]);
                            ldrNewRow["FIRST_NAME"] = Convert.ToString(acdoPerson["FIRST_NAME"]);
                            ldrNewRow["DOB"] = Convert.ToDateTime(acdoPerson["DOB"]);
                            //DateTime MD_DATE = Convert.ToDateTime(busGlobalFunctions.CalculateMinDistributionDate(Convert.ToDateTime(acdoPerson["DOB"]), Convert.ToDateTime(acdoPerson["VESTED_DATE"])));
                            //RID 118418 USING NEW FUNCTION TO GET MD DATE
                            DateTime MD_DATE = Convert.ToDateTime(busGlobalFunctions.CalculateMinDistributionDate(lbusPerson.icdoPerson.person_id, Convert.ToDateTime(acdoPerson["VESTED_DATE"])));
                            //ldrNewRow["AGE"] = busGlobalFunctions.CalculatePersonAge(Convert.ToDateTime(acdoPerson["DOB"]), MD_DATE);
                            ldrNewRow["YEAR_70_6"] = Convert.ToDateTime(acdoPerson["IDTDATEPERSONAGE70ANDHALF"]).Year;
                            ldrNewRow["MD_DATE"] = MD_DATE;
                            ldrNewRow["QLFD_YR_COUNT"] = lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                            ldrNewRow["VESTED_DATE"] = Convert.ToDateTime(acdoPerson["VESTED_DATE"]);
                            ldrNewRow["PLAN"] = lstrPlanCode;
                            //Ticket 79238 - Add Calc ID column
                            ldrNewRow["CALC_ID"] = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                            ldrNewRow["COMPUTATION_YR"] = iobjSystemManagement.icdoSystemManagement.batch_date.Year;//PROD PIR 854
                            ldrNewRow["IS_VALID_ADDRESS"] = Convert.ToString(acdoPerson["ValidAddressFlag"]); //PIR 861
                            //RID 118418 Added column for MD batch enhancement
                            ldrNewRow["RMD73_DATE"] = Convert.ToDateTime(acdoPerson["RMD73_DATE"]); //WI 23550 Ticket 143336 RMD72Date to RMD73Date
                            idtbMDParticipantsRecords.Rows.Add(ldrNewRow);
                        }
                        #endregion
                        #region Insert Records for MD Participant Address Report
                        //Ticket 79238
                        if (lbusBenefitCalculationHeader != null && lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id > 0)
                        {
                            DataRow ldrNewRow = idtbMDParticipantAddressRecords.NewRow();
                            ldrNewRow["MPI_PERSON_ID"] = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
                            ldrNewRow["LAST_NAME"] = Convert.ToString(acdoPerson["LAST_NAME"]);
                            ldrNewRow["FIRST_NAME"] = Convert.ToString(acdoPerson["FIRST_NAME"]);
                            ldrNewRow["ADDR_LINE_1"] = Convert.ToString(acdoPerson["ADDR_LINE_1"]);
                            ldrNewRow["ADDR_LINE_2"] = Convert.ToString(acdoPerson["ADDR_LINE_2"]);
                            ldrNewRow["ADDR_CITY"] = Convert.ToString(acdoPerson["ADDR_CITY"]);
                            ldrNewRow["ADDR_STATE_VALUE"] = Convert.ToString(acdoPerson["ADDR_STATE_VALUE"]);
                            ldrNewRow["ADDR_ZIP_CODE"] = Convert.ToString(acdoPerson["ADDR_ZIP_CODE"]);
                            idtbMDParticipantAddressRecords.Rows.Add(ldrNewRow);
                        }
                        #endregion
                    }

                    autlPassInfo.Commit();
                    #endregion
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
                // }
            }

        }
        #endregion

        #region Annual Interest Posting Batch
        public void AnnualInterestPostingBatch()
        {
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            int lintCount = 0;
            int lintTotalCount = 0;

            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();


            DataTable ldtPersonRetirementContributionInformation = busBase.Select("cdoPerson.LoadPreviousYearContributions", new object[0]);

            #region Get Rate of Interest
            decimal ldecRateOfInterest = 0;
            if (ldtPersonRetirementContributionInformation.Rows.Count > 0)
            {
                busCalculation lbusCalculation = new busCalculation();
                ldecRateOfInterest = lbusCalculation.CalculateRateOfInterest(iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1);
            }

            #endregion

            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

            Parallel.ForEach(ldtPersonRetirementContributionInformation.AsEnumerable(), po, (acdoPersonRetirementContribution, loopState) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "AnnualInterestPostingBatch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;


                CalculateRetirementInterestamount(acdoPersonRetirementContribution, lobjPassInfo, ldecRateOfInterest, lintCount, lintTotalCount);

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
        #endregion

        #region Calculate Retirement Interest Amount
        private void CalculateRetirementInterestamount(DataRow acdoPersonRetirementContribution, utlPassInfo autlPassInfo, decimal adecRateOfInterest, int aintCount, int aintTotalCount)
        {
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
                    PostInfoMessage(lstrMsg);
                    aintCount = 0;
                }
            }

            autlPassInfo.BeginTransaction();
            try
            {

                decimal ldecUVHPInterestAmt = busConstant.ZERO_DECIMAL, ldecEEInterestAmt = busConstant.ZERO_DECIMAL;
                string lstrContributionType = string.Empty;
                DateTime ldtDateOfDeath = new DateTime();

                if (Convert.ToString(acdoPersonRetirementContribution[enmBenefitApplication.retirement_date.ToString()]).IsNotNullOrEmpty())
                {
                    ldtDateOfDeath = Convert.ToDateTime(acdoPersonRetirementContribution[enmBenefitApplication.retirement_date.ToString()]);
                }


                busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution =
                    new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.LoadData(acdoPersonRetirementContribution);
                int lintComputationalYear = Convert.ToInt32(lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.computational_year);
                
                //RID 99947 Added Received Year variable and check in the below condition.
                int lintReceivedYear = Convert.ToInt32(acdoPersonRetirementContribution["RECEIVED_YEAR"]);

                if (
                    (( (Convert.ToString(acdoPersonRetirementContribution[enmBenefitApplication.benefit_type_value.ToString()]) == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT 
                      && ldtDateOfDeath != DateTime.MinValue && ldtDateOfDeath.Year > iobjSystemManagement.icdoSystemManagement.batch_date.Year - 2) ||
                    Convert.ToString(acdoPersonRetirementContribution[enmBenefitApplication.benefit_type_value.ToString()]).IsNullOrEmpty())
                    && lintComputationalYear == iobjSystemManagement.icdoSystemManagement.batch_date.Year - 2)
                    ||
                    (Convert.ToString(acdoPersonRetirementContribution[enmBenefitApplication.benefit_type_value.ToString()]).IsNullOrEmpty()
                     && lintComputationalYear <= iobjSystemManagement.icdoSystemManagement.batch_date.Year - 2
                     && lintReceivedYear == iobjSystemManagement.icdoSystemManagement.batch_date.Year - 2
                    )
                   )
                {

                    if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_UVHP)
                    {
                        ldecUVHPInterestAmt = (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_amount +
                                            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.uvhp_int_amount) * adecRateOfInterest;
                        lstrContributionType = busConstant.CONTRIBUTION_TYPE_UVHP;
                    }
                    else if (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_type_value == busConstant.CONTRIBUTION_TYPE_EE)
                    {
                        ldecEEInterestAmt = (lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_contribution_amount +
                                              lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.ee_int_amount) * adecRateOfInterest;
                        lstrContributionType = busConstant.CONTRIBUTION_TYPE_EE;
                    }

                    if (ldecEEInterestAmt > 0 || ldecUVHPInterestAmt > 0)
                    {
                        lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(acdoPersonRetirementContribution[enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                                             busGlobalFunctions.GetLastDateOfComputationYear(iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1), iobjSystemManagement.icdoSystemManagement.batch_date, iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1,
                                             busConstant.ZERO_DECIMAL, busConstant.ZERO_DECIMAL, busConstant.ZERO_DECIMAL, busConstant.TransactionTypeInterest,
                                             busConstant.ZERO_DECIMAL, busConstant.ZERO_DECIMAL, ldecEEInterestAmt, ldecUVHPInterestAmt, busConstant.ZERO_DECIMAL,
                                             lstrContributionType, lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.contribution_subtype_value);
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
        #endregion

        #region Process Notification Break In Service Batch
        public void ProcessNotificationBreakInServiceBatch(DataRow acdoPerson, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        {
            ArrayList aarrResult;
            Hashtable ahtbQueryBkmarks;

            int lintComputationYear = 0;

            // THere must some replacement for this in idict (the Connection String for Legacy)
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            autlPassInfo.BeginTransaction();

            //For Correspondance Purpose 
            aarrResult = new ArrayList();
            ahtbQueryBkmarks = new Hashtable();
            bool BISFlag = false;
            busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
            try
            {
                lock (iobjLock)
                {
                    aintCount++;
                    aintTotalCount++;
                    if (aintCount == 10000)
                    {
                        String lstrMsg = aintTotalCount + " : " + " Records Has Been Processed";
                        PostInfoMessage(lstrMsg);
                        aintCount = 0;
                    }
                }

                lbusPersonOverview.icdoPerson.LoadData(acdoPerson);
                lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusPersonOverview.lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson = lbusPersonOverview.icdoPerson;
                lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();

                if (lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth.IsNull() || lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth == DateTime.MinValue)
                {
                    lbusPersonOverview.lbusBenefitApplication.idecAge = 0;
                    //lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth = lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson.date_of_birth =
                    //lbusPersonOverview.icdoPerson.idtDateofBirth = lbusPersonOverview.icdoPerson.date_of_birth = DateTime.Now.AddYears(-43);
                }
                else
                {

                    lbusPersonOverview.lbusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAge(lbusPersonOverview.icdoPerson.idtDateofBirth, DateTime.Now);
                }

                lbusPersonOverview.lbusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAge(lbusPersonOverview.icdoPerson.idtDateofBirth, DateTime.Now);

                if (ldtbWorkInformationForAllParticipants.Rows.Count > 0 && ldtbWorkInformationForAllParticipants.IsNotNull() && acdoPerson[enmPerson.ssn.ToString()] != DBNull.Value)
                {
                    //PIR 1030
                    DateTime ldtOldForfeitureDateMPIPP = new DateTime();
                    DateTime ldtOldVestedDateMPIPP = new DateTime();
                    DateTime ldtNewForfeitureDateMPIPP = new DateTime();
                    DateTime ldtNewVestedDateMPIPP = new DateTime();

                    DateTime ldtOldForfeitureDateIAP = new DateTime();
                    DateTime ldtOldVestedDateIAP = new DateTime();
                    DateTime ldtNewForfeitureDateIAP = new DateTime();
                    DateTime ldtNewVestedDateIAP = new DateTime();

                    string lstrSSNDecrypted = Convert.ToString(acdoPerson[enmPerson.ssn.ToString()]);
                    DataRow[] ldrTempWorkInfo = ldtbWorkInformationForAllParticipants.FilterTable(utlDataType.String, "ssn", lstrSSNDecrypted);

                    //Temporary Commented
                    //lbusPersonOverview.lbusBenefitApplication.idrWeeklyWorkData  = busBase.Select("cdoPerson.GetWeeklyDatabySSN", new object[1] { lstrSSNDecrypted }).Select();

                    //if (ldtbWorkInformationForAllParticipantsWeekly.Rows.Count > 0 && ldtbWorkInformationForAllParticipantsWeekly.IsNotNull() && acdoPerson[enmPerson.ssn.ToString()] != DBNull.Value)
                    // lbusPersonOverview.lbusBenefitApplication.idrWeeklyWorkData = ldtbWorkInformationForAllParticipantsWeekly.FilterTable(utlDataType.String, "ssn", lstrSSNDecrypted);

                    DataRow[] ldrPersonAccounts = ldtbPersonAccounts.FilterTable(utlDataType.String, "person_id", lbusPersonOverview.icdoPerson.person_id.ToString());
                    busPersonAccount lbusPersonAccount = new busPersonAccount();
                    lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount = lbusPersonAccount.GetCollection<busPersonAccount>(ldrPersonAccounts, "icdoPersonAccount");

                    //PIR 1030
                    busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                    if (lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).Count() > 0)
                    {
                        lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id);
                        if (lbusLocalPersonAccountEligibility != null)
                        {
                            ldtOldForfeitureDateMPIPP = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                            ldtOldVestedDateMPIPP = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                        }
                    }

                    lbusLocalPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                    if (lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                    {
                        lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id);
                        if (lbusLocalPersonAccountEligibility != null)
                        {
                            ldtOldForfeitureDateIAP = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                            ldtOldVestedDateIAP = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                        }
                    }

                    if (!ldrTempWorkInfo.IsNullOrEmpty())
                    {
                        lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldrTempWorkInfo);
                        lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP = new Collection<cdoDummyWorkData>();

                        //if (lbusPersonOverview.icdoPerson.date_of_death.IsNotNull() && lbusPersonOverview.icdoPerson.date_of_death != DateTime.MinValue)
                        //    lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date = lbusPersonOverview.icdoPerson.date_of_death;
                        //else
                        lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date = DateTime.Now;

                        foreach (DataRow dr in ldrTempWorkInfo)
                        {
                            cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                            if (dr["Year"] != DBNull.Value)
                                lcdoDummyWorkData.year = Convert.ToInt32(dr["Year"].ToString());
                            else
                                lcdoDummyWorkData.year = 0;

                            if (dr["IAP_HOURS"] != DBNull.Value)
                            {
                                lcdoDummyWorkData.qualified_hours = Convert.ToDecimal(dr["IAP_HOURS"].ToString());
                                lcdoDummyWorkData.vested_hours = Convert.ToDecimal(dr["IAP_HOURS"].ToString());
                            }
                            else
                            {
                                lcdoDummyWorkData.qualified_hours = Decimal.Zero;
                                lcdoDummyWorkData.vested_hours = Decimal.Zero;
                            }

                            if (dr["CheckVesting"] != DBNull.Value)
                                lcdoDummyWorkData.CheckVesting = (dr["CheckVesting"].ToString()).ToString();

                            lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.Add(lcdoDummyWorkData);
                        }


                        if ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()) &&
                             lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count() > 0 &&
                             lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year == DateTime.Now.Year)
                        {
                            DateTime SatBeforeLastThrusday = busGlobalFunctions.GetLastDateOfComputationYear(DateTime.Now.Year);
                            if (DateTime.Now <= SatBeforeLastThrusday)
                            {
                                lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.RemoveAt(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.
                                    IndexOf(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last()));
                            }
                        }

                        if ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.IsNullOrEmpty()) &&
                             lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.Count() > 0 &&
                             lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().year == DateTime.Now.Year)
                        {
                            DateTime SatBeforeLastThrusday = busGlobalFunctions.GetLastDateOfComputationYear(DateTime.Now.Year);
                            if (DateTime.Now <= SatBeforeLastThrusday)
                            {
                                lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.RemoveAt(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.
                                    IndexOf(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last()));
                            }
                        }


                        if ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()) && lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                        {
                            lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusPersonOverview.lbusBenefitApplication.PaddingForBridgingService(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI);
                            lbusPersonOverview.lbusBenefitApplication.ProcessWorkHistoryPadding(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI, busConstant.MPIPP, true);
                            lbusPersonOverview.lbusBenefitApplication.ProcessWorkHistoryforBISandForfieture(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI, busConstant.MPIPP,
                                                                                                            lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.status_value,
                                                                                                            lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.end_date.Year);
                        }


                        if ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.IsNullOrEmpty()) && lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                        {
                            lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP = lbusPersonOverview.lbusBenefitApplication.PaddingForBridgingService(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP);
                            lbusPersonOverview.lbusBenefitApplication.ProcessWorkHistoryPadding(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP, busConstant.IAP, true);
                            lbusPersonOverview.lbusBenefitApplication.ProcessWorkHistoryforBISandForfieture(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP, busConstant.IAP,
                                                                                                            lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.status_value,
                                                                                                            lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.end_date.Year);
                        }


                        if ((((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()) && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.FirstOrDefault().CheckVesting == busConstant.FLAG_YES) ||
                             ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.IsNullOrEmpty()) && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.FirstOrDefault().CheckVesting == busConstant.FLAG_YES))
                            )
                            //                                lbusPersonOverview.lbusBenefitApplication.DetermineVesting(ablnBISBatch: true);
                            lbusPersonOverview.lbusBenefitApplication.DetermineVesting();



                        //PIR 1030
                        lbusLocalPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                        if (lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).Count() > 0)
                        {
                            lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id);
                            if (lbusLocalPersonAccountEligibility != null)
                            {
                                ldtNewForfeitureDateMPIPP = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                                ldtNewVestedDateMPIPP = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                            }
                        }

                        lbusLocalPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                        if (lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                        {
                            lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id);
                            if (lbusLocalPersonAccountEligibility != null)
                            {
                                ldtNewForfeitureDateIAP = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                                ldtNewVestedDateIAP = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                            }
                        }

                        //PIR 1030
                        if (ldtOldForfeitureDateMPIPP != ldtNewForfeitureDateMPIPP || ldtOldVestedDateMPIPP != ldtNewVestedDateMPIPP)
                        {
                            lock (iobjLock)
                            {


                                DataRow ldrNewRow = idtblVestingForfeitureChanges.NewRow();
                                ldrNewRow["MPI_PERSON_ID"] = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
                                ldrNewRow["PLAN_NAME"] = busConstant.MPIPP;

                                ldrNewRow["OLD_VESTED_DATE"] = ldtOldVestedDateMPIPP == DateTime.MinValue ? String.Empty : ldtOldVestedDateMPIPP.Date.ToString("d");
                                ldrNewRow["NEW_VESTED_DATE"] = ldtNewVestedDateMPIPP == DateTime.MinValue ? String.Empty : ldtNewVestedDateMPIPP.Date.ToString("d");
                                ldrNewRow["OLD_FORFEITURE_DATE"] = ldtOldForfeitureDateMPIPP == DateTime.MinValue ? String.Empty : ldtOldForfeitureDateMPIPP.Date.ToString("d");
                                ldrNewRow["NEW_FORFEITURE_DATE"] = ldtNewForfeitureDateMPIPP == DateTime.MinValue ? String.Empty : ldtNewForfeitureDateMPIPP.Date.ToString("d");
                                idtblVestingForfeitureChanges.Rows.Add(ldrNewRow);
                            }
                        }

                        if (ldtOldForfeitureDateIAP != ldtNewForfeitureDateIAP || ldtOldVestedDateIAP != ldtNewVestedDateIAP)
                        {
                            lock (iobjLock)
                            {

                                DataRow ldrNewRow = idtblVestingForfeitureChanges.NewRow();
                                ldrNewRow["MPI_PERSON_ID"] = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
                                ldrNewRow["PLAN_NAME"] = busConstant.IAP;

                                ldrNewRow["OLD_VESTED_DATE"] = ldtOldVestedDateIAP == DateTime.MinValue ? String.Empty : ldtOldVestedDateIAP.Date.ToString("d");
                                ldrNewRow["NEW_VESTED_DATE"] = ldtNewVestedDateIAP == DateTime.MinValue ? String.Empty : ldtNewVestedDateIAP.Date.ToString("d");
                                ldrNewRow["OLD_FORFEITURE_DATE"] = ldtOldForfeitureDateIAP == DateTime.MinValue ? String.Empty : ldtOldForfeitureDateIAP.Date.ToString("d");
                                ldrNewRow["NEW_FORFEITURE_DATE"] = ldtNewForfeitureDateIAP == DateTime.MinValue ? String.Empty : ldtNewForfeitureDateIAP.Date.ToString("d");
                                idtblVestingForfeitureChanges.Rows.Add(ldrNewRow);
                            }
                        }


                        aarrResult.Add(lbusPersonOverview);


                        if (!lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty() &&
                            lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => (i.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID || i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID) && i.icdoPersonAccount.status_value != busConstant.PERSON_ACCOUNT_STATUS_DECEASED).Count() > 0)
                        {

                            busPersonBatchFlags lbusPersonBatchFlags = new busPersonBatchFlags { icdoPersonBatchFlags = new cdoPersonBatchFlags() };

                            if (ldtPersonBatchFlag.Rows.Count > 0)
                            {
                                if (ldtPersonBatchFlag.FilterTable(utlDataType.String, "person_id", lbusPersonOverview.icdoPerson.person_id.ToString()).Count() > 0)
                                    lbusPersonBatchFlags.icdoPersonBatchFlags.LoadData(ldtPersonBatchFlag.FilterTable(utlDataType.String, "person_id", lbusPersonOverview.icdoPerson.person_id.ToString()).FirstOrDefault());
                            }


                            //Review with Abhishek
                            #region Code for BIS detection , Person Account Status TOGGLING and generation of Notification Correspondance for MPI PLAN
                            if ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty()) && lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson.date_of_death == DateTime.MinValue)
                            {
                                if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().bis_years_count >= 2
                                    || (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrBisParticipantFlag.IsNotNull() && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrBisParticipantFlag == busConstant.FLAG_YES))//Review with Abhishek
                                {
                                    if (lbusPersonBatchFlags.icdoPersonBatchFlags.person_batch_flag_id > 0)
                                    {
                                        string lstrTempOldBisFlagValue = string.Empty;

                                        if (lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag.IsNotNullOrEmpty())
                                            lstrTempOldBisFlagValue = lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag;

                                        lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_YES;
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.Update();

                                        if (lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                                        {
                                            lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.status_value = busConstant.PERSON_ACCOUNT_STATUS_INACTIVE;
                                            lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.Update();
                                        }

                                        if (string.IsNullOrEmpty(lstrTempOldBisFlagValue) ||
                                            lstrTempOldBisFlagValue == busConstant.FLAG_NO)
                                        {
                                            string str = string.Empty;

                                            if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().bis_years_count == 2
                                                && (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrForfietureFlag.IsNull() || lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrForfietureFlag != busConstant.FLAG_YES)
                                                )
                                            {
                                                //PIR-852- added below condition For: "Do not include participants who do not have a least one QY in total"                                        
                                                if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count > 0)
                                                {
                                                    if (lbusPersonOverview.icdoPerson.IS_VALID_ADDRESS_PRESENT == busConstant.FLAG_YES)
                                                    {
                                                        lbusPersonOverview.LoadCorrAddress();
                                                        if (lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES ||
                                                              ((lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.FirstOrDefault().CheckVesting.IsNull() || lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.FirstOrDefault().CheckVesting == busConstant.FLAG_NO)
                                                               && lbusPersonOverview.icdoPerson.IS_VESTED_IN_MPI == busConstant.FLAG_YES) ||
                                                            lbusPersonOverview.lbusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP))
                                                        {
                                                            str = this.CreateCorrespondence(busConstant.BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);//PROD PIR 845 - RASHMI
                                                        }
                                                        else
                                                        {
                                                            str = this.CreateCorrespondence(busConstant.NON_VESTED_BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);//PROD PIR 845 - RASHMI
                                                        }
                                                    }
                                                    lock (iobjLock)
                                                    {
                                                        //BIS Report
                                                        #region Insert Records for Reports
                                                        DataRow ldrNewRow = idtbBreakInServiceRecords.NewRow();
                                                        ldrNewRow["MPI_PERSON_ID"] = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
                                                        ldrNewRow["FIRST_NAME"] = Convert.ToString(acdoPerson["FIRST_NAME"]);
                                                        ldrNewRow["LAST_NAME"] = Convert.ToString(acdoPerson["LAST_NAME"]);
                                                        //ldrNewRow["RETIREMENT_DATE"] = Convert.ToDateTime(acdoPerson["RETIREMENT_DATE"]);
                                                        //ldrNewRow["BIS_PARTICIPANT_FLAG"] = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrBisParticipantFlag;
                                                        ldrNewRow["QUALIFIED_YEAR"] = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                                        ldrNewRow["IS_VESTED_IN_MPI"] = lbusPersonOverview.icdoPerson.IS_VESTED_IN_MPI;
                                                        ldrNewRow["DOB"] = lbusPersonOverview.icdoPerson.date_of_birth;
                                                        ldrNewRow["DOD"] = lbusPersonOverview.icdoPerson.date_of_death == DateTime.MinValue ? "" : Convert.ToString(lbusPersonOverview.icdoPerson.date_of_death);
                                                        ldrNewRow["VALID_ADDRESS_PRESENT"] = lbusPersonOverview.icdoPerson.IS_VALID_ADDRESS_PRESENT;
                                                        if ((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                             where obj.year == DateTime.Now.Year - 1
                                                             orderby obj.year descending
                                                             select obj.qualified_hours).Count() > 0)
                                                        {
                                                            ldrNewRow["LAST_YEAR_HOURS"] = Convert.ToDecimal((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                                              where obj.year == DateTime.Now.Year - 1
                                                                                                              orderby obj.year descending
                                                                                                              select obj.qualified_hours).FirstOrDefault());

                                                            ldrNewRow["SECOND_LAST_YEAR_HOURS"] = Convert.ToDecimal((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                                                     where obj.year == DateTime.Now.Year - 2
                                                                                                                     orderby obj.year descending
                                                                                                                     select obj.qualified_hours).FirstOrDefault());
                                                        }
                                                        ldrNewRow["LAST_YEAR"] = Convert.ToString(DateTime.Now.Year - 1); //RID 121740 Adding two new field
                                                        ldrNewRow["SECOND_LAST_YEAR"] = Convert.ToString(DateTime.Now.Year - 2);
                                                        idtbBreakInServiceRecords.Rows.Add(ldrNewRow);
                                                        #endregion
                                                    }
                                                }
                                            }

                                        }

                                    }
                                    else
                                    {
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.person_id = lbusPersonOverview.icdoPerson.person_id;
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_YES;
                                        string str = string.Empty;
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.Insert();
                                        if (lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                                        {
                                            lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.status_value = busConstant.PERSON_ACCOUNT_STATUS_INACTIVE;
                                            lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.Update();
                                        }

                                        if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().bis_years_count == 2
                                            && (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrForfietureFlag.IsNull() || lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrForfietureFlag != busConstant.FLAG_YES)
                                            )
                                        {
                                            //PIR-852- added below condition For: "Do not include participants who do not have a least one QY in total"                                        
                                            if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count > 0)
                                            {
                                                if (lbusPersonOverview.icdoPerson.IS_VALID_ADDRESS_PRESENT == busConstant.FLAG_YES)
                                                {
                                                    lbusPersonOverview.LoadCorrAddress();

                                                    if (lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES ||
                                                         ((lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.FirstOrDefault().CheckVesting.IsNull() || lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.FirstOrDefault().CheckVesting == busConstant.FLAG_NO)
                                                         && lbusPersonOverview.icdoPerson.IS_VESTED_IN_MPI == busConstant.FLAG_YES) ||
                                                            lbusPersonOverview.lbusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP))
                                                    {
                                                        str = this.CreateCorrespondence(busConstant.BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);//PROD PIR 845 - RASHMI
                                                    }
                                                    else
                                                    {
                                                        str = this.CreateCorrespondence(busConstant.NON_VESTED_BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);//PROD PIR 845 - RASHMI
                                                    }
                                                }

                                                lock (iobjLock)
                                                {
                                                    //BIS Report
                                                    #region Insert Records for Reports
                                                    DataRow ldrNewRow = idtbBreakInServiceRecords.NewRow();
                                                    ldrNewRow["MPI_PERSON_ID"] = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
                                                    ldrNewRow["FIRST_NAME"] = Convert.ToString(acdoPerson["FIRST_NAME"]);
                                                    ldrNewRow["LAST_NAME"] = Convert.ToString(acdoPerson["LAST_NAME"]);
                                                    //ldrNewRow["RETIREMENT_DATE"] = Convert.ToDateTime(acdoPerson["RETIREMENT_DATE"]);
                                                    //ldrNewRow["BIS_PARTICIPANT_FLAG"] = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrBisParticipantFlag;
                                                    ldrNewRow["QUALIFIED_YEAR"] = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                                    ldrNewRow["IS_VESTED_IN_MPI"] = lbusPersonOverview.icdoPerson.IS_VESTED_IN_MPI;
                                                    ldrNewRow["DOB"] = lbusPersonOverview.icdoPerson.date_of_birth;
                                                    ldrNewRow["DOD"] = lbusPersonOverview.icdoPerson.date_of_death == DateTime.MinValue ? "" : Convert.ToString(lbusPersonOverview.icdoPerson.date_of_death);
                                                    ldrNewRow["VALID_ADDRESS_PRESENT"] = lbusPersonOverview.icdoPerson.IS_VALID_ADDRESS_PRESENT;
                                                    if ((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                         where obj.year == DateTime.Now.Year - 1
                                                         orderby obj.year descending
                                                         select obj.qualified_hours).Count() > 0)
                                                    {
                                                        ldrNewRow["LAST_YEAR_HOURS"] = Convert.ToDecimal((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                                          where obj.year == DateTime.Now.Year - 1
                                                                                                          orderby obj.year descending
                                                                                                          select obj.qualified_hours).FirstOrDefault());

                                                        ldrNewRow["SECOND_LAST_YEAR_HOURS"] = Convert.ToDecimal((from obj in lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI
                                                                                                                 where obj.year == DateTime.Now.Year - 2
                                                                                                                 orderby obj.year descending
                                                                                                                 select obj.qualified_hours).FirstOrDefault());
                                                    }
                                                    ldrNewRow["LAST_YEAR"] = Convert.ToString(DateTime.Now.Year - 1); //RID 121740 Adding two new field
                                                    ldrNewRow["SECOND_LAST_YEAR"] = Convert.ToString(DateTime.Now.Year - 2);
                                                    idtbBreakInServiceRecords.Rows.Add(ldrNewRow);
                                                    #endregion
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count > 0 && lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag == busConstant.FLAG_YES)
                                    {
                                        //lbusPersonBatchFlags.icdoPersonBatchFlags.LoadData(ldtPersonBatchFlag.Rows[0]);
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_NO;
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.Update();
                                    }
                                    //changed
                                    else if (lbusPersonBatchFlags.icdoPersonBatchFlags.person_batch_flag_id > 0)
                                    {
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.person_id = lbusPersonOverview.icdoPerson.person_id;
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_NO;
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.Update();
                                    }
                                    //changed
                                    else
                                    {
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.person_id = lbusPersonOverview.icdoPerson.person_id;
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_NO;
                                        lbusPersonBatchFlags.icdoPersonBatchFlags.Insert();
                                    }

                                    if (lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                                    {
                                        lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.status_value = busConstant.PERSON_ACCOUNT_STATUS_ACTIVE;
                                        lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.Update();
                                    }

                                }
                            }

                            #endregion

                            #region Code for BIS detection , Person Account Status TOGGLING and generation of Notification Correspondance for IAP PLAN
                            if ((!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.IsNullOrEmpty()) && lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson.date_of_death == DateTime.MinValue)
                            {
                                if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().bis_years_count >= 2
                                    || (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().istrBisParticipantFlag.IsNotNull() && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP.Last().istrBisParticipantFlag == busConstant.FLAG_YES))//Review with Abhishek
                                {
                                    if (lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                                    {
                                        lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.status_value = busConstant.PERSON_ACCOUNT_STATUS_INACTIVE;
                                        lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.Update();
                                    }
                                }
                                else
                                {
                                    if (lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                                    {
                                        lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.status_value = busConstant.PERSON_ACCOUNT_STATUS_ACTIVE;
                                        lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.Update();
                                    }
                                }
                            }
                            #endregion

                            //Need to test this with some People
                            #region  COmmented Code 4 Yearly Forfeiture Batch to POST NEGATIVE ENTRIES FOR THE PERSON

                            //if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNotNull() && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count() > 0 && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrForfietureFlag.IsNotNull() && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrForfietureFlag == busConstant.FLAG_YES)
                            //{
                            //    if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().istrForfietureFlag==busConstant.FLAG_YES)
                            //    {
                            //        DataTable ldtbIAPAllocationDetail = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPRetriementContribution", new object[1] { lbusPersonOverview.icdoPerson.person_id });
                            //        if (ldtbIAPAllocationDetail.Rows.Count > 0)
                            //        {
                            //            lintComputationYear = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().year;

                            //            #region Making Negative Entries in Retirement Contribution Due to FORFEITURE
                            //            if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc1.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc1.ToString()]) != busConstant.ZERO_DECIMAL)
                            //            {
                            //                busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //                                                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            //                                                                            lintComputationYear,
                            //                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc1.ToString()]),
                            //                                                                            astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //                                                                            astrContributionType: busConstant.RCContributionTypeAllocation1);
                            //            }

                            //            if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2.ToString()]) != busConstant.ZERO_DECIMAL)
                            //            {
                            //                busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //                                                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            //                                                                            lintComputationYear,
                            //                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2.ToString()]),
                            //                                                                            astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //                                                                            astrContributionType: busConstant.RCContributionTypeAllocation2);
                            //            }

                            //            if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_invt.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_invt.ToString()]) != busConstant.ZERO_DECIMAL)
                            //            {
                            //                busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //                                                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            //                                                                            lintComputationYear,
                            //                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_invt.ToString()]),
                            //                                                                            astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //                                                                            astrContributionType: busConstant.RCContributionTypeAllocation2,
                            //                                                                            astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
                            //            }

                            //            if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_frft.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_frft.ToString()]) != busConstant.ZERO_DECIMAL)
                            //            {
                            //                busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //                                                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            //                                                                            lintComputationYear,
                            //                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc2_frft.ToString()]),
                            //                                                                            astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //                                                                            astrContributionType: busConstant.RCContributionTypeAllocation2,
                            //                                                                            astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
                            //            }

                            //            if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc3.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc3.ToString()]) != busConstant.ZERO_DECIMAL)
                            //            {
                            //                busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //                                                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            //                                                                            lintComputationYear,
                            //                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc3.ToString()]),
                            //                                                                            astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //                                                                            astrContributionType: busConstant.RCContributionTypeAllocation3);
                            //            }

                            //            if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4.ToString()]) != busConstant.ZERO_DECIMAL)
                            //            {
                            //                busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //                                                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            //                                                                            lintComputationYear,
                            //                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4.ToString()]),
                            //                                                                            astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //                                                                            astrContributionType: busConstant.RCContributionTypeAllocation4);
                            //            }

                            //            if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_frft.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_frft.ToString()]) != busConstant.ZERO_DECIMAL)
                            //            {
                            //                busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //                                                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            //                                                                            lintComputationYear,
                            //                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_frft.ToString()]),
                            //                                                                            astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //                                                                            astrContributionType: busConstant.RCContributionTypeAllocation4,
                            //                                                                            astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
                            //            }

                            //            if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_invt.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_invt.ToString()]) != busConstant.ZERO_DECIMAL)
                            //            {
                            //                busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //                                                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            //                                                                            lintComputationYear,
                            //                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc4_invt.ToString()]),
                            //                                                                            astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //                                                                            astrContributionType: busConstant.RCContributionTypeAllocation4,
                            //                                                                            astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
                            //            }

                            //            //if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_affl.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_affl.ToString()]) != busConstant.ZERO_DECIMAL)
                            //            //{
                            //            //    busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //            //    lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //            //                                                                busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //            //                                                                iobjSystemManagement.icdoSystemManagement.batch_date,
                            //            //                                                                lintComputationYear,
                            //            //                                                                adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_affl.ToString()]),
                            //            //                                                                astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //            //                                                                astrContributionType: busConstant.RCContributionTypeAllocation5,
                            //            //                                                                astrContributionSubtype: busConstant.RCContributionSubTypeAffiliates);
                            //            //}

                            //            //if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_both.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_both.ToString()]) != busConstant.ZERO_DECIMAL)
                            //            //{
                            //            //    busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //            //    lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //            //                                                                busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //            //                                                                iobjSystemManagement.icdoSystemManagement.batch_date,
                            //            //                                                                lintComputationYear,
                            //            //                                                                adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_both.ToString()]),
                            //            //                                                                astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //            //                                                                astrContributionType: busConstant.RCContributionTypeAllocation5,
                            //            //                                                                astrContributionSubtype: busConstant.RCContributionSubTypeBoth);
                            //            //}

                            //            //if (ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_nonaffl.ToString()] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_nonaffl.ToString()]) != busConstant.ZERO_DECIMAL)
                            //            //{
                            //            //    busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //            //    lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //            //                                                                busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //            //                                                                iobjSystemManagement.icdoSystemManagement.batch_date,
                            //            //                                                                lintComputationYear,
                            //            //                                                                adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0][enmIapallocationDetailPersonoverview.alloc5_nonaffl.ToString()]),
                            //            //                                                                astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //            //                                                                astrContributionType: busConstant.RCContributionTypeAllocation5,
                            //            //                                                                astrContributionSubtype: busConstant.RCContributionSubTypeNonAffiliates);
                            //            //}

                            //            if (ldtbIAPAllocationDetail.Rows[0]["alloc5"] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["alloc5"]) != busConstant.ZERO_DECIMAL)
                            //            {
                            //                busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //                                                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            //                                                                            lintComputationYear,
                            //                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["alloc5"]),
                            //                                                                            astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //                                                                            astrContributionType: busConstant.RCContributionTypeAllocation5);
                            //            }

                            //            if (ldtbIAPAllocationDetail.Rows[0]["L52ALLOC1"] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["L52ALLOC1"]) != busConstant.ZERO_DECIMAL)
                            //            {
                            //                busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //                                                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            //                                                                            lintComputationYear,
                            //                                                                            adec52SplAccountBalance: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["L52ALLOC1"]),
                            //                                                                            astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //                                                                            astrContributionType: busConstant.RCContributionTypeAllocation1);
                            //            }

                            //            if (ldtbIAPAllocationDetail.Rows[0]["L161ALLOC1"] != DBNull.Value && Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["L161ALLOC1"]) != busConstant.ZERO_DECIMAL)
                            //            {
                            //                busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
                            //                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(ldtbIAPAllocationDetail.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]),
                            //                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                            //                                                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            //                                                                            lintComputationYear,
                            //                                                                            adec161SplAccountBalance: busConstant.ZERO_DECIMAL - Convert.ToDecimal(ldtbIAPAllocationDetail.Rows[0]["L161ALLOC1"]),
                            //                                                                            astrTransactionType: busConstant.RCTransactionTypeForfeiture,
                            //                                                                            astrContributionType: busConstant.RCContributionTypeAllocation1);
                            //            }
                            //            #endregion
                            //        }
                            //    }
                            //}
                            #endregion
                        }

                    }
                }

                #region Dont delete Older Code (by Abhishek)
                //if (!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                //{
                //    #region Code for BIS detection and generation of Notification Correspondance if NOT already sent to Person - Updating the TABLE OF BATCH FLAGS -- CODE SHOULD BE OPTIMIZED
                //    if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count >= 2
                //        && lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI[lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IndexOf(lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last()) - 1].vested_hours < 200)
                //    {
                //        BISFlag = true;
                //    }

                //    if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_hours < 200 && BISFlag)
                //    {
                //        int lintVestedCount = 0;
                //        DataTable ldtblVestedCount = busPerson.Select("cdoPersonAccountEligibility.CheckIfPersonIsVested", new object[1] { lbusPersonOverview.icdoPerson.person_id });
                //        if (ldtblVestedCount.Rows.Count > 0 && Convert.ToString(ldtblVestedCount.Rows[0][0]).IsNotNullOrEmpty())
                //        {
                //            lintVestedCount = Convert.ToInt32(ldtblVestedCount.Rows[0][0]);
                //        }

                //        if (ldtPersonBatchFlag.Rows.Count > 0)
                //        {
                //            lbusPersonBatchFlags.icdoPersonBatchFlags.LoadData(ldtPersonBatchFlag.Rows[0]);
                //            if (string.IsNullOrEmpty(lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag) ||
                //                lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag == busConstant.FLAG_NO)
                //            {
                //                string str = string.Empty;

                //                if (ldtblVestedCount.Rows.Count > 0 && Convert.ToInt32(ldtblVestedCount.Rows[0][0]) > 0)
                //                {
                //                    str = this.CreateCorrespondence(busConstant.BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                //                }
                //                else
                //                {
                //                    str = this.CreateCorrespondence(busConstant.NON_VESTED_BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                //                }

                //            }
                //            lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_YES;
                //            lbusPersonBatchFlags.icdoPersonBatchFlags.Update();
                //        }
                //        else
                //        {
                //            lbusPersonBatchFlags.icdoPersonBatchFlags.person_id = lbusPersonOverview.icdoPerson.person_id;
                //            lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_YES;
                //            string str = string.Empty;

                //            if (lintVestedCount > 0)
                //            {
                //                str = this.CreateCorrespondence(busConstant.BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                //            }
                //            else
                //            {
                //                str = this.CreateCorrespondence(busConstant.NON_VESTED_BREAK_IN_SERVICE_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                //            }

                //            lbusPersonBatchFlags.icdoPersonBatchFlags.Insert();

                //        }
                //    }
                //    else
                //    {

                //        if (ldtPersonBatchFlag.Rows.Count > 0 && lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag == busConstant.FLAG_YES)
                //        {

                //            lbusPersonBatchFlags.icdoPersonBatchFlags.LoadData(ldtPersonBatchFlag.Rows[0]);
                //            lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_NO;
                //            lbusPersonBatchFlags.icdoPersonBatchFlags.Update();
                //        }
                //        else
                //        {
                //            lbusPersonBatchFlags.icdoPersonBatchFlags.person_id = lbusPersonOverview.icdoPerson.person_id;
                //            lbusPersonBatchFlags.icdoPersonBatchFlags.bis_flag = busConstant.FLAG_NO;
                //            lbusPersonBatchFlags.icdoPersonBatchFlags.Insert();

                //        }
                //    }
                //    #endregion


                //}
                #endregion

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

        #region BIS Report Table Schema
        public DataTable createTableDesignForBreakInServiceReport()
        {
            idtbBreakInServiceRecords = new DataTable();
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("FIRST_NAME", typeof(string)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("LAST_NAME", typeof(string)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("PARTICIPANT_NAME", typeof(string)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("BIS_FLAG", typeof(string)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("QUALIFIED_YEAR", typeof(int)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("BIS_PARTICIPANT_FLAG", typeof(string)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("RETIREMENT_DATE", typeof(DateTime)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("DOB", typeof(DateTime)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("DOD", typeof(string)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("LAST_YEAR_HOURS", typeof(decimal)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("SECOND_LAST_YEAR_HOURS", typeof(decimal)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("VALID_ADDRESS_PRESENT", typeof(string)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("IS_VESTED_IN_MPI", typeof(string)));
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("LAST_YEAR", typeof(string))); //RID 121740 Adding two new field
            idtbBreakInServiceRecords.Columns.Add(new DataColumn("SECOND_LAST_YEAR", typeof(string)));
            return idtbBreakInServiceRecords;
        }
        public DataTable idtbBreakInServiceRecords { get; set; }

        public DataTable idtblVestingForfeitureChanges { get; set; }

        public DataTable createTableDesignForVestingForfeitureChanges()
        {
            idtblVestingForfeitureChanges = new DataTable();
            idtblVestingForfeitureChanges.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            idtblVestingForfeitureChanges.Columns.Add(new DataColumn("PLAN_NAME", typeof(string)));
            idtblVestingForfeitureChanges.Columns.Add(new DataColumn("OLD_VESTED_DATE", typeof(string)));
            idtblVestingForfeitureChanges.Columns.Add(new DataColumn("OLD_FORFEITURE_DATE", typeof(string)));
            idtblVestingForfeitureChanges.Columns.Add(new DataColumn("NEW_VESTED_DATE", typeof(string)));
            idtblVestingForfeitureChanges.Columns.Add(new DataColumn("NEW_FORFEITURE_DATE", typeof(string)));
            return idtblVestingForfeitureChanges;
        }


        #endregion

        #region MD_PARTICIPANT Report Table Schema
        public DataTable createTableDesignForMDParticipantsReport() //PROD PIR 854 - MD_BATCH_REPORT
        {
            idtbMDParticipantsRecords = new DataTable();
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            //Ticket 79238 - Break up Full Participant Name into First and Last.
            //idtbMDParticipantsRecords.Columns.Add(new DataColumn("PARTICIPANT_NAME", typeof(string)));
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("LAST_NAME", typeof(string)));
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("FIRST_NAME", typeof(string)));
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("DOB", typeof(DateTime)));
            //idtbMDParticipantsRecords.Columns.Add(new DataColumn("AGE", typeof(decimal)));
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("YEAR_70_6", typeof(int)));
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("MD_DATE", typeof(DateTime)));
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("QLFD_YR_COUNT", typeof(int)));
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("VESTED_DATE", typeof(DateTime)));
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("PLAN", typeof(string)));
            //Ticket 79238 - Add Calc ID column
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("CALC_ID", typeof(int)));
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("COMPUTATION_YR", typeof(string)));
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("IS_VALID_ADDRESS", typeof(string))); //PIR 861
            //RID 118418 Added column for MD batch enhancement
            idtbMDParticipantsRecords.Columns.Add(new DataColumn("RMD73_DATE", typeof(DateTime)));  //WI 23550 Ticket 143336 RMD72Date to RMD73Date
            return idtbMDParticipantsRecords;
        }
        public DataTable idtbMDParticipantsRecords { get; set; }
        #endregion

        #region MD_PARTICIPANT_ADDRESS Report Table Schema
        //Ticket 79238
        public DataTable createTableDesignForMDParticipantAddressReport()
        {
            idtbMDParticipantAddressRecords = new DataTable();
            idtbMDParticipantAddressRecords.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            idtbMDParticipantAddressRecords.Columns.Add(new DataColumn("LAST_NAME", typeof(string)));
            idtbMDParticipantAddressRecords.Columns.Add(new DataColumn("FIRST_NAME", typeof(string)));

            idtbMDParticipantAddressRecords.Columns.Add(new DataColumn("ADDR_LINE_1", typeof(string)));
            idtbMDParticipantAddressRecords.Columns.Add(new DataColumn("ADDR_LINE_2", typeof(string)));
            idtbMDParticipantAddressRecords.Columns.Add(new DataColumn("ADDR_CITY", typeof(string)));
            idtbMDParticipantAddressRecords.Columns.Add(new DataColumn("ADDR_STATE_VALUE", typeof(string)));
            idtbMDParticipantAddressRecords.Columns.Add(new DataColumn("ADDR_ZIP_CODE", typeof(string)));
            return idtbMDParticipantAddressRecords;
        }
        public DataTable idtbMDParticipantAddressRecords { get; set; }
        #endregion  

        #region AnnualBenefitSummaryCorrespondenceBatch PIR 1003
        public void AnnualBenefitSummaryCorrespondenceBatch()
        {
            iobjPassInfo.idictParams["ID"] = "AnnualBenefitSummaryCorrespondenceBatch";
            DataTable ldtParticipants = busBase.Select("cdoPerson.GetParticipantsForAnnualBatch",
                new object[0] { });

            if (ldtParticipants != null && ldtParticipants.Rows.Count > 0)
            {
                foreach (DataRow ldrParticipants in ldtParticipants.Rows)
                {

                    if (Convert.ToString(ldrParticipants[enmPerson.ssn.ToString().ToUpper()]).IsNotNullOrEmpty())
                    {
                        busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                        int lintPersonId = lbusPerson.GetPersonIDFromSSN(Convert.ToString(ldrParticipants[enmPerson.ssn.ToString().ToUpper()]));
                        if (lbusPerson.FindPerson(lintPersonId))
                        {
                            lbusPerson.icdoPerson.recalculate_vesting_flag = busConstant.FLAG_YES;
                            lbusPerson.icdoPerson.Update();
                        }

                        try
                        {
                            busAnnualBenefitSummaryOverview lobjbusAnnualBenefitSummaryOverview = new busAnnualBenefitSummaryOverview();

                            if (lobjbusAnnualBenefitSummaryOverview.FindPerson(lintPersonId))
                            {
                                lobjbusAnnualBenefitSummaryOverview.LoadWorkHistory();
                                lobjbusAnnualBenefitSummaryOverview.GetTotalHours();

                                ArrayList aarrResult = new ArrayList();
                                Hashtable ahtbQueryBkmarks = new Hashtable();

                                aarrResult.Add(lobjbusAnnualBenefitSummaryOverview);
                                this.CreateCorrespondence(busConstant.ANNUAL_BENEFIT_SUMMARY_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true);
                                aarrResult.Clear();
                            }
                        }
                        catch (Exception e)
                        {
                            ExceptionManager.Publish(e);
                            String lstrMsg = "Error while Executing Batch,Error Message For MPID " + lbusPerson.icdoPerson.mpi_person_id + ":" + e.ToString();
                            PostErrorMessage(lstrMsg);
                        }

                    }
                }
            }

            MergePDFs(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_Annual_Benefit_Correspondence_Temp);
        }

        private void MergePDFs(string generatedpath)
        {

            DirectoryInfo dir = new DirectoryInfo(generatedpath);
            List<string> filesPath = new List<string>();
            foreach (FileInfo fi in dir.GetFiles("*.*").Where(t => t.CreationTime.Date == DateTime.Today.Date &&
                (t.FullName.Contains(busConstant.ANNUAL_BENEFIT_SUMMARY_LETTER) || t.FullName.Contains(busConstant.RETIREMENT_AFFIDAVIT_COVER_LETTER))).OrderBy(item => item.CreationTime))
            {
                if (fi.CreationTime > ibusJobHeader.icdoJobHeader.start_time)
                    filesPath.Add(fi.FullName);
            }

            // Get selected file path contains pdf files
            List<string> SelectedfilesPath = new List<string>();
            SelectedfilesPath = filesPath.Where(obj => obj.Contains(".pdf")).ToList();
            // validate SelectedfilesPath is null or empty
            if (SelectedfilesPath != null && SelectedfilesPath.Count > 0)
            {
                List<PdfReader> readerList = new List<PdfReader>();
                foreach (string filePath in SelectedfilesPath)
                {
                    PdfReader pdfReader = new PdfReader(filePath);
                    readerList.Add(pdfReader);
                }

                // Declare/Define output file path                


                Console.WriteLine("Merging Files");
                iTextSharp.text.Document document = null;
                PdfWriter writer = null;
                int iintPrintingCount = 0;
                foreach (PdfReader reader in readerList)
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        // validating to generate 5000 pages in one pdf file
                        if (iintPrintingCount == 0)
                        {
                            //Add datetime to output file path
                            string outPutFilePath = iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_Annual_Benefit_Correspondence;
                            if (!Directory.Exists(outPutFilePath))
                            {
                                Directory.CreateDirectory(outPutFilePath);
                            }
                            outPutFilePath = outPutFilePath + "AnnualBenefitSummaryCorrespondenceBatch-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                            //Define a new output document and its size, type
                            document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                            //Create blank output pdf file and get the stream to write on it.                            
                            writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create));
                            document.Open();
                        }

                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        document.Add(iTextSharp.text.Image.GetInstance(page));

                        iintPrintingCount++;
                        if (iintPrintingCount == 5000)
                        {
                            document.Close();
                            iintPrintingCount = 0;
                        }
                    }
                }
                if (document.IsOpen())
                {
                    document.Close();
                }
                Console.WriteLine("Closing Files");
            }

            if (Directory.Exists(generatedpath))
            {
                Directory.Delete(generatedpath, true);
            }
        }
        #endregion AnnualBenefitSummaryCorrespondenceBatch PIR 1003
        //Ticket#72507
        public DateTime GetLastWorkingDate(string astrSSN)
        {
            DateTime ldtLastWorkingDate = new DateTime();
            IDbConnection lconLegacy = DBFunction.GetDBConnection("Legacy");
            if (lconLegacy != null)
            {
                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
                IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
                lobjParameter.ParameterName = "@SSN";
                lobjParameter.DbType = DbType.String;
                lobjParameter.Value = astrSSN;
                lcolParameters.Add(lobjParameter); ;
                DataTable ldataTable = new DataTable();

                IDataReader lDataReader = DBFunction.DBExecuteProcedureResult("usp_GetLastWorkingDate", lcolParameters, lconLegacy, null);
                if (lDataReader != null)
                {
                    ldataTable.Load(lDataReader);
                    if (ldataTable.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(ldataTable.Rows[0][0])))
                        {
                            ldtLastWorkingDate = Convert.ToDateTime(ldataTable.Rows[0][0]);
                        }
                    }
                }
            }
            return ldtLastWorkingDate;
        }



        public DataTable createTableDesignForPensionEligibility()
        {
            idtbPensionEligibility = new DataTable();
            idtbPensionEligibility.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            idtbPensionEligibility.Columns.Add(new DataColumn("NAME", typeof(string)));
            idtbPensionEligibility.Columns.Add(new DataColumn("ADDRESS1", typeof(string))); //rid 124432
            idtbPensionEligibility.Columns.Add(new DataColumn("ADDRESS2", typeof(string))); //rid 124432
            idtbPensionEligibility.Columns.Add(new DataColumn("CITY", typeof(string))); //rid 124432
            idtbPensionEligibility.Columns.Add(new DataColumn("STATE", typeof(string))); //rid 124432
            idtbPensionEligibility.Columns.Add(new DataColumn("ZIPCODE", typeof(string))); //rid 124432
            idtbPensionEligibility.Columns.Add(new DataColumn("EMAIL_ADDRESS", typeof(string))); //rid 124432
            idtbPensionEligibility.Columns.Add(new DataColumn("DATE_of_Birth", typeof(DateTime)));
            idtbPensionEligibility.Columns.Add(new DataColumn("AGE", typeof(decimal)));
            idtbPensionEligibility.Columns.Add(new DataColumn("RETIREMENT_DATE", typeof(DateTime)));
            idtbPensionEligibility.Columns.Add(new DataColumn("MPI_Accrued_Benefit", typeof(string)));
            idtbPensionEligibility.Columns.Add(new DataColumn("IAP_Balance", typeof(string)));
            idtbPensionEligibility.Columns.Add(new DataColumn("EE_UVHP", typeof(string)));
            idtbPensionEligibility.Columns.Add(new DataColumn("QY_Total", typeof(int)));
            idtbPensionEligibility.Columns.Add(new DataColumn("Vested_Date", typeof(DateTime)));
            idtbPensionEligibility.Columns.Add(new DataColumn("Vested_Status", typeof(string)));
            idtbPensionEligibility.Columns.Add(new DataColumn("BIS", typeof(string)));
            idtbPensionEligibility.Columns.Add(new DataColumn("Local_Plan", typeof(string)));
            //76227
            idtbPensionEligibility.Columns.Add(new DataColumn("BAD_ADDRESS_FLAG", typeof(string)));
            idtbPensionEligibility.Columns.Add(new DataColumn("FIRST_NAME", typeof(string)));
            idtbPensionEligibility.Columns.Add(new DataColumn("LAST_NAME", typeof(string)));

            return idtbPensionEligibility;
        }
        public DataTable idtbPensionEligibility { get; set; }
    }
}

#endregion