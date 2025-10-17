using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;
using MPIPHP.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHPJobService;
using Sagitec.CustomDataObjects;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.Common;
using System.Threading.Tasks;
using Sagitec.ExceptionPub;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace MPIPHPJobService
{
    public class busIAPPaymentNotificationProcess : busBatchHandler
    {
        #region Properties

        private object iobjLock = null;
        //PIR 1002
        public Collection<busRetirementApplication> iclbRetirementApplication { get; set; }

        private DateTime idtRetirementDateFrom { get; set; }
        private DateTime idtRetirementDateTo { get; set; }

        #endregion

        #region RETIREMENT_AFFIDAVIT_BATCH
        public void RetirementAffidavitBatch()
        {
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            busBase lobjBase = new busBase();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "RetirementAffidavitBatch";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();

            //DateTime lFirstDayOfNextMonth;         
            //lFirstDayOfNextMonth = DateTime.Now.GetLastDayofMonth().AddDays(1);

            RetrieveBatchParameters();
            //PIR 1002
            DataTable ldtBenefitApplicationData = busBase.Select("cdoPayeeAccount.GetPayeeAccountsForRetirementAffidavitBatch", new object[2] { idtRetirementDateFrom, idtRetirementDateTo });

            if (ldtBenefitApplicationData.Rows.Count > 0)
            {
                iclbRetirementApplication = lobjBase.GetCollection<busRetirementApplication>(ldtBenefitApplicationData, "icdoBenefitApplication");
                iclbRetirementApplication.ForEach(t => t.istrRetirementDate = Convert.ToString(t.icdoBenefitApplication.retirement_date));

                foreach(busRetirementApplication lbusRetirementApplication in iclbRetirementApplication)
                {

                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "RetirementAffidavitBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    ArrayList aarrResult = new ArrayList();
                    Hashtable ahtbQueryBkmarks = new Hashtable();
                    //PIR 1002
                    try
                    {                        
                        lbusRetirementApplication.RetirementAffidavitCoverLetter();
                        aarrResult.Add(lbusRetirementApplication);
                        this.CreateCorrespondence(busConstant.RETIREMENT_AFFIDAVIT_COVER_LETTER, this.iobjPassInfo.istrUserID,
                            this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, ablnIsPDF: true, astrActiveAddr: lbusRetirementApplication.icdoBenefitApplication.VALID_ADDR_FLAG);
                        this.CreateCorrespondence(busConstant.RETIREMENT_AFFIDAVIT, this.iobjPassInfo.istrUserID,
                           this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, ablnIsPDF: true, astrActiveAddr: lbusRetirementApplication.icdoBenefitApplication.VALID_ADDR_FLAG);                     
                    }
                    catch (Exception e)
                    {
                        lock (iobjLock)
                        {
                            ExceptionManager.Publish(e);
                            String lstrMsg = "Error Occured for MPI_Person Id" + lbusRetirementApplication.ibusParticipant.icdoPerson.mpi_person_id;
                            PostErrorMessage(lstrMsg);
                        }
                    }
                    finally
                    {
                        if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                        {
                            lobjPassInfo.iconFramework.Close();
                        }

                        lobjPassInfo.iconFramework.Dispose();
                        lobjPassInfo.iconFramework = null;
                    }


                };
            }
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;

            if (ldtBenefitApplicationData.IsNotNull() && ldtBenefitApplicationData.Rows.Count > 0)
            {
                try
                {
                    ldtBenefitApplicationData.TableName = "ReportTable01";
                    CreatePDFReport(ldtBenefitApplicationData, "rpt24_AffidavitReport");

                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    String lstrMsg = "Report Generation Failed" + e;
                    PostErrorMessage(lstrMsg);
                }
            }

            MergePDFs(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_Affidavit_Temp);
        }


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
                            case busConstant.JobParamRetirementDateFrom:
                                idtRetirementDateFrom = Convert.ToDateTime(lobjParam.icdoJobParameters.param_value);
                                break;
                            case busConstant.JobParamRetirementDateTo:
                                idtRetirementDateTo = Convert.ToDateTime(lobjParam.icdoJobParameters.param_value);
                                break;
                        }
                    }
                }
            }
        }

        private void MergePDFs(string generatedpath)
        {

            DirectoryInfo dir = new DirectoryInfo(generatedpath);
            List<string> filesPath = new List<string>();
            foreach (FileInfo fi in dir.GetFiles("*.*").Where(t => t.CreationTime.Date == DateTime.Today.Date &&
                (t.FullName.Contains(busConstant.RETIREMENT_AFFIDAVIT) || t.FullName.Contains(busConstant.RETIREMENT_AFFIDAVIT_COVER_LETTER))).OrderBy(item => item.CreationTime))
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
                Document document = null;
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
                            string outPutFilePath = iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_Affidavit;
                            if (!Directory.Exists(outPutFilePath))
                            {
                                Directory.CreateDirectory(outPutFilePath);
                            }
                            outPutFilePath = outPutFilePath + "RetirementAffidavitBatchLetters-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
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
        #endregion
    }
}
