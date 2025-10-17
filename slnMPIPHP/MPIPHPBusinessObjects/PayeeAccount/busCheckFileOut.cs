using MPIPHP.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using System;
using System.Data;
using System.Linq;
using System.Globalization;

namespace MPIPHP.BusinessObjects.PayeeAccount
{
    [Serializable]
    public class busCheckFileOut : busFileBaseOut
    {
        public Collection<busCheckFileData> iclbCheckFileData { get; set; }
        //Propety to store check file details
        public DataTable idtCheckFile { get; set; }
        public string istrfilName { get; set; }
        ////property to store check stub rhic details
        //public DataTable idtGHDVPersonAccount { get; set; }
        ////property to store Payment details
        //public DataTable idtPayments { get; set; }
        ////property to store RHIC amount details
        //public DataTable idtRHICDetails { get; set; }
        public string istrBenefitType { get; set; }
        public DateTime idtPaymentDate { get; set; }

        public bool iblnFromMonthlyOrAdhocBatch { get; set; }

        public override void InitializeFile()
        {
            istrFileName = DateTime.Now.ToString("yyyyMMdd") + istrfilName + busConstant.FileFormattxt;
        }

        /// <summary>
        /// Method to load the collection which need to be written to Check file
        /// </summary>
        /// <param name="adtCheckFile">Datatable</param>
        public void LoadCheckFile(DataTable adtCheckFile)
        {
            iclbCheckFileData = new Collection<busCheckFileData>();
            idtCheckFile = (DataTable)iarrParameters[0];
            int lintPaymentScheduleID = Convert.ToInt32(iarrParameters[1]);
            DateTime ldtPaymentDate = Convert.ToDateTime(iarrParameters[2]);
            iblnFromMonthlyOrAdhocBatch = Convert.ToBoolean(iarrParameters[3]);
            if(iarrParameters.Length>5)
            {
                istrfilName = Convert.ToString(iarrParameters[5]);
            }
             else
                {
                    istrfilName = "";
                }
            //if (iblnFromMonthlyOrAdhocBatch)
            //{
            //    idtPayments = busBase.Select("cdoPaymentHistoryDetail.LoadPaymentDeductionRecords", new object[1] { lintPaymentScheduleID });
            //}
            //else
            //{
            //    idtPayments = busBase.Select("cdoPaymentHistoryDetail.LoadPaymentDeductionsForVendorPayment", new object[1] { lintPaymentScheduleID });
            //}
            //idtGHDVPersonAccount = busBase.Select("cdoPaymentHistoryDetail.LoadGHDVAccounts", new object[2] { ldtPaymentDate, lintPaymentScheduleID });
            //idtRHICDetails = busBase.Select("cdoBenefitRhicCombine.LoadRHICDetails", new object[2] { ldtPaymentDate, lintPaymentScheduleID });
            idtPaymentDate = ldtPaymentDate;

            

            foreach (DataRow dr in idtCheckFile.Rows)
            {
                busCheckFileData lobjCheckFileData = new busCheckFileData();
                lobjCheckFileData.InitializeObjects();

                //lobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(dr);
                lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.LoadData(dr);
                //prod pir 4359
                if (!string.IsNullOrEmpty(lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description) &&
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_value != busConstant.USA.ToString())
                {
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description =
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description.ToUpper();
                }
                else
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description = string.Empty;
                lobjCheckFileData.istrPayeeName = dr["PayeeName_OrganizationName"] != DBNull.Value ?
                   Convert.ToString(dr["PayeeName_OrganizationName"]).ToUpper() : string.Empty;
                lobjCheckFileData.idecGrossAmount = dr["Gross_Amount"] != DBNull.Value ?
                            Convert.ToDecimal(dr["Gross_Amount"]) : 0.0M;
                lobjCheckFileData.idecTaxableAmount = dr["TAXABLE_AMOUNT"] != DBNull.Value ?
                           Convert.ToDecimal(dr["TAXABLE_AMOUNT"]) : 0.0M;
                lobjCheckFileData.idecNonTaxableAmount = dr["NON_TAXABLE_AMOUNT"] != DBNull.Value ?
                           Convert.ToDecimal(dr["NON_TAXABLE_AMOUNT"]) : 0.0M;
                lobjCheckFileData.idecStateTaxAmount = dr["State_Tax_Amount"] != DBNull.Value ?
                            Convert.ToDecimal(dr["State_Tax_Amount"]) : 0.0M;
                lobjCheckFileData.idecFederalTaxAmount = dr["Federal_Tax_Amount"] != DBNull.Value ?
                            Convert.ToDecimal(dr["Federal_Tax_Amount"]) : 0.0M;
                lobjCheckFileData.idtPayDate = ldtPaymentDate;
                lobjCheckFileData.istrSSN = dr["SSN"] != DBNull.Value ?
                   Convert.ToString(dr["SSN"]).ToUpper() : string.Empty;

                lobjCheckFileData.idecDedAmount4 = dr["Deduction4"] != DBNull.Value ?
                   Convert.ToDecimal(dr["Deduction4"]) : 0.0M;

                lobjCheckFileData.idecDedAmount4 = dr["Deduction4"] != DBNull.Value ?
                    Convert.ToDecimal(dr["Deduction4"]) : 0.0M;

                lobjCheckFileData.istrDedAmount4 = lobjCheckFileData.idecDedAmount4 != Decimal.Zero ? Convert.ToString(lobjCheckFileData.idecDedAmount4.ToString("###0.00;(###0.00)")) : "0.00";

                lobjCheckFileData.idecDedAmount3 = dr["Deduction3"] != DBNull.Value ?
                  Convert.ToDecimal(dr["Deduction3"]) : 0.0M;
                lobjCheckFileData.istrPayeeName = dr["PayeeName_OrganizationName"] != DBNull.Value ?
                   Convert.ToString(dr["PayeeName_OrganizationName"]).ToUpper() : string.Empty;

                lobjCheckFileData.istrBenefitType = Convert.ToString(iarrParameters[4]);
                    
                //lobjCheckFileData.istrPersonIDOrgCodeID = dr["istrPersonIdOrOrgCode"] != DBNull.Value ?
                //    Convert.ToString(dr["istrPersonIdOrOrgCode"]) : string.Empty;

                lobjCheckFileData.istrPlanID = dr["PLAN_ID"] != DBNull.Value ?
                    Convert.ToString(dr["PLAN_ID"]) : string.Empty;

                if (!string.IsNullOrEmpty(lobjCheckFileData.istrPlanID) && lobjCheckFileData.istrPlanID != "1")
                {
                    lobjCheckFileData.istrLabel = "PENSION PLAN";
                    //lobjCheckFileData.istrAccountNumber = "1000122978C";
                    //lobjCheckFileData.istrAccountNumber = "158300149776C";
                    lobjCheckFileData.istrAccountNumber = "0000000000C";
                }
                else
                {
                    lobjCheckFileData.istrLabel = "IAP" + (dr["FUNDS_TYPE_VALUE"] != DBNull.Value ? Convert.ToString(" - " + dr["FUNDS_TYPE_VALUE"]) : string.Empty);
                    //lobjCheckFileData.istrAccountNumber = "1000122951C";
                    //lobjCheckFileData.istrAccountNumber = "158300149768C";
                    lobjCheckFileData.istrAccountNumber = "0000000000C";
                }

                //lobjCheckFileData.istrFractionalRoutingNumber = "16-49/1220";
                //lobjCheckFileData.istrFractionalRoutingNumber = "90-3582/1222";
                lobjCheckFileData.istrFractionalRoutingNumber = "00-00/0000";
                iclbCheckFileData.Add(lobjCheckFileData);
                if (lobjCheckFileData.idecDedAmount3>0)
                {
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.account_number = lobjCheckFileData.istrSSN;
                }

                if (lobjCheckFileData.istrPayeeName.Length > busConstant.MPIPHPBatch.EMDEON_PAYEE_NAME_LENGTH)
                    lobjCheckFileData.istrPayeeName = lobjCheckFileData.istrPayeeName.Remove(busConstant.MPIPHPBatch.EMDEON_PAYEE_NAME_LENGTH);//PIR 1077

                //lobjCheckFileData.istrBenefitTypeDesc = dr["BENEFIT_TYPE"] != DBNull.Value ?
                //    Convert.ToString(dr["BENEFIT_TYPE"]) : string.Empty;
                //lobjCheckFileData.istrBenefitOptionDesc = dr["BENEFIT_OPTION"] != DBNull.Value ?
                //    Convert.ToString(dr["BENEFIT_OPTION"]) : string.Empty;

                //if (iblnFromMonthlyOrAdhocBatch)
                //    LoadRHICDetails(lobjCheckFileData);

                //LoadPaymentDetails(lobjCheckFileData);

                //if (lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.net_amount > 0)
                //{
                //    lobjCheckFileData.istrAmountInWords =
                //        busGlobalFunctions.AmountToWords(lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.net_amount.ToString()).ToUpper();
                //}
                
            }
        }

        
        public DataTable idtbPALifeOption { get; set; }
        public DataTable idtbGHDVHistory { get; set; }
        public DataTable idtbLifeHistory { get; set; }

        

    }
}

