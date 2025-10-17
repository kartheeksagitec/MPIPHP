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
using System.Collections;
using Sagitec.DBUtility;
using System.Data.SqlClient;
using Sagitec.Common;

namespace MPIPHPJobService
{
    [Serializable]
    public class bus5500ReportBatch : busBatchHandler
    {
        #region Properties
        busBase lobjBase = new busBase();        
        
        #endregion

        public void Report5500Batch()
        {
            DataTable adt5500ReportData;
            DataTable adtReportData;
            DataTable adtReport05AData;
            DataTable adtSSAReport05AData;
            DataTable adtReport05BData;
            DataTable adtSSAReport05BData;

            DataTable adtCompYear;

            int lintrtn;
            cdo5500Report icdo5500Report = new cdo5500Report();
            cdoSsa5500ReportDetails icdoSSA5500ReportDetails = new cdoSsa5500ReportDetails();

            adtCompYear = busBase.Select("cdo5500Report.GetComputationYear", new object[0]);
            int lintComputationYear = adtCompYear.Rows.Count > 0 ? Convert.ToInt32(adtCompYear.Rows[0]["COMPUTATION_YEAR"].ToString()) : 0;

            adtReportData = busBase.Select("cdoDataExtractionBatchInfo.rpt5500Report", new object[1] { lintComputationYear });
            //hard code for testing only
            //adtReportData = busBase.Select("cdoDataExtractionBatchInfo.rpt5500Report", new object[1] { (2016) });
            adtReport05AData = busBase.Select("cdo5500Report.SSAReport05A", new object[0]);
            adtReport05BData = busBase.Select("cdo5500Report.SSAReport05B", new object[0]);

            adt5500ReportData = adtReportData.Copy();
            adtSSAReport05AData = adtReport05AData.Copy();
            adtSSAReport05BData = adtReport05BData.Copy();

            if (adt5500ReportData != null && adt5500ReportData.Rows.Count > 0)
            {

                string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_5500 + ".xlsx";
                string lstr5500ReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.REPORT_5500 + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

                DataSet lds5500ReportDataForExcel = new DataSet();
                lds5500ReportDataForExcel.Tables.Add(adt5500ReportData);

                if (adtSSAReport05AData != null && adtSSAReport05AData.Rows.Count > 0)
                    lds5500ReportDataForExcel.Tables.Add(adtSSAReport05AData);

                if (adtSSAReport05BData != null && adtSSAReport05BData.Rows.Count > 0)
                    lds5500ReportDataForExcel.Tables.Add(adtSSAReport05BData);

                busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstr5500ReportPath, "5500_Counts", lds5500ReportDataForExcel);
                

                lintrtn = DBFunction.DBNonQuery("cdo5500Report.DeleteIfExists",
                          new object[1] { lintComputationYear },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //5500 IAP Count Details
                if (adt5500ReportData != null && adt5500ReportData.Rows.Count > 0)
                {
                    foreach (DataRow ldr5500Report in adt5500ReportData.Rows)
                    {
                        string remarks = ldr5500Report["Remarks"].ToString();
                        switch (remarks)
                        {
                            case "Previous Year IAP count":
                                icdo5500Report.plan_id = 1;
                                icdo5500Report.computation_year = lintComputationYear;
                                break;
                            case "Subtotal 6d":
                                icdo5500Report.total_count = Convert.ToInt32(ldr5500Report["IAP_COUNT"].ToString());
                                break;
                            case "Active participants 6a(1)":
                                icdo5500Report.active_participants_6a1_count = Convert.ToInt32(ldr5500Report["IAP_COUNT"].ToString());
                                break;
                            case "Active participants 6a(2)":
                                icdo5500Report.active_participants_6a2_count = Convert.ToInt32(ldr5500Report["IAP_COUNT"].ToString());
                                break;
                            case "Retired participants 6b":
                                icdo5500Report.retired_seperated_ptp_count = Convert.ToInt32(ldr5500Report["IAP_COUNT"].ToString());
                                break;
                            case "Other retired or separated participants 6c":
                                icdo5500Report.other_retired_seperated_ptp_count = Convert.ToInt32(ldr5500Report["IAP_COUNT"].ToString());
                                break;
                            case "Deceased participants 6e":
                                icdo5500Report.deceased_ptp_count = Convert.ToInt32(ldr5500Report["IAP_COUNT"].ToString());
                                break;
                        }
                    }
                    icdo5500Report.employers_count = 0; //Actuary or Accounting will provide the detail.
                    icdo5500Report.Insert();
                }
                        

                //SSAReport05A details
                if (adtSSAReport05AData != null && adtSSAReport05AData.Rows.Count > 0)
                {
                    foreach (DataRow ldrSSAReport05A in adtSSAReport05AData.Rows)
                    {
                        icdoSSA5500ReportDetails.code = ldrSSAReport05A["MPI_5500_STATUS_CODE"].ToString();
                        icdoSSA5500ReportDetails.ssn = ldrSSAReport05A["SSN"].ToString();
                        //icdoSSA5500ReportDetails.name = ldrSSAReport05A["NAME"].ToString();
                        icdoSSA5500ReportDetails.first_name = ldrSSAReport05A["FIRST_NAME"].ToString();
                        icdoSSA5500ReportDetails.middle_name = ldrSSAReport05A["MIDDLE_NAME"].ToString();
                        icdoSSA5500ReportDetails.last_name = ldrSSAReport05A["LAST_NAME"].ToString();
                        icdoSSA5500ReportDetails.type_of_annuity = ldrSSAReport05A["TYPE_OF_ANNUITY"].ToString();
                        icdoSSA5500ReportDetails.payment_frequency = ldrSSAReport05A["PAYMENT_FREQUENCY"].ToString();
                        icdoSSA5500ReportDetails.gross_payment = Convert.ToDecimal(ldrSSAReport05A["DB_PAYMENT"]);
                        icdoSSA5500ReportDetails.units_shares = Convert.ToInt16(ldrSSAReport05A["UNITS_SHARES"]);
                        icdoSSA5500ReportDetails.total_value_account = Convert.ToDecimal(ldrSSAReport05A["TOTAL_VALUE_ACCOUNT"]);
                        icdoSSA5500ReportDetails.plan_year = Convert.ToInt16(ldrSSAReport05A["PLAN_YEAR_SEPERATED"]);
                        icdoSSA5500ReportDetails.vested = ldrSSAReport05A["VESTED"].ToString();
                        icdoSSA5500ReportDetails.plan_identifier = ldrSSAReport05A["PLAN_IDENTIFIER"].ToString();
                        icdoSSA5500ReportDetails.Insert();
                    }
                }

                //SSAReport05B details
                if (adtSSAReport05BData != null && adtSSAReport05BData.Rows.Count > 0)
                {
                    foreach (DataRow ldrSSAReport05B in adtSSAReport05BData.Rows)
                    {
                        icdoSSA5500ReportDetails.code = ldrSSAReport05B["IAP_5500_STATUS_CODE"].ToString();
                        icdoSSA5500ReportDetails.ssn = ldrSSAReport05B["SSN"].ToString();
                        //icdoSSA5500ReportDetails.name = ldrSSAReport05B["NAME"].ToString();
                        icdoSSA5500ReportDetails.first_name = ldrSSAReport05B["FIRST_NAME"].ToString();
                        icdoSSA5500ReportDetails.middle_name = ldrSSAReport05B["MIDDLE_NAME"].ToString();
                        icdoSSA5500ReportDetails.last_name = ldrSSAReport05B["LAST_NAME"].ToString();
                        icdoSSA5500ReportDetails.type_of_annuity = ldrSSAReport05B["TYPE_OF_ANNUITY"].ToString();
                        icdoSSA5500ReportDetails.payment_frequency = ldrSSAReport05B["PAYMENT_FREQUENCY"].ToString();
                        icdoSSA5500ReportDetails.gross_payment = Convert.ToDecimal(ldrSSAReport05B["DB_PAYMENT"]);
                        icdoSSA5500ReportDetails.units_shares = Convert.ToInt16(ldrSSAReport05B["UNITS_SHARES"]);
                        icdoSSA5500ReportDetails.total_value_account = Convert.ToDecimal(ldrSSAReport05B["TOTAL_VALUE_ACCOUNT"]);
                        icdoSSA5500ReportDetails.plan_year = Convert.ToInt16(ldrSSAReport05B["PLAN_YEAR_SEPERATED"]);
                        icdoSSA5500ReportDetails.vested = ldrSSAReport05B["VESTED"].ToString();
                        icdoSSA5500ReportDetails.plan_identifier = ldrSSAReport05B["PLAN_IDENTIFIER"].ToString();
                        icdoSSA5500ReportDetails.Insert();
                    }
                }

            }

        }
    }
}
