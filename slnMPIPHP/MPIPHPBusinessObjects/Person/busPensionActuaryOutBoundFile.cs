using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Linq;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Data.SqlClient;
using Sagitec.DataObjects;
using System.Collections;
using Sagitec.CustomDataObjects;
using Microsoft.Reporting.WinForms;
using System.IO;

namespace MPIPHP.BusinessObjects.Person
{
    [Serializable]
    public class busPensionActuaryOutBoundFile : busFileBaseOut
    {
        #region Properties
        busBase lobjBase = new busBase();
        public Collection<busDataExtractionBatchInfo> iclbPensionActuaryData { get; set; }
        public Collection<busPensionActuary> iclbbusPensionActuaryDataFromOpus { get; set; }
        public busSystemManagement iobjSystemManagement { get; set; }
        public cdoPensionActuary icdoPensionActuary { get; set; }

        #endregion

        public void LoadPensionActuaryData(DataTable adtbPensionActuaryData)
        {
            DataTable adtCompYear;
            adtCompYear = Sagitec.BusinessObjects.busBase.Select("cdoDataExtractionBatchInfo.GetLastPensionDataExtractionYear", new object[0]);
            int lintComputationYear = adtCompYear.Rows.Count > 0 ? Convert.ToInt32(adtCompYear.Rows[0]["YEAR"].ToString()) : 0;

            if (iobjSystemManagement == null)
            {
                iobjSystemManagement = new busSystemManagement();
                iobjSystemManagement.FindSystemManagement();
            }
            DateTime ToDate = busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear);
            DateTime FromDate = busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear - 1);
            FromDate = FromDate.AddDays(1);
            bool lboolInsertToOpusSucess = false;
            adtbPensionActuaryData = busBase.Select("cdoDataExtractionBatchInfo.GetDataForPensionActuaryFile", new object[3] { lintComputationYear, FromDate.Date, ToDate.Date });
            //PIR: 1065
            DataTable adtblPensionActuaryDataForExcel = adtbPensionActuaryData.Copy();
            System.Data.DataColumn newColumnYear = new System.Data.DataColumn("Year", typeof(System.Int32));
            newColumnYear.DefaultValue = lintComputationYear;
            adtblPensionActuaryDataForExcel.Columns.Add(newColumnYear);
            adtblPensionActuaryDataForExcel.TableName = "ReportTable01";
            if (adtbPensionActuaryData.Rows.Count > 0)
            {

                DataTable ldtPensionActuaryDataFromOpus = new DataTable();
                ldtPensionActuaryDataFromOpus = busBase.Select("cdoDataExtractionBatchInfo.GetExistingDataForpensionActuaryFromOpus", new object[1] { lintComputationYear });
                if (ldtPensionActuaryDataFromOpus.Rows.Count > 0)
                {
                    int lintrtn = DBFunction.DBNonQuery("cdoDataExtractionBatchInfo.DeleteRecordsIfExistfortheYear",
                              new object[1] { lintComputationYear }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    if (lintrtn > 0)
                    {
                        foreach (DataRow drPensionActuary in adtbPensionActuaryData.Rows)
                        {
                            InsertPensionActuary(drPensionActuary, lintComputationYear);
                            lboolInsertToOpusSucess = true;
                        }
                    }
                }
                else
                {
                    foreach (DataRow drPensionActuary in adtbPensionActuaryData.Rows)
                    {
                        InsertPensionActuary(drPensionActuary, lintComputationYear);
                        lboolInsertToOpusSucess = true;
                    }
                }
                if (lboolInsertToOpusSucess)
                {
                    if (adtblPensionActuaryDataForExcel != null && adtblPensionActuaryDataForExcel.Rows.Count > 0)
                    {

                        string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.PENSION_ACTUARY_REPORT + ".xlsx";
                        string lstrPensionActuaryReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.PENSION_ACTUARY_REPORT + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

                        DataSet ldsPensionActuaryDataForExcel = new DataSet();
                        ldsPensionActuaryDataForExcel.Tables.Add(adtblPensionActuaryDataForExcel);

                        busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                        lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrPensionActuaryReportPath, "Sheet1", ldsPensionActuaryDataForExcel);

                        //CreateExcelReport(adtblPensionActuaryDataForExcel, "rptPensionActuary");
                    }


                }
                iclbPensionActuaryData = lobjBase.GetCollection<busDataExtractionBatchInfo>(adtbPensionActuaryData, "icdoDataExtractionBatchInfo");

                foreach (busDataExtractionBatchInfo lbusDataExtractionBatchInfo in iclbPensionActuaryData)
                {
                    if (lbusDataExtractionBatchInfo.icdoDataExtractionBatchInfo.status_code_value == busConstant.VESTED_ACTIVE_PARTICIPANT)
                    {
                        lbusDataExtractionBatchInfo.icdoDataExtractionBatchInfo.status_code_value = busConstant.NON_VESTED_ACTIVE_PARTICIPANT;
                    }
                }


            }
        }


        public void InsertPensionActuary(DataRow drPensionActuary, int aintComputationYear)
        {

            busPensionActuary lbusPensionActuarydata = new busPensionActuary { icdoPensionActuary = new cdoPensionActuary() };
            lbusPensionActuarydata.icdoPensionActuary.computational_year = aintComputationYear;
            //if (drPensionActuary["PERSON_NAME"].ToString().IsNotNullOrEmpty())
            //{
            //    lbusPensionActuarydata.icdoPensionActuary.person_name = drPensionActuary["PERSON_NAME"].ToString();
            //}
            if (drPensionActuary["PERSON_SSN"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.person_ssn = drPensionActuary["PERSON_SSN"].ToString();
            }

            if (drPensionActuary["PERSON_GENDER_VALUE"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.person_gender_value = drPensionActuary["PERSON_GENDER_VALUE"].ToString();
            }
            if (drPensionActuary["istrPersonDateOfBirth"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.person_date_of_birth = Convert.ToDateTime(drPensionActuary["istrPersonDateOfBirth"]);
            }
            if (drPensionActuary["BENEFICIARY_FLAG"].ToString().IsNotNullOrEmpty())
            {

                lbusPensionActuarydata.icdoPensionActuary.beneficiary_flag = drPensionActuary["BENEFICIARY_FLAG"].ToString();
            }
            //if (drPensionActuary["BENEFICIARY_NAME"].ToString().IsNotNullOrEmpty())
            //{
            //    lbusPensionActuarydata.icdoPensionActuary.beneficiary_name = drPensionActuary["BENEFICIARY_NAME"].ToString();
            //}
            if (drPensionActuary["BENEFICIARY_SSN"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.beneficiary_ssn = drPensionActuary["BENEFICIARY_SSN"].ToString();
            }
            if (drPensionActuary["BENEFICIARY_GENDER_VALUE"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.beneficiary_gender_value = drPensionActuary["BENEFICIARY_GENDER_VALUE"].ToString();
            }
            if (drPensionActuary["BENEFICIARY_DOB"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.beneficiary_date_of_birth = Convert.ToDateTime(drPensionActuary["BENEFICIARY_DOB"]);
            }
            if (drPensionActuary["istrUnionCode"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.union_code = drPensionActuary["istrUnionCode"].ToString();
            }
            if (drPensionActuary["STATUS_CODE_VALUE"].ToString().IsNotNullOrEmpty())
            {
                if (drPensionActuary["STATUS_CODE_VALUE"].ToString() == busConstant.VESTED_ACTIVE_PARTICIPANT)
                {
                    lbusPensionActuarydata.icdoPensionActuary.status_code_value = busConstant.NON_VESTED_ACTIVE_PARTICIPANT;
                }
                else
                {
                    lbusPensionActuarydata.icdoPensionActuary.status_code_value = drPensionActuary["STATUS_CODE_VALUE"].ToString();
                }
            }
            if (drPensionActuary["PARTICIPANT_DATE_OF_DEATH"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.participant_date_of_death = Convert.ToDateTime(drPensionActuary["PARTICIPANT_DATE_OF_DEATH"]);
            }
            if (drPensionActuary["istrPlan"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.plan_name = drPensionActuary["istrPlan"].ToString();
            }
            lbusPensionActuarydata.icdoPensionActuary.total_qualified_years = Convert.ToInt32(Convert.ToBoolean(drPensionActuary["TOTAL_QF_YR_END_OF_LAST_COMP_YEAR"].IsDBNull()) ? busConstant.ZERO_INT : drPensionActuary["TOTAL_QF_YR_END_OF_LAST_COMP_YEAR"]);
            if (drPensionActuary["PARTICIPANT_STATE_VALUE"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.state_of_rsdnc = drPensionActuary["PARTICIPANT_STATE_VALUE"].ToString();
            }
            lbusPensionActuarydata.icdoPensionActuary.break_years = Convert.ToInt32(Convert.ToBoolean(drPensionActuary["Last_QF_YR_BEFORE_BIS"].IsDBNull()) ? busConstant.ZERO_INT : drPensionActuary["Last_QF_YR_BEFORE_BIS"]);
            lbusPensionActuarydata.icdoPensionActuary.non_eligible_benefit = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["NON_ELIGIBLE_BENEFIT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["NON_ELIGIBLE_BENEFIT"]);
            lbusPensionActuarydata.icdoPensionActuary.benefit_in_year = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["ACCRUED_BENEFIT_FOR_PRIOR_YEAR"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["ACCRUED_BENEFIT_FOR_PRIOR_YEAR"]);
            lbusPensionActuarydata.icdoPensionActuary.total_benefit = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR"]);
            lbusPensionActuarydata.icdoPensionActuary.total_ee_contribution_amt = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["TOTAL_EE_CONTRIBUTION_AMT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["TOTAL_EE_CONTRIBUTION_AMT"]);
            lbusPensionActuarydata.icdoPensionActuary.total_uvhp_amt = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["TOTAL_UVHP_AMT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["TOTAL_UVHP_AMT"]);
            lbusPensionActuarydata.icdoPensionActuary.credited_hr_ytd = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["YTD_HOURS_FOR_LAST_COMP_YEAR"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["YTD_HOURS_FOR_LAST_COMP_YEAR"]);
            lbusPensionActuarydata.icdoPensionActuary.credited_hours_total = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["TOTAL_HOURS"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["TOTAL_HOURS"]);
            lbusPensionActuarydata.icdoPensionActuary.credited_hours_last_year = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["YTD_HOURS_FOR_YEAR_BEFORE_LAST_COMP_YEAR"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["YTD_HOURS_FOR_YEAR_BEFORE_LAST_COMP_YEAR"]);
            lbusPensionActuarydata.icdoPensionActuary.total_ee_interest_amt = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["TOTAL_EE_INTEREST_AMT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["TOTAL_EE_INTEREST_AMT"]);
            lbusPensionActuarydata.icdoPensionActuary.total_uvhp_interest_amt = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["TOTAL_UVHP_INTEREST_AMT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["TOTAL_UVHP_INTEREST_AMT"]);
            lbusPensionActuarydata.icdoPensionActuary.monthly_benefit_amt = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["MONTHLY_BENEFIT_AMT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["MONTHLY_BENEFIT_AMT"]);
            lbusPensionActuarydata.icdoPensionActuary.remaining_mg = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["MG_AMT_LEFT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["MG_AMT_LEFT"]);
            
            if (drPensionActuary["BENEFIT_OPTION_CODE_VALUE"].ToString().IsNotNullOrEmpty())
            {

                lbusPensionActuarydata.icdoPensionActuary.benefit_option_code_value = drPensionActuary["BENEFIT_OPTION_CODE_VALUE"].ToString();
            }
            if (drPensionActuary["RETURN_TO_WORK_FLAG"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.return_to_work_flag = drPensionActuary["RETURN_TO_WORK_FLAG"].ToString();
            }
            if (drPensionActuary["DETERMINATION_DATE"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.date_of_retr_or_dsbl = Convert.ToDateTime(drPensionActuary["DETERMINATION_DATE"]);
            }
            if (drPensionActuary["BENEFICIARY_FIRST_PAYMENT_RECEIVE_DATE"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.beneficiary_first_payment_receive_date = Convert.ToDateTime(drPensionActuary["BENEFICIARY_FIRST_PAYMENT_RECEIVE_DATE"]);
            }
            if (drPensionActuary["PENSION_STOP_DATE"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.pension_stop_date = Convert.ToDateTime(drPensionActuary["PENSION_STOP_DATE"]);
            }
            lbusPensionActuarydata.icdoPensionActuary.total_qualified_years_at_ret = Convert.ToInt32(Convert.ToBoolean(drPensionActuary["TOTAL_QUALIFIED_YEARS_AT_RET"].IsDBNull()) ? busConstant.ZERO_INT : drPensionActuary["TOTAL_QUALIFIED_YEARS_AT_RET"]);
            lbusPensionActuarydata.icdoPensionActuary.total_qualified_hours_at_ret = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["TOTAL_QUALIFIED_HOURS_AT_RET"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["TOTAL_QUALIFIED_HOURS_AT_RET"]);
            if (drPensionActuary["BENEFICIARY_DATE_OF_DEATH"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.beneficiary_date_of_death = Convert.ToDateTime(drPensionActuary["BENEFICIARY_DATE_OF_DEATH"]);
            }

            lbusPensionActuarydata.icdoPensionActuary.cashout_amt = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["LUMP_AMT_TAKEN_IN_LAST_COMP_YR"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["LUMP_AMT_TAKEN_IN_LAST_COMP_YR"]);
            if (drPensionActuary["RETIREMENT_TYPE_VALUE"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.retirement_type_value = drPensionActuary["RETIREMENT_TYPE_VALUE"].ToString();

            }
            lbusPensionActuarydata.icdoPensionActuary.life_annuity_amt = Convert.ToDecimal(Convert.ToBoolean(drPensionActuary["LIFE_ANNUITY_AMT"].IsDBNull()) ? busConstant.ZERO_DECIMAL : drPensionActuary["LIFE_ANNUITY_AMT"]);
            if (drPensionActuary["MD_FLAG"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.md_flag = drPensionActuary["MD_FLAG"].ToString();

            }
            //RID 71411 adding new columns
            if (drPensionActuary["IS_DISABILITY_CONVERSION"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.is_disability_conversion = drPensionActuary["IS_DISABILITY_CONVERSION"].ToString();

            }
            if (drPensionActuary["IS_CONVERTED_FROM_POPUP"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.is_converted_from_popup = drPensionActuary["IS_CONVERTED_FROM_POPUP"].ToString();

            }
            if (drPensionActuary["DRO_MODEL"].ToString().IsNotNullOrEmpty())
            {
                lbusPensionActuarydata.icdoPensionActuary.dro_model = drPensionActuary["DRO_MODEL"].ToString();

            }

            lbusPensionActuarydata.icdoPensionActuary.Insert();

        }

    }

}


  
