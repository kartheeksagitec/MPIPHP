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
using Sagitec.CorBuilder;
using Sagitec.CustomDataObjects;

namespace MPIPHP.BusinessObjects.PayeeAccount
{
    [Serializable]
    public class busACHNotificationofChangeandReturnInboundFile : busFileBase
    {
        bool iblnIsValid = false;
        //property to store the data read from file
        public busPaymentHistoryDistribution ibusPaymentHistoryDistributionDetail { get; set; }
        //property to load the data
        public Collection<busPaymentHistoryDistribution> iclbPaymentHistoryDitribution { get; set; }
        public Collection<busPayeeAccountAchDetail> iclbPayeeAccountAchDetail { get; set; }
        public string istrEntryDetailIdentificationNumberFirstChar { get; set; }
        public int iintPersonId { get; set; }
        public string istrMPIPersonId { get; set; }
        public int iintPayeeAccountId { get; set; }
        public bool iblnIsPaymentRecord { get; set; }
        public int iintPaymentDistributionId { get; set; }
        CorBuilder lobjCorBuilder = null;
        public bool iblnIsACHAleadyEndDated { get; set; }
        public bool iblnIsReclaimed { get; set; }


        private static int lintHeaderGroupValue = 0;
        private string lstrHeaderGroupValue = "0";

        public busACHNotificationofChangeandReturnInboundFile()
        {
            iclbPaymentHistoryDitribution = new Collection<busPaymentHistoryDistribution>();
        }

        public override void SetHeaderGroupValue()
        {
            if (icdoFileDtl != null && icdoFileDtl.transaction_code_value.IsNotNullOrEmpty())
            {
                if (icdoFileDtl.transaction_code_value == "5")
                {
                    lintHeaderGroupValue++;
                    if (lintHeaderGroupValue < 10)
                        lstrHeaderGroupValue = "0" + lintHeaderGroupValue.ToString();
                    else
                        lstrHeaderGroupValue = lintHeaderGroupValue.ToString();
                }
                icdoFileDtl.header_group_value = lstrHeaderGroupValue;
            }
        }

        public override busBase NewHeader()
        {
            return base.NewHeader();
        }

        public override busBase NewDetail()
        {
            ibusPaymentHistoryDistributionDetail = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
            return ibusPaymentHistoryDistributionDetail;
        }

        public override string BeforeFieldAssigned(string astrFieldName, string astrFieldValue)
        {
            string lstrReturnValue = astrFieldValue;
            return lstrReturnValue;
        }

        #region Process Detail
        public override void ProcessDetail()
        {
            if (ibusPaymentHistoryDistributionDetail.istrEntryDetailIdentificationNumber.IsNotNullOrEmpty())
            {
                istrEntryDetailIdentificationNumberFirstChar = ibusPaymentHistoryDistributionDetail.istrEntryDetailIdentificationNumber.Trim().Substring(0, 1);

                if (istrEntryDetailIdentificationNumberFirstChar == "M")
                    istrMPIPersonId = ibusPaymentHistoryDistributionDetail.istrEntryDetailIdentificationNumber.Trim();
                else if (istrEntryDetailIdentificationNumberFirstChar == "P")
                {
                    iblnIsACHAleadyEndDated = false; //154673
                    iblnIsPaymentRecord = true;
                    iintPaymentDistributionId = Convert.ToInt32(ibusPaymentHistoryDistributionDetail.istrEntryDetailIdentificationNumber.Trim().Substring(1, ibusPaymentHistoryDistributionDetail.istrEntryDetailIdentificationNumber.Trim().Length - 1));
                }
            }


            if (istrEntryDetailIdentificationNumberFirstChar == "P" && icdoFileDtl.record_data.Substring(29, 10).Trim().IsNotNullOrEmpty()
                && Convert.ToInt32(icdoFileDtl.record_data.Substring(29, 10).Trim()) != 0)
            {
                iblnIsReclaimed = false;
                ibusPaymentHistoryDistributionDetail.istrEntryDetailIdentificationNumber = ibusPaymentHistoryDistributionDetail.istrEntryDetailIdentificationNumber.Trim().Substring(1, ibusPaymentHistoryDistributionDetail.istrEntryDetailIdentificationNumber.Trim().Length - 1);

                busPaymentHistoryDistribution lobjPaymentHistoryDistribution = new busPaymentHistoryDistribution();
                if (lobjPaymentHistoryDistribution.FindPaymentHistoryDistribution(Convert.ToInt32(ibusPaymentHistoryDistributionDetail.istrEntryDetailIdentificationNumber)))
                {

                    if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payee_account_ach_detail_id > 0 &&
                        lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value != busConstant.PAYMENT_DISTRIBUTION_STATUS_RECLAIMED)
                    {
                        busPayeeAccountAchDetail lobjPayeeAccountAchDetail = new busPayeeAccountAchDetail();
                        if (lobjPayeeAccountAchDetail.FindPayeeAccountAchDetail(lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payee_account_ach_detail_id))
                        {
                            iblnIsACHAleadyEndDated = false;
                            if (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date == DateTime.MinValue)
                            {
                                if (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date < DateTime.Today)
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.Now.AddDays(-1);
                                else
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date;

                                lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.Update();

                                iblnIsACHAleadyEndDated = true;
                            }
                        }
                    }
                    if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id > 0)
                    {
                        busPaymentHistoryHeader lobjPaymentHistoryHeader = new busPaymentHistoryHeader();

                        if (lobjPaymentHistoryHeader.FindPaymentHistoryHeader(lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id))
                        {
                            ///Code to initiate new workflow
                            Hashtable lhstRequestParams = new Hashtable();
                            lhstRequestParams.Add("PayeeAccountId", lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id);
                            lhstRequestParams.Add("PaymentDateFrom", string.Format("{0:MM/dd/yyyy}", lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date.Date));
                            lhstRequestParams.Add("PaymentDateTo", string.Format("{0:MM/dd/yyyy}", lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date.Date));

                            lobjPaymentHistoryHeader.ibusPayeeAccount = new busPayeeAccount();
                            if (lobjPaymentHistoryHeader.ibusPayeeAccount.FindPayeeAccount(lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id))
                                busWorkflowHelper.InitializeWorkflow(busConstant.PROCESS_STOP_REISSUE_OR_RECLAMATION, lobjPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.person_id,
                                                                            0, 0, lhstRequestParams);

                            if (lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id > 0 && iblnIsACHAleadyEndDated)
                            {
                                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                lbusPayeeAccount.icdoPayeeAccount.payee_account_id = lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id;
                                lbusPayeeAccount.CreateReviewPayeeAccountStatus();
                            }
                            busWorkflowHelper.InitializeWorkflow(busConstant.UPDATE_PAYEE_ACCOUNT, lobjPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.person_id,
                                        0, lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id, null);


                            #region payment history distribution from Reclaimed to Cleared.
                            if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_RECLAIMED)
                            {
                                iblnIsReclaimed = true;
                                lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_CLEARED;
                                lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();

                                busCheckReconciliationService lbusCheckReconciliationService = new busCheckReconciliationService();
                                lbusCheckReconciliationService.InsertIntoPaymentStatusHistory(lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id,
                                lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.PAYMENT_DISTRIBUTION_STATUS_CLEARED, DateTime.Now);

                                Collection<cdoProviderReportPayment> lclbProviderReportPayment = new Collection<cdoProviderReportPayment>();
                                DataTable ldtlGetTaxes = busBase.Select("cdoPaymentHistoryHeader.GetTaxesforReclamation", new object[1] { lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id });
                                if (ldtlGetTaxes.Rows.Count > 0)
                                {
                                    lclbProviderReportPayment = cdoProviderReportPayment.GetCollection<cdoProviderReportPayment>(ldtlGetTaxes);

                                    foreach (cdoProviderReportPayment lcdoProviderReportPayment in lclbProviderReportPayment)
                                    {
                                        lobjPaymentHistoryHeader.ibusPayeeAccount.InsertIntoProviderReportPayment(busConstant.SUSSYSTEM_TYPE_PAYMENT_RECLAIMED, lcdoProviderReportPayment.subsystem_ref_id,
                                            lcdoProviderReportPayment.person_id, lcdoProviderReportPayment.provider_org_id, lcdoProviderReportPayment.payee_account_id,
                                            lcdoProviderReportPayment.amount, lcdoProviderReportPayment.payment_history_header_id, lcdoProviderReportPayment.payment_item_type_id);
                                    }
                                }
                            }
                            else
                            {
                                if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.PAYMENT_DISTRIBUTION_STATUS_CLEARED)
                                {
                                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_OUTSTANDING;
                                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();

                                    busCheckReconciliationService lbusCheckReconciliationService = new busCheckReconciliationService();
                                    lbusCheckReconciliationService.InsertIntoPaymentStatusHistory(lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id,
                                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.PAYMENT_DISTRIBUTION_STATUS_OUTSTANDING, DateTime.Now);
                                }
                            }
                            #endregion

                        }
                    }

                }
            }
            else if ((istrMPIPersonId.IsNotNullOrEmpty() || (iblnIsPaymentRecord && iintPaymentDistributionId > 0)) && this.icdoFileDtl.transaction_code_value == "7")
            {

                if (istrMPIPersonId.IsNotNullOrEmpty())
                {
                    busPerson lbusPerson = new busPerson();
                    if (lbusPerson.FindPerson(istrMPIPersonId))
                    {
                        iintPersonId = lbusPerson.icdoPerson.person_id;
                        this.LoadAllACHData(lbusPerson.icdoPerson.person_id);
                        iblnIsReclaimed = false;
                    }
                }
                else
                {
                    busPaymentHistoryDistribution lbusPaymentHistoryDistribution = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
                    if (lbusPaymentHistoryDistribution.FindPaymentHistoryDistribution(iintPaymentDistributionId))
                    {

                        this.iclbPayeeAccountAchDetail = new Collection<busPayeeAccountAchDetail>();
                        busPayeeAccountAchDetail lbusPayeeAccountAchDetail = new busPayeeAccountAchDetail { icdoPayeeAccountAchDetail = new cdoPayeeAccountAchDetail() };
                        if (lbusPayeeAccountAchDetail.FindPayeeAccountAchDetail(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payee_account_ach_detail_id))
                        {
                            this.iclbPayeeAccountAchDetail.Add(lbusPayeeAccountAchDetail);

                            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            if (lbusPayeeAccount.FindPayeeAccount(lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.payee_account_id))
                            {
                                iintPersonId = lbusPayeeAccount.icdoPayeeAccount.person_id;
                            }
                        }


                    }
                }

                if (this.iclbPayeeAccountAchDetail != null)
                {
                    foreach (busPayeeAccountAchDetail lobjPayeeAccountAchDetail in this.iclbPayeeAccountAchDetail)
                    {
                        bool IsACHEndDated = false;
                        iintPayeeAccountId = lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.payee_account_id;

                        if (!iblnIsReclaimed)
                        {
                            if (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date == DateTime.MinValue)
                            {
                                if (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date < DateTime.Today)
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.Now.AddDays(-1);
                                else
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date;
                                lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.Update();

                                busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.UPDATE_PAYEE_ACCOUNT, iintPersonId, 0, iintPayeeAccountId, null);
                            }
                            else if (!iblnIsACHAleadyEndDated)
                            {
                                IsACHEndDated = true;
                            }
                        }

                        busOrganization lobjOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                        if (!IsACHEndDated)
                        {
                            switch (icdoFileDtl.record_data.Substring(3, 3).Trim())
                            {
                                case "C01":
                                    //Account Number Change
                                    if (icdoFileDtl.record_data.Substring(35, 28).Trim().IsNotNullOrEmpty())
                                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.account_number = icdoFileDtl.record_data.Substring(35, 28).Trim();

                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.MinValue;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.pre_note_completion_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.created_by = iobjPassInfo.istrUserID;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.Insert();
                                    this.GenerateCorrespondence(iintPayeeAccountId);
                                    break;

                                case "C02":
                                    if (lobjOrganization.FindOrganization(lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_org_id))
                                    {
                                        if (icdoFileDtl.record_data.Substring(35, 28).Trim().IsNotNullOrEmpty())
                                        {
                                            //Routing Number Change
                                            lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_org_id =
                                                InsertNewRetrieveOrganization(lobjOrganization, icdoFileDtl.record_data.Substring(35, 28).Trim());
                                        }
                                    }
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.MinValue;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.pre_note_completion_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.created_by = iobjPassInfo.istrUserID;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.Insert();
                                    this.GenerateCorrespondence(iintPayeeAccountId);
                                    break;

                                case "C05":
                                    //Transaction Code Change
                                    if ((icdoFileDtl.record_data.Substring(35, 28).Trim().IsNotNullOrEmpty()) && (icdoFileDtl.record_data.Substring(35, 28).Trim() == "33" || icdoFileDtl.record_data.Substring(35, 28).Trim() == "32"))
                                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_account_type_value = busConstant.BankAccountSavings;
                                    else if ((icdoFileDtl.record_data.Substring(35, 28).Trim().IsNotNullOrEmpty()) && (icdoFileDtl.record_data.Substring(35, 28).Trim() == "22" || icdoFileDtl.record_data.Substring(35, 28).Trim() == "23"))
                                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_account_type_value = busConstant.BankAccountChecking;

                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.MinValue;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.pre_note_completion_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.created_by = iobjPassInfo.istrUserID;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.Insert();
                                    this.GenerateCorrespondence(iintPayeeAccountId);

                                    break;


                                case "C06":
                                    //Account Number Change
                                    if (icdoFileDtl.record_data.Substring(35, 16).Trim().IsNotNullOrEmpty())
                                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.account_number = icdoFileDtl.record_data.Substring(35, 16).Trim();

                                    //Transaction Code Change
                                    if ((icdoFileDtl.record_data.Substring(55, 2).Trim().IsNotNullOrEmpty()) && (icdoFileDtl.record_data.Substring(55, 2).Trim() == "33" || icdoFileDtl.record_data.Substring(55, 2).Trim() == "32"))
                                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_account_type_value = busConstant.BankAccountSavings;
                                    else if ((icdoFileDtl.record_data.Substring(55, 2).Trim().IsNotNullOrEmpty()) && (icdoFileDtl.record_data.Substring(55, 2).Trim() == "22" || icdoFileDtl.record_data.Substring(55, 2).Trim() == "23"))
                                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_account_type_value = busConstant.BankAccountChecking;

                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.MinValue;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.pre_note_completion_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.created_by = iobjPassInfo.istrUserID;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.Insert();
                                    this.GenerateCorrespondence(iintPayeeAccountId);

                                    break;

                                case "C03":
                                    //Account Number Change
                                    if (icdoFileDtl.record_data.Substring(47, 16).Trim().IsNotNullOrEmpty())
                                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.account_number = icdoFileDtl.record_data.Substring(47, 16).Trim();

                                    if (lobjOrganization.FindOrganization(lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_org_id))
                                    {
                                        if (icdoFileDtl.record_data.Substring(35, 9).Trim().IsNotNullOrEmpty())
                                        {
                                            //Routing Number Change

                                            lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_org_id =
                                                InsertNewRetrieveOrganization(lobjOrganization, icdoFileDtl.record_data.Substring(35, 9).Trim());
                                        }
                                    }

                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.MinValue;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.pre_note_completion_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.created_by = iobjPassInfo.istrUserID;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.Insert();


                                    this.GenerateCorrespondence(iintPayeeAccountId);
                                    break;

                                case "C07":
                                    if (lobjOrganization.FindOrganization(lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_org_id))
                                    {
                                        if (icdoFileDtl.record_data.Substring(35, 9).Trim().IsNotNullOrEmpty())
                                        {
                                            //Routing Number Change
                                            lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_org_id =
                                                InsertNewRetrieveOrganization(lobjOrganization, icdoFileDtl.record_data.Substring(35, 9).Trim());
                                        }
                                    }

                                    //Account Number Change
                                    if (icdoFileDtl.record_data.Substring(44, 16).Trim().IsNotNullOrEmpty())
                                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.account_number = icdoFileDtl.record_data.Substring(44, 16).Trim();

                                    //Transaction Code Change
                                    if ((icdoFileDtl.record_data.Substring(61, 2).Trim().IsNotNullOrEmpty()) && (icdoFileDtl.record_data.Substring(61, 2).Trim() == "33" || icdoFileDtl.record_data.Substring(61, 2).Trim() == "32"))
                                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_account_type_value = busConstant.BankAccountSavings;
                                    else if ((icdoFileDtl.record_data.Substring(61, 2).Trim().IsNotNullOrEmpty()) && (icdoFileDtl.record_data.Substring(61, 2).Trim() == "22" || icdoFileDtl.record_data.Substring(61, 2).Trim() == "23"))
                                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_account_type_value = busConstant.BankAccountChecking;

                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.MinValue;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.pre_note_completion_date = DateTime.Now;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.created_by = iobjPassInfo.istrUserID;
                                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.Insert();
                                    this.GenerateCorrespondence(iintPayeeAccountId);

                                    break;
                            }

                        }

                    }


                    iintPersonId = 0;
                    iintPayeeAccountId = 0;
                    istrMPIPersonId = string.Empty;

                }
                iblnIsPaymentRecord = false;
                iintPaymentDistributionId = 0;
            }
            istrEntryDetailIdentificationNumberFirstChar = string.Empty;
        }


        private int InsertNewRetrieveOrganization(busOrganization abusOrganization, string astrRoutingNumber)
        {
            int lintOrgId = 0;


            DataTable ldtOrganization = busBase.Select<cdoOrganization>(
                new string[2] { "routing_number", "status_value" },
            new object[2] { astrRoutingNumber, busConstant.ORGANIZATION_STATUS_ACTIVE }, null, null);


            if (ldtOrganization != null && ldtOrganization.Rows.Count > 0)
            {
                lintOrgId = Convert.ToInt32(ldtOrganization.Rows[0][enmOrganization.org_id.ToString().ToUpper()]);
            }
            else
            {
                abusOrganization.icdoOrganization.mpi_org_id = string.Empty;
                abusOrganization.icdoOrganization.org_id = 0;
                abusOrganization.icdoOrganization.status_value = busConstant.ORGANIZATION_STATUS_ACTIVE;
                abusOrganization.icdoOrganization.routing_number = astrRoutingNumber;
                abusOrganization.icdoOrganization.created_by = iobjPassInfo.istrUserID;
                abusOrganization.icdoOrganization.created_date = DateTime.Now;
                abusOrganization.icdoOrganization.modified_by = iobjPassInfo.istrUserID;
                abusOrganization.icdoOrganization.modified_date = DateTime.Now;
                abusOrganization.icdoOrganization.update_seq = 0;

                if (string.IsNullOrEmpty(abusOrganization.icdoOrganization.mpi_org_id))
                {
                    cdoCodeValue lobjcdoCodeValue = HelperUtil.GetCodeValueDetails(52, busConstant.MPID);
                    int lintNewOrgID = Convert.ToInt32(lobjcdoCodeValue.data1);
                    abusOrganization.icdoOrganization.mpi_org_id = "M" + lintNewOrgID.ToString("D8");

                    lintNewOrgID += 1;
                    lobjcdoCodeValue.data1 = lintNewOrgID.ToString();
                    lobjcdoCodeValue.Update();

                }

                abusOrganization.icdoOrganization.Insert();
                //abusOrganization.icdoOrganization.Select();

                lintOrgId = abusOrganization.icdoOrganization.org_id;
            }


            return lintOrgId;
        }

        #endregion

        #region Create Correspondence
        public void GenerateCorrespondence(int aintPayeeAccountID)
        {
            busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
            if (lbusPayeeAccount.FindPayeeAccount(aintPayeeAccountID))
            {
                lbusPayeeAccount.ibusParticipant = new busPerson();
                if (lbusPayeeAccount.ibusParticipant.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id))
                {
                    ArrayList aarrResult = new ArrayList();
                    Hashtable ahtbQueryBkmarks = new Hashtable();
                    busCheckReconciliationService lbusCheckReconciliationService = new busCheckReconciliationService();
                    aarrResult.Add(lbusPayeeAccount);
                    lbusCheckReconciliationService.CreateCorrespondence(busConstant.NOTIFICATION_OF_CHANGE_ACH, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                }
            }
        }
        #endregion

        #region Loading All ACH data for Perosn
        public void LoadAllACHData(int aintPersonID)
        {
            busBase lobjBase = new busBase();
            DataTable ldtACHNotificationChangeandReturn = busBase.Select("cdoPaymentHistoryDistribution.LoadfleDetailsForACHNotificationChangeandReturn", new object[1] { aintPersonID });
            iclbPayeeAccountAchDetail = lobjBase.GetCollection<busPayeeAccountAchDetail>(ldtACHNotificationChangeandReturn, "icdoPayeeAccountAchDetail");
        }
        #endregion
    }
}
