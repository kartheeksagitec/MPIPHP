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
    public class busEGWBHealthEligiblityBatch : busBatchHandler
    {
        #region Properties

        //public Collection<cdoDummyWorkData> iclbHealthWorkHistory { get; set; }
        public Collection<cdoPerson> lclbPerson { get; set; }
        decimal idecAge { get; set; }
        public busMainBase ibusBaseActivityInstance { get; set; }
        private object iobjLock = null;
        public busSystemManagement iobjSystemManagement { get; set; }
        #endregion

        #region EGWP DETAILS

        public DataTable idtPersonAccount { get; set; }
        //PIR 1024
        public int iintYear { get; set; }

        //PIR 1024
        private void RetrieveBatchParameters()
        {
            if (ibusJobHeader != null)
            {
                if (ibusJobHeader.iclbJobDetail == null)
                    ibusJobHeader.LoadJobDetail(true);

                //foreach (busJobDetail lobjDetail in ibusJobHeader.iclbJobDetail)
                //{
                //    foreach (busJobParameters lobjParam in lobjDetail.iclbJobParameters)
                //    {
                //        switch (lobjParam.icdoJobParameters.param_name)
                //        {
                //            case busConstant.JobParamBatchYear:
                //                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty())
                //                    iintYear = Convert.ToInt32(lobjParam.icdoJobParameters.param_value);
                //                break;
                //        }
                //    }
                //}
                iintYear = DateTime.Now.Year;
            }
        }
        public void ProcessEGWPHealthEligibilityBatch()
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

            RetrieveBatchParameters();
                        
            DataTable ldtPersonList = busBase.Select("cdoPerson.GetEWGPParticipants", new object[0]);
            
            if (ldtPersonList.Rows.Count > 0)
            {
                #region Changes after F/w Upgrade
                IDbConnection lconLegacy = null;
                if (lconLegacy == null)
                {
                    lconLegacy = DBFunction.GetDBConnection("Legacy");
                }
                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                if (astrLegacyDBConnection != null)
                {
                    IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
                    lobjParameter.ParameterName = "@Year";
                    lobjParameter.DbType = DbType.Int32;
                    lobjParameter.Value = iintYear;
                    lcolParameters.Add(lobjParameter);

                    IDbDataParameter lobjBatchParameter = DBFunction.GetDBParameter();
                    lobjBatchParameter.ParameterName = "@Is_EGWP";
                    lobjBatchParameter.DbType = DbType.String;
                    lobjBatchParameter.Value = 'Y';
                    lcolParameters.Add(lobjBatchParameter);
                }
                DBFunction.DBExecuteProcedure("usp_GetHealthWorkHistoryForAllMpippParticipant", lcolParameters, lconLegacy, null);
                lconLegacy.Close();
                #endregion

                idtPersonAccount = busBase.Select("cdoDataExtractionBatchInfo.GetEGWPPersonAccount", new object[0]);                

                foreach (DataRow acdoPerson in ldtPersonList.Rows)
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "EGWPHealthEligibilityBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;
                                       
                    CalculateHealthEligibilityandCheckNewWorkingPeople(acdoPerson, lobjPassInfo, lintCount, lintTotalCount);

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;                    
                }
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

                        string lstrRetrHlthElgblFlg = string.Empty;                   
                        
                        if (lbusPersonOverview.icdoPerson.health_eligible_flag != null)
                        {
                            lstrRetrHlthElgblFlg = lbusPersonOverview.icdoPerson.health_eligible_flag;
                        }
                                                
                        if(lstrRetrHlthElgblFlg == "Y")
                        {
                            cdoEgwpDetails lcdoEgwpDetails = new cdoEgwpDetails();
                            lcdoEgwpDetails.mpi_person_id = Convert.ToString(acdoPerson["MPI_PERSON_ID"]);
                            lcdoEgwpDetails.retirement_date = Convert.ToDateTime(acdoPerson["RETIREMENT_DATE"]);
                            lcdoEgwpDetails.retirement_status = Convert.ToString(acdoPerson["RETIREMENT_STATUS"]);
                            lcdoEgwpDetails.disabled_flag = Convert.ToString(acdoPerson["DISABLED_FLAG"]);
                            lcdoEgwpDetails.status_date = Convert.ToDateTime(acdoPerson["STATUS_DATE"]);
                            lcdoEgwpDetails.created_by = ibusJobHeader.icdoJobHeader.created_by;
                            lcdoEgwpDetails.created_date = ibusJobHeader.icdoJobHeader.created_date;
                            lcdoEgwpDetails.modified_by = ibusJobHeader.icdoJobHeader.modified_by;
                            lcdoEgwpDetails.modified_date = ibusJobHeader.icdoJobHeader.modified_date;
                            lcdoEgwpDetails.Insert();
                        }                                                                          
                        
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
    }
}
