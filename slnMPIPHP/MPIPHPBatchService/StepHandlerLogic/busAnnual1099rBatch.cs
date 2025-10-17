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
using MPIPHP.BusinessObjects.PayeeAccount;

namespace MPIPHPJobService
{
    public class busAnnual1099rBatch : busBatchHandler
    {
        public busYearEndProcessRequest ibusYearEndProcessRequest { get; set; }
        public string istrFederalIDIAP { get; set; }
        public string istrFederalIDOther { get; set; }
        public string istrStateIDIAP { get; set; }
        public string istrStateIDOther { get; set; }
        public DateTime idtStartDate { get; set; }
        public DateTime idtEndDate { get; set; }
        //public string istrTransmitterControlCode { get; set; }
        //public string istrEmployerName { get; set; }
        //public string istrEmployerStrAdd { get; set; }
        //public string istrEmployerState { get; set; }
        //public string istrZipCodeExtension { get; set; }
        //public string istrZipCode { get; set; }
        //public string istrStateCode { get; set; }
        //public string istrEmployerCity { get; set; }
        //public string istrStateEmployerAccNo { get; set; }
        //public string istrContactName { get; set; }
        //public string istrContactEmail { get; set; }
        busCreateReports lobjCreateReports = new busCreateReports();
        
        public void ProcessAnnual1099rBatch()
        {
            try
            {

                istrProcessName = icdoBatchSchedule.step_name;
                //Loading the approved Annual batch request
                LoadConstants();
                if (ibusYearEndProcessRequest == null)
                    LoadPayment1099rRequest(busConstant.BatchRequest1099rStatusPending);
                if (ibusYearEndProcessRequest.icdoYearEndProcessRequest.year_end_process_request_id > 0)
                {
                    try
                    {
                        busPayment1099r lobjPayment1099r = new busPayment1099r();
                        //dropping temp 1099r table if any
                        lobjPayment1099r.DropTemp1099rTable();
                        //creating new temp 1099r table for Tax year
                        lobjPayment1099r.CreateTemp1099rTableWithData(ibusYearEndProcessRequest.icdoYearEndProcessRequest.year);
                        //create 1099r
                        CreatePayment1099r();
                        //create 1099r details report
                        Create1099rDetailsReport();
                        //Populate the Excess Refund Table
                        PopulateExcessRefund(ibusYearEndProcessRequest.icdoYearEndProcessRequest.year);

                        //Delete Negative Amount Values 1099r
                        lobjPayment1099r.DeleteNegativeRecords(ibusYearEndProcessRequest.icdoYearEndProcessRequest.year);
                        //create 1099r Form and IRS file
                        Create1099rFormAndIRSFile("PENSION");
                        Create1099rFormAndIRSFile("IAP");
                        //create 945 report
                        // Create945Report();
                        //updating the annual batch status
                        UpdateBatchRequest();
                        //dropping temp 1099r table if any
                        lobjPayment1099r.DropTemp1099rTable();
                    }
                    catch (Exception ex)
                    {
                        idlgUpdateProcessLog("Updating Corrected 1099r Batch Request failed", "INFO", istrProcessName);
                        ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_value = busConstant.BatchRequest1099rStatusFailed;
                        ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_date = DateTime.Now; //asharma Changed on 12/3/2012 based on discussion with Vinovin
                        ibusYearEndProcessRequest.icdoYearEndProcessRequest.processed_date = DateTime.Now;
                        ibusYearEndProcessRequest.icdoYearEndProcessRequest.Update();
                        ExceptionManager.Publish(ex);
                        throw ex;
                    }
                }

            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog(icdoBatchSchedule.step_name + " failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
            }
        }

        private void PopulateExcessRefund(int aintTaxYear)
        {
            DBFunction.DBNonQuery("cdoPayment1099r.PopulateExcessRefund", new object[1] { aintTaxYear },
                                   iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        public void ProcessEDDFiles()
        {
            Dictionary<string, busEDDReportData> eddReportStateData = new Dictionary<string, busEDDReportData>();
            eddReportStateData.Add(busConstant.CALIFORNIA, new busEDDReportData() { StateCode = busConstant.CALIFORNIA } );
            eddReportStateData.Add(busConstant.GEORGIA, new busEDDReportData() { StateCode = busConstant.GEORGIA });
            eddReportStateData.Add(busConstant.NORTH_CAROLINA, new busEDDReportData() { StateCode = busConstant.NORTH_CAROLINA });
            eddReportStateData.Add(busConstant.OREGON, new busEDDReportData() { StateCode = busConstant.OREGON });
            eddReportStateData.Add(busConstant.VERGINIA, new busEDDReportData() { StateCode = busConstant.VERGINIA });

            if (this.iobjPassInfo.iconFramework.State == ConnectionState.Closed)
            {
                this.iobjPassInfo.iconFramework.Open();
            }
            RetrieveBatchParameters();

            busEDDOutboundFile lobjbusEDDOutboundFile = null;
            DataSet dsPaymentEDDFilePensionReportData = new DataSet();
            DataSet dsPaymentEDDFileIAPReportData = new DataSet();
            foreach (KeyValuePair<string, busEDDReportData> item in eddReportStateData)
            {
                if (item.Key == busConstant.CALIFORNIA)
                {
                    busProcessOutboundFile lobjProcessPensionFile = new busProcessOutboundFile();
                    lobjProcessPensionFile.iarrParameters = new object[4];

                    lobjProcessPensionFile.iarrParameters[0] = busConstant.PENSION;
                    lobjProcessPensionFile.iarrParameters[1] = eddReportStateData[item.Key];
                    lobjProcessPensionFile.iarrParameters[2] = idtStartDate;
                    lobjProcessPensionFile.iarrParameters[3] = idtEndDate;

                    this.DeleteFile(1007);

                    idlgUpdateProcessLog(string.Format("Started creating {0} EDD {1} File", busConstant.PENSION, item.Key), "INFO", istrProcessName);
                    lobjProcessPensionFile.CreateOutboundFile(1007);
                    idlgUpdateProcessLog(string.Format("Ended creating {0} EDD {1} File", busConstant.PENSION, item.Key), "INFO", istrProcessName);

                    idlgUpdateProcessLog(string.Format("Started creating {0} EDD {1} File", busConstant.IAP, item.Key), "INFO", istrProcessName);
                    lobjProcessPensionFile.iarrParameters[0] = busConstant.IAP;
                    lobjProcessPensionFile.CreateOutboundFile(1007);
                    idlgUpdateProcessLog(string.Format("Ended creating {0} EDD {1} File", busConstant.IAP, item.Key), "INFO", istrProcessName);
                }
                else
                {
                    lobjbusEDDOutboundFile = new busEDDOutboundFile();
                    lobjbusEDDOutboundFile.Initialize(busConstant.PENSION, item.Value, idtStartDate, idtEndDate);
                    lobjbusEDDOutboundFile.GenerateEDDFileData();
                    lobjbusEDDOutboundFile = new busEDDOutboundFile();
                    lobjbusEDDOutboundFile.Initialize(busConstant.IAP, item.Value, idtStartDate, idtEndDate);
                    lobjbusEDDOutboundFile.GenerateEDDFileData();
                    if (item.Key == busConstant.OREGON)
                    {
                        string oregonTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.OREGON_PAYMENT_EDD_FILE_REPORT + ".xlsx";
                        string oregonReportPensionPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.OREGON_PAYMENT_EDD_FILE_REPORT + "_" + busConstant.PENSION + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
                        idlgUpdateProcessLog(string.Format("Started creating rptOregonPaymentEDDFileReport"), "INFO", istrProcessName);
                        busEDDOutboundFile.CreateOregonPaymentEDDFileReport(oregonTemplatePath, oregonReportPensionPath, item.Value.PaymentEDDFilePensionData);
                        idlgUpdateProcessLog(string.Format("Ended creating rptOregonPaymentEDDFileReport"), "INFO", istrProcessName);
                    }
                }
                dsPaymentEDDFilePensionReportData.Tables.Add(item.Value.PaymentEDDFilePensionData);
                dsPaymentEDDFileIAPReportData.Tables.Add(item.Value.PaymentEDDFileIAPData);
                idlgUpdateProcessLog(string.Format("Started creating rptEddFileReport_{0}", item.Key), "INFO", istrProcessName);
                busEDDOutboundFile.CreateEddFileReport(item.Key, item.Value.EDDFileReportData);
                idlgUpdateProcessLog(string.Format("Ended creating rptEddFileReport_{0}", item.Key), "INFO", istrProcessName);
            }
            string templatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.PAYMENT_EDD_FILE_REPORT + ".xlsx";
            string reportPensionPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.PAYMENT_EDD_FILE_REPORT + "_"  + busConstant.PENSION + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
            string reportIAPPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.PAYMENT_EDD_FILE_REPORT + "_" + busConstant.IAP + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

            idlgUpdateProcessLog(string.Format("Started creating rptPaymentEDDFileReport_{0}", busConstant.PENSION), "INFO", istrProcessName);
            busEDDOutboundFile.CreatePaymentEDDFileReport(templatePath, reportPensionPath, dsPaymentEDDFilePensionReportData);
            idlgUpdateProcessLog(string.Format("Ended creating rptPaymentEDDFileReport_{0}", busConstant.PENSION), "INFO", istrProcessName);

            idlgUpdateProcessLog(string.Format("Started creating rptPaymentEDDFileReport_{0}", busConstant.IAP), "INFO", istrProcessName);
            busEDDOutboundFile.CreatePaymentEDDFileReport(templatePath, reportIAPPath, dsPaymentEDDFileIAPReportData);
            idlgUpdateProcessLog(string.Format("Ended creating rptPaymentEDDFileReport_{0}", busConstant.IAP), "INFO", istrProcessName);

            idlgUpdateProcessLog("Started creating rptEDDExceptionReport", "INFO", istrProcessName);
            busEDDOutboundFile.CreateEDDExceptionReport(idtStartDate, idtEndDate);
            idlgUpdateProcessLog("Ended creating rptEDDExceptionReport", "INFO", istrProcessName);
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
                            case busConstant.JobParamStartDateEDD:
                                idtStartDate = Convert.ToDateTime(lobjParam.icdoJobParameters.param_value);
                                break;
                            case busConstant.JobParamEndDateEDD:
                                idtEndDate = Convert.ToDateTime(lobjParam.icdoJobParameters.param_value);
                                break;

                        }
                    }
                }
            }
        }
        
        private void LoadPayment1099rRequest(string astrStatusValue)
        {
            ibusYearEndProcessRequest = new busYearEndProcessRequest { icdoYearEndProcessRequest = new cdoYearEndProcessRequest() };

            DataTable ldt1099rRequests = busBase.Select<cdoYearEndProcessRequest>
                (new string[2] { enmYearEndProcessRequest.status_value.ToString(), enmYearEndProcessRequest.year_end_process_value.ToString() },
                new object[2] { astrStatusValue, busConstant.YEAR_END_PROC_1099R_ANNUAL },
                null, "YEAR desc");
            if (ldt1099rRequests.Rows.Count > 0)
            {
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.LoadData(ldt1099rRequests.Rows[0]);
            }
        }
        private void CreatePayment1099r()
        {
            try
            {
                idlgUpdateProcessLog("Creating Payment 1099r started", "INFO", istrProcessName);
                DBFunction.DBNonQuery("cdoPayment1099r.Create1099r",
                    new object[6] { istrFederalIDIAP, istrFederalIDOther, istrStateIDIAP, istrStateIDOther, ibusYearEndProcessRequest.icdoYearEndProcessRequest.year, ibusYearEndProcessRequest.icdoYearEndProcessRequest.year_end_process_request_id },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                idlgUpdateProcessLog("Creating Payment 1099r finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Payment 1099r failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }
        /// <summary>
        /// Method to create 1099r Details Report
        /// </summary>
        private void Create1099rDetailsReport()
        {
            try
            {
                
                idlgUpdateProcessLog("Creating 1099R Details Report started", "INFO", istrProcessName);
                DataTable ldtReportResult = busBase.Select("cdoPayment1099r.rptCreate1099rDetails",
                    new object[1] { ibusYearEndProcessRequest.icdoYearEndProcessRequest.year });
                ldtReportResult.TableName = "rpt1099RExceptionReport";
                if (ldtReportResult.Rows.Count > 0)
                   lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt1099RExceptionReport");
                idlgUpdateProcessLog("Creating 1099R Details Report finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating 1099R Details Report failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }
        /// <summary>
        /// Method to create 1099r Form and IRS File
        /// </summary>
        private void Create1099rFormAndIRSFile(string Plan_Identifier_Value)
        {
            try
            {
                string lstrReportPrefix = string.Empty;
                DataTable ldt1099rForm = new DataTable();
                idlgUpdateProcessLog("Creating Annual IRS File started", "INFO", istrProcessName);
                DataTable ldtReportResult = busBase.Select("cdoPayment1099r.rpt1099rForm",
                    new object[2] { ibusYearEndProcessRequest.icdoYearEndProcessRequest.year, Plan_Identifier_Value });
                if (ldtReportResult.Rows.Count > 0)
                {
                    //FM upgrade: 6.0.6.2 changes - busProcessFiles should be replaced with busProcessOutboundFile for outbound related method and processing
                    //busProcessFiles lobjProcessPensionFile = new busProcessFiles();
                    busProcessOutboundFile lobjProcessPensionFile = new busProcessOutboundFile();
                    lobjProcessPensionFile.iarrParameters = new object[4];
                    lobjProcessPensionFile.iarrParameters[0] = ibusYearEndProcessRequest.icdoYearEndProcessRequest.year;
                    lobjProcessPensionFile.iarrParameters[1] = false;
                    lobjProcessPensionFile.iarrParameters[2] = ldtReportResult;
                    lobjProcessPensionFile.iarrParameters[3] = Plan_Identifier_Value;
                    this.DeleteFile(6);
                    lobjProcessPensionFile.CreateOutboundFile(6);
                }
                idlgUpdateProcessLog("Creating Annual IRS File finished successfully", "INFO", istrProcessName);
                //Commented Abhishek - We do not need to do this.
                //idlgUpdateProcessLog("Creating Annual IRS 1099r Form started", "INFO", istrProcessName);

                //foreach (DataRow dr in ldtReportResult.Rows)
                //{
                //    lstrReportPrefix = string.Empty;
                //    lstrReportPrefix = dr["payee_account_id"].ToString() + "_" +
                //                        dr["distribution_code"].ToString() + "_" +
                //                       // dr["age59_split_flag"].ToString() + "_" +
                //                        dr["TAX_YEAR"].ToString() + "_" +
                //                        dr["corrected_flag"].ToString() + "_";
                //    ldt1099rForm = new DataTable();
                //    ldt1099rForm = ldtReportResult.Clone();
                //    ldt1099rForm.ImportRow(dr);
                //    ldt1099rForm.TableName = "rptForm1099Report";
                //    ldt1099rForm.AcceptChanges();
                //    lobjCreateReports.CreatePDFReport(ldt1099rForm, "rptForm1099RReport", lstrReportPrefix);
                //}
                //idlgUpdateProcessLog("Creating Annual 1099r Form finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Annual IRS File Form failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }
        private void UpdateBatchRequest()
        {
            try
            {
                idlgUpdateProcessLog("Updating Annual 1099r Batch Request started", "INFO", istrProcessName);
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_value = busConstant.BatchRequest1099rStatusComplete;
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.processed_date = iobjSystemManagement.icdoSystemManagement.batch_date; //asharma Changed on 12/3/2012 based on discussion with Vinovin
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_date = iobjSystemManagement.icdoSystemManagement.batch_date;
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.Update();
                idlgUpdateProcessLog("Updating Annual 1099r Batch Request finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Updating Annual 1099r Batch Request failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }
        void LoadConstants()
        {

            //For Query
            istrFederalIDIAP = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Federal_CODE_Id, "IAPR").description;

            istrFederalIDOther = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Federal_CODE_Id, "PENR").description;
            istrStateIDOther = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_Id, "PENR").description;

            istrStateIDIAP = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_Id, "IAPR").description;
            
            ////Other Constant
            //istrTransmitterControlCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CODE_ID, "TCNC").description;
            //istrEmployerName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CODE_ID, "MPI").description;
            //istrEmployerStrAdd = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_STREET_ADDRESS_CODE_ID, "VENT").description;
            //istrEmployerCity = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CITY_CODE_ID, "STUC").description;
            //istrEmployerState = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_STATE_CODE_ID, "CA").description;


            //istrZipCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_ZIP_CODE_ID, "1099").description;
            //istrStateCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_ID, "CALF").description;
            //istrContactName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.CONTACT_NAME_CODE_ID, "CONN").description;
            //istrContactEmail = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.CONTACT_EMAIL_CODE_ID, "CONE").description;




        }
    }
}




