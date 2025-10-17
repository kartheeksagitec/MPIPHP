using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.ExceptionPub;
using MPIPHP.BusinessObjects;
using System.Threading.Tasks;
using Sagitec.Interface;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.Reporting.WinForms;
 
namespace MPIPHPJobService
{
    public class busGenerateAnnualStatementBatch : busBatchHandler
    {
        #region Properties
        public Collection<busDataExtractionBatchInfo> iclbDataExtractionBatchInfo { get; set; }
        public Collection<busYearEndDataExtractionHeader> iclbYearEndDataExtractionHeader { get; set; }
        public int iintPrintingCounter { get; set; }
        private int iintTempTable { get; set; }
        public bool iblnCorreted { get; set; }

        //ChangeID: 57284
        public string lstrIsMDFlag { get; set; }
        public string lstrIsRetrSpecialAcctFlag { get; set; }

        public string lstrIsReemployedUnder65Flag { get; set; }
        //Ticket##128387 
        public string lstrPensionOnlyFlag { get; set; }

        int lintDomesticCnt = 0, lintInternationalCnt = 0, lintBadCnt = 0;
        #endregion

        public void GenerateAnnualStatements()
        {
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            busBase lobjBase = new busBase();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            int lintCount = 0;
            int lintTotalCount = 0;
            string lstrCorrectedFlag = busConstant.FLAG_NO;
            lstrIsMDFlag = busConstant.FLAG_NO;
            lstrIsRetrSpecialAcctFlag = busConstant.FLAG_NO;
            lstrIsReemployedUnder65Flag = busConstant.FLAG_NO;
            //Ticket##128387 
            lstrPensionOnlyFlag = busConstant.FLAG_NO;

            int lintComputationYear = iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1;
            RetrieveBatchParameters();

            if (iblnCorreted)
                lstrCorrectedFlag = busConstant.FLAG_YES;
            else
                lstrCorrectedFlag = busConstant.FLAG_NO;

            //Annual Statement Report Changes PIR 960
            //#region GET_PENSION_LATE_HRS history
            //utlConnection utlLegacyDBConnection = HelperFunction.GetDBConnectionProperties("Legacy");
            //string astrLegacyDBConnection = utlLegacyDBConnection.istrConnectionString;

            //SqlParameter[] parameters = new SqlParameter[2];
            //SqlParameter param1 = new SqlParameter("@COMPUTATIONYEAR", DbType.Int32);
            //SqlParameter param2 = new SqlParameter("@OPTIONALPARAMETER", DbType.Int32);
            //param1.Value = iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1;
            //parameters[0] = param1;
            //param2.Value = 1;
            //parameters[1] = param2;

            //DataTable ldtbPensionLateHrsInfoForAllParticipants = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetIAPSnapShotInfo", astrLegacyDBConnection, null, parameters);
            //ldtbPensionLateHrsInfoForAllParticipants.Dispose();
            //#endregion

            DataTable ldtbAnnualStatementAllParticipants = new DataTable();
            //Ticket##128387 
            ldtbAnnualStatementAllParticipants = busBase.Select("cdoDataExtractionBatchInfo.GenerateDataForAnnualStatement", new object[8] { lintComputationYear, iintPrintingCounter, iintTempTable, lstrCorrectedFlag, lstrIsRetrSpecialAcctFlag, lstrIsMDFlag, lstrIsReemployedUnder65Flag, lstrPensionOnlyFlag });
            ldtbAnnualStatementAllParticipants.Dispose();

            DataTable ldtbAnnualStatementParticipants = new DataTable();
            //Ticket##128387 
            ldtbAnnualStatementParticipants = busBase.Select("cdoDataExtractionBatchInfo.GetDataForAnnualStatement", new object[7] { lintComputationYear, iintTempTable, lstrCorrectedFlag, lstrIsMDFlag, lstrIsRetrSpecialAcctFlag, lstrIsReemployedUnder65Flag, lstrPensionOnlyFlag });

            if (ldtbAnnualStatementParticipants.Rows.Count > 0)
            {
                ReportViewer rvViewer = new ReportViewer();
                foreach (DataRow dr in ldtbAnnualStatementParticipants.AsEnumerable())
                {
                    int lintPersonID = Convert.ToInt32(dr[enmPerson.person_id.ToString()]);
                    string lstrSSNDecrypted = Convert.ToString(dr["PERSON_SSN"]), lstrPlanCode = string.Empty;
                    int lintCompYear = lintComputationYear; // DateTime.Now.Year - 1;
                    AnnualStatement(lintPersonID, lintCompYear, rvViewer);
                    lintTotalCount++;
                    if (lintTotalCount == 3000)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        lintTotalCount = 0;
                    }
                }

                ldtbAnnualStatementParticipants.Dispose();

                utlPassInfo lobjPassInfo1 = new utlPassInfo();
                lobjPassInfo1.idictParams = ldictParams;
                lobjPassInfo1.idictParams["ID"] = "GenerateAnnualStatementBatch";
                lobjPassInfo1.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo1;
                lobjPassInfo1.BeginTransaction();

                DataTable ldtbYearData = busBase.Select("cdoDataExtractionBatchInfo.GetDataForGivenYearToChangeFlag", new object[1] { lintComputationYear });
                if (ldtbYearData.Rows.Count > 0)
                {
                    busYearEndDataExtractionHeader lbusbusYearEndDataExtractionHeader = new busYearEndDataExtractionHeader { icdoYearEndDataExtractionHeader = new cdoYearEndDataExtractionHeader() };

                    lbusbusYearEndDataExtractionHeader.icdoYearEndDataExtractionHeader.LoadData(ldtbYearData.Rows[0]);
                    lbusbusYearEndDataExtractionHeader.icdoYearEndDataExtractionHeader.is_annual_statement_generated_flag = busConstant.FLAG_YES;
                    lbusbusYearEndDataExtractionHeader.icdoYearEndDataExtractionHeader.Update();
                }
                lobjPassInfo1.Commit();
                if (lobjPassInfo1.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo1.iconFramework.Close();
                }
                lobjPassInfo1.iconFramework.Dispose();
                lobjPassInfo1.iconFramework = null;
            }
            lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
            MergePdfsFromPath();

            {
                string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.STATEMENT_PARTICIPANTS_WITH_STATEMENT_FLAG + ".xlsx";
                string lstrAnnualStatementParticipantDetailReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.STATEMENT_PARTICIPANTS_WITH_STATEMENT_FLAG + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
                DataTable ldtbStatementParticipants = busBase.Select("entPerson.GetStatementParticipantsWithStatementFlag", new object[2] { lintComputationYear, iintTempTable });
                ldtbStatementParticipants.TableName = "ParticipantsWithStatementFlag";
                DataTable adtbStatementParticipants = ldtbStatementParticipants.Copy();
                DataSet ldsStatementParticipantsReportDataForExcel = new DataSet();
                ldsStatementParticipantsReportDataForExcel.Tables.Add(adtbStatementParticipants);
                ldtbStatementParticipants.Dispose();
                busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrAnnualStatementParticipantDetailReportPath, "Participant Details", ldsStatementParticipantsReportDataForExcel);
            }

            //Ticket##128387 
            if ((lstrIsMDFlag == busConstant.FLAG_YES || lstrIsRetrSpecialAcctFlag == busConstant.FLAG_YES || lstrIsReemployedUnder65Flag == busConstant.FLAG_YES || lstrPensionOnlyFlag == busConstant.FLAG_YES) && ldtbAnnualStatementParticipants.Rows.Count > 0)
            {
                string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_ANNUAL_STATEMENT_PARTICIPANT_DETAIL + ".xlsx";
                string lstrAnnualStatementParticipantDetailReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.REPORT_ANNUAL_STATEMENT_PARTICIPANT_DETAIL + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

                DataTable adtbAnnualStatementParticipants = ldtbAnnualStatementParticipants.Copy();
                adtbAnnualStatementParticipants.TableName = "AnnualStatementParticipantsDetails";
                DataSet ldsAnnualStatementParticipantsReportDataForExcel = new DataSet();
                ldsAnnualStatementParticipantsReportDataForExcel.Tables.Add(adtbAnnualStatementParticipants);

                busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrAnnualStatementParticipantDetailReportPath, "Participant Details", ldsAnnualStatementParticipantsReportDataForExcel);
            }
        }
        //Annual Statement Report Changes PIR 960
        public void AnnualStatement(int lintPersonID, int lintCompYear, ReportViewer rvViewer)
        {
            DataTable ldtbAnnualStatementSingleParticipant = busBase.Select("cdoDataExtractionBatchInfo.GenerateAnnualStatementForSinglePerson", new object[2] { lintCompYear, lintPersonID });
            cdoAnnualStatementBatchData lcdoAnnualStatementBatchData = new cdoAnnualStatementBatchData();
            lcdoAnnualStatementBatchData.LoadData(ldtbAnnualStatementSingleParticipant.Rows[0]);
            busPerson lbusPerson = new busPerson();
            string lstrPostfixName = lcdoAnnualStatementBatchData.addr_category_value + Convert.ToString(lcdoAnnualStatementBatchData.batch_id);
            lbusPerson.CreateAnnualStatementReport(rvViewer, lcdoAnnualStatementBatchData, lintCompYear, true, lstrPostfixName, iblnCorreted);//PIR 882
            lbusPerson = null;
            lcdoAnnualStatementBatchData = null;
            ldtbAnnualStatementSingleParticipant.Dispose();
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
                            case "PrintingCounter":
                                iintPrintingCounter = Convert.ToInt32(lobjParam.icdoJobParameters.param_value);
                                break;
                            //Annual Statement Report Changes PIR 960
                            case "RunForSpecificParticipants":
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty()
                                    && Convert.ToString(lobjParam.icdoJobParameters.param_value).Trim() == busConstant.FLAG_YES)
                                {
                                    iintTempTable = 1;
                                }
                                else
                                {
                                    iintTempTable = 0;
                                }
                                break;
                            case "CorrectedStatements":
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty()
                                    && Convert.ToString(lobjParam.icdoJobParameters.param_value).Trim() == busConstant.FLAG_YES)
                                {
                                    iblnCorreted = true;
                                }
                                else
                                {
                                    iblnCorreted = false;
                                }
                                break;

                            case "MDParticipantsOnly":

                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty()
                                    && Convert.ToString(lobjParam.icdoJobParameters.param_value).Trim() == busConstant.FLAG_YES)
                                {
                                    lstrIsMDFlag = Convert.ToString('Y');
                                }
                                else
                                {
                                    lstrIsMDFlag = Convert.ToString('N');
                                }
                                break;

                            case "RetiredSpecialAccountOnly":
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty()
                                    && Convert.ToString(lobjParam.icdoJobParameters.param_value).Trim() == busConstant.FLAG_YES)
                                {
                                    lstrIsRetrSpecialAcctFlag = Convert.ToString('Y');
                                }
                                else
                                {
                                    lstrIsRetrSpecialAcctFlag = Convert.ToString('N');
                                }
                                break;

                            case "ReemployedAgeLessThan65Only":
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty()
                                    && Convert.ToString(lobjParam.icdoJobParameters.param_value).Trim() == busConstant.FLAG_YES)
                                {
                                    lstrIsReemployedUnder65Flag = Convert.ToString('Y');
                                }
                                else
                                {
                                    lstrIsReemployedUnder65Flag = Convert.ToString('N');
                                }
                                break;
                            //Ticket##128387 
                            case "PensionOnlyStatement":
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty()
                                    && Convert.ToString(lobjParam.icdoJobParameters.param_value).Trim() == busConstant.FLAG_YES)
                                {
                                    lstrPensionOnlyFlag = Convert.ToString('Y');
                                }
                                else
                                {
                                    lstrPensionOnlyFlag = Convert.ToString('N');
                                }
                                break;
                                

                        }
                    }
                }
            }
        }

        void MergePdfsFromPath()
        {
            string labsRptGenPath = string.Empty;
            if (iblnCorreted)
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_DOMESTIC_CORRECTED_ADDRESS_PATH);
            else
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_DOMESTIC_ADDRESS_PATH);
            MergePDFs(labsRptGenPath, "DMST");

            if (iblnCorreted)
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_FOREIGN_CORRECTED_ADDRESS_PATH);
            else
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_FOREIGN_ADDRESS_PATH);
            MergePDFs(labsRptGenPath, "INTR");
        }

        void MergePDFs(string generatedpath, string astrpostfix)
        {
            DirectoryInfo dir = new DirectoryInfo(generatedpath);
            List<string> filesPath = new List<string>();
            foreach (FileInfo fi in dir.GetFiles("*.*").OrderBy(item => item.CreationTime))
            {
                if (fi.CreationTime > ibusJobHeader.icdoJobHeader.start_time)
                    filesPath.Add(fi.FullName);
            }
            List<string> AllPostFixs = (from obj in filesPath
                                        where obj.Contains("_")
                                        select new
                                        {
                                            postfix = obj.Substring(obj.LastIndexOf("_") + 1)
                                        }).Select(o => o.postfix).ToList();
            AllPostFixs = AllPostFixs.Where(obj => obj.Contains(astrpostfix)).ToList();
            AllPostFixs = AllPostFixs.Where(obj => obj.Contains(".pdf")).ToList();
            AllPostFixs = AllPostFixs.Select(o => o.Replace(".pdf", "")).Distinct().ToList();

            foreach (string postfix in AllPostFixs)
            {
                List<PdfReader> readerList = new List<PdfReader>();
                string outPutFilePath = generatedpath + postfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";

                List<string> SelectedfilesPath = new List<string>();
                SelectedfilesPath = filesPath.Where(obj => obj.EndsWith(postfix + ".pdf") && obj.Contains("_")).ToList();
                foreach (string filePath in SelectedfilesPath)
                {
                    PdfReader pdfReader = new PdfReader(filePath);
                    readerList.Add(pdfReader);
                }
                //Define a new output document and its size, type
                Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                //Create blank output pdf file and get the stream to write on it.
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create));
                document.Open();
                Console.WriteLine("Merging Files");
                foreach (PdfReader reader in readerList)
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        document.Add(iTextSharp.text.Image.GetInstance(page));
                    }
                }
                Console.WriteLine("Closing Files");
                document.Close();
            }
        }

    }
}

