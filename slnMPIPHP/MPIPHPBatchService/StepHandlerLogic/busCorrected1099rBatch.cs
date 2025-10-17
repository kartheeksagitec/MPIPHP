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
using System.Collections;

namespace MPIPHPJobService
{
    public class busCorrected1099rBatch : busBatchHandler
    {
        public busYearEndProcessRequest ibusYearEndProcessRequest { get; set; }
        public string istrFederalIDIAP { get; set; }
        public string istrFederalIDOther { get; set; }
        public string istrStateIDIAP { get; set; }
        public string istrStateIDOther { get; set; }
        public DateTime idtRefferenceDate { get;  set;}
        public DataTable ldt1099rCorrected { get; set; }
        public DataTable ldt1099rAnnual { get; set; }
        public DateTime idtYearDate { get; set; }
        public Collection<busPayment1099r> iclbusPayment1099r_Updated;
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

        public void ProcessCorrected1099rBatch()
        {
            try
            {

                istrProcessName = icdoBatchSchedule.step_name;
                //Loading the approved Annual batch request
                idtYearDate = new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.Year, 04, 15);
                LoadConstants();
                if (ibusYearEndProcessRequest == null)
                    LoadPayment1099rRequest(busConstant.BatchRequest1099rStatusPending);
                if (ibusYearEndProcessRequest.icdoYearEndProcessRequest.year_end_process_request_id > 0)
                {
                    try
                    {
                        busPayment1099r lobjPayment1099r = new busPayment1099r();
                        GetLastProceedProcRequest();
                        if (idtRefferenceDate != DateTime.MinValue)
                        {
                            iclbusPayment1099r_Updated = new Collection<busPayment1099r>();
                            //dropping temp 1099r table if any
                            lobjPayment1099r.DropTemp1099rTable();
                            //creating new temp 1099r table for Tax year
                            lobjPayment1099r.CreateTempCorrected1099rTableWithData(ibusYearEndProcessRequest.icdoYearEndProcessRequest.year, idtRefferenceDate);
                            //create 1099r
                            GetCorrected1099rData();
                            ////create 1099r details report
                            //CreatePayment1099r();
                            GetOld1099rdetail();
                            UpDateWithCancelDistrib();
                            UpDateWithSSNMerge();
                            UpDateWithDisability();
                            ////Delete 1099r
                            //lobjPayment1099r.DeleteNegativeRecords();
                            ////create 1099r Form and IRS file
                            Create1099rFormAndIRSFile("PENSION");
                            Create1099rFormAndIRSFile("IAP");
                            ////create 945 report
                            //// Create945Report();
                            ////updating the annual batch status
                            UpdateCorrectedFlag();
                            UpdateBatchRequest();
                            
                            ////dropping temp 1099r table if any
                            //lobjPayment1099r.DropTemp1099rTable();
                        }

                    }
                    catch (Exception ex)
                    {
                        idlgUpdateProcessLog("Updating Corrected 1099r Batch Request failed", "INFO", istrProcessName);
                        ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_value = busConstant.BatchRequest1099rStatusFailed;
                        ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_date = iobjSystemManagement.icdoSystemManagement.batch_date; //asharma Changed on 12/3/2012 based on discussion with Vinovin
                        ibusYearEndProcessRequest.icdoYearEndProcessRequest.processed_date = iobjSystemManagement.icdoSystemManagement.batch_date;
                        ibusYearEndProcessRequest.icdoYearEndProcessRequest.regenerate_flag = "N";
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
        private void GetLastProceedProcRequest()
        {
            try
            {
                idlgUpdateProcessLog("Creating Payment 1099r started", "INFO", istrProcessName);
                DataTable ldt1099rRequests = DBFunction.DBSelect("cdoPayment1099r.GetLastProceed1099rProcReq",
                    new object[1] {ibusYearEndProcessRequest.icdoYearEndProcessRequest.year },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                idlgUpdateProcessLog("Creating Payment 1099r finished successfully", "INFO", istrProcessName);
                if(ldt1099rRequests.Rows.Count>0)
                {
                    idtRefferenceDate = Convert.ToDateTime( ldt1099rRequests.Rows[0]["PROCESSED_DATE"]);
                }
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Payment 1099r failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }
        

        private void LoadPayment1099rRequest(string astrStatusValue)
        {
            ibusYearEndProcessRequest = new busYearEndProcessRequest { icdoYearEndProcessRequest = new cdoYearEndProcessRequest() };

            DataTable ldt1099rRequests = busBase.Select<cdoYearEndProcessRequest>
                (new string[2] { enmYearEndProcessRequest.status_value.ToString(), enmYearEndProcessRequest.year_end_process_value.ToString() },
                new object[2] { astrStatusValue, busConstant.CORRECTED_PROC_ANNUAL_STATM },
                null, "YEAR desc");
            if (ldt1099rRequests.Rows.Count > 0)
            {
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.LoadData(ldt1099rRequests.Rows[0]);
            }
            else
            {
                 ldt1099rRequests = busBase.Select<cdoYearEndProcessRequest>
                  (new string[2] { enmYearEndProcessRequest.regenerate_flag.ToString(), enmYearEndProcessRequest.year_end_process_value.ToString() },
                  new object[2] { busConstant.Flag_Yes, busConstant.CORRECTED_PROC_ANNUAL_STATM },
                  null, "PROCESSED_DATE desc");
                 if (ldt1099rRequests.Rows.Count > 0)
                 {
                     ibusYearEndProcessRequest.icdoYearEndProcessRequest.LoadData(ldt1099rRequests.Rows[0]);
                 }
            }
        }
        private void GetCorrected1099rData()
        {
            try
            {
                idlgUpdateProcessLog("Creating Payment 1099r started", "INFO", istrProcessName);
                ldt1099rCorrected = DBFunction.DBSelect("cdoPayment1099r.GetDataForCorrectedBatch",
                    new object[5] { istrFederalIDIAP, istrFederalIDOther, istrStateIDIAP, istrStateIDOther, ibusYearEndProcessRequest.icdoYearEndProcessRequest.year_end_process_request_id },
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
        private void UpDateWithCancelDistrib()
        {
            var CorrectedRows = (from c in ldt1099rCorrected.AsEnumerable()
                                 join o in ldt1099rAnnual.AsEnumerable() on
                                 new
                                 {
                                     cc = c.Field<int?>("PERSON_ID") == null ? 0 : Convert.ToInt32(c.Field<int?>("PERSON_ID")),
                                     cc1 = c.Field<string>("DISTRIBUTION_CODE").IsNullOrEmpty() ? "0" : c.Field<string>("DISTRIBUTION_CODE").Trim(),
                                     cc2 = c.Field<int?>("ORG_ID") == null ? 0 : Convert.ToInt32(c.Field<int?>("ORG_ID")),
                                     cc3 = c.Field<string>("PLAN_IDENTIFIER_VALUE").IsNullOrEmpty() ? "" : c.Field<string>("PLAN_IDENTIFIER_VALUE").Trim(),
                                 } equals
                                 new
                                 {
                                     cc = o.Field<int?>("PERSON_ID") == null ? 0 : Convert.ToInt32(o.Field<int?>("PERSON_ID")),
                                     cc1 = o.Field<string>("DISTRIBUTION_CODE").IsNullOrEmpty() ? "0" : o.Field<string>("DISTRIBUTION_CODE").Trim(),
                                     cc2 = o.Field<int?>("ORG_ID") == null ? 0 : Convert.ToInt32(o.Field<int?>("ORG_ID")),
                                     cc3 = o.Field<string>("PLAN_IDENTIFIER_VALUE").IsNullOrEmpty() ? "" : o.Field<string>("PLAN_IDENTIFIER_VALUE").Trim(),
                                 }
                                 where c.Field<string>("IDENTIFIER_VALUE") == busConstant.CORRECTED_IDENT_CANCEL
                                 select
                                 new
                                 {
                                     newrow = c,
                                     orgrow = o,
                                     GROSS_BENEFIT_AMOUNT = o.Field<decimal>("GROSS_BENEFIT_AMOUNT") - c.Field<decimal>("GROSS_BENEFIT_AMOUNT"),
                                     TAXABLE_AMOUNT = o.Field<decimal>("TAXABLE_AMOUNT") - c.Field<decimal>("TAXABLE_AMOUNT"),
                                     NON_TAXABLE_AMOUNT = o.Field<decimal>("NON_TAXABLE_AMOUNT") - c.Field<decimal>("NON_TAXABLE_AMOUNT"),
                                     FED_TAX_AMOUNT = o.Field<decimal>("FED_TAX_AMOUNT") - c.Field<decimal>("FED_TAX_AMOUNT"),
                                     STATE_TAX_AMOUNT = o.Field<decimal>("STATE_TAX_AMOUNT") - c.Field<decimal>("STATE_TAX_AMOUNT"),
                                     PAYMENT_1099R_ID = o.Field<int>("PAYMENT_1099R_ID")
                                 });


            foreach (var CorrectedRow in CorrectedRows)
            {
                busPayment1099r lbusPayment1099r = new busPayment1099r { icdoPayment1099r = new cdoPayment1099r() };
                if (iobjSystemManagement.icdoSystemManagement.batch_date < idtYearDate)
                {
                    lbusPayment1099r.icdoPayment1099r.LoadData(CorrectedRow.newrow);
                    lbusPayment1099r.icdoPayment1099r.gross_benefit_amount = CorrectedRow.GROSS_BENEFIT_AMOUNT;
                    lbusPayment1099r.icdoPayment1099r.taxable_amount = CorrectedRow.TAXABLE_AMOUNT;
                    lbusPayment1099r.icdoPayment1099r.non_taxable_amount = CorrectedRow.NON_TAXABLE_AMOUNT;
                    lbusPayment1099r.icdoPayment1099r.fed_tax_amount = CorrectedRow.FED_TAX_AMOUNT;
                    lbusPayment1099r.icdoPayment1099r.state_tax_amount = CorrectedRow.STATE_TAX_AMOUNT;
                    lbusPayment1099r.icdoPayment1099r.payment_1099r_id = CorrectedRow.PAYMENT_1099R_ID;
                    lbusPayment1099r.icdoPayment1099r.update_seq = Convert.ToInt32(CorrectedRow.orgrow["UPDATE_SEQ"]);
                    lbusPayment1099r.icdoPayment1099r.corrected_flag = busConstant.Flag_Yes;
                    lbusPayment1099r.icdoPayment1099r.Update();
                    iclbusPayment1099r_Updated.Add(lbusPayment1099r);
                }
                else
                {
                    lbusPayment1099r.icdoPayment1099r.LoadData(CorrectedRow.newrow);
                    lbusPayment1099r.icdoPayment1099r.gross_benefit_amount = CorrectedRow.GROSS_BENEFIT_AMOUNT;
                    lbusPayment1099r.icdoPayment1099r.gross_benefit_amount = CorrectedRow.GROSS_BENEFIT_AMOUNT;
                    lbusPayment1099r.icdoPayment1099r.taxable_amount = CorrectedRow.TAXABLE_AMOUNT;
                    lbusPayment1099r.icdoPayment1099r.non_taxable_amount = CorrectedRow.NON_TAXABLE_AMOUNT;
                    lbusPayment1099r.icdoPayment1099r.fed_tax_amount = CorrectedRow.FED_TAX_AMOUNT;
                    lbusPayment1099r.icdoPayment1099r.state_tax_amount = CorrectedRow.STATE_TAX_AMOUNT;
                    lbusPayment1099r.icdoPayment1099r.corrected_flag = busConstant.Flag_Yes;
                    lbusPayment1099r.icdoPayment1099r.Insert();
                }

            }
        }

        private void UpDateWithSSNMerge()
        {
            var CorrectedRows = (from c in ldt1099rCorrected.AsEnumerable()
                                 join o in ldt1099rAnnual.AsEnumerable() on
                                 new
                                 {
                                     cc = c.Field<int?>("PERSON_ID") == null ? 0 : Convert.ToInt32(c.Field<int?>("PERSON_ID")),
                                     cc1 = c.Field<string>("DISTRIBUTION_CODE").IsNullOrEmpty() ? "0" : c.Field<string>("DISTRIBUTION_CODE").Trim(),
                                     cc2 = c.Field<int?>("ORG_ID") == null ? 0 : Convert.ToInt32(c.Field<int?>("ORG_ID")),
                                     cc3 = c.Field<string>("PLAN_IDENTIFIER_VALUE").IsNullOrEmpty() ? "" : c.Field<string>("PLAN_IDENTIFIER_VALUE").Trim(),
                                 } equals
                                 new
                                 {
                                     cc = o.Field<int?>("PERSON_ID") == null ? 0 : Convert.ToInt32(o.Field<int?>("PERSON_ID")),
                                     cc1 = o.Field<string>("DISTRIBUTION_CODE").IsNullOrEmpty() ? "0" : o.Field<string>("DISTRIBUTION_CODE").Trim(),
                                     cc2 = o.Field<int?>("ORG_ID") == null ? 0 : Convert.ToInt32(o.Field<int?>("ORG_ID")),
                                     cc3 = o.Field<string>("PLAN_IDENTIFIER_VALUE").IsNullOrEmpty() ? "" : o.Field<string>("PLAN_IDENTIFIER_VALUE").Trim(),
                                 }

                                 where c.Field<string>("IDENTIFIER_VALUE") == busConstant.CORRECTED_IDENT_SSN
                                 select
                                 new
                                 {
                                     newrow = c,
                                     orgrow = o,
                                     PERSON_ID = c.Field<int>("PERSON_ID") ,
                                     PAYMENT_1099R_ID = o.Field<int>("PAYMENT_1099R_ID") 
                                 });


            foreach (var CorrectedRow in CorrectedRows)
            {
                busPayment1099r lbusPayment1099r = new busPayment1099r { icdoPayment1099r = new cdoPayment1099r() };
                if (iobjSystemManagement.icdoSystemManagement.batch_date < idtYearDate)
                {
                    lbusPayment1099r.icdoPayment1099r.LoadData(CorrectedRow.newrow);
                    lbusPayment1099r.icdoPayment1099r.person_id = CorrectedRow.PERSON_ID;
                    lbusPayment1099r.icdoPayment1099r.payment_1099r_id = CorrectedRow.PAYMENT_1099R_ID;
                    lbusPayment1099r.icdoPayment1099r.update_seq = Convert.ToInt32(CorrectedRow.orgrow["UPDATE_SEQ"]);
                    
                    lbusPayment1099r.icdoPayment1099r.corrected_flag = busConstant.Flag_Yes;
                    lbusPayment1099r.icdoPayment1099r.Update();
                    iclbusPayment1099r_Updated.Add(lbusPayment1099r);
                }
                else
                {
                    lbusPayment1099r.icdoPayment1099r.LoadData(CorrectedRow.newrow);
                    lbusPayment1099r.icdoPayment1099r.person_id = CorrectedRow.PERSON_ID;
                    
                    lbusPayment1099r.icdoPayment1099r.corrected_flag = busConstant.Flag_Yes;
                    lbusPayment1099r.icdoPayment1099r.Insert();
                }

            }



        }
        private void UpDateWithDisability()
        {
            var CorrectedRows = (from c in ldt1099rCorrected.AsEnumerable()
                                 join o in ldt1099rAnnual.AsEnumerable() on
                                 new
                                 {
                                     cc = c.Field<int?>("PERSON_ID") == null ? 0 : Convert.ToInt32(c.Field<int?>("PERSON_ID")),
                                     

                                    cc3 = c.Field<string>("PLAN_IDENTIFIER_VALUE").IsNullOrEmpty() ? "" : c.Field<string>("PLAN_IDENTIFIER_VALUE").Trim(),
                                 } equals
                                 new
                                 {
                                     cc = o.Field<int?>("PERSON_ID") == null ? 0 : Convert.ToInt32(o.Field<int?>("PERSON_ID")),
                                   
                                    cc3 = o.Field<string>("PLAN_IDENTIFIER_VALUE").IsNullOrEmpty() ? "" : o.Field<string>("PLAN_IDENTIFIER_VALUE").Trim(),
                                     
                                 }

                                 where c.Field<string>("IDENTIFIER_VALUE") == busConstant.CORRECTED_IDENT_DSBL
                                 select
                                 new
                                 {
                                     newrow = c,
                                     orgrow = o,

                                     GROSS_BENEFIT_AMOUNT = o.Field<decimal>("GROSS_BENEFIT_AMOUNT") - c.Field<decimal>("GROSS_BENEFIT_AMOUNT"),
                                     PAYMENT_1099R_ID = o.Field<int>("PAYMENT_1099R_ID"),
                                     OLD_DISTRIBUTION_CODE = c.Field<string>("DISTRIBUTION_CODE"),
                                     NEW_DISTRIBUTION_CODE = o.Field<string>("DISTRIBUTION_CODE")
                                 });


            foreach (var CorrectedRow in CorrectedRows)
            {
                busPayment1099r lbusPayment1099r = new busPayment1099r { icdoPayment1099r = new cdoPayment1099r() };
                if (iobjSystemManagement.icdoSystemManagement.batch_date < idtYearDate)
                {
                    if (CorrectedRow.NEW_DISTRIBUTION_CODE == CorrectedRow.OLD_DISTRIBUTION_CODE)
                    {
                        lbusPayment1099r.icdoPayment1099r.LoadData(CorrectedRow.newrow);
                        lbusPayment1099r.icdoPayment1099r.gross_benefit_amount = CorrectedRow.GROSS_BENEFIT_AMOUNT;
                        lbusPayment1099r.icdoPayment1099r.payment_1099r_id = CorrectedRow.PAYMENT_1099R_ID;
                        lbusPayment1099r.icdoPayment1099r.update_seq = Convert.ToInt32(CorrectedRow.orgrow["UPDATE_SEQ"]);
                        
                        lbusPayment1099r.icdoPayment1099r.corrected_flag = busConstant.Flag_Yes;
                        lbusPayment1099r.icdoPayment1099r.Update();
                        iclbusPayment1099r_Updated.Add(lbusPayment1099r);
                    }
                    else
                    {
                        lbusPayment1099r.icdoPayment1099r.LoadData(CorrectedRow.newrow);
                        lbusPayment1099r.icdoPayment1099r.gross_benefit_amount = CorrectedRow.GROSS_BENEFIT_AMOUNT;
                        lbusPayment1099r.icdoPayment1099r.corrected_flag = busConstant.Flag_Yes;
                        lbusPayment1099r.icdoPayment1099r.Insert();
                        iclbusPayment1099r_Updated.Add(lbusPayment1099r);
                    }
                }
                else
                {
                    lbusPayment1099r.icdoPayment1099r.LoadData(CorrectedRow.newrow);
                    
                    lbusPayment1099r.icdoPayment1099r.Insert();
                }

            }



        }
        void UpdateCorrectedFlag()
        {
            try
            {
                foreach (busPayment1099r lbusPayment1099r in iclbusPayment1099r_Updated)
                {
                    lbusPayment1099r.icdoPayment1099r.corrected_flag = busConstant.FLAG_NO;
                    lbusPayment1099r.icdoPayment1099r.Update();
                }
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Update Temp 1099r records", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }
        /// <summary>
        /// Method to create 1099r Details Report
        /// </summary>
        private void GetOld1099rdetail()
        {
            try
            {

                idlgUpdateProcessLog("Creating 1099R Details Report started", "INFO", istrProcessName);
                //Review for adding more criteria
                ldt1099rAnnual = busBase.Select("cdoPayment1099r.Get1099rDataForYear",
                    new object[1] { ibusYearEndProcessRequest.icdoYearEndProcessRequest.year });
                //ldtReportResult.TableName = "rpt1099RExceptionReport";
                //if (ldtReportResult.Rows.Count > 0)
                //    lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt1099RExceptionReport");
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
                DataTable ldtReportResult = busBase.Select("cdoPayment1099r.rpt1099rFormCorrected",
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
               
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Annual IRS File failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }
        private void UpdateBatchRequest()
        {
            try
            {
                idlgUpdateProcessLog("Updating Corrected 1099r Batch Request started", "INFO", istrProcessName);
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_value = busConstant.BatchRequest1099rStatusComplete;
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_date = iobjSystemManagement.icdoSystemManagement.batch_date; //asharma Changed on 12/3/2012 based on discussion with Vinovin
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.processed_date = iobjSystemManagement.icdoSystemManagement.batch_date;
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.regenerate_flag = "N";
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.Update();
                idlgUpdateProcessLog("Updating Corrected 1099r Batch Request finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Updating Corrected 1099r Batch Request failed", "INFO", istrProcessName);
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_value = busConstant.BatchRequest1099rStatusFailed;
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_date = iobjSystemManagement.icdoSystemManagement.batch_date; //asharma Changed on 12/3/2012 based on discussion with Vinovin
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.processed_date = iobjSystemManagement.icdoSystemManagement.batch_date;
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.Update();
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

            
        }
    }

    
}
