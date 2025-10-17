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
using Microsoft.Reporting.WinForms;
using System.IO;

namespace MPIPHPJobService
{
    public class busLateIAPAllocationBatch : busBatchHandler
    {
        public DataTable idtLateHoursAndContributions { get; set; }
        //property to contain previous year iap allocation summary
        public busIapAllocationSummary ibusPrevYearAllocationSummary { get; set; }
        public DataTable idtIAPPersonAccounts { get; set; }

        public DateTime idtEACDate { get; set; }
        public bool lisLateHourBatch { get; set; } 

        private object iobjLock = null;
        private int iintRecordCount = 0;
        private int iintTotalCount = 0;

        public override void Process()
        {
            try
            {
                LoadPreviousYearAllocationSummary();
                //PIR 628
                //LoadLateHoursAndContributions();
                //ProcessLateAllocation();
                LoadAndProcessLateHoursAllocation();
                CreateReportForLateIAPBatch();
               
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                String lstrMsg = "Error while executing the batch: " + ex.ToString();
                PostErrorMessage(lstrMsg);
            }
        }

        //PIR 628
        private void LoadAndProcessLateHoursAllocation()
        {
            if (ibusPrevYearAllocationSummary == null)
                LoadPreviousYearAllocationSummary();

           
            //pir-859
            int lintComputationYear = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.computation_year + 1;
            

            DateTime ldtEACutOffDate = new DateTime();
            DataTable ldtGetEACutoffDate = busBase.Select("cdoIapAllocationDetail.GetEACutoffDate", new object[1] { lintComputationYear });//PIR 628 New
            if (ldtGetEACutoffDate != null && ldtGetEACutoffDate.Rows.Count > 0 && Convert.ToString(ldtGetEACutoffDate.Rows[0][0]).IsNotNullOrEmpty())
            {
                ldtEACutOffDate = Convert.ToDateTime(ldtGetEACutoffDate.Rows[0][0]);
            }

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();

            //SqlParameter[] LateHourparameters = new SqlParameter[2];//pir-859

            //SqlParameter LateHourparam1 = new SqlParameter("@BatchRunDate", DbType.DateTime);
            //LateHourparam1.Value = iobjSystemManagement.icdoSystemManagement.batch_date;// DateTime.Now;
            //LateHourparameters[0] = LateHourparam1;

            ////PIR-859
            //SqlParameter LateHourparam2 = new SqlParameter("@BATCHNAME", DbType.String);
            //LateHourparam2.Value = busConstant.LATE_IAP_ALLOCATION_BATCH;
            //LateHourparameters[1] = LateHourparam2;            

            //DataTable ldtLateHoursAndContributions = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_PensionLateHours", lstrLegacyDBConnetion, null, LateHourparameters);

            DataTable ldtLateHoursAndContributions = busBase.Select("cdoIapAllocationDetail.GetPensionLateHours", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });


            if (ldtLateHoursAndContributions != null && ldtLateHoursAndContributions.Rows.Count > 0)
            {
                DataRow[] ldrLateHoursAndContributions = ldtLateHoursAndContributions.Select();
                //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
                Parallel.ForEach(ldrLateHoursAndContributions.AsEnumerable(), po, (acdoPerson, loopState) =>
                {
                    utlPassInfo lobjPassInfo = null;
                    try
                    {
                        lobjPassInfo = new utlPassInfo();
                        lobjPassInfo.idictParams = ldictParams;
                        lobjPassInfo.idictParams["ID"] = "LateIAPAllocation-Batch";
                        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                        utlPassInfo.iobjPassInfo = lobjPassInfo;

                        lock (iobjLock)
                        {
                            iintRecordCount++;
                            iintTotalCount++;
                            if (iintRecordCount == 100)
                            {
                                String lstrMsg = iintTotalCount + " : " + " Records Has Been Processed";
                                PostInfoMessage(lstrMsg);
                                iintRecordCount = 0;
                            }
                        }
                        lobjPassInfo.BeginTransaction();

                        //         IAP Enhancement item #8
                        // Mazher: - Please uncomment this method 
                        //  GetEffectiveDatefromApplication(Convert.ToInt32(acdoPerson[enmPerson.person_id.ToString()]));

                        busIAPAllocationDetailPersonOverview lbusIAPAllocationDetailPersonOverview = new busIAPAllocationDetailPersonOverview();
                        lbusIAPAllocationDetailPersonOverview.iblnInLateBatch = true;
                        lbusIAPAllocationDetailPersonOverview.LoadRecalculateAndPostIAPAllocationDetail(Convert.ToInt32(acdoPerson[enmPerson.person_id.ToString()]),
                         ldtEACutOffDate, Convert.ToInt32(acdoPerson[enmPersonAccountRetirementContribution.computational_year.ToString().ToUpper()]));//PIR 628 Extended

                        lobjPassInfo.Commit();
                    }
                    catch (Exception ex)
                    {
                        lock (iobjLock)
                        {
                            ExceptionManager.Publish(ex);
                            String lstrMsg = string.Empty;
                            string lstrMPID = Convert.ToString(acdoPerson[enmPerson.mpi_person_id.ToString()]);
                            if (!string.IsNullOrEmpty(lstrMPID))
                                lstrMsg = "Error while executing the batch for person " + Convert.ToString(lstrMPID) + " : " + ex.ToString();
                            else
                                lstrMsg = "Error while executing the batch : " + ex.ToString();

                            PostErrorMessage(lstrMsg);
                        }
                        if (lobjPassInfo != null)
                            lobjPassInfo.Rollback();
                    }
                    finally
                    {
                        if (lobjPassInfo != null)
                        {
                            if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                            {
                                lobjPassInfo.iconFramework.Close();
                            }
                            lobjPassInfo.iconFramework.Dispose();
                            lobjPassInfo.iconFramework = null;
                            lobjPassInfo = null;
                        }
                    }
                });

                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;
            }
        }

        private void CreateReportForLateIAPBatch()
        {
            DataTable ldtrptLateIAPReport = busBase.Select("cdoIapAllocationDetail.rptLateIAPBatch", new object[1] { iobjPassInfo.istrUserID });
            if (ldtrptLateIAPReport.IsNotNull() && ldtrptLateIAPReport.Rows.Count > 0)
            {
                try
                {
                    ldtrptLateIAPReport.TableName = "ReportTable01";
                    CreatePDFReport(ldtrptLateIAPReport, "rpt26_LateIAPAllocationBatch");
                    //Ticket#127241
                    CreateExcelReport(ldtrptLateIAPReport, "rpt26_LateIAPAllocationBatch");
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                }
            }
        }


        #region Create Excel Report
        private string CreateExcelReport(DataTable ldtbResultTable, string astrReportName, string astrPrefix = "")
        {

            ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            DataTable ldtbReportTable = ldtbResultTable;

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";
            ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);

            rvViewer.LocalReport.DataSources.Add(lrdsReport);

            byte[] bytes = rvViewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string labsRptGenPath = string.Empty;
            labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);

            string lstrReportFullName = string.Empty;

            if (astrReportName == "rpt26_LateIAPAllocationBatch")
            {
                lstrReportFullName = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + "rpt26_LateIAPAllocationBatch" + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            }
            else
            {
                if (astrPrefix.IsNotNullOrEmpty())
                    lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
                else
                {
                    lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
                }
            }

            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            return lstrReportFullName;
        }
        #endregion
        //         IAP Enhancement item #8
        //private void GetEffectiveDatefromApplication(int lperson_id)
        //{
        //    idtEACDate = DateTime.MinValue;
        //    DataTable ldtEffectiveDatefromApplication = busBase.Select("cdoIapAllocationDetail.GetEffectiveDatefromApplication", new object[1] { lperson_id });

        //    if(ldtEffectiveDatefromApplication != null && ldtEffectiveDatefromApplication.Rows.Count > 0 && Convert.ToString(ldtEffectiveDatefromApplication.Rows[0][0]).IsNotNullOrEmpty())
        //    {
        //       if(ldtEffectiveDatefromApplication.Rows[0]["RETIREMENT_DATE"] != DBNull.Value)
        //        {
        //            idtEACDate = Convert.ToDateTime(ldtEffectiveDatefromApplication.Rows[0]["RETIREMENT_DATE"]);
        //           // lisLateHourBatch = true;
        //        }
        //        else if (ldtEffectiveDatefromApplication.Rows[0]["WITHDRAWAL_DATE"] != DBNull.Value)
        //        {
        //            idtEACDate = Convert.ToDateTime(ldtEffectiveDatefromApplication.Rows[0]["WITHDRAWAL_DATE"]);
        //            //lisLateHourBatch = true;
        //        }

        //    }
        //}

        private void LoadPreviousYearAllocationSummary()
        {
            ibusPrevYearAllocationSummary = new busIapAllocationSummary();
            ibusPrevYearAllocationSummary.LoadLatestAllocationSummary();
        }

        #region PIR 628 Commneted Code
        /* /// <summary>
        /// Method to load Late hours/contributions
        /// </summary>
        private void LoadLateHoursAndContributions()
        {
            idtLateHoursAndContributions = new DataTable();
            idtLateHoursAndContributions.Columns.Add("empaccountno", Type.GetType("System.Int32"));
            idtLateHoursAndContributions.Columns.Add("computationyear", Type.GetType("System.Int32"));
            idtLateHoursAndContributions.Columns.Add("ssn", Type.GetType("System.String"));
            idtLateHoursAndContributions.Columns.Add("person_account_id", Type.GetType("System.Int32"));
            idtLateHoursAndContributions.Columns.Add("person_id", Type.GetType("System.Int32"));
            idtLateHoursAndContributions.Columns.Add("iaphours", Type.GetType("System.Decimal"));
            idtLateHoursAndContributions.Columns.Add("iaphoursa2", Type.GetType("System.Decimal"));
            idtLateHoursAndContributions.Columns.Add("iappercent", Type.GetType("System.Decimal"));
            idtLateHoursAndContributions.Columns.Add("lateiaphours", Type.GetType("System.Decimal"));
            idtLateHoursAndContributions.Columns.Add("lateiaphoursa2", Type.GetType("System.Decimal"));
            idtLateHoursAndContributions.Columns.Add("lateiappercent", Type.GetType("System.Decimal"));

            if (ibusPrevYearAllocationSummary == null)
                LoadPreviousYearAllocationSummary();

            if (idtIAPPersonAccounts == null || idtIAPPersonAccounts.Rows.Count == 0)
                LoadAllIAPPersonAccount();

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            //
            //SqlParameter[] lsqlParameters = new SqlParameter[4];
            //SqlParameter lsqlParam1 = new SqlParameter("@FROMDATE", DbType.DateTime);
            //SqlParameter lsqlParam2 = new SqlParameter("@TODATE", DbType.DateTime);
            //SqlParameter lsqlParam3 = new SqlParameter("@COMPUTATIONYEAR", DbType.Int32);
            //SqlParameter lsqlParam4 = new SqlParameter("@ORDERBY", DbType.String);
            
            //lsqlParam1.Value = busGlobalFunctions.GetFirstPayrollDayOfMonth(iobjSystemManagement.icdoSystemManagement.batch_date.Year,iobjSystemManagement.icdoSystemManagement.batch_date.Month);
            //lsqlParam2.Value = busGlobalFunctions.GetLastPayrollDayOfMonth(iobjSystemManagement.icdoSystemManagement.batch_date.Year, iobjSystemManagement.icdoSystemManagement.batch_date.Month);
            //lsqlParam3.Value = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.computation_year + 1;
            //lsqlParam4.Value = busConstant.FLAG_NO;

            //lsqlParameters[0] = lsqlParam1;
            //lsqlParameters[1] = lsqlParam2;
            //lsqlParameters[2] = lsqlParam3;
            //lsqlParameters[3] = lsqlParam4;
            ////stored procedure to take all late hours and contributions
            ////idtLateHoursAndContributions = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetLateHoursAndContributions", lstrLegacyDBConnetion, lsqlParameters);

            //DataTable ldtLateHoursAndContributions = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPSnapShotInfo", lstrLegacyDBConnetion, lsqlParameters);//
            //pir-859
            int lintComputationYear = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.computation_year + 1;
            DateTime ldtEACutOffDate = new DateTime();
            DataTable ldtGetEACutoffDate = busBase.Select("cdoIapAllocationDetail.GetEACutoffDate", new object[1] { ibusPrevYearAllocationSummary.icdoIapAllocationSummary.computation_year });
            if (ldtGetEACutoffDate != null && ldtGetEACutoffDate.Rows.Count > 0 && Convert.ToString(ldtGetEACutoffDate.Rows[0][0]).IsNotNullOrEmpty())
            {
                ldtEACutOffDate = Convert.ToDateTime(ldtGetEACutoffDate.Rows[0][0]);
            }

            DataTable ldtActivePersonWithIAPPlan = busBase.Select("cdoPersonAccount.GetActivePersonWithIAPPlan", new object[] { });

            SqlParameter[] LateHourparameters = new SqlParameter[1];
            SqlParameter LateHourparam1 = new SqlParameter("@BatchRunDate", DbType.DateTime);

            LateHourparam1.Value = iobjSystemManagement.icdoSystemManagement.batch_date;// DateTime.Now;
            LateHourparameters[0] = LateHourparam1;

            DataTable ldtLateHoursAndContributions = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_PensionLateHours", lstrLegacyDBConnetion, null, LateHourparameters);

            if (ldtLateHoursAndContributions != null && ldtLateHoursAndContributions.Rows.Count > 0)
            {
                if (ldtLateHoursAndContributions.AsEnumerable().Where(item => item.Field<int>("COMPUTATIONYEAR") < lintComputationYear).Count() > 0)
                {
                    ldtLateHoursAndContributions = ldtLateHoursAndContributions.AsEnumerable().Where(item => item.Field<int>("COMPUTATIONYEAR") < lintComputationYear).CopyToDataTable();

                    DataTable ldtFullWorkHistoryForIAP = new DataTable();
                    DataTable ldtRemainingHistoryForIAP = new DataTable();
                    string lstrPrevSSN = null;
                    int lintPrevYear = 0;
                    if (ldtLateHoursAndContributions != null && ldtLateHoursAndContributions.Rows.Count > 0)
                    {
                        DataRow[] ldrLateHoursAndContributions = ldtLateHoursAndContributions.FilterTable(utlDataType.String, "REPORTSTATUS", "L");
                        DataRow[] ldrIAPPersonAccount = null;
                        foreach (DataRow ldrLate in ldrLateHoursAndContributions)
                        {
                            if (ldtActivePersonWithIAPPlan != null && ldtActivePersonWithIAPPlan.Rows.Count > 0 &&
                                ldtActivePersonWithIAPPlan.AsEnumerable().Where(item => item.Field<string>("MPI_PERSON_ID") == Convert.ToString(ldrLate["MPID"])).Count() > 0)
                            {
                                if (lintPrevYear != 0 && (lintPrevYear != Convert.ToInt32(ldrLate["COMPUTATIONYEAR"]) || (lstrPrevSSN != null && lstrPrevSSN != Convert.ToString(ldrLate["SSN"]))))
                                {
                                    DataRow[] ldrRemaining = ldtFullWorkHistoryForIAP.FilterTable(utlDataType.Numeric, "computationyear", lintPrevYear);
                                    foreach (DataRow ldr in ldrRemaining)
                                    {
                                        DataRow ldrRem = idtLateHoursAndContributions.NewRow();
                                        ldrRem["empaccountno"] = ldr["empaccountno"];
                                        ldrRem["computationyear"] = ldr["computationyear"];
                                        ldrRem["ssn"] = ldr["ssn"];
                                        if (ldrIAPPersonAccount != null && ldrIAPPersonAccount.Length > 0)
                                        {
                                            ldrRem["person_account_id"] = ldrIAPPersonAccount[0]["person_account_id"] == DBNull.Value ? 0 : Convert.ToInt32(ldrIAPPersonAccount[0]["person_account_id"]);
                                            ldrRem["person_id"] = ldrIAPPersonAccount[0]["person_id"] == DBNull.Value ? 0 : Convert.ToInt32(ldrIAPPersonAccount[0]["person_id"]);
                                        }
                                        ldrRem["iaphours"] = ldr["iaphours"];
                                        ldrRem["iaphoursa2"] = ldr["iaphoursa2"];
                                        ldrRem["iappercent"] = ldr["iappercent"];
                                        idtLateHoursAndContributions.Rows.Add(ldrRem);
                                    }
                                    idtLateHoursAndContributions.AcceptChanges();
                                }
                                if (lstrPrevSSN != Convert.ToString(ldrLate["SSN"]))
                                {
                                    lintPrevYear = 0;
                                    ldtFullWorkHistoryForIAP = new DataTable();
                                    SqlParameter[] parameters = new SqlParameter[3];
                                    SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                                    SqlParameter param2 = new SqlParameter("@FROMDATE", DbType.DateTime);
                                    SqlParameter param3 = new SqlParameter("@COMPUTATIONYEAR", DbType.Int32);

                                    param1.Value = Convert.ToString(ldrLate["ssn"]);
                                    parameters[0] = param1;
                                    param2.Value = ldtEACutOffDate.AddDays(1);//PIR 859 NEW
                                    parameters[1] = param2;
                                    param3.Value = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.computation_year + 1;
                                    parameters[2] = param3;

                                    ldtFullWorkHistoryForIAP = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkhistoryForIAPAllocation", lstrLegacyDBConnetion, null, parameters);
                                    ldrIAPPersonAccount = idtIAPPersonAccounts.FilterTable(utlDataType.String, "ssn", Convert.ToString(ldrLate["ssn"]));
                                    ldtRemainingHistoryForIAP = ldtFullWorkHistoryForIAP.Copy();
                                }
                                DataRow ldrLHAndContr = idtLateHoursAndContributions.NewRow();
                                ldrLHAndContr["empaccountno"] = ldrLate["empaccountno"];
                                ldrLHAndContr["computationyear"] = ldrLate["computationyear"];
                                ldrLHAndContr["ssn"] = ldrLate["ssn"];
                                if (ldrIAPPersonAccount != null && ldrIAPPersonAccount.Length > 0)
                                {
                                    ldrLHAndContr["person_account_id"] = ldrIAPPersonAccount[0]["person_account_id"] == DBNull.Value ? 0 : Convert.ToInt32(ldrIAPPersonAccount[0]["person_account_id"]);
                                    ldrLHAndContr["person_id"] = ldrIAPPersonAccount[0]["person_id"] == DBNull.Value ? 0 : Convert.ToInt32(ldrIAPPersonAccount[0]["person_id"]);
                                }
                                else
                                {
                                    ldrLHAndContr["person_account_id"] = 0;
                                    ldrLHAndContr["person_id"] = 0;
                                }
                                ldrLHAndContr["lateiaphours"] = ldrLate["iaphours"];
                                ldrLHAndContr["lateiaphoursa2"] = ldrLate["iaphoursa2"];
                                ldrLHAndContr["lateiappercent"] = ldrLate["iappercent"];
                                if (ldtFullWorkHistoryForIAP != null && ldtFullWorkHistoryForIAP.Rows.Count > 0)
                                {
                                    DataRow[] ldrActualHours = ldtFullWorkHistoryForIAP.FilterTable(utlDataType.Numeric, "computationyear", Convert.ToInt32(ldrLate["computationyear"]));
                                    foreach (DataRow ldrAH in ldrActualHours)
                                    {
                                        if ((ldrLate["empaccountno"] == DBNull.Value && ldrAH["empaccountno"] == DBNull.Value) ||
                                            (ldrLate["empaccountno"] != DBNull.Value && ldrAH["empaccountno"] != DBNull.Value && Convert.ToInt32(ldrLate["empaccountno"]) == Convert.ToInt32(ldrAH["empaccountno"])))
                                        {
                                            ldrLHAndContr["iaphours"] = ldrAH["iaphours"];
                                            ldrLHAndContr["iaphoursa2"] = ldrAH["iaphoursa2"];
                                            ldrLHAndContr["iappercent"] = ldrAH["iappercent"];

                                            ldtFullWorkHistoryForIAP.Rows.Remove(ldrAH);
                                            ldtFullWorkHistoryForIAP.AcceptChanges();
                                            break;
                                        }
                                    }
                                }
                                idtLateHoursAndContributions.Rows.Add(ldrLHAndContr);
                                lstrPrevSSN = Convert.ToString(ldrLate["ssn"]);
                                lintPrevYear = Convert.ToInt32(ldrLate["computationyear"]);
                            }
                        }
                        if (lintPrevYear != 0)
                        {
                            DataRow[] ldrRemaining = ldtFullWorkHistoryForIAP.FilterTable(utlDataType.Numeric, "computationyear", lintPrevYear);
                            foreach (DataRow ldr in ldrRemaining)
                            {
                                DataRow ldrRem = idtLateHoursAndContributions.NewRow();
                                ldrRem["empaccountno"] = ldr["empaccountno"];
                                ldrRem["computationyear"] = ldr["computationyear"];
                                ldrRem["ssn"] = ldr["ssn"];
                                if (ldrIAPPersonAccount != null && ldrIAPPersonAccount.Length > 0)
                                {
                                    ldrRem["person_account_id"] = ldrIAPPersonAccount[0]["person_account_id"] == DBNull.Value ? 0 : Convert.ToInt32(ldrIAPPersonAccount[0]["person_account_id"]);
                                    ldrRem["person_id"] = ldrIAPPersonAccount[0]["person_id"] == DBNull.Value ? 0 : Convert.ToInt32(ldrIAPPersonAccount[0]["person_id"]);
                                }
                                else
                                {
                                    ldrRem["person_account_id"] = 0;
                                    ldrRem["person_id"] = 0;
                                }
                                ldrRem["iaphours"] = ldr["iaphours"];
                                ldrRem["iaphoursa2"] = ldr["iaphoursa2"];
                                ldrRem["iappercent"] = ldr["iappercent"];
                                idtLateHoursAndContributions.Rows.Add(ldrRem);
                            }
                        }
                        idtLateHoursAndContributions.AcceptChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Method to process the late allocation
        /// </summary>
        private void ProcessLateAllocation()
        {
            IEnumerable<int> lenmDistinctPersonAccounts = idtLateHoursAndContributions.AsEnumerable().Select(o => o.Field<int>("person_account_id")).Distinct();
            busIAPAllocationHelper lobjIAPAllocationHelper = new busIAPAllocationHelper();
            lobjIAPAllocationHelper.LoadIAPAllocationFactor();

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            iobjLock = new object();

            ParallelOptions p = new ParallelOptions();
            p.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

            Parallel.ForEach(lenmDistinctPersonAccounts.AsEnumerable(), p, (lintPersonAccountID, loopstate) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "LateIAPAllocation-Batch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;

                if (lintPersonAccountID != 0)
                    CheckEligibileAllocationsAndPostIntoContribution(lintPersonAccountID, lobjIAPAllocationHelper, lobjPassInfo);

                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;

            });
            lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        } */
        #endregion

        private void LoadAllIAPPersonAccount()
        {
            idtIAPPersonAccounts = new DataTable();
            idtIAPPersonAccounts = busBase.Select("cdoPersonAccount.GetAllIAPPersonAccount", new object[0] { });
        }

        /// <summary>
        /// Method to check the late hours and perform all eligible allocations
        /// </summary>
        private void CheckEligibileAllocationsAndPostIntoContribution(int aintPersonAccountID, busIAPAllocationHelper aobjIAPAllocationHelper, utlPassInfo aobjPassInfo)
        {
            lock (iobjLock)
            {
                iintRecordCount++;
                iintTotalCount++;
                if (iintRecordCount == 100)
                {
                    String lstrMsg = iintTotalCount + " : " + " Records Has Been Processed";
                    PostInfoMessage(lstrMsg);
                    iintRecordCount = 0;
                }
            }
            aobjPassInfo.BeginTransaction();

            DataTable ldtIAPContributions = new DataTable();
            DataTable ldtIAPFiltered = new DataTable();
            int lintComputationYear, lintPrevComputationYear;
            lintComputationYear = lintPrevComputationYear = 0;
            decimal ldecTotalYTDHours, ldecThru79Hours;
            decimal ldecTotalIAPHours = 0M;
            ldecThru79Hours = ldecTotalYTDHours = 0.0M;
            decimal ldecAllocation4Amount, ldecIAPAccountBalance = 0.00M, ldecAlloc1Amount, ldecAlloc2Amount, ldecAlloc2InvstAmount, ldecAlloc2FrftAmount, ldecAlloc3Amount, ldecAlloc4InvstAmount, ldecAlloc4FrftAmount, ldecAlloc5AfflAmount,
                ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount;
            bool lblnAgeFlag = false;
            decimal ldecFactor = 0;
            Collection<cdoIapAllocation5Recalculation> lclbIapAllocation5Recalculation = null;
            busBenefitApplication lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            try
            {
                lclbIapAllocation5Recalculation = LoadIAPAllocation5Information(aintPersonAccountID);
                DataRow[] ldrLateHoursAndContributions = idtLateHoursAndContributions.FilterTable(utlDataType.Numeric, "person_account_id", aintPersonAccountID);
                foreach (DataRow ldrLateHours in ldrLateHoursAndContributions)
                {
                    if (lintPrevComputationYear == Convert.ToInt32(ldrLateHours["computationyear"]))
                        continue;

                    ldecTotalYTDHours = 0.0M;
                    lintComputationYear = 0;
                    //lblnAgeFlag = false;
                    ldecAlloc1Amount = ldecAlloc2Amount = ldecAlloc2InvstAmount = ldecAlloc2FrftAmount = ldecAlloc3Amount = ldecAllocation4Amount = ldecAlloc4InvstAmount = ldecAlloc4FrftAmount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = 0.00M;
                    ldtIAPFiltered = new DataTable();

                    lintComputationYear = Convert.ToInt32(ldrLateHours["computationyear"]);
                    if (lintPrevComputationYear == 0)
                    {
                        ldtIAPContributions = new DataTable();
                        //Method to load IAP allocations from sgt_person_account_contribution table
                        ldtIAPContributions = LoadIAPContributions(aintPersonAccountID, lintComputationYear);
                        //PIR 859 NEW
                        if (ldtIAPContributions != null && ldtIAPContributions.Rows.Count > 0)
                        {
                            int lintFirstYear = Convert.ToInt32(ldtIAPContributions.Rows[0][enmPersonAccountRetirementContribution.computational_year.ToString()]);
                            int lintLastYear = Convert.ToInt32(ldtIAPContributions.Rows[ldtIAPContributions.Rows.Count - 1][enmPersonAccountRetirementContribution.computational_year.ToString()]);
                            int lintPersonAccountId = Convert.ToInt32(ldtIAPContributions.Rows[0][enmPersonAccountRetirementContribution.person_account_id.ToString()]);

                            for (int i = lintFirstYear; i <= lintLastYear; i++)
                            {
                                DataRow[] ldrldtIAPContributions =
                                    ldtIAPContributions.FilterTable(utlDataType.Numeric, enmPersonAccountRetirementContribution.computational_year.ToString().ToUpper(), i);
                                if (ldrldtIAPContributions != null && ldrldtIAPContributions.Length > 0)
                                    continue;
                                else
                                {
                                    DataRow ldrIAPContributions = ldtIAPContributions.NewRow();
                                    ldrIAPContributions[enmPersonAccountRetirementContribution.computational_year.ToString()] = i;
                                    ldrIAPContributions[enmPersonAccountRetirementContribution.person_account_id.ToString()] = lintPersonAccountId;
                                    foreach (DataColumn col in ldtIAPContributions.Columns)
                                    {
                                        if (col.ColumnName != Convert.ToString("computational_year").ToUpper() &&
                                            col.ColumnName != Convert.ToString("person_account_id").ToUpper())
                                        {
                                            ldrIAPContributions[col.ColumnName.ToString()] = 0M;
                                        }
                                    }
                                    ldtIAPContributions.Rows.Add(ldrIAPContributions);
                                }
                            }
                            ldtIAPContributions = ldtIAPContributions.AsEnumerable().OrderBy(t => t.Field<decimal>(Convert.ToString("computational_year").ToUpper())).CopyToDataTable();
                        }
                        //Method to load the IAP account balace as of the first year for which late hours came in
                        DataTable ldtIAPAccountBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAccountBalanceAsOfYear", new object[2] { aintPersonAccountID, lintComputationYear });
                        if (ldtIAPAccountBalance != null && ldtIAPAccountBalance.Rows.Count > 0)
                            ldecIAPAccountBalance = Convert.ToDecimal(ldtIAPAccountBalance.Rows[0][0]);

                        //ldecIAPAccountBalance = Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.GetIAPAccountBalanceAsOfYear", new object[2] { aintPersonAccountID, lintComputationYear },
                        //    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache));
                    }
                    //Block to load person account and work history. Used for allocation 2 and 5 calculation
                    if (lintPrevComputationYear == 0)
                    {
                        lblnAgeFlag = false;
                        lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lobjBenefitApplication.icdoBenefitApplication.person_id = Convert.ToInt32(ldrLateHours["person_id"]);
                        lobjBenefitApplication.LoadPerson();
                        lobjBenefitApplication.ibusPerson.LoadPersonAccountsByPlanId(busConstant.IAP_PLAN_ID);
                        lobjBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                        lblnAgeFlag = busGlobalFunctions.CalculatePersonAge(lobjBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, Convert.ToDateTime(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPInceptionDate))) < 55 ? true : false;
                        //cdoDummyWorkData lcdoWorkData1979 = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == 1979).FirstOrDefault();
                        ////IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
                        //if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
                        //{
                        //    ldecThru79Hours = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= 1979).Sum(o => o.qualified_hours);
                        //}

                        #region IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979

                        //Remove history for any forfieture year 1979
                        if (lobjBenefitApplication.aclbPersonWorkHistory_IAP != null && lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
                        {
                            if (lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.istrForfietureFlag == busConstant.FLAG_YES).Count() > 0)
                            {
                                int lintMaxForfietureYearBefore1979 = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(item => item.year <= busConstant.BenefitCalculation.YEAR_1979 && item.istrForfietureFlag == busConstant.FLAG_YES).Max(t => t.year);
                                lobjBenefitApplication.aclbPersonWorkHistory_IAP = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(item => item.year > lintMaxForfietureYearBefore1979).ToList().ToCollection();
                            }
                        }

                        if (lobjBenefitApplication.aclbPersonWorkHistory_IAP != null && lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
                        {
                            decimal ldecPreviousYearPaidIAPAccountBalance = 0M;
                            cdoDummyWorkData lcdoWorkData1979 = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).FirstOrDefault();
                            //IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
                            if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
                            {
                                int lintPaymentYear = 0;
                                DataTable ldtblPaymentYear = busBase.Select("cdoPersonAccountRetirementContribution.GetMaxPaymentYearOnOrBefore1979", new object[1] { lobjBenefitApplication.icdoBenefitApplication.person_id });
                                if (ldtblPaymentYear != null && ldtblPaymentYear.Rows.Count > 0 && Convert.ToString(ldtblPaymentYear.Rows[0][0]).IsNotNullOrEmpty())
                                {
                                    lintPaymentYear = Convert.ToInt32(ldtblPaymentYear.Rows[0][0]);
                                }
                                if (lintPaymentYear == 0)
                                {

                                    ldecThru79Hours = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Sum(o => o.qualified_hours);

                                }
                                else
                                {
                                    if (lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Count() > 0)
                                    {
                                        ldecThru79Hours = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Sum(o => o.qualified_hours);
                                    }
                                }

                                ldecThru79Hours += ldecPreviousYearPaidIAPAccountBalance;
                                if (ldecThru79Hours < 0)
                                    ldecThru79Hours = 0;
                            }
                        }

                        if (lobjBenefitApplication.aclbPersonWorkHistory_IAP != null && lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
                        {
                            lobjBenefitApplication.aclbPersonWorkHistory_IAP = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).ToList().ToCollection();
                        }

                        #endregion
                    }
                    //Block to update the IAP account balance if the late years are not consecutive
                    while (lintPrevComputationYear != 0 && (lintPrevComputationYear + 1) < lintComputationYear)
                    {
                        lintPrevComputationYear++;
                        //to recalculate the alloc1 for intermediate years
                        ldecAlloc1Amount = aobjIAPAllocationHelper.CalculateAllocation1Amount(lintPrevComputationYear, ldecIAPAccountBalance, 4, ref ldecFactor);

                        DataRow[] ldrIAPCont = ldtIAPContributions.FilterTable(utlDataType.Numeric, "computational_year", lintPrevComputationYear);
                        if (ldrIAPCont.Length > 0)
                        {
                            PostDifferenceAmountIntoContributionForAllocation1(aintPersonAccountID, lintPrevComputationYear, Convert.ToDecimal(ldrIAPCont[0]["alloc1"]), ldecAlloc1Amount);
                            //Block to calculate allocation 5 amount
                            if (lintPrevComputationYear >= 1996 && lintPrevComputationYear <= 2001)
                            {
                                string lstrCalculateAlloc5 = busConstant.FLAG_NO;
                                if (lclbIapAllocation5Recalculation != null && lclbIapAllocation5Recalculation.Where(item => item.computational_year == lintPrevComputationYear).Count() > 0)
                                {
                                    lstrCalculateAlloc5 = lclbIapAllocation5Recalculation.Where(item => item.computational_year == lintPrevComputationYear).FirstOrDefault().iap_allocation5_recalculate_flag;
                                }
                                else if (Convert.ToDecimal(ldrIAPCont[0]["alloc4"]) != 0.00M)
                                {
                                    lstrCalculateAlloc5 = busConstant.FLAG_YES;
                                }

                                if (lstrCalculateAlloc5 == busConstant.FLAG_YES)
                                {
                                    decimal ldecYTDHours = lobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == lintPrevComputationYear).Sum(o => o.qualified_hours);

                                    if (lintPrevComputationYear == 1996)
                                    {
                                        ldecAlloc5AfflAmount = aobjIAPAllocationHelper.CalcuateAllocation5AffliatesAmount(lintPrevComputationYear, lobjBenefitApplication.aclbPersonWorkHistory_IAP, lblnAgeFlag);
                                    }
                                    else
                                    {
                                        if (aobjIAPAllocationHelper.CheckParticipantIsAffiliate(lintPrevComputationYear, lobjBenefitApplication.ibusPerson.icdoPerson.istrSSNNonEncrypted))
                                            ldecAlloc5AfflAmount = aobjIAPAllocationHelper.CalcuateAllocation5AffliatesAmount(lintPrevComputationYear, lobjBenefitApplication.aclbPersonWorkHistory_IAP, lblnAgeFlag);
                                        else
                                            ldecAlloc5NonAfflAmount = aobjIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(lintPrevComputationYear, ldecYTDHours, busConstant.IAPAllocationNonAffiliatesFlag);
                                        ldecAlloc5BothAmount = aobjIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(lintPrevComputationYear, ldecYTDHours, busConstant.IAPAllocationBothAffAndNonAffFlag);
                                    }

                                    PostDifferenceAmountIntoContributionForAllocation5Affl(aintPersonAccountID, lintPrevComputationYear, Convert.ToDecimal(ldrIAPCont[0]["alloc5"]),
                                        (ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount));
                                }
                            }
                            ldecIAPAccountBalance += ldecAlloc1Amount + Convert.ToDecimal(ldrIAPCont[0]["alloc2"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc2_invt"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc2_frft"]) +
                                Convert.ToDecimal(ldrIAPCont[0]["alloc3"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc4"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc4_invt"]) + Convert.ToDecimal(ldrIAPCont[0]["alloc4_frft"]) +
                                ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount;
                        }
                        else
                        {
                            PostDifferenceAmountIntoContributionForAllocation1(aintPersonAccountID, lintPrevComputationYear, 0.00M, ldecAlloc1Amount);
                            ldecIAPAccountBalance += ldecAlloc1Amount + ldecAlloc5AfflAmount;
                        }
                        ldecAlloc1Amount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = 0.00M;
                    }

                    //ldecTotalYTDHours = Convert.ToDecimal(ldrLateHours["iaphoursa2"]) + Convert.ToDecimal(ldrLateHours["lateiaphoursa2"]);
                    ldtIAPFiltered = ldrLateHoursAndContributions.AsEnumerable().Where(o => o.Field<int>("computationyear") == lintComputationYear).CopyToDataTable();
                    foreach (DataRow ldr in ldtIAPFiltered.Rows)
                    {
                        ldecTotalIAPHours += (ldr["iaphours"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphours"])) + (ldr["lateiaphours"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["lateiaphours"]));
                        ldecTotalYTDHours += (ldr["iaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphoursa2"])) + (ldr["lateiaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["lateiaphoursa2"]));
                    }



                    //Fix for UAT PIR 1023
                    ////ldecAllocation4Amount = Convert.ToDecimal(ldrLateHours["iappercent"]) + Convert.ToDecimal(ldrLateHours["lateiappercent"]);
                    //ldecAllocation4Amount = aobjIAPAllocationHelper.CalculateAllocation4Amount(lintComputationYear, ldtIAPFiltered);                    

                    DataRow[] ldrIAPContribution = ldtIAPContributions.FilterTable(utlDataType.Numeric, "computational_year", lintComputationYear);
                    //Method to calculate Allocation 1 amount
                    
                    if (ldrIAPContribution.Length > 0 && lintPrevComputationYear == 0)
                        ldecAlloc1Amount = Convert.ToDecimal(ldrIAPContribution[0]["alloc1"]);
                    else
                        ldecAlloc1Amount = aobjIAPAllocationHelper.CalculateAllocation1Amount(lintComputationYear, ldecIAPAccountBalance, 4, ref ldecFactor);


                    if (ldecTotalIAPHours >= Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.QualifiedYearHours)))
                    {


                        //method to calculate allocation 2 amount
                        ldecAlloc2Amount = aobjIAPAllocationHelper.CalculateAllocation2Amount(lintComputationYear, ldecThru79Hours, ldecTotalYTDHours, DateTime.MinValue,
                                                                                DateTime.MinValue, DateTime.MinValue);
                        //method to calculate allocation 2 investment amount
                        ldecAlloc2InvstAmount = aobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
                                                                                DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationInvestmentFlag);
                        //method to calculate allocation 2 forfeiture amount
                        ldecAlloc2FrftAmount = aobjIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(lintComputationYear, ldecTotalYTDHours, DateTime.MinValue,
                                                                                DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationForfeitureFlag);
                        //method to calculate allocation 3 amount
                        ldecAlloc3Amount = aobjIAPAllocationHelper.CalculateAllocation3Amount(lintComputationYear, ldecThru79Hours, ldecTotalYTDHours);

                        //Fix for UAT PIR 1023
                        ldecAllocation4Amount = aobjIAPAllocationHelper.CalculateAllocation4Amount(lintComputationYear, ldtIAPFiltered);

                        //method to calculate allocation 4 investment amount
                        ldecAlloc4InvstAmount = aobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationInvestmentFlag);
                        //method to calculate allocation 4 forfeiture amount
                        ldecAlloc4FrftAmount = aobjIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(lintComputationYear, ldecAllocation4Amount, busConstant.IAPAllocationForfeitureFlag);
                        //Block to calculate allocation 5 amount
                        if (lintComputationYear >= 1996 && lintComputationYear <= 2001)
                        {
                            string lstrCalculateAlloc5 = busConstant.FLAG_NO;
                            if (lclbIapAllocation5Recalculation != null && lclbIapAllocation5Recalculation.Where(item => item.computational_year == lintComputationYear).Count() > 0)
                            {
                                lstrCalculateAlloc5 = lclbIapAllocation5Recalculation.Where(item => item.computational_year == lintComputationYear).FirstOrDefault().iap_allocation5_recalculate_flag;
                            }
                            else if (ldecAllocation4Amount != 0.00M)
                            {
                                lstrCalculateAlloc5 = busConstant.FLAG_YES;
                            }

                            if (lstrCalculateAlloc5 == busConstant.FLAG_YES)
                            {
                                if (lintComputationYear == 1996)
                                {
                                    ldecAlloc5AfflAmount = aobjIAPAllocationHelper.CalcuateAllocation5AffliatesAmount(lintComputationYear, lobjBenefitApplication.aclbPersonWorkHistory_IAP, lblnAgeFlag);
                                }
                                else
                                {

                                    if (aobjIAPAllocationHelper.CheckParticipantIsAffiliate(lintComputationYear, lobjBenefitApplication.ibusPerson.icdoPerson.istrSSNNonEncrypted))
                                        ldecAlloc5AfflAmount = aobjIAPAllocationHelper.CalcuateAllocation5AffliatesAmount(lintComputationYear, lobjBenefitApplication.aclbPersonWorkHistory_IAP, lblnAgeFlag);
                                    else
                                        ldecAlloc5NonAfflAmount = aobjIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(lintComputationYear, ldecTotalYTDHours, busConstant.IAPAllocationNonAffiliatesFlag);
                                    ldecAlloc5BothAmount = aobjIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(lintComputationYear, ldecTotalYTDHours, busConstant.IAPAllocationBothAffAndNonAffFlag);
                                }
                            }
                        }
                    }
                    //Method to post the difference amount into contribution amount
                    PostDifferenceAmountIntoContribution(lintComputationYear, aintPersonAccountID, ldrIAPContribution, ldecAlloc1Amount, ldecAlloc2Amount, ldecAlloc2InvstAmount, ldecAlloc2FrftAmount, ldecAlloc3Amount, ldecAllocation4Amount,
                                                    ldecAlloc4InvstAmount, ldecAlloc4FrftAmount, ldecAlloc5AfflAmount, ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount);
                    //updating IAP account balance with the latest allocation amounts
                    ldecIAPAccountBalance = ldecIAPAccountBalance + ldecAlloc1Amount + ldecAlloc2Amount + ldecAlloc2InvstAmount + ldecAlloc2FrftAmount + ldecAlloc3Amount + ldecAllocation4Amount +
                                                    ldecAlloc4InvstAmount + ldecAlloc4FrftAmount + ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount;
                    lintPrevComputationYear = lintComputationYear;
                }
                //To recalculate the allocation 1 for the last person
                PostAllocation1And5ForRemainingYears(lintPrevComputationYear, aintPersonAccountID, ldecIAPAccountBalance, ldtIAPContributions, aobjIAPAllocationHelper, lobjBenefitApplication, lblnAgeFlag, lclbIapAllocation5Recalculation);
                aobjPassInfo.Commit();
            }
            catch (Exception ex)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(ex);
                    String lstrMsg = "Error while calcualting Late Allocation for PersonAccount ID " + aintPersonAccountID.ToString() + " : " + ex.ToString();
                    PostErrorMessage(lstrMsg);
                }
                aobjPassInfo.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// Method to load IAP allocations from contribution table
        /// </summary>
        /// <param name="aintPersonAccountID">Person Account ID</param>
        /// <param name="aintComputationYear">Computation Year</param>
        /// <returns>Datatable of allocations</returns>
        private DataTable LoadIAPContributions(int aintPersonAccountID, int aintComputationYear)
        {
            return busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationsForPersonAccount", new object[2] { aintPersonAccountID, aintComputationYear });
        }

        /// <summary>
        /// Method to post the difference amount into contribution amount
        /// </summary>
        /// <param name="aintComputationYear">Computation Year</param>
        /// <param name="aintPersonAccountID">Person Account ID</param>
        /// <param name="adrIAPContribution">Filtered collection of contribution table</param>
        /// <param name="adecAlloc1Amount">New Allocation 1 amount</param>
        /// <param name="adecAlloc2Amount">New allocation 2 amount</param>
        /// <param name="adecAlloc2InvstAmount">New allocation 2 invst amount</param>
        /// <param name="adecAlloc2FrftAmount">New allocation 2 forfi</param>
        /// <param name="adecAlloc3Amount">New Allocation 3 amount</param>
        /// <param name="adecAllocation4Amount">New allocation 4 amount</param>
        /// <param name="adecAlloc4InvstAmount">New allocation 4 Invst</param>
        /// <param name="adecAlloc4FrftAmount">New allocation 4 frft</param>
        /// <param name="adecAlloc5AfflAmount">New allocation 5 affiliate amount</param>
        /// <param name="adecAlloc5NonAfflAmount">New allocation 5 non affiliate amount</param>
        /// <param name="adecAlloc5BothAmount">New allocation 5 both amount</param>
        public void PostDifferenceAmountIntoContribution(int aintComputationYear, int aintPersonAccountID, DataRow[] adrIAPContribution, decimal adecAlloc1Amount, decimal adecAlloc2Amount, decimal adecAlloc2InvstAmount, decimal adecAlloc2FrftAmount,
            decimal adecAlloc3Amount, decimal adecAllocation4Amount, decimal adecAlloc4InvstAmount, decimal adecAlloc4FrftAmount, decimal adecAlloc5AfflAmount, decimal adecAlloc5NonAfflAmount, decimal adecAlloc5BothAmount)
        {
            busPersonAccountRetirementContribution lobjRetrContribution;
            //block to insert the allocation 1 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc1"].IsDBNull()))
                adecAlloc1Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc1"]);
            if (adecAlloc1Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc1Amount,
                astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation1);
            }
            //block to insert the allocation 2 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc2"].IsDBNull()))
                adecAlloc2Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc2"]);

            if (adecAlloc2Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc2Amount,
                    astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation2);
            }
            //block to insert the allocation 2 invst difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc2_invt"].IsDBNull()))
                adecAlloc2InvstAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc2_invt"]);
            if (adecAlloc2InvstAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc2InvstAmount,
                astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation2, astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
            }
            //block to insert the allocation 2 frft difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc2_frft"].IsDBNull()))
                adecAlloc2FrftAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc2_frft"]);
            if (adecAlloc2FrftAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc2FrftAmount,
                astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation2, astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
            }
            //block to insert the allocation 3 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc3"].IsDBNull()))
                adecAlloc3Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc3"]);
            if (adecAlloc3Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc3Amount,
                astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation3);
            }
            //block to insert the allocation 4 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc4"].IsDBNull()))
                adecAllocation4Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc4"]);
            if (adecAllocation4Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAllocation4Amount,
                astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation4);
            }
            //block to insert the allocation 4 invt difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc4_invt"].IsDBNull()))
                adecAlloc4InvstAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc4_invt"]);
            if (adecAlloc4InvstAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc4InvstAmount,
                astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation4, astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
            }
            //block to insert the allocation 4 forft difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc4_frft"].IsDBNull()))
                adecAlloc4FrftAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc4_frft"]);
            if (adecAlloc4FrftAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc4FrftAmount,
                astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation4, astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
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
            decimal ldecTotalAlloc5 = adecAlloc5AfflAmount + adecAlloc5NonAfflAmount + adecAlloc5BothAmount;
            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc5"].IsDBNull()))
                ldecTotalAlloc5 -= Convert.ToDecimal(adrIAPContribution[0]["alloc5"]);
            if (ldecTotalAlloc5 != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear, adecIAPBalanceAmount: ldecTotalAlloc5,
                astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation5);
            }
        }

        /// <summary>
        /// Method to post the allocation 1 amount for remaining years
        /// </summary>
        /// <param name="aintPrevComputationYear">previous computation year</param>
        /// <param name="aintPersonAccountID">person account id</param>
        /// <param name="adecPrevYearAccountBalance">Prev year iap account balance</param>
        /// <param name="adtIAPContributions">IAP allocations from contribution table</param>
        /// <param name="aobjIAPHelper">bus. object containing allcation formula</param>
        private void PostAllocation1And5ForRemainingYears(int aintPrevComputationYear, int aintPersonAccountID, decimal adecPrevYearAccountBalance, DataTable adtIAPContributions, busIAPAllocationHelper aobjIAPHelper,
            busBenefitApplication aobjBenefitApplication, bool ablnAgeFlag, Collection<cdoIapAllocation5Recalculation> aclbIapAllocation5Recalculation)
        {
            IEnumerable<DataRow> lenmRemainingContributions = adtIAPContributions.AsEnumerable().Where(o => o.Field<decimal>("computational_year") > Convert.ToDecimal(aintPrevComputationYear));
            decimal ldecAlloc1Amount, ldecAlloc5AfflAmount, ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount, ldecFactor;
            ldecAlloc1Amount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = ldecFactor = 0.00M;
            foreach (DataRow ldr in lenmRemainingContributions)
            {
                //method to calculate allocation 1 amount
                ldecAlloc1Amount = aobjIAPHelper.CalculateAllocation1Amount(Convert.ToInt32(ldr["computational_year"]), adecPrevYearAccountBalance, 4, ref ldecFactor);
                //Block to calculate allocation 5 amount
                if (Convert.ToInt32(ldr["computational_year"]) >= 1996 && Convert.ToInt32(ldr["computational_year"]) <= 2001)
                {
                    string lstrCalculateAlloc5 = busConstant.FLAG_NO;
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

                        decimal ldecYTDHours = aobjBenefitApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == Convert.ToInt32(ldr["computational_year"])).Sum(o => o.qualified_hours);

                        if (Convert.ToInt32(ldr["computational_year"]) == 1996)
                        {
                            ldecAlloc5AfflAmount = aobjIAPHelper.CalcuateAllocation5AffliatesAmount(Convert.ToInt32(ldr["computational_year"]), aobjBenefitApplication.aclbPersonWorkHistory_IAP, ablnAgeFlag);
                        }
                        else
                        {
                            if (aobjIAPHelper.CheckParticipantIsAffiliate(Convert.ToInt32(ldr["computational_year"]), aobjBenefitApplication.ibusPerson.icdoPerson.istrSSNNonEncrypted))
                                ldecAlloc5AfflAmount = aobjIAPHelper.CalcuateAllocation5AffliatesAmount(Convert.ToInt32(ldr["computational_year"]), aobjBenefitApplication.aclbPersonWorkHistory_IAP, ablnAgeFlag);
                            else
                                ldecAlloc5NonAfflAmount = aobjIAPHelper.CalcuateAllocation5NonAffOrBothAmount(Convert.ToInt32(ldr["computational_year"]), ldecYTDHours, busConstant.IAPAllocationNonAffiliatesFlag);

                            ldecAlloc5BothAmount = aobjIAPHelper.CalcuateAllocation5NonAffOrBothAmount(Convert.ToInt32(ldr["computational_year"]), ldecYTDHours, busConstant.IAPAllocationBothAffAndNonAffFlag);
                        }
                        PostDifferenceAmountIntoContributionForAllocation5Affl(aintPersonAccountID, Convert.ToInt32(ldr["computational_year"]), Convert.ToDecimal(ldr["alloc5"]),
                            (ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount));
                    }
                }
                //method to post the difference amount into contribution table
                PostDifferenceAmountIntoContributionForAllocation1(aintPersonAccountID, Convert.ToInt32(ldr["computational_year"]), Convert.ToDecimal(ldr["alloc1"]), ldecAlloc1Amount);
                //updating iap account balance
                adecPrevYearAccountBalance += (ldecAlloc1Amount + Convert.ToDecimal(ldr["alloc2"]) + Convert.ToDecimal(ldr["alloc2_invt"]) + Convert.ToDecimal(ldr["alloc2_frft"]) + Convert.ToDecimal(ldr["alloc3"]) + Convert.ToDecimal(ldr["alloc4"]) +
                    Convert.ToDecimal(ldr["alloc4_invt"]) + Convert.ToDecimal(ldr["alloc4_frft"]) + ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount);
                ldecAlloc1Amount = ldecAlloc5AfflAmount = ldecAlloc5NonAfflAmount = ldecAlloc5BothAmount = 0.00M;
            }
        }

        /// <summary>
        /// Method to post the difference amount into contribution table for allocation 1
        /// </summary>
        /// <param name="aintPersonAccountID">person account id</param>
        /// <param name="aintComputationYear">computation year</param>
        /// <param name="adecOldAlloc1Amount">old allcation 1 amount</param>
        /// <param name="adecNewAlloc1Amount">new allocation 1 amount</param>
        public void PostDifferenceAmountIntoContributionForAllocation1(int aintPersonAccountID, int aintComputationYear, decimal adecOldAlloc1Amount, decimal adecNewAlloc1Amount)
        {
            busPersonAccountRetirementContribution lobjRetrContribution = new busPersonAccountRetirementContribution();
            if ((adecNewAlloc1Amount - adecOldAlloc1Amount) != 0)
            {
                //method to post the entires into contribution table
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear,
                adecIAPBalanceAmount: (adecNewAlloc1Amount - adecOldAlloc1Amount), astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation1);
            }
        }

        /// <summary>
        /// Method to post the difference amount into contribution table for allocation 5 affliates
        /// </summary>
        /// <param name="aintPersonAccountID">Person Account ID</param>
        /// <param name="aintComputationYear">Computation year</param>
        /// <param name="adecOldAlloc5AfflAmount">Old Alloc 5 Affl amount</param>
        /// <param name="adecNewAlloc5AfflAmount">New Alloc 5 Affl amount</param>
        private void PostDifferenceAmountIntoContributionForAllocation5Affl(int aintPersonAccountID, int aintComputationYear, decimal adecOldAlloc5Amount, decimal adecNewAlloc5Amount)
        {
            busPersonAccountRetirementContribution lobjRetrContribution = new busPersonAccountRetirementContribution();
            if ((adecNewAlloc5Amount - adecOldAlloc5Amount) != 0)
            {
                //method to post the entires into contribution table
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear), DateTime.Now, aintComputationYear,
                adecIAPBalanceAmount: (adecNewAlloc5Amount - adecOldAlloc5Amount), astrTransactionType: busConstant.RCTransactionTypeLateAllocation, astrContributionType: busConstant.RCContributionTypeAllocation5);
            }
        }

        public Collection<cdoIapAllocation5Recalculation> LoadIAPAllocation5Information(int aintPersonAccountId)
        {
            Collection<cdoIapAllocation5Recalculation> lclbIapAllocation5Recalculation = new Collection<cdoIapAllocation5Recalculation>();

            DataTable ldtblIapAllocation5Recalculation = busBase.Select("cdoIapAllocation5Recalculation.GetAllocation5Information", new object[1] { aintPersonAccountId });
            if (ldtblIapAllocation5Recalculation.Rows.Count > 0)
            {
                lclbIapAllocation5Recalculation = cdoIapAllocation5Recalculation.GetCollection<cdoIapAllocation5Recalculation>(ldtblIapAllocation5Recalculation);
            }

            return lclbIapAllocation5Recalculation;

        }
    }
}
