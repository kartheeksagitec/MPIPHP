

namespace MPIPHP.BusinessObjects.PayeeAccount
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sagitec.BusinessObjects;
    using System.Collections.ObjectModel;
    using System.Data;
    using Sagitec.DBUtility;
    using System.IO;
    using System.Collections;
    using Sagitec.Common;
    using System.Globalization;
    using MPIPHP.BusinessObjects;
    using MPIPHP.CustomDataObjects;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class busPreNoteACHOutbound : busFileBaseOut
    {
        public busPreNoteACHOutbound()
        {
        }

        private int lintEmptyRowCount = 0;
        private string istrIsPullACH = string.Empty;
        private bool iblnIsPreNoteVerification = false;
        private bool iblnIsACHPaymentDistribution = false;
        private string FileName = string.Empty;

        public string istrBenefitType { get; set; }

        
        public Collection<busACHProviderReportData> iclbACHProvider
        {
            get;
            set;
        }


        private DateTime _ldtTodaysDate;
        public DateTime ldtTodaysDate
        {
            get
            {
                _ldtTodaysDate = DateTime.Now;
                return _ldtTodaysDate;
            }
        }


        public DateTime idtEffectiveDate
        {
            get;
            set;
        }

        private string _istrAccountNumber;
        public string istrAccountNumber
        {
            get { return _istrAccountNumber; }
            set { _istrAccountNumber = value; }
        }

        private string _istrCompanyName;
        public string istrCompanyName
        {
            get { return _istrCompanyName; }
            set { _istrCompanyName = value; }
        }


        private int _lintBlockCount;
        public int lintBlockCount
        {
            get { return _lintBlockCount; }
            set { _lintBlockCount = value; }
        }

        private string _lstrBlockCount;
        public string lstrBlockCount
        {
            get
            {
                _lstrBlockCount = Convert.ToString(_lintBlockCount);
                return _lstrBlockCount.PadLeft(6, '0');
            }
        }

        private long _lintTotalRoutingNo;
        public long lintTotalRoutingNo
        {
            get { return _lintTotalRoutingNo; }
            set { _lintTotalRoutingNo = value; }
        }

        public long iintTotalRoutingNumber { get; set; }

        private string _lstrEntryHash;
        public string lstrEntryHash
        {
            get
            {
                _lstrEntryHash = Convert.ToString(_lintTotalRoutingNo);
                return _lstrEntryHash.PadLeft(10, '0');
            }
        }

        public string istrEntryHash
        {
            get
            {
                string lstrTempEntryHash = Convert.ToString(iintTotalRoutingNumber);
                if (lstrTempEntryHash.Length > 10)
                    lstrTempEntryHash = lstrTempEntryHash.Right(10);
                return lstrTempEntryHash.PadLeft(10, '0');
            }
        }

        public void LoadTotalRoutingNo()
        {
            if (iclbACHProvider != null)
            {
                foreach (busACHProviderReportData lobjACH in iclbACHProvider)
                {
                    if (!string.IsNullOrEmpty(lobjACH.lstrRoutingNumber))
                        _lintTotalRoutingNo += Convert.ToInt32(lobjACH.lstrRoutingNumber);
                    if (!string.IsNullOrEmpty(lobjACH.istrRoutingNumberFirstEightDigits))
                        iintTotalRoutingNumber += Convert.ToInt32(lobjACH.istrRoutingNumberFirstEightDigits);
                }
            }
        }

        private decimal _ldclTotalDebitAmount;
        public decimal ldclTotalDebitAmount
        {
            get { return _ldclTotalDebitAmount; }
            set { _ldclTotalDebitAmount = value; }
        }

        private decimal _ldclTotalCreditAmount;
        public decimal ldclTotalCreditAmount
        {
            get { return _ldclTotalCreditAmount; }
            set { _ldclTotalCreditAmount = value; }
        }
        public string ldclTotalDebitAmountFormatted
        {
            get
            {
                //return (Convert.ToInt32((ldclTotalDebitAmount * 100))).ToString().PadLeft(12, '0');
                return (ldclTotalDebitAmount.ToString().Replace(".", "").ToString().PadLeft(12, '0'));
            }
        }
        public string ldclTotalCreditAmounttFormatted
        {
            get
            {
                //return (ldclTotalCreditAmount.ToString().PadLeft(12, '0'));
                return (ldclTotalCreditAmount.ToString().Replace(".", "").ToString().PadLeft(12, '0'));
            }
        }
        public void LoadTotalDebitAndCreditAmount()
        {
            if (iclbACHProvider != null)
            {
                foreach (busACHProviderReportData lobjACH in iclbACHProvider)
                {
                    if (lobjACH.ldclContributionAmount > 0 && istrBenefitType == "RECLAIM")
                        _ldclTotalCreditAmount += lobjACH.ldclContributionAmount;
                    else
                        _ldclTotalDebitAmount += lobjACH.ldclContributionAmount;

                    lobjACH.ldclContributionAmount = Math.Abs(lobjACH.ldclContributionAmount);
                }
                _ldclTotalCreditAmount = Math.Abs(_ldclTotalCreditAmount);
                _ldclTotalDebitAmount = Math.Abs(_ldclTotalDebitAmount);
            }
        }

        private string _lintBatchControlRecordCount;
        public string lintBatchControlRecordCount
        {
            get
            {
                if (iclbACHProvider != null)
                    _lintBatchControlRecordCount = Convert.ToString(iclbACHProvider.Count);
                return _lintBatchControlRecordCount.PadLeft(6, '0');
            }
        }

        private string _lintFileControlRecordCount;
        public string lintFileControlRecordCount
        {
            get
            {
                if (iclbACHProvider != null)
                    _lintFileControlRecordCount = Convert.ToString(iclbACHProvider.Count);
                return _lintFileControlRecordCount.PadLeft(8, '0');
            }
        }

        private string _lstrServiceClassCode;
        private string istrACHFilename;
        public string lstrServiceClassCode
        {
            get { return _lstrServiceClassCode; }
            set { _lstrServiceClassCode = value; }
        }

        public void LoadServiceClassCode()
        {
            if (iclbACHProvider != null)
            {
                int lintNegAdjCount = 0;
                foreach (busACHProviderReportData lobjACH in iclbACHProvider)
                {
                    if (lobjACH.ldclContributionAmount < 0)
                        lintNegAdjCount += 1;
                }
                if (lintNegAdjCount == iclbACHProvider.Count)
                    _lstrServiceClassCode = busConstant.ServiceCode_CreditsOnly;
                else if (lintNegAdjCount == 0)
                    _lstrServiceClassCode = busConstant.ServiceCode_DebitsOnly;
                else
                    _lstrServiceClassCode = busConstant.ServiceCode_CreditDebitMixed;
            }
        }

        public override void InitializeFile()
        {
            if (istrIsPullACH == busConstant.Flag_Yes)
                istrFileName = "ACHFile" + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;

            if (!string.IsNullOrEmpty(istrBenefitType))
                istrFileName = istrBenefitType + "_" + "ACHFile" + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;

            if (iblnIsPreNoteVerification || iblnIsACHPaymentDistribution)
            {
                if (Convert.ToString(this.iarrParameters[3]).IsNotNullOrEmpty())
                {
                    //istrFileName = "ACHFile" + "_" + Convert.ToString(this.iarrParameters[3]) + "_"+ DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
                    istrFileName = "xf00.acfhc964.w700.ACH" + "_" + Convert.ToString(this.iarrParameters[3]) + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
                }
                else
                {
                    //istrFileName = "ACHFile" + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
                    istrFileName = "xf00.acfhc964.w700.ACH" + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
                }
            }
            
            if (!iblnIsACHPaymentDistribution)
                //istrFileName = "ACHReclamationFile" + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
                istrFileName = "xf00.acfhc964.w700.ACHReclamation" + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;

            if (!string.IsNullOrEmpty(FileName))
                istrFileName = FileName + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
        }

        /// <summary>
        /// Generate ACH for Deferred Comp, Insurance and Retirement Benefit Type UCS-036
        /// </summary>
        /// <param name="ldtbProviderDeffComp"></param>
        public void LoadACHEntryDetail(DataTable ldtbProviderDeffComp)
        {
            iclbACHProvider = new Collection<busACHProviderReportData>();
            iclbACHProvider = (Collection<busACHProviderReportData>)iarrParameters[0];
            istrBenefitType = Convert.ToString(iarrParameters[1]);
            FileName = Convert.ToString(iarrParameters[2]);
            LoadDetailCounter();
            //LoadTotalDebitAndCreditAmount();
            //LoadServiceClassCode();
            //As per Satya, both Payment batch and Vendor payment batch, its credit
            ldclTotalCreditAmount = Math.Abs(iclbACHProvider.Sum(o => o.ldclContributionAmount));
            _lstrServiceClassCode = busConstant.ServiceCode_CreditsOnly;
            LoadTotalRoutingNo();
            LoadBlockCount();
        }

        /// <summary>
        /// Pull ACH Button from Deposit Tape Maintenance Screen UCS-033
        /// </summary>
        /// <param name="ldtbPullACH"></param>
        public void LoadPullACH(DataTable ldtbPullACH)
        {
            //iclbACHProvider = new Collection<busACHProviderReportData>();
            //iclbACHProvider = (Collection<busACHProviderReportData>)iarrParameters[0];
            //istrIsPullACH = Convert.ToString(iarrParameters[1]);
            //istrBenefitType = Convert.ToString(iarrParameters[2]);
            //FileName = Convert.ToString(iarrParameters[3]);
            //idtEffectiveDate = Convert.ToDateTime(iarrParameters[4]);
            //LoadDetailCounter();
            //if (!iblnIsACHPaymentDistribution)
            //{
            //    LoadServiceClassCode();
            //}
            //As per Satya, Pull Ach should have opposite to what both Payment batch and Vendor payment batch
            LoadTotalDebitAndCreditAmount();            
            LoadTotalRoutingNo();
            LoadBlockCount();
        }

        public void LoadDetailCounter()
        {
            int lintCounter = 1;
            foreach (busACHProviderReportData lobjData in iclbACHProvider)
            {
                lobjData.istrDetailCount = "09130028" + lintCounter.ToString().PadLeft(7, '0');
                lintCounter++;
            }
        }

        /// <summary>
        ///  UCS - 071 -- 7.4 ACH - Pre-Note Verification Batch
        /// </summary>
        /// <param name="ldtbPreNote"></param>
        public void LoadPreNoteVerificationACH(DataTable ldtbPreNoteOld)
        {
            //FileName = "PreNoteACHFile";
            FileName = "xf00.acfhc964.w700.ACHPreNote";
            // iblnIsPreNoteVerification = Convert.ToBoolean(iarrParameters[0]);
            iclbACHProvider = new Collection<busACHProviderReportData>();

            // Load Payee Account ACH Detail and Person Account ACH Detail
            //ldtbPreNote = busBase.Select("cdoPayeeAccountAchDetail.ACHPreNoteVerificationBatch", new object[] { });
            int lintDetailCounter = 1;
            bool lblnDebitExists = false, lblnCreditExists = false;
            DataTable ldtbPreNote = new DataTable();
            if (iarrParameters.Length > 0)
            {
                FileName = FileName + "_" + iarrParameters[0];
                if (iarrParameters[0] == "IAP")
                {
                    
                    var ldtbPreNoteCol = (from obj in ldtbPreNoteOld.AsEnumerable()
                                          where obj.Field<Int32>("PLAN_ID") == 1
                                          select obj);
                    if (ldtbPreNoteCol.Count() > 0)
                    {
                        ldtbPreNote = ldtbPreNoteCol.CopyToDataTable();
                    }
                    else
                    {
                        ldtbPreNote = ldtbPreNoteOld.Clone();
                    }
                }
                else
                {
                    var ldtbPreNoteCol = (from obj in ldtbPreNoteOld.AsEnumerable()
                                          where obj.Field<Int32>("PLAN_ID") != 1
                                          select obj);
                    if (ldtbPreNoteCol.Count() > 0)
                    {
                        ldtbPreNote = ldtbPreNoteCol.CopyToDataTable();
                    }
                    else
                    {
                        ldtbPreNote = ldtbPreNoteOld.Clone();
                    }
                }
            }
            foreach (DataRow dr in ldtbPreNote.Rows)
            {
                busACHProviderReportData lobjACH = new busACHProviderReportData();
                lobjACH.istrDetailCount = lintDetailCounter.ToString().PadLeft(7, '0');
                if (dr["ROUTING_NUMBER"] != DBNull.Value)
                {
                    lobjACH.lstrRoutingNumber = Convert.ToString(dr["ROUTING_NUMBER"]).PadLeft(9, '0');
                    lobjACH.istrRoutingNumberFirstEightDigits = lobjACH.lstrRoutingNumber.Substring(0, lobjACH.lstrRoutingNumber.Length - 1).PadLeft(8, '0');
                }
                if (dr["account_no"] != DBNull.Value)
                    lobjACH.lstrDFIAccountNo = Convert.ToString(dr["account_no"]);
                if (dr["person_id"] != DBNull.Value)
                    lobjACH.lintPersonID = Convert.ToInt32(dr["person_id"]);
                if (dr["mpi_person_id"] != DBNull.Value)
                {
                    lobjACH.lstrMPIPersonId = Convert.ToString(dr["mpi_person_id"]);
                    if (lobjACH.lstrMPIPersonId.Length < 15)
                        lobjACH.lstrMPIPersonId = lobjACH.lstrMPIPersonId.ToString().PadLeft(15, ' ');
                }
                if (dr["PERSON_NAME"] != DBNull.Value)
                {
                     lobjACH.lstrPersonName = Convert.ToString(dr["PERSON_NAME"]);
                    if (lobjACH.lstrPersonName.Length > 20)
                        lobjACH.lstrPersonName = lobjACH.lstrPersonName.Substring(0, 20);
                   
                }
                if (dr["transaction_code"] != DBNull.Value)
                    lobjACH.lstrTransactionCode = Convert.ToString(dr["transaction_code"]);
                if (dr["PLAN_ID"] != DBNull.Value)
                {
                    if (Convert.ToString(dr["PLAN_ID"]) != "1")
                    {
                        istrAccountNumber = "1000122978";
                        istrCompanyName = "MPIPHP";
                    }
                    else
                    {
                        istrAccountNumber = "1950030749"; //Company ID field for IAP ACH 
                        istrCompanyName = "MPIPHP IAP";
                    }
                }
                // Update Pre-Note Flag for Payee Account ACH Detail and Person Account ACH Detail
                string lstrACHType = string.Empty;
                int lintACHID = 0;
                if ((dr["type"] != DBNull.Value) &&
                    (dr["ach_detail_id"] != DBNull.Value))
                {
                    lstrACHType = Convert.ToString(dr["type"]);
                    lintACHID = Convert.ToInt32(dr["ach_detail_id"]);
                    if (lstrACHType == "PE")
                    {
                        busPayeeAccountAchDetail lobjPEACHDetail = new busPayeeAccountAchDetail();
                        lobjPEACHDetail.FindPayeeAccountAchDetail(lintACHID);
                        lobjPEACHDetail.icdoPayeeAccountAchDetail.pre_note_completion_date = DateTime.Now;
                        lobjPEACHDetail.icdoPayeeAccountAchDetail.pre_note_flag = busConstant.FLAG_NO;
                        lobjPEACHDetail.icdoPayeeAccountAchDetail.Update();
                        if (lobjACH.lstrTransactionCode == "SAVE")
                        {
                            lobjACH.lstrTransactionCode = busConstant.CreditTransactionCodePrenoteSavings;
                            lblnCreditExists = true;
                        }
                        else if (lobjACH.lstrTransactionCode == "CHKG")
                        {
                            lobjACH.lstrTransactionCode = busConstant.CreditTransactionCodePrenoteChecking;
                            lblnCreditExists = true;
                        }
                        else
                            lobjACH.lstrTransactionCode = "00";
                    }
                    else if (lstrACHType == "PA")
                    {
                        busPayeeAccountAchDetail lobjPAACHDetail = new busPayeeAccountAchDetail();
                        lobjPAACHDetail.FindPayeeAccountAchDetail(lintACHID);
                        lobjPAACHDetail.icdoPayeeAccountAchDetail.pre_note_completion_date = DateTime.Now;
                        lobjPAACHDetail.icdoPayeeAccountAchDetail.pre_note_flag = busConstant.Flag_Yes;
                        lobjPAACHDetail.icdoPayeeAccountAchDetail.Update();
                        if (lobjACH.lstrTransactionCode == "SAV")
                        {
                            lobjACH.lstrTransactionCode = busConstant.DebitTransactionCodePrenoteSavings;
                            lblnDebitExists = true;
                        }
                        else if (lobjACH.lstrTransactionCode == "CHK")
                        {
                            lobjACH.lstrTransactionCode = busConstant.DebitTransactionCodePrenoteChecking;
                            lblnDebitExists = true;
                        }
                        else
                            lobjACH.lstrTransactionCode = "00";
                    }
                }
                iclbACHProvider.Add(lobjACH);
                lintDetailCounter++;
            }

            if (idtEffectiveDate == DateTime.MinValue)
            {
                idtEffectiveDate = DateTime.Today;
            }

            if (lblnDebitExists && lblnCreditExists)
            {
                lstrServiceClassCode = busConstant.ServiceCode_CreditDebitMixed;
            }
            else if (lblnCreditExists)
                lstrServiceClassCode = busConstant.ServiceCode_CreditsOnly;
            else if (lblnDebitExists)
                lstrServiceClassCode = busConstant.ServiceCode_DebitsOnly;

            LoadTotalRoutingNo();
            LoadBlockCount();
           
            ChangePreNoteFlag(ldtbPreNote);
            
        }
        public void ChangePreNoteFlag(DataTable ldtbPreNote)
        {

            foreach (DataRow dr in ldtbPreNote.Rows)
            {
                //string lstrACHType = string.Empty;
                int lintACHID = 0;
                lintACHID = Convert.ToInt32(dr["ach_detail_id"]);
                //if (lstrACHType == "PE")
                //{
                    busPayeeAccountAchDetail lobjPEACHDetail = new busPayeeAccountAchDetail();
                    lobjPEACHDetail.FindPayeeAccountAchDetail(lintACHID);
                    if (lobjPEACHDetail != null)
                    {
                        lobjPEACHDetail.icdoPayeeAccountAchDetail.pre_note_flag = busConstant.FLAG_NO;
                        lobjPEACHDetail.icdoPayeeAccountAchDetail.Update();
                    }
                //}
            }
        }
        public void LoadBlockCount()
        {
            int lintRowCount = 4;    // Other than Detail Record count

            if (iclbACHProvider != null)
                lintRowCount += iclbACHProvider.Count;

            lintEmptyRowCount = 10 - (lintRowCount % 10);
            if (lintEmptyRowCount == 10)
                _lintBlockCount = lintRowCount / 10;
            else
                _lintBlockCount = (lintRowCount + lintEmptyRowCount) / 10;
        }

        

        public override bool ValidateFile()
        {
            // Generate the file only the collection has Detail records.
            if (iblnIsPreNoteVerification)
            {
                if (iclbACHProvider.Count == 0)
                    return false;
            }
            return true;
        }

        //public string  lstrTransactionCode {get;set;}

        public void LoadACHPaymentHistoryDistribution(DataTable adtACHPaymentDistribution)
        {
            //string lstrTransactionCode = string.Empty;
            iblnIsACHPaymentDistribution = Convert.ToBoolean(iarrParameters[0]);
            DataTable ldtACHPaymentDistribution = (DataTable)iarrParameters[1];
            if (iblnIsACHPaymentDistribution == busConstant.BOOL_TRUE)
            {
                idtEffectiveDate = Convert.ToDateTime(iarrParameters[2]);
                istrBenefitType = Convert.ToString(iarrParameters[3]);
            }
            else
            {
                idtEffectiveDate = DateTime.Now;
                istrBenefitType = "RECLAIM";            
            }


            iclbACHProvider = new Collection<busACHProviderReportData>();
            int lintCounter = 1;
            foreach (DataRow dr in ldtACHPaymentDistribution.Rows)
            {
                // Add to ACH file collection

                busACHProviderReportData lobjProviderReportData = new busACHProviderReportData();
                if (dr["ROUTING_NUMBER"] != DBNull.Value)
                {
                    lobjProviderReportData.lstrRoutingNumber = Convert.ToString(dr["ROUTING_NUMBER"]).PadLeft(9, '0');
                    lobjProviderReportData.istrRoutingNumberFirstEightDigits = lobjProviderReportData.lstrRoutingNumber.Substring(0, lobjProviderReportData.lstrRoutingNumber.Length - 1).PadLeft(8, '0');
                }
                
                if (dr["account_no"] != DBNull.Value)
                    lobjProviderReportData.lstrDFIAccountNo = Convert.ToString(dr["account_no"]);
                
                if (dr["person_id"] != DBNull.Value)
                    lobjProviderReportData.lintPersonID = Convert.ToInt32(dr["person_id"]);
                
                if (dr["IIN"] != DBNull.Value)
                {
                    lobjProviderReportData.lstrMPIPersonId ="P" +Convert.ToString(dr["IIN"]);
                    if (lobjProviderReportData.lstrMPIPersonId.Length < 15)
                        lobjProviderReportData.lstrMPIPersonId = lobjProviderReportData.lstrMPIPersonId.ToString().PadLeft(15, ' ');
                }
                
                if (dr["PERSON_NAME"] != DBNull.Value)
                {
                    lobjProviderReportData.lstrPersonName = Convert.ToString(dr["PERSON_NAME"]);
                    if (lobjProviderReportData.lstrPersonName.Length > 20)
                        lobjProviderReportData.lstrPersonName = lobjProviderReportData.lstrPersonName.Substring(0, 20);
                }
               
                if (dr["NET_AMOUNT"] != DBNull.Value)
                {
                    lobjProviderReportData.ldclContributionAmount = Convert.ToDecimal(dr["NET_AMOUNT"]);
                }
                
                if (dr["PLAN_ID"] != DBNull.Value)
                {
                    if (Convert.ToString(dr["PLAN_ID"]) != "1")
                    {
                        istrAccountNumber = "1000122978";
                        istrCompanyName = "MPIPHP";
                    }
                    else
                    {
                        istrAccountNumber = "1950030749";   //Company ID field for IAP ACH 
                        istrCompanyName = "MPIPHP IAP";
                    }
                }

                if (Convert.ToString(iarrParameters[2]) == busConstant.PAYMENT_DISTRIBUTION_STATUS_RECLAIMED)
                {
                    if (dr["TRANSACTION_CODE"] != DBNull.Value && Convert.ToString(dr["TRANSACTION_CODE"]) == busConstant.BankAccountSavings)
                        lobjProviderReportData.lstrTransactionCode = busConstant.DebitTransactionCodeNonPrenoteSavings;
                    else
                        lobjProviderReportData.lstrTransactionCode = busConstant.DebitTransactionCodeNonPrenoteChecking;

                    #region updating distribution status and taxes
                    busPaymentHistoryDistribution lobjPaymentHistoryDistribution = new busPaymentHistoryDistribution();
                    if (lobjPaymentHistoryDistribution.FindPaymentHistoryDistribution(Convert.ToInt32(dr["IIN"])))
                    {
                        lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_RECLAIMED;
                        lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();

                        busCheckReconciliationService lbusCheckReconciliationService = new busCheckReconciliationService();
                        lbusCheckReconciliationService.InsertIntoPaymentStatusHistory(lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id,
                            lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.PAYMENT_DISTRIBUTION_STATUS_RECLAIMED, DateTime.Now);

                        Collection<cdoProviderReportPayment> lclbProviderReportPayment = new Collection<cdoProviderReportPayment>();
                        DataTable ldtlGetTaxes = busBase.Select("cdoPaymentHistoryHeader.GetTaxesforReclamation", new object[1] { lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id });
                        if (ldtlGetTaxes.Rows.Count > 0)
                        {
                            lclbProviderReportPayment = cdoProviderReportPayment.GetCollection<cdoProviderReportPayment>(ldtlGetTaxes);

                            foreach (cdoProviderReportPayment lcdoProviderReportPayment in lclbProviderReportPayment)
                            {
                                busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
                                lbusPayeeAccount.InsertIntoProviderReportPayment(busConstant.SUSSYSTEM_TYPE_PAYMENT_RECLAIMED, lcdoProviderReportPayment.subsystem_ref_id,
                                    lcdoProviderReportPayment.person_id, lcdoProviderReportPayment.provider_org_id, lcdoProviderReportPayment.payee_account_id,
                                    -(lcdoProviderReportPayment.amount), lcdoProviderReportPayment.payment_history_header_id, lcdoProviderReportPayment.payment_item_type_id);
                            }
                        }
                    }
                    #endregion

                }
                else
                {
                    //HotFix 07312013 for separate transaction code for saving and checking 
                    if (dr["TRANSACTION_CODE"] != DBNull.Value && Convert.ToString(dr["TRANSACTION_CODE"]) == busConstant.BankAccountSavings)
                        lobjProviderReportData.lstrTransactionCode = busConstant.CreditTransactionCodeNonPrenoteSavings;
                    else
                        lobjProviderReportData.lstrTransactionCode = busConstant.CreditTransactionCodeNonPrenoteChecking;
                }
                    //lobjProviderReportData.lstrTransactionCode = busConstant.CreditTransactionCodeNonPrenoteChecking;

                lobjProviderReportData.istrDetailCount = lintCounter.ToString().PadLeft(7, '0');

                if (!string.IsNullOrEmpty(lobjProviderReportData.lstrDFIAccountNo))
                {
                    iclbACHProvider.Add(lobjProviderReportData);
                    lintCounter++;
                }
            }
            LoadPullACH(adtACHPaymentDistribution);

            if (Convert.ToString(iarrParameters[2]) == busConstant.PAYMENT_DISTRIBUTION_STATUS_RECLAIMED)
                _lstrServiceClassCode = busConstant.ServiceCode_DebitsOnly;
            else
                _lstrServiceClassCode = busConstant.ServiceCode_CreditsOnly;
        }
        #region commented code
        /// <summary>
        ///  UCS-071 Create Pre-Note Transmittal Register Report
        /// </summary>
        //public void CreatePreNoteTransmittalReport()
        //{
        //    DataTable ldtbReportTable = new DataTable();
        //    ldtbReportTable.TableName = busConstant.ReportTableName;
        //    // Defining the Columns in DataTable
        //    DataColumn ldcBatchCount = new DataColumn("BatchCount", Type.GetType("System.Int32"));
        //    DataColumn ldcBlockCount = new DataColumn("BlockCount", Type.GetType("System.Int32"));
        //    DataColumn ldcAddendaCount = new DataColumn("AddendaCount", Type.GetType("System.Int32"));
        //    DataColumn ldcHashCount = new DataColumn("HashCount", Type.GetType("System.String"));
        //    DataColumn ldclCreditAmount = new DataColumn("TotalCredit", Type.GetType("System.Decimal"));
        //    DataColumn ldclDebitAmount = new DataColumn("TotalDebit", Type.GetType("System.Decimal"));
        //    DataColumn ldclEffectiveDate = new DataColumn("EffectiveDate", Type.GetType("System.DateTime"));
        //    DataColumn ldclFilename = new DataColumn("Filename", Type.GetType("System.String"));
        //    // Adding the Columns in DataTable
        //    ldtbReportTable.Columns.Add(ldcBatchCount);
        //    ldtbReportTable.Columns.Add(ldcBlockCount);
        //    ldtbReportTable.Columns.Add(ldcAddendaCount);
        //    ldtbReportTable.Columns.Add(ldcHashCount);
        //    ldtbReportTable.Columns.Add(ldclCreditAmount);
        //    ldtbReportTable.Columns.Add(ldclDebitAmount);
        //    ldtbReportTable.Columns.Add(ldclEffectiveDate);
        //    ldtbReportTable.Columns.Add(ldclFilename);
        //    // Adding Values to the Cells.
        //    DataRow dr = ldtbReportTable.NewRow();
        //    dr[ldcBatchCount] = 1;
        //    dr[ldcBlockCount] = _lintBlockCount;
        //    if (lintFileControlRecordCount != null)
        //        dr[ldcAddendaCount] = lintFileControlRecordCount;
        //    dr[ldcHashCount] = istrEntryHash ?? string.Empty;
        //    dr[ldclCreditAmount] = (iblnIsPreNoteVerification || istrIsPullACH == busConstant.Flag_Yes) ? 0.0M : ldclTotalCreditAmount;
        //    dr[ldclDebitAmount] = istrIsPullACH == busConstant.Flag_Yes ? ldclTotalDebitAmount : 0.0M;
        //    dr[ldclEffectiveDate] = idtEffectiveDate == DateTime.MinValue ? ldtTodaysDate : idtEffectiveDate;
        //    if( istrIsPullACH==busConstant.Flag_Yes && FileName.IsEmpty())
        //    {
        //        dr[ldclFilename] = "DF.BK870024" + " "+busConstant.InsurancePremiums;
        //    }
        //    else if (iblnIsPreNoteVerification)
        //    {
        //        dr[ldclFilename] = "DF.BK870024" + " "+ busConstant.RetirementAndInsurancePrenote;
        //    }
        //    else if(iblnIsACHPaymentDistribution)
        //    {
        //        dr[ldclFilename] = "DF.BK870002" + " " + busConstant.PensionPayments;
        //    }
        //    else
        //    {
        //        dr[ldclFilename] = FileName + " " + istrACHFilename;
        //    }
        //    ldtbReportTable.Rows.Add(dr);
        //    // Create Report Method.
        //    busNeoSpinBase lobjNSBase = new busNeoSpinBase();
        //    string lstrReportName = !string.IsNullOrEmpty(istrFileName) && istrFileName.LastIndexOf(".") > 0 ?
        //        istrFileName.Substring(0, istrFileName.LastIndexOf(".")) : "ACHTransmittalReport" + DateTime.Now.ToString(busConstant.DateFormat);
        //    lobjNSBase.CreateReportWithGivenName("rptPreNoteTransmittalRegister.rpt", ldtbReportTable, lstrReportName);
        //}
        //public override void FinalizeFile()
        //{
        //    if (lintEmptyRowCount != 10)
        //    {
        //        string istr = string.Empty;
        //        for (int i = 0; i < 94; i++)
        //            istr += "9";
        //        for (int i = 0; i < lintEmptyRowCount; i++)
        //            iswrOut.WriteLine(istr);
        //    }

        //    if ((iblnIsPreNoteVerification || FileName == busConstant.ACHFileNameDefCompVendorPayment || FileName == busConstant.ACHFileNameInsuranceVendorPayment ||
        //        FileName == busConstant.ACHFileNameRetirmentVendorPayment || istrIsPullACH == busConstant.Flag_Yes || iblnIsACHPaymentDistribution) && (iclbACHProvider != null))
        //    {
        //        if (FileName == busConstant.ACHFileNameRetirmentVendorPayment)
        //            istrACHFilename = busConstant.RetirementVendorPayment;
        //        else if (FileName == busConstant.ACHFileNameInsuranceVendorPayment)
        //            istrACHFilename = busConstant.InsuranceVendorPayment;
        //        else if (FileName == busConstant.ACHFileNameDefCompVendorPayment)
        //            istrACHFilename = busConstant.DefferedcompVendorPayment;
        //        else if (FileName == busConstant.ACHFileNameRetirmentEmployerPayment)
        //            istrACHFilename = busConstant.RetirementEmployerPayment;
        //        else if (FileName == busConstant.ACHFileNameInsuranceEmployerPayment)
        //            istrACHFilename = busConstant.InsuranceEmployerPayment;
        //        else if (FileName == busConstant.ACHFileNameDefCompEmployerPayment)
        //            istrACHFilename = busConstant.DefferedcompEmployerPayment;
        //        if (iclbACHProvider.Count > 0)
        //            CreatePreNoteTransmittalReport();
        //    }
        //}
        /// <summary>
        /// Method to load the ACH provider collection with data
        /// </summary>
        /// <param name="adtACHPaymentDistribution">Data table</param>
        //public void LoadACHPaymentHistoryDistribution(DataTable adtACHPaymentDistribution)
        //{
        //    string lstrTransactionCode = string.Empty;
        //    iblnIsACHPaymentDistribution = Convert.ToBoolean(iarrParameters[0]);
        //    DataTable ldtACHPaymentDistribution = (DataTable)iarrParameters[1];
        //    idtEffectiveDate = Convert.ToDateTime(iarrParameters[2]);
        //    iclbACHProvider = new Collection<busACHProviderReportData>();
        //    int lintCounter = 1;
        //    foreach (DataRow dr in ldtACHPaymentDistribution.Rows)
        //    {
        //        // Add to ACH file collection
        //        busACHProviderReportData lobjProviderReportData = new busACHProviderReportData();
        //        if (dr["PERSON_ID"] != DBNull.Value)
        //            lobjProviderReportData.lintPersonID = Convert.ToInt32(dr["PERSON_ID"]);

        //        if (dr["NET_AMOUNT"] != DBNull.Value)
        //            lobjProviderReportData.ldclContributionAmount = Convert.ToDecimal(dr["NET_AMOUNT"]);

        //        if (dr["ACCOUNT_NUMBER"] != DBNull.Value)
        //            lobjProviderReportData.lstrDFIAccountNo = Convert.ToString(dr["ACCOUNT_NUMBER"]);

        //        if (dr["ROUTING_NUMBER"] != DBNull.Value)
        //        {
        //            lobjProviderReportData.lstrRoutingNumber = Convert.ToString(dr["ROUTING_NUMBER"]);
        //            lobjProviderReportData.istrRoutingNumberFirstEightDigits = Convert.ToString(dr["ROUTING_NUMBER"])
        //                .Substring(0, Convert.ToString(dr["ROUTING_NUMBER"]).Length - 1).PadLeft(8, '0');
        //            lobjProviderReportData.istrCheckLastDigit = Convert.ToString(dr["ROUTING_NUMBER"])
        //                                                                   .Substring(Convert.ToString(dr["ROUTING_NUMBER"]).Length - 1, 1);
        //        }

        //        lstrTransactionCode = (dr["TRANSACTION_CODE"] != DBNull.Value ? Convert.ToString(dr["TRANSACTION_CODE"]) : string.Empty);
        //        if (lstrTransactionCode == busConstant.PersonAccountBankAccountSavings)
        //            lobjProviderReportData.lstrTransactionCode = lobjProviderReportData.ldclContributionAmount >= 0 ?
        //                busConstant.CreditTransactionCodeNonPrenoteSavings : busConstant.DebitTransactionCodeNonPrenoteSavings;
        //        else if (lstrTransactionCode == busConstant.PersonAccountBankAccountChecking)
        //            lobjProviderReportData.lstrTransactionCode = lobjProviderReportData.ldclContributionAmount >= 0 ?
        //                busConstant.CreditTransactionCodeNonPrenoteChecking : busConstant.DebitTransactionCodePrenoteChecking;
        //        lobjProviderReportData.istrDetailCount = "09130028" + lintCounter.ToString().PadLeft(7, '0');
        //        if (!string.IsNullOrEmpty(lobjProviderReportData.lstrDFIAccountNo))
        //        {
        //            iclbACHProvider.Add(lobjProviderReportData);
        //            lintCounter++;
        //        }
        //    }

        //    //uat pir - 2205 : from payroll only credit transactions
        //    ldclTotalCreditAmount = Math.Abs(iclbACHProvider.Sum(o => o.ldclContributionAmount));
        //    _lstrServiceClassCode = busConstant.ServiceCode_CreditsOnly;
        //    LoadTotalRoutingNo();
        //    LoadBlockCount();
        //}
        #endregion
    }
}
