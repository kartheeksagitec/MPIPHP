using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;
using MPIPHP.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using System.Linq;
using MPIPHPJobService;
using Sagitec.CustomDataObjects;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.Common;
using System.Threading.Tasks;
using Sagitec.ExceptionPub;

namespace MPIPHPJobService
{
    class busOnetimePaymentBatch : busBatchHandler
    {

        #region Properties

        private object iobjLock = null;

        #endregion
        //private void RetrieveOnetimeBatchParameters()
        //{
        //    if (ibusJobHeader != null)
        //    {
        //        if (ibusJobHeader.iclbJobDetail == null)
        //            ibusJobHeader.LoadJobDetail(true);

        //        foreach (busJobDetail lobjDetail in ibusJobHeader.iclbJobDetail)
        //        {
        //            foreach (busJobParameters lobjParam in lobjDetail.iclbJobParameters)
        //            {

        //                        break;

        //            }
        //        }
        //    }
        //}

        #region Public Methods

        public void OnetimePaymentBatch()
        {
            int lintCount = 0;
            int lintTotalCount = 0;

            utlConnection utlLegacyDBConnection = HelperFunction.GetDBConnectionProperties("core");
            string astrLegacyDBConnection = utlLegacyDBConnection.istrConnectionString;

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();

            DataTable ldtbOntimePaymentContractInfo = busBase.Select("cdoPayeeAccount.GetEffectiveStartDateForOneTimePayment",
                                            new object[0] { });
            DateTime ldtEffectiveStartDate = DateTime.MinValue;

            if (ldtbOntimePaymentContractInfo.Rows.Count > 0 && Convert.ToString(ldtbOntimePaymentContractInfo.Rows[0][0]).IsNotNullOrEmpty())
            {
                ldtEffectiveStartDate = Convert.ToDateTime(ldtbOntimePaymentContractInfo.Rows[0][0].ToString());
            }

            //RetrieveOnetimeBatchParameters();

            DataTable ldtbPayeeAccountInfo = busBase.Select("cdoPayeeAccount.LoadPayeeAccountForOnetimePayment", new object[0] { });

            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

            Parallel.ForEach(ldtbPayeeAccountInfo.AsEnumerable(), po, (acdoPerson, loopState) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "OnetimePaymentBatch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;

                if (Convert.ToString(acdoPerson["PERCENT_INCREASE"]).IsNotNullOrEmpty())
                {
                    CalculateAndCreateOnetimePayeeAccounts(acdoPerson, ldtEffectiveStartDate, lobjPassInfo, lintCount, lintTotalCount, ldtbPayeeAccountInfo);
                }

                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;
            });


            #region Update tables with end date
            utlPassInfo lobjPassInfo1 = new utlPassInfo();
            lobjPassInfo1.idictParams = ldictParams;
            lobjPassInfo1.idictParams["ID"] = "YearEndDataExtractionBatch";
            lobjPassInfo1.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjPassInfo1;

            if (lobjPassInfo1.iconFramework.State == ConnectionState.Closed)
                lobjPassInfo1.iconFramework.Open();
            DBFunction.DBNonQuery("cdoPayeeAccount.UpdateOnetimePaymentContractEndDate",
                          new object[1] { ldtEffectiveStartDate }, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);

            //lobjPassInfo1.BeginTransaction();
            //if (lbusActiveRetireeIncreaseContract != null)
            //{
            //    DateTime ldtEndDate = new DateTime(lintPlanYear, 12, 31);

            //    lbusActiveRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.effective_end_date = ldtEndDate;
            //    lbusActiveRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.Update();
            //    lobjPassInfo1.Commit();
            //}

            if (lobjPassInfo1.iconFramework.State == ConnectionState.Open)
            {
                lobjPassInfo1.iconFramework.Close();
            }
            lobjPassInfo1.iconFramework.Dispose();
            lobjPassInfo1.iconFramework = null;
            #endregion

            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        }

        #endregion

        #region Private Methods

        private void CalculateAndCreateOnetimePayeeAccounts(DataRow acdoPayeeAccount, DateTime adtEffectiveStartDate, utlPassInfo autlPassInfo,
                                                            int aintCount, int aintTotalCount, DataTable adtPersonInfo)
        {
            ArrayList aarrResult = new ArrayList();
            Hashtable ahtbQueryBkmarks = new Hashtable();
            busBase lbusBase = new busBase();
            busCalculation lbusCalculation = new busCalculation();

            lock (iobjLock)
            {
                aintCount++;
                aintTotalCount++;
                if (aintCount == 100)
                {
                    string lstrMsg = aintTotalCount + " : " + " Records Has Been Processed";
                    PostInfoMessage(lstrMsg);
                    aintCount = 0;
                }
            }

            autlPassInfo.BeginTransaction();
            try
            {
                int lintPlanId = Convert.ToInt32(acdoPayeeAccount[enmPlanBenefitXr.plan_id.ToString()]);
                int lintPayementCount = 0, lintNonSuspendibleMonth = 0;
                decimal ldecGrossAmount = 0;

                busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                DateTime ldteNextBenefitPaymentDate = adtEffectiveStartDate;  //busPayeeAccountHelper.GetLastBenefitPaymentDate(lintPlanId).AddMonths(1);


                LoadDataForOnetimePayeeAccount(lbusPerson, lbusPayeeAccount, acdoPayeeAccount, adtEffectiveStartDate,
                                                                        ref lintPayementCount, ref lintNonSuspendibleMonth, ref ldecGrossAmount);

                if ( //(lbusPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form == busConstant.FLAG_NO || lbusPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form.IsNullOrEmpty()) &&
                    (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING ||
                     acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED  ||
                     acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED ||
                     acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_REVIEW ) 
                    //&& ((lintNonSuspendibleMonth >= 1 || lbusPayeeAccount.icdoPayeeAccount.istrMDAge == busConstant.FLAG_YES) && lbusPerson.icdoPerson.date_of_death == DateTime.MinValue) 
                    && ldecGrossAmount != 0) 
                {
                    decimal ldecGuaranteedAmt = 0;
                    DataTable ldtbParticipantPayeeAccountDetails = new DataTable();

                    CreateOnetimePayeeAccounts(acdoPayeeAccount, lintPlanId, lbusPayeeAccount, ldteNextBenefitPaymentDate, ldecGrossAmount, 0, 0, 0);

                    if (acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        ldecGuaranteedAmt = Convert.ToDecimal(acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()]);
                        CreateOnetimePayeeAccounts(acdoPayeeAccount, lintPlanId, lbusPayeeAccount, ldteNextBenefitPaymentDate, 0, ldecGuaranteedAmt, 0, 0);
                    }
                    /*
                    if (Convert.ToString(acdoPayeeAccount[enmPayeeAccount.benefit_account_type_value.ToString()]) != busConstant.BENEFIT_TYPE_QDRO 
                                    && adtPersonInfo.AsEnumerable().Where(
                                     item => item.Field<int>(enmPayeeAccount.payee_benefit_account_id.ToString()) == lbusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id &&
                                     item.Field<string>(enmPayeeAccount.benefit_account_type_value.ToString()) == busConstant.BENEFIT_TYPE_QDRO 
                                     //&& item.Field<string>(enmDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase.ToString()) == busConstant.FLAG_YES
                                     ).Count() == 0)
                    {
                        CreateOnetimePayeeAccounts(acdoPayeeAccount, lintPlanId, lbusPayeeAccount, ldteNextBenefitPaymentDate, ldecGrossAmount, 0, 0, 0);

                        if (acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()].ToString().IsNotNullOrEmpty())
                        {
                            ldecGuaranteedAmt = Convert.ToDecimal(acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()]);
                            CreateOnetimePayeeAccounts(acdoPayeeAccount, lintPlanId, lbusPayeeAccount, ldteNextBenefitPaymentDate, 0, ldecGuaranteedAmt, 0, 0);
                        }
                    }
                    else if (Convert.ToString(acdoPayeeAccount[enmPayeeAccount.benefit_account_type_value.ToString()]) == busConstant.BENEFIT_TYPE_QDRO)
                    {
                        CreateOnetimePayeeAccounts(acdoPayeeAccount, lintPlanId, lbusPayeeAccount, ldteNextBenefitPaymentDate, ldecGrossAmount, 0, 0, 0);

                        if (acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()].ToString().IsNotNullOrEmpty())
                        {
                            ldecGuaranteedAmt = Convert.ToDecimal(acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()]);
                            CreateOnetimePayeeAccounts(acdoPayeeAccount, lintPlanId, lbusPayeeAccount, ldteNextBenefitPaymentDate, 0, ldecGuaranteedAmt, 0, 0);
                        }

                        //if (Convert.ToString(acdoPayeeAccount[enmDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase.ToString()]) == busConstant.FLAG_YES)
                        //{
                        if (adtPersonInfo.AsEnumerable().Where(
                                        item => item.Field<int>(enmPayeeAccount.payee_benefit_account_id.ToString()) == lbusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id &&
                                        item.Field<string>(enmPayeeAccount.benefit_account_type_value.ToString()) != busConstant.BENEFIT_TYPE_QDRO).Count() > 0)
                        {
                            ldtbParticipantPayeeAccountDetails = adtPersonInfo.AsEnumerable().Where(
                                            item => item.Field<int>(enmPayeeAccount.payee_benefit_account_id.ToString()) == lbusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id &&
                                            item.Field<string>(enmPayeeAccount.benefit_account_type_value.ToString()) != busConstant.BENEFIT_TYPE_QDRO).CopyToDataTable();
                        }

                        if (ldtbParticipantPayeeAccountDetails.Rows.Count > 0)
                        {
                            int lintParticipantPaymentCount = 0, lintParticipantNonSuspendibleMonth = 0;
                            decimal ldecParticipantGrossAmount = 0;

                            busPerson lbusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                            busPayeeAccount lbusParticipantPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            LoadDataForOnetimePayeeAccount(lbusParticipant, lbusParticipantPayeeAccount, ldtbParticipantPayeeAccountDetails.Rows[0], adtEffectiveStartDate,
                                                                    ref lintParticipantPaymentCount, ref lintParticipantNonSuspendibleMonth, ref ldecParticipantGrossAmount);

                            if ( //(lbusParticipantPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form == busConstant.FLAG_NO || lbusParticipantPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form.IsNullOrEmpty()) &&
                                    (ldtbParticipantPayeeAccountDetails.Rows[0]["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING ||
                                    (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED && lintParticipantPaymentCount >= 1) ||
                                    (ldtbParticipantPayeeAccountDetails.Rows[0]["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED && lintParticipantPaymentCount >= 1) ||
                                    (ldtbParticipantPayeeAccountDetails.Rows[0]["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_REVIEW && lintParticipantPaymentCount >= 1)) 
                                    //((lintParticipantNonSuspendibleMonth >= 1 || lbusParticipantPayeeAccount.icdoPayeeAccount.istrMDAge == busConstant.FLAG_YES) && lbusParticipant.icdoPerson.date_of_death == DateTime.MinValue) 
                                    && ldecParticipantGrossAmount != 0)  //RID 53296
                            {
                                CreateOnetimePayeeAccounts(ldtbParticipantPayeeAccountDetails.Rows[0], lintPlanId, lbusParticipantPayeeAccount, ldteNextBenefitPaymentDate, ldecParticipantGrossAmount,
                                                                    0, lbusPayeeAccount.icdoPayeeAccount.person_id, lbusPayeeAccount.icdoPayeeAccount.payee_account_id);

                                if (ldtbParticipantPayeeAccountDetails.Rows[0][enmLocal700GuaranteedAmount.guaranteed_amount.ToString()].ToString().IsNotNullOrEmpty())
                                {
                                    decimal ldecParticipantGuaranteedAmt = Convert.ToDecimal(ldtbParticipantPayeeAccountDetails.Rows[0][enmLocal700GuaranteedAmount.guaranteed_amount.ToString()]);
                                    CreateOnetimePayeeAccounts(ldtbParticipantPayeeAccountDetails.Rows[0], lintPlanId, lbusParticipantPayeeAccount,
                                                                        ldteNextBenefitPaymentDate, 0, ldecParticipantGuaranteedAmt, lbusPayeeAccount.icdoPayeeAccount.person_id, lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
                                }
                            }
                        }
                        //}
                    }
                    */
                }
                //else if (lbusPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form == busConstant.FLAG_YES)
                //{
                //    #region Create Payee Account in Review status

                //    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();

                //    lbusPayeeAccount.icdoPayeeAccount.plan_benefit_id = lbusPlanBenefitXr.GetPlanBenefitId(lintPlanId, busConstant.LUMP_SUM);
                //    lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date = ldteNextBenefitPaymentDate;
                //    lbusPayeeAccount.icdoPayeeAccount.reference_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                //    lbusPayeeAccount.icdoPayeeAccount.Insert();

                //    lbusPayeeAccount.CreateReviewPayeeAccountStatus();

                //    #endregion
                //}

                lbusPayeeAccount.istrBenefitBeginDate = busGlobalFunctions.ConvertDateIntoDifFormat(lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date);
                lbusPayeeAccount.istrMonthYear = lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date.Year.ToString();
                lbusPayeeAccount.iintPlanYear = adtEffectiveStartDate.Year;

                aarrResult.Add(lbusPayeeAccount);

                autlPassInfo.Commit();

            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    string lstrMsg = "Error while Executing Batch, Error Message: "+ acdoPayeeAccount["MPI_PERSON_ID"] + " PA = " + acdoPayeeAccount["PAYEE_ACCOUNT_ID"] + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();
            }

        }

        private void CreateOnetimePayeeAccounts(DataRow acdoPayeeAccount, int aintPlanId, busPayeeAccount abusPayeeAccount,
                                                        DateTime adteNextBenefitPaymentDate, decimal adecGrossAmount, decimal adecGuaranteedAmt,
                                                        int aintQDROPersonId, int aintQDROPayeeAccountId)
        {
            #region Create Payee Account

            DateTime dtRetireeInc = new DateTime();
            dtRetireeInc = adteNextBenefitPaymentDate;

            busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };

            if (abusPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance == 0)
                abusPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance = abusPayeeAccount.icdoPayeeAccount.remaining_non_taxable_from_conversion;

            if (aintQDROPersonId != 0 && aintQDROPayeeAccountId != 0)
            {
                lbusPayeeAccount.ManagePayeeAccount(0, aintQDROPersonId, abusPayeeAccount.icdoPayeeAccount.org_id,
                                                    abusPayeeAccount.icdoPayeeAccount.benefit_application_detail_id, abusPayeeAccount.icdoPayeeAccount.benefit_calculation_detail_id,
                                                    abusPayeeAccount.icdoPayeeAccount.dro_application_detail_id, abusPayeeAccount.icdoPayeeAccount.dro_calculation_detail_id,
                                                    abusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id, busConstant.BENEFIT_TYPE_QDRO,
                                                    abusPayeeAccount.icdoPayeeAccount.retirement_type_value, dtRetireeInc, abusPayeeAccount.icdoPayeeAccount.benefit_end_date,
                                                    abusPayeeAccount.icdoPayeeAccount.account_relation_value, abusPayeeAccount.icdoPayeeAccount.family_relation_value,
                                                    0, 0,
                                                    lbusPlanBenefitXr.GetPlanBenefitId(aintPlanId, busConstant.LUMP_SUM), DateTime.MinValue,
                                                    busConstant.FLAG_NO, busConstant.FLAG_NO, false, aintQDROPayeeAccountId, busConstant.FLAG_YES);
            }
            else
            {
                lbusPayeeAccount.ManagePayeeAccount(0, abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.org_id,
                                                    abusPayeeAccount.icdoPayeeAccount.benefit_application_detail_id, abusPayeeAccount.icdoPayeeAccount.benefit_calculation_detail_id,
                                                    abusPayeeAccount.icdoPayeeAccount.dro_application_detail_id, abusPayeeAccount.icdoPayeeAccount.dro_calculation_detail_id,
                                                    abusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id, abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
                                                    abusPayeeAccount.icdoPayeeAccount.retirement_type_value, dtRetireeInc, abusPayeeAccount.icdoPayeeAccount.benefit_end_date,
                                                    abusPayeeAccount.icdoPayeeAccount.account_relation_value, abusPayeeAccount.icdoPayeeAccount.family_relation_value,
                                                    0, 0,
                                                    lbusPlanBenefitXr.GetPlanBenefitId(aintPlanId, busConstant.LUMP_SUM), DateTime.MinValue,
                                                    busConstant.FLAG_NO, busConstant.FLAG_NO, false, abusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.FLAG_YES);
            }


            lbusPayeeAccount.icdoPayeeAccount.reference_id = abusPayeeAccount.icdoPayeeAccount.payee_account_id;

            busPayeeAccountStatus lbusPayeeAccountStatus = new busPayeeAccountStatus();


            if (Convert.ToString(acdoPayeeAccount["Status"]).IsNotNullOrEmpty() && (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING ||
                                                acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED))
            {
                lbusPayeeAccountStatus.InsertValuesInPayeeAccountStatus(lbusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                                    busConstant.PAYEE_ACCOUNT_STATUS_APPROVED, DateTime.Now);
            }
            else if (Convert.ToString(acdoPayeeAccount["Status"]).IsNotNullOrEmpty() && Convert.ToString(acdoPayeeAccount["Status"]) == busConstant.PAYEE_ACCOUNT_STATUS_REVIEW)
            {
                lbusPayeeAccountStatus.InsertValuesInPayeeAccountStatus(lbusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                                    busConstant.PAYEE_ACCOUNT_STATUS_REVIEW, DateTime.Now);
            }
            else if (Convert.ToString(acdoPayeeAccount["Status"]).IsNotNullOrEmpty() && Convert.ToString(acdoPayeeAccount["Status"]) == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED)
            {
                lbusPayeeAccountStatus.InsertValuesInPayeeAccountStatus(lbusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                                    busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED, DateTime.Now);
            }

            #endregion

            #region Calculate Retiree Increase Amount and insert data in payment item type table

            decimal ldecRetireeIncAmt = (adecGrossAmount + adecGuaranteedAmt) *
                                       (Convert.ToDecimal(acdoPayeeAccount["PERCENT_INCREASE"]) / 100);


            busPayeeAccountPaymentItemType lbusPayeeAccountPayementItemType = lbusPayeeAccount.CreatePayeeAccountPaymentItemType(
                48, ldecRetireeIncAmt, "0", 0, DateTime.Now, DateTime.MinValue, "N");
            lbusPayeeAccountPayementItemType.icdoPayeeAccountPaymentItemType.Insert();

            #endregion


            #region Setup Tax withholding, same as original payee account

            if (abusPayeeAccount.ibusParticipant == null)
            {
                if (lbusPayeeAccount.ibusPayeeBenefitAccount == null)
                {
                    lbusPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lbusPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lbusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                }

                abusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                abusPayeeAccount.ibusParticipant.FindPerson(abusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);

            }

            //Load existing fedral tax data
            busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholdingFed = new busPayeeAccountTaxWithholding();
            lbusPayeeAccountTaxWithholdingFed =
                lbusPayeeAccountTaxWithholdingFed.LoadTaxWithHoldingByPayeeAccountIdAndTaxType(lbusPayeeAccount.icdoPayeeAccount.reference_id, busConstant.FEDRAL_STATE_TAX);

            if (lbusPayeeAccountTaxWithholdingFed != null)
            {
                lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                if (lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_DOLLAR)
                {
                    lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.additional_tax_amount =
                        lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.additional_tax_amount *
                    (Convert.ToDecimal(acdoPayeeAccount["PERCENT_INCREASE"]) / 100);
                }
                lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.start_date = DateTime.Now;
                lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.end_date = DateTime.MinValue;
                lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.Insert();
            }

            //Load existing State tax data
            // CA_STATE_TAX, GA_STATE_TAX, NC_STATE_TAX, OR_STATE_TAX, VA_STATE_TAX 
            bool bFindStateTax = true;
            busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholdingState = new busPayeeAccountTaxWithholding();
            lbusPayeeAccountTaxWithholdingState =
            lbusPayeeAccountTaxWithholdingState.LoadTaxWithHoldingByPayeeAccountIdAndTaxType(lbusPayeeAccount.icdoPayeeAccount.reference_id, busConstant.CA_STATE_TAX);

            if (bFindStateTax && lbusPayeeAccountTaxWithholdingState != null)
            {
                lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                if (lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_DOLLAR)
                {
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount =
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount *
                                (Convert.ToDecimal(acdoPayeeAccount["PERCENT_INCREASE"]) / 100);
                }
                lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.start_date = DateTime.Now;
                lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.end_date = DateTime.MinValue;
                lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.Insert();
                bFindStateTax=false;
            }
            if (bFindStateTax)
            {
                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholdingStateGA = new busPayeeAccountTaxWithholding();
                lbusPayeeAccountTaxWithholdingState =
                        lbusPayeeAccountTaxWithholdingStateGA.LoadTaxWithHoldingByPayeeAccountIdAndTaxType(lbusPayeeAccount.icdoPayeeAccount.reference_id, busConstant.GA_STATE_TAX);

                if (bFindStateTax && lbusPayeeAccountTaxWithholdingState != null)
                {
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    if (lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_DOLLAR)
                    {
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount =
                            lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount *
                                    (Convert.ToDecimal(acdoPayeeAccount["PERCENT_INCREASE"]) / 100);
                    }
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.start_date = DateTime.Now;
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.end_date = DateTime.MinValue;
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.Insert();
                    bFindStateTax = false;
                }
            }
            if (bFindStateTax)
            {
                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholdingStateNC = new busPayeeAccountTaxWithholding();
                lbusPayeeAccountTaxWithholdingState =
                    lbusPayeeAccountTaxWithholdingStateNC.LoadTaxWithHoldingByPayeeAccountIdAndTaxType(lbusPayeeAccount.icdoPayeeAccount.reference_id, busConstant.NC_STATE_TAX);

                if (bFindStateTax && lbusPayeeAccountTaxWithholdingState != null)
                {
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    if (lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_DOLLAR)
                    {
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount =
                            lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount *
                                    (Convert.ToDecimal(acdoPayeeAccount["PERCENT_INCREASE"]) / 100);
                    }
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.start_date = DateTime.Now;
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.end_date = DateTime.MinValue;
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.Insert();
                    bFindStateTax = false;
                }
            }
            if (bFindStateTax)
            {
                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholdingStateOR = new busPayeeAccountTaxWithholding();
                lbusPayeeAccountTaxWithholdingState =
                    lbusPayeeAccountTaxWithholdingStateOR.LoadTaxWithHoldingByPayeeAccountIdAndTaxType(lbusPayeeAccount.icdoPayeeAccount.reference_id, busConstant.OR_STATE_TAX);

                if (bFindStateTax && lbusPayeeAccountTaxWithholdingState != null)
                {
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    if (lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_DOLLAR)
                    {
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount =
                            lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount *
                                    (Convert.ToDecimal(acdoPayeeAccount["PERCENT_INCREASE"]) / 100);
                    }
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.start_date = DateTime.Now;
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.end_date = DateTime.MinValue;
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.Insert();
                    bFindStateTax = false;
                }
            }
            if (bFindStateTax)
            {
                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholdingStateVA = new busPayeeAccountTaxWithholding();
                lbusPayeeAccountTaxWithholdingState =
                    lbusPayeeAccountTaxWithholdingStateVA.LoadTaxWithHoldingByPayeeAccountIdAndTaxType(lbusPayeeAccount.icdoPayeeAccount.reference_id, busConstant.VA_STATE_TAX);

                if (bFindStateTax && lbusPayeeAccountTaxWithholdingState != null)
                {
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    if (lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_DOLLAR)
                    {
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount =
                            lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount *
                                    (Convert.ToDecimal(acdoPayeeAccount["PERCENT_INCREASE"]) / 100);
                    }
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.start_date = DateTime.Now;
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.end_date = DateTime.MinValue;
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.Insert();
                    bFindStateTax = false;
                }
            }

            #endregion


            #region Calculate Tax Withholdings
            lbusPayeeAccount.idtNextBenefitPaymentDate = adteNextBenefitPaymentDate;
            lbusPayeeAccount.ProcessTaxWithHoldingDetails(true);
            lbusPayeeAccount.LoadPayeeAccountTaxWithholdings();


            #endregion

            #region ACH Informaation


            abusPayeeAccount.LoadPayeeAccountAchDetails();
            lbusPayeeAccount.iclbPayeeAccountAchDetail = new Collection<busPayeeAccountAchDetail>();
            foreach (busPayeeAccountAchDetail lbusPayeeAccountAchDetail in abusPayeeAccount.iclbPayeeAccountAchDetail)
            {
                if (lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date == DateTime.MinValue ||
                    lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date >= adteNextBenefitPaymentDate)
                {
                    busPayeeAccountAchDetail lbusRetireePayeeAccountAchDetail = new busPayeeAccountAchDetail { icdoPayeeAccountAchDetail = new cdoPayeeAccountAchDetail() };
                    lbusRetireePayeeAccountAchDetail = lbusPayeeAccountAchDetail;
                    lbusRetireePayeeAccountAchDetail.icdoPayeeAccountAchDetail.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    lbusRetireePayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date = DateTime.Now;
                    lbusRetireePayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.MinValue;

                    lbusRetireePayeeAccountAchDetail.icdoPayeeAccountAchDetail.Insert();
                    lbusPayeeAccount.iclbPayeeAccountAchDetail.Add(lbusRetireePayeeAccountAchDetail);
                }
            }

            #endregion

            #region Calculate Deduction

            decimal ldecTaxAmount = 0;

            abusPayeeAccount.LoadPayeeAccountDeduction();
            if (abusPayeeAccount.iclbPayeeAccountDeduction != null && abusPayeeAccount.iclbPayeeAccountDeduction.Count > 0)
            {
                lbusPayeeAccount.LoadPayeeAccountPaymentItemType();
                if (lbusPayeeAccount.iclbPayeeAccountTaxWithholding != null && lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Count > 0
                    && lbusPayeeAccount.iclbPayeeAccountPaymentItemType != null && lbusPayeeAccount.iclbPayeeAccountPaymentItemType.Count > 0)
                {
                    foreach (busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding in lbusPayeeAccount.iclbPayeeAccountTaxWithholding)
                    {
                        lbusPayeeAccountTaxWithholding.iclbPayeeAccountTaxWithholdingItemDetail = new Collection<busPayeeAccountTaxWithholdingItemDetail>();
                        lbusPayeeAccountTaxWithholding.LoadPayeeAccountTaxWithholdingItemDetails();
                        foreach (busPayeeAccountTaxWithholdingItemDetail lbusPayeeAccountTaxWithholdingItemDetail in lbusPayeeAccountTaxWithholding.iclbPayeeAccountTaxWithholdingItemDetail)
                        {
                            if (lbusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(t => t.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id ==
                                lbusPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id).Count() > 0)
                            {
                                ldecTaxAmount += lbusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(t => t.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id ==
                                lbusPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;
                            }
                        }
                    }
                }
            }


            //abusPayeeAccount.LoadPayeeAccountDeduction();
            lbusPayeeAccount.iclbPayeeAccountDeduction = new Collection<busPayeeAccountDeduction>();
            foreach (busPayeeAccountDeduction lbusPayeeAccountDeduction in abusPayeeAccount.iclbPayeeAccountDeduction)
            {
                if (lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.end_date == DateTime.MinValue ||
                    lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.end_date >= adteNextBenefitPaymentDate)
                {
                    busPayeeAccountDeduction lbusRetireePayeeAccountDeduction = new busPayeeAccountDeduction { icdoPayeeAccountDeduction = new cdoPayeeAccountDeduction() };
                    lbusRetireePayeeAccountDeduction = lbusPayeeAccountDeduction;

                    lbusRetireePayeeAccountDeduction.icdoPayeeAccountDeduction.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    lbusRetireePayeeAccountDeduction.icdoPayeeAccountDeduction.start_date = DateTime.Now;
                    lbusRetireePayeeAccountDeduction.icdoPayeeAccountDeduction.end_date = DateTime.MinValue;

                    decimal ldecDeductionAmount = 0;
                    ldecDeductionAmount = (lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.amount *
                        (Convert.ToDecimal(acdoPayeeAccount["PERCENT_INCREASE"]) / 100));

                    if ((ldecRetireeIncAmt - ldecTaxAmount) < ldecDeductionAmount)
                    {
                        lbusRetireePayeeAccountDeduction.icdoPayeeAccountDeduction.amount = ldecRetireeIncAmt - ldecTaxAmount;
                    }
                    else
                    {
                        lbusRetireePayeeAccountDeduction.icdoPayeeAccountDeduction.amount = ldecDeductionAmount;
                    }

                    lbusRetireePayeeAccountDeduction.icdoPayeeAccountDeduction.Insert();
                    lbusPayeeAccount.iclbPayeeAccountDeduction.Add(lbusRetireePayeeAccountDeduction);


                }
            }
            lbusPayeeAccount.ProcessDeductionDetails();

            #endregion

        }

        private void SetPayeeAccountInfo(busPayeeAccount abusPayeeAccount, DateTime adtEffectiveStartDate, DataRow acdoPayeeAccount)
        {

            #region retiree Increase Report
            abusPayeeAccount.icdoPayeeAccount.istrParticipantName = acdoPayeeAccount["PARTICIPANT_NAME"] == DBNull.Value ? "" : acdoPayeeAccount["PARTICIPANT_NAME"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrMPID = acdoPayeeAccount["MPI_PERSON_ID"] == DBNull.Value ? "" : acdoPayeeAccount["MPI_PERSON_ID"].ToString();
            abusPayeeAccount.icdoPayeeAccount.intPlanYear = acdoPayeeAccount["PLAN_YEAR"] == DBNull.Value ? 0 : Convert.ToInt32(acdoPayeeAccount["PLAN_YEAR"]);
            abusPayeeAccount.icdoPayeeAccount.istrPlanDescription = acdoPayeeAccount["PLAN_NAME"] == DBNull.Value ? "" : acdoPayeeAccount["PLAN_NAME"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrMDAge = acdoPayeeAccount["MD_AGE"] == DBNull.Value ? "" : acdoPayeeAccount["MD_AGE"].ToString();
            abusPayeeAccount.icdoPayeeAccount.idecFederalTax = acdoPayeeAccount["FEDERAL_TAX_AMOUNT"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["FEDERAL_TAX_AMOUNT"]);
            abusPayeeAccount.icdoPayeeAccount.idecStateTax = acdoPayeeAccount["STATE_TAX_AMOUNT"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["STATE_TAX_AMOUNT"]);
            abusPayeeAccount.icdoPayeeAccount.idecNetAmount = acdoPayeeAccount["NET_AMOUNT"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["NET_AMOUNT"]);
            //abusPayeeAccount.icdoPayeeAccount.istrRetireeIncreaseEligible = acdoPayeeAccount["RETIREE_INCREASE_ELIGIBLE"] == DBNull.Value ? "" : acdoPayeeAccount["RETIREE_INCREASE_ELIGIBLE"].ToString();
            //abusPayeeAccount.icdoPayeeAccount.istrRolloverEligible = acdoPayeeAccount["ROLLOVER_ELIGIBLE"] == DBNull.Value ? "" : acdoPayeeAccount["ROLLOVER_ELIGIBLE"].ToString();
            //abusPayeeAccount.icdoPayeeAccount.istrRolloverGroup = acdoPayeeAccount["ROLLOVER_Group"] == DBNull.Value ? "" : acdoPayeeAccount["ROLLOVER_Group"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrPaymentMethod = acdoPayeeAccount["PAYMENT_METHOD"] == DBNull.Value ? "" : acdoPayeeAccount["PAYMENT_METHOD"].ToString();
            //abusPayeeAccount.icdoPayeeAccount.istrContactName = acdoPayeeAccount["CONTACT_NAME"] == DBNull.Value ? "" : acdoPayeeAccount["CONTACT_NAME"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrPersonType = acdoPayeeAccount["PERSON_TYPE"] == DBNull.Value ? "" : acdoPayeeAccount["PERSON_TYPE"].ToString();
            abusPayeeAccount.icdoPayeeAccount.idecRetireeIncAmt = acdoPayeeAccount["RetireeIncAmt"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["RetireeIncAmt"]);
            abusPayeeAccount.icdoPayeeAccount.idecGrossAmt = acdoPayeeAccount["idecGrossAmount"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["idecGrossAmount"]);
            abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate = acdoPayeeAccount["BENEFIT_BEGIN_DATE"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(acdoPayeeAccount["BENEFIT_BEGIN_DATE"]);
            //abusPayeeAccount.icdoPayeeAccount.IS_ROLLOVER = acdoPayeeAccount["ROLLOVER_ELIGIBLE"] == DBNull.Value ? string.Empty : Convert.ToString(acdoPayeeAccount["ROLLOVER_ELIGIBLE"]);
            abusPayeeAccount.icdoPayeeAccount.istrPercentIncrease = acdoPayeeAccount["PERCENT_INCREASE"] == DBNull.Value ? string.Empty : Convert.ToString(acdoPayeeAccount["PERCENT_INCREASE"]);

            #endregion

            abusPayeeAccount.istrBenefitBeginDate = busGlobalFunctions.ConvertDateIntoDifFormat(adtEffectiveStartDate);
            abusPayeeAccount.istrMonthYear = iobjSystemManagement.icdoSystemManagement.batch_date.Year.ToString();
            abusPayeeAccount.iintPlanYear = adtEffectiveStartDate.Year;
        }


        private void LoadDataForOnetimePayeeAccount(busPerson abusPerson, busPayeeAccount abusPayeeAccount, DataRow acdoPayeeAccount,
                                                            DateTime adtEffectiveStartDate, ref int aintPayementCount, ref int aintNonSuspendibleMonth, ref decimal adecGrossAmount)
        {
            int lintPlanId = Convert.ToInt32(acdoPayeeAccount[enmPlanBenefitXr.plan_id.ToString()]);
            busCalculation lbusCalculation = new busCalculation();

            abusPerson.icdoPerson.LoadData(acdoPayeeAccount);
            abusPerson.FindPerson(abusPerson.icdoPerson.person_id);

            //DateTime ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(aintComputationYear, 01);
            //DateTime ldtendDate = busGlobalFunctions.GetLastPayrollDayOfMonth(aintComputationYear, 07);

            abusPayeeAccount.icdoPayeeAccount.LoadData(acdoPayeeAccount);
            abusPayeeAccount.FindPayeeAccount(abusPayeeAccount.icdoPayeeAccount.payee_account_id);
            SetPayeeAccountInfo(abusPayeeAccount, adtEffectiveStartDate, acdoPayeeAccount);
            abusPayeeAccount.LoadPayeeAccountPaymentItemType();
            abusPayeeAccount.LoadMonthlyGrossAmount();
            adecGrossAmount = abusPayeeAccount.idecGrossAmount;

            if (abusPayeeAccount.ibusParticipant == null)
            {
                abusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                abusPayeeAccount.ibusParticipant.FindPerson(abusPayeeAccount.icdoPayeeAccount.person_id);
            }

            if (acdoPayeeAccount["Status"].ToString() != busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING)
            {
                DataTable ldtbPaymentInfo = busBase.Select("cdoPayeeAccount.GetPaymentCount", new object[2] {
                                    abusPayeeAccount.icdoPayeeAccount.payee_account_id, abusPayeeAccount.icdoPayeeAccount.person_id });

                aintPayementCount = ldtbPaymentInfo.Rows.Count;
            }
            /*
            if (abusPayeeAccount.icdoPayeeAccount.reemployed_flag == busConstant.FLAG_YES)
            {
                Dictionary<int, Dictionary<int, decimal>> ldictHoursAfterRetirement = new Dictionary<int, Dictionary<int, decimal>>();
                DateTime ldtLastWorkingDate = new DateTime();
                string lstrEmpName = string.Empty;
                int lintReemployedYear = 0;
                ldictHoursAfterRetirement = lbusCalculation.LoadMPIHoursAfterRetirementDate(abusPerson.icdoPerson.istrSSNNonEncrypted,
                    ldtStartDate.AddMonths(-1), busConstant.MPIPP_PLAN_ID, ref ldtLastWorkingDate, ref lstrEmpName, lintReemployedYear);
                abusPayeeAccount.ibusParticipant.LoadPersonSuspendibleMonth();
                aintNonSuspendibleMonth = 7 - (lbusCalculation.GetSuspendibleMonthsBetweenTwoDates(ldictHoursAfterRetirement, abusPayeeAccount.ibusParticipant.iclbPersonSuspendibleMonth, ldtStartDate, ldtendDate));

                //Get the accrued benefit in case of re-employment If Suspended then directly from payment item types if not (could be getting ee derived) then from payment history.
                if (aintNonSuspendibleMonth > 0)
                {
                    abusPayeeAccount.LoadPayeeAccountStatuss();
                    if (!abusPayeeAccount.iclbPayeeAccountStatus.IsNullOrEmpty())
                    {
                        if (abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(item => item.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_value != busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED)
                        {
                            abusPayeeAccount.LoadReemploymentHistorys();
                            if (!abusPayeeAccount.iclcReemploymentHistory.IsNullOrEmpty())
                            {
                                if ((abusPayeeAccount.iclcReemploymentHistory.Where(item => item.reemployed_flag_to_date == DateTime.MinValue)).Count() > 0)
                                {
                                    if (abusPayeeAccount.icdoPayeeAccount.benefit_calculation_detail_id == 0)
                                    {
                                        DataTable ldtGross = new DataTable();
                                        ArrayList larrPaymentDate = lbusCalculation.GetNonSuspendibleMonthBetweenTwoDates(ldictHoursAfterRetirement, abusPayeeAccount.ibusParticipant.iclbPersonSuspendibleMonth, ldtStartDate, ldtendDate);
                                        if (larrPaymentDate != null && larrPaymentDate.Count > 0)
                                        {
                                            foreach (DateTime ldtPaymentDate in larrPaymentDate)
                                            {
                                                DateTime ldt = new DateTime(ldtPaymentDate.Year, ldtPaymentDate.Month, 01);
                                                ldtGross = busBase.Select("cdoPaymentHistoryDistribution.GetGrossAmountInAMonth", new object[2] { abusPayeeAccount.icdoPayeeAccount.payee_account_id, ldt });
                                                if (ldtGross.Rows.Count > 0)
                                                {
                                                    if (Convert.ToString(ldtGross.Rows[0]["Gross_Amount"]).IsNotNullOrEmpty())
                                                    {
                                                        adecGrossAmount = Convert.ToDecimal(ldtGross.Rows[0]["Gross_Amount"]);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        DataTable ldtGross = busBase.Select("cdoPaymentHistoryDistribution.GetBenefitAmtFromCalc", new object[1] { abusPayeeAccount.icdoPayeeAccount.payee_account_id });
                                        if (ldtGross.Rows.Count > 0)
                                        {
                                            if (Convert.ToString(ldtGross.Rows[0][0]).IsNotNullOrEmpty())
                                            {
                                                adecGrossAmount = Convert.ToDecimal(ldtGross.Rows[0][0]);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                aintNonSuspendibleMonth = lbusCalculation.GetNonSuspendibleMonths(abusPerson.icdoPerson.istrSSNNonEncrypted, abusPerson, aintComputationYear, lintPlanId, null, ldtStartDate, ldtendDate, false);

            }
            */
        }

        #endregion

    }
}
