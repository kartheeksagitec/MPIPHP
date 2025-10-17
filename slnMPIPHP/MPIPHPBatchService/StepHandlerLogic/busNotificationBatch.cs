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
using Sagitec.Common;
using System.Data.SqlClient;

namespace MPIPHPJobService
{
    public class busNotificationBatch : busBatchHandler
    {
        public busNotificationBatch()
        {
        }


        public void ProcessNotifications()
        {
            DataTable ldtbNotifications = busBase.Select<cdoBatchNotification>(new string[1] { enmBatchNotification.status_value.ToString() },
                    new object[1] { busConstant.BatchNotification.STATUS_ACTIVE }, null, null);

            ArrayList aarrResult = new ArrayList();
            Hashtable ahtbQueryBkmarks = new Hashtable();

            foreach (DataRow ldtrw in ldtbNotifications.Rows)
            {
                //iobjPassInfo.BeginTransaction();
                istrProcessName = busConstant.MPIPHPBatch.BATCH_NOTIFICATION;
                try
                {
                    busBatchNotification lbusBatchNotification = new busBatchNotification() { icdoBatchNotification = new cdoBatchNotification() };
                    lbusBatchNotification.icdoBatchNotification.LoadData(ldtrw);
                    Object lobjNotificationTimerInDays = lbusBatchNotification.icdoBatchNotification.notification_timer_days;

                    switch (lbusBatchNotification.icdoBatchNotification.notification_id)
                    {
                        case 1:
                            {
                                // Execute the Notification Query.
                                DataTable ldtbNotificationQuery = busBase.Select(lbusBatchNotification.icdoBatchNotification.sql_query_name,
                                                   new Object[1] { lobjNotificationTimerInDays });
                                if (ldtbNotificationQuery != null && ldtbNotificationQuery.Rows.Count > 0)
                                {
                                    busBase lbusBase = new busBase();
                                    Collection<busQdroApplication> lclbQdroApplication = lbusBase.GetCollection<busQdroApplication>(ldtbNotificationQuery, "icdoDroApplication");
                                    foreach (busQdroApplication lbusQdroApplication in lclbQdroApplication)
                                    {

                                        lbusQdroApplication.icdoDroApplication.batch_90day_flag = "Y";
                                        lbusQdroApplication.icdoDroApplication.Update();

                                        lbusQdroApplication.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                                        if (lbusQdroApplication.ibusParticipant.FindPerson(lbusQdroApplication.icdoDroApplication.person_id))
                                        {
                                            aarrResult.Add(lbusQdroApplication);
                                            this.CreateCorrespondence(busConstant.QDRO_STATUS_PENDING_90_DAYS, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                                            aarrResult.Clear();
                                        }
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }


                }
                catch (Exception e)
                {
                    //iobjPassInfo.Rollback();
                    idlgUpdateProcessLog("Error Occured with Message = " + e.Message, "ERR", istrProcessName);
                }
            }
        }

        public void EncryptSSN()
        {
            try
            {
                DataTable ldt = busBase.Select("cdoPerson.Lookup", new Object[] { });
                if (ldt.Rows.Count > 0)
                {
                    foreach (DataRow ldtr in ldt.Rows)
                    {
                        cdoPerson lcdo = new cdoPerson();
                        lcdo.LoadData(ldtr);
                        if (!string.IsNullOrEmpty(lcdo.ssn) && lcdo.ssn.IsNumeric() && lcdo.ssn.Length == 9)
                        {
                            lcdo.ssn = lcdo.ssn;
                            lcdo.Update();


                        }
                    }
                }

            }
            catch
            {

            }

        }

        public DataTable LoadDeathNotificationBatch()
        {
            DataTable ldtblist = null;
            //IDbConnection lconLegacy = DBFunction.GetDBConnection("Legacy");

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnection = utlLegacyDBConnetion.istrConnectionString;

            if (astrLegacyDBConnection != null)
            {
                ldtblist = busBase.Select("cdoDeathNotification.LoadDeathOutboundFile", new object[0] { });
                ldtblist.Columns.Add(new DataColumn("PREV_DATE_DEATH", typeof(DateTime)));
                foreach (DataRow ldtRow in ldtblist.Rows)
                {
                    busIAPAllocationHelper lbusIAPAllocationHelper = new busIAPAllocationHelper();
                    string istrUnionCode = lbusIAPAllocationHelper.GetTrueUnionCodeBySSNAndPlanYear(busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToDateTime(ldtRow["DATE_OF_DEATH"]).Year - 1).AddDays(1), busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToDateTime(ldtRow["DATE_OF_DEATH"]).Year), ldtRow["SSN"].ToString());
                    if (istrUnionCode.IsNotNullOrEmpty())
                    {
                        SqlParameter[] lParameters = new SqlParameter[1];
                        SqlParameter param1 = new SqlParameter("@UnionCode", DbType.String);
                        param1.Value = istrUnionCode;
                        lParameters[0] = param1;

                        DataTable ldataTable = new DataTable();
                        ldataTable = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetTrueUnionNamebyUnionCode", astrLegacyDBConnection, null, lParameters);

                        if (ldataTable.Rows.Count > 0)
                        {
                            ldtRow["UNION_CODE_DESC"] = Convert.ToString(ldataTable.Rows[0][0]);
                            ldtRow["UNION_CODE"] = ldataTable.Rows[0][1].ToString();
                        }
                        else
                        {
                            ldtRow["UNION_CODE_DESC"] = "No Union";
                        }
                    }

                    //Status Depending on Death
                    if (Convert.ToString(ldtRow[enmDeathNotification.death_notification_status_value.ToString()]) == busConstant.NOTIFICATION_STATUS_INCORRECTLY_REPORTED ||
                    Convert.ToString(ldtRow[enmDeathNotification.death_notification_status_value.ToString()]) == busConstant.NOTIFICATION_STATUS_NOT_DECEASED)
                    {
                        ldtRow["APP_STATUS"] = busConstant.INC_REPORT_DEATH_REPORT;
                        ldtRow["PREV_DATE_DEATH"] = Convert.ToDateTime(ldtRow[enmDeathNotification.date_of_death.ToString()].ToString());
                        ldtRow[enmDeathNotification.date_of_death.ToString()] = DBNull.Value;
                    }
                    else
                    {
                        ldtRow["APP_STATUS"] = busConstant.NEW_REPORT_DEATH_REPORT;
                    }
                }
            }
            ldtblist.TableName = "ReportTable01";
            ldtblist.DefaultView.Sort = "UNION_CODE_DESC,LAST_NAME, FIRST_NAME, SS4, DATE_OF_DEATH,STATUS ASC";
            ldtblist = ldtblist.DefaultView.ToTable();
            return ldtblist;

        }

        public string ReturnRetirementStatus(int aintPersonId)
        {
            string lstrRetireeStatus = "A";
            DataTable ldtbBenefitApplcation = busBase.Select("cdoDeathNotification.GetApprovedReirementorDisability", new object[1] { aintPersonId });
            if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
            {
                int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                if (lintCount > 0)
                {
                    lstrRetireeStatus = "R";
                }
            }
            return lstrRetireeStatus;

        }

        public void CreateLastOneYearDeathNotificationReport()
        {
            DataTable dtDeathReport = busBase.Select("entDeathNotification.GetLastOneYearDeathNotificationDetails", new object[0]);

            if (dtDeathReport != null && dtDeathReport.Rows.Count > 0)
            {
                string templatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.LAST_ONE_YEAR_DEATH_NOTIFICATION_REPORT + ".xlsx";
                string reportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.LAST_ONE_YEAR_DEATH_NOTIFICATION_REPORT + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

                DataSet dsReportData = new DataSet();
                dsReportData.Tables.Add(dtDeathReport.Copy());
                dsReportData.Tables[0].TableName = "ReportTable01";

                busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                lbusExcelReportGenerator.CreateExcelReport(templatePath, reportPath, "DEATH_NOTIFICATION", dsReportData);
            }
        }

    }
}
