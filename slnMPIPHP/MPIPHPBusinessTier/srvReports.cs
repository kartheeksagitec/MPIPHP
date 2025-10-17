#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MPIPHP.BusinessObjects;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using System.Linq;
using System.Data.SqlClient;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Collections.ObjectModel;

#endregion

namespace MPIPHP.BusinessTier
{
    public class srvReports : srvMPIPHP
    {
        public srvReports()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public DataSet LoadMailingLAbelsByCity(string astrState, string astrCity, DateTime adteCreateDate, DateTime adteModifiedDate)
        {
            if (adteCreateDate == DateTime.MinValue)
                adteCreateDate = Convert.ToDateTime("01/01/1900");

            if (adteModifiedDate == DateTime.MinValue)
                adteModifiedDate = Convert.ToDateTime("01/01/1900");

            return null;
            /*if(astrState == "")
                return new busPaymentDetails().LoadMailinLabelsByCity(astrState + "%", astrCity + "%", adteCreateDate, adteModifiedDate);
            else
                return new busPaymentDetails().LoadMailinLabelsByCity(astrState, astrCity + "%" , adteCreateDate, adteModifiedDate);*/
        }

        public DataSet LoadSpecialMailingLables(int iintOrgID)
        {
            /*if (iintOrgID == 0)
                return new busPaymentDetails().LoadSpecialMailingLables("%");
            else
                return new busPaymentDetails().LoadSpecialMailingLables(iintOrgID.ToString());*/
            return null;
        }

        #region Pension Report Of Death
        public DataSet PensionReportOfDeath(int MONTH, int YEAR)
        {
            DateTime idtStartDateofMonth = new DateTime(YEAR, MONTH, 1);
            DateTime idtLastDateofMonth = new DateTime(YEAR, MONTH, DateTime.DaysInMonth(YEAR, MONTH));

            DataTable dtlReportExecuteInfoTable = new DataTable();
            dtlReportExecuteInfoTable.Columns.Add(new DataColumn("StartDateofMonth", typeof(DateTime)));
            dtlReportExecuteInfoTable.Columns.Add(new DataColumn("LastDateofMonth", typeof(DateTime)));
            DataRow drInfo = dtlReportExecuteInfoTable.NewRow();
            drInfo["StartDateofMonth"] = idtStartDateofMonth;
            drInfo["LastDateofMonth"] = idtLastDateofMonth;
            dtlReportExecuteInfoTable.Rows.Add(drInfo);


            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            DataSet ldsReportDataSet = new DataSet();
            DataTable ldtbReportData = busBase.Select("cdoTempdata.rptPensionReportofDeath", new object[2] { MONTH, YEAR });
            ldtbReportData.Columns.Add("Occupation", typeof(string));
            ldtbReportData.Columns.Add("UnionCode", typeof(Int32));
            if (ldtbReportData != null && ldtbReportData.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbReportData.Rows)
                {
                    if (dr["DECRYPTED_SSN"] != DBNull.Value)
                    {
                        busIAPAllocationHelper lbusIAPAllocationHelper = new busIAPAllocationHelper();
                        string istrUnionCode = lbusIAPAllocationHelper.GetTrueUnionCodeBySSNAndPlanYear(busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToDateTime(dr["DATE_OF_DEATH"]).Year - 1).AddDays(1), busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToDateTime(dr["DATE_OF_DEATH"]).Year), dr["DECRYPTED_SSN"].ToString());

                        if (istrUnionCode.IsNotNullOrEmpty())
                        {
                            SqlParameter[] lParameters = new SqlParameter[1];
                            SqlParameter param1 = new SqlParameter("@UnionCode", DbType.String);
                            param1.Value = istrUnionCode;
                            lParameters[0] = param1;

                            DataTable ldtPersonOccupation = new DataTable();
                            ldtPersonOccupation = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetTrueUnionNamebyUnionCode", lstrLegacyDBConnetion, null, lParameters);

                            if (ldtPersonOccupation.Rows.Count > 0)
                            {
                                dr["Occupation"] = Convert.ToString(ldtPersonOccupation.Rows[0]["UNION_CODE_DESC"]);
                                dr["UnionCode"] = Convert.ToInt32(ldtPersonOccupation.Rows[0]["UNION_CODE"]);
                            }
                            else
                            {
                                dr["Occupation"] = "NO UNION";
                            }
                        }
                    }
                }
            }
            ldtbReportData.AcceptChanges();
            //DataTable ldtTempSortedData = 
            ldtbReportData.DefaultView.Sort = "Occupation,LAST_NAME, FIRST_NAME, SS4 ASC";
            ldsReportDataSet.Tables.Add(ldtbReportData.Copy());
            ldsReportDataSet.Tables[0].TableName = "ReportTable01";
            ldsReportDataSet.Tables.Add(dtlReportExecuteInfoTable.Copy());
            ldsReportDataSet.Tables[1].TableName = "ReportTable02";
            return ldsReportDataSet;
        }
        #endregion

        #region IAP MD Worked 400 Hours More
        public DataSet IAPMDWorked400(string ddlPensionYear)

        {
           // int YEAR = PensionYearID;
            DataSet ldsReportData = new DataSet();
            DataTable ldtIAPMDWorked = new DataTable();

            ldtIAPMDWorked = busBase.Select("cdoIapAllocationDetail.GetIAPMDWorked400Hrs", new object[1] { Convert.ToInt32(ddlPensionYear) });
            ldtIAPMDWorked.TableName = "ReportTable01";

            ldsReportData.Tables.Add(ldtIAPMDWorked.Copy());
            return ldsReportData;
        }
        #endregion

        #region IAP Reemployed Worked 870 Hours More
        public DataSet IAPReemployedWorked870(string ddlPensionYear)
        {
            DataSet ldsReportData = new DataSet();
            DataTable ldtIAPReemployedWorked = new DataTable();

            ldtIAPReemployedWorked = busBase.Select("cdoIapAllocationDetail.GetIAPReemployedWorked870Hrs", new object[1] { Convert.ToInt32(ddlPensionYear) });
            ldtIAPReemployedWorked.TableName = "ReportTable01";

            ldsReportData.Tables.Add(ldtIAPReemployedWorked.Copy());
            return ldsReportData;
        }
        #endregion

        #region Workflow Metric Summary Report
        TimeSpan duration;
        public DataSet WorkflowMetricSummaryReport(string startTime, string endTime, int typeId)
        {
            DataSet ldsReportDataSet = new DataSet();
            DataTable ldtbReportData = busBase.Select("cdoTempdata.rptWorkflowMetricsSummaryReport", new object[3] { startTime, endTime, typeId });
            ldtbReportData.Columns.Add("Avg_Process_Duration");
            ldtbReportData.Columns.Add("Avg_Backlog_Duration");
            ldtbReportData.Columns.Add("Mean_Process_Duration");
            ldtbReportData.Columns.Add("Mean_Backlog_Duration");

            foreach (DataRow dr in ldtbReportData.Rows)
            {
                if (dr["AVG_BACK_DURATION"] != DBNull.Value)
                {
                    duration = TimeSpan.FromMinutes(Convert.ToInt32(dr["AVG_BACK_DURATION"]));
                    dr["Avg_Backlog_Duration"] = duration.Days + ":" + duration.Hours + ":" + duration.Minutes;
                }

                if (dr["AVG_PROC_DURATION"] != DBNull.Value)
                {
                    duration = TimeSpan.FromMinutes(Convert.ToInt32(dr["AVG_PROC_DURATION"]));
                    dr["Avg_Process_Duration"] = duration.Days + ":" + duration.Hours + ":" + duration.Minutes;
                }

                if (dr["MEAN_PROC_DURATION"] != DBNull.Value)
                {
                    duration = TimeSpan.FromMinutes(Convert.ToInt32(dr["MEAN_PROC_DURATION"]));
                    dr["Mean_Process_Duration"] = duration.Days + ":" + duration.Hours + ":" + duration.Minutes;
                }

                if (dr["MEAN_BACK_DURATION"] != DBNull.Value)
                {
                    duration = TimeSpan.FromMinutes(Convert.ToInt32(dr["MEAN_BACK_DURATION"]));
                    dr["Mean_Backlog_Duration"] = duration.Days + ":" + duration.Hours + ":" + duration.Minutes;
                }
            }
            ldsReportDataSet.AcceptChanges();
            ldsReportDataSet.Tables.Add(ldtbReportData.Copy());
            ldsReportDataSet.Tables[0].TableName = "ReportTable01";
            return ldsReportDataSet;
        }
        #endregion

        #region Distribution Status Transition Report
        public DataSet DistributionStatusTransitionReport(string FromDate, string ToDate, int PLAN_ID)
        {
            DataSet ldsReportDataSet = new DataSet();

            ldsReportDataSet = DistributionTransitionData(FromDate, ToDate, PLAN_ID);
            return ldsReportDataSet;
        }

        private DataSet DistributionTransitionData(string FromDate, string ToDate, int PLAN_ID)
        {
            DataSet ldsReportDataSet = new DataSet();
            //Fetching all record 
            DataTable ldtbReportData = busBase.Select("cdoTempdata.rptDistributionStatusTransitionReport", new object[3] { PLAN_ID, FromDate, ToDate });
            ldtbReportData.TableName = "ReportTable01";

            DataTable ldtbReportTable02 = new DataTable();
            ldtbReportTable02.TableName = "ReportTable02";

            //Required Columns in report
            ldtbReportTable02.Columns.Add("Status");
            ldtbReportTable02.Columns.Add("Counts");
            ldtbReportTable02.Columns.Add("Gross");
            ldtbReportTable02.Columns.Add("Fed");
            ldtbReportTable02.Columns.Add("State");
            ldtbReportTable02.Columns.Add("Net");
            ldtbReportTable02.Columns.Add("Pay_Date");
            ldtbReportTable02.Columns.Add("Plan");

            //Group by Status
            var lstDistributionTotal = (from obj in ldtbReportData.AsEnumerable()
                                        group obj by new
                                        {
                                            ID = obj.Field<string>("DIST_STATUS"),
                                            ID1 = obj.Field<DateTime>("PAY_DATE").Year,
                                            ID2 = obj.Field<string>("PLAN")
                                        } into objp
                                        select new busdistributiontotal
                                        {
                                            istrStatus = objp.Key.ID,
                                            idecGrossAmt = objp.Sum(i => i.Field<decimal>("GROSS")),
                                            idecFedralAmt = objp.Sum(i => i.Field<decimal>("FED_TAX")),
                                            idecStateAmt = objp.Sum(i => i.Field<decimal>("STATE_TAX")),
                                            idecNetAmt = objp.Sum(i => i.Field<decimal>("NET")),
                                            iintcount = objp.Count(),
                                            iintYear = objp.Key.ID1,
                                            istrPlan = objp.Key.ID2,
                                        }).ToList();
            //Total by status
            foreach (var row in lstDistributionTotal)
            {
                DataRow dr = ldtbReportTable02.NewRow();
                dr["Status"] = row.istrStatus;
                dr["Counts"] = row.iintcount;
                dr["Gross"] = string.Format("{0:C}", row.idecGrossAmt);
                dr["Fed"] = string.Format("{0:C}", row.idecFedralAmt);
                dr["State"] = string.Format("{0:C}", row.idecStateAmt);
                dr["Net"] = string.Format("{0:C}", row.idecNetAmt);
                dr["PAY_DATE"] = row.iintYear;
                dr["Plan"] = row.istrPlan;
                ldtbReportTable02.Rows.Add(dr);
            }

            DataTable ldtbReportTable03 = new DataTable();
            ldtbReportTable03.TableName = "ReportTable03";

            //Required Columns in report
            ldtbReportTable03.Columns.Add("Counts");
            ldtbReportTable03.Columns.Add("Gross");
            ldtbReportTable03.Columns.Add("Fed");
            ldtbReportTable03.Columns.Add("State");
            ldtbReportTable03.Columns.Add("Net");
            ldtbReportTable03.Columns.Add("Plan");
            //Grand Total  --- Don't Delete
            //if (lstDistributionTotal != null && lstDistributionTotal.Count > 0)
            //{
            //    DataRow drTotal = ldtbReportTable03.NewRow();
            //    drTotal["Counts"] = (from obj in lstDistributionTotal.AsEnumerable()
            //                         select obj.iintcount).Sum();
            //    drTotal["Gross"] = string.Format("{0:C}", (from obj in lstDistributionTotal.AsEnumerable()
            //                                               select obj.idecGrossAmt).Sum());
            //    drTotal["Fed"] = string.Format("{0:C}", (from obj in lstDistributionTotal.AsEnumerable()
            //                                             select obj.idecFedralAmt).Sum());
            //    drTotal["State"] = string.Format("{0:C}", (from obj in lstDistributionTotal.AsEnumerable()
            //                                               select obj.idecStateAmt).Sum());
            //    drTotal["Net"] = string.Format("{0:C}", (from obj in lstDistributionTotal.AsEnumerable()
            //                                             select obj.idecNetAmt).Sum());
            //    drTotal["Plan"] = (from obj in lstDistributionTotal.AsEnumerable()
            //                                               select obj.istrPlan).FirstOrDefault().ToString();
            //    ldtbReportTable03.Rows.Add(drTotal);
            //}


            //Group by Plan
            var lstFinalTotalByPlan = (from obj in lstDistributionTotal.AsEnumerable()
                                       group obj by new
                                       {
                                           ID = obj.istrPlan
                                       } into objp
                                       select new
                                       {
                                           idecGrossAmt = objp.Sum(i => i.idecGrossAmt),
                                           idecFedralAmt = objp.Sum(i => i.idecFedralAmt),
                                           idecStateAmt = objp.Sum(i => i.idecStateAmt),
                                           idecNetAmt = objp.Sum(i => i.idecNetAmt),
                                           iintcount = objp.Sum(i => i.iintcount),
                                           istrPlan = objp.Key.ID,
                                       }).ToList();
            //Grand Total 
            foreach (var row in lstFinalTotalByPlan)
            {
                DataRow drTotal = ldtbReportTable03.NewRow();
                drTotal["Counts"] = row.iintcount;

                drTotal["Gross"] = string.Format("{0:C}", row.idecGrossAmt);
                drTotal["Fed"] = string.Format("{0:C}", row.idecFedralAmt);
                drTotal["State"] = string.Format("{0:C}", row.idecStateAmt);
                drTotal["Net"] = string.Format("{0:C}", row.idecNetAmt);
                drTotal["Plan"] = row.istrPlan;

                ldtbReportTable03.Rows.Add(drTotal);
            }

            ldsReportDataSet.Tables.Add(ldtbReportData.Copy());
            ldsReportDataSet.Tables.Add(ldtbReportTable02.Copy());
            ldsReportDataSet.Tables.Add(ldtbReportTable03.Copy());
            return ldsReportDataSet;
        }
        #endregion

        #region Payment Balanceing Report
        public DataSet PaymentBalanceingReport(string FromDate, string ToDate, int PLAN_ID)
        {
            DataSet ldsReportDataSet = new DataSet();
            //Fetching all record 
            DataTable ldtbReportData = busBase.Select("cdoTempdata.rpt16_PaymentBalancingReport", new object[3] { FromDate, ToDate, PLAN_ID });
            ldtbReportData.TableName = "ReportTable01";

            DataTable ldtbReportTable02 = new DataTable();
            ldtbReportTable02.TableName = "ReportTable02";

            //Required Columns in report
            ldtbReportTable02.Columns.Add("Gross_Amount", typeof(decimal));
            ldtbReportTable02.Columns.Add("Federal_Tax_Amount", typeof(decimal));
            ldtbReportTable02.Columns.Add("State_Tax_Amount", typeof(decimal));
            ldtbReportTable02.Columns.Add("ACCOUNT_TYPE", typeof(string));

            //Group by Status
            var lstDistributionTotal = (from obj in ldtbReportData.AsEnumerable()
                                        group obj by new { ID = obj.Field<string>("ACCOUNT_TYPE") } into objp
                                        select new busdistributiontotal
                                        {
                                            istrStatus = objp.Key.ID,
                                            idecGrossAmt = objp.Sum(i => i.Field<decimal>("Gross_Amount")),
                                            idecFedralAmt = objp.Sum(i => i["Federal_Tax_Amount"] == DBNull.Value ? 0.0M : i.Field<decimal>("Federal_Tax_Amount")),
                                            idecStateAmt = objp.Sum(i => i["State_Tax_Amount"] == DBNull.Value ? 0.0M : i.Field<decimal>("State_Tax_Amount")),
                                        }).ToList();
            //Total by status
            foreach (var row in lstDistributionTotal)
            {
                DataRow dr = ldtbReportTable02.NewRow();
                dr["ACCOUNT_TYPE"] = row.istrStatus;
                dr["Gross_Amount"] = row.idecGrossAmt;
                dr["Federal_Tax_Amount"] = row.idecFedralAmt;
                dr["State_Tax_Amount"] = row.idecStateAmt;
                ldtbReportTable02.Rows.Add(dr);
            }

            ldsReportDataSet.Tables.Add(ldtbReportData.Copy());
            ldsReportDataSet.Tables.Add(ldtbReportTable02.Copy());

            return ldsReportDataSet;
        }
        #endregion

        #region Retiree IAP Balance
        public DataSet RetireeIAPBalance(string astrBalance)
        {
            DataSet ldsReportData = new DataSet();
            DataTable ldtRetireeIAPBalance = new DataTable();
            decimal idecBalance = 0M;
            if (astrBalance.IsNotNullOrEmpty())
                idecBalance = Convert.ToDecimal(astrBalance.Trim('$'));

            ldtRetireeIAPBalance = busBase.Select("cdoDataExtractionBatchInfo.rptRetireeIAPBalance", new object[1] { idecBalance });
            ldtRetireeIAPBalance.TableName = "ReportTable01";

            ldsReportData.Tables.Add(ldtRetireeIAPBalance.Copy());
            return ldsReportData;
        }
        #endregion

        #region IAP Overpayment Report
        public DataSet IAPOverpaymentReport(string astrBalance, DateTime adtcutoffDate)
        {
            DataSet ldsReportData = new DataSet();
            DataTable ldtRetireeIAPBalance = new DataTable();
            decimal idecBalance = 0M;
            if (astrBalance.IsNotNullOrEmpty())
                idecBalance = Convert.ToDecimal(astrBalance.Trim('$'));

            if (adtcutoffDate == DateTime.MinValue)
                adtcutoffDate = DateTime.Now;

            ldtRetireeIAPBalance = busBase.Select("cdoDataExtractionBatchInfo.rptIAPOverpaymentReport", new object[2] { adtcutoffDate, idecBalance });
            ldtRetireeIAPBalance.TableName = "ReportTable01";

            ldsReportData.Tables.Add(ldtRetireeIAPBalance.Copy());
            return ldsReportData;
        }
        #endregion

        #region 1099R Recon Report
        public DataSet ReconReportfor1099R(string START_DATE, string END_DATE, string PLAN_IDENTIFIER, string STATE_CODE)
        {
            DataSet lds1099RReconReport = new DataSet();
            //Fetching all record 
            DataTable ldtbReportData = busBase.Select("cdoTempdata.rpt1099ReconReport", new object[4] { START_DATE, END_DATE, PLAN_IDENTIFIER, STATE_CODE });
            ldtbReportData.TableName = "ReportTable01";

            DataTable ldtbSummaryReportTable = new DataTable();
            ldtbSummaryReportTable.TableName = "ReportTable02";

            //Required Columns in report
            ldtbSummaryReportTable.Columns.Add("Gross_Amount", typeof(decimal));
            ldtbSummaryReportTable.Columns.Add("Result_Type", typeof(string));
            ldtbSummaryReportTable.Columns.Add("Report_order", typeof(int));
            ldtbSummaryReportTable.Columns.Add("FED_TAX_AMOUNT", typeof(decimal));
            ldtbSummaryReportTable.Columns.Add("STATE_TAX_AMOUNT", typeof(decimal));
            ldtbSummaryReportTable.Columns.Add("NET_AMOUNT", typeof(decimal));
            //Group by Report Item Type
            var lstReportSummaryDetails = (from obj in ldtbReportData.AsEnumerable()
                                           group obj by new { ID = obj.Field<string>("Result_Type"), ID1 = obj.Field<Int32>("Report_order") } into objp
                                           select new busdistributiontotal
                                           {
                                               istrStatus = objp.Key.ID,
                                               idecGrossAmt = objp.Sum(i => i["Gross_Amount"] == DBNull.Value ? 0.0M : i.Field<decimal>("Gross_Amount")),
                                               iintcount = objp.Key.ID1,
                                               idecFedralAmt = objp.Sum(i => i["FED_TAX_AMOUNT"] == DBNull.Value ? 0.0M : i.Field<decimal>("FED_TAX_AMOUNT")),
                                               idecStateAmt = objp.Sum(i => i["STATE_TAX_AMOUNT"] == DBNull.Value ? 0.0M : i.Field<decimal>("STATE_TAX_AMOUNT")),
                                               idecNetAmt = objp.Sum(i => i["NET_AMOUNT"] == DBNull.Value ? 0.0M : i.Field<decimal>("NET_AMOUNT")),
                                           }).ToList();
            //Total by Report Item Type
            foreach (var rowDetails in lstReportSummaryDetails)
            {
                DataRow dr = ldtbSummaryReportTable.NewRow();
                dr["Result_Type"] = "Total " + rowDetails.istrStatus;
                dr["Gross_Amount"] = rowDetails.idecGrossAmt;
                dr["Report_order"] = rowDetails.iintcount;
                dr["FED_TAX_AMOUNT"] = rowDetails.idecFedralAmt;
                dr["STATE_TAX_AMOUNT"] = rowDetails.idecStateAmt;
                dr["NET_AMOUNT"] = rowDetails.idecNetAmt;
                ldtbSummaryReportTable.Rows.Add(dr);
            }

            lds1099RReconReport.Tables.Add(ldtbReportData.Copy());
            lds1099RReconReport.Tables.Add(ldtbSummaryReportTable.Copy());

            return lds1099RReconReport;

        }
        #endregion

        #region Payment Directive Recon Report
        public DataSet PaymentDirectiveReconReport(int PLAN_ID, int ADHOC)
        {

            string lstrPlan = string.Empty;
            DataSet ldsPaymentDirectiveReconReport = new DataSet();
            DataTable ldtbTemp = busBase.Select("cdoPaymentDirectives.PaymentDirectiveReconReport", new object[2] { PLAN_ID, ADHOC });

            DataTable ldtbRecon = new DataTable();
            ldtbRecon.TableName = "ReportTable01";

            DataTable ldtbMismatches = new DataTable();

            if (PLAN_ID == busConstant.IAP_PLAN_ID)
            {
                lstrPlan = "IAP";
            }
            else
            {
                lstrPlan = "Pension";
            }

            ldtbRecon.Columns.Add("CATEGORY", typeof(string));
            ldtbRecon.Columns.Add("PAYEE_ACCOUNT_GROSS_AMOUNT", typeof(decimal));
            ldtbRecon.Columns.Add("PAYMENT_DIRECTIVE_GROSS_AMOUNT", typeof(decimal));
            ldtbRecon.Columns.Add("PAYMENT_DATE", typeof(DateTime));
            ldtbRecon.Columns.Add("PLAN", typeof(string));
            ldtbRecon.Columns.Add("DIFFERENCE", typeof(decimal));

            if ((from obj in ldtbTemp.AsEnumerable()
                 group obj by new { CATEGORY = obj.Field<string>("CATEGORY") } into objp
                 select new busPaymentDirectiveRecon
                 {
                     istrCategory = objp.Key.CATEGORY,

                     idecPayeeAccountGrossAmount = objp.Where(t => t.Field<string>("STATUS_VALUE") == busConstant.PayeeAccountStatusReceiving || t.Field<string>("STATUS_VALUE") == busConstant.PayeeAccountStatusApproved)
                        .Sum(i => i["PAYEE_ACCOUNT_GROSS_AMOUNT"] == DBNull.Value ? 0.0M : i.Field<decimal>("PAYEE_ACCOUNT_GROSS_AMOUNT")),

                     idecPaymentDirectivesGrossAmount = objp.Sum(i => i["PAYMENT_DIRECTIVE_GROSS_AMOUNT"] == DBNull.Value ? 0.0M : i.Field<decimal>("PAYMENT_DIRECTIVE_GROSS_AMOUNT")),

                     idtPaymentDate = objp.FirstOrDefault().Field<DateTime>("PAYMENT_DATE"),

                 }).Count() > 0)
            {
                var lvarRecon = (from obj in ldtbTemp.AsEnumerable()
                                 group obj by new { CATEGORY = obj.Field<string>("CATEGORY") } into objp
                                 select new busPaymentDirectiveRecon
                                 {
                                     istrCategory = objp.Key.CATEGORY,

                                     idecPayeeAccountGrossAmount = objp.Where(t => t.Field<string>("STATUS_VALUE") == busConstant.PayeeAccountStatusReceiving || t.Field<string>("STATUS_VALUE") == busConstant.PayeeAccountStatusApproved)
                                        .Sum(i => i["PAYEE_ACCOUNT_GROSS_AMOUNT"] == DBNull.Value ? 0.0M : i.Field<decimal>("PAYEE_ACCOUNT_GROSS_AMOUNT")),

                                     idecPaymentDirectivesGrossAmount = objp.Sum(i => i["PAYMENT_DIRECTIVE_GROSS_AMOUNT"] == DBNull.Value ? 0.0M : i.Field<decimal>("PAYMENT_DIRECTIVE_GROSS_AMOUNT")),

                                     idtPaymentDate = objp.FirstOrDefault().Field<DateTime>("PAYMENT_DATE"),

                                 }).ToList();


                foreach (var rowDetails in lvarRecon)
                {
                    DataRow dr = ldtbRecon.NewRow();
                    dr["CATEGORY"] = rowDetails.istrCategory;
                    dr["PAYEE_ACCOUNT_GROSS_AMOUNT"] = rowDetails.idecPayeeAccountGrossAmount;
                    dr["PAYMENT_DIRECTIVE_GROSS_AMOUNT"] = rowDetails.idecPaymentDirectivesGrossAmount;
                    dr["DIFFERENCE"] = rowDetails.idecPayeeAccountGrossAmount - rowDetails.idecPaymentDirectivesGrossAmount;
                    dr["PAYMENT_DATE"] = rowDetails.idtPaymentDate;
                    dr["PLAN"] = lstrPlan;
                    ldtbRecon.Rows.Add(dr);
                }
            }

            if ((from obj in ldtbTemp.AsEnumerable()
                 where Convert.ToDecimal(obj.Field<decimal>("DIFFERENCE")) != decimal.Zero
                 || (obj.Field<string>("STATUS_VALUE") != busConstant.PayeeAccountStatusReceiving && obj.Field<string>("STATUS_VALUE") != busConstant.PayeeAccountStatusApproved)
                 select obj
                                  ).Count() > 0)
            {
                var lvarMismatches = (from obj in ldtbTemp.AsEnumerable()
                                      where Convert.ToDecimal(obj.Field<decimal>("DIFFERENCE")) != decimal.Zero
                                      || (obj.Field<string>("STATUS_VALUE") != busConstant.PayeeAccountStatusReceiving && obj.Field<string>("STATUS_VALUE") != busConstant.PayeeAccountStatusApproved)
                                      select obj
                                      ).ToList();

                ldtbMismatches = lvarMismatches.CopyToDataTable();
            }

            ldtbMismatches.TableName = "ReportTable02";

            ldsPaymentDirectiveReconReport.Tables.Add(ldtbRecon.Copy());
            ldsPaymentDirectiveReconReport.Tables.Add(ldtbMismatches.Copy());
            ldsPaymentDirectiveReconReport.DataSetName = "rptPaymentDirectiveReconReport";

            return ldsPaymentDirectiveReconReport;

        }
        #endregion

        #region Payee Error Report
        public DataSet PayeeErrorReport(int aintPlanId, DateTime adtStatusEffectiveDateFrom, DateTime adtStatusEffectiveDateTo)
        {
            DateTime ldtNextBenefitPaymentDate = new DateTime();
            DataSet ldtReportResult;
            ldtReportResult = new DataSet();

            DateTime ldtLastBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate(aintPlanId);
            if (aintPlanId != busConstant.IAP_PLAN_ID)
                ldtNextBenefitPaymentDate = ldtLastBenefitPaymentDate.AddMonths(1);
            else
                ldtNextBenefitPaymentDate = busGlobalFunctions.GetPaymentDayForIAP(ldtLastBenefitPaymentDate.AddDays(7));

            DataTable ldtReportTable01 = new DataTable();
            ldtReportTable01 = busBase.Select("cdoPayeeAccount.FinalErrorReport", new object[5] { ldtNextBenefitPaymentDate, busConstant.Flag_Yes, adtStatusEffectiveDateFrom, adtStatusEffectiveDateTo, aintPlanId });
            ldtReportTable01.TableName = "ReportTable01";

            DataTable ldtReportTable02 = new DataTable();
            ldtReportTable02 = busBase.Select("cdoPayeeAccount.FinalErrorReport", new object[5] { ldtNextBenefitPaymentDate, busConstant.FLAG_NO, adtStatusEffectiveDateFrom, adtStatusEffectiveDateTo, aintPlanId });
            ldtReportTable02.TableName = "ReportTable02";

            DataTable ldtReportTable03 = new DataTable();
            ldtReportTable03.TableName = "ReportTable03";

            //Required Columns in report
            ldtReportTable03.Columns.Add("PLAN", typeof(string));
            ldtReportTable03.Columns.Add("PAYMENT_DATE", typeof(DateTime));
            ldtReportTable03.Columns.Add("STATUS_EFFECTIVE_DATE_FROM", typeof(DateTime));
            ldtReportTable03.Columns.Add("STATUS_EFFECTIVE_DATE_TO", typeof(DateTime));

            DataRow dr = ldtReportTable03.NewRow();

            if (aintPlanId == busConstant.IAP_PLAN_ID)
            {
                dr["PLAN"] = "IAP";
            }
            else
            {
                dr["PLAN"] = "Pension";
            }
            dr["PAYMENT_DATE"] = ldtNextBenefitPaymentDate;
            dr["STATUS_EFFECTIVE_DATE_FROM"] = adtStatusEffectiveDateFrom;
            dr["STATUS_EFFECTIVE_DATE_TO"] = adtStatusEffectiveDateTo;
            ldtReportTable03.Rows.Add(dr);

            ldtReportResult.Tables.Add(ldtReportTable01.Copy());
            ldtReportResult.Tables.Add(ldtReportTable02.Copy());
            ldtReportResult.Tables.Add(ldtReportTable03.Copy());

            return ldtReportResult;
        }
        #endregion Payee Error Report

        #region Overpayment Report For Deceased Retirees

        public DataSet OverpaymentReportForDeceasedRetirees()
        {
            DataSet ldsReportData = new DataSet();
            ldsReportData.DataSetName = "rptDeceasedRetireesOverpaymentReport";

            DataTable ReportTable02 = new DataTable();
            ReportTable02.TableName = "ReportTable02";


            DateTime ldtBalanceAsOfDate = new DateTime();
            decimal ldecBalanceAsOfPrevPeriod = decimal.Zero;
            decimal ldecAdditionalOPBalance = decimal.Zero;
            decimal ldecAmountReimbursed = decimal.Zero;
            decimal ldecOutstandingBalance = decimal.Zero;
            decimal ldecAdjustmentAmount = decimal.Zero;

            decimal ldecWriteOffBalanceAsOfPrevPeriod = decimal.Zero;
            int ldecWriteOffBalanceAsOfYear = 0;
            decimal ldecWriteOffBalanceCurrPeriod = decimal.Zero;
            decimal ldecWriteOffBalance = decimal.Zero;
            decimal ldecWriteOffBalanceAdjustment = decimal.Zero;

            DataTable ldtOverpaymentReportForDeceasedRetirees = new DataTable();
            DataTable ldtWriteOffOverpaymentsForDeceasedRetirees = new DataTable();

            ldtOverpaymentReportForDeceasedRetirees.TableName = "ReportTable01";

            ldtOverpaymentReportForDeceasedRetirees = busBase.Select("cdoDeathOverpaymentReportBalances.rptDeceasedRetireesOverpaymentReport", new object[0] { });

            DataTable ldtLatestBalances = busBase.Select("cdoDeathOverpaymentReportBalances.GetLatestBalances", new object[0] { });
            if (ldtLatestBalances != null && ldtLatestBalances.Rows.Count > 0)
            {
                if (Convert.ToString(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.outstanding_balance.ToString().ToUpper()]).IsNotNullOrEmpty())
                    ldecBalanceAsOfPrevPeriod = Convert.ToDecimal(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.outstanding_balance.ToString().ToUpper()]);

                if (Convert.ToString(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.write_off_balance_as_of_prev_period.ToString().ToUpper()]).IsNotNullOrEmpty())
                    ldecWriteOffBalanceAsOfPrevPeriod = Convert.ToDecimal(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.write_off_balance_as_of_prev_period.ToString().ToUpper()]);

                if (Convert.ToString(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.write_off_balance_as_of_year.ToString().ToUpper()]).IsNotNullOrEmpty())
                    ldecWriteOffBalanceAsOfYear = Convert.ToInt32(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.write_off_balance_as_of_year.ToString().ToUpper()]);

                if (Convert.ToString(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.write_off_balance_curr_period.ToString().ToUpper()]).IsNotNullOrEmpty())
                    ldecWriteOffBalanceCurrPeriod = Convert.ToDecimal(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.write_off_balance_curr_period.ToString().ToUpper()]);

                if (Convert.ToString(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.write_off_balance.ToString().ToUpper()]).IsNotNullOrEmpty())
                    ldecWriteOffBalance = Convert.ToDecimal(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.write_off_balance.ToString().ToUpper()]);


                if (Convert.ToString(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.adjustment_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                {
                    ldecAdjustmentAmount = Convert.ToDecimal(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.adjustment_amount.ToString().ToUpper()]);
                }
            }

            busDeathOverpaymentReportBalances lbusDeathOveraymentReportBalances
                = new busDeathOverpaymentReportBalances { icdoDeathOverpaymentReportBalances = new cdoDeathOverpaymentReportBalances() };

            if (ldtOverpaymentReportForDeceasedRetirees != null && ldtOverpaymentReportForDeceasedRetirees.Rows.Count > 0)
            {
                ldtBalanceAsOfDate = Convert.ToDateTime(ldtOverpaymentReportForDeceasedRetirees.Rows[0]["REPORT_AS_OF_DATE"]);

                if (ldtBalanceAsOfDate.Month == 3)
                {
                    ldtWriteOffOverpaymentsForDeceasedRetirees = busBase.Select("cdoDeathOverpaymentReportBalances.GetWriteOffBalance", new object[0] { });
                    if (ldtWriteOffOverpaymentsForDeceasedRetirees != null && ldtWriteOffOverpaymentsForDeceasedRetirees.Rows.Count > 0)
                    {

                        if (Convert.ToString(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.write_off_balance.ToString().ToUpper()]).IsNotNullOrEmpty())
                            ldecWriteOffBalanceAsOfPrevPeriod = Convert.ToDecimal(ldtLatestBalances.Rows[0][enmDeathOverpaymentReportBalances.write_off_balance.ToString().ToUpper()]);

                        if (Convert.ToString(ldtWriteOffOverpaymentsForDeceasedRetirees.Rows[0]["YEAR"]).IsNotNullOrEmpty())
                            ldecWriteOffBalanceAsOfYear = Convert.ToInt32(ldtWriteOffOverpaymentsForDeceasedRetirees.Rows[0]["YEAR"]);

                        if (Convert.ToString(ldtWriteOffOverpaymentsForDeceasedRetirees.Rows[0]["OS_BALANCE"]).IsNotNullOrEmpty())
                            ldecWriteOffBalanceCurrPeriod = Convert.ToDecimal(ldtWriteOffOverpaymentsForDeceasedRetirees.Rows[0]["OS_BALANCE"]);

                        ldecWriteOffBalance = ldecWriteOffBalanceAsOfPrevPeriod + ldecWriteOffBalanceCurrPeriod;

                    }
                }


                ldecAmountReimbursed = ldtOverpaymentReportForDeceasedRetirees.AsEnumerable().Sum(t => t.Field<decimal>("AMT_RECOUPED_IN_CURR_PERIOD")) * -1;
                ldecOutstandingBalance = ldtOverpaymentReportForDeceasedRetirees.AsEnumerable().Sum(t => t.Field<decimal>("OS_BALANCE"));

                if (ldtBalanceAsOfDate.Date == Convert.ToDateTime(busConstant.Death_Report_Start_Date).Date)
                {
                    ldecWriteOffBalanceAdjustment = ldecWriteOffBalance;
                }
                else
                {
                    ldecWriteOffBalanceAdjustment = ldecWriteOffBalanceCurrPeriod;
                }

                ldecAdditionalOPBalance = (ldecOutstandingBalance - ldecAmountReimbursed) - (ldecBalanceAsOfPrevPeriod - ldecWriteOffBalanceAdjustment + ldecAdjustmentAmount);
                ldecWriteOffBalance = ldecWriteOffBalanceAsOfPrevPeriod + ldecWriteOffBalanceCurrPeriod;


                //Required Columns in report
                ReportTable02.Columns.Add("balance_as_of_prev_period", typeof(decimal));
                ReportTable02.Columns.Add("additional_op_curr_period", typeof(decimal));
                ReportTable02.Columns.Add("amount_reimbursed_curr_period", typeof(decimal));
                ReportTable02.Columns.Add("outstanding_balance", typeof(decimal));
                ReportTable02.Columns.Add("write_off_balance_as_of_prev_period", typeof(decimal));
                ReportTable02.Columns.Add("write_off_balance_as_of_year", typeof(int));
                ReportTable02.Columns.Add("write_off_balance_curr_period", typeof(decimal));
                ReportTable02.Columns.Add("write_off_balance", typeof(decimal));
                ReportTable02.Columns.Add("write_off_balance_adjustment", typeof(decimal));
                ReportTable02.Columns.Add("adjustment_amount", typeof(decimal));

                DataRow ldrReportTable02 = ReportTable02.NewRow();

                ldrReportTable02["adjustment_amount"] = ldecAdjustmentAmount;

                lbusDeathOveraymentReportBalances.icdoDeathOverpaymentReportBalances.balance_as_of_date = ldtBalanceAsOfDate;
                ldrReportTable02["balance_as_of_prev_period"] = lbusDeathOveraymentReportBalances.icdoDeathOverpaymentReportBalances.balance_as_of_prev_period = ldecBalanceAsOfPrevPeriod;
                ldrReportTable02["additional_op_curr_period"] = lbusDeathOveraymentReportBalances.icdoDeathOverpaymentReportBalances.additional_op_curr_period = ldecAdditionalOPBalance;
                ldrReportTable02["amount_reimbursed_curr_period"] = lbusDeathOveraymentReportBalances.icdoDeathOverpaymentReportBalances.amount_reimbursed_curr_period = ldecAmountReimbursed;
                ldrReportTable02["outstanding_balance"] = lbusDeathOveraymentReportBalances.icdoDeathOverpaymentReportBalances.outstanding_balance = ldecOutstandingBalance;
                ldrReportTable02["write_off_balance_as_of_prev_period"] = lbusDeathOveraymentReportBalances.icdoDeathOverpaymentReportBalances.write_off_balance_as_of_prev_period = ldecWriteOffBalanceAsOfPrevPeriod;
                ldrReportTable02["write_off_balance_as_of_year"] = lbusDeathOveraymentReportBalances.icdoDeathOverpaymentReportBalances.write_off_balance_as_of_year = ldecWriteOffBalanceAsOfYear;
                ldrReportTable02["write_off_balance_curr_period"] = lbusDeathOveraymentReportBalances.icdoDeathOverpaymentReportBalances.write_off_balance_curr_period = ldecWriteOffBalanceCurrPeriod;
                ldrReportTable02["write_off_balance_adjustment"] = ldecWriteOffBalanceAdjustment * -1;
                ldrReportTable02["write_off_balance"] = lbusDeathOveraymentReportBalances.icdoDeathOverpaymentReportBalances.write_off_balance = ldecWriteOffBalance;

                lbusDeathOveraymentReportBalances.icdoDeathOverpaymentReportBalances.Insert();

                ReportTable02.Rows.Add(ldrReportTable02);
            }


            ldsReportData.Tables.Add(ldtOverpaymentReportForDeceasedRetirees.Copy());
            ldsReportData.Tables.Add(ReportTable02.Copy());
            return ldsReportData;
        }

        #endregion


        #region Payee Overpayment Report

        public DataSet PayeeOverpaymentReport()
        {
            DataSet ldsReportData = new DataSet();
            ldsReportData.DataSetName = "rptPayeeOverpaymentReport";

            DataTable ldtOverpaymentReportForPayee = new DataTable();
            ldtOverpaymentReportForPayee.TableName = "ReportTable01";

            ldtOverpaymentReportForPayee = busBase.Select("cdoPayeeOverpaymentReportData.rptPayeeOverpaymentReport", new object[0] { });

            ldsReportData.Tables.Add(ldtOverpaymentReportForPayee.Copy());
            return ldsReportData;
        }

        #endregion

        #region Reemployment Count Report Details
        public DataSet ReemploymentCountDetails(string FROM, string TO)
        {
            DataSet ldsReportData = new DataSet();
            DataTable ldtReemploymentDetails = new DataTable();
            DateTime ldtfrom, ldtto;
            if (FROM.IsNullOrEmpty() || TO.IsNullOrEmpty())
            {
                ldtfrom = new DateTime(1900, 01, 01);
                ldtto = new DateTime(1900, 01, 01);
            }
            else
            {
                ldtfrom = Convert.ToDateTime(FROM);
                ldtto = Convert.ToDateTime(TO);
            }

            ldtReemploymentDetails = busBase.Select("cdoTempdata.rptRe-employmentCountsbyMonth", new object[2] { ldtfrom, ldtto });
            ldtReemploymentDetails.TableName = "ReportTable01";
            ldsReportData.Tables.Add(ldtReemploymentDetails.Copy());
            return ldsReportData;
        }

        #endregion

        #region Retiree List Report By Dates
        //Note : Retiree List Report By Dates(online report) and New Retiree List Report (which gets generated from Payment Batch) shares same method , query and Report format.
        //Please make sure to change / test both the Reports whenever there is any change to the code , query or report design.
        public DataSet RetireeListReportByDates(DateTime adtPaymentDate)
        {
            DataSet ldsReportData = new DataSet();
            DataTable ldtRetireeListReportByDates = new DataTable();

            busCreateReports lbusCreateReports = new busCreateReports();

            ldtRetireeListReportByDates = lbusCreateReports.FinalRetireeListMangtReport(adtPaymentDate, 0);
            ldsReportData.Tables.Add(ldtRetireeListReportByDates.Copy());

            return ldsReportData;
        }
        #endregion
        public DataSet LocalGroupRetireeListReportByDates(string FROM, string TO, string Lcl_ID)
        {
            DateTime ldtfrom, ldtto;
            if (FROM.IsNullOrEmpty() || TO.IsNullOrEmpty())
            {
                ldtfrom = new DateTime(1900, 01, 01);
                ldtto = new DateTime(1900, 01, 01);
            }
            else
            {
                ldtfrom = Convert.ToDateTime(FROM);
                ldtto = Convert.ToDateTime(TO);
            }

            DataTable dtlReportExecuteInfoTable = new DataTable();
            dtlReportExecuteInfoTable.Columns.Add(new DataColumn("FromDate", typeof(DateTime)));
            dtlReportExecuteInfoTable.Columns.Add(new DataColumn("ToDate", typeof(DateTime)));
            dtlReportExecuteInfoTable.Columns.Add(new DataColumn("Header_Info", typeof(String)));
            DataRow drInfo = dtlReportExecuteInfoTable.NewRow();
            drInfo["FromDate"] = ldtfrom;
            drInfo["ToDate"] = ldtto;
            if (Lcl_ID != string.Empty)
            {
                drInfo["Header_Info"] = "Local " + Lcl_ID + " Retiree Report";
            }
            else
            {
                drInfo["Header_Info"] = "Local Retiree Report";
            }

            dtlReportExecuteInfoTable.Rows.Add(drInfo);

            DataSet ldsReportData = new DataSet();
            DataTable ldtLocalRetireeReport = new DataTable();

            ldtLocalRetireeReport = busBase.Select("cdoPerson.GetLocalRetireelistbyDateRange", new object[2] { ldtfrom, ldtto });

            DataTable ldtTempSortedData = null;//new DataTable();
            if (ldtLocalRetireeReport.Rows.Count > 0)
            {

                if (Lcl_ID != string.Empty)
                {
                    ldtTempSortedData = ldtLocalRetireeReport.AsEnumerable().Where(x => x.Field<int>("LocalNumber") == Convert.ToInt32(Lcl_ID)).AsDataTable();
                }
                else
                {
                    ldtTempSortedData = ldtLocalRetireeReport;
                }
                if (ldtTempSortedData.Rows.Count > 0)
                {
                   ldtTempSortedData.DefaultView.Sort = "NAME ASC";

                }

            }
            else
            {
                ldtTempSortedData = ldtLocalRetireeReport;

            }

            ldsReportData.Tables.Add(ldtTempSortedData.Copy());
            ldsReportData.Tables[0].TableName = "ReportTable01";
            ldsReportData.Tables.Add(dtlReportExecuteInfoTable.Copy());
            ldsReportData.Tables[1].TableName = "ReportTable02";
            return ldsReportData;
        }

        public DataSet GetRetireeIAPBalance (string FROM, string TO)
        {
            DateTime ldtfrom, ldtto;
            if (FROM.IsNullOrEmpty() || TO.IsNullOrEmpty())
            {
                ldtfrom = new DateTime(1900, 01, 01);
                ldtto = new DateTime(1900, 01, 01);
            }
            else
            {
                ldtfrom = Convert.ToDateTime(FROM);
                ldtto = Convert.ToDateTime(TO);
            }

          
            DataSet ldsReportData = new DataSet();
            DataTable ldtRetireeIAPReport = new DataTable();

            ldtRetireeIAPReport = busBase.Select("cdoDataExtractionBatchInfo.GetRetireeIAPBalance", new object[2] { ldtfrom, ldtto });

           
            
            ldsReportData.Tables.Add(ldtRetireeIAPReport.Copy());
            ldsReportData.Tables[0].TableName = "ReportTable01";
           return ldsReportData;
        }

        #region QPSA Benefit Election Form status
        public DataSet GetQPSABenefitElection()

        {
            // int YEAR = PensionYearID;
            DataSet ldsReportData = new DataSet();
            DataTable ldtQPSABenefitElection = new DataTable();

            ldtQPSABenefitElection = busBase.Select("cdoBenefitCalculationHeader.GetQPSABenefitElection", new object[0] {});
            ldtQPSABenefitElection.TableName = "ReportTable01";

            ldsReportData.Tables.Add(ldtQPSABenefitElection.Copy());
            return ldsReportData;
        }
        #endregion

        public DataSet GetLocalDeathReport(string FROM, string TO, string Lcl_ID)
        {
            DataSet ldsReportData = new DataSet();
            DataTable ldtMpegDeathReport = new DataTable();
            DateTime ldtfrom, ldtto;
            if (FROM.IsNullOrEmpty() || TO.IsNullOrEmpty())
            {
                ldtfrom = new DateTime(1900, 01, 01);
                ldtto = new DateTime(1900, 01, 01);
            }
            else
            {
                ldtfrom = Convert.ToDateTime(FROM);
                ldtto = Convert.ToDateTime(TO);
            }

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            DataSet ldsReportDataSet = new DataSet();
            DataTable ldtbReportData = busBase.Select("cdoPerson.GetLocalDeathReportList", new object[2] { ldtfrom, ldtto });
            ldtbReportData.Columns.Add("Occupation", typeof(string));
            ldtbReportData.Columns.Add("UnionCode", typeof(Int32));
            ldtbReportData.Columns.Add("LocalNumber", typeof(Int32));


            DataTable dtlReportExecuteInfoTable = new DataTable();
            dtlReportExecuteInfoTable.Columns.Add(new DataColumn("Header_Info", typeof(string)));
            DataRow drInfo = dtlReportExecuteInfoTable.NewRow();

            if (Lcl_ID != string.Empty)
            {
                drInfo["Header_Info"] = "Local " + Lcl_ID + " Death Report";
            }
            else
            {
                drInfo["Header_Info"] = "Local Death Report";
            }

            dtlReportExecuteInfoTable.Rows.Add(drInfo);


            if (ldtbReportData != null && ldtbReportData.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbReportData.Rows)
                {
                    if (dr["DECRYPTED_SSN"] != DBNull.Value)
                    {
                        busIAPAllocationHelper lbusIAPAllocationHelper = new busIAPAllocationHelper();
                        string istrUnionCode = lbusIAPAllocationHelper.GetTrueUnionCodeBySSNAndPlanYear(ldtfrom, ldtto, dr["DECRYPTED_SSN"].ToString());

                        if (istrUnionCode.IsNotNullOrEmpty())
                        {
                            DataTable ldtPersonOccupation = new DataTable();
                            //  ldtPersonOccupation = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetTrueUnionNamebyUnionCode", lstrLegacyDBConnetion, null, lParameters);
                            ldtPersonOccupation = busBase.Select("cdoPerson.GETUnionCodeandLocalNumber", new object[1] { Convert.ToInt32(istrUnionCode) });

                            if (ldtPersonOccupation.Rows.Count > 0)
                            {
                                dr["Occupation"] = Convert.ToString(ldtPersonOccupation.Rows[0]["UNION_CODE_DESC"]);
                                dr["UnionCode"] = Convert.ToInt32(ldtPersonOccupation.Rows[0]["UnionCode"]);
                                dr["LocalNumber"] = Convert.ToInt32(ldtPersonOccupation.Rows[0]["LocalNumber"]);
                            }
                            else
                            {
                                dr["Occupation"] = "NO UNION";
                            }
                        }
                    }
                }
            }
            ldtbReportData.AcceptChanges();
            DataTable ldtTempSortedData = null;//new DataTable();
            if (ldtbReportData.Rows.Count > 0)
            {

                if (Lcl_ID != string.Empty)
                {
                    ldtTempSortedData = ldtbReportData.AsEnumerable().Where(x => x.Field<int>("LocalNumber") == Convert.ToInt32(Lcl_ID)).AsDataTable();

                }
                else
                {
                    ldtTempSortedData = ldtbReportData;
                }
                if (ldtTempSortedData.Rows.Count > 0)
                {
                    ldtTempSortedData.DefaultView.Sort = "Occupation,LAST_NAME, FIRST_NAME ASC";

                }
            }
            else
            {
                ldtTempSortedData = ldtbReportData;

            }


            ldsReportDataSet.Tables.Add(ldtTempSortedData.Copy());
            ldsReportDataSet.Tables[0].TableName = "ReportTable01";
            ldsReportDataSet.Tables.Add(dtlReportExecuteInfoTable.Copy());
            ldsReportDataSet.Tables[1].TableName = "ReportTable02";

            return ldsReportDataSet;
        }



        public class busdistributiontotal
        {
            public decimal idecGrossAmt { get; set; }
            public decimal idecFedralAmt { get; set; }
            public decimal idecStateAmt { get; set; }
            public decimal idecNetAmt { get; set; }
            public string istrStatus { get; set; }
            public int iintcount { get; set; }
            public int iintYear { get; set; }
            public string istrPlan { get; set; }
        }

        public class busPaymentDirectiveRecon
        {
            public string istrCategory { get; set; }
            public decimal idecPayeeAccountGrossAmount { get; set; }
            public decimal idecPaymentDirectivesGrossAmount { get; set; }
            public decimal idecDifference { get; set; }
            public DateTime idtPaymentDate { get; set; }
        }
    }
}
