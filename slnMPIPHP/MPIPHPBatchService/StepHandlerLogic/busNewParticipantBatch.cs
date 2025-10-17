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

namespace MPIPHPJobService.StepHandlerLogic
{
    public class busNewParticipantBatch : busBatchHandler
    {
        #region Properties
        public busMainBase ibusBaseActivityInstance { get; set; }
        private object iobjLock = null;
        DataTable ldtbWorkInformationForAllParticipants { get; set; }
        DataTable ldtPersonBatchFlag { get; set; }
        DataTable ldtbPersonAccounts { get; set; }
        #endregion

        public busNewParticipantBatch()
        {
        }

        #region New Participant Batch
        public void NewParticipantBatch()
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
            iobjLock = new object();

            DataTable ldtPersonList = busBase.Select("cdoPaymentHistoryHeader.GetNewParticipant", new object[0]);
            ldtbPersonAccounts = busBase.Select("cdoPaymentHistoryHeader.GetNewParticipantPersonAccounts", new object[0]);
           
            if (ldtPersonList.Rows.Count > 0)
            {
                ldtbWorkInformationForAllParticipants = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkHistoryForNewMpippParticipant", astrLegacyDBConnection, null, lParameters);

                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

                Parallel.ForEach(ldtPersonList.AsEnumerable(), po, (acdoPerson, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "NewParticipantBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    ProcessNewParticipantBatch(acdoPerson, lobjPassInfo, lintCount, lintTotalCount);

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

        #region Process New Participant Batch
        public void ProcessNewParticipantBatch(DataRow acdoPerson, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        {
            ArrayList aarrResult = new ArrayList();
            Hashtable ahtbQueryBkmarks = new Hashtable();
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            //int lintComputationYear = 0;

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
            busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };

            try
            {
                lbusPersonOverview.icdoPerson.LoadData(acdoPerson);
                lbusPersonOverview.LoadPersonAddresss();
                lbusPersonOverview.LoadPersonContacts();
                lbusPersonOverview.LoadCorrAddress();

                lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusPersonOverview.lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonOverview.lbusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusPersonOverview.icdoPerson.idtDateofBirth, iobjSystemManagement.icdoSystemManagement.batch_date);
                lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson = lbusPersonOverview.icdoPerson;

                if (ldtbWorkInformationForAllParticipants.Rows.Count > 0 && ldtbWorkInformationForAllParticipants.IsNotNull() && acdoPerson[enmPerson.ssn.ToString()] != DBNull.Value)
                {
                    string lstrSSNDecrypted = Convert.ToString(acdoPerson[enmPerson.ssn.ToString()]);
                    DataRow[] ldrTempWorkInfo = ldtbWorkInformationForAllParticipants.FilterTable(utlDataType.String, "ssn", lstrSSNDecrypted);
                    DataRow[] ldrPersonAccounts = ldtbPersonAccounts.FilterTable(utlDataType.String, "person_id", lbusPersonOverview.icdoPerson.person_id.ToString());
                    busPersonAccount lbusPersonAccount = new busPersonAccount();
                    lbusPersonOverview.lbusBenefitApplication.ibusPerson.iclbPersonAccount = lbusPersonAccount.GetCollection<busPersonAccount>(ldrPersonAccounts, "icdoPersonAccount");

                    if (!ldrTempWorkInfo.IsNullOrEmpty())
                    {
                        lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldrTempWorkInfo);
                        lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_IAP = new Collection<cdoDummyWorkData>();
                        lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date = DateTime.Now;
                    }
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
                aarrResult.Add(lbusPersonOverview);
                
                if (!lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                {
                    lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count();

                    if (lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count == 1)
                    {
                        string str = this.CreateCorrespondence(busConstant.NEW_PARTICIPANT_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);

                        lbusPersonOverview.icdoPerson.new_participant_letter_send_flag = busConstant.FLAG_YES;
                        lbusPersonOverview.icdoPerson.Update();
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
        #endregion
    }
}
