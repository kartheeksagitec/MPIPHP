using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using System.Linq;
using Sagitec.Common;
using System.Threading.Tasks;
using Sagitec.ExceptionPub;

namespace MPIPHPJobService
{
    public class busIAPHardshipPaybackBatch : busBatchHandler
    {
        #region Properties
        public Collection<cdoDummyWorkData> iclbHealthWorkHistory { get; set; }
        public Collection<cdoPerson> lclbPerson { get; set; }
        decimal idecAge { get; set; }
        public busMainBase ibusBaseActivityInstance { get; set; }
        private object iobjLock = null;
        public int PAYEE_ACCOUNT_ID { get; set; }

        #endregion

        #region IAP Hardship Payback Batch
        public void IAPHardshipPaybackBatch()
        {
            ArrayList larrPersonIAPAdded = new ArrayList();
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

            this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;

            //Get all Payee Accounts that have paid back some of their Covid 19 IAP Hardship Withdrawal
            DataTable ldtPayeeInformation = busBase.Select("cdoPayeeAccount.LoadIAPHardshipPaybackBatch", new object[0]);

            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 1;// System.Environment.ProcessorCount * 4;


            Parallel.ForEach(ldtPayeeInformation.AsEnumerable(), po, (acdoPayeeAccount, loopState) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "IAPHardshipPaybackBatch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;

                GenerateCorrespondence(acdoPayeeAccount, lobjPassInfo, larrPersonIAPAdded, lintCount, lintTotalCount);

                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;
            });

            utlPassInfo.iobjPassInfo = lobjMainPassInfo;

            MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Correspondence_Path,
                              iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_PYBK, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);
        }
        #endregion

        #region GenerateCorrespondence
        private void GenerateCorrespondence(DataRow acdoPayeeAccount, utlPassInfo autlPassInfo, ArrayList arrPersonIAPAdded, int aintCount,
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

            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            lbusPayeeAccount.icdoPayeeAccount.LoadData(acdoPayeeAccount);
            lbusPayeeAccount.FindPayeeAccount(lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
            lbusPayeeAccount.LoadLastPaymentDate();
            lbusPayeeAccount.LoadPayeeAccountIAPPaybacks();
            lbusPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
            lbusPayeeAccount.ibusPayee.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id);
            lbusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
            lbusPayeeAccount.ibusParticipant.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id);
            lbusPayeeAccount.LoadCorresProperties(busConstant.IAP_PAYBACK_ANNUAL_BATCH_LETTER);  //Not sure if we need to do this or not...

            autlPassInfo.BeginTransaction();
            try
            {
                ArrayList aarrResult = new ArrayList();
                Hashtable ahtbQueryBkmarks1 = new Hashtable();

                aarrResult.Add(lbusPayeeAccount);

                #region Generated Correspondence and WorkFlow
                this.CreateCorrespondence(busConstant.IAP_PAYBACK_ANNUAL_BATCH_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks1, busConstant.BOOL_TRUE);
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
        }
        #endregion
    }
}
