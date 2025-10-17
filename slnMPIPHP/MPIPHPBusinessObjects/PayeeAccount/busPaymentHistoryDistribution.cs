#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPaymentHistoryDistribution:
	/// Inherited from busPaymentHistoryDistributionGen, the class is used to customize the business object busPaymentHistoryDistributionGen.
	/// </summary>
	[Serializable]
	public class busPaymentHistoryDistribution : busPaymentHistoryDistributionGen
	{
        public busPaymentHistoryHeader ibusPaymentHistoryHeader { get; set; }
        public Collection<busPaymentReissueDetail> iclbPaymentReissueDetail { get; set; }
        public Collection<busPerson> iclbPerson { get; set; }
        public int iintRecordCode { get; set; }
        public string istrRecordCode { get; set; }
        public string istrStatusCode { get; set; }
        public string istrPayeeMPID { get; set; }

        public string istrOrgID { get; set; }

        #region for ACH Change and return file
        public string istrEntryDetailIdentificationNumber { get; set; }
        #endregion 

        public string istrCurrentDate { get; set; }
        public string istrCheckACHDate { get; set; }
        public string istrPlanDescription { get; set; }

        public Collection<cdoPerson> iclbSurvivorNames { get; set; }

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();

            if (this.iclbPaymentReissueDetail.Count > 0)
            {
                DateTime ldtNextPaymentDate = new DateTime();
                int lintPayeeAccountPaymentTypeId = 0;

                #region  GET Next Payment Date ADHOC

                this.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadNextBenefitPaymentDate();
                this.ibusPaymentHistoryHeader.ibusPayeeAccount.iintPaymentReissueDetailId = 0;

                ldtNextPaymentDate = this.ibusPaymentHistoryHeader.ibusPayeeAccount.idtNextBenefitPaymentDate;
                this.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadBenefitDetails();
                this.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();

                #endregion  GET Next Payment Date ADHOC

                //PIR-622
                if (iclbSurvivorNames != null && iclbSurvivorNames.Count > 0)
                {

                    foreach (busPaymentReissueDetail lbusPaymentReissueDetail in iclbPaymentReissueDetail)
                    {
                        if (iclbSurvivorNames.Where(item => item.person_id == lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId && item.iintOrganisation > 0).Count() > 0)
                        {
                            lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_org_id =
                                iclbSurvivorNames.Where(item => item.person_id == lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId && item.iintOrganisation > 0).FirstOrDefault().iintOrganisation;
                            lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_person_id = 0;
                        }
                        else
                        {
                            lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_org_id = 0;
                        }
                    }
                }



                foreach (busPaymentReissueDetail lbusPaymentReissueDetail in this.iclbPaymentReissueDetail)
                {
                    
                    lbusPaymentReissueDetail.icdoPaymentReissueDetail.payment_history_distribution_id = this.icdoPaymentHistoryDistribution.payment_history_distribution_id;
                   
                    #region REISSUE TO PAYEE
                    if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_PAYEE)
                    {
                        lintPayeeAccountPaymentTypeId = this.ibusPaymentHistoryHeader.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM54,
                            this.icdoPaymentHistoryDistribution.net_amount, null, 0, ldtNextPaymentDate, DateTime.MinValue, busConstant.FLAG_YES, busConstant.BOOL_TRUE);

                        lbusPaymentReissueDetail.InsertIntoPaymentReissueItemDetail(lbusPaymentReissueDetail.icdoPaymentReissueDetail.payment_reissue_detail_id,
                            lintPayeeAccountPaymentTypeId);

                        break;
                    }
                    #endregion REISSUE TO PAYEE

                    #region TRANSGER ORGANIZATION
                    else if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_TRANSFER_ORGANIZATION)
                    {
                        if(lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRMPID != "")
                        {
                            DataTable ldtlOrDetails = Select("cdoOrganization.GetOrgDetails", new object[1] { lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRMPID });

                            lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_org_id = Convert.ToInt32(ldtlOrDetails.Rows[0]["ORG_ID"]);

                        }
                        
                        lintPayeeAccountPaymentTypeId = this.ibusPaymentHistoryHeader.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM54,
                             this.icdoPaymentHistoryDistribution.net_amount, null, 0, ldtNextPaymentDate, DateTime.MinValue, busConstant.FLAG_YES, busConstant.BOOL_TRUE);

                        lbusPaymentReissueDetail.InsertIntoPaymentReissueItemDetail(lbusPaymentReissueDetail.icdoPaymentReissueDetail.payment_reissue_detail_id,
                            lintPayeeAccountPaymentTypeId);

                        break;
                    }
                    #endregion REISSUE TRANSGER ORGANIZATION

                    #region REISSUE PAYEE TO ROLLOVER ORGANIZATION
                    else if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_ROLLOVER_ORGANIZATION)
                    {
                        this.ibusPaymentHistoryHeader.ibusPayeeAccount.ProcessRolloverDetails(this.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date, lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_rollover_org_id);
                        this.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadPayeeAccountRolloverDetails();

                        if (this.ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPayeeAccountRolloverDetail.
                            Where(item => item.icdoPayeeAccountRolloverDetail.rollover_org_id == lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_rollover_org_id).Count() > 0)
                        {

                            foreach (busPayeeAccountRolloverDetail lbusPayeeAccountRolloverDetail in this.ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPayeeAccountRolloverDetail)
                            {
                                if (lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_org_id == lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_rollover_org_id)
                                {
                                    lbusPayeeAccountRolloverDetail.LoadPayeeAccountRolloverItemDetails();

                                    if (lbusPayeeAccountRolloverDetail.iclbPayeeAccountRolloverItemDetail.Where(item => item.icdoPayeeAccountRolloverItemDetail.payee_account_rollover_detail_id ==
                                        lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id).Count() > 0)
                                    {

                                        foreach (busPayeeAccountRolloverItemDetail lbusPayeeAccountRolloverItemDetail in lbusPayeeAccountRolloverDetail.iclbPayeeAccountRolloverItemDetail)
                                        {
                                            if (lbusPayeeAccountRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_rollover_detail_id == lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id)
                                            {
                                                lintPayeeAccountPaymentTypeId = lbusPayeeAccountRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_payment_item_type_id;

                                                lbusPaymentReissueDetail.InsertIntoPaymentReissueItemDetail(lbusPaymentReissueDetail.icdoPaymentReissueDetail.payment_reissue_detail_id,
                                                                    lintPayeeAccountPaymentTypeId);

                                                busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                                                if (lbusPayeeAccountPaymentItemType.FindPayeeAccountPaymentItemType(lintPayeeAccountPaymentTypeId))
                                                {
                                                    lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date = ldtNextPaymentDate;
                                                    lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }

                        #region Negate Taxes From Provider Report Payment
                        Collection<cdoProviderReportPayment> lclbProviderReportPayment = new Collection<cdoProviderReportPayment>();
                        DataTable ldtlGetTaxes = Select("cdoPaymentHistoryHeader.GetTaxes", new object[1] { this.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id });
                        if (ldtlGetTaxes.Rows.Count > 0)
                        {
                            lclbProviderReportPayment = cdoProviderReportPayment.GetCollection<cdoProviderReportPayment>(ldtlGetTaxes);


                            foreach (cdoProviderReportPayment lcdoProviderReportPayment in lclbProviderReportPayment)
                            {
                                this.ibusPaymentHistoryHeader.ibusPayeeAccount.InsertIntoProviderReportPayment(busConstant.SUSSYSTEM_TYPE_PAYMENT_CANCEL, lcdoProviderReportPayment.subsystem_ref_id,
                                    lcdoProviderReportPayment.person_id, lcdoProviderReportPayment.provider_org_id, lcdoProviderReportPayment.payee_account_id,
                                    -(lcdoProviderReportPayment.amount), lcdoProviderReportPayment.payment_history_header_id, lcdoProviderReportPayment.payment_item_type_id);
                            }
                        }

                        #endregion



                        break;
                    }

                    #endregion REISSUE PAYEE TO ROLLOVER ORGANIZATION

                    #region REISSUE PAYEE TO SURVIVOR
                    else if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_SURVIVOR)
                    {
                        lbusPaymentReissueDetail.LoadPaymentReissueItemDetail();
                        if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.payment_reissue_detail_id == 0)
                        {
                            DataTable ldtGetSurvivorPercentage = Select("cdoPaymentReissueDetail.GetSurvivorPercentage", new object[2]{this.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id,
                            lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId});

                            if (ldtGetSurvivorPercentage.Rows.Count > 0)
                            {
                                //Need To Change
                                decimal ldecPercentage = Convert.ToDecimal(ldtGetSurvivorPercentage.Rows[0][0]);
                                lintPayeeAccountPaymentTypeId = this.ibusPaymentHistoryHeader.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.ITEM54,
                                    (this.icdoPaymentHistoryDistribution.net_amount / 100) * ldecPercentage, null, 0, ldtNextPaymentDate, DateTime.MinValue, busConstant.FLAG_YES,busConstant.BOOL_TRUE);

                                lbusPaymentReissueDetail.InsertIntoPaymentReissueItemDetail(lbusPaymentReissueDetail.icdoPaymentReissueDetail.payment_reissue_detail_id,
                                                       lintPayeeAccountPaymentTypeId);
                            }
                        }
                    }
                    #endregion REISSUE PAYEE TO SURVIVOR
                    


                    #region ROLLOVER ORGANIZATION
                    else if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION)
                    {
                        if (this.ibusPaymentHistoryHeader.iclbPaymentHistoryDetail.Count > 0)
                        {
                            foreach (busPaymentHistoryDetail lbusPaymentHistoryDetail in this.ibusPaymentHistoryHeader.iclbPaymentHistoryDetail)
                            {
                                string lstrAccountNumber = string.Empty;
                                int lintPayeeAccountRolloverItemId = 0;

                                string lPaymentItemItemCode = this.ibusPaymentHistoryHeader.ibusPayeeAccount.GetPaymentItemItemCodeByPaymentItemTypeID(lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.payment_item_type_id);

                                string lstrItemTypeCode = this.ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPaymentItemType.Where(item => item.icdoPaymentItemType.rollover_item_code
                                   == lPaymentItemItemCode).First().icdoPaymentItemType.item_type_code;

                                DataTable ldtblAccountNumber = Select("cdoPaymentReissueDetail.GetAccountNumber", new object[2]{this.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                    lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_rollover_org_id });

                                if (ldtblAccountNumber != null && ldtblAccountNumber.Rows.Count > 0 && Convert.ToString(ldtblAccountNumber.Rows[0][enmPayeeAccountRolloverDetail.account_number.ToString().ToUpper()]).IsNotNullOrEmpty())
                                {
                                    lstrAccountNumber = Convert.ToString(ldtblAccountNumber.Rows[0][enmPayeeAccountRolloverDetail.account_number.ToString().ToUpper()]);
                                }

                                if (ldtblAccountNumber != null && ldtblAccountNumber.Rows.Count > 0 && Convert.ToString(ldtblAccountNumber.Rows[0][enmPayeeAccountRolloverDetail.payee_account_rollover_detail_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                                {
                                    lintPayeeAccountRolloverItemId = Convert.ToInt32(ldtblAccountNumber.Rows[0][enmPayeeAccountRolloverDetail.payee_account_rollover_detail_id.ToString().ToUpper()]);
                                }

                                
                                lintPayeeAccountPaymentTypeId = this.ibusPaymentHistoryHeader.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lstrItemTypeCode,
                                    lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.amount, lstrAccountNumber, 0, ldtNextPaymentDate, DateTime.MinValue, busConstant.FLAG_YES, busConstant.BOOL_TRUE); //Rollover Reissue fix force to insert flag yes

                                lbusPaymentReissueDetail.InsertIntoPaymentReissueItemDetail(lbusPaymentReissueDetail.icdoPaymentReissueDetail.payment_reissue_detail_id,
                                                  lintPayeeAccountPaymentTypeId);

                                this.ibusPaymentHistoryHeader.ibusPayeeAccount.InsertIntoPayeeAccountRolloverItemDetail(lintPayeeAccountRolloverItemId, lintPayeeAccountPaymentTypeId);
                            }
                            break;
                        }

                    }
                    #endregion ROLLOVER ORGANIZATION

                    #region REISSUE ROLLOVER ORGANIZATION TO  PAYEE
                    else if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION_TO_PAYEE)
                    {
                        if (this.ibusPaymentHistoryHeader.iclbPaymentHistoryDetail.Count > 0)
                        {
                            this.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadPaymentItemType();
                            foreach (busPaymentHistoryDetail lbusPaymentHistoryDetail in this.ibusPaymentHistoryHeader.iclbPaymentHistoryDetail)
                            {
                                string lstrItemTypeCode = this.ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPaymentItemType.Where(item => item.icdoPaymentItemType.payment_item_type_id
                                    == lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.payment_item_type_id).First().icdoPaymentItemType.item_type_code;

                                string PayeeItemTypeCode = this.ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.rollover_item_code
                                    == (this.ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPaymentItemType.Where(item => item.icdoPaymentItemType.rollover_item_code
                                    == lstrItemTypeCode).First().icdoPaymentItemType.item_type_code)).First().icdoPaymentItemType.item_type_code;

                                lintPayeeAccountPaymentTypeId = this.ibusPaymentHistoryHeader.ibusPayeeAccount.CreatePayeeAccountPaymentItemType(PayeeItemTypeCode,
                                        lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.amount, null, 0, ldtNextPaymentDate, DateTime.MinValue, busConstant.FLAG_YES, busConstant.BOOL_FALSE);

                                lbusPaymentReissueDetail.InsertIntoPaymentReissueItemDetail(lbusPaymentReissueDetail.icdoPaymentReissueDetail.payment_reissue_detail_id,
                                                  lintPayeeAccountPaymentTypeId);

                            }

                           // this.ibusPaymentHistoryHeader.ibusPayeeAccount.ProcessTaxWithHoldingDetails(adtPaymentForReissue: ldtNextPaymentDate);
                            break;

                        }
                    }
                    #endregion REISSUE ROLLOVER ORGANIZATION TO  PAYEE

                }
            }
        }

        public void InsertPaymentDistributionStatusHistory(int aintPaymentDistributionId,int aintPaymentHistoryHeaderId,string astrDistributionStatusValue,DateTime adtTransactionDate)
        {
            LoadPaymentHistoryDistributionStatusHistorys();

            if (iclbPaymentHistoryDistributionStatusHistory != null && iclbPaymentHistoryDistributionStatusHistory.Count > 0)
            {
                if (iclbPaymentHistoryDistributionStatusHistory.LastOrDefault().icdoPaymentHistoryDistributionStatusHistory.distribution_status_value == astrDistributionStatusValue)
                {
                    busPaymentHistoryDistributionStatusHistory lbusPaymentHistoryDistributionStatusHistory  = new busPaymentHistoryDistributionStatusHistory { icdoPaymentHistoryDistributionStatusHistory = new cdoPaymentHistoryDistributionStatusHistory() };
                    lbusPaymentHistoryDistributionStatusHistory = iclbPaymentHistoryDistributionStatusHistory.LastOrDefault();

                    lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.payment_history_distribution_id = aintPaymentDistributionId;
                    lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.payment_history_header_id = aintPaymentHistoryHeaderId;
                    lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_id = busConstant.PAYMENT_DISTRIBUTION_STATUS_ID;
                    lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_value = astrDistributionStatusValue;
                    lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.transaction_date = adtTransactionDate;
                    lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.status_changed_by = utlPassInfo.iobjPassInfo.istrUserID;
                    lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.Update();


                }
                else
                {
                    cdoPaymentHistoryDistributionStatusHistory lcdoPaymentHistoryDistributionStatusHistory = new cdoPaymentHistoryDistributionStatusHistory();

                    lcdoPaymentHistoryDistributionStatusHistory.payment_history_distribution_id = aintPaymentDistributionId;
                    lcdoPaymentHistoryDistributionStatusHistory.payment_history_header_id = aintPaymentHistoryHeaderId;
                    lcdoPaymentHistoryDistributionStatusHistory.distribution_status_id = busConstant.PAYMENT_DISTRIBUTION_STATUS_ID;
                    lcdoPaymentHistoryDistributionStatusHistory.distribution_status_value = astrDistributionStatusValue;
                    lcdoPaymentHistoryDistributionStatusHistory.transaction_date = adtTransactionDate;
                    lcdoPaymentHistoryDistributionStatusHistory.status_changed_by = utlPassInfo.iobjPassInfo.istrUserID;
                    lcdoPaymentHistoryDistributionStatusHistory.Insert();
                }
            }
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            utlError lobjError = new utlError();
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();
            //   busPaymentReissueDetail lbusPaymentReissueDetail = new busPaymentReissueDetail();
            base.ValidateHardErrors(aenmPageMode);

            Hashtable lhstParams = new Hashtable();
          
            foreach (busPaymentReissueDetail lbusPaymentReissueDetail in this.iclbPaymentReissueDetail)
            {
                lhstParams.Clear();

                lhstParams["icdoPaymentReissueDetail.reissue_payment_type_value"] = lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value;
                lhstParams["icdoPaymentReissueDetail.istrRMPID"] = lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRMPID;
                lhstParams["icdoPaymentReissueDetail.recipient_person_id"] = lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_person_id;
                lhstParams["icdoPaymentReissueDetail.reissue_reason_value"] = lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_reason_value;
                lhstParams["icdoPaymentReissueDetail.istrRecipientRollOverOrgMPID"] = lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRecipientRollOverOrgMPID;
                lhstParams["icdoPaymentReissueDetail.recipient_rollover_org_id"] = lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_rollover_org_id;
                lhstParams["icdoPaymentReissueDetail.iintSurvivorId"] = lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId;
 
                lbusPaymentReissueDetail.CheckErrorOnAddButton(this, lhstParams, ref this.iarrErrors, true);
            }
        }

        
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            DateTime ldtNextPaymentDate = new DateTime();

            #region FOR IAP PUT PAYEE ACCOUNT IN REVIEWED STATUS
            if (this.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.plan_id == busConstant.IAP_PLAN_ID)
            {
                this.ibusPaymentHistoryHeader.ibusPayeeAccount.CreateReviewPayeeAccountStatus();
            }
            #endregion FOR IAP PUT PAYEE ACCOUNT IN REVIEWED STATUS

            ldtNextPaymentDate = this.ibusPaymentHistoryHeader.ibusPayeeAccount.idtNextBenefitPaymentDate;
            if (this.iclbPaymentReissueDetail.Count > 0)
            {
                foreach (busPaymentReissueDetail lbusPaymentReissueDetail in this.iclbPaymentReissueDetail)
                {
                    //if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_ROLLOVER_ORGANIZATION)
                    //{

                    //    #region Negate Taxes (INERT INTO PROVIDER REPORT PAYMENTS)
                    //    int lintCount = (int)DBFunction.DBExecuteScalar("cdoPaymentReissueDetail.CheckIfNegativeTaxesPosted", new object[1] { icdoPaymentHistoryDistribution.payment_history_header_id },
                    //        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);

                    //    if (lintCount == 0)
                    //    {
                    //        DataTable ldtblProviderReportPayment = Select("cdoPaymentReissueDetail.GetDataForProviderReportPayment", new object[1] { this.icdoPaymentHistoryDistribution.payment_history_header_id });
                    //        if (ldtblProviderReportPayment.Rows.Count > 0)
                    //        {
                    //            foreach (DataRow ldrProviderReportPayment in ldtblProviderReportPayment.Rows)
                    //            {
                    //                cdoProviderReportPayment lcdoProviderReportPayment = new cdoProviderReportPayment();
                    //                lcdoProviderReportPayment.subsystem_id = busConstant.PAYEE_ACCOUNT_SUBSYSTEM_ID;
                    //                lcdoProviderReportPayment.subsystem_value = busConstant.SUBSYSTEM_TYPE_BENEFIT_PAYMENT;
                    //                lcdoProviderReportPayment.subsystem_ref_id = Convert.ToInt32(ldrProviderReportPayment[enmProviderReportPayment.subsystem_ref_id.ToString().ToUpper()]);
                    //                lcdoProviderReportPayment.person_id = Convert.ToInt32(ldrProviderReportPayment[enmProviderReportPayment.person_id.ToString().ToUpper()]);
                    //                lcdoProviderReportPayment.provider_org_id = Convert.ToInt32(ldrProviderReportPayment[enmProviderReportPayment.provider_org_id.ToString().ToUpper()]);
                    //                lcdoProviderReportPayment.payee_account_id = Convert.ToInt32(ldrProviderReportPayment[enmProviderReportPayment.payee_account_id.ToString().ToUpper()]);
                    //                lcdoProviderReportPayment.effective_date = Convert.ToDateTime(ldrProviderReportPayment[enmProviderReportPayment.effective_date.ToString().ToUpper()]);
                    //                lcdoProviderReportPayment.amount = Convert.ToDecimal(ldrProviderReportPayment[enmProviderReportPayment.amount.ToString().ToUpper()]);
                    //                lcdoProviderReportPayment.payment_history_header_id = this.icdoPaymentHistoryDistribution.payment_history_header_id;
                    //                lcdoProviderReportPayment.payment_item_type_id = Convert.ToInt32(ldrProviderReportPayment[enmProviderReportPayment.payment_item_type_id.ToString().ToUpper()]);
                    //                lcdoProviderReportPayment.Insert();
                    //            }
                    //        }
                    //    }

                    //    #endregion Negate Taxes (INERT INTO PROVIDER REPORT PAYMENTS)

                    //}

                    if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION_TO_PAYEE)
                    {
                        this.ibusPaymentHistoryHeader.ibusPayeeAccount.iintPaymentReissueDetailId = lbusPaymentReissueDetail.icdoPaymentReissueDetail.payment_reissue_detail_id;
                        this.ibusPaymentHistoryHeader.ibusPayeeAccount.ProcessTaxWithHoldingDetails(adtPaymentForReissue: ldtNextPaymentDate);                      
                    }


                    this.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_REISSUE;
                    this.icdoPaymentHistoryDistribution.Update();

                    InsertPaymentDistributionStatusHistory(icdoPaymentHistoryDistribution.payment_history_distribution_id, icdoPaymentHistoryDistribution.payment_history_header_id,
                        busConstant.PAYMENT_DISTRIBUTION_STATUS_REISSUE, DateTime.Now);


                    if (lbusPaymentReissueDetail.iclbPaymentReissueItemDetail == null)
                        lbusPaymentReissueDetail.LoadPaymentReissueItemDetail();

                    foreach (busPaymentReissueItemDetail lbusPaymentReissueItemDetail in lbusPaymentReissueDetail.iclbPaymentReissueItemDetail)
                    {
                        lbusPaymentReissueItemDetail.icdoPaymentReissueItemDetail.payment_reissue_detail_id = lbusPaymentReissueDetail.icdoPaymentReissueDetail.payment_reissue_detail_id;
                        lbusPaymentReissueItemDetail.icdoPaymentReissueItemDetail.Update();
                    }
                }
            }
        }

       
        //Load Payment History Details
        public void LoadPaymentHistoryHeader()
        {
            if (ibusPaymentHistoryHeader == null)
                ibusPaymentHistoryHeader = new busPaymentHistoryHeader();
            ibusPaymentHistoryHeader.FindPaymentHistoryHeader(icdoPaymentHistoryDistribution.payment_history_header_id);
        }

        public void LoadPayee()
        {
            LoadPaymentHistoryHeader();
            ibusPaymentHistoryHeader.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            if (ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id != 0)
            {
                ibusPaymentHistoryHeader.ibusPayeeAccount.FindPayeeAccount(ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id);
            }
            
            ibusPaymentHistoryHeader.ibusPayee = new busPerson();
            if (ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id != 0)
            {
                ibusPaymentHistoryHeader.ibusPayee.FindPerson(ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id);
                ibusPaymentHistoryHeader.ibusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                ibusPaymentHistoryHeader.ibusPayeeAccount.ibusParticipant.FindPerson(ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id);
            }
            else
            {
                ibusPaymentHistoryHeader.ibusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                ibusPaymentHistoryHeader.ibusPayeeAccount.ibusParticipant.FindPerson(ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.person_id);
            }

        }

        public void LoadInitialData(string astrBenefitOptionValue)
        {
          
            LoadPaymentReissueDetail();

            DataTable ldtbPayeeMPID = busBase.Select("cdoPerson.GetPersonDetailsByPersonID", new object[1] { this.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.person_id });
            if (ldtbPayeeMPID.Rows.Count > 0)
                this.istrPayeeMPID = Convert.ToString(ldtbPayeeMPID.Rows[0]["MPI_PERSON_ID"]);
           
            if (this.iclbPaymentReissueDetail.Count > 0)
            {
                foreach (busPaymentReissueDetail lbusPaymentReissueDetail in this.iclbPaymentReissueDetail)
                {
                    //lbusPaymentReissueDetail.istrBenefitOptionValue = astrBenefitOptionValue;
                    if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_PAYEE ||
                        lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION_TO_PAYEE)
                    {
                        DataTable ldtbRecipientPersonMPID = busBase.Select("cdoPerson.GetPersonDetailsByPersonID", new object[1] { this.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.person_id });
                        if (ldtbPayeeMPID.Rows.Count > 0)
                            lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRMPID = Convert.ToString(ldtbPayeeMPID.Rows[0]["MPI_PERSON_ID"]);
                    }
                    else
                    {

                        if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_person_id > 0)
                        {
                            //Review
                            DataTable ldtbRecipientPersonMPID = busBase.Select("cdoPerson.GetPersonDetailsByPersonID", new object[1] { lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_person_id });
                            if (ldtbRecipientPersonMPID.Rows.Count > 0)
                            {
                                lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRMPID = Convert.ToString(ldtbRecipientPersonMPID.Rows[0]["MPI_PERSON_ID"]);
                            }
                        }
                        //PIR-622
                        if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_org_id > 0)
                        {
                            DataTable ldtbRecipientPersonMPID = busBase.Select("cdoOrganization.GetOrgDetailsByOrganizationId", new object[1] { lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_org_id });
                            if (ldtbRecipientPersonMPID.Rows.Count > 0)
                            {
                                lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRMPID = Convert.ToString(ldtbRecipientPersonMPID.Rows[0]["MPI_ORG_ID"]);
                            }
                        }

                        else if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_rollover_org_id > 0)
                        {
                            DataTable ldtbRecipientRolloverOrgMPID = busBase.Select("cdoOrganization.GetOrgDetailsByOrganizationId", new object[1] { lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_rollover_org_id });
                            if (ldtbRecipientRolloverOrgMPID.Rows.Count > 0)
                            {
                                lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRecipientRollOverOrgMPID = Convert.ToString(ldtbRecipientRolloverOrgMPID.Rows[0]["MPI_ORG_ID"]);
                            }
                        }
                    }
                }
            }

            iclbSurvivorNames = new Collection<cdoPerson>();

            DataTable ldtbGetSurvivorsForPayee = Select("cdoPaymentReissueDetail.GetSurvivorsForPayee", new object[1] { icdoPaymentHistoryDistribution.payment_history_distribution_id });
            if (ldtbGetSurvivorsForPayee.Rows.Count > 0)
            {
                iclbSurvivorNames = cdoPerson.GetCollection<cdoPerson>(ldtbGetSurvivorsForPayee);

            }
        }



        public Collection<cdoCodeValue> GetReissuePaymentTypes()
        {
            Collection<cdoCodeValue> lclbCodeValue = new Collection<cdoCodeValue>();
            busCodeValue lbusCodeValue = new busCodeValue();
            lclbCodeValue = lbusCodeValue.GetCodeValue(busConstant.REISSUE_PAYMENT_CODE_ID);
            

            if (this.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue != busConstant.LUMP_SUM)
            {
                lclbCodeValue.Remove(lclbCodeValue.Where(t => t.code_value == busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_ROLLOVER_ORGANIZATION).FirstOrDefault());
                lclbCodeValue.Remove(lclbCodeValue.Where(t => t.code_value == busConstant.REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION).FirstOrDefault());
                lclbCodeValue.Remove(lclbCodeValue.Where(t => t.code_value == busConstant.REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION_TO_PAYEE).FirstOrDefault());
            }

            return lclbCodeValue;

        }

        public bool CheckIfBenefitOptionLumpSum()
        {
            if (this.ibusPaymentHistoryHeader != null && this.ibusPaymentHistoryHeader.ibusPayeeAccount.IsNotNull())
            {
                ibusPaymentHistoryHeader.ibusPayeeAccount = new busPayeeAccount();
                ibusPaymentHistoryHeader.ibusPayeeAccount.FindPayeeAccount(ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id);
                ibusPaymentHistoryHeader.ibusPayeeAccount.LoadBenefitDetails();
                ibusPaymentHistoryHeader.ibusPayeeAccount.LoadDRODetails();

                if (this.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.LUMP_SUM)
                {
                    return true;
                }
            }
            
            return false;
        }


        public void LoadPaymentReissueDetail()
        {
            DataTable ldtblPaymentReissueDetail = Select("cdoPaymentReissueDetail.GetPaymentReissueDetail", new object[1] { icdoPaymentHistoryDistribution.payment_history_distribution_id });
            if (ldtblPaymentReissueDetail.Rows.Count > 0)
            {
                iclbPaymentReissueDetail = GetCollection<busPaymentReissueDetail>(ldtblPaymentReissueDetail, "icdoPaymentReissueDetail");
            }
        }

        public Collection<cdoPerson> LoadSurvivorsForPayee()
        {
            return iclbSurvivorNames;
        }

 
        public ArrayList btn_DeletReissueClick(int aintpayment_reissue_detail_id)
        {
            ArrayList arr = new ArrayList();
            busPaymentReissueDetail lbusPaymentReissueDetail = (from obj in iclbPaymentReissueDetail.AsEnumerable()
                                                                where obj.icdoPaymentReissueDetail.payment_reissue_detail_id == aintpayment_reissue_detail_id
                                                                select obj).FirstOrDefault();

            if (lbusPaymentReissueDetail != null)
            {

                lbusPaymentReissueDetail.LoadPaymentReissueItemDetail();


                if (lbusPaymentReissueDetail.iclbPaymentReissueItemDetail != null && lbusPaymentReissueDetail.iclbPaymentReissueItemDetail.Count > 0)
                {
                    foreach (busPaymentReissueItemDetail lbusPaymentReissueItemDetail in lbusPaymentReissueDetail.iclbPaymentReissueItemDetail)
                    {
                        busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                        if (lbusPayeeAccountPaymentItemType.FindPayeeAccountPaymentItemType(lbusPaymentReissueItemDetail.icdoPaymentReissueItemDetail.payee_account_payment_item_type_id))
                        {
                            lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Delete();
                        }

                        lbusPaymentReissueItemDetail.icdoPaymentReissueItemDetail.Delete();
                    }
                }

                lbusPaymentReissueDetail.icdoPaymentReissueDetail.Delete();



                #region REISSUE PAYEE TO ROLLOVER ORGANIZATION
                if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_ROLLOVER_ORGANIZATION)
                {

                    #region Insert Taxes into Provider Report Payment

                    Collection<cdoProviderReportPayment> lclbProviderReportPayment = new Collection<cdoProviderReportPayment>();
                    DataTable ldtlGetTaxes = Select("cdoPaymentHistoryHeader.GetTaxes", new object[1] { this.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id });
                    if (ldtlGetTaxes.Rows.Count > 0)
                    {
                        lclbProviderReportPayment = cdoProviderReportPayment.GetCollection<cdoProviderReportPayment>(ldtlGetTaxes);


                        foreach (cdoProviderReportPayment lcdoProviderReportPayment in lclbProviderReportPayment)
                        {
                            this.ibusPaymentHistoryHeader.ibusPayeeAccount.InsertIntoProviderReportPayment(busConstant.SUSSYSTEM_TYPE_PAYMENT_CANCEL, lcdoProviderReportPayment.subsystem_ref_id,
                                lcdoProviderReportPayment.person_id, lcdoProviderReportPayment.provider_org_id, lcdoProviderReportPayment.payee_account_id,
                                (lcdoProviderReportPayment.amount), lcdoProviderReportPayment.payment_history_header_id, lcdoProviderReportPayment.payment_item_type_id);
                        }
                    }

                    #endregion

                    #region Delete Rollover Item Details

                    if (this.ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPayeeAccountRolloverDetail == null)
                    {
                        this.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadPayeeAccountRolloverDetails();
                    }

                    foreach (busPayeeAccountRolloverDetail lbusPayeeAccountRolloverDetail in this.ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPayeeAccountRolloverDetail)
                    {
                        if (lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_org_id == lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_rollover_org_id)
                        {
                            lbusPayeeAccountRolloverDetail.LoadPayeeAccountRolloverItemDetails();


                            if (lbusPayeeAccountRolloverDetail != null && lbusPayeeAccountRolloverDetail.iclbPayeeAccountRolloverItemDetail != null)
                            {
                                foreach (busPayeeAccountRolloverItemDetail lbusPayeeAccountRolloverItemDetail in lbusPayeeAccountRolloverDetail.iclbPayeeAccountRolloverItemDetail)
                                {
                                    lbusPayeeAccountRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.Delete();
                                }

                                lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.Delete();
                            }
                        }
                    }

                    #endregion
                }

                #endregion REISSUE PAYEE TO ROLLOVER ORGANIZATION
                iclbPaymentReissueDetail.Remove(lbusPaymentReissueDetail);
            }

            arr.Add(this);
            return arr;
        }

        public override busBase GetCorPerson()
        {
                ibusPaymentHistoryHeader.ibusPayee.LoadPersonAddresss();
                ibusPaymentHistoryHeader.ibusPayee.LoadPersonContacts();
                ibusPaymentHistoryHeader.ibusPayee.LoadCorrAddress();
                return this.ibusPaymentHistoryHeader.ibusPayee;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {

            base.LoadCorresProperties(astrTemplateName);
            DateTime ldtCurrentDate = System.DateTime.Now;
            this.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);

            #region Payee-0018
            if (astrTemplateName == busConstant.RE_EMPLOYMENT_OVERPAYMENT_NOTICE_RE_PAYMENT_LETTER)
            {
                istrCheckACHDate = busGlobalFunctions.ConvertDateIntoDifFormat(this.icdoPaymentHistoryDistribution.check_ach_effective_date);
            }
            #endregion
        }

        public string LoadOrgMpiID(int MPI_ORG_ID)
        {
            DataTable ldtbOrgnizationId = busBase.Select("entPaymentHistoryDistribution.GetOrgMpId", new object[1] { MPI_ORG_ID });
            if (ldtbOrgnizationId.Rows.Count > 0)
            {
                istrOrgID = Convert.ToString(ldtbOrgnizationId.Rows[0][0]);

               
            }
            return istrOrgID;
        }
	}
}
