using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPIPHPJobService;
using System.Data;
using System.Collections.ObjectModel;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using MPIPHP.CustomDataObjects;
using Sagitec.DBUtility;
using Microsoft.Reporting.WinForms;
using System.IO;
using MPIPHP.BusinessTier;
using Sagitec.Common;
using System.Threading.Tasks;
using Sagitec.ExceptionPub;

namespace MPIPHPJobService
{
    public class busBenefitApplicationBatch : busBatchHandler
    {

        #region Properties
        DateTime idtFromDate { get; set; }
        DateTime idtToDate { get; set; }
        string istrLastBisYears = string.Empty;
        private object iobjLock = null;
        #endregion

        public void ReCalculateRetirementBenefitBatch()
        {
            busBase lobjBase = new busBase();

            RetrieveBatchParameters();
            //For Retirement
            DataTable ldtbResult = busBase.Select("cdoPayeeAccount.RecalculateRetirementBenefitsBatch", new object[2] { idtFromDate, idtToDate });
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
            lclbPayeeAccount = lobjBase.GetCollection<busPayeeAccount>(ldtbResult, "icdoPayeeAccount");

            DataTable ldtbReportTable01 = new DataTable();
            ldtbReportTable01.TableName = "ReportTable01";

            //Required Columns in report
            ldtbReportTable01.Columns.Add("PARTICIPANT_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("PARTICIPANT_MPI_PERSON_ID", typeof(string));
            ldtbReportTable01.Columns.Add("PAYEE_MPI_PERSON_ID", typeof(string));
            ldtbReportTable01.Columns.Add("PAYEE_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_TYPE", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_SUB_TYPE", typeof(string));
            ldtbReportTable01.Columns.Add("BREAK_IN_SERVICE", typeof(string));
            ldtbReportTable01.Columns.Add("LAST_BREAK_IN_SERVICE", typeof(string));
            ldtbReportTable01.Columns.Add("RETIREMENT_DATE", typeof(DateTime));
            ldtbReportTable01.Columns.Add("PLAN_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("RETIREMENT_BENEFIT_AMOUNT", typeof(decimal));
            ldtbReportTable01.Columns.Add("RECALCULATED_RETIREMENT_BENEFIT_AMOUNT", typeof(decimal));
            ldtbReportTable01.Columns.Add("PAYEE_ACCOUNT_ID", typeof(int));
            ldtbReportTable01.Columns.Add("COMMENT", typeof(string));
            ldtbReportTable01.Columns.Add("FROM_DATE", typeof(DateTime));
            ldtbReportTable01.Columns.Add("TO_DATE", typeof(DateTime));
            ldtbReportTable01.Columns.Add("CALCULATION_ID", typeof(Int32));
            ldtbReportTable01.Columns.Add("CALCULATION_STATUS", typeof(string));
            ldtbReportTable01.Columns.Add("INCREASE_ELIGIBLE", typeof(string));
            ldtbReportTable01.Columns.Add("REEMPLOYED", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_OPTION", typeof(string));
            ldtbReportTable01.Columns.Add("STATUS_VALUE", typeof(string));
            ldtbReportTable01.Columns.Add("SUSPENSION_STATUS_REASON_VALUE", typeof(string));
            ldtbReportTable01.Columns.Add("AGE", typeof(decimal));

            RetrieveBatchParameters();

            foreach (busPayeeAccount lbusPayeeAccount in lclbPayeeAccount)
            {
                ReCalculateRetirementBenefit(lbusPayeeAccount, ldtbReportTable01);
            }

            if (ldtbReportTable01 != null && ldtbReportTable01.Rows.Count > 0)
                CreateExcelReport(ldtbReportTable01, "rptRecalculateRetirementBenefit");

        }

        public void Approve10PercentIncreasePayeeAccount()
        {
            busBase lobjBase = new busBase();
            DataTable ldtbResult = busBase.Select("cdoPayeeAccount.Approve10PercentIncreasePayeeAccount", new object[0]);
            ArrayList aarrResult = new ArrayList();
            Hashtable ahtbQueryBkmarks = new Hashtable();

            #region Initialize report columns 
            DataTable ldtbReportTable01 = new DataTable();
            ldtbReportTable01.TableName = "ReportTable01";

            //Required Columns in report
            ldtbReportTable01.Columns.Add("PARTICIPANT_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("PARTICIPANT_MPI_PERSON_ID", typeof(string));
            ldtbReportTable01.Columns.Add("PAYEE_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("PAYEE_MPI_PERSON_ID", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_TYPE", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_SUB_TYPE", typeof(string));
            ldtbReportTable01.Columns.Add("PLAN_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_OPTION", typeof(string));
            ldtbReportTable01.Columns.Add("BREAK_IN_SERVICE", typeof(string));
            ldtbReportTable01.Columns.Add("BREAK_IN_SERVICE_YEAR", typeof(string));
            ldtbReportTable01.Columns.Add("RETIREMENT_DATE", typeof(DateTime));
            ldtbReportTable01.Columns.Add("RECALCULATED_RETIREMENT_BENEFIT_AMOUNT", typeof(decimal));
            ldtbReportTable01.Columns.Add("RETIREMENT_BENEFIT_AMOUNT", typeof(decimal));
            ldtbReportTable01.Columns.Add("RETRO_PAYMENT_MONTHS", typeof(decimal));
            ldtbReportTable01.Columns.Add("RECALCULATED_RETIREMENT_BENEFIT_DIFF", typeof(decimal));
            ldtbReportTable01.Columns.Add("TOTAL_RETRO_PAYMENT", typeof(decimal));
            ldtbReportTable01.Columns.Add("STATUS_VALUE", typeof(string));
            ldtbReportTable01.Columns.Add("REASON_NOT_APPROVED", typeof(string));
            ldtbReportTable01.Columns.Add("PAYMENT_DIRECTIVE_DATE", typeof(string));
            ldtbReportTable01.Columns.Add("PAYMENT_CONFIRMATION_LETTER_DATE", typeof(string));
            ldtbReportTable01.Columns.Add("PAYEE_ACCOUNT_ID", typeof(int));
            #endregion

            int lintCount = 0;
            int lintTotalCount = 0;

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();

            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 1; // System.Environment.ProcessorCount * 1;

            Parallel.ForEach(ldtbResult.AsEnumerable(), po, (acdoPerson, loopState) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "Approve10PercentIncreasePayeeAccountBatch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;

                #region Approve 10 Percent Increase Payee Account
                lock (iobjLock)
                {
                    lintCount++;
                    lintTotalCount++;
                    if (lintCount == 100)
                    {
                        String lstrMsg = lintTotalCount + " : " + " Records Has Been Processed";
                        PostInfoMessage(lstrMsg);
                        lintCount = 0;
                    }
                }

                int lpayeeAccountId = Convert.ToInt32(acdoPerson["PAYEE_ACCOUNT_ID"]);

                iobjPassInfo.BeginTransaction();
                try
                {
                    srvPayeeAccount lsrvPayeeAccount = new srvPayeeAccount();
                    lsrvPayeeAccount.iobjPassInfo = this.iobjPassInfo;
                    busPayeeAccount lbusPayeeAccount = lsrvPayeeAccount.FindPayeeAccount(lpayeeAccountId, ablnPaymentDirective: true);
                    lbusPayeeAccount.istrModifiedBy = iobjPassInfo.istrUserID;
                    lbusPayeeAccount.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(DateTime.Now);

                    ArrayList larrErrors = new ArrayList();
                    busPayeeAccountStatus lbusPayeeAccountStatus = new busPayeeAccountStatus();
                    lbusPayeeAccount.LoadPayeeAccountStatuss();
                    larrErrors = CheckErrorBeforeApprove(lbusPayeeAccount as busPayeeAccount, ref larrErrors);//(lbusPayeeAccount as busPayeeAccount, ahstParams, ref larrErrors);  

                    if (larrErrors.Count == 0)
                    {
                        //Generate a payment directive.  Don't do anything with the byte[] variable. This code will run in batch mode only.
                        lbusPayeeAccount.GeneratePaymentDirectiveForBatch();

                        //Update Payment Directive. Set payee_account_status column to "Approved". 
                        if (lbusPayeeAccount.iclbPayeeAccountStatus != null && lbusPayeeAccount.iclbPayeeAccountStatus.Count > 0
                            && lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(item => item.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_value == busConstant.PAYEE_ACCOUNT_STATUS_REVIEW)
                        {
                            if (lbusPayeeAccount.iclbPaymentDirectives != null && lbusPayeeAccount.iclbPaymentDirectives.Where(item => item.icdoPaymentDirectives.payment_cycle_date >= lbusPayeeAccount.idtNextBenefitPaymentDate).Count() > 0)
                            {
                                lbusPayeeAccount.iclbPaymentDirectives.Where(item => item.icdoPaymentDirectives.payment_cycle_date >= lbusPayeeAccount.idtNextBenefitPaymentDate).FirstOrDefault().icdoPaymentDirectives.modified_by
                                    = iobjPassInfo.istrUserID;
                                lbusPayeeAccount.iclbPaymentDirectives.Where(item => item.icdoPaymentDirectives.payment_cycle_date >= lbusPayeeAccount.idtNextBenefitPaymentDate).FirstOrDefault().icdoPaymentDirectives.modified_date
                                    = DateTime.Now;
                                lbusPayeeAccount.iclbPaymentDirectives.Where(item => item.icdoPaymentDirectives.payment_cycle_date >= lbusPayeeAccount.idtNextBenefitPaymentDate).FirstOrDefault().icdoPaymentDirectives.approved_by
                                    = iobjPassInfo.istrUserID;
                                lbusPayeeAccount.iclbPaymentDirectives.Where(item => item.icdoPaymentDirectives.payment_cycle_date >= lbusPayeeAccount.idtNextBenefitPaymentDate).FirstOrDefault().icdoPaymentDirectives.approved_date
                                    = DateTime.Now;
                                lbusPayeeAccount.iclbPaymentDirectives.Where(item => item.icdoPaymentDirectives.payment_cycle_date >= lbusPayeeAccount.idtNextBenefitPaymentDate).FirstOrDefault().icdoPaymentDirectives.payee_account_status
                                    = "Approved";
                                lbusPayeeAccount.iclbPaymentDirectives.Where(item => item.icdoPaymentDirectives.payment_cycle_date >= lbusPayeeAccount.idtNextBenefitPaymentDate).FirstOrDefault().icdoPaymentDirectives.Update();
                            }
                        }

                        //Approve Payee Account
                        lbusPayeeAccount.CreateApprovedPayeeAccountStatus(iobjPassInfo.istrUserID);

                        //Confirm ID dictionary parameter and save
                        lobjPassInfo.idictParams["ID"] = "Approve10PercentIncreasePayeeAccountBatch";
                        utlPassInfo.iobjPassInfo = lobjPassInfo;

                        //Generate Confirmation Letters, as long as Benefit Opion is not a Lump Sum
                        if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue != busConstant.LUMP_SUM)
                        {
                            this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE; 
                            aarrResult.Add(lbusPayeeAccount);
                            this.CreateCorrespondence(busConstant.RETROACTIVE_BENEFIT_INCREASE_NOTICE, lobjPassInfo.istrUserID, lobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, ablnIsPDF: true);
                            aarrResult.Clear();
                        }
                    }
                    //Populate report column
                    Approve10PercentIncreaseReport(lbusPayeeAccount, ldtbReportTable01, larrErrors);
                    iobjPassInfo.Commit();

                }
                catch (Exception e)
                {
                    lock (iobjLock)
                    {
                        ExceptionManager.Publish(e);
                        String lstrMsg = "Error while Executing Batch, Error Message For Payee Account ID " + lpayeeAccountId.ToString() + ":" + e.ToString();
                        PostErrorMessage(lstrMsg);
                    }
                    iobjPassInfo.Rollback();
                }
                #endregion

                //Clean up
                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;
            });

            //Create Report 
            if (ldtbReportTable01 != null && ldtbReportTable01.Rows.Count > 0)
                CreateExcelReport(ldtbReportTable01, "rptApprove10PercentIncrease");

            //Merge PDF letters into a single PDF (1 Domestic file, 1 International file)
            MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Correspondence_Path,
                           iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_Approve10PercentIncrease, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE, true);

            lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        }

        public ArrayList CheckErrorBeforeApprove(object aobj, ref ArrayList aarrErrors, bool ablnHardError = false)
        {
            utlError lobjError = null;
            //NEW
            busMainBase lbusMainBase = new busMainBase();
            string astrPrimaryFlag = string.Empty;
            int RollOverCount;
            DateTime idtDateTime;
            int iintTaxwithholdingCount = 0;
            busPayeeAccount lbusPayeeAccount = aobj as busPayeeAccount;
            busPayeeAccountStatus lbusPayeeAccountStatus = null;
            if (!lbusPayeeAccount.iclbPayeeAccountStatus.IsNullOrEmpty())
            {
                lbusPayeeAccountStatus = (from item in lbusPayeeAccount.iclbPayeeAccountStatus
                                          orderby item.icdoPayeeAccountStatus.status_effective_date descending
                                          select item).FirstOrDefault();
            }

            decimal ldecNextGrossPaymentACH = lbusPayeeAccount.idecNextGrossPaymentACH == decimal.Zero ? lbusPayeeAccount.idecPaidGrossAmount : lbusPayeeAccount.idecNextGrossPaymentACH;
            decimal ldecRetroAdjustmentAmount = lbusPayeeAccount.idecRetroAdjustmentAmount;
            if(lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue == busConstant.LUMP_SUM)
            {
                lobjError = lbusMainBase.AddError(6308, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            if (ldecRetroAdjustmentAmount + ldecNextGrossPaymentACH > 15000)
            {
                lobjError = lbusMainBase.AddError(6306, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }
            if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue != busConstant.LUMP_SUM
                && ldecRetroAdjustmentAmount == 0)
            {
                lobjError = lbusMainBase.AddError(6307, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            string istrScheduleType = string.Empty;
            if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId != 1)
                istrScheduleType = busConstant.PaymentScheduleTypeMonthly;
            else
                istrScheduleType = busConstant.PaymentScheduleTypeWeekly;

            #region PROD PIR 148
            if (lbusPayeeAccount.ibusPayee.IsNotNull()
                && lbusPayeeAccount.ibusPayee.icdoPerson.ssn.IsNullOrEmpty())
            {
                lobjError = lbusMainBase.AddError(6222, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }
            if (lbusPayeeAccount.ibusPayee.IsNotNull()
                && (lbusPayeeAccount.ibusPayee.icdoPerson.date_of_birth.IsNull() || lbusPayeeAccount.ibusPayee.icdoPerson.date_of_birth == DateTime.MinValue))
            {
                lobjError = lbusMainBase.AddError(6222, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }
            #endregion

            //Payment Adjustment BR-023-46
            if (lbusPayeeAccount.iclbPayeeAccountBenefitOverpayment != null && lbusPayeeAccount.iclbPayeeAccountBenefitOverpayment.Count > 0)
            {
                lbusPayeeAccount.LoadNextBenefitPaymentDate();
                DataTable ldtblBenefitAmount = busBase.Select("cdoRepaymentSchedule.GetBenefitAmount", new object[2] { lbusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                    lbusPayeeAccount.idtNextBenefitPaymentDate });

                if (ldtblBenefitAmount.Rows.Count > 0)
                {
                    foreach (busPayeeAccountBenefitOverpayment lbusPayeeAccountBenefitOverpayment in lbusPayeeAccount.iclbPayeeAccountBenefitOverpayment)
                    {
                        DataTable ldtblNextAmountDue = busBase.Select("cdoRepaymentSchedule.GetNextAmountDue", new object[1] { lbusPayeeAccountBenefitOverpayment.icdoPayeeAccountRetroPayment.payee_account_retro_payment_id });
                        if (ldtblNextAmountDue != null && ldtblNextAmountDue.Rows.Count > 0 && Convert.ToString(ldtblNextAmountDue.Rows[0][0]).IsNotNullOrEmpty())
                        {
                            if (Convert.ToDecimal(ldtblNextAmountDue.Rows[0][0]) > Convert.ToDecimal(ldtblBenefitAmount.Rows[0][0]))
                            {
                                lobjError = lbusMainBase.AddError(6106, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                        }
                    }
                }
            }


            //Disability Re-certification 
            //If user is trying to approve a payee account, where the disability recert date
            //is sent but no received date is there, then throw an error saying that Payee Account cannot be approved as Disability needs to be re-certified.
            if (lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
            {
                DataTable ldtblDisabilityBenefitHistory = busBase.Select("cdoPayeeAccountStatus.CheckDisabilityStatusHistory", new object[1] { lbusPayeeAccount.icdoPayeeAccount.benefit_application_detail_id });
                if (ldtblDisabilityBenefitHistory != null && ldtblDisabilityBenefitHistory.Rows.Count > 0)
                {
                    //PIR RID 70576 Added code to find payee age and only raise error if less than 65.
                    busPerson lbusPerson = new busPerson();
                    lbusPerson.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id);
                    decimal ldecAge = busGlobalFunctions.CalculatePersonAge(lbusPerson.icdoPerson.idtDateofBirth, DateTime.Now);
                    if (Convert.ToString(ldtblDisabilityBenefitHistory.Rows[0][enmDisabilityBenefitHistory.sent.ToString().ToUpper()]).IsNotNullOrEmpty()
                        && Convert.ToString(ldtblDisabilityBenefitHistory.Rows[0][enmDisabilityBenefitHistory.sent.ToString().ToUpper()]) == busConstant.FLAG_YES
                        && ldecAge < 65
                        )
                    {
                        lobjError = lbusMainBase.AddError(6137, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }
            }

            if (lbusPayeeAccount.iclbPayeeAccountStatus.IsNotNull())
            {
                if (lbusPayeeAccountStatus != null && lbusPayeeAccountStatus.icdoPayeeAccountStatus.status_value == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED)
                {
                    lobjError = lbusMainBase.AddError(6080, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }
            if (lbusPayeeAccount.icdoPayeeAccount.reemployed_flag == busConstant.FLAG_YES && lbusPayeeAccountStatus.icdoPayeeAccountStatus.status_value == busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED)
            {
                lobjError = lbusMainBase.AddError(6087, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;

            }


            int iintRolloverCount = 0;
            if (lbusPayeeAccount.iclbPayeeAccountTaxWithholding == null)
                lbusPayeeAccount.LoadPayeeAccountTaxWithholdings();

            // PROD PIR 518
            if (lbusPayeeAccount.iclbActiveRolloverDetails == null)
                lbusPayeeAccount.LoadActiveRolloverDetail();
            if (lbusPayeeAccount.iclbActiveRolloverDetails.Count() > 0)
            {
                iintRolloverCount = lbusPayeeAccount.iclbActiveRolloverDetails.Where(item => item.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfGross
                    || item.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfTaxable).Count();
            }

            iintTaxwithholdingCount = lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Count();
            if (iintTaxwithholdingCount == 0 && iintRolloverCount == 0 && lbusPayeeAccount.icdoPayeeAccount.transfer_org_id == 0)
            {
                lobjError = lbusMainBase.AddError(6034, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }
            //PIR-810
            if (lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY
                && lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT
                && lbusPayeeAccount.icdoPayeeAccount.family_relation_value != busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE
                && lbusPayeeAccount.icdoPayeeAccount.org_id != 0)
            {
                if (lbusPayeeAccount.ibusOrganization != null
                    && string.IsNullOrEmpty(lbusPayeeAccount.ibusOrganization.icdoOrganization.federal_id))
                {
                    lobjError = lbusMainBase.AddError(6234, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }

            //ask to abhishek
            lbusPayeeAccount.LoadBreakDownDetails();
            decimal idecTotalNetPaymentACH = lbusPayeeAccount.idecNextGrossPaymentACH + lbusPayeeAccount.idecRetroAdjustmentAmount + lbusPayeeAccount.idecNextGrossPaymentRollOver; //PROD PIR 389
            if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
            {
                if (idecTotalNetPaymentACH >= (Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(7034, busConstant.PensionMonthlyLimit)))
                    && (lbusPayeeAccount.icdoPayeeAccount.verified_flag.IsNullOrEmpty() || lbusPayeeAccount.icdoPayeeAccount.verified_flag == busConstant.FLAG_NO))
                {
                    lobjError = lbusMainBase.AddError(6036, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }
            else
            {
                if (idecTotalNetPaymentACH >= (Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(7034, busConstant.IAPMonthlyLimit)))
                    && (lbusPayeeAccount.icdoPayeeAccount.verified_flag.IsNullOrEmpty() || lbusPayeeAccount.icdoPayeeAccount.verified_flag == busConstant.FLAG_NO))
                {
                    lobjError = lbusMainBase.AddError(6037, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }
            if (lbusPayeeAccount.iclbPayeeAccountAchDetail != null)
                lbusPayeeAccount.LoadPayeeAccountAchDetails();
            int ValAchCount = lbusPayeeAccount.iclbPayeeAccountAchDetail.Where(item => busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                                                    item.icdoPayeeAccountAchDetail.ach_start_date, item.icdoPayeeAccountAchDetail.ach_end_date) && item.icdoPayeeAccountAchDetail.istrPreNoteFlag != "Y").Count();
            if (ValAchCount == 0)
            {
                Boolean iboolMailAddspresent = false;
                if (lbusPayeeAccount.ibusPayee != null)
                {
                    lbusPayeeAccount.ibusPayee.LoadPersonAddresss();
                    foreach (busPersonAddress lbusPersonAddress in lbusPayeeAccount.ibusPayee.iclbPersonAddress)
                    {
                        if ((lbusPersonAddress.icdoPersonAddress.end_date > DateTime.Now || lbusPersonAddress.icdoPersonAddress.end_date == DateTime.MinValue
                            || lbusPersonAddress.icdoPersonAddress.end_date.ToShortDateString() == DateTime.Now.ToShortDateString()) && // PROD PIR 329
                            lbusPersonAddress.icdoPersonAddress.start_date <= DateTime.Now && lbusPersonAddress.icdoPersonAddress.start_date != lbusPersonAddress.icdoPersonAddress.end_date)
                        {
                            foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in lbusPersonAddress.iclcPersonAddressChklist)
                            {
                                if (lcdoPersonAddressChklist.address_type_value == busConstant.MAILING_ADDRESS || lcdoPersonAddressChklist.address_type_value == busConstant.PHYSICAL_AND_MAILING_ADDRESS)
                                {
                                    iboolMailAddspresent = true;
                                }
                            }
                        }
                    }
                }
                else if (lbusPayeeAccount.ibusOrganization != null)
                {
                    lbusPayeeAccount.ibusOrganization.LoadOrgAddresss();
                    foreach (busOrgAddress lbusOrgAddress in lbusPayeeAccount.ibusOrganization.iclbOrgAddress)
                    {
                        if ((lbusOrgAddress.icdoOrgAddress.end_date >= DateTime.Now || lbusOrgAddress.icdoOrgAddress.end_date == DateTime.MinValue
                            || lbusOrgAddress.icdoOrgAddress.end_date.ToShortDateString() == DateTime.Now.ToShortDateString()) && // PROD PIR 329
                            lbusOrgAddress.icdoOrgAddress.start_date <= DateTime.Now && lbusOrgAddress.icdoOrgAddress.start_date != lbusOrgAddress.icdoOrgAddress.end_date)
                        {
                            if (lbusOrgAddress.icdoOrgAddress.address_type_value == busConstant.MAILING_ADDRESS || lbusOrgAddress.icdoOrgAddress.address_type_value == busConstant.PHYSICAL_AND_MAILING_ADDRESS)
                                iboolMailAddspresent = true;
                        }
                    }
                }

                //rohan 10212014 PIR 803
                bool lblnFlag = true;
                if (lbusPayeeAccount.idecNextNetPaymentACH == 0 && lbusPayeeAccount.iclbPayeeAccountRolloverDetail != null &&
                    lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Count > 0 && lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Where(t => t.icdoPayeeAccountRolloverDetail.status_value == "ACTV").Count() > 0)
                {
                    lblnFlag = false;
                }

                if (!iboolMailAddspresent && lblnFlag)
                {
                    lobjError = lbusMainBase.AddError(6216, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }



            }
            if (lbusPayeeAccount.iclbPayeeAccountDeduction == null)
                lbusPayeeAccount.LoadPayeeAccountDeduction();
            lbusPayeeAccount.iclbPayeeAccountDeductionActive = (from item in lbusPayeeAccount.iclbPayeeAccountDeduction
                                                                where busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                                                item.icdoPayeeAccountDeduction.start_date, item.icdoPayeeAccountDeduction.end_date)
                                                                select item).ToList().ToCollection<busPayeeAccountDeduction>();
            if (lbusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                lbusPayeeAccount.LoadNextBenefitPaymentDate();
            foreach (busPayeeAccountDeduction lbusPayeeAccountDeduction in lbusPayeeAccount.iclbPayeeAccountDeductionActive)
            {
                if (!lbusPayeeAccountDeduction.CheckMailAddress(lbusPayeeAccount.idtNextBenefitPaymentDate))
                {
                    lobjError = lbusMainBase.AddError(6217, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }

            return aarrErrors;
        }

        public void ReCalculateRetirementBenefit(busPayeeAccount abusPayeeAccount, DataTable ldtbReportTable01)
        {
            DataRow dr = ldtbReportTable01.NewRow();


            LoadPayeeAccountDetails(ref abusPayeeAccount);

            decimal lWithHoldingPercentage = abusPayeeAccount.iclbWithholdingInformation.Where(i => i.icdoWithholdingInformation.withholding_date_to == null || i.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue).Select(y => y.icdoWithholdingInformation.withholding_percentage).FirstOrDefault(); 

            decimal ldecIncreaseYear = 0;
            decimal ldecBenefit_Amount = 0;
            decimal ldecthrushHoldBenefit_Amount = 0;

            DataTable ldtbPlanBenefitRate = busBase.Select("cdoPlanBenefitRate.Lookup", new object[] { });
            Collection<cdoPlanBenefitRate> lclbcdoPlanBenefitRate = cdoDummyWorkData.GetCollection<cdoPlanBenefitRate>(ldtbPlanBenefitRate);

            if (lclbcdoPlanBenefitRate != null && lclbcdoPlanBenefitRate.Count > 0 && lclbcdoPlanBenefitRate.Where(t => t.rate_type_value == busConstant.BenefitCalculation.PLAN_B && t.increase_percentage > 0).Count() > 0)
            {
                ldecIncreaseYear = lclbcdoPlanBenefitRate.Where(t => t.rate_type_value == busConstant.BenefitCalculation.PLAN_B && t.increase_percentage > 0).OrderByDescending(t => t.plan_year).FirstOrDefault().plan_year;
            }
            //          
            dr["PARTICIPANT_NAME"] = abusPayeeAccount.ibusParticipant.icdoPerson.first_name + " " + abusPayeeAccount.ibusParticipant.icdoPerson.last_name;
            dr["PAYEE_MPI_PERSON_ID"] = abusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id;
            dr["PAYEE_NAME"] = abusPayeeAccount.ibusPayee.icdoPerson.first_name + " " + abusPayeeAccount.ibusPayee.icdoPerson.last_name;
            dr["PARTICIPANT_MPI_PERSON_ID"] = abusPayeeAccount.ibusParticipant.icdoPerson.mpi_person_id;
            dr["PLAN_NAME"] = abusPayeeAccount.icdoPayeeAccount.istrPlanCode;
            dr["RETIREMENT_BENEFIT_AMOUNT"] = abusPayeeAccount.idecNextGrossPaymentACH == decimal.Zero ? abusPayeeAccount.idecPaidGrossAmount : abusPayeeAccount.idecNextGrossPaymentACH;
            dr["BENEFIT_TYPE"] = abusPayeeAccount.icdoPayeeAccount.benefit_account_type_description;
            dr["PAYEE_ACCOUNT_ID"] = abusPayeeAccount.icdoPayeeAccount.payee_account_id;
            dr["BENEFIT_SUB_TYPE"] = abusPayeeAccount.icdoPayeeAccount.retirement_type_description;
            dr["RETIREMENT_DATE"] = abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate;
            dr["BENEFIT_OPTION"] = abusPayeeAccount.icdoPayeeAccount.istrBenefitOption;
            dr["FROM_DATE"] = idtFromDate;
            dr["TO_DATE"] = idtToDate;
            dr["REEMPLOYED"] = string.Empty;
            dr["STATUS_VALUE"] = abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_description;
            if (abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value.IsNotNullOrEmpty())
            {
                dr["SUSPENSION_STATUS_REASON_VALUE"] = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Payee_Account_Suspension_Reason_id,
                        abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value).description;
            }
            else
                dr["SUSPENSION_STATUS_REASON_VALUE"] = string.Empty;

            dr["AGE"] = Math.Round(busGlobalFunctions.CalculatePersonAge(abusPayeeAccount.ibusParticipant.icdoPerson.date_of_birth, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate), 2);
            dr["COMMENT"] = string.Empty;
            if(lWithHoldingPercentage > 0.0m)
            {
                dr["COMMENT"] = "Withholding % " + Convert.ToString(lWithHoldingPercentage) +" ";
            }
            try
            {
                int lintParticipantHasPendingDRO = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.CheckIfParticipantHasPendingDRO", new object[1] { abusPayeeAccount.icdoPayeeAccount.person_id },

                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                int lintParticipantDateOfDeath = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.CheckIfParticipantIsDead", new object[1] { abusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                int lintPendingAdjCalcCount = (int)DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.GetCountofPendingAdjCalc",
                                                                                new object[3] { abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value, abusPayeeAccount.icdoPayeeAccount.iintPlanId },
                                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if (lintParticipantHasPendingDRO > 0 && abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.BENEFIT_TYPE_QDRO)
                {

                    dr["COMMENT"] += "QDRO Payee Account re-calculation must me be processed first.";
                    ldtbReportTable01.Rows.Add(dr);
                    return;
                }

                else if (lintPendingAdjCalcCount > 0)
                {
                    DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.CancelPendingAdjustmentCalculations",
                                                                                new object[3] { abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value, abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.iintPlanId },
                                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }

                DateTime ldtLastPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate(abusPayeeAccount.icdoPayeeAccount.iintPlanId);

                if (abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
                {
                    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                    lbusPlanBenefitXr.FindPlanBenefitXr(abusPayeeAccount.icdoPayeeAccount.plan_benefit_id);
                    string lstrBenefitOption = lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;

                    DataTable ldtbList = busBase.Select("cdoDroApplication.LoadAllExistingQDRO", new object[1] { abusPayeeAccount.icdoPayeeAccount.person_id });

                    if (ldtbList != null && ldtbList.Rows.Count > 0)
                    {
                        dr["COMMENT"] += "QDRO On File.";
                        ldtbReportTable01.Rows.Add(dr);
                        return;
                    }

                    busBenefitCalculationRetirement lbusBenefitCalculationHeader = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    lbusBenefitCalculationHeader = abusPayeeAccount.ReCalculateBenefitForRetirement(lstrBenefitOption, ablnPostRetDeath: true);//ablnPostRetDeath it needs to be true, else it will attach the calculation with the payee account if there are hours after retirement.
                    DateTime ldtVestedDt = new DateTime();
                    DataTable ldtGetVestedDate = busBase.Select("cdoPersonAccountEligibility.GetVestedDateForMD", new object[1] { lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.person_id });
                    if (ldtGetVestedDate != null && ldtGetVestedDate.Rows.Count > 0 && (Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]) != DateTime.MinValue))
                    {
                        ldtVestedDt = Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]);
                    }
                    if (abusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.MPIPP_PLAN_ID && abusPayeeAccount.icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_LATE && lbusBenefitCalculationHeader != null &&
                            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail != null && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Count > 0
                            && lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.age > busGlobalFunctions.GetMinDistributionAge(lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.person_id, ldtVestedDt)//busConstant.BenefitCalculation.AGE_70_HALF
                            && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null
                            && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Where(t => t.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).Count() <= 0)
                    {
                        lbusBenefitCalculationHeader.RecalculateMDBenefits(lbusBenefitCalculationHeader, abusPayeeAccount, ablnRetiree: true);
                    }
                    else
                    {
                        //  DateTime ldtMDdt = new DateTime(lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.icdoPerson.date_of_birth.AddYears(70).AddMonths(6).Year + 1, 04, 01);
                        DateTime ldtMDdt = new DateTime();
                        ldtMDdt = busGlobalFunctions.GetMinDistributionDate(lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.person_id, ldtVestedDt);

                        if (ldtMDdt.Year <= DateTime.Now.Year &&
                           lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail
                           .Where(t => t.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES && t.icdoBenefitCalculationYearlyDetail.plan_year >= ldtMDdt.Year).Count() > 0
                          )
                        {
                            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.istrBenefitOptionValue = lstrBenefitOption;
                            lbusBenefitCalculationHeader.RecalculateMDBenefits(lbusBenefitCalculationHeader, abusPayeeAccount, true);
                        }
                    }

                    if (lbusBenefitCalculationHeader != null && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail != null
                        && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Count() > 0)
                    {
                        if (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault() != null &&
                            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions != null
                            && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Count() > 0)
                        {
                            ldecBenefit_Amount = lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                            dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"] = lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount;
                            dr["CALCULATION_ID"] = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                            dr["AGE"] = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.age > 0 ? Math.Round(lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.age, 2) : dr["AGE"];
                        }

                        dr["INCREASE_ELIGIBLE"] = busConstant.NO_CAPS;

                        if (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null
                            && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Count > 0)
                        {
                            decimal ldecBenefitRateupto10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == ldecIncreaseYear && t.qualified_year_limit_value == "10").FirstOrDefault().rate;
                            decimal ldecBenefitRateafter10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == ldecIncreaseYear && t.qualified_year_limit_value == "20").FirstOrDefault().rate;

                            foreach (busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail in lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.OrderByDescending(t => t.icdoBenefitCalculationYearlyDetail.plan_year))
                            {
                                if (lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year <= lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date.Year)
                                {
                                    if ((lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_rate == ldecBenefitRateupto10QY ||
                                        lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_rate == ldecBenefitRateafter10QY) && lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.vested_hours > 0
                                        && lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount > 0)
                                    {
                                        dr["INCREASE_ELIGIBLE"] = busConstant.YES_CAPS;
                                        break;
                                    }
                                }
                            }

                            if (lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null &&
                                lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Count() > 0 &&
                                lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Where(t => t.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).Count() > 0)
                            {
                                dr["REEMPLOYED"] = busConstant.YES_CAPS;
                            }
                        }
                    }

                    if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id == 0)
                    {
                        dr["COMMENT"] += "Participant is not eligible for Retirement Benefits";
                        ldtbReportTable01.Rows.Add(dr);
                        return;
                    }
                    #region Active 10% Increase
                    ldecthrushHoldBenefit_Amount = Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]) + Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]) * 10.5m / 100;
                    if ((dr["STATUS_VALUE"].ToString().IsNotNullOrEmpty() && dr["STATUS_VALUE"].ToString() != busConstant.PayeeAccountStatusSuspendedDescription) && dr["REEMPLOYED"].ToString() != busConstant.YES_CAPS
                       && ldecBenefit_Amount > Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]) && ldecBenefit_Amount <= ldecthrushHoldBenefit_Amount)
                    {
                        if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.istrRetirementType == "MIND" || lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.istrRetirementType.IsNullOrEmpty())
                        {
                            lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.istrRetirementType = lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.benefit_subtype_value;
                        }
                       
                        lbusBenefitCalculationHeader.btn_ApproveCalculation();

                        abusPayeeAccount.FindPayeeAccount(abusPayeeAccount.icdoPayeeAccount.payee_account_id);
                        dr["BENEFIT_SUB_TYPE"] = abusPayeeAccount.icdoPayeeAccount.retirement_type_description;

                        abusPayeeAccount.LoadPayeeAccountStatuss();
                        dr["STATUS_VALUE"] = abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_description;
                        if (abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value.IsNotNullOrEmpty())
                        {
                            dr["SUSPENSION_STATUS_REASON_VALUE"] = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Payee_Account_Suspension_Reason_id,
                                    abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value).description;
                        }
                        else
                        {
                            dr["SUSPENSION_STATUS_REASON_VALUE"] = string.Empty;
                        }
                    }


                    dr["BREAK_IN_SERVICE"] = string.Empty;
                    dr["LAST_BREAK_IN_SERVICE"] = string.Empty;
                    GetLastBISYear(lbusBenefitCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI);

                    if (istrLastBisYears != string.Empty && Convert.ToInt32(istrLastBisYears) >= 2017 && Convert.ToInt32(istrLastBisYears) <= 2020)
                    {
                        dr["BREAK_IN_SERVICE"] = "Yes";
                        dr["LAST_BREAK_IN_SERVICE"] = istrLastBisYears.ToString();

                    }

                    dr["CALCULATION_STATUS"] = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.status_description;
                    ldtbReportTable01.Rows.Add(dr);
                    #endregion
                }
                else if (abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                {
                    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                    lbusPlanBenefitXr.FindPlanBenefitXr(abusPayeeAccount.icdoPayeeAccount.plan_benefit_id);
                    string lstrBenefitOption = lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;

                    DataTable ldtbList = busBase.Select("cdoDroApplication.LoadAllExistingQDRO", new object[1] { abusPayeeAccount.icdoPayeeAccount.person_id });

                    if (ldtbList != null && ldtbList.Rows.Count > 0)
                    {
                        dr["COMMENT"] += "QDRO On File.";
                        ldtbReportTable01.Rows.Add(dr);
                        return;
                    }

                    busBenefitCalculationHeader lbusBenefitCalculationDis = new busBenefitCalculationHeader();
                    lbusBenefitCalculationDis = abusPayeeAccount.RecalculateParticipantDisabilityBenefits(lstrBenefitOption);

                    busDisabiltyBenefitCalculation lbusBenefitCalculationDisability = new busDisabiltyBenefitCalculation();

                    lbusBenefitCalculationDisability.icdoBenefitCalculationHeader = lbusBenefitCalculationDis.icdoBenefitCalculationHeader;
                    lbusBenefitCalculationDisability.LoadBenefitCalculationDetails();
                    lbusBenefitCalculationDisability.LoadDisabilityRetireeIncreases();
                    lbusBenefitCalculationDisability.ibusBenefitApplication = new busBenefitApplication();
                    lbusBenefitCalculationDisability.LoadBenefitApplication();
                    lbusBenefitCalculationDisability.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusBenefitCalculationDisability.ibusPerson.FindPerson(lbusBenefitCalculationDis.ibusPerson.icdoPerson.person_id);
                    lbusBenefitCalculationDisability.ibusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitCalculationDis.ibusBenefitApplication.aclbPersonWorkHistory_MPI;

                    if (lbusBenefitCalculationDisability != null && lbusBenefitCalculationDisability.iclbBenefitCalculationDetail != null
                        && lbusBenefitCalculationDisability.iclbBenefitCalculationDetail.Count() > 0)
                    {
                        if (lbusBenefitCalculationDisability.iclbBenefitCalculationDetail.FirstOrDefault() != null &&
                            lbusBenefitCalculationDisability.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions != null
                            && lbusBenefitCalculationDisability.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Count() > 0)
                        {
                            ldecBenefit_Amount = lbusBenefitCalculationDisability.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.disability_amount;
                            dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"] = lbusBenefitCalculationDisability.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.participant_amount;//.disability_amount;
                            dr["CALCULATION_ID"] = lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                            dr["AGE"] = lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.age > 0 ? Math.Round(lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.age, 2) : dr["AGE"];
                        }
                    }

                    dr["INCREASE_ELIGIBLE"] = busConstant.NO_CAPS;

                    if (lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null
                        && lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Count > 0)
                    {
                        decimal ldecBenefitRateupto10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == ldecIncreaseYear && t.qualified_year_limit_value == "10").FirstOrDefault().rate;
                        decimal ldecBenefitRateafter10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == ldecIncreaseYear && t.qualified_year_limit_value == "20").FirstOrDefault().rate;

                        foreach (busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail in lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.OrderByDescending(t => t.icdoBenefitCalculationYearlyDetail.plan_year))
                        {
                            if (lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year <= lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.retirement_date.Year)
                            {
                                if ((lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_rate == ldecBenefitRateupto10QY ||
                                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_rate == ldecBenefitRateafter10QY) && lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.vested_hours > 0
                                    && lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount > 0)
                                {
                                    dr["INCREASE_ELIGIBLE"] = busConstant.YES_CAPS;
                                    break;
                                }
                            }
                        }

                        if (lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null &&
                                lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Count() > 0 &&
                                lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Where(t => t.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).Count() > 0)
                        {
                            dr["REEMPLOYED"] = busConstant.YES_CAPS;
                        }
                    }

                    if (lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.benefit_calculation_header_id == 0)
                    {
                        dr["COMMENT"] += "Participant is not eligible for Retirement Benefits.";
                        ldtbReportTable01.Rows.Add(dr);
                        return;

                    }
                    #region Active 10%  Increase
                    ldecthrushHoldBenefit_Amount = Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]) + Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]) * 10.5m / 100;
                    if (dr["STATUS_VALUE"].ToString().IsNotNullOrEmpty() && dr["STATUS_VALUE"].ToString() != busConstant.PayeeAccountStatusSuspendedDescription && dr["REEMPLOYED"].ToString() != busConstant.YES_CAPS
                       && ldecBenefit_Amount > Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]) && ldecBenefit_Amount <= ldecthrushHoldBenefit_Amount)
                    {
                        if (lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.istrRetirementType == "MIND" || lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.istrRetirementType.IsNullOrEmpty())
                        {
                            lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.istrRetirementType = lbusBenefitCalculationDisability.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.benefit_subtype_value;
                        }
                        lbusBenefitCalculationDisability.btn_ApproveCalculation();

                        abusPayeeAccount.FindPayeeAccount(abusPayeeAccount.icdoPayeeAccount.payee_account_id);
                        dr["BENEFIT_SUB_TYPE"] = abusPayeeAccount.icdoPayeeAccount.retirement_type_description;

                        abusPayeeAccount.LoadPayeeAccountStatuss();
                        dr["STATUS_VALUE"] = abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_description;
                        if (abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value.IsNotNullOrEmpty())
                        {
                            dr["SUSPENSION_STATUS_REASON_VALUE"] = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Payee_Account_Suspension_Reason_id,
                                    abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value).description;
                        }
                        else
                        {
                            dr["SUSPENSION_STATUS_REASON_VALUE"] = string.Empty;
                        }

                    }

                    dr["BREAK_IN_SERVICE"] = string.Empty;
                    dr["LAST_BREAK_IN_SERVICE"] = string.Empty;
                    GetLastBISYear(lbusBenefitCalculationDisability.ibusBenefitApplication.aclbPersonWorkHistory_MPI);

                    if (istrLastBisYears != string.Empty && Convert.ToInt32(istrLastBisYears) >= 2017 && Convert.ToInt32(istrLastBisYears) <= 2020)
                    {
                        dr["BREAK_IN_SERVICE"] = "Yes";
                        dr["LAST_BREAK_IN_SERVICE"] = istrLastBisYears.ToString();

                    }

                    dr["CALCULATION_STATUS"] = lbusBenefitCalculationDisability.icdoBenefitCalculationHeader.status_description;
                    ldtbReportTable01.Rows.Add(dr);
                    #endregion
                }
                else if (abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                {
                    busBenefitCalculationPreRetirementDeath lbusBenefitCalculationPreRetirementDeath = new busBenefitCalculationPreRetirementDeath { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                    lbusPlanBenefitXr.FindPlanBenefitXr(abusPayeeAccount.icdoPayeeAccount.plan_benefit_id);
                    string lstrBenefitOption = lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;
                    lbusBenefitCalculationPreRetirementDeath = abusPayeeAccount.RecalculateSurvivorPreRetirementDeathBenefits(lstrBenefitOption);

                    if (lbusBenefitCalculationPreRetirementDeath != null && lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail != null
                       && lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.Count() > 0)
                    {
                        if (lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault() != null &&
                            lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions != null
                            && lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.Count() > 0)
                        {
                            ldecBenefit_Amount = lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                            dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"] = lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.survivor_amount;
                            dr["CALCULATION_ID"] = lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.benefit_calculation_header_id;

                            if (dr["RETIREMENT_BENEFIT_AMOUNT"].IsNotNull() && Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]) <= decimal.Zero)
                                dr["RETIREMENT_BENEFIT_AMOUNT"] = lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.paid_amount;

                            dr["AGE"] = lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.age > 0 ? Math.Round(lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.age, 2) : dr["AGE"];
                        }
                    }

                    dr["INCREASE_ELIGIBLE"] = busConstant.NO_CAPS;

                    if (lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null
                        && lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Count > 0)
                    {
                        decimal ldecBenefitRateupto10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == ldecIncreaseYear && t.qualified_year_limit_value == "10").FirstOrDefault().rate;
                        decimal ldecBenefitRateafter10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == ldecIncreaseYear && t.qualified_year_limit_value == "20").FirstOrDefault().rate;

                        foreach (busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail in lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.OrderByDescending(t => t.icdoBenefitCalculationYearlyDetail.plan_year))
                        {
                            if (lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.plan_year <= lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.retirement_date.Year)
                            {
                                if ((lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_rate == ldecBenefitRateupto10QY ||
                                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_rate == ldecBenefitRateafter10QY) && lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.vested_hours > 0
                                    && lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.accrued_benefit_amount > 0)
                                {
                                    dr["INCREASE_ELIGIBLE"] = busConstant.YES_CAPS;
                                    break;
                                }
                            }
                        }
                    }

                    if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.benefit_calculation_header_id == 0)
                    {
                        dr["COMMENT"] += "Participant is not eligible for Death Benefits";
                        ldtbReportTable01.Rows.Add(dr);
                        return;
                    }

                    #region Active 10%  Increase
                    
                    ldecthrushHoldBenefit_Amount = Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]) + Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]) * 10.5m / 100;
                    if (dr["STATUS_VALUE"].ToString().IsNotNullOrEmpty() && dr["STATUS_VALUE"].ToString() != busConstant.PayeeAccountStatusSuspendedDescription && dr["REEMPLOYED"].ToString() != busConstant.YES_CAPS
                       && ldecBenefit_Amount > Convert.ToDecimal(dr["RETIREMENT_BENEFIT_AMOUNT"]) && ldecBenefit_Amount <= ldecthrushHoldBenefit_Amount)
                    {
                        if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrRetirementType == "MIND" || lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrRetirementType.IsNullOrEmpty())
                        {
                            lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrRetirementType = lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail[0].icdoBenefitCalculationDetail.benefit_subtype_value;
                        }
                        lbusBenefitCalculationPreRetirementDeath.btn_ApproveCalculation();

                        abusPayeeAccount.FindPayeeAccount(abusPayeeAccount.icdoPayeeAccount.payee_account_id);
                        dr["BENEFIT_SUB_TYPE"] = abusPayeeAccount.icdoPayeeAccount.retirement_type_description;

                        abusPayeeAccount.LoadPayeeAccountStatuss();
                        dr["STATUS_VALUE"] = abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_description;
                        if (abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value.IsNotNullOrEmpty())
                        {
                            dr["SUSPENSION_STATUS_REASON_VALUE"] = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Payee_Account_Suspension_Reason_id,
                                    abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value).description;
                        }
                        else
                        {
                            dr["SUSPENSION_STATUS_REASON_VALUE"] = string.Empty;
                        }

                    }

                    dr["BREAK_IN_SERVICE"] = string.Empty;
                    dr["LAST_BREAK_IN_SERVICE"] = string.Empty;
                    GetLastBISYear(lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.aclbPersonWorkHistory_MPI);

                    if (istrLastBisYears != string.Empty && Convert.ToInt32(istrLastBisYears) >= 2017 && Convert.ToInt32(istrLastBisYears) <= 2020)
                    {
                        dr["BREAK_IN_SERVICE"] = "Yes";
                        dr["LAST_BREAK_IN_SERVICE"] = istrLastBisYears.ToString();

                    }

                    dr["CALCULATION_STATUS"] = lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.status_description;
                    ldtbReportTable01.Rows.Add(dr);
                    #endregion
                }
                else if (abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_QDRO)
                {
                    busQdroCalculationHeader lbusQdroCalculationHeader = new busQdroCalculationHeader { icdoQdroCalculationHeader = new cdoQdroCalculationHeader() };
                    lbusQdroCalculationHeader = abusPayeeAccount.ReCalculateBenefitForQDRO(abusPayeeAccount);

                    if (lbusQdroCalculationHeader != null && lbusQdroCalculationHeader.iclbQdroCalculationDetail != null
                            && lbusQdroCalculationHeader.iclbQdroCalculationDetail.Count() > 0)
                    {
                        if (lbusQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault() != null &&
                            lbusQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().iclbQdroCalculationOptions != null
                            && lbusQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().iclbQdroCalculationOptions.Count() > 0)
                        {
                            dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"] = lbusQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().iclbQdroCalculationOptions.FirstOrDefault().icdoQdroCalculationOptions.alt_payee_benefit_amount;
                            dr["CALCULATION_ID"] = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                            dr["AGE"] = lbusQdroCalculationHeader.icdoQdroCalculationHeader.age > 0 ? Math.Round(lbusQdroCalculationHeader.icdoQdroCalculationHeader.age, 2) : dr["AGE"];
                        }

                        if (abusPayeeAccount.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == abusPayeeAccount.icdoPayeeAccount.iintPlanId).Count() > 0)
                        {
                            dr["PLAN_NAME"] = abusPayeeAccount.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == abusPayeeAccount.icdoPayeeAccount.iintPlanId).FirstOrDefault().icdoPersonAccount.istrPlanCode;
                        }
                    }

                    dr["INCREASE_ELIGIBLE"] = busConstant.NO_CAPS;

                    if (lbusQdroCalculationHeader.iclbQdroCalculationDetail[0].iclbQdroCalculationYearlyDetail != null
                        && lbusQdroCalculationHeader.iclbQdroCalculationDetail[0].iclbQdroCalculationYearlyDetail.Count > 0)
                    {
                        decimal ldecBenefitRateupto10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == ldecIncreaseYear && t.qualified_year_limit_value == "10").FirstOrDefault().rate;
                        decimal ldecBenefitRateafter10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == ldecIncreaseYear && t.qualified_year_limit_value == "20").FirstOrDefault().rate;

                        foreach (busQdroCalculationYearlyDetail lbusQdroCalculationYearlyDetail in lbusQdroCalculationHeader.iclbQdroCalculationDetail[0].iclbQdroCalculationYearlyDetail.OrderByDescending(t => t.icdoQdroCalculationYearlyDetail.plan_year))
                        {
                            if (lbusQdroCalculationYearlyDetail.icdoQdroCalculationYearlyDetail.plan_year <= lbusQdroCalculationHeader.icdoQdroCalculationHeader.benefit_comencement_date.Year)
                            {
                                if ((lbusQdroCalculationYearlyDetail.icdoQdroCalculationYearlyDetail.benefit_rate == ldecBenefitRateupto10QY ||
                                    lbusQdroCalculationYearlyDetail.icdoQdroCalculationYearlyDetail.benefit_rate == ldecBenefitRateafter10QY) && lbusQdroCalculationYearlyDetail.icdoQdroCalculationYearlyDetail.vested_hours > 0
                                    && lbusQdroCalculationYearlyDetail.icdoQdroCalculationYearlyDetail.accrued_benefit_amount > 0)
                                {
                                    dr["INCREASE_ELIGIBLE"] = busConstant.YES_CAPS;
                                    break;
                                }
                            }
                        }
                    }

                    if (lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id == 0)
                    {
                        dr["COMMENT"] += "Participant is not eligible for QDRO Benefits";
                        ldtbReportTable01.Rows.Add(dr);
                        return;
                    }

                    dr["BREAK_IN_SERVICE"] = string.Empty;
                    dr["LAST_BREAK_IN_SERVICE"] = string.Empty;
                    GetLastBISYear(lbusQdroCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI);
                    if (istrLastBisYears != string.Empty && Convert.ToInt32(istrLastBisYears) >= 2017 && Convert.ToInt32(istrLastBisYears) <= 2020)
                    {
                        dr["BREAK_IN_SERVICE"] = "Yes";
                        dr["LAST_BREAK_IN_SERVICE"] = istrLastBisYears.ToString();

                    }

                    dr["CALCULATION_STATUS"] = lbusQdroCalculationHeader.icdoQdroCalculationHeader.status_description;
                    ldtbReportTable01.Rows.Add(dr);
                }

            }
            catch (Exception e)
            {
                PostErrorMessage("Error Occured for MPID : " + abusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id);
                dr["COMMENT"] = "Error Occured for MPID : " + abusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id;
            }
        }

        public void Approve10PercentIncreaseReport(busPayeeAccount abusPayeeAccount, DataTable ldtbReportTable01, ArrayList larrErrors)
        {
            DataRow dr = ldtbReportTable01.NewRow();
            LoadPayeeAccountDetails(ref abusPayeeAccount);
            decimal ldecIncreaseYear = 0;
            DataTable ldtbPlanBenefitRate = busBase.Select("cdoPlanBenefitRate.Lookup", new object[] { });
            Collection<cdoPlanBenefitRate> lclbcdoPlanBenefitRate = cdoDummyWorkData.GetCollection<cdoPlanBenefitRate>(ldtbPlanBenefitRate);

            if (lclbcdoPlanBenefitRate != null && lclbcdoPlanBenefitRate.Count > 0 && lclbcdoPlanBenefitRate.Where(t => t.rate_type_value == busConstant.BenefitCalculation.PLAN_B && t.increase_percentage > 0).Count() > 0)
            {
                ldecIncreaseYear = lclbcdoPlanBenefitRate.Where(t => t.rate_type_value == busConstant.BenefitCalculation.PLAN_B && t.increase_percentage > 0).OrderByDescending(t => t.plan_year).FirstOrDefault().plan_year;
            }

            dr["PARTICIPANT_NAME"] = abusPayeeAccount.ibusParticipant.icdoPerson.first_name + " " + abusPayeeAccount.ibusParticipant.icdoPerson.last_name;
            dr["PARTICIPANT_MPI_PERSON_ID"] = abusPayeeAccount.ibusParticipant.icdoPerson.mpi_person_id;
            dr["PAYEE_NAME"] = abusPayeeAccount.ibusPayee.icdoPerson.first_name + " " + abusPayeeAccount.ibusPayee.icdoPerson.last_name;
            dr["PAYEE_MPI_PERSON_ID"] = abusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id;
            dr["BENEFIT_TYPE"] = abusPayeeAccount.icdoPayeeAccount.benefit_account_type_description;
            dr["BENEFIT_SUB_TYPE"] = abusPayeeAccount.icdoPayeeAccount.retirement_type_description;
            dr["PLAN_NAME"] = abusPayeeAccount.icdoPayeeAccount.istrPlanCode;
            dr["BENEFIT_OPTION"] = abusPayeeAccount.icdoPayeeAccount.istrBenefitOption;

            busAnnualBenefitSummaryOverview lbusAnnualBenefitSummaryOverView = new busAnnualBenefitSummaryOverview();
            if (lbusAnnualBenefitSummaryOverView.FindPerson(abusPayeeAccount.icdoPayeeAccount.person_id))
            {
                lbusAnnualBenefitSummaryOverView.LoadWorkHistory(true);
            }
            int lintLatestBisYear2 = GetLastBreakInServiceYear(lbusAnnualBenefitSummaryOverView.lbusBenefitApplication);
            if (lintLatestBisYear2 >= 2017 && lintLatestBisYear2 <= 2020)
            {
                dr["BREAK_IN_SERVICE"] = busConstant.FLAG_YES;
            }
            else
            {
                dr["BREAK_IN_SERVICE"] = busConstant.FLAG_NO;
            }

            dr["BREAK_IN_SERVICE_YEAR"] = lintLatestBisYear2.ToString();
            dr["RETIREMENT_DATE"] = abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate;
            dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"] = abusPayeeAccount.idecNextGrossPaymentACH == decimal.Zero ? abusPayeeAccount.idecPaidGrossAmount : abusPayeeAccount.idecNextGrossPaymentACH;
            dr["RETIREMENT_BENEFIT_AMOUNT"] = abusPayeeAccount.idecPreviousMonthlyGrossAmt;
            dr["RETRO_PAYMENT_MONTHS"] = Convert.ToDecimal(abusPayeeAccount.iintMonthDiff);

            if ((dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"].IsNotNull() && dr["RETIREMENT_BENEFIT_AMOUNT"].IsNotNull())
                && ((decimal)dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"] - (decimal)dr["RETIREMENT_BENEFIT_AMOUNT"] > 0))
            {
                dr["RECALCULATED_RETIREMENT_BENEFIT_DIFF"] = (decimal)dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"] - (decimal)dr["RETIREMENT_BENEFIT_AMOUNT"];
            }
            else
            {
                dr["RECALCULATED_RETIREMENT_BENEFIT_DIFF"] = 0;
            }
            dr["TOTAL_RETRO_PAYMENT"] = abusPayeeAccount.idecRetroAdjustmentAmount;
            dr["STATUS_VALUE"] = abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_description;

            if (larrErrors.Count > 0)
            {
                utlError lobjError = new utlError();
                lobjError = (utlError)larrErrors[0];

                dr["REASON_NOT_APPROVED"] = lobjError.istrErrorMessage;
                dr["PAYMENT_DIRECTIVE_DATE"] = "";
                dr["PAYMENT_CONFIRMATION_LETTER_DATE"] = "";
            }
            else
            {
                dr["REASON_NOT_APPROVED"] = "";
                dr["PAYMENT_DIRECTIVE_DATE"] = DateTime.Now.ToShortDateString();
                dr["PAYMENT_CONFIRMATION_LETTER_DATE"] = DateTime.Now.ToShortDateString();
            }
            dr["PAYEE_ACCOUNT_ID"] = abusPayeeAccount.icdoPayeeAccount.payee_account_id;

            ldtbReportTable01.Rows.Add(dr);
        }

        //private decimal GetMonthDifference(busPayeeAccount abusPayeeAccount)
        //{
        //    decimal monthDiff = 0;
        //    if (abusPayeeAccount.iclbPayeeAccountRetroPayment.Where(item => item.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH &&
        //        item.icdoPayeeAccountRetroPayment.start_date == abusPayeeAccount.idtNextBenefitPaymentDate &&
        //        item.icdoPayeeAccountRetroPayment.end_date == abusPayeeAccount.idtNextBenefitPaymentDate.GetLastDayofMonth()).Count() > 0)
        //    {
        //        busPayeeAccountRetroPayment lbusPayeeAccountRetroPayment = new busPayeeAccountRetroPayment { icdoPayeeAccountRetroPayment = new cdoPayeeAccountRetroPayment() };
        //        lbusPayeeAccountRetroPayment = abusPayeeAccount.iclbPayeeAccountRetroPayment.Where(item => item.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH &&
        //        item.icdoPayeeAccountRetroPayment.start_date == abusPayeeAccount.idtNextBenefitPaymentDate &&
        //        item.icdoPayeeAccountRetroPayment.end_date == abusPayeeAccount.idtNextBenefitPaymentDate.GetLastDayofMonth()).FirstOrDefault();

        //        //get difference in months between effective start/end dates
        //        monthDiff = ((lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_end_date.Year - lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_start_date.Year) * 12) + lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_end_date.Month - lbusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_start_date.Month;
        //    }
        //    return monthDiff;
        //}

        private int GetLastBreakInServiceYear(busBenefitApplication lbusBenefitApplication)
        {
            int lintLatestBisYear2 = 0;     //Latest BIS Year 2, if applicable   
            cdoDummyWorkData lcdoDummyWorkData = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(a => a.bis_years_count == 2).LastOrDefault();
            if (lcdoDummyWorkData != null)
            {
                lintLatestBisYear2 = lcdoDummyWorkData.year;
            }
            return lintLatestBisYear2;
        }

        public void LoadPayeeAccountDetails(ref busPayeeAccount abusPayeeAccount)
        {
            abusPayeeAccount.LoadPayeeAccountAchDetails();
            abusPayeeAccount.LoadPayeeAccountPaymentItemType();
            abusPayeeAccount.LoadPayeeAccountRetroPayments();
            abusPayeeAccount.LoadPayeeAccountRetroPaymentDetails();

            //Payment Adjustment
            abusPayeeAccount.LoadPayeeAccountBenefitOverPayment();
            // lobjPayeeAccount.LoadPayeeAccountOverPaymentPaymentDetails();
            abusPayeeAccount.LoadAllRepaymentSchedules();

            abusPayeeAccount.LoadPayeeAccountRolloverDetails();
            abusPayeeAccount.LoadPayeeAccountStatuss();
            abusPayeeAccount.LoadPayeeAccountTaxWithholdings();
            abusPayeeAccount.LoadBenefitDetails();
            abusPayeeAccount.LoadDRODetails();
            abusPayeeAccount.LoadNextBenefitPaymentDate();
            abusPayeeAccount.LoadTotalRolloverAmount();
            abusPayeeAccount.LoadGrossAmount();
            abusPayeeAccount.LoadPayeeAccountDeduction();
            abusPayeeAccount.LoadNonTaxableAmount();
            abusPayeeAccount.GetCalculatedTaxAmount();
            abusPayeeAccount.LoadDeathNotificationStatus();
            abusPayeeAccount.LoadWithholdingInformation();
            abusPayeeAccount.GetCuurentPayeeAccountStatus();
            abusPayeeAccount.CheckAnnuity();
            abusPayeeAccount.LoadLastPaymentDate();
            abusPayeeAccount.LoadPaymentHistoryHeaderDetails();

            //Payee Account Details
            if (abusPayeeAccount.icdoPayeeAccount.person_id != 0)
            {
                abusPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
                abusPayeeAccount.ibusPayee.FindPerson(abusPayeeAccount.icdoPayeeAccount.person_id);
            }
            //Organization Details
            if (abusPayeeAccount.icdoPayeeAccount.org_id != 0)
            {
                abusPayeeAccount.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                abusPayeeAccount.ibusOrganization.FindOrganization(abusPayeeAccount.icdoPayeeAccount.org_id);
            }

            //TransferOrg Details
            if (abusPayeeAccount.icdoPayeeAccount.transfer_org_id != 0)
            {
                busOrganization lbusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                if (lbusOrganization.FindOrganization(abusPayeeAccount.icdoPayeeAccount.transfer_org_id))
                {
                    abusPayeeAccount.icdoPayeeAccount.istrOrgMPID = lbusOrganization.icdoOrganization.mpi_org_id;
                    abusPayeeAccount.icdoPayeeAccount.istrOrgName = lbusOrganization.icdoOrganization.org_name;
                }
            }

            //Participant Account Details
            if (abusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
            {
                abusPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                abusPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(abusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                abusPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
                abusPayeeAccount.ibusParticipant.FindPerson(abusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);
            }
            //lobjPayeeAccount.CheckPaymentHistoryHeader();

            if (abusPayeeAccount.icdoPayeeAccount.adjustment_payment_eligible_flag == busConstant.Flag_Yes)
            {
                abusPayeeAccount.iblnAdjustmentPaymentEliglbleFlag = busConstant.YES;
            }

            abusPayeeAccount.LoadBreakDownDetails();
        }

        public void GetLastBISYear(Collection<cdoDummyWorkData> aclbPersonWorkHistory_MPI)
        {
            istrLastBisYears = string.Empty;

            int lintLatestBisYear2 = 0;     //Latest BIS Year 2, if applicable   
            cdoDummyWorkData lcdoDummyWorkData = aclbPersonWorkHistory_MPI.Where(a => a.bis_years_count == 2).LastOrDefault();
            if (lcdoDummyWorkData != null)
            {
                lintLatestBisYear2 = lcdoDummyWorkData.year;
                istrLastBisYears = lintLatestBisYear2.ToString();
            }
        }

        #region Create Excel Report
        private string CreateExcelReport(DataTable ldtbResultTable, string astrReportName, string astrPrefix = "")
        {

            ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            DataTable ldtbReportTable = ldtbResultTable;

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";
            ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);

            rvViewer.LocalReport.DataSources.Add(lrdsReport);

            byte[] bytes = rvViewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string labsRptGenPath = string.Empty;
            labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);

            string lstrReportFullName = string.Empty;

            if (astrReportName == "rptApprove10PercentIncrease")
            {
                lstrReportFullName = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_APPROVE_10_PERCENT_INCREASE_REPORT_PATH) + busConstant.APPROVE_10_PERCENT_INCREASE_BATCH_REPORT + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            }
            else
            {
                if (astrPrefix.IsNotNullOrEmpty())
                    lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
                else
                {
                    lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
                }
            }

            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            return lstrReportFullName;
        }
        #endregion

        private void RetrieveBatchParameters()
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
                            case busConstant.JobParamFromDate:
                                idtFromDate = Convert.ToDateTime(lobjParam.icdoJobParameters.param_value);
                                break;
                            case busConstant.JobParamToDate:
                                idtToDate = Convert.ToDateTime(lobjParam.icdoJobParameters.param_value);
                                break;

                        }
                    }
                }
            }
        }

        public void ChangeStatusToCancel()
        {
            busBase lobjBase = new busBase();

            //For Retirement
            DataTable ldtbResult = busBase.Select("cdoBenefitApplication.RunBatchForBenefitApplication", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            Collection<busRetirementApplication> lclbbusBenefitApplication = new Collection<busRetirementApplication>();
            lclbbusBenefitApplication = lobjBase.GetCollection<busRetirementApplication>(ldtbResult, "icdoBenefitApplication");
            ArrayList aarrResult = new ArrayList();
            Hashtable ahtbQueryBkmarks = new Hashtable();

            foreach (busRetirementApplication lbusRetirementApplication in lclbbusBenefitApplication)
            {
                if (lbusRetirementApplication.ibusPerson == null)
                {
                    lbusRetirementApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    if (lbusRetirementApplication.ibusPerson.FindPerson(lbusRetirementApplication.icdoBenefitApplication.person_id))
                    {
                        lbusRetirementApplication.icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED;
                        lbusRetirementApplication.icdoBenefitApplication.cancellation_reason_value = busConstant.BENEFIT_CANCEL_OTHER;
                        lbusRetirementApplication.icdoBenefitApplication.reason_description = busConstant.BENEFIT_BATCH_CANCEL_AUTO;
                        lbusRetirementApplication.icdoBenefitApplication.Update();

                        lbusRetirementApplication.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(DateTime.Now);
                        aarrResult.Add(lbusRetirementApplication);
                        this.CreateCorrespondence(busConstant.CANCELLATION_NOTIFICATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                        //Ticket#69388
                        this.CreateCorrespondence(busConstant.RETIREMENT_CANCELLATION_FORM, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                        aarrResult.Clear();
                    }

                }
            }
        }


        public void ChangeWithdrawlStatusToCancel()
        {
            busBase lobjBase = new busBase();

            //For Withdrawal
            DataTable ldtbResult = busBase.Select("cdoBenefitApplication.RunBatchForWithdrawal", new object[0]);
            Collection<busWithdrawalApplication> lclbbusBenefitApplication = new Collection<busWithdrawalApplication>();
            lclbbusBenefitApplication = lobjBase.GetCollection<busWithdrawalApplication>(ldtbResult, "icdoBenefitApplication");

            ArrayList aarrResult = new ArrayList();
            Hashtable ahtbQueryBkmarks = new Hashtable();

            foreach (busWithdrawalApplication lbusWithdrawalApplication in lclbbusBenefitApplication)
            {
                if (lbusWithdrawalApplication.ibusPerson == null)
                {
                    lbusWithdrawalApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    if (lbusWithdrawalApplication.ibusPerson.FindPerson(lbusWithdrawalApplication.icdoBenefitApplication.person_id))
                    {

                        lbusWithdrawalApplication.icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED;
                        lbusWithdrawalApplication.icdoBenefitApplication.cancellation_reason_value = busConstant.BENEFIT_CANCEL_OTHER;
                        lbusWithdrawalApplication.icdoBenefitApplication.reason_description = busConstant.BENEFIT_BATCH_CANCEL_AUTO;
                        lbusWithdrawalApplication.icdoBenefitApplication.Update();

                        lbusWithdrawalApplication.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(DateTime.Now);

                        aarrResult.Add(lbusWithdrawalApplication);
                        this.CreateCorrespondence(busConstant.CANCELLATION_NOTIFICATION_WITHDRAWAL, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                        aarrResult.Clear();
                    }
                }

            }

        }
    }
}
