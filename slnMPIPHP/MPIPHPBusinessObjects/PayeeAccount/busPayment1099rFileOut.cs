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

namespace MPIPHP.BusinessObjects.PayeeAccount
{
    [Serializable]
    public class busPayment1099rFileOut : busFileBaseOut
    {
        //Collection of 1099r details for all the payees
        public Collection<busPayment1099rFile> iclbPayment1099r { get; set; }

        public decimal idecTotalDistribtionPercentage { get; set; }
        public decimal idecTotalGrossAmount { get; set; }
        public decimal idecTotalTaxableAmount { get; set; }
        public decimal idecTotatlNonTaxableAmount { get; set; }
        public decimal idecTotalCapitalGain { get; set; }
        public decimal idecTotalFedTaxWithheld { get; set; }
        public decimal idecTotalMemberContributions { get; set; }

        public string istrRecordSequenceNumber_C { get; set; }

        public string istrRecordSequenceNumber_F { get; set; }

        public string istrPriorYearDataIndiacator { get; set; }
        public string istrTestFileIdicatior { get; set; }
        public string istrTotalNoOfPayees { get; set; }
        public int iintTotalNoOfAPayees { get; set; }
        public string istrNameControl { get; set; }

        //Property to indicate whether file generating from correction batch annual batchs
        public bool iblnCorrectionBatchIndicator { get; set; }
        //Property to load Tax year
        public int iintTaxYear { get; set; }
        public string istrTransmitterControlCode { get; set; }
        public string istrEmployerName { get; set; }
        public string istrEmployerNameControlId { get; set; }
        public string istrEmployerStrAdd { get; set; }
        public string istrEmployerState { get; set; }
        public string istrZipCodeExtension { get; set; }
        public string istrZipCode { get; set; }
        public string istrStateCode { get; set; }
        public string istrEmployerCity { get; set; }
        public string istrStateEmployerAccNo { get; set; }
        public string istrTransTTNNo { get; set; }

        public string istrContactName { get; set; }
        public string istrContactEmail { get; set; }
        public string istrContactPhoneNo { get; set; } //PIR 838
        public string istrAmountCode { get; set; }
        public string istrPlanIdentifierValue { get; set; }
        public string istrPayerTaxNo { get; set; }



        //formatted amount fields
        public string istrFormattedTotalGrossAmount
        {
            get
            {
                decimal ldecTotalGrossAmount = idecTotalGrossAmount * 100;
                if (ldecTotalGrossAmount.ToString().IndexOf('.') > 0)
                    return ldecTotalGrossAmount.ToString().Substring(0, ldecTotalGrossAmount.ToString().IndexOf('.')).PadLeft(18, '0');
                else
                    return ldecTotalGrossAmount.ToString().PadLeft(18, '0');
            }
        }
        public string istrFormattedTotalTaxableAmount
        {
            get
            {
                decimal ldecTotalTaxableAmount = idecTotalTaxableAmount * 100;
                return ldecTotalTaxableAmount.ToString().Substring(0, ldecTotalTaxableAmount.ToString().IndexOf('.')).PadLeft(18, '0');
            }
        }
        public string istrFormattedTotalCapitalGain
        {
            get
            {
                decimal ldecTotalCapitalGain = idecTotalCapitalGain * 100;
                if (ldecTotalCapitalGain.ToString().IndexOf('.') > 0)
                    return ldecTotalCapitalGain.ToString().Substring(0, ldecTotalCapitalGain.ToString().IndexOf('.')).PadLeft(18, '0');
                else
                    return ldecTotalCapitalGain.ToString().PadLeft(18, '0');
            }
        }
        public string istrFormattedTotalFedTaxWithheld
        {
            get
            {
                decimal ldecTotalFedTaxWithheld = idecTotalFedTaxWithheld * 100;
                if (ldecTotalFedTaxWithheld.ToString().IndexOf('.') > 0)
                    return ldecTotalFedTaxWithheld.ToString().Substring(0, ldecTotalFedTaxWithheld.ToString().IndexOf('.')).PadLeft(18, '0');
                else
                    return ldecTotalFedTaxWithheld.ToString().PadLeft(18, '0');
            }
        }
        public string istrFormattedTotalNonTaxableAmount
        {
            get
            {
                decimal ldecTotalNonTaxableAmount = idecTotatlNonTaxableAmount * 100;
                if (ldecTotalNonTaxableAmount.ToString().IndexOf('.') > 0)
                    return ldecTotalNonTaxableAmount.ToString().Substring(0, ldecTotalNonTaxableAmount.ToString().IndexOf('.')).PadLeft(18, '0');
                else
                    return ldecTotalNonTaxableAmount.ToString().PadLeft(18, '0');
            }
        }
        public string istrFormattedTotalMemberContributions
        {
            get
            {
                decimal ldecTotalMemberContributions = idecTotalMemberContributions * 100;
                if (ldecTotalMemberContributions.ToString().IndexOf('.') > 0)
                    return ldecTotalMemberContributions.ToString().Substring(0, ldecTotalMemberContributions.ToString().IndexOf('.')).PadLeft(18, '0');
                else
                    return ldecTotalMemberContributions.ToString().PadLeft(18, '0');
            }
        }

        public string istrBlank241_C
        {
            get
            {
                string lstr = string.Empty;
                return lstr.PadRight(241, ' ');
            }
        }
        public string istrBlank2_C
        {
            get
            {
                string lstr = string.Empty;
                return lstr.PadRight(2, ' ');
            }
        }
        public string istrBlank241_F
        {
            get
            {
                string lstr = string.Empty;
                return lstr.PadRight(241, ' ');
            }
        }
        public string istrBlank2_F
        {
            get
            {
                string lstr = string.Empty;
                return lstr.PadRight(2, ' ');
            }
        }
        public override void InitializeFile()
        {

            base.InitializeFile();
        }
        public void Load1099rDetails(DataTable ldtb1099rDetails)
        {
            iclbPayment1099r = new Collection<busPayment1099rFile>();

            iintTaxYear = (int)iarrParameters[0];
            iblnCorrectionBatchIndicator = (bool)iarrParameters[1];
            ldtb1099rDetails = (DataTable)iarrParameters[2];
            istrPlanIdentifierValue = Convert.ToString(iarrParameters[3]);
            LoadConstants();
            int lintRecordSequenceNumber = 3;
            foreach (DataRow dr in ldtb1099rDetails.Rows)
            {
                busPayment1099rFile lobjPayment1099rFile = new busPayment1099rFile { icdoPayment1099r = new cdoPayment1099r() };
                lobjPayment1099rFile.icdoPayment1099r.LoadData(dr);
                //uat pir 1794 -- Start
                lobjPayment1099rFile.icdoPayment1099r.addr_line_1 =
                    (!string.IsNullOrEmpty(lobjPayment1099rFile.icdoPayment1099r.addr_line_1) ? lobjPayment1099rFile.icdoPayment1099r.addr_line_1.ToUpper() : string.Empty) + " " +
                    (!string.IsNullOrEmpty(lobjPayment1099rFile.icdoPayment1099r.addr_line_2) ? lobjPayment1099rFile.icdoPayment1099r.addr_line_2.ToUpper() : string.Empty);
                if (lobjPayment1099rFile.icdoPayment1099r.addr_line_1.Length > 40)
                    lobjPayment1099rFile.icdoPayment1099r.addr_line_1 = lobjPayment1099rFile.icdoPayment1099r.addr_line_1.ToString().Substring(0, 40);
                else
                    lobjPayment1099rFile.icdoPayment1099r.addr_line_1 = lobjPayment1099rFile.icdoPayment1099r.addr_line_1.ToString().PadRight(40, ' ');
                //uat pir 1794 -- End
                lobjPayment1099rFile.icdoPayment1099r.addr_city = !string.IsNullOrEmpty(lobjPayment1099rFile.icdoPayment1099r.addr_city) ?
                    lobjPayment1099rFile.icdoPayment1099r.addr_city.ToUpper() : string.Empty;
                if (dr["addr_state"] != DBNull.Value)
                {
                    lobjPayment1099rFile.addr_state = dr["addr_state"].ToString().ToUpper();
                }



                if (lobjPayment1099rFile.icdoPayment1099r.addr_country_value != "0001")
                {
                    lobjPayment1099rFile.istrForeignEntityIndicator = "1";
                }
                else
                {
                    lobjPayment1099rFile.istrForeignEntityIndicator = " ";
                }
                lobjPayment1099rFile.LoadPayeeAccount();
                lobjPayment1099rFile.istrRecordSequenceNumber = lintRecordSequenceNumber.ToString();
                lobjPayment1099rFile.istrRecordSequenceNumber = lobjPayment1099rFile.istrRecordSequenceNumber.PadLeft(8, '0');
                //Type of TIN will be blank if Payee is Person else "2" if payee is org
                //ask once
                // lobjPayment1099rFile.istrTypeOfTIN = string.IsNullOrEmpty(lobjPayment1099rFile.icdoPayment1099r.s) ? " " : "2";
                //TIN will be ssn if  be blank if Payee is Person else fedreal id if payee is org
                // lobjPayment1099rFile.istrTIN = string.IsNullOrEmpty(lobjPayment1099rFile.icdoPayment1099r.ssn) ?
                //lobjPayment1099rFile.icdoPayment1099r.federal_id : lobjPayment1099rFile.icdoPayment1099r.ssn;
                //ask once
                if (dr["RECIPIENTS_ID"] != DBNull.Value)
                {
                    lobjPayment1099rFile.istrTIN = dr["RECIPIENTS_ID"].ToString().ToUpper();
                }

                //= ;//lobjPayment1099rFile.icdoPayment1099r.federal_id;
                //lobjPayment1099rFile.istrTaxAmountNotDefined = lobjPayment1099rFile.icdoPayment1099r.taxable_amount > 0.0m ? "1" : " ";
                //Format percentage
                //systest pir 2396 : should be 'G' if corrected else empty
                lobjPayment1099rFile.istrCorrectedIndicator = iblnCorrectionBatchIndicator ? "G" : " ";

                if (lobjPayment1099rFile.icdoPayment1099r.dist_percentage > 0 && lobjPayment1099rFile.icdoPayment1099r.dist_percentage < 100)
                {
                    lobjPayment1099rFile.distribution_percentage = Convert.ToInt32(Math.Floor(lobjPayment1099rFile.icdoPayment1099r.dist_percentage)).ToString().PadLeft(2, '0');
                }
                else
                {
                    lobjPayment1099rFile.distribution_percentage = "  ";
                }
                if (lobjPayment1099rFile.icdoPayment1099r.name != null)
                {
                    //First 40 characters in Payee name
                    if (lobjPayment1099rFile.icdoPayment1099r.name.Length > 39)
                    {
                        lobjPayment1099rFile.PayeeName = lobjPayment1099rFile.icdoPayment1099r.name.Substring(0, 39).ToUpper();
                    }
                    else
                    {
                        lobjPayment1099rFile.PayeeName = lobjPayment1099rFile.icdoPayment1099r.name.ToUpper();
                    }
                }
                lobjPayment1099rFile.istrTotalDistributionIndicator = lobjPayment1099rFile.icdoPayment1099r.total_distribution_flag == busConstant.Flag_Yes ?
                    "1" : " ";
                string lstrTemp = String.Empty;
                if (lobjPayment1099rFile.icdoPayment1099r.mpi_person_id.IsNotNullOrEmpty())
                {
                    lstrTemp = lobjPayment1099rFile.icdoPayment1099r.mpi_person_id;
                }
                lobjPayment1099rFile.istrMpiId = lstrTemp.PadLeft(20, ' ');
                lobjPayment1099rFile.istrPayerTaxNo = lobjPayment1099rFile.icdoPayment1099r.payer_state_no; // istrPayerTaxNo;
                iclbPayment1099r.Add(lobjPayment1099rFile);
                lintRecordSequenceNumber++;
            }
            istrRecordSequenceNumber_C = (lintRecordSequenceNumber).ToString().PadLeft(8, '0');
            istrRecordSequenceNumber_F = (lintRecordSequenceNumber + 1).ToString().PadLeft(8, '0');
            LoadHeaderDetails();
            LoadFooterDetails();
            istrFileName = "Payment1099rFile" + "_" + istrPlanIdentifierValue + "_" + DateTime.Now.ToString(busConstant.DateFormat) + "_" + iintTaxYear.ToString() + busConstant.FileFormattxt;
            //istrFileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "_" + "1099r_" + istrPlanIdentifierValue + ".txt";
        }

        //Load object fields specified in the footer section
        public void LoadFooterDetails()
        {
            //ask once capital gain
            idecTotalCapitalGain = iclbPayment1099r.Sum(o => o.icdoPayment1099r.non_taxable_amount);
            idecTotalTaxableAmount = iclbPayment1099r.Sum(o => o.icdoPayment1099r.taxable_amount);
            idecTotatlNonTaxableAmount = iclbPayment1099r.Sum(o => o.icdoPayment1099r.non_taxable_amount);
            idecTotalGrossAmount = iclbPayment1099r.Sum(o => o.icdoPayment1099r.gross_benefit_amount);
            idecTotalFedTaxWithheld = iclbPayment1099r.Sum(o => o.icdoPayment1099r.fed_tax_amount);
            idecTotalMemberContributions = iclbPayment1099r.Sum(o => o.icdoPayment1099r.gross_benefit_amount);
        }
        //load object fields specified in the file header section
        public void LoadHeaderDetails()
        {
            istrPriorYearDataIndiacator = iblnCorrectionBatchIndicator ? "P" : " ";
            iintTotalNoOfAPayees = iclbPayment1099r.Count;
            istrTotalNoOfPayees = (iintTotalNoOfAPayees).ToString().PadLeft(8, '0');
        }
        void LoadConstants()
        {

            ////For Query
            if (istrPlanIdentifierValue != "IAP")
            {
                istrTransTTNNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Federal_CODE_Id, "PENR").description;
                istrEmployerName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CODE_ID, "MPEN").description;
                istrPayerTaxNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_Id, "PENR").description;


            }
            else
            {
                istrTransTTNNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Federal_CODE_Id, "IAPR").description;
                istrEmployerName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CODE_ID, "MIAP").description;
                istrPayerTaxNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_Id, "IAPR").description;
            }
            //istrFederalIDIAP = 

            //istrFederalIDOther = 
            //Other Constant


            istrTransmitterControlCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.TRANSMITTER_CONTROL_CODE, "TCNC").description;
            istrEmployerNameControlId = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_NAME_CONTROL_CODE_ID, "1099").description;

            istrEmployerStrAdd = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_STREET_ADDRESS_CODE_ID, "VENT").description;
            istrEmployerCity = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_CITY_CODE_ID, "STUC").description;
            istrEmployerState = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_STATE_CODE_ID, "CA").description;


            istrZipCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_ZIP_CODE_ID, "1099").description;
            istrStateCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.STATE_CODE_ID, "CALF").description;
            istrContactName = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.CONTACT_NAME_CODE_ID, "CONN").description;
            istrContactEmail = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.CONTACT_EMAIL_CODE_ID, "CONE").description;
            istrContactPhoneNo = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.EMPLOYER_TEL_EXTENSION_ID, "EDDP").description; // PIR 838
            istrAmountCode = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.AMOUNT_CODE_ID, "1099").description;




        }
        public override void BeforeWriteRecord()
        {




            base.BeforeWriteRecord();
        }


    }
}
