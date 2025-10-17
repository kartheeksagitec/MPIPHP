using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MPIPHP.BusinessObjects;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using System.Collections.ObjectModel;
using Sagitec.DBUtility;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace MPIPHP.BusinessTier
{
    public class srvStudio : srvMPIPHP
    {
        public srvStudio()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static Collection<IDbDataParameter> aarrParameter = new Collection<IDbDataParameter>();

        public void AddProcessRecord(string astrMapName, string astrTypeId, string astrStatusId, string astrTypeValue, ArrayList aalDisplayName)
        {
            iobjPassInfo.BeginTransaction();
            try
            {
                if (IsProcessExists(astrMapName))
                {
                    iobjPassInfo.Rollback();
                    throw new Exception("In SGW_PROCESS table Process with same name exists. Please Check.");
                }

                int maxProcessID = AddProcessInDB(astrMapName, astrTypeId, astrStatusId, astrTypeValue);
                if (maxProcessID < 0)
                {
                    iobjPassInfo.Rollback();
                    throw new Exception("Error occured during retrieval of max Process ID.");
                }

                UpdateActivities(aalDisplayName, maxProcessID, true);
                iobjPassInfo.Commit();
            }
            finally
            {
            }
        }

        public void RefreshActivities(string astrMapName, ArrayList aalDisplayName, bool ablnReInsertActivities)
        {
            int iProcessID = GetProcessID(astrMapName);
            if (iProcessID < 0)
            {
                throw new Exception("No Process Id found in Database. Operation cancelled.");
            }
            UpdateActivities(aalDisplayName, iProcessID, ablnReInsertActivities);
        }

        private int AddProcessInDB(string astrProcessName, string strTypeId, string strStatusId, string strTypeValue)
        {
            string strDescription = astrProcessName.Remove(0, 3);
            Match mCaps = Regex.Match(strDescription, "[A-Z]");
            if (mCaps != null)
            {
                mCaps = mCaps.NextMatch();
                if ((mCaps != null) && (mCaps.Index > 1))
                {
                    strDescription = strDescription.Substring(0, mCaps.Index) + " " + strDescription.Substring(mCaps.Index);
                }
            }
            string strProcessCode = string.Empty;
            int maxProcessID = GetMaxProcessID();

            if (maxProcessID > -1)
            {
                string strSqlProcessInsert = "INSERT INTO SGW_PROCESS" +
                    "(PROCESS_ID,DESCRIPTION,NAME,PRIORITY,TYPE_ID,TYPE_VALUE,STATUS_ID,STATUS_VALUE,USE_NEW_MAP_FLAG ,CREATED_BY ,CREATED_DATE ,MODIFIED_BY,MODIFIED_DATE ,UPDATE_SEQ)" +
                    "VALUES("
                    + maxProcessID.ToString() + ","
                    + "'" + strDescription + "',"
                    + "'" + astrProcessName + "',"
                    + "0,"
                    + strTypeId + ","
                    + "'" + strTypeValue + "',"
                    + strStatusId + ","
                    + "'ACT'" + ","
                    + "NULL" + ","
                    + "'Studio'" + ","
                    + "GETDATE()" + ","
                    + "'Studio'" + ","
                    + "GETDATE()" + ","
                    + "0"
                    + ")";

                DBFunction.DBSelect(strSqlProcessInsert, aarrParameter, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            return maxProcessID;
        }

        private void UpdateActivities(ArrayList aalDisplayName, int aintProcessID, bool iblnReInsertActivities)
        {
            ArrayList arrActivityFound = new ArrayList();
            if (iblnReInsertActivities)
            {
                DeleteActivities(aintProcessID);
            }

            int maxActivityID = -1;
            bool blnActivityAdded = false;
            for (int i = 0; i < aalDisplayName.Count; i++)
            {
                int activityID = -1;
                int sortOrder = -1;
                blnActivityAdded = false;
                string strDisplayName = Convert.ToString(aalDisplayName[i]);
                if ((iblnReInsertActivities) || (!IsActivityExists(strDisplayName, aintProcessID, out activityID, out sortOrder)))
                {
                    AddActivityInDB(strDisplayName, i + 1, aintProcessID, ref maxActivityID);
                    blnActivityAdded = true;
                    activityID = maxActivityID;
                }
                arrActivityFound.Add(activityID);
                if ((!blnActivityAdded) && (sortOrder != i + 1))
                {
                    UpdateSortOrder(aintProcessID, activityID, i + 1);
                }
            }

            if (!iblnReInsertActivities)
            {
                UpdateActivitiesInDB(arrActivityFound, aintProcessID);
            }
        }

        private void AddActivityInDB(string astrDispName, int aindex, int aintProcessID, ref int maxActivityID)
        {
            if (maxActivityID < 0)
            {
                string strSqlMaxActivityID = "SELECT MAX(ACTIVITY_ID) AS MAX_ACTIVITY_ID FROM SGW_ACTIVITY";
                DataTable dtFileName = DBFunction.DBSelect(strSqlMaxActivityID, aarrParameter, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (dtFileName.Rows[0]["MAX_ACTIVITY_ID"] is DBNull)
                {
                    maxActivityID = 1;
                }
                else
                {
                    maxActivityID = int.Parse(dtFileName.Rows[0]["MAX_ACTIVITY_ID"].ToString()) + 1;
                }
            }
            else
            {
                maxActivityID = maxActivityID + 1;
            }

            string strSqlProcessInsert = "INSERT INTO SGW_ACTIVITY" +
                "(ACTIVITY_ID,PROCESS_ID,NAME,DISPLAY_NAME,STANDARD_TIME_IN_MINUTES,ROLE_ID,SUPERVISOR_ROLE_ID,SORT_ORDER,IS_DELETED_FLAG ,CREATED_BY ,CREATED_DATE ,MODIFIED_BY,MODIFIED_DATE ,UPDATE_SEQ,ALLOW_INDEPENDENT_COMPLETE_IND)" +
                "VALUES("
                + maxActivityID.ToString() + ","
                + aintProcessID.ToString() + ","
                + "'" + astrDispName + "',"
                + "'" + astrDispName + "',"
                + "0,"
                + "0,"
                + "0,"
                + aindex.ToString() + ","
                + "NULL,"
                + "'Studio'" + ","
                + "GETDATE()" + ","
                + "'Studio'" + ","
                + "GETDATE()" + ","
                + "0" + ","
                + "'N'"
                + ")";

            DBFunction.DBSelect(strSqlProcessInsert, aarrParameter, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        private void UpdateActivitiesInDB(ArrayList arrActivityFound, int aintProcessID)
        {
            string strActivityIDs = string.Empty;
            foreach (int actId in arrActivityFound)
            {
                strActivityIDs += actId.ToString() + ",";
            }
            if (!string.IsNullOrEmpty(strActivityIDs))
            {
                strActivityIDs = strActivityIDs.Substring(0, strActivityIDs.Length - 1);
            }
            string strSqlActivityUpdate = string.Empty;
            if (!string.IsNullOrEmpty(strActivityIDs))
            {
                strSqlActivityUpdate = "UPDATE SGW_ACTIVITY SET IS_DELETED_FLAG='Y', MODIFIED_DATE=GETDATE(), MODIFIED_BY='Studio' WHERE PROCESS_ID=" + aintProcessID + " AND ACTIVITY_ID NOT IN (" + strActivityIDs + ")";
            }
            else
            {
                strSqlActivityUpdate = "UPDATE SGW_ACTIVITY SET IS_DELETED_FLAG='Y', MODIFIED_DATE=GETDATE(), MODIFIED_BY='Studio' WHERE PROCESS_ID=" + aintProcessID;
            }
            DBFunction.DBSelect(strSqlActivityUpdate, aarrParameter, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        private void UpdateSortOrder(int aintProcessID, int aintActivityID, int aintSortOrder)
        {
            string strSqlActivityUpdate = "UPDATE SGW_ACTIVITY SET SORT_ORDER =" + aintSortOrder + ", MODIFIED_DATE=GETDATE(), MODIFIED_BY='Studio' WHERE PROCESS_ID=" + aintProcessID + " AND ACTIVITY_ID =" + aintActivityID;
            DBFunction.DBSelect(strSqlActivityUpdate, aarrParameter, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        private int GetProcessID(string astrProcessName)
        {
            int processID = -1;
            try
            {
                string strSqlProcessID = "SELECT PROCESS_ID FROM SGW_PROCESS WHERE NAME='" + astrProcessName + "'";
                DataTable dtFileName = DBFunction.DBSelect(strSqlProcessID, iobjPassInfo.iconFramework);
                if (!(dtFileName.Rows[0]["PROCESS_ID"] is DBNull))
                {
                    processID = int.Parse(dtFileName.Rows[0]["PROCESS_ID"].ToString());
                }
                return processID;
            }
            catch
            {
                return -1;
            }
        }

        private bool IsActivityExists(string astrActivityName, int aintProcessID, out int aintActivityID, out int aintSortOrder)
        {
            try
            {
                aintActivityID = -1;
                aintSortOrder = -1;
                string strSqlProcessID = "SELECT ACTIVITY_ID, SORT_ORDER FROM SGW_ACTIVITY WHERE PROCESS_ID=" + aintProcessID + " AND NAME='" + astrActivityName + "'";
                DataTable dtFileName = DBFunction.DBSelect(strSqlProcessID, aarrParameter, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (dtFileName.Rows.Count > 0)
                {
                    if (!(dtFileName.Rows[0]["ACTIVITY_ID"] is DBNull))
                    {
                        int.TryParse(dtFileName.Rows[0]["ACTIVITY_ID"].ToString(), out aintActivityID);
                    }
                    if (!(dtFileName.Rows[0]["SORT_ORDER"] is DBNull))
                    {
                        int.TryParse(dtFileName.Rows[0]["SORT_ORDER"].ToString(), out aintSortOrder);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsProcessExists(string astrProcessName)
        {
            try
            {
                string strSqlProcessID = "SELECT PROCESS_ID FROM SGW_PROCESS WHERE NAME='" + astrProcessName + "'";
                DataTable dtFileName = DBFunction.DBSelect(strSqlProcessID, aarrParameter, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (dtFileName.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int GetMaxProcessID()
        {
            int maxProcessID = -1;
            try
            {
                string strSqlMaxProcessID = "SELECT MAX(PROCESS_ID) AS MAX_PROCESS_ID FROM SGW_PROCESS";
                DataTable dtFileName = DBFunction.DBSelect(strSqlMaxProcessID, aarrParameter, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (dtFileName.Rows[0]["MAX_PROCESS_ID"] is DBNull)
                {
                    maxProcessID = 1;
                }
                else
                {
                    maxProcessID = int.Parse(dtFileName.Rows[0]["MAX_PROCESS_ID"].ToString()) + 1;
                }
                return maxProcessID;
            }
            catch
            {
                return -1;
            }
        }

        private void DeleteActivities(int aintProcessID)
        {
            string strSqlProcessID = "DELETE  FROM SGW_ACTIVITY WHERE PROCESS_ID=" + aintProcessID;
            DataTable dtFileName = DBFunction.DBSelect(strSqlProcessID, aarrParameter, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        #region NeocertifyChanges
        public bool UpdateNeoCertifyOtherTable(object aobjBase, int aintScenarioOtherID, string astrObjectType)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter myBF = new BinaryFormatter();
                myBF.Serialize(ms, aobjBase);
                ms.Flush();

                IDbConnection NeoCertifyConnection = DBFunction.GetDBConnection("NeoCertify");
                if (NeoCertifyConnection != null)
                {
                    NeoCertifyConnection.Open();
                    string strUpdateSql = string.Empty;
                    if (astrObjectType == "SAVE")
                    {
                        strUpdateSql = "UPDATE SGT_TEST_SCENARIO_OTHER SET SAVE_OBJECT = @OBJ WHERE SCENARIO_ID = @ID";
                    }
                    else if (astrObjectType == "ACTION")
                    {
                        strUpdateSql = "UPDATE SGT_TEST_SCENARIO_OTHER SET INVOKE_METHOD_OBJECT = @OBJ WHERE SCENARIO_ID = @ID";
                    }
                    else
                    {
                        strUpdateSql = "UPDATE SGT_TEST_SCENARIO_OTHER SET INITIAL_LOAD_OBJECT = @OBJ WHERE SCENARIO_ID = @ID";
                    }
                    Collection<IDbDataParameter> larrParameter = new Collection<IDbDataParameter>();
                    IDbDataParameter lobjDbParameter = DBFunction.GetDBParameter();
                    lobjDbParameter.ParameterName = "@OBJ";
                    lobjDbParameter.DbType = DbType.Binary;
                    lobjDbParameter.Value = ms.ToArray();
                    larrParameter.Add(lobjDbParameter);

                    lobjDbParameter = DBFunction.GetDBParameter();
                    lobjDbParameter.ParameterName = "@ID";
                    lobjDbParameter.DbType = DbType.Int32;
                    lobjDbParameter.Value = aintScenarioOtherID;
                    larrParameter.Add(lobjDbParameter);
                    DBFunction.DBNonQuery(strUpdateSql, larrParameter, NeoCertifyConnection, null);
                    if (NeoCertifyConnection.State == ConnectionState.Open)
                    {
                        NeoCertifyConnection.Close();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }    
        # endregion


    }
}
