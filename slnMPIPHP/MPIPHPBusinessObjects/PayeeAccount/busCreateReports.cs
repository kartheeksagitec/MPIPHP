using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.IO;
using Microsoft.Reporting.WinForms;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busCreateReports : busMPIPHPBase
    {
        int aintIncludeReissueItem = 0;
        int aintCheckStartdate = 0;
        public DataTable dtlSceduleInfoTble { get; set; }
        public string istrEmployerName { get; set; }
        public string istrFullFileName { get; set; }
        public string istrEmployerStrAdd { get; set; }
        public string istrEmployerState { get; set; }
        public string istrTelExtension { get; set; }
        public string istrZipCode { get; set; }
        public string istrStateCode { get; set; }
        public string istrEmployerCity { get; set; }
        public string istrStateEmployerAccNo { get; set; }
        public string istrPayerTaxNo { get; set; }
        public string istrContactEmail { get; set; }
        public string istrWagePlanCode { get; set; }
        public int aintPlan { get; set; }
        public string istrPlanIdentifierValue { get; set; }
        public string istrContactName { get; set; }
        public busCreateReports()
        {
        }
        public busCreateReports(bool IncludeReissueItem, bool CheckStartDate)
        {
            if (IncludeReissueItem)
            {
                aintIncludeReissueItem = 1;
            }
            if (!CheckStartDate)
            {
                aintCheckStartdate = 1;
            }
        }
        public void CreateSceduleInfoTble(int aintPaymentSceduleId, string astrSceduleType, DateTime adtpaymentdate = new DateTime())
        {
            dtlSceduleInfoTble = new DataTable();
            dtlSceduleInfoTble.Columns.Add(new DataColumn("PAYMENT_SCHEDULE_ID", typeof(int)));
            dtlSceduleInfoTble.Columns.Add(new DataColumn("SCHEDULE_DESC", typeof(string)));
            dtlSceduleInfoTble.Columns.Add(new DataColumn("Payment_Date", typeof(DateTime)));
            DataRow dr = dtlSceduleInfoTble.NewRow();
            dr["PAYMENT_SCHEDULE_ID"] = aintPaymentSceduleId;
            dr["SCHEDULE_DESC"] = " - " + aintPaymentSceduleId + " - " + astrSceduleType;
            dr["Payment_Date"] = adtpaymentdate;
            dtlSceduleInfoTble.Rows.Add(dr);
            dtlSceduleInfoTble.TableName = "SceduleInfoTble";
        }
        public void CreateSceduleInfoTbleForAchreport(int aintPaymentSceduleId, string astrSceduleType)
        {
            LoadConstants();
            if (astrSceduleType == busConstant.PaymentScheduleTypeWeekly || astrSceduleType == busConstant.PaymentScheduleAdhocWeekly)
            {
                istrPlanIdentifierValue = "IAP";
            }
            dtlSceduleInfoTble = new DataTable();
            dtlSceduleInfoTble.Columns.Add(new DataColumn("COMPANY_NAME", typeof(string)));
            dtlSceduleInfoTble.Columns.Add(new DataColumn("TAX_ID", typeof(string)));
            dtlSceduleInfoTble.Columns.Add(new DataColumn("ACCOUNT_NO", typeof(string)));
            dtlSceduleInfoTble.Columns.Add(new DataColumn("CONTACT_NAME", typeof(string)));
            dtlSceduleInfoTble.Columns.Add(new DataColumn("COMPANY_FAX", typeof(string)));
            dtlSceduleInfoTble.Columns.Add(new DataColumn("COMPANY_TEL_EXT", typeof(string)));
            DataRow dr = dtlSceduleInfoTble.NewRow();
            dr["COMPANY_NAME"] = istrEmployerName;
            dr["TAX_ID"] = istrPayerTaxNo;
            dr["ACCOUNT_NO"] = istrPayerTaxNo;
            dr["CONTACT_NAME"] = istrContactName;
            dr["COMPANY_FAX"] = "(327) 877-2223";

            dr["COMPANY_TEL_EXT"] = istrTelExtension;
            dtlSceduleInfoTble.Rows.Add(dr);
            dtlSceduleInfoTble.TableName = "SceduleInfoTble";
        }
        void LoadConstants()
        {

            ////For Query
            if (istrPlanIdentifierValue != "IAP")
            {
                //istrStateEmployerAccNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Federal_CODE_Id, "PENR").description;
                istrEmployerName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CODE_ID, "MPEN").description;
                istrPayerTaxNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_Id, "PENC").description;


            }
            else
            {
                //istrStateEmployerAccNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Federal_CODE_Id, "IAPR").description;
                istrEmployerName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CODE_ID, "MIAP").description;
                istrPayerTaxNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_Id, "IAPC").description;
            }
            //istrFederalIDIAP = 

            //istrFederalIDOther = 
            //Other Constant




            istrEmployerStrAdd = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_STREET_ADDRESS_CODE_ID, "VENT").description;
            istrEmployerCity = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CITY_CODE_ID, "STUC").description;
            istrEmployerState = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_STATE_CODE_ID, "CA").description;
            istrTelExtension = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_TEL_EXTENSION_ID, "CANE").description;

            istrZipCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_ZIP_CODE_ID, "1099").description;
            istrStateCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_ID, "CALF").description;
            //istrContactName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.CONTACT_NAME_CODE_ID, "CONN").description;
            istrContactEmail = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.CONTACT_EMAIL_CODE_ID, "CONE").description;
            busUser lbusSenderInfo = new busUser();
            if (lbusSenderInfo.FindUser(this.iobjPassInfo.iintUserSerialID))
            {
                istrContactName = lbusSenderInfo.icdoUser.first_name.ToProperCase() + " " + lbusSenderInfo.icdoUser.last_name.ToProperCase();
            }




        }
        public DataTable TrialMonthlyBenefitPaymentbyItemReport(DateTime adtPaymentDate, bool ablnMnthly)
        {
            //if (ablnMnthly)
            return Select("cdoPayeeAccount.TrialMonthlyBenefitPaymentbyItemReport", new object[3] { adtPaymentDate, aintIncludeReissueItem, aintCheckStartdate });
            //else
            //    return Select("cdoPayeeAccount.TrialMonthlyBenefitPaymentbyItemReportAdhoc", new object[1] { adtPaymentDate });
        }
        public DataTable TrialMonthlyBenefitPaymentGrandTotalReport(DateTime adtPaymentDate, bool ablnMnthly)
        {
            //if (ablnMnthly)
            return Select("cdoPayeeAccount.TrialMonthlyBenefitPaymentGrandTotalReport", new object[3] { adtPaymentDate, aintIncludeReissueItem, aintCheckStartdate });
            //else
            //    return Select("cdoPayeeAccount.TrialMonthlyBenefitPaymentbyItemReportAdhoc", new object[1] { adtPaymentDate });
        }

        public DataTable TrialNewRetireeDetailReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialNewRetireeDetailReport", new object[1] { adtPaymentDate });
        }

        public DataTable TrialPayeeListReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialPayeeListReport", new object[1] { adtPaymentDate });
        }

        public DataTable TrialReinstatedRetireeDetailReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialReinstatedRetireeDetailReport", new object[1] { adtPaymentDate });
        }

        public DataTable TrialClosedorSuspendedPayeeAccountReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialClosedorSuspendedPayeeAccountReport", new object[1] { adtPaymentDate });
        }

        public DataTable TrialRetirementOptionSummaryReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialRetirementOptionSummaryReport", new object[1] { adtPaymentDate });
        }

        public DataSet TrialBenefitPaymentChangeReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            DataSet ldsBenefitPaymentChangeReport = new DataSet();

            DataTable ldtPersonDetails = Select("cdoPayeeAccount.TrialBenefitPaymentChangeMainReport", new object[2] { adtPaymentDate, busConstant.FLAG_NO });
            if (ldtPersonDetails.Rows.Count > 0)
            {
                ldtPersonDetails.TableName = busConstant.ReportTableName02;
                ldsBenefitPaymentChangeReport.Tables.Add(ldtPersonDetails.Copy());
            }
            DataTable ldtSummaryPersonDetails = Select("cdoPayeeAccount.TrialBenefitPaymentChangeMainReport", new object[2] { adtPaymentDate, busConstant.BenefitPaymentChangeGroupby });
            if (ldtSummaryPersonDetails.Rows.Count > 0)
            {
                ldtSummaryPersonDetails.TableName = busConstant.ReportTableName03;
                ldsBenefitPaymentChangeReport.Tables.Add(ldtSummaryPersonDetails.Copy());
            }
            return ldsBenefitPaymentChangeReport;
        }

        public DataTable TrialNonMonthlyPaymentDetailReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialNonMonthlyPaymentDetailReport", new object[1] { adtPaymentDate });
        }

        public DataTable TrialNonMonthlyPaymentDetailReportAdHoc(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialNonMonthlyPaymentDetailReportAdhoc", new object[1] { adtPaymentDate });
        }

        public DataTable TrialMonthlyBenefitPaymentSummaryReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialMonthlyBenefitPaymentSummaryReport", new object[1] { adtPaymentDate });
        }
        public DataTable TrialMonthlyReciepantCountReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialMonthlyReciepantCount", new object[1] { adtPaymentDate });
        }
        public DataTable FinalMonthlyBenefitPaymentbyItemReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalMonthlyBenefitPaymentbyItemReport", new object[1] { aintPaymentScheduleID });
        }
        public DataTable FinalMonthlyBenefitPaymentbyGrandTotalReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalMonthlyBenefitPaymentGrandTotalReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable FinalRetireeListMangtReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            //Note : Retiree List Report By Dates(online report) and New Retiree List Report (which gets generated from Payment Batch) shares same method , query and Report format.
            //Please make sure to change / test both the Reports whenever there is any change to the code , query or report design.

            return Select("cdoTempdata.rptRetireeList", new object[2] { adtPaymentDate, aintPaymentScheduleID });
        }

        public DataTable TrialVendorPaymentSummary(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialVendorPaymentSummary", new object[1] { adtPaymentDate });
        }


        public DataTable TrialVendorPaymentSummaryAdHoc(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialVendorPaymentSummaryAdHoc", new object[1] { adtPaymentDate });
        }
        public DataTable FinalOutstanding(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalOutstanding", new object[1] { aintPaymentScheduleID });
        }


        public DataTable FinalVendorPaymentSummary(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPaymentHistoryDetail.FinalVendorPaymentSummary", new object[1] { aintPaymentScheduleID });
        }
        public DataTable FindVendorPaymentSummaryStatus(int aintPlanID, int aintPaymentScheduleID, DateTime adtPaymentDate)
        {
            return Select("cdoPaymentHistoryDetail.FindVendorPaymentSummaryStatus", new object[2] { aintPaymentScheduleID, adtPaymentDate });
        }

        public DataTable FinalDuesWithholdingReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalDuesWithholdingReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable TrialMinimumGuaranteeChangeSummaryReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.TrialMinimumGuaranteeChangeSummaryReport", new object[1] { adtPaymentDate });
        }

        public DataTable FinalChildSupportReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalChildSupportReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable FinalIRSLimitReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.FinalIRSLimitReport", new object[1] { adtPaymentDate });
        }

        public DataTable FinalMasterPaymentReport(DateTime adtPaymentDate, int aintPaymentScheduleID, bool ablnMnthly)
        {
            if (ablnMnthly)
                return Select("cdoPayeeAccount.FinalMasterPaymentReport", new object[1] { aintPaymentScheduleID });
            else
                return Select("cdoPayeeAccount.FinalIAPMasterPaymentReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable FinalMasterBenefitPaymentReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalMasterBenefitPaymentReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable FinalACHRegisterReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalACHRegisterReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable FinalWireRegisterReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalWireRegisterReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable FinalWIRETransferReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalWIRETransferReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable FinalCheckRegisterReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalCheckRegisterReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable FinalIAPSummaryReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalIAPSummaryReport", new object[1] { aintPaymentScheduleID });
        }

        public DataSet FinalPayeeErrorReport(DateTime adtPaymentDate, DateTime adtStatusEffectiveDateFrom, DateTime adtStatusEffectiveDateTo)
        {
            DataTable ldtBatchDetails = new DataTable();
            ldtBatchDetails = Select("cdoPayeeAccount.FinalErrorReport", new object[4] { adtPaymentDate, busConstant.Flag_Yes, adtStatusEffectiveDateFrom, adtStatusEffectiveDateTo });
            ldtBatchDetails.TableName = "rptPayeeErrorReport";

            DataTable ldtPersonDetails = new DataTable();
            ldtPersonDetails = Select("cdoPayeeAccount.FinalErrorReport", new object[4] { adtPaymentDate, busConstant.FLAG_NO, adtStatusEffectiveDateFrom, adtStatusEffectiveDateTo });
            ldtPersonDetails.TableName = "rptPayeeErrorReportSup";

            DataSet ds = new DataSet();
            ds.Tables.Add(ldtBatchDetails.Copy());
            ds.Tables.Add(ldtPersonDetails.Copy());
            return ds;
            //Select("cdoPayeeAccount.FinalErrorReport", new object[1] { adtPaymentDate });
        }

        public DataTable CheckRegisterReportSummary(int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.CheckRegisterReportSummary", new object[1] { aintPaymentScheduleID });
        }
        public DataTable TFFRorTIAACREFReport(string astrChoice, int aintPaymentScheduleID)
        {
            return Select("cdoBenefitApplication.rptTFFRTransferReport", new object[2] { astrChoice, aintPaymentScheduleID });
        }

        public DataTable TFFRSalaryRecords(int aintPaymentScheduleID)
        {
            return Select("cdoBenefitApplication.rptTFFRSalaryRecords", new object[1] { aintPaymentScheduleID });
        }

        public DataTable MultipleACHOrCheckReport(int aintChoice, int aintPaymentScheduleID)
        {
            return Select("cdoPaymentHistoryDistribution.LoadPayeeWithMultipleACHorCheck", new object[2] { aintChoice, aintPaymentScheduleID });
        }
        //public DataTable FinalVendorPaymentSummary(DateTime adtPaymentDate, int aintPaymentScheduleID)
        //{
        //    return Select("cdoPayeeAccount.FinalVendorPaymentSummary", new object[1] { aintPaymentScheduleID });
        //}
        public DataTable FinalVendorPayment(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPaymentHistoryDetail.FinalVendorPayment", new object[1] { aintPaymentScheduleID });
        }
        public string CreatePDFReport(DataTable ldtbResultTable, string astrReportName, string astrPrefix = "", string astrSuffix = "")
        {

            ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            DataTable ldtbReportTable = ldtbResultTable;

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";
            ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);

            rvViewer.LocalReport.DataSources.Add(lrdsReport);
            if (dtlSceduleInfoTble != null)
            {
                ReportDataSource lrdsReportSced = new ReportDataSource(dtlSceduleInfoTble.TableName, dtlSceduleInfoTble);
                rvViewer.LocalReport.DataSources.Add(lrdsReportSced);
            }
            else
            {
                this.CreateSceduleInfoTble(0000, "ScheduleTypeNotDetermined", DateTime.MinValue);
                ReportDataSource lrdsReportSced = new ReportDataSource(dtlSceduleInfoTble.TableName, dtlSceduleInfoTble);
                rvViewer.LocalReport.DataSources.Add(lrdsReportSced);
            }
            byte[] bytes = rvViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string labsRptGenPath = string.Empty;

            if (astrPrefix.Contains("TRIAL"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_TRIAL_REPORT_PATH);
            }
            else if (astrPrefix.Contains("FINAL"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_FINAL_REPORT_PATH);
            }
            //Rohan RE-MD PIR 815
            else if (astrReportName == busConstant.MPIPHPBatch.REPORT_REEVALUATION_OF_MD && astrPrefix == busConstant.IAP)
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_REPORT_REEVALUATION_OF_MD_IAP);
            }
            //Rohan RE-MD PIR 815
            else if (astrReportName == busConstant.MPIPHPBatch.REPORT_REEVALUATION_OF_MD && astrPrefix != busConstant.IAP)
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_REPORT_REEVALUATION_OF_MD_Pension);
            }
            else
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);
            }

            string lstrReportFullName = string.Empty;

            if (astrPrefix.IsNotNullOrEmpty() && astrSuffix.IsNotNullOrEmpty())
                lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" + astrSuffix + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            else
            if (astrPrefix.IsNotNullOrEmpty())
                lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            else
            if (astrSuffix.IsNotNullOrEmpty())
                lstrReportFullName = labsRptGenPath + astrReportName + "_" + astrSuffix + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            else
            {
                lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            }

            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
           
            return lstrReportFullName;
        }

        #region Create Annual Statement Pdf
        //EStatement Enhancement
        public string CreateAnnualStatmentPDFReport(ReportViewer rvViewer, DataSet ldtbResultTable, string astrReportName, string astrPrefix = "", string astrMPIID = "", string AnnualAddressFlag = "", string lstrPostfixName = "",
            bool ablnCorrectedFlag = false, bool ablnEStatement = false)
        {
            //ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            //DataTable ldtbReportTable = ldtbResultTable;
            //rvViewer.LocalReport.ExecuteReportInCurrentAppDomain(AppDomain.CurrentDomain.Evidence);

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";

            foreach (DataTable dtResultTable in ldtbResultTable.Tables)
            {
                DataTable ldtbReportTable = dtResultTable;
                ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);
                rvViewer.LocalReport.DataSources.Add(lrdsReport);
            }
            if (dtlSceduleInfoTble != null)
            {
                ReportDataSource lrdsReportSced = new ReportDataSource(dtlSceduleInfoTble.TableName, dtlSceduleInfoTble);
                rvViewer.LocalReport.DataSources.Add(lrdsReportSced);
            }
            //ReportDataSource lrdsReport = new ReportDataSource(ldtbResultTable.DataSetName, ldtbResultTable);
            //rvViewer.LocalReport.DataSources.Add(lrdsReport);

            byte[] bytes = rvViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);


            rvViewer.LocalReport.ReleaseSandboxAppDomain();
            rvViewer.LocalReport.DataSources.Clear();
            //rvViewer.LocalReport.Dispose();
            rvViewer.Clear();
            rvViewer = null;
            //rvViewer.Dispose();

            string labsRptGenPath = string.Empty;

            if (astrPrefix.Contains("TRIAL"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_TRIAL_REPORT_PATH);
            }
            else if (astrPrefix.Contains("FINAL"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_FINAL_REPORT_PATH);
            }
            else if (astrPrefix.Contains("Annual") || astrMPIID.IsNotNullOrEmpty())
            {
                //Annual Statement Report Changes
                if (ablnEStatement)
                {
                    labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_ESTATEMENT_PATH);
                }
                else if (AnnualAddressFlag.IsNotNullOrEmpty() && AnnualAddressFlag == "FOREIGN_ADDRESS")
                {
                    if (ablnCorrectedFlag)
                        labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_FOREIGN_CORRECTED_ADDRESS_PATH);
                    else
                        labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_FOREIGN_ADDRESS_PATH);
                }
                else if (AnnualAddressFlag.IsNotNullOrEmpty() && AnnualAddressFlag == "DOMESTIC_ADDRESS")
                {
                    if (ablnCorrectedFlag)
                        labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_DOMESTIC_CORRECTED_ADDRESS_PATH);
                    else
                        labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_DOMESTIC_ADDRESS_PATH);
                }
                else if (AnnualAddressFlag.IsNotNullOrEmpty() && AnnualAddressFlag == "BAD_ADDRESS")
                {
                    if (ablnCorrectedFlag)
                        labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_BAD_CORRECTED_ADDRESS_PATH);
                    else
                        labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_BAD_ADDRESS_PATH);
                }
                else
                {
                    labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_PATH);

                }
                astrPrefix = string.Empty;
            }
            else
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);
            }

            string lstrReportFullName = string.Empty;

            if (astrPrefix.IsNotNullOrEmpty())
            {
                if (lstrPostfixName.IsNotNullOrEmpty())
                    lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "_" + lstrPostfixName + ".pdf";
                else
                    lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            }
            else
            {
                if (astrMPIID.IsNotNullOrEmpty())
                {
                    if (lstrPostfixName.IsNotNullOrEmpty())
                        lstrReportFullName = labsRptGenPath + astrReportName + "_" + astrMPIID + "_" +
                      DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "_" + lstrPostfixName + ".pdf";
                    else
                        lstrReportFullName = labsRptGenPath + astrReportName + "_" + astrMPIID + "_" +
                           DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                }
                else
                {
                    lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                }
            }

            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();

            return lstrReportFullName;
        }
        #endregion

        public string CreatePDFReport(DataSet ldtbResultTable, string astrReportName, string astrPrefix = "", string astrMPIID = "", string outputFolderPath = "")
        {

            ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            //DataTable ldtbReportTable = ldtbResultTable;

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";

            foreach (DataTable dtResultTable in ldtbResultTable.Tables)
            {
                DataTable ldtbReportTable = dtResultTable;
                ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);
                rvViewer.LocalReport.DataSources.Add(lrdsReport);
            }
            if (dtlSceduleInfoTble != null)
            {
                ReportDataSource lrdsReportSced = new ReportDataSource(dtlSceduleInfoTble.TableName, dtlSceduleInfoTble);
                rvViewer.LocalReport.DataSources.Add(lrdsReportSced);
            }
            //ReportDataSource lrdsReport = new ReportDataSource(ldtbResultTable.DataSetName, ldtbResultTable);
            //rvViewer.LocalReport.DataSources.Add(lrdsReport);

            byte[] bytes = rvViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
 
            string labsRptGenPath = string.Empty;

            if (!outputFolderPath.IsNullOrEmpty())
            {
                labsRptGenPath = outputFolderPath;
            }
            else if (astrPrefix.Contains("TRIAL"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_TRIAL_REPORT_PATH);
            }
             else if (astrPrefix.Contains("FINAL"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_FINAL_REPORT_PATH);
            }
			//LA Sunset - Payment Directives
            else if (astrPrefix.Contains(busConstant.PAYMENT_DIRECTIVES) && astrMPIID.IsNotNullOrEmpty())
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_PAYMENT_DIRECTIVE_PATH);
                astrPrefix = string.Empty;
            }
            else if (astrPrefix.Contains("Annual") || astrMPIID.IsNotNullOrEmpty())
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_PATH);
                astrPrefix = string.Empty;
            }
            else
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);
            }

            //LA Sunset - Payment Directives
            if (!Directory.Exists(labsRptGenPath))
                Directory.CreateDirectory(labsRptGenPath);

            string lstrReportFullName = string.Empty;

            if (astrPrefix.IsNotNullOrEmpty())
            {
                lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff") + ".pdf";
            }
            else
            {
                if (astrMPIID.IsNotNullOrEmpty())
                {
                    lstrReportFullName = labsRptGenPath + astrReportName + "_" + astrMPIID + "_" +
                       DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff") + ".pdf";
                }
                else
                {
                    lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff") + ".pdf";
                }
            }
            
           
            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
           
            return lstrReportFullName;
        }


        public string CreateIAPRecalcPDFReport(DataSet ldtbResultTable, string astrReportName, string astrPrefix = "", string astrMPIID = "")
        {

            ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            //DataTable ldtbReportTable = ldtbResultTable;

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";

            foreach (DataTable dtResultTable in ldtbResultTable.Tables)
            {
                DataTable ldtbReportTable = dtResultTable;
                ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);
                rvViewer.LocalReport.DataSources.Add(lrdsReport);
            }
            if (dtlSceduleInfoTble != null)
            {
                ReportDataSource lrdsReportSced = new ReportDataSource(dtlSceduleInfoTble.TableName, dtlSceduleInfoTble);
                rvViewer.LocalReport.DataSources.Add(lrdsReportSced);
            }
           

            byte[] bytes = rvViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string labsRptGenPath = string.Empty;

            if (astrPrefix.Contains(busConstant.MPIPHPBatch.GENERATED_RECALCULATE_IAP_ALLOCATION))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_RECALCULATE_IAP_ALLOCATION);
                astrPrefix = string.Empty;
            }

            else
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);
            }

            //LA Sunset - Payment Directives
            if (!Directory.Exists(labsRptGenPath))
                Directory.CreateDirectory(labsRptGenPath);

            string lstrReportFullName = string.Empty;

            if (astrPrefix.IsNotNullOrEmpty())
            {
                lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            }
            else
            {
                if (astrMPIID.IsNotNullOrEmpty())
                {
                    lstrReportFullName = labsRptGenPath + astrReportName + "_" + astrMPIID + "_" +
                       DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                }
                else
                {
                    lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                }
            }


            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            istrFullFileName = Path.GetFileNameWithoutExtension(lstrReportFullName);

            return lstrReportFullName;
        }

        #region Create Excel Report
        public string CreateExcelReport(DataTable ldtbResultTable, string astrReportName, string astrPrefix = "")
        {

            ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            DataTable ldtbReportTable = ldtbResultTable;

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";
            ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);

            rvViewer.LocalReport.DataSources.Add(lrdsReport);

            byte[] bytes = rvViewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string labsRptGenPath = string.Empty;
            labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);

            string lstrReportFullName = string.Empty;

            if (astrPrefix.IsNotNullOrEmpty())
                lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            else
            {
                lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            }

            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            return lstrReportFullName;
        }
        #endregion

        #region Used for generate Annual Statement report for Single person and MSS
        public byte[] CreateDynamicReport(DataSet ldtbResultTable, string astrReportName)
        {
            ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            //DataTable ldtbReportTable = ldtbResultTable;

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";

            foreach (DataTable dtResultTable in ldtbResultTable.Tables)
            {
                DataTable ldtbReportTable = dtResultTable;
                ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);
                rvViewer.LocalReport.DataSources.Add(lrdsReport);
            }

            byte[] bytes = rvViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            return bytes;

        }
        #endregion

    }
}
