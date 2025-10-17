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
using Sagitec.ExceptionPub;
using System.Xml;
using System.IO;
using System.Web;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MPIPHP.BusinessObjects.PayeeAccount
{
    [Serializable]
    public class busEDDOutboundFile : busFileBaseOut
    {
        #region Properties
        busBase lobjBase = new busBase();
        public busSystemManagement iobjSystemManagement { get; set; }
        public string istrEmployerName { get; set; }
        public string istrEmployerStrAdd { get; set; }
        public string istrEmployerState { get; set; }
        public string istrZipCodeExtension { get; set; }
        public string istrZipCode { get; set; }
        public string istrStateCode { get; set; }
        public string istrEmployerCity { get; set; }
        public string istrStateEmployerAccNo { get; set; }
        public string istrWagePlanCode { get; set; }
        public int aintPlan { get; set; }
        public string istrReportingYear { get; set; }
        public string istrPlanIdentifierValue { get; set; }
        public bool iblnIsTrue { get; set; }

        public DataTable dtReport { get; set; }
        public DataTable dtPaymentEDDFileReport { get; set; }
        public int lintQuarter { get; set; }
        public int lintTaxYear { get; set; }
        DateTime adtGivenDate, idtStartDate, idtEndDate;
        DataTable adtbRepayment;
        public XmlDocument ixmlDoc { get; set; }
        public XmlElement inodePayroll { get; set; }
        bool iblnIsIAP { get; set; }
        public busEDDReportData EDDReportData;
        public Collection<busEDDFileData> iclbEDDFileData
        {
            get;
            set;
        }

        public Collection<busEDDFileData> iclbEDDFileForReport
        {
            get;
            set;
        }

        #endregion
        public void LoadEDDFileData(DataTable adtbPensionActuaryData)
        {
            Initialize(Convert.ToString(iarrParameters[0]), (busEDDReportData)iarrParameters[1], Convert.ToDateTime(iarrParameters[2]), Convert.ToDateTime(iarrParameters[3]));
            LoadConstants();

            istrFileName = "PaymentEDDFile" + "_" + EDDReportData.StateCode + "_" + istrPlanIdentifierValue + "_" + DateTime.Now.ToString(busConstant.DateFormat) + ".csv";

            GenerateEDDFileData();
            LoadFooter();
        }
        public void Initialize(string planIdentifierValue, busEDDReportData objEDDReportData, DateTime dtStartDate, DateTime dtEndDate)
        {
            istrPlanIdentifierValue = planIdentifierValue;
            EDDReportData = objEDDReportData;
            idtStartDate = dtStartDate;
            idtEndDate = dtEndDate;
            if (EDDReportData.EDDFileReportData == null)
            {
                EDDReportData.EDDFileReportData = new DataTable();
                EDDReportData.PaymentEDDFilePensionData = new DataTable();
                EDDReportData.PaymentEDDFileIAPData = new DataTable();
            }
            dtReport = EDDReportData.EDDFileReportData;
            if (istrPlanIdentifierValue == busConstant.PENSION)
                dtPaymentEDDFileReport =  EDDReportData.PaymentEDDFilePensionData;
            else if (istrPlanIdentifierValue == busConstant.IAP)
                dtPaymentEDDFileReport = EDDReportData.PaymentEDDFileIAPData;

            adtGivenDate = DateTime.Now;
            lintQuarter = 1;

            if (idtStartDate != DateTime.MinValue)
            {
                lintTaxYear = idtStartDate.Year;
            }
            if (idtStartDate.Month >= 4 && idtEndDate.Month <= 6)
            {
                lintQuarter = 2;
                //idtStartDate = new DateTime(adtGivenDate.Year, 1, 1);
                //idtEndDate = new DateTime(adtGivenDate.Year, 3, 1);
                istrReportingYear = "06" + adtGivenDate.Year.ToString();
                //idtEffectiveDate = new DateTime(adtGivenDate.Year, 3, DateTime.DaysInMonth(adtGivenDate.Year, 3));
            }
            else if (idtStartDate.Month >= 7 && idtEndDate.Month <= 9)
            {
                lintQuarter = 3;
                //idtStartDate = new DateTime(adtGivenDate.Year, 4, 1);
                //idtEndDate = new DateTime(adtGivenDate.Year, 6, 1);
                istrReportingYear = "09" + adtGivenDate.Year.ToString();
                //idtEffectiveDate = new DateTime(adtGivenDate.Year, 6, DateTime.DaysInMonth(adtGivenDate.Year, 6));
            }
            else if (idtStartDate.Month >= 10 && idtEndDate.Month <= 12)
            {
                lintQuarter = 4;
                //idtStartDate = new DateTime(adtGivenDate.Year, 7, 1);
                //idtEndDate = new DateTime(adtGivenDate.Year, 9, 1);
                istrReportingYear = "12" + adtGivenDate.Year.ToString();
                //idtEffectiveDate = new DateTime(adtGivenDate.Year, 9, DateTime.DaysInMonth(adtGivenDate.Year, 9));
            }
            else if (idtStartDate.Month >= 1 && idtEndDate.Month <= 4)
            {
                lintQuarter = 1;
                //idtStartDate = new DateTime(adtGivenDate.Year-1, 9, 1);
                //idtEndDate = new DateTime(adtGivenDate.Year-1, 12, 1);
                istrReportingYear = "03" + adtGivenDate.Year.ToString();
                //idtEffectiveDate = new DateTime(adtGivenDate.Year, 9, DateTime.DaysInMonth(adtGivenDate.Year, 9));
            }
            else
            {
                lintQuarter = 1;
                istrReportingYear = "0" + adtGivenDate.Year.ToString();
            }
        }
        public void GenerateEDDFileData()
        {
            string lstrReportPath = string.Empty;
            try
            {
                LoadEddFileCollectionForQuaters(false);
                LoadEddFileCollectionForQuaters(true);

                string lstrnQuaterName = string.Empty;
                if (lintQuarter == 1)
                {
                    lstrnQuaterName = "1st Quarter  " + idtStartDate.Year;
                }
                else
                    if (lintQuarter == 2)
                {
                    lstrnQuaterName = "2nd Quarter  " + idtStartDate.Year;
                }
                else
                        if (lintQuarter == 3)
                {
                    lstrnQuaterName = "3rd Quarter  " + idtStartDate.Year;
                }
                else
                            if (lintQuarter == 4)
                {
                    lstrnQuaterName = "4th Quarter  " + idtStartDate.Year;
                }

                CreateEddFileReportDataTable();
                if (istrPlanIdentifierValue == "IAP")
                {
                    DataRow dr = dtReport.NewRow();
                    dr["Plan_Name"] = "IAP";
                    dr["Gross_Amount"] = iclbEDDFileData.Where(obj => obj.aintPlan == 1).Sum(obj => obj.idecGrossAmt);
                    dr["State_Amount"] = iclbEDDFileData.Where(obj => obj.aintPlan == 1).Sum(obj => obj.idecStateTax);
                    dr["Fed_Tax"] = iclbEDDFileData.Where(obj => obj.aintPlan == 1).Sum(obj => obj.idecFedAMt);
                    dr["Net_Amount"] = iclbEDDFileData.Where(obj => obj.aintPlan == 1).Sum(obj => obj.idecNetAMt);
                    dr["Count"] = iclbEDDFileData.Where(obj => obj.aintPlan == 1).Count();
                    dr["Quater"] = lstrnQuaterName;
                    dtReport.Rows.Add(dr);
                }
                else
                {
                    DataRow dr2 = dtReport.NewRow();
                    dr2["Plan_Name"] = "PENSION";
                    dr2["Gross_Amount"] = iclbEDDFileData.Where(obj => obj.aintPlan != 1 && obj.istrWithdralFlag != "Y").Sum(obj => obj.idecGrossAmt);
                    dr2["State_Amount"] = iclbEDDFileData.Where(obj => obj.aintPlan != 1 && obj.istrWithdralFlag != "Y").Sum(obj => obj.idecStateTax);
                    dr2["Fed_Tax"] = iclbEDDFileData.Where(obj => obj.aintPlan != 1 && obj.istrWithdralFlag != "Y").Sum(obj => obj.idecFedAMt);
                    dr2["Net_Amount"] = iclbEDDFileData.Where(obj => obj.aintPlan != 1 && obj.istrWithdralFlag != "Y").Sum(obj => obj.idecNetAMt);
                    dr2["Count"] = iclbEDDFileData.Where(obj => obj.aintPlan != 1).Count();
                    dr2["Quater"] = lstrnQuaterName;
                    dtReport.Rows.Add(dr2);
                }

                bool lblnIsPresent = false;
                DataRow drRefund = null;
                if (dtReport.Rows.Count > 0 && dtReport.FilterTable(utlDataType.String, "Plan_Name", "REFUND").Count() > 0)
                {
                    drRefund = dtReport.FilterTable(utlDataType.String, "Plan_Name", "REFUND").FirstOrDefault();
                    lblnIsPresent = true;
                }
                else
                {
                    drRefund = dtReport.NewRow();
                }

                drRefund["Plan_Name"] = "REFUND";
                drRefund["Gross_Amount"] = (drRefund["Gross_Amount"] != DBNull.Value ? Convert.ToDecimal(drRefund["Gross_Amount"]) : 0) + iclbEDDFileData.Where(obj => obj.istrWithdralFlag == "Y" && obj.aintPlan != 1).Sum(obj => obj.idecGrossAmt);
                drRefund["State_Amount"] = (drRefund["State_Amount"] != DBNull.Value ? Convert.ToDecimal(drRefund["State_Amount"]) : 0) + iclbEDDFileData.Where(obj => obj.istrWithdralFlag == "Y" && obj.aintPlan != 1).Sum(obj => obj.idecStateTax);
                drRefund["Fed_Tax"] = (drRefund["Fed_Tax"] != DBNull.Value ? Convert.ToDecimal(drRefund["Fed_Tax"]) : 0) + iclbEDDFileData.Where(obj => obj.istrWithdralFlag == "Y" && obj.aintPlan != 1).Sum(obj => obj.idecFedAMt);
                drRefund["Net_Amount"] = (drRefund["Net_Amount"] != DBNull.Value ? Convert.ToDecimal(drRefund["Net_Amount"]) : 0) + iclbEDDFileData.Where(obj => obj.istrWithdralFlag == "Y" && obj.aintPlan != 1).Sum(obj => obj.idecNetAMt);
                drRefund["Count"] = (drRefund["Count"] != DBNull.Value ? Convert.ToInt32(drRefund["Count"]) : 0) + iclbEDDFileData.Where(obj => obj.istrWithdralFlag == "Y" && obj.aintPlan != 1).Count();
                drRefund["Quater"] = lstrnQuaterName;
                if (lblnIsPresent)
                {
                    dtReport.AcceptChanges();
                }
                else
                {
                    dtReport.Rows.Add(drRefund);
                }

                //iclbEDDFileForReport = new Collection<busEDDFileData>(iclbEDDFileData);
                CreateEDDFileCollection();
                CreatePaymentEDDFileReportDataTable();
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }
        public static void CreateEddFileReport(string stateCode, DataTable dtReport)
        {
            try
            {
                busCreateReports lobjCreateReports = new busCreateReports();
                lobjCreateReports.CreatePDFReport(dtReport, "rptEddFileReport", string.Empty, stateCode);
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }
        public static void CreateEDDExceptionReport(DateTime idtStartDate, DateTime idtEndDate)
        {
            DataTable ldtbPersonData = new DataTable();
            DataTable ldtbOrgData = new DataTable();
            DataTable ldtbPersonIAPData = new DataTable();
            DataTable ldtbOrgIAPData = new DataTable();
            DataTable ldtFinalTable = new DataTable();

            ldtbPersonData = busBase.Select("cdoPayment1099r.GetPersonsForEDDReport", new object[3] { busConstant.PENSION, idtEndDate, idtStartDate });
            ldtbOrgData = busBase.Select("cdoPayment1099r.GetOrgsForEDDReport", new object[3] { busConstant.PENSION, idtEndDate, idtStartDate });

            ldtFinalTable = ldtbPersonData.Copy();
            ldtFinalTable.Merge(ldtbOrgData, true, MissingSchemaAction.Ignore);

            ldtbPersonIAPData = busBase.Select("cdoPayment1099r.GetPersonsForEDDReport", new object[3] { busConstant.IAP, idtEndDate, idtStartDate });
            ldtbOrgIAPData = busBase.Select("cdoPayment1099r.GetOrgsForEDDReport", new object[3] { busConstant.IAP, idtEndDate, idtStartDate });

            ldtFinalTable.Merge(ldtbPersonIAPData, true, MissingSchemaAction.Ignore);
            ldtFinalTable.Merge(ldtbOrgIAPData, true, MissingSchemaAction.Ignore);

            ldtFinalTable.TableName = "rptEDDExceptionReport";
            busCreateReports lbusCreateReports = new busCreateReports();
            lbusCreateReports.CreateExcelReport(ldtFinalTable, "rptEDDExceptionReport");
        }
        public static void CreatePaymentEDDFileReport(string templatePath, string reportPath, DataSet dsReportData)
        {
            if (dsReportData.Tables.Count > 0)
            {
                busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                lbusExcelReportGenerator.CreateExcelReport(templatePath, reportPath, dsReportData.Tables[0].TableName, dsReportData);
            }
        }
        public static void CreateOregonPaymentEDDFileReport(string templatePath, string reportPath, DataTable dtReport)
        {
            DataSet dsReportData = new DataSet();
            DataTable dtReportData = dtReport.AsEnumerable()
                                        .Where(r => r.Field<decimal>("STATE_TAX_AMOUNT") > 0)
                                        .CopyToDataTable();
            dsReportData.Tables.Add(dtReportData);
            if (dsReportData.Tables.Count > 0)
            {
                busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
                lbusExcelReportGenerator.CreateExcelReport(templatePath, reportPath, "EmployeeDetailReport", dsReportData);
            }
        }
        public void LoadEddFileCollectionForQuaters(bool IsOrganization)
        {
            DataTable adtbPensionActuaryData = null;
            if (!IsOrganization)
            {
                adtbPensionActuaryData = busBase.Select("cdoPayment1099r.GetPersonForEdd", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
            }
            else
            {
                adtbPensionActuaryData = busBase.Select("cdoPayment1099r.GetOrgForEdd", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
            }

            adtbRepayment = new DataTable();

            if (lintQuarter == 1 || lintQuarter == 4)
            {
                if (!IsOrganization)
                {
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithPerson", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                }
                else
                {
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithOrg", new object[3] { idtStartDate, idtEndDate, istrPlanIdentifierValue });
                }
                List<busPersonGross> PersonGrossAmt = (from obj in adtbPensionActuaryData.AsEnumerable()
                                                       group obj by new
                                                       {
                                                           PERSON_ID = obj.Field<Int32>("PERSON_ID"),
                                                           WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                       } into objgp
                                                       select new busPersonGross
                                                       {
                                                           PersonId = objgp.Key.PERSON_ID,
                                                           GrossAmt = objgp.Sum(i => i.Field<decimal>("GROSS_AMOUNT")),
                                                           amtpaid = (from obj in adtbRepayment.AsEnumerable()
                                                                      where
                                                                          obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                          && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                      select new
                                                                      {
                                                                          amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                      }).FirstOrDefault() != null
                                                                     ?
                                                                     (from obj in adtbRepayment.AsEnumerable()
                                                                      where
                                                                       obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                          && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                      select new
                                                                      {
                                                                          amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                      }).FirstOrDefault().amtPaid
                                                                     : 0M,
                                                           amtpaid_state_tax = (from obj in adtbRepayment.AsEnumerable()
                                                                                where
                                                                                    obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                                    && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                                select new
                                                                                {
                                                                                    amtPaid = obj.Field<decimal>("STATE_TAX")
                                                                                }).FirstOrDefault() != null
                                                                      ?
                                                                      (from obj in adtbRepayment.AsEnumerable()
                                                                       where
                                                                        obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                           && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                       select new
                                                                       {
                                                                           amtPaid = obj.Field<decimal>("STATE_TAX")
                                                                       }).FirstOrDefault().amtPaid
                                                                      : 0M,
                                                           remainGrs = objgp.Sum(i => i.Field<decimal>("GROSS_AMOUNT"))
                                                                       -
                                                                       ((from obj in adtbRepayment.AsEnumerable()
                                                                         where
                                                                            obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                             && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG

                                                                         select new
                                                                         {
                                                                             amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                         }).FirstOrDefault() != null
                                                                      ?
                                                                      (from obj in adtbRepayment.AsEnumerable()
                                                                       where
                                                                        obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                           && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                       select new
                                                                       {
                                                                           amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                       }).FirstOrDefault().amtPaid
                                                                      : 0M),
                                                           istrWithdralFlag = objgp.Key.WDRL_FLAG,
                                                           istrWithdralFlagRepayment =
                                                                      ((from obj in adtbRepayment.AsEnumerable()
                                                                        where
                                                                           obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                            && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG

                                                                        select new
                                                                        {
                                                                            WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                                        }).FirstOrDefault() != null
                                                                     ?
                                                                     (from obj in adtbRepayment.AsEnumerable()
                                                                      where
                                                                       obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                          && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                      select new
                                                                      {
                                                                          WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                                      }).FirstOrDefault().WDRL_FLAG
                                                                     : "0"),

                                                       }
                                    ).ToList();
                //PersonGrossAmt = PersonGrossAmt.Select(obj => obj.GrossAmt = obj.GrossAmt - obj.amtpaid);
                //Old code
                //List<DataRow> drcol = (from obj in adtbPensionActuaryData.AsEnumerable()
                //                       where !PersonGrossAmt.Where(amt => amt.GrossAmt > amt.amtpaid && amt.istrWithdralFlag == amt.istrWithdralFlagRepayment).Select(pe => pe.PersonId).Contains(obj.Field<Int32>("PERSON_ID"))
                //                       select obj).ToList();
                List<DataRow> drcol = (from obj in adtbPensionActuaryData.AsEnumerable()
                                       where !PersonGrossAmt.Where(amt => amt.GrossAmt < amt.amtpaid && amt.istrWithdralFlag == amt.istrWithdralFlagRepayment).Select(pe => pe.PersonId).Contains(obj.Field<Int32>("PERSON_ID"))
                                       select obj).ToList();
                LoadEddFileCollection(drcol, PersonGrossAmt, IsOrganization);
            }
            else if (lintQuarter == 2)
            {
                if (!IsOrganization)
                {
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithPerson", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                }
                else
                {
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithOrg", new object[3] { idtStartDate, idtEndDate, istrPlanIdentifierValue });
                }
                List<busPersonGross> PersonGrossAmt = (from obj in adtbPensionActuaryData.AsEnumerable()
                                                       group obj by new
                                                       {
                                                           PERSON_ID = obj.Field<Int32>("PERSON_ID"),
                                                           WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                       } into objgp
                                                       select new busPersonGross
                                                       {
                                                           PersonId = objgp.Key.PERSON_ID,
                                                           GrossAmt = objgp.Sum(i => i.Field<decimal>("GROSS_AMOUNT")),
                                                           amtpaid = (from obj in adtbRepayment.AsEnumerable()
                                                                      where
                                                                          obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                          && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                      select new
                                                                      {
                                                                          amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                      }).FirstOrDefault() != null
                                                                     ?
                                                                     (from obj in adtbRepayment.AsEnumerable()
                                                                      where
                                                                       obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                          && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                      select new
                                                                      {
                                                                          amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                      }).FirstOrDefault().amtPaid
                                                                     : 0M,
                                                           amtpaid_state_tax = (from obj in adtbRepayment.AsEnumerable()
                                                                                where
                                                                                    obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                                    && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                                select new
                                                                                {
                                                                                    amtPaid = obj.Field<decimal>("STATE_TAX")
                                                                                }).FirstOrDefault() != null
                                                                                ?
                                                                                (from obj in adtbRepayment.AsEnumerable()
                                                                                 where
                                                                                obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                                  && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                                 select new
                                                                                 {
                                                                                     amtPaid = obj.Field<decimal>("STATE_TAX")
                                                                                 }).FirstOrDefault().amtPaid
                                                                                    : 0M,
                                                           remainGrs = objgp.Sum(i => i.Field<decimal>("GROSS_AMOUNT"))
                                                                       -
                                                                       ((from obj in adtbRepayment.AsEnumerable()
                                                                         where
                                                                            obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                             && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG

                                                                         select new
                                                                         {
                                                                             amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                         }).FirstOrDefault() != null
                                                                      ?
                                                                      (from obj in adtbRepayment.AsEnumerable()
                                                                       where
                                                                        obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                           && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                       select new
                                                                       {
                                                                           amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                       }).FirstOrDefault().amtPaid
                                                                      : 0M),
                                                           istrWithdralFlag = objgp.Key.WDRL_FLAG,
                                                           istrWithdralFlagRepayment =
                                                    ((from obj in adtbRepayment.AsEnumerable()
                                                      where
                                                         obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                          && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG

                                                      select new
                                                      {
                                                          WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                      }).FirstOrDefault() != null
                                                   ?
                                                   (from obj in adtbRepayment.AsEnumerable()
                                                    where
                                                     obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                        && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                    select new
                                                    {
                                                        WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                    }).FirstOrDefault().WDRL_FLAG
                                                   : "0"),

                                                       }
                                    ).ToList();


                //For last quater
                idtStartDate = new DateTime(adtGivenDate.Year, 1, 1);
                idtEndDate = new DateTime(adtGivenDate.Year, 3, 1);
                DataTable adtbOldPensionActuaryData = new DataTable();
                if (!IsOrganization)
                {
                    adtbOldPensionActuaryData = busBase.Select("cdoPayment1099r.GetPersonForEdd", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithPerson", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                }
                else
                {
                    adtbOldPensionActuaryData = busBase.Select("cdoPayment1099r.GetOrgForEdd", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithOrg", new object[3] { idtStartDate, idtEndDate, istrPlanIdentifierValue });
                }

                List<busPersonGross> LstPersonGrossAmt = (from obj in adtbOldPensionActuaryData.AsEnumerable()
                                                          group obj by new
                                                          {
                                                              PERSON_ID = obj.Field<Int32>("PERSON_ID"),
                                                              WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                          } into objgp
                                                          select new busPersonGross
                                                          {
                                                              PersonId = objgp.Key.PERSON_ID,
                                                              GrossAmt = objgp.Sum(i => i.Field<decimal>("GROSS_AMOUNT")),
                                                              amtpaid = (from obj in adtbRepayment.AsEnumerable()
                                                                         where
                                                                             obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                             && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                         select new
                                                                         {
                                                                             amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                         }).FirstOrDefault() != null
                                                                        ?
                                                                        (from obj in adtbRepayment.AsEnumerable()
                                                                         where
                                                                          obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                             && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                         select new
                                                                         {
                                                                             amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                         }).FirstOrDefault().amtPaid
                                                                        : 0M,
                                                              amtpaid_state_tax = (from obj in adtbRepayment.AsEnumerable()
                                                                                   where
                                                                                       obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                                       && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                                   select new
                                                                                   {
                                                                                       amtPaid = obj.Field<decimal>("STATE_TAX")
                                                                                   }).FirstOrDefault() != null
                                                                      ?
                                                                      (from obj in adtbRepayment.AsEnumerable()
                                                                       where
                                                                      obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                        && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                       select new
                                                                       {
                                                                           amtPaid = obj.Field<decimal>("STATE_TAX")
                                                                       }).FirstOrDefault().amtPaid
                                                                          : 0M,
                                                              istrWithdralFlag = objgp.Key.WDRL_FLAG,
                                                              istrWithdralFlagRepayment =
                                                       ((from obj in adtbRepayment.AsEnumerable()
                                                         where
                                                            obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                             && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG

                                                         select new
                                                         {
                                                             WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                         }).FirstOrDefault() != null
                                                      ?
                                                      (from obj in adtbRepayment.AsEnumerable()
                                                       where
                                                        obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                           && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                       select new
                                                       {
                                                           WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                       }).FirstOrDefault().WDRL_FLAG
                                                      : "0"),
                                                          }
                                                      ).ToList();
                LstPersonGrossAmt = LstPersonGrossAmt.Where(obj => obj.GrossAmt - obj.amtpaid < 0).ToList();
                if (LstPersonGrossAmt.Count > 0)
                {
                    List<busPersonGross> RemainperGross = (from c in PersonGrossAmt
                                                           join o in LstPersonGrossAmt on
                                                           new
                                                           {
                                                               c.PersonId,
                                                               c.istrWithdralFlag
                                                           }
                                                           equals
                                                           new
                                                           {
                                                               o.PersonId,
                                                               o.istrWithdralFlag
                                                           }
                                                           select new busPersonGross
                                                           {
                                                               PersonId = c.PersonId,
                                                               GrossAmt = (c.GrossAmt - c.amtpaid) - (o.GrossAmt - o.amtpaid),



                                                           }).ToList();
                    foreach (var per in LstPersonGrossAmt)
                    {
                        var buspergrs = PersonGrossAmt.Where(obj => obj.PersonId == per.PersonId).FirstOrDefault();
                        if (buspergrs != null)
                        {
                            //per.GrossAmt = buspergrs.GrossAmt;
                            per.remainGrs = buspergrs.GrossAmt;
                        }

                    }
                }
                List<DataRow> drcol = (from obj in adtbPensionActuaryData.AsEnumerable()
                                       where !PersonGrossAmt.Where(amt => amt.GrossAmt < amt.amtpaid && amt.istrWithdralFlag == amt.istrWithdralFlagRepayment).Select(pe => pe.PersonId).Contains(obj.Field<Int32>("PERSON_ID"))
                                       select obj).ToList();
                LoadEddFileCollection(drcol, PersonGrossAmt, IsOrganization);
            }
            else if (lintQuarter == 3)
            {
                if (!IsOrganization)
                {
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithPerson", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                }
                else
                {
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithOrg", new object[3] { idtStartDate, idtEndDate, istrPlanIdentifierValue });
                }
                List<busPersonGross> PersonGrossAmt = (from obj in adtbPensionActuaryData.AsEnumerable()
                                                       group obj by new
                                                       {
                                                           PERSON_ID = obj.Field<Int32>("PERSON_ID"),
                                                           WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                       } into objgp
                                                       select new busPersonGross
                                                       {
                                                           PersonId = objgp.Key.PERSON_ID,
                                                           GrossAmt = objgp.Sum(i => i.Field<decimal>("GROSS_AMOUNT")),
                                                           amtpaid = (from obj in adtbRepayment.AsEnumerable()
                                                                      where
                                                                          obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                          && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                      select new
                                                                      {
                                                                          amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                      }).FirstOrDefault() != null
                                                                     ?
                                                                     (from obj in adtbRepayment.AsEnumerable()
                                                                      where
                                                                       obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                          && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                      select new
                                                                      {
                                                                          amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                      }).FirstOrDefault().amtPaid
                                                                     : 0M,
                                                           amtpaid_state_tax = (from obj in adtbRepayment.AsEnumerable()
                                                                                where
                                                                                    obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                                    && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                                select new
                                                                                {
                                                                                    amtPaid = obj.Field<decimal>("STATE_TAX")
                                                                                }).FirstOrDefault() != null
                                                                      ?
                                                                      (from obj in adtbRepayment.AsEnumerable()
                                                                       where
                                                                      obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                        && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                       select new
                                                                       {
                                                                           amtPaid = obj.Field<decimal>("STATE_TAX")
                                                                       }).FirstOrDefault().amtPaid
                                                                          : 0M,
                                                           remainGrs = objgp.Sum(i => i.Field<decimal>("GROSS_AMOUNT"))
                                                                       -
                                                                       ((from obj in adtbRepayment.AsEnumerable()
                                                                         where
                                                                            obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                             && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG

                                                                         select new
                                                                         {
                                                                             amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                         }).FirstOrDefault() != null
                                                                      ?
                                                                      (from obj in adtbRepayment.AsEnumerable()
                                                                       where
                                                                        obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                           && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                       select new
                                                                       {
                                                                           amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                       }).FirstOrDefault().amtPaid
                                                                      : 0M),
                                                           istrWithdralFlag = objgp.Key.WDRL_FLAG,
                                                           istrWithdralFlagRepayment =
                                                    ((from obj in adtbRepayment.AsEnumerable()
                                                      where
                                                         obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                          && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG

                                                      select new
                                                      {
                                                          WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                      }).FirstOrDefault() != null
                                                   ?
                                                   (from obj in adtbRepayment.AsEnumerable()
                                                    where
                                                     obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                        && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                    select new
                                                    {
                                                        WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                    }).FirstOrDefault().WDRL_FLAG
                                                   : "0"),

                                                       }
                                                   ).ToList();


                //For last quater
                idtStartDate = new DateTime(adtGivenDate.Year, 4, 1);
                idtEndDate = new DateTime(adtGivenDate.Year, 6, 1);
                DataTable adtbOldPensionActuaryData = new DataTable();
                if (!IsOrganization)
                {
                    adtbOldPensionActuaryData = busBase.Select("cdoPayment1099r.GetPersonForEdd", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithPerson", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                }
                else
                {
                    adtbOldPensionActuaryData = busBase.Select("cdoPayment1099r.GetOrgForEdd", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithOrg", new object[3] { idtStartDate, idtEndDate, istrPlanIdentifierValue });
                }

                List<busPersonGross> LstPersonGrossAmt = (from obj in adtbOldPensionActuaryData.AsEnumerable()
                                                          group obj by new
                                                          {
                                                              PERSON_ID = obj.Field<Int32>("PERSON_ID"),
                                                              WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                          } into objgp
                                                          select new busPersonGross
                                                          {
                                                              PersonId = objgp.Key.PERSON_ID,
                                                              GrossAmt = objgp.Sum(i => i.Field<decimal>("GROSS_AMOUNT")),
                                                              amtpaid = (from obj in adtbRepayment.AsEnumerable()
                                                                         where
                                                                             obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                             && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                         select new
                                                                         {
                                                                             amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                         }).FirstOrDefault() != null
                                                                        ?
                                                                        (from obj in adtbRepayment.AsEnumerable()
                                                                         where
                                                                          obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                             && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                         select new
                                                                         {
                                                                             amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                                         }).FirstOrDefault().amtPaid
                                                                        : 0M,
                                                              amtpaid_state_tax = (from obj in adtbRepayment.AsEnumerable()
                                                                                   where
                                                                                       obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                                       && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                                   select new
                                                                                   {
                                                                                       amtPaid = obj.Field<decimal>("STATE_TAX")
                                                                                   }).FirstOrDefault() != null
                                                                      ?
                                                                      (from obj in adtbRepayment.AsEnumerable()
                                                                       where
                                                                      obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                        && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                       select new
                                                                       {
                                                                           amtPaid = obj.Field<decimal>("STATE_TAX")
                                                                       }).FirstOrDefault().amtPaid
                                                                          : 0M,
                                                              istrWithdralFlag = objgp.Key.WDRL_FLAG,
                                                              istrWithdralFlagRepayment =
                                                       ((from obj in adtbRepayment.AsEnumerable()
                                                         where
                                                            obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                             && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG

                                                         select new
                                                         {
                                                             WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                         }).FirstOrDefault() != null
                                                      ?
                                                      (from obj in adtbRepayment.AsEnumerable()
                                                       where
                                                        obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                           && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                       select new
                                                       {
                                                           WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                                       }).FirstOrDefault().WDRL_FLAG
                                                      : "0"),
                                                          }
                                                      ).ToList();
                LstPersonGrossAmt = LstPersonGrossAmt.Where(obj => obj.GrossAmt - obj.amtpaid < 0).ToList();
                List<busPersonGross> RemainperGross = (from c in PersonGrossAmt
                                                       join o in LstPersonGrossAmt on
                                                       new
                                                       {
                                                           c.PersonId,
                                                           c.istrWithdralFlag
                                                       }
                                                       equals
                                                       new
                                                       {
                                                           o.PersonId,
                                                           o.istrWithdralFlag
                                                       }
                                                       select new busPersonGross
                                                       {
                                                           PersonId = c.PersonId,
                                                           GrossAmt = (c.GrossAmt - c.amtpaid) - (o.GrossAmt - o.amtpaid),



                                                       }).ToList();
                foreach (var per in RemainperGross)
                {
                    var buspergrs = PersonGrossAmt.Where(obj => obj.PersonId == per.PersonId).FirstOrDefault();
                    if (buspergrs != null)
                    {
                        per.GrossAmt = buspergrs.GrossAmt;
                        per.remainGrs = buspergrs.GrossAmt;
                    }

                }
                // fro last to last quater

                idtStartDate = new DateTime(adtGivenDate.Year, 1, 1);
                idtEndDate = new DateTime(adtGivenDate.Year, 3, 1);
                if (!IsOrganization)
                {
                    adtbOldPensionActuaryData = busBase.Select("cdoPayment1099r.GetPersonForEdd", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithPerson", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                }
                else
                {
                    adtbOldPensionActuaryData = busBase.Select("cdoPayment1099r.GetOrgForEdd", new object[4] { idtStartDate, idtEndDate, istrPlanIdentifierValue, EDDReportData.StateCode });
                    adtbRepayment = busBase.Select("cdoPayment1099r.GetRepaymentWithOrg", new object[3] { idtStartDate, idtEndDate, istrPlanIdentifierValue });
                }
                LstPersonGrossAmt = (from obj in adtbOldPensionActuaryData.AsEnumerable()
                                     group obj by new
                                     {
                                         PERSON_ID = obj.Field<Int32>("PERSON_ID"),
                                         WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                     } into objgp
                                     select new busPersonGross
                                     {
                                         PersonId = objgp.Key.PERSON_ID,
                                         GrossAmt = objgp.Sum(i => i.Field<decimal>("GROSS_AMOUNT")),
                                         amtpaid = (from obj in adtbRepayment.AsEnumerable()
                                                    where
                                                        obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                        && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                    select new
                                                    {
                                                        amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                    }).FirstOrDefault() != null
                                                   ?
                                                   (from obj in adtbRepayment.AsEnumerable()
                                                    where
                                                     obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                        && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                    select new
                                                    {
                                                        amtPaid = obj.Field<decimal>("AMOUNT_PAID")
                                                    }).FirstOrDefault().amtPaid
                                                   : 0M,
                                         amtpaid_state_tax = (from obj in adtbRepayment.AsEnumerable()
                                                              where
                                                                  obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                  && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                              select new
                                                              {
                                                                  amtPaid = obj.Field<decimal>("STATE_TAX")
                                                              }).FirstOrDefault() != null
                                                                      ?
                                                                      (from obj in adtbRepayment.AsEnumerable()
                                                                       where
                                                                      obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                                                        && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                                                       select new
                                                                       {
                                                                           amtPaid = obj.Field<decimal>("STATE_TAX")
                                                                       }).FirstOrDefault().amtPaid
                                                                          : 0M,
                                         istrWithdralFlag = objgp.Key.WDRL_FLAG,
                                         istrWithdralFlagRepayment =
                                  ((from obj in adtbRepayment.AsEnumerable()
                                    where
                                       obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                        && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG

                                    select new
                                    {
                                        WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                    }).FirstOrDefault() != null
                                 ?
                                 (from obj in adtbRepayment.AsEnumerable()
                                  where
                                   obj.Field<Int32>("PERSON_ID") == objgp.Key.PERSON_ID
                                      && obj.Field<string>("WDRL_FLAG") == objgp.Key.WDRL_FLAG
                                  select new
                                  {
                                      WDRL_FLAG = obj.Field<string>("WDRL_FLAG")
                                  }).FirstOrDefault().WDRL_FLAG
                                 : "0"),
                                     }
                                                      ).ToList();
                LstPersonGrossAmt = LstPersonGrossAmt.Where(obj => obj.GrossAmt - obj.amtpaid < 0).ToList();
                RemainperGross = (from c in PersonGrossAmt
                                  join o in LstPersonGrossAmt on
                                    new
                                    {
                                        c.PersonId,
                                        c.istrWithdralFlag
                                    }
                                    equals
                                    new
                                    {
                                        o.PersonId,
                                        o.istrWithdralFlag
                                    }
                                  select new busPersonGross
                                  {
                                      PersonId = c.PersonId,
                                      GrossAmt = (c.GrossAmt - c.amtpaid) - (o.GrossAmt - o.amtpaid),



                                  }).ToList();
                foreach (var per in RemainperGross)
                {
                    var buspergrs = PersonGrossAmt.Where(obj => obj.PersonId == per.PersonId).FirstOrDefault();
                    if (buspergrs != null)
                    {
                        //per.GrossAmt = buspergrs.GrossAmt;
                        per.remainGrs = buspergrs.GrossAmt;
                    }

                }
                List<DataRow> drcol = (from obj in adtbPensionActuaryData.AsEnumerable()
                                       //where !PersonGrossAmt.Where(amt => amt.remainGrs > 0).Select(pe => pe.PersonId).Contains(obj.Field<Int32>("PERSON_ID"))
                                       select obj).ToList();
                LoadEddFileCollection(drcol, PersonGrossAmt, IsOrganization);


            }

        }
        void LoadEddFileCollection(List<DataRow> drcol, List<busPersonGross> PersonGrossAmt, bool IsOrganization)
        {
            if (!IsOrganization)
            {
                iclbEDDFileData = new Collection<busEDDFileData>();
            }
            foreach (DataRow dr in drcol)
            {
                busEDDFileData lbusEDDFileData = new busEDDFileData();
                if (dr["SSN"] != DBNull.Value && Convert.ToString(dr["SSN"]).IsNotNullOrEmpty())
                {
                    lbusEDDFileData.ssn = Convert.ToString(dr["SSN"]);
                }
                else
                {
                    lbusEDDFileData.ssn = "I";
                }

                lbusEDDFileData.istrLastName = Regex.Replace(Convert.ToString(dr["LAST_NAME"]), "[^a-zA-Z ]", "");
                lbusEDDFileData.istrLastName = lbusEDDFileData.istrLastName.Substring(0, lbusEDDFileData.istrLastName.Length > 30 ? 30 : lbusEDDFileData.istrLastName.Length);

                lbusEDDFileData.istrFirstName = Regex.Replace(Convert.ToString(dr["FIRST_NAME"]), "[^a-zA-Z ]", "");
                lbusEDDFileData.istrFirstName = lbusEDDFileData.istrFirstName.Substring(0, lbusEDDFileData.istrFirstName.Length > 16 ? 16 : lbusEDDFileData.istrFirstName.Length);

                lbusEDDFileData.istrMiddleName = Regex.Replace(Convert.ToString(dr["MIDDLE_NAME"]), "[^a-zA-Z ]", "");
                lbusEDDFileData.istrWithdralFlag = Convert.ToString(dr["WDRL_FLAG"]);

                if (lbusEDDFileData.istrLastName.IsNullOrEmpty())
                {
                    lbusEDDFileData.istrLastName = Regex.Replace(Convert.ToString(dr["FIRST_NAME"]), "[^a-zA-Z ]", "");
                    lbusEDDFileData.istrLastName = lbusEDDFileData.istrLastName.Substring(0, lbusEDDFileData.istrLastName.Length > 30 ? 30 : lbusEDDFileData.istrLastName.Length);
                }

                //lbusEDDFileData.aintPlan = Convert.ToInt32(dr["PLAN_ID"]);
                if (istrPlanIdentifierValue != "IAP")
                {
                    lbusEDDFileData.aintPlan = 2;
                }
                else
                {
                    lbusEDDFileData.aintPlan = 1;
                }
                var buspergrs = PersonGrossAmt.Where(obj => obj.PersonId == Convert.ToInt32(dr["PERSON_ID"]) && obj.istrWithdralFlagRepayment == Convert.ToString(dr["WDRL_FLAG"])).FirstOrDefault();
                if (buspergrs != null)
                {
                    //lbusEDDFileData.idecGrossAmt = Convert.ToDecimal(dr["GROSS_AMOUNT"]) - buspergrs.amtpaid;
                    lbusEDDFileData.idecGrossAmt = buspergrs.remainGrs;
                    lbusEDDFileData.idecStateTax = Convert.ToDecimal(dr["STATE_TAX_AMOUNT"]) - buspergrs.amtpaid_state_tax;
                    lbusEDDFileData.idecFedAMt = Convert.ToDecimal(dr["FED_TAX_AMOUNT"]);
                    lbusEDDFileData.idecTaxableAMt = Convert.ToDecimal(dr["TAXABLE_AMOUNT"]);
                    lbusEDDFileData.aintMonth1Employee = Convert.ToInt32(dr["MONTH1_EMPLOYEE"]);
                    lbusEDDFileData.aintMonth2Employee = Convert.ToInt32(dr["MONTH2_EMPLOYEE"]);
                    lbusEDDFileData.aintMonth3Employee = Convert.ToInt32(dr["MONTH3_EMPLOYEE"]);

                }
                else
                {
                    lbusEDDFileData.idecGrossAmt = Convert.ToDecimal(dr["GROSS_AMOUNT"]);
                    lbusEDDFileData.idecStateTax = Convert.ToDecimal(dr["STATE_TAX_AMOUNT"]);
                    lbusEDDFileData.idecFedAMt = Convert.ToDecimal(dr["FED_TAX_AMOUNT"]);
                    lbusEDDFileData.idecTaxableAMt = Convert.ToDecimal(dr["TAXABLE_AMOUNT"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(dr["MONTH1_EMPLOYEE"])))
                    {
                        lbusEDDFileData.aintMonth1Employee = Convert.ToInt32(dr["MONTH1_EMPLOYEE"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(dr["MONTH2_EMPLOYEE"])))
                    {
                        lbusEDDFileData.aintMonth2Employee = Convert.ToInt32(dr["MONTH2_EMPLOYEE"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(dr["MONTH3_EMPLOYEE"])))
                    {
                        lbusEDDFileData.aintMonth3Employee = Convert.ToInt32(dr["MONTH3_EMPLOYEE"]);
                    }
                }
                lbusEDDFileData.idecNetAMt = lbusEDDFileData.idecGrossAmt - (lbusEDDFileData.idecStateTax + lbusEDDFileData.idecFedAMt);
                lbusEDDFileData.istrStateCode = !istrStateCode.IsNullOrEmpty() ? istrStateCode : EDDReportData.StateCode;
                lbusEDDFileData.istrStateEmployerAccNo = istrStateEmployerAccNo;
                lbusEDDFileData.istrWagePlanCode = istrWagePlanCode;
                lbusEDDFileData.istrReportingYear = istrReportingYear;
                if (lbusEDDFileData.idecGrossAmt > 0)
                {
                    iclbEDDFileData.Add(lbusEDDFileData);
                }


            }
        }
        void CreateEddFileReportDataTable()
        {
            if (dtReport.Rows.Count == 0)
            {
                dtReport.TableName = "rptEddFileReport";
                dtReport.Columns.Add("Plan_Name");
                DataColumn ldcCount = new DataColumn("Count", Type.GetType("System.Decimal"));
                dtReport.Columns.Add(ldcCount);
                DataColumn ldcGross = new DataColumn("Gross_Amount", Type.GetType("System.Decimal"));
                dtReport.Columns.Add(ldcGross);
                DataColumn ldcState = new DataColumn("State_Amount", Type.GetType("System.Decimal"));
                dtReport.Columns.Add(ldcState);
                DataColumn lstrFed = new DataColumn("Fed_Tax", Type.GetType("System.Decimal"));
                dtReport.Columns.Add(lstrFed);
                DataColumn lstrNet = new DataColumn("Net_Amount", Type.GetType("System.Decimal"));
                dtReport.Columns.Add(lstrNet);
                DataColumn lstrQuater = new DataColumn("Quater", Type.GetType("System.String"));
                dtReport.Columns.Add(lstrQuater);
            }
        }
        void CreatePaymentEDDFileReportDataTable()
        {
            if (dtPaymentEDDFileReport.Rows.Count == 0)
            {
                dtPaymentEDDFileReport.Columns.Add(new DataColumn("SSN", Type.GetType("System.String")));
                dtPaymentEDDFileReport.Columns.Add(new DataColumn("LAST_NAME", Type.GetType("System.String")));
                dtPaymentEDDFileReport.Columns.Add(new DataColumn("FIRST_NAME", Type.GetType("System.String")));
                dtPaymentEDDFileReport.Columns.Add(new DataColumn("MIDDLE_NAME", Type.GetType("System.String")));
                dtPaymentEDDFileReport.Columns.Add(new DataColumn("STATE_CODE", Type.GetType("System.String")));
                dtPaymentEDDFileReport.Columns.Add(new DataColumn("GROSS_AMOUNT", Type.GetType("System.Decimal")));
                dtPaymentEDDFileReport.Columns.Add(new DataColumn("STATE_TAX_AMOUNT", Type.GetType("System.Decimal")));
            }

            dtPaymentEDDFileReport.TableName = EDDReportData.StateCode;
            foreach (busEDDFileData item in iclbEDDFileData)
            {
                DataRow dr = dtPaymentEDDFileReport.NewRow();
                dr["SSN"] = item.ssn;
                dr["LAST_NAME"] = item.istrLastName;
                dr["FIRST_NAME"] = item.istrFirstName;
                dr["MIDDLE_NAME"] = item.istrMiddleName;
                dr["STATE_CODE"] = item.istrStateCode;
                dr["GROSS_AMOUNT"] = item.idecGrossAmt;
                dr["STATE_TAX_AMOUNT"] = item.idecStateTax;
                dtPaymentEDDFileReport.Rows.Add(dr);
            }

            if (istrPlanIdentifierValue == busConstant.PENSION)
                EDDReportData.PaymentEDDFilePensionData = dtPaymentEDDFileReport;
            else if (istrPlanIdentifierValue == busConstant.IAP)
                EDDReportData.PaymentEDDFileIAPData = dtPaymentEDDFileReport;
        }
        void LoadConstants()
        {
            //istrEmployerName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CODE_ID, "MPI").description;
            //AddressLine
            istrEmployerStrAdd = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_STREET_ADDRESS_CODE_ID, "VENT").description;
            //city
            istrEmployerCity = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CITY_CODE_ID, "STUC").description;
            //StateOrProvince
            istrEmployerState = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_STATE_CODE_ID, "CA").description;

            istrZipCodeExtension = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_ZIP_CODE_EXTENSION_ID, "1999").description;
            istrZipCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_ZIP_CODE_ID, "EDDF").description;
            istrStateCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_ID, "CAST").description;
            if (istrPlanIdentifierValue != "IAP")
            {
                //StateEINValue
                istrStateEmployerAccNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_STATE_EMPLOYER_ACCOUNT_NO_CODE_ID, "PENR").description;//Needed
                istrEmployerName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CODE_ID, "MPI").description;
                istrTINTypeValue = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Federal_CODE_Id, "PENR").description;
                istrStateUIDO = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_Id, "PEND").description;

            }
            else
            {
                //StateEINValue
                istrStateEmployerAccNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_STATE_EMPLOYER_ACCOUNT_NO_CODE_ID, "IAPR").description;//Needed
                istrEmployerName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CODE_ID, "MPI").description;
                istrTINTypeValue = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Federal_CODE_Id, "IAPR").description;
                istrStateUIDO = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_Id, "IAPD").description;
            }
            istrWagePlanCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_WAGE_PLAN_ID, "P").description;
            istrReturnType = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.OTHER_EDD_VALUE, "RETN").description;
            istrForm = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.OTHER_EDD_VALUE, "FORM").description;
            istrAction = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.OTHER_EDD_VALUE, "ACTD").description;
            istrTypeTTN = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.OTHER_EDD_VALUE, "TYPT").description;
            istrPhoneNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_TEL_EXTENSION_ID, "EDDP").description;
            istrContactEmail = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.CONTACT_EMAIL_CODE_ID, "CEED").description;
            istrContactFirstName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.CONTACT_NAME_CODE_ID, "CONF").description;
            istrContactLastName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.CONTACT_NAME_CODE_ID, "CONL").description;
        }
        public int aintNoofEmployee { get; set; }
        public decimal adecTotalGross { get; set; }
        public decimal adecStateTax { get; set; }
        void LoadFooter()
        {
            if (iclbEDDFileData != null)
            {
                aintNoofEmployee = iclbEDDFileData.Count();
                adecTotalGross = iclbEDDFileData.Sum(obj => obj.idecGrossAmt);
                adecTotalTaxable = iclbEDDFileData.Sum(obj => obj.idecTaxableAMt);
                adecStateTax = iclbEDDFileData.Sum(obj => obj.idecStateTax);
                aintTotalMonth1Employee = iclbEDDFileData.Sum(obj => obj.aintMonth1Employee);
                aintTotalMonth2Employee = iclbEDDFileData.Sum(obj => obj.aintMonth2Employee);
                aintTotalMonth3Employee = iclbEDDFileData.Sum(obj => obj.aintMonth3Employee);

            }

        }


        public void CreateEDDFileCollection()
        {
            iclbEDDFileData.ForEach(item => item.istrWithdralFlag = busConstant.FLAG_YES);


            iclbEDDFileData = (from obj in iclbEDDFileData
                               group obj by new
                               {
                                   ssn = obj.ssn,
                                   istrLastName = obj.istrLastName,
                                   istrFirstName = obj.istrFirstName,
                                   istrStateCode = obj.istrStateCode,
                                   istrStateEmployerAccNo = obj.istrStateEmployerAccNo,
                                   istrMiddleName = obj.istrMiddleName,
                                   istrWagePlanCode = obj.istrWagePlanCode,
                                   istrReportingYear = obj.istrReportingYear,
                               } into objgp
                               select new busEDDFileData
                               {

                                   ssn = objgp.Key.ssn,
                                   istrLastName = objgp.Key.istrLastName,
                                   istrFirstName = objgp.Key.istrFirstName,
                                   istrStateCode = objgp.Key.istrStateCode,
                                   istrStateEmployerAccNo = objgp.Key.istrStateEmployerAccNo,
                                   istrMiddleName = objgp.Key.istrMiddleName,
                                   istrWagePlanCode = objgp.Key.istrWagePlanCode,
                                   istrReportingYear = objgp.Key.istrReportingYear,
                                   idecGrossAmt = objgp.Sum(i => i.idecGrossAmt),
                                   idecStateTax = objgp.Sum(i => i.idecStateTax),
                                   idecTaxableAMt = objgp.Sum(i => i.idecTaxableAMt),
                                   aintMonth1Employee = objgp.Sum(i => i.aintMonth1Employee),
                                   aintMonth2Employee = objgp.Sum(i => i.aintMonth2Employee),
                                   aintMonth3Employee = objgp.Sum(i => i.aintMonth3Employee),

                               }).ToList().ToCollection<busEDDFileData>();

        }

        public override void BeforeWriteRecord()
        {
            base.BeforeWriteRecord();
            if (iobjDetail != null)
            {
                CreateDetailRecord((busEDDFileData)iobjDetail);
            }
        }

        public override void AfterWriteRecord()
        {
            base.AfterWriteRecord();
        }

        public override void InitializeFile()
        {
            base.InitializeFile();
            ixmlDoc = new XmlDocument();
            XmlDeclaration lxmlDec = ixmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            ixmlDoc.AppendChild(lxmlDec);
            XmlElement nodeReturnData = ixmlDoc.CreateElement("ReturnData");

            #region Create Schema
            busGlobalFunctions.SetXmlAttributeValue(ixmlDoc, nodeReturnData, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            busGlobalFunctions.SetXmlAttributeValue(ixmlDoc, nodeReturnData, "xmlns", "http://www.irs.gov/efile");

            XmlAttribute attrShemaLOcation = ixmlDoc.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
            string lstrSchemaDPath = "http://www.irs.gov/efile ReturnDataState.xsd";
            //lstrSchemaDPath = lstrSchemaDPath + HttpContext.Current.Server.MapPath("ReturnDataState.xsd");
            attrShemaLOcation.Value = lstrSchemaDPath;
            nodeReturnData.Attributes.Append(attrShemaLOcation);

            busGlobalFunctions.SetXmlAttributeValue(ixmlDoc, nodeReturnData, "documentCount", "2");
            #endregion

            #region Return Header State
            //Location
            XmlElement lxmlLoc = ixmlDoc.CreateElement("ContentLocation");
            lxmlLoc.InnerText = istrStateUIDO;
            nodeReturnData.AppendChild(lxmlLoc);

            //Header
            XmlElement lxmlHeaderState = ixmlDoc.CreateElement("ReturnHeaderState");
            busGlobalFunctions.SetXmlAttributeValue(ixmlDoc, lxmlHeaderState, "documentId", "A");

            //string lstrStartDate = idtStartDate.ToString("yyyy'-'MM'-'dd");
            //string lstrEndDate = idtEndDate.ToString("yyyy'-'MM'-'dd");
            // Tax Period Begin Date and Tax Period End Date as per Quarter.
            string lstrStartDate = Convert.ToDateTime(iarrParameters[2]).ToString("yyyy'-'MM'-'dd");
            string lstrEndDate = Convert.ToDateTime(iarrParameters[3]).ToString("yyyy'-'MM'-'dd");


            XmlElement lnodeBeginDate = ixmlDoc.CreateElement("TaxPeriodBeginDate");
            lnodeBeginDate.InnerText = Convert.ToString(lstrStartDate);
            lxmlHeaderState.AppendChild(lnodeBeginDate);

            XmlElement lnodeEndDate = ixmlDoc.CreateElement("TaxPeriodEndDate");
            lnodeEndDate.InnerText = Convert.ToString(lstrEndDate);
            lxmlHeaderState.AppendChild(lnodeEndDate);

            XmlElement lnodeReturnQuarter = ixmlDoc.CreateElement("ReturnQuarter");
            lnodeReturnQuarter.InnerText = Convert.ToString(lintQuarter);
            lxmlHeaderState.AppendChild(lnodeReturnQuarter);

            XmlElement lnodeTaxyear = ixmlDoc.CreateElement("Taxyear");
            lnodeTaxyear.InnerText = Convert.ToString(lintTaxYear);
            lxmlHeaderState.AppendChild(lnodeTaxyear);

            XmlElement lnodeReturnType = ixmlDoc.CreateElement("ReturnType");
            lnodeReturnType.InnerText = "StateCombined";
            lxmlHeaderState.AppendChild(lnodeReturnType);

            XmlElement lnodeForm = ixmlDoc.CreateElement("Form");
            lnodeForm.InnerText = "DE9C";
            lxmlHeaderState.AppendChild(lnodeForm);

            #region FillingAction
            XmlElement lnodeFillingAction = ixmlDoc.CreateElement("FilingAction");

            XmlElement lnodeAction = ixmlDoc.CreateElement("Action");
            lnodeAction.InnerText = "Original";
            lnodeFillingAction.AppendChild(lnodeAction);

            lxmlHeaderState.AppendChild(lnodeFillingAction);
            #endregion

            #region TIN
            XmlElement lnodeTIN = ixmlDoc.CreateElement("TIN");

            XmlElement lnodeTypeTIN = ixmlDoc.CreateElement("TypeTIN");
            lnodeTypeTIN.InnerText = "FEIN";
            lnodeTIN.AppendChild(lnodeTypeTIN);

            XmlElement lnodeTINTypeValue = ixmlDoc.CreateElement("TINTypeValue");
            lnodeTINTypeValue.InnerText = Convert.ToString(istrTINTypeValue);
            lnodeTIN.AppendChild(lnodeTINTypeValue);

            lxmlHeaderState.AppendChild(lnodeTIN);
            #endregion

            #region StateEIN
            XmlElement lnodeStateEIN = ixmlDoc.CreateElement("StateEIN");

            XmlElement lnodeTypeStateEIN = ixmlDoc.CreateElement("TypeStateEIN");
            lnodeTypeStateEIN.InnerText = "WithholdingAccountNo";
            lnodeStateEIN.AppendChild(lnodeTypeStateEIN);

            XmlElement lnodeStateEINValue = ixmlDoc.CreateElement("StateEINValue");
            lnodeStateEINValue.InnerText = Convert.ToString(istrStateEmployerAccNo);
            lnodeStateEIN.AppendChild(lnodeStateEINValue);

            lxmlHeaderState.AppendChild(lnodeStateEIN);
            #endregion

            #region StateCode
            XmlElement lnodeStateCode = ixmlDoc.CreateElement("StateCode");
            lnodeStateCode.InnerText = busConstant.CALIFORNIA;
            lxmlHeaderState.AppendChild(lnodeStateCode);
            #endregion

            #region BusinessAddress
            XmlElement lnodeBusinessAddress = ixmlDoc.CreateElement("BusinessAddress");

            XmlElement lnodeBusinessName = ixmlDoc.CreateElement("BusinessName");
            lnodeBusinessName.InnerText = istrEmployerName;
            lnodeBusinessAddress.AppendChild(lnodeBusinessName);

            XmlElement lnodeAddressLine = ixmlDoc.CreateElement("AddressLine");
            lnodeAddressLine.InnerText = istrEmployerStrAdd;
            lnodeBusinessAddress.AppendChild(lnodeAddressLine);

            XmlElement lnodeCity = ixmlDoc.CreateElement("City");
            lnodeCity.InnerText = istrEmployerCity;
            lnodeBusinessAddress.AppendChild(lnodeCity);

            XmlElement lnodeStateOrProvince = ixmlDoc.CreateElement("StateOrProvince");
            lnodeStateOrProvince.InnerText = istrEmployerState;
            lnodeBusinessAddress.AppendChild(lnodeStateOrProvince);

            XmlElement lnodeZipCode = ixmlDoc.CreateElement("ZipCode");
            //if (!string.IsNullOrEmpty(istrZipCodeExtension))
            //{
            //    lnodeZipCode.InnerText = istrZipCode + "-" + istrZipCodeExtension;
            //}
            //else
            //{
            lnodeZipCode.InnerText = istrZipCode;
            //}

            lnodeBusinessAddress.AppendChild(lnodeZipCode);

            XmlElement lnodePhoneNumber = ixmlDoc.CreateElement("PhoneNumber");
            lnodePhoneNumber.InnerText = istrPhoneNo;
            lnodeBusinessAddress.AppendChild(lnodePhoneNumber);

            lxmlHeaderState.AppendChild(lnodeBusinessAddress);
            #endregion

            #region Contact

            XmlElement lnodeContact = ixmlDoc.CreateElement("Contact");

            XmlElement lnodeContactName = ixmlDoc.CreateElement("ContactName");
            XmlElement lnodeContactFirstName = ixmlDoc.CreateElement("FirstName");
            lnodeContactFirstName.InnerText = istrContactFirstName;
            if (lnodeContactFirstName.InnerText.IsNullOrEmpty())
                lnodeContactFirstName.InnerText = "NONAME";
            lnodeContactName.AppendChild(lnodeContactFirstName);

            XmlElement lnodeContactLastName = ixmlDoc.CreateElement("LastName");
            lnodeContactLastName.InnerText = istrContactLastName;
            if (lnodeContactLastName.InnerText.IsNullOrEmpty())
                lnodeContactLastName.InnerText = "NONAME";
            lnodeContactName.AppendChild(lnodeContactLastName);
            lnodeContact.AppendChild(lnodeContactName);

            XmlElement lnodeContactEMAIL = ixmlDoc.CreateElement("EmailAddress");
            lnodeContactEMAIL.InnerText = istrContactEmail;
            lnodeContact.AppendChild(lnodeContactEMAIL);

            lxmlHeaderState.AppendChild(lnodeContact);

            #endregion

            nodeReturnData.AppendChild(lxmlHeaderState);

            #endregion

            #region StateRun

            //StateReturn
            XmlElement lxmlStateReturn = ixmlDoc.CreateElement("StateReturn");
            nodeReturnData.AppendChild(lxmlStateReturn);

            //StateCombined
            XmlElement lxmlSateCombined = ixmlDoc.CreateElement("StateCombined");
            busGlobalFunctions.SetXmlAttributeValue(ixmlDoc, lxmlSateCombined, "documentId", "-");

            //Footer
            XmlElement lnodeNumberOfEmployees = ixmlDoc.CreateElement("NumberOfEmployees");
            lnodeNumberOfEmployees.InnerText = Convert.ToString(aintNoofEmployee);
            lxmlSateCombined.AppendChild(lnodeNumberOfEmployees);

            XmlElement lnodeTotalWages = ixmlDoc.CreateElement("WHTotalWages");
            lnodeTotalWages.InnerText = "0.00";//PIR 935 //946
            lxmlSateCombined.AppendChild(lnodeTotalWages);

            XmlElement lnodeTotalIncomeTaxWithheld = ixmlDoc.CreateElement("TotalIncomeTaxWithheld");
            lnodeTotalIncomeTaxWithheld.InnerText = Convert.ToString(adecStateTax);
            lxmlSateCombined.AppendChild(lnodeTotalIncomeTaxWithheld);

            XmlElement lnodeTaxableWages = ixmlDoc.CreateElement("WHTaxableWages");
            lnodeTaxableWages.InnerText = Convert.ToString(adecTotalTaxable);//PIR 935
            lxmlSateCombined.AppendChild(lnodeTaxableWages);

            lxmlStateReturn.AppendChild(lxmlSateCombined);

            inodePayroll = ixmlDoc.CreateElement("PayRoll");
            lxmlSateCombined.AppendChild(inodePayroll);

            XmlElement lnodeMonth1 = ixmlDoc.CreateElement("Month1Employees");
            lnodeMonth1.InnerText = Convert.ToString(aintTotalMonth1Employee);
            lxmlSateCombined.AppendChild(lnodeMonth1);

            XmlElement lnodeMonth2 = ixmlDoc.CreateElement("Month2Employees");
            lnodeMonth2.InnerText = Convert.ToString(aintTotalMonth2Employee);
            lxmlSateCombined.AppendChild(lnodeMonth2);

            XmlElement lnodeMonth3 = ixmlDoc.CreateElement("Month3Employees");
            lnodeMonth3.InnerText = Convert.ToString(aintTotalMonth3Employee);
            lxmlSateCombined.AppendChild(lnodeMonth3);


            ixmlDoc.AppendChild(nodeReturnData);
            #endregion
        }

        public override void FinalizeFile()
        {
            base.FinalizeFile();
            if (iobjSystemManagement == null)
            {
                iobjSystemManagement = new busSystemManagement();
                iobjSystemManagement.FindSystemManagement();
            }

            string lstrFileName = iobjSystemManagement.icdoSystemManagement.base_directory;
            if (this.ibusFile.icdoFile.mailbox_path_code.IsNotNullOrEmpty())
            {
                DataTable ldt = busBase.Select("cdoSystemPaths.GetPathByCode", new object[1] { this.ibusFile.icdoFile.mailbox_path_code });
                if (ldt.IsNotNull() && ldt.Rows.Count > 0)
                {
                    string lstrpathcode = Convert.ToString(ldt.Rows[0]["path_value"]);
                    if (Directory.Exists(lstrFileName + lstrpathcode))
                    {
                        lstrFileName += lstrpathcode;
                    }
                }
            }
            lstrFileName += istrFileName.Replace(".csv", ".xml");


            ixmlDoc.Save(lstrFileName);
            //using (TextWriter sw = new StreamWriter(lstrFileName, false, Encoding.UTF8))
            //{
            //ixmlDoc.Save(sw);
            //}

        }

        public void CreateDetailRecord(busEDDFileData abusEDDFileData)
        {
            XmlElement lnodeEmployee = ixmlDoc.CreateElement("Employee");

            XmlElement anodeSSN = ixmlDoc.CreateElement("SSN");
            anodeSSN.InnerText = abusEDDFileData.ssn;
            lnodeEmployee.AppendChild(anodeSSN);

            XmlElement lnodeSubEmployeeName = ixmlDoc.CreateElement("Employee");
            XmlElement anodeFirstName = ixmlDoc.CreateElement("FirstName");
            anodeFirstName.InnerText = abusEDDFileData.istrFirstName;
            lnodeSubEmployeeName.AppendChild(anodeFirstName);

            if (!string.IsNullOrEmpty(abusEDDFileData.istrMiddleName))
            {
                XmlElement anodeMiddleName = ixmlDoc.CreateElement("MiddleName");
                anodeMiddleName.InnerText = abusEDDFileData.istrMiddleName.Substring(0, 1);
                lnodeSubEmployeeName.AppendChild(anodeMiddleName);
            }

            XmlElement anodeLastName = ixmlDoc.CreateElement("LastName");
            anodeLastName.InnerText = abusEDDFileData.istrLastName;
            lnodeSubEmployeeName.AppendChild(anodeLastName);
            lnodeEmployee.AppendChild(lnodeSubEmployeeName);

            XmlElement anodeTotalWage = ixmlDoc.CreateElement("TotalWages");
            anodeTotalWage.InnerText = "0.00";//PIR 935 //PIR 946
            lnodeEmployee.AppendChild(anodeTotalWage);

            XmlElement anodeTaxableWage = ixmlDoc.CreateElement("TaxableWages");
            anodeTaxableWage.InnerText = Convert.ToString(abusEDDFileData.idecTaxableAMt);////PIR 935
            lnodeEmployee.AppendChild(anodeTaxableWage);

            XmlElement anodeTaxableWithheld = ixmlDoc.CreateElement("TaxWithheld");
            anodeTaxableWithheld.InnerText = Convert.ToString(abusEDDFileData.idecStateTax);
            lnodeEmployee.AppendChild(anodeTaxableWithheld);

            XmlElement anodeWagePlan = ixmlDoc.CreateElement("WagePlan");
            anodeWagePlan.InnerText = abusEDDFileData.istrWagePlanCode;
            lnodeEmployee.AppendChild(anodeWagePlan);

            inodePayroll.AppendChild(lnodeEmployee);


        }

        public string istrTINTypeValue { get; set; }

        public string istrStateUIDO { get; set; }

        public string istrReturnType { get; set; }

        public string istrForm { get; set; }

        public string istrAction { get; set; }

        public string istrTypeTTN { get; set; }

        public string istrPhoneNo { get; set; }

        public string istrContactEmail { get; set; }

        public string istrContactFirstName { get; set; }

        public string istrContactLastName { get; set; }

        public decimal adecTotalTaxable { get; set; }

        public int aintTotalMonth1Employee { get; set; }

        public int aintTotalMonth2Employee { get; set; }

        public int aintTotalMonth3Employee { get; set; }
    }


    [Serializable]
    public class busEDDFileData : busMPIPHPBase
    {
        public string istrLastName { get; set; }
        public string istrFirstName { get; set; }
        public string istrMiddleName { get; set; }
        public decimal idecGrossAmt { get; set; }
        public decimal idecStateTax { get; set; }
        public string istrStateCode { get; set; }
        public string istrStateEmployerAccNo { get; set; }
        public string istrWagePlanCode { get; set; }
        public string ssn { get; set; }
        public string istrReportingYear { get; set; }
        public int aintPlan { get; set; }
        public string istrWithdralFlag { get; set; }


        public decimal idecTaxableAMt { get; set; }

        public int aintMonth1Employee { get; set; }

        public int aintMonth3Employee { get; set; }

        public int aintMonth2Employee { get; set; }

        public decimal idecFedAMt { get; set; }

        public decimal idecNetAMt { get; set; }
    }
    public class busPersonGross
    {
        public int PersonId { get; set; }

        public decimal GrossAmt { get; set; }
        public decimal amtpaid { get; set; }
        public decimal amtpaid_state_tax { get; set; }
        public decimal remainGrs { get; set; }
        public string istrWithdralFlag { get; set; }
        public string istrWithdralFlagRepayment { get; set; }


    }
    public class busEDDReportData
    {
        public string StateCode;
        public DataTable PaymentEDDFilePensionData;
        public DataTable PaymentEDDFileIAPData;
        public DataTable EDDFileReportData;
    }
}
