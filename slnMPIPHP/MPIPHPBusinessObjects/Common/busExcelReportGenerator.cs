using System;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MPIPHP.BusinessObjects
{
    public class busExcelReportGenerator
    {

        #region Functions

        /// <summary>
        /// This method creates the Excel file from Template and populates the values from the collection.
        /// </summary>
        public void CreateExcelReport(string astrTemplateFile, string astrFileName, string astrWorkSheetName, DataSet adsMain)
        {
            Excel.Application oXL = new Excel.Application();
            if (oXL.IsNotNull() && astrFileName.IsNotNull() && astrFileName.Length > 0)
            {
                // Donot display any alerts for user.
                oXL.DisplayAlerts = false;
                // Step1: Create a xls file using the xlt Template.astrFileName
                Excel.Workbook lobjTemplateWorkbook = null;
                object missing = Type.Missing;
                try
                {

                    lobjTemplateWorkbook = oXL.Workbooks.Open(astrTemplateFile, 0, true, 5,
                              string.Empty, string.Empty, true, Excel.XlPlatform.xlWindows, busConstant.DELIMITERTAB, false, false,
                              0, true, 0, Excel.XlCorruptLoad.xlNormalLoad);
                    //oXL.Workbooks[1].SaveAs(astrFileName, missing, missing, missing, false, missing, Excel.XlSaveAsAccessMode.xlExclusive, missing, missing, missing, missing, missing);

                    oXL.Workbooks[1].SaveAs(astrFileName, Excel.XlFileFormat.xlOpenXMLWorkbook, Missing.Value, Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                                Excel.XlSaveConflictResolution.xlUserResolution, true, Missing.Value, Missing.Value, Missing.Value);
                }
                finally
                {
                    // Release the resources.
                    if (oXL.Workbooks.IsNotNull())
                        oXL.Workbooks.Close();

                    if (lobjTemplateWorkbook.IsNotNull())
                        while (Marshal.ReleaseComObject(lobjTemplateWorkbook) > 0) ;

                    if (oXL.Workbooks.IsNotNull())
                        while (Marshal.ReleaseComObject(oXL.Workbooks) > 0) ;

                    lobjTemplateWorkbook = null;
                }

                Excel.Workbook lobjWorkbook = null;
                Excel.Worksheet lobjSheet = null;
                Excel.Range lobjRange = null;

                try
                {
                    // Step2: Open the created xls file
                    lobjWorkbook = oXL.Workbooks.Open(astrFileName, 0, false, 5,
                              string.Empty, string.Empty, true, Excel.XlPlatform.xlWindows, busConstant.DELIMITERTAB, true, false,
                              0, true, 0, Excel.XlCorruptLoad.xlNormalLoad);

                    lobjSheet = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];
                    lobjSheet.Name = astrWorkSheetName;

                    lobjSheet.Unprotect(System.Reflection.Missing.Value);

                    if (astrTemplateFile.Contains(busConstant.PENSION_ACTUARY_REPORT))
                    {
                        CreatePensionActuaryReport(adsMain, lobjSheet, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.REPORT_5500))
                    {
                        Excel._Worksheet lobjSheet5500_Counts = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];
                        Excel._Worksheet lobjSheetSSAReport05A = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[2];
                        Excel._Worksheet lobjSheetSSAReport05B = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[3];

                        Create5500Report(adsMain, lobjSheet5500_Counts, lobjSheetSSAReport05A, lobjSheetSSAReport05B, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.REPORT_ANNUAL_STATEMENT_PARTICIPANT_DETAIL))
                    {
                        Excel.Worksheet lobjSheetAnnualStatementParticipantDetails = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];
                        CreateAnnualStatementParticipantDetails(adsMain, lobjSheetAnnualStatementParticipantDetails, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.REPORT_IAP_ADJUSTMENT_PAYMENT))
                    {
                        Excel.Worksheet lobjSheetAdjustmentPayment = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];
                        CreateIapAdjustmentPayment(adsMain, lobjSheetAdjustmentPayment, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.REPORT_1099R_PENSION))
                    {
                        Excel.Worksheet lobjSheetPension1099RDetails = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];
                        CreateAnnual1099RDetails(adsMain, lobjSheetPension1099RDetails, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.REPORT_1099R_IAP))
                    {
                        Excel.Worksheet lobjSheetIAP1099RDetails = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];
                        CreateAnnual1099RDetails(adsMain, lobjSheetIAP1099RDetails, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.REPORT_PENSION_BENEFIT_VERIFICATION))
                    {
                        Excel._Worksheet lobjSheetSixtyDaysLetters = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[2];
                        Excel._Worksheet lobjSheetThirtyDaysLetters = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[3];
                        Excel._Worksheet lobjSheetSuspensionLetters = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[4];
                        CreatePensionBenefitVerificationReport(adsMain, lobjSheet, lobjSheetSixtyDaysLetters, lobjSheetThirtyDaysLetters, lobjSheetSuspensionLetters, ref lobjRange);
                    }
                    //Ticket#76267
                    else if (astrTemplateFile.Contains(busConstant.REPORT_RETIREE_HEALTH_ELIGIBLE) && !astrTemplateFile.Contains("30Day"))
                    {
                        Excel._Worksheet lobjSheetRetireeHealthEligible = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];

                        CreateRetireeHealthEligibleReport(adsMain, lobjSheetRetireeHealthEligible, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.REPORT_ANNUAL_RECALCULATE_TAXES))
                    {
                        Excel._Worksheet lobjSheetRetireeHealthEligible = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];

                        CreateAnnualRecalculateTaxesReport(adsMain, lobjSheetRetireeHealthEligible, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.REPORT_UVHP_EE_REFUND))
                    {
                        Excel._Worksheet lobjSheetUVHPandEEList = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];
                        CreateUVHPEEContributionRefundReport(adsMain, lobjSheetUVHPandEEList, ref lobjRange);

                    }
                    else if (astrTemplateFile.Contains(busConstant.REPORT_RETIREE_HEALTH_ELIGIBLE_30_DAY))
                    {
                        Excel._Worksheet lobjSheetRetireeHealthEligible30Day = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];

                        CreateRetireeHealthEligibleReport30Day(adsMain, lobjSheetRetireeHealthEligible30Day, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.REPORT_IAP_REQUIRED_MINIMUM_DISTRIBUTION))
                    {
                        Excel._Worksheet lobjSheetRequiredMinimumDistribution = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];

                        CreateIAPRequiredMinimumDistributionReport(adsMain, lobjSheetRequiredMinimumDistribution, ref lobjRange);
                    }
                    //rid 76227
                    else if (astrTemplateFile.Contains(busConstant.PENSION_ELIGIBILITY_BATCH_REPORT))
                    {
                        Excel._Worksheet lobjSheetPensionEligibility = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];

                        CreatePensionEligibilityBatchReport(adsMain, lobjSheetPensionEligibility, ref lobjRange);
                    }
                    //Ticket 79238 - Create Excel file with participant mailing addresses
                    else if (astrTemplateFile.Contains(busConstant.MD_PARTICIPANT_ADDRESS_BATCH_REPORT))
                    {
                        Excel._Worksheet lobjSheetMdAddresses = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];

                        CreateMdAddressBatchReport(adsMain, lobjSheetMdAddresses, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.LAST_ONE_YEAR_DEATH_NOTIFICATION_REPORT))
                    {
                        Excel._Worksheet lobjSheetDeathNotification = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];

                        CreateLastOneYearDeathNotificationReport(adsMain, lobjSheetDeathNotification, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.VENDOR_PAYMENT_SUMMARY_REPORT))
                    {
                        Excel._Worksheet lobjVendorPaymentSummary = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];

                        CreateVendorPaymentSummaryReport(adsMain, lobjVendorPaymentSummary, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.OREGON_PAYMENT_EDD_FILE_REPORT))
                    {
                        Excel._Worksheet lobjEmployeeDetailReport = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];

                        CreateOregonPaymentEDDFileReport(adsMain, lobjEmployeeDetailReport, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.PAYMENT_EDD_FILE_REPORT))
                    {
                        CreatePaymentEDDFileReport(adsMain, lobjWorkbook.Sheets, ref lobjRange);
                    }
                    else if (astrTemplateFile.Contains(busConstant.STATEMENT_PARTICIPANTS_WITH_STATEMENT_FLAG))
                    {
                        CreateStatementParticipantsWithStatementFlagReport(adsMain, lobjWorkbook.Sheets, ref lobjRange);
                    }

                    lobjWorkbook.SaveAs(astrFileName, Excel.XlFileFormat.xlOpenXMLWorkbook, Missing.Value, Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                              Excel.XlSaveConflictResolution.xlUserResolution, true, Missing.Value, Missing.Value, Missing.Value);

                }
                finally
                {
                    // Release the resources.
                    if (lobjRange.IsNotNull())
                        while (Marshal.ReleaseComObject(lobjRange) > 0)

                            if (lobjSheet.IsNotNull())
                                while (Marshal.ReleaseComObject(lobjSheet) > 0) ;

                    if (lobjWorkbook.IsNotNull())
                    {
                        while (Marshal.ReleaseComObject(lobjWorkbook) > 0) ;
                    }

                    if (oXL.Workbooks.IsNotNull())
                    {
                        oXL.Workbooks.Close();
                        while (Marshal.ReleaseComObject(oXL.Workbooks) > 0) ;
                    }

                    if (oXL.IsNotNull())
                    {
                        oXL.Quit();
                        while (Marshal.ReleaseComObject(oXL) > 0) ;
                    }
                    lobjRange = null;
                    lobjSheet = null;
                    lobjWorkbook = null;
                    oXL = null;
                }
            }
        }

        public void ReadExcelReport(string astrFileName, string astrWorkSheetName, Collection<string> adsMain)
        {
            Excel.Application oXL = new Excel.Application();
            if (oXL.IsNotNull() && astrFileName.IsNotNull() && astrFileName.Length > 0)
            {
                // Donot display any alerts for user.
                oXL.DisplayAlerts = false;
                // Step1: Create a xls file using the xlt Template.astrFileName
                Excel.Workbook lobjTemplateWorkbook = null;
                object missing = Type.Missing;
                try
                {

                    lobjTemplateWorkbook = oXL.Workbooks.Open(astrFileName, 0, true, 5,
                              string.Empty, string.Empty, true, Excel.XlPlatform.xlWindows, busConstant.DELIMITERTAB, false, false,
                              0, true, 0, Excel.XlCorruptLoad.xlNormalLoad);
                    //oXL.Workbooks[1].SaveAs(astrFileName, missing, missing, missing, false, missing, Excel.XlSaveAsAccessMode.xlExclusive, missing, missing, missing, missing, missing);

                    //oXL.Workbooks[1].SaveAs(astrFileName, Excel.XlFileFormat.xlOpenXMLWorkbook, Missing.Value, Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                    //            Excel.XlSaveConflictResolution.xlUserResolution, true, Missing.Value, Missing.Value, Missing.Value);
                }
                finally
                {
                    // Release the resources.
                    if (oXL.Workbooks.IsNotNull())
                        oXL.Workbooks.Close();

                    if (lobjTemplateWorkbook.IsNotNull())
                        while (Marshal.ReleaseComObject(lobjTemplateWorkbook) > 0) ;

                    if (oXL.Workbooks.IsNotNull())
                        while (Marshal.ReleaseComObject(oXL.Workbooks) > 0) ;

                    lobjTemplateWorkbook = null;
                }

                Excel.Workbook lobjWorkbook = null;
                Excel.Worksheet lobjSheet = null;
                Excel.Range lobjRange = null;

                try
                {
                    // Step2: Open the created xls file
                    lobjWorkbook = oXL.Workbooks.Open(astrFileName, 0, false, 5,
                              string.Empty, string.Empty, true, Excel.XlPlatform.xlWindows, busConstant.DELIMITERTAB, true, false,
                              0, true, 0, Excel.XlCorruptLoad.xlNormalLoad);

                    lobjSheet = (Microsoft.Office.Interop.Excel.Worksheet)lobjWorkbook.Sheets[1];
                    lobjSheet.Name = astrWorkSheetName;

                    lobjSheet.Unprotect(System.Reflection.Missing.Value);

                    //if (astrTemplateFile.Contains(busConstant.PENSION_ACTUARY_REPORT))
                    //{
                    //    CreatePensionActuaryReport(adsMain, lobjSheet, ref lobjRange);
                    //}

                    string str;
                    int rCnt;
                    int cCnt;
                    int rw = 0;
                    int cl = 0;

                    //xlApp = new Excel.Application();
                    //xlWorkBook = xlApp.Workbooks.Open(@"d:\csharp-Excel.xls", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    lobjRange = lobjSheet.UsedRange;
                    rw = lobjRange.Rows.Count;
                    cl = lobjRange.Columns.Count;

                    for (rCnt = 1; rCnt <= rw; rCnt++)
                    {
                        for (cCnt = 1; cCnt <= 1; cCnt++)
                        {
                            if ((string)(lobjRange.Cells[rCnt, cCnt] as Excel.Range).Value2 != "MPIPERSONID")
                            {
                                adsMain.Add((string)(lobjRange.Cells[rCnt, cCnt] as Excel.Range).Value2);

                            }
                            
                           
                        }
                    }


                }
                finally
                {
                    // Release the resources.
                    if (lobjRange.IsNotNull())
                        while (Marshal.ReleaseComObject(lobjRange) > 0)

                            if (lobjSheet.IsNotNull())
                                while (Marshal.ReleaseComObject(lobjSheet) > 0) ;

                    if (lobjWorkbook.IsNotNull())
                    {
                        while (Marshal.ReleaseComObject(lobjWorkbook) > 0) ;
                    }

                    if (oXL.Workbooks.IsNotNull())
                    {
                        oXL.Workbooks.Close();
                        while (Marshal.ReleaseComObject(oXL.Workbooks) > 0) ;
                    }

                    if (oXL.IsNotNull())
                    {
                        oXL.Quit();
                        while (Marshal.ReleaseComObject(oXL) > 0) ;
                    }
                    lobjRange = null;
                    lobjSheet = null;
                    lobjWorkbook = null;
                    oXL = null;
                }
            }
        }


        private static void Create5500Report(DataSet adsMain, Excel._Worksheet lobjSheet5500_Counts, Excel._Worksheet lobjSSAReport050A, Excel._Worksheet lobjSSAReport050B, ref Excel.Range lobjRange)
        {
            DataTable adtCompYear;
            adtCompYear = Sagitec.BusinessObjects.busBase.Select("cdo5500Report.GetComputationYear", new object[0]);
            int lintComputationYear = adtCompYear.Rows.Count > 0 ? Convert.ToInt32(adtCompYear.Rows[0]["COMPUTATION_YEAR"].ToString()) : 0;            

            lobjRange = lobjSheet5500_Counts.get_Range("D1", "D1");
            object content1 = new object[1] { lintComputationYear };
            // hard code for testing only
            //object content1 = new object[1] { "2016" };
            lobjRange.set_Value(Missing.Value, content1);

            int lintCurrentRow = 3;
            
            if (adsMain.Tables["rpt5500Report"].Rows.Count > 0)
            {
                foreach (DataRow ldr5500Report in adsMain.Tables["rpt5500Report"].Rows)
                {
                    //5500 IAP Count Details (5500_Counts Worksheet)
                    lobjRange = lobjSheet5500_Counts.get_Range("D" + lintCurrentRow.ToString(), "E" + lintCurrentRow.ToString());
                    object content = new object[2] { ldr5500Report["IAP_COUNT"], ldr5500Report["PENSION_COUNT"] };
                    lobjRange.set_Value(Missing.Value, content);
                    ++lintCurrentRow;
                }
            }

            //SSAReport05A Worksheet Details
            lintCurrentRow = 5;
            foreach (DataRow ldrSSAReport05A in adsMain.Tables["SSAReport05A"].Rows)
            {
                lobjRange = lobjSSAReport050A.get_Range("A" + lintCurrentRow.ToString(), "I" + lintCurrentRow.ToString());
                object content = new object[9] {
                                                    ldrSSAReport05A["MPI_5500_STATUS_CODE"],
                                                    ldrSSAReport05A["SSN"],
                                                    ldrSSAReport05A["FIRST_NAME"],
                                                    ldrSSAReport05A["MIDDLE_NAME"],
                                                    ldrSSAReport05A["LAST_NAME"],
                                                    ldrSSAReport05A["TYPE_OF_ANNUITY"],
                                                    ldrSSAReport05A["PAYMENT_FREQUENCY"],
                                                    ldrSSAReport05A["DB_PAYMENT"],
                                                    //ldrSSAReport05A["UNITS_SHARES"],
                                                    ldrSSAReport05A["TOTAL_VALUE_ACCOUNT"],
                                                    //ldrSSAReport05A["PLAN_YEAR_SEPERATED"],
                                                    //ldrSSAReport05A["VESTED"]
                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }

            //SSAReport05B Worksheet Details
            lintCurrentRow = 5;
            foreach (DataRow ldrSSAReport05B in adsMain.Tables["SSAReport05B"].Rows)
            {
                lobjRange = lobjSSAReport050B.get_Range("A" + lintCurrentRow.ToString(), "I" + lintCurrentRow.ToString());
                object content = new object[9] {
                                                    ldrSSAReport05B["IAP_5500_STATUS_CODE"],
                                                    ldrSSAReport05B["SSN"],
                                                    ldrSSAReport05B["FIRST_NAME"],
                                                    ldrSSAReport05B["MIDDLE_NAME"],
                                                    ldrSSAReport05B["LAST_NAME"],
                                                    ldrSSAReport05B["TYPE_OF_ANNUITY"],
                                                    ldrSSAReport05B["PAYMENT_FREQUENCY"],
                                                    ldrSSAReport05B["DB_PAYMENT"],
                                                    //ldrSSAReport05B["UNITS_SHARES"],
                                                    ldrSSAReport05B["TOTAL_VALUE_ACCOUNT"],
                                                    //ldrSSAReport05B["PLAN_YEAR_SEPERATED"],
                                                    //ldrSSAReport05B["VESTED"]
                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }



        }
        private static void CreatePensionActuaryReport(DataSet adsMain, Excel.Worksheet lobjSheet, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;
            //int lintStartIndex = 1;

            lobjSheet.Cells[lintCurrentRow + 0, 3] = adsMain.Tables["ReportTable01"].Rows[0]["Year"].ToString(); //(DateTime.Now.Year - 1).ToString();

            lintCurrentRow = lintCurrentRow + 3;
            foreach (DataRow ldrReportTable in adsMain.Tables["ReportTable01"].Rows)
            {
                lobjRange = lobjSheet.get_Range("A" + lintCurrentRow.ToString(), "AO" + lintCurrentRow.ToString()); //RID 125383 Remove 2 column change range AQ to AO
                //changed array size from 43 to 41
                object content = new object[41] {
                                                 //ldrReportTable["PERSON_NAME"],
                                                 ldrReportTable["PERSON_SSN"],
                                                 ldrReportTable["PERSON_GENDER_VALUE"],
                                                 ldrReportTable["istrPersonDateOfBirth"],
                                                 ldrReportTable["BENEFICIARY_FLAG"],
                                                 //ldrReportTable["BENEFICIARY_NAME"],
                                                 ldrReportTable["BENEFICIARY_SSN"],
                                                 ldrReportTable["BENEFICIARY_GENDER_VALUE"],
                                                 ldrReportTable["BENEFICIARY_DOB"],
                                                 ldrReportTable["istrUnionCode"],
                                                 ldrReportTable["STATUS_CODE_VALUE"],
                                                 ldrReportTable["PARTICIPANT_DATE_OF_DEATH"],
                                                 ldrReportTable["istrPlan"],

                                                 ldrReportTable["TOTAL_QF_YR_END_OF_LAST_COMP_YEAR"],
                                                 ldrReportTable["PARTICIPANT_STATE_VALUE"],
                                                 ldrReportTable["Last_QF_YR_BEFORE_BIS"],

                                                 ldrReportTable["NON_ELIGIBLE_BENEFIT"],
                                                 ldrReportTable["ACCRUED_BENEFIT_FOR_PRIOR_YEAR"],

                                                 ldrReportTable["ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR"],
                                                 ldrReportTable["TOTAL_EE_CONTRIBUTION_AMT"],
                                                 ldrReportTable["TOTAL_UVHP_AMT"],

                                                 ldrReportTable["YTD_HOURS_FOR_LAST_COMP_YEAR"],
                                                 ldrReportTable["TOTAL_HOURS"],
                                                 ldrReportTable["YTD_HOURS_FOR_YEAR_BEFORE_LAST_COMP_YEAR"],
                                                 ldrReportTable["TOTAL_EE_INTEREST_AMT"],
                                                 ldrReportTable["TOTAL_UVHP_INTEREST_AMT"],
                                                 ldrReportTable["MONTHLY_BENEFIT_AMT"],

                                                 ldrReportTable["MG_AMT_LEFT"],
                                                 ldrReportTable["BENEFIT_OPTION_CODE_VALUE"],
                                                 ldrReportTable["RETURN_TO_WORK_FLAG"],
                                                 ldrReportTable["DETERMINATION_DATE"],
                                                 ldrReportTable["BENEFICIARY_FIRST_PAYMENT_RECEIVE_DATE"],
                                                 ldrReportTable["PENSION_STOP_DATE"],
                                                 ldrReportTable["TOTAL_QUALIFIED_YEARS_AT_RET"],
                                                 ldrReportTable["TOTAL_QUALIFIED_HOURS_AT_RET"],
                                                 ldrReportTable["BENEFICIARY_DATE_OF_DEATH"],
                                                 ldrReportTable["LUMP_AMT_TAKEN_IN_LAST_COMP_YR"],
                                                 ldrReportTable["RETIREMENT_TYPE_VALUE"],
                                                 ldrReportTable["LIFE_ANNUITY_AMT"],
                                                 ldrReportTable["MD_FLAG"],
                                                 //RID 71411
                                                 ldrReportTable["IS_DISABILITY_CONVERSION"],
                                                 ldrReportTable["IS_CONVERTED_FROM_POPUP"],
                                                 ldrReportTable["DRO_MODEL"]
                                             };

                lobjRange.set_Value(Missing.Value, content);

                ++lintCurrentRow;
            }

        }

        private static void CreateAnnualStatementParticipantDetails(DataSet adsMain, Excel.Worksheet lobjSheet, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;
            int lintStartIndex = 1;

            lobjSheet.Cells[lintCurrentRow + 2, 1] = (DateTime.Now.Year - 1).ToString();
            lintCurrentRow = lintCurrentRow + 4;
            foreach (DataRow ldrReportTable in adsMain.Tables["AnnualStatementParticipantsDetails"].Rows)
            {
                lobjRange = lobjSheet.get_Range("A" + lintCurrentRow.ToString(), "C" + lintCurrentRow.ToString());

                object content = new object[3] {
                                                 ldrReportTable["MPI_PERSON_ID"],
                                                 ldrReportTable["FIRST_NAME"],
                                                 ldrReportTable["LAST_NAME"]
                                               };
                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }

        private static void CreateIapAdjustmentPayment(DataSet adsMain, Excel.Worksheet lobjSheet, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;

            lobjSheet.Cells[lintCurrentRow + 1, 2] = (DateTime.Today.Date).ToString("d");
            lintCurrentRow = lintCurrentRow + 4;
            foreach (DataRow ldrReportTable in adsMain.Tables["IapAdjustmentPayment"].Rows)
            {
                lobjRange = lobjSheet.get_Range("A" + lintCurrentRow.ToString(), "M" + lintCurrentRow.ToString());

                object content = new object[13] {
                                                 ldrReportTable["PAYEE_MPID"],
                                                 ldrReportTable["PARTICIPANT_MPID"],
                                                 ldrReportTable["PARTICIPANT_NAME"],
                                                 ldrReportTable["DATE_OF_DEATH"],
                                                 ldrReportTable["PAYEE_ACCOUNT_ID"],
                                                 ldrReportTable["BENEFIT_TYPE"],
                                                 ldrReportTable["RETIREMENT_DATE"],
                                                 ldrReportTable["WITHDRAWAL_DATE"],
                                                 ldrReportTable["AWARDED_ON_DATE"],
                                                 ldrReportTable["DRO_COMMENCEMENT_DATE"],
                                                 ldrReportTable["PAYMENT_DATE"],
                                                 ldrReportTable["FUND_TYPE"],
                                                 ldrReportTable["RO"]
                                               };
                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }

        private static void CreateAnnual1099RDetails(DataSet adsMain, Excel.Worksheet lobjSheet, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;

            lobjSheet.Cells[lintCurrentRow + 2, 1] = (DateTime.Now.Year - 1).ToString();
            lintCurrentRow = lintCurrentRow + 4;
            foreach (DataRow ldrReportTable in adsMain.Tables["Annual1099RDetails"].Rows)
            {
                lobjRange = lobjSheet.get_Range("A" + lintCurrentRow.ToString(), "AG" + lintCurrentRow.ToString());

                object content = new object[33] {
                                                 ldrReportTable["NAME"],
                                                 ldrReportTable["RECIPIENT_NAME"],
                                                 ldrReportTable["ADDR_LINE_1"],
                                                 ldrReportTable["ADDR_LINE_2"],
                                                 ldrReportTable["ADDR_CITY"],
                                                 ldrReportTable["ADDR_STATE_VALUE"],
                                                 ldrReportTable["ADDR_ZIP_CODE"],
                                                 ldrReportTable["ADDR_LINE_3"],
                                                 ldrReportTable["FEDERAL_ID"],
                                                 ldrReportTable["addr_state"],
                                                 ldrReportTable["GROSS_BENEFIT_AMOUNT"],
                                                 ldrReportTable["TAXABLE_AMOUNT"],
                                                 ldrReportTable["FED_TAX_AMOUNT"],
                                                 ldrReportTable["NON_TAXABLE_AMOUNT"],
                                                 ldrReportTable["DISTRIBUTION_CODE"],
                                                 ldrReportTable["STATE_TAX_AMOUNT"],
                                                 ldrReportTable["PAYER_STATE_NO"],
                                                 ldrReportTable["PAYEE_ACCOUNT_ID"],
                                                 ldrReportTable["RECIPIENTS_ID"],
                                                 ldrReportTable["PAYEE_NAME"],
                                                 ldrReportTable["PAYEE_ADDR_LINE_1"],
                                                 ldrReportTable["PAYEE_ADDR_LINE_2"],
                                                 ldrReportTable["PAYEE_ADDR_CITY"],
                                                 ldrReportTable["PAYEE_ADDR_STATE_VALUE"],
                                                 ldrReportTable["PAYEE_ADDR_ZIP_CODE"],
                                                 ldrReportTable["PAYEE_ADDR_LINE_3"],
                                                 ldrReportTable["age59_split_flag"],
                                                 ldrReportTable["CORRECTED_FLAG"],
                                                 ldrReportTable["AGE59_SPLIT_FLAG"],
                                                 ldrReportTable["TAX_YEAR"],
                                                 ldrReportTable["FOREIGN_PROVINCE"],
                                                 ldrReportTable["MPI_PERSON_ID"],
                                                 ldrReportTable["ADDR_LENGTH"]
                                               };
                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }

        /*  private static void CreateAnnual1099RReport(DataSet adsMain, Excel.Worksheet lobjSheet, ref Excel.Range lobjRange)
          {
              int lintCurrentRow = 2;
              int lintStartIndex = 2;

              foreach (DataRow ldrActuaryRetiree in adsMain.Tables["Annual1099RTable"].Rows)
              {
                  lobjRange = lobjSheet.get_Range("A" + lintCurrentRow.ToString(), "L" + lintCurrentRow.ToString());

                  object content = new object[12] {ldrActuaryRetiree["Payee_Account_Id"],
                                                   ldrActuaryRetiree["Recipient_SSN"],
                                                   ldrActuaryRetiree["Recipient_Name"],
                                                   ldrActuaryRetiree["Recipient_Address"],
                                                   ldrActuaryRetiree["Total_Gross_Amount"],
                                                   ldrActuaryRetiree["Taxable_Amount"],
                                                   ldrActuaryRetiree["Federal_Tax_Withheld"],
                                                   ldrActuaryRetiree["Employee_Contribution_Amt"],
                                                   ldrActuaryRetiree["Exclusion_Balance"],
                                                   ldrActuaryRetiree["Distribution_Code"],
                                                   ldrActuaryRetiree["State_Tax_Withheld"],
                                                   ldrActuaryRetiree["Gross_for_State_Tax"]
                          };

                  lobjRange.set_Value(Missing.Value, content);

                  ++lintCurrentRow;
              }

              if (adsMain.Tables["Annual1099RTable"].Rows.Count > 0)
              {
                  int lintLastRow = lintCurrentRow - 1;

                  lobjSheet.Cells[lintCurrentRow + 2, 1] = "GRAND TOTALS";

                  lintCurrentRow = lintCurrentRow + 4;

                  lobjSheet.Cells[lintCurrentRow, 1] = "Total Gross: ";
                  lobjSheet.Cells[lintCurrentRow, 3] = "=SUM(E" + lintStartIndex + ":E" + lintLastRow + ")";
                  lobjSheet.Cells[lintCurrentRow + 1, 1] = "Total Taxable : ";
                  lobjSheet.Cells[lintCurrentRow + 1, 3] = "=SUM(F" + lintStartIndex + ":F" + lintLastRow + ")";
                  lobjSheet.Cells[lintCurrentRow + 2, 1] = "Total Fed Tax : ";
                  lobjSheet.Cells[lintCurrentRow + 2, 3] = "=SUM(G" + lintStartIndex + ":G" + lintLastRow + ")";
                  lobjSheet.Cells[lintCurrentRow + 3, 1] = "Total EE Contribution : ";
                  lobjSheet.Cells[lintCurrentRow + 3, 3] = "=SUM(H" + lintStartIndex + ":H" + lintLastRow + ")";
                  lobjSheet.Cells[lintCurrentRow + 4, 1] = "Total Exclusion Balance : ";
                  lobjSheet.Cells[lintCurrentRow + 4, 3] = "=SUM(I" + lintStartIndex + ":I" + lintLastRow + ")";
                  lobjSheet.Cells[lintCurrentRow + 5, 1] = "Total State Tax : ";
                  lobjSheet.Cells[lintCurrentRow + 5, 3] = "=SUM(K" + lintStartIndex + ":K" + lintLastRow + ")";
                  lobjSheet.Cells[lintCurrentRow + 6, 1] = "Total Gross For State Tax : ";
                  lobjSheet.Cells[lintCurrentRow + 6, 3] = "=SUM(L" + lintStartIndex + ":L" + lintLastRow + ")";

                  Excel.Range chartRange = lobjSheet.get_Range("A" + (lintCurrentRow - 2).ToString(), "A" + (lintCurrentRow + 6).ToString());
                  chartRange.Font.Bold = true;
                  chartRange = lobjSheet.get_Range("C" + (lintCurrentRow).ToString(), "C" + (lintCurrentRow + 6).ToString());
                  chartRange.Font.Bold = true;
                  chartRange.NumberFormat = "$#,##0.00_);($#,##0.00)";

              }
          }*/

        //RequestID: 68932
        private static void CreatePensionBenefitVerificationReport(DataSet adsMain, Excel._Worksheet lobjNinetyDaysLetters, Excel._Worksheet lobjSixtyDaysLetters, Excel._Worksheet lobjThirtyDaysLetters, Excel._Worksheet lobjSuspensionLetters, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;
            int lintStartIndex = 1;

            if (adsMain.Tables.Count == 3)
            {
                //NinetyDays Letters Worksheet Details
                lintCurrentRow = lintCurrentRow + 1;
                foreach (DataRow ldrNinetyDaysLetter in adsMain.Tables["NINETYDAYSLETTERS"].Rows)
                {
                    lobjRange = lobjNinetyDaysLetters.get_Range("A" + lintCurrentRow.ToString(), "F" + lintCurrentRow.ToString());
                    object content = new object[6] {
                                                    ldrNinetyDaysLetter["MPI_PERSON_ID"],
                                                    ldrNinetyDaysLetter["FIRST_NAME"],
                                                    ldrNinetyDaysLetter["LAST_NAME"],
                                                    ldrNinetyDaysLetter["BENEFIT_DATE"],
                                                    ldrNinetyDaysLetter["BENEFIT_ACCOUNT_TYPE"],
                                                    ldrNinetyDaysLetter["VALID_ADDRESS"]
                                                };

                    lobjRange.set_Value(Missing.Value, content);
                    ++lintCurrentRow;
                }

                //SixtyDays Letters Worksheet Details
                lintCurrentRow = 2;
                foreach (DataRow ldrSixtyDaysLetter in adsMain.Tables["SIXTYDAYSLETTERS"].Rows)
                {
                    lobjRange = lobjSixtyDaysLetters.get_Range("A" + lintCurrentRow.ToString(), "F" + lintCurrentRow.ToString());
                    object content = new object[6] {
                                                    ldrSixtyDaysLetter["MPI_PERSON_ID"],
                                                    ldrSixtyDaysLetter["FIRST_NAME"],
                                                    ldrSixtyDaysLetter["LAST_NAME"],
                                                    ldrSixtyDaysLetter["BENEFIT_DATE"],
                                                    ldrSixtyDaysLetter["BENEFIT_ACCOUNT_TYPE"],
                                                    ldrSixtyDaysLetter["VALID_ADDRESS"]
                                                };

                    lobjRange.set_Value(Missing.Value, content);
                    ++lintCurrentRow;
                }
                //ThirtyDays Letters Worksheet Details
                lintCurrentRow = 2;
                foreach (DataRow ldrThirtyDaysLetter in adsMain.Tables["THIRTYDAYSLETTERS"].Rows)
                {
                    lobjRange = lobjThirtyDaysLetters.get_Range("A" + lintCurrentRow.ToString(), "Q" + lintCurrentRow.ToString());
                    object content = new object[17] {
                                                    ldrThirtyDaysLetter["MPI_PERSON_ID"],
                                                    ldrThirtyDaysLetter["FIRST_NAME"],
                                                    ldrThirtyDaysLetter["LAST_NAME"],
                                                    ldrThirtyDaysLetter["ADDRESS1"],
                                                    ldrThirtyDaysLetter["ADDRESS2"],
                                                    ldrThirtyDaysLetter["CITY"],
                                                    ldrThirtyDaysLetter["STATE"],
                                                    ldrThirtyDaysLetter["ZIPCODE"],
                                                    ldrThirtyDaysLetter["COUNTRY"],
                                                    ldrThirtyDaysLetter["BENEFIT_DATE"],
                                                    ldrThirtyDaysLetter["BENEFIT_ACCOUNT_TYPE"],
                                                    ldrThirtyDaysLetter["VALID_ADDRESS"],
                                                    ldrThirtyDaysLetter["HOME_PHONE"],
                                                    ldrThirtyDaysLetter["MOBILE_PHONE"],
                                                    ldrThirtyDaysLetter["WORK_PHONE"],
                                                    ldrThirtyDaysLetter["EMAIL_ADDRESS"],
                                                    ldrThirtyDaysLetter["UNION_CODE"],
                                                };

                    lobjRange.set_Value(Missing.Value, content);
                    ++lintCurrentRow;
                }
            }

            if (adsMain.Tables.Count == 1 && adsMain.Tables["SUSPENSIONLETTERS"].Rows.Count >0)
            {
                //Suspension Letters Worksheet Details
                lintCurrentRow = 2;
                foreach (DataRow ldrSuspensionLetter in adsMain.Tables["SUSPENSIONLETTERS"].Rows)
                {
                    lobjRange = lobjSuspensionLetters.get_Range("A" + lintCurrentRow.ToString(), "D" + lintCurrentRow.ToString());
                    object content = new object[4] {
                                                    ldrSuspensionLetter["MPI_PERSON_ID"],
                                                    ldrSuspensionLetter["FIRST_NAME"],
                                                    ldrSuspensionLetter["LAST_NAME"],
                                                    ldrSuspensionLetter["PAYEE_ACCOUNT_ID"]
                                                };

                    lobjRange.set_Value(Missing.Value, content);
                    ++lintCurrentRow;
                }
            }
        }



        #endregion
        //Ticket#76267
        private static void CreateRetireeHealthEligibleReport(DataSet adsMain, Excel._Worksheet lobjRetireeHealthEligible, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;
            int lintStartIndex = 1;

            //NinetyDays Letters Worksheet Details
            lintCurrentRow = lintCurrentRow + 1;
            foreach (DataRow ldrRetireeHealthEligible in adsMain.Tables["ReportTable01"].Rows)
            {
                lobjRange = lobjRetireeHealthEligible.get_Range("A" + lintCurrentRow.ToString(), "P" + lintCurrentRow.ToString());
                object content = new object[16] {
                                                     ldrRetireeHealthEligible["LASTNAME"],
                                                     ldrRetireeHealthEligible["FIRSTNAME"],
                                                     ldrRetireeHealthEligible["MPIPERSONID"],
                                                     ldrRetireeHealthEligible["Address1"],
                                                     ldrRetireeHealthEligible["Address2"],
                                                     ldrRetireeHealthEligible["City"],
                                                     ldrRetireeHealthEligible["State"],
                                                     ldrRetireeHealthEligible["ZipCode"],
                                                     ldrRetireeHealthEligible["DateofBirth"],
                                                     ldrRetireeHealthEligible["RetirementDate"],
                                                     ldrRetireeHealthEligible["AgeatRetirement"],
                                                     ldrRetireeHealthEligible["QualifiedYears"],
                                                     ldrRetireeHealthEligible["HealthHours"],
                                                     ldrRetireeHealthEligible["EligibilityBasedOn"],
                                                     ldrRetireeHealthEligible["SSADate"],
                                                     ldrRetireeHealthEligible["EffectiveDate"]
                                                    
        
                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }

        private static void CreateRetireeHealthEligibleReport30Day(DataSet adsMain, Excel._Worksheet lobjRetireeHealthEligible30Day, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;
            //int lintStartIndex = 1;

            //NinetyDays Letters Worksheet Details
            lintCurrentRow = lintCurrentRow + 1;
            foreach (DataRow ldrRetireeHealthEligible in adsMain.Tables["ReportTable01"].Rows)
            {
                lobjRange = lobjRetireeHealthEligible30Day.get_Range("A" + lintCurrentRow.ToString(), "R" + lintCurrentRow.ToString());
                object content = new object[18] {
                                                     ldrRetireeHealthEligible["LASTNAME"],
                                                     ldrRetireeHealthEligible["FIRSTNAME"],
                                                     ldrRetireeHealthEligible["MPIPERSONID"],
                                                     ldrRetireeHealthEligible["Address1"],
                                                     ldrRetireeHealthEligible["Address2"],
                                                     ldrRetireeHealthEligible["City"],
                                                     ldrRetireeHealthEligible["State"],
                                                     ldrRetireeHealthEligible["ZipCode"],
                                                     ldrRetireeHealthEligible["DateofBirth"],
                                                     ldrRetireeHealthEligible["RetirementDate"],
                                                     ldrRetireeHealthEligible["AgeatRetirement"],
                                                     ldrRetireeHealthEligible["QualifiedYears"],
                                                     ldrRetireeHealthEligible["HealthHours"],
                                                     ldrRetireeHealthEligible["EligibilityBasedOn"],
                                                     ldrRetireeHealthEligible["SSADate"],
                                                     ldrRetireeHealthEligible["EffectiveDate"],
                                                     ldrRetireeHealthEligible["LateAdditionDate"],
                                                     ldrRetireeHealthEligible["LateCancellationDate"]


                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }

        private static void CreateAnnualRecalculateTaxesReport(DataSet adsMain, Excel._Worksheet lobjAnnualRecalculateTaxes, ref Excel.Range lobjRange)
        {

           
            int lintCurrentRow = 1;
            //int lintStartIndex = 1;

            //NinetyDays Letters Worksheet Details
            lintCurrentRow = lintCurrentRow + 1;
            foreach (DataRow ldrAnnualRecalculateTaxes in adsMain.Tables["ReportTable01"].Rows)
            {
                lobjRange = lobjAnnualRecalculateTaxes.get_Range("A" + lintCurrentRow.ToString(), "K" + lintCurrentRow.ToString());
                object content = new object[11] {
                                                     ldrAnnualRecalculateTaxes["PERSONID"],
                                                     ldrAnnualRecalculateTaxes["PayeeAccountID"],
                                                     ldrAnnualRecalculateTaxes["NextGrossPayment"],
                                                     ldrAnnualRecalculateTaxes["OldFederal"],
                                                     ldrAnnualRecalculateTaxes["NewFederal"],
                                                     ldrAnnualRecalculateTaxes["DifferenceFed"],
                                                     ldrAnnualRecalculateTaxes["AdditionalFed"],
                                                     ldrAnnualRecalculateTaxes["OldState"],
                                                     ldrAnnualRecalculateTaxes["NewState"],
                                                     ldrAnnualRecalculateTaxes["DifferenceState"],
                                                     ldrAnnualRecalculateTaxes["AdditionalState"]
                                                    


                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }
        private static void CreateUVHPEEContributionRefundReport(DataSet adsMain, Excel._Worksheet lobjUVHPEEContributionRefund, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;
            //int lintStartIndex = 1;

            //NinetyDays Letters Worksheet Details
            lintCurrentRow = lintCurrentRow + 1;
            foreach (DataRow ldrUVHPEEContributionRefund in adsMain.Tables["ReportTable01"].Rows)
            {
                lobjRange = lobjUVHPEEContributionRefund.get_Range("A" + lintCurrentRow.ToString(), "S" + lintCurrentRow.ToString());
                object content = new object[19] {
                                                     ldrUVHPEEContributionRefund["MPID"],
                                                     ldrUVHPEEContributionRefund["DOB"],
                                                     ldrUVHPEEContributionRefund["SSN"],
                                                     ldrUVHPEEContributionRefund["FIRSTNAME"],
                                                     ldrUVHPEEContributionRefund["MI"],
                                                     ldrUVHPEEContributionRefund["LASTNAME"],
                                                     ldrUVHPEEContributionRefund["ValidAddress"],
                                                     ldrUVHPEEContributionRefund["Address1"],
                                                     ldrUVHPEEContributionRefund["Address2"],
                                                     ldrUVHPEEContributionRefund["City"],
                                                     ldrUVHPEEContributionRefund["State"],
                                                     ldrUVHPEEContributionRefund["ZipCode"],
                                                     ldrUVHPEEContributionRefund["FOREIGN_PROVINCE"],
                                                     ldrUVHPEEContributionRefund["COUNTRY"],
                                                     ldrUVHPEEContributionRefund["CALCID"],
                                                     ldrUVHPEEContributionRefund["UVHP"],
                                                     ldrUVHPEEContributionRefund["UVHPInt"],
                                                     ldrUVHPEEContributionRefund["EE"],
                                                     ldrUVHPEEContributionRefund["EEInt"]
                                                   


                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }

        //rid 76227
        private static void CreatePensionEligibilityBatchReport(DataSet adsMain, Excel._Worksheet lobjSheetPensionEligibility, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;
            
            lintCurrentRow = lintCurrentRow + 1;
            foreach (DataRow ldrPensionEligibility in adsMain.Tables["ReportTable01"].Rows)
            {
                lobjRange = lobjSheetPensionEligibility.get_Range("A" + lintCurrentRow.ToString(), "T" + lintCurrentRow.ToString());
                object content = new object[20] {
                                                     ldrPensionEligibility["MPI_PERSON_ID"],
                                                     ldrPensionEligibility["NAME"],
                                                     ldrPensionEligibility["ADDRESS1"], //rid 124432
                                                     ldrPensionEligibility["ADDRESS2"], //rid 124432
                                                     ldrPensionEligibility["CITY"], //rid 124432
                                                     ldrPensionEligibility["STATE"], //rid 124432
                                                     ldrPensionEligibility["ZIPCODE"], //rid 124432
                                                     ldrPensionEligibility["EMAIL_ADDRESS"], //rid 124432
                                                     ldrPensionEligibility["DATE_of_Birth"],
                                                     ldrPensionEligibility["Age"],
                                                     ldrPensionEligibility["RETIREMENT_DATE"],
                                                     ldrPensionEligibility["MPI_Accrued_Benefit"],
                                                     ldrPensionEligibility["IAP_Balance"],
                                                     ldrPensionEligibility["EE_UVHP"],
                                                     ldrPensionEligibility["QY_Total"],
                                                     ldrPensionEligibility["Vested_Date"],
                                                     ldrPensionEligibility["Vested_Status"],
                                                     ldrPensionEligibility["BIS"],
                                                     ldrPensionEligibility["Local_Plan"],
                                                     ldrPensionEligibility["BAD_ADDRESS_FLAG"]
                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }

        //Ticket 79238
        private static void CreateMdAddressBatchReport(DataSet adsMain, Excel._Worksheet lobjSheetMdAddresses, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;

            lintCurrentRow = lintCurrentRow + 1;
            foreach (DataRow ldrPensionEligibility in adsMain.Tables["ReportTable01"].Rows)
            {
                lobjRange = lobjSheetMdAddresses.get_Range("A" + lintCurrentRow.ToString(), "H" + lintCurrentRow.ToString());
                object content = new object[8] {
                                                     ldrPensionEligibility["MPI_PERSON_ID"],
                                                     ldrPensionEligibility["LAST_NAME"],
                                                     ldrPensionEligibility["FIRST_NAME"],
                                                    ldrPensionEligibility["ADDR_LINE_1"],
                                                    ldrPensionEligibility["ADDR_LINE_2"],
                                                    ldrPensionEligibility["ADDR_CITY"],
                                                    ldrPensionEligibility["ADDR_STATE_VALUE"],
                                                    ldrPensionEligibility["ADDR_ZIP_CODE"]
                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }
        private static void CreateIAPRequiredMinimumDistributionReport(DataSet adsMain, Excel._Worksheet lobjIAPRequiredMinimumDistribution, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;
            int lintStartIndex = 1;

            //NinetyDays Letters Worksheet Details
            lintCurrentRow = lintCurrentRow + 1;
            foreach (DataRow ldrIAPRequiredMinimumDistribution in adsMain.Tables["ReportTable01"].Rows)
            {
                lobjRange = lobjIAPRequiredMinimumDistribution.get_Range("A" + lintCurrentRow.ToString(), "S" + lintCurrentRow.ToString());
                object content = new object[19] {
                                                    ldrIAPRequiredMinimumDistribution["MPI_PERSON_ID"],
                                                    ldrIAPRequiredMinimumDistribution["LAST_NAME"],
                                                    ldrIAPRequiredMinimumDistribution["FIRST_NAME"],
                                                    ldrIAPRequiredMinimumDistribution["DATE_OF_BIRTH"],
                                                    ldrIAPRequiredMinimumDistribution["AGE"],
                                                    ldrIAPRequiredMinimumDistribution["QYCOUNT"],
                                                    ldrIAPRequiredMinimumDistribution["PLAN_NAME"],
                                                    ldrIAPRequiredMinimumDistribution["BREAK_YEARS"],
                                                    ldrIAPRequiredMinimumDistribution["IAP_BALANCE"],
                                                    ldrIAPRequiredMinimumDistribution["IAP_VESTING_DATE"],
                                                    ldrIAPRequiredMinimumDistribution["STATUS_VALUE"],
                                                    ldrIAPRequiredMinimumDistribution["MD_AGE"],
                                                    ldrIAPRequiredMinimumDistribution["MD_DATE"],
                                                    ldrIAPRequiredMinimumDistribution["ADDRESS1"],
                                                    ldrIAPRequiredMinimumDistribution["ADDRESS2"],
                                                    ldrIAPRequiredMinimumDistribution["CITY"],
                                                    ldrIAPRequiredMinimumDistribution["STATE"],
                                                    ldrIAPRequiredMinimumDistribution["ZIP"],
                                                    ldrIAPRequiredMinimumDistribution["COUNTRY"]



                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }

        private static void CreateLastOneYearDeathNotificationReport(DataSet adsMain, Excel._Worksheet lobjSheetDeathNotification, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;

            lintCurrentRow = lintCurrentRow + 1;
            foreach (DataRow ldrDeathNotification in adsMain.Tables["ReportTable01"].Rows)
            {
                lobjRange = lobjSheetDeathNotification.get_Range("A" + lintCurrentRow.ToString(), "F" + lintCurrentRow.ToString());
                object content = new object[6] {
                                                    ldrDeathNotification["MPI_PERSON_ID"],
                                                    ldrDeathNotification["SSN"],
                                                    ldrDeathNotification["FIRST_NAME"],
                                                    ldrDeathNotification["LAST_NAME"],
                                                    ldrDeathNotification["DATE_OF_DEATH"],
                                                    ldrDeathNotification["DEATH_NOTIFICATION_RECEIVED_DATE"]
                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }

        private static void CreateVendorPaymentSummaryReport(DataSet adsMain, Excel._Worksheet lobjVendorPaymentSummary, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;

            lobjVendorPaymentSummary.Cells[lintCurrentRow + 1, 2] = (DateTime.Today.Date).ToString("d");
            lintCurrentRow = lintCurrentRow + 4;
            foreach (DataRow ldrVendorPaymentSummary in adsMain.Tables["ReportTable01"].Rows)
            {
                lobjRange = lobjVendorPaymentSummary.get_Range("A" + lintCurrentRow.ToString(), "F" + lintCurrentRow.ToString());
                object content = new object[6] {
                                                    ldrVendorPaymentSummary["ORGANIZATION_NAME"],
                                                    ldrVendorPaymentSummary["PLAN_NAME"],
                                                    ldrVendorPaymentSummary["CHECK_AMOUNT"],
                                                    ldrVendorPaymentSummary["ACH_AMOUNT"],
                                                    ldrVendorPaymentSummary["NET_AMOUNT"],
                                                    ldrVendorPaymentSummary["PAYMENT_DATE"]
                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }
        private static void CreatePaymentEDDFileReport(DataSet adsMain, Excel.Sheets lobjSheets, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;
            int excelSheetIndex = 1;
            foreach (DataTable dtPaymentEDDFileData in adsMain.Tables)
            {
                lintCurrentRow = 2;
                Excel._Worksheet lobjSheet = (Microsoft.Office.Interop.Excel.Worksheet)lobjSheets[excelSheetIndex];
                foreach (DataRow item in dtPaymentEDDFileData.Rows)
                {
                    lobjRange = lobjSheet.get_Range("A" + lintCurrentRow.ToString(), "G" + lintCurrentRow.ToString());
                    object content = new object[7] {
                                                    item["SSN"],
                                                    item["LAST_NAME"],
                                                    item["FIRST_NAME"],
                                                    item["MIDDLE_NAME"],
                                                    item["STATE_CODE"],
                                                    item["GROSS_AMOUNT"],
                                                    item["STATE_TAX_AMOUNT"]
                                                };

                    lobjRange.set_Value(Missing.Value, content);
                    ++lintCurrentRow;
                }
                ++excelSheetIndex;
            }
        }
        private static void CreateOregonPaymentEDDFileReport(DataSet adsMain, Excel._Worksheet lobjEmployeeDetailReport, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;

            lintCurrentRow = lintCurrentRow + 1;
            foreach (DataRow item in adsMain.Tables[0].Rows)
            {
                string strMiddleInitial = Convert.ToString(item["MIDDLE_NAME"]).Length > 0 ? Convert.ToString(item["MIDDLE_NAME"]).Substring(0, 1) : "";
                lobjRange = lobjEmployeeDetailReport.get_Range("A" + lintCurrentRow.ToString(), "J" + lintCurrentRow.ToString());
                object content = new object[10] {
                                                    item["SSN"], //SSN
                                                    item["FIRST_NAME"], //First Name
                                                    strMiddleInitial, //Middle Initial
                                                    item["LAST_NAME"], //Last Name
                                                    0, //Hours Worked
                                                    item["STATE_TAX_AMOUNT"], //State Income Tax Withholding
                                                    0, //STT Subject Wages
                                                    0, //STT Withholding
                                                    0, //Total UI Subject Wages
                                                    0 //Paid Leave Subject Wages
                                                };

                lobjRange.set_Value(Missing.Value, content);
                ++lintCurrentRow;
            }
        }

        private static void CreateStatementParticipantsWithStatementFlagReport(DataSet adsMain, Excel.Sheets lobjSheets, ref Excel.Range lobjRange)
        {
            int lintCurrentRow = 1;
            int excelSheetIndex = 1;
            foreach (DataTable dtPaymentEDDFileData in adsMain.Tables)
            {
                lintCurrentRow = 2;
                Excel._Worksheet lobjSheet = (Microsoft.Office.Interop.Excel.Worksheet)lobjSheets[excelSheetIndex];
                foreach (DataRow item in dtPaymentEDDFileData.Rows)
                {
                    lobjRange = lobjSheet.get_Range("A" + lintCurrentRow.ToString(), "F" + lintCurrentRow.ToString());
                    object content = new object[6] {
                                                    item["MPI_PERSON_ID"],
                                                    item["FIRST_NAME"],
                                                    item["LAST_NAME"],
                                                    item["REGISTERED_EMAIL_ADDRESS"],
                                                    item["IS_PAPER_STATEMENT"],
                                                    item["LAST_UPDATED"]
                                                };

                    lobjRange.set_Value(Missing.Value, content);
                    ++lintCurrentRow;
                }
                ++excelSheetIndex;
            }
        }
    }
}


