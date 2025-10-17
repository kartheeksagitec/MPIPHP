using NeoBase.Common;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoBase.BPM
{
    public class busBpmReportMethods : busBase
    {

        public DataSet GetBPMCompletedActivity(DateTime madtStartDate, DateTime madtEndDate, string astrProcessId = null, string astrRole = null)
        {
            int lintProcessId = 0;
            if (astrProcessId.IsNull())
                lintProcessId = 0;
            else if (Int32.TryParse(astrProcessId, out lintProcessId))
                lintProcessId = Convert.ToInt32(astrProcessId);

            int lintRoleId = 0;
            if (astrRole.IsNull())
                lintRoleId = 0;
            else if (Int32.TryParse(astrRole, out lintRoleId))
                lintRoleId = Convert.ToInt32(astrRole);

            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.START_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.END_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.PROCESS);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.ROLE);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            if (madtStartDate.Equals(DateTime.MinValue))
            {
                madtStartDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.START_DATE] = madtStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            if (madtEndDate.Equals(DateTime.MinValue))
            {
                madtEndDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.END_DATE] = madtEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }
            // Modified by - Rahul Mane
            // Modified date - 11_13_2024
            // Parameters - Role id parameter passed to search record by the Role

            DataTable ldtbBPMCompletedActivity = DBFunction.DBSelect("entReport.rptBPMCompletedActivity",
               new object[5] { lintProcessId, madtStartDate, madtEndDate, busNeoBaseConstants.BPMCOMPLETEDACTIVITY, lintRoleId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            DataTable ldtbBPMCompletedActivityBarChart = DBFunction.DBSelect("entReport.rptBPMCompletedActivityBarChart",
                new object[5] { lintProcessId, madtStartDate, madtEndDate, busNeoBaseConstants.BPMCOMPLETEDACTIVITY, lintRoleId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (lintProcessId > 0)
            {
                DataTable ldtbProcessName = DBFunction.DBSelect("entReport.GetProcessNameByProcessID", new object[1] { lintProcessId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldtbProcessName.IsNotNull() && ldtbProcessName.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS] = ldtbProcessName.Rows[0][busNeoBaseConstants.NAME];
                }
            }

            if (lintRoleId > 0)
            {
                DataTable ldtbRoleName = DBFunction.DBSelect("entReport.GetRoleByRoleID", new object[1] { lintRoleId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldtbRoleName.IsNotNull() && ldtbRoleName.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.ROLE] = ldtbRoleName.Rows[0]["role_description"];
                }
            }
            ldtbReportParams.Rows.Add(ldrReportParam);
            DataSet ldstBPMCompletedActivityReport = new DataSet();

            ldtbBPMCompletedActivity.TableName = ReportConstants.REPORT_TABLE01;
            ldstBPMCompletedActivityReport.Tables.Add(ldtbBPMCompletedActivity.Copy());

            ldtbBPMCompletedActivityBarChart.TableName = ReportConstants.REPORT_TABLE02;
            ldstBPMCompletedActivityReport.Tables.Add(ldtbBPMCompletedActivityBarChart.Copy());
            ldstBPMCompletedActivityReport.Tables.Add(ldtbReportParams.Copy());

            return ldstBPMCompletedActivityReport;
        }

        /// <summary>
        /// Gets In Progress activities of BPM
        /// </summary>
        /// <param name="astrProcessId"></param>
        /// <param name="adtStartDate"></param>
        /// <param name="adtEndDate"></param>
        /// <param name="astrRole"></param>
        /// <returns></returns>
        public DataSet GetBPMInProgessActivity(DateTime madtStartDate, DateTime madtEndDate, string astrProcessId = null, string astrRole = null)
        {
            int lintProcessId = 0;

            if (astrProcessId.IsNullOrEmpty())
                lintProcessId = 0;
            else if (Int32.TryParse(astrProcessId, out lintProcessId))
                lintProcessId = Convert.ToInt32(astrProcessId);

            int lintRoleId = 0;
            if (astrRole.IsNullOrEmpty())
                lintRoleId = 0;
            else if (Int32.TryParse(astrRole, out lintRoleId))
                lintRoleId = Convert.ToInt32(astrRole);

            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.START_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.END_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.PROCESS);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.ROLE);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            if (madtStartDate.Equals(DateTime.MinValue))
            {
                madtStartDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.START_DATE] = madtStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            if (madtEndDate.Equals(DateTime.MinValue))
            {
                madtEndDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.END_DATE] = madtEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            // Modified by - Rahul Mane
            // Modified date - 11_13_2024
            // Parameters - Role id parameter passed to search record by the Role

            DataTable ldtbBPMActivityInProgress = DBFunction.DBSelect("entReport.rptBPMActivityInProgress",
               new object[5] { lintProcessId, madtStartDate, madtEndDate, busNeoBaseConstants.IN_PROCCESS, lintRoleId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            DataTable ldtbBPMActivityInProgressBarChart = DBFunction.DBSelect("entReport.rptBPMActivityInProgressBarChart",
                new object[5] { lintProcessId, madtStartDate, madtEndDate, busNeoBaseConstants.IN_PROCCESS, lintRoleId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (lintProcessId > 0)
            {
                DataTable ldtbProcessName = DBFunction.DBSelect("entReport.GetProcessNameByProcessID", new object[1] { lintProcessId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldtbProcessName.IsNotNull() && ldtbProcessName.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS] = ldtbProcessName.Rows[0][busNeoBaseConstants.NAME];
                }
            }

            if (lintRoleId > 0)
            {
                DataTable ldtbRoleName = DBFunction.DBSelect("entReport.GetRoleByRoleID", new object[1] { lintRoleId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldtbRoleName.IsNotNull() && ldtbRoleName.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.ROLE] = ldtbRoleName.Rows[0]["role_description"];
                }
            }

            ldtbReportParams.Rows.Add(ldrReportParam);

            ldtbBPMActivityInProgress.TableName = ReportConstants.REPORT_TABLE01;
            DataSet ldstOrgMainReport = new DataSet();
            ldstOrgMainReport.Tables.Add(ldtbBPMActivityInProgress.Copy());
            ldtbBPMActivityInProgressBarChart.TableName = ReportConstants.REPORT_TABLE02;
            ldstOrgMainReport.Tables.Add(ldtbBPMActivityInProgressBarChart.Copy());

            ldstOrgMainReport.Tables.Add(ldtbReportParams.Copy());
            return ldstOrgMainReport;
        }

        /// <summary>
        /// Gets Unclaimed activities of BPM
        /// </summary>
        /// <param name="adtStartDate"></param>
        /// <param name="adtEndDate"></param>
        /// <returns></returns>
        public DataSet GetBPMUnClaimedActivity(DateTime adtStartDate, DateTime adtEndDate)
        {
            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.START_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.END_DATE);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            if (adtStartDate.Equals(DateTime.MinValue))
            {
                adtStartDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.START_DATE] = adtStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            if (adtEndDate.Equals(DateTime.MinValue))
            {
                adtEndDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.END_DATE] = adtEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }
            ldtbReportParams.Rows.Add(ldrReportParam);
            DataTable ldtbBPMUnClaimedActivity = DBFunction.DBSelect("entReport.rptBPMUnclaimedActivities",
               new object[2] { adtStartDate, adtEndDate }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            DataTable ldtbBPMUnClaimedActivityPieChart = DBFunction.DBSelect("entReport.rptUnClaimedActivitiesPieChart",
                new object[2] { adtStartDate, adtEndDate }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);


            DataSet ldstOrgMainReport = new DataSet();
            ldtbBPMUnClaimedActivity.TableName = ReportConstants.REPORT_TABLE01;
            ldstOrgMainReport.Tables.Add(ldtbBPMUnClaimedActivity.Copy());

            ldtbBPMUnClaimedActivityPieChart.TableName = ReportConstants.REPORT_TABLE02;
            ldstOrgMainReport.Tables.Add(ldtbBPMUnClaimedActivityPieChart.Copy());
            ldtbReportParams.TableName = ReportConstants.REPORT_TABLE03;
            ldstOrgMainReport.Tables.Add(ldtbReportParams.Copy());
            ldtbReportParams.TableName = ReportConstants.REPORT_TABLE04;
            ldstOrgMainReport.Tables.Add(ldtbReportParams.Copy());
            return ldstOrgMainReport;
        }

        /// <summary>
        /// Work flow User Snapshot Report
        /// </summary>
        /// Developer:Ashish Saklani 08-06-2020 PIR -ID:1888
        /// Added the following code to handle the parameter scenario of the Report.
        /// <param name="Userid"></param>
        /// <returns></returns>
        public DataSet GetWorkflowUserSnapshot(string astrUserID = null, string astrProcessID = null)
        {
            DataSet ldsWorkFlowUserSnapshot = new DataSet();
            Collection<IDbDataParameter> lcolParams = new Collection<IDbDataParameter>();
            IDbDataParameter strUserID = DBFunction.GetDBParameter("@var_USER_ID", DbType.String, astrUserID);
            lcolParams.Add(strUserID);
            IDbDataParameter strProcessID = DBFunction.GetDBParameter("@var_PROCESS_ID", DbType.Int32, Convert.ToInt32(astrProcessID));
            lcolParams.Add(strProcessID);

            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.PROCESS_ID);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.USER_ID);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            int ID = 0;
            string U_name = String.Empty;
            // Developer- Rahul Mane
            // Iteration - Main-Iteration10
            // Date - 07-29-2021
            // PIR - 2305 - The system is not displaying the details under the generated reports related to the workflows.
            if (DBFunction.IsOracleConnection() || DBFunction.IsPostgreSQLConnection())
            {
                DBFunction.DBExecuteProcedure("USP_REP_USER_SNAPSHOT", lcolParams, iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
                DataTable ldtActivityStatistics = DBFunction.DBSelect("entReport.GetAcitvityStatisticsByWorkflowUserSnapshot", iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtActivityStatistics.TableName = ReportConstants.REPORT_TABLE01;
                ldtActivityStatistics.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                ldtActivityStatistics.Columns.Add(new DataColumn("User_ID", typeof(string)));

                foreach (DataRow rowitem in ldtActivityStatistics.Rows)
                {

                    rowitem[10] = astrProcessID != null ? astrProcessID : "ALL"; ;
                    rowitem[11] = astrUserID != null ? astrUserID : "ALL";
                }

                if (ldtActivityStatistics.IsNotNull() && ldtActivityStatistics.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtActivityStatistics.Rows[0][busNeoBaseConstants.PROCESS_ID];
                    ldrReportParam[busNeoBaseConstants.USER_ID] = ldtActivityStatistics.Rows[0][busNeoBaseConstants.USER_ID];
                }
                ldsWorkFlowUserSnapshot.Tables.Add(ldtActivityStatistics.Copy());
                ldsWorkFlowUserSnapshot.Tables.Add(ldtbReportParams.Copy());
            }
            else
            {
                DataTable ldtActivityStatistics = DBFunction.DBSelect("entReport.GetAcitvityStatisticsByWorkflowUserSnapshot", new object[4] { astrProcessID, astrUserID, ID, U_name }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtActivityStatistics.TableName = ReportConstants.REPORT_TABLE01;

                ldtActivityStatistics.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                ldtActivityStatistics.Columns.Add(new DataColumn("User_ID", typeof(string)));

                foreach (DataRow rowitem in ldtActivityStatistics.Rows)
                {

                    rowitem[10] = astrProcessID != null ? astrProcessID : "ALL"; ;
                    rowitem[11] = astrUserID != null ? astrUserID : "ALL";
                }
                if (ldtActivityStatistics.IsNotNull() && ldtActivityStatistics.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtActivityStatistics.Rows[0][busNeoBaseConstants.PROCESS_ID];
                    ldrReportParam[busNeoBaseConstants.USER_ID] = ldtActivityStatistics.Rows[0][busNeoBaseConstants.USER_ID];
                }
                ldsWorkFlowUserSnapshot.Tables.Add(ldtActivityStatistics.Copy());
                ldsWorkFlowUserSnapshot.Tables.Add(ldtbReportParams.Copy());
            }




            return ldsWorkFlowUserSnapshot;
        }

        /// <summary>
        /// WorkFlow User History Report Method.
        /// </summary>
        /// Developer:Ashish Saklani 08-06-2020 PIR -ID:1888
        /// Added the following code to handle the parameter scenario of the Report.
        /// <param name="astrUserID"></param>
        /// <param name="aintProcessID"></param>
        /// <returns></returns>
        public DataSet GetWorkFlowUserHistory(DateTime madtStartDate, DateTime madtEndDate, string astrUserID = null, string astrProcessId = null)
        {
            int Id = 0;
            string U_name = string.Empty;
            DataSet ldsWorkFlowUserHistory = new DataSet();
            Collection<IDbDataParameter> lcolParams = new Collection<IDbDataParameter>();
            IDbDataParameter strStart = DBFunction.GetDBParameter("var_START_DATE", DbType.DateTime, madtStartDate);
            lcolParams.Add(strStart);
            IDbDataParameter strEnd = DBFunction.GetDBParameter("var_END_DATE", DbType.DateTime, madtEndDate);
            lcolParams.Add(strEnd);
            IDbDataParameter strUserID = DBFunction.GetDBParameter("var_USER_ID", DbType.String, astrUserID);
            lcolParams.Add(strUserID);
            IDbDataParameter strProcessID = DBFunction.GetDBParameter("var_PROCESS_ID", DbType.Int32, Convert.ToInt32(astrProcessId));
            lcolParams.Add(strProcessID);

            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.START_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.END_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.PROCESS_ID);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.USER_ID);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            if (madtStartDate.Equals(DateTime.MinValue))
            {
                madtStartDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.START_DATE] = madtStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            if (madtEndDate.Equals(DateTime.MinValue))
            {
                madtEndDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.END_DATE] = madtEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }
            // Developer- Rahul Mane
            // Iteration - Main-Iteration10
            // Date - 07-29-2021
            // PIR - 2305 - The system is not displaying the details under the generated reports related to the workflows.
            if (DBFunction.IsOracleConnection() || DBFunction.IsPostgreSQLConnection())
            {
                DBFunction.DBExecuteProcedure("USP_REP_USER_HISTORY", lcolParams, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                DataTable ldtActivityDetails = DBFunction.DBSelect("entReport.RptWorkFlowUserHistoryFirst", iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtActivityDetails.TableName = ReportConstants.REPORT_TABLE01;
                ldtActivityDetails.Columns.Add(new DataColumn("Initiated_Date_from", typeof(DateTime)));
                ldtActivityDetails.Columns.Add(new DataColumn("Initiated_Date_to", typeof(DateTime)));
                ldtActivityDetails.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                ldtActivityDetails.Columns.Add(new DataColumn("User_ID", typeof(string)));

                foreach (DataRow rowitem in ldtActivityDetails.Rows)
                {
                    rowitem[16] = madtStartDate != null ? madtStartDate : DateTime.MinValue;
                    rowitem[17] = madtEndDate != null ? madtEndDate : DateTime.MinValue; ;
                    rowitem[18] = astrProcessId != null ? astrProcessId : "ALL";
                    rowitem[19] = astrUserID != null ? astrUserID : "ALL";

                }
                if (ldtActivityDetails.IsNotNull() && ldtActivityDetails.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtActivityDetails.Rows[0][busNeoBaseConstants.PROCESS_ID];
                    ldrReportParam[busNeoBaseConstants.USER_ID] = ldtActivityDetails.Rows[0][busNeoBaseConstants.USER_ID];
                }
                ldsWorkFlowUserHistory.Tables.Add(ldtActivityDetails.Copy());
                ldsWorkFlowUserHistory.Tables.Add(ldtbReportParams.Copy());

            }
            else
            {
                DataTable ldtActivityDetails = DBFunction.DBSelect("entReport.RptWorkFlowUserHistoryFirst", new object[6] { astrProcessId, madtStartDate, madtEndDate, astrUserID, Id, U_name }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtActivityDetails.TableName = ReportConstants.REPORT_TABLE01;
                ldtActivityDetails.Columns.Add(new DataColumn("Initiated_Date_from", typeof(DateTime)));
                ldtActivityDetails.Columns.Add(new DataColumn("Initiated_Date_to", typeof(DateTime)));
                ldtActivityDetails.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                ldtActivityDetails.Columns.Add(new DataColumn("User_ID", typeof(string)));
                foreach (DataRow rowitem in ldtActivityDetails.Rows)
                {
                    rowitem[16] = madtStartDate != null ? madtStartDate : DateTime.MinValue;
                    rowitem[17] = madtEndDate != null ? madtEndDate : DateTime.MinValue; ;
                    rowitem[18] = astrProcessId != null ? astrProcessId : "ALL";
                    rowitem[19] = astrUserID != null ? astrUserID : "ALL";

                }
                if (ldtActivityDetails.IsNotNull() && ldtActivityDetails.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtActivityDetails.Rows[0][busNeoBaseConstants.PROCESS_ID];
                    ldrReportParam[busNeoBaseConstants.USER_ID] = ldtActivityDetails.Rows[0][busNeoBaseConstants.USER_ID];
                }
                ldsWorkFlowUserHistory.Tables.Add(ldtActivityDetails.Copy());
                ldsWorkFlowUserHistory.Tables.Add(ldtbReportParams.Copy());
            }



            return ldsWorkFlowUserHistory;
        }

        /// <summary>
        /// WorkFlow Process History Report
        /// </summary>
        /// Developer:Ashish Saklani 08-06-2020 PIR -ID:1888
        /// Added the following code to handle the parameter scenario of the Report.
        /// <param name="aintProcessID"></param>
        /// <returns></returns>
        public DataSet LoadWorkFlowProcessHistoryReport(DateTime madtStartDate, DateTime madtEndDate, string astrProcessID = null)
        {
            int dayscount = 0;
            string astrStartDate = string.Empty;
            string astrEndDate = string.Empty;
            string afterReportdata = string.Empty;


            DataSet ldsProcessHistory = new DataSet();
            List<string> somevalueafterupdation = new List<string>();
            Collection<IDbDataParameter> lcolParams = new Collection<IDbDataParameter>();
            IDbDataParameter strStart = DBFunction.GetDBParameter("var_START_DATE", DbType.DateTime, madtStartDate);
            lcolParams.Add(strStart);
            IDbDataParameter strEnd = DBFunction.GetDBParameter("var_END_DATE", DbType.DateTime, madtEndDate);
            lcolParams.Add(strEnd);
            IDbDataParameter strProcessID = DBFunction.GetDBParameter("var_PROCESS_ID", DbType.Int32, Convert.ToInt32(astrProcessID));
            lcolParams.Add(strProcessID);

            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.START_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.END_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.PROCESS_ID);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            if (madtStartDate.Equals(DateTime.MinValue))
            {
                madtStartDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.START_DATE] = madtStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            if (madtEndDate.Equals(DateTime.MinValue))
            {
                madtEndDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.END_DATE] = madtEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            // Developer- Rahul Mane
            // Iteration - Main-Iteration10
            // Date - 07-29-2021
            // PIR - 2305 - The system is not displaying the details under the generated reports related to the workflows.
            if (DBFunction.IsOracleConnection() || DBFunction.IsPostgreSQLConnection())
            {

                DBFunction.DBExecuteProcedure("USP_REP_PRCS_HIST", lcolParams, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                DataTable ldtPrcssHist = DBFunction.DBSelect("entReport.LoadProcessHistoryResult", iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtPrcssHist.TableName = ReportConstants.REPORT_TABLE01;
                ldtPrcssHist.Columns.Add(new DataColumn("Initiated_Date_from", typeof(DateTime)));
                ldtPrcssHist.Columns.Add(new DataColumn("Initiated_Date_to", typeof(DateTime)));
                ldtPrcssHist.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                foreach (DataRow rowitem in ldtPrcssHist.Rows)
                {
                    rowitem[11] = madtStartDate != null ? madtStartDate : DateTime.MinValue;
                    rowitem[12] = madtEndDate != null ? madtEndDate : DateTime.MinValue; ;
                    rowitem[13] = astrProcessID != null ? astrProcessID : "ALL";
                }
                if (ldtPrcssHist.IsNotNull() && ldtPrcssHist.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtPrcssHist.Rows[0][busNeoBaseConstants.PROCESS_ID];
                }
                ldsProcessHistory.Tables.Add(ldtPrcssHist.Copy());
                ldsProcessHistory.Tables.Add(ldtbReportParams.Copy());
            }
            else
            {
                DataTable ldtPrcssHist = DBFunction.DBSelect("entReport.LoadProcessHistoryResult", new object[3] { astrProcessID, madtStartDate, madtEndDate }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtPrcssHist.TableName = ReportConstants.REPORT_TABLE01;

                ldtPrcssHist.Columns.Add(new DataColumn("Initiated_Date_from", typeof(DateTime)));
                ldtPrcssHist.Columns.Add(new DataColumn("Initiated_Date_to", typeof(DateTime)));
                ldtPrcssHist.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                foreach (DataRow rowitem in ldtPrcssHist.Rows)
                {
                    rowitem[11] = madtStartDate != null ? madtStartDate : DateTime.MinValue;
                    rowitem[12] = madtEndDate != null ? madtEndDate : DateTime.MinValue; ;
                    rowitem[13] = astrProcessID != null ? astrProcessID : "ALL";
                }
                if (ldtPrcssHist.IsNotNull() && ldtPrcssHist.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtPrcssHist.Rows[0][busNeoBaseConstants.PROCESS_ID];
                }
                ldsProcessHistory.Tables.Add(ldtPrcssHist.Copy());
                ldsProcessHistory.Tables.Add(ldtbReportParams.Copy());
            }
            return ldsProcessHistory;
        }

        #region [BPM Perforamce Metrics reports]
        // Developer- Rameshwar Landge
        // Iteration - Main-Iteration14
        // Date - 10-16-2024
        /// <summary>
        /// 
        /// </summary>
        /// <param name="adtmStartDate"></param>
        /// <param name="adtmEndDate"></param>
        /// <param name="astrProcessID"></param>
        /// <returns></returns>
        public DataSet ProcessLevelPerformanceMetrics(DateTime adtmStartDate, DateTime adtmEndDate, string astrProcessID, DateTime adtmProcessCompletionForm = default(DateTime), DateTime adtmProcessCompletionTo = default(DateTime))
        {
            int lintProcessId = 0;
            if (astrProcessID.IsNull())
                lintProcessId = 0;
            else if (Int32.TryParse(astrProcessID, out lintProcessId))
                lintProcessId = Convert.ToInt32(astrProcessID);


            DataSet ldsResult = new DataSet();
            if (adtmStartDate != null && adtmEndDate != null)
            {
                AdjustDate(ref adtmStartDate);
                AdjustDate(ref adtmEndDate, true);
                AdjustDate(ref adtmProcessCompletionForm);
                AdjustDate(ref adtmProcessCompletionTo, true);

                // below code used for parameter data display on report 
                DataTable ldtbReportParams = new DataTable(ReportConstants.REPORT_TABLE02)
                {
                    Columns =
                    {
                                busNeoBaseConstants.START_DATE,
                                busNeoBaseConstants.END_DATE,
                                busNeoBaseConstants.PROCESS
                    }
                };
                DataRow ldrReportParam = ldtbReportParams.NewRow();
                ldrReportParam[busNeoBaseConstants.START_DATE] = adtmStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
                ldrReportParam[busNeoBaseConstants.END_DATE] = adtmEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
                if (lintProcessId > 0)
                {
                    DataTable ldtbProcessName = DBFunction.DBSelect("entReport.GetProcessNameByProcessID", new object[1] { lintProcessId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (ldtbProcessName.IsNotNull() && ldtbProcessName.Rows.Count > 0)
                    {
                        ldrReportParam[busNeoBaseConstants.PROCESS] = ldtbProcessName.Rows[0][busNeoBaseConstants.NAME];
                    }
                }
                else
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS] = busNeoBaseConstants.ALLVALUE;
                }
                ldtbReportParams.Rows.Add(ldrReportParam);

                /// Below data table code is used for showing report data.
                DataTable ldtbReportData0 = busBase.Select(
               "entReport.BPMProcessLevelPerformanceMetrics",
                new object[] { adtmStartDate, adtmEndDate, astrProcessID ,adtmProcessCompletionForm,adtmProcessCompletionTo});



                if (ldtbReportData0 != null && ldtbReportParams != null)
                {

                    ldtbReportData0.TableName = ReportConstants.REPORT_TABLE01;
                    ldsResult.Tables.Add(ldtbReportData0.Copy());
                    ldsResult.Tables.Add(ldtbReportParams.Copy());
                }
            }
            return ldsResult;
        }


        /// <summary>
        /// Process Level Board Goal Report
        /// </summary>
        /// <param name="adtmStartDate"></param>
        /// <param name="adtmEndDate"></param>
        /// <returns></returns>
        public DataSet ProcessLevelBoardGoalReport(DateTime adtmStartDate, DateTime adtmEndDate, string astrProcessID, DateTime adtmProcessCompletionForm = default(DateTime), DateTime adtmProcessCompletionTo = default(DateTime))
        {
            DataSet ldsResult = new DataSet();
            if (adtmStartDate != null && adtmEndDate != null)
            {
                AdjustDate(ref adtmStartDate);
                AdjustDate(ref adtmEndDate, true);
                AdjustDate(ref adtmProcessCompletionForm);
                AdjustDate(ref adtmProcessCompletionTo, true);

                // below code used for parameter data display on report 
                DataTable ldtbReportParams = new DataTable(ReportConstants.REPORT_TABLE02)
                {
                    Columns =
                    {
                                busNeoBaseConstants.START_DATE,
                                busNeoBaseConstants.END_DATE,
                                
                    }
                };
                DataRow ldrReportParam = ldtbReportParams.NewRow();
                ldrReportParam[busNeoBaseConstants.START_DATE] = adtmStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
                ldrReportParam[busNeoBaseConstants.END_DATE] = adtmEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
               
                ldtbReportParams.Rows.Add(ldrReportParam);

                /// Below data table code is used for showing report data.
                DataTable ldtbReportData0 = busBase.Select(
               "entReport.RptProcessLevelBoardGoalReport",
                new object[] { adtmStartDate, adtmEndDate,adtmProcessCompletionForm,adtmProcessCompletionTo,astrProcessID});

                if (ldtbReportData0 != null && ldtbReportParams != null)
                {
                    ldtbReportData0.TableName = ReportConstants.REPORT_TABLE01;
                    ldsResult.Tables.Add(ldtbReportData0.Copy());
                    ldsResult.Tables.Add(ldtbReportParams.Copy());
                }
            }
            return ldsResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adtmStartDate"></param>
        /// <param name="adtmEndDate"></param>
        /// <returns></returns>
        public DataSet ActivityLevelPerformanceMetrics(DateTime adtmStartDate, DateTime adtmEndDate, DateTime adtmActivityCompletionDateFrom= default(DateTime), DateTime ActivityCompletionDateTo= default(DateTime), string astrProcessID = null, string astrActivityID = null, string astrActivityInstanceStatus = null, string astrUserID = null, string astrPersonID = null, string astrOrgID =null)
        {
            DataSet ldsResult = new DataSet();
            if (adtmStartDate != null && adtmEndDate != null && adtmActivityCompletionDateFrom != null && ActivityCompletionDateTo != null)
            {
                AdjustDate(ref adtmStartDate);
                AdjustDate(ref adtmEndDate, true);
                AdjustDate(ref adtmActivityCompletionDateFrom);
                AdjustDate(ref ActivityCompletionDateTo, true);
                // below code used for parameter data display on report 
                DataTable ldtbReportParams = new DataTable(ReportConstants.REPORT_TABLE02)
                {
                    Columns =
                    {
                                busNeoBaseConstants.START_DATE,
                                busNeoBaseConstants.END_DATE,

                    }
                };
                DataRow ldrReportParam = ldtbReportParams.NewRow();
                ldrReportParam[busNeoBaseConstants.START_DATE] = adtmStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
                ldrReportParam[busNeoBaseConstants.END_DATE] = adtmEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
                ldtbReportParams.Rows.Add(ldrReportParam);

                /// Below data table code is used for showing report data.
                /// Activity Header query
               
                DataTable ldtbReportData01 = busBase.Select(
               "entReport.ActivityLevelPerformanceMetricc",
                new object[] { adtmStartDate, adtmEndDate,adtmActivityCompletionDateFrom,ActivityCompletionDateTo,astrProcessID,astrActivityID,astrActivityInstanceStatus,astrUserID,astrPersonID, astrOrgID });


                // activity detail query. 
                DataTable ldtbReportData03 = busBase.Select(
               "entReport.ActivityLevelPerformanceMetricsFooterQuery",
                new object[] { adtmStartDate, adtmEndDate, adtmActivityCompletionDateFrom, ActivityCompletionDateTo, astrProcessID, astrActivityID, astrActivityInstanceStatus, astrUserID , astrPersonID, astrOrgID });

                if (ldtbReportData01 != null)
                {
                    ldtbReportData01.TableName = ReportConstants.REPORT_TABLE01;
                    ldsResult.Tables.Add(ldtbReportData01.Copy());
                    
                }
                if(ldtbReportParams != null )
                {
                    ldsResult.Tables.Add(ldtbReportParams.Copy());
                }
                if(ldtbReportData03 != null )
                {
                    ldtbReportData03.TableName = ReportConstants.REPORT_TABLE03;
                    ldsResult.Tables.Add(ldtbReportData03.Copy());
                }
            }
            return ldsResult;
        }

        #endregion

        #region [Common Method]
        // Developer- Rameshwar Landge
        // Iteration - Main-Iteration14
        // Date - 18 Nov 2024
        /// <summary>
        ///  The start date is set to midnight, and the end date is set to midnight using this method.
        /// </summary>
        /// <param name="adtmdate"></param>
        /// <param name="aisEndDate"></param>
        private void AdjustDate(ref DateTime adtmdate, bool aisEndDate = false)
        {
            if (adtmdate != DateTime.MinValue)
            {
                adtmdate = aisEndDate ? adtmdate.Date.AddDays(1).AddSeconds(-1) : adtmdate.Date;
            }
        }
        #endregion
    }
}
