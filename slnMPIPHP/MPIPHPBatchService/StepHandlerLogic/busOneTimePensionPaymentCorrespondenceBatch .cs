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
using System.Collections.Concurrent;
using iTextSharp.text.pdf;
using iTextSharp.text;
using MPIPHP.BusinessTier;

namespace MPIPHPJobService
{
    class busOneTimePensionPaymentCorrespondenceBatch : busBatchHandler
    {
        #region Properties
        private object iobjLock = null;
        ConcurrentBag<busPayeeAccount> iclbPayeeAccount = new ConcurrentBag<busPayeeAccount>();

               #endregion

        #region Public Methods

       
        public void OneTimePensionPaymentCorrespondenceBatch()
        {
            int lintCount = 0;
            int lintTotalCount = 0;

            utlConnection utlLegacyDBConnection = HelperFunction.GetDBConnectionProperties("core");
            string astrLegacyDBConnection = utlLegacyDBConnection.istrConnectionString;

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();

            DataTable ldtbPersonAccountInfo = new DataTable();

            ldtbPersonAccountInfo = busBase.Select("cdoOnetimeRetireePaymentContract.GetPayeeAccountIdsforOneTimePaymentCorrespondence", new object[0] { });

            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            if (ldtbPersonAccountInfo != null && ldtbPersonAccountInfo.Rows.Count > 0)
            {
                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 2;

                Parallel.ForEach(ldtbPersonAccountInfo.AsEnumerable(), po, (acdoPerson, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "OneTimePensionPaymentCorrespondenceBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    GenerateOneTimePaymentCorrespondence(acdoPerson, lobjPassInfo, lintCount, lintTotalCount);

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;
                });

                #region Merged PDF

                MergePdfsFilesFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Correspondence_Path,
                              iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Merged_Correspondence_path, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);



                #endregion



                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;


            }

        }

      

        public void GenerateOneTimePaymentCorrespondence(DataRow acdoPayeeAccount, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        {
            ArrayList aarrResult = new ArrayList();
            Hashtable ahtbQueryBkmarks = new Hashtable();
           
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

            int lpayeeAccountId = Convert.ToInt32(acdoPayeeAccount["PAYEE_ACCOUNT_ID"]);

             // autlPassInfo.BeginTransaction();
            try
            {

                busPayeeAccount lbusPayeeAccount = new busPayeeAccount{  icdoPayeeAccount = new cdoPayeeAccount() };
                lbusPayeeAccount.FindPayeeAccount(lpayeeAccountId);
                lbusPayeeAccount.LoadGrossAmount();

                if (lbusPayeeAccount.icdoPayeeAccount.person_id != 0)
                {
                    lbusPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
                  //  lbusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                    lbusPayeeAccount.ibusPayee.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id);
                    //lbusPayeeAccount.ibusPayee.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id);
                    lbusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                    lbusPayeeAccount.ibusParticipant.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id);
                 


                }
                //Organization Details
                if (lbusPayeeAccount.icdoPayeeAccount.org_id != 0)
                {
                    lbusPayeeAccount.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                    lbusPayeeAccount.ibusOrganization.FindOrganization(lbusPayeeAccount.icdoPayeeAccount.org_id);
                }

                //TransferOrg Details
                if (lbusPayeeAccount.icdoPayeeAccount.transfer_org_id != 0)
                {
                    busOrganization lbusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                    if (lbusOrganization.FindOrganization(lbusPayeeAccount.icdoPayeeAccount.transfer_org_id))
                    {
                        lbusPayeeAccount.icdoPayeeAccount.istrOrgMPID = lbusOrganization.icdoOrganization.mpi_org_id;
                        lbusPayeeAccount.icdoPayeeAccount.istrOrgName = lbusOrganization.icdoOrganization.org_name;
                    }
                }
               
                lbusPayeeAccount.istrModifiedBy = iobjPassInfo.istrUserID;
               

                ArrayList larrErrors = new ArrayList();
              //  lsrvPayeeAccount.iobjPassInfo.idictParams["ID"] = "OneTimePensionPaymentCorrespondenceBatch";
              // utlPassInfo.iobjPassInfo = autlPassInfo;

                this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;
                aarrResult.Add(lbusPayeeAccount);
                this.CreateCorrespondence(busConstant.ONETIME_PENSION_PAYMENT_LETTER, autlPassInfo.istrUserID, autlPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, ablnIsPDF: true);
                aarrResult.Clear();

              //  autlPassInfo.Commit();


            }

            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    string lstrMsg = "Error while Executing Batch (Payee Account ID : " + Convert.ToInt32(acdoPayeeAccount[enmPayeeAccount.payee_account_id.ToString()]) + " , Error Message: " + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
               // autlPassInfo.Rollback();
            }
        }
        #endregion

        #region Merge PDF into single PDF file //PROD PIR 814
        private void MergePdfsFilesFromPath(string astrGeneratedFilePath, string astrOutFilePath = "", bool ablnIsBISBatch = false, bool ablnIsMDBatch = false, bool ablnDoNotDelete = false)
        {
            MergeOneTimePDFs(astrGeneratedFilePath, "DMST", astrOutFilePath, ablnIsBISBatch, ablnIsMDBatch, ablnDoNotDelete); //PROD PIR 845-(second last parameter added For BIS batch)
            MergeOneTimePDFs(astrGeneratedFilePath, "INTR", astrOutFilePath, ablnIsBISBatch, ablnIsMDBatch, ablnDoNotDelete); //PROD PIR 845-(last parameter added For MD batch)
        }
        void MergeOneTimePDFs(string generatedpath, string astrpostfix, string astrOutFilePath = "", bool ablnIsBISBatch = false, bool ablnIsMDBatch = false, bool ablnDoNotDelete = false)//PROD PIR 845 -- RASHMI(second last parameter added For BIS batch and last parameter added for MD batch)
        {
            DirectoryInfo dir = new DirectoryInfo(generatedpath);
            List<string> filesPath = new List<string>();
            foreach (FileInfo fi in dir.GetFiles("*.*").OrderBy(item => item.CreationTime))
            {
                if (fi.CreationTime > ibusJobHeader.icdoJobHeader.start_time)
                    filesPath.Add(fi.FullName);
            }
            //Get All Files
            List<string> AllPostFixs = (from obj in filesPath
                                        where obj.Contains(astrpostfix)
                                        select obj).ToList();

            if (AllPostFixs != null && AllPostFixs.Count() > 0)
            {
                AllPostFixs = AllPostFixs.Where(obj => obj.Contains(".pdf")).ToList();
                AllPostFixs = AllPostFixs.Select(o => o.Replace(".pdf", "")).Distinct().ToList();

                List<PdfReader> readerList = new List<PdfReader>();
                string outPutFilePath = string.Empty;
                string outPutFilePathVested = string.Empty;
                string outPutFilePathNonVested = string.Empty;
                string lstrVested = string.Empty;
                              
                string outPutFilePathMD = string.Empty;

               
                List<string> SelectedfilesPath = new List<string>();
                SelectedfilesPath = filesPath.Where(obj => obj.EndsWith(astrpostfix + ".pdf")).ToList();

                if (!ablnIsBISBatch)
                {
                    if (!ablnIsMDBatch)
                    {
                        foreach (string filePath in SelectedfilesPath)
                        {
                            PdfReader pdfReader = new PdfReader(filePath);
                            readerList.Add(pdfReader);
                        }
                        //Define a new output document and its size, type
                        Document document = null;// new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //Create blank output pdf file and get the stream to write on it.
                        PdfWriter writer = null;// PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create));
                      //  document.Open();
                        Console.WriteLine("Merging Files");
                        int iintPrintingCount = 0;
                        foreach (PdfReader reader in readerList)
                        {
                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                if (iintPrintingCount == 0)
                                {
                                    //Add datetime to output file path
                                    if (istrMergeFilePrefix.IsNotNullOrEmpty())
                                        outPutFilePath = astrOutFilePath + "\\" + istrMergeFilePrefix + "-" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                                    else
                                        outPutFilePath = astrOutFilePath + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
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
                   
                }
               
                // Delete file if not required
                if (!ablnDoNotDelete)
                {
                    foreach (string filePath in SelectedfilesPath)
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }
        #endregion

    }
}
