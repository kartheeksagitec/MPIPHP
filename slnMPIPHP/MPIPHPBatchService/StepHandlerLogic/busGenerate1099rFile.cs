using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.ExceptionPub;
using System.Data;
using Sagitec.BusinessObjects;
using MPIPHP.BusinessObjects;
using System.Collections.ObjectModel;
using MPIPHP.CustomDataObjects;
using Sagitec.DBUtility;
using MPIPHP.DataObjects;
using MPIPHPJobService;


namespace MPIPHPJobService
{
    public class busGenerate1099rFile : busBatchHandler
    {


        public string istrProcessName { get; set; }
        public int iintYear { get; set; }
        public DateTime idtCorrectionStartDate { get; set; }

        public void ProcessBatch()
        {
            try
            {
                 istrProcessName = icdoBatchSchedule.step_name;
                 RetrieveBatchParameters();
                 if (istrProcessName.ToLower() == "annual")
                 {
                     Create1099rFormAndIRSFile("PENSION");
                     Create1099rFormAndIRSFile("IAP");
                 }
                 if (istrProcessName.ToLower() == "corrected")
                 {
                    Create1099rFormAndIRSFile("PENSION", true);
                    Create1099rFormAndIRSFile("IAP", true);
                }
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog(icdoBatchSchedule.step_name + " failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
            }
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
                            case busConstant.JobParamYear1099r:
                                istrProcessName = Convert.ToString(lobjParam.icdoJobParameters.param_value);
                                break;
                            case busConstant.JobParamProcess1099r:
                                iintYear = Convert.ToInt32(lobjParam.icdoJobParameters.param_value);
                                break;
                            case busConstant.JobParamCorrectionStartDate1099r:
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty())
                                    idtCorrectionStartDate = Convert.ToDateTime(lobjParam.icdoJobParameters.param_value);
                                break;

                        }
                    }
                }
            }
        }
        private void Create1099rFormAndIRSFile(string Plan_Identifier_Value, bool bCorrected = false)
        {
            try
            {
                string lstrReportPrefix = string.Empty;
                DataTable ldt1099rForm = new DataTable();
                
                idlgUpdateProcessLog("Creating Annual IRS File started", "INFO", istrProcessName);
                DataTable ldtReportResult;
                if(bCorrected)
                {
                    ldtReportResult = busBase.Select("cdoPayment1099r.rpt1099RCForm",
                    new object[3] { iintYear, Plan_Identifier_Value, idtCorrectionStartDate });
                }
                else
                {
                    ldtReportResult = busBase.Select("cdoPayment1099r.rpt1099rForm",
                    new object[2] { iintYear, Plan_Identifier_Value });
                }

                string lstrTemplatePath = string.Empty;
                string lstr1099RReportPath = string.Empty;

                if (ldtReportResult.Rows.Count > 0)
                {
                    //FM upgrade: 6.0.6.2 changes - busProcessFiles should be replaced with busProcessOutboundFile for outbound related method and processing
                    //busProcessFiles lobjProcessPensionFile = new busProcessFiles();
                    busProcessOutboundFile lobjProcessPensionFile = new busProcessOutboundFile();
                    lobjProcessPensionFile.iarrParameters = new object[4];
                    lobjProcessPensionFile.iarrParameters[0] = iintYear;
                    lobjProcessPensionFile.iarrParameters[1] = bCorrected ? true : false;
                    lobjProcessPensionFile.iarrParameters[2] = ldtReportResult;
                    lobjProcessPensionFile.iarrParameters[3] = bCorrected ? Plan_Identifier_Value+"_CORRECTED" : Plan_Identifier_Value;
                    this.DeleteFile(6);
                    lobjProcessPensionFile.CreateOutboundFile(6);
                }
                idlgUpdateProcessLog("Creating Annual IRS File finished successfully", "INFO", istrProcessName);
                
                //create excel file
                if(ldtReportResult != null && ldtReportResult.Rows.Count > 0)
                {
                    if (Plan_Identifier_Value == "PENSION")
                    {
                        if(bCorrected)
                        {
                            lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_1099R_PENSION + ".xlsx";
                            lstr1099RReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.REPORT_1099R_PENSION + "_Corrected" + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
                        }
                        else
                        {
                            lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_1099R_PENSION + ".xlsx";
                            lstr1099RReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.REPORT_1099R_PENSION + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
                        }
                    }
                    else if(Plan_Identifier_Value == "IAP")
                    {
                        if(bCorrected)
                        {
                            lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_1099R_IAP + ".xlsx";
                            lstr1099RReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.REPORT_1099R_IAP + "_Corrected" + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
                        }
                        else
                        {
                            lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_1099R_IAP + ".xlsx";
                            lstr1099RReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.REPORT_1099R_IAP + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
                        }
                    }
                    ldtReportResult.DefaultView.Sort = "ADDR_LENGTH DESC";
                    ldt1099rForm = ldtReportResult.DefaultView.ToTable();
                    ldt1099rForm.TableName = "Annual1099RDetails";

                    DataSet lds1099RReportDataForExcel = new DataSet();
                    if (ldt1099rForm != null && ldt1099rForm.Rows.Count > 0)
                        lds1099RReportDataForExcel.Tables.Add(ldt1099rForm);                   

                    busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                    lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstr1099RReportPath, "1099R_DETAILS", lds1099RReportDataForExcel);
                }
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Annual IRS File / 1099r Form failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }


    }
}
