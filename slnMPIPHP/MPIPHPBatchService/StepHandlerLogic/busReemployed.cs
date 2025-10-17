using Microsoft.Reporting.WinForms;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using MPIPHPJobService;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using Sagitec.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPIPHPJobService
{
    class busReemployed : busBatchHandler
    {

        #region Properties

        private object iobjLock = null;
        private string istrBatchName { get; set; }

        DataTable adtReempResumeBenefitPayeeAccounts = null;

        #endregion

        #region ReEmployed Batch : Daily changed to Monthly
        public void ReemployedBatch()
        {
            int lintCount = 0;
            int lintTotalCount = 0;
            
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            DataTable ldtbReportTable01 = new DataTable();
            ldtbReportTable01.TableName = "ReportTable01";

            //PIR 1051
            ldtbReportTable01.Columns.Add("PARTICIPANT_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("PARTICIPANT_MPI_PERSON_ID", typeof(string));
            ldtbReportTable01.Columns.Add("PAYEE_MPI_PERSON_ID", typeof(string));
            ldtbReportTable01.Columns.Add("PAYEE_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_TYPE", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_SUB_TYPE", typeof(string));
            ldtbReportTable01.Columns.Add("RETIREMENT_DATE", typeof(DateTime));
            ldtbReportTable01.Columns.Add("PLAN_NAME", typeof(string));            
            ldtbReportTable01.Columns.Add("PAYEE_ACCOUNT_ID", typeof(int));
            ldtbReportTable01.Columns.Add("FAMILY_RELATIONSHIP", typeof(string));
            ldtbReportTable01.Columns.Add("STATUS_VALUE", typeof(string));
            ldtbReportTable01.Columns.Add("SUSPENSION_STATUS_REASON_VALUE", typeof(string));
            ldtbReportTable01.Columns.Add("BATCH_ID", typeof(string));
            ldtbReportTable01.Columns.Add("STATUS_EFFECTIVE_DATE", typeof(DateTime));

            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();
            
            DataTable ldtReemployedPayeeAccounts = busBase.Select("cdoPayeeAccount.ReemployedParticipantsFromEADB", new object[0]);
                       

            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 1;

            Parallel.ForEach(ldtReemployedPayeeAccounts.AsEnumerable(), po, (ldtReemployed, loopState) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "ReemployedBatch";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                iobjPassInfo = lobjPassInfo;

                if (istrBatchName.IsNullOrEmpty())
                    istrBatchName = lobjPassInfo.istrUserID;
                
                EvaluateReEmployedRules(ldtReemployed, lobjPassInfo, lintCount, lintTotalCount, ldtbReportTable01);

                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;
            });

            iobjPassInfo = lobjMainPassInfo;

            if (ldtbReportTable01 != null && ldtbReportTable01.Rows.Count > 0)
                CreateExcelReport(ldtbReportTable01, "rptReemploymentBatch");
        }

        public void EvaluateReEmployedRules(DataRow adtPerson, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount, DataTable ldtbReportTable01)
        {
            busPayeeAccount lbusPayeeAccount = null;
            DataRow dr = ldtbReportTable01.NewRow();
            bool lblnReemployed = false;

            
            lock (iobjLock)
            {
                aintCount++;
                aintTotalCount++;
                if (aintCount == 100)
                {
                    String lstrMsg = aintTotalCount + " : " + " Records Has Been Processed";
                    PostInfoMessage(lstrMsg);
                    aintCount = 0;
                }
            }


            #region Evaluate ReEmployed Hours


            autlPassInfo.BeginTransaction();
            try
            {
                int lintPlanID = Convert.ToInt32(adtPerson[enmPlanBenefitXr.plan_id.ToString()]);
                DateTime ldtDOB = new DateTime();
                DateTime ldtMinDistributionDate = new DateTime();
                //RMD72Project
                int lintPersonId = Convert.ToInt32(adtPerson[enmPerson.person_id.ToString()]);
                DateTime ldtVestedDate = busGlobalFunctions.GetVestedDate(lintPersonId, lintPlanID);

                if (!string.IsNullOrEmpty(Convert.ToString(adtPerson[enmPerson.date_of_birth.ToString()])))
                {

                    ldtDOB = Convert.ToDateTime(adtPerson[enmPerson.date_of_birth.ToString()]);
                    // RID# 153935 As per teresa, even if participant has differed RMD batch should use 70.5 RMD date to check for suspension.
                    ldtMinDistributionDate = busGlobalFunctions.GetMinDistributionDate(ldtDOB, ldtVestedDate); //calculate MD date based on age 70.5
                    /*
                    //ldtMinDistributionDate = new DateTime(ldtDOB.AddYears(70).AddMonths(6).AddYears(1).Year, 04, 01);
                    //RMD72Project
                    ldtMinDistributionDate = busGlobalFunctions.GetMinDistributionDate(lintPersonId, ldtVestedDate);  //calculate MD Date based on participant MD age option
                    DateTime ldt72MinDate = busGlobalFunctions.Get72MinDistributionDate(ldtDOB, ldtVestedDate); //calculate MD date based on age 70.5
                    DateTime ldtRetirementDate = Convert.ToDateTime(adtPerson["RETIREMENT_DATE"]);

                    //If participant had age 72 option but retireed before 72MD date, business asked to use 70.5 instead of if participant age 72 MD date.
                    if (ldtMinDistributionDate == ldt72MinDate && ldtRetirementDate < ldtMinDistributionDate)
                    {
                        ldtMinDistributionDate = busGlobalFunctions.GetMinDistributionDate(ldtDOB, ldtVestedDate); //calculate MD date based on age 70.5
                    } */
                }
                if (iobjSystemManagement.icdoSystemManagement.batch_date < ldtMinDistributionDate)
                {
                    lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lbusPayeeAccount.icdoPayeeAccount.LoadData(adtPerson);
                                                            

                    if (lintPlanID == 1 || lintPlanID == 2)
                    {
                        string lstrRuleCorrespondence = lbusPayeeAccount.EvaluateReEmployedRules(adtPerson, iobjSystemManagement.icdoSystemManagement.batch_date,ref lblnReemployed, true, aintJobScheduleId: this.ibusJobHeader.icdoJobHeader.job_schedule_id);

                        if (!string.IsNullOrEmpty(lstrRuleCorrespondence))
                        {
                            ArrayList aarrResult = new ArrayList();
                            Hashtable ahtbQueryBkmarks = new Hashtable();
                            
                            if (lbusPayeeAccount.ibusParticipant == null)
                            {
                                lbusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                                lbusPayeeAccount.ibusParticipant.icdoPerson.LoadData(adtPerson);
                                lbusPayeeAccount.aintNoOverPayement = 1;
                                lbusPayeeAccount.ibusParticipant.LoadCorrAddress();
                                
                                String lstrMsg = "Re-Employment processed for Payee Account ID : " + lbusPayeeAccount.icdoPayeeAccount.payee_account_id + " .";
                                PostInfoMessage(lstrMsg);

                            }
                            aarrResult.Add(lbusPayeeAccount);
                            this.CreateReEmployedCorrespondence(lstrRuleCorrespondence, aarrResult, ahtbQueryBkmarks, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID);
                        }
                    }
                    else
                    {
                        lblnReemployed = true;
                    }

                    //PIR 1051
                    if (lblnReemployed)
                    {

                        LoadPayeeAccountDetails(ref lbusPayeeAccount);

                        if (lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.created_date 
                            >= ibusJobHeader.icdoJobHeader.start_time && lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_value
                            == busConstant.PayeeAccountStatusSuspended)
                        {

                            dr["PARTICIPANT_NAME"] = lbusPayeeAccount.ibusParticipant.icdoPerson.first_name + " " + lbusPayeeAccount.ibusParticipant.icdoPerson.last_name;
                            dr["PAYEE_MPI_PERSON_ID"] = lbusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id;
                            dr["PAYEE_NAME"] = lbusPayeeAccount.ibusPayee.icdoPerson.first_name + " " + lbusPayeeAccount.ibusPayee.icdoPerson.last_name;
                            dr["PARTICIPANT_MPI_PERSON_ID"] = lbusPayeeAccount.ibusParticipant.icdoPerson.mpi_person_id;
                            dr["PLAN_NAME"] = lbusPayeeAccount.icdoPayeeAccount.istrPlanCode;
                            dr["BENEFIT_TYPE"] = lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_description;
                            dr["PAYEE_ACCOUNT_ID"] = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                            dr["BENEFIT_SUB_TYPE"] = lbusPayeeAccount.icdoPayeeAccount.retirement_type_description;
                            dr["RETIREMENT_DATE"] = lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate.Date;
                            dr["FAMILY_RELATIONSHIP"] = lbusPayeeAccount.icdoPayeeAccount.family_relation_description;

                            dr["STATUS_VALUE"] = lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_description;
                            if (lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value.IsNotNullOrEmpty())
                            {
                                dr["SUSPENSION_STATUS_REASON_VALUE"] = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Payee_Account_Suspension_Reason_id,
                                        lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value).description;
                            }
                            else
                                dr["SUSPENSION_STATUS_REASON_VALUE"] = string.Empty;

                            dr["BATCH_ID"] = istrBatchName;
                            dr["STATUS_EFFECTIVE_DATE"] = lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_effective_date.Date;

                            ldtbReportTable01.Rows.Add(dr);

                            if (lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().
                            icdoPayeeAccountStatus.created_by != istrBatchName)
                            {
                                DBFunction.DBExecuteScalar("cdoPerson.UpdateBatchNameRempPayeeAccounts", new object[2] { istrBatchName,lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().
                                icdoPayeeAccountStatus.payee_account_status_id}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            }

                            if (lintPlanID == 2)
                                LoadOtherPayeeAccountsForParticipant(lbusPayeeAccount.ibusParticipant.icdoPerson.person_id, lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault()
                                    , ref ldtbReportTable01);
                        }
                    }
                }
                else
                {
                    //2nd year od MD onwards : Participant to be picked from annual reevation batch
                    //1st Year : From Resume Benefits Batch.
                    lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lbusPayeeAccount.icdoPayeeAccount.LoadData(adtPerson);
                    lbusPayeeAccount.icdoPayeeAccount.reemployed_flag_from_eadb = busConstant.FLAG_NO;
                    lbusPayeeAccount.icdoPayeeAccount.accrued_benefit_to_be_paid_reeval_flag = busConstant.FLAG_YES;
                    lbusPayeeAccount.icdoPayeeAccount.Update();
                }


            #endregion

                autlPassInfo.Commit();
            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    String lstrMsg = "Error while Executing Batch,Error Message For Payee Account ID: " + lbusPayeeAccount.icdoPayeeAccount.payee_account_id + ":" + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();
            }
        }


        public void LoadOtherPayeeAccountsForParticipant(int aintParticipantId,busPayeeAccountStatus abusPayeeAccountStatus, ref DataTable ldtbReportTable01)
        {
            DataTable ldtOtherPayeeAccounts = busBase.Select("cdoPayeeAccount.GetQDROAndLocalPayeeAccountsForParticipant", new object[2] { aintParticipantId,iobjPassInfo.istrUserID});

            if(ldtOtherPayeeAccounts != null && ldtOtherPayeeAccounts.Rows.Count > 0)
            {
                foreach (DataRow ldrOtherPayeeAccounts in ldtOtherPayeeAccounts.Rows)
                {
                    busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lbusPayeeAccount.icdoPayeeAccount.LoadData(ldrOtherPayeeAccounts);

                    busPayeeAccountStatus lbusPayeeAccountStatus = new busPayeeAccountStatus();

                    if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.MPIPP_PLAN_ID && lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                    {
                        lbusPayeeAccountStatus.InsertValuesInPayeeAccountStatus(lbusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED, DateTime.Now, abusPayeeAccountStatus.icdoPayeeAccountStatus.suspension_status_reason_value, aintJobScheduleId: this.ibusJobHeader.icdoJobHeader.job_schedule_id);
                    }

                    LoadPayeeAccountDetails(ref lbusPayeeAccount);


                    if (lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.created_date
                        >= ibusJobHeader.icdoJobHeader.start_time && lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_value
                        == busConstant.PayeeAccountStatusSuspended)
                    {
                        
                        DataRow dr = ldtbReportTable01.NewRow();

                        dr["PARTICIPANT_NAME"] = lbusPayeeAccount.ibusParticipant.icdoPerson.first_name + " " + lbusPayeeAccount.ibusParticipant.icdoPerson.last_name;
                        dr["PAYEE_MPI_PERSON_ID"] = lbusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id;
                        dr["PAYEE_NAME"] = lbusPayeeAccount.ibusPayee.icdoPerson.first_name + " " + lbusPayeeAccount.ibusPayee.icdoPerson.last_name;
                        dr["PARTICIPANT_MPI_PERSON_ID"] = lbusPayeeAccount.ibusParticipant.icdoPerson.mpi_person_id;
                        dr["PLAN_NAME"] = lbusPayeeAccount.icdoPayeeAccount.istrPlanCode;
                        dr["BENEFIT_TYPE"] = lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_description;
                        dr["PAYEE_ACCOUNT_ID"] = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                        dr["BENEFIT_SUB_TYPE"] = lbusPayeeAccount.icdoPayeeAccount.retirement_type_description;
                        dr["RETIREMENT_DATE"] = lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate.Date;
                        dr["FAMILY_RELATIONSHIP"] = lbusPayeeAccount.icdoPayeeAccount.family_relation_description;

                        dr["STATUS_VALUE"] = lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_description;
                        if (lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value.IsNotNullOrEmpty())
                        {
                            dr["SUSPENSION_STATUS_REASON_VALUE"] = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Payee_Account_Suspension_Reason_id,
                                    lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value).description;
                        }
                        else
                            dr["SUSPENSION_STATUS_REASON_VALUE"] = string.Empty;

                        dr["BATCH_ID"] = istrBatchName;
                        dr["STATUS_EFFECTIVE_DATE"] = lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_effective_date.Date;

                        ldtbReportTable01.Rows.Add(dr);

                        if(lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().
                            icdoPayeeAccountStatus.created_by != istrBatchName)
                        {
                            DBFunction.DBExecuteScalar("cdoPerson.UpdateBatchNameRempPayeeAccounts", new object[2] { istrBatchName,lbusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().
                                icdoPayeeAccountStatus.payee_account_status_id}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        }
                    }
                    
                }
            }
        }

        public void GetCorDescription()
        {

        }
        
        #endregion

        #region Annual ReEvaluation Batch


        public void AnnualReEvaluationforReEmployedBatch()
        {
            busBase lobjBase = new busBase();

            //For Retirement
            DataTable ldtbResult = busBase.Select("cdoPayeeAccount.GetReemployedParticipants", new object[0] { });
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
            if (ldtbResult != null && ldtbResult.Rows.Count > 0)
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
            ldtbReportTable01.Columns.Add("RETIREMENT_DATE", typeof(DateTime));
            ldtbReportTable01.Columns.Add("MIN_DISTRIBUTION_DATE", typeof(DateTime));
            ldtbReportTable01.Columns.Add("PLAN_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("PAYEE_ACCOUNT_ID", typeof(int));
            ldtbReportTable01.Columns.Add("COMMENT", typeof(string));
            ldtbReportTable01.Columns.Add("CALCULATION_ID", typeof(Int32));         
            ldtbReportTable01.Columns.Add("REEMPLOYED", typeof(string));
            ldtbReportTable01.Columns.Add("BENEFIT_OPTION", typeof(string));
            ldtbReportTable01.Columns.Add("STATUS_VALUE", typeof(string));
            ldtbReportTable01.Columns.Add("AGE", typeof(decimal));
            ldtbReportTable01.Columns.Add("SUSPENSION_STATUS_REASON_VALUE", typeof(string));
            ldtbReportTable01.Columns.Add("RETIREMENT_BENEFIT_AMOUNT", typeof(decimal));
            ldtbReportTable01.Columns.Add("RECALCULATED_RETIREMENT_BENEFIT_AMOUNT", typeof(decimal));


            foreach (busPayeeAccount lbusPayeeAccount in lclbPayeeAccount)
            {
                ReCalculateReemploymentBenefits(lbusPayeeAccount, ldtbReportTable01);
            }

            if (ldtbReportTable01 != null && ldtbReportTable01.Rows.Count > 0)
                CreateExcelReport(ldtbReportTable01, "rptReevaluationOfReemploymentBatch");

        }


        public void ReCalculateReemploymentBenefits(busPayeeAccount abusPayeeAccount, DataTable ldtbReportTable01)
        {
            DataRow dr = ldtbReportTable01.NewRow();


            LoadPayeeAccountDetails(ref abusPayeeAccount);

            dr["PARTICIPANT_NAME"] = abusPayeeAccount.ibusParticipant.icdoPerson.first_name + " " + abusPayeeAccount.ibusParticipant.icdoPerson.last_name;
            dr["PAYEE_MPI_PERSON_ID"] = abusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id;
            dr["PAYEE_NAME"] = abusPayeeAccount.ibusPayee.icdoPerson.first_name + " " + abusPayeeAccount.ibusPayee.icdoPerson.last_name;
            dr["PARTICIPANT_MPI_PERSON_ID"] = abusPayeeAccount.ibusParticipant.icdoPerson.mpi_person_id;
            dr["PLAN_NAME"] = abusPayeeAccount.icdoPayeeAccount.istrPlanCode;
            dr["BENEFIT_TYPE"] = abusPayeeAccount.icdoPayeeAccount.benefit_account_type_description;
            dr["PAYEE_ACCOUNT_ID"] = abusPayeeAccount.icdoPayeeAccount.payee_account_id;
            dr["BENEFIT_SUB_TYPE"] = abusPayeeAccount.icdoPayeeAccount.retirement_type_description;
            dr["RETIREMENT_DATE"] = abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate;
            dr["BENEFIT_OPTION"] = abusPayeeAccount.icdoPayeeAccount.istrBenefitOption;
            dr["RETIREMENT_BENEFIT_AMOUNT"] = abusPayeeAccount.idecNextGrossPaymentACH == decimal.Zero ? abusPayeeAccount.idecPaidGrossAmount : abusPayeeAccount.idecNextGrossPaymentACH;

            if (abusPayeeAccount.icdoPayeeAccount.benefit_account_type_description == "Retirement")
            {
                if ((abusPayeeAccount.icdoPayeeAccount.idtMin_Distribution_Date == DateTime.MinValue) || (abusPayeeAccount.icdoPayeeAccount.idtMin_Distribution_Date == null))
                {
                   
                    if (abusPayeeAccount.ibusParticipant.icdoPerson.date_of_birth != DateTime.MinValue)
                    {
                        dr["MIN_DISTRIBUTION_DATE"] = LoadMinDistributionDate(abusPayeeAccount.ibusParticipant.icdoPerson.date_of_birth, abusPayeeAccount.ibusParticipant.icdoPerson.person_id);
                    }


                }
                else
                {
                        if (abusPayeeAccount.icdoPayeeAccount.idtMin_Distribution_Date != DateTime.MinValue)
                        {
                            dr["MIN_DISTRIBUTION_DATE"] = abusPayeeAccount.icdoPayeeAccount.idtMin_Distribution_Date;

                        }
                }
            }

           
            

            dr["STATUS_VALUE"] = abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_description;

            dr["STATUS_VALUE"] = abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.status_description;
            if (abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value.IsNotNullOrEmpty())
            {
                dr["SUSPENSION_STATUS_REASON_VALUE"] = busGlobalFunctions.GetCodeValueDescriptionByValue(busConstant.Payee_Account_Suspension_Reason_id,
                        abusPayeeAccount.iclbPayeeAccountStatus.OrderByDescending(t => t.icdoPayeeAccountStatus.status_effective_date).FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value).description;
            }
            else
                dr["SUSPENSION_STATUS_REASON_VALUE"] = string.Empty;

            dr["AGE"] = Math.Round(busGlobalFunctions.CalculatePersonAge(abusPayeeAccount.ibusParticipant.icdoPerson.date_of_birth, abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate), 2);

          

            if(abusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
            {
                ldtbReportTable01.Rows.Add(dr);
                return;
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

                    dr["COMMENT"] = "QDRO Payee Account re-calculation must me be processed first.";
                    ldtbReportTable01.Rows.Add(dr);
                    return;
                }

                else if (lintPendingAdjCalcCount > 0)
                {
                    DBFunction.DBExecuteScalar("cdoBenefitCalculationHeader.CancelPendingAdjustmentCalculations",
                                                                                new object[3] { abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value, abusPayeeAccount.icdoPayeeAccount.person_id, abusPayeeAccount.icdoPayeeAccount.iintPlanId },
                                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }

                if (abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
                {
                    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                    lbusPlanBenefitXr.FindPlanBenefitXr(abusPayeeAccount.icdoPayeeAccount.plan_benefit_id);
                    string lstrBenefitOption = lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;

                    DataTable ldtbList = busBase.Select("cdoDroApplication.LoadAllExistingQDRO", new object[1] { abusPayeeAccount.icdoPayeeAccount.person_id });

                    if (ldtbList != null && ldtbList.Rows.Count > 0)
                    {
                        dr["COMMENT"] += "QDRO On File.";
                    }

                    busBenefitCalculationRetirement lbusBenefitCalculationHeader = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    lbusBenefitCalculationHeader = abusPayeeAccount.ReCalculateBenefitForRetirement(lstrBenefitOption, ablnPostRetDeath: true);

                    //RMD72Project change constant with global function.
                    if (abusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.MPIPP_PLAN_ID && abusPayeeAccount.icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_LATE && lbusBenefitCalculationHeader != null &&
                            lbusBenefitCalculationHeader.iclbBenefitCalculationDetail != null && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.Count > 0
                            && lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.age >  Convert.ToDecimal(busGlobalFunctions.GetMinDistributionAge(lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.icdoPerson.person_id))  //busConstant.BenefitCalculation.AGE_70_HALF
                            && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail != null
                            && lbusBenefitCalculationHeader.iclbBenefitCalculationDetail[0].iclbBenefitCalculationYearlyDetail.Where(t => t.icdoBenefitCalculationYearlyDetail.reemployed_flag == busConstant.FLAG_YES).Count() <= 0)
                    {
                        lbusBenefitCalculationHeader.RecalculateMDBenefits(lbusBenefitCalculationHeader, abusPayeeAccount, ablnRetiree: true);
                    }
                    else
                    {
                        //RMD72Project
                        //DateTime ldtMDdt = new DateTime(lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.icdoPerson.date_of_birth.AddYears(70).AddMonths(6).Year + 1, 04, 01);
                        DateTime ldtMDdt = busGlobalFunctions.GetMinDistributionDate(lbusBenefitCalculationHeader.ibusBenefitApplication.ibusPerson.icdoPerson.person_id);

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
                            dr["RECALCULATED_RETIREMENT_BENEFIT_AMOUNT"] = lbusBenefitCalculationHeader.iclbBenefitCalculationDetail.FirstOrDefault().iclbBenefitCalculationOptions.FirstOrDefault().icdoBenefitCalculationOptions.benefit_amount.RoundToTwoDecimalPoints();
                            dr["CALCULATION_ID"] = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                            dr["AGE"] = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.age > 0 ? Math.Round(lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.age, 2) : dr["AGE"];
                        }
                        
                    }

                    if (lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_calculation_header_id == 0)
                    {
                        dr["COMMENT"] += "Participant is not eligible for Retirement Benefits";
                        ldtbReportTable01.Rows.Add(dr);
                        return;
                    }
                    ldtbReportTable01.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                PostErrorMessage("Error Occured for MPID : " + abusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id);
                dr["COMMENT"] = "Error Occured for MPID : " + abusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id;
            }
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

            byte[] bytes = rvViewer.LocalReport.Render("Excel",null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            
            string labsRptGenPath = string.Empty;
            labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);

            string lstrReportFullName = string.Empty;

            if (astrPrefix.IsNotNullOrEmpty())
                lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            else
            {
                lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            }

            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            return lstrReportFullName;
        }
        #endregion

        private DateTime LoadMinDistributionDate(DateTime adtDateOfBirth, int person_id)
        {
            //DateTime ldtMinDistributionDate = DateTime.MinValue;
            //DateTime ldtDob = adtDateOfBirth;

            //ldtDob = ldtDob.AddYears(70);
            //ldtDob = ldtDob.AddMonths(6);

            //for pir-522
            //DateTime ldtVestedDt = new DateTime();
            //DataTable ldtGetVestedDate = busBase.Select("cdoPersonAccountEligibility.GetVestedDateForMD", new object[1] { person_id });
            //if (ldtGetVestedDate != null && ldtGetVestedDate.Rows.Count > 0 && (Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]) != DateTime.MinValue))
            //{
            //    ldtVestedDt = Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]);
            //    if (ldtVestedDt != DateTime.MinValue && ldtVestedDt > ldtDob)
            //        ldtMinDistributionDate = new DateTime(ldtVestedDt.Year + 1, 01, 01);
            //    else
            //        ldtMinDistributionDate = new DateTime(ldtDob.Year + 1, 04, 01);
            //}
            //else
            //    ldtMinDistributionDate = new DateTime(ldtDob.Year + 1, 04, 01);

            //RMD72Project
            DateTime ldtVestedDt = busGlobalFunctions.GetVestedDate(person_id, busConstant.MPIPP_PLAN_ID);
            DateTime ldtMinDistributionDate = busGlobalFunctions.GetMinDistributionDate(person_id, ldtVestedDt);

            return ldtMinDistributionDate;
        }

        //public void AnnualReEvaluationforReEmployedBatch()
        //{
        //    int lintCount = 0;
        //    int lintTotalCount = 0;

        //    DataTable ldtReemployedPayeeAccounts = busBase.Select("cdoPayeeAccount.GetReemployedParticipants", new object[0]);
        //    if (ldtReemployedPayeeAccounts.Rows.Count > 0)
        //    {
        //        Dictionary<string, object> ldictParams = new Dictionary<string, object>();
        //        foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
        //        {
        //            ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
        //        }
        //        iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
        //        utlPassInfo lobjMainPassInfo = iobjPassInfo;
        //        iobjLock = new object();

        //        //ParallelOptions po = new ParallelOptions();
        //        //po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

        //        foreach (DataRow adtPayeeAccounts in ldtReemployedPayeeAccounts.Rows)
        //        {
        //            //Parallel.ForEach(ldtReemployedPayeeAccounts.AsEnumerable(), po, (adtPayeeAccounts, loopState) =>
        //            //{
        //            utlPassInfo lobjPassInfo = new utlPassInfo();
        //            lobjPassInfo.idictParams = ldictParams;
        //            lobjPassInfo.idictParams["ID"] = "GenerateReEvaluationOfBenefitWorkflowBatch";
        //            lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
        //            utlPassInfo.iobjPassInfo = lobjPassInfo;

        //            GenerateReEvaluationOfBenefitWorkflow(adtPayeeAccounts, lobjPassInfo, lintCount, lintTotalCount);

        //            if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
        //            {
        //                lobjPassInfo.iconFramework.Close();
        //            }

        //            lobjPassInfo.iconFramework.Dispose();
        //            lobjPassInfo.iconFramework = null;
        //            //});
        //        }

        //        utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        //    }
        //}

        //public void GenerateReEvaluationOfBenefitWorkflow(DataRow adtPayeeAccount, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        //{
        //    bool lblnReEvaluateBenefits = false;
        //    bool lblnLateHoursReported = false;
        //    string lstrSSN = string.Empty;
        //    DateTime ldtFromDate = new DateTime();
        //    DateTime ldtToDate = new DateTime();
        //    DateTime ldtDOB = new DateTime();
        //    DateTime ldtRetirementDate = new DateTime();
        //    DateTime ldtMinDistributionDate = new DateTime();

        //    //To Check If Late Hours Reported Last Year
        //    DateTime ldtProcessedFromDate = new DateTime();
        //    DateTime ldtProcessedToDate = new DateTime();

        //    decimal ldecAge = decimal.Zero;
        //    string lstrBenefitTypeValue = string.Empty;
        //    string lstrBenefitSubTypeValue = string.Empty;
        //    string lstrBenefitOptionValue = string.Empty;
        //    string lstrPlanCode = string.Empty;
        //    int lintBenefitCalcID = 0;
        //    int lintBenefitAppID;
        //    int lintPlanID = 0;
        //    string lstrPayeeAccountCreatedBy = string.Empty;

        //    busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
        //    lbusPayeeAccount.icdoPayeeAccount.LoadData(adtPayeeAccount);
        //    ldtDOB = Convert.ToDateTime(adtPayeeAccount[enmPerson.date_of_birth.ToString()]);
        //    lstrPlanCode = Convert.ToString(adtPayeeAccount[enmPlan.plan_code.ToString()]);
        //    ldtRetirementDate = Convert.ToDateTime(adtPayeeAccount[enmBenefitApplication.retirement_date.ToString()]);
        //    lintBenefitAppID = Convert.ToInt32(adtPayeeAccount[enmBenefitApplication.benefit_application_id.ToString()]);
        //    lstrPayeeAccountCreatedBy = Convert.ToString(adtPayeeAccount["CREATED_BY"]);
        //    lintPlanID = Convert.ToInt32(adtPayeeAccount[enmPlanBenefitXr.plan_id.ToString()]);
        //    ldtMinDistributionDate = new DateTime(ldtDOB.AddYears(70).AddMonths(6).AddYears(1).Year, 4, 1);
        //    if (!string.IsNullOrEmpty(Convert.ToString(adtPayeeAccount[enmPayeeAccount.benefit_calculation_detail_id.ToString()])))
        //    {
        //        lintBenefitCalcID = Convert.ToInt32(adtPayeeAccount[enmPayeeAccount.benefit_calculation_detail_id.ToString()]);
        //    }
        //    if (lstrPayeeAccountCreatedBy == "CONVERSION")
        //    {
        //    }
        //    //Jugad for testing
        //    //ldtRetirementDate = new DateTime(2010, 1, 1);


        //    ldecAge = busGlobalFunctions.CalculatePersonAge(ldtDOB, iobjSystemManagement.icdoSystemManagement.batch_date);
        //    if (ldecAge >= 65)
        //    {
        //        ldtFromDate = ldtRetirementDate;
        //        ldtToDate = busGlobalFunctions.GetLastDateOfComputationYear(iobjSystemManagement.icdoSystemManagement.batch_date.AddYears(-1).Year);
        //        ldtProcessedFromDate = busGlobalFunctions.GetFirstDateOfComputationYear(iobjSystemManagement.icdoSystemManagement.batch_date.AddYears(-1).Year);
        //        ldtProcessedToDate = busGlobalFunctions.GetLastDateOfComputationYear(iobjSystemManagement.icdoSystemManagement.batch_date.AddYears(-1).Year);

        //        if (lstrPlanCode == busConstant.MPIPP || lstrPlanCode == busConstant.IAP) 
        //        {
        //            if (lstrPlanCode == busConstant.MPIPP && ldtRetirementDate < iobjSystemManagement.icdoSystemManagement.batch_date && ldtFromDate < ldtToDate)
        //            {
        //                lstrSSN = Convert.ToString(adtPayeeAccount[enmPerson.ssn.ToString()]);
        //                //ldtFromDate = Convert.ToDateTime(adtPayeeAccount[enmBenefitApplication.retirement_date.ToString()]);

        //                lstrBenefitTypeValue = Convert.ToString(adtPayeeAccount[enmBenefitApplication.benefit_type_value.ToString()]);
        //                lstrBenefitSubTypeValue = Convert.ToString(adtPayeeAccount[enmBenefitApplicationDetail.benefit_subtype_value.ToString()]);
        //                lstrBenefitOptionValue = Convert.ToString(adtPayeeAccount[enmPlanBenefitXr.benefit_option_value.ToString()]);
        //                //lintBenefitCalcID = Convert.ToInt32(adtPayeeAccount[enmBenefitCalculationHeader.benefit_calculation_header_id.ToString()]);

        //                lbusPayeeAccount.icdoPayeeAccount.iintBenefitCalculationID = lintBenefitCalcID;
        //                lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate = ldtRetirementDate;
        //                lbusPayeeAccount.icdoPayeeAccount.iintBenefitApplicationID = lintBenefitAppID;
        //                lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue = lstrBenefitOptionValue;
        //                lbusPayeeAccount.icdoPayeeAccount.iintPlanId = lintPlanID;

        //                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
        //                string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

        //                SqlParameter[] lsqlParameters = new SqlParameter[7];
        //                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
        //                SqlParameter param2 = new SqlParameter("@FROM_DATE", DbType.DateTime);
        //                SqlParameter param3 = new SqlParameter("@TO_DATE", DbType.DateTime);
        //                SqlParameter param4 = new SqlParameter("@PLANCODE", DbType.String);
        //                SqlParameter param5 = new SqlParameter("@PROCESSED_FROM_DATE", DbType.DateTime);
        //                SqlParameter param6 = new SqlParameter("@PROCESSED_TO_DATE", DbType.DateTime);

        //                SqlParameter returnvalue = new SqlParameter("@RETURN_VALUE", DbType.Int32);
        //                returnvalue.Direction = ParameterDirection.ReturnValue;

        //                param1.Value = lstrSSN;
        //                lsqlParameters[0] = param1;

        //                param2.Value = ldtFromDate;
        //                lsqlParameters[1] = param2;

        //                param3.Value = ldtToDate;
        //                lsqlParameters[2] = param3;

        //                param4.Value = lstrPlanCode;
        //                lsqlParameters[3] = param4;

        //                param5.Value = ldtProcessedFromDate;
        //                lsqlParameters[4] = param5;

        //                param6.Value = ldtProcessedToDate;
        //                lsqlParameters[5] = param6;

        //                lsqlParameters[6] = returnvalue;
        //                //lsqlParameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

        //                DataTable ldtGetHoursForLastComputationYear = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataBetweenTwoDates", lstrLegacyDBConnetion, null, lsqlParameters);
        //                if (Convert.ToInt32(returnvalue.Value) == 1)
        //                {
        //                    lblnLateHoursReported = true;
        //                }

        //                if (ldtGetHoursForLastComputationYear.Rows.Count > 0)
        //                {
        //                    if (ldtGetHoursForLastComputationYear.AsEnumerable().Where(datarow => Convert.ToDecimal(datarow["QUALIFIED_HOURS"]) >= 870).Count() > 0 || lblnLateHoursReported)
        //                    {
        //                        busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
        //                        lbusPerson.icdoPerson.LoadData(adtPayeeAccount);
        //                        ldtToDate = new DateTime(ldtToDate.AddYears(1).Year, 1, 1);

        //                        if (lstrBenefitTypeValue == busConstant.BENEFIT_TYPE_DISABILITY)
        //                        {
        //                            lbusPayeeAccount.CreateFinalCalculationForReEmployedDisabledParticipants(ldtGetHoursForLastComputationYear, adtPayeeAccount, ldtToDate, iobjSystemManagement.icdoSystemManagement.batch_date, false, lblnLateHoursReported);
        //                        }
        //                        else if (lstrBenefitTypeValue == busConstant.BENEFIT_TYPE_RETIREMENT)
        //                        {
        //                            lbusPayeeAccount.ReEvaluateReemployedParticipants(ldtGetHoursForLastComputationYear, adtPayeeAccount, ldtToDate, iobjSystemManagement.icdoSystemManagement.batch_date, null, false, lblnLateHoursReported);
        //                            //lbusPayeeAccount.CreateFinalCalculationForReEmployedParticipants(ldtGetHoursForLastComputationYear, adtPayeeAccount,ldtToDate,iobjSystemManagement.icdoSystemManagement.batch_date,null,false , lblnLateHoursReported);
        //                        }
        //                    }
        //                }
        //            }
        //            else if (lstrPlanCode == busConstant.IAP)
        //            {

        //                if (lstrBenefitTypeValue == busConstant.BENEFIT_TYPE_DISABILITY)
        //                {
        //                    lbusPayeeAccount.CreateFinalCalculationForReEmployedDisabledParticipants(null, adtPayeeAccount, ldtToDate, iobjSystemManagement.icdoSystemManagement.batch_date, false, lblnLateHoursReported);
        //                }
        //                else if (lstrBenefitTypeValue == busConstant.BENEFIT_TYPE_RETIREMENT)
        //                {
        //                    lbusPayeeAccount.ReEvaluateReemployedParticipants(null, adtPayeeAccount, ldtToDate, iobjSystemManagement.icdoSystemManagement.batch_date, null, false, lblnLateHoursReported);

        //                    //lbusPayeeAccount.CreateFinalCalculationForReEmployedParticipants(null, adtPayeeAccount, ldtToDate, iobjSystemManagement.icdoSystemManagement.batch_date,null, false, lblnLateHoursReported);
        //                }
        //            }
        //        }

        //    }
        //}

        #endregion

        #region Resume Benefits Batch
        public void ResumeBenefitsBatch()
        {
            int lintCount = 0;
            int lintTotalCount = 0;

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();


            DataTable ldtbReportTable01 = new DataTable();
            ldtbReportTable01.TableName = "ReportTable01";
          
            ldtbReportTable01.Columns.Add("PAYEE_MPI_PERSON_ID", typeof(string));
            ldtbReportTable01.Columns.Add("PAYEE_NAME", typeof(string));          
            ldtbReportTable01.Columns.Add("PAYEE_ACCOUNT_ID", typeof(int));            
            ldtbReportTable01.Columns.Add("PLAN_NAME", typeof(string));
            ldtbReportTable01.Columns.Add("RESUMPTION_RULE", typeof(string));
            ldtbReportTable01.Columns.Add("REEMPLOYED_AS_OF_DATE", typeof(string));
            ldtbReportTable01.Columns.Add("BATCH_ID", typeof(string));
         
            adtReempResumeBenefitPayeeAccounts = busBase.Select("cdoPayeeAccount.GetReEmployedToResumeBenefits", new object[0]);

            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 1;

            if (adtReempResumeBenefitPayeeAccounts != null)
            {
                Parallel.ForEach(adtReempResumeBenefitPayeeAccounts.AsEnumerable(), po, (ldtReemployed, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "ResumeBenefitsBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    ResumeReEmployedParticipantsBenefits(ldtReemployed, ldtbReportTable01, lobjPassInfo, lintCount, lintTotalCount);

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;
                });
            }

            utlPassInfo.iobjPassInfo = lobjMainPassInfo;

            if (ldtbReportTable01 != null && ldtbReportTable01.Rows.Count > 0)
                CreateExcelReport(ldtbReportTable01, "rptReemploymentResumeBenefitBatch");

        }

        public void ResumeReEmployedParticipantsBenefits(DataRow adtPerson, DataTable adtbReportTable01, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        {
            busPayeeAccount lbusPayeeAccount = null;
            string astrResumptionRule = string.Empty;

            lock (iobjLock)
            {
                aintCount++;
                aintTotalCount++;
                if (aintCount == 100)
                {
                    String lstrMsg = aintTotalCount + " : " + " Records Has Been Processed";
                    PostInfoMessage(lstrMsg);
                    aintCount = 0;
                }
            }
                                   

            autlPassInfo.BeginTransaction();
            try
            {
                lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lbusPayeeAccount.icdoPayeeAccount.LoadData(adtPerson);
                if (Convert.ToInt32(adtPerson[enmPlan.plan_id.ToString().ToUpper()]) == busConstant.MPIPP_PLAN_ID && 
                    lbusPayeeAccount.ResumeBenefits(adtPerson, iobjSystemManagement.icdoSystemManagement.batch_date, ref astrResumptionRule))
                {
                    DataRow dr = adtbReportTable01.NewRow();

                    dr["PAYEE_NAME"] = adtPerson["FULL_NAME"];
                    dr["PAYEE_MPI_PERSON_ID"] = adtPerson[enmPerson.mpi_person_id.ToString().ToUpper()];
                    dr["PAYEE_ACCOUNT_ID"] = adtPerson[enmPayeeAccount.payee_account_id.ToString().ToUpper()];
                    dr["PLAN_NAME"] = adtPerson[enmPlan.plan_code.ToString().ToUpper()];
                    dr["RESUMPTION_RULE"] = astrResumptionRule;
                    dr["BATCH_ID"] = iobjPassInfo.istrUserID;
                    dr["REEMPLOYED_AS_OF_DATE"] = Convert.ToString(adtPerson[enmPayeeAccount.reemployed_flag_as_of_date.ToString().ToUpper()]).IsNullOrEmpty() ?
                                                        string.Empty : Convert.ToDateTime(adtPerson[enmPayeeAccount.reemployed_flag_as_of_date.ToString().ToUpper()]).Date.ToString();
                    adtbReportTable01.Rows.Add(dr);

                    var lvarLocalPayeeAccounts = (from obj in adtReempResumeBenefitPayeeAccounts.AsEnumerable()
                                               where obj.Field<Int32>(enmPlan.plan_id.ToString().ToUpper()) != busConstant.MPIPP_PLAN_ID
                                               && obj.Field<Int32>(enmPerson.person_id.ToString().ToUpper()) == lbusPayeeAccount.icdoPayeeAccount.person_id
                                               select obj);

                    if(lvarLocalPayeeAccounts.Count() > 0)
                    {
                        foreach(DataRow ldrLocalPayeeAccnt in lvarLocalPayeeAccounts.CopyToDataTable().AsEnumerable())
                        {
                            DataRow drlcl = adtbReportTable01.NewRow();
                            drlcl["PAYEE_NAME"] = ldrLocalPayeeAccnt["FULL_NAME"];
                            drlcl["PAYEE_MPI_PERSON_ID"] = ldrLocalPayeeAccnt[enmPerson.mpi_person_id.ToString().ToUpper()];
                            drlcl["PAYEE_ACCOUNT_ID"] = ldrLocalPayeeAccnt[enmPayeeAccount.payee_account_id.ToString().ToUpper()];
                            drlcl["PLAN_NAME"] = ldrLocalPayeeAccnt[enmPlan.plan_code.ToString().ToUpper()];
                            drlcl["RESUMPTION_RULE"] = astrResumptionRule;
                            drlcl["BATCH_ID"] = iobjPassInfo.istrUserID;
                            drlcl["REEMPLOYED_AS_OF_DATE"] = Convert.ToString(ldrLocalPayeeAccnt[enmPayeeAccount.reemployed_flag_as_of_date.ToString().ToUpper()]).IsNullOrEmpty() ?
                                                        string.Empty : Convert.ToDateTime(ldrLocalPayeeAccnt[enmPayeeAccount.reemployed_flag_as_of_date.ToString().ToUpper()]).Date.ToString();
                            adtbReportTable01.Rows.Add(drlcl);
                        }
                    }
                }



                autlPassInfo.Commit();
            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    String lstrMsg = "Error while Executing Batch,Error Message For Payee Account ID: " + lbusPayeeAccount.icdoPayeeAccount.payee_account_id + ":" + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();
            }
        }

        #endregion

        #region Integration Re-Employment Batch

        public void UpadateParticipantAsReEmployedFromEADB()
        {
            try
            {
                DataTable ldtReemployedPayeeAccounts = busBase.Select("cdoPerson.GetRetPayeeAccounts", new object[0]);
                if (ldtReemployedPayeeAccounts.IsNotNull() && ldtReemployedPayeeAccounts.Rows.Count > 0)
                {
                    utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                    string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
                    SqlParameter[] lsqlParameters = new SqlParameter[2];

                    #region Reemployment Changes post production(To change batch to monthly from daily) : 5th Aug 2013 Mail On : 31st July 2013
                    DateTime ldtPreviousMonth = new DateTime();
                    ldtPreviousMonth = iobjSystemManagement.icdoSystemManagement.batch_date;


                    //Changes need to be do done in store procedure to pass these two dates.
                    DateTime ldtFromDate = new DateTime();
                    DateTime ldtToDate = new DateTime();

                    ldtPreviousMonth = busGlobalFunctions.GetLastPayrollDayOfMonth(iobjSystemManagement.icdoSystemManagement.batch_date.Year, iobjSystemManagement.icdoSystemManagement.batch_date.Month);
                    if(ldtPreviousMonth.Month == iobjSystemManagement.icdoSystemManagement.batch_date.Month)
                    {
                        ldtPreviousMonth = new DateTime(ldtPreviousMonth.AddMonths(-1).Year, ldtPreviousMonth.AddMonths(-1).Month, 01);
                    }
                    else 
                    {
                        ldtPreviousMonth = new DateTime(ldtPreviousMonth.Year,ldtPreviousMonth.Month,01);
                    }
                    ldtFromDate = busGlobalFunctions.GetFirstPayrollDayOfMonth(ldtPreviousMonth.Year,ldtPreviousMonth.Month);
                    ldtToDate = busGlobalFunctions.GetLastPayrollDayOfMonth(ldtPreviousMonth.Year,ldtPreviousMonth.Month);


                    #endregion

                    SqlParameter param1 = new SqlParameter("@FROM_DATE", DbType.DateTime);
                    SqlParameter param2 = new SqlParameter("@TO_DATE", DbType.DateTime);

                    param1.Value = ldtFromDate;
                    lsqlParameters[0] = param1;

                    param2.Value = ldtToDate;
                    lsqlParameters[1] = param2;

                    DataTable ldtbHoursProcessedLastDay = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetHoursProcessedPreviousDay", astrLegacyDBConnetion, null, lsqlParameters);
                    if (ldtbHoursProcessedLastDay.IsNotNull() && ldtbHoursProcessedLastDay.Rows.Count > 0)
                    {
                        #region JOINING EADB & LOCAL DATA SET
                        var collection = (from t1 in ldtReemployedPayeeAccounts.AsEnumerable()
                                          join t2 in ldtbHoursProcessedLastDay.AsEnumerable()
                                             on Convert.ToString(t1["SSN"]).Trim() equals Convert.ToString(t2["SSN"]).Trim()
                                          where Convert.ToDateTime(t2["todate"]) > Convert.ToDateTime(t1[enmBenefitApplication.retirement_date.ToString()]).AddDays(6 - Convert.ToInt32(Convert.ToDateTime(t1["RETIREMENT_DATE"]).DayOfWeek))
                                          select
                                        new
                                        {
                                            payee_account_id = t1["payee_account_id"],
                                            hoursworked = t2["hoursworked"],
                                            fromdate = t2["fromdate"]
                                        }).Distinct();


                        DataTable ldtOpusInformation = new DataTable();
                        ldtOpusInformation.Columns.Add("payee_account_id", typeof(int));
                        ldtOpusInformation.Columns.Add("fromdate", typeof(DateTime));
                        ldtOpusInformation.Columns.Add("hoursworked", typeof(decimal));
                        //ldtReemployedPayeeAccounts.Clear();

                        if (collection.IsNotNull() && collection.ToList().Count() > 0)
                        {
                            foreach (var item in collection)
                            {
                                DataRow ldtRow = ldtOpusInformation.NewRow();
                                ldtRow["payee_account_id"] = item.payee_account_id;
                                ldtRow["hoursworked"] = item.hoursworked;
                                ldtRow["fromdate"] = item.fromdate;
                                ldtOpusInformation.Rows.Add(ldtRow);
                            }
                        }
                        #endregion

                        #region Set Flag & Date
                        if (ldtOpusInformation.Rows.Count > 0)
                        {
                            string lstrFromDate = string.Empty;
                            decimal ldecHours = new decimal();

                            DataTable ldtReemployedHours = new DataTable();
                            ldtReemployedHours.Columns.Add("payee_account_id", Type.GetType("System.Int32"));
                            ldtReemployedHours.Columns.Add("hoursworked", Type.GetType("System.Decimal"));
                            ldtReemployedHours.Columns.Add("fromdate", Type.GetType("System.DateTime"));
                            var ReempPayeeAccount = from row in ldtOpusInformation.AsEnumerable()
                                                    group row by new { ID = row.Field<Int32>("payee_account_id") } into grp
                                                    select new
                                                    {
                                                        payee_account_id = grp.Key.ID,
                                                        hoursworked = grp.Sum(r => r.Field<Decimal>("hoursworked")),
                                                        fromdate = grp.Min(r => r.Field<DateTime>("fromdate"))
                                                    };
                            foreach (var row in ReempPayeeAccount)
                            {
                                DataRow ldtRow = ldtReemployedHours.NewRow();
                                ldtRow["payee_account_id"] = row.payee_account_id;
                                ldtRow["hoursworked"] = row.hoursworked;
                                ldtRow["fromdate"] = row.fromdate;
                                ldtReemployedHours.Rows.Add(ldtRow);
                            }
                            cdoPayeeAccount lcdoPayeeAccount;
                            ArrayList arrList = new ArrayList();
                            foreach (DataRow ldtRow in ldtReemployedHours.Rows)
                            {
                                lstrFromDate = string.Empty;
                                lstrFromDate = Convert.ToString(ldtRow["fromdate"]);

                                if (!string.IsNullOrEmpty(lstrFromDate) && !string.IsNullOrEmpty(Convert.ToString(ldtRow["hoursworked"])))
                                {
                                    ldecHours = Convert.ToDecimal(ldtRow["hoursworked"]);
                                    if (ldecHours > 0)
                                    {
                                        lcdoPayeeAccount = new cdoPayeeAccount();
                                        lcdoPayeeAccount.LoadData(ldtRow);
                                        if (!arrList.Contains(lcdoPayeeAccount.payee_account_id))
                                        {
                                            lcdoPayeeAccount.reemployed_flag_from_eadb = busConstant.FLAG_YES;
                                            lcdoPayeeAccount.reemployed_flag_as_of_date = Convert.ToDateTime(ldtRow["fromdate"]);
                                            lcdoPayeeAccount.update_seq = Convert.ToInt32(ldtReemployedPayeeAccounts.AsEnumerable().Where(item => Convert.ToInt32(item["payee_account_id"]) == lcdoPayeeAccount.payee_account_id).FirstOrDefault()["update_seq"]);
                                            lcdoPayeeAccount.Update();
                                            arrList.Add(lcdoPayeeAccount.payee_account_id);

                                            String lstrMsg = "Hours reported after retirement for Payee Account ID : " + lcdoPayeeAccount.payee_account_id + "." ;
                                            PostInfoMessage(lstrMsg);

                                        }
                                    }
                                }

                            }
                        }
                        #endregion

                    }
                }
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Error Occured with Message = " + ex.Message, "ERR", istrProcessName);
            }
        }


        #endregion

        public void CreateReEmployedCorrespondence(string astrRule, ArrayList aarrResult, Hashtable ahtbQueryBkmarks, string astrUserID, int aintUserSerialID)
        {
            if (astrRule == busConstant.REEMPLOYMENT_RULE_1)
            {
                this.CreateCorrespondence(busConstant.REEMPLOYMENT_WITHIN_TWO_MONTHS_OF_RETIREMENT, astrUserID, aintUserSerialID, aarrResult, ahtbQueryBkmarks);
            }
            else if (astrRule == busConstant.REEMPLOYMENT_RULE_2)
            {
                this.CreateCorrespondence(busConstant.RE_EMPLOYMENT_APPROACHING_UNREDUCED_LIMIT, astrUserID, aintUserSerialID, aarrResult, ahtbQueryBkmarks);

            }
            else if (astrRule == busConstant.REEMPLOYMENT_RULE_3)
            {
                //this.CreateCorrespondence(busConstant.RE_EMPLOYMENT_OVERPAYMENT_NOTICE_RE_PAYMENT_LETTER, astrUserID, aintUserSerialID, aarrResult, ahtbQueryBkmarks);
                //this.CreateCorrespondence(busConstant.RE_EMPLOYMENT_APPROACHING_UNREDUCED_LIMIT, astrUserID, aintUserSerialID, aarrResult, ahtbQueryBkmarks);

            }
            else if (astrRule == busConstant.REEMPLOYMENT_RULE_4)
            {
                this.CreateCorrespondence(busConstant.SSA_DISABILITY_STOP_OVERPAYMENT, astrUserID, aintUserSerialID, aarrResult, ahtbQueryBkmarks);
            }
            else if (astrRule == busConstant.REEMPLOYMENT_RULE_5)
            {
                //this.CreateCorrespondence(busConstant.RE_EMPLOYMENT_OVERPAYMENT_NOTICE_RE_PAYMENT_LETTER, astrUserID, aintUserSerialID, aarrResult, ahtbQueryBkmarks);
                //this.CreateCorrespondence(busConstant.RE_EMPLOYMENT_APPROACHING_UNREDUCED_LIMIT, astrUserID, aintUserSerialID, aarrResult, ahtbQueryBkmarks);
            }
        }
        
    }
}
