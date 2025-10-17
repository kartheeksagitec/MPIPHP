// -----------------------------------------------------------------------
// <copyright file="busSSADisabilityReCertificationBatch.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MPIPHPJobService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MPIPHPJobService;
    using Sagitec.BusinessObjects;
    using Sagitec.DBUtility;
    using System.Data;
    using System.Threading.Tasks;
    using MPIPHP.BusinessObjects;
    using System.Collections.ObjectModel;
    using MPIPHP.CustomDataObjects;
    using Sagitec.ExceptionPub;
    using System.Collections;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class busSSADisabilityReCertificationBatch : busBatchHandler
    {
        private object iobjLock = null;
        int iintCount = busConstant.ZERO_INT;
        int iintTotalCount = busConstant.ZERO_INT;

        public busSSADisabilityReCertificationBatch()
        {
        }

        public override void Process()
        {
            busBase lobjBase = new busBase();
            base.Process();
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            iobjLock = new object();

            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions lpoParallelOptions = new ParallelOptions();
            lpoParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;
            Collection<busPayeeAccount> lclbPayeeAccounts;
            DataSet idtbstSSADisability = new DataSet();
            createTableDesignForSSA(); createTableDesignForSSASuspending();

            DataTable ldtblPayeeAccountsForSuspension = busBase.Select("cdoBenefitApplication.GetDataToSuspendPayeeAccount", new object[0] { });
            if (ldtblPayeeAccountsForSuspension.Rows.Count > 0)
            {
                lclbPayeeAccounts = new Collection<busPayeeAccount>();
                lclbPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(ldtblPayeeAccountsForSuspension, "icdoPayeeAccount");

                Parallel.ForEach(lclbPayeeAccounts, lbusPayeeAccount =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "SSADisabilityReCertificationBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    SuspendPayeeAccounts(lbusPayeeAccount, lobjPassInfo);

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;
                });
            }

            lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;

            lpoParallelOptions = new ParallelOptions();
            lpoParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

            DataTable ldtblPayeeAccountsForProofOfDisability = busBase.Select("cdoBenefitApplication.GetPayeeAccountForProofOfDisability", new object[0] { });
            if (ldtblPayeeAccountsForProofOfDisability.Rows.Count > 0)
            {
                lclbPayeeAccounts = new Collection<busPayeeAccount>();
                lclbPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(ldtblPayeeAccountsForProofOfDisability, "icdoPayeeAccount");

                Parallel.ForEach(lclbPayeeAccounts, lbusPayeeAccount =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "SSADisabilityReCertificationBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    GenerateProofOfContinuousDisabilityLetter(lbusPayeeAccount, lobjPassInfo);

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;
                });
            }
            //PROD PIR 814
            MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + "Correspondence\\Generated\\", iobjSystemManagement.icdoSystemManagement.base_directory + "Reports\\SSADisability\\");

            idtbstSSADisability.Tables.Add(idtbSSADisabilitySuspending.Copy());
            idtbstSSADisability.Tables[0].TableName = "ReportTable01";
            idtbstSSADisability.Tables.Add(idtbSSADisability.Copy());
            idtbstSSADisability.Tables[1].TableName = "ReportTable02";
            idtbstSSADisability.DataSetName = "rptSSADisabilityBatchReport";
            this.CreatePDFReport(idtbstSSADisability, "rptSSADisabilityBatchReport", busConstant.MPIPHPBatch.GENERATED_SSADISABILITY_REPORT_PATH);


            lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        }


        private void GenerateProofOfContinuousDisabilityLetter(busPayeeAccount abusPayeeAccount, utlPassInfo autlPassInfo)
        {

            lock (iobjLock)
            {
                iintCount++;
                iintTotalCount++;
                if (iintCount == 10)
                {
                    String lstrMsg = iintTotalCount + " : " + " Records Has Been Processed";
                    PostInfoMessage(lstrMsg);
                    iintCount = 0;
                }
            }
            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            autlPassInfo.BeginTransaction();
            try
            {
                ArrayList aarrResult = new ArrayList();
                Hashtable ahtbQueryBkmarks = new Hashtable();

                abusPayeeAccount.LoadBenefitDetails();

                lbusPerson.FindPerson(abusPayeeAccount.icdoPayeeAccount.person_id);
                lbusPerson.LoadPersonAddresss();
                //lbusPerson.LoadPersonContacts();
                //lbusPerson.LoadCorrAddress();

                //Generate Corr Proof of Continuous Disability Letter

                busDisabilityApplication lbusDisabilityApplication = new busDisabilityApplication();
                busBenefitApplicationDetail lbusBenefitApplicationDetail = new busBenefitApplicationDetail();
                if (lbusBenefitApplicationDetail.FindBenefitApplicationDetail(abusPayeeAccount.icdoPayeeAccount.benefit_application_detail_id))
                {
                    if (lbusDisabilityApplication.FindBenefitApplication(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_id))
                    {
                        lbusDisabilityApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lbusDisabilityApplication.ibusPerson.FindPerson(lbusDisabilityApplication.icdoBenefitApplication.person_id);
                        aarrResult.Add(lbusDisabilityApplication);
                    }
                }

                decimal ldecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusDisabilityApplication.ibusPerson.icdoPerson.idtDateofBirth, lbusDisabilityApplication.icdoBenefitApplication.retirement_date);

                if (ldecAge < 65.00M)
                {
                    lock (iobjLock)
                    {
                        this.CreateCorrespondence(busConstant.PROOF_OF_SSA_CONTINUOUS_DISABILITY, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE); //PROD PIR 814
                        busDisabilityBenefitHistory lbusDisabilityBenefitHistory = new busDisabilityBenefitHistory { icdoDisabilityBenefitHistory = new cdoDisabilityBenefitHistory() };
                        lbusDisabilityBenefitHistory.icdoDisabilityBenefitHistory.benefit_application_id = abusPayeeAccount.icdoPayeeAccount.iintBenefitApplicationID;
                        lbusDisabilityBenefitHistory.icdoDisabilityBenefitHistory.disability_cont_letter_date = iobjSystemManagement.icdoSystemManagement.batch_date;
                        lbusDisabilityBenefitHistory.icdoDisabilityBenefitHistory.sent = busConstant.FLAG_YES;
                        lbusDisabilityBenefitHistory.icdoDisabilityBenefitHistory.received = busConstant.FLAG_NO;
                        lbusDisabilityBenefitHistory.icdoDisabilityBenefitHistory.Insert();

                        #region For report changes
                        DataRow dr = idtbSSADisability.NewRow();
                        dr["MPI_PERSON_ID"] = abusPayeeAccount.icdoPayeeAccount.istrMPID;
                        dr["FIRST_NAME"] = abusPayeeAccount.icdoPayeeAccount.istrFirsttName;
                        dr["LAST_NAME"] = abusPayeeAccount.icdoPayeeAccount.istrLastName;
                        dr["RETIREMENT_DATE"] = abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate;
                        dr["AGE"] = abusPayeeAccount.icdoPayeeAccount.idecAge;
                        dr["PAYEE_ACCOUNT_ID"] = abusPayeeAccount.icdoPayeeAccount.payee_account_id;
                        idtbSSADisability.Rows.Add(dr);
                        #endregion

                        //String lstrMsg = " " + lbusPerson.icdoPerson.mpi_person_id + ":" + e.ToString();
                        String lstrMsg = "Proof of SSA Continuous Disability Has been sent to Participant with MPID : " + lbusDisabilityApplication.ibusPerson.icdoPerson.mpi_person_id;
                        PostInfoMessage(lstrMsg);
                    }
                }

                autlPassInfo.Commit();
            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    String lstrMsg = "Error while Executing Batch,Error Message For MPID " + lbusPerson.icdoPerson.mpi_person_id + ":" + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();

            }
        }

        private void SuspendPayeeAccounts(busPayeeAccount abusPayeeAccount, utlPassInfo autlPassInfo)
        {

            lock (iobjLock)
            {
                iintCount++;
                iintTotalCount++;
                if (iintCount == 10)
                {
                    String lstrMsg = iintTotalCount + " : " + " Records Has Been Processed";
                    PostInfoMessage(lstrMsg);
                    iintCount = 0;
                }
            }

            autlPassInfo.BeginTransaction();
            try
            {
                lock (iobjLock)
                {
                    busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    Hashtable ahtbQueryBkmarks = new Hashtable();

                    lbusPerson.FindPerson(abusPayeeAccount.icdoPayeeAccount.person_id);
                    abusPayeeAccount.LoadPayeeAccountStatuss();

                    #region PROD PIR 814
                    string lstrMsg = string.Empty;
                    //Create Workflow. 
                    //busWorkflowHelper.InitializeWorkflow(busConstant.PROCESS_PAYEE_ACCOUNT, lbusPerson.icdoPerson.person_id, 0, abusPayeeAccount.icdoPayeeAccount.payee_account_id, ahtbQueryBkmarks);
                    // if (abusPayeeAccount.istrPayeeStatus != busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED)
                    if (abusPayeeAccount.iclbPayeeAccountStatus.Count() > 0 && abusPayeeAccount.iclbPayeeAccountStatus != null &&
                        abusPayeeAccount.iclbPayeeAccountStatus.FirstOrDefault().icdoPayeeAccountStatus.status_value != busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED
                        && abusPayeeAccount.iclbPayeeAccountStatus.FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value != busConstant.Suspension_Reason_For_Disability)
                    {
                        cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                        lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(abusPayeeAccount.icdoPayeeAccount.payee_account_id);
                        lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED;
                        lcdoPayeeAccountStatus.suspension_status_reason_value = busConstant.Suspension_Reason_For_Disability;
                        lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                        lcdoPayeeAccountStatus.Insert();

                        //145993
                        cdoNotes lcdoNotes = new cdoNotes();
                        lcdoNotes.person_id = lbusPerson.icdoPerson.person_id;
                        lcdoNotes.notes = "Payee account suspended for SSA Disability Re-certification non-compliance";
                        lcdoNotes.created_by = iobjPassInfo.istrUserID;
                        lcdoNotes.created_date = DateTime.Now;
                        lcdoNotes.modified_by = iobjPassInfo.istrUserID;
                        lcdoNotes.modified_date = DateTime.Now;
                        lcdoNotes.Insert();
                        
                        lstrMsg = "Payee Account with Payee Account Id :" + abusPayeeAccount.icdoPayeeAccount.payee_account_id + " suspended.";
                        PostInfoMessage(lstrMsg);
                        #region For report changes
                        DataRow dr = idtbSSADisabilitySuspending.NewRow();
                        dr["MPI_PERSON_ID"] = abusPayeeAccount.icdoPayeeAccount.istrMPID;
                        dr["FIRST_NAME"] = abusPayeeAccount.icdoPayeeAccount.istrFirsttName;
                        dr["LAST_NAME"] = abusPayeeAccount.icdoPayeeAccount.istrLastName;
                        dr["RETIREMENT_DATE"] = abusPayeeAccount.icdoPayeeAccount.idtRetireMentDate;
                        dr["AGE"] = abusPayeeAccount.icdoPayeeAccount.idecAge;
                        dr["PAYEE_ACCOUNT_ID"] = abusPayeeAccount.icdoPayeeAccount.payee_account_id;
                        idtbSSADisabilitySuspending.Rows.Add(dr);
                        #endregion
                    }
                    #endregion

                }

                autlPassInfo.Commit();
            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    String lstrMsg = "Error while Executing Batch,Suspend Payee - Error Message For Payee Account Id  " + abusPayeeAccount.icdoPayeeAccount.payee_account_id + ":" + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();

            }
        }

        /// <summary>
        /// Create Table for Report which contains
        /// Proof of SSA Continuous Disability Has been sent to Participant
        /// PROD PIR 814
        /// </summary>
        public DataTable createTableDesignForSSA()
        {
            idtbSSADisability = new DataTable();
            idtbSSADisability.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            idtbSSADisability.Columns.Add(new DataColumn("FIRST_NAME", typeof(string)));
            idtbSSADisability.Columns.Add(new DataColumn("LAST_NAME", typeof(string)));
            idtbSSADisability.Columns.Add(new DataColumn("RETIREMENT_DATE", typeof(DateTime)));
            idtbSSADisability.Columns.Add(new DataColumn("AGE", typeof(decimal)));
            idtbSSADisability.Columns.Add(new DataColumn("PAYEE_ACCOUNT_ID", typeof(int)));
            return idtbSSADisability;
        }
        public DataTable createTableDesignForSSASuspending()
        {
            idtbSSADisabilitySuspending = new DataTable();
            idtbSSADisabilitySuspending.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            idtbSSADisabilitySuspending.Columns.Add(new DataColumn("FIRST_NAME", typeof(string)));
            idtbSSADisabilitySuspending.Columns.Add(new DataColumn("LAST_NAME", typeof(string)));
            idtbSSADisabilitySuspending.Columns.Add(new DataColumn("RETIREMENT_DATE", typeof(DateTime)));
            idtbSSADisabilitySuspending.Columns.Add(new DataColumn("AGE", typeof(decimal)));
            idtbSSADisabilitySuspending.Columns.Add(new DataColumn("PAYEE_ACCOUNT_ID", typeof(int)));
            return idtbSSADisability;
        }
        public DataTable idtbSSADisability { get; set; }
        public DataTable idtbSSADisabilitySuspending { get; set; }
    }
}
