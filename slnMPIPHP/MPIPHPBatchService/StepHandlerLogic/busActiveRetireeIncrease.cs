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
    class busActiveRetireeIncreaseBatch : busBatchHandler
    {

        #region Properties

        private object iobjLock = null;
        //WI 14763 RID 118342
        bool iblnApprovedGroupOnly { get; set; }

        #endregion
        //WI 14763 RID 118342  adding iblnApprovedGroupOnly parameter
        private void RetrieveRetireeIncreaseBatchParameters()
        {
            if (ibusJobHeader != null)
            {
                if (ibusJobHeader.iclbJobDetail == null)
                    ibusJobHeader.LoadJobDetail(true);

                foreach (busJobDetail lobjDetail in ibusJobHeader.iclbJobDetail)
                {
                    foreach (busJobParameters lobjParam in lobjDetail.iclbJobParameters)
                    {
                        switch (lobjParam.icdoJobParameters.param_name)
                        {
                            //WI 14763 RID 118342
                            case busConstant.JobParamApprovedGroupOnly:
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value) == "Y")
                                    iblnApprovedGroupOnly = true;
                                else
                                    iblnApprovedGroupOnly = false;
                                break;

                        }
                    }
                }
            }
        }

        #region Public Methods

        public void ActiveRetireeIncreaseBatch()
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

            #region Get Plan Year

            DataTable ldtbJobScheduleInfo = busBase.Select("cdoPayeeAccount.GetPlanYrForRetireeInc",
                                            new object[1] { busConstant.MPIPHPBatch.ACTIVE_RETIREE_INCREASE_BATCH_SCHEDULE_ID });
            int lintPlanYear = 0;

            if (ldtbJobScheduleInfo.Rows.Count > 0 && Convert.ToString(ldtbJobScheduleInfo.Rows[0][enmJobScheduleParams.param_value.ToString()]).IsNotNullOrEmpty())
            {
                lintPlanYear = Convert.ToInt32(ldtbJobScheduleInfo.Rows[0][enmJobScheduleParams.param_value.ToString()]);
            }
            else
            {
                lintPlanYear = iobjSystemManagement.icdoSystemManagement.batch_date.Year;
            }

            #endregion

            //WI 14763 RID 118342  adding iblnApprovedGroupOnly parameter
            RetrieveRetireeIncreaseBatchParameters();
            //WI 14763 RID 118342  adding iblnApprovedGroupOnly parameter
            string strApprovedGroupFlag = busConstant.FLAG_NO;
            if (this.iblnApprovedGroupOnly)
                strApprovedGroupFlag = busConstant.FLAG_YES;

            DataTable ldtbPersonAccountInfo = busBase.Select("cdoPayeeAccount.LoadPayeeAccountForActiveRetiree", new object[2] { lintPlanYear, strApprovedGroupFlag });

            busActiveRetireeIncreaseContract lbusActiveRetireeIncreaseContract = new busActiveRetireeIncreaseContract();
            lbusActiveRetireeIncreaseContract = lbusActiveRetireeIncreaseContract.LoadActiveRetireeIncContractByPlanYear(lintPlanYear);

            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

            Parallel.ForEach(ldtbPersonAccountInfo.AsEnumerable(), po, (acdoPerson, loopState) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "ActiveRetireeIncreaseBatch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;

                if (Convert.ToString(acdoPerson["PERCENT_INCREASE"]).IsNotNullOrEmpty())
                {
                    CalculateAndCreateActiveRetireePayeeAccounts(acdoPerson, lintPlanYear, lobjPassInfo, lintCount, lintTotalCount, ldtbPersonAccountInfo);
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
            lobjPassInfo1.BeginTransaction();

            if (lbusActiveRetireeIncreaseContract != null)
            {
                DateTime ldtEndDate = new DateTime(lintPlanYear, 12, 31);

                //WI 14763 RID 118342  adding iblnApprovedGroupOnly parameter
                if (this.iblnApprovedGroupOnly)
                {
                    DBFunction.DBNonQuery("cdoTempdata.UpdateApprovedGroupsEffectiveEndDate",
                                   new object[2] { ldtEndDate, lintPlanYear },
                                                         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
                else
                {
                    lbusActiveRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.effective_end_date = ldtEndDate;
                    lbusActiveRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.Update();
                    lobjPassInfo1.Commit();
                }
            }

            if (lobjPassInfo1.iconFramework.State == ConnectionState.Open)
            {
                lobjPassInfo1.iconFramework.Close();
            }

            #endregion

            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        }

        #endregion

        #region Private Methods

        private void CalculateAndCreateActiveRetireePayeeAccounts(DataRow acdoPayeeAccount, int aintComputationYear, utlPassInfo autlPassInfo,
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
                DateTime ldteNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate(lintPlanId).AddMonths(1);


                LoadDataForRetireeIncreasePayeeAccount(lbusPerson, lbusPayeeAccount, acdoPayeeAccount, aintComputationYear,
                                                                        ref lintPayementCount, ref lintNonSuspendibleMonth, ref ldecGrossAmount);

                if ((lbusPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form == busConstant.FLAG_NO ||
                     lbusPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form.IsNullOrEmpty()) &&
                    (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING ||
                    (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED //&& lintPayementCount >= 1
                    ) ||
                    (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED //&& lintPayementCount >= 1
                    ) ||
                    (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_REVIEW //&& lintPayementCount >= 1
                    )) &&
                    ((lintNonSuspendibleMonth >= 1 || lbusPayeeAccount.icdoPayeeAccount.istrMDAge == busConstant.FLAG_YES) && lbusPerson.icdoPerson.date_of_death == DateTime.MinValue) && ldecGrossAmount != 0) //RID 53296
                {
                    decimal ldecGuaranteedAmt = 0;
                    DataTable ldtbParticipantPayeeAccountDetails = new DataTable();

                    if (Convert.ToString(acdoPayeeAccount[enmPayeeAccount.benefit_account_type_value.ToString()]) != busConstant.BENEFIT_TYPE_QDRO && adtPersonInfo.AsEnumerable().Where(
                                     item => item.Field<int>(enmPayeeAccount.payee_benefit_account_id.ToString()) == lbusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id &&
                                     item.Field<string>(enmPayeeAccount.benefit_account_type_value.ToString()) == busConstant.BENEFIT_TYPE_QDRO &&
                                     item.Field<string>(enmDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase.ToString()) == busConstant.FLAG_YES).Count() == 0)
                    {
                       CreateRetireeIncreasePayeeAccounts(acdoPayeeAccount, lintPlanId, lbusPayeeAccount, ldteNextBenefitPaymentDate, ldecGrossAmount, 0, 0, 0);

                        if (acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()].ToString().IsNotNullOrEmpty())
                        {
                            ldecGuaranteedAmt = Convert.ToDecimal(acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()]);
                            CreateRetireeIncreasePayeeAccounts(acdoPayeeAccount, lintPlanId, lbusPayeeAccount, ldteNextBenefitPaymentDate, 0, ldecGuaranteedAmt, 0, 0);
                        }
                    }
                    else if (Convert.ToString(acdoPayeeAccount[enmPayeeAccount.benefit_account_type_value.ToString()]) == busConstant.BENEFIT_TYPE_QDRO)
                    {
                        CreateRetireeIncreasePayeeAccounts(acdoPayeeAccount, lintPlanId, lbusPayeeAccount, ldteNextBenefitPaymentDate, ldecGrossAmount, 0, 0, 0);

                        if (acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()].ToString().IsNotNullOrEmpty())
                        {
                            ldecGuaranteedAmt = Convert.ToDecimal(acdoPayeeAccount[enmLocal700GuaranteedAmount.guaranteed_amount.ToString()]);
                            CreateRetireeIncreasePayeeAccounts(acdoPayeeAccount, lintPlanId, lbusPayeeAccount, ldteNextBenefitPaymentDate, 0, ldecGuaranteedAmt, 0, 0);
                        }

                        if (Convert.ToString(acdoPayeeAccount[enmDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase.ToString()]) == busConstant.FLAG_YES)
                        {
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
                                LoadDataForRetireeIncreasePayeeAccount(lbusParticipant, lbusParticipantPayeeAccount, ldtbParticipantPayeeAccountDetails.Rows[0], aintComputationYear,
                                                                        ref lintParticipantPaymentCount, ref lintParticipantNonSuspendibleMonth, ref ldecParticipantGrossAmount);

                                if ((lbusParticipantPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form == busConstant.FLAG_NO ||
                                        lbusParticipantPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form.IsNullOrEmpty()) &&
                                        (ldtbParticipantPayeeAccountDetails.Rows[0]["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING ||
                                        (acdoPayeeAccount["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED && lintParticipantPaymentCount >= 1) ||
                                        (ldtbParticipantPayeeAccountDetails.Rows[0]["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED && lintParticipantPaymentCount >= 1) ||
                                        (ldtbParticipantPayeeAccountDetails.Rows[0]["Status"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_REVIEW && lintParticipantPaymentCount >= 1)) &&
                                        ((lintParticipantNonSuspendibleMonth >= 1 || lbusParticipantPayeeAccount.icdoPayeeAccount.istrMDAge == busConstant.FLAG_YES) && lbusParticipant.icdoPerson.date_of_death == DateTime.MinValue) && ldecParticipantGrossAmount != 0)  //RID 53296
                                {
                                    CreateRetireeIncreasePayeeAccounts(ldtbParticipantPayeeAccountDetails.Rows[0], lintPlanId, lbusParticipantPayeeAccount, ldteNextBenefitPaymentDate, ldecParticipantGrossAmount,
                                                                        0, lbusPayeeAccount.icdoPayeeAccount.person_id, lbusPayeeAccount.icdoPayeeAccount.payee_account_id);

                                    if (ldtbParticipantPayeeAccountDetails.Rows[0][enmLocal700GuaranteedAmount.guaranteed_amount.ToString()].ToString().IsNotNullOrEmpty())
                                    {
                                        decimal ldecParticipantGuaranteedAmt = Convert.ToDecimal(ldtbParticipantPayeeAccountDetails.Rows[0][enmLocal700GuaranteedAmount.guaranteed_amount.ToString()]);
                                        CreateRetireeIncreasePayeeAccounts(ldtbParticipantPayeeAccountDetails.Rows[0], lintPlanId, lbusParticipantPayeeAccount,
                                                                            ldteNextBenefitPaymentDate, 0, ldecParticipantGuaranteedAmt, lbusPayeeAccount.icdoPayeeAccount.person_id, lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (lbusPayeeAccount.icdoPayeeAccount.review_payee_acc_for_retiree_inc_form == busConstant.FLAG_YES)
                {
                    #region Create Payee Account in Review status

                    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();

                    lbusPayeeAccount.icdoPayeeAccount.plan_benefit_id = lbusPlanBenefitXr.GetPlanBenefitId(lintPlanId, busConstant.LUMP_SUM);
                    lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date = ldteNextBenefitPaymentDate;
                    lbusPayeeAccount.icdoPayeeAccount.reference_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    lbusPayeeAccount.icdoPayeeAccount.Insert();

                    lbusPayeeAccount.CreateReviewPayeeAccountStatus();

                    #endregion
                }

                lbusPayeeAccount.istrBenefitBeginDate = busGlobalFunctions.ConvertDateIntoDifFormat(lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date);
                lbusPayeeAccount.istrMonthYear = lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date.Year.ToString();
                lbusPayeeAccount.iintPlanYear = aintComputationYear;

                aarrResult.Add(lbusPayeeAccount);

                autlPassInfo.Commit();

            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    string lstrMsg = "Error while Executing Batch, Error Message: " + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();
            }

        }

        private void CreateRetireeIncreasePayeeAccounts(DataRow acdoPayeeAccount, int aintPlanId, busPayeeAccount abusPayeeAccount,
                                                        DateTime adteNextBenefitPaymentDate, decimal adecGrossAmount, decimal adecGuaranteedAmt,
                                                        int aintQDROPersonId, int aintQDROPayeeAccountId)
        {
            #region Create Payee Account in Approved status

            //Ticket#131229
            //DateTime dt = new DateTime();
            DateTime dtRetireeInc = new DateTime();

            DataTable ldtbPlanYear = busBase.Select("cdoActiveRetireeIncreaseContract.GetActiveRetireeIncreasePlanYear", new object[0] { });

            if (ldtbPlanYear.Rows.Count > 0)
            {
                if (adteNextBenefitPaymentDate != Convert.ToDateTime("11/01/" + ldtbPlanYear.Rows[0][0].ToString()))
                {
                    dtRetireeInc = Convert.ToDateTime("11/01/" + ldtbPlanYear.Rows[0][0].ToString());
                }
                else
                {
                    dtRetireeInc = adteNextBenefitPaymentDate;
                }

            }
           


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
                                                    //Ticket#131229
                                                    abusPayeeAccount.icdoPayeeAccount.retirement_type_value, dtRetireeInc, abusPayeeAccount.icdoPayeeAccount.benefit_end_date,
                                                    abusPayeeAccount.icdoPayeeAccount.account_relation_value, abusPayeeAccount.icdoPayeeAccount.family_relation_value,
                    //abusPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount, abusPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance,
                                                    0, 0,
                                                    lbusPlanBenefitXr.GetPlanBenefitId(aintPlanId, busConstant.LUMP_SUM), DateTime.MinValue,
                                                    busConstant.FLAG_YES, busConstant.FLAG_NO, false, aintQDROPayeeAccountId);
            }
            else
            {
                lbusPayeeAccount.ManagePayeeAccount(0, abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.org_id,
                                                    abusPayeeAccount.icdoPayeeAccount.benefit_application_detail_id, abusPayeeAccount.icdoPayeeAccount.benefit_calculation_detail_id,
                                                    abusPayeeAccount.icdoPayeeAccount.dro_application_detail_id, abusPayeeAccount.icdoPayeeAccount.dro_calculation_detail_id,
                                                    abusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id, abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
                                                    //Ticket#131229
                                                    abusPayeeAccount.icdoPayeeAccount.retirement_type_value, dtRetireeInc, abusPayeeAccount.icdoPayeeAccount.benefit_end_date,
                                                    abusPayeeAccount.icdoPayeeAccount.account_relation_value, abusPayeeAccount.icdoPayeeAccount.family_relation_value,
                    //abusPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount, abusPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance,
                                                    0, 0,
                                                    lbusPlanBenefitXr.GetPlanBenefitId(aintPlanId, busConstant.LUMP_SUM), DateTime.MinValue,
                                                    busConstant.FLAG_YES, busConstant.FLAG_NO, false, abusPayeeAccount.icdoPayeeAccount.payee_account_id);
            }


            lbusPayeeAccount.icdoPayeeAccount.reference_id = abusPayeeAccount.icdoPayeeAccount.payee_account_id;

            busPayeeAccountStatus lbusPayeeAccountStatus = new busPayeeAccountStatus();

            //RID 75975
            //if (Convert.ToString(acdoPayeeAccount["Status"]).IsNotNullOrEmpty() &&
            //    Convert.ToString(acdoPayeeAccount["Status"]) == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED)
            //{
            //    lbusPayeeAccountStatus.InsertValuesInPayeeAccountStatus(lbusPayeeAccount.icdoPayeeAccount.payee_account_id,
            //                                                        busConstant.PAYEE_ACCOUNT_STATUS_REVIEW, DateTime.Now);
            //}
            //else
            //{
            //    lbusPayeeAccountStatus.InsertValuesInPayeeAccountStatus(lbusPayeeAccount.icdoPayeeAccount.payee_account_id,
            //                                                       busConstant.PAYEE_ACCOUNT_STATUS_APPROVED, DateTime.Now);
            //}

            //RID 75975
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


            #region Code Commented - Check If Rollover Exists For Previous Year Retiree Increase Payee Account
            /*
            DataTable ldtblPreviousYearRolloverPayeeAccount = busBase.Select("cdoTempdata.CheckIfRollover",
                new object[3] { lbusPayeeAccount.icdoPayeeAccount.person_id, lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value, adteNextBenefitPaymentDate.Year - 1 });
            if (ldtblPreviousYearRolloverPayeeAccount != null && ldtblPreviousYearRolloverPayeeAccount.Rows.Count > 0)
            {
                lblnCheckIfRollover = true;
                busPayeeAccount lbusRolloverPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lbusRolloverPayeeAccount.icdoPayeeAccount.LoadData(ldtblPreviousYearRolloverPayeeAccount.Rows[0]);
                lbusRolloverPayeeAccount.iclbPayeeAccountRolloverDetail = new Collection<busPayeeAccountRolloverDetail>();
                lbusRolloverPayeeAccount.LoadPayeeAccountRolloverDetails();

                busPayeeAccountPaymentItemType lbusRoloverPayeeAccountPayementItemType = lbusPayeeAccount.CreatePayeeAccountPaymentItemType(
                50, ldecRetireeIncAmt, "0", 0, adteNextBenefitPaymentDate, DateTime.MinValue, "N");
                lbusPayeeAccountPayementItemType.icdoPayeeAccountPaymentItemType.Insert();


                busPayeeAccountRolloverDetail lbusPayeeAccountRolloverDetail = new busPayeeAccountRolloverDetail { icdoPayeeAccountRolloverDetail = new cdoPayeeAccountRolloverDetail() };
                lbusPayeeAccountRolloverDetail = lbusRolloverPayeeAccount.iclbPayeeAccountRolloverDetail.FirstOrDefault();
                lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.amount = ldecRetireeIncAmt;
                lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.status_value = busConstant.PayeeAccountRolloverDetailStatusActive;
                lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.created_by = iobjPassInfo.istrUserID;
                lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.created_date = DateTime.Now;
                lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.modified_by = iobjPassInfo.istrUserID;
                lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.modified_date = DateTime.Now;
                lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.update_seq = 0;
                lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.Insert();

                lbusPayeeAccountRolloverDetail.InsertIntoRolloverItemDetail(lbusRoloverPayeeAccountPayementItemType);



                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
                lbusPayeeAccountTaxWithholding.InsertValuesInTaxWithHolding(lbusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.CA_STATE_TAX,
                     busConstant.Benefit_Distribution_Type_LumpSum, DateTime.Now, DateTime.MinValue, busConstant.NO_STATE_TAX, 0, lbusPayeeAccount.ibusPayee.icdoPerson.marital_status_value, 0, 0);

                lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
                lbusPayeeAccountTaxWithholding.InsertValuesInTaxWithHolding(lbusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.FEDRAL_STATE_TAX,
                    busConstant.Benefit_Distribution_Type_LumpSum, DateTime.Now, DateTime.MinValue, busConstant.NO_FEDRAL_TAX, 0, lbusPayeeAccount.ibusPayee.icdoPerson.marital_status_value, 0, 0);



            }
            */
            #endregion


            #region Calculate Fedral tax

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

            if ((abusPayeeAccount.icdoPayeeAccount.idecRetireeIncAmt < 750 || abusPayeeAccount.icdoPayeeAccount.istrMDAge == busConstant.FLAG_YES))
            {
                //Load existing fedral tax data
                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholdingFed = new busPayeeAccountTaxWithholding();
                lbusPayeeAccountTaxWithholdingFed =
                    lbusPayeeAccountTaxWithholdingFed.LoadTaxWithHoldingByPayeeAccountIdAndTaxType(lbusPayeeAccount.icdoPayeeAccount.reference_id, busConstant.FEDRAL_STATE_TAX);

                if (lbusPayeeAccountTaxWithholdingFed != null)
                {
                    lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    //2022 FDRL tax withholding. We are keeping the same withholding setup as participant's monthly account. Waiting for answer on how to handle Flat dollar amount in new method. 
                    if (lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_DOLLAR)
                    {
                        lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.additional_tax_amount =
                          lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.additional_tax_amount *
                        (Convert.ToDecimal(acdoPayeeAccount["PERCENT_INCREASE"]) / 100);
                    }
                    //PIR 820
                    lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.start_date = DateTime.Now;
                    lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.end_date = DateTime.MinValue;
                    lbusPayeeAccountTaxWithholdingFed.icdoPayeeAccountTaxWithholding.Insert();
                }

                //Load existing State tax data
                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholdingState = new busPayeeAccountTaxWithholding();
                lbusPayeeAccountTaxWithholdingState =
                    lbusPayeeAccountTaxWithholdingState.LoadTaxWithHoldingByPayeeAccountIdAndTaxType(lbusPayeeAccount.icdoPayeeAccount.reference_id, busConstant.CA_STATE_TAX);

                if (lbusPayeeAccountTaxWithholdingState != null)
                {
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    if (lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_DOLLAR)
                    {
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount =
                            lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.additional_tax_amount *
                                 (Convert.ToDecimal(acdoPayeeAccount["PERCENT_INCREASE"]) / 100);
                    }
                    //PIR 820
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.start_date = DateTime.Now;
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.end_date = DateTime.MinValue;
                    lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.Insert();
                }
            }
            else if (abusPayeeAccount.icdoPayeeAccount.idecRetireeIncAmt >= 750)
            {

                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
                //lbusPayeeAccountTaxWithholding.InsertValuesInTaxWithHolding(lbusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.FEDRAL_STATE_TAX,
                //busConstant.Benefit_Distribution_Type_Monthly_Benefit, DateTime.Now, DateTime.MinValue, busConstant.FLAT_PERCENT, 0, "M", 0, 20);

                //2022 FDRL tax withholding not giving option of flat percentage in monthly distribution, but for retiree increase still need to hold flat 20 percent.
                lbusPayeeAccountTaxWithholding.InsertValuesInTaxWithHolding(lbusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.FEDRAL_STATE_TAX,
                busConstant.Benefit_Distribution_Type_LumpSum, DateTime.Now, DateTime.MinValue, busConstant.FLAT_PERCENT, 0, "MQ", 0, 20);

                //Need TO Confirm
                //if (abusPayeeAccount.icdoPayeeAccount.istrPersonType == "Participant Account")
                //{
                //    lbusPayeeAccountTaxWithholding.InsertValuesInTaxWithHolding(lbusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.FEDRAL_STATE_TAX,
                //     busConstant.Benefit_Distribution_Type_Monthly_Benefit, adteNextBenefitPaymentDate, DateTime.MinValue, busConstant.FLAT_PERCENT, 0, "M", 0, 20);

                //}
                //else
                //{
                //    if (abusPayeeAccount.icdoPayeeAccount.istrPersonType == "Joint Annuitant")
                //    {
                //        lbusPayeeAccountTaxWithholding.InsertValuesInTaxWithHolding(lbusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.FEDRAL_STATE_TAX,
                //        busConstant.Benefit_Distribution_Type_Monthly_Benefit, adteNextBenefitPaymentDate, DateTime.MinValue, busConstant.FLAT_PERCENT, 0, "M", 0, 20);
                //    }
                //    else
                //    {
                //        lbusPayeeAccountTaxWithholding.InsertValuesInTaxWithHolding(lbusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.FEDRAL_STATE_TAX,
                //        busConstant.Benefit_Distribution_Type_Monthly_Benefit, adteNextBenefitPaymentDate, DateTime.MinValue, busConstant.FLAT_PERCENT, 0, "M", 0, 10);
                //    }
                //}
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
                    lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date >= new DateTime(adteNextBenefitPaymentDate.Year, 11, 01))
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
                    lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.end_date >= new DateTime(adteNextBenefitPaymentDate.Year, 11, 01))
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

        private void SetPayeeAccountInfo(busPayeeAccount abusPayeeAccount, int aintComputationYear, DataRow acdoPayeeAccount)
        {
            if (abusPayeeAccount.ibusParticipant != null)
            {
                //abusPayeeAccount.ibusParticipant.LoadCorrAddress();
                //abusPayeeAccount.icdoPayeeAccount.istrPrefix = abusPayeeAccount.ibusParticipant.icdoPerson.istrPreFix;
                //abusPayeeAccount.icdoPayeeAccount.istrLastName = abusPayeeAccount.ibusParticipant.icdoPerson.last_name;
                //abusPayeeAccount.icdoPayeeAccount.istrAddrLine1 = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_1;
                //abusPayeeAccount.icdoPayeeAccount.istrAddrLine2 = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_2;
                //abusPayeeAccount.icdoPayeeAccount.istrCity = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_city;
                //abusPayeeAccount.icdoPayeeAccount.istrState = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_description;
                //abusPayeeAccount.icdoPayeeAccount.istrZipCode = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.istrCompleteZipCode;
                //abusPayeeAccount.icdoPayeeAccount.istrForeignPostalCode = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.foreign_postal_code;
                //abusPayeeAccount.icdoPayeeAccount.istrCountryDescription = abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_description;
                //abusPayeeAccount.icdoPayeeAccount.istrCountryValue= abusPayeeAccount.ibusParticipant.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value;

            }

            #region retiree Increase Report
            abusPayeeAccount.icdoPayeeAccount.istrParticipantName = acdoPayeeAccount["PARTICIPANT_NAME"] == DBNull.Value ? "" : acdoPayeeAccount["PARTICIPANT_NAME"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrMPID = acdoPayeeAccount["MPI_PERSON_ID"] == DBNull.Value ? "" : acdoPayeeAccount["MPI_PERSON_ID"].ToString();
            abusPayeeAccount.icdoPayeeAccount.intPlanYear = acdoPayeeAccount["PLAN_YEAR"] == DBNull.Value ? 0 : Convert.ToInt32(acdoPayeeAccount["PLAN_YEAR"]);
            abusPayeeAccount.icdoPayeeAccount.istrPlanDescription = acdoPayeeAccount["PLAN_NAME"] == DBNull.Value ? "" : acdoPayeeAccount["PLAN_NAME"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrMDAge = acdoPayeeAccount["MD_AGE"] == DBNull.Value ? "" : acdoPayeeAccount["MD_AGE"].ToString();
            abusPayeeAccount.icdoPayeeAccount.idecFederalTax = acdoPayeeAccount["FEDERAL_TAX_AMOUNT"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["FEDERAL_TAX_AMOUNT"]);
            abusPayeeAccount.icdoPayeeAccount.idecStateTax = acdoPayeeAccount["STATE_TAX_AMOUNT"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["STATE_TAX_AMOUNT"]);
            abusPayeeAccount.icdoPayeeAccount.idecNetAmount = acdoPayeeAccount["NET_AMOUNT"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["NET_AMOUNT"]);
            abusPayeeAccount.icdoPayeeAccount.istrRetireeIncreaseEligible = acdoPayeeAccount["RETIREE_INCREASE_ELIGIBLE"] == DBNull.Value ? "" : acdoPayeeAccount["RETIREE_INCREASE_ELIGIBLE"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrRolloverEligible = acdoPayeeAccount["ROLLOVER_ELIGIBLE"] == DBNull.Value ? "" : acdoPayeeAccount["ROLLOVER_ELIGIBLE"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrRolloverGroup = acdoPayeeAccount["ROLLOVER_Group"] == DBNull.Value ? "" : acdoPayeeAccount["ROLLOVER_Group"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrPaymentMethod = acdoPayeeAccount["PAYMENT_METHOD"] == DBNull.Value ? "" : acdoPayeeAccount["PAYMENT_METHOD"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrContactName = acdoPayeeAccount["CONTACT_NAME"] == DBNull.Value ? "" : acdoPayeeAccount["CONTACT_NAME"].ToString();
            abusPayeeAccount.icdoPayeeAccount.istrPersonType = acdoPayeeAccount["PERSON_TYPE"] == DBNull.Value ? "" : acdoPayeeAccount["PERSON_TYPE"].ToString();
            abusPayeeAccount.icdoPayeeAccount.idecRetireeIncAmt = acdoPayeeAccount["RetireeIncAmt"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["RetireeIncAmt"]);
            abusPayeeAccount.icdoPayeeAccount.idecGrossAmt = acdoPayeeAccount["idecGrossAmount"] == DBNull.Value ? 0M : Convert.ToDecimal(acdoPayeeAccount["idecGrossAmount"]);
            abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate = acdoPayeeAccount["BENEFIT_BEGIN_DATE"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(acdoPayeeAccount["BENEFIT_BEGIN_DATE"]);
            abusPayeeAccount.icdoPayeeAccount.IS_ROLLOVER = acdoPayeeAccount["ROLLOVER_ELIGIBLE"] == DBNull.Value ? string.Empty : Convert.ToString(acdoPayeeAccount["ROLLOVER_ELIGIBLE"]);
            abusPayeeAccount.icdoPayeeAccount.istrPercentIncrease = acdoPayeeAccount["PERCENT_INCREASE"] == DBNull.Value ? string.Empty : Convert.ToString(acdoPayeeAccount["PERCENT_INCREASE"]);

            #endregion

            DateTime ldtDateTime = new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.Year, 11, 1);
            abusPayeeAccount.istrBenefitBeginDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtDateTime);
            abusPayeeAccount.istrMonthYear = iobjSystemManagement.icdoSystemManagement.batch_date.Year.ToString();
            abusPayeeAccount.iintPlanYear = aintComputationYear;
        }


        private void LoadDataForRetireeIncreasePayeeAccount(busPerson abusPerson, busPayeeAccount abusPayeeAccount, DataRow acdoPayeeAccount,
                                                            int aintComputationYear, ref int aintPayementCount, ref int aintNonSuspendibleMonth, ref decimal adecGrossAmount)
        {
            int lintPlanId = Convert.ToInt32(acdoPayeeAccount[enmPlanBenefitXr.plan_id.ToString()]);
            busCalculation lbusCalculation = new busCalculation();

            //abusPerson = new busPerson { icdoPerson = new cdoPerson() };
            abusPerson.icdoPerson.LoadData(acdoPayeeAccount);
            abusPerson.FindPerson(abusPerson.icdoPerson.person_id);

            #region Calculate non suspendible month count

            //DateTime ldtStartDate = new DateTime(aintComputationYear, 01, 01);
            //RID 75975
            //DateTime ldtendDate = new DateTime(aintComputationYear, 09, 30);
            //DateTime ldtendDate = new DateTime(aintComputationYear, 07, 31);
            DateTime ldtStartDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(aintComputationYear, 01);
            DateTime ldtendDate = busGlobalFunctions.GetLastPayrollDayOfMonth(aintComputationYear, 07);

            #endregion

            abusPayeeAccount.icdoPayeeAccount.LoadData(acdoPayeeAccount);
            abusPayeeAccount.FindPayeeAccount(abusPayeeAccount.icdoPayeeAccount.payee_account_id);
            SetPayeeAccountInfo(abusPayeeAccount, aintComputationYear, acdoPayeeAccount);
            abusPayeeAccount.LoadPayeeAccountPaymentItemType();
            //RID 75975
            //abusPayeeAccount.LoadGrossAmount();
            abusPayeeAccount.LoadMonthlyGrossAmount();
            adecGrossAmount = abusPayeeAccount.idecGrossAmount;




            if (abusPayeeAccount.ibusParticipant == null)
            {
                abusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                abusPayeeAccount.ibusParticipant.FindPerson(abusPayeeAccount.icdoPayeeAccount.person_id);
            }

            if (acdoPayeeAccount["Status"].ToString() != busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING)
            {
                //    aintPayementCount = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.GetCountOfPaymentMade",
                //                                       new object[2] { abusPayeeAccount.icdoPayeeAccount.payee_account_id, abusPayeeAccount.icdoPayeeAccount.person_id }, iobjPassInfo.iconFramework,
                //                                       iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                DataTable ldtbPaymentInfo = busBase.Select("cdoPayeeAccount.GetPaymentCount", new object[2] {
                                    abusPayeeAccount.icdoPayeeAccount.payee_account_id, abusPayeeAccount.icdoPayeeAccount.person_id });

                aintPayementCount = ldtbPaymentInfo.Rows.Count;

            }

            if (abusPayeeAccount.icdoPayeeAccount.reemployed_flag == busConstant.FLAG_YES)
            {
                Dictionary<int, Dictionary<int, decimal>> ldictHoursAfterRetirement = new Dictionary<int, Dictionary<int, decimal>>();
                DateTime ldtLastWorkingDate = new DateTime();
                string lstrEmpName = string.Empty;
                int lintReemployedYear = 0;
                ldictHoursAfterRetirement = lbusCalculation.LoadMPIHoursAfterRetirementDate(abusPerson.icdoPerson.istrSSNNonEncrypted,
                    ldtStartDate.AddMonths(-1), busConstant.MPIPP_PLAN_ID, ref ldtLastWorkingDate, ref lstrEmpName, lintReemployedYear);
                abusPayeeAccount.ibusParticipant.LoadPersonSuspendibleMonth();
                //RID 75975
                //aintNonSuspendibleMonth = 9 - (lbusCalculation.GetSuspendibleMonthsBetweenTwoDates(ldictHoursAfterRetirement, abusPayeeAccount.ibusParticipant.iclbPersonSuspendibleMonth, ldtStartDate, ldtendDate));
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
                                    //DateTime ldtReemployedFromDate = abusPayeeAccount.iclcReemploymentHistory.Where(item => item.reemployed_flag_to_date == DateTime.MinValue).FirstOrDefault().reemployed_flag_from_date;
                                    //ldtReemployedFromDate = new DateTime(ldtReemployedFromDate.Year, ldtReemployedFromDate.Month, 01);
                                    //DataTable ldtGross = busBase.Select("cdoPaymentHistoryDistribution.GetGrossAmountInAMonth", new object[2] { abusPayeeAccount.icdoPayeeAccount.payee_account_id, ldtReemployedFromDate });
                                    //if (ldtGross.Rows.Count > 0)
                                    //{
                                    //    if (Convert.ToString(ldtGross.Rows[0]["Gross_Amount"]).IsNotNullOrEmpty())
                                    //    {
                                    //        adecGrossAmount = Convert.ToDecimal(ldtGross.Rows[0]["Gross_Amount"]);
                                    //    }
                                    //}

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
                aintNonSuspendibleMonth = lbusCalculation.GetNonSuspendibleMonths(abusPerson.icdoPerson.istrSSNNonEncrypted, abusPerson,
                            aintComputationYear, lintPlanId, null, ldtStartDate, ldtendDate, false);

            }
        }

        #endregion


        #region Retiree Increase Rollover batch


        public void RetireeIncreaseRolloverBatch()
        {
            bool lblnFlag = false;
            utlConnection utlLookupDBConnection = HelperFunction.GetDBConnectionProperties("LookupDB");
            string astrLookupDBConnection = utlLookupDBConnection.istrConnectionString;


            SqlParameter[] RolloverInfoparameters = new SqlParameter[0];
            DataTable ldtblRolloverInfoparameters = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_opus_retiree_increase", astrLookupDBConnection, null, RolloverInfoparameters);

            if (ldtblRolloverInfoparameters != null && ldtblRolloverInfoparameters.Rows.Count > 0)
            {
                foreach (DataRow ldrRolloverInfoParameters in ldtblRolloverInfoparameters.Rows)
                {
                    //Rohan 10212014
                    if (ldrRolloverInfoParameters["PID"] != DBNull.Value && ldrRolloverInfoParameters["ScanDate"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["ScanDate"]).IsNotNullOrEmpty()
                        && Convert.ToDateTime(ldrRolloverInfoParameters["ScanDate"]).Year == DateTime.Now.Year)
                    {
                        try
                        {
                            //To get Active Retiree Increase Payee Account Details and Gross Amount
                            DataTable ldtbRetireeIncreasePayeeAccountDetails = busBase.Select("cdoPayeeBenefitAccount.ActiveRetireeIncreasePayeeAccountDetails", new object[2] { Convert.ToString(ldrRolloverInfoParameters["PID"]),
                                              DateTime.Now.Year });
                            if (ldtbRetireeIncreasePayeeAccountDetails != null && ldtbRetireeIncreasePayeeAccountDetails.Rows.Count > 0)
                            {
                                foreach (DataRow ldrRetireeIncreasePayeeAccountDetails in ldtbRetireeIncreasePayeeAccountDetails.Rows)
                                {
                                    busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
                                    lbusPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrRetireeIncreasePayeeAccountDetails["PAYEE_ACCOUNT_ID"]));

                                    bool IsRollOverInfoExists = false;
                                    lbusPayeeAccount.iclbPayeeAccountRolloverDetail = new Collection<busPayeeAccountRolloverDetail>();
                                    lbusPayeeAccount.LoadPayeeAccountRolloverDetails();
                                    if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail != null && lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Count > 0 &&
                                        lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Where(item => item.icdoPayeeAccountRolloverDetail.status_value == "ACTV").Count() > 0)
                                    {
                                        IsRollOverInfoExists = true;
                                    }


                                    if (Convert.ToDecimal(ldrRetireeIncreasePayeeAccountDetails["Gross_Amount"]) >= 750 && !IsRollOverInfoExists)
                                    {
                                        if (ldrRolloverInfoParameters["option2"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option2"]) == busConstant.YES_CAPS
                                            && ldrRolloverInfoParameters["option1"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option1"]) == busConstant.NO_CAPS)
                                        {
                                            if (lbusPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrRetireeIncreasePayeeAccountDetails["PAYEE_ACCOUNT_ID"])))
                                            {
                                                lbusPayeeAccount.iclbPayeeAccountTaxWithholding = new Collection<busPayeeAccountTaxWithholding>();
                                                lbusPayeeAccount.LoadPayeeAccountTaxWithholdings();
                                                foreach (busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding in lbusPayeeAccount.iclbPayeeAccountTaxWithholding)
                                                {
                                                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date = DateTime.Now;
                                                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.Update();

                                                    lbusPayeeAccountTaxWithholding.iclbPayeeAccountTaxWithholdingItemDetail = new Collection<busPayeeAccountTaxWithholdingItemDetail>();
                                                    lbusPayeeAccountTaxWithholding.LoadPayeeAccountTaxWithholdingItemDetails();

                                                    foreach (busPayeeAccountTaxWithholdingItemDetail lbusPayeeAccountTaxWithholdingItemDetail in lbusPayeeAccountTaxWithholding.iclbPayeeAccountTaxWithholdingItemDetail)
                                                    {
                                                        busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                                                        if (lbusPayeeAccountPaymentItemType.FindPayeeAccountPaymentItemType(lbusPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id))
                                                        {
                                                            lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = DateTime.Now;
                                                            lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
                                                        }
                                                    }
                                                }


                                                lbusPayeeAccount.iclbPayeeAccountAchDetail = new Collection<busPayeeAccountAchDetail>();
                                                lbusPayeeAccount.LoadPayeeAccountAchDetails();
                                                foreach (busPayeeAccountAchDetail lbusPayeeAccountAchDetail in lbusPayeeAccount.iclbPayeeAccountAchDetail)
                                                {
                                                    lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date = DateTime.Now.AddDays(-1);
                                                    lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.Now;
                                                    lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.Update();

                                                }

                                                // Insertig details into Rollover Detail Table
                                                lbusPayeeAccount.iclbPayeeAccountRolloverDetail = null;
                                                cdoPayeeAccountRolloverDetail lcdoPayeeAccountRolloverDetail = new cdoPayeeAccountRolloverDetail();
                                                lcdoPayeeAccountRolloverDetail.payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;

                                                if (ldrRolloverInfoParameters["Checkto"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["Checkto"]).IsNotNullOrEmpty())
                                                {
                                                    DataTable ldtbGetOrgID = busBase.Select("cdoPayeeBenefitAccount.GetOrgNamebyName", new object[1] { Convert.ToString(ldrRolloverInfoParameters["Checkto"]).TrimStart().TrimEnd() });
                                                    if (ldtbGetOrgID != null && ldtbGetOrgID.Rows.Count > 0 && Convert.ToString(ldtbGetOrgID.Rows[0][0]).IsNotNullOrEmpty())
                                                        lcdoPayeeAccountRolloverDetail.rollover_org_id = Convert.ToInt32(ldtbGetOrgID.Rows[0][0]);
                                                    else
                                                    {
                                                        cdoOrganization lcdoOrganization = new cdoOrganization();
                                                        lcdoOrganization.org_name = Convert.ToString(ldrRolloverInfoParameters["Checkto"]).TrimStart().TrimEnd();
                                                        lcdoOrganization.org_type_value = "RLIT";
                                                        lcdoOrganization.payment_type_value = "CHK";
                                                        lcdoOrganization.status_value = "A";

                                                        lcdoOrganization.Insert();

                                                        if (string.IsNullOrEmpty(lcdoOrganization.mpi_org_id))
                                                        {
                                                            //lintCapitalGain.ToString().PadLeft(12, '0');
                                                            cdoCodeValue lobjcdoCodeValue = HelperUtil.GetCodeValueDetails(52, busConstant.MPID);
                                                            int lintNewOrgID = Convert.ToInt32(lobjcdoCodeValue.data1);
                                                            lcdoOrganization.mpi_org_id = "M" + lintNewOrgID.ToString("D8");
                                                            lcdoOrganization.Update();

                                                            lintNewOrgID += 1;
                                                            lobjcdoCodeValue.data1 = lintNewOrgID.ToString();
                                                            lobjcdoCodeValue.Update();

                                                        }


                                                        lcdoPayeeAccountRolloverDetail.rollover_org_id = lcdoOrganization.org_id;
                                                    }
                                                }

                                                lcdoPayeeAccountRolloverDetail.rollover_option_value = busConstant.PayeeAccountRolloverOptionAllOfGross;
                                                lcdoPayeeAccountRolloverDetail.contact_name = ldrRolloverInfoParameters["PrintTile"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["PrintTile"]).IsNotNullOrEmpty() ? Convert.ToString(ldrRolloverInfoParameters["PrintTile"]) : null;
                                                lcdoPayeeAccountRolloverDetail.account_number = ldrRolloverInfoParameters["AcctNo"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["AcctNo"]).IsNotNullOrEmpty() ? Convert.ToString(ldrRolloverInfoParameters["AcctNo"]) : null;
                                                lcdoPayeeAccountRolloverDetail.addr_line_1 = ldrRolloverInfoParameters["ActAddress"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["ActAddress"]).IsNotNullOrEmpty() ? Convert.ToString(ldrRolloverInfoParameters["ActAddress"]) : null;
                                                lcdoPayeeAccountRolloverDetail.state_value = ldrRolloverInfoParameters["ActState"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["ActState"]).IsNotNullOrEmpty() ? Convert.ToString(ldrRolloverInfoParameters["ActState"]) : null;
                                                lcdoPayeeAccountRolloverDetail.country_value = "0001";
                                                lcdoPayeeAccountRolloverDetail.status_value = "ACTV";


                                                if (ldrRolloverInfoParameters["option2A"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option2A"]) == busConstant.YES_CAPS)
                                                    lcdoPayeeAccountRolloverDetail.rollover_type_value = "IS8a";
                                                else if (ldrRolloverInfoParameters["option2B"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option2B"]) == busConstant.YES_CAPS)
                                                    lcdoPayeeAccountRolloverDetail.rollover_type_value = "RS8a";
                                                else if (ldrRolloverInfoParameters["option2C"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option2C"]) == busConstant.YES_CAPS)
                                                    lcdoPayeeAccountRolloverDetail.rollover_type_value = "AS8b";
                                                else if (ldrRolloverInfoParameters["option2D"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option2D"]) == busConstant.YES_CAPS)
                                                    lcdoPayeeAccountRolloverDetail.rollover_type_value = "QS1a";
                                                else if (ldrRolloverInfoParameters["option2E"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option2E"]) == busConstant.YES_CAPS)
                                                    lcdoPayeeAccountRolloverDetail.rollover_type_value = "AS3a";
                                                else if (ldrRolloverInfoParameters["option2F"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option2F"]) == busConstant.YES_CAPS)
                                                    lcdoPayeeAccountRolloverDetail.rollover_type_value = "AS3b";
                                                else if (ldrRolloverInfoParameters["option2G"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option2G"]) == busConstant.YES_CAPS)
                                                    lcdoPayeeAccountRolloverDetail.rollover_type_value = "GS7b";


                                                lcdoPayeeAccountRolloverDetail.city = ldrRolloverInfoParameters["ActCity"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["ActCity"]).IsNotNullOrEmpty() ? Convert.ToString(ldrRolloverInfoParameters["ActCity"]) : null;
                                                lcdoPayeeAccountRolloverDetail.zip_code = ldrRolloverInfoParameters["ActPostalcode"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["ActPostalcode"]).IsNotNullOrEmpty() ? Convert.ToString(ldrRolloverInfoParameters["ActPostalcode"]).Substring(0, 5) : null;
                                                lcdoPayeeAccountRolloverDetail.zip_4_code = ldrRolloverInfoParameters["ActPostalcode"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["ActPostalcode"]).IsNotNullOrEmpty() && Convert.ToString(ldrRolloverInfoParameters["ActPostalcode"]).Length == 9 ? Convert.ToString(ldrRolloverInfoParameters["ActPostalcode"]).Substring(5, 4) : null;
                                                lcdoPayeeAccountRolloverDetail.Insert();

                                                //Process Rollover Details will push entries in Rollover item deatail and payee account payment type when Amount is Greater than 0
                                                lbusPayeeAccount.ProcessRolloverDetails();

                                                lbusPayeeAccount.CreateReviewPayeeAccountStatus();


                                                int lintTrackingNo = 0;
                                                if ((ldrRolloverInfoParameters["BarCode"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["BarCode"]).IsNotNullOrEmpty()))
                                                {
                                                    lintTrackingNo = Convert.ToInt32(ldrRolloverInfoParameters["BarCode"]);
                                                }

                                                SqlParameter[] RolloverInfoparam = new SqlParameter[2];
                                                SqlParameter RolloverInfoparam1 = new SqlParameter("@DocID", DbType.Int32);

                                                RolloverInfoparam1.Value = lintTrackingNo;
                                                RolloverInfoparam[0] = RolloverInfoparam1;

                                                SqlParameter RolloverInfoparam2 = new SqlParameter("@ProcessValue", DbType.String);

                                                if (ldrRolloverInfoParameters["option2"] != DBNull.Value && ldrRolloverInfoParameters["option1"] != DBNull.Value &&
                                                             Convert.ToString(ldrRolloverInfoParameters["option1"]) == Convert.ToString(ldrRolloverInfoParameters["option2"])
                                                    && ldrRolloverInfoParameters["option2"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option2"]) == busConstant.NO_CAPS)
                                                {
                                                    RolloverInfoparam2.Value = "S";
                                                }
                                                else
                                                {
                                                    RolloverInfoparam2.Value = "C";
                                                }
                                                RolloverInfoparam[1] = RolloverInfoparam2;

                                                busGlobalFunctions.ExecuteSPtoGetDataTable("usp_opus_retiree_increase_processed_upd", astrLookupDBConnection, null, RolloverInfoparam);

                                                lblnFlag = true;

                                            }
                                        }

                                    }


                                    bool IsStateTaxInfoExists = false;
                                    lbusPayeeAccount.iclbPayeeAccountTaxWithholding = new Collection<busPayeeAccountTaxWithholding>();
                                    lbusPayeeAccount.LoadPayeeAccountTaxWithholdings();
                                    if (lbusPayeeAccount.iclbPayeeAccountTaxWithholding != null && lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Count > 0 &&
                                        lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Where(item => item.icdoPayeeAccountTaxWithholding.tax_identifier_value == "STAT" && item.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue).Count() > 0)
                                    {
                                        IsStateTaxInfoExists = true;
                                    }


                                    if (lbusPayeeAccount.IsNotNull() && (ldrRolloverInfoParameters["SectionC1"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["SectionC1"]) == busConstant.YES_CAPS)
                                       && ldrRolloverInfoParameters["option2"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option2"]) == busConstant.NO_CAPS && !IsStateTaxInfoExists)
                                    {
                                        lbusPayeeAccount.iclbPayeeAccountTaxWithholding = null;
                                        lbusPayeeAccount.LoadNextBenefitPaymentDate();
                                        busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
                                        lbusPayeeAccountTaxWithholding.InsertValuesInTaxWithHolding(lbusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.CA_STATE_TAX,
                                        busConstant.Benefit_Distribution_Type_Monthly_Benefit, DateTime.Now, DateTime.MinValue, busConstant.FLAT_PERCENT, 0, "M", 0, 2);
                                        lbusPayeeAccount.ProcessTaxWithHoldingDetails();
                                        //PROD PIR 796 
                                        //lbusPayeeAccount.CreateReviewPayeeAccountStatus();


                                        int lintTrackingNo = 0;
                                        if ((ldrRolloverInfoParameters["BarCode"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["BarCode"]).IsNotNullOrEmpty()))
                                        {
                                            lintTrackingNo = Convert.ToInt32(ldrRolloverInfoParameters["BarCode"]);
                                        }

                                        SqlParameter[] RolloverInfoparam = new SqlParameter[2];
                                        SqlParameter RolloverInfoparam1 = new SqlParameter("@DocID", DbType.Int32);

                                        RolloverInfoparam1.Value = lintTrackingNo;
                                        RolloverInfoparam[0] = RolloverInfoparam1;

                                        SqlParameter RolloverInfoparam2 = new SqlParameter("@ProcessValue", DbType.String);

                                        if (ldrRolloverInfoParameters["option2"] != DBNull.Value && ldrRolloverInfoParameters["option1"] != DBNull.Value &&
                                                     Convert.ToString(ldrRolloverInfoParameters["option1"]) == Convert.ToString(ldrRolloverInfoParameters["option2"])
                                            && ldrRolloverInfoParameters["option2"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["option2"]) == busConstant.NO_CAPS)
                                        {
                                            RolloverInfoparam2.Value = "S";
                                        }
                                        else
                                        {
                                            RolloverInfoparam2.Value = "C";
                                        }
                                        RolloverInfoparam[1] = RolloverInfoparam2;

                                        busGlobalFunctions.ExecuteSPtoGetDataTable("usp_opus_retiree_increase_processed_upd", astrLookupDBConnection, null, RolloverInfoparam);

                                        lblnFlag = true;
                                    }
                                }
                            }

                            //Rohan 10212014
                            if (!lblnFlag)
                            {
                                int lintTrackingNo = 0;
                                if ((ldrRolloverInfoParameters["BarCode"] != DBNull.Value && Convert.ToString(ldrRolloverInfoParameters["BarCode"]).IsNotNullOrEmpty()))
                                {
                                    lintTrackingNo = Convert.ToInt32(ldrRolloverInfoParameters["BarCode"]);
                                }

                                SqlParameter[] RolloverInfoparam = new SqlParameter[2];
                                SqlParameter RolloverInfoparam1 = new SqlParameter("@DocID", DbType.Int32);

                                RolloverInfoparam1.Value = lintTrackingNo;
                                RolloverInfoparam[0] = RolloverInfoparam1;

                                SqlParameter RolloverInfoparam2 = new SqlParameter("@ProcessValue", DbType.String);
                                RolloverInfoparam2.Value = DBNull.Value;

                                RolloverInfoparam[1] = RolloverInfoparam2;

                                busGlobalFunctions.ExecuteSPtoGetDataTable("usp_opus_retiree_increase_processed_upd", astrLookupDBConnection, null, RolloverInfoparam);
                            }
                        }
                        catch
                        {
                            PostErrorMessage("Error Occured for : " + Convert.ToString(ldrRolloverInfoParameters["PID"]));
                        }

                    }
                }
            }
        }

        #endregion

    }
}
