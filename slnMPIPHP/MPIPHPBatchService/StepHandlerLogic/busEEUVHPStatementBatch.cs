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

namespace MPIPHPJobService
{
    public class busEEUVHPStatementBatch : busBatchHandler
    {
        #region Properties

        //public Collection<cdoDummyWorkData> iclbHealthWorkHistory { get; set; }
        public Collection<busPerson> lclbPerson { get; set; }

        Collection<busBenefitCalculationWithdrawal> lclbEEUVHPBatchCollection { get; set; }

        public DataTable rptUVHPandEERefundList { get; set; }
        decimal idecAge { get; set; }
        public busMainBase ibusBaseActivityInstance { get; set; }
        private object iobjLockValidAddress = null;

        private object iobjLockBadAddress = null;



        private int iintRecordCount = 0;
        private int iintTotalCount = 0;

        private int iintTotalCountCheckGC = 0;

        int iintTempTable { get; set; }
        #endregion
                
        public void LoadEEUVHPBenefitandInterestAmounts()
        {
           createTableDesignFortUVHPandEERefundList();

            Collection<busPerson> lclbEEUVHPValidAddressCorrespondence;
            Collection<busPerson> lclbEEUVHPBadAddressCorrespondence;

            iobjLockValidAddress = new object();
            string lbatchUserId = this.ibusJobHeader.BatchUserID;
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            iobjPassInfo.idictParams["ID"] = "EEUVHPStatementBatch";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            DataTable ldtblParticipantsEEUVHPBenefitAmounts = busBase.Select("cdoPerson.LoadParticipantsEEUVHPBenefitAmounts", new object[0]);

            if (ldtblParticipantsEEUVHPBenefitAmounts != null && ldtblParticipantsEEUVHPBenefitAmounts.Rows.Count > 0)
            {
                busBase lobjBase = new busBase();
                lclbPerson = new Collection<busPerson>();
                lclbPerson = lobjBase.GetCollection<busPerson>(ldtblParticipantsEEUVHPBenefitAmounts, "icdoPerson");

                lclbEEUVHPValidAddressCorrespondence = (from item in lclbPerson
                                                        where item.icdoPerson.ValidAddress == "Y"
                                                        select item).ToList().ToCollection<busPerson>();

                lclbEEUVHPBadAddressCorrespondence = (from item in lclbPerson
                                                      where item.icdoPerson.ValidAddress == "N"
                                                      select item).ToList().ToCollection<busPerson>();

                ParallelOptions lpoParallelOptions = new ParallelOptions();
                lpoParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 2;
                // Process Valid Addres Correspondence
                if (lclbEEUVHPValidAddressCorrespondence != null && lclbEEUVHPValidAddressCorrespondence.Count > 0)
                {
                    Parallel.ForEach(lclbEEUVHPValidAddressCorrespondence.AsEnumerable(), lpoParallelOptions, (lbusEEUVHPValidAddressCorrespondence, loopState) =>
                    {
                        utlPassInfo lobjPassInfo = new utlPassInfo();
                        lobjPassInfo.idictParams = ldictParams;
                        lobjPassInfo.idictParams["ID"] = "EEUVHPStatementBatch";
                        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                        utlPassInfo.iobjPassInfo = lobjPassInfo;
                        try
                        {

                            lock (iobjLockValidAddress)
                            {
                                iintTotalCountCheckGC++;
                                iintRecordCount++;
                                iintTotalCount++;
                                if (iintRecordCount == 100)
                                {
                                    String lstrMsg = iintTotalCount + " : " + " Records Has Been Processed.";
                                    PostInfoMessage(lstrMsg);
                                    iintRecordCount = 0;
                                }

                                if(iintTotalCountCheckGC == 3000)
                                {
                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                    iintTotalCountCheckGC = 0;

                                }

                            }

                            CreateWithdrawalEstimateCalculation(lbusEEUVHPValidAddressCorrespondence, "WDRL", 2, lbatchUserId);

                        }
                        catch (Exception e)
                        {
                            lock (iobjLockValidAddress)
                            {
                                ExceptionManager.Publish(e);
                                String lstrMsg = "Error while Executing Batch,Error Message For MPID " + Convert.ToString(lbusEEUVHPValidAddressCorrespondence.icdoPerson.mpi_person_id.ToString().ToUpper()) + ":" + e.ToString();
                                PostErrorMessage(lstrMsg);
                            }
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
                    MergePdfsFromPath();
                    lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjMainPassInfo;
                }
                // Process Bad Addres Correspondence
                if (lclbEEUVHPBadAddressCorrespondence != null && lclbEEUVHPBadAddressCorrespondence.Count > 0)
                {
                    iobjLockBadAddress = new object();
                    lpoParallelOptions = new ParallelOptions();
                    lpoParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 2;

                    Parallel.ForEach(lclbEEUVHPBadAddressCorrespondence.AsEnumerable(), lpoParallelOptions, (lbusEEUVHPBadAddressCorrespondence, loopState) =>
                    {
                        utlPassInfo lobjPassInfo = new utlPassInfo();
                        lobjPassInfo.idictParams = ldictParams;
                        lobjPassInfo.idictParams["ID"] = "EEUVHPStatementBatch";
                        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                        utlPassInfo.iobjPassInfo = lobjPassInfo;
                        try
                        {

                            lock (iobjLockBadAddress)
                            {
                                iintRecordCount++;
                                iintTotalCount++;
                                if (iintRecordCount == 100)
                                {
                                    String lstrMsg = iintTotalCount + " : " + " Records Has Been Processed.";
                                    PostInfoMessage(lstrMsg);
                                    iintRecordCount = 0;
                                }

                                if (iintTotalCountCheckGC == 3000)
                                {
                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                    iintTotalCountCheckGC = 0;

                                }
                            }

                            CreateWithdrawalEstimateCalculation(lbusEEUVHPBadAddressCorrespondence, "WDRL", 2, lbatchUserId);

                        }
                        catch (Exception e)
                        {
                            lock (iobjLockBadAddress)
                            {
                                ExceptionManager.Publish(e);
                                String lstrMsg = "Error while Executing Batch,Error Message For MPID " + Convert.ToString(lbusEEUVHPBadAddressCorrespondence.icdoPerson.mpi_person_id.ToString().ToUpper()) + ":" + e.ToString();
                                PostErrorMessage(lstrMsg);
                            }
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

                   
                }
           }
            ldtblParticipantsEEUVHPBenefitAmounts.Dispose();
            utlPassInfo lobjPassInfo1 = new utlPassInfo();
            lobjPassInfo1.idictParams = ldictParams;
            lobjPassInfo1.idictParams["ID"] = "EEUVHPStatementBatch";
            lobjPassInfo1.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjPassInfo1;
            DataTable ldtblParticipantsEEUVHPBenefitAmountsReport = busBase.Select("cdoPerson.LoadParticipantsEEUVHPBenefitAmountsReport", new object[1] {lbatchUserId.ToString()});

            if (ldtblParticipantsEEUVHPBenefitAmountsReport != null && ldtblParticipantsEEUVHPBenefitAmountsReport.Rows.Count > 0)
            {
                DataSet idtstUVHPandEERefundList = new DataSet();
                idtstUVHPandEERefundList.Tables.Add(ldtblParticipantsEEUVHPBenefitAmountsReport.Copy());
                idtstUVHPandEERefundList.Tables[0].TableName = "ReportTable01";
                idtstUVHPandEERefundList.DataSetName = "ReportTable01";

                string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_UVHP_EE_REFUND + ".xlsx";
                string lstrUVHPEERefundListReportPath = "";
                lstrUVHPEERefundListReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_UVHP_EE_REFUND_REPORT_PATH) + busConstant.REPORT_UVHP_EE_REFUND + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
                busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrUVHPEERefundListReportPath, "ReportTable01", idtstUVHPandEERefundList);

            }

        }
        #region Merged PDF
        // pir - 783 Merge pdf
        private void MergePdfsFromPath()
        {
            string lstrFilepath = iobjSystemManagement.icdoSystemManagement.base_directory + "Reports\\" + "UVHP&EE\\" + DateTime.Now.Year;
            //Added : Shankar Dated : 01-07-2016 (Create New Dir. If Does not Exists ) Retiree Increase Batch DIR Not Exists
            if (!System.IO.Directory.Exists(lstrFilepath))
                System.IO.Directory.CreateDirectory(lstrFilepath);
            //End 
            MergePDFs(lstrFilepath);
        }

        private void MergePDFs(string generatedpath)
        {
            DirectoryInfo dir = new DirectoryInfo(generatedpath);
            List<string> filesPath = new List<string>();
            foreach (FileInfo fi in dir.GetFiles("*.*").OrderBy(item => item.CreationTime))
            {
                if (fi.CreationTime > ibusJobHeader.icdoJobHeader.start_time)
                    filesPath.Add(fi.FullName);
            }

            // Get selected file path contains pdf files
            if (filesPath.Where(obj => obj.Contains(".pdf") && obj.Contains("WIDRWL-0020")).Any())
            {
                List<string> SelectedfilesPath = new List<string>();
                SelectedfilesPath = filesPath.Where(obj => obj.Contains(".pdf") && obj.Contains("WIDRWL-0020")).ToList();


                if (SelectedfilesPath != null && SelectedfilesPath.Count > 0 && SelectedfilesPath.Where(obj => obj.Contains("INTR")).Any())
                {
                    CategoricalMergePdf(SelectedfilesPath.Where(obj => obj.Contains("INTR")).ToList(), generatedpath, "WIDRWL-0020", "INTR");


                }
                if (SelectedfilesPath != null && SelectedfilesPath.Count > 0 && SelectedfilesPath.Where(obj => obj.Contains("DMST")).Any())
                {
                    CategoricalMergePdf(SelectedfilesPath.Where(obj => obj.Contains("DMST")).ToList(), generatedpath, "WIDRWL-0020", "DMST");


                }
            }

            if (filesPath.Where(obj => obj.Contains(".pdf") && obj.Contains("WIDRWL-0021")).Any())
            {
                List<string> SelectedfilesPath = new List<string>();
                SelectedfilesPath = filesPath.Where(obj => obj.Contains(".pdf") && obj.Contains("WIDRWL-0021")).ToList();
                if (SelectedfilesPath != null && SelectedfilesPath.Count > 0 && SelectedfilesPath.Where(obj => obj.Contains("INTR")).Any())
                {
                  CategoricalMergePdf(SelectedfilesPath.Where(obj => obj.Contains("INTR")).ToList(), generatedpath, "WIDRWL-0021", "INTR");
                    
                }
                if (SelectedfilesPath != null && SelectedfilesPath.Count > 0 && SelectedfilesPath.Where(obj => obj.Contains("DMST")).Any())
                {
                     CategoricalMergePdf(SelectedfilesPath.Where(obj => obj.Contains("DMST")).ToList(), generatedpath, "WIDRWL-0021", "DMST");
                }
            }
            if (filesPath.Where(obj => obj.Contains(".pdf") && obj.Contains("WIDRWL-0022")).Any())
            {
                List<string> SelectedfilesPath = new List<string>();
                SelectedfilesPath = filesPath.Where(obj => obj.Contains(".pdf") && obj.Contains("WIDRWL-0022")).ToList();
                if (SelectedfilesPath != null && SelectedfilesPath.Count > 0 && SelectedfilesPath.Where(obj => obj.Contains("INTR")).Any())
                {
                   CategoricalMergePdf(SelectedfilesPath.Where(obj => obj.Contains("INTR")).ToList(), generatedpath, "WIDRWL-0022", "INTR");
                }
                if (SelectedfilesPath != null && SelectedfilesPath.Count > 0 && SelectedfilesPath.Where(obj => obj.Contains("DMST")).Any())
                {
                    CategoricalMergePdf(SelectedfilesPath.Where(obj => obj.Contains("DMST")).ToList(), generatedpath, "WIDRWL-0022", "DMST");
                    
                }
            }

            // validate SelectedfilesPath is null or empty

        }

        public void CategoricalMergePdf(List<string> SelectedfilesPath, string generatedpath,string fileName,string location)
        {
            List<PdfReader> readerList = new List<PdfReader>();
            foreach (string filePath in SelectedfilesPath)
            {
                PdfReader pdfReader = new PdfReader(filePath);
                readerList.Add(pdfReader);
            }
            string lstrfilepath = string.Empty;

           if(location == "INTR")
            {
                lstrfilepath = generatedpath + "\\INTR\\";

            }
            else
            {
                lstrfilepath = generatedpath + "\\DMST\\";

            }
             
            if (!Directory.Exists(lstrfilepath))
            {
                Directory.CreateDirectory(lstrfilepath);
            }

            //if( filesPath.Where(obj => obj.Contains("WIDRWL-0021"))
            // {

            // }
            //PIR 977
            string istrOutPutFilePath = GetMergePdfOutputFilePath(lstrfilepath, fileName, location);

            Console.WriteLine("Merging Files");
            Document document = null;
            PdfWriter writer = null;
            int iintPrintingCount = 0;
            foreach (PdfReader reader in readerList)
            {
               // reader.NumberOfPages = 4935;
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    // validating to generate 5000 pages in one pdf file
                    if (iintPrintingCount == 0)
                    {
                        //Add datetime to output file path
                        string outPutFilePath = "";
                        outPutFilePath = istrOutPutFilePath + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                        //Define a new output document and its size, type
                        document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //Create blank output pdf file and get the stream to write on it.                            
                        writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create));
                        document.Open();
                    }

                    PdfImportedPage page = writer.GetImportedPage(reader, i);
                    document.Add(iTextSharp.text.Image.GetInstance(page));

                    iintPrintingCount++;
                    if (iintPrintingCount == 4935)
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

        private string GetMergePdfOutputFilePath(string generatedpath, string fileName, string location)
        {
            string outPutFilePath = "";
            if (fileName == "WIDRWL-0020" && location == "DMST")
            {
                outPutFilePath = generatedpath + "Withdrawal_EE_UVHP_REFUND_LETTER_A" + "_";
            }
            else if (fileName == "WIDRWL-0020" && location == "INTR")
            {
                outPutFilePath = generatedpath + "Withdrawal_EE_UVHP_REFUND_LETTER_A_Foreign" + "_";
            }
            else if (fileName == "WIDRWL-0021" && location == "DMST")
            {
                outPutFilePath = generatedpath + "Withdrawal_EE_UVHP_REFUND_LETTER_B" + "_";
            }
            else if (fileName == "WIDRWL-0021" && location == "INTR")
            {
                outPutFilePath = generatedpath + "Withdrawal_EE_UVHP_REFUND_LETTER_B_Foreign" + "_";

            }
            else if (fileName == "WIDRWL-0022" && location == "DMST")
            {
                outPutFilePath = generatedpath + "Withdrawal_EE_UVHP_REFUND_LETTER_C" + "_";
            }
            else if (fileName == "WIDRWL-0022" && location == "INTR")
            {
                outPutFilePath = generatedpath + "Withdrawal_EE_UVHP_REFUND_LETTER_C_Foreign" + "_";
            }
            return outPutFilePath;
        }
        #endregion
        public void CreateWithdrawalEstimateCalculation(busPerson lbusEEUVHPWithdrawalCalculation, string astr_benefit_type,int aint_plan_id, string batch_userId)
        {
            ArrayList aarrResult = new ArrayList();
            Hashtable ahtbQueryBkmarks = new Hashtable();
            string lstrActiveAddr = busConstant.Flag_Yes;

                this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;

                busBenefitCalculationWithdrawal lbusBenefitCalculationWithdrawal = new busBenefitCalculationWithdrawal { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            lbusBenefitCalculationWithdrawal.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationWithdrawal.ibusPerson.FindPerson(lbusEEUVHPWithdrawalCalculation.icdoPerson.mpi_person_id.ToString());
            lbusBenefitCalculationWithdrawal.ibusPerson.LoadPersonAddress();
            lbusBenefitCalculationWithdrawal.ibusPerson.LoadPersonAccounts();

            lbusBenefitCalculationWithdrawal.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            // Initial Setup for Checking Eligbility
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationWithdrawal.ibusPerson;
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount;
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            lbusBenefitCalculationWithdrawal.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)


            lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();



            //IN NEW MODE WHATEVER VALUES WE KNOW WE COULD FILL THOSE ATLEAST WHILE COMING TO THE MNTN SCREEN
            lbusBenefitCalculationWithdrawal.PopulateInitialDataBenefitCalculationHeader(lbusBenefitCalculationWithdrawal.ibusPerson.icdoPerson.person_id,
                                                                                        busConstant.ZERO_INT, busConstant.ZERO_INT, astr_benefit_type, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                                                                                       DateTime.MinValue, busConstant.ZERO_DECIMAL, aint_plan_id);
            lbusBenefitCalculationWithdrawal.LoadPersonNotes();
            lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            if (lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.retirement_date != DateTime.MinValue)
            {

                lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.retirement_date;
                lbusBenefitCalculationWithdrawal.ibusBenefitApplication.icdoBenefitApplication.withdrawal_date = lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.retirement_date;
                lbusBenefitCalculationWithdrawal.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationWithdrawal.ibusPerson.icdoPerson.idtDateofBirth, lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.retirement_date);
                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.retirement_date);
                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.age = lbusBenefitCalculationWithdrawal.ibusBenefitApplication.idecAge; //Load the AGE OF THE MAIN HEADER OBJECT AS WELL
                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.idecParticipantFullAge = Math.Floor(lbusBenefitCalculationWithdrawal.ibusBenefitApplication.idecAge);
                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.idecSurvivorFullAge = Math.Floor(lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement);
               

                lbusBenefitCalculationWithdrawal.iblnCalcualteUVHPBenefit = lbusBenefitCalculationWithdrawal.iblnCalculateIAPBenefit = lbusBenefitCalculationWithdrawal.iblnCalculateL161SplAccBenefit = lbusBenefitCalculationWithdrawal.iblnCalculateL52SplAccBenefit = lbusBenefitCalculationWithdrawal.iblnCalculateMPIPPBenefit = true;

                lbusBenefitCalculationWithdrawal.SetupPreRequisites_WithdrawalCalculations();
                if (lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.dro_application_id == 0)
                {
                    if (!lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                    {
                        lbusBenefitCalculationWithdrawal.LoadAllRetirementContributions(lbusBenefitCalculationWithdrawal.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                    }
                    else
                    {
                        lbusBenefitCalculationWithdrawal.LoadAllRetirementContributions(null);
                    }
                }
                else
                {
                    if (!lbusBenefitCalculationWithdrawal.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                    {
                        lbusBenefitCalculationWithdrawal.LoadAllRetirementContributions(lbusBenefitCalculationWithdrawal.ibusQdroApplication.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                    }
                    else
                    {
                        lbusBenefitCalculationWithdrawal.LoadAllRetirementContributions(null);
                    }
                }
            }
           if(lbusBenefitCalculationWithdrawal.ibusBenefitApplication.iclbEligiblePlans.Count() > 0)
            {
                lbusBenefitCalculationWithdrawal.BeforePersistChanges();
                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.created_by = batch_userId;
                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.created_date = DateTime.Now;
                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.modified_by = batch_userId;
                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.modified_date = DateTime.Now;
                lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.Insert();

                lbusBenefitCalculationWithdrawal.AfterPersistChanges();

            }

            if (lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.benefit_calculation_header_id > 0)
            {
                if (lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == 2).Count()>0)
                {

                     var luvhpEEBenefitAmount = lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == 2).Select(y => y.icdoBenefitCalculationDetail.total_uvhp_contribution_amount).FirstOrDefault() +
                                               lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == 2).Select(y => y.icdoBenefitCalculationDetail.non_vested_ee_amount).FirstOrDefault();
                    var luvhpEEInterestAmount = lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == 2).Select(y => y.icdoBenefitCalculationDetail.total_uvhp_interest_amount).FirstOrDefault() +
                                                lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == 2).Select(y => y.icdoBenefitCalculationDetail.non_vested_ee_interest).FirstOrDefault();


                    aarrResult.Add(lbusBenefitCalculationWithdrawal);

                    if(luvhpEEBenefitAmount + luvhpEEInterestAmount > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT)
                    {
                        this.CreateCorrespondence(busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_C, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true, lstrActiveAddr);
                    }
                    else
                    {
                        if (luvhpEEBenefitAmount + luvhpEEInterestAmount <= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && luvhpEEInterestAmount < 200)
                        {
                            
                            this.CreateCorrespondence(busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_A, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true, lstrActiveAddr);
                        }
                        if (luvhpEEBenefitAmount + luvhpEEInterestAmount <= busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && luvhpEEInterestAmount >= 200)
                        {
                            
                            this.CreateCorrespondence(busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_B, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true, lstrActiveAddr);
                        }

                    }

                    

                    //if (luvhpEEBenefitAmount > 5000)
                    //{
                    //    //  lbusBenefitCalculationWithdrawal.LoadCorresProperties(busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_C);
                    //    this.CreateCorrespondence(busConstant.Withdrawal_EE_UVHP_REFUND_LETTER_C, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true, lstrActiveAddr);
                    //}

                    //DataRow dr = rptUVHPandEERefundList.NewRow();

                    //dr["MPID"] = lbusBenefitCalculationWithdrawal.ibusPerson.icdoPerson.mpi_person_id;
                    //dr["DOB"] = lbusBenefitCalculationWithdrawal.ibusPerson.icdoPerson.date_of_birth;
                    //dr["SSN"] = lbusBenefitCalculationWithdrawal.ibusPerson.icdoPerson.ssn.ToString();
                    //dr["FIRSTNAME"] = lbusBenefitCalculationWithdrawal.ibusPerson.icdoPerson.first_name;
                    //dr["MI"] = lbusBenefitCalculationWithdrawal.ibusPerson.icdoPerson.middle_name;
                    //dr["LASTNAME"] = lbusBenefitCalculationWithdrawal.ibusPerson.icdoPerson.last_name;
                    //dr["ValidAddress"] = acdoPerson["ValidAddress"].ToString(); //lbusBenefitCalculationWithdrawal.ibusPerson.ibusPersonAddress.icdoPersonAddress.bad_address_flag;
                    //dr["Address1"] = acdoPerson["ADDR_LINE_1"].ToString();//lbusBenefitCalculationWithdrawal.ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_line_1;
                    //dr["Address2"] = acdoPerson["ADDR_LINE_2"].ToString(); //lbusBenefitCalculationWithdrawal.ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_line_2;
                    //dr["City"] = acdoPerson["ADDR_CITY"].ToString(); //lbusBenefitCalculationWithdrawal.ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_city;
                    //dr["State"] = acdoPerson["ADDR_STATE_VALUE"].ToString(); //lbusBenefitCalculationWithdrawal.ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_state_value;
                    //dr["ZipCode"] = acdoPerson["ADDR_ZIP_CODE"].ToString();//lbusBenefitCalculationWithdrawal.ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_zip_4_code;
                    //dr["CALCID"] = lbusBenefitCalculationWithdrawal.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                    //dr["UVHP"] = lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == 2).Select(y => y.icdoBenefitCalculationDetail.total_uvhp_contribution_amount).FirstOrDefault();
                    //dr["UVHPInt"] = lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == 2).Select(y => y.icdoBenefitCalculationDetail.total_uvhp_interest_amount).FirstOrDefault();
                    //dr["EE"] = lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == 2).Select(y => y.icdoBenefitCalculationDetail.non_vested_ee_amount).FirstOrDefault();
                    //dr["EEInt"] = lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail.Where(i => i.icdoBenefitCalculationDetail.plan_id == 2).Select(y => y.icdoBenefitCalculationDetail.non_vested_ee_interest).FirstOrDefault();
                    //// dr["Qyrs"] = lbusBenefitCalculationWithdrawal.iintTotalQYrs;

                    //rptUVHPandEERefundList.Rows.Add(dr);

                }

            }


        }

        public DataTable createTableDesignFortUVHPandEERefundList()
        {
            rptUVHPandEERefundList = new DataTable();
            rptUVHPandEERefundList.Columns.Add("MPID", typeof(string));
            rptUVHPandEERefundList.Columns.Add("DOB", typeof(DateTime));
            rptUVHPandEERefundList.Columns.Add("SSN", typeof(string));
            rptUVHPandEERefundList.Columns.Add("FIRSTNAME", typeof(string));
            rptUVHPandEERefundList.Columns.Add("MI", typeof(string));
            rptUVHPandEERefundList.Columns.Add("LASTNAME", typeof(string));
            rptUVHPandEERefundList.Columns.Add("ValidAddress", typeof(string));
            rptUVHPandEERefundList.Columns.Add("Address1", typeof(string));
            rptUVHPandEERefundList.Columns.Add("Address2", typeof(string));
            rptUVHPandEERefundList.Columns.Add("City", typeof(string));
            rptUVHPandEERefundList.Columns.Add("State", typeof(string));
            rptUVHPandEERefundList.Columns.Add("ZipCode", typeof(string));
            rptUVHPandEERefundList.Columns.Add("CALCID", typeof(int));
            rptUVHPandEERefundList.Columns.Add("UVHP", typeof(decimal));
            rptUVHPandEERefundList.Columns.Add("UVHPInt", typeof(decimal));
            rptUVHPandEERefundList.Columns.Add("EE", typeof(decimal));
            rptUVHPandEERefundList.Columns.Add("EEInt", typeof(decimal));
            rptUVHPandEERefundList.Columns.Add("Qyrs", typeof(int));
           

            return rptUVHPandEERefundList;
        }
   
    }
   

}
