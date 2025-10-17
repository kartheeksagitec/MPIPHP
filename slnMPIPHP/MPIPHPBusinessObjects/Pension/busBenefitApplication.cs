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
using Sagitec.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.SqlTypes;
using NeoSpin.BusinessObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary> 
    /// Class MPIPHP.BusinessObjects.busBenefitApplication:
    /// Inherited from busBenefitApplicationGen, the class is used to customize the business object busBenefitApplicationGen.
    /// </summary>
    [Serializable]
    public class busBenefitApplication : busBenefitApplicationGen
    {

        #region Properties for correspondence
        // Code - AARTI
        public DateTime dtSixtyDaysPriorDate { get; set; }
        public DateTime dtThirtyDaysPriorDate { get; set; }
        public int iintVestedYears { get; set; }
        //Ticket 85016 - Correspondence RETR-0054
        public int iintQualifiedYears { get; set; }
        //Ticket 85016 - Correspondence RETR-0054
        public decimal ldecTotalCreditedHrs { get; set; }
        public decimal idecVestedHours { get; set; }
        public string istrRetirementType { get; set; } //Added for populating in Cor        
        public string istrRetirementDt { get; set; }
        public string istrRetPktDeadlineDt { get; set; }// PIR-583
        public string istrPriorToRetirement { get; set; }
        public DateTime istrDtRetirementDt { get; set; } //For PIR 584

        public string istrSixtyDaysPriorDate { get; set; }
        public string istrThirtyDaysPriorDate { get; set; }
        public string istrOneDayPriorRtDate { get; set; }
        public string istrCurrentDate { get; set; }
        public bool iblnSetDateFlag { get; set; }
        public bool iblnDisConvDate { get; set; }
        public string istrWithDrwlDate { get; set; }
        public string istrMinimumDistributionDate { get; set; }
        public bool iblnIsOnsetDateLessThanRetrDate { get; set; }
        public DateTime idateRetirement { get; set; }
        public bool iblnWithdrawalForAlternatePayee { get; set; }
        public DateTime idueDate { get; set; }
        public string istrDueDate { get; set; }
        public busRetirementApplication ibusRetirementApplication { get; set; }
        public int iintRetrDateYear { get; set; }
        public string istrBenefitTypeDescription { get; set; }
        public DateTime idtRetirementDate { get; set; }
        public DateTime idtWithdrawalDate { get; set; }
        public string istrPlanDesc { get; set; }
        public DateTime idtDayBeforeWidrwlDate { get; set; }
        public DateTime idtDayOneOfNextMonth { get; set; }
        public string istrDayOneOfNextMonth { get; set; }
        public string istrDisabilityOnsetDate { get; set; }
        public string istrDisabilityConvDate { get; set; }
        public string istrApplicationReceivedDate { get; set; }
        public string istrNextBenefitPaymentDate { get; set; }
        public string istrIsUSA { get; set; }
        //WIDRWL-0007
        public string istrSubPlanDesc { get; set; }
        public string istrIsSpecialAcnt { get; set; }
        public string istrIsIAPSpecial { get; set; }

        public string istrIsIAPOptionFlag { get; set; }

        public bool iblnIAPFactor { get; set; }//PIR 1002
        public string istrRetirementDate { get; set; }//PIR 1002

        //DIS-0008
        public DateTime idtNextBenefitPaymentDate { get; set; }

        //WIDRWL-0008
        public decimal idecNonVestedEE { get; set; }
        public decimal idecNonVestedEEInterest { get; set; }
        public decimal idecUVHPAmount { get; set; }
        public decimal idecUVHPInterest { get; set; }

        public decimal idecTOTALInterest { get; set; }
        public decimal idecTotalEEUVHP { get; set; }

        public string istrBoth { get; set; }
        public string istrEEFlag { get; set; }
        public string istrUVHPFlag { get; set; }

        public decimal idecCovidIAP2018BalanceAmt { get; set; }
        public decimal idecCovidIAPMaxAllowedWithdrawalAmt { get; set; }
        public string istrPhoneAreaCode { get; set; }
        public string istrPhoneNumber { get; set; }
        public string istrGender { get; set; }

        public string istrPersonEmailID { get; set; }
        /// <summary>
        /// Payee Account Exist in Approved or Reviwed Status
        /// </summary>
        public Collection<busPayeeAccount> iclbPayeeAccount { get; set; }
        public Collection<busBenefitApplication> iclbDocumentsReceived { get; set; }
        public Collection<busBenefitApplication> iclbDocumentsPending { get; set; }

        public Collection<busBenefitCalculationHeader> iclbBenefitCalculationHeader { get; set; }

        public busDocumentProcessCrossref ibusDocumentProcessCrossref { get; set; }

        public Collection<busBenefitApplicationAuditingChecklist> iclbBenefitApplicationAuditingChecklist { get; set; }

        public string istrIsParticipant { get; set; }
        # endregion Properties for correspondence

        public busBenefitApplication()
        {
            LoadEligibiltyRules();
        }

        //ABHISHEK 
        //CAUTION DO NOT DELETE COMMENTED CODE -LOT OF COMMENTED CODE HAS NOT BEEN CLEANED UP RELATED TO NON-AFFLIATE ELIGBILITY SINCE CLIENT MIGHT COME BACK WITH IT. WILL CLEAN UP EVENTUALLY AFTER UAT 01
        # region Properties
        public DateTime idtLatestWithdrawalDate { get; set; }
        public DateTime idtEarliestRetirementDate { get; set; }

        //Code-Abhishek      
        public bool iblnEligbile4MPIBenefitPreDeath { get; set; }
        public bool iblnEligbile4L600BenefitPreDeath { get; set; }
        public bool iblnEligbile4L666BenefitPreDeath { get; set; }
        public bool iblnEligbile4L700BenefitPreDeath { get; set; }
        public bool iblnEligbile4L161BenefitPreDeath { get; set; }
        public bool iblnEligible4IAPBenefitPreDeath { get; set; }
        public bool iblnEligible4L52BenefitPreDeath { get; set; }

        public bool iblnLocalPlanExists4VestingCheck { get; set; }

        public int lintLateHoursCount { get; set; }
        //public Collection<cdoDummyWorkData> iclbWorkData4RetirementYearMPIPP { get; set; } //CAUTION - We only need this for Accrued Benefit Calculation and NOT for VESTING and ELIGBILITY

        public Collection<busDisabilityBenefitHistory> iclbDisabilityBenefitHistory { get; set; }
        public busPersonAccountEligibility ibusTempPersonAccountEligibility { get; set; } //Please do not use this property anywhere on the screen

        public Collection<cdoDummyWorkData> aclbPersonWorkHistory_MPI { get; set; }
        public Collection<cdoDummyWorkData> aclbPersonWorkHistory_IAP { get; set; }
        public Collection<cdoDummyWorkData> iclbOnlyMPI_WorkHistory { get; set; }
        public Collection<cdoDummyWorkData> aclbReEmployedWorkHistory { get; set; }

        public DataRow[] idrWeeklyWorkData { get; set; }

        public Collection<busPayeeAccountStatus> iclbPayeeAccountStatusByApplication { get; set; }

        public decimal idecTotalHoursTillLatestWithdrawalDate { get; set; }
        public decimal Local52_PensionCredits { get; set; }
        public decimal Local161_PensionCredits { get; set; }
        public decimal Local600_PensionCredits { get; set; }
        public decimal Local666_PensionCredits { get; set; }
        public decimal Local700_PensionCredits { get; set; }
        public decimal Local52_RetirementCredits { get; set; }
        public decimal Local161_RetirementCredits { get; set; }

        public bool QualifiedSpouseExistsForPlan { get; set; }
        public bool QualifiedSpouseExists { get; set; }
        public bool EligibileforL52Spl { get; set; }
        public bool EligibileforL161Spl { get; set; }
        public bool Eligibile4IAP { get; set; }
        public bool Eligibile4MPI { get; set; }
        public bool lbl4PersonOverviewSummary { get; set; }

        Collection<cdoBenefitProvisionEligibility> iclbAllVestingRules { get; set; }

        //public Collection<cdoDummyWorkData> aclbPersonWorkHistory_Local52 { get; set; }
        //public Collection<cdoDummyWorkData> aclbPersonWorkHistory_Local161 { get; set; } 
        //public Collection<cdoDummyWorkData> aclbPersonWorkHistory_Local600 { get; set; }
        //public Collection<cdoDummyWorkData> aclbPersonWorkHistory_Local666 { get; set; } 
        //public Collection<cdoDummyWorkData> aclbPersonWorkHistory_Local700 { get; set; }
        public Collection<cdoBenefitProvisionEligibility> iclbAllEligibilityRules { get; set; }

        private DataTable ldtPersonWorkHistory_MPI { get; set; }
        private DataTable ldtPersonWorkHistory_IAP { get; set; }

        private DataTable ldtLateHoursCount { get; set; }

        public Collection<string> Eligible_Plans { get; set; }
        public Collection<string> iclbEligiblePlans { get; set; }
        public bool NotEligible { get; set; }
        public bool lblPrerequisites_Set { get; set; }
        private Collection<int> ForfeitureYearsCollectionMPI = new Collection<int>();
        private Collection<int> ForfeitureYearsCollectionIAP = new Collection<int>();
        private Collection<int> ForfeitureYearsCollectionMPIPreMerger = new Collection<int>();

        public Collection<cdoPlanBenefitXr> lclcL52BenOptionsPreDeath { get; set; }

        public static string astrLegacyDBConnetion { get; set; }

        private ArrayList larrYears_MPI = new ArrayList();
        private ArrayList larrYears_IAP = new ArrayList();

        public bool SurvivorEntitled4MorePlans { get; set; }

        public Collection<cdoPlan> lclbBenPlans { get; set; }

        public busQdroApplication ibusQdroApplication { get; set; }

        public busPerson ibusParticipant { get; set; }
        public busPerson ibusAlternatePayee { get; set; }
        bool possibleSSNflag;
        //Code-Abhishek

        public decimal idecAge { get; set; }

        //Added For Death Calculation which Rule no. to apply according to Test Case
        public string istrLocal52RuleForDeathCalculation { get; set; }
        public Collection<busNotes> iclbNotes { get; set; }
        public string istrNewNotes { get; set; }
        public decimal idecAgeAtDeath { get; set; }
        public decimal idecAgeAtL52Merger { get; set; }

        public int aintBenefitDetailsPlanID { get; set; }

        //public string istrIsEligibleForIAP { get; set; }
        #region Properties for disability Conversion
        public busRetirementApplication ibusEarlyRetirementApplication { get; set; }
        public busPayeeAccount ibusPayeeAccount { get; set; }
        public bool iblnDisabilityConversion { get; set; }

        #endregion


        #region Properties for WorkFlow
        public bool L52Plan_Selected { get; set; }
        public bool L161Plan_Selected { get; set; }
        public bool L600Plan_Selected { get; set; }
        public bool L700Plan_Selected { get; set; }
        public bool L666Plan_Selected { get; set; }
        public bool MPIPPPlan_Selected { get; set; }
        public bool IAPPlan_Selected { get; set; }
        public int MPI_Final_Calculation_Id { get; set; }
        public int IAP_Final_Calculation_Id { get; set; }
        public int L52_Final_Calculation_Id { get; set; }
        public int L161_Final_Calculation_Id { get; set; }
        public int L600_Final_Calculation_Id { get; set; }
        public int L700_Final_Calculation_Id { get; set; }
        public int L666_Final_Calculation_Id { get; set; }

        #endregion

        public bool iblnCheckForChildSupport { get; set; }
        public int iintIsManager { get; set; } //for pir-522

        //PIR-799
        public Collection<busRetirementEligiblePlans> iclbRetirementEligiblePlans { get; set; }
        public busBenefitApplicationEligiblePlans ibusBenefitApplicationEligiblePlans { get; set; }
        public cdoBenefitApplicationEligiblePlans icdoBenefitApplicationEligiblePlans { get; set; }

        public bool iblnIsRtmtAppFirstTimeSaved { get; set; }

        //PIR-857
        #region pir_857
        public string istrRecalVestingFlg { get; set; }

        public DateTime idtVestingDtIAP { get; set; }
        public DateTime idtForfeitureDtIAP { get; set; }
        public DateTime idtVestingDtMPI { get; set; }
        public DateTime idtForfeitureDtMPI { get; set; }
        public DateTime idtVestingDtIAPaftrFlg { get; set; }
        public DateTime idtForfeitureDtIAPaftrFlg { get; set; }
        public DateTime idtVestingDtMPIaftrFlg { get; set; }
        public DateTime idtForfeitureDtMPIaftrFlg { get; set; }
        #endregion

        //PIR-811
        public bool iblnIsPendingProcessQDROApplicationWorkflow { get; set; }

        # endregion Properties

        # region overriden Methods

        public override busBase GetCorPerson()
        {
            return this.ibusPerson;
        }


        public void LoadDisabilityBenefitHistory()
        {
            DataTable ldtbDisBenHistoryDetails = Select("cdoDisabilityBenefitHistory.GetDisBenHistoryDetails", new object[1] { this.icdoBenefitApplication.benefit_application_id });
            iclbDisabilityBenefitHistory = GetCollection<busDisabilityBenefitHistory>(ldtbDisBenHistoryDetails, "icdoDisabilityBenefitHistory");
        }

        public override void LoadBenefitApplicationDetails()
        {
            base.LoadBenefitApplicationDetails();
            foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in iclbBenefitApplicationDetail)
            {
                DataTable ldtbPlanBen = Select("cdoBenefitApplicationDetail.GetPlan&BenefitFromID", new object[1] { lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id });
                if (ldtbPlanBen.Rows.Count > 0)
                {
                    DataRow ldtrRow = ldtbPlanBen.Rows[0];
                    lbusBenefitApplicationDetail.istrPlanName = Convert.ToString(ldtrRow[enmPlan.plan_name.ToString()]);
                    lbusBenefitApplicationDetail.iintPlan_ID = Convert.ToInt32(ldtrRow[enmPlan.plan_id.ToString()]);
                    lbusBenefitApplicationDetail.istrPlanCode = Convert.ToString(ldtrRow[enmPlan.plan_code.ToString()]);
                    lbusBenefitApplicationDetail.istrPlanBenefitDescription = Convert.ToString(ldtrRow["DESCRIPTION"]);
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = Convert.ToString(ldtrRow["CODE_VALUE"]);
                    //lbusBenefitApplicationDetail.istrBenefitAppDetailStatus = icdoBenefitApplication.application_status_description;

                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag == busConstant.FLAG_YES &&
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag == busConstant.FLAG_YES)
                    {
                        lbusBenefitApplicationDetail.istrSubPlan = busConstant.EE_UVHP;
                        lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.EE_UVHP;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue = Convert.ToString(ldtrRow["CODE_VALUE"]);
                    }
                    else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag == busConstant.FLAG_YES)
                    {
                        lbusBenefitApplicationDetail.istrSubPlan = busConstant.EE;
                        lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.EE;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue = Convert.ToString(ldtrRow["CODE_VALUE"]);
                    }
                    else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag == busConstant.FLAG_YES)
                    {
                        lbusBenefitApplicationDetail.istrSubPlan = busConstant.UVHP;
                        lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.UVHP;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue = Convert.ToString(ldtrRow["CODE_VALUE"]);
                    }
                    else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                    {
                        lbusBenefitApplicationDetail.istrSubPlan = busConstant.L52_SPL_ACC;
                        lbusBenefitApplicationDetail.istrSubPlanDescription = "Local-52 Special Account";
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue = Convert.ToString(ldtrRow["CODE_VALUE"]);
                        if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                        {
                            this.EligibileforL52Spl = true;
                        }
                    }
                    else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                    {
                        lbusBenefitApplicationDetail.istrSubPlan = busConstant.L161_SPL_ACC;
                        lbusBenefitApplicationDetail.istrSubPlanDescription = "Local-161 Special Account";
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue = Convert.ToString(ldtrRow["CODE_VALUE"]);
                        if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                        {
                            this.EligibileforL161Spl = true;
                        }
                    }

                    if (Eligible_Plans.IsNull())
                        Eligible_Plans = new Collection<string>();
                    Eligible_Plans.Add(lbusBenefitApplicationDetail.istrPlanCode); //This is done so that when u open and Approved guy your plans dropdown doesnt crash since we are not going to check Eligbility then

                }


                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id != 0)
                {
                    DataTable ldtbPersonDetails = Select("cdoBenefitApplication.GetJointAnunantDetailsFromID", new object[2] { lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id, ibusPerson.icdoPerson.person_id });
                    if (ldtbPersonDetails.Rows.Count > 0)
                    {
                        DataRow ldtrRow = ldtbPersonDetails.Rows[0];
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintJointAnnuaintID = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrFullName = Convert.ToString(ldtrRow["FULLNAME"]);
                        lbusBenefitApplicationDetail.istrJointAnnunantMpid = Convert.ToString(ldtrRow[enmPerson.mpi_person_id.ToString()]);
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = Convert.ToString(ldtrRow["RELATIONSHIP"]);
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrGender = Convert.ToString(ldtrRow["GENDER"]);
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idtDOB = Convert.ToDateTime(ldtrRow["DATEOFBIRTH"]);
                        if (!string.IsNullOrEmpty(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrDOB))
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrDOB = Convert.ToDateTime(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrDOB).ToString();
                        }
                    }

                }

                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id != 0)
                {
                    DataTable ldtbPersonssFullName = Select("cdoBenefitApplication.GetPersonsFullname", new object[1] { lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id });
                    if (ldtbPersonssFullName.Rows.Count > 0)
                    {
                        DataRow ldtrRow = ldtbPersonssFullName.Rows[0];
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorFullName = Convert.ToString(ldtrRow[0]);

                    }

                    DataTable ldtbPersonDetails = Select("cdoBenefitApplication.GetSurvivorDetailsFromSurvivorId", new object[3] { ibusPerson.icdoPerson.person_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id, lbusBenefitApplicationDetail.istrPlanCode });
                    if (ldtbPersonDetails.Rows.Count > 0)
                    {
                        DataRow ldtrRow = ldtbPersonDetails.Rows[0];
                        //lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorFullName = Convert.ToString(ldtrRow["FULLNAME"]);
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = Convert.ToString(ldtrRow["RELATIONSHIP"]);
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idecPercentage = Convert.ToDecimal(ldtrRow["PERCENTAGE"]);
                    }
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorTypeValue = busConstant.SURVIVOR_TYPE_PERSON;
                }
                else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id != 0)
                {
                    DataTable ldtbOrgFullName = Select("cdoBenefitApplication.GetOrgFullName", new object[1] { lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id });
                    if (ldtbOrgFullName.Rows.Count > 0)
                    {
                        DataRow ldtrRow = ldtbOrgFullName.Rows[0];
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrOrganizationName = Convert.ToString(ldtrRow[0]);
                    }

                    DataTable ldtbPersonDetails = Select("cdoBenefitApplication.GetOrgDetailsFromOrgId", new object[2] { lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id, ibusPerson.icdoPerson.person_id });
                    if (ldtbPersonDetails.Rows.Count > 0)
                    {
                        DataRow ldtrRow = ldtbPersonDetails.Rows[0];
                        //lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrOrganizationName = Convert.ToString(ldtrRow["FULLNAME"]);
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = Convert.ToString(ldtrRow["RELATIONSHIP"]);
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idecPercentage = Convert.ToDecimal(ldtrRow["PERCENTAGE"]);
                    }
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorTypeValue = busConstant.SURVIVOR_TYPE_ORG;
                }
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (this.icdoBenefitApplication.iobjPassInfo.ienmPageMode == utlPageMode.New)
            {
                UpdateStatusHistoryValue();
                LoadBenefitApplicationStatusHistorys();
                if (possibleSSNflag)
                {
                    //Initiate Workflow:Process SSN Merge

                    Hashtable lhstRequestParams = new Hashtable();

                    lhstRequestParams.Add("PERSON_ID", this.ibusPerson.icdoPerson.person_id);
                    lhstRequestParams.Add("MPI_PERSON_ID", this.ibusPerson.icdoPerson.mpi_person_id);

                    busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PROCESS_SSN_MERGE, this.ibusPerson.icdoPerson.person_id, 0, 0, lhstRequestParams);
                }
            }


            if (this.iclbBenefitApplicationDetail.Count > 0 && this.iblnWithdrawalForAlternatePayee && this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_id = this.icdoBenefitApplication.benefit_application_id;
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.created_by = this.icdoBenefitApplication.created_by;
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.created_date = this.icdoBenefitApplication.created_date;
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.modified_by = this.icdoBenefitApplication.modified_by;
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.modified_date = this.icdoBenefitApplication.modified_date;
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.update_seq = this.icdoBenefitApplication.update_seq;

                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.Insert();
                }
            }

            this.EvaluateInitialLoadRules();
            if (this.ibusBaseActivityInstance.IsNotNull() && ((Sagitec.Bpm.busBpmActivityInstance)this.ibusBaseActivityInstance).ibusBpmActivity.IsNull())
            {
                this.SetProcessInstanceParameters();
            }

        }

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            busSSNMerge lbusSSNMerge = new busSSNMerge() { icdoPerson = new cdoPerson() };
            bool IsPossibleDuplicates = false;
            DataTable ldtbPossibleSSNs = new DataTable();
            if (this.ibusPerson.icdoPerson.first_name.IsNotNullOrEmpty() && this.ibusPerson.icdoPerson.last_name.IsNotNullOrEmpty() && this.ibusPerson.icdoPerson.istrSSNNonEncrypted.IsNotNullOrEmpty()
                && this.ibusPerson.icdoPerson.date_of_birth != DateTime.MinValue)
            {
                ldtbPossibleSSNs = Select("cdoPerson.GetPossibleDuplicateSSNs", new object[5] { this.ibusPerson.icdoPerson.first_name , this.ibusPerson.icdoPerson.last_name,  
                            this.ibusPerson.icdoPerson.date_of_birth,this.ibusPerson.icdoPerson.istrSSNNonEncrypted, this.ibusPerson.icdoPerson.person_id});
            }
            if (ldtbPossibleSSNs != null && ldtbPossibleSSNs.Rows.Count > 0)
                IsPossibleDuplicates = true;

            if (IsPossibleDuplicates || lbusSSNMerge.CheckSSNExistsInHistory(this.ibusPerson))
            {
                possibleSSNflag = true;
            }
            else
            {
                possibleSSNflag = false;
            }

        }

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;
            int lintPersonId = 0;
            string lstrBenefitTypeValue = string.Empty;
            string lstrMpiPersonID = Convert.ToString(ahstParam["MPI_PERSON_ID"]).Trim();
            string lstrAlternatePayeeMpid = Convert.ToString(ahstParam["alternate_payee_mpid"]).Trim();
            string lstrBenefit_type_value = Convert.ToString(ahstParam["benefit_type_value"]).Trim();
            int lintDroApplicationId = 0;
            int lintEarlyRetireePayeeAccountCount = 0;

            // For RTMT Lookup
            if (iobjPassInfo.istrFormName != busConstant.Retirement_Application_Lookup_Form)
            {
                int.TryParse(ahstParam["dro_application_id"].ToString(), out lintDroApplicationId);
            }
            //PROD PIR 799
            if (iobjPassInfo.istrFormName == busConstant.Retirement_Application_Lookup_Form)
            {
                DataTable ldtbGetRTMApplication = busBase.Select("cdoBenefitApplication.GetRetirementApplication", new object[1] { lstrMpiPersonID });
                if (ldtbGetRTMApplication != null && ldtbGetRTMApplication.Rows.Count > 0)
                {
                    int iintEligiblePlanRetirementcnt = 0;
                    int iintEligiblePlansCount = 0;
                    bool iblnIsMPIPP_Flag = false;
                    bool iblnIsIAP_Flag = false;
                    bool iblnIsLocal600_Flag = false;
                    bool iblnIsLocal666_Flag = false;
                    bool iblnIslocal700_Flag = false;
                    bool iblnIslocal161_Flag = false;
                    bool iblnIslocal52_Flag = false;
                    for (int iint = 0; iint < ldtbGetRTMApplication.Rows.Count; iint++)
                    {
                        // get eligible plans count
                        if (iintEligiblePlansCount == 0)
                        {
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["MPIPP_ELIGIBLEFLAG"]) == "Y")
                            {
                                iintEligiblePlansCount++;
                            }
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["IAP_ELIGIBLEFLAG"]) == "Y")
                            {
                                iintEligiblePlansCount++;
                            }
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["LOCAL600_ELIGIBLEFLAG"]) == "Y")
                            {
                                iintEligiblePlansCount++;
                            }
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["LOCAL666_ELIGIBLEFLAG"]) == "Y")
                            {
                                iintEligiblePlansCount++;
                            }
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["LOCAL700_ELIGIBLEFLAG"]) == "Y")
                            {
                                iintEligiblePlansCount++;
                            }
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["LOCAL52_ELIGIBLEFLAG"]) == "Y")
                            {
                                iintEligiblePlansCount++;
                            }
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["LOCAL161_ELIGIBLEFLAG"]) == "Y")
                            {
                                iintEligiblePlansCount++;
                            }
                        }
                        // check retired plans
                        if (!iblnIsMPIPP_Flag)
                        {
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["mpipp_flag"]) == "Y")
                                iblnIsMPIPP_Flag = true;
                        }
                        if (!iblnIsIAP_Flag)
                        {
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["iap_flag"]) == "Y")
                                iblnIsIAP_Flag = true;
                        }
                        if (!iblnIsLocal600_Flag)
                        {
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["local600_flag"]) == "Y")
                                iblnIsLocal600_Flag = true;
                        }
                        if (!iblnIsLocal666_Flag)
                        {
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["local666_flag"]) == "Y")
                                iblnIsLocal666_Flag = true;
                        }
                        if (!iblnIslocal700_Flag)
                        {
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["local700_flag"]) == "Y")
                                iblnIslocal700_Flag = true;
                        }
                        if (!iblnIslocal161_Flag)
                        {
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["local161_flag"]) == "Y")
                                iblnIslocal161_Flag = true;
                        }
                        if (!iblnIslocal52_Flag)
                        {
                            if (Convert.ToString(ldtbGetRTMApplication.Rows[iint]["local52_flag"]) == "Y")
                                iblnIslocal52_Flag = true;
                        }
                    }
                    // get retired plans count                    
                    if (iblnIsMPIPP_Flag)
                        iintEligiblePlanRetirementcnt++;
                    if (iblnIsIAP_Flag)
                        iintEligiblePlanRetirementcnt++;
                    if (iblnIsLocal600_Flag)
                        iintEligiblePlanRetirementcnt++;
                    if (iblnIsLocal666_Flag)
                        iintEligiblePlanRetirementcnt++;
                    if (iblnIslocal700_Flag)
                        iintEligiblePlanRetirementcnt++;
                    if (iblnIslocal161_Flag)
                        iintEligiblePlanRetirementcnt++;
                    if (iblnIslocal52_Flag)
                        iintEligiblePlanRetirementcnt++;

                    // check Pending/Approved Retirement Application
                    if (iintEligiblePlansCount == iintEligiblePlanRetirementcnt)
                    {
                        lobjError = AddError(6231, " ");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                }
            }

            if (iobjPassInfo.idictParams.ContainsKey("UserID"))
            {
                string astrUserSerialID = iobjPassInfo.idictParams["UserID"].ToString();
                DataTable ldtbParticipantId = Select("cdoPerson.GetVIPFlagInfo", new object[2] { astrUserSerialID, lstrMpiPersonID });
                if (ldtbParticipantId.Rows.Count > 0)
                {
                    if (ldtbParticipantId.Rows[0]["istr_IS_LOGGED_IN_USER_VIP"].ToString() == "N" && ldtbParticipantId.Rows[0]["istrRelativeVipFlag"].ToString() == "Y")
                    {
                        lobjError = AddError(6175, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                }

            }

            if (string.IsNullOrEmpty(lstrMpiPersonID) && !(lstrBenefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && lintDroApplicationId > 0))
            {
                lobjError = AddError(4075, "");
                larrErrors.Add(lobjError);
                return larrErrors;
            }
            else if (lintDroApplicationId <= 0)
            {
                DataTable ldtbParticipantId = Select("cdoBenefitApplication.CheckParticipantExistance", new object[1] { lstrMpiPersonID });
                if (ldtbParticipantId.Rows.Count <= 0)
                {
                    lobjError = AddError(5016, "");
                    larrErrors.Add(lobjError);
                    return larrErrors;

                }
                else
                {
                    lintPersonId = Convert.ToInt32(ldtbParticipantId.Rows[0][enmPerson.person_id.ToString()]);
                    lstrBenefitTypeValue = Convert.ToString(ahstParam[busConstant.BENEFIT_TYPE_VALUE]);
                    if (ValidateExistingRecord(lintPersonId, lstrBenefitTypeValue, 0))
                    {
                        lobjError = AddError(5403, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }


                    if (lstrBenefitTypeValue == busConstant.BENEFIT_TYPE_RETIREMENT)
                    {
                        DataTable ldtbDateOfDeath = busBase.Select("cdoBenefitApplication.GetDateOfDeath", new object[1] { lintPersonId });
                        if (ldtbDateOfDeath.Rows.Count > 0)
                        {
                            int lintCount = Convert.ToInt32(ldtbDateOfDeath.Rows[0]["COUNT"]);

                            if (lintCount > 0)
                            {
                                lobjError = AddError(5146, "");
                                larrErrors.Add(lobjError);
                                return larrErrors;
                            }
                        }

                        int lintPayeeAccounts = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.GetActiveDisabilityPayeeAccounts", new object[1] { lintPersonId },
                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                        if (lintPayeeAccounts > 0)
                        {
                            //PIR-911 //PIR-911 Issue 3 part 1 
                            int lintPayeeAccountswithIAPPlan = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.GetActiveDisabilityPayeeAccountsWithIAPPlan", new object[1] { lintPersonId },
                                                                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            if (lintPayeeAccountswithIAPPlan <= 0)
                            {
                                lobjError = AddError(5498, "");
                                larrErrors.Add(lobjError);
                                return larrErrors;
                            }
                        }
                    }

                    if (lstrBenefitTypeValue == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        DataTable ldtbBenefitAppDateOfDeath = busBase.Select("cdoBenefitApplication.GetDateOfDeath", new object[1] { lintPersonId });
                        if (ldtbBenefitAppDateOfDeath.Rows.Count > 0)
                        {
                            int lintCount = Convert.ToInt32(ldtbBenefitAppDateOfDeath.Rows[0]["COUNT"]);

                            if (lintCount > 0)
                            {
                                lobjError = AddError(5145, "");
                                larrErrors.Add(lobjError);
                                return larrErrors;
                            }
                        }

                        //Removed - on 2/20/2013 
                        //DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.ValidationForNewDisabilityApplication_RTMT", new object[] { lintPersonId });
                        //if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
                        //{
                        //    int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                        //    if (lintCount > 0)
                        //    {
                        //        lobjError = AddError(5123, "");
                        //        larrErrors.Add(lobjError);
                        //        return larrErrors;
                        //    }
                        //}

                        DataTable ldtbBenefitApp = busBase.Select("cdoBenefitApplication.ValidationForNewDisabilityApplication_DDPR", new object[] { lintPersonId });
                        if (ldtbBenefitApp != null && ldtbBenefitApp.Rows.Count > 0)
                        {
                            int lintCount = Convert.ToInt32(ldtbBenefitApp.Rows[0]["COUNT"]);
                            if (lintCount > 0)
                            {
                                lobjError = AddError(5147, "");
                                larrErrors.Add(lobjError);
                                return larrErrors;
                            }
                        }

                        DataTable ldtbBenApplcation = busBase.Select("cdoBenefitApplication.ValidationForNewDisabilityApplication_DDPT", new object[] { lintPersonId });
                        if (ldtbBenApplcation != null && ldtbBenApplcation.Rows.Count > 0)
                        {
                            int lintCount = Convert.ToInt32(ldtbBenApplcation.Rows[0]["COUNT"]);
                            if (lintCount > 0)
                            {
                                lobjError = AddError(5148, "");
                                larrErrors.Add(lobjError);
                                return larrErrors;
                            }
                        }
                    }

                    if (lstrBenefitTypeValue == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                    {
                        DataTable ldtbBenefitAppDateOfDeath = busBase.Select("cdoBenefitApplication.GetDateOfDeathOfParticipant", new object[1] { lintPersonId });
                        if (ldtbBenefitAppDateOfDeath.Rows.Count > 0)
                        {
                            string lstrDateofDeath = Convert.ToString(ldtbBenefitAppDateOfDeath.Rows[0][0]);
                            if (string.IsNullOrEmpty(lstrDateofDeath))
                            {
                                lobjError = AddError(5086, "");
                                larrErrors.Add(lobjError);
                                return larrErrors;
                            }
                        }

                        DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedRetirement", new object[1] { lintPersonId });
                        if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
                        {
                            int lintCount = 0;
                            lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                            if (lintCount > 0)
                            {
                                DataTable ldtPayeeAccount = busBase.Select("cdoBenefitApplication.GetPayeeAccountsInApproved&ReviewedStatus", new object[1] { lintPersonId });
                                if (!(ldtPayeeAccount.IsNotNull() && ldtPayeeAccount.Rows.Count > 0))
                                {
                                   //Ticket : 50555
                                    object lobjMdWithNoIAP = null;
                                    lobjMdWithNoIAP = DBFunction.DBExecuteScalar("cdoBenefitApplication.CheckPayeeAccountMinimumDistributionRetirementType", new object[1] { lintPersonId },
                                                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                    int lintMdWithNoIAP = 0;
                                    if (lobjMdWithNoIAP != null)
                                    {
                                        lintMdWithNoIAP = ((int)lobjMdWithNoIAP);
                                    }
                                                                       
                                    if (lintMdWithNoIAP <= 0)
                                    {
                                        lobjError = AddError(5144, "");
                                        larrErrors.Add(lobjError);
                                        return larrErrors;
                                    }
                                }
                            }
                        }

                    }
                }
            }


            if (lstrBenefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && lstrAlternatePayeeMpid != "" && Convert.ToString(ahstParam["dro_application_id"]) == "")
            {
                lobjError = AddError(5425, "");
                larrErrors.Add(lobjError);
                return larrErrors;
            }

            if (lstrBenefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && Convert.ToString(ahstParam["dro_application_id"]) != "")
            {
                busQdroApplication lbusQdroApplication = new busQdroApplication();
                lbusQdroApplication.FindQdroApplication(Convert.ToInt32(ahstParam["dro_application_id"]));
                if (lbusQdroApplication.icdoDroApplication.dro_status_value != busConstant.DRO_APPROVED && lbusQdroApplication.icdoDroApplication.dro_status_value != busConstant.DRO_QUALIFIED)
                {
                    lobjError = AddError(5426, "");
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }

                DataTable ldtbWithdrawalForAlternatePayee = busBase.Select("cdoBenefitApplication.GetWdrlAlternatePayeeAppl", new object[1] { Convert.ToInt32(ahstParam["dro_application_id"]) });
                if (ldtbWithdrawalForAlternatePayee != null && ldtbWithdrawalForAlternatePayee.Rows.Count > 0)
                {
                    int lintCount = 0;
                    lintCount = Convert.ToInt32(ldtbWithdrawalForAlternatePayee.Rows[0][0]);
                    if (lintCount > 0)
                    {
                        lobjError = AddError(5467, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                }
            }

            if (ahstParam["benefit_type_value"].ToString() == string.Empty)
            {
                lobjError = AddError(5015, "");
                larrErrors.Add(lobjError);
            }
            if (lintPersonId != 0)          // To check if plan is not enrolled.
            {
                if (ahstParam[busConstant.BENEFIT_TYPE_VALUE].ToString() == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                {
                    DataTable ldtbList = Select("cdoPersonAccountBeneficiary.GetPlanForDeathPreRetr", new object[1] { lintPersonId });
                    if (ldtbList.Rows.Count == 0)
                    {
                        lobjError = AddError(5092, "");
                        larrErrors.Add(lobjError);
                    }
                }
                else if (ahstParam[busConstant.BENEFIT_TYPE_VALUE].ToString() != busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT
                    && ahstParam[busConstant.BENEFIT_TYPE_VALUE].ToString() != busConstant.BENEFIT_TYPE_WITHDRAWAL)
                {
                    //PIR - 1036
                    if (ahstParam[busConstant.BENEFIT_TYPE_VALUE].ToString() == busConstant.BENEFIT_TYPE_DISABILITY)
                    {                        
                        DataTable ldtblPayeeAccounts = busBase.Select("cdoBenefitApplication.GetEarlyRetirementPayeeInComp", new object[1] { lintPersonId });
                        if(ldtblPayeeAccounts != null)
                        { 
                            lintEarlyRetireePayeeAccountCount = ldtblPayeeAccounts.Rows.Count;
                        }
                    }

                    DataTable ldtbList = Select("cdoPersonAccountBeneficiary.GetPlan", new object[1] { lintPersonId });
                    if (ldtbList.Rows.Count == 0 && lintEarlyRetireePayeeAccountCount == 0)
                    {
                        lobjError = AddError(5092, "");
                        larrErrors.Add(lobjError);
                    }
                    else if (ldtbList.Rows.Count > 0 && ahstParam[busConstant.BENEFIT_TYPE_VALUE].ToString() == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                    {
                        ldtbList = Select("cdoPersonAccountBeneficiary.GetPlansForwithdrawlBenefit", new object[1] { lintPersonId });
                        if (ldtbList.Rows.Count == 0)
                        {
                            lobjError = AddError(5093, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                }
            }
            if (Convert.ToString(ahstParam["dro_application_id"]).IsNotNullOrEmpty() && lstrBenefit_type_value != busConstant.BENEFIT_TYPE_WITHDRAWAL)
            {
                lobjError = AddError(5466, "");
                larrErrors.Add(lobjError);
            }
            return larrErrors;
        }

        # endregion

        #region Public Methods


        //Code-Abhishek


        private void Reset_Variables()
        {
            #region Pension_Credits_Reset
            this.Local52_PensionCredits = 0;
            this.Local161_PensionCredits = 0;
            this.Local600_PensionCredits = 0;
            this.Local666_PensionCredits = 0;
            this.Local700_PensionCredits = 0;
            #endregion

            #region Eligbility_Vars_Reset
            //this.VestedinRule4_IAP = false;
            //this.VestedinRule4_MPI = false;
            this.ForfeitureYearsCollectionIAP.Clear();
            //this.ForfeitureYearsCollectionIAP_NA.Clear();
            this.ForfeitureYearsCollectionMPI.Clear();
            //this.ForfeitureYearsCollectionMPI_NA.Clear();
            this.larrYears_IAP.Clear();
            //this.larrYears_IAP_NA.Clear();
            this.larrYears_MPI.Clear();
            //this.larrYears_MPI_NA.Clear();
            this.icdoBenefitApplication.adtIAPVestingDate = DateTime.MinValue;
            this.icdoBenefitApplication.adtMPIVestingDate = DateTime.MinValue;
            this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_NO;
            this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.FLAG_NO;
            if (Eligible_Plans.IsNull())
                Eligible_Plans = new Collection<string>();
            else
                Eligible_Plans.Clear();

            if (iclbEligiblePlans.IsNull())
                iclbEligiblePlans = new Collection<string>();
            else
                iclbEligiblePlans.Clear();

            this.NotEligible = false;
            this.EligibileforL52Spl = false;
            this.EligibileforL161Spl = false;
            this.Eligibile4IAP = false;
            this.QualifiedSpouseExists = false;
            if (this.lclcL52BenOptionsPreDeath.IsNull())
                this.lclcL52BenOptionsPreDeath = new Collection<cdoPlanBenefitXr>();
            else
                this.lclcL52BenOptionsPreDeath.Clear();
            #endregion
        }

        //WI 23234 Secure Document submission
        private void RemoveDocumentUploadFlag(int aintPersonId, int aintBenefitApplicationId)
        {
            //int aintPersonId = ibusPerson.icdoPerson.person_id;
            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            DataTable ldtPerson;
            DataTable ldtCorPacketList = Select("cdoBenefitApplication.GetPacketTrackingList", new object[1] { aintPersonId });
            DataTable ldtOtherRetirementAppList = Select("cdoBenefitApplication.GetOtherRetirementApplication", new object[2] { aintPersonId, aintBenefitApplicationId });
            if (ldtCorPacketList != null && ldtCorPacketList.Rows.Count == 0 && ldtOtherRetirementAppList != null && ldtOtherRetirementAppList.Rows.Count == 0)
            {
                ldtPerson = Select("cdoPerson.GetPersonDocumentUploadFlag", new object[1] { aintPersonId });
                if (ldtPerson != null && ldtPerson.Rows.Count > 0)
                {
                    if (Convert.ToString(ldtPerson.Rows[0][0]) == "Y")
                    {
                        if (lbusPerson.FindPerson(aintPersonId))
                        {
                            lbusPerson.icdoPerson.document_upload_flag = busConstant.FLAG_NO;
                            lbusPerson.icdoPerson.Update();
                        }
                    }
                }
            }

        }

        private void Determine_VestingMPI(string astrPersonAccStatus, int aintEndofWorkYear, bool ablnDataExtract = false)
        {
            iblnLocalPlanExists4VestingCheck = false;
            int lintLateHoursExist = 0;

            if (lintLateHoursCount.IsNotNull() && lintLateHoursCount > 0)
                lintLateHoursExist = lintLateHoursCount;

            if (!ablnDataExtract && astrLegacyDBConnetion.IsNullOrEmpty())
            {
                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            }

            if (!ablnDataExtract)
            {
                SqlParameter[] lParameters = new SqlParameter[3];
                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                SqlParameter param2 = new SqlParameter("@EVALUATION_DATE ", DbType.DateTime);
                SqlParameter param3 = new SqlParameter("@VESTING_DATE", DbType.DateTime);

                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                lParameters[0] = param1;

                param2.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date;
                lParameters[1] = param2;

                param3.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                lParameters[2] = param3;


                //ldtLateHoursCount = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetCountForLateHours", astrLegacyDBConnetion, null, lParameters);
                //if (ldtLateHoursCount.Rows.Count > 0)
                //    lintLateHoursExist = Convert.ToInt32(ldtLateHoursCount.Rows[0][0]);
            }

            if (ablnDataExtract || (lintLateHoursExist > 0 || this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES ||
                     !(this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).Count() > 0)))
            {

                if (this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES)
                {
                    RemoveForfietureDateFromPersonAccountEligibility(busConstant.MPIPP);
                }

                if (this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains("Local")).Count() > 0)
                {
                    WorkOutVestingMPIPLan(aclbPersonWorkHistory_MPI);

                    if (!iblnLocalPlanExists4VestingCheck)
                        this.VestingCheckingSetup(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, 0);
                }
                else if (!iblnLocalPlanExists4VestingCheck)
                {
                    if (!(this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())) { }
                    this.VestingCheckingSetup(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, 0);
                }

                if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES)
                    ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, this.icdoBenefitApplication.adtMPIVestingDate, true, astrPersonAccStatus, aintEndofWorkYear);
                else if (!this.ForfeitureYearsCollectionMPI.IsNullOrEmpty())
                {
                    if ((this.icdoBenefitApplication.adtMPIVestingDate.IsNotNull() && ForfeitureYearsCollectionMPI.Last() < this.icdoBenefitApplication.adtMPIVestingDate.Year)
                        || this.icdoBenefitApplication.adtMPIVestingDate.IsNull() || this.icdoBenefitApplication.adtMPIVestingDate == DateTime.MinValue)
                        AddUpdateForfeitureDate(ForfeitureYearsCollectionMPI.Last(), busConstant.MPIPP, true);
                    if (!iblnLocalPlanExists4VestingCheck)
                        RemoveUnwantedPadding(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, ForfeitureYearsCollectionMPI.Last());
                }
                else
                {
                    AddUpdatePersonAccountEligibility(DateTime.MinValue, busConstant.MPIPP, String.Empty);
                }
                this.PopulateLocalsRelatedInformation(this.aclbPersonWorkHistory_MPI);
            }
            else
            {
                this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date, false, astrPersonAccStatus, aintEndofWorkYear);

                if (this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments.IsNullOrEmpty())
                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments += "MPI-" + busConstant.VESTED_COMMENT + this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vesting_rule;
                else if (!(this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments.Contains(busConstant.VESTED_COMMENT)))
                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments += "MPI-" + busConstant.VESTED_COMMENT + this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vesting_rule;

                this.PopulateLocalsRelatedInformation(this.aclbPersonWorkHistory_MPI);
            }
        }

        private void RemoveForfietureDateFromPersonAccountEligibility(string astrplan_code)
        {
            busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };

            int lintAccountId = this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == astrplan_code).First().icdoPersonAccount.person_account_id;

            if (lintAccountId > 0)
            {
                DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });

                if (ldtbPersonAccountEligibility.Rows.Count > 0)
                {
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                    DateTime adtOldVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;

                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date = DateTime.MinValue;
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.Update();
                }
            }

        }


        private void PopulateLocalsRelatedInformation(Collection<cdoDummyWorkData> aclbWorkHistory)
        {
            busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };

            foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
            {
                if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP)
                {
                    DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lbusPersonAccount.icdoPersonAccount.person_account_id });
                    if (ldtbPersonAccountEligibility.Rows.Count > 0)
                    {
                        lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);

                        if (lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.IsNotNull() && lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                        {
                            if (aclbWorkHistory.Where(item => item.year == lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).Count() > 0)
                            {
                                if (aclbWorkHistory.Where(item => item.year == lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments.IsNullOrEmpty())
                                    aclbWorkHistory.Where(item => item.year == lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments += "[VESTED -" + lbusPersonAccount.icdoPersonAccount.istrPlanCode + "]";
                                else if (!(aclbWorkHistory.Where(item => item.year == lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments.Contains("[VESTED -" + lbusPersonAccount.icdoPersonAccount.istrPlanCode + "]")))
                                    aclbWorkHistory.Where(item => item.year == lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments += "[VESTED -" + lbusPersonAccount.icdoPersonAccount.istrPlanCode + "]";
                            }
                        }
                        else if (lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.IsNotNull() && lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date != DateTime.MinValue)
                        {
                            if (aclbWorkHistory.Where(item => item.year == lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).Count() > 0)
                            {
                                if (aclbWorkHistory.Where(item => item.year == lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year).First().comments.IsNullOrEmpty())
                                    aclbWorkHistory.Where(item => item.year == lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year).First().comments += "[" + busConstant.FORFEITURE_COMMENT + "-" + lbusPersonAccount.icdoPersonAccount.istrPlanCode + "]";
                                else if (!(aclbWorkHistory.Where(item => item.year == lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year).First().comments.Contains(busConstant.FORFEITURE_COMMENT)))
                                    aclbWorkHistory.Where(item => item.year == lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year).First().comments += "[" + busConstant.FORFEITURE_COMMENT + "-" + lbusPersonAccount.icdoPersonAccount.istrPlanCode + "]";
                            }
                        }
                    }

                }
            }
        }


        private void Determine_VestingIAP(string astrPersonAccStatus, int aintEndofWorkYear, bool ablnSSNMerge = false, utlPassInfo aobjPassInfo = null)
        {
            int lintLateHoursExist = 0;

            if (lintLateHoursCount.IsNotNull() && lintLateHoursCount > 0)
                lintLateHoursExist = lintLateHoursCount;

            if (astrLegacyDBConnetion.IsNullOrEmpty())
            {
                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            }

            SqlParameter[] lParameters = new SqlParameter[3];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@EVALUATION_DATE ", DbType.DateTime);
            SqlParameter param3 = new SqlParameter("@VESTING_DATE", DbType.DateTime);

            if (CheckAlreadyVested(busConstant.IAP))
            {
                //int lintLateHoursExist = 0;
                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                lParameters[0] = param1;

                param2.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date;
                lParameters[1] = param2;

                param3.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                lParameters[2] = param3;

                //if (ablnSSNMerge == false)
                //ldtLateHoursCount = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetCountForLateHours", astrLegacyDBConnetion, null, lParameters);
                //else
                //ldtLateHoursCount = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetCountForLateHours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                //if (ldtLateHoursCount.Rows.Count > 0)
                //    lintLateHoursExist = Convert.ToInt32(ldtLateHoursCount.Rows[0][0]);

                if (lintLateHoursExist > 0 || this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES ||
                    !(this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).Count() > 0))
                {

                    if (this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES)
                    {
                        RemoveForfietureDateFromPersonAccountEligibility(busConstant.IAP);
                    }

                   
                    if (!(this.aclbPersonWorkHistory_IAP.IsNullOrEmpty()))          
                        this.VestingCheckingSetup(this.aclbPersonWorkHistory_IAP, busConstant.IAP, 0, false, ablnSSNMerge, aobjPassInfo);

                    //RID - 60453 
                    if (this.icdoBenefitApplication.adtIAPVestingDate == DateTime.MinValue && this.icdoBenefitApplication.adtMPIVestingDate != DateTime.MinValue && this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains("Local")).Count() > 0
                       && this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains("Local") && local.icdoPersonAccount.idtVestedDate != DateTime.MinValue).Count() > 0)
                    {
                        this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                        this.icdoBenefitApplication.adtIAPVestingDate = this.icdoBenefitApplication.adtMPIVestingDate;
                    }

                    if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES)
                        ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.icdoBenefitApplication.adtIAPVestingDate, true, astrPersonAccStatus, aintEndofWorkYear);
                    else if (!this.ForfeitureYearsCollectionIAP.IsNullOrEmpty())
                    {
                        if ((this.icdoBenefitApplication.adtIAPVestingDate.IsNotNull() && ForfeitureYearsCollectionIAP.Last() < this.icdoBenefitApplication.adtIAPVestingDate.Year)
                                || this.icdoBenefitApplication.adtIAPVestingDate.IsNull() || this.icdoBenefitApplication.adtIAPVestingDate == DateTime.MinValue)
                            AddUpdateForfeitureDate(ForfeitureYearsCollectionIAP.Last(), busConstant.IAP, true);
                        RemoveUnwantedPadding(this.aclbPersonWorkHistory_IAP, busConstant.IAP, ForfeitureYearsCollectionIAP.Last());
                    }
                    else
                    {
                        AddUpdatePersonAccountEligibility(DateTime.MinValue, busConstant.IAP, String.Empty);
                    }

                }
                else
                {

                    this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.Flag_Yes;
                    ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date, false, astrPersonAccStatus, aintEndofWorkYear);

                    if (this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).Count() > 0 && this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments.IsNullOrEmpty())
                        this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments += busConstant.VESTED_COMMENT + this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vesting_rule;
                    else if (this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).Count() > 0 && !(this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments.Contains(busConstant.VESTED_COMMENT)))
                        this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments += busConstant.VESTED_COMMENT + this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vesting_rule;

                }
            }
        }


        #region For MPI Workhistory Plan Visibility in Grid
        public bool CheckIfPersonHasLocal161()
        {
            if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0)
                return true;
            else
                return false;
        }
        public bool CheckIfPersonHasLocal666()
        {
            if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0)
                return true;
            else
                return false;
        }
        public bool CheckIfPersonHasLocal600()
        {
            if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0)
                return true;
            else
                return false;
        }
        public bool CheckIfPersonHasLocal700()
        {
            if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0)
                return true;
            else
                return false;
        }
        public bool CheckIfPersonHasLocal52()
        {
            if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0)
                return true;
            else
                return false;
        }
        #endregion

        private void Determine_VestingIAP_PreRetirementDeath(string astrPersonAccStatus, int aintEndofWorkYear, bool ablnSSNMerge, utlPassInfo aobjPassInfo)
        {
            int lintLateHoursExist = 0;

            if (lintLateHoursCount.IsNotNull() && lintLateHoursCount > 0)
                lintLateHoursExist = lintLateHoursCount;

            if (astrLegacyDBConnetion.IsNullOrEmpty())
            {
                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            }

            SqlParameter[] lParameters = new SqlParameter[3];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@EVALUATION_DATE ", DbType.DateTime);
            SqlParameter param3 = new SqlParameter("@VESTING_DATE", DbType.DateTime);

            if (CheckAlreadyVested(busConstant.IAP))
            {
                //int lintLateHoursExist = 0;
                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                lParameters[0] = param1;

                param2.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date;
                lParameters[1] = param2;

                param3.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                lParameters[2] = param3;

                //ldtLateHoursCount = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetCountForLateHours", astrLegacyDBConnetion, null, lParameters);
                //if (ldtLateHoursCount.Rows.Count > 0)
                //    lintLateHoursExist = Convert.ToInt32(ldtLateHoursCount.Rows[0][0]);

                if (lintLateHoursExist > 0 || this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES ||
                    !(this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).Count() > 0))
                {

                    if (this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES)
                    {
                        RemoveForfietureDateFromPersonAccountEligibility(busConstant.IAP);
                    }

                    if (!(this.aclbPersonWorkHistory_IAP.IsNullOrEmpty()))
                        this.VestingCheckingSetup(this.aclbPersonWorkHistory_IAP, busConstant.IAP, 0, false, ablnSSNMerge, aobjPassInfo);

                    if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES)
                        ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.icdoBenefitApplication.adtIAPVestingDate, true, astrPersonAccStatus, aintEndofWorkYear);
                    else if (!this.ForfeitureYearsCollectionIAP.IsNullOrEmpty())
                    {
                        if (this.ForfeitureYearsCollectionIAP.Last() > this.ibusPerson.icdoPerson.date_of_death.Year)
                        {
                            this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                            if (this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).Count() > 0)
                                this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).First().comments += busConstant.VESTED_COMMENT + "VestedOnDeath";
                            AddUpdatePersonAccountEligibility(this.ibusPerson.icdoPerson.date_of_death, busConstant.IAP, "VestedonDeath");
                            ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.ibusPerson.icdoPerson.date_of_death, true, astrPersonAccStatus, aintEndofWorkYear);
                        }
                        else
                        {
                            if ((this.icdoBenefitApplication.adtIAPVestingDate.IsNotNull() && ForfeitureYearsCollectionMPI.Last() < this.icdoBenefitApplication.adtIAPVestingDate.Year)
                                    || this.icdoBenefitApplication.adtIAPVestingDate.IsNull() || this.icdoBenefitApplication.adtIAPVestingDate == DateTime.MinValue)
                                AddUpdateForfeitureDate(ForfeitureYearsCollectionIAP.Last(), busConstant.IAP, true);
                            RemoveUnwantedPadding(this.aclbPersonWorkHistory_IAP, busConstant.IAP, ForfeitureYearsCollectionIAP.Last());
                        }
                    }
                    else
                    {
                        this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                        if (this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).Count() > 0)
                            this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).First().comments += busConstant.VESTED_COMMENT + "VestedOnDeath";
                        AddUpdatePersonAccountEligibility(this.ibusPerson.icdoPerson.date_of_death, busConstant.IAP, "VestedonDeath");
                        ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.ibusPerson.icdoPerson.date_of_death, true, astrPersonAccStatus, aintEndofWorkYear);
                    }


                }
                else
                {
                    this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.Flag_Yes;
                    ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date, false, astrPersonAccStatus, aintEndofWorkYear);

                    if (this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments.IsNullOrEmpty())
                        this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments += busConstant.VESTED_COMMENT + this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vesting_rule;
                    else if (!(this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments.Contains(busConstant.VESTED_COMMENT)))
                        this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).First().comments += busConstant.VESTED_COMMENT + this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vesting_rule;

                }
            }
        }

        //Code-Added Abhishek
        public void LoadandProcessWorkHistory_ForAllPlans(bool ablnSSNMerge = false, utlPassInfo aobjPassInfo = null, DateTime? adtEACutOffdate = null, bool ablnFromService = false) //PIR 628  //Added parameter For CRM Bug 9922
        {
            Reset_Variables();
            DataTable ldtbPensionCredits = new DataTable();

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            bool lblnTemp = this.CheckAlreadyVested(busConstant.MPIPP);

            SqlParameter[] parameters = new SqlParameter[6]; //PIR 628
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);
            SqlParameter param3 = new SqlParameter("@RETIREMENT_DATE", DbType.DateTime);
            SqlParameter param4 = new SqlParameter("@EVALUATION_DATE ", DbType.DateTime);
            SqlParameter param5 = new SqlParameter("@VESTING_DATE", DbType.DateTime);
            SqlParameter param6 = new SqlParameter("@EACutOffDate", DbType.DateTime); //PIR 628

            foreach (busPersonAccount account in this.ibusPerson.iclbPersonAccount)
            {
                #region Check What Plans He has and accordingly populate/Process the Work Historyfor those Plans (Essentially Prepare our summary Table)
                if (!this.ibusPerson.icdoPerson.ssn.IsNullOrEmpty())
                {
                    switch (account.icdoPersonAccount.istrPlanCode)
                    {
                        case busConstant.MPIPP:

                            param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                            parameters[0] = param1;

                            param2.Value = busConstant.MPIPP;
                            parameters[1] = param2;

                            if (this.icdoBenefitApplication.retirement_date.IsNull() || this.icdoBenefitApplication.retirement_date == DateTime.MinValue)
                            {
                                param3.Value = SqlDateTime.MinValue;
                                parameters[2] = param3;
                            }
                            else
                            {
                                param3.Value = this.icdoBenefitApplication.retirement_date;
                                parameters[2] = param3;
                            }

                            if (this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date.IsNotNull() && this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date != DateTime.MinValue)
                            {
                                param4.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date;
                                parameters[3] = param4;
                            }
                            else
                            {
                                param4.Value = new DateTime(1753, 1, 1);
                                parameters[3] = param4;
                            }

                            if (this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.IsNotNull() && this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                            {
                                param5.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                parameters[4] = param5;
                            }
                            else
                            {
                                param5.Value = new DateTime(1753, 1, 1);
                                parameters[4] = param5;
                            }
                            //PIR 628                            
                            if (adtEACutOffdate != null)
                            {
                                param6.Value = adtEACutOffdate;
                                parameters[5] = param6;
                            }
                            else
                            {
                                param6.Value = new DateTime(1753, 1, 1);
                                parameters[5] = param6;
                            }

                            //Specially Done for the Annual Benefit Summary Grrid
                            if (this.lbl4PersonOverviewSummary.IsNotNull() && this.lbl4PersonOverviewSummary) //Specially Done for the Annual Benefit Summary Grrid
                            {
                                SqlParameter[] parameter_overview = new SqlParameter[6];
                                SqlParameter param1_overview = new SqlParameter("@SSN", DbType.String);

                                param1_overview.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                parameter_overview[0] = param1_overview;
                                #region Local Merge ( tusharchandak )
                                SqlParameter param2_overview = new SqlParameter("@MERGER_DATE_L600", DbType.DateTime);

                                param2_overview.Value = busConstant.BenefitCalculation.MERGER_DATE_LOCAL_600;
                                parameter_overview[1] = param2_overview;
                                SqlParameter param3_overview = new SqlParameter("@MERGER_DATE_L666", DbType.DateTime);

                                param3_overview.Value = busConstant.BenefitCalculation.MERGER_DATE_LOCAL_666;
                                parameter_overview[2] = param3_overview;
                                SqlParameter param4_overview = new SqlParameter("@MERGER_DATE_L700", DbType.DateTime);

                                param4_overview.Value = busConstant.BenefitCalculation.MERGER_DATE_LOCAL_700;
                                parameter_overview[3] = param4_overview;
                                SqlParameter param5_overview = new SqlParameter("@MERGER_DATE_L52", DbType.DateTime);

                                param5_overview.Value = busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52;
                                parameter_overview[4] = param5_overview;
                                SqlParameter param6_overview = new SqlParameter("@MERGER_DATE_L161", DbType.DateTime);

                                param6_overview.Value = busConstant.BenefitCalculation.MERGER_DATE_LOCAL_161;
                                parameter_overview[5] = param6_overview;
                                #endregion
                                ldtPersonWorkHistory_MPI = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataForPersonOverview", astrLegacyDBConnetion, null, parameter_overview);
                            }

                            else
                            {
                                if (ablnSSNMerge == false)
                                    ldtPersonWorkHistory_MPI = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataTillGivenDate", astrLegacyDBConnetion, null, parameters);
                                else
                                    ldtPersonWorkHistory_MPI = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataTillGivenDate", astrLegacyDBConnetion, aobjPassInfo, parameters);
                            }

                            if (ldtPersonWorkHistory_MPI.Rows.Count > 0)
                            {

                                this.aclbPersonWorkHistory_MPI = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtPersonWorkHistory_MPI);
                                lintLateHoursCount = this.aclbPersonWorkHistory_MPI.First().iintLateHourCount;

                                this.aclbPersonWorkHistory_MPI = this.PaddingForBridgingService(this.aclbPersonWorkHistory_MPI);
                                this.ProcessWorkHistoryPadding(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP);

                                //PIR 355 Rohan
                                if ((iobjPassInfo != null && (iobjPassInfo.istrFormName == busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE || iobjPassInfo.istrFormName == "wfmBenefitCalculationPreRetirementDeathMaintenance"))
                                    || this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                                {
                                    ProcessWorkHistoryforBISandForfieture(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, busConstant.PERSON_ACCOUNT_STATUS_ACTIVE, this.icdoBenefitApplication.retirement_date.Year, ablnFromService);   //For CRM Bug 9922
                                }
                                else if(this.icdoBenefitApplication.min_distribution_date != DateTime.MinValue && this.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES)
                                {
                                    ProcessWorkHistoryforBISandForfieture(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, busConstant.PERSON_ACCOUNT_STATUS_ACTIVE, this.icdoBenefitApplication.retirement_date.Year, ablnFromService);  //For CRM Bug 9922
                                }
                                else
                                {
                                    ProcessWorkHistoryforBISandForfieture(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year, ablnFromService);  //For CRM Bug 9922
                                }

                                if (this.CheckAlreadyVested(busConstant.MPIPP))
                                {
                                    if (this.icdoBenefitApplication.min_distribution_date != DateTime.MinValue && this.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES)
                                    {
                                        this.Determine_VestingMPI(account.icdoPersonAccount.status_value, this.icdoBenefitApplication.retirement_date.Year, false);
                                    }
                                    else
                                    {
                                        this.Determine_VestingMPI(account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year, false);
                                    }
                                }

                            }

                            #region
                            if (ldtPersonWorkHistory_MPI.Rows.Count > 0)
                            {
                                this.aclbPersonWorkHistory_IAP = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtPersonWorkHistory_MPI);
                                foreach (cdoDummyWorkData item in this.aclbPersonWorkHistory_IAP)
                                {
                                    item.qualified_hours = item.idecIAPHours;
                                    item.vested_hours = item.idecIAPHours;
                                }
                                this.aclbPersonWorkHistory_IAP = this.PaddingForBridgingService(this.aclbPersonWorkHistory_IAP);
                                this.ProcessWorkHistoryPadding(this.aclbPersonWorkHistory_IAP, busConstant.IAP);
                                ProcessWorkHistoryforBISandForfieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year, ablnFromService);  //For CRM Bug 9922

                                if (this.CheckAlreadyVested(busConstant.IAP))
                                {
                                    if (this.icdoBenefitApplication.benefit_type_value != busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                                        this.Determine_VestingIAP(account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year, ablnSSNMerge, aobjPassInfo);
                                    else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                                        this.Determine_VestingIAP_PreRetirementDeath(account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year, ablnSSNMerge, aobjPassInfo);
                                }
                            }
                            #endregion

                            break;

                        case busConstant.IAP:

                            if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(r => r.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() == 0)
                            {
                                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                parameters[0] = param1;

                                param2.Value = busConstant.IAP;
                                parameters[1] = param2;

                                if (this.icdoBenefitApplication.retirement_date.IsNull() || this.icdoBenefitApplication.retirement_date == DateTime.MinValue)
                                {
                                    param3.Value = SqlDateTime.MinValue;
                                    parameters[2] = param3;
                                }
                                else
                                {
                                    param3.Value = this.icdoBenefitApplication.retirement_date;
                                    parameters[2] = param3;
                                }

                                if (this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date.IsNotNull() && this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date != DateTime.MinValue)
                                {
                                    param4.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date;
                                    parameters[3] = param4;
                                }
                                else
                                {
                                    param4.Value = new DateTime(1753, 1, 1);
                                    parameters[3] = param4;
                                }

                                if (this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.IsNotNull() && this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                                {
                                    param5.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                    parameters[4] = param5;
                                }
                                else
                                {
                                    param5.Value = new DateTime(1753, 1, 1);
                                    parameters[4] = param5;
                                }
                                //PIR 628
                                if (adtEACutOffdate != null)
                                {
                                    param6.Value = adtEACutOffdate;
                                    parameters[5] = param6;
                                }
                                else
                                {
                                    param6.Value = new DateTime(1753, 1, 1);
                                    parameters[5] = param6;
                                }

                                if (ablnSSNMerge == false)
                                    ldtPersonWorkHistory_IAP = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataTillGivenDate", astrLegacyDBConnetion, null, parameters);
                                else
                                    ldtPersonWorkHistory_IAP = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataTillGivenDate", astrLegacyDBConnetion, aobjPassInfo, parameters);


                                if (ldtPersonWorkHistory_IAP.Rows.Count > 0)
                                {

                                    this.aclbPersonWorkHistory_IAP = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtPersonWorkHistory_IAP);

                                    this.aclbPersonWorkHistory_IAP = this.PaddingForBridgingService(this.aclbPersonWorkHistory_IAP);

                                    this.ProcessWorkHistoryPadding(this.aclbPersonWorkHistory_IAP, busConstant.IAP);
                                    ProcessWorkHistoryforBISandForfieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year, ablnFromService);  //For CRM Bug 9922

                                    if (this.CheckAlreadyVested(busConstant.IAP))
                                    {
                                        if (this.icdoBenefitApplication.benefit_type_value != busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                                            this.Determine_VestingIAP(account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year, ablnSSNMerge, aobjPassInfo);
                                        else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                                            this.Determine_VestingIAP_PreRetirementDeath(account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year, ablnSSNMerge, aobjPassInfo);
                                    }
                                }
                            }
                            break;

                        case busConstant.Local_52:
                            ldtbPensionCredits = busBase.Select("cdoPersonAccountEligibility.GetPensionCreditsByPlanCode", new object[2] { account.icdoPersonAccount.person_account_id, busConstant.Local_52 });
                            if (ldtbPensionCredits.Rows.Count > 0 && ldtbPensionCredits.Rows[0][enmPersonAccountEligibility.pension_credits.ToString()].ToString().IsNotNullOrEmpty())
                            {
                                this.Local52_PensionCredits = Convert.ToDecimal(ldtbPensionCredits.Rows[0][enmPersonAccountEligibility.pension_credits.ToString()]);
                            }

                            #region Commented Code
                            ////FOR THE SAKE UNREDUCED EARLY ELIGIBILITY RULES WHERE MPI RULES APPLY
                            //if (this.CheckAlreadyVested(busConstant.Local_52))
                            //{
                            //    param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                            //    parameters[0] = param1;

                            //    param2.Value = busConstant.Local_52;
                            //    parameters[1] = param2;

                            //    if (this.icdoBenefitApplication.retirement_date.IsNull() || this.icdoBenefitApplication.retirement_date == DateTime.MinValue)
                            //    {
                            //        param3.Value = SqlDateTime.MinValue;
                            //        parameters[2] = param3;
                            //    }
                            //    else
                            //    {
                            //        param3.Value = this.icdoBenefitApplication.retirement_date;
                            //        parameters[2] = param3;
                            //    }

                            //    DataTable ldtbLocal52 = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataTillGivenDate", astrLegacyDBConnetion, parameters);

                            //    if (ldtbLocal52.Rows.Count > 0)
                            //    {
                            //        this.aclbPersonWorkHistory_Local52 = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtbLocal52);

                            //        this.aclbPersonWorkHistory_Local52 = this.PaddingForBridgingService(this.aclbPersonWorkHistory_Local52);
                            //    }
                            //}
                            #endregion
                            break;

                        case busConstant.Local_161:
                            ldtbPensionCredits = busBase.Select("cdoPersonAccountEligibility.GetPensionCreditsByPlanCode", new object[2] { account.icdoPersonAccount.person_account_id, busConstant.Local_161 });
                            if (ldtbPensionCredits.Rows.Count > 0 && ldtbPensionCredits.Rows[0][enmPersonAccountEligibility.pension_credits.ToString()].ToString().IsNotNullOrEmpty())
                            {
                                this.Local161_PensionCredits = Convert.ToDecimal(ldtbPensionCredits.Rows[0][enmPersonAccountEligibility.pension_credits.ToString()]);
                            }
                            break;

                        case busConstant.Local_600:
                            ldtbPensionCredits = busBase.Select("cdoPersonAccountEligibility.GetPensionCreditsByPlanCode", new object[2] { account.icdoPersonAccount.person_account_id, busConstant.Local_600 });
                            if (ldtbPensionCredits.Rows.Count > 0 && ldtbPensionCredits.Rows[0][enmPersonAccountEligibility.pension_credits.ToString()].ToString().IsNotNullOrEmpty())
                            {
                                this.Local600_PensionCredits = Convert.ToDecimal(ldtbPensionCredits.Rows[0][enmPersonAccountEligibility.pension_credits.ToString()]);
                            }

                            #region COmmented COde
                            //FOR THE SAKE UNREDUCED EARLY ELIGIBILITY RULES WHERE MPI RULES APPLY
                            //if (this.CheckAlreadyVested(busConstant.Local_600))
                            //{
                            //    param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                            //    parameters[0] = param1;

                            //    param2.Value = busConstant.Local_600;
                            //    parameters[1] = param2;

                            //    if (this.icdoBenefitApplication.retirement_date.IsNull() || this.icdoBenefitApplication.retirement_date == DateTime.MinValue)
                            //    {
                            //        param3.Value = SqlDateTime.MinValue;
                            //        parameters[2] = param3;
                            //    }
                            //    else
                            //    {
                            //        param3.Value = this.icdoBenefitApplication.retirement_date;
                            //        parameters[2] = param3;
                            //    }

                            //    DataTable ldtbLocal600 = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataTillGivenDate", astrLegacyDBConnetion, parameters);

                            //    if (ldtbLocal600.Rows.Count > 0)
                            //    {
                            //        this.aclbPersonWorkHistory_Local600 = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtbLocal600);

                            //        this.aclbPersonWorkHistory_Local600 = this.PaddingForBridgingService(this.aclbPersonWorkHistory_Local600);
                            //    }
                            //}
                            #endregion
                            break;

                        case busConstant.Local_666:
                            ldtbPensionCredits = busBase.Select("cdoPersonAccountEligibility.GetPensionCreditsByPlanCode", new object[2] { account.icdoPersonAccount.person_account_id, busConstant.Local_666 });
                            if (ldtbPensionCredits.Rows.Count > 0 && ldtbPensionCredits.Rows[0][enmPersonAccountEligibility.pension_credits.ToString()].ToString().IsNotNullOrEmpty())
                            {
                                this.Local666_PensionCredits = Convert.ToDecimal(ldtbPensionCredits.Rows[0][enmPersonAccountEligibility.pension_credits.ToString()]);
                            }

                            break;

                        case busConstant.LOCAL_700:
                            ldtbPensionCredits = busBase.Select("cdoPersonAccountEligibility.GetPensionCreditsByPlanCode", new object[2] { account.icdoPersonAccount.person_account_id, busConstant.LOCAL_700 });
                            if (ldtbPensionCredits.Rows.Count > 0 && ldtbPensionCredits.Rows[0][enmPersonAccountEligibility.pension_credits.ToString()].ToString().IsNotNullOrEmpty())
                            {
                                this.Local700_PensionCredits = Convert.ToDecimal(ldtbPensionCredits.Rows[0][enmPersonAccountEligibility.pension_credits.ToString()]);
                            }

                            #region Commented Code
                            //FOR THE SAKE UNREDUCED EARLY ELIGIBILITY RULES WHERE MPI RULES APPLY
                            //if (this.CheckAlreadyVested(busConstant.LOCAL_700))
                            //{
                            //    param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                            //    parameters[0] = param1;

                            //    param2.Value = busConstant.LOCAL_700;
                            //    parameters[1] = param2;

                            //    if (this.icdoBenefitApplication.retirement_date.IsNull() || this.icdoBenefitApplication.retirement_date == DateTime.MinValue)
                            //    {
                            //        param3.Value = SqlDateTime.MinValue;
                            //        parameters[2] = param3;
                            //    }
                            //    else
                            //    {
                            //        param3.Value = this.icdoBenefitApplication.retirement_date;
                            //        parameters[2] = param3;
                            //    }

                            //    DataTable ldtbLocal700 = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataTillGivenDate", astrLegacyDBConnetion, parameters);

                            //    if (ldtbLocal700.Rows.Count > 0)
                            //    {
                            //        this.aclbPersonWorkHistory_Local700 = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtbLocal700);

                            //        this.aclbPersonWorkHistory_Local700 = this.PaddingForBridgingService(this.aclbPersonWorkHistory_Local700);
                            //    }
                            //}
                            #endregion
                            break;

                        default: break;
                    }
                }
                #endregion
            }


            if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
            {
                foreach (cdoDummyWorkData item in this.aclbPersonWorkHistory_MPI)
                {
                    if (item.year >= busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52.Year)
                    {
                        if (item.vested_hours >= 360 && item.vested_hours <= 719)
                            this.Local52_RetirementCredits += Convert.ToDecimal(0.25);
                        else if (item.vested_hours >= 720 && item.vested_hours <= 1079)
                            this.Local52_RetirementCredits += Convert.ToDecimal(0.5);
                        else if (item.vested_hours >= 1080 && item.vested_hours <= 1439)
                            this.Local52_RetirementCredits += Convert.ToDecimal(0.75);
                        else if (item.vested_hours >= 1440)
                            this.Local52_RetirementCredits += Convert.ToDecimal(1);
                    }
                    else
                    {
                        if (item.L52_Hours >= 360 && item.L52_Hours <= 719)
                            this.Local52_RetirementCredits += Convert.ToDecimal(0.25);
                        else if (item.L52_Hours >= 720 && item.L52_Hours <= 1079)
                            this.Local52_RetirementCredits += Convert.ToDecimal(0.5);
                        else if (item.L52_Hours >= 1080 && item.L52_Hours <= 1439)
                            this.Local52_RetirementCredits += Convert.ToDecimal(0.75);
                        else if (item.L52_Hours >= 1440)
                            this.Local52_RetirementCredits += Convert.ToDecimal(1);
                    }

                    if (item.year < busConstant.BenefitCalculation.MERGER_DATE_LOCAL_161.Year)
                    {
                        if (item.L161_Hours >= 360 && item.L161_Hours <= 719)
                            this.Local161_RetirementCredits += Convert.ToDecimal(0.25);
                        else if (item.L161_Hours >= 720 && item.L161_Hours <= 1079)
                            this.Local161_RetirementCredits += Convert.ToDecimal(0.5);
                        else if (item.L161_Hours >= 1080 && item.L161_Hours <= 1439)
                            this.Local161_RetirementCredits += Convert.ToDecimal(0.75);
                        else if (item.L161_Hours >= 1440)
                            this.Local161_RetirementCredits += Convert.ToDecimal(1);
                    }
                }
            }

            if (this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES)
            {
                this.ibusPerson.icdoPerson.recalculate_vesting_flag = busConstant.FLAG_NO;

                this.ibusPerson.icdoPerson.Update();
            }
        }



        public void LoadandProcessWorkHistoryForDataExtractionBatch(DataTable adtWorkHistoryForMPI, DateTime adtForfeitureDate, ref DateTime adtVestedDate, bool ablnPopUpBenefitOption = false)
        {
            Reset_Variables();
            DataTable ldtbPensionCredits = new DataTable();

            #region Check What Plans He has and accordingly populate/Process the Work Historyfor those Plans (Essentially Prepare our summary Table)

            if (adtWorkHistoryForMPI.Rows.Count > 0)
            {

                this.aclbPersonWorkHistory_MPI = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(adtWorkHistoryForMPI);

                this.aclbPersonWorkHistory_MPI = this.PaddingForBridgingService(this.aclbPersonWorkHistory_MPI);
                this.ProcessWorkHistoryPadding(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP);
                ProcessWorkHistoryforBISandForfieture(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP);

                if (ablnPopUpBenefitOption)
                {
                    this.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, DateTime.Now);
                    this.ibusPerson.icdoPerson.istrSSNNonEncrypted = this.ibusPerson.icdoPerson.ssn;
                    this.ibusPerson.LoadPersonAccounts();
                    this.DetermineVesting(false, true);
                    if (this.icdoBenefitApplication.adtMPIVestingDate.IsNotNull() && this.icdoBenefitApplication.adtMPIVestingDate != DateTime.MinValue)
                        adtVestedDate = this.icdoBenefitApplication.adtMPIVestingDate;
                }

                if (adtVestedDate.IsNotNull() && adtVestedDate != DateTime.MinValue)
                    ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, adtVestedDate, false);

            }

            #endregion

            if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
            {
                foreach (cdoDummyWorkData item in this.aclbPersonWorkHistory_MPI)
                {
                    if (item.L52_Hours >= 360 && item.L52_Hours <= 719)
                        this.Local52_RetirementCredits += Convert.ToDecimal(0.25);
                    else if (item.L52_Hours >= 720 && item.L52_Hours <= 1079)
                        this.Local52_RetirementCredits += Convert.ToDecimal(0.5);
                    else if (item.L52_Hours >= 1080 && item.L52_Hours <= 1439)
                        this.Local52_RetirementCredits += Convert.ToDecimal(0.75);
                    else if (item.L52_Hours >= 1440)
                        this.Local52_RetirementCredits += Convert.ToDecimal(1);

                    if (item.year < busConstant.BenefitCalculation.MERGER_DATE_LOCAL_161.Year)
                    {
                        if (item.L161_Hours >= 360 && item.L161_Hours <= 719)
                            this.Local161_RetirementCredits += Convert.ToDecimal(0.25);
                        else if (item.L161_Hours >= 720 && item.L161_Hours <= 1079)
                            this.Local161_RetirementCredits += Convert.ToDecimal(0.5);
                        else if (item.L161_Hours >= 1080 && item.L161_Hours <= 1439)
                            this.Local161_RetirementCredits += Convert.ToDecimal(0.75);
                        else if (item.L161_Hours >= 1440)
                            this.Local161_RetirementCredits += Convert.ToDecimal(1);
                    }
                }
            }

            //if (this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES)
            //{
            //    this.ibusPerson.icdoPerson.recalculate_vesting_flag = busConstant.FLAG_NO;
            //    this.ibusPerson.icdoPerson.Update();
            //}
        }


        private void WorkOutVestingMPIPLan(Collection<cdoDummyWorkData> aclbPersonWorkHistory)
        {
            int lintPrevMergedYear = 0;
            string istrPrevMergedPlan = String.Empty;
            bool IsLocalVested = false;
            int lintVestingFound = 0;

            #region Code-Added Abhishek - as a new request in Sprint 2.0 (If MPIPP has work history before MERGER with EARLIEST Local - We might need to check MPI vesting on its own accord, otherwise Merger Date would be the Vesting Date if Local is VESTED
            foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount.OrderBy(item => item.icdoPersonAccount.idtMergerDate))
            {
                IsLocalVested = CheckAlreadyVested(lbusPersonAccount.icdoPersonAccount.istrPlanCode);
                // lintPrevMergedYear = lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year;
                //istrPrevMergedPlan = lbusPersonAccount.icdoPersonAccount.istrPlanCode;


                if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP && !CheckIfForfeited(lbusPersonAccount.icdoPersonAccount.person_account_id) && !(this.aclbPersonWorkHistory_MPI.IsNullOrEmpty()) && (lbusPersonAccount.icdoPersonAccount.idtMergerDate.IsNotNull() || lbusPersonAccount.icdoPersonAccount.idtMergerDate != DateTime.MinValue))
                {
                    if (IsLocalVested)
                        iblnLocalPlanExists4VestingCheck = true;

                    switch (istrPrevMergedPlan)
                    {
                        case busConstant.Local_161:
                            #region To-Setup-Vesting
                            if (IsLocalVested && this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year <= lintPrevMergedYear)
                            {
                                this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.FLAG_YES;
                                this.icdoBenefitApplication.adtMPIVestingDate = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date > lbusPersonAccount.icdoPersonAccount.idtMergerDate ? this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date : lbusPersonAccount.icdoPersonAccount.idtMergerDate;

                                if (this.aclbPersonWorkHistory_MPI.IsNotNull() && this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).Count() > 0)
                                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += busConstant.VESTED_ON_MERGER;
                                this.AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtMPIVestingDate, busConstant.MPIPP, "-ON MERGER WITH(" + istrPrevMergedPlan + ")");
                            }
                            else if (this.aclbPersonWorkHistory_MPI.Where(work => (work.qualified_hours > 0 || work.L161_Hours > 0) && work.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year).Count() > 0)
                            {
                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull())
                                    this.iclbOnlyMPI_WorkHistory.Clear();
                                else
                                    this.iclbOnlyMPI_WorkHistory = new Collection<cdoDummyWorkData>();

                                int lintStartYear = 0;
                                if (lintPrevMergedYear == 0)
                                    lintStartYear = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours > 0).First().year;
                                else
                                    lintStartYear = this.aclbPersonWorkHistory_MPI.Where(item => (item.qualified_hours > 0 || item.L161_Hours > 0)).First().year;

                                this.aclbPersonWorkHistory_MPI.ForEach(item =>
                                {
                                    if (item.year >= lintStartYear && item.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year)
                                    {
                                        cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                                        lcdoDummyWorkData.year = item.year;
                                        lcdoDummyWorkData.qualified_hours = item.qualified_hours + item.L161_Hours;
                                        lcdoDummyWorkData.vested_hours = item.vested_hours + item.L161_Hours;
                                        this.iclbOnlyMPI_WorkHistory.Add(lcdoDummyWorkData);
                                    }
                                }
                                    );

                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull() && this.iclbOnlyMPI_WorkHistory.Count > 0)
                                {
                                    this.ProcessWorkHistorySpeciallyforMPIPreMerger(this.iclbOnlyMPI_WorkHistory);
                                    this.VestingCheckingSetup(this.iclbOnlyMPI_WorkHistory, busConstant.MPIPP, lintPrevMergedYear);
                                }
                                if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES)// && this.icdoBenefitApplication.adtMPIVestingDate > lbusPersonAccount.icdoPersonAccount.idtMergerDate) || this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES)
                                {
                                    lintVestingFound = 1;
                                    //this.AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtMPIVestingDate, busConstant.MPIPP, this.iclbOnlyMPI_WorkHistory.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments);
                                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments;
                                }
                                else if (!this.ForfeitureYearsCollectionMPIPreMerger.IsNullOrEmpty())
                                {
                                    ForfeitureYearsCollectionMPIPreMerger.ForEach(year =>
                                    {
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == year).First().comments;
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().istrForfietureFlag = busConstant.FLAG_YES;
                                    }
                                    );
                                    //this.aclbPersonWorkHistory_MPI.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments;
                                    this.ForfeitureYearsCollectionMPI = this.ForfeitureYearsCollectionMPIPreMerger;
                                    //TODO Write a litte function here to adjust the Vested Year/Qualified Year Count based on the Forfeiture.
                                }
                            }

                            #endregion
                            break;

                        case busConstant.Local_600:
                            #region To-Setup-Vesting
                            if (IsLocalVested && this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year <= lintPrevMergedYear)
                            {
                                this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.FLAG_YES;
                                this.icdoBenefitApplication.adtMPIVestingDate = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date > lbusPersonAccount.icdoPersonAccount.idtMergerDate ? this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date : lbusPersonAccount.icdoPersonAccount.idtMergerDate;

                                if (this.aclbPersonWorkHistory_MPI.IsNotNull() && this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).Count() > 0)
                                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += busConstant.VESTED_ON_MERGER;
                                this.AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtMPIVestingDate, busConstant.MPIPP, "-ON MERGER WITH(" + istrPrevMergedPlan + ")");
                            }
                            else if (this.aclbPersonWorkHistory_MPI.Where(work => (work.qualified_hours > 0 || work.L600_Hours > 0) && work.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year).Count() > 0)
                            {
                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull())
                                    this.iclbOnlyMPI_WorkHistory.Clear();
                                else
                                    this.iclbOnlyMPI_WorkHistory = new Collection<cdoDummyWorkData>();

                                int lintStartYear = 0;
                                if (lintPrevMergedYear == 0)
                                    lintStartYear = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours > 0).First().year;
                                else
                                    lintStartYear = this.aclbPersonWorkHistory_MPI.Where(item => (item.qualified_hours > 0 || item.L600_Hours > 0)).First().year;

                                this.aclbPersonWorkHistory_MPI.ForEach(item =>
                                {
                                    if (item.year >= lintStartYear && item.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year)
                                    {
                                        cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                                        lcdoDummyWorkData.year = item.year;
                                        lcdoDummyWorkData.qualified_hours = item.qualified_hours + item.L600_Hours;
                                        lcdoDummyWorkData.vested_hours = item.vested_hours + item.L600_Hours;
                                        this.iclbOnlyMPI_WorkHistory.Add(lcdoDummyWorkData);
                                    }
                                }
                                    );

                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull() && this.iclbOnlyMPI_WorkHistory.Count > 0)
                                {
                                    this.ProcessWorkHistorySpeciallyforMPIPreMerger(this.iclbOnlyMPI_WorkHistory);
                                    this.VestingCheckingSetup(this.iclbOnlyMPI_WorkHistory, busConstant.MPIPP, lintPrevMergedYear);
                                }

                                if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES)// && this.icdoBenefitApplication.adtMPIVestingDate > lbusPersonAccount.icdoPersonAccount.idtMergerDate) || this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES)
                                {
                                    lintVestingFound = 1;
                                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments;
                                }
                                else if (!this.ForfeitureYearsCollectionMPIPreMerger.IsNullOrEmpty())
                                {
                                    ForfeitureYearsCollectionMPIPreMerger.ForEach(year =>
                                    {
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == year).First().comments;
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().istrForfietureFlag = busConstant.FLAG_YES;
                                    }
                                    );
                                    //this.aclbPersonWorkHistory_MPI.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments;
                                    this.ForfeitureYearsCollectionMPI = this.ForfeitureYearsCollectionMPIPreMerger;
                                    //TODO Write a litte function here to adjust the Vested Year/Qualified Year Count based on the Forfeiture.
                                }
                            }

                            #endregion
                            break;

                        case busConstant.Local_666:
                            #region To-Setup-Vesting
                            if (IsLocalVested && this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year <= lintPrevMergedYear)
                            {
                                this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.FLAG_YES;
                                this.icdoBenefitApplication.adtMPIVestingDate = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date > lbusPersonAccount.icdoPersonAccount.idtMergerDate ? this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date : lbusPersonAccount.icdoPersonAccount.idtMergerDate;

                                if (this.aclbPersonWorkHistory_MPI.IsNotNull() && this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).Count() > 0)
                                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += busConstant.VESTED_ON_MERGER;
                                this.AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtMPIVestingDate, busConstant.MPIPP, "-ON MERGER WITH(" + istrPrevMergedPlan + ")");
                            }
                            else if (this.aclbPersonWorkHistory_MPI.Where(work => (work.qualified_hours > 0 || work.L666_Hours > 0) && work.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year).Count() > 0)
                            {
                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull())
                                    this.iclbOnlyMPI_WorkHistory.Clear();
                                else
                                    this.iclbOnlyMPI_WorkHistory = new Collection<cdoDummyWorkData>();

                                int lintStartYear = 0;
                                if (lintPrevMergedYear == 0)
                                    lintStartYear = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours > 0).First().year;
                                else
                                    lintStartYear = this.aclbPersonWorkHistory_MPI.Where(item => (item.qualified_hours > 0 || item.L666_Hours > 0)).First().year;

                                this.aclbPersonWorkHistory_MPI.ForEach(item =>
                                {
                                    if (item.year >= lintStartYear && item.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year)
                                    {
                                        cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                                        lcdoDummyWorkData.year = item.year;
                                        lcdoDummyWorkData.qualified_hours = item.qualified_hours + item.L666_Hours;
                                        lcdoDummyWorkData.vested_hours = item.vested_hours + item.L666_Hours;
                                        this.iclbOnlyMPI_WorkHistory.Add(lcdoDummyWorkData);
                                    }
                                }
                                    );

                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull() && this.iclbOnlyMPI_WorkHistory.Count > 0)
                                {
                                    this.ProcessWorkHistorySpeciallyforMPIPreMerger(this.iclbOnlyMPI_WorkHistory);
                                    this.VestingCheckingSetup(this.iclbOnlyMPI_WorkHistory, busConstant.MPIPP, lintPrevMergedYear);
                                }
                                if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES)// && this.icdoBenefitApplication.adtMPIVestingDate > lbusPersonAccount.icdoPersonAccount.idtMergerDate) || this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES)
                                {
                                    lintVestingFound = 1;
                                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments;
                                }
                                else if (!this.ForfeitureYearsCollectionMPIPreMerger.IsNullOrEmpty())
                                {
                                    ForfeitureYearsCollectionMPIPreMerger.ForEach(year =>
                                    {
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == year).First().comments;
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().istrForfietureFlag = busConstant.FLAG_YES;
                                    }
                                    );
                                    //this.aclbPersonWorkHistory_MPI.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments;
                                    this.ForfeitureYearsCollectionMPI = this.ForfeitureYearsCollectionMPIPreMerger;
                                    //TODO Write a litte function here to adjust the Vested Year/Qualified Year Count based on the Forfeiture.
                                }
                            }

                            #endregion
                            break;

                        case busConstant.LOCAL_700:
                            #region To-Setup-Vesting
                            if (IsLocalVested && this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year <= lintPrevMergedYear)
                            {
                                this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.FLAG_YES;
                                this.icdoBenefitApplication.adtMPIVestingDate = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date > lbusPersonAccount.icdoPersonAccount.idtMergerDate ? this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date : lbusPersonAccount.icdoPersonAccount.idtMergerDate;

                                if (this.aclbPersonWorkHistory_MPI.IsNotNull() && this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).Count() > 0)
                                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += busConstant.VESTED_ON_MERGER;
                                this.AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtMPIVestingDate, busConstant.MPIPP, "-ON MERGER WITH(" + istrPrevMergedPlan + ")");
                            }
                            else if (this.aclbPersonWorkHistory_MPI.Where(work => (work.qualified_hours > 0 || work.L700_Hours > 0) && work.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year).Count() > 0)
                            {
                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull())
                                    this.iclbOnlyMPI_WorkHistory.Clear();
                                else
                                    this.iclbOnlyMPI_WorkHistory = new Collection<cdoDummyWorkData>();

                                int lintStartYear = 0;
                                if (lintPrevMergedYear == 0)
                                    lintStartYear = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours > 0).First().year;
                                else
                                    lintStartYear = this.aclbPersonWorkHistory_MPI.Where(item => (item.qualified_hours > 0 || item.L700_Hours > 0)).First().year;

                                this.aclbPersonWorkHistory_MPI.ForEach(item =>
                                {
                                    if (item.year >= lintStartYear && item.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year)
                                    {
                                        cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                                        lcdoDummyWorkData.year = item.year;
                                        lcdoDummyWorkData.qualified_hours = item.qualified_hours + item.L700_Hours;
                                        lcdoDummyWorkData.vested_hours = item.vested_hours + item.L700_Hours;
                                        this.iclbOnlyMPI_WorkHistory.Add(lcdoDummyWorkData);
                                    }
                                }
                                    );

                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull() && this.iclbOnlyMPI_WorkHistory.Count > 0)
                                {
                                    this.ProcessWorkHistorySpeciallyforMPIPreMerger(this.iclbOnlyMPI_WorkHistory);
                                    this.VestingCheckingSetup(this.iclbOnlyMPI_WorkHistory, busConstant.MPIPP, lintPrevMergedYear);
                                }

                                if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES)// && this.icdoBenefitApplication.adtMPIVestingDate > lbusPersonAccount.icdoPersonAccount.idtMergerDate) || this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES)
                                {
                                    lintVestingFound = 1;
                                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments;
                                }
                                else if (!this.ForfeitureYearsCollectionMPIPreMerger.IsNullOrEmpty())
                                {
                                    ForfeitureYearsCollectionMPIPreMerger.ForEach(year =>
                                    {
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == year).First().comments;
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().istrForfietureFlag = busConstant.FLAG_YES;
                                    }
                                    );
                                    //this.aclbPersonWorkHistory_MPI.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments;
                                    this.ForfeitureYearsCollectionMPI = this.ForfeitureYearsCollectionMPIPreMerger;
                                    //TODO Write a litte function here to adjust the Vested Year/Qualified Year Count based on the Forfeiture.
                                }
                            }

                            #endregion
                            break;

                        case busConstant.Local_52:
                            #region To-Setup-Vesting
                            if (IsLocalVested && this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year <= lintPrevMergedYear)
                            {
                                this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.FLAG_YES;
                                this.icdoBenefitApplication.adtMPIVestingDate = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date > lbusPersonAccount.icdoPersonAccount.idtMergerDate ? this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date : lbusPersonAccount.icdoPersonAccount.idtMergerDate;

                                if (this.aclbPersonWorkHistory_MPI.IsNotNull() && this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).Count() > 0)
                                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += busConstant.VESTED_ON_MERGER;
                                this.AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtMPIVestingDate, busConstant.MPIPP, "-ON MERGER WITH(" + istrPrevMergedPlan + ")");
                            }
                            else if (this.aclbPersonWorkHistory_MPI.Where(work => (work.qualified_hours > 0 || work.L52_Hours > 0) && work.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year).Count() > 0)
                            {
                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull())
                                    this.iclbOnlyMPI_WorkHistory.Clear();
                                else
                                    this.iclbOnlyMPI_WorkHistory = new Collection<cdoDummyWorkData>();

                                int lintStartYear = 0;
                                if (lintPrevMergedYear == 0)
                                    lintStartYear = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours > 0).First().year;
                                else
                                    lintStartYear = this.aclbPersonWorkHistory_MPI.Where(item => (item.qualified_hours > 0 || item.L52_Hours > 0)).First().year;

                                this.aclbPersonWorkHistory_MPI.ForEach(item =>
                                {
                                    if (item.year >= lintStartYear && item.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year)
                                    {
                                        cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                                        lcdoDummyWorkData.year = item.year;
                                        lcdoDummyWorkData.qualified_hours = item.qualified_hours + item.L52_Hours;
                                        lcdoDummyWorkData.vested_hours = item.vested_hours + item.L52_Hours;
                                        this.iclbOnlyMPI_WorkHistory.Add(lcdoDummyWorkData);
                                    }
                                }
                                    );

                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull() && this.iclbOnlyMPI_WorkHistory.Count > 0)
                                {
                                    this.ProcessWorkHistorySpeciallyforMPIPreMerger(this.iclbOnlyMPI_WorkHistory);
                                    this.VestingCheckingSetup(this.iclbOnlyMPI_WorkHistory, busConstant.MPIPP, lintPrevMergedYear);
                                }

                                if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES)// && this.icdoBenefitApplication.adtMPIVestingDate > lbusPersonAccount.icdoPersonAccount.idtMergerDate) || this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES)
                                {
                                    lintVestingFound = 1;
                                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments;
                                }
                                else if (!this.ForfeitureYearsCollectionMPIPreMerger.IsNullOrEmpty())
                                {
                                    ForfeitureYearsCollectionMPIPreMerger.ForEach(year =>
                                    {
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == year).First().comments;
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().istrForfietureFlag = busConstant.FLAG_YES;
                                    }
                                    );
                                    //this.aclbPersonWorkHistory_MPI.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments;
                                    this.ForfeitureYearsCollectionMPI = this.ForfeitureYearsCollectionMPIPreMerger;
                                    //TODO Write a litte function here to adjust the Vested Year/Qualified Year Count based on the Forfeiture.
                                }
                            }

                            #endregion
                            break;

                        default:
                            #region To-Setup-Vesting

                            if (this.aclbPersonWorkHistory_MPI.Where(work => work.qualified_hours > 0 && work.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year).Count() > 0)
                            {
                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull())
                                    this.iclbOnlyMPI_WorkHistory.Clear();
                                else
                                    this.iclbOnlyMPI_WorkHistory = new Collection<cdoDummyWorkData>();

                                int lintStartYear = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours > 0).First().year;

                                this.aclbPersonWorkHistory_MPI.ForEach(item =>
                                {
                                    if (item.year >= lintStartYear && item.year < lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year)
                                    {
                                        cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                                        lcdoDummyWorkData.year = item.year;
                                        lcdoDummyWorkData.qualified_hours = item.qualified_hours;
                                        lcdoDummyWorkData.vested_hours = item.vested_hours;
                                        this.iclbOnlyMPI_WorkHistory.Add(lcdoDummyWorkData);
                                    }
                                }
                                    );

                                if (this.iclbOnlyMPI_WorkHistory.IsNotNull() && this.iclbOnlyMPI_WorkHistory.Count > 0)
                                {
                                    this.ProcessWorkHistorySpeciallyforMPIPreMerger(this.iclbOnlyMPI_WorkHistory);
                                    this.VestingCheckingSetup(this.iclbOnlyMPI_WorkHistory, busConstant.MPIPP, 0);
                                }

                                if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES)// && this.icdoBenefitApplication.adtMPIVestingDate > lbusPersonAccount.icdoPersonAccount.idtMergerDate) || this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES)
                                {
                                    lintVestingFound = 1;
                                    if (this.iclbOnlyMPI_WorkHistory.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).Count() > 0)
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments;
                                }
                                else if (!this.ForfeitureYearsCollectionMPIPreMerger.IsNullOrEmpty())
                                {
                                    ForfeitureYearsCollectionMPIPreMerger.ForEach(year =>
                                    {
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == year).First().comments;
                                        this.aclbPersonWorkHistory_MPI.Where(item => item.year == year).First().istrForfietureFlag = busConstant.FLAG_YES;
                                    }
                                    );

                                    //this.aclbPersonWorkHistory_MPI.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments += this.iclbOnlyMPI_WorkHistory.Where(item => item.year == ForfeitureYearsCollectionMPIPreMerger.Last()).First().comments;
                                    if (this.ForfeitureYearsCollectionMPIPreMerger != null && this.ForfeitureYearsCollectionMPIPreMerger.Count() > 0)
                                    {
                                        #region Code changes done
                                        //Rohan
                                        foreach (int ForfeitureYearMPIPreMerger in ForfeitureYearsCollectionMPIPreMerger)
                                        {
                                            if (!ForfeitureYearsCollectionMPI.Contains(ForfeitureYearMPIPreMerger))
                                                this.ForfeitureYearsCollectionMPI.Add(ForfeitureYearMPIPreMerger);
                                        }

                                        ArrayList larrForfeitureYearsCollectionMPI = new ArrayList();
                                        ForfeitureYearsCollectionMPI.ForEach(year =>
                                        {
                                            larrForfeitureYearsCollectionMPI.Add(year);
                                        });
                                        larrForfeitureYearsCollectionMPI.Sort();
                                        ForfeitureYearsCollectionMPI.Clear();

                                        foreach (int larrForfeitureYearsMPI in larrForfeitureYearsCollectionMPI)
                                        {
                                            ForfeitureYearsCollectionMPI.Add(larrForfeitureYearsMPI);
                                        }
                                        #endregion

                                    }
                                    //TODO Write a litte function here to adjust the Vested Year/Qualified Year Count based on the Forfeiture.
                                }
                            }
                            if (IsLocalVested && this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES
                                && (this.ForfeitureYearsCollectionMPIPreMerger.IsNullOrEmpty() || (!this.ForfeitureYearsCollectionMPIPreMerger.IsNullOrEmpty() && this.aclbPersonWorkHistory_MPI.Where(work => work.qualified_hours > 0 && work.year > this.ForfeitureYearsCollectionMPIPreMerger.Last()).Count() > 0))
                                )
                            {

                                this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.FLAG_YES;
                                lintVestingFound = 1;
                                this.icdoBenefitApplication.adtMPIVestingDate = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date > lbusPersonAccount.icdoPersonAccount.idtMergerDate ? this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date : lbusPersonAccount.icdoPersonAccount.idtMergerDate;


                                DateTime ldtMergedDate = lbusPersonAccount.icdoPersonAccount.idtMergerDate;

                                //PIR 548 - Vesting Logic
                                #region Need to check for some more examples.Think. Commented code
                                //if (IsLocalVested && !ForfeitureYearsCollectionMPI.IsNullOrEmpty() &&
                                //    Convert.ToInt32(this.ForfeitureYearsCollectionMPI.Last().ToString()) < ldtMergedDate.Year)
                                //{
                                //    ;
                                //}
                                //else
                                //{
                                //    ForfeitureYearsCollectionMPI.Clear();
                                //    ForfeitureYearsCollectionMPIPreMerger.Clear();
                                //}
                                #endregion

                                //ROhan
                                this.ForfeitureYearsCollectionMPI = this.ForfeitureYearsCollectionMPIPreMerger;


                                if (this.aclbPersonWorkHistory_MPI.IsNotNull() && this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).Count() > 0)
                                    this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.icdoBenefitApplication.adtMPIVestingDate.Year).First().comments += busConstant.VESTED_ON_MERGER;
                                this.AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtMPIVestingDate, busConstant.MPIPP, "-ON MERGER WITH (" + lbusPersonAccount.icdoPersonAccount.istrPlanCode + ")");
                                this.AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtMPIVestingDate, busConstant.IAP, "-ON MERGER WITH (" + lbusPersonAccount.icdoPersonAccount.istrPlanCode + ")");

                            }
                            #endregion
                            break;

                    }

                    lintPrevMergedYear = lbusPersonAccount.icdoPersonAccount.idtMergerDate.Year;
                    istrPrevMergedPlan = lbusPersonAccount.icdoPersonAccount.istrPlanCode;

                    if (lintVestingFound == 1)
                        break;
                }
            }
            #endregion
        }

        //Code-Abhishek
        // Set benefit sub type based on the member's eligibility and vesting for Retirement 
        public void DetermineVesting(bool ablnBISBatch = false, bool ablnAnnualStmt = false)
        {
            int lintLateHoursExist = 0;

            if (lintLateHoursCount.IsNotNull() && lintLateHoursCount > 0)
                lintLateHoursExist = lintLateHoursCount;

            if (astrLegacyDBConnetion.IsNullOrEmpty())
            {
                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            }

            SqlParameter[] lParameters = new SqlParameter[3];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            SqlParameter param2 = new SqlParameter("@EVALUATION_DATE ", DbType.DateTime);
            SqlParameter param3 = new SqlParameter("@VESTING_DATE", DbType.DateTime);

            foreach (busPersonAccount account in this.ibusPerson.iclbPersonAccount)
            {
                switch (account.icdoPersonAccount.istrPlanCode)
                {
                    #region CHECK if VESTING CALCULATION REALLY REQUIRED OR NOT
                    case busConstant.MPIPP:
                        //TODO
                        //First Check If Person is Already Vested then Check IF he has LATE HOURS before Vesting Date then Recalculate.
                        //Else we need to return with the FLAG BEING SET    
                        if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                        {
                            #region Vesting Checking MPI Plan
                            iblnLocalPlanExists4VestingCheck = false;

                            if (!ablnBISBatch && CheckAlreadyVested(busConstant.MPIPP))
                            {
                                //Check for LATE HOURS 

                                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                lParameters[0] = param1;

                                param2.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date;
                                lParameters[1] = param2;

                                param3.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                lParameters[2] = param3;

                                //ldtLateHoursCount = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetCountForLateHours", astrLegacyDBConnetion, null, lParameters);
                                //if (ldtLateHoursCount.Rows.Count > 0)
                                //  lintLateHoursExist = Convert.ToInt32(ldtLateHoursCount.Rows[0][0]);
                            }
                            else
                            {
                                lintLateHoursExist = 1;     //To Avoid EADB hit when called from BIS Batch
                            }

                            if (lintLateHoursExist > 0 || this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES ||
                                !(this.aclbPersonWorkHistory_MPI.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).Count() > 0))
                            {
                                if (this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES || ablnBISBatch)
                                {
                                    RemoveForfietureDateFromPersonAccountEligibility(busConstant.MPIPP);
                                }

                                if (this.ibusPerson.IsNotNull() && this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(local => local.icdoPersonAccount.istrPlanCode.Contains("Local")).Count() > 0)
                                {
                                    WorkOutVestingMPIPLan(aclbPersonWorkHistory_MPI);

                                    if (!iblnLocalPlanExists4VestingCheck)
                                        this.VestingCheckingSetup(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, 0, ablnBISBatch);
                                }
                                else if (!iblnLocalPlanExists4VestingCheck)
                                {
                                    if (!(this.aclbPersonWorkHistory_MPI.IsNullOrEmpty()))
                                        this.VestingCheckingSetup(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, 0, ablnBISBatch);
                                }

                                if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES)
                                    ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, this.icdoBenefitApplication.adtMPIVestingDate, true, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year);
                                else if (!this.ForfeitureYearsCollectionMPI.IsNullOrEmpty())
                                {
                                    if ((this.icdoBenefitApplication.adtMPIVestingDate.IsNotNull() && ForfeitureYearsCollectionMPI.Last() < this.icdoBenefitApplication.adtMPIVestingDate.Year)
                                        || this.icdoBenefitApplication.adtMPIVestingDate.IsNull() || this.icdoBenefitApplication.adtMPIVestingDate == DateTime.MinValue)
                                        AddUpdateForfeitureDate(ForfeitureYearsCollectionMPI.Last(), busConstant.MPIPP, true);

                                    if (!iblnLocalPlanExists4VestingCheck && ablnBISBatch == false)
                                        RemoveUnwantedPadding(this.aclbPersonWorkHistory_MPI, busConstant.MPIPP, ForfeitureYearsCollectionMPI.Last());
                                }
                                else
                                {
                                    if (this.icdoBenefitApplication.istrIsPersonVestedinMPI.IsNullOrEmpty() || this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES)
                                        AddUpdatePersonAccountEligibility(DateTime.MinValue, busConstant.MPIPP, String.Empty);
                                }

                                if (ablnBISBatch == false)
                                    this.PopulateLocalsRelatedInformation(this.aclbPersonWorkHistory_MPI);
                            }

                            #endregion
                        }

                        break;

                    case busConstant.IAP:
                        //TODO
                        //First Check If Person is Already Vested then Check IF he has LATE HOURS before Vesting Date then Recalculate.
                        //Else we need to return with the FLAG BEING SET
                        if (!this.aclbPersonWorkHistory_IAP.IsNullOrEmpty() && this.icdoBenefitApplication.benefit_type_value != busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT && ablnAnnualStmt == false)
                        {
                            #region Vesting Checking IAP Plan
                            if (!ablnBISBatch && CheckAlreadyVested(busConstant.IAP))
                            {

                                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                lParameters[0] = param1;

                                param2.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date;
                                lParameters[1] = param2;

                                param3.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                lParameters[2] = param3;

                                //ldtLateHoursCount = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetCountForLateHours", astrLegacyDBConnetion, null, lParameters);
                                //if (ldtLateHoursCount.Rows.Count > 0)
                                //lintLateHoursExist = Convert.ToInt32(ldtLateHoursCount.Rows[0][0]);
                                if (lintLateHoursExist > 0 || this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES ||
                                    !(this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).Count() > 0))
                                {

                                    if (this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES)
                                    {
                                        RemoveForfietureDateFromPersonAccountEligibility(busConstant.IAP);
                                    }

                                    if (!(this.aclbPersonWorkHistory_IAP.IsNullOrEmpty()))
                                        this.VestingCheckingSetup(this.aclbPersonWorkHistory_IAP, busConstant.IAP, 0, ablnBISBatch);

                                    if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES)
                                        ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.icdoBenefitApplication.adtIAPVestingDate, true, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year);
                                    else if (!this.ForfeitureYearsCollectionIAP.IsNullOrEmpty())
                                    {

                                        if ((this.icdoBenefitApplication.adtIAPVestingDate.IsNotNull() && ForfeitureYearsCollectionIAP.Last() < this.icdoBenefitApplication.adtIAPVestingDate.Year)
                                                || this.icdoBenefitApplication.adtIAPVestingDate.IsNull() || this.icdoBenefitApplication.adtIAPVestingDate == DateTime.MinValue)
                                            AddUpdateForfeitureDate(ForfeitureYearsCollectionIAP.Last(), busConstant.IAP, true);

                                        if (ablnBISBatch == false)
                                            RemoveUnwantedPadding(this.aclbPersonWorkHistory_IAP, busConstant.IAP, ForfeitureYearsCollectionIAP.Last());
                                    }
                                    else
                                    {
                                        if (this.icdoBenefitApplication.istrIsPersonVestedinIAP.IsNullOrEmpty() || this.icdoBenefitApplication.istrIsPersonVestedinIAP != busConstant.FLAG_YES)
                                            AddUpdatePersonAccountEligibility(DateTime.MinValue, busConstant.IAP, String.Empty);
                                    }

                                }
                            }
                            else if ((ablnBISBatch && !this.idrWeeklyWorkData.IsNullOrEmpty()) || !ablnBISBatch)
                            {
                                RemoveForfietureDateFromPersonAccountEligibility(busConstant.IAP); //Vesting Forfeiture Changes

                                if (!(this.aclbPersonWorkHistory_IAP.IsNullOrEmpty()))
                                    this.VestingCheckingSetup(this.aclbPersonWorkHistory_IAP, busConstant.IAP, 0, ablnBISBatch);

                                if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES)
                                    ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.icdoBenefitApplication.adtIAPVestingDate, true, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year);
                                else if (!this.ForfeitureYearsCollectionIAP.IsNullOrEmpty())
                                {
                                    if ((this.icdoBenefitApplication.adtIAPVestingDate.IsNotNull() && ForfeitureYearsCollectionIAP.Last() < this.icdoBenefitApplication.adtIAPVestingDate.Year)
                                            || this.icdoBenefitApplication.adtIAPVestingDate.IsNull() || this.icdoBenefitApplication.adtIAPVestingDate == DateTime.MinValue)
                                        AddUpdateForfeitureDate(ForfeitureYearsCollectionIAP.Last(), busConstant.IAP, true);

                                    if (ablnBISBatch == false)
                                        RemoveUnwantedPadding(this.aclbPersonWorkHistory_IAP, busConstant.IAP, ForfeitureYearsCollectionIAP.Last());
                                }
                                else
                                {
                                    if (this.icdoBenefitApplication.istrIsPersonVestedinIAP.IsNullOrEmpty() || this.icdoBenefitApplication.istrIsPersonVestedinIAP != busConstant.FLAG_YES)
                                        AddUpdatePersonAccountEligibility(DateTime.MinValue, busConstant.IAP, String.Empty);
                                }
                            }
                            #endregion
                        }
                        else if (!this.aclbPersonWorkHistory_IAP.IsNullOrEmpty() && this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT && ablnAnnualStmt == false)
                        {
                            #region Vesting Checking IAP Plan in-case of Pre-Retirement Death
                            if (!ablnBISBatch && CheckAlreadyVested(busConstant.IAP))
                            {
                                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                lParameters[0] = param1;

                                param2.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date;
                                lParameters[1] = param2;

                                param3.Value = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                lParameters[2] = param3;

                                //ldtLateHoursCount = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetCountForLateHours", astrLegacyDBConnetion, null, lParameters);
                                //if (ldtLateHoursCount.Rows.Count > 0)
                                //lintLateHoursExist = Convert.ToInt32(ldtLateHoursCount.Rows[0][0]);

                                if (lintLateHoursExist > 0 || this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES ||
                                    !(this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.Year).Count() > 0))
                                {

                                    if (this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES)
                                    {
                                        RemoveForfietureDateFromPersonAccountEligibility(busConstant.IAP);
                                    }

                                    if (!(this.aclbPersonWorkHistory_IAP.IsNullOrEmpty())) { }
                                    this.VestingCheckingSetup(this.aclbPersonWorkHistory_IAP, busConstant.IAP, 0);

                                    if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES)
                                        ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.icdoBenefitApplication.adtIAPVestingDate, true, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year);
                                    else if (!this.ForfeitureYearsCollectionIAP.IsNullOrEmpty())
                                    {
                                        if (this.ForfeitureYearsCollectionIAP.Last() > this.ibusPerson.icdoPerson.date_of_death.Year)
                                        {
                                            this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                            if (this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).Count() > 0)
                                                this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).First().comments += busConstant.VESTED_COMMENT + "VestedOnDeath";
                                            AddUpdatePersonAccountEligibility(this.ibusPerson.icdoPerson.date_of_death, busConstant.IAP, "VestedonDeath");
                                            ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.ibusPerson.icdoPerson.date_of_death, true, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year);
                                        }
                                        else
                                        {
                                            if ((this.icdoBenefitApplication.adtIAPVestingDate.IsNotNull() && ForfeitureYearsCollectionIAP.Last() < this.icdoBenefitApplication.adtIAPVestingDate.Year)
                                                    || this.icdoBenefitApplication.adtIAPVestingDate.IsNull() || this.icdoBenefitApplication.adtIAPVestingDate == DateTime.MinValue)
                                                AddUpdateForfeitureDate(ForfeitureYearsCollectionIAP.Last(), busConstant.IAP, true);

                                            RemoveUnwantedPadding(this.aclbPersonWorkHistory_IAP, busConstant.IAP, ForfeitureYearsCollectionIAP.Last());
                                        }
                                    }
                                    else
                                    {
                                        this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                        if (this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).Count() > 0)
                                            this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).First().comments += busConstant.VESTED_COMMENT + "VestedOnDeath";
                                        AddUpdatePersonAccountEligibility(this.ibusPerson.icdoPerson.date_of_death, busConstant.IAP, "VestedonDeath");
                                        ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.ibusPerson.icdoPerson.date_of_death, true, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year);
                                    }
                                }
                            }
                            else
                            {
                                if (!(this.aclbPersonWorkHistory_IAP.IsNullOrEmpty()))
                                    this.VestingCheckingSetup(this.aclbPersonWorkHistory_IAP, busConstant.IAP, 0);

                                if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES)
                                    ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.icdoBenefitApplication.adtIAPVestingDate, true, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year);
                                else if (!this.ForfeitureYearsCollectionIAP.IsNullOrEmpty())
                                {
                                    if (this.ForfeitureYearsCollectionIAP.Last() > this.ibusPerson.icdoPerson.date_of_death.Year)
                                    {
                                        this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                        if (this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).Count() > 0)
                                            this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).First().comments += busConstant.VESTED_COMMENT + "VestedOnDeath";
                                        AddUpdatePersonAccountEligibility(this.ibusPerson.icdoPerson.date_of_death, busConstant.IAP, "VestedonDeath");
                                        ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.ibusPerson.icdoPerson.date_of_death, true, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year);
                                    }
                                    else
                                    {
                                        if ((this.icdoBenefitApplication.adtIAPVestingDate.IsNotNull() && ForfeitureYearsCollectionIAP.Last() < this.icdoBenefitApplication.adtIAPVestingDate.Year)
                                                || this.icdoBenefitApplication.adtIAPVestingDate.IsNull() || this.icdoBenefitApplication.adtIAPVestingDate == DateTime.MinValue)
                                            AddUpdateForfeitureDate(ForfeitureYearsCollectionIAP.Last(), busConstant.IAP, true);
                                        RemoveUnwantedPadding(this.aclbPersonWorkHistory_IAP, busConstant.IAP, ForfeitureYearsCollectionIAP.Last());
                                    }
                                }
                                else
                                {
                                    this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                    if (this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).Count() > 0)
                                        this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.ibusPerson.icdoPerson.date_of_death.Year).First().comments += busConstant.VESTED_COMMENT + "VestedOnDeath";
                                    AddUpdatePersonAccountEligibility(this.ibusPerson.icdoPerson.date_of_death, busConstant.IAP, "VestedonDeath");
                                    ProcessWorkHistorytoRemoveUnwantedForFieture(this.aclbPersonWorkHistory_IAP, busConstant.IAP, this.ibusPerson.icdoPerson.date_of_death, true, account.icdoPersonAccount.status_value, account.icdoPersonAccount.end_date.Year);
                                }
                            }
                            #endregion
                        }

                        break;
                    #endregion
                }

            }
            if (this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES)
            {
                this.ibusPerson.icdoPerson.recalculate_vesting_flag = busConstant.FLAG_NO;
                this.ibusPerson.icdoPerson.Update();
            }
        }

        public void DetermineBenefitSubTypeandEligibility_Retirement()
        {
            this.CheckEligibility_Retirement();
            if (Eligible_Plans.IsNullOrEmpty())
                NotEligible = true;
            else
            {
                iclbEligiblePlans = Eligible_Plans;
                NotEligible = false;
            }

        }

        public void DetermineBenefitSubTypeandEligibility_Disability()
        {
            this.CheckEligibility_Disability();
            if (Eligible_Plans.IsNullOrEmpty())
                NotEligible = true;
            else
            {
                iclbEligiblePlans = Eligible_Plans;
                NotEligible = false;
            }

        }

        public void DetermineBenefitSubTypeandEligibility_Withdrawal()
        {
            this.CheckEligiblity_Withdrawal();
            if (Eligible_Plans.IsNullOrEmpty())
                NotEligible = true;
            else
            {
                iclbEligiblePlans = Eligible_Plans;
                NotEligible = false;
            }

        }

        public void DetermineBenefitSubTypeandEligibility_DeathPreRetirement()
        {
            this.CheckEligibility_DeathPreRetirement();
            if (Eligible_Plans.IsNullOrEmpty())
                NotEligible = true;
            else
            {
                iclbEligiblePlans = Eligible_Plans;
                NotEligible = false;
            }

        }

        public void SetWorkflowRelatedVariablesforFinalCalculation(string astrPlanCode, int aintCalculationHeaderId)
        {
            switch (astrPlanCode)
            {
                case busConstant.MPIPP:
                    MPI_Final_Calculation_Id = aintCalculationHeaderId;
                    MPIPPPlan_Selected = true;
                    break;

                case busConstant.IAP:
                    IAP_Final_Calculation_Id = aintCalculationHeaderId;
                    IAPPlan_Selected = true;
                    break;

                case busConstant.Local_52:
                    L52_Final_Calculation_Id = aintCalculationHeaderId;
                    L52Plan_Selected = true;
                    break;

                case busConstant.Local_161:
                    L161_Final_Calculation_Id = aintCalculationHeaderId;
                    L161Plan_Selected = true;
                    break;

                case busConstant.Local_600:
                    L600_Final_Calculation_Id = aintCalculationHeaderId;
                    L600Plan_Selected = true;
                    break;

                case busConstant.Local_666:
                    L666_Final_Calculation_Id = aintCalculationHeaderId;
                    L666Plan_Selected = true;
                    break;

                case busConstant.LOCAL_700:
                    L700_Final_Calculation_Id = aintCalculationHeaderId;
                    L700Plan_Selected = true;
                    break;
            }
        }
        //Code-Abhishek


        public void CheckEligibility_Disability()
        {
            //Eligibility needs to be checked for each plan   
            if (!Eligible_Plans.IsNullOrEmpty())
            {
                Eligible_Plans.Clear();
            }
            if (this.icdoBenefitApplication.retirement_date == DateTime.MinValue)
            {
                return;
            }
            //PIR - 954
            if (this.icdoBenefitApplication.disability_onset_date == DateTime.MinValue)
            {
                return;
            }

            //DataTable ldtbEligibility = busBase.Select("cdoBenefitProvisionEligibility.FetchAllEligibilityRules", new object[0] { });
            //Collection<cdoBenefitProvisionEligibility> aclbAllEligibilityRules = new Collection<cdoBenefitProvisionEligibility>(); // Collection that will store the Eligibility Rule                        
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                //aclbAllEligibilityRules = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(ldtbEligibility);
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();
                bool EligibleforMPIPP = false;

                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).Count() > 0)
                {
                    bool lblnIsBIS = false;
                    Collection<cdoDummyWorkData> lclbPersonWorkHistoryBeforeRetirement = new Collection<cdoDummyWorkData>();
                    lclbPersonWorkHistoryBeforeRetirement = this.aclbPersonWorkHistory_MPI.Where(item => !item.iblnHoursAfterRetirement).ToList().ToCollection();
                    cdoBenefitProvisionEligibility lcdoBenefitProvisionEligibility = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY).FirstOrDefault();

                    //int Year = 0;
                    //PIR - 954
                    //int lintYearDiff =
                    //    this.icdoBenefitApplication.retirement_date.Year - 1 - lclbPersonWorkHistoryBeforeRetirement.Where(t=>t.year <= icdoBenefitApplication.disability_onset_date.Year -1).Last().year;
                    //if (lintYearDiff < 2)
                    //{

                        //PIR - 954
                        //if (this.icdoBenefitApplication.retirement_date.Year - 1 <= aclbPersonWorkHistory_MPI.Last().year)
                        //{
                        if(aclbPersonWorkHistory_MPI.Where(t => t.year <= icdoBenefitApplication.disability_onset_date.Year - 1).Count() > 0)
                        {
                            if (aclbPersonWorkHistory_MPI.Where(t => t.year <= icdoBenefitApplication.disability_onset_date.Year - 1).Last().bis_years_count >= 2 || (aclbPersonWorkHistory_MPI.Where(t => t.year <= icdoBenefitApplication.disability_onset_date.Year - 1).Last().istrBisParticipantFlag.IsNotNull() && aclbPersonWorkHistory_MPI.Where(t => t.year <= icdoBenefitApplication.disability_onset_date.Year - 1).Last().istrBisParticipantFlag == busConstant.FLAG_YES))
                            {
                                lblnIsBIS = true;
                            }
                        }
                        //else if (this.icdoBenefitApplication.retirement_date.Year - 1 > aclbPersonWorkHistory_MPI.Last().year)
                        //{
                        //    if (aclbPersonWorkHistory_MPI.Where(t => t.year <= icdoBenefitApplication.disability_onset_date.Year - 1).Last().bis_years_count >= 1 || (aclbPersonWorkHistory_MPI.Where(t => t.year <= icdoBenefitApplication.disability_onset_date.Year - 1 ).Last().istrBisParticipantFlag.IsNotNull() && aclbPersonWorkHistory_MPI.Where(t => t.year <= icdoBenefitApplication.disability_onset_date.Year - 1).Last().istrBisParticipantFlag == busConstant.FLAG_YES))
                        //    {
                        //        lblnIsBIS = true;
                        //    }
                        //}
                        // PROD PIR 170 
                        int lintlastPlusOne = aclbPersonWorkHistory_MPI.Last().year + 1;
                        int lintQualifiedYearCount = aclbPersonWorkHistory_MPI.Last().qualified_years_count; //Prod PIR 170 : This fix is made afterwards due to issues in eligiblity

                        if (!lblnIsBIS && lintlastPlusOne <= this.icdoBenefitApplication.retirement_date.Year)
                        {
                            int lintBIS = 0;
                            cdoDummyWorkData lcdoDummyWorkData = null;
                            for (int i = lintlastPlusOne; i <= this.icdoBenefitApplication.retirement_date.Year; i++)
                            {
                                lintBIS = lintBIS + 1;
                                lcdoDummyWorkData = new cdoDummyWorkData();
                                lcdoDummyWorkData.year = i;

                                lcdoDummyWorkData.qualified_years_count = lintQualifiedYearCount; //Prod PIR 170 : This fix is made afterwards due to issues in eligiblity

                                if (i != this.icdoBenefitApplication.retirement_date.Year)
                                {
                                    lcdoDummyWorkData.bis_years_count = lintBIS;
                                }
                                aclbPersonWorkHistory_MPI.Add(lcdoDummyWorkData);
                            }
                        }
                    //}

                    //else
                    //{
                    //    lblnIsBIS = true;
                    //}


                    //int Year = 0;
                    //if (this.icdoBenefitApplication.retirement_date.Year >= DateTime.Now.Year)
                    //    Year = lclbPersonWorkHistoryBeforeRetirement.Last().year;
                    //else if (this.icdoBenefitApplication.retirement_date.Year <= DateTime.Now.Year)
                    //{
                    //    if (lclbPersonWorkHistoryBeforeRetirement.Where(o => o.year == this.icdoBenefitApplication.retirement_date.Year).Count() > 0)
                    //        Year = this.icdoBenefitApplication.retirement_date.Year;
                    //    else
                    //        Year = lclbPersonWorkHistoryBeforeRetirement.Last().year;
                    //}

                    //UAT PIR 294,293 
                    //var BIScounter = from item in lclbPersonWorkHistoryBeforeRetirement where item.year == Year select item.bis_years_count;
                    //var BIScounter = from item in lclbPersonWorkHistoryBeforeRetirement where item.year == this.icdoBenefitApplication.retirement_date.Year select item.bis_years_count;

                    //if ((!BIScounter.IsNullOrEmpty()) && BIScounter.First() < 2)
                    //{

                    if (!lblnIsBIS)
                    {
                        int lintTotalQualifiedYears = lclbPersonWorkHistoryBeforeRetirement.Last().qualified_years_count;
                        decimal ldecTotalCreditedHours = 0;

                       //// for PIR-793(Commenting the code -- RASHMI)
                        //var LatestYearofForfieture = from item in lclbPersonWorkHistoryBeforeRetirement where item.qualified_years_count == 0 orderby item.year descending select item.year;

                        //if (!LatestYearofForfieture.IsNullOrEmpty())
                        //    ldecTotalCreditedHours = (from item in lclbPersonWorkHistoryBeforeRetirement where item.year > (Int32)LatestYearofForfieture.First() select item.qualified_hours).Sum();
                        //else
                            ldecTotalCreditedHours = (from item in lclbPersonWorkHistoryBeforeRetirement select item.qualified_hours).Sum();

                        #region Add Credits Hours of Other Local Plans (Code added in Sprint 2.0 as a part of changes in Vesting and Eligibility)
                        foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                        {
                            if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP)
                            {
                                switch (lbusPersonAccount.icdoPersonAccount.istrPlanCode)
                                {
                                    case busConstant.Local_52:
                                        if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id))
                                            ldecTotalCreditedHours += (from item in lclbPersonWorkHistoryBeforeRetirement select item.L52_Hours).Sum();
                                        break;
                                    case busConstant.Local_161:
                                        if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id))
                                            ldecTotalCreditedHours += (from item in lclbPersonWorkHistoryBeforeRetirement select item.L161_Hours).Sum();
                                        break;
                                    case busConstant.Local_600:
                                        if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id))
                                            ldecTotalCreditedHours += (from item in lclbPersonWorkHistoryBeforeRetirement select item.L600_Hours).Sum();
                                        break;
                                    case busConstant.Local_666:
                                        if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id))
                                            ldecTotalCreditedHours += (from item in lclbPersonWorkHistoryBeforeRetirement select item.L666_Hours).Sum();
                                        break;
                                    case busConstant.LOCAL_700:
                                        if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id))
                                            ldecTotalCreditedHours += (from item in lclbPersonWorkHistoryBeforeRetirement select item.L700_Hours).Sum();
                                        break;
                                }
                            }
                        }
                        #endregion

                        if (lintTotalQualifiedYears >= lcdoBenefitProvisionEligibility.qualified_years && ldecTotalCreditedHours >= lcdoBenefitProvisionEligibility.credited_hours)
                        {
                            EligibleforMPIPP = true;
                            //Eligible_Plans.Add(busConstant.MPIPP);
                        }
                    }

                    //}
                }

                if (EligibleforMPIPP)
                    this.ibusPerson.iclbPersonAccount.ForEach(item =>
                    {
                        Eligible_Plans.Add(item.icdoPersonAccount.istrPlanCode);
                    }
                                                              );

                else if (this.ibusPerson != null)
                {
                    if (this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                    {
                        DataTable lTempTable = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalancebyPersonAccount", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id });
                        //PIR 550
                        if (lTempTable.IsNotNull() && lTempTable.Rows.Count > 0 && Convert.ToString(lTempTable.Rows[0]["IAP_BALANCE"]).IsNotNullOrEmpty() && Convert.ToDecimal(lTempTable.Rows[0]["IAP_BALANCE"]) > 0)
                        {
                            if (Eligible_Plans.IsNull())
                                Eligible_Plans = new Collection<string>();
                            Eligible_Plans.Add(busConstant.IAP);
                        }
                    }
                }

            }
        }

        public bool CheckAlreadyVested(string astrPlanCode)
        {
            this.ibusTempPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
            int lintAccountId = 0;
            if (this.ibusPerson.iclbPersonAccount != null)
            {
                if (this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == astrPlanCode).Count() > 0)
                {
                    lintAccountId = this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == astrPlanCode).First().icdoPersonAccount.person_account_id;
                }
            }

            if (lintAccountId > 0)
            {
                DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });
                if (ldtbPersonAccountEligibility.Rows.Count > 0)
                {
                    this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                    if (this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.IsNotNull() && this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                    {
                        return true;
                    }
                }

            }
            return false;
        }


        //Code-Abhishek

        public void CheckEligibility_DeathPreRetirement()
        {

            //Eligibility needs to be checked for each plan   
            this.iblnEligbile4L161BenefitPreDeath = false;
            this.iblnEligbile4L600BenefitPreDeath = false;
            this.iblnEligbile4L666BenefitPreDeath = false;
            this.iblnEligbile4L700BenefitPreDeath = false;
            this.iblnEligbile4MPIBenefitPreDeath = false;
            this.iblnEligible4IAPBenefitPreDeath = false;
            this.iblnEligible4L52BenefitPreDeath = false;


            if (!Eligible_Plans.IsNullOrEmpty())
            {
                Eligible_Plans.Clear();
            }
            //DataTable ldtbEligibility = busBase.Select("cdoBenefitProvisionEligibility.FetchAllEligibilityRules", new object[0] { });
            //Collection<cdoBenefitProvisionEligibility> aclbAllEligibilityRules = new Collection<cdoBenefitProvisionEligibility>(); // Collection that will store the Eligibility Rule                        
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                //aclbAllEligibilityRules = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(ldtbEligibility);
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();
                foreach (busPersonAccount account in this.ibusPerson.iclbPersonAccount)
                {
                    busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                    DataTable ldtbPersonAccountEligibility = new DataTable();
                    DateTime ldtVestingDate = new DateTime();
                    DateTime ldtDateatAge65 = new DateTime();
                    DateTime ldtCompareBaseDate = new DateTime();
                    DateTime ldtNormalRetirementDate = new DateTime();
                    DateTime ldtLateRetirementDate = new DateTime();

                    switch (account.icdoPersonAccount.istrPlanCode)
                    {
                        case busConstant.MPIPP:
                            if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                            {
                                if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES && this.QualifiedSpouseExists) //In this case we are going to Assume that he is Vested in IAP TOO
                                {
                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    ldtDateatAge65 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;

                                    #region Algorithm for Late OR Normal
                                    if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id });
                                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                        {
                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                        }

                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                        if (ldtVestingDate > ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }

                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);
                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.MPIPP);

                                            //IMPNOTE Person has to retire for IAP as well compulsively so we need to PUSH the same to IAP PLAN AS WELL. IAP's OWN RULE COMES INTO PICTURE ONLY when HE IS ONLY RETIRING IN IAP
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.IAP);
                                            this.iblnEligbile4MPIBenefitPreDeath = true;
                                            this.iblnEligible4IAPBenefitPreDeath = true;
                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.MPIPP);

                                            //IMPNOTE Person has to retire for IAP as well compulsively so we need to PUSH the same to IAP PLAN AS WELL. IAP's OWN RULE COMES INTO PICTURE ONLY when HE IS ONLY RETIRING IN IAP
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.IAP);
                                            this.iblnEligbile4MPIBenefitPreDeath = true;
                                            this.iblnEligible4IAPBenefitPreDeath = true;
                                            break;
                                        }

                                    }
                                    #endregion

                                    #region Check For Early Un-Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    int lintTotalQualifiedYears = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                                    decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum();

                                    #region Add Credits Hours of Other Local Plans
                                    foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                                    {
                                        if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP)
                                        {
                                            switch (lbusPersonAccount.icdoPersonAccount.istrPlanCode)
                                            {
                                                case busConstant.Local_52:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();
                                                    break;
                                                case busConstant.Local_161:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();
                                                    break;
                                                case busConstant.Local_600:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum();
                                                    break;
                                                case busConstant.Local_666:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum();
                                                    break;
                                                case busConstant.LOCAL_700:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum();
                                                    break;
                                            }
                                        }
                                    }
                                    #endregion

                                    int flag = 0;

                                    if (lintTotalQualifiedYears >= 30)
                                    {
                                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                        {
                                            if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && ldecTotalCreditedHours >= rule.credited_hours)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.MPIPP);

                                                //IMPNOTE Person has to retire for IAP as well compulsively so we need to PUSH the same to IAP PLAN AS WELL. IAP's OWN RULE COMES INTO PICTURE ONLY when HE IS ONLY RETIRING IN IAP
                                                this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.IAP);
                                                this.iblnEligbile4MPIBenefitPreDeath = true;
                                                this.iblnEligible4IAPBenefitPreDeath = true;
                                                flag = 1;
                                                break;
                                            }
                                        }

                                        if (flag == 1)
                                            break;
                                    }
                                    #endregion

                                    #region Check for Special Reduced Early
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();

                                    var LatestYearofForfieture = from item in this.aclbPersonWorkHistory_MPI where item.qualified_years_count == 0 orderby item.year descending select item.year;

                                    if (!LatestYearofForfieture.IsNullOrEmpty())
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI where item.year > (Int32)LatestYearofForfieture.First() select item.qualified_hours).Sum();
                                    else
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum();

                                    #region Add Credits Hours of Other Local Plans
                                    foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                                    {
                                        if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP)
                                        {
                                            switch (lbusPersonAccount.icdoPersonAccount.istrPlanCode)
                                            {
                                                case busConstant.Local_52:
                                                    if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id))
                                                        ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();
                                                    break;
                                                case busConstant.Local_161:
                                                    if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id))
                                                        ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();
                                                    break;
                                                case busConstant.Local_600:
                                                    if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id))
                                                        ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum();
                                                    break;
                                                case busConstant.Local_666:
                                                    if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id))
                                                        ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum();
                                                    break;
                                                case busConstant.LOCAL_700:
                                                    if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id))
                                                        ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum();
                                                    break;
                                            }
                                        }
                                    }
                                    #endregion

                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.MPIPP);

                                            //IMPNOTE Person has to retire for IAP as well compulsively so we need to PUSH the same to IAP PLAN AS WELL. IAP's OWN RULE COMES INTO PICTURE ONLY when HE IS ONLY RETIRING IN IAP
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.IAP);
                                            iblnEligbile4MPIBenefitPreDeath = true;
                                            iblnEligible4IAPBenefitPreDeath = true;
                                            flag = 1;
                                            break;
                                        }

                                    }

                                    if (flag == 1)
                                        break;

                                    #endregion

                                    #region Check for Early Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.MPIPP);

                                            //IMPNOTE Person has to retire for IAP as well compulsively so we need to PUSH the same to IAP PLAN AS WELL. IAP's OWN RULE COMES INTO PICTURE ONLY when HE IS ONLY RETIRING IN IAP
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.IAP);

                                            flag = 1;
                                            break;
                                        }

                                    }
                                    #endregion

                                    if (Eligible_Plans.Contains(busConstant.MPIPP))
                                    {
                                        this.iblnEligible4IAPBenefitPreDeath = true;
                                        this.iblnEligbile4MPIBenefitPreDeath = true;
                                    }
                                    else
                                    {
                                        Eligible_Plans.Add(busConstant.MPIPP);
                                        this.iblnEligbile4MPIBenefitPreDeath = false;
                                    }
                                }
                                else
                                {
                                    this.iblnEligbile4MPIBenefitPreDeath = false;
                                    Eligible_Plans.Add(busConstant.MPIPP);
                                    if (Eligible_Plans.Where(plan => plan == busConstant.IAP).Count() == 0)
                                    {
                                        Eligible_Plans.Add(busConstant.IAP);
                                    }
                                }
                            }
                            break;

                        case busConstant.IAP:
                            if (this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES)
                            {
                                if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                {
                                    this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                    this.iblnEligible4IAPBenefitPreDeath = true;
                                }
                            }
                            if (!this.aclbPersonWorkHistory_IAP.IsNullOrEmpty())
                            {
                                if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES && Eligible_Plans.Where(plan => plan == busConstant.IAP).Count() == 0)
                                    Eligible_Plans.Add(busConstant.IAP);

                            }
                            break;

                        case busConstant.Local_52:
                            if (!IsVestingDateNull(busConstant.Local_52))// && this.QualifiedSpouseExists) //IMP-CHANGE - WE would need to make changes around this QUALIFIED SPOUSE THING
                            {
                                if (this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue && this.ibusPerson.icdoPerson.date_of_death > this.ibusPerson.icdoPerson.idtDateofBirth)
                                {
                                    idecAgeAtDeath = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.ibusPerson.icdoPerson.date_of_death);
                                    idecAgeAtL52Merger = idecAgeAtDeath;
                                    if (this.ibusPerson.icdoPerson.date_of_death >= Convert.ToDateTime(busConstant.MERGER_DATE_STRING))
                                    {
                                        idecAgeAtL52Merger = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, Convert.ToDateTime(busConstant.MERGER_DATE_STRING));
                                    }
                                }

                                if (this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue && this.ibusPerson.icdoPerson.date_of_death < Convert.ToDateTime(busConstant.MERGER_DATE_STRING))
                                {
                                    #region Pre_merger Death
                                    if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                                    {
                                        ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                        ldtDateatAge65 = DateTime.MinValue;
                                        ldtNormalRetirementDate = DateTime.MinValue;
                                        ldtLateRetirementDate = DateTime.MinValue;

                                        #region Algorithm for Late and Normal
                                        if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                        {
                                            ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id });
                                            if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                            {
                                                lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                            }

                                            ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                            ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                            if (ldtVestingDate > ldtDateatAge65)
                                            {
                                                ldtCompareBaseDate = ldtVestingDate;
                                            }
                                            else if (ldtVestingDate < ldtDateatAge65)
                                            {
                                                ldtCompareBaseDate = ldtDateatAge65;
                                            }
                                            else if (ldtVestingDate == ldtDateatAge65)
                                            {
                                                ldtCompareBaseDate = ldtDateatAge65;
                                            }

                                            if (ldtCompareBaseDate.Day == 1)
                                                ldtNormalRetirementDate = ldtCompareBaseDate;
                                            else
                                                ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                            ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                            if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                                Eligible_Plans.Add(busConstant.Local_52);
                                                break;
                                            }
                                            else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                                Eligible_Plans.Add(busConstant.Local_52);
                                                break;
                                            }

                                        }
                                        #endregion

                                        #region Check for Early Un-Reduced
                                        iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_52 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).ToList().ToCollection();
                                        int lintTotalQualifiedYears = 0;
                                        int lintTotalQualifiedYears_MPIPP = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                                        int lintTotalQualifiedYears_Local52 = this.aclbPersonWorkHistory_MPI.Where(item => item.L52_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                                        if (lintTotalQualifiedYears_MPIPP > lintTotalQualifiedYears_Local52)
                                            lintTotalQualifiedYears = lintTotalQualifiedYears_MPIPP;
                                        else
                                            lintTotalQualifiedYears = lintTotalQualifiedYears_Local52;
                                        decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();

                                        int flag_52 = 0;

                                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                        {
                                            if (this.idecAge >= rule.min_age && this.idecAge < rule.max_age)
                                            {
                                                if ((rule.special_years.IsNull() || rule.special_years <= 0) && lintTotalQualifiedYears >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)//PIR 905
                                                {
                                                    this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                    Eligible_Plans.Add(busConstant.Local_52);
                                                    flag_52 = 1;
                                                    break;
                                                }
                                                //lintTotalQualifiedYearsfromMPI + PREMERGER PENSION CREDITS FOR PLAN >= rule.special_years
                                                else if (this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count() > 0)
                                                {
                                                    if ((this.aclbPersonWorkHistory_MPI.Last().qualified_years_count - this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).FirstOrDefault().qualified_years_count) + Local52_PensionCredits >= rule.special_years) //TODO - VERY IMP this condition should not be mapped like this it should be total MPI qualified Years + PENSION CREDITS
                                                    {
                                                        this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                        Eligible_Plans.Add(busConstant.Local_52);
                                                        flag_52 = 1;
                                                        break;

                                                    }
                                                }

                                            }
                                        }

                                        if (flag_52 == 1)
                                            break;

                                        #endregion

                                        #region Check for Early Reduced
                                        iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_52 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                        {
                                            if (this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count() > 0)
                                            {
                                                if (this.idecAge >= rule.min_age && this.idecAge < rule.max_age && (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count - this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).FirstOrDefault().qualified_years_count) + Local52_PensionCredits >= rule.special_years)
                                                {
                                                    this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                                    Eligible_Plans.Add(busConstant.Local_52);
                                                    flag_52 = 1;
                                                    break;
                                                }
                                            }

                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Post Merger Death
                                    istrLocal52RuleForDeathCalculation = string.Empty;
                                    DataTable ldtbL52BenOptions = busBase.Select("cdoPlanBenefitXr.GetAllBenOptions4L52", new object[0] { });
                                    if (ldtbL52BenOptions.Rows.Count > 0)
                                    {
                                        this.lclcL52BenOptionsPreDeath = cdoPlanBenefitXr.GetCollection<cdoPlanBenefitXr>(ldtbL52BenOptions);

                                        if (this.Local52_PensionCredits < Convert.ToDecimal(15))
                                        {
                                            #region Pension Credits Less Than 15
                                            if (this.Local52_PensionCredits < Convert.ToDecimal(12))
                                            {
                                                if (this.QualifiedSpouseExists)
                                                {
                                                    //Rule # 1
                                                    CheckEligibilityTypeForLocal52_Death();
                                                    if (iblnEligible4L52BenefitPreDeath)
                                                    {
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.LUMP_SUM || benoption.benefit_option_value == busConstant.LIFE_ANNUTIY).ToList().ToCollection();
                                                    }
                                                    else
                                                    {
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.LUMP_SUM).ToList().ToCollection();
                                                    }
                                                    istrLocal52RuleForDeathCalculation = busConstant.L52_RULE_1;
                                                    Eligible_Plans.Add(busConstant.Local_52);

                                                }
                                            }
                                            else if (this.Local52_PensionCredits >= Convert.ToDecimal(12) && this.Local52_RetirementCredits < Convert.ToDecimal(15))
                                            {
                                                if (this.QualifiedSpouseExists && idecAgeAtDeath >= Convert.ToDecimal(55))
                                                {
                                                    CheckEligibilityTypeForLocal52_Death();
                                                    if (iblnEligible4L52BenefitPreDeath)
                                                    {
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN || benoption.benefit_option_value == busConstant.LIFE_ANNUTIY).ToList().ToCollection();
                                                    }
                                                    else
                                                    {
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN).ToList().ToCollection();
                                                    }
                                                    istrLocal52RuleForDeathCalculation = busConstant.L52_RULE_6;
                                                    Eligible_Plans.Add(busConstant.Local_52);

                                                }
                                                else if (this.QualifiedSpouseExists)
                                                {
                                                    CheckEligibilityTypeForLocal52_Death();
                                                    if (iblnEligible4L52BenefitPreDeath)
                                                    {
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN || benoption.benefit_option_value == busConstant.LIFE_ANNUTIY).ToList().ToCollection();
                                                    }
                                                    else
                                                    {
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN).ToList().ToCollection();
                                                    }
                                                    istrLocal52RuleForDeathCalculation = busConstant.L52_RULE_3;
                                                    Eligible_Plans.Add(busConstant.Local_52);

                                                }
                                            }
                                            else if (this.Local52_PensionCredits >= Convert.ToDecimal(12) && this.Local52_RetirementCredits > Convert.ToDecimal(15))
                                            {
                                                if (this.QualifiedSpouseExists)
                                                {
                                                    if (idecAgeAtL52Merger >= Convert.ToDecimal(55) && this.Local52_PensionCredits >= Convert.ToDecimal(20))
                                                    {
                                                        CheckEligibilityTypeForLocal52_Death();
                                                        if (iblnEligible4L52BenefitPreDeath)
                                                        {
                                                            this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN || benoption.benefit_option_value == busConstant.LIFE_ANNUTIY).ToList().ToCollection();
                                                        }
                                                        else
                                                        {
                                                            this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN).ToList().ToCollection();
                                                        }
                                                        istrLocal52RuleForDeathCalculation = busConstant.L52_RULE_2;
                                                        Eligible_Plans.Add(busConstant.Local_52);
                                                    }
                                                    else if (idecAgeAtDeath != 0)
                                                    {
                                                        CheckEligibilityTypeForLocal52_Death();
                                                        if (iblnEligible4L52BenefitPreDeath)
                                                        {
                                                            this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN || benoption.benefit_option_value == busConstant.LIFE_ANNUTIY).ToList().ToCollection();
                                                        }
                                                        else
                                                        {
                                                            this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN).ToList().ToCollection();
                                                        }
                                                        istrLocal52RuleForDeathCalculation = busConstant.L52_RULE_3;
                                                        Eligible_Plans.Add(busConstant.Local_52);
                                                    }
                                                }
                                                else
                                                {
                                                    if (idecAgeAtDeath >= Convert.ToDecimal(55))
                                                    {
                                                        istrLocal52RuleForDeathCalculation = busConstant.L52_RULE_4;
                                                        Eligible_Plans.Add(busConstant.Local_52);
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN).ToList().ToCollection();

                                                    }
                                                    else if (idecAgeAtDeath != 0)
                                                    {
                                                        istrLocal52RuleForDeathCalculation = busConstant.L52_RULE_5;
                                                        Eligible_Plans.Add(busConstant.Local_52);
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN).ToList().ToCollection();
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            #region Pension Credits Greater Than 15
                                            if (this.QualifiedSpouseExists)
                                            {
                                                if (idecAgeAtL52Merger >= Convert.ToDecimal(55) && this.Local52_PensionCredits >= Convert.ToDecimal(20))
                                                {
                                                    CheckEligibilityTypeForLocal52_Death();
                                                    if (this.iblnEligible4L52BenefitPreDeath)
                                                    {
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN || benoption.benefit_option_value == busConstant.LIFE_ANNUTIY).ToList().ToCollection();
                                                    }
                                                    else
                                                    {
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN).ToList().ToCollection();
                                                    }
                                                    istrLocal52RuleForDeathCalculation = busConstant.L52_RULE_2;
                                                    Eligible_Plans.Add(busConstant.Local_52);
                                                }

                                                else if (idecAgeAtDeath != 0)
                                                {
                                                    CheckEligibilityTypeForLocal52_Death();
                                                    if (this.iblnEligible4L52BenefitPreDeath)
                                                    {
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN || benoption.benefit_option_value == busConstant.LIFE_ANNUTIY).ToList().ToCollection();
                                                    }
                                                    else
                                                    {
                                                        this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN).ToList().ToCollection();
                                                    }

                                                    istrLocal52RuleForDeathCalculation = busConstant.L52_RULE_3;
                                                    Eligible_Plans.Add(busConstant.Local_52);
                                                }
                                            }
                                            else
                                            {
                                                if (idecAgeAtDeath >= Convert.ToDecimal(55))
                                                {
                                                    istrLocal52RuleForDeathCalculation = busConstant.L52_RULE_4;
                                                    Eligible_Plans.Add(busConstant.Local_52);
                                                    this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN).ToList().ToCollection();
                                                }

                                                else if (idecAgeAtDeath != 0)
                                                {
                                                    istrLocal52RuleForDeathCalculation = busConstant.L52_RULE_5;
                                                    Eligible_Plans.Add(busConstant.Local_52);
                                                    this.lclcL52BenOptionsPreDeath = this.lclcL52BenOptionsPreDeath.Where(benoption => benoption.benefit_option_value == busConstant.TEN_YEARS_TERM_CERTAIN).ToList().ToCollection();
                                                }

                                            }
                                            #endregion
                                        }
                                    }
                                    #endregion
                                }

                            }
                            break;

                        case busConstant.Local_161:
                            if (!IsVestingDateNull(busConstant.Local_161) && this.QualifiedSpouseExists) //IMP-CHANGE - WE would need to make changes around this QUALIFIED SPOUSE THING
                            {
                                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                                {
                                    #region Old Eligibility
                                    //ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    //DateTime ldtDateatAge60 = DateTime.MinValue;
                                    //ldtNormalRetirementDate = DateTime.MinValue;
                                    //ldtLateRetirementDate = DateTime.MinValue;
                                    //int flag_161 = 0;
                                    #endregion

                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    ldtDateatAge65 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;

                                    #region Algorithm for Late OR Normal
                                    if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id });
                                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                        {
                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                        }

                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                        if (ldtVestingDate > ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }

                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);
                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.Local_161);
                                            this.iblnEligbile4L161BenefitPreDeath = true;
                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.Local_161);
                                            this.iblnEligbile4L161BenefitPreDeath = true;
                                            break;
                                        }

                                    }
                                    #endregion

                                    #region Check For Early Un-Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    int lintTotalQualifiedYears = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                                    decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();

                                    int flag = 0;

                                    if (lintTotalQualifiedYears >= 30)
                                    {
                                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                        {
                                            if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && ldecTotalCreditedHours >= rule.credited_hours)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.Local_161);
                                                this.iblnEligbile4L161BenefitPreDeath = true;

                                                flag = 1;
                                                break;
                                            }
                                        }

                                        if (flag == 1)
                                            break;
                                    }
                                    #endregion

                                    #region Check for Special Reduced Early
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();

                                    var LatestYearofForfieture = from item in this.aclbPersonWorkHistory_MPI where item.qualified_years_count == 0 orderby item.year descending select item.year;

                                    if (!LatestYearofForfieture.IsNullOrEmpty())
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI where item.year > (Int32)LatestYearofForfieture.First() select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();
                                    else
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();


                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.Local_161);
                                            this.iblnEligbile4L161BenefitPreDeath = true;
                                            flag = 1;
                                            break;
                                        }

                                    }

                                    if (flag == 1)
                                        break;

                                    #endregion

                                    #region Check for Early Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.Local_161);
                                            this.iblnEligbile4L161BenefitPreDeath = true;
                                            flag = 1;
                                            break;
                                        }

                                    }
                                    #endregion

                                    if (!Eligible_Plans.Contains(busConstant.Local_161))
                                        Eligible_Plans.Add(busConstant.Local_161);

                                    #region Old Eligibility
                                    //#region Algorithm for Late and Normal
                                    //if (this.idecAge >= busConstant.LOCAL_161_RETIREMENT_NORMAL_AGE)
                                    //{
                                    //    ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id });
                                    //    if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                    //    {
                                    //        lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                    //    }

                                    //    ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                    //    ldtDateatAge60 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(60);

                                    //    if (ldtVestingDate > ldtDateatAge60)
                                    //    {
                                    //        ldtCompareBaseDate = ldtVestingDate;
                                    //    }
                                    //    else if (ldtVestingDate < ldtDateatAge60)
                                    //    {
                                    //        ldtCompareBaseDate = ldtDateatAge60;
                                    //    }
                                    //    else if (ldtVestingDate == ldtDateatAge60)
                                    //    {
                                    //        ldtCompareBaseDate = ldtDateatAge60;
                                    //    }
                                    //    if (ldtCompareBaseDate.Day == 1)
                                    //        ldtNormalRetirementDate = ldtCompareBaseDate;
                                    //    else                                        
                                    //        ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                    //    ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                    //    if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                    //    {
                                    //        this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                    //        Eligible_Plans.Add(busConstant.Local_161);
                                    //        this.iblnEligbile4L161BenefitPreDeath = true;
                                    //        break;
                                    //    }
                                    //    else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                    //    {
                                    //        this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                    //        Eligible_Plans.Add(busConstant.Local_161);
                                    //        this.iblnEligbile4L161BenefitPreDeath = true;
                                    //        break;
                                    //    }
                                    //    //else
                                    //    //{
                                    //    //    break;
                                    //    //}

                                    //}
                                    //#endregion

                                    //#region Check for Early Reduced
                                    //iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_161 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    //foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    //{
                                    //    if (this.idecAge >= rule.min_age && this.idecAge < rule.max_age)
                                    //    {
                                    //        this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                    //        Eligible_Plans.Add(busConstant.Local_161);
                                    //        this.iblnEligbile4L161BenefitPreDeath = true;
                                    //        flag_161 = 1;
                                    //        break;
                                    //    }

                                    //}
                                    //if(!Eligible_Plans.Contains(busConstant.Local_161))
                                    //    Eligible_Plans.Add(busConstant.Local_161);
                                    //#endregion
                                    #endregion
                                }
                            }
                            else if (!this.QualifiedSpouseExists)
                            {
                                Eligible_Plans.Add(busConstant.Local_161);
                            }
                            break;

                        case busConstant.Local_600:
                            if (!IsVestingDateNull(busConstant.Local_600))  //THIS ONE FUNCTON ITSELF SHOULD FILL A COMMON ICDO AS WELL. So that we can avoid 2 DB hits
                            {
                                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty() && this.QualifiedSpouseExists)
                                {
                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    ldtDateatAge65 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;

                                    #region Algorithm for Late OR Normal
                                    if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id });
                                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                        {
                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                        }

                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                        if (ldtVestingDate > ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }

                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);
                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.Local_600);
                                            this.iblnEligbile4L600BenefitPreDeath = true;
                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.Local_600);
                                            this.iblnEligbile4L600BenefitPreDeath = true;
                                            break;
                                        }

                                    }
                                    #endregion

                                    #region Check For Early Un-Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    int lintTotalQualifiedYears = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                                    decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum();

                                    int flag = 0;

                                    if (lintTotalQualifiedYears >= 30)
                                    {
                                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                        {
                                            if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && ldecTotalCreditedHours >= rule.credited_hours)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.Local_600);
                                                this.iblnEligbile4L600BenefitPreDeath = true;

                                                flag = 1;
                                                break;
                                            }
                                        }

                                        if (flag == 1)
                                            break;
                                    }
                                    #endregion

                                    #region Check for Special Reduced Early
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();

                                    var LatestYearofForfieture = from item in this.aclbPersonWorkHistory_MPI where item.qualified_years_count == 0 orderby item.year descending select item.year;

                                    if (!LatestYearofForfieture.IsNullOrEmpty())
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI where item.year > (Int32)LatestYearofForfieture.First() select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum();
                                    else
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum();


                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.Local_600);
                                            this.iblnEligbile4L600BenefitPreDeath = true;
                                            flag = 1;
                                            break;
                                        }

                                    }

                                    if (flag == 1)
                                        break;

                                    #endregion

                                    #region Check for Early Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.Local_600);
                                            this.iblnEligbile4L600BenefitPreDeath = true;
                                            flag = 1;
                                            break;
                                        }

                                    }
                                    #endregion

                                    if (!Eligible_Plans.Contains(busConstant.Local_600))
                                        Eligible_Plans.Add(busConstant.Local_600);

                                    #region OLD Eligbility
                                    //ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    //ldtDateatAge65 = DateTime.MinValue;
                                    //ldtNormalRetirementDate = DateTime.MinValue;
                                    //ldtLateRetirementDate = DateTime.MinValue;

                                    //#region Algorithm for Late and Normal
                                    //if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    //{
                                    //    ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id });
                                    //    if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                    //    {
                                    //        lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                    //    }

                                    //    ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                    //    ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                    //    if (ldtVestingDate > ldtDateatAge65)
                                    //    {
                                    //        ldtCompareBaseDate = ldtVestingDate;
                                    //    }
                                    //    else if (ldtVestingDate < ldtDateatAge65)
                                    //    {
                                    //        ldtCompareBaseDate = ldtDateatAge65;
                                    //    }
                                    //    else if (ldtVestingDate == ldtDateatAge65)
                                    //    {
                                    //        ldtCompareBaseDate = ldtDateatAge65;
                                    //    }
                                    //    if (ldtCompareBaseDate.Day == 1)
                                    //        ldtNormalRetirementDate = ldtCompareBaseDate;
                                    //    else                                        
                                    //        ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                    //    ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                    //    if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                    //    {
                                    //        this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                    //        Eligible_Plans.Add(busConstant.Local_600);
                                    //        this.iblnEligbile4L600BenefitPreDeath = true;
                                    //        break;
                                    //    }
                                    //    else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                    //    {
                                    //        this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                    //        Eligible_Plans.Add(busConstant.Local_600);
                                    //        this.iblnEligbile4L600BenefitPreDeath = true;
                                    //        break;
                                    //    }
                                    //    //else
                                    //    //{
                                    //    //    break;
                                    //    //}

                                    //}
                                    //#endregion

                                    //#region Check for Early Un-Reduced
                                    //iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_600 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).ToList().ToCollection();
                                    //int lintTotalQualifiedYears = 0;
                                    //int lintTotalQualifiedYears_Local600 = this.aclbPersonWorkHistory_Local600.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                                    //int lintTotalQualifiedYears_MPIPP = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();

                                    //if (lintTotalQualifiedYears_MPIPP > lintTotalQualifiedYears_Local600)
                                    //    lintTotalQualifiedYears = lintTotalQualifiedYears_MPIPP;
                                    //else
                                    //    lintTotalQualifiedYears = lintTotalQualifiedYears_Local600;

                                    //decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_Local600 select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum();

                                    //int flag_600 = 0;

                                    //foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    //{
                                    //    if (this.idecAge >= rule.min_age && this.idecAge < rule.max_age)
                                    //    {
                                    //        if ((rule.special_years.IsNull() || rule.special_years <= 0) && lintTotalQualifiedYears >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                                    //        {
                                    //            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                    //            Eligible_Plans.Add(busConstant.Local_600);
                                    //            this.iblnEligbile4L600BenefitPreDeath = true;
                                    //            flag_600 = 1;
                                    //            break;
                                    //        }
                                    //        else if (this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count() > 0)
                                    //        {
                                    //            if ((this.aclbPersonWorkHistory_MPI.Last().qualified_years_count - this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).FirstOrDefault().qualified_years_count) + Local600_PensionCredits >= rule.special_years) //TODO - VERY IMP this condition should not be mapped like this it should be total MPI qualified Years + PENSION CREDITS
                                    //            {
                                    //                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                    //                Eligible_Plans.Add(busConstant.Local_600);
                                    //                this.iblnEligbile4L600BenefitPreDeath = true;
                                    //                flag_600 = 1;
                                    //                break;

                                    //            }
                                    //        }

                                    //    }
                                    //}

                                    //if (flag_600 == 1)
                                    //    break;

                                    //#endregion

                                    //#region Check for Early Reduced
                                    //iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_600 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    //foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    //{
                                    //    if (this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count() > 0)
                                    //    {
                                    //        if (this.idecAge >= rule.min_age && this.idecAge < rule.max_age && (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count - this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).FirstOrDefault().qualified_years_count) + Local600_PensionCredits >= rule.special_years)
                                    //        {
                                    //            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                    //            Eligible_Plans.Add(busConstant.Local_600);
                                    //            this.iblnEligbile4L600BenefitPreDeath = true;
                                    //            flag_600 = 1;
                                    //            break;
                                    //        }
                                    //    }

                                    //}
                                    //if (!Eligible_Plans.Contains(busConstant.Local_600))
                                    //    Eligible_Plans.Add(busConstant.Local_600);
                                    //#endregion
                                    #endregion
                                }
                            }
                            break;

                        case busConstant.Local_666:
                            if (!IsVestingDateNull(busConstant.Local_666) && this.QualifiedSpouseExists)
                            {
                                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                                {
                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    ldtDateatAge65 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;

                                    #region Algorithm for Late OR Normal
                                    if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id });
                                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                        {
                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                        }

                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                        if (ldtVestingDate > ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }

                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);
                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.Local_666);
                                            this.iblnEligbile4L666BenefitPreDeath = true;
                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.Local_666);
                                            this.iblnEligbile4L666BenefitPreDeath = true;
                                            break;
                                        }

                                    }
                                    #endregion

                                    #region Check For Early Un-Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    int lintTotalQualifiedYears = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                                    decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum();

                                    int flag = 0;

                                    if (lintTotalQualifiedYears >= 30)
                                    {
                                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                        {
                                            if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && ldecTotalCreditedHours >= rule.credited_hours)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.Local_666);
                                                this.iblnEligbile4L666BenefitPreDeath = true;

                                                flag = 1;
                                                break;
                                            }
                                        }

                                        if (flag == 1)
                                            break;
                                    }
                                    #endregion

                                    #region Check for Special Reduced Early
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();

                                    var LatestYearofForfieture = from item in this.aclbPersonWorkHistory_MPI where item.qualified_years_count == 0 orderby item.year descending select item.year;

                                    if (!LatestYearofForfieture.IsNullOrEmpty())
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI where item.year > (Int32)LatestYearofForfieture.First() select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum();
                                    else
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum();


                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.Local_666);
                                            this.iblnEligbile4L666BenefitPreDeath = true;
                                            flag = 1;
                                            break;
                                        }

                                    }

                                    if (flag == 1)
                                        break;

                                    #endregion

                                    #region Check for Early Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.Local_666);
                                            this.iblnEligbile4L666BenefitPreDeath = true;
                                            flag = 1;
                                            break;
                                        }

                                    }
                                    #endregion

                                    if (!Eligible_Plans.Contains(busConstant.Local_666))
                                        Eligible_Plans.Add(busConstant.Local_666);


                                    #region OLD Eligbility
                                    //ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    //ldtDateatAge65 = DateTime.MinValue;
                                    //ldtNormalRetirementDate = DateTime.MinValue;
                                    //ldtLateRetirementDate = DateTime.MinValue;

                                    //#region Algorithm for Late and Normal
                                    //if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    //{
                                    //    ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id });
                                    //    if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                    //    {
                                    //        lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                    //    }

                                    //    ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                    //    ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                    //    if (ldtVestingDate > ldtDateatAge65)
                                    //    {
                                    //        ldtCompareBaseDate = ldtVestingDate;
                                    //    }
                                    //    else if (ldtVestingDate < ldtDateatAge65)
                                    //    {
                                    //        ldtCompareBaseDate = ldtDateatAge65;
                                    //    }
                                    //    else if (ldtVestingDate == ldtDateatAge65)
                                    //    {
                                    //        ldtCompareBaseDate = ldtDateatAge65;
                                    //    }
                                    //    if (ldtCompareBaseDate.Day == 1)
                                    //        ldtNormalRetirementDate = ldtCompareBaseDate;
                                    //    else                                        
                                    //        ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                    //    ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                    //    if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                    //    {
                                    //        this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                    //        Eligible_Plans.Add(busConstant.Local_666);
                                    //        this.iblnEligbile4L666BenefitPreDeath = true;
                                    //        break;
                                    //    }
                                    //    else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                    //    {
                                    //        this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                    //        Eligible_Plans.Add(busConstant.Local_666);
                                    //        this.iblnEligbile4L666BenefitPreDeath = true;
                                    //        break;
                                    //    }
                                    //    //else
                                    //    //{
                                    //    //    break;
                                    //    //}

                                    //}
                                    //#endregion

                                    //#region Check for Early Reduced

                                    //int flag_666 = 0;
                                    //iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_666 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    //foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    //{

                                    //    if (this.idecAge >= rule.min_age && this.idecAge < rule.max_age && Local666_PensionCredits >= rule.pension_credits)
                                    //    {
                                    //        this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                    //        Eligible_Plans.Add(busConstant.Local_666);
                                    //        this.iblnEligbile4L666BenefitPreDeath = true;
                                    //        flag_666 = 1;
                                    //        break;
                                    //    }

                                    //}
                                    //if (!Eligible_Plans.Contains(busConstant.Local_666))
                                    //    Eligible_Plans.Add(busConstant.Local_666);
                                    //#endregion
                                    #endregion
                                }
                            }
                            break;

                        case busConstant.LOCAL_700:
                            if (!IsVestingDateNull(busConstant.LOCAL_700) && this.QualifiedSpouseExists) //IMP-CHANGE - WE would need to make changes around this QUALIFIED SPOUSE THING
                            {
                                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                                {

                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    ldtDateatAge65 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;

                                    #region Algorithm for Late OR Normal
                                    if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id });
                                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                        {
                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                        }

                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                        if (ldtVestingDate > ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }

                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);
                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.LOCAL_700);
                                            this.iblnEligbile4L700BenefitPreDeath = true;
                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.LOCAL_700);
                                            this.iblnEligbile4L700BenefitPreDeath = true;
                                            break;
                                        }

                                    }
                                    #endregion

                                    #region Check For Early Un-Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    int lintTotalQualifiedYears = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                                    decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum();

                                    int flag = 0;

                                    if (lintTotalQualifiedYears >= 30)
                                    {
                                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                        {
                                            if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && ldecTotalCreditedHours >= rule.credited_hours)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.LOCAL_700);
                                                this.iblnEligbile4L700BenefitPreDeath = true;

                                                flag = 1;
                                                break;
                                            }
                                        }

                                        if (flag == 1)
                                            break;
                                    }
                                    #endregion

                                    #region Check for Special Reduced Early
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();

                                    var LatestYearofForfieture = from item in this.aclbPersonWorkHistory_MPI where item.qualified_years_count == 0 orderby item.year descending select item.year;

                                    if (!LatestYearofForfieture.IsNullOrEmpty())
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI where item.year > (Int32)LatestYearofForfieture.First() select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum();
                                    else
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum();


                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.LOCAL_700);
                                            this.iblnEligbile4L700BenefitPreDeath = true;
                                            flag = 1;
                                            break;
                                        }

                                    }

                                    if (flag == 1)
                                        break;

                                    #endregion

                                    #region Check for Early Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.LOCAL_700);
                                            this.iblnEligbile4L700BenefitPreDeath = true;
                                            flag = 1;
                                            break;
                                        }

                                    }
                                    #endregion

                                    if (!Eligible_Plans.Contains(busConstant.LOCAL_700))
                                        Eligible_Plans.Add(busConstant.LOCAL_700);

                                    #region OLD Eligibility
                                    //ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    //DateTime ldtDateatAge62 = DateTime.MinValue;
                                    //ldtNormalRetirementDate = DateTime.MinValue;
                                    //ldtLateRetirementDate = DateTime.MinValue;
                                    //int flag_700 = 0;
                                    //#region Algorithm for Late and Normal
                                    //if (this.idecAge >= busConstant.LOCAL_700_RETIREMENT_NORMAL_AGE)
                                    //{
                                    //    ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id });
                                    //    if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                    //    {
                                    //        lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                    //    }

                                    //    ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                    //    ldtDateatAge62 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(62);

                                    //    if (ldtVestingDate > ldtDateatAge62)
                                    //    {
                                    //        ldtCompareBaseDate = ldtVestingDate;
                                    //    }
                                    //    else if (ldtVestingDate < ldtDateatAge62)
                                    //    {
                                    //        ldtCompareBaseDate = ldtDateatAge62;
                                    //    }
                                    //    else if (ldtVestingDate == ldtDateatAge62)
                                    //    {
                                    //        ldtCompareBaseDate = ldtDateatAge62;
                                    //    }
                                    //    if (ldtCompareBaseDate.Day == 1)
                                    //        ldtNormalRetirementDate = ldtCompareBaseDate;
                                    //    else
                                    //        ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                    //    ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                    //    if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                    //    {
                                    //        this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                    //        Eligible_Plans.Add(busConstant.LOCAL_700);
                                    //        this.iblnEligbile4L700BenefitPreDeath = true;
                                    //        break;
                                    //    }
                                    //    else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                    //    {
                                    //        this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                    //        Eligible_Plans.Add(busConstant.LOCAL_700);
                                    //        this.iblnEligbile4L700BenefitPreDeath = true;
                                    //        break;
                                    //    }
                                    //    //else
                                    //    //{
                                    //    //    break;
                                    //    //}

                                    //}
                                    //#endregion

                                    //#region Check for Early Un-Reduced
                                    //iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.LOCAL_700 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    //int lintTotalQualifiedYears = 0;
                                    //int lintTotalQualifiedYears_Local700 = this.aclbPersonWorkHistory_Local700.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                                    //int lintTotalQualifiedYears_MPIPP = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();

                                    //if (lintTotalQualifiedYears_MPIPP > lintTotalQualifiedYears_Local700)
                                    //    lintTotalQualifiedYears = lintTotalQualifiedYears_MPIPP;
                                    //else
                                    //    lintTotalQualifiedYears = lintTotalQualifiedYears_Local700;

                                    //decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_Local700 select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum();


                                    //if (lintTotalQualifiedYears >= 30)
                                    //{
                                    //    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    //    {
                                    //        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && ldecTotalCreditedHours >= rule.credited_hours)
                                    //        {
                                    //            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                    //            Eligible_Plans.Add(busConstant.LOCAL_700);
                                    //            this.iblnEligbile4L700BenefitPreDeath = true;
                                    //            flag_700 = 1;
                                    //            break;
                                    //        }
                                    //    }

                                    //    if (flag_700 == 1)
                                    //        break;
                                    //}
                                    //#endregion

                                    //#region Check for Early Reduced
                                    //iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.LOCAL_700 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    //foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    //{
                                    //    if (this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count() > 0)
                                    //    {
                                    //        if (this.idecAge >= rule.min_age && this.idecAge < rule.max_age && (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count - this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).FirstOrDefault().qualified_years_count) + Local700_PensionCredits >= rule.special_years)
                                    //        {
                                    //            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                    //            Eligible_Plans.Add(busConstant.LOCAL_700);
                                    //            this.iblnEligbile4L700BenefitPreDeath = true;
                                    //            flag_700 = 1;
                                    //            break;
                                    //        }
                                    //    }

                                    //}

                                    #endregion

                                }

                            }
                            else
                            {
                                Eligible_Plans.Add(busConstant.LOCAL_700);
                            }
                            break;
                    }
                }
            }
        }


        public void CheckEligibilityTypeForLocal52_Death()
        {
            if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
            {
                this.iblnEligible4L52BenefitPreDeath = false;

                DateTime ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                DateTime ldtDateatAge65 = DateTime.MinValue;
                DateTime ldtNormalRetirementDate = DateTime.MinValue;
                DateTime ldtLateRetirementDate = DateTime.MinValue;
                DateTime ldtCompareBaseDate = new DateTime();
                decimal ldecTotalCreditedHours;
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };

                if (!iblnEligible4L52BenefitPreDeath)
                {
                    #region Algorithm for Late OR Normal
                    if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                    {
                        DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id });
                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                        {
                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                        }

                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                        ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                        if (ldtVestingDate > ldtDateatAge65)
                        {
                            ldtCompareBaseDate = ldtVestingDate;
                        }
                        else if (ldtVestingDate < ldtDateatAge65)
                        {
                            ldtCompareBaseDate = ldtDateatAge65;
                        }
                        else if (ldtVestingDate == ldtDateatAge65)
                        {
                            ldtCompareBaseDate = ldtDateatAge65;
                        }

                        if (ldtCompareBaseDate.Day == 1)
                            ldtNormalRetirementDate = ldtCompareBaseDate;
                        else
                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);
                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                        {
                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                            iblnEligible4L52BenefitPreDeath = true;
                        }
                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                        {
                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                            iblnEligible4L52BenefitPreDeath = true;
                        }
                    }

                    #endregion
                }
                if (!iblnEligible4L52BenefitPreDeath)
                {
                    #region Check For Early Un-Reduced
                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                    int lintTotalQualifiedYears = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                    ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();


                    if (lintTotalQualifiedYears >= 30)
                    {
                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                        {
                            if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && ldecTotalCreditedHours >= rule.credited_hours)
                            {
                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                iblnEligible4L52BenefitPreDeath = true;
                                break;
                            }
                        }

                    }

                    #endregion
                }
                if (!iblnEligible4L52BenefitPreDeath)
                {
                    #region Check for Special Reduced Early
                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();

                    var LatestYearofForfieture = from item in this.aclbPersonWorkHistory_MPI where item.qualified_years_count == 0 orderby item.year descending select item.year;

                    if (!LatestYearofForfieture.IsNullOrEmpty())
                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI where item.year > (Int32)LatestYearofForfieture.First() select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();
                    else
                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();


                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                    {
                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                        {
                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY;
                            iblnEligible4L52BenefitPreDeath = true;
                            break;
                        }

                    }

                    #endregion
                }
                if (!iblnEligible4L52BenefitPreDeath)
                {
                    #region Check for Early Reduced
                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                    {
                        if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years)
                        {
                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                            iblnEligible4L52BenefitPreDeath = true;
                            break;
                        }
                    }

                    #endregion
                }



            }
        }

        public void LoadEligibiltyRules()
        {
            DataTable ldtbEligibility = busBase.Select("cdoBenefitProvisionEligibility.FetchAllEligibilityRules", new object[0] { });
            if (ldtbEligibility.Rows.Count > 0)
            {
                iclbAllEligibilityRules = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(ldtbEligibility);
            }
        }

        public DateTime GetEarliestRetirementDate_Local600()
        {
            DateTime ldtEarliestRetrAge = new DateTime();
            LoadEligibiltyRules();
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };

                iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_600 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderBy(rule => rule.min_age).ToList().ToCollection();
                foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                {
                    if (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count + Local600_PensionCredits >= rule.special_years)
                    {
                        ldtEarliestRetrAge = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(rule.min_age));
                    }
                }

            }

            return ldtEarliestRetrAge;
        }

        public DateTime GetEarliestRetirementDate_Local666()
        {
            DateTime ldtEarliestRetrAge = new DateTime();
            LoadEligibiltyRules();
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };

                iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_666 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderBy(rule => rule.min_age).ToList().ToCollection();
                foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                {
                    if (Local666_PensionCredits >= rule.pension_credits)
                    {
                        ldtEarliestRetrAge = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(rule.min_age));
                    }
                }

            }

            return ldtEarliestRetrAge;
        }

        public DateTime GetEarliestRetirementDate_Local52()
        {
            DateTime ldtEarliestRetrAge = new DateTime();
            LoadEligibiltyRules();
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };

                iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_52 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderBy(rule => rule.min_age).ToList().ToCollection();
                foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                {
                    #region Check for Early Reduced


                    if (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count + Local52_PensionCredits >= rule.special_years)
                    {
                        ldtEarliestRetrAge = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(rule.min_age));
                    }

                    #endregion
                }

            }

            return ldtEarliestRetrAge;
        }

        public DateTime GetEarliestRetirementDate_Local700()
        {
            DateTime ldtEarliestRetrAge = new DateTime();

            LoadEligibiltyRules();
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };

                iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.LOCAL_700 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderBy(rule => rule.min_age).ToList().ToCollection();
                foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                {
                    if (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count + Local700_PensionCredits >= rule.special_years)
                    {
                        ldtEarliestRetrAge = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(rule.min_age));
                    }
                }

            }

            return ldtEarliestRetrAge;
        }

        public DateTime GetEarliestRetirementDate_Local161()
        {
            DateTime ldtEarliestRetrAge = new DateTime();
            LoadEligibiltyRules();
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };

                iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_161 && rule.benefit_account_type_value ==
                    busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderBy(rule => rule.min_age).ToList().ToCollection();
                foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                {
                    ldtEarliestRetrAge = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(rule.min_age));
                }

            }

            return ldtEarliestRetrAge;
        }

        public DateTime GetEarliestRetirementDate_MPIPP()
        {
            DateTime ldtEarliestRetrAge = new DateTime();
            LoadEligibiltyRules();
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();
                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };

                iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderBy(rule => rule.min_age).ToList().ToCollection();
                foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                {
                    if (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years)
                    {
                        ldtEarliestRetrAge = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(Convert.ToInt32(rule.min_age));
                        break;
                    }
                }
            }

            return ldtEarliestRetrAge;
        }


        public int GetBestAgeforReductionFactor_Local52()
        {
            // DataTable ldtbEligibility = busBase.Select("cdoBenefitProvisionEligibility.FetchAllEligibilityRules", new object[0] { });
            //Collection<cdoBenefitProvisionEligibility> aclbAllEligibilityRules = new Collection<cdoBenefitProvisionEligibility>(); // Collection that will store the Eligibility Rule                        
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                //aclbAllEligibilityRules = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(ldtbEligibility);
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();

                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                DataTable ldtbPersonAccountEligibility = new DataTable();
                DateTime ldtVestingDate = new DateTime();
                DateTime ldtDateatAge65 = new DateTime();
                DateTime ldtCompareBaseDate = new DateTime();
                DateTime ldtNormalRetirementDate = new DateTime();
                DateTime ldtLateRetirementDate = new DateTime();


                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                {
                    for (int i = 60; i <= 65; i++)
                    {
                        ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                        ldtDateatAge65 = DateTime.MinValue;
                        ldtNormalRetirementDate = DateTime.MinValue;
                        ldtLateRetirementDate = DateTime.MinValue;

                        iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_52 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).ToList().ToCollection();
                        int lintTotalQualifiedYears = 0;
                        int lintTotalQualifiedYears_MPIPP = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                        int lintTotalQualifiedYears_Local52 = this.aclbPersonWorkHistory_MPI.Where(item => item.L52_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                        if (lintTotalQualifiedYears_MPIPP > lintTotalQualifiedYears_Local52)
                            lintTotalQualifiedYears = lintTotalQualifiedYears_MPIPP;
                        else
                            lintTotalQualifiedYears = lintTotalQualifiedYears_Local52;
                        decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();


                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                        {
                            if (i >= rule.min_age && i < rule.max_age)
                            {
                                if ((rule.special_years.IsNull() || rule.special_years <= 0) && lintTotalQualifiedYears >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)//PIR 905
                                {
                                    return i;
                                }

                                else if (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count + Local52_PensionCredits >= rule.special_years)
                                {
                                    return i;
                                }

                            }
                        }
                    }

                    return busConstant.LOCAL_52_RETIREMENT_NORMAL_AGE;

                }
            }
            return busConstant.LOCAL_52_RETIREMENT_NORMAL_AGE;
        }

        public int GetBestAgeforReductionFactor_Local161()
        {
            //DataTable ldtbEligibility = busBase.Select("cdoBenefitProvisionEligibility.FetchAllEligibilityRules", new object[0] { });
            //Collection<cdoBenefitProvisionEligibility> aclbAllEligibilityRules = new Collection<cdoBenefitProvisionEligibility>(); // Collection that will store the Eligibility Rule                        
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                //aclbAllEligibilityRules = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(ldtbEligibility);
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();

                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                DataTable ldtbPersonAccountEligibility = new DataTable();
                DateTime ldtVestingDate = new DateTime();
                DateTime ldtDateatAge65 = new DateTime();
                DateTime ldtCompareBaseDate = new DateTime();
                DateTime ldtNormalRetirementDate = new DateTime();
                DateTime ldtLateRetirementDate = new DateTime();


                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                {
                    for (int i = 60; i <= 65; i++)
                    {
                        ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                        ldtDateatAge65 = DateTime.MinValue;
                        ldtNormalRetirementDate = DateTime.MinValue;
                        ldtLateRetirementDate = DateTime.MinValue;

                        iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_52 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).ToList().ToCollection();
                        int lintTotalQualifiedYears = 0;
                        int lintTotalQualifiedYears_MPIPP = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                        int lintTotalQualifiedYears_Local52 = this.aclbPersonWorkHistory_MPI.Where(item => item.L52_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                        if (lintTotalQualifiedYears_MPIPP > lintTotalQualifiedYears_Local52)
                            lintTotalQualifiedYears = lintTotalQualifiedYears_MPIPP;
                        else
                            lintTotalQualifiedYears = lintTotalQualifiedYears_Local52;
                        decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();


                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                        {
                            if (i >= rule.min_age && i < rule.max_age)
                            {
                                if ((rule.special_years.IsNull() || rule.special_years <= 0) && lintTotalQualifiedYears >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)//PIR 905
                                {
                                    return i;
                                }

                                else if (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count + Local52_PensionCredits >= rule.special_years)
                                {
                                    return i;
                                }

                            }
                        }
                    }

                    return busConstant.LOCAL_52_RETIREMENT_NORMAL_AGE;

                }
            }
            return busConstant.LOCAL_52_RETIREMENT_NORMAL_AGE;
        }

        public int GetBestAgeforReductionFactor_Local600()
        {
            //DataTable ldtbEligibility = busBase.Select("cdoBenefitProvisionEligibility.FetchAllEligibilityRules", new object[0] { });
            //Collection<cdoBenefitProvisionEligibility> aclbAllEligibilityRules = new Collection<cdoBenefitProvisionEligibility>(); // Collection that will store the Eligibility Rule                        
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                //aclbAllEligibilityRules = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(ldtbEligibility);
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();

                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                DataTable ldtbPersonAccountEligibility = new DataTable();
                DateTime ldtVestingDate = new DateTime();
                DateTime ldtDateatAge65 = new DateTime();
                DateTime ldtCompareBaseDate = new DateTime();
                DateTime ldtNormalRetirementDate = new DateTime();
                DateTime ldtLateRetirementDate = new DateTime();

                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                {
                    for (int i = 60; i <= 65; i++)
                    {
                        ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                        ldtDateatAge65 = DateTime.MinValue;
                        ldtNormalRetirementDate = DateTime.MinValue;
                        ldtLateRetirementDate = DateTime.MinValue;

                        iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_600 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).ToList().ToCollection();
                        int lintTotalQualifiedYears = 0;
                        int lintTotalQualifiedYears_Local600 = this.aclbPersonWorkHistory_MPI.Where(item => item.L600_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                        int lintTotalQualifiedYears_MPIPP = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();

                        if (lintTotalQualifiedYears_MPIPP > lintTotalQualifiedYears_Local600)
                            lintTotalQualifiedYears = lintTotalQualifiedYears_MPIPP;
                        else
                            lintTotalQualifiedYears = lintTotalQualifiedYears_Local600;

                        decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum();

                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                        {
                            if (i >= rule.min_age && i < rule.max_age)
                            {
                                if ((rule.special_years.IsNull() || rule.special_years <= 0) && lintTotalQualifiedYears >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                                {
                                    return i;
                                }
                                else if (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count + Local600_PensionCredits >= rule.special_years)
                                {
                                    return i;
                                }

                            }
                        }

                    }
                    return busConstant.LOCAL_600_RETIREMENT_NORMAL_AGE;
                }
            }
            return busConstant.LOCAL_600_RETIREMENT_NORMAL_AGE;
        }

        public int GetBestAgeforReductionFactor_Local700()
        {
            //DataTable ldtbEligibility = busBase.Select("cdoBenefitProvisionEligibility.FetchAllEligibilityRules", new object[0] { });
            //Collection<cdoBenefitProvisionEligibility> aclbAllEligibilityRules = new Collection<cdoBenefitProvisionEligibility>(); // Collection that will store the Eligibility Rule                        
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                //aclbAllEligibilityRules = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(ldtbEligibility);
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();

                busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                DataTable ldtbPersonAccountEligibility = new DataTable();
                DateTime ldtVestingDate = new DateTime();
                DateTime ldtDateatAge65 = new DateTime();
                DateTime ldtCompareBaseDate = new DateTime();
                DateTime ldtNormalRetirementDate = new DateTime();
                DateTime ldtLateRetirementDate = new DateTime();

                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                {
                    for (int i = 60; i <= 65; i++)
                    {
                        ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                        ldtDateatAge65 = DateTime.MinValue;
                        ldtNormalRetirementDate = DateTime.MinValue;
                        ldtLateRetirementDate = DateTime.MinValue;

                        iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.LOCAL_700 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                        int lintTotalQualifiedYears = 0;
                        int lintTotalQualifiedYears_Local700 = this.aclbPersonWorkHistory_MPI.Where(item => item.L700_Hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();
                        int lintTotalQualifiedYears_MPIPP = this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();

                        if (lintTotalQualifiedYears_MPIPP > lintTotalQualifiedYears_Local700)
                            lintTotalQualifiedYears = lintTotalQualifiedYears_MPIPP;
                        else
                            lintTotalQualifiedYears = lintTotalQualifiedYears_Local700;

                        decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum();


                        if (lintTotalQualifiedYears >= 30)
                        {
                            foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                            {
                                if ((this.idecAge >= rule.min_age && this.idecAge < rule.max_age) && ldecTotalCreditedHours >= rule.credited_hours)
                                {
                                    return i;
                                }
                            }

                        }

                    }
                    return busConstant.LOCAL_700_RETIREMENT_NORMAL_AGE;
                }
            }
            return busConstant.LOCAL_700_RETIREMENT_NORMAL_AGE;
        }
        //Code-Abhishek

        public void CheckEligibility_Retirement()
        {
            decimal ldecAgeinDecimal = this.idecAge;
            this.idecAge = Math.Floor(this.idecAge);
            idecTotalHoursTillLatestWithdrawalDate = Decimal.Zero;

            #region GetHoursTillLatestWithdrawalDateifApplicable
            DataTable ldtLatestWithdrawalDate = busBase.Select("cdoBenefitApplication.GetLatestWithdrawalDate", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtLatestWithdrawalDate.IsNotNull() && ldtLatestWithdrawalDate.Rows.Count > 0 && Convert.ToString(ldtLatestWithdrawalDate.Rows[0]["WITHDRAWAL_DATE"]).IsNotNullOrEmpty())
            {
                idtLatestWithdrawalDate = Convert.ToDateTime(ldtLatestWithdrawalDate.Rows[0]["WITHDRAWAL_DATE"]);

                if (this.icdoBenefitApplication.adtMPIVestingDate.IsNull() || this.icdoBenefitApplication.adtMPIVestingDate == DateTime.MinValue)
                {
                    int lintAccountId = 0;
                    if (this.ibusPerson.iclbPersonAccount != null)
                    {
                        if (this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).Count() > 0)
                        {
                            lintAccountId = this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id;
                        }
                    }

                    if (lintAccountId > 0)
                    {
                        DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });
                        if (ldtbPersonAccountEligibility.Rows.Count > 0 && ldtbPersonAccountEligibility.Rows[0]["VESTED_DATE"] != DBNull.Value
                            && Convert.ToString(ldtbPersonAccountEligibility.Rows[0]["VESTED_DATE"]).IsNotNullOrEmpty())
                        {
                            this.icdoBenefitApplication.adtMPIVestingDate = Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0]["VESTED_DATE"]);
                        }
                    }
                }

                if (idtLatestWithdrawalDate.IsNotNull() && this.icdoBenefitApplication.adtMPIVestingDate.IsNotNull() && idtLatestWithdrawalDate != DateTime.MinValue && this.icdoBenefitApplication.adtMPIVestingDate != DateTime.MinValue
                   && idtLatestWithdrawalDate < this.icdoBenefitApplication.adtMPIVestingDate)
                {

                    utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                    string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                    SqlParameter[] parameters = new SqlParameter[3];
                    SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                    SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);
                    SqlParameter param3 = new SqlParameter("@RETIREMENT_DATE", DbType.DateTime);

                    param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                    parameters[0] = param1;

                    param2.Value = busConstant.MPIPP;
                    parameters[1] = param2;

                    param3.Value = idtLatestWithdrawalDate;
                    parameters[2] = param3;

                    DataTable ldtWorkHistoryTillLatestWithdrawal = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataTillGivenDate", astrLegacyDBConnetion, null, parameters);
                    if (ldtWorkHistoryTillLatestWithdrawal.IsNotNull() && ldtWorkHistoryTillLatestWithdrawal.Rows.Count > 0)
                    {
                        idecTotalHoursTillLatestWithdrawalDate = ldtWorkHistoryTillLatestWithdrawal.AsEnumerable().Sum(i => i["VESTED_HOURS"] == DBNull.Value ? 0.0M : Convert.ToDecimal(i["VESTED_HOURS"]));
                    }
                }
            }


            #endregion

            //Eligibility needs to be checked for each plan   
            if (!Eligible_Plans.IsNullOrEmpty())
            {
                Eligible_Plans.Clear();
            }
            if (Eligible_Plans.IsNull())
                Eligible_Plans = new Collection<string>();
            //DataTable ldtbEligibility = busBase.Select("cdoBenefitProvisionEligibility.FetchAllEligibilityRules", new object[0] { });
            //Collection<cdoBenefitProvisionEligibility> aclbAllEligibilityRules = new Collection<cdoBenefitProvisionEligibility>(); // Collection that will store the Eligibility Rule                        
            if (iclbAllEligibilityRules != null && iclbAllEligibilityRules.Count > 0)
            {
                //aclbAllEligibilityRules = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(ldtbEligibility);
                Collection<cdoBenefitProvisionEligibility> iclbApplicableRules = new Collection<cdoBenefitProvisionEligibility>();

                foreach (busPersonAccount account in this.ibusPerson.iclbPersonAccount)
                {
                    busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                    DataTable ldtbPersonAccountEligibility = new DataTable();
                    DateTime ldtVestingDate = new DateTime();
                    DateTime ldtDateatAge65 = new DateTime();
                    DateTime ldtCompareBaseDate = new DateTime();
                    DateTime ldtNormalRetirementDate = new DateTime();
                    DateTime ldtLateRetirementDate = new DateTime();
                    DateTime ldtForfeitureDate = new DateTime(); //PIR-492

                    switch (account.icdoPersonAccount.istrPlanCode)
                    {
                        case busConstant.MPIPP:
                            if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                            {
                                if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_YES) //In this case we are going to Assume that he is Vested in IAP TOO
                                {
                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    ldtDateatAge65 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;
                                    
                                    //PIR-492
                                    ldtForfeitureDate = DateTime.MinValue;
                                    int lintQlfdYrsCntTilForfeiture = 0;
                                    decimal ldecQlfdHrsTilForfeiture = 0.0m;

                                    #region Algorithm for Late OR Normal
                                    if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id });
                                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                        {
                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                        }

                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                        if (ldtVestingDate > ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }

                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.MPIPP);
                                            Eligibile4MPI = true;

                                            //IMPNOTE Person has to retire for IAP as well compulsively so we need to PUSH the same to IAP PLAN AS WELL. IAP's OWN RULE COMES INTO PICTURE ONLY when HE IS ONLY RETIRING IN IAP
                                            if (this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                                Eligible_Plans.Add(busConstant.IAP);
                                            }

                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligibile4MPI = true;
                                            Eligible_Plans.Add(busConstant.MPIPP);

                                            //IMPNOTE Person has to retire for IAP as well compulsively so we need to PUSH the same to IAP PLAN AS WELL. IAP's OWN RULE COMES INTO PICTURE ONLY when HE IS ONLY RETIRING IN IAP
                                            if (this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                                Eligible_Plans.Add(busConstant.IAP);
                                            }
                                            break;
                                        }
                                        //else
                                        //{
                                        //    break;
                                        //}

                                    }
                                    #endregion

                                    #region For PIR-492(to take the qualified years count and qualified hours till forfeiture after withdrawn)
                                    ldtForfeitureDate = this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                                    if (ldtForfeitureDate != DateTime.MinValue)
                                    {
                                        if (idtLatestWithdrawalDate < ldtForfeitureDate && ldtForfeitureDate.Year >= 1976)
                                        {
                                            lintQlfdYrsCntTilForfeiture = (from item in this.aclbPersonWorkHistory_MPI
                                                                           where item.year > idtLatestWithdrawalDate.Year && item.year < ldtForfeitureDate.Year && item.qualified_years_count != 0
                                                                           orderby item.year descending
                                                                           select item.qualified_years_count).FirstOrDefault();

                                            ldecQlfdHrsTilForfeiture = (from item in this.aclbPersonWorkHistory_MPI
                                                                        where item.year > idtLatestWithdrawalDate.Year && item.year < ldtForfeitureDate.Year
                                                                        select item.qualified_hours).Sum();
                                        }
                                    }
                                    #endregion

                                    #region Check For Early Un-Reduced

                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();

                                    int lintTotalQualifiedYears = this.aclbPersonWorkHistory_MPI.Last().qualified_years_count + lintQlfdYrsCntTilForfeiture; //For PIR-492

                                    decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() - idecTotalHoursTillLatestWithdrawalDate;
                                    ldecTotalCreditedHours = ldecTotalCreditedHours + ldecQlfdHrsTilForfeiture; //For PIR-492
                                   
                                    #region Add Credits Hours of Other Local Plans
                                    foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                                    {
                                        if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP)
                                        {
                                            switch (lbusPersonAccount.icdoPersonAccount.istrPlanCode)
                                            {
                                                case busConstant.Local_52:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();
                                                    break;
                                                case busConstant.Local_161:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();
                                                    break;
                                                case busConstant.Local_600:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum();
                                                    break;
                                                case busConstant.Local_666:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum();
                                                    break;
                                                case busConstant.LOCAL_700:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum();
                                                    break;
                                            }
                                        }
                                    }
                                    #endregion

                                    int flag = 0;

                                    if (lintTotalQualifiedYears >= 30)
                                    {
                                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                        {
                                            if ((this.idecAge >= rule.min_age && this.idecAge <= rule.max_age) && ldecTotalCreditedHours >= rule.credited_hours)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                Eligibile4MPI = true;
                                                Eligible_Plans.Add(busConstant.MPIPP);

                                                //IMPNOTE Person has to retire for IAP as well compulsively so we need to PUSH the same to IAP PLAN AS WELL. IAP's OWN RULE COMES INTO PICTURE ONLY when HE IS ONLY RETIRING IN IAP
                                                if (this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                                                {
                                                    this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                    Eligible_Plans.Add(busConstant.IAP);
                                                }

                                                flag = 1;
                                                break;
                                            }
                                        }

                                        if (flag == 1)
                                            break;
                                    }
                                    #endregion

                                    #region Check for Special Reduced Early
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();

                                    //PIR 927
                                    //var LatestYearofForfieture = from item in this.aclbPersonWorkHistory_MPI where item.qualified_years_count == 0 orderby item.year descending select item.year;
                                    int LatestYearofForfieture = 0;
                                    if (ldtForfeitureDate != DateTime.MinValue)
                                        LatestYearofForfieture = ldtForfeitureDate.Year;

                                    //if (!LatestYearofForfieture.IsNullOrEmpty())
                                    if (LatestYearofForfieture != 0)
                                    {
                                        //ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI where item.year > (Int32)LatestYearofForfieture.First() select item.qualified_hours).Sum() - idecTotalHoursTillLatestWithdrawalDate;
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI where item.year > LatestYearofForfieture select item.qualified_hours).Sum() - idecTotalHoursTillLatestWithdrawalDate;
                                    }
                                    else
                                        ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() - idecTotalHoursTillLatestWithdrawalDate;

                                    #region Add Credits Hours of Other Local Plans
                                    foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                                    {
                                        if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP)
                                        {
                                            switch (lbusPersonAccount.icdoPersonAccount.istrPlanCode)
                                            {
                                                case busConstant.Local_52:
                                                    if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id))
                                                        ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();
                                                    break;
                                                case busConstant.Local_161:
                                                    if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id))
                                                        ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();
                                                    break;
                                                case busConstant.Local_600:
                                                    if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id))
                                                        ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum();
                                                    break;
                                                case busConstant.Local_666:
                                                    if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id))
                                                        ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum();
                                                    break;
                                                case busConstant.LOCAL_700:
                                                    if (!CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(acc => acc.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id))
                                                        ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum();
                                                    break;
                                            }
                                        }
                                    }
                                    #endregion

                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge <= rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY;
                                            Eligibile4MPI = true;
                                            Eligible_Plans.Add(busConstant.MPIPP);

                                            //IMPNOTE Person has to retire for IAP as well compulsively so we need to PUSH the same to IAP PLAN AS WELL. IAP's OWN RULE COMES INTO PICTURE ONLY when HE IS ONLY RETIRING IN IAP
                                            if (this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.IAP);
                                            }

                                            flag = 1;
                                            break;
                                        }

                                    }

                                    if (flag == 1)
                                        break;

                                    #endregion

                                    #region Check for Early Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.MPIPP && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if ((this.idecAge >= rule.min_age && this.idecAge <= rule.max_age) && this.aclbPersonWorkHistory_MPI.Last().qualified_years_count >= rule.qualified_years)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                            Eligibile4MPI = true;
                                            Eligible_Plans.Add(busConstant.MPIPP);

                                            //IMPNOTE Person has to retire for IAP as well compulsively so we need to PUSH the same to IAP PLAN AS WELL. IAP's OWN RULE COMES INTO PICTURE ONLY when HE IS ONLY RETIRING IN IAP
                                            if (this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.IAP);
                                            }

                                            flag = 1;
                                            break;
                                        }

                                    }
                                    #endregion
                                }
                            }
                            break;

                        case busConstant.IAP:
                            if (!this.aclbPersonWorkHistory_IAP.IsNullOrEmpty())
                            {
                                if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES && this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES) // This means when he is Exclusively Vested only in IAP we are going to check on this Rule.
                                {
                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    ldtDateatAge65 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;

                                    #region Algorithm for Late OR Normal
                                    if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.person_account_id });
                                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                        {
                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                        }

                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                        if (ldtVestingDate > ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }

                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.IAP);
                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.IAP);
                                            break;
                                        }
                                        //else
                                        //{
                                        //    break;
                                        //}

                                    }
                                    #endregion
                                }
                            }
                            break;

                        case busConstant.Local_52:
                            if (!IsVestingDateNull(busConstant.Local_52))
                            {
                                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                                {
                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    ldtDateatAge65 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;

                                    #region Algorithm for Late and Normal
                                    if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id });
                                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                        {
                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                        }

                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                        if (ldtVestingDate > ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }

                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.Local_52);
                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.Local_52);
                                            break;
                                        }
                                        //else
                                        //{
                                        //    break;
                                        //}

                                    }
                                    #endregion

                                    #region Check for Early Un-Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_52 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).ToList().ToCollection();
                                    int lintTotalQualifiedYears = 0;
                                    lintTotalQualifiedYears = this.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                    decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();

                                    #region Add Credits Hours of Other Local Plans
                                    foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                                    {
                                        if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP)
                                        {
                                            switch (lbusPersonAccount.icdoPersonAccount.istrPlanCode)
                                            {
                                                case busConstant.Local_161:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();
                                                    break;
                                                case busConstant.Local_600:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum();
                                                    break;
                                                case busConstant.Local_666:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum();
                                                    break;
                                                case busConstant.LOCAL_700:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum();
                                                    break;
                                            }
                                        }
                                    }
                                    #endregion

                                    int flag_52 = 0;

                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if (this.idecAge >= rule.min_age && this.idecAge <= rule.max_age)
                                        {
                                            if ((rule.special_years.IsNull() || rule.special_years <= 0) && lintTotalQualifiedYears >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.Local_52);
                                                flag_52 = 1;
                                                break;
                                            }
                                            else if (rule.special_years > 0)
                                            {
                                                //IMP - THis Special Year logic has been changed on account Changes in Vesting and Eligibility in SPRint 2.0
                                                if (this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count() > 0)
                                                {
                                                    if (this.aclbPersonWorkHistory_MPI.Where(item => item.year >= busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52.Year && item.qualified_hours >= 400).Count() + Local52_PensionCredits + Local161_PensionCredits + Local600_PensionCredits + Local700_PensionCredits + Local666_PensionCredits >= rule.special_years) //VERY IMP this condition should not be mapped like this it should be total MPI qualified Years + PENSION CREDITS
                                                    {
                                                        this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                        Eligible_Plans.Add(busConstant.Local_52);
                                                        flag_52 = 1;
                                                        break;

                                                    }
                                                }
                                            }

                                        }
                                    }

                                    if (flag_52 == 1)
                                        break;

                                    #endregion

                                    #region Check for Early Reduced

                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_52 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        //PIR 635 one Qualified year in MPI plan is not a required condition.
                                        //if (this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count() > 0)
                                        //{
                                        //PIR 635
                                        int lintTotalQualifiedYears_MPIPP = 0;
                                        if (aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count() > 0)
                                            lintTotalQualifiedYears_MPIPP = aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= busConstant.MIN_HOURS_FOR_VESTED_YEAR).Count();

                                        //PIR 635
                                        if (this.idecAge >= rule.min_age && this.idecAge <= rule.max_age && lintTotalQualifiedYears_MPIPP + Local52_PensionCredits + Local161_PensionCredits + Local600_PensionCredits + Local700_PensionCredits + Local666_PensionCredits >= rule.special_years)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.Local_52);
                                            flag_52 = 1;
                                            break;
                                        }
                                        // }

                                    }
                                    #endregion
                                }
                            }
                            break;

                        case busConstant.Local_161:
                            if (!IsVestingDateNull(busConstant.Local_161))
                            {
                                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                                {
                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    DateTime ldtDateatAge60 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;
                                    int flag_161 = 0;
                                    #region Algorithm for Late and Normal
                                    if (this.idecAge >= busConstant.LOCAL_161_RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id });
                                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                        {
                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                        }

                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge60 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(60);

                                        if (ldtVestingDate > ldtDateatAge60)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge60)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge60;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge60)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge60;
                                        }

                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.Local_161);
                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.Local_161);
                                            break;
                                        }
                                        //else
                                        //{
                                        //    break;
                                        //}

                                    }
                                    #endregion

                                    #region Check for Early Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_161 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if (this.idecAge >= rule.min_age && this.idecAge <= rule.max_age)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.Local_161);
                                            flag_161 = 1;
                                            break;
                                        }

                                    }

                                    #endregion
                                }
                            }
                            break;

                        case busConstant.Local_600:
                            if (!IsVestingDateNull(busConstant.Local_600))  //THIS ONE FUNCTON ITSELF SHOULD FILL A COMMON ICDO AS WELL. So that we can avoid 2 DB hits
                            {
                                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                                {
                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    ldtDateatAge65 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;

                                    #region Algorithm for Late and Normal
                                    //PIR :1064
                                    ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id });
                                    if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                    {
                                        lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                    }
                                    if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                        if (ldtVestingDate > ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }

                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.Local_600);
                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.Local_600);
                                            break;
                                        }
                                        //else
                                        //{
                                        //    break;
                                        //}

                                    }
                                    #endregion

                                    #region Check for Early Un-Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_600 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).ToList().ToCollection();
                                    int lintTotalQualifiedYears = 0;
                                    lintTotalQualifiedYears = this.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                                    int lintLocalQualifiedYear = 0;
                                    lintLocalQualifiedYear = lbusPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;

                                    decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum();

                                    #region Add Credits Hours of Other Local Plans
                                    foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                                    {
                                        if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP)
                                        {
                                            switch (lbusPersonAccount.icdoPersonAccount.istrPlanCode)
                                            {
                                                case busConstant.Local_52:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();
                                                    break;
                                                case busConstant.Local_161:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();
                                                    break;
                                                case busConstant.Local_666:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum();
                                                    break;
                                                case busConstant.LOCAL_700:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum();
                                                    break;
                                            }
                                        }
                                    }
                                    #endregion

                                    int flag_600 = 0;

                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if (this.idecAge >= rule.min_age && this.idecAge <= rule.max_age)
                                        {
                                            if ((rule.special_years.IsNull() || rule.special_years <= 0) && lintTotalQualifiedYears >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.Local_600);
                                                flag_600 = 1;
                                                break;
                                            }
                                            else if (rule.special_years > 0 && this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count() > 0)
                                            {
                                               
                                                if ((lintTotalQualifiedYears - lintLocalQualifiedYear) + Local52_PensionCredits + Local161_PensionCredits + Local600_PensionCredits + Local700_PensionCredits + Local666_PensionCredits >= rule.special_years) //TODO - VERY IMP this condition should not be mapped like this it should be total MPI qualified Years + PENSION CREDITS
                                                {
                                                    this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                    Eligible_Plans.Add(busConstant.Local_600);
                                                    flag_600 = 1;
                                                    break;

                                                }
                                            }

                                        }
                                    }

                                    if (flag_600 == 1)
                                        break;

                                    #endregion

                                    #region Check for Early Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_600 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if (this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count() > 0)
                                        {
                                          
                                            if (this.idecAge >= rule.min_age && this.idecAge <= rule.max_age && (lintTotalQualifiedYears - lintLocalQualifiedYear) + Local52_PensionCredits + Local161_PensionCredits + Local600_PensionCredits + Local700_PensionCredits + Local666_PensionCredits >= rule.special_years)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.Local_600);
                                                flag_600 = 1;
                                                break;
                                            }
                                        }

                                    }
                                    #endregion
                                }
                            }
                            break;

                        case busConstant.Local_666:
                            if (!IsVestingDateNull(busConstant.Local_666))
                            {
                                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                                {
                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    ldtDateatAge65 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;

                                    #region Algorithm for Late and Normal
                                    if (this.idecAge >= busConstant.RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id });
                                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                        {
                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                        }

                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                                        if (ldtVestingDate > ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge65)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge65;
                                        }
                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.Local_666);
                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.Local_666);
                                            break;
                                        }
                                        //else
                                        //{
                                        //    break;
                                        //}

                                    }
                                    #endregion

                                    #region Check for Early Un-Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_666 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).ToList().ToCollection();
                                    int lintTotalQualifiedYears = 0;
                                    lintTotalQualifiedYears = this.aclbPersonWorkHistory_MPI.Last().qualified_years_count;

                                    decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum();

                                    #region Add Credits Hours of Other Local Plans
                                    foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                                    {
                                        if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP)
                                        {
                                            switch (lbusPersonAccount.icdoPersonAccount.istrPlanCode)
                                            {
                                                case busConstant.Local_52:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();
                                                    break;
                                                case busConstant.Local_161:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();
                                                    break;
                                                case busConstant.Local_600:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum();
                                                    break;
                                                case busConstant.LOCAL_700:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum();
                                                    break;
                                            }
                                        }
                                    }
                                    #endregion

                                    int flag_666 = 0;

                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if (this.idecAge >= rule.min_age && this.idecAge <= rule.max_age)
                                        {
                                            if ((rule.special_years.IsNull() || rule.special_years <= 0) && lintTotalQualifiedYears >= rule.qualified_years && ldecTotalCreditedHours >= rule.credited_hours)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.Local_666);
                                                flag_666 = 1;
                                                break;
                                            }
                                        }
                                    }

                                    if (flag_666 == 1)
                                        break;

                                    #endregion

                                    #region Check for Early Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.Local_666 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        //TODO - ADD ON CODE NEEDS TO GO HERE PREMERGER PENSION CREDITS FOR PLAN >= rule.pensioncredits
                                        if (this.idecAge >= rule.min_age && this.idecAge <= rule.max_age && Local666_PensionCredits >= rule.pension_credits)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                            Eligible_Plans.Add(busConstant.Local_666);
                                            flag_666 = 1;
                                            break;
                                        }

                                    }
                                    #endregion
                                }
                            }
                            break;

                        case busConstant.LOCAL_700:
                            if (!IsVestingDateNull(busConstant.LOCAL_700))
                            {
                                if (!this.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                                {
                                    ldtVestingDate = DateTime.MinValue; //Re-intialize Everytime we use them
                                    DateTime ldtDateatAge62 = DateTime.MinValue;
                                    ldtNormalRetirementDate = DateTime.MinValue;
                                    ldtLateRetirementDate = DateTime.MinValue;
                                    int flag_700 = 0;
                                    #region Algorithm for Late and Normal
                                    if (this.idecAge >= busConstant.LOCAL_700_RETIREMENT_NORMAL_AGE)
                                    {
                                        ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id });
                                        if (ldtbPersonAccountEligibility.Rows.Count > 0)
                                        {
                                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                                        }

                                        ldtVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                                        ldtDateatAge62 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(62);

                                        if (ldtVestingDate > ldtDateatAge62)
                                        {
                                            ldtCompareBaseDate = ldtVestingDate;
                                        }
                                        else if (ldtVestingDate < ldtDateatAge62)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge62;
                                        }
                                        else if (ldtVestingDate == ldtDateatAge62)
                                        {
                                            ldtCompareBaseDate = ldtDateatAge62;
                                        }

                                        if (ldtCompareBaseDate.Day == 1)
                                            ldtNormalRetirementDate = ldtCompareBaseDate;
                                        else
                                            ldtNormalRetirementDate = busGlobalFunctions.GetLastDayofMonth(ldtCompareBaseDate).AddDays(1);

                                        ldtLateRetirementDate = ldtNormalRetirementDate.AddMonths(1);

                                        if (this.icdoBenefitApplication.retirement_date >= ldtLateRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_LATE;
                                            Eligible_Plans.Add(busConstant.LOCAL_700);
                                            break;
                                        }
                                        else if (this.icdoBenefitApplication.retirement_date == ldtNormalRetirementDate)
                                        {
                                            this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_NORMAL;
                                            Eligible_Plans.Add(busConstant.LOCAL_700);
                                            break;
                                        }
                                        //else
                                        //{
                                        //    break;
                                        //}

                                    }
                                    #endregion

                                    #region Check for Early Un-Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.LOCAL_700 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    int lintTotalQualifiedYears = 0;
                                    lintTotalQualifiedYears = this.aclbPersonWorkHistory_MPI.Last().qualified_years_count;

                                    decimal ldecTotalCreditedHours = (from item in this.aclbPersonWorkHistory_MPI select item.L700_Hours).Sum() + (from item in this.aclbPersonWorkHistory_MPI select item.qualified_hours).Sum();

                                    #region Add Credits Hours of Other Local Plans
                                    foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                                    {
                                        if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP)
                                        {
                                            switch (lbusPersonAccount.icdoPersonAccount.istrPlanCode)
                                            {
                                                case busConstant.Local_52:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L52_Hours).Sum();
                                                    break;
                                                case busConstant.Local_161:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L161_Hours).Sum();
                                                    break;
                                                case busConstant.Local_600:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L600_Hours).Sum();
                                                    break;
                                                case busConstant.Local_666:
                                                    ldecTotalCreditedHours += (from item in this.aclbPersonWorkHistory_MPI select item.L666_Hours).Sum();
                                                    break;
                                            }
                                        }
                                    }
                                    #endregion

                                    if (lintTotalQualifiedYears >= 30)
                                    {
                                        foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                        {
                                            if ((this.idecAge >= rule.min_age && this.idecAge <= rule.max_age) && ldecTotalCreditedHours >= rule.credited_hours)
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.LOCAL_700);
                                                flag_700 = 1;
                                                break;
                                            }
                                        }

                                        if (flag_700 == 1)
                                            break;
                                    }
                                    #endregion

                                    #region Check for Early Reduced
                                    iclbApplicableRules = iclbAllEligibilityRules.Where(rule => rule.istrPlanCode == busConstant.LOCAL_700 && rule.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT && rule.eligibility_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY).OrderByDescending(rule => rule.max_age).ToList().ToCollection();
                                    foreach (cdoBenefitProvisionEligibility rule in iclbApplicableRules)
                                    {
                                        if (this.idecAge >= rule.min_age && this.idecAge <= rule.max_age && (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count - this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count() > 0))
                                        {
                                            //PIR-565 Commenting following check.(Rashmi)
                                            //if (this.aclbPersonWorkHistory_MPI.Where(item => item.qualified_hours >= 400).Count() > 0)
                                            //{
                                            if (this.idecAge >= rule.min_age && this.idecAge < rule.max_age && (this.aclbPersonWorkHistory_MPI.Last().qualified_years_count + Local52_PensionCredits + Local161_PensionCredits + Local600_PensionCredits + Local700_PensionCredits + Local666_PensionCredits >= rule.special_years))
                                            {
                                                this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.istrRetirementSubType = busConstant.RETIREMENT_TYPE_REDUCED_EARLY;
                                                Eligible_Plans.Add(busConstant.LOCAL_700);
                                                flag_700 = 1;
                                                break;
                                            }
                                            //}
                                        }
                                    }
                                    #endregion
                                }
                            }
                            break;
                    }
                }
            }
            this.idecAge = ldecAgeinDecimal; // Doing this since we need ages in Decimal for Calculation purposes but for Eligibility we want to Conside FULL ages
        }
        //Code-Abhishek

        //Code-Abhishek 
        private void CheckEligiblity_Withdrawal()
        {
            IDbConnection lconLegacy = DBFunction.GetDBConnection("Legacy");
            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
            //Eligibility needs to be checked for each plan   
            if (!Eligible_Plans.IsNullOrEmpty())
            {
                Eligible_Plans.Clear();
            }

            foreach (busPersonAccount account in this.ibusPerson.iclbPersonAccount)
            {
                int lCount = 0;
                int lHoursFound = 0;
                int lCountRetired = 0;
                int lCountDisabled = 0;
                bool lblnRetired = busConstant.BOOL_FALSE;
                bool lblnDisabled = busConstant.BOOL_FALSE;

                DateTime CompareDate = new DateTime();
                if (!lcolParameters.IsNullOrEmpty())
                    lcolParameters.Clear();

                switch (account.icdoPersonAccount.istrPlanCode)
                {
                    case busConstant.Local_52:
                        #region Local52 Special Account Eligibility Check
                        lCount = (int)DBFunction.DBExecuteScalar("cdoPersonAccount.CheckPersonHasSpecialAccount", new object[1] { account.icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (lCount > 0)
                        {
                            CompareDate = this.icdoBenefitApplication.withdrawal_date.AddMonths(-3); // No Hours should be reported in Last 3 consecutive months from Withdrawal Date                                
                            if (lconLegacy != null)
                            {
                                IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
                                lobjParameter.ParameterName = "@SSN";
                                lobjParameter.DbType = DbType.String;
                                lobjParameter.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString();
                                lcolParameters.Add(lobjParameter);

                                IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
                                lobjParameter1.ParameterName = "@START_DATE";
                                lobjParameter1.DbType = DbType.DateTime;
                                lobjParameter1.Value = CompareDate;
                                lcolParameters.Add(lobjParameter1);

                                IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
                                lobjParameter2.ParameterName = "@END_DATE";
                                lobjParameter2.DbType = DbType.DateTime;
                                lobjParameter2.Value = this.icdoBenefitApplication.withdrawal_date;
                                lcolParameters.Add(lobjParameter2);

                                IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
                                lobjParameter3.ParameterName = "@RESULT";
                                lobjParameter3.DbType = DbType.Int32;
                                lobjParameter3.Direction = ParameterDirection.ReturnValue;
                                lcolParameters.Add(lobjParameter3);

                                DBFunction.DBExecuteProcedure("usp_CheckHoursReportedInGivenInterval", lcolParameters, lconLegacy, null);
                                lHoursFound = Convert.ToInt32(lcolParameters[3].Value);
                            }

                            lCountRetired = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.CheckIfRetired", new object[1] { account.icdoPersonAccount.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            if (lCountRetired > 0)
                                lblnRetired = busConstant.BOOL_TRUE;

                            lCountDisabled = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.CheckIfDisabled", new object[1] { account.icdoPersonAccount.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            if (lCountDisabled > 0)
                                lblnDisabled = busConstant.BOOL_TRUE;

                            decimal decAge = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.withdrawal_date);
                            if ((decAge >= Convert.ToDecimal(busConstant.LOCAL52_SPC_ACC_WDRL_DATE)) ||
                                ((decAge < Convert.ToDecimal(busConstant.LOCAL52_SPC_ACC_WDRL_DATE)) & (lHoursFound == 0 || lblnRetired || lblnDisabled)))
                            {
                                EligibileforL52Spl = true;
                                if (!Eligible_Plans.Contains(busConstant.IAP))
                                    Eligible_Plans.Add(busConstant.IAP);
                            }
                        }
                        #endregion
                        break;

                    case busConstant.Local_161:
                        #region Local161 Special Account Eligibility Check
                        lCount = (int)DBFunction.DBExecuteScalar("cdoPersonAccount.CheckPersonHasSpecialAccount", new object[1] { account.icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (lCount > 0)
                        {

                            CompareDate = this.icdoBenefitApplication.withdrawal_date.AddMonths(-6); // No Hours should be reported in Last 6 consecutive months from Withdrawal Date
                            if (lconLegacy != null)
                            {
                                IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
                                lobjParameter.ParameterName = "@SSN";
                                lobjParameter.DbType = DbType.String;
                                lobjParameter.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted.ToString();
                                lcolParameters.Add(lobjParameter);

                                IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
                                lobjParameter1.ParameterName = "@START_DATE";
                                lobjParameter1.DbType = DbType.DateTime;
                                lobjParameter1.Value = CompareDate;
                                lcolParameters.Add(lobjParameter1);

                                IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
                                lobjParameter2.ParameterName = "@END_DATE";
                                lobjParameter2.DbType = DbType.DateTime;
                                lobjParameter2.Value = this.icdoBenefitApplication.withdrawal_date;
                                lcolParameters.Add(lobjParameter2);

                                IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
                                lobjParameter3.ParameterName = "@RESULT";
                                lobjParameter3.DbType = DbType.Int32;
                                lobjParameter3.Direction = ParameterDirection.ReturnValue;
                                lcolParameters.Add(lobjParameter3);

                                DBFunction.DBExecuteProcedure("usp_CheckHoursReportedInGivenInterval", lcolParameters, lconLegacy, null);
                                lHoursFound = Convert.ToInt32(lcolParameters[3].Value);
                            }

                            lCountRetired = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.CheckIfRetired", new object[1] { account.icdoPersonAccount.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            if (lCountRetired > 0)
                                lblnRetired = busConstant.BOOL_TRUE;

                            lCountDisabled = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.CheckIfDisabled", new object[1] { account.icdoPersonAccount.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            if (lCountDisabled > 0)
                                lblnDisabled = busConstant.BOOL_TRUE;

                            if ((lHoursFound == 0) || (this.idecAge >= Convert.ToDecimal(busConstant.LOCAL161_SPC_ACC_WDRL_DATE)) || (lblnRetired) || (lblnDisabled))
                            {
                                EligibileforL161Spl = true;
                                if (!Eligible_Plans.Contains(busConstant.IAP))
                                    Eligible_Plans.Add(busConstant.IAP);
                            }
                        }
                        #endregion
                        break;

                    case busConstant.IAP:
                        // This means when he is Exclusively Vested only in IAP we are going to check on this Rule.
                        //Ticket#151762
                        if (this.icdoBenefitApplication.istrIsPersonVestedinIAP == busConstant.FLAG_YES && this.icdoBenefitApplication.istrIsPersonVestedinMPI != busConstant.FLAG_YES) //&& this.idecAge < busConstant.RETIREMENT_NORMAL_AGE)
                        {
                            bool lblnIs2024BIS = false;
                            if (this.aclbPersonWorkHistory_IAP.Where(item => item.qualified_hours < 200 && item.year == 2023).Count() > 0
                                     && this.aclbPersonWorkHistory_IAP.Where(item => item.qualified_hours < 200 && item.year == 2024).Count() > 0)
                            {
                                lblnIs2024BIS = true;
                            }
                            cdoDummyWorkData lcdoIAP = this.aclbPersonWorkHistory_IAP.Where(item => item.year < this.icdoBenefitApplication.withdrawal_date.Year).Last();
                            if (lcdoIAP.istrBisParticipantFlag == busConstant.Flag_Yes || (lcdoIAP.year == 2024 && lblnIs2024BIS)) // lcdoIAP.bis_years_count >= 2 PROD PIR 273
                            {
                                //if (this.aclbPersonWorkHistory_IAP.Where(item => item.qualified_hours < 400 && item.year == this.icdoBenefitApplication.withdrawal_date.Year).Count() > 0)
                                //{
                                //PROD PIR 604
                                if (this.aclbPersonWorkHistory_IAP.Where(item => item.qualified_hours < 400 && item.year == this.icdoBenefitApplication.withdrawal_date.Year).Count() > 0
                                     || this.aclbPersonWorkHistory_IAP.Where(item => item.year == this.icdoBenefitApplication.withdrawal_date.Year).Count() == 0)
                                {
                                    Eligibile4IAP = true;
                                    if (!Eligible_Plans.Contains(busConstant.IAP))
                                        Eligible_Plans.Add(busConstant.IAP);
                                }
                            }
                        }
                        break;

                    case busConstant.MPIPP:
                        DataTable ldtblCount = busBase.Select("cdoPersonAccountRetirementContribution.CheckIfEEAmountPresent", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id });
                        DataTable ldtblCountUVHP = busBase.Select("cdoPersonAccountRetirementContribution.CheckIfUVHPAmountPresent", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id });
                        if ((ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0) || (ldtblCountUVHP.Rows.Count > 0 && Convert.ToInt32(ldtblCountUVHP.Rows[0][0]) > 0))
                        {
                            if (!Eligible_Plans.Contains(busConstant.MPIPP))
                                Eligible_Plans.Add(busConstant.MPIPP);
                        }
                        break;

                }
            }
            lconLegacy.Close();
        }
        //Code-Abhishek 

        private void RemoveUnwantedPadding(Collection<cdoDummyWorkData> aclbPersonWorkHistory, string astrPlanCode, int LatestForfeitureYear)
        {
            if (astrPlanCode == busConstant.MPIPP)
            {
                foreach (int lintYear in larrYears_MPI)
                {
                    if (lintYear > LatestForfeitureYear)
                    {
                        if (aclbPersonWorkHistory.Where(t => t.year == lintYear).Count() > 0 && aclbPersonWorkHistory.Where(t => t.year >= lintYear && t.vested_hours <= 0).Count() == 0)
                            aclbPersonWorkHistory.Remove(aclbPersonWorkHistory.Where(t => t.year == lintYear).First());

                    }
                }
            }
            else if (astrPlanCode == busConstant.IAP)
            {
                foreach (int lintYear in larrYears_IAP)
                {
                    if (lintYear > LatestForfeitureYear)
                    {
                        if (aclbPersonWorkHistory.Where(t => t.year == lintYear).Count() > 0 && aclbPersonWorkHistory.Where(t => t.year >= lintYear && t.vested_hours <= 0).Count() == 0)
                            aclbPersonWorkHistory.Remove(aclbPersonWorkHistory.Where(t => t.year == lintYear).First());
                    }
                }
            }
        }

        public void ProcessWorkHistorytoRemoveUnwantedForFieture(Collection<cdoDummyWorkData> aclbPersonWorkHistory, string astrPlanCode, DateTime adtVestingDate, bool UpdateForfeitureDate, string astrPersonAccStatus = "", int aintEndofWorkYear = 0)
        {
            int lintYearMPIPlanBegins = 0;

            int lintWithDrawalYearBefore1976 = 0;
            DateTime ldteWithDrawalDate = new DateTime();

            string lstrBISParticipant = busConstant.FLAG_NO; //RID 52909

            DataTable ldtbWithdrawalBefore1976 = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawalBefore1976", new object[2] { this.ibusPerson.icdoPerson.person_id, astrPlanCode });
            //if (ldtbWithdrawalBefore1976.IsNotNull() && ldtbWithdrawalBefore1976.Rows.Count > 0)
            //    lintWithDrawalYearBefore1976 = Convert.ToDateTime(ldtbWithdrawalBefore1976.Rows[0][0].ToString()).Year;

            if (aclbPersonWorkHistory.Where(item => item.qualified_hours > 0).Count() > 0)
                lintYearMPIPlanBegins = aclbPersonWorkHistory.Where(item => item.qualified_hours > 0).First().year;

            Collection<cdoDummyWorkData> iclbSubSetToRemoveFF = new Collection<cdoDummyWorkData>();
            if (!aclbPersonWorkHistory.IsNullOrEmpty())
            {
                if (this.ibusTempPersonAccountEligibility.IsNotNull() || (adtVestingDate.IsNotNull() && adtVestingDate != DateTime.MinValue))
                {
                    iclbSubSetToRemoveFF = aclbPersonWorkHistory.Where(item => item.year >= adtVestingDate.Year).ToList().ToCollection();

                    int aintTotalQualifiedYearsCount = 0;
                    int aintTotalAnniversaryYearsCount = 0;
                    int aintTotalVestedYearsCount = 0;
                    int BISCounter = 0;
                    //BIS_2024_Update
                    //defining local variable to check 2022 status to handle 2023 special scenario for Qualified year and BIS rule.
                    bool lbln2022YearExists = false;
                    int lint2022QualifiedYearCount = 0;
                    int lint2022NonQualifiedYearCount = 0;
                    int lint2022BISYearCount = 0;
                    decimal ld2022VestedHours = 0.0m;
                    decimal ld2023VestedHours = 0.0m;

                    foreach (cdoDummyWorkData item in iclbSubSetToRemoveFF)
                    {
                        if (item.comments.IsNotNullOrEmpty() && (item.comments.EndsWith(busConstant.FORFEITURE_COMMENT) || item.comments.EndsWith(busConstant.FORFIETURE)))
                            item.comments = String.Empty;

                        item.iblnWithdrawalReset = false;

                        if (ldtbWithdrawalBefore1976.IsNotNull() && ldtbWithdrawalBefore1976.Rows.Count > 0)
                        {
                            if (ldtbWithdrawalBefore1976.AsEnumerable().Where(r => Convert.ToInt32(r["WITHDRAWAL_YEAR"]) == item.year).Any())
                            {
                                lintWithDrawalYearBefore1976 = Convert.ToInt32(ldtbWithdrawalBefore1976.AsEnumerable().Where(r => Convert.ToInt32(r["WITHDRAWAL_YEAR"]) == item.year).AsDataTable().Rows[0]["WITHDRAWAL_YEAR"]);
                                ldteWithDrawalDate = Convert.ToDateTime(ldtbWithdrawalBefore1976.AsEnumerable().Where(r => Convert.ToInt32(r["WITHDRAWAL_YEAR"]) == item.year).AsDataTable().Rows[0]["WITHDRAWAL_DATE"]);
                            }
                        }

                        if (lintWithDrawalYearBefore1976 != 0 && item.year == lintWithDrawalYearBefore1976)
                        {
                            //Prod PIR 10,56
                            int lintPlan = 0;
                            if (astrPlanCode == busConstant.MPIPP)
                                lintPlan = busConstant.MPIPP_PLAN_ID;
                            else if (astrPlanCode == busConstant.IAP)
                                lintPlan = busConstant.IAP_PLAN_ID;
                            decimal ldecHoursAfterWithdrawal = decimal.Zero;
                            busCalculation lbusCalculation = new busCalculation();
                            DateTime ldtToDate = new DateTime(item.year, 12, 31);
                            ldecHoursAfterWithdrawal = lbusCalculation.GetWorkDataAfterDate(this.ibusPerson.icdoPerson.istrSSNNonEncrypted, item.year, lintPlan, ldteWithDrawalDate);
                            if (ldecHoursAfterWithdrawal >= 400)
                            {
                                item.iblnWithdrawalReset = true;
                                aintTotalQualifiedYearsCount = 1;
                                aintTotalAnniversaryYearsCount = 0;
                                aintTotalVestedYearsCount = 1;
                                BISCounter = 0;

                                item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                                item.qualified_years_count = aintTotalQualifiedYearsCount;
                                item.vested_years_count = aintTotalVestedYearsCount;

                            }
                            else
                            {
                                item.iblnWithdrawalReset = true;
                                aintTotalQualifiedYearsCount = 0;
                                aintTotalAnniversaryYearsCount = 0;
                                aintTotalVestedYearsCount = 0;
                                BISCounter = 0;

                                item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                                item.qualified_years_count = aintTotalQualifiedYearsCount;
                                item.vested_years_count = aintTotalVestedYearsCount;
                            }
                        }


                        if (item.year == adtVestingDate.Year)  // Initialization with the Vested Date Thing.
                        {
                            aintTotalAnniversaryYearsCount = item.anniversary_years_count;
                            aintTotalQualifiedYearsCount = item.qualified_years_count;
                            aintTotalVestedYearsCount = item.vested_years_count;
                            BISCounter = item.bis_years_count;
                            if (UpdateForfeitureDate)
                            {
                                if (astrPlanCode == busConstant.MPIPP)
                                {
                                    if (!ForfeitureYearsCollectionMPI.IsNullOrEmpty() && ForfeitureYearsCollectionMPI.Where(date => date < adtVestingDate.Year).OrderByDescending(date => date).Count() > 0)
                                    {
                                        int Year = ForfeitureYearsCollectionMPI.Where(date => date < adtVestingDate.Year).OrderByDescending(date => date).First();
                                        AddUpdateForfeitureDate(Year, astrPlanCode, false);
                                    }
                                }
                                else if (astrPlanCode == busConstant.IAP)
                                {
                                    if (!ForfeitureYearsCollectionIAP.IsNullOrEmpty() && ForfeitureYearsCollectionIAP.Where(date => date < adtVestingDate.Year).OrderByDescending(date => date).Count() > 0)
                                    {
                                        int Year = ForfeitureYearsCollectionIAP.Where(date => date < adtVestingDate.Year).OrderByDescending(date => date).First();
                                        AddUpdateForfeitureDate(Year, astrPlanCode, false);
                                    }
                                }

                            }
                            continue;
                        }


                        //if (lintWithDrawalYearBefore1976 != 0 && item.year == lintWithDrawalYearBefore1976)
                        //{
                        //    aintTotalQualifiedYearsCount = 0;
                        //    aintTotalAnniversaryYearsCount = 0;
                        //    aintTotalVestedYearsCount = 0;
                        //    BISCounter = 0;
                        //    item.istrForfietureFlag = busConstant.FLAG_YES;

                        //    item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                        //    item.qualified_years_count = aintTotalQualifiedYearsCount;
                        //    item.vested_years_count = aintTotalVestedYearsCount;
                        //    item.istrForfietureFlag = busConstant.FLAG_YES;
                        //}

                        if (item.istrForfietureFlag == busConstant.FLAG_YES)
                            item.istrForfietureFlag = busConstant.FLAG_NO;

                        if (!item.iblnWithdrawalReset)
                        {
                            item.anniversary_years_count = ++aintTotalAnniversaryYearsCount; // To update the in-memory collection with current counters
                        }
                        #region Local Merge ( tusharchandak )
                        //PIR 753
                        //if (iobjPassInfo.istrFormName == "wfmAnnualBenefitSummaryOverviewMaintenance" || iobjPassInfo.istrFormName == "wfmPersonOverviewMaintenance" ||
                        //    iobjPassInfo.istrFormName == string.Empty)//Rashmi-for pir-565(use following code in case of Person_Overview Screen,AnnualBenefitSummaryOverview Screen and in case of batches)
                        //{
                        int iintLocalQualifiedYearsCount = 0;
                        if (item.year == (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_700).Year)
                        {
                            busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();
                            if (this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0)
                            {
                                lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id);
                                if (lbusLocalPersonAccountEligibility != null)
                                {
                                    iintLocalQualifiedYearsCount = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                }
                                aintTotalQualifiedYearsCount = aintTotalQualifiedYearsCount + iintLocalQualifiedYearsCount;
                                aintTotalVestedYearsCount = aintTotalVestedYearsCount + iintLocalQualifiedYearsCount;
                                //PIR 753
                                item.L700_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                            }
                        }
                        if (item.year == (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_600).Year)
                        {
                            busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();
                            if (this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0)
                            {
                                lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id);
                                if (lbusLocalPersonAccountEligibility != null)
                                {
                                    iintLocalQualifiedYearsCount = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                }
                                aintTotalQualifiedYearsCount = aintTotalQualifiedYearsCount + iintLocalQualifiedYearsCount;
                                aintTotalVestedYearsCount = aintTotalVestedYearsCount + iintLocalQualifiedYearsCount;
                                //PIR 753
                                item.L600_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                            }
                        }
                        if (item.year == (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_666).Year)
                        {
                            busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();
                            if (this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0)
                            {
                                lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id);
                                if (lbusLocalPersonAccountEligibility != null)
                                {
                                    iintLocalQualifiedYearsCount = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                }
                                aintTotalQualifiedYearsCount = aintTotalQualifiedYearsCount + iintLocalQualifiedYearsCount;
                                aintTotalVestedYearsCount = aintTotalVestedYearsCount + iintLocalQualifiedYearsCount;
                                //PIR 753
                                item.L666_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                            }
                        }
                        if (item.year == (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_161).Year)
                        {
                            busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();
                            if (this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0)
                            {
                                lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id);
                                if (lbusLocalPersonAccountEligibility != null)
                                {
                                    iintLocalQualifiedYearsCount = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                }
                                aintTotalQualifiedYearsCount = aintTotalQualifiedYearsCount + iintLocalQualifiedYearsCount;
                                aintTotalVestedYearsCount = aintTotalVestedYearsCount + iintLocalQualifiedYearsCount;
                                //PIR 753
                                item.L161_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                            }
                        }
                        if (item.year == (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52).Year)
                        {
                            busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();
                            if (this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0)
                            {
                                lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id);
                                if (lbusLocalPersonAccountEligibility != null)
                                {
                                    iintLocalQualifiedYearsCount = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                }
                                aintTotalQualifiedYearsCount = aintTotalQualifiedYearsCount + iintLocalQualifiedYearsCount;
                                aintTotalVestedYearsCount = aintTotalVestedYearsCount + iintLocalQualifiedYearsCount;
                                //PIR 753
                                item.L52_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                            }
                        }
                        //} //PIR 753
                        #endregion



                        if ((item.qualified_hours >= 400 && !item.iblnWithdrawalReset)) //PIR 753 ||
                        //(item.L700_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L600_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L666_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L161_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L52_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id)))
                        {
                            aintTotalQualifiedYearsCount++;
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                        }

                        if (item.vested_hours >= 200) //PIR 753 ||
                        //(item.L700_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L600_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L666_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L161_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L52_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id)))
                        {
                            if ((item.vested_hours >= 400 && !item.iblnWithdrawalReset)) //PIR 753 ||
                            //(item.L700_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id)) ||
                            //(item.L600_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id)) ||
                            //(item.L666_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id)) ||
                            //(item.L161_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id)) ||
                            //(item.L52_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id)))
                            {
                                aintTotalVestedYearsCount++;
                                item.vested_years_count = aintTotalVestedYearsCount;
                                lstrBISParticipant = busConstant.FLAG_NO; //RID 52909
                            }
                            else
                            {
                                item.qualified_years_count = aintTotalQualifiedYearsCount;
                                item.vested_years_count = aintTotalVestedYearsCount;
                            }

                            //PIR 911
                            if (BISCounter >= 2 && item.vested_hours < 400)
                                lstrBISParticipant = busConstant.FLAG_YES;

                            if (item.vested_hours >= 200)
                                BISCounter = 0;

                            //RID 52909
                            if (BISCounter >= 2)
                                lstrBISParticipant = busConstant.FLAG_YES;

                        }
                        if ((this.icdoBenefitApplication.retirement_date == DateTime.MinValue || this.icdoBenefitApplication.retirement_date.Year >= 2023) &&  item.year == 2023 && astrPlanCode == busConstant.MPIPP && (item.qualified_hours >= 65 && item.qualified_hours < 400))
                        {
                            aintTotalQualifiedYearsCount++;
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                        }

                        if (item.vested_hours < 200 && (astrPersonAccStatus != busConstant.PERSON_ACCOUNT_STATUS_RETIRED || astrPersonAccStatus != busConstant.PERSON_ACCOUNT_STATUS_DECEASED) && (item.year < aintEndofWorkYear || aintEndofWorkYear == 0 || aintEndofWorkYear == 1) && item.year >= lintYearMPIPlanBegins)
                        {
                            if (this.icdoBenefitApplication.retirement_date.Year == item.year)
                            {
                                // PROD PIR 62
                                item.bis_years_count = BISCounter;
                                item.qualified_years_count = aintTotalQualifiedYearsCount;
                                item.vested_years_count = aintTotalVestedYearsCount;
                                continue;
                            }

                            if (astrPlanCode.IsNotNullOrEmpty() && item.year != aclbPersonWorkHistory.First().year &&
                                ((aclbPersonWorkHistory.ElementAt(aclbPersonWorkHistory.IndexOf(item) - 1).L700_Hours + item.vested_hours > 200 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id) && item.year >= this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.idtMergerDate.Year) ||
                                    (aclbPersonWorkHistory.ElementAt(aclbPersonWorkHistory.IndexOf(item) - 1).L600_Hours + item.vested_hours > 200 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id) && item.year >= this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.idtMergerDate.Year) ||
                                    (aclbPersonWorkHistory.ElementAt(aclbPersonWorkHistory.IndexOf(item) - 1).L666_Hours + item.vested_hours > 200 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id) && item.year >= this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.idtMergerDate.Year) ||
                                    (aclbPersonWorkHistory.ElementAt(aclbPersonWorkHistory.IndexOf(item) - 1).L161_Hours + item.vested_hours > 200 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id) && item.year >= this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.idtMergerDate.Year) ||
                                    (aclbPersonWorkHistory.ElementAt(aclbPersonWorkHistory.IndexOf(item) - 1).L52_Hours + item.vested_hours > 200 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id) && item.year >= this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.idtMergerDate.Year)
                                )
                                )
                            { }
                            else { BISCounter++; }

                            //BIS_2024_Update
                            if ((item.year == 2023 && ((lbln2022YearExists && lint2022BISYearCount < 2) || !lbln2022YearExists) )  //not already in BIS in 2022, disregarding 2023 for BIS.
                                || (item.year == 2024 && item.vested_hours < 200 && ld2023VestedHours < 200 && ld2022VestedHours >= 400) //5_21_2025 revised rule by Gary
                                ) 
                            {
                                BISCounter--;
                            }

                            if (item.year == 2024 && lbln2022YearExists && lint2022BISYearCount == 1 && ld2023VestedHours < 200 && ld2022VestedHours < 200)
                            {
                                BISCounter = lint2022BISYearCount + 1;
                            }

                            item.bis_years_count = BISCounter;
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                            item.vested_years_count = aintTotalVestedYearsCount;

                            if (BISCounter >= 2)
                            {
                                item.comments = busConstant.BIS_PARTICIPANT + "(MPI_PLAN)";
                                lstrBISParticipant = busConstant.FLAG_YES;
                            }
                        }
                        else
                        {
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                            item.vested_years_count = aintTotalVestedYearsCount;

                            ////RID 52909 //PIR 911
                            //if (item.vested_hours < 400)
                            //    lstrBISParticipant = busConstant.FLAG_YES;
                        }

                        //RID 52909 //PIR 911
                        if (item.istrBisParticipantFlag.IsNullOrEmpty() && item.iintNonQualifiedYears >= 2)
                        {
                            item.istrBisParticipantFlag = lstrBISParticipant;
                            if (lstrBISParticipant == busConstant.FLAG_YES)
                                item.comments = busConstant.BIS_PARTICIPANT + "(MPI_PLAN)";
                        }

                        if(item.year == 2022)
                        {
                            lbln2022YearExists = true;
                            lint2022QualifiedYearCount = item.qualified_years_count;
                            lint2022NonQualifiedYearCount = item.iintNonQualifiedYears;
                            lint2022BISYearCount = item.bis_years_count;
                            ld2022VestedHours = item.vested_hours;
                        }
                        if (item.year == 2023)
                        {
                            ld2023VestedHours = item.vested_hours;
                        }

                    }
                }
                #region Commented Code-- THIS DOESNT SEEM TO BE REQUIRED REALLY AT ALL
                // NO NEED TO DO THIS SINCE SOMETIME WE NEED TO CHECK BIS PARTICIPANT AS OF TODAY LIKE IN THE CASE OF WITHDRAWAL
                //if (astrPlanCode == busConstant.MPIPP)
                //{
                //    foreach (int lintYear in larrYears_MPI)
                //    {
                //        if (lintYear > adtVestingDate.Year)
                //        {
                //            if (aclbPersonWorkHistory.Where(t => t.year == lintYear).Count() > 0)
                //              aclbPersonWorkHistory.Remove(aclbPersonWorkHistory.Where(t => t.year == lintYear).First());
                //        }
                //    }
                //}
                //else if (astrPlanCode == busConstant.IAP)
                //{
                //    foreach (int lintYear in larrYears_IAP)
                //    {
                //        if (lintYear > adtVestingDate.Year)
                //        {
                //            if (aclbPersonWorkHistory.Where(t => t.year == lintYear).Count() > 0)
                //              aclbPersonWorkHistory.Remove(aclbPersonWorkHistory.Where(t => t.year == lintYear).First());
                //        }
                //    }
                //}
                #endregion

            }
        }
        //PIR - 930
        public void ProcessWorkHistoryPaddingBetweenRetirementAndReEmployment( int RetirementYear, int ReEmploymentBeginYear,Collection<cdoDummyWorkData> aclbPersonWorkHistory)
        {
            int lintYearDiff = ReEmploymentBeginYear - RetirementYear;

            if(lintYearDiff > 0)
            {
                for(int i = RetirementYear+1; i < ReEmploymentBeginYear;i++)
                {
                    cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                    lcdoDummyWorkData.year = i;
                    lcdoDummyWorkData.qualified_hours = 0.0m;
                    lcdoDummyWorkData.vested_hours = 0.0m;
                    aclbPersonWorkHistory.Add(lcdoDummyWorkData);
                }
            }
        }

        public void ProcessWorkHistoryPadding(Collection<cdoDummyWorkData> aclbPersonWorkHistory, string astrPlanCode, bool ablnPaddingAttheEnd = true)
        {
            Collection<cdoDummyWorkData> aclbTemp = new Collection<cdoDummyWorkData>();

            foreach (cdoDummyWorkData lcdoDummyWorkData in aclbPersonWorkHistory)
            {
                cdoDummyWorkData lobjDummyWorkData = new cdoDummyWorkData();
                lobjDummyWorkData = lcdoDummyWorkData;
                aclbTemp.Add(lobjDummyWorkData);
            }


            //if (!NonAffiliate)
            //{
            if (astrPlanCode == busConstant.MPIPP)
                larrYears_MPI.Clear();
            else if (astrPlanCode == busConstant.IAP)
                larrYears_IAP.Clear();
            //}
            //else
            //{
            //    if (astrPlanCode == busConstant.MPIPP)
            //        larrYears_MPI_NA.Clear();
            //    else if (astrPlanCode == busConstant.IAP)
            //        larrYears_IAP_NA.Clear();
            //}             

            foreach (cdoDummyWorkData item in aclbTemp)
            {
                if ((aclbTemp.IndexOf(item) + 1 <= aclbTemp.Count - 1) && aclbTemp[aclbTemp.IndexOf(item) + 1].year - item.year > 1)
                {
                    int lintYearDiff = aclbTemp[aclbTemp.IndexOf(item) + 1].year - item.year;

                    for (int i = 1; i <= lintYearDiff - 1; i++)
                    {
                        cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                        lcdoDummyWorkData.year = item.year + i;
                        lcdoDummyWorkData.qualified_hours = 0.0m;
                        lcdoDummyWorkData.vested_hours = 0.0m;

                        //if (!NonAffiliate)
                        //{
                        if (astrPlanCode == busConstant.MPIPP)
                            larrYears_MPI.Add(lcdoDummyWorkData.year);
                        else if (astrPlanCode == busConstant.IAP)
                            larrYears_IAP.Add(lcdoDummyWorkData.year);
                        //}
                        //else
                        //{
                        //    if (astrPlanCode == busConstant.MPIPP)
                        //        larrYears_MPI_NA.Add(lcdoDummyWorkData.year);
                        //    else if (astrPlanCode == busConstant.IAP)
                        //        larrYears_IAP_NA.Add(lcdoDummyWorkData.year);
                        //}

                        aclbPersonWorkHistory.Insert(aclbPersonWorkHistory.IndexOf(item) + i, lcdoDummyWorkData);
                    }
                }
            }

            if (ablnPaddingAttheEnd)
            {
                //PIR 355
                if (aclbTemp.Count > 0)
                {
                    cdoDummyWorkData lDummyWorkDataLast = aclbTemp.Last();
                    int lintYearDiff = 0;

                    if (this.icdoBenefitApplication.retirement_date.Year >= DateTime.Now.Year)
                    {
                        lintYearDiff = DateTime.Now.Year - lDummyWorkDataLast.year;
                        DateTime SatBeforeLastThrusday = busGlobalFunctions.GetLastDateOfComputationYear(DateTime.Now.Year);

                        if (DateTime.Now <= SatBeforeLastThrusday)
                        {
                            lintYearDiff = lintYearDiff - 1;
                        }
                    }
                    else if (this.icdoBenefitApplication.retirement_date.Year > lDummyWorkDataLast.year)
                    {
                        lintYearDiff = this.icdoBenefitApplication.retirement_date.Year - lDummyWorkDataLast.year;
                    }

                    for (int i = 1; i <= lintYearDiff; i++)
                    {
                        cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                        lcdoDummyWorkData.year = lDummyWorkDataLast.year + i;
                        lcdoDummyWorkData.qualified_hours = 0.0m;
                        lcdoDummyWorkData.vested_hours = 0.0m;

                        //if (!NonAffiliate)
                        //{
                        if (astrPlanCode == busConstant.MPIPP)
                            larrYears_MPI.Add(lcdoDummyWorkData.year);
                        else if (astrPlanCode == busConstant.IAP)
                            larrYears_IAP.Add(lcdoDummyWorkData.year);
                        //}
                        //else
                        //{
                        //    if (astrPlanCode == busConstant.MPIPP)
                        //        larrYears_MPI_NA.Add(lcdoDummyWorkData.year);
                        //    else if (astrPlanCode == busConstant.IAP)
                        //        larrYears_IAP_NA.Add(lcdoDummyWorkData.year);
                        //}

                        aclbPersonWorkHistory.Insert(aclbPersonWorkHistory.IndexOf(lDummyWorkDataLast) + i, lcdoDummyWorkData);
                    }

                }
            }

        }


        public Collection<cdoDummyWorkData> PaddingForBridgingService(Collection<cdoDummyWorkData> aclbPersonWorkHistory)
        {
            DataTable ldtbResult = null;
            ldtbResult = busBase.Select("cdoBenefitApplication.GetBridgingHoursInfo", new object[1] { this.ibusPerson.icdoPerson.person_id });
            bool lblnFlag = false;
            bool lblnPosition = false;

            if (ldtbResult.Rows.Count > 0)
            {
                foreach (DataRow dt in ldtbResult.Rows)
                {
                    lblnFlag = false;
                    lblnPosition = false;

                    foreach (cdoDummyWorkData lobjDummyWorkData in aclbPersonWorkHistory)
                    {
                        if (lobjDummyWorkData.year == Convert.ToInt32(dt[0]))
                        {
                            //Ticket#85664
                            if (Convert.ToString(dt[2]) == "Disability" || Convert.ToString(dt[2]) == "Covid Hardship" || Convert.ToString(dt[2]) == "MOA 2024")
                            {
                                if (Convert.ToDecimal(dt[1]) == 200)
                                {
                                    lobjDummyWorkData.vested_hours = Convert.ToDecimal(dt[1]);
                                    lobjDummyWorkData.iblnBridgeServiceFlag = true;
                                }
                                else
                                {
                                    lobjDummyWorkData.vested_hours = lobjDummyWorkData.vested_hours + Convert.ToDecimal(dt[1]);
                                }
                            }
                            else
                            {
                                lobjDummyWorkData.vested_hours = lobjDummyWorkData.vested_hours + Convert.ToDecimal(dt[1]);

                            }

                            lobjDummyWorkData.comments += busConstant.BRIDGED_SERVICE + "(" + Convert.ToString(dt[2]) + ")";
                           
                            lblnFlag = true;
                            break;
                        }
                    }

                    if (!lblnFlag && (dt[1] != DBNull.Value && Convert.ToDecimal(dt[1]) != Convert.ToDecimal(0)))
                    {
                        cdoDummyWorkData lcdoDummyWorkData = new cdoDummyWorkData();
                        lcdoDummyWorkData.year = Convert.ToInt32(dt[0]);
                        lcdoDummyWorkData.qualified_hours = 0.0m;
                        lcdoDummyWorkData.vested_hours = Convert.ToDecimal(dt[1]);
                        lcdoDummyWorkData.comments += busConstant.BRIDGED_SERVICE + "(" + Convert.ToString(dt[2]) + ")";
                        //Ticket#85664
                        if (Convert.ToString(dt[2]) == "Disability" || Convert.ToString(dt[2]) == "Covid Hardship" || Convert.ToString(dt[2]) == "MOA 2024")
                        {
                            lcdoDummyWorkData.iblnBridgeServiceFlag = true;
                        }
                            aclbPersonWorkHistory.Add(lcdoDummyWorkData);
                        aclbPersonWorkHistory = aclbPersonWorkHistory.OrderBy(item => item.year).ToList().ToCollection();
                    }
                }
            }
            return aclbPersonWorkHistory;
        }

        private bool CheckIfForfeited(int aintPersonAccountId)
        {
            if (aintPersonAccountId > 0)
            {
                DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { aintPersonAccountId });
                if (ldtbPersonAccountEligibility.Rows.Count > 0)
                {
                    busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                    if (lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.IsNotNull() && lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date != DateTime.MinValue && (lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.IsNull() || lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date == DateTime.MinValue))
                    {
                        return true;
                    }
                }

            }
            return false;

        }

        public void ProcessWorkHistoryforBISandForfieture(Collection<cdoDummyWorkData> aclbPersonWorkHistory, string astrPlanCode, string astrPersonAccStatus = "", int aintEndofWorkYear = 0, bool ablnFromService = false)  //parameter added For CRM Bug 9922
        {
            //Algorithm to Scan thru each year and run counters for BIS and Fofieture            
            int aintTotalQualifiedYearsCount = 0;
            int aintTotalAnniversaryYearsCount = 0;
            int aintTotalVestedYearsCount = 0;
            string istrIsBISParticipant = busConstant.FLAG_NO;
            string istrIsBISParticipantTemp = busConstant.FLAG_NO; //PIR 552
            string lstrIsForfieture = busConstant.FLAG_NO;
            int lintTotalHealthYearsCount = 0;
            int BISCounter = 0;
            bool lblnBadWorkYear = false;
            int lintNonQualifiedWorkYearCounter = 0;
            //PIR 1041
            bool lblnHoursFlag = false;
            //BIS_2024_Update
            //defining local variable to check 2022 status to handle 2023 special scenario for Qualified year and BIS rule.
            bool lbln2022YearExists = false;
            int lint2022QualifiedYearCount = 0;
            int lint2022NonQualifiedYearCount = 0;
            int lint2022BISYearCount = 0;
            decimal ld2022VestedHours = 0.0m;
            decimal ld2023VestedHours = 0.0m;

            //double ldblTotalDays = busGlobalFunctions.GetLastDateOfComputationYear(aclbPersonWorkHistory.First().year).Subtract(this.ibusPerson.icdoPerson.idtDateofBirth).TotalDays;
            //decimal Age_Counter = Math.Round(Convert.ToDecimal(ldblTotalDays * 0.00273790), 4);
            //int lintYears, lintMonths, lintDays;
            //busGlobalFunctions.GetDetailTimeSpan(this.ibusPerson.icdoPerson.idtDateofBirth, busGlobalFunctions.GetLastDateOfComputationYear(aclbPersonWorkHistory.First().year), out lintYears, out lintMonths, out lintDays);
            //decimal Age_Counter = Math.Round(Convert.ToDecimal(lintYears) + Convert.ToDecimal(lintMonths) / 12, 4);
            if (!aclbPersonWorkHistory.IsNullOrEmpty() && aclbPersonWorkHistory.Count > 0)
            {
                DateTime ldtComputationYearEndDate = busGlobalFunctions.GetLastDateOfComputationYear(aclbPersonWorkHistory.First().year);

                //PIR 355
                decimal Age_Counter = 0;
                if (this.ibusPerson.icdoPerson.idtDateofBirth != DateTime.MinValue)
                    Age_Counter = (ldtComputationYearEndDate.Year - this.ibusPerson.icdoPerson.idtDateofBirth.Year) + Math.Round((ldtComputationYearEndDate.Month - this.ibusPerson.icdoPerson.idtDateofBirth.Month) / 12m, 4);

                int lintWithDrawalYearBefore1976 = 0;
                DateTime ldteWithDrawalDate = DateTime.MinValue;
                DataTable ldtbWithdrawalBefore1976 = new DataTable();
                if (!string.IsNullOrEmpty(astrPlanCode))
                    ldtbWithdrawalBefore1976 = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawalBefore1976", new object[2] { this.ibusPerson.icdoPerson.person_id, astrPlanCode });

                //if(ldtbWithdrawalBefore1976.IsNotNull() && ldtbWithdrawalBefore1976.Rows.Count > 0)
                //lintWithDrawalYearBefore1976 = Convert.ToDateTime(ldtbWithdrawalBefore1976.Rows[0][0].ToString()).Year;


                int index = 0;
                int first_year = 0;
                int lintYearMPIPlanBegins = 0;


                if (aclbPersonWorkHistory.Where(item => item.qualified_hours > 0).Count() > 0)
                    lintYearMPIPlanBegins = aclbPersonWorkHistory.Where(item => item.qualified_hours > 0).First().year;

                ArrayList items_to_remove = new ArrayList();
                #region Algorithm to Make-Up the in-memory WorkHistory Table
                foreach (cdoDummyWorkData item in aclbPersonWorkHistory)
                {
                    item.istrBisParticipantFlag = string.Empty;
                    if (ldtbWithdrawalBefore1976.IsNotNull() && ldtbWithdrawalBefore1976.Rows.Count > 0)
                    {
                        if (ldtbWithdrawalBefore1976.AsEnumerable().Where(r => Convert.ToInt32(r["WITHDRAWAL_YEAR"]) == item.year).Any())
                        {
                            ldteWithDrawalDate = Convert.ToDateTime(ldtbWithdrawalBefore1976.AsEnumerable().Where(r => Convert.ToInt32(r["WITHDRAWAL_YEAR"]) == item.year).AsDataTable().Rows[0]["WITHDRAWAL_DATE"]);
                            lintWithDrawalYearBefore1976 = Convert.ToInt32(ldtbWithdrawalBefore1976.AsEnumerable().Where(r => Convert.ToInt32(r["WITHDRAWAL_YEAR"]) == item.year).AsDataTable().Rows[0]["WITHDRAWAL_YEAR"]);
                        }
                        // lstrIsForfieture = busConstant.FLAG_YES;
                    }

                    item.iblnWithdrawalReset = false;

                    //Process Health years Count
                    if (item.idecTotalHealthHours >= 400)
                    {
                        lintTotalHealthYearsCount++;
                        item.iintHealthCount = lintTotalHealthYearsCount;
                    }
                    else
                    {
                        item.iintHealthCount = lintTotalHealthYearsCount;
                    }

                    //if (this.icdoBenefitApplication.retirement_date != DateTime.MinValue &&
                    //            this.icdoBenefitApplication.retirement_date.Year != item.year)
                    //{
                    item.age = Age_Counter++;
                    item.iintAgetoShow = Math.Floor(item.age); //TO MAP AGE TO THIS COLLECTION AS WELL
                    //}


                    #region Commented Code
                    // If DateTime.Now.Year is not Equal to Item.Year and DateTime.Today != Last Date of Computation Year for item.year
                    //  DateTime.Today >// End Date of this Computation Year
                    //if (DateTime.Now.Year == item.year)
                    //{
                    //    DateTime SatBeforeLastThrusday = busGlobalFunctions.GetLastDateOfComputationYear(DateTime.Now.Year);

                    //    if (DateTime.Now <= SatBeforeLastThrusday)
                    //    {
                    //        items_to_remove.Add(item.year);
                    //        if (astrPlanCode == busConstant.MPIPP && ablnReInitWD4CalcCollection)
                    //        {
                    //            item.qualified_years_count = aclbPersonWorkHistory.ElementAt((aclbPersonWorkHistory.IndexOf(item)) - 1).qualified_years_count;
                    //            item.vested_years_count = item.qualified_years_count;
                    //            this.iclbWorkData4RetirementYearMPIPP.Add(item);
                    //        }
                    //        continue;
                    //    }
                    //}
                    #endregion

                    if (item.vested_hours != 0)
                    {
                        lstrIsForfieture = busConstant.FLAG_NO;
                    }


                    if (lstrIsForfieture == busConstant.FLAG_NO && first_year != 0)
                    {
                        item.anniversary_years_count = ++aintTotalAnniversaryYearsCount; // To update the in-memory collection with current counters                   
                    }

                    if (lstrIsForfieture == busConstant.FLAG_NO && item.vested_hours > 1 && first_year == 0)
                    {
                        first_year++;
                    }
                    #region Local Merge ( tusharchandak )
                    //PIR 753 //PIR 832
                    //if (iobjPassInfo.istrFormName == "wfmAnnualBenefitSummaryOverviewMaintenance" || iobjPassInfo.istrFormName == "wfmPersonOverviewMaintenance" ||
                    //  iobjPassInfo.istrFormName == string.Empty || iobjPassInfo.istrFormName == null)//Rashmi-for pir-565(use following code in case of Person_Overview Screen,AnnualBenefitSummaryOverview Screen and in case of batches)
                    //{
                    int iintLocalQualifiedYearsCount = 0;
                    if (item.year == (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_700).Year)
                    {
                        busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();
                        if (this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0)
                        {
                            lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id);
                            if (lbusLocalPersonAccountEligibility != null)
                            {
                                iintLocalQualifiedYearsCount = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                if (iobjPassInfo.istrFormName != busConstant.ANNAUL_BENEFIT_SUMMARY_OVERVIEW_MAINTENANCE && iobjPassInfo.istrFormName != busConstant.PERSON_OVERVIEW_MAINTENANCE && iobjPassInfo.istrFormName != busConstant.BENEFIT_CALCULATION_RETIREMENT_MAINTENACE
                                    && iobjPassInfo.istrFormName != busConstant.MSS.ABS_FORM && iobjPassInfo.istrFormName != busConstant.MSS.PLAN_SUMMARY_FORM
                                    && !ablnFromService  //For CRM Bug 9922
                                    )//PIR 1024
                                    item.idecTotalHealthHours += lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours; //PIR 1024
                                    item.L700_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                            }
                            aintTotalQualifiedYearsCount = aintTotalQualifiedYearsCount + iintLocalQualifiedYearsCount;
                            aintTotalVestedYearsCount = aintTotalVestedYearsCount + iintLocalQualifiedYearsCount;
                            lintTotalHealthYearsCount += iintLocalQualifiedYearsCount; //PIR 832
                            item.iintHealthCount += iintLocalQualifiedYearsCount; //PIR 832
                            //PIR 753
                            //item.L700_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                        }
                    }
                    if (item.year == (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_600).Year)
                    {
                        busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();
                        if (this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0)
                        {
                            lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id);
                            if (lbusLocalPersonAccountEligibility != null)
                            {
                                iintLocalQualifiedYearsCount = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                if (iobjPassInfo.istrFormName != busConstant.ANNAUL_BENEFIT_SUMMARY_OVERVIEW_MAINTENANCE && iobjPassInfo.istrFormName != busConstant.PERSON_OVERVIEW_MAINTENANCE && iobjPassInfo.istrFormName != busConstant.BENEFIT_CALCULATION_RETIREMENT_MAINTENACE
                                    && iobjPassInfo.istrFormName != busConstant.MSS.ABS_FORM && iobjPassInfo.istrFormName != busConstant.MSS.PLAN_SUMMARY_FORM
                                    && !ablnFromService  //For CRM Bug 9922
                                    )//PIR 1024
                                    item.idecTotalHealthHours += lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours; //PIR 1024
                                    item.L600_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                            }
                            aintTotalQualifiedYearsCount = aintTotalQualifiedYearsCount + iintLocalQualifiedYearsCount;
                            aintTotalVestedYearsCount = aintTotalVestedYearsCount + iintLocalQualifiedYearsCount;
                            lintTotalHealthYearsCount += iintLocalQualifiedYearsCount; //PIR 832
                            item.iintHealthCount += iintLocalQualifiedYearsCount; //PIR 832
                            //PIR 753
                            //item.L600_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                        }
                    }
                    if (item.year == (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_666).Year)
                    {
                        busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();
                        if (this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0)
                        {
                            lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id);
                            if (lbusLocalPersonAccountEligibility != null)
                            {
                                iintLocalQualifiedYearsCount = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                if (iobjPassInfo.istrFormName != busConstant.ANNAUL_BENEFIT_SUMMARY_OVERVIEW_MAINTENANCE && iobjPassInfo.istrFormName != busConstant.PERSON_OVERVIEW_MAINTENANCE && iobjPassInfo.istrFormName != busConstant.BENEFIT_CALCULATION_RETIREMENT_MAINTENACE
                                    && iobjPassInfo.istrFormName != busConstant.MSS.ABS_FORM && iobjPassInfo.istrFormName != busConstant.MSS.PLAN_SUMMARY_FORM
                                    && !ablnFromService  //For CRM Bug 9922
                                    )//PIR 1024
                                    item.idecTotalHealthHours += lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours; //PIR 1024
                                    item.L666_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                            }
                            aintTotalQualifiedYearsCount = aintTotalQualifiedYearsCount + iintLocalQualifiedYearsCount;
                            aintTotalVestedYearsCount = aintTotalVestedYearsCount + iintLocalQualifiedYearsCount;
                            lintTotalHealthYearsCount += iintLocalQualifiedYearsCount; //PIR 832
                            item.iintHealthCount += iintLocalQualifiedYearsCount; //PIR 832
                            //PIR 753
                           // item.L666_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                        }
                    }
                    if (item.year == (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_161).Year)
                    {
                        busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();
                        if (this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0)
                        {
                            lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id);
                            if (lbusLocalPersonAccountEligibility != null)
                            {
                                iintLocalQualifiedYearsCount = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                if (iobjPassInfo.istrFormName != busConstant.ANNAUL_BENEFIT_SUMMARY_OVERVIEW_MAINTENANCE && iobjPassInfo.istrFormName != busConstant.PERSON_OVERVIEW_MAINTENANCE && iobjPassInfo.istrFormName != busConstant.BENEFIT_CALCULATION_RETIREMENT_MAINTENACE
                                    && iobjPassInfo.istrFormName != busConstant.MSS.ABS_FORM && iobjPassInfo.istrFormName != busConstant.MSS.PLAN_SUMMARY_FORM
                                    && !ablnFromService  //For CRM Bug 9922
                                    )//PIR 1024
                                    item.idecTotalHealthHours += lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours; //PIR 1024
                                    item.L161_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                            }
                            aintTotalQualifiedYearsCount = aintTotalQualifiedYearsCount + iintLocalQualifiedYearsCount;
                            aintTotalVestedYearsCount = aintTotalVestedYearsCount + iintLocalQualifiedYearsCount;
                            lintTotalHealthYearsCount += iintLocalQualifiedYearsCount; //PIR 832
                            item.iintHealthCount += iintLocalQualifiedYearsCount; //PIR 832
                            //PIR 753
                            //item.L161_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                        }
                    }
                    if (item.year == (busConstant.BenefitCalculation.MERGER_DATE_LOCAL_52).Year)
                    {
                        busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility();
                        if (this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0)
                        {
                            lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id);
                            if (lbusLocalPersonAccountEligibility != null)
                            {
                                iintLocalQualifiedYearsCount = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years;
                                if (iobjPassInfo.istrFormName != busConstant.ANNAUL_BENEFIT_SUMMARY_OVERVIEW_MAINTENANCE && iobjPassInfo.istrFormName != busConstant.PERSON_OVERVIEW_MAINTENANCE && iobjPassInfo.istrFormName != busConstant.BENEFIT_CALCULATION_RETIREMENT_MAINTENACE
                                    && iobjPassInfo.istrFormName != busConstant.MSS.ABS_FORM && iobjPassInfo.istrFormName != busConstant.MSS.PLAN_SUMMARY_FORM
                                    && !ablnFromService  //For CRM Bug 9922
                                    )//PIR 1024
                                    item.idecTotalHealthHours += lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours; //PIR 1024
                                    item.L52_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                            }
                            aintTotalQualifiedYearsCount = aintTotalQualifiedYearsCount + iintLocalQualifiedYearsCount;
                            aintTotalVestedYearsCount = aintTotalVestedYearsCount + iintLocalQualifiedYearsCount;
                            lintTotalHealthYearsCount += iintLocalQualifiedYearsCount; //PIR 832
                            item.iintHealthCount += iintLocalQualifiedYearsCount; //PIR 832
                            //PIR 753
                            //item.L52_Hours = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours;
                        }
                    }
                    //} //PIR 753
                    #endregion
                    //PIR 1041
                    if (this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode != busConstant.MPIPP &&
                            account.icdoPersonAccount.istrPlanCode != busConstant.IAP).Count() > 0 &&
                        this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode != busConstant.MPIPP &&
                            account.icdoPersonAccount.istrPlanCode != busConstant.IAP).OrderBy(t => t.icdoPersonAccount.idtMergerDate).FirstOrDefault().icdoPersonAccount.idtMergerDate.Year > item.year
                        && !lblnHoursFlag && item.vested_hours <= 0)
                    {
                        continue;
                    }
                    else
                    {
                        lblnHoursFlag = true;
                    }

                    if (item.qualified_hours >= 400) //PIR 753 ||
                    //(item.L700_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id)) ||
                    //(item.L600_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id)) ||
                    //(item.L666_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id)) ||
                    //(item.L161_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id)) ||
                    //(item.L52_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id)))
                    {

                        aintTotalQualifiedYearsCount++;
                        item.qualified_years_count = aintTotalQualifiedYearsCount;
                        item.istrForfietureFlag = busConstant.FLAG_NO;
                        istrIsBISParticipant = busConstant.FLAG_NO;
                        istrIsBISParticipantTemp = busConstant.FLAG_NO;//PIR 552
                        item.istrBisParticipantFlag = busConstant.FLAG_NO;
                        lintNonQualifiedWorkYearCounter = 0;
                    }

                    if ((this.icdoBenefitApplication.retirement_date == DateTime.MinValue || this.icdoBenefitApplication.retirement_date.Year >= 2023) && item.year == 2023 && astrPlanCode == busConstant.MPIPP && (item.qualified_hours >= 65 && item.qualified_hours < 400))
                    {
                        aintTotalQualifiedYearsCount++;
                        item.qualified_years_count = aintTotalQualifiedYearsCount;
                        //item.istrForfietureFlag = busConstant.FLAG_NO;
                        //istrIsBISParticipant = busConstant.FLAG_NO;
                        //istrIsBISParticipantTemp = busConstant.FLAG_NO;//PIR 552
                        //item.istrBisParticipantFlag = busConstant.FLAG_NO;
                        //lintNonQualifiedWorkYearCounter = 0;
                    }

                    if (item.vested_hours >= 200) //PIR 753 ||
                    // (item.L700_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id)) ||
                    //(item.L600_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id)) ||
                    //(item.L666_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id)) ||
                    //(item.L161_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id)) ||
                    //(item.L52_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id)))
                    {

                        if ((item.vested_hours >= 400) || (item.vested_hours == 200 && item.iblnBridgeServiceFlag == true)) //PIR 753 ||
                        //(item.L700_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L600_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L666_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L161_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id)) ||
                        //(item.L52_Hours >= 400 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id)))
                        {
                            //Ticket#85664
                            if (item.vested_hours >= 400)
                            {
                                aintTotalVestedYearsCount++;
                            }
                            
                            item.vested_years_count = aintTotalVestedYearsCount;
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                            item.istrForfietureFlag = busConstant.FLAG_NO;
                            lintNonQualifiedWorkYearCounter = 0;
                        }
                        else
                        {
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                            item.vested_years_count = aintTotalVestedYearsCount;
                        }

                        //item.bis_years_count = 0;
                        if (item.vested_hours >= 200)
                            BISCounter = 0;
                    }
                    //PIR 1052
                    else
                    {
                        item.qualified_years_count = aintTotalQualifiedYearsCount;
                        item.vested_years_count = aintTotalVestedYearsCount;
                    }


                    //PIR 856
                    //PIR-552(For Forfeiture Rule)
                    //PIR 970
                    if (lintNonQualifiedWorkYearCounter == 0 && item.vested_hours < 400 &&
                        ((lstrIsForfieture == busConstant.FLAG_YES && item.vested_hours != 0) || (lstrIsForfieture == busConstant.FLAG_NO)))
                    {
                        //Ticket#85664
                        if (item.iblnBridgeServiceFlag == true && item.vested_hours == 200)
                        {
                            lintNonQualifiedWorkYearCounter = 0;
                        }
                        else
                        {
                            lintNonQualifiedWorkYearCounter++;

                        }
                        
                    }
                    else if (lintNonQualifiedWorkYearCounter == 1 && item.vested_hours < 400)
                    {
                        //Ticket#85664
                        if (item.iblnBridgeServiceFlag == true && item.vested_hours == 200)
                        {
                            lintNonQualifiedWorkYearCounter = 0;
                        }
                        else
                        {
                            lintNonQualifiedWorkYearCounter++;

                        }
                    }
                    else if (lintNonQualifiedWorkYearCounter > 1 &&
                        item.vested_hours < 400 //&& item.vested_hours >= 200 
                        && item.year >= lintYearMPIPlanBegins && lstrIsForfieture == busConstant.FLAG_NO)
                    {
                        //Ticket#85664
                        if (item.iblnBridgeServiceFlag == true && item.vested_hours == 200)
                        {
                            lintNonQualifiedWorkYearCounter = 0;
                        }
                        else
                        {
                            lintNonQualifiedWorkYearCounter++;

                        }
                    }
                    else
                        lintNonQualifiedWorkYearCounter = 0;

                    //if ((this.icdoBenefitApplication.retirement_date == DateTime.MinValue || this.icdoBenefitApplication.retirement_date.Year >= 2023) && item.year == 2023 && astrPlanCode == busConstant.MPIPP && (item.qualified_hours >= 65 && item.qualified_hours < 400))
                    //{
                    //    lintNonQualifiedWorkYearCounter = 0;

                    //}


                        //if (item.vested_hours < 400 //&& item.vested_hours >= 200 
                        //    && item.year >= lintYearMPIPlanBegins && lstrIsForfieture == busConstant.FLAG_NO)
                        //    lintNonQualifiedWorkYearCounter++;

                    if (item.vested_hours < 400 && item.year >= lintYearMPIPlanBegins && istrIsBISParticipantTemp == busConstant.FLAG_YES)//PIR 552
                        item.istrBisParticipantFlag = busConstant.FLAG_YES;

                    if (item.vested_hours < 200 && (astrPersonAccStatus != busConstant.PERSON_ACCOUNT_STATUS_RETIRED || astrPersonAccStatus != busConstant.PERSON_ACCOUNT_STATUS_DECEASED) 
                        && (item.year < aintEndofWorkYear || aintEndofWorkYear == 0 || aintEndofWorkYear == 1) 
                        && lstrIsForfieture == busConstant.FLAG_NO && item.year >= lintYearMPIPlanBegins && lintYearMPIPlanBegins != 0)
                        lblnBadWorkYear = true;
                    else
                        lblnBadWorkYear = false;

                    if ((lblnBadWorkYear || (lintWithDrawalYearBefore1976 > 0 && lintWithDrawalYearBefore1976 == item.year) || lintNonQualifiedWorkYearCounter > 0) && lstrIsForfieture == busConstant.FLAG_NO)
                    {

                        if (this.icdoBenefitApplication.retirement_date != DateTime.MinValue && this.icdoBenefitApplication.retirement_date.Year == item.year && lblnBadWorkYear
                        && this.icdoBenefitApplication.retirement_date.Year >= DateTime.Now.Year) //PIR 1058
                        {
                            BISCounter++;
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                            item.vested_years_count = aintTotalVestedYearsCount;

                            item.iintNonQualifiedYears = lintNonQualifiedWorkYearCounter; //PIR 970

                            if (BISCounter >= 2)
                            {
                                if (item.comments.IsNotNullOrEmpty())
                                    item.comments += "," + "(NO) BIS SINCE COMPUTATION YEAR HAS NOT ENDED";
                                else
                                    item.comments += "(NO) BIS SINCE COMPUTATION YEAR HAS NOT ENDED";
                            }

                            BISCounter--;

                            continue;
                        }

                        if (lblnBadWorkYear)
                        {
                            //Additional Check to See the Locals as well. // Tricky Scenario at the Merger of Local Plan with the MPI plan.
                            if (astrPlanCode.IsNotNullOrEmpty() && item.year != aclbPersonWorkHistory.First().year &&
                                ((aclbPersonWorkHistory.ElementAt(aclbPersonWorkHistory.IndexOf(item) - 1).L700_Hours + item.vested_hours > 200 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.person_account_id) && item.year >= this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.idtMergerDate.Year) ||
                                    (aclbPersonWorkHistory.ElementAt(aclbPersonWorkHistory.IndexOf(item) - 1).L600_Hours + item.vested_hours > 200 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.person_account_id) && item.year >= this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.idtMergerDate.Year) ||
                                    (aclbPersonWorkHistory.ElementAt(aclbPersonWorkHistory.IndexOf(item) - 1).L666_Hours + item.vested_hours > 200 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.person_account_id) && item.year >= this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.idtMergerDate.Year) ||
                                    (aclbPersonWorkHistory.ElementAt(aclbPersonWorkHistory.IndexOf(item) - 1).L161_Hours + item.vested_hours > 200 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.person_account_id) && item.year >= this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.idtMergerDate.Year) ||
                                    (aclbPersonWorkHistory.ElementAt(aclbPersonWorkHistory.IndexOf(item) - 1).L52_Hours + item.vested_hours > 200 && this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0 && !CheckIfForfeited(this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.person_account_id) && item.year >= this.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.Local_52).First().icdoPersonAccount.idtMergerDate.Year)
                                )
                                )
                            { }
                            else { BISCounter++; }

                            //BIS_2024_Update
                            if (item.year == 2023 && lbln2022YearExists && lint2022BISYearCount < 2 )  //not already in BIS in 2022, disregarding 2023 for BIS.
                            {
                                BISCounter--;
                                if(lintNonQualifiedWorkYearCounter > 0)
                                    lintNonQualifiedWorkYearCounter--;   //Now 2023 will not be counted towards non-qualified years count for forfieture.
                            }

                            if (item.year == 2024 && item.vested_hours < 200 && ld2023VestedHours < 200 && ld2022VestedHours >= 400) //5_21_2025 revised rule by Gary
                            {
                                BISCounter--;
                            }

                            if (item.year == 2024 && lbln2022YearExists && lint2022BISYearCount == 1 && ld2023VestedHours < 200 && ld2022VestedHours < 200)
                            {
                                BISCounter = lint2022BISYearCount + 1;
                            }

                            item.bis_years_count = BISCounter;
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                            item.vested_years_count = aintTotalVestedYearsCount;

                            if (BISCounter >= 2)
                            {
                                //istrIsBISParticipant = busConstant.FLAG_YES;
                                istrIsBISParticipantTemp = busConstant.FLAG_YES;//PIR 552
                                //item.istrBisParticipantFlag = busConstant.FLAG_YES;

                                //if (item.comments.IsNotNullOrEmpty())
                                //    item.comments += "," + busConstant.BIS_PARTICIPANT + "(MPI_Plan)";
                                //else
                                //    item.comments += busConstant.BIS_PARTICIPANT + "(MPI_Plan)";
                            }

                        }

                        //PIR 970
                        if (istrIsBISParticipantTemp == busConstant.FLAG_YES && lintNonQualifiedWorkYearCounter >= 2)
                        {
                            istrIsBISParticipant = busConstant.FLAG_YES;
                            istrIsBISParticipantTemp = busConstant.FLAG_YES;//PIR 552
                            item.istrBisParticipantFlag = busConstant.FLAG_YES;

                            if (item.comments.IsNotNullOrEmpty())
                                item.comments += "," + busConstant.BIS_PARTICIPANT + "(MPI_Plan)";
                            else
                                item.comments += busConstant.BIS_PARTICIPANT + "(MPI_Plan)";
                        }

                        if (lstrIsForfieture == busConstant.FLAG_NO)
                        {
                            if (lintNonQualifiedWorkYearCounter >= 2 && item.year < 1976 && istrIsBISParticipant == busConstant.FLAG_YES)
                            {
                                //Prod PIR 10,56
                                #region Process Forfeiture
                                aintTotalQualifiedYearsCount = 0;
                                aintTotalAnniversaryYearsCount = 0;
                                aintTotalVestedYearsCount = 0;
                                BISCounter = 0;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                first_year = 0;
                                lintNonQualifiedWorkYearCounter = 0;

                                item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                                item.qualified_years_count = aintTotalQualifiedYearsCount;
                                item.vested_years_count = aintTotalVestedYearsCount;
                                item.istrForfietureFlag = busConstant.FLAG_YES;

                                if (astrPlanCode == busConstant.MPIPP)
                                    ForfeitureYearsCollectionMPI.Add(item.year);
                                else if (astrPlanCode == busConstant.IAP)
                                    ForfeitureYearsCollectionIAP.Add(item.year);
                                item.comments += busConstant.FORFIETURE;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                item.istrForfietureFlag = busConstant.FLAG_YES;

                                //PIR-552
                                istrIsBISParticipant = busConstant.FLAG_NO;
                                istrIsBISParticipantTemp = busConstant.FLAG_NO; //Forfieture Vesting Logic Fixes //PIR 1043//RID
                                //item.istrBisParticipantFlag = busConstant.FLAG_NO;//PIR 552
                                #endregion
                            }

                            else if (lintWithDrawalYearBefore1976 > 0 && lintWithDrawalYearBefore1976 == item.year)
                            {
                                #region Process Forfieture
                                int lintPlan = 0;
                                if (astrPlanCode == busConstant.MPIPP)
                                    lintPlan = busConstant.MPIPP_PLAN_ID;
                                else if (astrPlanCode == busConstant.IAP)
                                    lintPlan = busConstant.MPIPP_PLAN_ID; //PIR 753
                                decimal ldecHoursAfterWithdrawal = decimal.Zero;
                                busCalculation lbusCalculation = new busCalculation();
                                DateTime ldtToDate = new DateTime(item.year, 12, 31);
                                ldecHoursAfterWithdrawal = lbusCalculation.GetWorkDataAfterDate(this.ibusPerson.icdoPerson.istrSSNNonEncrypted, item.year, lintPlan, ldteWithDrawalDate);
                                //Mss
                                item.Withdrawal_Hours = item.qualified_hours - ldecHoursAfterWithdrawal;
                                if (ldecHoursAfterWithdrawal >= 400)
                                {
                                    item.iblnWithdrawalReset = true;
                                    aintTotalQualifiedYearsCount = 1;
                                    aintTotalAnniversaryYearsCount = 0;
                                    aintTotalVestedYearsCount = 1;
                                    BISCounter = 0;
                                    first_year = 0;
                                    lintNonQualifiedWorkYearCounter = 0;

                                    item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                                    item.qualified_years_count = aintTotalQualifiedYearsCount;
                                    item.vested_years_count = aintTotalVestedYearsCount;
                                }
                                else
                                {
                                    item.iblnWithdrawalReset = true;
                                    aintTotalQualifiedYearsCount = 0;
                                    aintTotalAnniversaryYearsCount = 0;
                                    aintTotalVestedYearsCount = 0;
                                    BISCounter = 0;
                                    lstrIsForfieture = busConstant.FLAG_YES;
                                    first_year = 0;
                                    lintNonQualifiedWorkYearCounter = 0;

                                    item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                                    item.qualified_years_count = aintTotalQualifiedYearsCount;
                                    item.vested_years_count = aintTotalVestedYearsCount;
                                    item.istrForfietureFlag = busConstant.FLAG_YES;

                                    if (astrPlanCode == busConstant.MPIPP)
                                        ForfeitureYearsCollectionMPI.Add(item.year);
                                    else if (astrPlanCode == busConstant.IAP)
                                        ForfeitureYearsCollectionIAP.Add(item.year);

                                    item.comments += busConstant.FORFIETURE;
                                    lstrIsForfieture = busConstant.FLAG_YES;
                                    item.istrForfietureFlag = busConstant.FLAG_YES;

                                    //PIR-552
                                    istrIsBISParticipant = busConstant.FLAG_NO;
                                    istrIsBISParticipantTemp = busConstant.FLAG_NO; //Forfieture Vesting Logic Fixes //PIR 1043//RID
                                    //item.istrBisParticipantFlag = busConstant.FLAG_NO;//PIR 552
                                }
                                #endregion
                            }

                            else if (lintNonQualifiedWorkYearCounter >= aintTotalVestedYearsCount && item.year <= 1985 && item.year >= 1976 && istrIsBISParticipant == busConstant.FLAG_YES) //&& BISCounter >= 2) PIR-552(Commenting last condition - for forfeiture , participant should be BISParticipant , no need to check BISCounter >=2)
                            {
                                #region Process Forfeiture
                                aintTotalQualifiedYearsCount = 0;
                                aintTotalAnniversaryYearsCount = 0;
                                aintTotalVestedYearsCount = 0;
                                BISCounter = 0;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                first_year = 0;
                                lintNonQualifiedWorkYearCounter = 0;

                                item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                                item.qualified_years_count = aintTotalQualifiedYearsCount;
                                item.vested_years_count = aintTotalVestedYearsCount;
                                item.istrForfietureFlag = busConstant.FLAG_YES;

                                if (astrPlanCode == busConstant.MPIPP)
                                    ForfeitureYearsCollectionMPI.Add(item.year);
                                else if (astrPlanCode == busConstant.IAP)
                                    ForfeitureYearsCollectionIAP.Add(item.year);
                                item.comments += busConstant.FORFIETURE;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                item.istrForfietureFlag = busConstant.FLAG_YES;

                                //PIR-552
                                istrIsBISParticipant = busConstant.FLAG_NO;
                                istrIsBISParticipantTemp = busConstant.FLAG_NO; //Forfieture Vesting Logic Fixes //PIR 1043//RID
                                //item.istrBisParticipantFlag = busConstant.FLAG_NO;//PIR 552
                                #endregion
                            }

                            else if (aintTotalVestedYearsCount >= 5 && lintNonQualifiedWorkYearCounter >= aintTotalVestedYearsCount && item.year >= 1986 && istrIsBISParticipant == busConstant.FLAG_YES)
                            {
                                #region Process Forfeiture
                                aintTotalQualifiedYearsCount = 0;
                                aintTotalAnniversaryYearsCount = 0;
                                aintTotalVestedYearsCount = 0;
                                BISCounter = 0;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                first_year = 0;
                                lintNonQualifiedWorkYearCounter = 0;

                                item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                                item.qualified_years_count = aintTotalQualifiedYearsCount;
                                item.vested_years_count = aintTotalVestedYearsCount;
                                item.istrForfietureFlag = busConstant.FLAG_YES;

                                if (astrPlanCode == busConstant.MPIPP)
                                    ForfeitureYearsCollectionMPI.Add(item.year);
                                else if (astrPlanCode == busConstant.IAP)
                                    ForfeitureYearsCollectionIAP.Add(item.year);


                                item.comments += busConstant.FORFIETURE;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                item.istrForfietureFlag = busConstant.FLAG_YES;

                                //PIR-552
                                istrIsBISParticipant = busConstant.FLAG_NO;
                                istrIsBISParticipantTemp = busConstant.FLAG_NO; //Forfieture Vesting Logic Fixes //PIR 1043//RID
                                //item.istrBisParticipantFlag = busConstant.FLAG_NO;//PIR 552
                                #endregion
                            }

                            else if (aintTotalVestedYearsCount < 5 && lintNonQualifiedWorkYearCounter >= 5 && item.year >= 1986 && istrIsBISParticipant == busConstant.FLAG_YES)
                            {
                                #region Process Forfeiture
                                aintTotalQualifiedYearsCount = 0;
                                aintTotalAnniversaryYearsCount = 0;
                                aintTotalVestedYearsCount = 0;
                                BISCounter = 0;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                first_year = 0;
                                lintNonQualifiedWorkYearCounter = 0;

                                item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                                item.qualified_years_count = aintTotalQualifiedYearsCount;
                                item.vested_years_count = aintTotalVestedYearsCount;
                                item.istrForfietureFlag = busConstant.FLAG_YES;

                                if (astrPlanCode == busConstant.MPIPP)
                                    ForfeitureYearsCollectionMPI.Add(item.year);
                                else if (astrPlanCode == busConstant.IAP)
                                    ForfeitureYearsCollectionIAP.Add(item.year);


                                item.comments += busConstant.FORFIETURE;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                item.istrForfietureFlag = busConstant.FLAG_YES;

                                //PIR-552
                                istrIsBISParticipant = busConstant.FLAG_NO;
                                istrIsBISParticipantTemp = busConstant.FLAG_NO; //Forfieture Vesting Logic Fixes //PIR 1043//RID
                                //item.istrBisParticipantFlag = busConstant.FLAG_NO;//PIR 552
                                #endregion
                            }
                        }
                        //Rashmi PIR-565(to carry forward Total Qualified years count till the last year if qualified hrs are not reported for any participant)
                        if (aintTotalQualifiedYearsCount > 0 && item.qualified_years_count == 0)
                        {
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                        }
                    }
                    else
                    {
                        item.qualified_years_count = aintTotalQualifiedYearsCount;
                        item.vested_years_count = aintTotalVestedYearsCount;
                        if (lstrIsForfieture == busConstant.FLAG_YES)
                        {
                            item.istrForfietureFlag = busConstant.FLAG_YES;

                            //PIR-552
                            istrIsBISParticipant = busConstant.FLAG_NO;
                            istrIsBISParticipantTemp = busConstant.FLAG_YES; //Forfieture Vesting Logic Fixes //PIR 1043//RID 52909
                            //item.istrBisParticipantFlag = busConstant.FLAG_NO;//PIR 552
                        }

                        // forfeited date added if has any withdrawal before 1976
                        //if (ldtbWithdrawalBefore1976.IsNotNull() && item.year <= 1976)
                        //{
                        //    if (item.vested_years_count > 0 && item.istrForfietureFlag == busConstant.FLAG_YES)
                        //        AddUpdateForfeitureDate(item.year, astrPlanCode, true);
                        //    else
                        //        AddUpdateForfeitureDate(item.year, astrPlanCode, false);
                        //}
                    }
                    item.iintNonQualifiedYears = lintNonQualifiedWorkYearCounter; //PIR 970

                    //BIS_2024_Update
                    if (item.year == 2022)
                    {
                        lbln2022YearExists = true;
                        lint2022QualifiedYearCount = item.qualified_years_count;
                        lint2022NonQualifiedYearCount = item.iintNonQualifiedYears;
                        lint2022BISYearCount = item.bis_years_count;
                        ld2022VestedHours = item.vested_hours;
                    }
                    if (item.year == 2023)
                    {
                        ld2023VestedHours = item.vested_hours;
                    }

                }
                #endregion
            }
        }


        public void ProcessWorkHistorySpeciallyforMPIPreMerger(Collection<cdoDummyWorkData> aclbPersonWorkHistory)
        {
            //Algorithm to Scan thru each year and run counters for BIS and Fofieture            
            int aintTotalQualifiedYearsCount = 0;
            int aintTotalAnniversaryYearsCount = 0;
            int aintTotalVestedYearsCount = 0;
            string istrIsBISParticipant = busConstant.FLAG_NO;
            string istrIsBISParticipantTemp = busConstant.FLAG_NO; //PIR 552
            string lstrIsForfieture = busConstant.FLAG_NO;
            int BISCounter = 0;

            //PIR 355
            double ldblTotalDays = 0;
            if (this.ibusPerson.icdoPerson.idtDateofBirth != DateTime.MinValue)
            {
                ldblTotalDays = busGlobalFunctions.GetLastDateOfComputationYear(aclbPersonWorkHistory.First().year).Subtract(this.ibusPerson.icdoPerson.idtDateofBirth).TotalDays;
            }
            decimal Age_Counter = Math.Round(Convert.ToDecimal(ldblTotalDays * 0.00273790), 4);
            int index = 0;
            bool lblnBadWorkYear = false;
            int lintNonQualifiedWorkYearCounter = 0;
            int first_year = 0;
            int lintWithDrawalYearBefore1976 = 0;
            int lintYearMPIPlanBegins = 0;

            if (aclbPersonWorkHistory.Where(item => item.qualified_hours > 0).Count() > 0)
                lintYearMPIPlanBegins = aclbPersonWorkHistory.Where(item => item.qualified_hours > 0).First().year;

            DataTable ldtbWithdrawalBefore1976 = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawalBefore1976", new object[2] { this.ibusPerson.icdoPerson.person_id, busConstant.MPIPP });
            //if (ldtbWithdrawalBefore1976.IsNotNull() && ldtbWithdrawalBefore1976.Rows.Count > 0)
            //    lintWithDrawalYearBefore1976 = Convert.ToDateTime(ldtbWithdrawalBefore1976.Rows[0][0].ToString()).Year;

            #region Algorithm to Make-Up the in-memory WorkHistory Table
            foreach (cdoDummyWorkData item in aclbPersonWorkHistory)
            {

                if (ldtbWithdrawalBefore1976.IsNotNull() && ldtbWithdrawalBefore1976.Rows.Count > 0)
                {
                    if (ldtbWithdrawalBefore1976.AsEnumerable().Where(r => Convert.ToInt32(r["WITHDRAWAL_YEAR"]) == item.year).Any())
                    {
                        DateTime ldteWithDrawalDate = Convert.ToDateTime(ldtbWithdrawalBefore1976.AsEnumerable().Where(r => Convert.ToInt32(r["WITHDRAWAL_YEAR"]) == item.year).AsDataTable().Rows[0]["WITHDRAWAL_DATE"]);
                        lintWithDrawalYearBefore1976 = Convert.ToInt32(ldtbWithdrawalBefore1976.AsEnumerable().Where(r => Convert.ToInt32(r["WITHDRAWAL_YEAR"]) == item.year).AsDataTable().Rows[0]["WITHDRAWAL_YEAR"]);
                    }
                }

                item.iblnWithdrawalReset = false;

                item.age = Age_Counter++; //TO MAP AGE TO THIS COLLECTION AS WELL

                if (item.vested_hours != 0)
                {
                    lstrIsForfieture = busConstant.FLAG_NO;
                }


                if (lstrIsForfieture == busConstant.FLAG_NO && first_year != 0)
                {
                    item.anniversary_years_count = ++aintTotalAnniversaryYearsCount; // To update the in-memory collection with current counters                   
                }

                if (lstrIsForfieture == busConstant.FLAG_NO)
                {
                    first_year++;
                }

                if (item.qualified_hours >= 400)
                {
                    aintTotalQualifiedYearsCount++;
                    item.qualified_years_count = aintTotalQualifiedYearsCount;
                    istrIsBISParticipant = busConstant.FLAG_NO;
                    istrIsBISParticipantTemp = busConstant.FLAG_NO;//PIR 552
                    item.istrBisParticipantFlag = busConstant.FLAG_NO;
                    lintNonQualifiedWorkYearCounter = 0;
                }

                if (item.vested_hours >= 200)
                {
                    if (item.vested_hours >= 400)
                    {
                        aintTotalVestedYearsCount++;
                        item.vested_years_count = aintTotalVestedYearsCount;
                        item.qualified_years_count = aintTotalQualifiedYearsCount;
                        lintNonQualifiedWorkYearCounter = 0;
                    }
                    else
                    {
                        item.qualified_years_count = aintTotalQualifiedYearsCount;
                        item.vested_years_count = aintTotalVestedYearsCount;
                    }

                    item.bis_years_count = 0;
                    BISCounter = 0;

                }

                //PIR-552(For Forfeiture Rule)
                //PIR 970
                if (lintNonQualifiedWorkYearCounter == 0 && item.vested_hours < 400)
                {
                    //Ticket#85664
                    if (item.iblnBridgeServiceFlag == true && item.vested_hours == 200)
                    {
                        lintNonQualifiedWorkYearCounter = 0;
                    }
                    else
                    {
                        lintNonQualifiedWorkYearCounter++;

                    }
                }
                else if (lintNonQualifiedWorkYearCounter == 1 && item.vested_hours < 400)
                {
                    //Ticket#85664
                    if (item.iblnBridgeServiceFlag == true && item.vested_hours == 200)
                    {
                        lintNonQualifiedWorkYearCounter = 0;
                    }
                    else
                    {
                        lintNonQualifiedWorkYearCounter++;

                    }
                }
                else if (lintNonQualifiedWorkYearCounter > 1 &&
                    item.vested_hours < 400 //&& item.vested_hours >= 200 
                    && item.year >= lintYearMPIPlanBegins && lstrIsForfieture == busConstant.FLAG_NO)
                {
                    //Ticket#85664
                    if (item.iblnBridgeServiceFlag == true && item.vested_hours == 200)
                    {
                        lintNonQualifiedWorkYearCounter = 0;
                    }
                    else
                    {
                        lintNonQualifiedWorkYearCounter++;

                    }
                }
                else
                    lintNonQualifiedWorkYearCounter = 0;

                //if (item.vested_hours < 400 //&& item.vested_hours >= 200 
                //    && item.year >= lintYearMPIPlanBegins && lstrIsForfieture == busConstant.FLAG_NO)
                //    lintNonQualifiedWorkYearCounter++;

                if (item.vested_hours < 400 && item.year >= lintYearMPIPlanBegins && istrIsBISParticipantTemp == busConstant.FLAG_YES)//PIR 552
                    item.istrBisParticipantFlag = busConstant.FLAG_YES;

                if (item.vested_hours < 200 && item.year >= lintYearMPIPlanBegins && lintYearMPIPlanBegins != 0)
                    lblnBadWorkYear = true;
                else
                    lblnBadWorkYear = false;


                if ((lblnBadWorkYear || lintNonQualifiedWorkYearCounter > 0 || (lintWithDrawalYearBefore1976 > 0 && lintWithDrawalYearBefore1976 == item.year)) && lstrIsForfieture == busConstant.FLAG_NO && item.year >= lintYearMPIPlanBegins)
                {
                    if (lblnBadWorkYear)
                    {
                        //Additional Check to See the Locals as well.
                        if (item.vested_hours < 200)
                            BISCounter++;
                        item.bis_years_count = BISCounter;
                        item.qualified_years_count = aintTotalQualifiedYearsCount;
                        item.vested_years_count = aintTotalVestedYearsCount;

                        if (BISCounter >= 2)
                        {
                            //istrIsBISParticipant = busConstant.FLAG_YES;
                            istrIsBISParticipantTemp = busConstant.FLAG_YES;
                            //item.istrBisParticipantFlag = busConstant.FLAG_YES;
                            //if (item.comments.IsNotNullOrEmpty())
                            //    item.comments += "," + busConstant.BIS_PARTICIPANT + "(MPI_PLAN)";
                            //else
                            //    item.comments += busConstant.BIS_PARTICIPANT + "(MPI_PLAN)";
                        }
                    }

                    //PIR 970
                    if (istrIsBISParticipantTemp == busConstant.FLAG_YES && lintNonQualifiedWorkYearCounter >= 2)
                    {
                        istrIsBISParticipant = busConstant.FLAG_YES;
                        istrIsBISParticipantTemp = busConstant.FLAG_YES;//PIR 552
                        item.istrBisParticipantFlag = busConstant.FLAG_YES;

                        if (item.comments.IsNotNullOrEmpty())
                            item.comments += "," + busConstant.BIS_PARTICIPANT + "(MPI_Plan)";
                        else
                            item.comments += busConstant.BIS_PARTICIPANT + "(MPI_Plan)";
                    }

                    if (lstrIsForfieture == busConstant.FLAG_NO)
                    {
                        if (lintNonQualifiedWorkYearCounter >= 2 && item.year < 1976 && istrIsBISParticipant == busConstant.FLAG_YES)
                        {
                            aintTotalQualifiedYearsCount = 0;
                            aintTotalAnniversaryYearsCount = 0;
                            aintTotalVestedYearsCount = 0;
                            BISCounter = 0;
                            lstrIsForfieture = busConstant.FLAG_YES;
                            first_year = 0;
                            lintNonQualifiedWorkYearCounter = 0; //ROHAN

                            item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                            item.vested_years_count = aintTotalVestedYearsCount;
                            item.istrForfietureFlag = busConstant.FLAG_YES;
                            item.comments += busConstant.FORFIETURE;
                            ForfeitureYearsCollectionMPIPreMerger.Add(item.year);

                            //PIR-552
                            istrIsBISParticipant = busConstant.FLAG_NO;
                            istrIsBISParticipantTemp = busConstant.FLAG_NO; //Forfieture Vesting Logic Fixes //PIR 1043//RID
                            //item.istrBisParticipantFlag = busConstant.FLAG_NO;//PIR 552
                        }

                        else if (lintWithDrawalYearBefore1976 > 0 && lintWithDrawalYearBefore1976 == item.year)
                        {
                            if (item.vested_hours >= 400)
                            {
                                aintTotalQualifiedYearsCount = 1;
                                aintTotalAnniversaryYearsCount = 0;
                                aintTotalVestedYearsCount = 1;
                                BISCounter = 0;
                                first_year = 0;
                                item.iblnWithdrawalReset = true;
                                lintNonQualifiedWorkYearCounter = 0; //ROHAN

                                item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                                item.qualified_years_count = aintTotalQualifiedYearsCount;
                                item.vested_years_count = aintTotalVestedYearsCount;
                            }
                            else
                            {
                                item.iblnWithdrawalReset = true;
                                aintTotalQualifiedYearsCount = 0;
                                aintTotalAnniversaryYearsCount = 0;
                                aintTotalVestedYearsCount = 0;
                                BISCounter = 0;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                first_year = 0;
                                lintNonQualifiedWorkYearCounter = 0; //ROHAN

                                item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                                item.qualified_years_count = aintTotalQualifiedYearsCount;
                                item.vested_years_count = aintTotalVestedYearsCount;
                                item.istrForfietureFlag = busConstant.FLAG_YES;
                                item.comments += busConstant.FORFIETURE;
                                ForfeitureYearsCollectionMPIPreMerger.Add(item.year);

                                //PIR-552
                                istrIsBISParticipant = busConstant.FLAG_NO;
                                istrIsBISParticipantTemp = busConstant.FLAG_NO; //Forfieture Vesting Logic Fixes //PIR 1043//RID
                                //item.istrBisParticipantFlag = busConstant.FLAG_NO;//PIR 552
                            }
                        }

                        else if (lintNonQualifiedWorkYearCounter >= aintTotalVestedYearsCount && item.year <= 1985 && item.year >= 1976 && istrIsBISParticipant == busConstant.FLAG_YES) //&& BISCounter >= 2(Commenting last condition - for forfeiture , participant should be BISParticipant , no need to check BISCounter >=2)
                        {
                            aintTotalQualifiedYearsCount = 0;
                            aintTotalAnniversaryYearsCount = 0;
                            aintTotalVestedYearsCount = 0;
                            BISCounter = 0;
                            lstrIsForfieture = busConstant.FLAG_YES;
                            first_year = 0;
                            lintNonQualifiedWorkYearCounter = 0; //ROHAN

                            item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                            item.vested_years_count = aintTotalVestedYearsCount;
                            item.istrForfietureFlag = busConstant.FLAG_YES;
                            item.comments += busConstant.FORFIETURE;
                            ForfeitureYearsCollectionMPIPreMerger.Add(item.year);

                            //PIR-552
                            istrIsBISParticipant = busConstant.FLAG_NO;
                            istrIsBISParticipantTemp = busConstant.FLAG_NO; //Forfieture Vesting Logic Fixes //PIR 1043//RID
                            //item.istrBisParticipantFlag = busConstant.FLAG_NO;//PIR 552
                        }

                        else if (aintTotalVestedYearsCount >= 5 && lintNonQualifiedWorkYearCounter >= aintTotalVestedYearsCount && item.year >= 1986 && istrIsBISParticipant == busConstant.FLAG_YES)
                        {
                            aintTotalQualifiedYearsCount = 0;
                            aintTotalAnniversaryYearsCount = 0;
                            aintTotalVestedYearsCount = 0;
                            BISCounter = 0;
                            lstrIsForfieture = busConstant.FLAG_YES;
                            first_year = 0;
                            lintNonQualifiedWorkYearCounter = 0; //ROHAN

                            item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                            item.vested_years_count = aintTotalVestedYearsCount;
                            item.istrForfietureFlag = busConstant.FLAG_YES;
                            item.comments += busConstant.FORFIETURE;
                            ForfeitureYearsCollectionMPIPreMerger.Add(item.year);

                            //PIR-552
                            istrIsBISParticipant = busConstant.FLAG_NO;
                            istrIsBISParticipantTemp = busConstant.FLAG_NO; //Forfieture Vesting Logic Fixes //PIR 1043//RID
                            //item.istrBisParticipantFlag = busConstant.FLAG_NO;//PIR 552
                        }

                        else if (aintTotalVestedYearsCount < 5 && lintNonQualifiedWorkYearCounter >= 5 && item.year >= 1986 && istrIsBISParticipant == busConstant.FLAG_YES)
                        {
                            aintTotalQualifiedYearsCount = 0;
                            aintTotalAnniversaryYearsCount = 0;
                            aintTotalVestedYearsCount = 0;
                            BISCounter = 0;
                            lstrIsForfieture = busConstant.FLAG_YES;
                            first_year = 0;
                            lintNonQualifiedWorkYearCounter = 0; //ROHAN

                            item.anniversary_years_count = aintTotalAnniversaryYearsCount;
                            item.qualified_years_count = aintTotalQualifiedYearsCount;
                            item.vested_years_count = aintTotalVestedYearsCount;
                            item.istrForfietureFlag = busConstant.FLAG_YES;
                            item.comments += busConstant.FORFIETURE;
                            ForfeitureYearsCollectionMPIPreMerger.Add(item.year);

                            //PIR-552
                            istrIsBISParticipant = busConstant.FLAG_NO;
                            istrIsBISParticipantTemp = busConstant.FLAG_NO; //Forfieture Vesting Logic Fixes //PIR 1043//RID
                            //item.istrBisParticipantFlag = busConstant.FLAG_NO;//PIR 552
                        }
                    }
                }

            }
            #endregion

        }


        public Collection<cdoPerson> GetBeneficariesOfParticipant(int aintPlan_Id, string istrMPI_ID)
        {
            Collection<cdoPerson> lclcBeneficary = null;
            DataTable ldtbResult = null;
            string lstrPlanName = string.Empty;
            DataTable ldtplan = Select("cdoPlan.GetPlanById", new object[1] { aintPlan_Id });
            if (ldtplan.Rows.Count > 0)
            {
                DataRow drRow = ldtplan.Rows[0];
                lstrPlanName = Convert.ToString(drRow[0]);
            }

            //Check if this is the case of Disability Conversion 
            //If in case Participant is married at the time of Early Retirement and has Early Retirement for benefit option of type JS and got divorced before 
            //Disability conversion ,so in this we will allow him to choose JS type benefit option,load Ex spouse
            if (icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
            {
                if (this.iclbPayeeAccount != null && this.iclbPayeeAccount.Count > 0)
                {
                    if (this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == aintPlan_Id).Count() > 0)
                    {
                        ldtbResult = busBase.Select("cdoBenefitApplication.GetBeneficiaryForParticipantDisabilityConversion",
                            new object[1] { this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == aintPlan_Id).FirstOrDefault().icdoPayeeAccount.payee_account_id });
                    }
                }
            }

            if (ldtbResult == null || (ldtbResult != null && ldtbResult.Rows.Count <= 0))
            {
                ldtbResult = busBase.Select("cdoBenefitApplication.GetBeneficaryofPersonForGivenPlan", new object[2] { this.icdoBenefitApplication.person_id, aintPlan_Id });
            }


            lclcBeneficary = doBase.GetCollection<cdoPerson>(ldtbResult);
            return lclcBeneficary;
        }

        public Collection<cdoPlanBenefitXr> GetBenefitOptionsforPlan(int aintPlan_Id)
        {
            DataTable ldtbResult = null;
            Collection<cdoPlanBenefitXr> lclcBenefitOptions = new Collection<cdoPlanBenefitXr>();


            if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
            {
                if (this.ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_SINGLE || this.ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_DIVORCED)
                {
                    ldtbResult = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForRetrifSinlgleorDivorced", new object[1] { aintPlan_Id });
                    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                }
                else if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP && item.istrSubPlan.IsNullOrEmpty()).Count() > 0 && aintPlan_Id == busConstant.IAP_PLAN_ID)
                {
                    string astrBenefitOption = this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP && item.istrSubPlan.IsNullOrEmpty()).First().icdoBenefitApplicationDetail.istrBenefitOptionValue;
                    if (astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                        astrBenefitOption = busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY;
                    else if (astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                        astrBenefitOption = busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY;
                    ldtbResult = busBase.Select("cdoBenefitApplicationDetail.GetBenefitFromPlanForRetrIAP", new object[2] { aintPlan_Id, astrBenefitOption });
                    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                }
                else
                {
                    //PROD PIR 67  07222013
                    //if ((this.icdoBenefitApplication.min_distribution_flag == busConstant.FLAG_YES ||
                    //    (this.iclbBenefitApplicationDetail.Where(item => item.icdoBenefitApplicationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_LATE).Count() > 0)) && aintPlan_Id != busConstant.IAP_PLAN_ID)
                    //{
                    //    aintPlan_Id = busConstant.MPIPP_PLAN_ID;

                    //    ldtbResult = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForRetr", new object[1] { aintPlan_Id });
                    //    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);

                    //}
                    //else
                    //{
                    ldtbResult = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForRetr", new object[1] { aintPlan_Id });
                    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                    //}

                }
            }
            else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
            {
                //for PIR-531
                if (this.icdoBenefitApplication.child_support_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty())  //EmergencyOneTimePayment - 03/17/2020
                {
                    cdoPlanBenefitXr lcdoPlanBenefitXR = new cdoPlanBenefitXr();
                    ldtbResult = busBase.Select("cdoBenefitApplication.GetBenOptionLumpSum", new object[] { });
                    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                }
                else
                {
                    ldtbResult = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForWdrl", new object[1] { aintPlan_Id });
                    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                    cdoPlanBenefitXr lcdoPlanBenefitXR = new cdoPlanBenefitXr();

                    if (!this.EligibileforL52Spl && !this.Eligibile4IAP && this.EligibileforL161Spl)
                    {
                        lcdoPlanBenefitXR = lclcBenefitOptions.Where(option => option.benefit_option_value == busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY).First();
                        lclcBenefitOptions.Remove(lcdoPlanBenefitXR);

                        lcdoPlanBenefitXR = lclcBenefitOptions.Where(option => option.benefit_option_value == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY).First();
                        lclcBenefitOptions.Remove(lcdoPlanBenefitXR);
                    }
                    else if (!this.EligibileforL161Spl)
                    {
                        if (lclcBenefitOptions.Where(option => option.benefit_option_value == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY).Count() > 0)
                        {
                            lcdoPlanBenefitXR = lclcBenefitOptions.Where(option => option.benefit_option_value == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY).First();
                            lclcBenefitOptions.Remove(lcdoPlanBenefitXR);
                        }
                    }
                }
            }
            else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
            {
                //Scenario arises from Production PIR-764  Disability Conversion (Review)
                //Check if this is the case of Disability Conversion 
                //If in case Participant is married at the time of Early Retirement and has Early Retirement for benefit option of type JS and got divorced before 
                //Disability conversion ,so in this we will allow him to choose JS type benefit option
                if (this.iclbPayeeAccount != null && this.iclbPayeeAccount.Count > 0)
                {
                    if (this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == aintPlan_Id).Count() > 0)
                    {
                        DataTable ldtbResultTemp = new DataTable();
                        if (aintPlan_Id == this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == aintPlan_Id).FirstOrDefault().icdoPayeeAccount.iintPlanId
                            && aintPlan_Id == busConstant.MPIPP_PLAN_ID)
                        {
                            ldtbResultTemp = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForDsbl", new object[1] { aintPlan_Id });
                            ldtbResult = ldtbResultTemp.FilterTable(utlDataType.Numeric, enmPlanBenefitXr.plan_benefit_id.ToString().ToUpper(),
                                this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == aintPlan_Id).FirstOrDefault().icdoPayeeAccount.plan_benefit_id).CopyToDataTable();
                            lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                            return lclcBenefitOptions;
                        }
                        else if (aintPlan_Id == this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == aintPlan_Id).FirstOrDefault().icdoPayeeAccount.iintPlanId)
                        {
                            this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == aintPlan_Id).FirstOrDefault().LoadBenefitDetails();
                            string istrBenefitOptionValue = this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == aintPlan_Id).FirstOrDefault().icdoPayeeAccount.istrBenefitOptionValue;

                            ldtbResultTemp = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForDsbl", new object[1] { busConstant.MPIPP_PLAN_ID });

                            if (istrBenefitOptionValue.IsNotNullOrEmpty())
                            {
                                ldtbResult = ldtbResultTemp.FilterTable(utlDataType.String, enmPlanBenefitXr.benefit_option_value.ToString().ToUpper(),
                                   istrBenefitOptionValue).CopyToDataTable();
                            }

                            if (ldtbResult == null || (ldtbResult != null && ldtbResult.Rows.Count <= 0))
                            {
                                ldtbResult = ldtbResultTemp;
                            }

                            lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                            return lclcBenefitOptions;

                        }
                    }
                }

                //if (this.ibusPerson.icdoPerson.marital_status_value != busConstant.MARITAL_STATUS_SINGLE || this.ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_DIVORCED)
                if (this.ibusPerson.icdoPerson.marital_status_value != busConstant.MARITAL_STATUS_MARRIED)
                {
                    ldtbResult = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForDsblifSinlgleorDivorced", new object[1] { aintPlan_Id });
                    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                }
                else if (this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP && item.istrSubPlan.IsNullOrEmpty()).Count() > 0 && aintPlan_Id == busConstant.IAP_PLAN_ID)
                {
                    string astrBenefitOption = this.iclbBenefitApplicationDetail.Where(item => item.istrPlanCode == busConstant.MPIPP && item.istrSubPlan.IsNullOrEmpty()).First().icdoBenefitApplicationDetail.istrBenefitOptionValue;
                    if (astrBenefitOption == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
                        astrBenefitOption = busConstant.JOINT_100_PERCENT_SURVIVOR_ANNUITY;
                    else if (astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY)
                        astrBenefitOption = busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY;
                    ldtbResult = busBase.Select("cdoBenefitApplicationDetail.GetBenefitFromPlanForDsblIAP", new object[2] { aintPlan_Id, astrBenefitOption });
                    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                }
                else
                {
                    ldtbResult = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForDsbl", new object[1] { aintPlan_Id });
                    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                }



                //BR-018-05 Disability Conversion (Review)
                //if (this.iclbPayeeAccount != null && this.iclbPayeeAccount.Count > 0)
                //{
                //    if (this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == aintPlan_Id).Count() > 0)
                //    {
                //        int lintPlanBenefitId = this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == aintPlan_Id).FirstOrDefault().icdoPayeeAccount.plan_benefit_id;
                //        ldtbResult.Clear();
                //        ldtbResult = busBase.Select("cdoPlanBenefitXr.GetPlanBenefitXRDeatilsFromPlanBenefitId", new object[1] { lintPlanBenefitId });

                //        Collection<cdoPlanBenefitXr> lclcBenefitOptionsXr = new Collection<cdoPlanBenefitXr>();
                //        lclcBenefitOptionsXr = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);

                //        if (lclcBenefitOptionsXr.Count > 0)
                //        {
                //            if (lclcBenefitOptions.Where(item => item.benefit_option_value == lclcBenefitOptionsXr.FirstOrDefault().benefit_option_value).Count() == 0)
                //                lclcBenefitOptions.Add(lclcBenefitOptionsXr.FirstOrDefault());
                //        }
                //    }
                //}

            }
            else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
            {
                if (iclbPayeeAccount != null && iclbPayeeAccount.Where(lbuspayeeacc => lbuspayeeacc.icdoPayeeAccount.iintPlanId == aintPlan_Id).Count() > 0)
                {
                    //ldtbResult = Select<cdoPlanBenefitXr>(
                    //new string[2] { enmPlanBenefitXr.plan_id.ToString(), enmPlanBenefitXr.death_pre_retirement_post_election_flag.ToString() },
                    //new object[2] { aintPlan_Id, busConstant.FLAG_YES }, null, null);
                    lclcBenefitOptions = GetBenefitOptionsForPreRetirementDeathPostElection(aintPlan_Id);
                    //lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                }
                if (lclcBenefitOptions.IsNullOrEmpty())
                {
                    if (this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue && this.ibusPerson.icdoPerson.date_of_death < Convert.ToDateTime(busConstant.MERGER_DATE_STRING) && aintPlan_Id == busConstant.LOCAL_52_PLAN_ID)
                    {
                        ldtbResult = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForRetr", new object[1] { aintPlan_Id });
                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                    }
                    else if (this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue && this.ibusPerson.icdoPerson.date_of_death >= Convert.ToDateTime(busConstant.MERGER_DATE_STRING) && aintPlan_Id == busConstant.LOCAL_52_PLAN_ID)
                    {
                        lclcBenefitOptions = this.lclcL52BenOptionsPreDeath;
                    }

                    else if (this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue && aintPlan_Id == busConstant.IAP_PLAN_ID && this.QualifiedSpouseExists != true)
                    {
                        ldtbResult = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForDeath", new object[1] { aintPlan_Id });
                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                        if (lclcBenefitOptions.Where(item => item.benefit_option_value == busConstant.LIFE).Count() > 0)
                            lclcBenefitOptions.Remove(lclcBenefitOptions.Where(item => item.benefit_option_value == busConstant.LIFE).First());
                    }
                    else
                    {
                        ldtbResult = busBase.Select("cdoBenefitApplication.GetBenefitFromPlanForDeath", new object[1] { aintPlan_Id });
                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                    }
                }
            }

            return lclcBenefitOptions;
        }

        public Collection<cdoPlan> GetSubPlan(int aintPlan_Id)
        {
            Collection<cdoPlan> lclbSubPlans = new Collection<cdoPlan>();

            if (aintPlan_Id == busConstant.MPIPP_PLAN_ID)
            {
                // if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                //{
                if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                {
                    DataTable ldtblCount = busBase.Select("cdoPersonAccountRetirementContribution.CheckIfEEAmountPresent", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id });
                    DataTable ldtblCountUVHP = busBase.Select("cdoPersonAccountRetirementContribution.CheckIfUVHPAmountPresent", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id });
                    if ((ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0) || (ldtblCountUVHP.Rows.Count > 0 && Convert.ToInt32(ldtblCountUVHP.Rows[0][0]) > 0))
                    {
                        cdoPlan lcdoPlan = new cdoPlan();
                        lcdoPlan.plan_code = busConstant.EE_UVHP;
                        lcdoPlan.plan_name = busConstant.EE_UVHP;
                        lclbSubPlans.Add(lcdoPlan);
                    }
                }
                //}
                //else
                //{
                //    if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && !CheckAlreadyVested(busConstant.MPIPP))
                //    {
                //        if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                //        {
                //            DataTable ldtblCount = busBase.Select("cdoPersonAccountRetirementContribution.CheckIfEEAmountPresent", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id });
                //            if (ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0)
                //            {
                //                cdoPlan lcdoPlan = new cdoPlan();
                //                lcdoPlan.plan_code = busConstant.EE;
                //                lcdoPlan.plan_name = busConstant.EE;
                //                lclbSubPlans.Add(lcdoPlan);
                //            }
                //        }
                //    }

                //    if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                //    {
                //        DataTable ldtblCount = busBase.Select("cdoPersonAccountRetirementContribution.CheckIfUVHPAmountPresent", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id });
                //        if (ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0)
                //        {
                //            cdoPlan lcdoPlan1 = new cdoPlan();
                //            lcdoPlan1.plan_code = busConstant.UVHP;
                //            lcdoPlan1.plan_name = busConstant.UVHP;
                //            lclbSubPlans.Add(lcdoPlan1);
                //        }
                //    }
                //}
            }
            else if (aintPlan_Id == busConstant.IAP_PLAN_ID && icdoBenefitApplication.child_support_flag != busConstant.FLAG_YES )//for PIR-531 
            {
                if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID).Count() > 0)
                {
                    DataTable ldtblCount = busBase.Select("cdoPersonAccount.CheckPersonHasSpecialAccount", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoPersonAccount.person_account_id });
                    if (ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0)
                    {
                        if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                        {
                            if (this.EligibileforL52Spl || icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty()) //EmergencyOneTimePayment - 03/17/2020
                            {
                                cdoPlan lcdoPlan = new cdoPlan();
                                lcdoPlan.plan_code = busConstant.L52_SPL_ACC;
                                lcdoPlan.plan_name = "LOCAL-52 SPECIAL ACCOUNT";
                                lclbSubPlans.Add(lcdoPlan);
                            }
                        }
                        else
                        {
                            cdoPlan lcdoPlan = new cdoPlan();
                            lcdoPlan.plan_code = busConstant.L52_SPL_ACC;
                            lcdoPlan.plan_name = "LOCAL-52 SPECIAL ACCOUNT";
                            lclbSubPlans.Add(lcdoPlan);
                        }
                    }
                }


                if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID).Count() > 0)
                {
                    DataTable ldtblCount = busBase.Select("cdoPersonAccount.CheckPersonHasSpecialAccount", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoPersonAccount.person_account_id });
                    if (ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0)
                    {

                        if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                        {
                            if (this.EligibileforL161Spl || icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty())
                            {
                                cdoPlan lcdoPlan1 = new cdoPlan();
                                lcdoPlan1.plan_code = busConstant.L161_SPL_ACC;
                                lcdoPlan1.plan_name = "LOCAL-161 SPECIAL ACCOUNT";
                                lclbSubPlans.Add(lcdoPlan1);
                            }
                        }
                        else
                        {
                            cdoPlan lcdoPlan1 = new cdoPlan();
                            lcdoPlan1.plan_code = busConstant.L161_SPL_ACC;
                            lcdoPlan1.plan_name = "LOCAL-161 SPECIAL ACCOUNT";
                            lclbSubPlans.Add(lcdoPlan1);
                        }
                    }
                }
            }

            return lclbSubPlans;
        }


        //public Collection<cdoPlan> GetSubPlan(int aintPlan_Id)
        //{
        //    Collection<cdoPlan> lclbSubPlans = new Collection<cdoPlan>();

        //    if (aintPlan_Id == busConstant.MPIPP_PLAN_ID)
        //    {
        //        int lCount = 0;
        //        if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
        //        {

        //            if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
        //            {
        //                lCount = (int)DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.CheckIfEEAmountPresent", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
        //                if (lCount > 0)
        //                {
        //                    cdoPlan lcdoPlan = new cdoPlan();
        //                    lcdoPlan.plan_code = busConstant.EE;
        //                    lcdoPlan.plan_name = busConstant.EE;
        //                    lclbSubPlans.Add(lcdoPlan);
        //                }
        //            }
        //        }
        //        else
        //        {

        //            if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
        //            {
        //                lCount = (int)DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.CheckIfUVHPAmountPresent", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
        //                if (lCount > 0)
        //                {
        //                    cdoPlan lcdoPlan1 = new cdoPlan();
        //                    lcdoPlan1.plan_code = busConstant.UVHP;
        //                    lcdoPlan1.plan_name = busConstant.UVHP;
        //                    lclbSubPlans.Add(lcdoPlan1);
        //                }
        //            }
        //        }
        //    }

        //    else if (aintPlan_Id == busConstant.IAP_PLAN_ID)
        //    {
        //        int lCount = 0;

        //        if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID).Count() > 0)
        //        {
        //            lCount = (int)DBFunction.DBExecuteScalar("cdoPersonAccount.CheckPersonHasSpecialAccount", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
        //            if (lCount > 0)
        //            {
        //                cdoPlan lcdoPlan = new cdoPlan();
        //                lcdoPlan.plan_code = busConstant.L52_SPL_ACC;
        //                lcdoPlan.plan_name = "LOCAL-52 SPECIAL ACCOUNT";
        //                lclbSubPlans.Add(lcdoPlan);
        //            }
        //        }


        //        if (this.ibusPerson.iclbPersonAccount.IsNotNull() && this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID).Count() > 0)
        //        {
        //            lCount = (int)DBFunction.DBExecuteScalar("cdoPersonAccount.CheckPersonHasSpecialAccount", new object[1] { this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
        //            if (lCount > 0)
        //            {
        //                cdoPlan lcdoPlan1 = new cdoPlan();
        //                lcdoPlan1.plan_code = busConstant.L161_SPL_ACC;
        //                lcdoPlan1.plan_name = "LOCAL-161 SPECIAL ACCOUNT";
        //                lclbSubPlans.Add(lcdoPlan1);
        //            }
        //        }
        //    }

        //    return lclbSubPlans;
        //}


        public Collection<cdoPlanBenefitXr> GetBenefitOptionsforSubPlan(string astrSubPlan)
        {
            DataTable ldtbResult = null;
            Collection<cdoPlanBenefitXr> lclcBenefitOptions = new Collection<cdoPlanBenefitXr>();

            if (astrSubPlan.IsNotNullOrEmpty())
            {
                if (astrSubPlan == busConstant.UVHP || astrSubPlan == busConstant.EE)
                {
                    if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
                    {
                        ldtbResult = busBase.Select("cdoPlanBenefitXr.GetBenefitsForSubPlanInRetr", new object[2] { astrSubPlan, busConstant.MPIPP_PLAN_ID });
                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                    }

                    else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                    {
                        ldtbResult = busBase.Select("cdoPlanBenefitXr.GetBenefitsForSubPlanInWdrl", new object[2] { astrSubPlan, busConstant.MPIPP_PLAN_ID });
                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);

                    }

                    else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                    {
                        ldtbResult = busBase.Select("cdoPlanBenefitXr.GetBenefitsForSubPlanInPreDeath", new object[2] { astrSubPlan, busConstant.MPIPP_PLAN_ID });
                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                    }

                    else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        ldtbResult = busBase.Select("cdoPlanBenefitXr.GetBenefitsForSubPlanInDisability", new object[2] { astrSubPlan, busConstant.MPIPP_PLAN_ID });
                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);

                    }
                }
                else if (astrSubPlan == busConstant.EE_UVHP)
                {

                    ldtbResult = busBase.Select("cdoPlanBenefitXr.GetBenefitsForSubPlanInPreDeath", new object[2] { astrSubPlan, busConstant.MPIPP_PLAN_ID });
                    lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);

                }
                else
                {
                    if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
                    {
                        ldtbResult = busBase.Select("cdoPlanBenefitXr.GetBenefitsForSubPlanInRetr", new object[2] { astrSubPlan, busConstant.IAP_PLAN_ID });
                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                    }

                    else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                    {
                        //EmergencyOneTimePayment - 03/17/2020
                        if (this.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty())
                        {
                            ldtbResult = busBase.Select("cdoBenefitApplication.GetBenOptionLumpSum", new object[] { });
                        }
                        else
                        {
                            ldtbResult = busBase.Select("cdoPlanBenefitXr.GetBenefitsForSubPlanInWdrl", new object[2] { astrSubPlan, busConstant.IAP_PLAN_ID });
                        }
                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                        if (this.ibusPerson.icdoPerson.marital_status_value != busConstant.MARITAL_STATUS_MARRIED)
                        {
                            lclcBenefitOptions = lclcBenefitOptions.Where(item => !(item.benefit_option_value == busConstant.QJ50 || item.benefit_option_value == busConstant.J100 || item.benefit_option_value == busConstant.JS75)).ToList().ToCollection();
                        }
                    }
                    else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                    {
                        ldtbResult = busBase.Select("cdoPlanBenefitXr.GetBenefitsForSubPlanInPreDeath", new object[2] { astrSubPlan, busConstant.IAP_PLAN_ID });
                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                    }
                    else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        ldtbResult = busBase.Select("cdoPlanBenefitXr.GetBenefitsForSubPlanInDisability", new object[2] { astrSubPlan, busConstant.IAP_PLAN_ID });
                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtbResult);
                    }
                }
            }

            return lclcBenefitOptions;
        }

        public void GetAgeAtRetirement(DateTime adtRetirementDate)
        {
            idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, adtRetirementDate);
        }


        public ArrayList CheckErrorOnAddButton(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false, bool ablnDisabilityConversion = false)
        {
            string lstrBenefitOption = string.Empty;
            string lstrPlanCode = string.Empty;
            utlError lobjError = null;
            object lobj = null;
            string lstrSubPlan = string.Empty;
            lstrSubPlan = Convert.ToString(ahstParams["istrSubPlan"]);

            string lstrUVHPFlag = null;
            string lstrEEFlag = null;
            string lstrSpL52Flag = null;
            string lstrSpL161Flag = null;

            if (lstrSubPlan == string.Empty)
            {
                lstrSubPlan = null;
            }


            if (aobj is busRetirementApplication)
            {
                lobj = aobj as busRetirementApplication;
            }
            else if (aobj is busWithdrawalApplication)
            {
                lobj = aobj as busWithdrawalApplication;
            }
            else if (aobj is busDisabilityApplication)
            {
                lobj = aobj as busDisabilityApplication;
            }
            DateTime ldtDisablityConversiondate = DateTime.MinValue;
           
            if (ahstParams.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(ahstParams["iintPlan_ID"])))
            {
                int lintPlanID = Convert.ToInt32(ahstParams["iintPlan_ID"]);
                busPlan lbusPaln = new busPlan();
                lbusPaln.FindPlan(lintPlanID);
                lstrPlanCode = lbusPaln.icdoPlan.plan_code;
                                
                if (ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"] != null && ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"] != "")
                {
                    lstrBenefitOption = Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"]);
                }

                else if (ahstParams["icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue"] != null && ahstParams["icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue"] != "")
                {
                    lstrBenefitOption = Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue"]);
                }

                else if (ahstParams["icdoBenefitApplicationDetail.plan_benefit_id"] != null)
                {
                    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                    lbusPlanBenefitXr.FindPlanBenefitXr(Convert.ToInt32(ahstParams["icdoBenefitApplicationDetail.plan_benefit_id"]));
                    lstrBenefitOption = lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value;
                }
                if (!string.IsNullOrEmpty(lstrBenefitOption))
                {
                    busCodeValue lbusCodeValue = new busCodeValue();
                    lbusCodeValue.icdoCodeValue = lbusCodeValue.GetCodeValue(busConstant.BENEFIT_OPTION_CODE_ID, lstrBenefitOption);

                    //

                    if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
                    {
                        DataTable ldtbBenefitApplcation = null;
                        if (this.icdoBenefitApplication.converted_min_distribution_flag != busConstant.FLAG_YES)
                        {
                            string lstrMinDistFlag = busConstant.FLAG_NO;
                            string lstrBenfitStatusValue = string.Empty;
                            //ROHAN :temporarily commented out ,don not delete
                            ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedRetirementForGivenPlan", new object[3] { this.icdoBenefitApplication.person_id, lintPlanID, this.icdoBenefitApplication.benefit_application_id });
                            if (ldtbBenefitApplcation.IsNotNull() && ldtbBenefitApplcation.Rows.Count > 0)
                            {
                                lstrMinDistFlag = Convert.ToString(ldtbBenefitApplcation.Rows[0][enmBenefitApplication.min_distribution_flag.ToString()]);
                                lstrBenfitStatusValue = Convert.ToString(ldtbBenefitApplcation.Rows[0][enmBenefitApplication.application_status_value.ToString()]);
                                if (lstrMinDistFlag == busConstant.FLAG_YES && lstrBenfitStatusValue == busConstant.BENEFIT_APPLICATION_STATUS_APPROVED)
                                {
                                    DataTable ldtbPayeeAccount = busBase.Select("cdoBenefitApplication.GetMinDistPayeeAccountForPlan", new object[2] { lintPlanID, this.icdoBenefitApplication.person_id });
                                    if (ldtbPayeeAccount.IsNotNull() && ldtbPayeeAccount.Rows.Count > 0)
                                    {
                                        string lstrAppID = Convert.ToString(ldtbPayeeAccount.Rows[0][enmBenefitApplicationDetail.benefit_application_id.ToString()]);
                                        //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
                                        //DataTable ldtbMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(6212);
                                        //string lstrMessage = ldtbMessageInfo.Rows[0]["display_message"].ToString();
                                        utlMessageInfo lobjutlMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(6212);
                                        string lstrMessage = lobjutlMessageInfo.display_message;
                                        lobjError = AddError(0, string.Format(lstrMessage, lstrAppID));
                                        aarrErrors.Add(lobjError);
                                        return aarrErrors;
                                    }
                                }
                            }
                        }
                        else
                        {
                            ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetAppvdRetrForConvertedMin", new object[3] { this.icdoBenefitApplication.person_id, lintPlanID, this.icdoBenefitApplication.benefit_application_id });
                        }
                        if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
                        {
                            //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
                            //DataTable ldtbMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5150);
                            //string lstrMessage = ldtbMessageInfo.Rows[0]["display_message"].ToString();
                            utlMessageInfo lobjutlMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5150);
                            string lstrMessage = lobjutlMessageInfo.display_message;
                            lobjError = AddError(0, string.Format(lstrMessage, lstrPlanCode));
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }

                    }
                    else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                    {
                        //ROHAN :temporarily commented out ,don not delete

                        DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedWithdrawalForGivenPlan",
                            new object[5] { this.icdoBenefitApplication.person_id, lintPlanID, this.icdoBenefitApplication.benefit_application_id, lstrSubPlan, this.icdoBenefitApplication.withdrawal_date });
                        if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
                        {
                            int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                            if (lintCount > 0)
                            {
                                lobjError = AddError(5151, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                        }

                        if (Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.MPIPP_PLAN_ID && lstrSubPlan.IsNullOrEmpty())
                        {
                            lobjError = AddError(5424, "");
                            aarrErrors.Add(lobjError);
                        }

                    }

                    else if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        //ROHAN :temporarily commented out ,don not delete
                        int lintPayeeAccounts = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.GetCompletedPayeeAccountForGivenPlan", new object[2] { this.icdoBenefitApplication.person_id, lintPlanID },
                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);


                        DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedDisabilityForGivenPlan", new object[3] { this.icdoBenefitApplication.person_id, lintPlanID, this.icdoBenefitApplication.benefit_application_id });
                        if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
                        {
                            int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                            if (lintCount > 0 && lintPayeeAccounts == 0)
                            {
                                //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
                                //DataTable ldtbMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5152);
                                //string lstrMessage = ldtbMessageInfo.Rows[0]["display_message"].ToString();
                                utlMessageInfo lobjutlMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5152);
                                string lstrMessage = lobjutlMessageInfo.display_message;
                                lobjError = AddError(0, string.Format(lstrMessage, lstrPlanCode));
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                        }
                    }


                    //
                    // Single can choose only lump sum and Life auunity.
                    if (!(!string.IsNullOrEmpty(lbusCodeValue.icdoCodeValue.data1) && lbusCodeValue.icdoCodeValue.data1 != busConstant.Joint_Survivor && lbusCodeValue.icdoCodeValue.data1 != busConstant.LEVEL_INCOME))
                    {
                        bool lblnflag = true;

                        if (icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                        {
                            if (this.iclbPayeeAccount != null && this.iclbPayeeAccount.Count > 0)
                            {
                                if (this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == lintPlanID).Count() > 0)
                                {
                                    DataTable ldtbResult = busBase.Select("cdoBenefitApplication.GetBeneficiaryForParticipantDisabilityConversion",
                                        new object[1] { this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == lintPlanID).FirstOrDefault().icdoPayeeAccount.payee_account_id });

                                    if (ldtbResult != null && ldtbResult.Rows.Count > 0)
                                    {
                                        lblnflag = false;
                                    }
                                }
                            }
                        }

                        if (lblnflag && ibusPerson.icdoPerson.marital_status_value != busConstant.MARITAL_STATUS_MARRIED && lstrPlanCode != busConstant.LOCAL_700)
                        {
                            lobjError = AddError(5022, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }
                    }

                    if (!(!string.IsNullOrEmpty(lbusCodeValue.icdoCodeValue.data1) && lbusCodeValue.icdoCodeValue.data1 != busConstant.Joint_Survivor))
                    {
                        if (lstrPlanCode != busConstant.LOCAL_700)
                        {
                            if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"]) == string.Empty || Convert.ToString(ahstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"]) == busConstant.ZERO_STRING)
                            {
                                lobjError = AddError(5020, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                        }
                        else
                        {
                            if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"]) == string.Empty || Convert.ToString(ahstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"]) == busConstant.ZERO_STRING)
                            {
                                lobjError = AddError(5020, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                        }
                    }
                    // For Ten Years Certain Life Annutiy plan child beneficary is mandatory.          
                    //if (!string.IsNullOrEmpty(lbusCodeValue.icdoCodeValue.data1) && lbusCodeValue.icdoCodeValue.data1 == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY && !(lobj is busDisabilityApplication))
                    //{
                    //    DataTable ldtbbenPar = Select("cdoBenefitApplication.GetChildBeneficaryOfParticipant", new object[1] { this.ibusPerson.icdoPerson.person_id });
                    //    if (ldtbbenPar.Rows.Count > 0)
                    //    {
                    //        DataRow drRow = ldtbbenPar.Rows[0];
                    //        int lintCount = Convert.ToInt32(drRow[0]);
                    //        if (lintCount == 0)
                    //        {
                    //            lobjError = AddError(5024, "");
                    //            aarrErrors.Add(lobjError);
                    //        }
                    //    }
                    //}
                    if (CheckDuplicatePlan(lobj, lintPlanID, lstrSubPlan, 0, ablnHardError))   // Plan cannot be duplicated.
                    {
                        bool lbnlFlag = busConstant.BOOL_FALSE;
                        foreach (utlError lError in aarrErrors)
                        {
                            if (lError.istrErrorID == "5023")
                            {
                                lbnlFlag = busConstant.BOOL_TRUE;
                                break;
                            }
                        }
                        if (!lbnlFlag)
                        {
                            lobjError = AddError(5023, "");
                            aarrErrors.Add(lobjError);
                        }
                    }
                    bool lblnCheckConversionPayeeAccountExists = false;
                    if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY && !this.iclbPayeeAccount.IsNullOrEmpty())
                    {
                        if (this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == lintPlanID).Count() > 0)
                        {
                            lblnCheckConversionPayeeAccountExists = true;
                        }
                    }
                    if (!lblnCheckConversionPayeeAccountExists)
                    {
                        aintBenefitDetailsPlanID = Convert.ToInt32(ahstParams["iintPlan_ID"]); // PROD PIR 151
                        if (ahstParams["icdoBenefitApplicationDetail.spousal_consent_flag"] != null && CheckSpousalConsent(lstrBenefitOption, ahstParams["icdoBenefitApplicationDetail.spousal_consent_flag"].ToString()))
                        {
                            lobjError = AddError(5025, "");
                            aarrErrors.Add(lobjError);
                        }
                    }

                }
                else  //benefit Option is required
                {
                    lobjError = AddError(5019, "");
                    aarrErrors.Add(lobjError);
                }
            }
            else  //Plan is Required
            {
                lobjError = AddError(1126, "");
                aarrErrors.Add(lobjError);
            }
            return aarrErrors;
        }

        public bool CheckSpousalConsent(string astrBenefitOption, string ablnSpousalFlagVal) // To Check Spousal Consent for three benefit options mentioned below.
        {
            //if ((this is busWithdrawalApplication || this is busDisabilityApplication || this is busRetirementApplication) && ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_MARRIED && astrBenefitOption != "QJ50"
            //    && ablnSpousalFlagVal != "Y")
            //{
            //    return true;
            //}
            //else if (ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_MARRIED && (astrBenefitOption != "QJ50"
            //    && astrBenefitOption != "JS75" && astrBenefitOption != "J100" && astrBenefitOption != "JS66") && ablnSpousalFlagVal != "Y")
            //{
            //    return true;
            //}
            // PROD PIR 151
            if (astrBenefitOption == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY && (this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == 3).Count()) > 0
                && (this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_600_PLAN_ID).FirstOrDefault().icdoPersonAccount.plan_id) == aintBenefitDetailsPlanID)
                return false;

            decimal ldecUVHPAndEESum = 0M;
            int lintPersonAccountID = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
            DataTable ldtbUVHPandEENVesSumAmount = busBase.Select("cdoPersonAccountRetirementContribution.GetUVHPSumWithNonVestedEESumContribun", new object[] { lintPersonAccountID });

            DateTime ldtJointAnnuitanttDOD = DateTime.MinValue;

            if (ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_MARRIED)
            {
                DataTable ldtbGetJointAnnuitantDOD = busBase.Select("cdoRelationship.GetJointAnnuitantID", new object[1] { this.ibusPerson.icdoPerson.person_id });
                if (ldtbGetJointAnnuitantDOD.IsNotNull() && ldtbGetJointAnnuitantDOD.Rows.Count > 0)
                {
                    if (!(String.IsNullOrEmpty(ldtbGetJointAnnuitantDOD.Rows[0]["DATE_OF_DEATH"].ToString())))
                    {
                        ldtJointAnnuitanttDOD = Convert.ToDateTime(ldtbGetJointAnnuitantDOD.Rows[0]["DATE_OF_DEATH"]);
                    }
                }
            }

            if ((this is busDisabilityApplication || this is busRetirementApplication || this is busRetirementWizard) && ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_MARRIED
                    && astrBenefitOption != "QJ50"
                    && ablnSpousalFlagVal != "Y" && ldtJointAnnuitanttDOD == DateTime.MinValue
                    )
            {
                return true;
            }

            if (ldtbUVHPandEENVesSumAmount.Rows.Count > 0)
            {
                if (ldtbUVHPandEENVesSumAmount.Rows[0][0] != DBNull.Value)
                    ldecUVHPAndEESum = Convert.ToInt32(ldtbUVHPandEENVesSumAmount.Rows[0][0]);
                if (this is busWithdrawalApplication && ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_MARRIED && astrBenefitOption != "QJ50"
                    && ablnSpousalFlagVal != "Y" &&
                    (astrBenefitOption == "LUMP" && ldecUVHPAndEESum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT) && ldtJointAnnuitanttDOD == DateTime.MinValue)
                {
                    return true;

                }


                else if (ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_MARRIED && (astrBenefitOption != "QJ50"
                    && astrBenefitOption != "JS75" && astrBenefitOption != "J100" && astrBenefitOption != "JS66")
                    && ablnSpousalFlagVal != "Y" &&
                    (astrBenefitOption == "LUMP" && ldecUVHPAndEESum > busConstant.BenefitCalculation.LUMP_SUM_CASH_OUT_LIMIT && ldtJointAnnuitanttDOD == DateTime.MinValue)
                  )
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckDuplicatePlan(object aobj, int aintplanID, string astrSubPlan, int aintSurvivorID = 0, bool ablnHardError = false)
        {
            int lintCount = 0;

            busRetirementApplication lbusRetirementApplication = null;
            busWithdrawalApplication lbusWithdrawalApplication = null;
            busDisabilityApplication lbusDisabilityApplication = null;
            busDeathPreRetirement lbusDeathPreRetirement = null;

            if (aobj is busRetirementApplication)
            {
                lbusRetirementApplication = aobj as busRetirementApplication;
            }
            else if (aobj is busWithdrawalApplication)
            {
                lbusWithdrawalApplication = aobj as busWithdrawalApplication;
            }
            else if (aobj is busDisabilityApplication)
            {
                lbusDisabilityApplication = aobj as busDisabilityApplication;
            }
            else if (aobj is busDeathPreRetirement)
            {
                lbusDeathPreRetirement = aobj as busDeathPreRetirement;
            }

            if (!ablnHardError)
            {
                if (lbusRetirementApplication != null)
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in lbusRetirementApplication.iclbBenefitApplicationDetail)
                    {
                        if ((lbusBenefitApplicationDetail.istrSubPlan).IsNullOrEmpty())
                            lbusBenefitApplicationDetail.istrSubPlan = null;

                        if (lbusBenefitApplicationDetail.iintPlan_ID == aintplanID && lbusBenefitApplicationDetail.istrSubPlan == astrSubPlan)
                        {
                            return true;
                        }
                    }
                }
                else if (lbusWithdrawalApplication != null)
                {
                    foreach (busBenefitApplicationDetail objbusBenefitApplicationDetail in lbusWithdrawalApplication.iclbBenefitApplicationDetail)
                    {
                        if ((objbusBenefitApplicationDetail.istrSubPlan).IsNullOrEmpty())
                            objbusBenefitApplicationDetail.istrSubPlan = null;
                        if (objbusBenefitApplicationDetail.iintPlan_ID == aintplanID && objbusBenefitApplicationDetail.istrSubPlan == astrSubPlan)
                        {
                            return true;
                        }
                    }
                }
                else if (lbusDisabilityApplication != null)
                {
                    foreach (busBenefitApplicationDetail objbusBenefitApplicationDetail in lbusDisabilityApplication.iclbBenefitApplicationDetail)
                    {
                        if ((objbusBenefitApplicationDetail.istrSubPlan).IsNullOrEmpty())
                            objbusBenefitApplicationDetail.istrSubPlan = null;
                        if (objbusBenefitApplicationDetail.iintPlan_ID == aintplanID && objbusBenefitApplicationDetail.istrSubPlan == astrSubPlan)
                        {
                            return true;
                        }
                    }
                }
                else if (lbusDeathPreRetirement != null)
                {
                    foreach (busBenefitApplicationDetail objbusBenefitApplicationDetail in lbusDeathPreRetirement.iclbBenefitApplicationDetail)
                    {
                        if ((objbusBenefitApplicationDetail.istrSubPlan).IsNullOrEmpty())
                            objbusBenefitApplicationDetail.istrSubPlan = null;
                        if (objbusBenefitApplicationDetail.iintPlan_ID == aintplanID && objbusBenefitApplicationDetail.istrSubPlan == astrSubPlan && objbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id == aintSurvivorID)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (lbusRetirementApplication != null)
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in lbusRetirementApplication.iclbBenefitApplicationDetail)
                    {
                        if ((lbusBenefitApplicationDetail.istrSubPlan).IsNullOrEmpty())
                            lbusBenefitApplicationDetail.istrSubPlan = null;
                        if (lbusBenefitApplicationDetail.iintPlan_ID == aintplanID && lbusBenefitApplicationDetail.istrSubPlan == astrSubPlan)
                        {

                            lintCount++;
                        }
                    }
                    if (lintCount > 1)
                    {
                        return true;
                    }
                }
                else if (lbusWithdrawalApplication != null)
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in lbusWithdrawalApplication.iclbBenefitApplicationDetail)
                    {
                        if ((lbusBenefitApplicationDetail.istrSubPlan).IsNullOrEmpty())
                            lbusBenefitApplicationDetail.istrSubPlan = null;
                        if (lbusBenefitApplicationDetail.iintPlan_ID == aintplanID && lbusBenefitApplicationDetail.istrSubPlan == astrSubPlan)
                        {
                            lintCount++;
                        }
                    }
                    if (lintCount > 1)
                    {
                        return true;
                    }
                }
                else if (lbusDisabilityApplication != null)
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in lbusDisabilityApplication.iclbBenefitApplicationDetail)
                    {
                        if ((lbusBenefitApplicationDetail.istrSubPlan).IsNullOrEmpty())
                            lbusBenefitApplicationDetail.istrSubPlan = null;
                        if (lbusBenefitApplicationDetail.iintPlan_ID == aintplanID && lbusBenefitApplicationDetail.istrSubPlan == astrSubPlan)
                        {

                            lintCount++;
                        }
                    }
                    if (lintCount > 1)
                    {
                        return true;
                    }
                }
                else if (lbusDeathPreRetirement != null)
                {
                    foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in lbusDeathPreRetirement.iclbBenefitApplicationDetail)
                    {
                        if ((lbusBenefitApplicationDetail.istrSubPlan).IsNullOrEmpty())
                            lbusBenefitApplicationDetail.istrSubPlan = null;
                        if (lbusBenefitApplicationDetail.iintPlan_ID == aintplanID && lbusBenefitApplicationDetail.istrSubPlan == astrSubPlan && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id == aintSurvivorID)
                        {

                            lintCount++;
                        }
                    }
                    if (lintCount > 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void LoadInitialData()
        {
            DataTable ldtbList = busPerson.Select("cdoBenefitApplication.CheckJoinderOnFile", new object[1] { ibusPerson.icdoPerson.person_id });
            if (ldtbList.Rows.Count > 0)
            {
                icdoBenefitApplication.istrJoinderOnFile = busConstant.YES;
            }
            else
            {
                icdoBenefitApplication.istrJoinderOnFile = busConstant.NO;
            }

            if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
            {                
                if ((icdoBenefitApplication.min_distribution_date) == (DateTime.MinValue) || (icdoBenefitApplication.min_distribution_date) == null)
                {
                    if (this.ibusPerson == null)
                    {
                        this.ibusPerson = new busPerson();
                        ibusPerson.FindPerson(icdoBenefitApplication.person_id);
                    }

                    if (ibusPerson.icdoPerson.idtDateofBirth != DateTime.MinValue)
                        icdoBenefitApplication.min_distribution_date = LoadMinDistributionDate(this.ibusPerson.icdoPerson.idtDateofBirth);
                }             
            }

            if (icdoBenefitApplication.benefit_application_id <= 0)
            {
                icdoBenefitApplication.benefit_type_id = busConstant.BENEFIT_TYPE_CODE_ID;
                icdoBenefitApplication.application_status_id = busConstant.BENEFIT_APPLICATION_STATUS_CODE_ID;
                icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                icdoBenefitApplication.application_status_description = busConstant.BENEFIT_APPLICATION_STATUS_PENDING_DESC;

                if (icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                {
                    icdoBenefitApplication.benefit_type_description = busConstant.BENEFIT_TYPE_WITHDRAWAL_DESC;
                }

                if (icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                {
                    icdoBenefitApplication.benefit_type_description = busConstant.BENEFIT_TYPE_DISABILITY_DESC;
                }

                if (icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                {
                    icdoBenefitApplication.benefit_type_description = busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT_DESC;
                }

                if (icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
                {
                    icdoBenefitApplication.benefit_type_description = busConstant.BENEFIT_TYPE_RETIREMENT_DESC;
                }
            }

            if (icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)           //need to relocate
            {
                DataTable ldtbMarriageDate = new DataTable();
                ldtbMarriageDate = busPerson.Select("cdoBenefitApplication.GetMarriageDateForWdrl", new object[1] { ibusPerson.icdoPerson.person_id });
                if (ldtbMarriageDate.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(ldtbMarriageDate.Rows[0][0])))
                {
                    icdoBenefitApplication.dtDateOfMarriage = Convert.ToDateTime(ldtbMarriageDate.Rows[0][0]);
                }
            }

            if (icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY) //need to relocate
            {

                DataTable ldtbEarlyRetirementDate = busPerson.Select("cdoBenefitApplication.GetEarlyRetirementDate", new object[1] { ibusPerson.icdoPerson.person_id });
                if (ldtbEarlyRetirementDate.Rows.Count > 0 && ldtbEarlyRetirementDate.Rows[0][0] != DBNull.Value)
                {
                    icdoBenefitApplication.dtEarlyRetirementDate = Convert.ToDateTime(ldtbEarlyRetirementDate.Rows[0][0]);
                }

            }

        }


        private DateTime LoadMinDistributionDate(DateTime adtDateOfBirth)
        {
            DateTime ldtMinDistributionDate = DateTime.MinValue;
            DateTime ldtDob = adtDateOfBirth;

            //ldtDob = ldtDob.AddYears(70);
            //ldtDob = ldtDob.AddMonths(6);

            //for pir-522
            DateTime ldtVestedDt = new DateTime();
            DataTable ldtGetVestedDate = busBase.Select("cdoPersonAccountEligibility.GetVestedDateForMD", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtGetVestedDate != null && ldtGetVestedDate.Rows.Count > 0 && (Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]) != DateTime.MinValue))
            {
                ldtVestedDt = Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]);
                ldtMinDistributionDate = busGlobalFunctions.GetMinDistributionDate(this.ibusPerson.icdoPerson.person_id, ldtVestedDt);

            }
            else {

                ldtMinDistributionDate = busGlobalFunctions.GetMinDistributionDate(this.ibusPerson.icdoPerson.person_id);

            }
                //if (ldtVestedDt != DateTime.MinValue && ldtVestedDt > ldtDob)

                //    ldtMinDistributionDate = new DateTime(ldtVestedDt.Year + 1, 01, 01);
                //else
                //    ldtMinDistributionDate = new DateTime(ldtDob.Year + 1, 04, 01);
           
            //else
            //    ldtMinDistributionDate = new DateTime(ldtDob.Year + 1, 04, 01);

            return ldtMinDistributionDate;
        }



        public bool IsApplicationReceivedDateNull()
        {
            if (this.icdoBenefitApplication.application_received_date == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }


        public bool IsApplicationReceivedDateGreaterThanDOB()
        {
            if (this.icdoBenefitApplication.application_received_date != DateTime.MinValue)
            {
                if (this.icdoBenefitApplication.application_received_date < ibusPerson.icdoPerson.idtDateofBirth)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsApplicationReceivedDateValid()
        {
            if (this.icdoBenefitApplication.application_received_date != DateTime.MinValue)
            {
                if (this.icdoBenefitApplication.application_received_date > DateTime.Today)
                {
                    return false;
                }
            }
            return true;
        }

        public DataTable GetPlanAndBenefitOptionValue(int aintPlanBenefitId)
        {
            DataTable ltlblPlanBenfit = new DataTable();
            ltlblPlanBenfit = Select("cdoPlanBenefitXr.GetPlanAndBenefitOptionValue", new object[1] { aintPlanBenefitId });
            return ltlblPlanBenfit;
        }

        public bool ValidateExistingRecord(int aintPersonId, string astrBenefitTypeValue, int aintbenefitapplicationid)
        {
            bool lblnResult = false;

            // Check Benefit Application Type already Exist
            //ROHAN :temporarily commented out ,don not delete

            //if (aintPersonId.IsNotNull() && astrBenefitTypeValue.IsNotNullOrEmpty())
            //{
            //    DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.CheckBenefitApplication", new object[] { aintPersonId, astrBenefitTypeValue, aintbenefitapplicationid });
            //    if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
            //    {
            //        int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
            //        if (lintCount > 0)
            //            lblnResult = true;
            //    }
            //}
            //return lblnResult;

            //if (astrBenefitTypeValue == busConstant.BENEFIT_TYPE_RETIREMENT)
            //{
            //    DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedRetirement", new object[] { aintPersonId });
            //    if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
            //    {
            //        int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
            //        if (lintCount > 0)
            //            lblnResult = true;
            //    }
            //}
            //else if (astrBenefitTypeValue == busConstant.BENEFIT_TYPE_DISABILITY)
            //{
            //    DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedDisability", new object[] { aintPersonId });
            //    if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
            //    {
            //        int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
            //        if (lintCount > 0)
            //            lblnResult = true;
            //    }
            //}
            return lblnResult;
        }



        public bool ValidateRecord()
        {
            bool lblnResult = false;

            //ROHAN :temporarily commented out ,don not delete

            // if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
            //{
            //    DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedPreDeathApplication", new object[1] { this.icdoBenefitApplication.person_id });
            //    if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
            //    {
            //        int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
            //        if (lintCount > 0)
            //            lblnResult = true;
            //    }
            //}

            return lblnResult;
        }


        public void UpdateStatusHistoryValue()
        {
            busBenefitApplicationStatusHistory lbusBenefitApplicationStatusHistory = new busBenefitApplicationStatusHistory();
            lbusBenefitApplicationStatusHistory.InsertStatusHistory(icdoBenefitApplication.benefit_application_id, icdoBenefitApplication.application_status_value,
               icdoBenefitApplication.modified_date, icdoBenefitApplication.modified_by);

        }

       



        //Code-Abhishek //This method could be used at the Person OverView Screen to get things done
        public void LoadWorkHistoryandSetupPrerequisites_PersonOverView(bool ablnFromService = false)
        {
            //Kunal :Fix For Vesting 02/28/2014
            this.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, DateTime.Now);

            //PIR 933
            bool lblnBool = false;
            if (this.ibusPerson.icdoPerson.recalculate_vesting_flag == busConstant.FLAG_YES)
            {
                lblnBool = true;
            }

            this.LoadandProcessWorkHistory_ForAllPlans(false, null, null, ablnFromService);

            //PIR 933
            if (lblnBool && (!this.CheckAlreadyVested(busConstant.MPIPP) || !this.CheckAlreadyVested(busConstant.IAP)))
            {
                this.ibusPerson.icdoPerson.recalculate_vesting_flag = busConstant.FLAG_YES;
            }

            if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.DetermineVesting();
            }
        }

        public void LoadWorkHistoryandSetupPrerequisites_Retirement()
        {
            //this.LoadandProcessWorkHistory_ForAllPlans(); -- NOT required since we do it in NEW OR FIND methods all the time

            if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.DetermineVesting();
                this.DetermineBenefitSubTypeandEligibility_Retirement();
            }
        }

        public void LoadWorkHistoryandSetupPrerequisites_Disability()
        {
            //this.LoadandProcessWorkHistory_ForAllPlans(); -- NOT required since we do it in NEW OR FIND methods all the time

            if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.DetermineVesting();
                this.DetermineBenefitSubTypeandEligibility_Disability();
            }
        }

        public void LoadWorkHistoryandSetupPrerequisites_Withdrawal()
        {
            //this.LoadandProcessWorkHistory_ForAllPlans(); -- NOT required since we do it in NEW OR FIND methods all the time

            if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.DetermineVesting();
                this.DetermineBenefitSubTypeandEligibility_Withdrawal();
            }
        }


        public void LoadWorkHistoryandSetupPrerequisites_PreRetirementDeath()
        {
            //this.LoadandProcessWorkHistory_ForAllPlans(); -- NOT required since we do it in NEW OR FIND methods all the time

            if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                SetupPrerequisitesDeath();
            }
        }


        public void btn_Pending()
        {
            icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
            icdoBenefitApplication.Update();
            if (this.iclbBenefitApplicationDetail.Count > 0)
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.application_detail_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.Update();
                }
            }
            UpdateStatusHistoryValue();
            //TEST
            EvaluateInitialLoadRules();
        }

        public virtual ArrayList btn_Approved()
        {
            ArrayList iarrError = new ArrayList();

            //PIR 924 checking in progress/certified death notification
            if (icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
            {
                Collection<busDeathNotification> lclbDeathNotifications = new Collection<busDeathNotification>();
                DataTable ldtblist = busPerson.Select("cdoPerson.LoadDeathNotificationDetails", new object[1] { this.icdoBenefitApplication.person_id });
                lclbDeathNotifications = GetCollection<busDeathNotification>(ldtblist, "icdoDeathNotification");
                if (lclbDeathNotifications != null && lclbDeathNotifications.Count() > 0 &&
                    lclbDeathNotifications.Where(titem => titem.icdoDeathNotification.death_notification_status_value == busConstant.NOTIFICATION_STATUS_IN_PROGRESS).Count() > 0)
                {
                    utlError lobjError = AddError(6282, "");  
                    iarrError.Add(lobjError);
                    return iarrError;
                }
            }

            if (this.iclbBenefitApplicationDetail != null && this.iclbBenefitApplicationDetail.Count > 0)
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id <= 0)
                    {
                        utlError lobjError = AddError(5480, "");
                        iarrError.Add(lobjError);
                        return iarrError;
                    }
                    if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                    {
                        DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.CheckIfWithdrawalRecordExistsForIAP", 
                                new object[4] { this.icdoBenefitApplication.person_id, this.icdoBenefitApplication.withdrawal_date, 
                                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrL52SpecialAccount, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrL161SpecialAccount });
                        if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 1)
                        {
                            utlError lobjError = AddError(6291, "");
                            iarrError.Add(lobjError);
                            return iarrError;
                        }
                    }
                }
            }

            if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
            {
                if (this.icdoBenefitApplication.converted_min_distribution_flag != busConstant.FLAG_YES)
                {
                    this.LoadWorkHistoryandSetupPrerequisites_Retirement();

                    foreach (busBenefitApplicationDetail item in this.iclbBenefitApplicationDetail)
                    {
                        if (!(Eligible_Plans.Contains(item.istrPlanCode)) || Eligible_Plans.IsNullOrEmpty())
                        {
                            utlError lobjError = AddError(5118, "");
                            iarrError.Add(lobjError);
                            return iarrError;
                        }
                    }
                }

                    //PIR-799
                    UpdateBenefitApplicationEligiblePlans();

                busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.WITHDRAWAL_WORKFLOW_NAME, icdoBenefitApplication.person_id, 0, 0, null);
            }

            if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
            {
                this.LoadWorkHistoryandSetupPrerequisites_Disability();

                //if (this.iblnDisabilityConversion)
                //{
                //DataTable ldtblPayeeAccounts = Select("cdoBenefitApplication.GetPayeeAccountinReceivedStatus", new object[1] { this.icdoBenefitApplication.person_id });
                 
                //PIR-954
                if (this.IsApplicationReceivedDateNull())
                {
                    utlError lobjError = AddError(5026, "");
                    iarrError.Add(lobjError);
                    return iarrError;
                }
               //PIR-911
                if (this.iblnDisabilityConversion && this.icdoBenefitApplication.ssa_application_date == DateTime.MinValue)
                {
                    utlError lobjError = AddError(5122, "");
                    iarrError.Add(lobjError);
                    return iarrError;
                }

                if (this.icdoBenefitApplication.ssa_application_date >= this.icdoBenefitApplication.application_received_date && !(this.icdoBenefitApplication.terminally_ill_flag == "Y")) 
                {
                    utlError lobjError = AddError(5130, "");
                    iarrError.Add(lobjError);
                    return iarrError;
                }

                //PIR - 954
                if (!(this.icdoBenefitApplication.terminally_ill_flag == "Y") && this.icdoBenefitApplication.disability_onset_date != DateTime.MinValue &&
                        this.icdoBenefitApplication.application_received_date < this.icdoBenefitApplication.disability_onset_date)
                {
                    utlError lobjError = AddError(5046, "");
                    iarrError.Add(lobjError);
                    return iarrError;
                }


                if (this.iclbPayeeAccount.Count > 0)
                {

                    foreach (busPayeeAccount lbusPayeeAccount in this.iclbPayeeAccount)
                    {
                        foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                        {
                            if (lbusBenefitApplicationDetail.iintPlan_ID == lbusPayeeAccount.icdoPayeeAccount.iintPlanId)
                            {
                                int lintNotCancelled = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.CheckIfApplicationDetailIsNotCancelled",
                                    new object[1] { lbusPayeeAccount.icdoPayeeAccount.benefit_application_detail_id },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                                if (lintNotCancelled > 0)
                                {
                                    utlError lobjError = null;
                                    //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
                                    //DataTable ldtbMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5491);
                                    //string lstrMessage = ldtbMessageInfo.Rows[0]["display_message"].ToString();
                                    utlMessageInfo lobjutlMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5491);
                                    string lstrMessage = lobjutlMessageInfo.display_message;
                                    lobjError = AddError(0, string.Format(lstrMessage, lbusBenefitApplicationDetail.istrPlanCode));
                                    iarrError.Add(lobjError);
                                    return iarrError;
                                }
                            }
                        }
                    }

                    //BR-018-12 TEMP commented needs confirmation
                    int lintQDROWF = (int)DBFunction.DBExecuteScalar("cdoDroApplication.GetActiveQDROWF",
                         new object[1] { this.ibusPerson.icdoPerson.person_id },
                             iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    
                    //rid 82043
                    bool lblnQDRODisability = false;
                    if (this.iclbBenefitApplicationDetail[0].iintPlan_ID == this.iclbPayeeAccount[0].icdoPayeeAccount.iintPlanId &&
                        this.iclbPayeeAccount[0].icdoPayeeAccount.benefit_end_date == DateTime.MinValue && this.iclbPayeeAccount[0].icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT &&
                        (this.iclbPayeeAccount[0].icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY || this.iclbPayeeAccount[0].icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY ||
                        this.iclbPayeeAccount[0].icdoPayeeAccount.retirement_type_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY))
                    {
                        lblnQDRODisability = true;
                    }

                    if (lintQDROWF > 0 && !lblnQDRODisability)
                    {
                        utlError lobjError = AddError(5513, "");
                        iarrError.Add(lobjError);
                        return iarrError;
                    }
                }

                int lintPendingQDROWorkflows = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.GetPendingQDROWorkflow", new object[1] { this.icdoBenefitApplication.person_id },
                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if (lintPendingQDROWorkflows > 0)
                {
                    bool lblnFlag = true;


                    int lintDroExists = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.CkeckIfQDOExists", new object[1] { this.icdoBenefitApplication.person_id },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (lintDroExists > 0)
                    {
                        int lintSharedInterestDroExists = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.CheclIfSharedInterestDroExists", new object[1] { this.icdoBenefitApplication.person_id },
                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (lintSharedInterestDroExists > 0)
                        {
                            lblnFlag = false;
                        }

                        if (lblnFlag)
                        {
                            int lintApprovedQDROFinalCalc = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.GetApprovedQDROFinalCalc", new object[1] { this.icdoBenefitApplication.person_id },
                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                            if (lintApprovedQDROFinalCalc == 0)
                            {
                                //PIR 811
                                iblnIsPendingProcessQDROApplicationWorkflow = true;
                                //utlError lobjError = AddError(5492, "");
                                //iarrError.Add(lobjError);
                                //return iarrError;
                            }
                        }
                    }
                }
                //}

                foreach (busBenefitApplicationDetail item in this.iclbBenefitApplicationDetail)
                {
                    if (!(Eligible_Plans.Contains(item.istrPlanCode)) || Eligible_Plans.IsNullOrEmpty())
                    {
                        utlError lobjError = AddError(5118, "");
                        iarrError.Add(lobjError);
                        return iarrError;
                    }
                }
                //Ticket#81132
               
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedDisabilityForGivenPlan", new object[3] { this.icdoBenefitApplication.person_id, lbusBenefitApplicationDetail.iintPlan_ID, this.icdoBenefitApplication.benefit_application_id });
                    if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
                    {
                        int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                        if (lintCount > 0)
                        {
                            //utlError lobjError = AddError(5152, "");
                            //iarrError.Add(lobjError);
                            //return iarrError;

                            //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
                            //DataTable ldtbMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5152);
                            //string lstrMessage = ldtbMessageInfo.Rows[0]["display_message"].ToString();
                            utlMessageInfo lobjutlMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5152);
                            string lstrMessage = lobjutlMessageInfo.display_message;
                            utlError lobjError = AddError(0, string.Format(lstrMessage, lbusBenefitApplicationDetail.istrPlanCode));
                            iarrError.Add(lobjError);
                            return iarrError;

                        }
                    }
                }

                        //PIR 811
                        ValidateSoftErrors();
            }


            if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
            {
                this.DetermineVesting();
                this.LoadWorkHistoryandSetupPrerequisites_PreRetirementDeath();

                foreach (busBenefitApplicationDetail item in this.iclbBenefitApplicationDetail)
                {
                    if (!(Eligible_Plans.Contains(item.istrPlanCode)) || Eligible_Plans.IsNullOrEmpty())
                    {
                        utlError lobjError = AddError(5118, "");
                        iarrError.Add(lobjError);
                        return iarrError;
                    }
                }
            }


            if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && !this.iblnWithdrawalForAlternatePayee)
            {
                if ((this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).IsNotNull()) && (this.icdoBenefitApplication.child_support_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.emergency_onetime_payment_flag == busConstant.FLAG_YES || this.icdoBenefitApplication.withdrawal_type_value.IsNotNullOrEmpty())) //EmergencyOneTimePayment - 03/17/2020
                {
                    Eligible_Plans.Clear();
                    Eligible_Plans.Add(busConstant.IAP);
                }
                else
                    this.LoadWorkHistoryandSetupPrerequisites_Withdrawal();

                foreach (busBenefitApplicationDetail item in this.iclbBenefitApplicationDetail)
                {
                    if (!(Eligible_Plans.Contains(item.istrPlanCode)) || Eligible_Plans.IsNullOrEmpty())
                    {
                        utlError lobjError = AddError(5118, "");
                        iarrError.Add(lobjError);
                        return iarrError;
                    }
                }
            }


            if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL && this.iblnWithdrawalForAlternatePayee)
            {
                int lintCount = (int)DBFunction.DBExecuteScalar("cdoBenefitApplicationDetail.GetBenefitDetails", new object[1] { this.icdoBenefitApplication.benefit_application_id },
                                                         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (lintCount <= 0)
                {
                    utlError lobjError = AddError(5480, "");
                    iarrError.Add(lobjError);
                    return iarrError;
                }
            }

            //PIR 999
            if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_WITHDRAWAL)
            {
                if (iclbBenefitApplicationDetail != null && iclbBenefitApplicationDetail.Count > 0
                    && iclbBenefitApplicationDetail.Where(t => t.istrPlanCode == busConstant.MPIPP && t.icdoBenefitApplicationDetail.ee_flag == busConstant.FLAG_YES).Count() > 0 &&
                    icdoBenefitApplication.effective_date == DateTime.MinValue)
                {
                    utlError lobjError = AddError(6274, "");
                    iarrError.Add(lobjError);
                    return iarrError;
                }
            }

            ValidateHardErrors(utlPageMode.All);


            if (this.iclbBenefitApplicationDetail.Count == 0)
            {
                utlError lobjError = AddError(5094, "");
                iarrError.Add(lobjError);
                return iarrError;
            }
            else
            {
                iarrError.Add(this);
            }

         

            //LA-Sunset for Retiree Health Eligibility.
            //Ticket#78795 User doesnt want to update the retiree health eligibility date with the retirement_date.
            //if ((icdoBenefitApplication.benefit_type_value == "RTMT" || icdoBenefitApplication.benefit_type_value == "DSBL") && (icdoBenefitApplication.retirement_date != DateTime.MinValue))
            //{
            //    if (this.iclbBenefitApplicationDetail.Where(item => item.iintPlan_ID != 1).Count() > 0)
            //    {
            //        if ((this.ibusPerson.icdoPerson.health_eligible_flag == "Y") && (this.ibusPerson.icdoPerson.retirement_health_date == DateTime.MinValue))
            //        {
            //            this.ibusPerson.icdoPerson.retirement_health_date = icdoBenefitApplication.retirement_date;

            //            this.ibusPerson.icdoPerson.Update();

            //        }

            //    }

            //}
            icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_APPROVED;

            icdoBenefitApplication.Update();
            if (this.iclbBenefitApplicationDetail.Count > 0)
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.application_detail_status_value = busConstant.BENEFIT_APPLICATION_STATUS_APPROVED;
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.Update();
                    lbusBenefitApplicationDetail.EvaluateInitialLoadRules();
                }
            }


            UpdateStatusHistoryValue();
            LoadBenefitApplicationStatusHistorys();
            EvaluateInitialLoadRules();
            return iarrError;
        }

        public void UpdateBenefitApplicationEligiblePlans()
        {
            LoadBenefitApplicationEligiblePlansDetails();
            if (this.iclbBenefitApplicationEligiblePlans != null && this.iclbBenefitApplicationEligiblePlans.Count > 0)
            {
                foreach (busBenefitApplicationEligiblePlans lbusBenefitApplicationEligiblePlans in iclbBenefitApplicationEligiblePlans)
                {
                    bool iblnIsIAP_Flag = false;
                    bool iblnIsMPIPP_Flag = false;
                    bool iblnIsLocal600_Flag = false;
                    bool iblnIsLocal666_Flag = false;
                    bool iblnIslocal700_Flag = false;
                    bool iblnIslocal161_Flag = false;
                    bool iblnIslocal52_Flag = false;
                    foreach (busBenefitApplicationDetail item in this.iclbBenefitApplicationDetail)
                    {
                        if (item.istrPlanCode == busConstant.IAP)
                        {
                            if (lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.iap_flag != busConstant.FLAG_YES)
                                lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.iap_flag = busConstant.FLAG_YES;
                            iblnIsIAP_Flag = true;
                        }
                        else if (item.istrPlanCode == busConstant.MPIPP)
                        {
                            if (lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.mpipp_flag != busConstant.FLAG_YES)
                                lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.mpipp_flag = busConstant.FLAG_YES;
                            iblnIsMPIPP_Flag = true;
                        }
                        else if (item.istrPlanCode == busConstant.Local_600)
                        {
                            if (lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local600_flag != busConstant.FLAG_YES)
                                lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local600_flag = busConstant.FLAG_YES;
                            iblnIsLocal600_Flag = true;
                        }
                        else if (item.istrPlanCode == busConstant.Local_666)
                        {
                            if (lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local666_flag != busConstant.FLAG_YES)
                                lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local666_flag = busConstant.FLAG_YES;
                            iblnIsLocal666_Flag = true;
                        }
                        else if (item.istrPlanCode == busConstant.LOCAL_700)
                        {
                            if (lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local700_flag != busConstant.FLAG_YES)
                                lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local700_flag = busConstant.FLAG_YES;
                            iblnIslocal700_Flag = true;
                        }
                        else if (item.istrPlanCode == busConstant.Local_161)
                        {
                            if (lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local161_flag != busConstant.FLAG_YES)
                                lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local161_flag = busConstant.FLAG_YES;
                            iblnIslocal161_Flag = true;
                        }
                        else if (item.istrPlanCode == busConstant.Local_52)
                        {
                            if (lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local52_flag != busConstant.FLAG_YES)
                                lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local52_flag = busConstant.FLAG_YES;
                            iblnIslocal52_Flag = true;
                        }
                    }

                    if (!iblnIsIAP_Flag && lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.iap_flag == busConstant.FLAG_YES)
                        lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.iap_flag = busConstant.FLAG_NO;

                    if (!iblnIsMPIPP_Flag && lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.mpipp_flag == busConstant.FLAG_YES)
                        lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.mpipp_flag = busConstant.FLAG_NO;

                    if (!iblnIsLocal600_Flag && lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local600_flag == busConstant.FLAG_YES)
                        lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local600_flag = busConstant.FLAG_NO;

                    if (!iblnIsLocal666_Flag && lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local666_flag == busConstant.FLAG_YES)
                        lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local666_flag = busConstant.FLAG_NO;

                    if (!iblnIslocal700_Flag && lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local700_flag == busConstant.FLAG_YES)
                        lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local700_flag = busConstant.FLAG_NO;

                    if (!iblnIslocal161_Flag && lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local161_flag == busConstant.FLAG_YES)
                        lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local161_flag = busConstant.FLAG_NO;

                    if (!iblnIslocal52_Flag && lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local52_flag == busConstant.FLAG_YES)
                        lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.local52_flag = busConstant.FLAG_NO;

                    //update eligible plans
                    lbusBenefitApplicationEligiblePlans.icdoBenefitApplicationEligiblePlans.Update();
                }
            }
        }

        public void btn_Denied()
        {
            icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_DENIED;
            icdoBenefitApplication.Update();
            if (this.iclbBenefitApplicationDetail.Count > 0)
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.application_detail_status_value = busConstant.BENEFIT_APPLICATION_STATUS_DENIED;
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.Update();
                }
            }
            UpdateStatusHistoryValue();
            //TEST
            EvaluateInitialLoadRules();
        }


        public ArrayList btn_Cancelled()
        {
            //ValidateHardErrors(utlPageMode.All);
            ArrayList iarrError = new ArrayList();
            int lCount = 0;

          
            //PROD PIR 792 //PIR 924
            //if (!(string.IsNullOrEmpty(this.icdoBenefitApplication.cancellation_reason_value)) && !(string.IsNullOrEmpty(this.icdoBenefitApplication.reason_description)))
            //{
            //PIR 924 removed person id parameter.
            //PIR-843
            object lobjCheckDistributionStatusValue = null;
            
             lobjCheckDistributionStatusValue = DBFunction.DBExecuteScalar("cdoPersonAccountRetirementContribution.GetPaymentCountForApplication", new object[1] { this.icdoBenefitApplication.benefit_application_id },
                                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            int iintPaymentCount = 0;
            if (lobjCheckDistributionStatusValue != null)
            {
                iintPaymentCount = ((int)lobjCheckDistributionStatusValue);
            }
            if (iintPaymentCount > 0)
            {
                utlError lobjError = AddError(6284, "");
                iarrError.Add(lobjError);
                return iarrError;
            }
           
            if ((this.icdoBenefitApplication.cancellation_reason_value == "" || this.icdoBenefitApplication.cancellation_reason_value == null)
                && (this is busRetirementApplication || (this is busWithdrawalApplication && this.icdoBenefitApplication.dro_application_id <= 0) || this is busDisabilityApplication))
            {
                utlError lobjError = AddError(5057, "");
                iarrError.Add(lobjError);
                return iarrError;
            }
            else if ((this is busRetirementApplication || this is busWithdrawalApplication || this is busDisabilityApplication)
                && (this.icdoBenefitApplication.cancellation_reason_value == busConstant.CANCELLATION_REASON_DECEASED))
            {
                lCount = (int)DBFunction.DBExecuteScalar("cdoBenefitApplication.GetDateOfDeathFromDeathNotification", new object[1] { this.icdoBenefitApplication.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (lCount == 0)
                {
                    utlError lobjError = AddError(5158, "");
                    iarrError.Add(lobjError);
                    return iarrError;
                }
            }
            else if ((this is busRetirementApplication || this is busWithdrawalApplication || this is busDisabilityApplication)
                           && (this.icdoBenefitApplication.cancellation_reason_value == busConstant.CANCELLATION_REASON_OTHER) && string.IsNullOrEmpty(this.icdoBenefitApplication.reason_description))
            {
                utlError lobjError = AddError(5167, "");
                iarrError.Add(lobjError);
                return iarrError;
            }
            else if (this is busWithdrawalApplication && this.icdoBenefitApplication.dro_application_id > 0 && string.IsNullOrEmpty(this.icdoBenefitApplication.reason_description))
            {
                utlError lobjError = AddError(5167, "");
                iarrError.Add(lobjError);
                return iarrError;
            }
           
            //else //PROD PIR 792
            //{
            //    iarrError.Add(this);
            //}


            #region Cancal All Benefit Calculations For this Benefit Application
            DataTable ldtblBenefitCalculations = new DataTable();
            Collection<busBenefitCalculationHeader> lclbbusBenefitCalculationHeader = new Collection<busBenefitCalculationHeader>();
            ldtblBenefitCalculations = Select("cdoBenefitApplication.GetCalculationsForBenefitApplication", new object[1] { this.icdoBenefitApplication.benefit_application_id });
            lclbbusBenefitCalculationHeader = GetCollection<busBenefitCalculationHeader>(ldtblBenefitCalculations, "icdoBenefitCalculationHeader");

            if (lclbbusBenefitCalculationHeader.Count > 0)
            {
                foreach (busBenefitCalculationHeader lbusBenefitCalculationHeader in lclbbusBenefitCalculationHeader)
                {
                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.status_value = busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED;
                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.Update();
                }
            }
            // For Cancel Payee Account Status..

            bool lblnIsEarlyToDisability = false;
            if (icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT
                && iclbBenefitApplicationDetail != null && iclbBenefitApplicationDetail.Count > 0
                && iclbBenefitApplicationDetail.Where(item => item.icdoBenefitApplicationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_REDUCED_EARLY
                    || item.icdoBenefitApplicationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY ||
                    item.icdoBenefitApplicationDetail.benefit_subtype_value == busConstant.RETIREMENT_TYPE_UNREDUCED_EARLY).Count() > 0)
            {
                int intCount = (int)DBFunction.DBExecuteScalar("cdoPayeeAccount.GetIsEarlyToDisabilityCount", new object[1] { this.icdoBenefitApplication.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (intCount > 0)
                    lblnIsEarlyToDisability = true;
            }

            this.LoadPayeeAccountStatusByApplicationID(this.icdoBenefitApplication.benefit_application_id);
            foreach (busPayeeAccountStatus lobjPayeeAccountStatus in this.iclbPayeeAccountStatusByApplication)
            {
                if (lblnIsEarlyToDisability)
                {
                    lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED;
                }
                else
                {
                    lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED;
                }
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.Insert();

                // Initiate workflow if distribution status value 'OUTS' or 'CLRD' and payee account status cancelled
                DataTable ldtOutsdorCleardDistribution = busBase.Select("cdoPaymentHistoryHeader.GetOutsandClrdDistributionByPayeeAccountID", new Object[1] { lobjPayeeAccountStatus.icdoPayeeAccountStatus.payee_account_id });
                if (ldtOutsdorCleardDistribution != null && ldtOutsdorCleardDistribution.Rows.Count > 0 && ldtOutsdorCleardDistribution.Rows[0][0] != DBNull.Value)
                {
                    ///Code to initiate new workflow
                    Hashtable lhstRequestParams = new Hashtable();
                    lhstRequestParams.Add("PayeeAccountId", lobjPayeeAccountStatus.icdoPayeeAccountStatus.payee_account_id);
                    lhstRequestParams.Add("PaymentDateFrom", string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(ldtOutsdorCleardDistribution.Rows[0][0])));
                    lhstRequestParams.Add("PaymentDateTo", string.Format("{0:MM/dd/yyyy}", DateTime.Now));

                    busWorkflowHelper.InitializeWorkflow(busConstant.PROCESS_STOP_REISSUE_OR_RECLAMATION, this.icdoBenefitApplication.person_id,
                                                                0, 0, lhstRequestParams);
                }
            }


            //Ticket 67976 This Ticket is to fix, when user cancels the benefit application,  person account status should change to Active from Retire status.
            if ((this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT || this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY) && this.icdoBenefitApplication.application_status_value == busConstant.BENEFIT_APPLICATION_STATUS_APPROVED)
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    var ibusPersonAccount = ibusPerson.iclbPersonAccount.Where(x => x.icdoPersonAccount.plan_id == lbusBenefitApplicationDetail.iintPlan_ID);

                    if (ibusPersonAccount != null)
                    {
                        foreach (busPersonAccount lbusPersonAccount in ibusPersonAccount)
                        {
                            lbusPersonAccount.icdoPersonAccount.status_value = busConstant.PERSON_ACCOUNT_STATUS_ACTIVE;
                            lbusPersonAccount.icdoPersonAccount.Update();
                        }
                    }
                }
            }


            #endregion Cancel All Benefit Calculation For this Benefit Application

            //LA - Sunset for Retiree Health Eligibility.//Ticket#78795 User doesnt want to update the retiree health eligibility date with the retirement_date.
            //if ((icdoBenefitApplication.benefit_type_value == "RTMT" || icdoBenefitApplication.benefit_type_value == "DSBL") && (icdoBenefitApplication.retirement_date != DateTime.MinValue))
            //{
            //    if (this.iclbBenefitApplicationDetail.Where(item => item.iintPlan_ID != 1).Count() > 0)
            //    {
            //        if ((this.ibusPerson.icdoPerson.health_eligible_flag == "Y") && (this.ibusPerson.icdoPerson.retirement_health_date == icdoBenefitApplication.retirement_date))
            //        {
            //            this.ibusPerson.icdoPerson.retirement_health_date = DateTime.MinValue;

            //            this.ibusPerson.icdoPerson.Update();

            //        }
            //    }
            //}

            icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED;
            icdoBenefitApplication.cancellation_reason_id = busConstant.CANCELLATION_REASON_ID;
            //LoadBenefitApplicationDetails();
            icdoBenefitApplication.Update();
            if (this.iclbBenefitApplicationDetail.Count > 0)
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.application_detail_status_value = busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED;
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.Update();
                    lbusBenefitApplicationDetail.EvaluateInitialLoadRules();
                }
            }
          
            UpdateStatusHistoryValue();
            LoadBenefitApplicationStatusHistorys();
            EvaluateInitialLoadRules();
            //WI 23234 Secure Document submission
            RemoveDocumentUploadFlag(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id);


            if (ibusBaseActivityInstance.IsNotNull() && ((Sagitec.Bpm.busBpmActivityInstance)ibusBaseActivityInstance).ibusBpmActivity.ibusBpmProcess.IsNotNull() && ((Sagitec.Bpm.busBpmActivityInstance)ibusBaseActivityInstance).ibusBpmActivity.ibusBpmProcess.icdoBpmProcess.name==busConstant.PersonAccountMaintenance.CANCEL_SERVICE_RETIREMENT_APPLICATION)
            {
                iarrError = btnSubmit();
            }
            else if (ibusBaseActivityInstance.IsNotNull())
            {
                busSolBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busSolBpmActivityInstance;
                if (lbusBpmActivityInstance != null)
                {
                    lbusBpmActivityInstance.istrTerminationReason = busConstant.ReturnToWorkRequest.TERMINATION_REASON;
                    iarrError = lbusBpmActivityInstance.InvokeWorkflowAction();
                }
            }
            iarrError.Add(this); //PROD PIR 792
            return iarrError;
        }

        public ArrayList btn_Incomplete()
        {
            ArrayList iarrError = new ArrayList();
            icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPLICATION_STATUS_INCOMPLETE;
            icdoBenefitApplication.Update();
            if (this.iclbBenefitApplicationDetail.Count > 0)
            {
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
                {
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.application_detail_status_value = busConstant.BENEFIT_APPLICATION_STATUS_INCOMPLETE;
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.Update();
                }
            }
            UpdateStatusHistoryValue();
            EvaluateInitialLoadRules();
            iarrError.Add(this);
            return iarrError;
        }

        public void LoadPersonNotes(string lstrFormName)
        {
            ibusPerson.iclbNotes = new Collection<busNotes>();
            DataTable ldtblist = busPerson.Select("cdoNotes.GetNotesForWithdrawalCaclulation", new object[2] { icdoBenefitApplication.person_id, lstrFormName });
            ibusPerson.iclbNotes = GetCollection<busNotes>(ldtblist, "icdoNotes");
            if (iclbNotes != null)
                iclbNotes = iclbNotes.OrderByDescending(obj => obj.icdoNotes.created_date).ToList().ToCollection<busNotes>();
        }

        public ArrayList AddNotes()
        {
            ArrayList larrResult = new ArrayList();
            string lstrFormName = string.Empty;

            if (istrNewNotes.IsNullOrEmpty())
            {
                utlError lutlError = AddError(4076, "");
                larrResult.Add(lutlError);
                return larrResult;
            }

            if (this is busRetirementApplication)
            {
                lstrFormName = busConstant.RETIREMENT_APPLICATION_MAINTAINENCE_FORM;
            }
            else if (this is busDisabilityApplication)
            {
                lstrFormName = busConstant.DISABILITY_APPLICATION_MAINTAINENCE_FORM;
            }
            else if (this is busDeathPreRetirement)
            {
                lstrFormName = busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE_FORM;
            }
            else if (this is busWithdrawalApplication)
            {
                lstrFormName = busConstant.WITHDRAWL_APPLICATION_MAINTAINENCE_FORM;
            }

            cdoNotes lcdoNotes = new cdoNotes();
            lcdoNotes.notes = this.istrNewNotes;
            lcdoNotes.person_id = this.icdoBenefitApplication.person_id;
            lcdoNotes.form_id = busConstant.Form_ID;
            lcdoNotes.form_value = lstrFormName;
            lcdoNotes.created_by = iobjPassInfo.istrUserID;
            lcdoNotes.created_date = DateTime.Now;
            lcdoNotes.Insert();
            this.LoadPersonNotes(lcdoNotes.form_value);
            istrNewNotes = string.Empty;
            larrResult.Add(this);
            return larrResult;
        }

        #endregion

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();
            bool checkResult = false;

            // Check Benefit Application Type already Existaf
            if (this.icdoBenefitApplication.person_id.IsNotNull() && this.icdoBenefitApplication.benefit_type_value.IsNotNull())
            {
                checkResult = ValidateExistingRecord(this.icdoBenefitApplication.person_id, this.icdoBenefitApplication.benefit_type_value, this.icdoBenefitApplication.benefit_application_id);
                if (checkResult == true)
                {
                    lobjError = AddError(5403, " ");
                    this.iarrErrors.Add(lobjError);
                    checkResult = true;
                }
            }
            if (this.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT)
            {
                bool lblnThrowValidation = false;
                DataTable ldtData = busBase.Select("cdoBenefitApplication.GetMinDistriPayeeAccForParticipant", new object[1] { this.ibusPerson.icdoPerson.person_id });
                string lstrPlanCode = string.Empty;
                ArrayList arrMinDistribution = new ArrayList();
                if (ldtData.IsNotNull() && ldtData.Rows.Count > 0)
                {

                    foreach (DataRow ldtRow in ldtData.Rows)
                    {
                        lstrPlanCode = string.Empty;
                        lstrPlanCode = Convert.ToString(ldtRow[enmPlan.plan_code.ToString()]);
                        if (!string.IsNullOrEmpty(lstrPlanCode))
                        {
                            arrMinDistribution.Add(lstrPlanCode.Trim());
                        }
                    }
                    if (Eligible_Plans.IsNullOrEmpty())
                    {
                        lblnThrowValidation = true;
                    }
                    else
                    {
                        //foreach (string lstrPlanCode1 in Eligible_Plans)
                        //{
                        //    if (!arrMinDistribution.Contains(lstrPlanCode1.Trim()))
                        //    {
                        //        lblnThrowValidation = false;
                        //    }
                        //    else
                        //    {
                        //        lblnThrowValidation = true;
                        //    }
                        //}
                        //102895 instead of checking Eligible plans checking that plan in the current application already has MD payee account.
                        foreach (busBenefitApplicationDetail item in this.iclbBenefitApplicationDetail)
                        {
                            if (!(arrMinDistribution.Contains(item.istrPlanCode.Trim())))
                            {
                                lblnThrowValidation = false;
                            }
                            else
                            {
                                lblnThrowValidation = true;
                            }
                        }
                    }
                    if (lblnThrowValidation)
                    {
                        foreach (DataRow ldtRow in ldtData.Rows)
                        {
                            lstrPlanCode = string.Empty;
                            lstrPlanCode = Convert.ToString(ldtRow[enmPlan.plan_code.ToString()]);
                            if (!string.IsNullOrEmpty(lstrPlanCode))
                            {
                                string lstrAppID = Convert.ToString(ldtRow[enmBenefitApplicationDetail.benefit_application_id.ToString()]);
                                if (!string.IsNullOrEmpty(lstrAppID))
                                {
                                    //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
                                    //DataTable ldtbMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(6212);
                                    //string lstrMessage = ldtbMessageInfo.Rows[0]["display_message"].ToString();
                                    utlMessageInfo lobjutlMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(6212);
                                    string lstrMessage = lobjutlMessageInfo.display_message;
                                    lobjError = AddError(0, string.Format(lstrMessage, lstrAppID));
                                    this.iarrErrors.Add(lobjError);
                                }

                            }


                        }
                    }
                }


            }
            base.ValidateHardErrors(aenmPageMode);
        }

        #region For Local666 RETIREMENT_TYPE_REDUCED_EARLY and AGE < 60
        public decimal GetFactorFor_Local666(int astrBenefitTypeValue, string astrBenfitAccountTypeValue, string astrBenefotAccountTypeSubValue, decimal aintAge) //Age less than 60
        {
            decimal factorForLessThan60 = 0;
            DataTable ldtbFactorForLocal666 = busBase.Select("cdoBenefitProvisionBenefitTypeFactor.GetEarlyReductionBenefitFactor", new object[] { astrBenefitTypeValue, astrBenfitAccountTypeValue, astrBenefotAccountTypeSubValue, aintAge });
            if (ldtbFactorForLocal666.Rows.Count > 0)
            {
                factorForLessThan60 = Convert.ToDecimal(ldtbFactorForLocal666.Rows[0]["BENEFIT_TYPE_FACTOR"]);
            }
            return factorForLessThan60;
        }
        #endregion

        #region Vesting


        public bool VestingCheckingSetup(Collection<cdoDummyWorkData> iclbWorkHistory, string astrplan_code, int aintTriggerYear, bool ablnBISBatch = false, bool ablnSSNMerge = false, utlPassInfo aobjPassInfo = null)
        {
            bool IsVested = false;
            Collection<cdoBenefitProvisionEligibility> iclbApplicableRulesintheYear = new Collection<cdoBenefitProvisionEligibility>();

            if (astrLegacyDBConnetion.IsNullOrEmpty())
            {
                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            }

            if (!iclbAllVestingRules.IsNullOrEmpty())
                iclbAllVestingRules.Clear();

            DataTable ldtbVestingEligibility = new DataTable();
            ldtbVestingEligibility = busBase.Select("cdoBenefitProvisionEligibility.FetchVestingRules", new object[1] { astrplan_code });
            if (ldtbVestingEligibility.Rows.Count > 0)
            {
                iclbAllVestingRules = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(ldtbVestingEligibility);
                int flag = 0;
                bool lblnEvaluateVestingRules = true;
                int lintYearBefore76 = 0;
                if (iclbWorkHistory.Where(item => item.year < 1976).Count() > 0)
                {
                    int lintPrevYearQualifiedYearCount = 0;
                    foreach (cdoDummyWorkData lcdoDummyWorkData in iclbWorkHistory.OrderBy(item => item.year))
                    {
                        if (lcdoDummyWorkData.year < 1976)
                        {
                            if (lintPrevYearQualifiedYearCount > lcdoDummyWorkData.qualified_years_count && (lcdoDummyWorkData.qualified_years_count == 0 || lcdoDummyWorkData.qualified_years_count == 1))
                            {
                                lintYearBefore76 = lcdoDummyWorkData.year;

                            }
                        }
                        lintPrevYearQualifiedYearCount = lcdoDummyWorkData.qualified_years_count;

                        //Forfieture Vesting Logic Fixes
                        if (lintPrevYearQualifiedYearCount >= 10)
                            break;
                    }
                }

                //PIR 933
                DataTable ldtMPIPPVestingPlanYearDate = null;
                DataTable ldtIAPVestingPlanYearDate = null;

                if (astrplan_code == busConstant.MPIPP)
                {
                    SqlParameter[] lParametersMPIPP = new SqlParameter[2];
                    SqlParameter lparamMPIPP1 = new SqlParameter("@SSN", DbType.String);
                    SqlParameter lparamMPIPP2 = new SqlParameter("@PLANCODE", DbType.String);

                    lparamMPIPP1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                    lParametersMPIPP[0] = lparamMPIPP1;

                    lparamMPIPP2.Value = busConstant.MPIPP;
                    lParametersMPIPP[1] = lparamMPIPP2;

                    ldtMPIPPVestingPlanYearDate = new DataTable();
                    ldtMPIPPVestingPlanYearDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParametersMPIPP);
                }
                else if (astrplan_code == busConstant.IAP)
                {
                    SqlParameter[] lParameters = new SqlParameter[2];
                    SqlParameter lparam1 = new SqlParameter("@SSN", DbType.String);
                    SqlParameter lparam2 = new SqlParameter("@PLANCODE", DbType.String);

                    lparam1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                    lParameters[0] = lparam1;

                    lparam2.Value = busConstant.IAP;
                    lParameters[1] = lparam2;

                    ldtIAPVestingPlanYearDate = new DataTable();
                    ldtIAPVestingPlanYearDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                }


                //RID
                int lintWithDrawalYearBefore1976 = 0;
                DataTable ldtbWithdrawalBefore1976 = new DataTable();
                if (!string.IsNullOrEmpty(astrplan_code))
                    ldtbWithdrawalBefore1976 = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawalBefore1976", new object[2] { this.ibusPerson.icdoPerson.person_id, astrplan_code });

                if (ldtbWithdrawalBefore1976 != null && ldtbWithdrawalBefore1976.Rows.Count > 0)
                {
                    lintWithDrawalYearBefore1976 = Convert.ToInt32(ldtbWithdrawalBefore1976.AsEnumerable().AsDataTable().Select("WITHDRAWAL_YEAR = MAX(WITHDRAWAL_YEAR)")[0].ItemArray[0]); 
                }

                foreach (cdoDummyWorkData workitem in iclbWorkHistory)
                {
                    if (workitem.year <= lintWithDrawalYearBefore1976)
                        continue;

                    lblnEvaluateVestingRules = true;
                    if (flag == 1)
                        break;
                    if (workitem.year < lintYearBefore76)
                        lblnEvaluateVestingRules = false;
                    if (lblnEvaluateVestingRules)
                    {
                        if (workitem.year == aintTriggerYear || aintTriggerYear == 0)
                        {
                            iclbApplicableRulesintheYear = iclbAllVestingRules.Where(rule => rule.effective_year <= workitem.year).OrderByDescending(rule => rule.effective_year).ToList().ToCollection();

                            foreach (cdoBenefitProvisionEligibility VestingRule in iclbApplicableRulesintheYear)
                            {
                                //PIR 933
                                if (astrplan_code == busConstant.MPIPP)
                                    IsVested = this.CheckIfPersonIsVested(iclbWorkHistory, astrplan_code, VestingRule, workitem.year, ablnBISBatch, ablnSSNMerge, aobjPassInfo, ldtMPIPPVestingPlanYearDate);
                                else
                                    IsVested = this.CheckIfPersonIsVested(iclbWorkHistory, astrplan_code, VestingRule, workitem.year, ablnBISBatch, ablnSSNMerge, aobjPassInfo, ldtIAPVestingPlanYearDate);

                                if (IsVested)
                                {
                                    flag = 1;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return IsVested;
        }

        //Code-Abhishek
        public bool CheckIfPersonIsVested(Collection<cdoDummyWorkData> iclbWorkHistory, string astrplan_code, cdoBenefitProvisionEligibility VestingRule, int aintWorkItemYear, bool ablnBISBatch = false, bool ablnSSNMerge = false, utlPassInfo aobjPassInfo = null, DataTable adtVestingPlanYearDate = null) //PIR 933
        {
            DataTable ldtbVestingDate = new DataTable();
            DateTime ldtDateOfVesting = new DateTime();
            ldtDateOfVesting = DateTime.MinValue;
            
            if (iclbAllVestingRules.Count > 0)
            {
                #region CHECK UNDER 6 VESTING RULES

                #region RULE-1
                //RULE-1 -Pension and IAP – Effective 10/26/1953 
                //  10 Vested Years, (excluding forfeitures and withdrawals)       
                if (VestingRule.eligibility_type_value == busConstant.RULE_1)
                {
                    //lcdoVestingRule = (cdoBenefitProvisionEligibility)iclbAllVestingRules.Where(rule => rule.eligibility_type_value.ToString() == busConstant.RULE_1).First();
                    if (VestingRule.IsNotNull())
                    {
                        if (iclbWorkHistory.Where(item => item.vested_years_count >= VestingRule.vested_years && item.year == aintWorkItemYear).Count() > 0)
                        {
                            SqlParameter[] lParameters = new SqlParameter[3];
                            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                            SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);
                            SqlParameter param3 = new SqlParameter("@YEAR", DbType.Int32);

                            //Get the Year when he actually got Vested 
                            var YearofVesting = from item in iclbWorkHistory where item.vested_years_count == VestingRule.vested_years && item.year == aintWorkItemYear orderby item.year ascending select item.year;

                            if (!(YearofVesting.IsNullOrEmpty()))
                            {
                                DataRow[] idrWeeklySubset;

                                int Year = (Int32)YearofVesting.First();
                                if (astrplan_code == busConstant.MPIPP)
                                {
                                    if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                    {
                                        //PIR 933
                                        //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                        //lParameters[0] = param1;

                                        //param2.Value = busConstant.MPIPP;
                                        //lParameters[1] = param2;

                                        //param3.Value = Year;
                                        //lParameters[2] = param3;

                                        //if (ablnSSNMerge == false)
                                        //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                                        //else
                                        //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                        //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                        //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                        //else
                                        //    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);

                                        if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                        {
                                            DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", Year);
                                            if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                            {
                                                ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                            }
                                            else
                                            {
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                            }
                                        }
                                        else
                                        {
                                            ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                        }
                                    }
                                    else
                                    {
                                        idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == Year).ToArray();

                                        Decimal Counter = Decimal.Zero;
                                        foreach (DataRow item in idrWeeklySubset)
                                        {
                                            Counter = Counter + Convert.ToDecimal(item["PensionHours"]);
                                            if (Counter >= 400.0M)
                                            {
                                                ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                break;
                                            }
                                        }

                                        if (Counter < 400.0M || Counter == Decimal.Zero)
                                            ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                    }

                                    this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                    this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                    iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_1;
                                    AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_1);
                                }

                                else if (astrplan_code == busConstant.IAP)
                                {
                                    if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                    {
                                        //PIR 933
                                        //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                        //lParameters[0] = param1;

                                        //param2.Value = busConstant.IAP;
                                        //lParameters[1] = param2;

                                        //param3.Value = Year;
                                        //lParameters[2] = param3;

                                        //if (ablnSSNMerge == false)
                                        //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                                        //else
                                        //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                        //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                        //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                        //else
                                        //    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);

                                        if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                        {
                                            DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", Year);
                                            if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                            {
                                                ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                            }
                                            else
                                            {
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                            }
                                        }
                                        else
                                        {
                                            ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                        }
                                    }
                                    else
                                    {
                                        idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == Year).ToArray();

                                        Decimal Counter = Decimal.Zero;
                                        foreach (DataRow item in idrWeeklySubset)
                                        {
                                            Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                            if (Counter >= 400.0M)
                                            {
                                                ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                break;
                                            }
                                        }

                                        if (Counter < 400.0M || Counter == Decimal.Zero)
                                            ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                    }

                                    this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                    this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                    iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_1;
                                    AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_1);
                                }

                                return true;
                            }
                        }
                    }
                }
                #endregion

                #region RULE-2
                //RULE-2  Pension and IAP – Effective 01/01/1976
                //  10 Anniversary Years (excluding forfeitures and withdrawals), and
                //  Age 65, and
                //	Not a Break in Service Participant        
                if (VestingRule.eligibility_type_value == busConstant.RULE_2)
                {
                    //lcdoVestingRule = (cdoBenefitProvisionEligibility)iclbAllVestingRules.Where(rule => rule.eligibility_type_value.ToString() == busConstant.RULE_2).First();
                    if (VestingRule.IsNotNull())
                    {
                        if (iclbWorkHistory.Where(item => item.anniversary_years_count >= VestingRule.anniversary_years && this.idecAge >= VestingRule.min_age && item.year == aintWorkItemYear).Count() > 0)
                        {
                            DateTime ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);
                            int YearatAge65 = ldtDateatAge65.Year;
                            int YearofFirstAnnivAfterForfeiture = 0;
                            int lintLatestForfYear = 0;
                            DateTime ldtAnniStartDate = new DateTime();

                            if ((from i in iclbWorkHistory where i.year <= aintWorkItemYear && i.istrForfietureFlag == "Y" orderby i.year descending select i.year).Count() > 0)
                                lintLatestForfYear = (from i in iclbWorkHistory where i.year <= aintWorkItemYear && i.istrForfietureFlag == "Y" orderby i.year descending select i.year).First();

                            DateTime ldtPlanStartDate = new DateTime();
                            if (iclbWorkHistory.IsNotNull() && iclbWorkHistory.Count > 0 && iclbWorkHistory.First().PlanStartDate.IsNotNull())
                                ldtPlanStartDate = iclbWorkHistory.First().PlanStartDate;
                            else
                                ldtPlanStartDate = this.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.istrPlanCode == astrplan_code).First().icdoPersonAccount.start_date;

                            if (lintLatestForfYear > 0)
                            {
                                if (iclbWorkHistory.Where(item => item.year > lintLatestForfYear && item.year <= aintWorkItemYear && item.vested_hours > 1).Any())
                                {
                                    YearofFirstAnnivAfterForfeiture = iclbWorkHistory.Where(item => item.year > lintLatestForfYear && item.year <= aintWorkItemYear && item.vested_hours > 1).First().year;

                                    if (this.idrWeeklyWorkData.IsNotNull() && (this.idrWeeklyWorkData.Where(i => Convert.ToDateTime(i["FromDate"]).Year == YearofFirstAnnivAfterForfeiture).Any()))
                                        ldtAnniStartDate = Convert.ToDateTime((this.idrWeeklyWorkData.Where(i => Convert.ToDateTime(i["FromDate"]).Year == YearofFirstAnnivAfterForfeiture).First())["FromDate"]);

                                    else
                                    {
                                        if (iclbWorkHistory.Where(r => r.year == YearofFirstAnnivAfterForfeiture).Count() > 0)
                                            ldtAnniStartDate = Convert.ToDateTime(iclbWorkHistory.Where(r => r.year == YearofFirstAnnivAfterForfeiture).First().firstHourReported);
                                    }

                                    ldtPlanStartDate = ldtAnniStartDate; //PIR - 856 
                                }
                            }
                            else
                            {
                                ldtAnniStartDate = ldtPlanStartDate;
                            }

                            //PIR 933
                            ////PIR - 856 
                            //DataTable ldtbVestingDateAt400Hours = new DataTable();
                            DateTime ldtVestingDateAt400Hours = new DateTime();
                            //SqlParameter[] lParameters = new SqlParameter[3];
                            //SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                            //SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);
                            //SqlParameter param3 = new SqlParameter("@YEAR", DbType.Int32);

                            //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                            //lParameters[0] = param1;

                            //param2.Value = busConstant.MPIPP;
                            //lParameters[1] = param2;

                            //param3.Value = aintWorkItemYear;
                            //lParameters[2] = param3;

                            //if (ablnSSNMerge == false)
                            //    ldtbVestingDateAt400Hours = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                            //else
                            //    ldtbVestingDateAt400Hours = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                            //if (ldtbVestingDateAt400Hours.Rows.Count > 0 && ldtbVestingDateAt400Hours.Rows[0][0] != DBNull.Value)
                            //    ldtVestingDateAt400Hours = Convert.ToDateTime(ldtbVestingDateAt400Hours.Rows[0][0]);

                            //PIR 933
                            if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                            {
                                DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", aintWorkItemYear);
                                if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                {
                                    ldtVestingDateAt400Hours = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                }
                            }

                            var YearofVesting = from item in iclbWorkHistory
                                                where item.anniversary_years_count >= VestingRule.anniversary_years && item.year >= YearatAge65 && item.year == aintWorkItemYear
                                                     && ((iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).istrBisParticipantFlag != busConstant.FLAG_YES) //PIR - 856 
                                                            || (iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).istrBisParticipantFlag == busConstant.FLAG_YES && ldtVestingDateAt400Hours != DateTime.MinValue))
                                                    && ((item.bis_years_count < 2)
                                                    || (iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).anniversary_years_count >= VestingRule.anniversary_years && iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).istrBisParticipantFlag != busConstant.FLAG_YES)
                                                    || (ldtPlanStartDate.AddYears(aintWorkItemYear - ldtPlanStartDate.Year) < busGlobalFunctions.GetLastDateOfComputationYear(aintWorkItemYear))
                                                    || (ldtAnniStartDate.AddYears(aintWorkItemYear - ldtAnniStartDate.Year) < busGlobalFunctions.GetLastDateOfComputationYear(aintWorkItemYear))
                                                    || (iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).anniversary_years_count >= VestingRule.anniversary_years && iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).bis_years_count >= 2 &&
                                                        iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item)).qualified_hours >= 400))
                                                orderby item.year ascending
                                                select item.year;

                            if (!(YearofVesting.IsNullOrEmpty()))
                            {
                                int Year = (Int32)YearofVesting.First();
                                if (astrplan_code == busConstant.MPIPP)
                                {
                                    //PIR - 856 
                                    if (ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year) < ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year))
                                    {

                                        List<DateTime> lstDateOfVesting = new List<DateTime> { ldtDateatAge65, ldtPlanStartDate.AddYears(VestingRule.anniversary_years), ldtVestingDateAt400Hours };

                                        ldtDateOfVesting = lstDateOfVesting.Max();
                                        ////PIR 548 - Vesting Logic
                                        //ldtDateOfVesting = new DateTime(ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) < ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year) ?
                                        //new DateTime(ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) : ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year);//.AddDays(1); //He Vests at the End of the Day.

                                    }
                                    else
                                    {
                                        //PIR - 856 
                                        List<DateTime> lstDateOfVesting = new List<DateTime> { ldtDateatAge65, ldtAnniStartDate.AddYears(VestingRule.anniversary_years), ldtVestingDateAt400Hours };

                                        ldtDateOfVesting = lstDateOfVesting.Max();
                                        //ldtDateOfVesting = new DateTime(ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) < ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year) ?
                                        // new DateTime(ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) : ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year);
                                    }

                                    //    ldtDateOfVesting = ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year);//.AddDays(1); //He Vests at the End of the Day.
                                    //else
                                    //    ldtDateOfVesting = ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year);

                                    this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;

                                    this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                    iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_2;
                                    AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtMPIVestingDate, astrplan_code, busConstant.RULE_2);
                                }
                                else if (astrplan_code == busConstant.IAP)
                                {
                                    //PIR - 856 
                                    if (ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year) < ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year))
                                    {

                                        List<DateTime> lstDateOfVesting = new List<DateTime> { ldtDateatAge65, ldtPlanStartDate.AddYears(VestingRule.anniversary_years), ldtVestingDateAt400Hours };

                                        ldtDateOfVesting = lstDateOfVesting.Max();
                                        ////PIR 548 - Vesting Logic
                                        //ldtDateOfVesting = new DateTime(ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) < ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year) ?
                                        //new DateTime(ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) : ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year);//.AddDays(1); //He Vests at the End of the Day.

                                    }
                                    else
                                    {
                                        //PIR - 856 
                                        List<DateTime> lstDateOfVesting = new List<DateTime> { ldtDateatAge65, ldtAnniStartDate.AddYears(VestingRule.anniversary_years), ldtVestingDateAt400Hours };

                                        ldtDateOfVesting = lstDateOfVesting.Max();
                                        //ldtDateOfVesting = new DateTime(ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) < ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year) ?
                                        // new DateTime(ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) : ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year);
                                    }
                                    //    ldtDateOfVesting = ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year);//.AddDays(1); //He Vests at the End of the Day.
                                    //else
                                    //    ldtDateOfVesting = ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year);

                                    this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;

                                    this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                    iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_2;
                                    AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtIAPVestingDate, astrplan_code, busConstant.RULE_2);
                                }

                                return true;   //Whereever age comes into picture we need to make sure that "idecAge is already set or else it will be object reference exception"
                            }
                        }
                    }
                }
                #endregion

                #region RULE-3A
                //RULE-3A
                // Pension and IAP – Effective 12/25/1988
                //•	5 Anniversary Years (excluding forfeitures and any service earned prior to December 25, 1988), and 
                //•	Age 65, and 
                //•	Not a Break in Service Participant                
                if (VestingRule.eligibility_type_value == busConstant.RULE_3A)
                {
                    //lcdoVestingRule = (cdoBenefitProvisionEligibility)iclbAllVestingRules.Where(rule => rule.eligibility_type_value.ToString() == busConstant.RULE_3A).First();
                    if (VestingRule.IsNotNull())
                    {
                        int lintAnnivYearCount = 0;
                        int lintLatestForfYear = 0;

                        if ((from i in iclbWorkHistory where i.year >= 1988 & i.year <= aintWorkItemYear && i.istrForfietureFlag == "Y" orderby i.year descending select i.year).Count() > 0)
                            lintLatestForfYear = (from i in iclbWorkHistory where i.year >= 1988 & i.year <= aintWorkItemYear && i.istrForfietureFlag == "Y" orderby i.year descending select i.year).First();

                        if (iclbWorkHistory.Where(x => x.year >= 1988 && x.year <= aintWorkItemYear && x.istrForfietureFlag == "Y").Any())
                            lintAnnivYearCount = 0;
                        else
                        {
                            if ((from i in iclbWorkHistory where i.year == 1988 orderby i.year select i.year).Any())
                                lintAnnivYearCount = (from i in iclbWorkHistory where i.year == 1988 orderby i.year select i.anniversary_years_count + 1).First();
                        }

                        if (iclbWorkHistory.Where(item => item.anniversary_years_count - lintAnnivYearCount >= VestingRule.anniversary_years && this.idecAge >= VestingRule.min_age && item.year > 1988 && item.year == aintWorkItemYear && item.year > lintLatestForfYear).Count() > 0)
                        {
                            DateTime ldtDateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);
                            int YearatAge65 = ldtDateatAge65.Year;
                            //int ThresHoldBisCount = 0;
                            int YearOfConcern = 0;
                            int YearofFirstAnnivAfterForfeiture = 0;
                            DateTime ldtAnniStartDate = new DateTime();
                            DateTime ldtPlanStartDate = new DateTime();
                            int YearatFifthAnnivAfter1988 = 0;
                            if (iclbWorkHistory.Where(item => item.anniversary_years_count - lintAnnivYearCount == VestingRule.anniversary_years && item.year > lintLatestForfYear).Count() > 0)
                            {
                                YearatFifthAnnivAfter1988 = iclbWorkHistory.Where(item => item.anniversary_years_count - lintAnnivYearCount == VestingRule.anniversary_years && item.year > lintLatestForfYear).First().year;
                            }
                            if (YearatFifthAnnivAfter1988 > YearatAge65 && iclbWorkHistory.Where(item => item.year == YearatAge65).Count() > 0)
                            {
                                YearOfConcern = YearatFifthAnnivAfter1988;
                            }
                            else
                            {
                                YearOfConcern = YearatAge65;
                            }

                            if (iclbWorkHistory.IsNotNull() && iclbWorkHistory.Count > 0 && iclbWorkHistory.First().PlanStartDate.IsNotNull())
                                ldtPlanStartDate = iclbWorkHistory.First().PlanStartDate;
                            else
                                ldtPlanStartDate = this.ibusPerson.iclbPersonAccount.Where(i => i.icdoPersonAccount.istrPlanCode == astrplan_code).First().icdoPersonAccount.start_date;

                            if (lintAnnivYearCount == 0)
                            {
                                if (iclbWorkHistory.Where(item => item.year > lintLatestForfYear && item.year <= aintWorkItemYear && item.vested_hours > 1).Any())
                                {
                                    YearofFirstAnnivAfterForfeiture = iclbWorkHistory.Where(item => item.year > lintLatestForfYear && item.year <= aintWorkItemYear && item.vested_hours > 1).First().year;

                                    if (this.idrWeeklyWorkData.IsNotNull() && (this.idrWeeklyWorkData.Where(i => Convert.ToDateTime(i["FromDate"]).Year == YearofFirstAnnivAfterForfeiture).Any()))
                                        ldtAnniStartDate = Convert.ToDateTime((this.idrWeeklyWorkData.Where(i => Convert.ToDateTime(i["FromDate"]).Year == YearofFirstAnnivAfterForfeiture).First())["FromDate"]);
                                    else
                                    {
                                        if (iclbWorkHistory.Where(r => r.year == YearofFirstAnnivAfterForfeiture).Count() > 0)
                                            ldtAnniStartDate = Convert.ToDateTime(iclbWorkHistory.Where(r => r.year == YearofFirstAnnivAfterForfeiture).First().firstHourReported);
                                    }

                                    ldtPlanStartDate = ldtAnniStartDate; //PIR - 856 
                                }
                            }
                            else
                            {
                                YearofFirstAnnivAfterForfeiture = ldtPlanStartDate.Year;
                                ldtAnniStartDate = ldtPlanStartDate;
                            }

                            //PIR 933
                            ////PIR - 856 
                            //DataTable ldtbVestingDateAt400Hours = new DataTable();
                            DateTime ldtVestingDateAt400Hours = new DateTime();
                            //SqlParameter[] lParameters = new SqlParameter[3];
                            //SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                            //SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);
                            //SqlParameter param3 = new SqlParameter("@YEAR", DbType.Int32);

                            //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                            //lParameters[0] = param1;

                            //param2.Value = busConstant.MPIPP;
                            //lParameters[1] = param2;

                            //param3.Value = aintWorkItemYear;
                            //lParameters[2] = param3;

                            //if (ablnSSNMerge == false)
                            //    ldtbVestingDateAt400Hours = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                            //else
                            //    ldtbVestingDateAt400Hours = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                            //if (ldtbVestingDateAt400Hours.Rows.Count > 0 && ldtbVestingDateAt400Hours.Rows[0][0] != DBNull.Value)
                            //    ldtVestingDateAt400Hours = Convert.ToDateTime(ldtbVestingDateAt400Hours.Rows[0][0]);

                            //PIR 933
                            if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                            {
                                DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", aintWorkItemYear);
                                if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                {
                                    ldtVestingDateAt400Hours = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                }
                            }

                            var YearofVesting = from item in iclbWorkHistory
                                                where item.anniversary_years_count - lintAnnivYearCount >= VestingRule.anniversary_years && item.year >= YearOfConcern && item.year > 1988 && item.year == aintWorkItemYear //&& ThresHoldBisCount < 2
                                                    && ((iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).istrBisParticipantFlag != busConstant.FLAG_YES) //PIR - 856 
                                                            || (iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).istrBisParticipantFlag == busConstant.FLAG_YES && ldtVestingDateAt400Hours != DateTime.MinValue))
                                                    && ((item.bis_years_count < 2)
                                                    || (iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).anniversary_years_count - lintAnnivYearCount >= VestingRule.anniversary_years && iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).istrBisParticipantFlag != busConstant.FLAG_YES)
                                                    || (ldtPlanStartDate.AddYears(aintWorkItemYear - ldtPlanStartDate.Year) < busGlobalFunctions.GetLastDateOfComputationYear(aintWorkItemYear))
                                                    || (ldtAnniStartDate.AddYears(aintWorkItemYear - ldtAnniStartDate.Year) < busGlobalFunctions.GetLastDateOfComputationYear(aintWorkItemYear))
                                                    || (iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).anniversary_years_count - lintAnnivYearCount >= VestingRule.anniversary_years && iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item) - 1).bis_years_count >= 2 &&
                                                        iclbWorkHistory.ElementAt(iclbWorkHistory.IndexOf(item)).qualified_hours >= 400))
                                                orderby item.year ascending
                                                select item.year;

                            if (!(YearofVesting.IsNullOrEmpty()))
                            {
                                int Year = (Int32)YearofVesting.First();
                                if (astrplan_code == busConstant.MPIPP)
                                {
                                    //PIR - 856 
                                    if (ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year) < ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year))
                                    {

                                        List<DateTime> lstDateOfVesting = new List<DateTime> { ldtDateatAge65, ldtPlanStartDate.AddYears(VestingRule.anniversary_years), ldtVestingDateAt400Hours };

                                        ldtDateOfVesting = lstDateOfVesting.Max();
                                        ////PIR 548 - Vesting Logic
                                        //ldtDateOfVesting = new DateTime(ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) < ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year) ?
                                        //new DateTime(ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) : ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year);//.AddDays(1); //He Vests at the End of the Day.

                                    }
                                    else
                                    {
                                        List<DateTime> lstDateOfVesting = new List<DateTime> { ldtDateatAge65, ldtAnniStartDate.AddYears(VestingRule.anniversary_years), ldtVestingDateAt400Hours };

                                        ldtDateOfVesting = lstDateOfVesting.Max();
                                        //ldtDateOfVesting = new DateTime(ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) < ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year) ?
                                        // new DateTime(ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) : ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year);
                                    }

                                    //if (iclbWorkHistory.Where(item => item.year == aintWorkItemYear).First().iintAgetoShow > 65)
                                    this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                    //else if (iclbWorkHistory.Where(item => item.year == aintWorkItemYear).First().anniversary_years_count > 5)
                                    //    this.icdoBenefitApplication.adtMPIVestingDate = ldtDateatAge65;
                                    //else
                                    //{
                                    //    if (ldtDateOfVesting < ldtDateatAge65)
                                    //        this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                    //    else
                                    //        this.icdoBenefitApplication.adtMPIVestingDate = ldtDateatAge65;
                                    //}

                                    this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                    iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_3A;
                                    AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtMPIVestingDate, astrplan_code, busConstant.RULE_3A);
                                }
                                else if (astrplan_code == busConstant.IAP)
                                {
                                    //PIR 548 - Vesting Logic
                                    //PIR - 856 
                                    if (ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year) < ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year))
                                    {

                                        List<DateTime> lstDateOfVesting = new List<DateTime> { ldtDateatAge65, ldtPlanStartDate.AddYears(VestingRule.anniversary_years), ldtVestingDateAt400Hours };

                                        ldtDateOfVesting = lstDateOfVesting.Max();
                                        ////PIR 548 - Vesting Logic
                                        //ldtDateOfVesting = new DateTime(ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) < ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year) ?
                                        //new DateTime(ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) : ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year);//.AddDays(1); //He Vests at the End of the Day.

                                    }
                                    else
                                    {
                                        List<DateTime> lstDateOfVesting = new List<DateTime> { ldtDateatAge65, ldtAnniStartDate.AddYears(VestingRule.anniversary_years), ldtVestingDateAt400Hours };

                                        ldtDateOfVesting = lstDateOfVesting.Max();
                                        //ldtDateOfVesting = new DateTime(ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) < ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year) ?
                                        // new DateTime(ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year).Year, ldtDateatAge65.Month, ldtDateatAge65.Day) : ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year);
                                    }
                                    //    ldtDateOfVesting = ldtPlanStartDate.AddYears(Year - ldtPlanStartDate.Year);//.AddDays(1); //He Vests at the End of the Day.
                                    //else
                                    //    ldtDateOfVesting = ldtAnniStartDate.AddYears(Year - ldtAnniStartDate.Year);

                                    //if (iclbWorkHistory.Where(item => item.year == aintWorkItemYear).First().iintAgetoShow > 65)
                                    this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                    //else if (iclbWorkHistory.Where(item => item.year == aintWorkItemYear).First().anniversary_years_count > 5)
                                    //    this.icdoBenefitApplication.adtIAPVestingDate = ldtDateatAge65;
                                    //else
                                    //{
                                    //    if (ldtDateOfVesting < ldtDateatAge65)
                                    //        this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                    //    else
                                    //        this.icdoBenefitApplication.adtIAPVestingDate = ldtDateatAge65;
                                    //}

                                    this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                    iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_3A;
                                    AddUpdatePersonAccountEligibility(this.icdoBenefitApplication.adtIAPVestingDate, astrplan_code, busConstant.RULE_3A);
                                }

                                return true;   //Whereever age comes into picture we need to make sure that "idecAge is already set or else it will be object reference exception"
                            }
                        }
                    }
                }
                #endregion

                #region RULE-3B
                //RULE-3B
                //•	5 Qualified Years (excluding forfeitures), and 
                //•	Age 65 , and 
                //•	If participant is a Break in Service Participant as of age 65, they must earn 40 credited hours within a computation year after their 65th birthday.
                if (VestingRule.eligibility_type_value == busConstant.RULE_3B)
                {
                    //lcdoVestingRule = (cdoBenefitProvisionEligibility)iclbAllVestingRules.Where(rule => rule.eligibility_type_value.ToString() == busConstant.RULE_3B).FirstOrDefault();
                    if (VestingRule.IsNotNull())
                    {
                        if (iclbWorkHistory.Where(item => item.qualified_years_count >= VestingRule.qualified_years && this.idecAge >= VestingRule.min_age && item.year == aintWorkItemYear).Count() > 0)
                        {
                            DataRow[] idrWeeklySubset;
                            DateTime DateatAge65 = this.ibusPerson.icdoPerson.idtDateofBirth.AddYears(65);

                            DateTime ComputationEndDateforYearofAge65 = busGlobalFunctions.GetLastDateOfComputationYear(DateatAge65.Year);

                            Collection<string> BIScount = new Collection<string>();

                            if (aintWorkItemYear > DateatAge65.Year)
                            {
                                BIScount.Add(busConstant.FLAG_NO);
                            }
                            else
                            {
                                if (DateatAge65 > ComputationEndDateforYearofAge65)
                                    BIScount = (from item in iclbWorkHistory where item.year == DateatAge65.Year select item.istrBisParticipantFlag).ToList().ToCollection();
                                else
                                    BIScount = (from item in iclbWorkHistory where item.year == DateatAge65.Year - 1 select item.istrBisParticipantFlag).ToList().ToCollection();
                            }

                            int Year = 0;

                            if (aintWorkItemYear > DateatAge65.Year)
                            {
                                Year = aintWorkItemYear;
                            }
                            else
                            {
                                if (DateatAge65 > ComputationEndDateforYearofAge65)
                                    Year = DateatAge65.Year + 1;
                                else
                                    Year = DateatAge65.Year;
                            }

                            if (!(BIScount.IsNullOrEmpty()) && BIScount.First().ToString() == busConstant.FLAG_YES) //This Signifies that he was a Break-In-Service Participant at the Age of 65
                            {
                                #region Participant is in BIS
                                SqlParameter[] lParameters = new SqlParameter[4];
                                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                                SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);
                                SqlParameter param3 = new SqlParameter("@YEAR", DbType.Int32);
                                SqlParameter param4 = new SqlParameter("@DATE_AT_AGE_65", DbType.DateTime);

                                if (astrplan_code == busConstant.MPIPP)
                                {
                                    #region MPIPP
                                    if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                    {
                                        param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                        lParameters[0] = param1;

                                        param2.Value = busConstant.MPIPP;
                                        lParameters[1] = param2;

                                        param3.Value = Year;
                                        lParameters[2] = param3;

                                        param4.Value = DateatAge65;
                                        lParameters[3] = param4;

                                        DataTable ldtbDate = new DataTable();
                                        if (ablnSSNMerge == false)
                                            ldtbDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursAftDateOfBirth", astrLegacyDBConnetion, null, lParameters);
                                        else
                                            ldtbDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursAftDateOfBirth", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                        if (ldtbDate.Rows.Count > 0 && ldtbDate.Rows[0][0] != DBNull.Value)
                                            ldtDateOfVesting = Convert.ToDateTime(ldtbDate.Rows[0][0]);
                                    }
                                    else
                                    {
                                        idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == Year && Convert.ToDateTime(i["FromDate"]) >= DateatAge65).ToArray();

                                        Decimal Counter = Decimal.Zero;
                                        foreach (DataRow item in idrWeeklySubset)
                                        {
                                            Counter = Counter + Convert.ToDecimal(item["PensionHours"]);
                                            if (Counter >= 40.0M)
                                            {
                                                ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                break;
                                            }
                                        }

                                    }
                                    #endregion
                                }

                                else if (astrplan_code == busConstant.IAP)
                                {
                                    #region IAP
                                    if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                    {
                                        param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                        lParameters[0] = param1;

                                        param2.Value = busConstant.IAP;
                                        lParameters[1] = param2;

                                        param3.Value = Year;
                                        lParameters[2] = param3;

                                        param4.Value = DateatAge65;
                                        lParameters[3] = param4;

                                        DataTable ldtbDate = new DataTable();
                                        if (ablnSSNMerge == false)
                                            ldtbDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursAftDateOfBirth", astrLegacyDBConnetion, null, lParameters);
                                        else
                                            ldtbDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursAftDateOfBirth", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                        if (ldtbDate.Rows.Count > 0 && ldtbDate.Rows[0][0] != DBNull.Value)
                                            ldtDateOfVesting = Convert.ToDateTime(ldtbDate.Rows[0][0]);
                                    }
                                    else
                                    {
                                        idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == Year && Convert.ToDateTime(i["FromDate"]) >= DateatAge65).ToArray();

                                        Decimal Counter = Decimal.Zero;
                                        foreach (DataRow item in idrWeeklySubset)
                                        {
                                            Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                            if (Counter >= 40.0M)
                                            {
                                                ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                break;
                                            }
                                        }

                                    }

                                    #endregion
                                }

                                if (ldtDateOfVesting.IsNotNull() && ldtDateOfVesting != DateTime.MinValue && iclbWorkHistory.Where(item => item.qualified_years_count >= VestingRule.qualified_years && this.idecAge >= VestingRule.min_age && item.year >= ldtDateOfVesting.Year && item.year == aintWorkItemYear).Count() > 0)
                                {
                                    #region IF He Earned 40 hours within same Computation Year - We need to Find Vesting Date
                                    if (astrplan_code == busConstant.MPIPP)
                                    {
                                        this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                        this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                        iclbWorkHistory.Where(item => item.year == ldtDateOfVesting.Year).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_3B;
                                        AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_3B);
                                    }
                                    else if (astrplan_code == busConstant.IAP)
                                    {
                                        this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                        this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                        iclbWorkHistory.Where(item => item.year == ldtDateOfVesting.Year).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_3B;
                                        AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_3B);
                                    }
                                    return true;
                                    #endregion
                                }

                                else
                                {
                                    #region Find Out if He Earned 40 HOURS in Next Years POST his/her 65th birthday
                                    var YearofVesting = from item in iclbWorkHistory where item.year > Year && item.qualified_hours >= VestingRule.credited_hours && item.qualified_years_count >= VestingRule.qualified_years && item.year == aintWorkItemYear orderby item.year ascending select item.year;

                                    if (!(YearofVesting.IsNullOrEmpty()))
                                    {
                                        int Yearof40Hours = (Int32)YearofVesting.First();
                                        SqlParameter[] lParameters40Hours = new SqlParameter[3];

                                        if (astrplan_code == busConstant.MPIPP)
                                        {
                                            if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                            {
                                                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                                lParameters40Hours[0] = param1;

                                                param2.Value = busConstant.MPIPP;
                                                lParameters40Hours[1] = param2;

                                                param3.Value = Yearof40Hours;
                                                lParameters40Hours[2] = param3;

                                                if (ablnSSNMerge == false)
                                                    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursInGivenYear", astrLegacyDBConnetion, null, lParameters40Hours);
                                                else
                                                    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursInGivenYear", astrLegacyDBConnetion, aobjPassInfo, lParameters40Hours);

                                                if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                                    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);

                                            }
                                            else
                                            {
                                                idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == Yearof40Hours).ToArray();

                                                Decimal Counter = Decimal.Zero;
                                                foreach (DataRow item in idrWeeklySubset)
                                                {
                                                    Counter = Counter + Convert.ToDecimal(item["PensionHours"]);
                                                    if (Counter >= 40.0M)
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                        break;
                                                    }
                                                }

                                            }

                                            if (ldtDateOfVesting.IsNotNull() && ldtDateOfVesting != DateTime.MinValue)
                                            {
                                                this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                                iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_3B;
                                                this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                                AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_3B);
                                                return true;
                                            }
                                        }
                                        else if (astrplan_code == busConstant.IAP)
                                        {
                                            if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                            {
                                                param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                                lParameters40Hours[0] = param1;

                                                param2.Value = busConstant.IAP;
                                                lParameters40Hours[1] = param2;

                                                param3.Value = Yearof40Hours;
                                                lParameters40Hours[2] = param3;

                                                if (ablnSSNMerge == false)
                                                    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursInGivenYear", astrLegacyDBConnetion, null, lParameters40Hours);
                                                else
                                                    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursInGivenYear", astrLegacyDBConnetion, aobjPassInfo, lParameters40Hours);

                                                if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                                    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                            }
                                            else
                                            {
                                                idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == Yearof40Hours).ToArray();

                                                Decimal Counter = Decimal.Zero;
                                                foreach (DataRow item in idrWeeklySubset)
                                                {
                                                    Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                                    if (Counter >= 40.0M)
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                        break;
                                                    }
                                                }

                                            }

                                            if (ldtDateOfVesting.IsNotNull() && ldtDateOfVesting != DateTime.MinValue)
                                            {
                                                this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                                iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_3B;
                                                this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                                AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_3B);
                                                return true;
                                            }

                                        }
                                    }

                                    #endregion

                                    #region Find Out if He Earned 400 HOURS after his BIS including the Year of HIS Birthday
                                    YearofVesting = from item in iclbWorkHistory where item.year >= Year && item.qualified_hours >= VestingRule.credited_hours && item.qualified_years_count >= VestingRule.qualified_years && item.year == aintWorkItemYear orderby item.year ascending select item.year;

                                    if (!(YearofVesting.IsNullOrEmpty()))
                                    {
                                        int Yearof400Hours = (Int32)YearofVesting.First();
                                        SqlParameter[] lParameters400Hours = new SqlParameter[3];

                                        if (astrplan_code == busConstant.MPIPP)
                                        {
                                            if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                            {
                                                //PIR 933
                                                //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                                //lParameters400Hours[0] = param1;

                                                //param2.Value = busConstant.MPIPP;
                                                //lParameters400Hours[1] = param2;

                                                //param3.Value = Yearof400Hours;
                                                //lParameters400Hours[2] = param3;

                                                //if (ablnSSNMerge == false)
                                                //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters400Hours);
                                                //else
                                                //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters400Hours);

                                                //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                                //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);

                                                //PIR 933
                                                if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                                {
                                                    DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", Yearof400Hours);
                                                    if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == Yearof400Hours).ToArray();

                                                Decimal Counter = Decimal.Zero;
                                                foreach (DataRow item in idrWeeklySubset)
                                                {
                                                    Counter = Counter + Convert.ToDecimal(item["PensionHours"]);
                                                    if (Counter >= 400.0M)
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                        break;
                                                    }
                                                }

                                            }

                                            if (ldtDateOfVesting.IsNotNull() && ldtDateOfVesting != DateTime.MinValue)
                                            {
                                                this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                                iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_3B;
                                                this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                                AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_3B);
                                                return true;
                                            }
                                        }
                                        else if (astrplan_code == busConstant.IAP)
                                        {
                                            if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                            {
                                                //PIR 933
                                                //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                                //lParameters400Hours[0] = param1;

                                                //param2.Value = busConstant.IAP;
                                                //lParameters400Hours[1] = param2;

                                                //param3.Value = Year;
                                                //lParameters400Hours[2] = param3;

                                                //if (ablnSSNMerge == false)
                                                //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters400Hours);
                                                //else
                                                //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters400Hours);

                                                //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                                //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);

                                                //PIR 933
                                                if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                                {
                                                    DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", Year);
                                                    if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == Yearof400Hours).ToArray();

                                                Decimal Counter = Decimal.Zero;
                                                foreach (DataRow item in idrWeeklySubset)
                                                {
                                                    Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                                    if (Counter >= 400.0M)
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                        break;
                                                    }
                                                }

                                            }

                                            if (ldtDateOfVesting.IsNotNull() && ldtDateOfVesting != DateTime.MinValue)
                                            {
                                                this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                                iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_3B;
                                                this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                                AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_3B);
                                                return true;
                                            }
                                        }
                                    }

                                    #endregion
                                }
                                #endregion
                            }
                            else if (iclbWorkHistory.Where(item => item.year == aintWorkItemYear).First().istrForfietureFlag.IsNotNullOrEmpty() && iclbWorkHistory.Where(item => item.year == aintWorkItemYear).First().istrForfietureFlag != busConstant.FLAG_YES)
                            {
                                var YearofVesting = from item in iclbWorkHistory where item.qualified_years_count >= VestingRule.qualified_years && item.year >= DateatAge65.Year && item.year == aintWorkItemYear orderby item.year ascending select item.year;

                                #region Participant is not in BIS as of his 65th Year
                                if (!(YearofVesting.IsNullOrEmpty()))
                                {
                                    int YearofV = (Int32)YearofVesting.First();
                                    SqlParameter[] lParameters = new SqlParameter[3];
                                    SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                                    SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);
                                    SqlParameter param3 = new SqlParameter("@YEAR", DbType.Int32);

                                    if (astrplan_code == busConstant.MPIPP)
                                    {
                                        if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                        {
                                            //PIR 933
                                            //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                            //lParameters[0] = param1;

                                            //param2.Value = busConstant.MPIPP;
                                            //lParameters[1] = param2;

                                            //param3.Value = YearofV;
                                            //lParameters[2] = param3;

                                            //if (ablnSSNMerge == false)
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                                            //else
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);


                                            //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                            //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                            //else
                                            //    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);

                                            if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                            {
                                                DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", YearofV);
                                                if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                                }
                                                else
                                                {
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                                }
                                            }
                                            else
                                            {
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                            }
                                        }
                                        else
                                        {
                                            idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == YearofV).ToArray();

                                            Decimal Counter = Decimal.Zero;
                                            foreach (DataRow item in idrWeeklySubset)
                                            {
                                                Counter = Counter + Convert.ToDecimal(item["PensionHours"]);
                                                if (Counter >= 400.0M)
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                    break;
                                                }
                                            }

                                            if (Counter < 400.0M || Counter == Decimal.Zero)
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                        }

                                        this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                        iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_3B;
                                        this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                        AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_3B);
                                    }
                                    else if (astrplan_code == busConstant.IAP)
                                    {
                                        if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                        {
                                            //PIR 933
                                            //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                            //lParameters[0] = param1;

                                            //param2.Value = busConstant.IAP;
                                            //lParameters[1] = param2;

                                            //param3.Value = YearofV;
                                            //lParameters[2] = param3;

                                            //if (ablnSSNMerge == true)
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                                            //else
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                            //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                            //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                            //else
                                            //    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);

                                            if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                            {
                                                DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", YearofV);
                                                if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                                }
                                                else
                                                {
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                                }
                                            }
                                            else
                                            {
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                            }
                                        }
                                        else
                                        {
                                            idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == YearofV).ToArray();

                                            Decimal Counter = Decimal.Zero;
                                            foreach (DataRow item in idrWeeklySubset)
                                            {
                                                Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                                if (Counter >= 400.0M)
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                    break;
                                                }
                                            }

                                            if (Counter < 400.0M || Counter == Decimal.Zero)
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                        }

                                        this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                        iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_3B;
                                        this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                        AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_3B);
                                    }

                                    return true;   //Whereever age comes into picture we need to make sure that "idecAge is already set or else it will be object reference exception"
                                }
                                #endregion
                            }
                        }
                    }
                }
                #endregion

                #region RULE-4
                //RULE-4 for NON-AFFILIATE PEOPLE
                //Rule 4 – Pension and IAP for Non-Affiliate Participants – Effective 12/24/1989
                // 5 Qualified Years (excluding forfeitures), and
                // At least 40 credited hours earned as a Non-affiliate participant after December 23, 1989.
                //	Non- Affiliate participants will beali identified by union code (Details will be included in the OPUS Sprint 1.0 Integration Specification Document): 
                //	09 – General Non-affiliated Employees
                //	59 – Designated Production Accountants
                //	79 – Designated Producers, Executive and Associate
                //	89 – Freelance Post-Production Supervisor Group Designation
                //	99 – Named Employers & Union Office Staffs                

                if (VestingRule.eligibility_type_value == busConstant.RULE_4)
                {
                    if (astrplan_code == busConstant.MPIPP)
                    {
                        //lcdoVestingRule = (cdoBenefitProvisionEligibility)iclbAllVestingRules.Where(rule => rule.eligibility_type_value.ToString() == busConstant.RULE_4).First();
                        DataRow[] idrWeeklySubset;
                        if (VestingRule.IsNotNull())
                        {
                            if (iclbWorkHistory.Where(item => item.vested_years_count >= VestingRule.vested_years && item.year == aintWorkItemYear).Count() > 0)
                            {
                                if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                {
                                    SqlParameter[] lParametersNA = new SqlParameter[2];
                                    SqlParameter param1NA = new SqlParameter("@SSN", DbType.String);
                                    SqlParameter param2NA = new SqlParameter("@PLANCODE", DbType.String);

                                    param1NA.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                    lParametersNA[0] = param1NA;

                                    param2NA.Value = busConstant.MPIPP;
                                    lParametersNA[1] = param2NA;

                                    DataTable ldtb40HoursasNA = new DataTable();
                                    if (ablnSSNMerge == false)
                                        ldtb40HoursasNA = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursNonAffliate", astrLegacyDBConnetion, null, lParametersNA);
                                    else
                                        ldtb40HoursasNA = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursNonAffliate", astrLegacyDBConnetion, aobjPassInfo, lParametersNA);

                                    if (ldtb40HoursasNA.Rows.Count > 0 && ldtb40HoursasNA.Rows[0][0] != DBNull.Value)
                                    {
                                        DateTime DateofGetting40Hours = Convert.ToDateTime(ldtb40HoursasNA.Rows[0][0]);

                                        var YeartoCompare = from item in iclbWorkHistory where item.vested_years_count >= VestingRule.vested_years && item.year >= DateofGetting40Hours.Year && item.year == aintWorkItemYear orderby item.year ascending select item.year;

                                        if (!YeartoCompare.IsNullOrEmpty())
                                        {
                                            if (DateofGetting40Hours.Year == YeartoCompare.First())
                                            {
                                                ldtDateOfVesting = DateofGetting40Hours;
                                            }
                                            else
                                            {
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YeartoCompare.First());
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    //change - Rohan
                                    DateTime DateofGetting40Hours = DateTime.MinValue;
                                    idrWeeklySubset = this.idrWeeklyWorkData.Where(i => i["FromDate"] != DBNull.Value && i["UnionCode"] != DBNull.Value && Convert.ToDateTime(i["FromDate"]) >= new DateTime(1989, 12, 23) &&
                                                                   (Convert.ToInt32(i["UnionCode"]) == 9 || Convert.ToInt32(i["UnionCode"]) == 59 || Convert.ToInt32(i["UnionCode"]) == 79 || Convert.ToInt32(i["UnionCode"]) == 89 || Convert.ToInt32(i["UnionCode"]) == 99)).ToArray();

                                    Decimal Counter = Decimal.Zero;
                                    foreach (DataRow item in idrWeeklySubset)
                                    {
                                        Counter = Counter + Convert.ToDecimal(item["PensionHours"]);
                                        if (Counter >= 40.0M)
                                        {
                                            DateofGetting40Hours = Convert.ToDateTime(item["ToDate"]);
                                            if (DateofGetting40Hours.Year < aintWorkItemYear)
                                                DateofGetting40Hours = busGlobalFunctions.GetLastDateOfComputationYear(aintWorkItemYear);

                                            break;
                                        }
                                    }

                                    //change - Rohan
                                    var YeartoCompare = from item in iclbWorkHistory where item.vested_years_count >= VestingRule.vested_years && item.year >= DateofGetting40Hours.Year && item.year == aintWorkItemYear orderby item.year ascending select item.year;

                                    if (!YeartoCompare.IsNullOrEmpty())
                                    {
                                        if (DateofGetting40Hours.Year == YeartoCompare.First())
                                        {
                                            ldtDateOfVesting = DateofGetting40Hours;
                                        }
                                        else
                                        {
                                            ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YeartoCompare.First());
                                        }
                                    }


                                }

                                if (ldtDateOfVesting.IsNotNull() && ldtDateOfVesting != DateTime.MinValue)
                                {
                                    this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                    iclbWorkHistory.Where(item => item.year == ldtDateOfVesting.Year).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_4;
                                    this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                    AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_4);
                                    return true;
                                }

                            }
                        }

                    }
                    else if (astrplan_code == busConstant.IAP)
                    {
                        DataRow[] idrWeeklySubset;
                        VestingRule = (cdoBenefitProvisionEligibility)iclbAllVestingRules.Where(rule => rule.eligibility_type_value.ToString() == busConstant.RULE_4).First();
                        if (VestingRule.IsNotNull())
                        {
                            if (iclbWorkHistory.Where(item => item.vested_years_count >= VestingRule.vested_years && item.year == aintWorkItemYear).Count() > 0)
                            {
                                if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                {
                                    SqlParameter[] lParametersNA = new SqlParameter[2];
                                    SqlParameter param1NA = new SqlParameter("@SSN", DbType.String);
                                    SqlParameter param2NA = new SqlParameter("@PLANCODE", DbType.String);

                                    param1NA.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                    lParametersNA[0] = param1NA;

                                    param2NA.Value = busConstant.MPIPP;
                                    lParametersNA[1] = param2NA;

                                    DataTable ldtb40HoursasNA = new DataTable();

                                    if (ablnSSNMerge == false)
                                        ldtb40HoursasNA = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursNonAffliate", astrLegacyDBConnetion, null, lParametersNA);
                                    else
                                        ldtb40HoursasNA = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate40HoursNonAffliate", astrLegacyDBConnetion, aobjPassInfo, lParametersNA);

                                    if (ldtb40HoursasNA.Rows.Count > 0 && ldtb40HoursasNA.Rows[0][0] != DBNull.Value)
                                    {
                                        DateTime DateofGetting40Hours = Convert.ToDateTime(ldtb40HoursasNA.Rows[0][0]);

                                        var YeartoCompare = from item in iclbWorkHistory where item.vested_years_count >= VestingRule.vested_years && item.year >= DateofGetting40Hours.Year && item.year == aintWorkItemYear orderby item.year ascending select item.year;

                                        if (!YeartoCompare.IsNullOrEmpty())
                                        {
                                            if (DateofGetting40Hours.Year == YeartoCompare.First())
                                            {
                                                ldtDateOfVesting = DateofGetting40Hours;
                                            }
                                            else
                                            {
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YeartoCompare.First());
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //change - Rohan
                                    DateTime DateofGetting40Hours = DateTime.MinValue;
                                    idrWeeklySubset = this.idrWeeklyWorkData.Where(i => i["FromDate"] != DBNull.Value && i["UnionCode"] != DBNull.Value && Convert.ToDateTime(i["FromDate"]) >= new DateTime(1989, 12, 23) && i["UnionCode"] != DBNull.Value &&
                                                                   (Convert.ToInt32(i["UnionCode"]) == 9 || Convert.ToInt32(i["UnionCode"]) == 59 || Convert.ToInt32(i["UnionCode"]) == 79 || Convert.ToInt32(i["UnionCode"]) == 89 || Convert.ToInt32(i["UnionCode"]) == 99)).ToArray();

                                    Decimal Counter = Decimal.Zero;
                                    foreach (DataRow item in idrWeeklySubset)
                                    {
                                        Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                        if (Counter >= 40.0M)
                                        {
                                            DateofGetting40Hours = Convert.ToDateTime(item["ToDate"]);
                                            if (DateofGetting40Hours.Year < aintWorkItemYear)
                                                DateofGetting40Hours = busGlobalFunctions.GetLastDateOfComputationYear(aintWorkItemYear);
                                            break;
                                        }
                                    }
                                    //change - Rohan
                                    var YeartoCompare = from item in iclbWorkHistory where item.vested_years_count >= VestingRule.vested_years && item.year >= DateofGetting40Hours.Year && item.year == aintWorkItemYear orderby item.year ascending select item.year;

                                    if (!YeartoCompare.IsNullOrEmpty())
                                    {
                                        if (DateofGetting40Hours.Year == YeartoCompare.First())
                                        {
                                            ldtDateOfVesting = DateofGetting40Hours;
                                        }
                                        else
                                        {
                                            ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YeartoCompare.First());
                                        }
                                    }
                                }

                                if (ldtDateOfVesting.IsNotNull() && ldtDateOfVesting != DateTime.MinValue)
                                {
                                    this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.Flag_Yes;

                                    if (iclbWorkHistory != null)
                                    {
                                        iclbWorkHistory.Where(item => item.year == ldtDateOfVesting.Year).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_4;
                                    }

                                    this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                    AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_4);
                                    return true;
                                }

                            }

                        }
                    }
                }
                #endregion

                #region RULE-5
                // RULE-5 – Pension and IAP – Effective 12/26/1999
                //•	5 Vested Years (excluding forfeitures) including 1 Vested Hour on or after 12/26/1999, 
                //•	If participant has a Break in Service prior to completion of 1 Vested Hour after 12/26/1999, they will not be vested 
                //until they earn one Vested Year after 12/25/1999 and are credited with five Vested Years.
                if (VestingRule.eligibility_type_value == busConstant.RULE_5)
                {
                    //lcdoVestingRule = (cdoBenefitProvisionEligibility)iclbAllVestingRules.Where(rule => rule.eligibility_type_value.ToString() == busConstant.RULE_5).First();
                    if (iclbWorkHistory.Where(item => item.vested_years_count >= VestingRule.vested_years && item.year == aintWorkItemYear).Count() > 0)
                    {
                        DataRow[] idrWeeklySubset;
                        //Query to get Date (Year) where Person's  got MPI hours >=1 and where date >= 12/25/1999
                        //Essentially Query EADB to find out when did the Person got his first Hours on or after DATE 12/25/1999
                        DateTime CompareDate = new DateTime();

                        SqlParameter[] lParams = new SqlParameter[3];
                        SqlParameter param1Test = new SqlParameter("@SSN", DbType.String);
                        SqlParameter param2Test = new SqlParameter("@PlanCode", DbType.String);
                        SqlParameter param3Test = new SqlParameter("@GivenDate", DbType.DateTime);

                        param1Test.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                        lParams[0] = param1Test;

                        param2Test.Value = astrplan_code;
                        lParams[1] = param2Test;

                        param3Test.Value = new DateTime(1999, 12, 25);
                        lParams[2] = param3Test;


                        int Year = 0;
                        int BISCheckYear = 0;

                        if (this.idrWeeklyWorkData.IsNullOrEmpty())
                        {
                            if (ablnSSNMerge == false)
                                ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate1HourAftGivenDate", astrLegacyDBConnetion, null, lParams);
                            else
                                ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate1HourAftGivenDate", astrLegacyDBConnetion, aobjPassInfo, lParams);
                        }
                        else
                        {
                            idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToDateTime(i["FromDate"]) >= new DateTime(1999, 12, 25)).ToArray();

                            Decimal Counter = Decimal.Zero;
                            foreach (DataRow item in idrWeeklySubset)
                            {
                                if (astrplan_code == busConstant.MPIPP)
                                    Counter = Counter + Convert.ToDecimal(item["PensionHours"]);
                                else
                                    Counter = Counter + Convert.ToDecimal(item["IAPHours"]);

                                if (Counter >= 1.0M)
                                {
                                    ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                    //ldtbVestingDate.Rows.Add(item);
                                    break;
                                }
                            }
                        }

                        if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                            CompareDate = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                        else if (ldtDateOfVesting.IsNotNull() && ldtDateOfVesting != DateTime.MinValue)
                            CompareDate = ldtDateOfVesting;

                        if (CompareDate.IsNotNull() && CompareDate != DateTime.MinValue)
                        {
                            if (aintWorkItemYear > CompareDate.Year)
                                Year = aintWorkItemYear;
                            else
                                Year = CompareDate.Year;

                            BISCheckYear = Year - 1;
                        }

                        else
                        {
                            ldtbVestingDate = busBase.Select("cdoPersonBridgeHoursDetail.Get1VestedHourYearAfter1999", new object[1] { this.ibusPerson.icdoPerson.person_id });

                            if (ldtbVestingDate.Rows.Count > 0)
                            {
                                Year = Convert.ToInt32(ldtbVestingDate.Rows[0][0]);
                                BISCheckYear = Year--;
                            }
                        }



                        //SO COMPAREDATE == RESULT DATE OF THE QUERY                        
                        if (Year > 0)
                        {
                            SqlParameter[] lParameters = new SqlParameter[3];
                            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                            SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);
                            SqlParameter param3 = new SqlParameter("@YEAR", DbType.Int32);
                            //Check if he was in BIS  prior to completion of a credited hour on or after 12/25/1999
                            var BIScounter = from item in iclbWorkHistory where item.year == BISCheckYear select item.istrBisParticipantFlag;
                            if (BIScounter.IsNotNull() && BIScounter.Count() > 0 && Convert.ToString(BIScounter.First()) == busConstant.FLAG_YES) //Indicates he is a BIS participant before earning that hour
                            {
                                // get Date (Year) after 12/26/1999 (essentially which year after 1999) in which he earned 1 Vested Year                                 
                                var YearofVesting = from item in iclbWorkHistory where item.year > 1999 && item.vested_years_count >= 5 && item.year == aintWorkItemYear && item.vested_hours >= 400 orderby item.year ascending select item.year;
                                if (!(YearofVesting.IsNullOrEmpty()))
                                {
                                    int YearofV = (Int32)YearofVesting.First();
                                    if (astrplan_code == busConstant.MPIPP)
                                    {
                                        if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                        {
                                            //PIR 933
                                            //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                            //lParameters[0] = param1;

                                            //param2.Value = busConstant.MPIPP;
                                            //lParameters[1] = param2;

                                            //param3.Value = YearofV;
                                            //lParameters[2] = param3;

                                            //if (ablnSSNMerge == false)
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                                            //else
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                            //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                            //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                            //else
                                            //    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);

                                            if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                            {
                                                DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", YearofV);
                                                if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                                }
                                                else
                                                {
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                                }
                                            }
                                            else
                                            {
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                            }
                                        }
                                        else
                                        {
                                            idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == YearofV).ToArray();

                                            Decimal Counter = Decimal.Zero;
                                            foreach (DataRow item in idrWeeklySubset)
                                            {
                                                Counter = Counter + Convert.ToDecimal(item["PensionHours"]);
                                                if (Counter >= 400.0M)
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                    break;
                                                }
                                            }

                                            if (Counter < 400.0M || Counter == Decimal.Zero)
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                        }

                                        this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                        iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_5;
                                        this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                        AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_5);
                                    }
                                    else if (astrplan_code == busConstant.IAP)
                                    {
                                        if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                        {
                                            //PIR 933
                                            //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                            //lParameters[0] = param1;

                                            //param2.Value = busConstant.IAP;
                                            //lParameters[1] = param2;

                                            //param3.Value = YearofV;
                                            //lParameters[2] = param3;

                                            //if (ablnSSNMerge == false)
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                                            //else
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                            //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                            //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                            //else
                                            //    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);


                                            if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                            {
                                                DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", YearofV);
                                                if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                                }
                                                else
                                                {
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                                }
                                            }
                                            else
                                            {
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                            }
                                        }
                                        else
                                        {
                                            idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == YearofV).ToArray();

                                            Decimal Counter = Decimal.Zero;
                                            foreach (DataRow item in idrWeeklySubset)
                                            {
                                                Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                                if (Counter >= 400.0M)
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                    break;
                                                }
                                            }

                                            if (Counter < 400.0M || Counter == Decimal.Zero)
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                        }

                                        this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                        this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                        iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_5;
                                        AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_5);
                                    }

                                    return true;
                                }
                            }
                            else
                            {
                                var YearofVesting = from item in iclbWorkHistory where item.year >= Year && item.vested_years_count >= 5 && item.year == aintWorkItemYear orderby item.year select item.year;
                                if (!(YearofVesting.IsNullOrEmpty()))
                                {
                                    int YearofV = (Int32)YearofVesting.First();
                                    if (astrplan_code == busConstant.MPIPP)
                                    {
                                        if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                        {
                                            //PIR 933
                                            //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                            //lParameters[0] = param1;

                                            //param2.Value = busConstant.MPIPP;
                                            //lParameters[1] = param2;

                                            //param3.Value = YearofV;
                                            //lParameters[2] = param3;

                                            //if (ablnSSNMerge == false)
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                                            //else
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                            //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                            //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                            //else
                                            //    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV); // PIR 548


                                            if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                            {
                                                DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", YearofV);
                                                if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                                }
                                                else
                                                {
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                                }
                                            }
                                            else
                                            {
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                            }
                                        }
                                        else
                                        {
                                            idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == YearofV).ToArray();

                                            Decimal Counter = Decimal.Zero;
                                            foreach (DataRow item in idrWeeklySubset)
                                            {
                                                Counter = Counter + Convert.ToDecimal(item["PensionHours"]);
                                                if (Counter >= 400.0M)
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                    break;
                                                }
                                            }

                                            if (Counter < 400.0M || Counter == Decimal.Zero)
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                        }

                                        this.icdoBenefitApplication.istrIsPersonVestedinMPI = busConstant.Flag_Yes;
                                        iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_5;
                                        this.icdoBenefitApplication.adtMPIVestingDate = ldtDateOfVesting;
                                        AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_5);
                                    }
                                    else if (astrplan_code == busConstant.IAP)
                                    {
                                        if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                        {
                                            //PIR 933
                                            //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                            //lParameters[0] = param1;

                                            //param2.Value = busConstant.IAP;
                                            //lParameters[1] = param2;

                                            //param3.Value = YearofV;
                                            //lParameters[2] = param3;

                                            //if (ablnSSNMerge == false)
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                                            //else
                                            //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                            //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                            //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                            //else
                                            //    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV); // PIR 548


                                            if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                            {
                                                DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", YearofV);
                                                if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                                }
                                                else
                                                {
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                                }
                                            }
                                            else
                                            {
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                            }
                                        }
                                        else
                                        {
                                            idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == YearofV).ToArray();

                                            Decimal Counter = Decimal.Zero;
                                            foreach (DataRow item in idrWeeklySubset)
                                            {
                                                Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                                if (Counter >= 400.0M)
                                                {
                                                    ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                    break;
                                                }
                                            }

                                            if (Counter < 400.0M || Counter == Decimal.Zero)
                                                ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(YearofV);
                                        }

                                        this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                        iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_5;
                                        this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                        AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_5);
                                    }

                                    return true;
                                }

                            }
                        }
                    }
                }
                #endregion

                #region RULE-6
                //RULE-6 -IAP only Effective 08/01/2000
                //•	1 Qualified Year
                //•	If participant incurs a Break in Service prior to completion of a credited hour on or after 08/01/2000, 
                //they will not vest until they earn 400 Credited Hours (1 Qualified Year)in a single Computation Year on or after 12/26/1999
                if (VestingRule.eligibility_type_value == busConstant.RULE_6)
                {
                    if (astrplan_code == busConstant.IAP)
                    {
                        //lcdoVestingRule = (cdoBenefitProvisionEligibility)iclbAllVestingRules.Where(rule => rule.eligibility_type_value.ToString() == busConstant.RULE_6).First();
                        if (VestingRule.IsNotNull())
                        {
                            DataRow[] idrWeeklySubset;
                            //Query to get Date (Year) where Person's  got IAP hours >=1 and where date >= 8/1/2000
                            //Essentially Query EADB to find out when did the Person got his first Hours (IAP Hour) on or after DATE 8/1/2000
                            DateTime CompareDate = new DateTime();
                            DateTime ComputationEndDateforCompareDate = new DateTime();
                            int Year = 0;

                            SqlParameter[] lParametersIAP = new SqlParameter[3];
                            SqlParameter param1IAP = new SqlParameter("@SSN", DbType.String);
                            SqlParameter param2IAP = new SqlParameter("@PlanCode", DbType.String);
                            SqlParameter param3IAP = new SqlParameter("@GivenDate", DbType.DateTime);

                            param1IAP.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                            lParametersIAP[0] = param1IAP;

                            param2IAP.Value = astrplan_code;
                            lParametersIAP[1] = param2IAP;

                            param3IAP.Value = new DateTime(2000, 08, 01);
                            lParametersIAP[2] = param3IAP;

                            if (this.idrWeeklyWorkData.IsNullOrEmpty())
                            {
                                if (ablnSSNMerge == false)
                                    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate1HourAftGivenDate", astrLegacyDBConnetion, null, lParametersIAP);
                                else
                                    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDate1HourAftGivenDate", astrLegacyDBConnetion, aobjPassInfo, lParametersIAP);
                            }
                            else
                            {
                                idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToDateTime(i["FromDate"]) >= new DateTime(2000, 08, 01)).ToArray();

                                Decimal Counter = Decimal.Zero;
                                foreach (DataRow item in idrWeeklySubset)
                                {
                                    Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                    if (Counter >= 1.0M)
                                    {
                                        ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                        //ldtbVestingDate.Rows.Add(item);
                                        break;
                                    }
                                }
                            }

                            if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                CompareDate = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                            else if (ldtDateOfVesting.IsNotNull() && ldtDateOfVesting != DateTime.MinValue)
                                CompareDate = ldtDateOfVesting;

                            if (CompareDate.IsNotNull() && CompareDate != DateTime.MinValue)
                            {
                                ComputationEndDateforCompareDate = busGlobalFunctions.GetLastDateOfComputationYear(CompareDate.Year);

                                if (ComputationEndDateforCompareDate >= CompareDate)
                                    Year = CompareDate.Year - 1;
                                else
                                    Year = CompareDate.Year;
                            }

                            if (CompareDate != DateTime.MinValue && Year > 0)
                            {

                                SqlParameter[] lParameters = new SqlParameter[3];
                                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                                SqlParameter param2 = new SqlParameter("@PLANCODE", DbType.String);
                                SqlParameter param3 = new SqlParameter("@YEAR", DbType.Int32);
                                //Check if he was in BIS  prior to completion of a credited hour on or after 08/01/2000, 

                                //PIR 856
                                var BIScounter = from item in iclbWorkHistory where item.year == Year select item.istrBisParticipantFlag;
                                if (BIScounter.IsNotNull() && BIScounter.Count() > 0 && BIScounter.First().ToString() == busConstant.FLAG_YES) //Indicates he is a BIS participant before earning that hour
                                {
                                    // get Date (Year) after 12/26/1999 (essentially which year after 1999) in which he earned 400 Qualified Hours 
                                    int Yearof400 = 0;
                                    var Yearof400Hoursafter1999 = from item in iclbWorkHistory where item.year > 1999 && item.qualified_hours >= 400 && item.year == aintWorkItemYear orderby item.year ascending select item.year;
                                    if (!Yearof400Hoursafter1999.IsNullOrEmpty())
                                    {
                                        Yearof400 = Yearof400Hoursafter1999.First();

                                        if (Year == Yearof400)
                                        {
                                            if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                            {
                                                //PIR 933
                                                //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                                //lParameters[0] = param1;

                                                //param2.Value = busConstant.IAP;
                                                //lParameters[1] = param2;

                                                //param3.Value = Year;
                                                //lParameters[2] = param3;

                                                //if (ablnSSNMerge == false)
                                                //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                                                //else
                                                //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                                //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                                //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                                //else
                                                //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);


                                                if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                                {
                                                    DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", Year);
                                                    if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                                    }
                                                    else
                                                    {
                                                        ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                                    }
                                                }
                                                else
                                                {
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                                }
                                            }
                                            else
                                            {
                                                idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == Year).ToArray();

                                                Decimal Counter = Decimal.Zero;
                                                foreach (DataRow item in idrWeeklySubset)
                                                {
                                                    Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                                    if (Counter >= 400.0M)
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                        break;
                                                    }
                                                }

                                                if (Counter < 400.0M || Counter == Decimal.Zero)
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                            }

                                            this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                            this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                            iclbWorkHistory.Where(item => item.year == ldtDateOfVesting.Year).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_6;
                                            AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_6);
                                            return true;
                                        }
                                        else if (Yearof400 < Year)
                                        {
                                            ldtDateOfVesting = CompareDate;
                                            this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                            this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                            iclbWorkHistory.Where(item => item.year == ldtDateOfVesting.Year).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_6;
                                            AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_6);
                                            return true;
                                        }
                                        else
                                        {
                                            Year = Yearof400;
                                            if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                            {
                                                //PIR 933
                                                //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                                //lParameters[0] = param1;

                                                //param2.Value = busConstant.IAP;
                                                //lParameters[1] = param2;

                                                //param3.Value = Year;
                                                //lParameters[2] = param3;

                                                //if (ablnSSNMerge == false)
                                                //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                                                //else
                                                //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                                //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                                //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                                //else
                                                //    ldtDateOfVesting = ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);

                                                if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                                {
                                                    DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", Year);
                                                    if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                                    }
                                                    else
                                                    {
                                                        ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                                    }
                                                }
                                                else
                                                {
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                                }
                                            }
                                            else
                                            {
                                                idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == Year).ToArray();
                                                Decimal Counter = Decimal.Zero;
                                                foreach (DataRow item in idrWeeklySubset)
                                                {
                                                    Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                                    if (Counter >= 400.0M)
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                        break;
                                                    }
                                                }

                                                if (Counter < 400.0M || Counter == Decimal.Zero)
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);

                                            }

                                            this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                            this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                            iclbWorkHistory.Where(item => item.year == ldtDateOfVesting.Year).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_6;
                                            AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_6);
                                            return true;
                                        }
                                    }

                                }

                                else
                                {
                                    var YearofVesting = from item in iclbWorkHistory where item.year >= CompareDate.Year && item.qualified_years_count >= VestingRule.qualified_years && item.year == aintWorkItemYear orderby item.year select item.year;
                                    if (!(YearofVesting.IsNullOrEmpty()))
                                    {
                                        Year = (Int32)YearofVesting.First();
                                        int Yearof400 = 0;
                                        var Yearof400Hoursafter1999 = from item in iclbWorkHistory where item.year > 1999 && item.qualified_hours >= 400 orderby item.year ascending select item.year;
                                        if (!Yearof400Hoursafter1999.IsNullOrEmpty())
                                        {
                                            Yearof400 = Yearof400Hoursafter1999.First();
                                        }

                                        if (Yearof400 != 0 && Yearof400 < CompareDate.Year)
                                        {
                                            ldtDateOfVesting = CompareDate;
                                        }
                                        else
                                        {
                                            if (this.idrWeeklyWorkData.IsNullOrEmpty())
                                            {
                                                //PIR 933
                                                //param1.Value = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
                                                //lParameters[0] = param1;

                                                //param2.Value = busConstant.IAP;
                                                //lParameters[1] = param2;

                                                //param3.Value = Year;
                                                //lParameters[2] = param3;

                                                //if (ablnSSNMerge == false)
                                                //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, null, lParameters);
                                                //else
                                                //    ldtbVestingDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetVestingDateAft400Hours", astrLegacyDBConnetion, aobjPassInfo, lParameters);

                                                //if (ldtbVestingDate.Rows.Count > 0 && ldtbVestingDate.Rows[0][0] != DBNull.Value)
                                                //    ldtDateOfVesting = Convert.ToDateTime(ldtbVestingDate.Rows[0][0]);
                                                //else
                                                //    ldtDateOfVesting = ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);

                                                if (adtVestingPlanYearDate != null && adtVestingPlanYearDate.Rows.Count > 0)
                                                {
                                                    DataRow[] ldrVestingPlanYearDate = adtVestingPlanYearDate.FilterTable(utlDataType.Numeric, "ComputationYear", Year);
                                                    if (ldrVestingPlanYearDate != null && ldrVestingPlanYearDate.Length > 0 && !Convert.ToString(ldrVestingPlanYearDate[0]["VESTING_DATE"]).IsNullOrEmpty())
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(ldrVestingPlanYearDate[0]["VESTING_DATE"]);
                                                    }
                                                    else
                                                    {
                                                        ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                                    }
                                                }
                                                else
                                                {
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                                }
                                            }
                                            else
                                            {
                                                idrWeeklySubset = this.idrWeeklyWorkData.Where(i => Convert.ToInt32(i["ComputationYear"]) == Year).ToArray();
                                                Decimal Counter = Decimal.Zero;
                                                foreach (DataRow item in idrWeeklySubset)
                                                {
                                                    Counter = Counter + Convert.ToDecimal(item["IAPHours"]);
                                                    if (Counter >= 400.0M)
                                                    {
                                                        ldtDateOfVesting = Convert.ToDateTime(item["ToDate"]);
                                                        break;
                                                    }
                                                }

                                                if (Counter < 400.0M || Counter == Decimal.Zero)
                                                    ldtDateOfVesting = busGlobalFunctions.GetLastDateOfComputationYear(Year);
                                            }

                                        }

                                        this.icdoBenefitApplication.istrIsPersonVestedinIAP = busConstant.FLAG_YES;
                                        this.icdoBenefitApplication.adtIAPVestingDate = ldtDateOfVesting;
                                        iclbWorkHistory.Where(item => item.year == YearofVesting.First()).First().comments += busConstant.VESTED_COMMENT + busConstant.RULE_6;
                                        AddUpdatePersonAccountEligibility(ldtDateOfVesting, astrplan_code, busConstant.RULE_6);
                                        return true;
                                    }


                                }
                            }
                        }
                    }
                }
                #endregion

                #endregion
            }

            return false;
        }

        //Code-Abhishek
        private void AddUpdatePersonAccountEligibility(DateTime adtVestingDate, string astrplan_code, string astrVestingRule)
        {
            busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };

            int lintAccountId = this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == astrplan_code).First().icdoPersonAccount.person_account_id;

            if (lintAccountId > 0)
            {
                //#region Code 4 Normal
                DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });

                if (ldtbPersonAccountEligibility.Rows.Count <= 0)
                {
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.person_account_id = lintAccountId;
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date = adtVestingDate;
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date = DateTime.Now;
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.vesting_rule = astrVestingRule;
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.Insert();

                    //IMP-Code to Take Care of the 23 People whos fate of Locals has not been determined as yet
                    foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                    {
                        if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP)
                        {
                            if (!CheckAlreadyVested(lbusPersonAccount.icdoPersonAccount.istrPlanCode) && !CheckIfForfeited(lbusPersonAccount.icdoPersonAccount.person_account_id))
                            {
                                lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date = adtVestingDate;
                                lbusPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date = DateTime.Now;
                                lbusPersonAccountEligibility.icdoPersonAccountEligibility.vesting_rule = astrVestingRule;
                                lbusPersonAccountEligibility.icdoPersonAccountEligibility.Update();
                            }

                        }
                    }
                }
                else
                {
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                    DateTime adtOldVestingDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;

                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date = adtVestingDate;
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date = DateTime.Now;
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.vesting_rule = astrVestingRule;
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.Update();

                    //IMP-Code to Take Care of the 23 People whos fate of Locals has not been determined as yet
                    foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                    {
                        if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP)
                        {
                            if (CheckAlreadyVested(lbusPersonAccount.icdoPersonAccount.istrPlanCode) && adtOldVestingDate.IsNotNull() && adtOldVestingDate != DateTime.MinValue && adtOldVestingDate == lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date)
                            {
                                lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date = adtVestingDate;
                                lbusPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date = DateTime.Now;
                                lbusPersonAccountEligibility.icdoPersonAccountEligibility.vesting_rule = astrVestingRule;
                                lbusPersonAccountEligibility.icdoPersonAccountEligibility.Update();
                            }

                        }
                    }

                }
                
            }
        }
        
        private void AddUpdateForfeitureDate(int Year, string astrPlanCode, bool Not_Vested)
        {
            busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
            DateTime LastDateofComputationYear = busGlobalFunctions.GetLastDateOfComputationYear(Year);
            
            if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                int lintAccountId = this.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == astrPlanCode).First().icdoPersonAccount.person_account_id;
                if (lintAccountId > 0)
                {
                    //#region 4 normal
                    DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });

                    if (ldtbPersonAccountEligibility.Rows.Count <= 0)
                    {
                        lbusPersonAccountEligibility.icdoPersonAccountEligibility.person_account_id = lintAccountId;
                        lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date = LastDateofComputationYear;
                        if (Not_Vested)
                        {
                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date = DateTime.MinValue;
                        }
                        lbusPersonAccountEligibility.icdoPersonAccountEligibility.Insert();
                    }
                    else
                    {
                        lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                        lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date = LastDateofComputationYear;
                        if (Not_Vested)
                        {
                            lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date = DateTime.MinValue;
                        }
                        lbusPersonAccountEligibility.icdoPersonAccountEligibility.Update();
                    }
                    
                }
            }
        }


        private bool IsVestingDateNull(string astrPlanCode)
        {
            int lintPersonAccountId = ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == astrPlanCode).First().icdoPersonAccount.person_account_id;
            if (lintPersonAccountId > 0)
            {
                DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintPersonAccountId });
                if (ldtbPersonAccountEligibility.Rows.Count > 0 && ldtbPersonAccountEligibility.Rows[0]["VESTED_DATE"] != DBNull.Value)
                {
                    DateTime lstrVestingDate = Convert.ToDateTime(ldtbPersonAccountEligibility.Rows[0]["VESTED_DATE"]);
                    if (lstrVestingDate.IsNotNull() && lstrVestingDate != DateTime.MinValue)
                        return false;
                }
            }

            return true;
        }
        #endregion

        public void CheckIfQualifiedSpouseinDeath()
        {
            DataTable ldtbLatestSpouseInfo = busBase.Select("cdoRelationship.GetLatestDateofMarriageIfQualifiedSpouseExists", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtbLatestSpouseInfo.Rows.Count > 0 && ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()].ToString().IsNotNullOrEmpty())
            {
                int Spouse_Person_ID = Convert.ToInt32(ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()]);

                int QualifiedDROExists = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckCountofApprovedDROforPersonandPayee", new object[2] { Spouse_Person_ID, this.ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //IF COUNT query Returns 1 then we know there is a DRO and hence no Qualified Spouse Exists
                if (QualifiedDROExists == 0)
                {
                    this.QualifiedSpouseExists = true;
                }
            }
        }

        public void SetupPrerequisitesDeath()
        {
            if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.DetermineBenefitSubTypeandEligibility_DeathPreRetirement();

                if (this.QualifiedSpouseExists)
                {
                    DataTable ldtbBenAccounts = busBase.Select("cdoPersonAccountBeneficiary.GetPlanfoLatestrQualifiedSpouse", new object[1] { this.ibusPerson.icdoPerson.person_id });
                    if (ldtbBenAccounts.Rows.Count > 0)
                    {
                        lclbBenPlans = cdoPlan.GetCollection<cdoPlan>(ldtbBenAccounts);

                        if (Eligible_Plans.Count > lclbBenPlans.Count)
                            this.SurvivorEntitled4MorePlans = true;
                        else
                            this.SurvivorEntitled4MorePlans = false;
                    }
                }
            }
        }

        public void GetPayeeAccountsInApprovedOrReviewSatus()
        {
            DataTable ldtPayeeAccount = busBase.Select("cdoBenefitApplication.GetPayeeAccountsInApproved&ReviewedStatus", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtPayeeAccount.Rows.Count > 0)
            {
                iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtPayeeAccount, "icdoPayeeAccount");
            }
        }

        public Collection<cdoPlanBenefitXr> GetBenefitOptionsForPreRetirementDeathPostElection(int aintPlanID)
        {
            Collection<cdoPlanBenefitXr> lclcBenefitOptions = new Collection<cdoPlanBenefitXr>();
            if (!this.iclbPayeeAccount.IsNullOrEmpty())
            {
                if (this.iclbPayeeAccount.Where(lbusP => lbusP.icdoPayeeAccount.iintPlanId == aintPlanID).Count() > 0)
                {
                    busPayeeAccount lbusPayeeAccount = (from lbusPayee in this.iclbPayeeAccount
                                                        where lbusPayee.icdoPayeeAccount.iintPlanId ==
                                                         aintPlanID
                                                        select lbusPayee).FirstOrDefault();
                    lbusPayeeAccount.LoadBenefitDetails();
                    if (lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate != DateTime.MinValue)
                    {
                        if (this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                        {
                            if (this.ibusPerson.icdoPerson.date_of_death.AddDays(90) >= lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate)
                            {

                                string lstrBenefitOptionValue = lbusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue;
                                if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.MPIPP_PLAN_ID || lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.LOCAL_161_PLAN_ID || lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.LOCAL_700_PLAN_ID ||
                                    lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.LOCAL_666_PLAN_ID || lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.LOCAL_600_PLAN_ID)
                                {
                                    DataTable ldtblist = busBase.Select("cdoBenefitApplicationDetail.GetBenefitOptionsForDeathPreRetPostElect90", new object[2] { lstrBenefitOptionValue, aintPlanID });
                                    if (ldtblist.IsNotNull() && ldtblist.Rows.Count > 0)
                                    {
                                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtblist);
                                        if (lstrBenefitOptionValue == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY || lstrBenefitOptionValue == busConstant.JOINT_50_PERCENT_SURVIVOR_ANNUITY)
                                        {
                                            lclcBenefitOptions.Remove(lclcBenefitOptions.Where(item => item.benefit_option_value == lstrBenefitOptionValue).FirstOrDefault());
                                        }
                                        else if (lstrBenefitOptionValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
                                        {
                                            if (this.ibusPerson.icdoPerson.date_of_death.AddDays(30) < lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate)
                                            {
                                                lclcBenefitOptions.Remove(lclcBenefitOptions.Where(item => item.benefit_option_value == lstrBenefitOptionValue).FirstOrDefault());
                                            }
                                        }
                                    }
                                }
                                else if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                                {
                                    DataTable ldtblist = busBase.Select("cdoBenefitApplicationDetail.GetBenefitOptionsForIAP", new object[] { });
                                    if (ldtblist.IsNotNull() && ldtblist.Rows.Count > 0)
                                    {
                                        lclcBenefitOptions = doBase.GetCollection<cdoPlanBenefitXr>(ldtblist);
                                    }
                                }
                                else
                                {
                                    lclcBenefitOptions = this.lclcL52BenOptionsPreDeath;
                                    if (lclcBenefitOptions.Where(item => item.benefit_option_value == lstrBenefitOptionValue).Count() == 0)
                                    {
                                        busPlanBenefitXr lbusPlan = new busPlanBenefitXr { icdoPlanBenefitXr = new cdoPlanBenefitXr() };
                                        lbusPlan.FindPlanBenefitXr(lbusPayeeAccount.icdoPayeeAccount.plan_benefit_id);
                                        lclcBenefitOptions.Add(lbusPlan.icdoPlanBenefitXr);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return lclcBenefitOptions;
        }

        public void LoadPayeeAccountStatusByApplicationID(int aintApplicationID)
        {
            DataTable ldtbList = busBase.Select("cdoPayeeAccount.GetPayeeAccountStatusByApplicationID", new object[1] { aintApplicationID });
            iclbPayeeAccountStatusByApplication = GetCollection<busPayeeAccountStatus>(ldtbList, "icdoPayeeAccountStatus");
        }

        public void LoadApproveRetirementApplication(int aintPersonid)
        {
            Collection<busBenefitApplication> lclbBenefitApplication = new Collection<busBenefitApplication>();

            DataTable ldtbList = Select<cdoBenefitApplication>(
               new string[2] { enmBenefitApplication.person_id.ToString(), enmBenefitApplication.application_status_value.ToString() },
               new object[2] { aintPersonid, busConstant.BENEFIT_APPL_APPROVED }, null, null);
            lclbBenefitApplication = GetCollection<busBenefitApplication>(ldtbList, "icdoBenefitApplication");

            if (!lclbBenefitApplication.IsNullOrEmpty())
            {
                this.icdoBenefitApplication = lclbBenefitApplication[0].icdoBenefitApplication;
            }

        }

        //Sid Jain 06152013
        public void LoadApproveRetirementApplicationForHealthActuary(int aintPersonid)
        {
            Collection<busBenefitApplication> lclbBenefitApplication = new Collection<busBenefitApplication>();
            DataTable ldtbList = busBase.Select("cdoBenefitApplication.GetApprovedMPIApplication", new object[1] { aintPersonid });

            //DataTable ldtbList = Select<cdoBenefitApplication>(
            //   new string[2] { enmBenefitApplication.person_id.ToString(), enmBenefitApplication.application_status_value.ToString() },
            //   new object[2] { aintPersonid, busConstant.BENEFIT_APPL_APPROVED }, null, null);

            lclbBenefitApplication = GetCollection<busBenefitApplication>(ldtbList, "icdoBenefitApplication");

            if (!lclbBenefitApplication.IsNullOrEmpty())
            {
                foreach (busBenefitApplication lobjApplication in lclbBenefitApplication)
                {
                    if (lobjApplication.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_RETIREMENT || lobjApplication.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DISABILITY ||
                        lobjApplication.icdoBenefitApplication.benefit_type_value == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                    {
                        this.icdoBenefitApplication = lobjApplication.icdoBenefitApplication;
                        break;
                    }
                }

            }

        }

        /// <summary>
        /// Check 
        /// </summary>
        /// <param name="aintPersonID"></param>
        /// <returns></returns>
        public busBenefitApplication LoadWithdrawalDataByPersonId(int aintPersonID)
        {

            DataTable ldtbList = busBase.Select("cdoBenefitApplication.CheckIfApprovedWithdrawalRecordExistsForMPI", new object[1] { aintPersonID });
            Collection<busBenefitApplication> lclbbusBenefitApplication = GetCollection<busBenefitApplication>(ldtbList, "icdoBenefitApplication");

            foreach (busBenefitApplication lbusBenefitApplication in lclbbusBenefitApplication)
            {
                DataTable ldtbWithdrawalBuybackData = busBase.Select("cdoBenefitApplication.GetWithdrawalBuyBackRecord", new object[2] { aintPersonID, 
                                                         lbusBenefitApplication.icdoBenefitApplication.withdrawal_date.Year});

                if (ldtbWithdrawalBuybackData.Rows.Count == 0)
                {
                    return lclbbusBenefitApplication[0];
                }
            }

            return null;
        }

        public busBenefitApplication LoadWithdrawalDataByPersonIdBefore1976(int aintPersonID)
        {
            DataTable ldtbList = busBase.Select("cdoBenefitApplication.CheckIfApprovedWithdrawalRecordExistsForMPIBefore1976", new object[1] { aintPersonID });

            Collection<busBenefitApplication> lclbbusBenefitApplication = GetCollection<busBenefitApplication>(ldtbList, "icdoBenefitApplication");

            foreach (busBenefitApplication lbusBenefitApplication in lclbbusBenefitApplication)
            {
                DataTable ldtbWithdrawalBuybackData = busBase.Select("cdoBenefitApplication.GetWithdrawalBuyBackRecord", new object[2] { aintPersonID, lbusBenefitApplication.icdoBenefitApplication.withdrawal_date.Year });

                if (ldtbWithdrawalBuybackData.Rows.Count == 0)
                {
                    return lclbbusBenefitApplication[0];

                }
            }

            return null;
        }

        #region For ABS and Person Overview Plan Summary
        public void ProcessBISandForfietureforABSandPersonOverview(Collection<cdoDummyWorkData> aclbPersonWorkHistory, int aintLocalQualifiedYearsCount)
        {
            string istrIsBISParticipant = busConstant.FLAG_NO;
            string lstrIsForfieture = busConstant.FLAG_NO;
            int BISCounter = 0;
            bool lblnBadWorkYear = false;
            int lintNonQualifiedWorkYearCounter = 0;
            int first_year = 0;
            int lintWithDrawalYearBefore1976 = 0;
            int lintYearMPIPlanBegins = 0;
            int lintTotalqualifiedYearsCount = 0;
            int lintTotalvestedYearsCount = 0;

            foreach (cdoDummyWorkData item in aclbPersonWorkHistory)
            {
                if (aintLocalQualifiedYearsCount != 0)
                {
                    lintTotalqualifiedYearsCount = aintLocalQualifiedYearsCount;
                    lintTotalvestedYearsCount = aintLocalQualifiedYearsCount;
                    aintLocalQualifiedYearsCount = 0;
                }
                else
                {
                    #region Algorithm to Make-Up the in-memory WorkHistory Table

                    item.iblnWithdrawalReset = false;
                    if (item.vested_hours != 0)
                    {
                        lstrIsForfieture = busConstant.FLAG_NO;
                    }

                    if (lstrIsForfieture == busConstant.FLAG_NO)
                    {
                        first_year++;
                    }

                    if (item.qualified_hours >= 400)
                    {
                        lintTotalqualifiedYearsCount++;
                        item.qualified_years_count = lintTotalqualifiedYearsCount;
                        istrIsBISParticipant = busConstant.FLAG_NO;
                        item.istrBisParticipantFlag = busConstant.FLAG_NO;
                        lintNonQualifiedWorkYearCounter = 0;
                    }
                    else
                    {
                        item.qualified_years_count = lintTotalqualifiedYearsCount;
                        item.vested_years_count = lintTotalvestedYearsCount;
                    }

                    if (item.vested_hours >= 200)
                    {
                        if (item.vested_hours >= 400)
                        {
                            lintTotalvestedYearsCount++;
                            item.vested_years_count = lintTotalvestedYearsCount;
                            item.qualified_years_count = lintTotalqualifiedYearsCount;
                            lintNonQualifiedWorkYearCounter = 0;
                        }
                        else
                        {
                            item.qualified_years_count = lintTotalqualifiedYearsCount;
                            item.vested_years_count = lintTotalvestedYearsCount;
                        }

                        item.bis_years_count = 0;
                        BISCounter = 0;

                    }

                    if (item.vested_hours < 400 //&& item.vested_hours >= 200 
                        && item.year >= lintYearMPIPlanBegins && lstrIsForfieture == busConstant.FLAG_NO)

                        if (item.iblnBridgeServiceFlag == true && item.vested_hours == 200)
                        {
                            lintNonQualifiedWorkYearCounter = 0;
                        }
                        else
                        {
                            lintNonQualifiedWorkYearCounter++;

                        }

                    if (item.vested_hours < 400 && item.year >= lintYearMPIPlanBegins && istrIsBISParticipant == busConstant.FLAG_YES)
                        item.istrBisParticipantFlag = busConstant.FLAG_YES;

                    if (item.vested_hours < 200 && item.year >= lintYearMPIPlanBegins && lintYearMPIPlanBegins != 0)
                        lblnBadWorkYear = true;
                    else
                        lblnBadWorkYear = false;


                    if ((lblnBadWorkYear || lintNonQualifiedWorkYearCounter > 0 || (lintWithDrawalYearBefore1976 > 0 && lintWithDrawalYearBefore1976 == item.year)) && lstrIsForfieture == busConstant.FLAG_NO && item.year >= lintYearMPIPlanBegins)
                    {
                        if (lblnBadWorkYear)
                        {
                            //Additional Check to See the Locals as well.
                            if (item.vested_hours < 200)
                                BISCounter++;
                            item.bis_years_count = BISCounter;
                            item.qualified_years_count = lintTotalqualifiedYearsCount;
                            item.vested_years_count = lintTotalvestedYearsCount;
                        }

                        if (lstrIsForfieture == busConstant.FLAG_NO)
                        {
                            if (lintNonQualifiedWorkYearCounter >= 2 && item.year < 1976 && istrIsBISParticipant == busConstant.FLAG_YES)
                            {
                                lintTotalqualifiedYearsCount = 0;
                                lintTotalvestedYearsCount = 0;
                                BISCounter = 0;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                first_year = 0;
                                item.qualified_years_count = lintTotalqualifiedYearsCount;
                                item.vested_years_count = lintTotalvestedYearsCount;
                                item.istrForfietureFlag = busConstant.FLAG_YES;
                            }

                            else if (lintWithDrawalYearBefore1976 > 0 && lintWithDrawalYearBefore1976 == item.year)
                            {
                                if (item.vested_hours >= 400)
                                {
                                    lintTotalqualifiedYearsCount = 1;
                                    lintTotalvestedYearsCount = 1;
                                    BISCounter = 0;
                                    first_year = 0;
                                    item.iblnWithdrawalReset = true;
                                    item.qualified_years_count = lintTotalqualifiedYearsCount;
                                    item.vested_years_count = lintTotalvestedYearsCount;
                                }
                                else
                                {
                                    item.iblnWithdrawalReset = true;
                                    lintTotalqualifiedYearsCount = 0;
                                    lintTotalvestedYearsCount = 0;
                                    BISCounter = 0;
                                    lstrIsForfieture = busConstant.FLAG_YES;
                                    first_year = 0;
                                    item.qualified_years_count = lintTotalqualifiedYearsCount;
                                    item.vested_years_count = lintTotalvestedYearsCount;
                                    item.istrForfietureFlag = busConstant.FLAG_YES;
                                }
                            }

                            else if (lintNonQualifiedWorkYearCounter >= lintTotalvestedYearsCount && item.year <= 1985 && item.year >= 1976 && BISCounter >= 2 && istrIsBISParticipant == busConstant.FLAG_YES)
                            {
                                lintTotalqualifiedYearsCount = 0;
                                lintTotalvestedYearsCount = 0;
                                BISCounter = 0;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                first_year = 0;
                                item.qualified_years_count = lintTotalqualifiedYearsCount;
                                item.vested_years_count = lintTotalvestedYearsCount;
                                item.istrForfietureFlag = busConstant.FLAG_YES;
                            }

                            else if (lintTotalvestedYearsCount >= 5 && lintNonQualifiedWorkYearCounter >= lintTotalvestedYearsCount && item.year >= 1986 && istrIsBISParticipant == busConstant.FLAG_YES)
                            {
                                lintTotalqualifiedYearsCount = 0;
                                lintTotalvestedYearsCount = 0;
                                BISCounter = 0;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                first_year = 0;
                                item.qualified_years_count = lintTotalqualifiedYearsCount;
                                item.vested_years_count = lintTotalvestedYearsCount;
                                item.istrForfietureFlag = busConstant.FLAG_YES;
                            }

                            else if (lintTotalvestedYearsCount < 5 && lintNonQualifiedWorkYearCounter >= 5 && item.year >= 1986 && istrIsBISParticipant == busConstant.FLAG_YES)
                            {
                                lintTotalqualifiedYearsCount = 0;
                                lintTotalvestedYearsCount = 0;
                                BISCounter = 0;
                                lstrIsForfieture = busConstant.FLAG_YES;
                                first_year = 0;
                                item.qualified_years_count = lintTotalqualifiedYearsCount;
                                item.vested_years_count = lintTotalvestedYearsCount;
                                item.istrForfietureFlag = busConstant.FLAG_YES;
                            }
                        }
                    }

                    #endregion
                }
            }
        }
        #endregion

        public void CancelIAPFinalPendingCalculations(int BenefitApplicationId)
        {
            DataTable ldtbBenefitCalHeader = busBase.Select("cdoBenefitCalculationHeader.GetBenefitCalculationHeader", new object[2] { BenefitApplicationId, "PEND" });
            Collection<cdoBenefitCalculationHeader> iclbBenefitCalculationHeader = cdoDummyWorkData.GetCollection<cdoBenefitCalculationHeader>(ldtbBenefitCalHeader);
            foreach (cdoBenefitCalculationHeader lbusBenefitCalc in iclbBenefitCalculationHeader)
            {
                if (lbusBenefitCalc.calculation_type_value == "FINL" && lbusBenefitCalc.status_value == "PEND")
                {
                    lbusBenefitCalc.status_value = "CANC";
                    lbusBenefitCalc.Update();
                }

            }
        }

        public bool CheckIfManager()
        {
            busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
            if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
            {
                return true;
            }
            return false;
        }

        public ArrayList btnSubmit()
        {
            ArrayList iarrError = new ArrayList();
            try
            {
                iarrError.AddRange(CompleteBpmActivityInstance("submit"));
            }
            catch (Exception ex)
            {
                utlError lutlError = GetErrorObject(utlMessageType.Framework, 10016, ex.Message);
                iarrError.Add(lutlError);
            }
            return iarrError;
        }

    }
}
