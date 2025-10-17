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
    public class busRetiremenWorkshop : busBatchHandler
    {
        #region Properties

       
        public Collection<string> lclbMpiPersonId { get; set; }

        public Collection<busPerson> lclbPerson { get; set; }
                     
        decimal idecAge { get; set; }
        public busMainBase ibusBaseActivityInstance { get; set; }
        private object iobjLockValidAddress = null;

        private object iobjLockBadAddress = null;

        private int iintRecordCount = 0;
        private int iintTotalCount = 0;

        private int iintTotalCountCheckGC = 0;

        int iintTempTable { get; set; }
        #endregion

        public void CreateRetirmentWorkshopCorrespondence()
        {
            
            Collection<busPerson> lclbRetirementValidAddressCorrespondence;
          
            lclbMpiPersonId = new Collection<string>();

            iobjLockValidAddress = new object();
            string lbatchUserId = this.ibusJobHeader.BatchUserID;
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            iobjPassInfo.idictParams["ID"] = "RetirementWorkshopBatch";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            DataTable ldtblParticipantsRetirementBenefits = new DataTable();
            string lstRetirementWrkshpListReportPath = "";
            lstRetirementWrkshpListReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.INPUT_MPI_RETIREMENT_WORKSHOP) + busConstant.INPUT_MPI_RETIREMENT_WORKSHOP_FILE + ".xlsx";
            busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
            if (File.Exists(lstRetirementWrkshpListReportPath))
            {
                lbusExcelReportGenerator.ReadExcelReport(lstRetirementWrkshpListReportPath, "ReportTable01", lclbMpiPersonId);
            }
            lclbRetirementValidAddressCorrespondence = new Collection<busPerson>();
            foreach (var mpiPersonId in lclbMpiPersonId)
            {
                ldtblParticipantsRetirementBenefits = busBase.Select("cdoPerson.GetParticipantValidAddressforRetirementWorkshop", new object[1] { mpiPersonId.ToString() });

                if (ldtblParticipantsRetirementBenefits != null && ldtblParticipantsRetirementBenefits.Rows.Count > 0)
                {
                    busBase lobjBase = new busBase();
                    lclbPerson = new Collection<busPerson>();
                    busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lclbPerson = lobjBase.GetCollection<busPerson>(ldtblParticipantsRetirementBenefits, "icdoPerson");
                    if (lclbPerson.Where(i => i.icdoPerson.ValidAddress == "Y").Count() > 0)
                    {
                        lbusPerson = lclbPerson.Where(i => i.icdoPerson.ValidAddress == "Y").FirstOrDefault();

                        lclbRetirementValidAddressCorrespondence.Add(lbusPerson);

                    }
                   
                }
            }
            ldtblParticipantsRetirementBenefits.Dispose();

            ParallelOptions lpoParallelOptions = new ParallelOptions();
            lpoParallelOptions.MaxDegreeOfParallelism = 1;// System.Environment.ProcessorCount * 1;
            // Process Valid Addres Correspondence
            if (lclbRetirementValidAddressCorrespondence != null && lclbRetirementValidAddressCorrespondence.Count > 0)
            {
                Parallel.ForEach(lclbRetirementValidAddressCorrespondence.AsEnumerable(), lpoParallelOptions, (lbusRetirementValidAddressCorrespondence, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "RetirementWorkshopBatch";
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

                            if (iintTotalCountCheckGC == 3000)
                            {
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                                iintTotalCountCheckGC = 0;

                            }

                        } lock (iobjLockValidAddress)
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

                            if (iintTotalCountCheckGC == 3000)
                            {
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                                iintTotalCountCheckGC = 0;

                            }

                        }
                        CreateRetirementEstimateCalculation(lbusRetirementValidAddressCorrespondence, "RTMT", 2, lbatchUserId);

                    }
                    catch (Exception e)
                    {
                        lock (iobjLockValidAddress)
                        {
                            ExceptionManager.Publish(e);
                            String lstrMsg = "Error while Executing Batch,Error Message For MPID " + Convert.ToString(lbusRetirementValidAddressCorrespondence.icdoPerson.mpi_person_id.ToString().ToUpper()) + ":" + e.ToString();
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

        }
        #region Merged PDF
        private void MergePdfsFromPath()
        {
            string lstrFilepath = iobjSystemManagement.icdoSystemManagement.base_directory + "Reports\\" + "MPI_Retirement_WorkShop\\" + DateTime.Now.Year;
            
            if (!System.IO.Directory.Exists(lstrFilepath))
                System.IO.Directory.CreateDirectory(lstrFilepath);
            System.IO.Directory.CreateDirectory(lstrFilepath + "\\DMST");
            System.IO.Directory.CreateDirectory(lstrFilepath + "\\INTR");
          
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
            if (filesPath.Where(obj => obj.Contains(".pdf") && obj.Contains("RETR-0071")).Any())
            {
                List<string> SelectedfilesPath = new List<string>();
                SelectedfilesPath = filesPath.Where(obj => obj.Contains(".pdf") && obj.Contains("RETR-0071")).ToList();


                if (SelectedfilesPath != null && SelectedfilesPath.Count > 0 && SelectedfilesPath.Where(obj => obj.Contains("INTR")).Any())
                {
                    CategoricalMergePdf(SelectedfilesPath.Where(obj => obj.Contains("INTR")).ToList(), generatedpath, busConstant.MPI_Retirement_Workshop, "INTR");


                }
                if (SelectedfilesPath != null && SelectedfilesPath.Count > 0 && SelectedfilesPath.Where(obj => obj.Contains("DMST")).Any())
                {
                    CategoricalMergePdf(SelectedfilesPath.Where(obj => obj.Contains("DMST")).ToList(), generatedpath, busConstant.MPI_Retirement_Workshop, "DMST");


                }
            }
        }

        public void CategoricalMergePdf(List<string> SelectedfilesPath, string generatedpath, string fileName, string location)
        {
            List<PdfReader> readerList = new List<PdfReader>();
            foreach (string filePath in SelectedfilesPath)
            {
                PdfReader pdfReader = new PdfReader(filePath);
                readerList.Add(pdfReader);
            }
            string lstrfilepath = string.Empty;

            if (location == "INTR")
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
            if (fileName == busConstant.MPI_Retirement_Workshop && location == "DMST")
            {
                outPutFilePath = generatedpath + "MPI_RetirementWorkshop_DMST" + "_";
            }
            else if (fileName == busConstant.MPI_Retirement_Workshop && location == "INTR")
            {
                outPutFilePath = generatedpath + "MPI_RetirementWorkshop_DMST" + "_";
            }


            return outPutFilePath;
        }
        #endregion
        public void CreateRetirementEstimateCalculation(busPerson lbusMpiRetiremenWrkShp, string astr_benefit_type, int aint_plan_id, string batch_userId)
        {
            int lintBeneficiaryPersonId = 0;
            ArrayList aarrResult = new ArrayList();
            Hashtable ahtbQueryBkmarks = new Hashtable();
            string lstrActiveAddr = busConstant.Flag_Yes;
            this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;

            busBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            lbusBenefitCalculationRetirement.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationRetirement.ibusPerson.FindPerson(lbusMpiRetiremenWrkShp.icdoPerson.mpi_person_id.ToString());
            lbusBenefitCalculationRetirement.ibusPerson.LoadPersonAccounts();
            busCalculation lbusCalculation = new busCalculation();
            int lintAccountId = 0;

            decimal ldecNormalRetirementAge = lbusCalculation.GetNormalRetirementAge(aint_plan_id);
            DateTime ldtNormalRetirementDate = lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(ldecNormalRetirementAge));

          
            if (lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount
                         .Where(t => t.icdoPersonAccount.plan_id == aint_plan_id).Count() > 0)
                lintAccountId = lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount
                    .Where(t => t.icdoPersonAccount.plan_id == aint_plan_id).FirstOrDefault().icdoPersonAccount.person_account_id;

            DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });

            if (ldtbPersonAccountEligibility != null && ldtbPersonAccountEligibility.Rows.Count > 0 &&
                      Convert.ToString(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]).IsNotNullOrEmpty())
            {
                if (Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]) > ldtNormalRetirementDate)
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate = Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0][enmPersonAccountEligibility.vested_date.ToString().ToUpper()]);
                else
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate;

                if (ldtNormalRetirementDate.Day != 1)
                {
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.idtNormalRetirementDate = ldtNormalRetirementDate.AddMonths(1).GetFirstDayofMonth();
                }
            }

            lbusBenefitCalculationRetirement.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            // Initial Setup for Checking Eligbility
            lbusBenefitCalculationRetirement.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationRetirement.ibusPerson;
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount;

            DataTable ldtbGetRTMApplication = busBase.Select("cdoBenefitApplication.GetLatestRetirementApplication", new object[1] { lbusMpiRetiremenWrkShp.icdoPerson.mpi_person_id.ToString() });
            if (ldtbGetRTMApplication != null && ldtbGetRTMApplication.Rows.Count > 0)
            {
                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date = Convert.ToDateTime(ldtbGetRTMApplication.Rows[0]["RETIREMENT_DATE"]);
                lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date;
            }
            else
            {
                lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            }

            lbusBenefitCalculationRetirement.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)

            #region Load Spouse Detail
            busRelationship lbusRelationship = new busRelationship();
            busPerson lbusParticipantBeneficiary = new busPerson();
            lbusParticipantBeneficiary = lbusRelationship.LoadExistingSpouseDetails(lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.person_id);

            if (lbusParticipantBeneficiary != null && lbusParticipantBeneficiary.icdoPerson.person_id > 0)
            {
                lintBeneficiaryPersonId = lbusParticipantBeneficiary.icdoPerson.person_id;
            }
           

            #endregion


            lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();

            lbusBenefitCalculationRetirement.PopulateInitialDataBenefitCalculationHeader(lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.person_id,
                                                                                        busConstant.ZERO_INT, lintBeneficiaryPersonId, astr_benefit_type, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                                                                                        DateTime.MinValue, busConstant.ZERO_DECIMAL, aint_plan_id);
            lbusBenefitCalculationRetirement.EvaluateInitialLoadRules();

            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.created_by = batch_userId;
            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.created_date = DateTime.Now;
            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.modified_by = batch_userId;
            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.modified_date = DateTime.Now;
            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.Insert();
            lbusBenefitCalculationRetirement.BeforeValidate(utlPageMode.New);
            lbusBenefitCalculationRetirement.BeforePersistChanges();
            lbusBenefitCalculationRetirement.AfterPersistChanges();
            aarrResult.Add(lbusBenefitCalculationRetirement);

            if (lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.benefit_calculation_header_id > 0)
            {
                this.CreateCorrespondence(busConstant.MPI_Retirement_Workshop, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, true, lstrActiveAddr);

            }



        }
    }

}



